using UnityEngine;

namespace Hollowmere
{
    public enum InteractKind { Tap, Hold }

    /// <summary>
    /// Anything the player can aim at and press E on. PlayerInteractor finds it by raycast
    /// (GetComponentInParent from the collider it hits) and sends the press through
    /// PlayerAgent.Interact, which is the one place a player's intent reaches the world.
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        /// <summary>Text after the key hint, e.g. "Open the door". Null hides the prompt.</summary>
        public abstract string Prompt(PlayerAgent player);

        /// <summary>False shows the prompt without a key hint (e.g. "Latched") and ignores presses.</summary>
        public virtual bool CanInteract(PlayerAgent player) => true;

        /// <summary>Seconds E must be held. 0 means a tap is enough.</summary>
        public virtual float HoldSeconds(PlayerAgent player) => 0f;

        /// <summary>Whether a short press still counts when HoldSeconds is above 0 (doors: tap opens fast and loud).</summary>
        public virtual bool AllowsTap(PlayerAgent player) => HoldSeconds(player) <= 0f;

        public abstract void Interact(PlayerAgent player, InteractKind kind);
    }
}
