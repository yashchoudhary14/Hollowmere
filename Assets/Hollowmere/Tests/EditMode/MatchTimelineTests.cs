using NUnit.Framework;

namespace Hollowmere.Tests
{
    /// <summary>docs/DESIGN.md "Core loop", "Dusk transition", "Night phase" and the "Sample match timeline".</summary>
    public class MatchTimelineTests
    {
        static readonly TimelineSettings S = new TimelineSettings();

        static float Min(int m, int s = 0) => m * 60f + s;

        [Test]
        public void SampleTimeline_Moments_MatchTheDesignDoc()
        {
            Assert.That(MatchTimeline.DayStart(S), Is.EqualTo(Min(1)));
            Assert.That(MatchTimeline.DuskStart(S), Is.EqualTo(Min(13)));
            Assert.That(MatchTimeline.NightStart(S), Is.EqualTo(Min(14, 30)), "Doors latch at 14:30.");
            Assert.That(MatchTimeline.Act2Start(S), Is.EqualTo(Min(20, 30)), "Midnight bell at 20:30.");
            Assert.That(MatchTimeline.PowerCutAt(S), Is.EqualTo(Min(31, 30)), "Power cut at 31:30.");
            Assert.That(MatchTimeline.DawnAt(S), Is.EqualTo(Min(34, 30)));
        }

        [TestCase(0f, MatchPhase.Arrival, 0)]
        [TestCase(59.9f, MatchPhase.Arrival, 0)]
        [TestCase(60f, MatchPhase.Day, 0)]
        [TestCase(779.9f, MatchPhase.Day, 0)]
        [TestCase(780f, MatchPhase.Dusk, 0)]
        [TestCase(870f, MatchPhase.Night, 1)]
        [TestCase(1229.9f, MatchPhase.Night, 1)]
        [TestCase(1230f, MatchPhase.Night, 2)]
        [TestCase(1710f, MatchPhase.Night, 3)]
        [TestCase(2069.9f, MatchPhase.Night, 3)]
        [TestCase(2070f, MatchPhase.Dawn, 0)]
        public void PhaseAndAct_AtBoundaries(float clock, MatchPhase phase, int act)
        {
            var p = MatchTimeline.At(S, clock);
            Assert.That(p.Phase, Is.EqualTo(phase));
            Assert.That(p.Act, Is.EqualTo(act));
        }

        [Test]
        public void Acts_Last6_8_And6Minutes()
        {
            Assert.That(MatchTimeline.At(S, MatchTimeline.NightStart(S)).PhaseLength, Is.EqualTo(Min(6)));
            Assert.That(MatchTimeline.At(S, MatchTimeline.Act2Start(S)).PhaseLength, Is.EqualTo(Min(8)));
            Assert.That(MatchTimeline.At(S, MatchTimeline.Act3Start(S)).PhaseLength, Is.EqualTo(Min(6)));
        }

        [Test]
        public void Dusk_Has12Tolls_7AndAHalfSecondsApart_AllBeforeNightFalls()
        {
            for (int k = 0; k < 12; k++)
                Assert.That(MatchTimeline.TollTime(S, k), Is.EqualTo(MatchTimeline.DuskStart(S) + 7.5f * k).Within(0.001f));
            Assert.That(MatchTimeline.TollTime(S, 11), Is.LessThan(MatchTimeline.NightStart(S)));
        }

        [Test]
        public void PowerCut_FallsInAct3()
        {
            var p = MatchTimeline.At(S, MatchTimeline.PowerCutAt(S));
            Assert.That(p.Phase, Is.EqualTo(MatchPhase.Night));
            Assert.That(p.Act, Is.EqualTo(3));
        }

        [Test]
        public void NextBoundary_StepsThroughEveryPhaseAndAct_ThenStops()
        {
            float clock = 0f;
            var seen = new System.Collections.Generic.List<float>();
            for (int i = 0; i < 10; i++)
            {
                float next = MatchTimeline.NextBoundary(S, clock);
                if (next == clock) break;
                seen.Add(next);
                clock = next;
            }
            Assert.That(seen, Is.EqualTo(new[]
            {
                MatchTimeline.DayStart(S), MatchTimeline.DuskStart(S), MatchTimeline.NightStart(S),
                MatchTimeline.Act2Start(S), MatchTimeline.Act3Start(S), MatchTimeline.DawnAt(S),
            }));
        }

        [Test]
        public void Crossed_CountsAMomentOnce()
        {
            Assert.That(MatchTimeline.Crossed(9f, 10f, 10f), Is.True);
            Assert.That(MatchTimeline.Crossed(10f, 11f, 10f), Is.False);
            Assert.That(MatchTimeline.Crossed(5f, 9.9f, 10f), Is.False);
        }
    }
}
