using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public static class ReactionPresentation22
    {
        private static readonly Queue<float> MajorBurstTimes = new Queue<float>();
        private static readonly Queue<float> AnyBurstTimes = new Queue<float>();

        public static void EmitReactionResolved(
            EnemyController owner,
            ElementalReactionDefinition definition,
            float damage,
            bool death,
            ReactionContext22 context)
        {
            if (definition == null)
                return;

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = death
                    ? SpellPresentationEventType.ReactionDeathTriggered
                    : SpellPresentationEventType.ReactionResolved,
                priority = PriorityFor(definition.tier, context),
                source = owner == null ? null : owner.transform,
                target = owner == null ? null : owner.transform,
                host = owner == null ? null : owner.gameObject,
                position = owner == null ? Vector3.zero : owner.DamagePoint,
                signature = definition.signature,
                catalyst = definition.catalyst,
                tier = definition.tier,
                radius = definition.radius,
                duration = definition.duration,
                intensity = Mathf.Clamp(
                    definition.damageMultiplier * 0.18f +
                    definition.ElementCount * 0.08f,
                    0.35f,
                    1.35f) * GenerationVisualScale(context),
                death = death,
                originCastId = context.originCastId,
                propagationGeneration = context.generation,
                reactionSourceKind = context.sourceKind,
                fieldAuthority = ReactionFieldAuthority22.Cosmetic
            });
        }

        public static void EmitSingleElementDeath(
            EnemyController owner,
            ReactionElement element,
            float radius,
            float intensity,
            ReactionContext22 context)
        {
            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.ReactionDeathTriggered,
                priority = context.generation <= 1
                    ? PresentationPriority.Important
                    : PresentationPriority.Normal,
                source = owner == null ? null : owner.transform,
                target = owner == null ? null : owner.transform,
                host = owner == null ? null : owner.gameObject,
                position = owner == null ? Vector3.zero : owner.DamagePoint,
                signature = element,
                catalyst = element,
                radius = radius,
                duration = 0.55f,
                intensity = intensity * GenerationVisualScale(context),
                death = true,
                originCastId = context.originCastId,
                propagationGeneration = context.generation,
                reactionSourceKind = context.sourceKind,
                fieldAuthority = ReactionFieldAuthority22.Cosmetic
            });
        }

        public static void EmitMechanic(
            ReactionMechanicSpec spec,
            ElementalReactionDefinition definition,
            EnemyController owner,
            Vector3 position,
            float baseDamage,
            bool death,
            int mechanicIndex,
            ReactionContext22 context)
        {
            if (spec == null || definition == null)
                return;

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.ReactionMechanic,
                priority = PriorityFor(definition.tier, context),
                source = owner == null ? null : owner.transform,
                target = owner == null ? null : owner.transform,
                host = owner == null ? null : owner.gameObject,
                position = position,
                signature = spec.payload == ReactionElement.None
                    ? definition.signature
                    : spec.payload,
                catalyst = definition.catalyst,
                tier = definition.tier,
                mechanicType = spec.type,
                mechanicId = definition.id + ":" + mechanicIndex + ":" + spec.type,
                count = spec.count,
                radius = spec.radius,
                duration = spec.duration,
                delay = spec.delay,
                intensity = Mathf.Clamp(
                    0.26f + spec.magnitude * 0.18f,
                    0.22f,
                    1.05f) * GenerationVisualScale(context),
                death = death,
                originCastId = context.originCastId,
                propagationGeneration = context.generation,
                reactionSourceKind = context.sourceKind,
                fieldAuthority = ReactionFieldAuthority22.Cosmetic
            });
        }

        public static void EmitField(
            SpellPresentationEventType type,
            GameObject host,
            ReactionElement signature,
            float radius,
            float duration,
            float intensity,
            ReactionContext22 context,
            ReactionFieldAuthority22 authority)
        {
            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = type,
                priority = FieldPriority(authority),
                host = host,
                source = host == null ? null : host.transform,
                position = host == null ? Vector3.zero : host.transform.position,
                signature = signature,
                catalyst = ElementalReactionCodex.PrimaryElement(signature),
                radius = radius,
                duration = duration,
                intensity = Mathf.Clamp01(intensity) *
                    FieldVisualScale(authority) *
                    GenerationVisualScale(context),
                originCastId = context.originCastId,
                propagationGeneration = context.generation,
                reactionSourceKind = context.sourceKind,
                fieldAuthority = authority
            });
        }

        public static void EmitChainContact(
            Vector3 from,
            Vector3 to,
            ReactionElement signature,
            int jump,
            ReactionContext22 context)
        {
            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.LinkActivated,
                priority = jump == 0
                    ? PresentationPriority.Important
                    : PresentationPriority.Normal,
                position = from,
                secondaryPosition = to,
                direction = (to - from).normalized,
                signature = signature,
                catalyst = ElementalReactionCodex.PrimaryElement(signature),
                count = jump,
                duration = 0.12f,
                intensity = ReactionBalance22.ChainVisualIntensity(jump) *
                    GenerationVisualScale(context),
                originCastId = context.originCastId,
                propagationGeneration = context.generation,
                reactionSourceKind = ReactionSourceKind22.Chain,
                fieldAuthority = ReactionFieldAuthority22.Cosmetic
            });
        }

        public static void EmitLocalDeathMarker(
            Vector3 position,
            ReactionElement signature,
            float intensity,
            ReactionContext22 context)
        {
            TryBurst(
                position,
                ElementalReactionCodex.BlendColor(signature),
                Mathf.Clamp(intensity, 0.16f, 0.55f),
                context,
                false);
        }

        public static bool TryBurst(
            Vector3 position,
            Color color,
            float intensity,
            ReactionContext22 context)
        {
            return TryBurst(position, color, intensity, context, intensity >= 0.85f);
        }

        public static bool TryBurst(
            Vector3 position,
            Color color,
            float intensity,
            ReactionContext22 context,
            bool major)
        {
            TrimBurstTimes();
            bool allowMajor = !major || MajorBurstTimes.Count < 2;
            bool allowAny = AnyBurstTimes.Count < 3;

            if (!allowAny)
            {
                ReactionDiagnostics22.RecordCoalesced("flash-limit", context.originCastId);
                return false;
            }

            float adjusted = intensity * GenerationVisualScale(context);
            if (!allowMajor)
            {
                adjusted *= 0.32f;
                major = false;
                ReactionDiagnostics22.RecordCoalesced("major-flash-limit", context.originCastId);
            }

            if (adjusted < 0.14f)
                return false;

            GameFeelSystem.Burst(position, color, adjusted);
            AnyBurstTimes.Enqueue(Time.unscaledTime);
            if (major)
                MajorBurstTimes.Enqueue(Time.unscaledTime);
            return true;
        }

        private static void TrimBurstTimes()
        {
            float now = Time.unscaledTime;
            while (AnyBurstTimes.Count > 0 && now - AnyBurstTimes.Peek() > 1f)
                AnyBurstTimes.Dequeue();
            while (MajorBurstTimes.Count > 0 && now - MajorBurstTimes.Peek() > 1f)
                MajorBurstTimes.Dequeue();
        }

        private static PresentationPriority PriorityFor(
            ReactionTier tier,
            ReactionContext22 context)
        {
            if (context.generation >= 2)
                return PresentationPriority.Normal;

            switch (tier)
            {
                case ReactionTier.Calamity:
                case ReactionTier.Apex:
                    return PresentationPriority.Critical;
                case ReactionTier.Catastrophe:
                case ReactionTier.Convergence:
                    return PresentationPriority.Important;
                default:
                    return PresentationPriority.Normal;
            }
        }

        private static PresentationPriority FieldPriority(
            ReactionFieldAuthority22 authority)
        {
            switch (authority)
            {
                case ReactionFieldAuthority22.ExplicitPersistent:
                    return PresentationPriority.Important;
                case ReactionFieldAuthority22.PrimaryReaction:
                    return PresentationPriority.Normal;
                default:
                    return PresentationPriority.Decorative;
            }
        }

        private static float GenerationVisualScale(ReactionContext22 context)
        {
            switch (context.generation)
            {
                case 0: return 1f;
                case 1: return 0.78f;
                case 2: return 0.48f;
                default: return 0.28f;
            }
        }

        private static float FieldVisualScale(ReactionFieldAuthority22 authority)
        {
            switch (authority)
            {
                case ReactionFieldAuthority22.ExplicitPersistent: return 1f;
                case ReactionFieldAuthority22.PrimaryReaction: return 0.78f;
                case ReactionFieldAuthority22.DeathResidue: return 0.48f;
                case ReactionFieldAuthority22.SecondaryPropagation: return 0.30f;
                default: return 0.16f;
            }
        }
    }

    public static class ReactionPresentationCoalescer22
    {
        private sealed class Stamp
        {
            public float time;
            public Vector3 position;
        }

        private static readonly Dictionary<string, Stamp> Stamps =
            new Dictionary<string, Stamp>();
        private static float _nextCleanup;

        public static bool TryAccept(ref SpellPresentationEvent presentationEvent)
        {
            if (presentationEvent.originCastId == 0L)
                return true;

            if (!IsReactionEvent(presentationEvent.type))
                return true;

            presentationEvent.intensity *= GenerationScale(
                presentationEvent.propagationGeneration);

            string key = BuildKey(presentationEvent);
            Stamp stamp;
            float now = Time.unscaledTime;

            if (Stamps.TryGetValue(key, out stamp) &&
                now - stamp.time <= ReactionBalance22.CoalescingWindow &&
                Vector3.Distance(stamp.position, presentationEvent.position) <=
                    Mathf.Max(1.2f, presentationEvent.radius * 0.65f))
            {
                presentationEvent.coalesced = true;
                ReactionDiagnostics22.RecordCoalesced(
                    presentationEvent.type.ToString(),
                    presentationEvent.originCastId);
                return false;
            }

            Stamps[key] = new Stamp
            {
                time = now,
                position = presentationEvent.position
            };

            if (now >= _nextCleanup)
            {
                _nextCleanup = now + 2f;
                Cleanup(now);
            }

            return true;
        }

        private static bool IsReactionEvent(SpellPresentationEventType type)
        {
            switch (type)
            {
                case SpellPresentationEventType.ReactionResolved:
                case SpellPresentationEventType.ReactionDeathTriggered:
                case SpellPresentationEventType.ReactionMechanic:
                case SpellPresentationEventType.FieldCreated:
                case SpellPresentationEventType.FieldMerged:
                case SpellPresentationEventType.FieldPulsed:
                case SpellPresentationEventType.FieldExpired:
                case SpellPresentationEventType.LinkActivated:
                    return true;
                default:
                    return false;
            }
        }

        private static string BuildKey(SpellPresentationEvent value)
        {
            return value.originCastId + "|" +
                   value.type + "|" +
                   value.signature + "|" +
                   value.mechanicType + "|" +
                   value.fieldAuthority;
        }

        private static float GenerationScale(int generation)
        {
            switch (generation)
            {
                case 0: return 1f;
                case 1: return 0.82f;
                case 2: return 0.55f;
                default: return 0.30f;
            }
        }

        private static void Cleanup(float now)
        {
            List<string> remove = new List<string>();
            foreach (KeyValuePair<string, Stamp> pair in Stamps)
            {
                if (now - pair.Value.time > 2f)
                    remove.Add(pair.Key);
            }

            for (int i = 0; i < remove.Count; i++)
                Stamps.Remove(remove[i]);
        }
    }
}
