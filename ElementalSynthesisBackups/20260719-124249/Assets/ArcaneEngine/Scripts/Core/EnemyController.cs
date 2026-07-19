using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum EnemyArchetype
    {
        Crawler, Bulwark, Hexer, Charger, Warden, Leech, Mirror, Assassin, Controller,
        OssuaryWarden, EmberTitan, ArchiveSeraph, VenomMatriarch, TrainingDummy
    }
    public enum EliteAffix { None, Frenzied, Shielded, Volatile, Vampiric, Resistant, Summoner }

    public sealed class EnemyController : MonoBehaviour, IDamageable, ITargetable, IStatusReceiver
    {
        public bool IsDead { get; private set; }
        public bool IsEliteOrBoss { get; private set; }
        public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        public EnemyArchetype Archetype { get { return _archetype; } }
        public bool IsBoss { get { return IsBossArchetype(_archetype); } }
        public float HealthRatio { get { return MaxHealth <= 0f ? 0f : Mathf.Clamp01(Health / MaxHealth); } }
        public float ShieldRatio { get { return MaxHealth <= 0f ? 0f : Mathf.Clamp01(_shield / MaxHealth); } }
        public float ArmorRatio { get { return _maxArmor <= 0f ? 0f : Mathf.Clamp01(_armor / _maxArmor); } }
        public SpellElement AdaptedElement { get { return _adaptedElement; } }
        public EnemyBrainState BrainState { get; private set; } = EnemyBrainState.Spawning;
        public bool IsAlive { get { return !IsDead && Health > 0f; } }
        public Vector3 DamagePoint { get { return HealthBarWorldPosition - Vector3.up * 0.35f; } }
        public bool CanBeTargeted { get { return IsAlive; } }
        public Vector3 TargetPoint { get { return DamagePoint; } }
        public SpellElement WeakElement { get { return _weakElement; } }
        public IReadOnlyList<EliteAffix> ActiveAffixes { get { return _affixes; } }
        public float HitRadius
        {
            get
            {
                if (_collider == null) return IsBoss ? 1.2f : 0.5f;
                Vector3 extents = _collider.bounds.extents;
                return Mathf.Max(0.35f, Mathf.Max(extents.x, extents.z));
            }
        }
        public string DisplayName
        {
            get
            {
                string name = _archetype.ToString().Replace("OssuaryWarden", "Dungeon Warden")
                    .Replace("EmberTitan", "Ember Titan").Replace("ArchiveSeraph", "Archive Seraph")
                    .Replace("VenomMatriarch", "Venom Matriarch");
                return (IsEliteOrBoss && !IsBoss ? "Elite " : string.Empty) + name;
            }
        }
        public string CombatRole
        {
            get
            {
                if (_archetype == EnemyArchetype.Bulwark || _archetype == EnemyArchetype.Warden) return "TANK";
                if (_archetype == EnemyArchetype.Hexer || _archetype == EnemyArchetype.Mirror) return "RANGED";
                if (_archetype == EnemyArchetype.Leech) return "SUPPORT";
                if (_archetype == EnemyArchetype.Assassin) return "ASSASSIN";
                if (_archetype == EnemyArchetype.Controller) return "CONTROLLER";
                return _archetype == EnemyArchetype.Charger ? "DISRUPTOR" : "CHASER";
            }
        }
        public Vector3 HealthBarWorldPosition
        {
            get
            {
                return transform.position + Vector3.up * Mathf.Max(1.5f, _healthBarLocalHeight);
            }
        }
        public string StatusSummary
        {
            get
            {
                List<string> states = new List<string>();
                if (Time.time < _poisonUntil) states.Add("POISON");
                if (Time.time < _burnUntil) states.Add("BURNING");
                if (Time.time < _shockUntil) states.Add("SHOCKED");
                if (Time.time < _chillUntil) states.Add("CHILLED");
                if (Time.time < _bleedUntil) states.Add("BLEED ×" + Mathf.Max(1, _bleedStacks));
                if (Time.time < _curseUntil) states.Add("CURSED");
                if (Time.time < _weakenUntil) states.Add("WEAKENED");
                if (Time.time < _vulnerabilityUntil) states.Add("VULNERABLE");
                if (Time.time < _frozenUntil) states.Add("FROZEN");
                if (Time.time < _stunnedUntil) states.Add("STAGGERED");
                if (_armor > 0f) states.Add("ARMOR");
                if (_shield > 0f) states.Add("SHIELD");
                if (_sameElementHits >= (IsBoss ? 3 : 4)) states.Add("RESISTANT-" + _adaptedElement.ToString().ToUpperInvariant());
                if (_affixes.Count > 0) states.AddRange(_affixes.Select(affix => affix.ToString().ToUpperInvariant()));
                string elementalSummary =
    ElementalReactionRuntime.GetStatusSummary(this);

if (!string.IsNullOrEmpty(elementalSummary))
    states.Add(elementalSummary);

return string.Join(" · ", states);
            }
        }

        private EnemyArchetype _archetype;
        private readonly List<EliteAffix> _affixes = new List<EliteAffix>();
        private float _speed;
        private float _damage;
        private float _attackCooldown;
        private float _specialCooldown;
        private float _frozenUntil;
        private float _poisonUntil;
        private float _poisonDamage;
        private float _poisonTick;
        private float _burnUntil;
        private float _burnDamage;
        private float _burnTick;
        private float _shockUntil;
        private float _shockMagnitude;
        private float _chillUntil;
        private float _chillMagnitude;
        private float _chillBuildup;
        private float _bleedUntil;
        private float _bleedDamage;
        private float _bleedTick;
        private int _bleedStacks;
        private CompiledSpell _bleedSource;
        private CastRequest _bleedContext;
        private float _curseUntil;
        private float _curseMagnitude;
        private float _weakenUntil;
        private float _weakenMagnitude;
        private float _vulnerabilityUntil;
        private float _vulnerabilityMagnitude;
        private float _shield;
        private int _bossPhase = 1;
        private SpellElement _adaptedElement = SpellElement.Arcane;
        private int _sameElementHits;
        private SpellElement _lastElement = SpellElement.Arcane;
        private CompiledSpell _poisonSource;
        private CastRequest _poisonContext;
        private CompiledSpell _burnSource;
        private CastRequest _burnContext;
        private Renderer _renderer;
        private Renderer[] _visualRenderers;
        private Collider _collider;
        private Color _baseColor;
        private DifficultySettings _difficulty;
        private CompiledSpell _phoenixSpell;
        private CastRequest _phoenixRequest;
        private float _trainingRefillAt;
        private float _armor;
        private float _maxArmor;
        private float _stagger;
        private float _stunnedUntil;
        private SpellElement _weakElement;
        private SpellElement _lastDamageElement = SpellElement.Arcane;
        private float _actionStateUntil;
        private float _healthBarLocalHeight = 1.5f;
        private bool _trainingStatusImmune;
        private bool _trainingMobile;
        private float _trainingDamageMultiplier = 1f;
        private Vector3 _trainingOrigin;

        public static EnemyController Spawn(Vector3 position, EnemyArchetype archetype, int depth, DifficultySettings difficulty, bool elite, bool boss, bool recordDiscovery = true)
        {
            DecorationExclusion.Reserve(position, boss ? 2.8f : elite ? 1.7f : 1.25f, boss ? "boss spawn" : elite ? "elite spawn" : "enemy spawn");
            Color color = ColorFor(archetype);
            bool bossArchetype = IsBossArchetype(archetype);
            Vector3 scale = archetype == EnemyArchetype.Bulwark || archetype == EnemyArchetype.Warden ? Vector3.one * 1.35f :
                boss || bossArchetype ? Vector3.one * 2.3f : Vector3.one;
            PrimitiveType primitive = archetype == EnemyArchetype.Hexer || archetype == EnemyArchetype.Warden || bossArchetype
                ? PrimitiveType.Cylinder : archetype == EnemyArchetype.Mirror ? PrimitiveType.Cube : PrimitiveType.Capsule;
            GameObject go = RuntimeVisuals.Primitive((elite ? "Elite " : "") + archetype, primitive, position, scale, color);
            RuntimeEntityToken entityToken = go.AddComponent<RuntimeEntityToken>();
            if (!entityToken.Acquire(RuntimeEntityKind.Enemy)) return null;
            EnemyController enemy = go.AddComponent<EnemyController>();
            enemy._archetype = archetype;
            enemy._difficulty = difficulty;
            enemy.IsEliteOrBoss = elite || boss;
            enemy._baseColor = color;
            enemy._renderer = go.GetComponent<Renderer>();
            enemy._visualRenderers = go.GetComponentsInChildren<Renderer>();
            enemy._collider = go.GetComponent<Collider>();
            float baseHealth = BaseHealth(archetype);
            float baseSpeed = BaseSpeed(archetype);
            enemy.MaxHealth = baseHealth * BalanceTuning.EnemyHealthScale(depth, elite, boss) * (difficulty.bulwarkEnemies ? 1.55f : 1f);
            enemy.Health = enemy.MaxHealth;
            enemy._armor = (archetype == EnemyArchetype.Bulwark || archetype == EnemyArchetype.Warden ? 28f : elite ? 12f : 0f);
            enemy._maxArmor = enemy._armor;
            enemy._weakElement = (SpellElement)(Mathf.Abs(depth + (int)archetype) % System.Enum.GetValues(typeof(SpellElement)).Length);
            enemy._speed = baseSpeed * (difficulty.frenziedEnemies ? 1.35f : 1f);
            enemy._damage = BaseDamage(archetype) * BalanceTuning.EnemyDamageScale(depth, elite);
            if (archetype == EnemyArchetype.Warden) enemy._shield = enemy.MaxHealth * 0.25f;
            if (elite) enemy.AssignAffixes(V1Determinism.Combine(depth, (int)archetype, archetype.ToString(), Mathf.RoundToInt(position.x * 10f + position.z * 31f)), difficulty.extraEliteAffixes ? 2 : 1);
            ProceduralEnemyVisual.Attach(enemy);
            GameWorld.Instance.Enemies.Add(enemy);
            if (boss || bossArchetype)
                go.AddComponent<BossEncounterMechanics>().Initialize(enemy, difficulty);
            if (recordDiscovery) ProfileManager.Discover("enemy:" + archetype);
            enemy.BrainState = EnemyBrainState.Repositioning;
            return enemy;
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {
            if (IsDead || GameWorld.Instance == null || !GameWorld.Instance.RunActive || GameWorld.Instance.ModalOpen) return;
            PlayerController player = GameWorld.Instance.Player;
            if (player == null) return;

            TickStatuses();
            if (_archetype == EnemyArchetype.TrainingDummy)
            {
                if (_trainingRefillAt > 0f && Time.time >= _trainingRefillAt) { Health = MaxHealth; _trainingRefillAt = 0f; }
                if (_trainingMobile)
                {
                    float angle = Time.time * 0.7f + Mathf.Abs(GetHashCode() % 7);
                    transform.position = _trainingOrigin + new Vector3(Mathf.Cos(angle) * 2.5f, 0f, Mathf.Sin(angle) * 2.5f);
                }
                return;
            }
            if (Time.time < _frozenUntil || Time.time < _stunnedUntil) { BrainState = EnemyBrainState.Staggered; return; }
            if (Time.time < _actionStateUntil) return;
            BrainState = EnemyBrainState.Repositioning;

            _attackCooldown -= Time.deltaTime;
            _specialCooldown -= Time.deltaTime;
            Vector3 delta = player.transform.position - transform.position;
            delta.y = 0f;
            float distance = delta.magnitude;

            if (_affixes.Contains(EliteAffix.Summoner) && _specialCooldown <= 0f && GameWorld.Instance.Enemies.Count < 18)
            {
                _specialCooldown = 7f;
                EnemyVisualEvents.EliteEvent(this, EliteAffix.Summoner);
                EnemyController minion = Spawn(transform.position + new Vector3(Random.Range(-2.5f, 2.5f), 0f, Random.Range(-2.5f, 2.5f)),
                    EnemyArchetype.Crawler, 3, _difficulty, false, false);
                EnemyVisualEvents.SummonRelationship(this, minion);
            }

            if (IsBoss) UpdateBoss(player, delta, distance);
            else if (_archetype == EnemyArchetype.Hexer || _archetype == EnemyArchetype.Mirror || _archetype == EnemyArchetype.Controller) UpdateRanged(player, delta, distance);
            else if (_archetype == EnemyArchetype.Assassin) UpdateAssassin(player, delta, distance);
            else if (_archetype == EnemyArchetype.Charger) UpdateCharger(player, delta, distance);
            else if (_archetype == EnemyArchetype.Leech) UpdateLeech(player, delta, distance);
            else UpdateMelee(player, delta, distance);
        }

        public bool TakeDamage(float amount, Color flashColor, SpellElement element = SpellElement.Arcane, bool critical = false)
        {
            if (IsDead) return false;
            _lastDamageElement = element;
            if (_archetype == EnemyArchetype.TrainingDummy)
            {
                GameWorld.Instance.Log("Training hit: " + Mathf.RoundToInt(amount) + " " + element + " damage.");
                float trainingDamage = Mathf.Max(0f, amount) * _trainingDamageMultiplier;
                Health = Mathf.Max(1f, Health - trainingDamage);
                _trainingRefillAt = Time.time + 1.25f;
                ModernCombatHUD.ShowDamage(transform.position, trainingDamage, flashColor, critical);
                Flash(flashColor);
                EnemyVisualEvents.Hit(this, element, flashColor, critical);
                return false;
            }

            float final = Mathf.Max(0f, amount);
            if (Time.time < _shockUntil) final *= 1f + _shockMagnitude;
            if (Time.time < _curseUntil) final *= 1f + _curseMagnitude;
            if (Time.time < _vulnerabilityUntil) final *= 1f + _vulnerabilityMagnitude;
            BossEncounterMechanics bossMechanics = IsBoss ? GetComponent<BossEncounterMechanics>() : null;
            if (bossMechanics != null) final = bossMechanics.ModifyIncomingDamage(final, element);
            if (element == _weakElement) final *= 1.25f;
            if (_armor > 0f)
            {
                float armorBefore = _armor;
                float absorbed = Mathf.Min(_armor, final * 0.22f);
                _armor -= absorbed;
                final -= absorbed;
                EnemyVisualEvents.ArmorHit(this, absorbed, armorBefore > 0f && _armor <= 0f);
            }
            if (_affixes.Contains(EliteAffix.Resistant)) { final *= 0.78f; EnemyVisualEvents.EliteEvent(this, EliteAffix.Resistant); }
            if (_difficulty != null && _difficulty.adaptiveEnemies)
            {
                if (element == _lastElement) _sameElementHits++; else { _lastElement = element; _sameElementHits = 1; }
                if (_sameElementHits >= 4) _adaptedElement = element;
                if (element == _adaptedElement && _sameElementHits >= 4)
                {
                    final *= 0.62f;
                    EnemyVisualEvents.ResistanceHit(this, element, _sameElementHits == 4);
                }
            }
            if (_shield > 0f)
            {
                float shieldBefore = _shield;
                float absorbed = Mathf.Min(_shield, final);
                _shield -= absorbed;
                final -= absorbed;
                EnemyVisualEvents.ShieldHit(this, absorbed, shieldBefore > 0f && _shield <= 0f);
                if (_affixes.Contains(EliteAffix.Shielded)) EnemyVisualEvents.EliteEvent(this, EliteAffix.Shielded);
            }
            if (amount > 0.01f && final <= 0.01f) EnemyVisualEvents.ZeroDamage(this, element);
            float before = Health;
            Health -= final;
ElementalReactionRuntime.RegisterDirectHit(
    this,
    element,
    final,
    critical);
            _stagger += final;
            if (_stagger >= MaxHealth * 0.22f)
            {
                _stagger = 0f;
                _stunnedUntil = Time.time + (IsBoss ? 0.25f : 0.65f);
                GameFeelSystem.Burst(transform.position + Vector3.up * 0.7f, Color.white, 0.7f);
            }
            ModernCombatHUD.ShowDamage(transform.position, final, flashColor, critical);
            Flash(flashColor);
            EnemyVisualEvents.Hit(this, element, flashColor, critical);
            if (Health <= 0f)
            {
                Die(before - final);
                return true;
            }
            return false;
        }

        public void ReceiveDamage(DamageRequest request)
        {
            TakeDamage(request.amount, ColorForElement(request.element), request.element, request.critical);
        }

        public void ReceiveStatus(SpellElement element, float strength, float duration, string sourceId)
        {
            if (element == SpellElement.Frost) ApplyFreeze(duration * Mathf.Max(0.2f, strength));
            else if (element == SpellElement.Toxic) ApplyPoison(Mathf.Max(0f, strength), duration, null, new CastRequest());
        }

        public void ApplyImpact(Vector3 direction, float force)
        {

ElementalReactionRuntime.ApplyBuildup(
    this,
    ReactionElement.Physical,
    Mathf.Clamp(force / 18f, 0.25f, 2.5f),
    4f,
    false);

            if (IsDead || IsBoss) return;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) return;
            ProceduralEnemyVisual visual = GetComponent<ProceduralEnemyVisual>();
            if (visual != null) visual.ReactToImpact(direction.normalized, force);
            transform.position += direction.normalized * Mathf.Clamp(force / Mathf.Max(20f, MaxHealth), 0.08f, 0.65f);
        }

        public bool HasStatus
        {
            get { return Time.time < _poisonUntil || Time.time < _burnUntil || Time.time < _shockUntil || Time.time < _chillUntil ||
                Time.time < _frozenUntil || Time.time < _bleedUntil || Time.time < _curseUntil || Time.time < _weakenUntil ||
                Time.time < _vulnerabilityUntil; }
        }

        public float ConsumeStatus()
        {
            float bonus = 0f;
            if (Time.time < _poisonUntil) bonus += _poisonDamage * Mathf.Max(0f, _poisonUntil - Time.time);
            if (Time.time < _burnUntil) bonus += _burnDamage * Mathf.Max(0f, _burnUntil - Time.time);
            if (Time.time < _frozenUntil) bonus += MaxHealth * 0.08f;
            if (Time.time < _shockUntil) bonus += MaxHealth * 0.04f;
            if (Time.time < _bleedUntil) bonus += _bleedDamage * Mathf.Max(1, _bleedStacks) * Mathf.Max(0f, _bleedUntil - Time.time);
            if (Time.time < _curseUntil || Time.time < _vulnerabilityUntil) bonus += MaxHealth * 0.03f;
            _poisonUntil = _burnUntil = _shockUntil = _chillUntil = _frozenUntil = _bleedUntil = _curseUntil = _weakenUntil = _vulnerabilityUntil = 0f;
            _poisonDamage = _burnDamage = _shockMagnitude = _chillMagnitude = _chillBuildup = _bleedDamage = _curseMagnitude = _weakenMagnitude = _vulnerabilityMagnitude = 0f;
            _bleedStacks = 0;
            return bonus;
        }

        public void ApplyPoison(float damagePerSecond, float duration, CompiledSpell source, CastRequest context)
        {
            if (_trainingStatusImmune) return;

ElementalReactionRuntime.ApplyBuildup(
    this,
    ReactionElement.Toxic,
    Mathf.Clamp(
        1f +
        damagePerSecond /
        Mathf.Max(1f, MaxHealth) *
        35f,
        0.5f,
        3.5f),
    duration,
    false);

            _poisonDamage = Mathf.Max(_poisonDamage, damagePerSecond * context.powerScale);
            _poisonUntil = Mathf.Max(_poisonUntil, Time.time + duration);
            _poisonTick = Mathf.Min(_poisonTick, 0.25f);
            _poisonSource = source;
            _poisonContext = context;
        }

        public void ApplyFreeze(float seconds)
        {
            if (_trainingStatusImmune) return;
            _frozenUntil = Mathf.Max(_frozenUntil, Time.time + seconds);
            if (_renderer != null) _renderer.sharedMaterial = RuntimeVisuals.Material(new Color(0.25f, 0.85f, 1f), 0.55f);
        }

        public void ApplyBurn(float damagePerSecond, float duration, CompiledSpell source, CastRequest context)
        {
            if (_trainingStatusImmune) return;

ElementalReactionRuntime.ApplyBuildup(
    this,
    ReactionElement.Fire,
    Mathf.Clamp(
        1f +
        damagePerSecond /
        Mathf.Max(1f, MaxHealth) *
        35f,
        0.5f,
        3.5f),
    duration,
    false);

            _burnDamage = Mathf.Max(_burnDamage, damagePerSecond * context.powerScale);
            _burnUntil = Mathf.Max(_burnUntil, Time.time + duration);
            _burnTick = Mathf.Min(_burnTick, 0.25f);
            _burnSource = source;
            _burnContext = context;
        }

        public void ApplyShock(float magnitude, float duration)
        {
            if (_trainingStatusImmune) return;

ElementalReactionRuntime.ApplyBuildup(
    this,
    ReactionElement.Lightning,
    Mathf.Clamp(magnitude * 5f, 0.5f, 3f),
    duration,
    false);

            _shockMagnitude = Mathf.Max(_shockMagnitude, Mathf.Clamp(magnitude, 0.05f, 0.3f));
            _shockUntil = Mathf.Max(_shockUntil, Time.time + duration);
        }

        public void ApplyChill(float magnitude, float duration)
        {
            if (_trainingStatusImmune) return;

ElementalReactionRuntime.ApplyBuildup(
    this,
    ReactionElement.Cold,
    Mathf.Clamp(magnitude * 4.5f, 0.5f, 3f),
    duration,
    false);

            if (Time.time >= _chillUntil) _chillBuildup = 0f;
            float applied = Mathf.Clamp(magnitude, 0.1f, 0.6f);
            _chillMagnitude = Mathf.Max(_chillMagnitude, applied);
            _chillBuildup += applied;
            _chillUntil = Mathf.Max(_chillUntil, Time.time + duration);
            if (_chillBuildup >= 1f)
            {
                _chillBuildup = 0f;
                ApplyFreeze(IsBoss ? 0.55f : 1.15f);
            }
        }

        public void ApplyBleed(float damagePerSecond, float duration, CompiledSpell source, CastRequest context)
        {
            if (_trainingStatusImmune) return;

ElementalReactionRuntime.ApplyBuildup(
    this,
    ReactionElement.Blood,
    1f,
    duration,
    false);

            _bleedStacks = Mathf.Clamp(Time.time < _bleedUntil ? _bleedStacks + 1 : 1, 1, 5);
            _bleedDamage = Mathf.Max(_bleedDamage, Mathf.Max(0f, damagePerSecond) * Mathf.Max(0.01f, context.powerScale));
            _bleedUntil = Mathf.Max(_bleedUntil, Time.time + Mathf.Max(0.25f, duration));
            _bleedTick = Mathf.Min(_bleedTick, 0.25f);
            _bleedSource = source;
            _bleedContext = context;
        }

        public void ApplyCurse(float magnitude, float duration)
        {
            if (_trainingStatusImmune) return;

ElementalReactionRuntime.ApplyBuildup(
    this,
    ReactionElement.Void,
    Mathf.Clamp(magnitude * 4f, 0.5f, 3f),
    duration,
    false);

            _curseMagnitude = Mathf.Max(_curseMagnitude, Mathf.Clamp(magnitude, 0.05f, 0.5f));
            _curseUntil = Mathf.Max(_curseUntil, Time.time + Mathf.Max(0.25f, duration));
        }

        public void ApplyWeaken(float magnitude, float duration)
        {
            if (_trainingStatusImmune) return;
            _weakenMagnitude = Mathf.Max(_weakenMagnitude, Mathf.Clamp(magnitude, 0.05f, 0.5f));
            _weakenUntil = Mathf.Max(_weakenUntil, Time.time + Mathf.Max(0.25f, duration));
        }

        public void ApplyVulnerability(float magnitude, float duration)
        {
            if (_trainingStatusImmune) return;
            _vulnerabilityMagnitude = Mathf.Max(_vulnerabilityMagnitude, Mathf.Clamp(magnitude, 0.05f, 0.5f));
            _vulnerabilityUntil = Mathf.Max(_vulnerabilityUntil, Time.time + Mathf.Max(0.25f, duration));
        }

        public void EmbedPhoenix(CompiledSpell spell, CastRequest request)
        {
            _phoenixSpell = spell;
            _phoenixRequest = request;
            GameWorld.Instance.Log("Phoenix Seed embeds in " + _archetype + ".");
        }

        public void PullToward(Vector3 position, float strength)
        {
            if (IsDead || Time.time < _frozenUntil || IsBoss) return;
            Vector3 delta = position - transform.position;
            delta.y = 0f;
            if (delta.sqrMagnitude > 0.1f) transform.position += delta.normalized * strength * Time.deltaTime;
        }

        public void ConfigureTrainingTarget(V21TrainingTargetProfile profile)
        {
            if (_archetype != EnemyArchetype.TrainingDummy) return;
            _trainingOrigin = transform.position;
            _trainingMobile = profile == V21TrainingTargetProfile.Mobile;
            _trainingStatusImmune = profile == V21TrainingTargetProfile.StatusImmune;
            _trainingDamageMultiplier = profile == V21TrainingTargetProfile.Resistant ? 0.5f :
                profile == V21TrainingTargetProfile.Vulnerable ? 1.5f : 1f;
            MaxHealth = profile == V21TrainingTargetProfile.BossSized ? 25000f : 10000f;
            Health = MaxHealth;
            _maxArmor = _armor = profile == V21TrainingTargetProfile.Armored ? 2000f : 0f;
            _shield = profile == V21TrainingTargetProfile.Shielded ? MaxHealth * 0.5f : 0f;
            transform.localScale = profile == V21TrainingTargetProfile.BossSized ? Vector3.one * 2.2f : Vector3.one;
            // RefreshHealthBarAnchor removed - method not defined
        }

        public void Heal(float amount)
        {
            Health = Mathf.Min(MaxHealth, Health + amount);
            Flash(new Color(0.25f, 1f, 0.45f));
            if (_affixes.Contains(EliteAffix.Vampiric)) EnemyVisualEvents.EliteEvent(this, EliteAffix.Vampiric);
        }

        private void UpdateMelee(PlayerController player, Vector3 delta, float distance)
        {
            if (distance > 1.4f)
            {
                MoveToward(delta, _archetype == EnemyArchetype.Warden && distance < 4f ? 0.4f : 1f);
                return;
            }
            if (_attackCooldown > 0f) return;
            if (AttackCoordinator.Instance != null && !AttackCoordinator.Instance.TryReserve(this, CombatRole, 1.1f)) return;
            _attackCooldown = _archetype == EnemyArchetype.Bulwark ? 1.5f : 0.95f;
            TelegraphMelee(player, _archetype == EnemyArchetype.Bulwark ? 0.45f : 0.25f);
        }

        private void UpdateRanged(PlayerController player, Vector3 delta, float distance)
        {
            if (_archetype == EnemyArchetype.Mirror && _specialCooldown <= 0f)
            {
                int intercepted = SpellProjectile.InterceptNear(transform.position, 2.8f, player.transform.position, _damage * 0.65f);
                if (intercepted > 0)
                {
                    _specialCooldown = 3.2f;
                    EnemyVisualEvents.Telegraph(this, "Projectile Reflection", delta, 2.8f, 0.16f);
                    return;
                }
            }
            if (_archetype == EnemyArchetype.Hexer && _specialCooldown <= 0f && distance < 11f)
            {
                _specialCooldown = 5.5f;
                HoldBrainState(EnemyBrainState.Telegraphing, 0.75f);
                EnemyVisualEvents.Telegraph(this, "Spell Suppression", delta, 1.1f, 0.65f);
                StartCoroutine(DelayedSpellSuppression(player, 0.65f));
                return;
            }
            if (_archetype == EnemyArchetype.Controller && _specialCooldown <= 0f)
            {
                _specialCooldown = 4.5f;
                Vector3 target = player.transform.position;
                HoldBrainState(EnemyBrainState.Telegraphing, 0.67f);
                EnemyVisualEvents.Telegraph(this, "Control Zone", target - transform.position, 2.1f, 0.55f);
                StartCoroutine(DelayedCorruptionHazard(player, target, 2.1f, _damage * 0.65f, new Color(0.1f, 0.9f, 0.75f), 0.55f));
                return;
            }
            if (distance < 5f) MoveToward(-delta, 0.65f);
            else if (distance > 10f) MoveToward(delta, 0.65f);
            if (_attackCooldown > 0f) return;
            if (AttackCoordinator.Instance != null && !AttackCoordinator.Instance.TryReserve(this, CombatRole, 0.8f)) return;
            float castDelay = _archetype == EnemyArchetype.Mirror ? 0.28f : 0.34f;
            HoldBrainState(EnemyBrainState.Telegraphing, castDelay + 0.12f);
            EnemyVisualEvents.Telegraph(this, "Ranged Cast", delta, 8f, castDelay);
            _attackCooldown = _archetype == EnemyArchetype.Mirror ? 1.4f : 1.8f;
            int volley = _archetype == EnemyArchetype.Mirror ? 3 : 1;
            StartCoroutine(DelayedRanged(delta.normalized, volley, castDelay));
        }

        private void UpdateCharger(PlayerController player, Vector3 delta, float distance)
        {
            if (_specialCooldown <= 0f && distance > 3f && distance < 12f)
            {
                if (AttackCoordinator.Instance != null && !AttackCoordinator.Instance.TryReserve(this, CombatRole, 1.2f)) { UpdateMelee(player, delta, distance); return; }
                HoldBrainState(EnemyBrainState.Telegraphing, 0.44f);
                _specialCooldown = 3.5f;
                EnemyVisualEvents.Telegraph(this, "Charge", delta, 9f, 0.32f);
                StartCoroutine(DelayedCharge(player, delta.normalized, Mathf.Min(8f, distance - 0.5f), 0.32f));
                return;
            }
            UpdateMelee(player, delta, distance);
        }

        private void UpdateLeech(PlayerController player, Vector3 delta, float distance)
        {
            if (_specialCooldown <= 0f)
            {
                _specialCooldown = 4f;
                EnemyController wounded = GameWorld.Instance.Enemies.Where(e => e != null && e != this && !e.IsDead).OrderBy(e => e.Health / e.MaxHealth).FirstOrDefault();
                if (wounded != null && wounded.Health < wounded.MaxHealth)
                {
                    RuntimeVisuals.Beam(transform.position, wounded.transform.position, new Color(0.25f, 1f, 0.4f), 0.4f);
                    wounded.Heal(wounded.MaxHealth * 0.12f);
                }
            }
            UpdateMelee(player, delta, distance);
        }

        private void UpdateAssassin(PlayerController player, Vector3 delta, float distance)
        {
            if (_specialCooldown <= 0f && distance > 2.5f)
            {
                if (AttackCoordinator.Instance != null && !AttackCoordinator.Instance.TryReserve(this, CombatRole, 1.1f)) { UpdateMelee(player, delta, distance); return; }
                HoldBrainState(EnemyBrainState.Telegraphing, 0.5f);
                _specialCooldown = 4.2f;
                Vector3 behind = player.transform.position - player.transform.forward * 2.2f;
                RuntimeVisuals.Beam(transform.position, behind, new Color(0.75f, 0.15f, 1f), 0.18f);
                transform.position = DungeonObstacle.Resolve(behind, HitRadius * 0.55f);
                TelegraphMelee(player, 0.38f);
                return;
            }
            UpdateMelee(player, delta, distance);
        }

        private void UpdateBoss(PlayerController player, Vector3 delta, float distance)
        {
            // Biome bosses own their phase mechanics in BossEncounterMechanics.
            // The legacy Warden keeps this close-range supplemental pattern.
            if (_archetype != EnemyArchetype.OssuaryWarden)
            {
                UpdateMelee(player, delta, distance);
                return;
            }
            if (_specialCooldown <= 0f)
            {
                if (AttackCoordinator.Instance != null && !AttackCoordinator.Instance.TryReserve(this, "BOSS", 1.1f)) { UpdateMelee(player, delta, distance); return; }
                HoldBrainState(EnemyBrainState.Telegraphing, _bossPhase == 1 ? 0.74f : 0.84f);
                _specialCooldown = Mathf.Max(1.5f, 3.2f - _bossPhase * 0.45f);
                if (_bossPhase == 1)
                {
                    EnemyVisualEvents.Telegraph(this, "Warden Radial Volley", Vector3.zero, 5.5f, 0.62f);
                    StartCoroutine(DelayedBossVolley(0.62f));
                }
                else
                {
                    Vector3 target = player.transform.position;
                    float radius = 2.4f + _bossPhase * 0.3f;
                    EnemyVisualEvents.Telegraph(this, "Warden Sanctum Hazard", target - transform.position, radius, 0.72f);
                    StartCoroutine(DelayedBossHazard(target, radius, 0.72f));
                }
                return;
            }
            UpdateMelee(player, delta, distance);
        }

        private void BeginBossPhase()
        {
            _shield += MaxHealth * 0.12f;
            _speed *= 1.12f;
            EnemyVisualEvents.BossPhase(this, _bossPhase);
            GameWorld.Instance.Log(DisplayName.ToUpperInvariant() + " — PHASE " + _bossPhase + ". New mechanics awaken.");
            Camera camera = Camera.main;
            if (camera != null)
            {
                IsometricCamera rig = camera.GetComponent<IsometricCamera>();
                if (rig != null) rig.Shake(0.8f, transform.position, 5, 0.42f);
            }
        }

        public void SetBossPhase(int phase)
        {
            if (!IsBoss) return;
            int next = Mathf.Clamp(phase, 1, 3);
            if (next <= _bossPhase) return;
            _bossPhase = next;
            BeginBossPhase();
        }

        public void SetBossAdaptation(SpellElement element, int repeatedHits)
        {
            if (!IsBoss) return;
            _adaptedElement = element;
            _sameElementHits = Mathf.Max(0, repeatedHits);
            if (_sameElementHits >= 3) EnemyVisualEvents.ResistanceHit(this, element, _sameElementHits == 3);
        }

        private void TelegraphMelee(PlayerController player, float delay)
        {
            HoldBrainState(EnemyBrainState.Telegraphing, delay + 0.12f);
            AdaptiveAudioSystem.PlayEnemyCue("melee", _archetype == EnemyArchetype.Bulwark || IsBoss);
            EnemyVisualEvents.Telegraph(this, "Melee", player == null ? transform.forward : player.transform.position - transform.position, 1.65f, delay);
            StartCoroutine(DelayedMelee(player, delay));
        }

        private System.Collections.IEnumerator DelayedMelee(PlayerController player, float delay)
        {
            yield return new WaitForSeconds(delay);
            HoldBrainState(EnemyBrainState.Attacking, 0.1f);
            if (!IsDead && player != null && (player.transform.position - transform.position).sqrMagnitude < 3.3f) DamagePlayer(player, _damage);
            HoldBrainState(EnemyBrainState.Recovering, 0.16f);
        }

        private System.Collections.IEnumerator DelayedRanged(Vector3 direction, int volley, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (IsDead) yield break;
            HoldBrainState(EnemyBrainState.Attacking, 0.14f);
            for (int i = 0; i < volley; i++)
            {
                Vector3 shotDirection = Quaternion.Euler(0f, volley == 1 ? 0f : -12f + i * 12f, 0f) * direction;
                EnemyBolt.Create(transform.position + Vector3.up * 0.55f, shotDirection, _damage, _baseColor);
            }
            yield return new WaitForSeconds(0.1f);
            if (!IsDead) HoldBrainState(EnemyBrainState.Recovering, 0.16f);
        }

        private System.Collections.IEnumerator DelayedCharge(PlayerController player, Vector3 direction, float distance, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (IsDead) yield break;
            HoldBrainState(EnemyBrainState.Attacking, 0.12f);
            transform.position = DungeonObstacle.Resolve(transform.position + direction * distance, HitRadius * 0.55f);
            if (player != null && (player.transform.position - transform.position).sqrMagnitude < 3f) DamagePlayer(player, _damage * 1.4f);
            HoldBrainState(EnemyBrainState.Recovering, 0.18f);
        }

        private System.Collections.IEnumerator DelayedHazard(Vector3 position, float radius, float damage, Color color, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!IsDead) PersistentEnemyHazard.Create(position, radius, damage, color, 0f);
        }

        private System.Collections.IEnumerator DelayedSpellSuppression(PlayerController player, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (IsDead || player == null || (player.transform.position - transform.position).sqrMagnitude > 132f) yield break;
            int slot = Mathf.Abs(GetEntityId().GetHashCode() + Mathf.FloorToInt(Time.time)) % 3;
            player.ApplyTemporaryRuneSuppression(slot, 3.5f, DisplayName);
            RuntimeVisuals.Beam(transform.position + Vector3.up * 0.7f, player.transform.position + Vector3.up * 0.6f, _baseColor, 0.3f);
        }

        private System.Collections.IEnumerator DelayedCorruptionHazard(PlayerController player, Vector3 position, float radius, float damage, Color color, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (IsDead) yield break;
            PersistentEnemyHazard.Create(position, radius, damage, color, 0f);
            if (player != null && CombatMath.PlanarDistanceSquared(player.transform.position, position) <= radius * radius)
                player.ApplyTemporaryModifierCorruption(4f, DisplayName);
        }

        private System.Collections.IEnumerator DelayedBossVolley(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (IsDead) yield break;
            HoldBrainState(EnemyBrainState.Attacking, 0.14f);
            for (int i = 0; i < 6; i++) EnemyBolt.Create(transform.position + Vector3.up, Quaternion.Euler(0f, i * 60f, 0f) * Vector3.forward, _damage * 0.75f, _baseColor);
            HoldBrainState(EnemyBrainState.Recovering, 0.18f);
        }

        private System.Collections.IEnumerator DelayedBossHazard(Vector3 position, float radius, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (IsDead) yield break;
            HoldBrainState(EnemyBrainState.Attacking, 0.18f);
            PersistentEnemyHazard.Create(position, radius, _damage * 0.45f, _baseColor, 0f);
            if (_bossPhase >= 3) for (int i = 0; i < 2; i++)
                Spawn(transform.position + new Vector3(i == 0 ? -3f : 3f, 0f, 2f), EnemyArchetype.Crawler, 7, _difficulty, true, false);
            HoldBrainState(EnemyBrainState.Recovering, 0.22f);
        }

        private void HoldBrainState(EnemyBrainState state, float duration)
        {
            BrainState = state;
            _actionStateUntil = Mathf.Max(_actionStateUntil, Time.time + Mathf.Max(0.02f, duration));
        }

        private void DamagePlayer(PlayerController player, float raw)
        {
            float scale = _difficulty != null && _difficulty.glassSoul ? 1.5f : 1f;
            if (Time.time < _weakenUntil) scale *= Mathf.Max(0.25f, 1f - _weakenMagnitude);
            bool landed = !player.IsInvulnerable;
            player.TakeDamage(raw * scale, DisplayName + " " + CombatRole.ToLowerInvariant() + " attack");
            if (landed)
            {
                if (_archetype == EnemyArchetype.Charger) player.ApplyCondition(PlayerCondition.Root, 0.38f, 1f, DisplayName);
                else if (_archetype == EnemyArchetype.Assassin) player.ApplyCondition(PlayerCondition.Stun, 0.24f, 1f, DisplayName);
                else if (_archetype == EnemyArchetype.Controller) player.ApplyCondition(PlayerCondition.Slow, 1.4f, 0.45f, DisplayName);
                else if (_archetype == EnemyArchetype.Hexer) player.ApplyCondition(PlayerCondition.Silence, 0.5f, 1f, DisplayName);
            }
            if (_affixes.Contains(EliteAffix.Vampiric)) Heal(raw * 0.7f);
        }

        private void MoveToward(Vector3 delta, float multiplier)
        {
            if (delta.sqrMagnitude < 0.01f) return;
            if (_affixes.Contains(EliteAffix.Frenzied)) EnemyVisualEvents.EliteEvent(this, EliteAffix.Frenzied);
            float statusSpeed = Time.time < _chillUntil ? Mathf.Max(0.35f, 1f - _chillMagnitude) : 1f;
            float currentSpeed = _speed * statusSpeed;
            Vector3 direction = DungeonNavigation.Steer(this, delta, Mathf.Max(0.45f, currentSpeed * multiplier * Time.deltaTime * 4f));
            if (direction.sqrMagnitude < 0.01f) direction = delta.normalized;
            transform.position = DungeonObstacle.Resolve(transform.position + direction * currentSpeed * multiplier * Time.deltaTime, HitRadius * 0.55f);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 1f - Mathf.Exp(-8f * Time.deltaTime));
        }

        private void TickStatuses()
        {
            if (Time.time >= _chillUntil) { _chillMagnitude = 0f; _chillBuildup = 0f; }
            if (Time.time < _poisonUntil)
            {
                _poisonTick -= Time.deltaTime;
                if (_poisonTick <= 0f)
                {
                    _poisonTick += 1f;
                    bool killed = TakeDamage(_poisonDamage, _poisonSource == null ? Color.green : _poisonSource.primaryColor,
                        _poisonSource == null ? SpellElement.Toxic : _poisonSource.element);
                    if (killed && _poisonSource != null) SpellExecutor.NotifyKill(_poisonSource, _poisonContext, transform.position);
                }
            }
            if (!IsDead && Time.time < _burnUntil)
            {
                _burnTick -= Time.deltaTime;
                if (_burnTick <= 0f)
                {
                    _burnTick += 1f;
                    bool killed = TakeDamage(_burnDamage, _burnSource == null ? new Color(1f, 0.25f, 0.05f) : _burnSource.primaryColor, SpellElement.Fire);
                    if (killed && _burnSource != null) SpellExecutor.NotifyKill(_burnSource, _burnContext, transform.position);
                }
            }
            if (!IsDead && Time.time < _bleedUntil)
            {
                _bleedTick -= Time.deltaTime;
                if (_bleedTick <= 0f)
                {
                    _bleedTick += 1f;
                    float bleed = _bleedDamage * Mathf.Max(1, _bleedStacks);
                    bool killed = TakeDamage(bleed, new Color(0.85f, 0.06f, 0.12f), SpellElement.Arcane);
                    if (killed && _bleedSource != null) SpellExecutor.NotifyKill(_bleedSource, _bleedContext, transform.position);
                }
            }
            if (Time.time >= _bleedUntil) _bleedStacks = 0;
        }

        private void AssignAffixes(int seed, int count)
        {
            EliteAffix[] pool = { EliteAffix.Frenzied, EliteAffix.Shielded, EliteAffix.Volatile, EliteAffix.Vampiric, EliteAffix.Resistant, EliteAffix.Summoner };
            for (int i = 0; i < count; i++)
            {
                EliteAffix affix = pool[Mathf.Abs(seed * 17 + i * 31) % pool.Length];
                if (!_affixes.Contains(affix)) _affixes.Add(affix);
            }
            if (_affixes.Contains(EliteAffix.Frenzied)) _speed *= 1.3f;
            if (_affixes.Contains(EliteAffix.Shielded)) _shield += MaxHealth * 0.35f;
            if (_affixes.Contains(EliteAffix.Summoner)) _specialCooldown = 1f;
        }

        private void Die(float finalDamage)
        {

ElementalReactionRuntime.NotifyDeath(this);

            if (IsDead) return;
            IsDead = true;
            BrainState = EnemyBrainState.Dead;
            if (IsBoss && RunStatistics.Instance != null) RunStatistics.Instance.CompleteBoss();
            if (AttackCoordinator.Instance != null) AttackCoordinator.Instance.Release(this);
            GameWorld.Instance.Enemies.Remove(this);
            if (_affixes.Contains(EliteAffix.Volatile)) { EnemyVisualEvents.EliteEvent(this, EliteAffix.Volatile); PersistentEnemyHazard.Create(transform.position, 2.7f, _damage, new Color(1f, 0.2f, 0.05f), 0.7f); }
            GameWorld.Instance.GetComponent<RunDirector>().OnEnemyKilled(this, transform.position, IsEliteOrBoss);
            if (_phoenixSpell != null) SpellExecutor.PhoenixErupt(_phoenixSpell, _phoenixRequest, transform.position);
            EnemyVisualEvents.Death(this, _lastDamageElement);
            GameObject burst = RuntimeVisuals.Primitive("Enemy Death", PrimitiveType.Sphere, transform.position + Vector3.up * 0.4f, Vector3.one * 0.5f, _baseColor);
            RuntimeVisuals.RemoveCollider(burst);
            Destroy(burst, 0.22f);
            Destroy(gameObject);
        }

        private void Flash(Color color)
        {
            if (_renderer == null || ProfileManager.Current.accessibility.reducedFlashes) return;
            _renderer.sharedMaterial = RuntimeVisuals.Material(color, 0.8f);
            CancelInvoke(nameof(RestoreColor));
            Invoke(nameof(RestoreColor), 0.1f);
        }

        private void RestoreColor()
        {
            if (_renderer != null && !IsDead)
            {
                Color color = Time.time < _burnUntil ? new Color(1f, 0.28f, 0.05f) :
                    Time.time < _poisonUntil ? new Color(0.3f, 0.95f, 0.18f) :
                    Time.time < _shockUntil ? new Color(0.4f, 0.65f, 1f) :
                    Time.time < _chillUntil ? new Color(0.3f, 0.85f, 1f) : _baseColor;
                _renderer.sharedMaterial = RuntimeVisuals.Material(color, 0.3f);
            }
        }

        public void RefreshVisualBounds()
        {
            _visualRenderers = GetComponentsInChildren<Renderer>(true);
            bool initialized = false;
            Bounds bounds = new Bounds(transform.position, Vector3.zero);
            for (int i = 0; i < _visualRenderers.Length; i++)
            {
                Renderer visual = _visualRenderers[i];
                if (visual == null || !visual.enabled || visual is TrailRenderer || visual is LineRenderer || visual.GetComponent<TextMesh>() != null) continue;
                if (!initialized) { bounds = visual.bounds; initialized = true; } else bounds.Encapsulate(visual.bounds);
            }
            if (initialized) _healthBarLocalHeight = Mathf.Clamp(bounds.max.y - transform.position.y + 0.28f, 1.25f, IsBoss ? 5.5f : 3.6f);
        }

        private static Color ColorFor(EnemyArchetype type)
        {
            switch (type)
            {
                case EnemyArchetype.Crawler: return new Color(0.92f, 0.18f, 0.22f);
                case EnemyArchetype.Bulwark: return new Color(0.85f, 0.45f, 0.08f);
                case EnemyArchetype.Hexer: return new Color(0.65f, 0.16f, 0.9f);
                case EnemyArchetype.Charger: return new Color(1f, 0.25f, 0.08f);
                case EnemyArchetype.Warden: return new Color(0.25f, 0.65f, 1f);
                case EnemyArchetype.Leech: return new Color(0.3f, 0.9f, 0.35f);
                case EnemyArchetype.Mirror: return new Color(0.85f, 0.85f, 1f);
                case EnemyArchetype.Assassin: return new Color(0.62f, 0.08f, 0.82f);
                case EnemyArchetype.Controller: return new Color(0.1f, 0.8f, 0.72f);
                case EnemyArchetype.OssuaryWarden: return new Color(0.95f, 0.08f, 0.25f);
                case EnemyArchetype.EmberTitan: return new Color(1f, 0.26f, 0.04f);
                case EnemyArchetype.ArchiveSeraph: return new Color(0.2f, 0.72f, 1f);
                case EnemyArchetype.VenomMatriarch: return new Color(0.32f, 0.95f, 0.08f);
                default: return new Color(0.4f, 0.75f, 0.85f);
            }
        }

        private static Color ColorForElement(SpellElement element)
        {
            if (element == SpellElement.Fire) return new Color(1f, 0.25f, 0.05f);
            if (element == SpellElement.Frost) return new Color(0.25f, 0.85f, 1f);
            if (element == SpellElement.Lightning) return new Color(0.75f, 0.45f, 1f);
            if (element == SpellElement.Toxic) return new Color(0.3f, 1f, 0.25f);
            if (element == SpellElement.Void) return new Color(0.65f, 0.12f, 0.9f);
            if (element == SpellElement.Physical)
    return new Color(0.75f, 0.67f, 0.55f);

if (element == SpellElement.Blood)
    return new Color(0.75f, 0.03f, 0.08f);

return new Color(0.4f, 0.75f, 1f);
        }

        private static float BaseHealth(EnemyArchetype type)
        {
            if (type == EnemyArchetype.TrainingDummy) return 500f;
            if (IsBossArchetype(type)) return type == EnemyArchetype.EmberTitan ? 610f : type == EnemyArchetype.VenomMatriarch ? 560f : 520f;
            if (type == EnemyArchetype.Bulwark || type == EnemyArchetype.Warden) return 110f;
            if (type == EnemyArchetype.Controller) return 78f;
            return type == EnemyArchetype.Crawler ? 42f : 64f;
        }

        private static float BaseSpeed(EnemyArchetype type)
        {
            if (type == EnemyArchetype.Bulwark || type == EnemyArchetype.Warden || IsBossArchetype(type))
                return type == EnemyArchetype.ArchiveSeraph ? 2.25f : 1.7f;
            if (type == EnemyArchetype.Charger) return 2.5f;
            if (type == EnemyArchetype.Assassin) return 3.7f;
            return type == EnemyArchetype.Crawler ? 3.2f : 2.35f;
        }

        private static float BaseDamage(EnemyArchetype type)
        {
            if (IsBossArchetype(type)) return type == EnemyArchetype.EmberTitan ? 25f : 22f;
            if (type == EnemyArchetype.Bulwark || type == EnemyArchetype.Charger) return 18f;
            if (type == EnemyArchetype.Assassin) return 15f;
            return 10f;
        }

        public static bool IsBossArchetype(EnemyArchetype type)
        {
            return type == EnemyArchetype.OssuaryWarden || type == EnemyArchetype.EmberTitan ||
                   type == EnemyArchetype.ArchiveSeraph || type == EnemyArchetype.VenomMatriarch;
        }
    }

    public sealed class EnemyBolt : MonoBehaviour
    {
        private Vector3 _direction;
        private float _damage;
        private float _age;
        private bool _reflected;

        public static EnemyBolt Create(Vector3 origin, Vector3 direction, float damage, Color color)
        {
            GameObject go = RuntimeVisuals.Primitive("Enemy Projectile", PrimitiveType.Sphere, origin, Vector3.one * 0.28f, color);
            RuntimeVisuals.RemoveCollider(go);
            RuntimeEntityToken token = go.AddComponent<RuntimeEntityToken>();
            if (!token.Acquire(RuntimeEntityKind.EnemyProjectile)) return null;
            EnemyBolt bolt = go.AddComponent<EnemyBolt>();
            bolt._direction = direction.normalized;
            bolt._damage = damage;
            return bolt;
        }

        private void Update()
        {
            if (GameWorld.Instance == null || !GameWorld.Instance.RunActive) { Destroy(gameObject); return; }
            _age += Time.deltaTime;
            transform.position += _direction * 7f * Time.deltaTime;
            IReadOnlyList<PersistentSpellZone> zones = PersistentSpellZone.Active;
            for (int i = 0; i < zones.Count; i++)
            {
                PersistentSpellZone zone = zones[i];
                if (zone == null) continue;
                if (!zone.ReflectsProjectiles || (zone.transform.position - transform.position).sqrMagnitude > zone.Radius * zone.Radius) continue;
                if (!_reflected)
                {
                    _reflected = true;
                    EnemyController target = GameWorld.Instance.NearestEnemy(transform.position);
                    if (target != null) _direction = (target.transform.position - transform.position).normalized;
                }
            }
            if (_reflected)
            {
                EnemyController target = GameWorld.Instance.NearestEnemy(transform.position, 1f);
                if (target != null) { target.TakeDamage(_damage, Color.white, SpellElement.Arcane); Destroy(gameObject); return; }
            }
            else if ((GameWorld.Instance.Player.transform.position - transform.position).sqrMagnitude < 0.8f)
            {
                GameWorld.Instance.Player.TakeDamage(_damage, "Enemy projectile");
                Destroy(gameObject);
                return;
            }
            if (_age > 5f || Mathf.Abs(transform.position.x) > 18f || Mathf.Abs(transform.position.z) > 18f) Destroy(gameObject);
        }
    }

    public sealed class PersistentEnemyHazard : MonoBehaviour
    {
        private float _radius;
        private float _damage;
        private float _delay;
        private float _life = 3f;
        private float _tick;
        private LineRenderer _ring;

        public static PersistentEnemyHazard Create(Vector3 position, float radius, float damage, Color color, float delay = 0.3f)
        {
            GameObject go = new GameObject("Enemy Ground Hazard");
            go.transform.position = position;
            RuntimeEntityToken token = go.AddComponent<RuntimeEntityToken>();
            if (!token.Acquire(RuntimeEntityKind.Hazard)) return null;
            PersistentEnemyHazard hazard = go.AddComponent<PersistentEnemyHazard>();
            hazard._radius = radius; hazard._damage = damage; hazard._delay = delay;
            hazard._ring = ProceduralVisualRuntime.Ring("Enemy Hazard Boundary", position, color, radius, 0.14f, 3.2f, go.transform, true);
            VisualCounterRegistration.Attach(go, VisualRuntimeKind.PersistentZone);
            return hazard;
        }

        private void Update()
        {
            if (GameWorld.Instance == null || !GameWorld.Instance.RunActive) { Destroy(gameObject); return; }
            _delay -= Time.deltaTime;
            _life -= Time.deltaTime;
            _tick -= Time.deltaTime;
            if (_delay <= 0f && _tick <= 0f && (GameWorld.Instance.Player.transform.position - transform.position).sqrMagnitude < _radius * _radius)
            {
                _tick = 0.8f;
                GameWorld.Instance.Player.TakeDamage(_damage, "Ground hazard");
            }
            if (_life <= 0f) Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_ring == null || _ring.transform.parent != transform) { _ring = null; return; }
            PooledVisualMarker marker = _ring.GetComponent<PooledVisualMarker>();
            if (marker == null || !marker.InUse) { _ring = null; return; }
            _ring.transform.SetParent(null, true);
            ProceduralVisualRuntime.Release(_ring.gameObject);
            _ring = null;
        }
    }
}
