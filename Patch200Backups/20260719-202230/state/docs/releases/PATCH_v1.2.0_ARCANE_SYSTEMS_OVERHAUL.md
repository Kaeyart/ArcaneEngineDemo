# Arcane Engine — Demo Update 1.2.0

## Arcane Systems Overhaul

Update 1.2.0 is one consolidated, twenty-seven-part systems patch. It turns the 1.1 foundations into a connected runtime: one input authority, one spell behavior model, one bounded combat event stream, one transactional item economy, one deterministic encounter policy, and one versioned persistence path.

## Update highlights

- Exact free mouse aiming now resolves after the camera has finished moving and remains independent from WASD.
- Modifier and equipment drag/drop no longer depend on a single IMGUI event surviving Unity button handling.
- Enemy bars are screen-space views driven from the enemy's current renderer bounds every `LateUpdate`.
- Every compiled spell now exposes a stable, inspectable behavior graph alongside its numeric result.
- Forge Dust, Binding Runes, and Corruption Cores create a real crafting economy separate from shop Gold.
- Combat, triggers, projectiles, hazards, UI feedback, and cosmetics use bounded runtime budgets.
- Profile schema 7 and run-checkpoint schema 6 preserve and migrate existing progress.

## 1. Input authority and gameplay contexts

- Added explicit Front End, Home Base, Combat, Workshop, Inventory, Map, Dialogue, and Pause input contexts.
- Input System mouse samples and root Game-view IMGUI samples feed one de-duplicated pointer authority.
- Refocus protection prevents a stale click or release from firing a spell or closing a panel.
- Modal input is isolated from combat input; opening a menu cannot also cast a spell.

## 2. Absolute mouse aiming and camera order

- The operating-system cursor remains unlocked and visible.
- A camera ray intersects the combat plane once and supplies the authoritative `AimPoint`.
- Camera tracking resolves before player aiming, facing, the ground reticle, and spell release.
- WASD movement and mouse aim remain completely independent at every camera angle.
- Middle Mouse rotation, wheel zoom, and R reset never replace or clamp cursor aim.

## 3. Movement, dodge, and player conditions

- Movement acceleration, collision resolution, camera-relative movement, and facing remain separate concerns.
- Slow, Root, Silence, and Stun are now real player conditions with short readable durations and resistance scaling.
- Dodge invulnerability prevents both damage and on-hit conditions.
- The HUD reports active conditions and dodge readiness without opening another panel.

## 4. Combat contract and event stream

- Added one bounded combat-event stream for manual casts, triggered casts, hits, criticals, statuses, kills, player damage, healing, dodges, rooms, rewards, and crafting.
- Each record stores sequence, frame, time, source, target, amount, position, generation, and detail.
- Listener failures are isolated and written to diagnostics instead of interrupting gameplay.
- The event history is capped at 256 records to prevent long-session growth.

## 5. Damage and hit-registration reliability

- Projectiles continue to collide through swept X/Z segments using visible spell size and enemy hit radius.
- Spell visuals, movement, collision, impact effects, and the mouse target use the same combat plane.
- Critical, elite, boss, weakness, Armor, Shield, resistance, and adaptation modifiers remain ordered and capped.
- Player damage, Shield absorption, healing, leech, status application, and death sources enter the shared runtime record.

## 6. Spell behavior graph

- Every board now compiles into an inspectable graph containing Base Spell, delivery, active modifiers, status stages, triggers, and final output.
- Stable build fingerprints identify identical layouts independently from display names.
- Graph node count and runtime cost are validated against hard limits.
- Disconnected pieces remain installed and visible but never enter the executable graph.
- Graph errors and performance warnings join the existing Workshop validation report.

## 7. Hex Workshop interaction rebuild

- Modifier drags begin before Unity buttons can consume the pointer press.
- Drops use a frame-persistent release recorded outside IMGUI, fixing missed board placement.
- The full rotated polyhex footprint previews in cyan or red with a plain-language legality result.
- Installed modifiers can be moved, rotated, returned to inventory, cancelled, undone, and redone without duplication.
- Click placement, Shift-rotation, and right-click removal remain available as accessibility alternatives.

## 8. Modifier ownership and transaction safety

- Owned copies remain global to the run while installed copies remain exclusive to one board.
- A lightweight runtime audit compares all three boards against owned and available quantities.
- Availability mismatches are repaired deterministically and recorded in diagnostics.
- Interrupted, illegal, or cancelled drops preserve both the original board and inventory count.
- Checkpoints are written only after committed changes or panel closure.

## 9. Triggered spells and termination guarantees

- Triggered casts use the linked spell's complete customized graph.
- Manual and triggered casts are explicitly identified for Unique-item rules and telemetry.
- Trigger Energy, per-rule activation caps, internal cooldowns, total activations, generation depth, and entity budgets all remain enforced.
- Trigger-cycle validation reports loops before play while runtime guards guarantee termination.
- Familiar attacks now spend the same entity budget as every other spawned projectile.

## 10. Spell identity and transformative equipment

- Element, delivery, targeting, movement, projectile, area, zone, summon, ailment, and trigger mutations remain composable.
- Unique rules continue to operate after normal affixes, board modifiers, and Base Spell compilation.
- Build history stores stable ID, element, delivery, modifier count, Trigger count, and estimated damage.
- The Workshop now shows the behavior graph beside execution layers and Trigger details.

## 11. Enemy AI state model

- Enemies retain explicit Spawn, Reposition, Telegraph, Attack, Recover, Stagger, and Dead states.
- Role-specific movement, attack reservation, obstacle steering, local separation, and recovery windows remain active.
- Chasers, Tanks, Ranged enemies, Supports, Disruptors, Assassins, and Controllers keep distinct counterplay.
- Crowd-control conditions, Armor, Shield, weakness, stagger, and elite properties remain visible in combat.

## 12. Role-aware encounter composition

- Encounter selection now runs through one deterministic role-aware planner.
- Early rooms use a smaller readable pool, mid-run rooms introduce disruption and support, and late rooms unlock the complete roster.
- Seed, room depth, and spawn index produce stable encounter compositions.
- Attack coordination continues to cap simultaneous global and per-role pressure.
- Release validation checks every generated archetype and late-game role coverage.

## 13. Elites, minibosses, and boss rules

- Elite affixes remain mechanically active: Frenzied, Shielded, Volatile, Vampiric, Resistant, and Summoner.
- Elite and Miniboss encounters use separate composition and crafting-material pressure.
- The Dungeon Warden retains three phases, Ward Pillars, adaptation, arena hazards, reinforcement rules, and anti-stall escalation.
- Boss rewards remain protected by permanent claim IDs and checkpoint recovery cannot duplicate them.
- Boss rooms guarantee a rare Corruption Core through the room reward policy.

## 14. Deterministic dungeon and route flow

- Room, encounter, objective, route, reward, shop, and item rolls remain derived from stable seeds.
- Standard, Daily, Custom Seed, and Endless use the same protected state machine.
- Physical rewards spawn at the room center; three readable route doors open only after required decisions.
- Entrance, exit, objective, obstacle, and actor placement keep validated separation and fallbacks.
- Route history, room state, and the current checkpoint remain resumable.

## 15. Objectives and reward resolution

- Thirteen objective families retain unique progress, failure pressure, bonus conditions, and reinforcement behavior.
- Rewards are committed through one recorded event before their mechanical effect is applied.
- Reward pity counters continue to protect Spell Modifier, equipment, and Base Spell availability.
- Run Perks, reward choices, doors, shops, Safe Rooms, and extraction cannot overlap illegally.
- Objective bonus experience remains checkpointed and appears in run history.

## 16. Merchant and run economy

- Gold remains run-only shop currency and is still lost when a run ends.
- Merchant stock, specialization, price, discount, sold state, services, and cursed variants remain deterministic.
- Purchases use guarded transactions and restore price and offer state after a recoverable failure.
- Gold Find applies only to earned Gold; transaction refunds never receive a bonus.
- Essence and Legendary Shards retain their separate permanent rules.

## 17. Forge material economy

- Added Forge Dust for common improvement actions.
- Added Binding Runes for affix creation, rarity upgrades, and affix locking.
- Added rare Corruption Cores for irreversible corruption.
- Normal kills can rarely drop Dust; elite enemies and difficult rooms supply stronger material rewards; bosses guarantee a Core.
- All three crafting materials are run-bound, saved in checkpoints, and cleared at run end.

## 18. Transactional Forge crafting

- Rarity upgrade, crafted affix, reroll, improve, remove, lock, quality, and corruption actions now have distinct material costs.
- Every action snapshots the complete item before mutation.
- Illegal results and exceptions restore rarity, affixes, quality, corruption, crafted state, flags, and all spent currencies.
- Successful crafts enter the combat/transaction event stream and save the run checkpoint.
- Corrupted items remain barred from ordinary Forge actions.

## 19. Action-RPG item generation

- The 48+ standard bases, 84 affix families, five tiers, 24+ spell-reactive families, 18 Uniques, and 12 corruptions remain fully generated.
- Common, Magic, Rare, Unique, and Corrupted rules retain prefix/suffix caps and weighted tags.
- Item level gates tiers; slot restrictions, modifier groups, local/global behavior, and duplicate exclusion are validated.
- Advanced tooltips expose tier, legal range, group, tags, source, and local/global behavior.
- Every Unique remains compiled against every Base Spell during release validation.

## 20. Equipment and inventory interaction

- Backpack items drag onto valid equipment slots using the same pointer authority as the Workshop.
- Equipped items drag back over the inventory column to unequip.
- Release handling no longer depends on `EventType.MouseUp` reaching a specific control.
- Invalid slot drops cancel safely and never remove or duplicate an item.
- Click equip/unequip, sorting, search, filters, favorites, locks, junk, selling, comparison, and loadouts remain supported.

## 21. Loot presentation and source rules

- Room, elite, boss, merchant, challenge, secret, and incidental drops retain distinct source pressure.
- Smart-loot weighting favors open slots and prepared build tags without guaranteeing upgrades.
- Ground filters hide rather than destroy equipment; Alt reveals filtered loot; Unique and Corrupted items remain protected.
- Ground-pickup and incidental-loot presentation budgets prevent unlimited label and object growth.
- Currency auto-pickup and physical item interaction remain independently configurable.

## 22. Run progression and build evolution

- Run Level, experience, three-choice Run Perks, temporary power, resources, routes, and decisions persist through checkpoints.
- Players can replace Base Spells, store Spell Copies, equip gear, and edit all spell boards during a run.
- One to three starting Spell slots unlock through permanent progression.
- Prepared modifiers use a budget and cannot be shared across boards without another physical copy.
- Training and boss practice continue to grant no run rewards.

## 23. Permanent progression and collection

- Essence upgrades retain starting-spell, preparation, reroll, archive, Health, Mana, and Power branches.
- Legendary Shards still evolve a physical standard Spell Copy into a selected Legendary Spell.
- Only one crafted Legendary Spell may be prepared; normal and Unique-item spell access remains independent.
- The Armory, Spell Archive, Legendary Archive, layouts, loadouts, discoveries, Bestiary, builds, runs, and transactions persist by stable ID.
- Full upgrade refunds preserve collected content and history.

## 24. Difficulty contracts and balance framework

- Fourteen selectable run modifiers retain independent mechanics and reward multipliers.
- Added a weighted Threat rating with Normal, Hard, Nightmare, and Apocalypse labels.
- High-impact rules such as Glass Soul, new boss phase, adaptive enemies, timers, and empty starts receive additional threat weight.
- Central movement, mitigation, critical, cooldown, projectile, Trigger, recursion, enemy, and reward caps remain authoritative.
- Validation verifies that higher Threat also increases reward pressure.

## 25. HUD, enemy bars, menus, and accessibility

- Replaced the unreliable world-bar path with synchronized screen-space enemy Health/Shield bars.
- Bars use the current renderer top bound after enemy and camera movement and expose values, name, role, status, scale, and height.
- Bosses retain a dedicated top-screen frame.
- Added adjustable drag-start distance and a saved click-to-place alternative.
- Existing UI scale, opacity, safe zone, reduced motion/flashes, contrast, cursor, damage-number, effect-density, off-screen, and audio-accessibility settings remain live.

## 26. Save integrity, recovery, and migration

- Profile schema is now version 7; run-checkpoint schema is now version 6; item schema remains version 11.
- Checkpoints now include Gold, Forge Dust, Binding Runes, Corruption Cores, rooms, routes, spells, boards, modifier ownership, Spell Copies, equipment, inventory, difficulty, perks, claims, shops, and combat statistics.
- Older supported profiles and checkpoints normalize missing fields to safe defaults.
- Atomic temporary writes, SHA-256 validation, rotating profile backups, run backup, quarantine, visible status, and diagnostic export remain enabled.
- Product name and save-folder identity are unchanged.

## 27. Performance, diagnostics, validation, and release tooling

- Added global tokens for player projectiles, triggered casts, enemy projectiles, and hazards.
- Per-cast entity budgets, pooled impact primitives, material caching, damage-number limits, enemy caps, loot caps, and effect-density reductions remain enforced.
- Runtime audits avoid scene-wide searches and sample performance at a bounded frequency.
- `Arcane Engine > Validate 1.2 Demo` validates combat geometry, catalogs, every Base Spell, every Unique combination, behavior graphs, Trigger rules, hex geometry, balance, difficulty, encounters, rewards, 2,000 layouts, and 10,000 generated items.
- Added a Unity 6000.5.2f1 Linux x86_64 build command, v1.2 build script, install guide, release checklist, and validation record.

## Compatibility

- Recommended Editor: Unity 6000.5.2f1.
- Target: Linux x86_64.
- Input: Unity Input System 1.19.0 with event fallback.
- Installation: in place over the existing ArcaneEngineDemo project.
- Existing product name and Linux save location are preserved.
