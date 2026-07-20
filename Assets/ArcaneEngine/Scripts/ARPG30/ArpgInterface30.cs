using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ArcaneEngine
{
    public sealed class ArpgInterface30 : MonoBehaviour
    {
        private bool _open;
        private int _tab;
        private Vector2 _leftScroll;
        private Vector2 _rightScroll;
        private string _selectedConstellationId;
        private string _selectedItemId;
        private string _selectedMapInstanceId;
        private string _uiMessage = "";
        private GUIStyle _title;
        private GUIStyle _section;
        private GUIStyle _small;
        private GUIStyle _wrap;

        private static readonly string[] Tabs = { "Character", "Atlas", "Constellations", "Items", "Discoveries" };

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.f7Key.wasPressedThisFrame) _open = !_open;
#else
            if (Input.GetKeyDown(KeyCode.F7)) _open = !_open;
#endif
        }

        private void OnGUI()
        {
            if (ArpgFoundation30.Instance == null || ArpgFoundation30.Profile == null) return;
            EnsureStyles();
            DrawStatusStrip();
            if (ArpgFoundation30.Profile.characterClass == ArpgClass30.Unchosen)
            {
                DrawClassSelection();
                return;
            }
            if (!_open) return;
            DrawMainPanel();
        }

        private void EnsureStyles()
        {
            if (_title != null) return;
            _title = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
            _section = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
            _small = new GUIStyle(GUI.skin.label) { fontSize = 11 };
            _wrap = new GUIStyle(GUI.skin.label) { wordWrap = true };
        }

        private void DrawStatusStrip()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            GUILayout.BeginArea(new Rect(12f, 10f, Mathf.Min(760f, Screen.width - 24f), 76f), GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUILayout.Label("ARCANE ENGINE 3.0", _section, GUILayout.Width(210f));
            GUILayout.Label(profile.characterClass == ArpgClass30.Unchosen ? "No ARPG character" : profile.characterClass + " · Level " + profile.level + " · Highest Tier " + Mathf.Max(0, profile.highestCompletedTier), GUILayout.ExpandWidth(true));
            if (profile.characterClass != ArpgClass30.Unchosen && GUILayout.Button(_open ? "Close [F7]" : "Atlas [F7]", GUILayout.Width(110f))) _open = !_open;
            GUILayout.EndHorizontal();
            if (ArpgFoundation30.Instance.MapActive)
            {
                ArpgMapItem30 map = ArpgFoundation30.Instance.ActiveMap;
                GUILayout.Label("ACTIVE MAP · Tier " + map.tier + " · " + map.rarity + (map.corrupted ? " · Corrupted" : string.Empty) + (ArpgFoundation30.Instance.TimedMapRemaining > 0f ? " · " + Mathf.CeilToInt(ArpgFoundation30.Instance.TimedMapRemaining) + "s" : string.Empty));
            }
            else GUILayout.Label(ArpgFoundation30.Instance.LastMessage, _small);
            GUILayout.EndArea();
        }

        private void DrawClassSelection()
        {
            Rect rect = new Rect(Screen.width * 0.5f - 360f, Screen.height * 0.5f - 245f, 720f, 490f);
            GUILayout.BeginArea(rect, GUI.skin.window);
            GUILayout.Label("Awaken in the Astral Refuge", _title);
            GUILayout.Label("Choose one permanent base class. You begin at Level 0 with one bare-bones Spell Core, no Support Runes, no Spell Links, no equipment, and no currency. Tier 0 maps are always available.", _wrap);
            GUILayout.Space(12f);
            foreach (ArpgClassDefinition30 definition in ArpgContent30.Classes)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label(definition.displayName, _section);
                GUILayout.Label(definition.description, _wrap);
                GUILayout.Label("Ascendancies: " + string.Join(" · ", definition.ascendancies.Select(value => ArpgContent30.Ascendancy(value).displayName).ToArray()), _small);
                if (GUILayout.Button("Begin as " + definition.displayName, GUILayout.Height(34f)))
                {
                    string message;
                    ArpgFoundation30.Instance.ChooseClass(definition.id, out message);
                    _uiMessage = message;
                    _open = true;
                }
                GUILayout.EndVertical();
            }
            if (!string.IsNullOrEmpty(_uiMessage)) GUILayout.Label(_uiMessage, _wrap);
            GUILayout.EndArea();
        }

        private void DrawMainPanel()
        {
            Rect rect = new Rect(Mathf.Max(20f, Screen.width * 0.04f), 94f, Mathf.Max(760f, Screen.width * 0.92f), Mathf.Max(540f, Screen.height - 118f));
            GUILayout.BeginArea(rect, GUI.skin.window);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Astral Refuge · Endgame-First ARPG", _title, GUILayout.ExpandWidth(true));
            if (ArpgFoundation30.Instance.MapActive && GUILayout.Button("Abandon Active Map", GUILayout.Width(150f))) ArpgFoundation30.Instance.AbandonMap();
            if (GUILayout.Button("X", GUILayout.Width(34f))) _open = false;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            for (int index = 0; index < Tabs.Length; index++)
            {
                if (GUILayout.Toggle(_tab == index, Tabs[index], GUI.skin.button)) _tab = index;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            if (_tab == 0) DrawCharacter();
            else if (_tab == 1) DrawAtlas();
            else if (_tab == 2) DrawConstellations();
            else if (_tab == 3) DrawItems();
            else DrawDiscoveries();
            GUILayout.Space(4f);
            if (!string.IsNullOrEmpty(_uiMessage)) GUILayout.Label(_uiMessage, GUI.skin.box);
            GUILayout.EndArea();
        }

        private void DrawCharacter()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            ArpgClassDefinition30 classDefinition = ArpgContent30.Class(profile.characterClass);
            ArpgStatAccumulator30 stats = ArpgStatHooks30.Build(profile);
            _rightScroll = GUILayout.BeginScrollView(_rightScroll);
            GUILayout.Label(classDefinition.displayName + " · Level " + profile.level, _section);
            GUILayout.Label(classDefinition.description, _wrap);
            GUILayout.Label("Experience: " + profile.experience + " / " + profile.ExperienceToNextLevel);
            GUILayout.Label("Constellation Points: " + profile.constellationPoints + " · Atlas Points: " + profile.atlasPoints + " · Ascendancy Points: " + profile.ascendancyPoints);
            GUILayout.Label("Attunement: " + ArpgStatHooks30.AttunementUsed(profile) + " / " + ArpgStatHooks30.AttunementMaximum(profile));
            GUILayout.Space(8f);
            GUILayout.Label("Persistent Statistics", _section);
            DrawStat("Spell Power", stats.Get(ArpgStat30.SpellPower), true);
            DrawStat("Maximum Health", stats.Get(ArpgStat30.MaximumHealth), false);
            DrawStat("Maximum Mana", stats.Get(ArpgStat30.MaximumMana), false);
            DrawStat("Critical Chance", stats.Get(ArpgStat30.CriticalChance), true);
            DrawStat("Cooldown Recovery", stats.Get(ArpgStat30.CooldownRecovery), true);
            DrawStat("Mana Efficiency", stats.Get(ArpgStat30.ManaEfficiency), true);
            DrawStat("Map Sustain", stats.Get(ArpgStat30.MapSustain), true);
            DrawStat("Item Rarity", stats.Get(ArpgStat30.ItemRarity), true);
            GUILayout.Space(8f);
            GUILayout.Label("Ascendancy", _section);
            if (profile.ascendancy == ArpgAscendancy30.None)
            {
                GUILayout.Label("Complete Tier 9 to earn the first two Ascendancy Points, then choose one permanent specialization.", _wrap);
                foreach (ArpgAscendancy30 id in classDefinition.ascendancies)
                {
                    ArpgAscendancyDefinition30 definition = ArpgContent30.Ascendancy(id);
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.Label(definition.displayName, _section);
                    GUILayout.Label(definition.description, _wrap);
                    GUI.enabled = profile.highestCompletedTier >= 9 && profile.ascendancyPoints >= 2;
                    if (GUILayout.Button("Choose " + definition.displayName))
                    {
                        string message;
                        ArpgFoundation30.Instance.ChooseAscendancy(id, out message);
                        _uiMessage = message;
                    }
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                }
            }
            else
            {
                ArpgAscendancyDefinition30 definition = ArpgContent30.Ascendancy(profile.ascendancy);
                GUILayout.Label(definition.displayName + " · " + definition.description, _wrap);
                foreach (ArpgAscendancyNodeDefinition30 node in definition.nodes)
                {
                    bool allocated = profile.allocatedAscendancyNodes.Contains(node.id);
                    GUILayout.BeginHorizontal(GUI.skin.box);
                    GUILayout.Label((allocated ? "✓ " : "○ ") + node.displayName + " — " + node.description, _wrap, GUILayout.ExpandWidth(true));
                    GUI.enabled = !allocated && profile.ascendancyPoints > 0;
                    if (GUILayout.Button(allocated ? "Allocated" : "Allocate", GUILayout.Width(90f)))
                    {
                        string message;
                        ArpgFoundation30.Instance.AllocateAscendancyNode(node.id, out message);
                        _uiMessage = message;
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }

        private void DrawAtlas()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(330f));
            GUILayout.Label("Atlas Progress", _section);
            foreach (ArpgMapBand30 band in new[] { ArpgMapBand30.White, ArpgMapBand30.Blue, ArpgMapBand30.Yellow, ArpgMapBand30.Red })
            {
                List<ArpgMapDefinition30> maps = ArpgContent30.Maps.Where(value => value.Band == band).ToList();
                int completed = maps.Count(value => profile.completedMapIds.Contains(value.id));
                int mastered = maps.Count(value => profile.masteredMapIds.Contains(value.id));
                GUILayout.Label(band + ": " + completed + "/" + maps.Count + " complete · " + mastered + " mastered");
            }
            GUILayout.Label("Highest completed tier: " + Mathf.Max(0, profile.highestCompletedTier));
            GUILayout.Label("Atlas Points: " + profile.atlasPoints);
            GUILayout.Space(8f);
            GUI.enabled = !ArpgFoundation30.Instance.MapActive;
            if (GUILayout.Button("Open Free Tier 0 Map", GUILayout.Height(38f)))
            {
                string message;
                ArpgFoundation30.Instance.LaunchFreeTierZero(out message);
                _uiMessage = message;
                if (ArpgFoundation30.Instance.MapActive) _open = false;
            }
            GUI.enabled = true;
            GUILayout.Space(8f);
            GUILayout.Label("Map Inventory", _section);
            _leftScroll = GUILayout.BeginScrollView(_leftScroll, GUI.skin.box);
            foreach (ArpgMapItem30 map in profile.maps.OrderBy(value => value.tier).ThenBy(value => value.rarity).ToList())
            {
                ArpgMapDefinition30 definition = ArpgContent30.Map(map.mapId);
                string label = "T" + map.tier + " " + map.Band + " · " + map.rarity + (map.corrupted ? " · CORRUPTED" : string.Empty) + "\n" + (definition == null ? map.mapId : definition.displayName);
                if (GUILayout.Toggle(_selectedMapInstanceId == map.instanceId, label, GUI.skin.button)) _selectedMapInstanceId = map.instanceId;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            ArpgMapItem30 selected = profile.maps.FirstOrDefault(value => value.instanceId == _selectedMapInstanceId);
            if (selected == null)
            {
                GUILayout.Label("Select a map item.", _section);
                GUILayout.Label("White tiers 0–9 form the foundation. Blue 10–19 forms the build. Yellow 20–29 specializes it. Red 30–39 tests and optimizes it.", _wrap);
            }
            else
            {
                ArpgMapDefinition30 definition = ArpgContent30.Map(selected.mapId);
                GUILayout.Label(definition == null ? selected.mapId : definition.displayName, _title);
                GUILayout.Label("Tier " + selected.tier + " · " + selected.Band + " band · " + selected.rarity + (selected.corrupted ? " · CORRUPTED" : string.Empty));
                GUILayout.Label("Boss: " + (definition == null ? "Unknown" : definition.bossName));
                GUILayout.Label("Quality: " + selected.quality + "%");
                GUILayout.Label("Mastery requirement: " + MasteryText(selected.Band), _wrap);
                GUILayout.Space(6f);
                GUILayout.Label("Map Modifiers", _section);
                if (selected.affixIds.Count == 0) GUILayout.Label("No modifiers.");
                foreach (string id in selected.affixIds)
                {
                    ArpgMapAffixDefinition30 affix = ArpgContent30.MapAffix(id);
                    if (affix != null) GUILayout.Label("• " + affix.displayName + " — " + affix.description, _wrap);
                }
                GUI.enabled = !ArpgFoundation30.Instance.MapActive;
                if (GUILayout.Button("Open Map", GUILayout.Height(38f)))
                {
                    string message;
                    ArpgFoundation30.Instance.LaunchMap(selected, out message);
                    _uiMessage = message;
                    if (ArpgFoundation30.Instance.MapActive) _open = false;
                }
                GUI.enabled = true;
                GUILayout.Space(8f);
                DrawMapCraftButtons(selected);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawConstellations()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(330f));
            GUILayout.Label("Constellations", _section);
            GUILayout.Label("Points: " + profile.constellationPoints + " · Attunement: " + ArpgStatHooks30.AttunementUsed(profile) + "/" + ArpgStatHooks30.AttunementMaximum(profile));
            _leftScroll = GUILayout.BeginScrollView(_leftScroll, GUI.skin.box);
            foreach (ArpgConstellationDefinition30 constellation in ArpgContent30.Constellations.Where(value => profile.discoveredConstellations.Contains(value.id)).OrderBy(value => value.category).ThenBy(value => value.displayName))
            {
                int allocated = constellation.nodes.Count(value => profile.allocatedConstellationNodes.Contains(value.id));
                string label = constellation.category + " · " + constellation.displayName + "\n" + allocated + "/" + constellation.nodes.Count + " Stars · " + constellation.attunementCost + " Attunement";
                if (GUILayout.Toggle(_selectedConstellationId == constellation.id, label, GUI.skin.button)) _selectedConstellationId = constellation.id;
            }
            GUILayout.EndScrollView();
            int resetCost = Mathf.Max(1, profile.allocatedConstellationNodes.Count / 10);
            if (GUILayout.Button("Reset All · " + resetCost + " Null Orb(s)"))
            {
                string message;
                ArpgFoundation30.Instance.ResetConstellations(out message);
                _uiMessage = message;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            ArpgConstellationDefinition30 selected = ArpgContent30.Constellation(_selectedConstellationId);
            if (selected == null)
            {
                GUILayout.Label("Select a discovered constellation.", _section);
                GUILayout.Label("Constellations are independent clusters. Invest through Small, Medium, and Large Stars to reach a Completion Boon. Completed constellations consume Attunement.", _wrap);
            }
            else
            {
                GUILayout.Label(selected.displayName, _title);
                GUILayout.Label(selected.category + " · " + selected.attunementCost + " Completion Attunement", _small);
                GUILayout.Label(selected.description, _wrap);
                _rightScroll = GUILayout.BeginScrollView(_rightScroll);
                foreach (ArpgConstellationNodeDefinition30 node in selected.nodes)
                {
                    bool allocated = profile.allocatedConstellationNodes.Contains(node.id);
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label((allocated ? "✓ " : "○ ") + node.size + " · " + node.displayName, _section, GUILayout.ExpandWidth(true));
                    GUILayout.Label(node.pointCost + " pt", GUILayout.Width(45f));
                    GUILayout.EndHorizontal();
                    GUILayout.Label(node.description, _wrap);
                    GUI.enabled = !allocated;
                    if (GUILayout.Button(allocated ? "Allocated" : "Allocate"))
                    {
                        string message;
                        ArpgFoundation30.Instance.AllocateConstellationNode(node.id, out message);
                        _uiMessage = message;
                    }
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawItems()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(350f));
            GUILayout.Label("Equipped", _section);
            foreach (ArpgItemSlot30 slot in Enum.GetValues(typeof(ArpgItemSlot30)).Cast<ArpgItemSlot30>())
            {
                ArpgItem30 item = profile.Equipped(slot);
                GUILayout.Label(slot + ": " + (item == null ? "Empty" : item.displayName), _small);
            }
            GUILayout.Space(6f);
            GUILayout.Label("Inventory · " + profile.items.Count + " items", _section);
            _leftScroll = GUILayout.BeginScrollView(_leftScroll, GUI.skin.box);
            foreach (ArpgItem30 item in profile.items.OrderByDescending(value => value.itemLevel).ThenByDescending(value => value.rarity))
            {
                string label = item.slot + " · " + item.rarity + " · iLvl " + item.itemLevel + "\n" + item.displayName;
                if (GUILayout.Toggle(_selectedItemId == item.instanceId, label, GUI.skin.button)) _selectedItemId = item.instanceId;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            ArpgItem30 selected = profile.GetItem(_selectedItemId);
            if (selected == null) GUILayout.Label("Select an item.", _section);
            else
            {
                GUILayout.Label(selected.displayName, _title);
                GUILayout.TextArea(ArpgItems30.Describe(selected), GUILayout.MinHeight(160f));
                if (GUILayout.Button("Equip in " + selected.slot, GUILayout.Height(34f))) ArpgFoundation30.Instance.EquipItem(selected);
                GUILayout.Space(8f);
                GUILayout.Label("Currency Crafting", _section);
                _rightScroll = GUILayout.BeginScrollView(_rightScroll);
                foreach (ArpgCurrency30 currency in Enum.GetValues(typeof(ArpgCurrency30)).Cast<ArpgCurrency30>())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(ArpgItems30.CurrencyName(currency) + " · " + profile.Currency(currency), GUILayout.ExpandWidth(true));
                    GUI.enabled = profile.Currency(currency) > 0;
                    if (GUILayout.Button("Use", GUILayout.Width(70f)))
                    {
                        string message;
                        ArpgFoundation30.Instance.CraftItem(selected, currency, out message);
                        _uiMessage = message;
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawDiscoveries()
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            _rightScroll = GUILayout.BeginScrollView(_rightScroll);
            GUILayout.Label("Persistent Discoveries", _title);
            GUILayout.Label("Spell Cores · " + profile.ownedCoreIds.Count, _section);
            foreach (string id in profile.ownedCoreIds)
            {
                SpellCoreDefinition definition = DemoCatalog.GetCore(id);
                GUILayout.Label("• " + (definition == null ? id : definition.displayName));
            }
            GUILayout.Space(6f);
            GUILayout.Label("Support Runes · " + profile.ownedRuneIds.Count, _section);
            foreach (string id in profile.ownedRuneIds)
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(id);
                GUILayout.Label("• " + (definition == null ? id : definition.displayName));
            }
            GUILayout.Space(6f);
            GUILayout.Label("Spell Link Conditions · " + profile.ownedLinkConditionIds.Count, _section);
            foreach (int value in profile.ownedLinkConditionIds) GUILayout.Label("• " + (Enum.IsDefined(typeof(SpellLinkCondition), value) ? ((SpellLinkCondition)value).ToString() : value.ToString()));
            GUILayout.Space(6f);
            GUILayout.Label("Currency", _section);
            foreach (ArpgCurrency30 currency in Enum.GetValues(typeof(ArpgCurrency30)).Cast<ArpgCurrency30>()) GUILayout.Label("• " + ArpgItems30.CurrencyName(currency) + ": " + profile.Currency(currency));
            GUILayout.Space(8f);
            GUILayout.Label("Roguelite Mode", _section);
            GUILayout.Label("The existing run modes remain installed and available through their original Home Base interface. They are optional Fracture-style content and do not replace persistent Atlas progression.", _wrap);
            GUILayout.EndScrollView();
        }

        private void DrawMapCraftButtons(ArpgMapItem30 map)
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            ArpgCurrency30[] currencies = { ArpgCurrency30.RefinementShard, ArpgCurrency30.SparkOfAlteration, ArpgCurrency30.SigilOfElevation, ArpgCurrency30.ChaosFragment, ArpgCurrency30.CorruptionCatalyst, ArpgCurrency30.DivineMeasure };
            GUILayout.Label("Map Crafting", _section);
            foreach (ArpgCurrency30 currency in currencies)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(ArpgItems30.CurrencyName(currency) + " · " + profile.Currency(currency), GUILayout.ExpandWidth(true));
                GUI.enabled = profile.Currency(currency) > 0;
                if (GUILayout.Button("Use", GUILayout.Width(70f)))
                {
                    string message;
                    ArpgFoundation30.Instance.CraftMap(map, currency, out message);
                    _uiMessage = message;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
        }

        private static string MasteryText(ArpgMapBand30 band)
        {
            if (band == ArpgMapBand30.White) return "Defeat the map guardian.";
            if (band == ArpgMapBand30.Blue) return "Complete the map while it is Magic or Rare.";
            if (band == ArpgMapBand30.Yellow) return "Complete the map while it is Rare.";
            return "Complete the map while it is Rare and Corrupted.";
        }

        private static void DrawStat(string label, float value, bool percentage)
        {
            GUILayout.Label(label + ": " + (percentage ? Mathf.RoundToInt(value * 100f) + "%" : value.ToString("0.##")));
        }
    }
}
