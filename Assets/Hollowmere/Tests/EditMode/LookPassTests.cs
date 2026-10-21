using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hollowmere.EditorTools;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Hollowmere.Tests
{
    /// <summary>
    /// The M1.5 look pass may change how the village looks, never how it plays: the gameplay layout (colliders,
    /// NavMesh obstacles, component transforms) must match the snapshot recorded before the pass, builder materials
    /// follow the MaterialLibrary with a flat-colour fallback, and builder boxes carry UVs in metres.
    /// </summary>
    public class LookPassTests
    {
        const string ScenePath = "Assets/Hollowmere/Scenes/Greybox.unity";

        [Test]
        public void Library_FindsExactKeysFirst_ThenTheLongestPrefix()
        {
            var lib = ScriptableObject.CreateInstance<MaterialLibrary>();
            lib.Entries.Add(new MaterialLibrary.Entry { Key = "Do*" });
            lib.Entries.Add(new MaterialLibrary.Entry { Key = "Door_*" });
            lib.Entries.Add(new MaterialLibrary.Entry { Key = "Door_Red" });
            Assert.That(lib.Find("Door_Red").Key, Is.EqualTo("Door_Red"));
            Assert.That(lib.Find("Door_Blue").Key, Is.EqualTo("Door_*"));
            Assert.That(lib.Find("Dome").Key, Is.EqualTo("Do*"));
            Assert.That(lib.Find("Floor"), Is.Null);
            Object.DestroyImmediate(lib);
        }

        [Test]
        public void GreyboxMaterials_FollowTheLibrary_OrStayFlat()
        {
            var lib = AssetDatabase.LoadAssetAtPath<MaterialLibrary>(MaterialLibraryBuilder.LibraryPath);
            if (lib == null) Assert.Ignore("Material library not built yet.");
            var mats = AssetDatabase.FindAssets("t:Material", new[] { BuilderKit_MaterialDir })
                .Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Material>).ToArray();
            Assert.That(mats, Is.Not.Empty);
            int dressed = 0;
            foreach (var m in mats)
            {
                if (m.name == "WorldText") continue;
                var e = lib.Find(m.name);
                if (e != null && e.HasTextures)
                {
                    dressed++;
                    Assert.That(m.GetTexture("_BaseMap"), Is.EqualTo(e.BaseMap), m.name);
                    Assert.That(m.GetTexture("_BumpMap"), Is.EqualTo(e.NormalMap), m.name);
                    Assert.That(m.GetTextureScale("_BaseMap").x, Is.EqualTo(1f / e.MetresPerTile).Within(1e-4f), m.name + " tiles by world size");
                }
                else
                {
                    Assert.That(m.GetTexture("_BaseMap"), Is.Null, m.name + " has no library texture, so it stays flat");
                    Assert.That(m.IsKeywordEnabled("_NORMALMAP"), Is.False, m.name);
                }
            }
            Assert.That(dressed, Is.GreaterThan(10), "Most surfaces should be textured once the third-party sets are in.");
        }

        const string BuilderKit_MaterialDir = "Assets/Hollowmere/Materials/Greybox";

        [Test]
        public void BuilderBoxes_HaveUvsInMetres_AndUnitColliders()
        {
            if (!File.Exists(ScenePath)) Assert.Ignore("Greybox scene not built yet.");
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            int boxes = 0;
            foreach (var f in Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var mesh = f.sharedMesh;
                if (mesh == null || !mesh.name.StartsWith("Cube_")) continue;
                boxes++;
                var s = f.transform.localScale;
                var uv = mesh.uv;
                float uRange = uv.Max(p => p.x) - uv.Min(p => p.x), vRange = uv.Max(p => p.y) - uv.Min(p => p.y);
                Assert.That(uRange, Is.EqualTo(Mathf.Max(s.x, s.z)).Within(0.002f), f.name);
                Assert.That(vRange, Is.EqualTo(Mathf.Max(s.y, s.z)).Within(0.002f), f.name);
                Assert.That(mesh.bounds.size, Is.EqualTo(Vector3.one), f.name + " keeps the unit cube's vertices");
                var box = f.GetComponent<BoxCollider>();
                if (box != null)
                {
                    Assert.That(box.size, Is.EqualTo(Vector3.one), f.name);
                    Assert.That(box.center, Is.EqualTo(Vector3.zero), f.name);
                }
            }
            Assert.That(boxes, Is.GreaterThan(500));
        }

        [Test]
        public void GameplayLayout_MatchesTheSnapshotFromBeforeTheLookPass()
        {
            if (!File.Exists(ScenePath)) Assert.Ignore("Greybox scene not built yet.");
            if (!File.Exists(LayoutFingerprint.FixturePath)) Assert.Ignore("No layout snapshot recorded.");
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var expected = File.ReadAllLines(LayoutFingerprint.FixturePath).Where(l => l.Length > 0).ToList();
            var actual = LayoutFingerprint.Compute();
            var removed = Diff(expected, actual);
            var added = Diff(actual, expected);
            Assert.That(removed, Is.Empty, "Gameplay objects changed or removed:\n" + string.Join("\n", removed.Take(12)));
            Assert.That(added, Is.Empty, "Gameplay objects added or changed:\n" + string.Join("\n", added.Take(12)));
        }

        /// <summary>Lines of a that b lacks, counting duplicates.</summary>
        static List<string> Diff(List<string> a, List<string> b)
        {
            var counts = b.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var result = new List<string>();
            foreach (var line in a)
            {
                if (counts.TryGetValue(line, out var n) && n > 0) counts[line] = n - 1;
                else result.Add(line);
            }
            return result;
        }
    }
}
