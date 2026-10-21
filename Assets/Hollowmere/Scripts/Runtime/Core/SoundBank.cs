using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Placeholder sounds synthesised at runtime, so the greybox needs no audio assets.
    /// Milestone 7's audio pass replaces these clips. Generated once and kept for the session.
    /// </summary>
    public static class SoundBank
    {
        const int Rate = 44100;

        static AudioClip s_bell, s_creak, s_step, s_thud, s_gasp, s_chime, s_clunk, s_knock, s_breath, s_chant;

        public static AudioClip Bell => s_bell != null ? s_bell : s_bell = Make("Bell", 5f, 1, FillBell);
        public static AudioClip Creak => s_creak != null ? s_creak : s_creak = Make("Creak", 0.7f, 2, FillCreak);
        public static AudioClip Step => s_step != null ? s_step : s_step = Make("Step", 0.15f, 3, FillStep);
        public static AudioClip Thud => s_thud != null ? s_thud : s_thud = Make("Thud", 0.35f, 4, FillThud);
        public static AudioClip Gasp => s_gasp != null ? s_gasp : s_gasp = Make("Gasp", 0.8f, 5, FillGasp);
        public static AudioClip Chime => s_chime != null ? s_chime : s_chime = Make("Chime", 1.6f, 6, FillChime);
        public static AudioClip Clunk => s_clunk != null ? s_clunk : s_clunk = Make("Clunk", 0.2f, 7, FillClunk);
        public static AudioClip Knock => s_knock != null ? s_knock : s_knock = Make("Knock", 1.0f, 8, FillKnock);
        public static AudioClip Breath => s_breath != null ? s_breath : s_breath = Make("HollowBreath", 2.5f, 9, FillBreath);
        public static AudioClip Chant => s_chant != null ? s_chant : s_chant = Make("Chant", 4f, 10, FillChant);

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
