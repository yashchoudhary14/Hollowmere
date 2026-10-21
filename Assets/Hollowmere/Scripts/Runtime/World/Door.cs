using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// A hinged door on its pivot. Holding E opens or closes it slowly and quietly; a tap swings it fast and it creaks
    /// (docs/DESIGN.md "Player systems", other noise sources). MatchDirector latches front doors on the last dusk toll
    /// and unlatches them at the midnight bell (docs/DESIGN.md "Dusk transition").
    /// The leaf carries a carving NavMeshObstacle, so a closed door also blocks the Hollow's paths.
    /// </summary>
    public class Door : Interactable
    {
        public House House;
        public float OpenAngle = 90f;
        [Tooltip("Seconds of holding E for the slow, quiet swing.")]
        public float QuietHoldSeconds = 0.6f;
        [Tooltip("Not in DESIGN.md yet: how long a slow and a quick swing take.")]
        public float SlowSwingSeconds = 1.4f;
        public float FastSwingSeconds = 0.3f;

        public bool IsOpen { get; private set; }
        public bool Latched { get; private set; }
        public bool IsMoving => !Mathf.Approximately(_angle, _target);

        Quaternion _closed;
        float _angle, _target, _speed = 1f;

        void Awake()
        {
            _closed = transform.localRotation;
        }

        void Update()
        {
            if (!IsMoving) return;
            _angle = Mathf.MoveTowards(_angle, _target, _speed * Time.deltaTime);
            transform.localRotation = _closed * Quaternion.Euler(0f, _angle, 0f);
        }

        public override string Prompt(PlayerAgent player)
        {
            if (Latched)
                return House != null && House.Contains(player.transform.position) ? "Latched until the midnight bell" : "Latched";
            return IsOpen ? "Close (hold: quietly)" : "Open (hold: quietly)";
        }

        public override bool CanInteract(PlayerAgent player) => !Latched;
        public override float HoldSeconds(PlayerAgent player) => QuietHoldSeconds;
        public override bool AllowsTap(PlayerAgent player) => true;

        public override void Interact(PlayerAgent player, InteractKind kind)
        {
            bool quiet = kind == InteractKind.Hold;
            Swing(!IsOpen, quiet ? SlowSwingSeconds : FastSwingSeconds, quiet ? NoiseRadius.DoorQuiet : NoiseRadius.DoorCreak, player.Index);
        }

        /// <summary>Opens or closes the door. Noise radius 0 is silent; sourcePlayer -1 is the world or the Hollow.</summary>
        public void Swing(bool open, float seconds, float noiseRadius, int sourcePlayer = -1)
        {
            if (open == IsOpen && !IsMoving) return;
            IsOpen = open;
            _target = open ? OpenAngle : 0f;
            _speed = OpenAngle / Mathf.Max(0.05f, seconds);
            NoiseSystem.Emit(transform.position, noiseRadius, NoiseKind.Door, sourcePlayer);
        }

        /// <summary>Dusk: swing shut and latch.</summary>
        public void CloseAndLatch()
        {
            Swing(false, FastSwingSeconds, NoiseRadius.DoorCreak);
            Latched = true;
        }

        public void Unlatch() => Latched = false;

        /// <summary>The Hollow letting itself in: always fast and loud, latched or not.</summary>
        public void ForceOpen()
        {
            Latched = false;
            Swing(true, FastSwingSeconds, NoiseRadius.DoorCreak);
        }
    }
}
