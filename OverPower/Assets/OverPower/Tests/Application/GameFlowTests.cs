using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using OverPower.Application;
using OverPower.Application.Ports.Logging;
using OverPower.Domain.Run;

namespace OverPower.Tests.Application
{
    [TestFixture]
    public class GameFlowTests
    {
        private class FakeSceneNavigator : ISceneNavigator
        {
            public GameSceneId RequestedScene { get; private set; } = GameSceneId.Bootstrap;
            public CancellationToken PropagatedToken { get; private set; }
            public int LoadCount { get; private set; }

            public GameSceneId CurrentScene => RequestedScene;

            public Task LoadSceneAsync(GameSceneId sceneId, CancellationToken cancellationToken = default)
            {
                RequestedScene = sceneId;
                PropagatedToken = cancellationToken;
                LoadCount++;
                return Task.CompletedTask;
            }
        }

        [Test]
        public async Task Startup_ShouldRequestMainMenu()
        {
            // Arrange
            var navigator = new FakeSceneNavigator();
            var controller = new GameFlowController(navigator, NullGameLogger.Instance);

            // Act
            await controller.StartupAsync();

            // Assert
            Assert.That(navigator.RequestedScene, Is.EqualTo(GameSceneId.MainMenu), "Startup must load MainMenu.");
            Assert.That(navigator.LoadCount, Is.EqualTo(1), "Load should be called exactly once.");
        }

        [Test]
        public async Task StartNewRun_ShouldRequestExploration()
        {
            // Arrange
            var navigator = new FakeSceneNavigator();
            var controller = new GameFlowController(navigator, NullGameLogger.Instance);

            // Act
            await controller.StartNewRunAsync(new RunSeed(123456789UL));

            // Assert
            Assert.That(navigator.RequestedScene, Is.EqualTo(GameSceneId.Exploration), "StartNewRun must load Exploration.");
        }

        [Test]
        public async Task GameFlow_ShouldPropagateCancellationToken()
        {
            // Arrange
            var navigator = new FakeSceneNavigator();
            var controller = new GameFlowController(navigator, NullGameLogger.Instance);
            var cts = new CancellationTokenSource();

            // Act
            await controller.StartupAsync(cts.Token);

            // Assert
            Assert.That(navigator.PropagatedToken, Is.EqualTo(cts.Token), "Cancellation token must be propagated to the navigator.");
        }
    }
}
