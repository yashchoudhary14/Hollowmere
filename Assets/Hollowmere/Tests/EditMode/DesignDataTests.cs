using System.IO;
using System.Linq;
using Hollowmere.EditorTools;
using NUnit.Framework;

namespace Hollowmere.Tests
{
    /// <summary>
    /// M1.6 P1: the design pack in docs/design/ loads into DesignSpecs and passes the C# twin of tools/housegen/run_check.py
    /// (DesignChecks). The houses keep HouseCatalog's identity, so letters, MatchPlan and the M1 tests keep working.
    /// </summary>
    public class DesignDataTests
    {
        HouseSpec[] _houses;
        VillageSpec _village;
        AssetCatalogSpec _catalog;

        [OneTimeSetUp]
        public void Load()
        {
            _houses = DesignData.LoadHouses();
            _village = DesignData.LoadVillage();
            _catalog = DesignData.LoadCatalog();
        }

        static void AssertNone(System.Collections.Generic.List<string> problems) =>
            Assert.That(problems, Is.Empty, string.Join("\n", problems));

        [Test]
        public void AllFifteenHouses_Load()
        {
            Assert.That(_houses.Length, Is.EqualTo(HouseCatalog.Count));
            for (int i = 0; i < _houses.Length; i++)
            {
                var h = _houses[i];
                Assert.That(h.Schema, Is.EqualTo("hollowmere.house/1"), h.Slug);
                Assert.That(h.Index, Is.EqualTo(i), h.Slug);
                Assert.That(h.Slug, Is.EqualTo(DesignData.Slug(i)));
                Assert.That(h.Levels.ContainsKey("G"), h.Slug);
                Assert.That(h.Wings, Is.Not.Empty, h.Slug);
                Assert.That(h.Rooms, Is.Not.Empty, h.Slug);
                Assert.That(h.Props, Is.Not.Empty, h.Slug);
                Assert.That(h.Rooms.Select(r => r.Id), Is.Unique, h.Slug);
                Assert.That(h.Props.Select(p => p.Id), Is.Unique, h.Slug);
                Assert.That(h.Rooms.All(r => r.Rect.Length == 4 && r.Inner.Length == 4), h.Slug);
                Assert.That(h.Props.All(p => p.Dims.Length == 3), h.Slug);
            }
            Assert.That(_village.Schema, Is.EqualTo("hollowmere.village/1"));
            Assert.That(_catalog.Models, Is.Not.Empty);
            Assert.That(_catalog.Materials, Is.Not.Empty);
        }

        [Test]
        public void Houses_MatchHouseCatalog()
        {
            foreach (var h in _houses) AssertNone(DesignChecks.Identity(h));
        }

        [Test]
        public void EveryRoom_LiesInsideAWing_AndRoomsDontOverlap()
        {
            foreach (var h in _houses) AssertNone(DesignChecks.Structure(h));
        }

        [Test]
        public void Doors_JoinTwoRoomsOnOneLevel_OrARoomAndOutside()
        {
            foreach (var h in _houses) AssertNone(DesignChecks.Openings(h));
        }

        [Test]
        public void EveryAssetKey_IsInTheCatalog()
        {
            foreach (var h in _houses) AssertNone(DesignChecks.AssetKeys(h, _catalog));
        }

        [Test]
        public void EveryHouse_MeetsTheSharedTemplate()
        {
            // Front + back door, 2 weak windows, 4-6 hiding spots, hearth, fuse box, phone, mirror, family photo, radio/TV.
            foreach (var h in _houses) AssertNone(DesignChecks.Requirements(h, _catalog));
        }

        [Test]
        public void Village_PlacesEveryHouse_AndFootprintsDontOverlap()
        {
            Assert.That(_village.Houses.Select(h => h.Slug), Is.EquivalentTo(_houses.Select(h => h.Slug)));
            AssertNone(DesignChecks.Village(_village, _houses, _catalog));
        }

        [Test]
        public void Loader_MapsSnakeCase_AndRejectsUnknownKeys()
        {
            var d = DesignData.Parse<DoorSpec>("{\"id\":\"d1\",\"room_a\":\"hall\",\"room_b\":\"ext\",\"at\":1.5,\"colour\":null}");
            Assert.That(d.RoomA, Is.EqualTo("hall"));
            Assert.That(d.RoomB, Is.EqualTo("ext"));
            Assert.That(d.At, Is.EqualTo(1.5f));
            var h = DesignData.Parse<HouseSpec>("{\"puzzle_kind_m1\":\"Clock\",\"chimneys_detail\":[{\"above_ridge\":1.0}]}");
            Assert.That(h.PuzzleKindM1, Is.EqualTo("Clock"));
            Assert.That(h.ChimneysDetail[0].AboveRidge, Is.EqualTo(1f));
            Assert.Throws<InvalidDataException>(() => DesignData.Parse<LevelSpec>("{\"y\":0,\"bogus\":1}"));
        }

        [Test]
        public void Checks_CatchBrokenData()
        {
            var h = DesignData.LoadHouse(0);
            h.Rooms[0].Rect[0] -= 5f;
            Assert.That(DesignChecks.Structure(h).Any(p => p.Contains("not inside any wing")), "room outside its wing");

            h = DesignData.LoadHouse(0);
            var front = h.Doors.First(d => d.Role == "front");
            front.RoomB = h.Rooms.First(r => r.Id != front.RoomA && !DesignChecks.Neighbours(h, h.FindRoom(front.RoomA), front.Side).Contains(r)).Id;
            Assert.That(DesignChecks.Openings(h), Is.Not.Empty, "door to a room that isn't across the wall");

            h = DesignData.LoadHouse(0);
            foreach (var w in h.Windows) w.Weak = false;
            Assert.That(DesignChecks.Requirements(h, _catalog).Any(p => p.Contains("weak windows")), "weak window count");

            h = DesignData.LoadHouse(0);
            h.Props[0].Key = "NoSuchAsset";
            Assert.That(DesignChecks.AssetKeys(h, _catalog), Is.Not.Empty, "unknown asset key");

            var v = DesignData.LoadVillage();
            v.Filler[0].Pos = new[] { v.Houses[0].Position[0], v.Houses[0].Position[2] };
            Assert.That(DesignChecks.Village(v, _houses, _catalog).Any(p => p.Contains("too close")), "filler on top of a house");
        }

        [Test]
        public void Rotate_MatchesUnityYRotation()
        {
            foreach (var yaw in new[] { 0f, 90f, 180f, 270f, 33f })
            {
                var q = UnityEngine.Quaternion.Euler(0f, yaw, 0f) * new UnityEngine.Vector3(1.5f, 0f, -2f);
                var r = DesignChecks.Rotate(1.5f, -2f, yaw);
                Assert.That(r.x, Is.EqualTo(q.x).Within(1e-4f), $"yaw {yaw}");
                Assert.That(r.y, Is.EqualTo(q.z).Within(1e-4f), $"yaw {yaw}");
            }
        }
    }
}
