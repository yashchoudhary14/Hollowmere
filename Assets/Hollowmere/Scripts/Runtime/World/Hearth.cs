using UnityEngine;
using UnityEngine.AI;

namespace Hollowmere
{
    /// <summary>
    /// A house's hearth. Lit in allotted and shrine houses, cold in hollow houses (docs/DESIGN.md "Setting", "Dusk transition").
    /// The Hollow cannot cross a lit hearth, so a lit hearth's room is a safe zone (docs/DESIGN.md "The monster"):
    /// a carving NavMeshObstacle over the room keeps the Hollow's paths out, and Protects() tells it not to grab there.
    /// </summary>
    public class Hearth : MonoBehaviour
    {
        public House House;
        public GameObject Fire;
        public NavMeshObstacle SafeZone;

        public bool Lit { get; private set; }

        public void SetLit(bool lit)
        {
            Lit = lit;
            if (Fire != null) Fire.SetActive(lit);
            if (SafeZone != null) SafeZone.enabled = lit;
        }

        public bool Protects(Vector3 worldPos) => Lit && House != null && House.HearthRoom.Contains(worldPos);
    }
}
