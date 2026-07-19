using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ArcaneEngine
{
    public enum ArcaneInputContext { FrontEnd, HomeBase, Combat, Workshop, Inventory, Map, Dialogue, Pause }

    /// <summary>
    /// Small compatibility layer over Unity's supported Input System package.
    /// Profiles can keep their existing serialized KeyCode bindings while all
    /// runtime reads use Keyboard/Mouse controls instead of the legacy Input Manager.
    /// </summary>
    public static class ArcaneInput
    {
        private static readonly HashSet<KeyCode> EventKeysHeld = new HashSet<KeyCode>();
        private static readonly Dictionary<KeyCode, int> EventKeyDownFrames = new Dictionary<KeyCode, int>();
        private static readonly Dictionary<KeyCode, int> EventKeyUpFrames = new Dictionary<KeyCode, int>();
        private static readonly Dictionary<KeyCode, int> DeliveredKeyDownFrames = new Dictionary<KeyCode, int>();
        private static readonly Dictionary<KeyCode, int> DeliveredKeyUpFrames = new Dictionary<KeyCode, int>();
        private static readonly Dictionary<KeyCode, int> DeliveredKeyDownEvents = new Dictionary<KeyCode, int>();
        private static readonly Dictionary<KeyCode, int> DeliveredKeyUpEvents = new Dictionary<KeyCode, int>();
        private static readonly bool[] EventMouseHeld = new bool[5];
        private static readonly int[] EventMouseDownFrames = { -100, -100, -100, -100, -100 };
        private static readonly int[] EventMouseUpFrames = { -100, -100, -100, -100, -100 };
        private static readonly int[] DeliveredMouseDownFrames = { -100, -100, -100, -100, -100 };
        private static readonly int[] DeliveredMouseUpFrames = { -100, -100, -100, -100, -100 };
        private static readonly int[] DeliveredMouseDownEvents = { -100, -100, -100, -100, -100 };
        private static readonly int[] DeliveredMouseUpEvents = { -100, -100, -100, -100, -100 };
        private static Vector3 _eventMousePosition;
        private static bool _eventMouseValid;
        private static int _ignorePointerUntilFrame = -100;
        private static float _eventScroll;
        private static int _eventScrollFrame = -100;
        private static float _eventDeltaX;
        private static int _eventDeltaFrame = -100;

        public static Vector3 MousePosition
        {
            get
            {
                // Never alternate coordinate authorities while aiming. In the Editor the
                // root IMGUI pump reports Game-view-local coordinates, while Input System
                // coordinates can be relative to the host window. Switching between them
                // caused the pointer ray to clamp to a side of the camera pixel rect.
                Vector2 value;
#if UNITY_EDITOR
                if (_eventMouseValid) value = _eventMousePosition;
                else if (Mouse.current != null) value = Mouse.current.position.ReadValue();
                else value = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
#else
                if (Mouse.current != null) value = Mouse.current.position.ReadValue();
                else if (_eventMouseValid) value = _eventMousePosition;
                else value = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
#endif
                value.x = Mathf.Clamp(value.x, 0f, Mathf.Max(1f, Screen.width - 1f));
                value.y = Mathf.Clamp(value.y, 0f, Mathf.Max(1f, Screen.height - 1f));
                return new Vector3(value.x, value.y, 0f);
            }
        }

        public static Vector2 GuiMousePosition { get { Vector3 value = MousePosition; return new Vector2(value.x, Screen.height - value.y); } }
        public static bool PointerTransitionsAllowed { get { return Time.frameCount > _ignorePointerUntilFrame; } }
        public static void SuppressPointerTransitions(int frames = 2)
        {
            _ignorePointerUntilFrame = Mathf.Max(_ignorePointerUntilFrame, Time.frameCount + Mathf.Max(1, frames));
        }

        public static ArcaneInputContext Context
        {
            get
            {
                if (V1TitleScreen.IsOpen || RunStartScreen.IsOpen) return ArcaneInputContext.FrontEnd;
                if (V1TitleScreen.IsMapOpen) return ArcaneInputContext.Map;
                if (DemoUI.Instance != null)
                {
                    if (DemoUI.Instance.WorkshopOpen) return ArcaneInputContext.Workshop;
                    if (DemoUI.Instance.InventoryOpen) return ArcaneInputContext.Inventory;
                    if (DemoUI.Instance.IsPaused) return ArcaneInputContext.Pause;
                }
                GameWorld world = GameWorld.Instance;
                return world != null && world.RunActive ? ArcaneInputContext.Combat : ArcaneInputContext.HomeBase;
            }
        }

        public static float MouseScrollY
        {
            get
            {
                if (Mouse.current == null) return _eventScrollFrame >= Time.frameCount - 1 ? _eventScroll : 0f;
                float value = Mouse.current.scroll.ReadValue().y;
                return Mathf.Abs(value) > 10f ? value / 120f : value;
            }
        }

        public static float MouseDeltaX
        {
            get
            {
                return Mouse.current == null
                    ? (_eventDeltaFrame >= Time.frameCount - 1 ? _eventDeltaX * 0.05f : 0f)
                    : Mouse.current.delta.ReadValue().x * 0.05f;
            }
        }

        public static bool GamepadActive
        {
            get
            {
                Gamepad pad = Gamepad.current;
                if (pad == null) return false;
                return pad.leftStick.ReadValue().sqrMagnitude > 0.01f ||
                       pad.rightStick.ReadValue().sqrMagnitude > 0.01f ||
                       pad.buttonSouth.isPressed || pad.leftTrigger.isPressed || pad.rightTrigger.isPressed;
            }
        }

        public static Vector2 GamepadMove
        {
            get
            {
                Gamepad pad = Gamepad.current;
                if (pad == null) return Vector2.zero;
                Vector2 value = pad.leftStick.ReadValue();
                float deadZone = ProfileManager.Current == null ? 0.18f : ProfileManager.Current.controls.controllerMoveDeadZone;
                return value.magnitude <= deadZone ? Vector2.zero : value.normalized * Mathf.InverseLerp(deadZone, 1f, value.magnitude);
            }
        }

        public static Vector2 GamepadAim
        {
            get
            {
                Gamepad pad = Gamepad.current;
                if (pad == null) return Vector2.zero;
                Vector2 value = pad.rightStick.ReadValue();
                float deadZone = ProfileManager.Current == null ? 0.22f : ProfileManager.Current.controls.controllerAimDeadZone;
                return value.magnitude <= deadZone ? Vector2.zero : value.normalized * Mathf.InverseLerp(deadZone, 1f, value.magnitude);
            }
        }

        public static Vector2 GamepadNavigate
        {
            get
            {
                Gamepad pad = Gamepad.current;
                if (pad == null) return Vector2.zero;
                Vector2 dpad = pad.dpad.ReadValue();
                if (dpad.sqrMagnitude > 0.01f) return dpad;
                Vector2 stick = pad.leftStick.ReadValue();
                return stick.sqrMagnitude < 0.36f ? Vector2.zero : stick;
            }
        }

        public static bool GamepadDodgeDown { get { return Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame; } }
        public static bool GamepadCancelDown { get { return Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame; } }
        public static bool GamepadSubmitDown { get { return Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame; } }
        public static bool GamepadMenuDown { get { return Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame; } }
        public static bool GamepadSpellHeld(int slot)
        {
            Gamepad pad = Gamepad.current;
            return pad != null && (slot == 0 ? pad.rightTrigger.isPressed : slot == 1 ? pad.leftTrigger.isPressed : pad.rightShoulder.isPressed);
        }
        public static bool GamepadSpellReleased(int slot)
        {
            Gamepad pad = Gamepad.current;
            return pad != null && (slot == 0 ? pad.rightTrigger.wasReleasedThisFrame : slot == 1 ? pad.leftTrigger.wasReleasedThisFrame : pad.rightShoulder.wasReleasedThisFrame);
        }

        public static void PulseGamepad(float low, float high, float seconds)
        {
            if (Gamepad.current == null || ProfileManager.Current == null || !ProfileManager.Current.controls.controllerVibrationEnabled) return;
            float scale = Mathf.Clamp01(ProfileManager.Current.controls.controllerVibration);
            Gamepad.current.SetMotorSpeeds(Mathf.Clamp01(low) * scale, Mathf.Clamp01(high) * scale);
            ArcaneInputPump.StopVibrationAfter(seconds);
        }

        public static bool GetKey(KeyCode keyCode)
        {
            KeyControl control = KeyControlFor(keyCode);
            return control != null ? control.isPressed : EventKeysHeld.Contains(keyCode);
        }

        public static bool GetKeyDown(KeyCode keyCode)
        {
            KeyControl control = KeyControlFor(keyCode);
            return ConsumeKeyTransition(keyCode, control != null && control.wasPressedThisFrame,
                EventKeyDownFrames, DeliveredKeyDownFrames, DeliveredKeyDownEvents);
        }

        public static bool GetKeyUp(KeyCode keyCode)
        {
            KeyControl control = KeyControlFor(keyCode);
            return ConsumeKeyTransition(keyCode, control != null && control.wasReleasedThisFrame,
                EventKeyUpFrames, DeliveredKeyUpFrames, DeliveredKeyUpEvents);
        }

        public static bool GetMouseButton(int button)
        {
            ButtonControl control = MouseButton(button);
            return control != null ? control.isPressed : button >= 0 && button < EventMouseHeld.Length && EventMouseHeld[button];
        }

        public static bool GetMouseButtonDown(int button)
        {
            if (!PointerTransitionsAllowed) return false;
            if (button < 0 || button >= EventMouseDownFrames.Length) return false;
            ButtonControl control = MouseButton(button);
            return ConsumeMouseTransition(button, control != null && control.wasPressedThisFrame,
                EventMouseDownFrames, DeliveredMouseDownFrames, DeliveredMouseDownEvents);
        }

        public static bool GetMouseButtonUp(int button)
        {
            if (!PointerTransitionsAllowed) return false;
            if (button < 0 || button >= EventMouseUpFrames.Length) return false;
            ButtonControl control = MouseButton(button);
            return ConsumeMouseTransition(button, control != null && control.wasReleasedThisFrame,
                EventMouseUpFrames, DeliveredMouseUpFrames, DeliveredMouseUpEvents);
        }

        public static void FeedEvent(Event current)
        {
            if (current == null) return;
            Vector2 guiPointer = current.mousePosition;
            if (!float.IsNaN(guiPointer.x) && !float.IsNaN(guiPointer.y) && !float.IsInfinity(guiPointer.x) && !float.IsInfinity(guiPointer.y) &&
                guiPointer.x >= 0f && guiPointer.y >= 0f &&
                guiPointer.x <= Screen.width && guiPointer.y <= Screen.height)
            {
                _eventMousePosition = new Vector3(guiPointer.x, Screen.height - guiPointer.y, 0f);
                _eventMouseValid = true;
            }
            if (current.type == EventType.KeyDown && current.keyCode != KeyCode.None)
            {
                if (EventKeysHeld.Add(current.keyCode))
                {
                    EventKeyDownFrames[current.keyCode] = Time.frameCount;
                    MarkAlreadyDelivered(current.keyCode, DeliveredKeyDownFrames, DeliveredKeyDownEvents);
                }
            }
            else if (current.type == EventType.KeyUp && current.keyCode != KeyCode.None)
            {
                EventKeysHeld.Remove(current.keyCode);
                EventKeyUpFrames[current.keyCode] = Time.frameCount;
                MarkAlreadyDelivered(current.keyCode, DeliveredKeyUpFrames, DeliveredKeyUpEvents);
            }
            else if (current.type == EventType.MouseDown && current.button >= 0 && current.button < EventMouseHeld.Length)
            {
                EventMouseHeld[current.button] = true;
                EventMouseDownFrames[current.button] = Time.frameCount;
                if (DeliveredMouseDownFrames[current.button] == Time.frameCount) DeliveredMouseDownEvents[current.button] = Time.frameCount;
            }
            else if (current.type == EventType.MouseUp && current.button >= 0 && current.button < EventMouseHeld.Length)
            {
                EventMouseHeld[current.button] = false;
                EventMouseUpFrames[current.button] = Time.frameCount;
                if (DeliveredMouseUpFrames[current.button] == Time.frameCount) DeliveredMouseUpEvents[current.button] = Time.frameCount;
            }
            else if (current.type == EventType.MouseDrag || current.type == EventType.MouseMove)
            {
                _eventDeltaX = current.delta.x;
                _eventDeltaFrame = Time.frameCount;
            }
            else if (current.type == EventType.ScrollWheel)
            {
                _eventScroll = -current.delta.y;
                _eventScrollFrame = Time.frameCount;
            }
        }

        public static void ResetEventState()
        {
            EventKeysHeld.Clear();
            for (int i = 0; i < EventMouseHeld.Length; i++) EventMouseHeld[i] = false;
            _eventDeltaX = 0f;
            _eventScroll = 0f;
            _ignorePointerUntilFrame = Time.frameCount + 2;
        }

        private static bool ConsumeKeyTransition(KeyCode keyCode, bool inputSystemTransition,
            Dictionary<KeyCode, int> eventFrames, Dictionary<KeyCode, int> deliveredFrames,
            Dictionary<KeyCode, int> deliveredEvents)
        {
            int deliveredFrame;
            if (deliveredFrames.TryGetValue(keyCode, out deliveredFrame) && deliveredFrame == Time.frameCount) return false;
            int eventFrame;
            bool hasEvent = eventFrames.TryGetValue(keyCode, out eventFrame);
            int deliveredEvent;
            if (!deliveredEvents.TryGetValue(keyCode, out deliveredEvent)) deliveredEvent = -100;
            if (!inputSystemTransition && (!hasEvent || eventFrame < Time.frameCount - 1 || eventFrame <= deliveredEvent)) return false;
            deliveredFrames[keyCode] = Time.frameCount;
            if (hasEvent) deliveredEvents[keyCode] = eventFrame;
            return true;
        }

        private static bool ConsumeMouseTransition(int button, bool inputSystemTransition, int[] eventFrames,
            int[] deliveredFrames, int[] deliveredEvents)
        {
            if (deliveredFrames[button] == Time.frameCount) return false;
            int eventFrame = eventFrames[button];
            if (!inputSystemTransition && (eventFrame < Time.frameCount - 1 || eventFrame <= deliveredEvents[button])) return false;
            deliveredFrames[button] = Time.frameCount;
            if (eventFrame >= Time.frameCount - 1) deliveredEvents[button] = eventFrame;
            return true;
        }

        private static void MarkAlreadyDelivered(KeyCode keyCode, Dictionary<KeyCode, int> deliveredFrames,
            Dictionary<KeyCode, int> deliveredEvents)
        {
            int frame;
            if (deliveredFrames.TryGetValue(keyCode, out frame) && frame == Time.frameCount)
                deliveredEvents[keyCode] = Time.frameCount;
        }

        private static ButtonControl MouseButton(int button)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return null;
            switch (button)
            {
                case 0: return mouse.leftButton;
                case 1: return mouse.rightButton;
                case 2: return mouse.middleButton;
                case 3: return mouse.backButton;
                case 4: return mouse.forwardButton;
                default: return null;
            }
        }

        private static KeyControl KeyControlFor(KeyCode keyCode)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return null;
            Key key;
            return TryConvertKey(keyCode, out key) ? keyboard[key] : null;
        }

        private static bool TryConvertKey(KeyCode keyCode, out Key key)
        {
            switch (keyCode)
            {
                case KeyCode.None: key = Key.None; return false;
                case KeyCode.LeftControl: key = Key.LeftCtrl; return true;
                case KeyCode.RightControl: key = Key.RightCtrl; return true;
                case KeyCode.Return: key = Key.Enter; return true;
                case KeyCode.KeypadEnter: key = Key.NumpadEnter; return true;
                case KeyCode.BackQuote: key = Key.Backquote; return true;
                case KeyCode.Print: key = Key.PrintScreen; return true;
            }

            string name = keyCode.ToString();
            if (name.StartsWith("Alpha", StringComparison.Ordinal)) name = "Digit" + name.Substring(5);
            else if (name.StartsWith("Keypad", StringComparison.Ordinal)) name = "Numpad" + name.Substring(6);
            return Enum.TryParse(name, true, out key) && key != Key.None;
        }
    }

    public sealed class ArcaneInputPump : MonoBehaviour
    {
        private static float _stopVibrationAt;
        public static void StopVibrationAfter(float seconds)
        {
            _stopVibrationAt = Time.unscaledTime + Mathf.Clamp(seconds, 0.02f, 1f);
        }
        private void Update()
        {
            if (_stopVibrationAt <= 0f || Time.unscaledTime < _stopVibrationAt) return;
            _stopVibrationAt = 0f;
            if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
        private void OnGUI() { ArcaneInput.FeedEvent(Event.current); }
        private void OnApplicationFocus(bool focused)
        {
            if (focused) return;
            ArcaneInput.ResetEventState();
            if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }
}
