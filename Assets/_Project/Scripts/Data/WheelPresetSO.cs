using System.Collections.Generic;
using UnityEngine;

namespace WheelGame.Data
{
    public enum WheelType { Normal, Safe, Super }

    [CreateAssetMenu(menuName = "WheelGame/WheelPreset")]
    public class WheelPresetSO : ScriptableObject
    {
        public WheelType wheelType = WheelType.Normal;

        [Tooltip("Slices count must equal the wheel's slice count in scene.")]
        public List<SliceDefinitionSO> slices = new List<SliceDefinitionSO>();

        [Tooltip("Optional: theme color for UI (gold/silver/normal)")]
        public Color themeColor = Color.white;

        [Header("Visual Assets")]
        public Sprite wheelImage;
        public Sprite indicatorImage;
    }
}

