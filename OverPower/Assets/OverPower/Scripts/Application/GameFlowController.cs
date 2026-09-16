using System;
using System.Threading;
using System.Threading.Tasks;

namespace OverPower.Application
{
    public class GameFlowController : IGameFlowController
    {
        private readonly ISceneNavigator _sceneNavigator;

        public GameFlowController(ISceneNavigator sceneNavigator)
        {
            _sceneNavigator = sceneNavigator ?? throw new ArgumentNullException(nameof(sceneNavigator));
        }

        public async Task StartupAsync(CancellationToken cancellationToken = default)
        {
            await _sceneNavigator.LoadSceneAsync(GameSceneId.MainMenu, cancellationToken);
        }

        public async Task StartNewRunAsync(CancellationToken cancellationToken = default)
        {
            await _sceneNavigator.LoadSceneAsync(GameSceneId.Exploration, cancellationToken);
        }

        public async Task ReturnToMainMenuAsync(CancellationToken cancellationToken = default)
        {
            await _sceneNavigator.LoadSceneAsync(GameSceneId.MainMenu, cancellationToken);
        }
    }
}
