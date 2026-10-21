using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Hollowmere.Tests
{
    /// <summary>Rules from docs/DESIGN.md: "Houses and puzzles", "Replayability", "Decisions".</summary>
    public class MatchSetupTests
    {
        [Test]
        public void Catalog_HasFifteenHouses_WithUniqueDoorAndChimneyPairs()
        {
            Assert.That(HouseCatalog.Houses.Length, Is.EqualTo(HouseCatalog.Count));
            Assert.That(HouseCatalog.Count, Is.EqualTo(15));

            var pairs = HouseCatalog.Houses.Select(h => (h.Door, h.Chimneys)).ToList();
            Assert.That(pairs, Is.Unique, "Each (door colour, chimney count) pair must point at exactly one house.");
            Assert.That(HouseCatalog.Houses.Select(h => h.Name), Is.Unique);
            Assert.That(HouseCatalog.Houses.Select(h => h.Part), Is.Unique);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void EachPlayer_GetsOneUniqueHouse(int players)
        {
            var plan = MatchSetup.Generate(1234, players, null);
            Assert.That(plan.AllottedHouses.Length, Is.EqualTo(players));
            Assert.That(plan.AllottedHouses, Is.Unique);
            Assert.That(plan.AllottedHouses.All(h => h >= 0 && h < HouseCatalog.Count));
        }

        [TestCase(1)]
        [TestCase(4)]
        [TestCase(6)]
        public void AltarOrder_IsAPermutationOfTheAllottedHouses(int players)
        {
            var plan = MatchSetup.Generate(99, players, null);
            CollectionAssert.AreEquivalent(plan.AllottedHouses, plan.AltarOrder);
            foreach (var house in plan.AllottedHouses)
                Assert.That(plan.SlotFor(house), Is.GreaterThanOrEqualTo(0));
        }

        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(6, 4)]
        public void ShrineCount_IsThreeForSmallGroups_FourForLarger(int players, int expectedShrines)
        {
            var plan = MatchSetup.Generate(7, players, null);
            Assert.That(plan.ShrineHouses.Length, Is.EqualTo(expectedShrines));
            Assert.That(plan.ShrineHouses, Is.Unique);
        }

        [Test]
        public void SameSeed_GivesTheSamePlan()
        {
            var a = MatchSetup.Generate(555, 4, null);
            var b = MatchSetup.Generate(555, 4, null);
            CollectionAssert.AreEqual(a.AllottedHouses, b.AllottedHouses);
            CollectionAssert.AreEqual(a.AltarOrder, b.AltarOrder);
            CollectionAssert.AreEqual(a.ShrineHouses, b.ShrineHouses);
            Assert.That(a.Variant, Is.EqualTo(b.Variant));
            for (int i = 0; i < HouseCatalog.Count; i++)
            {
                CollectionAssert.AreEqual(a.Solutions[i].Code, b.Solutions[i].Code);
                CollectionAssert.AreEqual(a.Solutions[i].Sequence, b.Solutions[i].Sequence);
                Assert.That(a.Solutions[i].ClockHour, Is.EqualTo(b.Solutions[i].ClockHour));
            }
            CollectionAssert.AreEqual(a.Letters, b.Letters);
        }

        [Test]
        public void DifferentSeeds_ChangeTheAllottedHouses()
        {
            var seen = new HashSet<string>();
            for (int seed = 0; seed < 20; seed++)
                seen.Add(string.Join(",", MatchSetup.Generate(seed, 4, null).AllottedHouses));
            Assert.That(seen.Count, Is.GreaterThan(10), "Allotments should vary between matches.");
        }

        [Test]
        public void Solutions_AreInRange()
        {
            for (int seed = 0; seed < 50; seed++)
            {
                var plan = MatchSetup.Generate(seed, 6, null);
                foreach (var s in plan.Solutions)
                {
                    Assert.That(s.Code.Length, Is.EqualTo(4));
                    Assert.That(s.Code.All(d => d >= 0 && d <= 9));
                    Assert.That(s.ClockHour, Is.InRange(1, 11), "Never 12: the clock dial starts at 12.");
                    Assert.That(s.Sequence.Length, Is.EqualTo(4));
                    Assert.That(s.Sequence, Is.Unique);
                    Assert.That(s.Sequence.All(i => i >= 0 && i < HouseCatalog.SequenceSymbols.Length));
                }
            }
        }

        [Test]
        public void Letters_AddressThePlayer_AndGiveThreeTraitsOfTheirHouse()
        {
            var names = new[] { "Asha", "Ben", "Chen" };
            var plan = MatchSetup.Generate(42, 3, names);
            for (int p = 0; p < 3; p++)
            {
                var letter = plan.Letters[p];
                var def = HouseCatalog.Houses[plan.AllottedHouses[p]];
                StringAssert.Contains(names[p], letter);

                int traits = 0;
                if (letter.Contains(HouseCatalog.DoorRiddle(def.Door))) traits++;
                if (letter.Contains("chimney")) traits++;
                if (letter.Contains(HouseCatalog.NumberWord(def.ClockHour) + " o'clock")) traits++;
                if (letter.Contains("The " + def.Family + " family")) traits++;
                Assert.That(traits, Is.EqualTo(3), letter);
            }
        }

        [Test]
        public void PlayerCount_IsClampedToOneThroughSix()
        {
            Assert.That(MatchSetup.Generate(1, 0, null).PlayerCount, Is.EqualTo(1));
            Assert.That(MatchSetup.Generate(1, 9, null).PlayerCount, Is.EqualTo(6));
        }

        [Test]
        public void ForcedVariant_IsUsed()
        {
            for (int v = 0; v < 4; v++)
                Assert.That((int)MatchSetup.Generate(3, 2, null, v).Variant, Is.EqualTo(v));
        }

        [Test]
        public void RitualBook_ListsEveryPartInAltarOrder()
        {
            var plan = MatchSetup.Generate(8, 4, null);
            var book = plan.RitualBookText();
            int last = -1;
            foreach (var house in plan.AltarOrder)
            {
                var part = HouseCatalog.Houses[house].Part.ToLowerInvariant();
                int at = book.IndexOf(part, System.StringComparison.Ordinal);
                Assert.That(at, Is.GreaterThan(last), $"'{part}' missing or out of order");
                last = at;
            }
        }
    }
}
