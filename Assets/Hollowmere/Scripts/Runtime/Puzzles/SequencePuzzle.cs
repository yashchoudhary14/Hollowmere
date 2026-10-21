using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// A panel of six symbols; four must be pressed in order (MatchPlan.Solutions[house].Sequence). A lamp lights
    /// for each correct press; a wrong one clunks and starts again. On a loud house (the Organist's pump organ)
    /// every press carries across the village.
    /// </summary>
    public class SequencePuzzle : PuzzleRig
    {
        public Renderer[] Lamps;
        public Material LampOn;
        public Material LampOff;

        public int Progress { get; private set; }

        int[] _sequence;

        protected override void Configure(PuzzleSolution solution)
        {
            _sequence = solution.Sequence;
            Progress = 0;
            RefreshLamps();
        }

        public void Press(int symbol, PlayerAgent player)
        {
            if (!Active) return;
            if (House.Def.LoudPuzzle)
                NoiseSystem.Emit(transform.position + Vector3.up, NoiseRadius.LoudPuzzle, NoiseKind.Puzzle, player.Index);

            int next = PuzzleRules.SequenceStep(Progress, symbol, _sequence);
            bool wrong = next != Progress + 1;
            Progress = next;
            RefreshLamps();
            if (PuzzleRules.SequenceComplete(Progress, _sequence)) Solve(player);
            else if (wrong) Fail(player, "A dull clunk. The lamps on the panel go out.");
        }

        void RefreshLamps()
        {
            for (int i = 0; i < Lamps.Length; i++)
                Lamps[i].sharedMaterial = i < Progress ? LampOn : LampOff;
        }
    }
}
