using System.Collections.Generic;
using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The Crossing: once the gate is open, the fog road to the old bridge. Walking only, footsteps behind you,
    /// and if you look back for half a second the Hollow takes you. Reaching the bridge means you escaped.
    /// Design: docs/DESIGN.md "The Crossing".
    /// </summary>
    public class Crossing : MonoBehaviour
    {
        public MatchDirector Director;
        public VillageLayout Layout;
        public PhaseLighting Lighting;
        [Tooltip("Seconds between the footsteps that follow you up the road.")]
        public float FollowStepSeconds = 1.1f;

        readonly Dictionary<PlayerAgent, float> _lookingBack = new Dictionary<PlayerAgent, float>();
        float _nextStep;

        static readonly Vector3 RoadAhead = Vector3.forward; // the road runs north from the gate to the bridge

        public bool IsOnRoad(PlayerAgent player) => _lookingBack.ContainsKey(player);

        void Update()
        {
            // Only after the ritual: on arrival the gate is open too, and the guests start on this same road.
            if (Director == null || Director.Outcome != MatchOutcome.Running || !Director.RitualComplete) return;

            foreach (var p in Director.Players)
            {
                if (p.Condition != PlayerCondition.Alive) continue;
                float z = p.transform.position.z;
                if (!IsOnRoad(p) && z > Layout.GateLineZ + 1.5f) Enter(p);
                if (!IsOnRoad(p)) continue;

                float angle = CrossingRules.AngleFromRoad(p.Head.forward, RoadAhead);
                _lookingBack[p] = CrossingRules.UpdateLookBack(_lookingBack[p], angle, Time.deltaTime);
                if (CrossingRules.IsTaken(_lookingBack[p])) Take(p);
                else if (z >= Layout.BridgeZ) Escape(p);
            }

            FollowSomeone();
        }

        void Enter(PlayerAgent p)
        {
            _lookingBack[p] = 0f;
            p.Motor.WalkOnly = true;
            Lighting.Crossing = true;
            p.Notify("Walk. Whatever you hear behind you, do not look back.");
        }

        void Take(PlayerAgent p)
        {
            p.SetCondition(PlayerCondition.Taken, $"Taken on the fog road at {MatchDirector.FormatClock(Director.Clock)}. You looked back.");
            p.Frozen = true;
            SoundBank.PlayAt(SoundBank.Breath, p.Head.position - p.Head.forward * 0.5f, 1f, 10f);
            p.Notify("Something behind you was waiting for you to turn around.");
        }

        void Escape(PlayerAgent p)
        {
            p.SetCondition(PlayerCondition.Escaped, $"Escaped across the old bridge at {MatchDirector.FormatClock(Director.Clock)}.");
            p.Frozen = true;
            p.Notify("Your feet find the planks of the bridge.");
        }

        /// <summary>Footsteps keep pace a few metres behind whoever is on the road, and sometimes something breathes.</summary>
        void FollowSomeone()
        {
            if (Time.time < _nextStep) return;
            foreach (var p in Director.Players)
            {
                if (p.Condition != PlayerCondition.Alive || !IsOnRoad(p)) continue;
                var behind = p.transform.position - RoadAhead * 4f + Vector3.right * Mathf.Sin(Time.time * 0.7f) * 1.5f;
                bool breathe = Mathf.PerlinNoise(Time.time * 0.3f, 0.5f) > 0.72f;
                SoundBank.PlayAt(breathe ? SoundBank.Breath : SoundBank.Step, behind, breathe ? 0.5f : 0.8f, 12f);
            }
            _nextStep = Time.time + FollowStepSeconds * Mathf.Lerp(0.8f, 1.3f, Mathf.PerlinNoise(Time.time, 0.2f));
        }
    }
}
