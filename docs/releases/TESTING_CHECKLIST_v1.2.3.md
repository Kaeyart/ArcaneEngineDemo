# Arcane Engine v1.2.3 — Aim Test

Run these checks in the Unity Game view after clearing the Console.

1. Move the mouse in a complete circle around the player. The character must face continuously through all 360 degrees.
2. Hold the cursor above, below, left, and right of the player for three seconds each. Facing must not jump to a screen edge after the pointer stops moving.
3. Move the cursor slowly across the character's center. Facing should retain its previous direction inside the small dead zone instead of spinning.
4. Move the cursor just outside the dead zone in every direction. Facing should resume immediately without a left/right preference.
5. Move with WASD while aiming in the opposite direction. Movement must not overwrite facing.
6. Rotate the camera with Middle Mouse and repeat the four cardinal directions. The cursor ray must remain correct at every camera yaw.
7. Cast projectile, hitscan, zone, movement, and triggered spells toward all four cardinal directions.
8. Open and close Workshop and Inventory, then repeat the test. Pointer authority must not change after a modal screen.
9. Leave the mouse still for five seconds. The world reticle and facing must stay fixed.
10. Confirm the Console contains no input, ray, GUILayout, or null-reference errors.
