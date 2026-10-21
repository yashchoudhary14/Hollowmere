using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Named places in the village that gameplay needs: spawns, the Hollow's roam route, the square.
    /// Built by GreyboxVillageBuilder. Design: docs/DESIGN.md "The village map".
    /// </summary>
    public class VillageLayout : MonoBehaviour
    {
        public Transform PlayerSpawn;
        public Transform HollowNightSpawn;     // the chapel: where the Hollow first appears each night
        public Transform[] ForestSpawns;       // just inside the wall, where the Hollow comes from
        public Transform[] RoamPoints;         // street crossings the Hollow patrols
        public Transform SquareCenter;         // the altar
        public float CircleRadius = 7f;        // stone circle around the altar
        public float GateLineZ = 150f;         // world z of the village gate
        public float BridgeZ = 352f;           // world z where the fog road reaches the bridge
    }
}
