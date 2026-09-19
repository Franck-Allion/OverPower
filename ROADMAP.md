# OverPower — MVP Roadmap

> **Status:** Initial roadmap — project starts from scratch  
> **Target:** Unity 6 / URP 2D / Windows PC / 1920×1080 / Keyboard + Mouse  
> **MVP scope:** One complete floor with exploration, guardian battle, end-of-run rewards and meta-progression  
> **Development model:** Small increments, always returning to a coherent and playable state

---

# Roadmap Rules

This roadmap is the operational source of truth for MVP delivery once committed to GitHub.

Development must follow these principles:

- every meaningful increment should leave the project coherent and preferably playable;
- architecture contracts in `AGENTS.md`, `ARCHITECTURE.md`, `GAME_DESIGN_MVP.md`, `DESIGN_SYSTEM.md`, `PROJECT_STRUCTURE.md` and `DEVELOPMENT_WORKFLOW.md` are mandatory;
- Domain gameplay logic must remain independent from Unity and third-party assets;
- player-facing work is not complete when it still looks like debug UI;
- tests and non-regression are part of every roadmap step, not a final phase;
- visuals, animation, feedback and usability are developed progressively, not postponed to the end;
- the `next` command reviews the previous implementation before selecting a coherent subset of the next unchecked items.

Roadmap item states:

```text
[ ] Not started
[~] In progress
[x] Completed and validated
```

---

# Milestone Overview

| Milestone | Goal | Playable checkpoint |
|---|---|---|
| 0.0 | Repository and development environment | Repository ready for AI-assisted development |
| 0.1 | Unity technical foundation | Project boots into a clean Main Menu |
| 0.2 | Core Domain foundation | Deterministic game rules compile and test outside Unity |
| 0.3 | Premium UI foundation | Main Menu and common HUD primitives establish visual language |
| 0.4 | Exploration vertical slice | Player can complete a small exploration room |
| 0.5 | Battle vertical slice | Player can complete a minimal guardian battle |
| 0.6 | Full card/unit combat | 3 units + 3 spells work through the complete battle turn loop |
| 0.7 | Complete exploration economy | Resources, recruits, apothecary and barracks work |
| 0.8 | Run loop and meta-progression | Complete run → rewards → unlock → new run |
| 0.9 | MVP content, balance and UX | One polished floor is replayable and understandable |
| 0.10 | MVP stabilization | Release-quality MVP candidate |

---

# 0.0 Repository, Tooling and AI Development Environment

## 0.0.1 Repository initialization

- [x] Create GitHub repository
- [x] Set default branch to `main`
- [x] Add Unity `.gitignore`
- [x] Configure Git LFS patterns for large binary source assets
- [x] Add initial `README.md`
- [x] Add project license / copyright placeholder
- [x] Add `Docs/ThirdParty/`
- [x] Add `THIRD_PARTY_NOTICES.md`
- [x] Add `AssetInventory.md`
- [x] Commit the AI development context files:
  - [x] `AGENTS.md`
  - [x] `ARCHITECTURE.md`
  - [x] `GAME_DESIGN_MVP.md`
  - [x] `DESIGN_SYSTEM.md`
  - [x] `PROJECT_STRUCTURE.md`
  - [x] `DEVELOPMENT_WORKFLOW.md`
  - [x] this `ROADMAP.md`

### Acceptance gate

- [x] Repository is readable without Unity installed
- [x] Project contracts are committed and versioned
- [x] `main` contains no generated Unity cache/build folders

---

## 0.0.2 Local AI tooling

- [x] Install and configure Codex development environment
- [x] Install and configure Gemini Code Assist environment
- [x] Document AnkleBreaker Unity MCP installation/configuration procedure
- [x] Record that runtime installation/configuration requires the Unity project/editor
- [x] Install `codebase-memory-mcp`
- [x] Configure codebase-memory scope to repository-owned source/documentation
- [x] Ensure proprietary third-party package source is not unnecessarily indexed or injected into AI context
- [x] Document MCP setup in `Docs/Development/AI_TOOLING.md`

### Acceptance gate

- [x] Both AI development environments can read the repository
- [x] MCP setup is documented and reproducible
- [x] AI agents receive project contracts before implementation work

---

# 0.1 Unity Project and Technical Foundation

## 0.1.1 Create the Unity project

- [x] Create Unity 6 project
- [x] Select/configure URP
- [x] Configure 2D Renderer
- [x] Set Windows as primary development target
- [x] Configure reference UI resolution to 1920×1080
- [x] Configure color space/render settings appropriate for the art direction
- [x] Enable Visible Meta Files
- [x] Enable Force Text serialization
- [x] Verify project reopens cleanly after first commit

### Unity MCP activation

- [x] Install/configure AnkleBreaker Unity MCP with the OverPower Unity project
- [x] Launch Unity and validate MCP connectivity
- [x] Perform one harmless read/write validation through Unity MCP
- [x] Document the verified configuration

---

## 0.1.2 Install required Unity packages

- [x] Unity Input System
- [x] Unity Localization
- [x] Addressables
- [x] TextMeshPro essentials
- [x] Unity Test Framework
- [x] Required 2D/URP packages
- [x] Verify package versions are compatible with selected Unity 6 version

Do not introduce optional packages yet unless required by an implemented roadmap item.

---

## 0.1.3 Import owned third-party assets

- [x] Import HeroEditor4D / Character Editor 4D
- [x] Import DOTween
- [x] Import DamageNumbersPro
- [x] Import AllIn1SpriteShader
- [x] Import required Hovl Studio package(s)
- [x] Import required JMO package(s)
- [x] Record exact package versions in `AssetInventory.md`
- [x] Record license/source information
- [x] Keep vendor folder structures intact where possible

### HeroEditor4D compatibility check

- [x] Import HeroEditor4D successfully
- [x] Verify the package compiles without critical errors in the OverPower Unity 6 / URP 2D project
- [x] Verify one representative HeroEditor4D character renders correctly
- [x] Verify no obvious URP/material/shader regression
- [x] Record OverPower-specific integration constraints if discovered

### Acceptance gate

- [x] HeroEditor4D loads and renders correctly in the OverPower Unity 6 / URP 2D project
- [x] No critical package/compiler/shader error remains
- [x] No vendor package is referenced by Domain/Application code

---

## 0.1.4 Create project-owned structure

- [x] Create `Assets/OverPower/`
- [x] Create structure defined by `PROJECT_STRUCTURE.md`
- [x] Create initial namespaces
- [x] Create initial Assembly Definitions:
  - [x] `OverPower.Domain`
  - [x] `OverPower.Application`
  - [x] `OverPower.Infrastructure`
  - [x] `OverPower.Unity`
  - [x] `OverPower.Unity.Integration.HeroEditor4D`
  - [x] Domain test assembly
  - [x] Application test assembly
- [x] Validate dependency direction
- [x] Ensure Domain does not reference `UnityEngine`

---

## 0.1.5 Create base scene flow

- [x] `Bootstrap`
- [x] `MainMenu`
- [x] `Exploration`
- [x] `Battle`
- [x] `MetaProgression`
- [x] Create explicit scene identifiers/configuration
- [x] Create minimal Game Flow service
- [x] Avoid duplicated persistent service objects
- [x] Add clean scene transition abstraction
- [x] Support cancellation during async scene transitions

### Playable checkpoint

Launch game:

```text
Bootstrap
→ MainMenu
```

Main Menu can initially contain only polished placeholder actions.

---

## 0.1.6 Input foundation

- [x] Create Input Actions asset
- [x] Add semantic actions:
  - [x] Navigate
  - [x] Point
  - [x] Click
  - [x] Submit
  - [x] Cancel
  - [x] EndTurn
  - [x] OpenDetails
- [x] Create input adapter/service
- [x] Prevent gameplay classes from reading keyboard/mouse directly
- [x] Ensure action design remains controller-compatible

---

## 0.1.7 Localization foundation

- [x] Configure French locale
- [x] Configure English locale
- [x] Create core string table structure
- [x] Localize Main Menu text
- [x] Add locale selector or development switching mechanism
- [x] Establish stable localization key naming convention
- [~] Add pseudo-localization workflow if practical (Deferred to Milestone 0.3 visual pass due to package setup complexity)
- [x] Verify no player-facing string is hardcoded in Main Menu scripts

---

## 0.1.8 CI foundation

- [x] Create GitHub Actions workflow
- [x] Restore dependencies
- [x] Compile Unity-independent C# Domain/Application test solution
- [x] Run unit tests
- [x] Fail build on test failure
- [x] Add CI documentation
- [x] Keep full Unity build outside mandatory primary CI

### Acceptance gate

- [x] Fresh clone documentation is sufficient to understand setup
- [x] `dotnet build` succeeds for testable C# layer
- [x] `dotnet test` succeeds
- [x] GitHub Actions is green (Verified green CI run #2 for commit 7213354)
- [x] Unity project opens without compilation errors

---

# 0.2 Core Domain Foundation

## 0.2.1 Shared Domain primitives

- [x] Stable `ContentId` value object or equivalent validation strategy
- [x] Define ID format rules
- [x] Detect invalid IDs
- [x] Detect duplicate registered content IDs
- [x] Add tests
- [x] Add result/error primitives without external Result library
- [x] Establish invariant/fail-fast strategy using standard exceptions where appropriate

---

## 0.2.2 Deterministic random service

- [x] Define `IRandomService`
- [x] Seeded implementation
- [x] Deterministic integer/range selection
- [x] Deterministic shuffle
- [x] Tests verifying identical seed → identical sequence
- [x] Tests verifying known seed outputs where implementation stability matters
- [x] Define seed ownership for a Run
- [x] Define derivation/substreams if needed to avoid unrelated systems changing each other's sequence
- [x] Log Run seed through logging abstraction

---

## 0.2.3 Logging abstraction

- [x] Define `IGameLogger`
- [x] Null logger
- [x] Test logger
- [x] Unity logger adapter
- [x] Semantic category conventions
- [x] Development/release verbosity policy

---

## 0.2.4 Content definition model

Create Unity-independent contracts/models required to translate authoring content into runtime content.

- [x] Unit definition model
- [x] Spell definition model
- [x] Artifact definition model
- [x] Ability definition model
- [x] Resource type model
- [x] Validation rules
- [x] Stable IDs
- [x] Localization key references where appropriate
- [x] Tests for invalid content definitions

Do not implement every future field prematurely.

---

## 0.2.5 Unit stack model

- [x] Unit stack quantity
- [x] HP per member
- [x] Total remaining HP
- [x] Current displayed quantity
- [x] Partial current member HP
- [x] Armor
- [x] Attack
- [x] Unit level placeholder/initial structure if needed
    — evaluated: not required yet
- [x] Ability references
- [x] Damage application
- [x] Healing rules
- [x] Death/empty-stack rule
- [x] Invariants
- [x] Tests covering boundary values

Required example:

```text
10 Guardians × 10 HP = 100 HP
73 HP remaining = x8, current member 3/10 HP
```

---

## 0.2.6 Card and deck model

- [x] Card identity
- [x] Card type: unit / spell
- [x] Runtime card representation
- [x] Deck
- [ ] Draw pile
- [ ] Hand
- [ ] Maximum hand size configuration
- [ ] Starting hand configuration
- [ ] Draw count configuration
- [ ] Deck empty behavior
- [x] Unit-card stack/merge semantics
- [x] Spell cards remain independent
- [~] Tests

Reference values:

```text
Deck = 20
Starting hand = 3
Maximum hand = 4
Draw = 1 / turn
```

---

## 0.2.7 Starting-hand rule

- [ ] Guarantee at least one unit card if deck contains one
- [ ] Preserve randomness for remaining cards
- [ ] Deterministic under seed
- [ ] No duplicate physical card references
- [ ] Tests for:
  - [ ] deck with unit cards
  - [ ] deck without unit cards
  - [ ] small deck edge cases

---

## 0.2.8 Player/hero runtime model

- [ ] Hero HP
- [ ] Hero max HP
- [ ] Mana
- [ ] Max mana
- [ ] Gold
- [ ] XP placeholder/structure
- [ ] Resource validation
- [ ] Spend/gain APIs
- [ ] No direct uncontrolled public mutation
- [ ] Tests

---

## 0.2.9 Save-domain DTO baseline

- [ ] Save schema version
- [ ] Settings section
- [ ] Meta currency
- [ ] Unlock IDs
- [ ] Statistics baseline
- [ ] Content/save migration hooks
- [ ] No Unity object references in persisted DTOs

### Acceptance gate

- [ ] Domain foundation compiles entirely outside Unity
- [ ] Unit stack, deck and RNG tests are deterministic
- [ ] No Domain test requires a Unity runtime

---

# 0.3 Premium UI and Presentation Foundation

## 0.3.1 Design System bootstrap

Create only the foundations needed for the first screens.

- [ ] Typography styles
- [ ] Semantic color palette
- [ ] Spacing tokens
- [ ] Motion tokens
- [ ] `UIDesignSystemConfig`
- [ ] Primary button
- [ ] Secondary button
- [ ] Icon button
- [ ] Panel
- [ ] Badge
- [ ] Resource counter/chip
- [ ] Tooltip foundation
- [ ] Modal/confirm dialog foundation
- [ ] Common transition component(s)

---

## 0.3.2 Main Menu premium pass

- [ ] Replace any bootstrap/debug controls
- [ ] Background/art composition
- [ ] Game title treatment
- [ ] Play button
- [ ] Settings access
- [ ] Language access
- [ ] Exit action
- [ ] Hover/pressed/disabled states
- [ ] DOTween transitions
- [ ] UI sound hooks
- [ ] Keyboard/mouse navigation
- [ ] Controller-friendly navigation topology prepared
- [ ] FR/EN localization
- [ ] 16:9 1080p reference validation
- [ ] Basic alternate-resolution validation

### Required placeholders

If final artwork is not available, document replacement PNG requirements.

---

## 0.3.3 Loading / scene transition presentation

- [ ] Premium fade/transition baseline
- [ ] No raw black freeze between scenes
- [ ] Loading indicator support if required
- [ ] Cancellation-safe transition flow
- [ ] Prevent multiple scene transition submissions
- [ ] Keep transitions visually restrained outside combat

---

## 0.3.4 Common gameplay HUD primitives

- [ ] Health display
- [ ] Mana display
- [ ] Gold display
- [ ] Action points display
- [ ] Unit stack badge
- [ ] Card frame baseline
- [ ] Phase/turn banner baseline
- [ ] Standard valid/invalid target states
- [ ] Tooltip visual style

### Acceptance gate

- [ ] Main Menu demonstrates intended quality direction
- [ ] No default Unity-looking controls remain in Main Menu
- [ ] Components are reusable instead of screen-local copies

---

# 0.4 Exploration Vertical Slice

Goal: create a small, attractive, playable exploration room before implementing the complete economy.

## 0.4.1 Exploration Domain model

- [ ] Exploration grid coordinates
- [ ] Walkable/non-walkable cells
- [ ] Player position
- [ ] Action points
- [ ] Movement validation
- [ ] Spend one action point per legal move
- [ ] Cannot move when no action point remains
- [ ] Explicit end-exploration action
- [ ] Exploration completion state
- [ ] Deterministic layout/content generation contract
- [ ] Tests

---

## 0.4.2 Exploration room presentation

- [ ] Static top-down room prototype
- [ ] Grid/cell representation
- [ ] HeroEditor4D hero prefab
- [ ] SortingGroup
- [ ] Correct top-down sorting
- [ ] Camera composition
- [ ] Visual boundaries/walls
- [ ] Hero idle animation
- [ ] Hero directional movement animation
- [ ] Movement feels responsive and premium
- [ ] No debug grid required in final presentation

---

## 0.4.3 Exploration movement interaction

- [ ] Keyboard/mouse interaction model
- [ ] Move cell-by-cell
- [ ] Input routed through Input System
- [ ] Domain validates movement
- [ ] Unity animates accepted movement
- [ ] Prevent input during committed movement animation if necessary
- [ ] Action point HUD updates
- [ ] Invalid move feedback
- [ ] Tests for Domain movement rules

---

## 0.4.4 Minimal collectible

Implement one resource type first.

- [ ] Gold pickup Domain event/result
- [ ] Gold pickup presentation
- [ ] Gold counter animation
- [ ] SFX hook
- [ ] VFX hook
- [ ] Pickup removed only after accepted collection
- [ ] Deterministic placement
- [ ] Tests

---

## 0.4.5 Exploration → Battle transition

- [ ] Trigger when action points reach zero
- [ ] Support explicit End Exploration action
- [ ] Prevent double trigger
- [ ] Persist current run state into battle context
- [ ] Premium transition
- [ ] Cancellation-safe load

### Playable checkpoint

```text
Main Menu
→ Start Run
→ Exploration room
→ Move HeroEditor4D character
→ Spend AP
→ Pick up gold
→ End exploration / reach 0 AP
→ Transition to Battle
```

No full battle mechanics required yet.

---

# 0.5 Battle Vertical Slice

Goal: validate the board, turn engine, HeroEditor4D unit representation and deterministic combat before full card complexity.

## 0.5.1 Battle board Domain

- [ ] Two sides
- [ ] 6 columns
- [ ] Front row
- [ ] Back row
- [ ] Board position value object
- [ ] Occupancy rules
- [ ] Legal placement validation
- [ ] Player/enemy ownership
- [ ] Tests

---

## 0.5.2 Front/Back target resolution

Authoritative targeting rule:

```text
Front
→ Back
→ Hero
```

- [ ] Target resolver
- [ ] Empty-column hero targeting
- [ ] No UI-only targeting rules
- [ ] Tests for every combination

---

## 0.5.3 Battle phase state machine

- [ ] Mana Reset
- [ ] Draw
- [ ] Battle
- [ ] End Turn request
- [ ] Resolution
- [ ] Next Player
- [ ] Hero defeat ends battle
- [ ] Invalid phase transition fails fast
- [ ] Tests for valid transitions
- [ ] Tests for invalid transitions

---

## 0.5.4 Battle actors and shared rule set

- [ ] Player side runtime state
- [ ] Guardian side runtime state
- [ ] Both sides use same legal actions
- [ ] No special guardian rules hidden in battle engine
- [ ] `IBattleAgent` or equivalent boundary
- [ ] Human agent/controller adapter
- [ ] Basic AI agent/controller adapter

---

## 0.5.5 Minimal deterministic AI

- [ ] Enumerate legal actions
- [ ] Select legal deployment/action
- [ ] Seed-driven tie-breaking
- [ ] No illegal action retries as normal behavior
- [ ] Simple scoring baseline
- [ ] Tests with fixed seeds

---

## 0.5.6 Battle board presentation

- [ ] Static top-down battlefield
- [ ] 2×6 cells per side
- [ ] Front/Back visually understandable
- [ ] Player/enemy sides immediately readable
- [ ] Board skin separated from board logic
- [ ] Tile highlight system
- [ ] Hover
- [ ] Selected
- [ ] Valid target
- [ ] Invalid target

Do not reuse SevenBattles homography unless genuinely required; top-down board should remain simpler.

---

## 0.5.7 HeroEditor4D battle unit adapter

- [ ] Typed HeroEditor4D adapter
- [ ] Idle
- [ ] Attack
- [ ] Hit
- [ ] Death
- [ ] Cast/shot hooks where available
- [ ] Facing appropriate opponent direction
- [ ] SortingGroup
- [ ] No generalized reflection-based runtime integration unless required by the package API

---

## 0.5.8 Minimal attack resolution

- [ ] Attack stat
- [ ] Damage application
- [ ] Stack HP update
- [ ] Hero damage when column empty
- [ ] Damage result data
- [ ] Presentation consumes result
- [ ] DamageNumbersPro feedback
- [ ] Hit animation
- [ ] Death animation/removal
- [ ] Tests

---

## 0.5.9 Minimal Battle HUD

- [ ] Hero health
- [ ] Enemy hero health
- [ ] Mana
- [ ] Active side
- [ ] Player-friendly phase
- [ ] End Turn control
- [ ] Unit stack quantity/partial HP
- [ ] Tooltips
- [ ] No raw debug text

### Playable checkpoint

A minimal battle can be completed using predefined starting units/actions:

```text
Battle begins
→ Player action
→ End Turn
→ Resolve attacks
→ Guardian AI turn
→ repeat
→ hero reaches 0 HP
→ battle ends
```

---

# 0.6 Full Card / Unit Combat MVP

## 0.6.1 Card authoring pipeline

- [ ] `UnitDefinition` ScriptableObject adapter
- [ ] `SpellDefinition` ScriptableObject adapter
- [ ] Stable content IDs
- [ ] Localization keys
- [ ] Artwork references / Addressables strategy
- [ ] Cost data
- [ ] Unit stats
- [ ] Spell targeting metadata
- [ ] Validation tooling
- [ ] Duplicate ID validation
- [ ] Missing-reference validation

---

## 0.6.2 Card Design System

- [ ] Finalized MVP card information hierarchy
- [ ] Artwork region
- [ ] Name
- [ ] Cost
- [ ] Card type
- [ ] Description
- [ ] Unit stats where relevant
- [ ] Ability text where relevant
- [ ] Playable highlight
- [ ] Unplayable treatment
- [ ] Hover state
- [ ] Selected state
- [ ] Tooltip/detail view
- [ ] FR/EN dynamic layout
- [ ] Placeholder PNG specification where needed

Visual direction: Magic-like presence, but original layout/art.

---

## 0.6.3 Draw presentation

Inspired by the strong presentation concept retained from Mystic Odyssey, but reimplemented cleanly.

- [ ] Draw card animation
- [ ] Deck → focus/readable position → hand
- [ ] Optional flip/reveal if appropriate
- [ ] Temporarily suppress conflicting interaction
- [ ] Restore interaction after animation
- [ ] DOTween-based implementation
- [ ] Animation never determines Domain card state
- [ ] Handle rapid transitions/cancellation safely

---

## 0.6.4 Unit card deployment

- [ ] Select unit card
- [ ] Display legal cells
- [ ] Enforce legal deployment
- [ ] Enforce column-opposition priority rule
- [ ] Spend costs
- [ ] Remove/move card according to card lifecycle
- [ ] Spawn/update one logical unit stack
- [ ] HeroEditor4D visual
- [ ] Deployment animation/VFX
- [ ] Tests

---

## 0.6.5 Unit stack reinforcement

When gaining/deploying more units of an already represented unit type:

- [ ] Merge quantity correctly
- [ ] Preserve HP semantics
- [ ] Update card quantity
- [ ] Update battlefield stack when rule permits
- [ ] Clear feedback
- [ ] Tests

---

## 0.6.6 Spell framework

- [ ] Generic spell play flow
- [ ] Spell cost validation
- [ ] Target selection
- [ ] Target validation
- [ ] Domain effect execution
- [ ] Presentation effect result
- [ ] VFX adapter
- [ ] Damage/heal feedback
- [ ] Spell discard/lifecycle
- [ ] Tests

---

## 0.6.7 MVP Spell 1

- [ ] Implement first simple spell
- [ ] Definition
- [ ] Domain effect
- [ ] target rules
- [ ] VFX
- [ ] SFX
- [ ] localization
- [ ] tests

---

## 0.6.8 MVP Spell 2

- [ ] Implement second spell with meaningfully different tactical purpose
- [ ] Definition
- [ ] Domain effect
- [ ] target rules
- [ ] VFX
- [ ] SFX
- [ ] localization
- [ ] tests

---

## 0.6.9 MVP Spell 3

- [ ] Implement third spell with meaningfully different tactical purpose
- [ ] Definition
- [ ] Domain effect
- [ ] target rules
- [ ] VFX
- [ ] SFX
- [ ] localization
- [ ] tests

---

## 0.6.10 Unit ability foundation

- [ ] Ability ID
- [ ] Ability definition
- [ ] Legal-use validation
- [ ] Cost
- [ ] target rules
- [ ] execution
- [ ] result
- [ ] UI display
- [ ] tooltip
- [ ] tests

Implement only abilities required by MVP units.

---

## 0.6.11 MVP Unit 1 — Guardian

- [ ] Definition
- [ ] stable ID
- [ ] HeroEditor4D appearance/config
- [ ] stats
- [ ] optional ability
- [ ] card
- [ ] animations
- [ ] localization
- [ ] tests

---

## 0.6.12 MVP Unit 2

- [ ] Definition
- [ ] stable ID
- [ ] HeroEditor4D appearance/config
- [ ] stats
- [ ] tactical identity distinct from Guardian
- [ ] optional ability
- [ ] card
- [ ] localization
- [ ] tests

---

## 0.6.13 MVP Unit 3

- [ ] Definition
- [ ] stable ID
- [ ] HeroEditor4D appearance/config
- [ ] stats
- [ ] tactical identity distinct from Units 1/2
- [ ] optional ability
- [ ] card
- [ ] localization
- [ ] tests

---

## 0.6.14 Cost framework

Support card/action cost combinations required by MVP:

- [ ] mana cost
- [ ] health cost
- [ ] gold cost
- [ ] multi-cost validation if needed
- [ ] atomic payment
- [ ] failure returns explicit gameplay result
- [ ] tests

---

## 0.6.15 Battle AI difficulty foundation

- [ ] Difficulty configuration
- [ ] Easy policy
- [ ] Normal policy
- [ ] Hard-ready extensible scoring
- [ ] AI uses only legal actions
- [ ] Seeded tie-breaking
- [ ] Unit placement evaluation
- [ ] spell-use evaluation
- [ ] resource conservation baseline
- [ ] deterministic tests

MVP does not require deep search unless later playtesting proves it necessary.

---

## 0.6.16 Combat feedback polish

- [ ] DamageNumbersPro damage
- [ ] healing feedback
- [ ] armor/shield feedback if used
- [ ] mana spend/gain feedback
- [ ] card playable glow
- [ ] invalid-action feedback
- [ ] selected target feedback
- [ ] attack anticipation
- [ ] hit feedback
- [ ] death feedback
- [ ] restrained camera/UI impact effects if appropriate
- [ ] sound hooks

---

## 0.6.17 Combat Interaction and Information Architecture

- [ ] Active side
- [ ] Current combat turn
- [ ] Player-friendly phase wording
- [ ] Player hero HP
- [ ] Enemy hero HP
- [ ] Player current/max Mana
- [ ] Enemy Mana where useful
- [ ] Squad count and partial-unit HP
- [ ] Unit-type level placeholder/readiness
- [ ] Selected ability per player squad where applicable
- [ ] Confirmed actions commit clearly
- [ ] Resolution starts automatically after End Turn
- [ ] Automatically transition after resolution
- [ ] Domain phases remain authoritative
- [ ] Hide internal orchestration from player

### Required premium presentation refactor

- [ ] Remove remaining raw/debug-looking text from final combat presentation
- [ ] Replace plain statuses with designed badges, banners, counters, icons and tooltips
- [ ] Replace debug typography in player-facing UI
- [ ] Establish final depth hierarchy for board, cards, HUD, tooltips and VFX
- [ ] Make all controls feel like game UI rather than developer controls

### Playable checkpoint

Player and AI can complete a card-driven battle using the 3 MVP unit types and 3 MVP spells.

---

# 0.7 Complete Exploration Economy

## 0.7.1 Procedural exploration content generation

- [ ] Seed-driven room content placement
- [ ] Placement constraints
- [ ] Avoid unreachable content
- [ ] Avoid illegal overlaps
- [ ] Configurable counts/weights
- [ ] Tests with fixed seeds
- [ ] Diagnostic seed display in development builds

---

## 0.7.2 Gold resource

- [ ] Finalized collection rule
- [ ] Visual
- [ ] VFX
- [ ] SFX
- [ ] HUD update
- [ ] tests

---

## 0.7.3 Max mana resource

- [ ] Increase max mana
- [ ] Define current-mana interaction
- [ ] Visual
- [ ] feedback
- [ ] localization if needed
- [ ] tests

---

## 0.7.4 Max health resource

- [ ] Increase max health
- [ ] Define current-health interaction
- [ ] Visual
- [ ] feedback
- [ ] tests

---

## 0.7.5 Spell pickups

- [ ] Add spell card to deck
- [ ] Duplicate spell allowed
- [ ] Visual pickup
- [ ] card preview
- [ ] feedback
- [ ] tests

---

## 0.7.6 Unit recruitment pickups

- [ ] Recruit unit
- [ ] Merge with existing unit type card
- [ ] Update quantity
- [ ] New unit card created only when type absent
- [ ] Visual recruit feedback
- [ ] tests

---

## 0.7.7 Apothecary

- [ ] Apothecary encounter/interactable
- [ ] Shop UI
- [ ] Spell offers
- [ ] Deterministic offer generation
- [ ] Gold price
- [ ] Purchase validation
- [ ] Purchase result feedback
- [ ] Shop closes cleanly
- [ ] FR/EN
- [ ] tests

---

## 0.7.8 Barracks

- [ ] Barracks encounter/interactable
- [ ] Shop UI
- [ ] Unit offers
- [ ] Deterministic offer generation
- [ ] Gold price
- [ ] Existing unit stack reinforcement
- [ ] Purchase validation
- [ ] Purchase feedback
- [ ] FR/EN
- [ ] tests

---

## 0.7.9 Exploration information architecture

- [ ] Current/max action points
- [ ] Hero HP/max HP
- [ ] Mana/max mana
- [ ] Gold
- [ ] deck access/preview if useful
- [ ] contextual interaction prompt
- [ ] tooltip/detail pattern
- [ ] End Exploration action
- [ ] no debug status panel

---

## 0.7.10 Exploration visual polish

- [ ] Finalize room composition for MVP floor
- [ ] Better environmental sprites
- [ ] Lighting
- [ ] subtle ambient VFX
- [ ] pickup readability
- [ ] shop readability
- [ ] interaction highlights
- [ ] transition polish
- [ ] audio ambience hooks

### Playable checkpoint

A player can make meaningful exploration decisions before entering the guardian battle.

---

# 0.8 Complete Run Loop and Meta-Progression

## 0.8.1 Run state

- [ ] Run ID/session identity if useful
- [ ] Run seed
- [ ] starting hero state
- [ ] current deck
- [ ] current units
- [ ] resources
- [ ] exploration state
- [ ] battle result
- [ ] completion result
- [ ] no cross-run state leakage
- [ ] tests

---

## 0.8.2 End-of-run result

Support:

```text
Victory
Hero Death
```

- [ ] End Run service/use case
- [ ] reward calculation baseline
- [ ] deterministic result inputs
- [ ] summary data
- [ ] tests

---

## 0.8.3 Meta currency

- [ ] Persistent meta currency
- [ ] Add reward after run
- [ ] Save atomically
- [ ] No duplicate reward on repeated transition/load
- [ ] Tests

---

## 0.8.4 Unlock system

- [ ] Unlockable content IDs
- [ ] Locked/unlocked state
- [ ] Unlock price
- [ ] Purchase validation
- [ ] Persistence
- [ ] Content registry integration
- [ ] Save migration readiness
- [ ] tests

---

## 0.8.5 MVP Artifact 1

- [ ] Stable ID
- [ ] unlock cost
- [ ] definition
- [ ] gameplay effect
- [ ] effect integrated through clean Domain boundary
- [ ] art/icon placeholder or final PNG
- [ ] localization
- [ ] tooltip
- [ ] tests

---

## 0.8.6 MVP Artifact 2

- [ ] Stable ID
- [ ] unlock cost
- [ ] definition
- [ ] gameplay effect
- [ ] effect integrated through clean Domain boundary
- [ ] art/icon placeholder or final PNG
- [ ] localization
- [ ] tooltip
- [ ] tests

---

## 0.8.7 MetaProgression screen

- [ ] Premium but visually calmer than combat
- [ ] Current meta currency
- [ ] Unlockable content
- [ ] Locked/unlocked states
- [ ] Spell entries
- [ ] Unit entries if used
- [ ] Artifact entries
- [ ] Tooltip/detail
- [ ] Purchase confirmation
- [ ] Insufficient currency feedback
- [ ] FR/EN
- [ ] DOTween transitions
- [ ] UI sound hooks

---

## 0.8.8 New Run after meta-progression

- [ ] Start a new run
- [ ] Newly unlocked content is available according to design rules
- [ ] Previous run runtime state is cleared
- [ ] persistent meta state remains
- [ ] new deterministic run seed created
- [ ] tests

### Major playable checkpoint — complete MVP loop

```text
Main Menu
→ Start Run
→ Explore
→ Collect / Recruit / Buy
→ Guardian Battle
→ Victory or Death
→ End Run Results
→ Earn Meta Points
→ Unlock Content
→ Start New Run
```

---

# 0.9 MVP Content, Balance, XP and UX

## 0.9.1 Unit XP

- [ ] Define XP award rule
- [ ] Award XP to eligible surviving/in-play units
- [ ] Unit level progression
- [ ] Stat progression rule
- [ ] Ability progression hook
- [ ] UI feedback
- [ ] tests

Keep the system simple enough for MVP.

---

## 0.9.2 Player XP / skill tree foundation

- [ ] Define player XP award
- [ ] Minimal skill tree/domain structure
- [ ] At least enough implementation to validate extensibility
- [ ] Do not overbuild a large tree for MVP
- [ ] UI foundation if included in MVP flow
- [ ] tests

If full player skill tree is judged too large for the MVP, preserve architecture and clearly defer content breadth.

---

## 0.9.3 Initial content balance

Balance:

- [ ] Hero HP
- [ ] starting mana/max mana
- [ ] action points
- [ ] unit HP
- [ ] unit armor
- [ ] unit attack
- [ ] spell costs
- [ ] spell effects
- [ ] gold economy
- [ ] shop prices
- [ ] meta reward
- [ ] unlock costs
- [ ] guardian deck
- [ ] AI difficulty

All values remain configurable.

---

## 0.9.4 Run pacing

- [ ] Exploration duration feels meaningful
- [ ] Battle duration acceptable
- [ ] Player makes multiple meaningful decisions
- [ ] Run is not decided almost entirely by opening draw
- [ ] Guaranteed starting unit rule feels correct
- [ ] Hand size 4 remains readable
- [ ] Deck size 20 remains appropriate
- [ ] Adjust values only with documented reason/playtest evidence

---

## 0.9.5 Tutorial / onboarding

Keep onboarding lightweight.

- [ ] Explain action points
- [ ] Explain pickups
- [ ] Explain shops
- [ ] Explain Front protects Back
- [ ] Explain direct hero damage from empty columns
- [ ] Explain cards/mana
- [ ] Explain End Turn
- [ ] Explain unit stacks
- [ ] Explain end-of-run meta-progression
- [ ] Localized FR/EN
- [ ] Avoid walls of text

---

## 0.9.6 Settings

- [ ] Master volume
- [ ] Music volume
- [ ] SFX volume
- [ ] Language
- [ ] display/fullscreen basics
- [ ] persisted settings
- [ ] safe defaults
- [ ] settings accessible from Main Menu
- [ ] settings architecture ready for later controller options

---

## 0.9.7 Save robustness

- [ ] Atomic save write verified
- [ ] Backup created
- [ ] Corrupted main save → backup recovery test
- [ ] Invalid version behavior
- [ ] Migration test baseline
- [ ] No silent destructive overwrite
- [ ] User-friendly recovery message

---

# 0.10 MVP Stabilization and Release Candidate

## 0.10.1 Non-regression

- [ ] Full Domain test suite
- [ ] Full Application test suite
- [ ] Selected Unity EditMode tests
- [ ] Selected Unity PlayMode smoke tests
- [ ] All known critical bugs fixed
- [ ] Important fixed bugs have regression tests

---

## 0.10.2 Performance validation

Target:

```text
60 FPS @ 1920×1080
<16.6 ms frame
0 B GC/frame target in stable gameplay
```

Validate:

- [ ] Exploration CPU
- [ ] Battle CPU
- [ ] UI animation
- [ ] HeroEditor4D
- [ ] VFX load
- [ ] DamageNumbers
- [ ] Addressables lifetime
- [ ] repeated run memory behavior
- [ ] scene transitions
- [ ] no recurring memory growth

Optimize only measured bottlenecks.

---

## 0.10.3 Visual quality pass

### Main Menu

- [ ] typography
- [ ] composition
- [ ] transitions
- [ ] hover/press states
- [ ] audio
- [ ] no placeholders unintentionally remaining

### Exploration

- [ ] environment readability
- [ ] visual hierarchy
- [ ] pickups
- [ ] HeroEditor4D integration
- [ ] feedback
- [ ] HUD
- [ ] lighting/VFX restraint

### Battle

- [ ] board hierarchy
- [ ] cards
- [ ] HUD
- [ ] HeroEditor4D units
- [ ] target highlights
- [ ] spell VFX
- [ ] damage feedback
- [ ] phase transitions
- [ ] end-turn clarity
- [ ] victory/defeat impact

### MetaProgression

- [ ] unlock readability
- [ ] calmer presentation
- [ ] purchase feedback
- [ ] consistency with Design System

---

## 0.10.4 Localization QA

- [ ] All player-facing text localized
- [ ] FR layout validation
- [ ] EN layout validation
- [ ] no clipping
- [ ] no untranslated debug keys
- [ ] plural/variable formatting validated where used

---

## 0.10.5 Input QA

- [ ] Mouse
- [ ] Keyboard
- [ ] focus behavior
- [ ] escape/cancel flows
- [ ] no input accepted during blocked transitions
- [ ] future controller compatibility not structurally broken

Controller implementation itself is post-MVP unless later promoted.

---

## 0.10.6 Commercial-readiness hygiene

- [ ] Third-party asset inventory updated
- [ ] Licenses documented
- [ ] No proprietary vendor source unnecessarily copied into project documentation
- [ ] Open-source notices complete
- [ ] No unlicensed placeholder artwork
- [ ] Version visible in diagnostics
- [ ] Save schema version finalized for MVP
- [ ] Debug/development-only options excluded or guarded in release

---

## 0.10.7 MVP acceptance run

Perform repeated clean runs from Main Menu to new run after meta-progression.

Validate:

- [ ] no blockers
- [ ] no known save corruption
- [ ] no known deterministic desync in same-version replay scenarios
- [ ] no critical visual regression
- [ ] no critical localization issue
- [ ] no broken Addressables handles
- [ ] no critical console exceptions
- [ ] gameplay loop understandable without developer explanation

---

# MVP Definition of Done

The MVP is considered complete when all of the following are true:

- [ ] Unity project follows the agreed architecture
- [ ] Domain compiles/tests independently of Unity
- [ ] GitHub CI protects non-regression
- [ ] MCP/AI workflow is documented
- [ ] Main Menu is polished
- [ ] Exploration is playable and visually coherent
- [ ] Exploration contains resources, spells, units, Apothecary and Barracks
- [ ] Guardian battle follows shared player/AI rules
- [ ] Battle uses 2×6 Front/Back rows per side
- [ ] Front protects Back and empty columns expose hero damage
- [ ] Unit stacks use cumulative HP semantics
- [ ] Deck uses configurable 20 / 4 / 3 / draw-1 reference rules
- [ ] Starting hand guarantees a unit card when possible
- [ ] 3 MVP unit types are implemented
- [ ] 3 MVP spells are implemented
- [ ] 2 unlockable MVP artifacts are implemented
- [ ] Run can end by victory or hero death
- [ ] Meta points are awarded and saved
- [ ] Meta-progression can unlock new content
- [ ] A second run reflects persistent unlocks
- [ ] FR and EN are supported
- [ ] Save is versioned and atomic
- [ ] Gameplay randomness is seed-driven through `IRandomService`
- [ ] Stable technical content IDs are enforced
- [ ] Performance target is met on the reference development environment
- [ ] No major debug-looking UI remains in player-facing MVP screens
- [ ] Visual presentation is intentionally polished rather than postponed
- [ ] Repository and third-party licensing hygiene are compatible with future commercial development

---

# Explicitly Post-MVP Unless Promoted Later

These items should not expand MVP scope without an explicit decision:

- [ ] Multiple tower floors
- [ ] Final god encounter
- [ ] Large unit roster
- [ ] Large spell catalog
- [ ] Large artifact catalog
- [ ] Deep skill tree
- [ ] Advanced AI search/minimax
- [ ] Controller implementation
- [ ] Cloud saves
- [ ] Steam integration
- [ ] Achievements
- [ ] Online services
- [ ] External analytics
- [ ] Crash-reporting vendor integration
- [ ] Mod support
- [ ] Multiple exploration biomes
- [ ] Multiple battle-board skins beyond what is useful to validate the system
- [ ] Full content pipeline/editor tooling beyond MVP needs

---

# Roadmap Maintenance Rule

When an implementation reveals that a future item should change:

1. update the roadmap in GitHub;
2. explain the reason in the associated commit/PR;
3. preserve architectural contracts;
4. do not silently implement a competing design;
5. let subsequent `next` calls operate from the updated GitHub roadmap.

The roadmap is expected to evolve, but it must remain internally coherent.
