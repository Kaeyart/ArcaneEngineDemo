using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ArcaneEngine
{
    public sealed class ArpgFoundation30 : MonoBehaviour
    {
        public static ArpgFoundation30 Instance { get; private set; }
        public static ArpgProfile30 Profile { get; private set; }

        public bool MapActive { get { return _mapActive; } }
        public ArpgMapItem30 ActiveMap { get { return _activeMap; } }
        public string LastMessage { get { return _lastMessage; } }
        public float TimedMapRemaining { get { return _timedMapRemaining; } }

        private GameWorld _world;
        private bool _mapActive;
        private bool _mapSpawned;
        private ArpgMapItem30 _activeMap;
        private DifficultySettings _activeDifficulty;
        private int _lastEnemyCount;
        private float _mapStartedAt;
        private float _timedMapRemaining;
        private bool _timedMapActive;
        private float _nextTimedDamage;
        private string _lastMessage = "Arcane Engine 3.0 foundation loaded.";
        private readonly List<string> _rewardLines = new List<string>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (Instance != null) return;
            GameObject host = new GameObject("Arcane Engine 3.0 · Endgame Foundation");
            DontDestroyOnLoad(host);
            host.AddComponent<ArpgFoundation30>();
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
            Profile = ArpgProfileStore30.Load();
            DiscoverAvailableConstellations();
            StartCoroutine(InitializeWorldWhenReady());
        }

        private IEnumerator InitializeWorldWhenReady()
        {
            float timeout = Time.realtimeSinceStartup + 12f;
            while (GameWorld.Instance == null && Time.realtimeSinceStartup < timeout) yield return null;
            _world = GameWorld.Instance;
            if (_world == null)
            {
                _lastMessage = "Arcane Engine 3.0 could not find GameWorld.";
                yield break;
            }
            yield return null;
            if (Profile.characterClass != ArpgClass30.Unchosen && Profile.starterLoadoutInitialized)
            {
                ArpgLegacyBridge30.RestoreOwnedDiscoveries(_world, Profile);
                _world.RunActive = false;
                _world.BuildSanctuaryEnvironment();
                _world.RecalculateStats(false);
                if (_world.Player != null) _world.Player.ResetForRun();
                _lastMessage = "Returned to the Astral Refuge. Press F7 to open the Atlas.";
            }
        }

        private void Update()
        {
            if (!_mapActive) return;
            if (_world == null) _world = GameWorld.Instance;
            if (_world == null || _world.Player == null) return;

            if (_world.Player.Health <= 0f)
            {
                FailMap("You died. The map was consumed and its progress was lost.");
                return;
            }

            int enemyCount = _world.Enemies.Count(value => value != null && !value.IsDead);
            if (enemyCount < _lastEnemyCount)
            {
                int kills = _lastEnemyCount - enemyCount;
                GrantExperience(kills * Mathf.Max(2, 2 + _activeMap.tier / 4), false);
            }
            _lastEnemyCount = enemyCount;

            if (_timedMapActive)
            {
                _timedMapRemaining -= Time.deltaTime;
                if (_timedMapRemaining <= 0f && Time.time >= _nextTimedDamage)
                {
                    _nextTimedDamage = Time.time + 1f;
                    _world.Player.TakeDamage(8f + _activeMap.tier * 0.55f);
                }
            }

            if (_mapSpawned && enemyCount == 0 && Time.time - _mapStartedAt > 0.75f) CompleteMap();
        }

        public bool ChooseClass(ArpgClass30 characterClass, out string message)
        {
            message = string.Empty;
            if (characterClass == ArpgClass30.Unchosen)
            {
                message = "Choose Mage, Ranger, or Warrior.";
                return false;
            }
            if (Profile.characterClass != ArpgClass30.Unchosen)
            {
                message = "This character has already chosen a class.";
                return false;
            }
            _world = GameWorld.Instance;
            if (_world == null)
            {
                message = "GameWorld is not ready.";
                return false;
            }
            Profile.characterClass = characterClass;
            Profile.level = 0;
            Profile.experience = 0;
            Profile.constellationPoints = 0;
            Profile.atlasPoints = 0;
            Profile.ascendancyPoints = 0;
            Profile.highestCompletedTier = -1;
            Profile.totalMapsCompleted = 0;
            Profile.totalDeaths = 0;
            Profile.maps.Clear();
            Profile.items.Clear();
            Profile.equipped.Clear();
            Profile.currencies.Clear();
            Profile.completedMapIds.Clear();
            Profile.masteredMapIds.Clear();
            Profile.allocatedConstellationNodes.Clear();
            Profile.discoveredConstellations.Clear();
            Profile.allocatedAscendancyNodes.Clear();
            Profile.ascendancy = ArpgAscendancy30.None;
            DiscoverAvailableConstellations();
            if (!ArpgLegacyBridge30.InitializeFreshCharacter(_world, Profile, out message))
            {
                Profile.characterClass = ArpgClass30.Unchosen;
                return false;
            }
            ArpgProfileStore30.Save(Profile);
            _lastMessage = message + " Tier 0 maps are freely available from the Atlas.";
            return true;
        }

        public bool LaunchFreeTierZero(out string message)
        {
            int seed = Mathf.Abs(Environment.TickCount ^ Profile.totalMapsCompleted * 7919);
            ArpgMapItem30 map = ArpgItems30.GenerateMap(0, seed, ArpgMapRarity30.Normal, false);
            map.instanceId = "free-tier-zero-" + seed;
            return LaunchMapInternal(map, false, out message);
        }

        public bool LaunchMap(ArpgMapItem30 map, out string message)
        {
            return LaunchMapInternal(map, true, out message);
        }

        private bool LaunchMapInternal(ArpgMapItem30 map, bool consume, out string message)
        {
            message = string.Empty;
            if (Profile.characterClass == ArpgClass30.Unchosen)
            {
                message = "Choose a class first.";
                return false;
            }
            if (_mapActive)
            {
                message = "A map is already active.";
                return false;
            }
            if (map == null || ArpgContent30.Map(map.mapId) == null)
            {
                message = "That map item is invalid.";
                return false;
            }
            if (map.tier > Mathf.Max(0, Profile.highestCompletedTier + 2))
            {
                message = "Complete lower map tiers before opening this map.";
                return false;
            }
            _world = GameWorld.Instance;
            if (_world == null || _world.Player == null)
            {
                message = "The game world is not ready.";
                return false;
            }
            if (consume)
            {
                ArpgMapItem30 stored = Profile.maps.FirstOrDefault(value => value.instanceId == map.instanceId);
                if (stored == null)
                {
                    message = "That map is not in the persistent map inventory.";
                    return false;
                }
                Profile.maps.Remove(stored);
            }

            _activeMap = map;
            _activeDifficulty = BuildDifficulty(map);
            _rewardLines.Clear();
            _world.ClearTransientObjects();
            _world.RunActive = true;
            _world.RecalculateStats(false);
            _world.Player.ResetForRun();
            _world.Player.transform.position = new Vector3(0f, 1f, -8f);
            _world.Player.GrantSpawnProtection(2f);

            RoomTemplate room = SelectRoom(map.seed);
            if (room == null)
            {
                if (consume) Profile.maps.Add(map);
                _world.RunActive = false;
                message = "No combat room was available.";
                return false;
            }
            _world.BuildRoom(room, map.tier + 1, map.seed);
            SpawnMapEnemies(map);
            _lastEnemyCount = _world.Enemies.Count(value => value != null && !value.IsDead);
            _mapActive = true;
            _mapSpawned = true;
            _mapStartedAt = Time.time;
            _timedMapActive = map.affixIds.Any(value =>
            {
                ArpgMapAffixDefinition30 affix = ArpgContent30.MapAffix(value);
                return affix != null && affix.difficultyFlag == "timedRooms";
            });
            _timedMapRemaining = _timedMapActive ? Mathf.Max(75f, 180f - map.tier * 2f) : 0f;
            _nextTimedDamage = Time.time;
            ArpgProfileStore30.Save(Profile);
            ArpgMapDefinition30 definition = ArpgContent30.Map(map.mapId);
            message = "Opened " + definition.displayName + " · Tier " + map.tier + " · " + map.rarity + (map.corrupted ? " · Corrupted" : string.Empty) + ".";
            _lastMessage = message;
            _world.Log(message);
            return true;
        }

        private RoomTemplate SelectRoom(int seed)
        {
            List<RoomTemplate> rooms = MegaCatalog.AllRooms.Where(value => value != null && value.type == DungeonRoomType.Combat).ToList();
            if (rooms.Count == 0) rooms = MegaCatalog.AllRooms.Where(value => value != null).ToList();
            if (rooms.Count == 0) return null;
            return rooms[Mathf.Abs(seed) % rooms.Count];
        }

        private void SpawnMapEnemies(ArpgMapItem30 map)
        {
            Array values = Enum.GetValues(typeof(EnemyArchetype));
            List<EnemyArchetype> normalPool = new List<EnemyArchetype>();
            List<EnemyArchetype> bossPool = new List<EnemyArchetype>();
            foreach (EnemyArchetype value in values)
            {
                string name = value.ToString().ToLowerInvariant();
                if (name.Contains("training")) continue;
                if (name.Contains("warden") || name.Contains("boss") || name.Contains("titan")) bossPool.Add(value);
                else normalPool.Add(value);
            }
            if (normalPool.Count == 0) normalPool.Add((EnemyArchetype)values.GetValue(0));
            if (bossPool.Count == 0) bossPool.Add(normalPool[normalPool.Count - 1]);

            System.Random random = new System.Random(map.seed);
            int normalCount = Mathf.Clamp(5 + map.tier / 2 + map.affixIds.Count, 5, 24);
            for (int index = 0; index < normalCount; index++)
            {
                float angle = index / (float)Mathf.Max(1, normalCount) * Mathf.PI * 2f + (float)random.NextDouble() * 0.35f;
                float radius = 4.5f + (index % 4) * 1.8f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0.75f, Mathf.Sin(angle) * radius + 3f);
                EnemyArchetype archetype = normalPool[random.Next(normalPool.Count)];
                bool elite = map.tier >= 10 && index > 0 && index % Mathf.Max(3, 8 - map.tier / 8) == 0;
                EnemyController.Spawn(position, archetype, Mathf.Max(1, map.tier + 1), _activeDifficulty, elite, false);
            }
            EnemyArchetype boss = bossPool[Mathf.Abs(map.seed / 17) % bossPool.Count];
            EnemyController.Spawn(new Vector3(0f, 0.75f, 9f), boss, Mathf.Max(1, map.tier + 2), _activeDifficulty, true, true);
        }

        private DifficultySettings BuildDifficulty(ArpgMapItem30 map)
        {
            DifficultySettings difficulty = new DifficultySettings();
            foreach (string affixId in map.affixIds)
            {
                ArpgMapAffixDefinition30 definition = ArpgContent30.MapAffix(affixId);
                if (definition == null || string.IsNullOrEmpty(definition.difficultyFlag)) continue;
                System.Reflection.FieldInfo field = typeof(DifficultySettings).GetField(definition.difficultyFlag);
                if (field != null && field.FieldType == typeof(bool)) field.SetValue(difficulty, true);
            }
            return difficulty;
        }

        private void CompleteMap()
        {
            if (!_mapActive || _activeMap == null) return;
            ArpgMapItem30 map = _activeMap;
            ArpgMapDefinition30 definition = ArpgContent30.Map(map.mapId);
            bool firstCompletion = !Profile.completedMapIds.Contains(map.mapId);
            bool firstMastery = !Profile.masteredMapIds.Contains(map.mapId) && MeetsMastery(map);
            if (firstCompletion) Profile.completedMapIds.Add(map.mapId);
            if (firstMastery) Profile.masteredMapIds.Add(map.mapId);
            Profile.highestCompletedTier = Mathf.Max(Profile.highestCompletedTier, map.tier);
            Profile.totalMapsCompleted++;
            if (firstMastery) Profile.atlasPoints++;
            GrantAscendancyMilestone(map.tier, firstCompletion);

            float rewardMultiplier = ArpgStatHooks30.MapRewardMultiplier(Profile, map);
            int completionExperience = Mathf.RoundToInt((30f + map.tier * 14f) * rewardMultiplier * (1f + ArpgStatHooks30.ExperienceBonus(Profile)));
            GrantExperience(completionExperience, true);
            _rewardLines.Add("Experience: " + completionExperience);

            int itemCount = Mathf.Clamp(1 + Mathf.FloorToInt((rewardMultiplier - 1f) * 1.2f) + (map.tier >= 20 ? 1 : 0), 1, 5);
            for (int index = 0; index < itemCount; index++)
            {
                ArpgItem30 item = ArpgItems30.GenerateItem(Mathf.Max(1, map.tier * 3 + 1), Profile.characterClass, map.seed + index * 997, ArpgStatHooks30.ItemRarityBonus(Profile));
                Profile.items.Add(item);
                _rewardLines.Add("Item: " + item.displayName);
            }

            int currencyCount = Mathf.Clamp(Mathf.RoundToInt(rewardMultiplier * (1f + ArpgStatHooks30.CurrencyFindBonus(Profile))), 1, 5);
            for (int index = 0; index < currencyCount; index++)
            {
                ArpgCurrency30 currency = RollCurrency(map.tier, map.seed + index * 131);
                Profile.AddCurrency(currency, 1);
                _rewardLines.Add(ArpgItems30.CurrencyName(currency));
            }

            if (map.tier == 0 && Profile.ownedRuneIds.Count == 0)
            {
                string rune = ArpgLegacyBridge30.GrantRandomRune(_world, Profile, map.seed + 17);
                if (!string.IsNullOrEmpty(rune)) _rewardLines.Add("Support Rune: " + rune);
            }
            else if (map.tier >= 2 && Mathf.Abs(map.seed) % 4 == 0)
            {
                string rune = ArpgLegacyBridge30.GrantRandomRune(_world, Profile, map.seed + 19);
                if (!string.IsNullOrEmpty(rune)) _rewardLines.Add("Support Rune: " + rune);
            }
            if (map.tier >= 3 && (Profile.ownedCoreIds.Count < 2 || Mathf.Abs(map.seed) % 9 == 0))
            {
                string core = ArpgLegacyBridge30.GrantRandomCore(_world, Profile, map.seed + 23);
                if (!string.IsNullOrEmpty(core)) _rewardLines.Add("Spell Core: " + core);
            }
            if (map.tier >= 6 && (Profile.ownedLinkConditionIds.Count == 0 || Mathf.Abs(map.seed) % 12 == 0))
            {
                string link = ArpgLegacyBridge30.GrantRandomLink(_world, Profile, map.seed + 29);
                if (!string.IsNullOrEmpty(link)) _rewardLines.Add("Spell Link: " + link);
            }

            GrantMapDrops(map, firstCompletion, rewardMultiplier);
            DiscoverAvailableConstellations();
            ArpgProfileStore30.Save(Profile);
            _lastMessage = "Completed " + (definition == null ? map.mapId : definition.displayName) + ". " + string.Join(" · ", _rewardLines.Take(6).ToArray());
            ReturnToRefuge(false);
        }

        private void GrantMapDrops(ArpgMapItem30 completed, bool firstCompletion, float rewardMultiplier)
        {
            int nextTier = Mathf.Min(39, completed.tier + 1);
            if (firstCompletion && completed.tier < 39)
            {
                ArpgMapItem30 guaranteed = ArpgItems30.GenerateMap(nextTier, completed.seed + 100003, SuggestedRarity(nextTier), false);
                Profile.maps.Add(guaranteed);
                _rewardLines.Add("Connected map: T" + nextTier);
            }
            float chance = Mathf.Clamp01(0.48f + ArpgStatHooks30.MapSustainBonus(Profile) + (rewardMultiplier - 1f) * 0.12f);
            System.Random random = new System.Random(completed.seed ^ Profile.totalMapsCompleted * 7919);
            if (random.NextDouble() <= chance)
            {
                int dropTier = random.NextDouble() < 0.28 && completed.tier < 39 ? completed.tier + 1 : completed.tier;
                ArpgMapRarity30 rarity = random.NextDouble() < 0.18 ? ArpgMapRarity30.Rare : random.NextDouble() < 0.52 ? ArpgMapRarity30.Magic : ArpgMapRarity30.Normal;
                Profile.maps.Add(ArpgItems30.GenerateMap(dropTier, random.Next(1, int.MaxValue / 2), rarity, false));
                _rewardLines.Add("Map drop: T" + dropTier + " " + rarity);
            }
        }

        private static ArpgMapRarity30 SuggestedRarity(int tier)
        {
            if (tier >= 20) return ArpgMapRarity30.Rare;
            if (tier >= 10) return ArpgMapRarity30.Magic;
            return ArpgMapRarity30.Normal;
        }

        private static ArpgCurrency30 RollCurrency(int tier, int seed)
        {
            System.Random random = new System.Random(seed);
            int roll = random.Next(1000);
            if (tier >= 30 && roll < 8) return ArpgCurrency30.FractureRune;
            if (tier >= 25 && roll < 22) return ArpgCurrency30.DivineMeasure;
            if (tier >= 20 && roll < 55) return ArpgCurrency30.CorruptionCatalyst;
            if (tier >= 15 && roll < 100) return ArpgCurrency30.ArcaneExalt;
            if (roll < 180) return ArpgCurrency30.ElementalEssence;
            if (roll < 260) return ArpgCurrency30.ChaosFragment;
            if (roll < 350) return ArpgCurrency30.NullOrb;
            if (roll < 450) return ArpgCurrency30.ReformationOrb;
            if (roll < 570) return ArpgCurrency30.SigilOfElevation;
            if (roll < 690) return ArpgCurrency30.RuneOfAugmentation;
            if (roll < 830) return ArpgCurrency30.SparkOfAlteration;
            return ArpgCurrency30.RefinementShard;
        }

        private bool MeetsMastery(ArpgMapItem30 map)
        {
            if (map.Band == ArpgMapBand30.White) return true;
            if (map.Band == ArpgMapBand30.Blue) return (int)map.rarity >= (int)ArpgMapRarity30.Magic;
            if (map.Band == ArpgMapBand30.Yellow) return (int)map.rarity >= (int)ArpgMapRarity30.Rare;
            return (int)map.rarity >= (int)ArpgMapRarity30.Rare && map.corrupted;
        }

        private void GrantAscendancyMilestone(int tier, bool firstCompletion)
        {
            if (!firstCompletion) return;
            if (tier == 9 || tier == 19 || tier == 29 || tier == 39)
            {
                Profile.ascendancyPoints += 2;
                _rewardLines.Add("Ascendancy Points: +2");
            }
        }

        public void FailMap(string reason)
        {
            if (!_mapActive) return;
            Profile.totalDeaths++;
            _lastMessage = reason;
            ArpgProfileStore30.Save(Profile);
            ReturnToRefuge(true);
        }

        public void AbandonMap()
        {
            if (!_mapActive) return;
            FailMap("Map abandoned. The map item was consumed.");
        }

        private void ReturnToRefuge(bool failure)
        {
            _mapActive = false;
            _mapSpawned = false;
            _activeMap = null;
            _timedMapRemaining = 0f;
            _timedMapActive = false;
            if (_world == null) return;
            _world.RunActive = false;
            _world.ClearTransientObjects();
            _world.BuildSanctuaryEnvironment();
            _world.RecalculateStats(false);
            if (_world.Player != null)
            {
                _world.Player.ResetForRun();
                _world.Player.transform.position = new Vector3(0f, 1f, 0f);
            }
            _world.Log(failure ? "Returned to the Astral Refuge after map failure." : "Map complete. Rewards secured in the Astral Refuge.");
        }

        public void GrantExperience(int amount, bool announce)
        {
            if (Profile.level >= 100 || amount <= 0) return;
            Profile.experience += amount;
            bool levelled = false;
            while (Profile.level < 100 && Profile.experience >= Profile.ExperienceToNextLevel)
            {
                int requirement = Profile.ExperienceToNextLevel;
                Profile.experience -= requirement;
                Profile.level++;
                Profile.constellationPoints++;
                levelled = true;
            }
            if (levelled)
            {
                DiscoverAvailableConstellations();
                if (_world != null)
                {
                    _world.RecalculateStats(true);
                    if (_world.Player != null) _world.Player.RestoreBetweenRooms(0.2f, 0.35f);
                    if (announce) _world.Log("LEVEL " + Profile.level + " · +1 Constellation Point per level gained.");
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
            ArpgConstellationDefinition30 constellation = ArpgContent30.Constellations.FirstOrDefault(value => value.nodes.Any(node => node.id == nodeId));
            ArpgConstellationNodeDefinition30 node = ArpgContent30.ConstellationNode(nodeId);
            if (constellation == null || node == null) { message = "Unknown constellation node."; return false; }
            if (!Profile.discoveredConstellations.Contains(constellation.id)) { message = "That constellation has not been discovered."; return false; }
            if (Profile.allocatedConstellationNodes.Contains(node.id)) { message = "That Star is already allocated."; return false; }
            if (!string.IsNullOrEmpty(node.prerequisiteId) && !Profile.allocatedConstellationNodes.Contains(node.prerequisiteId)) { message = "Allocate the previous Star first."; return false; }
            if (Profile.constellationPoints < node.pointCost) { message = "Requires " + node.pointCost + " Constellation Point(s)."; return false; }
            if (node.size == ArpgNodeSize30.Completion)
            {
                int used = ArpgStatHooks30.AttunementUsed(Profile);
                int maximum = ArpgStatHooks30.AttunementMaximum(Profile);
                if (used + constellation.attunementCost > maximum)
                {
                    message = "Requires " + constellation.attunementCost + " free Attunement. " + used + "/" + maximum + " is already used.";
                    return false;
                }
            }
            Profile.constellationPoints -= node.pointCost;
            Profile.allocatedConstellationNodes.Add(node.id);
            ArpgProfileStore30.Save(Profile);
            if (_world != null) _world.RecalculateStats(true);
            message = "Allocated " + node.displayName + ".";
            return true;
        }

        public bool ResetConstellations(out string message)
        {
            message = string.Empty;
            int cost = Mathf.Max(1, Profile.allocatedConstellationNodes.Count / 10);
            if (!Profile.SpendCurrency(ArpgCurrency30.NullOrb, cost))
            {
                message = "Reset requires " + cost + " Null Orb(s).";
                return false;
            }
            int refunded = Profile.allocatedConstellationNodes.Sum(value =>
            {
                ArpgConstellationNodeDefinition30 node = ArpgContent30.ConstellationNode(value);
                return node == null ? 0 : node.pointCost;
            });
            Profile.allocatedConstellationNodes.Clear();
            Profile.constellationPoints += refunded;
            ArpgProfileStore30.Save(Profile);
            if (_world != null) _world.RecalculateStats(true);
            message = "Constellations reset. " + refunded + " points refunded.";
            return true;
        }

        public bool ChooseAscendancy(ArpgAscendancy30 ascendancy, out string message)
        {
            message = string.Empty;
            if (Profile.ascendancy != ArpgAscendancy30.None) { message = "An Ascendancy has already been chosen."; return false; }
            ArpgAscendancyDefinition30 definition = ArpgContent30.Ascendancy(ascendancy);
            if (definition == null || definition.requiredClass != Profile.characterClass) { message = "That Ascendancy does not belong to this class."; return false; }
            if (Profile.highestCompletedTier < 9 || Profile.ascendancyPoints < 2) { message = "Complete Tier 9 and earn the first two Ascendancy Points."; return false; }
            Profile.ascendancy = ascendancy;
            ArpgProfileStore30.Save(Profile);
            message = "Ascended as " + definition.displayName + ".";
            return true;
        }

        public bool AllocateAscendancyNode(string nodeId, out string message)
        {
            message = string.Empty;
            if (Profile.ascendancy == ArpgAscendancy30.None) { message = "Choose an Ascendancy first."; return false; }
            ArpgAscendancyDefinition30 definition = ArpgContent30.Ascendancy(Profile.ascendancy);
            ArpgAscendancyNodeDefinition30 node = definition == null ? null : definition.nodes.FirstOrDefault(value => value.id == nodeId);
            if (node == null) { message = "That node does not belong to the active Ascendancy."; return false; }
            if (Profile.allocatedAscendancyNodes.Contains(node.id)) { message = "That Ascendancy node is already allocated."; return false; }
            if (Profile.ascendancyPoints <= 0) { message = "No Ascendancy Points remain."; return false; }
            int index = definition.nodes.IndexOf(node);
            if (index > 0 && !Profile.allocatedAscendancyNodes.Contains(definition.nodes[index - 1].id)) { message = "Allocate the preceding Ascendancy node first."; return false; }
            Profile.ascendancyPoints--;
            Profile.allocatedAscendancyNodes.Add(node.id);
            ArpgProfileStore30.Save(Profile);
            if (_world != null) _world.RecalculateStats(true);
            message = "Allocated " + node.displayName + ".";
            return true;
        }

        public void EquipItem(ArpgItem30 item)
        {
            if (item == null) return;
            Profile.Equip(item);
            ArpgProfileStore30.Save(Profile);
            if (_world != null) _world.RecalculateStats(true);
            _lastMessage = "Equipped " + item.displayName + ".";
        }

        public bool CraftItem(ArpgItem30 item, ArpgCurrency30 currency, out string message)
        {
            bool result = ArpgItems30.CraftItem(Profile, item, currency, out message);
            if (result)
            {
                ArpgProfileStore30.Save(Profile);
                if (_world != null) _world.RecalculateStats(true);
            }
            _lastMessage = message;
            return result;
        }

        public bool CraftMap(ArpgMapItem30 map, ArpgCurrency30 currency, out string message)
        {
            bool result = ArpgItems30.CraftMap(Profile, map, currency, out message);
            if (result) ArpgProfileStore30.Save(Profile);
            _lastMessage = message;
            return result;
        }
    }
}
