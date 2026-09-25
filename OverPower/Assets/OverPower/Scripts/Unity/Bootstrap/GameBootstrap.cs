using UnityEngine;
using System.Threading;
using OverPower.Application;
using OverPower.Application.Ports.Logging;
using OverPower.Unity.SceneFlow;
using OverPower.Unity.Input;
using OverPower.Unity.Localization;
using OverPower.Unity.Logging;

namespace OverPower.Unity.Bootstrap
{
    [DefaultExecutionOrder(-100)]
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private bool runOnAwake = true;

        private static GameBootstrap _instance;
        private UnitySceneNavigator _sceneNavigator;
        private GameFlowController _gameFlowController;
        private GameInputReader _inputReader;
        private UnityLocaleService _localeService;
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

            // Set up Logging with Verbosity Filter
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            GameLogVerbosity verbosity = GameLogVerbosity.Verbose;
#else
            GameLogVerbosity verbosity = GameLogVerbosity.Normal;
#endif

            IGameLogger rawLogger = new UnityGameLogger();
            IGameLogger logger = new VerbosityFilteredLogger(rawLogger, verbosity);

            _gameFlowController = new GameFlowController(_sceneNavigator, logger);
            _inputReader = gameObject.AddComponent<GameInputReader>();
            _localeService = new UnityLocaleService();
            _cts = new CancellationTokenSource();

            Debug.Log("[GameBootstrap] Global services composed successfully.");

            bool isBootstrapScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Bootstrap";
            if (runOnAwake && isBootstrapScene)
            {
                _ = StartAppFlowAsync();
            }
        }

        private async System.Threading.Tasks.Task StartAppFlowAsync()
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
        public IGameInput Input => _inputReader;
        public UnityLocaleService Locale => _localeService;
        public static bool IsInitialized => _instance != null;

        public static void EnsureInitialized()
        {
            if (_instance != null) return;

            var existing = Object.FindFirstObjectByType<GameBootstrap>();
            if (existing != null)
            {
                _instance = existing;
                return;
            }

            Debug.Log("[GameBootstrap] Auto-initializing GameBootstrap root for active scene execution.");
            var bootstrapObj = new GameObject("GameBootstrap");
            bootstrapObj.AddComponent<GameBootstrap>();
        }

        public static IGameFlowController GetGameFlow()
        {
            if (_instance == null)
            {
                EnsureInitialized();
            }
            return _instance != null ? _instance.GameFlow : null;
        }

        public static IGameInput GetInput()
        {
            if (_instance == null)
            {
                EnsureInitialized();
            }
            return _instance != null ? _instance.Input : null;
        }

        public static UnityLocaleService GetLocaleService()
        {
            if (_instance == null)
            {
                EnsureInitialized();
            }
            return _instance != null ? _instance.Locale : null;
        }
    }
}
