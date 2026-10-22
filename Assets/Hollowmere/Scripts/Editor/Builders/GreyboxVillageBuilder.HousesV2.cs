using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Hollowmere.EditorTools.BuilderKit;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// The 15 houses of village v2 (M1.6 P3, "HouseBuilderV2"): each built from its Hxx.json through HousePlan. Walls
    /// with their door and window openings, a floor per room with stairwell holes, a slab over each wing's top storey
    /// carried out under the eaves, roofs and chimneys, stair flights (a hidden ramp to walk on under visible treads),
    /// loft ladders (Ladder at both ends and a NavMeshLink for the Hollow), outside doors as M1 Door leaves, steps up to
    /// raised floors, a Room per JSON room (id, oriented volume, ceiling light) and the House. Hearth, fuse box, hiding
    /// spots, notes and the puzzle come with the gameplay move (P3b) and the props (P5).
    /// </summary>
    public static partial class GreyboxVillageBuilder
    {
        const float StepRise = 0.18f, StepTread = 0.3f;

        static House[] BuildHouseShellsV2(Transform village, VillagePlan plan, HousePlan[] housePlans)
        {
            var root = Node(village, "Houses");
            var houses = new House[housePlans.Length];
            for (int i = 0; i < housePlans.Length; i++)
            {
                var hp = housePlans[i];
                houses[i] = BuildHouseV2(root, plan, hp);
                PlanLabel(hp.House.Slug, new Vector3(hp.Place.Position[0], 0f, hp.Place.Position[2]), 6f);
            }
            return houses;
        }

        static House BuildHouseV2(Transform parent, VillagePlan plan, HousePlan hp)
        {
            var h = hp.House;
            var place = hp.Place;
            var node = Node(parent, $"{h.Slug} {h.Name}", new Vector3(place.Position[0], hp.FloorY, place.Position[2]), Quaternion.Euler(0f, place.Yaw, 0f));
            var house = node.gameObject.AddComponent<House>();
            house.Index = h.Index;

            var shell = Node(node, "Shell"); // art slot: walls, floors, roofs, chimneys
            var walls = Node(shell, "Walls");
            foreach (var w in hp.Walls) Box(walls, w.Name, w.Centre, w.Size, SurfaceMat(w.Mat));
            var floors = Node(shell, "Floors");
            foreach (var f in hp.Floors) Box(floors, f.Name, f.Centre, f.Size, SurfaceMat(f.Mat));
            foreach (var f in hp.TopSlabs) Box(floors, f.Name, f.Centre, f.Size, SurfaceMat(f.Mat));
            BuildRoofsV2(shell, plan, hp);

            BuildOpeningsV2(node, house, hp);
            BuildFlightsV2(node, hp);
            BuildRoomsV2(node, house, hp);

            // The house's own volume: each wing from the street (or its cellar) to the eaves, or the attic's ceiling.
            house.Volumes = h.Wings.Select(w =>
            {
                float bottom = Mathf.Min(VillagePlan.WingBottom(h, w), -hp.FloorY) - (w.Levels.Contains("C") ? 2.5f : 0f);
                float top = VillagePlan.WingEaves(h, w);
                if (w.Levels.Contains("A") && h.Levels.TryGetValue("A", out var attic)) top = Mathf.Max(top, attic.Y + attic.H);
                return new Bounds(new Vector3((w.Rect[0] + w.Rect[2]) * 0.5f, (bottom + top) * 0.5f, (w.Rect[1] + w.Rect[3]) * 0.5f),
                                  new Vector3(w.Rect[2] - w.Rect[0], top - bottom, w.Rect[3] - w.Rect[1]));
            }).ToArray();
            house.Interior = WorldBox(node, house.Volumes);
            return house;
        }

        /// <summary>The world-space box round local boxes under a turned node.</summary>
        static Bounds WorldBox(Transform node, IEnumerable<Bounds> local)
        {
            var b = new Bounds(node.position, Vector3.zero);
            bool first = true;
            foreach (var v in local)
                for (int k = 0; k < 8; k++)
                {
                    var corner = new Vector3((k & 1) == 0 ? v.min.x : v.max.x, (k & 2) == 0 ? v.min.y : v.max.y, (k & 4) == 0 ? v.min.z : v.max.z);
                    var p = node.TransformPoint(corner);
                    if (first) { b = new Bounds(p, Vector3.zero); first = false; }
                    else b.Encapsulate(p);
                }
            return b;
        }

        // ------------------------------------------------------------ roofs and chimneys

        static void BuildRoofsV2(Transform shell, VillagePlan plan, HousePlan hp)
        {
            var h = hp.House;
            var roofs = Node(shell, "Roofs");
            var ridges = new Dictionary<WingSpec, float>();
            var main = h.Wings[0].Rect;
            foreach (var w in h.Wings)
            {
                var r = w.Rect;
                float sx = r[2] - r[0], sz = r[3] - r[1];
                var c = new Vector3((r[0] + r[2]) * 0.5f, 0f, (r[1] + r[3]) * 0.5f);
                var shape = VillagePlan.ParseRoof(w.Roof, sx, sz);
                var away = new Vector2(c.x - (main[0] + main[2]) * 0.5f, c.z - (main[1] + main[3]) * 0.5f);
                if (away.sqrMagnitude < 0.01f) away = new Vector2(0f, -1f);
                ridges[w] = RoofV2(roofs, $"Roof_{w.Id}", shape, c, sx, sz, VillagePlan.WingEaves(h, w),
                                   SurfaceMat(plan.FirstMaterialKey(Stoplisted(w.Roof), "Roof")), away);
            }
            // Houses are walked in: the roof meshes are solid from inside (the attic) and from outside.
            foreach (var f in roofs.GetComponentsInChildren<MeshFilter>())
                f.gameObject.AddComponent<MeshCollider>().sharedMesh = f.sharedMesh;
            // Most pitches are under the agent's max slope: keep roofs in the bake (attic headroom) but never walkable.
            var notWalkable = roofs.gameObject.AddComponent<NavMeshModifier>();
            notWalkable.overrideArea = true;
            notWalkable.area = 1; // built-in "Not Walkable"
            BuildChimneysV2(shell, h, ridges);
        }

        // ------------------------------------------------------------ doors and windows

        static void BuildOpeningsV2(Transform node, House house, HousePlan hp)
        {
            var root = Node(node, "Openings");
            var frame = Mat("WindowFrame", new Color(0.2f, 0.15f, 0.1f));
            var glass = Mat("Window", new Color(0.07f, 0.09f, 0.12f), 0.8f);
            var def = HouseCatalog.Houses[hp.House.Index];
            Door front = null;
            // With an open porch as the front, the porch's own inner door is the front door that latches at dusk.
            var porch = hp.Openings.FirstOrDefault(o => o.IsDoor && o.Exterior && o.Door.Role == "front" && !o.HasLeaf);
            var inner = porch == null ? null : hp.Openings.FirstOrDefault(o => o.IsDoor && !o.Exterior && o.HasLeaf &&
                                                                             (o.Door.RoomA == porch.Room || o.Door.RoomB == porch.Room));
            foreach (var o in hp.Openings)
            {
                bool alongX = o.Side == "N" || o.Side == "S";
                if (!o.IsDoor)
                {
                    // Glass right through the wall, with the M1 frame; it blocks like the wall (weak windows break in a later phase).
                    var paneSize = alongX ? new Vector3(o.Width, o.Height, o.WallThick + 0.04f) : new Vector3(o.WallThick + 0.04f, o.Height, o.Width);
                    var pane = Box(root, (o.Weak ? "WeakWindow_" : "Window_") + o.Id, o.Centre, paneSize, glass);
                    WindowFrame(root, pane, alongX);
                    continue;
                }
                if (!o.HasLeaf) continue; // open porch entrances and arches

                // +z along the door's outward normal (an inner front door faces into its porch).
                var side = o == inner && o.Door.RoomA == porch.Room ? Opposite(o.Side) : o.Side; // a side is room_a's wall: from the porch it faces in
                var turn = Quaternion.Euler(0f, VillagePlan.SideYaw(side), 0f);
                var hinge = o.Centre + turn * new Vector3(-o.Width * 0.5f, -o.Height * 0.5f, 0f);
                var colour = o.Door.Colour ?? (o.Door.Role == "front" || o == inner ? def.Door.ToString() : null);
                var mat = colour != null && System.Enum.TryParse(colour, out DoorColour dc)
                    ? Mat($"Door_{dc}", HouseCatalog.DoorTint(dc), 0.3f)
                    : SurfaceMat("Planks");
                if ((o.Exterior && o.Usable) || o == inner)
                {
                    // M1 front door: pivot on the hinge, swings inward (+90 about y), carves the NavMesh while shut.
                    var pivot = Node(root, $"Door_{o.Id}", hinge, turn);
                    var visual = Node(pivot, "Visual");
                    Box(visual, "Leaf", new Vector3(o.Width * 0.5f, o.Height * 0.5f, 0f), new Vector3(o.Width - 0.04f, o.Height - 0.02f, 0.08f), mat);
                    ExcludeFromNavMesh(pivot.gameObject);
                    AddCarvingObstacle(pivot.gameObject, new Vector3(o.Width * 0.5f, o.Height * 0.5f, 0f), new Vector3(o.Width, o.Height, 0.3f));
                    var door = pivot.gameObject.AddComponent<Door>();
                    door.House = house;
                    if (o.Door.Role == "front" || o == inner) front = door;
                    if (o.Exterior && hp.FloorY > 0.05f) FrontSteps(root, o, hp.FloorY);
                }
                else if (o.Exterior)
                {
                    // Sealed doors and doors high up a wall (hoist, hayloft): a fixed leaf.
                    Box(root, $"FixedDoor_{o.Id}", o.Centre, alongX ? new Vector3(o.Width, o.Height, 0.08f) : new Vector3(0.08f, o.Height, o.Width), mat);
                }
            }
            // A porch opening is the front; the front leaf is then the first outside-usable door, or the porch's inner door.
            house.Door = front ?? root.GetComponentsInChildren<Door>().FirstOrDefault();
            var frontOpening = hp.Openings.FirstOrDefault(o => o.IsDoor && o.Exterior && o.Door.Role == "front");
            if (frontOpening != null)
            {
                DesignChecks.TryFrontDoorLocal(hp.House, out var local);
                var marker = Node(node, "FrontDoor", new Vector3(local.x, 0f, local.y), Quaternion.Euler(0f, VillagePlan.SideYaw(frontOpening.Side), 0f));
                house.Front = marker;
            }
        }

        /// <summary>Steps down from a raised threshold to the street, 0.18 m risers, 0.3 m treads.</summary>
        static void FrontSteps(Transform root, PlanOpening o, float floorY)
        {
            int n = Mathf.CeilToInt(floorY / StepRise);
            float rise = floorY / n;
            var turn = Quaternion.Euler(0f, VillagePlan.SideYaw(o.Side), 0f);
            var face = new Vector3(o.Centre.x, 0f, o.Centre.z) + turn * new Vector3(0f, 0f, o.WallThick * 0.5f); // the wall's outer face
            var stone = Mat("Stone", new Color(0.45f, 0.44f, 0.42f));
            for (int k = 1; k < n; k++)
            {
                // Step k: top k risers down from the floor, k treads out from the wall; each runs down to the street.
                float top = -k * rise, bottom = -floorY;
                var centre = face + turn * new Vector3(0f, (top + bottom) * 0.5f, (k - 0.5f) * StepTread);
                Box(root, $"Step_{o.Id}_{k}", centre, Abs(turn * new Vector3(o.Width + 0.4f, top - bottom, StepTread)), stone);
            }
        }

        static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        static string Opposite(string side) => side == "N" ? "S" : side == "S" ? "N" : side == "E" ? "W" : "E";

        // ------------------------------------------------------------ stairs and ladders

        static void BuildFlightsV2(Transform node, HousePlan hp)
        {
            var root = Node(node, "Stairs");
            foreach (var f in hp.Flights)
            {
                var r = f.Rect;
                var c = new Vector2((r[0] + r[2]) * 0.5f, (r[1] + r[3]) * 0.5f);
                float lowY = hp.Level(f.Lower).Y;
                float yaw = VillagePlan.YawOf(f.Up);
                var flight = Node(root, $"{f.Id} {f.Lower}-{f.Upper}", new Vector3(c.x, lowY, c.y), Quaternion.Euler(0f, yaw, 0f));
                var mat = SurfaceMat(string.IsNullOrEmpty(f.Mat) ? "FloorDarkPlanks" : f.Mat);
                if (f.Ladder) { BuildLadderV2(node, flight, f, hp, mat); continue; }

                // Walk on a hidden ramp (NavMesh and the character controller like it); see treads drawn over it.
                float length = Mathf.Sqrt(f.Run * f.Run + f.Rise * f.Rise);
                float slope = Mathf.Atan2(f.Rise, f.Run) * Mathf.Rad2Deg;
                var ramp = Node(flight, "Ramp", new Vector3(0f, f.Rise * 0.5f, 0f) - Quaternion.Euler(-slope, 0f, 0f) * new Vector3(0f, 0.05f, 0f), Quaternion.Euler(-slope, 0f, 0f));
                ramp.gameObject.AddComponent<BoxCollider>().size = new Vector3(f.Width, 0.1f, length);
                int steps = Mathf.Max(2, Mathf.RoundToInt(f.Rise / 0.19f));
                float riser = f.Rise / steps, tread = f.Run / steps;
                for (int k = 0; k < steps; k++)
                {
                    float z = -f.Run * 0.5f + tread * (k + 0.5f);
                    float top = riser * (k + 1);
                    Box(flight, $"Tread_{k:00}", new Vector3(0f, top - 0.04f, z), new Vector3(f.Width, 0.08f, tread + 0.02f), mat, false);
                    Box(flight, $"Riser_{k:00}", new Vector3(0f, top - riser * 0.5f, z - tread * 0.5f), new Vector3(f.Width, riser, 0.03f), mat, false);
                }
                // A stringer under the flight closes it from the side.
                Box(flight, "Stringer", new Vector3(0f, f.Rise * 0.5f - 0.15f, 0f), new Vector3(f.Width - 0.02f, 0.2f, length), mat, false, Quaternion.Euler(-slope, 0f, 0f));
            }
        }

        /// <summary>A loft ladder through its hatch: rails and rungs, a Ladder at each end, and a NavMeshLink for the Hollow.</summary>
        static void BuildLadderV2(Transform house, Transform flight, PlanFlight f, HousePlan hp, Material mat)
        {
            var wood = Mat("Wood", new Color(0.25f, 0.17f, 0.11f));
            float lean = Mathf.Atan2(f.Run * 0.8f, f.Rise) * Mathf.Rad2Deg; // from vertical, towards the head
            float length = Mathf.Sqrt(f.Rise * f.Rise + f.Run * 0.8f * f.Run * 0.8f);
            var ladder = Node(flight, "Ladder", new Vector3(0f, 0f, -f.Run * 0.4f), Quaternion.Euler(lean, 0f, 0f));
            foreach (float x in new[] { -0.22f, 0.22f })
                Box(ladder, "Rail", new Vector3(x, length * 0.5f, 0f), new Vector3(0.06f, length, 0.06f), wood, false);
            for (float y = 0.3f; y < length - 0.1f; y += 0.3f)
                Box(ladder, "Rung", new Vector3(0f, y, 0f), new Vector3(0.44f, 0.04f, 0.04f), wood, false);

            // Exits face on, away from the ladder.
            var bottom = Node(house, $"LadderFoot_{f.Id}", f.Bottom, Quaternion.Euler(0f, VillagePlan.YawOf(-f.Up), 0f));
            var top = Node(house, $"LadderHead_{f.Id}", f.Top, Quaternion.Euler(0f, VillagePlan.YawOf(f.Up), 0f));
            var climbUp = LadderEnd(flight, "ClimbUp", new Vector3(0f, 1f, -f.Run * 0.5f + 0.1f), new Vector3(0.8f, 2f, 0.6f), top, true);
            var climbDown = LadderEnd(flight, "ClimbDown", new Vector3(0f, f.Rise + 0.5f, 0f), new Vector3(Mathf.Max(0.7f, f.Width), 1f, Mathf.Max(0.7f, f.Run)), bottom, false);

            var link = Node(house, $"NavLink_{f.Id}").gameObject.AddComponent<NavMeshLink>();
            link.startPoint = f.Bottom;
            link.endPoint = f.Top;
            link.width = 0.6f;
            link.bidirectional = true;
        }

        static Ladder LadderEnd(Transform parent, string name, Vector3 centre, Vector3 size, Transform exit, bool up)
        {
            var t = Node(parent, name, centre);
            var box = t.gameObject.AddComponent<BoxCollider>();
            box.isTrigger = true; // aim at it; walk through it
            box.size = size;
            ExcludeFromNavMesh(t.gameObject);
            var ladder = t.gameObject.AddComponent<Ladder>();
            ladder.Exit = exit;
            ladder.Up = up;
            return ladder;
        }

        // ------------------------------------------------------------ rooms

        static void BuildRoomsV2(Transform node, House house, HousePlan hp)
        {
            var root = Node(node, "Rooms");
            var hearthRoom = HearthRoomId(hp);
            var rooms = new List<Room>();
            foreach (var pr in hp.Rooms)
            {
                var spec = pr.Spec;
                var inner = spec.Inner.Length == 4 ? spec.Inner : spec.Rect;
                float h = pr.CeilingY - pr.FloorY;
                var t = Node(root, spec.Id, new Vector3((inner[0] + inner[2]) * 0.5f, pr.FloorY, (inner[1] + inner[3]) * 0.5f));
                var room = t.gameObject.AddComponent<Room>();
                room.House = house;
                room.Id = spec.Id;
                room.Oriented = true;
                room.LocalBounds = new Bounds(new Vector3(0f, h * 0.5f, 0f), new Vector3(inner[2] - inner[0], h, inner[3] - inner[1]));
                room.Bounds = WorldBox(t, new[] { room.LocalBounds });
                room.IsHearthRoom = spec.Id == hearthRoom;
                float reach = Mathf.Max(inner[2] - inner[0], inner[3] - inner[1]);
                room.Light = PointLight(t, "CeilingLight", new Vector3(0f, h - 0.25f, 0f), new Color(1f, 0.82f, 0.6f), Mathf.Clamp(reach * 1.2f, 4f, 9f), 2.5f);
                SoftShadows(room.Light, UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierLow); // walls keep the light in
                room.Bulb = Ball(t, "Bulb", new Vector3(0f, h - 0.12f, 0f), 0.14f, Mat("Bulb", new Color(1f, 0.95f, 0.8f), 0.5f, new Color(2f, 1.8f, 1.4f)), collider: false);
                Node(t, "Probe", t.InverseTransformPoint(t.parent.parent.TransformPoint(pr.Probe))); // a clear spot on the floor (tests, the Hollow's search)
                rooms.Add(room);
            }
            house.Rooms = rooms.ToArray();
            house.HearthRoom = rooms.FirstOrDefault(r => r.IsHearthRoom);
        }

        /// <summary>The room holding the house's hearth: the one prop with role "hearth" (decoy fireplaces are "hearth-decor").</summary>
        static string HearthRoomId(HousePlan hp) =>
            hp.House.Props.FirstOrDefault(p => (p.Role ?? "").Split(' ').Contains("hearth"))?.Room;
    }
}
