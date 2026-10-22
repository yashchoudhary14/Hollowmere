using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Hollowmere's rendering setup for the look pass (M1.5), all generated here so nothing is tuned by hand:
    /// - Its own URP asset and renderer for the PC quality level, copied from the template's and tuned for a high-end
    ///   PC: Forward+, HDR (64-bit), 4x MSAA, 4 soft shadow cascades at 4096 out to 120 m, shadowed point lights,
    ///   HDR colour grading, and SSAO.
    /// - Volume profiles: a base look (ACES, bloom, grain, vignette, a muted grade) plus Day, Golden, Dusk, Night and
    ///   Crossing grades that PhaseLighting blends between.
    /// - The day sky (overcast HDRI), a dark warm cubemap for reflections inside houses, the flashlight's cookie and a
    ///   soft flame texture for fire particles.
    /// Idempotent: every run re-applies the same settings.
    /// </summary>
    public static class RenderSetup
    {
        const string Dir = "Assets/Hollowmere/Settings/Rendering";
        public const string PipelinePath = Dir + "/Hollowmere_PC_RPAsset.asset";
        public const string RendererPath = Dir + "/Hollowmere_PC_Renderer.asset";
        const string TemplatePipeline = "Assets/Settings/PC_RPAsset.asset";
        const string TemplateRenderer = "Assets/Settings/PC_Renderer.asset";
        const string ProfileDir = "Assets/Hollowmere/Settings/Volumes";
        const string SkyPath = "Assets/Hollowmere/Materials/Sky/DaySky.mat";
        public const string DayHdri = "Assets/ThirdParty/PolyHaven/overcast_soil_puresky/overcast_soil_puresky_2k.hdr";
        const string InteriorCubePath = "Assets/Hollowmere/Art/Textures/InteriorReflection.cubemap";
        public const string CookiePath = "Assets/Hollowmere/Art/Textures/FlashlightCookie.png";
        public const string FlamePath = "Assets/Hollowmere/Art/Textures/FlameSoft.png";
        const string FlameMaterialPath = "Assets/Hollowmere/Materials/Props/FireParticles.mat";

        public struct Assets
        {
            public VolumeProfile Base, Day, Golden, Dusk, Night, Crossing, InteriorNight;
            public Material Sky;
            public Cubemap DayReflections, InteriorReflections;
            public Texture2D FlashlightCookie;
            public Material FireParticles;
        }

        [MenuItem("Hollowmere/Setup Rendering")]
        public static void SetupMenu() => Setup();

        public static Assets Setup()
        {
            BuilderKit.EnsureFolder(Dir);
            BuilderKit.EnsureFolder(ProfileDir);
            Pipeline();
            var a = new Assets
            {
                Base = BaseProfile(),
                Day = GradeProfile("Day", exposure: 0.4f, contrast: 0f, saturation: 0f, filter: new Color(1f, 0.98f, 0.95f), temperature: 0f),
                Golden = GradeProfile("Golden", exposure: 0.45f, contrast: 4f, saturation: 6f, filter: new Color(1f, 0.9f, 0.76f), temperature: 14f),
                Dusk = GradeProfile("Dusk", exposure: 2.0f, contrast: 12f, saturation: -45f, filter: new Color(0.88f, 0.87f, 0.9f), temperature: -6f, vignette: 0.32f),
                // Night: cool and drained. The night is lit at a few thousandths in linear terms, deep in ACES's toe, so it
                // needs +3.8 EV (an eye adjusting to the dark) to keep M1's readable silhouettes against the fog.
                Night = GradeProfile("Night", exposure: 3.8f, contrast: 4f, saturation: -35f, filter: new Color(0.8f, 0.88f, 1f), temperature: -18f, vignette: 0.36f, grain: 0.42f, bloom: 0.5f),
                Crossing = GradeProfile("Crossing", exposure: 3.4f, contrast: -12f, saturation: -60f, filter: new Color(0.86f, 0.88f, 0.92f), temperature: -10f, vignette: 0.45f, grain: 0.5f),
                // Inside a lit house at night: the eye adjusts down and the lamplight stays warm.
                InteriorNight = GradeProfile("InteriorNight", exposure: 1.9f, contrast: 6f, saturation: -8f, filter: new Color(1f, 0.95f, 0.88f), temperature: 6f, vignette: 0.3f, grain: 0.32f, bloom: 0.45f),
                DayReflections = AssetDatabase.LoadAssetAtPath<Cubemap>(DayHdri),
                InteriorReflections = InteriorCubemap(),
                FlashlightCookie = Cookie(),
                FireParticles = FireMaterial(),
            };
            a.Sky = Sky(a.DayReflections);
            AssetDatabase.SaveAssets();
            Debug.Log("[Hollowmere] RENDERING SET UP: " + PipelinePath);
            return a;
        }

        // ------------------------------------------------------------ pipeline

        static void Pipeline()
        {
            if (AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath) == null) AssetDatabase.CopyAsset(TemplateRenderer, RendererPath);
            if (AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath) == null) AssetDatabase.CopyAsset(TemplatePipeline, PipelinePath);
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);

            renderer.renderingMode = RenderingMode.ForwardPlus;
            var ssao = renderer.rendererFeatures.FirstOrDefault(f => f is ScreenSpaceAmbientOcclusion);
            if (ssao != null)
            {
                var s = new SerializedObject(ssao);
                s.FindProperty("m_Active").boolValue = true;
                Set(s, "m_Settings.Intensity", 0.9f);
                Set(s, "m_Settings.Radius", 0.4f);
                Set(s, "m_Settings.DirectLightingStrength", 0.3f);
                Set(s, "m_Settings.Falloff", 60f);
                s.FindProperty("m_Settings.Samples").intValue = 0;      // High (12 samples)
                s.FindProperty("m_Settings.BlurQuality").intValue = 0;  // High (bilateral)
                s.FindProperty("m_Settings.NormalSamples").intValue = 1;
                s.FindProperty("m_Settings.Downsample").boolValue = false;
                s.ApplyModifiedPropertiesWithoutUndo();
            }
            else Debug.LogWarning("[Hollowmere] the PC renderer has no SSAO feature to tune");
            EditorUtility.SetDirty(renderer);

            var p = new SerializedObject(pipeline);
            var list = p.FindProperty("m_RendererDataList");
            list.arraySize = 1;
            list.GetArrayElementAtIndex(0).objectReferenceValue = renderer;
            p.FindProperty("m_DefaultRendererIndex").intValue = 0;
            p.FindProperty("m_SupportsHDR").boolValue = true;
            p.FindProperty("m_HDRColorBufferPrecision").intValue = 1;          // 64 bits
            p.FindProperty("m_MSAA").intValue = 4;
            p.FindProperty("m_RenderScale").floatValue = 1f;
            p.FindProperty("m_MainLightShadowsSupported").boolValue = true;
            p.FindProperty("m_MainLightShadowmapResolution").intValue = 4096;
            p.FindProperty("m_AdditionalLightsRenderingMode").intValue = 1;    // per pixel
            p.FindProperty("m_AdditionalLightsPerObjectLimit").intValue = 8;
            p.FindProperty("m_AdditionalLightShadowsSupported").boolValue = true;
            p.FindProperty("m_AdditionalLightsShadowmapResolution").intValue = 4096;
            p.FindProperty("m_AdditionalLightsShadowResolutionTierLow").intValue = 256;
            p.FindProperty("m_AdditionalLightsShadowResolutionTierMedium").intValue = 512;
            p.FindProperty("m_AdditionalLightsShadowResolutionTierHigh").intValue = 1024;
            p.FindProperty("m_ShadowDistance").floatValue = 120f;
            p.FindProperty("m_ShadowCascadeCount").intValue = 4;
            p.FindProperty("m_Cascade4Split").vector3Value = new Vector3(0.05f, 0.15f, 0.4f);
            p.FindProperty("m_SoftShadowsSupported").boolValue = true;
            p.FindProperty("m_SoftShadowQuality").intValue = 3;                // high
            p.FindProperty("m_ReflectionProbeBlending").boolValue = true;
            p.FindProperty("m_ReflectionProbeBoxProjection").boolValue = true;
            p.FindProperty("m_ColorGradingMode").intValue = 1;                 // HDR
            p.FindProperty("m_ColorGradingLutSize").intValue = 64;
            p.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(pipeline);

            GraphicsSettings.defaultRenderPipeline = pipeline;
            int current = QualitySettings.GetQualityLevel();
            var names = QualitySettings.names;
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] != "PC") continue;
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = pipeline;
            }
            QualitySettings.SetQualityLevel(current, false);
        }

        static void Set(SerializedObject s, string path, float v)
        {
            var prop = s.FindProperty(path);
            if (prop != null) prop.floatValue = v;
            else Debug.LogWarning($"[Hollowmere] no property {path} on {s.targetObject.name}");
        }

        // ------------------------------------------------------------ volume profiles

        static VolumeProfile Profile(string name)
        {
            var path = $"{ProfileDir}/{name}.asset";
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, path);
            }
            return profile;
        }

        /// <summary>The profile's component of type T (added as a sub-asset if it isn't there), with every override cleared.</summary>
        static T Component<T>(VolumeProfile profile) where T : VolumeComponent
        {
            if (!profile.TryGet<T>(out var c))
            {
                c = profile.Add<T>();
                c.name = typeof(T).Name;
                AssetDatabase.AddObjectToAsset(c, profile);
            }
            c.active = true;
            c.SetAllOverridesTo(false);
            EditorUtility.SetDirty(c);
            return c;
        }

        static VolumeProfile BaseProfile()
        {
            var p = Profile("Base");
            Component<Tonemapping>(p).mode.Override(TonemappingMode.ACES);
            var bloom = Component<Bloom>(p);
            bloom.threshold.Override(1f);
            bloom.intensity.Override(0.35f);
            bloom.scatter.Override(0.65f);
            bloom.tint.Override(new Color(1f, 0.9f, 0.8f));
            bloom.highQualityFiltering.Override(true);
            var grain = Component<FilmGrain>(p);
            grain.type.Override(FilmGrainLookup.Medium3);
            grain.intensity.Override(0.2f);
            grain.response.Override(0.8f);
            var vignette = Component<Vignette>(p);
            vignette.intensity.Override(0.22f);
            vignette.smoothness.Override(0.45f);
            vignette.color.Override(Color.black);
            var grade = Component<ColorAdjustments>(p);
            grade.contrast.Override(8f);
            grade.saturation.Override(-10f); // a muted, damp palette
            EditorUtility.SetDirty(p);
            return p;
        }

        static VolumeProfile GradeProfile(string name, float exposure, float contrast, float saturation, Color filter, float temperature,
                                          float vignette = -1f, float grain = -1f, float bloom = -1f, float gamma = 0f)
        {
            var p = Profile(name);
            var grade = Component<ColorAdjustments>(p);
            grade.postExposure.Override(exposure);
            grade.contrast.Override(8f + contrast);
            grade.saturation.Override(-10f + saturation);
            grade.colorFilter.Override(filter);
            Component<WhiteBalance>(p).temperature.Override(temperature);
            if (vignette >= 0f)
            {
                var v = Component<Vignette>(p);
                v.intensity.Override(vignette);
                v.smoothness.Override(0.45f);
            }
            if (grain >= 0f)
            {
                var g = Component<FilmGrain>(p);
                g.type.Override(FilmGrainLookup.Medium3);
                g.intensity.Override(grain);
                g.response.Override(0.8f);
            }
            if (bloom >= 0f) Component<Bloom>(p).intensity.Override(bloom);
            if (gamma != 0f) Component<LiftGammaGain>(p).gamma.Override(new Vector4(1f, 1f, 1f, gamma));
            EditorUtility.SetDirty(p);
            return p;
        }

        // ------------------------------------------------------------ sky, reflections, cookie, flames

        static Material Sky(Cubemap hdri)
        {
            BuilderKit.EnsureFolder(Path.GetDirectoryName(SkyPath).Replace('\\', '/'));
            var mat = AssetDatabase.LoadAssetAtPath<Material>(SkyPath);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Skybox/Cubemap"));
                AssetDatabase.CreateAsset(mat, SkyPath);
            }
            mat.shader = Shader.Find("Skybox/Cubemap");
            mat.SetTexture("_Tex", hdri);
            mat.SetColor("_Tint", new Color(0.5f, 0.5f, 0.5f)); // 0.5 is neutral for this shader
            mat.SetFloat("_Exposure", 1f);
            mat.SetFloat("_Rotation", 0f);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        /// <summary>A dim warm cube for box-projected probes inside the houses, so walls and floors don't reflect the sky.</summary>
        static Cubemap InteriorCubemap()
        {
            var cube = AssetDatabase.LoadAssetAtPath<Cubemap>(InteriorCubePath);
            if (cube != null) return cube;
            BuilderKit.EnsureFolder(Path.GetDirectoryName(InteriorCubePath).Replace('\\', '/'));
            cube = new Cubemap(16, TextureFormat.RGBAHalf, false);
            for (int f = 0; f < 6; f++)
            {
                var face = (CubemapFace)f;
                var c = face == CubemapFace.PositiveY ? new Color(0.05f, 0.045f, 0.04f) : face == CubemapFace.NegativeY ? new Color(0.03f, 0.022f, 0.016f) : new Color(0.06f, 0.05f, 0.04f);
                cube.SetPixels(Enumerable.Repeat(c, 16 * 16).ToArray(), face);
            }
            cube.Apply();
            AssetDatabase.CreateAsset(cube, InteriorCubePath);
            return cube;
        }

        /// <summary>The flashlight's cookie: a bright core, a soft falloff, a faint reflector ring and a little dust.</summary>
        static Texture2D Cookie()
        {
            return GeneratedTexture(CookiePath, 256, false, (x, y) =>
            {
                float r = new Vector2(x - 0.5f, y - 0.5f).magnitude * 2f;
                float beam = 1f - Mathf.SmoothStep(0.55f, 1f, r);
                float ring = 1f - 0.12f * Mathf.Exp(-Mathf.Pow((r - 0.62f) / 0.05f, 2f)) + 0.08f * Mathf.Exp(-Mathf.Pow((r - 0.48f) / 0.04f, 2f));
                float dust = 0.94f + 0.06f * Mathf.PerlinNoise(x * 23f, y * 23f);
                float v = Mathf.Clamp01(beam * ring * dust * (0.85f + 0.15f * (1f - r)));
                return new Color(v, v, v, 1f);
            }, ti =>
            {
                ti.textureType = TextureImporterType.Default;
                ti.sRGBTexture = false;
                ti.wrapMode = TextureWrapMode.Clamp;
                ti.mipmapEnabled = false;
                ti.textureCompression = TextureImporterCompression.Uncompressed;
            });
        }

        /// <summary>A soft, slightly tall blob with a hot core for additive flame particles.</summary>
        static Texture2D Flame()
        {
            return GeneratedTexture(FlamePath, 128, true, (x, y) =>
            {
                var d = new Vector2((x - 0.5f) * 2f, (y - 0.42f) * 1.5f);
                float a = Mathf.Clamp01(1f - d.magnitude);
                a = a * a * (3f - 2f * a);
                return new Color(1f, 1f, 1f, a);
            }, ti =>
            {
                ti.alphaIsTransparency = true;
                ti.wrapMode = TextureWrapMode.Clamp;
            });
        }

        static Texture2D GeneratedTexture(string path, int n, bool alpha, System.Func<float, float, Color> pixel, System.Action<TextureImporter> import)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (existing != null) return existing;
            var tex = new Texture2D(n, n, alpha ? TextureFormat.RGBA32 : TextureFormat.RGB24, false, true);
            var px = new Color[n * n];
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                    px[y * n + x] = pixel((x + 0.5f) / n, (y + 0.5f) / n);
            tex.SetPixels(px);
            BuilderKit.EnsureFolder(Path.GetDirectoryName(path).Replace('\\', '/'));
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
            var ti = (TextureImporter)AssetImporter.GetAtPath(path);
            import(ti);
            ti.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        /// <summary>Additive URP particle material for hearth flames.</summary>
        static Material FireMaterial()
        {
            BuilderKit.EnsureFolder(Path.GetDirectoryName(FlameMaterialPath).Replace('\\', '/'));
            var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            var mat = AssetDatabase.LoadAssetAtPath<Material>(FlameMaterialPath);
            if (mat == null)
            {
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, FlameMaterialPath);
            }
            mat.shader = shader;
            mat.SetTexture("_BaseMap", Flame());
            mat.SetColor("_BaseColor", new Color(1f, 0.85f, 0.6f, 1f));
            // HDR emission through the same soft shape, so the flames bloom.
            mat.EnableKeyword("_EMISSION");
            mat.SetTexture("_EmissionMap", mat.GetTexture("_BaseMap"));
            mat.SetColor("_EmissionColor", new Color(2.6f, 1.2f, 0.35f));
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_Blend", 2f); // additive
            mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (float)BlendMode.One);
            mat.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
            mat.SetFloat("_DstBlendAlpha", (float)BlendMode.One);
            mat.SetFloat("_ZWrite", 0f);
            mat.SetFloat("_Cull", 0f);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.renderQueue = (int)RenderQueue.Transparent;
            EditorUtility.SetDirty(mat);
            return mat;
        }
    }
}
