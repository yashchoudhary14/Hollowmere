using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Third-party models for the builders: placing them at real size, fitting them into a primitive's space, and
    /// swapping a "Visual" slot's primitives for a model while their colliders stay exactly where they were.
    /// Models import without colliders (ThirdPartyImport), so everything placed here is visual only.
    /// A missing model is skipped with a warning and the primitives stay, so a build never fails for want of art.
    /// </summary>
    static class PropKit
    {
        const string PH = "Assets/ThirdParty/PolyHaven/";
        const string SI = "Assets/ThirdParty/AssetStore/SaloonInterior/";

        // Poly Haven models face +z; Megascans (Saloon) models are noted where they differ.
        public const string Wardrobe = PH + "GothicCabinet_01/GothicCabinet_01_2k.fbx";
        public const string Bed = PH + "GothicBed_01/GothicBed_01_2k.fbx";
        public const string RockingChair = PH + "Rockingchair_01/Rockingchair_01_2k.fbx";
        public const string Bookshelf = PH + "wooden_bookshelf_worn/wooden_bookshelf_worn_2k.fbx";
        public const string GrandfatherClock = PH + "vintage_grandfather_clock_01/vintage_grandfather_clock_01_2k.fbx";
        public const string WineBarrel = PH + "wine_barrel_01/wine_barrel_01_2k.fbx";
        public const string Crate = PH + "wooden_crate_01/wooden_crate_01_2k.fbx";
        public const string Books = PH + "book_encyclopedia_set_01/book_encyclopedia_set_01_1k.fbx";
        public const string MantelClock = PH + "mantel_clock_01/mantel_clock_01_1k.fbx";
        public const string OilLamp = PH + "vintage_oil_lamp/vintage_oil_lamp_1k.fbx";
        public const string Candleholders = PH + "brass_candleholders/brass_candleholders_1k.fbx";
        public const string Candlestick = PH + "wooden_candlestick/wooden_candlestick_1k.fbx";
        public const string Lantern = PH + "Lantern_01/Lantern_01_1k.fbx";
        public const string WoodenBucket = PH + "wooden_bucket_01/wooden_bucket_01_1k.fbx";
        public const string Footstool = PH + "wooden_stool_02/wooden_stool_02_1k.fbx";
        public const string Hammer = PH + "wooden_hammer_01/wooden_hammer_01_1k.fbx";
        public const string Axe = PH + "wooden_axe_02/wooden_axe_02_1k.fbx";
        public const string WateringCan = PH + "watering_can_metal_01/watering_can_metal_01_1k.fbx";
        public const string Stump = PH + "tree_stump_01/tree_stump_01_2k.fbx";
        public const string FallenTrunk = PH + "dead_tree_trunk/dead_tree_trunk_2k.fbx";
        public const string Branches = PH + "dry_branches_medium_01/dry_branches_medium_01_1k.fbx";
        public const string MossRocks = PH + "rock_moss_set_02/rock_moss_set_02_2k.fbx";
        public const string Boulder = PH + "boulder_01/boulder_01_2k.fbx";

        public const string Barrel = SI + "His_Med_Storage_Barrel_Wood_Worn_01/SM_His_Med_Storage_Barrel_Wood_Worn_01.fbx";
        public const string FloorLamp = SI + "His_Sal_Furniture_Lamp_Wood_Old_02/SM_His_Sal_Furniture_Lamp_Wood_Old_02_A.fbx";
        public const string CartWheel = SI + "His_Wil_Cart_Wheel_Wood_Old_01/SM_His_Wil_Cart_Wheel_Wood_Old_01.fbx"; // lies flat
        public const string Horseshoe = SI + "His_Wil_Equipment_Horseshoe_Metal_Rusted_01/SM_His_Wil_Equipment_Horseshoe_Metal_Rusted_01.fbx"; // lies flat
        public const string LowTable = SI + "His_Wil_Furniture_Table_Wood_Worn_01/SM_His_Wil_Furniture_Table_Wood_Worn_01.fbx";
        public const string Beam = SI + "Ind_Sup_Beam_Wood_Old_04/SM_Ind_Sup_Beam_Wood_Old_04.fbx"; // 4 m along z
        public const string Chair = SI + "Res_Fur_Chair_Wood_Old_03/SM_Res_Fur_Chair_Wood_Old_03.fbx";
        public const string Stool = SI + "Res_Fur_Stool_Wood_Armless_01/SM_Res_Fur_Stool_Wood_Armless_01.fbx";
        public const string WallShelf = SI + "Res_Rur_Furniture_Shelf_Wood_Old_01/SM_Res_Rur_Furniture_Shelf_Wood_Old_01.fbx"; // back on -x
        public const string MetalBucket = SI + "Res_Rur_Storage_Bucket_Metal_Worn_03/SM_Res_Rur_Storage_Bucket_Metal_Worn_03.fbx";
        public const string Chest = SI + "Res_Sto_Chest_Wood_Worn_08/SM_Res_Sto_Chest_Wood_Worn_08.fbx";
        public const string Jug = SI + "Res_Sto_Flask_Ceramic_Worn_01/SM_Res_Sto_Flask_Ceramic_Worn_01.fbx";
        public const string PictureFrame = SI + "Urb_Dec_PictureFrame_Wood_Worn_01/SM_Urb_Dec_PictureFrame_Wood_Worn_01.fbx"; // face on +x

        public static bool Exists(string path) => AssetDatabase.LoadAssetAtPath<GameObject>(path) != null;

        /// <summary>Extra turn that brings a model's front round to +z (the rocking chair faces -x; the rest face +z).</summary>
        static float FrontYaw(string path) => path == RockingChair ? 90f : 0f;

        /// <summary>
        /// A model at real size (times scale): an anchor node at localPos turned by yaw, with the model's bottom centre
        /// on the anchor. Returns the anchor, or null if the model isn't in the project.
        /// </summary>
        public static Transform Model(Transform parent, string name, string path, Vector3 localPos, float yaw = 0f, float scale = 1f,
                                      Quaternion? tilt = null)
        {
            var model = Load(path);
            if (model == null) return null;
            var anchor = BuilderKit.Node(parent, name, localPos, Quaternion.Euler(0f, yaw + FrontYaw(path), 0f) * (tilt ?? Quaternion.identity));
            var holder = Instantiate(model, anchor);
            var b = LocalBounds(holder);
            holder.localScale = Vector3.one * scale;
            holder.localPosition = -new Vector3(b.center.x, b.min.y, b.center.z) * scale;
            if (tilt.HasValue)
            {
                // Tilted (leaning, hung, stood on edge): settle its lowest point back onto localPos.y.
                var low = LocalBounds(holder, parent).min.y;
                anchor.localPosition += Vector3.up * (localPos.y - low);
            }
            return anchor;
        }

        /// <summary>
        /// A model stretched to fill a box given in parent space (bottom on the box's floor, centred), turned by yaw first.
        /// uniform keeps proportions and fits inside the box.
        /// </summary>
        public static Transform Fit(Transform parent, string name, string path, Bounds box, float yaw = 0f, bool uniform = false)
        {
            var model = Load(path);
            if (model == null) return null;
            var anchor = BuilderKit.Node(parent, name, new Vector3(box.center.x, box.min.y, box.center.z), Quaternion.Euler(0f, yaw + FrontYaw(path), 0f));
            var holder = Instantiate(model, anchor);
            var b = LocalBounds(holder);
            // The box measured in the anchor's (turned) frame.
            var size = Quaternion.Inverse(anchor.localRotation) * box.size;
            size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
            var s = new Vector3(size.x / b.size.x, size.y / b.size.y, size.z / b.size.z);
            if (uniform) s = Vector3.one * Mathf.Min(s.x, s.y, s.z);
            holder.localScale = s;
            holder.localPosition = -Vector3.Scale(new Vector3(b.center.x, b.min.y, b.center.z), s);
            return anchor;
        }

        /// <summary>
        /// Replaces a Visual slot's primitives with a model fitted to their combined bounds. The primitives keep their
        /// transforms and colliders; only their renderers go. Returns the model anchor, or null (primitives untouched).
        /// </summary>
        public static Transform SwapVisual(Transform visual, string path, float yaw = 0f, bool uniform = false)
        {
            if (!Exists(path)) return null;
            var renderers = visual.GetComponentsInChildren<MeshRenderer>(true);
            if (renderers.Length == 0) return null;
            var box = BoundsIn(visual, renderers);
            var anchor = Fit(visual, "Model", path, box, yaw, uniform);
            foreach (var r in renderers) StripRenderer(r.gameObject);
            return anchor;
        }

        /// <summary>Removes a primitive's renderer and mesh, leaving its transform and collider.</summary>
        public static void StripRenderer(GameObject go)
        {
            var r = go.GetComponent<MeshRenderer>();
            if (r != null) Object.DestroyImmediate(r);
            var f = go.GetComponent<MeshFilter>();
            if (f != null) Object.DestroyImmediate(f);
        }

        static GameObject Load(string path)
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (model == null) Debug.LogWarning($"[Hollowmere] model missing, keeping primitives: {path}");
            return model;
        }

        /// <summary>
        /// The model under a holder node at the parent's origin. The model's own root keeps the transform the FBX gave
        /// it (some carry a 100x scale for centimetre meshes, or an axis rotation), so placement scales the holder.
        /// </summary>
        static Transform Instantiate(GameObject model, Transform parent)
        {
            var holder = BuilderKit.Node(parent, "Model");
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(model, holder);
            foreach (var r in inst.GetComponentsInChildren<Renderer>(true))
            {
                r.shadowCastingMode = ShadowCastingMode.On;
                r.lightProbeUsage = LightProbeUsage.Off; // nothing is baked; probes would only add cost
            }
            return holder;
        }

        /// <summary>
        /// Bounds of the meshes under a holder (LOD0 only) in the holder's space, or in another transform's space.
        /// Skinned meshes (clocks with a rigged pendulum) count at their bind pose.
        /// </summary>
        static Bounds LocalBounds(Transform holder, Transform space = null)
        {
            var toLocal = (space ?? holder).worldToLocalMatrix;
            bool any = false;
            var b = new Bounds();
            foreach (var r in holder.GetComponentsInChildren<Renderer>(true))
            {
                var mesh = r is SkinnedMeshRenderer skin ? skin.sharedMesh : r.GetComponent<MeshFilter>()?.sharedMesh;
                if (mesh == null) continue;
                if (r.GetComponentInParent<LODGroup>() != null && r.name.Contains("_LOD") && !r.name.EndsWith("_LOD0")) continue;
                Encapsulate(ref b, ref any, toLocal * r.transform.localToWorldMatrix, mesh.bounds);
            }
            return any ? b : new Bounds(Vector3.zero, Vector3.one);
        }

        static Bounds BoundsIn(Transform space, MeshRenderer[] renderers)
        {
            var toLocal = space.worldToLocalMatrix;
            bool any = false;
            var b = new Bounds();
            foreach (var r in renderers)
            {
                var f = r.GetComponent<MeshFilter>();
                if (f == null || f.sharedMesh == null) continue;
                Encapsulate(ref b, ref any, toLocal * r.transform.localToWorldMatrix, f.sharedMesh.bounds);
            }
            return b;
        }

        static void Encapsulate(ref Bounds b, ref bool any, Matrix4x4 m, Bounds mesh)
        {
            for (int i = 0; i < 8; i++)
            {
                var c = mesh.center + Vector3.Scale(mesh.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                var p = m.MultiplyPoint3x4(c);
                if (!any) { b = new Bounds(p, Vector3.zero); any = true; }
                else b.Encapsulate(p);
            }
        }

        // ------------------------------------------------------------ cobwebs

        public const string CobwebTexturePath = "Assets/Hollowmere/Art/Textures/Cobweb.png";

        /// <summary>
        /// A corner cobweb drawn by code (no asset of one was available): threads fanning from the top-left corner,
        /// sagging spiral strands between them, on transparent. Written once; delete the PNG to redraw it.
        /// </summary>
        public static Texture2D CobwebTexture()
        {
            var existing = AssetDatabase.LoadAssetAtPath<Texture2D>(CobwebTexturePath);
            if (existing != null) return existing;
            const int n = 512;
            var alpha = new float[n * n];
            var rng = new System.Random(77);
            void Splat(float x, float y, float a)
            {
                int cx = Mathf.RoundToInt(x), cy = Mathf.RoundToInt(y);
                for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    int px = cx + dx, py = cy + dy;
                    if (px < 0 || py < 0 || px >= n || py >= n) continue;
                    float d = Mathf.Sqrt((px - x) * (px - x) + (py - y) * (py - y));
                    float v = a * Mathf.Clamp01(1.3f - d);
                    int i = py * n + px;
                    alpha[i] = Mathf.Max(alpha[i], v);
                }
            }
            // Corner at (0, n): threads run down and right into the texture.
            int threads = 11;
            var angles = new float[threads];
            for (int t = 0; t < threads; t++)
                angles[t] = Mathf.Lerp(4f, 86f, t / (threads - 1f)) + (float)(rng.NextDouble() - 0.5) * 5f;
            foreach (var a in angles)
            {
                var dir = new Vector2(Mathf.Cos(a * Mathf.Deg2Rad), -Mathf.Sin(a * Mathf.Deg2Rad));
                float len = n * (0.85f + (float)rng.NextDouble() * 0.25f);
                for (float s = 0; s < len; s += 0.5f) Splat(dir.x * s, n - 1 + dir.y * s, 0.9f - 0.4f * s / len);
            }
            for (int ring = 1; ring <= 16; ring++)
            {
                float r = n * (0.05f + 0.055f * ring + (float)(rng.NextDouble() - 0.5) * 0.02f);
                for (int t = 0; t < threads - 1; t++)
                {
                    if (rng.NextDouble() < 0.12) continue; // broken strands
                    float a0 = angles[t] * Mathf.Deg2Rad, a1 = angles[t + 1] * Mathf.Deg2Rad;
                    var p0 = new Vector2(Mathf.Cos(a0), -Mathf.Sin(a0)) * r;
                    var p1 = new Vector2(Mathf.Cos(a1), -Mathf.Sin(a1)) * r;
                    float sag = r * 0.06f;
                    for (float u = 0; u <= 1f; u += 0.5f / Mathf.Max(8f, (p1 - p0).magnitude))
                    {
                        var p = Vector2.Lerp(p0, p1, u) + new Vector2(sag, -sag) * 0.7f * Mathf.Sin(u * Mathf.PI);
                        Splat(p.x, n - 1 + p.y, 0.65f);
                    }
                }
            }
            var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
            var px32 = new Color32[n * n];
            for (int i = 0; i < px32.Length; i++) px32[i] = new Color32(235, 235, 230, (byte)(alpha[i] * 255f));
            tex.SetPixels32(px32);
            BuilderKit.EnsureFolder(Path.GetDirectoryName(CobwebTexturePath).Replace('\\', '/'));
            File.WriteAllBytes(CobwebTexturePath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(CobwebTexturePath);
            var ti = (TextureImporter)AssetImporter.GetAtPath(CobwebTexturePath);
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(CobwebTexturePath);
        }

        public const string CobwebMaterialPath = "Assets/Hollowmere/Materials/Props/Cobweb.mat";
        public const string CobwebMeshPath = "Assets/Hollowmere/Art/Meshes/CobwebCorner.asset";

        public static Material CobwebMaterial()
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(CobwebMaterialPath);
            if (mat == null)
            {
                BuilderKit.EnsureFolder(Path.GetDirectoryName(CobwebMaterialPath).Replace('\\', '/'));
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, CobwebMaterialPath);
            }
            mat.SetColor("_BaseColor", new Color(0.85f, 0.85f, 0.82f, 1f));
            mat.SetTexture("_BaseMap", CobwebTexture());
            mat.SetFloat("_Smoothness", 0.2f);
            mat.SetFloat("_Cull", 0f); // both sides
            mat.doubleSidedGI = true;
            // Not reflective: otherwise the sky's reflection stays opaque across the clear parts (a grey sheet).
            ThirdPartyImport.MakeTransparent(mat, 0.75f, reflective: false);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        /// <summary>
        /// A unit corner web: the texture's top-left (where its threads start) at the origin, spreading along +x and +z
        /// under the ceiling and sagging down towards the far corner. Scale it by the web's size; a negative x or z
        /// scale mirrors it into the other corners.
        /// </summary>
        public static Mesh CobwebMesh()
        {
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(CobwebMeshPath);
            if (mesh != null) return mesh;
            mesh = new Mesh { name = "CobwebCorner" };
            mesh.vertices = new[] { new Vector3(0f, 0f, 0f), new Vector3(1f, -0.04f, 0f), new Vector3(0f, -0.04f, 1f), new Vector3(0.62f, -0.42f, 0.62f) };
            mesh.uv = new[] { new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 0f), new Vector2(1f, 0f) };
            mesh.triangles = new[] { 0, 1, 3, 0, 3, 2 };
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            BuilderKit.EnsureFolder(Path.GetDirectoryName(CobwebMeshPath).Replace('\\', '/'));
            AssetDatabase.CreateAsset(mesh, CobwebMeshPath);
            return mesh;
        }
    }
}
