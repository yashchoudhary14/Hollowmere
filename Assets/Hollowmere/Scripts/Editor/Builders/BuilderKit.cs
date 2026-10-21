using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Helpers shared by the builders: URP Lit material assets, primitives, text labels and walls.
    /// Every helper parents with worldPositionStays = false, so positions are local to the parent,
    /// and parents are expected to be unscaled nodes (sizes are in metres).
    /// </summary>
    static class BuilderKit
    {
        public const string MaterialDir = "Assets/Hollowmere/Materials/Greybox";

        static readonly Dictionary<string, Material> s_materials = new Dictionary<string, Material>();
        static Font s_font;
        static Material s_textMaterial;

        /// <summary>Call once at the start of a build.</summary>
        public static void Begin()
        {
            s_materials.Clear();
            s_textMaterial = null;
            s_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureFolder(MaterialDir);
        }

        public static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        // ------------------------------------------------------------ materials

        /// <summary>A URP Lit material asset, created on first use and updated on every build.</summary>
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
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", smoothness);
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

        /// <summary>A cube of the given size in metres, centred on localPos.</summary>
        public static GameObject Box(Transform parent, string name, Vector3 localPos, Vector3 size, Material mat,
                                     bool collider = true, Quaternion? localRot = null)
            => Prim(PrimitiveType.Cube, parent, name, localPos, size, mat, collider, localRot);

        /// <summary>An upright cylinder: diameter and full height in metres, centred on localPos.</summary>
        public static GameObject Cyl(Transform parent, string name, Vector3 localPos, float diameter, float height, Material mat,
                                     bool collider = true, Quaternion? localRot = null)
            => Prim(PrimitiveType.Cylinder, parent, name, localPos, new Vector3(diameter, height * 0.5f, diameter), mat, collider, localRot);

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
