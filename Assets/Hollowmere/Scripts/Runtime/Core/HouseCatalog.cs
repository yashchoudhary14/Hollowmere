using UnityEngine;

namespace Hollowmere
{
    public enum PuzzleKind { CodeLock, Clock, Sequence }

    public enum DoorColour { Blue, Red, Green, Yellow, Black, White }

    /// <summary>Static description of one of the 15 houses. Shared by the editor builder and the game.</summary>
    public readonly struct HouseDef
    {
        public readonly string Name;
        public readonly string Family;
        public readonly string Part;
        public readonly PuzzleKind Puzzle;
        public readonly DoorColour Door;
        public readonly int Chimneys;
        public readonly int ClockHour;
        public readonly bool LoudPuzzle;

        public HouseDef(string name, string family, string part, PuzzleKind puzzle,
                        DoorColour door, int chimneys, int clockHour, bool loudPuzzle = false)
        {
            Name = name; Family = family; Part = part; Puzzle = puzzle;
            Door = door; Chimneys = chimneys; ClockHour = clockHour; LoudPuzzle = loudPuzzle;
        }
    }

    public static class HouseCatalog
    {
        public const int Count = 15;

        // Every (door colour, chimney count) pair is unique, so a letter can always point at one house.
        public static readonly HouseDef[] Houses =
        {
            new HouseDef("Blue Door House",      "Hale",    "Iron gate key",      PuzzleKind.Clock,    DoorColour.Blue,   2, 3),
            new HouseDef("Rao House",            "Rao",     "Family seal ring",   PuzzleKind.CodeLock, DoorColour.Green,  1, 9),
            new HouseDef("Widow's House",        "Marsh",   "Hearth ember",       PuzzleKind.Sequence, DoorColour.Black,  1, 6),
            new HouseDef("Schoolmaster's House", "Penrose", "Bell clapper",       PuzzleKind.CodeLock, DoorColour.White,  2, 8),
            new HouseDef("Weaver's House",       "Ashdown", "Spool of red thread",PuzzleKind.Sequence, DoorColour.Red,    1, 11),
            new HouseDef("Doctor's House",       "Crane",   "Glass eye",          PuzzleKind.CodeLock, DoorColour.White,  1, 4),
            new HouseDef("Twins' House",         "Abney",   "Silver locket",      PuzzleKind.Sequence, DoorColour.Yellow, 2, 2),
            new HouseDef("Organist's House",     "Venn",    "Organ pipe",         PuzzleKind.Sequence, DoorColour.Red,    3, 7, loudPuzzle: true),
            new HouseDef("Butcher's House",      "Grice",   "Bone whistle",       PuzzleKind.CodeLock, DoorColour.Red,    2, 5),
            new HouseDef("Miller's House",       "Thorne",  "Last-harvest sheaf", PuzzleKind.Clock,    DoorColour.Yellow, 1, 10),
            new HouseDef("Photographer's House", "Lisle",   "Glass negative",     PuzzleKind.CodeLock, DoorColour.Black,  2, 12),
            new HouseDef("Priest's House",       "Ambrose", "Altar cloth",        PuzzleKind.CodeLock, DoorColour.White,  3, 1),
            new HouseDef("Postmaster's House",   "Dunn",    "Sealed letter",      PuzzleKind.Sequence, DoorColour.Green,  2, 3),
            new HouseDef("Farmer's House",       "Barrow",  "Straw heart",        PuzzleKind.Sequence, DoorColour.Green,  3, 6),
            new HouseDef("Dollmaker's House",    "Quill",   "Porcelain hand",     PuzzleKind.Sequence, DoorColour.Blue,   1, 9),
        };

        public static readonly string[] SequenceSymbols = { "Moon", "Eye", "Hand", "Key", "Bird", "Bell" };

        public static Color DoorTint(DoorColour c)
        {
            switch (c)
            {
                case DoorColour.Blue:   return new Color(0.16f, 0.27f, 0.55f);
                case DoorColour.Red:    return new Color(0.55f, 0.12f, 0.10f);
                case DoorColour.Green:  return new Color(0.18f, 0.38f, 0.20f);
                case DoorColour.Yellow: return new Color(0.75f, 0.62f, 0.18f);
                case DoorColour.Black:  return new Color(0.06f, 0.06f, 0.07f);
                default:                return new Color(0.88f, 0.86f, 0.80f);
            }
        }

        /// <summary>The riddle phrase a letter uses for a door colour.</summary>
        public static string DoorRiddle(DoorColour c)
        {
            switch (c)
            {
                case DoorColour.Blue:   return "the colour of a drowned sky";
                case DoorColour.Red:    return "the colour of an old wound";
                case DoorColour.Green:  return "the colour of pond moss";
                case DoorColour.Yellow: return "the colour of a sick moon";
                case DoorColour.Black:  return "the colour of a mourning veil";
                default:                return "the colour of bone";
            }
        }

        public static string NumberWord(int n)
        {
            string[] words = { "zero", "one", "two", "three", "four", "five", "six",
                               "seven", "eight", "nine", "ten", "eleven", "twelve" };
            return n >= 0 && n < words.Length ? words[n] : n.ToString();
        }
    }
}
