using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneEngine
{
    /// <summary>
    /// Passive UI Toolkit HUD. Gameplay input never depends on these elements and the
    /// overlay does not use buttons, IMGUI layout, or fixed world-height guesses.
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public sealed class ModernCombatHUD : MonoBehaviour
    {
        private sealed class BarView
        {
            public VisualElement root;
            public VisualElement fill;
            public Label text;

            public void Set(float ratio, string value)
            {
                fill.style.width = Length.Percent(Mathf.Clamp01(ratio) * 100f);
                text.text = value;
            }
        }

        private sealed class SpellView
        {
            public VisualElement root;
            public VisualElement accent;
            public Label key;
            public Label name;
            public Label details;
            public Label cooldown;
        }

        private sealed class EnemyView
        {
            public VisualElement root;
            public VisualElement health;
            public VisualElement delayed;
            public VisualElement shield;
            public VisualElement armor;
            public Label name;
            public Label status;
            public float baseWidth;
            public float displayedRatio = 1f;
        }

        private sealed class DamageView
        {
            public Label label;
            public Vector3 position;
            public float created;
            public float amount;
            public bool critical;
        }

        public static ModernCombatHUD Instance { get; private set; }

        private readonly Dictionary<EnemyController, EnemyView> _enemyViews = new Dictionary<EnemyController, EnemyView>();
        private readonly Stack<EnemyView> _enemyViewPool = new Stack<EnemyView>();
        private readonly HashSet<EnemyController> _liveEnemies = new HashSet<EnemyController>();
        private readonly List<EnemyController> _staleEnemies = new List<EnemyController>();
        private readonly List<DamageView> _damageViews = new List<DamageView>();
        private float _nextHudRefresh;
        private float _nextEnemyStructureSync;
        private float _nextOffscreenRefresh;
        private UIDocument _document;
        private PanelSettings _panelSettings;
        private VisualElement _root;
        private VisualElement _enemyLayer;
        private VisualElement _damageLayer;
        private VisualElement _crosshair;
        private VisualElement _playerCard;
        private VisualElement _runCard;
        private VisualElement _spellTray;
        private VisualElement _bossCard;
        private Label _bossName;
        private BarView _bossHealth;
        private BarView _health;
        private BarView _mana;
        private BarView _shield;
        private Label _roomTitle;
        private Label _runDetails;
        private VisualElement _objectiveCard;
        private Label _objectiveTitle;
        private Label _objectiveDetail;
        private BarView _objectiveProgress;
        private Label _mapLabel;
        private VisualElement _announcementCard;
        private Label _announcement;
        private VisualElement _tutorialCard;
        private Label _tutorialPrompt;
        private Label _controls;
        private VisualElement _interactionCard;
        private Label _interactionTitle;
        private Label _interactionDescription;
        private Label _offscreenIndicator;
        private readonly SpellView[] _spells = new SpellView[3];

        private static readonly Color Panel = new Color(0.025f, 0.035f, 0.065f, 0.94f);
        private static readonly Color PanelLight = new Color(0.06f, 0.085f, 0.13f, 0.96f);
        private static readonly Color Border = new Color(0.2f, 0.42f, 0.58f, 0.85f);
        private static readonly Color Text = new Color(0.9f, 0.95f, 1f, 1f);
        private static readonly Color Muted = new Color(0.58f, 0.68f, 0.78f, 1f);

        private void Awake()
        {
            Instance = this;
            _panelSettings = RuntimeUIFactory.SharedPanel;
            _document = RuntimeUIFactory.CreateDocument(transform, "UI Document · Combat HUD", 20);
            Build();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            UnityEngine.Cursor.visible = true;
        }

        public static void ShowDamage(Vector3 position, float amount, Color color, bool critical)
        {
            if (Instance == null || amount <= 0.01f ||
                (ProfileManager.Current != null && !ProfileManager.Current.accessibility.showDamageNumbers)) return;
            AccessibilitySettings settings = ProfileManager.Current.accessibility;
            if (Instance._damageViews.Count >= Mathf.RoundToInt(70f * Mathf.Clamp(settings.damageNumberDensity, 0.25f, 1f))) return;
            if (settings.combineRapidDamage)
            {
                DamageView combined = Instance._damageViews.LastOrDefault(view => Time.unscaledTime - view.created < 0.14f && (view.position - (position + Vector3.up * 0.9f)).sqrMagnitude < 0.5f);
                if (combined != null)
                {
                    combined.amount += amount; combined.critical |= critical; combined.created = Time.unscaledTime;
                    combined.label.text = Mathf.Max(1, Mathf.RoundToInt(combined.amount)).ToString();
                    combined.label.style.fontSize = Mathf.RoundToInt((combined.critical ? 25f : 17f) * settings.damageNumberScale);
                    combined.label.style.color = color;
                    return;
                }
            }
            if (V1PerformanceBudget.Instance != null && !V1PerformanceBudget.Instance.TryAcquire(RuntimeEntityKind.DamageNumber)) return;
            Label label = NewLabel(Mathf.Max(1, Mathf.RoundToInt(amount)).ToString(), Mathf.RoundToInt((critical ? 25f : 17f) * settings.damageNumberScale), Color.white, true);
            label.style.position = Position.Absolute;
            label.style.width = critical ? 130f : 100f;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.color = color;
            label.pickingMode = PickingMode.Ignore;
            Instance._damageLayer.Add(label);
            Instance._damageViews.Add(new DamageView { label = label, position = position + Vector3.up * 0.9f, created = Time.unscaledTime, amount = amount, critical = critical });
            while (Instance._damageViews.Count > 70) Instance.RemoveDamage(0);
        }

        private void Build()
        {
            _root = _document.rootVisualElement;
            _root.name = "Modern Combat HUD";
            _root.pickingMode = PickingMode.Ignore;
            _root.style.position = Position.Absolute;
            _root.style.left = 0f;
            _root.style.top = 0f;
            _root.style.right = 0f;
            _root.style.bottom = 0f;

            _enemyLayer = FullscreenLayer("Enemy Health Layer");
            _enemyLayer.style.display = DisplayStyle.Flex;
            _damageLayer = FullscreenLayer("Damage Number Layer");
            _root.Add(_enemyLayer);
            _root.Add(_damageLayer);

            BuildPlayerCard();
            BuildRunCard();
            BuildObjectiveCard();
            BuildSpellTray();
            BuildBossCard();
            BuildCrosshair();
            BuildInteractionCard();
            BuildAnnouncements();
            _offscreenIndicator = NewLabel(string.Empty, 14, new Color(1f, 0.72f, 0.25f), true);
            _offscreenIndicator.style.position = Position.Absolute;
            _offscreenIndicator.style.top = 272f;
            _offscreenIndicator.style.width = 360f;
            _offscreenIndicator.style.unityTextAlign = TextAnchor.MiddleCenter;
            _offscreenIndicator.style.display = DisplayStyle.None;
            _root.Add(_offscreenIndicator);
        }

        private void BuildPlayerCard()
        {
            _playerCard = PanelElement(350f, 170f);
            _playerCard.style.left = 24f;
            _playerCard.style.top = 24f;
            _root.Add(_playerCard);

            Label title = NewLabel("PLAYER", 12, Muted, true);
            title.style.letterSpacing = 2f;
            _playerCard.Add(title);
            _health = CreateBar("HEALTH", new Color(0.9f, 0.12f, 0.2f), 25f);
            _mana = CreateBar("MANA", new Color(0.1f, 0.5f, 1f), 23f);
            _shield = CreateBar("SHIELD", new Color(0.15f, 0.78f, 1f), 16f);
            _playerCard.Add(_health.root);
            _playerCard.Add(_mana.root);
            _playerCard.Add(_shield.root);
            _controls = NewLabel("WASD MOVE  ·  MOUSE AIM  ·  E INTERACT  ·  TAB SPELLS  ·  I GEAR", 10, Muted, false);
            _controls.style.marginTop = 7f;
            _playerCard.Add(_controls);
        }

        private void BuildRunCard()
        {
            VisualElement card = PanelElement(370f, 112f);
            _runCard = card;
            card.style.right = 24f;
            card.style.top = 24f;
            card.style.alignItems = Align.FlexEnd;
            _root.Add(card);
            _roomTitle = NewLabel("ROOM", 18, Text, true);
            _roomTitle.style.unityTextAlign = TextAnchor.MiddleRight;
            _runDetails = NewLabel(string.Empty, 12, Muted, false);
            _runDetails.style.unityTextAlign = TextAnchor.MiddleRight;
            _runDetails.style.marginTop = 7f;
            card.Add(_roomTitle);
            card.Add(_runDetails);
        }

        private void BuildObjectiveCard()
        {
            _objectiveCard = PanelElement(430f, 105f);
            _objectiveCard.style.right = 24f;
            _objectiveCard.style.top = 148f;
            _root.Add(_objectiveCard);
            _objectiveTitle = NewLabel("ROOM OBJECTIVE", 15, Text, true);
            _objectiveTitle.style.unityTextAlign = TextAnchor.MiddleRight;
            _objectiveDetail = NewLabel(string.Empty, 11, Muted, false);
            _objectiveDetail.style.unityTextAlign = TextAnchor.MiddleRight;
            _objectiveDetail.style.marginTop = 4f;
            _objectiveProgress = CreateBar(string.Empty, new Color(0.18f, 0.75f, 1f), 9f);
            _objectiveProgress.root.style.marginTop = 7f;
            _mapLabel = NewLabel(string.Empty, 10, Muted, true);
            _mapLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            _mapLabel.style.marginTop = 6f;
            _objectiveCard.Add(_objectiveTitle);
            _objectiveCard.Add(_objectiveDetail);
            _objectiveCard.Add(_objectiveProgress.root);
            _objectiveCard.Add(_mapLabel);
        }

        private void BuildAnnouncements()
        {
            _announcementCard = PanelElement(760f, 58f);
            _announcementCard.style.top = 112f;
            _announcementCard.style.display = DisplayStyle.None;
            _announcementCard.style.alignItems = Align.Center;
            _announcement = NewLabel(string.Empty, 18, Text, true);
            _announcement.style.unityTextAlign = TextAnchor.MiddleCenter;
            _announcementCard.Add(_announcement);
            _root.Add(_announcementCard);

            _tutorialCard = PanelElement(650f, 56f);
            _tutorialCard.style.bottom = 142f;
            _tutorialCard.style.display = DisplayStyle.None;
            _tutorialCard.style.alignItems = Align.Center;
            _tutorialPrompt = NewLabel(string.Empty, 15, new Color(0.45f, 1f, 0.72f), true);
            _tutorialPrompt.style.unityTextAlign = TextAnchor.MiddleCenter;
            _tutorialCard.Add(_tutorialPrompt);
            _root.Add(_tutorialCard);
        }

        private void BuildSpellTray()
        {
            VisualElement tray = new VisualElement();
            _spellTray = tray;
            tray.name = "Spell Tray";
            tray.pickingMode = PickingMode.Ignore;
            tray.style.position = Position.Absolute;
            tray.style.left = 0f;
            tray.style.right = 0f;
            tray.style.bottom = 22f;
            tray.style.height = 102f;
            tray.style.flexDirection = FlexDirection.Row;
            tray.style.justifyContent = Justify.Center;
            tray.style.alignItems = Align.FlexEnd;
            _root.Add(tray);

            string[] keys = { "LMB", "RMB", "Q" };
            for (int i = 0; i < 3; i++)
            {
                SpellView view = new SpellView();
                view.root = PanelElement(276f, 88f);
                view.root.style.position = Position.Relative;
                view.root.style.marginLeft = 7f;
                view.root.style.marginRight = 7f;
                view.root.style.paddingLeft = 18f;
                view.root.style.paddingTop = 11f;
                view.accent = new VisualElement();
                view.accent.pickingMode = PickingMode.Ignore;
                view.accent.style.position = Position.Absolute;
                view.accent.style.left = 0f;
                view.accent.style.top = 0f;
                view.accent.style.bottom = 0f;
                view.accent.style.width = 5f;
                view.key = NewLabel(keys[i], 11, Muted, true);
                view.name = NewLabel("EMPTY SLOT", 17, Text, true);
                view.details = NewLabel("Find a Spell Core", 11, Muted, false);
                view.cooldown = NewLabel(string.Empty, 22, Color.white, true);
                view.cooldown.style.position = Position.Absolute;
                view.cooldown.style.right = 13f;
                view.cooldown.style.top = 26f;
                view.root.Add(view.accent);
                view.root.Add(view.key);
                view.root.Add(view.name);
                view.root.Add(view.details);
                view.root.Add(view.cooldown);
                tray.Add(view.root);
                _spells[i] = view;
            }
        }

        private void BuildBossCard()
        {
            _bossCard = PanelElement(720f, 74f);
            _bossCard.style.left = 600f;
            _bossCard.style.top = 22f;
            _bossCard.style.display = DisplayStyle.None;
            _root.Add(_bossCard);
            _bossName = NewLabel("BOSS", 16, Text, true);
            _bossName.style.unityTextAlign = TextAnchor.MiddleCenter;
            _bossHealth = CreateBar(string.Empty, new Color(0.9f, 0.06f, 0.16f), 22f);
            _bossCard.Add(_bossName);
            _bossCard.Add(_bossHealth.root);
        }

        private void BuildCrosshair()
        {
            _crosshair = new VisualElement();
            _crosshair.name = "Aim Crosshair";
            _crosshair.pickingMode = PickingMode.Ignore;
            _crosshair.style.position = Position.Absolute;
            _crosshair.style.width = 30f;
            _crosshair.style.height = 30f;
            _crosshair.style.borderLeftWidth = 2f;
            _crosshair.style.borderRightWidth = 2f;
            _crosshair.style.borderTopWidth = 2f;
            _crosshair.style.borderBottomWidth = 2f;
            _crosshair.style.borderLeftColor = new Color(0.25f, 0.95f, 1f, 0.95f);
            _crosshair.style.borderRightColor = new Color(0.25f, 0.95f, 1f, 0.95f);
            _crosshair.style.borderTopColor = new Color(0.25f, 0.95f, 1f, 0.95f);
            _crosshair.style.borderBottomColor = new Color(0.25f, 0.95f, 1f, 0.95f);
            _crosshair.style.borderTopLeftRadius = 15f;
            _crosshair.style.borderTopRightRadius = 15f;
            _crosshair.style.borderBottomLeftRadius = 15f;
            _crosshair.style.borderBottomRightRadius = 15f;
            VisualElement dot = new VisualElement();
            dot.pickingMode = PickingMode.Ignore;
            dot.style.position = Position.Absolute;
            dot.style.left = 12f;
            dot.style.top = 12f;
            dot.style.width = 6f;
            dot.style.height = 6f;
            dot.style.backgroundColor = Color.white;
            dot.style.borderTopLeftRadius = 3f;
            dot.style.borderTopRightRadius = 3f;
            dot.style.borderBottomLeftRadius = 3f;
            dot.style.borderBottomRightRadius = 3f;
            _crosshair.Add(dot);
            _root.Add(_crosshair);
        }

        private void BuildInteractionCard()
        {
            _interactionCard = PanelElement(520f, 82f);
            _interactionCard.style.bottom = 132f;
            _interactionCard.style.display = DisplayStyle.None;
            _interactionCard.style.alignItems = Align.Center;
            _root.Add(_interactionCard);
            _interactionTitle = NewLabel("E  INTERACT", 16, Text, true);
            _interactionTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
            _interactionDescription = NewLabel(string.Empty, 11, Muted, false);
            _interactionDescription.style.unityTextAlign = TextAnchor.MiddleCenter;
            _interactionDescription.style.marginTop = 5f;
            _interactionCard.Add(_interactionTitle);
            _interactionCard.Add(_interactionDescription);
        }

        private void Update()
        {
            GameWorld world = GameWorld.Instance;
            bool runVisible = world != null && world.RunActive;
            if (_root == null) return;
            _root.style.display = runVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (!runVisible) { UnityEngine.Cursor.visible = true; return; }

            bool aiming = !world.ModalOpen;
            AccessibilitySettings settings = ProfileManager.Current.accessibility;
            _panelSettings.scale = settings.hudScale * settings.uiScale;
            _root.style.opacity = settings.hudOpacity;
            float safeMargin = (1f - settings.safeZone) * 180f;
            _playerCard.style.left = 24f + safeMargin; _playerCard.style.top = 24f + safeMargin;
            _runCard.style.right = 24f + safeMargin; _runCard.style.top = 24f + safeMargin;
            _objectiveCard.style.right = 24f + safeMargin; _objectiveCard.style.top = 148f + safeMargin;
            _spellTray.style.bottom = 22f + safeMargin;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            _crosshair.style.display = DisplayStyle.None;
            float cursorSize = 30f * settings.cursorScale;
            _crosshair.style.width = cursorSize; _crosshair.style.height = cursorSize;
            Color cursorColor = settings.highContrastCursor ? new Color(1f, 0.95f, 0.05f) : new Color(0.25f, 0.95f, 1f);
            _crosshair.style.borderLeftColor = cursorColor; _crosshair.style.borderRightColor = cursorColor;
            _crosshair.style.borderTopColor = cursorColor; _crosshair.style.borderBottomColor = cursorColor;
            if (Time.unscaledTime >= _nextHudRefresh)
            {
                _nextHudRefresh = Time.unscaledTime + 0.05f;
                UpdatePlayer(world);
                UpdateRun(world);
                UpdateSpells(world);
                UpdateBoss(world);
                UpdateObjective(world);
                UpdateInteraction();
            }
            SynchronizeEnemyViews(world);
            UpdateDamageViews();
            if (Time.unscaledTime >= _nextOffscreenRefresh)
            {
                _nextOffscreenRefresh = Time.unscaledTime + 0.12f;
                UpdateOffscreenIndicators(world);
            }
        }

        private void LateUpdate()
        {
            if (_root == null || _root.resolvedStyle.display == DisplayStyle.None) return;
            Camera camera = Camera.main;
            if (camera == null) return;
            _bossCard.style.left = Mathf.Max(12f, (RootWidth - 720f) * 0.5f);
            _interactionCard.style.left = Mathf.Max(12f, (RootWidth - 520f) * 0.5f);
            _announcementCard.style.left = Mathf.Max(12f, (RootWidth - 760f) * 0.5f);
            _tutorialCard.style.left = Mathf.Max(12f, (RootWidth - 650f) * 0.5f);
            _offscreenIndicator.style.left = Mathf.Max(12f, (RootWidth - 360f) * 0.5f);
            foreach (DamageView view in _damageViews)
            {
                float age = Time.unscaledTime - view.created;
                float drift = ProfileManager.Current.accessibility.reducedMotion ? 0.15f : 0.85f;
                PositionAtWorld(view.label, view.position + Vector3.up * age * drift, camera, view.label.resolvedStyle.width, -10f);
                view.label.style.opacity = 1f - Mathf.Clamp01(age / 0.95f);
            }
            AccessibilitySettings settings = ProfileManager.Current.accessibility;
            foreach (KeyValuePair<EnemyController, EnemyView> pair in _enemyViews)
            {
                EnemyController enemy = pair.Key;
                EnemyView view = pair.Value;
                if (enemy == null || enemy.IsDead) { view.root.style.display = DisplayStyle.None; continue; }
                float scale = settings.enemyHealthBarScale * (enemy.IsEliteOrBoss ? 1.12f : 1f);
                view.root.style.width = view.baseWidth * scale;
                PositionAtWorld(view.root, enemy.HealthBarWorldPosition + Vector3.up * settings.enemyHealthBarVerticalOffset,
                    camera, view.baseWidth * scale, -16f);
            }
        }

        private void PositionCrosshair()
        {
            float scaleX = RootWidth / Mathf.Max(1f, Screen.width);
            float scaleY = RootHeight / Mathf.Max(1f, Screen.height);
            Vector3 mouse = ArcaneInput.MousePosition;
            float half = 15f * ProfileManager.Current.accessibility.cursorScale;
            _crosshair.style.left = mouse.x * scaleX - half;
            _crosshair.style.top = (Screen.height - mouse.y) * scaleY - half;
        }

        private void UpdatePlayer(GameWorld world)
        {
            PlayerController player = world.Player;
            PlayerStats stats = world.Stats;
            if (player == null || stats == null) return;
            _health.Set(player.Health / Mathf.Max(1f, stats.maxHealth), "HEALTH   " + Mathf.CeilToInt(player.Health) + " / " + Mathf.CeilToInt(stats.maxHealth));
            _mana.Set(player.Mana / Mathf.Max(1f, stats.maxMana), "MANA   " + Mathf.CeilToInt(player.Mana) + " / " + Mathf.CeilToInt(stats.maxMana));
            _shield.root.style.display = player.Ward > 0.01f ? DisplayStyle.Flex : DisplayStyle.None;
            _shield.Set(player.Ward / Mathf.Max(1f, stats.maxHealth), "SHIELD   " + Mathf.CeilToInt(player.Ward));
            ControlSettings controls = ProfileManager.Current.controls;
            _controls.text = ArcaneInput.GamepadActive
                ? "L MOVE  ·  R AIM  ·  A INTERACT / DODGE  ·  RT / LT / RB SPELLS  ·  B BACK  ·  DODGE " +
                  (player.DodgeCooldownRemaining <= 0f ? "READY" : player.DodgeCooldownRemaining.ToString("0.0")) +
                  (string.IsNullOrEmpty(player.ConditionSummary) ? string.Empty : "  ·  " + player.ConditionSummary)
                : "WASD MOVE  ·  MOUSE AIM  ·  " + controls.interact.ToString().ToUpperInvariant() + " INTERACT  ·  " +
                  controls.workshop.ToString().ToUpperInvariant() + " SPELLCRAFT  ·  " + controls.spellLinks.ToString().ToUpperInvariant() + " LINKS  ·  " + controls.inventory.ToString().ToUpperInvariant() + " GEAR  ·  " +
                  controls.map.ToString().ToUpperInvariant() + " MAP  ·  DODGE " + (player.DodgeCooldownRemaining <= 0f ? "READY" : player.DodgeCooldownRemaining.ToString("0.0")) +
                  (string.IsNullOrEmpty(player.ConditionSummary) ? string.Empty : "  ·  " + player.ConditionSummary);
        }

        private void UpdateRun(GameWorld world)
        {
            RunDirector run = world.GetComponent<RunDirector>();
            if (run == null) return;
            _roomTitle.text = world.TrainingMode ? "TRAINING ROOM" : run.CurrentRoom == null ? "DUNGEON" : run.CurrentRoom.displayName.ToUpperInvariant();
            _runDetails.text = world.TrainingMode ? "Build testing · no rewards at risk" :
                run.RunMode.ToString().ToUpperInvariant() + "  ·  " + V12DifficultyContract.Name(run.Difficulty) + " THREAT " + V12DifficultyContract.ThreatRating(run.Difficulty) + "  ·  SEED " + run.CurrentSeed + "\nRUN LEVEL " + run.RunLevel + "  ·  XP " + run.RunExperience + " / " + run.ExperienceToNextLevel +
                "     ROOM " + (run.RoomIndex + 1) + (run.RunMode == DemoRunMode.Endless ? string.Empty : " / " + run.TotalRooms) + "     KILLS " + run.Kills + "     GOLD " + run.Gold +
                "     FORGE " + run.ForgeDust + "D / " + run.BindingRunes + "R / " + run.CorruptionCores + "C" +
                (run.Difficulty.timedRooms && run.EncounterActive ? "     TIME " + Mathf.CeilToInt(run.TimedRoomRemaining) : string.Empty);
        }

        private void UpdateObjective(GameWorld world)
        {
            DemoV05Director director = DemoV05Director.Instance;
            RunDirector run = world.GetComponent<RunDirector>();
            bool show = director != null && run != null && !world.TrainingMode && run.EncounterActive;
            _objectiveCard.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            if (show)
            {
                _objectiveTitle.text = director.Objective.Title;
                _objectiveDetail.text = director.Objective.Detail;
                _objectiveProgress.Set(director.Objective.Progress, string.Empty);
                _mapLabel.text = director.Map.Compact;
            }

            bool announce = director != null && Time.unscaledTime < director.AnnouncementUntil;
            _announcementCard.style.display = announce ? DisplayStyle.Flex : DisplayStyle.None;
            if (announce) _announcement.text = director.Announcement;

            TutorialDirector tutorial = TutorialDirector.Instance;
            bool contextual = V1GameDirector.Instance != null && Time.unscaledTime < V1GameDirector.Instance.ContextHintUntil && !string.IsNullOrEmpty(V1GameDirector.Instance.ContextHint);
            bool tutorialVisible = (tutorial != null && tutorial.Active) || contextual;
            _tutorialCard.style.display = tutorialVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (tutorialVisible) _tutorialPrompt.text = tutorial != null && tutorial.Active ? tutorial.Prompt : V1GameDirector.Instance.ContextHint;
        }

        private void UpdateSpells(GameWorld world)
        {
            for (int i = 0; i < _spells.Length; i++)
            {
                SpellView view = _spells[i];
                CompiledSpell spell = world.GetSpell((SpellSlot)i);
                if (spell == null)
                {
                    view.name.text = "EMPTY SLOT";
                    view.details.text = "Find a Spell Core";
                    view.accent.style.backgroundColor = new Color(0.22f, 0.28f, 0.35f);
                    view.cooldown.text = string.Empty;
                    continue;
                }
                view.name.text = spell.displayName.ToUpperInvariant();
                view.details.text = Mathf.RoundToInt(spell.damage) + " " + FriendlyElement(spell.element) + "  ·  " + Mathf.RoundToInt(spell.manaCost) + " Mana  ·  " + spell.delivery;
                view.accent.style.backgroundColor = spell.primaryColor;
                float cooldown = world.Player.GetCooldownRemaining((SpellSlot)i);
                view.cooldown.text = cooldown > 0.05f ? cooldown.ToString("0.0") : string.Empty;
                view.root.style.opacity = cooldown > 0.05f ? 0.62f : 1f;
            }
        }

        private void SynchronizeEnemyViews(GameWorld world)
        {
            AccessibilitySettings visibility = ProfileManager.Current == null ? null : ProfileManager.Current.accessibility;
            int mode = visibility == null ? 0 : Mathf.Clamp(visibility.enemyHealthBarMode, 0, 4);
            if (visibility != null && !visibility.alwaysShowEnemyHealth && mode == 0) mode = 1;
            EnemyController targeted = world.Player == null ? null : world.NearestEnemy(world.Player.AimPoint, 3f);
            bool rebuild = Time.unscaledTime >= _nextEnemyStructureSync;
            EnemyController boss = null;
            if (rebuild)
            {
                _nextEnemyStructureSync = Time.unscaledTime + 0.1f;
                _liveEnemies.Clear();
                for (int i = 0; i < world.Enemies.Count; i++)
                {
                    EnemyController enemy = world.Enemies[i];
                    if (enemy == null || enemy.IsDead) continue;
                    if (enemy.IsBoss) { boss = enemy; continue; }
                    bool visible = mode == 0 || (mode == 1 && enemy.Health < enemy.MaxHealth) ||
                        (mode == 2 && enemy == targeted) || (mode == 3 && enemy.IsEliteOrBoss);
                    if (mode != 4 && visible) _liveEnemies.Add(enemy);
                }
                _staleEnemies.Clear();
                foreach (KeyValuePair<EnemyController, EnemyView> pair in _enemyViews)
                    if (pair.Key == null || !_liveEnemies.Contains(pair.Key)) _staleEnemies.Add(pair.Key);
                for (int i = 0; i < _staleEnemies.Count; i++)
                {
                    EnemyController stale = _staleEnemies[i];
                    EnemyView staleView;
                    if (!_enemyViews.TryGetValue(stale, out staleView)) continue;
                    staleView.root.RemoveFromHierarchy();
                    _enemyViews.Remove(stale);
                    _enemyViewPool.Push(staleView);
                }
                foreach (EnemyController enemy in _liveEnemies)
                {
                    if (_enemyViews.ContainsKey(enemy)) continue;
                    EnemyView view = _enemyViewPool.Count > 0 ? _enemyViewPool.Pop() : CreateEnemyView(enemy);
                    ConfigureEnemyView(view, enemy);
                    _enemyViews[enemy] = view;
                    _enemyLayer.Add(view.root);
                }
            }

            foreach (KeyValuePair<EnemyController, EnemyView> pair in _enemyViews)
            {
                EnemyController enemy = pair.Key;
                if (enemy == null || enemy.IsDead) continue;
                EnemyView view = pair.Value;
                float currentRatio = enemy.HealthRatio;
                if (currentRatio > view.displayedRatio) view.displayedRatio = currentRatio;
                else view.displayedRatio = Mathf.MoveTowards(view.displayedRatio, currentRatio, Time.unscaledDeltaTime * 0.32f);
                view.delayed.style.width = Length.Percent(view.displayedRatio * 100f);
                view.health.style.width = Length.Percent(enemy.HealthRatio * 100f);
                view.shield.style.width = Length.Percent(enemy.ShieldRatio * 100f);
                view.shield.style.display = enemy.ShieldRatio > 0f ? DisplayStyle.Flex : DisplayStyle.None;
                view.armor.style.width = Length.Percent(enemy.ArmorRatio * 100f);
                view.armor.style.display = enemy.ArmorRatio > 0f ? DisplayStyle.Flex : DisplayStyle.None;
                view.name.text = enemy.IsEliteOrBoss ? enemy.DisplayName.ToUpperInvariant() : string.Empty;
                string values = ProfileManager.Current.accessibility.showEnemyHealthNumbers
                    ? Mathf.CeilToInt(enemy.Health) + "/" + Mathf.CeilToInt(enemy.MaxHealth) + " · " : string.Empty;
                string status = enemy.StatusSummary;
                if (ProfileManager.Current.accessibility.colorblindSymbols) status = StatusSymbols(status);
                view.status.text = values + enemy.CombatRole + (string.IsNullOrEmpty(status) ? string.Empty : " · " + status);
            }

            if (boss == null)
                for (int i = 0; i < world.Enemies.Count; i++)
                    if (world.Enemies[i] != null && !world.Enemies[i].IsDead && world.Enemies[i].IsBoss) { boss = world.Enemies[i]; break; }
            _bossCard.style.display = boss == null ? DisplayStyle.None : DisplayStyle.Flex;
            if (boss != null)
            {
                _bossName.text = boss.DisplayName.ToUpperInvariant();
                _bossHealth.Set(boss.HealthRatio, Mathf.CeilToInt(boss.Health) + " / " + Mathf.CeilToInt(boss.MaxHealth));
            }
        }

        private void UpdateBoss(GameWorld world)
        {
            EnemyController boss = null;
            for (int i = 0; i < world.Enemies.Count; i++)
            {
                EnemyController candidate = world.Enemies[i];
                if (candidate != null && !candidate.IsDead && candidate.IsBoss) { boss = candidate; break; }
            }
            _bossCard.style.display = boss == null ? DisplayStyle.None : DisplayStyle.Flex;
            if (boss == null) return;
            BossEncounterMechanics mechanics = boss.GetComponent<BossEncounterMechanics>();
            _bossName.text = boss.DisplayName.ToUpperInvariant() + (mechanics == null ? string.Empty : "  ·  PHASE " + mechanics.Phase + "  ·  " + mechanics.PhaseRule);
            _bossHealth.Set(boss.HealthRatio, Mathf.CeilToInt(boss.Health) + " / " + Mathf.CeilToInt(boss.MaxHealth));
        }

        private void UpdateInteraction()
        {
            WorldRoomInteractable interaction = WorldInteractionController.Instance == null ? null : WorldInteractionController.Instance.Current;
            _interactionCard.style.display = interaction == null ? DisplayStyle.None : DisplayStyle.Flex;
            if (interaction == null) return;
            string key = ProfileManager.Current == null ? "E" : ProfileManager.Current.controls.interact.ToString().ToUpperInvariant();
            _interactionTitle.text = key + "  ·  " + interaction.PromptAction + "  ·  " + interaction.PromptTitle;
            _interactionDescription.text = interaction.PromptDescription;
        }

        private void UpdateOffscreenIndicators(GameWorld world)
        {
            if (!ProfileManager.Current.accessibility.showOffscreenIndicators || Camera.main == null)
            {
                _offscreenIndicator.style.display = DisplayStyle.None;
                return;
            }
            Vector2 direction = Vector2.zero;
            int count = 0;
            foreach (EnemyController enemy in world.Enemies)
            {
                if (enemy == null || enemy.IsDead || enemy.IsBoss) continue;
                Vector3 viewport = Camera.main.WorldToViewportPoint(enemy.transform.position);
                if (viewport.z > 0f && viewport.x >= 0.04f && viewport.x <= 0.96f && viewport.y >= 0.05f && viewport.y <= 0.95f) continue;
                direction += new Vector2(viewport.x - 0.5f, viewport.y - 0.5f).normalized;
                count++;
            }
            _offscreenIndicator.style.display = count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            if (count == 0) return;
            string arrow = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? (direction.x < 0f ? "←" : "→") : (direction.y < 0f ? "↓" : "↑");
            _offscreenIndicator.text = arrow + "  " + count + " ENEMY" + (count == 1 ? string.Empty : "IES") + " OFF SCREEN  " + arrow;
        }

        private EnemyView CreateEnemyView(EnemyController enemy)
        {
            EnemyView view = new EnemyView();
            view.root = new VisualElement();
            view.root.pickingMode = PickingMode.Ignore;
            view.root.style.position = Position.Absolute;
            view.name = NewLabel(string.Empty, 10, Text, true);
            view.name.style.unityTextAlign = TextAnchor.MiddleCenter;
            VisualElement background = new VisualElement();
            background.pickingMode = PickingMode.Ignore;
            background.style.height = enemy.IsEliteOrBoss ? 9f : 7f;
            background.style.backgroundColor = new Color(0.02f, 0.025f, 0.04f, 0.94f);
            background.style.borderLeftWidth = 1f;
            background.style.borderRightWidth = 1f;
            background.style.borderTopWidth = 1f;
            background.style.borderBottomWidth = 1f;
            SetBorderColor(background, new Color(0.55f, 0.65f, 0.75f, 0.7f));
            view.health = new VisualElement();
            view.health.pickingMode = PickingMode.Ignore;
            view.health.style.height = Length.Percent(100f);
            view.delayed = new VisualElement();
            view.delayed.pickingMode = PickingMode.Ignore;
            view.delayed.style.position = Position.Absolute;
            view.delayed.style.left = 0f;
            view.delayed.style.top = 0f;
            view.delayed.style.height = Length.Percent(100f);
            view.delayed.style.width = Length.Percent(100f);
            view.delayed.style.backgroundColor = new Color(1f, 0.75f, 0.18f, 0.72f);
            view.shield = new VisualElement();
            view.shield.pickingMode = PickingMode.Ignore;
            view.shield.style.height = 3f;
            view.shield.style.backgroundColor = new Color(0.15f, 0.75f, 1f);
            view.armor = new VisualElement();
            view.armor.pickingMode = PickingMode.Ignore;
            view.armor.style.height = 3f;
            view.armor.style.backgroundColor = new Color(0.82f, 0.7f, 0.42f);
            view.status = NewLabel(string.Empty, 8, new Color(0.75f, 0.88f, 1f), true);
            view.status.style.unityTextAlign = TextAnchor.MiddleCenter;
            view.root.Add(view.name);
            background.Add(view.delayed);
            background.Add(view.health);
            view.root.Add(background);
            view.root.Add(view.shield);
            view.root.Add(view.armor);
            view.root.Add(view.status);
            ConfigureEnemyView(view, enemy);
            return view;
        }

        private static void ConfigureEnemyView(EnemyView view, EnemyController enemy)
        {
            float width = enemy.IsEliteOrBoss ? 126f : 92f;
            view.baseWidth = width;
            view.displayedRatio = enemy.HealthRatio;
            view.root.style.width = width;
            view.root.style.height = enemy.IsEliteOrBoss ? 39f : 24f;
            view.root.style.display = DisplayStyle.Flex;
            view.name.text = string.Empty;
            view.name.style.height = enemy.IsEliteOrBoss ? 14f : 0f;
            view.status.text = string.Empty;
            view.delayed.style.width = Length.Percent(100f);
            view.health.style.width = Length.Percent(100f);
            view.health.style.backgroundColor = enemy.IsEliteOrBoss ? new Color(1f, 0.28f, 0.12f) : new Color(0.9f, 0.12f, 0.18f);
            view.shield.style.display = DisplayStyle.None;
            view.armor.style.display = DisplayStyle.None;
        }

        private static string StatusSymbols(string source)
        {
            if (string.IsNullOrEmpty(source)) return source;
            return source.Replace("POISON", "◆ POISON").Replace("BURNING", "▲ BURN")
                .Replace("SHOCKED", "ϟ SHOCK").Replace("CHILLED", "◇ CHILL")
                .Replace("FROZEN", "✦ FROZEN").Replace("STAGGERED", "✕ STAGGER")
                .Replace("ARMOR", "▣ ARMOR").Replace("SHIELD", "⬡ SHIELD")
                .Replace("BLEED", "▼ BLEED").Replace("CURSE", "☾ CURSE")
                .Replace("WEAKENED", "▽ WEAK").Replace("VULNERABLE", "◎ VULNERABLE");
        }

        private void UpdateDamageViews()
        {
            for (int i = _damageViews.Count - 1; i >= 0; i--)
                if (Time.unscaledTime - _damageViews[i].created > 0.95f) RemoveDamage(i);
        }

        private void RemoveDamage(int index)
        {
            _damageViews[index].label.RemoveFromHierarchy();
            _damageViews.RemoveAt(index);
            if (V1PerformanceBudget.Instance != null) V1PerformanceBudget.Instance.Release(RuntimeEntityKind.DamageNumber);
        }

        private void PositionAtWorld(VisualElement element, Vector3 worldPosition, Camera camera, float width, float verticalOffset)
        {
            Vector3 viewport = camera.WorldToViewportPoint(worldPosition);
            if (viewport.z <= 0f || viewport.x < -0.04f || viewport.x > 1.04f || viewport.y < -0.04f || viewport.y > 1.04f)
            {
                element.style.display = DisplayStyle.None;
                return;
            }
            Vector3 screen = camera.WorldToScreenPoint(worldPosition);
            element.style.display = DisplayStyle.Flex;
            float scaleX = RootWidth / Mathf.Max(1f, Screen.width);
            float scaleY = RootHeight / Mathf.Max(1f, Screen.height);
            element.style.left = screen.x * scaleX - width * 0.5f;
            element.style.top = (Screen.height - screen.y) * scaleY + verticalOffset;
        }

        private float RootWidth
        {
            get
            {
                float width = _root.resolvedStyle.width;
                return float.IsNaN(width) || width <= 0f ? Mathf.Max(1f, Screen.width) : width;
            }
        }

        private float RootHeight
        {
            get
            {
                float height = _root.resolvedStyle.height;
                return float.IsNaN(height) || height <= 0f ? Mathf.Max(1f, Screen.height) : height;
            }
        }

        private static string FriendlyElement(SpellElement element)
        {
            return element == SpellElement.Frost ? "Cold" : element == SpellElement.Toxic ? "Poison" : element.ToString();
        }

        private static VisualElement FullscreenLayer(string name)
        {
            VisualElement layer = new VisualElement();
            layer.name = name;
            layer.pickingMode = PickingMode.Ignore;
            layer.style.position = Position.Absolute;
            layer.style.left = 0f;
            layer.style.top = 0f;
            layer.style.right = 0f;
            layer.style.bottom = 0f;
            return layer;
        }

        private static VisualElement PanelElement(float width, float height)
        {
            VisualElement panel = new VisualElement();
            panel.pickingMode = PickingMode.Ignore;
            panel.style.position = Position.Absolute;
            panel.style.width = width;
            panel.style.height = height;
            panel.style.paddingLeft = 15f;
            panel.style.paddingRight = 15f;
            panel.style.paddingTop = 12f;
            panel.style.paddingBottom = 10f;
            panel.style.backgroundColor = Panel;
            panel.style.borderLeftWidth = 1f;
            panel.style.borderRightWidth = 1f;
            panel.style.borderTopWidth = 1f;
            panel.style.borderBottomWidth = 1f;
            SetBorderColor(panel, Border);
            panel.style.borderTopLeftRadius = 8f;
            panel.style.borderTopRightRadius = 8f;
            panel.style.borderBottomLeftRadius = 8f;
            panel.style.borderBottomRightRadius = 8f;
            return panel;
        }

        private static BarView CreateBar(string label, Color color, float height)
        {
            BarView view = new BarView();
            view.root = new VisualElement();
            view.root.pickingMode = PickingMode.Ignore;
            view.root.style.position = Position.Relative;
            view.root.style.height = height;
            view.root.style.marginTop = 5f;
            view.root.style.backgroundColor = PanelLight;
            view.root.style.borderTopLeftRadius = 3f;
            view.root.style.borderTopRightRadius = 3f;
            view.root.style.borderBottomLeftRadius = 3f;
            view.root.style.borderBottomRightRadius = 3f;
            view.fill = new VisualElement();
            view.fill.pickingMode = PickingMode.Ignore;
            view.fill.style.position = Position.Absolute;
            view.fill.style.left = 0f;
            view.fill.style.top = 0f;
            view.fill.style.bottom = 0f;
            view.fill.style.width = Length.Percent(100f);
            view.fill.style.backgroundColor = color;
            view.text = NewLabel(label, 11, Color.white, true);
            view.text.style.position = Position.Absolute;
            view.text.style.left = 8f;
            view.text.style.right = 8f;
            view.text.style.top = 0f;
            view.text.style.bottom = 0f;
            view.text.style.unityTextAlign = TextAnchor.MiddleLeft;
            view.root.Add(view.fill);
            view.root.Add(view.text);
            return view;
        }

        private static Label NewLabel(string value, int size, Color color, bool bold)
        {
            Label label = new Label(value);
            label.pickingMode = PickingMode.Ignore;
            label.style.color = color;
            label.style.fontSize = size;
            label.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
            label.style.whiteSpace = WhiteSpace.NoWrap;
            return label;
        }

        private static void SetBorderColor(VisualElement element, Color color)
        {
            element.style.borderLeftColor = color;
            element.style.borderRightColor = color;
            element.style.borderTopColor = color;
            element.style.borderBottomColor = color;
        }
    }
}
