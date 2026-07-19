#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    public static class Patch200Diagnostics
    {
        [MenuItem("Arcane Engine/2.0/Validate Patch 2.0.0")]
        public static void Validate()
        {
            List<string> errors = new List<string>();
            HashSet<int> signatures = new HashSet<int>();
            HashSet<string> names = new HashSet<string>();
            HashSet<string> graphIds = new HashSet<string>();
            int definitionCount = 0;
            int planCount = 0;
            int resolveMechanics = 0;
            int deathMechanics = 0;

            foreach (ElementalReactionDefinition definition in ElementalReactionCodex.All)
            {
                definitionCount++;
                if (definition == null)
                {
                    errors.Add("Null elemental reaction definition.");
                    continue;
                }

                if (!signatures.Add((int)definition.signature))
                    errors.Add("Duplicate reaction signature: " + definition.signature);

                if (!names.Add(definition.displayName))
                    errors.Add("Duplicate reaction name: " + definition.displayName);

                int count = ElementalReactionCodex.CountBits(definition.signature);
                if (count < 2 || count > 7)
                    errors.Add("Invalid element count for " + definition.displayName + ": " + count);

                ReactionMechanicPlan plan = ElementalReactionMechanicCodex.Get(definition.signature);
                if (plan == null)
                {
                    errors.Add("Missing mechanic plan for " + definition.displayName);
                    continue;
                }

                planCount++;
                resolveMechanics += plan.resolve.Length;
                deathMechanics += plan.death.Length;

                if (string.IsNullOrEmpty(plan.graphId))
                    errors.Add("Missing graph ID for " + definition.displayName);
                else if (!graphIds.Add(plan.graphId))
                    errors.Add("Duplicate graph ID: " + plan.graphId);

                if (plan.resolve.Length == 0)
                    errors.Add("Empty resolve graph: " + definition.displayName);

                if (plan.death.Length == 0)
                    errors.Add("Empty death graph: " + definition.displayName);
            }

            int profileCount = 0;
            HashSet<ReactionElement> profiles = new HashSet<ReactionElement>();
            foreach (ElementVisualProfile2 profile in ElementVisualProfileRegistry2.All)
            {
                if (profile == null)
                {
                    errors.Add("Null element visual profile.");
                    continue;
                }

                profileCount++;
                if (!profiles.Add(profile.element))
                    errors.Add("Duplicate element visual profile: " + profile.element);

                if (profile.primary.a <= 0f)
                    errors.Add("Invisible primary profile color: " + profile.element);

                if (string.IsNullOrEmpty(profile.motionLanguage) ||
                    string.IsNullOrEmpty(profile.impactLanguage) ||
                    string.IsNullOrEmpty(profile.residueLanguage))
                {
                    errors.Add("Incomplete visual grammar: " + profile.element);
                }
            }

            string[] requiredFiles =
            {
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationCore.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationAPI.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationDirector.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationParticles.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationGeometry.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationAttachments.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationResidues.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationEffectObjects.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationEffectObjects2.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationAudioCamera.cs",
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationOverlays.cs",
                "Assets/ArcaneEngine/Scripts/Combat/ElementalReactionRuntime.cs",
                "Assets/ArcaneEngine/Scripts/Combat/ElementalReactionMechanicExecutor.cs",
                "docs/PATCH_NOTES_2.0.0.md",
                "docs/IMPLEMENTATION_MANIFEST_2.0.0.md",
                "docs/VALIDATION_CHECKLIST_2.0.0.md"
            };

            foreach (string path in requiredFiles)
            {
                if (!File.Exists(path))
                    errors.Add("Missing required Patch 2.0.0 file: " + path);
            }

            if (definitionCount != 120)
                errors.Add("Reaction definition count is " + definitionCount + "; expected 120.");

            if (planCount != 120)
                errors.Add("Reaction mechanic plan count is " + planCount + "; expected 120.");

            if (graphIds.Count != 120)
                errors.Add("Unique mechanic graph count is " + graphIds.Count + "; expected 120.");

            if (profileCount < 8)
                errors.Add("Visual profile count is " + profileCount + "; expected Arcane plus seven reaction elements.");

            if (GeneratedVisualRecipe.CurrentSchemaVersion != 20000)
                errors.Add("Unexpected generated visual recipe schema: " + GeneratedVisualRecipe.CurrentSchemaVersion);

            if (errors.Count > 0)
            {
                Debug.LogError(
                    "Arcane Engine Patch 2.0.0 validation failed with " +
                    errors.Count + " problem(s):\n- " +
                    string.Join("\n- ", errors));
                return;
            }

            Debug.Log(
                "Arcane Engine Patch 2.0.0 validation passed. " +
                "120 reaction signatures, 120 unique executable graphs, " +
                resolveMechanics + " resolution instructions, " +
                deathMechanics + " death instructions, " +
                profileCount + " visual profiles, and schema " +
                GeneratedVisualRecipe.CurrentSchemaVersion + ".");
        }

        [MenuItem("Arcane Engine/2.0/Open Patch Notes")]
        public static void OpenPatchNotes()
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(
                "docs/PATCH_NOTES_2.0.0.md");

            if (asset != null)
            {
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
            else
            {
                Debug.LogWarning("Patch notes file was not imported as an Asset. Open docs/PATCH_NOTES_2.0.0.md from the project folder.");
            }
        }
    }
}
#endif
