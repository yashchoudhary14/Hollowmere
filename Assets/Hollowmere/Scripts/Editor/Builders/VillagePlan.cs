using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Hollowmere.EditorTools
{
    /// <summary>A box on the ground plan: world (x, z) centre, size along its own x and z, Unity yaw in degrees.</summary>
    public struct Obb2
    {
        public string Id;
        public Vector2 Centre;
        public Vector2 Half;
        public float Yaw;

        public Obb2(string id, Vector2 centre, Vector2 size, float yaw)
        {
            Id = id;
            Centre = centre;
            Half = size * 0.5f;
            Yaw = yaw;
        }

        /// <summary>A box along the segment a-b (its local +z runs from a to b), thick across.</summary>
        public static Obb2 Along(string id, Vector2 a, Vector2 b, float thick) =>
            new Obb2(id, (a + b) * 0.5f, new Vector2(thick, Vector2.Distance(a, b)), VillagePlan.YawOf(b - a));

        public Vector2 Size => Half * 2f;
        public Vector2 ToLocal(Vector2 p) => DesignChecks.Rotate(p.x - Centre.x, p.y - Centre.y, -Yaw);
        public Vector2 ToWorld(Vector2 local) => Centre + DesignChecks.Rotate(local.x, local.y, Yaw);

        public bool Contains(Vector2 p, float margin = 0f)
        {
            var q = ToLocal(p);
            return Mathf.Abs(q.x) <= Half.x + margin && Mathf.Abs(q.y) <= Half.y + margin;
        }

        public Vector2[] Corners() => new[]
        {
            ToWorld(new Vector2(-Half.x, -Half.y)), ToWorld(new Vector2(Half.x, -Half.y)),
            ToWorld(new Vector2(Half.x, Half.y)), ToWorld(new Vector2(-Half.x, Half.y)),
        };
    }

    /// <summary>A road, path or the stream: the design polyline and its smoothed centreline.</summary>
    public sealed class PlanPath
    {
        public string Id;
        public string Name;
        public string Kind;       // street, lane, track, path, stream
        public float Width;
        public bool Closed;
        public Vector2[] Control; // village.json points
        public Vector2[] Points;  // Catmull-Rom, about every 0.5 m

        public float HalfWidth => Width * 0.5f;
        public float Distance(Vector2 p) => VillagePlan.PolylineDistance(Points, p);
        public float ControlDistance(Vector2 p) => VillagePlan.PolylineDistance(Control, p);

        /// <summary>Unit direction of the design segment nearest p.</summary>
        public Vector2 DirectionNear(Vector2 p)
        {
            VillagePlan.NearestOnPolyline(Control, p, out int seg, out _);
            return (Control[seg + 1] - Control[seg]).normalized;
        }
    }

    public enum BoundaryKind { None, Fence, Hedge, LowWall, Wall }

    /// <summary>One straight run of a plot or landmark boundary, already clipped clear of roads, water and doors.</summary>
    public sealed class PlanBoundary
    {
        public string Id;
        public int House = -1;    // owning house index, or -1 for landmarks
        public BoundaryKind Kind;
        public string Mat;
        public Vector2 A, B;
        public float Height;
        public float Thick;
        public bool Solid => Kind != BoundaryKind.Fence;
        public Obb2 Box => Obb2.Along(Id, A, B, Thick);
    }

    public sealed class PlanBridge
    {
        public string Id;
        public string Name;
        public Vector2 Pos;
        public float Yaw;          // local +z runs along the bridge (across the water)
        public float Width;
        public float Length;
        public bool Foot;          // timber footbridge (narrow) rather than the stone road bridge
        public float DeckTop => Foot ? 0.1f : 0.12f;
        public Obb2 Box => new Obb2(Id, Pos, new Vector2(Width, Length), Yaw);
    }

    public sealed class PlanBlocker
    {
        public string Id;
        public string Prop;        // FallenTree or BrokenCart (asset catalog key)
        public Vector2 Pos;
        public float Yaw;          // local +z along the lane, so local x lies across it
        public float Span;         // across the lane, verges included
        public float Depth;
        public Obb2 Box => new Obb2(Id, Pos, new Vector2(Span, Depth), Yaw);
    }

    /// <summary>A placed house's front door: on the outer wall face, facing out, with the straight walk to the nearest road.</summary>
    public sealed class PlanDoor
    {
        public int House;
        public string Slug;
        public Vector2 Door;
        public Vector2 Outward;
        public Vector2 RoadPoint;
        public float FloorY;
        public Vector2 TestPoint => Door + Outward * 1.2f;
    }

    public enum RoofKind { Gable, Flat, LeanTo, MRoof }

    public struct RoofShape
    {
        public RoofKind Kind;
        public bool RidgeAlongX;   // gable ridge along the wing's local x (else z)
        public float Pitch;        // degrees
    }

    /// <summary>
    /// The village v2 ground plan (M1.6 P2), computed from village.json and the house files without touching Unity
    /// objects: smoothed roads, the stream channel and pond, bridges, every solid footprint, plot and landmark
    /// boundaries clipped clear of roads, water and front doors, night blockers across their lanes, roam points,
    /// forest spawns and the forest belt. GreyboxVillageBuilder.VillageV2 turns it into the scene; tests and
    /// offline checks read the same numbers.
    /// </summary>
    public sealed class VillagePlan
    {
        // DESIGN.md / VILLAGE_LAYOUT.md "Water, bridges...": stream bed -1.2, pond surface -0.8.
        public const float StreamBank = 0.6f;   // horizontal run of each bank: steep, so the channel is a barrier
        public const float WaterY = -0.8f;
        public const float PondBedY = -1.6f;
        public const float BarrierThick = 0.3f, BarrierHeight = 1.6f;
        public const float RoadStep = 0.5f;     // VILLAGE_LAYOUT.md: Catmull-Rom smoothed, 0.5 m segments
        const float SampleStep = 0.25f;
        const float WallInner = 150f;            // ring wall centre line (GreyboxVillageBuilder.Half)
        const float TreeSpacing = 5.5f;
        const int TreeSeed = 6203;               // fixed: the forest belt is scenery, not match randomness

        public readonly VillageSpec Village;
        public readonly HouseSpec[] Houses;
        public readonly AssetCatalogSpec Catalog;

        public readonly List<PlanPath> Roads = new List<PlanPath>();
        public PlanPath Stream;
        public Obb2 Pond;
        public readonly List<Obb2> Buildings = new List<Obb2>();   // solid footprints: house wings, filler, landmark buildings
        public readonly Obb2[] Plots;                               // by house index
        public readonly List<PlanDoor> Doors = new List<PlanDoor>();
        public readonly List<PlanBoundary> Boundaries = new List<PlanBoundary>();
        public readonly List<PlanBridge> Bridges = new List<PlanBridge>();
        public readonly List<PlanBlocker> Blockers = new List<PlanBlocker>();
        public readonly List<Obb2> WaterBarriers = new List<Obb2>();
        public readonly List<Vector2> RoamPoints = new List<Vector2>();
        public readonly List<Vector2> ForestSpawns = new List<Vector2>();
        public readonly List<Vector2> Trees = new List<Vector2>();
        public Obb2 Square, Green, Graveyard, Schoolyard, Fields, Pinfold;
        public Rect ForestInner;                                    // forest belt's inner edge (x0, z0)-(x1, z1)

        // The M1 square and chapel stay exactly where GreyboxVillageBuilder builds them.
        public static readonly Vector2 AltarPos = new Vector2(0f, 112f);
        public static readonly Obb2 Chapel = new Obb2("L_CHAPEL", new Vector2(-60f, 112f), new Vector2(12f, 18f), 0f);

        VillagePlan(VillageSpec village, HouseSpec[] houses, AssetCatalogSpec catalog)
        {
            Village = village;
            Houses = houses;
            Catalog = catalog;
            Plots = new Obb2[houses.Length];
        }

        public static VillagePlan Make(VillageSpec village, HouseSpec[] houses, AssetCatalogSpec catalog)
        {
            var p = new VillagePlan(village, houses, catalog);
            p.MakePaths();
            p.MakeFootprints();
            p.MakeDoors();
            p.MakeBridges();
            p.MakeWaterBarriers();
            p.MakeBoundaries();
            p.MakeBlockers();
            p.MakeLayoutPoints();
            p.MakeTrees();
            return p;
        }

        // ================================================================== roads, stream, pond

        void MakePaths()
        {
            foreach (var r in Village.Roads)
            {
                var ctrl = r.Points.Select(V).ToArray();
                bool closed = ctrl.Length > 2 && Vector2.Distance(ctrl[0], ctrl[ctrl.Length - 1]) < 0.01f;
                Roads.Add(new PlanPath
                {
                    Id = r.Id, Name = r.Name, Kind = r.Kind, Width = r.Width, Closed = closed,
                    Control = ctrl, Points = CatmullRom(ctrl, closed, RoadStep),
                });
            }
            var s = Village.Stream.Points.Select(V).ToArray();
            Stream = new PlanPath { Id = "Stream", Name = "Stream", Kind = "stream", Width = Village.Stream.Width, Control = s, Points = CatmullRom(s, false, RoadStep) };
            var pond = Village.FindLandmark("L_POND");
            Pond = new Obb2("L_POND", V(pond.Pos), V(pond.Size), pond.Yaw);
            var f = Village.Bounds.ForestBeltInner;
            ForestInner = Rect.MinMaxRect(f[0], f[1], f[2], f[3]);
        }

        public float StreamBedY => Village.Stream.BedY;
        public float StreamTopHalf => Stream.HalfWidth + StreamBank;

        /// <summary>Ground height: 0 everywhere (streets and house pads stay flat) except the stream channel and the pond.</summary>
        public float GroundHeight(float x, float z)
        {
            var p = new Vector2(x, z);
            float h = 0f;
            if (z < StreamZMax + StreamTopHalf + 1f)
            {
                float d = Stream.Distance(p);
                if (d < StreamTopHalf)
                    h = d <= Stream.HalfWidth ? StreamBedY : Mathf.Lerp(StreamBedY, 0f, (d - Stream.HalfWidth) / StreamBank);
            }
            float e = PondE(p);
            if (e < 1f) h = Mathf.Min(h, e <= 0.8f ? PondBedY : Mathf.Lerp(PondBedY, 0f, (e - 0.8f) / 0.2f));
            return h;
        }

        float StreamZMax => Stream.Control.Max(c => c.y);

        /// <summary>Elliptical radius of p in the pond (1 = the pond's edge).</summary>
        public float PondE(Vector2 p)
        {
            var q = Pond.ToLocal(p);
            return Mathf.Sqrt(Sq(q.x / Pond.Half.x) + Sq(q.y / Pond.Half.y));
        }

        // ================================================================== footprints

        void MakeFootprints()
        {
            for (int i = 0; i < Houses.Length; i++)
            {
                var h = Houses[i];
                var place = Village.FindHouse(h.Slug);
                foreach (var w in h.Wings)
                    Buildings.Add(WingBox(place, h.Slug + "." + w.Id, w.Rect));
                var c = place.PlotPoly.Select(V).ToArray();
                Plots[i] = new Obb2(h.Slug + ".plot", (c[0] + c[1] + c[2] + c[3]) * 0.25f,
                                    new Vector2(Vector2.Distance(c[0], c[1]), Vector2.Distance(c[1], c[2])), place.Yaw);
            }
            foreach (var f in Village.Filler)
                Buildings.Add(new Obb2(f.Id, V(f.Pos), V(f.Size), f.Yaw));
            foreach (var l in Village.Landmarks)
            {
                switch (l.Kind)
                {
                    case "chapel": Buildings.Add(Chapel); break;
                    case "tower": case "board": case "shop": case "school": case "well": case "memorial": case "kiosk": case "shelter":
                        Buildings.Add(LandmarkBox(l));
                        break;
                }
            }
            Square = LandmarkBox(Village.FindLandmark("L_SQUARE"));
            Green = LandmarkBox(Village.FindLandmark("L_GREEN"));
            Graveyard = LandmarkBox(Village.FindLandmark("L_GRAVE"));
            Fields = LandmarkBox(Village.FindLandmark("L_FIELDS"));
            Pinfold = LandmarkBox(Village.FindLandmark("L_PINFOLD"));
            // VILLAGE_LAYOUT.md: schoolyard 26 x 16 behind the school (its back is local -z).
            var school = LandmarkBox(Village.FindLandmark("L_SCHOOL"));
            Schoolyard = new Obb2("Schoolyard", school.ToWorld(new Vector2(0f, -school.Half.y - 8f)), new Vector2(26f, 16f), school.Yaw);
        }

        public static Obb2 LandmarkBox(LandmarkSpec l) => l.Kind == "chapel" ? Chapel : new Obb2(l.Id, V(l.Pos), V(l.Size), l.Yaw);

        public static Obb2 WingBox(VillageHouseSpec place, string id, float[] r)
        {
            var centre = DesignChecks.ToWorld(place, (r[0] + r[2]) * 0.5f, (r[1] + r[3]) * 0.5f);
            return new Obb2(id, centre, new Vector2(r[2] - r[0], r[3] - r[1]), place.Yaw);
        }

        // ================================================================== doors

        void MakeDoors()
        {
            for (int i = 0; i < Houses.Length; i++)
            {
                var h = Houses[i];
                var place = Village.FindHouse(h.Slug);
                if (!DesignChecks.TryFrontDoorLocal(h, out var local)) continue;
                var side = h.Doors.First(d => d.Role == "front" && d.RoomB == DesignChecks.Outside).Side;
                var door = DesignChecks.ToWorld(place, local.x, local.y);
                var outLocal = SideNormal(side);
                var outward = DesignChecks.Rotate(outLocal.x, outLocal.y, place.Yaw);
                var best = new Vector2(float.MaxValue, float.MaxValue);
                foreach (var r in Roads)
                {
                    NearestOnPolyline(r.Control, door, out _, out var q);
                    if (Vector2.Distance(door, q) < Vector2.Distance(door, best)) best = q;
                }
                Doors.Add(new PlanDoor { House = i, Slug = h.Slug, Door = door, Outward = outward, RoadPoint = best, FloorY = place.Position[1] });
            }
        }

        public static Vector2 SideNormal(string side)
        {
            switch (side)
            {
                case "N": return new Vector2(0f, 1f);
                case "S": return new Vector2(0f, -1f);
                case "E": return new Vector2(1f, 0f);
                default: return new Vector2(-1f, 0f);
            }
        }

        /// <summary>Yaw that turns local +z to face out of the given wall.</summary>
        public static float SideYaw(string side)
        {
            switch (side)
            {
                case "N": return 0f;
                case "E": return 90f;
                case "S": return 180f;
                default: return 270f;
            }
        }

        // ================================================================== bridges and water barriers

        void MakeBridges()
        {
            foreach (var b in Village.Bridges)
            {
                // Centred where its road's centreline crosses the stream's, square to the road, and long enough that
                // both ends rest on flat ground (the design length is the minimum).
                var pos = V(b.Pos);
                var road = NearestRoad(pos);
                var dir = road.DirectionNear(pos);
                if (Crossing(road.Points, Stream.Points, pos, out var at, out var roadDir))
                {
                    pos = at;
                    dir = roadDir;
                }
                float reach = 0f;
                foreach (float s in new[] { -1f, 1f })
                {
                    float d = 0f;
                    while (d < 20f && GroundHeight(pos.x + dir.x * s * d, pos.y + dir.y * s * d) < 0f) d += 0.1f;
                    reach = Mathf.Max(reach, d);
                }
                Bridges.Add(new PlanBridge
                {
                    Id = b.Id, Name = b.Name, Pos = pos, Width = b.Width, Length = Mathf.Max(b.Length, 2f * (reach + 0.8f)),
                    Yaw = YawOf(dir), Foot = b.Width < 3f,
                });
            }
        }

        /// <summary>The crossing of two polylines nearest near, with the first one's direction there.</summary>
        static bool Crossing(Vector2[] a, Vector2[] b, Vector2 near, out Vector2 at, out Vector2 dirA)
        {
            at = near;
            dirA = Vector2.zero;
            float best = float.MaxValue;
            for (int i = 0; i + 1 < a.Length; i++)
            for (int j = 0; j + 1 < b.Length; j++)
            {
                if (!SegmentsCross(a[i], a[i + 1], b[j], b[j + 1], out var p)) continue;
                float d = Vector2.Distance(p, near);
                if (d >= best) continue;
                best = d;
                at = p;
                dirA = (a[i + 1] - a[i]).normalized;
            }
            return best < float.MaxValue;
        }

        static bool SegmentsCross(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2, out Vector2 at)
        {
            at = p;
            var r = p2 - p;
            var s = q2 - q;
            float denom = r.x * s.y - r.y * s.x;
            if (Mathf.Abs(denom) < 1e-8f) return false;
            var qp = q - p;
            float t = (qp.x * s.y - qp.y * s.x) / denom;
            float u = (qp.x * r.y - qp.y * r.x) / denom;
            if (t < 0f || t > 1f || u < 0f || u > 1f) return false;
            at = p + r * t;
            return true;
        }

        /// <summary>Invisible barriers along both banks and round the pond, so nobody walks into the water; gaps at the bridges.</summary>
        void MakeWaterBarriers()
        {
            float offset = StreamTopHalf + 0.25f;
            var pts = Stream.Points;
            int n = 0;
            foreach (float s in new[] { -1f, 1f })
            {
                // The bank line, sampled every 0.5 m; runs of allowed samples become pieces of up to 2 m.
                var bank = new Vector2[pts.Length];
                var ok = new bool[pts.Length];
                for (int i = 0; i < pts.Length; i++)
                {
                    var dir = (pts[Mathf.Min(pts.Length - 1, i + 1)] - pts[Mathf.Max(0, i - 1)]).normalized;
                    bank[i] = pts[i] + new Vector2(-dir.y, dir.x) * (s * offset);
                    ok[i] = InsideWall(bank[i], 0.6f) && PondE(bank[i]) >= 1.15f && !InBridgeZone(bank[i]) && !InBuilding(bank[i], 0.1f);
                }
                int start = -1;
                for (int i = 0; i <= pts.Length; i++)
                {
                    bool here = i < pts.Length && ok[i];
                    if (here && start < 0) start = i;
                    if ((!here || i - start == 4) && start >= 0)
                    {
                        int end = here ? i : i - 1;
                        if (end > start)
                        {
                            // Lengthened a little at both ends so the pieces overlap round bends.
                            var dir = (bank[end] - bank[start]).normalized;
                            WaterBarriers.Add(Obb2.Along($"StreamBank_{n++:000}", bank[start] - dir * 0.15f, bank[end] + dir * 0.15f, BarrierThick));
                        }
                        start = here ? i : -1;
                    }
                }
            }
            const int pondPieces = 40;
            for (int k = 0; k < pondPieces; k++)
            {
                Vector2 At(int j)
                {
                    float t = j * Mathf.PI * 2f / pondPieces;
                    return Pond.ToWorld(new Vector2(Mathf.Cos(t) * Pond.Half.x * 1.12f, Mathf.Sin(t) * Pond.Half.y * 1.12f));
                }
                var mid = (At(k) + At(k + 1)) * 0.5f;
                if (OnRoad(mid, 0.3f) || InBuilding(mid, 0.1f)) continue;
                WaterBarriers.Add(Obb2.Along($"PondEdge_{k:00}", At(k), At(k + 1), BarrierThick));
            }
        }

        bool InBridgeZone(Vector2 p)
        {
            foreach (var b in Bridges)
            {
                var q = b.Box.ToLocal(p);
                if (Mathf.Abs(q.x) < b.Width * 0.5f + 0.3f && Mathf.Abs(q.y) < b.Length * 0.5f + 1.5f) return true;
            }
            return false;
        }

        // ================================================================== boundaries

        void MakeBoundaries()
        {
            for (int i = 0; i < Houses.Length; i++)
            {
                var h = Houses[i];
                var front = h.Plot.FirstOrDefault(l => l.StartsWith("Front", StringComparison.OrdinalIgnoreCase)) ?? "";
                var rest = string.Join(" ", h.Plot.Where(l => l.StartsWith("Back", StringComparison.OrdinalIgnoreCase)));
                var frontKind = FrontBoundary(front);
                var backKind = BackBoundary(rest);
                var c = Plots[i].Corners(); // local (x0,z0), (x1,z0), (x1,z1), (x0,z1): back, right side, front, left side
                Run($"{h.Slug}.Back", c[0], c[1], backKind, BoundaryMat(backKind, rest), i);
                Run($"{h.Slug}.SideR", c[1], c[2], backKind, BoundaryMat(backKind, rest), i);
                Run($"{h.Slug}.Front", c[2], c[3], frontKind, BoundaryMat(frontKind, front), i);
                Run($"{h.Slug}.SideL", c[3], c[0], backKind, BoundaryMat(backKind, rest), i);
            }

            // Graveyard: dry-stone wall 1.2 m with the lychgate gap on Chapel Lane (VILLAGE_LAYOUT.md L_GRAVE).
            var lych = new Vector2(-56f, 88f);
            var g = Graveyard.Corners();
            for (int e = 0; e < 4; e++)
                Run($"Graveyard.{e}", g[e], g[(e + 1) % 4], BoundaryKind.Wall, "Stone", -1, height: 1.2f, gate: lych, gateWidth: 3f);

            // Schoolyard wall 2.2 m, gate on its west side.
            var y = Schoolyard.Corners();
            var yardGate = Schoolyard.ToWorld(new Vector2(-Schoolyard.Half.x, 0f));
            for (int e = 0; e < 4; e++)
                Run($"Schoolyard.{e}", y[e], y[(e + 1) % 4], BoundaryKind.Wall, "BrickRed", -1, height: 2.2f, gate: yardGate, gateWidth: 2.5f);

            // Fields: hedges all round; the field track and East Road cut their own gaps.
            var f = Fields.Corners();
            for (int e = 0; e < 4; e++)
                Run($"Fields.{e}", f[e], f[(e + 1) % 4], BoundaryKind.Hedge, "Hedge", -1);

            // Pinfold: a round dry-stone pen, open towards the nearest road.
            var pc = Pinfold.Centre;
            NearestOnPolyline(NearestRoad(pc).Control, pc, out _, out var towards);
            float gateAngle = Mathf.Atan2(towards.y - pc.y, towards.x - pc.x);
            const int pieces = 20;
            float radius = Pinfold.Half.x;
            for (int k = 0; k < pieces; k++)
            {
                float a0 = k * Mathf.PI * 2f / pieces, a1 = (k + 1) * Mathf.PI * 2f / pieces;
                if (Mathf.Abs(Mathf.DeltaAngle((a0 + a1) * 0.5f * Mathf.Rad2Deg, gateAngle * Mathf.Rad2Deg)) < 20f) continue;
                var pa = pc + new Vector2(Mathf.Cos(a0), Mathf.Sin(a0)) * radius;
                var pb = pc + new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * radius;
                Boundaries.Add(new PlanBoundary { Id = $"Pinfold.{k:00}", Kind = BoundaryKind.Wall, Mat = "Stone", A = pa, B = pb, Height = 1.5f, Thick = 0.5f });
            }
        }

        public static BoundaryKind FrontBoundary(string line)
        {
            var s = line.ToLowerInvariant();
            if (s.Length == 0) return BoundaryKind.Fence;
            if (s.Contains("no garden") || s.StartsWith("front: pavement")) return BoundaryKind.None;
            if (s.Contains("hedge")) return BoundaryKind.Hedge;
            if (s.Contains("wall") || s.Contains("railings")) return BoundaryKind.LowWall;
            return BoundaryKind.Fence;
        }

        public static BoundaryKind BackBoundary(string text)
        {
            var s = text.ToLowerInvariant();
            if (s.Contains("walled") || Regex.IsMatch(s, @"\bwall\b")) return BoundaryKind.Wall;
            if (s.Contains("hedge")) return BoundaryKind.Hedge;
            return BoundaryKind.Fence;
        }

        static string BoundaryMat(BoundaryKind kind, string text)
        {
            switch (kind)
            {
                case BoundaryKind.Hedge: return "Hedge";
                case BoundaryKind.Fence: return "Planks";
                default: return text.IndexOf("brick", StringComparison.OrdinalIgnoreCase) >= 0 ? "BrickRed" : "Stone";
            }
        }

        static float DefaultHeight(BoundaryKind k) =>
            k == BoundaryKind.Wall ? 1.8f : k == BoundaryKind.Hedge ? 1.6f : k == BoundaryKind.LowWall ? 0.9f : 1.1f;

        static float DefaultThick(BoundaryKind k) =>
            k == BoundaryKind.Hedge ? 0.8f : k == BoundaryKind.Fence ? 0.1f : 0.4f;

        /// <summary>Adds the parts of a-b where a boundary may stand (see BoundaryAllowed), as straight runs.</summary>
        void Run(string id, Vector2 a, Vector2 b, BoundaryKind kind, string mat, int house,
                 float height = -1f, Vector2? gate = null, float gateWidth = 0f)
        {
            if (kind == BoundaryKind.None) return;
            float len = Vector2.Distance(a, b);
            if (len < 0.01f) return;
            int n = Mathf.Max(1, Mathf.CeilToInt(len / SampleStep));
            int start = -1, part = 0;
            for (int k = 0; k <= n; k++)
            {
                bool ok = false;
                if (k < n)
                {
                    var p = Vector2.Lerp(a, b, (k + 0.5f) / n);
                    ok = BoundaryAllowed(p, house) && !(gate.HasValue && Vector2.Distance(p, gate.Value) < gateWidth * 0.5f);
                }
                if (ok && start < 0) start = k;
                if (!ok && start >= 0)
                {
                    var pa = Vector2.Lerp(a, b, (float)start / n);
                    var pb = Vector2.Lerp(a, b, (float)k / n);
                    if (Vector2.Distance(pa, pb) >= 0.6f)
                        Boundaries.Add(new PlanBoundary
                        {
                            Id = $"{id}_{part++}", House = house, Kind = kind, Mat = mat, A = pa, B = pb,
                            Height = height > 0f ? height : DefaultHeight(kind), Thick = DefaultThick(kind),
                        });
                    start = -1;
                }
            }
        }

        bool BoundaryAllowed(Vector2 p, int house)
        {
            if (!InsideWall(p, 1.2f) || OnRoad(p, 0.4f) || InWater(p, 0.4f) || InBuilding(p, 0.25f) || OnDoorPath(p, 0.9f)) return false;
            if (Square.Contains(p, 0.5f) || InEllipse(Green, p, 1.02f)) return false;
            for (int j = 0; j < Plots.Length; j++)
                if (j != house && Plots[j].Contains(p, -0.4f)) return false;
            return true;
        }

        // ================================================================== night blockers

        void MakeBlockers()
        {
            foreach (var b in Village.NightBlockers)
            {
                var pos = V(b.Pos);
                var road = NearestRoad(pos);
                bool tree = b.Prop == "FallenTree";
                Blockers.Add(new PlanBlocker
                {
                    Id = b.Id, Prop = b.Prop, Pos = pos, Yaw = YawOf(road.DirectionNear(pos)),
                    Span = road.Width + 3f, Depth = tree ? 1.2f : 1.6f,
                });
            }
        }

        // ================================================================== layout points and the forest belt

        void MakeLayoutPoints()
        {
            // VILLAGE_LAYOUT.md "Gameplay hooks": road vertices + the green + the square.
            var candidates = Roads.SelectMany(r => r.Control).ToList();
            candidates.Add(Green.Centre);
            candidates.Add(new Vector2(-20f, 100f));
            candidates.Add(new Vector2(20f, 100f));
            candidates.Add(new Vector2(-22f, 135f));
            foreach (var c in candidates)
            {
                if (!InsideWall(c, 3f) || InWater(c, 1f) || InBuilding(c, 1.2f) || Blockers.Any(b => b.Box.Contains(c, 1.5f))) continue;
                if (RoamPoints.Any(r => Vector2.Distance(r, c) < 4f)) continue;
                RoamPoints.Add(c);
            }

            // Eight on the forest belt's inner edge, round the compass (north: either side of the square).
            Vector2[] spawns =
            {
                new Vector2(-126f, 126f), new Vector2(126f, 126f), new Vector2(-126f, 40f), new Vector2(126f, 40f),
                new Vector2(-126f, -60f), new Vector2(126f, -50f), new Vector2(-60f, -138f), new Vector2(40f, -138f),
            };
            foreach (var s in spawns)
            {
                var p = s;
                for (int k = 0; k < 20 && !SpawnClear(p); k++) p = Vector2.MoveTowards(p, Vector2.zero, 2f);
                ForestSpawns.Add(p);
            }
        }

        bool SpawnClear(Vector2 p) => !InBuilding(p, 2f) && !InWater(p, 2f) && !OnRoad(p, 0.5f) && !InAnyPlot(p, 1f) && !Fields.Contains(p, 1f);

        void MakeTrees()
        {
            var rng = new System.Random(TreeSeed);
            for (float x = -WallInner + 3f; x <= WallInner - 3f; x += TreeSpacing)
            for (float z = -WallInner + 3f; z <= WallInner - 3f; z += TreeSpacing)
            {
                var p = new Vector2(x + Jitter(rng, 2.5f), z + Jitter(rng, 2.5f));
                if (ForestInner.Contains(p) || !InsideWall(p, 2.5f)) continue;
                if (OnRoad(p, 3f) || InBuilding(p, 3f) || InAnyPlot(p, 1f) || InWater(p, 2.5f) || Fields.Contains(p, 1f)) continue;
                if (Square.Contains(p, 6f) || Graveyard.Contains(p, 2f) || OnDoorPath(p, 2f)) continue;
                if (ForestSpawns.Any(s => Vector2.Distance(s, p) < 3f)) continue;
                if (Boundaries.Any(b => b.Solid && b.Box.Contains(p, 1f))) continue;
                Trees.Add(p);
            }
        }

        static float Jitter(System.Random rng, float amount) => ((float)rng.NextDouble() * 2f - 1f) * amount;

        // ================================================================== queries

        public bool InsideWall(Vector2 p, float margin) => Mathf.Abs(p.x) < WallInner - margin && Mathf.Abs(p.y) < WallInner - margin;

        public bool OnRoad(Vector2 p, float margin)
        {
            foreach (var r in Roads)
                if (r.Distance(p) < r.HalfWidth + margin) return true;
            return false;
        }

        public bool InWater(Vector2 p, float margin) => Stream.Distance(p) < StreamTopHalf + margin || PondE(p) < 1f + margin / Mathf.Min(Pond.Half.x, Pond.Half.y);

        public bool InBuilding(Vector2 p, float margin)
        {
            foreach (var b in Buildings)
                if (b.Contains(p, margin)) return true;
            return false;
        }

        public bool InAnyPlot(Vector2 p, float margin)
        {
            foreach (var pl in Plots)
                if (pl.Contains(p, margin)) return true;
            return false;
        }

        /// <summary>Within margin of a 3 m wide walk from a front door straight to its road.</summary>
        public bool OnDoorPath(Vector2 p, float margin)
        {
            foreach (var d in Doors)
                if (SegmentDistance(p, d.Door, d.RoadPoint) < 1.5f + margin) return true;
            return false;
        }

        public PlanPath NearestRoad(Vector2 p)
        {
            PlanPath best = null;
            float bestD = float.MaxValue;
            foreach (var r in Roads)
            {
                float d = r.ControlDistance(p);
                if (d < bestD) { bestD = d; best = r; }
            }
            return best;
        }

        static bool InEllipse(Obb2 box, Vector2 p, float scale)
        {
            var q = box.ToLocal(p);
            return Sq(q.x / (box.Half.x * scale)) + Sq(q.y / (box.Half.y * scale)) <= 1f;
        }

        /// <summary>The first word of text that is an asset catalog material key, or the fallback.</summary>
        public string FirstMaterialKey(string text, string fallback)
        {
            if (string.IsNullOrEmpty(text)) return fallback;
            foreach (Match m in Regex.Matches(text, "[A-Za-z][A-Za-z0-9]*"))
                if (Catalog.FindMaterial(m.Value) != null) return m.Value;
            return fallback;
        }

        // ================================================================== house massing (P2 footprint boxes)

        /// <summary>Lowest floor a wing stands on (cellars don't count), house-local.</summary>
        public static float WingBottom(HouseSpec h, WingSpec w)
        {
            float y = float.MaxValue;
            foreach (var lv in w.Levels)
                if (lv != "C" && lv != "A" && h.Levels.TryGetValue(lv, out var l)) y = Mathf.Min(y, l.Y);
            return y == float.MaxValue ? 0f : y;
        }

        /// <summary>Eaves height of a wing, house-local: the top of its highest storey (the attic is in the roof) plus the slab.</summary>
        public static float WingEaves(HouseSpec h, WingSpec w)
        {
            float y = 0f;
            foreach (var lv in w.Levels)
                if (lv != "C" && lv != "A" && h.Levels.TryGetValue(lv, out var l)) y = Mathf.Max(y, l.Y + l.H);
            return y + 0.25f;
        }

        /// <summary>Reads a wing's roof text ("Gable roof 35 deg, ridge along x...") into a greybox roof shape.</summary>
        public static RoofShape ParseRoof(string text, float spanX, float spanZ)
        {
            var s = (text ?? "").ToLowerInvariant();
            var shape = new RoofShape { Kind = RoofKind.Gable, RidgeAlongX = spanX >= spanZ, Pitch = 40f };
            var pitch = Regex.Match(s, @"(\d+(?:\.\d+)?)\s*deg");
            if (pitch.Success) shape.Pitch = Mathf.Clamp(float.Parse(pitch.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture), 10f, 65f);
            if (s.Contains("ridge along x") || s.Contains("ridges along x")) shape.RidgeAlongX = true;
            else if (s.Contains("ridge along z") || s.Contains("ridges along z")) shape.RidgeAlongX = false;

            if (s.Contains("m-roof")) shape.Kind = RoofKind.MRoof;
            else if (s.Trim().Length <= 1 || s.StartsWith("flat") || s.Contains("flat roof") || s.Contains("flat top") || s.Contains("flat lead") || s.Contains("flat felt"))
                shape.Kind = RoofKind.Flat;
            else if (s.Contains("lean-to") || s.Contains("mono-pitch") || s.Contains("catslide"))
            {
                shape.Kind = RoofKind.LeanTo;
                if (!pitch.Success) shape.Pitch = 22f;
            }
            return shape;
        }

        // ================================================================== geometry

        public static Vector2 V(float[] a) => new Vector2(a[0], a[1]);

        static float Sq(float v) => v * v;

        /// <summary>Unity yaw (degrees) whose local +z points along dir.</summary>
        public static float YawOf(Vector2 dir) => Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        public static float SegmentDistance(Vector2 p, Vector2 a, Vector2 b)
        {
            var ab = b - a;
            float l2 = ab.sqrMagnitude;
            float t = l2 == 0f ? 0f : Mathf.Clamp01(Vector2.Dot(p - a, ab) / l2);
            return Vector2.Distance(p, a + ab * t);
        }

        public static float PolylineDistance(IList<Vector2> pts, Vector2 p)
        {
            float best = float.MaxValue;
            for (int i = 0; i < pts.Count - 1; i++) best = Mathf.Min(best, SegmentDistance(p, pts[i], pts[i + 1]));
            return best;
        }

        public static void NearestOnPolyline(IList<Vector2> pts, Vector2 p, out int segment, out Vector2 nearest)
        {
            segment = 0;
            nearest = pts[0];
            float best = float.MaxValue;
            for (int i = 0; i < pts.Count - 1; i++)
            {
                var ab = pts[i + 1] - pts[i];
                float l2 = ab.sqrMagnitude;
                float t = l2 == 0f ? 0f : Mathf.Clamp01(Vector2.Dot(p - pts[i], ab) / l2);
                var q = pts[i] + ab * t;
                float d = Vector2.Distance(p, q);
                if (d < best) { best = d; segment = i; nearest = q; }
            }
        }

        /// <summary>
        /// Uniform Catmull-Rom through the control points, sampled about every step metres. Passes through every control
        /// point; a closed path (first point repeated last) wraps round and ends where it starts.
        /// </summary>
        public static Vector2[] CatmullRom(IList<Vector2> ctrl, bool closed, float step)
        {
            var c = closed ? ctrl.Take(ctrl.Count - 1).ToList() : ctrl.ToList();
            int count = c.Count;
            if (count < 2) return c.ToArray();
            Vector2 P(int i) => closed ? c[((i % count) + count) % count] : c[Mathf.Clamp(i, 0, count - 1)];
            var outPts = new List<Vector2>();
            int segments = closed ? count : count - 1;
            for (int i = 0; i < segments; i++)
            {
                Vector2 p0 = P(i - 1), p1 = P(i), p2 = P(i + 1), p3 = P(i + 2);
                int n = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(p1, p2) / step));
                for (int k = 0; k < n; k++)
                {
                    float t = (float)k / n, t2 = t * t, t3 = t2 * t;
                    outPts.Add(0.5f * (2f * p1 + (p2 - p0) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 + (3f * p1 - p0 - 3f * p2 + p3) * t3));
                }
            }
            outPts.Add(closed ? c[0] : c[count - 1]);
            return outPts.ToArray();
        }
    }
}
