# OverPower

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
*   **Infrastructure:** GitHub CI / Actions planned for domain compilation, unit tests, and automated releases
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

## Current State

> ⚠️ **Status: Repository initialization. Unity project not created yet.**
>
> This repository currently contains only the project's foundation, licensing structure, Git environment setup, and development contract specifications. Actual Unity implementation and game code will begin in the next roadmap step (0.1).
