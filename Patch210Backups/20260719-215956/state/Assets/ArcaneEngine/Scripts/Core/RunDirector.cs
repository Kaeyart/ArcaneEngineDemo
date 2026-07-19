using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    [Serializable]
    public struct ForgeCost
    {
        public int gold;
        public int dust;
        public int runes;
        public int cores;

        public ForgeCost(int goldCost, int dustCost, int runeCost = 0, int coreCost = 0)
        {
            gold = Mathf.Max(0, goldCost);
            dust = Mathf.Max(0, dustCost);
            runes = Mathf.Max(0, runeCost);
            cores = Mathf.Max(0, coreCost);
        }

        public string Describe()
        {
            List<string> parts = new List<string>();
            if (gold > 0) parts.Add(gold + " Gold");
            if (dust > 0) parts.Add(dust + " Forge Dust");
            if (runes > 0) parts.Add(runes + " Binding Rune" + (runes == 1 ? string.Empty : "s"));
            if (cores > 0) parts.Add(cores + " Corruption Core" + (cores == 1 ? string.Empty : "s"));
            return parts.Count == 0 ? "Free" : string.Join(" · ", parts);
        }
    }

    [Serializable]
    public sealed class DifficultySettings
    {
        public bool frenziedEnemies;
        public bool bulwarkEnemies;
        public bool glassSoul;
        public bool manaDrought;
        public bool extraEliteAffixes;
        public bool reducedHealing;
        public bool newBossPhase;
        public bool unstableWorld;
        public bool adaptiveEnemies;
        public bool cursedShops;
        public bool timedRooms;
        public bool reducedRerolls;
        public bool noStartingEquipment;
        public bool noStartingModifiers;

        public float RewardMultiplier
        {
            get
            {
                float value = 1f;
                if (frenziedEnemies) value += 0.2f;
                if (bulwarkEnemies) value += 0.25f;
                if (glassSoul) value += 0.35f;
                if (manaDrought) value += 0.2f;
                if (extraEliteAffixes) value += 0.25f;
                if (reducedHealing) value += 0.2f;
                if (newBossPhase) value += 0.35f;
                if (unstableWorld) value += 0.2f;
                if (adaptiveEnemies) value += 0.25f;
                if (cursedShops) value += 0.15f;
                if (timedRooms) value += 0.25f;
                if (reducedRerolls) value += 0.1f;
                if (noStartingEquipment) value += 0.45f;
                if (noStartingModifiers) value += 0.25f;
                return value;
            }
        }

        public int EnabledCount
        {
            get
            {
                int count = 0;
                foreach (System.Reflection.FieldInfo field in GetType().GetFields()) if (field.FieldType == typeof(bool) && (bool)field.GetValue(this)) count++;
                return count;
            }
        }
    }

    public sealed class RunDirector : MonoBehaviour
    {
        public readonly DifficultySettings Difficulty = new DifficultySettings();
        public readonly List<RewardOffer> RewardChoices = new List<RewardOffer>();
        public readonly List<ShopOffer> ShopOffers = new List<ShopOffer>();
        public readonly List<RoomTemplate> RouteChoices = new List<RoomTemplate>();
        private readonly List<string> _lastRunItemNames = new List<string>();
        private readonly List<string> _lastRunSpellNames = new List<string>();
        private readonly List<ItemSaveData> _lastRunItems = new List<ItemSaveData>();
        public IReadOnlyList<string> LastRunItemNames { get { return _lastRunItemNames; } }
        public IReadOnlyList<string> LastRunSpellNames { get { return _lastRunSpellNames; } }
        public IReadOnlyList<ItemSaveData> LastRunItems { get { return _lastRunItems; } }

        public int RoomIndex { get; private set; }
        public int TotalRooms { get; private set; }
        public int Kills { get; private set; }
        public int Drachmas { get; private set; }
        public int Gold { get { return Drachmas; } }
        public int ForgeDust { get; private set; }
        public int BindingRunes { get; private set; }
        public int CorruptionCores { get; private set; }
        public int LastEssenceAward { get; private set; }
        public int LastRunItemCount { get; private set; }
        public int LastRunForgeDust { get; private set; }
        public int LastRunBindingRunes { get; private set; }
        public int LastRunCorruptionCores { get; private set; }
        public int LastRunGold { get; private set; }
        public int LastRunSpellCopies { get; private set; }
        public int LastRunSupportRunes { get; private set; }
        public int RewardRerollsRemaining { get; private set; }
        public bool HasRunResult { get; private set; }
        public bool LastRunVictory { get; private set; }
        public bool PendingRewards { get; private set; }
        public bool PendingRoute { get; private set; }
        public bool PendingShop { get; private set; }
        public bool PendingSafeRoom { get; private set; }
        public bool PendingExtraction { get; private set; }
        public bool EncounterActive { get; private set; }
        public float TimedRoomRemaining { get; private set; }
        public RoomTemplate CurrentRoom { get; private set; }
        public DemoRunMode RunMode { get; private set; }
        public int RunLevel { get; private set; } = 1;
        public int RunExperience { get; private set; }
        public int ExperienceToNextLevel { get { return 35 + RunLevel * 20; } }
        public int CurrentSeed { get { return _runSeed; } }
        public int RoomsCleared { get; private set; }
        public int ObjectiveBonusExperience { get; private set; }
        public bool ReinforcementsPending { get; private set; }

        public event Action<RoomTemplate> RoomEntered;
        public event Action<RoomTemplate> RoomCleared;
        public event Action<EnemyController> EnemyDefeated;

        public int Wave { get { return RoomIndex + 1; } }

        private GameWorld _world;
        private int _runSeed;
        private string _runInstanceId;
        private int _roomsWithoutModifier;
        private int _roomsWithoutEquipment;
        private int _roomsWithoutCore;
        private readonly Queue<string> _recentRooms = new Queue<string>();
        private bool _roomWasPopulated;
        private bool _timedPenaltyApplied;
        private float _temporaryPowerBonus;
        private float _shopDiscount;
        private int? _requestedSeed;
        private readonly Dictionary<RunLevelPerk, int> _runPerks = new Dictionary<RunLevelPerk, int>();
        private readonly List<string> _routeHistory = new List<string>();
        private readonly HashSet<string> _usedRoomServices = new HashSet<string>();
        private readonly HashSet<string> _purchasedShopOffers = new HashSet<string>();
        private readonly HashSet<string> _claimedPermanentRewards = new HashSet<string>();
        private readonly List<RewardOffer> _rejectedRewards = new List<RewardOffer>();
        private float _nextPeriodicLinkPulse;

        private void Start() { _world = GameWorld.Instance; }

        private void Update()
        {
            if (_world == null || !_world.RunActive || _world.TrainingMode) return;
            if (EncounterActive && !_world.ModalOpen && Time.time >= _nextPeriodicLinkPulse)
            {
                _nextPeriodicLinkPulse = Time.time + 6f;
                Vector3 direction = _world.Player == null ? Vector3.forward : _world.Player.AimDirection;
                Vector3 position = _world.Player == null ? Vector3.zero : _world.Player.transform.position;
                SpellExecutor.NotifyPlayerEvent(TriggerMoment.OnPeriodic, position, direction);
            }
            if (Difficulty.timedRooms && EncounterActive)
            {
                TimedRoomRemaining -= Time.deltaTime;
                if (TimedRoomRemaining <= 0f && !_timedPenaltyApplied)
                {
                    _timedPenaltyApplied = true;
                    _world.Player.TakeDamage(18f + RoomIndex * 2f);
                    _world.Log("The room timer expired. You take bonus damage.");
                }
            }

            bool enemiesEmpty = _world.Enemies.Count == 0;
            bool objectiveComplete = DemoV05Director.Instance == null ? enemiesEmpty : DemoV05Director.Instance.CanCompleteEncounter(enemiesEmpty);
            if (EncounterActive && _roomWasPopulated && objectiveComplete)
            {
                EncounterActive = false;
                _roomWasPopulated = false;
                CompleteRoom();
            }
        }

        public void BeginRun()
        {
            BeginRun(DemoRunMode.Standard);
        }

        public void BeginRun(DemoRunMode mode)
        {
            ProfileManager.DeleteRunSnapshot();
            _world.ClearTransientObjects();
            _world.ResetRunSystems(Difficulty);
            _world.RecalculateStats(false);
            _world.Player.ResetForRun();
            _world.Player.transform.position = new Vector3(0f, 1f, 0f);
            RoomIndex = 0;
            RunMode = mode;
            _runSeed = mode == DemoRunMode.Daily ? DailySeed() : _requestedSeed.HasValue ? _requestedSeed.Value : UnityEngine.Random.Range(1, int.MaxValue / 4);
            _runInstanceId = Guid.NewGuid().ToString("N");
            _requestedSeed = null;
            TotalRooms = mode == DemoRunMode.Endless ? 999 : 10 + Mathf.Abs(_runSeed % 3);
            Kills = 0;
            Drachmas = 0;
            ForgeDust = 0;
            BindingRunes = 0;
            CorruptionCores = 0;
            LastEssenceAward = 0;
            LastRunItemCount = LastRunForgeDust = LastRunBindingRunes = LastRunCorruptionCores = LastRunGold = LastRunSpellCopies = LastRunSupportRunes = 0;
            _lastRunItemNames.Clear();
            _lastRunSpellNames.Clear();
            _lastRunItems.Clear();
            HasRunResult = false;
            LastRunVictory = false;
            RunLevel = 1;
            RunExperience = 0;
            RoomsCleared = 0;
            ObjectiveBonusExperience = 0;
            ReinforcementsPending = false;
            PendingRewards = PendingRoute = PendingShop = PendingSafeRoom = PendingExtraction = false;
            EncounterActive = false;
            _nextPeriodicLinkPulse = Time.time + 6f;
            ReinforcementsPending = false;
            RewardRerollsRemaining = Mathf.Max(0, ProfileManager.Current.RewardRerolls - (Difficulty.reducedRerolls ? 1 : 0));
            _roomsWithoutModifier = _roomsWithoutEquipment = _roomsWithoutCore = 0;
            _recentRooms.Clear();
            _temporaryPowerBonus = 0f;
            _shopDiscount = 0f;
            _runPerks.Clear();
            _routeHistory.Clear();
            _usedRoomServices.Clear();
            _purchasedShopOffers.Clear();
            _claimedPermanentRewards.Clear();
            _world.RunActive = true;
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.ResetRunPresentation();
            if (RunStatistics.Instance != null) RunStatistics.Instance.BeginRun();
            Time.timeScale = 1f;
            EnterRoom(MegaCatalog.RandomRoom(DungeonRoomType.Combat, _runSeed, _recentRooms));
            _world.Log("Run started. Choose your route; every Support Rune belongs to one spell at a time.");
        }

        public void BeginSeededRun(int seed)
        {
            int positive = seed == int.MinValue ? int.MaxValue / 4 : Mathf.Abs(seed);
            _requestedSeed = Mathf.Clamp(positive, 1, int.MaxValue / 4);
            BeginRun(DemoRunMode.Standard);
        }

        public bool ContinueSavedRun(out string message)
        {
            RunSnapshotData snapshot = ProfileManager.LoadRunSnapshot();
            if (snapshot == null)
            {
                message = "No valid saved run was found.";
                return false;
            }
            string expectedVisualSignature;
            string savedVisualSignature;
            if (!VisualContinuationValidation.MatchesSavedInputs(snapshot, out expectedVisualSignature, out savedVisualSignature))
            {
                message = "The saved run's visual reconstruction data failed validation. The backup save was not overwritten.";
                Debug.LogError(message + " Expected " + expectedVisualSignature + " but calculated " + savedVisualSignature + ".");
                return false;
            }
            RoomTemplate room = MegaCatalog.AllRooms.FirstOrDefault(candidate => candidate.id == snapshot.roomId);
            if (room == null)
            {
                message = "The saved room no longer exists in this version.";
                return false;
            }

            _world.ClearTransientObjects();
            CopyDifficulty(snapshot.difficulty, Difficulty);
            RoomIndex = Mathf.Max(0, snapshot.roomIndex);
            TotalRooms = Mathf.Max(RoomIndex + 1, snapshot.totalRooms);
            Kills = Mathf.Max(0, snapshot.kills);
            Drachmas = Mathf.Max(0, snapshot.gold);
            ForgeDust = Mathf.Max(0, snapshot.forgeDust);
            BindingRunes = Mathf.Max(0, snapshot.bindingRunes);
            CorruptionCores = Mathf.Max(0, snapshot.corruptionCores);
            RunMode = (DemoRunMode)Mathf.Clamp(snapshot.runMode, 0, 2);
            RunLevel = Mathf.Max(1, snapshot.runLevel);
            RunExperience = Mathf.Max(0, snapshot.runExperience);
            RoomsCleared = Mathf.Max(0, snapshot.roomsCleared);
            ObjectiveBonusExperience = Mathf.Max(0, snapshot.objectiveBonusExperience);
            RewardRerollsRemaining = Mathf.Max(0, snapshot.rewardRerolls);
            _roomsWithoutModifier = Mathf.Max(0, snapshot.roomsWithoutModifier);
            _roomsWithoutEquipment = Mathf.Max(0, snapshot.roomsWithoutEquipment);
            _roomsWithoutCore = Mathf.Max(0, snapshot.roomsWithoutSpell);
            _temporaryPowerBonus = snapshot.temporaryPowerBonus;
            _shopDiscount = snapshot.shopDiscount;
            _runPerks.Clear();
            if (snapshot.runPerks != null)
                foreach (RunPerkSaveData perk in snapshot.runPerks)
                    if (perk != null && Enum.IsDefined(typeof(RunLevelPerk), perk.perk)) _runPerks[(RunLevelPerk)perk.perk] = Mathf.Max(0, perk.rank);
            _runSeed = snapshot.runSeed;
            _runInstanceId = string.IsNullOrEmpty(snapshot.runInstanceId) ? Guid.NewGuid().ToString("N") : snapshot.runInstanceId;
            _recentRooms.Clear();
            _routeHistory.Clear(); if (snapshot.routeHistory != null) _routeHistory.AddRange(snapshot.routeHistory);
            _usedRoomServices.Clear(); if (snapshot.usedRoomServices != null) foreach (string id in snapshot.usedRoomServices) _usedRoomServices.Add(id);
            _purchasedShopOffers.Clear(); if (snapshot.purchasedShopOffers != null) foreach (string id in snapshot.purchasedShopOffers) _purchasedShopOffers.Add(id);
            _claimedPermanentRewards.Clear(); if (snapshot.claimedPermanentRewards != null) foreach (string id in snapshot.claimedPermanentRewards) _claimedPermanentRewards.Add(id);
            _rejectedRewards.Clear(); if (snapshot.rejectedRewards != null) _rejectedRewards.AddRange(snapshot.rejectedRewards.Where(value => value != null));
            HasRunResult = false;
            LastRunVictory = false;
            _world.RunActive = true;
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.ResetRunPresentation();
            if (RunStatistics.Instance != null) RunStatistics.Instance.Restore(snapshot.damageDealt, snapshot.damageTaken, snapshot.criticalHits, snapshot.dodges);
            if (!_world.RestoreRunState(snapshot))
            {
                _world.RunActive = false;
                message = "The saved spell loadout could not be restored.";
                return false;
            }
            string restoredVisualSignature = VisualContinuationValidation.CaptureRuntime(_world, _runSeed, RoomIndex, TotalRooms, room.id);
            if (!string.IsNullOrEmpty(snapshot.visualReconstructionSignature) && snapshot.visualReconstructionSignature != restoredVisualSignature)
            {
                _world.RunActive = false;
                _world.ClearTransientObjects();
                message = "The run loaded, but its procedural visual inputs did not reconstruct exactly. The checkpoint was left untouched.";
                Debug.LogError(message + " Saved " + snapshot.visualReconstructionSignature + " but restored " + restoredVisualSignature + ".");
                return false;
            }
            _world.RecalculateStats(false);
            _world.Player.ResetForRun();
            EnterRoom(room);
            _world.Player.RestoreSavedResources(snapshot.healthRatio, snapshot.manaRatio);
            _world.Player.RestoreDamageSource(snapshot.lastDamageSource);
            SaveRunCheckpoint();
            Time.timeScale = 1f;
            message = "Saved run restored. The current room restarts from the beginning.";
            _world.Log(message);
            return true;
        }

        public bool SaveAndQuit(out string message)
        {
            if (_world == null || !_world.RunActive || _world.TrainingMode)
            {
                message = "A normal dungeon run is not active.";
                return false;
            }
            if (!SaveRunCheckpoint())
            {
                message = ProfileManager.LastSaveStatus;
                return false;
            }
            _world.RunActive = false;
            PendingRewards = PendingRoute = PendingShop = PendingSafeRoom = PendingExtraction = false;
            EncounterActive = false;
            _world.ClearTransientObjects();
            _world.BuildSanctuaryEnvironment();
            _world.Equipment.LoadFromProfile(false);
            _world.RecalculateStats(false);
            _world.Player.transform.position = new Vector3(0f, 1f, 0f);
            Time.timeScale = 1f;
            message = "Run saved. Continue it from Home Base.";
            return true;
        }

        public bool SaveRunCheckpoint()
        {
            if (_world == null || !_world.RunActive || _world.TrainingMode || CurrentRoom == null) return false;
            RunSnapshotData snapshot = new RunSnapshotData
            {
                runSeed = _runSeed,
                runInstanceId = _runInstanceId,
                roomIndex = RoomIndex,
                totalRooms = TotalRooms,
                kills = Kills,
                gold = Drachmas,
                forgeDust = ForgeDust,
                bindingRunes = BindingRunes,
                corruptionCores = CorruptionCores,
                rewardRerolls = RewardRerollsRemaining,
                roomsWithoutModifier = _roomsWithoutModifier,
                roomsWithoutEquipment = _roomsWithoutEquipment,
                roomsWithoutSpell = _roomsWithoutCore,
                temporaryPowerBonus = _temporaryPowerBonus,
                shopDiscount = _shopDiscount,
                runMode = (int)RunMode,
                runLevel = RunLevel,
                runExperience = RunExperience,
                damageDealt = RunStatistics.Instance == null ? 0 : Mathf.RoundToInt(RunStatistics.Instance.DamageDealt),
                damageTaken = RunStatistics.Instance == null ? 0 : Mathf.RoundToInt(RunStatistics.Instance.DamageTaken),
                criticalHits = RunStatistics.Instance == null ? 0 : RunStatistics.Instance.CriticalHits,
                dodges = RunStatistics.Instance == null ? 0 : RunStatistics.Instance.Dodges,
                roomsCleared = RoomsCleared,
                objectiveBonusExperience = ObjectiveBonusExperience,
                runPerks = _runPerks.Select(pair => new RunPerkSaveData { perk = (int)pair.Key, rank = pair.Value }).ToList(),
                routeHistory = _routeHistory.ToList(),
                usedRoomServices = _usedRoomServices.ToList(),
                purchasedShopOffers = _purchasedShopOffers.ToList(),
                claimedPermanentRewards = _claimedPermanentRewards.ToList(),
                rejectedRewards = _rejectedRewards.ToList(),
                lastDamageSource = _world.Player == null ? "Unknown" : _world.Player.LastDamageSource,
                roomId = CurrentRoom.id,
                difficulty = CloneDifficulty(Difficulty)
            };
            _world.CaptureRunState(snapshot);
            snapshot.visualReconstructionSignature = VisualContinuationValidation.Compute(snapshot);
            return ProfileManager.SaveRunSnapshot(snapshot);
        }

        public void OnEnemyKilled(Vector3 deathPosition, bool eliteOrBoss)
        {
            OnEnemyKilled(null, deathPosition, eliteOrBoss);
        }

        public void OnEnemyKilled(EnemyController enemy, Vector3 deathPosition, bool eliteOrBoss)
        {
            if (_world != null && _world.TrainingMode) return;
            Kills++;
            ProfileManager.Current.totalKills++;
            int killSeed = V1Determinism.Combine(_runSeed, RoomIndex, enemy == null ? "enemy" : enemy.Archetype.ToString(), Kills);
            int earned = eliteOrBoss ? 9 + killSeed % 7 : 1 + killSeed % 3;
            AddDrachmas(Mathf.RoundToInt(earned * Difficulty.RewardMultiplier));
            if (eliteOrBoss) AddForgeMaterials(2 + killSeed % 3, 1, 0);
            else if (killSeed % 8 == 0) AddForgeMaterials(1, 0, 0);
            AddRunExperience(eliteOrBoss ? 14 : 5);
            if (EnemyDefeated != null) EnemyDefeated(enemy);
            UnityEngine.Random.State previousRandom = UnityEngine.Random.state;
            UnityEngine.Random.InitState(killSeed);
            _world.SpawnLoot(deathPosition);
            UnityEngine.Random.state = previousRandom;
        }

        public void AddRunExperience(int amount)
        {
            RunExperience += Mathf.Max(0, amount);
            while (RunExperience >= ExperienceToNextLevel)
            {
                RunExperience -= ExperienceToNextLevel;
                RunLevel++;
                ProfileManager.Current.highestRunLevel = Mathf.Max(ProfileManager.Current.highestRunLevel, RunLevel);
                _temporaryPowerBonus += 0.03f;
                _world.RecalculateStats(true);
                _world.Player.RestoreBetweenRooms(0.12f, 0.3f);
                RunLevelChoicePickup.SpawnChoices(this, RunLevel);
                _world.Log("RUN LEVEL " + RunLevel + " — choose one of three in-world perks.");
                if (DemoV05Director.Instance != null) DemoV05Director.Instance.ShowRunLevelUp(RunLevel);
            }
        }

        public void AddObjectiveBonus(int amount)
        {
            int bonus = Mathf.Max(0, amount);
            ObjectiveBonusExperience += bonus;
            AddRunExperience(bonus);
        }

        public void ApplyRunLevelPerk(RunLevelPerk perk)
        {
            int rank; _runPerks.TryGetValue(perk, out rank); _runPerks[perk] = rank + 1;
            _world.RecalculateStats(true);
            _world.Player.RestoreBetweenRooms(perk == RunLevelPerk.Vitality ? 0.18f : 0.04f, perk == RunLevelPerk.ManaEfficiency ? 0.2f : 0.04f);
            _world.Log(perk + " selected for this run · rank " + (rank + 1) + ".");
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce(perk.ToString().ToUpperInvariant() + " · RUN PERK ACTIVE", 2.2f);
            SaveRunCheckpoint();
        }

        public void ResumeAfterPerkChoice()
        {
            if (!_world.RunActive || PendingRewards || PendingRoute || PendingShop || PendingSafeRoom || PendingExtraction || EncounterActive) return;
            OpenRouteOrFinale();
        }

        public void ApplyRunBonuses(PlayerStats stats)
        {
            if (stats == null || GameWorld.Instance == null || !GameWorld.Instance.RunActive) return;
            stats.spellPower += _temporaryPowerBonus + PerkRank(RunLevelPerk.SpellPower) * 0.08f;
            stats.cooldownMultiplier *= Mathf.Pow(0.92f, PerkRank(RunLevelPerk.FastCasting));
            stats.maxHealth += PerkRank(RunLevelPerk.Vitality) * 15f;
            stats.manaCostMultiplier *= Mathf.Pow(0.91f, PerkRank(RunLevelPerk.ManaEfficiency));
            stats.critChance += PerkRank(RunLevelPerk.CriticalFocus) * 0.06f;
            stats.moveSpeed += PerkRank(RunLevelPerk.Movement) * 0.7f;
            stats.triggerEnergy += PerkRank(RunLevelPerk.TriggerCapacity) * 20f;
        }

        private int PerkRank(RunLevelPerk perk) { int rank; return _runPerks.TryGetValue(perk, out rank) ? rank : 0; }

        public void ChooseReward(int index)
        {
            if (!PendingRewards || index < 0 || index >= RewardChoices.Count) return;
            RewardOffer reward = RewardChoices[index];
            foreach (RewardOffer rejected in RewardChoices.Where((value, choiceIndex) => choiceIndex != index && value != null))
            {
                _rejectedRewards.Add(rejected);
                while (_rejectedRewards.Count > 6) _rejectedRewards.RemoveAt(0);
            }
            if (!string.IsNullOrEmpty(reward.id)) _claimedPermanentRewards.Add(reward.id);
            ApplyReward(reward);
            PendingRewards = false;
            WorldInteractionController.ClearType<RoomRewardPickup>();
            WorldInteractionController.ClearType<RewardRerollStation>();
            RewardChoices.Clear();
            if (_world.PendingSpellUpgrade && DemoUI.Instance != null) DemoUI.Instance.OpenWorkshopAnywhere();
            else if (_world.SpellLinks.HasPendingReward && DemoUI.Instance != null) DemoUI.Instance.OpenSpellLinksAnywhere();
            else OpenRouteOrFinale();
        }

        public void ResumeAfterSpellSystemReward()
        {
            if (_world == null || !_world.RunActive || _world.HasPendingSpellSystemReward || PendingRewards || PendingRoute || PendingShop || PendingSafeRoom || PendingExtraction || EncounterActive) return;
            SaveRunCheckpoint();
            OpenRouteOrFinale();
        }

        public void RerollRewards()
        {
            if (!PendingRewards || RewardRerollsRemaining <= 0) return;
            RewardRerollsRemaining--;
            BuildRewards(CurrentRoom.type);
            RoomRewardPickup.SpawnChoices(this, RewardChoices);
            _world.Log("Reward selection rerolled. " + RewardRerollsRemaining + " reroll(s) remain.");
        }

        public void ChooseRoute(int index)
        {
            if (!PendingRoute || index < 0 || index >= RouteChoices.Count) return;
            RoomTemplate selected = RouteChoices[index];
            PendingRoute = false;
            WorldInteractionController.ClearType<RoomDoor>();
            RouteChoices.Clear();
            RoomIndex++;
            EnterRoom(selected);
        }

        public void LeaveShop()
        {
            if (!PendingShop) return;
            PendingShop = false;
            WorldInteractionController.ClearType<ShopOfferPedestal>();
            WorldInteractionController.ClearType<RoomServiceStation>();
            OpenRouteOrFinale();
        }

        public void LeaveSafeRoom()
        {
            if (!PendingSafeRoom) return;
            PendingSafeRoom = false;
            WorldInteractionController.ClearType<RoomServiceStation>();
            OpenRouteOrFinale();
        }

        public bool IsRoomServiceUsed(string serviceId)
        {
            return _usedRoomServices.Contains(RoomIndex + ":" + serviceId);
        }

        public bool TryUseRoomService(string serviceId)
        {
            string stableId = RoomIndex + ":" + serviceId;
            if (_usedRoomServices.Contains(stableId)) return false;
            _usedRoomServices.Add(stableId);
            return true;
        }

        public bool BuyShopOffer(int index, out string message)
        {
            if (!PendingShop || index < 0 || index >= ShopOffers.Count)
            {
                message = "That offer is unavailable.";
                return false;
            }
            ShopOffer offer = ShopOffers[index];
            int price = Mathf.Max(1, Mathf.RoundToInt(offer.price * (1f - _shopDiscount)));
            if (offer.sold) { message = "That offer has already been purchased."; return false; }
            if (!string.IsNullOrEmpty(offer.contentId) && offer.contentId.StartsWith("service:") && !CanApplyShopService(offer.contentId, out message))
                return false;
            if (Drachmas < price) { message = "Requires " + price + " Gold."; return false; }
            Drachmas -= price;
            offer.sold = true;
            try
            {
                _purchasedShopOffers.Add(offer.id);
                ProfileManager.RecordTransaction("Run Shop", offer.id, -price, offer.title);
                if (!string.IsNullOrEmpty(offer.contentId) && offer.contentId.StartsWith("service:"))
                {
                    ApplyShopService(offer.contentId);
                    _usedRoomServices.Add(RoomIndex + ":" + offer.contentId);
                    message = "Purchased service: " + offer.title + ".";
                    SaveRunCheckpoint();
                    return true;
                }
                RewardOffer reward = new RewardOffer { category = offer.category, contentId = offer.contentId, amount = 1, title = offer.title, generatedItem = offer.generatedItem };
                ApplyReward(reward);
                message = "Purchased " + offer.title + " for " + price + " Gold.";
                SaveRunCheckpoint();
                return true;
            }
            catch (Exception exception)
            {
                Drachmas += price;
                offer.sold = false;
                _purchasedShopOffers.Remove(offer.id);
                message = "Purchase cancelled safely. No Gold was spent.";
                if (V1Diagnostics.Instance != null) V1Diagnostics.Instance.Recover(message, exception);
                return false;
            }
        }

        public void AddDrachmas(int amount)
        {
            float bonus = _world == null || _world.Stats == null ? 0f : _world.Stats.goldFind;
            Drachmas += Mathf.Max(0, Mathf.RoundToInt(amount * (1f + bonus)));
        }

        public void RefundGold(int amount) { Drachmas += Mathf.Max(0, amount); }

        public bool TrySpendGold(int amount)
        {
            amount = Mathf.Max(0, amount);
            if (Drachmas < amount) return false;
            Drachmas -= amount;
            return true;
        }

        public bool TrySpendForge(ForgeCost cost, out string message)
        {
            if (Drachmas < cost.gold || ForgeDust < cost.dust || BindingRunes < cost.runes || CorruptionCores < cost.cores)
            {
                message = "Requires " + cost.Describe() + ".";
                return false;
            }
            Drachmas -= cost.gold;
            ForgeDust -= cost.dust;
            BindingRunes -= cost.runes;
            CorruptionCores -= cost.cores;
            message = string.Empty;
            return true;
        }

        public void RefundForge(ForgeCost cost)
        {
            Drachmas += Mathf.Max(0, cost.gold);
            ForgeDust += Mathf.Max(0, cost.dust);
            BindingRunes += Mathf.Max(0, cost.runes);
            CorruptionCores += Mathf.Max(0, cost.cores);
        }

        public void AddForgeMaterials(int dust, int runes, int cores)
        {
            ForgeDust += Mathf.Max(0, dust);
            BindingRunes += Mathf.Max(0, runes);
            CorruptionCores += Mathf.Max(0, cores);
        }

        public bool SellCore(CoreSaveData core, out string message)
        {
            if (core == null || !_world.RunCoreSatchel.Contains(core)) { message = "That Spell Copy is not in Stored Spells."; return false; }
            SpellCoreDefinition definition = DemoCatalog.GetCore(core.coreId);
            _world.RunCoreSatchel.Remove(core);
            AddDrachmas(18 + RoomIndex * 2);
            ProfileManager.RecordTransaction("Sell Spell Copy", core.coreId, 18 + RoomIndex * 2, "Sold during run");
            message = "Sold " + definition.displayName + " for " + (18 + RoomIndex * 2) + " Gold.";
            return true;
        }

        public void EndRun(bool successfulExtraction = false)
        {
            if (!_world.RunActive || _world.TrainingMode) return;
            _world.RunActive = false;
            PendingRewards = PendingRoute = PendingShop = PendingSafeRoom = PendingExtraction = false;
            LastRunVictory = successfulExtraction;
            HasRunResult = true;
            LastRunItemCount = _world.Equipment.runBag.Count;
            _lastRunItemNames.Clear();
            _lastRunItemNames.AddRange(_world.Equipment.runBag.Where(item => item != null).Select(item => item.DisplayName));
            _lastRunItems.Clear();
            _lastRunItems.AddRange(_world.Equipment.runBag.Where(item => item != null).Select(item => item.ToSaveData(false)));
            LastRunForgeDust = ForgeDust;
            LastRunBindingRunes = BindingRunes;
            LastRunCorruptionCores = CorruptionCores;
            LastRunGold = Drachmas;
            LastRunSpellCopies = _world.RunCoreSatchel.Count;
            _lastRunSpellNames.Clear();
            _lastRunSpellNames.AddRange(_world.RunCoreSatchel.Where(core => core != null).Select(core =>
            {
                SpellCoreDefinition definition = DemoCatalog.GetCore(core.coreId);
                return definition == null ? core.coreId : definition.displayName;
            }));
            LastRunSupportRunes = _world.OwnedModifierCounts.Values.Sum();
            LastEssenceAward = Mathf.Max(1, Mathf.RoundToInt((Kills * 0.55f + RoomIndex * 2.4f) * Difficulty.RewardMultiplier));
            string endRewardId = "runend:" + _runInstanceId;
            if (ProfileManager.TryClaimPermanentReward(endRewardId))
            {
                ProfileManager.Current.essence += LastEssenceAward;
                ProfileManager.RecordTransaction("Run Result", endRewardId, LastEssenceAward, successfulExtraction ? "Extraction" : "Defeat");
            }
            else LastEssenceAward = 0;
            ProfileManager.Current.bestRoom = Mathf.Max(ProfileManager.Current.bestRoom, RoomIndex + 1);
            ProfileManager.Current.endlessUnlocked |= successfulExtraction;
            if (RunMode == DemoRunMode.Daily)
            {
                ProfileManager.Current.lastDailyChallengeDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                ProfileManager.Current.dailyChallengeBest = Mathf.Max(ProfileManager.Current.dailyChallengeBest, RoomsCleared);
            }
            if (successfulExtraction)
            {
                _world.BankRunLoot();
                ProfileManager.Current.completedRuns++;
                _world.Log("Extraction complete. Unsecured equipment, Spell Copies, and Forge materials are now secured; remaining Gold is lost.");
            }
            else
            {
                ProfileManager.Current.failedRuns++;
                _world.Log("You fell. Unsecured equipment, Forge materials, Spell Copies, Support Runes, and Gold are lost; Essence remains.");
            }
            Drachmas = 0;
            ForgeDust = 0;
            BindingRunes = 0;
            CorruptionCores = 0;
            RecordRunAndBuildHistory(successfulExtraction);
            if (NarrativeDirector.Instance != null) NarrativeDirector.Instance.OnRunEnded(successfulExtraction);
            ProfileManager.DeleteRunSnapshot();
            ProfileManager.Save();
            _world.ClearTransientObjects();
            _world.BuildSanctuaryEnvironment();
            _world.Equipment.LoadFromProfile(false);
            _world.RecalculateStats(false);
            _world.Player.transform.position = new Vector3(0f, 1f, 0f);
            Time.timeScale = 1f;
            RunResultVisualSequence.Show(this);
            if (DemoUI.Instance != null) DemoUI.Instance.ShowRunRecap();
        }

        public bool BuyUpgrade(string key, out string message)
        {
            ProfileData profile = ProfileManager.Current;
            int rank;
            int cap;
            if (key == "Health") { rank = profile.healthRank; cap = 8; }
            else if (key == "Mana") { rank = profile.manaRank; cap = 8; }
            else if (key == "Power") { rank = profile.powerRank; cap = 6; }
            else if (key == "StartingSpells") { rank = profile.startingSpellRank; cap = 2; }
            else if (key == "Preparation") { rank = profile.preparationRank; cap = 6; }
            else if (key == "Rerolls") { rank = profile.rewardRerollRank; cap = 3; }
            else if (key == "ArchiveTools") { rank = profile.archiveToolRank; cap = 3; }
            else { message = "Unknown permanent upgrade."; return false; }
            if (rank >= cap) { message = key + " is at maximum rank."; return false; }
            int cost = key == "StartingSpells" ? 35 + rank * 65 : 10 + rank * 12;
            if (profile.essence < cost) { message = "Requires " + cost + " Essence."; return false; }
            profile.essence -= cost;
            if (key == "Health") profile.healthRank++;
            else if (key == "Mana") profile.manaRank++;
            else if (key == "Power") profile.powerRank++;
            else if (key == "StartingSpells") profile.startingSpellRank++;
            else if (key == "Preparation") profile.preparationRank++;
            else if (key == "Rerolls") profile.rewardRerollRank++;
            else if (key == "ArchiveTools") profile.archiveToolRank++;
            ProfileManager.RecordTransaction("Permanent Upgrade", key, -cost, "Rank " + (rank + 1));
            ProfileManager.Save();
            _world.RecalculateStats(true);
            message = key + " permanently upgraded to rank " + (rank + 1) + ".";
            return true;
        }

        public bool UnlockContent(string id, bool modifier, out string message)
        {
            ProfileData profile = ProfileManager.Current;
            List<string> list = modifier ? profile.unlockedModifierIds : profile.unlockedCoreIds;
            if (list.Contains(id)) { message = "Already unlocked."; return false; }
            int cost = modifier ? 18 : 30;
            if (profile.essence < cost) { message = "Requires " + cost + " Essence."; return false; }
            profile.essence -= cost;
            list.Add(id);
            if (!modifier) profile.spellArchive.Add(new CoreSaveData(id));
            ProfileManager.RecordTransaction("Unlock", id, -cost, modifier ? "Support Rune" : "Spell Core");
            ProfileManager.Save();
            message = "Unlocked " + (modifier ? DemoCatalog.GetModifier(id).displayName : DemoCatalog.GetCore(id).displayName) + ".";
            return true;
        }

        public bool ForgeRelic(string relicId, string coreInstanceId, out string message)
        {
            RelicDefinition relic = MegaCatalog.GetRelic(relicId);
            CoreSaveData core = ProfileManager.FindCore(coreInstanceId);
            ProfileData profile = ProfileManager.Current;
            if (relic == null || core == null || core.coreId != relic.sourceCoreId) { message = "A matching Spell Copy is required."; return false; }
            if (!profile.discoveredRelicIds.Contains(relic.id)) { message = "That Legendary upgrade has not been discovered."; return false; }
            if (profile.bossKills < relic.requiredBossKills) { message = "Requires " + relic.requiredBossKills + " boss victories."; return false; }
            if (profile.relicShards < relic.shardCost) { message = "Requires " + relic.shardCost + " Legendary Shard(s)."; return false; }
            profile.relicShards -= relic.shardCost;
            profile.spellArchive.Remove(core);
            profile.relicArchive.Add(new RelicSaveData(relic.id, relic.sourceCoreId));
            ProfileManager.Discover("relic:" + relic.id);
            ProfileManager.RecordTransaction("Legendary Forge", relic.id, -relic.shardCost, "Consumed " + relic.sourceCoreId + " Spell Copy");
            ProfileManager.Save();
            message = relic.displayName + " created permanently. Its Legendary Effect is fixed; its spell board remains customizable.";
            return true;
        }

        public void ResetMetaProgression()
        {
            ProfileManager.ResetCurrent();
            _world.Equipment.LoadFromProfile(false);
            _world.RecalculateStats(false);
        }

        public void RespecUpgradeRanks(out int refund)
        {
            ProfileData profile = ProfileManager.Current;
            refund = RefundForRanks(profile.healthRank, false) + RefundForRanks(profile.manaRank, false) +
                     RefundForRanks(profile.powerRank, false) + RefundForRanks(profile.startingSpellRank, true) +
                     RefundForRanks(profile.preparationRank, false) + RefundForRanks(profile.rewardRerollRank, false) +
                     RefundForRanks(profile.archiveToolRank, false);
            profile.healthRank = profile.manaRank = profile.powerRank = profile.startingSpellRank = 0;
            profile.preparationRank = profile.rewardRerollRank = profile.archiveToolRank = 0;
            profile.essence += refund;
            ProfileManager.RecordTransaction("Upgrade Respec", "all_ranks", refund, "Full rank refund");
            ProfileManager.Save();
            _world.RecalculateStats(true);
        }

        private static int RefundForRanks(int ranks, bool startingSpells)
        {
            int total = 0;
            for (int rank = 0; rank < Mathf.Max(0, ranks); rank++) total += startingSpells ? 35 + rank * 65 : 10 + rank * 12;
            return total;
        }

        private void EnterRoom(RoomTemplate room)
        {
            CurrentRoom = room;
            if (_routeHistory.Count == 0 || _routeHistory[_routeHistory.Count - 1] != room.id) _routeHistory.Add(room.id);
            TrackRecent(room.id);
            _world.ClearTransientObjects();
            _world.BuildRoom(room, RoomIndex, V1Determinism.Combine(_runSeed, RoomIndex, room.id));
            if (room.type != DungeonRoomType.Extraction) RoomDoorStateVisuals.SpawnLocked(room.accentColor);
            _world.Player.transform.position = new Vector3(0f, 1f, -11f);
            _world.Player.GrantSpawnProtection(1.25f);
            _world.Player.RestoreBetweenRooms(0.12f, 0.25f);
            PendingRewards = PendingRoute = PendingShop = PendingSafeRoom = PendingExtraction = false;
            EncounterActive = false;
            _nextPeriodicLinkPulse = Time.time + 6f;
            _roomWasPopulated = false;
            _timedPenaltyApplied = false;
            TimedRoomRemaining = 42f - Mathf.Min(12f, RoomIndex * 1.2f);
            ProfileManager.Discover("room:" + room.id);
            _world.Log("ROOM " + (RoomIndex + 1) + "/" + TotalRooms + " — " + room.displayName + ".");

            switch (room.type)
            {
                case DungeonRoomType.Combat: SpawnEncounter(false, false, false); break;
                case DungeonRoomType.Elite: SpawnEncounter(true, false, false); break;
                case DungeonRoomType.Miniboss: SpawnEncounter(true, true, false); break;
                case DungeonRoomType.Boss: SpawnEncounter(true, true, true); break;
                case DungeonRoomType.Shop: BuildShop(); PendingShop = true; PhysicalShopSystem.Spawn(this, ShopOffers); break;
                case DungeonRoomType.SafeWorkshop: PendingSafeRoom = true; SafeRoomSystem.Spawn(this, false); break;
                case DungeonRoomType.HealingSanctuary:
                    _world.Player.Heal(_world.Stats.maxHealth * 0.5f * _world.Stats.healingMultiplier); PendingSafeRoom = true;
                    SafeRoomSystem.Spawn(this, true);
                    _world.Log("The sanctuary restores life. You may reorganize before continuing."); break;
                case DungeonRoomType.Extraction: PendingExtraction = true; ExtractionGate.Spawn(this); break;
                default:
                    SpawnEncounter(room.type == DungeonRoomType.Challenge || room.type == DungeonRoomType.CursedBargain,
                        room.type == DungeonRoomType.Challenge, false);
                    break;
            }
            if (RoomEntered != null) RoomEntered(room);
            SaveRunCheckpoint();
        }

        private void SpawnEncounter(bool elite, bool miniboss, bool boss)
        {
            UnityEngine.Random.State previousRandom = UnityEngine.Random.state;
            UnityEngine.Random.InitState(V1Determinism.Combine(_runSeed, RoomIndex, CurrentRoom == null ? "encounter" : CurrentRoom.id, 17));
            int count = boss ? 1 : miniboss ? 2 : Mathf.Min(15, 4 + RoomIndex + (elite ? 2 : 0));
            int disruptiveEnemies = 0;
            int supportEnemies = 0;
            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)Mathf.Max(1, count) * Mathf.PI * 2f + UnityEngine.Random.Range(-0.25f, 0.25f);
                float distance = UnityEngine.Random.Range(7f, 13f);
                Vector3 position = new Vector3(Mathf.Cos(angle) * distance, 0.75f, Mathf.Sin(angle) * distance + 2f);
                position = DungeonObstacle.Resolve(position, 0.65f);
                EnemyArchetype archetype = SelectArchetype(i, boss);
                bool disruptive = archetype == EnemyArchetype.Hexer || archetype == EnemyArchetype.Mirror || archetype == EnemyArchetype.Controller;
                bool support = archetype == EnemyArchetype.Leech || archetype == EnemyArchetype.Warden;
                if (!boss && disruptive && disruptiveEnemies >= (miniboss ? 1 : 2)) archetype = i % 2 == 0 ? EnemyArchetype.Crawler : EnemyArchetype.Bulwark;
                else if (!boss && support && supportEnemies >= 2) archetype = EnemyArchetype.Charger;
                if (archetype == EnemyArchetype.Hexer || archetype == EnemyArchetype.Mirror || archetype == EnemyArchetype.Controller) disruptiveEnemies++;
                if (archetype == EnemyArchetype.Leech || archetype == EnemyArchetype.Warden) supportEnemies++;
                int eliteLimit = miniboss ? 2 : elite ? (RoomIndex >= 7 ? 2 : 1) : 0;
                bool enemyElite = !boss && (i < eliteLimit || (i == 0 && RoomIndex > 4 && CurrentRoom.type == DungeonRoomType.Challenge));
                EnemyController spawned = EnemyController.Spawn(position, archetype, RoomIndex + 1, Difficulty, enemyElite, boss);
                if (miniboss && spawned != null)
                    spawned.gameObject.AddComponent<V21MinibossMechanics>().Initialize(spawned, CurrentRoom == null ? string.Empty : CurrentRoom.biome, i, Difficulty);
            }
            if (boss && Difficulty.newBossPhase)
                for (int i = 0; i < 3; i++) EnemyController.Spawn(new Vector3(-5f + i * 5f, 0.75f, 7f), EnemyArchetype.Hexer, RoomIndex + 1, Difficulty, true, false);
            UnityEngine.Random.state = previousRandom;
            EncounterActive = true;
            _roomWasPopulated = true;
        }

        public void SpawnObjectiveWave(int count, bool elite)
        {
            if (ReinforcementsPending) return;
            StartCoroutine(SpawnObjectiveWaveRoutine(count, elite));
        }

        private System.Collections.IEnumerator SpawnObjectiveWaveRoutine(int count, bool elite)
        {
            ReinforcementsPending = true;
            UnityEngine.Random.State previousRandom = UnityEngine.Random.state;
            UnityEngine.Random.InitState(V1Determinism.Combine(_runSeed, RoomIndex, "objective_wave", Kills + count));
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < Mathf.Max(1, count); i++)
            {
                float angle = i / (float)Mathf.Max(1, count) * Mathf.PI * 2f + UnityEngine.Random.Range(-0.2f, 0.2f);
                Vector3 position = new Vector3(Mathf.Cos(angle) * 12f, 0.75f, Mathf.Sin(angle) * 10f + 2f);
                positions.Add(position);
                LineRenderer warning = RuntimeVisuals.Ring("Reinforcement Warning", position, new Color(1f, 0.72f, 0.08f), 1.15f, 0.16f);
                Destroy(warning.gameObject, 0.85f);
            }
            UnityEngine.Random.state = previousRandom;
            EncounterActive = true; _roomWasPopulated = true;
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce("REINFORCEMENTS INCOMING · WATCH THE FLOOR", 1.2f);
            yield return new WaitForSeconds(0.8f);
            if (_world != null && _world.RunActive)
                for (int i = 0; i < positions.Count; i++)
                    EnemyController.Spawn(positions[i], SelectArchetype(i + Kills, false), RoomIndex + 1, Difficulty, elite && i == 0, false);
            ReinforcementsPending = false;
        }

        private EnemyArchetype SelectArchetype(int index, bool boss)
        {
            return V12EncounterPlanner.Select(_runSeed, RoomIndex, index, boss);
        }

        private void CompleteRoom()
        {
            RoomsCleared++;
            _world.Log("Room clear. Spellcrafting and Spell Link editing are now available.");
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce("ROOM CLEAR · SPELLCRAFTING AVAILABLE", 1.4f);
            AddRunExperience(10 + RoomIndex * 2);
            if (RoomCleared != null) RoomCleared(CurrentRoom);
            int roomMoney = Mathf.RoundToInt((6 + RoomIndex * 1.5f) * Difficulty.RewardMultiplier);
            AddDrachmas(roomMoney);
            ForgeMaterialReward materialReward = V12RewardPolicy.ForRoom(CurrentRoom.type, RoomIndex);
            AddForgeMaterials(materialReward.dust, materialReward.runes, materialReward.cores);
            RewardVisualSystem.ShowForgeGrant(_world.Player.transform.position, materialReward.dust, materialReward.runes, materialReward.cores);
            if (CurrentRoom.type == DungeonRoomType.Boss)
            {
                ProfileData profile = ProfileManager.Current;
                string bossRewardId = "boss:" + _runInstanceId + ":" + RoomIndex + ":" + CurrentRoom.id;
                if (!profile.claimedBossRewardIds.Contains(bossRewardId))
                {
                    profile.claimedBossRewardIds.Add(bossRewardId);
                    while (profile.claimedBossRewardIds.Count > 500) profile.claimedBossRewardIds.RemoveAt(0);
                    _claimedPermanentRewards.Add(bossRewardId);
                    profile.relicShards += 1;
                    RewardVisualSystem.ShowRelicShard(_world.Player.transform.position + Vector3.forward * 1.4f);
                    profile.bossKills++;
                    ProfileManager.RecordTransaction("Boss Reward", bossRewardId, 1, "Legendary Shard");
                    List<RelicDefinition> locked = MegaCatalog.AllRelics.Where(r => !profile.discoveredRelicIds.Contains(r.id) && r.requiredBossKills <= profile.bossKills).ToList();
                    if (locked.Count > 0)
                    {
                        RelicDefinition discovered = locked[Mathf.Abs(_runSeed + profile.bossKills) % locked.Count];
                        profile.discoveredRelicIds.Add(discovered.id);
                        _world.Log("Legendary upgrade discovered: " + discovered.displayName + ".");
                    }
                    ProfileManager.Save();
                }
                _world.SpawnLoot(_world.Player.transform.position + new Vector3(2f, 0.5f, 0f), true);
                _world.Log("Boss defeated. One Legendary Shard is saved immediately.");
                PresentRewards(DungeonRoomType.Boss);
                return;
            }
            PresentRewards(CurrentRoom.type);
        }

        private void PresentRewards(DungeonRoomType roomType)
        {
            BuildRewards(roomType);
            PendingRewards = true;
            RoomRewardPickup.SpawnChoices(this, RewardChoices);
            _world.Log("Rewards appeared in the center. Move close and press E to take one.");
        }

        private void OpenRouteOrFinale()
        {
            if (FindAnyObjectByType<RunLevelChoicePickup>() != null)
            {
                _world.Log("Choose a Run Perk before opening the route doors.");
                return;
            }
            if (CurrentRoom.type == DungeonRoomType.Boss && RunMode != DemoRunMode.Endless)
            {
                RoomIndex++;
                EnterRoom(MegaCatalog.RandomRoom(DungeonRoomType.Extraction, _runSeed + 999, _recentRooms));
                return;
            }
            BuildRouteChoices();
            PendingRoute = true;
            RoomDoor.SpawnChoices(this, RouteChoices);
            _world.Log("Route doors opened. Read the room icon, move close, and press E to enter.");
        }

        private void BuildRouteChoices()
        {
            RouteChoices.Clear();
            if (RunMode == DemoRunMode.Endless && CurrentRoom != null && CurrentRoom.type == DungeonRoomType.Boss)
            {
                RouteChoices.Add(MegaCatalog.RandomRoom(DungeonRoomType.Extraction, _runSeed + RoomIndex * 5, _recentRooms));
                RouteChoices.Add(MegaCatalog.RandomRoom(DungeonRoomType.Combat, _runSeed + RoomIndex * 5 + 1, _recentRooms));
                RouteChoices.Add(MegaCatalog.RandomRoom(DungeonRoomType.Elite, _runSeed + RoomIndex * 5 + 2, _recentRooms));
                return;
            }
            if (RunMode == DemoRunMode.Endless && (RoomIndex + 1) % 5 == 0)
            {
                for (int i = 0; i < 3; i++) RouteChoices.Add(MegaCatalog.RandomRoom(DungeonRoomType.Boss, _runSeed + RoomIndex + i * 17, _recentRooms));
                return;
            }
            if (RoomIndex >= TotalRooms - 2)
            {
                for (int i = 0; i < 3; i++)
                    RouteChoices.Add(MegaCatalog.RandomRoom(DungeonRoomType.Boss, _runSeed + RoomIndex + i * 17, _recentRooms));
                return;
            }
            if (RoomIndex == TotalRooms - 3)
            {
                RouteChoices.Add(MegaCatalog.RandomRoom(DungeonRoomType.SafeWorkshop, _runSeed + RoomIndex, _recentRooms));
                RouteChoices.Add(MegaCatalog.RandomRoom(DungeonRoomType.HealingSanctuary, _runSeed + RoomIndex + 1, _recentRooms));
                RouteChoices.Add(MegaCatalog.RandomRoom(DungeonRoomType.Elite, _runSeed + RoomIndex + 2, _recentRooms));
                return;
            }

            DungeonRoomType first = WeightedRoomType(RoomIndex * 2);
            DungeonRoomType second = WeightedRoomType(RoomIndex * 2 + 1);
            if (second == first) second = first == DungeonRoomType.Combat ? DungeonRoomType.ModifierReward : DungeonRoomType.Combat;
            DungeonRoomType third = WeightedRoomType(RoomIndex * 2 + 2);
            if (third == first || third == second) third = third == DungeonRoomType.EquipmentReward ? DungeonRoomType.Combat : DungeonRoomType.EquipmentReward;
            RouteChoices.Add(MegaCatalog.RandomRoom(first, _runSeed + RoomIndex * 43, _recentRooms));
            RouteChoices.Add(MegaCatalog.RandomRoom(second, _runSeed + RoomIndex * 43 + 1, _recentRooms));
            RouteChoices.Add(MegaCatalog.RandomRoom(third, _runSeed + RoomIndex * 43 + 2, _recentRooms));
        }

        private DungeonRoomType WeightedRoomType(int salt)
        {
            if (RoomIndex == 1 && Enumerable.Range(0, 3).Any(i => !_world.HasSpell((SpellSlot)i))) return DungeonRoomType.SpellCoreReward;
            if (_roomsWithoutModifier >= 2) return DungeonRoomType.ModifierReward;
            if (_roomsWithoutEquipment >= 3) return DungeonRoomType.EquipmentReward;
            if (_roomsWithoutCore >= 4) return DungeonRoomType.SpellCoreReward;
            DungeonRoomType[] pool =
            {
                DungeonRoomType.Combat, DungeonRoomType.Combat, DungeonRoomType.Elite, DungeonRoomType.ModifierReward,
                DungeonRoomType.EquipmentReward, DungeonRoomType.SpellCoreReward, DungeonRoomType.TreasureVault,
                DungeonRoomType.Shop, DungeonRoomType.CursedBargain, DungeonRoomType.Puzzle, DungeonRoomType.NarrativeEvent,
                DungeonRoomType.Challenge, DungeonRoomType.Secret, DungeonRoomType.Miniboss
            };
            DungeonRoomType type = pool[Mathf.Abs(_runSeed + salt * 101 + RoomIndex * 17) % pool.Length];
            if (type == DungeonRoomType.Shop && RoomIndex < 2) type = DungeonRoomType.Combat;
            return type;
        }

        private void BuildRewards(DungeonRoomType roomType)
        {
            RewardChoices.Clear();
            RewardCategory guaranteed = roomType == DungeonRoomType.ModifierReward ? RewardCategory.Modifier :
                roomType == DungeonRoomType.SpellCoreReward ? RewardCategory.SpellCore :
                roomType == DungeonRoomType.EquipmentReward ? RewardCategory.Equipment :
                roomType == DungeonRoomType.Boss ? RewardCategory.SpellUpgrade :
                _roomsWithoutModifier >= 2 ? RewardCategory.Modifier : RewardCategory.Drachmas;
            RewardChoices.Add(CreateReward(guaranteed, 0));
            RewardChoices.Add(CreateReward(BuildAwareCategory(), 1));
            RewardChoices.Add(CreateReward(roomType == DungeonRoomType.CursedBargain || roomType == DungeonRoomType.Challenge ? RewardCategory.CursedPower : RandomRareCategory(), 2));
            IReadOnlyList<V21RewardDefinitionAsset> authoredRewards = V21AuthoredContentOverlay.Rewards;
            if (authoredRewards.Count > 0 && Mathf.Abs(V1Determinism.Combine(_runSeed, RoomIndex, "authored_reward")) % 4 == 0)
            {
                V21RewardDefinitionAsset asset = authoredRewards[Mathf.Abs(V1Determinism.Combine(_runSeed, RoomIndex, "authored_reward_pick")) % authoredRewards.Count];
                if (asset != null && !string.IsNullOrEmpty(asset.stableId))
                    RewardChoices[2] = new RewardOffer { id = RoomIndex + ":authored:" + asset.stableId, category = asset.category,
                        title = asset.title, description = asset.description, contentId = asset.contentId, amount = Mathf.Max(1, asset.amount), color = asset.color };
            }
        }

        private RewardCategory BuildAwareCategory()
        {
            if (Enumerable.Range(0, 3).Any(i => !_world.HasSpell((SpellSlot)i))) return RewardCategory.SpellCore;
            if (_world.Player.Health < _world.Stats.maxHealth * 0.4f) return RewardCategory.Healing;
            if (_world.OwnedModifierCounts.Count < RoomIndex + 2) return RewardCategory.Modifier;
            return RewardCategory.Modifier;
        }

        private RewardCategory RandomRareCategory()
        {
            List<RewardCategory> pool = new List<RewardCategory>
            {
                RewardCategory.SpellUpgrade, RewardCategory.SpellUpgrade, RewardCategory.Blessing,
                RewardCategory.Drachmas, RewardCategory.ShopDiscount, RewardCategory.MapReveal,
                RewardCategory.ModifierTransformation
            };
            if (Enumerable.Range(0, 3).Count(index => _world.HasSpell((SpellSlot)index)) >= 2) pool.Add(RewardCategory.SpellLink);
            if (_world.SpellLinks.Slots < 3) pool.Add(RewardCategory.LinkUpgrade);
            return pool[Mathf.Abs(_runSeed + RoomIndex * 67 + RewardChoices.Count) % pool.Count];
        }

        private RewardOffer CreateReward(RewardCategory category, int salt)
        {
            int seed = Mathf.Abs(_runSeed + RoomIndex * 1009 + salt * 97 + RewardRerollsRemaining * 13);
            RewardOffer offer = new RewardOffer { id = RoomIndex + ":" + salt + ":" + seed, category = category, amount = 1 };
            if (category == RewardCategory.Modifier || category == RewardCategory.ModifierTransformation)
            {
                List<SpellModifierDefinition> pool = DemoCatalog.AllModifiers.Where(m => ProfileManager.Current.unlockedModifierIds.Contains(m.id)).ToList();
                if (pool.Count == 0) pool = DemoCatalog.AllModifiers.Take(8).ToList();
                SpellModifierDefinition modifier = pool[seed % pool.Count];
                offer.title = modifier.FullDisplayName.Replace("\n", " · "); offer.description = modifier.shortDescription + " Capacity Cost " + modifier.capacityCost + ". Physical run copy; cannot be shared."; offer.contentId = modifier.id; offer.color = modifier.uiColor;
            }
            else if (category == RewardCategory.SpellCore)
            {
                List<SpellCoreDefinition> pool = DemoCatalog.AllCores.Where(c => c.id != "void_maw" && ProfileManager.Current.unlockedCoreIds.Contains(c.id)).ToList();
                SpellCoreDefinition core = pool[seed % pool.Count];
                offer.title = core.displayName + " · Spell Copy"; offer.description = core.description + " Equip, store, sell, or use it for a Legendary upgrade."; offer.contentId = core.id; offer.color = core.color;
            }
            else if (category == RewardCategory.Equipment)
            {
                ItemDefinition item = _world.SelectLootBase(seed, CurrentRoom.type == DungeonRoomType.Boss);
                ItemInstance generated = new ItemInstance(item.id, RoomIndex + 1, false);
                int itemSeed = V1Determinism.Combine(_runSeed, RoomIndex, offer.id, V1Determinism.StableHash(item.id));
                V11Itemization.Generate(generated, item, itemSeed);
                if (Difficulty.cursedShops && CurrentRoom != null && CurrentRoom.type == DungeonRoomType.Shop)
                    V11Itemization.ApplyDeterministicCorruption(generated, V1Determinism.Combine(itemSeed, 0, "cursed_shop"));
                offer.generatedItem = generated.ToSaveData(false);
                offer.title = generated.DisplayName;
                offer.description = V11Itemization.BuildTooltip(generated, false) + "\nUNSECURED · Equip after extraction.";
                offer.contentId = item.id; offer.color = item.color;
            }
            else if (category == RewardCategory.Drachmas)
            {
                offer.amount = 22 + RoomIndex * 3; offer.title = offer.amount + " Gold"; offer.description = "Run-only currency. Spend it before finishing the run."; offer.color = new Color(1f, 0.8f, 0.2f);
            }
            else if (category == RewardCategory.Essence)
            {
                offer.amount = 3 + RoomIndex / 2; offer.title = offer.amount + " Essence"; offer.description = "Permanent currency secured immediately, even on death."; offer.color = new Color(0.3f, 1f, 0.85f);
            }
            else if (category == RewardCategory.Healing)
            {
                offer.amount = 35; offer.title = "Restorative Font"; offer.description = "Restore 35% maximum life and all mana."; offer.color = new Color(0.3f, 1f, 0.5f);
            }
            else if (category == RewardCategory.BoardExpansion)
            {
                offer.category = RewardCategory.SpellUpgrade;
                offer.title = "Spell Upgrade"; offer.description = "Choose one equipped spell. It gains one level, more Capacity, and additional board cells for this run."; offer.color = new Color(0.4f, 0.7f, 1f);
            }
            else if (category == RewardCategory.SpellUpgrade)
            {
                offer.title = "Spell Upgrade"; offer.description = "Choose one equipped spell. It gains one level, more Capacity, and additional board cells for this run."; offer.color = new Color(0.4f, 0.78f, 1f);
            }
            else if (category == RewardCategory.SpellLink)
            {
                offer.title = "Spell Link"; offer.description = "Choose one of three trigger conditions, then connect two equipped spells."; offer.contentId = seed.ToString(); offer.color = new Color(0.82f, 0.3f, 1f);
            }
            else if (category == RewardCategory.LinkUpgrade)
            {
                offer.title = "Link Upgrade"; offer.description = "Unlock one additional Spell Link slot for this run, up to three."; offer.color = new Color(0.68f, 0.4f, 1f);
            }
            else if (category == RewardCategory.EquipmentUpgrade)
            {
                offer.title = "Forge Material Cache"; offer.description = "Gain unsecured Forge materials. Secure them by extracting successfully."; offer.color = new Color(1f, 0.55f, 0.2f);
            }
            else if (category == RewardCategory.ShopDiscount)
            {
                offer.title = "Merchant's Seal"; offer.description = "All future shop prices are 20% lower this run."; offer.color = new Color(1f, 0.85f, 0.25f);
            }
            else if (category == RewardCategory.MapReveal)
            {
                offer.title = "Cartographer's Eye"; offer.description = "Reveal route categories and gain a reward reroll."; offer.color = new Color(0.5f, 0.9f, 1f);
            }
            else if (category == RewardCategory.CursedPower)
            {
                offer.title = "Cursed Power"; offer.description = "+25% spell power for this run, but lose 18% current Health and gain Spell Overload."; offer.risky = true; offer.color = new Color(1f, 0.08f, 0.35f);
            }
            else
            {
                offer.title = "Arcane Blessing"; offer.description = "Restore resources and gain a temporary power blessing."; offer.color = new Color(0.65f, 0.4f, 1f);
            }
            return offer;
        }

        private void ApplyReward(RewardOffer reward)
        {
            if (reward == null) return;
            V12CombatEventBus.Publish(V12CombatEventType.RewardCommitted, reward.category.ToString(), reward.contentId,
                reward.amount, _world == null || _world.Player == null ? Vector3.zero : _world.Player.transform.position, reward.title);
            _roomsWithoutModifier++; _roomsWithoutEquipment++; _roomsWithoutCore++;
            if (reward.category == RewardCategory.Modifier || reward.category == RewardCategory.ModifierTransformation) { _world.AddModifier(reward.contentId); _roomsWithoutModifier = 0; }
            else if (reward.category == RewardCategory.SpellCore) { _world.AddCoreCopy(reward.contentId); _roomsWithoutCore = 0; }
            else if (reward.category == RewardCategory.Equipment)
            {
                int itemSeed = V1Determinism.Combine(_runSeed, RoomIndex, reward.contentId,
                    V1Determinism.StableHash(reward.id ?? reward.title));
                ItemInstance item = reward.generatedItem != null
                    ? _world.Equipment.AddGenerated(reward.generatedItem, false)
                    : _world.Equipment.Add(reward.contentId, RoomIndex + 1, false, true, itemSeed);
                if (item != null && Difficulty.cursedShops && CurrentRoom != null && CurrentRoom.type == DungeonRoomType.Shop && !item.corrupted)
                    V11Itemization.ApplyDeterministicCorruption(item, V1Determinism.Combine(itemSeed, 0, "cursed_shop"));
                _roomsWithoutEquipment = 0;
            }
            else if (reward.category == RewardCategory.Drachmas) AddDrachmas(reward.amount);
            else if (reward.category == RewardCategory.Essence)
            {
                string claimId = "runreward:" + _runInstanceId + ":" + (reward.id ?? RoomIndex + ":essence");
                if (ProfileManager.TryClaimPermanentReward(claimId))
                {
                    ProfileManager.Current.essence += reward.amount;
                    ProfileManager.RecordTransaction("Room Reward", claimId, reward.amount, "Essence secured immediately");
                    ProfileManager.Save();
                }
                else _world.Log("That permanent room reward was already secured by this run.");
            }
            else if (reward.category == RewardCategory.Healing) { _world.Player.Heal(_world.Stats.maxHealth * 0.35f * _world.Stats.healingMultiplier); _world.Player.RestoreMana(_world.Stats.maxMana); }
            else if (reward.category == RewardCategory.BoardExpansion || reward.category == RewardCategory.SpellUpgrade)
            {
                string result;
                if (!_world.BeginSpellUpgradeReward(out result)) { AddDrachmas(25); _world.Log(result + " You received 25 Gold instead."); }
            }
            else if (reward.category == RewardCategory.SpellLink)
            {
                int linkSeed;
                if (!int.TryParse(reward.contentId, out linkSeed)) linkSeed = V1Determinism.Combine(_runSeed, RoomIndex, reward.id ?? "spell_link");
                string result;
                if (!_world.BeginSpellLinkReward(linkSeed, out result)) { AddDrachmas(25); _world.Log(result + " You received 25 Gold instead."); }
            }
            else if (reward.category == RewardCategory.LinkUpgrade)
            {
                string result;
                if (!_world.SpellLinks.AddSlot(out result)) AddDrachmas(25);
                _world.Log(result);
            }
            else if (reward.category == RewardCategory.EquipmentUpgrade)
            {
                AddForgeMaterials(5 + RoomIndex / 3, 1, 0);
                _world.Log("Equipment cannot be upgraded during an expedition. Unsecured Forge materials were added instead.");
            }
            else if (reward.category == RewardCategory.ShopDiscount) _shopDiscount = Mathf.Clamp01(_shopDiscount + 0.2f);
            else if (reward.category == RewardCategory.MapReveal) RewardRerollsRemaining++;
            else if (reward.category == RewardCategory.CursedPower)
            {
                _world.Player.TakeUnavoidableDamage(_world.Player.Health * 0.18f);
                _temporaryPowerBonus += 0.25f;
                _world.RecalculateStats(true);
            }
            else
            {
                _temporaryPowerBonus += 0.12f; _world.RecalculateStats(true);
                _world.Player.RestoreBetweenRooms(0.2f, 1f);
            }
            _world.Log("Chosen reward: " + reward.title + ".");
        }

        private void BuildShop()
        {
            ShopOffers.Clear();
            string[] specializations = { "Runesmith", "Armorer", "Apothecary", "Forbidden Archivist", "Spell Broker" };
            string shopkeeper = specializations[Mathf.Abs(_runSeed + RoomIndex) % specializations.Length];
            RewardCategory[][] stock =
            {
                new[] { RewardCategory.Modifier, RewardCategory.Modifier, RewardCategory.SpellUpgrade, RewardCategory.SpellLink, RewardCategory.LinkUpgrade },
                new[] { RewardCategory.Equipment, RewardCategory.Equipment, RewardCategory.EquipmentUpgrade, RewardCategory.Equipment, RewardCategory.Healing },
                new[] { RewardCategory.Healing, RewardCategory.Healing, RewardCategory.Blessing, RewardCategory.Modifier, RewardCategory.Equipment },
                new[] { RewardCategory.CursedPower, RewardCategory.ModifierTransformation, RewardCategory.Equipment, RewardCategory.SpellCore, RewardCategory.Blessing },
                new[] { RewardCategory.SpellCore, RewardCategory.SpellCore, RewardCategory.SpellUpgrade, RewardCategory.SpellLink, RewardCategory.Modifier }
            };
            int specializationIndex = Array.IndexOf(specializations, shopkeeper);
            for (int i = 0; i < 5; i++)
            {
                RewardCategory category = stock[Mathf.Max(0, specializationIndex)][i];
                RewardOffer reward = CreateReward(category, 20 + i);
                int price = category == RewardCategory.Healing ? 12 : category == RewardCategory.Modifier ? 22 : category == RewardCategory.Equipment ? 34 : category == RewardCategory.SpellCore ? 40 : 28;
                if (Difficulty.cursedShops)
                {
                    price = Mathf.RoundToInt(price * 0.8f);
                    reward.description += category == RewardCategory.Equipment
                        ? " This item is already corrupted; inspect its actual corruption above."
                        : " Cursed-market discount applied.";
                }
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":shop:" + i, title = shopkeeper + " · " + reward.title, description = reward.description,
                    contentId = reward.contentId, category = reward.category, price = price, generatedItem = reward.generatedItem });
            }
            ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:duplicate", title = "Duplicate a Support Rune", description = "Create another copy of one Support Rune owned this run.", contentId = "service:duplicate", category = RewardCategory.Blessing, price = 32 });
            ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:materials", title = "Armorer · Unsecured Forge Cache", description = "Gain Forge Dust and a Binding Rune. Materials become permanent only after extraction.", contentId = "service:materials", category = RewardCategory.EquipmentUpgrade, price = 30 });
            ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:recovery", title = "Apothecary · Full Recovery", description = "Restore Health and Mana. Equipment corruption can be changed only at Home Base.", contentId = "service:recovery", category = RewardCategory.Healing, price = 24 });
            ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:reveal", title = "Spell Broker · Recover Fate", description = "Gain one reward reroll and reveal route categories.", contentId = "service:reveal", category = RewardCategory.MapReveal, price = 20 });
            if (shopkeeper == "Runesmith")
            {
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:capacity", title = "Runesmith · Expand Spell Capacity", description = "Add one temporary board-radius bonus to the equipped spell with the least space.", contentId = "service:capacity", category = RewardCategory.BoardExpansion, price = 42 });
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:reroll_rune", title = "Runesmith · Transform a Support Rune", description = "Replace one spare Support Rune with a different compatible run-owned Rune.", contentId = "service:reroll_rune", category = RewardCategory.ModifierTransformation, price = 28 });
            }
            else if (shopkeeper == "Armorer")
            {
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:cleanse", title = "Armorer · Cleanse Corruption", description = "Remove corruption from one unsecured item. The rest of its affixes remain.", contentId = "service:cleanse", category = RewardCategory.EquipmentUpgrade, price = 38 });
                ItemInstance rerollTarget = _world.Equipment.runBag.FirstOrDefault(value =>
                {
                    string ignored;
                    return V11Itemization.CanServiceReroll(value, out ignored);
                });
                string rerollDescription = rerollTarget == null
                    ? "Reroll one unlocked Prefix or Suffix on an eligible Magic or Rare Run Bag item. No eligible item is currently carried."
                    : "Reroll one unlocked Prefix or Suffix on " + rerollTarget.DisplayName + ". Locked affixes and item rarity are preserved.";
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:reroll_affix", title = "Armorer · Reroll Equipment Affix", description = rerollDescription,
                    contentId = "service:reroll_affix", category = RewardCategory.EquipmentUpgrade, price = 34 });
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:upgrade_item", title = "Armorer · Improve Unsecured Item", description = "Improve the quality and one affix roll of the first eligible Run Bag item.", contentId = "service:upgrade_item", category = RewardCategory.EquipmentUpgrade, price = 36 });
            }
            else if (shopkeeper == "Apothecary")
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:ward", title = "Apothecary · Emergency Ward", description = "Gain a lasting Shield for the next room.", contentId = "service:ward", category = RewardCategory.Healing, price = 30 });
            else if (shopkeeper == "Forbidden Archivist")
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:recover_reward", title = "Archivist · Recover Rejected Reward", description = "Recover the most recently rejected room reward.", contentId = "service:recover_reward", category = RewardCategory.Blessing, price = 45 });
            else if (shopkeeper == "Spell Broker")
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":service:copy_spell", title = "Spell Broker · Duplicate Spell Copy", description = "Duplicate one Spell Copy already carried this run.", contentId = "service:copy_spell", category = RewardCategory.SpellCore, price = 48 });
            foreach (V21ShopServiceAsset service in V21AuthoredContentOverlay.ShopServices.Where(value => value != null &&
                !string.IsNullOrEmpty(value.stableId) && (string.IsNullOrEmpty(value.specialization) || value.specialization == shopkeeper)))
                ShopOffers.Add(new ShopOffer { id = RoomIndex + ":authored_service:" + service.stableId, title = service.title,
                    description = service.description, contentId = service.serviceContentId, category = service.category, price = Mathf.Max(1, service.price) });
            foreach (ShopOffer offer in ShopOffers) if (_purchasedShopOffers.Contains(offer.id)) offer.sold = true;
        }

        private void ApplyShopService(string service)
        {
            if (service == "service:duplicate")
            {
                string modifier = _world.OwnedModifierCounts.OrderByDescending(p => p.Value).Select(p => p.Key).FirstOrDefault();
                if (!string.IsNullOrEmpty(modifier)) _world.AddModifier(modifier);
            }
            else if (service == "service:materials")
            {
                AddForgeMaterials(6 + RoomIndex / 2, 1, 0);
                _world.Log("Purchased unsecured Forge materials. Extract to keep them.");
            }
            else if (service == "service:recovery")
            {
                _world.Player.Heal(_world.Stats.maxHealth * _world.Stats.healingMultiplier);
                _world.Player.RestoreMana(_world.Stats.maxMana);
            }
            else if (service == "service:reveal") RewardRerollsRemaining++;
            else if (service == "service:capacity")
            {
                SpellBoard board = Enumerable.Range(0, 3).Select(index => _world.GetBoard((SpellSlot)index)).Where(value => value != null)
                    .OrderBy(value => value.temporaryRadiusBonus).ThenBy(value => value.UsedCapacity()).FirstOrDefault();
                if (board != null) { board.temporaryRadiusBonus = Mathf.Clamp(board.temporaryRadiusBonus + 1, 0, 2); _world.MarkSpellsDirty(); }
            }
            else if (service == "service:reroll_rune")
            {
                string oldId = _world.ModifierInventory.Where(pair => pair.Value > 0).OrderBy(pair => pair.Key).Select(pair => pair.Key).FirstOrDefault();
                SpellModifierDefinition replacement = DemoCatalog.AllModifiers.Where(value => value.id != oldId)
                    .OrderBy(value => V1Determinism.Combine(_runSeed, RoomIndex, value.id)).FirstOrDefault();
                if (!string.IsNullOrEmpty(oldId) && replacement != null)
                {
                    _world.OwnedModifierCounts[oldId] = Mathf.Max(0, _world.OwnedModifierCounts[oldId] - 1);
                    _world.AddModifier(replacement.id);
                    _world.RecalculateModifierAvailability();
                }
            }
            else if (service == "service:cleanse")
            {
                ItemInstance item = _world.Equipment.runBag.FirstOrDefault(value => value != null && value.corrupted);
                if (item != null)
                {
                    item.corrupted = false;
                    item.corruptionId = string.Empty;
                    item.affixes.RemoveAll(value => value != null && value.kind == AffixKind.Corruption);
                }
            }
            else if (service == "service:reroll_affix")
            {
                ItemInstance item = _world.Equipment.runBag.FirstOrDefault(value =>
                {
                    string ignored;
                    return V11Itemization.CanServiceReroll(value, out ignored);
                });
                string before;
                string after;
                string result;
                if (V11Itemization.TryServiceReroll(item, V1Determinism.Combine(_runSeed, RoomIndex, service), out before, out after, out result))
                    _world.Log(result);
            }
            else if (service == "service:upgrade_item")
            {
                ItemInstance item = _world.Equipment.runBag.FirstOrDefault(value => value != null && value.quality < 20);
                if (item != null)
                {
                    item.quality = Mathf.Clamp(item.quality + 5, 0, 20);
                    AffixRoll roll = item.affixes.FirstOrDefault(value => value != null);
                    if (roll != null) roll.value *= 1.08f;
                }
            }
            else if (service == "service:ward") _world.Player.AddWard(_world.Stats.maxHealth * 0.35f, 120f);
            else if (service == "service:recover_reward")
            {
                RewardOffer rejected = _rejectedRewards.LastOrDefault();
                if (rejected != null) { _rejectedRewards.RemoveAt(_rejectedRewards.Count - 1); ApplyReward(rejected); }
            }
            else if (service == "service:copy_spell")
            {
                CoreSaveData source = _world.RunCoreSatchel.FirstOrDefault();
                if (source != null) _world.AddCoreCopy(source.coreId);
            }
        }

        private bool CanApplyShopService(string service, out string message)
        {
            message = string.Empty;
            string[] supported = { "service:duplicate", "service:materials", "service:recovery", "service:reveal", "service:capacity",
                "service:reroll_rune", "service:cleanse", "service:reroll_affix", "service:upgrade_item", "service:ward", "service:recover_reward", "service:copy_spell" };
            if (!supported.Contains(service)) message = "This authored service uses an unsupported effect ID and cannot be purchased.";
            else if (service == "service:duplicate" && !_world.OwnedModifierCounts.Any(pair => pair.Value > 0))
                message = "No spare Support Rune is available to duplicate.";
            else if (service == "service:capacity" && !Enumerable.Range(0, 3).Select(index => _world.GetBoard((SpellSlot)index))
                .Any(board => board != null && board.temporaryRadiusBonus < 2))
                message = "Every equipped spell has reached the temporary Capacity expansion limit.";
            else if (service == "service:reroll_rune" && !_world.ModifierInventory.Any(pair => pair.Value > 0))
                message = "No spare Support Rune is available to transform.";
            else if (service == "service:cleanse" && !_world.Equipment.runBag.Any(item => item != null && item.corrupted))
                message = "The Run Bag contains no corrupted item.";
            else if (service == "service:reroll_affix" && !_world.Equipment.runBag.Any(item =>
            {
                string ignored;
                return V11Itemization.CanServiceReroll(item, out ignored);
            }))
                message = "The Run Bag contains no eligible Magic or Rare item with an unlocked affix.";
            else if (service == "service:upgrade_item" && !_world.Equipment.runBag.Any(item => item != null && item.quality < 20))
                message = "The Run Bag contains no item that this service can improve.";
            else if (service == "service:recover_reward" && _rejectedRewards.Count == 0)
                message = "No rejected room reward is available to recover.";
            else if (service == "service:copy_spell" && !_world.RunCoreSatchel.Any(item => item != null && !string.IsNullOrEmpty(item.coreId)))
                message = "No Spell Copy is available to duplicate.";
            return string.IsNullOrEmpty(message);
        }

        private void TrackRecent(string id)
        {
            _recentRooms.Enqueue(id);
            while (_recentRooms.Count > 4) _recentRooms.Dequeue();
        }

        private static DifficultySettings CloneDifficulty(DifficultySettings source)
        {
            DifficultySettings copy = new DifficultySettings();
            CopyDifficulty(source, copy);
            return copy;
        }

        private static void CopyDifficulty(DifficultySettings source, DifficultySettings destination)
        {
            if (source == null || destination == null) return;
            destination.frenziedEnemies = source.frenziedEnemies;
            destination.bulwarkEnemies = source.bulwarkEnemies;
            destination.glassSoul = source.glassSoul;
            destination.manaDrought = source.manaDrought;
            destination.extraEliteAffixes = source.extraEliteAffixes;
            destination.reducedHealing = source.reducedHealing;
            destination.newBossPhase = source.newBossPhase;
            destination.unstableWorld = source.unstableWorld;
            destination.adaptiveEnemies = source.adaptiveEnemies;
            destination.cursedShops = source.cursedShops;
            destination.timedRooms = source.timedRooms;
            destination.reducedRerolls = source.reducedRerolls;
            destination.noStartingEquipment = source.noStartingEquipment;
            destination.noStartingModifiers = source.noStartingModifiers;
        }

        private static int DailySeed()
        {
            DateTime day = DateTime.UtcNow.Date;
            return Mathf.Abs(day.Year * 10000 + day.Month * 100 + day.Day);
        }

        private void RecordRunAndBuildHistory(bool victory)
        {
            RunStatistics stats = RunStatistics.Instance;
            RunHistorySave history = new RunHistorySave
            {
                runInstanceId = _runInstanceId, completedUtc = DateTime.UtcNow.ToString("o"), mode = (int)RunMode, seed = _runSeed, victory = victory,
                roomsCleared = RoomsCleared, kills = Kills, runLevel = RunLevel, essence = LastEssenceAward,
                damageDealt = stats == null ? 0 : Mathf.RoundToInt(stats.DamageDealt),
                damageTaken = stats == null ? 0 : Mathf.RoundToInt(stats.DamageTaken),
                criticalHits = stats == null ? 0 : stats.CriticalHits, dodges = stats == null ? 0 : stats.Dodges,
                topSpell = stats == null ? "None" : stats.BestSpell,
                deathSource = victory || _world.Player == null ? string.Empty : _world.Player.LastDamageSource,
                bossFightSeconds = stats == null ? 0f : stats.BossFightSeconds,
                bossPillarsDestroyed = stats == null ? 0 : stats.BossPillarsDestroyed,
                bossPhasesReached = stats == null ? 0 : stats.BossPhasesReached,
                objectiveBonusExperience = ObjectiveBonusExperience
            };
            for (int slot = 0; slot < 3; slot++)
            {
                SpellBoard board = _world.GetBoard((SpellSlot)slot);
                CompiledSpell spell = _world.GetSpell((SpellSlot)slot);
                if (spell == null || board == null) continue;
                history.finalSpellNames.Add(spell.displayName);
                SpellBuildValidationReport report = SpellBuildValidator.Validate(board, _world.Stats, _world.Equipment);
                ProfileManager.RecordBuild(new BuildHistorySave
                {
                    stableId = report.stableId, discoveredUtc = DateTime.UtcNow.ToString("o"), name = spell.displayName,
                    coreId = spell.coreId, element = spell.element.ToString(), delivery = spell.delivery.ToString(),
                    activeModifiers = report.activePieces, triggers = report.triggerCount, estimatedDps = report.estimatedDps
                });
            }
            ProfileManager.RecordRun(history);
        }
    }
}
