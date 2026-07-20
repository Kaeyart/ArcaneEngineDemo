using UnityEngine;

namespace ArcaneEngine
{
    [CreateAssetMenu(menuName = "Arcane Engine/2.1/Shop Service", fileName = "ShopService")]
        public sealed class V21ShopServiceAsset : ScriptableObject
        {
            public string stableId;
            public string specialization;
            public string title;
            [TextArea] public string description;
            public string serviceContentId = "service:recovery";
            public RewardCategory category = RewardCategory.Healing;
            public int price = 24;
        }
}
