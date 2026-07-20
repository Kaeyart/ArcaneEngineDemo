# Arcane Engine Demo — Patch 2.2.0
# Reaction Propagation and Battlefield Clarity

**Status:** Release candidate source package  
**Target Unity version:** 6000.5.2f1  
**Requires:** Patch 2.1.1; the standalone installer includes Patch 2.1.0 and Patch 2.1.1 when they are missing  
**Patch type:** Elemental reaction rework, secondary-effect balance pass, field-authority rework, SpellForge propagation limits, and combat-presentation consolidation

## Purpose

Patch 2.2.0 changes the systems that allow one spell or elemental death to multiply into many additional full-strength effects.

The previous runtime treated many secondary sources almost like direct casts. Death waves, Chain contacts, fields, delayed pulses, orbiting nodes, shards, status spreading, and repeated reaction instructions could all apply damage or buildup without a common record of how far the effect had propagated. High-tier reaction plans also executed every generated mechanic instruction, allowing many pulses, fields, chains, control effects, sounds, lights, flashes, and ground effects to resolve together.

This patch does not remove elemental cascades. It makes them decay unless the player has prepared the next targets or invested into a delivery that is explicitly allowed to continue.

The intended behavior is:

- A direct spell has full authority.
- A first derived effect can help finish a prepared target.
- A second derived effect can conclude a cascade but cannot reproduce it.
- Later recursion is blocked or presentation-only.
- A field has authority determined by its source.
- Split and Chain divide a conserved power budget rather than copying it.
- Reaction graphs select and stage defining mechanics instead of firing every instruction simultaneously.
- Related VFX events share one presentation event, flash allowance, camera response, and audio emphasis where possible.

## Added — Reaction lineage

Every new Patch 2.2 reaction context contains:

- Origin cast identifier.
- Propagation generation.
- Source category.
- Damage coefficient.
- Buildup coefficient.
- Reaction-recharge coefficient.
- Permission to activate a major ailment.
- Permission to trigger a death reaction.
- Permission to create a gameplay field.
- Permission to recharge reaction assembly.

Supported source categories are:

- Direct Cast.
- Rune Derived.
- Primary Reaction.
- Death Reaction.
- Field.
- Chain.
- Echo.
- Environmental.

The same origin identifier is retained by Split children, Chain networks, fields, pulses, orbiting nodes, shards, reaction echoes, and death propagation.

Targets receive a short lineage-specific revisit lock. This prevents several children from the same cast from repeatedly selecting one target during the same propagation sequence.

## Changed — Propagation generations

Generation zero represents the original cast or an explicitly authorized primary action.

Generation zero retains:

- 100% configured damage authority.
- 100% configured buildup authority.
- Full major-ailment permission.
- Full death-reaction permission.
- Full field-creation permission.
- Full reaction-recharge contribution.

Generation one represents the first derived effect. The default context retains:

- 60% inherited damage authority.
- 35% inherited buildup authority.
- 25% inherited reaction-recharge authority.
- Conditional major-ailment permission.
- Restricted death and field permissions inherited from its parent.

Generation two represents a recursively derived effect. The default context retains:

- 30% configured damage authority before mechanic-specific falloff.
- 10% configured buildup authority before mechanic-specific falloff.
- No reaction-recharge contribution.
- No major-ailment permission.
- No death-reaction permission.
- No field-creation permission.

Generation three and later receive no elemental buildup and no recursive permissions. A later-generation sequence may still terminate visually or deal a deliberately configured minor non-propagating hit.

## Added — Direct and propagated buildup

Each target now tracks direct and propagated buildup separately for all seven elements.

Direct buildup comes from generation-zero or otherwise explicitly authorized player-owned application.

Propagated buildup comes from death reactions, Chain, echoes, derived fields, reaction mechanics, status spreading, and recursive helper objects.

The status summary reports both values in diagnostic builds.

## Changed — Propagated buildup cap

Propagated buildup may occupy no more than 75% of a major-ailment threshold.

At least 35% of the threshold must be present as direct buildup before a major ailment may activate.

A generation-one effect may finish a major ailment only when the target has already reached at least 70% preparation for that element.

Generation-two effects cannot activate a major ailment.

This means a Shatter, death explosion, Toxic cloud, secondary Chain, or reaction echo can make an untouched target much easier to prepare, but it normally cannot generate the complete major state without direct player setup.

## Changed — Reaction participation threshold

An element now needs 20% of its major-state threshold before it joins a reaction signature.

The thresholds are calculated from each element's existing major threshold:

- Fire: 1.2 buildup.
- Cold: 1.6 buildup.
- Lightning: 1.2 buildup.
- Physical: 1.4 buildup.
- Blood: 1.2 buildup.
- Toxic: 1.6 buildup.
- Void: 1.4 buildup.

Incidental traces still remain on the target but do not immediately create a high-order signature.

## Changed — Major-ailment activation

When a major ailment activates, one complete threshold is consumed.

No more than 25% of one threshold may remain as overflow.

This replaces the previous behavior where the full meter remained available and could be refreshed by a very small secondary application.

When a major state ends, the target receives recovery against the same major state:

- Normal enemies: 2.25 seconds.
- Bosses: 4 seconds.

Direct damage still applies during recovery. Other elements and reaction signatures remain available. Propagated buildup cannot bypass the recovery period.

## Changed — Reaction consumption and signature lockouts

Reaction resolution consumes buildup according to tier:

| Tier | Buildup consumed | Same-signature lockout |
|---|---:|---:|
| Fusion | 55% | 2.5 seconds |
| Compound | 60% | 3.5 seconds |
| Catastrophe | 70% | 5 seconds |
| Convergence | 75% | 6.5 seconds |
| Calamity | 80% | 8 seconds |
| Apex / Worldbreak | 100% | 12 seconds |

A different valid signature may still form while one signature is locked.

Generation-zero buildup contributes fully to reaction charge. Generation-one buildup contributes 25%. Generation-two buildup contributes nothing.

## Reworked — Reaction graph execution

The 120 reaction definitions and 120 generated mechanic plans remain installed.

The executor no longer runs every instruction in a plan.

It first consolidates duplicate mechanic categories, then selects a deterministic set based on:

- Primary damage role.
- Catalyst role.
- Propagation role.
- Field allowance.
- Control allowance.
- Termination role.
- Tier priority.
- Source permissions.

Runtime mechanic limits are:

| Tier | Standard resolution | Death resolution |
|---|---:|---:|
| Fusion | 3 | 2 |
| Compound | 4 | 2 |
| Catastrophe | 5 | 3 |
| Convergence | 6 | 3 |
| Calamity | 8 | 4 |
| Apex / Worldbreak | 12 | 5 |

Duplicate Chain instructions become one stronger bounded Chain sequence. Duplicate fields become one field group. Duplicate pulses become one selected or staged sequence. Only one field category and one primary control category are selected per stage.

Catastrophe and higher reactions, or any selected set above four mechanics, execute through a stage runner instead of resolving every mechanic in one frame.

## Reworked — Buildup Wave

Buildup Wave is now explicitly propagated application.

It no longer calls the direct application path or contributes full direct reaction authority.

## Reworked — Single-element deaths

A full single-element death effect requires the corresponding major state or at least 75% of that element's threshold at death.

Otherwise the death receives only its local presentation marker.

Primary death propagation has:

- Maximum four targets.
- Fixed buildup budget.
- Distance and preparation-aware target selection.
- Lineage revisit prevention.
- Reduced generation-one behavior.
- No generation-two reproduction.

### Cold — Frozen Shatter

Primary Shatter uses:

- 2.8-unit radius.
- Maximum four targets.
- 4% of the dead enemy's maximum health before distance falloff.
- Approximately 4.8 total Chill budget.
- Maximum 1.25 Chill on one target.
- Completion only on sufficiently prepared targets.
- No automatic Cold field unless field creation was authorized.

A generation-one Shatter:

- Selects at most two targets.
- Uses approximately 35% of primary Shatter damage.
- Applies at most 0.35 Chill to one target.
- Cannot Freeze.
- Cannot create a field.
- Cannot create another mechanical Shatter.

### Fire — Ignited death

Ignited death uses a fixed Scorch budget and a bounded target list. It may finish Ignition on prepared targets but does not fully Ignite an untouched group. Secondary death explosions cannot reproduce full Fire death explosions. Death residue has reduced field authority and cannot create another field.

### Lightning — Overcharged death

Overcharged discharge selects at most three targets. Targets cannot be revisited by the same lineage. Static and damage decline by jump. Secondary discharges cannot create another complete discharge.

### Physical — Broken death

Broken death uses a fixed Trauma budget, bounded targets, and capped displacement. Secondary shockwaves cannot create additional shockwaves.

### Blood — Hemorrhaging death

Blood death uses a fixed Wound budget. Only authorized primary propagation may create gameplay residue. Secondary Bloodbursts cannot create more Blood fields or full Bloodbursts.

### Toxic — Saturated death

A primary Saturated death may create one short Toxic field using Death Residue authority. The cloud has reduced Poison buildup, cannot independently Saturate an untouched target, and cannot reproduce another death cloud.

### Void — Unstable death

A primary Unstable death performs one bounded pull and one collapse. Child collapses and secondary fields cannot create another rift.

## Reworked — Chain

Chain remains a prominent sequential effect, but power is conserved.

The default secondary target limit is three.

Damage coefficients by jump are:

- First secondary contact: 68%.
- Second secondary contact: 48%.
- Third secondary contact: 32%.
- Additional specialized jumps: 18% with further attenuation.

Buildup amounts by jump are:

- First: 0.45.
- Second: 0.28.
- Third: 0.16.
- Additional specialized jumps: 0.06.

Targets cannot be revisited within one lineage window.

Later contacts retain readable beam intensity but do not receive full impact explosions or full recursive authority.

## Reworked — Split and Chain

A cast now owns one Chain-energy pool.

Every structural copy that requests a Chain network reserves a fair share based on the number of copies and remaining pool. Split children do not each receive a complete independent network at full power.

The default pool permits a modest total increase for a Split–Chain build, but the increase is bounded and shared.

## Reworked — Spell status spreading

Spell status spreading no longer copies every full status effect to three nearby targets.

It now:

- Creates a derived Echo context.
- Uses a fixed total elemental buildup budget of 1.05.
- Selects at most three targets.
- Applies at most 0.45 buildup to one target.
- Cannot create a field.
- Obeys lineage target immunity.

## Reworked — Secondary spell explosions

Secondary explosion contacts now:

- Use an Echo context.
- Retain the configured explosion damage with propagation coefficients.
- Apply 0.35 elemental buildup instead of the full spell status package.
- Cannot create a gameplay field.
- Obey later-generation permissions.

## Added — Field authority

Every Patch 2.2 reaction field has one authority category:

- Explicit Persistent.
- Primary Reaction.
- Death Residue.
- Secondary Propagation.
- Cosmetic.

Default authority scaling is:

| Authority | Damage authority | Buildup authority |
|---|---:|---:|
| Explicit Persistent | 100% | 100% configured field coefficient |
| Primary Reaction | 70% | 30% |
| Death Residue | 40% | 18% |
| Secondary Propagation | 20% | 8% |
| Cosmetic | 0% | 0% |

A field's context determines whether it may trigger death propagation, recharge reactions, or create another field.

## Reworked — Field caps

The reaction-field runtime now enforces:

- Maximum sixteen gameplay fields globally.
- Maximum six gameplay fields in one local combat area.

When a new field exceeds the budget, the runtime attempts to merge or replace an existing lower-authority field. If it cannot do so without removing a more important field, the new field is rejected and reported by diagnostics.

## Reworked — Same-signature fields

Same-signature fields no longer run independent pulse loops when they overlap.

The strongest field remains authoritative. A valid reinforcement adds up to 10% power and a bounded duration refresh. Total field power is capped at 125%.

The old repeated multiplicative merge behavior is removed.

## Reworked — Mixed fields

Compatible overlapping signatures replace their parent loops with one merged field. The result has:

- One owner.
- One pulse loop.
- One capped power value.
- One boundary.
- One interior.
- One central signature.
- One audio/presentation owner.

A completed signature cannot repeatedly merge with copies of itself for unlimited scaling.

## Reworked — Field timing

Ordinary field pulse intervals are clamped between 0.6 and 1.2 seconds.

A field remains visually quiet between actual pulses.

Void pulling and Physical displacement are target-capped and execute only on field pulses.

## Reworked — Split Fields and Trail Line

Split Fields creates at most four gameplay fields.

Trail Line creates at most five gameplay segments, using wider spacing. Later-generation contexts cannot create gameplay fields.

One reaction stage may select only one field group.

## Reworked — Field Surge

Field Surge now has:

- Two-second per-field cooldown.
- Maximum 15% power increase.
- Maximum 25% remaining-duration increase.
- 0.4-second preparation before the surge resolves.
- No immediate untelegraphed pulse.
- The same 125% field power cap.

## Reworked — Major-state passive output

Ignited movement creates gameplay patches no more than once every 1.5 seconds, with at most two owned patches per Ignited enemy. The continuous trail may remain visual.

Overcharged passive arcs use a 1.25-second interval and at most two secondary targets.

Saturated propagation uses a 1.25-second interval and at most three targets. Its Poison application cannot independently Saturate an untouched target.

Unstable pulling uses a 0.5-second interval and at most four targets, using smoother bounded pull values.

## Reworked — Recursive helper objects

Delayed pulse emitters:

- Use at most five pulses.
- Use lineage context.
- Derive later pulses as Echo generation.
- Limit area targets.
- Obey the global flash limiter.

Orbiting reaction nodes:

- Use at most six nodes.
- Attack no faster than every 0.75 seconds.
- Derive attacks as later-generation Echo effects.
- Use bounded Chain contacts.

Reaction shards:

- Use at most fourteen shards.
- Derive from the originating lineage.
- Obey target revisit rules.
- Stop creating buildup or recursive effects beyond generation two.

## Added — Presentation-event coalescing

Presentation events are coalesced when they share:

- Origin cast.
- Event family.
- Signature or mechanic family.
- Compatible field authority.
- Overlapping position.
- A 0.12-second time window.

The gameplay may still resolve individual targets. Presentation uses one primary event plus local target responses.

## Added — Global burst limiting

Patch 2.2 limits major presentation bursts to two per second and total burst events to three per second. Events beyond that allowance are reduced or replaced by local presentation instead of producing another full flash.

## Reworked — Chain presentation

Chain contacts retain sequential beams and conductor relationships. Later jumps use lower visual intensity. Recipient contacts do not create a full explosion, camera impulse, and audio impact for every target.

## Reworked — Ground presentation

Gameplay fields use:

- One authoritative boundary.
- One quiet interior.
- One central signature.
- A pulse only when the gameplay field pulses.
- One expiry transition.

Cosmetic residue:

- Deals no damage.
- Applies no buildup.
- Creates no field.
- Uses no looping particle system.
- Uses no dynamic light.
- Lasts no more than 1.5 seconds by default.
- Is limited to ten local residue objects.

## Added — Propagation diagnostics

Press **F5** in Play Mode to open the Patch 2.2 diagnostics overlay.

The overlay reports:

- Direct buildup applications.
- Propagated buildup applications.
- Recursive effects blocked.
- Target revisits blocked.
- Mechanics selected and discarded.
- Presentation events coalesced.
- Fields merged and rejected.
- Active gameplay fields.
- Cascade reproduction ratio.
- Recent lineage decisions.

Editor menu:

`Arcane Engine > 2.2 > Propagation Diagnostics`

Validation menu:

`Arcane Engine > 2.2 > Validate Patch 2.2.0`

## Cascade reproduction target

Diagnostics estimate the number of qualifying reaction deaths created per reaction death in the same lineage.

The tuning target is:

- Unprepared packs: below 0.5.
- Partially prepared packs: approximately 0.5–0.8.
- Dedicated propagation builds: approximately 0.8–1.1.
- Rare high-investment or Apex events: temporarily above 1.
- Passive death residue alone: well below 1.

## Not changed

Patch 2.2.0 does not remove:

- The seven elements.
- The 120 reaction signatures.
- The 120 generated reaction plans.
- Major ailments.
- Frozen Shatter.
- Ignited deaths.
- Lightning Chain.
- Toxic clouds.
- Blood propagation.
- Physical shockwaves.
- Void collapses.
- Persistent fields.
- Split, Chain, Orbit, Return, Bounce, or Delay.
- Prepared cascade builds.
- Calamity or Worldbreak identity.

Broad direct-spell damage reduction is intentionally deferred until the new secondary-effect multiplication can be measured in Play Mode.

## Compatibility

Existing SpellForge configurations remain valid.

Patch 2.1 visual contracts remain installed. Patch 2.2 extends presentation events with reaction origin, generation, source, authority, and coalescing data.

Temporary combat state is rebuilt when entering Play Mode. No player progression reset is intended.

## Validation status

The package installer, source transforms, lexical source structure, marker checks, repeat installation, archive integrity, and rollback are validated outside Unity.

The following require the target Unity project:

- C# compilation and domain reload.
- Actual gameplay synchronization.
- Balance tuning.
- Reaction reproduction measurements.
- Shader and VFX review.
- Combat-density profiling.
- Audio and camera review.
- Accessibility review.

Patch 2.2.0 should be treated as a release candidate until those checks pass.
