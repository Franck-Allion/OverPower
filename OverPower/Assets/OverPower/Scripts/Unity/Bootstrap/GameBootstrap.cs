using UnityEngine;
using System.Threading;
using OverPower.Application;
using OverPower.Unity.SceneFlow;

namespace OverPower.Unity.Bootstrap
{
    [DefaultExecutionOrder(-100)]
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private bool runOnAwake = true;

        private static GameBootstrap _instance;
        private UnitySceneNavigator _sceneNavigator;
        private GameFlowController _gameFlowController;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("[GameBootstrap] Duplicate Bootstrap detected. Self-destroying this instance.");
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Compose Global Services
            _sceneNavigator = new UnitySceneNavigator();
            _gameFlowController = new GameFlowController(_sceneNavigator);
            _cts = new CancellationTokenSource();

            Debug.Log("[GameBootstrap] Global services composed successfully.");

            if (runOnAwake)
            {
                StartAppFlow();
            }
        }

        private async void StartAppFlow()
        {
            try
            {
                Debug.Log("[GameBootstrap] Starting application flow...");
                await _gameFlowController.StartupAsync(_cts.Token);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[GameBootstrap] Critical error during application startup: {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _instance = null;
                Debug.Log("[GameBootstrap] Bootstrap persistent root cleaned up.");
            }
        }

        public IGameFlowController GameFlow => _gameFlowController;

        public static IGameFlowController GetGameFlow()
        {
            if (_instance == null)
            {
                Debug.LogWarning("[GameBootstrap] No active GameBootstrap instance found! (If testing, load Bootstrap scene first).");
                return null;
            }
            return _instance.GameFlow;
        }
    }
}
