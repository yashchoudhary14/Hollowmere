using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// One of the 15 houses. GreyboxVillageBuilder fills in the references; MatchDirector applies
    /// the match state (allotted, shrine, hearth). Design: docs/DESIGN.md "Houses and puzzles".
    /// </summary>
    public class House : MonoBehaviour
    {
        public int Index;
        public Bounds Interior;            // world space, everything inside the outer walls
        public Room[] Rooms;
        public Room HearthRoom;
        public Door Door;
        public Hearth Hearth;
        public FuseBox FuseBox;
        public HidingSpot Wardrobe;
        public GameObject ShrineMarker;
        public Readable[] Notes;           // 3 per house, one per room
        public Transform[] ItemSpots;      // where small items like the winding key can be hidden
        public Transform PuzzleAnchor;
        public Transform ContainerAnchor;

        public HouseDef Def => HouseCatalog.Houses[Index];
        public bool Powered => FuseBox == null || FuseBox.Powered;

        public bool Contains(Vector3 worldPos) => Interior.Contains(worldPos);

        public Room RoomAt(Vector3 worldPos)
        {
            foreach (var room in Rooms)
                if (room != null && room.Contains(worldPos)) return room;
            return null;
        }

        public void RefreshLights()
        {
            foreach (var room in Rooms) room.Refresh(Powered);
        }

        public void SetLightsOn(bool on)
        {
            foreach (var room in Rooms) room.SwitchOn = on;
            RefreshLights();
        }

        /// <summary>Whether a spot inside is lit: a room light, or the fire in the hearth room.</summary>
        public bool IsLitAt(Vector3 worldPos)
        {
            var room = RoomAt(worldPos);
            if (room == null) return false;
            return room.IsLit || (room.IsHearthRoom && Hearth != null && Hearth.Lit);
        }
    }
}
