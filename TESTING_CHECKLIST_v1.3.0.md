# Arcane Engine v1.3.0 — Hardware Aim Test

1. Enter Home Base and move the hardware cursor continuously around the entire Game view.
2. Confirm the cyan world reticle follows the pointer without pausing, catching, accelerating, or snapping to an edge.
3. Circle the player at a constant mouse speed. Character rotation should remain immediate and continuous.
4. Move the pointer close to and across the player. Only the mathematically exact center may retain the previous direction; there is no surrounding dead zone.
5. Hold each WASD key while aiming elsewhere. Movement must never overwrite rotation.
6. Stop moving the cursor for five seconds, then move again. Aim must resume on the first frame without a sticky release.
7. Move the cursor to every Game-view edge and corner. The aim ray must not clamp to left or right.
8. Rotate and zoom the camera, then repeat the edge and circle tests.
9. Start a run and cast projectile, hitscan, zone, movement, and triggered spells in every direction.
10. Open and close Workshop and Inventory. Aim should suspend while the modal is open and resume immediately when closed.
11. Confirm the title footer reads `VERSION 1.3.0-DEMO`.
12. Confirm the Console contains no input, ray, null-reference, or GUILayout errors.
