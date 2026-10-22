using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Hollowmere.EditorTools.BuilderKit;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Lighting and atmosphere for the look pass (M1.5): grade volumes, sky and reflections, shadowed and flickering
    /// lights, hearth flames, and reflection probes that keep the sky out of the houses. The pipeline and the assets
    /// these use come from RenderSetup. Nothing here changes a light's on/off state or the fog.
    /// </summary>
    public static partial class GreyboxVillageBuilder
    {
        /// <summary>The always-on base look and the phase grades PhaseLighting blends between.</summary>
        static void BuildGrades(PhaseLighting lighting)
        {
            var root = new GameObject("PostProcessing").transform;
            Volume Grade(string name, VolumeProfile profile, int priority, float weight)
            {
                var v = Node(root, name).gameObject.AddComponent<Volume>();
                v.isGlobal = true;
                v.sharedProfile = profile;
                v.priority = priority;
                v.weight = weight;
                return v;
            }
            Grade("Base", s_render.Base, 0, 1f);
            lighting.DayGrade = Grade("Day", s_render.Day, 1, 1f);
            lighting.GoldenGrade = Grade("Golden", s_render.Golden, 2, 0f);
            lighting.DuskGrade = Grade("Dusk", s_render.Dusk, 3, 0f);
            lighting.NightGrade = Grade("Night", s_render.Night, 4, 0f);
            lighting.CrossingGrade = Grade("Crossing", s_render.Crossing, 5, 0f);
            lighting.DaySky = s_render.Sky;
            s_interiorNight = Grade("InteriorNight", s_render.InteriorNight, 10, 0f);
        }

        static Volume s_interiorNight;

        /// <summary>The indoor night grade, faded in by InteriorGrade while the camera is inside one of the houses.</summary>
        static void BuildInteriorGrade(PhaseLighting lighting, House[] houses)
        {
            var grade = lighting.gameObject.AddComponent<InteriorGrade>();
            grade.Lighting = lighting;
            grade.InteriorNight = s_interiorNight;
            grade.Houses = houses;
        }

        /// <summary>A few logs where the hearth's fire burns; they stay when the fire is out.</summary>
        static void HearthLogs(Transform hearth)
        {
            var logs = Node(hearth, "Logs", new Vector3(0f, FloorTop, 1.0f));
            PropKit.Fit(logs, "Log0", PropKit.FallenTrunk, new Bounds(new Vector3(0f, 0.06f, -0.06f), new Vector3(0.62f, 0.12f, 0.12f)), 8f);
            PropKit.Fit(logs, "Log1", PropKit.FallenTrunk, new Bounds(new Vector3(0f, 0.06f, 0.08f), new Vector3(0.6f, 0.12f, 0.12f)), 174f);
            PropKit.Fit(logs, "Log2", PropKit.FallenTrunk, new Bounds(new Vector3(0.02f, 0.16f, 0.01f), new Vector3(0.5f, 0.11f, 0.11f)), 95f);
        }

        /// <summary>
        /// A window's wooden frame, cross bars and sill, on both faces of the wall; the pane stops casting shadows so
        /// a lit room's light spills out through it. center/size are the pane's (local to the shell); alongX: in a
        /// front or back wall.
        /// </summary>
        static void WindowFrame(Transform shell, GameObject pane, bool alongX)
        {
            pane.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
            var wood = Mat("WindowFrame", new Color(0.2f, 0.15f, 0.1f));
            var c = pane.transform.localPosition;
            var s = pane.transform.localScale;
            float w = alongX ? s.x : s.z, h = s.y, d = (alongX ? s.z : s.x) + 0.06f;
            Vector3 Local(float across, float up, float depth) => alongX ? new Vector3(across, up, depth) : new Vector3(depth, up, across);
            var frame = Node(shell, pane.name + "_Frame", c);
            const float t = 0.07f;
            Box(frame, "Top", Vector3.zero + Vector3.up * (h * 0.5f - t * 0.5f), Local(w, t, d), wood, false);
            Box(frame, "Bottom", Vector3.down * (h * 0.5f - t * 0.5f), Local(w, t, d), wood, false);
            Box(frame, "Left", alongX ? Vector3.left * (w * 0.5f - t * 0.5f) : Vector3.back * (w * 0.5f - t * 0.5f), Local(t, h, d), wood, false);
            Box(frame, "Right", alongX ? Vector3.right * (w * 0.5f - t * 0.5f) : Vector3.forward * (w * 0.5f - t * 0.5f), Local(t, h, d), wood, false);
            Box(frame, "Mullion", Vector3.zero, Local(0.045f, h, d - 0.02f), wood, false);
            Box(frame, "Transom", Vector3.zero, Local(w, 0.045f, d - 0.02f), wood, false);
            Box(frame, "Sill", Vector3.down * (h * 0.5f + 0.03f), Local(w + 0.16f, 0.05f, d + 0.12f), wood, false);
        }

        /// <summary>The day sky and its reflections; PhaseLighting fades both at dusk.</summary>
        static void ApplySky()
        {
            RenderSettings.skybox = s_render.Sky;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            RenderSettings.customReflectionTexture = s_render.DayReflections;
            RenderSettings.reflectionIntensity = 1f;
        }

        /// <summary>Post-processing on, sky behind (PhaseLighting switches to the fog colour at night).</summary>
        static void LookCamera(Camera cam)
        {
            cam.clearFlags = CameraClearFlags.Skybox;
            var data = cam.GetUniversalAdditionalCameraData();
            data.renderPostProcessing = true;
            data.antialiasing = AntialiasingMode.None; // 4x MSAA from the pipeline asset
            data.renderShadows = true;
        }

        /// <summary>Soft shadows at a given atlas tier: room lights would otherwise shine through walls into the street.</summary>
        static void SoftShadows(Light light, int tier)
        {
            light.shadows = LightShadows.Soft;
            light.shadowNearPlane = 0.1f;
            light.shadowBias = 0.05f;
            light.shadowNormalBias = 0.4f;
            var data = new SerializedObject(light.GetUniversalAdditionalLightData());
            data.FindProperty("m_AdditionalLightsShadowResolutionTier").intValue = tier;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        static void Flicker(Light light, float amount, float speed)
        {
            var f = light.gameObject.AddComponent<LightFlicker>();
            f.Amount = amount;
            f.Speed = speed;
        }

        /// <summary>The fire's crackle, playing whenever the fire object is active. Atmosphere only: not a NoiseSystem sound.</summary>
        static void FireSound(Transform fire)
        {
            if (s_sounds == null || s_sounds.HearthFire == null) return;
            var s = fire.gameObject.AddComponent<AudioSource>();
            s.clip = s_sounds.HearthFire;
            s.loop = true;
            s.playOnAwake = true;
            s.volume = 0.55f;
            s.spatialBlend = 1f;
            s.rolloffMode = AudioRolloffMode.Logarithmic;
            s.minDistance = 1f;
            s.maxDistance = 12f;
        }

        /// <summary>Additive flame particles over the hearth's ember glow. They run whenever the fire object is active.</summary>
        static void FireParticles(Transform fire)
        {
            var embers = fire.Find("Flames");
            if (embers != null)
            {
                embers.localPosition = new Vector3(0f, -0.17f, 0f);
                embers.localScale = new Vector3(0.34f, 0.07f, 0.22f); // glowing embers among the logs
            }

            // Shapes emit along their local +z, so the emitter is turned to face up.
            var go = Node(fire, "FlameParticles", new Vector3(0f, -0.17f, 0f), Quaternion.Euler(-90f, 0f, 0f)).gameObject;
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 1f;
            main.loop = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.55f, 0.95f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.35f, 0.75f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.22f, 0.42f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.62f, 0.22f), new Color(1f, 0.82f, 0.4f));
            main.maxParticles = 80;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;

            var emission = ps.emission;
            emission.rateOverTime = 38f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(0.5f, 0.22f, 0.04f); // x across the hearth, y front to back (turned), z thin

            var colour = ps.colorOverLifetime;
            colour.enabled = true;
            var g = new Gradient();
            g.SetKeys(
                new[] { new GradientColorKey(new Color(1f, 0.85f, 0.5f), 0f), new GradientColorKey(new Color(1f, 0.45f, 0.12f), 0.45f), new GradientColorKey(new Color(0.6f, 0.12f, 0.04f), 1f) },
                new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.9f, 0.12f), new GradientAlphaKey(0.5f, 0.55f), new GradientAlphaKey(0f, 1f) });
            colour.color = g;

            var size = ps.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0.25f));

            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.35f;
            noise.frequency = 1.6f;
            noise.scrollSpeed = 0.8f;

            var r = go.GetComponent<ParticleSystemRenderer>();
            r.sharedMaterial = s_render.FireParticles;
            r.renderMode = ParticleSystemRenderMode.Billboard;
            r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false;
        }

        /// <summary>
        /// A box-projected probe filling the house with a dim warm cube, so floors and furniture reflect a room rather
        /// than the sky. Outside walls fall outside the box and keep the sky's reflections.
        /// </summary>
        static void InteriorProbe(Transform root)
        {
            var probe = Node(root, "InteriorReflections", new Vector3(0f, HouseH * 0.5f, 0f)).gameObject.AddComponent<ReflectionProbe>();
            probe.mode = ReflectionProbeMode.Custom;
            probe.customBakedTexture = s_render.InteriorReflections;
            probe.size = new Vector3(HouseW - 2f * WallT + 0.1f, HouseH + 0.1f, HouseD - 2f * WallT + 0.1f);
            probe.center = Vector3.zero;
            probe.boxProjection = true;
            probe.blendDistance = 0.2f;
            probe.importance = 10;
            probe.intensity = 1f;
        }
    }
}
