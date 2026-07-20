#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.Editor
{
    /// <summary>
    /// Patch 2.1.1 repair guard for authored ScriptableObject content.
    /// It runs only after compilation/import is idle and validates that Unity can
    /// resolve the three persistent content types to same-named MonoScript assets.
    /// </summary>
    [InitializeOnLoad]
    public static class Patch211AuthoredContentRepair
    {
        private const string Root = "Assets/ArcaneEngine/Resources/V21Content";
        private const string Marker = "Assets/ArcaneEngine/PATCH_2_1_1_CONTENT_REBUILT.txt";
        private const string SessionKey = "ArcaneEngine.Patch211.ContentRepairQueued";

        static Patch211AuthoredContentRepair()
        {
            if (SessionState.GetBool(SessionKey, false))
                return;

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += RepairWhenReady;
        }

        [MenuItem("Arcane Engine/2.1.1/Repair Authored Content")]
        public static void RepairNow()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                Debug.LogWarning(
                    "Patch 2.1.1 repair is waiting for Unity compilation/import to finish.");
                EditorApplication.delayCall += RepairWhenReady;
                return;
            }

            RepairInternal(force: true);
        }

        [MenuItem("Arcane Engine/2.1.1/Validate Hotfix")]
        public static void ValidateHotfix()
        {
            bool scriptsValid = ValidateScriptTypes(logErrors: true);
            int cores = Count("t:SpellCoreDefinition", Root + "/Spells");
            int runes = Count("t:SpellModifierDefinition", Root + "/Runes");
            int items = Count("t:ItemDefinition", Root + "/Items");

            if (scriptsValid && cores > 0 && runes > 0 && items > 0)
            {
                Debug.Log(
                    "Patch 2.1.1 valid: ScriptableObject script assets resolve correctly " +
                    "and authored content is loadable (" + cores + " cores, " +
                    runes + " runes, " + items + " items). Particle-pool fixes are compiled.");
            }
            else
            {
                Debug.LogError(
                    "Patch 2.1.1 validation failed. Run " +
                    "Arcane Engine > 2.1.1 > Repair Authored Content after compilation completes.");
            }
        }

        private static void RepairWhenReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += RepairWhenReady;
                return;
            }

            RepairInternal(force: false);
        }

        private static void RepairInternal(bool force)
        {
            if (!ValidateScriptTypes(logErrors: true))
            {
                Debug.LogError(
                    "Patch 2.1.1 cannot rebuild authored content because one or more " +
                    "ScriptableObject classes do not resolve to their same-named script asset.");
                return;
            }

            bool contentValid =
                Count("t:SpellCoreDefinition", Root + "/Spells") > 0 &&
                Count("t:SpellModifierDefinition", Root + "/Runes") > 0 &&
                Count("t:ItemDefinition", Root + "/Items") > 0;

            if (!force && contentValid && File.Exists(Marker))
                return;

            if (AssetDatabase.IsValidFolder(Root) || Directory.Exists(Root))
            {
                if (!AssetDatabase.DeleteAsset(Root) && Directory.Exists(Root))
                    Directory.Delete(Root, true);
            }

            string meta = Root + ".meta";
            if (File.Exists(meta))
                File.Delete(meta);

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            V21ContentBuilder.RebuildAll();

            int coreCount = Count("t:SpellCoreDefinition", Root + "/Spells");
            int runeCount = Count("t:SpellModifierDefinition", Root + "/Runes");
            int itemCount = Count("t:ItemDefinition", Root + "/Items");

            if (coreCount == 0 || runeCount == 0 || itemCount == 0)
            {
                Debug.LogError(
                    "Patch 2.1.1 rebuilt authored content, but one or more required " +
                    "asset families are still empty.");
                return;
            }

            File.WriteAllText(
                Marker,
                "Patch 2.1.1 authored content rebuilt successfully.\n" +
                "Spell cores: " + coreCount + "\n" +
                "Runes: " + runeCount + "\n" +
                "Items: " + itemCount + "\n");
            AssetDatabase.ImportAsset(Marker, ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets();

            Debug.Log(
                "Patch 2.1.1 authored-content repair complete: " + coreCount +
                " spell cores, " + runeCount + " runes and " + itemCount + " items.");
        }

        private static int Count(string filter, string folder)
        {
            if (!AssetDatabase.IsValidFolder(folder))
                return 0;
            return AssetDatabase.FindAssets(filter, new[] { folder }).Length;
        }

        private static bool ValidateScriptTypes(bool logErrors)
        {
            return ValidateType<SpellCoreDefinition>(logErrors) &
                   ValidateType<SpellModifierDefinition>(logErrors) &
                   ValidateType<ItemDefinition>(logErrors);
        }

        private static bool ValidateType<T>(bool logErrors)
            where T : ScriptableObject
        {
            T instance = ScriptableObject.CreateInstance<T>();
            MonoScript script = MonoScript.FromScriptableObject(instance);
            UnityEngine.Object.DestroyImmediate(instance);

            bool valid = script != null && script.GetClass() == typeof(T);
            if (!valid && logErrors)
            {
                Debug.LogError(
                    "Patch 2.1.1: Unity cannot resolve " + typeof(T).FullName +
                    " to a same-named MonoScript asset.");
            }
            return valid;
        }
    }
}
#endif
