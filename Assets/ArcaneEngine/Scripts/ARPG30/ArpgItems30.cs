using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public static class ArpgItems30
    {
        public static ArpgItem30 GenerateItem(int itemLevel, ArpgClass30 affinity, int seed, float rarityBonus)
        {
            ArpgContent30.Ensure();
            System.Random random = new System.Random(seed);
            List<ArpgItemBaseDefinition30> bases = ArpgContent30.ItemBases
                .Where(value => value.requiredLevel <= itemLevel && (value.affinity == ArpgClass30.Unchosen || value.affinity == affinity))
                .ToList();
            if (bases.Count == 0) bases = ArpgContent30.ItemBases.Where(value => value.requiredLevel <= itemLevel).ToList();
            if (bases.Count == 0) throw new InvalidOperationException("No item base is valid for item level " + itemLevel + ".");
            ArpgItemBaseDefinition30 selectedBase = bases[random.Next(bases.Count)];
            float roll = (float)random.NextDouble() + Mathf.Clamp(rarityBonus, 0f, 0.3f);
            ArpgItemRarity30 rarity = roll > 1.23f ? ArpgItemRarity30.Exceptional : roll > 0.83f ? ArpgItemRarity30.Rare : roll > 0.38f ? ArpgItemRarity30.Magic : ArpgItemRarity30.Normal;
            ArpgItem30 item = new ArpgItem30
            {
                instanceId = Guid.NewGuid().ToString("N"),
                baseId = selectedBase.id,
                displayName = selectedBase.displayName,
                slot = selectedBase.slot,
                rarity = rarity,
                itemLevel = Mathf.Max(1, itemLevel),
                quality = 0
            };
            int affixes = rarity == ArpgItemRarity30.Normal ? 0 : rarity == ArpgItemRarity30.Magic ? random.Next(1, 3) : rarity == ArpgItemRarity30.Exceptional ? random.Next(5, 7) : random.Next(3, 7);
            for (int index = 0; index < affixes; index++) AddRandomAffix(item, random, null);
            RefreshName(item);
            return item;
        }

        public static ArpgMapItem30 GenerateMap(int tier, int seed, ArpgMapRarity30 rarity, bool corrupted)
        {
            ArpgContent30.Ensure();
            tier = Mathf.Clamp(tier, 0, 39);
            List<ArpgMapDefinition30> maps = ArpgContent30.Maps.Where(value => value.tier == tier).ToList();
            System.Random random = new System.Random(seed);
            if (maps.Count == 0) throw new InvalidOperationException("No map definition exists for tier " + tier + ".");
            ArpgMapDefinition30 map = maps[random.Next(maps.Count)];
            ArpgMapItem30 item = new ArpgMapItem30
            {
                instanceId = Guid.NewGuid().ToString("N"),
                mapId = map.id,
                tier = tier,
                seed = seed,
                rarity = rarity,
                corrupted = corrupted,
                quality = 0
            };
            int affixCount = rarity == ArpgMapRarity30.Normal ? 0 : rarity == ArpgMapRarity30.Magic ? 2 : 4;
            List<ArpgMapAffixDefinition30> pool = ArpgContent30.MapAffixes.Where(value => value.minimumTier <= tier).ToList();
            while (item.affixIds.Count < affixCount && pool.Count > 0)
            {
                int index = random.Next(pool.Count);
                item.affixIds.Add(pool[index].id);
                pool.RemoveAt(index);
            }
            return item;
        }

        public static bool CraftItem(ArpgProfile30 profile, ArpgItem30 item, ArpgCurrency30 currency, out string message)
        {
            message = string.Empty;
            if (profile == null || item == null)
            {
                message = "Select an item first.";
                return false;
            }
            if (item.corrupted && currency != ArpgCurrency30.DivineMeasure)
            {
                message = "Corrupted items cannot be modified further.";
                return false;
            }
            if (!profile.SpendCurrency(currency, 1))
            {
                message = "No " + CurrencyName(currency) + " remains.";
                return false;
            }

            ArpgItem30 snapshot = CloneItemState(item);
            System.Random random = new System.Random(StableSeed(item.instanceId, currency.ToString(), item.AffixCount + item.quality));
            bool success = true;
            switch (currency)
            {
                case ArpgCurrency30.RefinementShard:
                    if (item.quality >= 20) { success = false; message = "Quality is already 20%."; }
                    else { item.quality = Mathf.Min(20, item.quality + 5); message = "Quality increased to " + item.quality + "%."; }
                    break;
                case ArpgCurrency30.SparkOfAlteration:
                    if (item.rarity != ArpgItemRarity30.Magic) { success = false; message = "Alteration requires a Magic item."; }
                    else
                    {
                        ClearUnfracturedAffixes(item);
                        bool firstAdded = AddRandomAffix(item, random, null, true);
                        bool secondAdded = AddRandomAffix(item, random, null, true);
                        success = firstAdded || secondAdded || item.fractured;
                        message = success ? "Magic modifiers rerolled while fractured modifiers were preserved." : "No compatible Magic modifier was available.";
                    }
                    break;
                case ArpgCurrency30.RuneOfAugmentation:
                    if (item.rarity != ArpgItemRarity30.Magic || item.AffixCount >= 2) { success = false; message = "Augmentation requires a Magic item with an open modifier."; }
                    else { success = AddRandomAffix(item, random, null); message = success ? "A modifier was added." : "No compatible modifier was available."; }
                    break;
                case ArpgCurrency30.SigilOfElevation:
                    if (item.rarity == ArpgItemRarity30.Normal)
                    {
                        item.rarity = ArpgItemRarity30.Magic;
                        success = AddRandomAffix(item, random, null);
                        message = success ? "The item became Magic." : "No compatible Magic modifier was available.";
                    }
                    else if (item.rarity == ArpgItemRarity30.Magic)
                    {
                        item.rarity = ArpgItemRarity30.Rare;
                        while (item.AffixCount < 3) if (!AddRandomAffix(item, random, null)) break;
                        message = "The item became Rare.";
                    }
                    else { success = false; message = "Elevation requires a Normal or Magic item."; }
                    break;
                case ArpgCurrency30.ArcaneExalt:
                    if ((int)item.rarity < (int)ArpgItemRarity30.Rare || item.AffixCount >= 6) { success = false; message = "Exaltation requires a Rare item with an open modifier."; }
                    else { success = AddRandomAffix(item, random, null); message = success ? "A Rare modifier was added." : "No compatible modifier was available."; }
                    break;
                case ArpgCurrency30.NullOrb:
                    success = RemoveRandomAffix(item, random);
                    message = success ? "One random modifier was removed." : "The item has no removable modifier.";
                    break;
                case ArpgCurrency30.ReformationOrb:
                case ArpgCurrency30.DivineMeasure:
                    success = RerollValues(item, random);
                    message = success ? "Modifier values were rerolled." : "The item has no modifiers.";
                    break;
                case ArpgCurrency30.ChaosFragment:
                    if (!RemoveRandomAffix(item, random)) { success = false; message = "The item has no modifier to replace."; }
                    else
                    {
                        success = AddRandomAffix(item, random, null);
                        message = success ? "One modifier was replaced." : "No compatible replacement modifier was available.";
                    }
                    break;
                case ArpgCurrency30.ElementalEssence:
                    if (item.rarity == ArpgItemRarity30.Normal) item.rarity = ArpgItemRarity30.Magic;
                    if (item.rarity == ArpgItemRarity30.Magic && item.AffixCount >= 2) item.rarity = ArpgItemRarity30.Rare;
                    success = AddRandomAffix(item, random, random.Next(2) == 0 ? "potency" : "trigger", false);
                    message = success ? "A guaranteed spell-family modifier was added." : "No compatible Essence modifier was available.";
                    break;
                case ArpgCurrency30.OmenOfControl:
                    success = AddRandomAffix(item, random, item.prefixes.Count <= item.suffixes.Count ? "vitality" : "efficiency", false);
                    message = success ? "The Omen directed the next addition." : "No controlled modifier was available.";
                    break;
                case ArpgCurrency30.CorruptionCatalyst:
                    if (item.corrupted) { success = false; message = "The item is already corrupted."; }
                    else
                    {
                        item.corrupted = true;
                        int outcome = random.Next(4);
                        if (outcome == 0 && item.AffixCount < 6) AddRandomAffix(item, random, null);
                        else if (outcome == 1) item.quality = Mathf.Min(30, item.quality + 10);
                        else if (outcome == 2) RerollValues(item, random);
                        message = outcome == 3 ? "The corruption changed nothing, but the item is sealed." : "The corruption transformed the item.";
                    }
                    break;
                case ArpgCurrency30.FractureRune:
                    if (item.fractured || item.AffixCount == 0) { success = false; message = "The item cannot be fractured."; }
                    else
                    {
                        List<ArpgAffixRoll30> candidates = MutableAffixes(item).ToList();
                        if (candidates.Count == 0) { success = false; message = "The item has no unfractured modifier to protect."; }
                        else
                        {
                            ArpgAffixRoll30 selected = candidates[random.Next(candidates.Count)];
                            item.fractured = true;
                            item.fracturedAffixId = selected.affixId;
                            message = "The " + selected.affixId + " modifier is now fractured and protected.";
                        }
                    }
                    break;
                default:
                    success = false;
                    message = "That currency operation is not implemented.";
                    break;
            }

            if (!success)
            {
                RestoreItemState(item, snapshot);
                profile.AddCurrency(currency, 1);
            }
            RefreshName(item);
            return success;
        }

        public static bool CraftMap(ArpgProfile30 profile, ArpgMapItem30 map, ArpgCurrency30 currency, out string message)
        {
            message = string.Empty;
            if (profile == null || map == null) { message = "Select a map first."; return false; }
            if (map.corrupted && currency != ArpgCurrency30.DivineMeasure) { message = "Corrupted maps cannot be modified."; return false; }
            if (!profile.SpendCurrency(currency, 1)) { message = "No " + CurrencyName(currency) + " remains."; return false; }
            System.Random random = new System.Random(StableSeed(map.instanceId, currency.ToString(), map.affixIds.Count + map.quality));
            bool success = true;
            switch (currency)
            {
                case ArpgCurrency30.RefinementShard:
                    if (map.quality >= 20) { success = false; message = "Map quality is already 20%."; }
                    else { map.quality = Mathf.Min(20, map.quality + 5); message = "Map quality increased."; }
                    break;
                case ArpgCurrency30.SparkOfAlteration:
                    if (map.rarity != ArpgMapRarity30.Magic) { success = false; message = "Alteration requires a Magic map."; }
                    else { RerollMapAffixes(map, random, 2); message = "Map modifiers rerolled."; }
                    break;
                case ArpgCurrency30.SigilOfElevation:
                    if (map.rarity == ArpgMapRarity30.Normal) { map.rarity = ArpgMapRarity30.Magic; RerollMapAffixes(map, random, 2); message = "Map became Magic."; }
                    else if (map.rarity == ArpgMapRarity30.Magic) { map.rarity = ArpgMapRarity30.Rare; RerollMapAffixes(map, random, 4); message = "Map became Rare."; }
                    else { success = false; message = "Elevation requires a Normal or Magic map."; }
                    break;
                case ArpgCurrency30.ChaosFragment:
                    if (map.rarity != ArpgMapRarity30.Rare) { success = false; message = "Chaos requires a Rare map."; }
                    else { RerollMapAffixes(map, random, 4); message = "Rare map modifiers rerolled."; }
                    break;
                case ArpgCurrency30.CorruptionCatalyst:
                    map.corrupted = true;
                    if (map.rarity != ArpgMapRarity30.Rare) map.rarity = ArpgMapRarity30.Rare;
                    RerollMapAffixes(map, random, Mathf.Min(6, 4 + random.Next(3)));
                    message = "Map corrupted and sealed.";
                    break;
                case ArpgCurrency30.DivineMeasure:
                    map.seed = StableSeed(map.instanceId, "divine-map", map.seed);
                    message = "Map reward rolls were remeasured.";
                    break;
                default:
                    success = false;
                    message = "That currency does not modify maps.";
                    break;
            }
            if (!success) profile.AddCurrency(currency, 1);
            return success;
        }

        public static void AddItemStats(ArpgItem30 item, ArpgStatAccumulator30 accumulator)
        {
            if (item == null || accumulator == null) return;
            ArpgItemBaseDefinition30 itemBase = ArpgContent30.ItemBase(item.baseId);
            if (itemBase != null) foreach (ArpgStatModifier30 modifier in itemBase.implicitModifiers) accumulator.Add(modifier, 1f + item.quality / 100f);
            foreach (ArpgAffixRoll30 roll in (item.prefixes ?? new List<ArpgAffixRoll30>()).Concat(item.suffixes ?? new List<ArpgAffixRoll30>()))
            {
                if (roll == null) continue;
                ArpgAffixDefinition30 definition = ArpgContent30.Affix(roll.affixId);
                if (definition != null) accumulator.Add(new ArpgStatModifier30(definition.stat, roll.value), 1f);
            }
        }

        public static string Describe(ArpgItem30 item)
        {
            if (item == null) return string.Empty;
            List<string> lines = new List<string>();
            lines.Add(item.displayName + " · " + item.rarity + " · iLvl " + item.itemLevel + (item.corrupted ? " · CORRUPTED" : string.Empty));
            if (item.quality > 0) lines.Add("Quality: +" + item.quality + "%");
            ArpgItemBaseDefinition30 itemBase = ArpgContent30.ItemBase(item.baseId);
            if (itemBase != null) foreach (ArpgStatModifier30 modifier in itemBase.implicitModifiers) lines.Add("Implicit: " + DescribeModifier(modifier.stat, modifier.value * (1f + item.quality / 100f)));
            foreach (ArpgAffixRoll30 roll in MutableAffixes(item))
            {
                ArpgAffixDefinition30 definition = ArpgContent30.Affix(roll.affixId);
                if (definition != null) lines.Add(DescribeModifier(definition.stat, roll.value) + " [T" + roll.tier + "]" + (item.fractured && item.fracturedAffixId == roll.affixId ? " [FRACTURED]" : string.Empty));
            }
            return string.Join("\n", lines.ToArray());
        }

        public static string CurrencyName(ArpgCurrency30 currency)
        {
            switch (currency)
            {
                case ArpgCurrency30.RefinementShard: return "Refinement Shard";
                case ArpgCurrency30.SparkOfAlteration: return "Spark of Alteration";
                case ArpgCurrency30.RuneOfAugmentation: return "Rune of Augmentation";
                case ArpgCurrency30.SigilOfElevation: return "Sigil of Elevation";
                case ArpgCurrency30.ArcaneExalt: return "Arcane Exalt";
                case ArpgCurrency30.NullOrb: return "Null Orb";
                case ArpgCurrency30.ReformationOrb: return "Reformation Orb";
                case ArpgCurrency30.ChaosFragment: return "Chaos Fragment";
                case ArpgCurrency30.ElementalEssence: return "Elemental Essence";
                case ArpgCurrency30.OmenOfControl: return "Omen of Control";
                case ArpgCurrency30.CorruptionCatalyst: return "Corruption Catalyst";
                case ArpgCurrency30.DivineMeasure: return "Divine Measure";
                case ArpgCurrency30.FractureRune: return "Fracture Rune";
                default: return currency.ToString();
            }
        }

        private static bool AddRandomAffix(ArpgItem30 item, System.Random random, string requestedFamily, bool allowFamilyFallback = true)
        {
            if (item == null) return false;
            if (item.prefixes == null) item.prefixes = new List<ArpgAffixRoll30>();
            if (item.suffixes == null) item.suffixes = new List<ArpgAffixRoll30>();
            List<string> existingFamilies = item.prefixes.Concat(item.suffixes)
                .Select(value => ArpgContent30.Affix(value.affixId))
                .Where(value => value != null)
                .Select(value => value.family)
                .ToList();
            List<ArpgAffixDefinition30> pool = ArpgContent30.Affixes
                .Where(value => value.minimumItemLevel <= item.itemLevel)
                .Where(value => value.validSlots.Contains(item.slot))
                .Where(value => !existingFamilies.Contains(value.family))
                .Where(value => string.IsNullOrEmpty(requestedFamily) || value.family == requestedFamily)
                .Where(value => value.prefix ? item.prefixes.Count < item.PrefixLimit : item.suffixes.Count < item.SuffixLimit)
                .ToList();
            if (pool.Count == 0 && !string.IsNullOrEmpty(requestedFamily) && allowFamilyFallback) return AddRandomAffix(item, random, null, true);
            if (pool.Count == 0) return false;
            int totalWeight = pool.Sum(value => Mathf.Max(1, value.weight));
            int roll = random.Next(totalWeight);
            ArpgAffixDefinition30 selected = pool[0];
            foreach (ArpgAffixDefinition30 candidate in pool)
            {
                roll -= Mathf.Max(1, candidate.weight);
                if (roll < 0) { selected = candidate; break; }
            }
            ArpgAffixRoll30 affix = new ArpgAffixRoll30
            {
                affixId = selected.id,
                tier = Mathf.Clamp(1 + selected.minimumItemLevel / 22, 1, 4),
                value = Mathf.Lerp(selected.minimum, selected.maximum, (float)random.NextDouble())
            };
            if (selected.prefix) item.prefixes.Add(affix); else item.suffixes.Add(affix);
            return true;
        }

        private static bool RemoveRandomAffix(ArpgItem30 item, System.Random random)
        {
            List<ArpgAffixRoll30> all = MutableAffixes(item).ToList();
            if (all.Count == 0) return false;
            ArpgAffixRoll30 selected = all[random.Next(all.Count)];
            if (item.prefixes.Contains(selected)) item.prefixes.Remove(selected); else item.suffixes.Remove(selected);
            return true;
        }

        private static bool RerollValues(ArpgItem30 item, System.Random random)
        {
            bool any = false;
            foreach (ArpgAffixRoll30 roll in (item.prefixes ?? new List<ArpgAffixRoll30>()).Concat(item.suffixes ?? new List<ArpgAffixRoll30>()))
            {
                if (roll == null) continue;
                ArpgAffixDefinition30 definition = ArpgContent30.Affix(roll.affixId);
                if (definition == null) continue;
                roll.value = Mathf.Lerp(definition.minimum, definition.maximum, (float)random.NextDouble());
                any = true;
            }
            return any;
        }

        private static void RerollMapAffixes(ArpgMapItem30 map, System.Random random, int count)
        {
            map.affixIds.Clear();
            List<ArpgMapAffixDefinition30> pool = ArpgContent30.MapAffixes.Where(value => value.minimumTier <= map.tier).ToList();
            while (map.affixIds.Count < count && pool.Count > 0)
            {
                int index = random.Next(pool.Count);
                map.affixIds.Add(pool[index].id);
                pool.RemoveAt(index);
            }
        }

        private static void RefreshName(ArpgItem30 item)
        {
            ArpgItemBaseDefinition30 itemBase = ArpgContent30.ItemBase(item.baseId);
            string baseName = itemBase == null ? "Unknown Item" : itemBase.displayName;
            if (item.rarity == ArpgItemRarity30.Normal) item.displayName = baseName;
            else if (item.rarity == ArpgItemRarity30.Magic) item.displayName = "Runed " + baseName;
            else if (item.rarity == ArpgItemRarity30.Exceptional) item.displayName = "Exalted " + baseName;
            else item.displayName = "Astral " + baseName;
        }

        private static string DescribeModifier(ArpgStat30 stat, float value)
        {
            switch (stat)
            {
                case ArpgStat30.MaximumHealth: return "+" + Mathf.RoundToInt(value) + " maximum Health";
                case ArpgStat30.MaximumMana: return "+" + Mathf.RoundToInt(value) + " maximum Mana";
                case ArpgStat30.MoveSpeed: return "+" + value.ToString("0.00") + " movement speed";
                case ArpgStat30.TriggerEnergy: return "+" + Mathf.RoundToInt(value) + " Trigger Energy";
                case ArpgStat30.Attunement: return "+" + Mathf.RoundToInt(value) + " Attunement";
                default: return "+" + Mathf.RoundToInt(value * 100f) + "% " + stat;
            }
        }

        private static ArpgItem30 CloneItemState(ArpgItem30 item)
        {
            return new ArpgItem30
            {
                instanceId = item.instanceId,
                baseId = item.baseId,
                displayName = item.displayName,
                slot = item.slot,
                rarity = item.rarity,
                itemLevel = item.itemLevel,
                quality = item.quality,
                corrupted = item.corrupted,
                fractured = item.fractured,
                fracturedAffixId = item.fracturedAffixId,
                prefixes = CloneAffixes(item.prefixes),
                suffixes = CloneAffixes(item.suffixes)
            };
        }

        private static void RestoreItemState(ArpgItem30 target, ArpgItem30 snapshot)
        {
            target.instanceId = snapshot.instanceId;
            target.baseId = snapshot.baseId;
            target.displayName = snapshot.displayName;
            target.slot = snapshot.slot;
            target.rarity = snapshot.rarity;
            target.itemLevel = snapshot.itemLevel;
            target.quality = snapshot.quality;
            target.corrupted = snapshot.corrupted;
            target.fractured = snapshot.fractured;
            target.fracturedAffixId = snapshot.fracturedAffixId;
            target.prefixes = CloneAffixes(snapshot.prefixes);
            target.suffixes = CloneAffixes(snapshot.suffixes);
        }

        private static List<ArpgAffixRoll30> CloneAffixes(IEnumerable<ArpgAffixRoll30> source)
        {
            if (source == null) return new List<ArpgAffixRoll30>();
            return source.Where(value => value != null).Select(value => new ArpgAffixRoll30
            {
                affixId = value.affixId,
                tier = value.tier,
                value = value.value
            }).ToList();
        }

        private static IEnumerable<ArpgAffixRoll30> MutableAffixes(ArpgItem30 item)
        {
            if (item == null) yield break;
            foreach (ArpgAffixRoll30 roll in item.prefixes ?? new List<ArpgAffixRoll30>())
                if (roll != null && (!item.fractured || item.fracturedAffixId != roll.affixId)) yield return roll;
            foreach (ArpgAffixRoll30 roll in item.suffixes ?? new List<ArpgAffixRoll30>())
                if (roll != null && (!item.fractured || item.fracturedAffixId != roll.affixId)) yield return roll;
        }

        private static void ClearUnfracturedAffixes(ArpgItem30 item)
        {
            if (item == null) return;
            item.prefixes.RemoveAll(value => value == null || !item.fractured || value.affixId != item.fracturedAffixId);
            item.suffixes.RemoveAll(value => value == null || !item.fractured || value.affixId != item.fracturedAffixId);
        }

        private static int StableSeed(string first, string second, int third)
        {
            return ArpgDeterminism30.Combine(first, second, third);
        }
    }
}
