using System.Collections.Generic;
using System.Linq;
using Hollowmere.EditorTools;
using NUnit.Framework;
using UnityEngine;

namespace Hollowmere.Tests
{
    /// <summary>
    /// M1.6 P3: each house's shell plan (HousePlan) from its Hxx.json, before any scene is built: every door and window
    /// is cut at its design size, nothing walls an opening up, flights are walkable and ladders are ladders, and every
    /// room has a clear spot and a way on and off each flight.
    /// </summary>
    public class HousePlanTests
    {
        HousePlan[] _plans;

        [OneTimeSetUp]
        public void Make()
        {
            var village = DesignData.LoadVillage();
            var catalog = DesignData.LoadCatalog();
            _plans = DesignData.LoadHouses().Select(h => HousePlan.Make(h, village.FindHouse(h.Slug), catalog)).ToArray();
        }

        static bool Inside(PlanBox b, Vector3 p, float margin = 0f) =>
            p.x > b.Min.x - margin && p.x < b.Max.x + margin && p.y > b.Min.y - margin && p.y < b.Max.y + margin && p.z > b.Min.z - margin && p.z < b.Max.z + margin;

        [Test]
        public void EveryDoorAndWindow_IsCutAtItsDesignSize()
        {
            foreach (var p in _plans)
            {
                var h = p.House;
                foreach (var w in h.Windows.Where(w => h.FindRoom(w.Room).Level != "A")) // attic gable lights come with the roofs' dressing,
                    Assert.That(p.Openings.Count(x => !x.IsDoor && x.Id == w.Id), Is.EqualTo(1), $"{h.Slug} window {w.Id}");
                foreach (var d in h.Doors.Where(d => d.Level != "A")) // so does H09's lucam (hoist door) in the roof
                {
                    var o = p.Openings.Where(x => x.IsDoor && x.Id == d.Id).ToList();
                    Assert.That(o.Count, Is.EqualTo(1), $"{h.Slug} door {d.Id} cut {o.Count} times");
                    Assert.That(o[0].Width, Is.EqualTo(d.Width), $"{h.Slug} {d.Id}");
                    Assert.That(d.Width, Is.GreaterThanOrEqualTo(0.7f), $"{h.Slug} {d.Id}: narrower than a player");
                }
            }
        }

        [Test]
        public void NoWallPiece_StandsInAnOpening()
        {
            foreach (var p in _plans)
                foreach (var o in p.Openings)
                {
                    // Sample the opening's clear area on its wall plane, inset 5 cm from its edges.
                    var along = o.Side == "N" || o.Side == "S" ? Vector3.right : Vector3.forward;
                    for (float u = -0.5f; u <= 0.5f; u += 0.25f)
                    for (float v = 0.1f; v <= 0.9f; v += 0.2f)
                    {
                        var q = o.Centre + along * (u * (o.Width - 0.1f)) + Vector3.up * ((v - 0.5f) * o.Height);
                        var hit = p.Walls.FirstOrDefault(w => Inside(w, q));
                        Assert.That(hit.Name, Is.Null, $"{p.House.Slug}: {hit.Name} stands in {o.Id}");
                    }
                }
        }

        [Test]
        public void Flights_AreWalkable_AndLaddersAreLadders()
        {
            foreach (var p in _plans)
                foreach (var f in p.Flights)
                {
                    if (f.Ladder) Assert.That(f.Slope, Is.GreaterThan(55f), $"{p.House.Slug} {f.Id}");
                    else Assert.That(f.Slope, Is.LessThanOrEqualTo(48f), $"{p.House.Slug} {f.Id} is too steep to walk ({f.Slope:0} deg)");
                    Assert.That(f.Width, Is.GreaterThanOrEqualTo(f.Ladder ? 0.45f : 0.6f), $"{p.House.Slug} {f.Id}"); // a ladder is climbed by interacting
                    Assert.That(f.Room, Is.Not.Null, $"{p.House.Slug} {f.Id} has no room below");
                    Assert.That(f.UpperRoom, Is.Not.Null, $"{p.House.Slug} {f.Id} has no room above");
                }
        }

        [Test]
        public void EveryFlight_StepsOnAndOffInsideItsRooms_ClearOfWalls()
        {
            foreach (var p in _plans)
                foreach (var f in p.Flights)
                {
                    foreach (var (point, roomId) in new[] { (f.Bottom, f.Room), (f.Top, f.UpperRoom) })
                    {
                        var room = p.House.FindRoom(roomId);
                        var inner = room.Inner;
                        Assert.That(point.x > inner[0] && point.x < inner[2] && point.z > inner[1] && point.z < inner[3], Is.True,
                                    $"{p.House.Slug} {f.Id}: {point} is outside {roomId}");
                        var feet = point + Vector3.up * 0.5f;
                        Assert.That(p.Walls.Any(w => Inside(w, feet, 0.2f)), Is.False, $"{p.House.Slug} {f.Id}: {point} is against a wall");
                    }
                }
        }

        [Test]
        public void EveryRoom_HasAClearProbe_OnItsFloor()
        {
            foreach (var p in _plans)
            {
                Assert.That(p.Rooms.Select(r => r.Spec.Id), Is.EqualTo(p.House.Rooms.Select(r => r.Id)), p.House.Slug);
                foreach (var r in p.Rooms)
                {
                    Assert.That(r.Probe.y, Is.EqualTo(p.House.Levels[r.Spec.Level].Y), $"{p.House.Slug} {r.Spec.Id}");
                    var feet = r.Probe + Vector3.up * 0.5f;
                    Assert.That(p.Walls.Any(w => Inside(w, feet, 0.25f)), Is.False, $"{p.House.Slug} {r.Spec.Id}: probe against a wall");
                    Assert.That(p.Flights.Any(f => (f.Lower == r.Spec.Level || f.Upper == r.Spec.Level) &&
                                                   r.Probe.x > f.Rect[0] && r.Probe.x < f.Rect[2] && r.Probe.z > f.Rect[1] && r.Probe.z < f.Rect[3]),
                                Is.False, $"{p.House.Slug} {r.Spec.Id}: probe on a stair");
                }
            }
        }

        [Test]
        public void Floors_HaveHolesOnlyOverFlights_AndCellarStairsOpenTheGround()
        {
            foreach (var p in _plans)
            {
                foreach (var f in p.Flights)
                {
                    var mid = new Vector3((f.Rect[0] + f.Rect[2]) * 0.5f, p.Level(f.Upper).Y - 0.1f, (f.Rect[1] + f.Rect[3]) * 0.5f);
                    Assert.That(p.Floors.Concat(p.TopSlabs).Any(b => Inside(b, mid)), Is.False, $"{p.House.Slug} {f.Id}: no stairwell in the floor above");
                }
                int cellarFlights = p.Flights.Count(f => f.Lower == "C" || f.Upper == "C");
                Assert.That(p.GroundHoles.Count, Is.EqualTo(cellarFlights), p.House.Slug);
                Assert.That(p.FloorY, Is.GreaterThanOrEqualTo(HousePlan.MinFloor), p.House.Slug);
            }
        }

        [Test]
        public void RectMinus_LeavesExactlyTheUncoveredArea()
        {
            var cells = HousePlan.RectMinus(new[] { 0f, 0f, 4f, 3f }, new List<float[]> { new[] { 1f, 1f, 2f, 2.5f } });
            float area = cells.Sum(c => (c[2] - c[0]) * (c[3] - c[1]));
            Assert.That(area, Is.EqualTo(12f - 1.5f).Within(1e-4f));
            Assert.That(cells.Any(c => c[0] < 1.5f && c[2] > 1.5f && c[1] < 1.5f && c[3] > 1.5f), Is.False, "the hole stays open");
            var gaps = HousePlan.Subtract(0f, 10f, new[] { (2f, 3f), (2.5f, 4f), (8f, 12f) });
            Assert.That(gaps, Is.EqualTo(new List<(float, float)> { (0f, 2f), (4f, 8f) }));
        }
    }
}
