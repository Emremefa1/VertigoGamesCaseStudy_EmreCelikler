using UnityEngine;

namespace WheelGame.Data
{
    [CreateAssetMenu(menuName = "WheelGame/SliceDefinition")]
    public class SliceDefinitionSO : ScriptableObject
    {
        public WheelGame.Core.RewardType rewardType = WheelGame.Core.RewardType.Money;
        public int amount = 1;
        public Sprite icon;
        public int rarity = 0;
        
        [Header("Item Conversion (only used if rewardType = Item)")]
        [Tooltip("Gold value when this item is earned a second time")]
        public int itemGoldConversionValue = 100;
    }
}

