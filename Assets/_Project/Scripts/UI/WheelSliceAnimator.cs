using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WheelGame.Core;
using DG.Tweening;

namespace WheelGame.UI
{
    /// <summary>
    /// Manages visual feedback for wheel slices during and after spin.
    /// Animates slice highlights, shows landing feedback, updates displays.
    /// </summary>
    public class WheelSliceAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WheelSliceView sliceView;
        [SerializeField] private Image sliceBackground;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Animation Settings")]
        [SerializeField] private float highlightDuration = 0.2f;
        [SerializeField] private float highlightScale = 1.1f;
        [SerializeField] private Color highlightColor = Color.yellow;

        private Color originalColor;
        private Vector3 originalScale;
        private Sequence activeTween;

        private void Awake()
        {
            if (sliceBackground == null) sliceBackground = GetComponent<Image>();
            if (sliceView == null) sliceView = GetComponent<WheelSliceView>();
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

            if (sliceBackground != null)
                originalColor = sliceBackground.color;

            originalScale = transform.localScale;
        }

        /// <summary>
        /// Highlight this slice when wheel is about to land on it
        /// </summary>
        public void PlayHighlight()
        {
            KillActiveTween();

            var seq = DOTween.Sequence();
            seq.Append(transform.DOScale(originalScale * highlightScale, highlightDuration * 0.5f).SetEase(Ease.OutBack));
            seq.Join(sliceBackground.DOColor(highlightColor, highlightDuration * 0.5f));
            seq.Append(transform.DOScale(originalScale, highlightDuration * 0.5f).SetEase(Ease.InBack));
            seq.Join(sliceBackground.DOColor(originalColor, highlightDuration * 0.5f));

            activeTween = seq;
        }

        /// <summary>
        /// Pulse animation when this slice is the landing result
        /// </summary>
        public void PlayLandingFeedback()
        {
            KillActiveTween();

            var seq = DOTween.Sequence();

            // Large pulse
            seq.Append(transform.DOScale(originalScale * 1.3f, 0.2f).SetEase(Ease.OutBack));
            seq.Append(transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBounce));

            activeTween = seq;
        }

        /// <summary>
        /// Fade out effect (for bomb landing)
        /// </summary>
        public void PlayBombEffect()
        {
            KillActiveTween();

            if (sliceBackground != null)
            {
                var seq = DOTween.Sequence();
                seq.Append(sliceBackground.DOColor(Color.red, 0.3f).SetEase(Ease.InOutQuad));
                seq.Append(sliceBackground.DOColor(originalColor, 0.3f).SetEase(Ease.InOutQuad));
                seq.AppendInterval(0.2f);
                seq.Append(sliceBackground.DOColor(Color.red, 0.2f));
                seq.Append(sliceBackground.DOColor(originalColor, 0.2f));

                activeTween = seq;
            }
        }

        private void KillActiveTween()
        {
            if (activeTween != null && activeTween.IsPlaying())
            {
                activeTween.Kill();
            }
        }

        private void OnDestroy()
        {
            KillActiveTween();
        }
    }
}
