using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ArpgPanel31
    {
        None,
        Character,
        MapDevice,
        Constellations,
        Inventory,
        Stash,
        Crafting,
        SpellForge,
        Training
    }

    public sealed class ArpgInterface30 : MonoBehaviour
    {
        public static ArpgInterface30 Instance { get; private set; }
        public bool IsOpen { get { return _panel != ArpgPanel31.None; } }
        public string Message { get { return _message; } }

        private ArpgPanel31 _panel = ArpgPanel31.None;
        private Vector2 _leftScroll;
        private Vector2 _rightScroll;
        private string _selectedConstellationId;
        private string _selectedItemId;
        private string _selectedMapId;
        private string _message = string.Empty;
        private float _messageUntil;
        private GUIStyle _title;
        private GUIStyle _heading;
        private GUIStyle _small;
        private GUIStyle _wrap;
        private GUIStyle _good;
        private GUIStyle _warning;
        private Texture2D _backdrop;
        private Texture2D _panelTexture;
        private Texture2D _accent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (_backdrop != null) Destroy(_backdrop);
            if (_panelTexture != null) Destroy(_panelTexture);
            if (_accent != null) Destroy(_accent);
        }

        private void Update()
        {
            if (ArpgFrontend31.Instance == null || !ArpgFrontend31.Instance.IsGameplay) return;
            if (ArpgFoundation30.Profile == null) return;

            if (ArpgInput31.Pressed(ArpgAction31.Inventory)) TogglePanel(ArpgPanel31.Inventory);
            if (ArpgInput31.Pressed(ArpgAction31.MapDevice)) TogglePanel(ArpgPanel31.MapDevice);
            if (ArpgInput31.Pressed(ArpgAction31.SpellForge)) TogglePanel(ArpgPanel31.SpellForge);
            if (ArpgInput31.Pressed(ArpgAction31.Constellations)) TogglePanel(ArpgPanel31.Constellations);
            if (ArpgInput31.CancelPressed() && _panel != ArpgPanel31.None) ClosePanel();
        }

        public void OpenPanel(ArpgPanel31 panel)
        {
            _panel = panel;
            _leftScroll = Vector2.zero;
            _rightScroll = Vector2.zero;
            SetCursorMode(true);
        }

        public void ClosePanel()
        {
            _panel = ArpgPanel31.None;
            SetCursorMode(false);
        }

        public void TogglePanel(ArpgPanel31 panel)
        {
            if (_panel == panel) ClosePanel();
            else OpenPanel(panel);
        }

        public void SetMessage(string message)
        {
            _message = message ?? string.Empty;
            _messageUntil = Time.unscaledTime + 5.5f;
        }

        private void SetCursorMode(bool menu)
        {
            Cursor.visible = menu;
            Cursor.lockState = menu ? CursorLockMode.None : CursorLockMode.Locked;
        }

        private void OnGUI()
        {
            if (ArpgFoundation30.Instance == null || ArpgFoundation30.Profile == null) return;
            if (ArpgFrontend31.Instance == null || ArpgFrontend31.Instance.State != ArpgFrontendState31.Gameplay) return;
            EnsureStyles();
            DrawHud();
            if (_panel != ArpgPanel31.None) DrawPanel();
            if (!string.IsNullOrEmpty(_message) && Time.unscaledTime <= _messageUntil)
                GUI.Label(new Rect(Screen.width * 0.5f - 360f, Screen.height - 112f, 720f, 44f), _message, GUI.skin.box);
        }

        private void DrawHud()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            Rect top = new Rect(18f, 14f, Mathf.Min(820f, Screen.width - 36f), 82f);
            GUI.Box(top, GUIContent.none);
            GUI.Label(new Rect(top.x + 16f, top.y + 10f, 320f, 25f), profile.characterName + " · " + profile.characterClass + " · Level " + profile.level, _heading);
            GUI.Label(new Rect(top.x + 16f, top.y + 38f, 360f, 20f), "XP " + profile.experience + " / " + profile.ExperienceToNextLevel + " · Inventory " + profile.InventoryCount + "/" + profile.inventoryCapacity, _small);
            GUI.Label(new Rect(top.x + 390f, top.y + 12f, 400f, 45f), ArpgFoundation30.Instance.LastMessage, _small);

            if (ArpgFoundation30.Instance.MapActive)
            {
                ArpgMapItem30 map = ArpgFoundation30.Instance.ActiveMap;
                GUI.Box(new Rect(Screen.width - 354f, 14f, 336f, 82f), GUIContent.none);
                GUI.Label(new Rect(Screen.width - 336f, 24f, 300f, 24f), "ACTIVE MAP · T" + map.tier + " " + map.rarity, _heading);
                string guardian = ArpgFoundation30.Instance.GuardianDefeated ? "Guardian defeated — secure the cache" : "Defeat the map guardian";
                GUI.Label(new Rect(Screen.width - 336f, 52f, 300f, 20f), guardian, _small);
                if (ArpgFoundation30.Instance.TimedMapRemaining > 0f)
                    GUI.Label(new Rect(Screen.width - 130f, 52f, 90f, 20f), Mathf.CeilToInt(ArpgFoundation30.Instance.TimedMapRemaining) + "s", _warning);
            }
            else
            {
                string objective = ArpgFoundation30.Instance.CurrentObjective;
                if (!string.IsNullOrEmpty(objective))
                {
                    GUI.Box(new Rect(Screen.width - 454f, 14f, 436f, 82f), GUIContent.none);
                    GUI.Label(new Rect(Screen.width - 434f, 24f, 390f, 24f), "CURRENT OBJECTIVE", _heading);
                    GUI.Label(new Rect(Screen.width - 434f, 52f, 390f, 20f), objective, _small);
                }
            }
        }

        private void DrawPanel()
        {
            Rect rect = new Rect(Mathf.Max(24f, Screen.width * 0.035f), 112f, Mathf.Max(860f, Screen.width * 0.93f), Mathf.Max(560f, Screen.height - 142f));
            GUI.DrawTexture(rect, _backdrop, ScaleMode.StretchToFill);
            GUILayout.BeginArea(new Rect(rect.x + 18f, rect.y + 14f, rect.width - 36f, rect.height - 28f));
            GUILayout.BeginHorizontal();
            GUILayout.Label(PanelTitle(_panel), _title, GUILayout.ExpandWidth(true));
            if (ArpgFoundation30.Instance.MapActive && GUILayout.Button("Abandon Map", GUILayout.Width(130f), GUILayout.Height(32f))) ArpgFoundation30.Instance.AbandonMap();
            if (GUILayout.Button("Close", GUILayout.Width(90f), GUILayout.Height(32f))) ClosePanel();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            DrawNav(ArpgPanel31.Character, "Character");
            DrawNav(ArpgPanel31.MapDevice, "Map Device");
            DrawNav(ArpgPanel31.Inventory, "Inventory");
            DrawNav(ArpgPanel31.Stash, "Stash");
            DrawNav(ArpgPanel31.Crafting, "Crafting");
            DrawNav(ArpgPanel31.SpellForge, "SpellForge");
            DrawNav(ArpgPanel31.Constellations, "Constellations");
            DrawNav(ArpgPanel31.Training, "Training");
            GUILayout.EndHorizontal();
            GUILayout.Space(8f);

            switch (_panel)
            {
                case ArpgPanel31.Character: DrawCharacter(); break;
                case ArpgPanel31.MapDevice: DrawMaps(); break;
                case ArpgPanel31.Inventory: DrawInventory(false, false); break;
                case ArpgPanel31.Stash: DrawInventory(true, false); break;
                case ArpgPanel31.Crafting: DrawInventory(false, true); break;
                case ArpgPanel31.SpellForge: DrawSpellForge(); break;
                case ArpgPanel31.Constellations: DrawConstellations(); break;
                case ArpgPanel31.Training: DrawTraining(); break;
            }
            GUILayout.EndArea();
        }

        private void DrawNav(ArpgPanel31 panel, string label)
        {
            if (GUILayout.Toggle(_panel == panel, label, GUI.skin.button, GUILayout.Height(28f))) _panel = panel;
        }

        private void DrawCharacter()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            ArpgClassDefinition30 classDefinition = ArpgContent30.Class(profile.characterClass);
            ArpgStatAccumulator30 stats = ArpgStatHooks30.Build(profile);
            _rightScroll = GUILayout.BeginScrollView(_rightScroll);
            GUILayout.Label(profile.characterName + " · " + (classDefinition == null ? profile.characterClass.ToString() : classDefinition.displayName), _heading);
            if (classDefinition != null) GUILayout.Label(classDefinition.description, _wrap);
            GUILayout.Label("Level " + profile.level + " · Experience " + profile.experience + "/" + profile.ExperienceToNextLevel);
            GUILayout.Label("Highest Tier " + Mathf.Max(0, profile.highestCompletedTier) + " · Maps " + profile.totalMapsCompleted + " · Deaths " + profile.totalDeaths);
            GUILayout.Label("Constellation Points " + profile.constellationPoints + " · Atlas Points " + profile.atlasPoints + " · Attunement " + ArpgStatHooks30.AttunementUsed(profile) + "/" + ArpgStatHooks30.AttunementMaximum(profile));
            GUILayout.Space(8f);
            GUILayout.Label("Combat Statistics", _heading);
            DrawStat("Spell Power", stats.Get(ArpgStat30.SpellPower), true);
            DrawStat("Maximum Health", stats.Get(ArpgStat30.MaximumHealth), false);
            DrawStat("Maximum Mana", stats.Get(ArpgStat30.MaximumMana), false);
            DrawStat("Armour", stats.Get(ArpgStat30.Armour), false);
            DrawStat("Evasion", stats.Get(ArpgStat30.Evasion), false);
            DrawStat("Arcane Ward", stats.Get(ArpgStat30.ArcaneWard), false);
            DrawStat("Critical Chance", stats.Get(ArpgStat30.CriticalChance), true);
            DrawStat("Move Speed", stats.Get(ArpgStat30.MoveSpeed), true);
            DrawStat("Ailment Buildup", stats.Get(ArpgStat30.AilmentBuildup), true);
            DrawStat("Reaction Power", stats.Get(ArpgStat30.ReactionPower), true);
            DrawStat("Rune Capacity", stats.Get(ArpgStat30.RuneCapacity), false);
            GUILayout.Space(8f);
            GUILayout.Label("Elemental Power", _heading);
            DrawStat("Fire", stats.Get(ArpgStat30.FirePower), true);
            DrawStat("Cold", stats.Get(ArpgStat30.ColdPower), true);
            DrawStat("Lightning", stats.Get(ArpgStat30.LightningPower), true);
            DrawStat("Physical", stats.Get(ArpgStat30.PhysicalPower), true);
            DrawStat("Blood", stats.Get(ArpgStat30.BloodPower), true);
            DrawStat("Toxic", stats.Get(ArpgStat30.ToxicPower), true);
            DrawStat("Void", stats.Get(ArpgStat30.VoidPower), true);
            GUILayout.Space(8f);
            GUILayout.Label("Equipped", _heading);
            foreach (ArpgItemSlot30 slot in Enum.GetValues(typeof(ArpgItemSlot30)).Cast<ArpgItemSlot30>())
            {
                ArpgItem30 item = profile.Equipped(slot);
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label(slot + ": " + (item == null ? "Empty" : item.displayName), GUILayout.ExpandWidth(true));
                GUI.enabled = item != null;
                if (GUILayout.Button("Unequip", GUILayout.Width(88f)) && profile.Unequip(slot)) SaveAndRecalculate("Unequipped " + item.displayName + ".");
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        private void DrawMaps()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(390f));
            GUILayout.Label("White Map Network · Tier 0–5", _heading);
            GUILayout.Label("Basic Completion unlocks connected maps. Mastery awards permanent Atlas progression.", _wrap);
            _leftScroll = GUILayout.BeginScrollView(_leftScroll, GUI.skin.box);
            foreach (ArpgMapDefinition30 map in ArpgContent30.Maps.Where(value => value.playableIn31 && value.tier <= 5).OrderBy(value => value.tier).ThenBy(value => value.displayName))
            {
                bool complete = profile.completedMapIds.Contains(map.id);
                bool mastered = profile.masteredMapIds.Contains(map.id);
                bool available = map.tier == 0 || map.tier <= profile.highestCompletedTier + 1 || profile.maps.Any(value => value != null && value.mapId == map.id);
                string label = (available ? string.Empty : "LOCKED · ") + "T" + map.tier + " " + map.displayName + "\n" + (complete ? "Complete" : "Incomplete") + (mastered ? " · Mastered" : string.Empty);
                GUI.enabled = available;
                if (GUILayout.Toggle(_selectedMapId == map.id, label, GUI.skin.button)) _selectedMapId = map.id;
                GUI.enabled = true;
            }
            GUILayout.EndScrollView();
            GUI.enabled = !ArpgFoundation30.Instance.MapActive;
            if (GUILayout.Button("Generate Free Tier 0 Map", GUILayout.Height(38f)))
            {
                string result;
                ArpgFoundation30.Instance.LaunchFreeTierZero(out result);
                SetMessage(result);
                if (ArpgFoundation30.Instance.MapActive) ClosePanel();
            }
            GUI.enabled = true;
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            ArpgMapDefinition30 selectedDefinition = ArpgContent30.Map(_selectedMapId);
            ArpgMapItem30 selectedItem = profile.maps.FirstOrDefault(value => value != null && value.mapId == _selectedMapId);
            if (selectedDefinition == null)
            {
                GUILayout.Label("Select a map node.", _heading);
            }
            else
            {
                GUILayout.Label(selectedDefinition.displayName, _title);
                GUILayout.Label("Tier " + selectedDefinition.tier + " · Guardian: " + selectedDefinition.bossName);
                GUILayout.Label("Region: " + selectedDefinition.region + " · " + selectedDefinition.environmentHint + " · Reward focus: " + selectedDefinition.rewardFocus, _wrap);
                GUILayout.Space(6f);
                GUILayout.Label("Mastery", _heading);
                GUILayout.Label(selectedDefinition.masteryDescription, _wrap);
                GUILayout.Space(6f);
                GUILayout.Label("Owned copies", _heading);
                List<ArpgMapItem30> copies = profile.maps.Where(value => value != null && value.mapId == selectedDefinition.id).OrderBy(value => value.rarity).ToList();
                if (copies.Count == 0) GUILayout.Label(selectedDefinition.tier == 0 ? "Tier 0 can be regenerated freely." : "No owned map item.");
                foreach (ArpgMapItem30 copy in copies)
                {
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.Label(copy.rarity + (copy.affixIds.Count > 0 ? " · " + copy.affixIds.Count + " modifier(s)" : string.Empty));
                    foreach (string id in copy.affixIds)
                    {
                        ArpgMapAffixDefinition30 affix = ArpgContent30.MapAffix(id);
                        if (affix != null) GUILayout.Label("• " + affix.displayName + " — " + affix.description, _small);
                    }
                    GUI.enabled = !ArpgFoundation30.Instance.MapActive;
                    if (GUILayout.Button("Open this map", GUILayout.Height(34f)))
                    {
                        string result;
                        ArpgFoundation30.Instance.LaunchMap(copy, out result);
                        SetMessage(result);
                        if (ArpgFoundation30.Instance.MapActive) ClosePanel();
                    }
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawInventory(bool stash, bool crafting)
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(410f));
            GUILayout.Label(stash ? "Persistent Stash" : crafting ? "Crafting Inventory" : "Inventory", _heading);
            IEnumerable<ArpgItem30> source = stash ? profile.StashItems() : profile.InventoryItems();
            List<ArpgItem30> items = source.OrderByDescending(value => value.rarity).ThenByDescending(value => value.itemLevel).ThenBy(value => value.displayName).ToList();
            GUILayout.Label(items.Count + (stash ? " stored items" : " / " + profile.inventoryCapacity + " carried items"), _small);
            _leftScroll = GUILayout.BeginScrollView(_leftScroll, GUI.skin.box);
            foreach (ArpgItem30 item in items)
            {
                string equipped = profile.IsEquipped(item.instanceId) ? " · EQUIPPED" : string.Empty;
                string locked = item.locked ? " · LOCKED" : string.Empty;
                string label = item.rarity + " · iLvl " + item.itemLevel + " · " + item.slot + equipped + locked + "\n" + item.displayName;
                if (GUILayout.Toggle(_selectedItemId == item.instanceId, label, GUI.skin.button)) _selectedItemId = item.instanceId;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            ArpgItem30 selected = profile.GetItem(_selectedItemId);
            if (selected == null || (stash != profile.IsInStash(selected.instanceId) && !crafting))
            {
                GUILayout.Label("Select an item.", _heading);
            }
            else
            {
                GUILayout.Label(selected.displayName, _title);
                GUILayout.TextArea(ArpgItems30.Describe(selected), GUILayout.MinHeight(180f));
                ArpgItem30 equipped = profile.Equipped(selected.slot);
                if (equipped != null && equipped.instanceId != selected.instanceId)
                {
                    GUILayout.Label("Compared with " + equipped.displayName, _heading);
                    GUILayout.TextArea(ArpgItems30.Compare(selected, equipped), GUILayout.MinHeight(80f));
                }
                GUILayout.BeginHorizontal();
                if (!stash && !crafting && GUILayout.Button("Equip", GUILayout.Height(34f)))
                {
                    string result;
                    if (profile.Equip(selected, out result)) SaveAndRecalculate(result); else SetMessage(result);
                }
                if (!stash && !crafting && GUILayout.Button("Move to Stash", GUILayout.Height(34f)))
                {
                    if (profile.MoveToStash(selected)) SaveAndRecalculate("Moved " + selected.displayName + " to the Stash.");
                }
                if (stash && GUILayout.Button("Move to Inventory", GUILayout.Height(34f)))
                {
                    if (profile.MoveToInventory(selected)) SaveAndRecalculate("Moved " + selected.displayName + " to the inventory.");
                    else SetMessage("Inventory is full.");
                }
                if (GUILayout.Button(selected.locked ? "Unlock" : "Lock", GUILayout.Height(34f)))
                {
                    selected.locked = !selected.locked;
                    SaveAndRecalculate((selected.locked ? "Locked " : "Unlocked ") + selected.displayName + ".");
                }
                GUILayout.EndHorizontal();
                if (crafting) DrawCraftingActions(selected);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawCraftingActions(ArpgItem30 item)
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            GUILayout.Space(8f);
            GUILayout.Label("Five Core Crafting Actions", _heading);
            ArpgCurrency30[] core =
            {
                ArpgCurrency30.SparkOfAlteration,
                ArpgCurrency30.RuneOfAugmentation,
                ArpgCurrency30.SigilOfElevation,
                ArpgCurrency30.ArcaneExalt,
                ArpgCurrency30.ReformationOrb
            };
            foreach (ArpgCurrency30 currency in core)
            {
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label(ArpgItems30.CurrencyName(currency) + " · " + profile.Currency(currency), GUILayout.ExpandWidth(true));
                GUI.enabled = profile.Currency(currency) > 0 && !item.locked;
                if (GUILayout.Button("Apply", GUILayout.Width(90f)))
                {
                    string result;
                    ArpgFoundation30.Instance.CraftItem(item, currency, out result);
                    SetMessage(result);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            if (item.locked) GUILayout.Label("Unlock this item before crafting.", _warning);
        }

        private void DrawSpellForge()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(430f));
            GUILayout.Label("Discovered Spell Cores", _heading);
            _leftScroll = GUILayout.BeginScrollView(_leftScroll, GUI.skin.box);
            foreach (string coreId in profile.ownedCoreIds)
            {
                bool active = coreId == profile.activeCoreId;
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label((active ? "ACTIVE · " : string.Empty) + ArpgLegacyBridge30.CoreDisplayName(coreId), _heading);
                GUI.enabled = !active;
                if (GUILayout.Button(active ? "Equipped" : "Equip Core"))
                {
                    string result;
                    if (ArpgLegacyBridge30.EquipCore(GameWorld.Instance, profile, coreId, out result)) SaveAndRecalculate(result); else SetMessage(result);
                }
                GUI.enabled = true;
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Support Runes", _heading);
            GUILayout.Label("Owned Support Runes are restored into the existing SpellForge board. Use the existing board controls to place them. Only edge-connected Runes are active.", _wrap);
            _rightScroll = GUILayout.BeginScrollView(_rightScroll);
            foreach (string runeId in profile.ownedRuneIds)
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(runeId);
                GUILayout.Label("• " + (definition == null ? runeId : definition.displayName));
            }
            if (profile.ownedRuneIds.Count == 0) GUILayout.Label("No Support Runes discovered yet.");
            GUILayout.Space(10f);
            GUILayout.Label("Spell Links", _heading);
            foreach (int value in profile.ownedLinkConditionIds)
                GUILayout.Label("• " + (Enum.IsDefined(typeof(SpellLinkCondition), value) ? ((SpellLinkCondition)value).ToString() : value.ToString()));
            if (profile.ownedLinkConditionIds.Count == 0) GUILayout.Label("No Spell Links discovered yet.");
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawConstellations()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(420f));
            GUILayout.Label("Starter Constellations", _heading);
            GUILayout.Label("Points " + profile.constellationPoints + " · Attunement " + ArpgStatHooks30.AttunementUsed(profile) + "/" + ArpgStatHooks30.AttunementMaximum(profile));
            _leftScroll = GUILayout.BeginScrollView(_leftScroll, GUI.skin.box);
            foreach (ArpgConstellationDefinition30 constellation in ArpgContent30.Constellations.Where(value => profile.discoveredConstellations.Contains(value.id)))
            {
                int allocated = constellation.nodes.Count(value => profile.allocatedConstellationNodes.Contains(value.id));
                if (GUILayout.Toggle(_selectedConstellationId == constellation.id, constellation.displayName + "\n" + allocated + "/" + constellation.nodes.Count + " Stars · " + constellation.attunementCost + " Attunement", GUI.skin.button))
                    _selectedConstellationId = constellation.id;
            }
            GUILayout.EndScrollView();
            int resetCost = Mathf.Max(1, profile.allocatedConstellationNodes.Count / 10);
            GUI.enabled = profile.allocatedConstellationNodes.Count > 0;
            if (GUILayout.Button("Reset All · " + resetCost + " Null Orb(s)"))
            {
                string result;
                ArpgFoundation30.Instance.ResetConstellations(out result);
                SetMessage(result);
            }
            GUI.enabled = true;
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            ArpgConstellationDefinition30 selected = ArpgContent30.Constellation(_selectedConstellationId);
            if (selected == null)
            {
                GUILayout.Label("Select a Constellation.", _heading);
            }
            else
            {
                GUILayout.Label(selected.displayName, _title);
                GUILayout.Label(selected.description, _wrap);
                _rightScroll = GUILayout.BeginScrollView(_rightScroll);
                foreach (ArpgConstellationNodeDefinition30 node in selected.nodes)
                {
                    bool allocated = profile.allocatedConstellationNodes.Contains(node.id);
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.Label((allocated ? "✓ " : "○ ") + node.size + " · " + node.displayName + " · " + node.pointCost + " pt", _heading);
                    GUILayout.Label(node.description, _wrap);
                    GUI.enabled = !allocated;
                    if (GUILayout.Button(allocated ? "Allocated" : "Allocate"))
                    {
                        string result;
                        ArpgFoundation30.Instance.AllocateConstellationNode(node.id, out result);
                        SetMessage(result);
                    }
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawTraining()
        {
            GUILayout.Label("Refuge Training Target", _title);
            GUILayout.Label("Attack the target in the Refuge to inspect direct damage, elemental composition, ailments, reactions, critical strikes, projectiles, and Chain behavior through the existing combat diagnostics.", _wrap);
            GUILayout.Space(10f);
            GUILayout.Label("Live diagnostic summary", _heading);
            string report = ArpgRefuge31.Instance == null ? "Training target is unavailable." : ArpgRefuge31.Instance.TrainingReport;
            GUILayout.TextArea(report, GUILayout.MinHeight(220f));
            GUILayout.Space(10f);
            if (GUILayout.Button("Reset Training Metrics", GUILayout.Width(220f), GUILayout.Height(34f)) && ArpgRefuge31.Instance != null)
            {
                ArpgRefuge31.Instance.ResetTrainingMetrics();
                SetMessage("Training metrics reset.");
            }
        }

        private void SaveAndRecalculate(string message)
        {
            ArpgProfileStore30.Save(ArpgFoundation30.Profile);
            if (GameWorld.Instance != null) GameWorld.Instance.RecalculateStats(true);
            SetMessage(message);
        }

        private void EnsureStyles()
        {
            if (_title != null) return;
            _backdrop = SolidTexture(new Color(0.025f, 0.035f, 0.065f, 0.97f));
            _panelTexture = SolidTexture(new Color(0.06f, 0.08f, 0.14f, 0.96f));
            _accent = SolidTexture(new Color(0.24f, 0.58f, 1f, 1f));
            _title = new GUIStyle(GUI.skin.label) { fontSize = 25, fontStyle = FontStyle.Bold, normal = { textColor = new Color(0.86f, 0.92f, 1f) } };
            _heading = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, normal = { textColor = new Color(0.68f, 0.82f, 1f) } };
            _small = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true, normal = { textColor = new Color(0.76f, 0.8f, 0.9f) } };
            _wrap = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
            _good = new GUIStyle(_small) { normal = { textColor = new Color(0.42f, 0.95f, 0.64f) } };
            _warning = new GUIStyle(_small) { normal = { textColor = new Color(1f, 0.56f, 0.25f) } };
        }

        private static Texture2D SolidTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.name = "ArcaneEngine31_UI";
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private static string PanelTitle(ArpgPanel31 panel)
        {
            switch (panel)
            {
                case ArpgPanel31.MapDevice: return "Astral Map Device";
                case ArpgPanel31.Constellations: return "Constellation Archive";
                case ArpgPanel31.Inventory: return "Inventory & Equipment";
                case ArpgPanel31.Stash: return "Persistent Stash";
                case ArpgPanel31.Crafting: return "Arcane Crafting Station";
                case ArpgPanel31.SpellForge: return "SpellForge Altar";
                case ArpgPanel31.Training: return "Combat Laboratory";
                default: return "Character Archive";
            }
        }

        private static void DrawStat(string label, float value, bool percentage)
        {
            GUILayout.Label(label + ": " + (percentage ? Mathf.RoundToInt(value * 100f) + "%" : value.ToString("0.##")));
        }
    }
}
