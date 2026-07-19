# Arcane Engine Demo — Patch 2.1.0
## Spell Morphology and Combat Presentation

**Package status:** Release candidate source package  
**Version type:** Feature update built on Patch 2.0.0  
**Target Unity version:** 6000.5.2f1  
**Rendering baseline:** Built-In Render Pipeline  
**External art dependency:** None  

Patch 2.1.0 changes how a SpellForge configuration is presented. A Rune is no longer treated only as a color, icon, or secondary particle attachment. The patch compiles the Core, ordered Rune graph, board positions, Rune rotations, delivery, element roles, catalyst, reaction tier, quality budget, and stable seed into one versioned visual contract. Runtime presentation is generated from that contract.

This package includes executable C# runtime systems, Editor asset generation, handwritten shaders, SpellForge UI integration, diagnostics, automated visual capture, validation tools, installation, backup, and rollback. Unity compilation and Play Mode validation remain required before the build should be called a final release.

---

# Added

## Authoritative Spell Visual Contract

Every compiled spell receives a `SpellVisualContract21` containing:

- Schema version `21000`.
- Stable configuration hash and contract ID.
- Core identity and delivery chassis.
- Lifecycle graph.
- Ordered Rune operator graph.
- Rune source IDs, coordinates, rotations, order, branch, magnitude, count, and duration.
- Element signature and element-role assignment.
- Catalyst, motion, impact, and residue elements.
- Generated body-part specification.
- Hierarchical deterministic seeds.
- Visual personality.
- Required readability rules.
- Estimated visual cost.
- Validation warnings.

The contract is cached by configuration hash. Live positions, timers, and active particle state are not stored in the immutable contract.

## Procedural Spell Morphology Compiler

Added `SpellMorphologyCompiler21` to convert a legal SpellForge build into an executable presentation plan.

The compiler now generates:

- Core, shell, and internal-energy body layers.
- Rune nodes and internal connection paths.
- Split nuclei and fracture seams.
- Orbiting bodies and guide rails.
- Chain conductor nodes.
- Delay timing rings.
- Persistent-field panels.
- Movement guides, trail emitters, and impact anchors.
- Delivery-specific body structures.
- Phase mutations and Rune interaction summaries.
- Cost estimates used by quality and importance reduction.

## Shared Spell Lifecycle

Added shared presentation phases:

- Prime.
- Charge.
- Release.
- Emit.
- Hold.
- Travel.
- Acquire Target.
- Contact.
- Resolve.
- Repeat.
- Persist.
- Return.
- Expire.
- Cancel.

Runes may insert, replace, repeat, branch, redirect, or transform phases. Presentation timing is driven from the same gameplay events and host lifetime rather than a separate combat simulation.

## Ordered and Branching Rune Graph

Rune order is preserved from the SpellForge board.

The compiler distinguishes operator order such as:

- Split followed by Chain.
- Chain followed by Split.
- Delay followed by Chain.
- Chain followed by Delay.
- Pull followed by Split.
- Split followed by Pull.
- Bounce followed by Persistent.
- Persistent followed by Bounce.
- Return combined with Orbit.
- Barrier combined with Reflect.

The graph records sequence, branch, board coordinate, and rotation. Generic graph composition remains available when no compound rule is required.

## Rune Operator Precedence

Added deterministic precedence for conflicts:

1. Delivery transformation.
2. Lifecycle transformation.
3. Structural operators.
4. Movement operators.
5. Targeting operators.
6. Resolution operators.
7. Persistence operators.
8. Element expression.
9. equipment and Relic accents.
10. Decorative modifiers.

Unsupported gameplay combinations remain unsupported rather than receiving misleading visuals.

## Rune Transformation Classes

Rune operators are classified as:

- Modifier.
- Structural.
- Transformative.

Structural Runes receive visible contribution guarantees across at least two lifecycle phases. The compiler validates missing structural contributions and reports them in diagnostics.

## Rune-by-Delivery Coverage

Added context-specific interpretation for Projectile, Beam, Zone, Nova, Meteor, Melee, Familiar, and Movement deliveries.

Current operator coverage includes:

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

Each operator resolves to a native description, a context-specific interpretation, or a documented procedural fallback.

## Core Identity Invariants

Every delivery chassis receives identity rules. Examples include:

- Projectile: traveling body, directional intent, contact resolution.
- Beam: source connection, continuous line, sustained contact.
- Zone: world anchor, visible boundary, repeated pulse.
- Nova: centered origin and radial propagation.
- Meteor: descending mass, impact prediction, heavy resolution.
- Melee: directional sweep and local contact.
- Familiar: autonomous body and owner relationship.
- Movement: departure, route, and arrival.

Diagnostics report contracts that fail to preserve required ancestry.

## Generated Spell Bodies

Added runtime construction of a physical visual hierarchy:

- Central core.
- Shell.
- Internal energy.
- Rune sockets.
- Internal Rune circuits.
- Secondary cores.
- Orbitals.
- Rings and bands.
- Spikes and crystals.
- Fracture seams.
- Motion guides.
- Trail emitters.
- Impact anchors.
- Field panels.

Bodies use generated meshes, generated materials, deterministic placement, Rune topology, element roles, and quality reduction.

## Rune-Specific Morphology

### Split

- Adds internal nuclei during formation.
- Adds visible fracture seams.
- Separates child structures after Emit.
- Preserves parent fingerprint and child seed hierarchy.
- Reduces child scale through visual-mass conservation.

### Chain

- Adds conductor nodes.
- Adds branching attachment structures.
- Charges transfer points before propagation.
- Uses jump-order attenuation.

### Orbit

- Adds orbital bodies and guide rails.
- Uses Rune rotation for orbital direction.
- Rotates surface and secondary structures tangentially.

### Return

- Adds reverse lifecycle state.
- Reverses surface flow and directional presentation.
- Adds a source-oriented return cue.

### Bounce

- Adds contact redirection cues.
- Preserves the body between contacts.
- Attenuates repeated contact emphasis.

### Delay

- Adds Hold phase presentation.
- Adds timing rings and accelerating pulses.
- Increases visible fracture and compression before resolution.

### Persistent

- Adds foldable field panels.
- Transforms body structure into a sustained field.
- Preserves originating glyphs, elements, and pulse rhythm.

### Pull

- Adds inward vectors, compression, and particle flow.
- Provides center-focused camera and audio cues.

### Spread Status

- Adds source-to-target propagation cues.
- Preserves element fingerprint and attenuation.

### Consume Status

- Adds extraction, inward transfer, compression, and release presentation.

### Barrier

- Adds defensive surfaces and durability-oriented body structure.

### Reflect

- Adds directional facets and reversal cues.

## SpellForge Board Topology

Rune position and rotation now influence morphology where the gameplay model provides meaningful structure.

Inputs include:

- Rune coordinate.
- Rune rotation.
- Direction from Core.
- Board distance.
- Sequence order.
- Branch assignment.
- Adjacency and symmetry summaries.

Generated consequences include:

- Rune-node placement.
- Internal circuit layout.
- Orbit direction.
- Split orientation.
- Branch angle.
- Field asymmetry.
- Pulse order.

Board topology does not invent combat behavior unsupported by the compiled spell.

## Hierarchical Stable Seeds

Added deterministic seed domains for:

- Core.
- Elements.
- Rune operators.
- Body.
- Trails.
- Impact.
- Residue.
- Audio.
- Icon.

Small board edits preserve unchanged substructures rather than randomizing the entire spell.

## Spell Surface Simulation

Generated body materials receive runtime state for:

- Charge.
- Pulse position.
- Flow direction.
- Fracture.
- Dissolve.
- Compression.
- Instability.
- Remaining lifecycle.
- Return direction.

Delay increases fracture and pulse frequency. Return reverses flow. Persistent effects unfold panels. Split separates nuclei. Expire dissolves and contracts the body.

## Elemental Material Roles

Mixed spells assign:

- Primary silhouette.
- Catalyst.
- Motion element.
- Impact element.
- Secondary accents.
- Residue element.

Element presentation is not produced by averaging all colors. Materials, motion, surface state, particles, and residue use role-specific inputs.

## Generated Internal VFX Asset Library

Added an Editor generator that creates project assets without third-party downloads.

The generator creates:

- One 32-cell procedural mask atlas.
- Gradient ramp texture rows.
- Twenty procedural mesh families.
- Eight deterministic variants per mesh family.
- Element/body material variants.
- Rune glyph resources.
- Ground-residue resources.
- Generation manifest.

Generated resources are written under:

`Assets/ArcaneEngine/Resources/Patch210/`

Regeneration occurs only when the generator version is stale, a required asset is missing, or validation detects corruption.

## Procedural Texture Masks

The generated atlas includes masks for:

- Soft particle.
- Hard core.
- Hollow ring.
- Starburst.
- Directional streak.
- Flame tongue.
- Smoke.
- Cellular smoke.
- Fracture.
- Crystal.
- Lightning branch.
- Droplet.
- Toxic bubble.
- Void aperture.
- Rune glyph.
- Spark flare.
- Ground crack.
- Dissolve noise.
- Radial and angular gradients.
- Additional seeded shape variants.

## Procedural Mesh Library

Generated mesh families include:

- Energy core.
- Low-poly sphere.
- Distorted core.
- Crystal.
- Shard.
- Lightning wedge.
- Crescent.
- Ribbon.
- Torus.
- Broken ring.
- Shockwave cone.
- Ground disc.
- Fracture mesh.
- Spiral.
- Hollow shell.
- Rune plate.
- Droplet.
- Debris.
- Conductor.
- Field panel.

## Shader Library

Added Built-In Render Pipeline shaders:

- `Arcane/VFX21/Additive`.
- `Arcane/VFX21/Premultiplied`.
- `Arcane/VFX21/Crystal`.
- `Arcane/VFX21/Distortion`.
- `Arcane/VFX21/GroundResidue`.
- `Arcane/VFX21/Ribbon`.
- `Arcane/VFX21/Beam`.
- `Arcane/VFX21/StatusOverlay`.
- `Arcane/VFX21/PostBloom`.

Shaders support procedural colors, catalyst colors, noise, pulse, fracture, dissolve, opacity, emission, flow, and lifecycle state. Unsupported shader paths fall back to simpler generated materials.

## Hierarchical Particle Presentation

Added reusable particle requests for:

- Body layers.
- Element layers.
- Trails.
- Impacts.
- Fields.
- Status attachments.
- Near-miss wakes.

Particle layers inherit stable seed, element roles, lifecycle, priority, and owner. Patch 2.0 pooling and hard budgets remain active.

## Casting Anticipation

Casting presentation now reflects Rune structure before release.

Examples:

- Split forms multiple nuclei.
- Orbit displays angular movement and rails.
- Chain displays conductor nodes.
- Delay displays timing pulses.
- Persistent displays field-ready structure.
- Return displays reverse-flow cues.
- Pull draws nearby presentation inward.

## Morphology Transitions

Added procedural transitions for:

- Compression.
- Expansion.
- Fracture.
- Separation.
- Orbit.
- Reverse flow.
- Field unfolding.
- Dissolve.
- Expiration.
- Contact deformation.

A projectile becoming a field now visibly changes state instead of only spawning an unrelated generic circle.

## Local Target Response

Targets receive local or approximate impact presentation:

- Scorch marks.
- Frost and crystals.
- Lightning conductor nodes.
- Physical fracture marks.
- Blood wound marks.
- Toxic contamination points.
- Void compression marks.

Target marks are presentation-only. Damage, statuses, and reaction ownership remain in gameplay systems.

## Target Deformation Illusions

Added short-lived directional compression and rebound. Heavy impact, Void compression, and Frozen-shell effects can create a deformation impression without permanently modifying enemy meshes.

## Environment Contact Grammar

Impacts are classified as:

- Enemy.
- Ground.
- Wall.
- Barrier.
- Water-like surface.
- Existing elemental field.
- Unknown fallback.

Residue meshes, cracks, streaks, and materials align to the resolved surface normal.

## Near-Miss Presentation

Fast spell hosts can produce distance-limited wakes, directional streaks, and synthesized near-miss audio when passing the player. Near-miss effects are globally throttled and can be disabled.

## Persistent Field Inheritance

Generated fields inherit:

- Spell fingerprint.
- Core and Rune nodes.
- Element roles.
- Catalyst.
- Stable seed.
- Pulse rhythm.
- Rune-specific structure.

Persistent effects are no longer limited to one generic element-colored circle.

## Visual Conservation

Added structural constraints:

- Split children share parent visual mass.
- Chain and repeated effects attenuate.
- Decorative effects reduce before required mechanics.
- Core, mechanic path, Rune transformation, element expression, and decoration use explicit attention hierarchy.

## Procedural Audio Grammar

Added runtime-generated audio using `AudioClip.Create`.

Audio is synthesized from:

- Core voice.
- Element waveform/noise texture.
- Rune rhythm.
- Lifecycle phase.
- Impact envelope.
- Field loop behavior.

Rune behavior changes pitch, pulse timing, envelope, stereo position, repetition, and attenuation. Split children and chain jumps do not each play full-volume duplicate sounds.

## Audio Voice Budget and Ducking

Added voice priorities:

- Critical.
- Important.
- Normal.
- Secondary.
- Field.

Lower-priority voices are rejected or replaced before critical cues. Important impacts and high-tier reactions can briefly reduce lower-priority procedural audio without relying only on increased volume.

## Structural Camera Feedback

Added direction-aware camera feedback for:

- Release.
- Impact.
- Movement arrival.
- Pull.
- Expiration.
- Critical reaction emphasis.

Camera effects are distance-, priority-, quality-, and accessibility-aware.

## Flash Limiter

Added a global flash-energy accumulator and minimum-interval limiter. Multiple overlapping events are reduced collectively rather than being evaluated only as separate effects.

## Controlled Bloom and Distortion

Added optional Built-In Pipeline post-processing hooks:

- Lightweight bloom using the Patch 2.1 post shader.
- Shared GrabPass distortion shader.

Both features have quality and accessibility controls. They are decorative and never required for mechanic readability.

## Haptic Event Hooks

Added presentation events carrying:

- Impact magnitude.
- Direction.
- Charge.
- Reaction tier.
- Beam or field intensity.
- Return arrival.

The patch exposes hooks but does not claim full platform-specific controller implementation.

## Generated Spell Icons

SpellForge builds receive deterministic generated icons using:

- Core silhouette.
- Dominant element.
- Primary Rune glyph.
- Delivery border.
- Limited secondary marks.
- Stable icon seed.

Icons are cached by configuration hash.

## SpellForge Morphology Preview

Added a preview panel to the Workshop with modes for:

- Identity.
- Lifecycle.
- Rune isolation.
- Readability.

The panel displays:

- Generated icon.
- Body structure.
- Element roles.
- Rune graph.
- Phase timeline.
- Cost summary.
- Validation warnings.

## SpellForge Visual Diff

Pending Rune placement now reports structural changes such as:

- Added or removed phases.
- Added or removed Rune operators.
- Body-part count changes.
- Element-role changes.
- Cost changes.
- Rotation- or order-driven morphology changes.

## Runtime Inspection and Reference Tools

Added runtime controls:

- `F6`: Patch 2.1 inspector and quality/accessibility overlay.
- `F7`: spawn non-damaging reference morphology bodies from equipped spells.

The inspector reports contract ID, schema, Core, chassis, lifecycle, ordered Rune graph, elements, seeds, body parts, cost, active particle/geometry/audio counts, camera impulse, flash energy, and warnings.

## Editor Spell Visual Lab

Added an Editor window with:

- Generated-asset controls.
- Patch validation.
- Reference-spell spawning.
- Phase scrubber.
- Active-contract inspection.
- Automated visual capture trigger.

## Phase Scrubber

Active generated spell hosts can be forced to Prime, Charge, Release, Emit, Travel, Contact, Resolve, Persist, Return, Expire, or Cancel for inspection. Debug override is removable without rebuilding the spell.

## Automated Lifecycle Capture

Added Play Mode visual capture for:

- Charge.
- Release.
- Travel.
- Contact.
- Resolve.
- Persist.
- Return.
- Expire.

The capture writes screenshots and a report under `Patch210Captures/`.

## Automated Validation

Editor validation checks:

- Required source files.
- Integration markers.
- Generated textures, gradients, meshes, materials, glyphs, and shaders.
- Generator version.
- Patch 2.0 reaction definition count.
- Patch 2.0 mechanic graph count.
- Active contract schema.
- Lifecycle length.
- Generated body completeness.
- Contract warnings.

---

# Changed

- Patch 2.0 `ProceduralSpellPresentation` now forwards compilation, cast, impact, expire, direction-change, movement, projectile-host, Zone-host, and Familiar-host events into the 2.1 morphology system.
- SpellForge Workshop now embeds the morphology preview beside the existing spell preview.
- Pending Rune placement now appends a morphology diff to the numerical preview.
- Generated effects use explicit owner categories for cast, projectile, field, Familiar, status, reaction, and scene-level presentation.
- Low quality performs structural simplification rather than only reducing particle counts.
- Mixed elements use role assignment rather than one averaged color.
- Repeated and derived effects preserve the originating spell fingerprint.
- Runtime visual cache invalidation now occurs on scene reset and generator changes.

---

# Fixed

- Corrected invalid enum relational comparisons in the bundled Patch 2.0 presentation and reaction sources before installation.
- Prevented the camera post component from creating an unintended standalone camera on the Patch 2.1 persistent root.
- Removed asset-database batching that conflicted with synchronous texture importer configuration.
- Separated contract warnings from validation failures.
- Added explicit Unity object lookup qualification in Editor validation and capture tools.
- Added an actual expiration presentation path for direct spell expiration events.

---

# Deprecated

- Flat unordered Rune visual flags as the final presentation model.
- One generic field appearance for all Persistent spells.
- Blended color as complete multi-element presentation.
- Unseeded procedural body generation.
- Presentation objects without explicit owners.
- Reconstructing spell identity separately in multiple visual components.
- Removing required mechanic cues before decorative layers.

---

# Not Removed

Patch 2.1.0 retains:

- All Patch 2.0 elemental mechanics.
- All 120 elemental signatures.
- All reaction mechanic graphs.
- Existing Spell Cores and Rune gameplay.
- Existing procedural geometry.
- Existing particle and material fallbacks.
- Existing telegraphs and accessibility symbols.
- Low-quality fallback rendering.

---

# Accessibility and Quality Controls

Runtime settings include:

- Low, Medium, and High presentation quality through the existing Patch 2.0 system.
- Morphology overlay toggle.
- Near-miss toggle.
- Target-response toggle.
- Bloom toggle.
- Distortion toggle.
- Flash limit.
- Existing reduced-motion, blood-presentation, density, intensity, light, particle, trail, and residue limits.

Required telegraphs, hazard boundaries, Rune structure, and primary element identity are retained when decorative layers are reduced.

---

# Compatibility

- Existing legal SpellForge configurations are recompiled into schema 21000 contracts.
- Generated contracts are replaceable cache data, not irreplaceable save data.
- Existing enum values are not reordered by this patch.
- Patch 2.0 is included and installed automatically when required.
- The installer targets Unity 6000.5.2f1.
- The Built-In Render Pipeline remains the baseline.
- Missing generated assets use runtime-generated meshes, textures, and materials until Editor regeneration completes.

---

# Known Release-Candidate Limits

The package was not compiled or rendered inside the user’s Unity Editor from this environment. The following remain release gates:

- Unity C# compilation.
- Shader import on the target GPU/driver.
- Generated-asset import.
- Actual SpellForge UI layout at supported resolutions.
- Play Mode synchronization.
- Visual quality review.
- Audio level review.
- Low/Medium/High profiling.
- Flash and motion accessibility review.
- Reference-suite captures.
- Cleanup across scene and run reset.

A source file, generated recipe, or placeholder object is not considered sufficient evidence of final visual quality. The included checklist must be completed in Unity before changing the package status from release candidate to released.

---

# Exclusions

Patch 2.1.0 does not claim:

- A handmade prefab for every SpellForge permutation.
- Bespoke recorded audio for every permutation.
- Replacement character casting animations.
- Permanent enemy mesh destruction.
- Full environmental destruction.
- URP or HDRP migration.
- A specialized compound rule for every theoretical Rune pair.
- Unlimited visual complexity.
- Platform-specific controller haptics.
- Automated judgment that an effect is artistically finished.

Every supported legal configuration is expected to receive a deterministic procedural result and a documented fallback.
