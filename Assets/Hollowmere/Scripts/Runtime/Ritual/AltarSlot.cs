using UnityEngine;

namespace Hollowmere
{
    /// <summary>One numbered slot on the altar. Slots beyond the match's part count stay cold.</summary>
    public class AltarSlot : Interactable
    {
        public Transform Display;

        public int Index { get; private set; }
        public bool InUse { get; private set; }
        public bool Filled => _altar != null && _altar.Placed[Index] >= 0;

        Altar _altar;

        public void Setup(Altar altar, int index, bool inUse)
        {
            _altar = altar;
            Index = index;
            InUse = inUse;
        }

        public override string Prompt(PlayerAgent player)
        {
            if (_altar == null) return null;
            string n = AltarRules.Numeral(Index);
            if (!InUse) return $"Slot {n}. Cold.";
            if (Filled) return $"Slot {n}: the {PartName(_altar.Placed[Index])}";
            return player.Inventory.HasPart ? $"Place the {PartName(player.Inventory.RitualPart)} in slot {n}" : $"Slot {n}. Empty.";
        }

        public override bool CanInteract(PlayerAgent player) => _altar != null && InUse && !Filled && player.Inventory.HasPart;

        public override void Interact(PlayerAgent player, InteractKind kind) => _altar.Place(this, player);

        /// <summary>Shows the placed part in the slot. It can't be picked up again.</summary>
        public void Hold(GameObject part)
        {
            if (part == null) return;
            var pickup = part.GetComponent<Pickup>();
            if (pickup != null) pickup.enabled = false;
            foreach (var c in part.GetComponentsInChildren<Collider>()) c.enabled = false;
            PlayerAgent.PlaceInWorld(part, Display.position);
        }

        static string PartName(int house) => HouseCatalog.Houses[house].Part.ToLowerInvariant();
    }
}
