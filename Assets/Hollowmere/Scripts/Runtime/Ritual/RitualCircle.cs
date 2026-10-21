using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The stone circle and its candles. With every part on the altar and every living player inside, the chant
    /// begins: 60 seconds, paused while anyone living is outside, back to zero whenever every candle is out.
    /// When it completes the gate opens. Design: docs/DESIGN.md "The Ritual of Passage".
    /// </summary>
    public class RitualCircle : MonoBehaviour
    {
        public Candle[] Candles;
        public float Radius = 7f;
        [Tooltip("DESIGN.md \"The Ritual of Passage\": a 60-second chant.")]
        public float ChantSeconds = 60f;

        public ChantState Chant { get; private set; } = new ChantState();
        public ChantStatus Status { get; private set; }

        MatchDirector _director;
        Altar _altar;
        AudioSource _drone;

        public void Setup(MatchDirector director, Altar altar)
        {
            _director = director;
            _altar = altar;
            Chant = new ChantState { Seconds = ChantSeconds };
            Status = ChantStatus.Waiting;
            int used = director.Plan.AltarOrder.Length;
            for (int i = 0; i < Candles.Length; i++) Candles[i].Setup(this, i < used);
        }

        public bool Contains(Vector3 worldPos)
        {
            var d = worldPos - transform.position;
            d.y = 0f;
            return d.magnitude <= Radius;
        }

        public int LitCount
        {
            get
            {
                int n = 0;
                foreach (var c in Candles) if (c.InUse && c.Lit) n++;
                return n;
            }
        }

        public void SnuffAll()
        {
            foreach (var c in Candles) c.Snuff();
        }

        void Update()
        {
            if (_director == null || _director.Outcome != MatchOutcome.Running || Chant.Complete) return;

            int living = 0, inside = 0;
            foreach (var p in _director.Players)
            {
                if (!p.IsAlive) continue;
                living++;
                if (Contains(p.transform.position)) inside++;
            }

            if (!Chant.Started)
            {
                if (!ChantState.CanStart(_altar.AllPlaced, living, inside)) return;
                Begin();
            }

            var status = Chant.Tick(Time.deltaTime, living > 0 && inside == living, LitCount);
            if (status != Status) Announce(status);
            Status = status;

            if (Chant.Complete)
            {
                if (_drone != null) _drone.Stop();
                _director.OpenGate();
            }
        }

        void Begin()
        {
            Chant.Begin();
            foreach (var c in Candles) if (c.InUse) c.SetLit(true);
            if (_drone == null)
            {
                _drone = gameObject.AddComponent<AudioSource>();
                _drone.clip = SoundBank.Chant;
                _drone.loop = true;
                _drone.spatialBlend = 0.5f;
                _drone.minDistance = 10f;
                _drone.maxDistance = 80f;
                _drone.volume = 0.6f;
            }
            _drone.Play();
            _director.NotifyAll("The chant begins. Keep the candles burning for a minute, and stay within the stones.");
            _director.RaiseChantStarted();
        }

        void Announce(ChantStatus status)
        {
            switch (status)
            {
                case ChantStatus.Paused: _director.NotifyAll("The chant falters. Everyone living must stand within the stones."); break;
                case ChantStatus.Reset: _director.NotifyAll("Every candle is out. The chant starts again from the beginning."); break;
                case ChantStatus.Chanting:
                    if (Status == ChantStatus.Paused || Status == ChantStatus.Reset) _director.NotifyAll("The chant goes on.");
                    break;
            }
        }
    }
}
