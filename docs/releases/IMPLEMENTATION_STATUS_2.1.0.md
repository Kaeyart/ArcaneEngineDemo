# Arcane Engine Demo 2.1 — Candid Requirement Ledger

Status meanings:

- **Coded:** a meaningful player-facing/runtime path exists in source.
- **Partial:** meaningful work exists, but the written 2.1 contract is not fully satisfied.
- **Verification pending:** only Unity import, Play Mode, hardware, listening, visual review or profiling can decide acceptance.

| Requirement | Source status | What exists | What remains before acceptance |
|---|---|---|---|
| 1. Authored content architecture | Coded; verification pending | Persistent asset types, Resources overlay, editor builder, stable IDs, duplicate-ID validation, 44 layout assets on generation | Validate custom asset authoring in Unity; stronger missing-reference/tag/shape validation; clean-clone authoring test |
| 2. Spell Workshop | Coded; partial | Dedicated UI Toolkit screen; drag/drop, rotation, placement, removal, capacity, locks, Undo/Redo, errors, value summary and animated preview | Gameplay-exact preview for every delivery/modifier; richer per-rune attribution; visual usability acceptance |
| 3. Spell Links | Coded; verification pending | Fourteen events, compatibility gates, cooldown/resource/generation/event budgets, finite cycle policy and UI explanation | Execute every combination in Play Mode; save/restore recursion test coverage |
| 4. Legendary spells | Coded; partial | Exactly three distinct mechanic signatures per eligible current Base Spell and validator | Per-Legendary authored audio and fully unique phase presentation; migration fixture for every retired historical ID |
| 5. Shops/services | Coded; partial | Weighted specializations and authoritative recovery, cleanse, Rune transform/duplicate, affix reroll, item improve, Capacity, reward recovery, Spell Copy, ward and reveal services | Dedicated before/after confirmation modal for every destructive service; comprehensive exploit/save-reload tests |
| 6. Profiles/saves | Coded; verification pending | Atomic replace fallback, hash, backups, quarantine, export/import, duplicate, restore browser and typed delete | Interrupted-write/corruption matrix, fixtures for every retained historical schema, eight real Continue cycles |
| 7. Input/camera | Coded; partial | Keyboard capture/conflict flow, controller navigation fallback, twin-stick aim, response/dead zones/assist/vibration, three camera modes and obstruction handling | Mouse-button/scroll rebinding and full controller rebind UI; physical-controller matrix; boss framing review |
| 8. Combat HUD/health bars | Coded; partial | One-camera anchors, pool, visibility modes, Health/Armor/Shield/delayed/healing states and status/elite/boss labels | Duration radials, exhaustive invulnerability/break presentation and crowded overlap solver; 30/60/120/uncapped capture tests |
| 9. Training/analytics | Coded; partial | Eight target profiles and resettable direct/trigger damage, hits, crits, statuses, procs, Mana, DPS and efficiency | Saved-build-versus-edited automated comparison; summon/item damage attribution; deterministic timing report |
| 10. Audio/music | Partial | Data-driven event assets, pooled sources and state transitions with volume/mono consumers | Current clips are generated placeholders; final authored recordings, mixer groups, dynamic range, output-device recovery, test page and listening/CPU acceptance |
| 11. Accessibility | Coded; partial | HUD/UI/tooltip scale, opacity, safe zone, contrast, symbols, simplified text, visual cues, mono, shake/flash/hit-stop/motion, text scale, toggle charge, controller assist, preview/reset | Consumer tests for every setting, broader tooltip coverage, formal subtitle styling and final reduced-motion review |
| 12. Inventory/equipment | Coded; partial | Dedicated Equipment/Stash/Run Bag views, Grid/List, drag/click equip, run lock, compare, filters, flags, salvage/dismantle confirmation | Drop/Sell and protected bulk actions where designs permit; controller and persistence acceptance matrix |
| 13. Boss/miniboss roster | Coded; partial | Four named boss mechanic modes and eight biome miniboss families | Authored behavior-graph assets, unique reward tables/presentation profiles and deterministic full encounter tests |
| 14. Enemy systems | Coded; partial | Melee/ranged/charge/support foundations plus disruption, suppression, reversible corruption, reflection and zone denial with composition caps | Complete anticipation/active/recovery/damage-shape records for every attack and unavoidable-overlap simulation |
| 15. UI architecture | Partial | Title/options/profiles/HUD plus Workshop/Links/Inventory/Training use UI Toolkit and shared runtime components | Several normal flows still rely on `DemoUI`; full authoritative modal-stack migration remains unfinished |
| 16. Visual contract | Partial | Mechanic-driven procedural spell, enemy, dungeon, reward and biome visual grammars; unsupported distortion disabled | Final art remains excluded; required comparison captures and human review were not produced in this environment |
| 17. Automated verification | Partial | Separate Edit/Play test assemblies, catalog/content validator and initial spell/link/camera/UI/run tests | Contract's complete smoke, collision, shop, save, focus, migration and full-loop suite is not yet comprehensive; tests were not run here |
| 18. Performance | Partial | Pools and bounded presentation systems exist; prior stress tooling remains in source | No profiler captures or hardware results were produced; budgets cannot honestly be marked accepted |
| 19. Repository/release | Coded locally; verification pending | Git ignores/LFS attributes, text serialization, pinned Unity version/manifest, scripts, changelog/status docs and local source tag | Clean-clone import/test/build on a second Unity workspace; remote repository/LFS transfer if desired |
| 20. Release evidence | Partial | This ledger, editor validator and manual checklist are included | Unity compilation/test XML, manual signoff, profiler/capture matrices and supported-hardware evidence must be generated |

## Release decision

This package is a **2.1 source candidate**, not an accepted 2.1 release. It may be promoted only after the Unity-generated evidence in `PLAY_MODE_ACCEPTANCE_2.1.0.md` passes. This wording is intentional: it prevents the code volume from being confused with proof that every promised behavior works.

