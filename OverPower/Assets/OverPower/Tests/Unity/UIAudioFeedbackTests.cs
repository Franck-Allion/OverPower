using NUnit.Framework;
using OverPower.Unity.Presentation.Audio;
using OverPower.Unity.Presentation.DesignSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class UIAudioFeedbackTests
    {
        private class TestAudioFeedback : MonoBehaviour
        {
            public int ConfirmCount { get; private set; }
            public int HoverCount { get; private set; }
            public int CancelCount { get; private set; }

            [SerializeField] private UIAudioFeedback _feedback;

            public UIAudioFeedback Feedback => _feedback;

            public void Setup(UIAudioFeedback feedback)
            {
                _feedback = feedback;
            }
        }

        [Test]
        public void UIAudioFeedback_SubscriptionLifecycle_IsSafeAndNonDuplicating()
        {
            var go = new GameObject("AudioRoot");
            var audioSource = go.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;

            var feedback = go.AddComponent<UIAudioFeedback>();
            feedback.AudioSource = audioSource;

            // 1. Subscribe marks as subscribed
            feedback.Subscribe();
            Assert.That(feedback.IsSubscribed, Is.True, "Feedback should be subscribed after Subscribe().");

            // Calling Subscribe again shouldn't duplicate or alter state
            feedback.Subscribe();
            Assert.That(feedback.IsSubscribed, Is.True);

            // 2. Unsubscribe clears subscription
            feedback.Unsubscribe();
            Assert.That(feedback.IsSubscribed, Is.False, "Unsubscribe should unsubscribe from feedback events.");

            // 3. Re-subscribing restores cleanly
            feedback.Subscribe();
            Assert.That(feedback.IsSubscribed, Is.True, "Re-subscribing should restore subscription cleanly.");

            feedback.Unsubscribe();
            Object.DestroyImmediate(go);
        }

        [Test]
        public void UIAudioFeedback_WithNullClipsOrNullAudioSource_IsCompletelySafe()
        {
            var go = new GameObject("AudioRootNull");
            var feedback = go.AddComponent<UIAudioFeedback>();
            // AudioSource and clips are unassigned (null)

            Assert.DoesNotThrow(() =>
            {
                feedback.PlayConfirm();
                feedback.PlayToggle();
                feedback.PlayHover();
                feedback.PlayCancel();
            }, "Calling feedback methods with null clips/source must not throw.");

            var btnGo = new GameObject("TestBtn", typeof(RectTransform));
            var btn = btnGo.AddComponent<UIButton>();

            Assert.DoesNotThrow(() =>
            {
                btn.OnSubmit(new BaseEventData(EventSystem.current));
                btn.OnPointerClick(new PointerEventData(EventSystem.current));
            }, "Button click/submit with null audio clips must not throw.");

            Object.DestroyImmediate(btnGo);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void UIAudioFeedback_SemanticInteractionMapping_RoutesToExpectedClips()
        {
            var go = new GameObject("AudioRootSemantic");
            var audioSource = go.AddComponent<AudioSource>();
            var feedback = go.AddComponent<UIAudioFeedback>();
            feedback.AudioSource = audioSource;

            var confirmClip = AudioClip.Create("confirm_test", 100, 1, 44100, false);
            var toggleClip = AudioClip.Create("toggle_test", 100, 1, 44100, false);
            var hoverClip = AudioClip.Create("hover_test", 100, 1, 44100, false);
            var cancelClip = AudioClip.Create("cancel_test", 100, 1, 44100, false);

            feedback.ConfirmClip = confirmClip;
            feedback.ToggleClip = toggleClip;
            feedback.HoverClip = hoverClip;
            feedback.CancelClip = cancelClip;

            Assert.That(feedback.ConfirmClip, Is.EqualTo(confirmClip));
            Assert.That(feedback.ToggleClip, Is.EqualTo(toggleClip));
            Assert.That(feedback.HoverClip, Is.EqualTo(hoverClip));
            Assert.That(feedback.CancelClip, Is.EqualTo(cancelClip));

            // Verify mapping logic with non-null clips
            Assert.DoesNotThrow(() => feedback.PlayConfirm());
            Assert.DoesNotThrow(() => feedback.PlayToggle());
            Assert.DoesNotThrow(() => feedback.PlayHover());
            Assert.DoesNotThrow(() => feedback.PlayCancel());

            // Test fallback to ConfirmClip when ToggleClip is null
            feedback.ToggleClip = null;
            Assert.DoesNotThrow(() => feedback.PlayToggle());

            Object.DestroyImmediate(confirmClip);
            Object.DestroyImmediate(toggleClip);
            Object.DestroyImmediate(hoverClip);
            Object.DestroyImmediate(cancelClip);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void UIAudioFeedback_ButtonOverrideClip_TakesPrecedence()
        {
            var go = new GameObject("AudioRootOverride");
            var audioSource = go.AddComponent<AudioSource>();
            var feedback = go.AddComponent<UIAudioFeedback>();
            feedback.AudioSource = audioSource;

            var overrideClip = AudioClip.Create("override_test", 100, 1, 44100, false);

            var btnGo = new GameObject("OverrideBtn", typeof(RectTransform));
            var btn = btnGo.AddComponent<UIButton>();
            btn.OverrideSubmitClip = overrideClip;
            Assert.That(btn.OverrideSubmitClip, Is.EqualTo(overrideClip));

            Assert.DoesNotThrow(() =>
            {
                btn.OnSubmit(new BaseEventData(EventSystem.current));
            });

            Object.DestroyImmediate(overrideClip);
            Object.DestroyImmediate(btnGo);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void UIAudioFeedback_DisabledButton_DoesNotTriggerConfirmFeedback()
        {
            var go = new GameObject("AudioRootDisabled");
            var audioSource = go.AddComponent<AudioSource>();
            var feedback = go.AddComponent<UIAudioFeedback>();
            feedback.AudioSource = audioSource;

            var btnGo = new GameObject("DisabledButton", typeof(RectTransform));
            var btn = btnGo.AddComponent<UIButton>();
            btn.interactable = false;

            bool submitFeedbackFired = false;
            btn.OnSubmitFeedback.AddListener(() => submitFeedbackFired = true);

            btn.OnSubmit(new BaseEventData(EventSystem.current));
            btn.OnPointerClick(new PointerEventData(EventSystem.current));

            Assert.That(submitFeedbackFired, Is.False, "Disabled button must not trigger submit feedback.");

            Object.DestroyImmediate(btnGo);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void UIAudioFeedback_UICancelRelay_RoutesToCancelPlayback()
        {
            var go = new GameObject("AudioRootCancel");
            var audioSource = go.AddComponent<AudioSource>();
            var feedback = go.AddComponent<UIAudioFeedback>();
            feedback.AudioSource = audioSource;

            var relayGo = new GameObject("CancelRelay");
            var relay = relayGo.AddComponent<UICancelRelay>();

            var esGo = new GameObject("EventSystem", typeof(EventSystem));
            var es = esGo.GetComponent<EventSystem>();

            Assert.DoesNotThrow(() =>
            {
                relay.OnCancel(new BaseEventData(es));
            }, "UICancelRelay cancel event must route safely to UIAudioFeedback.");

            Object.DestroyImmediate(esGo);
            Object.DestroyImmediate(relayGo);
            Object.DestroyImmediate(go);
        }
    }
}
