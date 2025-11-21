using UnityEngine;
using WheelGame.Core;

namespace WheelGame.Core
{
    /// <summary>
    /// Central orchestrator for game flow. Keeps minimal state and delegates responsibilities.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private ZoneController zoneController;
        [SerializeField] private RewardManager rewardManager;
        [SerializeField] private WheelController wheelController;
        [SerializeField] private UI.UIController uiController;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Basic wiring - UI subscribes via EventBus. Ensure services exist.
            if (zoneController == null) Debug.LogError("ZoneController not assigned");
            if (rewardManager == null) Debug.LogError("RewardManager not assigned");
            if (wheelController == null) Debug.LogError("WheelController not assigned");
            if (uiController == null) Debug.LogError("UIController not assigned");

            zoneController.Initialize();
            rewardManager.Initialize();
            wheelController.Initialize();
            uiController.Initialize();

            // set initial zone
            zoneController.SetZone(1);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
