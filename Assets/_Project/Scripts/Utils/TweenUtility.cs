using DG.Tweening;
using UnityEngine;

namespace WheelGame.Utils
{
    public static class TweenUtility
    {
        public static Sequence DoPunchScale(Transform t, float duration = 0.35f)
        {
            var s = DOTween.Sequence();
            s.Append(t.DOScale(1.12f, duration * 0.5f).SetEase(Ease.OutBack));
            s.Append(t.DOScale(1f, duration * 0.5f).SetEase(Ease.InBack));
            return s;
        }
    }
}
