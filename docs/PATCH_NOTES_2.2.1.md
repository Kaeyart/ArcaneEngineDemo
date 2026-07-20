# Arcane Engine Demo Patch 2.2.1
## Unity 6.5 EntityId Compatibility Hotfix

Patch 2.2.1 is a maintenance release for Patch 2.2.0.

## Fixed

- Replaced obsolete `Object.GetInstanceID()` calls in the Patch 2.2 reaction-lineage runtime with `Object.GetEntityId()`.
- Changed reaction target-visit history from integer keys to `UnityEngine.EntityId` keys.
- Changed elemental-field owner identity from an integer sentinel to `UnityEngine.EntityId`.
- Replaced ownerless field value `0` with `EntityId.None`.
- Preserved exact lineage target-revisit protection and Ignited trail-field ownership limits without integer hashing.

## Files changed

- `Assets/ArcaneEngine/Scripts/Combat/ReactionPropagation22.cs`
- `Assets/ArcaneEngine/Scripts/Combat/ElementalReactionField.cs`
- `Assets/ArcaneEngine/Scripts/Combat/ElementalReactionRuntime.cs`
- `Assets/ArcaneEngine/Scripts/Combat/ElementalReactionMechanicExecutor.cs`

## Added

- `Assets/ArcaneEngine/Editor/Patch221Diagnostics.cs`
- `Assets/ArcaneEngine/PATCH_2_2_1.txt`

## Not changed

This hotfix does not change:

- Damage values.
- Buildup coefficients.
- Chain limits or falloff.
- Field limits, authority, power, duration, or pulse timing.
- Major-ailment thresholds or recovery.
- Reaction mechanic budgets.
- VFX presentation or accessibility settings.
- Save data.

## Unity validation

After Unity finishes compiling, run:

`Arcane Engine > 2.2.1 > Validate EntityId Hotfix`

Then clear the Console and enter Play Mode. Confirm that the two CS0619 errors do not return.
