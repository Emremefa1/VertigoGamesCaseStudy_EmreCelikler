using UnityEngine;

namespace WheelGame.Core
{
    /// <summary>
    /// Simple pointer behaviour for optional pointer nudges when spin is about to end.
    /// </summary>
    public class WheelPointer : MonoBehaviour
    {
        [SerializeField] private RectTransform pointerRect;
        [SerializeField] private float nudgeAmount = 6f;
        [SerializeField] private float nudgeDuration = 0.15f;

        public void Nudge()
        {
            if (pointerRect == null) return;
            // simple local rotation nudge (non-DOTween fallback)
            pointerRect.localRotation = Quaternion.Euler(0, 0, nudgeAmount);
            // reset after short time
            Invoke(nameof(ResetNudge), nudgeDuration);
        }

        private void ResetNudge()
        {
            if (pointerRect == null) return;
            pointerRect.localRotation = Quaternion.identity;
        }
    }
}
