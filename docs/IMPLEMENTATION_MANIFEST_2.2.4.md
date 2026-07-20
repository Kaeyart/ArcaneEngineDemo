# Patch 2.2.4 Implementation Manifest

## Runtime additions

- `SpellEffectLifecycle224.cs`
  - Persistent lifecycle coordinator.
  - Cleans effects on room transitions, scene unloads, run termination, death cleanup, and the manual validation command.
  - Clears transient spell objects, reaction fields, residues, pooled particles, procedural audio, morphology owners, presentation coalescing, lineage state, diagnostic counters, and presentation caches.

## Runtime modifications

- `HexBoard.cs`
  - Replaces directional port activation with deterministic edge-touch breadth-first traversal.
  - Multi-cell Runes connect when any occupied hex touches the core or an already connected Rune.
  - Rotation still affects occupied shape and generated morphology, not connectivity permission.

- `SpellExecution.cs`
  - Adds independent `CastRequest.linkGeneration` recursion depth.
  - Evaluates equipped Spell Links before ordinary propagation-generation rejection.

- `SpellLinks.cs`
  - Uses `linkGeneration` instead of reaction/Split propagation generation.
  - Retains event-ID suppression, cooldowns, trigger power, and the six-generation Link ceiling.

- `RoomSceneManager.cs`
  - Clears active spell presentation before unloading or replacing a room.

- `GameWorld.cs`
  - Invokes the centralized cleanup from the existing transient-object/death cleanup path.

- `ElementalReactionField.cs`
  - Adds explicit clearing of every active reaction field.

- `ReactionPropagation22.cs`
  - Adds explicit lineage-registry reset.

- `PresentationResidues.cs`
  - Adds explicit clearing of active presentation residues.

- `ReactionPresentation22.cs`
  - Adds explicit clearing of coalescing timestamps.

- `MorphologyPresentationDirector21.cs`
  - Clears scene owners, target-local marks, deformation, flash state, and generated-asset runtime caches.

- `ProceduralAudioHaptics21.cs`
  - Stops and resets all pooled procedural voices during lifecycle cleanup.

## Editor additions

- `Patch224LifecycleLinkingValidator.cs`
  - `Arcane Engine > 2.2.4 > Validate Hotfix`
  - `Arcane Engine > 2.2.4 > Clear Active Spell Effects`

## Unchanged systems

Patch 2.2.4 does not change spell damage, elemental buildup, reaction coefficients, field power, Chain falloff, Spell Link trigger power, Spell Link cooldowns, Rune capacity, compatibility restrictions, board radius, overlap rules, or unlock rules.
