# OverPower — Naming & Folder Structure Contract

## Unity Project Root
The actual Unity project root is:
```text
<repository_root>/OverPower/
```
All folders, assets, and Unity settings are relative to this directory. Do **not** create a nested `<repository_root>/OverPower/OverPower/` folder, and do **not** move the Unity project to the repository root.

## Root structure
Within the Unity project root, project-owned content lives under `Assets/OverPower/`. Third-party content should remain in its original structure when practical.

```text
Assets/OverPower/
├── Art/
├── Audio/
├── Data/
├── Prefabs/
├── Scenes/
├── Scripts/
├── UI/
├── Localization/
├── Addressables/
├── Tests/
└── Settings/
```

## Suggested details
```text
Scripts/
├── Domain/
├── Application/
├── Infrastructure/
└── Unity/
    ├── Bootstrap/
    ├── Presentation/
    ├── Input/
    ├── Audio/
    ├── Integration/
    └── Debug/
```
Tests: `Tests/Domain`, `Tests/Application`, `Tests/Unity`.
Scenes: `Bootstrap`, `MainMenu`, `Exploration`, `Battle`, `MetaProgression`.

## C# naming
Types `PascalCase`; methods/properties `PascalCase`; private fields `_camelCase`; locals/parameters `camelCase`; interfaces `IName`; booleans read like questions; collections plural. Methods use explicit verbs such as `ResolveAttack`, `DrawCard`, `CanDeployUnit`, `ApplyDamage`.

## Avoid vague names
Avoid `BattleManager`, `Helper`, `Utils`, `Processor`, generic `System`, generic `Data` unless responsibility is precise. Prefer `BattleFlowController`, `BattleActionResolver`, `BattleState`, `BattleAiEvaluator`, `CardView`, `CardPresenter`.

## MonoBehaviour names
Use role-revealing names: `BattleBoardView`, `UnitView`, `CardView`, `ExplorationHudView`, `MainMenuPresenter`, `BattleInputController`, `SceneTransitionController`. Distinguish Domain and Unity types (`UnitStack` vs `UnitStackView`).

## ScriptableObjects
Definition types use `Definition`: `UnitDefinition`, `SpellDefinition`, `ArtifactDefinition`, `AbilityDefinition`. Suggested asset names: `Unit_Guardian.asset`, `Spell_Fireball.asset`, `Artifact_DivineShield.asset`; internal IDs remain `unit.guardian`, etc.

## Prefabs/assets
Useful short prefixes allowed: `PF_`, `MAT_`, `SPR_`, `TEX_`, `SFX_`, `MUS_`, `ANIM_`, `CTRL_`, `VFX_`. Do not prefix everything mechanically.

## GameObjects
Functional PascalCase: `BattleBoard`, `PlayerHud`, `EnemyHud`, `CardHand`, `FrontRow`, `BackRow`, `EndTurnButton`. Avoid `GameObject`, `Panel2`, `Empty`, `Thing`.

## Namespaces
Examples: `OverPower.Domain.Combat`, `OverPower.Application.Combat`, `OverPower.Infrastructure.Persistence`, `OverPower.Unity.Presentation.Battle`, `OverPower.Unity.Integration.HeroEditor4D`.

## Assemblies
Keep limited and meaningful: `OverPower.Domain`, `OverPower.Application`, `OverPower.Infrastructure`, `OverPower.Unity`, `OverPower.Unity.Integration.HeroEditor4D`, `OverPower.Tests.Domain`, `OverPower.Tests.Application`.

## Tests
Classes such as `DamageResolverTests`, `CardDrawServiceTests`, `BattleTurnTests`. Methods use `Action_WhenCondition_ExpectedResult`.

## Forbidden dumping grounds
Do not create vague folders such as `Misc`, `Utils`, `Temp`, `Old`, `Backup`, `New`, `Test2`. Avoid a generic top-level `Scripts/Common` dumping ground.

## Folder depth
Avoid excessive nesting; around 4–5 useful levels is a good normal ceiling. Reassess if paths become very deep.

## File-size guardrails
Not hard limits: >300 lines verify responsibility; >500 lines review recommended; >800 lines likely needs decomposition. Do not split mechanically just for line count.

## Most important rule
A concept has one obvious home. Example: `UnitStack` -> Domain/Units; `UnitStackView` -> Unity/Presentation/Battle; `UnitDefinition` asset -> Data/Units; `HeroEditor4DUnitVisual` -> Unity/Integration/HeroEditor4D.
