using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hollowmere.EditorTools
{
    /// <summary>An axis-aligned box in house-local space (metres; y = 0 is the ground floor).</summary>
    public struct PlanBox
    {
        public string Name;
        public Vector3 Centre;
        public Vector3 Size;
        public string Mat;
        public string Level;

        public PlanBox(string name, float x0, float y0, float z0, float x1, float y1, float z1, string mat, string level)
        {
            Name = name;
            Centre = new Vector3((x0 + x1) * 0.5f, (y0 + y1) * 0.5f, (z0 + z1) * 0.5f);
            Size = new Vector3(x1 - x0, y1 - y0, z1 - z0);
            Mat = mat;
            Level = level;
        }

        public Vector3 Min => Centre - Size * 0.5f;
        public Vector3 Max => Centre + Size * 0.5f;
    }

    /// <summary>A door or window cut into a wall run.</summary>
    public sealed class PlanOpening
    {
        public string Id;
        public bool IsDoor;
        public string Level;
        public string Room;        // the room it belongs to (doors: room_a)
        public string Side;        // wall of that room
        public bool Exterior;
        public Vector3 Centre;     // house-local: middle of the opening, on the wall's centre plane
        public Vector3 Outward;    // unit normal: out of the room (exterior) or from room_a to room_b
        public float Width, Height, Bottom; // bottom = local y of the sill or threshold
        public float WallThick;
        public bool Weak;
        public DoorSpec Door;
        public WindowSpec Window;

        /// <summary>Doors with a leaf (an open porch entrance or arch has none).</summary>
        public bool HasLeaf => IsDoor && !NoLeaf(Door.Kind);

        /// <summary>Ground-floor outside doors players open; upper-floor ones (hoist, pitching) and sealed ones stay shut.</summary>
        public bool Usable => IsDoor && Exterior && HasLeaf && Door.Role != "sealed" && Mathf.Abs(Bottom) < 0.01f;

        public static bool NoLeaf(string kind)
        {
            var k = (kind ?? "").ToLowerInvariant();
            return k.Contains("no leaf") || k.Contains("porch opening") || k.Contains("open pointed arch") || k.StartsWith("arch");
        }
    }

    /// <summary>A stair flight or ladder between two levels.</summary>
    public sealed class PlanFlight
    {
        public string Id;
        public string Room;        // the room on the lower level
        public string UpperRoom;
        public string Lower, Upper;
        public bool Ladder;
        public float[] Rect;       // x0, z0, x1, z1
        public Vector2 Up;         // unit direction of travel going up
        public Vector3 Bottom;     // house-local: on the lower floor, just off the foot
        public Vector3 Top;        // house-local: on the upper floor, just off the head
        public float Rise, Run, Width;
        public string Mat;
        public string Note;
        public float Slope => Mathf.Atan2(Rise, Run) * Mathf.Rad2Deg;
    }

    public sealed class PlanRoom
    {
        public RoomSpec Spec;
        public WingSpec Wing;
        public float FloorY, CeilingY;
        public Vector3 Probe;      // a clear spot on the floor (tests and the Hollow's search points)
    }

    /// <summary>
    /// One house's shell (M1.6 P3), computed from its Hxx.json without touching Unity objects: wall runs with the door
    /// and window openings cut out, floor slabs per room with stairwell holes, a top slab per wing (ceiling or attic
    /// floor, carried out under the eaves), stair flights and ladders, room probe points, and the holes the village
    /// ground needs over cellar stairs. Conventions as tools/housegen/core.py: rects reach the outer face of exterior
    /// walls (thickness taken inside) and the centre line of partitions (0.12 m, straddling the shared edge).
    /// </summary>
    public sealed class HousePlan
    {
        public const float PartitionT = 0.12f;   // core.py INT_T
        public const float SlabT = 0.25f;        // between storeys: level F starts 0.25 above G's ceiling
        public const float Overhang = 0.3f;      // roof overhang, matches GreyboxVillageBuilder.Gable
        public const float FlightClear = 0.6f;   // how far the bottom and top points stand off the flight
        public const float MinFloor = 0.03f;     // lowest ground-floor height above the street

        public HouseSpec House;
        public VillageHouseSpec Place;
        public float FloorY;                     // world height of the ground floor (never below the street)
        public readonly List<PlanBox> Walls = new List<PlanBox>();
        public readonly List<PlanBox> Floors = new List<PlanBox>();
        public readonly List<PlanBox> TopSlabs = new List<PlanBox>();
        public readonly List<PlanOpening> Openings = new List<PlanOpening>();
        public readonly List<PlanFlight> Flights = new List<PlanFlight>();
        public readonly List<PlanRoom> Rooms = new List<PlanRoom>();
        public readonly List<float[]> GroundHoles = new List<float[]>(); // house-local rects over cellar stairs

        static readonly string[] Sides = { "N", "S", "E", "W" };

        public static HousePlan Make(HouseSpec h, VillageHouseSpec place, AssetCatalogSpec catalog)
        {
            // Floors below the street (H02, H04, H09) come up to it (the village ground is flat under house pads), and a
            // floor at street level sits 3 cm up so it never shares a plane with the village ground.
            var p = new HousePlan { House = h, Place = place, FloorY = Mathf.Max(MinFloor, place.Position[1]) };
            p.MakeFlights();
            p.MakeRooms();
            p.MakeWalls(catalog);
            p.MakeFloors(catalog);
            return p;
        }

        public LevelSpec Level(string id) => House.Levels[id];

        public PlanRoom Room(string id) => Rooms.FirstOrDefault(r => r.Spec.Id == id);

        /// <summary>The level above lv inside a wing (attic included), or null for its top storey.</summary>
        public string NextLevel(WingSpec w, string lv)
        {
            float y = Level(lv).Y;
            return w.Levels.Where(l => House.Levels.ContainsKey(l) && Level(l).Y > y + 0.01f).OrderBy(l => Level(l).Y).FirstOrDefault();
        }

        /// <summary>The top storey of a wing that has walls (the attic sits in the roof).</summary>
        public string TopStorey(WingSpec w) =>
            w.Levels.Where(l => l != "A" && House.Levels.ContainsKey(l)).OrderBy(l => Level(l).Y).Last();

        // ================================================================== flights

        void MakeFlights()
        {
            var used = new HashSet<StairSpec>();
            foreach (var s in House.Stairs)
            {
                if (used.Contains(s)) continue;
                // A flight is listed in both its rooms: the stair itself and the void/hatch/arrival above or below.
                var pair = House.Stairs.Where(o => SameRect(o.Rect, s.Rect) && SameLevels(o, s)).ToList();
                foreach (var o in pair) used.Add(o);
                var main = pair.FirstOrDefault(o => !IsOpeningOnly(o.Kind)) ?? s;
                var lv0 = Level(main.Frm);
                var lv1 = Level(main.To);
                string lower = lv0.Y <= lv1.Y ? main.Frm : main.To, upper = lower == main.Frm ? main.To : main.Frm;
                var lowerRoom = pair.Select(o => House.FindRoom(o.Room)).FirstOrDefault(r => r != null && r.Level == lower) ?? House.FindRoom(main.Room);
                var upperRoom = pair.Select(o => House.FindRoom(o.Room)).FirstOrDefault(o => o != null && o.Level == upper)
                                ?? House.Rooms.FirstOrDefault(o => o.Level == upper && Overlaps(o.Rect, main.Rect)); // an attic hatch has no entry of its own
                // Some flights run into the thickness of an outside wall: keep them to the free floor of both rooms.
                var r = (float[])main.Rect.Clone();
                foreach (var room in new[] { lowerRoom, upperRoom })
                {
                    if (room == null || room.Inner.Length != 4) continue;
                    r[0] = Mathf.Max(r[0], room.Inner[0]); r[1] = Mathf.Max(r[1], room.Inner[1]);
                    r[2] = Mathf.Min(r[2], room.Inner[2]); r[3] = Mathf.Min(r[3], room.Inner[3]);
                }
                // core.py: ascent is the direction of travel from frm to to.
                var dir = VillagePlan.SideNormal(main.Ascent);
                var up = main.Frm == lower ? dir : -dir;
                bool alongZ = Mathf.Abs(dir.y) > 0.5f;
                float run = alongZ ? r[3] - r[1] : r[2] - r[0];
                float rise = Level(upper).Y - Level(lower).Y;
                var f = new PlanFlight
                {
                    Id = main.Id, Room = lowerRoom?.Id, UpperRoom = upperRoom?.Id, Lower = lower, Upper = upper, Up = up,
                    Rect = r, Mat = main.Mat, Note = main.Kind, Rise = rise, Run = run, Width = alongZ ? r[2] - r[0] : r[3] - r[1],
                };
                f.Ladder = f.Slope > 55f; // "ladder-stairs" at 40 degrees are stairs; loft ladders are near vertical
                Flights.Add(f);
                if (lower == "C" || upper == "C") GroundHoles.Add(r);
            }
            // Second pass, once every flight is known: where to step on and off.
            foreach (var f in Flights)
            {
                var r = f.Rect;
                var c = new Vector2((r[0] + r[2]) * 0.5f, (r[1] + r[3]) * 0.5f);
                f.Top = Landing(c + f.Up * (f.Run * 0.5f), f.Up, f.Width, House.FindRoom(f.UpperRoom), f.Upper, f);
                f.Bottom = Landing(c - f.Up * (f.Run * 0.5f), -f.Up, f.Width, House.FindRoom(f.Room), f.Lower, f);
            }
        }

        static bool IsOpeningOnly(string kind)
        {
            var k = (kind ?? "").ToLowerInvariant();
            return k.StartsWith("void") || k.StartsWith("stairwell void") || k.StartsWith("arrives") || k == "hatch";
        }

        static bool SameRect(float[] a, float[] b) => a.Length == 4 && b.Length == 4 && Enumerable.Range(0, 4).All(i => Mathf.Abs(a[i] - b[i]) < 0.01f);
        static bool SameLevels(StairSpec a, StairSpec b) => (a.Frm == b.Frm && a.To == b.To) || (a.Frm == b.To && a.To == b.Frm);

        /// <summary>
        /// Where you step off a flight at one end (end = the middle of that end, out = away from the flight): straight on
        /// if the room has space there, else to either side. Attic tops prefer the spot nearest the attic's middle (headroom).
        /// </summary>
        Vector3 Landing(Vector2 end, Vector2 outward, float width, RoomSpec room, string level, PlanFlight self)
        {
            var side = new Vector2(-outward.y, outward.x);
            var candidates = new List<Vector2> { end + outward * FlightClear };
            foreach (float back in new[] { 0.35f, 0.7f, 1.05f })
                foreach (float s in new[] { 1f, -1f })
                    candidates.Add(end - outward * back + side * (s * (width * 0.5f + 0.45f)));
            if (room == null) return V3(candidates[0], Level(level).Y);
            var inner = room.Inner.Length == 4 ? room.Inner : room.Rect;
            var middle = new Vector2((inner[0] + inner[2]) * 0.5f, (inner[1] + inner[3]) * 0.5f);
            IEnumerable<Vector2> order = level == "A" ? candidates.OrderBy(p => Vector2.Distance(p, middle)) : candidates;
            foreach (var p in order)
            {
                bool inside = p.x > inner[0] + 0.25f && p.x < inner[2] - 0.25f && p.y > inner[1] + 0.25f && p.y < inner[3] - 0.25f;
                bool clear = !Flights.Any(o => o != self && (o.Lower == level || o.Upper == level) && p.x > o.Rect[0] - 0.1f && p.x < o.Rect[2] + 0.1f && p.y > o.Rect[1] - 0.1f && p.y < o.Rect[3] + 0.1f);
                if (inside && clear) return V3(p, Level(level).Y);
            }
            return V3(candidates[0], Level(level).Y);
        }

        static Vector3 V3(Vector2 p, float y) => new Vector3(p.x, y, p.y);

        // ================================================================== rooms

        void MakeRooms()
        {
            foreach (var r in House.Rooms)
            {
                var wing = DesignChecks.WingOf(House, r);
                var lv = Level(r.Level);
                var pr = new PlanRoom { Spec = r, Wing = wing, FloorY = lv.Y, CeilingY = lv.Y + lv.H };
                pr.Probe = ProbeFor(r);
                Rooms.Add(pr);
            }
        }

        /// <summary>The clear spot nearest the room's middle: 0.4 m off its walls and off any stair flight or hole on its level.
        /// An open bay is often shallower than that, so its probe may stand in the bay's mouth, up to 0.6 m into its room.</summary>
        Vector3 ProbeFor(RoomSpec r)
        {
            var inner = (r.Inner.Length == 4 ? r.Inner : r.Rect).ToArray();
            foreach (var side in Sides)
            {
                if (!DesignChecks.Neighbours(House, r, side).Any(o => OpenBay(r, o))) continue;
                if (side == "N") inner[3] += 0.6f;
                else if (side == "S") inner[1] -= 0.6f;
                else if (side == "E") inner[2] += 0.6f;
                else inner[0] -= 0.6f;
            }
            var mid = new Vector2((inner[0] + inner[2]) * 0.5f, (inner[1] + inner[3]) * 0.5f);
            var blocked = Flights.Where(f => f.Lower == r.Level || f.Upper == r.Level).Select(f => f.Rect).ToList();
            Vector2 best = mid;
            float bestD = float.MaxValue;
            for (float x = inner[0] + 0.4f; x <= inner[2] - 0.4f + 1e-4f; x += 0.2f)
            for (float z = inner[1] + 0.4f; z <= inner[3] - 0.4f + 1e-4f; z += 0.2f)
            {
                var p = new Vector2(x, z);
                if (blocked.Any(b => p.x > b[0] - 0.4f && p.x < b[2] + 0.4f && p.y > b[1] - 0.4f && p.y < b[3] + 0.4f)) continue;
                float d = Vector2.Distance(p, mid);
                if (d < bestD) { bestD = d; best = p; }
            }
            return V3(best, Level(r.Level).Y);
        }

        // ================================================================== walls

        void MakeWalls(AssetCatalogSpec catalog)
        {
            foreach (var lvId in House.Levels.Keys)
            {
                if (lvId == "A") continue; // the attic is closed by the roof
                var lv = Level(lvId);
                var rooms = House.Rooms.Where(r => r.Level == lvId).ToList();
                foreach (var r in rooms)
                {
                    var wing = DesignChecks.WingOf(House, r);
                    if (wing == null) continue;
                    var next = NextLevel(wing, lvId);
                    float extTop = next != null ? Level(next).Y : lv.Y + lv.H + SlabT;
                    float extBottom = lvId == "G" ? Mathf.Min(lv.Y, -FloorY) : lv.Y;
                    string extMat = FirstKey(catalog, wing.Walls, "HouseWall");
                    string inMat = FirstKey(catalog, r.Walls, "InnerWall");
                    foreach (var side in Sides)
                    {
                        DesignChecks.WallSpan(r.Rect, side, out float lo, out float hi);
                        var covered = new List<(float a, float b, RoomSpec o)>();
                        foreach (var o in rooms)
                        {
                            if (o == r) continue;
                            if (DesignChecks.SharedEdge(r.Rect, o.Rect, side) <= 0.001f) continue;
                            DesignChecks.WallSpan(o.Rect, side, out float olo, out float ohi);
                            covered.Add((Mathf.Max(lo, olo), Mathf.Min(hi, ohi), o));
                        }
                        // Outside parts of this edge: the exterior wall, thickness inside the rect.
                        foreach (var (a, b) in Subtract(lo, hi, covered.Select(c => (c.a, c.b))))
                        {
                            var ops = new List<PlanOpening>();
                            foreach (var d in House.Doors.Where(d => d.RoomA == r.Id && d.Side == side && d.RoomB == DesignChecks.Outside && d.At > a && d.At < b))
                                ops.Add(DoorOpening(d, r, side, lv.Y, wing.T, true));
                            foreach (var w in House.Windows.Where(w => w.Room == r.Id && w.Side == side && w.At > a && w.At < b))
                                ops.Add(WindowOpening(w, r, side, lv.Y, wing.T));
                            AddRun($"{r.Id}.{side}", r.Rect, side, a, b, wing.T, true, extBottom, extTop, ops, extMat, lvId);
                        }
                        // Shared parts: one partition per pair of rooms (built from the room listed first).
                        foreach (var (a, b, o) in covered)
                        {
                            if (string.CompareOrdinal(r.Id, o.Id) > 0 || b - a < 0.01f) continue;
                            if (OpenBay(r, o)) continue;
                            var ops = new List<PlanOpening>();
                            foreach (var d in House.Doors.Where(d => (d.RoomA == r.Id && d.RoomB == o.Id && d.Side == side) ||
                                                                     (d.RoomA == o.Id && d.RoomB == r.Id && d.Side == Opposite(side))))
                                if (d.At > a && d.At < b) ops.Add(DoorOpening(d, House.FindRoom(d.RoomA), d.Side, lv.Y, PartitionT, false));
                            AddRun($"{r.Id}|{o.Id}", r.Rect, side, a, b, PartitionT, false, lv.Y, lv.Y + lv.H, ops, inMat, lvId);
                        }
                    }
                }
            }
        }

        PlanOpening DoorOpening(DoorSpec d, RoomSpec room, string side, float floor, float thick, bool exterior)
        {
            var n = VillagePlan.SideNormal(side);
            var o = new PlanOpening
            {
                Id = d.Id, IsDoor = true, Level = room.Level, Room = room.Id, Side = side, Exterior = exterior, Door = d,
                Width = d.Width, Height = d.Height, Bottom = floor, WallThick = thick, Outward = new Vector3(n.x, 0f, n.y),
            };
            o.Centre = OpeningCentre(room.Rect, side, d.At, floor + d.Height * 0.5f, exterior ? thick : 0f);
            Openings.Add(o);
            return o;
        }

        PlanOpening WindowOpening(WindowSpec w, RoomSpec room, string side, float floor, float thick)
        {
            var n = VillagePlan.SideNormal(side);
            var o = new PlanOpening
            {
                Id = w.Id, IsDoor = false, Level = room.Level, Room = room.Id, Side = side, Exterior = true, Window = w, Weak = w.Weak,
                Width = w.Width, Height = w.Height, Bottom = floor + w.Sill, WallThick = thick, Outward = new Vector3(n.x, 0f, n.y),
            };
            o.Centre = OpeningCentre(room.Rect, side, w.At, floor + w.Sill + w.Height * 0.5f, thick);
            Openings.Add(o);
            return o;
        }

        /// <summary>Middle of an opening on a room's edge; an exterior wall's centre plane is half its thickness inside.</summary>
        static Vector3 OpeningCentre(float[] r, string side, float at, float y, float extThick)
        {
            float inset = extThick * 0.5f;
            switch (side)
            {
                case "N": return new Vector3(at, y, r[3] - inset);
                case "S": return new Vector3(at, y, r[1] + inset);
                case "E": return new Vector3(r[2] - inset, y, at);
                default: return new Vector3(r[0] + inset, y, at);
            }
        }

        /// <summary>
        /// A wall along one edge of a room from a to b, from y0 to y1, with its openings cut out: solid pieces between
        /// them, and the wall below each sill and above each head.
        /// </summary>
        void AddRun(string name, float[] r, string side, float a, float b, float thick, bool exterior, float y0, float y1,
                    List<PlanOpening> ops, string mat, string level)
        {
            float c0, c1; // the wall's extent across its plane
            switch (side)
            {
                case "N": c0 = exterior ? r[3] - thick : r[3] - thick * 0.5f; c1 = exterior ? r[3] : r[3] + thick * 0.5f; break;
                case "S": c0 = exterior ? r[1] : r[1] - thick * 0.5f; c1 = exterior ? r[1] + thick : r[1] + thick * 0.5f; break;
                case "E": c0 = exterior ? r[2] - thick : r[2] - thick * 0.5f; c1 = exterior ? r[2] : r[2] + thick * 0.5f; break;
                default: c0 = exterior ? r[0] : r[0] - thick * 0.5f; c1 = exterior ? r[0] + thick : r[0] + thick * 0.5f; break;
            }
            bool alongX = side == "N" || side == "S";
            int part = 0;
            void Piece(float s0, float s1, float yy0, float yy1)
            {
                if (s1 - s0 < 0.01f || yy1 - yy0 < 0.01f) return;
                Walls.Add(alongX ? new PlanBox($"{name}_{part++}", s0, yy0, c0, s1, yy1, c1, mat, level)
                                 : new PlanBox($"{name}_{part++}", c0, yy0, s0, c1, yy1, s1, mat, level));
            }
            float cursor = a;
            foreach (var o in ops.OrderBy(o => alongX ? o.Centre.x : o.Centre.z))
            {
                float at = alongX ? o.Centre.x : o.Centre.z;
                float o0 = Mathf.Max(a, at - o.Width * 0.5f), o1 = Mathf.Min(b, at + o.Width * 0.5f);
                float top = Mathf.Min(y1, o.Bottom + o.Height);
                Piece(cursor, o0, y0, y1);
                Piece(o0, o1, y0, o.Bottom);
                Piece(o0, o1, top, y1);
                cursor = Mathf.Max(cursor, o1);
            }
            Piece(cursor, b, y0, y1);
        }

        static bool IsBay(RoomSpec r) => (r.Kind ?? "").EndsWith("-bay");

        /// <summary>A bay with no door to the room it belongs to is open to it: no partition between them.</summary>
        bool OpenBay(RoomSpec r, RoomSpec o) =>
            (IsBay(r) || IsBay(o)) && !House.Doors.Any(d => (d.RoomA == r.Id && d.RoomB == o.Id) || (d.RoomA == o.Id && d.RoomB == r.Id));

        static string Opposite(string side) => side == "N" ? "S" : side == "S" ? "N" : side == "E" ? "W" : "E";

        /// <summary>Parts of [lo, hi] not covered by any interval.</summary>
        public static List<(float, float)> Subtract(float lo, float hi, IEnumerable<(float a, float b)> cover)
        {
            var result = new List<(float, float)>();
            float cursor = lo;
            foreach (var (a, b) in cover.OrderBy(c => c.a))
            {
                if (a > cursor + 0.001f) result.Add((cursor, Mathf.Min(a, hi)));
                cursor = Mathf.Max(cursor, b);
            }
            if (hi > cursor + 0.001f) result.Add((cursor, hi));
            return result;
        }

        // ================================================================== floors and top slabs

        void MakeFloors(AssetCatalogSpec catalog)
        {
            foreach (var r in House.Rooms)
            {
                if (r.Level == "A") continue; // the attic floor is its wing's top slab
                var lv = Level(r.Level);
                // A room's floor has a hole where a flight comes up through it.
                var holes = Flights.Where(f => f.Upper == r.Level && Overlaps(f.Rect, r.Rect)).Select(f => f.Rect).ToList();
                float t = r.Level == "C" ? 0.2f : SlabT;
                foreach (var cell in RectMinus(r.Rect, holes))
                    Floors.Add(new PlanBox($"Floor_{r.Id}", cell[0], lv.Y - t, cell[1], cell[2], lv.Y, cell[3], FirstKey(catalog, r.Floor, "Floor"), r.Level));
            }
            // Each wing's top storey gets a ceiling slab carried out under the eaves (the soffit); with an attic above it is
            // the attic floor. Where another wing's upper floor already sits on it (a jetty), that part is left out.
            foreach (var w in House.Wings)
            {
                var top = TopStorey(w);
                var lv = Level(top);
                float y0 = lv.Y + lv.H, y1 = y0 + SlabT;
                var rect = new[] { w.Rect[0] - Overhang, w.Rect[1] - Overhang, w.Rect[2] + Overhang, w.Rect[3] + Overhang };
                var holes = new List<float[]>();
                holes.AddRange(Flights.Where(f => Mathf.Abs(Level(f.Upper).Y - y1) < 0.05f).Select(f => f.Rect));
                foreach (var o in House.Rooms)
                    if (DesignChecks.WingOf(House, o) != w && Mathf.Abs(Level(o.Level).Y - y1) < 0.05f && o.Level != "A")
                        holes.Add(o.Rect);
                foreach (var other in House.Wings)
                    if (other != w) holes.Add(other.Rect); // the eaves never reach into a neighbouring wing
                var attic = House.Rooms.FirstOrDefault(o => o.Level == "A" && DesignChecks.WingOf(House, o) == w);
                string mat = attic != null ? FirstKey(catalog, attic.Floor, "Floor") : "CeilingInterior";
                foreach (var cell in RectMinus(rect, holes))
                    TopSlabs.Add(new PlanBox($"Top_{w.Id}", cell[0], y0, cell[1], cell[2], y1, cell[3], mat, top));
            }
        }

        static bool Overlaps(float[] a, float[] b) => Mathf.Min(a[2], b[2]) - Mathf.Max(a[0], b[0]) > 0.01f && Mathf.Min(a[3], b[3]) - Mathf.Max(a[1], b[1]) > 0.01f;

        /// <summary>A rect minus holes, as the grid cells (on every hole edge) that no hole covers.</summary>
        public static List<float[]> RectMinus(float[] rect, List<float[]> holes)
        {
            var xs = new List<float> { rect[0], rect[2] };
            var zs = new List<float> { rect[1], rect[3] };
            foreach (var h in holes)
            {
                foreach (var x in new[] { h[0], h[2] }) if (x > rect[0] && x < rect[2]) xs.Add(x);
                foreach (var z in new[] { h[1], h[3] }) if (z > rect[1] && z < rect[3]) zs.Add(z);
            }
            xs = xs.Distinct().OrderBy(v => v).ToList();
            zs = zs.Distinct().OrderBy(v => v).ToList();
            var cells = new List<float[]>();
            for (int j = 0; j + 1 < zs.Count; j++)
            {
                // Merge runs of free cells along x to keep the box count down.
                float start = float.NaN;
                for (int i = 0; i + 1 < xs.Count; i++)
                {
                    float cx = (xs[i] + xs[i + 1]) * 0.5f, cz = (zs[j] + zs[j + 1]) * 0.5f;
                    bool free = !holes.Any(h => cx > h[0] && cx < h[2] && cz > h[1] && cz < h[3]);
                    if (free && float.IsNaN(start)) start = xs[i];
                    if (!free && !float.IsNaN(start)) { cells.Add(new[] { start, zs[j], xs[i], zs[j + 1] }); start = float.NaN; }
                }
                if (!float.IsNaN(start)) cells.Add(new[] { start, zs[j], xs[xs.Count - 1], zs[j + 1] });
            }
            return cells;
        }

        /// <summary>The first word of text that is an asset catalog material key, or the fallback.</summary>
        static string FirstKey(AssetCatalogSpec catalog, string text, string fallback)
        {
            if (string.IsNullOrEmpty(text)) return fallback;
            foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(text, "[A-Za-z][A-Za-z0-9]*"))
            {
                if (m.Value == "Ground" || m.Value == "Floor" || m.Value == "Door") continue; // words that name a key but mean something else
                if (catalog.FindMaterial(m.Value) != null) return m.Value;
            }
            return fallback;
        }
    }
}
