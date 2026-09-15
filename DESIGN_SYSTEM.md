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
Create when real screens need them: `PrimaryButton`, `SecondaryButton`, `IconButton`, `Panel`, `Modal`, `Tooltip`, `Badge`, `Counter`, `ResourceChip`, `ProgressBar`, `Tab`, `CardFrame`, `UnitStatLine`, `ConfirmDialog`, `EmptyState`.
Gameplay components may include `HealthDisplay`, `ManaDisplay`, `ArmorDisplay`, `GoldDisplay`, `TurnBanner`, `PhaseBanner`, `UnitStackBadge`, `AbilityButton`, `BoardTileHighlight`, `CardPlayableState`.

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
