# Arcane Engine 1.2.0 — Release Checklist

## Import and automated validation

- Open the existing project in Unity 6000.5.2f1 and allow compilation/package resolution to finish.
- Require zero red Console errors before entering Play Mode.
- Run `Arcane Engine > Validate 1.2 Demo` and require zero failures.
- Confirm the title footer reads `VERSION 1.2.0-DEMO · ARCANE SYSTEMS OVERHAUL`.

## Input, aiming, and modal state

- Move with WASD while sweeping the hardware cursor around the entire Game view.
- Confirm the player, ground reticle, projectile direction, beam, meteor, and targeted effects share the same point.
- Rotate the camera with Middle Mouse and verify the cursor still aims independently from movement.
- Open/close Tab, I, M, F1, and Escape panels repeatedly; require one transition per press and no immediate closing.
- Refocus the Editor Game view and verify the focus click does not cast or close a panel.

## Spell Workshop

- Drag a found modifier from the list to a legal board cell and confirm it installs on release.
- Rotate during a drag with Q, E, and Mouse Wheel; verify the complete footprint updates.
- Attempt overlap, out-of-bounds, illegal connector, and unavailable-copy drops; require readable rejection and no loss.
- Drag an installed modifier to a new cell and back over inventory.
- Verify undo, redo, right-click cancel, click placement, Shift rotation, and right-click removal.
- Put every available copy on one spell and confirm the other two cannot use phantom copies.
- Inspect graph nodes, execution layers, Trigger rules, stable ID, warnings, and runtime cost.

## Combat and performance

- Test projectile, nova, hitscan, beam, meteor, summon, movement, zone, melee, and defensive Base Spells.
- Test homing, weaving, piercing, bounce, return, orbit, split, chain, statuses, triggers, and spell-to-spell casting.
- Verify swept hits register at different visual projectile heights and camera angles.
- Trigger Slow, Root, Silence, and Stun; verify HUD text, resistance scaling, and dodge immunity.
- Create a high-entity triggered build and confirm mechanics terminate without an uncontrolled object explosion.
- Run a 30-minute combat soak and check that frame time, object count, and memory remain stable.

## Enemy bars, AI, and encounters

- Confirm every non-boss enemy gets a bar above its current rendered top bound.
- Change enemy-bar scale, height, always-show, and number settings live.
- Verify Health, Shield, role, elite name, and status update without drift or one-frame lag.
- Verify boss enemies use the dedicated boss frame rather than a normal bar.
- Test Chaser, Tank, Ranged, Support, Disruptor, Assassin, and Controller behavior.
- Confirm early rooms are readable and late rooms introduce the full role pool.
- Test all six elite affixes and all Dungeon Warden phases, Ward Pillars, adaptation, and anti-stall behavior.

## Dungeon, objectives, and rewards

- Complete a room, select one physical center reward with E, and use one of three route doors with E.
- Verify doors remain blocked while rewards or Run Perks are unresolved.
- Test all thirteen objective types and objective bonus experience.
- Test Combat, Elite, Miniboss, Challenge, Shop, Safe, Healing, Event, Secret, Treasure, Boss, and Extraction rooms.
- Restart the same seed and compare routes, encounters, rewards, shops, and item rolls.

## Items, loot, inventory, and Forge

- Generate Common, Magic, Rare, Unique, and Corrupted items across all ten slots.
- Verify prefix/suffix caps, tier gates, modifier groups, tags, implicits, quality, and corruption.
- Drag a backpack item onto its matching slot; attempt an invalid slot; drag equipped gear back to inventory.
- Verify Left/Right Shoulders and Gloves remain independent and two-handed weapons displace Offhand.
- Test search, all six sorts, slot/tag/rarity filters, Alt reveal, favorites, lock, junk, bulk sell, and comparison.
- Earn Forge Dust, Binding Runes, and a Corruption Core and confirm the HUD/checkpoint totals.
- Test all eight Forge actions and verify their displayed costs.
- Force an illegal Forge action and confirm the entire item and all currencies are restored.

## Run and permanent progression

- Start with one spell, unlock a second and third slot, and verify preparation limits.
- Find, store, equip, replace, sell, extract, and permanently archive Spell Copies.
- Evolve a valid Spell Copy with a Legendary Shard and enforce one active crafted Legendary Spell.
- Level during a run, select multiple Run Perks, Save & Quit, Continue, and verify every rank and resource.
- Test Standard, Daily, Custom Seed, and Endless through a complete result flow.
- Verify death loses run items, modifiers, Gold, and Forge materials while keeping secured Essence and Legendary Shards.

## Difficulty, accessibility, saves, and release

- Test every difficulty rule alone and several combined; verify Threat and reward multiplier increase.
- Test HUD/cursor/tooltip/damage/bar scale, safe zone, contrast, reduced motion/flashes, effect density, and drag distance.
- Migrate an existing schema-6 profile and schema-5 checkpoint without losing content.
- Corrupt a copied primary save and verify backup recovery/quarantine behavior.
- Export diagnostics with F10 and verify version, platform, profile, seed, room, save status, and recent events.
- Build Linux x86_64 with `BUILD_LINUX_v1.2.sh`, launch the executable, and repeat the critical aim/drag/save smoke tests.
