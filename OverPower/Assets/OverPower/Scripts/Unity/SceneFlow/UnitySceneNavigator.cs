using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using OverPower.Application;
using OverPower.Application.Ports.Logging;

namespace OverPower.Unity.SceneFlow
{
    public class UnitySceneNavigator : ISceneNavigator
    {
        private readonly ISceneTransitionPresentation _presentation;
        private readonly IGameLogger _logger;
        private readonly Func<string, AsyncOperation> _sceneLoader;
        private bool _isTransitioning;
        private GameSceneId _currentScene = GameSceneId.Bootstrap;

        public bool IsTransitioning => _isTransitioning;
        public GameSceneId CurrentScene => _currentScene;
        public ISceneTransitionPresentation Presentation => _presentation;

        public UnitySceneNavigator(
            ISceneTransitionPresentation presentation = null, 
            IGameLogger logger = null,
            Func<string, AsyncOperation> sceneLoader = null)
        {
            _presentation = presentation;
            _logger = logger;
            _sceneLoader = sceneLoader ?? (path => SceneManager.LoadSceneAsync(path));
        }

        public async Task LoadSceneAsync(GameSceneId sceneId, CancellationToken cancellationToken = default)
        {
            if (_isTransitioning)
            {
                LogWarning($"[SceneNavigator] Rejected request to load {sceneId} because a transition is already in progress.");
                throw new InvalidOperationException($"Transition already in progress. Rejected request to load {sceneId}.");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            _isTransitioning = true;
            string scenePath = GameSceneCatalog.GetScenePath(sceneId);

            // Phase 1: Cover current scene before initiating engine load
            if (_presentation != null)
            {
                try
                {
                    await _presentation.CoverAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    await _presentation.RevealAsync(CancellationToken.None);
                    _isTransitioning = false;
                    throw;
                }
                catch (Exception)
                {
                    await _presentation.RevealAsync(CancellationToken.None);
                    _isTransitioning = false;
                    throw;
                }
            }

            // Phase 2: Engine scene load with delayed indeterminate indicator
            bool cancelled = false;
            try
            {
                LogInfo($"[SceneNavigator] Starting transition to {sceneId} ({scenePath})...");

                AsyncOperation asyncOp = _sceneLoader(scenePath);
                if (asyncOp == null)
                {
                    throw new InvalidOperationException($"Failed to start loading scene: {scenePath}");
                }

                float loadStartTime = Time.realtimeSinceStartup;
                bool indicatorShown = false;
                const float indicatorDelaySeconds = 0.30f;

                while (!asyncOp.isDone)
                {
                    if (!indicatorShown && (Time.realtimeSinceStartup - loadStartTime) >= indicatorDelaySeconds)
                    {
                        _presentation?.ShowLoadingIndicator(true);
                        indicatorShown = true;
                    }

                    if (!cancelled && cancellationToken.IsCancellationRequested)
                    {
                        cancelled = true;
                        LogWarning($"[SceneNavigator] Transition to {sceneId} cancellation requested, but waiting for active Unity load to complete...");
                    }
                    await Task.Yield();
                }

                if (indicatorShown)
                {
                    _presentation?.ShowLoadingIndicator(false);
                }

                _currentScene = sceneId;
                LogInfo($"[SceneNavigator] Completed transition to {sceneId}.");

                // Phase 3: Reveal new scene
                if (_presentation != null)
                {
                    await _presentation.RevealAsync(CancellationToken.None);
                }

                if (cancelled)
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                LogWarning($"[SceneNavigator] Transition to {sceneId} was cancelled.");
                if (_presentation != null && _presentation.IsCovered)
                {
                    await _presentation.RevealAsync(CancellationToken.None);
                }
                throw;
            }
            catch (Exception ex)
            {
                LogError($"[SceneNavigator] Error loading scene {sceneId}: {ex.Message}");
                if (_presentation != null && _presentation.IsCovered)
                {
                    await _presentation.RevealAsync(CancellationToken.None);
                }
                throw;
            }
            finally
            {
                _presentation?.ShowLoadingIndicator(false);
                _isTransitioning = false;
                LogInfo("[SceneNavigator] Transition guard released. Ready for the next transition.");
            }
        }

        private const string SceneFlowCategory = "SCENE.FLOW";

        private void LogInfo(string message)
        {
            if (_logger != null) _logger.Info(SceneFlowCategory, message);
            else Debug.Log(message);
        }

        private void LogWarning(string message)
        {
            if (_logger != null) _logger.Warning(SceneFlowCategory, message);
            else Debug.LogWarning(message);
        }

        private void LogError(string message)
        {
            if (_logger != null) _logger.Error(SceneFlowCategory, message);
            else Debug.LogError(message);
        }
    }
}
