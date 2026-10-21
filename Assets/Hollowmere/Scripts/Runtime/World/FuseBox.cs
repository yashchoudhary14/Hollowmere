using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// A house's power. Tap to switch it off (a way to hide in the dark); a tripped or switched-off box is reset by
    /// holding E (docs/DESIGN.md "Decisions": hold interact, 2 s, a little noise). MatchDirector trips every box at
    /// the minute-17 power cut (docs/DESIGN.md "Night phase", Act 3).
    /// </summary>
    public class FuseBox : Interactable
    {
        public House House;
        public Transform Lever;
        public float ResetSeconds = 2f;

        public bool Powered { get; private set; } = true;

        public override string Prompt(PlayerAgent player) => Powered ? "Switch the power off" : "Reset the fuse box (hold)";
        public override float HoldSeconds(PlayerAgent player) => Powered ? 0f : ResetSeconds;

        public override void Interact(PlayerAgent player, InteractKind kind)
        {
            if (Powered)
            {
                SetPowered(false);
            }
            else
            {
                SetPowered(true);
                NoiseSystem.Emit(transform.position, NoiseRadius.FuseReset, NoiseKind.Object, player.Index);
            }
        }

        /// <summary>The power cut, or the Hollow's ability later on.</summary>
        public void Trip() => SetPowered(false);

        public void SetPowered(bool on)
        {
            Powered = on;
            if (Lever != null) Lever.localRotation = Quaternion.Euler(on ? 0f : 180f, 0f, 0f);
            if (House != null) House.RefreshLights();
        }
    }
}
