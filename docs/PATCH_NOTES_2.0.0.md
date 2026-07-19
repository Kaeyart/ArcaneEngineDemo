# Arcane Engine Demo
# Patch 2.0.0 — Elemental Synthesis and Procedural Spell Presentation

**Document type:** implementation and release-scope record  
**Version type:** major systems update  
**Target editor:** Unity 6000.5.2f1  
**Primary installation target:** Linux, including Zorin OS  
**Package format:** standalone installer with timestamped backup and rollback

These notes define what Patch 2.0.0 installs. They do not claim that a Unity build has passed until the project is opened, compiled, validated, and tested in Play Mode. The installer performs structural verification, but it cannot replace Unity compilation or gameplay testing.

## Versioning policy

- `2.0.0` is a major systems release. Major versions are used for architectural or gameplay changes that materially alter how the game is built or played.
- `2.1.0` and later minor versions are reserved for meaningful additions built on the 2.0 foundation, such as new Spell Cores, Rune families, elements, presentation backends, or game modes.
- `2.0.1`, `2.0.2`, and similar maintenance versions are reserved for bug fixes, missing assets, compilation corrections, platform compatibility, documentation corrections, and narrowly scoped balance fixes.

## Patch objective

Patch 2.0.0 establishes one connected runtime pipeline:

```text
SpellForge construction
→ compiled spell mechanics
→ Rune operators
→ elemental buildup
→ major ailments
→ reaction assembly
→ executable reaction graphs
→ environmental residues and field merging
→ deterministic procedural spell presentation
→ procedural reaction presentation
```

The project does not use a prefab-per-combination workflow. Players may create valid SpellForge configurations that were never individually authored or anticipated. The final mechanics and presentation are generated from authored vocabulary, deterministic rules, compiled spell data, Rune structure, element profiles, catalyst roles, reaction graphs, and active quality budgets.

## Standalone consolidation

The package consolidates the required previously developed corrections into one installation path. It preserves or installs:

- Reliable runtime UI pointer routing through panel-space picking and hierarchy traversal.
- Home Base and sanctuary movement without enabling combat-only simulation.
- Direct physical W, A, S, and D input fallback while retaining custom bindings.
- Safe generated-content recreation for invalid or missing authored-content assets.
- Physical Rune dragging into the SpellForge.
- Multi-cell placement ghosts and floating Rune previews.
- Q, E, and mouse-wheel Rune rotation during dragging.
- Moving already-installed Runes.
- Rejection of invalid placements without mutating the board.
- Disabled selection for Runes incompatible with the selected Spell Core.
- Live spell-stat previews before placement.
- Discrete spell casting once per press.
- Continuous input only for channeled spells.
- Charge-and-release behavior for charged spells.
- Removal of the unintended minimum cooldown clamp from non-channeled zero-cooldown spells.

## SpellForge interaction

### Physical Rune manipulation

The SpellForge supports:

- Dragging compatible Runes from the inventory to the board.
- A floating representation of the Rune's actual rotated shape.
- Green valid-placement ghosts.
- Red invalid-placement ghosts.
- A clear invalid-placement explanation.
- Dragging installed Runes to new positions.
- Cancelling a drag without changing the equipped spell.
- Right-click removal and inventory return.
- Rotation with Q, E, or the mouse wheel.
- Pointer capture so a drag remains stable when crossing child UI elements.

### Compatibility enforcement

A Rune that is not compatible with the selected Spell Core is:

- Visually reduced.
- Marked unavailable.
- Disabled.
- Unable to initiate a drag.
- Validated again before placement is committed.

### Spell preview

The placement preview recompiles a temporary board state and reports supported values including:

- Damage.
- Mana cost.
- Cooldown.
- Projectile count.
- Radius.
- Duration.
- Chain count.
- Split count.
- Element signature.
- Ailment application.
- Delivery changes.
- Trigger and persistence behavior where exposed by the compiled spell.

The preview does not permanently change the board until placement succeeds.

# Elemental ailment framework

Patch 2.0.0 installs seven reactive families:

- Fire.
- Cold.
- Lightning.
- Physical.
- Blood.
- Toxic.
- Void.

Arcane remains a neutral spell element unless converted by gameplay data.

Each reactive family has a buildup state, a threshold, a major state, runtime behavior, death behavior, field behavior where appropriate, boss handling, status text, and presentation grammar.

## Fire: Scorch → Ignited

Fire damage and Fire status calls apply Scorch buildup.

Scorch:

- Deals controlled Fire damage over time.
- Accumulates toward Ignition.
- Decays when its duration expires.
- Is displayed at low, medium, near-threshold, and major-state intensity.
- Can propagate through reaction graphs, Fire fields, and Ignited death effects.

Ignited targets:

- Take increased Fire damage over time.
- Leave short-lived burning trail segments while moving.
- Produce periodic Fire activity even when stationary.
- Emit sustained flames, embers, and smoke within the active visual budget.

Killing an Ignited target:

- Creates a Fire burst.
- Damages nearby enemies.
- Applies more Scorch at close range and reduced Scorch farther away.
- Leaves burning ground.
- Preserves source ownership through the reaction runtime.

## Cold: Chill → Frozen

Cold damage and Chill calls apply Chill buildup.

Chill:

- Slows movement through the existing enemy-status path.
- Accumulates toward Freeze.
- Increases visible frost, mist, crystal growth, and fracture density.

Frozen normal enemies:

- Are prevented from moving or attacking for the configured duration.
- Enter a brittle presentation state.
- Support enhanced Shatter interactions.

Bosses and control-resistant enemies receive reduced Freeze duration instead of unrestricted normal-enemy control.

Killing a Frozen target:

- Creates a Cold area burst.
- Applies Chill to nearby enemies.
- Emits procedural ice shards.
- Preserves Physical and Shatter interaction hooks.

## Lightning: Static → Overcharged

Lightning damage and Shock calls apply Static buildup.

Static:

- Builds toward Overcharged.
- Produces increasing sparks and surface arcs.
- Improves the readability of conductive target networks.

Overcharged targets:

- Periodically arc Lightning to nearby enemies.
- Apply reduced Static through secondary arcs.
- Function as temporary conductor nodes.

Killing an Overcharged target:

- Produces a chain discharge.
- Uses jump-based damage and presentation falloff.
- Applies Static to affected targets.

## Physical: Trauma → Broken

Impact, knockback, heavy Physical damage, and compatible effects apply Trauma.

Trauma:

- Builds toward a stagger threshold.
- Communicates posture and armor pressure.
- Uses impact force to determine application strength.

Broken targets:

- Are staggered.
- Receive Physical vulnerability for a limited duration.
- Are more readable through fractures, dust, pressure rings, and displacement cues.
- Recover without allowing indefinite stagger locking.

Killing a Broken target:

- Produces a Physical shockwave.
- Applies Trauma to nearby targets.
- Applies controlled displacement.

## Blood: Wound → Hemorrhaging

Bleed and Blood-tagged effects apply Wound buildup.

Wound:

- Deals damage over time.
- Is more effective while the affected enemy moves.
- Builds toward Hemorrhage.

Hemorrhaging targets:

- Take accelerated Wound damage.
- Display stronger wound and movement-trail presentation.
- Support propagation and execution-oriented reaction mechanics.

Killing a Hemorrhaging target:

- Produces a Bloodburst.
- Applies Wound buildup to nearby enemies.
- Creates Blood residue when required by the active graph.

Blood visuals can be set to Full, Reduced, or Hidden without removing gameplay information.

## Toxic: Poison → Saturated

Toxic damage and Poison calls apply Poison buildup.

Poison:

- Deals longer-duration Toxic damage.
- Builds toward Saturation.
- Propagates through Toxic fields and reaction mechanics.

Saturated targets:

- Emit a controlled Toxic aura.
- Apply reduced Poison buildup to nearby enemies.
- Retain Poison longer.
- Display stronger vapor, spores, bubbles, and contamination.

Killing a Saturated target:

- Produces Toxic Rupture.
- Creates a persistent Toxic cloud.
- Applies Poison on controlled field pulses.

## Void: Corruption → Unstable

Void damage and Curse calls apply Corruption buildup.

Corruption:

- Builds toward spatial instability.
- Supports Void vulnerability and healing-reduction integrations where available.
- Displays inward particles, hollow geometry, compression rings, and spatial-tear motifs.

Unstable targets:

- Periodically pull nearby enemies inward.
- Become stronger nodes for collapse mechanics.
- Use reduced control behavior against bosses.

Killing an Unstable target:

- Pulls nearby enemies inward.
- Produces delayed Void damage.
- Creates a Void rift.
- Applies Corruption through the resulting field.

# Elemental synthesis

## Complete signature space

Every multi-element subset is represented:

| Tier | Elements | Signatures |
|---|---:|---:|
| Fusion | 2 | 21 |
| Compound | 3 | 35 |
| Catastrophe | 4 | 35 |
| Convergence | 5 | 21 |
| Calamity | 6 | 7 |
| Apex | 7 | 1 |
| **Total** |  | **120** |

All 120 signatures have:

- A unique identifier.
- A unique name.
- A catalyst.
- An assembly window.
- A damage scale.
- A radius.
- A duration.
- Pulse and propagation values.
- A unique executable resolution graph.
- A unique executable death graph.
- A procedural visual expression.

## Reaction assembly

When at least two active elements are present, a reaction begins assembling rather than always resolving immediately.

During the assembly window:

- Additional elements may join.
- The signature upgrades in place.
- The visual assembly changes without restarting from zero.
- The catalyst is updated from the most recent valid elemental contribution.
- The final graph resolves when the window closes, a mechanic forces resolution, or the target dies.

Buildup is partially consumed rather than always erased. Reapplying sufficient buildup can recharge a previously resolved signature after its internal cooldown.

## Catalyst expression

The active set determines the reaction identity. The catalyst controls the resolution expression.

The same Fire, Toxic, and Lightning signature can emphasize:

- Outward combustion when Fire is the catalyst.
- Persistent toxic volume when Toxic is the catalyst.
- Rapid chained pulses when Lightning is the catalyst.

This preserves meaningful application order without creating factorially many separate reaction identities.

## Executable mechanic graphs

The runtime executes graph instructions rather than using a single generic burst.

Supported mechanics include:

- Burst.
- Pulse nova.
- Chain arc.
- Pull.
- Push.
- Freeze.
- Vulnerability.
- Buildup wave.
- Persistent field.
- Split fields.
- Delayed echo.
- Orbiting nodes.
- Shard nova.
- Missing-health execution.
- Thermal cycle.
- Rebound.
- Field surge.
- Trail line.
- Stagger.
- Buildup detonation.
- Contagion.
- Compression.

Every graph instruction emits a corresponding presentation event and a real runtime visual sequence. Higher-order reactions retain element contributions rather than becoming only larger explosions.

# Environmental fields

Elemental fields carry signatures and apply gameplay at controlled pulse intervals.

Field mechanics support:

- Persistent damage.
- Periodic buildup.
- Fire trails.
- Toxic clouds.
- Blood residue.
- Cold zones.
- Lightning arcs.
- Physical pressure.
- Void pull.
- Multi-element combinations.

When fields overlap within merge distance:

- Their element masks combine.
- Radius and duration are recalculated within hard limits.
- Damage and buildup are combined with controlled scaling.
- The visual residue changes its color, motion, particles, boundary, and motifs.
- The merged field can represent a higher-order signature.
- Field counts remain capped.

# Procedural spell presentation

## Core requirement

The final spell presentation is generated from:

- Spell Core ID.
- Delivery type.
- Compiled gameplay values.
- Rune operators.
- Rune placement and rotation.
- Board topology.
- Element signature.
- Catalyst.
- Projectile pattern and count.
- Size, speed, radius, and duration.
- Chain, split, bounce, return, and orbit behavior.
- Status, persistence, trigger, Relic, and equipment data.
- Stable seed.
- Recipe schema version.
- Active presentation quality and budget.

The system does not require one complete prefab for the exact configuration.

## Generated visual recipe

`GeneratedVisualRecipe` schema `20000` stores:

- Configuration hash.
- Stable seed.
- Core and delivery identity.
- Visual chassis.
- Full element signature.
- Primary, catalyst, motion, impact, and residue roles.
- Reaction tier and graph identity.
- Primary and secondary colors.
- Size, speed, radius, duration, and projectile count.
- Board-topology summary.
- Ordered Rune visual operators.
- Required layers.
- Decorative layers.
- Fallback layers.

Recipes are cached by deterministic configuration hash and reused between casts.

## Spell Core chassis

Spell Cores provide recognizable structural identity rather than one immutable finished effect.

Supported chassis include:

- Projectile.
- Beam.
- Zone.
- Nova.
- Meteor.
- Melee.
- Familiar.
- Movement.

A chassis controls silhouette, timing, attachment points, motion baseline, and impact structure. Elements and Runes transform that chassis procedurally.

## Rune visual operators

The recipe compiler derives reusable operators from compiled spell behavior:

- Homing.
- Arc.
- Pierce.
- Chain.
- Bounce.
- Return.
- Orbit.
- Accelerate.
- Split.
- Delay.
- Persistent.
- Spread Status.
- Consume Status.
- Barrier.
- Pull.
- Reflect.
- Repeat.
- Summon.
- Trigger.
- Movement.

Operators change structure rather than selecting a whole replacement prefab.

Examples:

- Chain adds conductor nodes, transfer paths, and element-specific target-to-target motion.
- Split adds parent compression, child markers, and fracture presentation.
- Return reverses trail language and adds return-phase cues.
- Orbit adds deterministic orbit nodes whose direction can reflect board rotation bias.
- Delay adds accelerating countdown rings and a delayed burst.
- Persistent converts impact expression into a field residue.
- Pull adds inward streaks and compression shells.
- Consume Status adds inward extraction and buildup detonation presentation.

## Board topology

Where the gameplay compiler exposes meaningful layout, the visual compiler reads:

- Rune coordinates.
- Rune rotation.
- Distance from the Core.
- Dominant direction.
- Approximate symmetry.
- Rotation bias.
- Ordered Rune IDs.

Topology can influence:

- Clockwise or counterclockwise Orbit.
- Directional bias.
- Operator ordering.
- Symmetric or asymmetric marker placement.
- Stable seed and visual identity.

Presentation does not invent gameplay that the board does not support.

## Determinism

The same valid configuration produces the same recognizable visual identity.

The stable hash uses supported values such as:

- Recipe schema.
- Core ID.
- Display name.
- Delivery.
- Element signature.
- Catalyst.
- Relic ID.
- Rune coordinates and rotations.
- Size.
- Projectile count.
- Chain, split, bounce, and pierce values.

Current time, frame count, unstable object IDs, and unseeded random selection are not used to define spell identity. Controlled particle variation remains cosmetic.

# Element visual grammar

The presentation system includes Arcane fallback plus seven reactive profiles.

Each profile defines:

- Primary and secondary colors.
- Smoke or low-opacity color.
- Authority and accent primitive shapes.
- Turbulence.
- Vertical bias.
- Pulse frequency.
- Particle lifetime, speed, and size.
- Motion language.
- Impact language.
- Residue language.
- A non-color shape symbol.

Element identity therefore uses color, shape, motion, timing, impact, and residue behavior.

- Fire: flame motion, embers, smoke, heat pulses, upward turbulence, burning residue.
- Cold: frost, mist, crystals, fracture growth, sharp fragments, locking motion, shatter.
- Lightning: branching, conductor nodes, rapid pulses, surface arcs, staccato timing.
- Physical: dust, debris, pressure rings, impact streaks, material fracture, grounded motion.
- Blood: droplets, ribbons, wounds, pools, organic pulses, movement trails.
- Toxic: vapor, spores, bubbles, contamination, drifting volume, persistent clouds.
- Void: inward particles, hollow geometry, compression rings, collapse motion, spatial tears.

# Multi-element role assignment

Mixed presentations assign:

- Primary silhouette element.
- Catalyst element.
- Motion element.
- Impact element.
- Secondary elements.
- Residue element.

This prevents every element from rendering at maximum intensity simultaneously. The catalyst controls timing and resolution emphasis; the primary element controls silhouette; secondary elements remain identifiable through accents; the residue role controls persistent fields.

# Presentation events and ownership

Gameplay decides what occurred. Presentation decides how it looks and sounds.

The event bus handles:

- Cast start.
- Projectile impact, return, and redirection.
- Status spread and consumption.
- Zone and Familiar creation.
- Movement arrival.
- Barrier and link events.
- Ailment buildup changes.
- Major ailment activation.
- Reaction assembly and upgrades.
- Reaction resolution and death resolution.
- Individual reaction mechanic instructions.
- Field creation, merging, pulses, and expiration.

Presentation objects do not apply damage, choose targets, advance status timers, or resolve deaths.

# Presentation backends

## Existing procedural geometry

The current geometry pipeline remains active for authoritative telegraphs and compatibility. Patch 2.0.0 adds a second pooled procedural layer without removing existing rings, beams, trails, primitives, lights, decals, or accessibility symbols.

## Particle systems

A pooled Particle System backend provides reusable:

- Cast particles.
- Impact particles.
- Trails.
- Status attachments.
- Field volumes.
- Smoke.
- Sparks.
- Mist.
- Debris.
- Shards.
- Residue particles.

Particle requests are reduced or denied only after required telegraphs are preserved.

## Materials

Runtime material libraries create and cache particle and mesh materials using supported shaders with fallbacks:

- Particle unlit shader.
- Legacy additive fallback.
- Sprite fallback.
- URP unlit mesh shader where available.
- Unlit or Standard fallback.

Element colors, alpha, and emission are applied through compatible material properties.

## Geometry and timed objects

The new geometry backend pools and animates:

- Rings.
- Beams.
- Primitives.
- Arc sweeps.
- Fracture lines.
- Motion streaks.
- Orbiting nodes.
- Moving shards.
- Transfer bodies.
- Delayed markers.
- Execute slashes.
- Thermal cycles.

## Environmental residue

Persistent residue visuals include:

- Field boundaries.
- Looping particles.
- Element motifs.
- Merge transitions.
- Pulse rings.
- Lightning bridges.
- Fire, Toxic, and Blood volume cues.
- Cold and Physical fragments.
- Void rings.

## Audio

Patch 2.0.0 adds procedural audio hooks and synthesized runtime cues for:

- Cast.
- Impact.
- Assembly.
- Reaction.
- Death reaction.
- Major ailment.
- Field pulse.

These are functional cues generated from element signatures and event types. They are not represented as final bespoke audio mastering for every reaction.

## Camera feedback

The presentation director adds bounded positional feedback for important events. It respects:

- Camera-feedback intensity.
- Reduced-motion mode.
- Hard maximum displacement.
- Priority and duration.

Camera movement is never required to read a mechanic.

# Enemy status presentation

Enemies can display several element states simultaneously.

The attachment controller tracks:

- Element.
- Normalized buildup.
- Expiration.
- Major-state flag.
- Looping particles.
- A major-state symbol.

Intensity changes as buildup approaches threshold. Major states add a distinct symbol and critical burst. Blood presentation respects the selected visibility mode.

# Quality, budgets, and graceful reduction

## Quality presets

Low:

- Reduced particle-system count.
- Reduced estimated particles.
- Reduced ring segments.
- Reduced residue and attachment limits.
- Existing procedural telegraphs retained.

Medium:

- Moderate particles and residues.
- Element-specific trails and status layers.
- Moderate geometry density.

High:

- Full configured particle density.
- Higher residue and attachment limits.
- More detailed rings and reaction sequences.

## Budgets

The runtime tracks:

- Active particle systems.
- Estimated active particles.
- Denied particle requests.
- Active geometry.
- Active status attachments.
- Active residues.
- Active procedural audio sources.
- Cached recipes.
- Published and processed presentation events.

Reserved and critical events can replace lower-priority decoration when required. Hard limits remain in place.

## Required versus decorative

The following remain required:

- Area boundaries.
- Hazard boundaries.
- Target connections needed to understand Chain or spread.
- Major ailment state.
- Reaction assembly state.
- Critical impact location.

Decorative particles, fragments, smoke density, secondary lights, and secondary accents are reduced first.

# Accessibility

The runtime includes settings for:

- Low, Medium, and High presentation quality.
- Effect density.
- Effect intensity.
- Camera feedback.
- Flash intensity.
- Reduced motion.
- Distortion permission where supported.
- Blood Full, Reduced, or Hidden mode.
- Persistent diagnostics overlay.

Element identities do not rely only on color. Existing accessibility symbols and telegraphs remain available.

Press `F10` in Play Mode to open the Patch 2.0.0 presentation settings.

# Diagnostics and stress testing

Press `F9` in Play Mode for diagnostics showing:

- Published and processed events.
- Critical-event count.
- Cached visual recipes.
- Active and available particle systems.
- Estimated particles and budget.
- Denied particle requests.
- Active geometry.
- Active status attachments.
- Active residues.
- Active procedural audio.
- Last generated recipe.
- Recent presentation-event history.

Press `F8` in Play Mode to run a non-damaging visual stress sequence covering:

- Seven single elements.
- Representative pairs and triples.
- Four-element reactions.
- The seven-element signature.
- Bursts.
- Pulses.
- Chains.
- Pull and push.
- Freeze.
- Fields.
- Split fields.
- Delayed echoes.
- Orbiting nodes.
- Shards.
- Thermal cycles.
- Contagion.
- Compression.

Use `Arcane Engine > 2.0 > Validate Patch 2.0.0` in the Unity Editor to verify:

- 120 reaction definitions.
- 120 mechanic plans.
- 120 unique graph IDs.
- Non-empty resolution and death graphs.
- Arcane plus seven element visual profiles.
- Required runtime files.
- Visual recipe schema 20000.

# Repository organization

Patch 2.0.0 adds:

```text
Assets/ArcaneEngine/Scripts/Presentation/
    PresentationCore.cs
    PresentationAPI.cs
    PresentationDirector.cs
    PresentationParticles.cs
    PresentationGeometry.cs
    PresentationAttachments.cs
    PresentationResidues.cs
    PresentationEffectObjects.cs
    PresentationEffectObjects2.cs
    PresentationAudioCamera.cs
    PresentationOverlays.cs
```

The existing Combat, Spells, Visuals, UI, Core, and Editor systems are adapted rather than replaced wholesale.

# Changed ownership

Deprecated behavior:

- Treating one blended color as a complete mixed-element visual.
- Requiring a whole prefab for an exact SpellForge configuration.
- Allowing decorative VFX to own gameplay truth.
- Unseeded random choices that change spell identity.
- Missing presentation when optional assets are absent.

Not removed:

- Existing Spell Cores.
- Existing Runes.
- Existing deliveries.
- Existing procedural geometry.
- Existing telegraphs.
- Existing accessibility symbols.

# Save and project compatibility

- Existing SpellElement values are not reordered; Physical and Blood are appended when absent.
- Existing SpellForge boards are recompiled.
- Visual recipes are generated and cached at runtime rather than required in save data.
- The installer supports a clean supported project or a project containing earlier Arcane Engine patch experiments.
- Every changed or newly created path is recorded in a timestamped backup.
- A generated rollback script restores the pre-installation state.

# Known risks before Unity verification

- The package performs structural validation but has not been compiled by the user's Unity Editor at package-generation time.
- Runtime source formatting may differ if the local project has unpublished manual edits.
- Transparent particle overdraw may require tuning after hardware profiling.
- Existing direct visual calls remain active alongside the new event-driven layer; duplicated presentation must be checked in Play Mode and reduced only where the existing telegraph is no longer needed.
- Distortion and render-pipeline-specific shader behavior use fallbacks and are not guaranteed on every hardware configuration.
- Extreme reaction recursion is controlled by the gameplay reaction system, field caps, reduced propagation, and visual budgets, but must be stress-tested in the complete game.

# Release verification requirements

Patch 2.0.0 should not be considered production-verified until:

- Unity compiles without errors.
- The Editor validation command passes.
- Movement and runtime UI remain functional.
- The SpellForge drag, preview, rotate, compatibility, move, and remove paths work.
- Instant, channeled, and charged casting work.
- Every single-element buildup and major state can be triggered.
- Single-element death behaviors work.
- Representative pair through Apex reactions resolve.
- Fields merge and expire correctly.
- Visual recipes are deterministic.
- F8, F9, and F10 tools function.
- Run reset and scene reload remove active temporary effects.
- Low quality preserves telegraphs.
- Reduced motion and Blood visibility settings work.
- No missing-script or missing-asset errors are introduced.

# Scope exclusions

Patch 2.0.0 does not claim:

- One handmade prefab for every configuration.
- One handmade VFX package for every reaction.
- Final professional audio mastering for all signatures.
- Cinematic sequences for every high-tier reaction.
- Unlimited particles, fields, chains, or death recursion.
- Hardware certification before profiling.
- Final balance values before Play Mode testing.

Any missing file, compile correction, reference correction, or narrowly scoped defect discovered after installation belongs in `2.0.1` maintenance work, not a new unrelated feature patch.
