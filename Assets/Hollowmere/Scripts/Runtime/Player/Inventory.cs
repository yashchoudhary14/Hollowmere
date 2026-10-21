using System;

namespace Hollowmere
{
    public enum ItemKind { None, Matches, Battery, WindingKey, RitualPart }

    /// <summary>
    /// What one player carries: 3 hand slots, one ritual-part slot, matches, spare batteries and a keepsake.
    /// Pure data so it can be unit-tested and later replicated. Design: docs/DESIGN.md "Player systems" and "Items and tools".
    /// </summary>
    [Serializable]
    public class Inventory
    {
        public const int HandSlotCount = 3;

        public ItemKind[] Hands = new ItemKind[HandSlotCount];
        public int Selected;
        public int RitualPart = -1;     // house index of the carried part, or -1
        public int Matches;
        public int Batteries;
        public string Keepsake = "";    // can't be dropped

        public bool HasPart => RitualPart >= 0;

        /// <summary>Carrying a ritual part means no sprinting.</summary>
        public bool CanSprint => !HasPart;

        public ItemKind SelectedItem => Hands[Selected];

        public static bool IsCounted(ItemKind kind) => kind == ItemKind.Matches || kind == ItemKind.Battery;

        public bool Has(ItemKind kind)
        {
            switch (kind)
            {
                case ItemKind.Matches: return Matches > 0;
                case ItemKind.Battery: return Batteries > 0;
                case ItemKind.RitualPart: return HasPart;
                default: return Array.IndexOf(Hands, kind) >= 0;
            }
        }

        /// <summary>Whether TryAdd would succeed, without changing anything.</summary>
        public bool CanAdd(ItemKind kind)
        {
            if (kind == ItemKind.None) return false;
            if (IsCounted(kind)) return true;
            if (kind == ItemKind.RitualPart) return !HasPart;
            return Array.IndexOf(Hands, ItemKind.None) >= 0;
        }

        /// <summary>Adds an item. Hand items go to the selected slot if it's free, else the first free one.
        /// Returns the hand slot used, -1 for counted items and the part, or -2 if there's no room.</summary>
        public int TryAdd(ItemKind kind, int amount = 1, int partHouse = -1)
        {
            if (!CanAdd(kind)) return -2;
            switch (kind)
            {
                case ItemKind.Matches: Matches += amount; return -1;
                case ItemKind.Battery: Batteries += amount; return -1;
                case ItemKind.RitualPart: RitualPart = partHouse; return -1;
            }
            int slot = Hands[Selected] == ItemKind.None ? Selected : Array.IndexOf(Hands, ItemKind.None);
            Hands[slot] = kind;
            return slot;
        }

        public bool Remove(ItemKind kind)
        {
            switch (kind)
            {
                case ItemKind.Matches: if (Matches <= 0) return false; Matches--; return true;
                case ItemKind.Battery: if (Batteries <= 0) return false; Batteries--; return true;
                case ItemKind.RitualPart: if (!HasPart) return false; RitualPart = -1; return true;
            }
            int slot = Array.IndexOf(Hands, kind);
            if (slot < 0) return false;
            Hands[slot] = ItemKind.None;
            return true;
        }

        /// <summary>Takes the carried part out of its slot and returns its house index, or -1.</summary>
        public int TakePart()
        {
            int part = RitualPart;
            RitualPart = -1;
            return part;
        }

        public void Select(int slot)
        {
            if (slot >= 0 && slot < HandSlotCount) Selected = slot;
        }
    }
}
