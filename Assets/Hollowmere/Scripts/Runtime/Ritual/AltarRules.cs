namespace Hollowmere
{
    public enum PlaceResult { Accepted, WrongSlot, SlotTaken, SlotUnused }

    /// <summary>
    /// Where ritual parts may go on the altar: slot i takes only the part of house MatchPlan.AltarOrder[i].
    /// Pure C#, unit-tested. Design: docs/DESIGN.md "The Ritual of Passage".
    /// </summary>
    public static class AltarRules
    {
        /// <summary>placed[slot] is the house whose part sits in that slot, or -1.</summary>
        public static PlaceResult Place(MatchPlan plan, int[] placed, int slot, int house)
        {
            if (slot < 0 || slot >= plan.AltarOrder.Length) return PlaceResult.SlotUnused;
            if (placed[slot] >= 0) return PlaceResult.SlotTaken;
            return plan.AltarOrder[slot] == house ? PlaceResult.Accepted : PlaceResult.WrongSlot;
        }

        public static bool AllPlaced(MatchPlan plan, int[] placed)
        {
            for (int slot = 0; slot < plan.AltarOrder.Length; slot++)
                if (placed[slot] != plan.AltarOrder[slot]) return false;
            return true;
        }

        public static string Numeral(int slot)
        {
            string[] numerals = { "I", "II", "III", "IV", "V", "VI" };
            return slot >= 0 && slot < numerals.Length ? numerals[slot] : (slot + 1).ToString();
        }
    }
}
