using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ReactionSourceKind22
    {
        DirectCast,
        RuneDerived,
        PrimaryReaction,
        DeathReaction,
        Field,
        Chain,
        Echo,
        Environmental
    }

    public enum ReactionFieldAuthority22
    {
        ExplicitPersistent,
        PrimaryReaction,
        DeathResidue,
        SecondaryPropagation,
        Cosmetic
    }

    [Serializable]
    public struct ReactionContext22
    {
        public long originCastId;
        public int generation;
        public ReactionSourceKind22 sourceKind;
        public float damageCoefficient;
        public float buildupCoefficient;
        public float reactionRechargeCoefficient;
        public bool canActivateMajor;
        public bool canTriggerDeathReaction;
        public bool canCreateField;
        public bool canRechargeReaction;

        public bool IsValid
        {
            get { return originCastId != 0L; }
        }

        public static ReactionContext22 Direct(ReactionSourceKind22 kind)
        {
            return new ReactionContext22
            {
                originCastId = ReactionLineageRegistry22.NextOrigin(),
                generation = 0,
                sourceKind = kind,
                damageCoefficient = 1f,
                buildupCoefficient = 1f,
                reactionRechargeCoefficient = 1f,
                canActivateMajor = true,
                canTriggerDeathReaction = true,
                canCreateField = true,
                canRechargeReaction = true
            };
        }

        public static ReactionContext22 Legacy(bool propagated)
        {
            return propagated
                ? Direct(ReactionSourceKind22.Environmental).Derive(ReactionSourceKind22.Echo)
                : Direct(ReactionSourceKind22.DirectCast);
        }

        public ReactionContext22 Derive(ReactionSourceKind22 kind)
        {
            int nextGeneration = Mathf.Max(0, generation + 1);
            ReactionContext22 next = this;
            next.generation = nextGeneration;
            next.sourceKind = kind;

            if (nextGeneration == 1)
            {
                next.damageCoefficient *= 0.60f;
                next.buildupCoefficient *= 0.35f;
                next.reactionRechargeCoefficient *= 0.25f;
                next.canActivateMajor = canActivateMajor;
                next.canTriggerDeathReaction = canTriggerDeathReaction;
                next.canCreateField = canCreateField;
                next.canRechargeReaction = canRechargeReaction;
            }
            else if (nextGeneration == 2)
            {
                // Generation two targets 30% damage and 10% buildup of the
                // original source while preserving any mechanic-specific
                // coefficient already carried by the generation-one parent.
                next.damageCoefficient *= generation == 1 ? 0.50f : 0.30f;
                next.buildupCoefficient *= generation == 1
                    ? (0.10f / 0.35f)
                    : 0.10f;
                next.reactionRechargeCoefficient = 0f;
                next.canActivateMajor = false;
                next.canTriggerDeathReaction = false;
                next.canCreateField = false;
                next.canRechargeReaction = false;
            }
            else
            {
                next.damageCoefficient *= 0.12f;
                next.buildupCoefficient = 0f;
                next.reactionRechargeCoefficient = 0f;
                next.canActivateMajor = false;
                next.canTriggerDeathReaction = false;
                next.canCreateField = false;
                next.canRechargeReaction = false;
            }

            return next;
        }

        public ReactionContext22 AsSource(ReactionSourceKind22 kind)
        {
            ReactionContext22 result = this;
            result.sourceKind = kind;
            return result;
        }

        public ReactionContext22 WithCoefficients(
            float damageScale,
            float buildupScale,
            float rechargeScale)
        {
            ReactionContext22 result = this;
            result.damageCoefficient *= Mathf.Max(0f, damageScale);
            result.buildupCoefficient *= Mathf.Max(0f, buildupScale);
            result.reactionRechargeCoefficient *= Mathf.Max(0f, rechargeScale);
            return result;
        }

        public ReactionContext22 WithoutFieldCreation()
        {
            ReactionContext22 result = this;
            result.canCreateField = false;
            return result;
        }

        public ReactionContext22 WithoutDeathPropagation()
        {
            ReactionContext22 result = this;
            result.canTriggerDeathReaction = false;
            return result;
        }
    }

    public static class ReactionBalance22
    {
        public const float PropagatedThresholdCap = 0.75f;
        public const float DirectThresholdRequirement = 0.35f;
        public const float GenerationOnePreparationRequirement = 0.70f;
        public const float ReactionParticipationFraction = 0.20f;
        public const float MajorOverflowFraction = 0.25f;
        public const int MaximumPropagationGeneration = 2;
        public const int MaximumSingleDeathTargets = 4;
        public const int MaximumChainTargets = 3;
        public const int MaximumGameplayFields = 16;
        public const int MaximumLocalGameplayFields = 6;
        public const float FieldPowerCap = 1.25f;
        public const float FieldReinforcement = 0.10f;
        public const float FieldSurgePowerCap = 0.15f;
        public const float FieldSurgeDurationCap = 0.25f;
        public const float FieldSurgeCooldown = 2f;
        public const float CoalescingWindow = 0.12f;

        public static float MajorRecoveryDuration(EnemyController owner)
        {
            if (owner == null)
                return 2.25f;

            if (owner.IsBoss)
                return 4f;

            if (owner.IsEliteOrBoss)
                return 3f;

            return 2.25f;
        }

        public static float ReactionConsumption(ReactionTier tier)
        {
            switch (tier)
            {
                case ReactionTier.Fusion: return 0.55f;
                case ReactionTier.Compound: return 0.60f;
                case ReactionTier.Catastrophe: return 0.70f;
                case ReactionTier.Convergence: return 0.75f;
                case ReactionTier.Calamity: return 0.80f;
                case ReactionTier.Apex: return 1f;
                default: return 0.60f;
            }
        }

        public static float SignatureLockout(ReactionTier tier)
        {
            switch (tier)
            {
                case ReactionTier.Fusion: return 2.5f;
                case ReactionTier.Compound: return 3.5f;
                case ReactionTier.Catastrophe: return 5f;
                case ReactionTier.Convergence: return 6.5f;
                case ReactionTier.Calamity: return 8f;
                case ReactionTier.Apex: return 12f;
                default: return 3.5f;
            }
        }

        public static int ResolveMechanicBudget(ReactionTier tier, bool death)
        {
            if (death)
            {
                switch (tier)
                {
                    case ReactionTier.Fusion: return 2;
                    case ReactionTier.Compound: return 2;
                    case ReactionTier.Catastrophe: return 3;
                    case ReactionTier.Convergence: return 3;
                    case ReactionTier.Calamity: return 4;
                    case ReactionTier.Apex: return 5;
                    default: return 2;
                }
            }

            switch (tier)
            {
                case ReactionTier.Fusion: return 3;
                case ReactionTier.Compound: return 4;
                case ReactionTier.Catastrophe: return 5;
                case ReactionTier.Convergence: return 6;
                case ReactionTier.Calamity: return 8;
                case ReactionTier.Apex: return 12;
                default: return 3;
            }
        }

        public static float FieldDamageAuthority(ReactionFieldAuthority22 authority)
        {
            switch (authority)
            {
                case ReactionFieldAuthority22.ExplicitPersistent: return 1f;
                case ReactionFieldAuthority22.PrimaryReaction: return 0.70f;
                case ReactionFieldAuthority22.DeathResidue: return 0.40f;
                case ReactionFieldAuthority22.SecondaryPropagation: return 0.20f;
                default: return 0f;
            }
        }

        public static float FieldBuildupAuthority(ReactionFieldAuthority22 authority)
        {
            switch (authority)
            {
                case ReactionFieldAuthority22.ExplicitPersistent: return 1f;
                case ReactionFieldAuthority22.PrimaryReaction: return 0.30f;
                case ReactionFieldAuthority22.DeathResidue: return 0.18f;
                case ReactionFieldAuthority22.SecondaryPropagation: return 0.08f;
                default: return 0f;
            }
        }

        public static int FieldPriority(ReactionFieldAuthority22 authority)
        {
            switch (authority)
            {
                case ReactionFieldAuthority22.ExplicitPersistent: return 4;
                case ReactionFieldAuthority22.PrimaryReaction: return 3;
                case ReactionFieldAuthority22.DeathResidue: return 2;
                case ReactionFieldAuthority22.SecondaryPropagation: return 1;
                default: return 0;
            }
        }

        public static float ChainDamageCoefficient(int jump)
        {
            switch (jump)
            {
                case 0: return 0.68f;
                case 1: return 0.48f;
                case 2: return 0.32f;
                default: return 0.18f * Mathf.Pow(0.75f, jump - 3);
            }
        }

        public static float ChainBuildupAmount(int jump)
        {
            switch (jump)
            {
                case 0: return 0.45f;
                case 1: return 0.28f;
                case 2: return 0.16f;
                default: return 0.06f;
            }
        }

        public static float ChainVisualIntensity(int jump)
        {
            switch (jump)
            {
                case 0: return 0.85f;
                case 1: return 0.70f;
                case 2: return 0.55f;
                default: return 0.40f;
            }
        }
    }

    public static class ReactionLineageRegistry22
    {
        private static long _nextOrigin;
        private static readonly Dictionary<long, Dictionary<int, float>> TargetTimes =
            new Dictionary<long, Dictionary<int, float>>();
        private static readonly Dictionary<long, float> LastTouched =
            new Dictionary<long, float>();
        private static float _nextCleanup;

        public static long NextOrigin()
        {
            return Interlocked.Increment(ref _nextOrigin);
        }

        public static bool TryMarkTarget(
            ReactionContext22 context,
            EnemyController target,
            float immunitySeconds)
        {
            if (!context.IsValid || target == null)
                return true;

            CleanupIfNeeded();

            Dictionary<int, float> targets;
            if (!TargetTimes.TryGetValue(context.originCastId, out targets))
            {
                targets = new Dictionary<int, float>();
                TargetTimes[context.originCastId] = targets;
            }

            int id = target.GetInstanceID();
            float until;
            if (targets.TryGetValue(id, out until) && Time.time < until)
            {
                ReactionDiagnostics22.RecordBlocked("target-revisit", context);
                return false;
            }

            targets[id] = Time.time + Mathf.Max(0.02f, immunitySeconds);
            LastTouched[context.originCastId] = Time.time;
            return true;
        }

        public static void Touch(ReactionContext22 context)
        {
            if (context.IsValid)
                LastTouched[context.originCastId] = Time.time;
        }

        private static void CleanupIfNeeded()
        {
            if (Time.time < _nextCleanup)
                return;

            _nextCleanup = Time.time + 2f;
            List<long> stale = new List<long>();

            foreach (KeyValuePair<long, float> pair in LastTouched)
            {
                if (Time.time - pair.Value > 12f)
                    stale.Add(pair.Key);
            }

            for (int i = 0; i < stale.Count; i++)
            {
                TargetTimes.Remove(stale[i]);
                LastTouched.Remove(stale[i]);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            _nextOrigin = 0L;
            TargetTimes.Clear();
            LastTouched.Clear();
            _nextCleanup = 0f;
        }
    }

    public static class ReactionDiagnostics22
    {
        private const int RecentLimit = 96;
        private static readonly Queue<string> Recent = new Queue<string>();
        private static readonly Dictionary<long, int> ReactionDeaths = new Dictionary<long, int>();
        private static readonly Dictionary<long, int> QualifyingChildren = new Dictionary<long, int>();

        public static int DirectApplications { get; private set; }
        public static int PropagatedApplications { get; private set; }
        public static int BlockedRecursiveEffects { get; private set; }
        public static int SelectedMechanics { get; private set; }
        public static int DiscardedMechanics { get; private set; }
        public static int CoalescedPresentationEvents { get; private set; }
        public static int FieldsMerged { get; private set; }
        public static int FieldsRejected { get; private set; }
        public static int TargetRevisitsBlocked { get; private set; }

        public static float ReproductionRatio
        {
            get
            {
                int causes = 0;
                int children = 0;

                foreach (int value in ReactionDeaths.Values)
                    causes += value;

                foreach (int value in QualifyingChildren.Values)
                    children += value;

                return causes <= 0 ? 0f : children / (float)causes;
            }
        }

        public static string[] Snapshot()
        {
            return Recent.ToArray();
        }

        public static void RecordApplication(ReactionContext22 context, ReactionElement element, float amount)
        {
            if (context.generation <= 0)
                DirectApplications++;
            else
                PropagatedApplications++;

            Add("BUILD " + context.sourceKind + " g" + context.generation + " " + element + " +" + amount.ToString("0.00") + " · origin " + context.originCastId);
        }

        public static void RecordBlocked(string reason, ReactionContext22 context)
        {
            BlockedRecursiveEffects++;
            if (reason == "target-revisit")
                TargetRevisitsBlocked++;
            Add("BLOCK " + reason + " · g" + context.generation + " · origin " + context.originCastId);
        }

        public static void RecordMechanic(bool selected, ReactionMechanicType type, ReactionContext22 context)
        {
            if (selected)
                SelectedMechanics++;
            else
                DiscardedMechanics++;

            Add((selected ? "SELECT " : "DROP ") + type + " · g" + context.generation + " · origin " + context.originCastId);
        }

        public static void RecordFieldMerge(ReactionElement signature)
        {
            FieldsMerged++;
            Add("FIELD MERGE " + ElementalReactionCodex.SignatureText(signature));
        }

        public static void RecordFieldRejected(ReactionElement signature)
        {
            FieldsRejected++;
            Add("FIELD REJECT " + ElementalReactionCodex.SignatureText(signature));
        }

        public static void RecordCoalesced(string family, long origin)
        {
            CoalescedPresentationEvents++;
            Add("COALESCE " + family + " · origin " + origin);
        }

        public static void RecordReactionDeath(ReactionContext22 context, bool qualifyingChild)
        {
            if (!context.IsValid)
                return;

            Dictionary<long, int> dictionary = qualifyingChild ? QualifyingChildren : ReactionDeaths;
            int value;
            dictionary.TryGetValue(context.originCastId, out value);
            dictionary[context.originCastId] = value + 1;
        }

        private static void Add(string message)
        {
            while (Recent.Count >= RecentLimit)
                Recent.Dequeue();

            Recent.Enqueue(Time.time.ToString("0.00") + " · " + message);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            DirectApplications = 0;
            PropagatedApplications = 0;
            BlockedRecursiveEffects = 0;
            SelectedMechanics = 0;
            DiscardedMechanics = 0;
            CoalescedPresentationEvents = 0;
            FieldsMerged = 0;
            FieldsRejected = 0;
            TargetRevisitsBlocked = 0;
            Recent.Clear();
            ReactionDeaths.Clear();
            QualifyingChildren.Clear();
        }
    }
}
