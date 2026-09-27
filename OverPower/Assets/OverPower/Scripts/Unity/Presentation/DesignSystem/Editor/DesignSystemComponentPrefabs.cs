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
            var group = root.gameObject.AddComponent<CanvasGroup>();
            var stateOutline = Image("StateOutline", root, Color.clear); Stretch(stateOutline.rectTransform, -7); stateOutline.raycastTarget = false;

            // Textured title/header zone. The source contains three stacked variants; use the light lower third.
            var header = Rect("Header", root);
            SetAbsolute(header, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), Vector2.zero, new Vector2(-10, 96));
            var headerTexture = header.gameObject.AddComponent<RawImage>();
            headerTexture.texture = titleTexture; headerTexture.uvRect = new Rect(0, 0, 1, 0.333333f); headerTexture.raycastTarget = false;
            var headerShade = Image("HeaderShade", header, new Color(0.04f, 0.07f, 0.12f, .24f)); Stretch(headerShade.rectTransform, 0);
            var title = PlainText("Title", header, TypographyStyle.Title, 50);
            SetAbsolute(title.rectTransform, new Vector2(0, .5f), new Vector2(1, .5f), new Vector2(.5f, .5f), new Vector2(28, 0), new Vector2(-128, 52));
            title.alignment = TextAlignmentOptions.Center; title.enableAutoSizing = true; title.fontSizeMin = 22; title.fontSizeMax = 36;
            var costBadge = Rect("CostBadge", header);
            SetAbsolute(costBadge, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(18, 0), new Vector2(70, 70));
            var costSurface = Image("CostSurface", costBadge, new Color(0.03f, 0.06f, .10f, .90f)); costSurface.sprite = roundSprite; Stretch(costSurface.rectTransform, 0);
            var costShadow = costSurface.gameObject.AddComponent<Shadow>(); costShadow.effectColor = new Color(0, 0, 0, .72f); costShadow.effectDistance = new Vector2(0, -3);
            var costOutline = costSurface.gameObject.AddComponent<Outline>(); costOutline.effectColor = WithAlpha(_config.GetColor(UISemanticColor.Mana), .9f); costOutline.effectDistance = new Vector2(1, -1);
            var familyIcon = Image("FamilyIcon", costBadge, Color.white); familyIcon.sprite = manaIcon; familyIcon.preserveAspect = true;
            SetAbsolute(familyIcon.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-15, 0), new Vector2(24, 32));
            var cost = PlainText("Cost", costBadge, TypographyStyle.Stat, 44);
            SetAbsolute(cost.rectTransform, new Vector2(0, 0), new Vector2(1, 1), new Vector2(.5f, .5f), new Vector2(14, 0), new Vector2(-28, 0)); cost.alignment = TextAlignmentOptions.Center;

            // Framed artwork dominates the middle of the card.
            var artFrame = Image("ArtworkFrame", root, WithAlpha(_config.GetColor(UISemanticColor.FloatingBorder), .96f));
            SetAbsolute(artFrame.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, -101), new Vector2(-28, 281));
            var artworkBackground = Image("ArtworkBackdrop", artFrame.transform, _config.GetColor(UISemanticColor.Background)); Stretch(artworkBackground.rectTransform, 4);
            artworkBackground.gameObject.AddComponent<RectMask2D>();
            var artwork = Image("Artwork", artworkBackground.transform, Color.white); Stretch(artwork.rectTransform, 0); artwork.preserveAspect = false; artwork.raycastTarget = false;
            var artworkAspect = artwork.gameObject.AddComponent<AspectRatioFitter>(); artworkAspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; artworkAspect.aspectRatio = 1.5f;
            var placeholder = Rect("ArtworkPlaceholder", artworkBackground.transform); Stretch(placeholder, 5);
            var placeholderSurface = Image("PlaceholderSurface", placeholder, new Color(.025f, .045f, .075f, 1f)); Stretch(placeholderSurface.rectTransform, 0);
            var placeholderTexture = Image("MysticGradient", placeholder, WithAlpha(_config.GetColor(UISemanticColor.Mana), .24f)); placeholderTexture.sprite = placeholderGradient; Stretch(placeholderTexture.rectTransform, 0);
            var glow = Image("MysticGlow", placeholder, WithAlpha(_config.GetColor(UISemanticColor.Mana), .26f)); glow.sprite = placeholderGlow; SetAbsolute(glow.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(250, 190));
            var sigil = Image("ArcaneSigil", placeholder, WithAlpha(_config.GetColor(UISemanticColor.Mana), .64f)); sigil.sprite = manaIcon; sigil.preserveAspect = true; SetAbsolute(sigil.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(84, 104));
            var sigilAccent = Image("ArcaneAccent", placeholder, WithAlpha(_config.GetColor(UISemanticColor.Gold), .24f)); sigilAccent.sprite = armorIcon; sigilAccent.preserveAspect = true; SetAbsolute(sigilAccent.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(132, 132));

            // Unit/Spell banners overlap the title and art, giving the card its type silhouette.
            var unitBanner = Image("UnitBanner", root, Color.white); unitBanner.sprite = unitBannerSprite; unitBanner.preserveAspect = true;
            SetAbsolute(unitBanner.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, -96), new Vector2(368, 42));
            var spellBanner = Image("SpellBanner", root, Color.white); spellBanner.sprite = spellBannerSprite; spellBanner.preserveAspect = true;
            SetAbsolute(spellBanner.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, -96), new Vector2(368, 42));

            // Textured rules body and integrated lower stat rail.
            var body = Image("BodyTexture", root, Color.white); body.sprite = bodySprite; body.type = UnityEngine.UI.Image.Type.Simple;
            SetAbsolute(body.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(.5f, 0), new Vector2(0, 10), new Vector2(-18, 228));
            var bodyShade = Image("BodyShade", body.transform, new Color(.04f, .025f, .015f, .12f)); Stretch(bodyShade.rectTransform, 0);
            var description = PlainText("Description", body.transform, TypographyStyle.BodySmall, 102);
            SetAbsolute(description.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(.5f, 1), new Vector2(0, -42), new Vector2(-76, 102));
            description.alignment = TextAlignmentOptions.TopLeft; description.textWrappingMode = TextWrappingModes.Normal; description.color = new Color32(31, 25, 22, 255); description.lineSpacing = 5;
            var rich = description.gameObject.AddComponent<UIRichText>(); SetReference(rich, "_config", _config);

            var stats = Rect("Stats", body.transform); SetAbsolute(stats, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(0, 14), new Vector2(372, 78));
            var attack = CreatePrimaryCardStatSlot("Attack", stats, new Vector2(-132, 37), attackIcon, UISemanticColor.Danger, out var attackSlot, out var attackCaption);
            var armor = CreateSecondaryCardStatSlot("Armor", stats, new Vector2(-43, 32), armorIcon, out var armorSlot, out var armorCaption);
            var unitCount = CreateQuantityBadge(stats, new Vector2(43, 34), roundSprite, out var unitCountSlot);
            var health = CreatePrimaryCardStatSlot("Health", stats, new Vector2(132, 37), healthIcon, UISemanticColor.Health, out var healthSlot, out var healthCaption);
            TMP_Text unitCountCaption = null;

            var spellFooter = Rect("SpellFooter", body.transform); SetAbsolute(spellFooter, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(0, 16), new Vector2(340, 66));
            var footerLeft = Image("LeftRule", spellFooter, WithAlpha(_config.GetColor(UISemanticColor.Gold), .34f)); SetAbsolute(footerLeft.rectTransform, new Vector2(0, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-24, 0), new Vector2(-38, 1));
            var footerRight = Image("RightRule", spellFooter, WithAlpha(_config.GetColor(UISemanticColor.Gold), .34f)); SetAbsolute(footerRight.rectTransform, new Vector2(.5f, .5f), new Vector2(1, .5f), new Vector2(.5f, .5f), new Vector2(24, 0), new Vector2(-38, 1));
            var footerSigil = Image("Sigil", spellFooter, WithAlpha(_config.GetColor(UISemanticColor.Mana), .48f)); footerSigil.sprite = manaIcon; footerSigil.preserveAspect = true; SetAbsolute(footerSigil.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(28, 38));

            // Ornamental separator and rarity medallion sit over the art/body seam.
            var separator = Image("MiddleBanner", root, Color.white); separator.sprite = middleBanner; separator.preserveAspect = true;
            SetAbsolute(separator.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, -382), new Vector2(414, 54));
            var medallion = Rect("RarityMedallion", root); SetAbsolute(medallion, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, .5f), new Vector2(0, -382), new Vector2(60, 60));
            var socket = Image("Socket", medallion, new Color(.07f, .06f, .055f, .96f)); socket.sprite = roundSprite; Stretch(socket.rectTransform, 0);
            var socketShadow = socket.gameObject.AddComponent<Shadow>(); socketShadow.effectColor = new Color(0, 0, 0, .75f); socketShadow.effectDistance = new Vector2(0, -3);
            var socketOutline = socket.gameObject.AddComponent<Outline>(); socketOutline.effectColor = WithAlpha(_config.GetColor(UISemanticColor.Gold), .72f); socketOutline.effectDistance = new Vector2(1, -1);
            var rarityGem = Image("Gem", medallion, Color.white); rarityGem.sprite = commonRarity; rarityGem.preserveAspect = true; Stretch(rarityGem.rectTransform, 4);

            var card = root.gameObject.AddComponent<UICardFrame>();
            SetReference(card, "_config", _config);
            SetReference(card, "_surface", root.Find("Surface").GetComponent<Image>());
            SetReference(card, "_accent", root.Find("TopAccentBar").GetComponent<Image>());
            SetReference(card, "_artwork", artwork); SetReference(card, "_artworkAspect", artworkAspect);
            SetReference(card, "_artworkPlaceholder", placeholder.gameObject); SetReference(card, "_artworkFrame", artFrame);
            SetReference(card, "_unitBanner", unitBanner.gameObject); SetReference(card, "_spellBanner", spellBanner.gameObject); SetReference(card, "_costBadge", costBadge.gameObject);
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
            SetAbsolute(root, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, .5f), position, new Vector2(104, 70));
            var plate = Image("Plate", root, new Color(.11f, .075f, .04f, .68f)); Stretch(plate.rectTransform, 0);
            var plateShadow = plate.gameObject.AddComponent<Shadow>(); plateShadow.effectColor = new Color(0, 0, 0, .55f); plateShadow.effectDistance = new Vector2(0, -2);
            var plateOutline = plate.gameObject.AddComponent<Outline>(); plateOutline.effectColor = new Color(.72f, .56f, .31f, .52f); plateOutline.effectDistance = new Vector2(1, -1);
            var icon = Image("Icon", root, _config.GetColor(tone)); icon.sprite = iconSprite; icon.preserveAspect = true;
            SetAbsolute(icon.rectTransform, new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(0, .5f), new Vector2(22, 6), new Vector2(28, 34));
            var value = PlainText(name, root, TypographyStyle.Stat, 42);
            SetAbsolute(value.rectTransform, new Vector2(0, 0), new Vector2(1, 1), new Vector2(.5f, .5f), new Vector2(15, 7), new Vector2(-38, -20)); value.alignment = TextAlignmentOptions.Center;
            caption = PlainText(name + "Caption", root, TypographyStyle.Caption, 14);
            SetAbsolute(caption.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(.5f, 0), new Vector2(10, 6), new Vector2(-24, 14));
            caption.alignment = TextAlignmentOptions.Center; caption.color = new Color32(174, 157, 130, 220);
            caption.enableAutoSizing = true; caption.fontSizeMin = 8; caption.fontSizeMax = 10;
            return value;
        }

        private static TMP_Text CreateSecondaryCardStatSlot(string name, Transform parent, Vector2 position, Sprite iconSprite,
            out GameObject slot, out TMP_Text caption)
        {
            var root = Rect(name + "Slot", parent); slot = root.gameObject;
            SetAbsolute(root, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, .5f), position, new Vector2(66, 60));
            var plate = Image("Plate", root, new Color(.10f, .09f, .075f, .54f)); Stretch(plate.rectTransform, 2);
            var outline = plate.gameObject.AddComponent<Outline>(); outline.effectColor = new Color(.60f, .63f, .63f, .42f); outline.effectDistance = new Vector2(1, -1);
            var icon = Image("Icon", root, WithAlpha(_config.GetColor(UISemanticColor.Armor), .76f)); icon.sprite = iconSprite; icon.preserveAspect = true;
            SetAbsolute(icon.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -2), new Vector2(20, 16));
            var value = PlainText(name, root, TypographyStyle.Stat, 32);
            SetAbsolute(value.rectTransform, new Vector2(0, 0), new Vector2(1, 1), new Vector2(.5f, .5f), new Vector2(0, 5), new Vector2(-8, -8)); value.alignment = TextAlignmentOptions.Center;
            caption = PlainText(name + "Caption", root, TypographyStyle.Caption, 12);
            SetAbsolute(caption.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(.5f, 0), new Vector2(0, 4), new Vector2(-8, 12));
            caption.alignment = TextAlignmentOptions.Center; caption.color = new Color32(158, 151, 137, 190);
            caption.enableAutoSizing = true; caption.fontSizeMin = 7; caption.fontSizeMax = 9;
            return value;
        }

        private static TMP_Text CreateQuantityBadge(Transform parent, Vector2 position, Sprite roundSprite, out GameObject slot)
        {
            var root = Rect("UnitCountSlot", parent); slot = root.gameObject;
            SetAbsolute(root, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, .5f), position, new Vector2(62, 62));
            var plate = Image("StackBadge", root, new Color(.10f, .065f, .025f, .88f)); plate.sprite = roundSprite; Stretch(plate.rectTransform, 0);
            var shadow = plate.gameObject.AddComponent<Shadow>(); shadow.effectColor = new Color(0, 0, 0, .65f); shadow.effectDistance = new Vector2(0, -2);
            var outline = plate.gameObject.AddComponent<Outline>(); outline.effectColor = WithAlpha(_config.GetColor(UISemanticColor.Gold), .72f); outline.effectDistance = new Vector2(1, -1);
            var value = PlainText("UnitCount", root, TypographyStyle.Stat, 40); Stretch(value.rectTransform, 6); value.alignment = TextAlignmentOptions.Center;
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
