using UnityEngine;

namespace Hollowmere
{
    /// <summary>One room of a house: its volume, ceiling light and switch state. Built by GreyboxVillageBuilder.</summary>
    public class Room : MonoBehaviour
    {
        public House House;
        public string Id;              // the room's id in docs/design/houses/Hxx.json (v2 houses)
        public Bounds Bounds;          // world space (for a turned house, the box round the room)
        [Tooltip("v2 houses stand at any angle: the room's exact volume in its own space, used instead of Bounds.")]
        public bool Oriented;
        public Bounds LocalBounds;
        public Light Light;
        public GameObject Bulb;
        public bool IsHearthRoom;
        public bool SwitchOn = true;

        public bool Contains(Vector3 worldPos) =>
            Oriented ? LocalBounds.Contains(transform.InverseTransformPoint(worldPos)) : Bounds.Contains(worldPos);

        public bool IsLit => Light != null && Light.enabled;

        public void Refresh(bool powered)
        {
            bool on = powered && SwitchOn;
            if (Light != null) Light.enabled = on;
            if (Bulb != null) Bulb.SetActive(on);
        }
    }
}
