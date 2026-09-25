# UI Audio Asset Specification

## Target Directory

Store future UI sound assets under:

```text
OverPower/Assets/OverPower/Audio/UI/
```

Target filenames:

*   `ui_menu_click.wav` (Primary/secondary main menu action buttons: Play, Settings, Exit)
*   `ui_language_click.wav` (Settings language switch toggle: Français / English)
*   `ui_hover.wav` (Hover / focus feedback)
*   `ui_cancel.wav` (Modal dismissal, return, cancel actions)

---

## Technical Specifications

| Property | Value | Notes |
|---|---|---|
| **Format** | PCM WAV | Uncompressed source asset |
| **Channels** | Mono or Stereo | Mono preferred for UI transients |
| **Sample Rate** | 44.1 kHz or 48 kHz | Standard game audio rates |
| **Bit Depth** | 16-bit or 24-bit | Normalized with clean zero-crossings |
| **Hover Duration** | ~40–150 ms | Very subtle, non-intrusive transient |
| **Confirm Duration** | ~80–250 ms | Crisp, tactile, positive engagement |
| **Cancel Duration** | ~80–250 ms | Soft, descending or dampening closure |

---

## Aesthetic & Design Direction

*   **Identity:** Restrained, premium, slightly metallic or mystical, soft gold and ancient fantasy resonance.
*   **Avoid:** Arcade bleeps, 8-bit chiptune sweeps, harsh high-frequency clicks, loud heavy impacts, or repetitive noisy tails.
*   **Tolerance:** The runtime `UIAudioFeedback` component safely handles null or missing clips without throwing exceptions or logging warnings.
