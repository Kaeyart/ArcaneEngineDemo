# Arcane Engine 3.0.0-alpha.1 Implementation Manifest

## Runtime files

### `ArpgData30.cs`

Defines the persistent ARPG profile, maps, items, affix rolls, equipment records, currencies, classes, Ascendancies, map bands, passive-node sizes, stat categories, atomic saving, backup loading, and migration repair.

### `ArpgContent30.cs`

Builds the initial content library:

- 3 classes.
- 9 Ascendancies.
- 39 Constellations.
- 390 Constellation nodes.
- 29 item bases.
- 56 tiered affixes.
- 80 standard maps.
- 40 standard tiers.
- 12 map affixes.

All IDs are deterministic strings intended to remain stable across future content patches.

### `ArpgItems30.cs`

Implements persistent item and map generation, rarity rolls, affix weighting, prefix/suffix limits, item descriptions, equipment stat extraction, thirteen currency verbs, map crafting, quality, corruption, and fracture state.

### `ArpgStats30.cs`

Collects class, level, passive, Ascendancy, item implicit, and item affix modifiers. Applies compatible values to the existing `PlayerStats` instance. Also calculates Attunement, map reward multipliers, map sustain, item rarity, currency discovery, and experience bonuses.

### `ArpgLegacyBridge30.cs`

Connects the new persistent mode to the existing SpellForge and equipment runtime. Creates the one-Core starting state, disables legacy starting equipment, restores owned Runes and Link conditions, and grants catalog-backed Core/Rune/Link discoveries.

### `ArpgFoundation30.cs`

Owns the persistent mode and the live map loop. Handles boot, class creation, map entry, map consumption, room construction, enemy spawning, death, abandonment, completion, rewards, experience, level-ups, Atlas completion, Mastery, map sustain, Ascendancy milestones, Constellation allocation, equipment, crafting, and refuge return.

### `ArpgInterface30.cs`

Provides the F7 runtime interface for character progression, Atlas maps, Constellations, equipment, crafting, currencies, and SpellForge discoveries.

## Editor file

### `Patch300Alpha1Validator.cs`

Adds validation, profile-folder access, and isolated ARPG-profile reset commands.

## Existing source modification

### `GameWorld.cs`

The installer adds one idempotent call after the existing equipment-stat build:

```csharp
ArpgStatHooks30.ApplyPersistentStats(Stats);
```

Marker:

```text
ARCANE_PATCH_300A1_PERSISTENT_STATS
```

No other existing source file is modified.

## Runtime save files

Created under `Application.persistentDataPath`:

```text
arcane_arpg_300_profile.json
arcane_arpg_300_profile.backup.json
```

These files are not created by the installer. They are created when the game runs.

## Installer behavior

The installer:

- Requires Patch 2.2.5.
- Requires Unity 6000.5.2f1 project metadata.
- Verifies its own checksum manifest.
- Warns when Unity appears open.
- Creates a process-unique timestamped backup.
- Backs up `GameWorld.cs` and every destination file.
- Applies the stat hook once.
- Copies runtime, Editor, marker, and documentation files.
- Verifies required identifiers and counts.
- Generates an exact rollback script.
- Supports repeat installation without duplicate source hooks.
