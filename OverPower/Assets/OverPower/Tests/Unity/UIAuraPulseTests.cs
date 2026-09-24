using System.Collections;
using DG.Tweening;
using NUnit.Framework;
using OverPower.Unity.Presentation.DesignSystem;
using UnityEngine;
using UnityEngine.TestTools;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class UIAuraPulseTests
    {
        [UnityTest]
        public IEnumerator UIAuraPulse_LifecycleAndAnimationBehavior_FollowsContract()
        {
            var rootObject = new GameObject("AuraPulseTestRoot", typeof(RectTransform), typeof(CanvasGroup));
            var rectTransform = rootObject.GetComponent<RectTransform>();
            var canvasGroup = rootObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0.5f;
            rectTransform.localScale = new Vector3(2f, 2f, 2f);

            var pulse = rootObject.AddComponent<UIAuraPulse>();
            pulse.ResolveReferences();
            yield return null;

            try
            {
                // 1. Valid references are resolved
                Assert.That(pulse.TargetTransform, Is.EqualTo(rectTransform), "TargetTransform must be resolved to self RectTransform.");
                Assert.That(pulse.CanvasGroup, Is.EqualTo(canvasGroup), "CanvasGroup must be resolved to self CanvasGroup.");
                Assert.That(pulse.AutoPlay, Is.True, "AutoPlay should be enabled by default.");

                // 2. Enabling with AutoPlay creates an active sequence
                Assert.That(pulse.isActiveAndEnabled, Is.True);
                pulse.Play();
                Assert.That(pulse.ActiveSequence, Is.Not.Null, "Active sequence must be created when enabled with AutoPlay.");
                Assert.That(pulse.ActiveSequence.IsActive(), Is.True, "Active sequence must be running.");

                // 3. Play/StartAnimation creates only one owned sequence
                var firstSeq = pulse.ActiveSequence;
                pulse.Play();
                var secondSeq = pulse.ActiveSequence;
                Assert.That(secondSeq, Is.Not.Null);
                Assert.That(secondSeq.IsActive(), Is.True);
                Assert.That(secondSeq, Is.Not.SameAs(firstSeq));

                pulse.StartAnimation();
                var thirdSeq = pulse.ActiveSequence;
                Assert.That(thirdSeq, Is.Not.Null);
                Assert.That(thirdSeq.IsActive(), Is.True);
                Assert.That(thirdSeq, Is.Not.SameAs(secondSeq));

                // 4. Stop kills/clears active sequence
                pulse.Stop();
                Assert.That(pulse.ActiveSequence, Is.Null, "ActiveSequence must be null after Stop.");
                Assert.That(canvasGroup.alpha, Is.EqualTo(pulse.BaseAlpha).Within(0.001f), "Stop must restore base alpha.");
                Assert.That(rectTransform.localScale, Is.EqualTo(Vector3.one * pulse.BaseScale), "Stop must restore base scale.");

                // 5. Disabling restores base alpha and base scale
                pulse.Play();
                Assert.That(pulse.ActiveSequence, Is.Not.Null);
                canvasGroup.alpha = 0.99f;
                rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                pulse.enabled = false;
                Assert.That(pulse.ActiveSequence, Is.Null, "ActiveSequence must be cleared on disable.");
                Assert.That(canvasGroup.alpha, Is.EqualTo(pulse.BaseAlpha).Within(0.001f), "Disabling must restore base alpha.");
                Assert.That(rectTransform.localScale, Is.EqualTo(Vector3.one * pulse.BaseScale), "Disabling must restore base scale.");

                // 6. Re-enabling does not accumulate multiple sequences
                pulse.enabled = true;
                pulse.Play();
                var reenabeledSeq1 = pulse.ActiveSequence;
                Assert.That(reenabeledSeq1, Is.Not.Null);
                Assert.That(reenabeledSeq1.IsActive(), Is.True);

                pulse.enabled = false;
                Assert.That(pulse.ActiveSequence, Is.Null);

                pulse.enabled = true;
                pulse.Play();
                var reenabeledSeq2 = pulse.ActiveSequence;
                Assert.That(reenabeledSeq2, Is.Not.Null);
                Assert.That(reenabeledSeq2.IsActive(), Is.True);

                // 7. SetAnimated(false) restores the resting state
                pulse.SetAnimated(false);
                Assert.That(pulse.IsAnimated, Is.False);
                Assert.That(pulse.ActiveSequence, Is.Null, "Sequence must be cleared when animation is disabled.");
                Assert.That(canvasGroup.alpha, Is.EqualTo(pulse.BaseAlpha).Within(0.001f), "Base alpha restored.");
                Assert.That(rectTransform.localScale, Is.EqualTo(Vector3.one * pulse.BaseScale), "Base scale restored.");

                pulse.SetAnimated(true);
                Assert.That(pulse.IsAnimated, Is.True);
                Assert.That(pulse.ActiveSequence, Is.Not.Null, "Sequence resumes when animation is re-enabled.");
                Assert.That(pulse.ActiveSequence.IsActive(), Is.True);
            }
            finally
            {
                pulse.Stop();
                if (UnityEngine.Application.isPlaying)
                {
                    Object.Destroy(rootObject);
                }
                else
                {
                    Object.DestroyImmediate(rootObject);
                }
            }
        }
    }
}
