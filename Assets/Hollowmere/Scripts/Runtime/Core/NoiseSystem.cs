using System;
using UnityEngine;

namespace Hollowmere
{
    public enum NoiseKind { Footstep, Voice, Door, Object, Puzzle, Bell, Breath, Ritual }

    public struct NoiseEvent
    {
        public Vector3 Position;
        public float Radius;      // how far an ordinary listener can hear it outdoors, in metres
        public NoiseKind Kind;
        public int SourcePlayer;  // player index, or -1 for world sounds
    }

    /// <summary>
    /// Noise radii in metres. Values come from docs/DESIGN.md "Player systems" unless marked as placeholders,
    /// which DESIGN.md doesn't give a number for yet; playtests will tune them.
    /// </summary>
    public static class NoiseRadius
    {
        public const float CrouchWalk = 0f;   // silent
        public const float Walk = 5f;
        public const float Sprint = 25f;
        public const float Jump = 10f;        // "vault or jump"

        // Placeholders.
        public const float DoorCreak = 12f;   // a quick open creaks
        public const float DoorQuiet = 0f;    // holding the button opens slowly and quietly
        public const float DroppedItem = 6f;
        public const float Gasp = 10f;        // holding your breath too long ends with a loud gasp
        public const float FuseReset = 4f;
        public const float PuzzleClunk = 4f;  // a wrong dial or symbol
        public const float PuzzleSolved = 10f;
        public const float LoudPuzzle = 40f;  // HouseDef.LoudPuzzle, e.g. the Organist's pump organ
        public const float AltarWrongSlot = 40f;
        public const float Ladder = 5f;       // climbing a ladder or through a hatch: like walking (not in DESIGN.md yet)
    }

    /// <summary>
    /// Every gameplay sound the Hollow might hear goes through here, so hearing rules live in one place.
    /// Design reference: docs/DESIGN.md, "Player systems" (noise radii) and "The monster" (hearing ranges).
    /// </summary>
    public static class NoiseSystem
    {
        public static event Action<NoiseEvent> OnNoise;

        // Static events survive play-mode restarts when domain reload is disabled, so clear them on load.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => OnNoise = null;

        public static void Emit(Vector3 position, float radius, NoiseKind kind, int sourcePlayer = -1)
        {
            if (radius <= 0f) return;
            OnNoise?.Invoke(new NoiseEvent
            {
                Position = position,
                Radius = radius,
                Kind = kind,
                SourcePlayer = sourcePlayer,
            });
        }

        /// <summary>Voice loudness (0..1 RMS from the mic) to noise radius: whisper 2 m, talk 10 m, shout 30 m.</summary>
        public static float VoiceRadius(float rms)
        {
            if (rms < 0.01f) return 0f;
            if (rms < 0.03f) return 2f;
            if (rms < 0.12f) return Mathf.Lerp(2f, 10f, (rms - 0.03f) / 0.09f);
            return Mathf.Lerp(10f, 30f, Mathf.Clamp01((rms - 0.12f) / 0.2f));
        }
    }
}
