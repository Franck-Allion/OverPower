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
using OverPower.Unity.Presentation.DesignSystem;
using OverPower.Unity.Presentation.MainMenu;

namespace OverPower.Unity.Presentation.DesignSystem.Editor
{
    public static class MainMenuSceneBuilder
    {
        private const string ScenePath = "Assets/OverPower/Scenes/MainMenu.unity";
        private const string ConfigPath = "Assets/OverPower/Data/UI/UIDesignSystemConfig.asset";
        private const string Root = "Assets/OverPower/UI/DesignSystem";
        private const string TexturesRoot = "Assets/OverPower/UI/DesignSystem/Textures";

        [MenuItem("OverPower/UI/Rebuild Main Menu Scene")]
        public static void Rebuild()
        {
            if (EditorApplication.isPlaying)
            {
                throw new InvalidOperationException("Exit Play Mode before authoring.");
            }

            var scene = EditorSceneManager.OpenScene(ScenePath);
            var config = AssetDatabase.LoadAssetAtPath<UIDesignSystemConfig>(ConfigPath);
            if (config == null)
            {
                Debug.LogError($"Config not found at {ConfigPath}");
                return;
            }

            var strings = LocalizationEditorSettings.GetStringTableCollection("UI.Core");
            if (strings == null)
            {
                Debug.LogError("String table collection UI.Core not found!");
                return;
            }

            // Load procedural/design assets
            var glowSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{TexturesRoot}/ui_soft_glow_box.png");
            var gradSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{TexturesRoot}/ui_vertical_gradient.png");
            var diamondSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{TexturesRoot}/ui_diamond_accent.png");

            // Find Canvas and clean it
            var canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                var canvasComp = canvasObj.GetComponent<Canvas>();
                canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
                
                var scaler = canvasObj.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }
            else
            {
                var canvasComp = canvasObj.GetComponent<Canvas>();
                canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            // Purge existing children
            var canvasRect = canvasObj.GetComponent<RectTransform>();
            for (int i = canvasRect.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(canvasRect.GetChild(i).gameObject);
            }

            // Re-bind world camera if Main Camera exists
            var cameraObj = GameObject.FindWithTag("MainCamera");
            if (cameraObj != null)
            {
                var camera = cameraObj.GetComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = config.GetColor(UISemanticColor.Background); // #0D141F
            }

            // 1. Fullscreen Background with custom artwork support
            var bgObj = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(canvasRect, false);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;
            var bgImage = bgObj.GetComponent<Image>();
            bgImage.raycastTarget = false;

            var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/OverPower/UI/Screens/MainMenu/menu_background.png");
            if (bgSprite != null)
            {
                bgImage.sprite = bgSprite;
                bgImage.color = Color.white;
                EditorUtility.SetDirty(bgImage);

                // Subtle soft vignette/tint overlay to ground the artwork and maintain contrast
                var overlayObj = new GameObject("DarkOverlay", typeof(RectTransform), typeof(Image));
                overlayObj.transform.SetParent(bgObj.transform, false);
                var overlayRect = overlayObj.GetComponent<RectTransform>();
                overlayRect.anchorMin = Vector2.zero;
                overlayRect.anchorMax = Vector2.one;
                overlayRect.offsetMin = overlayRect.offsetMax = Vector2.zero;
                var overlayImage = overlayObj.GetComponent<Image>();
                overlayImage.color = new Color(0.04f, 0.07f, 0.12f, 0.28f); // 28% atmospheric deep navy overlay
                overlayImage.raycastTarget = false;
                EditorUtility.SetDirty(overlayImage);
            }
            else
            {
                bgImage.color = config.GetColor(UISemanticColor.Background);
            }

            const float panelW = 640f;
            const float panelH = 780f;

            // 2. Soft Drop Shadow (Layer behind the panel)
            if (glowSprite != null)
            {
                var shadowObj = new GameObject("DropShadow", typeof(RectTransform), typeof(Image));
                shadowObj.transform.SetParent(canvasRect, false);
                var shadowRect = shadowObj.GetComponent<RectTransform>();
                shadowRect.anchorMin = shadowRect.anchorMax = new Vector2(0.5f, 0.5f);
                shadowRect.pivot = new Vector2(0.5f, 0.5f);
                shadowRect.sizeDelta = new Vector2(panelW + 36f, panelH + 36f);
                shadowRect.anchoredPosition = new Vector2(0f, -12f);
                var shadowImg = shadowObj.GetComponent<Image>();
                shadowImg.sprite = glowSprite;
                shadowImg.type = Image.Type.Sliced;
                shadowImg.color = new Color(0f, 0f, 0f, 0.60f); // Clean, grounded drop shadow
                shadowImg.raycastTarget = false;
            }

            // 3. Golden Aura Container with ambient breathing motion (UIAuraPulse)
            if (glowSprite != null)
            {
                var auraContainerObj = new GameObject("AuraContainer", typeof(RectTransform), typeof(CanvasGroup), typeof(UIAuraPulse));
                auraContainerObj.transform.SetParent(canvasRect, false);
                var auraContainerRect = auraContainerObj.GetComponent<RectTransform>();
                auraContainerRect.anchorMin = auraContainerRect.anchorMax = new Vector2(0.5f, 0.5f);
                auraContainerRect.pivot = new Vector2(0.5f, 0.5f);
                auraContainerRect.sizeDelta = new Vector2(panelW, panelH);
                auraContainerRect.anchoredPosition = Vector2.zero;

                var canvasGroup = auraContainerObj.GetComponent<CanvasGroup>();
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                canvasGroup.alpha = 0.35f;

                var auraPulse = auraContainerObj.GetComponent<UIAuraPulse>();
                var pulseSo = new SerializedObject(auraPulse);
                pulseSo.FindProperty("_targetTransform").objectReferenceValue = auraContainerRect;
                pulseSo.FindProperty("_canvasGroup").objectReferenceValue = canvasGroup;
                pulseSo.FindProperty("_baseAlpha").floatValue = 0.35f;
                pulseSo.FindProperty("_peakAlpha").floatValue = 0.90f;
                pulseSo.FindProperty("_baseScale").floatValue = 1.00f;
                pulseSo.FindProperty("_peakScale").floatValue = 1.012f;
                pulseSo.FindProperty("_halfCycleDuration").floatValue = 2.6f;
                pulseSo.FindProperty("_ease").enumValueIndex = (int)DG.Tweening.Ease.InOutSine;
                pulseSo.FindProperty("_autoPlay").boolValue = true;
                pulseSo.ApplyModifiedProperties();

                // 3a. Diffuse Outer Golden Aura (Discreet halo tightly contoured to the panel)
                var outerAuraObj = new GameObject("GoldenAuraOuter", typeof(RectTransform), typeof(Image));
                outerAuraObj.transform.SetParent(auraContainerRect, false);
                var outerAuraRect = outerAuraObj.GetComponent<RectTransform>();
                outerAuraRect.anchorMin = outerAuraRect.anchorMax = new Vector2(0.5f, 0.5f);
                outerAuraRect.pivot = new Vector2(0.5f, 0.5f);
                outerAuraRect.sizeDelta = new Vector2(panelW + 28f, panelH + 28f);
                outerAuraRect.anchoredPosition = new Vector2(0f, 0f);
                var outerAuraImg = outerAuraObj.GetComponent<Image>();
                outerAuraImg.sprite = glowSprite;
                outerAuraImg.type = Image.Type.Sliced;
                outerAuraImg.color = new Color(0.89f, 0.74f, 0.47f, 0.28f); // Subdued noble gold
                outerAuraImg.raycastTarget = false;

                // 3b. Subtle Inner Golden Glow (Slightly warmer, focused around upper edges)
                var innerAuraObj = new GameObject("GoldenAuraInner", typeof(RectTransform), typeof(Image));
                innerAuraObj.transform.SetParent(auraContainerRect, false);
                var innerAuraRect = innerAuraObj.GetComponent<RectTransform>();
                innerAuraRect.anchorMin = innerAuraRect.anchorMax = new Vector2(0.5f, 0.5f);
                innerAuraRect.pivot = new Vector2(0.5f, 0.5f);
                innerAuraRect.sizeDelta = new Vector2(panelW + 10f, panelH + 10f);
                innerAuraRect.anchoredPosition = new Vector2(0f, 2f);
                var innerAuraImg = innerAuraObj.GetComponent<Image>();
                innerAuraImg.sprite = glowSprite;
                innerAuraImg.type = Image.Type.Sliced;
                innerAuraImg.color = new Color(1f, 0.88f, 0.64f, 0.35f); // Warm luminance near border
                innerAuraImg.raycastTarget = false;
            }

            // 5. Central Elevated Main Menu Panel
            var mainPanelObj = new GameObject("MainPanel", typeof(RectTransform));
            mainPanelObj.transform.SetParent(canvasRect, false);
            var mainPanelRect = mainPanelObj.GetComponent<RectTransform>();
            mainPanelRect.anchorMin = mainPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
            mainPanelRect.pivot = new Vector2(0.5f, 0.5f);
            mainPanelRect.sizeDelta = new Vector2(panelW, panelH);

            // Wire UIPanel (Elevated style has gold outer border, top Accent Bar, and surface)
            var border = mainPanelObj.AddComponent<Image>();
            border.raycastTarget = false;

            var surfaceObj = new GameObject("Surface", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            surfaceObj.transform.SetParent(mainPanelObj.transform, false);
            var surface = surfaceObj.GetComponent<Image>();
            surface.raycastTarget = false;
            if (gradSprite != null)
            {
                surface.sprite = gradSprite;
                surface.type = Image.Type.Simple;
            }
            surfaceObj.GetComponent<LayoutElement>().ignoreLayout = true;
            var surfaceRect = surfaceObj.GetComponent<RectTransform>();
            surfaceRect.anchorMin = Vector2.zero;
            surfaceRect.anchorMax = Vector2.one;
            surfaceRect.offsetMin = new Vector2(2, 2);
            surfaceRect.offsetMax = new Vector2(-2, -2);

            var innerBorderObj = new GameObject("InnerBorder", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            innerBorderObj.transform.SetParent(surfaceObj.transform, false);
            var innerBorder = innerBorderObj.GetComponent<Image>();
            innerBorder.raycastTarget = false;
            innerBorderObj.GetComponent<LayoutElement>().ignoreLayout = true;
            var innerRect = innerBorderObj.GetComponent<RectTransform>();
            innerRect.anchorMin = Vector2.zero;
            innerRect.anchorMax = Vector2.one;
            innerRect.offsetMin = new Vector2(1, 1);
            innerRect.offsetMax = new Vector2(-1, -1);

            var accentBarObj = new GameObject("TopAccentBar", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            accentBarObj.transform.SetParent(mainPanelObj.transform, false);
            var accentBar = accentBarObj.GetComponent<Image>();
            accentBar.raycastTarget = false;
            accentBarObj.GetComponent<LayoutElement>().ignoreLayout = true;
            var accentRect = accentBarObj.GetComponent<RectTransform>();
            accentRect.anchorMin = new Vector2(0, 1);
            accentRect.anchorMax = new Vector2(1, 1);
            accentRect.pivot = new Vector2(0.5f, 1);
            accentRect.sizeDelta = new Vector2(0, 3);
            accentRect.anchoredPosition = Vector2.zero;

            var uiPanel = mainPanelObj.AddComponent<UIPanel>();
            SetReference(uiPanel, "_config", config);
            SetReference(uiPanel, "_surface", surface);
            SetReference(uiPanel, "_border", border);
            SetReference(uiPanel, "_innerBorder", innerBorder);
            SetReference(uiPanel, "_accentBar", accentBar);
            SetEnum(uiPanel, "_style", (int)UIPanelStyle.Elevated);
            uiPanel.Apply();

            // Refine accent bar to bright interactive gold for crisp top highlight
            accentBar.color = config.GetColor(UISemanticColor.Interactive);

            // Subtle Corner Accents (Mythological / noble corner ornaments)
            if (diamondSprite != null)
            {
                CreateCornerAccent("CornerTL", mainPanelRect, diamondSprite, new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(2, -2), config.GetColor(UISemanticColor.Primary));
                CreateCornerAccent("CornerTR", mainPanelRect, diamondSprite, new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(-2, -2), config.GetColor(UISemanticColor.Primary));
                CreateCornerAccent("CornerBL", mainPanelRect, diamondSprite, new Vector2(0, 0), new Vector2(0.5f, 0.5f), new Vector2(2, 2), config.GetColor(UISemanticColor.Primary));
                CreateCornerAccent("CornerBR", mainPanelRect, diamondSprite, new Vector2(1, 0), new Vector2(0.5f, 0.5f), new Vector2(-2, 2), config.GetColor(UISemanticColor.Primary));
            }

            // Set up main vertical layout inside panel
            var panelLayout = mainPanelObj.AddComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(60, 60, 80, 80);
            panelLayout.spacing = 32;
            panelLayout.childControlHeight = true;
            panelLayout.childControlWidth = true;
            panelLayout.childForceExpandHeight = false;
            panelLayout.childForceExpandWidth = true;

            // 6. Title Block
            var titleBlockObj = new GameObject("TitleBlock", typeof(RectTransform));
            titleBlockObj.transform.SetParent(mainPanelRect, false);
            var titleLayout = titleBlockObj.AddComponent<VerticalLayoutGroup>();
            titleLayout.spacing = 14;
            titleLayout.childControlHeight = true;
            titleLayout.childControlWidth = true;
            titleLayout.childForceExpandHeight = false;
            titleLayout.childForceExpandWidth = true;

            // 6a. Title Text (OVERPOWER)
            var titleTextObj = new GameObject("TitleText", typeof(RectTransform), typeof(CanvasRenderer));
            titleTextObj.transform.SetParent(titleBlockObj.transform, false);
            var titleText = titleTextObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "OVERPOWER";
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.raycastTarget = false;
            config.ApplyTypography(titleText, TypographyStyle.Display);
            titleText.color = config.GetColor(UISemanticColor.Primary); // Primary gold

            var titleLocalize = titleTextObj.AddComponent<LocalizeStringEvent>();
            titleLocalize.StringReference = new LocalizedString(strings.SharedData.TableCollectionNameGuid, "ui.main_menu.title");
            UnityEventTools.AddPersistentListener(titleLocalize.OnUpdateString, titleText.SetText);

            // 6b. Refined Mythological Divider: < ── ◆ ── >
            var dividerObj = new GameObject("Divider", typeof(RectTransform));
            dividerObj.transform.SetParent(titleBlockObj.transform, false);
            var divLayout = dividerObj.AddComponent<HorizontalLayoutGroup>();
            divLayout.childAlignment = TextAnchor.MiddleCenter;
            divLayout.spacing = 10;
            divLayout.childControlWidth = true;
            divLayout.childControlHeight = true;
            divLayout.childForceExpandWidth = false;
            divLayout.childForceExpandHeight = false;
            var divElement = dividerObj.AddComponent<LayoutElement>();
            divElement.preferredHeight = 14;

            // Left line
            var leftLineObj = new GameObject("LeftLine", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            leftLineObj.transform.SetParent(dividerObj.transform, false);
            leftLineObj.GetComponent<Image>().color = config.GetColor(UISemanticColor.Primary);
            leftLineObj.GetComponent<Image>().raycastTarget = false;
            var leftElem = leftLineObj.GetComponent<LayoutElement>();
            leftElem.preferredHeight = 2;
            leftElem.preferredWidth = 140;
            leftElem.flexibleWidth = 1;

            // Center Diamond
            if (diamondSprite != null)
            {
                var diaObj = new GameObject("CenterDiamond", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
                diaObj.transform.SetParent(dividerObj.transform, false);
                var diaImg = diaObj.GetComponent<Image>();
                diaImg.sprite = diamondSprite;
                diaImg.color = config.GetColor(UISemanticColor.Primary);
                diaImg.raycastTarget = false;
                var diaElem = diaObj.GetComponent<LayoutElement>();
                diaElem.preferredWidth = 12;
                diaElem.preferredHeight = 12;
            }

            // Right line
            var rightLineObj = new GameObject("RightLine", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            rightLineObj.transform.SetParent(dividerObj.transform, false);
            rightLineObj.GetComponent<Image>().color = config.GetColor(UISemanticColor.Primary);
            rightLineObj.GetComponent<Image>().raycastTarget = false;
            var rightElem = rightLineObj.GetComponent<LayoutElement>();
            rightElem.preferredHeight = 2;
            rightElem.preferredWidth = 140;
            rightElem.flexibleWidth = 1;

            // 7. Action Block (Buttons)
            var actionBlockObj = new GameObject("ActionBlock", typeof(RectTransform));
            actionBlockObj.transform.SetParent(mainPanelRect, false);
            var actionLayout = actionBlockObj.AddComponent<VerticalLayoutGroup>();
            actionLayout.spacing = 22;
            actionLayout.childControlHeight = true;
            actionLayout.childControlWidth = true;
            actionLayout.childForceExpandHeight = false;
            actionLayout.childForceExpandWidth = true;

            // 7a. Play Button (PrimaryButton prefab)
            var playButtonObj = (GameObject)PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>($"{Root}/Components/PrimaryButton.prefab"), 
                actionBlockObj.transform);
            playButtonObj.name = "PlayButton";
            var playBtn = playButtonObj.GetComponent<UIButton>();
            var playText = playButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            var playLocalize = playText.gameObject.AddComponent<LocalizeStringEvent>();
            playLocalize.StringReference = new LocalizedString(strings.SharedData.TableCollectionNameGuid, "ui.main_menu.play");
            UnityEventTools.AddPersistentListener(playLocalize.OnUpdateString, playText.SetText);

            // 7b. Language Button (SecondaryButton prefab)
            var langButtonObj = (GameObject)PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>($"{Root}/Components/SecondaryButton.prefab"), 
                actionBlockObj.transform);
            langButtonObj.name = "LanguageButton";
            var langBtn = langButtonObj.GetComponent<UIButton>();
            var langText = langButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            var langLocalize = langText.gameObject.AddComponent<LocalizeStringEvent>();
            langLocalize.StringReference = new LocalizedString(strings.SharedData.TableCollectionNameGuid, "ui.main_menu.language");
            UnityEventTools.AddPersistentListener(langLocalize.OnUpdateString, langText.SetText);

            // 7c. Exit Button (SecondaryButton prefab)
            var exitButtonObj = (GameObject)PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>($"{Root}/Components/SecondaryButton.prefab"), 
                actionBlockObj.transform);
            exitButtonObj.name = "ExitButton";
            var exitBtn = exitButtonObj.GetComponent<UIButton>();
            var exitText = exitButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            var exitLocalize = exitText.gameObject.AddComponent<LocalizeStringEvent>();
            exitLocalize.StringReference = new LocalizedString(strings.SharedData.TableCollectionNameGuid, "ui.main_menu.exit");
            UnityEventTools.AddPersistentListener(exitLocalize.OnUpdateString, exitText.SetText);

            // 8. Presenter Configuration
            var presenterObj = GameObject.Find("MainMenuPresenter");
            if (presenterObj == null)
            {
                presenterObj = new GameObject("MainMenuPresenter");
            }
            var presenter = presenterObj.GetComponent<MainMenuPresenter>();
            if (presenter == null)
            {
                presenter = presenterObj.AddComponent<MainMenuPresenter>();
            }

            var so = new SerializedObject(presenter);
            so.FindProperty("playButton").objectReferenceValue = playBtn;
            so.FindProperty("languageButton").objectReferenceValue = langBtn;
            so.FindProperty("exitButton").objectReferenceValue = exitBtn;
            so.ApplyModifiedProperties();

            // 9. Setup EventSystem first selected object
            var eventSystemObj = GameObject.Find("EventSystem");
            if (eventSystemObj != null)
            {
                var eventSystem = eventSystemObj.GetComponent<EventSystem>();
                if (eventSystem != null)
                {
                    eventSystem.firstSelectedGameObject = playButtonObj;
                }
            }

            EditorUtility.SetDirty(presenterObj);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            
            Debug.Log("Main Menu scene rebuilt successfully with premium Design System!");
        }

        private static void CreateCornerAccent(string name, RectTransform parent, Sprite sprite, Vector2 anchor, Vector2 pivot, Vector2 pos, Color color)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(10, 10);
            var img = obj.GetComponent<Image>();
            img.sprite = sprite;
            img.color = color;
            img.raycastTarget = false;
            obj.GetComponent<LayoutElement>().ignoreLayout = true;
        }

        private static void SetReference(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetEnum(UnityEngine.Object target, string field, int value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).enumValueIndex = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
