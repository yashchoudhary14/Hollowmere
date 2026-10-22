using System.Collections.Generic;
using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The chapel bell, the bargain's clock (docs/DESIGN.md "Setting"): the day's warning toll, the 12 dusk tolls,
    /// the midnight count and the dawn bell. Tolls queue so several rung at once still sound one after another.
    /// </summary>
    public class BellTower : MonoBehaviour
    {
        public Transform Bell;
        [Tooltip("Seconds between queued tolls (the midnight count, or tolls a debug skip passed over).")]
        public float Spacing = 1.8f;

        public bool Ringing => _queue.Count > 0;

        readonly Queue<float> _queue = new Queue<float>();
        AudioSource _source;
        float _next;

        void Awake()
        {
            _source = Bell.gameObject.AddComponent<AudioSource>();
            _source.spatialBlend = 0.6f;          // placed in the world, but heard across the village
            _source.rolloffMode = AudioRolloffMode.Linear;
            _source.minDistance = 40f;
            _source.maxDistance = 450f;
        }

        public void Toll(float volume = 1f) => _queue.Enqueue(Mathf.Clamp01(volume));

        public void Toll(int times, float volume)
        {
            for (int i = 0; i < times; i++) Toll(volume);
        }

        void Update()
        {
            if (_queue.Count == 0 || Time.time < _next) return;
            float volume = _queue.Dequeue();
            _source.PlayOneShot(SoundBank.Bell, volume);
            var strike = SoundBank.BellStrike; // the recorded strike over the synthesised ring, when there is one
            if (strike != null) _source.PlayOneShot(strike, volume);
            _next = Time.time + Spacing;
        }
    }
}
