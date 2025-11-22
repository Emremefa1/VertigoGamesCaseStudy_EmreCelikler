using UnityEngine;
using WheelGame.Core;

namespace WheelGame.Core
{
    /// <summary>
    /// Manages temporary and banked rewards. Bomb resets temporary rewards.
    /// Does not handle UI directly; uses EventBus.
    /// </summary>
    public class RewardManager : MonoBehaviour
    {
        private int temporaryReward;
        public int TemporaryReward 
        { 
            get => temporaryReward;
            private set => temporaryReward = value;
        }

        private int bankedReward;
        public int BankedReward 
        { 
            get => bankedReward;
            private set => bankedReward = value;
        }

        [SerializeField] private SaveSystem saveSystem;

        public void Initialize()
        {
            if (saveSystem == null) saveSystem = new SaveSystem();
            BankedReward = saveSystem.LoadBank();
            TemporaryReward = 0;
            EventBus.OnRewardChanged?.Invoke();
        }

        public void AddReward(int amount, RewardType rewardType)
        {
            // Only Gold contributes to temporary rewards
            if (rewardType == RewardType.Gold)
            {
                TemporaryReward += amount;
                EventBus.OnRewardChanged?.Invoke();
            }
        }

        /// <summary>
        /// Converts gold value (e.g., from duplicate item) to banked rewards.
        /// </summary>
        public void ConvertItemToGold(int goldValue)
        {
            BankedReward += goldValue;
            saveSystem.SaveBank(BankedReward);
            EventBus.OnRewardChanged?.Invoke();
        }

        public void WalkAway()
        {
            BankedReward += TemporaryReward;
            TemporaryReward = 0;
            saveSystem.SaveBank(BankedReward);
            EventBus.OnWalkAway?.Invoke();
            EventBus.OnRewardChanged?.Invoke();
        }

        public void TriggerBomb()
        {
            TemporaryReward = 0;
            EventBus.OnBombTriggered?.Invoke();
            EventBus.OnRewardChanged?.Invoke();
        }

        /// <summary>
        /// Attempts to revive by deducting gold cost from banked rewards.
        /// Returns true if revive was successful, false if not enough gold.
        /// </summary>
        public bool TryRevive(int goldCost)
        {
            if (BankedReward >= goldCost)
            {
                BankedReward -= goldCost;
                saveSystem.SaveBank(BankedReward);
                EventBus.OnRewardChanged?.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets only temporary reward (used when reviving from bomb).
        /// Keeps banked reward and zone intact.
        /// </summary>
        public void ResetTemporaryOnly()
        {
            TemporaryReward = 0;
            EventBus.OnRewardChanged?.Invoke();
        }

        public void ResetAll()
        {
            TemporaryReward = 0;
            BankedReward = 0;
            saveSystem.SaveBank(BankedReward);
            EventBus.OnRewardChanged?.Invoke();
        }
    }
}
