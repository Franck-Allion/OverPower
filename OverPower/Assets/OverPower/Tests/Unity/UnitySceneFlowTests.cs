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

            // Since this test runs in EditMode where SceneManager.LoadSceneAsync throws play-mode only errors,
            // we must expect the resulting console errors to prevent the test runner from failing.
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Error, new System.Text.RegularExpressions.Regex(".*Error loading scene MainMenu.*"));
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Error, new System.Text.RegularExpressions.Regex(".*Error loading scene Exploration.*"));

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
