using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The village gate: open on arrival, shut behind the guests, opened again by the Ritual of Passage
    /// (docs/DESIGN.md "Core loop", "Escape"). The leaves carry carving NavMeshObstacles.
    /// </summary>
    public class VillageGate : MonoBehaviour
    {
        public Transform LeafWest;
        public Transform LeafEast;
        public float SwingSeconds = 3f;

        public bool IsOpen { get; private set; }

        float _open01, _target;

        void Update()
        {
            if (Mathf.Approximately(_open01, _target)) return;
            _open01 = Mathf.MoveTowards(_open01, _target, Time.deltaTime / SwingSeconds);
            Apply();
        }

        public void SetOpen(bool open, bool instant = false)
        {
            IsOpen = open;
            _target = open ? 1f : 0f;
            if (instant)
            {
                _open01 = _target;
                Apply();
            }
            else
            {
                SoundBank.PlayAt(SoundBank.Creak, transform.position + Vector3.up * 2f, 1f, 80f, 0.55f);
            }
        }

        void Apply()
        {
            // Both leaves swing outwards, to the north.
            LeafWest.localRotation = Quaternion.Euler(0f, -90f * _open01, 0f);
            LeafEast.localRotation = Quaternion.Euler(0f, 90f * _open01, 0f);
        }
    }
}
