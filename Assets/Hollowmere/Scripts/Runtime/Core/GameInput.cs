using UnityEngine;
#if ENABLE_INPUT_SYSTEM && HM_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Hollowmere
{
    public enum GameKey
    {
        Interact, Flashlight, Sprint, Crouch, CloseEyes, HoldBreath, MicToggle,
        Pause, DebugOverlay, DebugSkip, DebugFreezeMonster, DebugTeleportHome, Restart
    }

    /// <summary>
    /// Reads keyboard and mouse through whichever input backend the project uses
    /// (new Input System package or the legacy Input Manager), so either setting works.
    /// </summary>
    public static class GameInput
    {
        public static Vector2 Move
        {
            get
            {
                float x = 0f, y = 0f;
#if ENABLE_INPUT_SYSTEM && HM_INPUT_SYSTEM
                var k = Keyboard.current;
                if (k != null)
                {
                    if (k.wKey.isPressed || k.upArrowKey.isPressed) y += 1f;
                    if (k.sKey.isPressed || k.downArrowKey.isPressed) y -= 1f;
                    if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
                    if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
                }
#else
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) y += 1f;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) y -= 1f;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += 1f;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x -= 1f;
#endif
                var v = new Vector2(x, y);
                return v.sqrMagnitude > 1f ? v.normalized : v;
            }
        }

        /// <summary>Mouse movement this frame, scaled to match the legacy "Mouse X/Y" axes.</summary>
        public static Vector2 Look
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && HM_INPUT_SYSTEM
                var m = Mouse.current;
                return m != null ? m.delta.ReadValue() * 0.1f : Vector2.zero;
#else
                return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
#endif
            }
        }

        public static bool MouseClicked
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && HM_INPUT_SYSTEM
                var m = Mouse.current;
                return m != null && m.leftButton.wasPressedThisFrame;
#else
                return Input.GetMouseButtonDown(0);
#endif
            }
        }

        public static bool Held(GameKey key) => Query(key, 0);
        public static bool Pressed(GameKey key) => Query(key, 1);

#if ENABLE_INPUT_SYSTEM && HM_INPUT_SYSTEM
        static bool Query(GameKey key, int mode)
        {
            var k = Keyboard.current;
            if (k == null) return false;
            switch (key)
            {
                case GameKey.Crouch:
                    return Check(k.leftCtrlKey, mode) || Check(k.cKey, mode);
                case GameKey.Sprint:
                    return Check(k.leftShiftKey, mode);
                default:
                    var mapped = ToKey(key);
                    return mapped != Key.None && Check(k[mapped], mode);
            }
        }

        static bool Check(UnityEngine.InputSystem.Controls.KeyControl c, int mode)
            => c != null && (mode == 0 ? c.isPressed : c.wasPressedThisFrame);

        static Key ToKey(GameKey key)
        {
            switch (key)
            {
                case GameKey.Interact:           return Key.E;
                case GameKey.Flashlight:         return Key.F;
                case GameKey.CloseEyes:          return Key.Q;
                case GameKey.HoldBreath:         return Key.Space;
                case GameKey.MicToggle:          return Key.M;
                case GameKey.Pause:              return Key.Escape;
                case GameKey.DebugOverlay:       return Key.F1;
                case GameKey.DebugSkip:          return Key.F2;
                case GameKey.DebugFreezeMonster: return Key.F3;
                case GameKey.DebugTeleportHome:  return Key.F4;
                case GameKey.Restart:            return Key.R;
                default:                         return Key.None;
            }
        }
#else
        static bool Query(GameKey key, int mode)
        {
            switch (key)
            {
                case GameKey.Crouch:
                    return Check(KeyCode.LeftControl, mode) || Check(KeyCode.C, mode);
                case GameKey.Sprint:
                    return Check(KeyCode.LeftShift, mode);
                default:
                    return Check(ToKeyCode(key), mode);
            }
        }

        static bool Check(KeyCode c, int mode) => mode == 0 ? Input.GetKey(c) : Input.GetKeyDown(c);

        static KeyCode ToKeyCode(GameKey key)
        {
            switch (key)
            {
                case GameKey.Interact:           return KeyCode.E;
                case GameKey.Flashlight:         return KeyCode.F;
                case GameKey.CloseEyes:          return KeyCode.Q;
                case GameKey.HoldBreath:         return KeyCode.Space;
                case GameKey.MicToggle:          return KeyCode.M;
                case GameKey.Pause:              return KeyCode.Escape;
                case GameKey.DebugOverlay:       return KeyCode.F1;
                case GameKey.DebugSkip:          return KeyCode.F2;
                case GameKey.DebugFreezeMonster: return KeyCode.F3;
                case GameKey.DebugTeleportHome:  return KeyCode.F4;
                case GameKey.Restart:            return KeyCode.R;
                default:                         return KeyCode.None;
            }
        }
#endif
    }
}
