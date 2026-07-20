using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public static class ElementalReactionRuntime
    {
        private static int _suppressDirectBuildup;
        private static int _ambientDepth;
        private static ReactionContext22 _ambientContext;

        private sealed class ContextScope22 : IDisposable
        {
            private readonly ReactionContext22 _previous;
            private readonly int _previousDepth;

            public ContextScope22(ReactionContext22 context)
            {
                _previous = _ambientContext;
                _previousDepth = _ambientDepth;
                _ambientContext = context;
                _ambientDepth++;
            }

            public void Dispose()
            {
                _ambientContext = _previous;
                _ambientDepth = _previousDepth;
            }
        }

        public static IDisposable UseContext(ReactionContext22 context)
        {
            return new ContextScope22(context);
        }

        public static void RegisterDirectHit(
            EnemyController enemy,
            SpellElement spellElement,
            float damage,
            bool critical)
        {
            if (_suppressDirectBuildup > 0 ||
                enemy == null ||
                enemy.IsDead ||
                damage <= 0.01f)
            {
                return;
            }

            ReactionElement element =
                ElementalReactionCodex.FromSpellElement(spellElement);

            if (element == ReactionElement.None)
                return;

            float healthRatio =
                enemy.MaxHealth <= 0f
                    ? 0f
                    : damage / enemy.MaxHealth;

            float buildup =
                Mathf.Clamp(
                    0.7f + healthRatio * 10f +
                    (critical ? 0.35f : 0f),
                    0.7f,
                    2.75f);

            ReactionContext22 context =
                _ambientDepth > 0 && _ambientContext.IsValid
                    ? _ambientContext
                    : ReactionContext22.Direct(ReactionSourceKind22.DirectCast);

            ApplyBuildup(
                enemy,
                element,
                buildup,
                5f,
                context);

            ElementalReactionController controller = Get(enemy);

            if (controller != null)
            {
                controller.RecordHitDamage(damage);
                controller.RecordIncomingContext(context);
            }
        }

        public static void ApplyBuildup(
            EnemyController enemy,
            ReactionElement element,
            float amount,
            float duration,
            bool propagated)
        {
            ReactionContext22 context =
                _ambientDepth > 0 && _ambientContext.IsValid
                    ? _ambientContext
                    : ReactionContext22.Legacy(propagated);

            ApplyBuildup(enemy, element, amount, duration, context);
        }

        public static void ApplyBuildup(
            EnemyController enemy,
            ReactionElement element,
            float amount,
            float duration,
            ReactionContext22 context)
        {
            if (enemy == null ||
                enemy.IsDead ||
                element == ReactionElement.None ||
                amount <= 0f)
            {
                return;
            }

            if (!context.IsValid)
                context = ReactionContext22.Legacy(context.generation > 0);

            if (context.generation > ReactionBalance22.MaximumPropagationGeneration &&
                context.buildupCoefficient <= 0f)
            {
                ReactionDiagnostics22.RecordBlocked("generation-buildup", context);
                return;
            }

            ElementalReactionController controller = Get(enemy);

            if (controller != null)
                controller.AddBuildup(element, amount, duration, context);
        }

        public static void NotifyDeath(EnemyController enemy)
        {
            if (enemy == null)
                return;

            ElementalReactionController controller =
                enemy.GetComponent<ElementalReactionController>();

            if (controller != null)
                controller.ResolveDeath();
        }

        public static string GetStatusSummary(EnemyController enemy)
        {
            if (enemy == null)
                return string.Empty;

            ElementalReactionController controller =
                enemy.GetComponent<ElementalReactionController>();

            return controller == null
                ? string.Empty
                : controller.StatusSummary;
        }

        public static void DealReactionDamage(
            EnemyController target,
            float amount,
            ReactionElement element,
            Color color,
            bool critical)
        {
            ReactionContext22 context =
                _ambientDepth > 0 && _ambientContext.IsValid
                    ? _ambientContext
                    : ReactionContext22.Legacy(true);

            DealReactionDamage(target, amount, element, color, critical, context);
        }

        public static void DealReactionDamage(
            EnemyController target,
            float amount,
            ReactionElement element,
            Color color,
            bool critical,
            ReactionContext22 context)
        {
            if (target == null ||
                target.IsDead ||
                amount <= 0.01f)
            {
                return;
            }

            if (!context.IsValid)
                context = ReactionContext22.Legacy(true);

            float scaledAmount =
                amount * Mathf.Max(0f, context.damageCoefficient);

            if (scaledAmount <= 0.01f)
                return;

            ElementalReactionController controller = Get(target);
            if (controller != null)
                controller.RecordIncomingContext(context);

            ReactionLineageRegistry22.Touch(context);
            _suppressDirectBuildup++;

            try
            {
                using (UseContext(context))
                {
                    target.TakeDamage(
                        scaledAmount,
                        color,
                        ElementalReactionCodex.ToSpellElement(element),
                        critical);
                }
            }
            finally
            {
                _suppressDirectBuildup--;
            }
        }

        public static List<EnemyController> EnemiesWithin(
            Vector3 position,
            float radius,
            EnemyController exclude)
        {
            List<EnemyController> results =
                new List<EnemyController>();

            GameWorld world = GameWorld.Instance;

            if (world == null || world.Enemies == null)
                return results;

            float radiusSquared = radius * radius;

            foreach (EnemyController enemy in world.Enemies)
            {
                if (enemy == null ||
                    enemy == exclude ||
                    enemy.IsDead)
                {
                    continue;
                }

                Vector3 delta = enemy.transform.position - position;
                delta.y = 0f;

                if (delta.sqrMagnitude <= radiusSquared)
                    results.Add(enemy);
            }

            results.Sort((a, b) =>
            {
                float aDistance =
                    (a.transform.position - position).sqrMagnitude;

                float bDistance =
                    (b.transform.position - position).sqrMagnitude;

                return aDistance.CompareTo(bDistance);
            });

            return results;
        }

        public static ElementalReactionField SpawnField(
            ReactionElement signature,
            Vector3 position,
            float radius,
            float duration,
            float damagePerPulse,
            float buildupPerPulse)
        {
            ReactionContext22 context =
                _ambientDepth > 0 && _ambientContext.IsValid
                    ? _ambientContext
                    : ReactionContext22.Direct(ReactionSourceKind22.PrimaryReaction)
                        .Derive(ReactionSourceKind22.Field);

            return SpawnField(
                signature,
                position,
                radius,
                duration,
                damagePerPulse,
                buildupPerPulse,
                ReactionFieldAuthority22.PrimaryReaction,
                context,
                EntityId.None);
        }

        public static ElementalReactionField SpawnField(
            ReactionElement signature,
            Vector3 position,
            float radius,
            float duration,
            float damagePerPulse,
            float buildupPerPulse,
            ReactionFieldAuthority22 authority,
            ReactionContext22 context,
            EntityId ownerId)
        {
            if (authority != ReactionFieldAuthority22.Cosmetic &&
                !context.canCreateField &&
                context.generation >= 2)
            {
                ReactionDiagnostics22.RecordBlocked("field-permission", context);
                return null;
            }

            return ElementalReactionField.Spawn(
                signature,
                position,
                radius,
                duration,
                damagePerPulse,
                buildupPerPulse,
                authority,
                context,
                ownerId);
        }

        private static ElementalReactionController Get(EnemyController enemy)
        {
            ElementalReactionController controller =
                enemy.GetComponent<ElementalReactionController>();

            if (controller == null)
                controller = enemy.gameObject.AddComponent<ElementalReactionController>();

            controller.Initialize(enemy);
            return controller;
        }
    }

    public sealed class ElementalReactionController : MonoBehaviour
    {
        private static readonly ReactionElement[] OrderedElements =
        {
            ReactionElement.Fire,
            ReactionElement.Cold,
            ReactionElement.Lightning,
            ReactionElement.Physical,
            ReactionElement.Blood,
            ReactionElement.Toxic,
            ReactionElement.Void
        };

        private readonly float[] _directBuildup = new float[7];
        private readonly float[] _propagatedBuildup = new float[7];
        private readonly float[] _expiresAt = new float[7];
        private readonly float[] _majorUntil = new float[7];
        private readonly float[] _recoveryUntil = new float[7];
        private readonly bool[] _majorWasActive = new bool[7];
        private readonly ReactionContext22[] _elementContexts = new ReactionContext22[7];
        private readonly Dictionary<int, float> _signatureLockouts =
            new Dictionary<int, float>();

        private EnemyController _owner;
        private ReactionElement _lastApplied;
        private ReactionElement _pendingSignature;
        private ReactionElement _resolvedSignature;
        private ReactionContext22 _lastContext;
        private ReactionContext22 _lastIncomingContext;

        private float _resolveAt;
        private float _nextReactionAllowed;
        private float _reactionCharge;
        private float _lastHitDamage;
        private float _nextDotTick;
        private float _nextTrail;
        private float _nextArc;
        private float _nextAura;
        private float _nextVoidPull;
        private bool _deathResolved;
        private Vector3 _lastPosition;

        public ReactionContext22 LastContext
        {
            get { return _lastContext; }
        }

        public string StatusSummary
        {
            get
            {
                if (_owner == null)
                    return string.Empty;

                List<string> states = new List<string>();

                for (int i = 0; i < OrderedElements.Length; i++)
                {
                    ReactionElement element = OrderedElements[i];
                    float total = Total(i);

                    if (total > 0.05f && Time.time < _expiresAt[i])
                    {
                        states.Add(
                            ElementalReactionCodex.StackName(element) +
                            " " + Mathf.CeilToInt(total));
                    }

                    if (Time.time < _majorUntil[i])
                    {
                        states.Add(ElementalReactionCodex.MajorStateName(element));
                    }
                    else if (Time.time < _recoveryUntil[i])
                    {
                        states.Add(ElementalReactionCodex.DisplayName(element) + " RESOLVE");
                    }
                }

                ElementalReactionDefinition definition =
                    ElementalReactionCodex.Get(_pendingSignature);

                if (definition != null && Time.time < _resolveAt)
                    states.Add("ASSEMBLING " + definition.displayName);

                return string.Join(" · ", states);
            }
        }

        public void Initialize(EnemyController owner)
        {
            if (_owner != null)
                return;

            _owner = owner;
            _lastPosition = owner.transform.position;
            _nextDotTick = Time.time + 0.7f;
        }

        public void RecordHitDamage(float damage)
        {
            _lastHitDamage = Mathf.Max(_lastHitDamage * 0.5f, damage);
        }

        public void RecordIncomingContext(ReactionContext22 context)
        {
            if (!context.IsValid)
                return;

            _lastIncomingContext = context;
            _lastContext = context;
        }

        public void AddBuildup(
            ReactionElement element,
            float amount,
            float duration,
            bool propagated)
        {
            AddBuildup(
                element,
                amount,
                duration,
                ReactionContext22.Legacy(propagated));
        }

        public void AddBuildup(
            ReactionElement element,
            float amount,
            float duration,
            ReactionContext22 context)
        {
            if (_owner == null || _owner.IsDead || _deathResolved)
                return;

            int index = ElementalReactionCodex.IndexOf(element);
            if (index < 0)
                return;

            if (!context.IsValid)
                context = ReactionContext22.Legacy(context.generation > 0);

            float threshold = ElementalReactionCodex.Threshold(element);
            float beforeTotal = Total(index);
            bool direct = context.generation <= 0 &&
                          context.sourceKind != ReactionSourceKind22.Field &&
                          context.sourceKind != ReactionSourceKind22.DeathReaction &&
                          context.sourceKind != ReactionSourceKind22.Echo;

            float adjustedAmount =
                amount * Mathf.Max(0f, context.buildupCoefficient);

            if (adjustedAmount <= 0.0001f)
            {
                ReactionDiagnostics22.RecordBlocked("zero-buildup-authority", context);
                return;
            }

            if (direct)
            {
                _directBuildup[index] =
                    Mathf.Clamp(
                        _directBuildup[index] + adjustedAmount,
                        0f,
                        threshold * 1.75f);
            }
            else
            {
                float cap = threshold * ReactionBalance22.PropagatedThresholdCap;
                _propagatedBuildup[index] =
                    Mathf.Clamp(
                        _propagatedBuildup[index] + adjustedAmount,
                        0f,
                        cap);
            }

            _expiresAt[index] =
                Mathf.Max(
                    _expiresAt[index],
                    Time.time + Mathf.Max(0.5f, duration));

            _lastApplied = element;
            _lastContext = context;
            _elementContexts[index] = context;

            if (context.canRechargeReaction)
            {
                _reactionCharge +=
                    adjustedAmount *
                    Mathf.Max(0f, context.reactionRechargeCoefficient);
            }

            ReactionDiagnostics22.RecordApplication(context, element, adjustedAmount);

            ProceduralSpellPresentation.EmitAilment(
                _owner,
                element,
                Total(index),
                threshold,
                duration,
                !direct);

            if (Total(index) >= threshold &&
                CanActivateMajor(index, threshold, beforeTotal, context))
            {
                ActivateMajor(element, index, threshold);
            }

            RefreshAssembly(true);
        }

        private bool CanActivateMajor(
            int index,
            float threshold,
            float beforeTotal,
            ReactionContext22 context)
        {
            if (!context.canActivateMajor || Time.time < _recoveryUntil[index])
                return false;

            if (context.generation <= 0)
                return true;

            if (context.generation == 1)
            {
                return beforeTotal >= threshold * ReactionBalance22.GenerationOnePreparationRequirement &&
                       _directBuildup[index] >= threshold * ReactionBalance22.DirectThresholdRequirement;
            }

            return false;
        }

        private void Update()
        {
            if (_owner == null ||
                _owner.IsDead ||
                GameWorld.Instance == null ||
                !GameWorld.Instance.RunActive)
            {
                return;
            }

            UpdateMajorRecovery();
            DecayExpiredBuildup();
            TickMinorDamage();
            TickMajorStates();
            RefreshAssembly(false);

            if (_pendingSignature != ReactionElement.None &&
                Time.time >= _resolveAt &&
                Time.time >= _nextReactionAllowed &&
                !SignatureLocked(_pendingSignature))
            {
                ResolvePendingReaction();
            }

            _lastPosition = transform.position;
        }

        private void UpdateMajorRecovery()
        {
            for (int i = 0; i < OrderedElements.Length; i++)
            {
                bool active = Time.time < _majorUntil[i];

                if (_majorWasActive[i] && !active)
                {
                    _recoveryUntil[i] =
                        Mathf.Max(
                            _recoveryUntil[i],
                            Time.time + ReactionBalance22.MajorRecoveryDuration(_owner));
                }

                _majorWasActive[i] = active;
            }
        }

        private void DecayExpiredBuildup()
        {
            for (int i = 0; i < OrderedElements.Length; i++)
            {
                if (Time.time < _expiresAt[i])
                    continue;

                _directBuildup[i] =
                    Mathf.MoveTowards(
                        _directBuildup[i],
                        0f,
                        Time.deltaTime * 2.5f);

                _propagatedBuildup[i] =
                    Mathf.MoveTowards(
                        _propagatedBuildup[i],
                        0f,
                        Time.deltaTime * 3.25f);
            }
        }

        private void TickMinorDamage()
        {
            if (Time.time < _nextDotTick)
                return;

            _nextDotTick = Time.time + 0.75f;

            float fire = Stack(ReactionElement.Fire);
            float blood = Stack(ReactionElement.Blood);
            float toxic = Stack(ReactionElement.Toxic);
            float voidStacks = Stack(ReactionElement.Void);
            float movement = (transform.position - _lastPosition).magnitude;

            float damage =
                _owner.MaxHealth *
                (
                    fire * 0.00085f +
                    toxic * 0.00075f +
                    voidStacks * 0.0005f +
                    blood * 0.0007f *
                    (movement > 0.08f ? 1.4f : 1f)
                );

            if (MajorActive(ReactionElement.Fire))
                damage += _owner.MaxHealth * 0.0045f;

            if (MajorActive(ReactionElement.Blood))
                damage += _owner.MaxHealth * 0.0038f;

            if (MajorActive(ReactionElement.Toxic))
                damage += _owner.MaxHealth * 0.0032f;

            ReactionElement dotMask = ReactionElement.None;
            if (fire > 0f) dotMask |= ReactionElement.Fire;
            if (blood > 0f) dotMask |= ReactionElement.Blood;
            if (toxic > 0f) dotMask |= ReactionElement.Toxic;
            if (voidStacks > 0f) dotMask |= ReactionElement.Void;

            if (damage > 0.01f)
            {
                ReactionContext22 context =
                    ContextForSignature(dotMask)
                        .Derive(ReactionSourceKind22.Environmental)
                        .WithoutDeathPropagation();

                ElementalReactionRuntime.DealReactionDamage(
                    _owner,
                    damage,
                    ElementalReactionCodex.PrimaryElement(dotMask),
                    ElementalReactionCodex.BlendColor(dotMask),
                    false,
                    context);
            }
        }

        private void TickMajorStates()
        {
            if (MajorActive(ReactionElement.Fire) && Time.time >= _nextTrail)
            {
                float moved = (transform.position - _lastPosition).magnitude;

                if (moved > 0.08f)
                {
                    _nextTrail = Time.time + 1.5f;
                    // ARCANE_PATCH_221_ENTITY_ID
                    EntityId ownerId = _owner.GetEntityId();

                    if (ElementalReactionField.CountOwned(ownerId, ReactionElement.Fire) < 2)
                    {
                        ReactionContext22 context =
                            ContextForElement(ReactionElement.Fire)
                                .Derive(ReactionSourceKind22.Field)
                                .WithoutFieldCreation();

                        ElementalReactionRuntime.SpawnField(
                            ReactionElement.Fire,
                            transform.position,
                            1.05f,
                            2.2f,
                            _owner.MaxHealth * 0.001f,
                            0.20f,
                            ReactionFieldAuthority22.SecondaryPropagation,
                            context,
                            ownerId);
                    }
                }
            }

            if (MajorActive(ReactionElement.Lightning) && Time.time >= _nextArc)
            {
                _nextArc = Time.time + 1.25f;
                ReactionContext22 context =
                    ContextForElement(ReactionElement.Lightning)
                        .Derive(ReactionSourceKind22.Chain)
                        .WithoutFieldCreation();

                ArcLightning(2, 4.5f, _owner.MaxHealth * 0.009f, context);
            }

            if (MajorActive(ReactionElement.Toxic) && Time.time >= _nextAura)
            {
                _nextAura = Time.time + 1.25f;
                ReactionContext22 context =
                    ContextForElement(ReactionElement.Toxic)
                        .Derive(ReactionSourceKind22.Environmental)
                        .WithoutFieldCreation();

                List<EnemyController> targets =
                    ElementalReactionRuntime.EnemiesWithin(
                        transform.position,
                        2.7f,
                        _owner);

                int limit = Mathf.Min(3, targets.Count);
                for (int i = 0; i < limit; i++)
                {
                    ElementalReactionRuntime.ApplyBuildup(
                        targets[i],
                        ReactionElement.Toxic,
                        0.30f,
                        4f,
                        context);
                }
            }

            if (MajorActive(ReactionElement.Void) && Time.time >= _nextVoidPull)
            {
                _nextVoidPull = Time.time + 0.5f;
                List<EnemyController> targets =
                    ElementalReactionRuntime.EnemiesWithin(
                        transform.position,
                        3.5f,
                        _owner);

                int limit = Mathf.Min(4, targets.Count);
                for (int i = 0; i < limit; i++)
                    targets[i].PullToward(transform.position, 2.4f);
            }
        }

        private void ActivateMajor(
            ReactionElement element,
            int index,
            float threshold)
        {
            float duration = MajorDuration(element);
            ConsumeThreshold(index, threshold);

            if (Time.time < _majorUntil[index])
            {
                _majorUntil[index] =
                    Mathf.Min(
                        Time.time + duration * 1.25f,
                        _majorUntil[index] + duration * 0.15f);
                return;
            }

            _majorUntil[index] = Time.time + duration;
            _majorWasActive[index] = true;

            ProceduralSpellPresentation.EmitMajorAilment(
                _owner,
                element,
                duration);

            switch (element)
            {
                case ReactionElement.Cold:
                    _owner.ApplyFreeze(_owner.IsBoss ? 0.55f : 1.8f);
                    break;

                case ReactionElement.Physical:
                    _owner.ApplyVulnerability(
                        _owner.IsBoss ? 0.1f : 0.2f,
                        duration);
                    break;

                case ReactionElement.Void:
                    ReactionPresentation22.TryBurst(
                        transform.position + Vector3.up * 0.7f,
                        ElementalReactionCodex.ColorFor(element),
                        0.75f,
                        _lastContext);
                    break;
            }

            if (GameWorld.Instance != null)
            {
                GameWorld.Instance.Log(
                    _owner.DisplayName +
                    " becomes " +
                    ElementalReactionCodex.MajorStateName(element) +
                    ".");
            }
        }

        private void ConsumeThreshold(int index, float threshold)
        {
            float total = Total(index);
            if (total <= 0f)
                return;

            float consume = Mathf.Min(threshold, total);
            float directShare = _directBuildup[index] / total;
            float directConsumed = consume * directShare;
            float propagatedConsumed = consume - directConsumed;

            _directBuildup[index] =
                Mathf.Max(0f, _directBuildup[index] - directConsumed);

            _propagatedBuildup[index] =
                Mathf.Max(0f, _propagatedBuildup[index] - propagatedConsumed);

            float overflowCap = threshold * ReactionBalance22.MajorOverflowFraction;
            float overflow = Total(index);

            if (overflow > overflowCap)
            {
                float scale = overflowCap / Mathf.Max(0.0001f, overflow);
                _directBuildup[index] *= scale;
                _propagatedBuildup[index] *= scale;
            }
        }

        private float MajorDuration(ReactionElement element)
        {
            switch (element)
            {
                case ReactionElement.Fire: return 5.5f;
                case ReactionElement.Cold: return _owner.IsBoss ? 1f : 2.4f;
                case ReactionElement.Lightning: return 4.5f;
                case ReactionElement.Physical: return 3.5f;
                case ReactionElement.Blood: return 5f;
                case ReactionElement.Toxic: return 6f;
                case ReactionElement.Void: return 4.5f;
                default: return 3f;
            }
        }

        private void RefreshAssembly(bool buildupChanged)
        {
            ReactionElement signature = ActiveSignature();
            int count = ElementalReactionCodex.CountBits(signature);

            if (count < 2)
            {
                _pendingSignature = ReactionElement.None;

                if (signature == ReactionElement.None)
                    _resolvedSignature = ReactionElement.None;

                return;
            }

            ElementalReactionDefinition definition =
                ElementalReactionCodex.Get(signature);

            if (definition == null || SignatureLocked(signature))
                return;

            bool signatureChanged = signature != _pendingSignature;
            bool recharged =
                signature == _resolvedSignature &&
                _reactionCharge >= 4f + count * 1.5f &&
                Time.time >= _nextReactionAllowed;

            ReactionElement previousPending = _pendingSignature;

            if (signatureChanged || recharged)
            {
                _pendingSignature = signature;
                _resolveAt = Time.time + definition.assemblyWindow;

                ProceduralSpellPresentation.EmitAssembly(
                    _owner,
                    signature,
                    _lastApplied,
                    definition.assemblyWindow,
                    previousPending != ReactionElement.None &&
                    previousPending != signature);

                if (signatureChanged)
                    _resolvedSignature = ReactionElement.None;
            }
            else if (buildupChanged && signature != _resolvedSignature)
            {
                _resolveAt = Mathf.Max(_resolveAt, Time.time + 0.15f);
            }
        }

        private bool SignatureLocked(ReactionElement signature)
        {
            float until;
            return _signatureLockouts.TryGetValue((int)signature, out until) &&
                   Time.time < until;
        }

        private void ResolvePendingReaction()
        {
            ElementalReactionDefinition definition =
                ElementalReactionCodex.Get(_pendingSignature);

            if (definition == null)
            {
                _pendingSignature = ReactionElement.None;
                return;
            }

            _resolvedSignature = definition.signature;
            _pendingSignature = ReactionElement.None;
            _nextReactionAllowed = Time.time + 0.35f;
            _reactionCharge = 0f;
            _signatureLockouts[(int)definition.signature] =
                Time.time + ReactionBalance22.SignatureLockout(definition.tier);

            float baseDamage =
                Mathf.Max(
                    _lastHitDamage * 0.4f,
                    _owner.MaxHealth *
                    (0.004f + definition.ElementCount * 0.003f));

            float reactionDamage =
                Mathf.Min(
                    _owner.MaxHealth * 0.18f,
                    baseDamage * definition.damageMultiplier);

            ReactionContext22 reactionContext =
                ContextForSignature(definition.signature)
                    .Derive(ReactionSourceKind22.PrimaryReaction);

            ReactionPresentation22.TryBurst(
                transform.position + Vector3.up * 0.65f,
                ElementalReactionCodex.BlendColor(definition.signature),
                Mathf.Clamp(
                    0.7f + definition.ElementCount * 0.14f,
                    0.75f,
                    1.6f),
                reactionContext);

            ElementalReactionRuntime.DealReactionDamage(
                _owner,
                reactionDamage * 0.32f,
                definition.catalyst,
                ElementalReactionCodex.BlendColor(definition.signature),
                (int)definition.tier >= (int)ReactionTier.Catastrophe,
                reactionContext);

            ReactionPresentation22.EmitReactionResolved(
                _owner,
                definition,
                reactionDamage,
                false,
                reactionContext);

            ElementalReactionMechanicExecutor.Execute(
                definition,
                this,
                _owner,
                reactionDamage,
                false,
                reactionContext);

            ConsumeForReaction(
                definition.signature,
                ReactionBalance22.ReactionConsumption(definition.tier));

            if (GameWorld.Instance != null)
            {
                ReactionMechanicPlan plan =
                    ElementalReactionMechanicCodex.Get(definition.signature);

                GameWorld.Instance.Log(
                    definition.displayName +
                    " [" + definition.tier + "] resolves on " +
                    _owner.DisplayName +
                    (plan == null ? "." : " · budgeted graph " + plan.graphId + "."));
            }
        }

        public void ResolveDeath()
        {
            if (_deathResolved || _owner == null)
                return;

            _deathResolved = true;
            ReactionElement signature = DeathSignature();
            int count = ElementalReactionCodex.CountBits(signature);

            if (count <= 0)
                return;

            ReactionContext22 parentContext =
                _lastIncomingContext.IsValid
                    ? _lastIncomingContext
                    : ContextForSignature(signature);

            if (!parentContext.canTriggerDeathReaction ||
                parentContext.generation >= ReactionBalance22.MaximumPropagationGeneration)
            {
                ReactionDiagnostics22.RecordBlocked("death-propagation", parentContext);
                ReactionPresentation22.EmitLocalDeathMarker(
                    _owner.DamagePoint,
                    signature,
                    0.45f,
                    parentContext);
                return;
            }

            ReactionContext22 deathContext =
                parentContext.Derive(ReactionSourceKind22.DeathReaction);

            ReactionDiagnostics22.RecordReactionDeath(
                deathContext,
                deathContext.generation >= 2);

            if (count == 1)
            {
                ResolveSingleElementDeath(signature, deathContext);
                return;
            }

            ElementalReactionDefinition definition =
                ElementalReactionCodex.Get(signature);

            if (definition == null)
                return;

            float damage =
                _owner.MaxHealth *
                Mathf.Clamp(
                    0.018f + definition.ElementCount * 0.007f,
                    0.03f,
                    0.09f) *
                definition.damageMultiplier;

            ReactionPresentation22.TryBurst(
                transform.position + Vector3.up * 0.6f,
                ElementalReactionCodex.BlendColor(signature),
                Mathf.Clamp(
                    0.85f + definition.ElementCount * 0.16f,
                    1f,
                    1.9f),
                deathContext);

            ReactionPresentation22.EmitReactionResolved(
                _owner,
                definition,
                damage,
                true,
                deathContext);

            ElementalReactionMechanicExecutor.Execute(
                definition,
                this,
                _owner,
                damage,
                true,
                deathContext);

            if (GameWorld.Instance != null)
            {
                ReactionMechanicPlan plan =
                    ElementalReactionMechanicCodex.Get(signature);

                GameWorld.Instance.Log(
                    definition.displayName +
                    " death graph erupts from " +
                    _owner.DisplayName +
                    (plan == null ? "." : " · budgeted graph " + plan.graphId + "."));
            }
        }

        private void ResolveSingleElementDeath(
            ReactionElement element,
            ReactionContext22 context)
        {
            bool reduced = context.generation >= 2;
            float radius = reduced ? 2.1f : 2.8f;
            float damage = _owner.MaxHealth * (reduced ? 0.014f : 0.04f);
            int maxTargets = reduced ? 2 : ReactionBalance22.MaximumSingleDeathTargets;

            if (element == ReactionElement.Lightning)
                maxTargets = Mathf.Min(maxTargets, 3);

            float totalBudget = SingleDeathBuildupBudget(element, reduced);
            float perTargetCap = SingleDeathPerTargetCap(element, reduced);

            ReactionPresentation22.EmitSingleElementDeath(
                _owner,
                element,
                radius,
                reduced ? 0.65f : 1f,
                context);

            Color color = ElementalReactionCodex.ColorFor(element);
            ReactionPresentation22.TryBurst(
                transform.position + Vector3.up * 0.6f,
                color,
                reduced ? 0.55f : 1f,
                context);

            List<EnemyController> nearby =
                ElementalReactionRuntime.EnemiesWithin(
                    transform.position,
                    radius,
                    _owner);

            List<EnemyController> selected =
                SelectDeathTargets(nearby, element, maxTargets, context);

            float remaining = totalBudget;
            int remainingTargets = selected.Count;

            for (int i = 0; i < selected.Count; i++)
            {
                EnemyController enemy = selected[i];
                float distance =
                    Vector3.Distance(transform.position, enemy.transform.position);
                float falloff = Mathf.Clamp01(1f - distance / radius);

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    damage * Mathf.Lerp(0.45f, 1f, falloff),
                    element,
                    color,
                    false,
                    context);

                float fairShare =
                    remainingTargets <= 0
                        ? 0f
                        : remaining / remainingTargets;

                float stacks =
                    Mathf.Min(
                        perTargetCap,
                        fairShare * Mathf.Lerp(0.75f, 1f, falloff));

                ElementalReactionRuntime.ApplyBuildup(
                    enemy,
                    element,
                    stacks,
                    4.5f,
                    context);

                remaining = Mathf.Max(0f, remaining - stacks);
                remainingTargets--;

                if (element == ReactionElement.Physical)
                {
                    using (ElementalReactionRuntime.UseContext(context))
                    {
                        enemy.ApplyImpact(
                            enemy.transform.position - transform.position,
                            reduced ? 8f : 14f);
                    }
                }

                if (element == ReactionElement.Void)
                    enemy.PullToward(transform.position, reduced ? 3f : 5f);

                ReactionPresentation22.EmitLocalDeathMarker(
                    enemy.DamagePoint,
                    element,
                    reduced ? 0.30f : 0.48f,
                    context);
            }

            if (!reduced &&
                context.canCreateField &&
                (element == ReactionElement.Fire ||
                 element == ReactionElement.Toxic ||
                 element == ReactionElement.Void ||
                 element == ReactionElement.Blood))
            {
                ElementalReactionRuntime.SpawnField(
                    element,
                    transform.position,
                    2.1f,
                    element == ReactionElement.Toxic ? 4.2f : 3.2f,
                    damage * 0.08f,
                    0.30f,
                    ReactionFieldAuthority22.DeathResidue,
                    context.WithoutFieldCreation(),
                    EntityId.None);
            }
        }

        private List<EnemyController> SelectDeathTargets(
            List<EnemyController> nearby,
            ReactionElement element,
            int limit,
            ReactionContext22 context)
        {
            nearby.Sort((a, b) =>
            {
                ElementalReactionController ac = a.GetComponent<ElementalReactionController>();
                ElementalReactionController bc = b.GetComponent<ElementalReactionController>();
                float ap = ac == null ? 0f : ac.GetPreparation(element);
                float bp = bc == null ? 0f : bc.GetPreparation(element);
                int preparation = bp.CompareTo(ap);
                if (preparation != 0)
                    return preparation;

                float ad = (a.transform.position - transform.position).sqrMagnitude;
                float bd = (b.transform.position - transform.position).sqrMagnitude;
                return ad.CompareTo(bd);
            });

            List<EnemyController> selected = new List<EnemyController>();

            for (int i = 0; i < nearby.Count && selected.Count < limit; i++)
            {
                if (ReactionLineageRegistry22.TryMarkTarget(context, nearby[i], 0.35f))
                    selected.Add(nearby[i]);
            }

            return selected;
        }

        private float SingleDeathBuildupBudget(
            ReactionElement element,
            bool reduced)
        {
            if (reduced)
                return element == ReactionElement.Cold ? 0.7f : 0.55f;

            switch (element)
            {
                case ReactionElement.Cold: return 5f;
                case ReactionElement.Lightning: return 2.1f;
                case ReactionElement.Toxic: return 3.4f;
                case ReactionElement.Void: return 3.2f;
                case ReactionElement.Physical: return 3.2f;
                default: return 4.2f;
            }
        }

        private float SingleDeathPerTargetCap(
            ReactionElement element,
            bool reduced)
        {
            if (reduced)
                return element == ReactionElement.Cold ? 0.35f : 0.28f;

            switch (element)
            {
                case ReactionElement.Cold: return 1.25f;
                case ReactionElement.Lightning: return 0.75f;
                case ReactionElement.Toxic: return 0.85f;
                default: return 1.05f;
            }
        }

        private void ArcLightning(
            int targets,
            float radius,
            float damage,
            ReactionContext22 context)
        {
            List<EnemyController> nearby =
                ElementalReactionRuntime.EnemiesWithin(
                    transform.position,
                    radius,
                    _owner);

            Vector3 from = transform.position + Vector3.up * 0.7f;
            int count = Mathf.Min(targets, nearby.Count);
            int jump = 0;

            for (int i = 0; i < nearby.Count && jump < count; i++)
            {
                EnemyController target = nearby[i];

                if (!ReactionLineageRegistry22.TryMarkTarget(context, target, 0.35f))
                    continue;

                Vector3 to = target.DamagePoint;
                RuntimeVisuals.Beam(
                    from,
                    to,
                    ElementalReactionCodex.ColorFor(ReactionElement.Lightning),
                    0.10f * ReactionBalance22.ChainVisualIntensity(jump));

                ReactionContext22 jumpContext =
                    context.WithCoefficients(
                        ReactionBalance22.ChainDamageCoefficient(jump),
                        1f,
                        1f);

                ElementalReactionRuntime.DealReactionDamage(
                    target,
                    damage,
                    ReactionElement.Lightning,
                    ElementalReactionCodex.ColorFor(ReactionElement.Lightning),
                    false,
                    jumpContext);

                ElementalReactionRuntime.ApplyBuildup(
                    target,
                    ReactionElement.Lightning,
                    ReactionBalance22.ChainBuildupAmount(jump),
                    4f,
                    jumpContext);

                from = to;
                jump++;
            }
        }

        public float ConsumeBuildupForMechanic(
            ReactionElement signature,
            float fraction)
        {
            float consumed = 0f;
            float clamped = Mathf.Clamp01(fraction);

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(signature))
            {
                int index = ElementalReactionCodex.IndexOf(element);
                if (index < 0)
                    continue;

                float direct = _directBuildup[index] * clamped;
                float propagated = _propagatedBuildup[index] * clamped;
                _directBuildup[index] -= direct;
                _propagatedBuildup[index] -= propagated;
                consumed += direct + propagated;
            }

            return consumed;
        }

        private void ConsumeForReaction(
            ReactionElement signature,
            float fraction)
        {
            float remaining = Mathf.Clamp01(1f - fraction);

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(signature))
            {
                int index = ElementalReactionCodex.IndexOf(element);
                if (index < 0)
                    continue;

                _directBuildup[index] *= remaining;
                _propagatedBuildup[index] *= remaining;
            }
        }

        private ReactionElement ActiveSignature()
        {
            ReactionElement signature = ReactionElement.None;

            for (int i = 0; i < OrderedElements.Length; i++)
            {
                float requirement =
                    ElementalReactionCodex.Threshold(OrderedElements[i]) *
                    ReactionBalance22.ReactionParticipationFraction;

                if (Total(i) >= requirement &&
                    Time.time < _expiresAt[i] + 0.4f)
                {
                    signature |= OrderedElements[i];
                }
            }

            return signature;
        }

        private ReactionElement DeathSignature()
        {
            ReactionElement signature = ReactionElement.None;

            for (int i = 0; i < OrderedElements.Length; i++)
            {
                float threshold = ElementalReactionCodex.Threshold(OrderedElements[i]);

                if (Time.time < _majorUntil[i] ||
                    Total(i) >= threshold * 0.75f)
                {
                    signature |= OrderedElements[i];
                }
            }

            return signature;
        }

        private bool MajorActive(ReactionElement element)
        {
            int index = ElementalReactionCodex.IndexOf(element);
            return index >= 0 && Time.time < _majorUntil[index];
        }

        private float Stack(ReactionElement element)
        {
            int index = ElementalReactionCodex.IndexOf(element);
            return index < 0 ? 0f : Total(index);
        }

        private float Total(int index)
        {
            return _directBuildup[index] + _propagatedBuildup[index];
        }

        public float GetDirectBuildup(ReactionElement element)
        {
            int index = ElementalReactionCodex.IndexOf(element);
            return index < 0 ? 0f : _directBuildup[index];
        }

        public float GetPropagatedBuildup(ReactionElement element)
        {
            int index = ElementalReactionCodex.IndexOf(element);
            return index < 0 ? 0f : _propagatedBuildup[index];
        }

        public float GetPreparation(ReactionElement element)
        {
            int index = ElementalReactionCodex.IndexOf(element);
            if (index < 0)
                return 0f;

            return Total(index) /
                   Mathf.Max(0.01f, ElementalReactionCodex.Threshold(element));
        }

        public float GetRecoveryRemaining(ReactionElement element)
        {
            int index = ElementalReactionCodex.IndexOf(element);
            return index < 0 ? 0f : Mathf.Max(0f, _recoveryUntil[index] - Time.time);
        }

        private ReactionContext22 ContextForElement(ReactionElement element)
        {
            int index = ElementalReactionCodex.IndexOf(element);

            if (index >= 0 && _elementContexts[index].IsValid)
                return _elementContexts[index];

            if (_lastContext.IsValid)
                return _lastContext;

            return ReactionContext22.Direct(ReactionSourceKind22.Environmental);
        }

        public ReactionContext22 ContextForSignature(ReactionElement signature)
        {
            ReactionContext22 best = _lastContext;
            int bestGeneration = best.IsValid ? best.generation : int.MaxValue;

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(signature))
            {
                int index = ElementalReactionCodex.IndexOf(element);
                if (index < 0)
                    continue;

                ReactionContext22 candidate = _elementContexts[index];
                if (candidate.IsValid && candidate.generation <= bestGeneration)
                {
                    best = candidate;
                    bestGeneration = candidate.generation;
                }
            }

            return best.IsValid
                ? best
                : ReactionContext22.Direct(ReactionSourceKind22.Environmental);
        }
    }
}
