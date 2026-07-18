# Arcane Engine v0.3.2 — World Room Flow Patch

## Free aiming

- The mouse is explicitly unlocked during active gameplay.
- The software crosshair follows the screen cursor without clamping.
- A ground reticle marks the exact world-space point used to calculate spell direction.
- A short player-facing line makes the outgoing direction readable at close range.
- Aim assistance no longer silently overrides ordinary mouse direction. When enabled in Options, it activates only while Left Shift is held.

## Physical post-combat loop

- Defeating a room now spawns three physical reward choices in the center.
- Walk near a reward to focus it and press E to take it.
- Choosing one reward removes the alternatives and opens physical route doors along the north wall.
- Standard route phases offer three doors.
- Each door displays a room-category label and uses a different icon shape/color for fights, elites, bosses, shops, healing, safe rooms, spell rewards, equipment, and Base Spells.
- Walk near a door and press E to enter its room.
- Reward-category, challenge, cursed, event, puzzle, treasure, and secret rooms now run an encounter before presenting their room reward.

## Tooltips and interaction HUD

- Every reward and route door has a small camera-facing world label.
- The nearest interactable receives a focused `[ E ]` prompt.
- The combat HUD displays the focused object's full name, action, and description.
- Interaction selection always chooses the closest valid object, preventing multiple rewards or doors from activating on one key press.

## Enemy Health bars

- Normal and elite bars are now camera-facing world presentations created with the enemy.
- Their position reads the live renderer top bound every frame.
- Fill position and width update directly from the enemy's current Health ratio.
- Elite names, affixes, Poison, and Frozen states appear with the bar.
- The accessibility option for always-visible enemy Health is respected.
- Boss Health remains in the dedicated top-screen boss bar.

## In-run building and equipment

- Tab opens the Spell Workshop during combat, after combat, while rewards are present, and while doors are open.
- I opens equipment and the run backpack in those same phases.
- Opening either panel pauses action safely; closing it returns to the unchanged room state.
- Spell modifiers and physical copies remain independently owned by their correct boards and inventories.

## Starting spell selection

- `Select Spells & Start` opens a new full-screen UI Toolkit loadout screen.
- Each unlocked starting slot is displayed as a dedicated card.
- Stored physical Spell Copies are listed with their count, description, and element accent.
- Legendary Spells are supported while preserving the one-active-Legendary rule.
- Duplicate standard selections require duplicate physical Spell Copies.
- Locked slots clearly identify the permanent-progression requirement.
- The dungeon cannot start without a valid spell in Slot 1.

## Compatibility

- In-place update for the same Unity project and save location.
- Unity Editor target remains 6000.5.2f1.
- Existing profiles and run snapshots retain their schema and paths.
- Application version is 0.3.2.
