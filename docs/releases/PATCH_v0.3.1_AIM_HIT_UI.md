# Arcane Engine v0.3.1 — Aim, Hit & UI Patch

## Aiming

- Removed the hidden room-edge clamp from the mouse aim point.
- The player now faces the exact ground-plane point under the cursor through the full camera view.
- Replaced the old world marker with a crisp software crosshair placed at the real screen-space mouse position.
- The operating-system cursor is hidden only while actively aiming and automatically returns for menus and modal screens.

## Hit registration

- Unified projectile rendering and collision on the same combat-plane trajectory.
- Arcing and weaving shots now sweep collision along their visible sideways path instead of testing an invisible straight centerline.
- Continuous segment collision remains active between frames, protecting fast projectiles from tunnelling through enemies.
- Enemy hit radii now come from live collider bounds instead of broad fixed guesses.
- Projectile hit radii now follow visible spell size, reducing both false misses and hits that looked too far away.
- Explosions, persistent zones, status spread, pulling, splitting, bouncing, and expiry consistently use planar combat distance and the resolved impact position.

## Enemy and boss Health presentation

- Replaced IMGUI world bars with a dedicated UI Toolkit overlay.
- Enemy bars anchor to the top of the actual renderer bounds and update after world movement each frame.
- Normal targets receive compact Health bars; elites add names and affixes; Shield and status information have separate readable channels.
- Bosses use a centered top-screen boss bar instead of a floating world bar.
- The “Always show enemy Health bars” option is respected; when disabled, undamaged normal targets remain clean until hit.
- Damage numbers use the same screen-space projection and accessibility setting as the new HUD.

## Combat HUD

- Added a passive, scalable combat HUD built with Unity UI Toolkit.
- Top-left player card: Health, Mana, conditional Shield, and compact control reminder.
- Top-right run card: room name, room count, kills, Gold, and optional room timer.
- Bottom spell tray: LMB/RMB/Q binding, spell name, damage type, damage, Mana cost, delivery type, elemental accent, and live cooldown.
- The combat HUD contains no buttons and never captures gameplay mouse input.
- Resolved-width fallbacks protect first-frame and resolution-change layout.

## Menu presentation

- Replaced Unity's default button appearance with consistent action cards, hover feedback, pressed states, padding, and readable typography.
- Updated tab styling, panel density, secondary text contrast, and spacing across Home Base, rewards, routes, shops, pause, Workshop, inventory, upgrades, and options.
- Kept interactions only where the player must make an actual choice; ordinary gameplay information is now passive HUD presentation.

## Compatibility

- Remains an in-place patch for the existing project path.
- Keeps the existing product name so Linux profile and run-save locations do not move.
- Adds Unity's built-in UI Elements module to the package manifest.
- Project version remains Unity 6000.5.2f1 and application version is now 0.3.1.
