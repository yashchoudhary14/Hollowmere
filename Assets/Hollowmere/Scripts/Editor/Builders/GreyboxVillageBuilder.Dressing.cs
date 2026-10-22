using UnityEngine;
using UnityEngine.Rendering;
using static Hollowmere.EditorTools.BuilderKit;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Set dressing for the look pass (M1.5): furniture and clutter in the houses, yards and back-garden fences,
    /// dead trees, rocks and fallen wood in the forest, fences along the fog road, cobwebs.
    /// Everything here is visual only. Models import without colliders and dressing primitives are made without
    /// them, so movement, interaction rays, the Hollow's hearing and sight, and the NavMesh bake are exactly as in
    /// M1 (LookPassTests checks the gameplay layout against the M1 snapshot). Fixed seeds make it deterministic,
    /// and it never draws from the RNGs that place gameplay or the forest's trunks.
    /// </summary>
    public static partial class GreyboxVillageBuilder
    {
        const int DressingSeed = 5150;
        const float FloorTop = 0.06f; // the house floor slab's top

        static Material s_cobweb;
        static GameObject[] s_deadTrees;

        static void BeginDressing()
        {
            s_cobweb = PropKit.CobwebMaterial();
            s_deadTrees = BuildDeadTreePrefabs();
        }

        static float Range(System.Random rng, float a, float b) => a + (float)rng.NextDouble() * (b - a);
        static bool Chance(System.Random rng, float p) => rng.NextDouble() < p;
        static T Pick<T>(System.Random rng, params T[] items) => items[rng.Next(items.Length)];

        // ------------------------------------------------------------ houses

        /// <summary>
        /// One house's interior and yard. Every house shares the same plan (see BuildHouse), so the spots below are
        /// fixed clear places against walls, away from doors, windows, tables, switches and the puzzle; the house's
        /// seed decides what stands in each.
        /// </summary>
        static void DressHouse(Transform root, House house)
        {
            var rng = new System.Random(DressingSeed + house.Index * 101);
            var d = Node(root, "Dressing");
            float inX = HouseW * 0.5f - WallT, inZ = HouseD * 0.5f - WallT;
            float innerN = InnerWallZ + WallT * 0.5f, innerS = InnerWallZ - WallT * 0.5f;
            float hearthX = house.Hearth.transform.localPosition.x;

            // Hall (front, z innerN..inZ)
            var hall = Node(d, "Hall");
            PropKit.Model(hall, "Chair", PropKit.Chair, new Vector3(-4.4f, FloorTop, 1.95f), Range(rng, -12f, 12f));
            if (Chance(rng, 0.75f)) PropKit.Model(hall, "Bookshelf", PropKit.Bookshelf, new Vector3(-1.35f, FloorTop, innerN + 0.31f), 0f);
            else PropKit.Model(hall, "Chest", PropKit.Chest, new Vector3(-1.35f, FloorTop, innerN + 0.36f), 90f);
            CornerClutter(hall, rng, new Vector3(-5.2f, FloorTop, 3.75f));
            PropKit.Model(hall, "Stool", PropKit.Stool, new Vector3(5.2f, FloorTop, 1.7f), Range(rng, 0f, 360f));
            if (Chance(rng, 0.6f)) PropKit.Model(hall, "FloorLamp", PropKit.FloorLamp, new Vector3(5.3f, FloorTop, 3.85f), Range(rng, 0f, 360f));
            Picture(hall, rng, new Vector3(-1.5f, 1.75f, inZ), 90f);
            var cloth = Mat("Cloth", new Color(0.45f, 0.3f, 0.25f));
            Box(hall, "Rug", new Vector3(2.6f, FloorTop + 0.004f, 2.65f), new Vector3(1.7f, 0.008f, 1.1f), cloth, false, Quaternion.Euler(0f, Range(rng, -6f, 6f), 0f));
            Box(hall, "Tablecloth", new Vector3(4.6f, 0.754f, 2.7f), new Vector3(0.96f, 0.006f, 0.66f), cloth, false); // under the side table's battery
            PropKit.Model(hall, "Horseshoe", PropKit.Horseshoe, new Vector3(0f, 2.36f, inZ - 0.03f), 0f, 1.3f, Quaternion.Euler(90f, 0f, 0f));
            Beams(hall, new[] { -3f, 3f }, innerN, inZ);
            Cobwebs(hall, rng, -inX, inX, innerN, inZ);

            // Hearth room (back left, x -inX..-0.15)
            var hr = Node(d, "HearthRoom");
            if (Chance(rng, 0.8f)) PropKit.Model(hr, "Bed", PropKit.Bed, new Vector3(-0.89f, FloorTop, -inZ + 1.04f), 0f);
            else PropKit.Model(hr, "Chest", PropKit.Chest, new Vector3(-0.55f, FloorTop, -inZ + 0.7f), 0f);
            PropKit.Model(hr, "RockingChair", PropKit.RockingChair, new Vector3(-4.55f, FloorTop, -2.25f), 121f + Range(rng, -10f, 10f));
            if (Chance(rng, 0.4f)) PropKit.Model(hr, "Footstool", PropKit.Footstool, new Vector3(-4.0f, FloorTop, -2.75f), Range(rng, 0f, 360f));
            LogPile(hr, rng, new Vector3(-5.05f, FloorTop, -3.75f));
            CornerClutter(hr, rng, new Vector3(-5.2f, FloorTop, 0.45f));
            PropKit.Model(hr, "OilLamp", PropKit.OilLamp, new Vector3(-4.72f, 0.75f, -0.82f), Range(rng, 0f, 360f), 0.55f);
            var mantel = new Vector3(hearthX, 1.42f, -inZ + 0.45f);
            PropKit.Model(hr, "Candleholders", PropKit.Candleholders, mantel + new Vector3(-0.6f, 0f, 0f), 0f, 0.8f);
            PropKit.Model(hr, "Candlestick", PropKit.Candlestick, mantel + new Vector3(1.12f, 0f, 0.05f), 0f);
            PropKit.Model(hr, "MantelClock", PropKit.MantelClock, mantel + new Vector3(0.28f, 0f, 0.02f), Range(rng, -8f, 8f)); // clear of the mantel's item spot at +0.75
            Picture(hr, rng, new Vector3(-0.15f, 1.95f, -3.2f), 180f);
            Beams(hr, new[] { -1.6f, -4.3f }, -inZ, innerS);
            Cobwebs(hr, rng, -inX, -0.15f, -inZ, innerS);

            // Puzzle room (back right, x 0.15..inX)
            var pr = Node(d, "PuzzleRoom");
            PropKit.Model(pr, "Chair", PropKit.Chair, new Vector3(1.0f, FloorTop, -1.75f), 180f + Range(rng, -12f, 12f));
            if (Chance(rng, 0.6f)) PropKit.Model(pr, "Bookshelf", PropKit.Bookshelf, new Vector3(0.15f + 0.31f, FloorTop, -0.2f), 90f);
            else CornerClutter(pr, rng, new Vector3(0.6f, FloorTop, 0.4f));
            CornerClutter(pr, rng, new Vector3(5.25f, FloorTop, 0.45f));
            var shelf = PropKit.Model(pr, "WallShelf", PropKit.WallShelf, new Vector3(inX, 1.55f, -3.0f), 180f);
            if (shelf != null)
            {
                PropKit.Model(pr, "Books", PropKit.Books, new Vector3(inX - 0.1f, 1.79f, -2.95f), 90f, 0.8f);
                PropKit.Model(pr, "Jug", PropKit.Jug, new Vector3(inX - 0.1f, 1.79f, -3.35f), Range(rng, 0f, 360f));
            }
            Picture(pr, rng, new Vector3(1.2f, 1.8f, innerS), -90f);
            Beams(pr, new[] { 1.4f, 4.4f }, -inZ, innerS);
            Cobwebs(pr, rng, 0.15f, inX, -inZ, innerS);

            DressYard(root, house, rng);
        }

        /// <summary>The chapel: a tall chair behind the lectern, facing the door, clear of the Hollow's night spawn.</summary>
        static void DressChapel(Transform chapel)
        {
            var d = Node(chapel, "Dressing");
            PropKit.Model(d, "TallChair", PropKit.TallChair, new Vector3(-5.1f, FloorTop, -3.4f), 90f);
        }

        /// <summary>A barrel, a crate stack or buckets in a corner.</summary>
        static void CornerClutter(Transform parent, System.Random rng, Vector3 pos)
        {
            switch (rng.Next(4))
            {
                case 0:
                    PropKit.Model(parent, "Barrel", PropKit.Barrel, pos, Range(rng, 0f, 360f));
                    break;
                case 1:
                    PropKit.Model(parent, "WineBarrel", PropKit.WineBarrel, pos, Range(rng, 0f, 360f), 0.9f);
                    break;
                case 2:
                    float yaw = Range(rng, -20f, 20f) + Pick(rng, 0f, 90f);
                    PropKit.Model(parent, "Crate", PropKit.Crate, pos, yaw);
                    if (Chance(rng, 0.6f)) PropKit.Model(parent, "CrateTop", PropKit.Crate, pos + Vector3.up * 0.35f, yaw + Range(rng, -25f, 25f));
                    break;
                default:
                    PropKit.Model(parent, "Bucket", PropKit.WoodenBucket, pos, Range(rng, 0f, 360f));
                    PropKit.Model(parent, "MetalBucket", PropKit.MetalBucket, pos + new Vector3(0.35f, 0f, -0.3f), Range(rng, 0f, 360f));
                    break;
            }
        }

        /// <summary>A family picture on a wall. pos is the centre on the wall face; yaw turns the frame's face (+x) into the room.</summary>
        static void Picture(Transform parent, System.Random rng, Vector3 pos, float yaw)
        {
            if (!Chance(rng, 0.85f)) return;
            float s = Range(rng, 0.55f, 0.8f);
            var turn = Quaternion.Euler(0f, yaw, 0f);
            var normal = turn * Vector3.right;
            if (PropKit.Model(parent, "Picture", PropKit.PictureFrame, pos - Vector3.up * 0.45f * s + normal * 0.005f, yaw, s) == null) return;
            // The frame comes empty: an old darkened canvas inside it.
            Box(parent, "Canvas", pos + normal * 0.014f, new Vector3(0.012f, 0.8f * s, 0.55f * s), Mat("Canvas", new Color(0.16f, 0.13f, 0.1f), 0.35f), false, turn);
        }

        /// <summary>Ceiling beams running along z between two walls, under the ceiling slab.</summary>
        static void Beams(Transform parent, float[] xs, float z0, float z1)
        {
            foreach (var x in xs)
                PropKit.Fit(parent, $"Beam_{x:0.0}", PropKit.Beam,
                    new Bounds(new Vector3(x, HouseH - 0.135f, (z0 + z1) * 0.5f), new Vector3(0.3f, 0.27f, z1 - z0 + 0.1f)));
        }

        /// <summary>Two or three logs stacked by the hearth, cut from the fallen trunk model.</summary>
        static void LogPile(Transform parent, System.Random rng, Vector3 pos)
        {
            var pile = Node(parent, "Logs", pos, Quaternion.Euler(0f, Range(rng, 80f, 100f), 0f));
            PropKit.Fit(pile, "Log0", PropKit.FallenTrunk, new Bounds(new Vector3(-0.08f, 0.07f, 0f), new Vector3(0.6f, 0.14f, 0.14f)));
            PropKit.Fit(pile, "Log1", PropKit.FallenTrunk, new Bounds(new Vector3(0.08f, 0.07f, 0.02f), new Vector3(0.6f, 0.14f, 0.14f)), 180f);
            if (Chance(rng, 0.7f))
                PropKit.Fit(pile, "Log2", PropKit.FallenTrunk, new Bounds(new Vector3(0f, 0.19f, 0.01f), new Vector3(0.55f, 0.13f, 0.13f)));
        }

        /// <summary>
        /// Cobwebs in some of a room's upper corners: a sagging sheet whose fan starts in the corner under the ceiling
        /// (PropKit.CobwebMesh), mirrored to fit whichever corner it is in.
        /// </summary>
        static void Cobwebs(Transform parent, System.Random rng, float x0, float x1, float z0, float z1)
        {
            var mesh = PropKit.CobwebMesh();
            var corners = new[] { new Vector3(x0, 0f, z0), new Vector3(x1, 0f, z0), new Vector3(x0, 0f, z1), new Vector3(x1, 0f, z1) };
            foreach (var c in corners)
            {
                if (!Chance(rng, 0.55f)) continue;
                float size = Range(rng, 0.45f, 0.85f);
                float sx = Mathf.Sign((x0 + x1) * 0.5f - c.x), sz = Mathf.Sign((z0 + z1) * 0.5f - c.z);
                var web = Node(parent, "Cobweb", new Vector3(c.x + sx * 0.01f, HouseH - 0.005f, c.z + sz * 0.01f));
                web.localScale = new Vector3(sx * size, size, sz * size);
                web.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
                var r = web.gameObject.AddComponent<MeshRenderer>();
                r.sharedMaterial = s_cobweb;
                r.shadowCastingMode = ShadowCastingMode.Off;
            }
        }

        /// <summary>Clutter by the front door and corners, tools, a lantern by the door, and a fenced back garden.</summary>
        static void DressYard(Transform root, House house, System.Random rng)
        {
            var yard = Node(root.Find("Dressing"), "Yard");
            float hx = HouseW * 0.5f, hz = HouseD * 0.5f;
            CornerClutter(yard, rng, new Vector3(-hx + 0.55f, 0f, hz + 0.55f));
            CornerClutter(yard, rng, new Vector3(hx - 0.6f, 0f, hz + 0.55f));
            if (Chance(rng, 0.5f))
                PropKit.Model(yard, "CartWheel", PropKit.CartWheel, new Vector3(-hx - 0.22f, 0f, Range(rng, -2f, 2f)), 0f, 1f, Quaternion.Euler(0f, 0f, 78f));
            if (Chance(rng, 0.6f))
                PropKit.Model(yard, "Axe", PropKit.Axe, new Vector3(2.4f, 0f, hz + 0.17f), Range(rng, -20f, 20f), 1f, Quaternion.Euler(-12f, 0f, 0f));
            if (Chance(rng, 0.5f))
                PropKit.Model(yard, "WateringCan", PropKit.WateringCan, new Vector3(hx + 0.4f, 0f, Range(rng, -3f, 0f)), Range(rng, 0f, 360f));
            if (Chance(rng, 0.5f))
                PropKit.Model(yard, "Hammer", PropKit.Hammer, new Vector3(-hx + 1.4f, 0f, hz + 0.7f), Range(rng, 0f, 360f), 1f, Quaternion.Euler(0f, 0f, 90f)); // left lying by the corner
            if (Chance(rng, 0.5f))
                PropKit.Model(yard, "Bucket", PropKit.WoodenBucket, new Vector3(-1.45f, 0f, hz + 0.4f), Range(rng, 0f, 360f));

            // A lantern on a bracket beside the door, between the door frame and the stopped clock.
            var lantern = Node(yard, "DoorLantern", new Vector3(1.0f, 0f, hz));
            Box(lantern, "Bracket", new Vector3(0f, 2.52f, 0.1f), new Vector3(0.03f, 0.03f, 0.2f), Mat("Metal", new Color(0.22f, 0.23f, 0.25f), 0.5f), collider: false);
            PropKit.Model(lantern, "Lantern", PropKit.Lantern, new Vector3(0f, 2.21f, 0.18f), Range(rng, -20f, 20f));

            // Back garden: an overgrown patch inside a broken post-and-rail fence.
            float gz0 = -hz, gz1 = -hz - 8f, gx = 9f;
            Box(yard, "Garden", new Vector3(0f, 0.008f, (gz0 + gz1) * 0.5f), new Vector3(2f * gx - 0.4f, 0.016f, gz0 - gz1 - 0.3f), Mat("Garden", new Color(0.25f, 0.3f, 0.18f)), collider: false);
            var fence = Node(yard, "Fence");
            FenceRun(fence, rng, new Vector3(-gx, 0f, gz0 - 0.3f), new Vector3(-gx, 0f, gz1), 0.25f);
            FenceRun(fence, rng, new Vector3(-gx, 0f, gz1), new Vector3(gx, 0f, gz1), 0.25f);
            FenceRun(fence, rng, new Vector3(gx, 0f, gz1), new Vector3(gx, 0f, gz0 - 0.3f), 0.25f);
            if (Chance(rng, 0.5f))
                PropKit.Model(yard, "Branches", PropKit.Branches, new Vector3(Range(rng, -6f, 6f), 0f, Range(rng, gz1 + 1.5f, gz0 - 1.5f)), Range(rng, 0f, 360f));
        }

        /// <summary>
        /// A post-and-rail fence from a to b with posts about every 2.2 m. Some posts lean, some rails are missing,
        /// whole bays are broken out with probability broken. No colliders: it is scenery.
        /// </summary>
        static void FenceRun(Transform parent, System.Random rng, Vector3 a, Vector3 b, float broken)
        {
            var planks = Mat("Planks", new Color(0.35f, 0.3f, 0.25f));
            float length = Vector3.Distance(a, b);
            int bays = Mathf.Max(1, Mathf.RoundToInt(length / 2.2f));
            var dir = (b - a) / length;
            var along = Quaternion.LookRotation(dir);
            Vector3 Post(int i) => a + dir * (length * i / bays);
            for (int i = 0; i <= bays; i++)
            {
                if (Chance(rng, 0.08f)) continue; // a missing post
                var lean = Quaternion.Euler(Range(rng, -6f, 6f), 0f, Range(rng, -6f, 6f));
                var p = Post(i);
                Box(parent, "Post", p + lean * new Vector3(0f, 0.6f, 0f), new Vector3(0.11f, 1.2f, 0.11f), planks, false, along * lean);
            }
            for (int i = 0; i < bays; i++)
            {
                if (Chance(rng, broken)) continue;
                var mid = (Post(i) + Post(i + 1)) * 0.5f;
                float bay = length / bays;
                foreach (var h in new[] { 0.45f, 0.95f })
                {
                    if (Chance(rng, 0.2f)) continue;
                    var sag = Quaternion.Euler(Range(rng, -3f, 3f), 0f, 0f);
                    Box(parent, "Rail", mid + Vector3.up * (h + Range(rng, -0.04f, 0.04f)), new Vector3(0.05f, 0.11f, bay + 0.1f), planks, false, along * sag);
                }
            }
        }

        // ------------------------------------------------------------ trees, forest floor, road, wall

        const string DeadTreeDir = "Assets/Hollowmere/Prefabs/Trees";
        const int DeadTreeVariants = 12;
        const float DeadTreeHeight = 10f; // the variants are built for a 10 m trunk and scaled to each tree

        /// <summary>
        /// Builds the dead-limb prefabs the forest's trees share (a scene with thousands of loose limbs would be a
        /// very large file). Rebuilt on every build, so their GUIDs and the trees' references stay put.
        /// </summary>
        static GameObject[] BuildDeadTreePrefabs()
        {
            EnsureFolder(DeadTreeDir);
            var prefabs = new GameObject[DeadTreeVariants];
            for (int v = 0; v < DeadTreeVariants; v++)
            {
                var temp = new GameObject($"DeadLimbs_{v:00}");
                DeadLimbs(temp.transform, new System.Random(DressingSeed * 31 + v), DeadTreeHeight);
                prefabs[v] = UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, $"{DeadTreeDir}/DeadLimbs_{v:00}.prefab");
                Object.DestroyImmediate(temp);
            }
            return prefabs;
        }

        /// <summary>
        /// Dead limbs for the tree whose trunk BuildForest made (the trunk keeps its collider): one of the shared
        /// variants, turned and scaled to the trunk's height. Seeded by the tree's number, so it never shifts where
        /// the trunks stand.
        /// </summary>
        static void DeadTree(Transform tree, int n, float h)
        {
            var rng = new System.Random(DressingSeed * 17 + n);
            var inst = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(s_deadTrees[rng.Next(DeadTreeVariants)], tree);
            inst.name = "Limbs";
            inst.transform.localPosition = Vector3.zero;
            inst.transform.localRotation = Quaternion.Euler(0f, Range(rng, 0f, 360f), 0f);
            inst.transform.localScale = Vector3.one * (h / DeadTreeHeight);
        }

        /// <summary>A root flare, a crooked leader above the trunk's top, and three to six limbs with twigs.</summary>
        static void DeadLimbs(Transform limbs, System.Random rng, float h)
        {
            var bark = Mat("Bark", new Color(0.24f, 0.2f, 0.16f));

            // Roots: thick limbs splaying out and down into the ground around the trunk's foot.
            int roots = rng.Next(4, 7);
            float rootAz = Range(rng, 0f, 360f);
            for (int i = 0; i < roots; i++)
            {
                float az = (rootAz + 360f * i / roots + Range(rng, -20f, 20f)) * Mathf.Deg2Rad;
                var outward = new Vector3(Mathf.Cos(az), 0f, Mathf.Sin(az));
                var dir = (outward - Vector3.up * Range(rng, 0.55f, 0.85f)).normalized;
                Limb(limbs, $"Root{i}", outward * 0.1f + Vector3.up * Range(rng, 0.25f, 0.4f), dir, Range(rng, 0.6f, 0.9f), Range(rng, 0.2f, 0.28f), bark);
            }

            // The trunk thins into a crooked leader above its top: segments, each starting where the last ended.
            var at = new Vector3(0f, h - 0.15f, 0f);
            var dirUp = Vector3.up;
            float leaderDia = 0.36f;
            int segments = rng.Next(2, 4);
            for (int i = 0; i < segments; i++)
            {
                dirUp = (dirUp + new Vector3(Range(rng, -0.35f, 0.35f), 0f, Range(rng, -0.35f, 0.35f))).normalized;
                float len = Range(rng, 0.9f, 1.8f);
                Limb(limbs, $"Leader{i}", at, dirUp, len, leaderDia, bark);
                at += dirUp * (len - leaderDia * 0.3f);
                leaderDia *= 0.62f;
            }

            int count = rng.Next(3, 7);
            for (int i = 0; i < count; i++)
            {
                float y = h * Range(rng, 0.45f, 0.95f);
                float az = Range(rng, 0f, 360f) * Mathf.Deg2Rad;
                float tilt = Range(rng, 35f, 70f) * Mathf.Deg2Rad;
                var dir = new Vector3(Mathf.Cos(az) * Mathf.Sin(tilt), Mathf.Cos(tilt), Mathf.Sin(az) * Mathf.Sin(tilt));
                float len = Range(rng, 1.4f, 3.4f) * (1.15f - y / h * 0.5f);
                float dia = Range(rng, 0.09f, 0.17f);
                var start = new Vector3(0f, y, 0f) + new Vector3(dir.x, 0f, dir.z).normalized * 0.2f;
                Limb(limbs, $"Limb{i}", start, dir, len, dia, bark);
                int twigs = rng.Next(1, 3);
                for (int k = 0; k < twigs; k++)
                {
                    var from = start + dir * len * Range(rng, 0.55f, 0.9f);
                    var tdir = (dir + new Vector3(Range(rng, -0.8f, 0.8f), Range(rng, 0.1f, 0.6f), Range(rng, -0.8f, 0.8f))).normalized;
                    Limb(limbs, $"Twig{i}_{k}", from, tdir, len * Range(rng, 0.3f, 0.5f), dia * 0.45f, bark);
                }
            }
        }

        /// <summary>A collider-less cylinder from start along dir.</summary>
        static void Limb(Transform parent, string name, Vector3 start, Vector3 dir, float length, float diameter, Material mat)
        {
            Cyl(parent, name, start + dir * (length * 0.5f), diameter, length, mat, false, Quaternion.FromToRotation(Vector3.up, dir));
        }

        /// <summary>
        /// Rocks, stumps, fallen trunks and dry branches in the forest band and inside the ring wall, and broken fences
        /// along both edges of the fog road, where its invisible walls are.
        /// </summary>
        static void DressGrounds(Transform village)
        {
            var rng = new System.Random(DressingSeed + 7);
            var grounds = Node(village, "Dressing");

            // Leaf litter over the ground beyond the wall: four thin sheets whose inner edges hide behind the wall's foot.
            var litter = Mat("LeafLitter", new Color(0.3f, 0.25f, 0.18f));
            const float edge0 = Half + 0.6f, outer = 220f, south = -280f, north = 480f;
            Box(grounds, "LeafLitterNorth", new Vector3(0f, 0.01f, (edge0 + north) * 0.5f), new Vector3(2f * outer, 0.02f, north - edge0), litter, false);
            Box(grounds, "LeafLitterSouth", new Vector3(0f, 0.01f, (south - edge0) * 0.5f), new Vector3(2f * outer, 0.02f, -edge0 - south), litter, false);
            Box(grounds, "LeafLitterWest", new Vector3(-(edge0 + outer) * 0.5f, 0.01f, 0f), new Vector3(outer - edge0, 0.02f, 2f * edge0), litter, false);
            Box(grounds, "LeafLitterEast", new Vector3((edge0 + outer) * 0.5f, 0.01f, 0f), new Vector3(outer - edge0, 0.02f, 2f * edge0), litter, false);

            // Forest floor outside the wall (the trees' band).
            var forest = Node(grounds, "ForestFloor");
            int n = 0;
            for (float x = -180f; x <= 180f; x += 14f)
            for (float z = -180f; z <= 180f; z += 14f)
            {
                var p = new Vector3(x + Range(rng, -5f, 5f), 0f, z + Range(rng, -5f, 5f));
                bool outsideWall = Mathf.Abs(p.x) > 157f || Mathf.Abs(p.z) > 157f;
                bool nearRoad = Mathf.Abs(p.x) < 12f && p.z > 140f;
                if (!outsideWall || nearRoad || !Chance(rng, 0.55f)) continue;
                ForestPiece(forest, rng, $"F{n++:000}", p);
            }

            // Rocks and old wood along the inside of the ring wall, away from the gate, square and chapel.
            var wallSide = Node(grounds, "WallSide");
            n = 0;
            for (float s = -140f; s <= 140f; s += 18f)
            {
                var sides = new[]
                {
                    (pos: new Vector3(s, 0f, -Half + 3f), north: false), (pos: new Vector3(-Half + 3f, 0f, s), north: false),
                    (pos: new Vector3(Half - 3f, 0f, s), north: false), (pos: new Vector3(s, 0f, Half - 3f), north: true),
                };
                foreach (var side in sides)
                {
                    var p = side.pos + new Vector3(Range(rng, -6f, 6f), 0f, Range(rng, -6f, 6f)) * 0.3f;
                    if (side.north && p.x > -85f && p.x < 45f) continue;           // chapel, square and gate
                    if (p.z > 85f && Mathf.Abs(p.x) < 45f) continue;               // the square
                    if (!Chance(rng, 0.45f)) continue;
                    ForestPiece(wallSide, rng, $"W{n++:000}", p);
                }
            }

            // Fog road: broken fences on the lines where its invisible walls stand.
            var road = Node(grounds, "RoadFences");
            const float edge = 6.55f, z0 = Half + 3f, z1 = 378f;
            FenceRun(Node(road, "West"), rng, new Vector3(-edge, 0f, z0), new Vector3(-edge, 0f, z1), 0.3f);
            FenceRun(Node(road, "East"), rng, new Vector3(edge, 0f, z0), new Vector3(edge, 0f, z1), 0.3f);
            for (float z = z0 + 6f; z < z1; z += 9f)
            {
                if (!Chance(rng, 0.35f)) continue;
                float side = Chance(rng, 0.5f) ? -1f : 1f;
                PropKit.Model(road, "Branches", PropKit.Branches, new Vector3(side * Range(rng, 7.5f, 10f), 0f, z + Range(rng, -3f, 3f)), Range(rng, 0f, 360f), Range(rng, 0.8f, 1.4f));
            }
        }

        static void ForestPiece(Transform parent, System.Random rng, string name, Vector3 p)
        {
            float yaw = Range(rng, 0f, 360f);
            switch (rng.Next(6))
            {
                case 0: PropKit.Model(parent, name + "_Stump", PropKit.Stump, p - Vector3.up * 0.12f, yaw, Range(rng, 0.7f, 1.2f)); break;
                case 1: PropKit.Model(parent, name + "_Trunk", PropKit.FallenTrunk, p, yaw, Range(rng, 1f, 1.8f)); break;
                case 2: PropKit.Model(parent, name + "_Boulder", PropKit.Boulder, p - Vector3.up * 0.15f, yaw, Range(rng, 0.6f, 1.3f)); break;
                case 3: PropKit.Model(parent, name + "_Rocks", PropKit.MossRocks, p - Vector3.up * 0.2f, yaw, Range(rng, 0.5f, 0.9f)); break;
                default: PropKit.Model(parent, name + "_Branches", PropKit.Branches, p, yaw, Range(rng, 0.9f, 1.6f)); break;
            }
        }

        // ------------------------------------------------------------ swapped visuals

        /// <summary>A table's look: top and four legs, inside the space of the M1 box (which keeps its collider).</summary>
        static void TableLook(Transform visual, GameObject box, Material wood)
        {
            PropKit.StripRenderer(box);
            var look = Node(visual, "Look");
            Box(look, "Top", new Vector3(0f, 0.725f, 0f), new Vector3(0.9f, 0.05f, 0.6f), wood, false);
            Box(look, "Apron", new Vector3(0f, 0.65f, 0f), new Vector3(0.8f, 0.1f, 0.5f), wood, false);
            for (int i = 0; i < 4; i++)
                Box(look, $"Leg{i}", new Vector3(i % 2 == 0 ? -0.39f : 0.39f, 0.35f, i < 2 ? -0.24f : 0.24f), new Vector3(0.06f, 0.7f, 0.06f), wood, false);
        }
    }
}
