using System;

namespace Hollowmere
{
    public enum ChantStatus { Waiting, Chanting, Paused, Guttering, Reset, Complete }

    /// <summary>
    /// The 60-second chant of the Ritual of Passage. It pauses while a living player is outside the stone circle.
    /// When every candle is out it gutters; if they all stay out for 3 seconds it starts again from zero.
    /// Pure C#, unit-tested. Design: docs/DESIGN.md "The Ritual of Passage" and "Decisions" (chant reset).
    /// </summary>
    [Serializable]
    public class ChantState
    {
        public float Seconds = 60f;
        public float ResetGraceSeconds = 3f;

        public float Progress { get; private set; }
        public bool Started { get; private set; }
        public bool Complete => Started && Progress >= Seconds;
        public float Progress01 => Seconds > 0f ? Math.Min(1f, Progress / Seconds) : 1f;

        float _allOutFor;

        /// <summary>The chant starts once every part is placed and every living player stands in the circle.</summary>
        public static bool CanStart(bool allPartsPlaced, int livingPlayers, int livingInCircle)
            => allPartsPlaced && livingPlayers > 0 && livingInCircle == livingPlayers;

        public void Begin()
        {
            Started = true;
            Progress = 0f;
            _allOutFor = 0f;
        }

        public ChantStatus Tick(float dt, bool everyoneInCircle, int litCandles)
        {
            if (!Started) return ChantStatus.Waiting;
            if (Complete) return ChantStatus.Complete;
            if (litCandles <= 0)
            {
                _allOutFor += dt;
                if (_allOutFor < ResetGraceSeconds) return ChantStatus.Guttering;
                Progress = 0f;
                return ChantStatus.Reset;
            }
            _allOutFor = 0f;
            if (!everyoneInCircle) return ChantStatus.Paused;
            Progress = Math.Min(Seconds, Progress + dt);
            return Complete ? ChantStatus.Complete : ChantStatus.Chanting;
        }
    }
}
