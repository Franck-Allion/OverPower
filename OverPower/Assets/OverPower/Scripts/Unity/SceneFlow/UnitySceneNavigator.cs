using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using OverPower.Application;

namespace OverPower.Unity.SceneFlow
{
    public class UnitySceneNavigator : ISceneNavigator
    {
        private bool _isTransitioning;
        private GameSceneId _currentScene = GameSceneId.Bootstrap;

        public GameSceneId CurrentScene => _currentScene;

        public async Task LoadSceneAsync(GameSceneId sceneId, CancellationToken cancellationToken = default)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning($"[SceneNavigator] Rejected request to load {sceneId} because a transition is already in progress.");
                throw new InvalidOperationException($"Transition already in progress. Rejected request to load {sceneId}.");
            }

            _isTransitioning = true;
            string scenePath = GameSceneCatalog.GetScenePath(sceneId);

            bool cancelled = false;
            try
            {
                Debug.Log($"[SceneNavigator] Starting transition to {sceneId} ({scenePath})...");

                AsyncOperation asyncOp = SceneManager.LoadSceneAsync(scenePath);
                if (asyncOp == null)
                {
                    throw new InvalidOperationException($"Failed to start loading scene: {scenePath}");
                }

                while (!asyncOp.isDone)
                {
                    if (!cancelled && cancellationToken.IsCancellationRequested)
                    {
                        cancelled = true;
                        Debug.LogWarning($"[SceneNavigator] Transition to {sceneId} cancellation requested, but waiting for active Unity load to complete...");
                    }
                    await Task.Yield();
                }

                if (cancelled)
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                _currentScene = sceneId;
                Debug.Log($"[SceneNavigator] Completed transition to {sceneId}.");
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"[SceneNavigator] Transition to {sceneId} was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneNavigator] Error loading scene {sceneId}: {ex.Message}");
                throw;
            }
            finally
            {
                _isTransitioning = false;
                Debug.Log($"[SceneNavigator] Transition guard released. Ready for the next transition.");
            }
        }
    }
}
