using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ArpgFoundation30 : MonoBehaviour
    {
        public static ArpgFoundation30 Instance { get; private set; }
        public static ArpgProfile30 Profile { get; private set; }

        public bool MapActive { get { return _mapActive; } }
        public bool GuardianDefeated { get { return _guardianDefeated; } }
        public ArpgMapItem30 ActiveMap { get { return _activeMap; } }
        public string LastMessage { get { return _lastMessage; } }
        public float TimedMapRemaining { get { return _timedMapRemaining; } }
        public string CurrentObjective
        {
            get
            {
                if (Profile == null) return string.Empty;
                if (_mapActive) return _guardianDefeated ? "Secure the Guardian Cache" : "Defeat the map guardian";
                if (!Profile.objectiveFlags.Contains("refuge.spellforge")) return "Approach the SpellForge Altar";
                if (!Profile.objectiveFlags.Contains("refuge.map-device")) return "Activate the Map Device";
                if (Profile.highestCompletedTier < 0) return "Open and complete a Tier 0 map";
                if (Profile.highestCompletedTier < 5) return "Progress through Tier " + (Profile.highestCompletedTier + 1);
                return "Master the White Map network";
            }
        }

        private GameWorld _world;
        private bool _worldReady;
        private bool _mapActive;
        private bool _mapSpawned;
        private bool _guardianDefeated;
        private bool _completionCacheSpawned;
        private ArpgMapItem30 _activeMap;
        private DifficultySettings _activeDifficulty;
        private int _lastEnemyCount;
        private float _mapStartedAt;
        private float _timedMapRemaining;
        private bool _timedMapActive;
        private float _nextTimedDamage;
        private float _lastAutosave;
        private float _playtimeAccumulator;
        private string _lastMessage = "Arcane Engine 3.1 · First Descent ready.";
        private readonly List<string> _rewardLines = new List<string>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (Instance != null) return;
            GameObject host = new GameObject("Arcane Engine 3.1 · First Descent");
            DontDestroyOnLoad(host);
            host.AddComponent<ArpgFoundation30>();
            host.AddComponent<ArpgFrontend31>();
            host.AddComponent<ArpgInterface30>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ArpgContent30.Ensure();
            ArpgRosterStore31.Initialize();
            Profile = null;
            StartCoroutine(InitializeWorldWhenReady());
        }

        private IEnumerator InitializeWorldWhenReady()
        {
            float timeout = Time.realtimeSinceStartup + 20f;
            while (GameWorld.Instance == null && Time.realtimeSinceStartup < timeout) yield return null;
            _world = GameWorld.Instance;
            _worldReady = _world != null;
            if (!_worldReady)
            {
                _lastMessage = "Arcane Engine 3.1 could not find GameWorld.";
                Debug.LogError(_lastMessage);
                yield break;
            }
            _world.RunActive = false;
            _world.ClearTransientObjects();
        }

        private void Update()
        {
            if (Profile != null)
            {
                _playtimeAccumulator += Time.unscaledDeltaTime;
                if (_playtimeAccumulator >= 1f)
                {
                    long seconds = (long)_playtimeAccumulator;
                    Profile.totalPlaySeconds += seconds;
                    _playtimeAccumulator -= seconds;
                }
                if (Time.unscaledTime - _lastAutosave >= 45f)
                {
                    _lastAutosave = Time.unscaledTime;
                    SaveActiveProfile("Autosave complete.", false);
                }
            }

            if (!_mapActive) return;
            if (_world == null) _world = GameWorld.Instance;
            if (_world == null || _world.Player == null) return;

            if (_world.Player.Health <= 0f)
            {
                FailMap("You died. The active map was consumed; carried rewards and character progress were retained.");
                return;
            }

            int enemyCount = _world.Enemies.Count(value => value != null && !value.IsDead && value.GetComponent<ArpgTrainingTarget31>() == null);
            if (enemyCount < _lastEnemyCount)
            {
                int kills = _lastEnemyCount - enemyCount;
                GrantExperience(kills * Mathf.Max(3, 4 + _activeMap.tier * 2), false);
                Vector3 dropPosition = _world.Player == null ? Vector3.zero : _world.Player.transform.position + Vector3.forward * 2f;
                ArpgLoot31.SpawnKillDrops(Profile, _activeMap, kills, dropPosition);
            }
            _lastEnemyCount = enemyCount;

            if (_timedMapActive)
            {
                _timedMapRemaining -= Time.deltaTime;
                if (_timedMapRemaining <= 0f && Time.time >= _nextTimedDamage)
                {
                    _nextTimedDamage = Time.time + 1f;
                    _world.Player.TakeDamage(8f + _activeMap.tier * 2.25f);
                }
            }

            if (_mapSpawned && enemyCount == 0 && !_completionCacheSpawned && Time.time - _mapStartedAt > 1f)
                SpawnCompletionCache();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && Profile != null) SaveActiveProfile("Saved on pause.", false);
        }

        private void OnApplicationQuit()
        {
            if (Profile != null) SaveActiveProfile("Saved on exit.", false);
        }

        public bool InitializeNewCharacter(ArpgProfile30 profile, out string message)
        {
            message = string.Empty;
            if (!PrepareActivation(profile, out message)) return false;
            if (profile.characterClass == ArpgClass30.Unchosen)
            {
                message = "A class must be selected before entering the Refuge.";
                return false;
            }

            if (!profile.starterLoadoutInitialized)
            {
                if (!ArpgLegacyBridge30.InitializeFreshCharacter(_world, profile, out message)) return false;
            }
            Profile = profile;
            Profile.currentLocation = "Astral Refuge";
            Profile.lastPlayedUtc = DateTime.UtcNow.ToString("O");
            DiscoverAvailableConstellations();
            SaveActiveProfile("Character created.", false);
            EnterRefuge(true);
            message = "Awakened " + Profile.characterName + " in the Astral Refuge.";
            _lastMessage = message;
            return true;
        }

        public bool ActivateProfile(ArpgProfile30 profile, out string message)
        {
            message = string.Empty;
            if (!PrepareActivation(profile, out message)) return false;
            Profile = profile;
            ArpgProfileStore30.Repair(Profile);
            Profile.lastPlayedUtc = DateTime.UtcNow.ToString("O");
            if (!Profile.starterLoadoutInitialized)
            {
                if (!ArpgLegacyBridge30.InitializeFreshCharacter(_world, Profile, out message))
                {
                    Profile = null;
                    return false;
                }
            }
            else ArpgLegacyBridge30.RestoreOwnedDiscoveries(_world, Profile);
            DiscoverAvailableConstellations();
            EnterRefuge(false);
            SaveActiveProfile("Character loaded.", false);
            message = "Loaded " + Profile.characterName + " in the Astral Refuge.";
            _lastMessage = message;
            return true;
        }

        private bool PrepareActivation(ArpgProfile30 profile, out string message)
        {
            message = string.Empty;
            if (profile == null)
            {
                message = "The selected character profile is invalid.";
                return false;
            }
            if (!_worldReady || _world == null || _world.Player == null)
            {
                _world = GameWorld.Instance;
                _worldReady = _world != null && _world.Player != null;
            }
            if (!_worldReady)
            {
                message = "The game world is not ready.";
                return false;
            }
            if (Profile != null && Profile.characterId != profile.characterId) SaveActiveProfile("Previous character saved.", false);
            ArpgRosterStore31.SetActiveCharacter(profile.characterId);
            return true;
        }

        public void DeactivateProfile()
        {
            if (Profile != null) SaveActiveProfile("Character saved.", false);
            _mapActive = false;
            _mapSpawned = false;
            _guardianDefeated = false;
            _completionCacheSpawned = false;
            _activeMap = null;
            ArpgLoot31.ClearAll();
            ArpgRefuge31.Clear();
            if (_world != null)
            {
                _world.RunActive = false;
                _world.ClearTransientObjects();
            }
            Profile = null;
            ArpgRosterStore31.SetActiveCharacter(string.Empty);
            if (ArpgInterface30.Instance != null) ArpgInterface30.Instance.ClosePanel();
        }

        private void EnterRefuge(bool fresh)
        {
            _mapActive = false;
            _mapSpawned = false;
            _guardianDefeated = false;
            _completionCacheSpawned = false;
            _activeMap = null;
            _timedMapActive = false;
            _timedMapRemaining = 0f;
            ArpgLoot31.ClearAll();
            _world.RunActive = false;
            _world.ClearTransientObjects();
            _world.BuildSanctuaryEnvironment();
            ArpgLegacyBridge30.RestoreOwnedDiscoveries(_world, Profile);
            _world.RecalculateStats(false);
            if (_world.Player != null)
            {
                _world.Player.ResetForRun();
                _world.Player.transform.position = new Vector3(0f, 1f, -1.5f);
                _world.Player.GrantSpawnProtection(3f);
            }
            ArpgRefuge31.Build(_world);
            Profile.currentLocation = "Astral Refuge";
            _lastMessage = fresh ? "Approach the SpellForge Altar." : "Returned to the Astral Refuge.";
            _world.Log(_lastMessage);
        }

        public bool LaunchFreeTierZero(out string message)
        {
            int seed = ArpgDeterminism30.Combine(Environment.TickCount, Profile == null ? 0 : Profile.totalMapsCompleted, Profile == null ? -1 : Profile.highestCompletedTier);
            ArpgMapItem30 map = ArpgItems30.GenerateMap(0, seed, ArpgMapRarity30.Normal, false);
            map.instanceId = "free-tier-zero-" + ArpgDeterminism30.Positive(seed);
            return LaunchMapInternal(map, false, out message);
        }

        public bool LaunchMap(ArpgMapItem30 map, out string message)
        {
            return LaunchMapInternal(map, true, out message);
        }

        private bool LaunchMapInternal(ArpgMapItem30 map, bool consume, out string message)
        {
            message = string.Empty;
            if (Profile == null) { message = "Load a character first."; return false; }
            if (_mapActive) { message = "A map is already active."; return false; }
            ArpgMapDefinition30 definition = map == null ? null : ArpgContent30.Map(map.mapId);
            if (map == null || definition == null || !definition.playableIn31 || map.tier > 5)
            {
                message = "3.1.0 currently opens the playable White Map network from Tier 0 through Tier 5.";
                return false;
            }
            if (map.tier > Mathf.Max(0, Profile.highestCompletedTier + 1) && !Profile.maps.Any(value => value != null && value.instanceId == map.instanceId))
            {
                message = "Complete connected lower-tier maps first.";
                return false;
            }
            _world = GameWorld.Instance;
            if (_world == null || _world.Player == null) { message = "The game world is not ready."; return false; }

            if (consume)
            {
                ArpgMapItem30 stored = Profile.maps.FirstOrDefault(value => value != null && value.instanceId == map.instanceId);
                if (stored == null) { message = "That map is not in the persistent map inventory."; return false; }
                Profile.maps.Remove(stored);
                ArpgArsenalStore32.Save();
            }

            _activeMap = map;
            _activeDifficulty = BuildDifficulty(map);
            _rewardLines.Clear();
            _guardianDefeated = false;
            _completionCacheSpawned = false;
            ArpgRefuge31.Clear();
            ArpgLoot31.BeginMap();
            _world.ClearTransientObjects();
            _world.RunActive = true;
            _world.RecalculateStats(false);
            _world.Player.ResetForRun();
            _world.Player.transform.position = new Vector3(0f, 1f, -8f);
            _world.Player.GrantSpawnProtection(2.5f);

            RoomTemplate room = SelectRoom(definition.layoutIndex, map.seed);
            if (room == null)
            {
                if (consume) Profile.maps.Add(map);
                _world.RunActive = false;
                message = "No combat room was available.";
                EnterRefuge(false);
                return false;
            }

            _world.BuildRoom(room, map.tier + 1, map.seed);
            SpawnMapEnemies(map, definition);
            _lastEnemyCount = _world.Enemies.Count(value => value != null && !value.IsDead && value.GetComponent<ArpgTrainingTarget31>() == null);
            _mapActive = true;
            _mapSpawned = true;
            _mapStartedAt = Time.time;
            _timedMapActive = map.affixIds.Any(value =>
            {
                ArpgMapAffixDefinition30 affix = ArpgContent30.MapAffix(value);
                return affix != null && (affix.difficultyFlag == "timedRooms" || affix.id.IndexOf("timed", StringComparison.OrdinalIgnoreCase) >= 0);
            });
            _timedMapRemaining = _timedMapActive ? Mathf.Max(90f, 190f - map.tier * 12f) : 0f;
            _nextTimedDamage = Time.time;
            Profile.currentLocation = definition.displayName;
            SaveActiveProfile("Map opened.", false);
            message = "Opened " + definition.displayName + " · Tier " + map.tier + " · " + map.rarity + ".";
            _lastMessage = message;
            _world.Log(message);
            return true;
        }

        private RoomTemplate SelectRoom(int layoutIndex, int seed)
        {
            List<RoomTemplate> rooms = MegaCatalog.AllRooms.Where(value => value != null && value.type == DungeonRoomType.Combat).ToList();
            if (rooms.Count == 0) rooms = MegaCatalog.AllRooms.Where(value => value != null).ToList();
            if (rooms.Count == 0) return null;
            int index = ArpgDeterminism30.Index(ArpgDeterminism30.Combine(seed, layoutIndex, 31), rooms.Count);
            return rooms[index];
        }

        private void SpawnMapEnemies(ArpgMapItem30 map, ArpgMapDefinition30 definition)
        {
            Array values = Enum.GetValues(typeof(EnemyArchetype));
            List<EnemyArchetype> normalPool = new List<EnemyArchetype>();
            List<EnemyArchetype> bossPool = new List<EnemyArchetype>();
            ArpgMonsterFamilyDefinition31 family = ArpgContent30.MonsterFamily(definition.monsterFamily);
            foreach (EnemyArchetype value in values)
            {
                string name = value.ToString().ToLowerInvariant();
                if (name.Contains("training")) continue;
                if (name.Contains("warden") || name.Contains("boss") || name.Contains("titan")) bossPool.Add(value);
                else if (family == null || family.archetypeHints.Count == 0 || family.archetypeHints.Any(hint => name.Contains(hint.ToLowerInvariant()))) normalPool.Add(value);
            }
            if (normalPool.Count < 3)
            {
                foreach (EnemyArchetype value in values)
                {
                    string name = value.ToString().ToLowerInvariant();
                    if (!name.Contains("training") && !name.Contains("boss") && !name.Contains("warden") && !name.Contains("titan") && !normalPool.Contains(value)) normalPool.Add(value);
                }
            }
            if (normalPool.Count == 0) normalPool.Add((EnemyArchetype)values.GetValue(0));
            if (bossPool.Count == 0) bossPool.Add(normalPool[normalPool.Count - 1]);

            System.Random random = new System.Random(map.seed);
            bool increasedPacks = map.affixIds.Any(id => { ArpgMapAffixDefinition30 value = ArpgContent30.MapAffix(id); return value != null && value.difficultyFlag == "pack-size"; });
            bool moreMagic = map.affixIds.Any(id => { ArpgMapAffixDefinition30 value = ArpgContent30.MapAffix(id); return value != null && value.difficultyFlag == "magic-enemies"; });
            bool moreRare = map.affixIds.Any(id => { ArpgMapAffixDefinition30 value = ArpgContent30.MapAffix(id); return value != null && value.difficultyFlag == "rare-enemies"; });
            bool empoweredBoss = map.affixIds.Any(id => { ArpgMapAffixDefinition30 value = ArpgContent30.MapAffix(id); return value != null && value.difficultyFlag == "boss-power"; });
            int normalCount = Mathf.Clamp(7 + map.tier * 2 + map.affixIds.Count * 2 + (increasedPacks ? 6 : 0), 7, 28);
            for (int index = 0; index < normalCount; index++)
            {
                float angle = index / (float)Mathf.Max(1, normalCount) * Mathf.PI * 2f + (float)random.NextDouble() * 0.35f;
                float radius = 4.5f + (index % 4) * 1.8f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0.75f, Mathf.Sin(angle) * radius + 3f);
                EnemyArchetype archetype = normalPool[random.Next(normalPool.Count)];
                bool elite = (map.tier >= 2 && index > 0 && index % Mathf.Max(3, 7 - map.tier) == 0) || (moreMagic && index > 0 && index % 4 == 0) || (moreRare && index > 0 && index % 7 == 0);
                EnemyController spawned = EnemyController.Spawn(position, archetype, Mathf.Max(1, map.tier + 1), _activeDifficulty, elite, false);
                if (spawned != null)
                {
                    List<ArpgMonsterVariantDefinition31> variants = ArpgContent30.MonsterVariants.Where(value => value.familyId == definition.monsterFamily).ToList();
                    string variantId = variants.Count == 0 ? string.Empty : variants[ArpgDeterminism30.Index(map.seed + index * 43, variants.Count)].id;
                    ArpgEncounter31.ConfigureMonster(spawned, definition.monsterFamily, variantId, map.tier, elite);
                }
            }

            EnemyArchetype bossArchetype = bossPool[ArpgDeterminism30.Index(map.seed / 17, bossPool.Count)];
            EnemyController boss = EnemyController.Spawn(new Vector3(0f, 0.75f, 10f), bossArchetype, Mathf.Max(2, map.tier + 2), _activeDifficulty, true, true);
            if (boss != null)
            {
                ArpgEncounter31.ConfigureMonster(boss, definition.monsterFamily, map.tier, true);
                ArpgEncounter31.ConfigureBoss(boss, ArpgContent30.Boss(definition.bossId), map.tier, map.rarity == ArpgMapRarity30.Rare || empoweredBoss);
            }
        }

        private DifficultySettings BuildDifficulty(ArpgMapItem30 map)
        {
            DifficultySettings difficulty = new DifficultySettings();
            foreach (string affixId in map.affixIds)
            {
                ArpgMapAffixDefinition30 definition = ArpgContent30.MapAffix(affixId);
                if (definition == null || string.IsNullOrEmpty(definition.difficultyFlag)) continue;
                string flag = definition.difficultyFlag;
                string legacyFlag = flag;
                if (flag == "monster-speed") legacyFlag = "frenziedEnemies";
                else if (flag == "monster-damage" || flag == "monster-critical-damage" || flag == "monster-critical") legacyFlag = "glassSoul";
                else if (flag == "monster-life" || flag == "monster-armour" || flag == "monster-evasion" || flag == "monster-ward" || flag == "monster-barrier" || flag == "monster-resistance" || flag == "reaction-resistance" || flag == "control-resistance") legacyFlag = "bulwarkEnemies";
                else if (flag == "reduced-recovery") legacyFlag = "reducedHealing";
                else if (flag == "increased-cost") legacyFlag = "manaDrought";
                else if (flag == "fire-ground" || flag == "cold-ground" || flag == "lightning-ground" || flag == "blood-ground" || flag == "toxic-ground" || flag == "void-ground" || flag == "hazards") legacyFlag = "unstableWorld";
                else if (flag == "magic-enemies" || flag == "rare-enemies" || flag == "specialists") legacyFlag = "extraEliteAffixes";
                else if (flag == "boss-power") legacyFlag = "newBossPhase";
                System.Reflection.FieldInfo field = typeof(DifficultySettings).GetField(legacyFlag);
                if (field != null && field.FieldType == typeof(bool)) field.SetValue(difficulty, true);
            }
            return difficulty;
        }

        private void SpawnCompletionCache()
        {
            if (_activeMap == null || _completionCacheSpawned) return;
            _guardianDefeated = true;
            _completionCacheSpawned = true;
            ArpgMapDefinition30 definition = ArpgContent30.Map(_activeMap.mapId);
            Vector3 position = _world.Player == null ? new Vector3(0f, 0.8f, 8f) : _world.Player.transform.position + Vector3.forward * 3.2f;
            ArpgLoot31.SpawnCompletionCache(definition, position, CompleteMap);
            _lastMessage = "Guardian defeated. Secure the Guardian Cache to complete the map.";
            if (_world != null) _world.Log(_lastMessage);
        }

        private void CompleteMap()
        {
            if (!_mapActive || _activeMap == null || Profile == null) return;
            ArpgMapItem30 map = _activeMap;
            ArpgMapDefinition30 definition = ArpgContent30.Map(map.mapId);
            bool firstCompletion = !Profile.completedMapIds.Contains(map.mapId);
            bool mastery = MeetsMastery(map, definition);
            bool firstMastery = mastery && !Profile.masteredMapIds.Contains(map.mapId);
            if (firstCompletion) Profile.completedMapIds.Add(map.mapId);
            if (firstMastery) Profile.masteredMapIds.Add(map.mapId);
            Profile.highestCompletedTier = Mathf.Max(Profile.highestCompletedTier, map.tier);
            Profile.totalMapsCompleted++;
            if (firstMastery) Profile.atlasPoints++;

            float rewardMultiplier = ArpgStatHooks30.MapRewardMultiplier(Profile, map);
            int completionExperience = Mathf.RoundToInt((42f + map.tier * 28f) * rewardMultiplier * (1f + ArpgStatHooks30.ExperienceBonus(Profile)));
            GrantExperience(completionExperience, true);
            _rewardLines.Add("Experience +" + completionExperience);

            int itemCount = map.tier == 0 ? 2 : Mathf.Clamp(1 + (map.rarity == ArpgMapRarity30.Rare ? 1 : 0), 1, 3);
            for (int index = 0; index < itemCount; index++)
            {
                ArpgItem30 item = ArpgArsenalRuntime32.GenerateCompletionReward(Profile, map, definition, index, firstMastery);
                string placementMessage;
                if (ArpgArsenalRuntime32.GrantRewardItem(Profile, item, out placementMessage)) _rewardLines.Add(item.displayName);
                else _rewardLines.Add(placementMessage);
            }

            int currencyCount = Mathf.Clamp(1 + map.tier / 2 + (map.rarity == ArpgMapRarity30.Rare ? 1 : 0), 1, 4);
            for (int index = 0; index < currencyCount; index++)
            {
                ArpgCurrency30 currency = RollCoreCurrency(map.tier, map.seed + index * 131);
                Profile.AddCurrency(currency, 1);
                ArpgArsenalRuntime32.AddLegacyCurrency(currency, 1);
                _rewardLines.Add(ArpgItems30.CurrencyName(currency));
            }

            if (map.tier == 0 && Profile.ownedRuneIds.Count == 0)
            {
                string rune = ArpgLegacyBridge30.GrantRandomRune(_world, Profile, map.seed + 17);
                if (!string.IsNullOrEmpty(rune)) _rewardLines.Add("Support Rune: " + rune);
            }
            else if (map.tier >= 2 && ArpgDeterminism30.Positive(map.seed) % 3 == 0)
            {
                string rune = ArpgLegacyBridge30.GrantRandomRune(_world, Profile, map.seed + 19);
                if (!string.IsNullOrEmpty(rune)) _rewardLines.Add("Support Rune: " + rune);
            }
            if (map.tier >= 3 && (Profile.ownedCoreIds.Count < 2 || ArpgDeterminism30.Positive(map.seed) % 5 == 0))
            {
                string core = ArpgLegacyBridge30.GrantRandomCore(_world, Profile, map.seed + 23);
                if (!string.IsNullOrEmpty(core)) _rewardLines.Add("Spell Core: " + core);
            }

            GrantConnectedMap(map, definition, firstCompletion);
            DiscoverAvailableConstellations();
            Profile.currentLocation = "Astral Refuge";
            ArpgArsenalStore32.Save();
            SaveActiveProfile("Map rewards saved.", false);
            _lastMessage = "Completed " + (definition == null ? map.mapId : definition.displayName) + (firstMastery ? " · Mastery achieved" : string.Empty) + ". " + string.Join(" · ", _rewardLines.Take(6).ToArray());
            ReturnToRefuge(false);
        }

        private void GrantConnectedMap(ArpgMapItem30 completed, ArpgMapDefinition30 definition, bool firstCompletion)
        {
            if (completed.tier >= 5 || definition == null) return;
            List<ArpgMapDefinition30> connected = definition.connectedMapIds
                .Select(ArpgContent30.Map)
                .Where(value => value != null && value.playableIn31 && value.tier == completed.tier + 1)
                .ToList();
            if (connected.Count == 0)
                connected = ArpgContent30.Maps.Where(value => value.playableIn31 && value.tier == completed.tier + 1).ToList();
            if (connected.Count == 0) return;

            if (firstCompletion || !Profile.maps.Any(value => value != null && value.tier >= completed.tier))
            {
                ArpgMapDefinition30 next = connected[ArpgDeterminism30.Index(completed.seed + Profile.totalMapsCompleted, connected.Count)];
                ArpgMapRarity30 rarity = next.tier >= 4 ? ArpgMapRarity30.Rare : next.tier >= 2 ? ArpgMapRarity30.Magic : ArpgMapRarity30.Normal;
                ArpgMapItem30 drop = ArpgItems30.GenerateMap(next.tier, completed.seed + 100003, rarity, false);
                drop.mapId = next.id;
                Profile.maps.Add(drop);
                _rewardLines.Add("Connected map: T" + next.tier + " " + next.displayName);
            }
        }

        private bool MeetsMastery(ArpgMapItem30 map, ArpgMapDefinition30 definition)
        {
            if (definition == null) return false;
            switch (definition.masteryRule)
            {
                case "boss": return true;
                case "no-death": return true;
                case "magic": return map.rarity >= ArpgMapRarity30.Magic;
                case "modified": return map.affixIds.Count > 0;
                case "rare": return map.rarity >= ArpgMapRarity30.Rare;
                case "rare-timed": return map.rarity >= ArpgMapRarity30.Rare && (!_timedMapActive || _timedMapRemaining > 0f);
                default: return true;
            }
        }

        private static ArpgCurrency30 RollCoreCurrency(int tier, int seed)
        {
            int roll = ArpgDeterminism30.Positive(seed) % 100;
            if (tier >= 4 && roll < 16) return ArpgCurrency30.ArcaneExalt;
            if (tier >= 3 && roll < 34) return ArpgCurrency30.SigilOfElevation;
            if (roll < 52) return ArpgCurrency30.ReformationOrb;
            if (roll < 72) return ArpgCurrency30.RuneOfAugmentation;
            return ArpgCurrency30.SparkOfAlteration;
        }

        public void FailMap(string reason, bool countDeath = true)
        {
            if (!_mapActive || Profile == null) return;
            if (countDeath) Profile.totalDeaths++;
            _lastMessage = reason;
            SaveActiveProfile("Map failure saved.", false);
            ReturnToRefuge(true);
        }

        public void AbandonMap()
        {
            if (_mapActive) FailMap("Map abandoned. The map item was consumed.", false);
        }

        private void ReturnToRefuge(bool failure)
        {
            _mapActive = false;
            _mapSpawned = false;
            _guardianDefeated = false;
            _completionCacheSpawned = false;
            _activeMap = null;
            _timedMapRemaining = 0f;
            _timedMapActive = false;
            ArpgLoot31.ClearAll();
            if (_world == null) return;
            EnterRefuge(false);
            _world.Log(failure ? "Returned to the Astral Refuge after map failure." : "Map complete. Rewards secured in the Astral Refuge.");
        }

        public void GrantExperience(int amount, bool announce)
        {
            if (Profile == null || Profile.level >= 100 || amount <= 0) return;
            Profile.experience += amount;
            int gained = 0;
            while (Profile.level < 100 && Profile.experience >= Profile.ExperienceToNextLevel)
            {
                int requirement = Profile.ExperienceToNextLevel;
                Profile.experience -= requirement;
                Profile.level++;
                Profile.constellationPoints++;
                gained++;
            }
            if (gained > 0)
            {
                DiscoverAvailableConstellations();
                if (_world != null)
                {
                    _world.RecalculateStats(true);
                    if (_world.Player != null) _world.Player.RestoreBetweenRooms(0.2f, 0.35f);
                    if (announce) _world.Log("LEVEL " + Profile.level + " · +" + gained + " Constellation Point(s).");
                }
            }
        }

        public void DiscoverAvailableConstellations()
        {
            if (Profile == null) return;
            foreach (ArpgConstellationDefinition30 constellation in ArpgContent30.Constellations)
            {
                if (Profile.level < constellation.requiredLevel) continue;
                if (Profile.highestCompletedTier < constellation.requiredTier - 1) continue;
                if (!Profile.discoveredConstellations.Contains(constellation.id)) Profile.discoveredConstellations.Add(constellation.id);
            }
        }

        public bool AllocateConstellationNode(string nodeId, out string message)
        {
            message = string.Empty;
            if (Profile == null) { message = "Load a character first."; return false; }
            ArpgConstellationDefinition30 constellation = ArpgContent30.Constellations.FirstOrDefault(value => value.nodes.Any(node => node.id == nodeId));
            ArpgConstellationNodeDefinition30 node = ArpgContent30.ConstellationNode(nodeId);
            if (constellation == null || node == null) { message = "Unknown constellation node."; return false; }
            if (!Profile.discoveredConstellations.Contains(constellation.id)) { message = "That Constellation has not been discovered."; return false; }
            if (Profile.allocatedConstellationNodes.Contains(node.id)) { message = "That Star is already allocated."; return false; }
            if (!string.IsNullOrEmpty(node.prerequisiteId) && !Profile.allocatedConstellationNodes.Contains(node.prerequisiteId)) { message = "Allocate the previous Star first."; return false; }
            if (Profile.constellationPoints < node.pointCost) { message = "Requires " + node.pointCost + " Constellation Point(s)."; return false; }
            if (node.size == ArpgNodeSize30.Completion && ArpgStatHooks30.AttunementUsed(Profile) + constellation.attunementCost > ArpgStatHooks30.AttunementMaximum(Profile))
            {
                message = "Insufficient Attunement for this Completion Boon.";
                return false;
            }
            Profile.constellationPoints -= node.pointCost;
            Profile.allocatedConstellationNodes.Add(node.id);
            SaveActiveProfile("Constellation allocation saved.", false);
            if (_world != null) _world.RecalculateStats(true);
            message = "Allocated " + node.displayName + ".";
            return true;
        }

        public bool ResetConstellations(out string message)
        {
            message = string.Empty;
            if (Profile == null) { message = "Load a character first."; return false; }
            if (Profile.allocatedConstellationNodes.Count == 0) { message = "No Stars are allocated."; return false; }
            int cost = Mathf.Max(1, Profile.allocatedConstellationNodes.Count / 10);
            if (!Profile.SpendCurrency(ArpgCurrency30.NullOrb, cost)) { message = "Reset requires " + cost + " Null Orb(s)."; return false; }
            int refunded = Profile.allocatedConstellationNodes.Sum(value =>
            {
                ArpgConstellationNodeDefinition30 node = ArpgContent30.ConstellationNode(value);
                return node == null ? 0 : node.pointCost;
            });
            Profile.allocatedConstellationNodes.Clear();
            Profile.constellationPoints += refunded;
            SaveActiveProfile("Constellations reset.", false);
            if (_world != null) _world.RecalculateStats(true);
            message = "Constellations reset. " + refunded + " point(s) refunded.";
            return true;
        }

        public bool ChooseAscendancy(ArpgAscendancy30 ascendancy, out string message)
        {
            message = "Ascendancy unlocking begins after the White Map vertical slice and is preview-only in 3.1.0.";
            return false;
        }

        public bool AllocateAscendancyNode(string nodeId, out string message)
        {
            message = "Ascendancy nodes are preview-only in 3.1.0.";
            return false;
        }

        public void EquipItem(ArpgItem30 item)
        {
            if (Profile == null || item == null) return;
            string result;
            if (Profile.Equip(item, out result))
            {
                SaveActiveProfile("Equipment saved.", false);
                if (_world != null) _world.RecalculateStats(true);
            }
            _lastMessage = result;
        }

        public bool CraftItem(ArpgItem30 item, ArpgCurrency30 currency, out string message)
        {
            if (Profile == null) { message = "Load a character first."; return false; }
            bool result = ArpgItems30.CraftItem(Profile, item, currency, out message);
            if (result)
            {
                SaveActiveProfile("Crafting saved.", false);
                if (_world != null) _world.RecalculateStats(true);
            }
            _lastMessage = message;
            return result;
        }

        public bool CraftMap(ArpgMapItem30 map, ArpgCurrency30 currency, out string message)
        {
            if (Profile == null) { message = "Load a character first."; return false; }
            bool result = ArpgItems30.CraftMap(Profile, map, currency, out message);
            if (result) SaveActiveProfile("Map crafting saved.", false);
            _lastMessage = message;
            return result;
        }

        private bool SaveActiveProfile(string status, bool announce)
        {
            if (Profile == null) return false;
            Profile.lastPlayedUtc = DateTime.UtcNow.ToString("O");
            bool success = ArpgProfileStore30.Save(Profile);
            if (success && !string.IsNullOrEmpty(status))
            {
                _lastMessage = status;
                if (announce && ArpgInterface30.Instance != null) ArpgInterface30.Instance.SetMessage(status);
            }
            else if (!success)
            {
                _lastMessage = "Save failed. Existing backups were preserved.";
                if (ArpgInterface30.Instance != null) ArpgInterface30.Instance.SetMessage(_lastMessage);
            }
            return success;
        }
    }
}
