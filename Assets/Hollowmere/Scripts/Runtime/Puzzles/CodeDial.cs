using UnityEngine;

namespace Hollowmere
{
    /// <summary>One dial of a CodeLockPuzzle: E turns it to the next digit.</summary>
    public class CodeDial : Interactable
    {
        public CodeLockPuzzle Rig;
        public int Index;
        public TextMesh Digit;

        public override string Prompt(PlayerAgent player)
        {
            if (Rig.Solved) return null;
            return Rig.Active ? $"Turn the dial (it shows {Rig.Dials[Index]})" : Rig.DormantPrompt;
        }

        public override bool CanInteract(PlayerAgent player) => Rig.Active;

        public override void Interact(PlayerAgent player, InteractKind kind) => Rig.Turn(Index, player);
    }
}
