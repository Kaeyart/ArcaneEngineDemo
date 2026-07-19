using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneEngine
{
    /// <summary>
    /// Bridges the UI Toolkit Combat HUD (ArcaneHUD.uxml) to GameWorld data.
    /// Attach to the same GameObject as the UIDocument.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class ArcaneUIHUDController : MonoBehaviour
    {
        private UIDocument _doc;
        private VisualElement _healthFill, _manaFill, _wardFill;
        private Label _statsLabel, _slot1CD, _slot2CD, _slot3CD;
        private Label _log1, _log2, _log3;
        private VisualElement _slot1, _slot2, _slot3;
        private float _displayedHealth = 100f, _displayedMana = 100f, _displayedWard;

        private void Awake()
        {
            _doc = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            if (_doc == null || _doc.rootVisualElement == null) return;
            var root = _doc.rootVisualElement;
            _healthFill = root.Q<VisualElement>("health-bar-fill");
            _manaFill = root.Q<VisualElement>("mana-bar-fill");
            _wardFill = root.Q<VisualElement>("ward-bar-fill");
            _statsLabel = root.Q<Label>("player-stats-label");
            _slot1CD = root.Q<Label>("spell-1-cooldown");
            _slot2CD = root.Q<Label>("spell-2-cooldown");
            _slot3CD = root.Q<Label>("spell-3-cooldown");
            _log1 = root.Q<Label>("log-line-1"); _log2 = root.Q<Label>("log-line-2"); _log3 = root.Q<Label>("log-line-3");
            _slot1 = root.Q<VisualElement>("spell-slot-1"); _slot2 = root.Q<VisualElement>("spell-slot-2"); _slot3 = root.Q<VisualElement>("spell-slot-3");
        }

        private void Update()
        {
            GameWorld world = GameWorld.Instance;
            if (world == null) return;

            PlayerController player = world.Player;
            PlayerStats stats = world.Stats;
            if (player != null && stats != null)
            {
                _displayedHealth = Mathf.MoveTowards(_displayedHealth, player.Health, Time.unscaledDeltaTime * stats.maxHealth * 2f);
                _displayedMana = Mathf.MoveTowards(_displayedMana, player.Mana, Time.unscaledDeltaTime * stats.maxMana * 2f);
                _displayedWard = Mathf.MoveTowards(_displayedWard, player.Ward, Time.unscaledDeltaTime * Mathf.Max(1f, stats.maxHealth) * 2f);

                float healthPercent = Mathf.Clamp01(_displayedHealth / Mathf.Max(1f, stats.maxHealth));
                float manaPercent = Mathf.Clamp01(_displayedMana / Mathf.Max(1f, stats.maxMana));
                float wardPercent = Mathf.Clamp01(_displayedWard / Mathf.Max(1f, stats.maxHealth));

                if (_healthFill != null) _healthFill.style.width = Length.Percent(healthPercent * 100f);
                if (_manaFill != null) _manaFill.style.width = Length.Percent(manaPercent * 100f);
                if (_wardFill != null) _wardFill.style.width = Length.Percent(wardPercent * 100f);
                if (_statsLabel != null)
                    _statsLabel.text = $"HP: {_displayedHealth:F0}/{stats.maxHealth:F0}  |  MP: {_displayedMana:F0}/{stats.maxMana:F0}";
            }

            for (int i = 0; i < 3; i++)
            {
                CompiledSpell spell = world.GetSpell((SpellSlot)i);
                float remaining = player != null ? player.GetCooldownRemaining((SpellSlot)i) : 0f;
                Label cdLabel = i == 0 ? _slot1CD : (i == 1 ? _slot2CD : _slot3CD);
                VisualElement slotEl = i == 0 ? _slot1 : (i == 1 ? _slot2 : _slot3);
                if (cdLabel != null && remaining > 0.05f)
                    cdLabel.text = remaining.ToString("F1");
                else if (cdLabel != null)
                    cdLabel.text = "";
                if (slotEl != null)
                {
                    slotEl.RemoveFromClassList("spell-slot-ready");
                    slotEl.RemoveFromClassList("spell-slot-on-cooldown");
                    slotEl.RemoveFromClassList("spell-slot-empty");
                    if (spell == null) slotEl.AddToClassList("spell-slot-empty");
                    else if (remaining > 0.05f) slotEl.AddToClassList("spell-slot-on-cooldown");
                    else slotEl.AddToClassList("spell-slot-ready");
                }
            }

            if (world.CombatLog.Count > 0 && _log1 != null) _log1.text = world.CombatLog.Count > 0 ? world.CombatLog[0] : "";
            if (_log2 != null) _log2.text = world.CombatLog.Count > 1 ? world.CombatLog[1] : "";
            if (_log3 != null) _log3.text = world.CombatLog.Count > 2 ? world.CombatLog[2] : "";
        }
    }
}
