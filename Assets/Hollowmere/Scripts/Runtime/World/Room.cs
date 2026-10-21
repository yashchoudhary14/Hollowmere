using UnityEngine;

namespace Hollowmere
{
    /// <summary>One room of a house: its volume and ceiling light. Built by GreyboxVillageBuilder.</summary>
    public class Room : MonoBehaviour
    {
        public House House;
        public Bounds Bounds;          // world space
        public Light Light;
        public bool IsHearthRoom;

        public bool Contains(Vector3 worldPos) => Bounds.Contains(worldPos);
    }
}
