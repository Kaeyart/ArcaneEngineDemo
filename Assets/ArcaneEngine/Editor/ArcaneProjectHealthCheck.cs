#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.Editor
{
    public static class ArcaneProjectHealthCheck
    {
        private const string GeneratedContentRoot = "Assets/ArcaneEngine/Resources/V21Content";

        [MenuItem("Arcane Engine/Diagnostics/Run Project Health Check")]
        public static void Run()
        {
            List<string> errors = new List<string>();
            List<string> warnings = new List<string>();

            CheckGeneratedAssets(errors);
            CheckLargeScripts(warnings);
            CheckProjectVersion(warnings);
            CheckRequiredPaths(errors);

            foreach (string warning in warnings)
                Debug.LogWarning("[Arcane Health] " + warning);

            foreach (string error in errors)
                Debug.LogError("[Arcane Health] " + error);

            string summary = $"Arcane Engine health check: {errors.Count} error(s), {warnings.Count} warning(s).";
            if (errors.Count == 0)
                Debug.Log("[Arcane Health] " + summary);

            EditorUtility.DisplayDialog(
                "Arcane Engine Health Check",
                summary + "\n\nSee the Console for details.",
                "OK");
        }

        private static void CheckGeneratedAssets(List<string> errors)
        {
            if (!Directory.Exists(GeneratedContentRoot))
            {
                errors.Add("Generated content is missing. Run Arcane Engine > 2.1 > Rebuild Authored Content.");
                return;
            }

            foreach (string path in Directory.EnumerateFiles(GeneratedContentRoot, "*.asset", SearchOption.AllDirectories))
            {
                string text;
                try
                {
                    text = File.ReadAllText(path);
                }
                catch (Exception exception)
                {
                    errors.Add($"Cannot read generated asset '{path}': {exception.Message}");
                    continue;
                }

                if (text.Contains("m_Script: {fileID: 0}"))
                    errors.Add($"Generated asset has a missing script reference: {path}");
            }
        }

        private static void CheckLargeScripts(List<string> warnings)
        {
            const int warningLineCount = 900;
            string scriptsRoot = "Assets/ArcaneEngine/Scripts";
            if (!Directory.Exists(scriptsRoot))
                return;

            foreach (string path in Directory.EnumerateFiles(scriptsRoot, "*.cs", SearchOption.AllDirectories))
            {
                int lines = File.ReadLines(path).Count();
                if (lines >= warningLineCount)
                    warnings.Add($"Large script ({lines} lines): {path}. Extract one responsibility at a time.");
            }
        }

        private static void CheckProjectVersion(List<string> warnings)
        {
            const string versionPath = "ProjectSettings/ProjectVersion.txt";
            if (!File.Exists(versionPath))
                return;

            string version = File.ReadAllText(versionPath);
            if (!version.Contains("6000.5.2f1"))
                warnings.Add("Project editor version differs from the validated 6000.5.2f1 baseline.");
        }

        private static void CheckRequiredPaths(List<string> errors)
        {
            string[] required =
            {
                "Assets/ArcaneEngine/Scripts/Core/GameWorld.cs",
                "Assets/ArcaneEngine/Scripts/Core/PlayerController.cs",
                "Assets/ArcaneEngine/Scripts/Spells/HexBoard.cs",
                "Assets/ArcaneEngine/Scripts/UI/V21ProductUI.cs",
                "Assets/ArcaneEngine/Scenes/Main.unity"
            };

            foreach (string path in required)
            {
                if (!File.Exists(path))
                    errors.Add("Required project file is missing: " + path);
            }
        }
    }
}
#endif
