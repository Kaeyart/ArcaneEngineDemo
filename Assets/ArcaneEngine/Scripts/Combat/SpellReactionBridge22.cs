using UnityEngine;

namespace ArcaneEngine
{
    public static class SpellReactionBridge22
    {
        public static ReactionContext22 ContextFor(
            CastRequest request,
            ReactionSourceKind22 sourceKind)
        {
            int generation = Mathf.Clamp(
                request.generation,
                0,
                ReactionBalance22.MaximumPropagationGeneration + 1);

            long origin = request.budget == null
                ? ReactionLineageRegistry22.NextOrigin()
                : request.budget.eventId;

            ReactionContext22 context = new ReactionContext22
            {
                originCastId = origin,
                generation = generation,
                sourceKind = sourceKind,
                damageCoefficient = 1f,
                buildupCoefficient = 1f,
                reactionRechargeCoefficient = 1f,
                canActivateMajor = true,
                canTriggerDeathReaction = true,
                canCreateField = true,
                canRechargeReaction = true
            };

            if (generation == 1)
            {
                context.damageCoefficient = 0.60f;
                context.buildupCoefficient = 0.35f;
                context.reactionRechargeCoefficient = 0.25f;
            }
            else if (generation >= 2)
            {
                context.damageCoefficient = 0.30f;
                context.buildupCoefficient = 0.10f;
                context.reactionRechargeCoefficient = 0f;
                context.canActivateMajor = false;
                context.canTriggerDeathReaction = false;
                context.canCreateField = false;
                context.canRechargeReaction = false;
            }

            return context;
        }

        public static ReactionContext22 ChainContext(CastRequest request)
        {
            ReactionContext22 parent = ContextFor(
                request,
                request.generation <= 0
                    ? ReactionSourceKind22.DirectCast
                    : ReactionSourceKind22.RuneDerived);

            int generation = Mathf.Min(
                ReactionBalance22.MaximumPropagationGeneration,
                parent.generation + 1);

            return new ReactionContext22
            {
                originCastId = parent.originCastId,
                generation = generation,
                sourceKind = ReactionSourceKind22.Chain,
                damageCoefficient = generation == 1 ? 1f : 0.30f,
                buildupCoefficient = generation == 1 ? 1f : 0.10f,
                reactionRechargeCoefficient = generation == 1 ? 0.25f : 0f,
                canActivateMajor = generation == 1,
                canTriggerDeathReaction = generation == 1,
                canCreateField = false,
                canRechargeReaction = generation == 1
            };
        }

        public static int ChainTargetLimit(CompiledSpell spell)
        {
            if (spell == null)
                return ReactionBalance22.MaximumChainTargets;

            int configured = Mathf.Max(0, spell.chainTargets);
            if (configured <= ReactionBalance22.MaximumChainTargets)
                return configured;

            // Existing high-investment Chain configurations retain limited
            // specialization above the three-target default, capped at five.
            return Mathf.Clamp(
                ReactionBalance22.MaximumChainTargets +
                Mathf.CeilToInt((configured - ReactionBalance22.MaximumChainTargets) * 0.5f),
                ReactionBalance22.MaximumChainTargets,
                5);
        }

        public static float ChainNetworkScale(
            SpellCastBudget budget,
            CompiledSpell spell)
        {
            if (budget == null)
                return 0.65f;

            int structuralCopies = 1;
            if (spell != null)
            {
                structuralCopies = Mathf.Max(1, spell.projectileCount);
                if (spell.splitOnHit)
                    structuralCopies *= Mathf.Max(1, spell.splitCount);
            }

            return budget.ReserveChainNetwork(structuralCopies);
        }
    }
}
