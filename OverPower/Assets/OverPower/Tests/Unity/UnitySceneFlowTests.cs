using NUnit.Framework;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using OverPower.Application;
using OverPower.Unity.Bootstrap;
using OverPower.Unity.Presentation.SceneTransition;
using OverPower.Unity.SceneFlow;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class UnitySceneFlowTests
    {
        private class FakeTransitionPresentation : ISceneTransitionPresentation
        {
            public int CoverCount { get; private set; }
            public int RevealCount { get; private set; }
            public int IndicatorShowCount { get; private set; }
            public int IndicatorHideCount { get; private set; }
            public bool IsCovered { get; private set; }
            public bool IsAnimating { get; private set; }

            public Task CoverAsync(CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                CoverCount++;
                IsCovered = true;
                return Task.CompletedTask;
            }

            public Task RevealAsync(CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                RevealCount++;
                IsCovered = false;
                return Task.CompletedTask;
            }

            public void ShowLoadingIndicator(bool visible)
            {
                if (visible) IndicatorShowCount++;
                else IndicatorHideCount++;
            }
        }

        [Test]
        public void SceneNavigator_ShouldThrow_OnDuplicateTransition()
        {
            var fakePresentation = new FakeTransitionPresentation();
            // Injects null loader which throws on scene load attempt, testing guard rejection while transition is active
            var navigator = new UnitySceneNavigator(fakePresentation);

            var cts = new CancellationTokenSource();

            // First transition starts and covers
            Task taskA = navigator.LoadSceneAsync(GameSceneId.MainMenu, cts.Token);

            // Immediate second transition must throw InvalidOperationException
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await navigator.LoadSceneAsync(GameSceneId.Exploration, cts.Token);
            }, "Starting a second transition while one is active must throw InvalidOperationException.");

            cts.Cancel();
        }

        [Test]
        public void SceneNavigator_CancellationBeforeLoad_ThrowsAndReleasesGuard()
        {
            var fakePresentation = new FakeTransitionPresentation();
            var navigator = new UnitySceneNavigator(fakePresentation);

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.CatchAsync<OperationCanceledException>(async () =>
            {
                await navigator.LoadSceneAsync(GameSceneId.MainMenu, cts.Token);
            });

            Assert.That(navigator.IsTransitioning, Is.False, "Transition guard must be false after immediate cancellation.");
            Assert.That(fakePresentation.CoverCount, Is.EqualTo(0), "Cover should not be called if cancelled before start.");
        }

        [Test]
        public void SceneNavigator_CoversScreen_BeforeAttemptingLoad()
        {
            var fakePresentation = new FakeTransitionPresentation();
            var navigator = new UnitySceneNavigator(fakePresentation);
            var cts = new CancellationTokenSource();

            try
            {
                Task loadTask = navigator.LoadSceneAsync(GameSceneId.MainMenu, cts.Token);
            }
            catch
            {
                // Ignored
            }

            Assert.That(fakePresentation.CoverCount, Is.GreaterThanOrEqualTo(1), "Screen must be covered before scene load proceeds.");
            cts.Cancel();
        }

        [Test]
        public void SceneNavigator_TransitionGuard_ReleasedAfterException()
        {
            // Pass a loader that throws an exception
            var fakePresentation = new FakeTransitionPresentation();
            var navigator = new UnitySceneNavigator(fakePresentation, null, path => throw new InvalidOperationException("Simulated load failure"));

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await navigator.LoadSceneAsync(GameSceneId.MainMenu);
            });

            Assert.That(navigator.IsTransitioning, Is.False, "Transition guard must be released after completion or failure.");
            Assert.That(fakePresentation.IsCovered, Is.False, "Presentation must be revealed/restored after exception.");
        }

        [UnityTest]
        public IEnumerator SceneFlow_BootstrapToMainMenu_CompletesWithOverlayRevealed()
        {
#if UNITY_EDITOR
            if (!UnityEngine.Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/OverPower/Scenes/Bootstrap.unity");
            }
            else
            {
                SceneManager.LoadScene("Assets/OverPower/Scenes/Bootstrap.unity");
            }
#else
            SceneManager.LoadScene("Assets/OverPower/Scenes/Bootstrap.unity");
#endif
            yield return null;

            if (UnityEngine.Application.isPlaying)
            {
                float timeout = 5f;
                while (SceneManager.GetActiveScene().name != "MainMenu" && timeout > 0f)
                {
                    timeout -= Time.unscaledDeltaTime;
                    yield return null;
                }

                Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("MainMenu"), "Bootstrap must transition to MainMenu.");
                Assert.That(GameBootstrap.IsInitialized, Is.True, "GameBootstrap should be initialized.");

                var overlay = GameBootstrap.GetTransitionOverlay();
                Assert.That(overlay, Is.Not.Null, "Transition overlay must exist under persistent bootstrap root.");
                
                timeout = 2f;
                while (overlay.IsAnimating && timeout > 0f)
                {
                    timeout -= Time.unscaledDeltaTime;
                    yield return null;
                }

                Assert.That(overlay.IsCovered, Is.False, "Overlay must be revealed after scene transition finishes.");
                Assert.That(overlay.CanvasGroup.blocksRaycasts, Is.False, "Overlay must not block raycasts after reveal.");
            }
        }
    }
}
