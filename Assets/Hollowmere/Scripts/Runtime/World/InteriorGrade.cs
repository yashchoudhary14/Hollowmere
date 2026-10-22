using UnityEngine;
using UnityEngine.Rendering;

namespace Hollowmere
{
    /// <summary>
    /// Look pass (M1.5): indoors at night the eye adjusts to the lamps, so a warmer, less exposed grade fades in while
    /// the camera is inside a house. Uses the houses' interior bounds, not trigger colliders, so nothing physical is
    /// added for it; purely visual.
    /// </summary>
    [LookOnly]
    public class InteriorGrade : MonoBehaviour
    {
        public PhaseLighting Lighting;
        public Volume InteriorNight;
        public House[] Houses;
        [Tooltip("Seconds to adjust when stepping in or out.")]
        public float BlendSeconds = 0.8f;

        float _inside;

        void LateUpdate()
        {
            var cam = Camera.main;
            if (cam == null) return;
            _inside = Mathf.MoveTowards(_inside, Inside(cam.transform.position) ? 1f : 0f, Time.deltaTime / Mathf.Max(0.01f, BlendSeconds));
            Apply();
        }

        public bool Inside(Vector3 worldPos)
        {
            if (Houses == null) return false;
            foreach (var h in Houses)
                if (h != null && h.Contains(worldPos)) return true;
            return false;
        }

        /// <summary>Jump straight to the grade for a camera at this position (shots, teleports).</summary>
        public void Snap(Vector3 cameraPos)
        {
            _inside = Inside(cameraPos) ? 1f : 0f;
            Apply();
        }

        void Apply()
        {
            if (InteriorNight != null) InteriorNight.weight = _inside * (Lighting != null ? Lighting.NightAmount : 1f);
        }
    }
}
