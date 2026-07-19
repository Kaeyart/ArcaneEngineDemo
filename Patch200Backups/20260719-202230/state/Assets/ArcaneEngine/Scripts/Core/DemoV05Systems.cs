using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum DemoRunMode { Standard, Daily, Endless }
    public enum RunLevelPerk { SpellPower, FastCasting, Vitality, ManaEfficiency, CriticalFocus, Movement, TriggerCapacity }
    public enum RoomObjectiveType
    {
        Eliminate, Survive, ProtectArtifact, DestroyPortals, PriorityTargets, InterruptRitual,
        EscalatingWaves, EscortOrb, HoldRunes, OrderedHunt, HuntInvisible, EscapeCollapse, FlawlessBonus
    }

    public sealed class RunStatistics : MonoBehaviour
    {
        public static RunStatistics Instance { get; private set; }
        public float DamageDealt { get; private set; }
        public float DamageTaken { get; private set; }
        public int CriticalHits { get; private set; }
        public int Dodges { get; private set; }
        public float BossFightSeconds { get; private set; }
        public int BossPillarsDestroyed { get; private set; }
        public int BossPhasesReached { get; private set; }
        public readonly Dictionary<string, float> SpellDamage = new Dictionary<string, float>();
        private float _bossStartedAt = -1f;

        private void Awake() { Instance = this; }
        public void BeginRun()
        {
            DamageDealt = DamageTaken = BossFightSeconds = 0f; CriticalHits = Dodges = BossPillarsDestroyed = 0;
            BossPhasesReached = 0; _bossStartedAt = -1f; SpellDamage.Clear();
        }
        public void Restore(float dealt, float taken, int criticalHits = 0, int dodges = 0)
        {
            BeginRun(); DamageDealt = Mathf.Max(0f, dealt); DamageTaken = Mathf.Max(0f, taken);
            CriticalHits = Mathf.Max(0, criticalHits); Dodges = Mathf.Max(0, dodges);
        }
        public void RecordDamageTaken(float value) { DamageTaken += Mathf.Max(0f, value); }
        public void RecordDodge() { Dodges++; }
        public void BeginBoss() { if (_bossStartedAt < 0f) _bossStartedAt = Time.time; BossPhasesReached = Mathf.Max(1, BossPhasesReached); }
        public void RecordBossPhase(int phase) { BossPhasesReached = Mathf.Max(BossPhasesReached, phase); }
        public void RecordBossPillar() { BossPillarsDestroyed++; }
        public void CompleteBoss() { if (_bossStartedAt >= 0f) BossFightSeconds = Mathf.Max(BossFightSeconds, Time.time - _bossStartedAt); }
        public void RecordSpellDamage(string spell, float value, bool critical)
        {
            value = Mathf.Max(0f, value); DamageDealt += value; if (critical) CriticalHits++;
            if (string.IsNullOrEmpty(spell)) spell = "Unknown Spell";
            if (!SpellDamage.ContainsKey(spell)) SpellDamage[spell] = 0f;
            SpellDamage[spell] += value;
        }
        public string BestSpell
        {
            get { return SpellDamage.Count == 0 ? "None" : SpellDamage.OrderByDescending(pair => pair.Value).First().Key; }
        }
    }

    public static class GameFeelSystem
    {
        public static void ElementImpact(Vector3 position, SpellElement element, Color color, bool critical, float damage)
        {
            // SpellVisualEvents and EnemyVisualEvents own the visible impact. Keeping a
            // second legacy burst here produced two presentation identities for one hit
            // and bypassed the bounded 2.0 visual pools. This layer now owns hit stop only.
            if (DemoV05Director.Instance != null && (critical || damage > 90f)) DemoV05Director.Instance.RequestHitStop(critical ? 0.045f : 0.025f);
        }

        public static void Burst(Vector3 position, Color color, float scale)
        {
            float density = ProfileManager.Current.accessibility.effectDensity;
            int count = Mathf.Clamp(Mathf.RoundToInt((3f + scale * 5f) * density), 2, 9);
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = UnityEngine.Random.onUnitSphere * scale * 0.45f;
                GameObject particle = RuntimeObjectPool.Rent("Impact Fragment", i % 2 == 0 ? PrimitiveType.Cube : PrimitiveType.Sphere,
                    position + offset, Vector3.one * UnityEngine.Random.Range(0.08f, 0.18f) * scale, color);
                if (particle == null) continue;
                TransientFragment fragment = particle.GetComponent<TransientFragment>();
                if (fragment == null) fragment = particle.AddComponent<TransientFragment>();
                fragment.Initialize(offset.normalized * UnityEngine.Random.Range(1.5f, 4f));
            }
        }
    }

    public sealed class TransientFragment : MonoBehaviour
    {
        public Vector3 velocity;
        private float _life = 0.35f;
        public void Initialize(Vector3 initialVelocity) { velocity = initialVelocity; _life = 0.35f; }
        private void OnEnable() { _life = 0.35f; }
        private void Update()
        {
            transform.position += velocity * Time.deltaTime; velocity += Vector3.down * 8f * Time.deltaTime;
            transform.localScale *= Mathf.Pow(0.06f, Time.deltaTime); _life -= Time.deltaTime;
            if (_life <= 0f) RuntimeObjectPool.Return(gameObject);
        }
    }

    public sealed class DemoV05Director : MonoBehaviour
    {
        public static DemoV05Director Instance { get; private set; }
        public RoomObjectiveSystem Objective { get; private set; }
        public DungeonMapState Map { get; private set; }
        public string Announcement { get; private set; }
        public float AnnouncementUntil { get; private set; }

        private RunDirector _run;
        private bool _stopping;
        private float _nextHitStopAt;

        private void Awake()
        {
            Instance = this;
            Map = new DungeonMapState();
            Objective = new RoomObjectiveSystem();
        }

        private void Start()
        {
            _run = GetComponent<RunDirector>();
            if (_run == null) return;
            _run.RoomEntered += OnRoomEntered;
            _run.RoomCleared += OnRoomCleared;
            _run.EnemyDefeated += OnEnemyDefeated;
        }

        private void OnDestroy()
        {
            if (_run == null) return;
            _run.RoomEntered -= OnRoomEntered; _run.RoomCleared -= OnRoomCleared; _run.EnemyDefeated -= OnEnemyDefeated;
        }

        private void Update()
        {
            if (_run == null || GameWorld.Instance == null || !GameWorld.Instance.RunActive || GameWorld.Instance.TrainingMode) return;
            Objective.Tick(GameWorld.Instance, _run);
        }

        private void OnRoomEntered(RoomTemplate room)
        {
            Map.Enter(room, _run.RoomIndex);
            if (_run.EncounterActive)
            {
                Objective.Begin(room, _run);
                DungeonDecorator.Decorate(room, _run.RoomIndex, _run.CurrentSeed);
                Announce(room.displayName.ToUpperInvariant() + "  ·  " + Objective.Title, 2.7f);
            }
            else
            {
                Objective.Clear();
                Announce(room.displayName.ToUpperInvariant(), 2.2f);
            }
        }

        private void OnRoomCleared(RoomTemplate room)
        {
            Map.ClearCurrent();
            int bonus = Objective.BonusExperience;
            if (bonus > 0) _run.AddObjectiveBonus(bonus);
        }

        private void OnEnemyDefeated(EnemyController enemy) { Objective.OnEnemyDefeated(enemy); }

        public bool CanCompleteEncounter(bool enemiesEmpty)
        {
            bool complete = Objective.CanComplete(enemiesEmpty);
            if (complete && !enemiesEmpty && Objective.AllowsLivingEnemiesAtCompletion)
            {
                foreach (EnemyController enemy in GameWorld.Instance.Enemies.ToArray()) if (enemy != null) Destroy(enemy.gameObject);
                GameWorld.Instance.Enemies.Clear();
            }
            return complete;
        }

        public void ShowRunLevelUp(int level) { Announce("RUN LEVEL " + level + "  ·  CHOOSE ONE OF THREE RUN PERKS", 2.8f); }
        public void Announce(string text, float duration) { Announcement = text; AnnouncementUntil = Time.unscaledTime + duration; }
        public void ResetRunPresentation() { Objective.Clear(); Map.Reset(); Announcement = string.Empty; AnnouncementUntil = 0f; }

        public void RequestHitStop(float duration)
        {
            if (ProfileManager.Current != null && ProfileManager.Current.accessibility.reducedMotion) return;
            float strength = ProfileManager.Current == null ? 0.65f : ProfileManager.Current.accessibility.hitStop;
            duration *= Mathf.Clamp01(strength);
            if (duration <= 0.001f || _stopping || !gameObject.activeInHierarchy || Time.unscaledTime < _nextHitStopAt) return;
            _nextHitStopAt = Time.unscaledTime + 0.12f;
            StartCoroutine(HitStop(duration));
        }

        private IEnumerator HitStop(float duration)
        {
            _stopping = true; float previous = Time.timeScale; Time.timeScale = 0.03f;
            yield return new WaitForSecondsRealtime(duration);
            if (GameWorld.Instance != null && !GameWorld.Instance.ModalOpen) Time.timeScale = previous;
            _stopping = false;
        }
    }

    public sealed class DungeonMapState
    {
        public sealed class Node { public string name; public DungeonRoomType type; public bool cleared; public int depth; }
        public readonly List<Node> Nodes = new List<Node>();
        public void Reset() { Nodes.Clear(); }
        public void Enter(RoomTemplate room, int depth) { Nodes.Add(new Node { name = room.displayName, type = room.type, depth = depth }); }
        public void ClearCurrent() { if (Nodes.Count > 0) Nodes[Nodes.Count - 1].cleared = true; }
        public string Compact
        {
            get { return string.Join("  ›  ", Nodes.Skip(Mathf.Max(0, Nodes.Count - 5)).Select(node => (node.cleared ? "◆" : "◇") + Short(node.type))); }
        }
        private static string Short(DungeonRoomType type)
        {
            if (type == DungeonRoomType.Boss) return "BOSS"; if (type == DungeonRoomType.Shop) return "SHOP";
            if (type == DungeonRoomType.Elite || type == DungeonRoomType.Miniboss) return "ELITE";
            if (type == DungeonRoomType.HealingSanctuary) return "HEAL"; return "ROOM";
        }
    }

    public sealed class RoomObjectiveSystem
    {
        public RoomObjectiveType Type { get; private set; }
        public string Title { get; private set; }
        public string Detail { get; private set; }
        public float Progress { get; private set; }
        public int BonusExperience { get; private set; }
        public bool AllowsLivingEnemiesAtCompletion { get; private set; }

        private readonly List<ObjectiveNode> _nodes = new List<ObjectiveNode>();
        private readonly List<EnemyController> _priorityTargets = new List<EnemyController>();
        private float _timer;
        private float _target;
        private float _startHealth;
        private int _wave;
        private int _initialEnemies;
        private int _orderedProgress;
        private bool _complete;
        private GameObject _escort;
        private Vector3 _escortTarget;
        private LineRenderer _zone;
        private GameObject _optionalMarker;
        private RoomTemplate _room;

        public void Begin(RoomTemplate room, RunDirector run)
        {
            Clear(); _room = room; BonusExperience = 0; AllowsLivingEnemiesAtCompletion = false;
            if (room.type == DungeonRoomType.Boss) Type = RoomObjectiveType.Eliminate;
            else Type = (RoomObjectiveType)(Mathf.Abs(run.CurrentSeed + run.RoomIndex * 37) % Enum.GetValues(typeof(RoomObjectiveType)).Length);
            _startHealth = GameWorld.Instance.Player.Health;
            _initialEnemies = Mathf.Max(1, GameWorld.Instance.Enemies.Count);
            Configure(run);
        }

        private void Configure(RunDirector run)
        {
            switch (Type)
            {
                case RoomObjectiveType.Survive:
                    Title = "SURVIVE THE ONSLAUGHT"; _timer = _target = 22f; AllowsLivingEnemiesAtCompletion = true; break;
                case RoomObjectiveType.ProtectArtifact:
                    Title = "PROTECT THE ARCANE RELIC"; _timer = _target = 25f; SpawnNode(new Vector3(0f, 0f, 3f), "ARCANE RELIC", 8, true); break;
                case RoomObjectiveType.DestroyPortals:
                    Title = "DESTROY THE SUMMONING PORTALS"; SpawnNodes("PORTAL", 3, 2); break;
                case RoomObjectiveType.PriorityTargets:
                    Title = "ELIMINATE THE MARKED TARGETS"; MarkPriorityTargets(); break;
                case RoomObjectiveType.InterruptRitual:
                    Title = "INTERRUPT THE RITUAL"; SpawnNodes("RITUAL FOCUS", 3, 3); break;
                case RoomObjectiveType.EscalatingWaves:
                    Title = "BREAK THREE ESCALATING WAVES"; _wave = 1; break;
                case RoomObjectiveType.EscortOrb:
                    Title = "ESCORT THE ORB TO THE NORTH SEAL"; CreateEscort(); break;
                case RoomObjectiveType.HoldRunes:
                    Title = "HOLD THE CENTRAL RUNE"; _target = 10f; CreateZone(Vector3.zero, 2.7f, new Color(0.2f, 0.8f, 1f)); break;
                case RoomObjectiveType.OrderedHunt:
                    Title = "ORDERED HUNT"; Detail = "Defeat Tank → Ranged → Chaser roles in sequence for a bonus."; break;
                case RoomObjectiveType.HuntInvisible:
                    Title = "HUNT THE VEILED ENEMIES"; Detail = "Hidden enemies become visible when you move close."; VeilEnemies(); break;
                case RoomObjectiveType.EscapeCollapse:
                    Title = "ESCAPE TO THE NORTH SEAL"; Detail = "Reach the glowing seal before the collapse."; _timer = _target = 18f; AllowsLivingEnemiesAtCompletion = true; CreateZone(new Vector3(0f, 0f, 12.5f), 2.2f, new Color(1f, 0.28f, 0.08f)); break;
                case RoomObjectiveType.FlawlessBonus:
                    Title = "FLAWLESS CHALLENGE"; Detail = "Clear the room without taking damage for +30 XP.";
                    _optionalMarker = ObjectiveStateMarker.Create(new Vector3(0f, 0.08f, -8.4f), "FLAWLESS BONUS", ObjectiveWorldState.Optional); break;
                default:
                    Title = "ELIMINATE ALL ENEMIES"; Detail = "Break armor, interrupt attacks, and clear the room."; break;
            }
        }

        public void Tick(GameWorld world, RunDirector run)
        {
            if (_complete || !run.EncounterActive) { UpdateDetail(world, run); return; }
            if (Type == RoomObjectiveType.Survive)
            {
                _timer -= Time.deltaTime; Progress = 1f - Mathf.Clamp01(_timer / _target); if (_timer <= 0f) _complete = true;
            }
            else if (Type == RoomObjectiveType.ProtectArtifact)
            {
                _timer -= Time.deltaTime; Progress = 1f - Mathf.Clamp01(_timer / _target);
                ObjectiveNode relic = _nodes.FirstOrDefault(node => node != null);
                int attackers = relic == null ? 0 : world.Enemies.Count(enemy => enemy != null &&
                    CombatMath.PlanarDistanceSquared(enemy.transform.position, relic.transform.position) < 20f);
                if (relic != null) relic.ApplyPressure(attackers * Time.deltaTime);
                if (relic == null || relic.ChargesRemaining <= 0) { Type = RoomObjectiveType.Eliminate; Title = "THE RELIC FELL · ELIMINATE THE ATTACKERS"; }
                else if (_timer <= 0f) _complete = true;
            }
            else if (Type == RoomObjectiveType.DestroyPortals || Type == RoomObjectiveType.InterruptRitual)
            {
                int remaining = _nodes.Count(node => node != null && node.ChargesRemaining > 0); Progress = 1f - remaining / (float)Mathf.Max(1, _nodes.Count);
                if (remaining == 0 && world.Enemies.Count == 0) _complete = true;
            }
            else if (Type == RoomObjectiveType.EscalatingWaves && world.Enemies.Count == 0 && !run.ReinforcementsPending)
            {
                if (_wave >= 3) _complete = true;
                else { _wave++; run.SpawnObjectiveWave(3 + _wave * 2, _wave == 3); }
                Progress = _wave / 3f;
            }
            else if (Type == RoomObjectiveType.EscortOrb)
            {
                if (_escort == null) { _complete = true; return; }
                Vector3 player = world.Player.transform.position; Vector3 delta = player - _escort.transform.position; delta.y = 0f;
                if (delta.sqrMagnitude < 14f) _escort.transform.position = Vector3.MoveTowards(_escort.transform.position, _escortTarget, 1.7f * Time.deltaTime);
                Progress = Mathf.InverseLerp(-8f, _escortTarget.z, _escort.transform.position.z);
                if ((_escort.transform.position - _escortTarget).sqrMagnitude < 0.3f && world.Enemies.Count == 0) _complete = true;
            }
            else if (Type == RoomObjectiveType.HoldRunes)
            {
                Vector3 delta = world.Player.transform.position; delta.y = 0f;
                if (delta.sqrMagnitude <= 2.7f * 2.7f) _timer += Time.deltaTime;
                else _timer = Mathf.Max(0f, _timer - Time.deltaTime * 0.4f);
                Progress = Mathf.Clamp01(_timer / _target); if (_timer >= _target && world.Enemies.Count == 0) _complete = true;
            }
            else if (Type == RoomObjectiveType.EscapeCollapse)
            {
                _timer -= Time.deltaTime; Progress = Mathf.Clamp01(world.Player.transform.position.z / 12.5f);
                if (world.Player.transform.position.z >= 11.2f) _complete = true;
                else if (_timer <= 0f) { world.Player.TakeDamage(18f); _timer = 8f; }
            }
            else if (Type == RoomObjectiveType.PriorityTargets)
            {
                _priorityTargets.RemoveAll(enemy => enemy == null || enemy.IsDead);
                Progress = 1f - _priorityTargets.Count / 2f;
                if (_priorityTargets.Count == 0) { _complete = true; AllowsLivingEnemiesAtCompletion = true; }
            }
            else if (Type == RoomObjectiveType.Eliminate || Type == RoomObjectiveType.HuntInvisible || Type == RoomObjectiveType.FlawlessBonus || Type == RoomObjectiveType.OrderedHunt)
                Progress = 1f - world.Enemies.Count / (float)_initialEnemies;
            UpdateDetail(world, run);
        }

        private void UpdateDetail(GameWorld world, RunDirector run)
        {
            if (Type == RoomObjectiveType.Survive || Type == RoomObjectiveType.ProtectArtifact || Type == RoomObjectiveType.EscapeCollapse)
                Detail = Mathf.CeilToInt(Mathf.Max(0f, _timer)) + " seconds";
            else if (Type == RoomObjectiveType.EscalatingWaves) Detail = "Wave " + _wave + " / 3";
            else if (Type == RoomObjectiveType.HoldRunes) Detail = _timer.ToString("0.0") + " / " + _target.ToString("0") + " seconds secured";
            else if (Type == RoomObjectiveType.EscortOrb) Detail = Mathf.RoundToInt(Progress * 100f) + "% escorted";
            else if (Type == RoomObjectiveType.PriorityTargets) Detail = _priorityTargets.Count + " marked target(s) remain";
            else if (string.IsNullOrEmpty(Detail)) Detail = world.Enemies.Count + " enemies remaining";
        }

        public bool CanComplete(bool enemiesEmpty)
        {
            if (Type == RoomObjectiveType.Eliminate || Type == RoomObjectiveType.HuntInvisible)
                return enemiesEmpty;
            if (Type == RoomObjectiveType.FlawlessBonus)
            {
                bool succeeded = GameWorld.Instance.Player.Health >= _startHealth - 0.01f;
                if (enemiesEmpty && succeeded) BonusExperience = 30;
                ObjectiveStateMarker marker = _optionalMarker == null ? null : _optionalMarker.GetComponent<ObjectiveStateMarker>();
                if (marker != null && enemiesEmpty) marker.SetState(succeeded ? ObjectiveWorldState.Complete : ObjectiveWorldState.Failed);
                return enemiesEmpty;
            }
            if (Type == RoomObjectiveType.OrderedHunt) { if (enemiesEmpty && _orderedProgress >= 3) BonusExperience = 25; return enemiesEmpty; }
            return _complete;
        }

        public void OnEnemyDefeated(EnemyController enemy)
        {
            if (Type != RoomObjectiveType.OrderedHunt || enemy == null) return;
            string expected = _orderedProgress == 0 ? "TANK" : _orderedProgress == 1 ? "RANGED" : "CHASER";
            if (enemy.CombatRole == expected) _orderedProgress++; else _orderedProgress = 0;
            Detail = "Sequence " + _orderedProgress + " / 3 · next " + (_orderedProgress == 0 ? "TANK" : _orderedProgress == 1 ? "RANGED" : "CHASER");
        }

        private void SpawnNodes(string label, int count, int charges)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f;
                SpawnNode(new Vector3(Mathf.Cos(angle) * 7f, 0f, Mathf.Sin(angle) * 6f + 2f), label + " " + (i + 1), charges);
            }
        }

        private void SpawnNode(Vector3 position, string label, int charges, bool protectedArtifact = false) { _nodes.Add(ObjectiveNode.Create(position, label, charges, protectedArtifact)); }

        private void CreateEscort()
        {
            _escort = RuntimeVisuals.Primitive("Escort Orb", PrimitiveType.Sphere, new Vector3(0f, 0.8f, -9f), Vector3.one * 0.8f, new Color(0.3f, 0.9f, 1f));
            RuntimeVisuals.RemoveCollider(_escort); _escortTarget = new Vector3(0f, 0.8f, 11.5f);
            _zone = RuntimeVisuals.Ring("Escort Destination", _escortTarget, new Color(0.3f, 0.9f, 1f), 1.8f, 0.12f);
        }

        private void CreateZone(Vector3 position, float radius, Color color) { _zone = RuntimeVisuals.Ring("Objective Zone", position, color, radius, 0.16f); }

        private void VeilEnemies()
        {
            foreach (EnemyController enemy in GameWorld.Instance.Enemies)
            {
                Renderer renderer = enemy == null ? null : enemy.GetComponent<Renderer>();
                if (renderer != null) enemy.gameObject.AddComponent<VeiledEnemy>().targetRenderer = renderer;
            }
        }

        private void MarkPriorityTargets()
        {
            _priorityTargets.Clear();
            foreach (EnemyController enemy in GameWorld.Instance.Enemies.Where(value => value != null && !value.IsDead)
                .OrderByDescending(value => value.IsEliteOrBoss).ThenByDescending(value => value.MaxHealth).Take(2))
            {
                _priorityTargets.Add(enemy);
                LineRenderer mark = RuntimeVisuals.Ring("Priority Target Mark", enemy.transform.position, new Color(1f, 0.16f, 0.08f),
                    enemy.HitRadius + 0.55f, 0.13f, enemy.transform);
                mark.transform.localPosition = Vector3.zero;
            }
            Detail = _priorityTargets.Count + " marked target(s) remain";
        }

        public void Clear()
        {
            foreach (ObjectiveNode node in _nodes) if (node != null) UnityEngine.Object.Destroy(node.gameObject); _nodes.Clear();
            if (_escort != null) UnityEngine.Object.Destroy(_escort); if (_zone != null) UnityEngine.Object.Destroy(_zone.gameObject);
            if (_optionalMarker != null) UnityEngine.Object.Destroy(_optionalMarker);
            _escort = null; _zone = null; _optionalMarker = null; _timer = _target = Progress = 0f; _complete = false; _orderedProgress = 0;
            _priorityTargets.Clear();
        }
    }

    public sealed class ObjectiveNode : WorldRoomInteractable
    {
        public int ChargesRemaining { get; private set; }
        private float _pressure;
        private bool _protectedArtifact;
        private ObjectiveStateMarker _stateMarker;
        public static ObjectiveNode Create(Vector3 position, string label, int charges, bool protectedArtifact = false)
        {
            GameObject root = new GameObject(label); root.transform.position = position;
            ObjectiveNode node = root.AddComponent<ObjectiveNode>(); node.ChargesRemaining = charges; node._protectedArtifact = protectedArtifact;
            node.PromptTitle = label; node.PromptAction = protectedArtifact ? "DEFEND" : "CHANNEL";
            node.PromptDescription = protectedArtifact ? "Enemies damage its stability while they remain nearby." : "Interact or strike it with spells to disrupt this objective.";
            GameObject visual = RuntimeVisuals.Primitive(label, PrimitiveType.Cylinder, position + Vector3.up * 0.7f, new Vector3(0.8f, 0.8f, 0.8f), new Color(0.8f, 0.15f, 1f), root.transform);
            RuntimeVisuals.RemoveCollider(visual); visual.transform.localPosition = Vector3.up * 0.7f;
            RuntimeVisuals.Ring(label + " Ring", position, new Color(0.8f, 0.15f, 1f), 1.1f, 0.12f, root.transform).transform.localPosition = Vector3.zero;
            BiomeLightingProfile lighting = ProceduralLightingDirector.CurrentProfile;
            PriorityLightAnchor.Attach(root, lighting == null ? new Color(0.8f, 0.15f, 1f) : lighting.objectivePriority, 4.2f, 0.62f, 4);
            node._stateMarker = root.AddComponent<ObjectiveStateMarker>();
            node._stateMarker.Initialize(label, ObjectiveWorldState.Active);
            node.CreateLabel(Color.white); return node;
        }
        public override void Interact()
        {
            if (_protectedArtifact) return;
            Damage(1);
        }
        public bool Damage(int stages)
        {
            if (_protectedArtifact) return false;
            ChargesRemaining -= Mathf.Max(1, stages); PromptDescription = Mathf.Max(0, ChargesRemaining) + " channel stages remain.";
            GameFeelSystem.Burst(transform.position + Vector3.up * 0.6f, new Color(0.8f, 0.15f, 1f), 0.8f);
            if (ChargesRemaining <= 0) { if (_stateMarker != null) _stateMarker.SetState(ObjectiveWorldState.Complete); Destroy(gameObject, 0.18f); }
            return true;
        }
        public void ApplyPressure(float amount)
        {
            _pressure += Mathf.Max(0f, amount);
            if (_pressure < 1f) return;
            int damage = Mathf.FloorToInt(_pressure); _pressure -= damage; ChargesRemaining = Mathf.Max(0, ChargesRemaining - damage);
            PromptDescription = ChargesRemaining + " stability remains.";
            if (ChargesRemaining <= 0 && _stateMarker != null) _stateMarker.SetState(ObjectiveWorldState.Failed);
        }
    }

    public enum ObjectiveWorldState { Active, Complete, Failed, Optional }

    public sealed class ObjectiveStateMarker : MonoBehaviour
    {
        private LineRenderer _ring;
        private TextMesh _symbol;
        private ObjectiveWorldState _state;

        public static GameObject Create(Vector3 position, string label, ObjectiveWorldState state)
        {
            GameObject root = new GameObject("Objective Indicator · " + label);
            root.transform.position = position;
            ObjectiveStateMarker marker = root.AddComponent<ObjectiveStateMarker>();
            marker.Initialize(label, state);
            return root;
        }

        public void Initialize(string label, ObjectiveWorldState state)
        {
            if (_ring == null)
            {
                _ring = RuntimeVisuals.Ring("Objective State Boundary", transform.position, Color.white, 1.35f, 0.07f, transform);
                _ring.transform.localPosition = Vector3.zero;
                GameObject textRoot = new GameObject("Objective State Symbol");
                textRoot.transform.SetParent(transform, false);
                _symbol = textRoot.AddComponent<TextMesh>();
                _symbol.anchor = TextAnchor.MiddleCenter; _symbol.alignment = TextAlignment.Center; _symbol.fontSize = 48; _symbol.characterSize = 0.12f;
                _symbol.transform.localPosition = Vector3.up * 0.18f; _symbol.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }
            SetState(state);
        }

        public void SetState(ObjectiveWorldState state)
        {
            _state = state;
            Color color = state == ObjectiveWorldState.Active ? new Color(0.25f, 0.9f, 1f) : state == ObjectiveWorldState.Complete ? new Color(0.2f, 1f, 0.55f) :
                state == ObjectiveWorldState.Failed ? new Color(1f, 0.12f, 0.16f) : new Color(1f, 0.78f, 0.22f);
            if (_ring != null) { _ring.startColor = _ring.endColor = color; _ring.startWidth = _ring.endWidth = state == ObjectiveWorldState.Active ? 0.1f : 0.065f; }
            if (_symbol != null) { _symbol.text = state == ObjectiveWorldState.Active ? "!" : state == ObjectiveWorldState.Complete ? "✓" : state == ObjectiveWorldState.Failed ? "×" : "+"; _symbol.color = color; }
        }

        private void Update()
        {
            if (_ring == null) return;
            float rate = _state == ObjectiveWorldState.Active ? 3f : _state == ObjectiveWorldState.Optional ? 1.2f : 0f;
            _ring.transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.unscaledTime * rate) * (rate <= 0f ? 0f : 0.05f));
        }
    }

    public sealed class VeiledEnemy : MonoBehaviour
    {
        public Renderer targetRenderer;
        private void Update()
        {
            if (targetRenderer == null || GameWorld.Instance == null || GameWorld.Instance.Player == null) return;
            bool visible = (GameWorld.Instance.Player.transform.position - transform.position).sqrMagnitude < 30f;
            targetRenderer.enabled = visible;
        }
    }

    public static class DungeonDecorator
    {
        public static void Decorate(RoomTemplate room, int depth, int seed)
        {
            UnityEngine.Random.State state = UnityEngine.Random.state; UnityEngine.Random.InitState(seed ^ depth * 193);
            for (int i = 0; i < 7; i++)
            {
                Vector3 position = new Vector3(UnityEngine.Random.Range(-13f, 13f), 0f, UnityEngine.Random.Range(-12f, 12f));
                if (position.sqrMagnitude < 20f) position += position.normalized * 5f;
                BreakableProp.Create(position, i % 3 == 0 ? "Ancient Urn" : i % 3 == 1 ? "Bone Crate" : "Rune Statue", room.accentColor);
            }
            for (int i = 0; i < 2; i++) DungeonBladeTrap.Create(new Vector3(UnityEngine.Random.Range(-9f, 9f), 0.25f, UnityEngine.Random.Range(-6f, 9f)), room.accentColor);
            DungeonChest.Create(new Vector3(-12f, 0f, 11f), false);
            SecretSwitch.Create(new Vector3(12.5f, 0f, -8f));
            UnityEngine.Random.state = state;
        }
    }

    public sealed class BreakableProp : MonoBehaviour
    {
        private static readonly List<BreakableProp> ActiveProps = new List<BreakableProp>();
        public static IReadOnlyList<BreakableProp> Active { get { return ActiveProps; } }
        private float _health = 24f;
        private bool _dropsLoot = true;
        public event Action Destroyed;

        private void OnEnable()
        {
            if (!ActiveProps.Contains(this)) ActiveProps.Add(this);
        }

        private void OnDisable()
        {
            ActiveProps.Remove(this);
        }

        public static BreakableProp Create(Vector3 position, string label, Color color)
        {
            GameObject go = RuntimeVisuals.Primitive(label, label.Contains("Urn") ? PrimitiveType.Cylinder : PrimitiveType.Cube,
                position + Vector3.up * 0.55f, Vector3.one * UnityEngine.Random.Range(0.55f, 0.95f), Color.Lerp(color, new Color(0.28f, 0.22f, 0.18f), 0.65f));
            BreakableProp prop = go.AddComponent<BreakableProp>(); return prop;
        }

        public BreakableProp SetHealth(float health, bool dropsLoot = true)
        {
            _health = Mathf.Max(1f, health);
            _dropsLoot = dropsLoot;
            return this;
        }

        public void Damage(float amount)
        {
            _health -= amount; GameFeelSystem.Burst(transform.position, GetComponent<Renderer>().sharedMaterial.color, 0.45f);
            if (_health > 0f) return;
            if (Destroyed != null) Destroyed();
            if (_dropsLoot && UnityEngine.Random.value < 0.32f) GameWorld.Instance.SpawnLoot(transform.position);
            Destroy(gameObject);
        }
    }

    public sealed class DungeonChest : WorldRoomInteractable
    {
        private bool _rare;
        public static DungeonChest Create(Vector3 position, bool rare)
        {
            GameObject root = new GameObject(rare ? "Secret Chest" : "Dungeon Chest"); root.transform.position = position;
            DungeonChest chest = root.AddComponent<DungeonChest>(); chest._rare = rare;
            chest.PromptTitle = rare ? "SECRET CHEST" : "DUNGEON CHEST"; chest.PromptAction = "OPEN"; chest.PromptDescription = rare ? "Contains improved loot." : "Contains Gold or a dungeon item.";
            GameObject visual = RuntimeVisuals.Primitive("Chest", PrimitiveType.Cube, position + Vector3.up * 0.45f, new Vector3(1.2f, 0.75f, 0.8f), rare ? new Color(0.85f, 0.35f, 1f) : new Color(0.65f, 0.4f, 0.12f), root.transform);
            RuntimeVisuals.RemoveCollider(visual); visual.transform.localPosition = Vector3.up * 0.45f; chest.CreateLabel(Color.white); return chest;
        }
        public override void Interact()
        {
            RunDirector run = GameWorld.Instance.GetComponent<RunDirector>(); run.AddDrachmas(_rare ? 35 : 14);
            if (_rare || UnityEngine.Random.value < 0.45f) GameWorld.Instance.SpawnLoot(transform.position + Vector3.up * 0.4f, _rare);
            GameFeelSystem.Burst(transform.position + Vector3.up, _rare ? Color.magenta : Color.yellow, 1f); Destroy(gameObject);
        }
    }

    public sealed class SecretSwitch : WorldRoomInteractable
    {
        public static SecretSwitch Create(Vector3 position)
        {
            GameObject root = RuntimeVisuals.Primitive("Hidden Rune Switch", PrimitiveType.Cylinder, position + Vector3.up * 0.12f, new Vector3(0.5f, 0.08f, 0.5f), new Color(0.28f, 0.3f, 0.34f));
            SecretSwitch secret = root.AddComponent<SecretSwitch>(); secret.PromptTitle = "FAINT WALL RUNE"; secret.PromptAction = "REVEAL SECRET"; secret.PromptDescription = "A hidden mechanism hums behind the stone."; secret.CreateLabel(new Color(0.6f, 0.8f, 1f)); return secret;
        }
        public override void Interact()
        {
            DungeonChest.Create(new Vector3(12f, 0f, 11f), true); GameFeelSystem.Burst(transform.position, new Color(0.5f, 0.8f, 1f), 1f);
            GameWorld.Instance.Log("A secret compartment opens across the room."); Destroy(gameObject);
        }
    }

    public sealed class DungeonBladeTrap : MonoBehaviour
    {
        private float _tick;
        public static DungeonBladeTrap Create(Vector3 position, Color color)
        {
            GameObject root = RuntimeVisuals.Primitive("Rotating Blade Trap", PrimitiveType.Cube, position, new Vector3(3f, 0.15f, 0.35f), Color.Lerp(color, Color.red, 0.45f));
            DungeonBladeTrap trap = root.AddComponent<DungeonBladeTrap>(); return trap;
        }
        private void Update()
        {
            transform.Rotate(0f, 150f * Time.deltaTime, 0f); _tick -= Time.deltaTime;
            if (_tick <= 0f && GameWorld.Instance != null && GameWorld.Instance.Player != null &&
                (GameWorld.Instance.Player.transform.position - transform.position).sqrMagnitude < 8f)
            { _tick = 0.8f; GameWorld.Instance.Player.TakeDamage(9f); }
        }
    }

    public sealed class RunLevelChoicePickup : WorldRoomInteractable
    {
        private RunDirector _run;
        private RunLevelPerk _perk;

        public static void SpawnChoices(RunDirector run, int runLevel)
        {
            WorldInteractionController.ClearType<RunLevelChoicePickup>();
            List<RunLevelPerk> pool = Enum.GetValues(typeof(RunLevelPerk)).Cast<RunLevelPerk>()
                .OrderBy(value => Mathf.Abs(run.CurrentSeed + runLevel * 71 + (int)value * 193)).Take(3).ToList();
            for (int i = 0; i < pool.Count; i++) Create(run, pool[i], new Vector3(-4.5f + i * 4.5f, 0f, -1.5f));
        }

        private static void Create(RunDirector run, RunLevelPerk perk, Vector3 position)
        {
            GameObject root = new GameObject("Run Perk · " + perk); root.transform.position = position;
            RunLevelChoicePickup choice = root.AddComponent<RunLevelChoicePickup>(); choice._run = run; choice._perk = perk;
            choice.PromptTitle = FriendlyName(perk); choice.PromptAction = "CHOOSE RUN PERK"; choice.PromptDescription = Description(perk);
            Color color = new Color(0.18f + (int)perk * 0.08f, 0.82f - (int)perk * 0.05f, 1f);
            GameObject visual = RuntimeVisuals.Primitive("Run Perk", PrimitiveType.Cube, position + Vector3.up * 0.85f, Vector3.one * 0.75f, color, root.transform);
            RuntimeVisuals.RemoveCollider(visual); visual.transform.localPosition = Vector3.up * 0.85f;
            visual.transform.localRotation = Quaternion.Euler(0f, 45f, 45f);
            RuntimeVisuals.Ring("Run Perk Ring", position, color, 1f, 0.1f, root.transform).transform.localPosition = Vector3.zero;
            choice.CreateLabel(Color.white);
        }

        public override void Interact()
        {
            if (_run == null) return;
            _run.ApplyRunLevelPerk(_perk);
            WorldInteractionController.ClearType<RunLevelChoicePickup>();
            _run.ResumeAfterPerkChoice();
        }

        private static string FriendlyName(RunLevelPerk perk)
        {
            if (perk == RunLevelPerk.SpellPower) return "ARCANE FORCE";
            if (perk == RunLevelPerk.FastCasting) return "QUICK CASTING";
            if (perk == RunLevelPerk.ManaEfficiency) return "MANA CONTROL";
            if (perk == RunLevelPerk.CriticalFocus) return "DEADLY FOCUS";
            if (perk == RunLevelPerk.TriggerCapacity) return "TRIGGER RESERVOIR";
            return perk.ToString().ToUpperInvariant();
        }

        private static string Description(RunLevelPerk perk)
        {
            if (perk == RunLevelPerk.SpellPower) return "+8% spell power for this run.";
            if (perk == RunLevelPerk.FastCasting) return "8% faster cooldown recovery for this run.";
            if (perk == RunLevelPerk.Vitality) return "+15 maximum Health for this run.";
            if (perk == RunLevelPerk.ManaEfficiency) return "Spells cost 9% less Mana for this run.";
            if (perk == RunLevelPerk.CriticalFocus) return "+6% critical chance for this run.";
            if (perk == RunLevelPerk.Movement) return "+0.7 movement speed for this run.";
            return "+20 Trigger Energy for chained spells this run.";
        }
    }
}
