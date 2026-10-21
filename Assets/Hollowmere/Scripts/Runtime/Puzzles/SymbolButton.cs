namespace Hollowmere
{
    /// <summary>One symbol button on a SequencePuzzle.</summary>
    public class SymbolButton : Interactable
    {
        public SequencePuzzle Rig;
        public int Symbol;

        public override string Prompt(PlayerAgent player)
        {
            if (Rig.Solved) return null;
            return Rig.Active ? $"Press the {HouseCatalog.SequenceSymbols[Symbol]}" : Rig.DormantPrompt;
        }

        public override bool CanInteract(PlayerAgent player) => Rig.Active;

        public override void Interact(PlayerAgent player, InteractKind kind) => Rig.Press(Symbol, player);
    }
}
