using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ArcaneEngine
{
    public enum ArpgAction31
    {
        Interact,
        Inventory,
        MapDevice,
        SpellForge,
        Constellations,
        Pause
    }

    public static class ArpgSettings31
    {
        private const string Prefix = "ArcaneEngine31.";

        public static float MasterVolume
        {
            get { return PlayerPrefs.GetFloat(Prefix + "MasterVolume", 0.9f); }
            set { PlayerPrefs.SetFloat(Prefix + "MasterVolume", Mathf.Clamp01(value)); Apply(); }
        }

        public static float MusicVolume
        {
            get { return PlayerPrefs.GetFloat(Prefix + "MusicVolume", 0.65f); }
            set { PlayerPrefs.SetFloat(Prefix + "MusicVolume", Mathf.Clamp01(value)); }
        }

        public static float EffectsVolume
        {
            get { return PlayerPrefs.GetFloat(Prefix + "EffectsVolume", 0.9f); }
            set { PlayerPrefs.SetFloat(Prefix + "EffectsVolume", Mathf.Clamp01(value)); }
        }

        public static float InterfaceVolume
        {
            get { return PlayerPrefs.GetFloat(Prefix + "InterfaceVolume", 0.8f); }
            set { PlayerPrefs.SetFloat(Prefix + "InterfaceVolume", Mathf.Clamp01(value)); }
        }

        public static float InterfaceScale
        {
            get { return PlayerPrefs.GetFloat(Prefix + "InterfaceScale", 1f); }
            set { PlayerPrefs.SetFloat(Prefix + "InterfaceScale", Mathf.Clamp(value, 0.8f, 1.35f)); }
        }

        public static float CameraShake
        {
            get { return PlayerPrefs.GetFloat(Prefix + "CameraShake", 0.7f); }
            set { PlayerPrefs.SetFloat(Prefix + "CameraShake", Mathf.Clamp01(value)); }
        }

        public static float FlashIntensity
        {
            get { return PlayerPrefs.GetFloat(Prefix + "FlashIntensity", 0.75f); }
            set { PlayerPrefs.SetFloat(Prefix + "FlashIntensity", Mathf.Clamp01(value)); }
        }

        public static bool ShowDamageNumbers
        {
            get { return PlayerPrefs.GetInt(Prefix + "DamageNumbers", 1) != 0; }
            set { PlayerPrefs.SetInt(Prefix + "DamageNumbers", value ? 1 : 0); }
        }

        public static bool VSync
        {
            get { return PlayerPrefs.GetInt(Prefix + "VSync", 1) != 0; }
            set { PlayerPrefs.SetInt(Prefix + "VSync", value ? 1 : 0); Apply(); }
        }

        public static int FrameRateLimit
        {
            get { return PlayerPrefs.GetInt(Prefix + "FrameRateLimit", 120); }
            set { PlayerPrefs.SetInt(Prefix + "FrameRateLimit", Mathf.Clamp(value, 30, 360)); Apply(); }
        }

        public static bool Fullscreen
        {
            get { return PlayerPrefs.GetInt(Prefix + "Fullscreen", Screen.fullScreen ? 1 : 0) != 0; }
            set
            {
                PlayerPrefs.SetInt(Prefix + "Fullscreen", value ? 1 : 0);
                Screen.fullScreenMode = value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            }
        }

        public static bool HighContrastLoot
        {
            get { return PlayerPrefs.GetInt(Prefix + "HighContrastLoot", 0) != 0; }
            set { PlayerPrefs.SetInt(Prefix + "HighContrastLoot", value ? 1 : 0); }
        }

        public static bool HoldToInteract
        {
            get { return PlayerPrefs.GetInt(Prefix + "HoldToInteract", 0) != 0; }
            set { PlayerPrefs.SetInt(Prefix + "HoldToInteract", value ? 1 : 0); }
        }

        public static string ResolutionLabel
        {
            get { return Screen.width + " × " + Screen.height + " @ " + Screen.currentResolution.refreshRateRatio.value.ToString("0") + " Hz"; }
        }

        public static void ChangeResolution(int direction)
        {
            Resolution[] resolutions = Screen.resolutions;
            if (resolutions == null || resolutions.Length == 0) return;
            int current = 0;
            int bestDistance = int.MaxValue;
            for (int index = 0; index < resolutions.Length; index++)
            {
                int distance = Math.Abs(resolutions[index].width - Screen.width) + Math.Abs(resolutions[index].height - Screen.height);
                if (distance < bestDistance) { bestDistance = distance; current = index; }
            }
            int target = Mathf.Clamp(current + Math.Sign(direction), 0, resolutions.Length - 1);
            Resolution selected = resolutions[target];
            Screen.SetResolution(selected.width, selected.height, Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, selected.refreshRateRatio);
        }

        public static int QualityLevel
        {
            get { return PlayerPrefs.GetInt(Prefix + "QualityLevel", Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, Math.Max(0, QualitySettings.names.Length - 1))); }
            set
            {
                int maximum = Mathf.Max(0, QualitySettings.names.Length - 1);
                int level = Mathf.Clamp(value, 0, maximum);
                PlayerPrefs.SetInt(Prefix + "QualityLevel", level);
                if (QualitySettings.names.Length > 0) QualitySettings.SetQualityLevel(level, true);
            }
        }

        public static void Apply()
        {
            AudioListener.volume = MasterVolume;
            QualitySettings.vSyncCount = VSync ? 1 : 0;
            Application.targetFrameRate = VSync ? -1 : FrameRateLimit;
            if (QualitySettings.names.Length > 0)
                QualitySettings.SetQualityLevel(Mathf.Clamp(QualityLevel, 0, QualitySettings.names.Length - 1), true);
            Screen.fullScreenMode = Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        }

        public static KeyCode Key(ArpgAction31 action)
        {
            KeyCode fallback;
            switch (action)
            {
                case ArpgAction31.Interact: fallback = KeyCode.E; break;
                case ArpgAction31.Inventory: fallback = KeyCode.I; break;
                case ArpgAction31.MapDevice: fallback = KeyCode.M; break;
                case ArpgAction31.SpellForge: fallback = KeyCode.K; break;
                case ArpgAction31.Constellations: fallback = KeyCode.C; break;
                default: fallback = KeyCode.Escape; break;
            }

            int stored = PlayerPrefs.GetInt(Prefix + "Key." + action, (int)fallback);
            return Enum.IsDefined(typeof(KeyCode), stored) ? (KeyCode)stored : fallback;
        }

        public static void SetKey(ArpgAction31 action, KeyCode key)
        {
            if (key == KeyCode.None) return;
            PlayerPrefs.SetInt(Prefix + "Key." + action, (int)key);
            PlayerPrefs.Save();
        }

        public static string BindingLabel(ArpgAction31 action)
        {
            return Key(action).ToString();
        }
    }

    public static class ArpgInput31
    {
        public static bool Pressed(ArpgAction31 action)
        {
#if ENABLE_INPUT_SYSTEM
            if (action == ArpgAction31.Pause && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) return true;
            KeyCode configured = ArpgSettings31.Key(action);
            UnityEngine.InputSystem.Key parsed;
            if (Keyboard.current != null && Enum.TryParse(configured.ToString(), true, out parsed))
            {
                var control = Keyboard.current[parsed];
                if (control != null && control.wasPressedThisFrame) return true;
            }

            if (Gamepad.current != null)
            {
                if (action == ArpgAction31.Interact && Gamepad.current.buttonSouth.wasPressedThisFrame) return true;
                if (action == ArpgAction31.Pause && Gamepad.current.startButton.wasPressedThisFrame) return true;
                if (action == ArpgAction31.Inventory && Gamepad.current.selectButton.wasPressedThisFrame) return true;
            }
            return false;
#else
            return Input.GetKeyDown(ArpgSettings31.Key(action));
#endif
        }

        public static bool Held(ArpgAction31 action)
        {
#if ENABLE_INPUT_SYSTEM
            KeyCode configured = ArpgSettings31.Key(action);
            UnityEngine.InputSystem.Key parsed;
            if (Keyboard.current != null && Enum.TryParse(configured.ToString(), true, out parsed))
            {
                var control = Keyboard.current[parsed];
                if (control != null && control.isPressed) return true;
            }
            return action == ArpgAction31.Interact && Gamepad.current != null && Gamepad.current.buttonSouth.isPressed;
#else
            return Input.GetKey(ArpgSettings31.Key(action));
#endif
        }

        public static bool ConfirmPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)) ||
                   (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);
#else
            return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);
#endif
        }

        public static bool CancelPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
                   (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame);
#else
            return Input.GetKeyDown(KeyCode.Escape);
#endif
        }

        public static int VerticalNavigation()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) return -1;
                if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame) return 1;
            }
            if (Gamepad.current != null)
            {
                if (Gamepad.current.dpad.up.wasPressedThisFrame) return -1;
                if (Gamepad.current.dpad.down.wasPressedThisFrame) return 1;
            }
            return 0;
#else
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) return -1;
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) return 1;
            return 0;
#endif
        }

        public static int HorizontalNavigation()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame) return -1;
                if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame) return 1;
            }
            if (Gamepad.current != null)
            {
                if (Gamepad.current.dpad.left.wasPressedThisFrame) return -1;
                if (Gamepad.current.dpad.right.wasPressedThisFrame) return 1;
            }
            return 0;
#else
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) return -1;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) return 1;
            return 0;
#endif
        }
    }
}
