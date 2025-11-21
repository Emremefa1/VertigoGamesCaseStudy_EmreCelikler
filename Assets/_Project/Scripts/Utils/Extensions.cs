using System.Collections.Generic;
using UnityEngine;

namespace WheelGame.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Shuffle list in place (Fisher-Yates)
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, System.Random rng = null)
        {
            if (rng == null) rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static float AngleForIndex(int index, int sliceCount)
        {
            if (sliceCount <= 0) return 0f;
            return index * (360f / sliceCount);
        }
    }
}
