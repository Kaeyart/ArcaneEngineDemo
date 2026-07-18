# Arcane Engine — 1.0 Demo: The Relic Forge

This is the complete systems milestone defined by the 39-part 1.0 plan. It upgrades the project from a feature prototype into a structured, replayable demo with a title-to-ending flow, protected progression, deterministic runs, validated buildcraft, professional release tooling, and player-facing quality controls.

## 1. Complete expedition structure

- Added a formal game-flow state machine covering Title, Home Base, preparation, combat, rewards, routes, services, extraction, results, and pause states.
- Standard runs now form a complete loop from starting-spell selection through a final boss, extraction, banking, recap, and narrative conclusion. Daily, custom-seed, and Endless variants use the same protected loop.

## 2. Combat reliability overhaul

- Player and enemy combat now share explicit damageable, targetable, and status-receiver contracts. Damage records its source, damage type, critical state, and impact position.
- Swept planar hits, spawn protection, mitigation caps, dodge invulnerability, obstacle resolution, and enemy separation remove common false hits, misses, and unfair room-entry damage.

## 3. Spell validation and trigger safety

- Every board produces a validation report with active/inactive pieces, exact inactive reasons, errors, warnings, Trigger count, Spell Overload, estimated DPS, resource rate, peak entity estimate, and stable build ID.
- Trigger graphs are cycle-checked. Runtime chains also retain Trigger Energy, per-rule limits, internal cooldowns, total activation limits, recursion depth, and entity budgets.

## 4. Global balance framework

- Centralized armor, resistance, cooldown, projectile, trigger, movement, enemy scaling, and reward caps in `BalanceTuning`.
- Spell compilation enforces final caps after Base Spell, Legendary effect, item mutation, board modifiers, gear, and player statistics have been applied.

## 5. Player movement and input

- Added saved mouse-button bindings, cancel-cast input, hold/tap charge behavior, map input, camera-relative or world-relative movement, dodge readiness, and conflict detection.
- Movement acceleration, obstacle resolution, facing, casting, invulnerability, and room-entry reset behavior are driven from one player controller.

## 6. Camera and aiming

- The software crosshair follows the real unlocked mouse position and the world reticle uses the same ground-plane aim point used by spell execution.
- Camera rotation and sensitivity, zoom, reset, reduced motion, cursor size, cursor contrast, and camera-relative movement are configurable.

## 7. Combat feedback and readability

- Enemy bars remain renderer-synchronized; Elites show names, every enemy shows its combat role and status, bosses use a dedicated top bar, and off-screen danger indicators are available.
- Damage numbers support scale control, rapid-hit combining, critical emphasis, reduced motion, and hard pooling/budget limits. Telegraph and cursor contrast modes are included.

## 8. Modifier presentation and Workshop usability

- The Workshop now exposes stable build IDs, validation status, estimated performance, compilation errors, inactive-piece explanations, execution order, triggers, warnings, undo/redo, search, and saved layouts.
- Directional connector rules, occupied shapes, rotation, ownership, and disconnected modifiers remain visible and mechanically enforced.

## 9. Spell identity and build history

- Saved and completed builds are recorded by stable fingerprint with name, Base Spell, element, delivery, active modifiers, triggers, and estimated DPS.
- Build history deduplicates identical configurations while preserving the most recent name and discovery time.

## 10. Enemy artificial intelligence

- Added explicit Spawning, Repositioning, Telegraphing, Attacking, Recovering, Staggered, and Dead states.
- A shared attack coordinator limits simultaneous pressure by role. Navigation samples alternate paths around obstacles and applies local separation to reduce stacking.

## 11. Enemy readability and Bestiary information

- The Collection now gives every enemy a plain-language role, behavior summary, and counterplay description.
- Combat roles, Shields, Armor, stagger, statuses, elite modifiers, and boss rules are exposed through the world HUD and boss objective presentation.

## 12. Authored encounter direction

- Encounter composition scales by depth while limiting simultaneous Elites. Early rooms use a smaller role pool before advanced combinations appear.
- Objective reinforcements use visible spawn rings and announcements before arriving. Challenge, Miniboss, Elite, and Boss rooms use separate composition rules.

## 13. Final boss reconstruction

- The Dungeon Warden retains three mechanical phases, breakable Ward Pillars, adaptive damage counterplay, phase-specific patterns, arena changes, and difficulty additions.
- Phase transitions clear incompatible bolts and hazards; an anti-stall escalation activates after an overlong phase. Boss time, phases, pillars, damage, and strongest spell enter run telemetry.
- Legendary Shards and boss discovery rewards use permanent claim IDs and cannot be duplicated by checkpoint recovery.

## 14. Procedural dungeon generation

- Room layout, encounter, objective, route, reward, and item-roll seeds are deterministic and separated by stable derived seeds.
- Placement validation reserves entrances, exits, objectives, room boundaries, and minimum separation. Failed random placement uses validated fallback positions.
- Release validation simulates 2,000 layout seeds, and development builds can display obstacle navigation radii.

## 15. Routes and dungeon map

- Added a proper M-key dungeon map with visited path, clear/current state, seed, depth, available branches, room categories, and legend.
- Physical doors show room type, risk, difficulty, and hazards. Route history is checkpointed and recorded; doors remain closed until rewards and Run Perks are resolved.

## 16. Objective refinement

- Thirteen objective types retain distinct rules, progress, consequences, target logic, bonus conditions, reinforcement behavior, and readable HUD descriptions.
- Objective bonus experience is now tracked through checkpoints and displayed in the run recap.

## 17. Run Levels and temporary perks

- Physical three-choice Run Perks remain deterministic by seed/level and are protected from overlapping rewards and doors.
- Perk ranks, XP, temporary power, resource restoration, and all checkpoint data persist through Save & Quit. Training kills grant no run rewards.

## 18. Permanent progression

- The permanent tree supports starting-spell slots, preparation, rerolls, archive tools, Health, Mana, Power, and content unlocks with rank caps and scaling costs.
- Added a safe full refund for upgrade ranks without deleting items, spell storage, unlocks, discoveries, history, or profiles. Permanent transactions enter an audit history.

## 19. Economy and merchants

- Essence, Legendary Shards, and Gold keep separate persistence rules. Shops use deterministic stock, named specializations, discounts, cursed variants, services, and sold-state checkpoints.
- Shop purchases are guarded transactions: failed purchases restore Gold and availability, report recovery, and do not silently consume currency.

## 20. Itemization and equipment comparison

- The ten-slot equipment model retains levels, upgrades, affixes, rarities, corruption, sets, two-handed rules, Uniques, favorites, locking, junk, and loadouts.
- Every inventory item now shows an estimated power delta against the currently equipped item in its slot before selection.

## 21. Inventory and transaction safety

- Equipment and Spell Copy sales, bulk junk sales, shop purchases, unlocks, upgrades, respecs, boss rewards, and Legendary forging are recorded as transactions.
- Purchased offers, used services, claimed rewards, backpack contents, equipment, and Spell Copies are checkpointed to prevent repetition or double spending.

## 22. Unified UI architecture

- UI visibility is coordinated through shared game-flow and modal state rather than independent gameplay checks.
- UI Toolkit owns the title screen, run preparation, combat HUD, enemy bars, crosshair, map, announcements, interaction cards, and accessibility front end; complex Workshop/Home Base panels share the same profile and flow services.

## 23. Combat HUD redesign

- The HUD presents Health, Mana, Shield, dodge state, spells, cooldowns, room, seed, level, XP, Gold, objective, route breadcrumb, interaction, tutorial, boss, enemy, status, damage, and off-screen danger information.
- HUD scale, opacity, cursor scale, damage scale, enemy-bar visibility, visual sound cues, reduced motion, and contrast options update live.

## 24. Title screen, pause flow, and run results

- Added a real title screen with Continue, New Expedition, Home Base, three profiles, options, accessibility, credits, version, save state, and Quit.
- New Expedition requires confirmation before replacing an existing checkpoint. Pause supports resume, checkpoint, diagnostic export, Save & Quit to Home or Title, and clearly labeled abandonment.
- Results include outcome, mode, seed, rooms, level, kills, Essence, objective bonuses, damage, criticals, dodges, strongest spell, final damage source, and boss performance.

## 25. Guided tutorial and contextual learning

- The Apprentice Trial remains an interactive movement, aim, dodge, loot, Workshop, and modified-cast tutorial.
- Added persistent one-time context topics, readable combat hints, dismissible hint storage, and visualized narrative/audio cues.

## 26. Accessibility expansion

- Added independent HUD, tooltip, cursor, and damage-number scale data; HUD opacity; safe-zone data; reduced motion; reduced flashes; contrast modes; off-screen indicators; visual audio cues; mono-audio data; and simplified-description preference.
- Expanded audio categories and saved camera, aim, movement, charge, and visibility preferences without changing difficulty or rewards.

## 27. Profile save reliability

- Profiles use versioned JSON, stable IDs, durable temporary writes, validation reads, SHA-256 checksums, three rotating backups, visible status, and corruption quarantine.
- Three profiles independently store progression, content, items, spells, builds, runs, transactions, feedback, tutorial state, options, and control bindings.

## 28. Run checkpoint integrity

- Checkpoints include mode, seed, room entry, resources, spells, boards, ownership, equipment, backpack, stored copies, route history, perks, shops, services, permanent claims, pity counters, economy, stats, and damage source.
- Continue validates content/version/checksum, falls back to a backup, restarts the current room cleanly, restores resources, and immediately writes a fresh checkpoint.

## 29. Runtime architecture refactor

- Added explicit flow, damage, target, status, balance, validation, deterministic seed, diagnostics, navigation, attack coordination, performance, pooling, narrative, and build-release services.
- Definition data, runtime instances, compiled spells, execution, persistence, and presentation remain separated by stable IDs.

## 30. Performance and long-session stability

- Added category budgets for enemies, player/enemy projectiles, triggered casts, hazards, damage numbers, and cosmetics.
- High-frequency impact fragments now use a reusable object pool. Effect density reduces optional particles and sound concurrency before mechanics are affected.

## 31. Error handling and recovery

- Recoverable failures produce player-facing messages and a timestamped diagnostic ring buffer instead of silently losing progress.
- F10 or the Pause menu exports version, platform, profile, run seed, room, save state, and recent recovery events beside the profile saves.

## 32. Automated test coverage

- Runtime/release validation covers swept combat geometry, every Base Spell compilation, content IDs, hex counts/rotation, stable seeds, build fingerprints, mitigation caps, and 2,000 generated layouts.
- The Unity Editor exposes `Arcane Engine > Validate 1.0 Demo`; command-line builds fail when validation reports an error.

## 33. Manual quality assurance

- Added `TESTING_CHECKLIST_v1.0.0.md` covering clean import, title flow, input, combat, all room loops, buildcraft, economy, boss, persistence, accessibility, soak testing, and Linux release.

## 34. Structured playtesting and balance feedback

- Run histories retain reproducible seeds and performance data for up to 30 runs; build history retains up to 80 configurations.
- The result screen records one-click difficulty, polish, or buildcraft feedback alongside the exact run seed.

## 35. Adaptive audio implementation

- Music now blends between Home, exploration, combat, and boss intensity with smooth transitions and narrative ducking.
- Music, ambience, effects, UI, enemy, and voice categories have separate saved levels. Repeated spell sounds are concurrency-limited and visual audio cues are optional.

## 36. Narrative context and demo conclusion

- Added a concise Relic Forge objective, milestone room lines, Secret/Event context, boss introduction, extraction line, failure line, and dedicated demo-complete message.
- Story discoveries enter the profile Collection without blocking repeat runs.

## 37. Standalone Linux release

- Added an Editor build command and `BUILD_LINUX_v1.0.sh` for Unity 6000.5.2f1 Linux x86_64 builds.
- The pipeline validates first, builds `ArcaneEngine.x86_64`, writes build information and a run script, and supports a custom output path.

## 38. Release management and versioning

- Project and build versions are now 1.0.0 / 1.0.0-demo while preserving the product name and existing Linux save path.
- The source package includes planned notes, final notes, install instructions, acceptance tests, automated validation, and reproducible build commands.

## 39. Final polish, consistency, and 1.0 scope lock

- Player-facing terminology consistently uses Base Spell, Spell Copy, Spell Modifier, Legendary Spell, Legendary Shard, Gold, Essence, Shield, Spell Overload, Home Base, and Run Perk.
- The 1.0 demo scope is locked around one complete systems-rich expedition, one final boss, repeatable modes, persistent mastery, safe saves, and expert-level spell experimentation.

## Compatibility

- Unity Editor: 6000.5.2f1 recommended.
- Target: Linux x86_64.
- Existing profile location is preserved because the product name remains `Arcane Engine - The Relic Forge`.
- This ZIP is installed over the existing project; it does not require another Unity Hub project.
