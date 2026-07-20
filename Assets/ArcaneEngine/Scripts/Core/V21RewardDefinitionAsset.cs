using UnityEngine;

namespace ArcaneEngine
{
    [CreateAssetMenu(menuName = "Arcane Engine/2.1/Reward", fileName = "Reward")]
        public sealed class V21RewardDefinitionAsset : ScriptableObject
        {
            public string stableId;
            public RewardCategory category = RewardCategory.Essence;
            public string title;
            [TextArea] public string description;
            public string contentId;
            public int amount = 1;
            public Color color = Color.white;
        }
}
