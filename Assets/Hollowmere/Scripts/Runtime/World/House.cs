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
        public Transform DoorPivot;
        public GameObject ShrineMarker;
        public Transform[] NoteSpots;      // 3 per house, one per room
        public Transform[] ItemSpots;      // where small items like the winding key can be hidden
        public Transform PuzzleAnchor;
        public Transform ContainerAnchor;

        public HouseDef Def => HouseCatalog.Houses[Index];

        public bool Contains(Vector3 worldPos) => Interior.Contains(worldPos);

        public Room RoomAt(Vector3 worldPos)
        {
            foreach (var room in Rooms)
                if (room != null && room.Contains(worldPos)) return room;
            return null;
        }
    }
}
