using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using static Hollowmere.EditorTools.BuilderKit;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Village v2 (M1.6 P2): builds Assets/Hollowmere/Scenes/Village.unity from docs/design/village.json and the house
    /// files. Run it with tools/unity.ps1 run -Method Hollowmere.EditorTools.VillageBuilderV2.Build, then
    /// VillageBuilderV2.Shots for the review images. The playable M1 game stays in Greybox.unity until P3 moves the
    /// gameplay onto this ground plan.
    /// </summary>
    public static class VillageBuilderV2
    {
        public const string ScenePath = "Assets/Hollowmere/Scenes/Village.unity";
        public const string NavMeshPath = "Assets/Hollowmere/Scenes/Village_NavMesh.asset";
        public const string MeshPath = "Assets/Hollowmere/Art/Meshes/VillageV2Meshes.asset";        // ground, streets, water, roofs
        public const string BoxMeshPath = "Assets/Hollowmere/Art/Meshes/VillageV2BuilderMeshes.asset"; // BuilderKit's mesh cache for this scene

        [MenuItem("Hollowmere/Build Village v2 (M1.6)")]
        public static void Build() => GreyboxVillageBuilder.BuildVillageV2();

        /// <summary>Renders every v2 camera to Logs/claude/v2_*.png (top-down with plan labels and no fog).</summary>
        [MenuItem("Hollowmere/Village v2 Shots (M1.6)")]
        public static void Shots() => GreyboxVillageBuilder.VillageV2Shots();

        /// <summary>Renders each house to Logs/claude/house_Hxx_*.png: front and back three-quarter views (compare with
        /// docs/design/ref/Hxx_massing_*.png) and a cutaway straight down per level (compare with ref/Hxx_plan.png).</summary>
        [MenuItem("Hollowmere/Village v2 House Shots (M1.6)")]
        public static void HouseShots() => GreyboxVillageBuilder.VillageV2HouseShots();
    }

    /// <summary>
    /// The village v2 ground plan (M1.6 P2, docs/design/VILLAGE_LAYOUT.md): ground with the stream channel and pond,
    /// streets from the village.json polylines, bridges, the 15 houses as footprint massing at their position and yaw,
    /// filler buildings as closed shells, landmarks, plot walls, hedges and fences, lamps, scarecrows, the forest belt,
    /// the six night blockers (off by day) and the new roam and forest spawn points. Gate, square, chapel, ring wall,
    /// fog road and the outer forest come from the M1 builders unchanged. The geometry comes from VillagePlan.
    /// </summary>
    public static partial class GreyboxVillageBuilder
    {
        static Mesh s_v2Meshes;

        public static void BuildVillageV2()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var plan = VillagePlan.Make(DesignData.LoadVillage(), DesignData.LoadHouses(), DesignData.LoadCatalog());
            var housePlans = plan.Houses.Select(h => HousePlan.Make(h, plan.Village.FindHouse(h.Slug), plan.Catalog)).ToArray();

            Begin(VillageBuilderV2.BoxMeshPath);
            s_render = RenderSetup.Setup();
            BeginDressing();
            BeginV2Meshes();
            EnsureFolder("Assets/Hollowmere/Scenes");
            EnsureFolder("Assets/Hollowmere/Settings");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            s_planLabels = new GameObject(PlanLabelsName).transform;
            s_planLabels.gameObject.SetActive(false);

            var village = new GameObject("Village").transform; // the NavMesh bakes from everything under here
            BuildGroundV2(village, plan, GroundHoles(plan, housePlans));
            BuildRingWall(village);
            BuildGate(village);
            BuildRoad(village);
            BuildForest(village);
            BuildStreetsV2(village, plan);
            BuildWaterV2(village, plan);
            BuildHouseShellsV2(village, plan, housePlans);
            BuildFillerV2(village, plan);
            BuildLandmarksV2(village, plan);
            BuildBoundariesV2(village, plan);
            BuildLampsV2(village, plan);
            BuildScarecrowsV2(village, plan);
            BuildForestBeltV2(village, plan);
            BuildBlockersV2(village, plan);
            DressGrounds(village);
            BuildSquare(village);
            var chapelSpawn = BuildChapel(village, out _);
            BuildLayoutV2(plan, chapelSpawn);
            BuildLights();
            BuildShotCamerasV2();
            ApplyRenderSettings();

            SetAgentForHouses();
            AddNavMeshSurface(village);
            Object.FindAnyObjectByType<Unity.AI.Navigation.NavMeshSurface>().voxelSize = 0.08f; // a third of the agent radius
            BakeNavMesh(VillageBuilderV2.NavMeshPath);
            EditorUtility.SetDirty(s_v2Meshes);
            End();

            EditorSceneManager.SaveScene(scene, VillageBuilderV2.ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Hollowmere] VILLAGE V2 BUILT: {VillageBuilderV2.ScenePath} ({plan.Houses.Length} houses, {plan.Village.Filler.Length} filler, " +
                      $"{plan.Boundaries.Count} boundary runs, {plan.Trees.Count} belt trees, {plan.RoamPoints.Count} roam points)");
        }

        // ------------------------------------------------------------ procedural meshes (sub-assets of one file)

        static void BeginV2Meshes()
        {
            var path = VillageBuilderV2.MeshPath;
            s_v2Meshes = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (s_v2Meshes == null)
            {
                EnsureFolder(Path.GetDirectoryName(path).Replace('\\', '/'));
                s_v2Meshes = new Mesh { name = "VillageV2" }; // an empty main object holding the meshes below
                AssetDatabase.CreateAsset(s_v2Meshes, path);
            }
            foreach (var m in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Mesh>().ToArray())
                if (m != s_v2Meshes) Object.DestroyImmediate(m, true);
            s_gable = null;
            s_wedge = null;
        }

        static Mesh V2Mesh(string name, List<Vector3> verts, List<int> tris, List<Vector2> uvs)
        {
            var mesh = new Mesh { name = name };
            if (verts.Count > 65000) mesh.indexFormat = IndexFormat.UInt32;
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            AssetDatabase.AddObjectToAsset(mesh, s_v2Meshes);
            return mesh;
        }

        static GameObject MeshObject(Transform parent, string name, Mesh mesh, Material mat, bool collider,
                                     Vector3 pos = default, Quaternion? rot = null, Vector3? scale = null, bool shadows = true)
        {
            var t = Node(parent, name, pos, rot);
            if (scale.HasValue) t.localScale = scale.Value;
            t.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            var r = t.gameObject.AddComponent<MeshRenderer>();
            r.sharedMaterial = mat;
            if (!shadows) r.shadowCastingMode = ShadowCastingMode.Off;
            if (collider) t.gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
            return t.gameObject;
        }

        // ------------------------------------------------------------ ground

        /// <summary>
        /// The ground inside the wall as one mesh with a collider: 8 m tiles, flat tiles as a single quad, tiles near the
        /// stream or pond as a 1 m grid following VillagePlan.GroundHeight. Over cellar stairs the ground has holes: tiles
        /// touching one split down to 0.25 m and leave out what lies inside it. Outside the wall, the M1 ground continues as boxes.
        /// </summary>
        static void BuildGroundV2(Transform village, VillagePlan plan, List<Obb2> holes)
        {
            const float half = 152f, tile = 8f, fine = 1f;
            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();
            void Quad(float x0, float z0, float x1, float z1, bool flat)
            {
                int a = verts.Count;
                foreach (var (x, z) in new[] { (x0, z0), (x0, z1), (x1, z1), (x1, z0) })
                {
                    verts.Add(new Vector3(x, flat ? 0f : plan.GroundHeight(x, z), z));
                    uvs.Add(new Vector2(x, z));
                }
                tris.Add(a); tris.Add(a + 1); tris.Add(a + 2);
                tris.Add(a); tris.Add(a + 2); tris.Add(a + 3);
            }
            void Holed(float x0, float z0, float x1, float z1, bool flat)
            {
                var corners = new[] { new Vector2(x0, z0), new Vector2(x0, z1), new Vector2(x1, z1), new Vector2(x1, z0) };
                var mid = new Vector2((x0 + x1) * 0.5f, (z0 + z1) * 0.5f);
                if (holes.Any(hole => corners.All(c => hole.Contains(c)))) return;          // all inside: left out
                bool touches = holes.Any(hole => HoleTouches(hole, x0, z0, x1, z1));
                if (!touches) { Quad(x0, z0, x1, z1, flat); return; }
                if (x1 - x0 <= 0.26f) { if (!holes.Any(hole => hole.Contains(mid))) Quad(x0, z0, x1, z1, flat); return; }
                float xm = (x0 + x1) * 0.5f, zm = (z0 + z1) * 0.5f;
                Holed(x0, z0, xm, zm, flat); Holed(xm, z0, x1, zm, flat); Holed(x0, zm, xm, z1, flat); Holed(xm, zm, x1, z1, flat);
            }
            for (float z0 = -half; z0 < half - 0.01f; z0 += tile)
            for (float x0 = -half; x0 < half - 0.01f; x0 += tile)
            {
                float x1 = Mathf.Min(x0 + tile, half), z1 = Mathf.Min(z0 + tile, half);
                bool flat = TileFlat(plan, x0 - fine, z0 - fine, x1 + fine, z1 + fine, fine);
                bool holed = holes.Any(hole => HoleTouches(hole, x0, z0, x1, z1));
                if (flat || holed)
                {
                    if (holed) Holed(x0, z0, x1, z1, flat);
                    else Quad(x0, z0, x1, z1, true);
                    continue;
                }
                for (float z = z0; z < z1 - 0.01f; z += fine)
                for (float x = x0; x < x1 - 0.01f; x += fine)
                    Quad(x, z, x + fine, z + fine, false);
            }
            var ground = Mat("Ground", new Color(0.23f, 0.27f, 0.17f));
            MeshObject(village, "Ground", V2Mesh("Ground", verts, tris, uvs), ground, true);

            // Beyond the wall: the outer forest and the fog road stand on the M1 ground, cut round the village.
            var outside = Node(village, "GroundOutside");
            const float outer = 220f, south = -280f, north = 480f;
            Box(outside, "North", new Vector3(0f, -0.5f, (half + north) * 0.5f), new Vector3(2f * outer, 1f, north - half), ground);
            Box(outside, "South", new Vector3(0f, -0.5f, (south - half) * 0.5f), new Vector3(2f * outer, 1f, -half - south), ground);
            Box(outside, "West", new Vector3(-(half + outer) * 0.5f, -0.5f, 0f), new Vector3(outer - half, 1f, 2f * half), ground);
            Box(outside, "East", new Vector3((half + outer) * 0.5f, -0.5f, 0f), new Vector3(outer - half, 1f, 2f * half), ground);
        }

        static bool HoleTouches(Obb2 hole, float x0, float z0, float x1, float z1)
        {
            var c = hole.Corners();
            return c.Max(p => p.x) > x0 && c.Min(p => p.x) < x1 && c.Max(p => p.y) > z0 && c.Min(p => p.y) < z1;
        }

        /// <summary>World boxes over every cellar stair, where the village ground must open (HousePlan.GroundHoles).</summary>
        static List<Obb2> GroundHoles(VillagePlan plan, HousePlan[] housePlans)
        {
            var holes = new List<Obb2>();
            foreach (var hp in housePlans)
                foreach (var r in hp.GroundHoles)
                {
                    var box = VillagePlan.WingBox(hp.Place, hp.House.Slug + ".cellarstair", r);
                    holes.Add(new Obb2(box.Id, box.Centre, box.Size + new Vector2(0.1f, 0.1f), box.Yaw));
                }
            return holes;
        }

        /// <summary>
        /// Indoors the doors are 0.7-1.0 m wide (docs/design), so the NavMesh agent (the Humanoid type every surface bakes
        /// for) gets a 0.25 m radius and climbs 50-degree flights. Stored in ProjectSettings/NavMeshAreas.asset.
        /// </summary>
        static void SetAgentForHouses()
        {
            var builder = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AI.NavMeshBuilder");
            var settings = builder?.GetProperty("navMeshSettingsObject", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null) as Object;
            if (settings == null)
            {
                Debug.LogError("[Hollowmere] Couldn't reach the NavMesh agent settings: set Window > AI > Navigation > Agents > Humanoid to radius 0.25, max slope 50.");
                return;
            }
            var so = new SerializedObject(settings);
            var agents = so.FindProperty("m_Settings");
            if (agents == null || agents.arraySize == 0) { Debug.LogError("[Hollowmere] NavMesh agent settings not found."); return; }
            var humanoid = agents.GetArrayElementAtIndex(0);
            humanoid.FindPropertyRelative("agentRadius").floatValue = 0.25f;
            humanoid.FindPropertyRelative("agentSlope").floatValue = 50f;
            so.ApplyModifiedPropertiesWithoutUndo();
            Debug.Log("[Hollowmere] NavMesh agent (Humanoid): radius 0.25, max slope 50");
        }

        static bool TileFlat(VillagePlan plan, float x0, float z0, float x1, float z1, float step)
        {
            for (float z = z0; z <= z1 + 0.001f; z += step)
            for (float x = x0; x <= x1 + 0.001f; x += step)
                if (plan.GroundHeight(x, z) < -0.001f) return false;
            return true;
        }

        // ------------------------------------------------------------ streets

        static void BuildStreetsV2(Transform village, VillagePlan plan)
        {
            var root = Node(village, "Streets");
            for (int i = 0; i < plan.Roads.Count; i++)
            {
                var r = plan.Roads[i];
                var node = Node(root, $"{r.Id} {r.Name}");
                float y = 0.0005f * i; // roads of one kind meeting at a junction don't share a height
                switch (r.Kind)
                {
                    case "street": // VILLAGE_LAYOUT.md: setts, 1.5 m pavements, a kerb
                        Ribbon(node, "Pavement", plan, r.Points, r.Width + 3f, 0.03f + y, SurfaceMat("Paving"));
                        Ribbon(node, "Kerb", plan, r.Points, r.Width + 0.3f, 0.036f + y, SurfaceMat("Kerb"));
                        Ribbon(node, "Setts", plan, r.Points, r.Width, 0.042f + y, SurfaceMat("Cobbles2"));
                        break;
                    case "lane": // mud and leaves down the middle, gravel edges
                        Ribbon(node, "Edges", plan, r.Points, r.Width, 0.026f + y, SurfaceMat("GroundGravel"));
                        Ribbon(node, "Centre", plan, r.Points, r.Width - 1.2f, 0.032f + y, SurfaceMat("Road"));
                        break;
                    case "track": // mud with a grass strip
                        Ribbon(node, "Mud", plan, r.Points, r.Width, 0.022f + y, SurfaceMat("GroundMud"));
                        Ribbon(node, "GrassStrip", plan, r.Points, 0.7f, 0.028f + y, SurfaceMat("GroundGrassPath"));
                        break;
                    default:
                        Ribbon(node, "Path", plan, r.Points, r.Width, 0.018f + y, SurfaceMat("GroundGrassPath"));
                        break;
                }
            }
            // A flagged path from every front door to its road.
            var walks = Node(root, "DoorPaths");
            foreach (var d in plan.Doors)
                Ribbon(walks, d.Slug, plan, new[] { d.Door + d.Outward * 0.05f, d.RoadPoint }, 1.2f, 0.016f, SurfaceMat("PavingSlabs"));
        }

        /// <summary>A flat strip along a polyline, UVs in metres, no collider. Stretches over water are left out: a bridge carries them.</summary>
        static void Ribbon(Transform parent, string name, VillagePlan plan, IList<Vector2> pts, float width, float y, Material mat)
        {
            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();
            var wet = new List<bool>();
            float h = width * 0.5f, along = 0f;
            for (int i = 0; i < pts.Count; i++)
            {
                var dir = (pts[Mathf.Min(pts.Count - 1, i + 1)] - pts[Mathf.Max(0, i - 1)]).normalized;
                var left = new Vector2(-dir.y, dir.x);
                if (i > 0) along += Vector2.Distance(pts[i - 1], pts[i]);
                var l = pts[i] + left * h;
                var r = pts[i] - left * h;
                verts.Add(new Vector3(l.x, y, l.y));
                verts.Add(new Vector3(r.x, y, r.y));
                uvs.Add(new Vector2(-h, along));
                uvs.Add(new Vector2(h, along));
                wet.Add(plan.GroundHeight(l.x, l.y) < -0.01f || plan.GroundHeight(r.x, r.y) < -0.01f || plan.GroundHeight(pts[i].x, pts[i].y) < -0.01f);
            }
            for (int i = 0; i < pts.Count - 1; i++)
            {
                if (wet[i] || wet[i + 1]) continue;
                int l0 = 2 * i, r0 = l0 + 1, l1 = l0 + 2, r1 = l0 + 3;
                tris.Add(l0); tris.Add(l1); tris.Add(r1);
                tris.Add(l0); tris.Add(r1); tris.Add(r0);
            }
            MeshObject(parent, name, V2Mesh($"{parent.name}_{name}", verts, tris, uvs), mat, false, shadows: false);
        }

        // ------------------------------------------------------------ water and bridges

        static void BuildWaterV2(Transform village, VillagePlan plan)
        {
            var root = Node(village, "Water");
            var water = Mat("Water", new Color(0.05f, 0.07f, 0.07f), 0.6f); // P2 note: glossier water mirrored the sky white

            // Stream surface at the pond's level, a little into the banks.
            float surfaceHalf = plan.Stream.HalfWidth + VillagePlan.StreamBank * (VillagePlan.WaterY - plan.StreamBedY) / -plan.StreamBedY + 0.05f;
            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();
            var pts = plan.Stream.Points;
            for (int i = 0; i < pts.Length; i++)
            {
                var dir = (pts[Mathf.Min(pts.Length - 1, i + 1)] - pts[Mathf.Max(0, i - 1)]).normalized;
                var left = new Vector2(-dir.y, dir.x);
                var l = pts[i] + left * surfaceHalf;
                var r = pts[i] - left * surfaceHalf;
                verts.Add(new Vector3(l.x, VillagePlan.WaterY, l.y));
                verts.Add(new Vector3(r.x, VillagePlan.WaterY, r.y));
                uvs.Add(new Vector2(l.x, l.y));
                uvs.Add(new Vector2(r.x, r.y));
                if (i == 0 || (plan.PondE(pts[i - 1]) < 1f && plan.PondE(pts[i]) < 1f)) continue; // the pond has its own surface
                int l0 = 2 * (i - 1), r0 = l0 + 1, l1 = l0 + 2, r1 = l0 + 3;
                tris.Add(l0); tris.Add(l1); tris.Add(r1);
                tris.Add(l0); tris.Add(r1); tris.Add(r0);
            }
            MeshObject(root, "Stream", V2Mesh("Stream", verts, tris, uvs), water, false, shadows: false);

            // Mill pond (VILLAGE_LAYOUT.md L_POND): surface at -0.8 where the banks reach it.
            Ellipse(root, "Pond", new Vector3(plan.Pond.Centre.x, VillagePlan.WaterY, plan.Pond.Centre.y), plan.Pond.Size * 0.92f, plan.Pond.Yaw, water);

            // Invisible barriers along the banks and round the pond: nobody walks into the water, and the NavMesh
            // only crosses at the bridges.
            var banks = Node(root, "Barriers");
            foreach (var b in plan.WaterBarriers)
                InvisibleWall(banks, b, VillagePlan.BarrierHeight);

            // Where the stream passes under the ring wall: a grille closes the gap.
            var metal = Mat("Metal", new Color(0.22f, 0.23f, 0.25f), 0.5f);
            foreach (var end in new[] { plan.Stream.Control[0], plan.Stream.Control[plan.Stream.Control.Length - 1] })
                Box(root, "Culvert", new Vector3(Mathf.Sign(end.x) * Half, plan.StreamBedY * 0.5f, end.y),
                    new Vector3(RingWallThickness + 0.2f, -plan.StreamBedY + 0.1f, 2f * plan.StreamTopHalf + 0.6f), metal);

            var bridges = Node(root, "Bridges");
            foreach (var b in plan.Bridges) BuildBridgeV2(bridges, plan, b);
        }

        static void InvisibleWall(Transform parent, Obb2 box, float height)
        {
            var t = Node(parent, box.Id, new Vector3(box.Centre.x, height * 0.5f, box.Centre.y), Quaternion.Euler(0f, box.Yaw, 0f));
            t.gameObject.AddComponent<BoxCollider>().size = new Vector3(box.Size.x, height, box.Size.y);
        }

        /// <summary>B1 the stone road bridge with a pier between its two arches, B2 the timber footbridge over the mill race.</summary>
        static void BuildBridgeV2(Transform parent, VillagePlan plan, PlanBridge b)
        {
            var node = Node(parent, $"{b.Id} {b.Name}", new Vector3(b.Pos.x, 0f, b.Pos.y), Quaternion.Euler(0f, b.Yaw, 0f));
            float top = b.DeckTop;
            if (b.Foot)
            {
                var wood = Mat("BridgeWood", new Color(0.32f, 0.24f, 0.17f));
                Box(node, "Deck", new Vector3(0f, top - 0.075f, 0f), new Vector3(b.Width, 0.15f, b.Length), wood);
                foreach (float side in new[] { -1f, 1f })
                {
                    float x = side * (b.Width * 0.5f - 0.05f);
                    Box(node, side < 0f ? "RailW" : "RailE", new Vector3(x, top + 0.95f, 0f), new Vector3(0.08f, 0.08f, b.Length), wood, false);
                    for (int k = 0; k < 3; k++)
                        Box(node, $"Post{(side < 0f ? "W" : "E")}{k}", new Vector3(x, top + 0.5f, (k - 1) * (b.Length * 0.5f - 0.1f)), new Vector3(0.1f, 1f, 0.1f), wood, false);
                    Blocker(node, side < 0f ? "GuardW" : "GuardE", new Vector3(x, top + 0.5f, 0f), new Vector3(0.1f, 1f, b.Length));
                }
                Box(node, "Trestle", new Vector3(0f, (plan.StreamBedY + top) * 0.5f, 0f), new Vector3(b.Width, top - plan.StreamBedY, 0.25f), wood, false);
            }
            else
            {
                var stone = Mat("Stone", new Color(0.45f, 0.44f, 0.42f));
                Box(node, "Deck", new Vector3(0f, top - 0.25f, 0f), new Vector3(b.Width, 0.5f, b.Length), stone);
                foreach (float side in new[] { -1f, 1f })
                    Box(node, side < 0f ? "ParapetW" : "ParapetE", new Vector3(side * (b.Width * 0.5f - 0.2f), top + 0.5f, 0f), new Vector3(0.4f, 1f, b.Length), stone);
                Box(node, "Pier", new Vector3(0f, (plan.StreamBedY + top - 0.5f) * 0.5f, 0f), new Vector3(b.Width, top - 0.5f - plan.StreamBedY, 1f), stone);
            }
        }

        // ------------------------------------------------------------ roofs (houses: GreyboxVillageBuilder.HousesV2.cs)

        /// <summary>Words in the house text that name a material key but describe something else ("Ground floor: ...").</summary>
        static string Stoplisted(string text) =>
            System.Text.RegularExpressions.Regex.Replace(text ?? "", @"\b(Ground|Floor|Door)\b", "");

        static float RoofV2(Transform shell, string name, RoofShape shape, Vector3 c, float sx, float sz, float eaves, Material mat, Vector2 away,
                            bool soffit = false)
        {
            const float overhang = 0.3f;
            // Closes the underside of the eaves (houses have their top slab there instead).
            if (soffit && shape.Kind != RoofKind.Flat)
                Box(shell, name + "_Soffit", c + Vector3.up * (eaves - 0.03f), new Vector3(sx + 2f * overhang, 0.06f, sz + 2f * overhang), SurfaceMat("Planks"), false);
            float tan = Mathf.Tan(shape.Pitch * Mathf.Deg2Rad);
            switch (shape.Kind)
            {
                case RoofKind.Flat:
                    Box(shell, name, c + Vector3.up * (eaves + 0.1f), new Vector3(sx + 0.2f, 0.2f, sz + 0.2f), mat, false);
                    return eaves + 0.2f;

                case RoofKind.LeanTo:
                {
                    bool alongX = Mathf.Abs(away.x) > Mathf.Abs(away.y);
                    var low = alongX ? new Vector2(Mathf.Sign(away.x), 0f) : new Vector2(0f, Mathf.Sign(away.y));
                    float run = (alongX ? sx : sz) + overhang, width = (alongX ? sz : sx) + 2f * overhang;
                    float rise = Mathf.Min(run * tan, 2.5f);
                    MeshObject(shell, name, WedgeMesh(), mat, false, c + Vector3.up * eaves,
                               Quaternion.Euler(0f, VillagePlan.YawOf(low), 0f), new Vector3(width, rise, run));
                    return eaves + rise;
                }

                case RoofKind.MRoof:
                {
                    // Two parallel gables side by side, a valley between.
                    float span = (shape.RidgeAlongX ? sz : sx) * 0.5f;
                    float rise = (span * 0.5f + overhang * 0.5f) * tan;
                    for (int k = 0; k < 2; k++)
                    {
                        float off = (k == 0 ? -0.5f : 0.5f) * span;
                        var at = c + (shape.RidgeAlongX ? new Vector3(0f, eaves, off) : new Vector3(off, eaves, 0f));
                        Gable(shell, $"{name}_{k}", at, shape.RidgeAlongX, shape.RidgeAlongX ? sx : sz, span, rise, mat);
                    }
                    return eaves + rise;
                }

                default:
                {
                    float span = shape.RidgeAlongX ? sz : sx;
                    float rise = (span * 0.5f + overhang) * tan;
                    Gable(shell, name, c + Vector3.up * eaves, shape.RidgeAlongX, shape.RidgeAlongX ? sx : sz, span, rise, mat);
                    return eaves + rise;
                }
            }
        }

        /// <summary>A gable roof volume: ridge along local x (or z), over a span, eaves overhanging 0.3 m all round.</summary>
        static void Gable(Transform parent, string name, Vector3 at, bool ridgeAlongX, float length, float span, float rise, Material mat)
        {
            const float overhang = 0.3f;
            MeshObject(parent, name, GableMesh(), mat, false, at, Quaternion.Euler(0f, ridgeAlongX ? 0f : 90f, 0f),
                       new Vector3(length + 2f * overhang, rise, span + 2f * overhang));
        }

        static void BuildChimneysV2(Transform shell, HouseSpec h, Dictionary<WingSpec, float> ridges)
        {
            var brick = Mat("Chimney", new Color(0.42f, 0.22f, 0.17f));
            var pot = Mat("ChimneyPot", new Color(0.55f, 0.3f, 0.2f));
            for (int k = 0; k < h.ChimneysDetail.Length; k++)
            {
                var c = h.ChimneysDetail[k];
                var wing = h.Wings.OrderBy(w => DistanceToRect(w.Rect, c.X, c.Z)).First();
                float bottom = VillagePlan.WingEaves(h, wing) - 0.5f;
                float top = ridges[wing] + c.AboveRidge;
                var stack = Box(shell, $"Chimney_{k}", new Vector3(c.X, (bottom + top) * 0.5f, c.Z), new Vector3(c.W, top - bottom, c.D), brick, false);
                // Pots stand along the stack's longer side so they can be counted from the street.
                for (int p = 0; p < c.Pots; p++)
                {
                    float t = c.Pots == 1 ? 0f : (p / (c.Pots - 1f) - 0.5f) * 0.6f;
                    var off = c.W >= c.D ? new Vector3(t * c.W, 0f, 0f) : new Vector3(0f, 0f, t * c.D);
                    Cyl(shell, $"Chimney_{k}_Pot{p}", new Vector3(c.X, top + 0.2f, c.Z) + off, 0.22f, 0.4f, pot, false);
                }
                NoShadows(stack);
            }
        }

        static float DistanceToRect(float[] r, float x, float z)
        {
            float dx = Mathf.Max(r[0] - x, 0f, x - r[2]), dz = Mathf.Max(r[1] - z, 0f, z - r[3]);
            return dx * dx + dz * dz;
        }

        // ------------------------------------------------------------ filler and landmarks

        /// <summary>VILLAGE_LAYOUT.md "Filler buildings": locked, boarded shells in three variants (cottage, terrace, outbuilding).</summary>
        static void BuildFillerV2(Transform village, VillagePlan plan)
        {
            var root = Node(village, "Filler");
            var planks = Mat("Planks", new Color(0.35f, 0.3f, 0.25f));
            for (int n = 0; n < plan.Village.Filler.Length; n++)
            {
                var f = plan.Village.Filler[n];
                var name = f.Name.ToLowerInvariant();
                bool terrace = name.Contains("terrace") || name.Contains("pair");
                bool ruin = name.Contains("ruin");
                bool cottage = !terrace && !ruin && name.Contains("cottage");
                bool outbuilding = !terrace && !ruin && !cottage;
                float eaves = terrace ? 5.4f : cottage ? 4.2f : ruin ? 3f : name.Contains("barn") ? 5.5f : 3.2f;
                string wallKey = terrace ? "BrickRed" : outbuilding ? (name.Contains("garage") || name.Contains("forge") ? "BrickMixed" : "Weatherboard")
                                       : (n % 2 == 0 ? "RenderMossy" : "StoneBlock");
                var node = Node(root, $"{f.Id} {f.Name}", new Vector3(f.Pos[0], 0f, f.Pos[1]), Quaternion.Euler(0f, f.Yaw, 0f));
                float w = f.Size[0], d = f.Size[1];
                Box(node, "Shell", new Vector3(0f, eaves * 0.5f, 0f), new Vector3(w, eaves, d), SurfaceMat(wallKey));
                if (!ruin)
                    RoofV2(node, "Roof", new RoofShape { Kind = RoofKind.Gable, RidgeAlongX = w >= d, Pitch = outbuilding ? 25f : 40f },
                           Vector3.zero, w, d, eaves, SurfaceMat(outbuilding ? "CorrugatedRust" : "RoofSlateGrey"), Vector2.zero, true);

                // Boarded door(s) and windows on the front (+z) face.
                int doors = terrace ? 2 : 1;
                for (int k = 0; k < doors; k++)
                {
                    float x = doors == 1 ? 0f : (k == 0 ? -w * 0.25f : w * 0.25f);
                    Box(node, $"BoardedDoor{k}", new Vector3(x, 1f, d * 0.5f + 0.03f), new Vector3(1f, 2f, 0.06f), planks, false);
                }
                if (w >= 6f && !outbuilding)
                    foreach (float x in new[] { -w * 0.38f, w * 0.38f })
                        Box(node, "BoardedWindow", new Vector3(x, 1.5f, d * 0.5f + 0.03f), new Vector3(1f, 1f, 0.06f), planks, false);
                PlanLabel(f.Id, new Vector3(f.Pos[0], 0f, f.Pos[1]), 4f);
            }
        }

        static void BuildLandmarksV2(Transform village, VillagePlan plan)
        {
            var root = Node(village, "Landmarks");
            var grass = Mat("Garden", new Color(0.25f, 0.3f, 0.18f));
            var stone = Mat("Stone", new Color(0.45f, 0.44f, 0.42f));
            var dark = Mat("Dark", new Color(0.08f, 0.08f, 0.08f));
            var wood = Mat("Wood", new Color(0.25f, 0.17f, 0.11f));
            foreach (var l in plan.Village.Landmarks)
            {
                var box = VillagePlan.LandmarkBox(l);
                var at = new Vector3(box.Centre.x, 0f, box.Centre.y);
                var turn = Quaternion.Euler(0f, box.Yaw, 0f);
                switch (l.Id)
                {
                    case "L_STORE":
                    {
                        var node = Node(root, l.Name, at, turn);
                        float w = box.Size.x, d = box.Size.y;
                        Box(node, "Shell", new Vector3(0f, 2.5f, 0f), new Vector3(w, 5f, d), SurfaceMat("BrickRed"));
                        RoofV2(node, "Roof", new RoofShape { Kind = RoofKind.Gable, RidgeAlongX = true, Pitch = 38f }, Vector3.zero, w, d, 5f, SurfaceMat("RoofSlateGrey"), Vector2.zero, true);
                        Box(node, "ShopWindow", new Vector3(-w * 0.2f, 1.5f, d * 0.5f + 0.03f), new Vector3(w * 0.45f, 1.6f, 0.06f), Mat("Window", new Color(0.07f, 0.09f, 0.12f), 0.8f), false);
                        Box(node, "Door", new Vector3(w * 0.25f, 1.05f, d * 0.5f + 0.03f), new Vector3(1.1f, 2.1f, 0.06f), wood, false);
                        Box(node, "SignBoard", new Vector3(0f, 3.3f, d * 0.5f + 0.05f), new Vector3(w * 0.8f, 0.6f, 0.08f), Mat("SignGreen", new Color(0.12f, 0.22f, 0.15f)), false);
                        Label(node, "Sign", "HOLLOWMERE STORES - BAKER", new Vector3(0f, 3.3f, d * 0.5f + 0.1f), Quaternion.Euler(0f, 180f, 0f), 0.32f, new Color(0.9f, 0.85f, 0.65f));
                        Box(node, "OvenChimney", new Vector3(-w * 0.35f, 7f, -d * 0.2f), new Vector3(0.9f, 4f, 0.9f), Mat("Chimney", new Color(0.42f, 0.22f, 0.17f)), false);
                        break;
                    }
                    case "L_SCHOOL":
                    {
                        var node = Node(root, l.Name, at, turn);
                        float w = box.Size.x, d = box.Size.y;
                        Box(node, "Shell", new Vector3(0f, 2.75f, 0f), new Vector3(w, 5.5f, d), SurfaceMat("StoneBlock"));
                        float ridge = RoofV2(node, "Roof", new RoofShape { Kind = RoofKind.Gable, RidgeAlongX = true, Pitch = 45f }, Vector3.zero, w, d, 5.5f, SurfaceMat("RoofSlateGrey"), Vector2.zero, true);
                        Box(node, "BellCupola", new Vector3(0f, ridge + 0.6f, 0f), new Vector3(1.2f, 1.2f, 1.2f), wood, false);
                        Box(node, "Door", new Vector3(0f, 1.2f, d * 0.5f + 0.03f), new Vector3(1.4f, 2.4f, 0.06f), wood, false);
                        var yard = plan.Schoolyard;
                        Box(root, "Schoolyard", new Vector3(yard.Centre.x, 0.012f, yard.Centre.y), new Vector3(yard.Size.x - 0.4f, 0.02f, yard.Size.y - 0.4f),
                            SurfaceMat("FloorConcrete"), false, Quaternion.Euler(0f, yard.Yaw, 0f));
                        var swings = Node(root, "Swings", new Vector3(yard.Centre.x, 0f, yard.Centre.y), Quaternion.Euler(0f, yard.Yaw, 0f));
                        var iron = Mat("Metal", new Color(0.22f, 0.23f, 0.25f), 0.5f);
                        Box(swings, "Beam", new Vector3(4f, 2.4f, 0f), new Vector3(3.2f, 0.1f, 0.1f), iron, false);
                        foreach (float x in new[] { 2.4f, 5.6f })
                            Box(swings, "Leg", new Vector3(x, 1.2f, 0f), new Vector3(0.1f, 2.4f, 0.1f), iron);
                        foreach (float x in new[] { 3.4f, 4.6f })
                            Box(swings, "Seat", new Vector3(x, 0.45f, 0f), new Vector3(0.5f, 0.05f, 0.2f), wood, false);
                        break;
                    }
                    case "L_GRAVE":
                        BuildGraveyardV2(root, box, grass, stone, dark);
                        break;
                    case "L_GREEN":
                    {
                        Ellipse(root, "Green", at + Vector3.up * 0.012f, box.Size, box.Yaw, grass);
                        // The old oak (SmallTree x2.5 in the catalog), clear of the well, the memorial and the loop path:
                        // dead like the forest, a thick trunk under the shared dead-limb prefabs (P2 note: no sphere on a pole).
                        var oak = Node(root, "Oak", at + new Vector3(8f, 0f, -6f));
                        Cyl(oak, "Trunk", new Vector3(0f, 4.5f, 0f), 1.1f, 9f, Mat("Trunk", new Color(0.24f, 0.18f, 0.13f)));
                        DeadTree(oak, 7, 9f);
                        foreach (var (pos, yaw) in new[] { (at + new Vector3(-10f, 0f, 9f), 30f), (at + new Vector3(4f, 0f, -11f), 180f) })
                        {
                            var bench = Node(root, "Bench", pos, Quaternion.Euler(0f, yaw, 0f));
                            Box(bench, "Seat", new Vector3(0f, 0.45f, 0f), new Vector3(1.6f, 0.06f, 0.45f), wood);
                            Box(bench, "Back", new Vector3(0f, 0.75f, -0.2f), new Vector3(1.6f, 0.45f, 0.05f), wood, false);
                        }
                        break;
                    }
                    case "L_WELL":
                    {
                        var node = Node(root, l.Name, at, turn);
                        Cyl(node, "Wall", new Vector3(0f, 0.45f, 0f), box.Size.x, 0.9f, stone);
                        Cyl(node, "Mouth", new Vector3(0f, 0.905f, 0f), box.Size.x - 0.5f, 0.01f, dark, false);
                        foreach (float x in new[] { -1.1f, 1.1f })
                            Box(node, "Post", new Vector3(x, 1.6f, 0f), new Vector3(0.15f, 1.4f, 0.15f), wood, false);
                        Box(node, "Beam", new Vector3(0f, 2.3f, 0f), new Vector3(2.4f, 0.12f, 0.12f), wood, false);
                        Box(node, "Hood", new Vector3(0f, 2.75f, 0f), new Vector3(2.8f, 0.1f, 1.6f), SurfaceMat("RoofTilesGrey"), false);
                        break;
                    }
                    case "L_MEMORIAL":
                    {
                        var node = Node(root, l.Name, at, turn);
                        var granite = Mat("StandingStone", new Color(0.45f, 0.44f, 0.42f));
                        Box(node, "Step", new Vector3(0f, 0.2f, 0f), new Vector3(box.Size.x, 0.4f, box.Size.y), granite);
                        Box(node, "Plinth", new Vector3(0f, 0.65f, 0f), new Vector3(1.1f, 0.5f, 1.1f), granite);
                        Box(node, "Shaft", new Vector3(0f, 2.2f, 0f), new Vector3(0.45f, 2.6f, 0.45f), granite);
                        Label(node, "Names", "THEIR NAME LIVETH", new Vector3(0f, 0.65f, 0.56f), Quaternion.Euler(0f, 180f, 0f), 0.08f, new Color(0.2f, 0.2f, 0.2f));
                        break;
                    }
                    case "L_KIOSK":
                    {
                        var node = Node(root, l.Name, at, turn);
                        Box(node, "Box", new Vector3(0f, 1.25f, 0f), new Vector3(box.Size.x, 2.5f, box.Size.y), Mat("KioskRed", new Color(0.62f, 0.08f, 0.06f), 0.4f));
                        Box(node, "Glazing", new Vector3(0f, 1.4f, box.Size.y * 0.5f + 0.01f), new Vector3(0.7f, 1.4f, 0.02f), Mat("Window", new Color(0.07f, 0.09f, 0.12f), 0.8f), false);
                        break;
                    }
                    case "L_SHELTER":
                    {
                        var node = Node(root, l.Name, at, turn);
                        float w = box.Size.x, d = box.Size.y;
                        var glassWood = Mat("ShelterWood", new Color(0.3f, 0.32f, 0.3f));
                        Box(node, "Back", new Vector3(0f, 1.1f, -d * 0.5f + 0.05f), new Vector3(w, 2.2f, 0.1f), glassWood);
                        Box(node, "SideW", new Vector3(-w * 0.5f + 0.05f, 1.1f, 0f), new Vector3(0.1f, 2.2f, d), glassWood);
                        Box(node, "SideE", new Vector3(w * 0.5f - 0.05f, 1.1f, 0f), new Vector3(0.1f, 2.2f, d), glassWood);
                        Box(node, "Roof", new Vector3(0f, 2.3f, 0f), new Vector3(w + 0.3f, 0.12f, d + 0.3f), SurfaceMat("CorrugatedRust"), false);
                        Box(node, "Bench", new Vector3(0f, 0.45f, -d * 0.5f + 0.35f), new Vector3(w - 0.4f, 0.06f, 0.35f), wood, false);
                        break;
                    }
                    case "L_FIELDS":
                    {
                        var field = Box(root, "Fields", at + Vector3.up * 0.011f, new Vector3(box.Size.x - 1f, 0.02f, box.Size.y - 1f), Mat("Stubble", new Color(0.52f, 0.46f, 0.3f)), false, turn);
                        NoShadows(field);
                        break;
                    }
                    case "L_PINFOLD":
                    {
                        Ellipse(root, "PinfoldFloor", at + Vector3.up * 0.011f, box.Size, box.Yaw, SurfaceMat("GroundMud"));
                        break;
                    }
                    // L_GATE, L_SQUARE, L_ALTAR, L_BELL, L_BOARD, L_CHAPEL: the M1 builders; L_POND: BuildWaterV2.
                }
            }
        }

        /// <summary>Graveyard: grass, rows of gravestones, four angels, two yews and an open grave. Its walls are plan boundaries.</summary>
        static void BuildGraveyardV2(Transform root, Obb2 box, Material grass, Material stone, Material dark)
        {
            var node = Node(root, "Graveyard", new Vector3(box.Centre.x, 0f, box.Centre.y), Quaternion.Euler(0f, box.Yaw, 0f));
            var lawn = Box(node, "Grass", new Vector3(0f, 0.011f, 0f), new Vector3(box.Size.x - 0.6f, 0.02f, box.Size.y - 0.6f), grass, false);
            NoShadows(lawn);
            var rng = new System.Random(DressingSeed + 41); // fixed: scenery, not match randomness
            var headstone = Mat("Gravestone", new Color(0.5f, 0.5f, 0.48f));
            int n = 0;
            // The chapel takes the north-east corner (local +x, +z); the stones stand in rows west and south of it.
            for (float z = -box.Half.y + 3f; z <= box.Half.y - 3f; z += 3f)
            for (float x = -box.Half.x + 3f; x <= box.Half.x - 3f; x += 2.6f)
            {
                var world = box.ToWorld(new Vector2(x, z));
                if (VillagePlan.Chapel.Contains(world, 2f) || rng.NextDouble() < 0.15) continue;
                float lean = Range(rng, -7f, 7f);
                Box(node, $"Gravestone_{n++:00}", new Vector3(x + Range(rng, -0.3f, 0.3f), 0.45f, z), new Vector3(0.7f, 0.9f, 0.15f), headstone, true,
                    Quaternion.Euler(lean, Range(rng, -6f, 6f), Range(rng, -4f, 4f)));
            }
            var angel = Mat("StandingStone", new Color(0.45f, 0.44f, 0.42f));
            foreach (var p in new[] { new Vector3(-10f, 0f, -12f), new Vector3(-10f, 0f, 6f), new Vector3(-1f, 0f, -12f), new Vector3(-1f, 0f, 2f) })
            {
                var a = Node(node, "AngelStatue", p);
                Box(a, "Plinth", new Vector3(0f, 0.4f, 0f), new Vector3(0.8f, 0.8f, 0.8f), angel);
                Box(a, "Figure", new Vector3(0f, 1.5f, 0f), new Vector3(0.5f, 1.4f, 0.4f), angel, false);
            }
            foreach (var p in new[] { new Vector3(-12f, 0f, 14f), new Vector3(-12f, 0f, -15f) })
            {
                var yew = Node(node, "Yew", p);
                Cyl(yew, "Trunk", new Vector3(0f, 1.2f, 0f), 0.7f, 2.4f, Mat("Trunk", new Color(0.24f, 0.18f, 0.13f)));
                Ball(yew, "Crown", new Vector3(0f, 3.4f, 0f), 4.5f, Mat("YewFoliage", new Color(0.07f, 0.12f, 0.07f)), collider: false);
            }
            Box(node, "OpenGrave", new Vector3(-5f, 0.012f, -4f), new Vector3(1f, 0.02f, 2.2f), dark, false);
            Box(node, "Spoil", new Vector3(-3.9f, 0.3f, -4f), new Vector3(0.9f, 0.6f, 2f), SurfaceMat("GroundMud"), false);
        }

        // ------------------------------------------------------------ boundaries, lamps, scarecrows

        static void BuildBoundariesV2(Transform village, VillagePlan plan)
        {
            var root = Node(village, "Boundaries");
            var rng = new System.Random(DressingSeed + 23); // fixed: fences are scenery
            foreach (var b in plan.Boundaries)
            {
                var a3 = new Vector3(b.A.x, 0f, b.A.y);
                var b3 = new Vector3(b.B.x, 0f, b.B.y);
                if (!b.Solid)
                {
                    FenceRun(Node(root, b.Id), rng, a3, b3, 0.15f);
                    continue;
                }
                var box = b.Box;
                var mat = b.Kind == BoundaryKind.Hedge ? Mat("Hedge", new Color(0.12f, 0.2f, 0.1f)) : SurfaceMat(b.Mat);
                Box(root, b.Id, new Vector3(box.Centre.x, b.Height * 0.5f, box.Centre.y), new Vector3(box.Size.x, b.Height, box.Size.y), mat, true, Quaternion.Euler(0f, box.Yaw, 0f));
            }
        }

        /// <summary>Street lamps (catalog StreetLamp, 4.2 m). Their lights come with P8 (1 in 3 working by day, dead at night).</summary>
        static void BuildLampsV2(Transform village, VillagePlan plan)
        {
            var root = Node(village, "StreetLamps");
            var iron = Mat("Metal", new Color(0.22f, 0.23f, 0.25f), 0.5f);
            var glass = Mat("LampGlass", new Color(0.3f, 0.3f, 0.25f), 0.8f);
            for (int i = 0; i < plan.Village.StreetLamps.Length; i++)
            {
                var p = plan.Village.StreetLamps[i];
                var lamp = Node(root, $"StreetLamp_{i:00}", new Vector3(p[0], 0f, p[1]));
                Cyl(lamp, "Post", new Vector3(0f, 2.1f, 0f), 0.14f, 4.2f, iron);
                Box(lamp, "Arm", new Vector3(0.3f, 4.1f, 0f), new Vector3(0.6f, 0.06f, 0.06f), iron, false);
                Box(lamp, "Lantern", new Vector3(0.55f, 3.85f, 0f), new Vector3(0.32f, 0.42f, 0.32f), glass, false);
            }
        }

        /// <summary>The six scarecrows of the H13 puzzle (DESIGN.md: they turn to face the nearest player between acts, from P8).</summary>
        static void BuildScarecrowsV2(Transform village, VillagePlan plan)
        {
            var root = Node(village, "Scarecrows");
            var wood = Mat("Wood", new Color(0.25f, 0.17f, 0.11f));
            var sacking = Mat("Sacking", new Color(0.45f, 0.38f, 0.26f));
            var hat = Mat("Dark", new Color(0.08f, 0.08f, 0.08f));
            for (int i = 0; i < plan.Village.Scarecrows.Length; i++)
            {
                var p = plan.Village.Scarecrows[i];
                var pos = new Vector2(p[0], p[1]);
                var facing = VillagePlan.YawOf(plan.Fields.Centre - pos);
                var s = Node(root, $"Scarecrow_{i + 1}", new Vector3(pos.x, 0f, pos.y), Quaternion.Euler(0f, facing, 0f));
                Cyl(s, "Post", new Vector3(0f, 1.1f, 0f), 0.12f, 2.2f, wood);
                Box(s, "Crossbar", new Vector3(0f, 1.55f, 0f), new Vector3(1.2f, 0.08f, 0.08f), wood, false);
                Box(s, "Coat", new Vector3(0f, 1.3f, 0.05f), new Vector3(0.5f, 0.75f, 0.25f), sacking, false);
                Ball(s, "Head", new Vector3(0f, 1.95f, 0f), 0.32f, sacking, collider: false);
                Cyl(s, "HatBrim", new Vector3(0f, 2.12f, 0f), 0.5f, 0.03f, hat, false);
                Cyl(s, "HatCrown", new Vector3(0f, 2.24f, 0f), 0.26f, 0.22f, hat, false);
            }
        }

        // ------------------------------------------------------------ forest belt and night blockers

        /// <summary>The forest belt inside the ring wall (VILLAGE_LAYOUT.md: 20-25 m deep), the M1 trunks and dead limbs.</summary>
        static void BuildForestBeltV2(Transform village, VillagePlan plan)
        {
            var root = Node(village, "ForestBelt");
            var trunk = Mat("Trunk", new Color(0.24f, 0.18f, 0.13f));
            var rng = new System.Random(1972); // fixed: scenery
            for (int i = 0; i < plan.Trees.Count; i++)
                Tree(root, i, new Vector3(plan.Trees[i].x, 0f, plan.Trees[i].y), rng, trunk);
        }

        /// <summary>
        /// DESIGN.md "How the village changes at night": a fallen tree or broken cart blocks one random path. All six
        /// are built across their lanes, inactive; P8 switches on the one MatchPlan picks. When on, they block players
        /// (collider) and carve the NavMesh.
        /// </summary>
        static void BuildBlockersV2(Transform village, VillagePlan plan)
        {
            var root = Node(village, "NightBlockers");
            var bark = Mat("Bark", new Color(0.24f, 0.2f, 0.16f));
            var wood = Mat("Wood", new Color(0.25f, 0.17f, 0.11f));
            var crate = Mat("ChestWood", new Color(0.3f, 0.2f, 0.12f));
            foreach (var b in plan.Blockers)
            {
                var node = Node(root, $"{b.Id} {b.Prop}", new Vector3(b.Pos.x, 0f, b.Pos.y), Quaternion.Euler(0f, b.Yaw, 0f));
                var visual = Node(node, "Visual");
                if (b.Prop == "FallenTree")
                {
                    Cyl(visual, "Trunk", new Vector3(0f, 0.5f, 0f), 0.9f, b.Span, bark, false, Quaternion.Euler(0f, 0f, 90f));
                    Ball(visual, "RootPlate", new Vector3(-b.Span * 0.5f, 0.9f, 0f), 2.2f, Mat("Ground", new Color(0.23f, 0.27f, 0.17f)), collider: false);
                    for (int k = 0; k < 4; k++)
                        Limb(visual, $"Branch{k}", new Vector3(b.Span * (0.1f + 0.1f * k), 0.7f, 0f),
                             new Vector3(0.3f, 0.6f, k % 2 == 0 ? 0.7f : -0.7f).normalized, 1.6f, 0.16f, bark);
                }
                else
                {
                    // The cart lies tipped across the lane; spilled crates close the rest of it.
                    Box(visual, "CartBed", new Vector3(-b.Span * 0.5f + 1.9f, 0.7f, 0f), new Vector3(3.5f, 0.9f, 1.5f), wood, false, Quaternion.Euler(0f, 0f, 12f));
                    Cyl(visual, "Wheel", new Vector3(-b.Span * 0.5f + 1.4f, 0.6f, 0.8f), 1.2f, 0.12f, wood, false, Quaternion.Euler(90f, 0f, 0f));
                    for (float x = -b.Span * 0.5f + 4f; x < b.Span * 0.5f - 0.3f; x += 1.05f)
                        Box(visual, "Crate", new Vector3(x, 0.45f, 0f), new Vector3(0.9f, 0.9f, 0.9f), crate, false, Quaternion.Euler(0f, x * 37f % 25f, 0f));
                }
                var col = node.gameObject.AddComponent<BoxCollider>();
                col.center = new Vector3(0f, 0.8f, 0f);
                col.size = new Vector3(b.Span, 1.6f, b.Depth);
                ExcludeFromNavMesh(node.gameObject);
                AddCarvingObstacle(node.gameObject, new Vector3(0f, 1f, 0f), new Vector3(b.Span, 2f, b.Depth));
                node.gameObject.SetActive(false);
            }
        }

        // ------------------------------------------------------------ layout and cameras

        static VillageLayout BuildLayoutV2(VillagePlan plan, Transform hollowSpawn)
        {
            var layout = new GameObject("Layout").AddComponent<VillageLayout>();
            var t = layout.transform;
            layout.PlayerSpawn = Node(t, "PlayerSpawn", new Vector3(0f, 0.1f, Half + 12f), Quaternion.Euler(0f, 180f, 0f));
            layout.HollowNightSpawn = hollowSpawn;
            layout.SquareCenter = Node(t, "SquareCenter", AltarPos);
            layout.CircleRadius = CircleRadius;
            layout.GateLineZ = Half;
            layout.BridgeZ = BridgeZ;
            layout.ForestSpawns = plan.ForestSpawns.Select((p, i) => Node(t, $"ForestSpawn_{i}", new Vector3(p.x, 0f, p.y))).ToArray();
            layout.RoamPoints = plan.RoamPoints.Select((p, i) => Node(t, $"Roam_{i:00}", new Vector3(p.x, 0f, p.y))).ToArray();
            return layout;
        }

        static readonly (string name, Vector3 pos, Vector3 lookAt)[] V2Cameras =
        {
            ("Cam_V2_Aerial", new Vector3(0f, 120f, -190f), new Vector3(0f, 0f, 10f)),
            ("Cam_V2_MainStreet", new Vector3(1.2f, 1.7f, 84f), new Vector3(0.5f, 1.4f, 40f)),
            ("Cam_V2_MainStreetNorth", new Vector3(-0.5f, 1.7f, 22f), new Vector3(1f, 2f, 75f)),
            ("Cam_V2_Green", new Vector3(-14f, 1.7f, 18f), new Vector3(6f, 1.2f, 0f)),
            ("Cam_V2_MillLane", new Vector3(-38f, 1.7f, -27f), new Vector3(-70f, 1.5f, -60f)),
            ("Cam_V2_MillPond", new Vector3(-80f, 2f, -70f), new Vector3(-105f, 1f, -92f)),
            ("Cam_V2_ChapelLane", new Vector3(-46f, 1.7f, 30f), new Vector3(-53f, 2f, 80f)),
            ("Cam_V2_SchoolLane", new Vector3(6f, 1.7f, 69.5f), new Vector3(50f, 2f, 68f)),
            ("Cam_V2_EastRoad", new Vector3(30f, 1.7f, -4f), new Vector3(85f, 2f, -28f)),
            ("Cam_V2_StreamBridge", new Vector3(14f, 2.2f, -100f), new Vector3(19f, 0f, -116f)),
            ("Cam_V2_Fields", new Vector3(60f, 2f, -60f), new Vector3(100f, 1f, -100f)),
        };

        static void BuildShotCamerasV2()
        {
            var root = new GameObject("ShotCameras").transform;
            var top = Node(root, "Cam_V2_TopDown", new Vector3(0f, 250f, 0f), Quaternion.Euler(90f, 0f, 0f)).gameObject.AddComponent<Camera>();
            top.enabled = false;
            top.orthographic = true;
            top.orthographicSize = 158f; // the whole village, north up, for docs/design/ref/village_map_v2.png
            top.nearClipPlane = 1f;
            top.farClipPlane = 400f;
            top.backgroundColor = FogDay;
            LookCamera(top);
            foreach (var (name, pos, lookAt) in V2Cameras) ShotCamera(root, name, pos, lookAt);
        }

        const string PlanLabelsName = "PlanLabels";
        static Transform s_planLabels; // slug labels over every building, read from straight above; on only for the top-down shot

        static void PlanLabel(string text, Vector3 at, float size) =>
            Label(s_planLabels, text, text, new Vector3(at.x, 30f, at.z), Quaternion.Euler(90f, 0f, 0f), size, Color.white);

        public static void VillageV2Shots()
        {
            EditorSceneManager.OpenScene(VillageBuilderV2.ScenePath, OpenSceneMode.Single);
            var labels = EditorSceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(g => g.name == PlanLabelsName);
            var top = GameObject.Find("Cam_V2_TopDown").GetComponent<Camera>();
            bool fog = RenderSettings.fog;
            RenderSettings.fog = false;
            if (labels != null) labels.SetActive(true);
            BatchTools.RenderToPng(top, "Logs/claude/v2_TopDown.png");
            if (labels != null) labels.SetActive(false);
            RenderSettings.fog = fog;
            foreach (var (name, _, _) in V2Cameras)
                BatchTools.RenderToPng(GameObject.Find(name).GetComponent<Camera>(), $"Logs/claude/v2_{name.Substring(7)}.png");
            Debug.Log("[Hollowmere] VILLAGE V2 SHOTS OK: Logs/claude/v2_*.png");
        }

        public static void VillageV2HouseShots()
        {
            EditorSceneManager.OpenScene(VillageBuilderV2.ScenePath, OpenSceneMode.Single); // never saved: roofs come off for the cutaways
            var cam = new GameObject("HouseShotCamera").AddComponent<Camera>();
            cam.enabled = false;
            cam.fieldOfView = 50f;
            cam.backgroundColor = FogDay;
            LookCamera(cam);
            int shots = 0;
            foreach (var h in DesignData.LoadHouses())
            {
                var node = GameObject.Find($"Village/Houses/{h.Slug} {h.Name}")?.transform;
                if (node == null) { Debug.LogError($"[Hollowmere] {h.Slug} not in {VillageBuilderV2.ScenePath}"); continue; }
                var house = node.GetComponent<House>();
                float x0 = h.Wings.Min(w => w.Rect[0]), z0 = h.Wings.Min(w => w.Rect[1]), x1 = h.Wings.Max(w => w.Rect[2]), z1 = h.Wings.Max(w => w.Rect[3]);

                // Three-quarter views from 35 degrees left of straight on, front and back.
                var b = house.Interior;
                float reach = Mathf.Max(b.extents.x, b.extents.y, b.extents.z);
                var front = house.Front != null ? house.Front.forward : node.forward;
                cam.orthographic = false;
                cam.nearClipPlane = 0.3f;
                cam.farClipPlane = 400f;
                foreach (var (label, dir) in new[] { ("front", front), ("back", -front) })
                {
                    var look = Quaternion.Euler(0f, -35f, 0f) * dir;
                    cam.transform.position = b.center + look * (reach * 2.4f + 4f) + Vector3.up * (reach * 0.9f + 2f);
                    cam.transform.LookAt(b.center);
                    BatchTools.RenderToPng(cam, $"Logs/claude/house_{h.Slug}_{label}.png");
                    shots++;
                }

                // Cutaways: roofs off, straight down from just under each level's ceiling, house +z up the image as in the ref plans.
                var roofs = node.Find("Shell/Roofs");
                if (roofs != null) roofs.gameObject.SetActive(false);
                cam.orthographic = true;
                cam.orthographicSize = Mathf.Max((z1 - z0) * 0.5f, (x1 - x0) * 0.5f * 900f / 1600f) + 1f;
                cam.nearClipPlane = 0.01f;
                foreach (var lv in h.Levels.OrderBy(l => l.Value.Y))
                {
                    var eye = new Vector3((x0 + x1) * 0.5f, lv.Value.Y + lv.Value.H - 0.05f, (z0 + z1) * 0.5f);
                    cam.transform.SetPositionAndRotation(node.TransformPoint(eye), node.rotation * Quaternion.Euler(90f, 0f, 0f));
                    cam.farClipPlane = lv.Value.H + 1f;
                    BatchTools.RenderToPng(cam, $"Logs/claude/house_{h.Slug}_plan_{lv.Key}.png");
                    shots++;
                }
                if (roofs != null) roofs.gameObject.SetActive(true);
            }
            Object.DestroyImmediate(cam.gameObject);
            Debug.Log($"[Hollowmere] HOUSE SHOTS OK: {shots} PNGs in Logs/claude/house_*.png");
        }

        /// <summary>A flat ellipse (size = diameters along local x, z) with UVs in metres, no collider.</summary>
        static void Ellipse(Transform parent, string name, Vector3 centre, Vector2 size, float yaw, Material mat)
        {
            const int segments = 48;
            var verts = new List<Vector3> { Vector3.zero };
            var uvs = new List<Vector2> { Vector2.zero };
            var tris = new List<int>();
            for (int k = 0; k <= segments; k++)
            {
                float a = k * Mathf.PI * 2f / segments;
                var p = new Vector3(Mathf.Cos(a) * size.x * 0.5f, 0f, Mathf.Sin(a) * size.y * 0.5f);
                verts.Add(p);
                uvs.Add(new Vector2(p.x, p.z));
                // Counterclockwise in x-z seen from above, so each fan triangle is wound (centre, next, this) to face up.
                if (k > 0) { tris.Add(0); tris.Add(k + 1); tris.Add(k); }
            }
            MeshObject(parent, name, V2Mesh(name, verts, tris, uvs), mat, false, centre, Quaternion.Euler(0f, yaw, 0f), shadows: false);
        }

        // ------------------------------------------------------------ roof meshes and placeholder colours

        static Mesh s_gable, s_wedge;

        /// <summary>Unit gable roof: base x, z in [-0.5, 0.5] at y 0, ridge along x at z 0, y 1. Scale it to (length, rise, span).</summary>
        static Mesh GableMesh()
        {
            if (s_gable != null) return s_gable;
            var a = new Vector3(-0.5f, 0f, -0.5f); var b = new Vector3(0.5f, 0f, -0.5f);
            var c = new Vector3(0.5f, 0f, 0.5f);   var d = new Vector3(-0.5f, 0f, 0.5f);
            var r0 = new Vector3(-0.5f, 1f, 0f);   var r1 = new Vector3(0.5f, 1f, 0f);
            var faces = new[] { new[] { a, b, r1, r0 }, new[] { c, d, r0, r1 }, new[] { b, c, r1 }, new[] { d, a, r0 } };
            return s_gable = Solid("GableRoof", faces, new Vector3(0f, 0.33f, 0f), true);
        }

        /// <summary>Unit lean-to roof: base x, z in [-0.5, 0.5] at y 0, high edge (y 1) along z -0.5, low edge along z +0.5.</summary>
        static Mesh WedgeMesh()
        {
            if (s_wedge != null) return s_wedge;
            var a = new Vector3(-0.5f, 0f, -0.5f); var b = new Vector3(0.5f, 0f, -0.5f);
            var c = new Vector3(0.5f, 0f, 0.5f);   var d = new Vector3(-0.5f, 0f, 0.5f);
            var e = new Vector3(-0.5f, 1f, -0.5f); var f = new Vector3(0.5f, 1f, -0.5f);
            var faces = new[] { new[] { e, f, c, d }, new[] { a, b, f, e }, new[] { b, c, f }, new[] { d, a, e } };
            return s_wedge = Solid("LeanToRoof", faces, new Vector3(0f, 0.3f, -0.15f), true);
        }

        /// <summary>
        /// A flat-shaded mesh from convex polygons, each wound so its face points away from the inside point; double-sided
        /// adds the inward faces too (a roof seen from the attic and from under the eaves).
        /// </summary>
        static Mesh Solid(string name, Vector3[][] faces, Vector3 inside, bool doubleSided = false)
        {
            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();
            foreach (var face in faces)
            {
                var centre = Vector3.zero;
                foreach (var p in face) centre += p;
                centre /= face.Length;
                var normal = Vector3.Cross(face[1] - face[0], face[2] - face[0]);
                bool flip = Vector3.Dot(normal, centre - inside) < 0f;
                int start = verts.Count;
                var u = (face[1] - face[0]).normalized;
                var v = Vector3.Cross(normal.normalized, u);
                foreach (var p in face)
                {
                    verts.Add(p);
                    uvs.Add(new Vector2(Vector3.Dot(p, u), Vector3.Dot(p, v)));
                }
                for (int k = 1; k + 1 < face.Length; k++)
                {
                    if (flip) { tris.Add(start); tris.Add(start + k + 1); tris.Add(start + k); }
                    else { tris.Add(start); tris.Add(start + k); tris.Add(start + k + 1); }
                }
                if (!doubleSided) continue;
                // The inward side gets its own vertices, so its normals point inward.
                int back = verts.Count;
                for (int k = 0; k < face.Length; k++) { verts.Add(face[k]); uvs.Add(uvs[start + k]); }
                for (int k = 1; k + 1 < face.Length; k++)
                {
                    if (flip) { tris.Add(back); tris.Add(back + k); tris.Add(back + k + 1); }
                    else { tris.Add(back); tris.Add(back + k + 1); tris.Add(back + k); }
                }
            }
            return V2Mesh(name, verts, tris, uvs);
        }

        /// <summary>
        /// A material named after its asset catalog key, so the P4 library entries dress it; until then a flat colour
        /// close to the real surface, so the massing reads in screenshots.
        /// </summary>
        static Material SurfaceMat(string key) => Mat(key, PlaceholderColour(key));

        static Color PlaceholderColour(string key)
        {
            switch (key)
            {
                case "BrickRed": return new Color(0.5f, 0.24f, 0.18f);
                case "BrickDark": return new Color(0.28f, 0.17f, 0.14f);
                case "BrickMixed": return new Color(0.48f, 0.33f, 0.26f);
                case "BrickPaintedWorn": return new Color(0.78f, 0.74f, 0.64f);
                case "StuccoWhite": case "Whitewash": return new Color(0.85f, 0.84f, 0.8f);
                case "RenderCream": case "RenderCracked": case "RenderMossy": case "Pebbledash": return new Color(0.74f, 0.7f, 0.6f);
                case "RenderOchre": return new Color(0.75f, 0.6f, 0.35f);
                case "StoneBlock": case "PlasterStone": return new Color(0.48f, 0.47f, 0.44f);
                case "SandstoneBlocks": return new Color(0.66f, 0.56f, 0.42f);
                case "TimberBeam": return new Color(0.16f, 0.12f, 0.1f);
                case "Weatherboard": return new Color(0.58f, 0.6f, 0.58f);
                case "RawPlankWall": return new Color(0.4f, 0.32f, 0.24f);
                case "RoofSlateGrey": case "RoofTilesGrey": return new Color(0.24f, 0.25f, 0.27f);
                case "RoofClay2": case "RoofClay3": return new Color(0.5f, 0.26f, 0.18f);
                case "RoofRedSlate": return new Color(0.36f, 0.22f, 0.2f);
                case "Thatch": case "ThatchReed": return new Color(0.5f, 0.42f, 0.27f);
                case "CorrugatedRust": return new Color(0.42f, 0.24f, 0.15f);
                case "Cobbles2": return new Color(0.36f, 0.35f, 0.33f);
                case "Paving": case "PavingSlabs": case "Kerb": return new Color(0.5f, 0.49f, 0.46f);
                case "GroundGravel": return new Color(0.46f, 0.43f, 0.38f);
                case "Road": case "GroundMud": return new Color(0.3f, 0.25f, 0.2f);
                case "GroundGrassPath": return new Color(0.3f, 0.34f, 0.2f);
                case "FloorConcrete": return new Color(0.45f, 0.45f, 0.43f);
                case "Stone": return new Color(0.45f, 0.44f, 0.42f);
                case "Planks": return new Color(0.35f, 0.3f, 0.25f);
                default: return new Color(0.6f, 0.58f, 0.54f);
            }
        }
    }
}
