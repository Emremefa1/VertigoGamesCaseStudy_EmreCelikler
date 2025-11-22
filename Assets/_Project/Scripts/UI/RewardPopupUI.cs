using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using WheelGame.Core;
using WheelGame.Data;

namespace WheelGame.UI
{
    /// <summary>
    /// Displays a popup showing the reward just earned from spinning the wheel.
    /// Animates in with scale+fade, displays briefly, then animates out.
    /// Scalable: easy to customize timing, styling, and animation behavior.
    /// </summary>
    public class RewardPopupUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform popupRoot;
        [SerializeField] private Image rewardIcon;
        [SerializeField] private TMP_Text rewardAmount;
        [SerializeField] private TMP_Text rewardLabel;

        [Header("Bomb Action Buttons")]
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button giveUpButton;
        [SerializeField] private TMP_Text reviveCostText;

        [Header("Animation Settings")]
        [SerializeField] private float animInDuration = 0.3f;
        [SerializeField] private float displayDuration = 2.0f;
        [SerializeField] private float animOutDuration = 0.3f;
        [SerializeField] private Ease easeIn = Ease.OutBack;
        [SerializeField] private Ease easeOut = Ease.InQuad;

        [Header("Bomb Settings")]
        [SerializeField] private int reviveCost = 25; // gold cost to revive

        private Sequence currentSequence;
        private RewardManager rewardManager;
        private bool waitingForBombAction = false;

        private void OnValidate()
        {
            // Auto-find components if not assigned
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (popupRoot == null) popupRoot = GetComponent<RectTransform>();
        }

        private void Start()
        {
            // Ensure popup starts invisible and not blocking input
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;

            // Setup bomb buttons if they exist
            if (reviveButton != null)
            {
                reviveButton.onClick.RemoveAllListeners();
                reviveButton.onClick.AddListener(OnReviveClicked);
            }
            if (giveUpButton != null)
            {
                giveUpButton.onClick.RemoveAllListeners();
                giveUpButton.onClick.AddListener(OnGiveUpClicked);
            }

            // Find RewardManager if not set
            if (rewardManager == null) rewardManager = FindObjectOfType<RewardManager>();
        }

        private void OnDestroy()
        {
            // Kill any active animation on destroy
            currentSequence?.Kill();
        }

        /// <summary>
        /// Shows the reward popup with animation.
        /// For bombs, displays revive/give up buttons and waits for player input.
        /// </summary>
        public void ShowReward(SliceDefinition rewardDef)
        {
            if (rewardDef == null)
            {
                Debug.LogWarning("RewardPopupUI.ShowReward: rewardDef is null");
                return;
            }

            // Kill existing sequence if any
            currentSequence?.Kill();

            // Setup UI with reward data
            SetupRewardDisplay(rewardDef);

            // Check if this is a bomb
            bool isBomb = rewardDef.rewardType == RewardType.Bomb;

            // Start animation sequence
            if (isBomb)
            {
                waitingForBombAction = true;
                ShowBombPopup();
            }
            else
            {
                waitingForBombAction = false;
                currentSequence = CreateAnimationSequence();
                currentSequence.Play();
            }
        }

        private void ShowBombPopup()
        {
            var sequence = DOTween.Sequence();

            // Initial state: invisible, small scale
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            popupRoot.localScale = Vector3.one * 0.8f;

            // Animate in and enable buttons
            sequence.AppendCallback(() => canvasGroup.blocksRaycasts = true);
            sequence.Append(canvasGroup.DOFade(1f, animInDuration).SetEase(easeIn));
            sequence.Join(popupRoot.DOScale(Vector3.one, animInDuration).SetEase(easeIn));
            sequence.AppendCallback(() => SetBombButtonsActive(true));

            currentSequence = sequence;
            currentSequence.Play();
        }

        private void SetBombButtonsActive(bool active)
        {
            if (reviveButton != null) reviveButton.interactable = active;
            if (giveUpButton != null) giveUpButton.interactable = active;
        }

        private void OnReviveClicked()
        {
            if (!waitingForBombAction) return;
            waitingForBombAction = false;

            // Check if player has enough gold
            if (rewardManager != null && rewardManager.TryRevive(reviveCost))
            {
                // Successfully revived - keep temporary rewards, dismiss popup
                // Game continues with collected rewards intact
                Debug.Log($"Revived! Gold cost: {reviveCost}. Rewards kept!");
                DismissPopup();
            }
            else
            {
                Debug.Log($"Not enough gold to revive. Cost: {reviveCost}, Have: {rewardManager?.BankedReward ?? 0}");
                waitingForBombAction = true; // Re-enable waiting since revive failed
            }
        }

        private void OnGiveUpClicked()
        {
            if (!waitingForBombAction) return;
            waitingForBombAction = false;

            // Clear temporary rewards but keep banked value when giving up on bomb
            if (rewardManager != null)
            {
                rewardManager.ResetTemporaryOnly();
            }
            
            // Clear rewards display panel
            var rewardsPanel = FindObjectOfType<RewardsDisplayPanel>();
            if (rewardsPanel != null)
            {
                rewardsPanel.ClearDisplay();
            }

            // Reset zone to 1
            var zoneController = FindObjectOfType<ZoneController>();
            if (zoneController != null)
            {
                zoneController.SetZone(1);
            }

            DismissPopup();
        }

        private void DismissPopup()
        {
            SetBombButtonsActive(false);

            // Animate out
            var outSequence = DOTween.Sequence();
            outSequence.Append(canvasGroup.DOFade(0f, animOutDuration).SetEase(easeOut));
            outSequence.Join(popupRoot.DOScale(Vector3.one * 0.8f, animOutDuration).SetEase(easeOut));
            outSequence.AppendCallback(() => canvasGroup.blocksRaycasts = false);

            currentSequence = outSequence;
            currentSequence.Play();
        }

        private void SetupRewardDisplay(SliceDefinition rewardDef)
        {
            // Set icon
            if (rewardIcon != null)
            {
                rewardIcon.sprite = rewardDef.icon;
                rewardIcon.enabled = rewardDef.icon != null;
            }

            // Set amount text (only for stackables)
            if (rewardAmount != null)
            {
                bool showAmount = rewardDef.rewardType != RewardType.Item && rewardDef.rewardType != RewardType.Bomb;
                if (showAmount)
                {
                    rewardAmount.text = $"x{rewardDef.amount}";
                    rewardAmount.enabled = true;
                }
                else
                {
                    rewardAmount.enabled = false;
                }
            }

            // Set type label
            if (rewardLabel != null)
            {
                rewardLabel.text = GetRewardTypeLabel(rewardDef.rewardType);
            }

            // Show bomb UI only for bombs
            bool isBomb = rewardDef.rewardType == RewardType.Bomb;
            
            if (reviveButton != null) reviveButton.gameObject.SetActive(isBomb);
            if (giveUpButton != null) giveUpButton.gameObject.SetActive(isBomb);
            if (reviveCostText != null)
            {
                if (isBomb)
                {
                    reviveCostText.text = reviveCost.ToString();
                    reviveCostText.gameObject.SetActive(true);
                }
                else
                {
                    reviveCostText.gameObject.SetActive(false);
                }
            }
        }

        private string GetRewardTypeLabel(RewardType type)
        {
            return type switch
            {
                RewardType.Money => "MONEY",
                RewardType.Gold => "GOLD",
                RewardType.Chests => "CHESTS",
                RewardType.Item => "ITEM",
                RewardType.Bomb => "BOMB",
                _ => "REWARD"
            };
        }

        private Sequence CreateAnimationSequence()
        {
            var sequence = DOTween.Sequence();

            // Initial state: invisible, small scale
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            popupRoot.localScale = Vector3.one * 0.8f;

            // Animate in: fade + scale (also enable blocking during display)
            sequence.AppendCallback(() => canvasGroup.blocksRaycasts = true);
            sequence.Append(canvasGroup.DOFade(1f, animInDuration).SetEase(easeIn));
            sequence.Join(popupRoot.DOScale(Vector3.one, animInDuration).SetEase(easeIn));

            // Hold for display duration
            sequence.AppendInterval(displayDuration);

            // Animate out: fade + scale down (disable blocking after)
            sequence.Append(canvasGroup.DOFade(0f, animOutDuration).SetEase(easeOut));
            sequence.Join(popupRoot.DOScale(Vector3.one * 0.8f, animOutDuration).SetEase(easeOut));
            sequence.AppendCallback(() => canvasGroup.blocksRaycasts = false);

            return sequence;
        }

        public bool IsAnimating => currentSequence != null && currentSequence.IsActive();
    }
}
