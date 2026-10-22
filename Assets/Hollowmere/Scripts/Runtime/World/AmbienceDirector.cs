using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Ambience (M1.5), following docs/DESIGN.md "Audio and atmosphere" and "Day phase": birds and insects by day
    /// until minute 10, when the birds go silent and the wind rises; wind through dusk and night; indoors, room tone and
    /// clock ticks, with wind in the chimney at night. "Silence is a tool", so the beds stay low. Optional rain in
    /// Act 3. Outdoor beds are muffled indoors. Distant dogs by day and dusk, a house settling now and then at night.
    /// None of this goes through NoiseSystem: it is atmosphere the Hollow doesn't hear.
    /// Also hands the scene's SoundLibrary to SoundBank.
    /// </summary>
    [LookOnly]
    public class AmbienceDirector : MonoBehaviour
    {
        public SoundLibrary Library;
        public MatchDirector Director;
        public House[] Houses;

        [Header("Levels")]
        public float DayVolume = 0.32f;
        public float WindVolume = 0.3f;
        public float NightWindVolume = 0.42f;
        public float RoomToneVolume = 0.14f;
        public float ClockVolume = 0.12f;
        public float ChimneyVolume = 0.22f;
        [Tooltip("How much of the outdoor ambience still comes through the walls.")]
        public float Muffle = 0.25f;

        [Header("Rain (not in DESIGN.md yet)")]
        [Tooltip("Rain outside and on the windows in Act 3, as the fog thickens.")]
        public bool RainInAct3 = true;
        public float RainVolume = 0.32f;

        [Header("Occasional sounds")]
        public Vector2 DogEverySeconds = new Vector2(45f, 110f);
        public Vector2 CreakEverySeconds = new Vector2(35f, 90f);

        AudioSource _day, _wind, _rain, _room, _clock, _chimney, _rainWindow;
        float _inside;
        float _nextDog, _nextCreak;
        int _count;

        void Awake()
        {
            SoundBank.Library = Library;
            if (Library == null) return;
            _day = Bed("Day", Library.DayOutdoors);
            _wind = Bed("Wind", Library.NightWind);
            _rain = Bed("Rain", Library.RainOutdoors);
            _room = Bed("RoomTone", Library.RoomTone);
            _clock = Bed("ClockTicks", Library.ClockTicks);
            _chimney = Bed("ChimneyWind", Library.ChimneyWind);
            _rainWindow = Bed("RainOnWindows", Library.RainWindow);
            _nextDog = Interval(DogEverySeconds);
            _nextCreak = Interval(CreakEverySeconds);
        }

        AudioSource Bed(string name, AudioClip clip)
        {
            if (clip == null) return null;
            var go = new GameObject("Ambience " + name);
            go.transform.SetParent(transform, false);
            var s = go.AddComponent<AudioSource>();
            s.clip = clip;
            s.loop = true;
            s.spatialBlend = 0f;
            s.volume = 0f;
            s.playOnAwake = false;
            s.Play();
            return s;
        }

        void Update()
        {
            if (Library == null || Director == null) return;
            var listener = Camera.main != null ? Camera.main.transform.position : transform.position;
            _inside = Mathf.MoveTowards(_inside, Inside(listener) ? 1f : 0f, Time.deltaTime * 1.5f);
            var now = Director.Now;

            // Outdoors: birds until minute 10 of the day, then wind rising into dusk and night.
            float birds = 0f, wind = 0f, rain = 0f, chimney = 0f;
            switch (now.Phase)
            {
                case MatchPhase.Arrival:
                    birds = 1f;
                    break;
                case MatchPhase.Day:
                    float quiet = Director.Timeline.DaySeconds - 120f; // DESIGN.md: at 10 min of the 12-minute day
                    birds = 1f - Mathf.Clamp01((now.PhaseElapsed - quiet) / 20f);
                    wind = Mathf.Clamp01((now.PhaseElapsed - quiet) / 60f) * WindVolume;
                    break;
                case MatchPhase.Dusk:
                    wind = Mathf.Lerp(WindVolume, NightWindVolume, now.Phase01);
                    chimney = now.Phase01;
                    break;
                default:
                    wind = NightWindVolume;
                    chimney = 1f;
                    if (RainInAct3 && now.Act >= 3) rain = Mathf.Clamp01((Director.Clock - MatchTimeline.Act3Start(Director.Timeline)) / 20f);
                    break;
            }
            float outside = Mathf.Lerp(1f, Muffle, _inside);
            Set(_day, birds * DayVolume * outside);
            Set(_wind, wind * outside);
            Set(_rain, rain * RainVolume * outside);
            Set(_room, RoomToneVolume * _inside);
            Set(_clock, ClockVolume * _inside);
            Set(_chimney, chimney * ChimneyVolume * _inside);
            Set(_rainWindow, rain * RainVolume * _inside);

            bool night = now.Phase == MatchPhase.Night || now.Phase == MatchPhase.Dawn;
            if (Time.time >= _nextDog)
            {
                _nextDog = Time.time + Interval(DogEverySeconds);
                if (!night) Distant(Library.DistantDogs, listener, 0.35f * outside);
            }
            if (Time.time >= _nextCreak)
            {
                _nextCreak = Time.time + Interval(CreakEverySeconds);
                if (night && _inside > 0.5f) Distant(Library.HouseCreaks, listener, 0.3f, 6f);
            }
        }

        static void Set(AudioSource s, float volume)
        {
            if (s != null) s.volume = Mathf.MoveTowards(s.volume, volume, Time.deltaTime * 0.25f);
        }

        bool Inside(Vector3 p)
        {
            if (Houses == null) return false;
            foreach (var h in Houses)
                if (h != null && h.Contains(p)) return true;
            return false;
        }

        /// <summary>A one-shot from somewhere around the listener, `distance` metres off.</summary>
        void Distant(AudioClip[] clips, Vector3 listener, float volume, float distance = 90f)
        {
            if (clips == null || clips.Length == 0 || volume <= 0.01f) return;
            var clip = clips[_count % clips.Length];
            float angle = Hash(_count * 7 + 3) * Mathf.PI * 2f;
            _count++;
            if (clip == null) return;
            var at = listener + new Vector3(Mathf.Cos(angle), 0.5f, Mathf.Sin(angle)) * distance;
            SoundBank.PlayAt(clip, at, volume, distance * 2.5f);
        }

        /// <summary>Spacing for occasional sounds: a fixed sequence through the range (no match randomness needed).</summary>
        float Interval(Vector2 range) => Mathf.Lerp(range.x, range.y, Hash(++_count));

        static float Hash(int n) => Mathf.Repeat(Mathf.Sin(n * 12.9898f) * 43758.5453f, 1f);
    }
}
