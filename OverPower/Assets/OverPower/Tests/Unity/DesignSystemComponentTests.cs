using System;
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
using Object = UnityEngine.Object;

namespace OverPower.Tests.Unity
{
    public sealed class DesignSystemComponentTests
    {
        private sealed class ClickCounter
        {
            public int Count { get; private set; }
            public void OnClick() => Count++;
        }

        private const string Components = "Assets/OverPower/UI/DesignSystem/Components/";

        [TestCase("Panel", typeof(UIPanel))]
        [TestCase("Badge", typeof(UIBadge))]
        [TestCase("ResourceChip", typeof(UIResourceChip))]
        [TestCase("Tooltip", typeof(UITooltip))]
        [TestCase("ConfirmDialog", typeof(UIConfirmDialog))]
        public void Prefab_LoadsWithValidPresentationReferences(string name, Type componentType)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Components + name + ".prefab");
            Assert.That(prefab, Is.Not.Null);
            Assert.That(prefab.GetComponent(componentType), Is.Not.Null);
            foreach (var transform in prefab.GetComponentsInChildren<Transform>(true))
                Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject), Is.Zero);
            foreach (var text in prefab.GetComponentsInChildren<TMP_Text>(true)) Assert.That(text.font, Is.Not.Null);
            foreach (var component in prefab.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (component.GetType().Namespace != typeof(UIPanel).Namespace) continue;
                var so = new SerializedObject(component);
                var property = so.GetIterator();
                while (property.NextVisible(true))
                {
                    if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                    // These documented injection points are supplied by scene composition; visual scaling and optional icons are not required on all variants.
                    if (new[] { "_background", "_bounds", "_canvas", "_visual", "_icon" }.Contains(property.name)) continue;
                    if (property.name.StartsWith("m_")) continue;
                    Assert.That(property.objectReferenceValue, Is.Not.Null, name + ": " + property.propertyPath);
                }
            }
        }

        [TestCase(9, -1, "9")]
        [TestCase(99, -1, "99")]
        [TestCase(999, -1, "999")]
        [TestCase(12, 20, "12")]
        [TestCase(100, 100, "100")]
        [TestCase(int.MaxValue, -1, "2147483647")]
        public void ResourceChip_FormatsValuesWithoutFloatRounding(int value, int maximum, string expected)
        {
            var instance = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Components + "ResourceChip.prefab"));
            try
            {
                var chip = instance.GetComponent<UIResourceChip>();
                if (maximum < 0) chip.SetValue(value); else chip.SetValue(value, maximum);
                Assert.That(instance.transform.Find("Value").GetComponent<TMP_Text>().text, Is.EqualTo(expected));
                var max = instance.transform.Find("Maximum");
                Assert.That(max.gameObject.activeSelf, Is.EqualTo(maximum >= 0));
                if (maximum >= 0) Assert.That(max.GetComponent<TMP_Text>().text, Is.EqualTo("/ " + maximum));
                chip.SetValue(9);
                Assert.That(max.gameObject.activeSelf, Is.False, "Switching to a single value removes the maximum.");
            }
            finally { Object.DestroyImmediate(instance); }
        }

        [UnityTest]
        public IEnumerator TooltipModalAndTransitions_HandleRapidInteractionAndLifetimes()
        {
            EditorSceneManager.OpenScene("Assets/OverPower/Scenes/Development/DesignSystemPreview.unity");
            yield return new EnterPlayMode();
            yield return null;
            var preview = Object.FindFirstObjectByType<DesignSystemPreviewView>();
            preview.ShowComponents();
            yield return null;
            var system = EventSystem.current ?? Object.FindFirstObjectByType<EventSystem>();
            var pointer = new PointerEventData(system);
            var tooltip = preview.Tooltip;
            var trigger = preview.TooltipTriggers[0];
            for (int i = 0; i < 30; i++) { trigger.OnPointerEnter(pointer); trigger.OnPointerExit(pointer); }
            yield return new WaitForSecondsRealtime(0.6f);
            Assert.That(tooltip.IsVisible, Is.False);
            trigger.OnSelect(new BaseEventData(system));
            yield return new WaitForSecondsRealtime(0.6f);
            Assert.That(tooltip.IsVisible, Is.True, "Focus can show details without hover.");
            trigger.OnDeselect(new BaseEventData(system));
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(tooltip.IsVisible, Is.False);
            trigger.OpenDetails();
            trigger.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.6f);
            Assert.That(tooltip.IsVisible, Is.False, "A pending delay must be cancelled on disable.");
            trigger.gameObject.SetActive(true);
            trigger.OpenDetails();
            yield return new WaitForSecondsRealtime(0.6f);
            Assert.That(tooltip.IsVisible, Is.True);
            Object.Destroy(trigger);
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(tooltip.IsVisible, Is.False, "Destroyed owners must release an open tooltip.");

            object owner = new object();
            foreach (var point in new[] { Vector2.zero, new Vector2(Screen.width, 0),
                new Vector2(0, Screen.height), new Vector2(Screen.width, Screen.height) })
            {
                tooltip.Show(owner, "Title", "Prepared tooltip content", null, point);
                yield return new WaitForSecondsRealtime(0.3f);
                Canvas.ForceUpdateCanvases();
                var corners = new Vector3[4];
                ((RectTransform)tooltip.transform).GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    var local = tooltip.Bounds.InverseTransformPoint(corner);
                    Assert.That(local.x, Is.InRange(tooltip.Bounds.rect.xMin - 1, tooltip.Bounds.rect.xMax + 1));
                    Assert.That(local.y, Is.InRange(tooltip.Bounds.rect.yMin - 1, tooltip.Bounds.rect.yMax + 1));
                }
            }
            var secondOwner = new object();
            tooltip.Show(secondOwner, "Other", "Content", null, Vector2.zero);
            tooltip.Hide(owner);
            Assert.That(tooltip.IsVisible, Is.True, "A previous owner cannot dismiss the current tooltip.");
            tooltip.Hide(secondOwner);

            var dialog = preview.Dialog;
            if (system.currentSelectedGameObject == null && preview.ComponentFocus != null)
            {
                system.SetSelectedGameObject(preview.ComponentFocus.gameObject);
            }
            var originalFocus = system.currentSelectedGameObject ?? preview.ComponentFocus.gameObject;
            var counter = new ClickCounter();
            originalFocus.GetComponent<UIButton>().onClick.AddListener(counter.OnClick);
            dialog.Open();
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(dialog.Background.interactable, Is.False);
            Assert.That(dialog.Background.blocksRaycasts, Is.False);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(dialog.Primary.gameObject));
            ExecuteEvents.Execute(originalFocus, pointer, ExecuteEvents.pointerClickHandler);
            Assert.That(counter.Count, Is.Zero, "Background buttons must reject events while the modal owns focus.");
            ExecuteEvents.Execute(dialog.Primary.gameObject, new AxisEventData(system) { moveDir = MoveDirection.Right }, ExecuteEvents.moveHandler);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(dialog.Secondary.gameObject));
            ExecuteEvents.Execute(dialog.Secondary.gameObject, new BaseEventData(system), ExecuteEvents.cancelHandler);
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(dialog.Background.interactable, Is.True);
            Assert.That(dialog.Background.blocksRaycasts, Is.True);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(originalFocus));
            dialog.Open(); dialog.Close(); dialog.Open();
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(dialog.IsOpen, Is.True);
            Assert.That(dialog.Background.interactable, Is.False, "A replaced close must not release the background.");
            dialog.gameObject.SetActive(false);
            Assert.That(dialog.Background.interactable, Is.True);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(originalFocus));
            dialog.gameObject.SetActive(true);
            dialog.Background.interactable = false;
            dialog.Open(); dialog.Close();
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(dialog.Background.interactable, Is.False, "Restore the original state, not an unconditional true.");
            dialog.Background.interactable = true;

            var reveal = Object.FindFirstObjectByType<DesignSystemPreviewView>()?.Reveal
                ?? GameObject.Find("RevealPanel")?.GetComponent<UITransition>()
                ?? Object.FindObjectsByType<UITransition>(FindObjectsSortMode.None).FirstOrDefault(t => t.name == "RevealPanel");
            Assert.That(reveal, Is.Not.Null, "Reveal transition must exist in preview.");
            var group = reveal.GetComponent<CanvasGroup>();
            for (int i = 0; i < 30; i++) { reveal.Show(); reveal.Hide(); }
            reveal.Show();
            yield return new WaitForSecondsRealtime(0.4f);
            Assert.That(group.alpha, Is.EqualTo(1f).Within(0.001f));
            Assert.That(reveal.IsAnimating, Is.False);
            reveal.Hide(); reveal.Hide(); reveal.ShowImmediate();
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(group.alpha, Is.EqualTo(1f));
            reveal.Hide(); reveal.gameObject.SetActive(false);
            Assert.That(reveal.IsAnimating, Is.False);
            reveal.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(group.alpha, Is.EqualTo(1f));
            reveal.Hide(); Object.Destroy(reveal.gameObject);
            var activeSystem = EventSystem.current ?? system;
            if (activeSystem != null) activeSystem.SetSelectedGameObject(originalFocus);
            dialog.Open(); Object.Destroy(dialog.gameObject);
            yield return new WaitForSecondsRealtime(0.4f);
            Assert.That((EventSystem.current ?? system).currentSelectedGameObject, Is.EqualTo(originalFocus));
            yield return new ExitPlayMode();
        }

        [UnityTearDown]
        public IEnumerator LeavePlayModeAfterFailure()
        {
            if (UnityEngine.Application.isPlaying) yield return new ExitPlayMode();
        }
    }
}
