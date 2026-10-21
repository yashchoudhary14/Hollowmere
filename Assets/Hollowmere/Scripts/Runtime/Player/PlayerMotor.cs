using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// First-person movement: walk, crouch, sprint with stamina, jump, mouse look, and footstep noise.
    /// Speeds and noise radii: docs/DESIGN.md "Player systems".
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        public PlayerAgent Agent;
        public Transform Head;

        [Header("Speeds, DESIGN.md \"Player systems\"")]
        public float WalkSpeed = 2.5f;
        public float CrouchSpeed = 1.5f;
        public float SprintSpeed = 5f;
        public float SprintSeconds = 6f;
        [Tooltip("Not in DESIGN.md yet: seconds for stamina to refill from empty.")]
        public float RecoverySeconds = 8f;
        [Tooltip("Not in DESIGN.md yet: after running dry, sprinting comes back at this much stamina.")]
        public float SprintResumeAt = 0.35f;

        [Header("Body")]
        public float StandHeight = 1.8f;
        public float CrouchHeight = 1.15f;
        public float StandEye = 1.65f;
        public float CrouchEye = 1.0f;
        public float JumpHeight = 0.8f;
        public float Gravity = -18f;
        public float MouseSensitivity = 2f;

        [Header("Footsteps: one noise event per stride")]
        public float WalkStride = 0.8f;
        public float SprintStride = 1.3f;
        public float CrouchStride = 0.6f;

        public float Stamina01 { get; private set; } = 1f;
        public bool IsCrouching { get; private set; }
        public bool IsSprinting { get; private set; }
        public float LastNoiseRadius { get; private set; }
        public float LastNoiseTime { get; private set; } = -99f;
        public float Yaw => _yaw;
        public float Pitch => _pitch;

        /// <summary>Set during the Crossing: walking only (DESIGN.md "The Crossing").</summary>
        public bool WalkOnly;

        /// <summary>Optional limits on the view, e.g. peeking out of a wardrobe. Null means free.</summary>
        public System.Func<float, float, Vector2> LookLimiter;

        CharacterController _cc;
        float _yaw, _pitch, _verticalVelocity, _stride, _eye, _airTime;
        bool _exhausted;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _yaw = transform.eulerAngles.y;
            _eye = StandEye;
        }

        void Update()
        {
            if (Agent.CanLook) Look();
            Move(Agent.CanMove);
        }

        void Look()
        {
            var d = GameInput.Look * MouseSensitivity;
            _yaw += d.x;
            _pitch = Mathf.Clamp(_pitch - d.y, -85f, 85f);
            if (LookLimiter != null)
            {
                var limited = LookLimiter(_yaw, _pitch);
                _yaw = limited.x;
                _pitch = limited.y;
            }
            ApplyRotation();
        }

        void ApplyRotation()
        {
            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            Head.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        void Move(bool canMove)
        {
            if (!_cc.enabled) return;
            var input = canMove ? GameInput.Move : Vector2.zero;

            if (canMove && GameInput.Held(GameKey.Crouch)) IsCrouching = true;
            else if (IsCrouching && CanStandUp()) IsCrouching = false;

            bool wantSprint = canMove && GameInput.Held(GameKey.Sprint) && input.y > 0.1f
                              && !IsCrouching && !WalkOnly && Agent.Inventory.CanSprint;
            if (_exhausted && Stamina01 >= SprintResumeAt) _exhausted = false;
            IsSprinting = wantSprint && !_exhausted && Stamina01 > 0f;
            if (IsSprinting)
            {
                Stamina01 -= Time.deltaTime / SprintSeconds;
                if (Stamina01 <= 0f) { Stamina01 = 0f; _exhausted = true; }
            }
            else
            {
                Stamina01 = Mathf.Min(1f, Stamina01 + Time.deltaTime / RecoverySeconds);
            }

            float speed = IsCrouching ? CrouchSpeed : IsSprinting ? SprintSpeed : WalkSpeed;
            var horizontal = (transform.right * input.x + transform.forward * input.y) * speed;

            bool wasGrounded = _cc.isGrounded;
            if (wasGrounded)
            {
                _airTime = 0f;
                if (_verticalVelocity < 0f) _verticalVelocity = -2f;
                if (canMove && !WalkOnly && !IsCrouching && GameInput.Pressed(GameKey.Jump))
                    _verticalVelocity = Mathf.Sqrt(2f * -Gravity * JumpHeight);
            }
            else
            {
                _airTime += Time.deltaTime;
            }
            _verticalVelocity += Gravity * Time.deltaTime;

            var before = transform.position;
            _cc.Move((horizontal + Vector3.up * _verticalVelocity) * Time.deltaTime);

            // Landing after a jump or a drop: "vault or jump", 10 m.
            if (!wasGrounded && _cc.isGrounded && _airTime > 0.35f) EmitStep(NoiseRadius.Jump);

            if (_cc.isGrounded)
            {
                var step = transform.position - before;
                step.y = 0f;
                _stride += step.magnitude;
                float strideLength = IsCrouching ? CrouchStride : IsSprinting ? SprintStride : WalkStride;
                if (_stride >= strideLength)
                {
                    _stride = 0f;
                    EmitStep(IsCrouching ? NoiseRadius.CrouchWalk : IsSprinting ? NoiseRadius.Sprint : NoiseRadius.Walk);
                }
            }

            float height = Mathf.MoveTowards(_cc.height, IsCrouching ? CrouchHeight : StandHeight, 4f * Time.deltaTime);
            _cc.height = height;
            _cc.center = new Vector3(0f, height * 0.5f, 0f);
            _eye = Mathf.MoveTowards(_eye, IsCrouching ? CrouchEye : StandEye, 3f * Time.deltaTime);
            Head.localPosition = new Vector3(0f, _eye, 0f);
        }

        void EmitStep(float radius)
        {
            LastNoiseRadius = radius;
            LastNoiseTime = Time.time;
            NoiseSystem.Emit(transform.position, radius, NoiseKind.Footstep, Agent.Index);
        }

        bool CanStandUp()
        {
            float r = _cc.radius * 0.9f;
            var start = transform.position + Vector3.up * (_cc.radius + 0.05f);
            return !Physics.SphereCast(start, r, Vector3.up, out _, StandHeight - 2f * _cc.radius, ~0, QueryTriggerInteraction.Ignore);
        }

        /// <summary>Moves the player without the CharacterController fighting it (spawns, hiding, debug teleport).</summary>
        public void Teleport(Vector3 position, float yaw)
        {
            bool was = _cc.enabled;
            _cc.enabled = false;
            transform.position = position;
            _yaw = yaw;
            _pitch = 0f;
            ApplyRotation();
            _verticalVelocity = 0f;
            _cc.enabled = was;
        }

        public void SetColliderEnabled(bool on) => _cc.enabled = on;

        public void SetLook(float yaw, float pitch)
        {
            _yaw = yaw;
            _pitch = pitch;
            ApplyRotation();
        }
    }
}
