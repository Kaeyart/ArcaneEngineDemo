# Arcane Engine — v1.0.6 Performance, Aim, UI, and Health Bar Hotfix

This in-place release addresses the four gameplay problems reported after v1.0.5.

## Mouse aim

- Unity Input System mouse coordinates are now the authoritative live pointer position whenever a mouse device is available.
- The intermittent IMGUI pointer cache is retained only as a device-loss fallback and can no longer freeze the crosshair or aim ray at an old location.
- Player facing remains independent from WASD movement and continuously follows the cursor-to-ground aim point.
- The software crosshair and spell targeting now read the same live source.

## UI stability

- Keyboard and mouse press/release transitions are consumed exactly once.
- Input System and IMGUI can no longer replay the same physical press on adjacent frames.
- Workshop, equipment, map, help, pause, interaction, and mouse-driven screens no longer open and immediately close from a duplicated transition.

## Enemy Health bars

- The combat HUD now runs after the camera's LateUpdate, eliminating the one-frame camera mismatch that made bars appear to trail enemies.
- Health-bar transforms use the current camera and current renderer bounds every rendered frame.
- Off-screen and behind-camera bars are hidden instead of floating at invalid screen positions.
- Enemy view membership is synchronized at a controlled cadence while Health and Shield values remain responsive.
- LateUpdate no longer allocates a temporary copy of the entire enemy-bar dictionary.

## Performance

- World interactions use a live registry instead of scanning every object in the scene every frame.
- Enemy projectiles use a live Persistent Spell Zone registry instead of performing one scene search per projectile per frame.
- Projectile collision with Breakable Props and Objective Nodes uses live registries rather than repeated global object searches.
- High-frequency HUD text and off-screen indicator work is rate-limited while aim and world anchoring remain frame-accurate.
- The 2,000-seed dungeon stress test no longer runs when Play is pressed. Full stress validation remains available through `Arcane Engine > Validate 1.0 Demo` and runs before release builds.

## Compatibility

- Includes every release from v1.0.0 through v1.0.5.
- Existing profiles, settings, run checkpoints, stored spells, and equipment remain compatible.
- Target Editor remains Unity 6000.5.2f1 on Linux.
