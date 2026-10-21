using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static Hollowmere.EditorTools.BuilderKit;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Builds Assets/Hollowmere/Scenes/Greybox.unity from primitives: the walled village, the 15 houses,
    /// the town square and chapel, the fog road to the bridge, lights, the player and the Hollow, and the NavMesh.
    /// Idempotent: every run rebuilds the scene from scratch.
    ///
    /// Layout follows docs/DESIGN.md "The village map"; houses follow "Houses and puzzles".
    /// Art slots for milestone 7: each house keeps its walls and roof under "Shell", and every gameplay
    /// object keeps its primitives under a child named "Visual", so either can be swapped for a prefab
    /// without touching the gameplay components.
    /// </summary>
    public static partial class GreyboxVillageBuilder
    {
        public const string ScenePath = "Assets/Hollowmere/Scenes/Greybox.unity";
        const string NavMeshPath = "Assets/Hollowmere/Scenes/Greybox_NavMesh.asset";
        const string LightingPath = "Assets/Hollowmere/Settings/GreyboxLighting.lighting";

        // Village, docs/DESIGN.md "The village map": 300 x 300 m inside the wall, square and gate at the north edge,
        // about 200 m of fog road beyond the gate to the old bridge.
        const float Half = 150f;
        const float RingWallHeight = 5f, RingWallThickness = 1.2f;
        const float GateHalfWidth = 4f;
        const float RoadHalfWidth = 3f;
        const float BridgeZ = 352f;
        static readonly Vector3 AltarPos = new Vector3(0f, 0f, 112f);
        const float CircleRadius = 7f;

        // House shell in local space, front door facing +z (north, towards the square).
        const float HouseW = 12f, HouseD = 9f, HouseH = 3f, WallT = 0.3f;
        // Doorways are wide because the Hollow's NavMesh agent (radius 0.5) has to fit through them.
        const float DoorWidth = 1.6f, DoorHeight = 2.2f, InnerDoorWidth = 1.6f, InnerWallZ = 1f;
        const float RoofPitch = 45f;

        static readonly Color FogDay = new Color(0.72f, 0.75f, 0.78f);

        [MenuItem("Hollowmere/Build Greybox Village")]
        public static void Build()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            Begin();
            BeginDressing();
            EnsureFolder("Assets/Hollowmere/Scenes");
            EnsureFolder("Assets/Hollowmere/Settings");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var village = new GameObject("Village").transform; // the NavMesh bakes from everything under here
            BuildGround(village);
            BuildRingWall(village);
            var gate = BuildGate(village);
            BuildRoad(village);
            BuildForest(village);

            var houseRoot = Node(village, "Houses");
            var houses = new House[HouseCatalog.Count];
            for (int i = 0; i < houses.Length; i++)
                houses[i] = BuildHouse(houseRoot, i, HousePosition(i));

            DressGrounds(village);

            var (board, bell, altar, circle) = BuildSquare(village);
            var chapelSpawn = BuildChapel(village, out var ritualBook);
            var layout = BuildLayout(chapelSpawn);

            var lighting = BuildLights();
            var player = BuildPlayer(layout.PlayerSpawn);
            var hollow = BuildHollow(layout.HollowNightSpawn, layout);
            BuildShotCameras();
            ApplyRenderSettings();

            var director = new GameObject("Match").AddComponent<MatchDirector>();
            director.Houses = houses;
            director.Players = new[] { player };
            director.Layout = layout;
            director.Board = board;
            director.RitualBook = ritualBook;
            director.Bell = bell;
            director.Lighting = lighting;
            director.Gate = gate;
            director.Altar = altar;
            director.Circle = circle;
            director.gameObject.AddComponent<NoiseAudio>();
            var crossing = director.gameObject.AddComponent<Crossing>();
            crossing.Director = director;
            crossing.Layout = layout;
            crossing.Lighting = lighting;
            hollow.Director = director;

            var ui = new GameObject("HUD");
            var hud = ui.AddComponent<Hud>();
            hud.Player = player;
            hud.Director = director;
            hud.Hollow = hollow;
            var overlay = ui.AddComponent<DebugOverlay>();
            overlay.Player = player;
            overlay.Director = director;
            overlay.Hollow = hollow;
            AddNavMeshSurface(village);
            BakeNavMesh();
            End();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log($"[Hollowmere] GREYBOX BUILT: {ScenePath} ({houses.Length} houses)");
        }

        /// <summary>Three rows of five houses south of the square, all facing north.</summary>
        public static Vector3 HousePosition(int index)
        {
            int row = index / 5, col = index % 5;
            return new Vector3(-96f + 48f * col, 0f, 55f - 50f * row);
        }

        // ------------------------------------------------------------ ground, wall, gate, road, forest

        static void BuildGround(Transform village)
        {
            Box(village, "Ground", new Vector3(0f, -0.5f, 100f), new Vector3(440f, 1f, 760f), Mat("Ground", new Color(0.23f, 0.27f, 0.17f)));
        }

        static void BuildRingWall(Transform village)
        {
            var stone = Mat("Stone", new Color(0.45f, 0.44f, 0.42f));
            var wall = Node(village, "Wall");
            float gap = 2f * GateHalfWidth + 2.4f;
            WallAlongX(wall, "North", Half, -Half, Half, RingWallHeight, RingWallThickness, stone, new Gap(0f, gap, RingWallHeight + 1f));
            WallAlongX(wall, "South", -Half, -Half, Half, RingWallHeight, RingWallThickness, stone);
            WallAlongZ(wall, "West", -Half, -Half, Half, RingWallHeight, RingWallThickness, stone);
            WallAlongZ(wall, "East", Half, -Half, Half, RingWallHeight, RingWallThickness, stone);
        }

        static VillageGate BuildGate(Transform village)
        {
            var stone = Mat("Stone", new Color(0.45f, 0.44f, 0.42f));
            var wood = Mat("GateWood", new Color(0.20f, 0.14f, 0.09f));
            var gate = Node(village, "Gate", new Vector3(0f, 0f, Half));
            Box(gate, "PostWest", new Vector3(-GateHalfWidth - 0.6f, 3f, 0f), new Vector3(1.2f, 6f, 1.6f), stone);
            Box(gate, "PostEast", new Vector3(GateHalfWidth + 0.6f, 3f, 0f), new Vector3(1.2f, 6f, 1.6f), stone);
            Box(gate, "Lintel", new Vector3(0f, 6.4f, 0f), new Vector3(2f * GateHalfWidth + 2.4f, 0.8f, 1.6f), stone);
            // docs/DESIGN.md "The Crossing": carved into the gate. Readable from the square.
            Label(gate, "Inscription", "DO NOT LOOK BACK", new Vector3(0f, 6.4f, -0.81f), Quaternion.identity, 0.32f, new Color(0.15f, 0.13f, 0.12f));

            // Leaves open outwards (north). Left hinge at -x swings with -90, right hinge at +x with +90.
            var component = gate.gameObject.AddComponent<VillageGate>();
            for (int side = 0; side < 2; side++)
            {
                float sign = side == 0 ? -1f : 1f;
                var pivot = Node(gate, side == 0 ? "LeafWest" : "LeafEast", new Vector3(sign * GateHalfWidth, 0f, 0f));
                var visual = Node(pivot, "Visual");
                Box(visual, "Planks", new Vector3(-sign * GateHalfWidth * 0.5f, 2.25f, 0f), new Vector3(GateHalfWidth - 0.05f, 4.5f, 0.25f), wood);
                Box(visual, "Brace", new Vector3(-sign * GateHalfWidth * 0.5f, 2.25f, -0.15f), new Vector3(GateHalfWidth - 0.4f, 0.25f, 0.06f), wood);
                ExcludeFromNavMesh(pivot.gameObject);
                AddCarvingObstacle(pivot.gameObject, new Vector3(-sign * GateHalfWidth * 0.5f, 2.25f, 0f), new Vector3(GateHalfWidth, 4.5f, 0.5f));
                if (side == 0) component.LeafWest = pivot;
                else component.LeafEast = pivot;
            }
            return component;
        }

        static void BuildRoad(Transform village)
        {
            var road = Node(village, "FogRoad");
            float length = BridgeZ - Half;
            Box(road, "Road", new Vector3(0f, 0.02f, Half + length * 0.5f), new Vector3(2f * RoadHalfWidth, 0.04f, length), Mat("Road", new Color(0.30f, 0.28f, 0.26f)));

            var plank = Mat("BridgeWood", new Color(0.32f, 0.24f, 0.17f));
            var bridge = Node(road, "Bridge", new Vector3(0f, 0f, BridgeZ));
            Box(bridge, "Deck", new Vector3(0f, 0.15f, 7f), new Vector3(5f, 0.2f, 14f), plank);
            Box(bridge, "RailWest", new Vector3(-2.4f, 0.8f, 7f), new Vector3(0.12f, 0.1f, 14f), plank, collider: false);
            Box(bridge, "RailEast", new Vector3(2.4f, 0.8f, 7f), new Vector3(0.12f, 0.1f, 14f), plank, collider: false);
            for (int i = 0; i <= 4; i++)
            {
                Box(bridge, $"PostW{i}", new Vector3(-2.4f, 0.45f, i * 3.5f), new Vector3(0.15f, 0.9f, 0.15f), plank, collider: false);
                Box(bridge, $"PostE{i}", new Vector3(2.4f, 0.45f, i * 3.5f), new Vector3(0.15f, 0.9f, 0.15f), plank, collider: false);
            }

            // Invisible walls keep the Crossing on the road (docs/DESIGN.md "The Crossing").
            Blocker(road, "EdgeWest", new Vector3(-7f, 2f, 265f), new Vector3(1f, 4f, 230f));
            Blocker(road, "EdgeEast", new Vector3(7f, 2f, 265f), new Vector3(1f, 4f, 230f));
            Blocker(road, "End", new Vector3(0f, 2f, 380f), new Vector3(15f, 4f, 1f));
        }

        static void BuildForest(Transform village)
        {
            var trunk = Mat("Trunk", new Color(0.24f, 0.18f, 0.13f));
            var forest = Node(village, "Forest");
            var rng = new System.Random(1971); // fixed: the forest is scenery, not match randomness
            int n = 0;

            // A band of trees outside the wall.
            for (float x = -184f; x <= 184f; x += 12f)
            for (float z = -184f; z <= 184f; z += 12f)
            {
                var p = new Vector3(x + Jitter(rng, 4f), 0f, z + Jitter(rng, 4f));
                bool outsideWall = Mathf.Abs(p.x) > 155f || Mathf.Abs(p.z) > 155f;
                bool onRoad = Mathf.Abs(p.x) < 10f && p.z > 140f;
                if (outsideWall && !onRoad) Tree(forest, n++, p, rng, trunk);
            }

            // Both sides of the fog road.
            for (float z = 156f; z <= 384f; z += 8f)
            for (float x = 10f; x <= 38f; x += 8f)
            {
                Tree(forest, n++, new Vector3(x + Jitter(rng, 2.5f), 0f, z + Jitter(rng, 3f)), rng, trunk);
                Tree(forest, n++, new Vector3(-x + Jitter(rng, 2.5f), 0f, z + Jitter(rng, 3f)), rng, trunk);
            }
        }

        static float Jitter(System.Random rng, float amount) => ((float)rng.NextDouble() * 2f - 1f) * amount;

        static void Tree(Transform parent, int n, Vector3 pos, System.Random rng, Material trunk)
        {
            float h = 7f + (float)rng.NextDouble() * 5f;
            var t = Node(parent, $"Tree_{n:000}", pos);
            Cyl(t, "Trunk", new Vector3(0f, h * 0.5f, 0f), 0.5f, h, trunk);
            DeadTree(t, n, h); // M1.5: bare limbs instead of the greybox crown
        }

        static void Blocker(Transform parent, string name, Vector3 pos, Vector3 size)
        {
            var t = Node(parent, name, pos);
            var box = t.gameObject.AddComponent<BoxCollider>();
            box.size = size;
        }

        // ------------------------------------------------------------ houses

        static House BuildHouse(Transform parent, int index, Vector3 origin)
        {
            var def = HouseCatalog.Houses[index];
            var root = Node(parent, $"House_{index:00} {def.Name}", origin);
            var house = root.gameObject.AddComponent<House>();
            house.Index = index;
            float hx = HouseW * 0.5f, hz = HouseD * 0.5f;

            BuildHouseShell(root, def);
            house.Interior = WorldBounds(root, new Vector3(0f, HouseH * 0.5f, 0f), new Vector3(HouseW - 2f * WallT, HouseH, HouseD - 2f * WallT));

            // Front door: hinge on the west side, swings inwards (+90 about y).
            var doorPivot = Node(root, "FrontDoor", new Vector3(-DoorWidth * 0.5f, 0f, hz - WallT * 0.5f));
            var doorVisual = Node(doorPivot, "Visual");
            var doorMat = Mat($"Door_{def.Door}", HouseCatalog.DoorTint(def.Door), 0.3f);
            Box(doorVisual, "Leaf", new Vector3(DoorWidth * 0.5f, DoorHeight * 0.5f, 0f), new Vector3(DoorWidth - 0.04f, DoorHeight - 0.02f, 0.08f), doorMat);
            Ball(doorVisual, "KnobOut", new Vector3(DoorWidth - 0.18f, 1.0f, 0.07f), 0.08f, Mat("Metal", new Color(0.22f, 0.23f, 0.25f), 0.5f), collider: false);
            Ball(doorVisual, "KnobIn", new Vector3(DoorWidth - 0.18f, 1.0f, -0.07f), 0.08f, Mat("Metal", new Color(0.22f, 0.23f, 0.25f), 0.5f), collider: false);
            ExcludeFromNavMesh(doorPivot.gameObject);
            AddCarvingObstacle(doorPivot.gameObject, new Vector3(DoorWidth * 0.5f, DoorHeight * 0.5f, 0f), new Vector3(DoorWidth, DoorHeight, 0.3f));
            house.Door = doorPivot.gameObject.AddComponent<Door>();
            house.Door.House = house;

            BuildHouseFront(root, def, house, hz);

            // Rooms: hall across the front, hearth room back-left (west), puzzle room back-right (east).
            var rooms = Node(root, "Rooms");
            float innerFront = InnerWallZ + WallT * 0.5f, innerBack = InnerWallZ - WallT * 0.5f;
            float inX = hx - WallT, inZ = hz - WallT;
            var hall = BuildRoom(rooms, house, "Hall", new Vector3(0f, 0f, (innerFront + inZ) * 0.5f), new Vector3(2f * inX, HouseH, inZ - innerFront), false);
            var hearthRoom = BuildRoom(rooms, house, "HearthRoom", new Vector3((-inX - WallT * 0.5f) * 0.5f, 0f, (-inZ + innerBack) * 0.5f), new Vector3(inX - WallT * 0.5f, HouseH, innerBack + inZ), true);
            var puzzleRoom = BuildRoom(rooms, house, "PuzzleRoom", new Vector3((inX + WallT * 0.5f) * 0.5f, 0f, (-inZ + innerBack) * 0.5f), new Vector3(inX - WallT * 0.5f, HouseH, innerBack + inZ), false);
            house.Rooms = new[] { hall, hearthRoom, puzzleRoom };
            house.HearthRoom = hearthRoom;

            var interior = Node(root, "Interior");
            house.Hearth = BuildHearth(interior, house, hearthRoom.Bounds.center.x - origin.x, -inZ);
            house.FuseBox = BuildFuseBox(interior, house, new Vector3(-2.3f, 1.5f, inZ));
            BuildLightSwitch(interior, "Switch_Hall", new Vector3(1.15f, 1.3f, inZ), hall);
            BuildLightSwitch(interior, "Switch_HearthRoom", new Vector3(-4.3f, 1.3f, innerBack), hearthRoom);
            BuildLightSwitch(interior, "Switch_PuzzleRoom", new Vector3(4.3f, 1.3f, innerBack), puzzleRoom);
            house.Wardrobe = BuildWardrobe(interior, new Vector3(inX, 0f, -1.4f));

            // The hearth room is a safe zone while the fire burns: a carving obstacle keeps the Hollow's paths out.
            var zone = Node(root, "HearthSafeZone", root.InverseTransformPoint(hearthRoom.Bounds.center));
            var zoneObstacle = zone.gameObject.AddComponent<NavMeshObstacle>();
            zoneObstacle.shape = NavMeshObstacleShape.Box;
            zoneObstacle.size = hearthRoom.Bounds.size;
            zoneObstacle.carving = true;
            zoneObstacle.carveOnlyStationary = true;
            zoneObstacle.enabled = false; // MatchDirector lights the hearth
            house.Hearth.SafeZone = zoneObstacle;

            var notes = new List<Readable>();
            var itemSpots = new List<Transform>();
            notes.Add(BuildTable(interior, "Table_Hall", new Vector3(-4.4f, 0f, 2.7f)).gameObject.AddComponent<Readable>());
            notes.Add(BuildTable(interior, "Table_HearthRoom", new Vector3(-5.0f, 0f, -1.0f)).gameObject.AddComponent<Readable>());
            notes.Add(BuildTable(interior, "Table_PuzzleRoom", new Vector3(1.0f, 0f, -2.5f)).gameObject.AddComponent<Readable>());
            var sideTable = BuildTable(interior, "SideTable_Hall", new Vector3(4.6f, 0f, 2.7f), paper: false);
            itemSpots.Add(Node(interior, "ItemSpot_Mantel", new Vector3(hearthRoom.Bounds.center.x - origin.x + 0.75f, 1.43f, -inZ + 0.45f)));
            itemSpots.Add(Node(interior, "ItemSpot_HallTable", interior.InverseTransformPoint(sideTable.position)));
            itemSpots.Add(Node(interior, "ItemSpot_PuzzleTable", new Vector3(1.25f, 0.76f, -2.35f)));
            house.Notes = notes.ToArray();
            house.ItemSpots = itemSpots.ToArray();

            // DESIGN.md "Items and tools": matches in every house; batteries in drawers.
            BuildPickup(interior, "Matches", new Vector3(-4.1f, 0.76f, 2.8f), ItemKind.Matches, "a box of matches", 5);
            BuildPickup(interior, "Battery", new Vector3(4.85f, 0.76f, 2.85f), ItemKind.Battery, "a battery");

            house.Container = BuildContainer(Node(interior, "PartContainer", new Vector3(4.85f, 0f, -3.55f)), house);
            house.Puzzle = BuildPuzzleRig(Node(interior, "PuzzleRig", new Vector3(3f, 0f, -inZ)), interior, house, house.Container);
            DressHouse(root, house);

            return house;
        }

        static void BuildHouseShell(Transform root, HouseDef def)
        {
            float hx = HouseW * 0.5f, hz = HouseD * 0.5f;
            var wall = Mat("HouseWall", new Color(0.70f, 0.64f, 0.54f));
            var inner = Mat("InnerWall", new Color(0.62f, 0.56f, 0.46f));
            var roof = Mat("Roof", new Color(0.28f, 0.15f, 0.11f));
            var window = Mat("Window", new Color(0.07f, 0.09f, 0.12f), 0.8f);

            var shell = Node(root, "Shell"); // art slot: walls, floor, roof, chimneys
            Box(shell, "Floor", new Vector3(0f, 0.03f, 0f), new Vector3(HouseW, 0.06f, HouseD), Mat("Floor", new Color(0.33f, 0.24f, 0.17f)));
            WallAlongX(shell, "WallFront", hz - WallT * 0.5f, -hx, hx, HouseH, WallT, wall, new Gap(0f, DoorWidth, DoorHeight));
            WallAlongX(shell, "WallBack", -hz + WallT * 0.5f, -hx, hx, HouseH, WallT, wall);
            WallAlongZ(shell, "WallWest", -hx + WallT * 0.5f, -hz + WallT, hz - WallT, HouseH, WallT, wall);
            WallAlongZ(shell, "WallEast", hx - WallT * 0.5f, -hz + WallT, hz - WallT, HouseH, WallT, wall);
            WallAlongX(shell, "WallInner", InnerWallZ, -hx + WallT, hx - WallT, HouseH, WallT, inner,
                new Gap(-3f, InnerDoorWidth, DoorHeight), new Gap(3f, InnerDoorWidth, DoorHeight));
            WallAlongZ(shell, "WallPartition", 0f, -hz + WallT, InnerWallZ - WallT * 0.5f, HouseH, WallT, inner);
            Box(shell, "Ceiling", new Vector3(0f, HouseH + 0.075f, 0f), new Vector3(HouseW, 0.15f, HouseD), Mat("Ceiling", new Color(0.55f, 0.52f, 0.48f)));

            // Pitched roof with the ridge along x, and gable ends.
            float halfDepth = hz + 0.3f;
            float slab = halfDepth / Mathf.Cos(RoofPitch * Mathf.Deg2Rad);
            float rise = halfDepth * Mathf.Tan(RoofPitch * Mathf.Deg2Rad);
            float eaves = HouseH + 0.15f;
            Box(shell, "RoofFront", new Vector3(0f, eaves + rise * 0.5f, halfDepth * 0.5f), new Vector3(HouseW + 0.6f, 0.2f, slab), roof, true, Quaternion.Euler(RoofPitch, 0f, 0f));
            Box(shell, "RoofBack", new Vector3(0f, eaves + rise * 0.5f, -halfDepth * 0.5f), new Vector3(HouseW + 0.6f, 0.2f, slab), roof, true, Quaternion.Euler(-RoofPitch, 0f, 0f));
            float gable = halfDepth * Mathf.Sqrt(2f); // a square turned 45 degrees: its top half is the gable triangle
            Box(shell, "GableWest", new Vector3(-hx + WallT * 0.5f, eaves, 0f), new Vector3(WallT - 0.04f, gable, gable), wall, false, Quaternion.Euler(45f, 0f, 0f));
            Box(shell, "GableEast", new Vector3(hx - WallT * 0.5f, eaves, 0f), new Vector3(WallT - 0.04f, gable, gable), wall, false, Quaternion.Euler(45f, 0f, 0f));

            // Chimneys rise above the ridge so they can be counted from the street (docs/DESIGN.md "Day phase").
            var chimney = Mat("Chimney", new Color(0.42f, 0.22f, 0.17f));
            float[][] chimneyX = { new[] { -3f }, new[] { -3f, 3f }, new[] { -3.5f, 0.5f, 4f } };
            float ridge = eaves + rise;
            foreach (var x in chimneyX[Mathf.Clamp(def.Chimneys, 1, 3) - 1])
            {
                float top = ridge + 1.2f;
                Box(shell, $"Chimney_{x:0.0}", new Vector3(x, (HouseH + top) * 0.5f, 0f), new Vector3(0.8f, top - HouseH, 0.8f), chimney);
            }

            // Windows: dark panes through the wall.
            float pane = WallT + 0.04f;
            Box(shell, "Window_FrontW", new Vector3(-3.5f, 1.7f, hz - WallT * 0.5f), new Vector3(1.3f, 1.1f, pane), window, false);
            Box(shell, "Window_FrontE", new Vector3(3.5f, 1.7f, hz - WallT * 0.5f), new Vector3(1.3f, 1.1f, pane), window, false);
            Box(shell, "Window_BackW", new Vector3(-4.6f, 1.7f, -hz + WallT * 0.5f), new Vector3(1.1f, 1.0f, pane), window, false);
            Box(shell, "Window_BackE", new Vector3(1.4f, 1.7f, -hz + WallT * 0.5f), new Vector3(1.1f, 1.0f, pane), window, false);
            Box(shell, "Window_West", new Vector3(-hx + WallT * 0.5f, 1.7f, -1.7f), new Vector3(pane, 1.0f, 1.2f), window, false);
            Box(shell, "Window_East", new Vector3(hx - WallT * 0.5f, 1.7f, 2.7f), new Vector3(pane, 1.0f, 1.2f), window, false);
        }

        /// <summary>Exterior traits a letter can describe: nameplate and stopped clock; plus the hidden shrine marker.</summary>
        static void BuildHouseFront(Transform root, HouseDef def, House house, float hz)
        {
            var front = Node(root, "Front");
            var dark = Mat("Dark", new Color(0.08f, 0.08f, 0.08f));
            var cream = Mat("Cream", new Color(0.90f, 0.87f, 0.78f));
            var faceOut = Quaternion.Euler(0f, 180f, 0f); // labels on the front wall read from the street

            var plate = Node(front, "Nameplate", new Vector3(-1.6f, 2.0f, hz));
            Box(plate, "Board", new Vector3(0f, 0f, 0.02f), new Vector3(1.1f, 0.3f, 0.04f), Mat("Wood", new Color(0.25f, 0.17f, 0.11f)), collider: false);
            Label(plate, "Name", def.Family.ToUpperInvariant(), new Vector3(0f, 0f, 0.045f), faceOut, 0.16f, new Color(0.92f, 0.88f, 0.75f));

            // The stopped clock beside the door. Clockwise as seen from the street is negative about +z.
            var clock = Node(front, "StoppedClock", new Vector3(1.5f, 1.75f, hz));
            Cyl(clock, "Rim", new Vector3(0f, 0f, 0.015f), 0.62f, 0.03f, dark, false, Quaternion.Euler(90f, 0f, 0f));
            Cyl(clock, "Face", new Vector3(0f, 0f, 0.035f), 0.54f, 0.01f, cream, false, Quaternion.Euler(90f, 0f, 0f));
            var hour = Node(clock, "HourHand", new Vector3(0f, 0f, 0.045f), Quaternion.Euler(0f, 0f, -(def.ClockHour % 12) * 30f));
            Box(hour, "Hand", new Vector3(0f, 0.08f, 0f), new Vector3(0.04f, 0.16f, 0.01f), dark, false);
            var minute = Node(clock, "MinuteHand", new Vector3(0f, 0f, 0.05f));
            Box(minute, "Hand", new Vector3(0f, 0.12f, 0f), new Vector3(0.025f, 0.24f, 0.01f), dark, false);

            // docs/DESIGN.md "Shrine houses": a carved diamond above the door, hidden unless this house is a shrine.
            var marker = Node(front, "ShrineMarker", new Vector3(0f, 2.62f, hz + 0.06f));
            Box(marker, "Diamond", Vector3.zero, new Vector3(0.32f, 0.32f, 0.05f), Mat("Shrine", new Color(0.95f, 0.9f, 0.7f), 0.3f, new Color(0.9f, 0.8f, 0.5f)), false, Quaternion.Euler(0f, 0f, 45f));
            marker.gameObject.SetActive(false);
            house.ShrineMarker = marker.gameObject;
        }

        static Room BuildRoom(Transform parent, House house, string name, Vector3 localCenterAtFloor, Vector3 size, bool hearthRoom)
        {
            var t = Node(parent, name, localCenterAtFloor);
            var room = t.gameObject.AddComponent<Room>();
            room.House = house;
            room.IsHearthRoom = hearthRoom;
            room.Bounds = new Bounds(t.position + Vector3.up * size.y * 0.5f, size);
            room.Light = PointLight(t, "CeilingLight", new Vector3(0f, HouseH - 0.25f, 0f), new Color(1f, 0.82f, 0.6f), 7f, 2.5f);
            room.Bulb = Ball(t, "Bulb", new Vector3(0f, HouseH - 0.12f, 0f), 0.14f, Mat("Bulb", new Color(1f, 0.95f, 0.8f), 0.5f, new Color(2f, 1.8f, 1.4f)), collider: false);
            return room;
        }

        static Hearth BuildHearth(Transform interior, House house, float x, float backWallZ)
        {
            var stone = Mat("HearthStone", new Color(0.35f, 0.33f, 0.31f));
            var hearth = Node(interior, "Hearth", new Vector3(x, 0f, backWallZ));
            var visual = Node(hearth, "Visual");
            Box(visual, "Base", new Vector3(0f, 0.65f, 0.4f), new Vector3(2.2f, 1.3f, 0.8f), stone);
            Box(visual, "Firebox", new Vector3(0f, 0.45f, 0.81f), new Vector3(1.1f, 0.75f, 0.03f), Mat("Soot", new Color(0.04f, 0.035f, 0.03f)), false);
            Box(visual, "Mantel", new Vector3(0f, 1.36f, 0.45f), new Vector3(2.5f, 0.12f, 0.95f), Mat("Wood", new Color(0.25f, 0.17f, 0.11f)));
            Box(visual, "Breast", new Vector3(0f, 2.2f, 0.3f), new Vector3(1.6f, 1.6f, 0.6f), stone);
            var fire = Node(hearth, "Fire", new Vector3(0f, 0.3f, 1.0f));
            Ball(fire, "Flames", Vector3.zero, 0.38f, Mat("Fire", new Color(1f, 0.5f, 0.15f), 0.2f, new Color(4f, 1.6f, 0.4f)), collider: false);
            PointLight(fire, "FireLight", new Vector3(0f, 0.3f, 0.2f), new Color(1f, 0.55f, 0.25f), 6f, 5f);
            fire.gameObject.SetActive(false);

            var component = hearth.gameObject.AddComponent<Hearth>();
            component.House = house;
            component.Fire = fire.gameObject;
            return component;
        }

        static FuseBox BuildFuseBox(Transform interior, House house, Vector3 localPos)
        {
            var box = Node(interior, "FuseBox", localPos, Quaternion.Euler(0f, 180f, 0f)); // +z points into the hall
            var visual = Node(box, "Visual");
            Box(visual, "Case", new Vector3(0f, 0f, 0.07f), new Vector3(0.45f, 0.6f, 0.14f), Mat("Metal", new Color(0.22f, 0.23f, 0.25f), 0.5f));
            var lever = Node(box, "Lever", new Vector3(0.12f, 0f, 0.16f));
            Box(lever, "Handle", new Vector3(0f, 0.06f, 0f), new Vector3(0.05f, 0.14f, 0.05f), Mat("Dark", new Color(0.08f, 0.08f, 0.08f)), false);
            var fuse = box.gameObject.AddComponent<FuseBox>();
            fuse.House = house;
            fuse.Lever = lever;
            return fuse;
        }

        static void BuildLightSwitch(Transform interior, string name, Vector3 localPos, Room room)
        {
            var sw = Node(interior, name, localPos, Quaternion.Euler(0f, 180f, 0f));
            Box(Node(sw, "Visual"), "Plate", new Vector3(0f, 0f, 0.015f), new Vector3(0.09f, 0.13f, 0.03f), Mat("Cream", new Color(0.90f, 0.87f, 0.78f)));
            sw.gameObject.AddComponent<LightSwitch>().Room = room;
        }

        static HidingSpot BuildWardrobe(Transform interior, Vector3 localPos)
        {
            var wardrobe = Node(interior, "Wardrobe", localPos, Quaternion.Euler(0f, -90f, 0f)); // +z points west, into the room
            var visual = Node(wardrobe, "Visual");
            var wood = Mat("Wood", new Color(0.25f, 0.17f, 0.11f));
            Box(visual, "Body", new Vector3(0f, 1.05f, 0.33f), new Vector3(1.2f, 2.1f, 0.65f), wood);
            Box(visual, "DoorGap", new Vector3(0f, 1.05f, 0.66f), new Vector3(0.02f, 1.9f, 0.01f), Mat("Soot", new Color(0.04f, 0.035f, 0.03f)), false);
            PropKit.SwapVisual(visual, PropKit.Wardrobe);
            var spot = wardrobe.gameObject.AddComponent<HidingSpot>();
            spot.Inside = Node(wardrobe, "Inside", new Vector3(0f, 0f, 0.33f));
            return spot;
        }

        /// <summary>A small table; returns the note spot on top (a sheet of paper unless paper is false).</summary>
        static Transform BuildTable(Transform interior, string name, Vector3 localPos, bool paper = true)
        {
            var table = Node(interior, name, localPos);
            var visual = Node(table, "Visual");
            var wood = Mat("Wood", new Color(0.25f, 0.17f, 0.11f));
            TableLook(visual, Box(visual, "Top", new Vector3(0f, 0.375f, 0f), new Vector3(0.9f, 0.75f, 0.6f), wood), wood);
            var spot = Node(table, "NoteSpot", new Vector3(-0.15f, 0.76f, 0f));
            if (paper)
                Box(spot, "Paper", new Vector3(0f, 0.003f, 0f), new Vector3(0.22f, 0.006f, 0.3f), Mat("Paper", new Color(0.88f, 0.85f, 0.76f)));
            return spot;
        }

        /// <summary>The house's puzzle rig against the back wall of the puzzle room (docs/DESIGN.md "Houses and puzzles").</summary>
        static PuzzleRig BuildPuzzleRig(Transform anchor, Transform interior, House house, PartContainer container)
        {
            var metal = Mat("Metal", new Color(0.22f, 0.23f, 0.25f), 0.5f);
            var wood = Mat("Wood", new Color(0.25f, 0.17f, 0.11f));
            var dark = Mat("Dark", new Color(0.08f, 0.08f, 0.08f));
            var cream = Mat("Cream", new Color(0.90f, 0.87f, 0.78f));
            // The rig faces into the room (+z), so from the player's side its -x is on the right:
            // rows run from +x to -x so dials, symbols and lamps read left to right.
            var intoRoom = Quaternion.Euler(0f, 180f, 0f); // labels on the back wall read from inside the room
            var visual = Node(anchor, "Visual");
            PuzzleRig rig;

            switch (house.Def.Puzzle)
            {
                case PuzzleKind.CodeLock:
                {
                    Box(visual, "Stand", new Vector3(0f, 0.45f, 0.18f), new Vector3(1.0f, 0.9f, 0.36f), wood);
                    Box(visual, "Safe", new Vector3(0f, 1.25f, 0.18f), new Vector3(1.0f, 0.7f, 0.36f), metal);
                    var code = anchor.gameObject.AddComponent<CodeLockPuzzle>();
                    code.DialParts = new CodeDial[4];
                    for (int i = 0; i < 4; i++)
                    {
                        var dial = Node(anchor, $"Dial_{i}", new Vector3(0.33f - 0.22f * i, 1.25f, 0.36f));
                        Cyl(Node(dial, "Visual"), "Knob", new Vector3(0f, 0f, 0.025f), 0.17f, 0.05f, dark, true, Quaternion.Euler(90f, 0f, 0f));
                        var part = dial.gameObject.AddComponent<CodeDial>();
                        part.Rig = code;
                        part.Index = i;
                        part.Digit = Label(dial, "Digit", "0", new Vector3(0f, 0f, 0.055f), intoRoom, 0.1f, Color.white);
                        code.DialParts[i] = part;
                    }
                    rig = code;
                    break;
                }

                case PuzzleKind.Clock:
                {
                    Box(visual, "Case", new Vector3(0f, 1.1f, 0.23f), new Vector3(0.7f, 2.2f, 0.45f), wood);
                    Box(visual, "PendulumWindow", new Vector3(0f, 0.8f, 0.46f), new Vector3(0.34f, 0.8f, 0.02f), Mat("Window", new Color(0.07f, 0.09f, 0.12f), 0.8f), false);
                    PropKit.SwapVisual(visual, PropKit.GrandfatherClock);
                    var clock = anchor.gameObject.AddComponent<ClockPuzzle>();
                    var face = Node(anchor, "Face", new Vector3(0f, 1.75f, 0.45f));
                    Cyl(Node(face, "Visual"), "Dial", new Vector3(0f, 0f, 0.01f), 0.5f, 0.02f, cream, true, Quaternion.Euler(90f, 0f, 0f));
                    var hour = Node(face, "HourHand", new Vector3(0f, 0f, 0.03f));
                    Box(hour, "Hand", new Vector3(0f, 0.07f, 0f), new Vector3(0.035f, 0.14f, 0.01f), dark, false);
                    var minute = Node(face, "MinuteHand", new Vector3(0f, 0f, 0.035f));
                    Box(minute, "Hand", new Vector3(0f, 0.1f, 0f), new Vector3(0.02f, 0.2f, 0.01f), dark, false);
                    face.gameObject.AddComponent<ClockFace>().Rig = clock;
                    var keyhole = Node(anchor, "Keyhole", new Vector3(0f, 1.38f, 0.46f));
                    Box(Node(keyhole, "Visual"), "Plate", Vector3.zero, new Vector3(0.12f, 0.16f, 0.02f), metal);
                    keyhole.gameObject.AddComponent<ClockKeyhole>().Rig = clock;
                    clock.HourHand = hour;
                    // The winding key; MatchDirector moves it to the item spot the plan picked.
                    clock.Key = BuildPickup(interior, "WindingKey", interior.InverseTransformPoint(house.ItemSpots[0].position), ItemKind.WindingKey, "the winding key");
                    rig = clock;
                    break;
                }

                default:
                {
                    Box(visual, "Panel", new Vector3(0f, 1.35f, 0.04f), new Vector3(1.3f, 0.95f, 0.08f), wood);
                    var sequence = anchor.gameObject.AddComponent<SequencePuzzle>();
                    for (int i = 0; i < HouseCatalog.SequenceSymbols.Length; i++)
                    {
                        float x = 0.4f - 0.4f * (i % 3), y = i < 3 ? 1.55f : 1.15f;
                        var button = Node(anchor, $"Symbol_{HouseCatalog.SequenceSymbols[i]}", new Vector3(x, y, 0.08f));
                        Box(Node(button, "Visual"), "Button", new Vector3(0f, 0f, 0.03f), new Vector3(0.32f, 0.32f, 0.06f), cream);
                        Label(button, "Label", HouseCatalog.SequenceSymbols[i].ToUpperInvariant(), new Vector3(0f, 0f, 0.065f), intoRoom, 0.07f, new Color(0.1f, 0.08f, 0.06f));
                        var part = button.gameObject.AddComponent<SymbolButton>();
                        part.Rig = sequence;
                        part.Symbol = i;
                    }
                    sequence.LampOff = Mat("LampOff", new Color(0.15f, 0.15f, 0.12f));
                    sequence.LampOn = Mat("LampOn", new Color(1f, 0.85f, 0.5f), 0.4f, new Color(3f, 2.2f, 1f));
                    sequence.Lamps = new Renderer[4];
                    for (int i = 0; i < 4; i++)
                        sequence.Lamps[i] = Ball(Node(anchor, $"Lamp_{i}", new Vector3(0.3f - 0.2f * i, 1.92f, 0.09f)), "Bulb", Vector3.zero, 0.07f, sequence.LampOff, collider: false).GetComponent<Renderer>();
                    rig = sequence;
                    break;
                }
            }

            rig.House = house;
            rig.Container = container;
            return rig;
        }

        /// <summary>The chest that holds the house's ritual part until its puzzle is solved.</summary>
        static PartContainer BuildContainer(Transform anchor, House house)
        {
            var wood = Mat("ChestWood", new Color(0.30f, 0.20f, 0.12f));
            var visual = Node(anchor, "Visual");
            Box(visual, "Body", new Vector3(0f, 0.25f, 0f), new Vector3(1.0f, 0.5f, 0.6f), wood);
            var lid = Node(anchor, "Lid", new Vector3(0f, 0.5f, -0.3f)); // hinge on the back edge; opens with Euler(-105, 0, 0)
            Box(lid, "Board", new Vector3(0f, 0.04f, 0.31f), new Vector3(1.02f, 0.08f, 0.62f), wood);

            var container = anchor.gameObject.AddComponent<PartContainer>();
            container.Lid = lid;
            container.Part = BuildPickup(anchor, "RitualPart", new Vector3(0f, 0.5f, 0f), ItemKind.RitualPart, "the " + house.Def.Part.ToLowerInvariant());
            container.Part.PartHouse = house.Index;
            container.Part.gameObject.SetActive(false);
            return container;
        }

        // ------------------------------------------------------------ square and chapel

        static (NoticeBoard board, BellTower bell, Altar altar, RitualCircle circle) BuildSquare(Transform village)
        {
            var square = Node(village, "Square");
            Box(square, "Paving", new Vector3(0f, 0.02f, 117f), new Vector3(70f, 0.04f, 60f), Mat("Paving", new Color(0.42f, 0.40f, 0.37f)));

            // Altar with six numbered slots (docs/DESIGN.md "The Ritual of Passage"), read left to right from the south.
            var altarNode = Node(square, "Altar", AltarPos);
            var altarStone = Mat("AltarStone", new Color(0.30f, 0.29f, 0.28f));
            Box(Node(altarNode, "Visual"), "Block", new Vector3(0f, 0.5f, 0f), new Vector3(3.8f, 1.0f, 1.3f), altarStone);
            var altar = altarNode.gameObject.AddComponent<Altar>();
            altar.Slots = new AltarSlot[6];
            altar.EjectPoint = Node(altarNode, "EjectPoint", new Vector3(0.4f, 0.05f, -1.6f));
            for (int i = 0; i < 6; i++)
            {
                float x = -1.55f + 0.62f * i;
                var slot = Node(altarNode, $"Slot_{i + 1}", new Vector3(x, 1.0f, 0f));
                Box(Node(slot, "Visual"), "Plate", new Vector3(0f, 0.02f, 0f), new Vector3(0.5f, 0.04f, 0.5f), Mat("Dark", new Color(0.08f, 0.08f, 0.08f)));
                Label(altarNode, $"Numeral_{i + 1}", AltarRules.Numeral(i), new Vector3(x, 0.6f, -0.66f), Quaternion.identity, 0.2f, new Color(0.75f, 0.72f, 0.65f));
                altar.Slots[i] = slot.gameObject.AddComponent<AltarSlot>();
                altar.Slots[i].Display = Node(slot, "Display", new Vector3(0f, 0.04f, 0f));
            }

            // Stone circle and its six candles, one per possible part.
            var circleNode = Node(square, "StoneCircle", AltarPos);
            var stone = Mat("StandingStone", new Color(0.45f, 0.44f, 0.42f));
            for (int k = 0; k < 12; k++)
            {
                float a = k * 30f * Mathf.Deg2Rad;
                var p = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * CircleRadius;
                Box(circleNode, $"Stone_{k:00}", p + Vector3.up * 0.95f, new Vector3(0.8f, 1.9f, 0.5f), stone, true, Quaternion.LookRotation(-p));
            }
            var circle = circleNode.gameObject.AddComponent<RitualCircle>();
            circle.Radius = CircleRadius;
            circle.Candles = new Candle[6];
            altar.Circle = circle;
            for (int i = 0; i < 6; i++)
            {
                float a = (15f + 60f * i) * Mathf.Deg2Rad;
                var candle = Node(circleNode, $"Candle_{i + 1}", new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * (CircleRadius - 0.8f));
                var visual = Node(candle, "Visual");
                Box(visual, "Plinth", new Vector3(0f, 0.3f, 0f), new Vector3(0.4f, 0.6f, 0.4f), stone);
                Cyl(visual, "Wax", new Vector3(0f, 0.75f, 0f), 0.1f, 0.3f, Mat("Candle", new Color(0.92f, 0.9f, 0.8f)));
                var flame = Node(candle, "Flame", new Vector3(0f, 0.95f, 0f));
                Ball(flame, "Glow", Vector3.zero, 0.07f, Mat("Flame", new Color(1f, 0.7f, 0.3f), 0.2f, new Color(3f, 1.8f, 0.6f)), collider: false);
                PointLight(flame, "Light", new Vector3(0f, 0.1f, 0f), new Color(1f, 0.7f, 0.35f), 4f, 2.5f);
                flame.gameObject.SetActive(false);
                circle.Candles[i] = candle.gameObject.AddComponent<Candle>();
                circle.Candles[i].Flame = flame.gameObject;
            }

            // Notice board facing the houses (south), six letter spots.
            var board = Node(square, "NoticeBoard", new Vector3(14f, 0f, 96f));
            var boardVisual = Node(board, "Visual");
            var wood = Mat("Wood", new Color(0.25f, 0.17f, 0.11f));
            Box(boardVisual, "PostW", new Vector3(-1.6f, 1.2f, 0f), new Vector3(0.15f, 2.4f, 0.15f), wood);
            Box(boardVisual, "PostE", new Vector3(1.6f, 1.2f, 0f), new Vector3(0.15f, 2.4f, 0.15f), wood);
            Box(boardVisual, "Board", new Vector3(0f, 1.6f, 0f), new Vector3(3.4f, 1.5f, 0.08f), Mat("Cork", new Color(0.45f, 0.33f, 0.22f)));
            Box(boardVisual, "Roof", new Vector3(0f, 2.48f, -0.05f), new Vector3(3.8f, 0.1f, 0.5f), wood);
            Label(board, "Heading", "LETTERS FOR OUR GUESTS", new Vector3(0f, 2.24f, -0.05f), Quaternion.identity, 0.14f, new Color(0.92f, 0.88f, 0.75f));
            var noticeBoard = board.gameObject.AddComponent<NoticeBoard>();
            noticeBoard.Letters = new Readable[6];
            for (int i = 0; i < 6; i++)
            {
                var letter = Node(board, $"Letter_{i + 1}", new Vector3(-1.1f + 1.1f * (i % 3), i < 3 ? 1.88f : 1.3f, -0.05f));
                Box(Node(letter, "Visual"), "Envelope", Vector3.zero, new Vector3(0.36f, 0.26f, 0.01f), Mat("Paper", new Color(0.88f, 0.85f, 0.76f)));
                noticeBoard.Letters[i] = letter.gameObject.AddComponent<Readable>();
                noticeBoard.Letters[i].Set("A sealed letter", "", "Read the letter");
            }

            // Bell tower on the square.
            var tower = Node(square, "BellTower", new Vector3(26f, 0f, 132f));
            var towerVisual = Node(tower, "Visual");
            var masonry = Mat("Stone", new Color(0.45f, 0.44f, 0.42f));
            Box(towerVisual, "Shaft", new Vector3(0f, 8f, 0f), new Vector3(4f, 16f, 4f), masonry);
            for (int c = 0; c < 4; c++)
            {
                float px = (c % 2 == 0 ? -1.75f : 1.75f), pz = (c < 2 ? -1.75f : 1.75f);
                Box(towerVisual, $"Post{c}", new Vector3(px, 17.5f, pz), new Vector3(0.5f, 3f, 0.5f), masonry);
            }
            Box(towerVisual, "Cap", new Vector3(0f, 19.2f, 0f), new Vector3(4.6f, 0.4f, 4.6f), Mat("ChapelRoof", new Color(0.24f, 0.24f, 0.26f)));
            var bell = Node(tower, "Bell", new Vector3(0f, 17.6f, 0f));
            Ball(bell, "Visual", Vector3.zero, 1.3f, Mat("Bronze", new Color(0.55f, 0.38f, 0.2f), 0.6f), collider: false);
            var bellTower = tower.gameObject.AddComponent<BellTower>();
            bellTower.Bell = bell;

            return (noticeBoard, bellTower, altar, circle);
        }

        /// <summary>The chapel west of the square, with the chained Ritual Book. Returns the Hollow's night spawn.</summary>
        static Transform BuildChapel(Transform village, out Readable ritualBook)
        {
            const float w = 12f, d = 18f, h = 7f, t = 0.4f;
            var wallMat = Mat("ChapelWall", new Color(0.58f, 0.57f, 0.55f));
            var wood = Mat("Wood", new Color(0.25f, 0.17f, 0.11f));
            var chapel = Node(village, "Chapel", new Vector3(-60f, 0f, 112f));
            var shell = Node(chapel, "Shell");
            Box(shell, "Floor", new Vector3(0f, 0.03f, 0f), new Vector3(w, 0.06f, d), Mat("Paving", new Color(0.42f, 0.40f, 0.37f)));
            WallAlongZ(shell, "WallEast", w * 0.5f - t * 0.5f, -d * 0.5f, d * 0.5f, h, t, wallMat, new Gap(0f, 2.4f, 3.2f));
            WallAlongZ(shell, "WallWest", -w * 0.5f + t * 0.5f, -d * 0.5f, d * 0.5f, h, t, wallMat);
            WallAlongX(shell, "WallNorth", d * 0.5f - t * 0.5f, -w * 0.5f, w * 0.5f, h, t, wallMat);
            WallAlongX(shell, "WallSouth", -d * 0.5f + t * 0.5f, -w * 0.5f, w * 0.5f, h, t, wallMat);
            Box(shell, "Ceiling", new Vector3(0f, h + 0.075f, 0f), new Vector3(w, 0.15f, d), Mat("Ceiling", new Color(0.55f, 0.52f, 0.48f)));
            const float pitch = 35f;
            float halfW = w * 0.5f + 0.3f;
            float slab = halfW / Mathf.Cos(pitch * Mathf.Deg2Rad), rise = halfW * Mathf.Tan(pitch * Mathf.Deg2Rad);
            var roof = Mat("ChapelRoof", new Color(0.24f, 0.24f, 0.26f));
            Box(shell, "RoofEast", new Vector3(halfW * 0.5f, h + 0.15f + rise * 0.5f, 0f), new Vector3(slab, 0.2f, d + 0.6f), roof, true, Quaternion.Euler(0f, 0f, -pitch));
            Box(shell, "RoofWest", new Vector3(-halfW * 0.5f, h + 0.15f + rise * 0.5f, 0f), new Vector3(slab, 0.2f, d + 0.6f), roof, true, Quaternion.Euler(0f, 0f, pitch));

            // Pews either side of the aisle.
            for (int row = 0; row < 4; row++)
            {
                float x = 2.5f - row * 1.6f;
                Box(shell, $"PewN{row}", new Vector3(x, 0.25f, 3.6f), new Vector3(0.5f, 0.5f, 4f), wood);
                Box(shell, $"PewS{row}", new Vector3(x, 0.25f, -3.6f), new Vector3(0.5f, 0.5f, 4f), wood);
            }

            // Lectern with the chained Ritual Book, facing the door (east).
            var lectern = Node(chapel, "RitualBook", new Vector3(-3.6f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));
            var visual = Node(lectern, "Visual");
            Box(visual, "Post", new Vector3(0f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 0.35f), wood);
            Box(visual, "Top", new Vector3(0f, 1.15f, 0f), new Vector3(0.7f, 0.05f, 0.5f), wood, true, Quaternion.Euler(-15f, 0f, 0f));
            Box(visual, "Book", new Vector3(0f, 1.2f, 0.01f), new Vector3(0.48f, 0.06f, 0.34f), Mat("BookLeather", new Color(0.32f, 0.1f, 0.08f)), true, Quaternion.Euler(-15f, 0f, 0f));
            Box(visual, "Chain", new Vector3(0.2f, 0.75f, 0f), new Vector3(0.03f, 0.75f, 0.03f), Mat("Metal", new Color(0.22f, 0.23f, 0.25f), 0.5f), false);
            ritualBook = lectern.gameObject.AddComponent<Readable>();
            ritualBook.Set("The Ritual Book", "", "Read the Ritual Book");

            return Node(chapel, "HollowSpawn", new Vector3(-4.6f, 0f, 5f));
        }

        // ------------------------------------------------------------ layout, lights, player, Hollow

        static VillageLayout BuildLayout(Transform hollowSpawn)
        {
            var layout = new GameObject("Layout").AddComponent<VillageLayout>();
            var t = layout.transform;
            // Arrival: the guests start on the road and walk in through the open gate (docs/DESIGN.md "Core loop").
            layout.PlayerSpawn = Node(t, "PlayerSpawn", new Vector3(0f, 0.1f, Half + 12f), Quaternion.Euler(0f, 180f, 0f));
            layout.HollowNightSpawn = hollowSpawn;
            layout.SquareCenter = Node(t, "SquareCenter", AltarPos);
            layout.CircleRadius = CircleRadius;
            layout.GateLineZ = Half;
            layout.BridgeZ = BridgeZ;

            var forest = new List<Transform>();
            Vector3[] forestPoints =
            {
                new Vector3(-140f, 0f, 140f), new Vector3(140f, 0f, 140f), new Vector3(-140f, 0f, 0f), new Vector3(140f, 0f, 0f),
                new Vector3(-140f, 0f, -140f), new Vector3(0f, 0f, -140f), new Vector3(140f, 0f, -140f),
            };
            for (int i = 0; i < forestPoints.Length; i++) forest.Add(Node(t, $"ForestSpawn_{i}", forestPoints[i]));
            layout.ForestSpawns = forest.ToArray();

            // Street crossings between the houses, plus the square.
            var roam = new List<Transform>();
            float[] xs = { -120f, -72f, -24f, 24f, 72f, 120f };
            float[] zs = { 80f, 30f, -20f, -75f, -120f };
            foreach (var z in zs)
            foreach (var x in xs)
                roam.Add(Node(t, $"Roam_{roam.Count:00}", new Vector3(x, 0f, z)));
            roam.Add(Node(t, $"Roam_{roam.Count:00}", new Vector3(-20f, 0f, 100f)));
            roam.Add(Node(t, $"Roam_{roam.Count:00}", new Vector3(20f, 0f, 100f)));
            roam.Add(Node(t, $"Roam_{roam.Count:00}", new Vector3(-22f, 0f, 135f)));
            layout.RoamPoints = roam.ToArray();
            return layout;
        }

        static PhaseLighting BuildLights()
        {
            var sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.93f, 0.8f);
            sun.intensity = 1.3f;
            sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
            RenderSettings.sun = sun;

            var moon = new GameObject("Moon").AddComponent<Light>();
            moon.type = LightType.Directional;
            moon.color = new Color(0.55f, 0.65f, 0.9f);
            moon.intensity = 0.12f;
            moon.shadows = LightShadows.Soft;
            moon.transform.rotation = Quaternion.Euler(35f, 160f, 0f);
            moon.enabled = false;

            var lighting = new GameObject("Lighting").AddComponent<PhaseLighting>();
            lighting.Sun = sun;
            lighting.Moon = moon;
            return lighting;
        }

        static PlayerAgent BuildPlayer(Transform spawn)
        {
            var player = new GameObject("Player");
            player.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            var cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.35f;
            cc.center = new Vector3(0f, 0.9f, 0f);
            cc.stepOffset = 0.35f;
            cc.slopeLimit = 50f;

            var body = Prim(PrimitiveType.Capsule, Node(player.transform, "Visual"), "Body", new Vector3(0f, 0.9f, 0f), new Vector3(0.7f, 0.9f, 0.7f),
                            Mat("PlayerBody", new Color(0.35f, 0.4f, 0.45f)), collider: false);
            NoShadows(body);

            var head = Node(player.transform, "Head", new Vector3(0f, 1.65f, 0f));
            head.gameObject.tag = "MainCamera";
            var cam = head.gameObject.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 500f;
            cam.fieldOfView = 70f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = FogDay;
            head.gameObject.AddComponent<AudioListener>();

            // Flashlight beam, slightly below and right of the eyes so its shadows read.
            var beam = Node(head, "FlashlightBeam", new Vector3(0.12f, -0.12f, 0.1f)).gameObject.AddComponent<Light>();
            beam.type = LightType.Spot;
            beam.spotAngle = 55f;
            beam.innerSpotAngle = 28f;
            beam.color = new Color(1f, 0.95f, 0.85f);
            beam.shadows = LightShadows.Soft;
            beam.enabled = false;

            var agent = player.AddComponent<PlayerAgent>();
            var motor = player.AddComponent<PlayerMotor>();
            var interactor = player.AddComponent<PlayerInteractor>();
            var flashlight = player.AddComponent<Flashlight>();
            var mic = player.AddComponent<MicNoise>();
            agent.Head = head;
            agent.Motor = motor;
            agent.Interactor = interactor;
            agent.Flashlight = flashlight;
            agent.Mic = mic;
            agent.DisplayName = "You";
            motor.Agent = agent;
            motor.Head = head;
            interactor.Agent = agent;
            flashlight.Agent = agent;
            flashlight.Beam = beam;
            mic.Agent = agent;
            return agent;
        }

        static Pickup BuildPickup(Transform parent, string name, Vector3 localPos, ItemKind kind, string label, int amount = 1)
        {
            var t = Node(parent, name, localPos);
            var pickup = t.gameObject.AddComponent<Pickup>();
            pickup.Kind = kind;
            pickup.Label = label;
            pickup.Amount = amount;
            // A generous trigger so small things are easy to aim at.
            var hitbox = t.gameObject.AddComponent<BoxCollider>();
            hitbox.isTrigger = true;
            hitbox.size = new Vector3(0.3f, 0.2f, 0.3f);
            hitbox.center = new Vector3(0f, 0.08f, 0f);

            var visual = Node(t, "Visual");
            switch (kind)
            {
                case ItemKind.Matches:
                    Box(visual, "Matchbox", new Vector3(0f, 0.015f, 0f), new Vector3(0.11f, 0.03f, 0.07f), Mat("Matchbox", new Color(0.6f, 0.12f, 0.08f)), false);
                    break;
                case ItemKind.Battery:
                    Cyl(visual, "Cell", new Vector3(0f, 0.02f, 0f), 0.035f, 0.1f, Mat("Battery", new Color(0.15f, 0.25f, 0.4f), 0.5f), false, Quaternion.Euler(0f, 0f, 90f));
                    break;
                case ItemKind.WindingKey:
                    Box(visual, "Bow", new Vector3(0f, 0.01f, 0f), new Vector3(0.06f, 0.015f, 0.05f), Mat("Brass", new Color(0.75f, 0.6f, 0.25f), 0.7f), false);
                    Box(visual, "Shaft", new Vector3(0f, 0.01f, 0.06f), new Vector3(0.015f, 0.015f, 0.08f), Mat("Brass", new Color(0.75f, 0.6f, 0.25f), 0.7f), false);
                    break;
                default:
                    Ball(visual, "Thing", new Vector3(0f, 0.06f, 0f), 0.12f, Mat("Gold", new Color(0.85f, 0.65f, 0.25f), 0.7f), collider: false);
                    break;
            }
            return pickup;
        }

        static HollowAI BuildHollow(Transform spawn, VillageLayout layout)
        {
            var hollow = new GameObject("Hollow");
            hollow.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            var agent = hollow.AddComponent<NavMeshAgent>();
            agent.speed = 1.4f;          // docs/DESIGN.md "The monster": walks at 1.4 m/s
            agent.angularSpeed = 360f;
            agent.acceleration = 12f;
            agent.radius = 0.4f;
            agent.height = 2.5f;
            agent.stoppingDistance = 0.6f;
            var col = hollow.AddComponent<CapsuleCollider>();
            col.radius = 0.35f;
            col.height = 2.5f;
            col.center = new Vector3(0f, 1.25f, 0f);

            // Tall and too thin (docs/DESIGN.md "The monster", Appearance).
            var visual = Node(hollow.transform, "Visual");
            var skin = Mat("Hollow", new Color(0.03f, 0.03f, 0.035f), 0.3f);
            Prim(PrimitiveType.Capsule, visual, "Body", new Vector3(0f, 1.15f, 0f), new Vector3(0.5f, 1.1f, 0.35f), skin, collider: false);
            Ball(visual, "Head", new Vector3(0f, 2.3f, 0.02f), 0.36f, skin, collider: false);
            Box(visual, "ArmW", new Vector3(-0.32f, 1.25f, 0f), new Vector3(0.09f, 1.3f, 0.09f), skin, false, Quaternion.Euler(0f, 0f, -4f));
            Box(visual, "ArmE", new Vector3(0.32f, 1.25f, 0f), new Vector3(0.09f, 1.3f, 0.09f), skin, false, Quaternion.Euler(0f, 0f, 4f));
            var eyes = Mat("HollowEyes", new Color(0.9f, 0.9f, 0.85f), 0.5f, new Color(1.5f, 1.5f, 1.3f));
            Ball(visual, "EyeW", new Vector3(-0.08f, 2.33f, 0.17f), 0.05f, eyes, collider: false);
            Ball(visual, "EyeE", new Vector3(0.08f, 2.33f, 0.17f), 0.05f, eyes, collider: false);

            var ai = hollow.AddComponent<HollowAI>();
            ai.Layout = layout;
            ai.Eyes = Node(hollow.transform, "Eyes", new Vector3(0f, 2.33f, 0.2f));
            ai.Visual = visual.gameObject;
            // Ignore Raycast: its own body never blocks its sight and hearing checks, or the player's interact ray.
            foreach (var t in hollow.GetComponentsInChildren<Transform>(true)) t.gameObject.layer = 2;
            return ai;
        }

        /// <summary>Disabled cameras for tools/unity.ps1 shot -Camera &lt;name&gt;.</summary>
        static void BuildShotCameras()
        {
            var root = new GameObject("ShotCameras").transform;
            ShotCamera(root, "Cam_Overview", new Vector3(0f, 150f, -230f), new Vector3(0f, 0f, 40f));
            ShotCamera(root, "Cam_Square", new Vector3(0f, 9f, 86f), new Vector3(0f, 1.5f, 116f));
            ShotCamera(root, "Cam_House", new Vector3(7f, 2.2f, 22f), new Vector3(0f, 2.2f, 9.5f));
            ShotCamera(root, "Cam_Interior", new Vector3(5.2f, 1.7f, 8.9f), new Vector3(-2.5f, 1.1f, 1.5f));
            // Puzzle rooms: house 7 has the sequence panel, house 1 the code lock, house 0 the clock.
            foreach (var (name, house) in new[] { ("Cam_PuzzleRoom", 7), ("Cam_CodeLock", 1), ("Cam_Clock", 0) })
            {
                var o = HousePosition(house);
                ShotCamera(root, name, o + new Vector3(1.6f, 1.6f, -0.6f), o + new Vector3(3.2f, 1.3f, -4.2f));
            }
            ShotCamera(root, "Cam_Road", new Vector3(0f, 1.7f, 160f), new Vector3(0f, 1.5f, 260f));

            // Look pass (M1.5): house 7's rooms and yard, the forest band.
            var h7 = HousePosition(7);
            ShotCamera(root, "Cam_HearthRoom", h7 + new Vector3(-0.6f, 1.6f, 0.5f), h7 + new Vector3(-3.4f, 1.0f, -3.8f));
            ShotCamera(root, "Cam_Hall", h7 + new Vector3(2.6f, 1.6f, 1.4f), h7 + new Vector3(-2.0f, 1.3f, 4.2f));
            ShotCamera(root, "Cam_FrontYard", h7 + new Vector3(3.8f, 1.7f, 9.5f), h7 + new Vector3(0f, 1.4f, 4.5f));
            ShotCamera(root, "Cam_Garden", h7 + new Vector3(13f, 2.6f, -17f), h7 + new Vector3(0f, 0.8f, -7f));
            ShotCamera(root, "Cam_Forest", new Vector3(20f, 1.7f, -158f), new Vector3(36f, 2.5f, -176f));
            ShotCamera(root, "Cam_Fireside", h7 + new Vector3(-1.2f, 1.3f, -0.6f), h7 + new Vector3(-4.6f, 0.6f, -2.4f));
        }

        static void ShotCamera(Transform root, string name, Vector3 pos, Vector3 lookAt)
        {
            var t = Node(root, name, pos, Quaternion.LookRotation(lookAt - pos));
            var cam = t.gameObject.AddComponent<Camera>();
            cam.enabled = false;
            cam.fieldOfView = 60f;
            cam.farClipPlane = 800f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = FogDay;
        }

        static void ApplyRenderSettings()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0025f;
            RenderSettings.fogColor = FogDay;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.42f, 0.43f, 0.47f);
            RenderSettings.skybox = null;

            // Realtime lighting only: no baked or realtime GI to compute.
            var lighting = AssetDatabase.LoadAssetAtPath<LightingSettings>(LightingPath);
            if (lighting == null)
            {
                lighting = new LightingSettings();
                AssetDatabase.CreateAsset(lighting, LightingPath);
            }
            lighting.bakedGI = false;
            lighting.realtimeGI = false;
            EditorUtility.SetDirty(lighting);
            Lightmapping.lightingSettings = lighting;
        }

        // ------------------------------------------------------------ navigation

        static void ExcludeFromNavMesh(GameObject go)
        {
            var modifier = go.AddComponent<NavMeshModifier>();
            modifier.ignoreFromBuild = true;
        }

        static void AddCarvingObstacle(GameObject go, Vector3 center, Vector3 size)
        {
            var obstacle = go.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.center = center;
            obstacle.size = size;
            obstacle.carving = true;
            obstacle.carveOnlyStationary = true;
        }

        static void AddNavMeshSurface(Transform village)
        {
            var surface = village.gameObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.overrideVoxelSize = true;
            surface.voxelSize = 0.125f; // 4 voxels per agent radius, so doorways don't erode shut
        }

        [MenuItem("Hollowmere/Rebake Greybox NavMesh")]
        public static void RebakeNavMesh()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            BakeNavMesh();
            EditorSceneManager.SaveScene(scene);
        }

        /// <summary>
        /// Bakes the open Greybox scene. Door panels and gate leaves carry a NavMeshModifier set to ignore-from-build,
        /// so doorways bake open; their carving NavMeshObstacles close them again while the doors are shut.
        /// Note that the obstacles carve as soon as the data loads, in edit mode too.
        /// </summary>
        static void BakeNavMesh()
        {
            var surface = Object.FindAnyObjectByType<NavMeshSurface>();
            surface.BuildNavMesh();
            AssetDatabase.DeleteAsset(NavMeshPath);
            AssetDatabase.CreateAsset(surface.navMeshData, NavMeshPath);
            Debug.Log("[Hollowmere] NAVMESH BAKED");
        }
    }
}
