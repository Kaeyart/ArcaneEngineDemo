#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.Editor
{
    /// <summary>
    /// Patch 2.2.2 repair and validation for the remaining authored-content
    /// ScriptableObject families. Unity serializes persistent ScriptableObjects
    /// through their MonoScript identity, so every persistent type must live in
    /// a same-named script file.
    /// </summary>
    [InitializeOnLoad]
    public static class Patch222AuthoredContentRepair
    {
        private const string Root = "Assets/ArcaneEngine/Resources/V21Content";
        private const string Marker = "Assets/ArcaneEngine/PATCH_2_2_2_CONTENT_REBUILT.txt";
        private const string SessionKey = "ArcaneEngine.Patch222.ContentRepairQueued";

        static Patch222AuthoredContentRepair()
        {
            if (SessionState.GetBool(SessionKey, false))
                return;

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += RepairWhenReady;
        }

        [MenuItem("Arcane Engine/2.2.2/Repair Authored Content")]
        public static void RepairNow()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                Debug.LogWarning(
                    "Patch 2.2.2 repair is waiting for Unity compilation/import to finish.");
                EditorApplication.delayCall += RepairWhenReady;
                return;
            }

            RepairInternal(force: true);
        }

        [MenuItem("Arcane Engine/2.2.2/Validate Hotfix")]
        public static void ValidateHotfix()
        {
            bool scriptsValid = ValidateScriptTypes(logErrors: true);
            ContentCounts counts = ReadCounts();
            bool contentValid = counts.IsValid;

            if (scriptsValid && contentValid)
            {
                Debug.Log(
                    "Patch 2.2.2 valid: all persistent V21 authored-content types " +
                    "resolve to same-named MonoScript assets and generated content is loadable. " +
                    counts.Summary);
            }
            else
            {
                Debug.LogError(
                    "Patch 2.2.2 validation failed. Run " +
                    "Arcane Engine > 2.2.2 > Repair Authored Content after compilation completes. " +
                    counts.Summary);
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
                    "Patch 2.2.2 cannot rebuild authored content because one or more " +
                    "ScriptableObject classes do not resolve to a same-named MonoScript asset.");
                return;
            }

            ContentCounts before = ReadCounts();
            if (!force && before.IsValid)
            {
                WriteMarker(before);
                return;
            }

            if (AssetDatabase.IsValidFolder(Root) || Directory.Exists(Root))
            {
                if (!AssetDatabase.DeleteAsset(Root) && Directory.Exists(Root))
                    Directory.Delete(Root, true);
            }

            string rootMeta = Root + ".meta";
            if (File.Exists(rootMeta))
                File.Delete(rootMeta);

            if (File.Exists(Marker))
                File.Delete(Marker);
            if (File.Exists(Marker + ".meta"))
                File.Delete(Marker + ".meta");

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            V21ContentBuilder.RebuildAll();

            ContentCounts after = ReadCounts();
            if (!after.IsValid)
            {
                Debug.LogError(
                    "Patch 2.2.2 rebuilt authored content, but one or more required " +
                    "asset families are still empty. " + after.Summary);
                return;
            }

            WriteMarker(after);
            AssetDatabase.SaveAssets();

            Debug.Log(
                "Patch 2.2.2 authored-content repair complete. " + after.Summary);
        }

        private static void WriteMarker(ContentCounts counts)
        {
            File.WriteAllText(
                Marker,
                "Patch 2.2.2 authored content rebuilt successfully.\n" +
                counts.Summary + "\n");
            AssetDatabase.ImportAsset(Marker, ImportAssetOptions.ForceSynchronousImport);
        }

        private static ContentCounts ReadCounts()
        {
            return new ContentCounts
            {
                spellCores = Count("t:SpellCoreDefinition", Root + "/Spells"),
                runes = Count("t:SpellModifierDefinition", Root + "/Runes"),
                items = Count("t:ItemDefinition", Root + "/Items"),
                relics = Count("t:RelicDefinition", Root + "/Relics"),
                roomLayouts = Count("t:V21RoomLayoutAsset", Root + "/Rooms"),
                enemies = Count("t:V21EnemyContentAsset", Root + "/Enemies"),
                audioEvents = Count("t:V21AudioEventAsset", Root + "/Audio"),
                affixes = Count("t:V21AffixContentAsset", Root + "/Affixes"),
                roomDefinitions = Count("t:V21RoomDefinitionAsset", Root + "/RoomDefinitions"),
                shops = Count("t:V21ShopServiceAsset", Root + "/Shops"),
                rewards = Count("t:V21RewardDefinitionAsset", Root + "/Rewards")
            };
        }

        private static int Count(string filter, string folder)
        {
            if (!AssetDatabase.IsValidFolder(folder))
                return 0;
            return AssetDatabase.FindAssets(filter, new[] { folder }).Length;
        }

        private static bool ValidateScriptTypes(bool logErrors)
        {
            return ValidateType<V21RoomLayoutAsset>(logErrors) &
                   ValidateType<V21EnemyContentAsset>(logErrors) &
                   ValidateType<V21AudioEventAsset>(logErrors) &
                   ValidateType<V21AffixContentAsset>(logErrors) &
                   ValidateType<V21RoomDefinitionAsset>(logErrors) &
                   ValidateType<V21ShopServiceAsset>(logErrors) &
                   ValidateType<V21RewardDefinitionAsset>(logErrors);
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
                    "Patch 2.2.2: Unity cannot resolve " + typeof(T).FullName +
                    " to a same-named MonoScript asset.");
            }
            return valid;
        }

        private struct ContentCounts
        {
            public int spellCores;
            public int runes;
            public int items;
            public int relics;
            public int roomLayouts;
            public int enemies;
            public int audioEvents;
            public int affixes;
            public int roomDefinitions;
            public int shops;
            public int rewards;

            public bool IsValid
            {
                get
                {
                    return spellCores > 0 &&
                           runes > 0 &&
                           items > 0 &&
                           relics > 0 &&
                           roomLayouts >= 36 &&
                           enemies > 0 &&
                           audioEvents >= 24 &&
                           affixes > 0 &&
                           roomDefinitions > 0 &&
                           shops > 0 &&
                           rewards > 0;
                }
            }

            public string Summary
            {
                get
                {
                    return
                        "Cores=" + spellCores +
                        ", Runes=" + runes +
                        ", Items=" + items +
                        ", Relics=" + relics +
                        ", RoomLayouts=" + roomLayouts +
                        ", Enemies=" + enemies +
                        ", AudioEvents=" + audioEvents +
                        ", Affixes=" + affixes +
                        ", RoomDefinitions=" + roomDefinitions +
                        ", Shops=" + shops +
                        ", Rewards=" + rewards + ".";
                }
            }
        }
    }
}
#endif
