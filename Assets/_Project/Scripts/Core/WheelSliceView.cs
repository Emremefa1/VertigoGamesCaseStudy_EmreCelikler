using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WheelGame.Data;

namespace WheelGame.Core
{
    /// <summary>
    /// Enhanced visual component for a single wheel slice in the 8-hole design.
    /// Displays reward icon and amount (if not an Item).
    /// Naming: ui_image_slice_icon, ui_text_slice_value_value, ui_image_slice_bg
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class WheelSliceView : MonoBehaviour
    {
        [Header("UI refs (assign in prefab)")]
        [SerializeField] private Image ui_image_slice_icon;
        [SerializeField] private TMP_Text ui_text_slice_value_value;
        [SerializeField] private Image ui_image_slice_bg;

        [Header("Visual Settings")]
        [SerializeField] private Color rewardColor = Color.white;
        [SerializeField] private Color bombColor = new Color(1f, 0.2f, 0.2f); // Red for bomb

        private SliceDefinitionSO currentData;
        private bool isBomb;

        public void SetData(SliceDefinitionSO so)
        {
            currentData = so;

            if (so == null)
            {
                if (ui_text_slice_value_value != null) ui_text_slice_value_value.text = "";
                if (ui_image_slice_icon != null) ui_image_slice_icon.enabled = false;
                return;
            }

            isBomb = so.rewardType == Core.RewardType.Bomb;

            // Set background color based on type
            if (ui_image_slice_bg != null)
            {
                ui_image_slice_bg.color = isBomb ? bombColor : rewardColor;
            }

            // Set icon
            if (ui_image_slice_icon != null)
            {
                ui_image_slice_icon.sprite = so.icon;
                ui_image_slice_icon.enabled = so.icon != null;
            }

            // Set amount value - only for stackable types (Money, Gold, Chests)
            if (ui_text_slice_value_value != null)
            {
                if (so.rewardType == RewardType.Item || so.rewardType == RewardType.Bomb)
                {
                    // Items and Bombs don't show amount
                    ui_text_slice_value_value.text = "";
                }
                else
                {
                    // Stackable types show amount with "x" prefix
                    ui_text_slice_value_value.text = so.amount > 0 ? $"x{so.amount}" : "";
                }
            }
        }

        public bool IsBomb() => isBomb;
        public int GetRewardAmount() => currentData != null ? currentData.amount : 0;
        public RewardType GetRewardType() => currentData != null ? currentData.rewardType : RewardType.Money;
    }
}
