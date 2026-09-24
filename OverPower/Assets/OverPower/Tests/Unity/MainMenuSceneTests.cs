using System.Collections;
using NUnit.Framework;
using OverPower.Unity.Presentation.DesignSystem;
using OverPower.Unity.Presentation.MainMenu;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class MainMenuSceneTests
    {
        [UnityTest]
        public IEnumerator MainMenuScene_LoadsWithValidPremiumDesignComponents()
        {
            // Open the MainMenu scene
            EditorSceneManager.OpenScene("Assets/OverPower/Scenes/MainMenu.unity");
            yield return null;

            // 1. Verify MainMenuPresenter exists
            var presenter = Object.FindFirstObjectByType<MainMenuPresenter>();
            Assert.That(presenter, Is.Not.Null, "MainMenuPresenter must exist in the Main Menu scene.");

            // 2. Verify Canvas and MainPanel exist
            var canvasObj = GameObject.Find("Canvas");
            Assert.That(canvasObj, Is.Not.Null, "Canvas must exist.");

            var mainPanelObj = canvasObj.transform.Find("MainPanel");
            Assert.That(mainPanelObj, Is.Not.Null, "MainPanel must exist.");

            var uiPanel = mainPanelObj.GetComponent<UIPanel>();
            Assert.That(uiPanel, Is.Not.Null, "MainPanel must have UIPanel component attached.");

            // 3. Verify Play, Settings, and Exit buttons exist inside ActionBlock (no standalone LanguageButton)
            var actionBlock = mainPanelObj.Find("ActionBlock");
            Assert.That(actionBlock, Is.Not.Null, "ActionBlock must exist.");

            var playButtonObj = actionBlock.Find("PlayButton");
            Assert.That(playButtonObj, Is.Not.Null, "PlayButton must exist inside ActionBlock.");
            var playBtn = playButtonObj.GetComponent<UIButton>();
            Assert.That(playBtn, Is.Not.Null, "PlayButton must have UIButton component.");
            Assert.That(playBtn.Family, Is.EqualTo(UIButtonFamily.Primary), "PlayButton must be PrimaryButton.");

            var settingsButtonObj = actionBlock.Find("SettingsButton");
            Assert.That(settingsButtonObj, Is.Not.Null, "SettingsButton must exist inside ActionBlock.");
            var settingsBtn = settingsButtonObj.GetComponent<UIButton>();
            Assert.That(settingsBtn, Is.Not.Null, "SettingsButton must have UIButton component.");
            Assert.That(settingsBtn.Family, Is.EqualTo(UIButtonFamily.Secondary), "SettingsButton must be SecondaryButton.");

            var langButtonObj = actionBlock.Find("LanguageButton");
            Assert.That(langButtonObj, Is.Null, "Standalone LanguageButton must not exist in ActionBlock.");

            var exitButtonObj = actionBlock.Find("ExitButton");
            Assert.That(exitButtonObj, Is.Not.Null, "ExitButton must exist inside ActionBlock.");
            var exitBtn = exitButtonObj.GetComponent<UIButton>();
            Assert.That(exitBtn, Is.Not.Null, "ExitButton must have UIButton component.");
            Assert.That(exitBtn.Family, Is.EqualTo(UIButtonFamily.Secondary), "ExitButton must be SecondaryButton.");

            // 4. Verify SettingsModal exists with UISettingsModal and UITransition
            var settingsModalObj = canvasObj.transform.Find("SettingsModal");
            Assert.That(settingsModalObj, Is.Not.Null, "SettingsModal must exist under Canvas.");
            var settingsModal = settingsModalObj.GetComponent<UISettingsModal>();
            Assert.That(settingsModal, Is.Not.Null, "SettingsModal must have UISettingsModal component.");
            var modalTransition = settingsModalObj.GetComponent<UITransition>();
            Assert.That(modalTransition, Is.Not.Null, "SettingsModal must have UITransition component.");
            Assert.That(settingsModal.LanguageButton, Is.Not.Null, "SettingsModal must reference LanguageButton.");
            Assert.That(settingsModal.CloseButton, Is.Not.Null, "SettingsModal must reference CloseButton.");

            // 5. Verify MainMenuPresenter references
            Assert.That(presenter.PlayButton, Is.EqualTo(playBtn));
            Assert.That(presenter.SettingsButton, Is.EqualTo(settingsBtn));
            Assert.That(presenter.ExitButton, Is.EqualTo(exitBtn));
            Assert.That(presenter.SettingsModal, Is.EqualTo(settingsModal));
            Assert.That(presenter.MainPanelTransition, Is.Not.Null);

            // 6. Verify UIAudioFeedback and AudioSource exist
            var audioObj = GameObject.Find("UIAudio");
            Assert.That(audioObj, Is.Not.Null, "UIAudio GameObject must exist.");
            var audioSource = audioObj.GetComponent<AudioSource>();
            Assert.That(audioSource, Is.Not.Null, "UIAudio must have an AudioSource.");
            Assert.That(audioSource.spatialBlend, Is.EqualTo(0f), "AudioSource must be 2D.");
            Assert.That(audioSource.playOnAwake, Is.False, "AudioSource must not play on awake.");
            var uiAudio = audioObj.GetComponent<OverPower.Unity.Presentation.Audio.UIAudioFeedback>();
            Assert.That(uiAudio, Is.Not.Null, "UIAudio must have UIAudioFeedback component.");
            Assert.That(uiAudio.AudioSource, Is.EqualTo(audioSource), "UIAudioFeedback must reference the AudioSource.");

            // 7. Verify no missing scripts on any GameObject in the hierarchy
            foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach (var comp in go.GetComponentsInChildren<Component>(true))
                {
                    Assert.That(comp, Is.Not.Null, $"Missing script detected in Main Menu scene hierarchy under '{go.name}'.");
                }
            }
        }

        [UnityTest]
        public IEnumerator MainMenuScene_NavigationTopology_IsExplicitAndControllerReady()
        {
            EditorSceneManager.OpenScene("Assets/OverPower/Scenes/MainMenu.unity");
            yield return null;

            var canvasObj = GameObject.Find("Canvas");
            var actionBlock = canvasObj.transform.Find("MainPanel/ActionBlock");

            var playBtn = actionBlock.Find("PlayButton").GetComponent<UIButton>();
            var settingsBtn = actionBlock.Find("SettingsButton").GetComponent<UIButton>();
            var exitBtn = actionBlock.Find("ExitButton").GetComponent<UIButton>();

            // Play navigation: down -> Settings, up -> Exit (wrap)
            Assert.That(playBtn.navigation.mode, Is.EqualTo(UnityEngine.UI.Navigation.Mode.Explicit));
            Assert.That(playBtn.navigation.selectOnDown, Is.EqualTo(settingsBtn));
            Assert.That(playBtn.navigation.selectOnUp, Is.EqualTo(exitBtn));

            // Settings navigation: up -> Play, down -> Exit
            Assert.That(settingsBtn.navigation.mode, Is.EqualTo(UnityEngine.UI.Navigation.Mode.Explicit));
            Assert.That(settingsBtn.navigation.selectOnUp, Is.EqualTo(playBtn));
            Assert.That(settingsBtn.navigation.selectOnDown, Is.EqualTo(exitBtn));

            // Exit navigation: up -> Settings, down -> Play (wrap)
            Assert.That(exitBtn.navigation.mode, Is.EqualTo(UnityEngine.UI.Navigation.Mode.Explicit));
            Assert.That(exitBtn.navigation.selectOnUp, Is.EqualTo(settingsBtn));
            Assert.That(exitBtn.navigation.selectOnDown, Is.EqualTo(playBtn));

            // EventSystem initial selected
            var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            Assert.That(eventSystem, Is.Not.Null);
            Assert.That(eventSystem.firstSelectedGameObject, Is.EqualTo(playBtn.gameObject));
        }

        [UnityTest]
        public IEnumerator MainMenuScene_AllPlayerFacingLabels_HaveLocalizationReferences()
        {
            EditorSceneManager.OpenScene("Assets/OverPower/Scenes/MainMenu.unity");
            yield return null;

            var canvasObj = GameObject.Find("Canvas");
            var localizers = canvasObj.GetComponentsInChildren<UnityEngine.Localization.Components.LocalizeStringEvent>(true);
            Assert.That(localizers.Length, Is.GreaterThanOrEqualTo(6));

            var foundKeys = new System.Collections.Generic.HashSet<string>();
            foreach (var loc in localizers)
            {
                Assert.That(loc.StringReference.TableReference.TableCollectionName, Is.EqualTo("UI.Core"));
                foundKeys.Add(loc.StringReference.TableEntryReference.Key);
            }

            Assert.That(foundKeys.Contains("ui.main_menu.title"), Is.True, "Must contain ui.main_menu.title");
            Assert.That(foundKeys.Contains("ui.main_menu.play"), Is.True, "Must contain ui.main_menu.play");
            Assert.That(foundKeys.Contains("ui.main_menu.settings"), Is.True, "Must contain ui.main_menu.settings");
            Assert.That(foundKeys.Contains("ui.main_menu.language"), Is.True, "Must contain ui.main_menu.language");
            Assert.That(foundKeys.Contains("ui.main_menu.exit"), Is.True, "Must contain ui.main_menu.exit");
            Assert.That(foundKeys.Contains("ui.settings.title"), Is.True, "Must contain ui.settings.title");
            Assert.That(foundKeys.Contains("ui.common.close"), Is.True, "Must contain ui.common.close");
        }

        [UnityTest]
        public IEnumerator MainMenuScene_SettingsModal_FlowAndCancelSemantics_OperateCorrectly()
        {
            EditorSceneManager.OpenScene("Assets/OverPower/Scenes/MainMenu.unity");
            yield return null;

            var presenter = Object.FindFirstObjectByType<MainMenuPresenter>();
            Assert.That(presenter, Is.Not.Null);

            var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            Assert.That(eventSystem, Is.Not.Null);

            // Initially Play button is selected
            eventSystem.SetSelectedGameObject(presenter.PlayButton.gameObject);
            Assert.That(eventSystem.currentSelectedGameObject, Is.EqualTo(presenter.PlayButton.gameObject));

            var modal = presenter.SettingsModal;
            var mainPanelCg = presenter.MainPanelTransition.GetComponent<CanvasGroup>();

            // Simulate selecting SettingsButton and clicking it
            eventSystem.SetSelectedGameObject(presenter.SettingsButton.gameObject);
            Assert.That(eventSystem.currentSelectedGameObject, Is.EqualTo(presenter.SettingsButton.gameObject));

            modal.Open();
            yield return null;

            Assert.That(modal.IsOpen, Is.True, "Modal must be open.");
            Assert.That(mainPanelCg.interactable, Is.False, "Background must be non-interactable when modal is open.");
            Assert.That(mainPanelCg.blocksRaycasts, Is.False, "Background must not block raycasts when modal is open.");
            Assert.That(eventSystem.currentSelectedGameObject, Is.EqualTo(modal.LanguageButton.gameObject), "Modal should receive initial focus.");

            // Semantic Cancel closes modal and restores background and previous focus
            modal.OnCancel(new UnityEngine.EventSystems.BaseEventData(eventSystem));
            if (UnityEngine.Application.isPlaying)
            {
                yield return new WaitForSecondsRealtime(0.35f);
            }
            else
            {
                modal.CloseImmediate();
            }

            Assert.That(modal.IsOpen, Is.False, "Cancel must close modal.");
            Assert.That(mainPanelCg.interactable, Is.True, "Background interaction must be restored.");
            Assert.That(mainPanelCg.blocksRaycasts, Is.True, "Background raycasts must be restored.");
            Assert.That(eventSystem.currentSelectedGameObject, Is.EqualTo(presenter.SettingsButton.gameObject), "Focus must return to Settings button.");
        }

        [UnityTest]
        public IEnumerator MainMenuScene_UIButtonStates_ReflectInteractableState()
        {
            EditorSceneManager.OpenScene("Assets/OverPower/Scenes/MainMenu.unity");
            yield return null;

            var presenter = Object.FindFirstObjectByType<MainMenuPresenter>();
            var playBtn = presenter.PlayButton;

            // Initially interactable
            Assert.That(playBtn.interactable, Is.True);
            var so = new UnityEditor.SerializedObject(playBtn);
            var disabledMark = (GameObject)so.FindProperty("_disabledMark").objectReferenceValue;
            var focusMark = (GameObject)so.FindProperty("_focusMark").objectReferenceValue;

            Assert.That(disabledMark.activeSelf, Is.False, "Disabled mark should be inactive when button is interactable.");

            // Set non-interactable
            playBtn.interactable = false;
            var evaluateMethod = typeof(UnityEngine.UI.Selectable).GetMethod("EvaluateAndTransitionToSelectionState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            evaluateMethod?.Invoke(playBtn, null);
            yield return null;

            Assert.That(disabledMark.activeSelf, Is.True, "Disabled mark should be active when button is non-interactable.");

            // Restore interactable
            playBtn.interactable = true;
            evaluateMethod?.Invoke(playBtn, null);
            yield return null;
            Assert.That(disabledMark.activeSelf, Is.False);
        }

        [UnityTest]
        public IEnumerator MainMenuScene_AuraPulse_ExistsAndBehavesSafely()
        {
            EditorSceneManager.OpenScene("Assets/OverPower/Scenes/MainMenu.unity");
            yield return null;

            var canvasObj = GameObject.Find("Canvas");
            Assert.That(canvasObj, Is.Not.Null, "Canvas must exist.");

            var auraContainer = canvasObj.transform.Find("AuraContainer");
            Assert.That(auraContainer, Is.Not.Null, "AuraContainer must exist under Canvas.");

            var pulse = auraContainer.GetComponent<UIAuraPulse>();
            Assert.That(pulse, Is.Not.Null, "UIAuraPulse component must be attached to AuraContainer.");
            Assert.That(pulse.IsAnimated, Is.True, "Aura pulse should be animated by default.");
            Assert.That(pulse.BaseScale, Is.EqualTo(1.0f).Within(0.001f));
            Assert.That(pulse.PeakScale, Is.InRange(1.005f, 1.025f));
            Assert.That(pulse.HalfCycleDuration, Is.InRange(2.0f, 5.0f));

            var canvasGroup = auraContainer.GetComponent<CanvasGroup>();
            Assert.That(canvasGroup, Is.Not.Null, "CanvasGroup must exist on AuraContainer.");

            // Verify safe disable / resting state restoration
            pulse.enabled = false;
            Assert.That(auraContainer.localScale, Is.EqualTo(Vector3.one));
            Assert.That(canvasGroup.alpha, Is.EqualTo(pulse.BaseAlpha).Within(0.01f));

            // Verify safe re-enable
            pulse.enabled = true;
            Assert.That(pulse.IsAnimated, Is.True);

            // Verify reduced-motion toggle
            pulse.SetAnimated(false);
            Assert.That(pulse.IsAnimated, Is.False);
            Assert.That(auraContainer.localScale, Is.EqualTo(Vector3.one));
            Assert.That(canvasGroup.alpha, Is.EqualTo(pulse.BaseAlpha).Within(0.01f));

            pulse.SetAnimated(true);
            Assert.That(pulse.IsAnimated, Is.True);
        }

        [UnityTest]
        public IEnumerator MainMenuScene_CommittedAuraHierarchy_HasValidStructuralConfiguration()
        {
            EditorSceneManager.OpenScene("Assets/OverPower/Scenes/MainMenu.unity");
            yield return null;

            var canvasObj = GameObject.Find("Canvas");
            Assert.That(canvasObj, Is.Not.Null, "Canvas must exist.");

            // 1. Scene contains AuraContainer
            var auraContainer = canvasObj.transform.Find("AuraContainer");
            Assert.That(auraContainer, Is.Not.Null, "AuraContainer must exist under Canvas.");

            // 2. Contains CanvasGroup
            var canvasGroup = auraContainer.GetComponent<CanvasGroup>();
            Assert.That(canvasGroup, Is.Not.Null, "CanvasGroup must exist on AuraContainer.");

            // 3. Contains UIAuraPulse
            var pulse = auraContainer.GetComponent<UIAuraPulse>();
            Assert.That(pulse, Is.Not.Null, "UIAuraPulse must exist on AuraContainer.");

            // 4. Contains AuraVisual
            var auraVisual = auraContainer.Find("AuraVisual");
            Assert.That(auraVisual, Is.Not.Null, "AuraVisual must exist under AuraContainer.");

            // 5. UIAuraPulse enabled
            Assert.That(pulse.enabled, Is.True, "UIAuraPulse must be enabled.");

            // 6. AutoPlay enabled
            Assert.That(pulse.AutoPlay, Is.True, "AutoPlay must be enabled.");

            // 7. target RectTransform valid
            Assert.That(pulse.TargetTransform, Is.Not.Null, "Target RectTransform reference must be valid.");
            Assert.That(pulse.TargetTransform, Is.EqualTo(auraContainer.GetComponent<RectTransform>()), "Target RectTransform must point to AuraContainer.");

            // 8. CanvasGroup reference valid
            Assert.That(pulse.CanvasGroup, Is.Not.Null, "CanvasGroup reference on UIAuraPulse must be valid.");
            Assert.That(pulse.CanvasGroup, Is.EqualTo(canvasGroup), "CanvasGroup must point to AuraContainer's CanvasGroup.");

            // 9. AuraVisual active
            Assert.That(auraVisual.gameObject.activeSelf, Is.True, "AuraVisual GameObject must be active.");
            Assert.That(auraVisual.gameObject.activeInHierarchy, Is.True, "AuraVisual must be active in hierarchy.");
        }

        [Test]
        public void UI_InteractionFeedbackHooks_ExistAndCanBeSubscribed()
        {
            var btnGo = new GameObject("TestButton", typeof(RectTransform));
            var btn = btnGo.AddComponent<UIButton>();

            bool hoverEventFired = false;
            bool pressEventFired = false;
            bool submitEventFired = false;

            btn.OnHoverFeedback.AddListener(() => hoverEventFired = true);
            btn.OnPressFeedback.AddListener(() => pressEventFired = true);
            btn.OnSubmitFeedback.AddListener(() => submitEventFired = true);

            btn.OnHoverFeedback.Invoke();
            Assert.That(hoverEventFired, Is.True);

            btn.OnPressFeedback.Invoke();
            Assert.That(pressEventFired, Is.True);

            btn.OnSubmitFeedback.Invoke();
            Assert.That(submitEventFired, Is.True);

            bool cancelFeedbackFired = false;
            System.Action<UICancelRelay> cancelListener = (relay) => cancelFeedbackFired = true;
            UICancelRelay.OnCancelFeedback += cancelListener;

            var relayGo = new GameObject("TestCancelRelay");
            var relay = relayGo.AddComponent<UICancelRelay>();
            var esGo = new GameObject("ES");
            var es = esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            relay.OnCancel(new UnityEngine.EventSystems.BaseEventData(es));

            Assert.That(cancelFeedbackFired, Is.True);

            UICancelRelay.OnCancelFeedback -= cancelListener;

            Object.DestroyImmediate(btnGo);
            Object.DestroyImmediate(relayGo);
            Object.DestroyImmediate(esGo);
        }
    }
}
