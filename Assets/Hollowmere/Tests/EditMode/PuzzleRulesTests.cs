using System.Linq;
using NUnit.Framework;

namespace Hollowmere.Tests
{
    /// <summary>Puzzle answer checks and the notes that hint at them (docs/DESIGN.md "Houses and puzzles").</summary>
    public class PuzzleRulesTests
    {
        [Test]
        public void Dials_Wrap_From9To0()
        {
            Assert.That(PuzzleRules.NextDigit(0), Is.EqualTo(1));
            Assert.That(PuzzleRules.NextDigit(9), Is.EqualTo(0));
        }

        [Test]
        public void Code_MatchesOnlyTheExactDigits()
        {
            Assert.That(PuzzleRules.CodeMatches(new[] { 4, 7, 0, 2 }, new[] { 4, 7, 0, 2 }), Is.True);
            Assert.That(PuzzleRules.CodeMatches(new[] { 4, 7, 2, 0 }, new[] { 4, 7, 0, 2 }), Is.False);
            Assert.That(PuzzleRules.CodeMatches(new[] { 4, 7, 0 }, new[] { 4, 7, 0, 2 }), Is.False);
        }

        [Test]
        public void ClockHand_StepsFrom12Through1To12()
        {
            int hour = 12;
            var seen = Enumerable.Range(0, 12).Select(_ => hour = PuzzleRules.NextHour(hour)).ToArray();
            Assert.That(seen, Is.EqualTo(Enumerable.Range(1, 12).ToArray()));
        }

        [Test]
        public void Sequence_AdvancesOnTheRightSymbol_AndStartsAgainOnAWrongOne()
        {
            int[] seq = { 2, 5, 0, 3 };
            int p = 0;
            p = PuzzleRules.SequenceStep(p, 2, seq); Assert.That(p, Is.EqualTo(1));
            p = PuzzleRules.SequenceStep(p, 5, seq); Assert.That(p, Is.EqualTo(2));
            p = PuzzleRules.SequenceStep(p, 4, seq); Assert.That(p, Is.EqualTo(0), "Wrong symbol resets.");
            p = PuzzleRules.SequenceStep(p, 2, seq);
            p = PuzzleRules.SequenceStep(p, 2, seq); Assert.That(p, Is.EqualTo(1), "A wrong press that is the first symbol counts as a fresh start.");
            p = PuzzleRules.SequenceStep(p, 5, seq);
            p = PuzzleRules.SequenceStep(p, 0, seq);
            p = PuzzleRules.SequenceStep(p, 3, seq);
            Assert.That(PuzzleRules.SequenceComplete(p, seq), Is.True);
        }

        [Test]
        public void Notes_RevealEveryAnswer_SplitAcrossTwoRooms()
        {
            for (int seed = 0; seed < 25; seed++)
            {
                var plan = MatchSetup.Generate(seed, 6, null);
                for (int house = 0; house < HouseCatalog.Count; house++)
                {
                    var notes = HouseNotes.Write(plan, house);
                    var s = plan.Solutions[house];
                    Assert.That(notes.Length, Is.EqualTo(3));
                    switch (HouseCatalog.Houses[house].Puzzle)
                    {
                        case PuzzleKind.CodeLock:
                            StringAssert.Contains($"{HouseNotes.Digit(s.Code[0])} and {HouseNotes.Digit(s.Code[1])}", notes[0].Text);
                            StringAssert.Contains($"{HouseNotes.Digit(s.Code[2])}, then {HouseNotes.Digit(s.Code[3])}", notes[1].Text);
                            break;
                        case PuzzleKind.Clock:
                            StringAssert.Contains($"points at {HouseCatalog.NumberWord(s.ClockHour)}", notes[0].Text);
                            StringAssert.Contains(HouseNotes.KeySpotPhrases[plan.KeySpots[house]], notes[1].Text);
                            break;
                        default:
                            StringAssert.Contains($"First the {HouseNotes.Symbol(s.Sequence[0])}, then the {HouseNotes.Symbol(s.Sequence[1])}", notes[0].Text);
                            StringAssert.Contains($"then the {HouseNotes.Symbol(s.Sequence[2])}, and last of all the {HouseNotes.Symbol(s.Sequence[3])}", notes[1].Text);
                            break;
                    }
                    StringAssert.Contains(HouseNotes.WhatTookThem(plan.Variant), notes[2].Text);
                }
            }
        }

        [Test]
        public void KeySpots_AreInRange_AndTheSameForTheSameSeed()
        {
            var a = MatchSetup.Generate(77, 4, null);
            var b = MatchSetup.Generate(77, 4, null);
            CollectionAssert.AreEqual(a.KeySpots, b.KeySpots);
            Assert.That(a.KeySpots.Length, Is.EqualTo(HouseCatalog.Count));
            Assert.That(a.KeySpots.All(k => k >= 0 && k < HouseNotes.KeySpotCount));
        }
    }
}
