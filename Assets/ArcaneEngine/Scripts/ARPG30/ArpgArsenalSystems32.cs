using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public static class ArpgArsenalItemFactory32
    {
        public static bool SuppressOverride { get; private set; }

        public static ArpgItem30 Generate(int itemLevel, ArpgClass30 affinity, int seed, float rarityBonus)
        {
            ArpgArsenalContent32.Ensure();
            System.Random random = new System.Random(seed);
            double uniqueChance = 0.012 + Mathf.Clamp(rarityBonus, 0f, 0.6f) * 0.06;
            double exceptionalChance = 0.035 + Mathf.Clamp(rarityBonus, 0f, 0.6f) * 0.10;
            double roll = random.NextDouble();

            if (roll < uniqueChance) return GenerateUnique(itemLevel, seed);

            SuppressOverride = true;
            ArpgItem30 item;
            try { item = ArpgItems30.GenerateItem(itemLevel, affinity, seed, rarityBonus); }
            finally { SuppressOverride = false; }

            bool exceptional = roll < uniqueChance + exceptionalChance && (int)item.rarity >= (int)ArpgItemRarity30.Rare;
            if (exceptional)
            {
                item.rarity = ArpgItemRarity30.Exceptional;
                ArpgItemBaseDefinition30 b = ArpgContent30.ItemBase(item.baseId);
                item.displayName = "Exalted " + (b == null ? item.displayName : b.displayName);
                ArpgAffixRoll30 first = (item.prefixes ?? new List<ArpgAffixRoll30>()).FirstOrDefault();
                if (first != null)
                {
                    ArpgAffixDefinition30 def = ArpgContent30.Affix(first.affixId);
                    first.tier = 6;
                    if (def != null) first.value = def.maximum * 1.18f;
                }
            }

            ArpgArsenalStore32.RegisterGeneratedItem(item, string.Empty, exceptional);
            return item;
        }

        private static ArpgItem30 GenerateUnique(int itemLevel, int seed)
        {
            System.Random random = new System.Random(seed);
            List<ArpgUniqueDefinition32> pool = ArpgArsenalContent32.UniqueItems
                .Where(x => { ArpgItemBaseDefinition30 b = ArpgContent30.ItemBase(x.baseId); return b == null || b.requiredLevel <= itemLevel; }).ToList();
            if (pool.Count == 0) pool = ArpgArsenalContent32.UniqueItems.ToList();
            ArpgUniqueDefinition32 unique = pool[random.Next(pool.Count)];
            ArpgItemBaseDefinition30 selected = ArpgContent30.ItemBase(unique.baseId);
            ArpgItem30 item = new ArpgItem30
            {
                instanceId = Guid.NewGuid().ToString("N"),
                baseId = unique.baseId,
                displayName = unique.displayName,
                slot = selected == null ? ArpgItemSlot30.Relic : selected.slot,
                rarity = ArpgItemRarity30.Unique,
                itemLevel = Mathf.Max(1, itemLevel),
                acquiredUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            ArpgArsenalStore32.RegisterGeneratedItem(item, unique.id, false);
            return item;
        }

        public static ArpgItem30 GenerateBossUnique(string bossId,int itemLevel,int seed)
        {
            List<ArpgUniqueDefinition32> pool=ArpgArsenalContent32.UniqueItems.Where(value=>value!=null&&value.bossId==bossId).ToList();
            if(pool.Count==0)return GenerateUnique(itemLevel,seed);
            ArpgUniqueDefinition32 unique=pool[ArpgDeterminism30.Index(seed,pool.Count)];
            ArpgItemBaseDefinition30 selected=ArpgContent30.ItemBase(unique.baseId);
            ArpgItem30 item=new ArpgItem30{instanceId=Guid.NewGuid().ToString("N"),baseId=unique.baseId,displayName=unique.displayName,slot=selected==null?ArpgItemSlot30.Relic:selected.slot,rarity=ArpgItemRarity30.Unique,itemLevel=Mathf.Max(1,itemLevel),acquiredUnix=DateTimeOffset.UtcNow.ToUnixTimeSeconds()};
            ArpgArsenalStore32.RegisterGeneratedItem(item,unique.id,false);return item;
        }
    }

    public static class ArpgArsenalStats32
    {
        public static void AddExtendedItemStats(ArpgItem30 item, ArpgStatAccumulator30 accumulator)
        {
            if (item == null || accumulator == null) return;
            ArpgItemState32 state = ArpgArsenalStore32.ItemState(item.instanceId);
            if (state == null) return;
            ArpgUniqueDefinition32 unique = ArpgArsenalContent32.Unique(state.uniqueId);
            if (unique != null)
            {
                foreach (ArpgStatModifier30 modifier in unique.modifiers) accumulator.Add(modifier, 1f);
                ApplyUniqueRule(unique.id,accumulator);
            }
            ArpgCorruptedImplicitDefinition32 corruption = ArpgArsenalContent32.CorruptedImplicit(state.corruptedImplicitId);
            if (corruption != null) { accumulator.Add(corruption.modifier, 1f); accumulator.Add(corruption.penalty, 1f); }
        }

        private static void ApplyUniqueRule(string id,ArpgStatAccumulator30 accumulator)
        {
            int index=-1;if(!string.IsNullOrEmpty(id)&&id.StartsWith("ae32.unique.",StringComparison.Ordinal))int.TryParse(id.Substring(id.Length-2),out index);
            switch(index)
            {
                case 0: accumulator.Add(ArpgStat30.ProjectileSpeed,0.12f);accumulator.Add(ArpgStat30.SpellPower,-0.08f);break;
                case 1: accumulator.Add(ArpgStat30.LightningPower,0.18f);accumulator.Add(ArpgStat30.CooldownRecovery,-0.05f);break;
                case 2: accumulator.Add(ArpgStat30.AilmentBuildup,0.22f);accumulator.Add(ArpgStat30.ColdPower,-0.30f);break;
                case 3: accumulator.Add(ArpgStat30.RuneCapacity,1f);accumulator.Add(ArpgStat30.ManaEfficiency,-0.12f);break;
                case 4: accumulator.Add(ArpgStat30.Duration,0.20f);accumulator.Add(ArpgStat30.MoveSpeed,-0.05f);break;
                case 5: accumulator.Add(ArpgStat30.TriggerEnergy,18f);accumulator.Add(ArpgStat30.SpellPower,-0.10f);break;
                case 6: accumulator.Add(ArpgStat30.PhysicalPower,0.20f);accumulator.Add(ArpgStat30.Evasion,-25f);break;
                case 7: accumulator.Add(ArpgStat30.VoidPower,0.20f);accumulator.Add(ArpgStat30.ArcaneWard,-20f);break;
                case 8: accumulator.Add(ArpgStat30.BloodPower,0.16f);accumulator.Add(ArpgStat30.ToxicPower,-0.10f);break;
                case 9: accumulator.Add(ArpgStat30.Duration,0.18f);accumulator.Add(ArpgStat30.ProjectileSpeed,-0.08f);break;
                case 10: accumulator.Add(ArpgStat30.ReactionPower,0.18f);accumulator.Add(ArpgStat30.CriticalChance,-0.06f);break;
                case 11: accumulator.Add(ArpgStat30.RuneCapacity,1f);accumulator.Add(ArpgStat30.SpellPower,-0.08f);break;
                case 12: accumulator.Add(ArpgStat30.BloodPower,0.20f);accumulator.Add(ArpgStat30.MaximumHealth,-15f);break;
                case 13: accumulator.Add(ArpgStat30.Duration,0.18f);accumulator.Add(ArpgStat30.ToxicPower,-0.08f);break;
                case 14: accumulator.Add(ArpgStat30.MoveSpeed,0.10f);accumulator.Add(ArpgStat30.ArcaneWard,-12f);break;
                case 15: accumulator.Add(ArpgStat30.ColdPower,0.18f);accumulator.Add(ArpgStat30.FirePower,-0.08f);break;
                case 16: accumulator.Add(ArpgStat30.FirePower,0.18f);accumulator.Add(ArpgStat30.AilmentBuildup,-0.06f);break;
                case 17: accumulator.Add(ArpgStat30.CooldownRecovery,0.16f);accumulator.Add(ArpgStat30.MaximumHealth,-12f);break;
                case 18: accumulator.Add(ArpgStat30.Armour,45f);accumulator.Add(ArpgStat30.BarrierStrength,-0.06f);break;
                case 19: accumulator.Add(ArpgStat30.Attunement,2f);accumulator.Add(ArpgStat30.GoldFind,-0.08f);break;
                case 20: accumulator.Add(ArpgStat30.VoidPower,0.18f);accumulator.Add(ArpgStat30.AreaOfEffect,-0.10f);break;
                case 21: accumulator.Add(ArpgStat30.MapQuantity,0.18f);accumulator.Add(ArpgStat30.MapRarity,-0.08f);break;
                case 22: accumulator.Add(ArpgStat30.ReactionPower,0.16f);accumulator.Add(ArpgStat30.SpellPower,-0.06f);break;
                case 23: accumulator.Add(ArpgStat30.RuneCapacity,1f);accumulator.Add(ArpgStat30.ManaEfficiency,-0.06f);break;
            }
        }

        public static string DescribeExtended(ArpgItem30 item)
        {
            if (item == null) return string.Empty;
            ArpgItemState32 state = ArpgArsenalStore32.ItemState(item.instanceId);
            if (state == null) return string.Empty;
            List<string> lines = new List<string>();
            ArpgUniqueDefinition32 unique = ArpgArsenalContent32.Unique(state.uniqueId);
            if (unique != null)
            {
                lines.Add(unique.description);
                lines.Add("Constraint: " + unique.constraint);
                if (!string.IsNullOrEmpty(unique.bossId)) lines.Add("Exclusive source: " + unique.bossId);
            }
            if (state.exceptional) lines.Add("[EXCEPTIONAL] Elevated and sealed modifier.");
            ArpgCorruptedImplicitDefinition32 corruption = ArpgArsenalContent32.CorruptedImplicit(state.corruptedImplicitId);
            if (corruption != null) lines.Add("[CORRUPTED IMPLICIT] " + corruption.displayName + " — " + corruption.description);
            if (state.favorite) lines.Add("[FAVORITE]");
            ArpgFootprintDefinition32 footprint = ArpgArsenalContent32.Footprint(item);
            lines.Add("Footprint: " + footprint.width + "×" + footprint.height);
            return string.Join("\n", lines.ToArray());
        }
    }

    public static class ArpgCrafting32
    {
        public static bool Apply(ArpgProfile30 profile, ArpgItem30 item, ArpgCurrency32 currency, out string message)
        {
            message = string.Empty;
            if (profile == null || item == null) { message = "Select an item."; return false; }
            if (item.locked) { message = "Unlock the item before crafting."; return false; }

            ArpgItemState32 state = ArpgArsenalStore32.EnsureItemState(profile, item);
            if (item.corrupted && currency != ArpgCurrency32.TemperingPrism) { message = "Corrupted items cannot use ordinary crafting."; return false; }
            if (!string.IsNullOrEmpty(state.uniqueId)) { message = "Unique items cannot use ordinary modifier crafting."; return false; }
            if (!ArpgArsenalStore32.SpendCurrency(currency, 1))
            {
                ArpgCurrencyDefinition32 def = ArpgArsenalContent32.Currency(currency);
                message = "No " + (def == null ? currency.ToString() : def.displayName) + " remains.";
                return false;
            }

            string itemJson = JsonUtility.ToJson(item);
            string stateJson = JsonUtility.ToJson(state);
            System.Random random = new System.Random(ArpgDeterminism30.StableHash(item.instanceId + currency + state.craftHistory.Count));
            bool success = false;
            try
            {
                switch (currency)
                {
                    case ArpgCurrency32.FluxShard:
                        success = item.rarity == ArpgItemRarity30.Magic;
                        if (success) { Preserve(item, state, false, false); AddAffix(item, random, null); AddAffix(item, random, null); message = "Magic modifiers rerolled."; }
                        else message = "Flux Shards require a Magic item.";
                        break;
                    case ArpgCurrency32.BindingSeal:
                        success = item.rarity == ArpgItemRarity30.Magic && item.AffixCount < 2 && AddAffix(item, random, null);
                        message = success ? "A Magic modifier was added." : "Binding Seals require an open Magic modifier.";
                        break;
                    case ArpgCurrency32.SovereignEmber:
                        success = item.rarity == ArpgItemRarity30.Magic;
                        if (success) { item.rarity = ArpgItemRarity30.Rare; while (item.AffixCount < 4 && AddAffix(item, random, null)) { } message = "The item became Rare."; }
                        else message = "Sovereign Embers require a Magic item.";
                        break;
                    case ArpgCurrency32.AstralNeedle:
                        success = item.rarity == ArpgItemRarity30.Rare && item.AffixCount < 6 && AddAffix(item, random, null);
                        message = success ? "A Rare modifier was added." : "Astral Needles require an open Rare slot.";
                        break;
                    case ArpgCurrency32.TemperingPrism:
                        success = RerollValues(item, state, random); message = success ? "Modifier values rerolled." : "No mutable modifiers.";
                        break;
                    case ArpgCurrency32.ArtisansMeasure:
                        success = item.quality < 20; if (success) { item.quality = Mathf.Min(20, item.quality + 5); message = "Quality increased to " + item.quality + "%."; } else message = "Quality is already maximum.";
                        break;
                    case ArpgCurrency32.SeveranceChisel:
                        success = RemoveAffix(item, state, random); message = success ? "One modifier removed." : "No removable modifier.";
                        break;
                    case ArpgCurrency32.TransferenceSigil:
                        success = RemoveAffix(item, state, random) && AddAffix(item, random, null); message = success ? "One modifier replaced." : "No compatible replacement.";
                        break;
                    case ArpgCurrency32.ResonantBrand:
                        string[] families = { "ae32.flame", "ae32.frost", "ae32.storm", "ae32.blood", "ae32.toxic", "ae32.void" };
                        success = AddAffix(item, random, families[random.Next(families.Length)]); message = success ? "An elemental family was forced." : "No compatible elemental modifier.";
                        break;
                    case ArpgCurrency32.AscendantSpark:
                        success = RaiseTier(item, state, random); message = success ? "One modifier tier improved." : "No modifier can be raised.";
                        break;
                    case ArpgCurrency32.PrefixWard:
                        state.prefixWard = true; success = true; message = "Prefixes protected for the next reforge."; break;
                    case ArpgCurrency32.SuffixWard:
                        state.suffixWard = true; success = true; message = "Suffixes protected for the next reforge."; break;
                    case ArpgCurrency32.MemoryLoom:
                        success = MemoryReforge(item, state, random); message = success ? "Reforged while preserving one modifier." : "No modifier can be preserved."; break;
                    case ArpgCurrency32.FractureRune:
                        success = Fracture(item, state, random); message = success ? "One modifier permanently fractured." : "The item cannot be fractured."; break;
                    case ArpgCurrency32.OmenOfRevision:
                        state.omen = "PreferHigherTier"; success = true; message = "The next supported craft prefers a higher tier."; break;
                }
            }
            catch (Exception exception) { message = "Crafting failed safely: " + exception.Message; success = false; }

            if (!success)
            {
                JsonUtility.FromJsonOverwrite(itemJson, item);
                JsonUtility.FromJsonOverwrite(stateJson, state);
                ArpgArsenalStore32.AddCurrency(currency, 1);
                return false;
            }

            RefreshName(item, state);
            state.craftHistory.Add(new ArpgCraftRecord32 { operation = currency.ToString(), detail = message, unix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
            if (state.craftHistory.Count > 40) state.craftHistory.RemoveAt(0);
            if (currency != ArpgCurrency32.OmenOfRevision) state.omen = string.Empty;
            ArpgArsenalStore32.Save(); ArpgProfileStore30.Save(profile); return true;
        }

        public static bool Corrupt(ArpgProfile30 profile, ArpgItem30 item, out string message)
        {
            message = string.Empty;
            if (profile == null || item == null) { message = "Select an item."; return false; }
            if (item.locked) { message = "Unlock the item first."; return false; }
            if (item.corrupted) { message = "The item is already corrupted."; return false; }
            ArpgItemState32 state = ArpgArsenalStore32.EnsureItemState(profile, item);
            System.Random random = new System.Random(ArpgDeterminism30.StableHash(item.instanceId + ".corrupt"));
            item.corrupted = true;
            ArpgCorruptedImplicitDefinition32 effect = ArpgArsenalContent32.CorruptedImplicitDefinitions[random.Next(ArpgArsenalContent32.CorruptedImplicitDefinitions.Count)];
            state.corruptedImplicitId = effect.id;
            int outcome = random.Next(4);
            if (outcome == 0) item.quality = Mathf.Min(30, item.quality + 10);
            else if (outcome == 1) RaiseTier(item, state, random);
            else if (outcome == 2 && item.AffixCount < 6) AddAffix(item, random, null);
            state.craftHistory.Add(new ArpgCraftRecord32 { operation = "Corruption", detail = effect.displayName, unix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
            ArpgArsenalStore32.Save(); ArpgProfileStore30.Save(profile);
            message = "Corruption applied: " + effect.displayName + "."; return true;
        }

        private static bool AddAffix(ArpgItem30 item, System.Random random, string family)
        {
            if (item.prefixes == null) item.prefixes = new List<ArpgAffixRoll30>();
            if (item.suffixes == null) item.suffixes = new List<ArpgAffixRoll30>();
            List<string> existing = item.prefixes.Concat(item.suffixes).Select(x => ArpgContent30.Affix(x.affixId)).Where(x => x != null).Select(x => x.family).ToList();
            List<ArpgAffixDefinition30> pool = ArpgContent30.Affixes
                .Where(x => x.minimumItemLevel <= item.itemLevel && x.validSlots.Contains(item.slot) && !existing.Contains(x.family))
                .Where(x => string.IsNullOrEmpty(family) || x.family == family)
                .Where(x => x.prefix ? item.prefixes.Count < item.PrefixLimit : item.suffixes.Count < item.SuffixLimit).ToList();
            if (pool.Count == 0) return false;
            ArpgAffixDefinition30 selected = pool[random.Next(pool.Count)];
            ArpgAffixRoll30 roll = new ArpgAffixRoll30 { affixId = selected.id, tier = Mathf.Clamp(1 + selected.minimumItemLevel / 8, 1, 6), value = Mathf.Lerp(selected.minimum, selected.maximum, (float)random.NextDouble()) };
            if (selected.prefix) item.prefixes.Add(roll); else item.suffixes.Add(roll);
            return true;
        }

        private static bool RerollValues(ArpgItem30 item, ArpgItemState32 state, System.Random random)
        {
            bool changed = false;
            foreach (ArpgAffixRoll30 roll in item.prefixes.Concat(item.suffixes))
            {
                if (roll == null || Protected(item, state, roll)) continue;
                ArpgAffixDefinition30 def = ArpgContent30.Affix(roll.affixId);
                if (def == null) continue;
                roll.value = Mathf.Lerp(def.minimum, def.maximum, (float)random.NextDouble()); changed = true;
            }
            return changed;
        }

        private static bool RemoveAffix(ArpgItem30 item, ArpgItemState32 state, System.Random random)
        {
            List<ArpgAffixRoll30> list = item.prefixes.Concat(item.suffixes).Where(x => x != null && !Protected(item, state, x)).ToList();
            if (list.Count == 0) return false;
            ArpgAffixRoll30 selected = list[random.Next(list.Count)];
            if (item.prefixes.Contains(selected)) item.prefixes.Remove(selected); else item.suffixes.Remove(selected);
            return true;
        }

        private static bool RaiseTier(ArpgItem30 item, ArpgItemState32 state, System.Random random)
        {
            List<ArpgAffixRoll30> list = item.prefixes.Concat(item.suffixes).Where(x => x != null && x.tier < 6 && !Protected(item, state, x)).ToList();
            if (list.Count == 0) return false;
            ArpgAffixRoll30 selected = list[random.Next(list.Count)]; selected.tier++;
            ArpgAffixDefinition30 def = ArpgContent30.Affix(selected.affixId); if (def != null) selected.value = Mathf.Min(def.maximum * 1.08f, selected.value * 1.14f);
            return true;
        }

        private static bool MemoryReforge(ArpgItem30 item, ArpgItemState32 state, System.Random random)
        {
            ArpgAffixRoll30 memory = item.prefixes.Concat(item.suffixes).FirstOrDefault(x => x != null && !Protected(item, state, x));
            if (memory == null) return false;
            string json = JsonUtility.ToJson(memory); bool prefix = item.prefixes.Contains(memory);
            Preserve(item, state, state.prefixWard, state.suffixWard);
            ArpgAffixRoll30 restored = JsonUtility.FromJson<ArpgAffixRoll30>(json);
            if (prefix && !item.prefixes.Any(x => x.affixId == restored.affixId)) item.prefixes.Add(restored);
            if (!prefix && !item.suffixes.Any(x => x.affixId == restored.affixId)) item.suffixes.Add(restored);
            while (item.AffixCount < (item.rarity == ArpgItemRarity30.Magic ? 2 : 4) && AddAffix(item, random, null)) { }
            state.prefixWard = false; state.suffixWard = false; return true;
        }

        private static bool Fracture(ArpgItem30 item, ArpgItemState32 state, System.Random random)
        {
            if (item.fractured || item.AffixCount == 0 || (int)item.rarity < (int)ArpgItemRarity30.Rare) return false;
            List<ArpgAffixRoll30> list = item.prefixes.Concat(item.suffixes).Where(x => x != null && !state.sealedAffixIds.Contains(x.affixId)).ToList();
            if (list.Count == 0) return false;
            ArpgAffixRoll30 selected = list[random.Next(list.Count)]; item.fractured = true; item.fracturedAffixId = selected.affixId; return true;
        }

        private static void Preserve(ArpgItem30 item, ArpgItemState32 state, bool prefixes, bool suffixes)
        {
            item.prefixes.RemoveAll(x => x != null && !Protected(item, state, x) && !prefixes);
            item.suffixes.RemoveAll(x => x != null && !Protected(item, state, x) && !suffixes);
        }
        private static bool Protected(ArpgItem30 item, ArpgItemState32 state, ArpgAffixRoll30 roll)
        { return roll != null && ((item.fractured && item.fracturedAffixId == roll.affixId) || state.sealedAffixIds.Contains(roll.affixId)); }

        private static void RefreshName(ArpgItem30 item, ArpgItemState32 state)
        {
            ArpgItemBaseDefinition30 b = ArpgContent30.ItemBase(item.baseId); string n = b == null ? "Unknown Item" : b.displayName;
            ArpgUniqueDefinition32 u = ArpgArsenalContent32.Unique(state.uniqueId);
            if (u != null) item.displayName = u.displayName;
            else if (state.exceptional) item.displayName = "Exalted " + n;
            else if (item.rarity == ArpgItemRarity30.Normal) item.displayName = n;
            else if (item.rarity == ArpgItemRarity30.Magic) item.displayName = "Runed " + n;
            else item.displayName = "Astral " + n;
        }
    }

    public static class ArpgMapCrafting32
    {
        public static bool Apply(ArpgProfile30 profile, ArpgMapItem30 map, ArpgCurrency32 currency, out string message)
        {
            message = string.Empty;
            if (profile == null || map == null) { message = "Select a map."; return false; }
            if (map.corrupted) { message = "Corrupted maps are sealed."; return false; }
            if (!ArpgArsenalStore32.SpendCurrency(currency, 1)) { message = "No matching currency remains."; return false; }
            System.Random random = new System.Random(ArpgDeterminism30.StableHash(map.instanceId + currency));
            bool success = true;
            if (currency == ArpgCurrency32.FluxShard && map.rarity == ArpgMapRarity30.Magic) Reroll(map, random, 2);
            else if (currency == ArpgCurrency32.SovereignEmber && map.rarity != ArpgMapRarity30.Rare)
            { map.rarity = map.rarity == ArpgMapRarity30.Normal ? ArpgMapRarity30.Magic : ArpgMapRarity30.Rare; Reroll(map, random, map.rarity == ArpgMapRarity30.Magic ? 2 : 4); }
            else if (currency == ArpgCurrency32.ArtisansMeasure && map.quality < 20) map.quality = Mathf.Min(20, map.quality + 5);
            else if (currency == ArpgCurrency32.TransferenceSigil && map.rarity == ArpgMapRarity30.Rare) Reroll(map, random, 4);
            else success = false;
            if (!success) { ArpgArsenalStore32.AddCurrency(currency, 1); message = "That currency is invalid for this map."; return false; }
            ArpgArsenalStore32.Save();ArpgProfileStore30.Save(profile); message = "Map crafting completed."; return true;
        }

        public static bool Corrupt(ArpgProfile30 profile, ArpgMapItem30 map, out string message)
        {
            message = string.Empty;if(profile==null||map==null){message="Select a map.";return false;}if(map.corrupted){message="The map is already corrupted.";return false;}
            map.corrupted=true;map.rarity=ArpgMapRarity30.Rare;Reroll(map,new System.Random(ArpgDeterminism30.StableHash(map.instanceId+".corrupt")),6);
            ArpgArsenalStore32.Save();ArpgProfileStore30.Save(profile);message="The map is corrupted and sealed.";return true;
        }
        public static float RewardMultiplier(ArpgMapItem30 map)
        {
            if(map==null)return 1f;float v=1f+map.quality*0.005f;
            foreach(string id in map.affixIds){ArpgMapAffixDefinition30 d=ArpgContent30.MapAffix(id);if(d!=null)v*=Mathf.Max(1f,d.rewardMultiplier);}return v;
        }
        private static void Reroll(ArpgMapItem30 map,System.Random random,int count)
        {
            map.affixIds.Clear();List<ArpgMapAffixDefinition30> pool=ArpgContent30.MapAffixes.Where(x=>x.minimumTier<=map.tier).ToList();
            while(map.affixIds.Count<count&&pool.Count>0){int i=random.Next(pool.Count);map.affixIds.Add(pool[i].id);pool.RemoveAt(i);}
        }
    }

    public static class ArpgVendor32
    {
        public static void Ensure(ArpgProfile30 profile){if(profile==null)return;ArpgVendorState32 v=ArpgArsenalStore32.Current.vendor;if(v.offers.Count==0||profile.totalMapsCompleted>v.mapsCompletedAtRefresh)Refresh(profile);}
        public static void Refresh(ArpgProfile30 profile)
        {
            ArpgVendorState32 v=ArpgArsenalStore32.Current.vendor;List<ArpgVendorOffer32> reserved=v.offers.Where(x=>x!=null&&x.reserved).Take(1).ToList();
            v.offers.Clear();v.offers.AddRange(reserved);int level=Mathf.Max(1,profile.highestCompletedTier*2+2);
            for(int i=0;i<8;i++)
            {
                int seed=ArpgDeterminism30.Combine(profile.saveRevision,profile.totalMapsCompleted,i+v.refreshRevision*31);
                ArpgItem30 item=ArpgArsenalItemFactory32.Generate(level,profile.characterClass,seed,i==7?0.22f:0.02f);
                v.offers.Add(new ArpgVendorOffer32{offerId=Guid.NewGuid().ToString("N"),item=item,goldCost=30+item.itemLevel*12+item.AffixCount*28+(int)item.rarity*45});
            }
            v.refreshRevision++;v.mapsCompletedAtRefresh=profile.totalMapsCompleted;ArpgArsenalStore32.Save();
        }

        public static bool Buy(ArpgProfile30 profile,ArpgVendorOffer32 offer,out string message)
        {
            message=string.Empty;if(profile==null||offer==null||offer.item==null){message="Invalid offer.";return false;}
            if(ArpgArsenalStore32.Current.gold<offer.goldCost){message="Not enough Gold.";return false;}
            ArpgArsenalStore32.Current.gold-=offer.goldCost;if(!profile.AddItemToInventory(offer.item)){ArpgArsenalStore32.Current.gold+=offer.goldCost;message="The profile rejected the item.";return false;}
            string result;if(!ArpgArsenalStore32.AutoPlace(profile,offer.item,ArpgArsenalStore32.InventoryContainer(profile.characterId),out result))
            {profile.items.Remove(offer.item);ArpgArsenalStore32.Current.gold+=offer.goldCost;message=result;return false;}
            ArpgArsenalStore32.Current.vendor.offers.Remove(offer);ArpgArsenalStore32.Save();ArpgProfileStore30.Save(profile);message="Purchased "+offer.item.displayName+".";return true;
        }
        public static int SellValue(ArpgItem30 item){return item==null?0:8+item.itemLevel*3+item.AffixCount*9+item.quality+(int)item.rarity*15;}
        public static bool Sell(ArpgProfile30 p,ArpgItem30 item,out string message)
        {
            message=string.Empty;ArpgItemState32 s=item==null?null:ArpgArsenalStore32.ItemState(item.instanceId);
            if(item==null){message="Select an item.";return false;}if(item.locked||(s!=null&&s.favorite)){message="Unlock and unfavorite it first.";return false;}
            int value=SellValue(item);ArpgArsenalStore32.Current.gold+=value;ArpgArsenalStore32.RemoveItem(p,item);message="Sold for "+value+" Gold.";return true;
        }
        public static bool Salvage(ArpgProfile30 p,ArpgItem30 item,out string message)
        {
            message=string.Empty;ArpgItemState32 s=item==null?null:ArpgArsenalStore32.ItemState(item.instanceId);
            if(item==null){message="Select an item.";return false;}if(item.locked||(s!=null&&s.favorite)){message="Unlock and unfavorite it first.";return false;}
            int amount=1+item.AffixCount+item.quality/5+(int)item.rarity;ArpgCurrency32 reward=(int)item.rarity>=(int)ArpgItemRarity30.Exceptional?ArpgCurrency32.AscendantSpark:(int)item.rarity>=(int)ArpgItemRarity30.Rare?ArpgCurrency32.BindingSeal:ArpgCurrency32.FluxShard;
            ArpgArsenalStore32.AddCurrency(reward,amount);ArpgArsenalStore32.RemoveItem(p,item);message="Salvaged into "+amount+" "+ArpgArsenalContent32.Currency(reward).displayName+".";return true;
        }
        public static bool Gamble(ArpgProfile30 p,ArpgItemSlot30 slot,out string message)
        {
            int cost=120+Mathf.Max(0,p.highestCompletedTier)*30;message=string.Empty;if(ArpgArsenalStore32.Current.gold<cost){message="Not enough Gold.";return false;}
            ArpgArsenalStore32.Current.gold-=cost;int seed=ArpgDeterminism30.Combine(p.saveRevision,p.totalMapsCompleted,(int)slot*101+ArpgArsenalStore32.Current.revision);
            ArpgItem30 item=ArpgArsenalItemFactory32.Generate(Mathf.Max(1,p.level),p.characterClass,seed,0.08f);item.slot=slot;
            if(!p.AddItemToInventory(item)){ArpgArsenalStore32.Current.gold+=cost;message="The profile rejected the item.";return false;}
            string result;if(!ArpgArsenalStore32.AutoPlace(p,item,ArpgArsenalStore32.InventoryContainer(p.characterId),out result)){p.items.Remove(item);ArpgArsenalStore32.Current.gold+=cost;message=result;return false;}
            ArpgProfileStore30.Save(p);message="Gamble revealed "+item.displayName+".";return true;
        }
    }

    public static class ArpgArsenalRuntime32
    {
        public static bool TryPickupItem(ArpgProfile30 profile,ArpgItem30 item,out string message)
        {
            message=string.Empty;if(profile==null||item==null){message="Invalid item.";return false;}profile.inventoryCapacity=200;
            if(!profile.AddItemToInventory(item)){message="The profile rejected the item.";return false;}
            ArpgArsenalStore32.EnsureProfile(profile);
            if(!ArpgArsenalStore32.AutoPlace(profile,item,ArpgArsenalStore32.InventoryContainer(profile.characterId),out message)){profile.items.Remove(item);return false;}
            message="Picked up "+item.displayName+".";return true;
        }
        public static bool GrantRewardItem(ArpgProfile30 profile,ArpgItem30 item,out string message)
        {
            message=string.Empty;if(profile==null||item==null){message="Invalid reward item.";return false;}
            if(!profile.items.Any(value=>value!=null&&value.instanceId==item.instanceId))profile.items.Add(item);
            ArpgArsenalStore32.EnsureProfile(profile);ArpgArsenalStore32.EnsureItemState(profile,item);
            if(ArpgArsenalStore32.AutoPlace(profile,item,ArpgArsenalStore32.InventoryContainer(profile.characterId),out message))return true;
            if(ArpgArsenalStore32.AutoPlace(profile,item,"stash.recovery",out message)){message="Recovery stash: "+item.displayName;return true;}
            message="Reward placement failed safely for "+item.displayName+".";return false;
        }
        public static ArpgItem30 GenerateCompletionReward(ArpgProfile30 profile,ArpgMapItem30 map,ArpgMapDefinition30 definition,int index,bool firstMastery)
        {
            int itemLevel=Mathf.Max(1,map.tier*3+1);int seed=map.seed+index*997;
            if(firstMastery&&index==0&&definition!=null)return ArpgArsenalItemFactory32.GenerateBossUnique(definition.bossId,itemLevel,seed);
            return ArpgArsenalItemFactory32.Generate(itemLevel,profile.characterClass,seed,ArpgStatHooks30.ItemRarityBonus(profile));
        }
        public static void AddLegacyCurrency(ArpgCurrency30 c,int amount){ArpgArsenalStore32.AddCurrency(ArpgArsenalContent32.FromLegacy(c),amount);}
        public static bool ShouldShowLoot(ArpgLootPickup31 pickup)
        {
            if(pickup==null||pickup.kind!=ArpgLootKind31.Item||pickup.item==null)return true;
            ArpgLootFilterPreset32 filter=ArpgArsenalStore32.ActiveFilter();if(filter==null)return true;ArpgItemState32 state=ArpgArsenalStore32.ItemState(pickup.item.instanceId);
            foreach(ArpgLootFilterRule32 rule in filter.rules.Where(x=>x!=null&&x.enabled))
            {
                bool match=(int)pickup.item.rarity>=(int)rule.minimumRarity&&pickup.item.itemLevel>=rule.minimumItemLevel;
                if(rule.uniquesOnly)match&=state!=null&&!string.IsNullOrEmpty(state.uniqueId);if(rule.exceptionalOnly)match&=state!=null&&state.exceptional;if(rule.corruptedOnly)match&=pickup.item.corrupted;
                if(!string.IsNullOrEmpty(rule.itemClass)){ArpgItemBaseDefinition30 b=ArpgContent30.ItemBase(pickup.item.baseId);match&=b!=null&&b.family.IndexOf(rule.itemClass,StringComparison.OrdinalIgnoreCase)>=0;}
                if(match)return rule.action==ArpgFilterAction32.Show;
            }
            return true;
        }
    }
}
