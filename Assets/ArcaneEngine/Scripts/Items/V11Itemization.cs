using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ForgeAction { UpgradeRarity, AddAffix, RerollAffix, ImproveRoll, RemoveAffix, LockAffix, AddQuality, Corrupt }

    public sealed class V11AffixDefinition
    {
        public string id;
        public string displayName;
        public string stat;
        public AffixKind kind;
        public string group;
        public string[] tags;
        public EquipmentSlot[] slots;
        public float baseMinimum;
        public float baseMaximum;
        public bool percentage;
        public bool local;
        public int weight;

        public bool Allows(ItemDefinition item)
        {
            if (item == null) return false;
            if (slots != null && slots.Length > 0 && !slots.Contains(item.slot)) return false;
            if (tags == null || tags.Length == 0 || tags.Contains("any")) return true;
            string[] itemTags = item.itemTags ?? new string[0];
            return tags.Any(tag => itemTags.Contains(tag));
        }

        public void RangeForTier(int tier, out float minimum, out float maximum)
        {
            float multiplier = tier == 1 ? 1.8f : tier == 2 ? 1.42f : tier == 3 ? 1.13f : tier == 4 ? 0.88f : 0.68f;
            minimum = baseMinimum * multiplier;
            maximum = baseMaximum * multiplier;
        }
    }

    /// <summary>
    /// Deterministic 1.1 item generator. Definitions, weighting, validation,
    /// migration, tooltip formatting and spell integration share one catalog.
    /// </summary>
    public static class V11Itemization
    {
        public const int DataVersion = 11;
        private static readonly Dictionary<string, V11AffixDefinition> Affixes = new Dictionary<string, V11AffixDefinition>();
        private static readonly string[] Corruptions =
        {
            "unstable_might|Unstable Might|spell_power|0.12|Power rises, but Spell Overload rises by 12.",
            "blood_price|Blood Price|life|-22|Lose maximum Health and gain stronger spells.",
            "glass_mind|Glass Mind|mana|38|Gain maximum Mana and lose Armor.",
            "storm_scar|Storm Scar|lightning_damage|0.18|Lightning spells gain damage.",
            "ember_scar|Ember Scar|fire_damage|0.18|Fire spells gain damage.",
            "winter_scar|Winter Scar|cold_damage|0.18|Cold spells gain damage.",
            "venom_scar|Venom Scar|toxic_damage|0.18|Poison spells gain damage.",
            "void_scar|Void Scar|void_damage|0.18|Void spells gain damage.",
            "swift_ruin|Swift Ruin|move_speed|0.55|Move faster but recover Dodge more slowly.",
            "trigger_brand|Trigger Brand|trigger_energy|24|Gain Trigger Energy with added instability.",
            "perfected_implicit|Perfected Implicit|quality|8|The base item gains additional quality.",
            "fractured_echo|Fractured Echo|cooldown|0.08|Recover spells faster but pay more Mana."
        };
        private static bool _initialized;

        public static IEnumerable<V11AffixDefinition> AllAffixes { get { Ensure(); return Affixes.Values; } }
        public static int CorruptionCount { get { return Corruptions.Length; } }

        public static void Ensure()
        {
            if (_initialized) return;
            _initialized = true;
            BuildAffixes(AffixKind.Prefix, PrefixSpecs);
            BuildAffixes(AffixKind.Suffix, SuffixSpecs);
            BuildCorruptionAffixes();
            foreach (V21AffixContentAsset asset in Resources.LoadAll<V21AffixContentAsset>("V21Content/Affixes"))
                if (asset != null && !string.IsNullOrEmpty(asset.stableId)) Affixes[asset.stableId] = asset.ToRuntime();
        }

        public static void Generate(ItemInstance item, ItemDefinition definition, int requestedSeed)
        {
            Ensure();
            if (item == null || definition == null) return;
            item.dataVersion = DataVersion;
            item.generationSeed = requestedSeed == 0
                ? V1Determinism.Combine(item.itemLevel, V1Determinism.StableHash(item.instanceId), definition.id, 1101)
                : requestedSeed;
            System.Random random = new System.Random(item.generationSeed);
            item.affixes.Clear();
            item.quality = 0;
            item.corrupted = false;
            item.corruptionId = string.Empty;
            item.craftedAffixId = string.Empty;
            if (definition.rarity == ItemRarity.Unique)
            {
                item.rarity = ItemRarity.Unique;
                return;
            }

            int rarityBonus = GameWorld.Instance != null && GameWorld.Instance.RunActive && GameWorld.Instance.Stats != null
                ? Mathf.RoundToInt(GameWorld.Instance.Stats.rarityFind * 100f) : 0;
            int roll = Mathf.Max(0, random.Next(100) - rarityBonus);
            ItemRarity rolled = roll < 34 ? ItemRarity.Common : roll < 76 ? ItemRarity.Magic : ItemRarity.Rare;
            if (definition.rarity == ItemRarity.Magic && rolled == ItemRarity.Common) rolled = ItemRarity.Magic;
            if (definition.rarity == ItemRarity.Rare) rolled = ItemRarity.Rare;
            item.rarity = rolled;
            int count = rolled == ItemRarity.Common ? 0 : rolled == ItemRarity.Magic ? random.Next(1, 3) : random.Next(3, 7);
            for (int i = 0; i < count; i++)
                if (!TryAddAffix(item, definition, random, false)) break;
        }

        public static void MigrateLegacy(ItemInstance item)
        {
            Ensure();
            if (item == null || item.dataVersion >= DataVersion) return;
            ItemDefinition definition = item.Definition;
            item.rarity = definition == null ? ItemRarity.Common : definition.rarity;
            if (item.affixes == null) item.affixes = new List<AffixRoll>();
            foreach (AffixRoll roll in item.affixes)
            {
                V11AffixDefinition affix;
                string migratedId = LegacyId(roll.id);
                if (!Affixes.TryGetValue(migratedId, out affix))
                {
                    roll.kind = AffixKind.Legacy;
                    roll.group = "legacy_" + roll.id;
                    roll.tier = 5;
                    continue;
                }
                roll.id = affix.id;
                roll.kind = affix.kind;
                roll.group = affix.group;
                roll.tier = 5;
                roll.minimumRoll = roll.value;
                roll.maximumRoll = roll.value;
                roll.local = affix.local;
            }
            item.dataVersion = DataVersion;
        }

        public static bool TryAddAffix(ItemInstance item, ItemDefinition definition, System.Random random, bool crafted)
        {
            Ensure();
            if (item == null || definition == null || item.rarity == ItemRarity.Common || item.rarity == ItemRarity.Unique) return false;
            if (crafted && !string.IsNullOrEmpty(item.craftedAffixId)) return false;
            int prefixCap = item.rarity == ItemRarity.Magic ? 1 : 3;
            int suffixCap = item.rarity == ItemRarity.Magic ? 1 : 3;
            int prefixes = item.affixes.Count(value => value.kind == AffixKind.Prefix);
            int suffixes = item.affixes.Count(value => value.kind == AffixKind.Suffix);
            List<V11AffixDefinition> candidates = Affixes.Values.Where(value => value.Allows(definition) &&
                !item.affixes.Any(existing => existing.group == value.group) &&
                (value.kind == AffixKind.Prefix ? prefixes < prefixCap : suffixes < suffixCap)).ToList();
            if (candidates.Count == 0) return false;
            V11AffixDefinition selected = Weighted(candidates, random);
            int tier = RollTier(item.itemLevel, random);
            float minimum, maximum; selected.RangeForTier(tier, out minimum, out maximum);
            float value = Mathf.Lerp(minimum, maximum, (float)random.NextDouble());
            AffixRoll roll = new AffixRoll
            {
                id = selected.id,
                value = value,
                tier = tier,
                kind = selected.kind,
                group = selected.group,
                minimumRoll = minimum,
                maximumRoll = maximum,
                crafted = crafted,
                locked = false,
                local = selected.local
            };
            item.affixes.Add(roll);
            if (crafted) item.craftedAffixId = roll.id;
            return true;
        }

        public static bool ApplyPlayerAffix(PlayerStats stats, AffixRoll roll, float scale)
        {
            Ensure();
            V11AffixDefinition definition;
            if (roll == null || !Affixes.TryGetValue(roll.id, out definition)) return false;
            float value = roll.value;
            // Local flat defenses/resources scale with the item's quality and
            // upgrade level. Global affixes and percentage modifiers do not.
            float localScale = roll.local ? scale : 1f;
            switch (definition.stat)
            {
                case "life": stats.maxHealth += value * localScale; break;
                case "mana": stats.maxMana += value * localScale; break;
                case "armor": stats.armor += value * localScale; break;
                case "resist": case "fire_resist": case "cold_resist": case "lightning_resist": case "toxic_resist": case "void_resist": stats.resistance += value; break;
                case "spell_power": stats.spellPower += value; break;
                case "crit_chance": stats.critChance += value; break;
                case "crit_damage": stats.critDamage += value; break;
                case "move_speed": stats.moveSpeed += value; break;
                case "cooldown": case "cast_speed": stats.cooldownMultiplier *= 1f - Mathf.Clamp(value, 0f, 0.35f); break;
                case "cooldown_on_kill": stats.cooldownOnKillReduction += value; break;
                case "trigger_energy": case "trigger_recovery": stats.triggerEnergy += value; break;
                case "mana_efficiency": stats.manaCostMultiplier *= 1f - Mathf.Clamp(value, 0f, 0.3f); break;
                case "healing": case "healing_received": stats.healingMultiplier += value; break;
                case "instability_resist": case "corruption_resist": stats.instabilityResistance += value; break;
                case "dodge_recovery": stats.dodgeCooldown *= 1f - Mathf.Clamp(value, 0f, 0.28f); break;
                case "life_regen": stats.lifeRegen += value; break;
                case "mana_regen": stats.manaRegen += value; break;
                case "shield_power": stats.shieldPower += value; break;
                case "elite_damage": stats.eliteDamage += value; break;
                case "boss_damage": stats.bossDamage += value; break;
                case "spell_leech": stats.spellLeech += value; break;
                case "gold_find": stats.goldFind += value; break;
                case "rarity_find": stats.rarityFind += value; break;
                case "pickup_range": stats.pickupRange += value; break;
                case "interaction_speed": stats.interactionSpeed += value; break;
                case "stun_resist": stats.stunResistance += value; break;
                case "knockback_resist": stats.knockbackResistance += value; break;
                case "damage_low_mana": stats.lowManaDamage += value; break;
                case "damage_high_health": stats.highHealthDamage += value; break;
                case "damage_while_shielded": stats.shieldedDamage += value; break;
                case "damage_after_dodge": stats.afterDodgeDamage += value; break;
                case "mana_to_power": stats.manaToPower += value; break;
                case "health_to_power": stats.healthToPower += value; break;
                case "first_cast_power": stats.firstCastPower += value; break;
            }
            return true;
        }

        public static void ApplySpellAffixes(CompiledSpell spell, IEnumerable<ItemInstance> equipped)
        {
            Ensure();
            if (spell == null || equipped == null) return;
            foreach (ItemInstance item in equipped)
            {
                if (item == null || item.affixes == null) continue;
                foreach (AffixRoll roll in item.affixes)
                {
                    V11AffixDefinition definition;
                    if (!Affixes.TryGetValue(roll.id, out definition)) continue;
                    float value = roll.value;
                    bool projectile = spell.delivery == SpellDelivery.Projectile || spell.delivery == SpellDelivery.Summon;
                    bool area = spell.delivery == SpellDelivery.Nova || spell.delivery == SpellDelivery.Meteor || spell.delivery == SpellDelivery.Zone || spell.explosionRadius > 0f;
                    if (definition.stat == "fire_damage" && spell.element == SpellElement.Fire ||
                        definition.stat == "cold_damage" && spell.element == SpellElement.Frost ||
                        definition.stat == "lightning_damage" && spell.element == SpellElement.Lightning ||
                        definition.stat == "toxic_damage" && spell.element == SpellElement.Toxic ||
                        definition.stat == "void_damage" && spell.element == SpellElement.Void ||
                        definition.stat == "arcane_damage" && spell.element == SpellElement.Arcane ||
                        definition.stat == "projectile_damage" && projectile || definition.stat == "area_damage" && area ||
                        definition.stat == "beam_damage" && spell.delivery == SpellDelivery.Beam ||
                        definition.stat == "zone_damage" && spell.delivery == SpellDelivery.Zone ||
                        definition.stat == "summon_damage" && spell.delivery == SpellDelivery.Summon ||
                        definition.stat == "melee_damage" && spell.delivery == SpellDelivery.Melee ||
                        definition.stat == "trigger_damage" && spell.triggers.Count > 0 ||
                        definition.stat == "ailment_damage" && (spell.poisonDamage > 0f || spell.freezeSeconds > 0f)) spell.damage *= 1f + value;
                    else if (definition.stat == "projectile_speed" && projectile) spell.speed *= 1f + value;
                    else if (definition.stat == "duration") { spell.lifetime *= 1f + value; spell.zoneDuration *= 1f + value; }
                    else if (definition.stat == "area_size") { spell.radius += value; spell.explosionRadius += value; }
                    else if (definition.stat == "homing" && projectile) spell.homingStrength += value;
                    else if (definition.stat == "chain_range" && projectile) spell.chainTargets += Mathf.Max(0, Mathf.RoundToInt(value));
                    else if (definition.stat == "pierce" && projectile) spell.pierce += Mathf.Max(0, Mathf.RoundToInt(value));
                    else if (definition.stat == "ailment_chance") { spell.poisonDamage *= 1f + value; spell.freezeSeconds *= 1f + value; }
                    else if (definition.stat == "ailment_duration") { spell.poisonDuration *= 1f + value; spell.freezeSeconds *= 1f + value; }
                    else if (definition.stat == "trigger_recovery" && spell.triggers.Count > 0)
                        foreach (TriggerSpec trigger in spell.triggers) trigger.energyCost *= 1f - Mathf.Clamp(value * 0.01f, 0f, 0.25f);
                }
                ApplyCorruptionToSpell(spell, item.corruptionId);
            }
        }

        public static string ItemName(ItemInstance item)
        {
            if (item == null || item.Definition == null) return "Unknown Item";
            if (item.rarity == ItemRarity.Unique) return item.Definition.displayName;
            AffixRoll prefix = item.affixes.FirstOrDefault(value => value.kind == AffixKind.Prefix);
            AffixRoll suffix = item.affixes.FirstOrDefault(value => value.kind == AffixKind.Suffix);
            string prefixName = prefix == null ? string.Empty : Get(prefix.id).displayName + " ";
            string suffixName = suffix == null ? string.Empty : " " + Get(suffix.id).displayName;
            return prefixName + item.Definition.displayName + suffixName;
        }

        public static string BuildTooltip(ItemInstance item, bool advanced)
        {
            if (item == null || item.Definition == null) return "Unknown equipment";
            if (ProfileManager.Current != null && ProfileManager.Current.accessibility.simplifiedDescriptions) advanced = false;
            ItemDefinition definition = item.Definition;
            List<string> lines = new List<string>
            {
                ItemName(item),
                item.rarity.ToString().ToUpperInvariant() + " · ITEM LEVEL " + item.itemLevel + " · " + definition.slot,
                (string.IsNullOrEmpty(definition.baseFamily) ? "Equipment" : definition.baseFamily) + (item.quality > 0 ? " · Quality +" + item.quality + "%" : string.Empty),
                "OWNERSHIP · " + (item.banked ? "PERMANENT" : "UNSECURED")
            };
            if (!string.IsNullOrEmpty(definition.implicitText)) lines.Add("IMPLICIT · " + definition.implicitText);
            if (item.corrupted) lines.Add("CORRUPTED · " + CorruptionDescription(item.corruptionId));
            foreach (AffixRoll roll in item.affixes.OrderBy(value => value.kind).ThenBy(value => value.id)) lines.Add(FormatAffix(roll, advanced));
            if (definition.rarity == ItemRarity.Unique) lines.Add("UNIQUE RULE · " + SimplifyDescription(definition.description));
            else if (!string.IsNullOrEmpty(definition.description)) lines.Add(SimplifyDescription(definition.description));
            return string.Join("\n", lines);
        }

        private static string SimplifyDescription(string text)
        {
            if (ProfileManager.Current == null || !ProfileManager.Current.accessibility.simplifiedDescriptions || string.IsNullOrEmpty(text)) return text;
            string value = text.Replace("increased", "more").Replace("decreased", "less")
                .Replace("subsequent", "later").Replace("approximately", "about")
                .Replace("replenishes", "restores").Replace("expenditure", "cost");
            int semicolon = value.IndexOf(';');
            return semicolon > 0 ? value.Substring(0, semicolon) + "." : value;
        }

        public static string BuildComparison(EquipmentInventory inventory, ItemInstance candidate, ProfileData profile)
        {
            if (inventory == null || candidate == null || candidate.Definition == null || profile == null) return "No comparison available.";
            EquipmentInventory simulated = new EquipmentInventory();
            foreach (KeyValuePair<EquipmentSlot, ItemInstance> pair in inventory.equipped) simulated.equipped[pair.Key] = pair.Value;
            ItemInstance current;
            inventory.equipped.TryGetValue(candidate.Definition.slot, out current);
            List<string> notes = new List<string>();
            if (candidate.Definition.slot == EquipmentSlot.Offhand)
            {
                ItemInstance weapon;
                if (simulated.equipped.TryGetValue(EquipmentSlot.Weapon, out weapon) && weapon != null && weapon.Definition != null && weapon.Definition.twoHanded)
                    return "Cannot equip while " + weapon.DisplayName + " is equipped. Remove the two-handed weapon at Home Base first.";
            }
            if (candidate.Definition.slot == EquipmentSlot.Weapon && candidate.Definition.twoHanded)
            {
                ItemInstance offhand;
                if (simulated.equipped.TryGetValue(EquipmentSlot.Offhand, out offhand) && offhand != null)
                { simulated.equipped.Remove(EquipmentSlot.Offhand); notes.Add("Removes Offhand: " + offhand.DisplayName); }
            }
            simulated.equipped[candidate.Definition.slot] = candidate;
            PlayerStats before = inventory.BuildStats(profile.healthRank, profile.manaRank, profile.powerRank);
            PlayerStats after = simulated.BuildStats(profile.healthRank, profile.manaRank, profile.powerRank);
            AddComparisonDelta(notes, "Maximum Health", before.maxHealth, after.maxHealth, false);
            AddComparisonDelta(notes, "Maximum Mana", before.maxMana, after.maxMana, false);
            AddComparisonDelta(notes, "Movement Speed", before.moveSpeed, after.moveSpeed, false);
            AddComparisonDelta(notes, "Armor", before.armor, after.armor, false);
            AddComparisonDelta(notes, "Resistance", before.resistance, after.resistance, true);
            AddComparisonDelta(notes, "Critical Chance", before.critChance, after.critChance, true);
            AddComparisonDelta(notes, "Critical Damage", before.critDamage, after.critDamage, true);
            AddComparisonDelta(notes, "Spell Power", before.spellPower, after.spellPower, true);
            AddComparisonDelta(notes, "Trigger Energy", before.triggerEnergy, after.triggerEnergy, false);
            AddComparisonDelta(notes, "Cooldown Multiplier", before.cooldownMultiplier, after.cooldownMultiplier, true);
            AddComparisonDelta(notes, "Mana Cost Multiplier", before.manaCostMultiplier, after.manaCostMultiplier, true);
            if (candidate.Definition.mutation != UniqueMutation.None) notes.Add("New Unique rule: " + candidate.Definition.description);
            notes.Insert(0, current == null ? "Compared with an empty " + candidate.Definition.slot + " slot." : "Replacing: " + current.DisplayName);
            return notes.Count == 1 ? notes[0] + "\nNo direct character-stat changes. Review its spell-specific affixes or Unique rule." : string.Join("\n", notes);
        }

        private static void AddComparisonDelta(List<string> lines, string label, float before, float after, bool percentage)
        {
            if (Mathf.Abs(after - before) < 0.0001f) return;
            string oldValue = percentage ? (before * 100f).ToString("0.#") + "%" : before.ToString("0.#");
            string newValue = percentage ? (after * 100f).ToString("0.#") + "%" : after.ToString("0.#");
            float delta = after - before;
            string deltaValue = percentage ? (delta * 100f).ToString("+0.#;-0.#") + "%" : delta.ToString("+0.#;-0.#");
            lines.Add(label + ": " + oldValue + " → " + newValue + " (" + deltaValue + ")");
        }

        public static string FormatAffix(AffixRoll roll, bool advanced)
        {
            if (roll == null) return string.Empty;
            V11AffixDefinition definition;
            if (!Affixes.TryGetValue(roll.id, out definition)) return "[LEGACY] " + roll.id + " " + roll.value.ToString("0.##");
            string value = definition.percentage ? (roll.value * 100f).ToString("0.#") + "%" : roll.value.ToString("0.#");
            string type = definition.kind == AffixKind.Prefix ? "PREFIX" : definition.kind == AffixKind.Suffix ? "SUFFIX" : definition.kind.ToString().ToUpperInvariant();
            string text = type + " · " + EffectText(definition.stat, value);
            if (roll.crafted) text += " · CRAFTED";
            if (roll.locked) text += " · LOCKED";
            if (advanced) text += " · T" + roll.tier + " [" + FormatRange(definition, roll.minimumRoll, roll.maximumRoll) + "] · " +
                                  (roll.local ? "LOCAL" : "GLOBAL") + " · GROUP " + definition.group + " · TAGS " + string.Join(",", definition.tags ?? new string[0]);
            return text;
        }

        public static bool TryCraft(ItemInstance item, ForgeAction action, RunDirector run, out string message)
        {
            Ensure();
            if (!CanCraft(item, action, out message)) return false;
            ForgeCost cost = CostFor(action, item.itemLevel);
            if (!ProfileManager.TrySpendForgeMaterials(cost, out message)) return false;
            ItemSaveData before = item.ToSaveData(false);
            System.Random random = new System.Random(V1Determinism.Combine(item.generationSeed, item.affixes.Count, action.ToString(), item.quality));
            bool success = false;
            try
            {
                switch (action)
                {
                    case ForgeAction.UpgradeRarity:
                        if (item.rarity == ItemRarity.Common) { item.rarity = ItemRarity.Magic; success = TryAddAffix(item, item.Definition, random, true); }
                        else if (item.rarity == ItemRarity.Magic)
                        {
                            item.rarity = ItemRarity.Rare;
                            success = true;
                            if (string.IsNullOrEmpty(item.craftedAffixId)) TryAddAffix(item, item.Definition, random, true);
                        }
                        break;
                    case ForgeAction.AddAffix:
                        if (string.IsNullOrEmpty(item.craftedAffixId)) success = TryAddAffix(item, item.Definition, random, true);
                        break;
                    case ForgeAction.RerollAffix:
                        AffixRoll reroll = item.affixes.Where(value => !value.locked).OrderBy(value => random.Next()).FirstOrDefault();
                        if (reroll != null) { item.affixes.Remove(reroll); if (item.craftedAffixId == reroll.id) item.craftedAffixId = string.Empty; success = TryAddAffix(item, item.Definition, random, reroll.crafted); }
                        break;
                    case ForgeAction.ImproveRoll:
                        AffixRoll improve = item.affixes.Where(value => !value.locked && value.value < value.maximumRoll).OrderBy(value => random.Next()).FirstOrDefault();
                        if (improve != null) { improve.value = Mathf.Min(improve.maximumRoll, Mathf.Lerp(improve.value, improve.maximumRoll, 0.5f)); success = true; }
                        break;
                    case ForgeAction.RemoveAffix:
                        AffixRoll remove = item.affixes.Where(value => !value.locked).OrderBy(value => random.Next()).FirstOrDefault();
                        if (remove != null) { item.affixes.Remove(remove); if (item.craftedAffixId == remove.id) item.craftedAffixId = string.Empty; success = true; }
                        break;
                    case ForgeAction.LockAffix:
                        AffixRoll lockRoll = item.affixes.FirstOrDefault(value => !value.locked);
                        if (lockRoll != null && item.affixes.All(value => !value.locked)) { lockRoll.locked = true; success = true; }
                        break;
                    case ForgeAction.AddQuality:
                        if (item.quality < 20) { item.quality = Mathf.Min(20, item.quality + 5); success = true; }
                        break;
                    case ForgeAction.Corrupt:
                        if (!item.corrupted) { ApplyCorruption(item, random); success = true; }
                        break;
                }
            }
            catch (Exception exception)
            {
                success = false;
                if (V1Diagnostics.Instance != null) V1Diagnostics.Instance.Recover("Forge transaction rolled back safely.", exception);
            }
            if (!success)
            {
                RestoreItem(item, before);
                ProfileManager.RefundForgeMaterials(cost);
            }
            message = success ? action + " completed on " + ItemName(item) + " for " + ProfileManager.PermanentForgeCostDescription(cost) + "." : "That Forge action has no legal result for this item. All materials were refunded.";
            if (success)
                V12CombatEventBus.Publish(V12CombatEventType.CraftCommitted, action.ToString(), item.instanceId,
                    cost.gold + cost.dust + cost.runes * 10f + cost.cores * 40f, Vector3.zero, ItemName(item));
            return success;
        }

        public static bool CanCraft(ItemInstance item, ForgeAction action, out string reason)
        {
            Ensure();
            if (item == null || item.Definition == null) { reason = "Select a valid secured item."; return false; }
            if (GameWorld.Instance != null && GameWorld.Instance.RunActive) { reason = "Available only at the Home Base Forge."; return false; }
            if (item.corrupted && action != ForgeAction.Corrupt) { reason = "Corrupted items cannot use ordinary Forge actions."; return false; }

            switch (action)
            {
                case ForgeAction.UpgradeRarity:
                    if (item.rarity == ItemRarity.Rare || item.rarity == ItemRarity.Unique) { reason = "Already at the highest craftable rarity."; return false; }
                    break;
                case ForgeAction.AddAffix:
                    if (item.rarity == ItemRarity.Common) { reason = "Upgrade this item to Magic first."; return false; }
                    if (item.rarity == ItemRarity.Unique) { reason = "Unique items cannot gain standard affixes."; return false; }
                    if (!string.IsNullOrEmpty(item.craftedAffixId)) { reason = "This item already has a crafted affix."; return false; }
                    int prefixCap = item.rarity == ItemRarity.Magic ? 1 : 3;
                    int suffixCap = item.rarity == ItemRarity.Magic ? 1 : 3;
                    int prefixes = item.affixes.Count(value => value.kind == AffixKind.Prefix);
                    int suffixes = item.affixes.Count(value => value.kind == AffixKind.Suffix);
                    bool candidate = Affixes.Values.Any(value => value.Allows(item.Definition) && !item.affixes.Any(existing => existing.group == value.group) &&
                        (value.kind == AffixKind.Prefix ? prefixes < prefixCap : suffixes < suffixCap));
                    if (!candidate) { reason = "No legal affix slot or affix family remains."; return false; }
                    break;
                case ForgeAction.RerollAffix:
                    if (!item.affixes.Any(value => !value.locked)) { reason = "No unlocked affix can be rerolled."; return false; }
                    break;
                case ForgeAction.ImproveRoll:
                    if (!item.affixes.Any(value => !value.locked && value.value < value.maximumRoll)) { reason = "No unlocked roll can be improved."; return false; }
                    break;
                case ForgeAction.RemoveAffix:
                    if (!item.affixes.Any(value => !value.locked)) { reason = "No unlocked affix can be removed."; return false; }
                    break;
                case ForgeAction.LockAffix:
                    if (item.affixes.Count == 0) { reason = "This item has no affix to protect."; return false; }
                    if (item.affixes.Any(value => value.locked)) { reason = "Only one affix may be protected."; return false; }
                    break;
                case ForgeAction.AddQuality:
                    if (item.quality >= 20) { reason = "Quality is already at the 20% cap."; return false; }
                    break;
                case ForgeAction.Corrupt:
                    if (item.corrupted) { reason = "This item is already corrupted."; return false; }
                    break;
            }
            reason = string.Empty;
            return true;
        }

        public static bool CanServiceReroll(ItemInstance item, out string reason)
        {
            Ensure();
            if (item == null || item.Definition == null) { reason = "No valid item was selected."; return false; }
            if (item.corrupted) { reason = "Corrupted items must be cleansed before an affix can be rerolled."; return false; }
            if (item.rarity != ItemRarity.Magic && item.rarity != ItemRarity.Rare)
            { reason = "Affix rerolling supports Magic and Rare items only."; return false; }
            if (!item.affixes.Any(value => value != null && !value.locked && (value.kind == AffixKind.Prefix || value.kind == AffixKind.Suffix)))
            { reason = "The item has no unlocked Prefix or Suffix."; return false; }
            reason = string.Empty;
            return true;
        }

        public static bool TryServiceReroll(ItemInstance item, int seed, out string before, out string after, out string message)
        {
            before = string.Empty;
            after = string.Empty;
            if (!CanServiceReroll(item, out message)) return false;
            ItemSaveData snapshot = item.ToSaveData(false);
            System.Random random = new System.Random(V1Determinism.Combine(seed, item.affixes.Count, "shop_affix_reroll", item.instanceId.GetHashCode()));
            AffixRoll replaced = item.affixes.Where(value => value != null && !value.locked &&
                (value.kind == AffixKind.Prefix || value.kind == AffixKind.Suffix)).OrderBy(value => random.Next()).First();
            before = FormatAffix(replaced, true);
            bool crafted = replaced.crafted;
            item.affixes.Remove(replaced);
            if (item.craftedAffixId == replaced.id) item.craftedAffixId = string.Empty;
            if (!TryAddAffix(item, item.Definition, random, crafted))
            {
                RestoreItem(item, snapshot);
                message = "No legal replacement affix was available; the item was not changed.";
                return false;
            }
            AffixRoll replacement = item.affixes.FirstOrDefault(value => value != null &&
                !snapshot.affixes.Any(old => old != null && old.id == value.id && Mathf.Approximately(old.value, value.value)));
            if (replacement == null) replacement = item.affixes.LastOrDefault();
            after = FormatAffix(replacement, true);
            message = item.DisplayName + ": " + before + " was replaced with " + after + ". Locked affixes were preserved.";
            return true;
        }

        private static void RestoreItem(ItemInstance item, ItemSaveData saved)
        {
            item.instanceId = saved.instanceId;
            item.definitionId = saved.definitionId;
            item.itemLevel = saved.itemLevel;
            item.upgradeLevel = saved.upgradeLevel;
            item.favorite = saved.favorite;
            item.junk = saved.junk;
            item.locked = saved.locked;
            item.corrupted = saved.corrupted;
            item.banked = saved.banked;
            item.dataVersion = saved.dataVersion;
            item.rarity = saved.rarity;
            item.quality = saved.quality;
            item.generationSeed = saved.generationSeed;
            item.corruptionId = saved.corruptionId;
            item.craftedAffixId = saved.craftedAffixId;
            item.affixes = saved.affixes == null ? new List<AffixRoll>() : saved.affixes.Select(value => value.Clone()).ToList();
        }

        public static ForgeCost CostFor(ForgeAction action, int itemLevel)
        {
            int levelMaterial = Mathf.Max(0, itemLevel / 6);
            switch (action)
            {
                case ForgeAction.UpgradeRarity: return new ForgeCost(0, 8 + levelMaterial, 1);
                case ForgeAction.AddAffix: return new ForgeCost(0, 6 + levelMaterial, 1);
                case ForgeAction.RerollAffix: return new ForgeCost(0, 5 + levelMaterial);
                case ForgeAction.ImproveRoll: return new ForgeCost(0, 4 + levelMaterial);
                case ForgeAction.RemoveAffix: return new ForgeCost(0, 5 + levelMaterial);
                case ForgeAction.LockAffix: return new ForgeCost(0, 3 + levelMaterial, 1);
                case ForgeAction.AddQuality: return new ForgeCost(0, 5 + levelMaterial);
                case ForgeAction.Corrupt: return new ForgeCost(0, 0, 0, 1);
                default: return new ForgeCost(0, 0);
            }
        }

        public static bool ApplyDeterministicCorruption(ItemInstance item, int seed)
        {
            Ensure();
            if (item == null || item.Definition == null || item.corrupted) return false;
            ApplyCorruption(item, new System.Random(seed));
            return item.corrupted && !string.IsNullOrEmpty(item.corruptionId);
        }

        public static int ValidateGeneratedItems(int samples)
        {
            Ensure();
            int failures = 0;
            List<ItemDefinition> bases = DemoCatalog.AllItems.Where(value => value.rarity != ItemRarity.Unique &&
                (!string.IsNullOrEmpty(value.baseFamily) || !string.IsNullOrEmpty(value.setId))).ToList();
            if (bases.Count < 32) { Debug.LogError("Item-system validation: fewer than 32 legal procedural/set bases."); failures++; }
            if (Affixes.Values.Count(value => value.kind == AffixKind.Prefix || value.kind == AffixKind.Suffix) < 84)
            { Debug.LogError("1.1 item validation: fewer than 84 standard affix families."); failures++; }
            string[] reactiveStats =
            {
                "fire_damage", "cold_damage", "lightning_damage", "toxic_damage", "void_damage", "arcane_damage",
                "projectile_damage", "area_damage", "beam_damage", "zone_damage", "summon_damage", "melee_damage",
                "trigger_damage", "ailment_damage", "projectile_speed", "duration", "area_size", "homing",
                "chain_range", "pierce", "ailment_chance", "ailment_duration", "trigger_recovery", "shield_power"
            };
            if (reactiveStats.Count(stat => Affixes.Values.Any(value => value.stat == stat)) < 24)
            { Debug.LogError("1.1 item validation: fewer than 24 spell-reactive affix families."); failures++; }
            if (DemoCatalog.AllItems.Count(value => value.rarity == ItemRarity.Unique) < 18) { Debug.LogError("1.1 item validation: fewer than 18 Unique items."); failures++; }
            if (Corruptions.Length < 12) { Debug.LogError("1.1 item validation: fewer than 12 corruption outcomes."); failures++; }
            for (int i = 0; i < samples && bases.Count > 0; i++)
            {
                ItemDefinition definition = bases[i % bases.Count];
                ItemInstance item = new ItemInstance(definition.id, 1 + i % 30);
                Generate(item, definition, V1Determinism.Combine(i, item.itemLevel, definition.id, 11));
                int prefixCount = item.affixes.Count(value => value.kind == AffixKind.Prefix);
                int suffixCount = item.affixes.Count(value => value.kind == AffixKind.Suffix);
                int cap = item.rarity == ItemRarity.Magic ? 1 : item.rarity == ItemRarity.Rare ? 3 : 0;
                if (prefixCount > cap || suffixCount > cap || item.affixes.Select(value => value.group).Distinct().Count() != item.affixes.Count ||
                    item.affixes.Any(value => value.tier < 1 || value.tier > 5 || !Get(value.id).Allows(definition)))
                { failures++; if (failures < 8) Debug.LogError("1.1 illegal generated item at sample " + i + ": " + ItemName(item)); }
            }
            return failures;
        }

        public static V11AffixDefinition Get(string id)
        {
            Ensure();
            V11AffixDefinition value; Affixes.TryGetValue(id, out value); return value;
        }

        private static void ApplyCorruption(ItemInstance item, System.Random random)
        {
            List<string> eligible = Corruptions.Where(entry => CorruptionAllowed(entry.Split('|')[0], item.Definition)).ToList();
            if (eligible.Count == 0) eligible.Add(Corruptions[0]);
            string[] parts = eligible[random.Next(eligible.Count)].Split('|');
            item.corrupted = true;
            item.corruptionId = parts[0];
            if (parts[2] == "quality") item.quality += Mathf.RoundToInt(Parse(parts[3]));
            else
            {
                V11AffixDefinition pseudo = Get("corrupt_" + parts[0]);
                item.affixes.Add(new AffixRoll { id = pseudo.id, value = pseudo.baseMinimum, minimumRoll = pseudo.baseMinimum, maximumRoll = pseudo.baseMaximum,
                    tier = 1, kind = AffixKind.Corruption, group = "corruption" });
            }
        }

        public static void ApplyCorruptionToPlayer(PlayerStats stats, string id)
        {
            if (stats == null || string.IsNullOrEmpty(id)) return;
            if (id == "unstable_might") stats.instabilityResistance -= 12f;
            else if (id == "blood_price") stats.spellPower += 0.14f;
            else if (id == "glass_mind") stats.armor = Mathf.Max(0f, stats.armor - 15f);
            else if (id == "swift_ruin") stats.dodgeCooldown *= 1.18f;
            else if (id == "trigger_brand") stats.instabilityResistance -= 6f;
        }

        private static bool CorruptionAllowed(string id, ItemDefinition item)
        {
            if (item == null) return false;
            string[] tags = item.itemTags ?? new string[0];
            if (id == "swift_ruin") return item.slot == EquipmentSlot.Boots || tags.Contains("mobile");
            if (id == "trigger_brand") return tags.Contains("trigger");
            if (id == "glass_mind" || id == "fractured_echo") return tags.Contains("caster") || tags.Contains("ward");
            if (id == "storm_scar" || id == "ember_scar" || id == "winter_scar" || id == "venom_scar" || id == "void_scar")
                return tags.Contains("caster") || tags.Contains("weapon");
            return true;
        }

        private static void ApplyCorruptionToSpell(CompiledSpell spell, string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (id == "storm_scar" && spell.element == SpellElement.Lightning || id == "ember_scar" && spell.element == SpellElement.Fire ||
                id == "winter_scar" && spell.element == SpellElement.Frost || id == "venom_scar" && spell.element == SpellElement.Toxic ||
                id == "void_scar" && spell.element == SpellElement.Void) spell.damage *= 1.18f;
            if (id == "unstable_might") { spell.damage *= 1.12f; spell.instability += 12f; }
            if (id == "fractured_echo") { spell.cooldown *= 0.92f; spell.manaCost *= 1.1f; }
            if (id == "trigger_brand") spell.instability += 6f;
        }

        private static string CorruptionDescription(string id)
        {
            string entry = Corruptions.FirstOrDefault(value => value.StartsWith(id + "|", StringComparison.Ordinal));
            return string.IsNullOrEmpty(entry) ? id : entry.Split('|')[4];
        }

        private static V11AffixDefinition Weighted(List<V11AffixDefinition> candidates, System.Random random)
        {
            int total = candidates.Sum(value => Mathf.Max(1, value.weight));
            int roll = random.Next(total);
            foreach (V11AffixDefinition candidate in candidates)
            {
                roll -= Mathf.Max(1, candidate.weight);
                if (roll < 0) return candidate;
            }
            return candidates[candidates.Count - 1];
        }

        private static int RollTier(int itemLevel, System.Random random)
        {
            int best = itemLevel >= 24 ? 1 : itemLevel >= 16 ? 2 : itemLevel >= 10 ? 3 : itemLevel >= 5 ? 4 : 5;
            int tier = 5;
            for (int value = 4; value >= best; value--)
                if (random.NextDouble() < (value == best ? 0.28 : 0.48)) tier = value;
            return tier;
        }

        private static void BuildAffixes(AffixKind kind, string[] specs)
        {
            foreach (string spec in specs)
            {
                string[] p = spec.Split('|');
                V11AffixDefinition definition = new V11AffixDefinition
                {
                    id = p[0], displayName = p[1], stat = p[2], kind = kind, group = p[3], baseMinimum = Parse(p[4]), baseMaximum = Parse(p[5]),
                    percentage = p[6] == "p", tags = p[7].Split(','), slots = ParseSlots(p[8]), weight = int.Parse(p[9]), local = p.Length > 10 && p[10] == "l"
                };
                Affixes[definition.id] = definition;
            }
        }

        private static void BuildCorruptionAffixes()
        {
            foreach (string entry in Corruptions)
            {
                string[] parts = entry.Split('|');
                if (parts[2] == "quality") continue;
                V11AffixDefinition definition = new V11AffixDefinition
                {
                    id = "corrupt_" + parts[0], displayName = parts[1], stat = parts[2], kind = AffixKind.Corruption,
                    group = "corruption", tags = new[] { "corruption" }, slots = new EquipmentSlot[0],
                    baseMinimum = Parse(parts[3]), baseMaximum = Parse(parts[3]), percentage = IsPercentStat(parts[2]), weight = 1
                };
                Affixes[definition.id] = definition;
            }
        }

        private static EquipmentSlot[] ParseSlots(string value)
        {
            if (value == "*") return new EquipmentSlot[0];
            return value.Split(',').Select(name => (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), name)).ToArray();
        }

        private static float Parse(string value) { return float.Parse(value, CultureInfo.InvariantCulture); }
        private static string FormatRange(V11AffixDefinition definition, float minimum, float maximum)
        {
            return definition.percentage ? (minimum * 100f).ToString("0.#") + "–" + (maximum * 100f).ToString("0.#") + "%" : minimum.ToString("0.#") + "–" + maximum.ToString("0.#");
        }

        private static string EffectText(string stat, string value)
        {
            string label = stat.Replace('_', ' ');
            if (stat.EndsWith("resist")) return "+" + value + " " + label;
            if (stat == "life" || stat == "mana" || stat == "armor" || stat == "trigger_energy") return "+" + value + " " + label;
            if (stat == "chain_range" || stat == "pierce") return "+" + value + " " + label;
            return "+" + value + " " + label;
        }

        private static bool IsPercentStat(string stat)
        {
            return stat != "life" && stat != "mana" && stat != "armor" && stat != "trigger_energy" && stat != "chain_range" && stat != "pierce" && stat != "area_size";
        }

        private static string LegacyId(string id)
        {
            switch (id)
            {
                case "life": return "stout"; case "mana": return "lucid"; case "armor": return "plated"; case "resist": return "of_warding";
                case "power": return "potent"; case "crit": return "of_precision"; case "speed": return "of_haste"; case "cooldown": return "of_recurrence";
                default: return id;
            }
        }

        private static readonly string[] PrefixSpecs =
        {
            "stout|Stout|life|life|8|18|n|armor,mobile,ward|*|100|l", "vital|Vital|life|life_high|12|24|n|armor|Helmet,Chest,Pants|72|l",
            "lucid|Lucid|mana|mana|7|16|n|ward,caster|*|100|l", "bottomless|Bottomless|mana|mana_high|12|23|n|ward|Helmet,Chest,Offhand|65|l",
            "plated|Plated|armor|armor|4|11|n|armor|*|100|l", "fortified|Fortified|armor|armor_high|8|16|n|armor|Chest,Pants,LeftShoulder,RightShoulder|62|l",
            "potent|Potent|spell_power|spell_power|0.03|0.075|p|caster,weapon|*|88", "archmage|Archmage's|spell_power|spell_power_high|0.05|0.10|p|caster|Weapon,Offhand,Helmet|48",
            "searing|Searing|fire_damage|fire_damage|0.05|0.11|p|caster,weapon|*|78", "glacial|Glacial|cold_damage|cold_damage|0.05|0.11|p|caster,weapon|*|78",
            "stormbound|Stormbound|lightning_damage|lightning_damage|0.05|0.11|p|caster,weapon|*|78", "viridian|Viridian|toxic_damage|toxic_damage|0.05|0.11|p|caster,weapon|*|78",
            "riftborn|Riftborn|void_damage|void_damage|0.05|0.11|p|caster,weapon|*|72", "runic|Runic|arcane_damage|arcane_damage|0.05|0.11|p|caster,weapon|*|72",
            "ballistic|Ballistic|projectile_damage|projectile_damage|0.05|0.12|p|caster,weapon|Weapon,LeftGlove,RightGlove|74", "cataclysmic|Cataclysmic|area_damage|area_damage|0.05|0.12|p|caster|Chest,LeftShoulder,RightShoulder,Weapon|70",
            "circuit|Circuit-Forged|trigger_damage|trigger_damage|0.06|0.13|p|trigger,caster|Helmet,RightShoulder,Offhand,Weapon|56", "malignant|Malignant|ailment_damage|ailment_damage|0.06|0.14|p|caster|LeftGlove,RightGlove,Weapon|62",
            "shielded|Shielded|shield_power|shield_power|0.05|0.13|p|ward|Offhand,Chest,LeftShoulder,RightShoulder|68", "restorative|Restorative|healing|healing|0.04|0.10|p|ward,armor|Helmet,Chest,Offhand|70",
            "ember_laced|Ember-Laced|fire_damage|added_fire|0.04|0.09|p|weapon|Weapon,LeftGlove,RightGlove|62", "rime_laced|Rime-Laced|cold_damage|added_cold|0.04|0.09|p|weapon|Weapon,LeftGlove,RightGlove|62",
            "spark_laced|Spark-Laced|lightning_damage|added_lightning|0.04|0.09|p|weapon|Weapon,LeftGlove,RightGlove|62", "focused|Focused|beam_damage|beam_damage|0.06|0.13|p|caster|Weapon,Offhand,Helmet|65",
            "lingering|Lingering|zone_damage|zone_damage|0.06|0.13|p|caster|Weapon,Offhand,Chest|65", "familiar|Familiar-Bound|summon_damage|summon_damage|0.06|0.14|p|caster|Helmet,LeftShoulder,RightShoulder,Offhand|58",
            "duelist|Duelist's|melee_damage|melee_damage|0.07|0.15|p|weapon|Weapon,LeftGlove,RightGlove|62", "aegisborn|Aegisborn|damage_while_shielded|shielded_damage|0.04|0.10|p|ward|*|64",
            "fleetbound|Fleetbound|damage_after_dodge|dodge_damage|0.05|0.12|p|mobile|Boots,LeftGlove,RightGlove|58", "desperate|Desperate|damage_low_mana|low_mana_damage|0.06|0.14|p|caster|*|52",
            "exalted|Exalted|damage_high_health|high_health_damage|0.05|0.11|p|armor|Chest,Helmet,LeftShoulder,RightShoulder|58", "recursive|Recursive|trigger_damage|trigger_depth|0.05|0.12|p|trigger|Helmet,Offhand,Weapon|48",
            "opening|Opening|first_cast_power|first_cast|0.07|0.16|p|caster|Weapon,LeftGlove,RightGlove|48", "furnace|Furnace|mana_to_power|mana_power|0.04|0.10|p|caster|Weapon,Offhand|44",
            "sanguine|Sanguine|health_to_power|health_power|0.05|0.12|p|armor,caster|Chest,Weapon|42", "warding|Warding|shield_power|ward_on_cast|0.05|0.12|p|ward|Offhand,Helmet|56",
            "afterimage|Afterimage|shield_power|ward_on_dodge|0.05|0.12|p|mobile,ward|Boots,LeftShoulder,RightShoulder|52", "steadfast|Steadfast|armor|armor_casting|5|12|n|armor|Chest,Pants|66|l",
            "regenerating|Regenerating|life_regen|life_regen|2|5|n|armor|Helmet,Chest,Pants|54", "meditative|Meditative|mana_regen|mana_regen|2|5|n|ward|Helmet,Chest,Offhand|54",
            "devouring|Devouring|spell_leech|spell_leech|0.02|0.05|p|caster|Weapon,LeftGlove,RightGlove|38", "executioner|Executioner's|cooldown_on_kill|kill_cooldown|0.02|0.06|p|weapon|Weapon,LeftGlove,RightGlove|42"
        };

        private static readonly string[] SuffixSpecs =
        {
            "of_embers|of Embers|fire_resist|fire_resist|0.04|0.10|p|any|*|100", "of_rime|of Rime|cold_resist|cold_resist|0.04|0.10|p|any|*|100",
            "of_storms|of Storms|lightning_resist|lightning_resist|0.04|0.10|p|any|*|100", "of_antidotes|of Antidotes|toxic_resist|toxic_resist|0.04|0.10|p|any|*|95",
            "of_the_rift|of the Rift|void_resist|void_resist|0.04|0.10|p|any|*|90", "of_warding|of Warding|resist|all_resist|0.025|0.065|p|any|*|55",
            "of_precision|of Precision|crit_chance|crit_chance|0.025|0.065|p|caster,weapon|*|78", "of_ruin|of Ruin|crit_damage|crit_damage|0.06|0.16|p|caster,weapon|Weapon,LeftGlove,RightGlove,Helmet|68",
            "of_cadence|of Cadence|cast_speed|cast_speed|0.04|0.10|p|caster|Weapon,LeftGlove,RightGlove,Offhand|75", "of_recurrence|of Recurrence|cooldown|cooldown|0.025|0.065|p|caster|*|64",
            "of_haste|of Haste|move_speed|move_speed|0.25|0.65|n|mobile|Boots|88", "of_evasion|of Evasion|dodge_recovery|dodge_recovery|0.035|0.085|p|mobile|Boots,Pants|76",
            "of_velocity|of Velocity|projectile_speed|projectile_speed|0.06|0.16|p|caster,weapon|Weapon,LeftGlove,RightGlove|74", "of_persistence|of Persistence|duration|duration|0.06|0.16|p|caster|Helmet,Chest,Offhand|70",
            "of_expansion|of Expansion|area_size|area_size|0.12|0.32|n|caster|Weapon,LeftShoulder,RightShoulder,Chest|62", "of_economy|of Economy|mana_efficiency|mana_efficiency|0.025|0.07|p|caster,ward|*|72",
            "of_capacity|of Capacity|trigger_energy|trigger_energy|6|15|n|trigger|Helmet,RightShoulder,Offhand,Weapon|72", "of_relaying|of Relaying|trigger_recovery|trigger_recovery|4|11|n|trigger|Helmet,RightShoulder,Offhand|55",
            "of_infection|of Infection|ailment_chance|ailment_chance|0.05|0.13|p|caster|Weapon,LeftGlove,RightGlove|68", "of_torment|of Torment|ailment_duration|ailment_duration|0.06|0.16|p|caster|Weapon,LeftGlove,RightGlove,Helmet|66",
            "of_venom|of Venom|toxic_damage|poison_strength|0.05|0.12|p|caster|Weapon,LeftGlove,RightGlove|64", "of_winter|of Winter|cold_damage|freeze_strength|0.05|0.12|p|caster|Weapon,LeftGlove,RightGlove|64",
            "of_arcing|of Arcing|chain_range|chain_range|0.35|0.8|n|caster|Weapon,Offhand,RightGlove|52", "of_seeking|of Seeking|homing|homing|0.4|1.1|n|caster|Weapon,Offhand,Helmet|52",
            "of_puncture|of Puncture|pierce|pierce|0.4|0.9|n|weapon|Weapon,LeftGlove,RightGlove|50", "of_contagion|of Contagion|ailment_damage|status_spread|0.05|0.12|p|caster|Helmet,LeftShoulder,RightShoulder|48",
            "of_clarity|of Clarity|mana|crit_mana|4|10|n|ward,caster|Helmet,Offhand|62", "of_feasting|of Feasting|life|kill_health|4|10|n|armor|Chest,Pants,Weapon|62",
            "of_reflexes|of Reflexes|shield_power|dodge_shield|0.04|0.10|p|mobile,ward|Boots,LeftShoulder,RightShoulder|54", "of_serenity|of Serenity|cast_speed|shielded_cast|0.04|0.10|p|ward,caster|Offhand,Helmet|54",
            "of_last_stand|of the Last Stand|resist|low_health_resist|0.035|0.085|p|armor|Chest,Pants,Helmet|56", "of_hunters|of Hunters|elite_damage|elite_damage|0.05|0.12|p|weapon|Weapon,LeftGlove,RightGlove|50",
            "of_kings|of Kings|boss_damage|boss_damage|0.05|0.12|p|weapon|Weapon,Helmet,Offhand|46", "of_reaching|of Reaching|pickup_range|pickup_range|0.3|0.8|n|mobile|Boots,LeftGlove,RightGlove|45",
            "of_fortune|of Fortune|gold_find|gold_find|0.05|0.13|p|any|*|38", "of_discovery|of Discovery|rarity_find|rarity_find|0.04|0.10|p|any|Helmet,Boots,LeftGlove,RightGlove|34",
            "of_composure|of Composure|stun_resist|stun_resist|0.06|0.16|p|armor|Chest,Pants,Helmet|62", "of_anchoring|of Anchoring|knockback_resist|knockback_resist|0.06|0.16|p|armor|Chest,Pants,Boots|62",
            "of_rituals|of Rituals|interaction_speed|interaction_speed|0.06|0.16|p|any|LeftGlove,RightGlove,Boots|48", "of_mending|of Mending|healing_received|healing_received|0.04|0.11|p|armor,ward|Helmet,Chest,Pants|58",
            "of_purity|of Purity|corruption_resist|corruption_resist|2|6|n|ward|Helmet,Offhand,LeftShoulder,RightShoulder|40", "of_stability|of Stability|instability_resist|instability_resist|2|7|n|caster,ward|Helmet,Offhand,Chest|58"
        };
    }
}
