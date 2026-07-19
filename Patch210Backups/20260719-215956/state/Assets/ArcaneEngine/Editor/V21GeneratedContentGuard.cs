#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.Editor
{
    public static class V21GeneratedContentGuard
    {
        private const string Root = "Assets/ArcaneEngine/Resources/V21Content";

        [MenuItem("Arcane Engine/2.1/Repair Generated Content")]
        public static void Repair()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Repair Arcane Engine Content",
                "This deletes and regenerates Assets/ArcaneEngine/Resources/V21Content. " +
                "Use this when generated assets contain missing-script references.",
                "Repair",
                "Cancel");

            if (!confirmed)
                return;

            if (AssetDatabase.IsValidFolder(Root))
                AssetDatabase.DeleteAsset(Root);
            else if (Directory.Exists(Root))
                Directory.Delete(Root, true);

            AssetDatabase.Refresh();
            V21ContentBuilder.RebuildAll();
            Debug.Log("[Arcane Content] Generated content repaired.");
        }
    }
}
#endif
