using NUnit.Framework;

namespace Hollowmere.Tests
{
    /// <summary>docs/DESIGN.md "Player systems": 3 hand slots, 1 ritual-part slot, no sprinting while carrying a part.</summary>
    public class InventoryTests
    {
        [Test]
        public void HandItems_FillTheSelectedSlotFirst_ThenTheFirstFreeOne()
        {
            var inv = new Inventory();
            inv.Select(1);
            Assert.That(inv.TryAdd(ItemKind.WindingKey), Is.EqualTo(1));
            Assert.That(inv.TryAdd(ItemKind.WindingKey), Is.EqualTo(0));
            Assert.That(inv.TryAdd(ItemKind.WindingKey), Is.EqualTo(2));
            Assert.That(inv.TryAdd(ItemKind.WindingKey), Is.EqualTo(-2), "Three hand slots only.");
        }

        [Test]
        public void MatchesAndBatteries_AreCounted_NotHeld()
        {
            var inv = new Inventory();
            Assert.That(inv.TryAdd(ItemKind.Matches, 5), Is.EqualTo(-1));
            Assert.That(inv.TryAdd(ItemKind.Battery), Is.EqualTo(-1));
            Assert.That(inv.Matches, Is.EqualTo(5));
            Assert.That(inv.Batteries, Is.EqualTo(1));
            Assert.That(inv.Hands, Is.All.EqualTo(ItemKind.None));
            Assert.That(inv.Remove(ItemKind.Matches), Is.True);
            Assert.That(inv.Matches, Is.EqualTo(4));
        }

        [Test]
        public void CarryingAPart_StopsSprinting_AndOnlyOneFits()
        {
            var inv = new Inventory();
            Assert.That(inv.CanSprint, Is.True);
            Assert.That(inv.TryAdd(ItemKind.RitualPart, partHouse: 7), Is.EqualTo(-1));
            Assert.That(inv.CanSprint, Is.False);
            Assert.That(inv.TryAdd(ItemKind.RitualPart, partHouse: 3), Is.EqualTo(-2));
            Assert.That(inv.TakePart(), Is.EqualTo(7));
            Assert.That(inv.CanSprint, Is.True);
        }
    }
}
