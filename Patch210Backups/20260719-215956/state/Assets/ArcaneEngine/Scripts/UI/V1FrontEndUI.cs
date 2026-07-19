using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneEngine
{
    public sealed class V1TitleScreen : MonoBehaviour
    {
        public static V1TitleScreen Instance { get; private set; }
        public static bool IsOpen { get { return Instance != null && Instance._titleOpen; } }
        public static bool IsMapOpen { get { return Instance != null && Instance._mapOpen; } }

        private UIDocument _document;
        private VisualElement _root;
        private VisualElement _title;
        private VisualElement _titleContent;
        private VisualElement _map;
        private VisualElement _mapNodes;
        private Label _mapHeader;
        private Label _message;
        private Label _saveStatus;
        private string _profileImportPath = string.Empty;
        private string _profileDeleteConfirmation = string.Empty;
        private string _profileToolMessage = string.Empty;
        private V21BindingAction? _captureBinding;
        private V21BindingAction? _conflictingBinding;
        private KeyCode _pendingBindingKey;
        private bool _titleOpen;
        private bool _mapOpen;
        private float _nextMapRefresh;
        private float _confirmNewUntil;
        private bool _mainMenuVisible;

        private static readonly Color Background = new Color(0.006f, 0.009f, 0.02f, 0.97f);
        private static readonly Color Panel = new Color(0.025f, 0.04f, 0.072f, 0.98f);
        private static readonly Color Card = new Color(0.055f, 0.085f, 0.13f, 1f);
        private static readonly Color Accent = new Color(0.16f, 0.78f, 0.9f, 1f);
        private static readonly Color Gold = new Color(1f, 0.72f, 0.24f, 1f);
        private static readonly Color Text = new Color(0.92f, 0.96f, 1f, 1f);
        private static readonly Color Muted = new Color(0.58f, 0.7f, 0.8f, 1f);

        private void Awake()
        {
            Instance = this;
            _document = RuntimeUIFactory.CreateDocument(transform, "UI Document · Title and Map", 100);
            Build();
            ShowTitle();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (_titleOpen || _mapOpen)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
            }
            GameWorld world = GameWorld.Instance;
            if (_titleOpen && _mainMenuVisible)
            {
                if (ArcaneInput.GetKeyDown(KeyCode.Return) || ArcaneInput.GetKeyDown(KeyCode.KeypadEnter) || ArcaneInput.GetKeyDown(KeyCode.Alpha1)) StartPreparation();
                else if (ArcaneInput.GetKeyDown(KeyCode.Alpha2) || ArcaneInput.GetKeyDown(KeyCode.Escape)) EnterHome();
            }
            if (!_titleOpen && world != null && world.RunActive && ArcaneInput.GetKeyDown(ProfileManager.Current.controls.map)) ToggleMap();
            if (_mapOpen && ArcaneInput.GetKeyDown(KeyCode.Escape)) HideMap();
            if (_mapOpen && Time.unscaledTime >= _nextMapRefresh)
            {
                _nextMapRefresh = Time.unscaledTime + 0.35f;
                RefreshMap();
            }
            if (_mapOpen)
            {
                float width = _root.resolvedStyle.width; if (float.IsNaN(width) || width <= 0f) width = Screen.width;
                float height = _root.resolvedStyle.height; if (float.IsNaN(height) || height <= 0f) height = Screen.height;
                _map.style.left = Mathf.Max(12f, (width - 900f) * 0.5f);
                _map.style.top = Mathf.Max(12f, (height - 850f) * 0.5f);
            }
            if (_saveStatus != null) _saveStatus.text = "SAVE · " + ProfileManager.LastSaveStatus;
        }

        public void ShowTitle()
        {
            _titleOpen = true;
            _mapOpen = false;
            _root.style.display = DisplayStyle.Flex;
            _title.style.display = DisplayStyle.Flex;
            _map.style.display = DisplayStyle.None;
            BuildMainMenu();
            Time.timeScale = 1f;
        }

        private void OnGUI()
        {
            if (!_titleOpen || Event.current == null || Event.current.type != EventType.KeyDown) return;
            if (_captureBinding.HasValue)
            {
                KeyCode capturedKey = Event.current.keyCode;
                if (capturedKey == KeyCode.Escape) { _captureBinding = null; _conflictingBinding = null; BuildOptions(); Event.current.Use(); return; }
                if (capturedKey != KeyCode.None)
                {
                    V21BindingAction? conflict;
                    string message;
                    V21BindingService.TryAssign(_captureBinding.Value, capturedKey, V21BindingConflictPolicy.Cancel, out conflict, out message);
                    _pendingBindingKey = capturedKey;
                    _conflictingBinding = conflict;
                    if (!conflict.HasValue) _captureBinding = null;
                    BuildOptions();
                    Event.current.Use();
                    return;
                }
            }
            if (!_mainMenuVisible) return;
            KeyCode key = Event.current.keyCode;
            if (key == KeyCode.Return || key == KeyCode.KeypadEnter || key == KeyCode.Alpha1)
            {
                Event.current.Use();
                StartPreparation();
            }
            else if (key == KeyCode.Alpha2 || key == KeyCode.Escape)
            {
                Event.current.Use();
                EnterHome();
            }
        }

        public void ToggleMap()
        {
            if (_titleOpen) return;
            _mapOpen = !_mapOpen;
            _root.style.display = _mapOpen ? DisplayStyle.Flex : DisplayStyle.None;
            _title.style.display = DisplayStyle.None;
            _map.style.display = _mapOpen ? DisplayStyle.Flex : DisplayStyle.None;
            Time.timeScale = _mapOpen ? 0f : 1f;
            if (_mapOpen) RefreshMap();
        }

        public void HideMap()
        {
            _mapOpen = false;
            _root.style.display = DisplayStyle.None;
            _map.style.display = DisplayStyle.None;
            if (GameWorld.Instance != null && GameWorld.Instance.RunActive) Time.timeScale = 1f;
        }

        private void Build()
        {
            _root = _document.rootVisualElement;
            _root.name = "Arcane Engine Front End";
            _root.style.position = Position.Absolute;
            _root.style.left = 0f; _root.style.top = 0f; _root.style.right = 0f; _root.style.bottom = 0f;
            _root.style.backgroundColor = Background;

            _title = new VisualElement();
            _title.style.position = Position.Absolute;
            _title.style.left = 0f; _title.style.top = 0f; _title.style.right = 0f; _title.style.bottom = 0f;
            _title.style.alignItems = Align.Center;
            _title.style.justifyContent = Justify.Center;
            _titleContent = new VisualElement();
            _titleContent.style.width = 780f;
            _titleContent.style.minHeight = 680f;
            _titleContent.style.paddingLeft = 48f; _titleContent.style.paddingRight = 48f;
            _titleContent.style.paddingTop = 38f; _titleContent.style.paddingBottom = 34f;
            _titleContent.style.backgroundColor = Panel;
            Border(_titleContent, Accent, 2f); Radius(_titleContent, 14f);
            _title.Add(_titleContent);
            _root.Add(_title);

            _map = new VisualElement();
            _map.style.position = Position.Absolute;
            _map.style.width = 900f; _map.style.height = 850f;
            _map.style.paddingLeft = 34f; _map.style.paddingRight = 34f;
            _map.style.paddingTop = 28f; _map.style.paddingBottom = 28f;
            _map.style.backgroundColor = Panel;
            Border(_map, Accent, 2f); Radius(_map, 14f);
            _mapHeader = Label("DUNGEON MAP", 28, Text, true);
            _map.Add(_mapHeader);
            Label legend = Label("◆ Cleared   ◇ Current   ? Future route   ·   Press M or Escape to close", 13, Muted, false);
            legend.style.marginTop = 6f; legend.style.marginBottom = 18f;
            _map.Add(legend);
            ScrollView mapScroll = new ScrollView(ScrollViewMode.Vertical);
            mapScroll.style.flexGrow = 1f;
            _mapNodes = mapScroll.contentContainer;
            _map.Add(mapScroll);
            _root.Add(_map);
            _map.style.display = DisplayStyle.None;
        }

        private void BuildMainMenu()
        {
            _mainMenuVisible = true;
            _titleContent.Clear();
            Label eyebrow = Label("THE RELIC FORGE", 14, Gold, true);
            eyebrow.style.letterSpacing = 4f;
            _titleContent.Add(eyebrow);
            Label title = Label("ARCANE ENGINE", 52, Text, true);
            title.style.marginTop = 2f;
            _titleContent.Add(title);
            Label subtitle = Label("1.0 DEMO  ·  Build impossible spells. Survive what they awaken.", 16, Muted, false);
            subtitle.style.marginBottom = 30f;
            _titleContent.Add(subtitle);

            Button continueButton = MenuButton("CONTINUE SAVED RUN", ContinueRun, true);
            continueButton.SetEnabled(ProfileManager.HasRunSnapshot);
            _titleContent.Add(continueButton);
            _titleContent.Add(MenuButton("NEW EXPEDITION", StartPreparation, false));
            _titleContent.Add(MenuButton("ENTER HOME BASE", EnterHome, false));
            _titleContent.Add(MenuButton("PROFILES", BuildProfiles, false));
            _titleContent.Add(MenuButton("OPTIONS & ACCESSIBILITY", BuildOptions, false));
            _titleContent.Add(MenuButton("CREDITS", BuildCredits, false));
            _titleContent.Add(MenuButton("QUIT", () => Application.Quit(), false));

            _message = Label(ProfileManager.HasRunSnapshot ? "A valid room-entry checkpoint is available." : "No saved run. Your permanent profile is safe.", 13, Muted, false);
            _message.style.marginTop = 22f;
            _titleContent.Add(_message);
            _saveStatus = Label("SAVE · " + ProfileManager.LastSaveStatus, 11, Muted, false);
            _saveStatus.style.marginTop = 7f;
            _titleContent.Add(_saveStatus);
            Label shortcuts = Label("ENTER / 1  NEW EXPEDITION    ·    2 / ESCAPE  HOME BASE", 11, Accent, true);
            shortcuts.style.marginTop = 10f;
            _titleContent.Add(shortcuts);
            Label version = Label("VERSION 2.1.0-DEMO  ·  PROMISE COMPLETION UPDATE  ·  LINUX x86_64", 10, new Color(0.4f, 0.5f, 0.6f), true);
            version.style.marginTop = 12f;
            _titleContent.Add(version);
        }

        private void BuildProfiles()
        {
            _mainMenuVisible = false;
            _titleContent.Clear();
            _titleContent.Add(Label("PROFILES", 34, Text, true));
            _titleContent.Add(Paragraph("Three independent profiles are supported. Each profile has separate unlocks, options, histories, backups, and a single room-entry run checkpoint."));
            for (int index = 0; index < 3; index++)
            {
                int captured = index;
                Button profile = MenuButton((ProfileManager.ActiveIndex == index ? "◆ " : "") + "PROFILE " + (index + 1), () =>
                {
                    ProfileManager.SwitchProfile(captured);
                    GameWorld world = GameWorld.Instance;
                    if (world != null)
                    {
                        world.Equipment.LoadFromProfile(false);
                        world.RecalculateStats(false);
                        if (HomeBaseController.Instance != null) HomeBaseController.Instance.Rebuild();
                    }
                    BuildProfiles();
                }, ProfileManager.ActiveIndex == index);
                _titleContent.Add(profile);
            }
            _titleContent.Add(Label("PROFILE TOOLS", 18, Accent, true));
            if (!string.IsNullOrEmpty(_profileToolMessage)) _titleContent.Add(Paragraph(_profileToolMessage));
            _titleContent.Add(MenuButton("EXPORT ACTIVE PROFILE", () =>
            {
                string path;
                string message;
                ProfileManager.ExportCurrentProfile(null, out path, out message);
                _profileToolMessage = message;
                BuildProfiles();
            }, false));
            int duplicateTarget = (ProfileManager.ActiveIndex + 1) % 3;
            _titleContent.Add(MenuButton("DUPLICATE ACTIVE PROFILE TO SLOT " + (duplicateTarget + 1), () =>
            {
                string message;
                ProfileManager.DuplicateProfile(ProfileManager.ActiveIndex, duplicateTarget, out message);
                _profileToolMessage = message;
                BuildProfiles();
            }, false));
            string[] backups = ProfileManager.AvailableBackups(ProfileManager.ActiveIndex);
            if (backups.Length > 0)
            {
                string newest = backups[0];
                _titleContent.Add(MenuButton("RESTORE NEWEST BACKUP · " + System.IO.File.GetLastWriteTime(newest).ToString("g"), () =>
                {
                    string message;
                    ProfileManager.RestoreBackup(ProfileManager.ActiveIndex, newest, out message);
                    _profileToolMessage = message;
                    BuildProfiles();
                }, false));
            }
            TextField import = new TextField("IMPORT FILE PATH");
            import.value = _profileImportPath;
            import.RegisterValueChangedCallback(evt => _profileImportPath = evt.newValue ?? string.Empty);
            _titleContent.Add(import);
            _titleContent.Add(MenuButton("VALIDATE & IMPORT INTO ACTIVE SLOT", () =>
            {
                string message;
                ProfileManager.ImportProfile(_profileImportPath, ProfileManager.ActiveIndex, out message);
                _profileToolMessage = message;
                BuildProfiles();
            }, false));
            TextField delete = new TextField("TYPE DELETE PROFILE " + (ProfileManager.ActiveIndex + 1));
            delete.value = _profileDeleteConfirmation;
            delete.RegisterValueChangedCallback(evt => _profileDeleteConfirmation = evt.newValue ?? string.Empty);
            _titleContent.Add(delete);
            _titleContent.Add(MenuButton("DELETE ACTIVE PROFILE", () =>
            {
                string message;
                if (ProfileManager.DeleteProfileWithConfirmation(ProfileManager.ActiveIndex, _profileDeleteConfirmation, out message))
                    _profileDeleteConfirmation = string.Empty;
                _profileToolMessage = message;
                BuildProfiles();
            }, false));
            _titleContent.Add(MenuButton("BACK", BuildMainMenu, false));
        }

        private void BuildOptions()
        {
            _mainMenuVisible = false;
            _titleContent.Clear();
            _titleContent.Add(Label("OPTIONS & ACCESSIBILITY", 34, Text, true));
            ScrollView scroll = new ScrollView(ScrollViewMode.Vertical);
            scroll.style.flexGrow = 1f;
            _titleContent.Add(scroll);
            AccessibilitySettings settings = ProfileManager.Current.accessibility;
            AddSlider(scroll, "MASTER VOLUME", 0f, 1f, settings.masterVolume, value => settings.masterVolume = value);
            AddSlider(scroll, "MUSIC VOLUME", 0f, 1f, settings.musicVolume, value => settings.musicVolume = value);
            AddSlider(scroll, "COMBAT EFFECTS", 0f, 1f, settings.effectsVolume, value => settings.effectsVolume = value);
            AddSlider(scroll, "AMBIENCE VOLUME", 0f, 1f, settings.ambienceVolume, value => settings.ambienceVolume = value);
            AddSlider(scroll, "UI VOLUME", 0f, 1f, settings.uiVolume, value => settings.uiVolume = value);
            AddSlider(scroll, "ENEMY VOLUME", 0f, 1f, settings.enemyVolume, value => settings.enemyVolume = value);
            AddSlider(scroll, "VOICE VOLUME", 0f, 1f, settings.voiceVolume, value => settings.voiceVolume = value);
            AddSlider(scroll, "TEXT SIZE (SMALL / STANDARD / LARGE)", 0.85f, 1.3f, settings.uiScale, value => settings.uiScale = value);
            AddSlider(scroll, "HUD SCALE", 0.75f, 1.4f, settings.hudScale, value => settings.hudScale = value);
            AddSlider(scroll, "HUD OPACITY", 0.35f, 1f, settings.hudOpacity, value => settings.hudOpacity = value);
            AddSlider(scroll, "HUD SAFE ZONE", 0.8f, 1f, settings.safeZone, value => settings.safeZone = value);
            AddSlider(scroll, "TOOLTIP SCALE", 0.8f, 1.5f, settings.tooltipScale, value => settings.tooltipScale = value);
            AddSlider(scroll, "CURSOR SCALE", 0.7f, 1.8f, settings.cursorScale, value => settings.cursorScale = value);
            AddSlider(scroll, "DAMAGE NUMBER SCALE", 0.7f, 1.6f, settings.damageNumberScale, value => settings.damageNumberScale = value);
            AddSlider(scroll, "ENEMY HEALTH BAR SCALE", 0.6f, 1.8f, settings.enemyHealthBarScale, value => settings.enemyHealthBarScale = value);
            AddSlider(scroll, "ENEMY HEALTH BAR HEIGHT", -0.75f, 1.5f, settings.enemyHealthBarVerticalOffset, value => settings.enemyHealthBarVerticalOffset = value);
            AddSlider(scroll, "ENEMY BARS (ALWAYS / DAMAGED / TARGETED / ELITE / OFF)", 0f, 4f, settings.enemyHealthBarMode, value => settings.enemyHealthBarMode = Mathf.RoundToInt(value));
            AddSlider(scroll, "DRAG START DISTANCE", 3f, 24f, settings.dragThreshold, value => settings.dragThreshold = value);
            AddSlider(scroll, "SCREEN SHAKE", 0f, 1f, settings.screenShake, value => settings.screenShake = value);
            AddSlider(scroll, "EFFECT DENSITY", 0.25f, 1f, settings.effectDensity, value => settings.effectDensity = value);
            AddSlider(scroll, "VISUAL PRESET (LOW / MEDIUM / HIGH)", 0f, 2f, settings.visualQuality, value => VisualQualityPolicy.ApplyPreset(settings, Mathf.RoundToInt(value)));
            AddSlider(scroll, "SPELL VISUAL DENSITY", 0.25f, 1f, settings.spellEffectDensity, value => settings.spellEffectDensity = value);
            AddSlider(scroll, "ENVIRONMENT DENSITY", 0.2f, 1f, settings.environmentDensity, value => settings.environmentDensity = value);
            AddSlider(scroll, "DYNAMIC LIGHTS (OFF / LIMITED / HIGH)", 0f, 2f, settings.dynamicLightQuality, value => settings.dynamicLightQuality = Mathf.RoundToInt(value));
            AddSlider(scroll, "SHADOWS (OFF / HARD / SOFT)", 0f, 2f, settings.shadowQuality, value => settings.shadowQuality = Mathf.RoundToInt(value));
            AddSlider(scroll, "PERSISTENT MARK DURATION", 0.25f, 2f, settings.decalDuration, value => settings.decalDuration = value);
            AddSlider(scroll, "HIT STOP", 0f, 1f, settings.hitStop, value => settings.hitStop = value);
            AddSlider(scroll, "DAMAGE NUMBER DENSITY", 0.25f, 1f, settings.damageNumberDensity, value => settings.damageNumberDensity = value);
            AddToggle(scroll, "REDUCED MOTION", settings.reducedMotion, value => settings.reducedMotion = value);
            AddToggle(scroll, "REDUCED FLASHES", settings.reducedFlashes, value => settings.reducedFlashes = value);
            AddToggle(scroll, "HIGH-CONTRAST TELEGRAPHS", settings.highContrastTelegraphs, value => settings.highContrastTelegraphs = value);
            AddToggle(scroll, "HIGH-CONTRAST CURSOR", settings.highContrastCursor, value => settings.highContrastCursor = value);
            AddToggle(scroll, "COLORBLIND CONNECTORS", settings.colorblindConnectors, value => settings.colorblindConnectors = value);
            AddToggle(scroll, "COLORBLIND ELEMENT, RARITY & STATUS SYMBOLS", settings.colorblindSymbols, value => settings.colorblindSymbols = value);
            AddToggle(scroll, "VISUAL DIAGNOSTICS (F10)", settings.showVisualDiagnostics, value => settings.showVisualDiagnostics = value);
            AddToggle(scroll, "CLICK-TO-PLACE ALTERNATIVE", settings.clickPlacementAlternative, value => settings.clickPlacementAlternative = value);
            AddToggle(scroll, "AIM ASSIST WITH LEFT SHIFT", settings.autoAim, value => settings.autoAim = value);
            AddToggle(scroll, "ALWAYS SHOW ENEMY HEALTH", settings.alwaysShowEnemyHealth, value => settings.alwaysShowEnemyHealth = value);
            AddToggle(scroll, "ENEMY HEALTH NUMBERS", settings.showEnemyHealthNumbers, value => settings.showEnemyHealthNumbers = value);
            AddToggle(scroll, "DAMAGE NUMBERS", settings.showDamageNumbers, value => settings.showDamageNumbers = value);
            AddToggle(scroll, "VISUAL AUDIO CUES", settings.visualAudioCues, value => settings.visualAudioCues = value);
            AddToggle(scroll, "MONO AUDIO", settings.monoAudio, value => settings.monoAudio = value);
            AddToggle(scroll, "WORLD-RELATIVE MOVEMENT", settings.worldRelativeMovement, value => settings.worldRelativeMovement = value);
            AddToggle(scroll, "CAMERA ROTATION", settings.cameraRotationEnabled, value => settings.cameraRotationEnabled = value);
            AddSlider(scroll, "CAMERA MODE (IMMEDIATE / SMOOTH / FIXED)", 0f, 2f, settings.cameraMode, value => settings.cameraMode = Mathf.RoundToInt(value));
            AddToggle(scroll, "CAMERA OBSTRUCTION FADE", settings.cameraObstructionFade, value => settings.cameraObstructionFade = value);
            AddToggle(scroll, "PAUSE FOR TUTORIAL TEXT", settings.pauseForTutorialText, value => settings.pauseForTutorialText = value);
            AddToggle(scroll, "SIMPLIFIED DESCRIPTIONS", settings.simplifiedDescriptions, value => settings.simplifiedDescriptions = value);
            ControlSettings controls = ProfileManager.Current.controls;
            AddToggle(scroll, "HOLD TO CHARGE (OFF USES PRESS-TO-CAST)", controls.holdToCharge, value => controls.holdToCharge = value);
            AddSlider(scroll, "CONTROLLER MOVE DEAD ZONE", 0.05f, 0.5f, controls.controllerMoveDeadZone, value => controls.controllerMoveDeadZone = value);
            AddSlider(scroll, "CONTROLLER AIM DEAD ZONE", 0.05f, 0.5f, controls.controllerAimDeadZone, value => controls.controllerAimDeadZone = value);
            AddSlider(scroll, "CONTROLLER AIM RESPONSE", 0.25f, 2.5f, controls.controllerAimSensitivity, value => controls.controllerAimSensitivity = value);
            AddSlider(scroll, "CONTROLLER AIM ASSIST", 0f, 1f, controls.controllerAimAssist, value => controls.controllerAimAssist = value);
            AddSlider(scroll, "CONTROLLER VIBRATION", 0f, 1f, controls.controllerVibration, value => controls.controllerVibration = value);
            AddToggle(scroll, "CONTROLLER VIBRATION ENABLED", controls.controllerVibrationEnabled, value => controls.controllerVibrationEnabled = value);
            scroll.Add(Label("KEYBOARD & MOUSE BINDINGS", 18, Accent, true));
            foreach (V21BindingAction action in V21BindingService.All)
            {
                V21BindingAction captured = action;
                scroll.Add(MenuButton(V21BindingService.Friendly(action).ToUpperInvariant() + "  ·  " +
                    (_captureBinding == action ? "PRESS A KEY…" : V21BindingService.Get(action).ToString().ToUpperInvariant()), () =>
                {
                    _captureBinding = captured;
                    _conflictingBinding = null;
                    BuildOptions();
                }, false));
            }
            if (_captureBinding.HasValue && _conflictingBinding.HasValue)
            {
                scroll.Add(Paragraph(_pendingBindingKey + " is already bound to " + V21BindingService.Friendly(_conflictingBinding.Value) + "."));
                scroll.Add(MenuButton("SWAP BINDINGS", () => ResolveBinding(V21BindingConflictPolicy.Swap), true));
                scroll.Add(MenuButton("REPLACE EXISTING BINDING", () => ResolveBinding(V21BindingConflictPolicy.Replace), false));
                scroll.Add(MenuButton("CANCEL REBIND", () => { _captureBinding = null; _conflictingBinding = null; BuildOptions(); }, false));
            }
            scroll.Add(MenuButton("RESET ALL BINDINGS", () => { V21BindingService.ResetDefaults(); BuildOptions(); }, false));
            scroll.Add(Label("ACCESSIBILITY PREVIEW", 18, Accent, true));
            scroll.Add(Paragraph("STATUS SYMBOLS  ● BURN   ❄ CHILL   ⚡ SHOCK   ☠ TOXIC\nFOCUS INDICATOR  cyan border (yellow in high-contrast mode)\nAUDIO CAPTIONS  ♪ meaningful cues appear as HUD notices when Visual Audio Cues is enabled."));
            scroll.Add(MenuButton("RESET ACCESSIBILITY OPTIONS", () =>
            {
                ProfileManager.Current.accessibility = new AccessibilitySettings();
                BuildOptions();
            }, false));
            _titleContent.Add(MenuButton("SAVE & BACK", () => { ProfileManager.Save(); BuildMainMenu(); }, true));
        }

        private void ResolveBinding(V21BindingConflictPolicy policy)
        {
            if (!_captureBinding.HasValue) return;
            V21BindingAction? conflict;
            string message;
            V21BindingService.TryAssign(_captureBinding.Value, _pendingBindingKey, policy, out conflict, out message);
            _captureBinding = null;
            _conflictingBinding = null;
            BuildOptions();
        }

        private void BuildCredits()
        {
            _mainMenuVisible = false;
            _titleContent.Clear();
            _titleContent.Add(Label("CREDITS", 34, Text, true));
            _titleContent.Add(Paragraph("ARCANE ENGINE — THE RELIC FORGE\n\nGame direction and playtesting: Kaey\nDesign, engineering, systems, and prototype presentation: collaborative production with Codex\nBuilt with Unity 6\n\nVersion 2.1 is a Promise Completion source candidate. Runtime acceptance, hardware checks, visual review and performance evidence remain recorded separately from implementation claims."));
            _titleContent.Add(MenuButton("BACK", BuildMainMenu, false));
        }

        private void ContinueRun()
        {
            RunDirector run = GameWorld.Instance == null ? null : GameWorld.Instance.GetComponent<RunDirector>();
            string result = null;
            if (run == null || !run.ContinueSavedRun(out result))
            {
                _message.text = result ?? "The saved run could not be loaded.";
                return;
            }
            CloseTitle();
        }

        private void StartPreparation()
        {
            if (ProfileManager.HasRunSnapshot && Time.unscaledTime > _confirmNewUntil)
            {
                _confirmNewUntil = Time.unscaledTime + 5f;
                _message.text = "A saved run exists. Choose NEW EXPEDITION again within five seconds to replace it.";
                return;
            }
            RunDirector run = GameWorld.Instance == null ? null : GameWorld.Instance.GetComponent<RunDirector>();
            if (run == null)
            {
                _message.text = "The run service is still starting. Wait one moment and try again.";
                return;
            }
            CloseTitle();
            RunStartScreen.Show(run);
        }

        private void EnterHome() { CloseTitle(); }

        private void CloseTitle()
        {
            _titleOpen = false;
            _root.style.display = DisplayStyle.None;
            _title.style.display = DisplayStyle.None;
            Time.timeScale = 1f;
        }

        private void RefreshMap()
        {
            _mapNodes.Clear();
            RunDirector run = GameWorld.Instance == null ? null : GameWorld.Instance.GetComponent<RunDirector>();
            if (run == null) return;
            _mapHeader.text = "DUNGEON MAP  ·  SEED " + run.CurrentSeed + "  ·  DEPTH " + (run.RoomIndex + 1);
            DungeonMapState state = DemoV05Director.Instance == null ? null : DemoV05Director.Instance.Map;
            if (state != null)
            {
                foreach (DungeonMapState.Node node in state.Nodes)
                {
                    Color color = node.cleared ? new Color(0.35f, 0.9f, 0.65f) : Gold;
                    string marker = node.cleared ? "◆" : "◇";
                    _mapNodes.Add(MapCard(marker + "  DEPTH " + (node.depth + 1) + "  ·  " + node.name.ToUpperInvariant(), node.type.ToString().ToUpperInvariant(), color));
                }
            }
            if (run.PendingRoute && run.RouteChoices.Count > 0)
            {
                _mapNodes.Add(Label("AVAILABLE ROUTES", 14, Muted, true));
                foreach (RoomTemplate route in run.RouteChoices)
                    _mapNodes.Add(MapCard("?  " + route.displayName.ToUpperInvariant(), RoomIcon(route.type) + "  " + route.type.ToString().ToUpperInvariant(), Accent));
            }
        }

        private static VisualElement MapCard(string title, string detail, Color color)
        {
            VisualElement card = new VisualElement();
            card.style.minHeight = 66f; card.style.marginBottom = 8f;
            card.style.paddingLeft = 16f; card.style.paddingRight = 16f;
            card.style.paddingTop = 10f; card.style.paddingBottom = 8f;
            card.style.backgroundColor = Card; Border(card, color, 1f); Radius(card, 7f);
            card.Add(Label(title, 16, Text, true));
            card.Add(Label(detail, 11, color, true));
            return card;
        }

        private static string RoomIcon(DungeonRoomType type)
        {
            if (type == DungeonRoomType.Shop) return "¤";
            if (type == DungeonRoomType.Boss || type == DungeonRoomType.Miniboss) return "☠";
            if (type == DungeonRoomType.HealingSanctuary) return "+";
            if (type == DungeonRoomType.EquipmentReward) return "▣";
            if (type == DungeonRoomType.SpellCoreReward || type == DungeonRoomType.ModifierReward) return "✦";
            return "⚔";
        }

        private static Button MenuButton(string text, Action action, bool primary)
        {
            Button button = RuntimeUIFactory.CreateButton(action);
            button.text = text;
            button.style.height = 51f; button.style.marginBottom = 8f;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            float scale = ProfileManager.Current == null ? 1f : ProfileManager.Current.accessibility.uiScale;
            button.style.fontSize = 15f * scale; button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.style.paddingLeft = 18f;
            button.style.color = Text;
            button.style.backgroundColor = primary ? new Color(0.08f, 0.42f, 0.52f) : Card;
            Border(button, primary ? Accent : new Color(0.16f, 0.3f, 0.42f), 1f); Radius(button, 6f);
            return button;
        }

        private static Label Label(string text, int size, Color color, bool bold)
        {
            Label label = new Label(text);
            float scale = ProfileManager.Current == null ? 1f : ProfileManager.Current.accessibility.uiScale;
            label.style.fontSize = size * scale; label.style.color = color;
            label.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        private static Label Paragraph(string text)
        {
            Label label = Label(text, 15, Muted, false);
            label.style.marginTop = 14f; label.style.marginBottom = 25f;
            return label;
        }

        private static void AddSlider(VisualElement parent, string title, float minimum, float maximum, float value, Action<float> changed)
        {
            Slider slider = new Slider(title, minimum, maximum) { value = value };
            slider.style.height = 42f; slider.style.color = Text;
            slider.RegisterValueChangedCallback(evt => changed(evt.newValue));
            parent.Add(slider);
        }

        private static void AddToggle(VisualElement parent, string title, bool value, Action<bool> changed)
        {
            Toggle toggle = new Toggle(title) { value = value };
            toggle.style.height = 34f; toggle.style.color = Text;
            toggle.RegisterValueChangedCallback(evt => changed(evt.newValue));
            parent.Add(toggle);
        }

        private static void Border(VisualElement element, Color color, float width)
        {
            element.style.borderLeftWidth = width; element.style.borderRightWidth = width;
            element.style.borderTopWidth = width; element.style.borderBottomWidth = width;
            element.style.borderLeftColor = color; element.style.borderRightColor = color;
            element.style.borderTopColor = color; element.style.borderBottomColor = color;
        }

        private static void Radius(VisualElement element, float radius)
        {
            element.style.borderTopLeftRadius = radius; element.style.borderTopRightRadius = radius;
            element.style.borderBottomLeftRadius = radius; element.style.borderBottomRightRadius = radius;
        }
    }
}
