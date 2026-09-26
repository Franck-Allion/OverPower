using System.Collections.Generic;
using OverPower.Unity.Presentation.DesignSystem.Preview;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem.Editor
{
    public static partial class DesignSystemPreviewBuilder
    {
        private static void BuildComponentsPreview(Canvas canvas, RectTransform foundations, RectTransform buttonSamples)
        {
            var panelPrefab = CreatePanel();
            var badgePrefab = CreateBadge();
            var chipPrefab = CreateResourceChip();
            var tooltipPrefab = CreateTooltip();
            var modalPrefab = CreateConfirmDialog(panelPrefab);
            var healthOrbPrefab = CreateVitalResourceOrb("HealthResourceOrb", UISemanticColor.Health);
            var manaOrbPrefab = CreateVitalResourceOrb("ManaResourceOrb", UISemanticColor.Mana);
            var unitStackBadgePrefab = CreateUnitStackBadge();

            var background = Rect("BackgroundContent", canvas.transform);
            Stretch(background, 0);
            var backgroundGroup = background.gameObject.AddComponent<CanvasGroup>();
            foundations.SetParent(background, false);
            var page = Rect("Components", background);
            Stretch(page, UISpacing.Page);
            Vertical(page, 0);
            page.GetComponent<VerticalLayoutGroup>().spacing = UISpacing.Xl;
            Text(page, "components.eyebrow", "OVERPOWER  /  REUSABLE COMPONENTS", "OVERPOWER  /  COMPOSANTS RÉUTILISABLES", TypographyStyle.Caption, 26, UISemanticColor.Primary);
            Text(page, "components.title", "The component library", "La bibliothèque visuelle", TypographyStyle.Display, 82);
            Text(page, "components.intro", "Small primitives. Shared language. Ready for real screens.",
                "Des éléments simples. Un langage commun. Prêts pour les écrans du jeu.", TypographyStyle.Body, 38, UISemanticColor.TextSecondary);
            var columns = Rect("ComponentColumns", page);
            Height(columns, 650);
            Horizontal(columns, 0, 0);
            var columnsLayout = columns.GetComponent<HorizontalLayoutGroup>();
            columnsLayout.spacing = UISpacing.Lg;
            columnsLayout.childForceExpandWidth = columnsLayout.childForceExpandHeight = true;
            var surfaces = Column("Surfaces", columns);
            var details = Column("Details", columns);
            var decisions = Column("Decisions", columns);
            var richText = Column("RichText", columns);
            var gameplayHud = Column("GameplayHUD", columns);
            var samplePanel = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, surfaces);
            Height((RectTransform)samplePanel.transform, 170);
            Label(samplePanel.transform.Find("Header").GetComponent<TMP_Text>(), "panel.title", "GUARDIAN", "GARDIEN");
            var panelBody = PlainText("Body", samplePanel.transform.Find("Content"), TypographyStyle.BodySmall, 64);
            Label(panelBody, "panel.body", "Front-line defender. High armor and shield capacity. Absorbs heavy damage to protect the back row.",
                "Défenseur de première ligne. Armure et bouclier élevés. Absorbe de lourds dégâts pour protéger le rang arrière.");
            var badges = Rect("BadgeSamples", surfaces);
            Horizontal(badges, 0, 0);
            Height(badges, 48);
            BadgeExample(badgePrefab, badges, UISemanticColor.Secondary, "badge.ready", "Ready", "Prêt");
            BadgeExample(badgePrefab, badges, UISemanticColor.Success, "badge.empowered", "Empowered", "Renforcé");
            BadgeExample(badgePrefab, badges, UISemanticColor.Warning, "badge.locked", "Locked", "Verrouillé");
            Text(surfaces, "resources.heading", "Numbers with context", "Des valeurs contextualisées", TypographyStyle.Heading, 48);
            var numbers = Rect("ResourceValues", surfaces);
            Horizontal(numbers, 0, 0);
            Height(numbers, 108);
            numbers.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = true;
            ChipWithCaption(chipPrefab, numbers, UISemanticColor.Health, 9, "resource.health", "Health", "Santé");
            ChipWithCaption(chipPrefab, numbers, UISemanticColor.Mana, 99, "resource.mana", "Mana", "Mana");
            ChipWithCaption(chipPrefab, numbers, UISemanticColor.Gold, 999, "resource.gold", "Gold", "Or");
            var ratios = Rect("ResourceRatios", surfaces);
            Horizontal(ratios, 0, 0);
            Height(ratios, 68);
            ChipExample(chipPrefab, ratios, UISemanticColor.Health, 12, 20);
            ChipExample(chipPrefab, ratios, UISemanticColor.Mana, 100, 100);

            Text(details, "components.details", "02  /  DETAILS & MOTION", "02  /  DÉTAILS ET MOUVEMENT", TypographyStyle.Caption, 32, UISemanticColor.Primary);
            Text(details, "tooltip.intro", "Details, when you need them", "Des détails au bon moment", TypographyStyle.Heading, 48);
            Text(details, "tooltip.instructions", "Hover or focus an inspection control. The detail view stays inside the canvas.",
                "Survolez ou sélectionnez un point d’inspection. Les détails restent dans le cadre.", TypographyStyle.BodySmall, 80, UISemanticColor.TextSecondary);
            var inspect = ExampleButton(details, UIButtonFamily.Secondary, "tooltip.inspect", "Inspect details", "Voir les détails");
            var revealPanel = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, details);
            Height((RectTransform)revealPanel.transform, 166);
            Label(revealPanel.transform.Find("Header").GetComponent<TMP_Text>(), "motion.title", "A restrained reveal", "Une apparition discrète");
            Label(PlainText("Body", revealPanel.transform.Find("Content"), TypographyStyle.BodySmall, 64),
                "motion.body", "Shared timings. No loops. Safe to interrupt.", "Des durées communes. Sans boucle. Interruptible.");
            var reveal = Transition((RectTransform)revealPanel.transform, null, true, false);
            var toggle = ExampleButton(details, UIButtonFamily.Secondary, "motion.toggle", "Show / hide", "Afficher / masquer");

            Text(decisions, "components.decisions", "03  /  DECISIONS", "03  /  DÉCISIONS", TypographyStyle.Caption, 32, UISemanticColor.Primary);
            Text(decisions, "modal.intro", "A clear moment to decide", "Un temps pour décider", TypographyStyle.Heading, 48);
            Text(decisions, "modal.instructions", "A focused dialog, with two clear actions. Background controls pause until it closes.",
                "Un dialogue ciblé, deux actions claires. Les commandes du fond attendent sa fermeture.", TypographyStyle.BodySmall, 100, UISemanticColor.TextSecondary);
            var open = ExampleButton(decisions, UIButtonFamily.Primary, "modal.open", "Open confirmation", "Ouvrir la confirmation");
            Text(decisions, "modal.navigation", "Arrows navigate. Submit confirms. Cancel returns focus to the opening control.",
                "Les flèches naviguent. Valider confirme. Annuler rend le focus au bouton d’origine.", TypographyStyle.BodySmall, 120, UISemanticColor.TextSecondary);
            Text(decisions, "components.note", "Presentation only — no gameplay state or scene loading.",
                "Présentation uniquement — sans état de jeu ni chargement de scène.", TypographyStyle.Caption, 100, UISemanticColor.TextSecondary);

            Text(richText, "components.richtext", "04  /  INLINE ICONS & RICH TEXT", "04  /  ICÔNES EN LIGNE ET TEXTE ENRICHI", TypographyStyle.Caption, 32, UISemanticColor.Primary);
            Text(richText, "richtext.intro", "A clean inline syntax", "Un texte enrichi fluide", TypographyStyle.Heading, 48);
            Text(richText, "richtext.instructions", "Authors write {health} or {mana}. Rendering converts tokens to high-fidelity icons.",
                "Les auteurs écrivent {health} ou {mana}. Le moteur affiche les icônes haute-fidélité.", TypographyStyle.BodySmall, 80, UISemanticColor.TextSecondary);

            // Side-by-side baseline comparison
            RichText(richText, "richtext.comparison", "Baseline:  {attack}  {health}  {gold}  {mana}", "Ligne de base :  {attack}  {health}  {gold}  {mana}", TypographyStyle.Body, 40);

            // Card-description constrained block
            var cardPanel = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, richText);
            Height((RectTransform)cardPanel.transform, 310);
            Label(cardPanel.transform.Find("Header").GetComponent<TMP_Text>(), "richtext.card_title", "Fireball", "Boule de feu");
            var cardContent = cardPanel.transform.Find("Content");
            var cardContentLayout = cardContent.GetComponent<VerticalLayoutGroup>();
            if (cardContentLayout != null)
            {
                cardContentLayout.spacing = UISpacing.Sm;
            }

            // Create individual description sentences using Source Sans 3 Body / BodySmall inside the card panel
            RichText(cardContent, "richtext.sentence1", "Deal 8 {attack} to the targeted unit.", "Inflige 8 {attack} à l'unité ciblée.", TypographyStyle.BodySmall, 40);
            RichText(cardContent, "richtext.sentence2", "Apply burn damage for 2 turns.", "Applique des dégâts de brûlure pendant 2 tours.", TypographyStyle.BodySmall, 40);
            RichText(cardContent, "richtext.sentence3", "Costs 3 {mana}.", "Coûte 3 {mana}.", TypographyStyle.BodySmall, 40);
            RichText(cardContent, "richtext.sentence4", "Gain 15 {gold} upon victory.", "Gagne 15 {gold} en cas de victoire.", TypographyStyle.BodySmall, 40);

            // 05 / GAMEPLAY HUD PRIMITIVES
            Text(gameplayHud, "hud.eyebrow", "05  /  GAMEPLAY HUD PRIMITIVES", "05  /  HUD DE JEU", TypographyStyle.Caption, 32, UISemanticColor.Primary);
            Text(gameplayHud, "hud.intro", "Vital orbs & tactical chips", "Orbes vitaux et jetons tactiques", TypographyStyle.Heading, 48);
            Text(gameplayHud, "hud.instructions", "Ornate orbs represent vital health & mana. Tactical chips display compact gold & AP.",
                "Les orbes ornés représentent la santé et le mana. Les jetons affichent l'or et les PA.", TypographyStyle.BodySmall, 70, UISemanticColor.TextSecondary);

            // 1. Orbs Row (Health 73/100, Mana 3/5)
            var orbsRow = Rect("OrbsRow", gameplayHud);
            Height(orbsRow, 140);
            Horizontal(orbsRow, 0, 0);
            var orbsLayout = orbsRow.GetComponent<HorizontalLayoutGroup>();
            orbsLayout.spacing = UISpacing.Lg;
            orbsLayout.childAlignment = TextAnchor.MiddleCenter;

            var healthOrbObj = (GameObject)PrefabUtility.InstantiatePrefab(healthOrbPrefab, orbsRow);
            healthOrbObj.name = "HealthOrb";
            var healthOrb = healthOrbObj.GetComponent<UIVitalResourceOrb>();
            var healthSo = new SerializedObject(healthOrb);
            healthSo.FindProperty("_current").intValue = 73;
            healthSo.FindProperty("_maximum").intValue = 100;
            healthSo.ApplyModifiedProperties();
            healthOrb.SetValue(73, 100);

            var manaOrbObj = (GameObject)PrefabUtility.InstantiatePrefab(manaOrbPrefab, orbsRow);
            manaOrbObj.name = "ManaOrb";
            var manaOrb = manaOrbObj.GetComponent<UIVitalResourceOrb>();
            var manaSo = new SerializedObject(manaOrb);
            manaSo.FindProperty("_current").intValue = 3;
            manaSo.FindProperty("_maximum").intValue = 5;
            manaSo.ApplyModifiedProperties();
            manaOrb.SetValue(3, 5);

            // 2. Compact Chips Row (Action Points 4/6, Gold 125)
            var chipsRow = Rect("ChipsRow", gameplayHud);
            Height(chipsRow, 56);
            Horizontal(chipsRow, 0, 0);
            chipsRow.GetComponent<HorizontalLayoutGroup>().spacing = UISpacing.Md;

            var apChipObj = (GameObject)PrefabUtility.InstantiatePrefab(chipPrefab, chipsRow);
            apChipObj.name = "APChip";
            var apChip = apChipObj.GetComponent<UIResourceChip>();
            var apSo = new SerializedObject(apChip);
            apSo.FindProperty("_labelPrefix").stringValue = "AP";
            apSo.FindProperty("_value").intValue = 4;
            apSo.FindProperty("_maximum").intValue = 6;
            apSo.FindProperty("_hasMaximum").boolValue = true;
            apSo.FindProperty("_tone").enumValueIndex = (int)UISemanticColor.Interactive;
            apSo.ApplyModifiedProperties();
            apChip.SetPrefix("AP");
            apChip.SetValue(4, 6);
            apChip.SetTone(UISemanticColor.Interactive);

            var goldIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/OverPower/UI/DesignSystem/Icons/Inline/icon_gold.png");
            var goldChipObj = (GameObject)PrefabUtility.InstantiatePrefab(chipPrefab, chipsRow);
            goldChipObj.name = "GoldChip";
            var goldChip = goldChipObj.GetComponent<UIResourceChip>();
            var goldSo = new SerializedObject(goldChip);
            goldSo.FindProperty("_labelPrefix").stringValue = "";
            goldSo.FindProperty("_value").intValue = 125;
            goldSo.FindProperty("_hasMaximum").boolValue = false;
            goldSo.FindProperty("_tone").enumValueIndex = (int)UISemanticColor.Gold;
            goldSo.ApplyModifiedProperties();
            goldChip.SetPrefix("");
            goldChip.SetIcon(goldIcon);
            goldChip.SetValue(125);
            goldChip.SetTone(UISemanticColor.Gold);

            // 3. Unit Stack Badge Row (x8 3/10)
            var stackRow = Rect("StackRow", gameplayHud);
            Height(stackRow, 60);
            Horizontal(stackRow, 0, 0);
            stackRow.GetComponent<HorizontalLayoutGroup>().spacing = UISpacing.Md;

            var stackBadgeObj = (GameObject)PrefabUtility.InstantiatePrefab(unitStackBadgePrefab, stackRow);
            stackBadgeObj.name = "UnitStackBadge";
            var stackBadge = stackBadgeObj.GetComponent<UIUnitStackBadge>();
            var stackSo = new SerializedObject(stackBadge);
            stackSo.FindProperty("_displayedQuantity").intValue = 8;
            stackSo.FindProperty("_currentMemberHp").intValue = 3;
            stackSo.FindProperty("_hpPerMember").intValue = 10;
            stackSo.ApplyModifiedProperties();
            stackBadge.SetValues(8, 3, 10);

            // 4. Gameplay Tooltip Sample
            var potionBtn = ExampleButton(gameplayHud, UIButtonFamily.Secondary, "potion.inspect", "Inspect potion", "Examiner potion");

            Text(page, "components.footer", "Inspect the four edge markers • All text uses the EN / FR preview table",
                "Inspectez les quatre repères de bord • Tous les textes utilisent la table EN / FR", TypographyStyle.Caption, 32, UISemanticColor.TextSecondary);

            var tooltipObject = (GameObject)PrefabUtility.InstantiatePrefab(tooltipPrefab, canvas.transform);
            var tooltip = tooltipObject.GetComponent<UITooltip>();
            tooltip.BindCanvas((RectTransform)canvas.transform, canvas);
            PrefabUtility.RecordPrefabInstancePropertyModifications(tooltip);
            var triggers = new List<UITooltipTrigger> { TooltipTrigger(inspect, tooltip), TooltipTrigger(potionBtn, tooltip) };
            var edges = Rect("EdgeControls", page);
            edges.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            Stretch(edges, -UISpacing.Page);
            var anchors = new[] { new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 1), new Vector2(0.5f, 0) };
            var offsets = new[] { new Vector2(32, 0), new Vector2(-32, 0), new Vector2(0, -32), new Vector2(0, 32) };
            for (int i = 0; i < anchors.Length; i++)
            {
                var edge = InstantiateButton(UIButtonFamily.Icon, edges);
                edge.name = "EdgeInspection" + i;
                var rect = (RectTransform)edge.transform;
                rect.anchorMin = rect.anchorMax = anchors[i];
                rect.anchoredPosition = offsets[i];
                triggers.Add(TooltipTrigger(edge, tooltip));
            }

            var modalObject = (GameObject)PrefabUtility.InstantiatePrefab(modalPrefab, canvas.transform);
            var dialog = modalObject.GetComponent<UIConfirmDialog>();
            dialog.BindBackground(backgroundGroup);
            PrefabUtility.RecordPrefabInstancePropertyModifications(dialog);
            var modalTexts = modalObject.GetComponentsInChildren<TMP_Text>(true);
            foreach (var text in modalTexts)
            {
                if (text.name == "Header") Label(text, "dialog.title", "Abandon Expedition?", "Abandonner l'expédition ?");
                else if (text.name == "Body") Label(text, "dialog.body", "All accumulated gold, meta currency, and progress made during this expedition will be lost. Return to the safety of the main sanctuary?",
                    "Tout l'or, la méta-monnaie et les progrès accumulés pendant cette expédition seront perdus. Revenir à la sécurité du sanctuaire principal ?");
            }
            Label(dialog.Primary.GetComponentInChildren<TMP_Text>(), "dialog.confirm", "Abandon", "Abandonner");
            Label(dialog.Secondary.GetComponentInChildren<TMP_Text>(), "dialog.cancel", "Continue", "Continuer");
            UnityEventTools.AddPersistentListener(open.onClick, dialog.Open);
            var preview = canvas.gameObject.AddComponent<DesignSystemPreviewView>();
            SetReference(preview, "_foundations", foundations.gameObject);
            SetReference(preview, "_components", page.gameObject);
            SetReference(preview, "_componentFocus", open);
            SetReference(preview, "_tooltip", tooltip);
            SetReference(preview, "_dialog", dialog);
            SetReference(preview, "_reveal", reveal);
            SetArray(preview, "_buttonSamples", buttonSamples.GetComponentsInChildren<UIButton>());
            SetArray(preview, "_tooltipTriggers", triggers.ToArray());
            UnityEventTools.AddPersistentListener(toggle.onClick, preview.ToggleReveal);
            var tabs = Rect("PreviewPages", background);
            tabs.anchorMin = tabs.anchorMax = Vector2.one;
            tabs.pivot = Vector2.one;
            tabs.anchoredPosition = new Vector2(-UISpacing.Page, -UISpacing.Page);
            tabs.sizeDelta = new Vector2(520, 48);
            Horizontal(tabs, 0, 0);
            var firstTab = ExampleButton(tabs, UIButtonFamily.Secondary, "tabs.foundations", "Foundations", "Fondations");
            var secondTab = ExampleButton(tabs, UIButtonFamily.Secondary, "tabs.components", "Components", "Composants");
            UnityEventTools.AddPersistentListener(firstTab.onClick, preview.ShowFoundations);
            UnityEventTools.AddPersistentListener(secondTab.onClick, preview.ShowComponents);
            page.gameObject.SetActive(false);
        }

        private static void BadgeExample(GameObject prefab, Transform parent, UISemanticColor tone, string key, string en, string fr)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            var badge = instance.GetComponent<UIBadge>();
            badge.SetTone(tone);
            PrefabUtility.RecordPrefabInstancePropertyModifications(badge);
            Label(instance.GetComponentInChildren<TMP_Text>(), key, en, fr);
        }
        private static void ChipExample(GameObject prefab, Transform parent, UISemanticColor tone, int value, int? maximum = null)
        {
            var chip = ((GameObject)PrefabUtility.InstantiatePrefab(prefab, parent)).GetComponent<UIResourceChip>();
            chip.SetTone(tone);
            if (maximum.HasValue) chip.SetValue(value, maximum.Value); else chip.SetValue(value);
            PrefabUtility.RecordPrefabInstancePropertyModifications(chip);
        }
        private static void ChipWithCaption(GameObject prefab, Transform parent, UISemanticColor tone, int value, string key, string en, string fr)
        {
            var column = Column(key, parent);
            column.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = true;
            Text(column, key, en, fr, TypographyStyle.Caption, 26, tone);
            ChipExample(prefab, column, tone, value);
        }
        private static UIButton ExampleButton(Transform parent, UIButtonFamily family, string key, string en, string fr)
        {
            var button = InstantiateButton(family, parent);
            Label(button.GetComponentInChildren<TMP_Text>(), key, en, fr);
            return button;
        }
        private static void Label(TMP_Text text, string key, string en, string fr)
        {
            AddString(key, en, fr);
            Localize(text, key, en);
        }
        private static UITooltipTrigger TooltipTrigger(UIButton button, UITooltip tooltip)
        {
            button.gameObject.SetActive(false);
            var trigger = button.gameObject.AddComponent<UITooltipTrigger>();
            SetReference(trigger, "_config", _config);
            SetReference(trigger, "_tooltip", tooltip);
            AddString("tooltip.title", "GUARD STANCE", "POSTURE DE GARDE");
            AddString("tooltip.body", "Heal 20 {health} to the targeted unit. Leave the control to dismiss.",
                "Soigne 20 {health} à l'unité ciblée. Quittez le contrôle pour fermer.");
            var so = new SerializedObject(trigger);
            foreach (var pair in new[] { new[] { "_title", "tooltip.title" }, new[] { "_body", "tooltip.body" } })
            {
                var reference = so.FindProperty(pair[0]);
                reference.FindPropertyRelative("m_TableReference.m_TableCollectionName").stringValue = "GUID:" + _strings.SharedData.TableCollectionNameGuid.ToString("N");
                reference.FindPropertyRelative("m_TableEntryReference.m_Key").stringValue = pair[1];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            button.AccessibleLabel.TableReference = _strings.SharedData.TableCollectionNameGuid;
            button.AccessibleLabel.TableEntryReference = "tooltip.title";
            button.gameObject.SetActive(true);
            return trigger;
        }
        private static UIRichText RichText(Transform parent, string key, string en, string fr, TypographyStyle style, float height)
        {
            var rect = Rect(key, parent);
            Height(rect, height);
            rect.gameObject.SetActive(false);

            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.raycastTarget = false;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Ellipsis;
            _config.ApplyTypography(text, style);

            var rich = rect.gameObject.AddComponent<UIRichText>();
            SetReference(rich, "_config", _config);

            AddString(key, en, fr);

            var localized = rect.gameObject.AddComponent<LocalizeStringEvent>();
            localized.StringReference = new LocalizedString(_strings.SharedData.TableCollectionNameGuid, key);
            UnityEventTools.AddPersistentListener(localized.OnUpdateString, rich.SetText);

            rect.gameObject.SetActive(true);
            return rich;
        }
        private static void SetArray<T>(UnityEngine.Object target, string field, T[] values) where T : UnityEngine.Object
        {
            var so = new SerializedObject(target);
            var array = so.FindProperty(field);
            array.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
