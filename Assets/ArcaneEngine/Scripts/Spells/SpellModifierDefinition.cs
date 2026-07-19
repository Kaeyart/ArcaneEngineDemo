using System;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class SpellModifierDefinition : ScriptableObject
        {
            public string id;
            public string displayName;
            public string flavorName;
            [TextArea] public string shortDescription;
            [TextArea] public string advancedDescription;
            public ModifierEffect effect;
            public ModifierCategory category;
            public ConnectorFamily connectorFamily = ConnectorFamily.Flow;
            public float magnitude = 1f;
            public int inputSide = 3;
            public int[] outputSides = { 0 };
            public HexCoord[] shape = { new HexCoord(0, 0) };
            public Color uiColor = Color.white;
            public int instability;
            public int preparationCost = 1;
            public int capacityCost = 1;
            public bool stackable = true;
            public bool availableAsSupport = true;
            public SpellDelivery[] compatibleDeliveries = new SpellDelivery[0];
    
            public string FullDisplayName
            {
                get { return string.IsNullOrEmpty(flavorName) || flavorName == displayName ? displayName : displayName + "\n" + flavorName; }
            }
    
            public bool IsCompatible(SpellCoreDefinition core)
            {
                return core != null && (compatibleDeliveries == null || compatibleDeliveries.Length == 0 ||
                    System.Array.IndexOf(compatibleDeliveries, core.delivery) >= 0);
            }
        }
}
