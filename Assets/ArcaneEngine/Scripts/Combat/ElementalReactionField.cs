using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ElementalReactionField : MonoBehaviour
    {
        private static readonly List<ElementalReactionField> Active =
            new List<ElementalReactionField>();

        private ReactionElement _signature;
        private ReactionFieldAuthority22 _authority;
        private ReactionContext22 _context;
        // ARCANE_PATCH_221_ENTITY_ID
        private EntityId _ownerId;
        private float _radius;
        private float _createdAt;
        private float _expiresAt;
        private float _damagePerPulse;
        private float _buildupPerPulse;
        private float _powerMultiplier;
        private float _nextPulse;
        private float _pulseInterval;
        private float _nextSurgeAllowed;
        private float _pendingSurgeAt;
        private float _pendingSurgePower;
        private float _pendingSurgeDuration;
        private int _reinforcementCount;
        private Renderer _renderer;
        private Vector3 _baseScale;

        public ReactionElement Signature { get { return _signature; } }
        public ReactionFieldAuthority22 Authority { get { return _authority; } }
        public ReactionContext22 Context { get { return _context; } }
        public float Radius { get { return _radius; } }
        public float RemainingDuration { get { return Mathf.Max(0f, _expiresAt - Time.time); } }
        public float PowerMultiplier { get { return _powerMultiplier; } }
        public int ReinforcementCount { get { return _reinforcementCount; } }

        public static int ActiveCount
        {
            get
            {
                CleanupList();
                return Active.Count;
            }
        }

        public static int CountOwned(EntityId ownerId, ReactionElement signature)
        {
            CleanupList();
            int count = 0;

            for (int i = 0; i < Active.Count; i++)
            {
                ElementalReactionField field = Active[i];
                if (field != null &&
                    field._ownerId == ownerId &&
                    ElementalReactionCodex.Contains(field._signature, signature))
                {
                    count++;
                }
            }

            return count;
        }

        public static ElementalReactionField[] Snapshot()
        {
            CleanupList();
            return Active.ToArray();
        }

        public static ElementalReactionField Spawn(
            ReactionElement signature,
            Vector3 position,
            float radius,
            float duration,
            float damagePerPulse,
            float buildupPerPulse)
        {
            ReactionContext22 context =
                ReactionContext22.Direct(ReactionSourceKind22.PrimaryReaction)
                    .Derive(ReactionSourceKind22.Field);

            return Spawn(
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

        public static ElementalReactionField Spawn(
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
            if (signature == ReactionElement.None ||
                authority == ReactionFieldAuthority22.Cosmetic)
            {
                return null;
            }

            CleanupList();
            radius = Mathf.Max(0.6f, radius);
            duration = Mathf.Max(0.4f, duration);

            ElementalReactionField mergeTarget =
                FindMergeTarget(position, radius, signature, authority);

            if (mergeTarget != null)
            {
                mergeTarget.Merge(
                    signature,
                    radius,
                    duration,
                    damagePerPulse,
                    buildupPerPulse,
                    authority,
                    context,
                    ownerId);

                return mergeTarget;
            }

            if (!EnsureCapacity(position, authority))
            {
                ReactionDiagnostics22.RecordFieldRejected(signature);
                return null;
            }

            GameObject gameObject =
                GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            gameObject.name =
                "Elemental Field " +
                ElementalReactionCodex.SignatureText(signature) +
                " [" + authority + "]";

            gameObject.transform.position =
                new Vector3(position.x, position.y + 0.025f, position.z);

            Collider collider = gameObject.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            ElementalReactionField result =
                gameObject.AddComponent<ElementalReactionField>();

            result.Initialize(
                signature,
                radius,
                duration,
                damagePerPulse,
                buildupPerPulse,
                authority,
                context,
                ownerId);

            Active.Add(result);
            return result;
        }

        public static void SurgeNear(
            Vector3 position,
            float radius,
            ReactionElement signature,
            float damageScale,
            float durationBonus)
        {
            SurgeNear(
                position,
                radius,
                signature,
                damageScale,
                durationBonus,
                ReactionContext22.Legacy(true));
        }

        public static void SurgeNear(
            Vector3 position,
            float radius,
            ReactionElement signature,
            float damageScale,
            float durationBonus,
            ReactionContext22 context)
        {
            CleanupList();
            float radiusSquared = radius * radius;
            ElementalReactionField best = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < Active.Count; i++)
            {
                ElementalReactionField field = Active[i];
                if (field == null)
                    continue;

                Vector3 delta = field.transform.position - position;
                delta.y = 0f;
                float distance = delta.sqrMagnitude;

                if (distance <= radiusSquared && distance < bestDistance)
                {
                    best = field;
                    bestDistance = distance;
                }
            }

            if (best == null)
            {
                if (context.canCreateField && context.generation <= 1)
                {
                    Spawn(
                        signature,
                        position,
                        Mathf.Max(1f, radius * 0.45f),
                        Mathf.Max(1f, durationBonus),
                        Mathf.Max(0.1f, damageScale),
                        0.12f,
                        ReactionFieldAuthority22.SecondaryPropagation,
                        context.WithoutFieldCreation(),
                        EntityId.None);
                }
                else
                {
                    ReactionDiagnostics22.RecordBlocked("surge-no-field", context);
                }
                return;
            }

            best.QueueSurge(damageScale, durationBonus, context);
        }

        private static ElementalReactionField FindMergeTarget(
            Vector3 position,
            float radius,
            ReactionElement signature,
            ReactionFieldAuthority22 authority)
        {
            ElementalReactionField best = null;
            float bestScore = float.MinValue;

            for (int i = 0; i < Active.Count; i++)
            {
                ElementalReactionField field = Active[i];
                if (field == null)
                    continue;

                float mergeDistance = Mathf.Max(field._radius, radius) * 0.72f;
                Vector3 delta = field.transform.position - position;
                delta.y = 0f;

                if (delta.sqrMagnitude > mergeDistance * mergeDistance)
                    continue;

                bool same = field._signature == signature;
                float score =
                    (same ? 10f : 4f) +
                    ReactionBalance22.FieldPriority(field._authority) +
                    ReactionBalance22.FieldPriority(authority) -
                    delta.magnitude * 0.2f;

                if (score > bestScore)
                {
                    best = field;
                    bestScore = score;
                }
            }

            return best;
        }

        private static bool EnsureCapacity(
            Vector3 position,
            ReactionFieldAuthority22 incomingAuthority)
        {
            int localCount = 0;
            ElementalReactionField weakestLocal = null;
            int weakestPriority = int.MaxValue;
            float localRadiusSquared = 12f * 12f;

            for (int i = 0; i < Active.Count; i++)
            {
                ElementalReactionField field = Active[i];
                if (field == null)
                    continue;

                Vector3 delta = field.transform.position - position;
                delta.y = 0f;

                if (delta.sqrMagnitude <= localRadiusSquared)
                {
                    localCount++;
                    int priority = ReactionBalance22.FieldPriority(field._authority);
                    if (priority < weakestPriority)
                    {
                        weakestPriority = priority;
                        weakestLocal = field;
                    }
                }
            }

            IReadOnlyList<PersistentSpellZone> spellZones =
                PersistentSpellZone.Active;
            for (int i = 0; i < spellZones.Count; i++)
            {
                PersistentSpellZone zone = spellZones[i];
                if (zone == null)
                    continue;

                Vector3 delta = zone.transform.position - position;
                delta.y = 0f;
                if (delta.sqrMagnitude <= localRadiusSquared)
                    localCount++;
            }

            int incomingPriority = ReactionBalance22.FieldPriority(incomingAuthority);

            if (localCount >= ReactionBalance22.MaximumLocalGameplayFields)
            {
                // Explicit SpellForge zones own the highest authority. Reaction
                // residue may replace only a weaker reaction field, never an
                // explicit Persistent zone.
                if (weakestLocal == null || incomingPriority <= weakestPriority)
                    return false;

                Active.Remove(weakestLocal);
                Destroy(weakestLocal.gameObject);
            }

            int totalFields = Active.Count + spellZones.Count;
            while (totalFields >= ReactionBalance22.MaximumGameplayFields)
            {
                ElementalReactionField weakest = FindWeakest();
                if (weakest == null ||
                    incomingPriority <= ReactionBalance22.FieldPriority(weakest._authority))
                {
                    return false;
                }

                Active.Remove(weakest);
                Destroy(weakest.gameObject);
                totalFields--;
            }

            return true;
        }

        private static ElementalReactionField FindWeakest()
        {
            ElementalReactionField weakest = null;
            int weakestPriority = int.MaxValue;
            float oldest = float.MaxValue;

            for (int i = 0; i < Active.Count; i++)
            {
                ElementalReactionField field = Active[i];
                if (field == null)
                    continue;

                int priority = ReactionBalance22.FieldPriority(field._authority);
                if (priority < weakestPriority ||
                    (priority == weakestPriority && field._createdAt < oldest))
                {
                    weakest = field;
                    weakestPriority = priority;
                    oldest = field._createdAt;
                }
            }

            return weakest;
        }

        private void Initialize(
            ReactionElement signature,
            float radius,
            float duration,
            float damagePerPulse,
            float buildupPerPulse,
            ReactionFieldAuthority22 authority,
            ReactionContext22 context,
            EntityId ownerId)
        {
            _signature = signature;
            _authority = authority;
            _context = PrepareFieldContext(context, authority);
            _ownerId = ownerId;
            _radius = Mathf.Max(0.6f, radius);
            _createdAt = Time.time;
            _expiresAt = Time.time + Mathf.Max(0.4f, duration);
            _damagePerPulse = Mathf.Max(0f, damagePerPulse);
            _buildupPerPulse = Mathf.Max(0f, buildupPerPulse);
            _powerMultiplier = 1f;
            _pulseInterval = CalculatePulseInterval(signature, duration);
            _nextPulse = Time.time + Mathf.Min(0.35f, _pulseInterval * 0.5f);
            _nextSurgeAllowed = Time.time + 0.5f;
            _renderer = GetComponent<Renderer>();

            RefreshMaterial();
            RefreshScale();

            ReactionPresentation22.EmitField(
                SpellPresentationEventType.FieldCreated,
                gameObject,
                _signature,
                _radius,
                duration,
                0.55f,
                _context,
                _authority);
        }

        private static ReactionContext22 PrepareFieldContext(
            ReactionContext22 context,
            ReactionFieldAuthority22 authority)
        {
            if (!context.IsValid)
                context = ReactionContext22.Legacy(true);

            context.sourceKind = ReactionSourceKind22.Field;
            context.canCreateField = false;

            if (authority != ReactionFieldAuthority22.ExplicitPersistent)
                context.canTriggerDeathReaction = false;

            if (authority == ReactionFieldAuthority22.DeathResidue ||
                authority == ReactionFieldAuthority22.SecondaryPropagation)
            {
                context.canActivateMajor = false;
                context.canRechargeReaction = false;
                context.reactionRechargeCoefficient = 0f;
            }

            return context;
        }

        private void Merge(
            ReactionElement signature,
            float radius,
            float duration,
            float damagePerPulse,
            float buildupPerPulse,
            ReactionFieldAuthority22 authority,
            ReactionContext22 context,
            EntityId ownerId)
        {
            bool sameSignature = _signature == signature;
            ReactionElement combined = _signature | signature;
            int existingPriority = ReactionBalance22.FieldPriority(_authority);
            int incomingPriority = ReactionBalance22.FieldPriority(authority);

            _signature = combined;
            _radius = Mathf.Clamp(
                Mathf.Max(_radius, radius) +
                (sameSignature ? 0f : ElementalReactionCodex.CountBits(combined) * 0.05f),
                0.6f,
                7.5f);

            float remaining = RemainingDuration;
            float extension = Mathf.Min(duration * 0.25f, 2f);
            _expiresAt = Time.time + Mathf.Max(remaining, duration) + extension;

            _damagePerPulse = Mathf.Max(_damagePerPulse, damagePerPulse);
            _buildupPerPulse = Mathf.Max(_buildupPerPulse, buildupPerPulse);
            _powerMultiplier = Mathf.Min(
                ReactionBalance22.FieldPowerCap,
                _powerMultiplier + ReactionBalance22.FieldReinforcement);
            _reinforcementCount++;

            if (incomingPriority > existingPriority)
            {
                _authority = authority;
                _context = PrepareFieldContext(context, authority);
                _ownerId = ownerId;
            }

            _pulseInterval = CalculatePulseInterval(_signature, RemainingDuration);
            RefreshMaterial();
            RefreshScale();
            ReactionDiagnostics22.RecordFieldMerge(_signature);

            ReactionPresentation22.EmitField(
                SpellPresentationEventType.FieldMerged,
                gameObject,
                _signature,
                _radius,
                duration,
                0.70f,
                _context,
                _authority);
        }

        private void QueueSurge(
            float damageScale,
            float durationBonus,
            ReactionContext22 context)
        {
            if (Time.time < _nextSurgeAllowed)
            {
                ReactionDiagnostics22.RecordBlocked("field-surge-cooldown", context);
                return;
            }

            _nextSurgeAllowed = Time.time + ReactionBalance22.FieldSurgeCooldown;
            _pendingSurgeAt = Time.time + 0.4f;
            _pendingSurgePower = Mathf.Min(
                ReactionBalance22.FieldSurgePowerCap,
                Mathf.Max(0f, damageScale - 1f));
            _pendingSurgeDuration = Mathf.Min(
                Mathf.Max(0f, durationBonus),
                RemainingDuration * ReactionBalance22.FieldSurgeDurationCap);

            ReactionPresentation22.EmitField(
                SpellPresentationEventType.FieldMerged,
                gameObject,
                _signature,
                _radius,
                0.4f,
                0.45f,
                _context,
                _authority);
        }

        private void ApplyPendingSurge()
        {
            if (_pendingSurgeAt <= 0f || Time.time < _pendingSurgeAt)
                return;

            _powerMultiplier = Mathf.Min(
                ReactionBalance22.FieldPowerCap,
                _powerMultiplier + _pendingSurgePower);
            _expiresAt += _pendingSurgeDuration;
            _pendingSurgeAt = 0f;
            _pendingSurgePower = 0f;
            _pendingSurgeDuration = 0f;
            _nextPulse = Mathf.Max(_nextPulse, Time.time + 0.12f);
        }

        private void Update()
        {
            if (Time.time >= _expiresAt)
            {
                Destroy(gameObject);
                return;
            }

            ApplyPendingSurge();

            float quietMotion = 1f + Mathf.Sin(Time.time * 1.5f) * 0.012f;
            transform.localScale = new Vector3(
                _baseScale.x * quietMotion,
                _baseScale.y,
                _baseScale.z * quietMotion);

            if (Time.time < _nextPulse)
                return;

            _nextPulse = Time.time + _pulseInterval;
            Pulse();
        }

        private void Pulse()
        {
            ReactionPresentation22.EmitField(
                SpellPresentationEventType.FieldPulsed,
                gameObject,
                _signature,
                _radius,
                _pulseInterval,
                0.50f,
                _context,
                _authority);

            List<EnemyController> enemies =
                ElementalReactionRuntime.EnemiesWithin(
                    transform.position,
                    _radius,
                    null);

            ReactionElement primary =
                ElementalReactionCodex.PrimaryElement(_signature);

            Color color = ElementalReactionCodex.BlendColor(_signature);
            int elementCount = Mathf.Max(1, ElementalReactionCodex.CountBits(_signature));
            float damageAuthority = ReactionBalance22.FieldDamageAuthority(_authority);
            float buildupAuthority = ReactionBalance22.FieldBuildupAuthority(_authority);
            int controlCount = 0;

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyController enemy = enemies[i];
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                float falloff = Mathf.Clamp01(1f - distance / Mathf.Max(0.1f, _radius));
                ReactionContext22 pulseContext = _context;

                if (!ReactionLineageRegistry22.TryMarkTarget(
                    pulseContext,
                    enemy,
                    Mathf.Min(0.45f, _pulseInterval * 0.75f)))
                {
                    continue;
                }

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    _damagePerPulse *
                    _powerMultiplier *
                    damageAuthority *
                    Mathf.Lerp(0.45f, 1f, falloff),
                    primary,
                    color,
                    false,
                    pulseContext);

                if (buildupAuthority > 0f)
                {
                    foreach (ReactionElement element in ElementalReactionCodex.Enumerate(_signature))
                    {
                        ElementalReactionRuntime.ApplyBuildup(
                            enemy,
                            element,
                            _buildupPerPulse *
                            _powerMultiplier *
                            buildupAuthority /
                            Mathf.Sqrt(elementCount),
                            3.5f,
                            pulseContext);
                    }
                }

                if (controlCount < 4 &&
                    ElementalReactionCodex.Contains(_signature, ReactionElement.Void))
                {
                    enemy.PullToward(transform.position, 2f + elementCount * 0.35f);
                    controlCount++;
                }

                if (controlCount < 4 &&
                    ElementalReactionCodex.Contains(_signature, ReactionElement.Physical))
                {
                    using (ElementalReactionRuntime.UseContext(pulseContext))
                    {
                        enemy.ApplyImpact(
                            enemy.transform.position - transform.position,
                            4f + elementCount);
                    }
                    controlCount++;
                }
            }
        }

        private static float CalculatePulseInterval(
            ReactionElement signature,
            float duration)
        {
            ElementalReactionDefinition definition = ElementalReactionCodex.Get(signature);
            int pulses = definition == null ? 5 : Mathf.Max(1, definition.pulseCount);

            return Mathf.Clamp(
                Mathf.Max(0.6f, duration) / pulses,
                0.6f,
                1.2f);
        }

        private void RefreshMaterial()
        {
            if (_renderer == null)
                return;

            float alpha = _authority == ReactionFieldAuthority22.ExplicitPersistent
                ? 0.30f
                : _authority == ReactionFieldAuthority22.PrimaryReaction
                    ? 0.24f
                    : 0.18f;

            _renderer.sharedMaterial = RuntimeVisuals.Material(
                ElementalReactionCodex.BlendColor(_signature),
                alpha);
        }

        private void RefreshScale()
        {
            _baseScale = new Vector3(
                _radius * 2f,
                0.022f,
                _radius * 2f);
            transform.localScale = _baseScale;
        }

        private void OnDestroy()
        {
            ReactionPresentation22.EmitField(
                SpellPresentationEventType.FieldExpired,
                gameObject,
                _signature,
                _radius,
                0f,
                0f,
                _context,
                _authority);

            Active.Remove(this);
        }

        private static void CleanupList()
        {
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                if (Active[i] == null)
                    Active.RemoveAt(i);
            }
        }
    }
}
