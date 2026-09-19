# OverPower — MVP Game Design Contract

## High concept
The player controls a hero tested by the gods. The hero must climb a tower and eventually face a god. The MVP contains **one complete floor** and demonstrates the full roguelite loop.

## MVP loop
```text
Start Run -> Explore Floor -> Collect / Recruit / Purchase -> Guardian Battle
-> Victory OR Hero Death -> End Run Rewards -> Meta-Progression Unlocks -> New Run
```
The player earns meta-progression points when the run ends whether through defeat or floor completion. Exact balancing may evolve.

## MVP content target
Minimum: 3 unit types, 3 spells, 2 artifacts. Artifacts can be unlocked through meta-progression rather than starting unlocked.

## Exploration
- 2D top-down.
- HeroEditor4D/Character4D representation.
- Grid/cell movement.
- Moving one cell consumes one action point.
- Possible content: gold, mana-max resource, health-max resource, spells, units/recruits, apothecaries, barracks.
- Apothecary sells spells for gold.
- Barracks sells units for gold.
- When AP is exhausted or the player ends exploration, guardian battle begins.

## Battle board
Static 2D top-down board. Each side owns 2 rows × 6 columns:
```text
Back : 6 cells
Front: 6 cells
```
Board visuals may have multiple skins later without changing rules.

## Front / Back targeting
For an enemy column: Front first, then Back, then enemy hero if column empty. Future abilities may explicitly override this baseline.

## Unit deployment
Legal-placement rules belong to Domain, not UI-only validation. Deployment should prioritize opposing relevant unopposed enemy columns according to the agreed deployment rule.

## Turn phases
```text
MANA RESET -> DRAW -> BATTLE -> END TURN -> DAMAGE/RESOLUTION -> NEXT PLAYER
```
Draw: 1 card by default, no draw if hand full or deck empty. No fatigue required for MVP unless explicitly added later.

Battle actions may deploy units, cast spells, or use supported unit abilities. Costs may include mana, health and/or gold. Targets may be cells, units, groups or another explicit target set.

## Player and guardian
Guardian follows the same rules as the human player: deck, hand, mana, units, legal deployments, spells/actions. AI gets no rule-breaking privileges. Difficulty changes decision quality.

## Deck and hand
Reference MVP values:
- deck size: 20;
- maximum hand: 4;
- starting hand: 3;
- draw per turn: 1;
- starting hand guarantees at least one unit card if the deck contains one.
All values configurable, not hardcoded into engine logic.

## Unit cards and stacking
A unit card represents a unit type, not an individual unit. All units of that type
recruited during exploration accumulate in the run's unit roster. The deck contains
at most one physical card per unit ContentId. Recruiting more of an already owned
unit type increases the roster quantity without creating another unit card.

When a unit card is deployed in battle, it deploys the full currently owned quantity
of that type as one UnitStack. For example, 9 recruited Guardians plus one
`unit.guardian` card deploy as one Guardian UnitStack with quantity 9.

The recruited quantity is not stored in RuntimeCard or Deck. RuntimeCard identifies
the card and unit type; the run roster owns recruited quantity before deployment;
UnitStack owns combat HP and quantity once deployed. Future first acquisition adds
the unit card once and records the recruited quantity in the roster. Roster state,
acquisition orchestration and deployment are not implemented in this increment.

Spell cards remain independent physical cards: multiple cards may share a spell
ContentId if each has a distinct CardInstanceId. Every physical ID must be unique
within a deck, across both unit and spell cards.

## Unit stacks and HP
One logical stack represents the squad. Example: Guardian 10 HP ×10 = 100 total HP. At 73 HP, display ×8 with current partial member at 3/10 HP. The stack remains one authoritative gameplay entity.

Healing supports three distinct capabilities:
- **Survivor-only healing** (`SurvivorOnly`) restores HP inside currently surviving members only. It cannot resurrect lost members, and has no effect on an empty/dead stack.
- **Revival healing** (`ReviveToInitial`) restores HP and can resurrect lost members up to the stack's initial quantity, but cannot exceed it. It can successfully recover an empty stack.
- **Temporary-overflow healing** (`TemporaryOverflow`) can exceed the initial quantity during combat, raising the combat HP and displayed squad size beyond permanent limits. These extra temporary members do not become permanent roster growth and disappear when combat ends. Damage applied during combat naturally consumes temporary members first, and at combat end, any remaining temporary overflow is cleanly discarded.

## Unit statistics
At least HP, armor, attack and optional abilities. Exact formulas are later balancing concerns but must live in Domain.

## Hero and victory
Battle ends when a hero reaches defeat condition (baseline: hero HP reaches zero).

The authoritative hero runtime state manages:
- **Hero HP:** 0 = dead (IsDead becomes true). Damage cannot reduce HP below 0, and healing is capped at MaxHp. Overkill and negative inputs are rejected.
- **Max HP Upgrade:** Increasing MaxHp immediately restores the same amount to CurrentHp (making max health upgrades instantly beneficial). Negative or zero increases are rejected, and integer overflow is strictly guarded.
- **Mana:** Spending mana is atomic (insufficient resources leave state completely unchanged; no partial spending). Mana restoration caps at MaxMana.
- **Max Mana Upgrade:** Increasing MaxMana immediately restores the same amount to CurrentMana. Negative or zero increases are rejected, and integer overflow is strictly guarded.
- **Gold:** Cannot become negative. Spending gold is atomic, and gains are strictly guarded against integer overflow.
- **XP:** Accumulates experience points safely without leveling behavior for now; leveling and progression rules are deferred.

## XP and progression
At battle end, player and surviving/in-play units gain XP according to detailed rules to be implemented later. Unit XP can improve attack, defense or abilities; player XP can support a skill tree. Architecture must not block this.

## Meta-progression
End run -> calculate reward points -> persist -> unlock screen -> spend on content -> unlocked content available to later runs.

## Randomness
Exploration generation, shuffling and other gameplay randomness must be reproducible through `IRandomService` seed.

## Visual direction
Directional references, not literal copies:
- Hades: polish, visual richness, transitions, atmosphere;
- Magic: The Gathering: card presence and information hierarchy;
- Slay the Spire / Monster Train: immediate comprehension and interaction simplicity.
Target: **premium visual impact + clear information hierarchy + simple interaction**.
