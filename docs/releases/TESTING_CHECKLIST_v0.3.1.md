# Arcane Engine v0.3.1 — acceptance checklist

## Install and compile

- Close Unity and apply the ZIP with the supplied full terminal command.
- Reopen the same project in Unity Hub with Unity 6000.5.2f1.
- Confirm Package Manager resolves UI Elements and the Console has no red compiler errors.
- Open `Assets/ArcaneEngine/Scenes/Main.unity` and press Play.

## Exact aim and cursor

- Move the cursor to all four screen corners and confirm the software crosshair remains directly under it.
- Circle the player with the mouse and confirm facing/cast direction follows continuously through 360 degrees.
- Rotate and zoom the camera, then repeat the corner and full-circle checks.
- Open Pause, Workshop, Inventory, a reward, and a route choice; confirm the normal cursor returns and gameplay crosshair hides.

## Registration

- In Training, hit each target from front, rear, both sides, minimum range, and maximum useful range.
- Test small, large, fast, split, homing, arcing, weaving, piercing, bouncing, and returning projectiles.
- Confirm the visible projectile body crosses the target when damage registers.
- Confirm fast shots do not pass through targets between frames.
- Confirm explosions and zones damage enemies whose feet are inside the visible combat radius.

## Enemy bars and feedback

- Confirm every bar stays centered above its enemy while it walks, charges, is pulled, changes scale, or the camera rotates.
- Confirm bars do not lag a frame or drift to a fixed world height.
- Confirm normal, elite, Shield, Poison, Frozen, and boss presentations are distinct and readable.
- Disable “Always show enemy Health bars”; confirm full-health normal bars hide and appear when damaged.
- Disable damage numbers; confirm numbers disappear without hiding Health bars.

## HUD and menus

- Confirm the HUD has no clickable gameplay buttons and does not block casting.
- Confirm Health, Mana, Shield, room, kills, Gold, timer, spell details, and cooldowns update live.
- Test at 1280×720, 1600×900, and 1920×1080 in windowed and fullscreen modes.
- Confirm spell cards and top cards do not overlap the crosshair or boss bar.
- Confirm Home Base tabs, rewards, routes, shops, pause, Workshop, inventory, upgrades, and options share the new card/hover/pressed visual language.

## Regression

- Save a profile, Save & Quit mid-run, reopen the project, and Continue Saved Run.
- Confirm the save path and existing profiles are unchanged from v0.3.0.
- Complete and fail one run; confirm Gold, Essence, Legendary Shards, items, Spell Copies, and run snapshot follow their established persistence rules.
