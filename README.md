# OverPower

[![Core CI](https://github.com/Franck-Allion/OverPower/actions/workflows/core-ci.yml/badge.svg)](https://github.com/Franck-Allion/OverPower/actions/workflows/core-ci.yml)
![Version](https://img.shields.io/github/v/tag/Franck-Allion/OverPower?label=version&sort=semver)
![Unity](https://img.shields.io/badge/Unity-6.3-black)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Languages](https://img.shields.io/badge/Localization-EN%20%7C%20FR-informational)

A 2D roguelite tactical RPG developed with Unity 6.

---

## Project

**OverPower** is a 2D roguelite where a hero tested by the gods explores a tower floor, collects resources, recruits units, and acquires spells before fighting the floor guardian through card-driven tactical battles.

---

## MVP Goal

The immediate milestone for OverPower is to deliver one complete, polished floor demonstrating:

```text
Main Menu
 └── Exploration (Tower floor layout, gathering, events)
      └── Guardian Battle (Card-driven tactical grid-based/turn-based combat)
           ├── Victory or Death (End of run conditions)
           ├── Run Rewards (Rewards selection)
           ├── Meta-Progression (Unlocks & upgrades)
           └── New Run (Next cycle initiation)
```

---

## Technology

OverPower is built using the following modern Unity tech stack:

*   **Engine:** Unity 6
*   **Graphics:** Universal Render Pipeline (URP) with 2D Renderer
*   **Language:** Pure/testable C# Domain architecture (independent of Unity's API)
*   **Target Platform:** Windows PC
*   **Supported Languages:** French (FR) + English (EN)
*   **Infrastructure:** GitHub Actions Core CI validates pure C# Domain/Application build and tests automatically on push/PR
*   **Methodology:** AI-assisted development using Codex/Gemini under strict, versioned project contracts

---

## Project Documentation

The development, design, and architecture of OverPower are governed by the following contract files:

*   [`AGENTS.md`](AGENTS.md) — AI agent personas, directives, and interaction standards.
*   [`ARCHITECTURE.md`](ARCHITECTURE.md) — The decoupled Domain/Application/Infrastructure architecture contract.
*   [`GAME_DESIGN_MVP.md`](GAME_DESIGN_MVP.md) — The Core loop, systems, and gameplay specifications.
*   [`DESIGN_SYSTEM.md`](DESIGN_SYSTEM.md) — Visual rules, layouts, typography, and premium UI conventions.
*   [`PROJECT_STRUCTURE.md`](PROJECT_STRUCTURE.md) — Physical and logical folder organization across the repository.
*   [`DEVELOPMENT_WORKFLOW.md`](DEVELOPMENT_WORKFLOW.md) — Standards, branching models, validation pipelines, and git conventions.
*   [`ROADMAP.md`](ROADMAP.md) — Project milestones, task lists, and operational sources of truth.

---

## Canonical Repository Layout

```text
Repository root/
├── AGENTS.md
├── ARCHITECTURE.md
├── GAME_DESIGN_MVP.md
├── DESIGN_SYSTEM.md
├── PROJECT_STRUCTURE.md
├── DEVELOPMENT_WORKFLOW.md
├── ROADMAP.md
├── Docs/
└── OverPower/        <-- Authoritative Unity Project Root
    ├── Assets/
    ├── Packages/
    └── ProjectSettings/
```

The repository contains project governance and documentation at the root. The actual Unity project root is `/OverPower/`. Do **not** create another nested `/OverPower/OverPower/` or move the Unity project to the repository root.

---

## Current State

> **Status: Core Domain Foundation in Progress (Roadmap through 0.2.5 complete)**
>
> Core Domain Foundation is in progress, with roadmap milestones through 0.2.5 fully complete. Shared primitives, deterministic seedable RNG/substreams, the logging foundation, core content definitions, and the authoritative UnitStack combat and healing-rules model are fully implemented and verified by stand-alone unit tests.

---

## Core Build & Tests

OverPower implements a completely decoupled Domain-driven architecture. Developers can restore, build, and test the pure core of the application completely outside of Unity using standard .NET commands:

```powershell
# 1. Restore lightweight NuGet packages
dotnet restore OverPower.Core.sln

# 2. Compile the core solution in Release configuration
dotnet build OverPower.Core.sln --configuration Release --no-restore

# 3. Run all pure C# unit tests
dotnet test OverPower.Core.sln --configuration Release --no-build
```

For more details on our CI structure and boundaries, see [`Docs/Development/CI.md`](Docs/Development/CI.md).
