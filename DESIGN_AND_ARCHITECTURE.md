# Arcane Engine v1.0.0 — architecture

## 1.0 runtime services

`V1GameDirector` owns flow and autosave policy; `BalanceTuning` owns caps and scaling; `SpellBuildValidator` owns pre-execution build analysis; `AttackCoordinator` owns simultaneous enemy pressure; `DungeonNavigation` and `DungeonLayoutValidator` own steering and safe placement; `V1PerformanceBudget` and `RuntimeObjectPool` protect long sessions; `V1Diagnostics` owns recoverable error reporting; and `NarrativeDirector` owns non-blocking story beats. These services communicate through stable IDs and runtime instances rather than scene references.

## Runtime flow

`GameBootstrap` creates `GameWorld`, `RunDirector`, and `DemoUI`. `GameWorld` owns the player, three spell boards, equipment runtime, physical run inventories, generated room geometry, and compiled-spell caches. `RunDirector` owns expedition state, procedural routing, encounter state, reward generation, shops, difficulty contracts, boss rewards, and extraction. `ProfileManager` owns versioned permanent data.

## State boundaries

Permanent profile data:

- Essence and banked Relic Shards
- unlocks and discovery records
- physical Spell Core archive
- permanent forged Relic archive
- extracted Armory items
- prepared spell and modifier selections
- equipment and spell loadouts
- upgrade ranks, records, options, and bindings

Run data:

- three active runtime Core instances
- reserve Run Spell Satchel copies
- physical modifier counts and board ownership
- temporary equipment discoveries
- Drachmas, room blessings, corruption, board expansion, reward rerolls, and route history

On death, run data is discarded after death-safe Essence is awarded. On successful extraction, bankable Core and equipment instances are copied into the permanent profile before run data is rebuilt.

## Spell pipeline

1. A `SpellCoreDefinition` supplies the cast, delivery, element, payload, cost, timing, and presentation foundation.
2. An optional `RelicDefinition` applies one immutable signature.
3. Equipped Unique rules may replace or extend the foundation.
4. `SpellBoard.GetActivePlacements` walks directional connector routes from the Core.
5. `SpellCompiler` applies active modifiers in graph order and emits a `CompiledSpell`.
6. Equipment and permanent statistics are folded into the compiled result.
7. `SpellExecutor` interprets the delivery and behavior fields.
8. Triggered spells compile independently, then receive explicit inherited power and target context.
9. Presentation reads element, movement, collision, payload, Relic, and instability fields from the same compiled result.

No possible spell combination needs its own script or prefab.

## Recursion safety

One manual cast owns one `SpellCastBudget`:

- shared Trigger Energy
- activation count per trigger source
- per-trigger internal cooldown
- generation cap of six
- dynamic entity budget scaled by accessibility effect density

The budget is passed to every descendant. Legal cycles can therefore run for several generations but cannot be infinite.

## Physical modifier ownership

`OwnedModifierCounts` records every copy granted to the run. Available counts are recomputed by subtracting placements across all three boards. This makes sharing impossible by construction while allowing safe removal, undo, redo, layout validation, and Core replacement.

## Procedural dungeon rules

`MegaCatalog` contains authored room templates for all room categories. `RunDirector` selects templates and rewards using:

- controlled floor length
- forced pre-boss recovery and final boss placement
- early Core weighting for empty slots
- modifier, equipment, and Core pity counters
- shop timing protection
- recent-template suppression
- branch diversity
- difficulty and current build state

Geometry is regenerated from each authored template's obstacle pattern, hazard permission, palette, and encounter capacity. The same arena footprint keeps combat readable while room identity and topology change.

## Item model

`ItemDefinition` describes base identity, slot, rarity, authored stats, set identity, granted spell, and Unique rule. `ItemInstance` supplies stable identity, item level, upgrades, rolled affixes, corruption, favorite/lock state, and bank status. Two-handed weapon/offhand legality is enforced by `EquipmentInventory`.

## Save format

Profiles are pretty-printed JSON under `Application.persistentDataPath/ArcaneEngineProfiles`. Saves use a temporary file and previous-version backup. Three independent profiles are supported. Version normalization initializes new fields, and legacy demo PlayerPrefs are imported when a profile is first created.

## Validation and performance

`ContentValidator` checks stable IDs, shapes, connector sides, and Relic source references during catalog initialization. Runtime limits constrain trigger depth, activation counts, projectiles, summons, zones, sounds, and generated entities. The effect-density option lowers budgets before it lowers gameplay information.
