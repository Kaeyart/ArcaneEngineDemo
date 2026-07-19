# Arcane Engine v0.3.0 — acceptance checklist

## Installation and boot

- Close Unity before applying the patch.
- Run the supplied in-place installation command against the existing v0.2 project.
- Reopen the same project with Unity 6000.5.2f1.
- Confirm Unity imports without Safe Mode or compiler errors.
- Open `Assets/ArcaneEngine/Scenes/Main.unity` and press Play.

## Aiming and damage

- Move with WASD while aiming independently through a full circle.
- Rotate the camera with Middle Mouse, then confirm WASD remains camera-relative.
- Zoom with the wheel and reset camera rotation with R.
- Fire at every Training target from the front, side, rear, close range, and maximum range.
- Confirm Fireball, arcing shots, homing shots, fast shots, large shots, and split shots register damage.
- Confirm the ground cursor matches cast direction.
- Confirm Training target Health visibly falls, damage numbers appear, and Health refills after a short delay.

## Health bars and feedback

- Confirm normal enemies show Health bars.
- Confirm elites show their name and modifiers.
- Confirm Shielded enemies display a separate blue Shield bar.
- Confirm Poison and Frozen status labels appear.
- Confirm the boss uses the top-screen boss bar.
- Disable damage numbers and always-visible bars in Options; confirm both settings work.

## Profiles and run saves

- Press Save Now and confirm the save status changes to Saved.
- Confirm the displayed save folder opens on Linux.
- Make at least four profile changes and confirm `profile_0.json`, `.bak1`, `.bak2`, `.bak3`, and `.sha256` are produced.
- Start a run, alter every spell board, find modifiers/items/spells, spend Gold, lose Health and Mana, then choose Save & Quit.
- Confirm Home Base appears and Continue Saved Run is available.
- Continue and confirm loadout, boards, inventory, equipment, modifiers, Gold, difficulty, depth, and resources return; confirm the current room restarts.
- Die or finish the run and confirm Continue Saved Run disappears.
- Start profile reset, cancel it, and confirm progress remains. Repeat and confirm deliberately.

## Content and progression

- Unlock and test Poison Bolt, Frost Lance, Storm Orb, and Gravity Field.
- Test all six new modifiers on compatible and incompatible spell deliveries.
- Craft and test Venom Burst, Frozen Beam, Chain Storm, and Black Hole.
- Confirm only one Legendary Spell may be active.
- Confirm Gold disappears after a run while Essence and Legendary Shards follow their permanent rules.
