# AI Development Tooling Setup

This document provides a comprehensive, reproducible guide for setting up and maintaining the AI development environment for **OverPower**.

---

## Purpose

To ensure high quality, architectural consistency, and robust developer ergonomics, OverPower utilizes AI-assisted development (such as Codex and Gemini Code Assist) operating under strict, versioned project contracts. 

By grounding AI agents in identical, authoritative repository contracts (`AGENTS.md`, `ARCHITECTURE.md`, `ROADMAP.md`, etc.), we prevent the duplication of systems, architectural drift, and the creation of competing conventions, ensuring that all code written remains clean, decoupled, and fully testable.

---

## Canonical Context

To maintain alignment and ensure zero drift, all AI agents (including Codex, Gemini Code Assist, and Gemini CLI) MUST initialize their session context with the following canonical sources in order of priority:

1.  **[`AGENTS.md`](../../AGENTS.md)** — Master persona, instruction set, and behavior rules.
2.  **Specialized Project Contracts** — Context-specific architectural boundaries:
    *   [`ARCHITECTURE.md`](../../ARCHITECTURE.md) — Tech stack, assemblies, decoupled domain, error, and logging rules.
    *   [`GAME_DESIGN_MVP.md`](../../GAME_DESIGN_MVP.md) — MVP gameplay rules and core loop.
    *   [`DESIGN_SYSTEM.md`](../../DESIGN_SYSTEM.md) — UI layouts, premium design, and typography.
    *   [`PROJECT_STRUCTURE.md`](../../PROJECT_STRUCTURE.md) — Naming, folders, and namespaces.
    *   [`DEVELOPMENT_WORKFLOW.md`](../../DEVELOPMENT_WORKFLOW.md) — Git branching, testing standards, and Definition of Done.
3.  **[`ROADMAP.md`](../../ROADMAP.md)** — Active milestones, task checklists, and current project progress.
4.  **Existing Implementation Patterns** — Previously implemented C# scripts, tests, and domain entities that serve as the local standard for clean, idiomatic development.

---

## Codex Development Environment

### Prerequisites
*   Compatible IDE or environment supporting Codex (e.g., Cursor, VS Code, or custom AI coding agents).
*   Access to the local OverPower workspace folder.

### Repository Access & Loading
*   **Workspace Folder:** Open the root `v1.0` folder of the OverPower repository directly in your IDE.
*   **Project-Instruction Loading:**
    *   Codex-based environments must load [`AGENTS.md`](../../AGENTS.md) as the primary instruction file at the start of any conversation or task.
    *   In Cursor, this can be achieved by adding `.cursorrules` or `.codecompanion` configuration referencing `AGENTS.md`, or by explicitly prompting/indexing `AGENTS.md` and `DEVELOPMENT_WORKFLOW.md` into the chat context.

### MCP Configuration Approach
*   Codex environments support the Model Context Protocol (MCP). Configured MCP servers are defined in the IDE settings (e.g., Cursor’s MCP settings or `claud_desktop_config.json`) to allow Codex to interface with the local workspace indexer and upcoming Unity Editor bridge.

### Verification Procedure
1.  Initiate a fresh prompt.
2.  Ask Codex: *"What is the main mission of OverPower and what are its core architectural rules?"*
3.  **Expected Outcome:** Codex must cite specific details from `AGENTS.md` and `ARCHITECTURE.md` (e.g., Domain must not depend on `UnityEngine`, use seedable randomness, etc.) without relying on external generic Unity assumptions.

---

## Gemini Code Assist Environment

### Prerequisites
*   Gemini Code Assist (IDE extension) or Gemini CLI.
*   Access to the local OverPower workspace folder.

### Repository Access & Loading
*   **Workspace Folder:** Open the root `v1.0` folder of the OverPower repository.
*   **Project-Instruction Loading:**
    *   Gemini CLI automatically loads project-specific instructions from the root [`GEMINI.md`](../../GEMINI.md) file.
    *   `GEMINI.md` serves as a master bridge pointing back to `AGENTS.md` and the rest of the authoritative contracts, ensuring Gemini operates with identical constraints.

### MCP Configuration Approach
*   Set up MCP servers within the Gemini CLI or extension settings. Configuration points to the same local MCP instances (Codebase Memory and AnkleBreaker Unity bridge) used by Codex, ensuring parity in workspace perception.

### Verification Procedure
1.  In the Gemini chat terminal, run: *"Check the project instruction file and summarize the testing rules."*
2.  **Expected Outcome:** Gemini must read `GEMINI.md` and summarize the decoupling rules and the unit testing conventions in `DEVELOPMENT_WORKFLOW.md`.

---

## AnkleBreaker Unity MCP

AnkleBreaker Unity MCP is the bridge that turns AI assistants into full Unity co-pilots by exposing scene manipulation, GameObject inspectors, terrain sculptors, profile metrics, and asset management APIs.

*   **Upstream Project/Sources:**
    *   **MCP Server (Node.js):** [AnkleBreaker-Studio/unity-mcp-server](https://github.com/AnkleBreaker-Studio/unity-mcp-server)
    *   **Unity Editor Plugin (UPM Package):** [AnkleBreaker-Studio/unity-mcp-plugin](https://github.com/AnkleBreaker-Studio/unity-mcp-plugin)
*   **Installed Version/Tag:** TBD (will be locked to the latest stable release at the time of Unity project creation in Roadmap 0.1).

### Installation & Configuration Approach
1.  **Unity Editor Plugin Integration:**  
    Once the Unity project is created (in Roadmap 0.1.1), add the plugin via Unity's Package Manager (`Window > Package Manager > Add package from git URL`):
    ```text
    https://github.com/AnkleBreaker-Studio/unity-mcp-plugin.git
    ```
2.  **Local Node.js MCP Server Installation:**
    Clone the server repository to a secure directory outside the main game repository:
    ```bash
    git clone https://github.com/AnkleBreaker-Studio/unity-mcp-server.git
    cd unity-mcp-server
    npm install
    ```
3.  **AI Assistant Configuration:**  
    Register the MCP server in your AI client's configuration file (e.g., `claude_desktop_config.json`):
    ```json
    {
      "mcpServers": {
        "unity": {
          "command": "node",
          "args": ["C:/path/to/unity-mcp-server/src/index.js"],
          "env": {
            "UNITY_HUB_PATH": "C:\\Program Files\\Unity Hub\\Unity Hub.exe",
            "UNITY_BRIDGE_PORT": "7890"
          }
        }
      }
    }
    ```

### Current Status
*   **Status: Unity Project Created & Integration Verified.**
*   The Unity editor bridge is active, and AnkleBreaker has been successfully integrated with the project at the Unity project root path: `<repository_root>/OverPower/`.

### Verified Unity Validation Step (Milestone 0.1.1 & 0.1.2 Complete)
The validation process has been successfully executed with the running Unity Editor:
1.  Launch the Unity Editor and open the project at `OverPower/`.
2.  The AnkleBreaker plugin starts, listening on port `7890`.
3.  Execute AnkleBreaker commands via Gemini CLI or other MCP clients.
4.  **Verification Result:** Confirmed that core and advanced tools execute correctly, including:
    *   Reading project info, packages, and scene hierarchy.
    *   Successfully executing reversible write operations (e.g., creating and deleting a validation GameObject).

---

## codebase-memory-mcp

`codebase-memory-mcp` is a high-performance C-based code intelligence server that parses source code via Tree-sitter, indexing functions, classes, and structures into a local SQLite database, resulting in a 99% token reduction during codebase research.

*   **Upstream Project/Source:** [DeusData/codebase-memory-mcp](https://github.com/DeusData/codebase-memory-mcp)
*   **Version:** Latest stable binary.

### Configuration & Method Actually Used
1.  **Installation (Windows PowerShell):**
    ```powershell
    irm https://raw.githubusercontent.com/DeusData/codebase-memory-mcp/main/install.ps1 | iex
    ```
2.  **Project Exclusion Scope:**  
    A project-specific `.cbmignore` file is committed to the repository root. This ensures the indexer bypasses local build artifacts, standard IDE directories, and proprietary third-party libraries.
3.  **Registering the MCP Server:**
    Configure the client setting to spawn the `codebase-memory-mcp` binary targeting the OverPower workspace.

### Exclusions & Privacy Policy for Proprietary Packages
To protect commercial intellectual property and prevent cognitive pollution of the AI's context window, **proprietary third-party package sources must not be indexed by `codebase-memory-mcp`.**

The committed [`.cbmignore`](../../.cbmignore) explicitly excludes the following directories:
*   `/OverPower/Assets/HeroEditor4D/`
*   `/OverPower/Assets/DamageNumbersPro/`
*   `/OverPower/Assets/Plugins/AllIn1SpriteShader/`
*   `/OverPower/Assets/Hovl Studio/`
*   `/OverPower/Assets/JMO Assets/`
*   `/OverPower/Assets/Plugins/Demigiant/`
*   `/OverPower/Assets/HealthBar/`

### Verification Procedure
1.  Start the MCP server with the `--ui` flag to visualize the graph.
2.  In your AI terminal, ask: *"Search for the UnitStack class definition and its fields."*
3.  **Expected Outcome:** The indexer must query the local SQLite knowledge graph, return the structural definition rapidly (without loading the entire file if it's large), and must confirm that no matches are found inside the ignored third-party asset folders.

---

## Security & Repository Hygiene

*   **Secrets & Tokens:** **NEVER commit API keys, personal access tokens, or local credentials** (such as `.env` files or hardcoded parameters). Use the `.gitignore` to prevent leakage.
*   **Machine-Local Settings:** Machine-specific paths (e.g., paths to `Unity Hub.exe` or local caches) must reside in local configuration files (like `claude_desktop_config.json` or local IDE settings) and must **not** be committed to the shared repository.
*   **Proprietary Source Safety:** Do not index or train models on commercial assets (e.g., HeroEditor4D script codebase). The `.cbmignore` is your primary guardrail.
*   **Safe Grounding:** Shared project contracts (`AGENTS.md`, `ARCHITECTURE.md`, `ROADMAP.md`) contain no proprietary code or secrets, and are safe, lightweight, and intended for public repository context.

---

## Troubleshooting

### Issue: PowerShell Execution Policy prevents installing `codebase-memory-mcp`
*   **Cause:** Windows prevents running unsigned download scripts by default.
*   **Solution:** Open PowerShell as Administrator and adjust the execution policy for the current scope before running the install script:
    ```powershell
    Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process
    ```
