using UnityEngine;
using UnityEngine.Rendering;

namespace Hollowmere
{
    /// <summary>
    /// Sun, moon, fog and ambient light for each phase. Design: docs/DESIGN.md "Audio and atmosphere" (visual style),
    /// "Day phase" (time signals), "Dusk transition", "The village map" (night visibility) and "The Crossing".
    /// Look pass (M1.5): also the sky, reflections and colour grade. The grade volumes are blended by weight here; the
    /// fog densities, which set how far anyone can see, are unchanged.
    /// </summary>
    public class PhaseLighting : MonoBehaviour
    {
        public Light Sun;
        public Light Moon;

        [Header("Look pass: grade volumes (built by RenderSetup), day sky")]
        public Volume DayGrade;
        public Volume GoldenGrade;
        public Volume DuskGrade;
        public Volume NightGrade;
        public Volume CrossingGrade;
        [Tooltip("Skybox material for daylight; at night the camera clears to the fog colour instead.")]
        public Material DaySky;
        [Tooltip("Reflections from the sky by day, and the faint share left at night.")]
        public float DayReflections = 1f;
        public float NightReflections = 0.1f;

        Material _sky; // a copy, so fading the sky never edits the material asset
        Material Sky => _sky != null ? _sky : DaySky != null ? _sky = new Material(DaySky) : null;

        /// <summary>How much of the sky shows (1 by day, falling to 0 through dusk). Below a sliver the camera clears to fog.</summary>
        public float SkyAmount { get; private set; } = 1f;

        /// <summary>How far into night the look is (0 by day, 1 at night), for grades that only apply in the dark.</summary>
        public float NightAmount { get; private set; }

        [Header("Colours")]
        public Color DayFog = new Color(0.72f, 0.75f, 0.78f);
        public Color GoldenFog = new Color(0.80f, 0.70f, 0.55f);
        public Color NightFog = new Color(0.035f, 0.04f, 0.055f);
        public Color DayAmbient = new Color(0.42f, 0.43f, 0.47f);
        public Color NightAmbient = new Color(0.025f, 0.03f, 0.045f);
        public Color SunMorning = new Color(1f, 0.95f, 0.85f);
        public Color SunGold = new Color(1f, 0.72f, 0.42f);

        [Header("Fog density (exponential squared)")]
        public float DayFogDensity = 0.0025f;
        [Tooltip("DESIGN.md \"The village map\": moonlight and fog cut visibility to about 25 m outdoors.")]
        public float NightFogDensity = 0.07f;
        [Tooltip("DESIGN.md \"Night phase\": in Act 3 the fog thickens.")]
        public float HuntFogDensity = 0.09f;
        [Tooltip("DESIGN.md \"The Crossing\": thick fog on the road.")]
        public float CrossingFogDensity = 0.12f;
        public float MoonIntensity = 0.12f;

        /// <summary>Set by the Crossing.</summary>
        public bool Crossing;

        public void Apply(TimelinePoint now)
        {
            switch (now.Phase)
            {
                case MatchPhase.Arrival: Day(0f); break;
                case MatchPhase.Day: Day(now.Phase01); break;
                case MatchPhase.Dusk: Dusk(now.Phase01); break;
                default: Night(now.Act >= 3 || now.Phase == MatchPhase.Dawn ? HuntFogDensity : NightFogDensity); break;
            }
            if (Crossing) RenderSettings.fogDensity = CrossingFogDensity;
            Weight(CrossingGrade, Crossing ? 1f : 0f);
            var cam = Camera.main;
            if (cam != null) ApplyToCamera(cam);
        }

        /// <summary>Sky or fog colour behind everything, to match the phase. Shot cameras use this too.</summary>
        public void ApplyToCamera(Camera cam)
        {
            bool sky = DaySky != null && SkyAmount > 0.05f;
            cam.clearFlags = sky ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
            cam.backgroundColor = RenderSettings.fogColor;
        }

        /// <summary>The look-pass parts of a phase: grade weights, how much sky shows, how strong reflections are.</summary>
        void Look(float day, float golden, float dusk, float night, float sky)
        {
            Weight(DayGrade, day);
            Weight(GoldenGrade, golden);
            Weight(DuskGrade, dusk);
            Weight(NightGrade, night);
            SkyAmount = sky;
            NightAmount = Mathf.Clamp01(night + dusk * 0.5f);
            if (Sky != null)
            {
                RenderSettings.skybox = Sky;
                Sky.SetFloat("_Exposure", Mathf.Lerp(0.05f, 1f, sky));
                // Tint the sky towards the fog as it fades, so the horizon meets the fog without a seam.
                Sky.SetColor("_Tint", Color.Lerp(RenderSettings.fogColor, new Color(0.5f, 0.5f, 0.5f), sky));
            }
            RenderSettings.reflectionIntensity = Mathf.Lerp(NightReflections, DayReflections, sky);
        }

        static void Weight(Volume v, float w)
        {
            if (v != null) v.weight = w;
        }

        /// <summary>Shadows lengthen and the light turns gold through the day.</summary>
        void Day(float t)
        {
            Sun.enabled = true;
            Moon.enabled = false;
            Sun.transform.rotation = Quaternion.Euler(Mathf.Lerp(55f, 18f, t), -35f, 0f);
            Sun.color = Color.Lerp(SunMorning, SunGold, t);
            Sun.intensity = Mathf.Lerp(1.3f, 1.1f, t);
            RenderSettings.fogColor = Color.Lerp(DayFog, GoldenFog, t * t);
            RenderSettings.fogDensity = DayFogDensity;
            RenderSettings.ambientLight = DayAmbient;
            Look(1f - t * t, t * t, 0f, 0f, 1f);
        }

        /// <summary>Orange fading to grey over the 90 seconds as colour drains from the world.</summary>
        void Dusk(float t)
        {
            Sun.enabled = t < 0.97f;
            Moon.enabled = true;
            Sun.transform.rotation = Quaternion.Euler(Mathf.Lerp(18f, 2f, t), -35f, 0f);
            Sun.color = Color.Lerp(SunGold, new Color(0.5f, 0.5f, 0.55f), t);
            Sun.intensity = Mathf.Lerp(1.1f, 0f, t);
            Moon.intensity = Mathf.Lerp(0f, MoonIntensity, t);
            RenderSettings.fogColor = Color.Lerp(GoldenFog, NightFog, t);
            RenderSettings.fogDensity = Mathf.Lerp(DayFogDensity, NightFogDensity, t * t);
            RenderSettings.ambientLight = Color.Lerp(DayAmbient, NightAmbient, t);
            // Gold drains into the grey dusk grade over the first half, then night takes over.
            float first = Mathf.Clamp01(t * 2f), second = Mathf.Clamp01(t * 2f - 1f);
            Look(0f, 1f - first, first - second, second, 1f - Mathf.SmoothStep(0f, 1f, t));
        }

        void Night(float density)
        {
            Sun.enabled = false;
            Moon.enabled = true;
            Moon.intensity = MoonIntensity;
            RenderSettings.fogColor = NightFog;
            RenderSettings.fogDensity = Mathf.MoveTowards(RenderSettings.fogDensity, density, 0.02f * Time.deltaTime);
            RenderSettings.ambientLight = NightAmbient;
            Look(0f, 0f, 0f, 1f, 0f);
        }
    }
}
