using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Surface art for the builder's materials, keyed by the names the builders pass to <c>BuilderKit.Mat()</c>
    /// ("HouseWall", "Floor", "Door_*", ...). An entry without a base map leaves that material as the builder's flat
    /// colour, so a missing texture never breaks a build. Textures tile by world size: builder meshes carry UVs in
    /// metres and the material repeats every <see cref="Entry.MetresPerTile"/>.
    /// The asset is seeded by <c>MaterialLibraryBuilder</c>; entries with <see cref="Entry.Auto"/> off are left as edited.
    /// </summary>
    [CreateAssetMenu(menuName = "Hollowmere/Material Library", fileName = "MaterialLibrary")]
    public class MaterialLibrary : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            [Tooltip("The Mat() name this entry dresses. A trailing * matches every name with that prefix (Door_*).")]
            public string Key;
            [Tooltip("On: the builder refreshes this entry from its defaults on every build. Off: kept exactly as edited.")]
            public bool Auto = true;

            public Texture2D BaseMap;
            public Texture2D NormalMap;
            [Tooltip("URP metallic (R) / smoothness (A) map. Third-party arm and roughness maps are swizzled into this layout on import.")]
            public Texture2D MetallicGlossMap;
            [Tooltip("Occlusion read from G. A Poly Haven arm map serves as both this and the metallic/smoothness map.")]
            public Texture2D OcclusionMap;

            [Tooltip("World size of one texture repeat, in metres.")]
            public float MetresPerTile = 2f;
            [Tooltip("Multiplies the base map.")]
            public Color Tint = Color.white;
            [Tooltip("Scale the base map so its average colour matches the builder's colour (door colours are a clue).")]
            public bool KeepBuilderColour;
            public float NormalScale = 1f;
            [Tooltip("Multiplies the map's smoothness; with no map, replaces the builder's smoothness when 0 or more.")]
            public float Smoothness = 0.85f;
            [Tooltip("Metallic for materials without a metallic map; below 0 keeps the builder's (0).")]
            public float Metallic = -1f;
            public float OcclusionStrength = 1f;

            public bool HasTextures => BaseMap != null;
        }

        public List<Entry> Entries = new List<Entry>();

        /// <summary>The entry for a material name: an exact key first, then the longest matching "prefix*" key.</summary>
        public Entry Find(string name)
        {
            Entry best = null;
            int bestLength = -1;
            foreach (var e in Entries)
            {
                if (e == null || string.IsNullOrEmpty(e.Key)) continue;
                if (e.Key == name) return e;
                if (e.Key.EndsWith("*"))
                {
                    var prefix = e.Key.Substring(0, e.Key.Length - 1);
                    if (name.StartsWith(prefix, StringComparison.Ordinal) && prefix.Length > bestLength)
                    {
                        best = e;
                        bestLength = prefix.Length;
                    }
                }
            }
            return best;
        }
    }
}
