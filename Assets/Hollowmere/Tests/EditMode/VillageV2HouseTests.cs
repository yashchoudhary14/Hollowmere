using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hollowmere.EditorTools;
using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace Hollowmere.Tests
{
    /// <summary>
    /// M1.6 P3: the house shells VillageBuilderV2 puts in Village.unity. Each house has its rooms in Hxx.json order, a
    /// front door and one hearth room; with every door open, every room is reachable on the NavMesh from outside the
    /// front door (stairs walked, ladders climbed); each ladder has a climb at both ends and a NavMeshLink for the Hollow.
    /// Skipped until the scene is built.
    /// </summary>
    public class VillageV2HouseTests
    {
        HouseSpec[] _specs;
        HousePlan[] _plans;
        NavMeshObstacle[] _obstacles;

        [OneTimeSetUp]
        public void OpenScene()
        {
            if (!File.Exists(VillageBuilderV2.ScenePath)) Assert.Ignore("Village v2 scene not built yet (run VillageBuilderV2.Build).");
            EditorSceneManager.OpenScene(VillageBuilderV2.ScenePath, OpenSceneMode.Single);
            var village = DesignData.LoadVillage();
            var catalog = DesignData.LoadCatalog();
            _specs = DesignData.LoadHouses();
            _plans = _specs.Select(h => HousePlan.Make(h, village.FindHouse(h.Slug), catalog)).ToArray();
            // Every door open: shut doors carve the NavMesh as soon as it loads, so turn them off and reload it.
            _obstacles = Object.FindObjectsByType<NavMeshObstacle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var o in _obstacles) o.enabled = false;
            var surface = Object.FindAnyObjectByType<NavMeshSurface>();
            surface.RemoveData();
            surface.AddData();
        }

        [OneTimeTearDown]
        public void DoorsShut()
        {
            if (_obstacles == null) return;
            foreach (var o in _obstacles)
                if (o != null) o.enabled = true;
        }

        static House Find(HouseSpec h)
        {
            var go = GameObject.Find($"Village/Houses/{h.Slug} {h.Name}");
            Assert.That(go, Is.Not.Null, h.Slug);
            return go.GetComponent<House>();
        }

        static Vector3 Probe(Room room)
        {
            var probe = room.transform.Find("Probe");
            Assert.That(probe, Is.Not.Null, $"{room.House.name} {room.Id} has no Probe");
            return probe.position;
        }

        /// <summary>The NavMesh under a floor point: within 0.5 m across and 0.3 m up or down, so never the floor above or below.</summary>
        static bool OnFloor(Vector3 p, out Vector3 onMesh)
        {
            onMesh = p;
            if (!NavMesh.SamplePosition(p, out var hit, 0.5f, NavMesh.AllAreas) || Mathf.Abs(hit.position.y - p.y) > 0.3f) return false;
            onMesh = hit.position;
            return true;
        }

        static NavMeshPath PathBetween(Vector3 a, Vector3 b)
        {
            var path = new NavMeshPath();
            NavMesh.CalculatePath(a, b, NavMesh.AllAreas, path);
            return path;
        }

        static bool Reaches(IEnumerable<Vector3> from, Vector3 to) =>
            from.Any(a => PathBetween(a, to).status == NavMeshPathStatus.PathComplete);

        static float Length(NavMeshPath path)
        {
            float d = 0f;
            for (int i = 1; i < path.corners.Length; i++) d += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            return d;
        }

        static bool IsBay(HouseSpec h, string roomId) => (h.FindRoom(roomId).Kind ?? "").EndsWith("-bay");

        [Test]
        public void EveryHouse_HasItsRooms_AFrontDoor_AndOneHearthRoom()
        {
            foreach (var h in _specs)
            {
                var house = Find(h);
                Assert.That(house.Index, Is.EqualTo(h.Index), h.Slug);
                Assert.That(house.Rooms.Select(r => r.Id), Is.EqualTo(h.Rooms.Select(r => r.Id)), h.Slug);
                Assert.That(house.Door, Is.Not.Null, $"{h.Slug}: no front door leaf");
                Assert.That(house.Front, Is.Not.Null, $"{h.Slug}: no FrontDoor marker");
                Assert.That(house.Rooms.Count(r => r.IsHearthRoom), Is.EqualTo(1), $"{h.Slug}: hearth rooms");
                Assert.That(house.HearthRoom, Is.Not.Null, h.Slug);
                foreach (var room in house.Rooms)
                {
                    var feet = Probe(room) + Vector3.up * 0.5f;
                    Assert.That(house.Contains(feet), Is.True, $"{h.Slug} {room.Id}: probe outside the house");
                    // An open bay's probe may stand in the bay's mouth, in the room it opens off (HousePlan.ProbeFor).
                    if (!IsBay(h, room.Id)) Assert.That(room.Contains(feet), Is.True, $"{h.Slug} {room.Id}: probe outside its room");
                }
            }
        }

        [Test]
        public void EveryRoom_IsReachable_FromOutsideTheFrontDoor_WithTheDoorsOpen()
        {
            var failures = new List<string>();
            for (int i = 0; i < _specs.Length; i++)
            {
                var h = _specs[i];
                var house = Find(h);
                Assert.That(NavMesh.SamplePosition(house.Front.position + house.Front.forward * 1.2f, out var street, 2f, NavMesh.AllAreas), Is.True,
                            $"{h.Slug}: no NavMesh outside the front door");
                // Players climb ladders (Ladder teleports them), so reaching one end of a ladder reaches the other.
                var reached = new List<Vector3> { street.position };
                var ladders = new List<(Vector3 foot, Vector3 head)>();
                foreach (var f in _plans[i].Flights.Where(f => f.Ladder))
                {
                    if (!OnFloor(house.transform.TransformPoint(f.Bottom), out var foot)) { failures.Add($"{h.Slug} {f.Id}: no floor at the ladder's foot"); continue; }
                    if (!OnFloor(house.transform.TransformPoint(f.Top), out var head)) { failures.Add($"{h.Slug} {f.Id}: no floor at the ladder's head"); continue; }
                    ladders.Add((foot, head));
                }
                for (bool grew = true; grew;)
                {
                    grew = false;
                    foreach (var (foot, head) in ladders)
                    {
                        if (!reached.Contains(head) && Reaches(reached, foot)) { reached.Add(head); grew = true; }
                        if (!reached.Contains(foot) && Reaches(reached, head)) { reached.Add(foot); grew = true; }
                    }
                }
                foreach (var room in house.Rooms)
                {
                    var probe = Probe(room);
                    if (!OnFloor(probe, out var onMesh)) failures.Add($"{h.Slug} {room.Id}: no NavMesh at its probe {probe}");
                    else if (!Reaches(reached, onMesh)) failures.Add($"{h.Slug} {room.Id}: unreachable from the front door");
                }
            }
            Assert.That(failures, Is.Empty, string.Join("\n", failures));
        }

        [Test]
        public void EveryStair_IsWalked_UpAndDown()
        {
            var failures = new List<string>();
            for (int i = 0; i < _specs.Length; i++)
            {
                var h = _specs[i];
                var house = Find(h);
                foreach (var f in _plans[i].Flights.Where(f => !f.Ladder))
                {
                    if (!OnFloor(house.transform.TransformPoint(f.Bottom), out var foot)) { failures.Add($"{h.Slug} {f.Id}: no floor at its foot"); continue; }
                    if (!OnFloor(house.transform.TransformPoint(f.Top), out var head)) { failures.Add($"{h.Slug} {f.Id}: no floor at its head"); continue; }
                    // Up and down this flight, not round by another one: no longer than the flight and a few steps.
                    float direct = Vector3.Distance(foot, head) * 1.5f + 2f;
                    foreach (var (a, b, way) in new[] { (foot, head, "up"), (head, foot, "down") })
                    {
                        var path = PathBetween(a, b);
                        if (path.status != NavMeshPathStatus.PathComplete) failures.Add($"{h.Slug} {f.Id}: can't walk {way} it");
                        else if (Length(path) > direct) failures.Add($"{h.Slug} {f.Id}: the way {way} goes round ({Length(path):0.0} m)");
                    }
                }
            }
            Assert.That(failures, Is.Empty, string.Join("\n", failures));
        }

        [Test]
        public void EveryLadder_HasAClimbAtEachEnd_AndALinkForTheHollow()
        {
            for (int i = 0; i < _specs.Length; i++)
            {
                var h = _specs[i];
                var house = Find(h);
                foreach (var f in _plans[i].Flights.Where(f => f.Ladder))
                {
                    string at = $"{h.Slug} {f.Id}";
                    var flight = house.transform.Find($"Stairs/{f.Id} {f.Lower}-{f.Upper}");
                    Assert.That(flight, Is.Not.Null, at);
                    var climbs = flight.GetComponentsInChildren<Ladder>();
                    Assert.That(climbs.Length, Is.EqualTo(2), at);
                    var up = climbs.Single(l => l.Up);
                    var down = climbs.Single(l => !l.Up);
                    Assert.That(up.Exit.name, Is.EqualTo($"LadderHead_{f.Id}"), at);
                    Assert.That(down.Exit.name, Is.EqualTo($"LadderFoot_{f.Id}"), at);
                    Assert.That(Vector3.Distance(up.Exit.position, house.transform.TransformPoint(f.Top)), Is.LessThan(0.01f), at);
                    Assert.That(Vector3.Distance(down.Exit.position, house.transform.TransformPoint(f.Bottom)), Is.LessThan(0.01f), at);
                    Assert.That(climbs.All(l => l.GetComponent<BoxCollider>().isTrigger), Is.True, $"{at}: a climb blocks the way");
                    Assert.That(OnFloor(up.Exit.position, out _) && OnFloor(down.Exit.position, out _), Is.True, $"{at}: an exit is off the NavMesh");

                    var link = house.transform.Find($"NavLink_{f.Id}")?.GetComponent<NavMeshLink>();
                    Assert.That(link, Is.Not.Null, at);
                    Assert.That(Vector3.Distance(link.startPoint, f.Bottom), Is.LessThan(0.01f), at);
                    Assert.That(Vector3.Distance(link.endPoint, f.Top), Is.LessThan(0.01f), at);
                    Assert.That(link.bidirectional, Is.True, at);
                }
            }
        }
    }
}
