using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Seeds Assets/Hollowmere/Art/MaterialLibrary.asset: which third-party texture set dresses each builder material,
    /// at what world size, with what tint. Idempotent: entries marked Auto are refreshed from the table below on every
    /// build; entries with Auto turned off, and entries not in the table, are left as they are.
    /// A texture set that isn't in the project simply leaves its material flat.
    /// </summary>
    public static class MaterialLibraryBuilder
    {
        public const string LibraryPath = "Assets/Hollowmere/Art/MaterialLibrary.asset";

        struct Default
        {
            public string Key, Folder;
            public float MetresPerTile, Smoothness, Metallic;
            public Color Tint;
            public bool KeepBuilderColour;
        }

        static Default Set(string key, string folder, float metresPerTile, Color? tint = null, bool keepBuilderColour = false, float smoothness = 0.85f)
            => new Default
            {
                Key = key, Folder = folder, MetresPerTile = metresPerTile, Tint = tint ?? Color.white,
                KeepBuilderColour = keepBuilderColour, Smoothness = smoothness, Metallic = -1f,
            };

        static Default Flat(string key, float smoothness, float metallic)
            => new Default { Key = key, MetresPerTile = 1f, Tint = Color.white, Smoothness = smoothness, Metallic = metallic };

        static Color Grey(float v) => new Color(v, v, v);

        // Keys are the names the builders pass to BuilderKit.Mat(). Folders are under Assets/ThirdParty.
        static readonly Default[] Defaults =
        {
            // Outdoors
            Set("Ground", "PolyHaven/forrest_ground_01", 4f, Grey(0.9f)),
            Set("Grass", "AssetStore/TerrainTexturesFree/Grass 09", 3f),
            Set("Garden", "AssetStore/TerrainTexturesFree/Grass 06", 3f, Grey(0.7f)),
            Set("LeafLitter", "AssetStore/TerrainTexturesFree/Leaves 02", 2f, Grey(0.8f)),
            Set("Road", "PolyHaven/brown_mud_leaves_01", 4f),
            Set("Paving", "PolyHaven/mossy_cobblestone", 4f),
            Set("Stone", "PolyHaven/stone_wall", 3f),
            Set("StandingStone", "AmbientCG/Rock034", 2f, Grey(0.85f)),
            Set("AltarStone", "AmbientCG/Rock034", 1.5f, Grey(0.6f)),
            Set("Trunk", "PolyHaven/bark_brown_02", 2f, Grey(0.8f)),
            Set("Bark", "PolyHaven/bark_brown_02", 2f, Grey(0.8f)),
            Set("Planks", "PolyHaven/brown_planks_03", 2f),
            Set("GateWood", "PolyHaven/brown_planks_03", 2f, Grey(0.7f)),
            Set("BridgeWood", "PolyHaven/brown_planks_03", 2f, Grey(0.85f)),

            // Buildings
            Set("HouseWall", "PolyHaven/grey_plaster", 3f, new Color(1f, 0.94f, 0.84f)),
            Set("InnerWall", "PolyHaven/white_plaster_02", 2.5f, new Color(0.92f, 0.85f, 0.74f)),
            Set("Roof", "PolyHaven/ceramic_roof_01", 2.5f),
            Set("ChapelRoof", "PolyHaven/roof_slates_03", 2.5f),
            Set("ChapelWall", "PolyHaven/castle_wall_slates", 3f),
            Set("Chimney", "PolyHaven/castle_brick_02_red", 1.5f),
            Set("Floor", "AmbientCG/WoodFloor043", 3f, new Color(0.8f, 0.75f, 0.7f)),
            Set("Ceiling", "PolyHaven/brown_planks_03", 2.5f, Grey(0.75f)),
            Set("HearthStone", "PolyHaven/stone_brick_wall_001", 1.5f),
            Set("Door_*", "PolyHaven/wood_peeling_paint_weathered", 1.5f, keepBuilderColour: true),

            // Furniture and small things
            Set("Wood", "AmbientCG/WoodFloor043", 1.5f, new Color(0.6f, 0.55f, 0.5f)),
            Set("ChestWood", "AmbientCG/WoodFloor043", 1.2f, new Color(0.7f, 0.62f, 0.55f)),
            Set("Metal", "PolyHaven/rusty_metal_04", 0.8f),
            Set("BookLeather", "PolyHaven/brown_leather", 0.5f),
            Set("Cloth", "PolyHaven/fabric_pattern_05", 1f),
            Flat("Bronze", 0.55f, 0.9f),
            Flat("Brass", 0.6f, 1f),
            Flat("Gold", 0.7f, 1f),
        };

        /// <summary>Loads the library (creating it if needed) and refreshes its Auto entries.</summary>
        [MenuItem("Hollowmere/Seed Material Library")]
        public static MaterialLibrary Ensure()
        {
            var lib = AssetDatabase.LoadAssetAtPath<MaterialLibrary>(LibraryPath);
            if (lib == null)
            {
                BuilderKit.EnsureFolder(Path.GetDirectoryName(LibraryPath).Replace('\\', '/'));
                lib = ScriptableObject.CreateInstance<MaterialLibrary>();
                AssetDatabase.CreateAsset(lib, LibraryPath);
            }

            foreach (var d in Defaults)
            {
                var e = lib.Entries.FirstOrDefault(x => x != null && x.Key == d.Key);
                if (e == null)
                {
                    e = new MaterialLibrary.Entry { Key = d.Key };
                    lib.Entries.Add(e);
                }
                if (!e.Auto) continue;
                var maps = FindMaps(d.Folder);
                e.BaseMap = maps.colour;
                e.NormalMap = maps.normal;
                e.MetallicGlossMap = maps.gloss;
                e.OcclusionMap = maps.occlusion;
                e.MetresPerTile = d.MetresPerTile;
                e.Tint = d.Tint;
                e.KeepBuilderColour = d.KeepBuilderColour;
                e.NormalScale = 1f;
                e.Smoothness = d.Smoothness;
                e.Metallic = d.Metallic;
                e.OcclusionStrength = 1f;
            }
            EditorUtility.SetDirty(lib);
            AssetDatabase.SaveAssets();
            return lib;
        }

        /// <summary>The colour, normal, metallic/smoothness and occlusion maps of a texture set folder under Assets/ThirdParty.</summary>
        public static (Texture2D colour, Texture2D normal, Texture2D gloss, Texture2D occlusion) FindMaps(string folder)
        {
            if (string.IsNullOrEmpty(folder)) return default;
            var path = ThirdPartyImport.Root + folder;
            if (!AssetDatabase.IsValidFolder(path)) return default;
            var files = Directory.GetFiles(path).Where(f => !f.EndsWith(".meta")).Select(f => f.Replace('\\', '/')).ToArray();
            Texture2D Find(ThirdPartyImport.MapKind kind)
            {
                var f = files.FirstOrDefault(x => ThirdPartyImport.Classify(x) == kind);
                return f == null ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(f);
            }
            var arm = Find(ThirdPartyImport.MapKind.PackedArm);
            return (Find(ThirdPartyImport.MapKind.Colour), Find(ThirdPartyImport.MapKind.Normal),
                    arm ?? Find(ThirdPartyImport.MapKind.Roughness), arm ?? Find(ThirdPartyImport.MapKind.Occlusion));
        }
    }
}
