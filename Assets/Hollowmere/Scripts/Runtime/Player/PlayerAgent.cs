using System.Collections.Generic;
using UnityEngine;

namespace Hollowmere
{
    public enum PlayerCondition { Alive, Grabbed, Dead, Escaped, Taken }

    /// <summary>
    /// One player: identity, condition, inventory and the objects they carry. Every interaction goes through
    /// Interact(), the player's intent; milestone 2 turns that call into a request the host carries out.
    /// Design: docs/DESIGN.md "Player systems".
    /// </summary>
    public class PlayerAgent : MonoBehaviour
    {
        public int Index;
        public string DisplayName = "Traveller";
        public Inventory Inventory = new Inventory();

        [Header("Parts")]
        public Transform Head;
        public PlayerMotor Motor;
        public PlayerInteractor Interactor;
        public Flashlight Flashlight;
        public MicNoise Mic;

        public PlayerCondition Condition { get; private set; } = PlayerCondition.Alive;
        public bool IsAlive => Condition == PlayerCondition.Alive || Condition == PlayerCondition.Grabbed;

        /// <summary>How the match ended for this player, for the results screen.</summary>
        public string Fate { get; private set; } = "";

        // Reading a note, a letter or the Ritual Book.
        public string PanelTitle { get; private set; }
        public string PanelText { get; private set; }
        public bool PanelOpen => PanelText != null;

        /// <summary>Esc frees the mouse; a click takes it back. The match keeps running.</summary>
        public bool Paused { get; private set; }

        /// <summary>Screens that need the mouse: the debug overlay and the results screen.</summary>
        public bool DebugCapture;
        public bool ResultsCapture;
        public bool UiCapture => DebugCapture || ResultsCapture;

        /// <summary>Set by the match (arrival cut, results) to stop movement.</summary>
        public bool Frozen;

        /// <summary>Set by something that takes over the E key, like the wardrobe you're hiding in.</summary>
        public Interactable ForcedTarget;

        /// <summary>True while hiding; movement stops but the view can peek.</summary>
        public bool InHiding;
        public HidingSpot Hiding;
        public bool HoldingBreath;
        public float Breath01;

        public bool CanMove => Condition == PlayerCondition.Alive && !PanelOpen && !Paused && !Frozen && !InHiding && !UiCapture;
        public bool CanLook => Condition == PlayerCondition.Alive && !PanelOpen && !Paused && !UiCapture;

        readonly GameObject[] _handObjects = new GameObject[Inventory.HandSlotCount];
        GameObject _partObject;
        readonly List<(string text, float time)> _messages = new List<(string, float)>();

        public IReadOnlyList<(string text, float time)> Messages => _messages;

        void Update()
        {
            if (GameInput.Pressed(GameKey.Pause))
            {
                if (PanelOpen) ClosePanel();
                else Paused = !Paused;
            }
            else if (Paused && GameInput.MouseClicked && !UiCapture)
            {
                Paused = false;
            }

            bool free = Paused || PanelOpen || UiCapture;
            Cursor.lockState = free ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = free;

            if (!CanMove) return;
            if (GameInput.Pressed(GameKey.Slot1)) Inventory.Select(0);
            if (GameInput.Pressed(GameKey.Slot2)) Inventory.Select(1);
            if (GameInput.Pressed(GameKey.Slot3)) Inventory.Select(2);
            if (GameInput.Pressed(GameKey.Drop)) DropSelected();
        }

        // ------------------------------------------------------------ intents

        /// <summary>The one way a player acts on the world.</summary>
        public void Interact(Interactable target, InteractKind kind)
        {
            if (target == null || !IsAlive || !target.CanInteract(this)) return;
            target.Interact(this, kind);
        }

        public bool PickUp(Pickup pickup)
        {
            int slot = Inventory.TryAdd(pickup.Kind, pickup.Amount, pickup.PartHouse);
            if (slot == -2)
            {
                Notify(pickup.Kind == ItemKind.RitualPart ? "You're already carrying a ritual part." : "Your hands are full. Press G to drop something.");
                return false;
            }
            if (slot >= 0) _handObjects[slot] = pickup.gameObject;
            else if (pickup.Kind == ItemKind.RitualPart) _partObject = pickup.gameObject;
            pickup.gameObject.SetActive(false);
            Notify(pickup.Kind == ItemKind.RitualPart ? $"You carry {pickup.Label}. You can't run with it." : $"Took {pickup.Label}.");
            return true;
        }

        /// <summary>Drops the selected hand item, or the ritual part if that hand is empty.</summary>
        public void DropSelected()
        {
            GameObject obj;
            int slot = Inventory.Selected;
            if (Inventory.Hands[slot] != ItemKind.None)
            {
                Inventory.Hands[slot] = ItemKind.None;
                obj = _handObjects[slot];
                _handObjects[slot] = null;
            }
            else if (Inventory.HasPart)
            {
                Inventory.TakePart();
                obj = _partObject;
                _partObject = null;
            }
            else return;

            PlaceInWorld(obj, DropPoint());
            NoiseSystem.Emit(transform.position, NoiseRadius.DroppedItem, NoiseKind.Object, Index);
        }

        /// <summary>Hands over the carried part (to the altar). Returns its house index, or -1.</summary>
        public int GiveUpPart(out GameObject partObject)
        {
            partObject = _partObject;
            _partObject = null;
            return Inventory.TakePart();
        }

        /// <summary>Uses up a carried item (a match, the winding key). Returns false if it isn't carried.</summary>
        public bool Consume(ItemKind kind)
        {
            int slot = System.Array.IndexOf(Inventory.Hands, kind);
            if (!Inventory.Remove(kind)) return false;
            if (slot >= 0 && _handObjects[slot] != null)
            {
                Destroy(_handObjects[slot]);
                _handObjects[slot] = null;
            }
            return true;
        }

        public static void PlaceInWorld(GameObject obj, Vector3 position)
        {
            if (obj == null) return;
            obj.transform.position = position;
            obj.SetActive(true);
        }

        Vector3 DropPoint()
        {
            var forward = Vector3.ProjectOnPlane(Head.forward, Vector3.up).normalized;
            float reach = 0.6f;
            if (Physics.Raycast(Head.position, forward, out var wall, reach, ~0, QueryTriggerInteraction.Ignore))
                reach = Mathf.Max(0f, wall.distance - 0.2f);
            var above = Head.position + forward * reach;
            return Physics.Raycast(above, Vector3.down, out var floor, 3f, ~0, QueryTriggerInteraction.Ignore)
                ? floor.point + Vector3.up * 0.03f
                : transform.position + Vector3.up * 0.05f;
        }

        // ------------------------------------------------------------ reading

        public void OpenPanel(string title, string text)
        {
            PanelTitle = title;
            PanelText = text ?? "";
        }

        public void ClosePanel()
        {
            PanelTitle = null;
            PanelText = null;
        }

        // ------------------------------------------------------------ condition

        public void SetCondition(PlayerCondition condition, string fate = null)
        {
            Condition = condition;
            if (fate != null) Fate = fate;
            if (condition != PlayerCondition.Alive)
            {
                ClosePanel();
                if (Flashlight != null && condition != PlayerCondition.Grabbed) Flashlight.SetOn(false);
            }
        }

        // ------------------------------------------------------------ messages

        public void Notify(string text)
        {
            _messages.Add((text, Time.time));
            if (_messages.Count > 4) _messages.RemoveAt(0);
        }
    }
}
