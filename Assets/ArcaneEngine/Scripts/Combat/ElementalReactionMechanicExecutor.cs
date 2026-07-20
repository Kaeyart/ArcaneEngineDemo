using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public static class ElementalReactionMechanicExecutor
    {
        private sealed class Candidate22
        {
            public ReactionMechanicSpec spec;
            public int firstIndex;
            public float score;
        }

        public static void Execute(
            ElementalReactionDefinition definition,
            ElementalReactionController controller,
            EnemyController owner,
            float baseDamage,
            bool death)
        {
            ReactionContext22 context = controller == null
                ? ReactionContext22.Legacy(true)
                : controller.ContextForSignature(definition == null
                    ? ReactionElement.None
                    : definition.signature)
                    .Derive(death
                        ? ReactionSourceKind22.DeathReaction
                        : ReactionSourceKind22.PrimaryReaction);

            Execute(definition, controller, owner, baseDamage, death, context);
        }

        public static void Execute(
            ElementalReactionDefinition definition,
            ElementalReactionController controller,
            EnemyController owner,
            float baseDamage,
            bool death,
            ReactionContext22 context)
        {
            if (definition == null || owner == null)
                return;

            ReactionMechanicPlan plan =
                ElementalReactionMechanicCodex.Get(definition.signature);

            if (plan == null)
            {
                Fallback(definition, owner, baseDamage, death, context);
                return;
            }

            ReactionMechanicSpec[] source = death ? plan.death : plan.resolve;
            List<ReactionMechanicSpec> selected =
                SelectMechanics(source, definition, death, context);

            if (selected.Count == 0)
            {
                Fallback(definition, owner, baseDamage, death, context);
                return;
            }

            bool staged =
                (int)definition.tier >= (int)ReactionTier.Catastrophe ||
                selected.Count > 4;

            if (staged)
            {
                ReactionMechanicStageRunner22.Spawn(
                    owner.transform.position,
                    definition,
                    controller,
                    owner,
                    baseDamage,
                    death,
                    context,
                    selected);
                return;
            }

            for (int i = 0; i < selected.Count; i++)
            {
                ExecuteSelectedSpec(
                    selected[i],
                    definition,
                    controller,
                    owner,
                    owner.transform.position,
                    baseDamage,
                    death,
                    context,
                    i);
            }
        }

        private static List<ReactionMechanicSpec> SelectMechanics(
            ReactionMechanicSpec[] mechanics,
            ElementalReactionDefinition definition,
            bool death,
            ReactionContext22 context)
        {
            int budget = ReactionBalance22.ResolveMechanicBudget(definition.tier, death);
            List<Candidate22> consolidated = Consolidate(mechanics, definition);
            List<ReactionMechanicSpec> selected = new List<ReactionMechanicSpec>();
            HashSet<ReactionMechanicType> chosen = new HashSet<ReactionMechanicType>();
            bool fieldChosen = false;
            bool controlChosen = false;

            AddBest(consolidated, selected, chosen, IsPrimaryDamage, ref fieldChosen, ref controlChosen, budget);
            AddBest(consolidated, selected, chosen,
                c => c.spec.element == definition.catalyst ||
                     ElementalReactionCodex.Contains(c.spec.payload, definition.catalyst),
                ref fieldChosen, ref controlChosen, budget);
            AddBest(consolidated, selected, chosen, IsPropagation, ref fieldChosen, ref controlChosen, budget);
            AddBest(consolidated, selected, chosen, IsField, ref fieldChosen, ref controlChosen, budget);
            AddBest(consolidated, selected, chosen, IsControl, ref fieldChosen, ref controlChosen, budget);
            AddBest(consolidated, selected, chosen, IsTermination, ref fieldChosen, ref controlChosen, budget);

            consolidated.Sort((a, b) => b.score.CompareTo(a.score));

            for (int i = 0; i < consolidated.Count && selected.Count < budget; i++)
            {
                Candidate22 candidate = consolidated[i];
                if (chosen.Contains(candidate.spec.type))
                    continue;

                if (IsField(candidate) && fieldChosen)
                    continue;

                if (IsControl(candidate) && controlChosen)
                    continue;

                selected.Add(candidate.spec);
                chosen.Add(candidate.spec.type);
                fieldChosen |= IsField(candidate);
                controlChosen |= IsControl(candidate);
            }

            selected.Sort((a, b) =>
            {
                int stage = StageOf(a.type).CompareTo(StageOf(b.type));
                if (stage != 0)
                    return stage;
                return ScoreType(b.type).CompareTo(ScoreType(a.type));
            });

            for (int i = 0; i < consolidated.Count; i++)
            {
                bool kept = chosen.Contains(consolidated[i].spec.type);
                ReactionDiagnostics22.RecordMechanic(kept, consolidated[i].spec.type, context);
            }

            return selected;
        }

        private static List<Candidate22> Consolidate(
            ReactionMechanicSpec[] mechanics,
            ElementalReactionDefinition definition)
        {
            Dictionary<ReactionMechanicType, List<KeyValuePair<int, ReactionMechanicSpec>>> groups =
                new Dictionary<ReactionMechanicType, List<KeyValuePair<int, ReactionMechanicSpec>>>();

            for (int i = 0; i < mechanics.Length; i++)
            {
                List<KeyValuePair<int, ReactionMechanicSpec>> group;
                if (!groups.TryGetValue(mechanics[i].type, out group))
                {
                    group = new List<KeyValuePair<int, ReactionMechanicSpec>>();
                    groups[mechanics[i].type] = group;
                }

                group.Add(new KeyValuePair<int, ReactionMechanicSpec>(i, mechanics[i]));
            }

            List<Candidate22> results = new List<Candidate22>();

            foreach (KeyValuePair<ReactionMechanicType, List<KeyValuePair<int, ReactionMechanicSpec>>> pair in groups)
            {
                List<KeyValuePair<int, ReactionMechanicSpec>> group = pair.Value;
                ReactionMechanicSpec strongest = group[0].Value;
                int firstIndex = group[0].Key;
                float magnitude = strongest.magnitude;
                float radius = strongest.radius;
                float duration = strongest.duration;
                float delay = strongest.delay;
                int count = strongest.count;
                ReactionElement payload = strongest.payload;
                ReactionElement element = strongest.element;

                for (int i = 1; i < group.Count; i++)
                {
                    ReactionMechanicSpec current = group[i].Value;
                    if (current.magnitude > strongest.magnitude)
                        strongest = current;

                    magnitude = Mathf.Max(magnitude, current.magnitude) +
                                Mathf.Min(0.12f, current.magnitude * 0.08f);
                    radius = Mathf.Max(radius, current.radius);
                    duration = Mathf.Max(duration, current.duration);
                    delay = Mathf.Min(delay, current.delay);
                    count += Mathf.Max(1, Mathf.RoundToInt(current.count * 0.25f));
                    payload |= current.payload;

                    if (current.element == definition.catalyst)
                        element = current.element;
                }

                magnitude = Mathf.Min(magnitude, Mathf.Max(0.25f, strongest.magnitude * 1.35f));
                count = CapCount(pair.Key, count);

                ReactionMechanicSpec merged = new ReactionMechanicSpec(
                    pair.Key,
                    element,
                    payload,
                    magnitude,
                    radius,
                    duration,
                    count,
                    delay);

                results.Add(new Candidate22
                {
                    spec = merged,
                    firstIndex = firstIndex,
                    score = Score(merged, definition, group.Count)
                });
            }

            return results;
        }

        private static void AddBest(
            List<Candidate22> candidates,
            List<ReactionMechanicSpec> selected,
            HashSet<ReactionMechanicType> chosen,
            Predicate<Candidate22> predicate,
            ref bool fieldChosen,
            ref bool controlChosen,
            int budget)
        {
            if (selected.Count >= budget)
                return;

            Candidate22 best = null;

            for (int i = 0; i < candidates.Count; i++)
            {
                Candidate22 candidate = candidates[i];
                if (chosen.Contains(candidate.spec.type) || !predicate(candidate))
                    continue;

                if (IsField(candidate) && fieldChosen)
                    continue;

                if (IsControl(candidate) && controlChosen)
                    continue;

                if (best == null || candidate.score > best.score)
                    best = candidate;
            }

            if (best == null)
                return;

            selected.Add(best.spec);
            chosen.Add(best.spec.type);
            fieldChosen |= IsField(best);
            controlChosen |= IsControl(best);
        }

        private static float Score(
            ReactionMechanicSpec spec,
            ElementalReactionDefinition definition,
            int duplicates)
        {
            float score = ScoreType(spec.type);
            score += Mathf.Clamp(spec.magnitude, 0f, 4f) * 0.8f;
            score += Mathf.Clamp(spec.radius, 0f, 10f) * 0.08f;
            score += Mathf.Min(0.8f, duplicates * 0.12f);

            if (spec.element == definition.catalyst ||
                ElementalReactionCodex.Contains(spec.payload, definition.catalyst))
            {
                score += 2.5f;
            }

            return score;
        }

        private static float ScoreType(ReactionMechanicType type)
        {
            switch (type)
            {
                case ReactionMechanicType.DetonateBuildup: return 10f;
                case ReactionMechanicType.Burst: return 9.5f;
                case ReactionMechanicType.ChainArc: return 9f;
                case ReactionMechanicType.PulseNova: return 8.8f;
                case ReactionMechanicType.ShardNova: return 8.3f;
                case ReactionMechanicType.Contagion: return 8f;
                case ReactionMechanicType.Execute: return 7.8f;
                case ReactionMechanicType.Field: return 7.4f;
                case ReactionMechanicType.SplitFields: return 7.2f;
                case ReactionMechanicType.ThermalCycle: return 7f;
                case ReactionMechanicType.BuildupWave: return 6.8f;
                case ReactionMechanicType.Compression: return 6.6f;
                case ReactionMechanicType.DelayedEcho: return 6.4f;
                case ReactionMechanicType.OrbitingNodes: return 6.2f;
                case ReactionMechanicType.FieldSurge: return 6f;
                case ReactionMechanicType.TrailLine: return 5.8f;
                case ReactionMechanicType.Freeze: return 5.6f;
                case ReactionMechanicType.Stagger: return 5.4f;
                case ReactionMechanicType.Rebound: return 5.2f;
                case ReactionMechanicType.Pull: return 5f;
                case ReactionMechanicType.Push: return 4.8f;
                case ReactionMechanicType.Vulnerability: return 4.6f;
                default: return 4f;
            }
        }

        private static int StageOf(ReactionMechanicType type)
        {
            if (type == ReactionMechanicType.DelayedEcho ||
                type == ReactionMechanicType.Execute ||
                type == ReactionMechanicType.DetonateBuildup)
                return 3;

            if (type == ReactionMechanicType.Field ||
                type == ReactionMechanicType.SplitFields ||
                type == ReactionMechanicType.FieldSurge ||
                type == ReactionMechanicType.TrailLine)
                return 2;

            if (type == ReactionMechanicType.ChainArc ||
                type == ReactionMechanicType.BuildupWave ||
                type == ReactionMechanicType.Contagion ||
                type == ReactionMechanicType.Pull ||
                type == ReactionMechanicType.Push ||
                type == ReactionMechanicType.Freeze ||
                type == ReactionMechanicType.Vulnerability ||
                type == ReactionMechanicType.Stagger ||
                type == ReactionMechanicType.Compression ||
                type == ReactionMechanicType.Rebound)
                return 1;

            return 0;
        }

        private static int CapCount(ReactionMechanicType type, int count)
        {
            switch (type)
            {
                case ReactionMechanicType.SplitFields: return Mathf.Clamp(count, 2, 4);
                case ReactionMechanicType.TrailLine: return Mathf.Clamp(count, 2, 5);
                case ReactionMechanicType.ChainArc: return Mathf.Clamp(count, 1, 5);
                case ReactionMechanicType.Contagion: return Mathf.Clamp(count, 1, 4);
                case ReactionMechanicType.ShardNova: return Mathf.Clamp(count, 3, 14);
                case ReactionMechanicType.OrbitingNodes: return Mathf.Clamp(count, 2, 6);
                case ReactionMechanicType.PulseNova:
                case ReactionMechanicType.ThermalCycle:
                case ReactionMechanicType.DelayedEcho:
                    return Mathf.Clamp(count, 1, 5);
                default: return Mathf.Clamp(count, 1, 8);
            }
        }

        private static bool IsPrimaryDamage(Candidate22 value)
        {
            switch (value.spec.type)
            {
                case ReactionMechanicType.Burst:
                case ReactionMechanicType.PulseNova:
                case ReactionMechanicType.ShardNova:
                case ReactionMechanicType.Execute:
                case ReactionMechanicType.ThermalCycle:
                case ReactionMechanicType.DetonateBuildup:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsPropagation(Candidate22 value)
        {
            return value.spec.type == ReactionMechanicType.ChainArc ||
                   value.spec.type == ReactionMechanicType.BuildupWave ||
                   value.spec.type == ReactionMechanicType.Contagion;
        }

        private static bool IsField(Candidate22 value)
        {
            return value.spec.type == ReactionMechanicType.Field ||
                   value.spec.type == ReactionMechanicType.SplitFields ||
                   value.spec.type == ReactionMechanicType.FieldSurge ||
                   value.spec.type == ReactionMechanicType.TrailLine;
        }

        private static bool IsControl(Candidate22 value)
        {
            return value.spec.type == ReactionMechanicType.Pull ||
                   value.spec.type == ReactionMechanicType.Push ||
                   value.spec.type == ReactionMechanicType.Freeze ||
                   value.spec.type == ReactionMechanicType.Vulnerability ||
                   value.spec.type == ReactionMechanicType.Rebound ||
                   value.spec.type == ReactionMechanicType.Stagger ||
                   value.spec.type == ReactionMechanicType.Compression;
        }

        private static bool IsTermination(Candidate22 value)
        {
            return value.spec.type == ReactionMechanicType.Execute ||
                   value.spec.type == ReactionMechanicType.DetonateBuildup ||
                   value.spec.type == ReactionMechanicType.DelayedEcho;
        }

        public static void ExecuteSelectedSpec(
            ReactionMechanicSpec spec,
            ElementalReactionDefinition definition,
            ElementalReactionController controller,
            EnemyController owner,
            Vector3 position,
            float baseDamage,
            bool death,
            ReactionContext22 context,
            int mechanicIndex)
        {
            ReactionElement primary =
                spec.element == ReactionElement.None
                    ? definition.catalyst
                    : spec.element;

            ReactionElement payload =
                spec.payload == ReactionElement.None
                    ? definition.signature
                    : spec.payload;

            float damage = Mathf.Max(0f, baseDamage * spec.magnitude);
            ReactionContext22 mechanicContext =
                context.AsSource(SourceFor(spec.type));

            ReactionPresentation22.EmitMechanic(
                spec,
                definition,
                owner,
                position,
                baseDamage,
                death,
                mechanicIndex,
                mechanicContext);

            switch (spec.type)
            {
                case ReactionMechanicType.Burst:
                    DamageOwner(owner, damage, primary, payload, death, mechanicContext);
                    DamageArea(
                        position,
                        owner,
                        spec.radius,
                        damage * 0.72f,
                        primary,
                        payload,
                        0.22f + definition.ElementCount * 0.05f,
                        death && (int)definition.tier >= (int)ReactionTier.Convergence,
                        mechanicContext,
                        Mathf.Clamp(2 + definition.ElementCount, 3, 8));
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
                        Mathf.Max(0.18f, spec.duration / spec.count),
                        spec.delay,
                        ReactionPulseMode.Normal,
                        mechanicContext);
                    break;

                case ReactionMechanicType.ChainArc:
                    int chainLimit = (int)definition.tier >= (int)ReactionTier.Calamity
                        ? Mathf.Min(5, spec.count)
                        : Mathf.Min(ReactionBalance22.MaximumChainTargets, spec.count);
                    Chain(
                        position,
                        owner,
                        spec.radius,
                        chainLimit,
                        damage,
                        primary,
                        payload,
                        mechanicContext);
                    break;

                case ReactionMechanicType.Pull:
                    PullArea(position, owner, spec.radius,
                        Mathf.Min(8f, 2.5f + spec.magnitude * 4f), mechanicContext, 4);
                    break;

                case ReactionMechanicType.Push:
                    PushArea(position, owner, spec.radius,
                        Mathf.Min(18f, 8f + spec.magnitude * 10f), mechanicContext, 4);
                    break;

                case ReactionMechanicType.Freeze:
                    FreezeArea(position, owner, spec.radius, spec.duration, mechanicContext, 5);
                    break;

                case ReactionMechanicType.Vulnerability:
                    VulnerabilityArea(
                        position,
                        owner,
                        spec.radius,
                        Mathf.Clamp(spec.magnitude, 0.05f, 0.30f),
                        spec.duration,
                        6);
                    break;

                case ReactionMechanicType.BuildupWave:
                    ApplyPayloadArea(
                        position,
                        owner,
                        spec.radius,
                        payload,
                        spec.magnitude,
                        spec.duration,
                        mechanicContext,
                        Mathf.Clamp(2 + definition.ElementCount, 3, 7));
                    break;

                case ReactionMechanicType.Field:
                    SpawnFieldForMechanic(
                        payload, position, spec.radius, spec.duration,
                        damage * 0.18f,
                        Mathf.Max(0.12f, spec.magnitude * 0.35f),
                        death, mechanicContext);
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
                        definition.signature,
                        death,
                        mechanicContext);
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
                        ReactionPulseMode.Normal,
                        mechanicContext.Derive(ReactionSourceKind22.Echo));
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
                        spec.count,
                        mechanicContext);
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
                        spec.count,
                        mechanicContext);
                    break;

                case ReactionMechanicType.Execute:
                    ExecuteLowHealth(
                        position,
                        owner,
                        spec.radius,
                        damage,
                        primary,
                        payload,
                        spec.magnitude,
                        mechanicContext);
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
                        Mathf.Max(0.18f, spec.duration / spec.count),
                        spec.delay,
                        ReactionPulseMode.Thermal,
                        mechanicContext);
                    break;

                case ReactionMechanicType.Rebound:
                    PushArea(position, owner, spec.radius,
                        Mathf.Min(16f, 10f + spec.magnitude * 8f), mechanicContext, 4);
                    ElementalReactionPulseEmitter.Spawn(
                        position,
                        owner,
                        payload,
                        primary,
                        damage * 0.75f,
                        spec.radius,
                        spec.radius * 0.55f,
                        Mathf.Max(1, spec.count),
                        0.28f,
                        spec.delay,
                        ReactionPulseMode.Pull,
                        mechanicContext.Derive(ReactionSourceKind22.Echo));
                    break;

                case ReactionMechanicType.FieldSurge:
                    ElementalReactionField.SurgeNear(
                        position,
                        spec.radius,
                        payload,
                        Mathf.Clamp(spec.magnitude, 1f, 1.15f),
                        Mathf.Max(0.5f, spec.duration),
                        mechanicContext);
                    break;

                case ReactionMechanicType.TrailLine:
                    SpawnTrailLine(
                        position,
                        payload,
                        primary,
                        damage,
                        spec,
                        definition.signature,
                        mechanicIndex,
                        death,
                        mechanicContext);
                    break;

                case ReactionMechanicType.Stagger:
                    PushArea(position, owner, spec.radius,
                        Mathf.Min(12f, 5f + spec.magnitude * 7f), mechanicContext, 4);
                    VulnerabilityArea(
                        position,
                        owner,
                        spec.radius,
                        Mathf.Clamp(0.06f + spec.magnitude * 0.08f, 0.06f, 0.22f),
                        Mathf.Max(0.6f, spec.duration),
                        5);
                    break;

                case ReactionMechanicType.DetonateBuildup:
                    float consumed = controller == null
                        ? 0f
                        : controller.ConsumeBuildupForMechanic(
                            payload,
                            Mathf.Clamp(0.18f + spec.magnitude * 0.14f, 0.2f, 0.58f));

                    float detonationDamage = damage +
                        (owner == null ? 0f : owner.MaxHealth * consumed * 0.0013f);

                    DamageOwner(owner, detonationDamage, primary, payload, death, mechanicContext);
                    DamageArea(
                        position,
                        owner,
                        spec.radius,
                        detonationDamage * 0.58f,
                        primary,
                        payload,
                        0.12f,
                        death,
                        mechanicContext,
                        Mathf.Clamp(2 + definition.ElementCount, 3, 7));
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
                        primary,
                        mechanicContext);
                    break;

                case ReactionMechanicType.Compression:
                    PullArea(position, owner, spec.radius,
                        Mathf.Min(9f, 4f + spec.magnitude * 5f), mechanicContext, 4);
                    VulnerabilityArea(
                        position,
                        owner,
                        spec.radius,
                        Mathf.Clamp(0.08f + spec.magnitude * 0.07f, 0.08f, 0.24f),
                        Mathf.Max(0.8f, spec.duration),
                        5);
                    ElementalReactionPulseEmitter.Spawn(
                        position,
                        owner,
                        payload,
                        primary,
                        damage,
                        spec.radius * 0.4f,
                        spec.radius,
                        Mathf.Max(1, spec.count),
                        0.28f,
                        spec.delay,
                        ReactionPulseMode.Push,
                        mechanicContext.Derive(ReactionSourceKind22.Echo));
                    break;
            }
        }

        private static ReactionSourceKind22 SourceFor(ReactionMechanicType type)
        {
            switch (type)
            {
                case ReactionMechanicType.ChainArc: return ReactionSourceKind22.Chain;
                case ReactionMechanicType.Field:
                case ReactionMechanicType.SplitFields:
                case ReactionMechanicType.FieldSurge:
                case ReactionMechanicType.TrailLine:
                    return ReactionSourceKind22.Field;
                case ReactionMechanicType.DelayedEcho:
                case ReactionMechanicType.PulseNova:
                case ReactionMechanicType.ThermalCycle:
                    return ReactionSourceKind22.Echo;
                default:
                    return ReactionSourceKind22.PrimaryReaction;
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
            DamageArea(
                position, source, radius, damage, element, payload, buildup,
                critical, ReactionContext22.Legacy(true), 8);
        }

        public static void DamageArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float damage,
            ReactionElement element,
            ReactionElement payload,
            float buildup,
            bool critical,
            ReactionContext22 context,
            int maxTargets)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);

            Color color = ElementalReactionCodex.BlendColor(payload);
            int limit = Mathf.Min(Mathf.Max(1, maxTargets), targets.Count);

            for (int i = 0; i < limit; i++)
            {
                EnemyController enemy = targets[i];
                float distance = Vector3.Distance(position, enemy.transform.position);
                float falloff = Mathf.Clamp01(1f - distance / Mathf.Max(0.1f, radius));

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    damage * Mathf.Lerp(0.42f, 1f, falloff),
                    element,
                    color,
                    critical,
                    context);

                ApplyPayload(
                    enemy,
                    payload,
                    buildup * Mathf.Lerp(0.55f, 1f, falloff),
                    4.5f,
                    context);
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
            Chain(
                start, source, radius, count, damage, element, payload,
                ReactionContext22.Legacy(true));
        }

        public static void Chain(
            Vector3 start,
            EnemyController source,
            float radius,
            int count,
            float damage,
            ReactionElement element,
            ReactionElement payload,
            ReactionContext22 context)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(start, radius, source);

            Vector3 from = start + Vector3.up * 0.65f;
            int limit = Mathf.Min(Mathf.Max(1, count), targets.Count);
            Color color = ElementalReactionCodex.BlendColor(payload);
            int jump = 0;

            for (int i = 0; i < targets.Count && jump < limit; i++)
            {
                EnemyController target = targets[i];
                if (!ReactionLineageRegistry22.TryMarkTarget(context, target, 0.35f))
                    continue;

                Vector3 to = target.DamagePoint;
                RuntimeVisuals.Beam(
                    from,
                    to,
                    color,
                    0.11f * ReactionBalance22.ChainVisualIntensity(jump));

                ReactionContext22 jumpContext = context.WithCoefficients(
                    ReactionBalance22.ChainDamageCoefficient(jump),
                    1f,
                    1f);

                ElementalReactionRuntime.DealReactionDamage(
                    target,
                    damage,
                    element,
                    color,
                    false,
                    jumpContext);

                ApplyPayload(
                    target,
                    payload,
                    ReactionBalance22.ChainBuildupAmount(jump),
                    4f,
                    jumpContext);

                ReactionPresentation22.EmitChainContact(
                    from, to, payload, jump, jumpContext);

                from = to;
                jump++;
            }
        }

        public static void PullArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float strength)
        {
            PullArea(position, source, radius, strength, ReactionContext22.Legacy(true), 4);
        }

        public static void PullArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float strength,
            ReactionContext22 context,
            int maxTargets)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);
            int limit = Mathf.Min(maxTargets, targets.Count);

            for (int i = 0; i < limit; i++)
                targets[i].PullToward(position, Mathf.Min(10f, strength));
        }

        public static void PushArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float force)
        {
            PushArea(position, source, radius, force, ReactionContext22.Legacy(true), 4);
        }

        public static void PushArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float force,
            ReactionContext22 context,
            int maxTargets)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);
            int limit = Mathf.Min(maxTargets, targets.Count);

            for (int i = 0; i < limit; i++)
            {
                EnemyController enemy = targets[i];
                Vector3 direction = enemy.transform.position - position;
                using (ElementalReactionRuntime.UseContext(context))
                    enemy.ApplyImpact(direction, Mathf.Min(20f, force));
            }
        }

        public static void FreezeArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float duration)
        {
            FreezeArea(position, source, radius, duration, ReactionContext22.Legacy(true), 5);
        }

        public static void FreezeArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float duration,
            ReactionContext22 context,
            int maxTargets)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);
            int limit = Mathf.Min(maxTargets, targets.Count);

            for (int i = 0; i < limit; i++)
            {
                ElementalReactionRuntime.ApplyBuildup(
                    targets[i],
                    ReactionElement.Cold,
                    Mathf.Clamp(0.55f + duration * 0.12f, 0.55f, 0.9f),
                    4f,
                    context);
            }
        }

        public static void VulnerabilityArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float magnitude,
            float duration)
        {
            VulnerabilityArea(position, source, radius, magnitude, duration, 6);
        }

        public static void VulnerabilityArea(
            Vector3 position,
            EnemyController source,
            float radius,
            float magnitude,
            float duration,
            int maxTargets)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);
            int limit = Mathf.Min(maxTargets, targets.Count);

            for (int i = 0; i < limit; i++)
                targets[i].ApplyVulnerability(magnitude, duration);
        }

        public static void ApplyPayload(
            EnemyController enemy,
            ReactionElement payload,
            float amount,
            float duration,
            bool propagated)
        {
            ApplyPayload(
                enemy,
                payload,
                amount,
                duration,
                ReactionContext22.Legacy(propagated));
        }

        public static void ApplyPayload(
            EnemyController enemy,
            ReactionElement payload,
            float amount,
            float duration,
            ReactionContext22 context)
        {
            int count = Mathf.Max(1, ElementalReactionCodex.CountBits(payload));
            float perElement = amount / Mathf.Sqrt(count);

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(payload))
            {
                ElementalReactionRuntime.ApplyBuildup(
                    enemy,
                    element,
                    perElement,
                    duration,
                    context);
            }
        }

        private static void ApplyPayloadArea(
            Vector3 position,
            EnemyController source,
            float radius,
            ReactionElement payload,
            float amount,
            float duration,
            ReactionContext22 context,
            int maxTargets)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);
            int limit = Mathf.Min(maxTargets, targets.Count);

            for (int i = 0; i < limit; i++)
                ApplyPayload(targets[i], payload, amount, duration, context);
        }

        private static void DamageOwner(
            EnemyController owner,
            float damage,
            ReactionElement element,
            ReactionElement payload,
            bool critical,
            ReactionContext22 context)
        {
            if (owner == null || owner.IsDead || owner.Health <= 0f)
                return;

            ElementalReactionRuntime.DealReactionDamage(
                owner,
                damage,
                element,
                ElementalReactionCodex.BlendColor(payload),
                critical,
                context);
        }

        private static void SpawnFieldForMechanic(
            ReactionElement payload,
            Vector3 position,
            float radius,
            float duration,
            float damage,
            float buildup,
            bool death,
            ReactionContext22 context)
        {
            ReactionFieldAuthority22 authority =
                context.generation >= 2
                    ? ReactionFieldAuthority22.SecondaryPropagation
                    : death
                        ? ReactionFieldAuthority22.DeathResidue
                        : ReactionFieldAuthority22.PrimaryReaction;

            ElementalReactionRuntime.SpawnField(
                payload,
                position,
                radius,
                duration,
                damage,
                buildup,
                authority,
                context.WithoutFieldCreation(),
                EntityId.None); // ARCANE_PATCH_221_ENTITY_ID
        }

        private static void SpawnSplitFields(
            Vector3 position,
            ReactionElement payload,
            float damage,
            float radius,
            float duration,
            int count,
            float magnitude,
            ReactionElement seedSignature,
            bool death,
            ReactionContext22 context)
        {
            if (!context.canCreateField && context.generation >= 2)
            {
                ReactionDiagnostics22.RecordBlocked("split-fields", context);
                return;
            }

            int fieldCount = Mathf.Clamp(count, 2, 4);
            float orbit = Mathf.Max(1f, radius * 0.5f);
            float fieldRadius = Mathf.Max(0.75f, radius * 0.28f);
            float offset = ((int)seedSignature % 360) * Mathf.Deg2Rad;
            float sharedDamage = damage * 0.20f / fieldCount;
            float sharedBuildup = Mathf.Max(0.08f, magnitude * 0.30f) / Mathf.Sqrt(fieldCount);

            for (int i = 0; i < fieldCount; i++)
            {
                float angle = offset + Mathf.PI * 2f * i / fieldCount;
                Vector3 point = position + new Vector3(
                    Mathf.Cos(angle) * orbit,
                    0f,
                    Mathf.Sin(angle) * orbit);

                SpawnFieldForMechanic(
                    payload,
                    point,
                    fieldRadius,
                    duration,
                    sharedDamage,
                    sharedBuildup,
                    death,
                    context);
            }
        }

        private static void ExecuteLowHealth(
            Vector3 position,
            EnemyController source,
            float radius,
            float damage,
            ReactionElement element,
            ReactionElement payload,
            float magnitude,
            ReactionContext22 context)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);

            if (source != null && !source.IsDead && source.Health > 0f)
                targets.Insert(0, source);

            Color color = ElementalReactionCodex.BlendColor(payload);
            int limit = Mathf.Min(6, targets.Count);

            for (int i = 0; i < limit; i++)
            {
                EnemyController enemy = targets[i];
                float missing = 1f - enemy.HealthRatio;
                float executeScale = 1f + missing * (1.1f + magnitude * 0.5f);

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    damage * executeScale,
                    element,
                    color,
                    enemy.HealthRatio <= 0.18f,
                    context);
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
            ReactionElement element,
            ReactionContext22 context)
        {
            List<EnemyController> targets =
                ElementalReactionRuntime.EnemiesWithin(position, radius, source);

            int limit = Mathf.Min(Mathf.Clamp(count, 1, 4), targets.Count);
            int affected = 0;

            for (int i = 0; i < targets.Count && affected < limit; i++)
            {
                EnemyController enemy = targets[i];
                if (!ReactionLineageRegistry22.TryMarkTarget(context, enemy, 0.35f))
                    continue;

                float scale = Mathf.Pow(0.72f, affected);

                ApplyPayload(
                    enemy,
                    payload,
                    amount * scale,
                    duration,
                    context);

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    damage * 0.28f * scale,
                    element,
                    ElementalReactionCodex.BlendColor(payload),
                    false,
                    context);

                affected++;
            }
        }

        private static void SpawnTrailLine(
            Vector3 position,
            ReactionElement payload,
            ReactionElement primary,
            float damage,
            ReactionMechanicSpec spec,
            ReactionElement seedSignature,
            int mechanicIndex,
            bool death,
            ReactionContext22 context)
        {
            if (!context.canCreateField && context.generation >= 2)
            {
                ReactionDiagnostics22.RecordBlocked("trail-field", context);
                return;
            }

            int count = Mathf.Clamp(spec.count, 2, 5);
            float angle =
                (((int)seedSignature * 37 + mechanicIndex * 53) % 360) *
                Mathf.Deg2Rad;

            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            float spacing = Mathf.Max(0.65f, spec.radius / count);
            float sharedDamage = damage * 0.16f / count;
            float sharedBuildup = Mathf.Max(0.06f, spec.magnitude * 0.22f) / Mathf.Sqrt(count);

            for (int i = 0; i < count; i++)
            {
                float centered = i - (count - 1) * 0.5f;
                Vector3 point = position + direction * centered * spacing;

                SpawnFieldForMechanic(
                    payload,
                    point,
                    Mathf.Max(0.65f, spacing * 0.72f),
                    spec.duration,
                    sharedDamage,
                    sharedBuildup,
                    death,
                    context);
            }
        }

        private static void Fallback(
            ElementalReactionDefinition definition,
            EnemyController owner,
            float baseDamage,
            bool death,
            ReactionContext22 context)
        {
            DamageOwner(
                owner,
                baseDamage * definition.damageMultiplier * 0.65f,
                definition.catalyst,
                definition.signature,
                death,
                context);

            DamageArea(
                owner.transform.position,
                owner,
                definition.radius,
                baseDamage * 0.55f,
                definition.catalyst,
                definition.signature,
                definition.spreadStacks * 0.12f,
                death,
                context,
                Mathf.Clamp(2 + definition.ElementCount, 3, 7));
        }
    }

    public sealed class ReactionMechanicStageRunner22 : MonoBehaviour
    {
        private sealed class Entry22
        {
            public ReactionMechanicSpec spec;
            public float executeAt;
            public int index;
        }

        private readonly List<Entry22> _entries = new List<Entry22>();
        private ElementalReactionDefinition _definition;
        private ElementalReactionController _controller;
        private EnemyController _owner;
        private float _baseDamage;
        private bool _death;
        private ReactionContext22 _context;
        private int _next;

        public static void Spawn(
            Vector3 position,
            ElementalReactionDefinition definition,
            ElementalReactionController controller,
            EnemyController owner,
            float baseDamage,
            bool death,
            ReactionContext22 context,
            List<ReactionMechanicSpec> mechanics)
        {
            GameObject host = new GameObject(
                "Reaction Stages " + definition.displayName);
            host.transform.position = position;

            ReactionMechanicStageRunner22 runner =
                host.AddComponent<ReactionMechanicStageRunner22>();

            runner._definition = definition;
            runner._controller = controller;
            runner._owner = owner;
            runner._baseDamage = baseDamage;
            runner._death = death;
            runner._context = context;

            float now = Time.time;
            int previousStage = -1;
            float stageTime = now;

            for (int i = 0; i < mechanics.Count; i++)
            {
                int stage = StageFor(mechanics[i].type);
                if (stage != previousStage)
                {
                    stageTime = now + stage * 0.20f;
                    previousStage = stage;
                }

                runner._entries.Add(new Entry22
                {
                    spec = mechanics[i],
                    executeAt = stageTime + Mathf.Min(0.25f, mechanics[i].delay),
                    index = i
                });
            }
        }

        private void Update()
        {
            while (_next < _entries.Count && Time.time >= _entries[_next].executeAt)
            {
                Entry22 entry = _entries[_next];
                ElementalReactionMechanicExecutor.ExecuteSelectedSpec(
                    entry.spec,
                    _definition,
                    _controller,
                    _owner,
                    transform.position,
                    _baseDamage,
                    _death,
                    _context,
                    entry.index);
                _next++;
            }

            if (_next >= _entries.Count)
                Destroy(gameObject);
        }

        private static int StageFor(ReactionMechanicType type)
        {
            if (type == ReactionMechanicType.Field ||
                type == ReactionMechanicType.SplitFields ||
                type == ReactionMechanicType.FieldSurge ||
                type == ReactionMechanicType.TrailLine)
                return 2;

            if (type == ReactionMechanicType.DelayedEcho ||
                type == ReactionMechanicType.Execute ||
                type == ReactionMechanicType.DetonateBuildup)
                return 3;

            if (type == ReactionMechanicType.ChainArc ||
                type == ReactionMechanicType.BuildupWave ||
                type == ReactionMechanicType.Contagion ||
                type == ReactionMechanicType.Pull ||
                type == ReactionMechanicType.Push ||
                type == ReactionMechanicType.Freeze ||
                type == ReactionMechanicType.Vulnerability ||
                type == ReactionMechanicType.Stagger ||
                type == ReactionMechanicType.Compression ||
                type == ReactionMechanicType.Rebound)
                return 1;

            return 0;
        }
    }
}
