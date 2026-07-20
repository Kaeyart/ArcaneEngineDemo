# Patch 2.2.0 Implementation Manifest

## Runtime source

### ReactionPropagation22.cs
Defines lineage contexts, source categories, propagation generations, global balance constants, target revisit protection, diagnostic counters, mechanic budgets, field authority, Chain falloff, major-state consumption, and signature lockouts.

### ElementalReactionRuntime.cs
Replaces the reaction controller and runtime adapters with direct/propagated buildup tracking, major-state threshold consumption, recovery, participation thresholds, reaction recharge authority, same-signature lockouts, bounded single-element deaths, passive-state throttles, and contextual damage/field helpers.

### ElementalReactionMechanicExecutor.cs
Consolidates duplicate reaction mechanics, chooses deterministic tier budgets, enforces one field and one control family per stage, stages high-tier reactions, and propagates lineage through every mechanic helper.

### ElementalReactionField.cs
Implements field source authority, sixteen global fields, six local fields, capped reinforcement, replacement-based merging, one pulse loop, 0.6–1.2-second timing, surge preparation/cooldown, target caps, and lineage-aware damage/buildup.

### ElementalReactionPulseEmitter.cs
Limits delayed pulses, derives later pulses, applies contextual area authority, and uses the global burst limiter.

### ElementalReactionOrbiters.cs
Limits nodes and attack frequency, derives attacks as Echo effects, and uses bounded Chain contacts.

### ElementalReactionShard.cs
Limits shard count, tracks lineage, blocks revisits, and terminates recursive authority after generation two.

### SpellReactionBridge22.cs
Maps `CastRequest` and `SpellCastBudget` into reaction contexts and coordinates the shared Split/Chain energy pool.

### ReactionDiagnosticsOverlay22.cs
Installs the Play Mode F5 diagnostics panel.

## Presentation source

### ReactionPresentation22.cs
Emits lineage-aware reaction, death, mechanic, field, and Chain events; limits major flashes; and coalesces compatible events.

### PresentationCore.cs
Extends `SpellPresentationEvent` with lineage data and sends all events through the coalescer before subscribers receive them.

### PresentationResidues.cs
Separates gameplay field presentation from cosmetic residue, removes idle looping particle volumes, caps local cosmetic residue, and pulses only when gameplay pulses.

## Spell execution source transform

`tools/apply_patch_220.py` patches the installed `SpellExecution.cs` to add:

- A shared Chain-energy pool to `SpellCastBudget`.
- Direct-hit reaction contexts.
- Bounded three-target Chain with jump falloff.
- Shared Split/Chain network power.
- Fixed-budget status spreading.
- Contextual secondary explosions.
- A unified sixteen-global/six-local budget shared by reaction fields and SpellForge Persistent zones.
- Same-element Persistent-zone merging and capped 125% reinforcement.
- Explicit Persistent-field priority over reaction residue.
- Five-segment trail maximum with a conserved segment-power budget.
- Generation-two field conversion to short cosmetic residue.
- Field-authority damage and buildup coefficients.
- 0.85-second Persistent-zone pulse timing.
- Pull execution only on the actual field pulse, capped at four targets.
- Removal of the redundant second ground ring; one authoritative gameplay boundary remains.

## Editor source

### Patch220Diagnostics.cs
Adds source validation and an Editor diagnostics window.

## Marker

`Assets/ArcaneEngine/PATCH_2_2_0.txt`

Schema: `reaction-propagation-22000`
