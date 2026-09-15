# OverPower — Development Workflow, Git & `next` Protocol

## Source of truth
Once the repository exists, GitHub is authoritative for current roadmap, code, relevant commits, tests, diffs and documentation. Do not use an old chat copy when GitHub has a newer version.

## Branch strategy
Lightweight trunk-based development:
```text
main
feature/<roadmap-id>-<short-name>
fix/<roadmap-id>-<short-name>
chore/<short-name>
```
Avoid long-lived `develop` unless later genuinely needed.

## `main`
Must remain coherent: testable, Domain/Application compile, tests pass, no known critical regression, ideally Unity remains playable at the expected roadmap point.

## Commits
Simple Conventional Commits: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`, `perf`, `ci`. Suggested scopes: `combat`, `cards`, `units`, `exploration`, `ui`, `save`, `progression`, `ai`, `infra`, `ci`.

Example:
```text
feat(combat): add stack deployment

Implements roadmap 0.6.4.
Adds deployment validation and unit stack creation.
Includes tests for occupied tiles and invalid placement.
```

Commits should be atomic and coherent. Prefer roughly 1–4 coherent commits per AI-light task depending on complexity. Avoid meaningless subjects such as `update`, `changes`, `final`, `wip`.

## Before commit
Run applicable checks: `dotnet build`, `dotnet test`. For Unity work also verify Unity script compilation, affected scene/prefab references and `.meta` integrity.

## Unity source control
Visible Meta Files + Force Text. Version `.meta`. Do not regenerate GUIDs without reason. Do not move third-party assets unnecessarily.

## Git LFS
Use for large binaries where appropriate, e.g. PSD/PSB, WAV/MP3/OGG, FBX, MP4/MOV, ZIP and large PNGs when justified. Do not force every small PNG into LFS.

## `.gitignore`
Use Unity-standard ignore rules. Ignore Library, Temp, Obj, Logs, Build/Builds, UserSettings. Do not ignore Assets, Packages, ProjectSettings.

## Pull requests
Optional for tiny solo increments; recommended for architectural changes, larger roadmap batches and risky refactors. Include roadmap item, summary, tests, known risks and screenshots for significant UI changes.

## Versioning
Pre-1.0 semantic form `0.x.y`. Suggested milestones: 0.1 boot+menu, 0.2 exploration, 0.3 combat, 0.4 complete run loop, 1.0 first stable commercial release.

# `next` protocol

## Purpose
`next` is a quality gate plus AI-light prompt generator, not simply “continue coding”.

## Mandatory GitHub review
Before selecting new work:
1. read current roadmap from GitHub;
2. identify previous roadmap scope;
3. inspect latest relevant commit/commit group;
4. inspect actual diff;
5. inspect relevant tests;
6. inspect changed files needed to judge architecture/presentation.

## Status decision
Exactly one:

### REFUSED
Use for significant defect: scope not implemented, important regression, broken tests/build, architecture violation, missing critical tests, major player-facing quality failure, or dangerous debt. Do not advance roadmap. Generate only a focused corrective prompt and corrective commit message.

### ACCEPTED WITH RESERVATION
Use when fundamentally correct but a small issue should be fixed before new work. Select next roadmap lot, but put a mandatory mini-fix first in the prompt and add appropriate tests.

### ACCEPTED
Previous work is complete enough to proceed. Select next coherent roadmap subset and generate the next AI-light prompt.

## Roadmap batching
Choose a logical subset: enough to advance efficiently, small enough for a Light model, coherent, objectively verifiable. Guideline: ~3–8 simple sub-items or ~1–3 complex sub-items. If one item spans Domain + Application + Unity + substantial UI/VFX + tests, keep the batch smaller. Do not group unrelated adjacent items.

## Generated prompt structure
```text
STATUS OF PREVIOUS STEP
- ACCEPTED / ACCEPTED WITH RESERVATION / REFUSED
- evidence/findings
- mandatory fix if any

ROADMAP SCOPE
- roadmap section
- included sub-items
- explicitly excluded sub-items

IMPLEMENTATION GOAL

CONTEXT
- relevant existing architecture/code patterns
- important existing files

MANDATORY PRE-FIX
- only when required

IMPLEMENTATION REQUIREMENTS

ARCHITECTURE CONSTRAINTS

UNITY / PRESENTATION REQUIREMENTS

TESTS REQUIRED

DO NOT

DEFINITION OF DONE

VALIDATION BEFORE COMMIT
- build
- tests
- Unity compile
- scene/prefab checks as applicable

COMMIT MESSAGE
- exact proposed subject/body
```
The prompt must be specific enough that Codex Light / Gemini Flash does not need to invent architectural choices.

## Do not trust completion claims
Do not mark roadmap work complete merely because an AI says “done” or “tests pass”. Validate repository state and actual implementation when access permits.

## Visual acceptance
Player-facing items require presentation quality appropriate for the roadmap stage. Debug controls/raw labels may prove logic temporarily but do not complete premium UI work.

## Final repository rule
Every committed state should remain coherent and explainable; future commits should not be required merely to make the current commit make sense.
