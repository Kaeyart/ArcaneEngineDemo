using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ArpgClass30 { Unchosen = -1, Mage = 0, Ranger = 1, Warrior = 2 }
    public enum ArpgAscendancy30
    {
        None = -1,
        Elementalist = 0, Chronomancer = 1, Voidcaller = 2,
        Deadeye = 3, Stormrunner = 4, Warden = 5,
        Juggernaut = 6, Spellblade = 7, Bastion = 8
    }

    public enum ArpgMapBand30 { White = 0, Blue = 1, Yellow = 2, Red = 3, Pinnacle = 4 }
    public enum ArpgMapRarity30 { Normal = 0, Magic = 1, Rare = 2 }
    public enum ArpgItemRarity30 { Normal = 0, Magic = 1, Rare = 2, Unique = 3, Exceptional = 4 }
    public enum ArpgItemSlot30 { MainHand, OffHand, Helmet, BodyArmour, Gloves, Boots, Belt, Amulet, Ring1, Ring2, Relic }
    public enum ArpgCurrency30
    {
        RefinementShard,
        SparkOfAlteration,
        RuneOfAugmentation,
        SigilOfElevation,
        ArcaneExalt,
        NullOrb,
        ReformationOrb,
        ChaosFragment,
        ElementalEssence,
        OmenOfControl,
        CorruptionCatalyst,
        DivineMeasure,
        FractureRune
    }

    public enum ArpgNodeSize30 { Small, Medium, Large, Completion }
    public enum ArpgStat30
    {
        SpellPower,
        MaximumHealth,
        MaximumMana,
        CriticalChance,
        MoveSpeed,
        CooldownRecovery,
        ManaEfficiency,
        Healing,
        GoldFind,
        TriggerEnergy,
        MapQuantity,
        MapRarity,
        MapSustain,
        ItemRarity,
        CurrencyFind,
        ExperienceGain,
        Attunement,
        Armour,
        Evasion,
        ArcaneWard,
        FirePower,
        ColdPower,
        LightningPower,
        PhysicalPower,
        BloodPower,
        ToxicPower,
        VoidPower,
        AilmentBuildup,
        ReactionPower,
        AreaOfEffect,
        Duration,
        ProjectileSpeed,
        ChainRetention,
        RuneCapacity,
        BarrierStrength
    }

    [Serializable]
    public sealed class ArpgStatModifier30
    {
        public ArpgStat30 stat;
        public float value;

        public ArpgStatModifier30() { }

        public ArpgStatModifier30(ArpgStat30 statValue, float amount)
        {
            stat = statValue;
            value = amount;
        }
    }

    [Serializable]
    public sealed class ArpgAffixRoll30
    {
        public string affixId;
        public int tier;
        public float value;
    }

    [Serializable]
    public sealed class ArpgItem30
    {
        public string instanceId;
        public string baseId;
        public string displayName;
        public ArpgItemSlot30 slot;
        public ArpgItemRarity30 rarity;
        public int itemLevel;
        public int quality;
        public bool corrupted;
        public bool fractured;
        public string fracturedAffixId;
        public bool locked;
        public long acquiredUnix;
        public List<ArpgAffixRoll30> prefixes = new List<ArpgAffixRoll30>();
        public List<ArpgAffixRoll30> suffixes = new List<ArpgAffixRoll30>();

        public int AffixCount
        {
            get { return (prefixes == null ? 0 : prefixes.Count) + (suffixes == null ? 0 : suffixes.Count); }
        }

        public int PrefixLimit
        {
            get { return rarity == ArpgItemRarity30.Magic ? 1 : (int)rarity >= (int)ArpgItemRarity30.Rare ? 3 : 0; }
        }

        public int SuffixLimit
        {
            get { return rarity == ArpgItemRarity30.Magic ? 1 : (int)rarity >= (int)ArpgItemRarity30.Rare ? 3 : 0; }
        }
    }

    [Serializable]
    public sealed class ArpgEquippedItem30
    {
        public ArpgItemSlot30 slot;
        public string itemInstanceId;
    }

    [Serializable]
    public sealed class ArpgMapItem30
    {
        public string instanceId;
        public string mapId;
        public int tier;
        public int seed;
        public ArpgMapRarity30 rarity;
        public bool corrupted;
        public int quality;
        public List<string> affixIds = new List<string>();

        public ArpgMapBand30 Band
        {
            get
            {
                if (tier >= 40) return ArpgMapBand30.Pinnacle;
                if (tier >= 30) return ArpgMapBand30.Red;
                if (tier >= 20) return ArpgMapBand30.Yellow;
                if (tier >= 10) return ArpgMapBand30.Blue;
                return ArpgMapBand30.White;
            }
        }
    }

    [Serializable]
    public sealed class ArpgCurrencyStack30
    {
        public ArpgCurrency30 currency;
        public int amount;
    }

    [Serializable]
    public sealed class ArpgProfile30
    {
        public int dataVersion = 32000;
        public int saveRevision;
        public string characterId = Guid.NewGuid().ToString("N");
        public string characterName = "Astral Wanderer";
        public ArpgClass30 characterClass = ArpgClass30.Unchosen;
        public ArpgAscendancy30 ascendancy = ArpgAscendancy30.None;
        public string createdUtc = DateTime.UtcNow.ToString("O");
        public string lastPlayedUtc = DateTime.UtcNow.ToString("O");
        public long totalPlaySeconds;
        public string currentLocation = "Astral Refuge";
        public string activeCoreId;
        public int level;
        public int experience;
        public int constellationPoints;
        public int atlasPoints;
        public int ascendancyPoints;
        public int highestCompletedTier = -1;
        public int totalMapsCompleted;
        public int totalDeaths;
        public int attunementBase = 4;
        public int inventoryCapacity = 40;
        public bool starterLoadoutInitialized;
        public bool migratedFromLegacy;
        public bool firstRefugeSequenceComplete;
        public bool hasSeenTitleIntroduction;
        public List<string> completedMapIds = new List<string>();
        public List<string> masteredMapIds = new List<string>();
        public List<string> allocatedConstellationNodes = new List<string>();
        public List<string> discoveredConstellations = new List<string>();
        public List<string> allocatedAscendancyNodes = new List<string>();
        public List<string> ownedCoreIds = new List<string>();
        public List<string> ownedRuneIds = new List<string>();
        public List<int> ownedLinkConditionIds = new List<int>();
        public List<string> stashItemIds = new List<string>();
        public List<string> objectiveFlags = new List<string>();
        public List<string> tutorialFlags = new List<string>();
        public List<ArpgMapItem30> maps = new List<ArpgMapItem30>();
        public List<ArpgItem30> items = new List<ArpgItem30>();
        public List<ArpgEquippedItem30> equipped = new List<ArpgEquippedItem30>();
        public List<ArpgCurrencyStack30> currencies = new List<ArpgCurrencyStack30>();

        public int ExperienceToNextLevel
        {
            get
            {
                int current = Mathf.Clamp(level, 0, 100);
                return 34 + current * 14 + Mathf.RoundToInt(current * current * 0.85f);
            }
        }

        public int AttunementMaximum
        {
            get
            {
                int fromLevel = level / 10;
                int fromAtlas = masteredMapIds == null ? 0 : masteredMapIds.Count / 3;
                return attunementBase + fromLevel + fromAtlas;
            }
        }

        public int InventoryCount
        {
            get
            {
                if (items == null) return 0;
                HashSet<string> equippedIds = new HashSet<string>((equipped ?? new List<ArpgEquippedItem30>())
                    .Where(value => value != null && !string.IsNullOrEmpty(value.itemInstanceId))
                    .Select(value => value.itemInstanceId));
                HashSet<string> stashIds = new HashSet<string>(stashItemIds ?? new List<string>());
                return items.Count(value => value != null && !equippedIds.Contains(value.instanceId) && !stashIds.Contains(value.instanceId));
            }
        }

        public int Currency(ArpgCurrency30 currency)
        {
            ArpgCurrencyStack30 stack = currencies == null
                ? null
                : currencies.FirstOrDefault(value => value != null && value.currency == currency);
            return stack == null ? 0 : stack.amount;
        }

        public void AddCurrency(ArpgCurrency30 currency, int amount)
        {
            if (amount == 0) return;
            if (currencies == null) currencies = new List<ArpgCurrencyStack30>();
            ArpgCurrencyStack30 stack = currencies.FirstOrDefault(value => value != null && value.currency == currency);
            if (stack == null)
            {
                stack = new ArpgCurrencyStack30 { currency = currency, amount = 0 };
                currencies.Add(stack);
            }

            stack.amount = Mathf.Clamp(stack.amount + amount, 0, 999999);
        }

        public bool SpendCurrency(ArpgCurrency30 currency, int amount)
        {
            amount = Mathf.Max(0, amount);
            if (Currency(currency) < amount) return false;
            AddCurrency(currency, -amount);
            return true;
        }

        public ArpgItem30 GetItem(string instanceId)
        {
            return items == null ? null : items.FirstOrDefault(value => value != null && value.instanceId == instanceId);
        }

        public ArpgItem30 Equipped(ArpgItemSlot30 slot)
        {
            ArpgEquippedItem30 record = equipped == null ? null : equipped.FirstOrDefault(value => value != null && value.slot == slot);
            return record == null ? null : GetItem(record.itemInstanceId);
        }

        public bool IsEquipped(string instanceId)
        {
            return equipped != null && equipped.Any(value => value != null && value.itemInstanceId == instanceId);
        }

        public bool IsInStash(string instanceId)
        {
            return stashItemIds != null && stashItemIds.Contains(instanceId);
        }

        public IEnumerable<ArpgItem30> InventoryItems()
        {
            if (items == null) yield break;
            foreach (ArpgItem30 item in items)
            {
                if (item == null || IsEquipped(item.instanceId) || IsInStash(item.instanceId)) continue;
                yield return item;
            }
        }

        public IEnumerable<ArpgItem30> StashItems()
        {
            if (stashItemIds == null) yield break;
            foreach (string id in stashItemIds)
            {
                ArpgItem30 item = GetItem(id);
                if (item != null) yield return item;
            }
        }

        public bool AddItemToInventory(ArpgItem30 item)
        {
            if (item == null || InventoryCount >= inventoryCapacity) return false;
            if (items == null) items = new List<ArpgItem30>();
            if (string.IsNullOrEmpty(item.instanceId)) item.instanceId = Guid.NewGuid().ToString("N");
            if (item.acquiredUnix <= 0) item.acquiredUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (!items.Any(value => value != null && value.instanceId == item.instanceId)) items.Add(item);
            if (stashItemIds != null) stashItemIds.Remove(item.instanceId);
            return true;
        }

        public bool MoveToStash(ArpgItem30 item)
        {
            if (item == null || IsEquipped(item.instanceId)) return false;
            if (stashItemIds == null) stashItemIds = new List<string>();
            if (!stashItemIds.Contains(item.instanceId)) stashItemIds.Add(item.instanceId);
            return true;
        }

        public bool MoveToInventory(ArpgItem30 item)
        {
            if (item == null || InventoryCount >= inventoryCapacity) return false;
            if (stashItemIds != null) stashItemIds.Remove(item.instanceId);
            return true;
        }

        public bool Equip(ArpgItem30 item, out string message)
        {
            message = string.Empty;
            if (item == null)
            {
                message = "Select an item.";
                return false;
            }

            ArpgItemBaseDefinition30 itemBase = ArpgContent30.ItemBase(item.baseId);
            int requirement = itemBase == null ? 0 : itemBase.requiredLevel;
            if (level < requirement)
            {
                message = "Requires level " + requirement + ".";
                return false;
            }

            if (equipped == null) equipped = new List<ArpgEquippedItem30>();
            ArpgEquippedItem30 record = equipped.FirstOrDefault(value => value != null && value.slot == item.slot);
            if (record == null)
            {
                record = new ArpgEquippedItem30 { slot = item.slot };
                equipped.Add(record);
            }

            record.itemInstanceId = item.instanceId;
            if (stashItemIds != null) stashItemIds.Remove(item.instanceId);
            message = "Equipped " + item.displayName + ".";
            return true;
        }

        public void Equip(ArpgItem30 item)
        {
            string ignored;
            Equip(item, out ignored);
        }

        public bool Unequip(ArpgItemSlot30 slot)
        {
            if (equipped == null) return false;
            ArpgEquippedItem30 record = equipped.FirstOrDefault(value => value != null && value.slot == slot);
            if (record == null) return false;
            equipped.Remove(record);
            return true;
        }
    }

    public static class ArpgProfileStore30
    {
        public const int CurrentSchema = 32000;

        private const string FileName = "arcane_arpg_300_profile.json";
        private const string BackupName = "arcane_arpg_300_profile.backup.json";

        public static string ProfilePath { get { return Path.Combine(Application.persistentDataPath, FileName); } }
        public static string BackupPath { get { return Path.Combine(Application.persistentDataPath, BackupName); } }

        public static ArpgProfile30 Load()
        {
            if (ArpgRosterStore31.Initialized && !string.IsNullOrEmpty(ArpgRosterStore31.ActiveCharacterId))
            {
                ArpgProfile30 active = ArpgRosterStore31.LoadCharacter(ArpgRosterStore31.ActiveCharacterId);
                if (active != null) return active;
            }

            ArpgProfile30 profile = TryLoad(ProfilePath);
            if (profile == null) profile = TryLoad(BackupPath);
            if (profile == null) profile = new ArpgProfile30();
            Repair(profile);
            return profile;
        }

        public static ArpgProfile30 LoadLegacy()
        {
            ArpgProfile30 profile = TryLoad(ProfilePath);
            if (profile == null) profile = TryLoad(BackupPath);
            if (profile != null) Repair(profile);
            return profile;
        }

        public static bool Save(ArpgProfile30 profile)
        {
            if (profile == null) return false;
            Repair(profile);
            if (ArpgRosterStore31.Initialized && !ArpgRosterStore31.LegacyMigrationInProgress)
                return ArpgRosterStore31.SaveCharacter(profile);

            return SaveLegacy(profile);
        }

        public static bool SaveLegacy(ArpgProfile30 profile)
        {
            if (profile == null) return false;
            Repair(profile);
            try
            {
                string directory = Path.GetDirectoryName(ProfilePath);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
                string json = JsonUtility.ToJson(profile, true);
                string temporary = ProfilePath + ".tmp";
                File.WriteAllText(temporary, json);
                if (File.Exists(ProfilePath)) File.Copy(ProfilePath, BackupPath, true);
                if (File.Exists(ProfilePath)) File.Delete(ProfilePath);
                File.Move(temporary, ProfilePath);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError("Arcane Engine profile save failed: " + exception.Message);
                return false;
            }
        }

        private static ArpgProfile30 TryLoad(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                string json = File.ReadAllText(path);
                return string.IsNullOrWhiteSpace(json) ? null : JsonUtility.FromJson<ArpgProfile30>(json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Arcane Engine profile could not be loaded from " + path + ": " + exception.Message);
                return null;
            }
        }

        public static void Repair(ArpgProfile30 profile)
        {
            if (profile == null) return;
            profile.dataVersion = CurrentSchema;
            if (string.IsNullOrEmpty(profile.characterId)) profile.characterId = Guid.NewGuid().ToString("N");
            if (string.IsNullOrWhiteSpace(profile.characterName)) profile.characterName = "Astral Wanderer";
            profile.characterName = profile.characterName.Trim();
            if (!Enum.IsDefined(typeof(ArpgClass30), profile.characterClass)) profile.characterClass = ArpgClass30.Unchosen;
            if (!Enum.IsDefined(typeof(ArpgAscendancy30), profile.ascendancy)) profile.ascendancy = ArpgAscendancy30.None;
            if (string.IsNullOrEmpty(profile.createdUtc)) profile.createdUtc = DateTime.UtcNow.ToString("O");
            if (string.IsNullOrEmpty(profile.lastPlayedUtc)) profile.lastPlayedUtc = profile.createdUtc;
            if (string.IsNullOrEmpty(profile.currentLocation)) profile.currentLocation = "Astral Refuge";
            profile.totalPlaySeconds = Math.Max(0L, profile.totalPlaySeconds);
            profile.saveRevision = Mathf.Max(0, profile.saveRevision);
            profile.level = Mathf.Clamp(profile.level, 0, 100);
            profile.experience = Mathf.Max(0, profile.experience);
            profile.constellationPoints = Mathf.Max(0, profile.constellationPoints);
            profile.atlasPoints = Mathf.Max(0, profile.atlasPoints);
            profile.ascendancyPoints = Mathf.Max(0, profile.ascendancyPoints);
            profile.highestCompletedTier = Mathf.Clamp(profile.highestCompletedTier, -1, 39);
            profile.totalMapsCompleted = Mathf.Max(0, profile.totalMapsCompleted);
            profile.totalDeaths = Mathf.Max(0, profile.totalDeaths);
            profile.attunementBase = Mathf.Clamp(profile.attunementBase, 0, 100);
            profile.inventoryCapacity = Mathf.Clamp(Mathf.Max(profile.inventoryCapacity, 200), 20, 200);

            if (profile.completedMapIds == null) profile.completedMapIds = new List<string>();
            if (profile.masteredMapIds == null) profile.masteredMapIds = new List<string>();
            if (profile.allocatedConstellationNodes == null) profile.allocatedConstellationNodes = new List<string>();
            if (profile.discoveredConstellations == null) profile.discoveredConstellations = new List<string>();
            if (profile.allocatedAscendancyNodes == null) profile.allocatedAscendancyNodes = new List<string>();
            if (profile.ownedCoreIds == null) profile.ownedCoreIds = new List<string>();
            if (profile.ownedRuneIds == null) profile.ownedRuneIds = new List<string>();
            if (profile.ownedLinkConditionIds == null) profile.ownedLinkConditionIds = new List<int>();
            if (profile.stashItemIds == null) profile.stashItemIds = new List<string>();
            if (profile.objectiveFlags == null) profile.objectiveFlags = new List<string>();
            if (profile.tutorialFlags == null) profile.tutorialFlags = new List<string>();
            if (profile.maps == null) profile.maps = new List<ArpgMapItem30>();
            if (profile.items == null) profile.items = new List<ArpgItem30>();
            if (profile.equipped == null) profile.equipped = new List<ArpgEquippedItem30>();
            if (profile.currencies == null) profile.currencies = new List<ArpgCurrencyStack30>();

            profile.completedMapIds = CleanStrings(profile.completedMapIds);
            profile.masteredMapIds = CleanStrings(profile.masteredMapIds);
            profile.allocatedConstellationNodes = CleanStrings(profile.allocatedConstellationNodes);
            profile.discoveredConstellations = CleanStrings(profile.discoveredConstellations);
            profile.allocatedAscendancyNodes = CleanStrings(profile.allocatedAscendancyNodes);
            profile.ownedCoreIds = CleanStrings(profile.ownedCoreIds);
            profile.ownedRuneIds = CleanStrings(profile.ownedRuneIds);
            profile.stashItemIds = CleanStrings(profile.stashItemIds);
            profile.objectiveFlags = CleanStrings(profile.objectiveFlags);
            profile.tutorialFlags = CleanStrings(profile.tutorialFlags);
            profile.ownedLinkConditionIds = profile.ownedLinkConditionIds
                .Where(value => Enum.IsDefined(typeof(SpellLinkCondition), value))
                .Distinct()
                .ToList();

            HashSet<string> mapInstances = new HashSet<string>();
            profile.maps.RemoveAll(value => value == null || string.IsNullOrEmpty(value.mapId));
            foreach (ArpgMapItem30 map in profile.maps.ToArray())
            {
                if (string.IsNullOrEmpty(map.instanceId)) map.instanceId = Guid.NewGuid().ToString("N");
                if (!mapInstances.Add(map.instanceId))
                {
                    profile.maps.Remove(map);
                    continue;
                }

                map.tier = Mathf.Clamp(map.tier, 0, 39);
                map.quality = Mathf.Clamp(map.quality, 0, 30);
                if (!Enum.IsDefined(typeof(ArpgMapRarity30), map.rarity)) map.rarity = ArpgMapRarity30.Normal;
                if (map.affixIds == null) map.affixIds = new List<string>();
                map.affixIds = CleanStrings(map.affixIds);
            }

            HashSet<string> itemInstances = new HashSet<string>();
            profile.items.RemoveAll(value => value == null || string.IsNullOrEmpty(value.baseId));
            foreach (ArpgItem30 item in profile.items.ToArray())
            {
                if (string.IsNullOrEmpty(item.instanceId)) item.instanceId = Guid.NewGuid().ToString("N");
                if (!itemInstances.Add(item.instanceId))
                {
                    profile.items.Remove(item);
                    continue;
                }

                item.itemLevel = Mathf.Clamp(item.itemLevel, 1, 200);
                item.quality = Mathf.Clamp(item.quality, 0, item.corrupted ? 30 : 20);
                item.acquiredUnix = Math.Max(0L, item.acquiredUnix);
                if (!Enum.IsDefined(typeof(ArpgItemSlot30), item.slot)) item.slot = ArpgItemSlot30.MainHand;
                if (!Enum.IsDefined(typeof(ArpgItemRarity30), item.rarity)) item.rarity = ArpgItemRarity30.Normal;
                if (item.prefixes == null) item.prefixes = new List<ArpgAffixRoll30>();
                if (item.suffixes == null) item.suffixes = new List<ArpgAffixRoll30>();
                RepairAffixes(item.prefixes, item.PrefixLimit);
                RepairAffixes(item.suffixes, item.SuffixLimit);

                List<ArpgAffixRoll30> all = item.prefixes.Concat(item.suffixes).ToList();
                if (item.fractured)
                {
                    if (string.IsNullOrEmpty(item.fracturedAffixId) || all.All(value => value.affixId != item.fracturedAffixId))
                        item.fracturedAffixId = all.Count == 0 ? string.Empty : all[0].affixId;
                    if (string.IsNullOrEmpty(item.fracturedAffixId)) item.fractured = false;
                }
                else item.fracturedAffixId = string.Empty;
            }

            HashSet<string> validItems = new HashSet<string>(profile.items.Select(value => value.instanceId));
            profile.stashItemIds = profile.stashItemIds.Where(validItems.Contains).Distinct().ToList();
            profile.equipped = profile.equipped
                .Where(value => value != null && !string.IsNullOrEmpty(value.itemInstanceId))
                .Where(value => validItems.Contains(value.itemInstanceId))
                .GroupBy(value => value.slot)
                .Select(group => group.Last())
                .Where(value =>
                {
                    ArpgItem30 item = profile.GetItem(value.itemInstanceId);
                    return item != null && item.slot == value.slot;
                })
                .ToList();

            HashSet<string> equippedIds = new HashSet<string>(profile.equipped.Select(value => value.itemInstanceId));
            profile.stashItemIds.RemoveAll(equippedIds.Contains);

            profile.currencies = profile.currencies
                .Where(value => value != null && value.amount > 0 && Enum.IsDefined(typeof(ArpgCurrency30), value.currency))
                .GroupBy(value => value.currency)
                .Select(group => new ArpgCurrencyStack30
                {
                    currency = group.Key,
                    amount = Mathf.Clamp(group.Sum(value => Mathf.Max(0, value.amount)), 0, 999999)
                })
                .Where(value => value.amount > 0)
                .ToList();

            if (string.IsNullOrEmpty(profile.activeCoreId) || !profile.ownedCoreIds.Contains(profile.activeCoreId))
                profile.activeCoreId = profile.ownedCoreIds.FirstOrDefault();
        }

        private static List<string> CleanStrings(IEnumerable<string> values)
        {
            return values == null
                ? new List<string>()
                : values.Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value.Trim()).Distinct().ToList();
        }

        private static void RepairAffixes(List<ArpgAffixRoll30> rolls, int limit)
        {
            rolls.RemoveAll(value => value == null || string.IsNullOrEmpty(value.affixId));
            HashSet<string> seen = new HashSet<string>();
            rolls.RemoveAll(value => !seen.Add(value.affixId));
            foreach (ArpgAffixRoll30 roll in rolls)
            {
                roll.tier = Mathf.Clamp(roll.tier, 1, 10);
                if (float.IsNaN(roll.value) || float.IsInfinity(roll.value)) roll.value = 0f;
            }

            if (rolls.Count > limit) rolls.RemoveRange(limit, rolls.Count - limit);
        }
    }
}
