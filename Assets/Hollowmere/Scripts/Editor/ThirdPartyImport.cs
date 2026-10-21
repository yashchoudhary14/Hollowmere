using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Import rules for everything under Assets/ThirdParty, so nothing is fixed by hand:
    /// - Textures are capped at 2048. Normal maps import as Normal; mask maps (AO, roughness, metal, alpha) are linear.
    /// - URP Lit reads metal from R, occlusion from G and smoothness from A. Poly Haven's packed "arm" maps
    ///   (R = AO, G = roughness, B = metal) and lone roughness maps are swizzled into that layout at import,
    ///   so one texture serves as both _MetallicGlossMap and _OcclusionMap and no repacked files are needed.
    /// - HDRIs import as cubemaps (sky and reflections).
    /// - Models import without animation, cameras or lights. BuildModelMaterials then gives each FBX material a real
    ///   URP Lit asset in Materials/ThirdParty, filled from the maps beside the FBX whose names start with the
    ///   material's name (Poly Haven: id[_part]_map_res.jpg), and remaps the FBX onto it.
    /// </summary>
    public class ThirdPartyImport : AssetPostprocessor
    {
        public const string Root = "Assets/ThirdParty/";
        public const int MaxSize = 2048;

        public enum MapKind { Colour, Normal, PackedArm, Roughness, Occlusion, Metal, Alpha, Hdri, Other }

        public static bool IsThirdParty(string path) => path.Replace('\\', '/').StartsWith(Root);

        /// <summary>Reimports every third-party asset under these rules: textures first, so models can find their maps.</summary>
        [MenuItem("Hollowmere/Reimport Third-Party Assets")]
        public static void ReimportAll()
        {
            var paths = AssetDatabase.FindAssets("", new[] { Root.TrimEnd('/') })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => !AssetDatabase.IsValidFolder(p))
                .Distinct().ToArray();
            var textures = paths.Where(p => AssetImporter.GetAtPath(p) is TextureImporter).ToArray();
            var models = paths.Where(p => AssetImporter.GetAtPath(p) is ModelImporter).ToArray();
            foreach (var p in textures) AssetDatabase.ImportAsset(p, ImportAssetOptions.ForceUpdate);
            foreach (var p in models) AssetDatabase.ImportAsset(p, ImportAssetOptions.ForceUpdate);
            Debug.Log($"[Hollowmere] THIRD-PARTY REIMPORTED: {textures.Length} textures, {models.Length} models");
            BuildModelMaterials();
        }

        // Map suffixes, checked at the end of the name once the resolution is stripped. Only the suffix counts:
        // asset names can contain map words themselves ("watering_can_metal_01_diff_1k" is a colour map).
        static readonly (string suffix, MapKind kind)[] Suffixes =
        {
            ("_nor_gl", MapKind.Normal), ("_normalgl", MapKind.Normal),
            ("_arm", MapKind.PackedArm),
            ("_rough", MapKind.Roughness), ("_roughness", MapKind.Roughness),
            ("_ao", MapKind.Occlusion), ("_ambientocclusion", MapKind.Occlusion),
            ("_metal", MapKind.Metal), ("_metallic", MapKind.Metal), ("_metalness", MapKind.Metal),
            ("_alpha", MapKind.Alpha), ("_mask", MapKind.Alpha), ("_opacity", MapKind.Alpha),
            ("_diff", MapKind.Colour), ("_albedo", MapKind.Colour), ("_color", MapKind.Colour),
            ("_col_01", MapKind.Colour), ("_col_02", MapKind.Colour), ("_col_03", MapKind.Colour),
        };

        /// <summary>"lantern_01_brass_diff_1k" -> "lantern_01_brass_diff" (Poly Haven names end in the resolution).</summary>
        static string WithoutResolution(string path)
            => Regex.Replace(Path.GetFileNameWithoutExtension(path).ToLowerInvariant(), @"_\d+k$", "");

        /// <summary>What a third-party texture file is, from the end of its name.</summary>
        public static MapKind Classify(string path)
        {
            if (path.EndsWith(".hdr", System.StringComparison.OrdinalIgnoreCase)) return MapKind.Hdri;
            var n = WithoutResolution(path);
            foreach (var (suffix, kind) in Suffixes)
                if (n.EndsWith(suffix)) return kind;
            return MapKind.Other;
        }

        // ------------------------------------------------------------ textures

        void OnPreprocessTexture()
        {
            if (!IsThirdParty(assetPath)) return;
            var ti = (TextureImporter)assetImporter;
            // Every setting from scratch, so the result never depends on how the file was imported before.
            ti.textureType = TextureImporterType.Default;
            ti.textureShape = TextureImporterShape.Texture2D;
            ti.sRGBTexture = true;
            ti.swizzleR = TextureImporterSwizzle.R;
            ti.swizzleG = TextureImporterSwizzle.G;
            ti.swizzleB = TextureImporterSwizzle.B;
            ti.swizzleA = TextureImporterSwizzle.A;
            ti.alphaSource = TextureImporterAlphaSource.FromInput;
            ti.ClearPlatformTextureSettings("Standalone");
            ti.maxTextureSize = MaxSize;
            ti.mipmapEnabled = true;

            switch (Classify(assetPath))
            {
                case MapKind.Normal:
                    ti.textureType = TextureImporterType.NormalMap;
                    break;
                case MapKind.PackedArm:
                    Linear(ti);
                    Swizzle(ti, TextureImporterSwizzle.B, TextureImporterSwizzle.R, TextureImporterSwizzle.Zero, TextureImporterSwizzle.OneMinusG);
                    break;
                case MapKind.Roughness:
                    Linear(ti);
                    Swizzle(ti, TextureImporterSwizzle.Zero, TextureImporterSwizzle.One, TextureImporterSwizzle.Zero, TextureImporterSwizzle.OneMinusR);
                    break;
                case MapKind.Occlusion:
                case MapKind.Metal:
                case MapKind.Alpha:
                    Linear(ti);
                    break;
                case MapKind.Hdri:
                    ti.textureShape = TextureImporterShape.TextureCube;
                    ti.sRGBTexture = false;
                    break;
            }
        }

        static void Linear(TextureImporter ti)
        {
            ti.textureType = TextureImporterType.Default;
            ti.sRGBTexture = false;
        }

        /// <summary>Swizzled masks carry smoothness in alpha, so they need a format that keeps it (BC7 on desktop).</summary>
        static void Swizzle(TextureImporter ti, TextureImporterSwizzle r, TextureImporterSwizzle g, TextureImporterSwizzle b, TextureImporterSwizzle a)
        {
            ti.swizzleR = r;
            ti.swizzleG = g;
            ti.swizzleB = b;
            ti.swizzleA = a;
            ti.alphaSource = TextureImporterAlphaSource.FromInput;
            ti.alphaIsTransparency = false;
            var standalone = ti.GetPlatformTextureSettings("Standalone");
            standalone.overridden = true;
            standalone.format = TextureImporterFormat.BC7;
            standalone.maxTextureSize = MaxSize;
            ti.SetPlatformTextureSettings(standalone);
        }

        // ------------------------------------------------------------ models

        void OnPreprocessModel()
        {
            if (!IsThirdParty(assetPath)) return;
            var mi = (ModelImporter)assetImporter;
            mi.importAnimation = false;
            mi.importCameras = false;
            mi.importLights = false;
            mi.importBlendShapes = false;
            mi.isReadable = false;
            mi.addCollider = false;
            mi.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            if (assetPath.Contains("/Mixamo/")) mi.animationType = ModelImporterAnimationType.Human;
        }

        // ------------------------------------------------------------ model materials

        public const string ModelMaterialRoot = "Assets/Hollowmere/Materials/ThirdParty";

        /// <summary>
        /// Gives every third-party model real URP Lit material assets, one per FBX material, filled from the maps beside
        /// the FBX and remapped onto it. Idempotent; run after the textures have imported.
        /// </summary>
        [MenuItem("Hollowmere/Build Third-Party Model Materials")]
        public static void BuildModelMaterials()
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            int count = 0;
            foreach (var path in AssetDatabase.FindAssets("t:Model", new[] { Root.TrimEnd('/') }).Select(AssetDatabase.GUIDToAssetPath).Distinct())
            {
                var importer = (ModelImporter)AssetImporter.GetAtPath(path);
                var id = Path.GetFileName(Path.GetDirectoryName(path));
                var folder = Path.GetDirectoryName(path).Replace('\\', '/');
                var maps = Directory.GetFiles(folder).Where(f => !f.EndsWith(".meta")).Select(f => f.Replace('\\', '/')).ToArray();
                var prefixes = maps.Where(f => Classify(f) == MapKind.Colour).Select(PrefixOf).Distinct().ToArray();

                var names = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Material>().Select(m => m.name)
                    .Concat(importer.GetExternalObjectMap().Keys.Where(k => k.type == typeof(Material)).Select(k => k.name))
                    .Distinct().ToArray();
                foreach (var name in names)
                {
                    BuilderKit.EnsureFolder($"{ModelMaterialRoot}/{id}");
                    var matPath = $"{ModelMaterialRoot}/{id}/{name}.mat";
                    var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    if (mat == null)
                    {
                        mat = new Material(shader);
                        AssetDatabase.CreateAsset(mat, matPath);
                    }
                    Configure(mat, name, maps, prefixes);
                    EditorUtility.SetDirty(mat);
                    importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), name), mat);
                    count++;
                }
                importer.SaveAndReimport();
            }
            AssetDatabase.SaveAssets();
            Debug.Log($"[Hollowmere] MODEL MATERIALS BUILT: {count}");
        }

        static void Configure(Material mat, string name, string[] maps, string[] prefixes)
        {
            mat.shader = Shader.Find("Universal Render Pipeline/Lit");
            mat.enableInstancing = true;
            var lower = name.ToLowerInvariant();
            if (lower.Contains("flame"))
            {
                // Flame cards: the game lights its own flames.
                mat.SetColor("_BaseColor", new Color(0f, 0f, 0f, 0f));
                MakeTransparent(mat, 0f);
                return;
            }

            string prefix = prefixes.Length > 0 ? BestPrefix(name, prefixes) : null;
            string Find(MapKind kind) => maps.FirstOrDefault(f => Classify(f) == kind && PrefixOf(f) == prefix);

            mat.SetColor("_BaseColor", Color.white);
            SetMap(mat, "_BaseMap", Find(MapKind.Colour), null);
            SetMap(mat, "_BumpMap", Find(MapKind.Normal), "_NORMALMAP");
            if (SetMap(mat, "_MetallicGlossMap", Find(MapKind.PackedArm) ?? Find(MapKind.Roughness), "_METALLICSPECGLOSSMAP"))
                mat.SetFloat("_Smoothness", 0.85f); // scales the map's smoothness a touch: damp, not polished
            if (SetMap(mat, "_OcclusionMap", Find(MapKind.PackedArm) ?? Find(MapKind.Occlusion), "_OCCLUSIONMAP"))
                mat.SetFloat("_OcclusionStrength", 1f);
            if (lower.Contains("glass")) MakeTransparent(mat, 0.18f);
        }

        static bool SetMap(Material mat, string property, string path, string keyword)
        {
            var tex = string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<Texture>(path);
            mat.SetTexture(property, tex);
            if (keyword != null)
            {
                if (tex != null) mat.EnableKeyword(keyword);
                else mat.DisableKeyword(keyword);
            }
            return tex != null;
        }

        public static void MakeTransparent(Material m, float alpha)
        {
            var c = m.GetColor("_BaseColor");
            m.SetColor("_BaseColor", new Color(c.r, c.g, c.b, alpha));
            m.SetFloat("_Surface", 1f);
            m.SetFloat("_Blend", 0f);
            m.SetFloat("_ZWrite", 0f);
            m.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            m.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            m.SetOverrideTag("RenderType", "Transparent");
            m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            m.renderQueue = (int)RenderQueue.Transparent;
            m.SetShaderPassEnabled("ShadowCaster", false);
        }

        /// <summary>"Lantern_01_brass_diff_1k.jpg" -> "lantern_01_brass": the name without its map suffix and resolution.</summary>
        public static string PrefixOf(string path)
        {
            var n = WithoutResolution(path);
            foreach (var (suffix, _) in Suffixes)
                if (n.EndsWith(suffix)) return n.Substring(0, n.Length - suffix.Length);
            return n;
        }

        /// <summary>The map prefix sharing the longest start with the material's name (or the only one there is).</summary>
        public static string BestPrefix(string materialName, string[] prefixes)
        {
            if (prefixes.Length == 1) return prefixes[0];
            var m = new string(materialName.ToLowerInvariant().Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
            string best = prefixes[0];
            int bestScore = -1;
            foreach (var p in prefixes)
            {
                int score = 0;
                while (score < p.Length && score < m.Length && p[score] == m[score]) score++;
                if (m.Contains(p)) score += 100;        // the full prefix appears in the material name
                if (score > bestScore) { bestScore = score; best = p; }
            }
            return best;
        }
    }
}
