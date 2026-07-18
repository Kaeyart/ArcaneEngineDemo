# Arcane Engine 1.6.0 Demo — Visual Overhaul I: Spell Identity

## Implemented

- Added a `SpellVisualDescriptor` compiled from the active `CompiledSpell`. It reads element, delivery, projectile pattern, size, radius, lifetime, homing, arc flight, pierce, bounce, return, orbit, split, chain, delay, persistent zones, status interactions, barrier state, instability, trigger context, and Relic signature.
- Added six element-specific shape languages: expanding Fire, crystalline Frost, branching Lightning, organic Toxic, hollow Void, and geometric Arcane.
- Added delivery-aware cast cues for all ten delivery types without changing cast timing or damage rules.
- Rebuilt player projectile bodies from pooled core, shell, orbital, and trail layers. Projectile collision and steering remain owned by `SpellProjectile`.
- Added real-event impact, critical-hit, expiration, bounce, return, delay, persistent-area, barrier, and triggered-cast accents.
- Added shared runtime pools for primitive bodies and lines. Hard caps in this build are 320 active pooled visuals, 10 spell lights, 48 persistent marks, 64 cached objects per primitive shape, and 96 cached lines.
- Added Low, Medium, and High procedural quality levels. Low reduces geometry and subdivision but keeps projectile cores, boundaries, and telegraphs.
- Added save-safe visual settings migration. Existing profiles inherit their former effect-density value.

## Mechanical boundaries

- Visuals do not calculate damage, steering, collision, radius, duration, triggering, or status rules.
- A visual failing to acquire from the pool removes decoration only; gameplay execution continues.
- The single-camera creation and aiming path is unchanged.

## Verification status

- All C# sources pass a full tree-sitter syntax parse.
- Unity Play Mode, GPU profiling, and final visual tuning still require testing in Unity 6000.5.2f1.
