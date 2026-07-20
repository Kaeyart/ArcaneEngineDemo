#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    public static class Patch210Diagnostics
    {
        private static readonly string[] RequiredRuntimeFiles =
        {
            "Assets/ArcaneEngine/Scripts/Presentation21/SpellVisualContract21.cs",
            "Assets/ArcaneEngine/Scripts/Presentation21/SpellMorphologyCompiler21.cs",
            "Assets/ArcaneEngine/Scripts/Presentation21/GeneratedAssetRuntime21.cs",
            "Assets/ArcaneEngine/Scripts/Presentation21/MorphologyRuntime21.cs",
            "Assets/ArcaneEngine/Scripts/Presentation21/MorphologyPresentationDirector21.cs",
            "Assets/ArcaneEngine/Scripts/Presentation21/ProceduralAudioHaptics21.cs",
            "Assets/ArcaneEngine/Scripts/Presentation21/CameraOverlayReference21.cs",
            "Assets/ArcaneEngine/Scripts/Presentation21/SpellForgeMorphologyPreview21.cs",
            "Assets/ArcaneEngine/Scripts/Presentation21/AutomatedCapture21.cs"
        };

        [MenuItem("Arcane Engine/2.1/Validate Patch 2.1.0")]
        public static void ValidatePatch()
        {
            List<string> failures = new List<string>();
            List<string> warnings = new List<string>();

            for (int i = 0; i < RequiredRuntimeFiles.Length; i++)
                if (!File.Exists(RequiredRuntimeFiles[i])) failures.Add("Missing runtime file: " + RequiredRuntimeFiles[i]);

            ValidateMarker(
                "Assets/ArcaneEngine/Scripts/Presentation/PresentationAPI.cs",
                "SpellMorphologyPresentation21",
                failures);
            ValidateMarker(
                "Assets/ArcaneEngine/Scripts/UI/V21ProductUI.cs",
                "SpellForgeMorphologyPreview21.Build",
                failures);
            ValidateMarker(
                "Assets/ArcaneEngine/Scripts/UI/V21ProductUI.WorkshopDrag.cs",
                "SpellForgeMorphologyPreview21.DiffSummary",
                failures);

            List<string> assetFailures = Patch210AssetBuilder.Validate();
            failures.AddRange(assetFailures);

            string reactionDefinitions = "Assets/ArcaneEngine/Scripts/Combat/ElementalReactionCodex.Generated.cs";
            string reactionGraphs = "Assets/ArcaneEngine/Scripts/Combat/ElementalReactionMechanicCodex.Generated.cs";
            if (File.Exists(reactionDefinitions))
            {
                int definitions = Count(File.ReadAllText(reactionDefinitions), "new ElementalReactionDefinition(");
                if (definitions != 120) failures.Add("Expected 120 reaction definitions; found " + definitions + ".");
            }
            else failures.Add("Elemental reaction definitions are missing.");

            if (File.Exists(reactionGraphs))
            {
                string text = File.ReadAllText(reactionGraphs);
                int graphs = Count(text, "new ReactionMechanicPlan(");
                int mechanics = Count(text, "new ReactionMechanicSpec(");
                if (graphs != 120) failures.Add("Expected 120 reaction mechanic graphs; found " + graphs + ".");
                if (mechanics < 1000) failures.Add("Reaction mechanic instruction payload appears incomplete: " + mechanics + ".");
            }
            else failures.Add("Elemental reaction mechanic graphs are missing.");

            if (!Application.isPlaying)
                warnings.Add("Runtime contract, phase, owner, interruption and cleanup checks require Play Mode.");
            else
            {
                SpellVisualContract21 contract = SpellMorphologyPresentation21.LastContract;
                if (contract == null) warnings.Add("No spell contract has been compiled in the current Play Mode session.");
                else
                {
                    if (contract.schemaVersion != SpellVisualContract21.CurrentSchemaVersion) failures.Add("Active contract schema is stale.");
                    warnings.AddRange(contract.validationWarnings);
                    if (contract.lifecycle.Count < 5) failures.Add("Active lifecycle graph is incomplete.");
                    if (contract.bodyParts.Count < 3) failures.Add("Active generated body is incomplete.");
                }
            }

            if (failures.Count == 0)
            {
                string message = "Patch 2.1.0 validation passed.\n" +
                                 "Runtime files: " + RequiredRuntimeFiles.Length + "\n" +
                                 "Generated asset validation: passed" +
                                 (warnings.Count == 0 ? string.Empty : "\nWarnings:\n- " + string.Join("\n- ", warnings.ToArray()));
                Debug.Log(message);
                EditorUtility.DisplayDialog("Patch 2.1.0 Validation", message, "OK");
            }
            else
            {
                string message = "Patch 2.1.0 validation failed:\n- " + string.Join("\n- ", failures.ToArray());
                if (warnings.Count > 0) message += "\nWarnings:\n- " + string.Join("\n- ", warnings.ToArray());
                Debug.LogError(message);
                EditorUtility.DisplayDialog("Patch 2.1.0 Validation", message, "OK");
            }
        }

        [MenuItem("Arcane Engine/2.1/Run Automated Visual Capture")]
        public static void RunCapture()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog(
                    "Patch 2.1 Capture",
                    "Enter Play Mode, load a scene with GameWorld and the player, then run this command again.",
                    "OK");
                return;
            }
            Patch210AutomatedCapture21.Begin();
        }

        private static void ValidateMarker(string path, string marker, List<string> failures)
        {
            if (!File.Exists(path))
            {
                failures.Add("Missing integration file: " + path);
                return;
            }
            if (File.ReadAllText(path).IndexOf(marker, StringComparison.Ordinal) < 0)
                failures.Add("Integration marker is missing: " + marker + " in " + path);
        }

        private static int Count(string text, string marker)
        {
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(marker, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += marker.Length;
            }
            return count;
        }
    }

    public sealed class SpellVisualLab21 : EditorWindow
    {
        private SpellPhase21 _phase = SpellPhase21.Charge;
        private float _phaseProgress = 0.5f;
        private Vector2 _scroll;
        private bool _autoRefresh = true;

        [MenuItem("Arcane Engine/2.1/Open Spell Visual Lab")]
        public static void Open()
        {
            SpellVisualLab21 window = GetWindow<SpellVisualLab21>();
            window.titleContent = new GUIContent("Spell Visual Lab 2.1");
            window.minSize = new Vector2(520f, 640f);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            ClearPhaseOverride();
        }

        private void OnEditorUpdate()
        {
            if (_autoRefresh) Repaint();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Patch 2.1 Spell Visual Lab", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "The lab inspects the active generated contract, scrubs lifecycle phases, launches reference previews and records deterministic screenshots.",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Assets")) Patch210AssetBuilder.GenerateAll();
            if (GUILayout.Button("Validate Patch")) Patch210Diagnostics.ValidatePatch();
            if (GUILayout.Button("Reference Preview")) RunReferencePreview();
            if (GUILayout.Button("Capture")) Patch210Diagnostics.RunCapture();
            EditorGUILayout.EndHorizontal();

            _autoRefresh = EditorGUILayout.Toggle("Auto refresh", _autoRefresh);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Phase scrubber", EditorStyles.boldLabel);
            _phase = (SpellPhase21)EditorGUILayout.EnumPopup("Phase", _phase);
            _phaseProgress = EditorGUILayout.Slider("Normalized progress", _phaseProgress, 0f, 1f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply to active spell bodies")) ApplyPhaseOverride();
            if (GUILayout.Button("Resume runtime phases")) ClearPhaseOverride();
            EditorGUILayout.EndHorizontal();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            SpellVisualContract21 contract = SpellMorphologyPresentation21.LastContract;
            if (contract == null)
            {
                EditorGUILayout.HelpBox("No spell visual contract is active. Enter Play Mode and compile or cast a spell.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.TextArea(contract.DebugSummary, GUILayout.MinHeight(170f));
                EditorGUILayout.LabelField("Core invariants", EditorStyles.boldLabel);
                for (int i = 0; i < contract.coreInvariants.Count; i++)
                {
                    CoreIdentityInvariant21 invariant = contract.coreInvariants[i];
                    EditorGUILayout.LabelField((invariant.required ? "Required" : "Optional") + " · " + invariant.description);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Ordered Rune graph", EditorStyles.boldLabel);
                for (int i = 0; i < contract.runeGraph.Count; i++)
                {
                    RuneOperatorNode21 node = contract.runeGraph[i];
                    EditorGUILayout.LabelField(node.ToString());
                    EditorGUILayout.LabelField("    " + node.deliveryImplementation, EditorStyles.miniLabel);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Lifecycle", EditorStyles.boldLabel);
                for (int i = 0; i < contract.lifecycle.Count; i++)
                {
                    LifecycleNode21 phase = contract.lifecycle[i];
                    EditorGUILayout.LabelField(
                        phase.phase + " · " + phase.source +
                        " · " + phase.normalizedStart.ToString("0.00") +
                        "–" + (phase.normalizedStart + phase.normalizedDuration).ToString("0.00"));
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Interaction rules", EditorStyles.boldLabel);
                if (contract.interactionRules.Count == 0) EditorGUILayout.LabelField("No specialized compound rules.");
                for (int i = 0; i < contract.interactionRules.Count; i++) EditorGUILayout.LabelField(contract.interactionRules[i], EditorStyles.wordWrappedLabel);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Visual cost", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(contract.cost.ToString(), EditorStyles.wordWrappedLabel);
                if (contract.validationWarnings.Count > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Warnings", EditorStyles.boldLabel);
                    for (int i = 0; i < contract.validationWarnings.Count; i++)
                        EditorGUILayout.HelpBox(contract.validationWarnings[i], MessageType.Warning);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private static void ApplyPhaseOverride()
        {
            GeneratedSpellMorphologyHost21[] hosts = UnityEngine.Object.FindObjectsByType<GeneratedSpellMorphologyHost21>();
            SpellVisualLab21 window = GetWindow<SpellVisualLab21>();
            for (int i = 0; i < hosts.Length; i++)
                if (hosts[i] != null) hosts[i].SetDebugPhase(window._phase, window._phaseProgress);
            Debug.Log("Patch 2.1 phase scrubber applied " + window._phase + " to " + hosts.Length + " active spell bodies.");
        }

        private static void ClearPhaseOverride()
        {
            if (!EditorApplication.isPlaying) return;
            GeneratedSpellMorphologyHost21[] hosts = UnityEngine.Object.FindObjectsByType<GeneratedSpellMorphologyHost21>();
            for (int i = 0; i < hosts.Length; i++) if (hosts[i] != null) hosts[i].ClearDebugPhase();
        }

        private static void RunReferencePreview()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("Spell Visual Lab", "Enter Play Mode before creating the in-game reference preview.", "OK");
                return;
            }
            Patch210ReferenceRuntime21 reference = UnityEngine.Object.FindAnyObjectByType<Patch210ReferenceRuntime21>();
            if (reference == null) reference = MorphologyPresentationDirector21.Instance.gameObject.GetComponent<Patch210ReferenceRuntime21>();
            if (reference != null) reference.BuildReferencePreview();
        }
    }
}
#endif
