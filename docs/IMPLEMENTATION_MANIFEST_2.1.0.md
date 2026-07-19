# Patch 2.1.0 Implementation Manifest

This manifest maps the Patch 2.1 scope to executable source and integration points.

## Runtime source

| File | Implemented responsibility |
|---|---|
| `SpellVisualContract21.cs` | Schema 21000 contract, lifecycle/Rune/body/cost/seed data, owner and phase enums, deterministic seed functions. |
| `SpellMorphologyCompiler21.cs` | Contract compilation, board reflection, Rune ordering, precedence, interaction rules, delivery coverage, body generation specification, cost model, identity checks, cache. |
| `GeneratedAssetRuntime21.cs` | Runtime fallback meshes, textures, gradients, materials, shader selection, quality/accessibility settings. |
| `MorphologyRuntime21.cs` | Explicit owners, generated body construction, body-part surface state, host lifecycle, split/orbit/persist/return/expire behavior, projectile/Zone/Familiar attachment, near-miss sampling. |
| `MorphologyPresentationDirector21.cs` | Cast, impact, expiration, movement, reaction assembly, field, barrier, target, environment, flash, camera, residue and scene cleanup presentation. |
| `ProceduralAudioHaptics21.cs` | Deterministic generated audio clips, phase voices, priority budget, ducking, near-miss audio, haptic event bus. |
| `CameraOverlayReference21.cs` | Directional camera response, optional bloom/flash composite, F6 diagnostics/settings, F7 reference morphology. |
| `SpellForgeMorphologyPreview21.cs` | Generated icon, identity/lifecycle/Rune-isolation/readability preview, phase timeline, morphology diff. |
| `AutomatedCapture21.cs` | Play Mode reference-spell lifecycle screenshot capture and report. |

## Editor source

| File | Implemented responsibility |
|---|---|
| `Patch210AssetBuilder.cs` | Deterministic atlas, gradients, 160 mesh variants, materials, manifest, auto-regeneration, validation. |
| `Patch210Diagnostics.cs` | Patch validation menu, asset/reaction/integration checks, Spell Visual Lab, phase scrubber, capture launcher. |

## Shaders

| Shader | Use |
|---|---|
| `ArcaneVFX21Additive.shader` | Energy cores, sparks, high-emission body parts. |
| `ArcaneVFX21Premultiplied.shader` | Translucent shells and controlled alpha. |
| `ArcaneVFX21Crystal.shader` | Cold/crystal Fresnel and fracture presentation. |
| `ArcaneVFX21Distortion.shader` | Controlled Built-In Pipeline spatial distortion. |
| `ArcaneVFX21GroundResidue.shader` | Surface-aligned fields and residue. |
| `ArcaneVFX21Ribbon.shader` | Trails, Rune circuits, movement guides. |
| `ArcaneVFX21Beam.shader` | Beam and Chain structures. |
| `ArcaneVFX21StatusOverlay.shader` | Local target buildup and state marks. |
| `ArcaneVFX21PostBloom.shader` | Optional camera bloom/flash composite. |

## Patched project files

| Project file | Integration |
|---|---|
| `Assets/ArcaneEngine/Scripts/Presentation/PresentationAPI.cs` | Forwards compile, cast, impact, expire, direction, movement, host, Zone and Familiar calls into Patch 2.1. |
| `Assets/ArcaneEngine/Scripts/UI/V21ProductUI.cs` | Adds the generated morphology preview to the SpellForge details view. |
| `Assets/ArcaneEngine/Scripts/UI/V21ProductUI.WorkshopDrag.cs` | Adds before/after morphology diff to pending placement preview. |

## Generated Unity assets

Generated on Editor import under `Assets/ArcaneEngine/Resources/Patch210/`:

- `Textures/ArcaneVFXAtlas.png`.
- `Textures/ArcaneGradientRamps.png`.
- Twenty mesh families with eight deterministic variants each.
- Element/body material variants.
- `generation-manifest.json`.

Runtime fallback generation is available before or when an Editor-generated asset is unavailable.

## Included foundation

The package contains the standalone Patch 2.0.0 installer and therefore also installs:

- Movement/UI/SpellForge/casting corrections.
- Seven elemental buildup families.
- Seven major states and death behavior.
- 120 multi-element reaction definitions.
- 120 executable reaction graphs.
- Mergeable fields.
- Patch 2.0 presentation events, particles, geometry, fields, audio, camera, diagnostics and accessibility controls.

## Non-placeholder accounting

The package does not count the following as implementation:

- Empty interfaces.
- Data types without consumers.
- Menu items without runtime behavior.
- Shader names without shader source.
- Reaction names without mechanic executors.
- Preview text without generated body/phase data.

Each main Patch 2.1 contract is consumed by runtime morphology, SpellForge preview, diagnostics, or asset generation.
