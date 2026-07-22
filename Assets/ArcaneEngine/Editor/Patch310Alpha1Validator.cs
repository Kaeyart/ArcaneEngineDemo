using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    public static class Patch310Alpha1Validator
    {
        private const string MarkerPath = "Assets/ArcaneEngine/PATCH_3_1_0_ALPHA_1.txt";
        private const string GameWorldPath = "Assets/ArcaneEngine/Scripts/Core/GameWorld.cs";

        [MenuItem("Arcane Engine/3.1/Validate First Descent")]
        public static void Validate()
        {
            try
            {
                ArpgContent30.Ensure();
                string contentFailure = ArpgContent30.Validate();
                if (!string.IsNullOrEmpty(contentFailure)) throw new InvalidOperationException(contentFailure);
                if (!File.Exists(MarkerPath)) throw new FileNotFoundException("Patch marker is missing.", MarkerPath);
                if (!File.Exists(GameWorldPath)) throw new FileNotFoundException("GameWorld.cs is missing.", GameWorldPath);

                string gameWorld = File.ReadAllText(GameWorldPath);
                Require(gameWorld.Contains("ARCANE_PATCH_300A1_PERSISTENT_STATS"), "GameWorld persistent-stat hook is missing.");
                Require(gameWorld.Contains("ArpgStatHooks30.ApplyPersistentStats(Stats)"), "GameWorld does not invoke persistent 3.x stats.");
                Require(ArpgProfileStore30.CurrentSchema == 31000, "Profile schema is not 31000.");
                Require(ArpgRosterStore31.MaximumSlots == 8, "Character roster does not expose eight slots.");
                Require(ArpgContent30.Classes.Count == 3, "Expected three playable classes.");
                Require(ArpgContent30.Ascendancies.Count == 9, "Expected nine Ascendancy previews.");
                Require(ArpgContent30.Constellations.Count == 6, "Expected six playable starter Constellations.");
                Require(ArpgContent30.Constellations.Sum(value => value.nodes.Count) >= 60, "Expected at least sixty implemented Stars.");
                Require(ArpgContent30.ItemBases.Count == 36, "Expected thirty-six item bases.");
                Require(ArpgContent30.Affixes.Count == 72, "Expected seventy-two affix definitions.");
                Require(ArpgContent30.Maps.Count == 80, "Expected eighty map definitions in the forty-tier model.");
                Require(ArpgContent30.Maps.Count(value => value.playableIn31) == 12, "Expected twelve playable Tier 0–5 map nodes.");
                Require(ArpgContent30.MapAffixes.Count == 12, "Expected twelve White Map modifiers.");
                Require(ArpgContent30.MonsterFamilies.Count == 3, "Expected three monster families.");
                Require(ArpgContent30.MonsterVariants.Count == 18, "Expected eighteen enemy variants.");
                Require(ArpgContent30.Bosses.Count == 6, "Expected six White Map bosses.");

                string report =
                    "ARCANE ENGINE 3.1.0-alpha.1 · FIRST DESCENT\n\n" +
                    "Content validation: PASS\n" +
                    "Character slots: " + ArpgRosterStore31.MaximumSlots + "\n" +
                    "Classes / Ascendancies: " + ArpgContent30.Classes.Count + " / " + ArpgContent30.Ascendancies.Count + "\n" +
                    "Starter Constellations / Stars: " + ArpgContent30.Constellations.Count + " / " + ArpgContent30.Constellations.Sum(value => value.nodes.Count) + "\n" +
                    "Item bases / Affixes: " + ArpgContent30.ItemBases.Count + " / " + ArpgContent30.Affixes.Count + "\n" +
                    "Map model / Playable nodes / Modifiers: " + ArpgContent30.Maps.Count + " / " + ArpgContent30.Maps.Count(value => value.playableIn31) + " / " + ArpgContent30.MapAffixes.Count + "\n" +
                    "Monster families / Variants / Bosses: " + ArpgContent30.MonsterFamilies.Count + " / " + ArpgContent30.MonsterVariants.Count + " / " + ArpgContent30.Bosses.Count + "\n\n" +
                    "Static content and integration checks passed. Complete the supplied Play Mode checklist before treating runtime behavior as validated.";
                Debug.Log(report);
                EditorUtility.DisplayDialog("First Descent validation passed", report, "OK");
            }
            catch (Exception exception)
            {
                Debug.LogError("Arcane Engine 3.1 First Descent validation failed:\n" + exception);
                EditorUtility.DisplayDialog("First Descent validation failed", exception.Message, "OK");
                throw;
            }
        }

        [MenuItem("Arcane Engine/3.1/Open Character Save Folder")]
        public static void OpenSaveFolder()
        {
            string path = Path.Combine(Application.persistentDataPath, "ArcaneEngine31");
            Directory.CreateDirectory(path);
            EditorUtility.RevealInFinder(path);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
