using UnityEngine;

namespace ArcaneEngine
{
    [CreateAssetMenu(menuName = "Arcane Engine/2.1/Enemy Content", fileName = "EnemyContent")]
        public sealed class V21EnemyContentAsset : ScriptableObject
        {
            public string stableId;
            public string displayName;
            public EnemyArchetype archetype;
            public string biome;
            public string mechanicalFamily;
            [TextArea] public string counterplay;
            public Color primary = Color.white;
            public Color telegraph = Color.red;
            public float anticipationSeconds = 0.7f;
            public float activeSeconds = 0.25f;
            public float recoverySeconds = 0.5f;
            public float damageRadius = 1f;
        }
}
