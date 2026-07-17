using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum EquipmentSlot
    {
        Helmet,
        LeftShoulder,
        RightShoulder,
        Chest,
        LeftGlove,
        RightGlove,
        Pants,
        Boots,
        Weapon,
        Offhand
    }

    public enum ItemRarity { Common, Magic, Rare, Unique }
    public enum AffixKind { Legacy, Prefix, Suffix, Corruption }

    public enum UniqueMutation
    {
        None,
        MiniatureSunPrimary,
        VoidMawSecondary,
        DodgeCastsSecondary,
        BloodCasting,
        EchoPrimaryIntoSecondary,
        WildTriggerReservoir,
        ImpossibleConnectors,
        ThirdSpellEcho,
        CorruptedPower,
        AllLightningNoCritical,
        CentralVirtualConnector,
        HealingReservoir,
        OrbitingArsenal,
        ManaWardExchange,
        TriggeredDominion,
        AilmentConvergence,
        TwoHandedCircuit,
        TwinGloveLink
    }

    [Serializable]
    public sealed class PlayerStats
    {
        public float maxHealth = 100f;
        public float maxMana = 100f;
        public float moveSpeed = 7f;
        public float armor;
        public float resistance;
        public float critChance = 0.05f;
        public float critDamage = 1.5f;
        public float spellPower = 1f;
        public float cooldownMultiplier = 1f;
        public float manaCostMultiplier = 1f;
        public float triggerEnergy = 100f;
        public float dodgeCooldown = 1.2f;
        public float healingMultiplier = 1f;
        public float instabilityResistance;
        public float lifeRegen;
        public float manaRegen;
        public float shieldPower;
        public float eliteDamage;
        public float bossDamage;
        public float spellLeech;
        public float goldFind;
        public float rarityFind;
        public float pickupRange;
        public float interactionSpeed;
        public float stunResistance;
        public float knockbackResistance;
        public float lowManaDamage;
        public float highHealthDamage;
        public float shieldedDamage;
        public float afterDodgeDamage;
        public float manaToPower;
        public float healthToPower;
        public float firstCastPower;
        public float cooldownOnKillReduction;

        public PlayerStats Clone() { return (PlayerStats)MemberwiseClone(); }
    }

    public sealed class ItemDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public EquipmentSlot slot;
        public ItemRarity rarity;
        public bool twoHanded;
        public float health;
        public float mana;
        public float movementSpeed;
        public float armor;
        public float resistance;
        public float critChance;
        public float critDamage;
        public float spellPower;
        public float cooldownReduction;
        public float triggerEnergy;
        public UniqueMutation mutation;
        public string setId;
        public string grantedCoreId;
        public string baseFamily;
        public string implicitText;
        public string[] itemTags = new string[0];
        public Color color = Color.white;
    }

    [Serializable]
    public sealed class ItemInstance
    {
        public string instanceId;
        public string definitionId;
        public int itemLevel = 1;
        public int upgradeLevel;
        public bool favorite;
        public bool junk;
        public bool locked;
        public bool corrupted;
        public bool banked;
        public int dataVersion = 11;
        public ItemRarity rarity;
        public int quality;
        public int generationSeed;
        public string corruptionId;
        public string craftedAffixId;
        public List<AffixRoll> affixes = new List<AffixRoll>();

        public ItemInstance(string definition, int level = 1, bool isBanked = false)
        {
            instanceId = Guid.NewGuid().ToString("N");
            definitionId = definition;
            itemLevel = Mathf.Max(1, level);
            banked = isBanked;
            ItemDefinition source = DemoCatalog.GetItem(definition);
            rarity = source == null ? ItemRarity.Common : source.rarity;
        }

        public ItemInstance(ItemSaveData data)
        {
            instanceId = data.instanceId;
            definitionId = data.definitionId;
            itemLevel = data.itemLevel;
            upgradeLevel = data.upgradeLevel;
            favorite = data.favorite;
            junk = data.junk;
            locked = data.locked;
            corrupted = data.corrupted;
            banked = data.banked;
            dataVersion = data.dataVersion <= 0 ? 10 : data.dataVersion;
            ItemDefinition source = DemoCatalog.GetItem(definitionId);
            rarity = data.dataVersion >= 11 && Enum.IsDefined(typeof(ItemRarity), data.rarity)
                ? data.rarity
                : source == null ? ItemRarity.Common : source.rarity;
            quality = data.quality;
            generationSeed = data.generationSeed;
            corruptionId = data.corruptionId;
            craftedAffixId = data.craftedAffixId;
            affixes = data.affixes == null ? new List<AffixRoll>() : data.affixes.Select(a => a.Clone()).ToList();
            V11Itemization.MigrateLegacy(this);
        }

        public ItemDefinition Definition { get { return DemoCatalog.GetItem(definitionId); } }

        public ItemSaveData ToSaveData(bool forceBanked)
        {
            return new ItemSaveData
            {
                instanceId = instanceId,
                definitionId = definitionId,
                itemLevel = itemLevel,
                upgradeLevel = upgradeLevel,
                favorite = favorite,
                junk = junk,
                locked = locked,
                corrupted = corrupted,
                banked = forceBanked || banked,
                dataVersion = 11,
                rarity = rarity,
                quality = quality,
                generationSeed = generationSeed,
                corruptionId = corruptionId,
                craftedAffixId = craftedAffixId,
                affixes = affixes.Select(a => a.Clone()).ToList()
            };
        }

        public string AffixSummary()
        {
            if (affixes == null || affixes.Count == 0) return string.Empty;
            return string.Join(" · ", affixes.Select(a => V11Itemization.FormatAffix(a, false)));
        }

        private static string AffixName(string id)
        {
            switch (id)
            {
                case "life": return "Life";
                case "mana": return "Mana";
                case "armor": return "Armor";
                case "resist": return "Resistance";
                case "power": return "Spell Power";
                case "crit": return "Critical Chance";
                case "speed": return "Movement";
                case "cooldown": return "Cooldown";
                default: return id;
            }
        }

        private static string FormatAffix(AffixRoll affix)
        {
            bool percentage = affix.id == "resist" || affix.id == "power" || affix.id == "crit" || affix.id == "cooldown";
            return "+" + (percentage ? (affix.value * 100f).ToString("0") + "%" : affix.value.ToString("0.0"));
        }

        public string DisplayName { get { return V11Itemization.ItemName(this); } }
        public string AdvancedTooltip { get { return V11Itemization.BuildTooltip(this, true); } }
    }

    public sealed class EquipmentInventory
    {
        // Permanent, banked items that are not currently equipped.
        public readonly List<ItemInstance> backpack = new List<ItemInstance>();
        // Items acquired during the active expedition. These are never legal
        // equipment sources and are lost unless BankAllRunItems is called.
        public readonly List<ItemInstance> runBag = new List<ItemInstance>();
        public readonly Dictionary<EquipmentSlot, ItemInstance> equipped = new Dictionary<EquipmentSlot, ItemInstance>();
        public event Action Changed;
        public bool EquipmentLocked { get { return GameWorld.Instance != null && GameWorld.Instance.RunActive; } }

        public bool ValidateLoadout(out string message)
        {
            ItemInstance weapon;
            ItemInstance offhand;
            if (equipped.TryGetValue(EquipmentSlot.Weapon, out weapon) && weapon != null && weapon.Definition != null && weapon.Definition.twoHanded &&
                equipped.TryGetValue(EquipmentSlot.Offhand, out offhand) && offhand != null)
            {
                message = weapon.DisplayName + " is two-handed and cannot enter an expedition with " + offhand.DisplayName + " equipped.";
                return false;
            }
            if (equipped.Values.Where(item => item != null).GroupBy(item => item.instanceId).Any(group => group.Count() > 1))
            {
                message = "The loadout contains the same item instance in more than one slot. Re-equip the affected items at Home Base.";
                return false;
            }
            message = "Loadout is valid.";
            return true;
        }

        public void LoadFromProfile(bool suppressStartingEquipment)
        {
            backpack.Clear();
            runBag.Clear();
            equipped.Clear();
            foreach (ItemSaveData saved in ProfileManager.Current.armory)
            {
                if (saved == null || DemoCatalog.GetItem(saved.definitionId) == null) continue;
                ItemInstance permanent = new ItemInstance(saved);
                permanent.banked = true;
                backpack.Add(permanent);
            }

            if (!suppressStartingEquipment)
            {
                foreach (SlotItemSave slot in ProfileManager.Current.equippedItems)
                {
                    ItemInstance item = backpack.FirstOrDefault(i => i.instanceId == slot.itemInstanceId);
                    if (item == null || item.Definition == null) continue;
                    backpack.Remove(item);
                    equipped[slot.slot] = item;
                }

                if (equipped.Count == 0)
                {
                    foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
                    {
                        ItemInstance item = backpack.FirstOrDefault(i => i.Definition != null && i.Definition.slot == slot);
                        string ignored;
                        if (item != null) Equip(item, out ignored, false);
                    }
                    ProfileManager.SaveEquipped(equipped);
                }
            }
            Changed?.Invoke();
        }

        public void RestoreRunState(IEnumerable<ItemSaveData> storedItems, IEnumerable<RunEquippedItemSnapshot> wornItems)
        {
            backpack.Clear();
            runBag.Clear();
            equipped.Clear();
            foreach (ItemSaveData saved in ProfileManager.Current.armory)
            {
                if (saved == null || DemoCatalog.GetItem(saved.definitionId) == null) continue;
                ItemInstance permanent = new ItemInstance(saved);
                permanent.banked = true;
                backpack.Add(permanent);
            }
            if (wornItems != null)
            {
                foreach (RunEquippedItemSnapshot saved in wornItems)
                {
                    if (saved == null || saved.item == null || DemoCatalog.GetItem(saved.item.definitionId) == null) continue;
                    ItemInstance banked = backpack.FirstOrDefault(item => item.instanceId == saved.item.instanceId);
                    if (banked != null) backpack.Remove(banked);
                    equipped[saved.slot] = new ItemInstance(saved.item);
                }
            }
            if (storedItems != null)
                foreach (ItemSaveData saved in storedItems)
                    if (saved != null && DemoCatalog.GetItem(saved.definitionId) != null)
                    {
                        ItemInstance unsecured = new ItemInstance(saved);
                        unsecured.banked = false;
                        runBag.Add(unsecured);
                    }
            Changed?.Invoke();
        }

        public ItemInstance Add(string itemDefinitionId, int itemLevel = 1, bool banked = false, bool rollAffixes = true, int generationSeed = 0)
        {
            ItemDefinition definition = DemoCatalog.GetItem(itemDefinitionId);
            if (definition == null) return null;
            ItemInstance item = new ItemInstance(itemDefinitionId, itemLevel, banked);
            if (rollAffixes) V11Itemization.Generate(item, definition, generationSeed);
            if (banked) { backpack.Add(item); Changed?.Invoke(); }
            else runBag.Add(item);
            return item;
        }

        public ItemInstance AddGenerated(ItemSaveData saved, bool banked = false)
        {
            if (saved == null || DemoCatalog.GetItem(saved.definitionId) == null) return null;
            ItemInstance item = new ItemInstance(saved);
            item.instanceId = Guid.NewGuid().ToString("N");
            item.banked = banked;
            if (banked) { backpack.Add(item); Changed?.Invoke(); }
            else runBag.Add(item);
            return item;
        }

        public bool Equip(ItemInstance item, out string message, bool notify = true)
        {
            if (EquipmentLocked)
            {
                message = "Equipment is locked during an expedition. Extract found equipment and change your loadout at Home Base.";
                return false;
            }
            if (item == null || item.Definition == null || !backpack.Contains(item))
            {
                message = "That item is not in the inventory.";
                return false;
            }

            ItemDefinition definition = item.Definition;
            if (definition.slot == EquipmentSlot.Offhand && HasTwoHandedWeapon())
            {
                message = "Unequip the two-handed weapon before using an offhand.";
                return false;
            }

            if (definition.slot == EquipmentSlot.Weapon && definition.twoHanded && equipped.ContainsKey(EquipmentSlot.Offhand))
                UnequipInternal(EquipmentSlot.Offhand);
            if (equipped.ContainsKey(definition.slot)) UnequipInternal(definition.slot);
            backpack.Remove(item);
            equipped[definition.slot] = item;
            message = "Equipped " + item.DisplayName + ".";
            if (notify) Changed?.Invoke();
            return true;
        }

        public bool Unequip(EquipmentSlot slot, out string message)
        {
            if (EquipmentLocked)
            {
                message = "Equipment is locked during an expedition.";
                return false;
            }
            if (!equipped.ContainsKey(slot))
            {
                message = "That slot is already empty.";
                return false;
            }
            string itemName = equipped[slot].DisplayName;
            UnequipInternal(slot);
            message = "Unequipped " + itemName + ".";
            Changed?.Invoke();
            return true;
        }

        public bool Sell(ItemInstance item, out int drachmas, out string message)
        {
            if (EquipmentLocked)
            {
                drachmas = 0;
                message = "Permanent equipment cannot be sold during an expedition. Unsecured equipment may be salvaged instead.";
                return false;
            }
            drachmas = 0;
            if (item == null || item.locked || !backpack.Contains(item))
            {
                message = item != null && item.locked ? "Locked items cannot be sold." : "Item is not available to sell.";
                return false;
            }
            drachmas = 8 + item.itemLevel * 2 + (int)item.rarity * 7 + item.upgradeLevel * 5 + item.quality;
            backpack.Remove(item);
            message = "Sold " + item.DisplayName + " for " + drachmas + " Gold.";
            Changed?.Invoke();
            return true;
        }

        public bool SalvageRunItem(ItemInstance item, RunDirector run, bool confirmProtected, out string message)
        {
            if (run == null || GameWorld.Instance == null || !GameWorld.Instance.RunActive)
            {
                message = "Run salvage is available only during an active expedition.";
                return false;
            }
            if (item == null || !runBag.Contains(item))
            {
                message = "That item is not in the Unsecured Run Bag.";
                return false;
            }
            bool protectedItem = item.favorite || item.locked || item.corrupted || item.rarity == ItemRarity.Rare || item.rarity == ItemRarity.Unique;
            if (protectedItem && !confirmProtected)
            {
                message = "Confirm salvage for this " + item.rarity + (item.corrupted ? " corrupted" : string.Empty) + " item.";
                return false;
            }

            int rarity = Mathf.Clamp((int)item.rarity, 0, 3);
            int gold = 2 + item.itemLevel / 2 + rarity * 3;
            int dust = 1 + rarity + Mathf.Max(0, item.itemLevel / 8);
            int runes = item.rarity == ItemRarity.Rare ? 1 : item.rarity == ItemRarity.Unique ? 2 : 0;
            int cores = item.corrupted ? 1 : 0;
            if (item.rarity == ItemRarity.Magic && item.itemLevel >= 8) runes = 1;
            runBag.Remove(item);
            run.AddDrachmas(gold);
            run.AddForgeMaterials(dust, runes, cores);
            message = "Salvaged " + item.DisplayName + " for " + gold + " Gold and " + dust + " unsecured Forge Dust" +
                (runes > 0 ? ", " + runes + " Binding Rune" + (runes == 1 ? string.Empty : "s") : string.Empty) +
                (cores > 0 ? ", " + cores + " Corruption Core" + (cores == 1 ? string.Empty : "s") : string.Empty) + ".";
            return true;
        }

        public bool DismantlePermanentItem(ItemInstance item, bool confirmProtected, out string message)
        {
            if (EquipmentLocked)
            {
                message = "Permanent Stash management is available only at Home Base.";
                return false;
            }
            if (item == null || !backpack.Contains(item))
            {
                message = "Only an unequipped Permanent Stash item can be dismantled.";
                return false;
            }
            bool protectedItem = item.favorite || item.locked || item.corrupted || item.rarity == ItemRarity.Rare || item.rarity == ItemRarity.Unique;
            if (protectedItem && !confirmProtected)
            {
                message = "Confirm dismantling this protected item.";
                return false;
            }
            int rarity = Mathf.Clamp((int)item.rarity, 0, 3);
            int dust = 1 + rarity + Mathf.Max(0, item.itemLevel / 8);
            int runes = item.rarity == ItemRarity.Rare ? 1 : item.rarity == ItemRarity.Unique ? 2 : 0;
            int cores = item.corrupted ? 1 : 0;
            backpack.Remove(item);
            ProfileManager.Current.armory.RemoveAll(saved => saved != null && saved.instanceId == item.instanceId);
            ProfileManager.SecureForgeMaterials(dust, runes, cores);
            ProfileManager.Save();
            message = "Dismantled " + item.DisplayName + " for " + dust + " Forge Dust" +
                (runes > 0 ? ", " + runes + " Binding Rune" + (runes == 1 ? string.Empty : "s") : string.Empty) +
                (cores > 0 ? ", " + cores + " Corruption Core" + (cores == 1 ? string.Empty : "s") : string.Empty) + ".";
            Changed?.Invoke();
            return true;
        }

        public bool HasMutation(UniqueMutation mutation)
        {
            return equipped.Values.Any(i => i.Definition != null && i.Definition.mutation == mutation);
        }

        public IEnumerable<UniqueMutation> ActiveMutations
        {
            get
            {
                return equipped.Values.Where(item => item != null && item.Definition != null && item.Definition.mutation != UniqueMutation.None)
                    .Select(item => item.Definition.mutation).Distinct();
            }
        }

        public PlayerStats BuildStats(int permanentHealthRank, int permanentManaRank, int permanentPowerRank)
        {
            PlayerStats result = new PlayerStats
            {
                maxHealth = 100f + permanentHealthRank * 8f,
                maxMana = 100f + permanentManaRank * 8f,
                spellPower = 1f + permanentPowerRank * 0.04f
            };

            foreach (ItemInstance item in equipped.Values)
            {
                ItemDefinition definition = item.Definition;
                if (definition == null) continue;
                float levelScale = 1f + (item.itemLevel - 1) * 0.025f + item.upgradeLevel * 0.08f + item.quality * 0.005f;
                result.maxHealth += definition.health * levelScale;
                result.maxMana += definition.mana * levelScale;
                result.moveSpeed += definition.movementSpeed;
                result.armor += definition.armor * levelScale;
                result.resistance += definition.resistance;
                result.critChance += definition.critChance;
                result.critDamage += definition.critDamage;
                result.spellPower += definition.spellPower * levelScale;
                result.cooldownMultiplier *= 1f - Mathf.Clamp(definition.cooldownReduction, 0f, 0.5f);
                result.triggerEnergy += definition.triggerEnergy;
                ApplyAffixes(result, item.affixes, levelScale);
                if (item.corrupted) V11Itemization.ApplyCorruptionToPlayer(result, item.corruptionId);
            }
            Dictionary<string, int> setCounts = equipped.Values.Where(item => item.Definition != null && !string.IsNullOrEmpty(item.Definition.setId))
                .GroupBy(item => item.Definition.setId).ToDictionary(group => group.Key, group => group.Count());
            int warden; setCounts.TryGetValue("warden", out warden);
            if (warden >= 2) { result.armor += 10f; result.resistance += 0.05f; }
            if (warden >= 3) result.maxHealth += 25f;
            int storm; setCounts.TryGetValue("storm", out storm);
            if (storm >= 2) { result.maxMana += 25f; result.spellPower += 0.08f; }
            if (storm >= 3) { result.triggerEnergy += 30f; result.cooldownMultiplier *= 0.94f; }
            int ember; setCounts.TryGetValue("ember", out ember);
            if (ember >= 2) { result.critChance += 0.08f; result.critDamage += 0.2f; }
            if (HasMutation(UniqueMutation.WildTriggerReservoir)) result.triggerEnergy += 80f;
            if (HasMutation(UniqueMutation.ManaWardExchange)) { result.maxHealth -= 20f; result.maxMana += 75f; result.triggerEnergy += 35f; }
            if (HasMutation(UniqueMutation.HealingReservoir)) { result.healingMultiplier += 0.35f; result.spellPower += 0.12f; }
            if (HasMutation(UniqueMutation.TwinGloveLink) && equipped.ContainsKey(EquipmentSlot.LeftGlove) && equipped.ContainsKey(EquipmentSlot.RightGlove))
            { result.spellPower += 0.12f; result.cooldownMultiplier *= 0.9f; }
            if (HasMutation(UniqueMutation.AllLightningNoCritical)) result.critChance = 0f;
            return result;
        }

        public string SetBonusSummary()
        {
            List<string> active = new List<string>();
            foreach (IGrouping<string, ItemInstance> group in equipped.Values.Where(item => item.Definition != null && !string.IsNullOrEmpty(item.Definition.setId)).GroupBy(item => item.Definition.setId))
            {
                int count = group.Count();
                if (group.Key == "warden") active.Add("Warden " + count + "/3" + (count >= 2 ? " · Armor bonus active" : string.Empty));
                else if (group.Key == "storm") active.Add("Storm " + count + "/3" + (count >= 2 ? " · Mana bonus active" : string.Empty));
                else if (group.Key == "ember") active.Add("Ember " + count + "/2" + (count >= 2 ? " · Critical bonus active" : string.Empty));
            }
            return active.Count == 0 ? "No equipment set bonuses" : string.Join("\n", active);
        }

        public void ApplyFoundationMutation(CompiledSpell spell, SpellSlot slot)
        {
            if (slot == SpellSlot.Slot1 && HasMutation(UniqueMutation.MiniatureSunPrimary))
            {
                spell.displayName = "Dying Sun";
                spell.element = SpellElement.Fire;
                spell.primaryColor = new Color(1f, 0.23f, 0.03f);
                spell.accentColor = new Color(1f, 0.9f, 0.25f);
                spell.damage *= 1.65f;
                spell.speed *= 0.34f;
                spell.lifetime *= 1.45f;
                spell.size *= 2.2f;
                spell.explosionRadius = Mathf.Max(4.5f, spell.explosionRadius * 2f);
                spell.manaCost *= 1.45f;
                spell.cooldown *= 1.2f;
                spell.isMiniatureSun = true;
                spell.pullsEnemies = true;
            }

            if (slot == SpellSlot.Slot2 && HasMutation(UniqueMutation.VoidMawSecondary))
            {
                spell.coreId = "void_maw";
                spell.displayName = "Maw Between Worlds";
                spell.delivery = SpellDelivery.Projectile;
                spell.element = SpellElement.Void;
                spell.primaryColor = new Color(0.5f, 0.08f, 0.9f);
                spell.accentColor = new Color(1f, 0.3f, 0.95f);
                spell.damage *= 1.25f;
                spell.speed = 5f;
                spell.lifetime = 3.5f;
                spell.size = 1.4f;
                spell.explosionRadius = 2.2f;
                spell.manaCost *= 1.35f;
                spell.cooldown *= 1.15f;
                spell.isVoidMaw = true;
                spell.pullsEnemies = true;
            }

            if (slot == SpellSlot.Slot1 && HasMutation(UniqueMutation.EchoPrimaryIntoSecondary))
                spell.triggers.Add(NewEquipmentTrigger("echoing_saint", TriggerMoment.OnCast, SpellSlot.Slot2, 42f, 0.4f));
            if (slot == SpellSlot.Slot3 && HasMutation(UniqueMutation.ThirdSpellEcho))
                spell.triggers.Add(NewEquipmentTrigger("third_spell_echo", TriggerMoment.OnHit, SpellSlot.Slot1, 36f, 0.45f));
        }

        public void ApplyGlobalSpellStats(CompiledSpell spell)
        {
            if (HasMutation(UniqueMutation.CorruptedPower)) spell.instability += 15f;
            if (HasMutation(UniqueMutation.ImpossibleConnectors))
            {
                spell.instability *= 0.55f;
                spell.executionLayers.Add("Unique: Impossible Angles · 45% less Spell Overload");
            }
            V11Itemization.ApplySpellAffixes(spell, equipped.Values);
            if (HasMutation(UniqueMutation.AllLightningNoCritical))
            {
                spell.element = SpellElement.Lightning;
                spell.primaryColor = new Color(0.35f, 0.65f, 1f);
                spell.damage *= 1.28f;
                spell.executionLayers.Add("Unique: Storm Without Chance · all damage becomes Lightning");
            }
            if (HasMutation(UniqueMutation.OrbitingArsenal) && spell.delivery == SpellDelivery.Projectile)
            { spell.orbitCaster = true; spell.lifetime = Mathf.Max(4f, spell.lifetime); spell.executionLayers.Add("Unique: Orbiting Arsenal"); }
            if (HasMutation(UniqueMutation.TriggeredDominion)) spell.executionLayers.Add("Unique: Triggered Dominion · manual ×0.82 / triggered ×1.30");
            if (HasMutation(UniqueMutation.AilmentConvergence)) spell.spreadsStatus = true;
            if (HasMutation(UniqueMutation.TwoHandedCircuit) && spell.triggers.Count > 0)
            { spell.cooldown *= 0.88f; spell.instability += 8f; }
        }

        public void BankAllRunItems()
        {
            ItemInstance[] all = runBag.Where(item => item != null).ToArray();
            foreach (ItemInstance item in all)
            {
                item.banked = true;
                if (ProfileManager.Current.armory.All(i => i.instanceId != item.instanceId))
                    ProfileManager.Current.armory.Add(item.ToSaveData(true));
            }
            runBag.Clear();
        }

        public void SaveSanctuaryEquipment(bool persist = true)
        {
            foreach (ItemInstance item in backpack.Concat(equipped.Values))
            {
                if (!item.banked) continue;
                ItemSaveData saved = ProfileManager.Current.armory.FirstOrDefault(i => i.instanceId == item.instanceId);
                if (saved == null) ProfileManager.Current.armory.Add(item.ToSaveData(true));
                else
                {
                    int index = ProfileManager.Current.armory.IndexOf(saved);
                    ProfileManager.Current.armory[index] = item.ToSaveData(true);
                }
            }
            ProfileManager.SaveEquipped(equipped, persist);
        }

        private bool HasTwoHandedWeapon()
        {
            ItemInstance weapon;
            return equipped.TryGetValue(EquipmentSlot.Weapon, out weapon) && weapon.Definition != null && weapon.Definition.twoHanded;
        }

        private void UnequipInternal(EquipmentSlot slot)
        {
            ItemInstance old = equipped[slot];
            equipped.Remove(slot);
            backpack.Add(old);
        }

        private static void ApplyAffixes(PlayerStats stats, IEnumerable<AffixRoll> affixes, float scale)
        {
            if (affixes == null) return;
            foreach (AffixRoll affix in affixes)
            {
                if (V11Itemization.ApplyPlayerAffix(stats, affix, scale)) continue;
                switch (affix.id)
                {
                    case "life": stats.maxHealth += affix.value * scale; break;
                    case "mana": stats.maxMana += affix.value * scale; break;
                    case "armor": stats.armor += affix.value * scale; break;
                    case "resist": stats.resistance += affix.value; break;
                    case "power": stats.spellPower += affix.value; break;
                    case "crit": stats.critChance += affix.value; break;
                    case "speed": stats.moveSpeed += affix.value; break;
                    case "cooldown": stats.cooldownMultiplier *= 1f - affix.value; break;
                }
            }
        }

        private static TriggerSpec NewEquipmentTrigger(string id, TriggerMoment moment, SpellSlot slot, float energy, float power)
        {
            return new TriggerSpec
            {
                sourceId = id,
                moment = moment,
                linkedSlot = slot,
                targetContext = TargetContext.Impact,
                energyCost = energy,
                inheritedPower = power,
                maxActivationsPerEvent = 1
            };
        }
    }
}
