# OverPower — Project Instructions for Gemini CLI & Code Assist

> **Authority**  
> This file is the entry point for Gemini-based development tools. It directs Gemini to the authoritative project contracts, ensuring alignment with Codex and other development agents. Do not duplicate or drift from the core contracts.

## Authoritative Project Contracts

When working on this repository, you must always read and respect the primary project contracts:

1.  [`AGENTS.md`](AGENTS.md) — The master persona, behavior, and interaction standards for AI agents. **Read this first.**
2.  [`ARCHITECTURE.md`](ARCHITECTURE.md) — Technical baseline, decoupling rules, assemblies, persistence, async, error, and logging conventions.
3.  [`GAME_DESIGN_MVP.md`](GAME_DESIGN_MVP.md) — Authoritative MVP gameplay rules and core loop.
4.  [`DESIGN_SYSTEM.md`](DESIGN_SYSTEM.md) — UI/UX layouts, presentation, typography, and premium UI conventions.
5.  [`PROJECT_STRUCTURE.md`](PROJECT_STRUCTURE.md) — Naming, folder layout, namespaces, ScriptableObjects, and asset naming guidelines.
6.  [`DEVELOPMENT_WORKFLOW.md`](DEVELOPMENT_WORKFLOW.md) — Git, commits, tests, definition of done, and the `next` protocol.
7.  [`ROADMAP.md`](ROADMAP.md) — Active milestones, task checklists, and operational source of truth.

## Core Mandates

- **Decoupled Domain:** The core Domain gameplay logic MUST remain pure C# and MUST NOT depend on `UnityEngine` or third-party packages.
- **No Side-Effects on Design Assets:** ScriptableObjects are configurations, not mutable runtime state.
- **Determinism:** All random operations affecting gameplay must utilize the seedable `IRandomService`.
- **Definition of Done:** No feature is done without corresponding unit/integration tests and polished visual presentation appropriate for the roadmap stage.
