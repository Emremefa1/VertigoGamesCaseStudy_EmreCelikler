using System;
namespace WheelGame.Core
{
    /// <summary>
    /// Simple static event bus for game-wide events.
    /// Use lightweight Action delegates for performance and testability.
    /// </summary>
    public static class EventBus
    {
        public static Action OnSpinStarted;
        public static Action<int> OnSpinRequested; // target slice index (optional debug)
        public static Action<int, SliceDefinition> OnSpinCompleted; // sliceIndex, sliceDef
        public static Action<int> OnZoneChanged; // zoneIndex
        public static Action OnBombTriggered;
        public static Action OnWalkAway;
        public static Action OnRewardChanged;
    }
}
