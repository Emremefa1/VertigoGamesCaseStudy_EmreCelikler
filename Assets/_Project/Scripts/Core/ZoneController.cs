using UnityEngine;
using WheelGame.Data;

namespace WheelGame.Core
{
    /// <summary>
    /// Manages zone progression and assigns custom WheelPresets for each zone.
    /// Validates that only SafePreset is used for zones 5, 10, 15, ... (multiples of 5)
    /// and only SuperPreset is used for zones 30, 60, 90, ... (multiples of 30).
    /// Flexible: supports any number of zones (5, 25, 27, 60, etc.)
    /// </summary>
    public class ZoneController : MonoBehaviour
    {
        [Header("Zone Wheel Assignments")]
        [SerializeField] private WheelPresetSO[] zoneWheels = new WheelPresetSO[0];

        public int CurrentZone { get; private set; } = 1;
        public WheelPresetSO CurrentPreset { get; private set; }

        public void Initialize()
        {
            if (zoneWheels.Length == 0)
            {
                Debug.LogError("ZoneController: No wheels assigned!");
                return;
            }

            // Validate all assignments
            for (int zone = 1; zone <= zoneWheels.Length; zone++)
            {
                WheelPresetSO wheel = zoneWheels[zone - 1];
                if (wheel == null)
                {
                    Debug.LogError($"Zone {zone}: Wheel not assigned");
                    continue;
                }

                // Validate zone type requirements
                bool isMultipleOf30 = zone % 30 == 0;
                bool isMultipleOf5 = zone % 5 == 0;

                if (isMultipleOf30 && wheel.wheelType != WheelType.Super)
                {
                    Debug.LogError($"Zone {zone} (multiple of 30) must use SuperPreset, but got {wheel.wheelType}");
                }
                else if (isMultipleOf5 && !isMultipleOf30 && wheel.wheelType != WheelType.Safe)
                {
                    Debug.LogError($"Zone {zone} (multiple of 5) must use SafePreset, but got {wheel.wheelType}");
                }
            }

            ApplyPresetForZone(CurrentZone);
        }

        public void SetZone(int zone)
        {
            CurrentZone = Mathf.Clamp(zone, 1, zoneWheels.Length);
            ApplyPresetForZone(CurrentZone);
            EventBus.OnZoneChanged?.Invoke(CurrentZone);
        }

        private void ApplyPresetForZone(int zone)
        {
            if (zoneWheels.Length == 0)
            {
                Debug.LogError("ZoneController: No wheels assigned");
                return;
            }

            if (zone < 1 || zone > zoneWheels.Length)
            {
                Debug.LogError($"Zone {zone} out of range [1-{zoneWheels.Length}]");
                return;
            }

            CurrentPreset = zoneWheels[zone - 1];
            if (CurrentPreset == null)
            {
                Debug.LogError($"Zone {zone} has no wheel assigned");
            }
        }

        public void AdvanceZone()
        {
            SetZone(CurrentZone + 1);
        }

        public bool IsSafeZone => CurrentZone % 5 == 0 && CurrentZone % 30 != 0;
        public bool IsSuperZone => CurrentZone % 30 == 0;
        public int MaxZone => zoneWheels.Length;
    }
}
