# Arcane Engine Demo
# Patch 3.0.0-alpha.1.1 — Foundation Hardening

**Requires:** 3.0.0-alpha.1  
**Target editor:** Unity 6000.5.2f1  
**Validation status:** package/static validation complete; target-editor compilation and Play Mode confirmation required

## Purpose

This focused patch hardens the first persistent-ARPG foundation before content expansion. It does not add campaign content or begin the full alpha.2 item catalogue.

## Changes

- Advances the persistent profile schema from `30001` to `30002`.
- Repairs null, duplicate, malformed, and out-of-range profile records during load.
- Merges duplicate currency stacks and removes invalid equipment references.
- Repairs map, item, affix, discovery, and enum data without deleting the legacy roguelite profile.
- Replaces process-dependent `string.GetHashCode()` crafting seeds with stable FNV-based deterministic hashing.
- Removes `int.MinValue` indexing hazards in map selection and Core/Rune/Link discovery.
- Makes Fracture Runes protect a specific affix instead of setting a cosmetic boolean only.
- Preserves fractured modifiers during alteration, chaos replacement, value rerolls, and removal.
- Marks the protected modifier in item descriptions.
- Makes crafting transactional: failed operations restore the item exactly and refund the currency.
- Makes Essence and Omen family targeting strict; they fail and refund the currency when the requested family is unavailable.
- Adds explicit diagnostics for missing item-base and map-definition pools.

## Scope boundary

This patch is static hardening. It does not claim Unity compilation or runtime correctness until tested in Unity 6000.5.2f1.
