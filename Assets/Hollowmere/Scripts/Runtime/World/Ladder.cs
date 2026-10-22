using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// One end of a loft ladder or hatch (M1.6: attics and haylofts are only reached this way). Holding E climbs to
    /// the other end; the climb is about as loud as walking. The Hollow uses the NavMeshLink built beside it.
    /// </summary>
    public class Ladder : Interactable
    {
        public Transform Exit;                 // where the climb ends: the floor at the other end, facing on
        public bool Up = true;                 // climbing up (else down)
        [Tooltip("Not in DESIGN.md yet: seconds of holding E to climb.")]
        public float ClimbSeconds = 0.6f;

        public override string Prompt(PlayerAgent player) => Up ? "Climb up" : "Climb down";

        public override bool CanInteract(PlayerAgent player) => Exit != null && !player.InHiding;

        public override float HoldSeconds(PlayerAgent player) => ClimbSeconds;

        public override void Interact(PlayerAgent player, InteractKind kind)
        {
            if (Exit == null) return;
            NoiseSystem.Emit(transform.position, NoiseRadius.Ladder, NoiseKind.Footstep, player.Index);
            player.Motor.Teleport(Exit.position, Exit.eulerAngles.y);
        }
    }
}
