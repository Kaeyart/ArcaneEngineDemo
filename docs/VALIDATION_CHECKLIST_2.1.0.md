# Patch 2.1.0 Unity Validation Checklist

Do not call the build a final 2.1.0 release until every required item is verified in Unity 6000.5.2f1.

## Import and compilation

- [ ] Project opens without package-resolution errors.
- [ ] All C# scripts compile without errors.
- [ ] All nine Patch 2.1 shaders compile.
- [ ] Patch210AssetBuilder runs once when assets are absent.
- [ ] Generation manifest reports version 21000.
- [ ] Texture atlas and gradient ramps import correctly.
- [ ] Generated meshes and materials are present.
- [ ] `Arcane Engine > 2.1 > Validate Patch 2.1.0` passes.

## Patch 2.0 foundation

- [ ] Runtime UI buttons remain clickable.
- [ ] Home Base and run movement function.
- [ ] Physical W/A/S/D input functions.
- [ ] Rune dragging, movement, rotation, validation and preview function.
- [ ] Instant, channeled and charged casting remain correct.
- [ ] Elemental codex validates 120 signatures.
- [ ] Reaction mechanics execute without recursion leaks.

## Contract generation

- [ ] Every equipped legal spell produces schema 21000.
- [ ] Contract ID remains stable across repeated compiles.
- [ ] Core ID and chassis are correct.
- [ ] Rune source IDs match the board.
- [ ] Rune coordinates and rotations match the board.
- [ ] Rune order matches gameplay order.
- [ ] Element roles match the final compiled spell.
- [ ] Small board edits preserve unchanged body substructures.

## Core identity

- [ ] Projectile retains a traveling body and directional intent.
- [ ] Beam retains source connection and continuous contact.
- [ ] Zone retains boundary, anchor and pulse.
- [ ] Nova remains radial.
- [ ] Meteor retains descent and heavy impact.
- [ ] Melee retains sweep and contact direction.
- [ ] Familiar remains autonomous and owner-linked.
- [ ] Movement retains departure, route and arrival.

## Rune visibility

For every supported Rune:

- [ ] At least one major phase visibly changes.
- [ ] At least one supporting phase visibly changes.
- [ ] The Rune remains recognizable on Low quality.
- [ ] Rune glyph is stable in Forge and world presentation.
- [ ] Unsupported deliveries report a fallback or unsupported state.

## Ordered graph cases

- [ ] Split → Chain differs from Chain → Split.
- [ ] Delay → Chain differs from Chain → Delay.
- [ ] Pull → Split differs from Split → Pull.
- [ ] Bounce → Persistent differs from Persistent → Bounce.
- [ ] Orbit + Return reverses flow coherently.
- [ ] Barrier + Reflect displays directional facets and reversal.

## Lifecycle

- [ ] Prime/Charge anticipation is visible.
- [ ] Release is synchronized with gameplay.
- [ ] Emit creates the host body.
- [ ] Travel follows the authoritative host.
- [ ] Contact appears at the correct point.
- [ ] Resolve aligns with damage/status timing.
- [ ] Persist matches field radius and duration.
- [ ] Return reverses direction and flow.
- [ ] Expire removes or returns owned objects.
- [ ] Cancel removes or returns owned objects.

## Generated body

- [ ] Core, shell and internal-energy layers are visible.
- [ ] Rune nodes and circuits follow board topology.
- [ ] Split nuclei separate without duplicating full parent mass.
- [ ] Orbitals use the correct rotation direction.
- [ ] Chain conductor nodes appear before transfer.
- [ ] Delay timing rings accelerate correctly.
- [ ] Persistent panels unfold into fields.
- [ ] No generated part appears at world origin unexpectedly.

## Elements and materials

- [ ] Fire, Cold, Lightning, Physical, Blood, Toxic and Void are recognizable without color alone.
- [ ] Mixed elements use distinct roles.
- [ ] Catalyst is the highest-priority accent without whitening the full spell.
- [ ] Emission remains readable under bloom.
- [ ] Distortion has a valid non-distortion fallback.
- [ ] Material state follows Charge, Hold, Return and Expire.

## Target response

- [ ] Marks appear near contact locations.
- [ ] Fire creates local scorch/flame response.
- [ ] Cold creates frost/crystal response.
- [ ] Lightning creates conductor response.
- [ ] Physical creates fracture/debris response.
- [ ] Blood respects Full/Reduced/Hidden modes.
- [ ] Toxic creates contamination response.
- [ ] Void creates compression response.
- [ ] Presentation marks never apply damage.
- [ ] Target scale returns to baseline after deformation.

## Environment response

- [ ] Ground residue aligns to the ground normal.
- [ ] Wall residue aligns to the wall normal.
- [ ] Barrier response remains distinct.
- [ ] Existing field contact remains readable.
- [ ] Near-miss effects are throttled.
- [ ] Near-miss can be disabled.

## Persistent fields

- [ ] Field preserves spell fingerprint.
- [ ] Split + Persistent produces the expected pattern.
- [ ] Orbit + Persistent retains orbiting structure.
- [ ] Chain + Persistent retains propagation cues.
- [ ] Delay + Persistent remains dormant until activation where gameplay supports it.
- [ ] Field duration and pulse match gameplay.
- [ ] Expired fields clean up all owners, audio and lights.

## Audio

- [ ] Every base Core has a generated phase voice.
- [ ] Elements alter waveform/noise character.
- [ ] Split child voices attenuate.
- [ ] Chain jump voices attenuate.
- [ ] Delay pulse accelerates.
- [ ] Return has a reverse/arrival cue.
- [ ] Field loops stop on expiration.
- [ ] Voice budget preserves critical cues.
- [ ] Ducking restores to baseline.

## Camera and screen effects

- [ ] Release impulse follows cast direction.
- [ ] Impact impulse follows travel direction.
- [ ] Movement arrival response is directional.
- [ ] Flash limiter reduces simultaneous flashes.
- [ ] Bloom can be disabled.
- [ ] Distortion can be disabled.
- [ ] Reduced motion limits camera response.
- [ ] Pausing and time-scale changes do not leave effects stuck.

## SpellForge preview

- [ ] Preview panel appears without breaking existing UI layout.
- [ ] Generated icon is stable.
- [ ] Identity mode displays body hierarchy.
- [ ] Lifecycle mode animates phases.
- [ ] Rune Isolation highlights the selected/primary Rune.
- [ ] Readability mode removes decoration while retaining structure.
- [ ] Pending placement displays an accurate morphology diff.
- [ ] Invalid placement does not create a contract or permanent preview state.

## Diagnostics and capture

- [ ] F6 overlay opens and closes.
- [ ] F7 reference bodies spawn without damage.
- [ ] Spell Visual Lab opens.
- [ ] Phase scrubber controls active hosts.
- [ ] Automated capture writes all requested lifecycle frames.
- [ ] Capture report includes contract ID and cost.
- [ ] Scene reset clears reference objects and active presentation owners.

## Quality and performance

- [ ] Low quality preserves body, Rune structure and telegraphs.
- [ ] Medium quality uses generated particles/materials within budget.
- [ ] High quality enables controlled bloom/distortion.
- [ ] Distant effects reduce structurally.
- [ ] Off-screen decorative effects reduce.
- [ ] Critical effects are not denied for ordinary decoration.
- [ ] Particle, geometry, trail, light and audio budgets hold in stress tests.
- [ ] No owner, particle, audio loop, light or generated GameObject leaks after scene reset.

## Accessibility

- [ ] Element identity does not rely only on hue.
- [ ] Global flash limit functions with several Lightning effects.
- [ ] Reduced-motion mode functions.
- [ ] Blood reduction and hiding function.
- [ ] Required telegraphs remain visible at Low quality.
- [ ] Bloom and distortion controls function independently.

## Release decision

- [ ] No required checklist item remains unresolved.
- [ ] Known issues are written into final release notes.
- [ ] Planned wording is converted to Added/Changed only for verified features.
- [ ] Exact rollback was tested against the final ZIP.
