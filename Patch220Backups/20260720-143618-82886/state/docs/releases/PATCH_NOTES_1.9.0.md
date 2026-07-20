# Arcane Engine 1.9.0 Demo — Visual Overhaul IV: Lighting, Rewards & Performance

## Implemented

- Added biome lighting profiles using one directional key light, one non-shadowed rim light, biome ambient color, exponential fog, and a biome-matched camera background. The established single isometric gameplay camera remains the only camera created by the game.
- Added configurable Off, Limited, and High dynamic-light budgets. Spell lights use a hard cap of 4 in Limited and 10 in High; higher-priority critical effects may replace a lower-priority active light.
- Added Off, Hard, and Soft shadow options. Decorative point lights never cast shadows.
- Rebuilt reward presentation around actual reward data. Equipment, Support Runes, Spell Cores, currencies, healing, upgrades, Spell Links, and risky rewards now receive different shape symbols and ring structures. Generated equipment rarity controls the number of rarity rings and presentation height.
- Added Low, Medium, and High overall visual quality; independent spell-effect density; environment density; dynamic-light quality; shadow quality; persistent-mark duration; supported-effect distortion toggle; screen shake; hit stop; damage-number density; reduced flashes; colorblind element/rarity symbols; and the diagnostics toggle to both option screens.
- Element and reward categories use geometry as well as color at every quality level. Reduced-flash mode disables short spell lights and retains boundaries, cores, and telegraphs.
- Added an F10 visual diagnostics overlay showing frame time, FPS, current biome and room type, enemy and projectile counts, active and cached pooled visuals, light and decal use, room and scene renderer counts, runtime material count, quality, density, and adaptive decorative scale.
- Added an adaptive safeguard. Sustained frame times above 31 ms reduce new decorative spell visuals in bounded steps to 55% of the configured density; sustained recovery below 21 ms restores them gradually. This never removes gameplay projectiles, enemy attacks, zone boundaries, or telegraphs.
- Added shared-material accounting, bounded visual pools, hard-expiry paths for temporary effects, pooled line/primitive reuse, light-priority replacement, and room-transition cleanup.
- Added editor and runtime validation for all six element grammars, all 11 enemy visual definitions, deterministic two-biome routing, and the Warden Sanctum boss override.
- Diagnostic report exports now include visual-pool, light, decal, material, and adaptive-budget state.

## Actual runtime caps in this implementation

| Resource | Cap |
|---|---:|
| Active pooled spell and impact visuals | 320 before density scaling |
| Cached objects per primitive shape | 64 |
| Cached line objects | 96 |
| Dynamic spell lights | 0 / 4 / 10 by setting |
| Persistent visual marks | 48 |
| Existing authoritative spell entities | 150 through the pre-existing gameplay budget |

These are safety ceilings, not performance promises. The useful settings depend on hardware, resolution, active spell construction, enemy count, and Unity quality configuration.

## Mechanical and accessibility boundaries

- Lighting, fog, reward symbols, damage numbers, hit stop, and visual pools do not determine damage, collision, targeting, drops, rarity, route selection, or save state.
- Reduced settings remove decorative layers before projectile cores, enemy attacks, hazards, zone boundaries, interaction prompts, or health information.
- Distortion is disabled cleanly where the active runtime shader has no supported distortion path; the toggle does not substitute a full-screen post-processing package.
- No guaranteed frame-rate target is claimed until the build is profiled on named hardware.

## Verification status

- Every C# source passes a full tree-sitter syntax parse.
- The four cumulative source checkpoints have been archived separately.
- Unity compilation, Play Mode route testing, GPU/CPU profiling, Linux player build testing, and accessibility review still require execution in Unity 6000.5.2f1 before this patch can be called release-validated.
