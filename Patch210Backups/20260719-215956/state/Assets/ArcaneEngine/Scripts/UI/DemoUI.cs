using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class DemoUI : MonoBehaviour
    {
        private enum SanctuaryTab { Expedition, Preparation, Archives, Armory, Forge, Upgrades, Codex, Training }

        public static DemoUI Instance { get; private set; }
        public bool ModalOpen
        {
            get
            {
                return _workshopOpen || _spellLinksOpen || _inventoryOpen || _helpOpen || _pauseOpen || _sanctuaryVisible || RunStartScreen.IsOpen || V1TitleScreen.IsOpen || V1TitleScreen.IsMapOpen;
            }
        }
        public bool IsPaused { get { return _pauseOpen; } }
        public bool WorkshopOpen { get { return _workshopOpen || (V21ProductUI.Instance != null && V21ProductUI.Instance.WorkshopOpen); } }
        public bool InventoryOpen { get { return _inventoryOpen || (V21ProductUI.Instance != null && V21ProductUI.Instance.InventoryOpen); } }

        private bool _workshopOpen;
        private bool _spellLinksOpen;
        private bool _inventoryOpen;
        private bool _helpOpen;
        private bool _pauseOpen;
        private bool _confirmReset;
        private bool _confirmRespec;
        private bool _expertDetails;
        private SpellSlot _workshopSlot = SpellSlot.Slot1;
        private string _selectedModifier = "homing";
        private int _rotation;
        private bool _draggingModifier;
        private PlacedModifier _draggedPlacedModifier;
        private bool _dragDropHandled;
        private int _itemSortMode;
        private int _stashSlotFilter = -1;
        private int _stashMinimumRarity;
        private int _stashSpecialFilter;
        private string _itemSearch = string.Empty;
        private ItemInstance _selectedRunItem;
        private string _pendingSalvageInstanceId;
        private ItemInstance _selectedStashItem;
        private ForgeAction? _pendingForgeAction;
        private int _leftPointerReleaseFrame = -100;
        private string _message = "Home Base is ready.";
        private SanctuaryTab _sanctuaryTab;
        private Vector2 _mainScroll;
        private Vector2 _secondaryScroll;
        private Vector2 _tertiaryScroll;
        private string _search = string.Empty;
        private string _layoutName = "My Spell";
        private int _selectedProfile;
        private Texture2D _hexTexture;
        private Texture2D _circleTexture;
        private GUIStyle _title;
        private GUIStyle _header;
        private GUIStyle _body;
        private GUIStyle _small;
        private GUIStyle _center;
        private GUIStyle _button;
        private GUIStyle _panel;
        private GUIStyle _tab;
        private bool _stylesReady;
        private bool _sanctuaryVisible;
        private int _linkSourceSlot;
        private int _linkDestinationSlot = 1;
        private SpellLinkCondition _linkCondition = SpellLinkCondition.OnHit;
        private CompiledSpell _placementPreviewSpell;

        private void Awake() { Instance = this; }

        private void Update()
        {
            GameWorld world = GameWorld.Instance;
            if (world == null) return;
            ControlSettings controls = ProfileManager.Current.controls;
            if (world.RunActive && world.PendingSpellUpgrade && !WorkshopOpen) OpenWorkshopAnywhere();
            else if (world.RunActive && world.SpellLinks != null && world.SpellLinks.HasPendingReward &&
                !(_spellLinksOpen || (V21ProductUI.Instance != null && V21ProductUI.Instance.LinksOpen))) OpenSpellLinksAnywhere();
            if ((_workshopOpen || _inventoryOpen) && ArcaneInput.GetMouseButtonUp(0)) _leftPointerReleaseFrame = Time.frameCount;
            if (world.RunActive && ArcaneInput.GetKeyDown(controls.workshop))
            {
                bool closing = _workshopOpen;
                if (_inventoryOpen) SaveCurrentRunCheckpoint();
                _workshopOpen = !_workshopOpen; _spellLinksOpen = false; _inventoryOpen = false; _helpOpen = false; _pauseOpen = false; SyncPause();
                if (closing) { CancelTransientDrags(); SaveCurrentRunCheckpoint(); }
            }
            if (world.RunActive && ArcaneInput.GetKeyDown(controls.spellLinks))
            {
                _spellLinksOpen = !_spellLinksOpen; _workshopOpen = false; _inventoryOpen = false; _helpOpen = false; _pauseOpen = false; SyncPause();
            }
            if (world.RunActive && ArcaneInput.GetKeyDown(controls.inventory))
            {
                bool closing = _inventoryOpen;
                if (_workshopOpen) { CancelTransientDrags(); SaveCurrentRunCheckpoint(); }
                _inventoryOpen = !_inventoryOpen; _workshopOpen = false; _spellLinksOpen = false; _helpOpen = false; _pauseOpen = false; SyncPause();
                if (closing) SaveCurrentRunCheckpoint();
            }
            if (ArcaneInput.GetKeyDown(controls.help))
            {
                _helpOpen = !_helpOpen;
                if (_helpOpen) { _workshopOpen = false; _spellLinksOpen = false; _inventoryOpen = false; _pauseOpen = false; }
                SyncPause();
            }
            if (ArcaneInput.GetKeyDown(KeyCode.Escape))
            {
                if (!world.RunActive && _sanctuaryVisible) { _sanctuaryVisible = false; return; }
                if (_workshopOpen || _spellLinksOpen || _inventoryOpen || _helpOpen || _pauseOpen) CloseModals();
                else if (world.RunActive && !world.TrainingMode) { _pauseOpen = true; SyncPause(); }
            }
            if (_workshopOpen && world.CanEditSpells && ArcaneInput.GetKeyDown(KeyCode.Q)) _rotation = (_rotation + 5) % 6;
            if (_workshopOpen && world.CanEditSpells && ArcaneInput.GetKeyDown(KeyCode.E)) _rotation = (_rotation + 1) % 6;
            if (world.RunActive) Time.timeScale = ModalOpen ? 0f : 1f;
        }

        private void OnGUI()
        {
            EnsureStyles();
            GameWorld world = GameWorld.Instance;
            if (world == null || world.Stats == null) return;
            if (RunStartScreen.IsOpen) return;
            RunDirector run = world.GetComponent<RunDirector>();
            if (!world.RunActive)
            {
                if (_sanctuaryVisible) DrawSanctuary(world, run);
                else
                {
                    string interact = ProfileManager.Current.controls.interact.ToString().ToUpperInvariant();
                    GUI.Label(new Rect(22f, 18f, 900f, 34f), "HOME BASE · WASD to explore · move near a station and press " + interact, _header);
                    WorldRoomInteractable focused = WorldInteractionController.Instance == null ? null : WorldInteractionController.Instance.Current;
                    if (focused != null)
                    {
                        Rect prompt = new Rect(Mathf.Max(20f, Screen.width * 0.5f - 310f), Screen.height - 112f, 620f, 82f);
                        GUI.Box(prompt, GUIContent.none, _panel);
                        GUI.Label(new Rect(prompt.x + 16f, prompt.y + 10f, prompt.width - 32f, 24f), interact + " · " + focused.PromptAction + " · " + focused.PromptTitle, _header);
                        GUI.Label(new Rect(prompt.x + 16f, prompt.y + 38f, prompt.width - 32f, 36f), focused.PromptDescription, _small);
                    }
                }
                if (_helpOpen) DrawHelp(); return;
            }
            if (_workshopOpen) DrawWorkshop(world);
            else if (_spellLinksOpen) DrawSpellLinks(world);
            else if (_inventoryOpen) DrawRunInventory(world, run);
            else if (_helpOpen) DrawHelp();
            else if (_pauseOpen) DrawPause(run);
        }

        public void OpenWorkshopAnywhere()
        {
            if (V21ProductUI.Instance != null) { V21ProductUI.Instance.OpenWorkshop(); return; }
            _workshopOpen = true; _spellLinksOpen = false; _inventoryOpen = _helpOpen = _pauseOpen = false; SyncPause();
        }

        public void OpenSpellLinksAnywhere()
        {
            if (V21ProductUI.Instance != null) { V21ProductUI.Instance.OpenLinks(); return; }
            _spellLinksOpen = true; _workshopOpen = false; _inventoryOpen = _helpOpen = _pauseOpen = false; SyncPause();
        }

        public void OpenInventoryAnywhere()
        {
            if (V21ProductUI.Instance != null) { V21ProductUI.Instance.OpenInventory(); return; }
            _inventoryOpen = true; _workshopOpen = _spellLinksOpen = _helpOpen = _pauseOpen = false; SyncPause();
        }

        public void OpenHomeSection(int index)
        {
            _sanctuaryVisible = true;
            _sanctuaryTab = (SanctuaryTab)Mathf.Clamp(index, 0, 7);
            _mainScroll = _secondaryScroll = Vector2.zero;
        }

        public void ShowRunRecap()
        {
            _sanctuaryVisible = true;
            _sanctuaryTab = SanctuaryTab.Expedition;
            _mainScroll = _secondaryScroll = Vector2.zero;
        }

        private void DrawSanctuary(GameWorld world, RunDirector run)
        {
            Rect panel = new Rect(20f, 20f, Screen.width - 40f, Screen.height - 40f);
            GUI.Box(panel, GUIContent.none, _panel);
            GUI.Label(new Rect(panel.x + 24f, panel.y + 14f, 520f, 40f), "ARCANE ENGINE · HOME BASE", _title);
            GUI.Label(new Rect(panel.xMax - 720f, panel.y + 17f, 685f, 28f),
                ProfileManager.Current.profileName + " · " + ProfileManager.Current.essence + " Essence · " + ProfileManager.Current.relicShards + " Legendary Shards · " +
                ProfileManager.Current.forgeDust + " Dust · " + ProfileManager.Current.bindingRunes + " Runes · " + ProfileManager.Current.corruptionCores + " Cores", _header);

            string[] tabs = { "START RUN", "SPELL LOADOUT", "SPELL STORAGE", "EQUIPMENT", "FORGE", "UPGRADES", "COLLECTION", "OPTIONS" };
            float tabWidth = (panel.width - 48f) / tabs.Length;
            for (int i = 0; i < tabs.Length; i++)
            {
                Color previous = GUI.backgroundColor;
                GUI.backgroundColor = (int)_sanctuaryTab == i ? new Color(0.25f, 0.7f, 0.85f) : new Color(0.18f, 0.22f, 0.3f);
                if (GUI.Button(new Rect(panel.x + 24f + i * tabWidth, panel.y + 60f, tabWidth - 4f, 38f), tabs[i], _tab))
                {
                    _sanctuaryTab = (SanctuaryTab)i; _mainScroll = _secondaryScroll = Vector2.zero;
                }
                GUI.backgroundColor = previous;
            }

            Rect content = new Rect(panel.x + 24f, panel.y + 112f, panel.width - 48f, panel.height - 136f);
            switch (_sanctuaryTab)
            {
                case SanctuaryTab.Expedition: DrawExpedition(run, content); break;
                case SanctuaryTab.Preparation: DrawPreparation(content); break;
                case SanctuaryTab.Archives: DrawArchives(run, content); break;
                case SanctuaryTab.Armory: DrawSanctuaryArmory(world, content); break;
                case SanctuaryTab.Forge: DrawSanctuaryForge(world, content); break;
                case SanctuaryTab.Upgrades: DrawUpgrades(run, content); break;
                case SanctuaryTab.Codex: DrawCodex(content); break;
                case SanctuaryTab.Training: DrawTrainingAndOptions(world, content); break;
            }
        }

        private void DrawExpedition(RunDirector run, Rect area)
        {
            GUI.Box(area, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(area, 22f));
            GUILayout.Label("START A DUNGEON RUN", _title);
            GUILayout.Label("Choose routes, customize three independent spells, defeat the boss, and extract to secure equipment, Forge materials, and Spell Copies found during the run.", _body);
            GUILayout.Space(12f);
            GUILayout.Label("DIFFICULTY MODIFIERS · OPTIONAL", _header);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width((area.width - 70f) * 0.5f));
            run.Difficulty.frenziedEnemies = ContractToggle(run.Difficulty.frenziedEnemies, "Faster Enemies", "Enemies move 35% faster", "+20%");
            run.Difficulty.bulwarkEnemies = ContractToggle(run.Difficulty.bulwarkEnemies, "Tougher Enemies", "Enemies have 55% more Health", "+25%");
            run.Difficulty.glassSoul = ContractToggle(run.Difficulty.glassSoul, "Deadly Enemies", "Enemies deal 50% more damage", "+35%");
            run.Difficulty.manaDrought = ContractToggle(run.Difficulty.manaDrought, "Low Mana Recovery", "Mana regenerates slowly", "+20%");
            run.Difficulty.extraEliteAffixes = ContractToggle(run.Difficulty.extraEliteAffixes, "Extra Elite Modifiers", "Elites gain another modifier", "+25%");
            run.Difficulty.reducedHealing = ContractToggle(run.Difficulty.reducedHealing, "Reduced Healing", "Healing is 45% weaker", "+20%");
            run.Difficulty.newBossPhase = ContractToggle(run.Difficulty.newBossPhase, "Harder Boss", "Boss gains adds and harder phases", "+35%");
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width((area.width - 70f) * 0.5f));
            run.Difficulty.unstableWorld = ContractToggle(run.Difficulty.unstableWorld, "More Spell Overload", "Spell Overload is more dangerous", "+20%");
            run.Difficulty.adaptiveEnemies = ContractToggle(run.Difficulty.adaptiveEnemies, "Adaptive Resistance", "Repeated damage types build resistance", "+25%");
            run.Difficulty.cursedShops = ContractToggle(run.Difficulty.cursedShops, "Cursed Shops", "Cheaper but cursed shop stock", "+15%");
            run.Difficulty.timedRooms = ContractToggle(run.Difficulty.timedRooms, "Timed Rooms", "Combat rooms are timed", "+25%");
            run.Difficulty.reducedRerolls = ContractToggle(run.Difficulty.reducedRerolls, "Fewer Rerolls", "Lose one reward reroll", "+10%");
            run.Difficulty.noStartingEquipment = ContractToggle(run.Difficulty.noStartingEquipment, "No Starting Equipment", "Start with no gear", "+45%");
            run.Difficulty.noStartingModifiers = ContractToggle(run.Difficulty.noStartingModifiers, "No Starting Modifiers", "Start without modifiers", "+25%");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            GUILayout.Label("Reward multiplier x" + run.Difficulty.RewardMultiplier.ToString("0.00") + " · " + run.Difficulty.EnabledCount + " difficulty modifiers active", _header);
            if (run.HasRunResult)
            {
                RunStatistics stats = RunStatistics.Instance;
                GUILayout.Label((run.LastRunVictory ? "RUN COMPLETE" : "RUN ENDED") + " · " + run.RunMode.ToString().ToUpperInvariant() + " · seed " + run.CurrentSeed, _title);
                if (run.LastRunVictory) GUILayout.Label("EXTRACTION SUCCESSFUL · UNSECURED LOOT BANKED", _header);
                GUILayout.Label("Rooms cleared " + run.RoomsCleared + " · Run Level " + run.RunLevel + " · " + run.Kills + " kills · " + run.LastEssenceAward + " Essence kept", _body);
                GUILayout.Label((run.LastRunVictory ? "Secured " : "Lost ") + run.LastRunItemCount + " equipment item(s) · " + run.LastRunForgeDust + " Forge Dust · " +
                    run.LastRunBindingRunes + " Binding Runes · " + run.LastRunCorruptionCores + " Corruption Cores", _body);
                GUILayout.Label((run.LastRunVictory ? "Secured " : "Lost ") + run.LastRunSpellCopies + " Spell Copies · " +
                    (run.LastRunVictory ? "Lost " : "Lost ") + run.LastRunSupportRunes + " dungeon Support Runes · Lost " + run.LastRunGold + " Gold", _body);
                GUILayout.Label("Objective bonus experience earned: " + run.ObjectiveBonusExperience, _body);
                if (stats != null)
                    GUILayout.Label("Damage dealt " + Mathf.RoundToInt(stats.DamageDealt) + " · damage taken " + Mathf.RoundToInt(stats.DamageTaken) +
                        " · critical hits " + stats.CriticalHits + " · dodges " + stats.Dodges + " · top spell " + stats.BestSpell +
                        (stats.BossPhasesReached > 0 ? "\nBoss time " + stats.BossFightSeconds.ToString("0.0") + "s · pillars " + stats.BossPillarsDestroyed + " · phases " + stats.BossPhasesReached : string.Empty), _body);
                if (!run.LastRunVictory && GameWorld.Instance != null && GameWorld.Instance.Player != null)
                    GUILayout.Label("Final damage source: " + GameWorld.Instance.Player.LastDamageSource, _body);
                GUILayout.Label("ONE-CLICK PLAYTEST FEEDBACK", _header);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("TOO PUNISHING")) { ProfileManager.RecordFeedback(run.CurrentSeed, 2, "difficulty"); _message = "Feedback recorded with the run seed."; }
                if (GUILayout.Button("FAIR, NEEDS POLISH")) { ProfileManager.RecordFeedback(run.CurrentSeed, 3, "polish"); _message = "Feedback recorded with the run seed."; }
                if (GUILayout.Button("BUILD FELT GREAT")) { ProfileManager.RecordFeedback(run.CurrentSeed, 5, "buildcraft"); _message = "Feedback recorded with the run seed."; }
                GUILayout.EndHorizontal();
                GUILayout.Space(8f);
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("Starting loadout: " + ProfileManager.Current.StartingSpellSlots + " spell slot(s), " + CountPreparedModifiers() + " starting Support Rune(s), " +
                ProfileManager.Current.equippedItems.Count + "/10 equipment slots selected. Full equipment is always allowed.", _body);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("SELECT SPELLS & START", GUILayout.Height(56f))) { _helpOpen = false; RunStartScreen.Show(run); }
            if (ProfileManager.HasRunSnapshot && GUILayout.Button("CONTINUE SAVED RUN", GUILayout.Height(56f))) { string result; if (!run.ContinueSavedRun(out result)) _message = result; }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private bool ContractToggle(bool value, string name, string effect, string reward)
        {
            return GUILayout.Toggle(value, "  " + name + " — " + effect + "  (" + reward + ")", _button, GUILayout.Height(34f));
        }

        private void DrawPreparation(Rect area)
        {
            GUI.Box(area, GUIContent.none, _panel);
            Rect spells = new Rect(area.x + 16f, area.y + 16f, area.width * 0.46f, area.height - 32f);
            Rect modifiers = new Rect(spells.xMax + 14f, area.y + 16f, area.xMax - spells.xMax - 30f, area.height - 32f);
            GUI.Box(spells, GUIContent.none, _panel); GUI.Box(modifiers, GUIContent.none, _panel);

            GUILayout.BeginArea(Inset(spells, 16f));
            GUILayout.Label("STARTING SPELLS", _title);
            GUILayout.Label("Progression allows " + ProfileManager.Current.StartingSpellSlots + " starting spell slot(s). Empty slots can accept a Spell Core found during the run. Only one Legendary Spell may be active.", _body);
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            for (int slot = 0; slot < 3; slot++)
            {
                bool unlocked = slot < ProfileManager.Current.StartingSpellSlots;
                PreparedSpellSave selected = ProfileManager.Current.preparedSpells.FirstOrDefault(s => s.slotIndex == slot);
                GUILayout.Label("SPELL SLOT " + (slot + 1) + (unlocked ? "" : " · LOCKED UNTIL UPGRADED"), _header);
                if (!unlocked) { GUILayout.Label("This position begins empty, but dungeon cores can fill it.", _small); continue; }
                string current = selected == null ? "Empty" : selected.isRelic ? MegaCatalog.GetRelic(selected.contentId).displayName + " · LEGENDARY" : DemoCatalog.GetCore(selected.contentId).displayName;
                GUILayout.Label("Prepared: " + current, _body);
                foreach (string coreId in ProfileManager.Current.spellArchive.Select(c => c.coreId).Distinct())
                {
                    SpellCoreDefinition core = DemoCatalog.GetCore(coreId);
                    if (core != null && GUILayout.Button(core.displayName + " · Spell Core", GUILayout.Height(32f))) SetPreparedSpell(slot, core.id, false);
                }
                foreach (RelicSaveData saved in ProfileManager.Current.relicArchive)
                {
                    RelicDefinition relic = MegaCatalog.GetRelic(saved.relicId);
                    if (relic != null && GUILayout.Button(relic.displayName + " · LEGENDARY", GUILayout.Height(34f))) SetPreparedSpell(slot, relic.id, true);
                }
                if (GUILayout.Button("Leave slot empty", GUILayout.Height(28f)))
                {
                    ProfileManager.Current.preparedSpells.RemoveAll(s => s.slotIndex == slot); ProfileManager.Save();
                }
                GUILayout.Space(10f);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(Inset(modifiers, 16f));
            int spent = PreparationSpent();
            GUILayout.Label("STARTING SUPPORT RUNES", _title);
            GUILayout.Label("Loadout Points " + spent + "/" + ProfileManager.Current.PreparationBudget + ". Each Support Rune belongs to one Spell Board and cannot be shared.", _body);
            GUILayout.BeginHorizontal(); GUILayout.Label("Search", GUILayout.Width(55f)); _search = GUILayout.TextField(_search); GUILayout.EndHorizontal();
            _secondaryScroll = GUILayout.BeginScrollView(_secondaryScroll);
            foreach (SpellModifierDefinition modifier in DemoCatalog.AllModifiers.Where(m => ProfileManager.Current.unlockedModifierIds.Contains(m.id))
                .Where(m => string.IsNullOrEmpty(_search) || m.displayName.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0).OrderBy(m => m.category).ThenBy(m => m.displayName))
            {
                PreparedModifierSave selected = ProfileManager.Current.preparedModifiers.FirstOrDefault(m => m.modifierId == modifier.id);
                int count = selected == null ? 0 : selected.count;
                GUILayout.BeginHorizontal();
                Color previous = GUI.backgroundColor; GUI.backgroundColor = modifier.uiColor;
                if (GUILayout.Button(modifier.FullDisplayName + " ×" + count + "\nLoadout Cost " + modifier.preparationCost + " · Board Capacity " + modifier.capacityCost, GUILayout.Height(58f)))
                    AddPreparedModifier(modifier);
                GUI.backgroundColor = previous;
                if (GUILayout.Button("−", GUILayout.Width(34f), GUILayout.Height(50f))) RemovePreparedModifier(modifier.id);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.Label(_message, _small);
            GUILayout.EndArea();
        }

        private void DrawArchives(RunDirector run, Rect area)
        {
            GUI.Box(area, GUIContent.none, _panel);
            Rect archive = new Rect(area.x + 16f, area.y + 16f, area.width * 0.36f, area.height - 32f);
            Rect forge = new Rect(archive.xMax + 14f, area.y + 16f, area.xMax - archive.xMax - 30f, area.height - 32f);
            GUI.Box(archive, GUIContent.none, _panel); GUI.Box(forge, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(archive, 16f));
            GUILayout.Label("SPELL STORAGE", _title);
            GUILayout.Label("There is no storage cap. Spell Copies are consumed only when making a Legendary Spell; crafted Legendary Spells are permanent.", _body);
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            foreach (IGrouping<string, CoreSaveData> group in ProfileManager.Current.spellArchive.GroupBy(c => c.coreId).OrderBy(g => g.Key))
            {
                SpellCoreDefinition core = DemoCatalog.GetCore(group.Key);
                if (core != null) GUILayout.Label(core.displayName + " Spell Copy ×" + group.Count() + "\n" + core.description, _body);
            }
            GUILayout.Space(10f); GUILayout.Label("LEGENDARY SPELLS", _header);
            foreach (RelicSaveData saved in ProfileManager.Current.relicArchive)
            {
                RelicDefinition relic = MegaCatalog.GetRelic(saved.relicId);
                if (relic != null) GUILayout.Label(relic.displayName + "\n" + relic.signatureRule, _body);
            }
            GUILayout.EndScrollView(); GUILayout.EndArea();

            GUILayout.BeginArea(Inset(forge, 16f));
            GUILayout.Label("LEGENDARY UPGRADES", _title);
            GUILayout.Label("Requires a matching Spell Copy, Legendary Shards, discovery, and boss victories. Every Legendary Spell can still use the hex board.", _body);
            _secondaryScroll = GUILayout.BeginScrollView(_secondaryScroll);
            foreach (RelicDefinition relic in MegaCatalog.AllRelics.OrderBy(r => r.sourceCoreId).ThenBy(r => r.shardCost))
            {
                bool discovered = ProfileManager.Current.discoveredRelicIds.Contains(relic.id);
                CoreSaveData core = ProfileManager.Current.spellArchive.FirstOrDefault(c => c.coreId == relic.sourceCoreId);
                bool owned = ProfileManager.Current.relicArchive.Any(r => r.relicId == relic.id);
                Color previous = GUI.backgroundColor; GUI.backgroundColor = discovered ? relic.color : Color.gray;
                GUILayout.BeginVertical(_panel);
                GUILayout.Label((discovered ? relic.displayName : "UNDISCOVERED " + DemoCatalog.GetCore(relic.sourceCoreId).displayName + " BRANCH") + (owned ? " · OWNED" : string.Empty), _header);
                GUILayout.Label(discovered ? relic.description + "\n" + relic.signatureRule : "Defeat bosses, use difficulty modifiers, and explore hidden encounters to discover this upgrade.", _body);
                GUILayout.Label("Cost: " + relic.shardCost + " Legendary Shard(s) · requires " + relic.requiredBossKills + " boss victory/victories · consumes one " + DemoCatalog.GetCore(relic.sourceCoreId).displayName + " Spell Copy", _small);
                if (!owned && GUILayout.Button("FORGE " + relic.displayName, GUILayout.Height(32f)))
                {
                    string result; run.ForgeRelic(relic.id, core == null ? null : core.instanceId, out result); _message = result;
                }
                GUILayout.EndVertical(); GUI.backgroundColor = previous; GUILayout.Space(5f);
            }
            GUILayout.EndScrollView(); GUILayout.Label(_message, _body); GUILayout.EndArea();
        }

        private void DrawSanctuaryArmory(GameWorld world, Rect area)
        {
            GUI.Box(area, GUIContent.none, _panel);
            Rect stats = new Rect(area.x + 16f, area.y + 16f, area.width * 0.24f, area.height - 32f);
            Rect doll = new Rect(stats.xMax + 12f, area.y + 16f, area.width * 0.30f, area.height - 32f);
            Rect storage = new Rect(doll.xMax + 12f, area.y + 16f, area.xMax - doll.xMax - 28f, area.height - 32f);
            GUI.Box(stats, GUIContent.none, _panel); GUI.Box(doll, GUIContent.none, _panel); GUI.Box(storage, GUIContent.none, _panel);

            GUILayout.BeginArea(Inset(stats, 14f));
            GUILayout.Label("LOADOUT STATISTICS", _title);
            PlayerStats value = world.Equipment.BuildStats(ProfileManager.Current.healthRank, ProfileManager.Current.manaRank, ProfileManager.Current.powerRank);
            GUILayout.Label("Health  " + value.maxHealth.ToString("0") + "\nMana  " + value.maxMana.ToString("0") + "\nMovement  " + value.moveSpeed.ToString("0.0") +
                "\nArmor  " + value.armor.ToString("0.0") + "\nResistance  " + (value.resistance * 100f).ToString("0.#") + "%\nCritical Chance  " + (value.critChance * 100f).ToString("0.#") +
                "%\nCritical Damage  " + (value.critDamage * 100f).ToString("0.#") + "%\nSpell Power  " + (value.spellPower * 100f).ToString("0.#") + "%\nTrigger Energy  " + value.triggerEnergy.ToString("0"), _body);
            GUILayout.Space(12f); GUILayout.Label("SET BONUSES", _header); GUILayout.Label(world.Equipment.SetBonusSummary(), _small);
            GUILayout.Space(12f); GUILayout.Label("RULES", _header);
            GUILayout.Label("This complete ten-slot loadout is locked when the expedition begins. No reserve equipment enters the dungeon.", _small);
            GUILayout.EndArea();

            GUILayout.BeginArea(Inset(doll, 14f));
            GUILayout.Label("EQUIPPED LOADOUT", _title);
            GUILayout.Label("Select a slot to inspect it and filter the Stash. Every action has an explicit button.", _small);
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                ItemInstance item; world.Equipment.equipped.TryGetValue(slot, out item);
                Color old = GUI.backgroundColor; GUI.backgroundColor = item == null || item.Definition == null ? Color.gray : item.Definition.color;
                if (GUILayout.Button(slot.ToString().ToUpperInvariant() + "\n" + (item == null ? "— EMPTY —" : ItemLabel(item)), GUILayout.Height(54f)))
                { _selectedStashItem = item; _stashSlotFilter = (int)slot; }
                GUI.backgroundColor = old;
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("SAVE A")) SaveEquipmentLoadout(world, "Loadout A");
            if (GUILayout.Button("SAVE B")) SaveEquipmentLoadout(world, "Loadout B");
            if (GUILayout.Button("SAVE C")) SaveEquipmentLoadout(world, "Loadout C");
            GUILayout.EndHorizontal();
            foreach (EquipmentLoadoutSave loadout in ProfileManager.Current.equipmentLoadouts)
                if (GUILayout.Button("EQUIP " + loadout.name.ToUpperInvariant())) LoadEquipmentLoadout(world, loadout);
            GUILayout.EndArea();

            GUILayout.BeginArea(Inset(storage, 14f));
            GUILayout.Label("PERMANENT STASH · " + world.Equipment.backpack.Count + " ITEMS", _title);
            GUILayout.BeginHorizontal(); GUILayout.Label("SEARCH", GUILayout.Width(58f)); _search = GUILayout.TextField(_search); GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            string[] sortNames = { "NEWEST", "RARITY", "LEVEL", "SLOT" };
            if (GUILayout.Button("SORT · " + sortNames[Mathf.Clamp(_itemSortMode, 0, 3)])) _itemSortMode = (_itemSortMode + 1) % 4;
            if (GUILayout.Button("SLOT · " + (_stashSlotFilter < 0 ? "ALL" : ((EquipmentSlot)_stashSlotFilter).ToString())))
                _stashSlotFilter = _stashSlotFilter >= Enum.GetValues(typeof(EquipmentSlot)).Length - 1 ? -1 : _stashSlotFilter + 1;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("RARITY · " + (ItemRarity)Mathf.Clamp(_stashMinimumRarity, 0, 3))) _stashMinimumRarity = (_stashMinimumRarity + 1) % 4;
            string[] specialFilters = { "ALL", "FAVORITES", "JUNK", "UNIQUE", "SETS" };
            if (GUILayout.Button("VIEW · " + specialFilters[Mathf.Clamp(_stashSpecialFilter, 0, specialFilters.Length - 1)]))
                _stashSpecialFilter = (_stashSpecialFilter + 1) % specialFilters.Length;
            GUILayout.EndHorizontal();
            IEnumerable<ItemInstance> stashItems = world.Equipment.backpack.Where(item => item != null && item.Definition != null)
                .Where(item => _stashSlotFilter < 0 || (int)item.Definition.slot == _stashSlotFilter)
                .Where(item => (int)item.rarity >= _stashMinimumRarity)
                .Where(item => _stashSpecialFilter == 0 || _stashSpecialFilter == 1 && item.favorite || _stashSpecialFilter == 2 && item.junk ||
                    _stashSpecialFilter == 3 && item.rarity == ItemRarity.Unique || _stashSpecialFilter == 4 && !string.IsNullOrEmpty(item.Definition.setId))
                .Where(item => string.IsNullOrEmpty(_search) || item.DisplayName.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.affixes.Any(affix => V11Itemization.FormatAffix(affix, false).IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0));
            if (_itemSortMode == 1) stashItems = stashItems.OrderByDescending(item => item.rarity).ThenByDescending(item => item.itemLevel);
            else if (_itemSortMode == 2) stashItems = stashItems.OrderByDescending(item => item.itemLevel).ThenByDescending(item => item.rarity);
            else if (_itemSortMode == 3) stashItems = stashItems.OrderBy(item => item.Definition.slot).ThenByDescending(item => item.rarity);
            else stashItems = stashItems.Reverse();
            _secondaryScroll = GUILayout.BeginScrollView(_secondaryScroll, GUILayout.Height(storage.height * 0.46f));
            foreach (ItemInstance item in stashItems.ToArray())
            {
                Color old = GUI.backgroundColor; GUI.backgroundColor = item.Definition.color;
                if (GUILayout.Button((item.favorite ? "★ " : string.Empty) + (item.junk ? "JUNK · " : string.Empty) + ItemLabel(item), GUILayout.Height(62f))) _selectedStashItem = item;
                GUI.backgroundColor = old;
            }
            GUILayout.EndScrollView();
            GUILayout.Label("SELECTED ITEM", _header);
            if (_selectedStashItem == null) GUILayout.Label("Choose a stash or equipped item.", _small);
            else
            {
                GUILayout.Label(V11Itemization.BuildTooltip(_selectedStashItem, false), _small);
                GUILayout.Label(ItemComparison(world, _selectedStashItem), _small);
                bool inStash = world.Equipment.backpack.Contains(_selectedStashItem);
                GUILayout.BeginHorizontal();
                if (inStash && GUILayout.Button("EQUIP", GUILayout.Height(34f)))
                { string result; world.Equipment.Equip(_selectedStashItem, out result); world.Equipment.SaveSanctuaryEquipment(); world.RecalculateStats(true); _message = result; }
                if (!inStash && GUILayout.Button("UNEQUIP", GUILayout.Height(34f)))
                { string result; world.Equipment.Unequip(_selectedStashItem.Definition.slot, out result); world.Equipment.SaveSanctuaryEquipment(); world.RecalculateStats(true); _message = result; }
                if (GUILayout.Button(_selectedStashItem.favorite ? "UNFAVORITE" : "FAVORITE", GUILayout.Height(34f)))
                { _selectedStashItem.favorite = !_selectedStashItem.favorite; if (_selectedStashItem.favorite) _selectedStashItem.junk = false; world.Equipment.SaveSanctuaryEquipment(); }
                if (GUILayout.Button(_selectedStashItem.locked ? "UNPROTECT" : "PROTECT ITEM", GUILayout.Height(34f)))
                { _selectedStashItem.locked = !_selectedStashItem.locked; world.Equipment.SaveSanctuaryEquipment(); }
                GUILayout.EndHorizontal();
                if (GUILayout.Button(_selectedStashItem.junk ? "REMOVE JUNK MARK" : "MARK AS JUNK", GUILayout.Height(30f)))
                { _selectedStashItem.junk = !_selectedStashItem.junk; if (_selectedStashItem.junk) _selectedStashItem.favorite = false; world.Equipment.SaveSanctuaryEquipment(); }
            }
            GUILayout.Label(_message, _body);
            GUILayout.EndArea();
        }

        private void DrawSanctuaryForge(GameWorld world, Rect area)
        {
            GUI.Box(area, GUIContent.none, _panel);
            Rect list = new Rect(area.x + 16f, area.y + 16f, area.width * 0.31f, area.height - 32f);
            Rect itemPanel = new Rect(list.xMax + 12f, area.y + 16f, area.width * 0.34f, area.height - 32f);
            Rect actions = new Rect(itemPanel.xMax + 12f, area.y + 16f, area.xMax - itemPanel.xMax - 28f, area.height - 32f);
            GUI.Box(list, GUIContent.none, _panel); GUI.Box(itemPanel, GUIContent.none, _panel); GUI.Box(actions, GUIContent.none, _panel);

            GUILayout.BeginArea(Inset(list, 14f));
            GUILayout.Label("EQUIPMENT FORGE", _title);
            GUILayout.Label("SECURED MATERIALS\n" + ProfileManager.Current.forgeDust + " Forge Dust · " + ProfileManager.Current.bindingRunes + " Binding Runes · " + ProfileManager.Current.corruptionCores + " Corruption Cores", _body);
            GUILayout.BeginHorizontal(); GUILayout.Label("SEARCH", GUILayout.Width(58f)); _itemSearch = GUILayout.TextField(_itemSearch); GUILayout.EndHorizontal();
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            foreach (ItemInstance item in world.Equipment.backpack.Concat(world.Equipment.equipped.Values).Where(item => item != null && item.Definition != null)
                .Where(item => string.IsNullOrEmpty(_itemSearch) || item.DisplayName.IndexOf(_itemSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(item => item.rarity).ThenByDescending(item => item.itemLevel).ToArray())
                if (GUILayout.Button(ItemLabel(item), GUILayout.Height(58f))) { _selectedStashItem = item; _pendingForgeAction = null; }
            GUILayout.EndScrollView(); GUILayout.EndArea();

            GUILayout.BeginArea(Inset(itemPanel, 14f));
            GUILayout.Label("SELECTED SECURED ITEM", _title);
            if (_selectedStashItem == null) GUILayout.Label("Select a secured item. Equipment cannot be forged during an expedition.", _body);
            else
            {
                _secondaryScroll = GUILayout.BeginScrollView(_secondaryScroll);
                GUILayout.Label(V11Itemization.BuildTooltip(_selectedStashItem, true), _body);
                GUILayout.Space(10f); GUILayout.Label("LOADOUT EFFECT", _header); GUILayout.Label(ItemComparison(world, _selectedStashItem), _small);
                GUILayout.EndScrollView();
            }
            GUILayout.Label(_message, _body); GUILayout.EndArea();

            GUILayout.BeginArea(Inset(actions, 14f));
            GUILayout.Label("FORGE ACTIONS", _title);
            GUILayout.Label("Every action shows its complete secured-material cost. Failed actions refund all materials.", _small);
            foreach (ForgeAction action in Enum.GetValues(typeof(ForgeAction)))
            {
                ForgeCost cost = V11Itemization.CostFor(action, _selectedStashItem == null ? 1 : _selectedStashItem.itemLevel);
                string actionName = action == ForgeAction.LockAffix ? "Protect Affix" : SplitWords(action.ToString());
                bool confirmation = action == ForgeAction.Corrupt || action == ForgeAction.RemoveAffix;
                bool pending = _pendingForgeAction.HasValue && _pendingForgeAction.Value == action;
                string unavailable;
                bool legal = V11Itemization.CanCraft(_selectedStashItem, action, out unavailable);
                bool affordable = ProfileManager.Current.forgeDust >= cost.dust && ProfileManager.Current.bindingRunes >= cost.runes && ProfileManager.Current.corruptionCores >= cost.cores;
                GUI.enabled = legal && affordable;
                string status = !legal ? unavailable : !affordable ? "Insufficient secured materials" : ProfileManager.PermanentForgeCostDescription(cost);
                if (GUILayout.Button((confirmation && !pending ? "REVIEW " : "") + actionName.ToUpperInvariant() + "\n" + status, GUILayout.Height(48f)))
                {
                    if (confirmation && !pending)
                    { _pendingForgeAction = action; _message = "Review the item, then press " + actionName + " again to confirm."; }
                    else
                    {
                        string result;
                        if (V11Itemization.TryCraft(_selectedStashItem, action, null, out result))
                        { world.Equipment.SaveSanctuaryEquipment(); world.RecalculateStats(true); ProfileManager.Save(); }
                        _message = result; _pendingForgeAction = null;
                    }
                }
                GUI.enabled = true;
            }
            GUILayout.Space(10f);
            bool canDismantle = _selectedStashItem != null && world.Equipment.backpack.Contains(_selectedStashItem);
            GUI.enabled = canDismantle;
            bool dismantlePending = _selectedStashItem != null && _pendingSalvageInstanceId == _selectedStashItem.instanceId;
            if (GUILayout.Button((dismantlePending ? "CONFIRM DISMANTLE" : "REVIEW DISMANTLE") + "\nDestroys this secured item", GUILayout.Height(50f)))
            {
                if (!dismantlePending) { _pendingSalvageInstanceId = _selectedStashItem.instanceId; _message = "Press Confirm Dismantle again. This cannot be undone."; }
                else
                {
                    string result; ItemInstance removed = _selectedStashItem;
                    if (world.Equipment.DismantlePermanentItem(removed, true, out result)) _selectedStashItem = null;
                    _pendingSalvageInstanceId = null; _message = result;
                }
            }
            GUI.enabled = true;
            GUILayout.EndArea();
        }

        private void DrawUpgrades(RunDirector run, Rect area)
        {
            GUI.Box(area, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(area, 24f));
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            GUILayout.Label("PERMANENT UPGRADE TREE · " + ProfileManager.Current.essence + " ESSENCE", _title);
            GUILayout.Label("The tree emphasizes access, preparation, variety, and mastery. Raw percentage power remains secondary.", _body);
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            DrawUpgradeColumn(run, new[] { "StartingSpells", "Preparation", "Rerolls", "ArchiveTools" }, new[]
            {
                "Start with another selected Spell Core (maximum three)", "+3 Loadout Points", "+1 reward reroll per run", "More saved builds and discovery tools"
            });
            DrawUpgradeColumn(run, new[] { "Health", "Mana", "Power" }, new[] { "+8 starting life", "+8 starting mana", "+4% spell power" });
            GUILayout.BeginVertical(GUILayout.Width((area.width - 90f) / 3f));
            GUILayout.Label("CONTENT UNLOCKS", _header);
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            foreach (SpellCoreDefinition core in DemoCatalog.AllCores.Where(c => c.id != "void_maw" && !ProfileManager.Current.unlockedCoreIds.Contains(c.id)))
                if (GUILayout.Button(core.displayName + " · 30 Essence\n" + core.description, GUILayout.Height(58f))) { string result; run.UnlockContent(core.id, false, out result); _message = result; }
            foreach (SpellModifierDefinition modifier in DemoCatalog.AllModifiers.Where(m => !ProfileManager.Current.unlockedModifierIds.Contains(m.id)))
                if (GUILayout.Button(modifier.FullDisplayName + " · 18 Essence\n" + modifier.shortDescription, GUILayout.Height(58f))) { string result; run.UnlockContent(modifier.id, true, out result); _message = result; }
            GUILayout.EndScrollView(); GUILayout.EndVertical(); GUILayout.EndHorizontal();
            GUILayout.Label(_message, _body); GUILayout.FlexibleSpace();
            if (!_confirmRespec)
            {
                if (GUILayout.Button("RESPEC PERMANENT UPGRADE RANKS", GUILayout.Height(34f))) _confirmRespec = true;
            }
            else
            {
                GUILayout.Label("Refund all Essence spent on upgrade ranks. Content unlocks, items, spells, history, and discoveries remain untouched.", _small);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("CONFIRM RESPEC", GUILayout.Height(34f))) { int refund; run.RespecUpgradeRanks(out refund); _confirmRespec = false; _message = refund + " Essence refunded."; }
                if (GUILayout.Button("CANCEL", GUILayout.Height(34f))) _confirmRespec = false;
                GUILayout.EndHorizontal();
            }
            if (!_confirmReset)
            {
                if (GUILayout.Button("RESET THIS PROFILE", GUILayout.Height(30f))) _confirmReset = true;
            }
            else
            {
                GUILayout.Label("This permanently deletes this profile's progression and saved run.", _small);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("CONFIRM RESET", GUILayout.Height(34f))) { run.ResetMetaProgression(); _confirmReset = false; _message = "Profile reset to clean defaults."; }
                if (GUILayout.Button("CANCEL", GUILayout.Height(34f))) _confirmReset = false;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView(); GUILayout.EndArea();
        }

        private void DrawUpgradeColumn(RunDirector run, string[] keys, string[] benefits)
        {
            GUILayout.BeginVertical(GUILayout.Width((Screen.width - 150f) / 3f));
            GUILayout.Label(keys[0] == "StartingSpells" ? "POSSIBILITY" : "FOUNDATION", _header);
            for (int i = 0; i < keys.Length; i++)
            {
                int rank = UpgradeRank(keys[i]); int cap = keys[i] == "StartingSpells" ? 2 : keys[i] == "Power" || keys[i] == "Preparation" ? 6 : keys[i] == "Rerolls" || keys[i] == "ArchiveTools" ? 3 : 8;
                int cost = keys[i] == "StartingSpells" ? 35 + rank * 65 : 10 + rank * 12;
                if (GUILayout.Button(keys[i] + " rank " + rank + "/" + cap + "\n" + benefits[i] + (rank >= cap ? " · MAX" : " · " + cost + " Essence"), GUILayout.Height(62f)))
                {
                    string result; run.BuyUpgrade(keys[i], out result); _message = result;
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawCodex(Rect area)
        {
            GUI.Box(area, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(area, 22f));
            GUILayout.Label("DISCOVERY CODEX", _title);
            int total = DemoCatalog.AllCores.Count() + DemoCatalog.AllModifiers.Count() + DemoCatalog.AllItems.Count() + MegaCatalog.AllRelics.Count() + MegaCatalog.AllRooms.Count();
            GUILayout.Label(ProfileManager.Current.codexEntries.Count + "/" + total + " discoveries recorded · bosses " + ProfileManager.Current.bossKills + " · best room " + ProfileManager.Current.bestRoom + " · total kills " + ProfileManager.Current.totalKills, _body);
            GUILayout.BeginHorizontal(); GUILayout.Label("Search", GUILayout.Width(55f)); _search = GUILayout.TextField(_search); GUILayout.EndHorizontal();
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            GUILayout.Label("RECENT RUNS", _header);
            foreach (RunHistorySave run in ProfileManager.Current.runHistory.AsEnumerable().Reverse().Take(5))
                GUILayout.Label((run.victory ? "◆ COMPLETE" : "◇ DEFEAT") + " · " + ((DemoRunMode)run.mode).ToString().ToUpperInvariant() + " · seed " + run.seed +
                    " · rooms " + run.roomsCleared + " · kills " + run.kills + " · level " + run.runLevel + " · top spell " + run.topSpell +
                    (string.IsNullOrEmpty(run.deathSource) ? string.Empty : " · fell to " + run.deathSource), _body);
            GUILayout.Label("RECENT SPELL BUILDS", _header);
            foreach (BuildHistorySave build in ProfileManager.Current.buildHistory.AsEnumerable().Reverse().Take(5))
                GUILayout.Label("✦ " + build.name + " · " + build.element + " " + build.delivery + " · " + build.activeModifiers + " Support Runes · " + build.estimatedDps.ToString("0.0") + " estimated DPS", _body);
            GUILayout.Label("RECENT TRANSACTIONS", _header);
            foreach (TransactionSave transaction in ProfileManager.Current.transactionHistory.AsEnumerable().Reverse().Take(5))
                GUILayout.Label((transaction.amount >= 0 ? "+" : string.Empty) + transaction.amount + " · " + transaction.category + " · " + transaction.note, _small);
            GUILayout.Label("DISCOVERIES", _header);
            foreach (string entry in ProfileManager.Current.codexEntries.Where(e => string.IsNullOrEmpty(_search) || e.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0).OrderBy(e => e))
                GUILayout.Label("◆ " + DescribeCodexEntry(entry), _body);
            GUILayout.EndScrollView(); GUILayout.EndArea();
        }

        private void DrawTrainingAndOptions(GameWorld world, Rect area)
        {
            GUI.Box(area, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(area, 24f));
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            GUILayout.Label("TRAINING CHAMBER", _title);
            GUILayout.Label("Test prepared spells against durable targets without risking loot. The same Spell Boards, Spell Links, runtime limits, visuals, and equipment rules are used.", _body);
            if (GUILayout.Button("ENTER TRAINING CHAMBER", GUILayout.Height(50f))) world.EnterTraining();
            if (ProfileManager.Current.bossKills > 0 && GUILayout.Button("PRACTICE THE DUNGEON WARDEN", GUILayout.Height(44f))) world.EnterBossPractice();
            GUILayout.Space(18f); GUILayout.Label("ACCESSIBILITY & EFFECT BUDGETS", _title);
            AccessibilitySettings settings = ProfileManager.Current.accessibility;
            GUILayout.Label("UI text scale " + settings.uiScale.ToString("0.00"), _body); settings.uiScale = GUILayout.HorizontalSlider(settings.uiScale, 0.85f, 1.3f);
            GUILayout.Label("Effect density " + settings.effectDensity.ToString("0.00") + " · dynamically limits projectiles, trails, lights, sounds, summons, and recursive entities", _body);
            settings.effectDensity = GUILayout.HorizontalSlider(settings.effectDensity, 0.35f, 1f);
            GUILayout.Label("Visual quality preset " + (ArcaneVisualQuality)Mathf.Clamp(settings.visualQuality, 0, 2) + " · changes density, lights, shadows, marks, geometry layers, and environment detail", _body);
            int previousVisualQuality = settings.visualQuality;
            int selectedVisualQuality = Mathf.RoundToInt(GUILayout.HorizontalSlider(settings.visualQuality, 0f, 2f));
            if (selectedVisualQuality != previousVisualQuality) VisualQualityPolicy.ApplyPreset(settings, selectedVisualQuality);
            GUILayout.Label("Spell visual density " + settings.spellEffectDensity.ToString("0.00"), _body); settings.spellEffectDensity = GUILayout.HorizontalSlider(settings.spellEffectDensity, 0.25f, 1f);
            GUILayout.Label("Environment density " + settings.environmentDensity.ToString("0.00"), _body); settings.environmentDensity = GUILayout.HorizontalSlider(settings.environmentDensity, 0.2f, 1f);
            GUILayout.Label("Dynamic lights " + (settings.dynamicLightQuality == 0 ? "OFF" : settings.dynamicLightQuality == 1 ? "LIMITED" : "HIGH"), _body); settings.dynamicLightQuality = Mathf.RoundToInt(GUILayout.HorizontalSlider(settings.dynamicLightQuality, 0f, 2f));
            GUILayout.Label("Shadows " + (settings.shadowQuality == 0 ? "OFF" : settings.shadowQuality == 1 ? "HARD" : "SOFT"), _body); settings.shadowQuality = Mathf.RoundToInt(GUILayout.HorizontalSlider(settings.shadowQuality, 0f, 2f));
            GUILayout.Label("Persistent mark duration " + settings.decalDuration.ToString("0.00"), _body); settings.decalDuration = GUILayout.HorizontalSlider(settings.decalDuration, 0.25f, 2f);
            GUILayout.Label("Hit stop " + settings.hitStop.ToString("0.00"), _body); settings.hitStop = GUILayout.HorizontalSlider(settings.hitStop, 0f, 1f);
            GUILayout.Label("Damage number density " + settings.damageNumberDensity.ToString("0.00"), _body); settings.damageNumberDensity = GUILayout.HorizontalSlider(settings.damageNumberDensity, 0.25f, 1f);
            GUILayout.Label("Screen shake " + settings.screenShake.ToString("0.00"), _body); settings.screenShake = GUILayout.HorizontalSlider(settings.screenShake, 0f, 1f);
            settings.reducedFlashes = GUILayout.Toggle(settings.reducedFlashes, "Reduced flashes", _button);
            settings.colorblindConnectors = GUILayout.Toggle(settings.colorblindConnectors, "Colorblind connector palette", _button);
            settings.colorblindSymbols = GUILayout.Toggle(settings.colorblindSymbols, "Colorblind element, rarity, and status symbols", _button);
            settings.showVisualDiagnostics = GUILayout.Toggle(settings.showVisualDiagnostics, "Visual diagnostics overlay (F10)", _button);
            settings.autoAim = GUILayout.Toggle(settings.autoAim, "Hold Left Shift for aim assistance", _button);
            settings.alwaysShowEnemyHealth = GUILayout.Toggle(settings.alwaysShowEnemyHealth, "Always show enemy Health bars", _button);
            settings.showDamageNumbers = GUILayout.Toggle(settings.showDamageNumbers, "Show damage numbers", _button);
            ProfileManager.Current.autoCollectGold = GUILayout.Toggle(ProfileManager.Current.autoCollectGold, "Automatically collect nearby Gold", _button);
            ProfileManager.Current.autoCollectEssence = GUILayout.Toggle(ProfileManager.Current.autoCollectEssence, "Automatically collect nearby Essence", _button);
            GUILayout.Space(10f); GUILayout.Label("AUDIO MIX", _header);
            GUILayout.Label("Master " + Mathf.RoundToInt(settings.masterVolume * 100f) + "%", _body); settings.masterVolume = GUILayout.HorizontalSlider(settings.masterVolume, 0f, 1f);
            GUILayout.Label("Music " + Mathf.RoundToInt(settings.musicVolume * 100f) + "%", _body); settings.musicVolume = GUILayout.HorizontalSlider(settings.musicVolume, 0f, 1f);
            GUILayout.Label("Combat effects " + Mathf.RoundToInt(settings.effectsVolume * 100f) + "%", _body); settings.effectsVolume = GUILayout.HorizontalSlider(settings.effectsVolume, 0f, 1f);
            GUILayout.Label("Ambience " + Mathf.RoundToInt(settings.ambienceVolume * 100f) + "%", _body); settings.ambienceVolume = GUILayout.HorizontalSlider(settings.ambienceVolume, 0f, 1f);
            GUILayout.Label("UI " + Mathf.RoundToInt(settings.uiVolume * 100f) + "%", _body); settings.uiVolume = GUILayout.HorizontalSlider(settings.uiVolume, 0f, 1f);
            GUILayout.Space(10f); GUILayout.Label("INPUT REBINDING", _header);
            ControlSettings controls = ProfileManager.Current.controls;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Slot 3: " + controls.spellSlot3)) controls.spellSlot3 = NextKey(controls.spellSlot3, new[] { KeyCode.Q, KeyCode.R, KeyCode.F, KeyCode.C });
            if (GUILayout.Button("Dodge: " + controls.dodge)) controls.dodge = NextKey(controls.dodge, new[] { KeyCode.Space, KeyCode.LeftShift, KeyCode.LeftControl });
            if (GUILayout.Button("Spellcraft: " + controls.workshop)) controls.workshop = NextKey(controls.workshop, new[] { KeyCode.Tab, KeyCode.G, KeyCode.V });
            if (GUILayout.Button("Spell Links: " + controls.spellLinks)) controls.spellLinks = NextKey(controls.spellLinks, new[] { KeyCode.L, KeyCode.K, KeyCode.H });
            if (GUILayout.Button("Inventory: " + controls.inventory)) controls.inventory = NextKey(controls.inventory, new[] { KeyCode.I, KeyCode.B, KeyCode.N });
            if (GUILayout.Button("Interact: " + controls.interact)) controls.interact = NextKey(controls.interact, new[] { KeyCode.E, KeyCode.F, KeyCode.R });
            GUILayout.EndHorizontal();
            List<string> controlConflicts = ControlSettingsValidator.FindConflicts(controls);
            if (controlConflicts.Count > 0) GUILayout.Label("BINDING CONFLICT · " + string.Join(" · ", controlConflicts), _small);
            if (GUILayout.Button("RESET KEYBOARD & MOUSE CONTROLS", GUILayout.Height(30f))) { ProfileManager.Current.controls = new ControlSettings(); controls = ProfileManager.Current.controls; }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("1280 × 720")) Screen.SetResolution(1280, 720, Screen.fullScreenMode);
            if (GUILayout.Button("1600 × 900")) Screen.SetResolution(1600, 900, Screen.fullScreenMode);
            if (GUILayout.Button("1920 × 1080")) Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
            if (GUILayout.Button(Screen.fullScreen ? "WINDOWED" : "FULLSCREEN")) Screen.fullScreen = !Screen.fullScreen;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("SAVE NOW", GUILayout.Height(34f))) { ProfileManager.Save(); _stylesReady = false; _message = ProfileManager.LastSaveStatus; }
            if (GUILayout.Button("SAVE OPTIONS", GUILayout.Height(34f))) { ProfileManager.Save(); _stylesReady = false; _message = "Accessibility, resolution, and input settings saved."; }
            if (GUILayout.Button("OPEN SAVE FOLDER", GUILayout.Height(34f))) Application.OpenURL("file://" + ProfileManager.ProfileFolderPath);
            GUILayout.EndHorizontal();
            GUILayout.Label("Save folder: " + ProfileManager.ProfileFolderPath + "\n" + ProfileManager.LastSaveStatus, _small);
            GUILayout.Space(15f); GUILayout.Label("PROFILES", _header);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 3; i++)
            {
                int index = i;
                if (GUILayout.Button((ProfileManager.ActiveIndex == i ? "ACTIVE · " : string.Empty) + "Profile " + (i + 1), GUILayout.Height(36f)))
                {
                    ProfileManager.SwitchProfile(index); world.Equipment.LoadFromProfile(false); world.RecalculateStats(false); _selectedProfile = index;
                }
            }
            GUILayout.EndHorizontal(); GUILayout.Label(_message, _body);
            GUILayout.EndScrollView(); GUILayout.EndArea();
        }

        private void DrawHUD(GameWorld world, RunDirector run)
        {
            GUI.Box(new Rect(16f, 16f, 430f, 151f), GUIContent.none, _panel);
            string room = world.TrainingMode ? "TRAINING CHAMBER" : "ROOM " + (run.RoomIndex + 1) + "/" + run.TotalRooms + " · " + (run.CurrentRoom == null ? "" : run.CurrentRoom.displayName);
            GUI.Label(new Rect(30f, 23f, 400f, 24f), room, _header);
            DrawBar(new Rect(30f, 53f, 390f, 22f), world.Player.Health / world.Stats.maxHealth, new Color(0.85f, 0.12f, 0.18f),
                "HEALTH " + Mathf.CeilToInt(world.Player.Health) + "/" + Mathf.CeilToInt(world.Stats.maxHealth) + (world.Player.Ward > 0f ? " + " + Mathf.CeilToInt(world.Player.Ward) + " SHIELD" : string.Empty));
            DrawBar(new Rect(30f, 80f, 390f, 22f), world.Player.Mana / world.Stats.maxMana, new Color(0.1f, 0.45f, 0.95f), "MANA " + Mathf.CeilToInt(world.Player.Mana) + "/" + Mathf.CeilToInt(world.Stats.maxMana));
            GUI.Label(new Rect(30f, 107f, 400f, 20f), "Kills " + run.Kills + " · " + run.Gold + " Gold · reward x" + run.Difficulty.RewardMultiplier.ToString("0.00"), _small);
            GUI.Label(new Rect(30f, 128f, 400f, 20f), "Tab Spell Crafting · I Inventory · Esc Pause" + (run.Difficulty.timedRooms && run.EncounterActive ? " · TIMER " + Mathf.CeilToInt(run.TimedRoomRemaining) : string.Empty), _small);

            float logX = Screen.width - 430f;
            GUI.Box(new Rect(logX, 16f, 414f, 170f), GUIContent.none, _panel);
            GUI.Label(new Rect(logX + 14f, 23f, 380f, 22f), "ARCANE LOG", _header);
            for (int i = 0; i < Mathf.Min(7, world.CombatLog.Count); i++) GUI.Label(new Rect(logX + 14f, 48f + i * 17f, 385f, 18f), world.CombatLog[i], _small);

            float bottom = Screen.height - 112f;
            GUI.Box(new Rect(16f, bottom, Screen.width - 32f, 96f), GUIContent.none, _panel);
            string[] controls = { "LMB", "RMB", "Q" };
            for (int i = 0; i < 3; i++)
            {
                CompiledSpell spell = world.GetSpell((SpellSlot)i);
                float width = (Screen.width - 76f) / 3f;
                GUI.Label(new Rect(30f + i * width, bottom + 10f, width - 12f, 46f), controls[i] + " · " + (spell == null ? "EMPTY — find a Spell Core" : spell.CompactSummary()), _header);
                if (spell != null)
                {
                    int links = world.SpellLinks == null ? 0 : world.SpellLinks.Links.Count(value => value.sourceSlot == i);
                    GUI.Label(new Rect(30f + i * width, bottom + 60f, width - 12f, 20f), "Overload " + (Mathf.Clamp01(spell.instability / 450f) * 100f).ToString("0.0") + "% · " + links + " outgoing Link(s)", _small);
                }
            }
        }

        private void DrawPause(RunDirector run)
        {
            Rect panel = CenteredPanel(560f, 430f);
            GUI.Box(panel, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(panel, 28f));
            GUILayout.Label("PAUSED", _title);
            GUILayout.Label("Your current profile is protected by automatic backups. Save & Quit stores the run and returns to Home Base; continuing restarts the current room.", _body);
            GUILayout.Space(15f);
            if (GUILayout.Button("RESUME", GUILayout.Height(48f))) CloseModals();
            if (GUILayout.Button("SAVE RUN CHECKPOINT", GUILayout.Height(42f))) _message = run.SaveRunCheckpoint() ? ProfileManager.LastSaveStatus : "Run could not be saved.";
            if (GUILayout.Button("EXPORT DIAGNOSTIC REPORT", GUILayout.Height(38f)) && V1Diagnostics.Instance != null)
            {
                try { _message = "Report exported: " + V1Diagnostics.Instance.ExportReport(); }
                catch (Exception exception) { _message = "Export failed: " + exception.Message; }
            }
            if (GUILayout.Button("SAVE & QUIT TO HOME BASE", GUILayout.Height(48f)))
            {
                string result;
                if (run.SaveAndQuit(out result)) { _pauseOpen = false; _message = result; }
                else _message = result;
            }
            if (GUILayout.Button("SAVE & QUIT TO TITLE", GUILayout.Height(44f)))
            {
                string result;
                if (run.SaveAndQuit(out result))
                {
                    _pauseOpen = false; _message = result;
                    if (V1TitleScreen.Instance != null) V1TitleScreen.Instance.ShowTitle();
                }
                else _message = result;
            }
            GUILayout.Space(8f);
            GUILayout.Label(_message, _small);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ABANDON RUN · TEMPORARY REWARDS WILL BE LOST", GUILayout.Height(40f)))
            {
                _pauseOpen = false;
                run.EndRun(false);
            }
            GUILayout.EndArea();
        }

        private void DrawRewards(RunDirector run)
        {
            Rect panel = CenteredPanel(1080f, 570f); GUI.Box(panel, GUIContent.none, _panel);
            GUI.Label(new Rect(panel.x + 28f, panel.y + 20f, 700f, 40f), "ROOM REWARD · CHOOSE ONE", _title);
            GUI.Label(new Rect(panel.x + 28f, panel.y + 62f, 950f, 24f), "One useful option, one build-aware option, and one unusual or transformative possibility.", _body);
            float width = (panel.width - 88f) / 3f;
            for (int i = 0; i < run.RewardChoices.Count; i++)
            {
                RewardOffer reward = run.RewardChoices[i]; Rect card = new Rect(panel.x + 24f + i * (width + 20f), panel.y + 108f, width, 330f);
                Color previous = GUI.backgroundColor; GUI.backgroundColor = reward.color;
                GUI.Box(card, GUIContent.none, _panel); GUI.Label(new Rect(card.x + 16f, card.y + 18f, card.width - 32f, 48f), reward.title + (reward.risky ? " · RISK" : string.Empty), _header);
                GUI.Label(new Rect(card.x + 16f, card.y + 76f, card.width - 32f, 170f), reward.description, _body);
                if (GUI.Button(new Rect(card.x + 16f, card.yMax - 64f, card.width - 32f, 44f), "CHOOSE", _button)) run.ChooseReward(i);
                GUI.backgroundColor = previous;
            }
            if (run.RewardRerollsRemaining > 0 && GUI.Button(new Rect(panel.center.x - 130f, panel.yMax - 68f, 260f, 38f), "REROLL · " + run.RewardRerollsRemaining + " REMAIN")) run.RerollRewards();
        }

        private void DrawRoute(RunDirector run)
        {
            Rect panel = CenteredPanel(900f, 440f); GUI.Box(panel, GUIContent.none, _panel);
            GUI.Label(new Rect(panel.x + 28f, panel.y + 22f, 650f, 42f), "CHOOSE THE NEXT ROUTE", _title);
            float width = (panel.width - 74f) / Mathf.Max(1, run.RouteChoices.Count);
            for (int i = 0; i < run.RouteChoices.Count; i++)
            {
                RoomTemplate room = run.RouteChoices[i]; Rect card = new Rect(panel.x + 24f + i * (width + 18f), panel.y + 90f, width, 280f);
                Color previous = GUI.backgroundColor; GUI.backgroundColor = room.accentColor; GUI.Box(card, GUIContent.none, _panel);
                GUI.Label(new Rect(card.x + 16f, card.y + 18f, card.width - 32f, 52f), room.displayName, _header);
                GUI.Label(new Rect(card.x + 16f, card.y + 80f, card.width - 32f, 95f), FriendlyRoomType(room.type) + "\nDifficulty " + room.difficulty + (room.hasHazards ? " · Hazards" : "") + "\nHandcrafted room, selected for this run.", _body);
                if (GUI.Button(new Rect(card.x + 16f, card.yMax - 62f, card.width - 32f, 42f), "ENTER " + FriendlyRoomType(room.type).ToUpperInvariant())) run.ChooseRoute(i);
                GUI.backgroundColor = previous;
            }
        }

        private void DrawShop(RunDirector run)
        {
            Rect panel = CenteredPanel(1040f, 700f); GUI.Box(panel, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(panel, 24f));
            GUILayout.Label("DUNGEON SHOP · " + run.Gold + " GOLD", _title);
            GUILayout.Label("Gold lasts only for this run. Spend it before the run ends.", _body);
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            for (int i = 0; i < run.ShopOffers.Count; i++)
            {
                ShopOffer offer = run.ShopOffers[i];
                if (offer.category == RewardCategory.Equipment) GUILayout.Label("UNSECURED EQUIPMENT STOCK · CANNOT BE EQUIPPED THIS RUN", _header);
                GUILayout.BeginHorizontal(_panel); GUILayout.BeginVertical(); GUILayout.Label(offer.title + (offer.sold ? " · SOLD" : string.Empty), _header); GUILayout.Label(offer.description, _body); GUILayout.EndVertical();
                if (GUILayout.Button(offer.sold ? "SOLD" : offer.price + " GOLD", GUILayout.Width(110f), GUILayout.Height(60f))) { string result; run.BuyShopOffer(i, out result); _message = result; }
                GUILayout.EndHorizontal(); GUILayout.Space(4f);
            }
            GUILayout.EndScrollView(); GUILayout.Label(_message, _body);
            if (GUILayout.Button("LEAVE SHOP", GUILayout.Height(42f))) run.LeaveShop();
            GUILayout.EndArea();
        }

        private void DrawSafeRoom(RunDirector run)
        {
            Rect panel = CenteredPanel(680f, 370f); GUI.Box(panel, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(panel, 26f)); GUILayout.Label("SAFE SPELLCRAFT ROOM", _title);
            GUILayout.Label("Enemies cannot enter. Replace Spell Cores, rebuild all three boards, inspect Spell Links, and review unsecured equipment before continuing.", _body);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OPEN SPELLCRAFT", GUILayout.Height(46f))) _workshopOpen = true;
            if (GUILayout.Button("OPEN SPELL LINKS", GUILayout.Height(40f))) _spellLinksOpen = true;
            if (GUILayout.Button("OPEN UNSECURED RUN BAG", GUILayout.Height(40f))) _inventoryOpen = true;
            if (GUILayout.Button("CONTINUE RUN", GUILayout.Height(46f))) { _workshopOpen = _inventoryOpen = false; run.LeaveSafeRoom(); }
            GUILayout.EndArea();
            if (_workshopOpen) DrawWorkshop(GameWorld.Instance); else if (_spellLinksOpen) DrawSpellLinks(GameWorld.Instance); else if (_inventoryOpen) DrawRunInventory(GameWorld.Instance, run);
        }

        private void DrawExtraction(RunDirector run)
        {
            Rect panel = CenteredPanel(720f, 420f); GUI.Box(panel, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(panel, 28f)); GUILayout.Label("RETURN GATE STABILIZED", _title);
            GUILayout.Label("The boss is dead. Extract to secure every item in the Run Bag, stored Spell Copy, and unsecured Forge material. Legendary Shards and Essence are already safe. Temporary Support Runes and unspent Gold end with the run.", _body);
            GUILayout.Space(15f);
            GUILayout.Label("Stored Spells: " + GameWorld.Instance.RunCoreSatchel.Count + " Spell Copies · " + GameWorld.Instance.Equipment.runBag.Count + " unsecured item(s) · " +
                run.ForgeDust + " Dust · " + run.BindingRunes + " Runes · " + run.CorruptionCores + " Cores · " + run.Gold + " unspent Gold", _header);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("FINISH RUN AND RETURN HOME", GUILayout.Height(58f))) run.EndRun(true);
            GUILayout.EndArea();
        }

        private void DrawWorkshop(GameWorld world)
        {
            _dragDropHandled = false;
            bool canEdit = world.CanEditSpells;
            if (!canEdit) { _draggingModifier = false; _draggedPlacedModifier = null; }
            Rect panel = CenteredPanel(1240f, 820f); GUI.Box(panel, GUIContent.none, _panel);
            GUI.Label(new Rect(panel.x + 24f, panel.y + 14f, 620f, 40f), "SPELLCRAFT · SUPPORT RUNES", _title);
            GUI.Label(new Rect(panel.x + 590f, panel.y + 22f, 360f, 24f), canEdit ? "ROOM SAFE · EDITING AVAILABLE" : "READ ONLY · " + world.SpellEditLockReason, _header);
            if (GUI.Button(new Rect(panel.xMax - 260f, panel.y + 18f, 150f, 32f), "SPELL LINKS · " + ProfileManager.Current.controls.spellLinks)) OpenSpellLinksAnywhere();
            if (GUI.Button(new Rect(panel.xMax - 94f, panel.y + 18f, 70f, 32f), "Close")) CloseModals();
            for (int i = 0; i < 3; i++)
            {
                CompiledSpell candidate = world.GetSpell((SpellSlot)i);
                if (GUI.Button(new Rect(panel.x + 26f + i * 220f, panel.y + 62f, 208f, 38f), "Slot " + (i + 1) + " · " + (candidate == null ? "EMPTY" : candidate.displayName))) _workshopSlot = (SpellSlot)i;
            }
            SpellBoard board = world.GetBoard(_workshopSlot);
            if (board == null)
            {
                GUI.Label(new Rect(panel.x + 40f, panel.y + 130f, 650f, 60f), "This spell slot is empty. Equip a Spell Copy from Stored Spells.", _header);
                DrawCoreSatchelInstall(world, new Rect(panel.x + 40f, panel.y + 210f, 620f, panel.height - 250f));
                return;
            }
            CompiledSpell spell = world.GetSpell(_workshopSlot);
            SpellBuildValidationReport validation = SpellBuildValidator.Validate(board, world.Stats, world.Equipment);
            GUI.Label(new Rect(panel.x + 28f, panel.y + 112f, 735f, 24f), spell.CompactSummary(), _header);
            GUI.Label(new Rect(panel.x + 28f, panel.y + 138f, 735f, 22f), "SPELL LEVEL " + board.spellLevel + "/5 · CAPACITY " + board.UsedCapacity() + "/" + board.Capacity +
                " · OVERLOAD CHANCE " + (Mathf.Clamp01(spell.instability / 450f) * 100f).ToString("0.0") + "% · CONNECTED " + board.GetActivePlacements().Count + "/" + board.placed.Count, _small);

            if (world.PendingSpellUpgrade)
            {
                Rect upgrade = new Rect(panel.x + 28f, panel.y + 166f, 690f, 58f);
                GUI.Box(upgrade, GUIContent.none, _panel);
                GUI.Label(new Rect(upgrade.x + 12f, upgrade.y + 5f, 210f, 22f), "CHOOSE A SPELL TO LEVEL", _header);
                for (int i = 0; i < 3; i++)
                {
                    SpellBoard candidateBoard = world.GetBoard((SpellSlot)i);
                    string label = candidateBoard == null ? "EMPTY" : "SLOT " + (i + 1) + " · LEVEL " + candidateBoard.spellLevel + " → " + Mathf.Min(5, candidateBoard.spellLevel + 1);
                    GUI.enabled = canEdit && candidateBoard != null && candidateBoard.spellLevel < 5;
                    if (GUI.Button(new Rect(upgrade.x + 220f + i * 150f, upgrade.y + 10f, 140f, 36f), label))
                    {
                        string result;
                        if (world.ApplySpellUpgrade((SpellSlot)i, out result)) { _message = result; _workshopOpen = false; SyncPause(); }
                        else _message = result;
                    }
                    GUI.enabled = true;
                }
            }

            Vector2 boardCenter = new Vector2(panel.x + 370f, panel.y + 465f);
            float hexSize = board.Radius > 3 ? 36f : 43f;
            _placementPreviewSpell = null;
            DrawHexBoard(world, board, boardCenter, hexSize, canEdit);
            DrawSpellPreview(spell, _placementPreviewSpell, new Rect(panel.x + 28f, panel.yMax - 116f, 690f, 72f));

            Rect side = new Rect(panel.x + 748f, panel.y + 64f, panel.width - 772f, panel.height - 88f);
            GUI.Box(side, GUIContent.none, _panel);
            GUILayout.BeginArea(Inset(side, 14f));
            if (!canEdit) GUILayout.Label("SPELLCRAFTING LOCKED\n" + world.SpellEditLockReason + " You may inspect this build now.", _body);
            GUI.enabled = canEdit;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("UNDO") && board.Undo()) { world.RecalculateModifierAvailability(); _message = "Undo applied."; }
            if (GUILayout.Button("REDO") && board.Redo()) { world.RecalculateModifierAvailability(); _message = "Redo applied."; }
            if (GUILayout.Button("Q ↺ / E ↻")) _rotation = (_rotation + 1) % 6;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(); _layoutName = GUILayout.TextField(_layoutName);
            if (GUILayout.Button("SAVE BUILD", GUILayout.Width(110f))) SaveSpellLayout(board);
            GUILayout.EndHorizontal();
            if (ProfileManager.Current.savedSpellLayouts.Count > 0)
            {
                GUILayout.BeginHorizontal();
                foreach (SavedSpellLayout layout in ProfileManager.Current.savedSpellLayouts.Take(3))
                    if (GUILayout.Button(layout.name, GUILayout.Height(28f))) LoadSpellLayout(world, board, layout);
                GUILayout.EndHorizontal();
            }
            GUI.enabled = true;
            GUILayout.BeginHorizontal(); GUILayout.Label("Search", GUILayout.Width(48f)); _search = GUILayout.TextField(_search); _expertDetails = GUILayout.Toggle(_expertDetails, "Expert"); GUILayout.EndHorizontal();
            SpellModifierDefinition selected = DemoCatalog.GetModifier(_selectedModifier);
            if (selected != null)
            {
                GUILayout.Label(selected.FullDisplayName + " · rotation " + _rotation, _header);
                GUILayout.Label(_expertDetails ? selected.advancedDescription : selected.shortDescription, _body);
                SpellCoreDefinition selectedCore = DemoCatalog.GetCore(board.coreId);
                GUILayout.Label("Size " + selected.shape.Length + " hexes · Capacity Cost " + selected.capacityCost + " · Overload " + selected.instability +
                    (selected.IsCompatible(selectedCore) ? " · Compatible" : " · Requires " + string.Join("/", selected.compatibleDeliveries.Select(value => value.ToString()))), _small);
            }
            _secondaryScroll = GUILayout.BeginScrollView(_secondaryScroll, GUILayout.Height(255f));
            foreach (SpellModifierDefinition modifier in DemoCatalog.AllModifiers.Where(m => world.OwnedModifierCounts.ContainsKey(m.id))
                .Where(m => string.IsNullOrEmpty(_search) || m.displayName.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0).OrderBy(m => m.category).ThenBy(m => m.displayName))
            {
                int count; world.ModifierInventory.TryGetValue(modifier.id, out count);
                bool compatibleWithSelectedSpell = modifier.IsCompatible(DemoCatalog.GetCore(board.coreId));
                Color previous = GUI.backgroundColor; GUI.backgroundColor = modifier.id == _selectedModifier ? modifier.uiColor : Color.Lerp(Color.gray, modifier.uiColor, 0.45f);
                Rect modifierRect = GUILayoutUtility.GetRect(new GUIContent(modifier.FullDisplayName), _button, GUILayout.Height(58f));
                Event modifierEvent = Event.current;
                bool modifierPressed = modifierEvent.type == EventType.MouseDown && modifierEvent.button == 0 && modifierRect.Contains(modifierEvent.mousePosition);
                if (modifierPressed && canEdit)
                {
                    _selectedModifier = modifier.id;
                    if (!compatibleWithSelectedSpell)
                        _message = modifier.displayName + " requires " + string.Join("/", modifier.compatibleDeliveries.Select(value => value.ToString())) + " delivery.";
                    else if (count > 0) { _draggingModifier = true; _draggedPlacedModifier = null; }
                    modifierEvent.Use();
                }
                string compatibility = compatibleWithSelectedSpell ? string.Empty : " · INCOMPATIBLE";
                GUI.enabled = canEdit && compatibleWithSelectedSpell && count > 0;
                GUI.Button(modifierRect, modifier.FullDisplayName + " ×" + count + " · COST " + modifier.capacityCost + compatibility + "\n" + modifier.shortDescription, _button);
                GUI.enabled = true;
                GUI.backgroundColor = previous;
            }
            GUILayout.EndScrollView();
            GUILayout.Label("STORED SPELL COPIES", _header);
            if (world.RunCoreSatchel.Count == 0) GUILayout.Label("No unsecured Spell Copies found this run.", _small);
            else
            {
                _mainScroll = GUILayout.BeginScrollView(_mainScroll, GUILayout.Height(78f));
                foreach (CoreSaveData core in world.RunCoreSatchel.ToArray())
                {
                    SpellCoreDefinition coreDefinition = DemoCatalog.GetCore(core.coreId);
                    GUI.enabled = canEdit && coreDefinition != null;
                    if (GUILayout.Button((coreDefinition == null ? "Unknown Spell" : coreDefinition.displayName) + " · INSTALL IN SLOT " + ((int)_workshopSlot + 1), GUILayout.Height(30f)))
                    { string result; world.InstallCore(_workshopSlot, core, out result); _message = result; }
                    GUI.enabled = true;
                }
                GUILayout.EndScrollView();
            }
            GUILayout.Label("COMPILED SPELL DETAILS", _header);
            _tertiaryScroll = GUILayout.BeginScrollView(_tertiaryScroll, GUILayout.Height(118f));
            SpellBehaviorGraph behaviorGraph = world.GetSpellGraph(board.slot);
            string graphSummary = behaviorGraph == null
                ? "BEHAVIOR GRAPH · unavailable"
                : "BEHAVIOR GRAPH · " + behaviorGraph.nodes.Count + " nodes · cost " + behaviorGraph.runtimeCost + " · " +
                  (behaviorGraph.Valid ? "VALID" : "BLOCKED");
            string graphNodes = behaviorGraph == null || behaviorGraph.nodes.Count == 0
                ? "Nodes · none"
                : string.Join("\n", behaviorGraph.nodes.Take(10).Select(node => node.order + " · " + node.kind + " · " + node.label)) +
                  (behaviorGraph.nodes.Count > 10 ? "\n… " + (behaviorGraph.nodes.Count - 10) + " more graph nodes" : string.Empty);
            string executionLayers = spell.executionLayers == null || spell.executionLayers.Count == 0
                ? "Execution · no additional layers"
                : string.Join("\n", spell.executionLayers.Select(layer => "→ " + layer));
            string triggerSummary = spell.triggers == null || spell.triggers.Count == 0
                ? "Unique/Legendary triggers · none"
                : string.Join("\n", spell.triggers.Select(trigger => "Unique/Legendary trigger · " + trigger.moment + " → Slot " + ((int)trigger.linkedSlot + 1)));
            // Keep the GUILayout control tree constant between Layout and Repaint. Runtime graph
            // rebuilds may change the text, but never the number of controls in this group.
            GUILayout.Label(graphSummary, _small);
            GUILayout.Label(graphNodes, _small);
            GUILayout.Label(executionLayers, _small);
            GUILayout.Label(triggerSummary, _small);
            GUILayout.EndScrollView();
            GUILayout.Label(_expertDetails ? spell.ExpertSummary() : validation.CompactSummary, _small);
            GUILayout.Label(validation.warnings.Count > 0 ? "Warnings: " + string.Join(" · ", validation.warnings) : "Warnings: none", _small);
            GUILayout.Label(validation.errors.Count > 0 ? "Blocked: " + string.Join(" · ", validation.errors) : "Blocked: no", _small);
            GUILayout.Label(_message, _body); GUILayout.EndArea();
            if (canEdit && _draggingModifier && Event.current.type == EventType.ScrollWheel)
            {
                _rotation = (_rotation + (Event.current.delta.y > 0f ? 1 : 5)) % 6;
                Event.current.Use();
            }
            if (canEdit && _draggingModifier && Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                _draggingModifier = false; _draggedPlacedModifier = null; _message = "Drag cancelled safely."; Event.current.Use();
            }
            if (_draggingModifier && Event.current.type != EventType.Used)
            {
                Vector2 mouse = ArcaneInput.GuiMousePosition;
                SpellModifierDefinition dragging = DemoCatalog.GetModifier(_selectedModifier);
                GUI.Label(new Rect(mouse.x + 14f, mouse.y + 12f, 220f, 52f), "DRAGGING · ROTATION " + _rotation + "\n" + (dragging == null ? "Support Rune" : dragging.displayName), _center);
                if (_leftPointerReleaseFrame == Time.frameCount && !_dragDropHandled)
                {
                    if (_draggedPlacedModifier != null && side.Contains(mouse))
                    {
                        string returned;
                        if (board.RemoveAt(_draggedPlacedModifier.anchor, out returned))
                        { world.RecalculateModifierAvailability(); _message = "Returned " + DemoCatalog.GetModifier(returned).displayName + " to inventory."; }
                    }
                    else _message = "Drop cancelled; the build was not changed.";
                    _draggingModifier = false; _draggedPlacedModifier = null;
                }
            }
        }

        private void DrawSpellLinks(GameWorld world)
        {
            bool canEdit = world.CanEditSpells;
            Rect panel = CenteredPanel(1040f, 720f);
            GUI.Box(panel, GUIContent.none, _panel);
            GUI.Label(new Rect(panel.x + 24f, panel.y + 14f, 620f, 40f), "SPELL LINKS", _title);
            if (GUI.Button(new Rect(panel.xMax - 270f, panel.y + 18f, 160f, 32f), "OPEN SPELLCRAFT")) OpenWorkshopAnywhere();
            if (GUI.Button(new Rect(panel.xMax - 94f, panel.y + 18f, 70f, 32f), "Close")) CloseModals();

            GUILayout.BeginArea(new Rect(panel.x + 24f, panel.y + 62f, panel.width - 48f, panel.height - 86f));
            GUILayout.Label("Connect a source spell to a destination spell. The destination keeps its complete Spell Board.", _body);
            GUILayout.Label(canEdit ? "ROOM SAFE · LINK EDITING AVAILABLE" : "READ ONLY · " + world.SpellEditLockReason, _header);
            GUILayout.Label("LINK SLOTS " + world.SpellLinks.Links.Count + "/" + world.SpellLinks.Slots + " · Links use their own Trigger Power and cooldown. Cycles are not allowed.", _header);

            if (world.SpellLinks.PendingChoices.Count > 0)
            {
                GUILayout.BeginVertical(_panel);
                GUILayout.Label("SPELL LINK REWARD · CHOOSE ONE CONDITION", _header);
                GUILayout.Label("After choosing, select the source and destination below.", _small);
                GUILayout.BeginHorizontal();
                GUI.enabled = canEdit;
                foreach (SpellLinkCondition condition in world.SpellLinks.PendingChoices.ToArray())
                {
                    if (GUILayout.Button(SpellLinkRules.DisplayName(condition) + "\n" + (SpellLinkRules.TriggerPower(condition) * 100f).ToString("0") +
                        "% power · " + SpellLinkRules.Cooldown(condition).ToString("0.00") + "s", GUILayout.Height(58f)))
                    {
                        string result;
                        if (world.SelectSpellLinkCondition(condition, out result)) _linkCondition = condition;
                        _message = result;
                    }
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.Space(8f);
            }

            if (world.SpellLinks.PendingCondition.HasValue) _linkCondition = world.SpellLinks.PendingCondition.Value;

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(_panel, GUILayout.Width(300f));
            GUILayout.Label("1 · SOURCE SPELL", _header);
            for (int i = 0; i < 3; i++)
            {
                CompiledSpell spell = world.GetSpell((SpellSlot)i);
                GUI.enabled = canEdit && spell != null;
                Color old = GUI.backgroundColor;
                if (_linkSourceSlot == i) GUI.backgroundColor = new Color(0.25f, 0.75f, 1f);
                if (GUILayout.Button("SLOT " + (i + 1) + " · " + (spell == null ? "EMPTY" : spell.displayName), GUILayout.Height(42f))) _linkSourceSlot = i;
                GUI.backgroundColor = old;
                GUI.enabled = true;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_panel, GUILayout.Width(350f));
            GUILayout.Label("2 · CONDITION", _header);
            if (!world.SpellLinks.OwnedConditions.Any()) GUILayout.Label("No Link condition is available. Find a Spell Link room reward.", _body);
            foreach (SpellLinkCondition condition in world.SpellLinks.OwnedConditions)
            {
                GUI.enabled = canEdit;
                Color old = GUI.backgroundColor;
                if (_linkCondition == condition) GUI.backgroundColor = new Color(0.75f, 0.35f, 1f);
                if (GUILayout.Button(SpellLinkRules.DisplayName(condition) + " · " + (SpellLinkRules.TriggerPower(condition) * 100f).ToString("0") +
                    "% · " + SpellLinkRules.Cooldown(condition).ToString("0.00") + "s", GUILayout.Height(36f))) _linkCondition = condition;
                GUI.backgroundColor = old;
            }
            GUI.enabled = true;
            GUILayout.Label(SpellLinkRules.Description(_linkCondition), _small);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_panel, GUILayout.Width(300f));
            GUILayout.Label("3 · DESTINATION SPELL", _header);
            for (int i = 0; i < 3; i++)
            {
                CompiledSpell spell = world.GetSpell((SpellSlot)i);
                GUI.enabled = canEdit && spell != null && i != _linkSourceSlot;
                Color old = GUI.backgroundColor;
                if (_linkDestinationSlot == i) GUI.backgroundColor = new Color(0.75f, 0.35f, 1f);
                if (GUILayout.Button("SLOT " + (i + 1) + " · " + (spell == null ? "EMPTY" : spell.displayName), GUILayout.Height(42f))) _linkDestinationSlot = i;
                GUI.backgroundColor = old;
                GUI.enabled = true;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.enabled = canEdit && world.SpellLinks.OwnedConditions.Contains(_linkCondition) && world.SpellLinks.Links.Count < world.SpellLinks.Slots;
            if (GUILayout.Button("CREATE SPELL LINK", GUILayout.Height(46f)))
            {
                bool completingReward = world.SpellLinks.HasPendingReward;
                string result;
                if (world.CreateSpellLink(_linkSourceSlot, _linkDestinationSlot, _linkCondition, out result) && completingReward)
                { _spellLinksOpen = false; SyncPause(); }
                _message = result;
            }
            GUI.enabled = true;
            GUI.enabled = canEdit;
            if (world.SpellLinks.PendingCondition.HasValue && GUILayout.Button("KEEP THIS CONDITION AND LINK IT LATER", GUILayout.Height(30f)))
            {
                world.KeepPendingLinkConditionForLater();
                _spellLinksOpen = false;
                SyncPause();
            }
            GUI.enabled = true;

            GUILayout.Space(8f);
            GUILayout.Label("ACTIVE LINKS", _header);
            _tertiaryScroll = GUILayout.BeginScrollView(_tertiaryScroll, GUILayout.Height(150f));
            if (world.SpellLinks.Links.Count == 0) GUILayout.Label("No active links. Spells can still be cast normally.", _body);
            foreach (SpellLinkSave link in world.SpellLinks.Links.ToArray())
            {
                CompiledSpell source = world.GetSpell((SpellSlot)link.sourceSlot);
                CompiledSpell destination = world.GetSpell((SpellSlot)link.destinationSlot);
                GUILayout.BeginHorizontal(_panel);
                GUILayout.Label((source == null ? "Empty" : source.displayName) + "  — " + SpellLinkRules.DisplayName(link.condition) + " →  " +
                    (destination == null ? "Empty" : destination.displayName) + "\n" + (SpellLinkRules.TriggerPower(link.condition) * 100f).ToString("0") +
                    "% Trigger Power · " + SpellLinkRules.Cooldown(link.condition).ToString("0.00") + "s cooldown" +
                    (world.SpellLinks.CooldownRemaining(link) > 0f ? " · READY IN " + world.SpellLinks.CooldownRemaining(link).ToString("0.0") + "s" : " · READY"), _body);
                GUI.enabled = canEdit;
                if (GUILayout.Button("REMOVE", GUILayout.Width(90f), GUILayout.Height(48f))) { string result; world.SpellLinks.Remove(link.instanceId, out result); _message = result; }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.Label(_message, _body);
            GUILayout.EndArea();
        }

        private void DrawHexBoard(GameWorld world, SpellBoard board, Vector2 center, float size, bool editable)
        {
            Dictionary<HexCoord, PlacedModifier> occupied = board.GetOccupiedMap();
            HashSet<PlacedModifier> active = new HashSet<PlacedModifier>(board.GetActivePlacements());
            Event current = Event.current;
            Vector2 globalPointer = ArcaneInput.GuiMousePosition;
            HexCoord hoverCell = new HexCoord(); bool hasHover = false;
            foreach (HexCoord possible in board.AllCells())
            {
                Vector2 possiblePoint = HexToScreen(possible, center, size);
                if (PointInsideHex(globalPointer, possiblePoint, size * 0.92f)) { hoverCell = possible; hasHover = true; break; }
            }
            HashSet<HexCoord> previewCells = new HashSet<HexCoord>(); bool previewValid = false; bool previewConnected = false; string previewReason = string.Empty;
            if (editable && _draggingModifier && hasHover && !hoverCell.Equals(new HexCoord(0, 0)))
            {
                SpellModifierDefinition dragDefinition = DemoCatalog.GetModifier(_selectedModifier);
                if (dragDefinition != null)
                {
                    previewCells = new HashSet<HexCoord>(SpellBoard.GetOccupiedCells(dragDefinition, hoverCell, _rotation));
                    previewValid = board.CanPlace(_selectedModifier, hoverCell, _rotation, _draggedPlacedModifier, out previewReason);
                    if (previewValid)
                    {
                        SpellBoard previewBoard = board.CreatePlacementPreview(_selectedModifier, hoverCell, _rotation, _draggedPlacedModifier);
                        if (previewBoard != null)
                        {
                            PlacedModifier candidate = previewBoard.placed.LastOrDefault();
                            previewConnected = candidate != null && previewBoard.IsPlacementConnected(candidate);
                            _placementPreviewSpell = SpellCompiler.Compile(previewBoard, world.Stats, world.Equipment);
                            if (!previewConnected) previewReason = "This drop is allowed, but the rune will be disconnected and provide no effect.";
                        }
                    }
                }
            }
            foreach (HexCoord cell in board.AllCells().OrderBy(c => c.r).ThenBy(c => c.q))
            {
                Vector2 point = HexToScreen(cell, center, size); Rect rect = new Rect(point.x - size, point.y - size, size * 2f, size * 2f);
                bool isCore = cell.Equals(new HexCoord(0, 0)); PlacedModifier piece; occupied.TryGetValue(cell, out piece);
                bool sealedCell = !board.IsCellUnlocked(cell);
                Color color = sealedCell ? new Color(0.045f, 0.055f, 0.075f) : new Color(0.12f, 0.15f, 0.22f);
                string label = sealedCell ? "LOCKED\nLV " + RequiredSpellLevel(cell) : string.Empty;
                if (isCore)
                {
                    SpellCoreDefinition core = DemoCatalog.GetCore(board.coreId); color = core.color; label = "CORE\n" + ShortName(core.displayName);
                }
                else if (piece != null)
                {
                    SpellModifierDefinition definition = DemoCatalog.GetModifier(piece.modifierId); color = active.Contains(piece) ? definition.uiColor : Color.gray * 0.65f;
                    label = piece.anchor.Equals(cell) ? ShortName(definition.displayName) : "◆";
                }
                if (_draggingModifier && previewCells.Contains(cell))
                {
                    color = previewValid && previewConnected ? new Color(0.1f, 0.95f, 1f, 0.9f) : new Color(1f, 0.12f, 0.18f, 0.9f);
                    label = piece == _draggedPlacedModifier ? "MOVE" : "+";
                }
                Color previous = GUI.color; GUI.color = color; GUI.DrawTexture(rect, _hexTexture, ScaleMode.StretchToFill, true); GUI.color = Color.white; GUI.Label(rect, label, _center); GUI.color = previous;
                if (piece != null && piece.anchor.Equals(cell)) DrawConnectors(piece, center, size);
                if (isCore) DrawCoreOutputs(point, size * 0.78f);

                bool inside = rect.Contains(globalPointer) && PointInsideHex(globalPointer, point, size * 0.92f);
                if (editable && _draggingModifier && _leftPointerReleaseFrame == Time.frameCount && inside && !isCore)
                {
                    int count; world.ModifierInventory.TryGetValue(_selectedModifier, out count);
                    string result;
                    if (_draggedPlacedModifier != null)
                    {
                        if (board.TryMove(_draggedPlacedModifier, cell, _rotation, out result)) world.RecalculateModifierAvailability();
                        _message = result;
                    }
                    else if (count <= 0) _message = "No unused copy remains.";
                    else { if (board.TryPlace(_selectedModifier, cell, _rotation, out result)) world.RecalculateModifierAvailability(); _message = result; }
                    _draggingModifier = false; _draggedPlacedModifier = null; _dragDropHandled = true;
                }
                else if (editable && current.type == EventType.MouseDown && inside)
                {
                    if (current.button == 1 && !isCore && piece != null)
                    {
                        string returned; if (board.RemoveAt(cell, out returned)) { world.RecalculateModifierAvailability(); _message = "Removed " + DemoCatalog.GetModifier(returned).displayName + " and returned it to available Support Runes."; }
                    }
                    else if (current.button == 0 && !isCore && piece != null && current.shift)
                    {
                        string result; board.RotateAt(cell, 1, out result); world.RecalculateModifierAvailability(); _message = result;
                    }
                    else if (current.button == 0 && !isCore && piece != null)
                    {
                        _selectedModifier = piece.modifierId; _rotation = piece.rotation; _draggedPlacedModifier = piece; _draggingModifier = true;
                        _message = "Dragging an installed Support Rune. Drop it on the board to move it or over the inventory to remove it.";
                    }
                    else if (current.button == 0 && !isCore && piece == null)
                    {
                        if (sealedCell) { _message = "That cell unlocks at Spell Level " + RequiredSpellLevel(cell) + "."; current.Use(); continue; }
                        if (!ProfileManager.Current.accessibility.clickPlacementAlternative)
                            _message = "Click placement is disabled. Drag a Support Rune from the inventory list.";
                        else
                        {
                            int count; world.ModifierInventory.TryGetValue(_selectedModifier, out count);
                            if (count <= 0) _message = "No unused copy remains.";
                            else { string result; if (board.TryPlace(_selectedModifier, cell, _rotation, out result)) world.RecalculateModifierAvailability(); _message = result; }
                        }
                    }
                    current.Use();
                }
            }
            if (editable && _draggingModifier && hasHover) _message = previewValid && previewConnected ? "Connected drop · release to commit." : previewReason;
            GUI.Label(new Rect(center.x - 330f, center.y + 270f, 660f, 42f), editable
                ? "Drag or click Support Runes · Q/E or Wheel rotates · Right-click removes or cancels\nWhite input touches the Spell Core or a cyan output. Gray runes are disconnected and cost no Capacity."
                : "READ ONLY · Clear the room to install, move, rotate, or remove Support Runes.", _center);
        }

        private void DrawRunInventory(GameWorld world, RunDirector run)
        {
            Rect panel = CenteredPanel(1180f, 790f); GUI.Box(panel, GUIContent.none, _panel);
            GUI.Label(new Rect(panel.x + 26f, panel.y + 16f, 760f, 40f), "UNSECURED RUN BAG", _title);
            GUI.Label(new Rect(panel.x + 450f, panel.y + 21f, 590f, 28f), "LOADOUT LOCKED · EXTRACT TO KEEP EQUIPMENT", _header);
            if (GUI.Button(new Rect(panel.xMax - 96f, panel.y + 18f, 70f, 32f), "Close")) CloseModals();

            Rect left = new Rect(panel.x + 24f, panel.y + 68f, 325f, panel.height - 92f);
            Rect middle = new Rect(left.xMax + 12f, left.y, 410f, left.height);
            Rect right = new Rect(middle.xMax + 12f, left.y, panel.xMax - middle.xMax - 36f, left.height);
            GUI.Box(left, GUIContent.none, _panel); GUI.Box(middle, GUIContent.none, _panel); GUI.Box(right, GUIContent.none, _panel);

            GUILayout.BeginArea(Inset(left, 14f));
            GUILayout.Label("LOCKED STARTING LOADOUT", _header);
            GUILayout.Label("Equipment cannot be changed during this expedition.", _small);
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                ItemInstance item; world.Equipment.equipped.TryGetValue(slot, out item);
                GUILayout.Label(slot + "\n" + (item == null ? "— empty —" : ItemLabel(item)), _button, GUILayout.Height(50f));
            }
            GUILayout.EndScrollView();
            GUILayout.Label(world.Equipment.SetBonusSummary(), _small);
            GUILayout.EndArea();

            GUILayout.BeginArea(Inset(middle, 14f));
            GUILayout.Label("UNSECURED ITEMS · " + world.Equipment.runBag.Count, _header);
            GUILayout.Label(run.Gold + " Gold · " + run.ForgeDust + " Dust · " + run.BindingRunes + " Runes · " + run.CorruptionCores + " Cores", _small);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("LOOT FILTER · " + (ItemRarity)Mathf.Clamp(ProfileManager.Current.minimumLootRarity, 0, 3), GUILayout.Height(28f)))
            { ProfileManager.Current.minimumLootRarity = (ProfileManager.Current.minimumLootRarity + 1) % 4; ProfileManager.Save(); }
            if (GUILayout.Button("SALVAGE SAFE JUNK", GUILayout.Height(28f))) SalvageSafeJunk(world, run);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(); GUILayout.Label("SEARCH", GUILayout.Width(52f)); _itemSearch = GUILayout.TextField(_itemSearch); GUILayout.EndHorizontal();
            _secondaryScroll = GUILayout.BeginScrollView(_secondaryScroll);
            IEnumerable<ItemInstance> runItems = world.Equipment.runBag.Where(item => item != null && item.Definition != null)
                .Where(item => string.IsNullOrEmpty(_itemSearch) || item.DisplayName.IndexOf(_itemSearch, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.affixes.Any(affix => V11Itemization.FormatAffix(affix, false).IndexOf(_itemSearch, StringComparison.OrdinalIgnoreCase) >= 0))
                .OrderByDescending(item => item.favorite).ThenByDescending(item => item.rarity).ThenByDescending(item => item.itemLevel);
            foreach (ItemInstance item in runItems.ToArray())
            {
                Color old = GUI.backgroundColor;
                GUI.backgroundColor = item.Definition.color;
                string flags = "UNSECURED · " + (item.favorite ? "★ " : string.Empty) + (item.junk ? "JUNK · " : string.Empty);
                if (GUILayout.Button(flags + ItemLabel(item), GUILayout.Height(72f))) _selectedRunItem = item;
                GUI.backgroundColor = old;
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(Inset(right, 14f));
            GUILayout.Label("ITEM DETAILS", _header);
            if (_selectedRunItem == null || !world.Equipment.runBag.Contains(_selectedRunItem))
            {
                _selectedRunItem = null;
                GUILayout.Label("Select an unsecured item to inspect it. Items and salvage materials are lost if the expedition fails.", _body);
            }
            else
            {
                _tertiaryScroll = GUILayout.BeginScrollView(_tertiaryScroll, GUILayout.Height(right.height - 220f));
                GUILayout.Label(V11Itemization.BuildTooltip(_selectedRunItem, _expertDetails), _body);
                GUILayout.Space(8f);
                GUILayout.Label("EXACT LOADOUT CHANGES", _header);
                GUILayout.Label(ItemComparison(world, _selectedRunItem), _small);
                GUILayout.EndScrollView();
                _expertDetails = GUILayout.Toggle(_expertDetails, "Show affix tiers and ranges");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(_selectedRunItem.favorite ? "UNFAVORITE" : "FAVORITE", GUILayout.Height(34f)))
                { _selectedRunItem.favorite = !_selectedRunItem.favorite; if (_selectedRunItem.favorite) _selectedRunItem.junk = false; run.SaveRunCheckpoint(); }
                if (GUILayout.Button(_selectedRunItem.junk ? "KEEP" : "MARK JUNK", GUILayout.Height(34f)))
                { _selectedRunItem.junk = !_selectedRunItem.junk; if (_selectedRunItem.junk) _selectedRunItem.favorite = false; run.SaveRunCheckpoint(); }
                GUILayout.EndHorizontal();
                bool confirmationNeeded = _selectedRunItem.favorite || _selectedRunItem.locked || _selectedRunItem.corrupted ||
                    _selectedRunItem.rarity == ItemRarity.Rare || _selectedRunItem.rarity == ItemRarity.Unique;
                bool confirmed = _pendingSalvageInstanceId == _selectedRunItem.instanceId;
                string salvageLabel = confirmationNeeded && !confirmed ? "REVIEW SALVAGE" : "CONFIRM SALVAGE";
                if (GUILayout.Button(salvageLabel + "\nGain run Gold and unsecured Forge materials", GUILayout.Height(54f)))
                {
                    if (confirmationNeeded && !confirmed)
                    { _pendingSalvageInstanceId = _selectedRunItem.instanceId; _message = "Press Confirm Salvage again. This item will be destroyed."; }
                    else
                    {
                        ItemInstance salvaged = _selectedRunItem;
                        string result;
                        if (world.Equipment.SalvageRunItem(salvaged, run, true, out result))
                        { _selectedRunItem = null; _pendingSalvageInstanceId = null; run.SaveRunCheckpoint(); }
                        _message = result;
                    }
                }
            }
            GUILayout.Space(8f);
            GUILayout.Label(_message, _body);
            GUILayout.EndArea();
        }

        private void SalvageSafeJunk(GameWorld world, RunDirector run)
        {
            int count = 0;
            foreach (ItemInstance item in world.Equipment.runBag.Where(value => value != null && value.junk && !value.favorite && !value.locked &&
                !value.corrupted && value.rarity != ItemRarity.Rare && value.rarity != ItemRarity.Unique).ToArray())
            {
                string ignored;
                if (world.Equipment.SalvageRunItem(item, run, false, out ignored)) count++;
            }
            if (count > 0) run.SaveRunCheckpoint();
            _message = count == 0 ? "No safe Common or Magic junk was marked." : "Salvaged " + count + " junk item(s). Materials remain unsecured until extraction.";
        }

        private void DrawCoreSatchelInstall(GameWorld world, Rect area)
        {
            GUILayout.BeginArea(area); _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            foreach (CoreSaveData core in world.RunCoreSatchel.ToArray())
            {
                SpellCoreDefinition definition = DemoCatalog.GetCore(core.coreId);
                if (GUILayout.Button("Install " + definition.displayName + "\n" + definition.description, GUILayout.Height(60f)))
                {
                    string result; world.InstallCore(_workshopSlot, core, out result); _message = result;
                }
            }
            GUILayout.EndScrollView(); GUILayout.Label(_message, _body); GUILayout.EndArea();
        }

        private void DrawSpellPreview(CompiledSpell spell, CompiledSpell preview, Rect area)
        {
            CompiledSpell shown = preview ?? spell;
            GUI.Box(area, GUIContent.none, _panel); GUI.Label(new Rect(area.x + 10f, area.y + 5f, 530f, 18f), preview == null ? "CURRENT SPELL · * maximum assumes every projectile/repeat hits" : "BEFORE / AFTER DROP PREVIEW · * theoretical maximum", _small);
            GUI.Label(new Rect(area.x + 10f, area.y + 23f, 520f, 18f), "CURRENT  " + PreviewStats(spell), _small);
            if (preview != null) GUI.Label(new Rect(area.x + 10f, area.y + 42f, 520f, 18f), "AFTER      " + PreviewStats(preview), _small);
            else GUI.Label(new Rect(area.x + 10f, area.y + 42f, 520f, 18f), spell.delivery + " · " + spell.element + " · " + spell.projectilePattern, _small);
            float phase = (Mathf.Sin(Time.unscaledTime * (shown.homingStrength > 0f ? 3f : 1.8f)) + 1f) * 0.5f;
            float x = Mathf.Lerp(area.x + 550f, area.xMax - 32f, phase); float y = area.center.y + Mathf.Sin(Time.unscaledTime * 5f) * shown.arcAmount * 6f;
            Color previous = GUI.color; GUI.color = shown.primaryColor;
            float scale = Mathf.Clamp(18f * shown.size, 10f, 44f); GUI.DrawTexture(new Rect(x - scale * 0.5f, y - scale * 0.5f, scale, scale), _circleTexture); GUI.color = previous;
        }

        private static string PreviewStats(CompiledSpell spell)
        {
            if (spell == null) return "Unavailable";
            float theoretical = spell.damage * Mathf.Max(1, spell.projectileCount) * Mathf.Max(1, spell.repeatCount);
            return spell.displayName + " | " + spell.damage.ToString("0") + "/hit | " + Mathf.Max(1, spell.projectileCount) + " projectile(s) | max " +
                theoretical.ToString("0") + "* | " + spell.element + " | " + spell.manaCost.ToString("0.0") + " mana | " + spell.cooldown.ToString("0.00") + "s";
        }

        private void DrawHelp()
        {
            Rect panel = CenteredPanel(940f, 700f); GUI.Box(panel, GUIContent.none, _panel);
            GUI.Label(new Rect(panel.x + 26f, panel.y + 18f, 720f, 40f), "ARCANIST FIELD MANUAL", _title);
            if (GUI.Button(new Rect(panel.xMax - 96f, panel.y + 20f, 70f, 32f), "Close")) { _helpOpen = false; SyncPause(); }
            GUILayout.BeginArea(new Rect(panel.x + 30f, panel.y + 72f, panel.width - 60f, panel.height - 96f));
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);
            HelpSection("Combat", "WASD moves. Aim with the mouse. Left Mouse casts Slot 1, Right Mouse casts Slot 2, Q casts Slot 3, and Space dodges. Telegraphs communicate melee, charges, volleys, hazards, shields, healing, summoning, and boss phases.");
            HelpSection("Spellcraft · Tab", "The Spell Board is read-only during combat. Clear the room, then drag or click a Support Rune onto the board. Rotate with Q/E or the Mouse Wheel. Right Mouse cancels or removes safely. Undo, Redo and live previews remain available.");
            HelpSection("Spell Links · " + ProfileManager.Current.controls.spellLinks, "Spell Links are separate from the Spell Board. Choose a source spell, a condition, and a destination spell. Links cannot form a cycle and use their own power and cooldown.");
            HelpSection("Support Rune ownership", "A Support Rune installed on one spell is unavailable to the other two. Dungeon Support Runes vanish after the run. Spell Copies, equipment and Forge materials become permanent only after extraction. Essence and boss Legendary Shards are safe immediately; Gold always vanishes.");
            HelpSection("Triggered spells", "A Spell Link casts the complete customized destination spell at its listed Trigger Power. Each link has a cooldown, each source cast activates a link at most once, and standard links cannot form cycles.");
            HelpSection("Legendary and Unique", "Only one crafted Legendary Spell may be active. Its Legendary Effect is fixed while compatible Support Runes remain customizable. Item-only Unique Spells do not count against that limit. Unique equipment can still change Spell Cores and other rules.");
            HelpSection("Locked equipment loadout", "Choose all ten equipment slots at Home Base. Two-handed weapons disable the Offhand slot. The complete loadout locks when an expedition begins. Found equipment is unsecured and may be inspected or salvaged, but it cannot be equipped until extraction.");
            HelpSection("Beginner path", "Start with damage, an elemental conversion, and one movement Support Rune. Follow the connected route from the Spell Core and stay within Capacity. Learn Spell Links separately after finding the first Link reward.");
            GUILayout.Label("F1 toggles this guide. Escape closes manual panels. Combat pauses while a construction or decision panel is open.", _center);
            GUILayout.EndScrollView(); GUILayout.EndArea();
        }

        private void HelpSection(string title, string text) { GUILayout.Label(title, _header); GUILayout.Label(text, _body); GUILayout.Space(9f); }

        private void SetPreparedSpell(int slot, string id, bool relic)
        {
            if (relic)
            {
                PreparedSpellSave otherRelic = ProfileManager.Current.preparedSpells.FirstOrDefault(s => s.isRelic && s.slotIndex != slot);
                if (otherRelic != null) { _message = "Only one Legendary Spell may be active. Remove " + MegaCatalog.GetRelic(otherRelic.contentId).displayName + " first."; return; }
            }
            else
            {
                int available = ProfileManager.Current.spellArchive.Count(c => c.coreId == id);
                int usedElsewhere = ProfileManager.Current.preparedSpells.Count(s => !s.isRelic && s.slotIndex != slot && s.contentId == id);
                if (usedElsewhere >= available) { _message = "Another " + DemoCatalog.GetCore(id).displayName + " Spell Copy is required for that second starting slot."; return; }
            }
            ProfileManager.Current.preparedSpells.RemoveAll(s => s.slotIndex == slot);
            ProfileManager.Current.preparedSpells.Add(new PreparedSpellSave { slotIndex = slot, contentId = id, isRelic = relic });
            ProfileManager.Save(); _message = "Spell Slot " + (slot + 1) + " preparation updated.";
        }

        private void AddPreparedModifier(SpellModifierDefinition modifier)
        {
            if (PreparationSpent() + modifier.preparationCost > ProfileManager.Current.PreparationBudget) { _message = "Not enough Loadout Points."; return; }
            PreparedModifierSave selected = ProfileManager.Current.preparedModifiers.FirstOrDefault(m => m.modifierId == modifier.id);
            if (selected == null) ProfileManager.Current.preparedModifiers.Add(new PreparedModifierSave { modifierId = modifier.id, count = 1 }); else selected.count++;
            ProfileManager.Save(); _message = "Added a starting copy of " + modifier.displayName + ".";
        }

        private void RemovePreparedModifier(string id)
        {
            PreparedModifierSave selected = ProfileManager.Current.preparedModifiers.FirstOrDefault(m => m.modifierId == id);
            if (selected == null) return; selected.count--; if (selected.count <= 0) ProfileManager.Current.preparedModifiers.Remove(selected); ProfileManager.Save();
        }

        private int PreparationSpent()
        {
            int total = 0;
            foreach (PreparedModifierSave selected in ProfileManager.Current.preparedModifiers)
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(selected.modifierId); if (definition != null) total += definition.preparationCost * selected.count;
            }
            return total;
        }

        private int CountPreparedModifiers() { return ProfileManager.Current.preparedModifiers.Sum(m => m.count); }

        private void SaveSpellLayout(SpellBoard board)
        {
            if (string.IsNullOrWhiteSpace(_layoutName)) _layoutName = "Spell Layout " + (ProfileManager.Current.savedSpellLayouts.Count + 1);
            SavedSpellLayout existing = ProfileManager.Current.savedSpellLayouts.FirstOrDefault(l => l.name == _layoutName);
            if (existing != null) ProfileManager.Current.savedSpellLayouts.Remove(existing);
            int cap = 3 + ProfileManager.Current.archiveToolRank * 3;
            if (ProfileManager.Current.savedSpellLayouts.Count >= cap) { _message = "Saved-layout capacity reached. Upgrade Archive Tools."; return; }
            ProfileManager.Current.savedSpellLayouts.Add(board.CreateLayout(_layoutName));
            CompiledSpell spell = GameWorld.Instance.GetSpell(board.slot);
            SpellBuildValidationReport report = SpellBuildValidator.Validate(board, GameWorld.Instance.Stats, GameWorld.Instance.Equipment);
            if (spell != null) ProfileManager.RecordBuild(new BuildHistorySave
            {
                stableId = report.stableId, discoveredUtc = DateTime.UtcNow.ToString("o"), name = _layoutName,
                coreId = spell.coreId, element = spell.element.ToString(), delivery = spell.delivery.ToString(),
                activeModifiers = report.activePieces, triggers = report.triggerCount, estimatedDps = report.estimatedDps
            });
            ProfileManager.Save(); _message = "Saved and validated build: " + _layoutName + ".";
        }

        private void LoadSpellLayout(GameWorld world, SpellBoard board, SavedSpellLayout layout)
        {
            Dictionary<string, int> required = layout.pieces.GroupBy(p => p.modifierId).ToDictionary(g => g.Key, g => g.Count());
            foreach (KeyValuePair<string, int> pair in required)
            {
                int installedElsewhere = Enumerable.Range(0, 3).Select(i => world.GetBoard((SpellSlot)i)).Where(b => b != null && b != board).Sum(b => b.placed.Count(p => p.modifierId == pair.Key));
                int owned; world.OwnedModifierCounts.TryGetValue(pair.Key, out owned);
                if (owned - installedElsewhere < pair.Value) { _message = "This build needs more copies of " + DemoCatalog.GetModifier(pair.Key).displayName + "."; return; }
            }
            board.LoadLayout(layout); world.RecalculateModifierAvailability(); _message = "Loaded " + layout.name + ".";
        }

        private void SaveEquipmentLoadout(GameWorld world, string name)
        {
            ProfileManager.Current.equipmentLoadouts.RemoveAll(l => l.name == name);
            EquipmentLoadoutSave loadout = new EquipmentLoadoutSave { name = name };
            foreach (KeyValuePair<EquipmentSlot, ItemInstance> pair in world.Equipment.equipped)
                if (pair.Value.banked) loadout.slots.Add(new SlotItemSave { slot = pair.Key, itemInstanceId = pair.Value.instanceId });
            ProfileManager.Current.equipmentLoadouts.Add(loadout); ProfileManager.Save(); _message = name + " saved.";
        }

        private void LoadEquipmentLoadout(GameWorld world, EquipmentLoadoutSave loadout)
        {
            Dictionary<EquipmentSlot, ItemInstance> original = world.Equipment.equipped.ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot))) world.Equipment.Unequip(slot, out _);
            List<KeyValuePair<EquipmentSlot, ItemInstance>> target = new List<KeyValuePair<EquipmentSlot, ItemInstance>>();
            foreach (SlotItemSave saved in loadout.slots)
            {
                ItemInstance item = world.Equipment.backpack.FirstOrDefault(i => i.instanceId == saved.itemInstanceId);
                if (item == null)
                {
                    RestoreEquipmentSnapshot(world, original);
                    _message = loadout.name + " could not be equipped because one saved item is missing. Your previous equipment was restored.";
                    return;
                }
                target.Add(new KeyValuePair<EquipmentSlot, ItemInstance>(saved.slot, item));
            }
            foreach (KeyValuePair<EquipmentSlot, ItemInstance> pair in target.OrderBy(pair => pair.Key == EquipmentSlot.Weapon ? 0 : pair.Key == EquipmentSlot.Offhand ? 2 : 1))
            {
                string result;
                if (world.Equipment.Equip(pair.Value, out result)) continue;
                RestoreEquipmentSnapshot(world, original);
                _message = loadout.name + " was invalid. Your previous equipment was restored.";
                return;
            }
            world.Equipment.SaveSanctuaryEquipment(); _message = loadout.name + " equipped.";
        }

        private static void RestoreEquipmentSnapshot(GameWorld world, Dictionary<EquipmentSlot, ItemInstance> snapshot)
        {
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot))) { string ignored; world.Equipment.Unequip(slot, out ignored); }
            foreach (KeyValuePair<EquipmentSlot, ItemInstance> pair in snapshot.OrderBy(pair => pair.Key == EquipmentSlot.Weapon ? 0 : pair.Key == EquipmentSlot.Offhand ? 2 : 1))
            {
                ItemInstance item = world.Equipment.backpack.FirstOrDefault(value => value.instanceId == pair.Value.instanceId);
                string ignored; if (item != null) world.Equipment.Equip(item, out ignored);
            }
        }

        private int UpgradeRank(string key)
        {
            ProfileData p = ProfileManager.Current;
            if (key == "Health") return p.healthRank; if (key == "Mana") return p.manaRank; if (key == "Power") return p.powerRank;
            if (key == "StartingSpells") return p.startingSpellRank; if (key == "Preparation") return p.preparationRank;
            if (key == "Rerolls") return p.rewardRerollRank; return p.archiveToolRank;
        }

        private string DescribeCodexEntry(string entry)
        {
            string[] parts = entry.Split(':'); if (parts.Length != 2) return entry;
            if (parts[0] == "core") { SpellCoreDefinition d = DemoCatalog.GetCore(parts[1]); return d == null ? entry : "SPELL · " + d.displayName + " — " + d.description; }
            if (parts[0] == "modifier") { SpellModifierDefinition d = DemoCatalog.GetModifier(parts[1]); return d == null ? entry : "SUPPORT RUNE · " + d.FullDisplayName.Replace("\n", " · ") + " — " + d.shortDescription; }
            if (parts[0] == "item") { ItemDefinition d = DemoCatalog.GetItem(parts[1]); return d == null ? entry : "ITEM · " + d.displayName + " — " + d.description; }
            if (parts[0] == "relic") { RelicDefinition d = MegaCatalog.GetRelic(parts[1]); return d == null ? entry : "LEGENDARY SPELL · " + d.displayName + " — " + d.signatureRule; }
            if (parts[0] == "enemy")
            {
                EnemyArchetype enemy;
                return Enum.TryParse(parts[1], true, out enemy) ? "ENEMY · " + enemy.ToString().Replace("OssuaryWarden", "Dungeon Warden") + " — " + EnemyBestiary.Describe(enemy) : entry;
            }
            return parts[0].ToUpperInvariant() + " · " + parts[1].Replace('_', ' ');
        }

        private static string ItemLabel(ItemInstance item)
        {
            return item.DisplayName + " · " + item.rarity + " iLvl " + item.itemLevel + (item.upgradeLevel > 0 ? " +" + item.upgradeLevel : string.Empty) +
                   (item.corrupted ? " · CORRUPTED" : string.Empty) + (item.favorite ? " · ★" : string.Empty) + (string.IsNullOrEmpty(item.AffixSummary()) ? string.Empty : "\n" + item.AffixSummary());
        }

        private static string ItemComparison(GameWorld world, ItemInstance candidate)
        {
            if (world == null) return "No comparison";
            return V11Itemization.BuildComparison(world.Equipment, candidate, ProfileManager.Current);
        }

        private static KeyCode NextKey(KeyCode current, KeyCode[] choices)
        {
            int index = Array.IndexOf(choices, current);
            return choices[(index + 1 + choices.Length) % choices.Length];
        }

        private static string SplitWords(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            System.Text.StringBuilder result = new System.Text.StringBuilder(value.Length + 8);
            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0 && char.IsUpper(value[i]) && !char.IsUpper(value[i - 1])) result.Append(' ');
                result.Append(value[i]);
            }
            return result.ToString();
        }

        private void DrawConnectors(PlacedModifier piece, Vector2 boardCenter, float hexSize)
        {
            SpellModifierDefinition definition = DemoCatalog.GetModifier(piece.modifierId); int input = SpellBoard.RotateSide(definition.inputSide, piece.rotation);
            Color inputColor = ProfileManager.Current.accessibility.colorblindConnectors ? new Color(1f, 0.8f, 0.1f) : new Color(1f, 0.15f, 0.2f);
            Color outputColor = ProfileManager.Current.accessibility.colorblindConnectors ? new Color(0.2f, 0.55f, 1f) : new Color(0.1f, 0.95f, 1f);
            Vector2 inputCenter = HexToScreen(piece.anchor, boardCenter, hexSize);
            DrawConnector(inputCenter, hexSize * 0.78f, input, inputColor, 12f, true);
            foreach (int output in definition.outputSides)
            {
                int absoluteSide = SpellBoard.RotateSide(output, piece.rotation);
                HexCoord portCell = SpellBoard.GetOutputPortCell(piece, absoluteSide);
                Vector2 outputCenter = HexToScreen(portCell, boardCenter, hexSize);
                DrawConnector(outputCenter, hexSize * 0.78f, absoluteSide, outputColor, 10f, false);
            }
        }

        private void DrawCoreOutputs(Vector2 center, float radius) { for (int side = 0; side < 6; side++) DrawConnector(center, radius, side, Color.white, 8f); }

        private void DrawConnector(Vector2 center, float radius, int side, Color color, float diameter, bool input = false)
        {
            float angle = side * 60f * Mathf.Deg2Rad; Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            bool accessible = ProfileManager.Current != null && ProfileManager.Current.accessibility.colorblindConnectors;
            Color previous = GUI.color; GUI.color = color;
            GUI.DrawTexture(new Rect(point.x - diameter * 0.5f, point.y - diameter * 0.5f, diameter, diameter), accessible && input ? Texture2D.whiteTexture : _circleTexture);
            if (accessible && !input)
            {
                GUI.color = new Color(0.04f, 0.05f, 0.07f);
                GUI.DrawTexture(new Rect(point.x - diameter * 0.16f, point.y - diameter * 0.16f, diameter * 0.32f, diameter * 0.32f), _circleTexture);
            }
            GUI.color = previous;
        }

        private void DrawBar(Rect rect, float value, Color color, string label)
        {
            GUI.Box(rect, GUIContent.none); Color previous = GUI.color; GUI.color = color;
            GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, (rect.width - 4f) * Mathf.Clamp01(value), rect.height - 4f), Texture2D.whiteTexture);
            GUI.color = previous; GUI.Label(rect, label, _center);
        }

        private void CloseModals()
        {
            if (GameWorld.Instance != null && GameWorld.Instance.HasPendingSpellSystemReward)
            {
                _message = "Finish assigning the room reward before closing this screen.";
                if (GameWorld.Instance.PendingSpellUpgrade) OpenWorkshopAnywhere(); else OpenSpellLinksAnywhere();
                return;
            }
            CancelTransientDrags();
            _workshopOpen = _spellLinksOpen = _inventoryOpen = _helpOpen = _pauseOpen = false;
            SaveCurrentRunCheckpoint();
            SyncPause();
        }

        private void CancelTransientDrags()
        {
            _draggingModifier = false;
            _draggedPlacedModifier = null;
            _dragDropHandled = false;
        }

        private static void SaveCurrentRunCheckpoint()
        {
            GameWorld world = GameWorld.Instance;
            if (world == null || !world.RunActive || world.TrainingMode) return;
            RunDirector run = world.GetComponent<RunDirector>();
            if (run != null) run.SaveRunCheckpoint();
        }

        private void SyncPause() { if (GameWorld.Instance != null && GameWorld.Instance.RunActive) Time.timeScale = ModalOpen ? 0f : 1f; }

        private Rect CenteredPanel(float width, float height)
        {
            width = Mathf.Min(width, Screen.width - 24f); height = Mathf.Min(height, Screen.height - 24f);
            return new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
        }

        private static Rect Inset(Rect rect, float amount) { return new Rect(rect.x + amount, rect.y + amount, rect.width - amount * 2f, rect.height - amount * 2f); }

        private static Vector2 HexToScreen(HexCoord coord, Vector2 center, float radius)
        {
            return center + new Vector2(radius * Mathf.Sqrt(3f) * (coord.q + coord.r * 0.5f), radius * 1.5f * coord.r);
        }

        private static bool PointInsideHex(Vector2 point, Vector2 center, float radius)
        {
            Vector2 delta = point - center; delta.x = Mathf.Abs(delta.x); delta.y = Mathf.Abs(delta.y);
            return delta.x <= Mathf.Sqrt(3f) * radius * 0.5f && delta.y <= radius && Mathf.Sqrt(3f) * delta.y + delta.x <= Mathf.Sqrt(3f) * radius;
        }

        private static string ShortName(string value)
        {
            string[] words = value.Split(' '); if (words.Length == 1) return value.Length <= 8 ? value.ToUpperInvariant() : value.Substring(0, 7).ToUpperInvariant();
            return string.Join("\n", words.Take(2).Select(w => w.Length <= 7 ? w.ToUpperInvariant() : w.Substring(0, 6).ToUpperInvariant()));
        }

        private static int RequiredSpellLevel(HexCoord cell)
        {
            return SpellBoard.RequiredLevelForCell(cell);
        }

        private static string FriendlyRoomType(DungeonRoomType type)
        {
            switch (type)
            {
                case DungeonRoomType.Combat: return "Combat Room";
                case DungeonRoomType.Elite: return "Elite Fight";
                case DungeonRoomType.ModifierReward: return "Support Rune Reward";
                case DungeonRoomType.SpellCoreReward: return "Spell Core Reward";
                case DungeonRoomType.EquipmentReward: return "Equipment Reward";
                case DungeonRoomType.TreasureVault: return "Treasure Room";
                case DungeonRoomType.Shop: return "Shop";
                case DungeonRoomType.SafeWorkshop: return "Safe Room";
                case DungeonRoomType.HealingSanctuary: return "Healing Room";
                case DungeonRoomType.CursedBargain: return "Cursed Choice";
                case DungeonRoomType.Puzzle: return "Puzzle Room";
                case DungeonRoomType.NarrativeEvent: return "Event Room";
                case DungeonRoomType.Challenge: return "Challenge Room";
                case DungeonRoomType.Secret: return "Secret Room";
                case DungeonRoomType.Miniboss: return "Mini-boss";
                case DungeonRoomType.Boss: return "Boss";
                case DungeonRoomType.Extraction: return "Finish Run";
                default: return type.ToString();
            }
        }

        private void EnsureStyles()
        {
            if (_stylesReady)
            {
                float tooltip = ProfileManager.Current == null ? 1f : ProfileManager.Current.accessibility.tooltipScale;
                float ui = ProfileManager.Current == null ? 1f : ProfileManager.Current.accessibility.uiScale;
                _body.fontSize = Mathf.RoundToInt(13f * ui * tooltip);
                _small.fontSize = Mathf.RoundToInt(11f * ui * tooltip);
                GUI.skin.button = _button;
                return;
            }
            _stylesReady = true;
            _hexTexture = BuildHexTexture(128); _circleTexture = BuildCircleTexture(64);
            float textScale = ProfileManager.Current.accessibility.uiScale;
            _title = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(27 * textScale), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _header = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(15 * textScale), fontStyle = FontStyle.Bold, wordWrap = true };
            float tooltipScale = ProfileManager.Current.accessibility.tooltipScale;
            _body = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(13 * textScale * tooltipScale), wordWrap = true };
            _small = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(11 * textScale * tooltipScale), wordWrap = true };
            _center = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(12 * textScale), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            _button = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(13 * textScale), alignment = TextAnchor.MiddleLeft, wordWrap = true,
                fontStyle = FontStyle.Bold, padding = new RectOffset(16, 16, 9, 9), margin = new RectOffset(3, 3, 3, 3),
                border = new RectOffset(1, 1, 1, 1)
            };
            _button.normal.background = MakeSolidTexture(new Color(0.075f, 0.105f, 0.16f, 0.98f));
            _button.hover.background = MakeSolidTexture(new Color(0.1f, 0.25f, 0.34f, 1f));
            _button.active.background = MakeSolidTexture(new Color(0.06f, 0.48f, 0.58f, 1f));
            _button.normal.textColor = new Color(0.86f, 0.92f, 0.98f);
            _button.hover.textColor = Color.white;
            _button.active.textColor = Color.white;
            _tab = new GUIStyle(_button)
            {
                fontSize = Mathf.RoundToInt(11 * textScale), fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter, wordWrap = true, padding = new RectOffset(8, 8, 7, 7)
            };
            _tab.normal.background = MakeSolidTexture(new Color(0.055f, 0.075f, 0.115f, 1f));
            _tab.hover.background = MakeSolidTexture(new Color(0.1f, 0.28f, 0.38f, 1f));
            _tab.active.background = MakeSolidTexture(new Color(0.08f, 0.55f, 0.67f, 1f));
            _panel = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(12, 12, 12, 12), margin = new RectOffset(4, 4, 4, 4),
                border = new RectOffset(1, 1, 1, 1)
            };
            _panel.normal.background = MakePanelTexture();
            foreach (GUIStyle style in new[] { _title, _header, _body, _small, _center }) style.normal.textColor = new Color(0.9f, 0.94f, 1f);
            _small.normal.textColor = new Color(0.6f, 0.7f, 0.8f);
            GUI.skin.button = _button;
        }

        private static Texture2D BuildHexTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false); texture.hideFlags = HideFlags.HideAndDontSave;
            Vector2 center = Vector2.one * (size - 1) * 0.5f; float radius = size * 0.48f;
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
            {
                bool inside = PointInsideHex(new Vector2(x, y), center, radius); bool inner = PointInsideHex(new Vector2(x, y), center, radius - 4f);
                texture.SetPixel(x, y, inside ? (inner ? Color.white : new Color(0.65f, 0.7f, 0.8f, 1f)) : Color.clear);
            }
            texture.Apply(); return texture;
        }

        private static Texture2D BuildCircleTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false); texture.hideFlags = HideFlags.HideAndDontSave;
            Vector2 center = Vector2.one * (size - 1) * 0.5f; float radius = size * 0.48f;
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++) texture.SetPixel(x, y, Vector2.Distance(new Vector2(x, y), center) <= radius ? Color.white : Color.clear);
            texture.Apply(); return texture;
        }

        private static Texture2D MakePanelTexture()
        {
            return MakeSolidTexture(new Color(0.025f, 0.035f, 0.065f, 0.975f));
        }

        private static Texture2D MakeSolidTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false); texture.hideFlags = HideFlags.HideAndDontSave;
            texture.SetPixel(0, 0, color); texture.Apply(); return texture;
        }
    }
}
