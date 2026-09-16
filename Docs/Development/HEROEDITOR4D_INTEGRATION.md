# HeroEditor4D Integration & Validation Report

This document records the integration notes, technical spike results, validation state, and discovered limitations for the **HeroEditor4D** character sprite system under **OverPower's** URP 2D rendering pipeline.

---

## Technical Spike Findings

The technical spike for `0.1.3` was conducted to verify that the modular sprites and skeletal animations provided by HeroEditor4D function correctly and efficiently within our target technical stack.

### 1. Rendering Pipeline (URP 2D Compatibility)
* **Status:** Verified.
* **Findings:** 
  * The modular sprites used by HeroEditor4D are standard Unity `SpriteRenderer` components. They are fully compatible with the Unity URP 2D Renderer.
  * **Material & Lighting Setup:** By default, imported assets may use legacy/unlit materials. For OverPower's dynamic lighting setup, the characters must use materials with the `Universal Render Pipeline/2D/Sprite-Lit-Default` or `Universal Render Pipeline/2D/Sprite-Unlit-Default` shaders.
  * **Pink Shader Prevention:** Do not use legacy pipeline shaders (like `Sprites/Default` or custom legacy Lit shaders) as they will render pink in a URP 2D project.

### 2. Character Structure & SortingGroup Behavior
* **Status:** Verified with constraints.
* **Findings:**
  * A HeroEditor4D character consists of a deep hierarchy of individual body parts, equipment, and facial feature GameObjects, each containing its own `SpriteRenderer` (up to 20+ renderers per character).
  * **Sorting Issue:** Without explicit grouping, individual body parts or equipment parts can weave between background/foreground elements, causing visual sorting glitches (e.g., a weapon rendering behind a wall while the character's body renders in front).
  * **Solution:** A `SortingGroup` component **must be added** to the root GameObject of every HeroEditor4D character prefab. This forces Unity to treat the entire character as a single renderable entity for sorting purposes relative to other environment elements, preventing overlapping glitches.

### 3. Animations & Four-Directional Orientation
* **Status:** Verified.
* **Findings:**
  * HeroEditor4D supports modular sprite swaps and bone/skeletal animations for different directions (Front, Back, Side).
  * Since our gameplay is four-directional grid-based or free-movement exploration, we rotate or flip the character representation.
  * Flip-X on the root transform is sufficient for side-to-side transitions (Left vs. Right). Side views use the `Side` sprite layouts, while vertical movements use `Front` (downward) and `Back` (upward) sprite layouts.

### 4. Compatibility with Camera & Render Setup
* **Status:** Verified.
* **Findings:**
  * Pixel-perfect and standard orthographic cameras handle HeroEditor4D sprites seamlessly.
  * No texture filtering anomalies or artifacts were observed when setting sprite textures to Bilinear or Point filter mode (depending on the final resolution scaling strategy).

---

## Discovered Limitations & Architectural Guardrails

### 1. High Draw Call Count per Character
* **Problem:** Since each character is built from 20+ individual `SpriteRenderer` components, a single character can generate up to 20+ draw calls (batches) if they use different materials or are not batched.
* **Mitigation:**
  * Enable **SRP Batching** or **Sprite Atlas** packing where possible. All HeroEditor4D gear/body sprites must be packed into unified Sprite Atlases.
  * Use shared materials across all character parts.

### 2. Animation Sync & Scripting Overhead
* **Problem:** Swapping equipment at runtime requires searching and re-assigning sprites on multiple child components. This is costly if done every frame or too frequently.
* **Mitigation:**
  * State modifications or equipment swaps must occur on a decoupled, event-driven basis (e.g., only when gear is equipped/unequipped).
  * Production adapters must wrap HeroEditor4D's presentation layer, ensuring that the core Domain is entirely unaware of Hippo Games' scripts or data structures.

### 3. No Direct Domain Reference
* **Mandate:** As per `ARCHITECTURE.md` and `GEMINI.md`, the core Domain/Application assemblies **must not** reference any HeroEditor4D scripts or assemblies directly.
* **Mitigation:**
  * All character visual logic must live in a dedicated infrastructure/presentation assembly (e.g., `OverPower.Unity.Integration.HeroEditor4D`).
  * Communication from the game logic to the character visuals will be handled via decoupled interfaces or event buses.

---

## Technical Validation Status

* [x] **Render Compatibility:** Verified under URP 2D Renderer.
* [x] **Idle/Run Animation:** Verified functional.
* [x] **Four-Directional Orientation:** Verified with standard controller.
* [x] **Sorting Group:** Verified as mandatory on character roots.
* [x] **No Domain/Application references:** Verified strictly isolated.
