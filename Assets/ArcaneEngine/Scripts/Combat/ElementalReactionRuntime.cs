using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public static class ElementalReactionRuntime
    {
        private static int _suppressDirectBuildup;

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

            ApplyBuildup(
                enemy,
                element,
                buildup,
                5f,
                false);

            ElementalReactionController controller = Get(enemy);

            if (controller != null)
                controller.RecordHitDamage(damage);
        }

        public static void ApplyBuildup(
            EnemyController enemy,
            ReactionElement element,
            float amount,
            float duration,
            bool propagated)
        {
            if (enemy == null ||
                enemy.IsDead ||
                element == ReactionElement.None ||
                amount <= 0f)
            {
                return;
            }

            ElementalReactionController controller = Get(enemy);

            if (controller != null)
            {
                controller.AddBuildup(
                    element,
                    amount,
                    duration,
                    propagated);
            }
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
            if (target == null ||
                target.IsDead ||
                amount <= 0.01f)
            {
                return;
            }

            _suppressDirectBuildup++;

            try
            {
                target.TakeDamage(
                    amount,
                    color,
                    ElementalReactionCodex.ToSpellElement(element),
                    critical);
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
            return ElementalReactionField.Spawn(
                signature,
                position,
                radius,
                duration,
                damagePerPulse,
                buildupPerPulse);
        }

        private static ElementalReactionController Get(
            EnemyController enemy)
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

        private readonly float[] _buildup = new float[7];
        private readonly float[] _expiresAt = new float[7];
        private readonly float[] _majorUntil = new float[7];

        private EnemyController _owner;
        private ReactionElement _lastApplied;
        private ReactionElement _pendingSignature;
        private ReactionElement _resolvedSignature;

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

                    if (_buildup[i] > 0.05f &&
                        Time.time < _expiresAt[i])
                    {
                        states.Add(
                            ElementalReactionCodex.StackName(element) +
                            " " +
                            Mathf.CeilToInt(_buildup[i]));
                    }

                    if (Time.time < _majorUntil[i])
                    {
                        states.Add(
                            ElementalReactionCodex.MajorStateName(element));
                    }
                }

                ElementalReactionDefinition definition =
                    ElementalReactionCodex.Get(_pendingSignature);

                if (definition != null &&
                    Time.time < _resolveAt)
                {
                    states.Add("ASSEMBLING " + definition.displayName);
                }

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

        public void AddBuildup(
            ReactionElement element,
            float amount,
            float duration,
            bool propagated)
        {
            if (_owner == null ||
                _owner.IsDead ||
                _deathResolved)
            {
                return;
            }

            int index = ElementalReactionCodex.IndexOf(element);

            if (index < 0)
                return;

            float adjustedAmount =
                propagated
                    ? amount * 0.72f
                    : amount;

            _buildup[index] =
                Mathf.Clamp(
                    _buildup[index] + adjustedAmount,
                    0f,
                    14f);

            _expiresAt[index] =
                Mathf.Max(
                    _expiresAt[index],
                    Time.time + Mathf.Max(0.5f, duration));

            _lastApplied = element;
            _reactionCharge += adjustedAmount;

            if (_buildup[index] >=
                ElementalReactionCodex.Threshold(element))
            {
                ActivateMajor(element, index);
            }

            RefreshAssembly(true);
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

            DecayExpiredBuildup();
            TickMinorDamage();
            TickMajorStates();
            RefreshAssembly(false);

            if (_pendingSignature != ReactionElement.None &&
                Time.time >= _resolveAt &&
                Time.time >= _nextReactionAllowed)
            {
                ResolvePendingReaction();
            }

            _lastPosition = transform.position;
        }

        private void DecayExpiredBuildup()
        {
            for (int i = 0; i < _buildup.Length; i++)
            {
                if (_buildup[i] <= 0f)
                    continue;

                if (Time.time >= _expiresAt[i])
                {
                    _buildup[i] =
                        Mathf.MoveTowards(
                            _buildup[i],
                            0f,
                            Time.deltaTime * 2.5f);
                }
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

            float movement =
                (transform.position - _lastPosition).magnitude;

            float damage =
                _owner.MaxHealth *
                (
                    fire * 0.00115f +
                    toxic * 0.00095f +
                    voidStacks * 0.00065f +
                    blood * 0.0009f *
                    (movement > 0.08f ? 1.75f : 1f)
                );

            if (MajorActive(ReactionElement.Fire))
                damage += _owner.MaxHealth * 0.006f;

            if (MajorActive(ReactionElement.Blood))
                damage += _owner.MaxHealth * 0.005f;

            if (MajorActive(ReactionElement.Toxic))
                damage += _owner.MaxHealth * 0.004f;

            ReactionElement dotMask = ReactionElement.None;

            if (fire > 0f) dotMask |= ReactionElement.Fire;
            if (blood > 0f) dotMask |= ReactionElement.Blood;
            if (toxic > 0f) dotMask |= ReactionElement.Toxic;
            if (voidStacks > 0f) dotMask |= ReactionElement.Void;

            if (damage > 0.01f)
            {
                ReactionElement primary =
                    ElementalReactionCodex.PrimaryElement(dotMask);

                ElementalReactionRuntime.DealReactionDamage(
                    _owner,
                    damage,
                    primary,
                    ElementalReactionCodex.BlendColor(dotMask),
                    false);
            }
        }

        private void TickMajorStates()
        {
            if (MajorActive(ReactionElement.Fire) &&
                Time.time >= _nextTrail)
            {
                float moved =
                    (transform.position - _lastPosition).magnitude;

                if (moved > 0.08f)
                {
                    _nextTrail = Time.time + 0.45f;

                    ElementalReactionRuntime.SpawnField(
                        ReactionElement.Fire,
                        transform.position,
                        1.15f,
                        2.6f,
                        _owner.MaxHealth * 0.0015f,
                        0.32f);
                }
            }

            if (MajorActive(ReactionElement.Lightning) &&
                Time.time >= _nextArc)
            {
                _nextArc = Time.time + 0.85f;
                ArcLightning(1, 4.5f, _owner.MaxHealth * 0.012f);
            }

            if (MajorActive(ReactionElement.Toxic) &&
                Time.time >= _nextAura)
            {
                _nextAura = Time.time + 1f;

                foreach (
                    EnemyController enemy
                    in ElementalReactionRuntime.EnemiesWithin(
                        transform.position,
                        2.7f,
                        _owner))
                {
                    ElementalReactionRuntime.ApplyBuildup(
                        enemy,
                        ReactionElement.Toxic,
                        0.45f,
                        4f,
                        true);
                }
            }

            if (MajorActive(ReactionElement.Void) &&
                Time.time >= _nextVoidPull)
            {
                _nextVoidPull = Time.time + 0.15f;

                foreach (
                    EnemyController enemy
                    in ElementalReactionRuntime.EnemiesWithin(
                        transform.position,
                        3.5f,
                        _owner))
                {
                    enemy.PullToward(transform.position, 3.2f);
                }
            }
        }

        private void ActivateMajor(
            ReactionElement element,
            int index)
        {
            float duration = MajorDuration(element);

            if (Time.time < _majorUntil[index])
            {
                _majorUntil[index] =
                    Mathf.Min(
                        Time.time + duration * 1.5f,
                        _majorUntil[index] + duration * 0.25f);

                return;
            }

            _majorUntil[index] = Time.time + duration;

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
                    GameFeelSystem.Burst(
                        transform.position + Vector3.up * 0.7f,
                        ElementalReactionCodex.ColorFor(element),
                        0.9f);
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

            if (definition == null)
                return;

            bool signatureChanged =
                signature != _pendingSignature;

            bool recharged =
                signature == _resolvedSignature &&
                _reactionCharge >= 4f + count * 1.5f &&
                Time.time >= _nextReactionAllowed;

            if (signatureChanged || recharged)
            {
                _pendingSignature = signature;
                _resolveAt =
                    Time.time + definition.assemblyWindow;

                if (signatureChanged)
                    _resolvedSignature = ReactionElement.None;
            }
            else if (buildupChanged &&
                     signature != _resolvedSignature)
            {
                _resolveAt =
                    Mathf.Max(
                        _resolveAt,
                        Time.time + 0.15f);
            }
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
    _nextReactionAllowed = Time.time + 1.45f;
    _reactionCharge = 0f;

    float baseDamage =
        Mathf.Max(
            _lastHitDamage * 0.4f,
            _owner.MaxHealth *
            (0.004f + definition.ElementCount * 0.003f));

    float reactionDamage =
        Mathf.Min(
            _owner.MaxHealth * 0.22f,
            baseDamage * definition.damageMultiplier);

    GameFeelSystem.Burst(
        transform.position + Vector3.up * 0.65f,
        ElementalReactionCodex.BlendColor(definition.signature),
        Mathf.Clamp(
            0.75f + definition.ElementCount * 0.18f,
            0.8f,
            2f));

    ElementalReactionRuntime.DealReactionDamage(
        _owner,
        reactionDamage * 0.32f,
        definition.catalyst,
        ElementalReactionCodex.BlendColor(definition.signature),
        definition.tier >= ReactionTier.Catastrophe);

    ElementalReactionMechanicExecutor.Execute(
        definition,
        this,
        _owner,
        reactionDamage,
        false);

    ConsumeForReaction(definition.signature, 0.24f);

    if (GameWorld.Instance != null)
    {
        ReactionMechanicPlan plan =
            ElementalReactionMechanicCodex.Get(definition.signature);

        GameWorld.Instance.Log(
            definition.displayName +
            " [" + definition.tier + "] resolves on " +
            _owner.DisplayName +
            (plan == null ? "." : " · graph " + plan.graphId + "."));
    }
}

        private void ApplyReactionToArea(
            ElementalReactionDefinition definition,
            float damage,
            bool deathReaction)
        {
            List<EnemyController> nearby =
                ElementalReactionRuntime.EnemiesWithin(
                    transform.position,
                    definition.radius,
                    _owner);

            Color color =
                ElementalReactionCodex.BlendColor(
                    definition.signature);

            int affected = 0;

            foreach (EnemyController enemy in nearby)
            {
                float distance =
                    Vector3.Distance(
                        transform.position,
                        enemy.transform.position);

                float falloff =
                    Mathf.Clamp01(
                        1f - distance /
                        Mathf.Max(0.1f, definition.radius));

                float scaled =
                    damage *
                    Mathf.Lerp(0.45f, 1f, falloff);

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    scaled,
                    definition.catalyst,
                    color,
                    deathReaction &&
                    definition.tier >= ReactionTier.Convergence);

                foreach (
                    ReactionElement element
                    in ElementalReactionCodex.Enumerate(
                        definition.signature))
                {
                    float stacks =
                        definition.spreadStacks *
                        Mathf.Lerp(0.35f, 0.8f, falloff);

                    ElementalReactionRuntime.ApplyBuildup(
                        enemy,
                        element,
                        stacks,
                        4.5f,
                        true);
                }

                if (ElementalReactionCodex.Contains(
                    definition.signature,
                    ReactionElement.Physical))
                {
                    Vector3 direction =
                        enemy.transform.position -
                        transform.position;

                    enemy.ApplyImpact(
                        direction,
                        15f +
                        definition.ElementCount * 4f);
                }

                if (ElementalReactionCodex.Contains(
                    definition.signature,
                    ReactionElement.Void))
                {
                    enemy.PullToward(
                        transform.position,
                        5f +
                        definition.ElementCount);
                }

                affected++;

                if (affected >=
                    Mathf.Max(
                        3,
                        definition.pulseCount +
                        definition.ElementCount))
                {
                    break;
                }
            }

            if (ElementalReactionCodex.Contains(
                definition.signature,
                ReactionElement.Lightning))
            {
                ArcLightning(
                    Mathf.Max(1, definition.pulseCount - 1),
                    definition.radius + 1.5f,
                    damage * 0.55f);
            }
        }

        public void ResolveDeath()
{
    if (_deathResolved || _owner == null)
        return;

    _deathResolved = true;

    ReactionElement signature =
        ActiveSignature() |
        MajorSignature();

    int count = ElementalReactionCodex.CountBits(signature);

    if (count <= 0)
        return;

    if (count == 1)
    {
        ResolveSingleElementDeath(signature);
        return;
    }

    ElementalReactionDefinition definition =
        ElementalReactionCodex.Get(signature);

    if (definition == null)
        return;

    float damage =
        _owner.MaxHealth *
        Mathf.Clamp(
            0.025f + definition.ElementCount * 0.011f,
            0.04f,
            0.14f) *
        definition.damageMultiplier;

    GameFeelSystem.Burst(
        transform.position + Vector3.up * 0.6f,
        ElementalReactionCodex.BlendColor(signature),
        Mathf.Clamp(
            1f + definition.ElementCount * 0.22f,
            1.2f,
            2.8f));

    ElementalReactionMechanicExecutor.Execute(
        definition,
        this,
        _owner,
        damage,
        true);

    if (GameWorld.Instance != null)
    {
        ReactionMechanicPlan plan =
            ElementalReactionMechanicCodex.Get(signature);

        GameWorld.Instance.Log(
            definition.displayName +
            " death graph erupts from " +
            _owner.DisplayName +
            (plan == null ? "." : " · graph " + plan.graphId + "."));
    }
}

        private void ResolveSingleElementDeath(
            ReactionElement element)
        {
            float radius = 3.2f;
            float damage = _owner.MaxHealth * 0.065f;
            Color color = ElementalReactionCodex.ColorFor(element);

            GameFeelSystem.Burst(
                transform.position + Vector3.up * 0.6f,
                color,
                1.25f);

            List<EnemyController> nearby =
                ElementalReactionRuntime.EnemiesWithin(
                    transform.position,
                    radius,
                    _owner);

            foreach (EnemyController enemy in nearby)
            {
                float distance =
                    Vector3.Distance(
                        transform.position,
                        enemy.transform.position);

                float falloff =
                    Mathf.Clamp01(1f - distance / radius);

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    damage * Mathf.Lerp(0.45f, 1f, falloff),
                    element,
                    color,
                    false);

                float stacks =
                    distance <= radius * 0.5f ? 2f : 1f;

                ElementalReactionRuntime.ApplyBuildup(
                    enemy,
                    element,
                    stacks,
                    4.5f,
                    true);

                if (element == ReactionElement.Physical)
                {
                    enemy.ApplyImpact(
                        enemy.transform.position - transform.position,
                        24f);
                }

                if (element == ReactionElement.Void)
                    enemy.PullToward(transform.position, 8f);
            }

            if (element == ReactionElement.Fire ||
                element == ReactionElement.Toxic ||
                element == ReactionElement.Void ||
                element == ReactionElement.Blood)
            {
                ElementalReactionRuntime.SpawnField(
                    element,
                    transform.position,
                    2.5f,
                    element == ReactionElement.Toxic ? 6f : 4.5f,
                    damage * 0.1f,
                    0.45f);
            }
        }

        private void ArcLightning(
            int targets,
            float radius,
            float damage)
        {
            List<EnemyController> nearby =
                ElementalReactionRuntime.EnemiesWithin(
                    transform.position,
                    radius,
                    _owner);

            Vector3 from =
                transform.position + Vector3.up * 0.7f;

            int count = Mathf.Min(targets, nearby.Count);

            for (int i = 0; i < count; i++)
            {
                EnemyController target = nearby[i];
                Vector3 to =
                    target.transform.position + Vector3.up * 0.65f;

                RuntimeVisuals.Beam(
                    from,
                    to,
                    ElementalReactionCodex.ColorFor(
                        ReactionElement.Lightning),
                    0.14f);

                ElementalReactionRuntime.DealReactionDamage(
                    target,
                    damage * Mathf.Pow(0.82f, i),
                    ReactionElement.Lightning,
                    ElementalReactionCodex.ColorFor(
                        ReactionElement.Lightning),
                    false);

                ElementalReactionRuntime.ApplyBuildup(
                    target,
                    ReactionElement.Lightning,
                    i == 0 ? 1.2f : 0.75f,
                    4f,
                    true);

                from = to;
            }
        }


        public float ConsumeBuildupForMechanic(
            ReactionElement signature,
            float fraction)
        {
            float consumed = 0f;
            float clamped = Mathf.Clamp01(fraction);

            foreach (
                ReactionElement element
                in ElementalReactionCodex.Enumerate(signature))
            {
                int index = ElementalReactionCodex.IndexOf(element);

                if (index < 0)
                    continue;

                float amount = _buildup[index] * clamped;
                _buildup[index] -= amount;
                consumed += amount;
            }

            return consumed;
        }

        private void ConsumeForReaction(
            ReactionElement signature,
            float fraction)
        {
            foreach (
                ReactionElement element
                in ElementalReactionCodex.Enumerate(signature))
            {
                int index = ElementalReactionCodex.IndexOf(element);

                if (index >= 0)
                {
                    _buildup[index] *=
                        Mathf.Clamp01(1f - fraction);
                }
            }
        }

        private ReactionElement ActiveSignature()
        {
            ReactionElement signature = ReactionElement.None;

            for (int i = 0; i < OrderedElements.Length; i++)
            {
                if (_buildup[i] > 0.05f &&
                    Time.time < _expiresAt[i] + 0.4f)
                {
                    signature |= OrderedElements[i];
                }
            }

            return signature;
        }

        private ReactionElement MajorSignature()
        {
            ReactionElement signature = ReactionElement.None;

            for (int i = 0; i < OrderedElements.Length; i++)
            {
                if (Time.time < _majorUntil[i])
                    signature |= OrderedElements[i];
            }

            return signature;
        }

        private bool MajorActive(ReactionElement element)
        {
            int index = ElementalReactionCodex.IndexOf(element);

            return index >= 0 &&
                   Time.time < _majorUntil[index];
        }

        private float Stack(ReactionElement element)
        {
            int index = ElementalReactionCodex.IndexOf(element);

            return index < 0 ? 0f : _buildup[index];
        }

        private static bool CreatesResidue(
            ReactionElement signature)
        {
            return ElementalReactionCodex.Contains(
                       signature,
                       ReactionElement.Fire) ||
                   ElementalReactionCodex.Contains(
                       signature,
                       ReactionElement.Toxic) ||
                   ElementalReactionCodex.Contains(
                       signature,
                       ReactionElement.Void) ||
                   ElementalReactionCodex.Contains(
                       signature,
                       ReactionElement.Blood);
        }
    }
}
