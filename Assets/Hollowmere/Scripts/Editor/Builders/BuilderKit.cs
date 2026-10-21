using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Helpers shared by the builders: URP Lit material assets, primitives, text labels and walls.
    /// Every helper parents with worldPositionStays = false, so positions are local to the parent,
    /// and parents are expected to be unscaled nodes (sizes are in metres).
    /// Materials are dressed from the <see cref="MaterialLibrary"/>; boxes and cylinders get meshes with UVs in metres,
    /// so textures keep their world size on any shape. Transforms and colliders are the plain primitives' either way.
    /// </summary>
    static class BuilderKit
    {
        public const string MaterialDir = "Assets/Hollowmere/Materials/Greybox";
        public const string MeshAssetPath = "Assets/Hollowmere/Art/Meshes/BuilderMeshes.asset";

        static readonly Dictionary<string, Material> s_materials = new Dictionary<string, Material>();
        static readonly Dictionary<string, Mesh> s_meshes = new Dictionary<string, Mesh>();
        static readonly HashSet<string> s_usedMeshes = new HashSet<string>();
        static readonly Dictionary<string, float> s_meanLuminance = new Dictionary<string, float>();
        static MaterialLibrary s_library;
        static Mesh s_meshAsset;
        static Font s_font;
        static Material s_textMaterial;

        /// <summary>Call once at the start of a build.</summary>
        public static void Begin()
        {
            s_materials.Clear();
            s_textMaterial = null;
            s_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureFolder(MaterialDir);
            s_library = MaterialLibraryBuilder.Ensure();
            BeginMeshes();
        }

        /// <summary>Call once at the end of a build, before saving: drops cached meshes this build didn't use.</summary>
        public static void End()
        {
            foreach (var pair in s_meshes.Where(p => !s_usedMeshes.Contains(p.Key)).ToArray())
            {
                Object.DestroyImmediate(pair.Value, true);
                s_meshes.Remove(pair.Key);
            }
            EditorUtility.SetDirty(s_meshAsset);
            AssetDatabase.SaveAssets();
        }

        public static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        // ------------------------------------------------------------ materials

        /// <summary>
        /// A URP Lit material asset, created on first use and updated on every build. The library entry for the name
        /// supplies its textures and world tiling; without one (or without a base map) it is the flat colour given.
        /// </summary>
        public static Material Mat(string name, Color color, float smoothness = 0.15f, Color? emission = null)
        {
            if (s_materials.TryGetValue(name, out var cached)) return cached;

            var path = $"{MaterialDir}/{name}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.enableInstancing = true;
            var entry = s_library != null ? s_library.Find(name) : null;
            bool metal = entry != null && entry.Metallic >= 0f; // flat entries that make a material metal (Bronze, Brass, Gold)
            if (entry != null && entry.HasTextures) Dress(mat, entry, color, smoothness);
            else Flat(mat, color, metal ? entry.Smoothness : smoothness, metal ? entry.Metallic : 0f);
            if (emission.HasValue)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emission.Value);
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.black);
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
            EditorUtility.SetDirty(mat);
            s_materials[name] = mat;
            return mat;
        }

        static void Flat(Material mat, Color color, float smoothness, float metallic)
        {
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", smoothness);
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_BumpScale", 1f);
            mat.SetFloat("_OcclusionStrength", 1f);
            SetMap(mat, "_BaseMap", null, null);
            SetMap(mat, "_BumpMap", null, "_NORMALMAP");
            SetMap(mat, "_MetallicGlossMap", null, "_METALLICSPECGLOSSMAP");
            SetMap(mat, "_OcclusionMap", null, "_OCCLUSIONMAP");
            mat.SetTextureScale("_BaseMap", Vector2.one);
        }

        static void Dress(Material mat, MaterialLibrary.Entry e, Color builderColour, float builderSmoothness)
        {
            var tint = e.Tint;
            if (e.KeepBuilderColour)
            {
                // Scale the texture so its average matches the builder's colour (in linear light): the door stays "red".
                var k = builderColour.linear / Mathf.Max(0.02f, MeanLuminance(e.BaseMap));
                tint = new Color(k.r, k.g, k.b).gamma;
            }
            mat.SetColor("_BaseColor", new Color(tint.r, tint.g, tint.b, 1f));
            mat.SetFloat("_Smoothness", e.MetallicGlossMap != null ? e.Smoothness : builderSmoothness); // with a map it scales the map
            mat.SetFloat("_Metallic", e.Metallic >= 0f ? e.Metallic : 0f);
            mat.SetFloat("_BumpScale", e.NormalScale);
            mat.SetFloat("_OcclusionStrength", e.OcclusionStrength);
            SetMap(mat, "_BaseMap", e.BaseMap, null);
            SetMap(mat, "_BumpMap", e.NormalMap, "_NORMALMAP");
            SetMap(mat, "_MetallicGlossMap", e.MetallicGlossMap, "_METALLICSPECGLOSSMAP");
            SetMap(mat, "_OcclusionMap", e.OcclusionMap, "_OCCLUSIONMAP");
            // Builder meshes have UVs in metres, so one repeat every MetresPerTile.
            mat.SetTextureScale("_BaseMap", Vector2.one / Mathf.Max(0.01f, e.MetresPerTile));
            mat.SetTextureOffset("_BaseMap", Vector2.zero);
        }

        static void SetMap(Material mat, string property, Texture tex, string keyword)
        {
            mat.SetTexture(property, tex);
            if (keyword == null) return;
            if (tex != null) mat.EnableKeyword(keyword);
            else mat.DisableKeyword(keyword);
        }

        /// <summary>Average linear luminance of a colour map, read from its source file (the import is GPU-only).</summary>
        static float MeanLuminance(Texture2D tex)
        {
            var path = AssetDatabase.GetAssetPath(tex);
            if (s_meanLuminance.TryGetValue(path, out var cached)) return cached;
            var copy = new Texture2D(2, 2);
            float mean = 0.5f;
            if (ImageConversion.LoadImage(copy, File.ReadAllBytes(path)))
            {
                var px = copy.GetPixels32();
                double sum = 0;
                int n = 0;
                for (int i = 0; i < px.Length; i += 61)
                {
                    var c = ((Color)px[i]).linear;
                    sum += 0.2126 * c.r + 0.7152 * c.g + 0.0722 * c.b;
                    n++;
                }
                mean = (float)(sum / n);
            }
            Object.DestroyImmediate(copy);
            return s_meanLuminance[path] = mean;
        }

        static Material TextMaterial()
        {
            if (s_textMaterial != null) return s_textMaterial;
            var shader = Shader.Find("Hollowmere/WorldText");
            if (shader == null)
            {
                Debug.LogWarning("[Hollowmere] Hollowmere/WorldText shader not found; labels use the built-in font material.");
                return s_textMaterial = s_font.material;
            }
            var path = $"{MaterialDir}/WorldText.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.shader = shader;
            mat.mainTexture = s_font.material.mainTexture;
            EditorUtility.SetDirty(mat);
            return s_textMaterial = mat;
        }

        // ------------------------------------------------------------ nodes and primitives

        public static Transform Node(Transform parent, string name, Vector3 localPos = default, Quaternion? localRot = null)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.localPosition = localPos;
            t.localRotation = localRot ?? Quaternion.identity;
            return t;
        }

        public static GameObject Prim(PrimitiveType type, Transform parent, string name, Vector3 localPos, Vector3 scale,
                                      Material mat, bool collider = true, Quaternion? localRot = null)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = localRot ?? Quaternion.identity;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!collider) Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        /// <summary>A cube of the given size in metres, centred on localPos. Its UVs are in metres (see MetricMesh).</summary>
        public static GameObject Box(Transform parent, string name, Vector3 localPos, Vector3 size, Material mat,
                                     bool collider = true, Quaternion? localRot = null)
        {
            var go = Prim(PrimitiveType.Cube, parent, name, localPos, size, mat, collider, localRot);
            var filter = go.GetComponent<MeshFilter>();
            filter.sharedMesh = MetricMesh(PrimitiveType.Cube, filter.sharedMesh, size);
            return go;
        }

        /// <summary>An upright cylinder: diameter and full height in metres, centred on localPos. UVs in metres.</summary>
        public static GameObject Cyl(Transform parent, string name, Vector3 localPos, float diameter, float height, Material mat,
                                     bool collider = true, Quaternion? localRot = null)
        {
            var go = Prim(PrimitiveType.Cylinder, parent, name, localPos, new Vector3(diameter, height * 0.5f, diameter), mat, collider, localRot);
            var filter = go.GetComponent<MeshFilter>();
            filter.sharedMesh = MetricMesh(PrimitiveType.Cylinder, filter.sharedMesh, new Vector3(diameter, height, diameter));
            return go;
        }

        // ------------------------------------------------------------ meshes with UVs in metres

        static void BeginMeshes()
        {
            s_meshes.Clear();
            s_usedMeshes.Clear();
            s_meshAsset = AssetDatabase.LoadAssetAtPath<Mesh>(MeshAssetPath);
            if (s_meshAsset == null)
            {
                EnsureFolder(Path.GetDirectoryName(MeshAssetPath).Replace('\\', '/'));
                s_meshAsset = new Mesh { name = "BuilderMeshes" }; // an empty main object holding the cached meshes
                AssetDatabase.CreateAsset(s_meshAsset, MeshAssetPath);
            }
            foreach (var m in AssetDatabase.LoadAllAssetsAtPath(MeshAssetPath).OfType<Mesh>())
                if (m != s_meshAsset) s_meshes[m.name] = m;
        }

        static string Mm(float metres) => Mathf.RoundToInt(metres * 1000f).ToString(System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// The unit primitive's mesh with its UVs scaled to metres for a given size (the object's scale), cached per size
        /// as a sub-asset of BuilderMeshes.asset. Vertices and normals are the primitive's, so the transform and the
        /// collider stay exactly as before; only texturing changes. Box faces read upright, with u running to the right.
        /// </summary>
        public static Mesh MetricMesh(PrimitiveType type, Mesh unit, Vector3 size)
        {
            // Cylinders come in many random heights (trees): texturing them to the nearest 5 cm across and 25 cm up
            // is invisible and keeps the cache to a few dozen meshes.
            if (type == PrimitiveType.Cylinder)
                size = new Vector3(Mathf.Max(0.05f, Mathf.Round(size.x * 20f) / 20f), Mathf.Max(0.25f, Mathf.Round(size.y * 4f) / 4f), Mathf.Max(0.05f, Mathf.Round(size.z * 20f) / 20f));
            var key = $"{type}_{Mm(size.x)}x{Mm(size.y)}x{Mm(size.z)}";
            s_usedMeshes.Add(key);
            if (s_meshes.TryGetValue(key, out var mesh) && mesh != null) return mesh;

            mesh = Object.Instantiate(unit);
            mesh.name = key;
            var v = mesh.vertices;
            var n = mesh.normals;
            var uv = mesh.uv;
            for (int i = 0; i < v.Length; i++)
            {
                Vector3 p = v[i], q = n[i];
                if (type == PrimitiveType.Cube)
                {
                    if (Mathf.Abs(q.x) > 0.5f) uv[i] = new Vector2(Mathf.Sign(q.x) * p.z * size.z, p.y * size.y);
                    else if (Mathf.Abs(q.y) > 0.5f) uv[i] = new Vector2(p.x * size.x, Mathf.Sign(q.y) * p.z * size.z);
                    else uv[i] = new Vector2(-Mathf.Sign(q.z) * p.x * size.x, p.y * size.y);
                }
                else if (Mathf.Abs(q.y) > 0.9f) uv[i] = new Vector2(p.x * size.x, p.z * size.z); // cylinder caps
                else uv[i] = new Vector2(uv[i].x * Mathf.PI * size.x, (p.y + 1f) * 0.5f * size.y); // cylinder side
            }
            mesh.uv = uv;
            mesh.RecalculateTangents();
            AssetDatabase.AddObjectToAsset(mesh, s_meshAsset);
            s_meshes[key] = mesh;
            return mesh;
        }

        public static GameObject Ball(Transform parent, string name, Vector3 localPos, float diameter, Material mat, bool collider = true)
            => Prim(PrimitiveType.Sphere, parent, name, localPos, Vector3.one * diameter, mat, collider);

        public static void NoShadows(GameObject go)
        {
            foreach (var r in go.GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = ShadowCastingMode.Off;
        }

        /// <summary>
        /// World text. A TextMesh reads correctly when the camera looks along the label's +z,
        /// so a label on a wall facing the viewer needs its +z pointing into the wall.
        /// </summary>
        public static TextMesh Label(Transform parent, string name, string text, Vector3 localPos, Quaternion localRot,
                                     float lineHeight, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = localRot;
            var tm = go.AddComponent<TextMesh>();
            tm.font = s_font;
            tm.text = text;
            tm.fontSize = 64;
            tm.characterSize = lineHeight * 10f / tm.fontSize;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = color;
            var mr = go.GetComponent<MeshRenderer>();
            mr.sharedMaterial = TextMaterial();
            mr.shadowCastingMode = ShadowCastingMode.Off;
            return tm;
        }

        public static Light PointLight(Transform parent, string name, Vector3 localPos, Color color, float range, float intensity,
                                       LightShadows shadows = LightShadows.None)
        {
            var t = Node(parent, name, localPos);
            var light = t.gameObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.range = range;
            light.intensity = intensity;
            light.shadows = shadows;
            return light;
        }

        // ------------------------------------------------------------ walls

        public struct Gap
        {
            public float Center, Width, Height;
            public Gap(float center, float width, float height) { Center = center; Width = width; Height = height; }
        }

        /// <summary>A wall running along x at local z, from x0 to x1, with openings (doors) cut out.</summary>
        public static void WallAlongX(Transform parent, string name, float z, float x0, float x1, float height, float thickness,
                                      Material mat, params Gap[] gaps)
        {
            float cursor = x0;
            int part = 0;
            foreach (var g in SortGaps(gaps))
            {
                float g0 = g.Center - g.Width * 0.5f, g1 = g.Center + g.Width * 0.5f;
                if (g0 > cursor)
                    Box(parent, $"{name}_{part++}", new Vector3((cursor + g0) * 0.5f, height * 0.5f, z), new Vector3(g0 - cursor, height, thickness), mat);
                if (g.Height < height)
                    Box(parent, $"{name}_Lintel{part++}", new Vector3(g.Center, (g.Height + height) * 0.5f, z), new Vector3(g.Width, height - g.Height, thickness), mat);
                cursor = g1;
            }
            if (x1 > cursor)
                Box(parent, $"{name}_{part}", new Vector3((cursor + x1) * 0.5f, height * 0.5f, z), new Vector3(x1 - cursor, height, thickness), mat);
        }

        /// <summary>A wall running along z at local x, from z0 to z1, with openings cut out.</summary>
        public static void WallAlongZ(Transform parent, string name, float x, float z0, float z1, float height, float thickness,
                                      Material mat, params Gap[] gaps)
        {
            float cursor = z0;
            int part = 0;
            foreach (var g in SortGaps(gaps))
            {
                float g0 = g.Center - g.Width * 0.5f, g1 = g.Center + g.Width * 0.5f;
                if (g0 > cursor)
                    Box(parent, $"{name}_{part++}", new Vector3(x, height * 0.5f, (cursor + g0) * 0.5f), new Vector3(thickness, height, g0 - cursor), mat);
                if (g.Height < height)
                    Box(parent, $"{name}_Lintel{part++}", new Vector3(x, (g.Height + height) * 0.5f, g.Center), new Vector3(thickness, height - g.Height, g.Width), mat);
                cursor = g1;
            }
            if (z1 > cursor)
                Box(parent, $"{name}_{part}", new Vector3(x, height * 0.5f, (cursor + z1) * 0.5f), new Vector3(thickness, height, z1 - cursor), mat);
        }

        static List<Gap> SortGaps(Gap[] gaps)
        {
            var list = new List<Gap>(gaps);
            list.Sort((a, b) => a.Center.CompareTo(b.Center));
            return list;
        }

        /// <summary>World-space bounds from a local centre and size under an unrotated parent.</summary>
        public static Bounds WorldBounds(Transform parent, Vector3 localCenter, Vector3 size)
            => new Bounds(parent.TransformPoint(localCenter), size);
    }
}
