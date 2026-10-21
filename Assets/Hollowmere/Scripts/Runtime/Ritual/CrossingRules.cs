using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The Crossing's one rule: turning to look back (more than 120° from the road, for half a second) means the
    /// Hollow takes you. Unit-tested. Design: docs/DESIGN.md "The Crossing"; "Decisions": looking back kills you.
    /// </summary>
    public static class CrossingRules
    {
        public const float LookBackDegrees = 120f;
        public const float LookBackSeconds = 0.5f;

        /// <summary>Degrees between where the player faces and the road ahead, ignoring up and down.</summary>
        public static float AngleFromRoad(Vector3 facing, Vector3 roadAhead)
        {
            facing.y = 0f;
            roadAhead.y = 0f;
            return Vector3.Angle(facing, roadAhead);
        }

        /// <summary>How long the player has been looking back, after another frame of dt seconds.</summary>
        public static float UpdateLookBack(float lookingBackFor, float angle, float dt)
            => angle > LookBackDegrees ? lookingBackFor + dt : 0f;

        public static bool IsTaken(float lookingBackFor) => lookingBackFor >= LookBackSeconds;
    }
}
