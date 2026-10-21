using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Base for a house puzzle. MatchDirector sets it up with the house's answer from MatchPlan.Solutions; it only
    /// works at night (docs/DESIGN.md "Decisions"). Solving it opens the house's PartContainer.
    /// Design: docs/DESIGN.md "Houses and puzzles".
    /// </summary>
    public abstract class PuzzleRig : MonoBehaviour
    {
        public House House;
        public PartContainer Container;

        public MatchDirector Director { get; private set; }
        public bool Solved { get; private set; }

        /// <summary>Whether its parts respond right now.</summary>
        public bool Active => !Solved && Director != null && Director.PuzzlesActive;

        /// <summary>Prompt for a part while the puzzle sleeps (or null once solved).</summary>
        public string DormantPrompt => Solved ? null : "It won't move. Not until dark.";

        public void Setup(MatchDirector director, PuzzleSolution solution)
        {
            Director = director;
            Solved = false;
            Configure(solution);
        }

        protected abstract void Configure(PuzzleSolution solution);

        protected void Solve(PlayerAgent player)
        {
            Solved = true;
            Container.Open();
            // DESIGN.md "Houses and puzzles": some puzzles are loud by nature (the Organist's pump organ).
            float noise = House.Def.LoudPuzzle ? NoiseRadius.LoudPuzzle : NoiseRadius.PuzzleSolved;
            NoiseSystem.Emit(transform.position + Vector3.up, noise, NoiseKind.Puzzle, player.Index);
            player.Notify("Something unlocks with a heavy click. The chest is open.");
        }

        protected void Fail(PlayerAgent player, string message)
        {
            NoiseSystem.Emit(transform.position + Vector3.up, NoiseRadius.PuzzleClunk, NoiseKind.Puzzle, player.Index);
            if (message != null) player.Notify(message);
        }
    }
}
