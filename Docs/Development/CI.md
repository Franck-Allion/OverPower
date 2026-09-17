# Core CI Pipeline & Architectural Validation

This document defines the architecture, capabilities, local execution procedures, and design constraints of the **OverPower Core CI** pipeline.

---

## Architectural Isolation Promise

One of the central architectural contracts of OverPower is **strict decoupling of core business logic from Unity and presentation concerns**. 

Our core Domain and Application layers are written in pure C# and compiled into dedicated Assemblies that contain:
* No references to the `UnityEngine` engine or `UnityEditor` editor assemblies.
* No dependencies on any commercial Unity Asset Store packages (like HeroEditor4D, DOTween, or DamageNumbersPro).
* No dependency on Unity's proprietary asset database or serialized settings.

To enforce and prove this promise at a toolset level, the primary Continuous Integration (CI) pipeline compiles and executes unit tests for these assemblies **without requiring any Unity Editor, licenses, activation, or proprietary binaries**.

---

## Solution & Project Structure

The decoupled build pipeline utilizes a standalone **Microsoft .NET SDK** solution structure independent of Unity's generated solution files:

```text
repository/
├── OverPower.Core.sln                      # Core .NET 8 solution
├── DotNet/
│   ├── OverPower.Domain/
│   │   └── OverPower.Domain.csproj         # Glob-compiles Assets/OverPower/Scripts/Domain/
│   ├── OverPower.Application/
│   │   └── OverPower.Application.csproj    # Glob-compiles Assets/OverPower/Scripts/Application/
│   ├── OverPower.Domain.Tests/
│   │   └── OverPower.Domain.Tests.csproj   # Glob-compiles Assets/OverPower/Tests/Domain/
│   └── OverPower.Application.Tests/
│       └── OverPower.Application.Tests.csproj # Glob-compiles Assets/OverPower/Tests/Application/
└── OverPower/
    └── Assets/
        └── OverPower/                      # Authoritative Unity project folder
```

### The "One Source" Rule
To prevent code duplication, the `.csproj` files do not hold separate copies of C# classes. They use MSBuild globbing patterns to directly compile the **authoritative C# source files owned by the Unity project root**. 

This guarantees that any changes made during development inside Unity are automatically validated by the external `.NET` pipeline.

---

## Continuous Integration Workflow (`core-ci.yml`)

The mandatory GitHub Actions core pipeline runs on every push or pull request targeting the `main` branch. It executes:

1. **Checkout:** Clones the public repository.
2. **Setup .NET:** Installs the standard, stable **.NET 8 SDK**.
3. **Restore Dependencies:** Resolves lightweight packages (like NUnit).
4. **Compile Solution:** Builds the `OverPower.Core.sln` solution in `Release` configuration. If any Unity-dependent type is referenced by mistake, the build immediately fails compilation.
5. **Execute Unit Tests:** Discovers and runs NUnit tests under standard dotnet runner.

---

## Local Developer Workflow

A licensed developer, automated test agent, or external contributor can compile and execute the core test suite on any workstation **without having the Unity Editor or any owned packages installed**.

### Requirements
* **.NET 8 SDK** (or higher) installed on the local system.

### Verification Commands
From the repository root folder, execute the following commands in sequence:

```powershell
# 1. Restore lightweight NuGet packages
dotnet restore OverPower.Core.sln

# 2. Compile the core solution in Release configuration
dotnet build OverPower.Core.sln --configuration Release --no-restore

# 3. Run all pure C# unit tests
dotnet test OverPower.Core.sln --configuration Release --no-build
```

---

## Important Pipeline Limits (What is NOT Verified)

It is critical to understand the distinction between Core CI and the final Unity Build:

> **"A green Core CI does NOT prove that Unity scenes, materials, or assets compile."**

### What the Core CI Proves
* Core Domain models, state transitions, and Application services are 100% syntactically correct and decoupled.
* Standard C# code rules, nullability, and architectural bounds are respected.
* Decoupled Application-level test logic (such as game flow scene load intent and state-machine transitions) behaves as designed.

### What is Left to In-Editor Validation
The following cannot be validated outside Unity and are verified locally before committing:
* Unity scenes (`Bootstrap`, `MainMenu`) and serialized inspector references.
* Shader graph and material visual regressions (e.g., pink materials).
* Third-party commercial script integrations (like HeroEditor4D animations and DOTween visual juice).
* Package Manager dependencies and Addressable Assets compilation.

A full Unity build and Test Runner CI pipeline is intentionally excluded from the mandatory primary workflow to prevent high billing costs, complex license activation cycles, and slow runner execution.
