# Arcane Engine Demo 2.0.0 — Visual Contract Completion

**Release state:** implemented and statically validated  
**Unity target:** 6000.5.2f1  
**Play Mode acceptance:** pending on the target machine

This is a cumulative project update. It completes the concrete visual and presentation work left unfinished or described too broadly in versions 1.6–1.9. The update does not claim final art, final sound, a measured frame-rate guarantee, or Unity verification that has not happened.

## Correctness and performance foundation

- Replaced seed-specific floor material creation with four finite floor variants per biome and a quantized, shared 160-entry material cache.
- Kept scene-wide visual scans out of ordinary gameplay. Renderer, particle and light scans now run only while the F10 diagnostics panel is visible.
- Added one idempotent cleanup path for pooled primitives, rings, beams, trails, lights, decals, telegraphs, status layers and spell attachments.
- Added explicit Low, Medium and High visual budgets. They change active-effect caps, ring detail, trails, decals, environment density, dynamic lights and shadows while preserving mechanical boundaries.
- Added runtime stress reports that record Unity version, resolution, CPU, GPU, memory, peak and average frame time, resource peaks and post-cleanup counts. These reports provide measurement data; they do not invent a performance guarantee.

## Spell visual compiler

- Every compiled spell now owns a revisioned `SpellVisualDescriptor`. Board, Core, Legendary, equipment-mutation and Spell Link changes rebuild it; casts consume the current descriptor.
- The descriptor reads element, delivery, pattern, real projectile count, dimensions, timing, motion behaviors, statuses, barriers, trigger context, instability, Unique mutation, Legendary signature, priority and quality cost.
- Fire, Frost, Lightning, Toxic, Void and Arcane use different geometry, motion, trails, impacts and status language, not only different colors.
- Projectile, Nova, Hitscan, Beam, Meteor, Summon, Movement, Zone, Melee and Defensive spells have separate construction paths matched to their real timing and footprint.
- Spell presentation is divided into anticipation, manifestation, active/sustain and resolution phases without adding artificial combat delay.
- Standard, Cone, Converge and Ring patterns use the real projectile arrangement.
- Homing, Arc Flight, Pierce, Chain, Bounce, Return, Orbit, Split/Fork, acceleration, delayed detonation, persistent trails, damage zones, status spread/consume, barriers and instability respond to authoritative gameplay events.
- Triggered casts keep the destination spell's identity while adding a brief source/link cue. Optional layers are reduced during dense trigger chains.
- Critical hits use a bounded local accent and a gated hit-stop request so multi-hit spells cannot repeatedly freeze the game.
- Overload has four bounded visual tiers.
- All current Legendary Spell signatures and all current Unique equipment mutations have explicit enum-keyed presentation profiles.

## Enemy presentation

- All eleven enemy archetypes have separate modular silhouettes and metadata for attack origin, target point, movement, hit response, death response and biome decoration.
- Enemy animation now follows the authoritative combat state: idle, locomotion, telegraph, attack, recovery, impact, stagger/freeze and death.
- All six Elite affixes have persistent identifiers plus real-event accents. Summoner relationships use the actual spawned minion; Vampiric uses the actual heal event.
- Burning, Poison, Shock, Chill, Freeze, Stagger, Armor, Shield and Resistance use bounded target-attached treatments with immediate cleanup. Freeze has a removal shatter.
- Armor absorption, shield absorption/break, resistance reduction, adaptation and zero-damage hits are distinct.
- Fire, Frost, Lightning, Toxic, Void, Arcane and neutral kills have different pooled death sequences without implying extra damage.
- Health bars continue to use camera-correct UI Toolkit projection and refreshed assembled bounds, with off-screen rejection and Health/Armor/Shield information.
- The Dungeon Warden retains its real three-phase mechanics and now receives phase silhouettes, coordinated telegraphs, adaptation feedback and a dedicated death path.
- Added an exportable telegraph audit table containing preparation time, footprint, damage frame and recovery for every recorded enemy attack.

## Dungeons, rooms and Home Base

- Ossuary Catacombs, Ember Foundry, Sunken Archive and Venom Cistern now have separate procedural architecture, prop language, floor families, decals, ambient motion and lighting references.
- Warden Sanctum is an overlay on the route biome rather than a replacement biome.
- The original gameplay shell and collision remain authoritative. Added dressing has no collision.
- Procedural decoration reserves the reward/combat center, player entry, route-door approaches, room apparatus and every real enemy/Elite/boss spawn. Overlapping peripheral props and decals disable themselves.
- All seventeen room types have a purpose-specific apparatus builder. The comparison suite generates all 68 route-biome/room-purpose combinations.
- Doors have biome frames and persistent locked, available, focused and selected states. Transitions use a short non-pausing curtain with cleanup.
- Background layers remain outside the walkable space and scale with environment quality.
- Home Base contains ten distinct, function-readable station structures while keeping the original station interaction positions.
- Lighting profiles now include ambient, key, rim, fog, player/enemy separation, objective/door priority, exposure bounds and shadow policy.
- Dynamic lights use a bounded priority pool for bosses, objectives, Legendary/critical impacts, Elites, active doors and environment anchors.
- Camera-safe foreground objects hide only while actually covering the player. The single-camera startup fix remains in `GameWorld.CreatePlayerAndCamera()`.

## Rewards, equipment and run results

- Equipment pickups are built from the generated slot, rarity, corruption state and actual Unique mutation.
- Support Rune pickups use the stored hex footprint and functional category. Spell Core pickups use element and delivery.
- Gold, Essence, Legendary Shards, Forge Dust, Binding Runes, Corruption Cores and dungeon Support Runes have separate geometry, motion and ownership wording.
- Room rewards remain gated by objective completion. Picking one reward immediately disables the alternatives before route doors open.
- Extraction and defeat sequences use the actual item instances/names and exact resource totals, including secured versus lost state.
- Shared world interactions use Idle, Available, Focused, Unavailable, Selected and Completed states. Objectives add Active, Complete, Failed and Optional states.

## Accessibility, diagnostics and validation

- Reduced Flashes, Reduced Motion, high-contrast telegraphs, element/rarity/status symbols and shape-distinct spell-grid connectors are wired to live presentation.
- The unsupported distortion option remains hidden instead of presenting a setting with no effect.
- F10 diagnostics separately report spell bodies, primitives, rings, beams, trails, particles, projectiles, zones, telegraphs, statuses, lights, shadows, decals, room/reward objects, material families, live material instances, descriptor revisions and telegraph mismatches.
- Added seeded stress cases for projectile density, Homing/Split, three repeating Spell Links, trails/zones, dual-affix Elites, phase-three Warden presentation, 30 generated rooms, eight save reconstruction disk round-trips, Home/training cycles, Low with Reduced Flashes and High at maximum density.
- Added an automated 87-case screenshot catalog: quality/accessibility combinations, six elements in color and grayscale, ten deliveries, Elite affixes, statuses, 68 biome/room pairs, Home Base stations and equipment identity.
- Run snapshot schema 9 stores a deterministic visual reconstruction signature. Restore aborts on a signature mismatch rather than overwriting the checkpoint; older snapshots without the signature can migrate.

## Validation boundary

Repository validation completed here:

- All C# files parse successfully.
- Every source and content asset has a Unity `.meta` file with a unique GUID.
- The project targets Unity 6000.5.2f1 and bundle version 2.0.0.
- The only runtime material constructor is inside the bounded shared material system.
- The menu command `Arcane Engine > Validate 2.0.0 Demo` is included.

Still required on the target machine:

- Unity compilation with zero Console errors.
- The editor validation command.
- Play Mode acceptance for gameplay, visuals, cleanup, save/continue and stress reports.
- Hardware measurements before making any frame-rate claim.

The exact checks are in `PLAY_MODE_ACCEPTANCE_2.0.0.md`.
