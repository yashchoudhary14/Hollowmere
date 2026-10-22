using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// A cosmetic flame flicker: wobbles the light's intensity around the value it started with. It never turns the
    /// light on or off, so anything that reads a light's state (a room counting as lit) is unaffected.
    /// The noise is seeded from the light's position, so it needs no random numbers.
    /// </summary>
    [LookOnly]
    [RequireComponent(typeof(Light))]
    public class LightFlicker : MonoBehaviour
    {
        [Tooltip("Fraction of the intensity the flicker swings by.")]
        public float Amount = 0.15f;
        [Tooltip("How fast the flicker moves.")]
        public float Speed = 7f;

        Light _light;
        float _base;
        float _seed;

        void Awake()
        {
            _light = GetComponent<Light>();
            _base = _light.intensity;
            var p = transform.position;
            _seed = Mathf.Repeat(p.x * 12.9898f + p.z * 78.233f, 100f);
        }

        void Update()
        {
            float t = Time.time * Speed;
            float n = Mathf.PerlinNoise(t, _seed) * 0.7f + Mathf.PerlinNoise(t * 2.7f, _seed + 31f) * 0.3f;
            _light.intensity = _base * (1f + Amount * (2f * n - 1f));
        }
    }
}
