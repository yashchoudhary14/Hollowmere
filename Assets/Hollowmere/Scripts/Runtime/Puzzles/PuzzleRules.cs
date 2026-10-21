namespace Hollowmere
{
    /// <summary>
    /// Answer checks for the three milestone-1 puzzles. Pure C#, unit-tested.
    /// Design: docs/DESIGN.md "Houses and puzzles"; answers come from MatchPlan.Solutions.
    /// </summary>
    public static class PuzzleRules
    {
        /// <summary>A code-lock dial turns 0 → 9 and wraps back to 0.</summary>
        public static int NextDigit(int digit) => (digit + 1) % 10;

        public static bool CodeMatches(int[] dials, int[] code)
        {
            if (dials == null || code == null || dials.Length != code.Length) return false;
            for (int i = 0; i < code.Length; i++)
                if (dials[i] != code[i]) return false;
            return true;
        }

        /// <summary>The clock's hour hand starts at 12 and steps 1, 2 … 12.</summary>
        public static int NextHour(int hour) => hour % 12 + 1;

        public static bool ClockMatches(int hour, int solutionHour) => hour == solutionHour;

        /// <summary>
        /// Progress through a symbol sequence after one press. A wrong press starts again,
        /// counting that press if it was the first symbol.
        /// </summary>
        public static int SequenceStep(int progress, int pressed, int[] sequence)
        {
            if (progress < sequence.Length && pressed == sequence[progress]) return progress + 1;
            return pressed == sequence[0] ? 1 : 0;
        }

        public static bool SequenceComplete(int progress, int[] sequence) => progress >= sequence.Length;
    }
}
