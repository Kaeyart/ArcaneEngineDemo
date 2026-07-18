using UnityEngine;

namespace ArcaneEngine
{
    [DefaultExecutionOrder(900)]
    public sealed class PlayerController : MonoBehaviour, IDamageable, ITargetable
    {
        public float Health { get; private set; }
        public float Mana { get; private set; }
        public float Ward { get; private set; }
        public Vector3 AimPoint { get; private set; }
        public Vector3 AimDirection { get; private set; } = Vector3.forward;
        public SpellSlot LastCastSlot { get; private set; } = SpellSlot.Slot1;
        public string LastDamageSource { get; private set; } = "Unknown";
        public float DodgeCooldownRemaining { get { return Mathf.Max(0f, _dodgeCooldown); } }
        public float DodgeCooldownRatio { get { return _stats == null ? 0f : Mathf.Clamp01(DodgeCooldownRemaining / Mathf.Max(0.01f, _stats.dodgeCooldown)); } }
        public bool IsInvulnerable { get { return Time.time < _invulnerableUntil; } }
        public bool IsAlive { get { return Health > 0f; } }
        public Vector3 DamagePoint { get { return transform.position + Vector3.up * 0.8f; } }
        public bool CanBeTargeted { get { return IsAlive; } }
        public Vector3 TargetPoint { get { return DamagePoint; } }
        public string ConditionSummary
        {
            get
            {
                if (Time.time < _stunUntil) return "STUNNED";
                if (Time.time < _rootUntil) return "ROOTED";
                if (Time.time < _silenceUntil) return "SILENCED";
                if (Time.time < _runeSuppressionUntil) return "SPELL " + (_suppressedSpellSlot + 1) + " SUPPRESSED";
                if (Time.time < _modifierCorruptionUntil) return "RUNES DISRUPTED";
                if (Time.time < _slowUntil) return "SLOWED";
                return string.Empty;
            }
        }

        private PlayerStats _stats;
        private readonly float[] _cooldowns = new float[3];
        private readonly float[] _chargeTimes = new float[3];
        private float _dodgeCooldown;
        private float _invulnerableUntil;
        private float _wardUntil;
        private float _healingReservoir;
        private float _dodgePowerUntil;
        private bool _firstCastReady;
        private Vector3 _lastMove = Vector3.forward;
        private Vector3 _moveVelocity;
        private HardwareMouseAim _hardwareAim;
        private GameObject _aimMarker;
        private LineRenderer _aimRing;
        private LineRenderer _aimDirection;
        private float _slowUntil;
        private float _rootUntil;
        private float _silenceUntil;
        private float _stunUntil;
        private float _runeSuppressionUntil;
        private int _suppressedSpellSlot = -1;
        private float _modifierCorruptionUntil;
        private float _slowMultiplier = 1f;

        private void OnDestroy()
        {
            if (_aimMarker != null) Destroy(_aimMarker);
        }

        private void Awake()
        {
            _hardwareAim = GetComponent<HardwareMouseAim>();
        }

        private void Update()
{
    GameWorld world = GameWorld.Instance;

    if (world == null || _stats == null)
        return;

    bool runActive = world.RunActive;

    // Combat/run systems should only update during an active run.
    if (runActive)
    {
        for (int i = 0; i < _cooldowns.Length; i++)
            _cooldowns[i] -= Time.deltaTime;

        _dodgeCooldown -= Time.deltaTime;

        if (Time.time > _wardUntil)
            Ward = 0f;

        RunDirector director = world.GetComponent<RunDirector>();

        float manaRegen =
            (director != null && director.Difficulty.manaDrought
                ? 6f
                : 14f)
            + _stats.manaRegen;

        Mana = Mathf.Min(
            _stats.maxMana,
            Mana + manaRegen * Time.deltaTime);

        if (_stats.lifeRegen > 0f && Health > 0f)
        {
            Health = Mathf.Min(
                _stats.maxHealth,
                Health + _stats.lifeRegen * Time.deltaTime);
        }

        if (world.TrainingMode &&
            ArcaneInput.GetKeyDown(KeyCode.Escape))
        {
            world.LeaveTraining();
            return;
        }
    }

    // Do not move underneath menus or modal screens.
    if (world.ModalOpen || RunStartScreen.IsOpen)
    {
        _moveVelocity = Vector3.MoveTowards(
            _moveVelocity,
            Vector3.zero,
            40f * Time.unscaledDeltaTime);

        return;
    }

    // Combat conditions only prevent movement during an active run.
    if (runActive && Time.time < _stunUntil)
    {
        _moveVelocity = Vector3.MoveTowards(
            _moveVelocity,
            Vector3.zero,
            40f * Time.deltaTime);

        return;
    }

    // Movement is allowed both in Home Base and during runs.
    Move();

    // Dodge remains a combat/training action.
    if (runActive &&
        (ArcaneInput.GetKeyDown(
             ProfileManager.Current.controls.dodge) ||
         ArcaneInput.GamepadDodgeDown))
    {
        Dodge();
    }
}
        private void LateUpdate()
        {
            GameWorld world = GameWorld.Instance;
            if (world == null) { SetAimVisuals(false); return; }

            // Facing is a player-level control, not a run-only combat system. Keep it live
            // in Home Base, training, and dungeon rooms; modal screens alone suspend it.
            bool aimActive = !world.ModalOpen && !RunStartScreen.IsOpen;
            if (aimActive)
            {
                ApplyHardwareAim();
            }
            UpdateAimVisuals(aimActive);

            // Casting and combat conditions remain run-only even though facing is global.
            if (!world.RunActive || !aimActive || ArcaneInput.Context != ArcaneInputContext.Combat || Time.time < _silenceUntil || Time.time < _stunUntil) return;
            ControlSettings controls = ProfileManager.Current.controls;
            HandleCastInput(SpellSlot.Slot1, ArcaneInput.GetMouseButton(controls.primaryMouseButton) || ArcaneInput.GamepadSpellHeld(0),
                ArcaneInput.GetMouseButtonUp(controls.primaryMouseButton) || ArcaneInput.GamepadSpellReleased(0));
            HandleCastInput(SpellSlot.Slot2, ArcaneInput.GetMouseButton(controls.secondaryMouseButton) || ArcaneInput.GamepadSpellHeld(1),
                ArcaneInput.GetMouseButtonUp(controls.secondaryMouseButton) || ArcaneInput.GamepadSpellReleased(1));
            HandleCastInput(SpellSlot.Slot3, ArcaneInput.GetKey(controls.spellSlot3) || ArcaneInput.GamepadSpellHeld(2),
                ArcaneInput.GetKeyUp(controls.spellSlot3) || ArcaneInput.GamepadSpellReleased(2));
            if (ArcaneInput.GetKeyDown(controls.cancelCast))
                for (int i = 0; i < _chargeTimes.Length; i++) _chargeTimes[i] = 0f;
        }

        public void ApplyStats(PlayerStats stats, float healthRatio, float manaRatio)
        {
            _stats = stats;
            Health = Mathf.Clamp(stats.maxHealth * healthRatio, 1f, stats.maxHealth);
            Mana = Mathf.Clamp(stats.maxMana * manaRatio, 0f, stats.maxMana);
        }

        public void ResetForRun()
        {
            if (_stats == null) _stats = GameWorld.Instance.Stats;
            Health = _stats.maxHealth;
            Mana = _stats.maxMana;
            Ward = 0f;
            _healingReservoir = 0f;
            _wardUntil = 0f;
            _dodgePowerUntil = 0f;
            _firstCastReady = true;
            _slowUntil = _rootUntil = _silenceUntil = _stunUntil = _runeSuppressionUntil = _modifierCorruptionUntil = 0f;
            _suppressedSpellSlot = -1;
            _slowMultiplier = 1f;
            if (GameWorld.Instance.Equipment.HasMutation(UniqueMutation.ManaWardExchange))
            {
                Ward = _stats.maxMana * 0.3f;
                _wardUntil = float.PositiveInfinity;
            }
            for (int i = 0; i < _cooldowns.Length; i++) _cooldowns[i] = 0f;
            for (int i = 0; i < _chargeTimes.Length; i++) _chargeTimes[i] = 0f;
            _dodgeCooldown = 0f;
            _invulnerableUntil = Time.time + 1f;
            LastDamageSource = "Unknown";
            AimDirection = transform.forward.sqrMagnitude > 0.01f ? transform.forward.normalized : Vector3.forward;
            AimPoint = new Vector3(transform.position.x, HardwareMouseAim.CombatPlaneHeight, transform.position.z) + AimDirection * 4f;
        }

        public void RestoreSavedResources(float healthRatio, float manaRatio)
        {
            if (_stats == null) return;
            Health = Mathf.Clamp(_stats.maxHealth * Mathf.Clamp01(healthRatio), 1f, _stats.maxHealth);
            Mana = Mathf.Clamp(_stats.maxMana * Mathf.Clamp01(manaRatio), 0f, _stats.maxMana);
        }

        public void RestoreDamageSource(string source)
        {
            LastDamageSource = string.IsNullOrEmpty(source) ? "Unknown" : source;
        }

        public float GetCooldownRemaining(SpellSlot slot)
        {
            int index = (int)slot;
            return index < 0 || index >= _cooldowns.Length ? 0f : Mathf.Max(0f, _cooldowns[index]);
        }

        public void RestoreBetweenRooms(float healthFraction, float manaFraction)
        {
            if (_stats == null) return;
            Heal(_stats.maxHealth * healthFraction);
            RestoreMana(_stats.maxMana * manaFraction);
        }

        public void RestoreMana(float amount) { Mana = Mathf.Min(_stats.maxMana, Mana + Mathf.Max(0f, amount)); }

        public void AddWard(float amount, float duration)
        {
            Ward = Mathf.Max(Ward, amount * (1f + _stats.shieldPower));
            _wardUntil = Mathf.Max(_wardUntil, Time.time + duration);
        }

        public void TakeDamage(float rawDamage) { TakeDamage(rawDamage, "Enemy attack"); }

        public void TakeDamage(float rawDamage, string source)
        {
            if (!GameWorld.Instance.RunActive || Time.time < _invulnerableUntil) return;
            LastDamageSource = string.IsNullOrEmpty(source) ? "Enemy attack" : source;
            ApplyDamage(Mathf.Max(1f, rawDamage * BalanceTuning.ArmorMultiplier(_stats.armor) * BalanceTuning.ResistanceMultiplier(_stats.resistance)), true);
        }

        public void TakeUnavoidableDamage(float rawDamage) { TakeUnavoidableDamage(rawDamage, "Unavoidable damage"); }
        public void TakeUnavoidableDamage(float rawDamage, string source)
        {
            LastDamageSource = string.IsNullOrEmpty(source) ? "Unavoidable damage" : source;
            ApplyDamage(Mathf.Max(0f, rawDamage), false);
        }

        public void ReceiveDamage(DamageRequest request)
        {
            if (request.unavoidable) TakeUnavoidableDamage(request.amount, request.sourceDisplayName);
            else TakeDamage(request.amount, request.sourceDisplayName);
        }

        public void GrantSpawnProtection(float seconds)
        {
            _invulnerableUntil = Mathf.Max(_invulnerableUntil, Time.time + Mathf.Max(0f, seconds));
        }

        public void Heal(float amount)
        {
            float effective = Mathf.Max(0f, amount) * _stats.healingMultiplier;
            float missing = Mathf.Max(0f, _stats.maxHealth - Health);
            Health = Mathf.Min(_stats.maxHealth, Health + effective);
            if (effective > 0f)
                V12CombatEventBus.Publish(V12CombatEventType.PlayerHealed, "player", "player", Mathf.Min(effective, missing), transform.position, "Healing");
            if (GameWorld.Instance != null && GameWorld.Instance.Equipment.HasMutation(UniqueMutation.HealingReservoir))
                _healingReservoir = Mathf.Min(_stats.maxHealth * 0.6f, _healingReservoir + Mathf.Max(0f, effective - missing));
        }

        public void ApplyCondition(PlayerCondition condition, float duration, float strength, string source)
        {
            float resistance = condition == PlayerCondition.Stun ? _stats.stunResistance : _stats.knockbackResistance * 0.5f;
            float adjusted = Mathf.Max(0.08f, duration * (1f - Mathf.Clamp01(resistance)));
            if (condition == PlayerCondition.Slow)
            {
                _slowUntil = Mathf.Max(_slowUntil, Time.time + adjusted);
                _slowMultiplier = Mathf.Min(_slowMultiplier, Mathf.Clamp(1f - strength, 0.35f, 0.9f));
            }
            else if (condition == PlayerCondition.Root) _rootUntil = Mathf.Max(_rootUntil, Time.time + adjusted);
            else if (condition == PlayerCondition.Silence) _silenceUntil = Mathf.Max(_silenceUntil, Time.time + adjusted);
            else if (condition == PlayerCondition.Stun) _stunUntil = Mathf.Max(_stunUntil, Time.time + adjusted);
            V12CombatEventBus.Publish(V12CombatEventType.StatusApplied, source, "player", adjusted, transform.position, condition.ToString());
        }

        public void ApplyTemporaryRuneSuppression(int slot, float duration, string source)
        {
            _suppressedSpellSlot = Mathf.Clamp(slot, 0, 2);
            _runeSuppressionUntil = Mathf.Max(_runeSuppressionUntil, Time.time + Mathf.Clamp(duration, 0.25f, 8f));
            V12CombatEventBus.Publish(V12CombatEventType.StatusApplied, source, "player", duration, transform.position,
                "Spell " + (_suppressedSpellSlot + 1) + " temporarily suppressed; saved build unchanged");
        }

        public void ApplyTemporaryModifierCorruption(float duration, string source)
        {
            _modifierCorruptionUntil = Mathf.Max(_modifierCorruptionUntil, Time.time + Mathf.Clamp(duration, 0.25f, 8f));
            V12CombatEventBus.Publish(V12CombatEventType.StatusApplied, source, "player", duration, transform.position,
                "Support Rune power temporarily reduced; saved build unchanged");
        }

        public void TeleportToward(Vector3 target, float maximumDistance)
        {
            Vector3 delta = target - transform.position;
            delta.y = 0f;
            if (delta.magnitude > maximumDistance) delta = delta.normalized * maximumDistance;
            transform.position = DungeonObstacle.Resolve(ClampToRoom(transform.position + delta));
        }

        private void ApplyDamage(float damage, bool notifyTriggers)
        {
            if (Ward > 0f)
            {
                float wardBefore = Ward;
                float absorbed = Mathf.Min(Ward, damage);
                Ward -= absorbed;
                damage -= absorbed;
                GameWorld.Instance.Log("Shield absorbs " + Mathf.RoundToInt(absorbed) + " damage.");
                SpellDeliveryVisuals.BarrierEvent(transform, new Color(0.25f, 0.82f, 1f), wardBefore > 0f && Ward <= 0f, absorbed);
                if (notifyTriggers && absorbed > 0f)
                {
                    SpellExecutor.NotifyPlayerEvent(TriggerMoment.OnBlock, transform.position, _lastMove);
                    if (wardBefore > 0f && Ward <= 0f)
                        SpellExecutor.NotifyPlayerEvent(TriggerMoment.OnShieldBreak, transform.position, _lastMove);
                }
            }
            if (damage <= 0f) return;
            Health -= damage;
            V12CombatEventBus.Publish(V12CombatEventType.PlayerDamaged, LastDamageSource, "player", damage, transform.position, notifyTriggers ? "Hit" : "Unavoidable");
            if (RunStatistics.Instance != null) RunStatistics.Instance.RecordDamageTaken(damage);
            GameWorld.Instance.Log("You take " + Mathf.RoundToInt(damage) + " damage.");
            Flash(new Color(1f, 0.1f, 0.1f));
            Camera camera = Camera.main;
            if (camera != null)
            {
                IsometricCamera rig = camera.GetComponent<IsometricCamera>();
                if (rig != null) rig.Shake(Mathf.Clamp(damage / 60f, 0.08f, 0.45f), transform.position, 4, 0.22f);
            }
            if (notifyTriggers) SpellExecutor.NotifyPlayerEvent(TriggerMoment.OnDamageTaken, transform.position, _lastMove);
            if (Health <= 0f)
            {
                Health = 0f;
                GameWorld.Instance.GetComponent<RunDirector>().EndRun(false);
            }
        }

        private void Move()
        {
            ControlSettings controls = ProfileManager.Current.controls;
            float x = (ArcaneInput.GetKey(controls.moveRight) ? 1f : 0f) - (ArcaneInput.GetKey(controls.moveLeft) ? 1f : 0f);
            float z = (ArcaneInput.GetKey(controls.moveForward) ? 1f : 0f) - (ArcaneInput.GetKey(controls.moveBack) ? 1f : 0f);
            Vector2 gamepadMove = ArcaneInput.GamepadMove;
            if (gamepadMove.sqrMagnitude > 0.001f) { x = gamepadMove.x; z = gamepadMove.y; }
            Vector3 movement = new Vector3(x, 0f, z);
            Camera camera = Camera.main;
            IsometricCamera rig = camera == null ? null : camera.GetComponent<IsometricCamera>();
            if (rig != null && !ProfileManager.Current.accessibility.worldRelativeMovement) movement = rig.PlanarRight * x + rig.PlanarForward * z;
            if (movement.sqrMagnitude > 1f) movement.Normalize();
bool combatConditionsApply =
    GameWorld.Instance != null &&
    GameWorld.Instance.RunActive;

float conditionMultiplier =
    !combatConditionsApply
        ? 1f
        : Time.time < _rootUntil
            ? 0f
            : Time.time < _slowUntil
                ? _slowMultiplier
                : 1f;

if (!combatConditionsApply || Time.time >= _slowUntil)
    _slowMultiplier = 1f;
            float response = movement.sqrMagnitude > 0.01f ? 24f : 32f;
            _moveVelocity = Vector3.MoveTowards(_moveVelocity, desiredVelocity, response * Time.deltaTime);
            if (_moveVelocity.sqrMagnitude > 0.04f) _lastMove = _moveVelocity.normalized;
            transform.position = DungeonObstacle.Resolve(ClampToRoom(transform.position + _moveVelocity * Time.deltaTime));
        }

        private void ApplyHardwareAim()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            if (_hardwareAim == null) _hardwareAim = GetComponent<HardwareMouseAim>();
            if (_hardwareAim == null || !_hardwareAim.HasAim) return;
            AimPoint = _hardwareAim.WorldPoint;
            AimDirection = _hardwareAim.WorldDirection;
            transform.rotation = Quaternion.LookRotation(AimDirection, Vector3.up);
        }

        private void HandleCastInput(SpellSlot slot, bool held, bool released)
        {
            int index = (int)slot;
            CompiledSpell spell = GameWorld.Instance.GetSpell(slot);
            if (spell == null) return;
            if (spell.castMethod == SpellCastMethod.Charged)
            {
                if (!ProfileManager.Current.controls.holdToCharge)
                {
                    if (held && _chargeTimes[index] <= 0f) { _chargeTimes[index] = 1f; TryManualCast(slot, 1.25f, 1f); }
                    if (released) _chargeTimes[index] = 0f;
                    return;
                }
                if (held && _cooldowns[index] <= 0f) _chargeTimes[index] = Mathf.Min(1.5f, _chargeTimes[index] + Time.deltaTime);
                if (released && _chargeTimes[index] > 0.05f)
                {
                    float normalized = Mathf.Clamp01(_chargeTimes[index] / 1.5f);
                    _chargeTimes[index] = 0f;
                    TryManualCast(slot, Mathf.Lerp(0.75f, 2.1f, normalized), Mathf.Lerp(0.7f, 1.5f, normalized));
                }
                return;
            }
            if (held) TryManualCast(slot, 1f, 1f);
        }

        private void TryManualCast(SpellSlot slot, float castPower, float costScale)
        {
            int index = (int)slot;
            if (index < 0 || index > 2 || _cooldowns[index] > 0f) return;
            if (Time.time < _runeSuppressionUntil && index == _suppressedSpellSlot) return;
            CompiledSpell spell = GameWorld.Instance.GetSpell(slot);
            if (spell == null) return;
            float totalHealthCost = spell.healthCost * costScale;
            bool bloodCasting = GameWorld.Instance.Equipment.HasMutation(UniqueMutation.BloodCasting);
            if (bloodCasting) totalHealthCost += spell.manaCost * costScale;
            else if (Mana < spell.manaCost * costScale) return;
            if (Health <= totalHealthCost + 1f) return;
            if (!bloodCasting) Mana -= spell.manaCost * costScale;
            Health -= totalHealthCost;

            float manaRatio = Mana / Mathf.Max(1f, _stats.maxMana);
            float healthRatio = Health / Mathf.Max(1f, _stats.maxHealth);
            if (manaRatio < 0.3f) castPower *= 1f + _stats.lowManaDamage;
            if (healthRatio > 0.8f) castPower *= 1f + _stats.highHealthDamage;
            if (Ward > 0f) castPower *= 1f + _stats.shieldedDamage;
            if (Time.time < _dodgePowerUntil) castPower *= 1f + _stats.afterDodgeDamage;
            castPower *= 1f + _stats.manaToPower * manaRatio + _stats.healthToPower * (1f - healthRatio);
            if (_firstCastReady) { castPower *= 1f + _stats.firstCastPower; _firstCastReady = false; }
            if (Time.time < _modifierCorruptionUntil) castPower *= 0.75f;

            if (_healingReservoir > 0f && GameWorld.Instance.Equipment.HasMutation(UniqueMutation.HealingReservoir))
            {
                castPower *= 1f + Mathf.Clamp(_healingReservoir / Mathf.Max(1f, _stats.maxHealth), 0f, 0.6f);
                _healingReservoir = 0f;
            }

            Vector3 direction = AimDirection;
            Vector3 resolvedTarget = AimPoint;
            bool keyboardAssist = ProfileManager.Current.accessibility.autoAim && ArcaneInput.GetKey(KeyCode.LeftShift);
            float controllerAssist = ArcaneInput.GamepadActive ? ProfileManager.Current.controls.controllerAimAssist : 0f;
            if (keyboardAssist || controllerAssist > 0.01f)
            {
                float radius = keyboardAssist ? 2.8f : Mathf.Lerp(0.5f, 4.5f, controllerAssist);
                EnemyController assisted = GameWorld.Instance.NearestEnemy(AimPoint, radius);
                if (assisted != null)
                {
                    Vector3 assistedTarget = assisted.transform.position;
                    float blend = keyboardAssist ? 1f : Mathf.Lerp(0.15f, 0.75f, controllerAssist);
                    resolvedTarget = Vector3.Lerp(resolvedTarget, assistedTarget, blend);
                    direction = resolvedTarget - transform.position;
                }
            }
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) direction = transform.forward;
            direction.Normalize();
            SpellCastBudget budget = new SpellCastBudget(_stats.triggerEnergy);
            SpellExecutor.Cast(spell, new CastRequest
            {
                origin = transform.position + Vector3.up * 0.15f + direction * 0.75f,
                direction = direction,
                targetPosition = resolvedTarget,
                powerScale = castPower,
                generation = 0,
                budget = budget,
                manualCast = true
            });
            LastCastSlot = slot;
            _cooldowns[index] = spell.cooldown;
        }

        private void UpdateAimVisuals(bool visible)
        {
            if (_aimMarker == null)
            {
                _aimMarker = new GameObject("Exact Mouse Aim Point");
                _aimRing = RuntimeVisuals.Ring("Aim Ground Reticle", Vector3.zero, new Color(0.2f, 0.95f, 1f), 0.42f, 0.055f, _aimMarker.transform);
                _aimRing.transform.localPosition = Vector3.zero;
                GameObject directionObject = new GameObject("Aim Direction");
                directionObject.transform.SetParent(_aimMarker.transform, false);
                _aimDirection = directionObject.AddComponent<LineRenderer>();
                _aimDirection.sharedMaterial = RuntimeVisuals.Material(new Color(0.2f, 0.95f, 1f), 0.8f);
                _aimDirection.positionCount = 2;
                _aimDirection.startWidth = 0.045f;
                _aimDirection.endWidth = 0.018f;
                _aimDirection.startColor = new Color(0.2f, 0.95f, 1f, 0.8f);
                _aimDirection.endColor = new Color(0.2f, 0.95f, 1f, 0.15f);
                _aimDirection.useWorldSpace = true;
            }
            SetAimVisuals(visible);
            if (!visible) return;
            bool interactable = WorldInteractionController.Instance != null && WorldInteractionController.Instance.Current != null;
            bool target = GameWorld.Instance.NearestEnemy(AimPoint, 1.6f) != null;
            Color aimColor = interactable ? new Color(1f, 0.76f, 0.12f) : target ? new Color(0.25f, 1f, 0.42f) :
                ProfileManager.Current.accessibility.highContrastCursor ? new Color(1f, 0.95f, 0.05f) : new Color(0.2f, 0.95f, 1f);
            _aimRing.startColor = _aimRing.endColor = aimColor;
            _aimDirection.startColor = aimColor;
            _aimDirection.endColor = new Color(aimColor.r, aimColor.g, aimColor.b, 0.15f);
            _aimMarker.transform.position = AimPoint + Vector3.up * 0.03f;
            Vector3 from = transform.position + Vector3.up * 0.08f;
            Vector3 direction = AimPoint - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) direction = transform.forward;
            Vector3 to = from + direction.normalized * Mathf.Min(2.4f, direction.magnitude);
            _aimDirection.SetPosition(0, from);
            _aimDirection.SetPosition(1, to);
        }

        private void SetAimVisuals(bool visible)
        {
            if (_aimMarker != null && _aimMarker.activeSelf != visible) _aimMarker.SetActive(visible);
        }

        private void Dodge()
        {
            if (_dodgeCooldown > 0f) return;
            Vector3 origin = transform.position;
            Vector3 direction = _lastMove.sqrMagnitude > 0.01f ? _lastMove : transform.forward;
            transform.position = DungeonObstacle.Resolve(ClampToRoom(transform.position + direction * 4f));
            _dodgeCooldown = _stats.dodgeCooldown;
            _invulnerableUntil = Time.time + 0.35f;
            _dodgePowerUntil = Time.time + 2.5f;
            if (RunStatistics.Instance != null) RunStatistics.Instance.RecordDodge();
            V12CombatEventBus.Publish(V12CombatEventType.PlayerDodged, "player", string.Empty, 1f, origin, direction.ToString());
            SpellExecutor.NotifyPlayerEvent(TriggerMoment.OnDodge, origin, direction);

            if (GameWorld.Instance.Equipment.HasMutation(UniqueMutation.DodgeCastsSecondary))
            {
                CompiledSpell secondary = GameWorld.Instance.GetSpell(SpellSlot.Slot2);
                if (secondary != null)
                    SpellExecutor.Cast(secondary, new CastRequest { origin = origin, direction = direction, targetPosition = origin,
                        powerScale = 0.65f, generation = 1, budget = new SpellCastBudget(_stats.triggerEnergy * 0.7f), manualCast = false });
            }
        }

        public void ReduceCooldownsAfterKill(float fraction)
        {
            fraction = Mathf.Clamp01(fraction);
            for (int i = 0; i < _cooldowns.Length; i++) _cooldowns[i] = Mathf.Max(0f, _cooldowns[i] * (1f - fraction));
        }

        private static Vector3 ClampToRoom(Vector3 value)
        {
            value.x = Mathf.Clamp(value.x, -15.2f, 15.2f);
            value.z = Mathf.Clamp(value.z, -15.2f, 15.2f);
            value.y = 1f;
            return value;
        }

        private void Flash(Color color)
        {
            if (ProfileManager.Current.accessibility.reducedFlashes) return;
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = RuntimeVisuals.Material(color, 0.8f);
            CancelInvoke(nameof(RestoreColor));
            Invoke(nameof(RestoreColor), 0.12f);
        }

        private void RestoreColor()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = RuntimeVisuals.Material(new Color(0.2f, 0.85f, 1f), 0.3f);
        }
    }
}
