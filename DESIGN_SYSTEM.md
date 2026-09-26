# OverPower — Design System Contract

## Objective
OverPower must look polished throughout the MVP. The Design System is built progressively alongside real screens, but foundations appear early. Do not postpone all visual quality to a final polish phase.

## Presentation principles
Combine Hades-like polish/impact, Magic-like card hierarchy, and Slay the Spire / Monster Train-like clarity. Do not copy their art direction literally.

## Combat vs non-combat density
Non-combat screens are calmer and more restrained. Combat may concentrate stronger animation, VFX, reactive highlights, DamageNumbers, higher information density and impact feedback.

## Technology
uGUI + TextMeshPro, DOTween for suitable UI motion, Unity Localization for all player-facing text. Reference 1920×1080 with proper CanvasScaler/anchors/layouts. Preserve future controller navigation.

## Early foundations
Create progressively, but early:
- typography;
- semantic colors;
- spacing;
- buttons;
- panels;
- badges;
- resource counters;
- tooltips;
- cards;
- transitions.

## Tokens
Suggested semantic typography: `Display`, `Title`, `Heading`, `Body`, `BodySmall`, `Caption`, `Stat`, `Button`.
Suggested semantic colors: `Primary`, `Secondary`, `Interactive`, `Selected`, `Disabled`, `Danger`, `Warning`, `Success`, `Health`, `Mana`, `Armor`, `Gold`, `Locked`, `Affordable`, `Unaffordable`.
Suggested spacing scale: 4, 8, 12, 16, 24, 32, 48, 64.
Suggested motion durations: Fast≈0.12s, Normal≈0.20s, Emphasis≈0.35s.

## Reusable components
All reusable UI prefabs reside under `Assets/OverPower/UI/DesignSystem/Components/`.

### General UI Primitives
*   `PrimaryButton.prefab` (`UIButton`, Family: `Primary`) — High-emphasis action button with hover/press scale and audio feedback hooks.
*   `SecondaryButton.prefab` (`UIButton`, Family: `Secondary`) — Standard outlined action button.
*   `IconButton.prefab` (`UIButton`, Family: `Icon`) — 64×64 icon button with full click-target padding.
*   `Panel.prefab` (`UIPanel`) — Card and container surface supporting `Default`, `Elevated`, and `Sunken` styles with top accent bar and inner borders.
*   `Badge.prefab` (`UIBadge`) — Compact status indicator.
*   `ConfirmDialog.prefab` (`UIConfirmDialog`) — Modal dialog with backdrop dimming, input blocking, and primary/secondary actions.
*   `Tooltip.prefab` (`UITooltip`) — Floating inspection card with bounds-clamping and inline icon support. Triggered via `UITooltipTrigger`.

### Authoritative Gameplay HUD Primitives (Milestone 0.3.4)
The following components represent the authoritative gameplay HUD primitives. When constructing Exploration (0.4) or Battle (0.5) screens, AI agents MUST reuse these exact components and prefabs rather than creating screen-local variants:

| Need | Prefab | C# Component | Namespace | Primary API & Usage |
|---|---|---|---|---|
| **Health** | `HealthResourceOrb.prefab` | `UIVitalResourceOrb` | `OverPower.Unity.Presentation.DesignSystem` | `SetValue(current, max)`<br>Crimson liquid with wave motion, unchanged neutral metallic frame. Displays readable `73 / 100`. |
| **Mana** | `ManaResourceOrb.prefab` | `UIVitalResourceOrb` | `OverPower.Unity.Presentation.DesignSystem` | `SetValue(current, max)`<br>Mystic blue liquid with wave motion, unchanged neutral metallic frame. Displays readable `3 / 5`. |
| **Gold** | `ResourceChip.prefab` | `UIResourceChip` | `OverPower.Unity.Presentation.DesignSystem` | `SetValue(gold)`<br>Displays scalar number (e.g. `125`) with `icon_gold.png` and `UISemanticColor.Gold`. |
| **Action Points (AP)** | `ResourceChip.prefab` | `UIResourceChip` | `OverPower.Unity.Presentation.DesignSystem` | `SetPrefix("AP")`<br>`SetValue(current, max)`<br>Displays tactical `AP 4 / 6` with `UISemanticColor.Interactive`. |
| **Unit Stack Badge** | `UnitStackBadge.prefab` | `UIUnitStackBadge` | `OverPower.Unity.Presentation.DesignSystem` | `SetValues(quantity, currentMemberHp, hpPerMember)`<br>Displays primary quantity `x8` and secondary member HP `3 / 10` with mini health fill bar. |
| **Scene Transition** | Persistent under `GameBootstrap` | `UISceneTransitionOverlay` | `OverPower.Unity.Presentation.SceneTransition` | Controlled via `ISceneNavigator.LoadSceneAsync(sceneId)`. Fullscreen `#0D141F` fade, input blocking, and delayed loading indicator. |

### Technical Rules for HUD Primitives
1.  **No Decoupling Violations:** HUD components receive scalar presentation values only. They must not depend on Domain models (`UnitStack`, `HeroState`, `BattleState`).
2.  **Vital Resource Orbs:** Both Health and Mana share the exact same `UIVitalResourceOrb` component. Instances instantiate their material at runtime so simultaneous fill/tone changes never collide. The metallic frame remains neutral silver (`Color.white`) and is never tinted.
3.  **Action Points vs. Vital Orbs:** Action Points are tactical data and must remain compact (`UIResourceChip`), never ornate orbs.
4.  **Reference Showcase:** Open `Assets/OverPower/Scenes/Development/DesignSystemPreview.unity` and inspect column `05 / GAMEPLAY HUD PRIMITIVES` for canonical layout and typography examples.

## Reuse rule
Before creating a new component, inspect `Assets/OverPower/UI/DesignSystem/`. Reuse or cleanly extend an existing component. Do not create local variants merely to finish faster.

## Suggested structure
```text
Assets/OverPower/UI/
├── DesignSystem/
│   ├── Tokens/
│   ├── Typography/
│   ├── Materials/
│   ├── Icons/
│   ├── Components/
│   ├── Gameplay/
│   └── Motion/
└── Screens/
    ├── MainMenu/
    ├── Exploration/
    ├── Battle/
    └── MetaProgression/
```

## Central configuration
A small `UIDesignSystemConfig` ScriptableObject may hold selected global references such as fonts, standard sprites/materials, semantic palette, common UI sounds and transition timings. Do not turn it into a huge universal theme object.

## Inline gameplay icons
To maintain readability and quick semantic recognition inside localized rich text (such as card descriptions, ability tooltips, and explanatory text), the design system establishes a stable, parser-independent inline-icon token convention:

### Author Syntax
Content authors and localization tables express resources using semantic braces:
- `{attack}` - Attack/offensive power
- `{health}` - Health/HP pool
- `{gold}` - Gold run-currency
- `{mana}` - Mana resource

### Presentation Mapping
At render-time, these brace tokens are dynamically translated into TextMeshPro `<sprite name="...">` tags pointing to the centralized `OverPowerInlineIcons` Sprite Asset:
- `{attack}` &rarr; `<sprite name="attack">`
- `{health}` &rarr; `<sprite name="health">`
- `{gold}`   &rarr; `<sprite name="gold">`
- `{mana}`   &rarr; `<sprite name="mana">`

Unknown tokens are left completely untouched and unchanged.

### Source Assets
- **Format:** Transparent background PNG.
- **Dimensions:** 256×256 px.
- **Location:** `Assets/OverPower/UI/DesignSystem/Icons/Inline/`
- **Public Sprite Names:** `attack`, `health`, `gold`, `mana`.

## Cards
Stable information architecture: artwork, name, cost, type, description, stats where relevant, abilities/effects, rarity/state if introduced. Theme can evolve while hierarchy stays consistent.

## Interaction states
Where relevant: Default, Hover, Pressed, Selected, Disabled, Locked, Invalid, Affordable, Unaffordable. Avoid critical color-only communication.

## Feedback standards
Standardize damage, healing, mana/gold changes, playable/unplayable card, valid/invalid target, selected unit, victory, defeat. DamageNumbersPro is allowed for suitable combat feedback.

## Accessibility baseline
Readable contrast, no critical color-only information, legible text, distinct interactive states, future controller navigation, reasonable click targets.

## Missing artwork/placeholders
If final PNG is missing: use a clean placeholder, document purpose, expected width×height, aspect ratio/transparency, and roadmap replacement work. Placeholder does not make final presentation complete.

## No Debug UI as Final UI
Raw controls and labels may exist briefly during implementation, but player-facing roadmap items are not complete until information is expressed through intended counters/icons/badges/bars/tooltips/banners/animations/typography/feedback states.

## Evolution process
```text
build real screen -> discover reusable pattern -> extract/refine component/token -> reuse
```
Once accepted, a pattern becomes the preferred project standard.

## Bootstrap implementation (0.3.1, tokens and buttons)

The selected configuration is `Assets/OverPower/Data/UI/UIDesignSystemConfig.asset`.
Presentation scripts live in `Scripts/Unity/Presentation/DesignSystem`; semantic
roles use enums (`TypographyStyle`, `UISemanticColor`, `UIMotion`). `UISpacing`
provides the eight contract values. Prefabs reference the configuration explicitly.
The central font is applied on enable; TMP's serialized font is an authoring preview,
not an independent theme override.

`UI/DesignSystem/Components` contains PrimaryButton, SecondaryButton and IconButton.
All use `UIButton`, a uGUI Button specialization: filled primary, outlined secondary,
64×64 icon target. A side mark denotes hover/focus; an unavailable bar and reduced
opacity accompany disabled colors. Only the visual child scales; the hit target
stays stable. Each control replaces its own unscaled DOTween and resets on disable
or destruction. Screen composition owns `onClick`, localized TMP labels and the
icon's `AccessibleLabel` LocalizedString hook. This hook prepares future tooltips;
it does not implement a tooltip or a screen-reader integration.

Open `Scenes/Development/DesignSystemPreview.unity` for the FR/EN validation
composition (1920×1080, CanvasScaler, nested layouts). It is not in the build scene
list. `OverPower > UI > Create Design System Preview` explicitly regenerates its
prefabs and scene; it overwrites those authored preview assets. Production screens
are not modified. The preview uses its own `UI.DesignSystemPreview` string table;
technical role names remain identical in both languages.

Production typography: The project integrates Source Sans 3 as the central functional sans-serif font (covering Body, Heading, Button, Stat, and Caption) and Cinzel as the expressive display and title-level font (Display, Title). Both are integrated production typography choices with their license and provenance (SIL Open Font License 1.1) formally documented in the Third-Party notices and Asset Inventory.
The icon button's neutral disc is a built-in Unity sprite placeholder. Screens must
supply an appropriate project-owned icon: 64×64 transparent sprite, square aspect,
readable at 20×20 inside the 64×64 target. Replace it during the first actual screen
integration; it carries no Settings/Language behavior.
