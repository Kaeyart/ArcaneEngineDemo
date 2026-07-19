# Patch 2.0.0 Validation Checklist

## Installation

- Close Unity before installation.
- Confirm the installer prints `Arcane Engine Demo Patch 2.0.0 installed.`
- Record the printed backup path.
- Confirm the installer reports 120 reaction definitions and 120 mechanic plans.

## Unity compilation

- Open the patched project.
- Wait until script compilation and asset import finish.
- Do not enter Play Mode while authored content is rebuilding.
- Confirm there are no red Console errors.
- Run `Arcane Engine > 2.0 > Validate Patch 2.0.0`.

## Existing corrections

- Click runtime UI buttons.
- Move with W, A, S, and D in Home Base.
- Start a run and confirm combat-only systems remain run-scoped.
- Verify instant spells cast once per press.
- Verify channeled spells continue while held.
- Verify charged spells charge and release.

## SpellForge

- Drag a compatible Rune from inventory.
- Rotate with Q and E.
- Rotate with the mouse wheel.
- Drop on a valid cell.
- Drop on an invalid cell and confirm no board mutation.
- Move an installed Rune.
- Right-click to remove a Rune.
- Confirm incompatible Runes cannot be selected.
- Confirm preview values match the final spell after placement.

## Elements

Trigger and inspect:

- Scorch and Ignited.
- Chill and Frozen.
- Static and Overcharged.
- Trauma and Broken.
- Wound and Hemorrhaging.
- Poison and Saturated.
- Corruption and Unstable.

Kill one enemy in each major state and verify the corresponding death behavior.

## Reactions

- Trigger a pair and observe assembly.
- Add a third element during assembly.
- Add a fourth element during assembly.
- Confirm the signature upgrades rather than restarting.
- Trigger a reaction death.
- Trigger a persistent field.
- Merge two fields.
- Verify the field signature and presentation change.

## Procedural presentation

- Use F9 to inspect generated recipes.
- Recast the same spell and confirm its visual identity remains stable.
- Change one Rune and confirm the recipe hash and operator list change.
- Change Rune rotation and inspect topology output.
- Test Chain, Split, Return, Orbit, Delay, Persistent, Pull, Spread, and Consume.
- Confirm element roles remain readable in mixed spells.

## Accessibility and quality

Use F10:

- Test Low, Medium, and High quality.
- Reduce density.
- Enable Reduced Motion.
- Set camera feedback to zero.
- Test Blood Full, Reduced, and Hidden.
- Confirm critical telegraphs remain visible.

## Stress

- Press F8.
- Observe all stress-test stages.
- Use F9 to verify budgets and denied requests.
- Repeat the test.
- Reset the run and confirm temporary objects clear.
- Reload the scene and confirm no persistent effect objects remain.

## Rollback

Run the rollback command printed by the installer only when Unity is closed. Reopen Unity and verify the pre-installation project state.
