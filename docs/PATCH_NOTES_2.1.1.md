# Arcane Engine Demo — Patch 2.1.1

**Release type:** Maintenance hotfix  
**Dependency:** Patch 2.1.0  
**Gameplay balance:** Unchanged

## Fixed

### Authored-content ScriptableObject references

- Moved `SpellCoreDefinition` into `SpellCoreDefinition.cs`.
- Moved `SpellModifierDefinition` into `SpellModifierDefinition.cs`.
- Moved `ItemDefinition` into `ItemDefinition.cs`.
- Removed the legacy duplicate declarations from `SpellDefinitions.cs` and `ItemSystem.cs`.
- Deletes the corrupted generated `V21Content` assets during installation.
- Rebuilds spell, Rune, item and other authored content after Unity recompiles.
- Adds an Editor repair guard that verifies each persistent ScriptableObject type resolves to the correct same-named `MonoScript` before rebuilding.
- Adds `Arcane Engine > 2.1.1 > Repair Authored Content`.
- Adds `Arcane Engine > 2.1.1 > Validate Hotfix`.

This corrects the repeated Console messages in which Unity resolved spell and Rune assets as `HexCoord`, and item assets as `PlayerStats`.

### Pooled Particle System reconfiguration

- Stops and clears a pooled Particle System before changing its duration or random seed.
- Disables automatic random seeds before assigning the deterministic spell seed.
- Prevents `Setting the duration while system is still playing is not supported`.
- Prevents `Setting the random seed while system is still playing is not supported`.

### Velocity-over-lifetime curve modes

- Resets X, Y and Z velocity curves to the same `TwoConstants` mode whenever a pooled particle is reused.
- Resets retained radial and orbital velocity values before applying the next elemental profile.
- Prevents `Particle Velocity curves must all be in the same mode`.

## Changed

- Generated authored content is now considered disposable build output and is rebuilt after the hotfix changes Unity script identities.
- The 2.1.1 installer creates its own backup and rollback script.

## Not changed

- Spell damage, mana costs, cooldowns and status values.
- Rune mechanics or ordering.
- Elemental reaction definitions.
- Visual morphology rules.
- Save-data formats.

## Validation

After Unity finishes compiling and rebuilding content, run:

```text
Arcane Engine > 2.1.1 > Validate Hotfix
```

Expected validation includes nonzero counts for spell cores, Runes and items, with no `HexCoord` or `PlayerStats` ScriptableObject errors during `Resources.LoadAll`.
