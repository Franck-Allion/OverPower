# Asset Inventory

This document maintains an authoritative inventory of all third-party libraries, packages, and assets used in the **OverPower** project. 

Every asset integrated into this project must be logged here to ensure legal compliance, clear attribution, and clean dependency tracking.

## Third-Party Asset Register

| Asset / Library | Publisher | Purpose | Version | Source | License / EULA | Purchase / ownership status | Integration location | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Character Editor 4D / HeroEditor4D** | Real-Tuts / Hippo Games | Modular character sprite generation and animations | `7.7` | Unity Asset Store | Standard Unity Asset Store EULA | Purchased | `OverPower/Assets/HeroEditor4D/` | Main character and unit sprite/animation foundation. |
| **DOTween (Digital Octopus Tween)** | Demigiant (Daniele Giardini) | High-performance programmatic animation/tweening engine | `1.3.030` | Asset Store | Standard Asset Store EULA | Free Edition | `OverPower/Assets/Plugins/Demigiant/` | Used for UI and juice effects. |
| **DamageNumbersPro** | Ekincan Tas | High-performance popping combat numbers and text effects | `4.56` | Unity Asset Store | Standard Unity Asset Store EULA | Purchased | `OverPower/Assets/DamageNumbersPro/` | Handles combat text rendering and visual feedback. |
| **AllIn1SpriteShader** | Seaside Studios | Comprehensive suite of 2D shaders and visual effects | `4.68` | Unity Asset Store | Standard Unity Asset Store EULA | Purchased | `OverPower/Assets/Plugins/AllIn1SpriteShader/` | Sprite-based special effects and outline shaders. |
| **Hovl Studio assets (AAA Projectiles Vol 1)** | Hovl Studio (Vladyslav Horobets) | High-quality projectile particle effects and textures | `Version not exposed by imported asset` | Unity Asset Store | Standard Unity Asset Store EULA | Purchased | `OverPower/Assets/Hovl Studio/` | Spell and combat particles/VFX. |
| **JMO assets (Cartoon FX Remaster)** | Jean Moreno (JMO) | Particle/UI helper tools and visual effect packs (Cartoon FX) | `Version not exposed by imported asset` | Unity Asset Store | Standard Unity Asset Store EULA | Purchased | `OverPower/Assets/JMO Assets/` | General combat VFX and particle optimization utilities. |

---

## Addition Procedure

When introducing a new third-party asset or library to the project:
1.  **Verify Compatibility:** Ensure the license is compatible with a commercial, closed-source game (e.g., MIT, Apache 2.0, standard Asset Store EULA).
2.  **Add to Git LFS:** If the package contains large binaries (FBX, WAV, PSD, etc.), confirm that those extensions are tracked in `.gitattributes`.
3.  **Update this File:** Add a row to the table above with accurate information.
4.  **Update Notices:** If the library requires a mandatory copyright attribution or license text inclusion, add it to [`Docs/ThirdParty/THIRD_PARTY_NOTICES.md`](THIRD_PARTY_NOTICES.md).
