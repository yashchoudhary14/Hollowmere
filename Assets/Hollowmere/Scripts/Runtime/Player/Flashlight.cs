using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The starter-kit flashlight: F toggles, B swaps in a spare battery, the beam flickers when low.
    /// A lit flashlight makes its holder visible to the Hollow. Design: docs/DESIGN.md "Items and tools".
    /// </summary>
    public class Flashlight : MonoBehaviour
    {
        public PlayerAgent Agent;
        public Light Beam;

        [Header("DESIGN.md \"Items and tools\": 15 m cone, 8 minutes of battery")]
        public float Range = 15f;
        public float BatterySeconds = 480f;
        [Tooltip("Not in DESIGN.md yet: below this charge the beam flickers.")]
        public float FlickerBelow = 0.15f;
        public float Intensity = 6f;

        public float Charge01 = 1f;
        public bool IsOn { get; private set; }

        void Start()
        {
            Beam.range = Range;
            SetOn(false);
        }

        void Update()
        {
            if (Agent.IsAlive && !Agent.Paused && !Agent.PanelOpen)
            {
                if (GameInput.Pressed(GameKey.Flashlight)) Toggle();
                if (GameInput.Pressed(GameKey.SwapBattery)) SwapBattery();
            }

            if (!IsOn) return;
            Charge01 -= Time.deltaTime / BatterySeconds;
            if (Charge01 <= 0f)
            {
                Charge01 = 0f;
                SetOn(false);
                Agent.Notify("The flashlight dies. Press B to change the battery.");
                return;
            }
            Beam.intensity = Charge01 < FlickerBelow ? FlickerIntensity() : Intensity;
        }

        float FlickerIntensity()
        {
            float level = Mathf.Lerp(0.4f, 1f, Charge01 / FlickerBelow);
            return Mathf.PerlinNoise(Time.time * 11f, 0.37f) < 0.3f ? Intensity * 0.05f : Intensity * level;
        }

        public void Toggle()
        {
            if (!IsOn && Charge01 <= 0f)
            {
                Agent.Notify("The flashlight is dead. Press B to change the battery.");
                return;
            }
            SetOn(!IsOn);
        }

        public void SetOn(bool on)
        {
            IsOn = on;
            if (Beam != null) Beam.enabled = on;
        }

        void SwapBattery()
        {
            if (Charge01 >= 0.99f) return;
            if (!Agent.Inventory.Remove(ItemKind.Battery))
            {
                Agent.Notify("No spare batteries.");
                return;
            }
            Charge01 = 1f;
            Agent.Notify("Fresh battery.");
        }
    }
}
