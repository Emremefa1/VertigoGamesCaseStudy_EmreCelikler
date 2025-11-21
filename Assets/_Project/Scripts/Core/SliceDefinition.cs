using UnityEngine;

namespace WheelGame.Core
{
    public enum RewardType
    {
        Money,
        Gold,
        Chests,
        Item,
        Bomb
    }

    /// <summary>
    /// Runtime representation of a reward slice. This mirrors the ScriptableObject data.
    /// </summary>
    [System.Serializable]
    public class SliceDefinition
    {
        public RewardType rewardType;
        public int amount;
        public Sprite icon;
        public int rarity; // optional
        public int itemGoldConversionValue = 100; // gold value if item earned twice
    }
}
