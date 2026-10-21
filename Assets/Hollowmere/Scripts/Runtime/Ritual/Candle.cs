using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// One candle on the rim of the stone circle, one per ritual part. All light when the chant begins; the Hollow
    /// puts them out and players relight them with matches from inside the circle (docs/DESIGN.md "The Ritual of Passage").
    /// </summary>
    public class Candle : Interactable
    {
        public GameObject Flame;

        public bool Lit { get; private set; }
        public bool InUse { get; private set; }

        RitualCircle _circle;

        public void Setup(RitualCircle circle, bool inUse)
        {
            _circle = circle;
            InUse = inUse;
            SetLit(false);
        }

        public void SetLit(bool lit)
        {
            Lit = lit;
            if (Flame != null) Flame.SetActive(lit);
        }

        public void Snuff() => SetLit(false);

        public override string Prompt(PlayerAgent player)
        {
            if (!InUse || _circle == null) return null;
            if (Lit) return "The candle burns.";
            if (!_circle.Chant.Started) return "A cold candle.";
            if (!_circle.Contains(player.transform.position)) return "Step inside the stones to relight it";
            return player.Inventory.Matches > 0 ? $"Relight the candle ({player.Inventory.Matches} matches)" : "Relight the candle (no matches left)";
        }

        public override bool CanInteract(PlayerAgent player)
            => InUse && !Lit && _circle != null && _circle.Chant.Started && !_circle.Chant.Complete
               && _circle.Contains(player.transform.position) && player.Inventory.Matches > 0;

        public override void Interact(PlayerAgent player, InteractKind kind)
        {
            if (player.Consume(ItemKind.Matches)) SetLit(true);
        }
    }
}
