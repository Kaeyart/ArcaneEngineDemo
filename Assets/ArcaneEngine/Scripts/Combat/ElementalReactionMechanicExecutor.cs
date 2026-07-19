using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public static class ElementalReactionMechanicExecutor
    {
        public static void Execute(
            ElementalReactionDefinition definition,
            ElementalReactionController controller,
            EnemyController owner,
            float baseDamage,
            bool death)
        {
            if (definition == null || owner == null)
                return;

            ReactionMechanicPlan plan =
                ElementalReactionMechanicCodex.Get(definition.signature);

            if (plan == null)
            {
                Fallback(definition, owner, baseDamage, death);
                return;
            }

            ReactionMechanicSpec[] mechanics =
                death ? plan.death : plan.resolve;

            for (int i = 0; i < mechanics.Length; i++)
            {
                ProceduralSpellPresentation.EmitReactionMechanic(
                    mechanics[i],
                    definition,
                    owner,
                    baseDamage,
                    death,
                    i);

                ExecuteSpec(
                    mechanics[i],
                    definition,
                    controller,
                    owner,
                    baseDamage,
                    death,
                    i);
            }
        }

        private static void ExecuteSpec(
            ReactionMechanicSpec spec,
            ElementalReactionDefinition definition,
            ElementalReactionController controller,
            EnemyController owner,
            float baseDamage,
            bool death,
            int mechanicIndex)
        {
            Vector3 position = owner.transform.position;
            ReactionElement primary =
                spec.element == ReactionElement.None
                    ? definition.catalyst
                    : spec.element;

            ReactionElement payload =
                spec.payload == ReactionElement.None
                    ? definition.signature
                    : spec.payload;

            float damage =
                Mathf.Max(0f, baseDamage * spec.magnitude);

            switch (spec.type)
            {
                case ReactionMechanicType.Burst:
                    DamageOwner(owner, damage, primary, payload, death);
                    DamageArea(
                        position,
                        owner,
                        spec.radius,
                        damage * 0.72f,
                        primary,
                        payload,
                        0.35f + definition.ElementCount * 0.08f,
                        death && definition.tier >= ReactionTier.Convergence);
                    break;

                case ReactionMechanicType.PulseNova:
                    ElementalReactionPulseEmitter.Spawn(
                        position,
                        owner,
                        payload,
                        primary,
                        damage,
                        spec.radius * 0.45f,
                        spec.radius,
                        spec.count,
                        Mathf.Max(0.12f, spec.duration / spec.count),
                        spec.delay,
                        ReactionPulseMode.Normal);
                    break;

                case ReactionMechanicType.ChainArc:
                    Chain(
                        position,
                        owner,
                        spec.radius,
                        spec.count,
                        damage,
                        primary,
                        payload);
                    break;

                case ReactionMechanicType.Pull:
                    PullArea(position, owner, spec.radius, 3f + spec.magnitude * 8f);
                    break;

                case ReactionMechanicType.Push:
                    PushArea(position, owner, spec.radius, 12f + spec.magnitude * 22f);
                    break;

                case ReactionMechanicType.Freeze:
                    FreezeArea(position, owner, spec.radius, spec.duration);
                    break;

                case ReactionMechanicType.Vulnerability:
                    VulnerabilityArea(
                        position,
                        owner,
                        spec.radius,
                        Mathf.Clamp(spec.magnitude, 0.05f, 0.5f),
                        spec.duration);
                    break;

                case ReactionMechanicType.BuildupWave:
                    ApplyPayloadArea(
                        position,
                        owner,
                        spec.radius,
                        payload,
                        spec.magnitude,
                        spec.duration,
                        false);
                    break;

                case ReactionMechanicType.Field:
                    ElementalReactionRuntime.SpawnField(
                        payload,
                        position,
                        spec.radius,
                        spec.duration,
                        damage * 0.18f,
                        Mathf.Max(0.15f, spec.magnitude * 0.45f));
                    break;

                case ReactionMechanicType.SplitFields:
                    SpawnSplitFields(
                        position,
                        payload,
                        damage,
                        spec.radius,
                        spec.duration,
                        spec.count,
                        spec.magnitude,
                        definition.signature);
                    break;

                case ReactionMechanicType.DelayedEcho:
                    ElementalReactionPulseEmitter.Spawn(
                        position,
                        owner,
                        payload,
                        primary,
                        damage,
                        spec.radius * 0.75f,
                        spec.radius,
                        spec.count,
                        0.32f,
                        spec.delay,
                        ReactionPulseMode.Normal);
                    break;

                case ReactionMechanicType.OrbitingNodes:
                    ElementalReactionOrbiters.Spawn(
                        position,
                        owner,
                        payload,
                        primary,
                        damage,
                        spec.radius,
                        spec.duration,
                        spec.count);
                    break;

                case ReactionMechanicType.ShardNova:
                    ElementalReactionShard.SpawnNova(
                        position + Vector3.up * 0.55f,
                        owner,
                        payload,
                        primary,
                        damage,
                        spec.radius,
                        spec.duration,
                        spec.count);
                    break;

                case ReactionMechanicType.Execute:
                    ExecuteLowHealth(
                        position,
                        owner,
                        spec.radius,
                        damage,
                        primary,
                        payload,
                        spec.magnitude);
                    break;

                case ReactionMechanicType.ThermalCycle:
                    ElementalReactionPulseEmitter.Spawn(
                        position,
                        owner,
                        payload,
                        primary,
                        damage,
                        spec.radius * 0.55f,
                        spec.radius,
                        spec.count,
                        Mathf.Max(0.13f, spec.duration / spec.count),
                        spec.delay,
                        ReactionPulseMode.Thermal);
                    break;

                case ReactionMechanicType.Rebound:
                    PushArea(position, owner, spec.radius, 18f + spec.magnitude * 18f);

                    ElementalReactionPulseEmitter.Spawn(
                        position,
                        owner,
                        payload,
                        primary,
                        damage * 0.75f,
                        spec.radius,
                        spec.radius * 0.55f,
                        Mathf.Max(1, spec.count),
                        0.25f,
                        spec.delay,
                        ReactionPulseMode.Pull);
                    break;

                case ReactionMechanicType.FieldSurge:
                    ElementalReactionField.SurgeNear(
                        position,
                        spec.radius,
                        payload,
                        Mathf.Max(1f, spec.magnitude),
                        spec.duration);
                    break;

                case ReactionMechanicType.TrailLine:
                    SpawnTrailLine(
                        position,
                        payload,
                        primary,
                        damage,
                        spec,
                        definition.signature,
                        mechanicIndex);
                    break;

                case ReactionMechanicType.Stagger:
                    PushArea(position, owner, spec.radius, 8f + spec.magnitude * 16f);
                    VulnerabilityArea(
                        position,
                        owner,
                        spec.radius,
                        Mathf.Clamp(0.08f + spec.magnitude * 0.12f, 0.08f, 0.32f),
                        Mathf.Max(0.6f, spec.duration));
                    break;

                case ReactionMechanicType.DetonateBuildup:
                    float consumed = controller == null
                        ? 0f
                        : controller.ConsumeBuildupForMechanic(
                            payload,
                            Mathf.Clamp(0.18f + spec.magnitude * 0.18f, 0.2f, 0.72f));

                    float detonationDamage =
                        damage +
                        owner.MaxHealth * consumed * 0.0018f;

                    DamageOwner(
                        owner,
                        detonationDamage,
                        primary,
                        payload,
                        death);

                    DamageArea(
                        position,
                        owner,
                        spec.radius,
                        detonationDamage * 0.68f,
                        primary,
                        payload,
                        0.2f,
                        death);
                    break;

                case ReactionMechanicType.Contagion:
                    Contagion(
                        position,
                        owner,
                        spec.radius,
                        payload,
                        spec.magnitude,
                        spec.duration,
                        spec.count,
                        damage,
                        primary);
                    break;

                case ReactionMechanicType.Compression:
                    PullArea(position, owner, spec.radius, 7f + spec.magnitude * 10f);
                    VulnerabilityArea(
                        position,
                        owner,
                        spec.radius,
                        Mathf.Clamp(0.1f + spec.magnitude * 0.1f, 0.1f, 0.35f),
                        Mathf.Max(0.8f, spec.duration));

                    ElementalReactionPulseEmitter.Spawn(
                        position,
                        owner,
                        payload,
                        primary,
                        damage,
                        spec.radius * 0.4f,
                        spec.radius,
                        Mathf.Max(1, spec.count),
                        0.24f,
                        spec.delay,
                        ReactionPulseMode.Push);
                    break;
            }
        }

        public static void DamageArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float damage,
            ReactionElement element,
            ReactionElement payload,
            float buildup,
            bool critical)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(
                    position,
                    radius,
                    source);

            Color color = ElementalReactionCodex.BlendColor(payload);

            foreach (EnemyController enemy in targets)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                float falloff = Mathf.Clamp01(1f - distance / Mathf.Max(0.1f, radius));

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    damage * Mathf.Lerp(0.42f, 1f, falloff),
                    element,
                    color,
                    critical);

                ApplyPayload(
                    enemy,
                    payload,
                    buildup * Mathf.Lerp(0.55f, 1f, falloff),
                    4.5f,
                    true);
            }
        }

        public static void Chain(
            Vector3 start,
            EnemyController source,
            float radius,
            int count,
            float damage,
            ReactionElement element,
            ReactionElement payload)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(start, radius, source);

            Vector3 from = start + Vector3.up * 0.65f;
            int limit = Mathf.Min(count, targets.Count);
            Color color = ElementalReactionCodex.BlendColor(payload);

            for (int i = 0; i < limit; i++)
            {
                EnemyController target = targets[i];
                Vector3 to = target.DamagePoint;

                RuntimeVisuals.Beam(from, to, color, 0.14f);

                ElementalReactionRuntime.DealReactionDamage(
                    target,
                    damage * Mathf.Pow(0.84f, i),
                    element,
                    color,
                    false);

                ApplyPayload(
                    target,
                    payload,
                    Mathf.Lerp(0.9f, 0.35f, i / Mathf.Max(1f, limit - 1f)),
                    4f,
                    true);

                from = to;
            }
        }

        public static void PullArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float strength)
        {
            foreach (
                EnemyController enemy
                in ElementalReactionRuntime.EnemiesWithin(position, radius, source))
            {
                enemy.PullToward(position, strength);
            }
        }

        public static void PushArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float force)
        {
            foreach (
                EnemyController enemy
                in ElementalReactionRuntime.EnemiesWithin(position, radius, source))
            {
                Vector3 direction = enemy.transform.position - position;
                enemy.ApplyImpact(direction, force);
            }
        }

        public static void FreezeArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float duration)
        {
            foreach (
                EnemyController enemy
                in ElementalReactionRuntime.EnemiesWithin(position, radius, source))
            {
                enemy.ApplyFreeze(enemy.IsBoss ? duration * 0.28f : duration);
                ElementalReactionRuntime.ApplyBuildup(
                    enemy,
                    ReactionElement.Cold,
                    0.8f,
                    4f,
                    true);
            }
        }

        public static void VulnerabilityArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float magnitude,
            float duration)
        {
            foreach (
                EnemyController enemy
                in ElementalReactionRuntime.EnemiesWithin(position, radius, source))
            {
                enemy.ApplyVulnerability(magnitude, duration);
            }
        }

        public static void ApplyPayload(
            EnemyController enemy,
            ReactionElement payload,
            float amount,
            float duration,
            bool propagated)
        {
            int count = Mathf.Max(1, ElementalReactionCodex.CountBits(payload));
            float perElement = amount / Mathf.Sqrt(count);

            foreach (
                ReactionElement element
                in ElementalReactionCodex.Enumerate(payload))
            {
                ElementalReactionRuntime.ApplyBuildup(
                    enemy,
                    element,
                    perElement,
                    duration,
                    propagated);
            }
        }

        private static void ApplyPayloadArea(
            Vector3 position,
            EnemyController source,
            float radius,
            ReactionElement payload,
            float amount,
            float duration,
            bool propagated)
        {
            foreach (
                EnemyController enemy
                in ElementalReactionRuntime.EnemiesWithin(position, radius, source))
            {
                ApplyPayload(enemy, payload, amount, duration, propagated);
            }
        }

        private static void DamageOwner(
            EnemyController owner,
            float damage,
            ReactionElement element,
            ReactionElement payload,
            bool critical)
        {
            if (owner == null ||
                owner.IsDead ||
                owner.Health <= 0f)
            {
                return;
            }

            ElementalReactionRuntime.DealReactionDamage(
                owner,
                damage,
                element,
                ElementalReactionCodex.BlendColor(payload),
                critical);
        }

        private static void SpawnSplitFields(
            Vector3 position,
            ReactionElement payload,
            float damage,
            float radius,
            float duration,
            int count,
            float magnitude,
            ReactionElement seedSignature)
        {
            int fieldCount = Mathf.Clamp(count, 2, 12);
            float orbit = Mathf.Max(1f, radius * 0.55f);
            float fieldRadius = Mathf.Max(0.8f, radius * 0.32f);
            float offset = ((int)seedSignature % 360) * Mathf.Deg2Rad;

            for (int i = 0; i < fieldCount; i++)
            {
                float angle = offset + Mathf.PI * 2f * i / fieldCount;
                Vector3 point = position + new Vector3(
                    Mathf.Cos(angle) * orbit,
                    0f,
                    Mathf.Sin(angle) * orbit);

                ElementalReactionRuntime.SpawnField(
                    payload,
                    point,
                    fieldRadius,
                    duration,
                    damage * 0.12f,
                    Mathf.Max(0.12f, magnitude * 0.32f));
            }
        }

        private static void ExecuteLowHealth(
            Vector3 position,
            EnemyController source,
            float radius,
            float damage,
            ReactionElement element,
            ReactionElement payload,
            float magnitude)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);

            if (source != null &&
                !source.IsDead &&
                source.Health > 0f)
            {
                targets.Insert(0, source);
            }

            Color color = ElementalReactionCodex.BlendColor(payload);

            foreach (EnemyController enemy in targets)
            {
                float missing = 1f - enemy.HealthRatio;
                float executeScale = 1f + missing * (1.5f + magnitude);

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    damage * executeScale,
                    element,
                    color,
                    enemy.HealthRatio <= 0.22f);
            }
        }

        private static void Contagion(
            Vector3 position,
            EnemyController source,
            float radius,
            ReactionElement payload,
            float amount,
            float duration,
            int count,
            float damage,
            ReactionElement element)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);

            int limit = Mathf.Min(count, targets.Count);

            for (int i = 0; i < limit; i++)
            {
                EnemyController enemy = targets[i];
                float scale = Mathf.Pow(0.86f, i);

                ApplyPayload(
                    enemy,
                    payload,
                    amount * scale,
                    duration,
                    true);

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    damage * 0.35f * scale,
                    element,
                    ElementalReactionCodex.BlendColor(payload),
                    false);
            }
        }

        private static void SpawnTrailLine(
            Vector3 position,
            ReactionElement payload,
            ReactionElement primary,
            float damage,
            ReactionMechanicSpec spec,
            ReactionElement seedSignature,
            int mechanicIndex)
        {
            int count = Mathf.Clamp(spec.count, 2, 12);
            float angle =
                (((int)seedSignature * 37 + mechanicIndex * 53) % 360) *
                Mathf.Deg2Rad;

            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            float spacing = Mathf.Max(0.65f, spec.radius / count);

            for (int i = 0; i < count; i++)
            {
                float centered = i - (count - 1) * 0.5f;
                Vector3 point = position + direction * centered * spacing;

                ElementalReactionRuntime.SpawnField(
                    payload,
                    point,
                    Mathf.Max(0.7f, spacing * 0.85f),
                    spec.duration,
                    damage * 0.1f,
                    Mathf.Max(0.12f, spec.magnitude * 0.28f));
            }
        }

        private static void Fallback(
            ElementalReactionDefinition definition,
            EnemyController owner,
            float baseDamage,
            bool death)
        {
            DamageOwner(
                owner,
                baseDamage * definition.damageMultiplier,
                definition.catalyst,
                definition.signature,
                death);

            DamageArea(
                owner.transform.position,
                owner,
                definition.radius,
                baseDamage,
                definition.catalyst,
                definition.signature,
                definition.spreadStacks * 0.25f,
                death);
        }
    }
}
