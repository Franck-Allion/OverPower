# OverPower — Architecture & Technical Stack Contract

## Technical baseline
| Area | Decision |
|---|---|
| Engine | Unity 6 |
| Rendering | URP + 2D Renderer |
| Target | Windows PC, 1920×1080 reference |
| Input | Unity Input System |
| UI | uGUI + TextMeshPro |
| Localization | Unity Localization |
| Languages | French + English |
| Animation/Tweening | DOTween |
| Dynamic assets | Addressables, selectively |
| Domain tests | .NET/NUnit-compatible tests |
| Unity tests | Unity Test Framework |
| Persistence | Versioned atomic JSON save |
| Randomness | Seedable `IRandomService` |
| Telemetry | None externally in MVP; interfaces only |
| CI | GitHub Actions: compile testable C# + tests |
| MCP | AnkleBreaker Unity MCP + codebase-memory-mcp |

## Architecture
```text
OverPower
├── Domain
│   ├── Combat
│   ├── Cards
│   ├── Units
│   ├── Exploration
│   ├── Run
│   ├── Progression
│   └── Economy
├── Application
│   ├── UseCases
│   ├── GameFlow
│   └── Ports
├── Infrastructure
│   ├── Persistence
│   ├── Random
│   ├── Logging
│   └── Configuration
└── Unity
    ├── Bootstrap
    ├── Presentation
    ├── Input
    ├── Audio
    └── Integration
```
Dependency direction: `Unity -> Application -> Domain`; Infrastructure implements inner-layer ports. Domain never depends on Unity or third-party packages.

## Recommended assemblies
Keep asmdefs limited and meaningful:
```text
OverPower.Domain
OverPower.Application
OverPower.Infrastructure
OverPower.Unity
OverPower.Unity.Integration.HeroEditor4D
OverPower.Tests.Domain
OverPower.Tests.Application
```

## Scene architecture
Reference scenes:
```text
Bootstrap -> MainMenu -> Exploration -> Battle -> MetaProgression
```
Bootstrap initializes global services. Gameplay scenes are presentation/composition containers, not authoritative business-logic containers.

## Input architecture
Consume semantic actions, not physical keys: `Navigate`, `Point`, `Click`, `Submit`, `Cancel`, `EndTurn`, `OpenDetails`.

## ScriptableObject policy
Definitions: `UnitDefinition`, `SpellDefinition`, `ArtifactDefinition`, `AbilityDefinition`. Runtime state uses pure C# objects. Never persist runtime state by mutating design assets.

## Content IDs
Stable IDs such as `unit.guardian`, `spell.fireball`, `artifact.divine_shield`. Displayed names come from localization.

Runtime cards live in `Domain/Cards`. Immutable `RuntimeCard` holds a caller-supplied
`CardInstanceId`, `CardType` (Unit/Spell), and authored `ContentId`, with matching
`unit.`/`spell.` categories. Physical identity is distinct from content identity:
multiple Spell cards may reference the same spell ContentId, while Deck permits
only one physical Unit card per unit ContentId. Compare `InstanceId` explicitly;
`RuntimeCard` retains reference equality. IDs are positive `ulong` values, allocated
by future deterministic construction orchestration, never by RuntimeCard or Deck.
Zero/default IDs are rejected at the runtime-card boundary.

`Deck` is immutable composition constructed from supplied cards, preserving their
references and order in a defensive read-only collection. It rejects null inputs,
duplicate physical IDs across all card types, and duplicate unit ContentIds.
An empty composition is valid. Future acquisition can construct a new validated
composition with the added cards. Deck is distinct from the future randomized
battle DrawPile and current Hand; shuffle, draw and empty-deck gameplay are deferred.

A unit RuntimeCard identifies a unit type only. Neither RuntimeCard nor Deck owns
recruited quantity. External run-roster state will own that quantity; deployment
will combine the card's ContentId, the roster's full currently owned quantity and
UnitDefinition into one UnitStack, which owns combat HP/quantity. Further recruits
of an existing type change the roster, keeping its single unit card. Roster,
acquisition, deployment and spell execution remain deferred to later roadmap work.

## Addressables
Use selectively for items such as unit prefabs, card artwork, larger VFX/audio, expanding content. Centralize load/release behavior and avoid leaked handles.

## Third-party integration
Third-party assets are presentation/integration dependencies.
- HeroEditor4D: typed adapter assembly preferred over generalized reflection.
- DamageNumbersPro: presentation only after Domain resolved values.
- DOTween: UI/presentation animation only; gameplay rules must not live exclusively in tween callbacks.

## Persistence
Versioned JSON, migration-aware, atomic write, backup recovery:
```text
serialize -> temp file -> successful write -> atomic replace -> backup
```
Initial durable data: schema version, meta currency, unlocks, settings, required statistics. Active-run persistence may be added later.

## Logging
Use `IGameLogger` or equivalent, with adapters such as `UnityLogger`, `NullLogger`, `TestLogger`, `CompositeLogger`, `FileLogger`. Domain must not bind to Unity logging.

## Randomness and determinism
Use seedable `IRandomService`. All gameplay randomness goes through it. Record run seed, game version and relevant content/save versions. Tests use fixed seeds and sets of known seeds.

## AI opponent
Human and AI share battle rules and legal actions. Conceptual boundary:
```text
IBattleAgent
├── HumanBattleAgent
└── AiBattleAgent
```
Difficulty changes decision quality, not rule privileges.

## Async strategy
Use Task/.NET/Unity async APIs first. No UniTask without demonstrated need. Use CancellationToken for lifetimes beyond current scene/screen. Avoid orphan tasks. Keep visual coroutines separate from application/domain orchestration.

## Performance contract
60 FPS @1080p, <16.6 ms frame budget, target 8–10 ms CPU headroom, 0 B GC/frame target during stable gameplay, no blocking heavy load, pool recurring temporary objects where useful, release Addressables correctly, no recurring memory growth, target <2 s perceptible scene transition on SSD, profile before advanced optimization.

## Error policy
Expected gameplay outcome -> explicit result. Invariant violation -> fail fast. Technical failure -> handle at boundary. No empty catches; never continue with invalid authoritative state.

## CI
GitHub Actions must restore dependencies, compile Unity-independent/testable C#, run Domain/Application tests, and fail on regression. Full Unity build is not required in the primary CI contract.
