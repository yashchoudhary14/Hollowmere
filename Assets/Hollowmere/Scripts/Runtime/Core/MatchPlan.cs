using System;
using System.Collections.Generic;
using System.Text;

namespace Hollowmere
{
    public enum HollowVariant { Listener, Lamplighter, Guest, Watcher }

    /// <summary>The randomised answer for one house's puzzle.</summary>
    [Serializable]
    public class PuzzleSolution
    {
        public int[] Code = new int[4];
        public int ClockHour = 1;
        public int[] Sequence = new int[4];
    }

    /// <summary>
    /// Everything that is randomised for one match. Pure data, no Unity objects, so the
    /// same plan can later be generated on the host and sent to clients.
    /// </summary>
    public class MatchPlan
    {
        public int Seed;
        public int PlayerCount;
        public int[] AllottedHouses;   // player index -> house index
        public int[] AltarOrder;       // altar slot -> house index
        public int[] ShrineHouses;
        public PuzzleSolution[] Solutions; // house index -> solution
        public string[] PlayerNames;
        public string[] Letters;       // player index -> letter text
        public HollowVariant Variant;

        public bool IsAllotted(int house) => Array.IndexOf(AllottedHouses, house) >= 0;
        public int OwnerOf(int house) => Array.IndexOf(AllottedHouses, house);
        public bool IsShrine(int house) => Array.IndexOf(ShrineHouses, house) >= 0;
        public int SlotFor(int house) => Array.IndexOf(AltarOrder, house);

        public string RitualBookText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("THE RITE OF PASSAGE");
            sb.AppendLine();
            sb.AppendLine("The gate opens only for these, laid upon the altar in this order:");
            for (int i = 0; i < AltarOrder.Length; i++)
                sb.AppendLine($"   {i + 1}.  The {HouseCatalog.Houses[AltarOrder[i]].Part.ToLowerInvariant()}");
            sb.AppendLine();
            sb.AppendLine("Then stand within the stones, all of you who still breathe,");
            sb.AppendLine("and keep the flames alive until the chant is done.");
            sb.AppendLine();
            sb.AppendLine($"Where the carved diamond hangs above a door, the dead may be called home.");
            sb.AppendLine($"This autumn it hangs above {HouseCatalog.NumberWord(ShrineHouses.Length)} doors.");
            return sb.ToString();
        }
    }

    public static class MatchSetup
    {
        public static MatchPlan Generate(int seed, int playerCount, string[] playerNames, int forcedVariant = -1)
        {
            var rng = new Random(seed);
            playerCount = Math.Max(1, Math.Min(6, playerCount));

            var plan = new MatchPlan { Seed = seed, PlayerCount = playerCount };

            // 1. Allot one house per player.
            var houses = new List<int>();
            for (int i = 0; i < HouseCatalog.Count; i++) houses.Add(i);
            Shuffle(houses, rng);
            plan.AllottedHouses = houses.GetRange(0, playerCount).ToArray();

            // 2. Altar order: the allotted houses' parts in a random order.
            var order = new List<int>(plan.AllottedHouses);
            Shuffle(order, rng);
            plan.AltarOrder = order.ToArray();

            // 3. Shrines: 3 for small groups, 4 for larger. May overlap allotted houses.
            int shrineCount = playerCount <= 3 ? 3 : 4;
            var shrinePool = new List<int>();
            for (int i = 0; i < HouseCatalog.Count; i++) shrinePool.Add(i);
            Shuffle(shrinePool, rng);
            plan.ShrineHouses = shrinePool.GetRange(0, shrineCount).ToArray();

            // 4. Puzzle answers for every house (hollow houses get them too, for later cross-house puzzles).
            plan.Solutions = new PuzzleSolution[HouseCatalog.Count];
            for (int i = 0; i < HouseCatalog.Count; i++)
            {
                var s = new PuzzleSolution();
                for (int d = 0; d < 4; d++) s.Code[d] = rng.Next(0, 10);
                s.ClockHour = rng.Next(1, 12); // 1..11, never the dial's starting 12
                var symbols = new List<int> { 0, 1, 2, 3, 4, 5 };
                Shuffle(symbols, rng);
                for (int k = 0; k < 4; k++) s.Sequence[k] = symbols[k];
                plan.Solutions[i] = s;
            }

            // 5. Letters.
            plan.PlayerNames = new string[playerCount];
            plan.Letters = new string[playerCount];
            for (int p = 0; p < playerCount; p++)
            {
                plan.PlayerNames[p] = playerNames != null && p < playerNames.Length && !string.IsNullOrEmpty(playerNames[p])
                    ? playerNames[p] : $"Traveller {p + 1}";
                plan.Letters[p] = WriteLetter(plan.PlayerNames[p], HouseCatalog.Houses[plan.AllottedHouses[p]], rng);
            }

            plan.Variant = forcedVariant >= 0 && forcedVariant <= 3
                ? (HollowVariant)forcedVariant
                : (HollowVariant)rng.Next(0, 4);

            return plan;
        }

        static string WriteLetter(string name, HouseDef def, Random rng)
        {
            var traits = new List<string>
            {
                $"Yours is the door {HouseCatalog.DoorRiddle(def.Door)}.",
                def.Chimneys == 1
                    ? "Smoke leaves it by a single chimney."
                    : $"Smoke leaves it by {HouseCatalog.NumberWord(def.Chimneys)} chimneys.",
                $"The clock beside its door gave up at {HouseCatalog.NumberWord(def.ClockHour)} o'clock and never started again.",
                $"The {def.Family} family kept it, once.",
            };
            Shuffle(traits, rng);

            var sb = new StringBuilder();
            sb.AppendLine($"Dear {name},");
            sb.AppendLine();
            sb.AppendLine("We have kept a house for you. You will know it.");
            sb.AppendLine();
            for (int i = 0; i < 3; i++) sb.AppendLine(traits[i]);
            sb.AppendLine();
            sb.AppendLine("Something of ours waits inside it. Bring it to the altar.");
            sb.AppendLine("Be indoors before the last bell.");
            sb.AppendLine();
            sb.AppendLine("— Hollowmere");
            return sb.ToString();
        }

        public static void Shuffle<T>(IList<T> list, Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
