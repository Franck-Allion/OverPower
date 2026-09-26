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

        private static GameObject CreateVitalResourceOrb(string name, UISemanticColor tone)
        {
            var root = Rect(name, null);
            root.gameObject.SetActive(false);
            root.sizeDelta = new Vector2(130, 130);
            var sizing = root.gameObject.AddComponent<LayoutElement>();
            sizing.preferredWidth = sizing.minWidth = 130;
            sizing.preferredHeight = sizing.minHeight = 130;

            // 1. Liquid Layer
            var liquidObj = Rect("Liquid", root);
            liquidObj.sizeDelta = new Vector2(100, 100);
            liquidObj.anchoredPosition = new Vector2(0, 0);
            var liquidImg = liquidObj.gameObject.AddComponent<Image>();
            liquidImg.raycastTarget = false;

            var lineSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/HealthBar/bars/bar16/line.png");
            var lineMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/HealthBar/bars/bar16/health16.mat");
            if (lineSprite != null)
            {
                liquidImg.sprite = lineSprite;
            }
            if (lineMat != null)
            {
                liquidImg.material = lineMat;
            }

            // 2. Metallic Frame Layer (Front)
            var frameObj = Rect("Frame", root);
            frameObj.sizeDelta = new Vector2(126, 163);
            frameObj.anchoredPosition = new Vector2(0, 8);
            var frameImg = frameObj.gameObject.AddComponent<Image>();
            frameImg.raycastTarget = false;
            var frontSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/HealthBar/bars/bar16/front.png");
            if (frontSprite != null)
            {
                frameImg.sprite = frontSprite;
            }

            // 3. Exact Numeric Value Label
            var valueText = PlainText("Value", root, TypographyStyle.Stat, 32);
            valueText.alignment = TextAlignmentOptions.Center;
            valueText.rectTransform.anchoredPosition = new Vector2(0, -6);
            valueText.text = tone == UISemanticColor.Health ? "73 / 100" : "3 / 5";

            var shadow = valueText.gameObject.AddComponent<UnityEngine.UI.Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.85f);
            shadow.effectDistance = new Vector2(1f, -1f);

            var orb = root.gameObject.AddComponent<UIVitalResourceOrb>();
            orb.InitializeReferences(_config, liquidImg, frameImg, valueText, tone);
            SetReference(orb, "_config", _config);
            SetReference(orb, "_liquidImage", liquidImg);
            SetReference(orb, "_frameImage", frameImg);
            SetReference(orb, "_valueLabel", valueText);
            SetEnum(orb, "_tone", (int)tone);

            return SaveComponent(root);
        }

        private static GameObject CreateUnitStackBadge()
        {
            var root = Rect("UnitStackBadge", null);
            root.gameObject.SetActive(false);
            root.sizeDelta = new Vector2(120, 56);
            PanelSurface(root, UIPanelStyle.Elevated);
            var sizing = root.gameObject.AddComponent<LayoutElement>();
            sizing.preferredWidth = sizing.minWidth = 120;
            sizing.preferredHeight = sizing.minHeight = 56;

            var content = Rect("Content", root);
            Stretch(content, UISpacing.Sm);
            Vertical(content, 0);
            var vlg = content.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 2;

            var qtyText = PlainText("Quantity", content, TypographyStyle.Stat, 26);
            qtyText.alignment = TextAlignmentOptions.MidlineLeft;
            qtyText.text = "x8";

            var hpRow = Rect("HpRow", content);
            Height(hpRow, 16);
            Horizontal(hpRow, 0, 0);

            var hpBarBg = Image("HpBarBg", hpRow, _config.GetColor(UISemanticColor.Surface));
            FixedSize(hpBarBg.rectTransform, 40, 6);
            var hpBarFill = Image("HpBarFill", hpBarBg.transform, _config.GetColor(UISemanticColor.Health));
            hpBarFill.type = UnityEngine.UI.Image.Type.Filled;
            hpBarFill.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            hpBarFill.fillAmount = 0.3f;
            Stretch(hpBarFill.rectTransform, 0);

            var hpText = PlainText("MemberHp", hpRow, TypographyStyle.Caption, 16);
            hpText.alignment = TextAlignmentOptions.MidlineLeft;
            hpText.text = "3 / 10";

            var badge = root.gameObject.AddComponent<UIUnitStackBadge>();
            var surface = root.Find("Surface")?.GetComponent<UnityEngine.UI.Image>();
            var border = root.GetComponent<UnityEngine.UI.Image>();
            badge.InitializeReferences(_config, surface, border, qtyText, hpText, hpBarFill);
            SetReference(badge, "_config", _config);
            SetReference(badge, "_background", surface);
            SetReference(badge, "_border", border);
            SetReference(badge, "_quantityLabel", qtyText);
            SetReference(badge, "_memberHpLabel", hpText);
            SetReference(badge, "_memberHpFill", hpBarFill);

            return SaveComponent(root);
        }

        private static GameObject CreateCardFrame()
        {
            var root = Rect("CardFrame", null);
            root.gameObject.SetActive(false);
            root.sizeDelta = new Vector2(380, 590);
            PanelSurface(root, UIPanelStyle.Elevated);
            var group = root.gameObject.AddComponent<CanvasGroup>();
            var stateOutline = Image("StateOutline", root, Color.clear); Stretch(stateOutline.rectTransform, -7); stateOutline.raycastTarget = false;

            // Header: cost is a primary visual anchor rather than secondary metadata.
            var header = Rect("Header", root); SetAbsolute(header, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, 0), new Vector2(-28, 70));
            var headerRule = Image("HeaderRule", header, _config.GetColor(UISemanticColor.FloatingBorder));
            SetAbsolute(headerRule.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 2), new Vector2(-16, 2));
            var costBadge = Image("CostBadge", header, WithAlpha(_config.GetColor(UISemanticColor.Gold), 0.20f));
            SetAbsolute(costBadge.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(12, 0), new Vector2(58, 48));
            var cost = PlainText("Cost", costBadge.transform, TypographyStyle.Stat, 48); Stretch(cost.rectTransform, 0); cost.alignment = TextAlignmentOptions.Center;
            var familyIcon = Image("FamilyIcon", header, _config.GetColor(UISemanticColor.Interactive));
            SetAbsolute(familyIcon.rectTransform, new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(-12, 0), new Vector2(24, 24));
            var type = PlainText("Type", header, TypographyStyle.Caption, 32);
            SetAbsolute(type.rectTransform, new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(-44, 0), new Vector2(116, 32)); type.alignment = TextAlignmentOptions.MidlineRight;

            // Full-width artwork window. The fallback remains deliberate when production art is absent.
            var artFrame = Image("ArtworkFrame", root, _config.GetColor(UISemanticColor.FloatingBorder));
            SetAbsolute(artFrame.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, -82), new Vector2(-28, 230));
            var artworkBackground = Image("ArtworkBackdrop", artFrame.transform, WithAlpha(_config.GetColor(UISemanticColor.Background), .94f)); Stretch(artworkBackground.rectTransform, 3);
            var artwork = Image("Artwork", artworkBackground.transform, Color.white); Stretch(artwork.rectTransform, 5); artwork.preserveAspect = false; artwork.raycastTarget = false;
            var placeholder = Rect("ArtworkPlaceholder", artworkBackground.transform); Stretch(placeholder, 5);
            var placeholderSurface = Image("PlaceholderSurface", placeholder, WithAlpha(_config.GetColor(UISemanticColor.Secondary), .22f)); Stretch(placeholderSurface.rectTransform, 0);
            var placeholderGlow = Image("PlaceholderGlow", placeholder, WithAlpha(_config.GetColor(UISemanticColor.Mana), .24f)); SetAbsolute(placeholderGlow.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(0, 10), new Vector2(118, 118));
            var placeholderLabel = PlainText("PlaceholderLabel", placeholder, TypographyStyle.Caption, 28); Stretch(placeholderLabel.rectTransform, 0); placeholderLabel.alignment = TextAlignmentOptions.Center; placeholderLabel.text = "ARTWORK";
            var artCaption = PlainText("ArtCaption", artFrame.transform, TypographyStyle.Caption, 22); SetAbsolute(artCaption.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(.5f, 0), new Vector2(0, 9), new Vector2(-20, 22)); artCaption.alignment = TextAlignmentOptions.BottomRight; artCaption.text = "OVERPOWER";

            // Central rarity medallion creates a physical transition between art and rules.
            var medallion = Rect("RarityMedallion", root); SetAbsolute(medallion, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, -312), new Vector2(118, 54));
            var medallionPlate = Image("Plate", medallion, WithAlpha(_config.GetColor(UISemanticColor.Background), .98f)); Stretch(medallionPlate.rectTransform, 0);
            var rarityGem = Image("Gem", medallion, _config.GetColor(UISemanticColor.TextSecondary)); SetAbsolute(rarityGem.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(15, 0), new Vector2(26, 26));
            var rarity = PlainText("Rarity", medallion, TypographyStyle.Caption, 28); SetAbsolute(rarity.rectTransform, new Vector2(0, 0), new Vector2(1, 1), new Vector2(.5f, .5f), new Vector2(10, 0), new Vector2(-42, 28)); rarity.alignment = TextAlignmentOptions.MidlineRight;

            var title = PlainText("Title", root, TypographyStyle.Heading, 44); SetAbsolute(title.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, -345), new Vector2(-42, 44)); title.alignment = TextAlignmentOptions.Center;
            var bodyRule = Image("BodyRule", root, WithAlpha(_config.GetColor(UISemanticColor.FloatingBorder), .7f)); SetAbsolute(bodyRule.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, -392), new Vector2(-56, 1));
            var description = PlainText("Description", root, TypographyStyle.BodySmall, 112); SetAbsolute(description.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, -457), new Vector2(-52, 112)); description.alignment = TextAlignmentOptions.TopLeft; description.textWrappingMode = TextWrappingModes.Normal;
            var rich = description.gameObject.AddComponent<UIRichText>(); SetReference(rich, "_config", _config);

            var stats = Rect("Stats", root); SetAbsolute(stats, new Vector2(0, 0), new Vector2(1, 0), new Vector2(.5f, 0), new Vector2(0, 30), new Vector2(-34, 64));
            var statsSurface = Image("StatsSurface", stats, WithAlpha(_config.GetColor(UISemanticColor.Background), .60f)); Stretch(statsSurface.rectTransform, 0);
            var attack = CreateStatSlot("Attack", "ATK", stats, 0f, .333f);
            var armor = CreateStatSlot("Armor", "ARM", stats, .333f, .667f);
            var health = CreateStatSlot("Health", "HP", stats, .667f, 1f);
            var card = root.gameObject.AddComponent<UICardFrame>();
            SetReference(card, "_config", _config);
            SetReference(card, "_surface", root.Find("Surface").GetComponent<Image>());
            SetReference(card, "_accent", root.Find("TopAccentBar").GetComponent<Image>());
            SetReference(card, "_artwork", artwork);
            SetReference(card, "_artworkPlaceholder", placeholder.gameObject); SetReference(card, "_artworkFrame", artFrame);
            SetReference(card, "_familyIcon", familyIcon); SetReference(card, "_rarityMedallion", medallion.gameObject);
            SetReference(card, "_rarityGem", rarityGem); SetReference(card, "_rarityLabel", rarity);
            SetReference(card, "_typeLabel", type); SetReference(card, "_costLabel", cost);
            SetReference(card, "_titleLabel", title); SetReference(card, "_description", rich);
            SetReference(card, "_stats", stats.gameObject); SetReference(card, "_attackLabel", attack);
            SetReference(card, "_armorLabel", armor); SetReference(card, "_healthLabel", health); SetReference(card, "_group", group); SetReference(card, "_stateOutline", stateOutline);
            return SaveComponent(root);
        }

        private static void SetAbsolute(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchorMin; rect.anchorMax = anchorMax; rect.pivot = pivot;
            rect.anchoredPosition = position; rect.sizeDelta = size;
            var layout = rect.GetComponent<LayoutElement>(); if (layout != null) layout.ignoreLayout = true;
        }

        private static Color WithAlpha(Color color, float alpha) { color.a = alpha; return color; }

        private static TMP_Text CreateStatSlot(string name, string label, Transform parent, float minX, float maxX)
        {
            var value = PlainText(name, parent, TypographyStyle.Stat, 38);
            SetAbsolute(value.rectTransform, new Vector2(minX, 0), new Vector2(maxX, 1), new Vector2(.5f, .5f), new Vector2(0, -7), new Vector2(0, -20));
            value.alignment = TextAlignmentOptions.Bottom;
            var caption = PlainText(name + "Label", parent, TypographyStyle.Caption, 18);
            SetAbsolute(caption.rectTransform, new Vector2(minX, 1), new Vector2(maxX, 1), new Vector2(.5f, 1), new Vector2(0, -6), new Vector2(0, 16));
            caption.alignment = TextAlignmentOptions.Top; caption.text = label;
            return value;
        }

        private static GameObject CreatePhaseBanner()
        {
            var root = Rect("PhaseBanner", null); root.gameObject.SetActive(false); root.sizeDelta = new Vector2(480, 86);
            PanelSurface(root, UIPanelStyle.Elevated);
            var group = root.gameObject.AddComponent<CanvasGroup>();
            var label = PlainText("Label", root, TypographyStyle.Heading, 64); label.alignment = TextAlignmentOptions.Center;
            var banner = root.gameObject.AddComponent<UIPhaseBanner>();
            SetReference(banner, "_config", _config); SetReference(banner, "_group", group); SetReference(banner, "_visual", root);
            SetReference(banner, "_label", label); SetReference(banner, "_accent", root.Find("TopAccentBar").GetComponent<Image>());
            return SaveComponent(root);
        }

        private static GameObject CreateTargetStateVisual()
        {
            var root = Rect("TargetStateVisual", null); root.gameObject.SetActive(false); root.sizeDelta = new Vector2(150, 100);
            var outline = root.gameObject.AddComponent<Image>(); outline.color = Color.clear; outline.raycastTarget = false;
            var corners = Image("CornerCue", root, Color.white); Stretch(corners.rectTransform, 4);
            var blocked = PlainText("BlockedCue", root, TypographyStyle.Heading, 80); blocked.text = "×"; blocked.alignment = TextAlignmentOptions.Center;
            var glow = Image("SelectedGlow", root, Color.white); Stretch(glow.rectTransform, 10); glow.raycastTarget = false;
            var state = root.gameObject.AddComponent<UITargetStateVisual>();
            SetReference(state, "_config", _config); SetReference(state, "_outline", outline); SetReference(state, "_cornerCue", corners.gameObject);
            SetReference(state, "_blockedCue", blocked.gameObject); SetReference(state, "_selectedGlow", glow);
            state.SetState(TargetVisualState.None);
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
