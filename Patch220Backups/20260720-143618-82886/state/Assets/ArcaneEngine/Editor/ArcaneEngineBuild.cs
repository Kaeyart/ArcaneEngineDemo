using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ArcaneEngine.Editor
{
    public static class ArcaneEngineBuild
    {
        private const string Version = "2.1.0-demo";
        private const string MainScene = "Assets/ArcaneEngine/Scenes/Main.unity";

        [MenuItem("Arcane Engine/Validate 2.1.0 Demo")]
        public static void ValidateDemo()
        {
            DemoCatalog.Ensure();
            int failures = RuntimeValidation.ValidateCombatAndContent();
            failures += ProceduralVisualValidation.Validate();
            failures += VisualCorrectiveContractValidation.Validate();
            failures += V21AcceptanceValidation.Validate();
            if (failures > 0) throw new BuildFailedException("Arcane Engine validation found " + failures + " failure(s). Check the Console.");
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(MainScene) == null) throw new BuildFailedException("Required scene is missing: " + MainScene);
            Debug.Log("Arcane Engine 2.1.0 source and content validation passed. Complete automated Play Mode and manual hardware acceptance before release sign-off.");
        }

        [MenuItem("Arcane Engine/Build/Linux x86_64 Release")]
        public static void BuildLinux()
        {
            ValidateDemo();
            string outputRoot = ArgumentValue("-arcaneBuildPath");
            if (string.IsNullOrEmpty(outputRoot)) outputRoot = Path.GetFullPath("Builds/Linux");
            Directory.CreateDirectory(outputRoot);
            string executable = Path.Combine(outputRoot, "ArcaneEngine.x86_64");

            PlayerSettings.productName = "Arcane Engine - The Relic Forge";
            PlayerSettings.bundleVersion = "2.1.0";
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(MainScene, true) };

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { MainScene },
                locationPathName = executable,
                target = BuildTarget.StandaloneLinux64,
                options = HasArgument("-arcaneDevelopmentBuild") ? BuildOptions.Development : BuildOptions.None
            };
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException("Linux build failed: " + report.summary.result + " · " + report.summary.totalErrors + " errors.");

            string manifest = "Arcane Engine - The Relic Forge\nVersion " + Version + "\nUnity " + Application.unityVersion +
                              "\nBuilt UTC " + DateTime.UtcNow.ToString("o") + "\nTarget Linux x86_64\nSize " + report.summary.totalSize + " bytes\n";
            File.WriteAllText(Path.Combine(outputRoot, "BUILD_INFO.txt"), manifest);
            File.WriteAllText(Path.Combine(outputRoot, "RUN_ARCANE_ENGINE.sh"), "#!/usr/bin/env bash\nDIR=\"$(cd \"$(dirname \"${BASH_SOURCE[0]}\")\" && pwd)\"\nexec \"$DIR/ArcaneEngine.x86_64\" \"$@\"\n");
            Debug.Log("Arcane Engine Linux release built at " + outputRoot);
        }

        private static bool HasArgument(string key)
        {
            return Environment.GetCommandLineArgs().Any(value => string.Equals(value, key, StringComparison.OrdinalIgnoreCase));
        }

        private static string ArgumentValue(string key)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++) if (string.Equals(args[i], key, StringComparison.OrdinalIgnoreCase)) return args[i + 1];
            return null;
        }
    }
}
