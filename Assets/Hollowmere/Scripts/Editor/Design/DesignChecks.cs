using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// C# twin of tools/housegen: core.py validate() and the village checks in village.py build(), run on the loaded
    /// JSON with the same numbers. Each method returns a list of readable problems (empty = fine), prefixed with the
    /// house slug. The geometry helpers are public so the M1.6 builders use the same rules as the checks.
    /// </summary>
    public static class DesignChecks
    {
        const float Edge = 1e-4f;      // two edges coincide (core.py 1e-6, widened for float)
        const float Span = 1e-3f;      // minimum shared edge length that counts as touching (core.py neighbours)
        const float OverlapTol = 0.005f; // core.py _overlap tol
        public const float BuildingGap = 1.5f; // village.py: buildings at least 1.5 m apart
        public const string Outside = "ext";

        public static readonly string[] Sides = { "N", "S", "E", "W" };

        // docs/DESIGN.md "Houses and puzzles", shared requirements: catalog tag or prop role, "a|b" = either.
        static readonly string[] RequiredTags = { "hearth", "fuse-box", "phone", "mirror", "family-photo", "tv|radio" };
        static readonly string[] RequiredRoomKinds = { "kitchen", "living", "bedroom", "bathroom" };
        static readonly string[] RequiredAnchors = { "item-spot", "note-spot", "puzzle", "container", "shrine" };
        // Landmark kinds that are open ground, not buildings (village.py build()).
        static readonly string[] OpenLandmarkKinds = { "area", "fields", "water", "ritual" };

        // ================================================================== houses

        public static List<string> House(HouseSpec h, AssetCatalogSpec catalog)
        {
            var e = new List<string>();
            e.AddRange(Identity(h));
            e.AddRange(Structure(h));
            e.AddRange(Openings(h));
            e.AddRange(AssetKeys(h, catalog));
            e.AddRange(Requirements(h, catalog));
            return e;
        }

        /// <summary>Slug, name, family, part, puzzle kind, door colour, chimneys, clock hour and loudness match HouseCatalog.</summary>
        public static List<string> Identity(HouseSpec h)
        {
            var e = new List<string>();
            if (h.Index < 0 || h.Index >= HouseCatalog.Count)
            {
                e.Add($"{h.Slug}: index {h.Index} outside HouseCatalog");
                return e;
            }
            var d = HouseCatalog.Houses[h.Index];
            if (h.Slug != DesignData.Slug(h.Index)) e.Add($"{h.Slug}: slug doesn't match index {h.Index}");
            if (h.Name != d.Name) e.Add($"{h.Slug}: name '{h.Name}' != HouseCatalog '{d.Name}'");
            if (h.Family != d.Family) e.Add($"{h.Slug}: family '{h.Family}' != HouseCatalog '{d.Family}'");
            if (h.Part != d.Part) e.Add($"{h.Slug}: part '{h.Part}' != HouseCatalog '{d.Part}'");
            if (!Enum.TryParse(h.PuzzleKindM1, out PuzzleKind kind) || kind != d.Puzzle)
                e.Add($"{h.Slug}: puzzle kind '{h.PuzzleKindM1}' != HouseCatalog {d.Puzzle}");
            if (!Enum.TryParse(h.DoorColour, out DoorColour colour) || colour != d.Door)
                e.Add($"{h.Slug}: door colour '{h.DoorColour}' != HouseCatalog {d.Door}");
            if (h.Chimneys != d.Chimneys) e.Add($"{h.Slug}: {h.Chimneys} chimneys != HouseCatalog {d.Chimneys}");
            if (h.ClockHour != d.ClockHour) e.Add($"{h.Slug}: clock hour {h.ClockHour} != HouseCatalog {d.ClockHour}");
            if (h.LoudPuzzle != d.LoudPuzzle) e.Add($"{h.Slug}: loud puzzle {h.LoudPuzzle} != HouseCatalog {d.LoudPuzzle}");
            return e;
        }

        /// <summary>Levels exist, every room lies inside a wing that has its level, rooms on a level don't overlap.</summary>
        public static List<string> Structure(HouseSpec h)
        {
            var e = new List<string>();
            foreach (var w in h.Wings)
            {
                if (w.Rect.Length != 4) e.Add($"{h.Slug}: wing {w.Id} rect needs 4 numbers");
                foreach (var lv in w.Levels)
                    if (!h.Levels.ContainsKey(lv)) e.Add($"{h.Slug}: wing {w.Id} uses unknown level {lv}");
            }
            if (e.Count > 0) return e;
            var ids = new HashSet<string>();
            foreach (var r in h.Rooms)
            {
                if (!ids.Add(r.Id)) e.Add($"{h.Slug}: duplicate room id {r.Id}");
                if (r.Rect.Length != 4) { e.Add($"{h.Slug}: room {r.Id} rect needs 4 numbers"); continue; }
                if (!h.Levels.ContainsKey(r.Level)) e.Add($"{h.Slug}: room {r.Id} on unknown level {r.Level}");
                if (WingOf(h, r) == null) e.Add($"{h.Slug}: room {r.Id} not inside any wing on level {r.Level}");
            }
            if (e.Count > 0) return e;
            for (int i = 0; i < h.Rooms.Length; i++)
                for (int j = i + 1; j < h.Rooms.Length; j++)
                {
                    var a = h.Rooms[i];
                    var b = h.Rooms[j];
                    if (a.Level == b.Level && RectsOverlap(a.Rect, b.Rect))
                        e.Add($"{h.Slug}: rooms overlap {a.Id} / {b.Id}");
                }
            return e;
        }

        /// <summary>
        /// Doors join a room and outside (on an exterior edge) or two rooms on the same level across the given wall, within
        /// the shared span; windows sit on exterior edges within the wall; stairs lie inside their room and join two levels.
        /// </summary>
        public static List<string> Openings(HouseSpec h)
        {
            var e = new List<string>();
            foreach (var d in h.Doors)
            {
                var ra = h.FindRoom(d.RoomA);
                if (ra == null) { e.Add($"{h.Slug}: door {d.Id} room_a {d.RoomA} doesn't exist"); continue; }
                if (!IsSide(d.Side)) { e.Add($"{h.Slug}: door {d.Id} side '{d.Side}'"); continue; }
                if (d.Level != ra.Level) e.Add($"{h.Slug}: door {d.Id} on level {d.Level} but {ra.Id} is on {ra.Level}");
                WallSpan(ra.Rect, d.Side, out float lo, out float hi);
                if (!Fits(d.At, d.Width, lo, hi)) e.Add($"{h.Slug}: door {d.Id} outside the wall span of {ra.Id}");
                if (d.RoomB == Outside)
                {
                    if (!IsExteriorEdge(h, ra, d.Side)) e.Add($"{h.Slug}: door {d.Id} marked exterior on an interior edge of {ra.Id}");
                    continue;
                }
                var rb = h.FindRoom(d.RoomB);
                if (rb == null) { e.Add($"{h.Slug}: door {d.Id} room_b {d.RoomB} doesn't exist"); continue; }
                if (rb.Level != ra.Level) { e.Add($"{h.Slug}: door {d.Id} joins {ra.Id} ({ra.Level}) and {rb.Id} ({rb.Level}) on different levels"); continue; }
                if (!Neighbours(h, ra, d.Side).Contains(rb)) { e.Add($"{h.Slug}: door {d.Id}: {rb.Id} is not across {ra.Id}'s {d.Side} wall"); continue; }
                WallSpan(rb.Rect, d.Side, out float lo2, out float hi2);
                if (!Fits(d.At, d.Width, Mathf.Max(lo, lo2), Mathf.Min(hi, hi2))) e.Add($"{h.Slug}: door {d.Id} not within the span {ra.Id} and {rb.Id} share");
                else if (!Fits(d.At, d.Width + PartitionT, Mathf.Max(lo, lo2), Mathf.Min(hi, hi2)))
                    e.Add($"{h.Slug}: door {d.Id} runs into the partition at the end of its wall (clear opening narrower than {d.Width})");
            }
            // core.py: no door opens onto a stair flight or stairwell, only onto its foot (lower floor) or head (upper floor).
            foreach (var st in h.Stairs)
            {
                var room = h.FindRoom(st.Room);
                if (room == null || !h.Levels.ContainsKey(st.Frm) || !h.Levels.ContainsKey(st.To) || !IsSide(st.Ascent)) continue;
                string lower = h.Levels[st.Frm].Y <= h.Levels[st.To].Y ? st.Frm : st.To;
                var up = VillagePlan.SideNormal(st.Ascent);
                if (st.Frm != lower) up = -up;
                var allowed = StairEnd(st.Rect, room.Level == lower ? -up : up);
                foreach (var d in h.Doors)
                    foreach (var sideRoom in new[] { d.RoomA, d.RoomB })
                    {
                        var r = sideRoom == Outside ? null : h.FindRoom(sideRoom);
                        if (r == null || r.Level != room.Level) continue;
                        var zone = DoorZone(h, d, r);
                        // 0.051: core.py tests > 0.05 in doubles; floats land on either side of an exact 0.05 overlap.
                        if (RectsOverlap(zone, st.Rect, 0.051f) && !RectInside(Clip(zone, st.Rect), allowed, 0.01f))
                            e.Add($"{h.Slug}: door {d.Id} opens onto stair {st.Id} in {r.Id}");
                    }
            }
            foreach (var w in h.Windows)
            {
                var r = h.FindRoom(w.Room);
                if (r == null) { e.Add($"{h.Slug}: window {w.Id} room {w.Room} doesn't exist"); continue; }
                if (!IsSide(w.Side)) { e.Add($"{h.Slug}: window {w.Id} side '{w.Side}'"); continue; }
                if (!IsExteriorEdge(h, r, w.Side)) e.Add($"{h.Slug}: window {w.Id} on an interior edge of {r.Id}");
                WallSpan(r.Rect, w.Side, out float lo, out float hi);
                if (!Fits(w.At, w.Width, lo, hi)) e.Add($"{h.Slug}: window {w.Id} outside the wall span of {r.Id}");
            }
            foreach (var s in h.Stairs)
            {
                var r = h.FindRoom(s.Room);
                if (r == null) { e.Add($"{h.Slug}: stair {s.Id} room {s.Room} doesn't exist"); continue; }
                if (!h.Levels.ContainsKey(s.Frm) || !h.Levels.ContainsKey(s.To) || s.Frm == s.To)
                    e.Add($"{h.Slug}: stair {s.Id} must join two existing levels ({s.Frm} -> {s.To})");
                if (s.Rect.Length != 4 || !RectInside(s.Rect, r.Rect)) e.Add($"{h.Slug}: stair {s.Id} outside room {r.Id}");
            }
            return e;
        }

        /// <summary>Every prop key, floor, door, stair and chimney material is in the asset catalog; prop and anchor rooms exist.</summary>
        public static List<string> AssetKeys(HouseSpec h, AssetCatalogSpec catalog)
        {
            var e = new List<string>();
            var propIds = new HashSet<string>(h.Props.Select(p => p.Id));
            foreach (var p in h.Props)
            {
                if (!catalog.HasKey(p.Key)) e.Add($"{h.Slug}: prop {p.Id} key {p.Key} not in the asset catalog");
                if (h.FindRoom(p.Room) == null) e.Add($"{h.Slug}: prop {p.Id} in unknown room {p.Room}");
                if (!string.IsNullOrEmpty(p.On) && !propIds.Contains(p.On)) e.Add($"{h.Slug}: prop {p.Id} stands on unknown prop {p.On}");
            }
            foreach (var r in h.Rooms)
                if (!catalog.HasKey(r.Floor)) e.Add($"{h.Slug}: room {r.Id} floor {r.Floor} not in the asset catalog");
            foreach (var d in h.Doors)
                if (!catalog.HasKey(d.Mat)) e.Add($"{h.Slug}: door {d.Id} material {d.Mat} not in the asset catalog");
            foreach (var s in h.Stairs)
                if (!catalog.HasKey(s.Mat)) e.Add($"{h.Slug}: stair {s.Id} material {s.Mat} not in the asset catalog");
            foreach (var c in h.ChimneysDetail)
                if (!catalog.HasKey(c.Mat)) e.Add($"{h.Slug}: chimney material {c.Mat} not in the asset catalog");
            foreach (var a in h.Anchors)
                if (h.FindRoom(a.Room) == null) e.Add($"{h.Slug}: {a.Kind} anchor in unknown room {a.Room}");
            return e;
        }

        /// <summary>DESIGN.md shared house template: doors, weak windows, hiding spots, required props, rooms, anchors, chimneys.</summary>
        public static List<string> Requirements(HouseSpec h, AssetCatalogSpec catalog)
        {
            var e = new List<string>();
            var tags = new HashSet<string>();
            foreach (var p in h.Props)
            {
                var m = catalog.FindModel(p.Key);
                if (m != null) tags.UnionWith(m.Tags);
                if (!string.IsNullOrEmpty(p.Role)) tags.UnionWith(p.Role.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }
            foreach (var need in RequiredTags)
                if (!need.Split('|').Any(tags.Contains)) e.Add($"{h.Slug}: missing required {need.Replace("|", " or ")}");

            int hiding = AnchorCount(h, "hiding");
            if (hiding < 4 || hiding > 6) e.Add($"{h.Slug}: {hiding} hiding spots (need 4-6)");

            var outer = h.Doors.Where(d => d.RoomB == Outside).ToArray();
            if (!outer.Any(d => d.Role == "front")) e.Add($"{h.Slug}: no front door");
            if (!outer.Any(d => d.Role == "back")) e.Add($"{h.Slug}: no back door");

            int weak = h.Windows.Count(w => w.Weak);
            if (weak != 2) e.Add($"{h.Slug}: {weak} weak windows (need exactly 2)");

            var kinds = h.Rooms.Select(r => r.Kind).ToList();
            foreach (var k in RequiredRoomKinds)
                if (!kinds.Contains(k)) e.Add($"{h.Slug}: no {k}");
            if (kinds.Count(k => k == "bedroom") < 2) e.Add($"{h.Slug}: needs two bedrooms");
            if (!kinds.Contains("cellar") && !kinds.Contains("attic")) e.Add($"{h.Slug}: needs a cellar or an attic");

            foreach (var k in RequiredAnchors)
                if (AnchorCount(h, k) == 0) e.Add($"{h.Slug}: no {k} anchor");
            if (AnchorCount(h, "item-spot") < 3) e.Add($"{h.Slug}: needs at least 3 item spots");
            if (AnchorCount(h, "note-spot") != 3) e.Add($"{h.Slug}: needs exactly 3 note spots");

            if (h.ChimneysDetail.Length != h.Chimneys) e.Add($"{h.Slug}: {h.ChimneysDetail.Length} chimney stacks != {h.Chimneys}");
            return e;
        }

        public static int AnchorCount(HouseSpec h, string kind) => h.Anchors.Count(a => a.Kind == kind);

        // ================================================================== village

        /// <summary>
        /// village.py build(): buildings (house wings, filler, landmark buildings) at least BuildingGap apart, no house plot
        /// over another building; every house placed once; to_altar matches the front door; blocker props in the catalog.
        /// </summary>
        public static List<string> Village(VillageSpec v, IList<HouseSpec> houses, AssetCatalogSpec catalog)
        {
            var e = new List<string>();
            var fps = new List<Footprint>();
            foreach (var h in houses)
            {
                var place = v.FindHouse(h.Slug);
                if (place == null) { e.Add($"village: {h.Slug} is not placed"); continue; }
                if (place.Name != h.Name) e.Add($"village: {h.Slug} named '{place.Name}', house JSON says '{h.Name}'");
                if (place.Position.Length != 3) { e.Add($"village: {h.Slug} position needs x, y, z"); continue; }
                if (h.World == null || !h.World.Position.SequenceEqual(place.Position) || h.World.Yaw != place.Yaw)
                    e.Add($"village: {h.Slug} world position/yaw in the house JSON differs from village.json");
                if (TryFrontDoorLocal(h, out Vector2 door))
                {
                    var world = ToWorld(place, door.x, door.y);
                    var listed = new Vector2(place.FrontDoor[0], place.FrontDoor[1]);
                    if (Vector2.Distance(world, listed) > 0.05f)
                        e.Add($"village: {h.Slug} front door lands at {world}, village.json says {listed} (placement formula differs)");
                }
                else e.Add($"village: {h.Slug} has no front door to an outside wall");
                fps.Add(new Footprint
                {
                    Id = h.Slug,
                    IsHouse = true,
                    Parts = h.Wings.Select(w => WingPoly(place, w.Rect)).ToArray(),
                    Plot = place.PlotPoly.Select(p => new Vector2(p[0], p[1])).ToArray(),
                });
            }
            if (v.Houses.Length != houses.Count) e.Add($"village: {v.Houses.Length} placed houses, {houses.Count} house files");
            foreach (var f in v.Filler)
                fps.Add(Box(f.Id, RectPoly(f.Pos[0], f.Pos[1], f.Size[0], f.Size[1], f.Yaw)));
            foreach (var l in v.Landmarks)
                if (!OpenLandmarkKinds.Contains(l.Kind))
                    fps.Add(Box(l.Id, RectPoly(l.Pos[0], l.Pos[1], l.Size[0], l.Size[1], l.Yaw)));

            for (int i = 0; i < fps.Count; i++)
                for (int j = i + 1; j < fps.Count; j++)
                {
                    var a = fps[i];
                    var b = fps[j];
                    if (a.Parts.Any(p => b.Parts.Any(q => PolysOverlap(p, q, -BuildingGap))))
                        e.Add($"village: buildings too close: {a.Id} / {b.Id}");
                    else if (a.IsHouse && b.Parts.Any(q => PolysOverlap(a.Plot, q)))
                        e.Add($"village: plot of {a.Id} overlaps building {b.Id}");
                    else if (b.IsHouse && a.Parts.Any(q => PolysOverlap(b.Plot, q)))
                        e.Add($"village: plot of {b.Id} overlaps building {a.Id}");
                }

            var altar = v.FindLandmark("L_ALTAR");
            if (altar == null) e.Add("village: no L_ALTAR landmark");
            else
                foreach (var hv in v.Houses)
                {
                    float d = Vector2.Distance(new Vector2(hv.FrontDoor[0], hv.FrontDoor[1]), new Vector2(altar.Pos[0], altar.Pos[1]));
                    if (Mathf.Abs(d - hv.ToAltar) > 0.15f) e.Add($"village: {hv.Slug} front door is {d:0.0} m from the altar, to_altar says {hv.ToAltar}");
                }
            foreach (var b in v.NightBlockers)
                if (!catalog.HasKey(b.Prop)) e.Add($"village: blocker {b.Id} prop {b.Prop} not in the asset catalog");
            return e;
        }

        sealed class Footprint
        {
            public string Id;
            public bool IsHouse;
            public Vector2[][] Parts;
            public Vector2[] Plot;
        }

        static Footprint Box(string id, Vector2[] poly) => new Footprint { Id = id, Parts = new[] { poly }, Plot = poly };

        // ================================================================== geometry (house-local rects: x0, z0, x1, z1)

        public static bool IsSide(string side) => Array.IndexOf(Sides, side) >= 0;

        public static bool RectsOverlap(float[] a, float[] b, float tol = OverlapTol) =>
            Mathf.Min(a[2], b[2]) - Mathf.Max(a[0], b[0]) > tol && Mathf.Min(a[3], b[3]) - Mathf.Max(a[1], b[1]) > tol;

        public static bool RectInside(float[] inner, float[] outer, float tol = Edge) =>
            inner[0] >= outer[0] - tol && inner[1] >= outer[1] - tol && inner[2] <= outer[2] + tol && inner[3] <= outer[3] + tol;

        /// <summary>Length of b's edge lying along a's given side (0 when b doesn't touch that side).</summary>
        public static float SharedEdge(float[] a, float[] b, string side)
        {
            switch (side)
            {
                case "N": return Mathf.Abs(b[1] - a[3]) < Edge ? Mathf.Min(a[2], b[2]) - Mathf.Max(a[0], b[0]) : 0f;
                case "S": return Mathf.Abs(b[3] - a[1]) < Edge ? Mathf.Min(a[2], b[2]) - Mathf.Max(a[0], b[0]) : 0f;
                case "E": return Mathf.Abs(b[0] - a[2]) < Edge ? Mathf.Min(a[3], b[3]) - Mathf.Max(a[1], b[1]) : 0f;
                case "W": return Mathf.Abs(b[2] - a[0]) < Edge ? Mathf.Min(a[3], b[3]) - Mathf.Max(a[1], b[1]) : 0f;
                default: return 0f;
            }
        }

        /// <summary>Rooms on the same level sharing part of the given edge of r (core.py House.neighbours).</summary>
        public static List<RoomSpec> Neighbours(HouseSpec h, RoomSpec r, string side) =>
            h.Rooms.Where(o => o != r && o.Level == r.Level && SharedEdge(r.Rect, o.Rect, side) > Span).ToList();

        /// <summary>True when part of the edge has no room behind it, i.e. it is (partly) an outside wall.</summary>
        public static bool IsExteriorEdge(HouseSpec h, RoomSpec r, string side)
        {
            WallSpan(r.Rect, side, out float lo, out float hi);
            float covered = Neighbours(h, r, side).Sum(o => SharedEdge(r.Rect, o.Rect, side));
            return covered < hi - lo - Span;
        }

        /// <summary>The wing containing the room on its level, or null.</summary>
        public static WingSpec WingOf(HouseSpec h, RoomSpec r) =>
            h.Wings.FirstOrDefault(w => w.Levels.Contains(r.Level) && w.Rect.Length == 4 && RectInside(r.Rect, w.Rect));

        /// <summary>Range along a wall: x for N/S walls, z for E/W walls.</summary>
        public static void WallSpan(float[] rect, string side, out float lo, out float hi)
        {
            bool ns = side == "N" || side == "S";
            lo = ns ? rect[0] : rect[1];
            hi = ns ? rect[2] : rect[3];
        }

        static bool Fits(float at, float width, float lo, float hi) => lo + width / 2 - Edge <= at && at <= hi - width / 2 + Edge;

        const float PartitionT = 0.12f; // core.py INT_T
        const float DoorClear = 0.85f;  // core.py DOOR_CLEAR: free depth in front of every door
        const float StairEndDepth = 0.6f; // core.py STAIR_END

        /// <summary>The clearance box in front of a door, on the side of the given room (core.py door_zone).</summary>
        public static float[] DoorZone(HouseSpec h, DoorSpec d, RoomSpec room)
        {
            string side = d.RoomA == room.Id ? d.Side : d.Side == "N" ? "S" : d.Side == "S" ? "N" : d.Side == "E" ? "W" : "E";
            float w = d.Width + 0.2f;
            var r = room.Rect;
            switch (side)
            {
                case "N": return new[] { d.At - w / 2, r[3] - DoorClear, d.At + w / 2, r[3] };
                case "S": return new[] { d.At - w / 2, r[1], d.At + w / 2, r[1] + DoorClear };
                case "E": return new[] { r[2] - DoorClear, d.At - w / 2, r[2], d.At + w / 2 };
                default: return new[] { r[0], d.At - w / 2, r[0] + DoorClear, d.At + w / 2 };
            }
        }

        /// <summary>The strip of a stair rect at the end the unit axis direction d points to (core.py stair_end).</summary>
        static float[] StairEnd(float[] r, Vector2 d)
        {
            if (d.y > 0.5f) return new[] { r[0], r[3] - StairEndDepth, r[2], r[3] };
            if (d.y < -0.5f) return new[] { r[0], r[1], r[2], r[1] + StairEndDepth };
            if (d.x > 0.5f) return new[] { r[2] - StairEndDepth, r[1], r[2], r[3] };
            return new[] { r[0], r[1], r[0] + StairEndDepth, r[3] };
        }

        static float[] Clip(float[] a, float[] b) =>
            new[] { Mathf.Max(a[0], b[0]), Mathf.Max(a[1], b[1]), Mathf.Min(a[2], b[2]), Mathf.Min(a[3], b[3]) };

        /// <summary>House-local (x, z) of the front door's centre on the outer wall face (village.py build()).</summary>
        public static bool TryFrontDoorLocal(HouseSpec h, out Vector2 local)
        {
            local = Vector2.zero;
            var d = h.Doors.FirstOrDefault(x => x.Role == "front" && x.RoomB == Outside);
            var room = d == null ? null : h.FindRoom(d.RoomA);
            if (room == null || room.Rect.Length != 4) return false;
            var r = room.Rect;
            switch (d.Side)
            {
                case "N": local = new Vector2(d.At, r[3]); return true;
                case "S": local = new Vector2(d.At, r[1]); return true;
                case "E": local = new Vector2(r[2], d.At); return true;
                case "W": local = new Vector2(r[0], d.At); return true;
                default: return false;
            }
        }

        // ================================================================== geometry (village world space, Vector2 = x, z)

        /// <summary>Unity Y rotation of a local (x, z) by yaw degrees: x' = x cos + z sin, z' = -x sin + z cos.</summary>
        public static Vector2 Rotate(float x, float z, float yaw)
        {
            float a = yaw * Mathf.Deg2Rad;
            float c = Mathf.Cos(a), s = Mathf.Sin(a);
            return new Vector2(x * c + z * s, -x * s + z * c);
        }

        /// <summary>House-local (x, z) to world (x, z) for a placed house.</summary>
        public static Vector2 ToWorld(VillageHouseSpec place, float x, float z) =>
            new Vector2(place.Position[0], place.Position[2]) + Rotate(x, z, place.Yaw);

        public static Vector2[] WingPoly(VillageHouseSpec place, float[] r) => new[]
        {
            ToWorld(place, r[0], r[1]), ToWorld(place, r[2], r[1]), ToWorld(place, r[2], r[3]), ToWorld(place, r[0], r[3]),
        };

        public static Vector2[] RectPoly(float cx, float cz, float w, float d, float yaw)
        {
            var c = new Vector2(cx, cz);
            return new[]
            {
                c + Rotate(-w / 2, -d / 2, yaw), c + Rotate(w / 2, -d / 2, yaw), c + Rotate(w / 2, d / 2, yaw), c + Rotate(-w / 2, d / 2, yaw),
            };
        }

        /// <summary>Separating-axis test for convex polygons; a negative margin also counts polygons closer than -margin.</summary>
        public static bool PolysOverlap(Vector2[] a, Vector2[] b, float margin = 0f)
        {
            foreach (var poly in new[] { a, b })
                for (int i = 0; i < poly.Length; i++)
                {
                    var p1 = poly[i];
                    var p2 = poly[(i + 1) % poly.Length];
                    var axis = new Vector2(-(p2.y - p1.y), p2.x - p1.x).normalized;
                    Project(a, axis, out float minA, out float maxA);
                    Project(b, axis, out float minB, out float maxB);
                    if (maxA + margin <= minB || maxB + margin <= minA) return false;
                }
            return true;
        }

        static void Project(Vector2[] poly, Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            foreach (var p in poly)
            {
                float d = Vector2.Dot(p, axis);
                if (d < min) min = d;
                if (d > max) max = d;
            }
        }
    }
}
