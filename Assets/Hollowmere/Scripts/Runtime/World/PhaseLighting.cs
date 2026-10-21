using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Sun, moon, fog and ambient light for each phase. Design: docs/DESIGN.md "Audio and atmosphere" (visual style),
    /// "Day phase" (time signals), "Dusk transition", "The village map" (night visibility) and "The Crossing".
    /// </summary>
    public class PhaseLighting : MonoBehaviour
    {
        public Light Sun;
        public Light Moon;

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
            var cam = Camera.main;
            if (cam != null) cam.backgroundColor = RenderSettings.fogColor;
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
        }

        void Night(float density)
        {
            Sun.enabled = false;
            Moon.enabled = true;
            Moon.intensity = MoonIntensity;
            RenderSettings.fogColor = NightFog;
            RenderSettings.fogDensity = Mathf.MoveTowards(RenderSettings.fogDensity, density, 0.02f * Time.deltaTime);
            RenderSettings.ambientLight = NightAmbient;
        }
    }
}
