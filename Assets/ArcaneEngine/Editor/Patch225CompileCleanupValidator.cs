#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    public static class Patch225CompileCleanupValidator
    {
        [MenuItem("Arcane Engine/2.2.5/Validate Compile Cleanup Hotfix")]
        public static void Validate()
        {
            string root = "Assets/ArcaneEngine";
            Check(root + "/PATCH_2_2_5.txt", "compile-cleanup-hotfix-22500");
            Check(root + "/Scripts/Combat/ElementalReactionField.cs", "ARCANE_PATCH_225_FIELD_CLEAR_FIX");
            Check(root + "/Scripts/Spells/SpellEffectLifecycle224.cs", "ARCANE_PATCH_225_FIND_API_FIX");
            Check(root + "/Scripts/Presentation21/MorphologyPresentationDirector21.cs", "ARCANE_PATCH_225_UNUSED_FIELD_FIX");

            string lifecycle = File.ReadAllText(root + "/Scripts/Spells/SpellEffectLifecycle224.cs");
            if (lifecycle.Contains("FindObjectsSortMode"))
                throw new System.Exception("Patch 2.2.5 validation failed: obsolete FindObjectsSortMode remains in lifecycle cleanup.");

            string field = File.ReadAllText(root + "/Scripts/Combat/ElementalReactionField.cs");
            if (!field.Contains("CleanupList();"))
                throw new System.Exception("Patch 2.2.5 validation failed: ElementalReactionField cleanup does not call CleanupList().");

            string director = File.ReadAllText(root + "/Scripts/Presentation21/MorphologyPresentationDirector21.cs");
            if (director.Contains("_lastImportantEvent"))
                throw new System.Exception("Patch 2.2.5 validation failed: unused presentation field remains.");

            Debug.Log("Arcane Engine Patch 2.2.5 validation passed: field cleanup, Unity 6.5 object searches, and warning cleanup are installed.");
        }

        private static void Check(string path, string marker)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Patch 2.2.5 validation failed. Missing file: " + path);
            if (!File.ReadAllText(path).Contains(marker))
                throw new System.Exception("Patch 2.2.5 validation failed. Missing marker " + marker + " in " + path);
        }
    }
}
#endif
