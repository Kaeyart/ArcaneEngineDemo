# Arcane Engine 2.0 Demo — Implementation Status

This file is the source-of-truth tracker for the corrective 2.0 demo patch. Its 53 rows preserve commitments 1–52 plus the separate 21A transparency/overdraw requirement from the planned visual-contract completion document.

Status meanings:

- `PLANNED`: not implemented yet.
- `IN PROGRESS`: implementation is being edited.
- `IMPLEMENTED / STATICALLY VALIDATED`: code and content exist and pass the available repository checks; Unity play-mode behavior is not claimed.
- `UNITY VERIFIED`: manually tested in Unity 6000.5.2f1.
- `BLOCKED`: cannot be completed without a named external dependency or decision.

No row may be described as complete in release notes unless it reaches at least `IMPLEMENTED / STATICALLY VALIDATED`. Only the user’s Unity test can promote a row to `UNITY VERIFIED` in this environment.

| # | Commitment | Status | Evidence / remaining check |
|---:|---|---|---|
| 1 | Bounded floor-material system | IMPLEMENTED / STATICALLY VALIDATED | Four finite floor variants per biome; quantized 160-entry shared cache; 30-room runtime test records material peaks. |
| 2 | Diagnostics polling correction | IMPLEMENTED / STATICALLY VALIDATED | Event counters run continuously; renderer/material/particle/light scans execute only while F10 diagnostics are visible. |
| 3 | Unified cleanup ownership | IMPLEMENTED / STATICALLY VALIDATED | Shared transient teardown, idempotent pooled release, owner checks and baseline/post-cleanup stress reports cover all registered families. |
| 4 | Evidence-based budgets | IMPLEMENTED / STATICALLY VALIDATED | Low/Medium/High caps and adaptive scaling are explicit; runtime reports record hardware, resolution, peak/average frame time and resource peaks. Unity measurements remain pending. |
| 5 | Descriptor ownership and recompilation | IMPLEMENTED / STATICALLY VALIDATED | `CompiledSpell` owns a revisioned template rebuilt by board/core/relic/equipment/Spell Link changes; casts clone presentation context only. |
| 6 | Complete descriptor inputs | IMPLEMENTED / STATICALLY VALIDATED | Mechanical presentation inputs, mutations, Legendary signature, link context, priority and cost are mapped. |
| 7 | Element grammar beyond color | IMPLEMENTED / STATICALLY VALIDATED | Six elements have separate shape, motion, trail, impact and status grammar plus optional symbols. |
| 8 | Ten genuinely distinct delivery constructions | IMPLEMENTED / STATICALLY VALIDATED | All ten `SpellDelivery` values have explicit construction paths tied to their real footprint/timing. |
| 9 | Four-phase spell lifecycle | IMPLEMENTED / STATICALLY VALIDATED | Delivery-specific anticipation, manifestation, active/sustain and resolution hooks are present without adding combat delay. |
| 10 | Projectile pattern mapping | IMPLEMENTED / STATICALLY VALIDATED | Standard, Cone, Converge and Ring read the actual projectile arrangement. |
| 11 | Behavior-specific Support Rune responses | IMPLEMENTED / STATICALLY VALIDATED | Actual steering, direction change, chain, split, status, zone and barrier events drive their matching accents. |
| 12 | Triggered cast identity | IMPLEMENTED / STATICALLY VALIDATED | Trigger generation reduces optional layers and adds source/link identity without persistent room lines. |
| 13 | Unique-trigger accents | IMPLEMENTED / STATICALLY VALIDATED | An enum-keyed registry covers every current `UniqueMutation`; item-name text is not authoritative. |
| 14 | Critical-hit presentation | IMPLEMENTED / STATICALLY VALIDATED | Sharp pooled accents, flash safeguards, one combat event and a 0.12-second hit-stop gate are wired. |
| 15 | Complete target-attached statuses | IMPLEMENTED / STATICALLY VALIDATED | Priority-budgeted multi-part Burning, Poison, Shock, Chill, Freeze, Stagger, Armor, Shield and Resistance treatments clean up immediately; Freeze shatters on removal. |
| 16 | Bounded overload levels | IMPLEMENTED / STATICALLY VALIDATED | Four measured instability tiers use capped element-specific irregularity. |
| 17 | Legendary Spell registry | IMPLEMENTED / STATICALLY VALIDATED | All 21 non-None `RelicSignature` values have explicit functional profiles. |
| 18 | Pool coverage | IMPLEMENTED / STATICALLY VALIDATED | Bodies, rings, beams, trails, lights, telegraphs, statuses and death parts use bounded pools; persistent architecture is deterministically destroyed. |
| 19 | Material policy | IMPLEMENTED / STATICALLY VALIDATED | Shared quantized materials are bounded and render-pipeline-aware with validated fallback order; runtime `.material` instancing is absent. |
| 20 | Real Low, Medium and High presets | IMPLEMENTED / STATICALLY VALIDATED | Presets change visual caps, density, trails, decals, environment density, lights and shadows; critical boundaries remain. |
| 21 | Working accessibility settings | IMPLEMENTED / STATICALLY VALIDATED | Reduced Flashes/Motion, high-contrast telegraphs, element/rarity/status symbols and shape-distinct connectors are wired; unsupported distortion remains hidden. |
| 21A | Transparency and overdraw policy | IMPLEMENTED / STATICALLY VALIDATED | The procedural pipeline favors opaque geometry; persistent layers, lights and decals have hard caps. |
| 22 | Archetype silhouette definitions | IMPLEMENTED / STATICALLY VALIDATED | All 11 archetypes have unique modular silhouettes plus attack, target, motion, hit, death and biome metadata. |
| 23 | Procedural animation states | IMPLEMENTED / STATICALLY VALIDATED | Authoritative brain state and velocity drive idle, locomotion, lean, telegraph, attack, recovery, impact, stagger/freeze and death presentation. |
| 24 | Elite affix completion | IMPLEMENTED / STATICALLY VALIDATED | Six affixes have persistent identifiers and real-event responses, including spawned-minion and vampiric tethers. |
| 25 | Attack telegraph audit | IMPLEMENTED / STATICALLY VALIDATED | Authoritative attack delays/footprints create telegraphs and populate an exportable prep/footprint/damage/recovery table with tolerance checks. |
| 26 | Armor, shield and resistance feedback | IMPLEMENTED / STATICALLY VALIDATED | Absorb, break, reduced, adapted and zero-damage responses are visually distinct. |
| 27 | Enemy death sequences | IMPLEMENTED / STATICALLY VALIDATED | Pooled Fire, Frost, Lightning, Toxic, Void, Arcane and neutral resolutions avoid false damage zones. |
| 28 | Health bars and indicators | IMPLEMENTED / STATICALLY VALIDATED | Camera-correct UI Toolkit bars use refreshed assembled bounds, off-screen rejection, Health/Armor/Shield layers, status/affix text and boss phases. |
| 29 | Dungeon Warden completion | IMPLEMENTED / STATICALLY VALIDATED | Dedicated assembly, three silhouette phases, pillars/adds/adaptation, phase transitions, exact telegraphs and coordinated death path are connected. |
| 30 | Four authored procedural kits | IMPLEMENTED / STATICALLY VALIDATED | Ossuary, Foundry, Archive and Cistern have separate architecture, props, floor families, decals, ambient motion and lighting references. |
| 31 | Warden Sanctum as an overlay | IMPLEMENTED / STATICALLY VALIDATED | Boss dressing layers onto the deterministic route biome instead of replacing it. |
| 32 | Authoritative room shell | IMPLEMENTED / STATICALLY VALIDATED | Existing gameplay shell remains authoritative; procedural floor, trim, walls/corners, entrances and background add no collision. |
| 33 | Controlled floor variation | IMPLEMENTED / STATICALLY VALIDATED | Finite variants, deterministic wear/decal choices and low-contrast non-hazard objective guides are used. |
| 34 | Authored prop clusters and exclusion volumes | IMPLEMENTED / STATICALLY VALIDATED | Seeded perimeter kit clusters remain outside reserved center, spawn, reward and door approach volumes; colliders are disabled. |
| 35 | Seventeen specific room-purpose overlays | IMPLEMENTED / STATICALLY VALIDATED | Every `DungeonRoomType` has a dedicated apparatus builder; the capture suite renders all 68 biome/room combinations. |
| 36 | Doors and transitions | IMPLEMENTED / STATICALLY VALIDATED | Biome frames, persistent icons, locked/available/focused/selected states and a no-pause cleanup curtain are wired. |
| 37 | Background depth | IMPLEMENTED / STATICALLY VALIDATED | Non-walkable layers, distant silhouettes, darkness, fog separation and quality-bound ambient motion are noninteractive. |
| 38 | Complete Home Base identity | IMPLEMENTED / STATICALLY VALIDATED | Ten station types have distinct function-readable structures at unchanged interaction positions. |
| 39 | Lighting profile contents | IMPLEMENTED / STATICALLY VALIDATED | Biome profiles include ambient/key/rim/fog, player/enemy/objective/door separation, exposure bounds and shadow policy. |
| 40 | Real dynamic-light priority users | IMPLEMENTED / STATICALLY VALIDATED | Bosses, objectives, Legendary/critical impacts, Elites, doors and environment anchors request a pre-emptive bounded pool. |
| 41 | Camera-safe foreground and shake | IMPLEMENTED / STATICALLY VALIDATED | Foreground occluders hide only when covering the player; shake is position-, priority-, duration- and rate-aware; the single-camera fix is retained. |
| 42 | Equipment world identity | IMPLEMENTED / STATICALLY VALIDATED | Generated slot, rarity, corruption and enum-keyed Unique mutation data build the pickup; exact locked-loadout comparison remains. |
| 43 | Support Rune world identity | IMPLEMENTED / STATICALLY VALIDATED | The actual stored footprint and functional category build the pickup without changing Rune rotation. |
| 44 | Spell Core world identity | IMPLEMENTED / STATICALLY VALIDATED | Actual element and delivery select symbol geometry. |
| 45 | Currency and material identities | IMPLEMENTED / STATICALLY VALIDATED | Gold, Essence, Legendary Shards, Forge Dust, Binding Runes, Corruption Cores and dungeon Support Runes have distinct geometry/motion/ownership wording. |
| 46 | Reward-room sequence | IMPLEMENTED / STATICALLY VALIDATED | Objective cleanup gates reveal; pedestals, focus, selection and immediate alternative deactivation precede doors. |
| 47 | Extraction and defeat presentation | IMPLEMENTED / STATICALLY VALIDATED | Actual generated item instances/names and exact resource totals drive secure/loss geometry and text; save authority is unchanged. |
| 48 | Interaction states | IMPLEMENTED / STATICALLY VALIDATED | Shared Idle/Available/Focused/Unavailable/Selected/Completed language is used; objective markers add Active/Complete/Failed/Optional states. |
| 49 | Diagnostics coverage | IMPLEMENTED / STATICALLY VALIDATED | F10 separately reports every promised category, descriptor revisions and telegraph mismatches; it is disabled by default. |
| 50 | Reproducible stress scenarios | IMPLEMENTED / STATICALLY VALIDATED | Seeded projectiles, Homing/Split, three repeating links, trails/zones, dual-affix Elites, phase-three Warden, 30-room route, eight disk round-trips, Home cycles and Low/High cases record peaks/cleanup. |
| 51 | Visual comparison matrix | IMPLEMENTED / STATICALLY VALIDATED | The automated catalog queues 87 captures spanning all required elements, deliveries, affixes, statuses, 68 room/biome pairs, stations, equipment and quality/accessibility cases. |
| 52 | Save and continuation validation | IMPLEMENTED / STATICALLY VALIDATED | Schema v9 stores a deterministic reconstruction signature; pre-load/post-restore mismatches abort without overwriting the checkpoint; older empty-signature saves migrate. |

## Environment limitation

The project can be parsed and statically inspected here, but the Unity editor/player is not available in this environment. Packaging will therefore use `IMPLEMENTED / STATICALLY VALIDATED` language, and the handoff will include exact Unity verification steps.
