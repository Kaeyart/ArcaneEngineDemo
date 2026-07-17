using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum LootKind { Modifier, Item, Essence, Drachmas, SpellCore }

    public sealed class LootPickup : WorldRoomInteractable
    {
        private static readonly List<LootPickup> ActiveLoot = new List<LootPickup>();
        public static int ActiveCount { get { return ActiveLoot.Count(value => value != null); } }
        public LootKind kind;
        public string contentId;
        public int amount;
        public int itemLevel = 1;
        public int itemSeed;
        public ItemRarity DisplayedRarity { get { return _previewItem == null ? ItemRarity.Common : _previewItem.rarity; } }
        public ItemInstance PreviewItem { get { return _previewItem; } }
        public ItemDefinition ItemDefinition { get { return kind == LootKind.Item ? DemoCatalog.GetItem(contentId) : null; } }
        public SpellModifierDefinition ModifierDefinition { get { return kind == LootKind.Modifier ? DemoCatalog.GetModifier(contentId) : null; } }
        public SpellCoreDefinition CoreDefinition { get { return kind == LootKind.SpellCore ? DemoCatalog.GetCore(contentId) : null; } }
        private ItemInstance _previewItem;
        private Vector3 _basePosition;
        private float _hoverPhase;
        private LineRenderer _lootBeam;
        private bool _hiddenByFilter;
        private float _nextStackRefresh;
        public bool HiddenByFilter { get { return _hiddenByFilter; } }
        public override bool SupportsHoldInteraction { get { return kind == LootKind.Item || kind == LootKind.Modifier; } }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!ActiveLoot.Contains(this)) ActiveLoot.Add(this);
        }

        protected override void OnDisable()
        {
            ActiveLoot.Remove(this);
            base.OnDisable();
        }

        public static LootPickup CreateModifier(Vector3 position, string modifierId)
        {
            SpellModifierDefinition definition = DemoCatalog.GetModifier(modifierId);
            return Create(position, LootKind.Modifier, modifierId, 1, definition == null ? Color.white : definition.uiColor, 1);
        }

        public static LootPickup CreateItem(Vector3 position, string itemId, int level = 1)
        {
            ItemDefinition definition = DemoCatalog.GetItem(itemId);
            int seed = V1Determinism.Combine(Mathf.RoundToInt(position.x * 100f), Mathf.RoundToInt(position.z * 100f), itemId,
                UnityEngine.Random.Range(1, int.MaxValue));
            return Create(position, LootKind.Item, itemId, 1, definition == null ? Color.white : definition.color, level, seed);
        }

        public static LootPickup CreateEssence(Vector3 position, int essence)
        {
            return Create(position, LootKind.Essence, "essence", essence, new Color(0.3f, 1f, 0.85f), 1);
        }

        public static LootPickup CreateDrachmas(Vector3 position, int amount)
        {
            return Create(position, LootKind.Drachmas, "drachmas", amount, new Color(1f, 0.78f, 0.18f), 1);
        }

        public static LootPickup CreateCore(Vector3 position, string coreId)
        {
            SpellCoreDefinition definition = DemoCatalog.GetCore(coreId);
            return Create(position, LootKind.SpellCore, coreId, 1, definition == null ? Color.white : definition.color, 1);
        }

        private static LootPickup Create(Vector3 position, LootKind lootKind, string id, int count, Color color, int level, int seed = 0)
        {
            PrimitiveType type = lootKind == LootKind.Modifier ? PrimitiveType.Cube : lootKind == LootKind.Item ? PrimitiveType.Capsule :
                lootKind == LootKind.SpellCore ? PrimitiveType.Cylinder : PrimitiveType.Sphere;
            GameObject go = RuntimeVisuals.Primitive(lootKind + " Pickup", type, position + Vector3.up * 0.65f,
                lootKind == LootKind.Item ? new Vector3(0.45f, 0.65f, 0.45f) : Vector3.one * 0.48f, color);
            RuntimeVisuals.RemoveCollider(go);
            LootPickup pickup = go.AddComponent<LootPickup>();
            pickup.kind = lootKind; pickup.contentId = id; pickup.amount = count; pickup.itemLevel = level; pickup.itemSeed = seed;
            if (lootKind == LootKind.Item)
            {
                ItemDefinition itemDefinition = DemoCatalog.GetItem(id);
                pickup._previewItem = new ItemInstance(id, level);
                V11Itemization.Generate(pickup._previewItem, itemDefinition, seed);
            }
            pickup._basePosition = go.transform.position; pickup._hoverPhase = Random.Range(0f, Mathf.PI * 2f);
            pickup.PromptTitle = pickup.DisplayName();
            pickup.PromptDescription = pickup.Description();
            pickup.PromptAction = lootKind == LootKind.Item ? "TAKE · HOLD INTERACT TO SALVAGE" : lootKind == LootKind.Modifier ? "TAKE · HOLD INTERACT TO QUICK-INSTALL WHEN SAFE" : "COLLECT";
            RuntimeVisuals.Ring("Pickup Ring", go.transform.position, color, 0.55f, 0.05f, go.transform).transform.localPosition = Vector3.zero;
            ItemRarity displayedRarity = pickup._previewItem == null ? ItemRarity.Common : pickup._previewItem.rarity;
            float beamHeight = lootKind == LootKind.Item ? 1.8f + (int)displayedRarity * 0.7f : 1.55f;
            pickup._lootBeam = CreateLootBeam(go.transform, color, beamHeight,
                lootKind == LootKind.Item ? 0.06f + (int)displayedRarity * 0.02f : 0.055f);
            pickup.CreateLabel(Color.Lerp(Color.white, color, 0.25f));
            RewardVisualSystem.AttachPickup(pickup, color);
            return pickup;
        }

        private void Update()
        {
            RefreshFilterVisibility();
            if (_hiddenByFilter) return;
            transform.Rotate(0f, 75f * Time.deltaTime, 0f, Space.World);
            transform.position = _basePosition + Vector3.up * Mathf.Sin(Time.time * 3f + _hoverPhase) * 0.12f;
            if (Time.unscaledTime >= _nextStackRefresh) { _nextStackRefresh = Time.unscaledTime + 0.18f; RefreshLabelStack(); }
            if (GameWorld.Instance == null || GameWorld.Instance.Player == null || !GameWorld.Instance.RunActive) return;
            bool auto = kind == LootKind.Drachmas ? ProfileManager.Current.autoCollectGold : kind == LootKind.Essence && ProfileManager.Current.autoCollectEssence;
            if (auto && (GameWorld.Instance.Player.transform.position - transform.position).sqrMagnitude < 2.2f) Collect(false);
        }

        private void RefreshFilterVisibility()
        {
            bool reveal = ArcaneInput.GetKey(KeyCode.LeftAlt) || ArcaneInput.GetKey(KeyCode.RightAlt);
            bool hidden = false;
            if (kind == LootKind.Item && !reveal && ProfileManager.Current != null)
            {
                ItemDefinition item = DemoCatalog.GetItem(contentId);
                ItemRarity rarity = _previewItem == null || item == null ? ItemRarity.Common : _previewItem.rarity;
                bool protectedDrop = rarity == ItemRarity.Unique || (_previewItem != null && _previewItem.corrupted);
                bool wrongSlot = item != null && ProfileManager.Current.lootSlotFilter >= 0 && (int)item.slot != ProfileManager.Current.lootSlotFilter;
                bool wrongTag = item != null && !string.IsNullOrEmpty(ProfileManager.Current.lootTagFilter) &&
                    (item.itemTags == null || !item.itemTags.Contains(ProfileManager.Current.lootTagFilter));
                hidden = !protectedDrop && ((int)rarity < ProfileManager.Current.minimumLootRarity || wrongSlot || wrongTag);
            }
            if (_hiddenByFilter == hidden) return;
            _hiddenByFilter = hidden;
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true)) renderer.enabled = !hidden;
        }

        private void RefreshLabelStack()
        {
            int stack = ActiveLoot.Where(value => value != null && !value._hiddenByFilter && value != this &&
                (value._basePosition - _basePosition).sqrMagnitude < 1.45f).Count(value => value.GetEntityId().GetHashCode() < GetEntityId().GetHashCode());
            SetLabelVerticalOffset(2.1f + Mathf.Min(4, stack) * 0.48f);
        }

        public override void Interact() { Collect(false); }
        public override void HoldInteract() { Collect(true); }

        private void Collect(bool quickUse)
        {
            if (kind == LootKind.Modifier)
            {
                GameWorld.Instance.AddModifier(contentId, amount);
                if (quickUse) { string message; GameWorld.Instance.TryQuickInstallModifier(contentId, out message); GameWorld.Instance.Log(message); }
            }
            else if (kind == LootKind.Item)
            {
                ItemInstance instance = GameWorld.Instance.Equipment.Add(contentId, itemLevel, false, true, itemSeed);
                if (quickUse && instance != null)
                {
                    RunDirector run = GameWorld.Instance.GetComponent<RunDirector>();
                    string salvaged;
                    GameWorld.Instance.Equipment.SalvageRunItem(instance, run, true, out salvaged);
                    GameWorld.Instance.Log(salvaged);
                }
                if (!quickUse && instance != null) GameWorld.Instance.Log("Added unsecured " + instance.rarity + " item to the Run Bag: " + instance.DisplayName + ".");
                ProfileManager.Discover("item:" + contentId);
                RunDirector checkpoint = GameWorld.Instance.GetComponent<RunDirector>();
                if (checkpoint != null) checkpoint.SaveRunCheckpoint();
            }
            else if (kind == LootKind.SpellCore) GameWorld.Instance.AddCoreCopy(contentId);
            else if (kind == LootKind.Drachmas)
            {
                GameWorld.Instance.GetComponent<RunDirector>().AddDrachmas(amount);
                GameWorld.Instance.Log("Collected " + amount + " Gold.");
            }
            else
            {
                ProfileManager.Current.essence += amount;
                ProfileManager.Save();
                GameWorld.Instance.Log("Recovered " + amount + " death-safe Essence.");
            }
            Destroy(gameObject);
        }

        private string DisplayName()
        {
            if (kind == LootKind.Modifier) { SpellModifierDefinition value = DemoCatalog.GetModifier(contentId); return value == null ? "SPELL MODIFIER" : value.displayName.ToUpperInvariant(); }
            if (kind == LootKind.Item) return _previewItem == null ? "EQUIPMENT" : _previewItem.DisplayName.ToUpperInvariant();
            if (kind == LootKind.SpellCore) { SpellCoreDefinition value = DemoCatalog.GetCore(contentId); return value == null ? "BASE SPELL" : value.displayName.ToUpperInvariant() + " · SPELL COPY"; }
            return kind == LootKind.Drachmas ? amount + " GOLD" : amount + " ESSENCE";
        }

        private string Description()
        {
            if (kind == LootKind.Modifier) { SpellModifierDefinition value = DemoCatalog.GetModifier(contentId); return value == null ? string.Empty : value.shortDescription; }
            if (kind == LootKind.Item)
            {
                ItemDefinition value = DemoCatalog.GetItem(contentId); if (value == null) return string.Empty;
                ItemRarity rarity = _previewItem == null ? value.rarity : _previewItem.rarity;
                string filter = rarity != ItemRarity.Unique && (int)rarity < ProfileManager.Current.minimumLootRarity ? " · HOLD ALT TO REVEAL FILTERED LOOT" : string.Empty;
                string details = _previewItem == null ? rarity + " " + value.slot + " · " + StatLine(value) : V11Itemization.BuildTooltip(_previewItem, false);
                if (_previewItem != null && GameWorld.Instance != null && GameWorld.Instance.Equipment != null)
                {
                    string comparison = V11Itemization.BuildComparison(GameWorld.Instance.Equipment, _previewItem, ProfileManager.Current);
                    details += "\n\nCOMPARED WITH LOCKED LOADOUT\n" + string.Join("\n", comparison.Split('\n').Take(5));
                }
                return details + "\nUNSECURED · Loadout locked until extraction" + filter;
            }
            if (kind == LootKind.SpellCore) { SpellCoreDefinition value = DemoCatalog.GetCore(contentId); return value == null ? string.Empty : value.description; }
            return kind == LootKind.Drachmas ? "Run currency used at physical merchants." : "Permanent currency secured immediately.";
        }

        private static string StatLine(ItemDefinition item)
        {
            string line = string.Empty;
            if (item.health != 0f) line += "+" + item.health.ToString("0") + " Health  ";
            if (item.mana != 0f) line += "+" + item.mana.ToString("0") + " Mana  ";
            if (item.armor != 0f) line += "+" + item.armor.ToString("0") + " Armor  ";
            if (item.movementSpeed != 0f) line += "+" + item.movementSpeed.ToString("0.0") + " Move  ";
            if (item.spellPower != 0f) line += "+" + (item.spellPower * 100f).ToString("0") + "% Power  ";
            if (item.critChance != 0f) line += "+" + (item.critChance * 100f).ToString("0") + "% Crit  ";
            return string.IsNullOrEmpty(line) ? item.description : line.TrimEnd();
        }

        private static LineRenderer CreateLootBeam(Transform parent, Color color, float height, float width)
        {
            GameObject beamObject = new GameObject("Loot Rarity Beam");
            beamObject.transform.SetParent(parent, false);
            LineRenderer line = beamObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.sharedMaterial = RuntimeVisuals.Material(color, 1.25f);
            line.positionCount = 2;
            line.SetPosition(0, Vector3.down * 0.55f);
            line.SetPosition(1, Vector3.up * height);
            line.startWidth = width;
            line.endWidth = width * 0.2f;
            line.startColor = color;
            line.endColor = new Color(color.r, color.g, color.b, 0f);
            return line;
        }
    }
}
