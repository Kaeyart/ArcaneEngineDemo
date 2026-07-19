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
            int count = 0;

            foreach (
                ElementalReactionDefinition definition
                in ElementalReactionCodex.All)
            {
                count++;

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

            if (count != 120 ||
                masks.Count != 120 ||
                names.Count != 120)
            {
                Debug.LogError(
                    "Elemental Reaction Codex validation failed. " +
                    "Definitions=" + count +
                    ", masks=" + masks.Count +
                    ", names=" + names.Count +
                    ". Expected 120 each.");

                return;
            }

            Debug.Log(
                "Elemental Reaction Codex valid: " +
                "120 unique multi-element reactions.");
        }
    }
}
#endif
