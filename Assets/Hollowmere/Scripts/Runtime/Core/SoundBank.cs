using UnityEngine;

namespace Hollowmere
{
    /// <summary>What a footstep lands on, for choosing its sound.</summary>
    public enum Surface { Wood, Stone, Dirt, Leaves }

    /// <summary>
    /// The game's sounds. Recorded clips come from the scene's <see cref="SoundLibrary"/> (set by AmbienceDirector);
    /// anything the library lacks falls back to a clip synthesised at runtime, so nothing ever goes silent.
    /// Variations play in turn. Synthesised clips are generated once and kept for the session.
    /// </summary>
    public static class SoundBank
    {
        const int Rate = 44100;

        /// <summary>The recorded sounds, or null for synthesised placeholders only.</summary>
        public static SoundLibrary Library;

        static AudioClip s_bell, s_creak, s_step, s_thud, s_gasp, s_chime, s_clunk, s_knock, s_breath, s_chant;
        static AudioClip s_knocking;
        static AudioClip s_knockingSource;
        static int s_turn;

        // Clips and the library are per session; clear them when play starts without a domain reload.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            Library = null;
            s_knocking = s_knockingSource = null;
            s_turn = 0;
        }

        static AudioClip SynthBell => s_bell != null ? s_bell : s_bell = Make("Bell", 5f, 1, FillBell);
        static AudioClip SynthCreak => s_creak != null ? s_creak : s_creak = Make("Creak", 0.7f, 2, FillCreak);
        static AudioClip SynthStep => s_step != null ? s_step : s_step = Make("Step", 0.15f, 3, FillStep);
        static AudioClip SynthThud => s_thud != null ? s_thud : s_thud = Make("Thud", 0.35f, 4, FillThud);
        static AudioClip SynthGasp => s_gasp != null ? s_gasp : s_gasp = Make("Gasp", 0.8f, 5, FillGasp);
        static AudioClip SynthChime => s_chime != null ? s_chime : s_chime = Make("Chime", 1.6f, 6, FillChime);
        static AudioClip SynthClunk => s_clunk != null ? s_clunk : s_clunk = Make("Clunk", 0.2f, 7, FillClunk);
        static AudioClip SynthKnock => s_knock != null ? s_knock : s_knock = Make("Knock", 1.0f, 8, FillKnock);
        static AudioClip SynthBreath => s_breath != null ? s_breath : s_breath = Make("HollowBreath", 2.5f, 9, FillBreath);
        static AudioClip SynthChant => s_chant != null ? s_chant : s_chant = Make("Chant", 4f, 10, FillChant);

        /// <summary>The bell's ring (synthesised; a recorded strike is layered on top, see <see cref="BellStrike"/>).</summary>
        public static AudioClip Bell => SynthBell;
        /// <summary>The recorded strike of a toll, or null.</summary>
        public static AudioClip BellStrike => Next(Library != null ? Library.BellStrike : null);
        public static AudioClip Creak => Next(Library != null ? Library.DoorCreak : null) ?? SynthCreak;
        public static AudioClip Step => Footstep(Surface.Dirt);
        public static AudioClip Thud => Next(Library != null ? Library.Thud : null) ?? SynthThud;
        public static AudioClip HeavyThud => Next(Library != null ? Library.HeavyThud : null) ?? SynthThud;
        public static AudioClip Gasp => Next(Library != null ? Library.Gasp : null) ?? SynthGasp;
        public static AudioClip Chime => Next(Library != null ? Library.Chime : null) ?? SynthChime;
        public static AudioClip Clunk => Next(Library != null ? Library.Clunk : null) ?? SynthClunk;
        public static AudioClip Breath => Next(Library != null ? Library.HollowBreath : null) ?? SynthBreath;
        /// <summary>The Hollow's exhale as it starts a hunt.</summary>
        public static AudioClip Exhale => Next(Library != null ? Library.HollowExhale : null) ?? Breath;
        public static AudioClip Chant => Library != null && Library.Chant != null ? Library.Chant : SynthChant;

        /// <summary>The village gate swinging, and the pitch to play it at (the synthesised creak is pitched down).</summary>
        public static AudioClip GateGroan => Next(Library != null ? Library.GateGroan : null) ?? SynthCreak;
        public static float GatePitch => Library != null && Library.GateGroan != null && Library.GateGroan.Length > 0 ? 1f : 0.55f;

        /// <summary>Knocking at a door: a recorded knock repeated three times, or the synthesised knocking.</summary>
        public static AudioClip Knock
        {
            get
            {
                var single = Next(Library != null ? Library.Knock : null);
                if (single == null) return SynthKnock;
                if (s_knocking == null || s_knockingSource != single)
                {
                    s_knocking = Repeat(single, 3, 0.32f) ?? single;
                    s_knockingSource = single;
                }
                return s_knocking;
            }
        }

        /// <summary>A footstep on the given surface (any surface's steps if that one has none; synthesised last).</summary>
        public static AudioClip Footstep(Surface surface)
        {
            var lib = Library;
            if (lib == null) return SynthStep;
            AudioClip[] clips;
            switch (surface)
            {
                case Surface.Wood: clips = lib.StepWood; break;
                case Surface.Stone: clips = lib.StepStone; break;
                case Surface.Leaves: clips = lib.StepLeaves; break;
                default: clips = lib.StepDirt; break;
            }
            return Next(clips) ?? Next(lib.StepDirt) ?? SynthStep;
        }

        /// <summary>Landing from a jump on the given surface (falls back to a footstep).</summary>
        public static AudioClip Landing(Surface surface)
        {
            var lib = Library;
            if (lib == null) return SynthStep;
            AudioClip[] clips;
            switch (surface)
            {
                case Surface.Wood: clips = lib.LandWood; break;
                case Surface.Stone: clips = lib.LandStone; break;
                case Surface.Leaves: clips = lib.LandLeaves; break;
                default: clips = lib.LandDirt; break;
            }
            return Next(clips) ?? Footstep(surface);
        }

        /// <summary>The surface under a point: the renderer of whatever collider is just below it.</summary>
        public static Surface SurfaceAt(Vector3 position)
        {
            if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out var hit, 2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                var r = hit.collider.GetComponent<Renderer>();
                if (r != null && r.sharedMaterial != null) return SurfaceFor(r.sharedMaterial.name, hit.point);
            }
            return SurfaceFor("", position);
        }

        /// <summary>
        /// Footstep surface from a builder material name (BuilderKit.Mat names): floorboards and planks are wood,
        /// paving and stone are stone, ground is dirt in the village and leaves in the forest beyond the wall.
        /// </summary>
        public static Surface SurfaceFor(string materialName, Vector3 position)
        {
            var n = materialName ?? "";
            if (n.Contains("Floor") || n.Contains("Wood") || n.Contains("Plank") || n.Contains("Ceiling")) return Surface.Wood;
            if (n.Contains("Paving") || n.Contains("Stone") || n.Contains("Chimney")) return Surface.Stone;
            if (n.Contains("Leaf") || n.Contains("Leaves")) return Surface.Leaves;
            bool beyondWall = Mathf.Abs(position.x) > 150f || Mathf.Abs(position.z) > 150f;
            bool onRoad = n.Contains("Road");
            return beyondWall && !onRoad ? Surface.Leaves : Surface.Dirt;
        }

        /// <summary>The next of a set of variations, in turn; null if there are none.</summary>
        static AudioClip Next(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0) return null;
            s_turn++;
            for (int i = 0; i < clips.Length; i++)
            {
                var c = clips[(s_turn + i) % clips.Length];
                if (c != null) return c;
            }
            return null;
        }

        /// <summary>A clip played `times` times, `gap` seconds apart, as one clip (needs a clip loaded into memory).</summary>
        public static AudioClip Repeat(AudioClip clip, int times, float gap)
        {
            if (clip.loadType == AudioClipLoadType.Streaming) return null;
            if (clip.loadState != AudioDataLoadState.Loaded && !clip.LoadAudioData()) return null;
            int channels = clip.channels, rate = clip.frequency;
            var src = new float[clip.samples * channels];
            if (!clip.GetData(src, 0)) return null;
            int step = Mathf.RoundToInt(gap * rate) * channels;
            var dst = new float[step * (times - 1) + src.Length];
            for (int k = 0; k < times; k++)
            {
                float gain = 1f - 0.12f * k; // each knock a little softer
                for (int i = 0; i < src.Length; i++) dst[k * step + i] += src[i] * gain;
            }
            var result = AudioClip.Create(clip.name + "_x" + times, dst.Length / channels, channels, rate, false);
            result.SetData(dst, 0);
            return result;
        }

        /// <summary>Plays a clip once at a point in the world, as a short-lived 3D source.</summary>
        public static void PlayAt(AudioClip clip, Vector3 position, float volume, float maxDistance = 40f, float pitch = 1f)
        {
            var go = new GameObject($"Sound {clip.name}");
            go.transform.position = position;
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = Mathf.Clamp01(volume);
            source.pitch = pitch;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1f;
            source.maxDistance = Mathf.Max(2f, maxDistance);
            source.Play();
            Object.Destroy(go, clip.length / Mathf.Max(0.1f, pitch) + 0.2f);
        }

        delegate void Filler(float[] data, System.Random rng);

        static AudioClip Make(string name, float seconds, int seed, Filler fill)
        {
            var data = new float[Mathf.CeilToInt(seconds * Rate)];
            fill(data, new System.Random(seed));
            var clip = AudioClip.Create(name, data.Length, 1, Rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static float Noise(System.Random rng) => (float)(rng.NextDouble() * 2.0 - 1.0);
        static float Sin(float hz, float t) => Mathf.Sin(2f * Mathf.PI * hz * t);

        static void FillBell(float[] d, System.Random rng)
        {
            // A church-bell-like spectrum on G3: hum, prime, minor third, fifth, octave and two upper partials.
            float[] ratio = { 0.5f, 1f, 1.183f, 1.506f, 2f, 2.514f, 2.662f };
            float[] amp = { 0.6f, 1f, 0.6f, 0.4f, 0.35f, 0.2f, 0.15f };
            float[] decay = { 3.5f, 2.5f, 2f, 1.6f, 1.2f, 0.9f, 0.7f };
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate, v = 0f;
                for (int k = 0; k < ratio.Length; k++) v += amp[k] * Mathf.Exp(-t / decay[k]) * Sin(196f * ratio[k], t);
                d[i] = v * Mathf.Min(1f, t * 300f) * 0.3f;
            }
        }

        static void FillCreak(float[] d, System.Random rng)
        {
            float phase = 0f, smooth = 0f;
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                float hz = 95f + 45f * Mathf.Sin(2f * Mathf.PI * 2.7f * t) + 20f * Mathf.Sin(2f * Mathf.PI * 11f * t);
                phase += hz / Rate;
                smooth += 0.3f * (Noise(rng) - smooth);
                float env = Mathf.Min(1f, t * 20f) * Mathf.Clamp01((0.7f - t) * 6f);
                float pulse = Mathf.Pow(Mathf.Abs(Mathf.Sin(Mathf.PI * phase)), 12f);
                d[i] = env * (0.7f * pulse * Sin(700f, t) + 0.25f * smooth) * 0.8f;
            }
        }

        static void FillStep(float[] d, System.Random rng)
        {
            float y = 0f;
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                y += 0.12f * (Noise(rng) - y);
                d[i] = y * Mathf.Exp(-t * 45f) * 2.2f + Sin(80f, t) * Mathf.Exp(-t * 60f) * 0.3f;
            }
        }

        static void FillThud(float[] d, System.Random rng)
        {
            float y = 0f;
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                y += 0.1f * (Noise(rng) - y);
                d[i] = Sin(70f, t) * Mathf.Exp(-t * 14f) * 0.7f + y * Mathf.Exp(-t * 30f) * 1.5f;
            }
        }

        static void FillGasp(float[] d, System.Random rng)
        {
            float lo = 0f;
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                float n = Noise(rng);
                lo += 0.25f * (n - lo);
                float env = t < 0.08f ? t / 0.08f : Mathf.Exp(-(t - 0.08f) * 4f);
                d[i] = (n - lo) * env * 0.6f;
            }
        }

        static void FillChime(float[] d, System.Random rng)
        {
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                d[i] = (Sin(880f, t) * Mathf.Exp(-t * 2.5f) + 0.5f * Sin(1320f, t) * Mathf.Exp(-t * 3f)
                        + 0.25f * Sin(1760f, t) * Mathf.Exp(-t * 4f)) * Mathf.Min(1f, t * 400f) * 0.35f;
            }
        }

        static void FillClunk(float[] d, System.Random rng)
        {
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                d[i] = Sin(140f, t) * Mathf.Exp(-t * 25f) * 0.7f + Noise(rng) * Mathf.Exp(-t * 60f) * 0.3f;
            }
        }

        static void FillKnock(float[] d, System.Random rng)
        {
            float[] at = { 0f, 0.28f, 0.56f };
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate, v = 0f;
                foreach (var k in at)
                {
                    float u = t - k;
                    if (u < 0f) continue;
                    v += Sin(105f, u) * Mathf.Exp(-u * 28f) + 0.25f * Noise(rng) * Mathf.Exp(-u * 90f);
                }
                d[i] = v * 0.7f;
            }
        }

        /// <summary>A low, looping drone of voices for the chant. Partials are whole cycles over 4 s so it loops cleanly.</summary>
        static void FillChant(float[] d, System.Random rng)
        {
            float[] hz = { 110f, 165f, 220f, 277.5f };
            float[] amp = { 1f, 0.6f, 0.45f, 0.2f };
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate, v = 0f;
                for (int k = 0; k < hz.Length; k++)
                    v += amp[k] * (1f + 0.25f * Mathf.Sin(2f * Mathf.PI * 0.25f * (k + 1) * t)) * Sin(hz[k], t);
                d[i] = v * 0.15f;
            }
        }

        static void FillBreath(float[] d, System.Random rng)
        {
            // Wet, slow breathing: low-passed noise swelling twice.
            float y = 0f;
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                y += 0.04f * (Noise(rng) - y);
                float swell = Mathf.Pow(Mathf.Max(0f, Mathf.Sin(Mathf.PI * t / 1.25f)), 2f);
                d[i] = y * swell * 4f * (1f + 0.3f * Sin(31f, t));
            }
        }
    }
}
