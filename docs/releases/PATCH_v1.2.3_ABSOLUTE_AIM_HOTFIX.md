# Arcane Engine v1.2.3 — Absolute Aim Hotfix

## Root causes

- The Unity Editor could alternate between root Game-view IMGUI coordinates and Input System host-window coordinates. The mismatched fallback was clamped into the camera pixel rectangle, producing apparent left/right edge sticking.
- Facing used the raw world vector from the player to the cursor intersection. Near the character that vector approaches zero, so tiny cursor changes produced disproportionately large angular changes.

## Fixed

- The Unity Editor now keeps the Game-view-local IMGUI pointer as one stable coordinate authority after it becomes available.
- Standalone builds keep Unity Input System mouse coordinates authoritative, with IMGUI retained only as a device fallback.
- Pointer authority no longer swaps after a two-frame timeout.
- Invalid NaN and infinite pointer samples are rejected.
- Added a 14-pixel cursor-over-character aim dead zone.
- The last valid 360-degree aim direction is retained inside that dead zone.
- Added a minimum 0.65-world-unit aim distance so facing, casting, and the world reticle never derive direction from a near-zero vector.
- Player rotation and manual spell release now consume the same stable aim direction.
- Point-targeted spells continue to use the resolved cursor world position outside the small center dead zone.

## Preserved

- WASD movement remains independent from aiming.
- Middle Mouse camera rotation, wheel zoom, and R camera reset remain separate.
- Left Shift aim assistance remains opt-in.
- The hardware cursor stays visible and unlocked.
- Existing profiles, checkpoints, spell layouts, items, and progression remain compatible.
