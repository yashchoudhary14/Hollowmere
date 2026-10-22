using System.Linq;
using Hollowmere.EditorTools;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Hollowmere.Tests
{
    /// <summary>
    /// Import rules for Assets/ThirdParty (M1.5): nothing above 2K, normal maps typed as Normal, mask maps linear,
    /// packed AO/rough/metal maps swizzled for URP Lit, and every model material rebuilt as URP Lit with its maps.
    /// </summary>
    public class ThirdPartyImportTests
    {
        static string[] Paths(System.Type importer) => AssetDatabase.FindAssets("", new[] { "Assets/ThirdParty" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => !AssetDatabase.IsValidFolder(p) && importer.IsInstanceOfType(AssetImporter.GetAtPath(p)))
            .Distinct().ToArray();

        [Test]
        public void NamesClassify_AsTheirMaps()
        {
            Assert.That(ThirdPartyImport.Classify("x/grey_plaster_nor_gl_2k.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.Normal));
            Assert.That(ThirdPartyImport.Classify("x/WoodFloor043_2K-JPG_NormalGL.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.Normal));
            Assert.That(ThirdPartyImport.Classify("x/grey_plaster_arm_2k.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.PackedArm));
            Assert.That(ThirdPartyImport.Classify("x/WoodFloor043_2K-JPG_Roughness.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.Roughness));
            Assert.That(ThirdPartyImport.Classify("x/Rock034_2K-JPG_AmbientOcclusion.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.Occlusion));
            Assert.That(ThirdPartyImport.Classify("x/Lantern_01_brass_diff_1k.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.Colour));
            Assert.That(ThirdPartyImport.Classify("x/fabric_pattern_05_col_01_2k.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.Colour));
            Assert.That(ThirdPartyImport.Classify("x/vintage_oil_lamp_glass_alpha_1k.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.Alpha));
            Assert.That(ThirdPartyImport.Classify("x/kloppenheim_02_2k.hdr"), Is.EqualTo(ThirdPartyImport.MapKind.Hdri));
            Assert.That(ThirdPartyImport.Classify("x/watering_can_metal_01_diff_1k.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.Colour), "Only the suffix names the map.");
            Assert.That(ThirdPartyImport.Classify("x/watering_can_metal_01_arm_1k.jpg"), Is.EqualTo(ThirdPartyImport.MapKind.PackedArm));
            Assert.That(ThirdPartyImport.PrefixOf("x/watering_can_metal_01_nor_gl_1k.jpg"), Is.EqualTo("watering_can_metal_01"));
            Assert.That(ThirdPartyImport.PrefixOf("x/Lantern_01_brass_diff_1k.jpg"), Is.EqualTo("lantern_01_brass"));
            Assert.That(ThirdPartyImport.BestPrefix("Lantern_01_painted", new[] { "lantern_01_brass", "lantern_01_painted" }), Is.EqualTo("lantern_01_painted"));
        }

        [Test]
        public void Textures_AreAtMost2K_AndImportedAsTheirKind()
        {
            var paths = Paths(typeof(TextureImporter));
            if (paths.Length == 0) Assert.Ignore("No third-party textures in the project yet.");
            foreach (var p in paths)
            {
                var ti = (TextureImporter)AssetImporter.GetAtPath(p);
                Assert.That(ti.maxTextureSize, Is.LessThanOrEqualTo(ThirdPartyImport.MaxSize), p);
                var tex = AssetDatabase.LoadAssetAtPath<Texture>(p);
                Assert.That(tex, Is.Not.Null, p);
                Assert.That(Mathf.Max(tex.width, tex.height), Is.LessThanOrEqualTo(ThirdPartyImport.MaxSize), p);

                switch (ThirdPartyImport.Classify(p))
                {
                    case ThirdPartyImport.MapKind.Normal:
                        Assert.That(ti.textureType, Is.EqualTo(TextureImporterType.NormalMap), p);
                        break;
                    case ThirdPartyImport.MapKind.PackedArm:
                        Assert.That(ti.sRGBTexture, Is.False, p);
                        Assert.That(ti.swizzleR, Is.EqualTo(TextureImporterSwizzle.B), p + " metal from B");
                        Assert.That(ti.swizzleG, Is.EqualTo(TextureImporterSwizzle.R), p + " occlusion from R");
                        Assert.That(ti.swizzleA, Is.EqualTo(TextureImporterSwizzle.OneMinusG), p + " smoothness from 1 - G");
                        break;
                    case ThirdPartyImport.MapKind.Roughness:
                    case ThirdPartyImport.MapKind.Occlusion:
                    case ThirdPartyImport.MapKind.Metal:
                    case ThirdPartyImport.MapKind.Alpha:
                        Assert.That(ti.sRGBTexture, Is.False, p);
                        break;
                    case ThirdPartyImport.MapKind.Colour:
                        Assert.That(ti.sRGBTexture, Is.True, p);
                        break;
                    case ThirdPartyImport.MapKind.Hdri:
                        Assert.That(ti.textureShape, Is.EqualTo(TextureImporterShape.TextureCube), p);
                        break;
                }
            }
        }

        [Test]
        public void Models_UseUrpLit_WithTheirOwnColourMaps()
        {
            var paths = Paths(typeof(ModelImporter));
            if (paths.Length == 0) Assert.Ignore("No third-party models in the project yet.");
            foreach (var p in paths)
            {
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                var mats = model.GetComponentsInChildren<Renderer>(true).SelectMany(r => r.sharedMaterials).Distinct().ToArray();
                Assert.That(mats, Is.Not.Empty, p);
                foreach (var m in mats)
                {
                    Assert.That(m, Is.Not.Null, $"{p}: a renderer has no material");
                    Assert.That(AssetDatabase.GetAssetPath(m), Does.StartWith(ThirdPartyImport.ModelMaterialRoot), $"{p}: {m.name} isn't remapped to a material asset");
                    Assert.That(m.shader.name, Is.EqualTo("Universal Render Pipeline/Lit"), $"{p}: {m.name}");
                    var lower = m.name.ToLowerInvariant();
                    if (lower.Contains("flame") || lower.Contains("glass")) continue;
                    Assert.That(m.GetTexture("_BaseMap"), Is.Not.Null, $"{p}: {m.name} has no colour map");
                    Assert.That(m.GetTexture("_BumpMap"), Is.Not.Null, $"{p}: {m.name} has no normal map");
                }
            }
        }

        [Test]
        public void EveryThirdPartyAsset_HasASourceRecord_AndIsCredited()
        {
            if (!System.IO.Directory.Exists("Assets/ThirdParty")) Assert.Ignore("No third-party assets yet.");
            var credits = System.IO.File.Exists("CREDITS.md") ? System.IO.File.ReadAllText("CREDITS.md") : "";
            Assert.That(credits, Is.Not.Empty, "CREDITS.md is missing: run tools/write-credits.ps1");
            // Asset folders: PolyHaven/<id>, AmbientCG/<id>, AssetStore/<pack>/<item>, Fab/<pack>/<item>.
            var folders = new[] { "PolyHaven", "AmbientCG" }
                .SelectMany(s => System.IO.Directory.GetDirectories("Assets/ThirdParty/" + s))
                .Concat(new[] { "AssetStore", "Fab" }
                    .Where(s => System.IO.Directory.Exists("Assets/ThirdParty/" + s))
                    .SelectMany(s => System.IO.Directory.GetDirectories("Assets/ThirdParty/" + s))
                    .SelectMany(System.IO.Directory.GetDirectories))
                .ToArray();
            Assert.That(folders, Is.Not.Empty);
            foreach (var f in folders)
            {
                var path = System.IO.Path.Combine(f, "source.json");
                Assert.That(System.IO.File.Exists(path), Is.True, $"{f} has no source.json");
                var json = System.IO.File.ReadAllText(path);
                Assert.That(json, Does.Contain("\"licence\""), path);
                Assert.That(json, Does.Contain("\"url\""), path);
                var name = System.Text.RegularExpressions.Regex.Match(json, "\"name\":\\s*\"([^\"]+)\"").Groups[1].Value;
                Assert.That(credits, Does.Contain(name), $"{name} isn't in CREDITS.md: run tools/write-credits.ps1");
            }
        }

        [Test]
        public void Audio_IsVorbis_LoopsStream_OneShotsAreMono()
        {
            var paths = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/ThirdParty", AudioImportRules.AudioRoot.TrimEnd('/') })
                .Select(AssetDatabase.GUIDToAssetPath).Distinct().ToArray();
            if (paths.Length == 0) Assert.Ignore("No recorded audio in the project yet.");
            foreach (var p in paths)
            {
                var ai = (AudioImporter)AssetImporter.GetAtPath(p);
                var s = ai.defaultSampleSettings;
                Assert.That(s.compressionFormat, Is.EqualTo(AudioCompressionFormat.Vorbis), p);
                if (AudioImportRules.IsLoop(p))
                    Assert.That(s.loadType, Is.EqualTo(AudioClipLoadType.Streaming), p);
                else
                {
                    Assert.That(ai.forceToMono, Is.True, p);
                    Assert.That(s.loadType, Is.EqualTo(AudioClipLoadType.DecompressOnLoad), p);
                }
            }
        }
    }
}
