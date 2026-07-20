#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine
{
    public static class Patch300Alpha1Validator
    {
        private const string MarkerPath = "Assets/ArcaneEngine/PATCH_3_0_0_ALPHA_1.txt";
        private const string GameWorldPath = "Assets/ArcaneEngine/Scripts/Core/GameWorld.cs";

        [MenuItem("Arcane Engine/3.0/Validate Endgame Foundation")]
        public static void Validate()
        {
            ArpgContent30.Ensure();
            string contentFailure = ArpgContent30.Validate();
            if (!string.IsNullOrEmpty(contentFailure))
            {
                Debug.LogError("Arcane Engine 3.0 content validation failed:\n" + contentFailure);
                return;
            }

            if (!File.Exists(MarkerPath))
            {
                Debug.LogError("Patch marker is missing: " + MarkerPath);
                return;
            }
            if (!File.Exists(GameWorldPath) || !File.ReadAllText(GameWorldPath).Contains("ARCANE_PATCH_300A1_PERSISTENT_STATS"))
            {
                Debug.LogError("GameWorld persistent-stat hook is missing.");
                return;
            }

            int classes = ArpgContent30.Classes.Count;
            int ascendancies = ArpgContent30.Ascendancies.Count;
            int constellations = ArpgContent30.Constellations.Count;
            int constellationNodes = ArpgContent30.Constellations.Sum(value => value.nodes.Count);
            int maps = ArpgContent30.Maps.Count;
            int tiers = ArpgContent30.Maps.Select(value => value.tier).Distinct().Count();
            int bases = ArpgContent30.ItemBases.Count;
            int affixes = ArpgContent30.Affixes.Count;
            int mapAffixes = ArpgContent30.MapAffixes.Count;

            Debug.Log(
                "Arcane Engine 3.0 Alpha 1 validation passed.\n" +
                "Classes: " + classes + "\n" +
                "Ascendancies: " + ascendancies + "\n" +
                "Constellations: " + constellations + "\n" +
                "Constellation nodes: " + constellationNodes + "\n" +
                "Atlas maps: " + maps + "\n" +
                "Standard map tiers: " + tiers + "\n" +
                "Item bases: " + bases + "\n" +
                "Tiered affixes: " + affixes + "\n" +
                "Map affixes: " + mapAffixes + "\n" +
                "Currency actions: " + Enum.GetValues(typeof(ArpgCurrency30)).Length + "\n" +
                "Runtime entry: F7");
        }

        [MenuItem("Arcane Engine/3.0/Open Persistent Profile Folder")]
        public static void OpenProfileFolder()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        [MenuItem("Arcane Engine/3.0/Reset ARPG Profile", true)]
        private static bool CanResetProfile()
        {
            return !Application.isPlaying;
        }

        [MenuItem("Arcane Engine/3.0/Reset ARPG Profile")]
        public static void ResetProfile()
        {
            if (!EditorUtility.DisplayDialog(
                    "Reset Arcane Engine 3.0 profile",
                    "This deletes only the new ARPG profile and its backup. Existing legacy profiles and roguelite progression are not deleted.",
                    "Reset",
                    "Cancel")) return;

            if (File.Exists(ArpgProfileStore30.ProfilePath)) File.Delete(ArpgProfileStore30.ProfilePath);
            if (File.Exists(ArpgProfileStore30.BackupPath)) File.Delete(ArpgProfileStore30.BackupPath);
            Debug.Log("Arcane Engine 3.0 ARPG profile reset. Enter Play Mode to create a new level-zero character.");
        }
    }
}
#endif
