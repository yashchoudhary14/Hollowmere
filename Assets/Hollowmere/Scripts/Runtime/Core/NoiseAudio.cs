using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Plays a sound for every NoiseSystem event, so what the player hears is what the Hollow can hear.
    /// Silent actions (crouch steps, slow doors) emit no event and so make no sound. Footsteps sound like the surface
    /// they land on.
    /// </summary>
    public class NoiseAudio : MonoBehaviour
    {
        void OnEnable() => NoiseSystem.OnNoise += Play;
        void OnDisable() => NoiseSystem.OnNoise -= Play;

        void Play(NoiseEvent e)
        {
            AudioClip clip;
            float volume = Mathf.Clamp(e.Radius / 25f, 0.15f, 1f);
            switch (e.Kind)
            {
                case NoiseKind.Footstep:
                    var surface = SoundBank.SurfaceAt(e.Position);
                    clip = e.Radius == NoiseRadius.Jump ? SoundBank.Landing(surface) : SoundBank.Footstep(surface);
                    volume = Mathf.Clamp(e.Radius / 25f, 0.35f, 1f) * 0.8f;
                    break;
                case NoiseKind.Door: clip = SoundBank.Creak; break;
                case NoiseKind.Object: clip = SoundBank.Thud; break;
                case NoiseKind.Puzzle: clip = e.Radius >= NoiseRadius.PuzzleSolved ? SoundBank.Chime : SoundBank.Clunk; break;
                case NoiseKind.Breath: clip = SoundBank.Gasp; break;
                case NoiseKind.Ritual: clip = SoundBank.HeavyThud; volume = 1f; break;
                default: return; // voices are heard live; bells ring from the tower
            }
            SoundBank.PlayAt(clip, e.Position, volume, Mathf.Max(12f, e.Radius * 1.5f));
        }
    }
}
