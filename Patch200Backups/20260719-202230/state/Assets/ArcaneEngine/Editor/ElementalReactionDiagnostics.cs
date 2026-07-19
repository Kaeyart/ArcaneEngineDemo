#if UNITY_EDITOR
using System.Collections.Generic;
using ArcaneEngine;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    public static class ElementalReactionDiagnostics
    {
        [MenuItem(
            "Arcane Engine/Diagnostics/Validate Elemental Reaction Codex")]
        public static void Validate()
        {
            HashSet<int> masks = new HashSet<int>();
            HashSet<string> names = new HashSet<string>();
            HashSet<string> graphIds = new HashSet<string>();
            int definitions = 0;
            int plans = 0;
            int resolveMechanics = 0;
            int deathMechanics = 0;

            foreach (
                ElementalReactionDefinition definition
                in ElementalReactionCodex.All)
            {
                definitions++;

                if (!masks.Add((int)definition.signature))
                {
                    Debug.LogError(
                        "Duplicate reaction signature: " +
                        definition.signature);
                }

                if (!names.Add(definition.displayName))
                {
                    Debug.LogError(
                        "Duplicate reaction name: " +
                        definition.displayName);
                }

                ReactionMechanicPlan plan =
                    ElementalReactionMechanicCodex.Get(
                        definition.signature);

                if (plan == null)
                {
                    Debug.LogError(
                        "Missing mechanic graph for " +
                        definition.displayName + ".");

                    continue;
                }

                plans++;
                resolveMechanics += plan.resolve.Length;
                deathMechanics += plan.death.Length;

                if (!graphIds.Add(plan.graphId))
                {
                    Debug.LogError(
                        "Duplicate mechanic graph ID: " +
                        plan.graphId);
                }

                if (plan.resolve.Length == 0)
                {
                    Debug.LogError(
                        "Reaction has no resolve mechanics: " +
                        definition.displayName);
                }

                if (plan.death.Length == 0)
                {
                    Debug.LogError(
                        "Reaction has no death mechanics: " +
                        definition.displayName);
                }

                int elements =
                    ElementalReactionCodex.CountBits(
                        definition.signature);

                if (elements < 2 || elements > 7)
                {
                    Debug.LogError(
                        "Invalid reaction element count for " +
                        definition.displayName +
                        ": " +
                        elements);
                }
            }

            bool valid =
                definitions == 120 &&
                masks.Count == 120 &&
                names.Count == 120 &&
                plans == 120 &&
                graphIds.Count == 120 &&
                ElementalReactionMechanicCodex.Count == 120;

            if (!valid)
            {
                Debug.LogError(
                    "Elemental reaction validation failed. " +
                    "Definitions=" + definitions +
                    ", masks=" + masks.Count +
                    ", names=" + names.Count +
                    ", plans=" + plans +
                    ", graph IDs=" + graphIds.Count +
                    ". Expected 120 each.");

                return;
            }

            Debug.Log(
                "Elemental reactions valid: " +
                "120 signatures, 120 unique executable mechanic graphs, " +
                resolveMechanics + " resolve mechanics and " +
                deathMechanics + " death mechanics.");
        }
    }
}
#endif
