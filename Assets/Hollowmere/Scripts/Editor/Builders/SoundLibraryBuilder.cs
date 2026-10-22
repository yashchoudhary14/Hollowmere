using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Fills Assets/Hollowmere/Audio/SoundLibrary.asset from the clips tools/prepare-audio.ps1 cut into
    /// Assets/Hollowmere/Audio and the footstep pack in Assets/ThirdParty/AssetStore/EssentialFootsteps.
    /// Idempotent: every slot is rewritten from the files on every run; a missing file leaves its slot empty and
    /// SoundBank falls back to its synthesised sound.
    /// </summary>
    public static class SoundLibraryBuilder
    {
        public const string LibraryPath = "Assets/Hollowmere/Audio/SoundLibrary.asset";
        const string A = "Assets/Hollowmere/Audio/";
        const string Steps = "Assets/ThirdParty/AssetStore/EssentialFootsteps/";

        [MenuItem("Hollowmere/Build Sound Library")]
        public static SoundLibrary Ensure()
        {
            var lib = AssetDatabase.LoadAssetAtPath<SoundLibrary>(LibraryPath);
            if (lib == null)
            {
                lib = ScriptableObject.CreateInstance<SoundLibrary>();
                AssetDatabase.CreateAsset(lib, LibraryPath);
            }

            lib.BellStrike = Clips("Bells/BellToll_A", "Bells/BellToll_B");
            lib.DoorCreak = Clips("Doors/Creak_A", "Doors/Creak_B");
            lib.GateGroan = Clips("Doors/GateGroan");
            lib.Knock = Clips("Doors/Knock_A", "Doors/Knock_B");
            lib.Thud = Clips("Impacts/Thud_A", "Impacts/BooksFall");
            lib.HeavyThud = Clips("Impacts/Thud_B");
            lib.Gasp = Clips("Voice/Gasp");
            lib.Chime = Clips("Puzzle/Chime_A", "Puzzle/Chime_B");
            lib.Clunk = Clips("Puzzle/Clunk");
            lib.HollowBreath = Clips("Hollow/Breath_A", "Hollow/Breath_B");
            lib.HollowExhale = Clips("Hollow/Exhale");
            lib.Chant = Clip("Ritual/ChantDrone");

            lib.StepWood = Pack("Wood", "Footsteps", "Footstep_Wood_Boots_Walk_");
            lib.StepStone = Pack("Concrete", "Footsteps", "Footstep_Concrete_Boots_Walk_");
            lib.StepDirt = Pack("Dirt", "Footsteps", "Footstep_Dirt_Boots_Walk_");
            lib.StepLeaves = Pack("Leaves", "Footsteps", "Footstep_FootstepLeaves_Boots_Walk_");
            lib.LandWood = Pack("Wood", "Jump_Land", "Footstep_Wood_Boots_Land_");
            lib.LandStone = Pack("Concrete", "Jump_Land", "Footstep_Concrete_Boots_Land_");
            lib.LandDirt = Pack("Dirt", "Jump_Land", "Footstep_Dirt_Boots_Land_");
            lib.LandLeaves = Pack("Leaves", "Jump_Land", "Footstep_FootstepLeaves_Boots_Land_");

            lib.DayOutdoors = Clip("Ambience/DayOutdoors");
            lib.NightWind = Clip("Ambience/NightWind");
            lib.RainOutdoors = Clip("Ambience/RainOutdoors");
            lib.RoomTone = Clip("Ambience/RoomTone");
            lib.ClockTicks = Clip("Ambience/ClockTicks");
            lib.ChimneyWind = Clip("Ambience/ChimneyWind");
            lib.RainWindow = Clip("Ambience/RainWindow");
            lib.HearthFire = Clip("Ambience/HearthFire");
            lib.DistantDogs = Clips("Ambience/DogsDistant");
            lib.HouseCreaks = Clips("Impacts/HouseCreak");

            EditorUtility.SetDirty(lib);
            AssetDatabase.SaveAssets();
            return lib;
        }

        static AudioClip Clip(string name) => AssetDatabase.LoadAssetAtPath<AudioClip>($"{A}{name}.wav");

        static AudioClip[] Clips(params string[] names) => names.Select(Clip).Where(c => c != null).ToArray();

        /// <summary>Numbered clips 1, 2, ... from the footstep pack, as many as there are.</summary>
        static AudioClip[] Pack(string surface, string folder, string prefix)
            => Enumerable.Range(1, 12)
                .Select(i => AssetDatabase.LoadAssetAtPath<AudioClip>($"{Steps}{surface}/{folder}/{prefix}{i}.wav"))
                .Where(c => c != null).ToArray();
    }
}
