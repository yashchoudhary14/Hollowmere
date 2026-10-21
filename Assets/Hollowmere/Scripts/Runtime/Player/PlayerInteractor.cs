using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Finds what the player is looking at and turns E presses into taps and holds.
    /// Prompts and the hold bar are drawn by the HUD from Target and HoldProgress01.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        public PlayerAgent Agent;
        public float Reach = 2.5f;
        [Tooltip("On things that also accept a hold (doors), a press shorter than this is a tap.")]
        public float TapSeconds = 0.25f;

        public Interactable Target { get; private set; }
        public float HoldProgress01 { get; private set; }

        Interactable _pressed;
        float _heldFor;

        void Update()
        {
            if (Agent.PanelOpen)
            {
                Target = null;
                CancelHold();
                if (GameInput.Pressed(GameKey.Interact)) Agent.ClosePanel();
                return;
            }

            var target = FindTarget();
            Target = target;
            if (target != _pressed) CancelHold();
            if (target == null) return;

            if (GameInput.Pressed(GameKey.Interact) && target.CanInteract(Agent))
            {
                if (target.HoldSeconds(Agent) <= 0f)
                {
                    Agent.Interact(target, InteractKind.Tap);
                    return;
                }
                _pressed = target;
                _heldFor = 0f;
            }
            if (_pressed == null) return;

            if (GameInput.Held(GameKey.Interact))
            {
                _heldFor += Time.deltaTime;
                float need = _pressed.HoldSeconds(Agent);
                HoldProgress01 = need > 0f ? Mathf.Clamp01(_heldFor / need) : 1f;
                if (_heldFor >= need)
                {
                    var held = _pressed;
                    CancelHold();
                    Agent.Interact(held, InteractKind.Hold);
                }
            }
            else
            {
                var tapped = _pressed;
                bool isTap = _heldFor <= TapSeconds;
                CancelHold();
                if (isTap && tapped.AllowsTap(Agent)) Agent.Interact(tapped, InteractKind.Tap);
            }
        }

        void CancelHold()
        {
            _pressed = null;
            _heldFor = 0f;
            HoldProgress01 = 0f;
        }

        Interactable FindTarget()
        {
            if (Agent.ForcedTarget != null) return Agent.ForcedTarget;
            if (!Agent.CanMove) return null;
            var ray = new Ray(Agent.Head.position, Agent.Head.forward);
            if (!Physics.Raycast(ray, out var hit, Reach, ~0, QueryTriggerInteraction.Collide)) return null;
            var found = hit.collider.GetComponentInParent<Interactable>();
            return found != null && found.isActiveAndEnabled && found.Prompt(Agent) != null ? found : null;
        }
    }
}
