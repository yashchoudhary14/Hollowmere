using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The altar on the square with six slots. Slot i takes only the part MatchPlan.AltarOrder[i] names; a part in
    /// the wrong slot is thrown back, every candle goes out and the Hollow is called (docs/DESIGN.md "The Ritual of
    /// Passage"). A part on the altar is safe: nobody can take it back (docs/DESIGN.md "Night phase").
    /// </summary>
    public class Altar : MonoBehaviour
    {
        public AltarSlot[] Slots;
        public RitualCircle Circle;
        public Transform EjectPoint;

        public MatchDirector Director { get; private set; }
        public int[] Placed { get; private set; } = new int[0];   // slot -> house index, or -1

        public bool AllPlaced => Director != null && AltarRules.AllPlaced(Director.Plan, Placed);

        public void Setup(MatchDirector director)
        {
            Director = director;
            Placed = new int[Slots.Length];
            for (int i = 0; i < Placed.Length; i++) Placed[i] = -1;
            for (int i = 0; i < Slots.Length; i++) Slots[i].Setup(this, i, i < director.Plan.AltarOrder.Length);
        }

        public void Place(AltarSlot slot, PlayerAgent player)
        {
            int house = player.Inventory.RitualPart;
            if (house < 0) return;
            string part = HouseCatalog.Houses[house].Part.ToLowerInvariant();

            switch (AltarRules.Place(Director.Plan, Placed, slot.Index, house))
            {
                case PlaceResult.Accepted:
                    player.GiveUpPart(out var accepted);
                    Placed[slot.Index] = house;
                    slot.Hold(accepted);
                    player.Notify($"The {part} settles into slot {AltarRules.Numeral(slot.Index)}. It's safe there now.");
                    if (AllPlaced) Director.NotifyAll("Every part is on the altar. Stand within the stones, all of you, to begin the chant.");
                    break;

                case PlaceResult.WrongSlot:
                    player.GiveUpPart(out var thrown);
                    PlayerAgent.PlaceInWorld(thrown, EjectPoint.position);
                    Circle.SnuffAll();
                    NoiseSystem.Emit(transform.position + Vector3.up, NoiseRadius.AltarWrongSlot, NoiseKind.Ritual, player.Index);
                    Director.CallTheHollow(transform.position);
                    player.Notify($"The altar throws the {part} back at your feet. Every candle gutters out. Something heard that.");
                    break;

                case PlaceResult.SlotTaken:
                    player.Notify("That slot is already filled.");
                    break;

                default:
                    player.Notify("That slot is cold. Nothing belongs there this year.");
                    break;
            }
        }
    }
}
