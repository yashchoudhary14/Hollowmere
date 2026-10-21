using UnityEngine;

namespace Hollowmere
{
    /// <summary>One room of a house: its volume, ceiling light and switch state. Built by GreyboxVillageBuilder.</summary>
    public class Room : MonoBehaviour
    {
        public House House;
        public Bounds Bounds;          // world space
        public Light Light;
        public GameObject Bulb;
        public bool IsHearthRoom;
        public bool SwitchOn = true;

        public bool Contains(Vector3 worldPos) => Bounds.Contains(worldPos);

        public bool IsLit => Light != null && Light.enabled;

        public void Refresh(bool powered)
        {
            bool on = powered && SwitchOn;
            if (Light != null) Light.enabled = on;
            if (Bulb != null) Bulb.SetActive(on);
        }
    }
}
