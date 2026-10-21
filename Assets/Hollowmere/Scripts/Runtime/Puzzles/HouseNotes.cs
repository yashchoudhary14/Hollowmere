namespace Hollowmere
{
    /// <summary>
    /// Writes a house's three notes from the MatchPlan: the hall and hearth-room notes split the puzzle's answer
    /// (and, for the clock, say where the winding key is); the back-room note is the family's lore and what took
    /// them last autumn, which hints at the Hollow's variant. Pure C#, unit-tested.
    /// Design: docs/DESIGN.md "Houses and puzzles", "Monster variants" (information sources).
    /// </summary>
    public static class HouseNotes
    {
        public struct Note
        {
            public string Title;
            public string Text;
        }

        /// <summary>House.ItemSpots order, as GreyboxVillageBuilder builds them.</summary>
        public static readonly string[] KeySpotPhrases =
        {
            "on the mantel above the hearth",
            "on the little table in the hall",
            "on the table in the back room",
        };

        public static int KeySpotCount => KeySpotPhrases.Length;

        /// <summary>Notes for the hall, the hearth room and the back room, in that order.</summary>
        public static Note[] Write(MatchPlan plan, int house)
        {
            var def = HouseCatalog.Houses[house];
            var s = plan.Solutions[house];
            var notes = new Note[3];
            switch (def.Puzzle)
            {
                case PuzzleKind.CodeLock:
                    notes[0] = new Note
                    {
                        Title = "A torn page",
                        Text = "I've split the numbers for the safe in the back room, in case they come looking.\n\n" +
                               $"The first two are {Digit(s.Code[0])} and {Digit(s.Code[1])}.",
                    };
                    notes[1] = new Note
                    {
                        Title = "A note by the hearth",
                        Text = $"The last two for the safe: {Digit(s.Code[2])}, then {Digit(s.Code[3])}.\n\nBurn this after.",
                    };
                    break;

                case PuzzleKind.Clock:
                    notes[0] = new Note
                    {
                        Title = "A child's drawing",
                        Text = "A crayon drawing of the tall clock in the back room. " +
                               $"Its little hand points at {HouseCatalog.NumberWord(s.ClockHour)}.\n\n" +
                               "Underneath, in careful capitals: SET IT, WIND IT, AND IT OPENS.",
                    };
                    notes[1] = new Note
                    {
                        Title = "A note by the hearth",
                        Text = $"I've put the winding key {KeySpotPhrases[plan.KeySpots[house]]}. Don't let the children play with it.",
                    };
                    break;

                default:
                    notes[0] = new Note
                    {
                        Title = "A hymn sheet",
                        Text = "Written in the margin, for the panel in the back room:\n\n" +
                               $"First the {Symbol(s.Sequence[0])}, then the {Symbol(s.Sequence[1])}…",
                    };
                    notes[1] = new Note
                    {
                        Title = "A note by the hearth",
                        Text = $"…then the {Symbol(s.Sequence[2])}, and last of all the {Symbol(s.Sequence[3])}.\n\nGet it wrong and it starts again from the beginning.",
                    };
                    break;
            }

            notes[2] = new Note
            {
                Title = $"The {def.Family} family",
                Text = $"The {def.Family}s kept this house for four generations, and kept the {def.Part.ToLowerInvariant()} safe in it. " +
                       "They were good to the village, and the village was good to them, until last autumn.\n\n" + WhatTookThem(plan.Variant),
            };
            return notes;
        }

        public static string Digit(int d) => HouseCatalog.NumberWord(d);

        public static string Symbol(int index) => HouseCatalog.SequenceSymbols[index].ToLowerInvariant();

        /// <summary>One line per variant, matching its tells in docs/DESIGN.md "Monster variants".</summary>
        public static string WhatTookThem(HollowVariant variant)
        {
            switch (variant)
            {
                case HollowVariant.Listener: return "It never looked at us. It listened. By the end we only whispered, and Father wouldn't let us whisper either.";
                case HollowVariant.Lamplighter: return "It came wherever a lamp was burning. We learned to sit in the dark, and it went past the windows without stopping.";
                case HollowVariant.Guest: return "It knocked so politely. Mother said never open, and never, ever tell it to come in.";
                default: return "It only moved when none of us were looking at it. We took turns watching the window all night.";
            }
        }
    }
}
