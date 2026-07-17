# Arcane Engine v0.3.2 — acceptance checklist

## Starting loadout

- At Home Base select `Select Spells & Start`.
- Confirm the dedicated loadout screen appears above the Home Base UI.
- Select Fireball, Ice Nova, and Lightning Strike in Slot 1 one at a time and confirm the card updates.
- Confirm locked Slots 2 and 3 cannot be selected before their permanent upgrades.
- Confirm duplicate spells require duplicate stored Spell Copies.
- Enter the dungeon and confirm the selected Slot 1 spell appears in the combat HUD and casts.

## Mouse aim

- Move the mouse freely through every screen edge and corner.
- Confirm the screen crosshair and ground reticle move together.
- Confirm the short direction line and player facing point toward the reticle.
- Cast without Left Shift and confirm no enemy snapping occurs.
- Enable aim assistance, hold Left Shift, and confirm intentional snap assistance works.
- Rotate and zoom the camera and repeat the checks.

## Combat, rewards, and doors

- Defeat every enemy and confirm three rewards appear in the room center instead of a modal selection screen.
- Approach each reward and confirm its world title and focused HUD description appear.
- Press E on one reward and confirm only that reward applies and all three reward objects disappear.
- Confirm three physical doors appear along the north wall.
- Approach each door and confirm its category/icon, room name, difficulty, and hazard information.
- Press E and confirm the selected room loads.
- Complete reward-category, challenge, cursed, event, puzzle, treasure, and secret rooms and confirm they require combat before rewards.

## Health bars

- Confirm normal enemies have a red bar above their rendered top edge.
- Confirm the bar follows moving, charging, pulled, large, and small enemies without drift.
- Confirm elites show their name and status/affix text.
- Disable always-visible bars and confirm normal full-Health bars hide until damaged.
- Confirm bosses use the top-screen boss bar.

## Workshop and equipment

- During active combat press Tab, modify a board, close it, and confirm the compiled spell changes.
- During active combat press I, equip an item, close it, and confirm stats update.
- Repeat both actions while rewards are present and while route doors are open.
- Confirm opening these panels pauses the room and closing them restores it.

## Save regression

- Save & Quit during a run, reopen the same project, and Continue.
- Confirm the current room restarts with the saved spell boards, equipment, stored spells, modifiers, Gold, Health, and Mana.
- Confirm existing profile files remain in the same Linux save folder.
