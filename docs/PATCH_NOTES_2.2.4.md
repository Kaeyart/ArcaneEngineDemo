# Patch 2.2.4 — Lifecycle and Linking Hotfix

## Fixed: spell effects surviving rooms and death

Patch 2.2.4 introduces a central spell-effect lifecycle owner. It clears active projectiles, delayed effects, familiars, persistent zones, reaction fields, reaction stage objects, morphology hosts, residues, pooled particles, procedural audio voices, coalescing state, and reaction-lineage state when:

- The active room changes.
- A room scene unloads.
- The run changes from active to inactive, including player death.
- The manual Editor cleanup command is used.

Cleanup is explicit and immediate. Transient GameObjects are disabled before destruction so their child presentation owners cancel in the same frame.

## Fixed: Spell Links suppressed by propagation generation

Patch 2.2 used one `CastRequest.generation` value for reaction propagation, Split children, repeated effects, and linked-cast recursion. This could make a valid Spell Link reach the general propagation ceiling before its Link condition was evaluated.

Patch 2.2.4 adds an independent `linkGeneration` value. Spell Links retain their existing per-event activation guard and cooldowns, but are no longer rejected because a spell was produced by Split, Return, a reaction echo, or another non-Link propagation path.

## Changed: Support Rune connectivity

Rune activation no longer requires directional input and output ports to face each other.

A Rune is active when any occupied hex belonging to that Rune shares an edge with:

- The central Spell Core, or
- Any Rune already connected to the Spell Core.

Connectivity is resolved as a deterministic breadth-first flood fill across the entire board. This includes multi-cell Runes and chains of touching Runes. Rotation still changes the Rune’s occupied shape and generated spell morphology, but rotation no longer randomly disconnects a physically touching Rune.

## Not changed

- Spell damage and buildup values.
- Reaction propagation balance.
- Chain damage falloff.
- Field authority.
- Spell Link conditions, trigger power, or cooldown values.
- Rune capacity costs.
- Rune compatibility restrictions.
- Board overlap and unlocked-cell rules.
