# Arcane Engine 1.0 Demo acceptance checklist

## Import and release validation

- Install over the existing project, open it in Unity 6000.5.2f1, and confirm there are no red compiler errors.
- Confirm the Console reports that combat, catalogs, hex geometry, spells, balance, deterministic seeds, and 2,000 layouts passed.
- Run `Arcane Engine > Validate 1.0 Demo` and then the Linux release build.

## Title, profiles, and complete flow

- Verify Continue is disabled without a checkpoint and loads the correct room when one exists.
- Verify New Expedition requires a second confirmation when it would replace a checkpoint.
- Switch among all three profiles and verify their progression, options, and checkpoints remain independent.
- Complete Title → preparation → rooms → boss → extraction → recap → Home Base.

## Controls, camera, and accessibility

- Aim at every window edge before and after rotating/zooming the camera; crosshair, reticle, facing, and spell direction must agree.
- Test camera-relative and world-relative movement, charged-cast cancellation, alternate charge behavior, dodge invulnerability, and room-entry protection.
- Test HUD/cursor/damage scale, HUD opacity, reduced motion/flashes, contrast modes, visual audio cues, enemy bars, and off-screen indicators.
- Create a binding conflict and verify the options panel identifies it; reset controls.

## Spell system

- Build connected and disconnected boards, rotate pieces, cover illegal cells, undo/redo, save/load a layout, and verify ownership across all three spells.
- Confirm the validation summary, stable ID, estimated DPS, entity count, inactive reasons, and trigger warnings update.
- Create spell-to-spell cycles and verify Trigger Energy, activation, depth, cooldown, and entity limits stop recursion without freezing.
- Test every Base Spell delivery and at least one Legendary Spell with further modifiers.

## Combat, enemies, and performance

- Verify normal, armored, Shielded, Elite, and boss hit registration plus combined damage numbers.
- Observe every enemy role, attack coordination, obstacle steering, stagger, status, healing, adaptation, summoning, and readable telegraph.
- Force many projectiles/triggers/impacts and verify mechanics continue while optional visuals/sounds are reduced.
- Run a 45-minute Endless session and watch for rising allocations, stuck enemies, missing bars, lost inputs, or duplicated rewards.

## Rooms, objectives, map, and deterministic seeds

- Exercise all thirteen objectives and verify progress, failure behavior, bonuses, and recap Objective XP.
- Verify reinforcements display floor warnings before spawning.
- Press M throughout the run and verify path, seed, depth, current/cleared nodes, available routes, categories, and legend.
- Repeat the same custom seed twice and compare room count, layout, encounters, routes, rewards, affixes, and enemy drops.

## Loot, equipment, economy, and progression

- Verify item comparison deltas, all ten slots, two-handed/offhand rules, affixes, corruption, sets, Uniques, filters, favorites, locks, junk, and loadouts.
- Buy every merchant offer/service, Save & Quit, Continue, and verify sold/used state cannot repeat.
- Sell items and Spell Copies; verify Gold and transaction history. Confirm Gold ends with the run.
- Buy upgrades/unlocks, perform a respec, forge a Legendary Spell, and verify currencies and content remain correct after restart.

## Boss and ending

- Break every Ward Pillar, test all three phases, wait for anti-stall escalation, and verify phase cleanup removes old hazards.
- Defeat the boss, force-close after the reward save, Continue, and confirm the Legendary Shard cannot be granted twice.
- Verify boss time, pillars, phases, damage, strongest spell, narrative ending, Unique loot, and extraction result.
- Unlock and enter Dungeon Warden practice; verify it grants no Gold, XP, Essence, Shard, item, or run completion.

## Saves, recovery, and diagnostics

- Save & Quit with modified spells, items, perks, routes, shop state, damage, and resources; verify exact restoration at room entry.
- Corrupt the primary profile copy in a test profile and verify quarantine plus backup recovery.
- Corrupt the primary run snapshot and verify backup recovery or a clear no-valid-checkpoint message.
- Export diagnostics with F10 and from Pause; verify version, platform, profile, seed, room, save status, and recent events.

## Final release

- Launch `ArcaneEngine.x86_64` outside the Editor, complete a run, restart the application, and verify persistence.
- Test 1280×720, 1600×900, 1920×1080, windowed, and fullscreen modes.
- Confirm the title and build report show 1.0.0-demo and the product remains `Arcane Engine - The Relic Forge`.
