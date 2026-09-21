using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem.Editor
{
    public static partial class DesignSystemPreviewBuilder
    {
        private static GameObject CreatePanel()
        {
            var root = Rect("Panel", null);
            root.gameObject.SetActive(false);
            root.sizeDelta = new Vector2(480, 200);
            PanelSurface(root);
            Vertical(root, UISpacing.Xl);
            PlainText("Header", root, TypographyStyle.Heading, 40);
            var content = Rect("Content", root);
            Vertical(content, 0);
            var layout = content.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 64;
            layout.flexibleHeight = 1;
            return SaveComponent(root);
        }

        private static GameObject CreateBadge()
        {
            var root = Rect("Badge", null);
            root.gameObject.SetActive(false);
            PanelSurface(root, UIPanelStyle.Subtle);
            Horizontal(root, UISpacing.Md, UISpacing.Sm);
            var sizing = root.gameObject.AddComponent<LayoutElement>();
            sizing.minHeight = 40;
            var accent = Image("Accent", root, _config.GetColor(UISemanticColor.Secondary));
            FixedSize(accent.rectTransform, 3, 20);
            var icon = Image("Icon", root, Color.white);
            FixedSize(icon.rectTransform, 20, 20);
            var label = PlainText("Label", root, TypographyStyle.Caption, 24);
            var badge = root.gameObject.AddComponent<UIBadge>();
            SetReference(badge, "_config", _config);
            SetReference(badge, "_accent", accent);
            SetReference(badge, "_icon", icon);
            SetReference(badge, "_label", label);
            return SaveComponent(root);
        }

        private static GameObject CreateResourceChip()
        {
            var root = Rect("ResourceChip", null);
            root.gameObject.SetActive(false);
            PanelSurface(root);
            Horizontal(root, UISpacing.Lg, UISpacing.Sm);
            var icon = Image("Icon", root, Color.white);
            icon.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            FixedSize(icon.rectTransform, 20, 20);
            
            var value = PlainText("Value", root, TypographyStyle.Stat, 40);
            var valueLayout = value.GetComponent<LayoutElement>();
            if (valueLayout != null)
            {
                valueLayout.minWidth = 32;
                valueLayout.flexibleWidth = 1;
            }

            var maximum = PlainText("Maximum", root, TypographyStyle.BodySmall, 32);
            var maxLayout = maximum.GetComponent<LayoutElement>();
            if (maxLayout != null)
            {
                maxLayout.minWidth = 48;
                maxLayout.flexibleWidth = 0;
            }

            var chip = root.gameObject.AddComponent<UIResourceChip>();
            SetReference(chip, "_config", _config);
            SetReference(chip, "_icon", icon);
            SetReference(chip, "_valueLabel", value);
            SetReference(chip, "_maximumLabel", maximum);
            return SaveComponent(root);
        }

        private static GameObject CreateTooltip()
        {
            var root = Rect("Tooltip", null);
            root.gameObject.SetActive(false);
            root.pivot = new Vector2(0, 1);
            root.sizeDelta = new Vector2(420, 240);
            PanelSurface(root, UIPanelStyle.Elevated);
            Vertical(root, UISpacing.Xl);
            var fit = root.gameObject.AddComponent<ContentSizeFitter>();
            fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var icon = Image("Icon", root, _config.GetColor(UISemanticColor.Primary));
            FixedSize(icon.rectTransform, 24, 24);
            var title = PlainText("Title", root, TypographyStyle.Heading, 40);
            var body = PlainText("Body", root, TypographyStyle.BodySmall, 112);
            body.textWrappingMode = TextWrappingModes.Normal;
            body.overflowMode = TextOverflowModes.Ellipsis;
            var transition = Transition(root, null, false, false);
            var tooltip = root.gameObject.AddComponent<UITooltip>();
            SetReference(tooltip, "_config", _config);
            SetReference(tooltip, "_transition", transition);
            SetReference(tooltip, "_title", title);
            SetReference(tooltip, "_body", body);
            SetReference(tooltip, "_icon", icon);
            return SaveComponent(root);
        }

        private static GameObject CreateConfirmDialog(GameObject panelPrefab)
        {
            var root = Rect("ConfirmDialog", null);
            root.gameObject.SetActive(false);
            Stretch(root, 0);
            var backdrop = root.gameObject.AddComponent<Image>();
            backdrop.raycastTarget = true;
            var panel = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, root);
            var panelRect = (RectTransform)panel.transform;
            panelRect.sizeDelta = new Vector2(740, 360);
            panel.GetComponent<UIPanel>().SetStyle(UIPanelStyle.Elevated);
            var title = panel.transform.Find("Header").GetComponent<TMP_Text>();
            var content = (RectTransform)panel.transform.Find("Content");
            var body = PlainText("Body", content, TypographyStyle.Body, 120);
            body.textWrappingMode = TextWrappingModes.Normal;
            var actions = Rect("Actions", content);
            Horizontal(actions, 0, 0);
            Height(actions, 64);
            var primary = InstantiateButton(UIButtonFamily.Primary, actions);
            var secondary = InstantiateButton(UIButtonFamily.Secondary, actions);
            var navigation = new Navigation { mode = Navigation.Mode.Explicit,
                selectOnUp = secondary, selectOnDown = secondary, selectOnLeft = secondary, selectOnRight = secondary };
            primary.navigation = navigation;
            navigation.selectOnUp = navigation.selectOnDown = navigation.selectOnLeft = navigation.selectOnRight = primary;
            secondary.navigation = navigation;
            PrefabUtility.RecordPrefabInstancePropertyModifications(primary);
            PrefabUtility.RecordPrefabInstancePropertyModifications(secondary);
            var transition = Transition(root, panelRect, false, true);
            var dialog = root.gameObject.AddComponent<UIConfirmDialog>();
            SetReference(dialog, "_config", _config);
            SetReference(dialog, "_transition", transition);
            SetReference(dialog, "_backdrop", backdrop);
            SetReference(dialog, "_title", title);
            SetReference(dialog, "_body", body);
            SetReference(dialog, "_primary", primary);
            SetReference(dialog, "_secondary", secondary);
            SetReference(primary.gameObject.AddComponent<UICancelRelay>(), "_dialog", dialog);
            SetReference(secondary.gameObject.AddComponent<UICancelRelay>(), "_dialog", dialog);
            return SaveComponent(root);
        }

        private static void PanelSurface(RectTransform root, UIPanelStyle style = UIPanelStyle.Default)
        {
            var border = root.gameObject.AddComponent<Image>();
            border.raycastTarget = false;
            
            var surface = Image("Surface", root, _config.GetColor(UISemanticColor.Surface));
            surface.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            Stretch(surface.rectTransform, 1);

            // Subtle inner border (1-pixel inset highlight inside surface)
            var innerBorder = Image("InnerBorder", surface.transform, Color.clear);
            innerBorder.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            Stretch(innerBorder.rectTransform, 1);

            // Subtle top accent bar (3-pixel gold line anchored to top)
            var accentBar = Image("TopAccentBar", root, Color.clear);
            accentBar.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            accentBar.rectTransform.anchorMin = new Vector2(0, 1);
            accentBar.rectTransform.anchorMax = new Vector2(1, 1);
            accentBar.rectTransform.pivot = new Vector2(0.5f, 1);
            accentBar.rectTransform.sizeDelta = new Vector2(0, 3);
            accentBar.rectTransform.anchoredPosition = new Vector2(0, 0);

            var panel = root.gameObject.AddComponent<UIPanel>();
            SetReference(panel, "_config", _config);
            SetReference(panel, "_surface", surface);
            SetReference(panel, "_border", border);
            SetReference(panel, "_innerBorder", innerBorder);
            SetReference(panel, "_accentBar", accentBar);
            SetEnum(panel, "_style", (int)style);
            panel.Apply();
        }

        private static UITransition Transition(RectTransform root, RectTransform visual, bool visible, bool receivesInput)
        {
            var group = root.gameObject.AddComponent<CanvasGroup>();
            var transition = root.gameObject.AddComponent<UITransition>();
            SetReference(transition, "_config", _config);
            SetReference(transition, "_group", group);
            SetReference(transition, "_visual", visual);
            SetBool(transition, "_initiallyVisible", visible);
            SetBool(transition, "_receivesInput", receivesInput);
            if (visible) transition.ShowImmediate(); else transition.HideImmediate();
            return transition;
        }

        private static TMP_Text PlainText(string name, Transform parent, TypographyStyle style, float height)
        {
            var rect = Rect(name, parent);
            rect.gameObject.SetActive(false);
            Height(rect, height);
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = string.Empty;
            text.raycastTarget = false;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.color = _config.GetColor(UISemanticColor.TextPrimary);
            _config.ApplyTypography(text, style);
            var typography = rect.gameObject.AddComponent<UITypography>();
            SetReference(typography, "_config", _config);
            SetEnum(typography, "_style", (int)style);
            rect.gameObject.SetActive(true);
            return text;
        }

        private static void Vertical(RectTransform root, int padding)
        {
            var layout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(padding, padding, padding, padding);
            layout.spacing = UISpacing.Md;
            layout.childControlHeight = layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
        }
        private static void Horizontal(RectTransform root, int horizontalPadding, int verticalPadding)
        {
            var layout = root.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(horizontalPadding, horizontalPadding, verticalPadding, verticalPadding);
            layout.spacing = UISpacing.Md;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlHeight = layout.childControlWidth = true;
            layout.childForceExpandHeight = layout.childForceExpandWidth = false;
        }
        private static void FixedSize(RectTransform rect, float width, float height)
        {
            var layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.minWidth = layout.preferredWidth = width;
            layout.minHeight = layout.preferredHeight = height;
        }
        private static UIButton InstantiateButton(UIButtonFamily family, Transform parent)
        {
            return ((GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(
                Root + "/Components/" + family + "Button.prefab"), parent)).GetComponent<UIButton>();
        }
        private static GameObject SaveComponent(RectTransform root)
        {
            root.gameObject.SetActive(true);
            var result = PrefabUtility.SaveAsPrefabAsset(root.gameObject, Root + "/Components/" + root.name + ".prefab");
            UnityEngine.Object.DestroyImmediate(root.gameObject);
            return result;
        }
        private static void SetEnum(UnityEngine.Object target, string field, int value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).enumValueIndex = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        private static void SetBool(UnityEngine.Object target, string field, bool value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).boolValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
