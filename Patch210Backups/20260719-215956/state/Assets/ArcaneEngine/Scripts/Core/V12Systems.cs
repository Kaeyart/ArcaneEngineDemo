using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum PlayerCondition { Slow, Root, Silence, Stun }

    public enum V12CombatEventType
    {
        ManualCast,
        TriggeredCast,
        Hit,
        CriticalHit,
        StatusApplied,
        EnemyKilled,
        PlayerDamaged,
        PlayerHealed,
        PlayerDodged,
        RoomEntered,
        RoomCleared,
        RewardCommitted,
        CraftCommitted
    }

    [Serializable]
    public struct V12CombatEvent
    {
        public long sequence;
        public int frame;
        public float runTime;
        public V12CombatEventType type;
        public string sourceId;
        public string targetId;
        public string detail;
        public float amount;
        public Vector3 position;
        public int generation;
    }

    /// <summary>
    /// One bounded event stream shared by combat, telemetry, triggers, diagnostics,
    /// and future presentation. Gameplay never waits on a subscriber.
    /// </summary>
    public static class V12CombatEventBus
    {
        private const int Capacity = 256;
        private static readonly Queue<V12CombatEvent> History = new Queue<V12CombatEvent>();
        private static long _sequence;

        public static event Action<V12CombatEvent> Published;
        public static IEnumerable<V12CombatEvent> Recent { get { return History; } }

        public static void Publish(V12CombatEventType type, string sourceId, string targetId,
            float amount, Vector3 position, string detail = "", int generation = 0)
        {
            V12CombatEvent record = new V12CombatEvent
            {
                sequence = ++_sequence,
                frame = Time.frameCount,
                runTime = Time.time,
                type = type,
                sourceId = sourceId ?? string.Empty,
                targetId = targetId ?? string.Empty,
                detail = detail ?? string.Empty,
                amount = amount,
                position = position,
                generation = generation
            };
            History.Enqueue(record);
            while (History.Count > Capacity) History.Dequeue();
            Action<V12CombatEvent> handler = Published;
            if (handler == null) return;
            try { handler(record); }
            catch (Exception exception)
            {
                if (V1Diagnostics.Instance != null)
                    V1Diagnostics.Instance.Recover("A combat-event listener failed and was isolated.", exception);
            }
        }

        public static void ClearRunHistory()
        {
            History.Clear();
            _sequence = 0;
        }
    }

    public enum SpellBehaviorNodeKind { BaseSpell, Delivery, Modifier, Trigger, Status, Output }

    public sealed class SpellBehaviorNode
    {
        public string id;
        public string label;
        public SpellBehaviorNodeKind kind;
        public int order;
        public readonly List<string> next = new List<string>();
    }

    public sealed class SpellBehaviorGraph
    {
        public string stableId;
        public SpellSlot slot;
        public string coreId;
        public int runtimeCost;
        public readonly List<SpellBehaviorNode> nodes = new List<SpellBehaviorNode>();
        public readonly List<string> errors = new List<string>();
        public readonly List<string> warnings = new List<string>();
        public bool Valid { get { return errors.Count == 0; } }
    }

    /// <summary>
    /// Converts a board into an inspectable execution graph. The normal compiler
    /// remains the numeric authority; this graph is the behavioral authority used
    /// by validation, diagnostics, and the Workshop inspector.
    /// </summary>
    public static class V12SpellGraphCompiler
    {
        public const int MaximumNodes = 96;

        public static SpellBehaviorGraph Compile(SpellBoard board, CompiledSpell spell)
        {
            SpellBehaviorGraph graph = new SpellBehaviorGraph();
            if (board == null || spell == null)
            {
                graph.errors.Add("A board and compiled spell are required.");
                return graph;
            }

            graph.slot = board.slot;
            graph.coreId = board.coreId;
            graph.stableId = SpellBuildValidator.StableFingerprint(board);
            SpellBehaviorNode core = Node("core:" + board.coreId, spell.displayName, SpellBehaviorNodeKind.BaseSpell, 0);
            SpellBehaviorNode delivery = Node("delivery:" + spell.delivery, spell.delivery.ToString(), SpellBehaviorNodeKind.Delivery, 1);
            core.next.Add(delivery.id);
            graph.nodes.Add(core);
            graph.nodes.Add(delivery);
            SpellBehaviorNode tail = delivery;

            foreach (PlacedModifier placement in board.GetActivePlacements().OrderBy(value => value.placementOrder))
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(placement.modifierId);
                if (definition == null)
                {
                    graph.errors.Add("Missing modifier definition: " + placement.modifierId + ".");
                    continue;
                }
                SpellBehaviorNode modifier = Node("modifier:" + placement.placementOrder + ":" + placement.modifierId,
                    definition.displayName, SpellBehaviorNodeKind.Modifier, graph.nodes.Count);
                tail.next.Add(modifier.id);
                graph.nodes.Add(modifier);
                tail = modifier;
            }

            if (spell.poisonDamage > 0f || spell.burnDamage > 0f || spell.shockMagnitude > 0f || spell.chillMagnitude > 0f || spell.freezeSeconds > 0f)
            {
                List<string> statusNames = new List<string>();
                if (spell.poisonDamage > 0f) statusNames.Add("Poison");
                if (spell.burnDamage > 0f) statusNames.Add("Burning");
                if (spell.shockMagnitude > 0f) statusNames.Add("Shock");
                if (spell.chillMagnitude > 0f) statusNames.Add("Chill");
                if (spell.freezeSeconds > 0f) statusNames.Add("Freeze");
                string statusLabel = string.Join(" + ", statusNames);
                SpellBehaviorNode status = Node("status:" + statusLabel, statusLabel, SpellBehaviorNodeKind.Status, graph.nodes.Count);
                tail.next.Add(status.id);
                graph.nodes.Add(status);
                tail = status;
            }

            foreach (TriggerSpec trigger in spell.triggers)
            {
                string id = "trigger:" + graph.nodes.Count + ":" + trigger.sourceId;
                SpellBehaviorNode triggerNode = Node(id,
                    trigger.moment + " → Spell " + ((int)trigger.linkedSlot + 1), SpellBehaviorNodeKind.Trigger, graph.nodes.Count);
                triggerNode.next.Add("slot:" + ((int)trigger.linkedSlot + 1));
                tail.next.Add(triggerNode.id);
                graph.nodes.Add(triggerNode);
            }

            SpellBehaviorNode output = Node("output:" + graph.stableId, spell.element + " " + spell.delivery,
                SpellBehaviorNodeKind.Output, graph.nodes.Count);
            tail.next.Add(output.id);
            graph.nodes.Add(output);
            graph.runtimeCost = Mathf.Max(1, spell.projectileCount) * Mathf.Max(1, spell.repeatCount) *
                Mathf.Max(1, 1 + spell.splitCount) + spell.triggers.Count * 4 + (spell.zoneDuration > 0f ? 6 : 0);

            if (graph.nodes.Count > MaximumNodes) graph.errors.Add("Spell behavior graph exceeds " + MaximumNodes + " nodes.");
            if (graph.runtimeCost > V1PerformanceBudget.MaximumSpellEntities)
                graph.warnings.Add("The mechanical graph exceeds the normal presentation budget; optional visuals will be reduced.");
            if (board.placed.Count > board.GetActivePlacements().Count)
                graph.warnings.Add("Disconnected modifiers remain installed but do not enter the execution graph.");
            return graph;
        }

        private static SpellBehaviorNode Node(string id, string label, SpellBehaviorNodeKind kind, int order)
        {
            return new SpellBehaviorNode { id = id, label = label, kind = kind, order = order };
        }
    }

    public static class V12EncounterPlanner
    {
        private static readonly EnemyArchetype[] Early =
        {
            EnemyArchetype.Crawler, EnemyArchetype.Bulwark, EnemyArchetype.Crawler, EnemyArchetype.Hexer
        };

        private static readonly EnemyArchetype[] Mid =
        {
            EnemyArchetype.Crawler, EnemyArchetype.Bulwark, EnemyArchetype.Hexer, EnemyArchetype.Charger,
            EnemyArchetype.Warden, EnemyArchetype.Leech, EnemyArchetype.Assassin
        };

        private static readonly EnemyArchetype[] Late =
        {
            EnemyArchetype.Crawler, EnemyArchetype.Bulwark, EnemyArchetype.Hexer, EnemyArchetype.Charger,
            EnemyArchetype.Warden, EnemyArchetype.Leech, EnemyArchetype.Mirror, EnemyArchetype.Assassin,
            EnemyArchetype.Controller
        };

        public static EnemyArchetype Select(int seed, int roomIndex, int index, bool boss)
        {
            if (boss)
            {
                EnemyArchetype[] bosses =
                {
                    EnemyArchetype.OssuaryWarden, EnemyArchetype.EmberTitan,
                    EnemyArchetype.ArchiveSeraph, EnemyArchetype.VenomMatriarch
                };
                return bosses[Mathf.Abs(V1Determinism.Combine(seed, roomIndex, "boss_roster", 0)) % bosses.Length];
            }
            EnemyArchetype[] pool = roomIndex < 2 ? Early : roomIndex < 6 ? Mid : Late;
            int offset = V1Determinism.Combine(seed, roomIndex, "encounter_role", index);
            return pool[Mathf.Abs(offset) % pool.Length];
        }

        public static int Validate()
        {
            int failures = 0;
            if (!Late.Contains(EnemyArchetype.Leech) || !Late.Contains(EnemyArchetype.Controller) || !Late.Contains(EnemyArchetype.Assassin))
            { Debug.LogError("1.2 encounter validation: late-game role coverage is incomplete."); failures++; }
            for (int room = 0; room < 12; room++)
                for (int index = 0; index < 18; index++)
                    if (!Enum.IsDefined(typeof(EnemyArchetype), Select(771, room, index, false)))
                    { Debug.LogError("1.2 encounter validation produced an invalid archetype."); return failures + 1; }
            return failures;
        }
    }

    public struct ForgeMaterialReward
    {
        public int dust;
        public int runes;
        public int cores;
    }

    public static class V12RewardPolicy
    {
        public static ForgeMaterialReward ForRoom(DungeonRoomType type, int depth)
        {
            ForgeMaterialReward reward = new ForgeMaterialReward { dust = 2 + Mathf.Max(0, depth) / 3 };
            if (type == DungeonRoomType.Elite || type == DungeonRoomType.Miniboss || type == DungeonRoomType.Challenge) reward.runes = 1;
            if (type == DungeonRoomType.Boss) { reward.dust += 5; reward.runes = 2; reward.cores = 1; }
            if (type == DungeonRoomType.TreasureVault) reward.dust += 3;
            return reward;
        }

        public static int Validate()
        {
            int failures = 0;
            foreach (DungeonRoomType type in Enum.GetValues(typeof(DungeonRoomType)))
            {
                ForgeMaterialReward reward = ForRoom(type, 8);
                if (reward.dust < 0 || reward.runes < 0 || reward.cores < 0) failures++;
            }
            if (ForRoom(DungeonRoomType.Boss, 10).cores < 1) failures++;
            if (failures > 0) Debug.LogError("1.2 reward validation: one or more room material rewards are invalid.");
            return failures;
        }
    }

    public static class V12DifficultyContract
    {
        public static int ThreatRating(DifficultySettings settings)
        {
            if (settings == null) return 0;
            int score = settings.EnabledCount;
            if (settings.glassSoul) score += 2;
            if (settings.newBossPhase) score += 2;
            if (settings.noStartingEquipment) score += 2;
            if (settings.adaptiveEnemies || settings.timedRooms) score += 1;
            return score;
        }

        public static string Name(DifficultySettings settings)
        {
            int score = ThreatRating(settings);
            return score <= 0 ? "NORMAL" : score <= 3 ? "HARD" : score <= 7 ? "NIGHTMARE" : "APOCALYPSE";
        }

        public static int Validate()
        {
            DifficultySettings baseline = new DifficultySettings();
            DifficultySettings hard = new DifficultySettings { frenziedEnemies = true, glassSoul = true, timedRooms = true };
            if (ThreatRating(hard) <= ThreatRating(baseline) || hard.RewardMultiplier <= baseline.RewardMultiplier)
            { Debug.LogError("1.2 difficulty validation: threat and rewards are not monotonic."); return 1; }
            return 0;
        }
    }

    /// <summary>
    /// Consolidates run audits, telemetry, performance sampling, and recovery.
    /// It is intentionally lightweight and never scans the scene hierarchy.
    /// </summary>
    public sealed class V12SystemsDirector : MonoBehaviour
    {
        public const string ReleaseName = "2.1.0-demo · Promise Completion Candidate";
        public static V12SystemsDirector Instance { get; private set; }
        public int CombatEventsThisRun { get; private set; }
        public int OwnershipRecoveries { get; private set; }
        public float SmoothedFrameMilliseconds { get; private set; }
        public string LastAudit { get; private set; } = "Waiting for runtime";

        private RunDirector _run;
        private float _nextAudit;
        private float _nextPerformanceSample;

        private void Awake()
        {
            Instance = this;
            V12CombatEventBus.Published += OnCombatEvent;
        }

        private void Start()
        {
            _run = GetComponent<RunDirector>();
            if (_run != null)
            {
                _run.RoomEntered += OnRoomEntered;
                _run.RoomCleared += OnRoomCleared;
            }
            LastAudit = "Runtime services connected";
        }

        private void OnDestroy()
        {
            if (_run != null)
            {
                _run.RoomEntered -= OnRoomEntered;
                _run.RoomCleared -= OnRoomCleared;
            }
            V12CombatEventBus.Published -= OnCombatEvent;
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (Time.unscaledTime >= _nextPerformanceSample)
            {
                _nextPerformanceSample = Time.unscaledTime + 0.25f;
                float milliseconds = Time.unscaledDeltaTime * 1000f;
                SmoothedFrameMilliseconds = Mathf.Lerp(SmoothedFrameMilliseconds <= 0f ? milliseconds : SmoothedFrameMilliseconds,
                    milliseconds, 0.18f);
            }
            if (Time.unscaledTime < _nextAudit) return;
            _nextAudit = Time.unscaledTime + 0.75f;
            AuditModifierOwnership();
        }

        private void OnCombatEvent(V12CombatEvent record)
        {
            CombatEventsThisRun++;
        }

        private void OnRoomEntered(RoomTemplate room)
        {
            if (_run != null && _run.RoomIndex == 0)
            {
                V12CombatEventBus.ClearRunHistory();
                CombatEventsThisRun = 0;
            }
            V12CombatEventBus.Publish(V12CombatEventType.RoomEntered, room == null ? "room" : room.id,
                string.Empty, _run == null ? 0f : _run.RoomIndex, Vector3.zero, room == null ? string.Empty : room.displayName);
        }

        private void OnRoomCleared(RoomTemplate room)
        {
            V12CombatEventBus.Publish(V12CombatEventType.RoomCleared, room == null ? "room" : room.id,
                string.Empty, _run == null ? 0f : _run.RoomIndex, Vector3.zero, room == null ? string.Empty : room.displayName);
        }

        private void AuditModifierOwnership()
        {
            GameWorld world = GameWorld.Instance;
            if (world == null || !world.RunActive) { LastAudit = "Home Base · no run ownership to audit"; return; }
            Dictionary<string, int> installed = new Dictionary<string, int>();
            for (int slot = 0; slot < 3; slot++)
            {
                SpellBoard board = world.GetBoard((SpellSlot)slot);
                if (board == null) continue;
                foreach (PlacedModifier piece in board.placed)
                {
                    int count; installed.TryGetValue(piece.modifierId, out count); installed[piece.modifierId] = count + 1;
                }
            }

            bool mismatch = false;
            foreach (KeyValuePair<string, int> used in installed)
            {
                int owned;
                if (!world.OwnedModifierCounts.TryGetValue(used.Key, out owned) || used.Value > owned) { mismatch = true; break; }
            }
            foreach (KeyValuePair<string, int> owned in world.OwnedModifierCounts)
            {
                int used; installed.TryGetValue(owned.Key, out used);
                int available; world.ModifierInventory.TryGetValue(owned.Key, out available);
                if (available != Mathf.Max(0, owned.Value - used)) { mismatch = true; break; }
            }
            if (mismatch)
            {
                int returned = world.RepairModifierOwnership();
                OwnershipRecoveries++;
                if (V1Diagnostics.Instance != null) V1Diagnostics.Instance.Record("1.2 ownership audit repaired modifier availability and returned " + returned + " illegal placement(s).");
            }
            LastAudit = mismatch ? "Modifier ownership repaired" : "Modifier ownership valid";
        }

        public static int ValidateReleaseSurface()
        {
            int failures = V12EncounterPlanner.Validate() + V12RewardPolicy.Validate() + V12DifficultyContract.Validate();
            EquipmentInventory equipment = new EquipmentInventory();
            PlayerStats stats = new PlayerStats();
            foreach (SpellCoreDefinition core in DemoCatalog.AllCores)
            {
                try
                {
                    SpellBoard board = new SpellBoard(SpellSlot.Slot1, core.id);
                    CompiledSpell spell = SpellCompiler.Compile(board, stats, equipment);
                    SpellBehaviorGraph graph = V12SpellGraphCompiler.Compile(board, spell);
                    if (!graph.Valid || graph.nodes.Count < 3 || string.IsNullOrEmpty(graph.stableId))
                    { Debug.LogError("1.2 graph validation failed for " + core.id + "."); failures++; }
                }
                catch (Exception exception)
                { Debug.LogError("1.2 graph validation exception for " + core.id + ": " + exception.Message); failures++; }
            }
            foreach (ForgeAction action in Enum.GetValues(typeof(ForgeAction)))
            {
                ForgeCost cost = V11Itemization.CostFor(action, 20);
                if (cost.gold < 0 || cost.dust < 0 || cost.runes < 0 || cost.cores < 0)
                { Debug.LogError("1.2 Forge validation failed for " + action + "."); failures++; }
            }
            return failures;
        }

        public string BuildRuntimeSummary()
        {
            return ReleaseName + " · " + LastAudit + " · " + CombatEventsThisRun + " combat events · " +
                   SmoothedFrameMilliseconds.ToString("0.0") + " ms frame · " + OwnershipRecoveries + " ownership recoveries";
        }
    }
}
