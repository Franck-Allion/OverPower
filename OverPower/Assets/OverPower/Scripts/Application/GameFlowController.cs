using System;
using System.Threading;
using System.Threading.Tasks;
using OverPower.Domain.Run;
using OverPower.Application.Ports.Logging;

namespace OverPower.Application
{
    public class GameFlowController : IGameFlowController
    {
        private readonly ISceneNavigator _sceneNavigator;
        private readonly IGameLogger _logger;

        public GameFlowController(ISceneNavigator sceneNavigator, IGameLogger logger)
        {
            _sceneNavigator = sceneNavigator ?? throw new ArgumentNullException(nameof(sceneNavigator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartupAsync(CancellationToken cancellationToken = default)
        {
            await _sceneNavigator.LoadSceneAsync(GameSceneId.MainMenu, cancellationToken);
        }

        public async Task StartNewRunAsync(RunSeed seed, CancellationToken cancellationToken = default)
        {
            _logger.Info(GameLogCategories.RunStart, $"Run started with seed {seed.Value}");
            await _sceneNavigator.LoadSceneAsync(GameSceneId.Exploration, cancellationToken);
        }

        public async Task ReturnToMainMenuAsync(CancellationToken cancellationToken = default)
        {
            await _sceneNavigator.LoadSceneAsync(GameSceneId.MainMenu, cancellationToken);
        }
    }
}
