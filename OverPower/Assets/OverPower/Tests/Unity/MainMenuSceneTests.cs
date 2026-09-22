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

            // 3. Verify Play, Language, and Exit buttons exist inside ActionBlock
            var actionBlock = mainPanelObj.Find("ActionBlock");
            Assert.That(actionBlock, Is.Not.Null, "ActionBlock must exist.");

            var playButtonObj = actionBlock.Find("PlayButton");
            Assert.That(playButtonObj, Is.Not.Null, "PlayButton must exist inside ActionBlock.");
            var playBtn = playButtonObj.GetComponent<UIButton>();
            Assert.That(playBtn, Is.Not.Null, "PlayButton must have UIButton component.");

            var langButtonObj = actionBlock.Find("LanguageButton");
            Assert.That(langButtonObj, Is.Not.Null, "LanguageButton must exist inside ActionBlock.");
            var langBtn = langButtonObj.GetComponent<UIButton>();
            Assert.That(langBtn, Is.Not.Null, "LanguageButton must have UIButton component.");

            var exitButtonObj = actionBlock.Find("ExitButton");
            Assert.That(exitButtonObj, Is.Not.Null, "ExitButton must exist inside ActionBlock.");
            var exitBtn = exitButtonObj.GetComponent<UIButton>();
            Assert.That(exitBtn, Is.Not.Null, "ExitButton must have UIButton component.");

            // 4. Verify no missing scripts on any GameObject in the hierarchy
            foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach (var comp in go.GetComponentsInChildren<Component>(true))
                {
                    Assert.That(comp, Is.Not.Null, $"Missing script detected in Main Menu scene hierarchy under '{go.name}'.");
                }
            }
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
    }
}
