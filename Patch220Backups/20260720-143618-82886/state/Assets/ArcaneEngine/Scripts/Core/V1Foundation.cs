using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine;

namespace ArcaneEngine
{
    public enum GameFlowState
    {
        Boot,
        Title,
        HomeBase,
        PreparingRun,
        Exploring,
        Encounter,
        RewardSelection,
        RouteSelection,
        Shop,
        SafeRoom,
        Extraction,
        RunResult,
        Paused
    }

    public enum EnemyBrainState { Spawning, Repositioning, Telegraphing, Attacking, Recovering, Staggered, Dead }
    public enum RuntimeEntityKind { Enemy, PlayerProjectile, TriggeredCast, EnemyProjectile, Hazard, DamageNumber, Cosmetic }

    public interface IDamageable
    {
        bool IsAlive { get; }
        Vector3 DamagePoint { get; }
        void ReceiveDamage(DamageRequest request);
    }

    public interface ITargetable
    {
        bool CanBeTargeted { get; }
        Vector3 TargetPoint { get; }
    }

    public interface IStatusReceiver
    {
        void ReceiveStatus(SpellElement element, float strength, float duration, string sourceId);
    }

    [Serializable]
    public struct DamageRequest
    {
        public float amount;
        public SpellElement element;
        public bool critical;
        public bool unavoidable;
        public string sourceId;
        public string sourceDisplayName;
        public Vector3 hitPoint;
    }

    public static class BalanceTuning
    {
        public const float MaximumResistance = 0.65f;
        public const float MinimumCooldown = 0.12f;
        public const float MaximumCriticalChance = 0.8f;
        public const float MaximumMovementSpeed = 13f;
        public const int MaximumTriggerDepth = 8;
        public const int MaximumTriggerActivations = 48;
        public const int MaximumProjectilesPerCast = 40;
        public const int MaximumLivingEnemies = 24;

        public static float ArmorMultiplier(float armor)
        {
            return 100f / (100f + Mathf.Max(0f, armor));
        }

        public static float ResistanceMultiplier(float resistance)
        {
            return 1f - Mathf.Clamp(resistance, 0f, MaximumResistance);
        }

        public static float EnemyHealthScale(int depth, bool elite, bool boss)
        {
            return (1f + Mathf.Max(0, depth - 1) * 0.16f) * (elite ? 1.6f : 1f) * (boss ? 2.15f : 1f);
        }

        public static float EnemyDamageScale(int depth, bool elite)
        {
            return (1f + Mathf.Max(0, depth) * 0.065f) * (elite ? 1.18f : 1f);
        }

        public static float RewardScale(int depth, float difficultyMultiplier)
        {
            return Mathf.Max(1f, (1f + depth * 0.12f) * Mathf.Max(1f, difficultyMultiplier));
        }

        public static CompiledSpell EnforceSpellCaps(CompiledSpell spell)
        {
            if (spell == null) return null;
            spell.cooldown = Mathf.Max(MinimumCooldown, spell.cooldown);
            spell.projectileCount = Mathf.Clamp(spell.projectileCount, 1, MaximumProjectilesPerCast);
            spell.repeatCount = Mathf.Clamp(spell.repeatCount, 1, 12);
            spell.chainTargets = Mathf.Clamp(spell.chainTargets, 0, 16);
            spell.splitCount = Mathf.Clamp(spell.splitCount, 0, 12);
            spell.lifetime = Mathf.Clamp(spell.lifetime, 0.08f, 18f);
            spell.zoneDuration = Mathf.Clamp(spell.zoneDuration, 0f, 18f);
            return spell;
        }
    }

    public static class V1Determinism
    {
        public static int StableHash(string value)
        {
            unchecked
            {
                uint hash = 2166136261u;
                if (value != null)
                    for (int i = 0; i < value.Length; i++) { hash ^= value[i]; hash *= 16777619u; }
                return (int)(hash & 0x7fffffff);
            }
        }

        public static int Combine(int seed, int depth, string contentId, int salt = 0)
        {
            unchecked { return (seed * 486187739 + depth * 16777619 + StableHash(contentId) + salt * 397) & 0x7fffffff; }
        }
    }

    public static class ControlSettingsValidator
    {
        public static List<string> FindConflicts(ControlSettings controls)
        {
            if (controls == null) return new List<string> { "Control settings are missing." };
            Dictionary<KeyCode, List<string>> bindings = new Dictionary<KeyCode, List<string>>();
            Add(bindings, controls.moveForward, "Move Forward"); Add(bindings, controls.moveBack, "Move Back");
            Add(bindings, controls.moveLeft, "Move Left"); Add(bindings, controls.moveRight, "Move Right");
            Add(bindings, controls.spellSlot3, "Spell Slot 3"); Add(bindings, controls.dodge, "Dodge");
            Add(bindings, controls.interact, "Interact"); Add(bindings, controls.workshop, "Spellcraft");
            Add(bindings, controls.spellLinks, "Spell Links");
            Add(bindings, controls.inventory, "Inventory"); Add(bindings, controls.help, "Help");
            Add(bindings, controls.map, "Dungeon Map"); Add(bindings, controls.cancelCast, "Cancel Cast");
            return bindings.Where(pair => pair.Key != KeyCode.None && pair.Value.Count > 1)
                .Select(pair => pair.Key + " is assigned to " + string.Join(" and ", pair.Value)).ToList();
        }

        private static void Add(Dictionary<KeyCode, List<string>> bindings, KeyCode key, string action)
        {
            List<string> actions;
            if (!bindings.TryGetValue(key, out actions)) { actions = new List<string>(); bindings[key] = actions; }
            actions.Add(action);
        }
    }

    public static class EnemyBestiary
    {
        public static string Describe(EnemyArchetype type)
        {
            if (type == EnemyArchetype.Crawler) return "CHASER · Fast melee pressure. Dodge through its short attack recovery.";
            if (type == EnemyArchetype.Bulwark) return "TANK · Armored, slow, and heavy-hitting. Sustained damage breaks its Armor.";
            if (type == EnemyArchetype.Hexer) return "RANGED · Maintains distance and fires aimed bolts. Close the gap or use cover.";
            if (type == EnemyArchetype.Charger) return "DISRUPTOR · Telegraphs a long charge. Step sideways, then punish the recovery.";
            if (type == EnemyArchetype.Warden) return "TANK · Carries a Shield and protects nearby threats. Break the Shield before committing burst damage.";
            if (type == EnemyArchetype.Leech) return "SUPPORT · Heals wounded allies. Priority target when stronger enemies are present.";
            if (type == EnemyArchetype.Mirror) return "RANGED · Fires spread volleys and controls open lanes. Fight from an angle.";
            if (type == EnemyArchetype.Assassin) return "ASSASSIN · Teleports behind the player before striking. Keep moving when it disappears.";
            if (type == EnemyArchetype.Controller) return "CONTROLLER · Creates lasting danger zones. Reposition before casting long sequences.";
            if (type == EnemyArchetype.OssuaryWarden) return "BOSS · Break Ward Pillars, survive the Bone Storm, then rotate damage types against adaptation.";
            return "TRAINING TARGET · Used for safe build testing and grants no rewards.";
        }
    }

    public static class DungeonLayoutValidator
    {
        public static bool IsValidPlacement(Vector3 position, float radius, IList<Vector3> occupied)
        {
            if (Mathf.Abs(position.x) + radius > 14.5f || Mathf.Abs(position.z) + radius > 14.5f) return false;
            if (position.z < -8.5f && Mathf.Abs(position.x) < 4.5f) return false;
            if (position.z > 10.5f && Mathf.Abs(position.x) < 4f) return false;
            if (position.sqrMagnitude < Mathf.Pow(3.4f + radius, 2f)) return false;
            if (occupied != null)
                foreach (Vector3 existing in occupied)
                    if (CombatMath.PlanarDistanceSquared(existing, position) < Mathf.Pow(2.2f + radius, 2f)) return false;
            return true;
        }

        public static Vector3 FindPlacement(float minimum, float maximum, float radius, IList<Vector3> occupied, int attempts = 24)
        {
            for (int i = 0; i < attempts; i++)
            {
                Vector3 candidate = new Vector3(UnityEngine.Random.Range(minimum, maximum), 0f, UnityEngine.Random.Range(minimum, maximum));
                if (IsValidPlacement(candidate, radius, occupied)) return candidate;
            }
            for (int i = 0; i < 16; i++)
            {
                float angle = ((occupied == null ? 0 : occupied.Count) + i) * 2.399963f;
                Vector3 fallback = new Vector3(Mathf.Cos(angle) * 10f, 0f, Mathf.Sin(angle) * 9f);
                if (IsValidPlacement(fallback, radius, occupied)) return fallback;
            }
            return new Vector3(12f, 0f, 0f);
        }
    }

    public sealed class SpellBuildValidationReport
    {
        public bool valid;
        public int activePieces;
        public int inactivePieces;
        public int triggerCount;
        public int estimatedPeakEntities;
        public float estimatedDps;
        public float manaPerSecond;
        public float instability;
        public int usedCapacity;
        public int maximumCapacity;
        public int behaviorGraphNodes;
        public int behaviorRuntimeCost;
        public string stableId;
        public readonly List<string> errors = new List<string>();
        public readonly List<string> warnings = new List<string>();
        public readonly List<string> inactiveReasons = new List<string>();

        public string CompactSummary
        {
            get
            {
                string status = valid ? "BUILD VALID" : "BUILD BLOCKED";
                return status + " · " + activePieces + " active · " + inactivePieces + " inactive · " +
                       estimatedDps.ToString("0.0") + " estimated DPS · " + estimatedPeakEntities + " peak entities";
            }
        }
    }

    public static class SpellBuildValidator
    {
        public static SpellBuildValidationReport Validate(SpellBoard board, PlayerStats stats, EquipmentInventory equipment)
        {
            SpellBuildValidationReport report = new SpellBuildValidationReport();
            if (board == null)
            {
                report.errors.Add("No spell board is assigned.");
                return report;
            }
            SpellCoreDefinition core = DemoCatalog.GetCore(board.coreId);
            if (core == null)
            {
                report.errors.Add("The selected Spell Core is missing from the content catalog.");
                return report;
            }

            List<PlacedModifier> active = board.GetActivePlacements();
            report.activePieces = active.Count;
            report.inactivePieces = Mathf.Max(0, board.placed.Count - active.Count);
            report.usedCapacity = board.UsedCapacity();
            report.maximumCapacity = board.Capacity;
            if (report.usedCapacity > report.maximumCapacity)
                report.errors.Add("The connected Support Runes use " + report.usedCapacity + " Capacity, but the spell has " + report.maximumCapacity + ".");
            foreach (PlacedModifier piece in board.placed.Where(piece => !active.Contains(piece)))
                report.inactiveReasons.Add((DemoCatalog.GetModifier(piece.modifierId)?.displayName ?? piece.modifierId) + ": " + board.GetInactiveReason(piece));

            try
            {
                CompiledSpell spell = BalanceTuning.EnforceSpellCaps(SpellCompiler.Compile(board, stats ?? new PlayerStats(), equipment ?? new EquipmentInventory()));
                if (spell == null) report.errors.Add("The spell compiler returned no spell.");
                else
                {
                    report.triggerCount = spell.triggers.Count;
                    report.instability = spell.instability;
                    float directPerCast = spell.damage * Mathf.Max(1, spell.projectileCount) * Mathf.Max(1, spell.repeatCount);
                    float statusPerCast = spell.poisonDamage * Mathf.Max(0f, spell.poisonDuration) + spell.burnDamage * Mathf.Max(0f, spell.burnDuration);
                    report.estimatedDps = (directPerCast + statusPerCast) / Mathf.Max(BalanceTuning.MinimumCooldown, spell.cooldown);
                    report.manaPerSecond = (spell.manaCost + spell.healthCost) / Mathf.Max(BalanceTuning.MinimumCooldown, spell.cooldown);
                    report.estimatedPeakEntities = Mathf.Max(1, spell.projectileCount) * Mathf.Max(1, 1 + spell.splitCount) * Mathf.Max(1, spell.repeatCount) + spell.triggers.Count * 3;
                    SpellBehaviorGraph graph = V12SpellGraphCompiler.Compile(board, spell);
                    report.behaviorGraphNodes = graph.nodes.Count;
                    report.behaviorRuntimeCost = graph.runtimeCost;
                    report.errors.AddRange(graph.errors);
                    report.warnings.AddRange(graph.warnings);
                    report.warnings.AddRange(spell.warnings);
                    if (spell.instability > 100f) report.warnings.Add("High Spell Overload may make this build difficult to sustain.");
                    if (report.estimatedPeakEntities > V1PerformanceBudget.MaximumSpellEntities)
                        report.warnings.Add("This cast can exceed the visual entity budget; cosmetic duplicates will be reduced first.");
                    if (spell.triggers.Any(trigger => trigger.maxActivationsPerEvent <= 0 || trigger.energyCost <= 0f))
                        report.errors.Add("A trigger has no activation or energy limit.");
                    if (spell.triggers.GroupBy(trigger => trigger.moment + ":" + trigger.linkedSlot).Any(group => group.Count() > 4))
                        report.warnings.Add("Several triggers share the same event and target spell. Expect heavy Trigger Limit use.");
                }
            }
            catch (Exception exception)
            {
                report.errors.Add("Compilation failed: " + exception.Message);
            }

            if (report.inactivePieces > 0) report.warnings.Add(report.inactivePieces + " Support Rune(s) are placed but disconnected.");
            report.stableId = StableFingerprint(board);
            report.valid = report.errors.Count == 0;
            return report;
        }

        public static List<string> ValidateTriggerGraph(IEnumerable<SpellBoard> boards, PlayerStats stats, EquipmentInventory equipment)
        {
            List<string> issues = new List<string>();
            Dictionary<SpellSlot, List<SpellSlot>> graph = new Dictionary<SpellSlot, List<SpellSlot>>();
            foreach (SpellBoard board in boards ?? Enumerable.Empty<SpellBoard>())
            {
                CompiledSpell spell;
                try { spell = SpellCompiler.Compile(board, stats ?? new PlayerStats(), equipment ?? new EquipmentInventory()); }
                catch (Exception exception) { issues.Add("Slot " + ((int)board.slot + 1) + " failed: " + exception.Message); continue; }
                if (spell == null) continue;
                graph[board.slot] = spell.triggers.Select(trigger => trigger.linkedSlot).Distinct().ToList();
            }
            foreach (SpellSlot origin in graph.Keys)
                if (HasCycle(origin, origin, graph, new HashSet<SpellSlot>(), 0))
                    issues.Add("Trigger cycle detected from Spell Slot " + ((int)origin + 1) + ". Runtime energy and depth limits will stop recursion.");
            return issues.Distinct().ToList();
        }

        public static string StableFingerprint(SpellBoard board)
        {
            if (board == null) return "missing";
            StringBuilder source = new StringBuilder(board.coreId).Append('|').Append(board.relicId).Append('|').Append(board.Radius).Append('|').Append(board.spellLevel);
            foreach (PlacedModifier piece in board.placed.OrderBy(piece => piece.placementOrder))
                source.Append('|').Append(piece.modifierId).Append('@').Append(piece.anchor.q).Append(',').Append(piece.anchor.r).Append(':').Append(piece.rotation);
            return Hash128.Compute(source.ToString()).ToString();
        }

        private static bool HasCycle(SpellSlot origin, SpellSlot current, Dictionary<SpellSlot, List<SpellSlot>> graph, HashSet<SpellSlot> path, int depth)
        {
            if (depth > BalanceTuning.MaximumTriggerDepth || !path.Add(current)) return current == origin;
            List<SpellSlot> next;
            if (!graph.TryGetValue(current, out next)) { path.Remove(current); return false; }
            foreach (SpellSlot linked in next)
            {
                if (linked == origin || HasCycle(origin, linked, graph, path, depth + 1)) { path.Remove(current); return true; }
            }
            path.Remove(current);
            return false;
        }
    }

    public sealed class AttackCoordinator : MonoBehaviour
    {
        public static AttackCoordinator Instance { get; private set; }
        private sealed class Reservation { public EnemyController enemy; public string role; public float expires; }
        private readonly List<Reservation> _active = new List<Reservation>();

        private void Awake() { Instance = this; }
        private void OnDestroy() { if (Instance == this) Instance = null; }
        private void Update() { _active.RemoveAll(item => item.enemy == null || item.enemy.IsDead || Time.time >= item.expires); }

        public bool TryReserve(EnemyController enemy, string role, float duration)
        {
            if (enemy == null) return false;
            _active.RemoveAll(item => item.enemy == null || item.enemy.IsDead || Time.time >= item.expires);
            if (_active.Any(item => item.enemy == enemy)) return true;
            int globalLimit = GameWorld.Instance != null && GameWorld.Instance.Enemies.Count > 12 ? 4 : 3;
            int roleLimit = role == "RANGED" ? 2 : role == "DISRUPTOR" || role == "ASSASSIN" ? 1 : 2;
            if (_active.Count >= globalLimit || _active.Count(item => item.role == role) >= roleLimit) return false;
            _active.Add(new Reservation { enemy = enemy, role = role, expires = Time.time + Mathf.Max(0.15f, duration) });
            return true;
        }

        public void Release(EnemyController enemy) { _active.RemoveAll(item => item.enemy == enemy); }
        public int ActiveAttackers { get { return _active.Count; } }
    }

    public sealed class V1PerformanceBudget : MonoBehaviour
    {
        public const int MaximumSpellEntities = 150;
        public static V1PerformanceBudget Instance { get; private set; }
        private readonly Dictionary<RuntimeEntityKind, int> _counts = new Dictionary<RuntimeEntityKind, int>();
        public int RejectedCosmetics { get; private set; }

        private void Awake() { Instance = this; }
        private void OnDestroy() { if (Instance == this) Instance = null; }

        public bool TryAcquire(RuntimeEntityKind kind)
        {
            int current; _counts.TryGetValue(kind, out current);
            int limit = Limit(kind);
            if (current >= limit)
            {
                if (kind == RuntimeEntityKind.Cosmetic || kind == RuntimeEntityKind.DamageNumber) RejectedCosmetics++;
                return false;
            }
            _counts[kind] = current + 1;
            return true;
        }

        public void Release(RuntimeEntityKind kind)
        {
            int current; _counts.TryGetValue(kind, out current); _counts[kind] = Mathf.Max(0, current - 1);
        }

        public int Count(RuntimeEntityKind kind) { int count; return _counts.TryGetValue(kind, out count) ? count : 0; }

        private static int Limit(RuntimeEntityKind kind)
        {
            if (kind == RuntimeEntityKind.Enemy) return BalanceTuning.MaximumLivingEnemies;
            if (kind == RuntimeEntityKind.PlayerProjectile) return 100;
            if (kind == RuntimeEntityKind.TriggeredCast) return 48;
            if (kind == RuntimeEntityKind.EnemyProjectile) return 80;
            if (kind == RuntimeEntityKind.Hazard) return 32;
            if (kind == RuntimeEntityKind.DamageNumber) return 70;
            return 110;
        }
    }

    public static class RuntimeObjectPool
    {
        private static readonly Dictionary<PrimitiveType, Stack<GameObject>> Pools = new Dictionary<PrimitiveType, Stack<GameObject>>();
        private static Transform _root;

        public static GameObject Rent(string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color)
        {
            if (V1PerformanceBudget.Instance != null && !V1PerformanceBudget.Instance.TryAcquire(RuntimeEntityKind.Cosmetic)) return null;
            Stack<GameObject> pool;
            if (!Pools.TryGetValue(type, out pool)) { pool = new Stack<GameObject>(); Pools[type] = pool; }
            GameObject result = null;
            while (pool.Count > 0 && result == null) result = pool.Pop();
            if (result == null)
            {
                result = RuntimeVisuals.Primitive(name, type, position, scale, color);
                RuntimeVisuals.RemoveCollider(result);
                PooledPrimitive marker = result.AddComponent<PooledPrimitive>();
                marker.type = type;
            }
            result.name = name;
            result.transform.SetParent(null, true);
            result.transform.position = position;
            result.transform.localScale = scale;
            Renderer renderer = result.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = RuntimeVisuals.Material(color, 0.55f);
            result.SetActive(true);
            return result;
        }

        public static void Return(GameObject value)
        {
            if (value == null || !value.activeSelf) return;
            PooledPrimitive marker = value.GetComponent<PooledPrimitive>();
            if (marker == null) { UnityEngine.Object.Destroy(value); return; }
            if (_root == null)
            {
                GameObject root = new GameObject("Arcane Engine Runtime Pool");
                UnityEngine.Object.DontDestroyOnLoad(root);
                _root = root.transform;
            }
            value.SetActive(false);
            value.transform.SetParent(_root, false);
            Stack<GameObject> pool;
            if (!Pools.TryGetValue(marker.type, out pool)) { pool = new Stack<GameObject>(); Pools[marker.type] = pool; }
            if (pool.Count < 80) pool.Push(value); else UnityEngine.Object.Destroy(value);
            if (V1PerformanceBudget.Instance != null) V1PerformanceBudget.Instance.Release(RuntimeEntityKind.Cosmetic);
        }
    }

    public sealed class PooledPrimitive : MonoBehaviour { public PrimitiveType type; }

    public sealed class RuntimeEntityToken : MonoBehaviour
    {
        private RuntimeEntityKind _kind;
        private bool _held;
        public bool Acquire(RuntimeEntityKind kind)
        {
            _kind = kind;
            _held = V1PerformanceBudget.Instance == null || V1PerformanceBudget.Instance.TryAcquire(kind);
            if (!_held) Destroy(gameObject);
            return _held;
        }
        private void OnDestroy() { if (_held && V1PerformanceBudget.Instance != null) V1PerformanceBudget.Instance.Release(_kind); }
    }

    public sealed class V1Diagnostics : MonoBehaviour
    {
        public static V1Diagnostics Instance { get; private set; }
        private readonly Queue<string> _events = new Queue<string>();
        public string LastRecoveryMessage { get; private set; }

        private void Awake() { Instance = this; Record("Runtime diagnostics started."); }
        private void OnDestroy() { if (Instance == this) Instance = null; }
        public void Record(string message)
        {
            _events.Enqueue(DateTime.UtcNow.ToString("o") + " · " + message);
            while (_events.Count > 120) _events.Dequeue();
        }
        public void Recover(string playerMessage, Exception exception = null)
        {
            LastRecoveryMessage = playerMessage;
            Record("RECOVERY · " + playerMessage + (exception == null ? string.Empty : " · " + exception.Message));
            Debug.LogWarning("Arcane Engine recovered: " + playerMessage);
        }
        public string BuildReport()
        {
            GameWorld world = GameWorld.Instance;
            RunDirector run = world == null ? null : world.GetComponent<RunDirector>();
            return "ARCANE ENGINE 2.0 DIAGNOSTIC\nUTC " + DateTime.UtcNow.ToString("o") +
                   "\nUnity " + Application.unityVersion + "\nPlatform " + Application.platform +
                   "\nProfile " + (ProfileManager.Current == null ? "none" : ProfileManager.Current.stableProfileId) +
                   "\nRun seed " + (run == null ? 0 : run.CurrentSeed) + "\nRoom " + (run == null ? 0 : run.RoomIndex + 1) +
                   "\nSave " + ProfileManager.LastSaveStatus +
                   "\nVisuals " + ProceduralVisualRuntime.ActiveVisuals + " active / " + ProceduralVisualRuntime.PooledVisuals + " pooled" +
                   "\nLights " + ProceduralVisualRuntime.ActiveLights + " · Decals " + ProceduralVisualRuntime.ActiveDecals +
                   "\nShared materials " + RuntimeVisuals.MaterialCount + " · Adaptive scale " + ProceduralVisualRuntime.AdaptiveScale.ToString("0.00") +
                   "\n\n" + string.Join("\n", _events);
        }

        public string ExportReport()
        {
            string folder = ProfileManager.ProfileFolderPath;
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "diagnostic_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".txt");
            File.WriteAllText(path, BuildReport());
            Record("Diagnostic exported to " + path);
            return path;
        }
    }

    public static class DungeonNavigation
    {
        public static Vector3 Steer(EnemyController self, Vector3 desiredDirection, float distance)
        {
            desiredDirection.y = 0f;
            if (desiredDirection.sqrMagnitude < 0.001f) return Vector3.zero;
            Vector3 forward = desiredDirection.normalized;
            Vector3 origin = self.transform.position;
            float radius = self.HitRadius * 0.6f;
            Vector3 best = forward;
            float bestScore = Score(origin, forward, distance, radius);
            for (int step = 1; step <= 3; step++)
            {
                float angle = step * 28f;
                Vector3 left = Quaternion.Euler(0f, -angle, 0f) * forward;
                Vector3 right = Quaternion.Euler(0f, angle, 0f) * forward;
                float leftScore = Score(origin, left, distance, radius);
                float rightScore = Score(origin, right, distance, radius);
                if (leftScore > bestScore) { best = left; bestScore = leftScore; }
                if (rightScore > bestScore) { best = right; bestScore = rightScore; }
            }
            Vector3 separation = Vector3.zero;
            if (GameWorld.Instance != null)
                foreach (EnemyController enemy in GameWorld.Instance.Enemies)
                {
                    if (enemy == null || enemy == self || enemy.IsDead) continue;
                    Vector3 away = origin - enemy.transform.position; away.y = 0f;
                    if (away.sqrMagnitude > 0.001f && away.sqrMagnitude < 2.25f) separation += away.normalized * (1.5f - Mathf.Sqrt(away.sqrMagnitude));
                }
            return (best + separation * 0.45f).normalized;
        }

        private static float Score(Vector3 origin, Vector3 direction, float distance, float radius)
        {
            Vector3 candidate = origin + direction * Mathf.Max(0.35f, distance);
            Vector3 resolved = DungeonObstacle.Resolve(candidate, radius);
            float freedom = 1f - Mathf.Clamp01((candidate - resolved).magnitude / Mathf.Max(0.35f, distance));
            return freedom + Vector3.Dot(direction, (candidate - origin).normalized) * 0.2f;
        }
    }

    public sealed class V1GameDirector : MonoBehaviour
    {
        public static V1GameDirector Instance { get; private set; }
        public GameFlowState State { get; private set; }
        public string ContextHint { get; private set; }
        public float ContextHintUntil { get; private set; }
        public string SaveStatus { get { return ProfileManager.LastSaveStatus; } }
        private float _autosaveAt;
        private float _playSecondsPending;
        private GameFlowState _previous;

        private void Awake()
        {
            Instance = this;
            State = GameFlowState.Boot;
        }

        private void Start()
        {
            ProfileManager.Current.firstLaunchComplete = true;
            ProfileManager.Save();
            State = V1TitleScreen.IsOpen ? GameFlowState.Title : GameFlowState.HomeBase;
            _autosaveAt = Time.unscaledTime + 45f;
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Update()
        {
            _previous = State;
            if (State != GameFlowState.Title && State != GameFlowState.Boot) _playSecondsPending += Time.unscaledDeltaTime;
            State = ResolveState();
            if (_previous != State && V1Diagnostics.Instance != null) V1Diagnostics.Instance.Record("State " + _previous + " -> " + State);
            if (Time.unscaledTime >= _autosaveAt)
            {
                _autosaveAt = Time.unscaledTime + 45f;
                SafeAutosave();
            }
            UpdateContextHints();
            if (ArcaneInput.GetKeyDown(KeyCode.F10) && V1Diagnostics.Instance != null)
            {
                try { ShowHint("diagnostic_export", "Diagnostic report exported: " + V1Diagnostics.Instance.ExportReport(), 4f); }
                catch (Exception exception) { V1Diagnostics.Instance.Recover("Diagnostic export failed.", exception); }
            }
        }

        public void SafeAutosave()
        {
            try
            {
                FlushPlayTime();
                ProfileManager.Save();
                GameWorld world = GameWorld.Instance;
                if (world != null && world.RunActive && !world.TrainingMode)
                {
                    RunDirector run = world.GetComponent<RunDirector>();
                    if (run != null && !run.EncounterActive) run.SaveRunCheckpoint();
                }
            }
            catch (Exception exception)
            {
                if (V1Diagnostics.Instance != null) V1Diagnostics.Instance.Recover("Autosave failed safely. Progress remains in memory.", exception);
            }
        }

        public void ShowHint(string id, string text, float duration = 5f)
        {
            ProfileData profile = ProfileManager.Current;
            if (profile.dismissedContextHints.Contains(id)) return;
            bool persistentTopic = !id.StartsWith("audio:") && !id.StartsWith("narrative:") && id != "diagnostic_export";
            if (persistentTopic && profile.completedTutorialTopics.Contains(id)) return;
            ContextHint = text;
            ContextHintUntil = Time.unscaledTime + duration;
            if (persistentTopic && !profile.completedTutorialTopics.Contains(id)) profile.completedTutorialTopics.Add(id);
        }

        public void DismissHint(string id)
        {
            if (!ProfileManager.Current.dismissedContextHints.Contains(id)) ProfileManager.Current.dismissedContextHints.Add(id);
            ProfileManager.Save();
            ContextHint = string.Empty;
        }

        private GameFlowState ResolveState()
        {
            if (V1TitleScreen.IsOpen) return GameFlowState.Title;
            GameWorld world = GameWorld.Instance;
            if (world == null) return GameFlowState.Boot;
            if (DemoUI.Instance != null && DemoUI.Instance.IsPaused) return GameFlowState.Paused;
            RunDirector run = world.GetComponent<RunDirector>();
            if (!world.RunActive) return run != null && run.HasRunResult ? GameFlowState.RunResult : GameFlowState.HomeBase;
            if (run == null) return GameFlowState.Exploring;
            if (run.PendingRewards) return GameFlowState.RewardSelection;
            if (run.PendingRoute) return GameFlowState.RouteSelection;
            if (run.PendingShop) return GameFlowState.Shop;
            if (run.PendingSafeRoom) return GameFlowState.SafeRoom;
            if (run.PendingExtraction) return GameFlowState.Extraction;
            return run.EncounterActive ? GameFlowState.Encounter : GameFlowState.Exploring;
        }

        private void UpdateContextHints()
        {
            if (ContextHintUntil > Time.unscaledTime) return;
            ContextHint = string.Empty;
            GameWorld world = GameWorld.Instance;
            if (world == null || !world.RunActive || world.ModalOpen) return;
            RunDirector run = world.GetComponent<RunDirector>();
            if (run != null && run.RoomIndex == 0 && run.EncounterActive)
                ShowHint("first_combat", "Move with WASD, aim anywhere with the mouse, cast with the mouse buttons, and dodge with Space.", 6f);
        }

        private void OnApplicationPause(bool paused) { if (paused) SafeAutosave(); }
        private void OnApplicationQuit() { SafeAutosave(); }

        private void FlushPlayTime()
        {
            long seconds = (long)_playSecondsPending;
            if (seconds <= 0) return;
            ProfileManager.Current.totalPlaySeconds += seconds;
            _playSecondsPending -= seconds;
        }
    }

    public sealed class NarrativeDirector : MonoBehaviour
    {
        public static NarrativeDirector Instance { get; private set; }
        private RunDirector _run;

        private void Awake() { Instance = this; }
        private void Start()
        {
            _run = GetComponent<RunDirector>();
            if (_run != null) _run.RoomEntered += OnRoomEntered;
        }
        private void OnDestroy()
        {
            if (_run != null) _run.RoomEntered -= OnRoomEntered;
            if (Instance == this) Instance = null;
        }

        private void OnRoomEntered(RoomTemplate room)
        {
            if (room == null || GameWorld.Instance == null || GameWorld.Instance.TrainingMode) return;
            string text = null;
            if (_run.RoomIndex == 0) text = "The Forge below remembers every spell ever broken. Bring back its heart.";
            else if (room.type == DungeonRoomType.NarrativeEvent) text = "An old inscription names the Warden: keeper, prisoner, and final key.";
            else if (room.type == DungeonRoomType.Secret) text = "A hidden seal opens. Someone expected a spellsmith to return.";
            else if (room.type == DungeonRoomType.Boss) text = "The Dungeon Warden bars the Relic Forge. Break the pillars. Break the oath.";
            else if (room.type == DungeonRoomType.Extraction) text = "The Forge answers your spell. Leave now, and carry its spark into the next descent.";
            else if (_run.RoomIndex == Mathf.Max(2, _run.TotalRooms / 2)) text = "The deeper halls are adapting to your magic. Change the question before they learn the answer.";
            if (string.IsNullOrEmpty(text)) return;
            GameWorld.Instance.Log("STORY · " + text);
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce(text, 4.2f);
            AdaptiveAudioSystem.PlayNarrativeCue("room_" + _run.RoomIndex + "_" + room.id, text);
            ProfileManager.Discover("story:" + room.id);
        }

        public void OnRunEnded(bool victory)
        {
            if (GameWorld.Instance == null) return;
            string text = victory
                ? "DEMO COMPLETE · The Relic Forge has awakened. Its deeper engines remain sealed for the full journey."
                : "The Forge keeps the shape of your failed spell. Essence returns with you; the descent waits.";
            GameWorld.Instance.Log(text);
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce(text, 5f);
            AdaptiveAudioSystem.PlayNarrativeCue(victory ? "demo_complete" : "run_failed", text);
        }
    }
}
