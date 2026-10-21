using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// A grandfather clock: set the hour hand, then wind it with the winding key hidden in the house.
    /// The hour is MatchPlan.Solutions[house].ClockHour (never 12, where the hand starts); the key's spot is
    /// MatchPlan.KeySpots[house]. Design: docs/DESIGN.md "Example: Blue Door House".
    /// </summary>
    public class ClockPuzzle : PuzzleRig
    {
        public Transform HourHand;
        public Pickup Key;

        public int Hour { get; private set; } = 12;

        int _solution;

        protected override void Configure(PuzzleSolution solution)
        {
            _solution = solution.ClockHour;
            Hour = 12;
            Refresh();
        }

        public void PlaceKey(Vector3 position)
        {
            if (Key == null) return;
            PlayerAgent.PlaceInWorld(Key.gameObject, position);
        }

        public void Advance()
        {
            if (!Active) return;
            Hour = PuzzleRules.NextHour(Hour);
            Refresh();
        }

        public void Wind(PlayerAgent player)
        {
            if (!Active || !player.Inventory.Has(ItemKind.WindingKey)) return;
            if (PuzzleRules.ClockMatches(Hour, _solution))
            {
                player.Consume(ItemKind.WindingKey);
                Solve(player);
            }
            else
            {
                Fail(player, "The clock whirs, shudders and stops. Not that hour.");
            }
        }

        // Clockwise as seen from the room is negative about the face's +z.
        void Refresh() => HourHand.localRotation = Quaternion.Euler(0f, 0f, -(Hour % 12) * 30f);
    }
}
