using UnityEngine;

namespace ArcaneEngine
{
    [CreateAssetMenu(menuName = "Arcane Engine/2.1/Affix", fileName = "Affix")]
        public sealed class V21AffixContentAsset : ScriptableObject
        {
            public string stableId;
            public string displayName;
            public string stat;
            public AffixKind kind;
            public string group;
            public string[] tags = new string[0];
            public EquipmentSlot[] slots = new EquipmentSlot[0];
            public float baseMinimum;
            public float baseMaximum;
            public bool percentage;
            public bool local;
            public int weight = 50;

            public V11AffixDefinition ToRuntime()
            {
                return new V11AffixDefinition { id = stableId, displayName = displayName, stat = stat, kind = kind, group = group,
                    tags = tags ?? new string[0], slots = slots ?? new EquipmentSlot[0], baseMinimum = baseMinimum,
                    baseMaximum = baseMaximum, percentage = percentage, local = local, weight = Mathf.Max(1, weight) };
            }
        }
}
