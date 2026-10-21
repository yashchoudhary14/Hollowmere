namespace Hollowmere
{
    /// <summary>A safe with four dials. The code is MatchPlan.Solutions[house].Code; the house's notes split it in two.</summary>
    public class CodeLockPuzzle : PuzzleRig
    {
        public CodeDial[] DialParts;

        public int[] Dials { get; private set; } = new int[4];

        int[] _code;

        protected override void Configure(PuzzleSolution solution)
        {
            _code = solution.Code;
            Dials = new int[DialParts.Length];
            Refresh();
        }

        public void Turn(int dial, PlayerAgent player)
        {
            if (!Active) return;
            Dials[dial] = PuzzleRules.NextDigit(Dials[dial]);
            Refresh();
            if (PuzzleRules.CodeMatches(Dials, _code)) Solve(player);
        }

        void Refresh()
        {
            for (int i = 0; i < DialParts.Length; i++)
                if (DialParts[i].Digit != null) DialParts[i].Digit.text = Dials[i].ToString();
        }
    }
}
