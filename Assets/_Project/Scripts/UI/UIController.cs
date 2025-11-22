using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WheelGame.Core;

namespace WheelGame.UI
{
    /// <summary>
    /// Wires UI elements and listens to EventBus events.
    /// This component intentionally finds references in OnValidate to avoid manual OnClick wiring.
    /// Integrates with RewardsDisplayPanel and slice animations.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Header("References (auto-assigned on validate)")]
        [SerializeField] private Button spinButton;
        [SerializeField] private Button walkButton;
        [SerializeField] private TMP_Text zoneText_value;
        [SerializeField] private TMP_Text bankedReward_value;
        [SerializeField] private RewardsDisplayPanel rewardsDisplayPanel;
        [SerializeField] private RewardPopupUI rewardPopup;

        [Header("Wheel Visuals")]
        [SerializeField] private Image wheelImage;
        [SerializeField] private Image indicatorImage;

        private GameManager gm;
        private RewardManager rewardManager;
        private ZoneController zoneController;
        private WheelController wheelController;

        private void OnValidate()
        {
            // try auto find by naming convention if fields are null
            if (spinButton == null) spinButton = transform.Find("Buttons/Button_Spin")?.GetComponent<Button>();
            if (walkButton == null) walkButton = transform.Find("Buttons/Button_WalkAway")?.GetComponent<Button>();
            if (zoneText_value == null) zoneText_value = transform.Find("Panel_Top/ZoneLabel_value")?.GetComponent<TMP_Text>();
            if (bankedReward_value == null) bankedReward_value = transform.Find("Panel_Top/BankedLabel_value")?.GetComponent<TMP_Text>();
            if (rewardsDisplayPanel == null) rewardsDisplayPanel = GetComponentInChildren<RewardsDisplayPanel>();
            if (rewardPopup == null) rewardPopup = GetComponentInChildren<RewardPopupUI>();
        }

        public void Initialize()
        {
            gm = GameManager.Instance;
            rewardManager = FindObjectOfType<RewardManager>();
            zoneController = FindObjectOfType<ZoneController>();
            wheelController = FindObjectOfType<WheelController>();

            // initialize rewards display panel if present
            if (rewardsDisplayPanel != null && rewardManager != null)
            {
                rewardsDisplayPanel.Initialize(rewardManager);
            }

            // wire buttons in code (no OnClick references in Inspector)
            if (spinButton != null)
            {
                spinButton.onClick.RemoveAllListeners();
                spinButton.onClick.AddListener(() =>
                {
                    if (wheelController != null && !wheelController.IsSpinning)
                    {
                        wheelController.StartSpin();
                    }
                });
            }
            if (walkButton != null)
            {
                walkButton.onClick.RemoveAllListeners();
                walkButton.onClick.AddListener(() =>
                {
                    // walk away only allowed when wheel not spinning and zone is safe or super
                    if (wheelController != null && !wheelController.IsSpinning)
                    {
                        if (zoneController.IsSafeZone || zoneController.IsSuperZone)
                        {
                            rewardManager.WalkAway();
                        }
                        else
                        {
                            Debug.Log("Walk away only allowed on safe/super zone.");
                        }
                    }
                });
            }

            // subscribe events
            EventBus.OnZoneChanged += OnZoneChanged;
            EventBus.OnRewardChanged += OnRewardChanged;
            EventBus.OnSpinStarted += OnSpinStarted;
            EventBus.OnSpinCompleted += OnSpinCompleted;
            EventBus.OnBombTriggered += OnBombTriggered;

            // initialize UI
            OnZoneChanged(zoneController.CurrentZone);
            OnRewardChanged();
        }

        private void OnDestroy()
        {
            EventBus.OnZoneChanged -= OnZoneChanged;
            EventBus.OnRewardChanged -= OnRewardChanged;
            EventBus.OnSpinStarted -= OnSpinStarted;
            EventBus.OnSpinCompleted -= OnSpinCompleted;
            EventBus.OnBombTriggered -= OnBombTriggered;
        }

        private void OnZoneChanged(int zone)
        {
            if (zoneText_value != null) zoneText_value.text = $"ZONE {zone}";

            // Update wheel and indicator visuals from preset
            if (zoneController != null && zoneController.CurrentPreset != null)
            {
                var preset = zoneController.CurrentPreset;
                if (wheelImage != null && preset.wheelImage != null)
                {
                    wheelImage.sprite = preset.wheelImage;
                }
                if (indicatorImage != null && preset.indicatorImage != null)
                {
                    indicatorImage.sprite = preset.indicatorImage;
                }
            }
        }

        private void OnRewardChanged()
        {
            if (rewardManager == null) rewardManager = FindObjectOfType<RewardManager>();
            if (bankedReward_value != null) bankedReward_value.text = rewardManager != null ? rewardManager.BankedReward.ToString() : "0";
        }

        private void OnSpinStarted()
        {
            // Disable buttons during spin
            if (spinButton != null) spinButton.interactable = false;
            if (walkButton != null) walkButton.interactable = false;
        }

        private void OnSpinCompleted(int sliceIndex, Core.SliceDefinition def)
        {
            // Show reward popup (popup handles bomb logic: revive or game over)
            if (rewardPopup != null)
            {
                rewardPopup.ShowReward(def);
            }

            // handle non-bomb rewards
            if (def.rewardType != Core.RewardType.Bomb)
            {
                rewardManager.AddReward(def.amount, def.rewardType);
                // advance zone after successful reward
                FindObjectOfType<ZoneController>().AdvanceZone();
            }
            // For bombs: RewardPopupUI handles revive/give up logic, so we don't do anything here

            // Re-enable buttons
            if (spinButton != null) spinButton.interactable = true;
            if (walkButton != null) walkButton.interactable = true;
        }

        private void OnBombTriggered()
        {
            Debug.Log("Bomb triggered: temporary reward lost.");
        }
    }
}
