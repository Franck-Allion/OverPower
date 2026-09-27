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
            const string cardsRoot = "Assets/OverPower/UI/Cards";
            var titleTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(cardsRoot + "/Textures/texture-title.png");
            var bodySprite = AssetDatabase.LoadAssetAtPath<Sprite>(cardsRoot + "/Textures/texture-capacity.png");
            var middleBanner = AssetDatabase.LoadAssetAtPath<Sprite>(cardsRoot + "/Decorations/middle-banner.png");
            var commonRarity = AssetDatabase.LoadAssetAtPath<Sprite>(cardsRoot + "/Decorations/common.png");
            var unitBannerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(cardsRoot + "/Decorations/unit-banner.png");
            var spellBannerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(cardsRoot + "/Decorations/spell-banner.png");
            var manaIcon = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Icons/Inline/icon_mana.png");
            var attackIcon = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Icons/Inline/icon_attack.png");
            var healthIcon = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Icons/Inline/icon_health.png");
            var armorIcon = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Textures/ui_diamond_accent.png");
            var placeholderGradient = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Textures/ui_vertical_gradient.png");
            var placeholderGlow = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Textures/ui_soft_glow_box.png");
            var roundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

            var root = Rect("CardFrame", null);
            root.gameObject.SetActive(false);
            root.sizeDelta = new Vector2(430, 620);
            var rootLayout = root.gameObject.AddComponent<LayoutElement>();
            rootLayout.minWidth = rootLayout.preferredWidth = 430;
            rootLayout.minHeight = rootLayout.preferredHeight = 620;
            rootLayout.flexibleWidth = rootLayout.flexibleHeight = 0;
            PanelSurface(root, UIPanelStyle.Elevated);

            // Outer card physical presence: floating shadow and tangible depth
            var cardShadow = root.gameObject.AddComponent<Shadow>();
            cardShadow.effectColor = new Color(0f, 0f, 0f, 0.48f);
            cardShadow.effectDistance = new Vector2(0, -6);

            var group = root.gameObject.AddComponent<CanvasGroup>();
            var stateOutline = Image("StateOutline", root, Color.clear); Stretch(stateOutline.rectTransform, -7); stateOutline.raycastTarget = false;

            // Textured title/header zone: reduced vertical footprint (~8.3%, from 96 to 88)
            var header = Rect("Header", root);
            SetAbsolute(header, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), Vector2.zero, new Vector2(-10, 88));
            var headerTexture = header.gameObject.AddComponent<RawImage>();
            headerTexture.texture = titleTexture; headerTexture.uvRect = new Rect(0, 0, 1, 0.333333f); headerTexture.raycastTarget = false;
            var headerShade = Image("HeaderShade", header, new Color(0.03f, 0.06f, 0.10f, .20f)); Stretch(headerShade.rectTransform, 0);

            // Refined, centered card title with elegant proportions and reliable single-line overflow
            var title = PlainText("Title", header, TypographyStyle.Title, 38);
            title.rectTransform.anchorMin = new Vector2(0, 0);
            title.rectTransform.anchorMax = new Vector2(1, 1);
            title.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            title.rectTransform.offsetMin = new Vector2(96, 0);
            title.rectTransform.offsetMax = new Vector2(-20, 0);
            title.alignment = TextAlignmentOptions.Center; title.characterSpacing = 3f;
            title.enableWordWrapping = false;
            title.overflowMode = TextOverflowModes.Overflow;
            title.enableAutoSizing = true; title.fontSizeMin = 16; title.fontSizeMax = 23;

            // Resource cost: repeated mana icons integrated into header texture (no numeric text widget)
            var costBadge = Rect("CostBadge", header);
            SetAbsolute(costBadge, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(16, 0), new Vector2(80, 34));
            var costLayout = costBadge.gameObject.AddComponent<HorizontalLayoutGroup>();
            costLayout.spacing = 5; costLayout.childAlignment = TextAnchor.MiddleLeft;
            costLayout.childControlWidth = false; costLayout.childControlHeight = false;
            costLayout.childForceExpandWidth = false; costLayout.childForceExpandHeight = false;
            var costFitter = costBadge.gameObject.AddComponent<ContentSizeFitter>();
            costFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            costFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            for (int i = 0; i < 3; i++)
            {
                var icon = Image($"CostIcon_{i}", costBadge, new Color(0.38f, 0.85f, 1.0f, 0.98f));
                icon.sprite = manaIcon; icon.preserveAspect = true; icon.rectTransform.sizeDelta = new Vector2(22, 30);
                var elem = icon.gameObject.AddComponent<LayoutElement>(); elem.preferredWidth = 22; elem.preferredHeight = 30;
                var iconShadow = icon.gameObject.AddComponent<Shadow>(); iconShadow.effectColor = new Color(0, 0, 0, 0.40f); iconShadow.effectDistance = new Vector2(0, -1);
            }
            var cost = PlainText("Cost", costBadge, TypographyStyle.Stat, 28);
            cost.gameObject.SetActive(false); // Kept for reference but hidden in favor of repeated icons
            var familyIcon = costBadge.transform.Find("CostIcon_0")?.GetComponent<Image>();

            // Recessed framed artwork dominating the middle (expanded height: 289px)
            var artFrame = Image("ArtworkFrame", root, WithAlpha(_config.GetColor(UISemanticColor.FloatingBorder), .96f));
            SetAbsolute(artFrame.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, -93), new Vector2(-28, 289));
            var artFrameShadow = artFrame.gameObject.AddComponent<Shadow>(); artFrameShadow.effectColor = new Color(0, 0, 0, 0.40f); artFrameShadow.effectDistance = new Vector2(0, -2);

            var artworkBackground = Image("ArtworkBackdrop", artFrame.transform, _config.GetColor(UISemanticColor.Background)); Stretch(artworkBackground.rectTransform, 4);
            artworkBackground.gameObject.AddComponent<RectMask2D>();
            var artwork = Image("Artwork", artworkBackground.transform, Color.white); Stretch(artwork.rectTransform, 0); artwork.preserveAspect = false; artwork.raycastTarget = false;
            artwork.rectTransform.anchoredPosition = new Vector2(0, -6); // Refined vertical offset preserving face & silhouette
            var artworkAspect = artwork.gameObject.AddComponent<AspectRatioFitter>(); artworkAspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; artworkAspect.aspectRatio = 1.495f;

            // Subtle inner depth overlay recessed behind the frame opening
            var artDepth = Image("ArtDepth", artworkBackground.transform, new Color(0.02f, 0.02f, 0.03f, 0.08f));
            Stretch(artDepth.rectTransform, 0); artDepth.raycastTarget = false;
            var artDepthOutline = artDepth.gameObject.AddComponent<Outline>(); artDepthOutline.effectColor = new Color(0f, 0f, 0f, 0.60f); artDepthOutline.effectDistance = new Vector2(1, -1);
            var artTopEdge = Image("TopInnerEdge", artDepth.transform, new Color(0f, 0f, 0f, .42f)); SetAbsolute(artTopEdge.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), Vector2.zero, new Vector2(-4, 3));
            var artLeftEdge = Image("LeftInnerEdge", artDepth.transform, new Color(0f, 0f, 0f, .30f)); SetAbsolute(artLeftEdge.rectTransform, new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, .5f), Vector2.zero, new Vector2(3, -4));
            var artRightEdge = Image("RightInnerEdge", artDepth.transform, new Color(0f, 0f, 0f, .30f)); SetAbsolute(artRightEdge.rectTransform, new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, .5f), Vector2.zero, new Vector2(3, -4));

            // Polished spell placeholder: ethereal magical atmosphere instead of generic gray box
            var placeholder = Rect("ArtworkPlaceholder", artworkBackground.transform); Stretch(placeholder, 0);
            var placeholderSurface = Image("PlaceholderSurface", placeholder, new Color(.015f, .035f, .070f, 1f)); Stretch(placeholderSurface.rectTransform, 0);
            var placeholderTexture = Image("MysticGradient", placeholder, new Color(0.04f, 0.11f, 0.24f, 0.75f)); placeholderTexture.sprite = placeholderGradient; Stretch(placeholderTexture.rectTransform, 0);
            var glow = Image("MysticGlow", placeholder, new Color(0.12f, 0.48f, 0.88f, 0.25f)); glow.sprite = placeholderGlow; SetAbsolute(glow.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(300, 220));
            var sigilAccent = Image("ArcaneAccent", placeholder, new Color(0.28f, 0.68f, 0.95f, 0.18f)); sigilAccent.sprite = armorIcon; sigilAccent.preserveAspect = true; SetAbsolute(sigilAccent.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(160, 160));
            var sigil = Image("ArcaneSigil", placeholder, new Color(0.32f, 0.72f, 1.0f, 0.32f)); sigil.sprite = manaIcon; sigil.preserveAspect = true; SetAbsolute(sigil.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(0, -2), new Vector2(115, 145));
            var upperSpark = Image("UpperSpark", placeholder, new Color(.52f, .82f, 1f, .35f)); upperSpark.sprite = armorIcon; upperSpark.preserveAspect = true; SetAbsolute(upperSpark.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(24, 24));

            // Unit/Spell banners with distinctive non-text visual symbols
            var unitBanner = Image("UnitBanner", root, Color.white); unitBanner.sprite = unitBannerSprite; unitBanner.preserveAspect = true;
            SetAbsolute(unitBanner.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, -88), new Vector2(368, 42));
            var unitGlow = Image("UnitCrestGlow", unitBanner.transform, new Color(0.95f, 0.72f, 0.32f, 0.35f)); unitGlow.sprite = placeholderGlow; SetAbsolute(unitGlow.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(50, 34));

            var spellBanner = Image("SpellBanner", root, Color.white); spellBanner.sprite = spellBannerSprite; spellBanner.preserveAspect = true;
            SetAbsolute(spellBanner.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, -88), new Vector2(368, 42));
            var spellGlow = Image("SpellCrestGlow", spellBanner.transform, new Color(0.25f, 0.75f, 1.0f, 0.40f)); spellGlow.sprite = placeholderGlow; SetAbsolute(spellGlow.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(50, 34));

            // Textured rules body with safe margins, high contrast typography, and non-clipping overflow
            var body = Image("BodyTexture", root, Color.white); body.sprite = bodySprite; body.type = UnityEngine.UI.Image.Type.Simple;
            SetAbsolute(body.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(.5f, 0), new Vector2(0, 10), new Vector2(-18, 228));
            var bodyShade = Image("BodyShade", body.transform, new Color(.04f, .025f, .015f, .10f)); Stretch(bodyShade.rectTransform, 0);
            var description = PlainText("Description", body.transform, TypographyStyle.BodySmall, 114);
            SetAbsolute(description.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, -32), new Vector2(-46, 114));
            description.margin = new Vector4(6, 2, 6, 2);
            description.overflowMode = TextOverflowModes.Overflow;
            description.enableWordWrapping = true;
            description.alignment = TextAlignmentOptions.TopLeft; description.textWrappingMode = TextWrappingModes.Normal;
            description.color = new Color32(18, 10, 6, 255); description.lineSpacing = 8;
            description.enableAutoSizing = true; description.fontSizeMin = 15; description.fontSizeMax = 18;
            var rich = description.gameObject.AddComponent<UIRichText>(); SetReference(rich, "_config", _config);

            // Integrated fantasy stat footer
            var stats = Rect("Stats", body.transform); SetAbsolute(stats, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(0, 6), new Vector2(382, 84));
            var footerSurface = Image("FooterSurface", stats, new Color(.04f, .025f, .015f, .28f)); Stretch(footerSurface.rectTransform, 0);
            var plinthShadow = footerSurface.gameObject.AddComponent<Shadow>(); plinthShadow.effectColor = new Color(0, 0, 0, .35f); plinthShadow.effectDistance = new Vector2(0, -2);
            var plinthOutline = footerSurface.gameObject.AddComponent<Outline>(); plinthOutline.effectColor = new Color(.58f, .44f, .26f, .28f); plinthOutline.effectDistance = new Vector2(1, -1);
            var footerDivider = Image("FooterDivider", stats, new Color(0.72f, 0.55f, 0.32f, 0.45f));
            SetAbsolute(footerDivider.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, 0), new Vector2(-16, 1));
            var centerSpine = Image("CenterCrest", stats, new Color(.10f, .075f, .045f, .26f)); centerSpine.sprite = placeholderGradient; SetAbsolute(centerSpine.rectTransform, new Vector2(.5f, 0), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, 0), new Vector2(76, -6));
            var leftDivider = Image("LeftDivider", stats, new Color(0.60f, 0.48f, 0.30f, 0.20f)); SetAbsolute(leftDivider.rectTransform, new Vector2(.5f, 0), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(-54, -3), new Vector2(1, -22));
            var rightDivider = Image("RightDivider", stats, new Color(0.60f, 0.48f, 0.30f, 0.20f)); SetAbsolute(rightDivider.rectTransform, new Vector2(.5f, 0), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(54, -3), new Vector2(1, -22));

            var attack = CreatePrimaryCardStatSlot("Attack", stats, new Vector2(-132, 42), attackIcon, UISemanticColor.Danger, out var attackSlot, out var attackCaption);
            var health = CreatePrimaryCardStatSlot("Health", stats, new Vector2(132, 42), healthIcon, UISemanticColor.Health, out var healthSlot, out var healthCaption);
            var unitCount = CreateQuantityBadge(stats, new Vector2(0, 52), roundSprite, out var unitCountSlot);
            var armor = CreateSecondaryCardStatSlot("Armor", stats, new Vector2(0, 18), armorIcon, out var armorSlot, out var armorCaption);
            TMP_Text unitCountCaption = null;

            // Spell footer: balanced ornamental finish maintaining identical footprint and height
            var spellFooter = Rect("SpellFooter", body.transform); SetAbsolute(spellFooter, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(0, 6), new Vector2(380, 84));
            var spellPlinth = Image("FooterSurface", spellFooter, new Color(.05f, .035f, .02f, .35f)); Stretch(spellPlinth.rectTransform, 0);
            var spellPlinthShadow = spellPlinth.gameObject.AddComponent<Shadow>(); spellPlinthShadow.effectColor = new Color(0, 0, 0, .35f); spellPlinthShadow.effectDistance = new Vector2(0, -2);
            var spellPlinthOutline = spellPlinth.gameObject.AddComponent<Outline>(); spellPlinthOutline.effectColor = new Color(.58f, .44f, .26f, .28f); spellPlinthOutline.effectDistance = new Vector2(1, -1);
            var spellFooterDivider = Image("FooterDivider", spellFooter, new Color(0.68f, 0.52f, 0.30f, 0.50f));
            SetAbsolute(spellFooterDivider.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, 0), new Vector2(-16, 1));
            var footerLeft = Image("LeftRule", spellFooter, WithAlpha(_config.GetColor(UISemanticColor.Gold), .55f)); SetAbsolute(footerLeft.rectTransform, new Vector2(0, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-26, 0), new Vector2(-44, 1));
            var footerRight = Image("RightRule", spellFooter, WithAlpha(_config.GetColor(UISemanticColor.Gold), .55f)); SetAbsolute(footerRight.rectTransform, new Vector2(.5f, .5f), new Vector2(1, .5f), new Vector2(.5f, .5f), new Vector2(26, 0), new Vector2(-44, 1));
            var leftFlourish = Image("LeftFlourish", spellFooter, WithAlpha(_config.GetColor(UISemanticColor.Gold), .55f)); leftFlourish.sprite = armorIcon; leftFlourish.preserveAspect = true; SetAbsolute(leftFlourish.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-75, 0), new Vector2(10, 10));
            var rightFlourish = Image("RightFlourish", spellFooter, WithAlpha(_config.GetColor(UISemanticColor.Gold), .55f)); rightFlourish.sprite = armorIcon; rightFlourish.preserveAspect = true; SetAbsolute(rightFlourish.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(75, 0), new Vector2(10, 10));
            var sigilBacking = Image("SigilBacking", spellFooter, new Color(0.03f, 0.07f, 0.14f, 0.95f)); sigilBacking.sprite = roundSprite; SetAbsolute(sigilBacking.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(38, 38));
            var sigilOutline = sigilBacking.gameObject.AddComponent<Outline>(); sigilOutline.effectColor = WithAlpha(_config.GetColor(UISemanticColor.Gold), .80f); sigilOutline.effectDistance = new Vector2(1, -1);
            var footerSigil = Image("Sigil", spellFooter, WithAlpha(_config.GetColor(UISemanticColor.Mana), .95f)); footerSigil.sprite = manaIcon; footerSigil.preserveAspect = true; SetAbsolute(footerSigil.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(20, 26));

            // Ornamental separator and rarity medallion embedded into the center socket
            var separator = Image("MiddleBanner", root, Color.white); separator.sprite = middleBanner; separator.preserveAspect = true;
            SetAbsolute(separator.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, -382), new Vector2(414, 50));
            var sepShadow = separator.gameObject.AddComponent<Shadow>(); sepShadow.effectColor = new Color(0, 0, 0, 0.45f); sepShadow.effectDistance = new Vector2(0, -2);
            var medallion = Rect("RarityMedallion", root); SetAbsolute(medallion, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, -382), new Vector2(44, 44));
            var socket = Image("Socket", medallion, new Color(.06f, .05f, .04f, .98f)); socket.sprite = roundSprite; Stretch(socket.rectTransform, 0);
            var socketShadow = socket.gameObject.AddComponent<Shadow>(); socketShadow.effectColor = new Color(0, 0, 0, .80f); socketShadow.effectDistance = new Vector2(0, -2);
            var socketOutline = socket.gameObject.AddComponent<Outline>(); socketOutline.effectColor = WithAlpha(_config.GetColor(UISemanticColor.Gold), .82f); socketOutline.effectDistance = new Vector2(1, -1);
            var rarityGem = Image("Gem", medallion, Color.white); rarityGem.sprite = commonRarity; rarityGem.preserveAspect = true; Stretch(rarityGem.rectTransform, 3);

            var card = root.gameObject.AddComponent<UICardFrame>();
            SetReference(card, "_config", _config);
            SetReference(card, "_surface", root.Find("Surface").GetComponent<Image>());
            SetReference(card, "_accent", root.Find("TopAccentBar").GetComponent<Image>());
            SetReference(card, "_artwork", artwork); SetReference(card, "_artworkAspect", artworkAspect);
            SetReference(card, "_artworkPlaceholder", placeholder.gameObject); SetReference(card, "_artworkFrame", artFrame);
            SetReference(card, "_unitBanner", unitBanner.gameObject); SetReference(card, "_spellBanner", spellBanner.gameObject); SetReference(card, "_costBadge", costBadge.gameObject);
            SetReference(card, "_manaIconSprite", manaIcon);
            SetReference(card, "_familyIcon", familyIcon); SetReference(card, "_rarityMedallion", medallion.gameObject);
            SetReference(card, "_rarityGem", rarityGem);
            SetReference(card, "_commonRaritySprite", commonRarity);
            SetReference(card, "_costLabel", cost);
            SetReference(card, "_titleLabel", title); SetReference(card, "_description", rich);
            SetReference(card, "_stats", stats.gameObject); SetReference(card, "_spellFooter", spellFooter.gameObject); SetReference(card, "_attackSlot", attackSlot); SetReference(card, "_armorSlot", armorSlot); SetReference(card, "_healthSlot", healthSlot); SetReference(card, "_unitCountSlot", unitCountSlot);
            SetReference(card, "_attackLabel", attack); SetReference(card, "_armorLabel", armor); SetReference(card, "_healthLabel", health); SetReference(card, "_unitCountLabel", unitCount);
            SetReference(card, "_attackCaption", attackCaption); SetReference(card, "_armorCaption", armorCaption); SetReference(card, "_healthCaption", healthCaption); SetReference(card, "_unitCountCaption", unitCountCaption);
            SetReference(card, "_group", group); SetReference(card, "_stateOutline", stateOutline);
            return SaveComponent(root);
        }

        private static void SetAbsolute(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchorMin; rect.anchorMax = anchorMax; rect.pivot = pivot;
            rect.anchoredPosition = position; rect.sizeDelta = size;
            var layout = rect.GetComponent<LayoutElement>(); if (layout != null) layout.ignoreLayout = true;
        }

        private static Color WithAlpha(Color color, float alpha) { color.a = alpha; return color; }

        private static TMP_Text CreatePrimaryCardStatSlot(string name, Transform parent, Vector2 position, Sprite iconSprite,
            UISemanticColor tone, out GameObject slot, out TMP_Text caption)
        {
            var root = Rect(name + "Slot", parent); slot = root.gameObject;
            SetAbsolute(root, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, .5f), position, new Vector2(106, 64));
            var icon = Image("Icon", root, _config.GetColor(tone)); icon.sprite = iconSprite; icon.preserveAspect = true;
            SetAbsolute(icon.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(12, 6), new Vector2(26, 30));
            var iconShadow = icon.gameObject.AddComponent<Shadow>(); iconShadow.effectColor = new Color(0, 0, 0, 0.45f); iconShadow.effectDistance = new Vector2(0, -1);
            var value = PlainText(name, root, TypographyStyle.Stat, 36);
            value.overflowMode = TextOverflowModes.Overflow;
            SetAbsolute(value.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(40, 6), new Vector2(58, 42));
            value.alignment = TextAlignmentOptions.Center;
            var valShadow = value.gameObject.AddComponent<Shadow>(); valShadow.effectColor = new Color(0, 0, 0, 0.45f); valShadow.effectDistance = new Vector2(0, -1);
            caption = PlainText(name + "Caption", root, TypographyStyle.Caption, 10);
            caption.overflowMode = TextOverflowModes.Overflow;
            SetAbsolute(caption.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(.5f, 0), new Vector2(0, 5), new Vector2(0, 12));
            caption.alignment = TextAlignmentOptions.Center; caption.color = new Color32(175, 155, 128, 220);
            caption.enableAutoSizing = true; caption.fontSizeMin = 8; caption.fontSizeMax = 11;
            return value;
        }

        private static TMP_Text CreateSecondaryCardStatSlot(string name, Transform parent, Vector2 position, Sprite iconSprite,
            out GameObject slot, out TMP_Text caption)
        {
            var root = Rect(name + "Slot", parent); slot = root.gameObject;
            SetAbsolute(root, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, .5f), position, new Vector2(78, 26));
            var plate = Image("Plate", root, new Color(.07f, .08f, .09f, .35f)); Stretch(plate.rectTransform, 0);
            var outline = plate.gameObject.AddComponent<Outline>(); outline.effectColor = new Color(.55f, .62f, .68f, .30f); outline.effectDistance = new Vector2(1, -1);
            var icon = Image("Icon", root, WithAlpha(_config.GetColor(UISemanticColor.Armor), .95f)); icon.sprite = iconSprite; icon.preserveAspect = true;
            SetAbsolute(icon.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(10, 0), new Vector2(16, 16));
            var value = PlainText(name, root, TypographyStyle.Stat, 22);
            value.overflowMode = TextOverflowModes.Overflow;
            SetAbsolute(value.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(28, 0), new Vector2(44, 24));
            value.alignment = TextAlignmentOptions.Center;
            caption = PlainText(name + "Caption", root, TypographyStyle.Caption, 9);
            caption.gameObject.SetActive(false);
            caption.color = Color.clear;
            return value;
        }

        private static TMP_Text CreateQuantityBadge(Transform parent, Vector2 position, Sprite roundSprite, out GameObject slot)
        {
            var root = Rect("UnitCountSlot", parent); slot = root.gameObject;
            SetAbsolute(root, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, .5f), position, new Vector2(38, 38));
            var plate = Image("StackBadge", root, new Color(.07f, .05f, .02f, .96f)); plate.sprite = roundSprite; Stretch(plate.rectTransform, 0);
            var shadow = plate.gameObject.AddComponent<Shadow>(); shadow.effectColor = new Color(0, 0, 0, .65f); shadow.effectDistance = new Vector2(0, -2);
            var outline = plate.gameObject.AddComponent<Outline>(); outline.effectColor = WithAlpha(_config.GetColor(UISemanticColor.Gold), .88f); outline.effectDistance = new Vector2(1, -1);
            var innerBevel = Image("InnerBevel", root, new Color(0.92f, 0.78f, 0.45f, 0.22f)); innerBevel.sprite = roundSprite; Stretch(innerBevel.rectTransform, 2); innerBevel.raycastTarget = false;
            var value = PlainText("UnitCount", root, TypographyStyle.Stat, 22);
            value.overflowMode = TextOverflowModes.Overflow;
            Stretch(value.rectTransform, 0); value.alignment = TextAlignmentOptions.Center;
            value.color = new Color32(248, 232, 196, 255);
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
