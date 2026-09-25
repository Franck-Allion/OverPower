using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using OverPower.Unity.Presentation.SceneTransition;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class UISceneTransitionOverlayTests
    {
        [Test]
        public void Overlay_InitialState_IsRevealedAndDoesNotBlockRaycasts()
        {
            var overlay = UISceneTransitionOverlay.Create(null);

            Assert.That(overlay.IsCovered, Is.False, "Overlay should start in revealed state.");
            Assert.That(overlay.CanvasGroup.alpha, Is.EqualTo(0f), "Alpha should start at 0.");
            Assert.That(overlay.CanvasGroup.blocksRaycasts, Is.False, "Overlay should not block raycasts initially.");
            Assert.That(overlay.LoadingIndicator, Is.Not.Null, "Loading indicator must exist.");

            Object.DestroyImmediate(overlay.gameObject);
        }

        [Test]
        public void Overlay_SetCoveredImmediate_SetsAlphaOneAndBlocksRaycasts()
        {
            var overlay = UISceneTransitionOverlay.Create(null);

            overlay.SetCoveredImmediate();

            Assert.That(overlay.IsCovered, Is.True, "IsCovered should be true.");
            Assert.That(overlay.CanvasGroup.alpha, Is.EqualTo(1f), "Alpha should be 1.");
            Assert.That(overlay.CanvasGroup.blocksRaycasts, Is.True, "Covered overlay must block raycasts.");

            Object.DestroyImmediate(overlay.gameObject);
        }

        [Test]
        public void Overlay_SetRevealedImmediate_SetsAlphaZeroAndUnblocksRaycasts()
        {
            var overlay = UISceneTransitionOverlay.Create(null);
            overlay.SetCoveredImmediate();

            overlay.SetRevealedImmediate();

            Assert.That(overlay.IsCovered, Is.False, "IsCovered should be false.");
            Assert.That(overlay.CanvasGroup.alpha, Is.EqualTo(0f), "Alpha should be 0.");
            Assert.That(overlay.CanvasGroup.blocksRaycasts, Is.False, "Revealed overlay must not block raycasts.");

            Object.DestroyImmediate(overlay.gameObject);
        }

        [Test]
        public void Overlay_CoverAndRevealAsync_FollowFullCoverageContract()
        {
            var overlay = UISceneTransitionOverlay.Create(null);
            overlay.CoverDuration = 0f;
            overlay.RevealDuration = 0f;

            var coverTask = overlay.CoverAsync();
            Assert.That(coverTask.IsCompleted, Is.True);
            Assert.That(overlay.IsCovered, Is.True);
            Assert.That(overlay.CanvasGroup.alpha, Is.EqualTo(1f));
            Assert.That(overlay.CanvasGroup.blocksRaycasts, Is.True);

            var revealTask = overlay.RevealAsync();
            Assert.That(revealTask.IsCompleted, Is.True);
            Assert.That(overlay.IsCovered, Is.False);
            Assert.That(overlay.CanvasGroup.alpha, Is.EqualTo(0f));
            Assert.That(overlay.CanvasGroup.blocksRaycasts, Is.False);

            Object.DestroyImmediate(overlay.gameObject);
        }

        [Test]
        public void Overlay_LoadingIndicator_CanBeShownAndHiddenSafely()
        {
            var overlay = UISceneTransitionOverlay.Create(null);

            Assert.DoesNotThrow(() => overlay.ShowLoadingIndicator(true));
            Assert.DoesNotThrow(() => overlay.ShowLoadingIndicator(false));

            Object.DestroyImmediate(overlay.gameObject);
        }

        [Test]
        public void Overlay_RapidCoverAndReveal_DoesNotThrow()
        {
            var overlay = UISceneTransitionOverlay.Create(null);

            Assert.DoesNotThrow(() =>
            {
                overlay.SetCoveredImmediate();
                overlay.SetRevealedImmediate();
                overlay.SetCoveredImmediate();
                overlay.SetRevealedImmediate();
            });

            Object.DestroyImmediate(overlay.gameObject);
        }

        [Test]
        public void Overlay_DisablingAndDestroying_CleansUpSafely()
        {
            var overlay = UISceneTransitionOverlay.Create(null);
            overlay.ShowLoadingIndicator(true);

            Assert.DoesNotThrow(() =>
            {
                overlay.enabled = false;
                overlay.enabled = true;
                Object.DestroyImmediate(overlay.gameObject);
            });
        }
    }
}
