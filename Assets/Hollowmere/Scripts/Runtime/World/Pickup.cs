using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// An item lying in the world. Taking it hides the object; dropping a hand item or the ritual part
    /// puts the same object back down, so whatever it looks like travels with it.
    /// </summary>
    public class Pickup : Interactable
    {
        public ItemKind Kind;
        public int Amount = 1;
        public int PartHouse = -1;       // for ItemKind.RitualPart: which house's part this is
        public string Label = "it";      // "a box of matches", "the iron gate key"

        public override string Prompt(PlayerAgent player) => $"Take {Label}";

        public override void Interact(PlayerAgent player, InteractKind kind) => player.PickUp(this);
    }
}
