using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
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
            FreezeHollow();
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
            FreezeHollow();

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
                SolveWithPlansAnswer(d, house, player);
                Assert.That(house.Puzzle.Solved, Is.True, $"{kind} in {house.name}");
                yield return new WaitForSeconds(1f);
                Assert.That(house.Container.IsOpen, Is.True);
                Assert.That(house.Container.Part.gameObject.activeSelf, Is.True);

                Assert.That(player.PickUp(house.Container.Part), Is.True);
                Assert.That(player.Inventory.RitualPart, Is.EqualTo(house.Index));
                player.GiveUpPart(out _);
            }
        }

        /// <summary>
        /// A two-player match played by one person to the end: both puzzles, a part in the wrong slot,
        /// both parts placed, the chant (with every candle out once), the gate, and the walk to the bridge.
        /// </summary>
        [UnityTest]
        public IEnumerator FullMatch_PartsChantAndCrossing_EndInAWin()
        {
            yield return StartNightOfTwoPlayerMatch();
            var d = Object.FindAnyObjectByType<MatchDirector>();
            var player = Object.FindAnyObjectByType<PlayerAgent>();
            d.Circle.Chant.Seconds = 2f; // the real chant is 60 s

            for (int i = 0; i < d.Plan.PlayerCount; i++)
            {
                var house = d.Houses[d.Plan.AllottedHouses[i]];
                SolveWithPlansAnswer(d, house, player);
                yield return null;
                Assert.That(player.PickUp(house.Container.Part), Is.True);

                int right = d.Plan.SlotFor(house.Index);
                if (i == 0)
                {
                    var wrong = d.Altar.Slots[1 - right];
                    d.Altar.Place(wrong, player);
                    Assert.That(player.Inventory.HasPart, Is.False, "A wrong slot throws the part back.");
                    Assert.That(d.Altar.Placed[wrong.Index], Is.EqualTo(-1));
                    Assert.That(house.Container.Part.gameObject.activeSelf, Is.True);
                    Assert.That(player.PickUp(house.Container.Part), Is.True);
                }
                d.Altar.Place(d.Altar.Slots[right], player);
                Assert.That(d.Altar.Placed[right], Is.EqualTo(house.Index));
            }
            Assert.That(d.Altar.AllPlaced, Is.True);
            Assert.That(d.Circle.Chant.Started, Is.False, "Nobody is in the circle yet.");

            player.Motor.Teleport(d.Circle.transform.position + new Vector3(0f, 0.1f, -3f), 0f);
            yield return null;
            yield return null;
            Assert.That(d.Circle.Chant.Started, Is.True);
            Assert.That(d.Circle.LitCount, Is.EqualTo(2));

            d.Circle.SnuffAll();
            yield return null;
            Assert.That(d.Circle.Status, Is.EqualTo(ChantStatus.Guttering), "All out: a few seconds to relight before it resets.");
            var candle = d.Circle.Candles[0];
            Assert.That(candle.CanInteract(player), Is.True, "Relight from inside the circle with a match.");
            int matches = player.Inventory.Matches;
            player.Interact(candle, InteractKind.Tap);
            Assert.That(candle.Lit, Is.True);
            Assert.That(player.Inventory.Matches, Is.EqualTo(matches - 1));

            yield return new WaitForSeconds(2.5f);
            Assert.That(d.Circle.Chant.Complete, Is.True);
            Assert.That(d.Gate.IsOpen, Is.True);

            player.Motor.Teleport(new Vector3(0f, 0.1f, d.Layout.GateLineZ + 5f), 0f);
            yield return null;
            Assert.That(player.Motor.WalkOnly, Is.True, "Only walking on the fog road.");
            player.Motor.Teleport(new Vector3(0f, 0.1f, d.Layout.BridgeZ + 1f), 0f);
            yield return null;
            yield return null;
            Assert.That(player.Condition, Is.EqualTo(PlayerCondition.Escaped));
            Assert.That(d.Outcome, Is.EqualTo(MatchOutcome.Won));
        }

        [UnityTest]
        public IEnumerator LookingBackOnTheFogRoad_GetsYouTaken()
        {
            yield return StartNightOfTwoPlayerMatch();
            var d = Object.FindAnyObjectByType<MatchDirector>();
            var player = Object.FindAnyObjectByType<PlayerAgent>();
            d.OpenGate();
            player.Motor.Teleport(new Vector3(0f, 0.1f, d.Layout.GateLineZ + 20f), 0f);
            yield return null;
            player.Motor.SetLook(180f, 0f);
            yield return new WaitForSeconds(0.8f);
            Assert.That(player.Condition, Is.EqualTo(PlayerCondition.Taken));
            Assert.That(d.Outcome, Is.EqualTo(MatchOutcome.Lost));
        }

        /// <summary>DESIGN.md "Dusk transition": a player outside when the bell ends is hunted at once; a grab kills in 5 s.</summary>
        [UnityTest]
        public IEnumerator CaughtOutsideAtNightfall_TheHollowHuntsGrabsAndKills()
        {
            SceneManager.LoadScene("Greybox");
            yield return null;
            yield return null;
            var d = Object.FindAnyObjectByType<MatchDirector>();
            var player = Object.FindAnyObjectByType<PlayerAgent>();
            var hollow = Object.FindAnyObjectByType<HollowAI>();
            Assert.That(hollow.State, Is.EqualTo(HollowState.Dormant), "It sleeps through the day.");

            for (int i = 0; i < 3; i++) d.SkipToNextPhase(); // day, dusk, night
            yield return null;
            Assert.That(d.HouseAt(player.transform.position), Is.Null, "The player is out on the square.");
            Assert.That(hollow.State, Is.EqualTo(HollowState.Hunt));
            Assert.That(hollow.Target, Is.EqualTo(player));

            float give = Time.time + 40f;
            while (player.Condition == PlayerCondition.Alive && Time.time < give) yield return null;
            Assert.That(player.Condition, Is.EqualTo(PlayerCondition.Grabbed), "It should reach and grab the player.");

            yield return new WaitForSeconds(5.5f);
            Assert.That(player.Condition, Is.EqualTo(PlayerCondition.Dead));
            Assert.That(d.Outcome, Is.EqualTo(MatchOutcome.Lost));
        }

        /// <summary>DESIGN.md "The monster": it cannot cross a lit hearth. Doors open, the hall is reachable, the hearth room isn't.</summary>
        [UnityTest]
        public IEnumerator LitHearthRoom_IsShutToTheHollow_EvenWithTheDoorOpen()
        {
            SceneManager.LoadScene("Greybox");
            yield return null;
            yield return null;
            var d = Object.FindAnyObjectByType<MatchDirector>();
            FreezeHollow();
            var home = d.HomeOf(Object.FindAnyObjectByType<PlayerAgent>());
            Assert.That(home.Hearth.Lit, Is.True);
            home.Door.Swing(true, 0.1f, 0f);
            yield return new WaitForSeconds(1.5f); // carving updates once the door has stopped

            var outside = home.Door.transform.position + home.transform.forward * 3f;
            Assert.That(PathStatus(outside, home.Rooms[0].Bounds.center), Is.EqualTo(NavMeshPathStatus.PathComplete), "The hall");
            Assert.That(PathStatus(outside, home.HearthRoom.Bounds.center), Is.Not.EqualTo(NavMeshPathStatus.PathComplete), "The hearth room");
        }

        static NavMeshPathStatus PathStatus(Vector3 from, Vector3 to)
        {
            NavMesh.SamplePosition(from, out var a, 3f, NavMesh.AllAreas);
            to.y = from.y;
            var path = new NavMeshPath();
            NavMesh.CalculatePath(a.position, to, NavMesh.AllAreas, path);
            if (path.status == NavMeshPathStatus.PathComplete && path.corners.Length > 0 && Vector3.Distance(path.corners[^1], to) > 0.6f)
                return NavMeshPathStatus.PathPartial;
            return path.status;
        }

        static void FreezeHollow() => Object.FindAnyObjectByType<HollowAI>().Frozen = true;

        static IEnumerator StartNightOfTwoPlayerMatch()
        {
            MatchLaunch.Next = new MatchLaunch.Settings { Seed = 2026, Players = 2, ClockSpeed = 1f };
            SceneManager.LoadScene("Greybox");
            yield return null;
            yield return null;
            var d = Object.FindAnyObjectByType<MatchDirector>();
            FreezeHollow();
            Assert.That(d.Plan.PlayerCount, Is.EqualTo(2));
            for (int i = 0; i < 3; i++) d.SkipToNextPhase(); // day, dusk, night
            yield return null;
        }

        static void SolveWithPlansAnswer(MatchDirector d, House house, PlayerAgent player)
        {
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
        }
    }
}
