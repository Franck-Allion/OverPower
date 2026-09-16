# Third-Party Setup & Restoration Procedure

This document defines the authoritative procedure for restoring and setting up third-party commercial and open-source dependencies in the **OverPower** project.

## Commercial Asset Policy

To comply with the Unity Asset Store EULA and prevent unauthorized redistribution of copyrighted commercial assets, **all commercial asset source files and binaries are intentionally excluded from the public Git repository via `.gitignore`**. 

Only `.meta` files for the parent directories or essential configurations may be tracked to maintain reference integrity. **The presence of committed `.meta` files alone does NOT constitute a complete dependency installation.**

When cloning this repository fresh, any developer or automated pipeline must restore these assets locally before the project can compile and run.

---

## Restoration Overview

All dependencies should be restored to their exact designated folders under the `<repo>/OverPower/Assets/` directory.

| Product Name | Local Target Folder (Relative to `OverPower/`) | Git Status | Source / Import Method |
| :--- | :--- | :--- | :--- |
| **HeroEditor4D** | `Assets/HeroEditor4D/` | Ignored | Unity Asset Store / My Assets |
| **DOTween** | `Assets/Plugins/Demigiant/` | Ignored | Unity Asset Store / My Assets |
| **DamageNumbersPro** | `Assets/DamageNumbersPro/` | Ignored | Unity Asset Store / My Assets |
| **AllIn1SpriteShader** | `Assets/Plugins/AllIn1SpriteShader/` | Ignored | Unity Asset Store / My Assets |
| **Hovl Studio AAA Projectiles** | `Assets/Hovl Studio/` | Ignored | Unity Asset Store / My Assets |
| **Cartoon FX Remaster (JMO)** | `Assets/JMO Assets/` | Ignored | Unity Asset Store / My Assets |
| **16 Health bar high quality** | `Assets/HealthBar/` | Ignored | Unity Asset Store / My Assets |

---

## Detailed Setup Instructions

### 1. HeroEditor4D (Character Editor 4D)
- **Target Folder:** `OverPower/Assets/HeroEditor4D/`
- **Import Method:** Open Unity, navigate to `Window > Package Manager`, filter by `Packages: My Assets`, search for `Character Editor 4D` or `HeroEditor4D`, and select **Import**.
- **Post-Import Setup:**
  - If prompted to overwrite or upgrade packages, select "Yes" or "Install/Upgrade".
  - If compiling for the first time under URP 2D, make sure the default materials are using 2D/URP compatible shaders.
- **Verification:**
  - Open `Assets/HeroEditor4D/QuickStart.unity`.
  - Press **Play** in the editor. Verify that the character renders, animates correctly, and can be customized without errors.

### 2. DOTween (Digital Octopus Tween)
- **Target Folder:** `OverPower/Assets/Plugins/Demigiant/`
- **Import Method:** Search `DOTween` in the Package Manager under `My Assets`, and select **Import**.
- **Post-Import Setup:**
  - After import completes, the **DOTween Utility Panel** should pop up automatically. If it does not, open it via the Unity menu: `Tools > Demigiant > DOTween Utility Panel`.
  - Press the **Setup DOTween...** button to run the assembly generator and setup modules.
  - In the **Add / Remove Modules** tab, ensure standard Unity modules (UI, 2D, etc.) are enabled.
- **Verification:**
  - No compiler errors are present in the console.
  - Verify that `DG.Tweening` namespace is accessible in editor scripts.

### 3. DamageNumbersPro
- **Target Folder:** `OverPower/Assets/DamageNumbersPro/`
- **Import Method:** Search `DamageNumbersPro` in Package Manager under `My Assets` and click **Import**.
- **Post-Import Setup:** None required.
- **Verification:**
  - Check that the `DamageNumbersPro.asmdef` exists in `Assets/DamageNumbersPro/` and that the project compiles with no errors.

### 4. AllIn1SpriteShader
- **Target Folder:** `OverPower/Assets/Plugins/AllIn1SpriteShader/`
- **Import Method:** Search `All in 1 Sprite Shader` in Package Manager under `My Assets` and click **Import**.
- **Post-Import Setup:**
  - Under URP, open the shader controller or inspector if a custom material appears pink.
  - If a window asks to generate shader variants, let it complete.
- **Verification:**
  - Check `Assets/Plugins/AllIn1SpriteShader/Demo/DemoScene.unity` to verify that sprites display with special effects/outlines without any pink/broken shaders.

### 5. Hovl Studio (AAA Projectiles Vol 1)
- **Target Folder:** `OverPower/Assets/Hovl Studio/`
- **Import Method:** Search `AAA Projectiles Vol 1` in Package Manager under `My Assets` and click **Import**.
- **Post-Import Setup:**
  - Ensure materials are converted to URP 2D/Lit or standard Sprite-Lit shaders if they use legacy 3D shaders and appear pink.
- **Verification:**
  - Open a projectile prefab under `Assets/Hovl Studio/AAA Projectiles Vol 1/Prefabs/` and verify that the particle systems are fully configured and functional.

### 6. JMO Assets (Cartoon FX Remaster)
- **Target Folder:** `OverPower/Assets/JMO Assets/`
- **Import Method:** Search `Cartoon FX Remaster` in Package Manager under `My Assets` and click **Import**.
- **Post-Import Setup:** None.
- **Verification:**
  - Check the tool `Tools > Cartoon FX Easy Editor` to ensure the menu item is active and launches correctly.

### 7. 16 Health bar high quality
- **Target Folder:** `OverPower/Assets/HealthBar/`
- **Import Method:** Search `16 Health bar high quality` in Package Manager under `My Assets` and click **Import**.
- **Post-Import Setup:** None.
- **Verification:**
  - Check `Assets/HealthBar/HealthBar.unity` scene to verify health bar rendering and shader compatibility under URP 2D.

---

## Known Constraints & Order of Import

While assets are largely independent, it is recommended to import them in the following sequence to minimize compilation rebuild cycles:
1. **DOTween** (Run setup immediately after import)
2. **HeroEditor4D**
3. **DamageNumbersPro**
4. **AllIn1SpriteShader**
5. **Hovl Studio / JMO Assets** (VFX packs)
6. **16 Health bar high quality** (Presentation-only pack)

If compilation fails during import, verify that the package versions align with Unity 6 (URP).
