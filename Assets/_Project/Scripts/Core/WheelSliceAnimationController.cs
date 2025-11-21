using System.Collections.Generic;
using UnityEngine;
using WheelGame.Core;

namespace WheelGame.Core
{
    /// <summary>
    /// Manages all visual slice animators on the wheel.
    /// Handles highlight feedback, landing animations, and bomb effects.
    /// </summary>
    public class WheelSliceAnimationController : MonoBehaviour
    {
        [SerializeField] private Transform slicesParent;
        
        private List<WheelGame.UI.WheelSliceAnimator> sliceAnimators = new List<WheelGame.UI.WheelSliceAnimator>();
        private WheelGame.UI.WheelSliceAnimator currentLandingSlice;

        public void Initialize()
        {
            if (slicesParent == null)
            {
                Debug.LogError("slicesParent not assigned");
                return;
            }

            // Gather all slice animators
            sliceAnimators.Clear();
            foreach (Transform child in slicesParent)
            {
                var animator = child.GetComponent<WheelGame.UI.WheelSliceAnimator>();
                if (animator != null)
                {
                    sliceAnimators.Add(animator);
                }
            }

            Debug.Log($"Initialized {sliceAnimators.Count} slice animators");
        }

        /// <summary>
        /// Highlight the slice that will be landed on
        /// </summary>
        public void HighlightSlice(int sliceIndex)
        {
            if (sliceIndex < 0 || sliceIndex >= sliceAnimators.Count)
                return;

            // Clear previous highlight
            foreach (var animator in sliceAnimators)
            {
                if (animator != null) animator.PlayHighlight();
            }

            currentLandingSlice = sliceAnimators[sliceIndex];
        }

        /// <summary>
        /// Play landing feedback on the slice
        /// </summary>
        public void PlayLandingFeedback(int sliceIndex, bool isBomb)
        {
            if (sliceIndex < 0 || sliceIndex >= sliceAnimators.Count)
                return;

            var animator = sliceAnimators[sliceIndex];
            if (animator == null) return;

            if (isBomb)
            {
                animator.PlayBombEffect();
            }
            else
            {
                animator.PlayLandingFeedback();
            }
        }

        public int GetSliceCount() => sliceAnimators.Count;
    }
}
