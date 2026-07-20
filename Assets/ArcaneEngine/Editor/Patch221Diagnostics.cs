using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.Editor
{
    public static class Patch221Diagnostics
    {
        private const string CombatRoot = "Assets/ArcaneEngine/Scripts/Combat/";

        [MenuItem("Arcane Engine/2.2.1/Validate EntityId Hotfix")]
        public static void Validate()
        {
            string propagation = Read("ReactionPropagation22.cs");
            string field = Read("ElementalReactionField.cs");
            string runtime = Read("ElementalReactionRuntime.cs");
            string executor = Read("ElementalReactionMechanicExecutor.cs");

            Require(!propagation.Contains("GetInstanceID"),
                "ReactionPropagation22.cs still uses GetInstanceID().");
            Require(!runtime.Contains("GetInstanceID"),
                "ElementalReactionRuntime.cs still uses GetInstanceID().");
            Require(propagation.Contains("Dictionary<EntityId, float>"),
                "Lineage target history is not keyed by EntityId.");
            Require(propagation.Contains("target.GetEntityId()"),
                "Lineage targets do not use GetEntityId().");
            Require(field.Contains("private EntityId _ownerId;"),
                "Elemental fields do not store an EntityId owner.");
            Require(field.Contains("CountOwned(EntityId ownerId"),
                "Elemental field ownership API is not EntityId-based.");
            Require(runtime.Contains("_owner.GetEntityId()"),
                "Major-state field ownership does not use GetEntityId().");
            Require(runtime.Contains("EntityId.None"),
                "Ownerless runtime fields do not use EntityId.None.");
            Require(executor.Contains("EntityId.None"),
                "Mechanic-created ownerless fields do not use EntityId.None.");

            Debug.Log(
                "Patch 2.2.1 validation passed: reaction target history and " +
                "field ownership use Unity 6.5 EntityId values.");
        }

        private static string Read(string fileName)
        {
            string path = CombatRoot + fileName;
            if (!File.Exists(path))
                throw new FileNotFoundException("Required Patch 2.2 file is missing.", path);
            return File.ReadAllText(path);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
                throw new System.InvalidOperationException(message);
        }
    }
}
