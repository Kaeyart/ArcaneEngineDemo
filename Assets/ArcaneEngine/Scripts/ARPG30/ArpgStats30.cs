using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ArpgStatAccumulator30
    {
        private readonly Dictionary<ArpgStat30, float> _values = new Dictionary<ArpgStat30, float>();

        public void Add(ArpgStatModifier30 modifier, float multiplier)
        {
            if (modifier == null) return;
            Add(modifier.stat, modifier.value * multiplier);
        }

        public void Add(ArpgStat30 stat, float value)
        {
            float current;
            _values.TryGetValue(stat, out current);
            _values[stat] = current + value;
        }

        public float Get(ArpgStat30 stat)
        {
            float value;
            return _values.TryGetValue(stat, out value) ? value : 0f;
        }
    }

    public static class ArpgStatHooks30
    {
        public const string Marker = "ARCANE_PATCH_300A1_PERSISTENT_STATS";

        public static void ApplyPersistentStats(PlayerStats stats)
        {
            if (stats == null || ArpgFoundation30.Instance == null || ArpgFoundation30.Profile == null) return;
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            if (profile.characterClass == ArpgClass30.Unchosen) return;
            ArpgStatAccumulator30 accumulator = Build(profile);

            stats.maxHealth += accumulator.Get(ArpgStat30.MaximumHealth);
            stats.maxMana += accumulator.Get(ArpgStat30.MaximumMana);
            stats.spellPower += accumulator.Get(ArpgStat30.SpellPower);
            stats.critChance += accumulator.Get(ArpgStat30.CriticalChance);
            stats.moveSpeed += accumulator.Get(ArpgStat30.MoveSpeed);
            stats.triggerEnergy += accumulator.Get(ArpgStat30.TriggerEnergy);
            stats.cooldownMultiplier *= Mathf.Clamp(1f - accumulator.Get(ArpgStat30.CooldownRecovery), 0.35f, 1f);
            stats.manaCostMultiplier *= Mathf.Clamp(1f - accumulator.Get(ArpgStat30.ManaEfficiency), 0.35f, 1f);
            stats.healingMultiplier *= 1f + Mathf.Max(0f, accumulator.Get(ArpgStat30.Healing));
            stats.goldFind += accumulator.Get(ArpgStat30.GoldFind);
        }

        public static ArpgStatAccumulator30 Build(ArpgProfile30 profile)
        {
            ArpgContent30.Ensure();
            ArpgStatAccumulator30 accumulator = new ArpgStatAccumulator30();
            if (profile == null) return accumulator;

            ArpgClassDefinition30 classDefinition = ArpgContent30.Class(profile.characterClass);
            if (classDefinition != null)
            {
                foreach (ArpgStatModifier30 modifier in classDefinition.baseModifiers) accumulator.Add(modifier, 1f);
            }

            foreach (string nodeId in profile.allocatedConstellationNodes)
            {
                ArpgConstellationNodeDefinition30 node = ArpgContent30.ConstellationNode(nodeId);
                if (node == null) continue;
                foreach (ArpgStatModifier30 modifier in node.modifiers) accumulator.Add(modifier, 1f);
            }

            if (profile.ascendancy != ArpgAscendancy30.None)
            {
                ArpgAscendancyDefinition30 ascendancy = ArpgContent30.Ascendancy(profile.ascendancy);
                if (ascendancy != null)
                {
                    foreach (ArpgAscendancyNodeDefinition30 node in ascendancy.nodes)
                    {
                        if (!profile.allocatedAscendancyNodes.Contains(node.id)) continue;
                        foreach (ArpgStatModifier30 modifier in node.modifiers) accumulator.Add(modifier, 1f);
                    }
                }
            }

            foreach (ArpgEquippedItem30 record in profile.equipped)
            {
                ArpgItem30 item = profile.GetItem(record.itemInstanceId);
                ArpgItems30.AddItemStats(item, accumulator);
            }

            accumulator.Add(ArpgStat30.MaximumHealth, profile.level * (profile.characterClass == ArpgClass30.Warrior ? 4f : 2.5f));
            accumulator.Add(ArpgStat30.MaximumMana, profile.level * (profile.characterClass == ArpgClass30.Mage ? 3f : 1.5f));
            accumulator.Add(ArpgStat30.SpellPower, profile.level * 0.006f);
            return accumulator;
        }

        public static int AttunementUsed(ArpgProfile30 profile)
        {
            if (profile == null) return 0;
            int total = 0;
            foreach (ArpgConstellationDefinition30 constellation in ArpgContent30.Constellations)
            {
                ArpgConstellationNodeDefinition30 completion = constellation.nodes.LastOrDefault(value => value.size == ArpgNodeSize30.Completion);
                if (completion != null && profile.allocatedConstellationNodes.Contains(completion.id)) total += constellation.attunementCost;
            }
            return total;
        }

        public static int AttunementMaximum(ArpgProfile30 profile)
        {
            if (profile == null) return 0;
            int result = profile.AttunementMaximum;
            ArpgStatAccumulator30 accumulator = Build(profile);
            result += Mathf.RoundToInt(accumulator.Get(ArpgStat30.Attunement));
            return result;
        }

        public static float MapRewardMultiplier(ArpgProfile30 profile, ArpgMapItem30 map)
        {
            ArpgStatAccumulator30 accumulator = Build(profile);
            float result = 1f + accumulator.Get(ArpgStat30.MapQuantity) + map.quality / 100f;
            foreach (string affixId in map.affixIds)
            {
                ArpgMapAffixDefinition30 affix = ArpgContent30.MapAffix(affixId);
                if (affix != null) result *= affix.rewardMultiplier;
            }
            if (map.corrupted) result *= 1.18f;
            return Mathf.Clamp(result, 1f, 4f);
        }

        public static float ItemRarityBonus(ArpgProfile30 profile)
        {
            return Mathf.Clamp(ArpgStatHooks30.Build(profile).Get(ArpgStat30.ItemRarity), 0f, 0.35f);
        }

        public static float CurrencyFindBonus(ArpgProfile30 profile)
        {
            return Mathf.Clamp(ArpgStatHooks30.Build(profile).Get(ArpgStat30.CurrencyFind), 0f, 0.5f);
        }

        public static float MapSustainBonus(ArpgProfile30 profile)
        {
            return Mathf.Clamp(ArpgStatHooks30.Build(profile).Get(ArpgStat30.MapSustain), 0f, 0.5f);
        }

        public static float ExperienceBonus(ArpgProfile30 profile)
        {
            return Mathf.Clamp(ArpgStatHooks30.Build(profile).Get(ArpgStat30.ExperienceGain), 0f, 0.5f);
        }
    }
}
