namespace Hollowmere
{
    /// <summary>The clock face: E moves the hour hand on by one hour.</summary>
    public class ClockFace : Interactable
    {
        public ClockPuzzle Rig;

        public override string Prompt(PlayerAgent player)
        {
            if (Rig.Solved) return null;
            return Rig.Active ? $"Move the hour hand (it points at {Rig.Hour})" : Rig.DormantPrompt;
        }

        public override bool CanInteract(PlayerAgent player) => Rig.Active;

        public override void Interact(PlayerAgent player, InteractKind kind) => Rig.Advance();
    }
}
