# Localization Foundation & Development Workflow

This document defines the authoritative architecture, naming conventions, and development procedures for the **OverPower** project localization system.

---

## Technical Stack & Decoupling

OverPower utilizes the official **Unity Localization** package integrated with **Addressables** and **TextMeshPro (TMP)**.

To maintain strict architectural decoupling:
* **Pure Domain/Application logic must remain completely unaware of UnityEngine.Localization or presentation types.**
* **Core gameplay logic may refer to localization keys as standard C# `string` values or custom Value Objects, but must never reference `LocalizedString`, `StringTable`, or `LocalizationSettings` directly.**
* The Unity presentation layer is responsible for translating keys to localized strings. Direct static references to `LocalizationSettings` are wrapped inside the focused **`UnityLocaleService`** class to centralize access.

---

## Locale Configurations

The following locales are supported for the MVP baseline:

| Locale Name | Identifier (Code) | Status | Character support |
| :--- | :--- | :--- | :--- |
| **English** | `en` | Default / Fallback | Standard Latin-1 |
| **French** | `fr` | Supported | Full Latin accented characters |

---

## Stable Key Convention

Localization keys are immutable technical identifiers that represent semantic UI areas or content entries, rather than the literal translation text.

### Structure: `<area>.<screen-or-feature>.<element>`

* **`ui`:** For user interface, menus, buttons, and HUD elements.
  * *Example:* `ui.main_menu.play`, `ui.main_menu.settings`, `ui.battle.end_turn`.
* **`unit`:** For character and monster titles or descriptions.
  * *Example:* `unit.guardian.name`, `unit.guardian.description`.
* **`spell`:** For spell or card titles and descriptions.
  * *Example:* `spell.fireball.name`, `spell.fireball.description`.
* **`artifact`:** For items or relics.
  * *Example:* `artifact.divine_shield.name`.

*Rule: Never use the actual displayed English string as the key.*

---

## Table Organization

All localization string tables are organized under `Assets/OverPower/Localization/Tables/`.
The primary String Table Collection is:
* **`UI.Core`**: Handles all core user interface elements, main menu text, and common shared actions.

---

## Development Procedures

### 1. How to Add a New Localized String
1. Open the Unity Editor.
2. Navigate to `Window > Asset Management > Localization Tables`.
3. Select the `UI.Core` table collection.
4. Add a new key following the `<area>.<screen-or-feature>.<element>` convention.
5. Provide the translations for both **English (en)** and **French (fr)**.
6. Press `Save` or let Unity auto-save, and ensure the changes are staged in Git.

### 2. How to Add a New Locale Later
1. Open the Localization Tables window.
2. Click on `Add Locale` and select the desired language/region.
3. Unity will generate a new locale `.asset` file under `Assets/OverPower/Localization/Locales/` and create the corresponding localized tables under `Tables/`.
4. Ensure the new locale is added to the `LocalesProvider` inside the `LocalizationSettings` asset.

### 3. How to Use Localized Strings in UI
* **No Hardcoded Strings in Scripts:** Player-facing text must never be defined as raw C# string literals inside scripts or MonoBehaviours.
* **Scene/UI Localizer:** Attach the **`LocalizeStringEvent`** component to your TextMeshPro GameObject. Bind its `StringReference` property to the target table (e.g., `UI.Core`) and the specific semantic key.
* **Code-based Loading:** If a string must be fetched programmatically (e.g. dynamic event prompts):
  ```csharp
  var localizedString = new LocalizedString { TableReference = "UI.Core", TableEntryReference = "ui.main_menu.play" };
  string translation = localizedString.GetLocalizedString();
  ```

---

## TextMeshPro (TMP) Compatibility & Accents

The default project-required TextMeshPro font resources and settings reside under `Assets/TextMesh Pro/` and **must be fully tracked and committed to Git**. This ensures that scene font references and GUIDs remain reproducible out of the box after cloning.

The primary font used (LiberationSans SDF) contains native glyph support for standard French accents:
```text
é è ê ë à â ç ù û ô î ï É È À Ç
```
Always verify that any future custom font assets generated for the Design System fully support these characters to prevent empty/pink square glitched render boxes.
