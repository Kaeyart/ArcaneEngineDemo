using System;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ItemDefinition : ScriptableObject
        {
            public string id;
            public string displayName;
            [TextArea] public string description;
            public EquipmentSlot slot;
            public ItemRarity rarity;
            public bool twoHanded;
            public float health;
            public float mana;
            public float movementSpeed;
            public float armor;
            public float resistance;
            public float critChance;
            public float critDamage;
            public float spellPower;
            public float cooldownReduction;
            public float triggerEnergy;
            public UniqueMutation mutation;
            public string setId;
            public string grantedCoreId;
            public string baseFamily;
            public string implicitText;
            public string[] itemTags = new string[0];
            public Color color = Color.white;
        }
}
