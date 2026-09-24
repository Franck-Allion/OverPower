using System.Collections;
using System.Linq;
using NUnit.Framework;
using OverPower.Unity.Presentation.DesignSystem;
using OverPower.Unity.Presentation.DesignSystem.Preview;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace OverPower.Tests.Unity
{
    public sealed class DesignSystemButtonTests
    {
        private const string Preview = "Assets/OverPower/Scenes/Development/DesignSystemPreview.unity";

        [TestCase("Primary")]
        [TestCase("Secondary")]
        [TestCase("Icon")]
        public void Prefab_HasCompleteReferencesAndCentralConfig(string family)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/OverPower/UI/DesignSystem/Components/" + family + "Button.prefab");
            Assert.That(asset, Is.Not.Null);
            var button = asset.GetComponent<UIButton>();
            var serialized = new SerializedObject(button);
            foreach (var field in new[] { "_config", "_visual", "_surface", "_border", "_focusMark", "_disabledMark", "_visualOpacity" })
                Assert.That(serialized.FindProperty(field).objectReferenceValue, Is.Not.Null, field);
            Assert.That(asset.GetComponentsInChildren<Transform>(true)
                .Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)), Is.Zero);
            foreach (var label in asset.GetComponentsInChildren<TMP_Text>(true)) Assert.That(label.font, Is.Not.Null);
            Assert.That(button.Family.ToString(), Is.EqualTo(family));
            if (family == "Icon") Assert.That(button.Icon.sprite, Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator Buttons_EventSystemAndLifecycle_RemainStable()
        {
            EditorSceneManager.OpenScene(Preview);
            yield return new EnterPlayMode();
            yield return null;
            var preview = Object.FindFirstObjectByType<DesignSystemPreviewView>();
            preview.ShowFoundations();
            Canvas.ForceUpdateCanvases();
            var eventSystem = EventSystem.current ?? Object.FindFirstObjectByType<EventSystem>();
            Assert.That(eventSystem, Is.Not.Null);
            eventSystem.SetSelectedGameObject(preview.ButtonSamples[0].gameObject);
            var buttons = preview.ButtonSamples;
            Assert.That(buttons.Length, Is.EqualTo(6));
            Assert.That(eventSystem.currentSelectedGameObject.GetComponent<UIButton>().Family, Is.EqualTo(UIButtonFamily.Primary));
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject,
                new AxisEventData(eventSystem) { moveDir = MoveDirection.Down }, ExecuteEvents.moveHandler);
            Assert.That(eventSystem.currentSelectedGameObject.GetComponent<UIButton>().Family, Is.EqualTo(UIButtonFamily.Secondary),
                "Keyboard navigation must start immediately and skip the disabled primary.");
            eventSystem.SetSelectedGameObject(null);
            foreach (var button in buttons.Where(b => b.interactable))
            {
                var pointer = new PointerEventData(eventSystem) { button = PointerEventData.InputButton.Left };
                var visual = button.transform.GetChild(0);
                var clicks = 0;
                button.onClick.AddListener(() => clicks++);
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
                yield return new WaitForSecondsRealtime(0.3f);
                Assert.That(visual.localScale.x, Is.GreaterThan(1f), "Hover must visibly emphasize the visual.");
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
                yield return new WaitForSecondsRealtime(0.2f);
                Assert.That(visual.localScale.x, Is.LessThan(1f), "Press must compress the visual.");
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerExitHandler);
                for (int i = 0; i < 30; i++)
                {
                    ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
                    ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
                    ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
                    ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                    ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerExitHandler);
                }
                Assert.That(clicks, Is.EqualTo(30));
                eventSystem.SetSelectedGameObject(null);
                yield return new WaitForSecondsRealtime(0.4f);
                Assert.That(Vector3.Distance(visual.localScale, Vector3.one), Is.LessThan(0.001f));
                eventSystem.SetSelectedGameObject(button.gameObject);
                yield return null;
                Assert.That(visual.Find("Focus").gameObject.activeSelf, Is.True);
                ExecuteEvents.Execute(button.gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
                Assert.That(clicks, Is.EqualTo(31));
                button.interactable = false;
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(button.gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
                Assert.That(clicks, Is.EqualTo(31), "Disabled controls must reject both click and submit.");
                Assert.That(visual.Find("UnavailableBar").gameObject.activeSelf, Is.True);
                Assert.That(visual.localScale, Is.EqualTo(Vector3.one));
                button.interactable = true;
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
                button.gameObject.SetActive(false);
                Assert.That(visual.localScale, Is.EqualTo(Vector3.one));
                button.gameObject.SetActive(true);
                eventSystem.SetSelectedGameObject(null);
                yield return new WaitForSecondsRealtime(0.4f);
                Assert.That(visual.localScale, Is.EqualTo(Vector3.one));
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
                Object.Destroy(button.gameObject);
                yield return new WaitForSecondsRealtime(0.4f);
            }
            yield return new ExitPlayMode();
        }
    }
}
