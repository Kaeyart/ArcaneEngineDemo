# Patch 2.0.0 Implementation Manifest

This document maps the Patch 2.0.0 scope to runtime implementation.

| Scope | Runtime implementation |
|---|---|
| Deterministic visual recipes | `ProceduralSpellVisualCompiler2`, `GeneratedVisualRecipe` |
| Element visual grammar | `ElementVisualProfileRegistry2` |
| Rune visual operators | Recipe operator extraction and `GeneratedSpellHostVisual2` |
| Board-topology inputs | `BoardTopologySummary` and reflection-based board resolver |
| Gameplay/presentation boundary | `SpellPresentationBus`, `ProceduralSpellPresentation` |
| Cast and impact presentation | `SpellPresentationDirector2` |
| Particle backend | `PresentationParticlePool2`, `PooledPresentationParticle2` |
| Geometry backend | `PresentationGeometry2`, `PooledPresentationGeometry2` |
| Projectile/Zone/Familiar layers | `GeneratedSpellHostVisual2` |
| Enemy ailment stages | `ElementalStatusVisual2` |
| Reaction assembly | `ReactionAssemblyVisual2` |
| Environmental residue | `PresentationResidue2` |
| Field merging and surge visuals | `PresentationResidue2.Merge`, `SurgeNear` |
| Mechanic-specific sequences | `PresentationEffectObjects*.cs` |
| Audio hooks | `ProceduralPresentationAudio2` |
| Camera feedback | `PresentationCameraFeedback2` |
| Accessibility/settings | `Patch200PresentationSettings`, F10 overlay |
| Diagnostics | F9 overlay and Editor validation menu |
| Stress testing | F8 runtime stress sequence |
| 120 definitions | `ElementalReactionCodex.Generated.cs` |
| 120 executable graphs | `ElementalReactionMechanicCodex.Generated.cs` |
| Gameplay execution | `ElementalReactionMechanicExecutor` |
| Buildup and major states | `ElementalReactionRuntime` |
| Fields | `ElementalReactionField` |
| Spell compiler integration | `ProceduralSpellPresentation.Compile(result, board)` |
| Existing visual integration | Hooks in `SpellVisualEvents`, `SpellVisualAttachment`, and `SpellDeliveryVisuals` |
| SpellForge interaction | `V21ProductUI.WorkshopDrag.cs` and patched UI methods |
| Casting correction | `PlayerController.CastInput.cs` |
| Movement/input correction | patched `PlayerController.cs` |
| Generated-content repair | patched `V21ContentBuilder.cs` |
| UI pointer repair | patched `RuntimeUIFactory.cs` |
