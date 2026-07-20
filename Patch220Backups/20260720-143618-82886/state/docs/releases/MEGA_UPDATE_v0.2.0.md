# MEGA UPDATE v0.2.0 — The Relic Forge

This build implements the planned transition from an arena spell-engine prototype into a finite dungeon-crawler roguelite RPG demo.

## Added

- Permanent Sanctuary, Spell Archive, Relic Archive, Armory, Relic Forge, Preparation Table, Difficulty Altar, Training Chamber, Codex, upgrade tree, options, and multiple profiles.
- Complete ten-slot starting loadouts with no artificial Heirloom restriction.
- Three active spell positions, permanent one-to-three starting selection progression, empty-slot discovery weighting, and safe Core replacement.
- Physical standard Spell Core copies with archive, run satchel, duplicate, sale, extraction, and evolution uses.
- 21 branching Relic evolutions with one-active-Relic enforcement and separately categorized item-exclusive spells.
- Essence, Relic Shards, and Drachmas with distinct sources, sinks, and persistence.
- Handcrafted procedural Catacomb room sets, finite floors, branches, hazards, shops, events, challenges, safe rooms, minibosses, boss, and extraction.
- Weighted reward trios, build-aware direction, unusual choices, rerolls, and pity rules.
- Temporary physical modifier ownership and Preparation Budget.
- Workshop 2.0 with directional polyhex construction, six connector families, drag-and-drop, rotation, active/inactive states, undo/redo, saved layouts, search, previews, expert inspection, and trigger forecasts.
- Ten delivery families and expanded mechanics including movement, collision, status, spawn, targeting, resource, and recursive trigger layers.
- Ten equipment positions, generated affixes, levels, upgrades, sets, corruption, favorites, locks, loadouts, selling, and authored Uniques.
- Expanded enemies, elites, contract variants, multiphase boss, mechanic-driven visuals, generated layered audio, feedback, and accessibility budgets.
- Versioned JSON saving, migration, backups, stable identifiers, validation, and performance termination safeguards.

## Changed

- Endless waves are now finite expeditions.
- Starting spell count is permanent progression; starting equipment can be complete.
- Modifiers are owned physical run copies and never shared between boards.
- Successfully extracted equipment and Core copies are permanent; unbanked discoveries remain at risk.
- Relic evolution branches change rules rather than supplying simple percentage upgrades.
- Pure random drops are now progression-aware decisions assembled within authored constraints.

## Removed

- One-Heirloom restriction.
- Forced weak veteran starts.
- Arena waves as the primary loop.
- Artificial storage caps.
- Cross-board modifier sharing.
- Unlimited run copies from permanent modifier unlocks.
- Currency conversion and overlapping currency identities.
- Hardcoded scripts for individual spell combinations.
- PlayerPrefs as the primary save format; it remains only as a one-time migration source.
