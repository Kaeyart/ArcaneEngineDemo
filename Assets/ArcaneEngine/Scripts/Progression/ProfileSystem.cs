using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ArcaneEngine
{
    [Serializable]
    public sealed class CoreSaveData
    {
        public string instanceId;
        public string coreId;

        public CoreSaveData() { }
        public CoreSaveData(string id)
        {
            instanceId = Guid.NewGuid().ToString("N");
            coreId = id;
        }

        public CoreSaveData CloneForRun()
        {
            return new CoreSaveData(coreId);
        }
    }

    [Serializable]
    public sealed class RelicSaveData
    {
        public string instanceId;
        public string relicId;
        public string sourceCoreId;

        public RelicSaveData() { }
        public RelicSaveData(string evolutionId, string coreId)
        {
            instanceId = Guid.NewGuid().ToString("N");
            relicId = evolutionId;
            sourceCoreId = coreId;
        }
    }

    [Serializable]
    public sealed class AffixRoll
    {
        public string id;
        public float value;
        public int tier;
        public AffixKind kind;
        public string group;
        public float minimumRoll;
        public float maximumRoll;
        public bool crafted;
        public bool locked;
        public bool local;

        public AffixRoll() { }
        public AffixRoll(string affixId, float amount)
        {
            id = affixId;
            value = amount;
        }

        public AffixRoll Clone()
        {
            return new AffixRoll
            {
                id = id,
                value = value,
                tier = tier,
                kind = kind,
                group = group,
                minimumRoll = minimumRoll,
                maximumRoll = maximumRoll,
                crafted = crafted,
                locked = locked,
                local = local
            };
        }
    }

    [Serializable]
    public sealed class ItemSaveData
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
        public int dataVersion;
        public ItemRarity rarity;
        public int quality;
        public int generationSeed;
        public string corruptionId;
        public string craftedAffixId;
        public List<AffixRoll> affixes = new List<AffixRoll>();

        public ItemSaveData() { }
        public ItemSaveData(string id, int level, bool isBanked)
        {
            instanceId = Guid.NewGuid().ToString("N");
            definitionId = id;
            itemLevel = Mathf.Max(1, level);
            banked = isBanked;
        }

        public ItemSaveData Clone()
        {
            ItemSaveData copy = new ItemSaveData
            {
                instanceId = instanceId,
                definitionId = definitionId,
                itemLevel = itemLevel,
                upgradeLevel = upgradeLevel,
                favorite = favorite,
                junk = junk,
                locked = locked,
                corrupted = corrupted,
                banked = banked,
                dataVersion = dataVersion,
                rarity = rarity,
                quality = quality,
                generationSeed = generationSeed,
                corruptionId = corruptionId,
                craftedAffixId = craftedAffixId,
                affixes = affixes == null ? new List<AffixRoll>() : affixes.Select(a => a.Clone()).ToList()
            };
            return copy;
        }
    }

    [Serializable]
    public sealed class SlotItemSave
    {
        public EquipmentSlot slot;
        public string itemInstanceId;
    }

    [Serializable]
    public sealed class PreparedSpellSave
    {
        public int slotIndex;
        public string contentId;
        public bool isRelic;
    }

    [Serializable]
    public sealed class PreparedModifierSave
    {
        public string modifierId;
        public int count;
    }

    [Serializable]
    public sealed class PlacedModifierSave
    {
        public string modifierId;
        public int q;
        public int r;
        public int rotation;
    }

    [Serializable]
    public sealed class SavedSpellLayout
    {
        public string name;
        public string coreId;
        public string relicId;
        public List<PlacedModifierSave> pieces = new List<PlacedModifierSave>();
    }

    [Serializable]
    public sealed class EquipmentLoadoutSave
    {
        public string name;
        public List<SlotItemSave> slots = new List<SlotItemSave>();
    }

    [Serializable]
    public sealed class AccessibilitySettings
    {
        public int visualQuality = 2;
        public float spellEffectDensity = 1f;
        public float environmentDensity = 1f;
        public int dynamicLightQuality = 1;
        public int shadowQuality = 1;
        public float decalDuration = 1f;
        // Legacy save field retained for compatibility. The Standard runtime renderer has
        // no supported distortion pass, so 2.0 does not expose or enable this setting.
        public bool distortion;
        public float hitStop = 0.65f;
        public float damageNumberDensity = 1f;
        public bool colorblindSymbols;
        public bool showVisualDiagnostics;
        public float uiScale = 1f;
        public float effectDensity = 1f;
        public float screenShake = 0.65f;
        public bool reducedFlashes;
        public bool colorblindConnectors;
        public bool autoAim;
        public bool alwaysShowEnemyHealth = true;
        // 0 Always, 1 Damaged, 2 Targeted, 3 Elite/Boss only, 4 Off.
        public int enemyHealthBarMode;
        public bool showEnemyHealthNumbers = true;
        public float enemyHealthBarScale = 1f;
        public float enemyHealthBarVerticalOffset;
        public bool showDamageNumbers = true;
        public bool smoothPlayerTurning = true;
        public float masterVolume = 0.85f;
        public float musicVolume = 0.6f;
        public float effectsVolume = 0.85f;
        public float ambienceVolume = 0.55f;
        public float uiVolume = 0.75f;
        public float enemyVolume = 0.85f;
        public float voiceVolume = 0.8f;
        public float hudScale = 1f;
        public float tooltipScale = 1f;
        public float damageNumberScale = 1f;
        public float hudOpacity = 1f;
        public float safeZone = 1f;
        public float cursorScale = 1f;
        public float mouseSensitivity = 1f;
        public float cameraRotationSensitivity = 1f;
        public bool reducedMotion;
        public bool highContrastTelegraphs;
        public bool highContrastCursor;
        public bool combineRapidDamage = true;
        public bool visualAudioCues = true;
        public bool monoAudio;
        public bool pauseForTutorialText = true;
        public bool simplifiedDescriptions;
        public bool showOffscreenIndicators = true;
        public bool worldRelativeMovement;
        public bool cameraRotationEnabled = true;
        public float dragThreshold = 7f;
        public float doubleClickWindow = 0.32f;
        public bool clickPlacementAlternative = true;
        // 0 Immediate Follow, 1 Smooth Follow, 2 Fixed Camera.
        public int cameraMode = 1;
        public bool cameraObstructionFade = true;
    }

    [Serializable]
    public sealed class RunHistorySave
    {
        public string runInstanceId;
        public string completedUtc;
        public int mode;
        public int seed;
        public bool victory;
        public int roomsCleared;
        public int kills;
        public int runLevel;
        public int essence;
        public int damageDealt;
        public int damageTaken;
        public int criticalHits;
        public int dodges;
        public float bossFightSeconds;
        public int bossPillarsDestroyed;
        public int bossPhasesReached;
        public int objectiveBonusExperience;
        public string topSpell;
        public string deathSource;
        public List<string> finalSpellNames = new List<string>();
    }

    [Serializable]
    public sealed class BuildHistorySave
    {
        public string stableId;
        public string discoveredUtc;
        public string name;
        public string coreId;
        public string element;
        public string delivery;
        public int activeModifiers;
        public int triggers;
        public float estimatedDps;
    }

    [Serializable]
    public sealed class TransactionSave
    {
        public string utc;
        public string category;
        public string contentId;
        public int amount;
        public string note;
    }

    [Serializable]
    public sealed class PlaytestFeedbackSave
    {
        public string utc;
        public int seed;
        public int rating;
        public string tag;
    }

    [Serializable]
    public sealed class RunSpellSnapshot
    {
        public int slotIndex;
        public CoreSaveData activeSpell;
        public bool canBeKept;
        public string baseSpellId;
        public string legendarySpellId;
        public int boardRadiusBonus;
        public int spellLevel = 1;
        public List<PlacedModifierSave> modifiers = new List<PlacedModifierSave>();
    }

    [Serializable]
    public sealed class RunEquippedItemSnapshot
    {
        public EquipmentSlot slot;
        public ItemSaveData item;
    }

    [Serializable]
    public sealed class RunPerkSaveData
    {
        public int perk;
        public int rank;
    }

    [Serializable]
    public sealed class RunSnapshotData
    {
        public const int CurrentVersion = 10;
        public int version = CurrentVersion;
        public string savedUtc;
        public string runInstanceId;
        public int runSeed;
        public int roomIndex;
        public int totalRooms;
        public int kills;
        public int gold;
        public int forgeDust;
        public int bindingRunes;
        public int corruptionCores;
        public int rewardRerolls;
        public int roomsWithoutModifier;
        public int roomsWithoutEquipment;
        public int roomsWithoutSpell;
        public float temporaryPowerBonus;
        public float shopDiscount;
        public int runMode;
        public int runLevel = 1;
        public int runExperience;
        public int damageDealt;
        public int damageTaken;
        public int criticalHits;
        public int dodges;
        public int roomsCleared;
        public int objectiveBonusExperience;
        public List<RunPerkSaveData> runPerks = new List<RunPerkSaveData>();
        public List<string> routeHistory = new List<string>();
        public List<string> usedRoomServices = new List<string>();
        public List<string> purchasedShopOffers = new List<string>();
        public List<string> claimedPermanentRewards = new List<string>();
        public List<RewardOffer> rejectedRewards = new List<RewardOffer>();
        public string lastDamageSource;
        public float healthRatio = 1f;
        public float manaRatio = 1f;
        public string roomId;
        // v9: deterministic checksum of every saved input that can rebuild a
        // spell, equipment mutation, Spell Link, biome and room presentation.
        public string visualReconstructionSignature;
        public DifficultySettings difficulty = new DifficultySettings();
        public List<RunSpellSnapshot> spells = new List<RunSpellSnapshot>();
        public List<PreparedModifierSave> ownedModifiers = new List<PreparedModifierSave>();
        public List<CoreSaveData> storedSpells = new List<CoreSaveData>();
        public List<string> keepableSpellInstanceIds = new List<string>();
        public List<ItemSaveData> backpack = new List<ItemSaveData>();
        // v8: explicit name for the temporary ownership domain. `backpack` is
        // retained as a migration source for older checkpoints only.
        public List<ItemSaveData> unsecuredItems = new List<ItemSaveData>();
        public List<RunEquippedItemSnapshot> equippedItems = new List<RunEquippedItemSnapshot>();
        public int linkSlots = 1;
        public List<int> ownedLinkConditions = new List<int>();
        public List<SpellLinkSave> spellLinks = new List<SpellLinkSave>();

        public void Normalize()
        {
            if (difficulty == null) difficulty = new DifficultySettings();
            if (spells == null) spells = new List<RunSpellSnapshot>();
            if (ownedModifiers == null) ownedModifiers = new List<PreparedModifierSave>();
            if (storedSpells == null) storedSpells = new List<CoreSaveData>();
            if (keepableSpellInstanceIds == null) keepableSpellInstanceIds = new List<string>();
            if (backpack == null) backpack = new List<ItemSaveData>();
            if (unsecuredItems == null) unsecuredItems = new List<ItemSaveData>();
            if (version < 8 && unsecuredItems.Count == 0 && backpack.Count > 0)
                unsecuredItems = backpack.Where(item => item != null && !item.banked).Select(item => item.Clone()).ToList();
            if (equippedItems == null) equippedItems = new List<RunEquippedItemSnapshot>();
            if (ownedLinkConditions == null) ownedLinkConditions = new List<int>();
            if (spellLinks == null) spellLinks = new List<SpellLinkSave>();
            if (runPerks == null) runPerks = new List<RunPerkSaveData>();
            if (routeHistory == null) routeHistory = new List<string>();
            if (usedRoomServices == null) usedRoomServices = new List<string>();
            if (purchasedShopOffers == null) purchasedShopOffers = new List<string>();
            if (claimedPermanentRewards == null) claimedPermanentRewards = new List<string>();
            if (rejectedRewards == null) rejectedRewards = new List<RewardOffer>();
            healthRatio = Mathf.Clamp01(healthRatio);
            manaRatio = Mathf.Clamp01(manaRatio);
            linkSlots = Mathf.Clamp(linkSlots <= 0 ? 1 : linkSlots, 1, 3);
            ownedLinkConditions.RemoveAll(value => !Enum.IsDefined(typeof(SpellLinkCondition), value));
            spellLinks.RemoveAll(link => link == null || link.sourceSlot < 0 || link.sourceSlot > 2 ||
                link.destinationSlot < 0 || link.destinationSlot > 2 || link.sourceSlot == link.destinationSlot);
            foreach (RunSpellSnapshot spell in spells) if (spell != null) spell.spellLevel = Mathf.Clamp(spell.spellLevel <= 0 ? 1 : spell.spellLevel, 1, 5);
        }
    }

    [Serializable]
    public sealed class ControlSettings
    {
        public KeyCode moveForward = KeyCode.W;
        public KeyCode moveBack = KeyCode.S;
        public KeyCode moveLeft = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;
        public KeyCode spellSlot3 = KeyCode.Q;
        public KeyCode dodge = KeyCode.Space;
        public KeyCode interact = KeyCode.E;
        public KeyCode workshop = KeyCode.Tab;
        public KeyCode spellLinks = KeyCode.L;
        public KeyCode inventory = KeyCode.I;
        public KeyCode help = KeyCode.F1;
        public KeyCode map = KeyCode.M;
        public KeyCode cancelCast = KeyCode.LeftControl;
        public int primaryMouseButton;
        public int secondaryMouseButton = 1;
        public bool holdToCharge = true;
        public float controllerMoveDeadZone = 0.18f;
        public float controllerAimDeadZone = 0.22f;
        public float controllerAimSensitivity = 1f;
        public float controllerAimAssist = 0.35f;
        public float controllerVibration = 0.65f;
        public bool controllerVibrationEnabled = true;
    }

    [Serializable]
    public sealed class ProfileData
    {
        public const int CurrentVersion = 11;
        public int version = CurrentVersion;
        public string stableProfileId = Guid.NewGuid().ToString("N");
        public string profileName = "Player";
        public string createdUtc;
        public string lastSavedUtc;
        public long totalPlaySeconds;
        public int essence;
        public int relicShards;
        public int forgeDust;
        public int bindingRunes;
        public int corruptionCores;
        public int healthRank;
        public int manaRank;
        public int powerRank;
        public int startingSpellRank;
        public int preparationRank;
        public int rewardRerollRank;
        public int archiveToolRank;
        public int completedRuns;
        public int failedRuns;
        public int bossKills;
        public int bestRoom;
        public int totalKills;
        public bool tutorialCompleted;
        public bool endlessUnlocked;
        public int highestRunLevel;
        public int dailyChallengeBest;
        public string lastDailyChallengeDate;
        public bool autoCollectGold = true;
        public bool autoCollectEssence = true;
        public int minimumLootRarity;
        public int lootSlotFilter = -1;
        public string lootTagFilter = string.Empty;
        public bool firstLaunchComplete;
        public string lastSuccessfulSaveUtc;
        public List<string> completedTutorialTopics = new List<string>();
        public List<string> dismissedContextHints = new List<string>();
        public List<string> claimedBossRewardIds = new List<string>();
        public List<RunHistorySave> runHistory = new List<RunHistorySave>();
        public List<BuildHistorySave> buildHistory = new List<BuildHistorySave>();
        public List<TransactionSave> transactionHistory = new List<TransactionSave>();
        public List<PlaytestFeedbackSave> playtestFeedback = new List<PlaytestFeedbackSave>();
        public List<string> unlockedCoreIds = new List<string>();
        public List<string> unlockedModifierIds = new List<string>();
        public List<int> unlockedLinkConditionIds = new List<int>();
        public List<string> discoveredRelicIds = new List<string>();
        public List<string> codexEntries = new List<string>();
        public List<CoreSaveData> spellArchive = new List<CoreSaveData>();
        public List<RelicSaveData> relicArchive = new List<RelicSaveData>();
        public List<ItemSaveData> armory = new List<ItemSaveData>();
        public List<SlotItemSave> equippedItems = new List<SlotItemSave>();
        public List<PreparedSpellSave> preparedSpells = new List<PreparedSpellSave>();
        public List<PreparedModifierSave> preparedModifiers = new List<PreparedModifierSave>();
        public List<SavedSpellLayout> savedSpellLayouts = new List<SavedSpellLayout>();
        public List<SavedSpellLayout> retiredSpellLayouts = new List<SavedSpellLayout>();
        public List<EquipmentLoadoutSave> equipmentLoadouts = new List<EquipmentLoadoutSave>();
        public AccessibilitySettings accessibility = new AccessibilitySettings();
        public ControlSettings controls = new ControlSettings();
        public int legacySpellLinkCredits;

        public int StartingSpellSlots { get { return Mathf.Clamp(1 + startingSpellRank, 1, 3); } }
        public int PreparationBudget { get { return 4 + preparationRank * 3; } }
        public int RewardRerolls { get { return rewardRerollRank; } }

        public void Normalize()
        {
            int loadedVersion = version;
            version = CurrentVersion;
            if (string.IsNullOrEmpty(stableProfileId)) stableProfileId = Guid.NewGuid().ToString("N");
            if (string.IsNullOrEmpty(profileName)) profileName = "Player";
            if (string.IsNullOrEmpty(createdUtc)) createdUtc = DateTime.UtcNow.ToString("o");
            if (unlockedCoreIds == null) unlockedCoreIds = new List<string>();
            if (unlockedModifierIds == null) unlockedModifierIds = new List<string>();
            if (unlockedLinkConditionIds == null) unlockedLinkConditionIds = new List<int>();
            if (discoveredRelicIds == null) discoveredRelicIds = new List<string>();
            if (codexEntries == null) codexEntries = new List<string>();
            if (spellArchive == null) spellArchive = new List<CoreSaveData>();
            if (relicArchive == null) relicArchive = new List<RelicSaveData>();
            if (armory == null) armory = new List<ItemSaveData>();
            if (equippedItems == null) equippedItems = new List<SlotItemSave>();
            if (preparedSpells == null) preparedSpells = new List<PreparedSpellSave>();
            if (preparedModifiers == null) preparedModifiers = new List<PreparedModifierSave>();
            if (savedSpellLayouts == null) savedSpellLayouts = new List<SavedSpellLayout>();
            if (retiredSpellLayouts == null) retiredSpellLayouts = new List<SavedSpellLayout>();
            if (equipmentLoadouts == null) equipmentLoadouts = new List<EquipmentLoadoutSave>();
            if (accessibility == null) accessibility = new AccessibilitySettings();
            if (controls == null) controls = new ControlSettings();
            if (completedTutorialTopics == null) completedTutorialTopics = new List<string>();
            if (dismissedContextHints == null) dismissedContextHints = new List<string>();
            if (claimedBossRewardIds == null) claimedBossRewardIds = new List<string>();
            if (runHistory == null) runHistory = new List<RunHistorySave>();
            if (buildHistory == null) buildHistory = new List<BuildHistorySave>();
            if (transactionHistory == null) transactionHistory = new List<TransactionSave>();
            if (playtestFeedback == null) playtestFeedback = new List<PlaytestFeedbackSave>();
            if (loadedVersion < 4)
            {
                autoCollectGold = true;
                autoCollectEssence = true;
                if (accessibility.masterVolume <= 0f && accessibility.musicVolume <= 0f && accessibility.effectsVolume <= 0f)
                {
                    accessibility.masterVolume = 0.85f; accessibility.musicVolume = 0.6f; accessibility.effectsVolume = 0.85f;
                    accessibility.ambienceVolume = 0.55f; accessibility.uiVolume = 0.75f;
                }
            }
            if (loadedVersion < 5)
            {
                accessibility.hudScale = accessibility.hudScale <= 0f ? 1f : accessibility.hudScale;
                accessibility.tooltipScale = accessibility.tooltipScale <= 0f ? 1f : accessibility.tooltipScale;
                accessibility.damageNumberScale = accessibility.damageNumberScale <= 0f ? 1f : accessibility.damageNumberScale;
                accessibility.hudOpacity = accessibility.hudOpacity <= 0f ? 1f : accessibility.hudOpacity;
                accessibility.safeZone = accessibility.safeZone <= 0f ? 1f : accessibility.safeZone;
                accessibility.cursorScale = accessibility.cursorScale <= 0f ? 1f : accessibility.cursorScale;
                accessibility.mouseSensitivity = accessibility.mouseSensitivity <= 0f ? 1f : accessibility.mouseSensitivity;
                accessibility.cameraRotationSensitivity = accessibility.cameraRotationSensitivity <= 0f ? 1f : accessibility.cameraRotationSensitivity;
                accessibility.enemyVolume = accessibility.enemyVolume <= 0f ? 0.85f : accessibility.enemyVolume;
                accessibility.voiceVolume = accessibility.voiceVolume <= 0f ? 0.8f : accessibility.voiceVolume;
            }
            if (loadedVersion < 7)
            {
                accessibility.dragThreshold = 7f;
                accessibility.doubleClickWindow = 0.32f;
                accessibility.clickPlacementAlternative = true;
            }
            if (loadedVersion < 8)
            {
                Dictionary<string, SpellLinkCondition> legacyConditions = new Dictionary<string, SpellLinkCondition>
                {
                    { "trigger_slot2", SpellLinkCondition.OnHit },
                    { "trigger_slot1", SpellLinkCondition.OnKill },
                    { "trigger_slot3", SpellLinkCondition.OnCast },
                    { "trigger_expire", SpellLinkCondition.OnExpire }
                };
                string[] retiredIds = { "trigger_slot2", "trigger_slot1", "trigger_slot3", "trigger_expire", "target_player", "target_enemy", "target_impact" };
                int retiredTargetUnlocks = new[] { "target_player", "target_enemy", "target_impact" }.Count(unlockedModifierIds.Contains);
                foreach (KeyValuePair<string, SpellLinkCondition> pair in legacyConditions)
                    if (unlockedModifierIds.Contains(pair.Key) && !unlockedLinkConditionIds.Contains((int)pair.Value))
                        unlockedLinkConditionIds.Add((int)pair.Value);
                legacySpellLinkCredits += preparedModifiers.Where(value => value != null && retiredIds.Contains(value.modifierId)).Sum(value => Mathf.Max(0, value.count));
                preparedModifiers.RemoveAll(value => value != null && retiredIds.Contains(value.modifierId));
                unlockedModifierIds.RemoveAll(retiredIds.Contains);
                essence += retiredTargetUnlocks * 18;
                foreach (SavedSpellLayout layout in savedSpellLayouts.Where(value => value != null && value.pieces != null && value.pieces.Any(piece => retiredIds.Contains(piece.modifierId))).ToArray())
                {
                    SavedSpellLayout archived = new SavedSpellLayout { name = layout.name + " (Pre-1.4 archive)", coreId = layout.coreId, relicId = layout.relicId };
                    archived.pieces = layout.pieces.Select(piece => new PlacedModifierSave { modifierId = piece.modifierId, q = piece.q, r = piece.r, rotation = piece.rotation }).ToList();
                    retiredSpellLayouts.Add(archived);
                    layout.pieces.RemoveAll(piece => retiredIds.Contains(piece.modifierId));
                }
                controls.spellLinks = KeyCode.L;
            }
            if (loadedVersion < 9)
            {
                forgeDust = Mathf.Max(0, forgeDust);
                bindingRunes = Mathf.Max(0, bindingRunes);
                corruptionCores = Mathf.Max(0, corruptionCores);
            }
            if (loadedVersion < 10)
            {
                accessibility.visualQuality = 2;
                accessibility.spellEffectDensity = accessibility.effectDensity <= 0f ? 1f : accessibility.effectDensity;
                accessibility.environmentDensity = 1f;
                accessibility.dynamicLightQuality = 1;
                accessibility.shadowQuality = 1;
                accessibility.decalDuration = 1f;
                accessibility.distortion = false;
                accessibility.hitStop = 0.65f;
                accessibility.damageNumberDensity = 1f;
            }
            forgeDust = Mathf.Max(0, forgeDust);
            bindingRunes = Mathf.Max(0, bindingRunes);
            corruptionCores = Mathf.Max(0, corruptionCores);
            accessibility.enemyHealthBarScale = accessibility.enemyHealthBarScale <= 0f ? 1f : accessibility.enemyHealthBarScale;
            accessibility.uiScale = Mathf.Clamp(accessibility.uiScale <= 0f ? 1f : accessibility.uiScale, 0.85f, 1.3f);
            accessibility.hudScale = Mathf.Clamp(accessibility.hudScale, 0.75f, 1.4f);
            accessibility.tooltipScale = Mathf.Clamp(accessibility.tooltipScale, 0.8f, 1.5f);
            accessibility.damageNumberScale = Mathf.Clamp(accessibility.damageNumberScale, 0.7f, 1.6f);
            accessibility.hudOpacity = Mathf.Clamp(accessibility.hudOpacity, 0.35f, 1f);
            accessibility.safeZone = Mathf.Clamp(accessibility.safeZone, 0.8f, 1f);
            accessibility.cursorScale = Mathf.Clamp(accessibility.cursorScale, 0.7f, 1.8f);
            accessibility.mouseSensitivity = Mathf.Clamp(accessibility.mouseSensitivity, 0.25f, 2.5f);
            accessibility.cameraRotationSensitivity = Mathf.Clamp(accessibility.cameraRotationSensitivity, 0.25f, 2.5f);
            accessibility.enemyHealthBarScale = Mathf.Clamp(accessibility.enemyHealthBarScale, 0.6f, 1.8f);
            accessibility.enemyHealthBarVerticalOffset = Mathf.Clamp(accessibility.enemyHealthBarVerticalOffset, -0.75f, 1.5f);
            accessibility.dragThreshold = Mathf.Clamp(accessibility.dragThreshold <= 0f ? 7f : accessibility.dragThreshold, 3f, 24f);
            accessibility.doubleClickWindow = Mathf.Clamp(accessibility.doubleClickWindow <= 0f ? 0.32f : accessibility.doubleClickWindow, 0.18f, 0.75f);
            accessibility.visualQuality = Mathf.Clamp(accessibility.visualQuality, 0, 2);
            accessibility.spellEffectDensity = Mathf.Clamp(accessibility.spellEffectDensity <= 0f ? accessibility.effectDensity : accessibility.spellEffectDensity, 0.25f, 1f);
            accessibility.environmentDensity = Mathf.Clamp(accessibility.environmentDensity <= 0f ? 1f : accessibility.environmentDensity, 0.2f, 1f);
            accessibility.dynamicLightQuality = Mathf.Clamp(accessibility.dynamicLightQuality, 0, 2);
            accessibility.shadowQuality = Mathf.Clamp(accessibility.shadowQuality, 0, 2);
            accessibility.decalDuration = Mathf.Clamp(accessibility.decalDuration <= 0f ? 1f : accessibility.decalDuration, 0.25f, 2f);
            accessibility.distortion = false;
            accessibility.hitStop = Mathf.Clamp01(accessibility.hitStop);
            accessibility.damageNumberDensity = Mathf.Clamp(accessibility.damageNumberDensity <= 0f ? 1f : accessibility.damageNumberDensity, 0.25f, 1f);
            controls.controllerMoveDeadZone = Mathf.Clamp(controls.controllerMoveDeadZone <= 0f ? 0.18f : controls.controllerMoveDeadZone, 0.05f, 0.5f);
            controls.controllerAimDeadZone = Mathf.Clamp(controls.controllerAimDeadZone <= 0f ? 0.22f : controls.controllerAimDeadZone, 0.05f, 0.5f);
            controls.controllerAimSensitivity = Mathf.Clamp(controls.controllerAimSensitivity <= 0f ? 1f : controls.controllerAimSensitivity, 0.25f, 2.5f);
            controls.controllerAimAssist = Mathf.Clamp01(controls.controllerAimAssist);
            controls.controllerVibration = Mathf.Clamp01(controls.controllerVibration);
            minimumLootRarity = Mathf.Clamp(minimumLootRarity, 0, 3);
            lootSlotFilter = Mathf.Clamp(lootSlotFilter, -1, Enum.GetValues(typeof(EquipmentSlot)).Length - 1);
            if (lootTagFilter == null) lootTagFilter = string.Empty;
            highestRunLevel = Mathf.Max(1, highestRunLevel);
            startingSpellRank = Mathf.Clamp(startingSpellRank, 0, 2);
            preparationRank = Mathf.Max(0, preparationRank);
            preparedSpells.RemoveAll(s => s == null || s.slotIndex < 0 || s.slotIndex > 2 || string.IsNullOrEmpty(s.contentId));
            preparedModifiers.RemoveAll(m => m == null || m.count <= 0 || string.IsNullOrEmpty(m.modifierId));
            unlockedLinkConditionIds.RemoveAll(value => !Enum.IsDefined(typeof(SpellLinkCondition), value));
        }
    }

    public static class ProfileManager
    {
        private const string FolderName = "ArcaneEngineProfiles";
        private const int BackupCount = 3;
        private static ProfileData _current;
        private static int _activeIndex;
        private static bool _saving;

        public static string LastSaveStatus { get; private set; } = "Not saved yet";
        public static string ProfileFolderPath { get { return ProfileFolder; } }
        public static bool HasRunSnapshot { get { return File.Exists(RunPath(_activeIndex)); } }

        public static ProfileData Current
        {
            get
            {
                if (_current == null) Load(0);
                return _current;
            }
        }

        public static int ActiveIndex { get { return _activeIndex; } }

        public static void Load(int profileIndex)
        {
            _activeIndex = Mathf.Clamp(profileIndex, 0, 2);
            Directory.CreateDirectory(ProfileFolder);
            string path = ProfilePath(_activeIndex);
            string error;
            _current = TryReadJson<ProfileData>(path, out error);

            if (_current == null && File.Exists(path))
            {
                Debug.LogWarning("Profile could not be read; attempting backups. " + error);
                Quarantine(path);
            }
            if (_current == null) _current = LoadBackup(_activeIndex);

            if (_current == null)
            {
                _current = CreateDefaultProfile();
                MigrateLegacyPlayerPrefs(_current);
                Save();
            }
            _current.Normalize();
            LastSaveStatus = "Loaded profile " + (_activeIndex + 1);
        }

        public static void Save()
        {
            if (_current == null || _saving) return;
            _saving = true;
            string path = null;
            string temporary = null;
            try
            {
                _current.Normalize();
                Directory.CreateDirectory(ProfileFolder);
                path = ProfilePath(_activeIndex);
                temporary = path + ".tmp";
                _current.lastSavedUtc = DateTime.UtcNow.ToString("o");
                _current.lastSuccessfulSaveUtc = _current.lastSavedUtc;
                string json = JsonUtility.ToJson(_current, true);
                WriteAllTextDurable(temporary, json);
                ProfileData validation = JsonUtility.FromJson<ProfileData>(File.ReadAllText(temporary));
                if (validation == null) throw new InvalidDataException("Temporary profile did not validate.");
                RotateBackups(path);
                CommitTemporary(temporary, path);
                WriteAllTextDurable(path + ".sha256", ComputeHash(json));
                LastSaveStatus = "Saved " + DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception exception)
            {
                if (!string.IsNullOrEmpty(temporary) && File.Exists(temporary)) File.Delete(temporary);
                LastSaveStatus = "SAVE FAILED: " + exception.Message;
                Debug.LogError("Arcane Engine profile save failed: " + exception.Message);
            }
            finally { _saving = false; }
        }

        public static bool SaveRunSnapshot(RunSnapshotData snapshot)
        {
            if (snapshot == null) return false;
            snapshot.version = RunSnapshotData.CurrentVersion;
            snapshot.savedUtc = DateTime.UtcNow.ToString("o");
            snapshot.Normalize();
            string path = RunPath(_activeIndex);
            string temporary = path + ".tmp";
            try
            {
                Directory.CreateDirectory(ProfileFolder);
                string json = JsonUtility.ToJson(snapshot, true);
                WriteAllTextDurable(temporary, json);
                RunSnapshotData validation = JsonUtility.FromJson<RunSnapshotData>(File.ReadAllText(temporary));
                if (validation == null || validation.version != RunSnapshotData.CurrentVersion)
                    throw new InvalidDataException("Run snapshot did not validate.");
                if (File.Exists(path)) File.Copy(path, path + ".bak", true);
                if (File.Exists(path + ".sha256")) File.Copy(path + ".sha256", path + ".bak.sha256", true);
                CommitTemporary(temporary, path);
                WriteAllTextDurable(path + ".sha256", ComputeHash(json));
                LastSaveStatus = "Run saved " + DateTime.Now.ToString("HH:mm:ss");
                return true;
            }
            catch (Exception exception)
            {
                if (File.Exists(temporary)) File.Delete(temporary);
                LastSaveStatus = "RUN SAVE FAILED: " + exception.Message;
                Debug.LogError(LastSaveStatus);
                return false;
            }
        }

        public static RunSnapshotData LoadRunSnapshot()
        {
            string error;
            string primary = RunPath(_activeIndex);
            RunSnapshotData snapshot = TryReadJson<RunSnapshotData>(primary, out error);
            if (snapshot == null && File.Exists(primary)) Quarantine(primary);
            if (snapshot == null)
                snapshot = TryReadJson<RunSnapshotData>(primary + ".bak", out error);
            if (snapshot == null || snapshot.version > RunSnapshotData.CurrentVersion) return null;
            snapshot.Normalize();
            return snapshot;
        }

        public static void DeleteRunSnapshot()
        {
            foreach (string suffix in new[] { string.Empty, ".bak", ".bak.sha256", ".tmp", ".sha256" })
            {
                string path = RunPath(_activeIndex) + suffix;
                if (File.Exists(path)) File.Delete(path);
            }
        }

        public static void SwitchProfile(int profileIndex)
        {
            Save();
            _current = null;
            Load(profileIndex);
        }

        public static void ResetCurrent()
        {
            string path = ProfilePath(_activeIndex);
            if (File.Exists(path)) File.Delete(path);
            for (int i = 1; i <= BackupCount; i++) if (File.Exists(path + ".bak" + i)) File.Delete(path + ".bak" + i);
            if (File.Exists(path + ".bak")) File.Delete(path + ".bak");
            if (File.Exists(path + ".sha256")) File.Delete(path + ".sha256");
            DeleteRunSnapshot();
            _current = CreateDefaultProfile();
            Save();
        }

        public static void Discover(string stableId)
        {
            if (string.IsNullOrEmpty(stableId) || Current.codexEntries.Contains(stableId)) return;
            Current.codexEntries.Add(stableId);
            Save();
        }

        public static void RecordTransaction(string category, string contentId, int amount, string note)
        {
            if (Current.transactionHistory == null) Current.transactionHistory = new List<TransactionSave>();
            Current.transactionHistory.Add(new TransactionSave
            {
                utc = DateTime.UtcNow.ToString("o"), category = category ?? "Unknown", contentId = contentId ?? string.Empty,
                amount = amount, note = note ?? string.Empty
            });
            while (Current.transactionHistory.Count > 100) Current.transactionHistory.RemoveAt(0);
        }

        public static void RecordRun(RunHistorySave run)
        {
            if (run == null) return;
            Current.runHistory.Add(run);
            while (Current.runHistory.Count > 30) Current.runHistory.RemoveAt(0);
        }

        public static void RecordBuild(BuildHistorySave build)
        {
            if (build == null || string.IsNullOrEmpty(build.stableId)) return;
            BuildHistorySave existing = Current.buildHistory.FirstOrDefault(value => value != null && value.stableId == build.stableId);
            if (existing != null) Current.buildHistory.Remove(existing);
            Current.buildHistory.Add(build);
            while (Current.buildHistory.Count > 80) Current.buildHistory.RemoveAt(0);
        }

        public static void RecordFeedback(int seed, int rating, string tag)
        {
            Current.playtestFeedback.Add(new PlaytestFeedbackSave
            {
                utc = DateTime.UtcNow.ToString("o"), seed = seed, rating = Mathf.Clamp(rating, 1, 5), tag = tag ?? string.Empty
            });
            while (Current.playtestFeedback.Count > 60) Current.playtestFeedback.RemoveAt(0);
            Save();
        }

        public static bool ExportCurrentProfile(string destinationDirectory, out string exportedPath, out string message)
        {
            exportedPath = string.Empty;
            message = string.Empty;
            try
            {
                Save();
                if (string.IsNullOrEmpty(destinationDirectory)) destinationDirectory = Path.Combine(ProfileFolder, "Exports");
                Directory.CreateDirectory(destinationDirectory);
                string safeName = string.Concat((Current.profileName ?? "Player").Select(value =>
                    Path.GetInvalidFileNameChars().Contains(value) ? '_' : value));
                exportedPath = Path.Combine(destinationDirectory,
                    safeName + "_profile_" + (_activeIndex + 1) + "_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".arcaneprofile");
                File.Copy(ProfilePath(_activeIndex), exportedPath, false);
                File.Copy(ProfilePath(_activeIndex) + ".sha256", exportedPath + ".sha256", true);
                message = "Profile exported to " + exportedPath;
                return true;
            }
            catch (Exception exception) { message = "Export failed: " + exception.Message; return false; }
        }

        public static bool ImportProfile(string sourcePath, int targetIndex, out string message)
        {
            message = string.Empty;
            targetIndex = Mathf.Clamp(targetIndex, 0, 2);
            try
            {
                if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath)) { message = "Import file was not found."; return false; }
                string json = File.ReadAllText(sourcePath);
                if (File.Exists(sourcePath + ".sha256") && !string.Equals(File.ReadAllText(sourcePath + ".sha256").Trim(), ComputeHash(json), StringComparison.OrdinalIgnoreCase))
                { message = "Import rejected: the profile checksum does not match."; return false; }
                ProfileData imported = JsonUtility.FromJson<ProfileData>(json);
                if (imported == null || imported.version > ProfileData.CurrentVersion)
                { message = "The profile is invalid or was made by a newer unsupported version."; return false; }
                imported.Normalize();
                string target = ProfilePath(targetIndex);
                Directory.CreateDirectory(ProfileFolder);
                if (File.Exists(target)) File.Copy(target, target + ".pre_import_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"), false);
                string temporary = target + ".import.tmp";
                WriteAllTextDurable(temporary, JsonUtility.ToJson(imported, true));
                CommitTemporary(temporary, target);
                WriteAllTextDurable(target + ".sha256", ComputeHash(File.ReadAllText(target)));
                if (targetIndex == _activeIndex) { _current = imported; Save(); }
                message = "Imported profile into slot " + (targetIndex + 1) + ".";
                return true;
            }
            catch (Exception exception) { message = "Import failed: " + exception.Message; return false; }
        }

        public static bool DuplicateProfile(int sourceIndex, int targetIndex, out string message)
        {
            sourceIndex = Mathf.Clamp(sourceIndex, 0, 2);
            targetIndex = Mathf.Clamp(targetIndex, 0, 2);
            if (sourceIndex == targetIndex) { message = "Choose a different destination slot."; return false; }
            string source = ProfilePath(sourceIndex);
            if (!File.Exists(source)) { message = "The source profile does not exist."; return false; }
            string temporaryExport = Path.Combine(ProfileFolder, "duplicate_" + Guid.NewGuid().ToString("N") + ".arcaneprofile");
            try
            {
                File.Copy(source, temporaryExport, false);
                bool result = ImportProfile(temporaryExport, targetIndex, out message);
                if (result)
                {
                    ProfileData duplicated = TryReadJson<ProfileData>(ProfilePath(targetIndex), out message);
                    if (duplicated != null)
                    {
                        duplicated.stableProfileId = Guid.NewGuid().ToString("N");
                        duplicated.profileName = (duplicated.profileName ?? "Player") + " Copy";
                        WriteAllTextDurable(ProfilePath(targetIndex), JsonUtility.ToJson(duplicated, true));
                        WriteAllTextDurable(ProfilePath(targetIndex) + ".sha256", ComputeHash(File.ReadAllText(ProfilePath(targetIndex))));
                        if (targetIndex == _activeIndex) _current = duplicated;
                    }
                    message = "Profile duplicated into slot " + (targetIndex + 1) + ".";
                }
                return result;
            }
            finally { if (File.Exists(temporaryExport)) File.Delete(temporaryExport); }
        }

        public static string[] AvailableBackups(int profileIndex)
        {
            profileIndex = Mathf.Clamp(profileIndex, 0, 2);
            string path = ProfilePath(profileIndex);
            List<string> result = new List<string>();
            foreach (string candidate in Directory.Exists(ProfileFolder)
                ? Directory.GetFiles(ProfileFolder, Path.GetFileName(path) + ".bak*") : new string[0])
                if (!candidate.EndsWith(".sha256", StringComparison.OrdinalIgnoreCase)) result.Add(candidate);
            return result.OrderByDescending(File.GetLastWriteTimeUtc).ToArray();
        }

        public static bool RestoreBackup(int profileIndex, string backupPath, out string message)
        {
            message = string.Empty;
            profileIndex = Mathf.Clamp(profileIndex, 0, 2);
            try
            {
                string allowedRoot = Path.GetFullPath(ProfileFolder) + Path.DirectorySeparatorChar;
                string fullBackup = Path.GetFullPath(backupPath ?? string.Empty);
                if (!fullBackup.StartsWith(allowedRoot, StringComparison.Ordinal) || !AvailableBackups(profileIndex).Contains(fullBackup))
                { message = "That backup does not belong to the selected profile."; return false; }
                string error;
                ProfileData backup = TryReadJson<ProfileData>(fullBackup, out error);
                if (backup == null) { message = "Backup validation failed: " + error; return false; }
                string target = ProfilePath(profileIndex);
                if (File.Exists(target)) File.Copy(target, target + ".before_restore_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"), false);
                string temporary = target + ".restore.tmp";
                WriteAllTextDurable(temporary, JsonUtility.ToJson(backup, true));
                CommitTemporary(temporary, target);
                WriteAllTextDurable(target + ".sha256", ComputeHash(File.ReadAllText(target)));
                if (profileIndex == _activeIndex) { _current = backup; _current.Normalize(); }
                message = "Restored backup from " + File.GetLastWriteTime(backupPath).ToString("g") + ".";
                return true;
            }
            catch (Exception exception) { message = "Restore failed: " + exception.Message; return false; }
        }

        public static bool DeleteProfileWithConfirmation(int profileIndex, string typedConfirmation, out string message)
        {
            profileIndex = Mathf.Clamp(profileIndex, 0, 2);
            string required = "DELETE PROFILE " + (profileIndex + 1);
            if (!string.Equals((typedConfirmation ?? string.Empty).Trim(), required, StringComparison.Ordinal))
            { message = "Type " + required + " exactly."; return false; }
            foreach (string path in Directory.GetFiles(ProfileFolder, "profile_" + profileIndex + ".json*"))
                File.Delete(path);
            foreach (string path in Directory.GetFiles(ProfileFolder, "run_" + profileIndex + ".json*"))
                File.Delete(path);
            if (profileIndex == _activeIndex) { _current = CreateDefaultProfile(); Save(); }
            message = "Profile " + (profileIndex + 1) + " deleted.";
            return true;
        }

        public static bool TryClaimPermanentReward(string stableId)
        {
            if (string.IsNullOrEmpty(stableId) || Current.claimedBossRewardIds.Contains(stableId)) return false;
            Current.claimedBossRewardIds.Add(stableId);
            while (Current.claimedBossRewardIds.Count > 500) Current.claimedBossRewardIds.RemoveAt(0);
            return true;
        }

        public static bool SpendEssence(int amount)
        {
            if (amount < 0 || Current.essence < amount) return false;
            Current.essence -= amount;
            Save();
            return true;
        }

        public static bool SpendRelicShards(int amount)
        {
            if (amount < 0 || Current.relicShards < amount) return false;
            Current.relicShards -= amount;
            Save();
            return true;
        }

        public static CoreSaveData FindCore(string instanceId)
        {
            return Current.spellArchive.FirstOrDefault(c => c.instanceId == instanceId);
        }

        public static ItemSaveData FindItem(string instanceId)
        {
            return Current.armory.FirstOrDefault(i => i.instanceId == instanceId);
        }

        public static void SaveEquipped(Dictionary<EquipmentSlot, ItemInstance> equipped, bool persist = true)
        {
            Current.equippedItems.Clear();
            foreach (KeyValuePair<EquipmentSlot, ItemInstance> pair in equipped)
            {
                if (pair.Value == null || !pair.Value.banked) continue;
                Current.equippedItems.Add(new SlotItemSave { slot = pair.Key, itemInstanceId = pair.Value.instanceId });
            }
            if (persist) Save();
        }

        public static bool TrySpendForgeMaterials(ForgeCost cost, out string message)
        {
            if (Current.forgeDust < cost.dust || Current.bindingRunes < cost.runes || Current.corruptionCores < cost.cores)
            {
                message = "Requires " + PermanentForgeCostDescription(cost) + ".";
                return false;
            }
            Current.forgeDust -= Mathf.Max(0, cost.dust);
            Current.bindingRunes -= Mathf.Max(0, cost.runes);
            Current.corruptionCores -= Mathf.Max(0, cost.cores);
            message = string.Empty;
            return true;
        }

        public static void RefundForgeMaterials(ForgeCost cost)
        {
            Current.forgeDust += Mathf.Max(0, cost.dust);
            Current.bindingRunes += Mathf.Max(0, cost.runes);
            Current.corruptionCores += Mathf.Max(0, cost.cores);
        }

        public static void SecureForgeMaterials(int dust, int runes, int cores)
        {
            Current.forgeDust += Mathf.Max(0, dust);
            Current.bindingRunes += Mathf.Max(0, runes);
            Current.corruptionCores += Mathf.Max(0, cores);
        }

        public static string PermanentForgeCostDescription(ForgeCost cost)
        {
            List<string> parts = new List<string>();
            if (cost.dust > 0) parts.Add(cost.dust + " Forge Dust");
            if (cost.runes > 0) parts.Add(cost.runes + " Binding Rune" + (cost.runes == 1 ? string.Empty : "s"));
            if (cost.cores > 0) parts.Add(cost.cores + " Corruption Core" + (cost.cores == 1 ? string.Empty : "s"));
            return parts.Count == 0 ? "Free" : string.Join(" · ", parts);
        }

        private static string ProfileFolder { get { return Path.Combine(Application.persistentDataPath, FolderName); } }
        private static string ProfilePath(int index) { return Path.Combine(ProfileFolder, "profile_" + index + ".json"); }
        private static string RunPath(int index) { return Path.Combine(ProfileFolder, "run_" + index + ".json"); }

        private static void Quarantine(string path)
        {
            try
            {
                if (!File.Exists(path)) return;
                string quarantine = path + ".corrupt_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                File.Copy(path, quarantine, false);
            }
            catch (Exception exception) { Debug.LogWarning("Could not quarantine damaged save: " + exception.Message); }
        }

        private static ProfileData LoadBackup(int index)
        {
            string error;
            for (int i = 1; i <= BackupCount; i++)
            {
                ProfileData backup = TryReadJson<ProfileData>(ProfilePath(index) + ".bak" + i, out error);
                if (backup != null)
                {
                    LastSaveStatus = "Recovered backup " + i;
                    return backup;
                }
            }
            return TryReadJson<ProfileData>(ProfilePath(index) + ".bak", out error);
        }

        private static T TryReadJson<T>(string path, out string error) where T : class
        {
            error = string.Empty;
            if (!File.Exists(path)) return null;
            try
            {
                string json = File.ReadAllText(path);
                string hashPath = path + ".sha256";
                if (File.Exists(hashPath))
                {
                    string expected = File.ReadAllText(hashPath).Trim();
                    if (!string.Equals(expected, ComputeHash(json), StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("Checksum mismatch in " + Path.GetFileName(path) + ".");
                }
                T value = JsonUtility.FromJson<T>(json);
                if (value == null) throw new InvalidDataException("JSON contained no data.");
                return value;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return null;
            }
        }

        private static void RotateBackups(string path)
        {
            string oldest = path + ".bak" + BackupCount;
            if (File.Exists(oldest)) File.Delete(oldest);
            if (File.Exists(oldest + ".sha256")) File.Delete(oldest + ".sha256");
            for (int i = BackupCount - 1; i >= 1; i--)
            {
                string source = path + ".bak" + i;
                string destination = path + ".bak" + (i + 1);
                if (File.Exists(source)) File.Move(source, destination);
                if (File.Exists(source + ".sha256")) File.Move(source + ".sha256", destination + ".sha256");
            }
            if (File.Exists(path)) File.Copy(path, path + ".bak1", true);
            if (File.Exists(path + ".sha256")) File.Copy(path + ".sha256", path + ".bak1.sha256", true);
        }

        private static void CommitTemporary(string temporary, string destination)
        {
            if (File.Exists(destination))
            {
                try { File.Replace(temporary, destination, null, true); return; }
                catch (PlatformNotSupportedException) { }
                catch (IOException) { }
                string previous = destination + ".replace_previous";
                if (File.Exists(previous)) File.Delete(previous);
                File.Move(destination, previous);
                try { File.Move(temporary, destination); }
                catch
                {
                    if (!File.Exists(destination) && File.Exists(previous)) File.Move(previous, destination);
                    throw;
                }
                if (File.Exists(previous)) File.Delete(previous);
                return;
            }
            File.Move(temporary, destination);
        }

        private static void WriteAllTextDurable(string path, string text)
        {
            byte[] bytes = new UTF8Encoding(false).GetBytes(text);
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(true);
            }
        }

        private static string ComputeHash(string value)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
                StringBuilder builder = new StringBuilder(hash.Length * 2);
                foreach (byte part in hash) builder.Append(part.ToString("x2"));
                return builder.ToString();
            }
        }

        private static ProfileData CreateDefaultProfile()
        {
            ProfileData profile = new ProfileData();
            profile.createdUtc = DateTime.UtcNow.ToString("o");
            profile.unlockedCoreIds.AddRange(new[] { "fireball", "ice_nova", "lightning_strike", "arc_beam" });
            profile.unlockedModifierIds.AddRange(new[] { "triple", "toxic", "frost", "homing", "arc", "pierce", "explosion", "efficient" });
            profile.spellArchive.Add(new CoreSaveData("fireball"));
            profile.spellArchive.Add(new CoreSaveData("ice_nova"));
            profile.spellArchive.Add(new CoreSaveData("lightning_strike"));
            profile.preparedSpells.Add(new PreparedSpellSave { slotIndex = 0, contentId = "fireball", isRelic = false });
            profile.preparedSpells.Add(new PreparedSpellSave { slotIndex = 1, contentId = "ice_nova", isRelic = false });
            profile.preparedSpells.Add(new PreparedSpellSave { slotIndex = 2, contentId = "lightning_strike", isRelic = false });
            profile.preparedModifiers.Add(new PreparedModifierSave { modifierId = "triple", count = 1 });
            profile.preparedModifiers.Add(new PreparedModifierSave { modifierId = "toxic", count = 1 });
            profile.preparedModifiers.Add(new PreparedModifierSave { modifierId = "efficient", count = 1 });

            string[] starterItems =
            {
                "starter_helm", "starter_shoulder_l", "starter_shoulder_r", "starter_chest", "starter_glove_l",
                "starter_glove_r", "starter_pants", "runner_boots", "apprentice_wand", "mana_orb"
            };
            foreach (string id in starterItems) profile.armory.Add(new ItemSaveData(id, 1, true));
            return profile;
        }

        private static void MigrateLegacyPlayerPrefs(ProfileData profile)
        {
            profile.essence += PlayerPrefs.GetInt("ArcaneEngine.Meta.Essence", 0);
            profile.healthRank = PlayerPrefs.GetInt("ArcaneEngine.Meta.Health", 0);
            profile.manaRank = PlayerPrefs.GetInt("ArcaneEngine.Meta.Mana", 0);
            profile.powerRank = PlayerPrefs.GetInt("ArcaneEngine.Meta.Power", 0);
        }
    }
}
