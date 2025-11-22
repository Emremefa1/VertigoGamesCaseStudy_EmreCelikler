using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using WheelGame.Data;

namespace WheelGame.Core
{
    /// <summary>
    /// Handles wheel spin animation and deterministic slice selection.
    /// Uses DOTween for smooth easing.
    /// </summary>
    [DisallowMultipleComponent]
    public class WheelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform wheelRoot; // rotates
        [SerializeField] private WheelBuilder builder;
        [SerializeField] private ZoneController zoneController;

        [Header("Spin tuning")]
        [SerializeField] private int minFullRotations = 3;
        [SerializeField] private float spinDuration = 3.0f;
        [SerializeField] private Ease spinEase = Ease.OutQuart;
        [SerializeField] private int sliceCount = 8;

        private bool isSpinning;
        private int lastTargetIndex = -1;

        public void Initialize()
        {
            if (wheelRoot == null) Debug.LogError("wheelRoot not assigned");
            if (builder == null) Debug.LogError("builder not assigned");
            if (zoneController == null) Debug.LogError("zoneController not assigned");
            ConfigureFromPreset(zoneController.CurrentPreset);
            
            // Subscribe to zone changes to update wheel preset
            EventBus.OnZoneChanged += OnZoneChanged;
        }

        private void OnDestroy()
        {
            EventBus.OnZoneChanged -= OnZoneChanged;
        }

        private void OnZoneChanged(int newZone)
        {
            // Update wheel slices when zone changes
            ConfigureFromPreset(zoneController.CurrentPreset);
        }

        public void ConfigureFromPreset(WheelPresetSO preset)
        {
            if (preset == null) return;
            // ensure slice count matches
            sliceCount = Mathf.Max(3, builder.SetupSlices(preset.slices));
        }

        public void StartSpin(int targetIndex = -1)
        {
            if (isSpinning) return;
            isSpinning = true;
            EventBus.OnSpinStarted?.Invoke();

            var preset = zoneController.CurrentPreset;
            if (preset == null) { Debug.LogError("Preset null"); return; }

            // Create deterministic random for target selection and offset
            var seed = DateTime.Now.Millisecond ^ zoneController.CurrentZone;
            var rand = new System.Random(seed);

            if (targetIndex < 0)
            {
                // choose a random index but avoid bias
                targetIndex = rand.Next(0, sliceCount);
            }
            lastTargetIndex = targetIndex;

            // calculate rotation degrees to land on target
            float anglePerSlice = 360f / sliceCount;
            float randomOffset = rand.Next(0, 45); 
            float endRotation = -(minFullRotations * 360f + targetIndex * anglePerSlice + randomOffset);

            // Ensure final rotation aligns to nearest multiple of 45 degrees
            endRotation = SnapToNearestQuarterRotation(endRotation);

            wheelRoot
                .DORotate(new Vector3(0, 0, endRotation), spinDuration, RotateMode.FastBeyond360)
                .SetEase(spinEase)
                .OnComplete(() =>
                {
                    OnSpinComplete(targetIndex);
                });
        }

        /// <summary>
        /// Snaps the rotation to the nearest multiple of 45 degrees (0, 45, 90, 135, 180, 225, 270, 315, etc.)
        /// </summary>
        private float SnapToNearestQuarterRotation(float rotation)
        {
            // Divide by 45 to get 45-degree increments, round to nearest integer, multiply back
            float eighthTurns = Mathf.Round(rotation / 45f);
            return eighthTurns * 45f;
        }

        private void OnSpinComplete(int targetIndex)
        {
            isSpinning = false;
            
            // Determine which slice is actually at the pointer based on wheel rotation
            // Account for the snapped rotation to find the actual index at top
            float currentRotation = wheelRoot.localEulerAngles.z;
            float normalizedRotation = NormalizeRotation(currentRotation);
            int actualIndex = GetSliceIndexAtPointer(normalizedRotation);
            
            // Get slice definition from actual index
            var sliceDefSO = builder.GetSliceSO(actualIndex);
            // convert to runtime model
            var runtime = new SliceDefinition
            {
                rewardType = sliceDefSO != null ? sliceDefSO.rewardType : Core.RewardType.Money,
                amount = sliceDefSO != null ? sliceDefSO.amount : 0,
                icon = sliceDefSO != null ? sliceDefSO.icon : null,
                rarity = sliceDefSO != null ? sliceDefSO.rarity : 0,
                itemGoldConversionValue = sliceDefSO != null ? sliceDefSO.itemGoldConversionValue : 100
            };

            EventBus.OnSpinCompleted?.Invoke(actualIndex, runtime);
        }

        /// <summary>
        /// Normalizes rotation to 0-360 range
        /// </summary>
        private float NormalizeRotation(float rotation)
        {
            rotation %= 360f;
            if (rotation < 0f) rotation += 360f;
            return rotation;
        }

        /// <summary>
        /// Determines which slice index is at the pointer (top) based on wheel rotation.
        /// Clockwise layout: index 0 at 0°, index 1 at 45° (or -315°), index 2 at 90° (or -270°), etc.
        /// </summary>
        private int GetSliceIndexAtPointer(float normalizedRotation)
        {
            float anglePerSlice = 360f / sliceCount;
            // For clockwise layout with positive angles: index = angle / anglePerSlice
            int sliceIndex = Mathf.RoundToInt(normalizedRotation / anglePerSlice) % sliceCount;
            
            return sliceIndex;
        }

        public bool IsSpinning => isSpinning;
    }
}
