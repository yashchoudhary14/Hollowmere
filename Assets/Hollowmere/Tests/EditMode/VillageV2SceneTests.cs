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
    /// M1.6 P2: the village v2 scene VillageBuilderV2 generated. Every front door reaches the square on the NavMesh, with
    /// no night blocker and with each one on its own (one per match, DESIGN.md "How the village changes at night");
    /// distances to the altar match village.json; the M1 landmarks haven't moved. Skipped until the scene is built.
    /// </summary>
    public class VillageV2SceneTests
    {
        VillageSpec _village;
        HouseSpec[] _houses;
        VillageLayout _layout;
        NavMeshSurface _surface;
        NavMeshObstacle[] _obstacles;
        GameObject[] _blockers;

        [OneTimeSetUp]
        public void OpenScene()
        {
            if (!File.Exists(VillageBuilderV2.ScenePath)) Assert.Ignore("Village v2 scene not built yet (run VillageBuilderV2.Build).");
            EditorSceneManager.OpenScene(VillageBuilderV2.ScenePath, OpenSceneMode.Single);
            _village = DesignData.LoadVillage();
            _houses = DesignData.LoadHouses();
            _layout = Object.FindAnyObjectByType<VillageLayout>();
            _surface = Object.FindAnyObjectByType<NavMeshSurface>();
            _obstacles = Object.FindObjectsByType<NavMeshObstacle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var root = GameObject.Find("Village/NightBlockers").transform;
            _blockers = Enumerable.Range(0, root.childCount).Select(i => root.GetChild(i).gameObject).ToArray();
        }

        [TearDown]
        public void AllBlockersOff()
        {
            foreach (var b in _blockers) b.SetActive(false);
        }

        /// <summary>Gate leaves stay shut; only the given blocker is on. Reloads the NavMesh so carving is reapplied.</summary>
        void OnlyBlocker(GameObject on)
        {
            foreach (var b in _blockers) b.SetActive(b == on);
            foreach (var o in _obstacles) o.enabled = true;
            _surface.RemoveData();
            _surface.AddData();
        }

        Transform FrontDoor(HouseSpec h) => GameObject.Find($"Village/Houses/{h.Slug} {h.Name}/FrontDoor").transform;

        [Test]
        public void Scene_HasEveryHouseAtItsPlace_AndTheV2Furniture()
        {
            foreach (var h in _houses)
            {
                var place = _village.FindHouse(h.Slug);
                var house = GameObject.Find($"Village/Houses/{h.Slug} {h.Name}");
                Assert.That(house, Is.Not.Null, h.Slug);
                Assert.That(Vector3.Distance(house.transform.position, new Vector3(place.Position[0], place.Position[1], place.Position[2])), Is.LessThan(0.01f), h.Slug);
                Assert.That(Mathf.Abs(Mathf.DeltaAngle(house.transform.eulerAngles.y, place.Yaw)), Is.LessThan(0.1f), h.Slug);
            }
            Assert.That(GameObject.Find("Village/Filler").transform.childCount, Is.EqualTo(_village.Filler.Length));
            Assert.That(GameObject.Find("Village/StreetLamps").transform.childCount, Is.EqualTo(_village.StreetLamps.Length));
            Assert.That(GameObject.Find("Village/Scarecrows").transform.childCount, Is.EqualTo(6));
            Assert.That(_blockers.Length, Is.EqualTo(6));
            Assert.That(_blockers.All(b => !b.activeSelf), "Night blockers are off by day; P8 switches on the one MatchPlan picks.");
        }

        [Test]
        public void M1Landmarks_StayWhereTheyWere()
        {
            Assert.That(GameObject.Find("Village/Gate").transform.position, Is.EqualTo(new Vector3(0f, 0f, 150f)));
            Assert.That(GameObject.Find("Village/Square/Altar").transform.position, Is.EqualTo(new Vector3(0f, 0f, 112f)));
            Assert.That(GameObject.Find("Village/Square/BellTower").transform.position, Is.EqualTo(new Vector3(26f, 0f, 132f)));
            Assert.That(GameObject.Find("Village/Chapel").transform.position, Is.EqualTo(new Vector3(-60f, 0f, 112f)));
            Assert.That(_layout.SquareCenter.position, Is.EqualTo(new Vector3(0f, 0f, 112f)));
            Assert.That(_layout.GateLineZ, Is.EqualTo(150f));
        }

        [Test]
        public void FrontDoorDistances_ToTheAltar_MatchVillageJson()
        {
            var altar = new Vector2(_layout.SquareCenter.position.x, _layout.SquareCenter.position.z);
            foreach (var h in _houses)
            {
                var door = FrontDoor(h).position;
                float d = Vector2.Distance(new Vector2(door.x, door.z), altar);
                Assert.That(d, Is.EqualTo(_village.FindHouse(h.Slug).ToAltar).Within(2f), h.Slug);
            }
        }

        [Test]
        public void EveryFrontDoor_ReachesTheSquare()
        {
            OnlyBlocker(null);
            AssertEveryHouseReachesTheSquare("no blocker");
        }

        [Test]
        public void EachNightBlocker_ClosesItsLane_AndLeavesEveryHouseARoute()
        {
            foreach (var b in _blockers)
            {
                OnlyBlocker(b);
                Assert.That(NavMesh.SamplePosition(b.transform.position, out _, 0.3f, NavMesh.AllAreas), Is.False, $"{b.name} doesn't carve the lane");
                AssertEveryHouseReachesTheSquare(b.name);
            }
        }

        void AssertEveryHouseReachesTheSquare(string label)
        {
            var square = OnNavMesh(_layout.SquareCenter.position + new Vector3(0f, 0f, -10f));
            foreach (var h in _houses)
            {
                var door = FrontDoor(h);
                var from = OnNavMesh(door.position + door.forward * 1.2f);
                var path = new NavMeshPath();
                NavMesh.CalculatePath(from, square, NavMesh.AllAreas, path);
                Assert.That(path.status, Is.EqualTo(NavMeshPathStatus.PathComplete), $"{h.Slug} ({label})");
            }
        }

        [Test]
        public void RoamAndSpawnPoints_AreOnTheNavMesh()
        {
            OnlyBlocker(null);
            var points = _layout.RoamPoints.Concat(_layout.ForestSpawns).Append(_layout.HollowNightSpawn).Append(_layout.PlayerSpawn);
            foreach (var p in points)
                Assert.That(NavMesh.SamplePosition(p.position, out _, 1.5f, NavMesh.AllAreas), Is.True, p.name);
            Assert.That(_layout.ForestSpawns.Length, Is.EqualTo(8));
        }

        static Vector3 OnNavMesh(Vector3 p)
        {
            Assert.That(NavMesh.SamplePosition(p, out var hit, 2f, NavMesh.AllAreas), Is.True, $"No NavMesh near {p}");
            return hit.position;
        }
    }
}
