using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ArpgSaveHealth31
    {
        Healthy,
        RestoredBackup,
        Migrated,
        Damaged
    }

    [Serializable]
    public sealed class ArpgCharacterSlot31
    {
        public string characterId;
        public string characterName;
        public ArpgClass30 characterClass;
        public ArpgAscendancy30 ascendancy;
        public int level;
        public int highestCompletedTier;
        public string createdUtc;
        public string lastPlayedUtc;
        public long totalPlaySeconds;
        public string currentLocation;
        public ArpgSaveHealth31 saveHealth = ArpgSaveHealth31.Healthy;
    }

    [Serializable]
    public sealed class ArpgRosterIndex31
    {
        public int dataVersion = 31000;
        public string activeCharacterId;
        public List<ArpgCharacterSlot31> characters = new List<ArpgCharacterSlot31>();
    }

    public static class ArpgRosterStore31
    {
        public const int MaximumSlots = 8;

        private const string RootFolderName = "ArcaneEngine31";
        private const string IndexFileName = "roster.json";
        private static ArpgRosterIndex31 _index;

        public static bool Initialized { get; private set; }
        public static bool LegacyMigrationInProgress { get; private set; }
        public static string LastSaveStatus { get; private set; } = "Save system ready.";
        public static float LastSaveStatusRealtime { get; private set; }

        public static string RootPath { get { return Path.Combine(Application.persistentDataPath, RootFolderName); } }
        public static string CharacterPath { get { return Path.Combine(RootPath, "Characters"); } }
        public static string BackupPath { get { return Path.Combine(RootPath, "Backups"); } }
        public static string DeletedPath { get { return Path.Combine(RootPath, "DeletedCharacters"); } }
        public static string IndexPath { get { return Path.Combine(RootPath, IndexFileName); } }
        public static string ActiveCharacterId { get { return _index == null ? string.Empty : _index.activeCharacterId; } }

        public static IReadOnlyList<ArpgCharacterSlot31> Slots
        {
            get
            {
                Initialize();
                return _index.characters
                    .OrderByDescending(value => ParseUtc(value.lastPlayedUtc))
                    .ThenBy(value => value.characterName)
                    .ToList();
            }
        }

        public static void Initialize()
        {
            if (Initialized) return;
            Directory.CreateDirectory(RootPath);
            Directory.CreateDirectory(CharacterPath);
            Directory.CreateDirectory(BackupPath);
            Directory.CreateDirectory(DeletedPath);

            _index = LoadIndex();
            if (_index == null) _index = new ArpgRosterIndex31();
            RepairIndex(_index);
            ImportLegacyProfileIfNecessary();
            RebuildMissingSummaries();
            SaveIndex();
            Initialized = true;
        }

        public static ArpgCharacterSlot31 Slot(string characterId)
        {
            Initialize();
            return _index.characters.FirstOrDefault(value => value != null && value.characterId == characterId);
        }

        public static ArpgProfile30 CreateCharacter(string characterName, ArpgClass30 characterClass, out string message)
        {
            Initialize();
            message = ValidateCharacterName(characterName, null);
            if (!string.IsNullOrEmpty(message)) return null;
            if (_index.characters.Count >= MaximumSlots)
            {
                message = "All " + MaximumSlots + " character slots are occupied.";
                return null;
            }

            DateTime now = DateTime.UtcNow;
            ArpgProfile30 profile = new ArpgProfile30
            {
                dataVersion = 31000,
                characterId = Guid.NewGuid().ToString("N"),
                characterName = characterName.Trim(),
                characterClass = characterClass,
                ascendancy = ArpgAscendancy30.None,
                createdUtc = now.ToString("O"),
                lastPlayedUtc = now.ToString("O"),
                currentLocation = "Astral Refuge",
                level = 0,
                experience = 0,
                constellationPoints = 0,
                atlasPoints = 0,
                ascendancyPoints = 0,
                highestCompletedTier = -1
            };

            ArpgProfileStore30.Repair(profile);
            _index.characters.Add(Summary(profile, ArpgSaveHealth31.Healthy));
            _index.activeCharacterId = profile.characterId;
            if (!SaveCharacter(profile))
            {
                _index.characters.RemoveAll(value => value.characterId == profile.characterId);
                SaveIndex();
                message = "The character could not be written to disk.";
                return null;
            }

            message = "Created " + profile.characterName + " the " + profile.characterClass + ".";
            return profile;
        }

        public static string ValidateCharacterName(string candidate, string ignoredCharacterId)
        {
            Initialize();
            if (string.IsNullOrWhiteSpace(candidate)) return "Enter a character name.";
            string value = candidate.Trim();
            if (value.Length < 3 || value.Length > 16) return "Names must contain 3–16 characters.";
            if (char.IsWhiteSpace(candidate[0]) || char.IsWhiteSpace(candidate[candidate.Length - 1]))
                return "Names cannot begin or end with whitespace.";

            for (int index = 0; index < value.Length; index++)
            {
                char character = value[index];
                if (char.IsLetterOrDigit(character) || character == ' ' || character == '\'' || character == '-') continue;
                return "Names may use letters, numbers, spaces, apostrophes, and hyphens.";
            }

            if (_index.characters.Any(slot =>
                slot != null &&
                slot.characterId != ignoredCharacterId &&
                string.Equals(slot.characterName, value, StringComparison.OrdinalIgnoreCase)))
                return "That character name is already in use.";

            return string.Empty;
        }

        public static bool RenameCharacter(string characterId, string newName, out string message)
        {
            Initialize();
            message = ValidateCharacterName(newName, characterId);
            if (!string.IsNullOrEmpty(message)) return false;

            ArpgProfile30 profile = LoadCharacter(characterId);
            if (profile == null)
            {
                message = "The character save could not be loaded.";
                return false;
            }

            profile.characterName = newName.Trim();
            if (!SaveCharacter(profile))
            {
                message = "The renamed character could not be saved.";
                return false;
            }

            message = "Renamed the character to " + profile.characterName + ".";
            return true;
        }

        public static bool DeleteCharacter(string characterId, string typedName, out string message)
        {
            Initialize();
            ArpgCharacterSlot31 slot = Slot(characterId);
            if (slot == null)
            {
                message = "Character slot not found.";
                return false;
            }

            if (!string.Equals((typedName ?? string.Empty).Trim(), slot.characterName, StringComparison.Ordinal))
            {
                message = "Type the exact character name to confirm deletion.";
                return false;
            }

            try
            {
                string stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
                MoveIfPresent(CharacterFile(characterId), Path.Combine(DeletedPath, stamp + "-" + characterId + ".json"));
                MoveIfPresent(BackupFile(characterId, 1), Path.Combine(DeletedPath, stamp + "-" + characterId + ".backup1.json"));
                MoveIfPresent(BackupFile(characterId, 2), Path.Combine(DeletedPath, stamp + "-" + characterId + ".backup2.json"));
                _index.characters.RemoveAll(value => value != null && value.characterId == characterId);
                if (_index.activeCharacterId == characterId)
                    _index.activeCharacterId = _index.characters
                        .OrderByDescending(value => ParseUtc(value.lastPlayedUtc))
                        .Select(value => value.characterId)
                        .FirstOrDefault();
                SaveIndex();
                message = slot.characterName + " was moved to the recoverable DeletedCharacters folder.";
                SetStatus("Character moved to recoverable deletion.", true);
                return true;
            }
            catch (Exception exception)
            {
                message = "Character deletion failed: " + exception.Message;
                SetStatus("Character deletion failed.", false);
                return false;
            }
        }

        public static void SetActiveCharacter(string characterId)
        {
            Initialize();
            if (!string.IsNullOrEmpty(characterId) && Slot(characterId) == null) return;
            _index.activeCharacterId = characterId;
            SaveIndex();
        }

        public static ArpgProfile30 LoadCharacter(string characterId)
        {
            Initialize();
            if (string.IsNullOrEmpty(characterId)) return null;

            ArpgProfile30 profile = TryLoadProfile(CharacterFile(characterId));
            ArpgSaveHealth31 health = ArpgSaveHealth31.Healthy;
            if (profile == null)
            {
                profile = TryLoadProfile(BackupFile(characterId, 1));
                health = ArpgSaveHealth31.RestoredBackup;
            }
            if (profile == null)
            {
                profile = TryLoadProfile(BackupFile(characterId, 2));
                health = ArpgSaveHealth31.RestoredBackup;
            }

            if (profile == null)
            {
                ArpgCharacterSlot31 damaged = Slot(characterId);
                if (damaged != null) damaged.saveHealth = ArpgSaveHealth31.Damaged;
                SaveIndex();
                SetStatus("Character save is damaged.", false);
                return null;
            }

            ArpgProfileStore30.Repair(profile);
            profile.characterId = characterId;
            UpdateSummary(profile, health);
            if (health == ArpgSaveHealth31.RestoredBackup)
            {
                WriteProfile(CharacterFile(characterId), profile);
                SetStatus("Backup restored for " + profile.characterName + ".", true);
            }
            else SetStatus("Loaded " + profile.characterName + ".", true);
            return profile;
        }

        public static bool SaveCharacter(ArpgProfile30 profile)
        {
            Initialize();
            if (profile == null || string.IsNullOrEmpty(profile.characterId)) return false;

            ArpgProfileStore30.Repair(profile);
            profile.saveRevision++;
            profile.lastPlayedUtc = DateTime.UtcNow.ToString("O");

            try
            {
                string current = CharacterFile(profile.characterId);
                string backup1 = BackupFile(profile.characterId, 1);
                string backup2 = BackupFile(profile.characterId, 2);
                string temporary = current + ".tmp";

                Directory.CreateDirectory(CharacterPath);
                Directory.CreateDirectory(BackupPath);
                File.WriteAllText(temporary, JsonUtility.ToJson(profile, true));

                if (File.Exists(backup2)) File.Delete(backup2);
                if (File.Exists(backup1)) File.Move(backup1, backup2);
                if (File.Exists(current)) File.Copy(current, backup1, true);
                if (File.Exists(current)) File.Delete(current);
                File.Move(temporary, current);

                _index.activeCharacterId = profile.characterId;
                UpdateSummary(profile, ArpgSaveHealth31.Healthy);
                SaveIndex();
                SetStatus("Save complete.", true);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError("Arcane Engine 3.1 character save failed: " + exception.Message);
                SetStatus("Save failed: " + exception.Message, false);
                return false;
            }
        }

        private static ArpgRosterIndex31 LoadIndex()
        {
            try
            {
                if (!File.Exists(IndexPath)) return null;
                string json = File.ReadAllText(IndexPath);
                return string.IsNullOrWhiteSpace(json) ? null : JsonUtility.FromJson<ArpgRosterIndex31>(json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Arcane Engine roster index could not be loaded: " + exception.Message);
                return null;
            }
        }

        private static void SaveIndex()
        {
            if (_index == null) return;
            RepairIndex(_index);
            try
            {
                string temporary = IndexPath + ".tmp";
                File.WriteAllText(temporary, JsonUtility.ToJson(_index, true));
                if (File.Exists(IndexPath)) File.Delete(IndexPath);
                File.Move(temporary, IndexPath);
            }
            catch (Exception exception)
            {
                Debug.LogError("Arcane Engine roster index save failed: " + exception.Message);
            }
        }

        private static void ImportLegacyProfileIfNecessary()
        {
            if (_index.characters.Count > 0) return;
            ArpgProfile30 legacy = ArpgProfileStore30.LoadLegacy();
            if (legacy == null || legacy.characterClass == ArpgClass30.Unchosen) return;

            LegacyMigrationInProgress = true;
            try
            {
                legacy.dataVersion = 31000;
                legacy.migratedFromLegacy = true;
                if (string.IsNullOrEmpty(legacy.characterId)) legacy.characterId = Guid.NewGuid().ToString("N");
                if (string.IsNullOrEmpty(legacy.createdUtc)) legacy.createdUtc = DateTime.UtcNow.ToString("O");
                legacy.lastPlayedUtc = DateTime.UtcNow.ToString("O");
                _index.characters.Add(Summary(legacy, ArpgSaveHealth31.Migrated));
                _index.activeCharacterId = legacy.characterId;
                WriteProfile(CharacterFile(legacy.characterId), legacy);
                SetStatus("Legacy 3.0 profile migrated.", true);
            }
            finally
            {
                LegacyMigrationInProgress = false;
            }
        }

        private static void RebuildMissingSummaries()
        {
            HashSet<string> ids = new HashSet<string>(_index.characters
                .Where(value => value != null && !string.IsNullOrEmpty(value.characterId))
                .Select(value => value.characterId));

            foreach (string file in Directory.GetFiles(CharacterPath, "*.json"))
            {
                string id = Path.GetFileNameWithoutExtension(file);
                if (ids.Contains(id)) continue;
                ArpgProfile30 profile = TryLoadProfile(file);
                if (profile == null) continue;
                profile.characterId = id;
                _index.characters.Add(Summary(profile, ArpgSaveHealth31.Healthy));
                ids.Add(id);
            }
        }

        private static void RepairIndex(ArpgRosterIndex31 index)
        {
            index.dataVersion = 31000;
            if (index.characters == null) index.characters = new List<ArpgCharacterSlot31>();
            index.characters = index.characters
                .Where(value => value != null && !string.IsNullOrEmpty(value.characterId))
                .GroupBy(value => value.characterId)
                .Select(group => group.Last())
                .Take(MaximumSlots)
                .ToList();

            if (!string.IsNullOrEmpty(index.activeCharacterId) &&
                index.characters.All(value => value.characterId != index.activeCharacterId))
                index.activeCharacterId = string.Empty;
        }

        private static ArpgProfile30 TryLoadProfile(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                string json = File.ReadAllText(path);
                return string.IsNullOrWhiteSpace(json) ? null : JsonUtility.FromJson<ArpgProfile30>(json);
            }
            catch
            {
                return null;
            }
        }

        private static void WriteProfile(string path, ArpgProfile30 profile)
        {
            string temporary = path + ".tmp";
            File.WriteAllText(temporary, JsonUtility.ToJson(profile, true));
            if (File.Exists(path)) File.Delete(path);
            File.Move(temporary, path);
        }

        private static void UpdateSummary(ArpgProfile30 profile, ArpgSaveHealth31 health)
        {
            ArpgCharacterSlot31 slot = _index.characters.FirstOrDefault(value => value.characterId == profile.characterId);
            ArpgCharacterSlot31 replacement = Summary(profile, health);
            if (slot == null) _index.characters.Add(replacement);
            else
            {
                slot.characterName = replacement.characterName;
                slot.characterClass = replacement.characterClass;
                slot.ascendancy = replacement.ascendancy;
                slot.level = replacement.level;
                slot.highestCompletedTier = replacement.highestCompletedTier;
                slot.createdUtc = replacement.createdUtc;
                slot.lastPlayedUtc = replacement.lastPlayedUtc;
                slot.totalPlaySeconds = replacement.totalPlaySeconds;
                slot.currentLocation = replacement.currentLocation;
                slot.saveHealth = replacement.saveHealth;
            }
        }

        private static ArpgCharacterSlot31 Summary(ArpgProfile30 profile, ArpgSaveHealth31 health)
        {
            return new ArpgCharacterSlot31
            {
                characterId = profile.characterId,
                characterName = profile.characterName,
                characterClass = profile.characterClass,
                ascendancy = profile.ascendancy,
                level = profile.level,
                highestCompletedTier = profile.highestCompletedTier,
                createdUtc = profile.createdUtc,
                lastPlayedUtc = profile.lastPlayedUtc,
                totalPlaySeconds = profile.totalPlaySeconds,
                currentLocation = profile.currentLocation,
                saveHealth = health
            };
        }

        private static string CharacterFile(string characterId)
        {
            return Path.Combine(CharacterPath, characterId + ".json");
        }

        private static string BackupFile(string characterId, int generation)
        {
            return Path.Combine(BackupPath, characterId + ".backup" + generation + ".json");
        }

        private static void MoveIfPresent(string source, string destination)
        {
            if (!File.Exists(source)) return;
            if (File.Exists(destination)) File.Delete(destination);
            File.Move(source, destination);
        }

        private static DateTime ParseUtc(string value)
        {
            DateTime parsed;
            return DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static void SetStatus(string message, bool success)
        {
            LastSaveStatus = message;
            LastSaveStatusRealtime = Time.realtimeSinceStartup;
            if (!success) Debug.LogWarning("Arcane Engine 3.1: " + message);
        }
    }
}
