# Arcane Engine Demo Patch 2.2.3
# Procedural Audio and Reward Safety Hotfix

Patch 2.2.3 is a maintenance release for Patch 2.2.2. It does not change spell damage, reaction propagation, Chain behavior, fields, Rune behavior, generated morphology, or save data.

## Fixed

### Disabled procedural audio voices

Procedural audio voices were parented directly to the object they followed. When a spell host, projectile, pooled presentation object, or other owner was inactive, the voice also became inactive in the hierarchy. Calling `AudioSource.Play()` then produced repeated `Can not play a disabled audio source` errors.

Patch 2.2.3 keeps every pooled voice under the persistent, always-active procedural-audio root. A voice follows its owner by updating its world position instead of becoming the owner's child.

The pool now also:

- Reactivates reused voice GameObjects.
- Re-enables reused AudioSources.
- Stops a voice before assigning a new clip.
- Removes destroyed voices from the static pool.
- Tracks ownership explicitly for `StopOwned`.
- Stops owner-bound loops when their owner becomes inactive.
- Preserves one-shot audio at its last valid world position.
- Reactivates the persistent audio root if it was disabled.

### Null reward content identifiers

Reward presentation could classify an offer as a Spell Core while the offer's `contentId` was null. `DemoCatalog.GetCore(null)` passed that value directly into `Dictionary.TryGetValue`, which throws `ArgumentNullException`.

`DemoCatalog.GetCore`, `GetModifier`, and `GetItem` now return `null` for null or empty identifiers. Reward presentation already supports a missing definition and will use its generic family presentation rather than aborting room completion.

## Not changed

- Direct spell power.
- Elemental buildup or ailment thresholds.
- Reaction lineage and cascade limits.
- Field authority or field merging.
- SpellForge configurations.
- Procedural VFX morphology.
- Item, Rune, or reward selection logic.

## Unity validation

After installation and compilation, run:

`Arcane Engine > 2.2.3 > Validate Hotfix`

Then clear the Console, cast several spells repeatedly, complete a room, and open the reward choices. The disabled-audio and null-catalog errors should not return.
