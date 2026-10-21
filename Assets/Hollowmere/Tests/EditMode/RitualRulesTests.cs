using NUnit.Framework;
using UnityEngine;

namespace Hollowmere.Tests
{
    /// <summary>docs/DESIGN.md "The Ritual of Passage" and "The Crossing".</summary>
    public class RitualRulesTests
    {
        static int[] Empty() => new[] { -1, -1, -1, -1, -1, -1 };

        [Test]
        public void Altar_TakesEachPart_OnlyInItsOwnSlot()
        {
            var plan = MatchSetup.Generate(5, 4, null);
            var placed = Empty();
            for (int slot = 0; slot < 4; slot++)
            {
                int house = plan.AltarOrder[slot];
                for (int other = 0; other < 4; other++)
                    if (other != slot && placed[other] < 0)
                        Assert.That(AltarRules.Place(plan, placed, other, house), Is.EqualTo(PlaceResult.WrongSlot));
                Assert.That(AltarRules.AllPlaced(plan, placed), Is.False);
                Assert.That(AltarRules.Place(plan, placed, slot, house), Is.EqualTo(PlaceResult.Accepted));
                placed[slot] = house;
            }
            Assert.That(AltarRules.AllPlaced(plan, placed), Is.True);
        }

        [Test]
        public void Altar_FilledAndUnusedSlots_TakeNothing()
        {
            var plan = MatchSetup.Generate(9, 2, null);
            var placed = Empty();
            placed[0] = plan.AltarOrder[0];
            Assert.That(AltarRules.Place(plan, placed, 0, plan.AltarOrder[0]), Is.EqualTo(PlaceResult.SlotTaken));
            Assert.That(AltarRules.Place(plan, placed, 2, plan.AltarOrder[1]), Is.EqualTo(PlaceResult.SlotUnused), "Two players use slots I and II only.");
        }

        [TestCase(true, 2, 2, ExpectedResult = true)]
        [TestCase(false, 2, 2, ExpectedResult = false)]
        [TestCase(true, 2, 1, ExpectedResult = false)]
        [TestCase(true, 0, 0, ExpectedResult = false)]
        public bool Chant_StartsWithEveryPart_AndEveryLivingPlayerInside(bool allPlaced, int living, int inside)
            => ChantState.CanStart(allPlaced, living, inside);

        [Test]
        public void Chant_Completes_After60SecondsOfChanting()
        {
            var chant = new ChantState();
            Assert.That(chant.Tick(1f, true, 4), Is.EqualTo(ChantStatus.Waiting));
            chant.Begin();
            for (int i = 0; i < 59; i++) Assert.That(chant.Tick(1f, true, 4), Is.EqualTo(ChantStatus.Chanting));
            Assert.That(chant.Tick(1f, true, 4), Is.EqualTo(ChantStatus.Complete));
            Assert.That(chant.Complete, Is.True);
        }

        [Test]
        public void Chant_Pauses_WhileAnyoneLivingIsOutside()
        {
            var chant = new ChantState();
            chant.Begin();
            chant.Tick(10f, true, 3);
            Assert.That(chant.Tick(5f, false, 3), Is.EqualTo(ChantStatus.Paused));
            Assert.That(chant.Progress, Is.EqualTo(10f));
        }

        [Test]
        public void Chant_StartsAgainFromZero_WhenEveryCandleStaysOutFor3Seconds()
        {
            var chant = new ChantState();
            chant.Begin();
            chant.Tick(40f, true, 2);
            Assert.That(chant.Tick(1f, true, 0), Is.EqualTo(ChantStatus.Guttering));
            Assert.That(chant.Tick(1f, true, 0), Is.EqualTo(ChantStatus.Guttering));
            Assert.That(chant.Progress, Is.EqualTo(40f), "No progress while guttering, but nothing lost yet.");
            Assert.That(chant.Tick(1f, true, 0), Is.EqualTo(ChantStatus.Reset));
            Assert.That(chant.Progress, Is.EqualTo(0f));
            Assert.That(chant.Tick(1f, true, 1), Is.EqualTo(ChantStatus.Chanting), "Relighting one candle starts it again.");
        }

        [Test]
        public void Chant_AQuickRelight_SavesTheProgress()
        {
            var chant = new ChantState();
            chant.Begin();
            chant.Tick(30f, true, 1);
            chant.Tick(2.5f, true, 0);
            Assert.That(chant.Tick(1f, true, 1), Is.EqualTo(ChantStatus.Chanting));
            Assert.That(chant.Progress, Is.EqualTo(31f));
            chant.Tick(2.5f, true, 0);
            Assert.That(chant.Tick(1f, true, 0), Is.EqualTo(ChantStatus.Reset), "The grace starts over after each relight, then runs out.");
        }

        [Test]
        public void LookingBack_PastOneTwentyDegrees_ForHalfASecond_MeansTaken()
        {
            float back = 0f;
            for (int i = 0; i < 4; i++) back = CrossingRules.UpdateLookBack(back, 150f, 0.1f);
            Assert.That(CrossingRules.IsTaken(back), Is.False, "A glance under half a second is survivable.");
            back = CrossingRules.UpdateLookBack(back, 150f, 0.1f);
            Assert.That(CrossingRules.IsTaken(back), Is.True);
        }

        [Test]
        public void TurningBackToTheRoad_ResetsTheLook()
        {
            float back = CrossingRules.UpdateLookBack(0f, 170f, 0.45f);
            back = CrossingRules.UpdateLookBack(back, 30f, 0.1f);
            back = CrossingRules.UpdateLookBack(back, 170f, 0.1f);
            Assert.That(CrossingRules.IsTaken(back), Is.False);
            Assert.That(CrossingRules.UpdateLookBack(0f, 120f, 1f), Is.EqualTo(0f), "Exactly 120 degrees is not looking back.");
        }

        [Test]
        public void AngleFromRoad_IgnoresLookingUpOrDown()
        {
            var upAhead = new Vector3(0f, 5f, 1f);
            Assert.That(CrossingRules.AngleFromRoad(upAhead, Vector3.forward), Is.EqualTo(0f).Within(0.01f));
            Assert.That(CrossingRules.AngleFromRoad(Vector3.back, Vector3.forward), Is.EqualTo(180f).Within(0.01f));
        }
    }
}
