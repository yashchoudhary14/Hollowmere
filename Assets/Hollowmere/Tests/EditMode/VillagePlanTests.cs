using System.Linq;
using Hollowmere.EditorTools;
using NUnit.Framework;
using UnityEngine;

namespace Hollowmere.Tests
{
    /// <summary>
    /// M1.6 P2: the village v2 ground plan (VillagePlan) computed from docs/design, before any scene is built: roads
    /// follow their design polylines, the stream only crosses roads at the bridges, boundaries and water barriers
    /// never close a road or a front door, and the night blockers sit across their lanes.
    /// </summary>
    public class VillagePlanTests
    {
        VillagePlan _plan;

        [OneTimeSetUp]
        public void Make() => _plan = VillagePlan.Make(DesignData.LoadVillage(), DesignData.LoadHouses(), DesignData.LoadCatalog());

        [Test]
        public void CatmullRom_PassesThroughEveryControlPoint()
        {
            var ctrl = new[] { new Vector2(0f, 0f), new Vector2(10f, 3f), new Vector2(14f, 15f), new Vector2(2f, 20f) };
            var pts = VillagePlan.CatmullRom(ctrl, false, 0.5f);
            foreach (var c in ctrl)
                Assert.That(pts.Min(p => Vector2.Distance(p, c)), Is.LessThan(1e-3f), c.ToString());
            for (int i = 0; i + 1 < pts.Length; i++)
                Assert.That(Vector2.Distance(pts[i], pts[i + 1]), Is.LessThan(0.75f), "about every 0.5 m (a bend's arc is a little longer than its chord)");
            var loop = VillagePlan.CatmullRom(new[] { ctrl[0], ctrl[1], ctrl[2], ctrl[0] }, true, 0.5f);
            Assert.That(Vector2.Distance(loop[0], loop[loop.Length - 1]), Is.LessThan(1e-4f), "a closed path ends where it starts");
        }

        [Test]
        public void EveryRoad_FollowsItsDesignPolyline()
        {
            Assert.That(_plan.Roads.Count, Is.EqualTo(_plan.Village.Roads.Length));
            foreach (var r in _plan.Roads)
                foreach (var p in r.Points)
                    Assert.That(r.ControlDistance(p), Is.LessThan(r.HalfWidth + 0.5f), $"{r.Id} strays from its design line at {p}"); // corners round off a little
        }

        [Test]
        public void Ground_IsFlatUnderRoadsDoorsAndHouses_ExceptWhereBridgesCross()
        {
            foreach (var r in _plan.Roads)
                foreach (var p in r.Points)
                    if (!_plan.Bridges.Any(b => b.Box.Contains(p, 1f)))
                        Assert.That(_plan.GroundHeight(p.x, p.y), Is.EqualTo(0f), $"{r.Id} dips into water at {p}");
            foreach (var d in _plan.Doors)
                Assert.That(_plan.GroundHeight(d.TestPoint.x, d.TestPoint.y), Is.EqualTo(0f), d.Slug);
            Assert.That(_plan.GroundHeight(_plan.Stream.Control[3].x, _plan.Stream.Control[3].y), Is.LessThan(-1f), "the stream bed is sunk");
        }

        [Test]
        public void Bridges_SpanTheChannel()
        {
            Assert.That(_plan.Bridges.Count, Is.EqualTo(2));
            foreach (var b in _plan.Bridges)
            {
                Assert.That(_plan.Stream.Distance(b.Pos), Is.LessThan(_plan.Stream.HalfWidth), $"{b.Id} isn't over the stream");
                foreach (float end in new[] { -0.5f, 0.5f })
                {
                    var p = b.Box.ToWorld(new Vector2(0f, end * b.Length));
                    Assert.That(_plan.GroundHeight(p.x, p.y), Is.EqualTo(0f), $"{b.Id} end {end} isn't on the bank");
                }
                foreach (var w in _plan.WaterBarriers)
                    for (float t = -0.5f; t <= 0.5f; t += 0.1f)
                    {
                        var q = b.Box.ToLocal(w.ToWorld(new Vector2(0f, t * w.Size.y)));
                        Assert.That(Mathf.Abs(q.x) >= b.Width * 0.5f || Mathf.Abs(q.y) >= b.Length * 0.5f, Is.True, $"{w.Id} cuts across {b.Id}");
                    }
            }
        }

        [Test]
        public void EveryHouse_HasAFrontDoor_WithAClearWalkToItsRoad()
        {
            Assert.That(_plan.Doors.Select(d => d.House), Is.EquivalentTo(Enumerable.Range(0, HouseCatalog.Count)));
            foreach (var d in _plan.Doors)
            {
                Assert.That(Vector2.Distance(d.Door, d.RoadPoint), Is.LessThan(14.5f), $"{d.Slug} is too far from a road");
                for (float t = 0.05f; t <= 1f; t += 0.05f)
                {
                    var p = Vector2.Lerp(d.Door + d.Outward * 0.6f, d.RoadPoint, t);
                    Assert.That(_plan.InBuilding(p, 0.1f), Is.False, $"{d.Slug}'s walk runs into a building at {p}");
                    Assert.That(_plan.Boundaries.Any(b => b.Solid && b.Box.Contains(p, 0.4f)), Is.False, $"{d.Slug}'s walk runs into a wall at {p}");
                    Assert.That(_plan.WaterBarriers.Any(b => b.Contains(p, 0.4f)), Is.False, $"{d.Slug}'s walk runs into a barrier at {p}");
                }
            }
        }

        [Test]
        public void Boundaries_StayOffRoadsAndWater()
        {
            Assert.That(_plan.Boundaries.Count(b => b.House >= 0), Is.GreaterThan(30));
            foreach (var b in _plan.Boundaries)
                for (float t = 0.1f; t <= 0.9f; t += 0.2f)
                {
                    var p = Vector2.Lerp(b.A, b.B, t);
                    Assert.That(_plan.OnRoad(p, 0.3f), Is.False, $"{b.Id} stands on a road at {p}");
                    Assert.That(_plan.InWater(p, 0.3f), Is.False, $"{b.Id} stands in the water at {p}");
                }
        }

        [Test]
        public void NightBlockers_LieAcrossTheirLanes()
        {
            Assert.That(_plan.Blockers.Count, Is.EqualTo(6));
            foreach (var b in _plan.Blockers)
            {
                var road = _plan.NearestRoad(b.Pos);
                Assert.That(road.ControlDistance(b.Pos), Is.LessThan(road.HalfWidth), $"{b.Id} isn't on a road");
                Assert.That(b.Span, Is.GreaterThan(road.Width), $"{b.Id} doesn't span {road.Id}");
                var along = DesignChecks.Rotate(0f, 1f, b.Yaw);
                Assert.That(Mathf.Abs(Vector2.Dot(along, road.DirectionNear(b.Pos))), Is.GreaterThan(0.99f), $"{b.Id} isn't square to {road.Id}");
            }
        }

        [Test]
        public void RoamAndSpawnPoints_AreClear()
        {
            Assert.That(_plan.RoamPoints.Count, Is.GreaterThan(30));
            Assert.That(_plan.ForestSpawns.Count, Is.EqualTo(8));
            foreach (var p in _plan.RoamPoints.Concat(_plan.ForestSpawns))
            {
                Assert.That(_plan.InBuilding(p, 1f), Is.False, p.ToString());
                Assert.That(_plan.InWater(p, 0.5f), Is.False, p.ToString());
                Assert.That(_plan.InsideWall(p, 2f), Is.True, p.ToString());
            }
            foreach (var t in _plan.Trees)
                Assert.That(_plan.ForestInner.Contains(t), Is.False, $"tree {t} inside the village");
        }

        [Test]
        public void Roofs_ReadFromTheHouseText()
        {
            var gable = VillagePlan.ParseRoof("Gable roof, ridge along x at 9.6 m, 42 deg, RoofSlateGrey", 7.6f, 10f);
            Assert.That(gable.Kind, Is.EqualTo(RoofKind.Gable));
            Assert.That(gable.RidgeAlongX, Is.True);
            Assert.That(gable.Pitch, Is.EqualTo(42f));
            Assert.That(VillagePlan.ParseRoof("Steep 55 deg gable facing the street (ridge along z)", 7f, 8f).RidgeAlongX, Is.False);
            Assert.That(VillagePlan.ParseRoof("Flat roof hidden behind a 0.6 m parapet", 5f, 6f).Kind, Is.EqualTo(RoofKind.Flat));
            Assert.That(VillagePlan.ParseRoof("-", 9f, 8f).Kind, Is.EqualTo(RoofKind.Flat));
            Assert.That(VillagePlan.ParseRoof("Catslide continuation of the main roof", 6f, 2.4f).Kind, Is.EqualTo(RoofKind.LeanTo));
            Assert.That(VillagePlan.ParseRoof("M-roof: two parallel gables with ridges along z", 10f, 8f).Kind, Is.EqualTo(RoofKind.MRoof));
            Assert.That(VillagePlan.FrontBoundary("Front: no garden - house sits on the pavement line"), Is.EqualTo(BoundaryKind.None));
            Assert.That(VillagePlan.FrontBoundary("Front: 6 m garden, crazy-paving path, overgrown privet hedge"), Is.EqualTo(BoundaryKind.Hedge));
            Assert.That(VillagePlan.BackBoundary("Back: tiny walled garden"), Is.EqualTo(BoundaryKind.Wall));
        }
    }
}
