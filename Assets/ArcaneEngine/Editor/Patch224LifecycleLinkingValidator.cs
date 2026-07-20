#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    public static class Patch224LifecycleLinkingValidator
    {
        [MenuItem("Arcane Engine/2.2.4/Validate Hotfix")]
        public static void Validate()
        {
            string root = Application.dataPath + "/ArcaneEngine";
            Check(root + "/Scripts/Spells/HexBoard.cs", "ARCANE_PATCH_224_TOUCH_CONNECTIVITY");
            Check(root + "/Scripts/Spells/HexBoard.cs", "BuildTouchDistances224");
            Check(root + "/Scripts/Spells/SpellExecution.cs", "ARCANE_PATCH_224_LINK_DEPTH");
            Check(root + "/Scripts/Spells/SpellLinks.cs", "ARCANE_PATCH_224_LINK_RUNTIME");
            Check(root + "/Scripts/Spells/SpellEffectLifecycle224.cs", "ARCANE_PATCH_224_LIFECYCLE_OWNERSHIP");
            Check(root + "/Scripts/Core/RoomSceneManager.cs", "ARCANE_PATCH_224_ROOM_CLEANUP");
            Check(root + "/Scripts/Core/GameWorld.cs", "ARCANE_PATCH_224_GAMEWORLD_CLEANUP");
            Check(root + "/Scripts/Presentation21/ProceduralAudioHaptics21.cs", "ARCANE_PATCH_224_LIFECYCLE_AUDIO_CLEAR");
            Check(root + "/Scripts/Presentation21/MorphologyPresentationDirector21.cs", "ARCANE_PATCH_224_DIRECTOR_CLEAR");
            Check(root + "/Scripts/Presentation21/MorphologyPresentationDirector21.cs", "ARCANE_PATCH_224_TARGET_MARK_CLEAR");
            Check(root + "/Scripts/Combat/ElementalReactionField.cs", "ARCANE_PATCH_224_FIELD_CLEAR");
            Check(root + "/Scripts/Combat/ReactionPropagation22.cs", "ARCANE_PATCH_224_LINEAGE_CLEAR");
            Check(root + "/Scripts/Presentation/PresentationResidues.cs", "ARCANE_PATCH_224_RESIDUE_CLEAR");
            Check(root + "/Scripts/Presentation/ReactionPresentation22.cs", "ARCANE_PATCH_224_COALESCER_CLEAR");
            Debug.Log("Patch 2.2.4 validation passed: centralized lifecycle cleanup, independent Spell Link depth, and edge-touch Rune connectivity are installed.");
        }

        [MenuItem("Arcane Engine/2.2.4/Clear Active Spell Effects")]
        public static void ClearActiveEffects()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Enter Play Mode before clearing active spell effects.");
                return;
            }
            SpellEffectLifecycle224.ClearAll(SpellEffectCleanupReason224.ManualValidation);
            Debug.Log("Patch 2.2.4 cleared active spell effects. Transient roots cleared: " + SpellEffectLifecycle224.LastClearedObjects);
        }

        private static void Check(string path, string marker)
        {
            if (!File.Exists(path) || !File.ReadAllText(path).Contains(marker))
                throw new System.InvalidOperationException("Patch 2.2.4 validation failed: " + marker + " missing from " + path);
        }
    }
}
#endif
