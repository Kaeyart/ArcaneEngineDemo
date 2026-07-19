using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ReactionMechanicType
    {
        Burst,
        PulseNova,
        ChainArc,
        Pull,
        Push,
        Freeze,
        Vulnerability,
        BuildupWave,
        Field,
        SplitFields,
        DelayedEcho,
        OrbitingNodes,
        ShardNova,
        Execute,
        ThermalCycle,
        Rebound,
        FieldSurge,
        TrailLine,
        Stagger,
        DetonateBuildup,
        Contagion,
        Compression
    }

    [Serializable]
    public sealed class ReactionMechanicSpec
    {
        public readonly ReactionMechanicType type;
        public readonly ReactionElement element;
        public readonly ReactionElement payload;
        public readonly float magnitude;
        public readonly float radius;
        public readonly float duration;
        public readonly int count;
        public readonly float delay;

        public ReactionMechanicSpec(
            ReactionMechanicType mechanicType,
            ReactionElement primaryElement,
            ReactionElement payloadSignature,
            float mechanicMagnitude,
            float mechanicRadius,
            float mechanicDuration,
            int mechanicCount,
            float mechanicDelay)
        {
            type = mechanicType;
            element = primaryElement;
            payload = payloadSignature;
            magnitude = mechanicMagnitude;
            radius = mechanicRadius;
            duration = mechanicDuration;
            count = Mathf.Max(1, mechanicCount);
            delay = Mathf.Max(0f, mechanicDelay);
        }

        public string Summary
        {
            get
            {
                return type + "(" +
                    ElementalReactionCodex.SignatureText(payload) +
                    ", r=" + radius.ToString("0.0") +
                    ", x=" + magnitude.ToString("0.00") +
                    ", n=" + count + ")";
            }
        }
    }

    public sealed class ReactionMechanicPlan
    {
        public readonly ReactionMechanicSpec[] resolve;
        public readonly ReactionMechanicSpec[] death;
        public readonly string graphId;

        public ReactionMechanicPlan(
            string uniqueGraphId,
            ReactionMechanicSpec[] resolveMechanics,
            ReactionMechanicSpec[] deathMechanics)
        {
            graphId = uniqueGraphId;
            resolve = resolveMechanics ?? Array.Empty<ReactionMechanicSpec>();
            death = deathMechanics ?? Array.Empty<ReactionMechanicSpec>();
        }
    }

    public static partial class ElementalReactionMechanicCodex
    {
        private static readonly Dictionary<int, ReactionMechanicPlan> Plans =
            BuildPlans();

        private static Dictionary<int, ReactionMechanicPlan> BuildPlans()
        {
            Dictionary<int, ReactionMechanicPlan> plans =
                new Dictionary<int, ReactionMechanicPlan>(120);

            PopulateGenerated(plans);
            return plans;
        }

        private static void PopulateGenerated(
            Dictionary<int, ReactionMechanicPlan> plans)
        {
            PopulateGeneratedPart(plans);
        }

        static partial void PopulateGeneratedPart(
            Dictionary<int, ReactionMechanicPlan> plans);

        public static int Count { get { return Plans.Count; } }

        public static ReactionMechanicPlan Get(ReactionElement signature)
        {
            ReactionMechanicPlan plan;
            return Plans.TryGetValue((int)signature, out plan) ? plan : null;
        }

        public static IEnumerable<KeyValuePair<int, ReactionMechanicPlan>> All
        {
            get { return Plans; }
        }
    }
}
