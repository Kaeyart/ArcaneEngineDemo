# Arcane Engine — v1.0.5 Gameplay Input and Audio Hotfix

This release fixes the condition where the title could be exited but Home Base and run actions still received no usable keyboard or mouse input.

## Gameplay input

- Added `ArcaneInputPump` to the persistent bootstrap object.
- IMGUI events now provide a second independent input source alongside Unity Input System 1.19.0.
- The fallback tracks held keys, key-down, key-up, held mouse buttons, mouse-down, mouse-up, pointer position, wheel motion, and horizontal mouse drag.
- Event transitions bridge safely into the following gameplay update, preventing events delivered after `Update` from being lost.
- Losing application focus clears held fallback state so movement or casting cannot become stuck.
- WASD movement, E interaction, Space dodge, mouse casting, Q, Tab, I, M, F1, F10, camera rotation, camera zoom, and charged-cast release all use the combined input source.

## Audio

- Removed the continuously looping generated 55 Hz dungeon ambience tone.
- Removed the continuously looping generated 110 Hz music tone.
- Music and ambience sources remain silent placeholders for authored assets instead of synthesizing a background hum.
- Short UI, enemy-warning, and narrative event cues remain available and continue to respect their volume controls.

## Compatibility

- Includes all fixes from v1.0.1 through v1.0.4.
- Existing profiles, run checkpoints, controls, and save paths remain compatible.
