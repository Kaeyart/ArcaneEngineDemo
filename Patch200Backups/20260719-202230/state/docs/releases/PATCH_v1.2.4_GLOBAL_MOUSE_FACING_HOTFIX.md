# Arcane Engine v1.2.4 — Global Mouse Facing Hotfix

## Root cause

- `PlayerController.LateUpdate()` returned before resolving aim whenever `RunActive` was false.
- Home Base separately rotated the player toward movement velocity.
- Therefore the corrected absolute aiming system from v1.2.3 never executed in Home Base, and WASD continued to overwrite the visible facing direction there.

## Fixed

- Mouse aim and player facing now run in every playable world state: Home Base, training, exploration, combat, rewards, shops, and safe rooms.
- Home Base movement no longer writes player rotation.
- `PlayerController` is now the sole owner of playable-space facing.
- Title, preparation, Workshop, Inventory, Map, Help, and Pause screens suspend world aiming while visible.
- The ground aim reticle is visible in Home Base so aim behavior can be verified immediately after leaving the title screen.
- Spell input, Mana spending, cooldowns, and cast execution remain run-only.

## Included previous fixes

- Stable Editor versus standalone pointer authority from v1.2.3.
- Near-character aim dead zone and minimum direction distance from v1.2.3.
- Workshop GUILayout stabilization from v1.2.2.
- Profile schema migration compile fix from v1.2.1.
