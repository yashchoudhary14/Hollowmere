using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The chest holding a house's ritual part. It stays locked until the house's puzzle is solved,
    /// then the lid swings up and the part can be taken (docs/DESIGN.md "Night phase", each player's job).
    /// </summary>
    public class PartContainer : Interactable
    {
        public Transform Lid;
        public Pickup Part;
        public float OpenSeconds = 0.8f;

        public bool IsOpen { get; private set; }

        float _open01;

        public override string Prompt(PlayerAgent player) => IsOpen ? null : "Locked tight.";
        public override bool CanInteract(PlayerAgent player) => false;
        public override void Interact(PlayerAgent player, InteractKind kind) { }

        public void ResetClosed()
        {
            IsOpen = false;
            _open01 = 0f;
            Lid.localRotation = Quaternion.identity;
            if (Part != null) Part.gameObject.SetActive(false);
        }

        public void Open()
        {
            IsOpen = true;
            if (Part != null) Part.gameObject.SetActive(true);
        }

        void Update()
        {
            if (!IsOpen || _open01 >= 1f) return;
            _open01 = Mathf.MoveTowards(_open01, 1f, Time.deltaTime / OpenSeconds);
            Lid.localRotation = Quaternion.Euler(-105f * _open01, 0f, 0f);
        }
    }
}
