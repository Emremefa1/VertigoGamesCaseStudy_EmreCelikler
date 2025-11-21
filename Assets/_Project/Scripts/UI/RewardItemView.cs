using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// Displays a single reward item in the rewards panel.
    /// Shows icon and quantity in a stacked format.
    /// Naming convention: ui_image_reward_icon, ui_text_reward_quantity_value
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class RewardItemView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image ui_image_reward_icon;
        [SerializeField] private TMP_Text ui_text_reward_quantity_value;
        [SerializeField] private Image ui_image_reward_background;

        private SliceDefinition rewardData;
        private int quantity;

        public void SetReward(SliceDefinition sliceDef, int qty)
        {
            rewardData = sliceDef;
            quantity = qty;

            if (ui_image_reward_icon != null && sliceDef != null)
            {
                ui_image_reward_icon.sprite = sliceDef.icon;
                ui_image_reward_icon.enabled = sliceDef.icon != null;
            }

            // Only show quantity for stackable types (Money, Gold, Chests), not for Items
            if (ui_text_reward_quantity_value != null)
            {
                if (sliceDef.rewardType == RewardType.Item)
                {
                    ui_text_reward_quantity_value.text = "";
                    ui_text_reward_quantity_value.enabled = false;
                }
                else
                {
                    ui_text_reward_quantity_value.text = qty > 1 ? qty.ToString() : "";
                    ui_text_reward_quantity_value.enabled = true;
                }
            }

            // Optional: color background by rarity
            if (ui_image_reward_background != null && sliceDef != null)
            {
                ui_image_reward_background.color = GetRarityColor(sliceDef.rarity);
            }
        }

        public void AddQuantity(int amount)
        {
            quantity += amount;
            if (ui_text_reward_quantity_value != null)
            {
                ui_text_reward_quantity_value.text = quantity > 1 ? quantity.ToString() : "";
            }
        }

        private Color GetRarityColor(int rarity)
        {
            return rarity switch
            {
                0 => new Color(0.7f, 0.7f, 0.7f),    // common: gray
                1 => new Color(0.2f, 0.8f, 0.2f),    // uncommon: green
                2 => new Color(0.2f, 0.6f, 1f),      // rare: blue
                3 => new Color(1f, 0.85f, 0f),       // legendary: gold
                _ => Color.white                       // default: white
            };
        }

        public int GetQuantity() => quantity;
        public SliceDefinition GetRewardData() => rewardData;
    }
}
