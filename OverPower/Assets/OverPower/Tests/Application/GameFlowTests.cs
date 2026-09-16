using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using OverPower.Application;

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
            var controller = new GameFlowController(navigator);

            // Act
            await controller.StartupAsync();

            // Assert
            Assert.AreEqual(GameSceneId.MainMenu, navigator.RequestedScene, "Startup must load MainMenu.");
            Assert.AreEqual(1, navigator.LoadCount, "Load should be called exactly once.");
        }

        [Test]
        public async Task StartNewRun_ShouldRequestExploration()
        {
            // Arrange
            var navigator = new FakeSceneNavigator();
            var controller = new GameFlowController(navigator);

            // Act
            await controller.StartNewRunAsync();

            // Assert
            Assert.AreEqual(GameSceneId.Exploration, navigator.RequestedScene, "StartNewRun must load Exploration.");
        }

        [Test]
        public async Task GameFlow_ShouldPropagateCancellationToken()
        {
            // Arrange
            var navigator = new FakeSceneNavigator();
            var controller = new GameFlowController(navigator);
            var cts = new CancellationTokenSource();

            // Act
            await controller.StartupAsync(cts.Token);

            // Assert
            Assert.AreEqual(cts.Token, navigator.PropagatedToken, "Cancellation token must be propagated to the navigator.");
        }
    }
}
