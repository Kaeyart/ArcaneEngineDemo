# Arcane Engine 3.1.0-alpha.1 — First Descent

First Descent converts the 3.0 architecture foundation into a playable persistent ARPG vertical slice. It implements the full route from boot and character creation to the Astral Refuge, Tier 0–5 maps, equipment, crafting, Constellations, death recovery, and persistent reload.

## Complete front-end flow

- Dedicated animated title screen with Continue, Start Game, Options, Credits, and Exit.
- Continue appears only when a valid character exists and targets the most recently played character.
- Eight persistent character slots with last-played sorting, class, level, map progress, playtime, location, Ascendancy preview, and save-health state.
- Center-stage selected-character presentation with right-side roster navigation.
- Character rename and recoverable deletion with typed name confirmation.
- Dedicated Mage, Ranger, and Warrior creation flow with class identity, starting Core, resources, defenses, complexity, and three Ascendancy previews per class.
- Name normalization, validation, uniqueness checking, and final creation confirmation.
- Controlled loading transition directly into the Astral Refuge.
- Keyboard, mouse, and controller-oriented navigation paths plus configurable keyboard bindings.

## Playable Astral Refuge

The Refuge is a physical playable hub, not a menu. Runtime-built stations include:

- SpellForge Altar.
- Map Device with permanent Tier 0 recovery.
- Inventory and Stash access.
- Crafting Station.
- Constellation interface.
- Character statistics station.
- Training target with damage, element, critical, reaction, and event telemetry.

A new character enters at Level 0 with one class Core and no equipment, Support Runes, Links, currency, Constellation allocations, or map completion. The opening objective chain directs the player to the SpellForge and Map Device without a campaign or exposition sequence.

## White Map vertical slice

- Twelve connected playable map nodes covering Tiers 0–5.
- Six procedural map layout identities integrated with the existing room generator.
- Twelve map modifiers with difficulty and reward effects.
- Normal, Magic, and Rare map items.
- Basic Completion and map-specific Mastery conditions.
- Guaranteed forward map protection on first completion.
- Permanent free Tier 0 recovery.
- Map consumption, abandonment, death failure, and Refuge return.
- Carried drops and character progress survive map failure; unclaimed ground loot is cleared.
- Forty-tier/eighty-definition data model remains available for later bands, while only Tiers 0–5 are enabled in this release.

## Monsters and bosses

- Three implemented monster families: Ashbound, Mirekin, and Astral Constructs.
- Eighteen explicit variants, six per family, with runtime presentation and variant behaviors.
- Normal, elite, and boss encounters integrated with existing enemy archetypes and combat systems.
- Six White Map guardian identities: Ember Warden, Frostbound Matron, Stormcoil Behemoth, Bone Regent, Mireheart, and Astral Sentinel.
- Guardian phase logic, arena hazards, telegraphs, protected completion cache, and first-completion rewards.

## Persistent itemization

- Eleven equipment slots: main hand, off hand, helmet, body armour, gloves, boots, belt, amulet, two rings, and relic.
- Thirty-six equipment bases.
- Seventy-two affix definitions with tiers, tags, item-level gates, weights, local/global intent, conflicts, and numerical ranges.
- Normal, Magic, and Rare generation.
- One-prefix/one-suffix Magic limits and three-prefix/three-suffix Rare limits.
- Ground loot, pickup, inventory, stash, equip/unequip, lock state, requirements, tooltips, and equipped-item comparison.
- Immediate persistent-stat rebuild after equipment changes.

## Crafting

Five core actions are implemented through the existing persistent currency enum with new player-facing names:

- Flux Shard — reroll Magic modifiers.
- Binding Seal — add a valid Magic modifier.
- Sovereign Ember — upgrade Magic to Rare.
- Astral Needle — add a valid Rare modifier.
- Tempering Prism — reroll values inside existing tiers.

Crafting enforces item class, item level, affix limits, conflict groups, locks, and fractured-modifier protection. Operations are transactional: a failed operation restores the item and refunds its currency.

## Starter Constellations

Six complete Constellations provide at least sixty implemented Stars:

- Pyre Crown.
- Winter’s Veil.
- Storm Circuit.
- Iron Colossus.
- Living Bulwark.
- Weaver’s Hand.

Each contains sequential Small, Medium, Large, and Completion nodes; allocation requirements; point costs; immediate stat application; completion boons; Attunement cost; persistent saving; and reset behavior.

## Progression and persistence

- Level 0 start, persistent experience, levels, Constellation points, Atlas points, highest completed tier, completion, Mastery, deaths, and playtime.
- Persistent inventory, stash, equipment, currencies, maps, Core/Rune/Link discoveries, active Core, Constellations, objectives, and current location.
- Eight-character roster with stable IDs independent of names and slots.
- Schema 31000.
- Atomic writes, two rotating backups, save repair, legacy 3.0 migration, backup restoration, visible status, and recoverable deleted-character storage.
- Deterministic hashing for loot and crafting paths that require stable seeds.

## Interface and settings

- Title, character selection, creation, confirmation, loading, pause, HUD, inventory, stash, equipment comparison, map device, map preview, map crafting, crafting, SpellForge, Constellations, character statistics, training telemetry, death/recovery messaging, objectives, experience, and currency displays.
- Master/music/effects/interface volume, fullscreen, VSync, frame-rate limit, quality, resolution cycling, interface scale, camera shake, flash intensity, damage-number visibility, high-contrast loot labels, hold-to-interact, and keyboard rebinding.

## Existing systems retained

SpellForge, seven elements, ailments, controlled reaction propagation, Spell Links, Rune mechanics, procedural rooms, procedural spell presentation, audio events, reaction diagnostics, and the optional legacy roguelite code remain present.

## Explicitly deferred

Blue/Yellow/Red playable bands, Ascendancy unlocking, Unique/Exceptional/Legendary item content, corruption, advanced endgame crafting, a full loot-filter editor, pinnacle encounters, Fracture Runs, campaign content, multiplayer, final authored art, final animation/audio libraries, and final balance are not claimed by this alpha.

## Validation status

Package structure, checksums, content counts, schema markers, dependency markers, and static source checks are validated. Unity 6000.5.2f1 compilation, target-editor validator execution, and the full Play Mode route remain required before runtime success is claimed.
