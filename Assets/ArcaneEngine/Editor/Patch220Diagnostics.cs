#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    public sealed class Patch220DiagnosticsWindow : EditorWindow
    {
        private Vector2 _scroll;

        [MenuItem("Arcane Engine/2.2/Propagation Diagnostics")]
        private static void Open()
        {
            GetWindow<Patch220DiagnosticsWindow>("Reaction Propagation 2.2");
        }

        [MenuItem("Arcane Engine/2.2/Validate Patch 2.2.0")]
        private static void ValidateMenu()
        {
            List<string> failures = ValidateInstallation();
            if (failures.Count == 0)
                Debug.Log("[Arcane Engine 2.2] Validation passed. Reaction lineage, mechanic budgets, field authority, presentation coalescing and SpellExecution integration markers are present.");
            else
                Debug.LogError("[Arcane Engine 2.2] Validation failed:\n- " + string.Join("\n- ", failures));
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Patch 2.2 — Reaction Propagation", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This inspector reports current Play Mode counters. Press F5 in Play Mode for the in-game overlay.", MessageType.Info);

            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                if (GUILayout.Button("Validate Installed Source"))
                    ValidateMenu();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Direct applications", ReactionDiagnostics22.DirectApplications.ToString());
            EditorGUILayout.LabelField("Propagated applications", ReactionDiagnostics22.PropagatedApplications.ToString());
            EditorGUILayout.LabelField("Blocked recursion", ReactionDiagnostics22.BlockedRecursiveEffects.ToString());
            EditorGUILayout.LabelField("Target revisits blocked", ReactionDiagnostics22.TargetRevisitsBlocked.ToString());
            EditorGUILayout.LabelField("Mechanics selected", ReactionDiagnostics22.SelectedMechanics.ToString());
            EditorGUILayout.LabelField("Mechanics discarded", ReactionDiagnostics22.DiscardedMechanics.ToString());
            EditorGUILayout.LabelField("Presentation coalesced", ReactionDiagnostics22.CoalescedPresentationEvents.ToString());
            EditorGUILayout.LabelField("Fields merged", ReactionDiagnostics22.FieldsMerged.ToString());
            EditorGUILayout.LabelField("Fields rejected", ReactionDiagnostics22.FieldsRejected.ToString());
            EditorGUILayout.LabelField("Reproduction ratio", ReactionDiagnostics22.ReproductionRatio.ToString("0.00"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Recent decisions", EditorStyles.boldLabel);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            string[] recent = ReactionDiagnostics22.Snapshot();
            for (int i = recent.Length - 1; i >= 0; i--)
                EditorGUILayout.SelectableLabel(recent[i], GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Reset Runtime Counters"))
                ReactionDiagnostics22.Reset();
        }

        private static List<string> ValidateInstallation()
        {
            List<string> failures = new List<string>();
            RequireFile("Assets/ArcaneEngine/PATCH_2_2_0.txt", failures);
            RequireFile("Assets/ArcaneEngine/Scripts/Combat/ReactionPropagation22.cs", failures);
            RequireFile("Assets/ArcaneEngine/Scripts/Combat/SpellReactionBridge22.cs", failures);
            RequireFile("Assets/ArcaneEngine/Scripts/Presentation/ReactionPresentation22.cs", failures);
            RequireFile("Assets/ArcaneEngine/Scripts/Combat/ReactionDiagnosticsOverlay22.cs", failures);

            RequireMarker("Assets/ArcaneEngine/Scripts/Spells/SpellExecution.cs", "ARCANE_PATCH_220_REACTION_CLARITY", failures);
            RequireMarker("Assets/ArcaneEngine/Scripts/Combat/ElementalReactionRuntime.cs", "PropagatedThresholdCap", failures);
            RequireMarker("Assets/ArcaneEngine/Scripts/Combat/ElementalReactionMechanicExecutor.cs", "ResolveMechanicBudget", failures);
            RequireMarker("Assets/ArcaneEngine/Scripts/Combat/ElementalReactionField.cs", "MaximumGameplayFields", failures);
            RequireMarker("Assets/ArcaneEngine/Scripts/Presentation/PresentationCore.cs", "ReactionPresentationCoalescer22", failures);

            if (ReactionBalance22.MaximumGameplayFields != 16)
                failures.Add("Maximum gameplay-field cap is not 16.");
            if (ReactionBalance22.MaximumLocalGameplayFields != 6)
                failures.Add("Local gameplay-field cap is not 6.");
            if (ReactionBalance22.MaximumPropagationGeneration != 2)
                failures.Add("Maximum mechanical propagation generation is not 2.");
            if (ReactionBalance22.MaximumChainTargets != 3)
                failures.Add("Default secondary Chain target limit is not 3.");

            return failures;
        }

        private static void RequireFile(string path, List<string> failures)
        {
            if (!File.Exists(path))
                failures.Add("Missing " + path);
        }

        private static void RequireMarker(string path, string marker, List<string> failures)
        {
            if (!File.Exists(path))
            {
                failures.Add("Missing " + path);
                return;
            }

            string text = File.ReadAllText(path);
            if (!text.Contains(marker))
                failures.Add(path + " is missing marker " + marker + ".");
        }
    }
}
#endif
