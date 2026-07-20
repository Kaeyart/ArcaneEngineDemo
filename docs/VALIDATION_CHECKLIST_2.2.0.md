# Patch 2.2.0 Validation Checklist

## Installation

- Close Unity.
- Install Patch 2.2.0 using the supplied installer.
- Confirm the installer reports 120 reaction definitions and 120 reaction plans.
- Open Unity 6000.5.2f1.
- Wait for compilation and asset import.
- Run `Arcane Engine > 2.2 > Validate Patch 2.2.0`.

## Console

- No C# compiler errors.
- No missing method or type errors from reaction source replacement.
- No orphaned field or presentation exceptions.
- No repeated particle reconfiguration warnings from Patch 2.1.1.

## Direct versus propagated buildup

- Direct casts can activate every major state.
- Pure propagated buildup stops at 75% threshold.
- A target needs at least 35% direct contribution.
- Generation-one propagation can finish a target at 70% preparation.
- Generation-two buildup cannot activate a major state.
- An element below 20% threshold does not join a reaction signature.

## Major states

- Activating a major state consumes one full threshold.
- Overflow does not exceed 25% threshold.
- Normal recovery lasts approximately 2.25 seconds.
- Boss recovery lasts approximately 4 seconds.
- Direct damage continues during recovery.
- Propagated buildup cannot bypass recovery.

## Reaction resolution

- Fusion selects no more than three standard mechanics.
- Compound selects no more than four.
- Catastrophe selects no more than five.
- Convergence selects no more than six.
- Calamity selects no more than eight.
- Apex selects no more than twelve.
- Death budgets match 2/2/3/3/4/5.
- Duplicate chains and fields consolidate.
- High-tier mechanics stage instead of resolving in one frame.
- Reaction consumption and signature lockout match the tier table.

## Elemental deaths

- A full death mechanic requires a major state or 75% preparation.
- Primary death effects select no more than four targets.
- Generation-one deaths are visibly and mechanically reduced.
- Generation-two deaths do not propagate.
- Shatter uses 2.8 radius and no more than four targets.
- Generation-one Shatter cannot Freeze.
- Fire, Toxic, Blood and Void death residue cannot reproduce fields.
- Lightning death Chain does not revisit targets.
- Physical displacement remains capped.

## Chain and Split

- Default Chain reaches no more than three secondary targets.
- Jump damage and buildup decline.
- Targets are not revisited by one lineage.
- Split children share one Chain-energy pool.
- Three Split children do not each create a full-strength network.
- Later contacts remain readable without full impact explosions.

## Fields

- No more than sixteen gameplay fields exist globally across reaction fields and SpellForge Persistent/trail zones.
- No more than six gameplay fields exist locally across both field runtimes.
- Same-signature fields merge into one pulse owner.
- Field power never exceeds 125%.
- Mixed fields replace parent pulse loops.
- Pulse intervals remain between 0.6 and 1.2 seconds.
- Split Fields creates no more than four gameplay fields.
- Trail Line creates no more than five gameplay segments.
- Field Surge uses a two-second cooldown and preparation.
- Death and secondary fields have reduced authority.
- Generation-two effects cannot create fields.

## Passive states

- Ignited gameplay trail patches are spaced by at least 1.5 seconds.
- SpellForge trail lines create no more than five gameplay segments and share one bounded power budget.
- Generation-two zone requests become short cosmetic residue instead of gameplay fields.
- Persistent-zone Pull executes only on the field pulse and affects no more than four targets.
- One Ignited enemy owns no more than two gameplay patches.
- Overcharged passive arcs use approximately 1.25 seconds and no more than two targets.
- Saturated aura uses approximately 1.25 seconds and no more than three targets.
- Unstable pull uses approximately 0.5 seconds and no more than four targets.

## Presentation

- Related events within 0.12 seconds coalesce.
- Shatter cascades produce one primary wave plus local breaks.
- Chain remains sequential and visible.
- Field interiors remain quiet while idle.
- Fields pulse only on gameplay pulses.
- Cosmetic residue lasts no more than approximately 1.5 seconds.
- No more than ten local cosmetic residues remain.
- Cosmetic residue has no damage, buildup, looping particles, or dynamic light.
- Major bursts do not exceed two per second.
- Total burst events do not exceed three per second.

## Diagnostics

- Press F5 in Play Mode.
- Verify origin, generation and source messages appear.
- Verify selected/discarded mechanic counts change.
- Verify blocked recursion increases during cascades.
- Verify field merge/reject counters operate.
- Verify coalescing counter operates.
- Verify reproduction ratio is measurable.

## Combat-density targets

- Unprepared packs remain below approximately 0.5 reproduction ratio.
- Prepared packs can create short cascades.
- Dedicated propagation builds approach 0.8–1.1 with investment.
- Passive death residue remains well below 1.
- Enemy danger telegraphs remain readable during maximum supported density.

## Cleanup

- Scene reset removes fields, staged mechanics, pulses, orbiters, shards, residue and audio owners.
- No lineage target history persists into a new session.
- No field power grows beyond its cap after repeated merges.
- No orphan camera, light, audio, or particle object remains.
