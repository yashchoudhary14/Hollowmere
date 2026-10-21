using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Hollowmere.Tests
{
    /// <summary>
    /// Plays the generated Greybox scene for a few seconds. Any exception or error log fails the test,
    /// which catches broken wiring that EditMode tests can't see.
    /// </summary>
    public class GreyboxSmokeTests
    {
        [UnityTest]
        public IEnumerator Greybox_PlaysForAFewSeconds_WithoutErrors()
        {
            SceneManager.LoadScene("Greybox");
            yield return null;
            yield return null;

            var player = Object.FindAnyObjectByType<PlayerAgent>();
            Assert.That(player, Is.Not.Null, "No PlayerAgent in the scene.");
            Assert.That(Object.FindAnyObjectByType<Hud>(), Is.Not.Null, "No HUD in the scene.");

            float until = Time.time + 3f;
            while (Time.time < until) yield return null;

            Assert.That(player.IsAlive, Is.True);
        }

        /// <summary>docs/DESIGN.md "Dusk transition" and "Night phase": latch at night, unlatch at midnight, power cut, dawn loss.</summary>
        [UnityTest]
        public IEnumerator Director_RunsTheMatch_FromArrivalToDawn()
        {
            SceneManager.LoadScene("Greybox");
            yield return null;
            yield return null;

            var d = Object.FindAnyObjectByType<MatchDirector>();
            Assert.That(d.Plan, Is.Not.Null);
            foreach (var h in d.Houses)
            {
                bool lived = d.Plan.IsAllotted(h.Index) || d.Plan.IsShrine(h.Index);
                Assert.That(h.Hearth.Lit, Is.EqualTo(lived), h.name);
                Assert.That(h.ShrineMarker.activeSelf, Is.EqualTo(d.Plan.IsShrine(h.Index)), h.name);
            }
            Assert.That(d.Board.Letters[0].Text, Does.Contain(d.Plan.PlayerNames[0]));
            Assert.That(d.RitualBook.Text, Does.Contain("THE RITE OF PASSAGE"));

            d.SkipToNextPhase();
            yield return null;
            Assert.That(d.Now.Phase, Is.EqualTo(MatchPhase.Day));
            Assert.That(d.Gate.IsOpen, Is.False, "The gate shuts behind the guests.");
            Assert.That(d.PuzzlesActive, Is.False, "Puzzles only work at night.");

            d.SkipToNextPhase(); // dusk
            d.SkipToNextPhase(); // night, act 1
            yield return null;
            Assert.That(d.Now.Act, Is.EqualTo(1));
            Assert.That(d.Houses, Has.All.Matches<House>(h => h.Door.Latched && !h.Door.IsOpen));
            Assert.That(d.PuzzlesActive, Is.True);

            d.SkipToNextPhase(); // act 2
            yield return null;
            Assert.That(d.Now.Act, Is.EqualTo(2));
            Assert.That(d.Houses, Has.None.Matches<House>(h => h.Door.Latched));

            d.SkipToNextPhase(); // act 3
            d.SkipToNextPhase(); // dawn, passing the minute-17 power cut
            yield return null;
            Assert.That(d.Houses, Has.None.Matches<House>(h => h.Powered));
            Assert.That(d.Outcome, Is.EqualTo(MatchOutcome.Lost));
        }

        /// <summary>docs/DESIGN.md "Houses and puzzles": each puzzle kind opens its chest with the plan's answer, and only at night.</summary>
        [UnityTest]
        public IEnumerator EachPuzzleKind_OpensItsChest_WithThePlansAnswer()
        {
            SceneManager.LoadScene("Greybox");
            yield return null;
            yield return null;
            var d = Object.FindAnyObjectByType<MatchDirector>();
            var player = Object.FindAnyObjectByType<PlayerAgent>();

            var codeHouse = d.Houses.First(h => h.Def.Puzzle == PuzzleKind.CodeLock);
            var codeRig = (CodeLockPuzzle)codeHouse.Puzzle;
            codeRig.Turn(0, player);
            Assert.That(codeRig.Dials[0], Is.EqualTo(0), "By day the dials don't turn.");

            for (int i = 0; i < 3; i++) d.SkipToNextPhase(); // day, dusk, night
            yield return null;
            Assert.That(d.PuzzlesActive, Is.True);

            foreach (var kind in new[] { PuzzleKind.CodeLock, PuzzleKind.Clock, PuzzleKind.Sequence })
            {
                var house = d.Houses.First(h => h.Def.Puzzle == kind);
                var answer = d.Plan.Solutions[house.Index];
                switch (house.Puzzle)
                {
                    case CodeLockPuzzle code:
                        for (int i = 0; i < 4; i++)
                            for (int turns = 0; turns < 10 && code.Dials[i] != answer.Code[i]; turns++) code.Turn(i, player);
                        break;
                    case ClockPuzzle clock:
                        Assert.That(player.PickUp(clock.Key), Is.True);
                        for (int turns = 0; turns < 12 && clock.Hour != answer.ClockHour; turns++) clock.Advance();
                        clock.Wind(player);
                        Assert.That(player.Inventory.Has(ItemKind.WindingKey), Is.False, "Winding uses up the key.");
                        break;
                    case SequencePuzzle sequence:
                        sequence.Press((answer.Sequence[0] + 1) % 6, player); // a wrong first press resets harmlessly
                        Assert.That(sequence.Progress, Is.EqualTo(0));
                        foreach (var symbol in answer.Sequence) sequence.Press(symbol, player);
                        break;
                }
                Assert.That(house.Puzzle.Solved, Is.True, $"{kind} in {house.name}");
                yield return new WaitForSeconds(1f);
                Assert.That(house.Container.IsOpen, Is.True);
                Assert.That(house.Container.Part.gameObject.activeSelf, Is.True);

                Assert.That(player.PickUp(house.Container.Part), Is.True);
                Assert.That(player.Inventory.RitualPart, Is.EqualTo(house.Index));
                player.GiveUpPart(out _);
            }
        }
    }
}
