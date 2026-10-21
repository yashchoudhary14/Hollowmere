namespace Hollowmere
{
    /// <summary>The clock's keyhole: hold E with the winding key to wind the clock and test the hour.</summary>
    public class ClockKeyhole : Interactable
    {
        public ClockPuzzle Rig;
        public float WindSeconds = 1.5f;

        public override string Prompt(PlayerAgent player)
        {
            if (Rig.Solved) return null;
            if (!Rig.Active) return Rig.DormantPrompt;
            return player.Inventory.Has(ItemKind.WindingKey) ? "Wind the clock (hold)" : "A keyhole. It needs a winding key.";
        }

        public override bool CanInteract(PlayerAgent player) => Rig.Active && player.Inventory.Has(ItemKind.WindingKey);

        public override float HoldSeconds(PlayerAgent player) => WindSeconds;

        public override void Interact(PlayerAgent player, InteractKind kind) => Rig.Wind(player);
    }
}
