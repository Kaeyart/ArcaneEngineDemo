# Arcane Engine 1.1.0 — Release Checklist

Use Unity 6000.5.2f1. Open `Assets/ArcaneEngine/Scenes/Main.unity`.

## Automated gate

- Run `Arcane Engine > Validate 1.1 Demo` and require zero failures.
- Confirm the title footer reads `VERSION 1.1.0-DEMO · THE FORGE REBORN`.
- Confirm the Console contains no compiler error, missing-theme warning, or startup null reference.

## Aim and combat

- Hold W while moving the cursor through a complete circle; movement stays forward and facing follows the cursor.
- Cast all three spell slots toward near, far, left, right, and screen-edge targets.
- Rotate the camera with Middle Mouse and zoom with the wheel; verify the ground marker remains the cast target.
- Open a UI and click/drag inside it; no combat spell should fire.

## Enemy bars

- Damage, knock back, charge, pull, and kill standard and elite enemies.
- Confirm bars remain attached and disappear at death.
- Confirm Shield and status layers update.
- Test always-show, damaged-only, numbers, scale, and vertical-offset options.
- Confirm the boss uses only the top-screen boss frame.

## Spell Workshop

- Drag a modifier from inventory to valid and invalid cells.
- Rotate with Q, E, and Mouse Wheel while dragging.
- Move an installed modifier; remove it by dragging over inventory.
- Cancel with Right Mouse and close the panel during a drag.
- Perform 100 mixed place/move/rotate/remove/undo/redo actions and verify no copy loss or duplication.
- Verify the same physical copy cannot be installed on two boards.

## Items and Forge

- Inspect Common, Magic, Rare, Unique, and Corrupted tooltips in beginner and advanced modes.
- Verify Magic never exceeds 1 prefix/1 suffix and Rare never exceeds 3/3.
- Apply every Forge action; verify cost, failure refund, crafted limit, lock behavior, quality cap, and corruption lockout.
- Test all six sorts, text search, rarity/slot/tag filters, favorites, junk, lock, selling, and pagination.
- Drag backpack equipment to slots and equipped items back to inventory.
- Compare a two-handed weapon while an Offhand is equipped.

## Loot and saves

- Set filters that hide a drop; hold Alt to reveal it.
- Verify Unique and Corrupted items cannot be hidden.
- Create several nearby drops and confirm labels stack.
- Close Workshop/inventory, quit, and continue the run; verify boards, equipment, affixes, quality, corruption, currencies, and room checkpoint.
- Load a v1.0 profile and confirm the backup plus Legacy migration behavior.

## Stress

- Complete a 60-minute run while watching active enemies, spell entities, loot labels, and bar pool size.
- Require no stale bars, duplicate modifiers, illegal equipment, unbounded triggers, repeated input transitions, or persistent frame-time spikes.
