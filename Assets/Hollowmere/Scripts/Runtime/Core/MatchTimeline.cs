using System;
using UnityEngine;

namespace Hollowmere
{
    public enum MatchPhase { Arrival, Day, Dusk, Night, Dawn }

    /// <summary>Phase lengths for one match. Defaults are Normal difficulty from docs/DESIGN.md.</summary>
    [Serializable]
    public class TimelineSettings
    {
        [Tooltip("DESIGN.md \"Core loop\": arrival, 1 min")]
        public float ArrivalSeconds = 60f;
        [Tooltip("DESIGN.md \"Day phase\": 12 min on Normal")]
        public float DaySeconds = 720f;
        [Tooltip("DESIGN.md \"Day phase\", time signals: a single warning toll at 11 min")]
        public float WarningTollDaySeconds = 660f;
        [Tooltip("DESIGN.md \"Dusk transition\": 90 s")]
        public float DuskSeconds = 90f;
        [Tooltip("DESIGN.md \"Dusk transition\": 12 tolls, one every 7.5 s")]
        public int DuskTolls = 12;
        [Tooltip("DESIGN.md \"Night phase\": Act 1, The Quiet, 6 min")]
        public float Act1Seconds = 360f;
        [Tooltip("DESIGN.md \"Night phase\": Act 2, The Siege, 8 min")]
        public float Act2Seconds = 480f;
        [Tooltip("DESIGN.md \"Night phase\": Act 3, The Hunt, 6 min")]
        public float Act3Seconds = 360f;
        [Tooltip("DESIGN.md \"Night phase\": a village-wide power cut at minute 17 of the night")]
        public float PowerCutNightSeconds = 1020f;
    }

    /// <summary>Where the match clock is: phase, act (1–3 at night, else 0) and progress through the phase.</summary>
    public struct TimelinePoint
    {
        public MatchPhase Phase;
        public int Act;
        public float PhaseElapsed;
        public float PhaseLength;

        public float PhaseRemaining => Math.Max(0f, PhaseLength - PhaseElapsed);
        public float Phase01 => PhaseLength > 0f ? Math.Min(1f, PhaseElapsed / PhaseLength) : 1f;
    }

    /// <summary>
    /// Phase maths for one match: pure C#, unit-tested. The clock is seconds since arrival.
    /// Arrival → Day → Dusk → Night (Act 1, 2, 3) → Dawn, the loss.
    /// </summary>
    public static class MatchTimeline
    {
        public static float DayStart(TimelineSettings s) => s.ArrivalSeconds;
        public static float WarningTollAt(TimelineSettings s) => DayStart(s) + s.WarningTollDaySeconds;
        public static float DuskStart(TimelineSettings s) => DayStart(s) + s.DaySeconds;
        public static float NightStart(TimelineSettings s) => DuskStart(s) + s.DuskSeconds;
        public static float Act2Start(TimelineSettings s) => NightStart(s) + s.Act1Seconds;
        public static float Act3Start(TimelineSettings s) => Act2Start(s) + s.Act2Seconds;
        public static float DawnAt(TimelineSettings s) => Act3Start(s) + s.Act3Seconds;
        public static float PowerCutAt(TimelineSettings s) => NightStart(s) + s.PowerCutNightSeconds;

        /// <summary>When dusk toll number toll (0-based) rings. The last rings one interval before night falls.</summary>
        public static float TollTime(TimelineSettings s, int toll) => DuskStart(s) + toll * (s.DuskSeconds / s.DuskTolls);

        public static TimelinePoint At(TimelineSettings s, float clock)
        {
            if (clock < DayStart(s)) return Point(MatchPhase.Arrival, 0, clock, s.ArrivalSeconds);
            if (clock < DuskStart(s)) return Point(MatchPhase.Day, 0, clock - DayStart(s), s.DaySeconds);
            if (clock < NightStart(s)) return Point(MatchPhase.Dusk, 0, clock - DuskStart(s), s.DuskSeconds);
            if (clock < Act2Start(s)) return Point(MatchPhase.Night, 1, clock - NightStart(s), s.Act1Seconds);
            if (clock < Act3Start(s)) return Point(MatchPhase.Night, 2, clock - Act2Start(s), s.Act2Seconds);
            if (clock < DawnAt(s)) return Point(MatchPhase.Night, 3, clock - Act3Start(s), s.Act3Seconds);
            return Point(MatchPhase.Dawn, 0, clock - DawnAt(s), 0f);
        }

        /// <summary>The next phase or act boundary after the clock (debug skip).</summary>
        public static float NextBoundary(TimelineSettings s, float clock)
        {
            foreach (var b in new[] { DayStart(s), DuskStart(s), NightStart(s), Act2Start(s), Act3Start(s), DawnAt(s) })
                if (b > clock + 0.001f) return b;
            return clock;
        }

        /// <summary>Whether a moment was passed while the clock moved from before to after.</summary>
        public static bool Crossed(float before, float after, float moment) => before < moment && after >= moment;

        static TimelinePoint Point(MatchPhase phase, int act, float elapsed, float length)
            => new TimelinePoint { Phase = phase, Act = act, PhaseElapsed = elapsed, PhaseLength = length };
    }
}
