# Arcane Engine Demo 2.0.0 — Play Mode Acceptance

Use Unity 6000.5.2f1. Do not approve the patch as Unity-verified until every blocking check below passes.

## 1. Import and compile

1. Open `Assets/ArcaneEngine/Scenes/Main.unity`.
2. Wait for import and script compilation to finish.
3. Confirm the Console has zero red compile errors.
4. Run `Arcane Engine > Validate 2.0.0 Demo`.
5. Confirm the validation completion message appears without an exception.

## 2. Startup and input

1. Enter Play Mode.
2. Confirm the Hierarchy contains exactly one camera named `Isometric Camera`.
3. Start or continue a run.
4. Move with WASD while independently aiming through a full circle with the mouse.
5. Confirm aim does not stick, flip to a screen edge or change sensitivity based on cursor distance.
6. Open and close every allowed UI once. Confirm a single key press does not immediately reopen or close it.

## 3. Core gameplay regression

1. Clear a combat room using all equipped spells.
2. Confirm projectile hit registration and damage.
3. Confirm enemy health bars remain above their assembled silhouettes and disappear off-screen/on death.
4. Pick one center reward with E. Confirm unchosen alternatives deactivate immediately.
5. Read the three route-door states and enter one with E.
6. Confirm spell editing is blocked during active combat and allowed after the room is clear.
7. Confirm Support Runes can be dragged, dropped and rotated without changing their stored footprint incorrectly.
8. Confirm equipment cannot be swapped during a run and an unwanted equipment reward can be salvaged.

## 4. Visual contract sampling

1. Cast at least one spell of every available element and delivery.
2. Test Homing, Split/Fork, Chain, Bounce/Return, a persistent zone and a barrier if available.
3. Trigger a Spell Link chain and confirm link cues expire.
4. Fight one Elite with two affixes and inspect persistent affix identity and event responses.
5. Apply Burning, Poison, Shock, Chill/Freeze and confirm removal/death cleanup.
6. Fight the Dungeon Warden through all three phases.
7. Visit multiple room purposes in each reachable biome and confirm dressing never covers the player entry, center reward, doors or enemies.
8. Confirm there are no pink/missing-material renderers.

## 5. Save and continuation

1. Save at a valid room-entry checkpoint.
2. Return to the title and continue the saved run.
3. Confirm room, spells, Support Runes, Spell Links, equipment and reward state reconstruct correctly.
4. Confirm no visual reconstruction mismatch is logged.
5. Finish through extraction and through defeat in separate runs; compare the displayed secured/lost items and currencies with the actual run state.

## 6. Accessibility and quality

1. Test Low, Medium and High visual quality.
2. Test Reduced Flashes, Reduced Motion, high-contrast telegraphs and colorblind symbols independently.
3. Confirm mechanical boundaries remain visible on Low.
4. Confirm Reduced Motion affects presentation but not projectile movement, hit timing or combat authority.

## 7. Diagnostics and stress evidence

1. Enable Visual Diagnostics in Options, then press F10.
2. Run each named stress button and wait for the post-cleanup result.
3. Treat every `CHECK FAILED` result as blocking until explained and fixed.
4. Run `30 ROOMS`; confirm material families remain at or below the displayed cache limit.
5. Run `SAVE x8`; confirm all disk reconstruction round-trips pass.
6. Run `CAPTURE FULL CATALOG MATRIX`; confirm 87 files plus the context and manifest are produced.
7. Review grayscale element captures, all delivery families, all status/affix captures and the 68 biome/room captures.
8. Archive the stress reports with the tested hardware, resolution and Unity version before publishing performance claims.

## Approval rule

Blocking failures are compile errors, validation exceptions, broken input, incorrect damage/rewards/save state, visual cleanup leaks, missing mechanical boundaries, obstructed gameplay spaces, or failed reconstruction signatures. Cosmetic tuning notes may be recorded separately, but should not be misreported as implemented fixes.
