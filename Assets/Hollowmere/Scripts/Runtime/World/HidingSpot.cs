using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// A wardrobe to hide in. Inside, the view is limited to a peek through the doors and Space holds your breath
    /// for up to 8 seconds; holding on too long ends in a loud gasp. The Hollow finds a hidden player only if it can
    /// hear their breathing within 3 m (docs/DESIGN.md "The monster", "Player systems").
    /// </summary>
    public class HidingSpot : Interactable
    {
        public Transform Inside;
        public float PeekYaw = 30f;
        public float PeekPitch = 25f;

        [Header("DESIGN.md \"Player systems\": hold your breath up to 8 s")]
        public float BreathHoldSeconds = 8f;
        [Tooltip("Not in DESIGN.md yet: seconds after a gasp before you can hold your breath again.")]
        public float BreathRecoverSeconds = 4f;
        [Tooltip("DESIGN.md \"The monster\": breathing is heard within 3 m.")]
        public float BreathAudibleRange = 3f;

        public PlayerAgent Occupant { get; private set; }

        float _held, _recover, _faceYaw;

        public override string Prompt(PlayerAgent player)
        {
            if (Occupant == player) return "Leave the wardrobe";
            return Occupant == null ? "Hide in the wardrobe" : null;
        }

        public override void Interact(PlayerAgent player, InteractKind kind)
        {
            if (Occupant == player) Leave();
            else if (Occupant == null) Enter(player);
        }

        void Enter(PlayerAgent player)
        {
            Occupant = player;
            player.InHiding = true;
            player.Hiding = this;
            player.ForcedTarget = this;
            _faceYaw = Inside.eulerAngles.y;
            player.Motor.SetColliderEnabled(false);
            player.Motor.Teleport(Inside.position, _faceYaw);
            player.Motor.LookLimiter = (yaw, pitch) => new Vector2(
                _faceYaw + Mathf.Clamp(Mathf.DeltaAngle(_faceYaw, yaw), -PeekYaw, PeekYaw),
                Mathf.Clamp(pitch, -PeekPitch, PeekPitch));
            _held = 0f;
        }

        /// <summary>Steps out in front of the wardrobe. Also used when the Hollow drags someone out.</summary>
        public void Leave()
        {
            var player = Occupant;
            if (player == null) return;
            Occupant = null;
            player.InHiding = false;
            player.Hiding = null;
            player.ForcedTarget = null;
            player.HoldingBreath = false;
            player.Breath01 = 0f;
            player.Motor.LookLimiter = null;
            player.Motor.Teleport(Inside.position + Inside.forward * 0.9f, _faceYaw);
            player.Motor.SetColliderEnabled(true);
        }

        void Update()
        {
            if (Occupant == null) return;
            var player = Occupant;
            if (!player.IsAlive)
            {
                Leave();
                return;
            }

            if (_recover > 0f) _recover -= Time.deltaTime;
            bool want = GameInput.Held(GameKey.HoldBreath) && !player.Paused && _recover <= 0f;
            if (want)
            {
                player.HoldingBreath = true;
                _held += Time.deltaTime;
                if (_held >= BreathHoldSeconds) Gasp(player);
            }
            else if (player.HoldingBreath)
            {
                player.HoldingBreath = false;
                _held = 0f;
            }
            player.Breath01 = _recover > 0f ? 0f : _held / BreathHoldSeconds;
        }

        void Gasp(PlayerAgent player)
        {
            player.HoldingBreath = false;
            _held = 0f;
            _recover = BreathRecoverSeconds;
            NoiseSystem.Emit(Inside.position + Vector3.up * 1.5f, NoiseRadius.Gasp, NoiseKind.Breath, player.Index);
            player.Notify("You gasp for air.");
        }

        /// <summary>Whether someone at listenerPos would hear the occupant breathing.</summary>
        public bool BreathingAudibleFrom(Vector3 listenerPos)
        {
            if (Occupant == null || Occupant.HoldingBreath) return false;
            return Vector3.Distance(listenerPos, Inside.position) <= BreathAudibleRange;
        }
    }
}
