using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ArcaneEngine
{
    public static class ArpgLegacyBridge30
    {
        public static bool InitializeFreshCharacter(GameWorld world, ArpgProfile30 profile, out string message)
        {
            message = string.Empty;
            if (world == null || profile == null || profile.characterClass == ArpgClass30.Unchosen)
            {
                message = "The world or class selection is unavailable.";
                return false;
            }
            try
            {
                DifficultySettings clean = new DifficultySettings { noStartingEquipment = true, noStartingModifiers = true };
                world.ClearTransientObjects();
                world.RunActive = false;
                world.ResetRunSystems(clean);
                if (world.Equipment != null) world.Equipment.LoadFromProfile(true);
                world.ModifierInventory.Clear();
                world.OwnedModifierCounts.Clear();
                world.RunCoreSatchel.Clear();
                if (world.SpellLinks != null) world.SpellLinks.ResetRun();

                string starter = FindStarterCore(profile.characterClass);
                if (string.IsNullOrEmpty(starter) || DemoCatalog.GetCore(starter) == null) starter = "fireball";
                ForceSingleCore(world, starter);
                profile.ownedCoreIds.Clear();
                profile.ownedCoreIds.Add(starter);
                profile.ownedRuneIds.Clear();
                profile.ownedLinkConditionIds.Clear();
                profile.starterLoadoutInitialized = true;
                PersistPreparedStarter(starter);
                world.MarkSpellsDirty();
                world.RepairModifierOwnership();
                world.RecalculateStats(false);
                if (world.Player != null) world.Player.ResetForRun();
                message = "Created a level-zero " + profile.characterClass + " with " + FriendlyCoreName(starter) + ".";
                return true;
            }
            catch (Exception exception)
            {
                message = "Could not initialize the fresh ARPG loadout: " + exception.Message;
                Debug.LogException(exception);
                return false;
            }
        }

        public static void RestoreOwnedDiscoveries(GameWorld world, ArpgProfile30 profile)
        {
            if (world == null || profile == null || profile.characterClass == ArpgClass30.Unchosen) return;
            try
            {
                if (world.Equipment != null) world.Equipment.LoadFromProfile(true);
                world.OwnedModifierCounts.Clear();
                world.ModifierInventory.Clear();
                foreach (string runeId in profile.ownedRuneIds)
                {
                    if (DemoCatalog.GetModifier(runeId) != null) world.AddModifier(runeId, 1);
                }
                EnsureOwnedCores(world, profile);
                if (world.SpellLinks != null)
                {
                    world.SpellLinks.ResetRun();
                    foreach (int value in profile.ownedLinkConditionIds)
                    {
                        if (Enum.IsDefined(typeof(SpellLinkCondition), value)) world.SpellLinks.GrantLegacyCondition((SpellLinkCondition)value);
                    }
                }
                world.RepairModifierOwnership();
                world.RecalculateStats(false);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Arcane Engine 3.0 discovery restoration failed safely: " + exception.Message);
            }
        }

        public static string GrantRandomRune(GameWorld world, ArpgProfile30 profile, int seed)
        {
            List<string> pool = EnumerateCatalog("AllModifiers")
                .Where(value => GetBool(value, "availableAsSupport", true))
                .Select(value => GetString(value, "id"))
                .Where(value => !string.IsNullOrEmpty(value) && DemoCatalog.GetModifier(value) != null && !profile.ownedRuneIds.Contains(value))
                .Distinct()
                .ToList();
            if (pool.Count == 0) return string.Empty;
            string selected = pool[ArpgDeterminism30.Index(seed, pool.Count)];
            profile.ownedRuneIds.Add(selected);
            if (world != null) world.AddModifier(selected, 1);
            SpellModifierDefinition definition = DemoCatalog.GetModifier(selected);
            return definition == null ? selected : definition.displayName;
        }

        public static string GrantRandomCore(GameWorld world, ArpgProfile30 profile, int seed)
        {
            List<string> pool = EnumerateCatalog("AllCores")
                .Select(value => GetString(value, "id"))
                .Where(value => !string.IsNullOrEmpty(value) && DemoCatalog.GetCore(value) != null && !profile.ownedCoreIds.Contains(value))
                .Distinct()
                .ToList();
            if (pool.Count == 0) return string.Empty;
            string selected = pool[ArpgDeterminism30.Index(seed, pool.Count)];
            profile.ownedCoreIds.Add(selected);
            if (world != null) world.AddCoreCopy(selected);
            return FriendlyCoreName(selected);
        }

        public static string GrantRandomLink(GameWorld world, ArpgProfile30 profile, int seed)
        {
            List<int> pool = Enum.GetValues(typeof(SpellLinkCondition)).Cast<SpellLinkCondition>()
                .Where(value => !string.Equals(value.ToString(), "None", StringComparison.OrdinalIgnoreCase))
                .Select(value => (int)value)
                .Where(value => !profile.ownedLinkConditionIds.Contains(value))
                .ToList();
            if (pool.Count == 0) return string.Empty;
            int selected = pool[ArpgDeterminism30.Index(seed, pool.Count)];
            profile.ownedLinkConditionIds.Add(selected);
            if (world != null && world.SpellLinks != null) world.SpellLinks.GrantLegacyCondition((SpellLinkCondition)selected);
            return ((SpellLinkCondition)selected).ToString();
        }

        public static string FindStarterCore(ArpgClass30 characterClass)
        {
            ArpgClassDefinition30 classDefinition = ArpgContent30.Class(characterClass);
            string[] hints = classDefinition == null || string.IsNullOrEmpty(classDefinition.starterCoreHint)
                ? new[] { "fireball" }
                : classDefinition.starterCoreHint.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<object> cores = EnumerateCatalog("AllCores");
            foreach (string hint in hints)
            {
                object match = cores.FirstOrDefault(value =>
                {
                    string id = GetString(value, "id");
                    string name = GetString(value, "displayName");
                    return (!string.IsNullOrEmpty(id) && id.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0) ||
                           (!string.IsNullOrEmpty(name) && name.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0);
                });
                string matchId = GetString(match, "id");
                if (!string.IsNullOrEmpty(matchId)) return matchId;
            }
            return DemoCatalog.GetCore("fireball") == null ? cores.Select(value => GetString(value, "id")).FirstOrDefault(value => !string.IsNullOrEmpty(value)) : "fireball";
        }


        private static void EnsureOwnedCores(GameWorld world, ArpgProfile30 profile)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            SpellBoard[] boards = typeof(GameWorld).GetField("_boards", flags).GetValue(world) as SpellBoard[];
            CoreSaveData[] active = typeof(GameWorld).GetField("_activeCores", flags).GetValue(world) as CoreSaveData[];
            bool[] bankable = typeof(GameWorld).GetField("_activeCoreBankable", flags).GetValue(world) as bool[];
            if (boards != null && active != null)
            {
                for (int index = 0; index < active.Length; index++)
                {
                    CoreSaveData core = active[index];
                    if (core == null || profile.ownedCoreIds.Contains(core.coreId)) continue;
                    boards[index] = null;
                    active[index] = null;
                    if (bankable != null && index < bankable.Length) bankable[index] = false;
                }
            }
            if ((boards == null || boards.All(value => value == null)) && profile.ownedCoreIds.Count > 0)
            {
                string starter = profile.ownedCoreIds.FirstOrDefault(value => DemoCatalog.GetCore(value) != null);
                if (!string.IsNullOrEmpty(starter)) ForceSingleCore(world, starter);
                boards = typeof(GameWorld).GetField("_boards", flags).GetValue(world) as SpellBoard[];
                active = typeof(GameWorld).GetField("_activeCores", flags).GetValue(world) as CoreSaveData[];
            }

            HashSet<string> existing = new HashSet<string>();
            if (active != null)
            {
                foreach (CoreSaveData core in active) if (core != null && !string.IsNullOrEmpty(core.coreId)) existing.Add(core.coreId);
            }
            foreach (CoreSaveData core in world.RunCoreSatchel) if (core != null && !string.IsNullOrEmpty(core.coreId)) existing.Add(core.coreId);
            foreach (string coreId in profile.ownedCoreIds)
            {
                if (string.IsNullOrEmpty(coreId) || existing.Contains(coreId) || DemoCatalog.GetCore(coreId) == null) continue;
                world.AddCoreCopy(coreId);
                existing.Add(coreId);
            }
            world.MarkSpellsDirty();
        }

        private static void PersistPreparedStarter(string starter)
        {
            try
            {
                if (ProfileManager.Current == null) return;
                ProfileManager.Current.preparedSpells.Clear();
                ProfileManager.Current.preparedSpells.Add(new PreparedSpellSave
                {
                    slotIndex = 0,
                    contentId = starter,
                    isRelic = false
                });
                ProfileManager.Current.preparedModifiers.Clear();
                ProfileManager.Save();
            }
            catch (Exception exception)
            {
                Debug.LogWarning("The starter Core is active, but the legacy prepared-loadout mirror could not be saved: " + exception.Message);
            }
        }

        private static void ForceSingleCore(GameWorld world, string coreId)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            SpellBoard[] boards = typeof(GameWorld).GetField("_boards", flags).GetValue(world) as SpellBoard[];
            CoreSaveData[] cores = typeof(GameWorld).GetField("_activeCores", flags).GetValue(world) as CoreSaveData[];
            bool[] bankable = typeof(GameWorld).GetField("_activeCoreBankable", flags).GetValue(world) as bool[];
            if (boards == null || cores == null || bankable == null) throw new InvalidOperationException("The SpellForge runtime layout could not be accessed.");
            for (int index = 0; index < boards.Length; index++)
            {
                boards[index] = null;
                cores[index] = null;
                bankable[index] = false;
            }
            boards[0] = new SpellBoard(SpellSlot.Slot1, coreId);
            cores[0] = new CoreSaveData(coreId);
        }

        private static List<object> EnumerateCatalog(string propertyName)
        {
            try
            {
                PropertyInfo property = typeof(DemoCatalog).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
                IEnumerable enumerable = property == null ? null : property.GetValue(null, null) as IEnumerable;
                if (enumerable == null) return new List<object>();
                List<object> result = new List<object>();
                foreach (object value in enumerable) if (value != null) result.Add(value);
                return result;
            }
            catch
            {
                return new List<object>();
            }
        }

        private static string GetString(object value, string member)
        {
            if (value == null) return string.Empty;
            Type type = value.GetType();
            FieldInfo field = type.GetField(member, BindingFlags.Public | BindingFlags.Instance);
            if (field != null) return field.GetValue(value) as string ?? string.Empty;
            PropertyInfo property = type.GetProperty(member, BindingFlags.Public | BindingFlags.Instance);
            return property == null ? string.Empty : property.GetValue(value, null) as string ?? string.Empty;
        }

        private static bool GetBool(object value, string member, bool fallback)
        {
            if (value == null) return fallback;
            Type type = value.GetType();
            FieldInfo field = type.GetField(member, BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(bool)) return (bool)field.GetValue(value);
            PropertyInfo property = type.GetProperty(member, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.PropertyType == typeof(bool)) return (bool)property.GetValue(value, null);
            return fallback;
        }

        private static string FriendlyCoreName(string id)
        {
            SpellCoreDefinition definition = DemoCatalog.GetCore(id);
            return definition == null ? id : definition.displayName;
        }
    }
}
