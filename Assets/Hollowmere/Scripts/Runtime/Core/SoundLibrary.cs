using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The recorded sounds (M1.5). SoundBank plays these when they are set and falls back to its synthesised clips
    /// for anything missing, so the game never goes silent for want of a file. Arrays hold variations, played in turn.
    /// Filled by SoundLibraryBuilder from Assets/Hollowmere/Audio and the footstep pack.
    /// </summary>
    [CreateAssetMenu(menuName = "Hollowmere/Sound Library", fileName = "SoundLibrary")]
    public class SoundLibrary : ScriptableObject
    {
        [Header("One-shots")]
        [Tooltip("The strike of a toll; the synthesised bell rings on under it.")]
        public AudioClip[] BellStrike;
        public AudioClip[] DoorCreak;
        public AudioClip[] GateGroan;
        [Tooltip("A single knock; SoundBank repeats it into a knocking.")]
        public AudioClip[] Knock;
        public AudioClip[] Thud;
        public AudioClip[] HeavyThud;
        public AudioClip[] Gasp;
        public AudioClip[] Chime;
        public AudioClip[] Clunk;
        public AudioClip[] HollowBreath;
        public AudioClip[] HollowExhale;
        public AudioClip Chant;

        [Header("Footsteps by surface")]
        public AudioClip[] StepWood;
        public AudioClip[] StepStone;
        public AudioClip[] StepDirt;
        public AudioClip[] StepLeaves;
        public AudioClip[] LandWood;
        public AudioClip[] LandStone;
        public AudioClip[] LandDirt;
        public AudioClip[] LandLeaves;

        [Header("Ambience loops")]
        public AudioClip DayOutdoors;
        public AudioClip NightWind;
        public AudioClip RainOutdoors;
        public AudioClip RoomTone;
        public AudioClip ClockTicks;
        public AudioClip ChimneyWind;
        public AudioClip RainWindow;
        public AudioClip HearthFire;

        [Header("Ambience one-shots")]
        public AudioClip[] DistantDogs;
        public AudioClip[] HouseCreaks;
    }
}
