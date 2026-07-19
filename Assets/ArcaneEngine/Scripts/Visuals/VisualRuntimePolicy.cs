using System;
using UnityEngine;

namespace ArcaneEngine
{
    public enum VisualRuntimeKind
    {
        Projectile,
        PersistentZone,
        EnemyAssembly,
        StatusLayer,
        Telegraph,
        RewardPresentation,
        InteractionPresentation,
        Trail,
        DeathPiece,
        Familiar,
        RoomRenderer
    }

    /// <summary>
    /// Event-driven counters used by diagnostics and stress tests. Unlike scene queries,
    /// these have no cost while the diagnostics overlay is hidden.
    /// </summary>
    public static class VisualRuntimeRegistry
    {
        private static readonly int[] Counts = new int[Enum.GetValues(typeof(VisualRuntimeKind)).Length];
        private static int _roomRenderers;
        private static string _roomLabel = "none";

        public static void Register(VisualRuntimeKind kind) { Counts[(int)kind]++; }
        public static void Unregister(VisualRuntimeKind kind) { Counts[(int)kind] = Mathf.Max(0, Counts[(int)kind] - 1); }
        public static int Count(VisualRuntimeKind kind) { return Counts[(int)kind]; }
        public static int RoomRenderers { get { return _roomRenderers; } }
        public static string RoomLabel { get { return _roomLabel; } }

        public static void SetRoom(string label, int renderers)
        {
            _roomLabel = string.IsNullOrEmpty(label) ? "none" : label;
            _roomRenderers = Mathf.Max(0, renderers);
            Counts[(int)VisualRuntimeKind.RoomRenderer] = _roomRenderers;
        }

        public static void ClearRoom()
        {
            _roomLabel = "none";
            _roomRenderers = 0;
            Counts[(int)VisualRuntimeKind.RoomRenderer] = 0;
        }
    }

    public sealed class VisualCounterRegistration : MonoBehaviour
    {
        private VisualRuntimeKind _kind;
        private bool _initialized;
        private bool _registered;

        public static VisualCounterRegistration Attach(GameObject target, VisualRuntimeKind kind)
        {
            if (target == null) return null;
            VisualCounterRegistration registration = target.GetComponent<VisualCounterRegistration>();
            if (registration == null) registration = target.AddComponent<VisualCounterRegistration>();
            registration.Initialize(kind);
            return registration;
        }

        public void Initialize(VisualRuntimeKind kind)
        {
            if (_registered) VisualRuntimeRegistry.Unregister(_kind);
            _kind = kind;
            _initialized = true;
            _registered = true;
            VisualRuntimeRegistry.Register(_kind);
        }

        private void OnEnable()
        {
            if (!_initialized || _registered) return;
            _registered = true;
            VisualRuntimeRegistry.Register(_kind);
        }

        private void OnDisable()
        {
            if (!_registered) return;
            VisualRuntimeRegistry.Unregister(_kind);
            _registered = false;
        }

        private void OnDestroy()
        {
            if (!_registered) return;
            VisualRuntimeRegistry.Unregister(_kind);
            _registered = false;
        }
    }

    public struct VisualQualityBudget
    {
        public int maxActiveVisuals;
        public int maxLights;
        public int maxDecals;
        public int ringSegments;
        public int statusLayers;
        public int deathPieces;
        public bool projectileShells;
        public bool ambientMotion;

        public static VisualQualityBudget Current
        {
            get { return ForQuality(ProceduralVisualRuntime.Quality); }
        }

        public static VisualQualityBudget ForQuality(ArcaneVisualQuality quality)
        {
            if (quality == ArcaneVisualQuality.Low)
                return new VisualQualityBudget { maxActiveVisuals = 150, maxLights = 2, maxDecals = 16, ringSegments = 20, statusLayers = 1, deathPieces = 3, projectileShells = false, ambientMotion = false };
            if (quality == ArcaneVisualQuality.Medium)
                return new VisualQualityBudget { maxActiveVisuals = 240, maxLights = 5, maxDecals = 32, ringSegments = 32, statusLayers = 2, deathPieces = 5, projectileShells = true, ambientMotion = true };
            return new VisualQualityBudget { maxActiveVisuals = 320, maxLights = 10, maxDecals = 48, ringSegments = 48, statusLayers = 3, deathPieces = 8, projectileShells = true, ambientMotion = true };
        }
    }

    public static class VisualQualityPolicy
    {
        public static void ApplyPreset(AccessibilitySettings settings, int quality)
        {
            if (settings == null) return;
            settings.visualQuality = Mathf.Clamp(quality, 0, 2);
            if (settings.visualQuality == 0)
            {
                settings.spellEffectDensity = 0.48f;
                settings.environmentDensity = 0.38f;
                settings.dynamicLightQuality = 0;
                settings.shadowQuality = 0;
                settings.decalDuration = 0.45f;
            }
            else if (settings.visualQuality == 1)
            {
                settings.spellEffectDensity = 0.75f;
                settings.environmentDensity = 0.68f;
                settings.dynamicLightQuality = 1;
                settings.shadowQuality = 1;
                settings.decalDuration = 0.85f;
            }
            else
            {
                settings.spellEffectDensity = 1f;
                settings.environmentDensity = 1f;
                settings.dynamicLightQuality = 2;
                settings.shadowQuality = 2;
                settings.decalDuration = 1.25f;
            }
        }
    }

    public static class VisualAccessibility
    {
        public static bool UseSymbols
        {
            get { return ProfileManager.Current != null && ProfileManager.Current.accessibility.colorblindSymbols; }
        }

        public static string ElementSymbol(SpellElement element)
        {
            switch (element)
            {
                case SpellElement.Fire: return "▲";
                case SpellElement.Frost: return "◆";
                case SpellElement.Lightning: return "ϟ";
                case SpellElement.Toxic: return "●";
                case SpellElement.Void: return "○";
                case SpellElement.Arcane: return "◈";
                case SpellElement.Physical: return "■";
                case SpellElement.Blood: return "♥";
                default: return "✦";
            }
        }

        public static string RaritySymbol(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Magic: return "◇";
                case ItemRarity.Rare: return "△";
                case ItemRarity.Unique: return "✦";
                default: return "□";
            }
        }

        public static TextMesh AddWorldSymbol(Transform parent, string value, Color color, float size = 0.22f)
        {
            if (!UseSymbols || parent == null || string.IsNullOrEmpty(value)) return null;
            GameObject root = new GameObject("Accessibility Symbol · " + value);
            root.transform.SetParent(parent, false);
            TextMesh text = root.AddComponent<TextMesh>();
            text.text = value;
            text.fontSize = 56;
            text.characterSize = size;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = color;
            root.transform.localPosition = Vector3.up * 0.2f;
            root.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            return text;
        }
    }
}
