# OverPower — AGENTS.md

> **Purpose**  
> Mandatory entry point for any AI development agent working on OverPower. Read this file first, then the referenced project contracts before modifying code.

## 1. Mission
OverPower is a 2D roguelite developed with Unity 6. The project must progress through small, playable increments. Every meaningful development cycle should leave the project coherent and demonstrable.

The MVP must validate technical feasibility, code clarity, reliability, testability, evolvability, maintainable Unity scene structure, a complete gameplay loop, and **premium visual quality / ergonomics from the MVP**. A player-facing feature that works but looks like debug UI is not finished.

## 2. Required project contracts
Before implementing a task, read the relevant files:
- `ARCHITECTURE.md` — technical stack, assemblies, dependencies, persistence, logging, RNG, async, performance.
- `GAME_DESIGN_MVP.md` — authoritative MVP gameplay rules.
- `DESIGN_SYSTEM.md` — UI/UX, visual hierarchy, reusable components and presentation rules.
- `PROJECT_STRUCTURE.md` — naming, namespaces, asset and folder conventions.
- `DEVELOPMENT_WORKFLOW.md` — Git, commits, tests, Definition of Done, roadmap execution and `next` protocol.

If documents conflict, `AGENTS.md` and the most specific contract for the topic take precedence.

## 3. Core architecture rules
```text
Unity Presentation / Integration
            ↓
        Application
            ↓
          Domain

Infrastructure implements ports exposed by inner layers.
```

Mandatory rules:
- `Domain` must not reference `UnityEngine`.
- `Domain` must not reference third-party Unity assets.
- `Application` orchestrates use cases and game flow.
- `Infrastructure` implements technical services such as persistence, logging, configuration and deterministic randomness.
- `Unity` handles presentation, scenes, input, audio, VFX, animation and third-party integrations.
- Do not put authoritative gameplay logic in `MonoBehaviour`.
- Do not create an alternative architecture because it is more familiar.
- Extend existing patterns instead of introducing parallel systems.
- Avoid circular assembly references.

## 4. Third-party packages
Authorized/owned packages include Character Editor 4D / HeroEditor4D, DamageNumbersPro, AllIn1SpriteShader, Hovl Studio assets, JMO assets and DOTween.

Third-party APIs stay isolated in Unity integration code. `Domain` and `Application` must never directly reference them. Do not add a new dependency unless it brings significant value. Prefer MIT/BSD/Apache-2.0 for new open-source dependencies and verify licensing before integration.

## 5. Unity and platform baseline
- Unity 6.
- URP + 2D Renderer.
- PC Windows MVP.
- Reference resolution: 1920×1080.
- Keyboard + mouse for MVP.
- Future controller support must remain possible.
- Unity Input System mandatory.
- uGUI + TextMeshPro standard UI.
- Unity Localization from the beginning.
- French + English in MVP.
- No player-facing string hardcoded in gameplay/UI scripts.
- Addressables from the beginning only where they provide real value.

## 6. Domain and runtime state
Use normal C# classes for gameplay state/rules. ScriptableObjects are primarily design/configuration definitions, not authoritative runtime state.

```text
UnitDefinition -> design-time content
UnitStackState -> runtime gameplay state
```

Never mutate project ScriptableObject assets as the normal runtime-state or persistence mechanism.

## 7. Stable content IDs
Every important content item has an immutable technical ID, for example:
```text
unit.guardian
spell.fireball
artifact.divine_shield
ability.guardian.taunt
resource.gold
```
Rules: lowercase, dot-separated, no spaces/accents, displayed names are never IDs, and shipped IDs are never casually renamed or repurposed.

## 8. Deterministic randomness
All gameplay-affecting randomness uses `IRandomService`. Forbidden in Domain gameplay: `UnityEngine.Random`, direct global `System.Random`, or hidden implicit random generators. Runs, battles, procedural exploration and tests must be reproducible from a known seed. Important seeds must be recorded in diagnostics/logs.

## 9. Async rules
- No `async void` except legitimate Unity callbacks where unavoidable.
- Use `CancellationToken` for operations that may outlive a scene/screen/object.
- Scene transitions cancel associated long-running operations where appropriate.
- Do not introduce UniTask while .NET/Unity APIs are sufficient.
- Do not mix multiple async paradigms for the same responsibility without a clear reason.
- Coroutines are for Unity timing/visual sequencing, not a substitute for application/domain orchestration.

## 10. Error and invariant policy
Classify invalid situations as:
1. expected gameplay result -> explicit result value/object;
2. invariant violation -> fail fast;
3. technical error -> handle at technical boundary.

Never use empty `catch { }`. Never swallow an exception and continue with corrupted state. Do not add null checks without understanding why the value is null. Gameplay validation must not rely on UI validation. Critical invariants require tests.

## 11. Logging
Do not spread `Debug.Log` through gameplay code. Use the project logging abstraction. Prefer semantic categories such as `RUN.START`, `COMBAT.CARD.PLAYED`, `COMBAT.DAMAGE.RESOLVED`, `SAVE.SUCCESS`. Verbose traces belong to Development Builds. No external telemetry/analytics in MVP, but interfaces must allow future crash reporting/analytics.

## 12. Runtime anti-patterns
Do not:
- put business logic in `Update()`;
- repeatedly call global object searches in hot paths;
- repeatedly call `GetComponent` in hot loops when a reference can be cached;
- use `Resources.Load` as normal dynamic loading;
- continuously instantiate/destroy high-frequency temporary effects when pooling is appropriate;
- use LINQ in hot paths when it creates avoidable allocations;
- create global service locators;
- create a global event bus as the primary communication system;
- duplicate gameplay rules between player and AI;
- use ScriptableObjects as mutable authoritative runtime state;
- use package-specific APIs from Domain/Application;
- hardcode configurable game-design values;
- use display names as IDs;
- rely on Script Execution Order for critical behavior unless documented;
- rely on magic GameObject names for dependencies;
- create vague God Objects or dumping-ground helpers;
- catch exceptions only to log and continue in an invalid state;
- introduce premature low-level optimization that harms clarity without profiler evidence.

## 13. Performance contract
Target:
- 60 FPS at 1920×1080;
- frame budget <16.6 ms;
- ideally 8–10 ms CPU on the reference dev machine;
- target 0 B GC allocation/frame in stable exploration/combat;
- no heavy synchronous asset/file loading during gameplay;
- no recurring memory growth across runs;
- correct Addressables handle lifetime;
- pool frequent temporary VFX/DamageNumbers/projectiles when appropriate;
- scene transitions target <2 s perceptible loading on SSD or use a premium transition;
- profile before advanced optimization.

## 14. Testing
Most gameplay rules must be testable outside Unity. Use unit tests for Domain/Application. Use Unity Test Framework only where Unity is required. Bug fixes should add regression tests when appropriate. Never delete or weaken tests merely to make CI pass.

## 15. AI-agent behavior
Before editing: read the task, read relevant contracts, inspect existing patterns, identify the smallest coherent change, preserve architecture, identify required tests.

During implementation: prefer explicit code, small cohesive classes, interfaces only for real boundaries, reuse Design System components, do not redesign unrelated systems, do not modify unrelated files unless required.

Before completion: build, run relevant tests, verify Unity compilation where applicable, validate scene/prefab references, remove temporary debug code, update relevant docs, verify localization, document placeholder replacement dimensions.

## 16. Definition of Done
An item is complete only when requested behavior works, architecture contracts are respected, relevant tests pass, no known regression is introduced, error cases are handled, unnecessary debug logging is gone, player-facing presentation is appropriate for the roadmap stage, localization is used, placeholders are documented and relevant technical docs are updated.

## 17. Git
Follow `DEVELOPMENT_WORKFLOW.md`. Never force-push `main`, commit a knowingly broken state to `main`, use meaningless commit messages, combine unrelated broad refactors with feature work, or rewrite tests to hide regressions.

## 18. Final rule
When uncertain, choose the implementation that is easiest to understand, easiest to test, consistent with existing patterns, isolated from Unity/third-party details when it is gameplay logic, and least likely to create a second competing way to do the same thing.
