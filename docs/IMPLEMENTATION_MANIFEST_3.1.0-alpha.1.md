# Implementation Manifest — 3.1.0-alpha.1 First Descent

## Runtime source

- `ArpgData30.cs` — schema 31000, profiles, items, maps, currency, repair, migration entry points.
- `ArpgRoster31.cs` — eight-character roster, atomic per-character saves, rotating backups, rename, recoverable deletion, legacy import.
- `ArpgSettings31.cs` — persistent global options, keyboard/controller input bridge, bindings and display controls.
- `ArpgContent30.cs` — classes, Ascendancies, six Constellations, 36 bases, 72 affixes, 80 maps, 12 modifiers, three families, 18 variants, six bosses.
- `ArpgItems30.cs` — deterministic item/map generation, tooltips, comparison, transactional crafting.
- `ArpgStats30.cs` — class/equipment/Constellation aggregation and GameWorld stat hook.
- `ArpgLegacyBridge30.cs` — starting Core and persistent Core/Rune/Link discovery bridge.
- `ArpgFrontend31.cs` — title, roster, creation, confirmation, loading, options, credits and pause flow.
- `ArpgRefuge31.cs` — physical Refuge stations, opening objective interactions, training target and telemetry.
- `ArpgLoot31.cs` — world pickups, labels, protected Guardian Cache and kill drops.
- `ArpgEncounters31.cs` — family/variant behavior, bosses, phases, hazards and telegraphs.
- `ArpgFoundation30.cs` — boot, character activation, Refuge/map state, Tier 0–5 loop, rewards, death, progression and saves.
- `ArpgInterface30.cs` — HUD and all persistent gameplay panels.
- `ArpgDeterminism30.cs` — stable hashing, seed combination and safe indexing.

## Editor and release support

- `Patch310Alpha1Validator.cs` — menu validation and save-folder access.
- `PATCH_3_1_0_ALPHA_1.txt` — installed marker.
- Patch notes, implementation manifest, migration notes and Play Mode checklist.
- One-shot installer with prerequisites, backup, rollback, idempotent copy, checksum verification and static audit.

## Integration boundary

The patch relies on the existing `GameWorld.RecalculateStats` hook introduced by 3.0.0-alpha.1:

```text
ARCANE_PATCH_300A1_PERSISTENT_STATS
ArpgStatHooks30.ApplyPersistentStats(Stats)
```

The installer refuses to proceed when that hook or the 3.0 foundation marker is missing.

## Honest status

This manifest describes included source and data contracts. It is not evidence of target-editor compilation or runtime correctness. Those require Unity 6000.5.2f1 and the version-specific validation checklist.
