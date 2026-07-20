# MEGA UPDATE v0.3.0 — Spells Unleashed

This is an in-place update for the existing Arcane Engine v0.2 Unity project.

## Critical fixes

- Mouse aiming now resolves against the combat floor and supports full 360-degree facing.
- WASD movement remains intuitive after camera rotation.
- Projectiles no longer stack visual height offsets into gameplay collision.
- Fast projectiles use swept planar collision and explicit target hit radii.
- Homing, orbiting, arcing, returning, piercing, bouncing, and split projectiles share the corrected hit rules.
- Hitscan attacks use planar width and target hit radii.
- Training targets now show real temporary Health loss and damage numbers.

## Combat presentation

- Added a ground aim marker.
- Added enemy Health bars, elite names and modifiers, Shields, status text, damage numbers, and a full boss bar.
- Added optional always-visible Health bars and damage-number settings.
- Added camera zoom, middle-mouse camera rotation, camera reset, and instant/smooth player turn behavior.

## Saving

- Profile format upgraded to version 3 with stable profile identity and timestamps.
- Profile writes are verified before replacing the live save.
- Added SHA-256 integrity files and three rotating backups.
- Corrupt primary saves attempt backup recovery instead of becoming blank profiles.
- Added Save Now, visible save status, save-folder display, and Open Save Folder.
- Added protected two-step profile reset.
- Added separate run snapshots, Save Run Checkpoint, Save & Quit, and Continue Saved Run.
- Continuing restores spells, board layouts, modifier ownership, inventory, equipment, Gold, resources, difficulty settings, seed, depth, and room. The current room restarts cleanly.

## Clear terminology

- Sanctuary → Home Base.
- Expedition → Run.
- Spell Core → Base Spell or Spell Copy.
- Relic Spell → Legendary Spell.
- Relic Shard → Legendary Shard.
- Drachmas → Gold.
- Ward → Shield.
- Instability → Spell Overload.
- Contracts → Difficulty Modifiers.

## New content

- Added Poison Bolt, Frost Lance, Storm Orb, and Gravity Field.
- Added Double Shot, Large Area, Fast Projectile, Slow and Heavy, Delayed Blast, and Damage Field.
- Added Venom Burst, Frozen Beam, Chain Storm, and Black Hole Legendary upgrades.
- All new spells remain compatible with the existing hex-board and triggered-spell systems.

## Technical

- Added shared planar combat math.
- Added runtime validation for swept collision and every Base Spell.
- Expanded content validation to items, granted spells, rooms, modifiers, Base Spells, and Legendary upgrade sources.
- Retained Unity 6000.5.2f1 and all required built-in modules in `Packages/manifest.json`.
