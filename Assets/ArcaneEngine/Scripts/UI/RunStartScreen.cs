using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneEngine
{
    public sealed class RunStartScreen : MonoBehaviour
    {
        public static RunStartScreen Instance { get; private set; }
        public static bool IsOpen { get { return Instance != null && Instance._open; } }

        private UIDocument _document;
        private VisualElement _root;
        private VisualElement _panel;
        private VisualElement _slotStrip;
        private VisualElement _spellList;
        private VisualElement _modeStrip;
        private TextField _seedField;
        private Label _heading;
        private Label _message;
        private RunDirector _run;
        private int _selectedSlot;
        private bool _open;
        private DemoRunMode _selectedMode;
        private string _seedValue = string.Empty;

        private static readonly Color Panel = new Color(0.025f, 0.035f, 0.065f, 0.985f);
        private static readonly Color Card = new Color(0.06f, 0.085f, 0.13f, 1f);
        private static readonly Color Accent = new Color(0.12f, 0.68f, 0.82f, 1f);
        private static readonly Color Text = new Color(0.91f, 0.95f, 1f, 1f);
        private static readonly Color Muted = new Color(0.58f, 0.68f, 0.78f, 1f);

        private void Awake()
        {
            Instance = this;
            _document = RuntimeUIFactory.CreateDocument(transform, "UI Document · Run Start", 60);
            Build();
            Hide();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public static void Show(RunDirector run)
        {
            if (Instance == null || run == null) return;
            Instance._run = run;
            Instance._selectedSlot = 0;
            Instance._selectedMode = DemoRunMode.Standard;
            Instance._seedValue = string.Empty;
            Instance._open = true;
            Instance._root.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            Instance.Refresh();
        }

        private void Hide()
        {
            _open = false;
            if (_root != null) _root.style.display = DisplayStyle.None;
        }

        private void Update()
        {
            if (!_open) return;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            float width = _root.resolvedStyle.width;
            float height = _root.resolvedStyle.height;
            if (float.IsNaN(width) || width <= 0f) width = Screen.width;
            if (float.IsNaN(height) || height <= 0f) height = Screen.height;
            _panel.style.left = Mathf.Max(12f, (width - 1160f) * 0.5f);
            _panel.style.top = Mathf.Max(12f, (height - 760f) * 0.5f);
            if (ArcaneInput.GetKeyDown(KeyCode.Escape)) Hide();
        }

        private void Build()
        {
            _root = _document.rootVisualElement;
            _root.name = "Starting Spell Selection";
            _root.style.position = Position.Absolute;
            _root.style.left = 0f; _root.style.top = 0f; _root.style.right = 0f; _root.style.bottom = 0f;
            _root.style.backgroundColor = new Color(0.005f, 0.008f, 0.018f, 0.88f);

            _panel = new VisualElement();
            _panel.style.position = Position.Absolute;
            _panel.style.width = 1160f;
            _panel.style.height = 760f;
            _panel.style.paddingLeft = 30f; _panel.style.paddingRight = 30f;
            _panel.style.paddingTop = 26f; _panel.style.paddingBottom = 24f;
            _panel.style.backgroundColor = Panel;
            SetBorder(_panel, Accent, 2f);
            SetRadius(_panel, 12f);
            _root.Add(_panel);

            _heading = Label("CHOOSE YOUR STARTING SPELL", 30, Text, true);
            _panel.Add(_heading);
            Label intro = Label("Select the Spell Core carried in each unlocked slot. Additional slots are permanent upgrades; empty slots can still be filled with Spell Copies found during the run.", 14, Muted, false);
            intro.style.whiteSpace = WhiteSpace.Normal;
            intro.style.marginTop = 7f;
            _panel.Add(intro);

            _modeStrip = new VisualElement();
            _modeStrip.style.flexDirection = FlexDirection.Row;
            _modeStrip.style.height = 68f;
            _modeStrip.style.marginTop = 14f;
            _panel.Add(_modeStrip);

            _slotStrip = new VisualElement();
            _slotStrip.style.flexDirection = FlexDirection.Row;
            _slotStrip.style.height = 116f;
            _slotStrip.style.marginTop = 22f;
            _panel.Add(_slotStrip);

            Label libraryTitle = Label("AVAILABLE SPELL COPIES", 14, Muted, true);
            libraryTitle.style.letterSpacing = 1.5f;
            libraryTitle.style.marginTop = 14f;
            _panel.Add(libraryTitle);

            ScrollView scroll = new ScrollView(ScrollViewMode.Vertical);
            scroll.style.flexGrow = 1f;
            scroll.style.marginTop = 8f;
            _spellList = scroll.contentContainer;
            _panel.Add(scroll);

            _message = Label(string.Empty, 12, Muted, false);
            _message.style.height = 25f;
            _message.style.marginTop = 8f;
            _panel.Add(_message);

            VisualElement actions = new VisualElement();
            actions.style.flexDirection = FlexDirection.Row;
            actions.style.justifyContent = Justify.FlexEnd;
            actions.style.height = 54f;
            Button cancel = ActionButton("BACK", Hide, false);
            cancel.style.width = 170f;
            Button start = ActionButton("ENTER THE DUNGEON", StartRun, true);
            start.style.width = 300f;
            actions.Add(cancel); actions.Add(start);
            _panel.Add(actions);
        }

        private void Refresh()
        {
            ProfileData profile = ProfileManager.Current;
            if (profile == null) return;
            _heading.text = "CHOOSE YOUR STARTING SPELL  ·  " + profile.StartingSpellSlots + " SLOT" + (profile.StartingSpellSlots == 1 ? string.Empty : "S");
            RefreshModes(profile);
            _slotStrip.Clear();
            for (int slot = 0; slot < 3; slot++)
            {
                int captured = slot;
                bool unlocked = slot < profile.StartingSpellSlots;
                PreparedSpellSave prepared = profile.preparedSpells.FirstOrDefault(value => value.slotIndex == slot);
                string chosen = PreparedName(prepared);
                Button slotCard = ActionButton("SLOT " + (slot + 1) + "\n" + (unlocked ? chosen : "LOCKED · PERMANENT UPGRADE"), () =>
                {
                    if (!unlocked) return;
                    _selectedSlot = captured;
                    Refresh();
                }, _selectedSlot == slot && unlocked);
                slotCard.style.flexGrow = 1f;
                slotCard.style.height = 102f;
                slotCard.style.marginRight = slot < 2 ? 10f : 0f;
                slotCard.SetEnabled(unlocked);
                _slotStrip.Add(slotCard);
            }

            _spellList.Clear();
            IEnumerable<IGrouping<string, CoreSaveData>> groups = profile.spellArchive.Where(core => core != null && DemoCatalog.GetCore(core.coreId) != null).GroupBy(core => core.coreId);
            foreach (IGrouping<string, CoreSaveData> group in groups.OrderBy(group => DemoCatalog.GetCore(group.Key).displayName))
            {
                SpellCoreDefinition core = DemoCatalog.GetCore(group.Key);
                int count = group.Count();
                string coreId = core.id;
                Button choice = ActionButton(core.displayName.ToUpperInvariant() + "   ×" + count + "\n" + core.description,
                    () => SelectStandard(coreId, count), false);
                choice.style.height = 66f;
                choice.style.marginBottom = 7f;
                choice.style.borderLeftColor = core.color;
                choice.style.borderLeftWidth = 5f;
                _spellList.Add(choice);
            }

            foreach (RelicSaveData saved in profile.relicArchive.Where(value => value != null))
            {
                RelicDefinition relic = MegaCatalog.GetRelic(saved.relicId);
                if (relic == null) continue;
                string relicId = relic.id;
                Button choice = ActionButton("LEGENDARY · " + relic.displayName.ToUpperInvariant() + "\n" + relic.signatureRule,
                    () => SelectRelic(relicId), false);
                choice.style.height = 70f;
                choice.style.marginBottom = 7f;
                choice.style.borderLeftColor = relic.color;
                choice.style.borderLeftWidth = 5f;
                _spellList.Add(choice);
            }
        }

        private void SelectStandard(string coreId, int ownedCopies)
        {
            ProfileData profile = ProfileManager.Current;
            int usedElsewhere = profile.preparedSpells.Count(value => value.slotIndex != _selectedSlot && !value.isRelic && value.contentId == coreId && value.slotIndex < profile.StartingSpellSlots);
            if (usedElsewhere >= ownedCopies)
            {
                _message.text = "Another physical Spell Copy is required to use that spell in two slots.";
                return;
            }
            profile.preparedSpells.RemoveAll(value => value.slotIndex == _selectedSlot);
            profile.preparedSpells.Add(new PreparedSpellSave { slotIndex = _selectedSlot, contentId = coreId, isRelic = false });
            ProfileManager.Save();
            _message.text = DemoCatalog.GetCore(coreId).displayName + " selected for Slot " + (_selectedSlot + 1) + ".";
            Refresh();
        }

        private void SelectRelic(string relicId)
        {
            ProfileData profile = ProfileManager.Current;
            PreparedSpellSave other = profile.preparedSpells.FirstOrDefault(value => value.isRelic && value.slotIndex != _selectedSlot && value.slotIndex < profile.StartingSpellSlots);
            if (other != null)
            {
                _message.text = "Only one Legendary Spell can be active. Remove it from the other slot first.";
                return;
            }
            profile.preparedSpells.RemoveAll(value => value.slotIndex == _selectedSlot);
            profile.preparedSpells.Add(new PreparedSpellSave { slotIndex = _selectedSlot, contentId = relicId, isRelic = true });
            ProfileManager.Save();
            _message.text = MegaCatalog.GetRelic(relicId).displayName + " selected for Slot " + (_selectedSlot + 1) + ".";
            Refresh();
        }

        private void StartRun()
        {
            ProfileData profile = ProfileManager.Current;
            PreparedSpellSave first = profile.preparedSpells.FirstOrDefault(value => value.slotIndex == 0);
            if (first == null)
            {
                _message.text = "Select a starting spell for Slot 1 first.";
                return;
            }
            GameWorld world = GameWorld.Instance;
            string loadoutMessage = string.Empty;
            if (world == null || world.Equipment == null || !world.Equipment.ValidateLoadout(out loadoutMessage))
            {
                _message.text = string.IsNullOrEmpty(loadoutMessage) ? "The equipment loadout could not be validated." : loadoutMessage;
                return;
            }
            ProfileManager.Save();
            RunDirector run = _run;
            int customSeed = 0;
            bool useSeed = _selectedMode == DemoRunMode.Standard && _seedField != null &&
                !string.IsNullOrWhiteSpace(_seedField.value) && int.TryParse(_seedField.value, out customSeed) && customSeed != 0;
            Hide();
            if (run != null)
            {
                if (useSeed) run.BeginSeededRun(customSeed);
                else run.BeginRun(_selectedMode);
            }
        }

        private void RefreshModes(ProfileData profile)
        {
            _modeStrip.Clear();
            AddModeButton(DemoRunMode.Standard, "STANDARD\nDefeat the final boss", true);
            AddModeButton(DemoRunMode.Daily, "DAILY CHALLENGE\nShared UTC seed", true);
            AddModeButton(DemoRunMode.Endless, "ENDLESS\nBoss every 5 rooms", profile.endlessUnlocked);
            _seedField = new TextField("CUSTOM SEED");
            _seedField.value = _seedValue;
            _seedField.RegisterValueChangedCallback(change => _seedValue = change.newValue);
            _seedField.tooltip = "Optional Standard-run seed. Leave blank for a random dungeon.";
            _seedField.style.width = 210f;
            _seedField.style.height = 58f;
            _seedField.style.marginLeft = 8f;
            _seedField.style.color = Text;
            _seedField.style.backgroundColor = Card;
            _seedField.SetEnabled(_selectedMode == DemoRunMode.Standard);
            _modeStrip.Add(_seedField);
        }

        private void AddModeButton(DemoRunMode mode, string label, bool enabled)
        {
            Button button = ActionButton(label + (enabled ? string.Empty : " · LOCKED"), () =>
            {
                if (!enabled)
                {
                    _message.text = "Complete a Standard run to unlock Endless mode.";
                    return;
                }
                _selectedMode = mode;
                Refresh();
            }, _selectedMode == mode);
            button.style.flexGrow = 1f;
            button.style.height = 58f;
            button.style.marginRight = 8f;
            button.SetEnabled(enabled);
            _modeStrip.Add(button);
        }

        private static string PreparedName(PreparedSpellSave prepared)
        {
            if (prepared == null) return "EMPTY";
            if (prepared.isRelic)
            {
                RelicDefinition relic = MegaCatalog.GetRelic(prepared.contentId);
                return relic == null ? "EMPTY" : relic.displayName.ToUpperInvariant();
            }
            SpellCoreDefinition core = DemoCatalog.GetCore(prepared.contentId);
            return core == null ? "EMPTY" : core.displayName.ToUpperInvariant();
        }

        private static Button ActionButton(string text, System.Action action, bool selected)
        {
            Button button = RuntimeUIFactory.CreateButton(action);
            button.text = text;
            button.style.fontSize = 14f;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.color = Text;
            button.style.paddingLeft = 16f; button.style.paddingRight = 16f;
            button.style.backgroundColor = selected ? new Color(0.08f, 0.38f, 0.48f, 1f) : Card;
            SetBorder(button, selected ? Accent : new Color(0.18f, 0.3f, 0.4f), selected ? 2f : 1f);
            SetRadius(button, 7f);
            return button;
        }

        private static Label Label(string text, int size, Color color, bool bold)
        {
            Label label = new Label(text);
            label.style.fontSize = size;
            label.style.color = color;
            label.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
            return label;
        }

        private static void SetBorder(VisualElement element, Color color, float width)
        {
            element.style.borderLeftWidth = width; element.style.borderRightWidth = width;
            element.style.borderTopWidth = width; element.style.borderBottomWidth = width;
            element.style.borderLeftColor = color; element.style.borderRightColor = color;
            element.style.borderTopColor = color; element.style.borderBottomColor = color;
        }

        private static void SetRadius(VisualElement element, float radius)
        {
            element.style.borderTopLeftRadius = radius; element.style.borderTopRightRadius = radius;
            element.style.borderBottomLeftRadius = radius; element.style.borderBottomRightRadius = radius;
        }
    }
}
