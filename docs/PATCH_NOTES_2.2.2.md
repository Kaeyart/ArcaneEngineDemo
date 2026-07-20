# Arcane Engine Demo Patch 2.2.2
# Authored Content and Input System Hotfix

Patch 2.2.2 is a maintenance release for Patch 2.2.1. It does not change spell damage, reaction balance, propagation limits, fields, Runes, VFX tuning, or save data.

## Fixed

### Remaining generated-content script identities

The remaining persistent V21 ScriptableObject classes were still declared together inside `V21AuthoredContent.cs`. Unity persistent assets require a stable MonoScript identity, and classes that do not live in a same-named script file can deserialize as missing Behaviour references.

Patch 2.2.2 moves all seven remaining persistent content types into same-named scripts:

- `V21RoomLayoutAsset.cs`
- `V21EnemyContentAsset.cs`
- `V21AudioEventAsset.cs`
- `V21AffixContentAsset.cs`
- `V21RoomDefinitionAsset.cs`
- `V21ShopServiceAsset.cs`
- `V21RewardDefinitionAsset.cs`

This fixes the reported missing-script errors while loading room definitions, affixes, and audio events. It also prevents the same fault from appearing later for room layouts, enemy content, shops, or rewards.

### Authored-content regeneration

The installer removes the stale generated `Assets/ArcaneEngine/Resources/V21Content` directory after backing it up. Unity rebuilds the complete authored-content library using the corrected MonoScript identities.

The Editor repair validates and rebuilds:

- Spell cores and Runes.
- Items and Relics.
- Room layouts and room definitions.
- Enemy content.
- Audio events.
- Affixes.
- Shop services.
- Rewards.

### Input System compatibility

`ReactionDiagnosticsOverlay22` no longer reads F5 through the legacy `UnityEngine.Input` API when the project uses the Input System package.

- Input System projects use `Keyboard.current.f5Key.wasPressedThisFrame`.
- Legacy-input projects retain a guarded `Input.GetKeyDown` fallback.
- Projects with neither input backend compile without attempting to read an unavailable input API.

## Not changed

- Direct spell damage.
- Elemental buildup values.
- Reaction lineage.
- Death propagation.
- Chain limits.
- Field authority and merging.
- SpellForge builds.
- Patch 2.1 morphology and VFX behavior.

## Unity validation

After installation and compilation, run:

`Arcane Engine > 2.2.2 > Validate Hotfix`

The validator must report nonzero counts for every generated content family and at least 36 room layouts and 24 audio events.
