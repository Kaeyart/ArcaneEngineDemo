# Patch 2.2.4 Unity Validation Checklist

## Compilation

- [ ] Unity `6000.5.2f1` completes C# compilation.
- [ ] No new Console errors or warnings appear from Patch 2.2.4.
- [ ] `Arcane Engine > 2.2.4 > Validate Hotfix` passes.

## Rune connectivity

- [ ] Place a Rune directly beside the Spell Core in all six rotations; it remains active in every rotation.
- [ ] Place a second Rune touching only the first Rune; it remains active.
- [ ] Build a chain of at least six touching Runes; every Rune remains active.
- [ ] Test multi-cell Runes touching through any occupied edge.
- [ ] Separate one Rune by one empty hex; it becomes inactive.
- [ ] Rotate a multi-cell Rune so its footprint changes; connectivity follows physical occupied cells.
- [ ] Capacity, overlap, compatibility, and unlocked-cell restrictions still function.

## Spell Links

- [ ] Every owned Link condition can still be assigned to valid source and destination spells.
- [ ] On Cast, On Hit, On Kill, On Expire, and projectile events activate when their conditions occur.
- [ ] A linked spell still activates when the source originated from Split or another propagation path.
- [ ] Link cycles terminate through event guards, cooldowns, trigger energy, and Link depth.
- [ ] One Link does not activate twice for the same cast-budget event.
- [ ] Link power and cooldown values remain unchanged.

## Lifecycle cleanup

- [ ] Cast projectiles, delayed effects, fields, familiars, and reactions, then change rooms; no spell presentation survives.
- [ ] Repeat through an additive room unload and active-scene change.
- [ ] Create several persistent fields, then die; no field, residue, audio loop, target mark, trail, or particle remains.
- [ ] Start another run after death; no previous-run effect reappears.
- [ ] Use `Arcane Engine > 2.2.4 > Clear Active Spell Effects` during Play Mode; active effects clear immediately.
- [ ] Ordinary room geometry, enemies, loot, UI, camera, and persistent managers are not destroyed by the cleanup.

## Audio and presentation

- [ ] Procedural looping voices stop during cleanup.
- [ ] One-shot voices do not continue across room boundaries.
- [ ] Target-local status marks and deformation reset.
- [ ] Reaction presentation coalescing does not suppress the first effect in a new room.
- [ ] Reaction lineage and diagnostic counters restart cleanly.

## Regression testing

- [ ] Patch 2.2 reaction propagation and field limits remain active.
- [ ] Patch 2.2.3 audio pooling remains functional.
- [ ] SpellForge saves load without migration or data loss.
- [ ] Existing equipped Spell Links remain present.
