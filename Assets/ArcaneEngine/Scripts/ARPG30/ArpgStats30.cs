using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public const string Marker = "ARCANE_PATCH_310_FIRST_DESCENT_STATS";

        public static void ApplyPersistentStats(PlayerStats stats)
        {
            if (stats == null || ArpgFoundation30.Instance == null || ArpgFoundation30.Profile == null) return;
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            if (profile.characterClass == ArpgClass30.Unchosen) return;

            ArpgStatAccumulator30 accumulator = Build(profile);
            float elemental =
                accumulator.Get(ArpgStat30.FirePower) +
                accumulator.Get(ArpgStat30.ColdPower) +
                accumulator.Get(ArpgStat30.LightningPower) +
                accumulator.Get(ArpgStat30.PhysicalPower) +
                accumulator.Get(ArpgStat30.BloodPower) +
                accumulator.Get(ArpgStat30.ToxicPower) +
                accumulator.Get(ArpgStat30.VoidPower);

            stats.maxHealth += accumulator.Get(ArpgStat30.MaximumHealth) + accumulator.Get(ArpgStat30.Armour) * 0.12f;
            stats.maxMana += accumulator.Get(ArpgStat30.MaximumMana) + accumulator.Get(ArpgStat30.ArcaneWard) * 0.18f;
            stats.spellPower += accumulator.Get(ArpgStat30.SpellPower) + elemental * 0.35f + accumulator.Get(ArpgStat30.ReactionPower) * 0.25f;
            stats.critChance += accumulator.Get(ArpgStat30.CriticalChance);
            stats.moveSpeed += accumulator.Get(ArpgStat30.MoveSpeed);
            stats.triggerEnergy += accumulator.Get(ArpgStat30.TriggerEnergy);
            stats.cooldownMultiplier *= Mathf.Clamp(1f - accumulator.Get(ArpgStat30.CooldownRecovery), 0.35f, 1f);
            stats.manaCostMultiplier *= Mathf.Clamp(1f - accumulator.Get(ArpgStat30.ManaEfficiency), 0.35f, 1f);
            stats.healingMultiplier *= 1f + Mathf.Max(0f, accumulator.Get(ArpgStat30.Healing));
            stats.goldFind += accumulator.Get(ArpgStat30.GoldFind);

            AddMember(stats, "armour", accumulator.Get(ArpgStat30.Armour));
            AddMember(stats, "armor", accumulator.Get(ArpgStat30.Armour));
            AddMember(stats, "evasion", accumulator.Get(ArpgStat30.Evasion));
            AddMember(stats, "arcaneWard", accumulator.Get(ArpgStat30.ArcaneWard));
            AddMember(stats, "projectileSpeed", accumulator.Get(ArpgStat30.ProjectileSpeed));
            AddMember(stats, "areaMultiplier", accumulator.Get(ArpgStat30.AreaOfEffect));
            AddMember(stats, "durationMultiplier", accumulator.Get(ArpgStat30.Duration));
            AddMember(stats, "barrierMultiplier", accumulator.Get(ArpgStat30.BarrierStrength));
            AddMember(stats, "ailmentBuildup", accumulator.Get(ArpgStat30.AilmentBuildup));
            AddMember(stats, "reactionPower", accumulator.Get(ArpgStat30.ReactionPower));
            AddMember(stats, "runeCapacity", accumulator.Get(ArpgStat30.RuneCapacity));
            AddMember(stats, "chainRetention", accumulator.Get(ArpgStat30.ChainRetention));
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

            foreach (string nodeId in profile.allocatedConstellationNodes ?? new List<string>())
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

            foreach (ArpgEquippedItem30 record in profile.equipped ?? new List<ArpgEquippedItem30>())
            {
                if (record == null) continue;
                ArpgItem30 item = profile.GetItem(record.itemInstanceId);
                ArpgItems30.AddItemStats(item, accumulator);
            }

            accumulator.Add(ArpgStat30.MaximumHealth, profile.level * (profile.characterClass == ArpgClass30.Warrior ? 4.5f : 2.8f));
            accumulator.Add(ArpgStat30.MaximumMana, profile.level * (profile.characterClass == ArpgClass30.Mage ? 3.2f : 1.5f));
            accumulator.Add(ArpgStat30.SpellPower, profile.level * 0.007f);

            ApplyCompletionBoons(profile, accumulator);
            return accumulator;
        }

        private static void ApplyCompletionBoons(ArpgProfile30 profile, ArpgStatAccumulator30 accumulator)
        {
            if (profile.allocatedConstellationNodes.Contains("constellation.pyre-crown.completion"))
            {
                accumulator.Add(ArpgStat30.FirePower, 0.12f);
                accumulator.Add(ArpgStat30.AilmentBuildup, 0.08f);
            }

            if (profile.allocatedConstellationNodes.Contains("constellation.winters-veil.completion"))
            {
                accumulator.Add(ArpgStat30.ColdPower, 0.10f);
                accumulator.Add(ArpgStat30.Duration, 0.12f);
            }

            if (profile.allocatedConstellationNodes.Contains("constellation.storm-circuit.completion"))
            {
                accumulator.Add(ArpgStat30.LightningPower, 0.10f);
                accumulator.Add(ArpgStat30.ChainRetention, 0.18f);
            }

            if (profile.allocatedConstellationNodes.Contains("constellation.iron-colossus.completion"))
            {
                float armour = accumulator.Get(ArpgStat30.Armour);
                accumulator.Add(ArpgStat30.PhysicalPower, Mathf.Clamp(armour / 700f, 0f, 0.25f));
                accumulator.Add(ArpgStat30.MoveSpeed, -0.18f);
            }

            if (profile.allocatedConstellationNodes.Contains("constellation.living-bulwark.completion"))
            {
                accumulator.Add(ArpgStat30.MaximumHealth, 25f);
                accumulator.Add(ArpgStat30.BarrierStrength, 0.15f);
            }

            if (profile.allocatedConstellationNodes.Contains("constellation.weavers-hand.completion"))
            {
                accumulator.Add(ArpgStat30.RuneCapacity, 2f);
                accumulator.Add(ArpgStat30.ManaEfficiency, 0.08f);
            }
        }

        public static int AttunementUsed(ArpgProfile30 profile)
        {
            if (profile == null) return 0;
            int total = 0;
            foreach (ArpgConstellationDefinition30 constellation in ArpgContent30.Constellations)
            {
                ArpgConstellationNodeDefinition30 completion = constellation.nodes.LastOrDefault(value => value.size == ArpgNodeSize30.Completion);
                if (completion != null && profile.allocatedConstellationNodes.Contains(completion.id))
                    total += constellation.attunementCost;
            }
            return total;
        }

        public static int AttunementMaximum(ArpgProfile30 profile)
        {
            if (profile == null) return 0;
            int result = profile.AttunementMaximum;
            result += Mathf.RoundToInt(Build(profile).Get(ArpgStat30.Attunement));
            return result;
        }

        public static float MapRewardMultiplier(ArpgProfile30 profile, ArpgMapItem30 map)
        {
            if (profile == null || map == null) return 1f;
            ArpgStatAccumulator30 accumulator = Build(profile);
            float result = 1f + accumulator.Get(ArpgStat30.MapQuantity) + map.quality / 100f;
            foreach (string affixId in map.affixIds ?? new List<string>())
            {
                ArpgMapAffixDefinition30 affix = ArpgContent30.MapAffix(affixId);
                if (affix != null) result *= affix.rewardMultiplier;
            }
            if (map.corrupted) result *= 1.18f;
            return Mathf.Clamp(result, 1f, 4f);
        }

        public static float ItemRarityBonus(ArpgProfile30 profile)
        {
            return Mathf.Clamp(Build(profile).Get(ArpgStat30.ItemRarity), 0f, 0.35f);
        }

        public static float CurrencyFindBonus(ArpgProfile30 profile)
        {
            return Mathf.Clamp(Build(profile).Get(ArpgStat30.CurrencyFind), 0f, 0.5f);
        }

        public static float MapSustainBonus(ArpgProfile30 profile)
        {
            return Mathf.Clamp(Build(profile).Get(ArpgStat30.MapSustain), 0f, 0.5f);
        }

        public static float ExperienceBonus(ArpgProfile30 profile)
        {
            return Mathf.Clamp(Build(profile).Get(ArpgStat30.ExperienceGain), 0f, 0.5f);
        }

        private static void AddMember(object target, string name, float amount)
        {
            if (target == null || Mathf.Abs(amount) < 0.0001f) return;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type type = target.GetType();
            FieldInfo field = type.GetField(name, flags);
            if (field != null && field.FieldType == typeof(float))
            {
                field.SetValue(target, (float)field.GetValue(target) + amount);
                return;
            }

            PropertyInfo property = type.GetProperty(name, flags);
            if (property != null && property.CanRead && property.CanWrite && property.PropertyType == typeof(float))
                property.SetValue(target, (float)property.GetValue(target, null) + amount, null);
        }
    }
}
