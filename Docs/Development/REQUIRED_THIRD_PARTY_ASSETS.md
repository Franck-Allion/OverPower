# Required third-party assets

## HealthBar package

The purchased **HealthBar** package is required in each developer's local Unity
installation of OverPower.

| Item | Requirement |
|---|---|
| Purpose | Premium Health and Mana resource-orb presentation |
| Required local path | `OverPower/Assets/HealthBar/` |
| Required variant | `bars/bar16/` |
| Resources used | `line.png`, `front.png`, `health16.mat` |

The raw purchased package is intentionally excluded from Git and must not be
redistributed by this repository. Developers must obtain and install their own
licensed copy before opening or building the complete Unity project. The shared
`UIVitalResourceOrb` safely instances `health16.mat` per orb: Health retains its
original crimson liquid, Mana tints only the liquid blue, and the metal frame
remains neutral.
