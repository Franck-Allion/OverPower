using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using OverPower.Application;
using OverPower.Unity.SceneFlow;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class UnitySceneFlowTests
    {
        [Test]
        public void SceneNavigator_ShouldThrow_OnDuplicateTransition()
        {
            // Arrange
            var navigator = new UnitySceneNavigator();
            var cts = new CancellationTokenSource();

            // Act & Assert
            // Start transition A (which remains active in the async yield loop)
            Task taskA = navigator.LoadSceneAsync(GameSceneId.MainMenu, cts.Token);

            // Attempt to start transition B immediately while A is active
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await navigator.LoadSceneAsync(GameSceneId.Exploration, cts.Token);
            }, "Starting a second transition while one is active must throw InvalidOperationException.");

            // Clean up
            cts.Cancel();
        }
    }
}
