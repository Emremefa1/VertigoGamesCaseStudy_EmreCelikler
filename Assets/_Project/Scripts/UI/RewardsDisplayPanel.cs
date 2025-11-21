using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// Displays collected rewards in a grid/stack format.
    /// Shows temporary rewards collected during current spin session.
    /// EXIT button allows player to walk away and bank rewards.
    /// </summary>
    public class RewardsDisplayPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button ui_button_exit;
        [SerializeField] private GridLayoutGroup ui_grid_rewards_container;
        [SerializeField] private TMP_Text ui_text_reward_total_value;
        [SerializeField] private GameObject rewardItemPrefab;
        [SerializeField] private RectTransform rewardsContainer;

        [Header("Settings")]
        [SerializeField] private int maxDisplayItems = 12;
        

        private Dictionary<RewardType, RewardItemView> displayedRewards = new Dictionary<RewardType, RewardItemView>();
        private Dictionary<Sprite, RewardItemView> displayedItems = new Dictionary<Sprite, RewardItemView>(); // key is item icon
        private RewardManager rewardManager;
        private int totalMoneyValue;

        private void OnValidate()
        {
            // Auto-find references by naming convention
            if (ui_button_exit == null) ui_button_exit = transform.Find("Button_Exit")?.GetComponent<Button>();
            if (rewardsContainer == null) rewardsContainer = transform.Find("Container_Rewards")?.GetComponent<RectTransform>();
            if (ui_text_reward_total_value == null) ui_text_reward_total_value = transform.Find("Text_TotalValue")?.GetComponent<TMP_Text>();
        }

        public void Initialize(RewardManager rm)
        {
            rewardManager = rm;

            if (ui_button_exit != null)
            {
                ui_button_exit.onClick.RemoveAllListeners();
                ui_button_exit.onClick.AddListener(() =>
                {
                    if (rewardManager != null)
                    {
                        rewardManager.WalkAway();
                        ClearDisplay();
                    }
                });
            }

            EventBus.OnSpinCompleted += OnRewardEarned;
            EventBus.OnRewardChanged += UpdateTotalDisplay;
            EventBus.OnBombTriggered += OnBombTriggered;

            UpdateTotalDisplay();
        }

        private void OnDestroy()
        {
            EventBus.OnSpinCompleted -= OnRewardEarned;
            EventBus.OnRewardChanged -= UpdateTotalDisplay;
            EventBus.OnBombTriggered -= OnBombTriggered;
        }

        private void OnRewardEarned(int sliceIndex, SliceDefinition sliceDef)
        {
            if (sliceDef.rewardType == RewardType.Bomb)
            {
                OnBombTriggered();
                return;
            }

            // Add to display
            AddRewardToDisplay(sliceDef);
            UpdateTotalDisplay();
        }

        private void AddRewardToDisplay(SliceDefinition sliceDef)
        {
            if (sliceDef.rewardType == RewardType.Bomb)
            {
                return; // Bombs are handled separately
            }

            // Handle stackable rewards (Money, Gold, Chests)
            if (sliceDef.rewardType == RewardType.Money || 
                sliceDef.rewardType == RewardType.Gold || 
                sliceDef.rewardType == RewardType.Chests)
            {
                if (displayedRewards.TryGetValue(sliceDef.rewardType, out var existingView))
                {
                    // Stack quantity
                    existingView.AddQuantity(sliceDef.amount);
                }
                else if (displayedRewards.Count + displayedItems.Count < maxDisplayItems)
                {
                    DisplayStackableReward(sliceDef);
                }
            }
            // Handle items
            else if (sliceDef.rewardType == RewardType.Item)
            {
                // Items need itemDefinition reference from SliceDefinitionSO
                // We'll need to pass the full SliceDefinitionSO or get it from builder
                HandleItemReward(sliceDef);
            }
        }

        private void DisplayStackableReward(SliceDefinition sliceDef)
        {
            if (rewardItemPrefab == null)
            {
                Debug.LogWarning("rewardItemPrefab not assigned");
                return;
            }

            var go = Instantiate(rewardItemPrefab, rewardsContainer);
            var view = go.GetComponent<RewardItemView>();
            
            if (view != null)
            {
                view.SetReward(sliceDef, sliceDef.amount);
                displayedRewards[sliceDef.rewardType] = view;
                go.name = $"RewardItem_{sliceDef.rewardType}";
            }
        }

        private void HandleItemReward(SliceDefinition sliceDef)
        {
            if (rewardItemPrefab == null || sliceDef.icon == null)
            {
                Debug.LogWarning("rewardItemPrefab not assigned or item icon is null");
                return;
            }

            Sprite itemKey = sliceDef.icon;

            // Check if we already have this item displayed
            if (displayedItems.TryGetValue(itemKey, out var existingView))
            {
                // Item already earned once, convert to gold but keep item displayed
                int goldValue = sliceDef.itemGoldConversionValue;
                
                // Add gold reward
                if (displayedRewards.TryGetValue(RewardType.Gold, out var goldView))
                {
                    goldView.AddQuantity(goldValue);
                }
                else if (displayedRewards.Count + displayedItems.Count < maxDisplayItems)
                {
                    // Create new gold display
                    var goldSlice = new SliceDefinition
                    {
                        rewardType = RewardType.Gold,
                        amount = goldValue,
                        icon = null,
                        rarity = 0
                    };
                    DisplayStackableReward(goldSlice);
                }
                
                // Don't remove or destroy the item - keep it displayed
                Debug.Log($"Item with icon '{itemKey.name}' earned twice - converted {goldValue} gold but item remains displayed");
            }
            else if (displayedRewards.Count + displayedItems.Count < maxDisplayItems)
            {
                // First time earning this item - display it
                var go = Instantiate(rewardItemPrefab, rewardsContainer);
                var view = go.GetComponent<RewardItemView>();
                
                if (view != null)
                {
                    view.SetReward(sliceDef, sliceDef.amount);
                    displayedItems[itemKey] = view;
                    go.name = $"RewardItem_{itemKey.name}";
                }
            }
        }

        private void UpdateTotalDisplay()
        {
            if (rewardManager == null) return;

            totalMoneyValue = rewardManager.TemporaryReward;

            if (ui_text_reward_total_value != null)
            {
                ui_text_reward_total_value.text = $"${totalMoneyValue}";
            }
        }

        private void OnBombTriggered()
        {
            // Visual feedback - show bomb message but DON'T clear yet
            // The RewardPopupUI handles whether to keep or clear based on revive/give up
            if (ui_text_reward_total_value != null)
            {
                ui_text_reward_total_value.text = "BOMB! Revive or Give Up?";
                ui_text_reward_total_value.color = Color.red;
            }
        }

        public void ClearDisplay()
        {
            // Destroy all reward items
            foreach (var view in displayedRewards.Values)
            {
                if (view != null) Destroy(view.gameObject);
            }
            displayedRewards.Clear();
            
            // Destroy all items
            foreach (var view in displayedItems.Values)
            {
                if (view != null) Destroy(view.gameObject);
            }
            displayedItems.Clear();

            totalMoneyValue = 0;
            if (ui_text_reward_total_value != null)
            {
                ui_text_reward_total_value.text = "$0";
                ui_text_reward_total_value.color = Color.white;
            }
        }

        public int GetDisplayedRewardCount() => displayedRewards.Count;
        public int GetTotalDisplayedValue() => totalMoneyValue;
    }
}
