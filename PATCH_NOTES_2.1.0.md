# Arcane Engine Demo 2.1 — Source Patch Notes

## What is implemented in source

- Added persistent ScriptableObject authoring types and an editor content builder for spells, Support Runes, items, affixes, Legendary paths, enemies, room definitions, room layouts, audio events, shop services and rewards.
- Added eight room shell constructions and 44 generated authored layout assets: six combat and two objective layouts per biome, plus shared shop/rest layouts and biome miniboss/boss arenas.
- Replaced the primary Workshop, Spell Links, Inventory and Training screens with dedicated UI Toolkit product screens.
- Added Workshop drag/drop, root-level drop resolution, rotation, click placement, placement errors, Undo/Redo, rune removal, spell-level reward assignment, Capacity and compiled-value explanations.
- Added a bounded live spell preview and before/current compiled-stat display. This preview is a UI simulation and still requires Play Mode comparison against real spell execution.
- Expanded Spell Links to On Cast, Hit, Critical Hit, Kill, Expire, Status Applied/Consumed, Projectile Impact/Expire, Dodge, Block, Shield Break, Damage Taken and Periodic events.
- Added source/destination compatibility rejection, link cooldowns, generation bounds, per-event activation bounds and resource-cost enforcement. Cycles are allowed only inside those finite budgets.
- Added three mechanically distinct Legendary signatures for each standard Base Spell in the current catalog, plus catalog validation.
- Added specialized shop stock and authoritative services for recovery, materials, Rune duplication/transformation, Capacity expansion, corruption cleansing, equipment affix reroll, item improvement, rejected-reward recovery, Spell Copy duplication, shielding and route reveal.
- Added hashed profile export/import, duplicate profile, timestamped backup browsing/restoration, typed deletion and atomic-replace fallback behavior.
- Added twin-stick aiming, response/dead-zone/assist settings, vibration, UI navigation fallback and input-glyph-aware HUD instructions.
- Added Immediate, Smooth and Fixed camera modes, obstruction fading and single-camera enforcement.
- Reworked world health-bar pooling and visibility modes; added Health, Armor, Shield, delayed damage, healing, status text/stacks, elite and boss presentation paths.
- Added Bleed, Curse, Weaken and Vulnerable mechanics; added temporary spell suppression, reversible rune disruption/corruption, projectile interception/reflection and encounter role caps.
- Added four named boss identities and eight miniboss families with runtime mechanic components.
- Added configurable training targets and direct/trigger damage, hit, critical, status, proc, Mana, DPS and damage-per-resource analytics.
- Added a data-driven audio event layer and home/exploration/combat/elite/boss/reward state selection. The bundled clips are generated placeholder assets, not final authored production audio.
- Added UI text scaling, tooltip scaling, simplified item descriptions, color-independent status symbols, visual audio cues, motion/flash/hit-stop settings, controller aim assistance and an accessibility preview/reset section.
- Added Grid/List inventory presentation, Stash/Run Bag/Equipment separation, comparison, search/sort/filter, equipping, unequipping, junk/favorite/protect, salvage and dismantle confirmation.
- Added Edit Mode and Play Mode test assemblies, an editor content validator, release scripts, Unity source-control ignores and a requirement ledger.

## Correctly described limitations

- UI Toolkit does not yet own every normal-player flow; legacy `DemoUI` still supplies several run/reward/shop/result paths.
- The audio architecture is data-driven, but the supplied tone assets do not meet the contract's final authored-audio requirement and no final AudioMixer review was performed.
- Status bars show supported state and stacks, but duration radials and a full crowded-bar overlap solver are not complete.
- Controller paths are source-coded but were not tested on physical controller hardware here.
- The 50-room, camera, save-cycle, exploit, performance and full-loop claims are acceptance tests, not claims made by this archive.
- No Unity executable was available in the packaging environment, so compilation and Play Mode success remain unverified.

