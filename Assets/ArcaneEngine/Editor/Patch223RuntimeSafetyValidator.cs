#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine
{
    public static class Patch223RuntimeSafetyValidator
    {
        [MenuItem("Arcane Engine/2.2.3/Validate Hotfix")]
        public static void ValidateHotfix()
        {
            bool audioReady = ProceduralSpellAudio21.Hotfix223Ready;
            bool nullSafe = DemoCatalog.GetCore(null) == null &&
                            DemoCatalog.GetModifier(null) == null &&
                            DemoCatalog.GetItem(null) == null;

            if (audioReady && nullSafe)
            {
                Debug.Log(
                    "Patch 2.2.3 valid: procedural audio root is active, pooled voices " +
                    "use detached follow ownership, and catalog lookups are null-safe.");
            }
            else
            {
                Debug.LogError(
                    "Patch 2.2.3 validation failed. AudioReady=" + audioReady +
                    ", NullSafeCatalog=" + nullSafe + ".");
            }
        }
    }
}
#endif
