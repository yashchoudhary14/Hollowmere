using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Settings carried across a restart's scene reload. This is not match state: MatchDirector reads and
    /// clears it in Awake. Milestone 2's lobby replaces it.
    /// </summary>
    public static class MatchLaunch
    {
        public struct Settings
        {
            public int Seed;
            public int Players;
            public float ClockSpeed;
        }

        public static Settings? Next;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => Next = null;
    }
}
