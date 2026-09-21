using System;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.Localization;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem.Editor
{
    /// <summary>Explicit authoring command; never runs on import or changes production scenes.</summary>
    public static partial class DesignSystemPreviewBuilder
    {
        public const string Root = "Assets/OverPower/UI/DesignSystem";
        public const string ConfigPath = "Assets/OverPower/Data/UI/UIDesignSystemConfig.asset";
        public const string ScenePath = "Assets/OverPower/Scenes/Development/DesignSystemPreview.unity";
        private static UIDesignSystemConfig _config;
        private static StringTableCollection _strings;

        [MenuItem("OverPower/UI/Create Design System Preview")]
        public static void Build()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Exit Play Mode before authoring.");
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            foreach (string path in new[] { Root + "/Tokens", Root + "/Typography", Root + "/Motion",
                Root + "/Components", "Assets/OverPower/Data/UI", "Assets/OverPower/Scenes/Development" }) Folder(path);
            _config = AssetDatabase.LoadAssetAtPath<UIDesignSystemConfig>(ConfigPath);
            if (_config == null)
            {
                _config = ScriptableObject.CreateInstance<UIDesignSystemConfig>();
                SetReference(_config, "_font", AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                    "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset"));
                AssetDatabase.CreateAsset(_config, ConfigPath);
            }
            _strings = LocalizationEditorSettings.GetStringTableCollection("UI.DesignSystemPreview")
                ?? LocalizationEditorSettings.CreateStringTableCollection("UI.DesignSystemPreview", "Assets/OverPower/Localization/Tables");
            foreach (var locale in LocalizationEditorSettings.GetLocales())
                if (_strings.GetTable(locale.Identifier) == null) _strings.AddNewTable(locale.Identifier);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("PreviewCamera", typeof(Camera)).GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = _config.GetColor(UISemanticColor.Background);
            camera.orthographic = true;
            camera.transform.position = new Vector3(0, 0, -10);
            var canvas = Rect("DesignSystemPreview", null).gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 10;
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule)).GetComponent<EventSystem>();
            var page = Rect("Page", canvas.transform);
            Stretch(page, UISpacing.Page);
            var layout = page.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = UISpacing.Xl;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            Text(page, "eyebrow", "OVERPOWER  /  VISUAL FOUNDATION", "OVERPOWER  /  FONDATIONS VISUELLES", TypographyStyle.Caption, 26, UISemanticColor.Primary);
            Text(page, "title", "Design system", "Système visuel", TypographyStyle.Display, 82);
            Text(page, "intro", "Quiet surfaces. Clear intent. Deliberate feedback.", "Des surfaces sobres. Des intentions claires. Un retour précis.", TypographyStyle.Body, 38, UISemanticColor.TextSecondary);
            var columns = Rect("Columns", page);
            Height(columns, 610);
            var row = columns.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.spacing = UISpacing.Page;
            row.childControlWidth = true;
            row.childControlHeight = true;
            row.childForceExpandHeight = true;
            row.childForceExpandWidth = true;
            var actions = Column("Actions", columns);
            Text(actions, "actions", "01  /  ACTIONS", "01  /  ACTIONS", TypographyStyle.Caption, 32, UISemanticColor.Primary);
            Text(actions, "hierarchy", "One interaction language", "Un langage commun", TypographyStyle.Heading, 42);
            foreach (UIButtonFamily family in Enum.GetValues(typeof(UIButtonFamily)))
            {
                var prefab = CreateButton(family);
                string key = family == UIButtonFamily.Primary ? "continue" : family == UIButtonFamily.Secondary ? "back" : "information";
                string en = family == UIButtonFamily.Primary ? "Continue" : family == UIButtonFamily.Secondary ? "Back" : "Information";
                string fr = family == UIButtonFamily.Primary ? "Continuer" : family == UIButtonFamily.Secondary ? "Retour" : "Informations";
                for (int i = 0; i < 2; i++)
                {
                    var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, actions);
                    instance.name = family + (i == 0 ? "Default" : "Disabled");
                    var button = instance.GetComponent<UIButton>();
                    button.interactable = i == 0;
                    if (family == UIButtonFamily.Primary && i == 0) eventSystem.firstSelectedGameObject = instance;
                    AddString(key, en, fr);
                    button.AccessibleLabel.TableReference = _strings.SharedData.TableCollectionNameGuid;
                    button.AccessibleLabel.TableEntryReference = key;
                    var label = instance.GetComponentInChildren<TMP_Text>();
                    if (label != null) Localize(label, key, en);
                }
            }
            var typography = Column("Typography", columns);
            Text(typography, "type", "02  /  TYPOGRAPHY", "02  /  TYPOGRAPHIE", TypographyStyle.Caption, 32, UISemanticColor.Primary);
            foreach (TypographyStyle style in Enum.GetValues(typeof(TypographyStyle)))
                Text(typography, "type." + style, style == TypographyStyle.Stat ? "128 / 256" : style.ToString(),
                    style == TypographyStyle.Stat ? "128 / 256" : style.ToString(), style,
                    style == TypographyStyle.Display ? 82 : style == TypographyStyle.Title ? 58 : 40);
            var palette = Column("Palette", columns);
            Text(palette, "palette", "03  /  SEMANTIC PALETTE", "03  /  PALETTE SÉMANTIQUE", TypographyStyle.Caption, 32, UISemanticColor.Primary);
            foreach (UISemanticColor role in Enum.GetValues(typeof(UISemanticColor)))
            {
                var swatchRow = Rect(role.ToString(), palette);
                Height(swatchRow, 22);
                var horizontal = swatchRow.gameObject.AddComponent<HorizontalLayoutGroup>();
                horizontal.spacing = UISpacing.Md;
                horizontal.childControlWidth = true;
                horizontal.childControlHeight = true;
                horizontal.childForceExpandWidth = false;
                var swatch = Image("Swatch", swatchRow, _config.GetColor(role));
                var size = swatch.gameObject.AddComponent<LayoutElement>();
                size.preferredWidth = UISpacing.Xxl;
                Text(swatchRow, "color." + role, role.ToString(), role.ToString(), TypographyStyle.Caption, 22);
            }
            Text(page, "footer", "Arrows: focus     Enter: submit     •     Solid: primary   /   Outline: secondary   /   Bar: unavailable",
                "Flèches : focus     Entrée : valider     •     Plein : principal   /   Contour : secondaire   /   Barre : indisponible",
                TypographyStyle.Caption, 40, UISemanticColor.TextSecondary);
            BuildComponentsPreview(canvas, page, actions);
            foreach (var table in _strings.StringTables) EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(_strings.SharedData);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        private static GameObject CreateButton(UIButtonFamily family)
        {
            bool iconOnly = family == UIButtonFamily.Icon;
            var root = Rect(family + "Button", null);
            root.gameObject.SetActive(false);
            root.sizeDelta = new Vector2(iconOnly ? 64 : 400, 64);
            var sizing = root.gameObject.AddComponent<LayoutElement>();
            sizing.preferredHeight = 64;
            sizing.minHeight = 64;
            sizing.preferredWidth = iconOnly ? 64 : 400;
            var hit = root.gameObject.AddComponent<Image>();
            hit.color = Color.clear;
            var button = root.gameObject.AddComponent<UIButton>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = hit;
            var visual = Rect("Visual", root);
            Stretch(visual, 0);
            if (iconOnly) { visual.anchorMax = new Vector2(0, 1); visual.sizeDelta = new Vector2(64, 0); visual.anchoredPosition = new Vector2(32, 0); }
            var opacity = visual.gameObject.AddComponent<CanvasGroup>();
            var border = Image("Border", visual, _config.GetColor(UISemanticColor.Primary));
            Stretch(border.rectTransform, 0);
            var surface = Image("Surface", visual, _config.GetColor(UISemanticColor.Surface));
            Stretch(surface.rectTransform, 2);
            var focus = Image("Focus", visual, _config.GetColor(UISemanticColor.Selected));
            focus.rectTransform.anchorMin = new Vector2(0, 0);
            focus.rectTransform.anchorMax = new Vector2(0, 1);
            focus.rectTransform.sizeDelta = new Vector2(4, -16);
            focus.rectTransform.anchoredPosition = new Vector2(-10, 0);
            var disabled = Image("UnavailableBar", visual, _config.GetColor(UISemanticColor.TextSecondary));
            disabled.rectTransform.anchorMin = disabled.rectTransform.anchorMax = new Vector2(1, 0.5f);
            disabled.rectTransform.sizeDelta = new Vector2(12, 2);
            disabled.rectTransform.anchoredPosition = new Vector2(-16, 0);
            TMP_Text label = null;
            Image icon = null;
            if (iconOnly)
            {
                icon = Image("Icon", visual, Color.white);
                icon.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                icon.rectTransform.sizeDelta = new Vector2(20, 20);
                icon.preserveAspect = true;
            }
            else
            {
                label = Rect("Label", visual).gameObject.AddComponent<TextMeshProUGUI>();
                Stretch(label.rectTransform, UISpacing.Xl);
                label.rectTransform.offsetMin = new Vector2(UISpacing.Xl, 0);
                label.rectTransform.offsetMax = new Vector2(-UISpacing.Xl, 0);
                label.text = string.Empty;
                label.alignment = TextAlignmentOptions.Center;
                label.raycastTarget = false;
                _config.ApplyTypography(label, TypographyStyle.Button);
            }
            SetReference(button, "_config", _config);
            var so = new SerializedObject(button);
            so.FindProperty("_family").enumValueIndex = (int)family;
            so.ApplyModifiedPropertiesWithoutUndo();
            SetReference(button, "_visual", visual);
            SetReference(button, "_surface", surface);
            SetReference(button, "_border", border);
            SetReference(button, "_focusMark", focus.gameObject);
            SetReference(button, "_disabledMark", disabled.gameObject);
            SetReference(button, "_visualOpacity", opacity);
            SetReference(button, "_label", label);
            SetReference(button, "_icon", icon);
            root.gameObject.SetActive(true);
            var prefab = PrefabUtility.SaveAsPrefabAsset(root.gameObject, Root + "/Components/" + family + "Button.prefab");
            UnityEngine.Object.DestroyImmediate(root.gameObject);
            return prefab;
        }

        private static RectTransform Column(string name, Transform parent)
        {
            var column = Rect(name, parent);
            var sizing = column.gameObject.AddComponent<LayoutElement>();
            sizing.minWidth = 0;
            sizing.preferredWidth = 0;
            sizing.flexibleWidth = 1;
            var layout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = UISpacing.Sm;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return column;
        }

        private static void Text(Transform parent, string key, string en, string fr, TypographyStyle style, float height,
            UISemanticColor color = UISemanticColor.TextPrimary)
        {
            var rect = Rect(key, parent);
            Height(rect, height);
            var layout = rect.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.flexibleWidth = 1;
                layout.minWidth = 100;
            }
            rect.gameObject.SetActive(false);
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.raycastTarget = false;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            _config.ApplyTypography(text, style);
            text.color = _config.GetColor(color);
            var typography = rect.gameObject.AddComponent<UITypography>();
            SetReference(typography, "_config", _config);
            var so = new SerializedObject(typography);
            so.FindProperty("_style").enumValueIndex = (int)style;
            so.FindProperty("_color").enumValueIndex = (int)color;
            so.ApplyModifiedPropertiesWithoutUndo();
            AddString(key, en, fr);
            Localize(text, key, en);
            rect.gameObject.SetActive(true);
        }

        private static void AddString(string key, string en, string fr)
        {
            ((StringTable)_strings.GetTable(new LocaleIdentifier("en"))).AddEntry(key, en);
            ((StringTable)_strings.GetTable(new LocaleIdentifier("fr"))).AddEntry(key, fr);
        }

        private static void Localize(TMP_Text text, string key, string preview)
        {
            text.text = preview;
            var localized = text.gameObject.AddComponent<LocalizeStringEvent>();
            localized.StringReference = new LocalizedString(_strings.SharedData.TableCollectionNameGuid, key);
            UnityEventTools.AddPersistentListener(localized.OnUpdateString, text.SetText);
        }

        private static RectTransform Rect(string name, Transform parent)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            if (parent != null) rect.SetParent(parent, false);
            return rect;
        }
        private static Image Image(string name, Transform parent, Color color)
        {
            var image = Rect(name, parent).gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }
        private static void Stretch(RectTransform rect, float inset)
        { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = Vector2.one * inset; rect.offsetMax = Vector2.one * -inset; }
        private static void Height(RectTransform rect, float value)
        {
            var layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = value;
            layout.flexibleHeight = 0;
        }
        private static void SetReference(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        private static void Folder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int split = path.LastIndexOf('/');
            string parent = path.Substring(0, split);
            Folder(parent);
            AssetDatabase.CreateFolder(parent, path.Substring(split + 1));
        }
    }
}
