using System.IO;
using System.Linq;
using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace Hollowmere.Tests
{
    /// <summary>
    /// Checks the scene GreyboxVillageBuilder generated. Skipped if it hasn't been built yet.
    /// Closed doors carve the NavMesh even in edit mode, so path tests set the carving they need first.
    /// </summary>
    public class GreyboxSceneTests
    {
        const string ScenePath = "Assets/Hollowmere/Scenes/Greybox.unity";
        House[] _houses;
        VillageLayout _layout;
        NavMeshSurface _surface;
        NavMeshObstacle[] _obstacles;

        [OneTimeSetUp]
        public void OpenScene()
        {
            if (!File.Exists(ScenePath)) Assert.Ignore("Greybox scene not built yet (run GreyboxVillageBuilder.Build).");
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            _houses = Object.FindObjectsByType<House>(FindObjectsSortMode.None).OrderBy(h => h.Index).ToArray();
            _layout = Object.FindAnyObjectByType<VillageLayout>();
            _surface = Object.FindAnyObjectByType<NavMeshSurface>();
            _obstacles = Object.FindObjectsByType<NavMeshObstacle>(FindObjectsSortMode.None);
        }

        /// <summary>Turns the door and gate obstacles on or off, then reloads the NavMesh so the carving is reapplied.</summary>
        void SetDoorCarving(bool on)
        {
            foreach (var o in _obstacles) o.enabled = on;
            _surface.RemoveData();
            _surface.AddData();
        }

        [Test]
        public void ClosedFrontDoors_KeepTheNavMeshOutOfTheHouses()
        {
            SetDoorCarving(true);
            var from = OnNavMesh(_layout.SquareCenter.position + new Vector3(0f, 0f, -10f));
            foreach (var h in _houses)
            {
                var hall = h.Rooms[0];
                var to = OnNavMesh(hall.Bounds.center - Vector3.up * hall.Bounds.extents.y);
                var path = new NavMeshPath();
                NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);
                Assert.That(path.status, Is.Not.EqualTo(NavMeshPathStatus.PathComplete), h.name);
            }
        }

        [Test]
        public void Scene_HasEveryHouseOnce_WithItsParts()
        {
            Assert.That(_houses.Select(h => h.Index), Is.EqualTo(Enumerable.Range(0, HouseCatalog.Count)));
            foreach (var h in _houses)
            {
                Assert.That(h.Rooms.Length, Is.EqualTo(3), h.name);
                Assert.That(h.HearthRoom, Is.Not.Null, h.name);
                Assert.That(h.Notes.Length, Is.EqualTo(3), h.name);
                Assert.That(h.Door, Is.Not.Null, h.name);
                Assert.That(h.Hearth, Is.Not.Null, h.name);
                Assert.That(h.FuseBox, Is.Not.Null, h.name);
                Assert.That(h.Wardrobe, Is.Not.Null, h.name);
                Assert.That(h.ItemSpots.Length, Is.GreaterThanOrEqualTo(3), h.name);
                Assert.That(h.ShrineMarker, Is.Not.Null, h.name);
                Assert.That(h.ShrineMarker.activeSelf, Is.False, "Shrine markers are hidden until the match plan says otherwise.");
                Assert.That(h.Contains(h.HearthRoom.Bounds.center), Is.True, h.name);
            }
        }

        [Test]
        public void EveryHouseInterior_IsReachableFromTheSquare_ThroughOpenDoors()
        {
            SetDoorCarving(false);
            var from = OnNavMesh(_layout.SquareCenter.position + new Vector3(0f, 0f, -10f));
            foreach (var h in _houses)
            {
                foreach (var room in h.Rooms)
                {
                    var to = OnNavMesh(room.Bounds.center - Vector3.up * room.Bounds.extents.y);
                    var path = new NavMeshPath();
                    NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);
                    Assert.That(path.status, Is.EqualTo(NavMeshPathStatus.PathComplete), $"{h.name} / {room.name}");
                }
            }
        }

        [Test]
        public void RoamAndSpawnPoints_AreOnTheNavMesh()
        {
            var points = _layout.RoamPoints.Concat(_layout.ForestSpawns).Append(_layout.HollowNightSpawn).Append(_layout.PlayerSpawn);
            foreach (var p in points)
                Assert.That(NavMesh.SamplePosition(p.position, out _, 1.5f, NavMesh.AllAreas), Is.True, p.name);
        }

        static Vector3 OnNavMesh(Vector3 p)
        {
            Assert.That(NavMesh.SamplePosition(p, out var hit, 2f, NavMesh.AllAreas), Is.True, $"No NavMesh near {p}");
            return hit.position;
        }
    }
}
