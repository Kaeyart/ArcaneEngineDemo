# Arcane Engine — v1.0.3 Menu Flow Hotfix

This maintenance release fixes the title screen becoming a dead end and removes the repeated AudioListener warning flood reported in Unity 6000.5.2f1.

## Menu flow

- Added `ReliableButton`, which retains normal UI Toolkit click handling and also supports direct activation from the Input System.
- Added a per-document pointer fallback that resolves the real mouse position against visible button bounds. This keeps title, profile, options, and starting-spell buttons usable if Unity's UI Toolkit dispatcher misses Linux pointer clicks.
- Suppressed the corresponding delayed Toolkit click after a fallback activation, preventing actions such as New Expedition from firing twice.
- Added title keyboard routes: Enter or 1 opens New Expedition; 2 or Escape enters Home Base.
- Added visible shortcut instructions on the title screen.
- Updated the displayed title version to 1.0.3-demo.

## Console cleanup

- Disabled stale or editor-created AudioListeners before creating the game camera's single listener.
- The game keeps exactly one active runtime listener while preventing the repeated two-listener warning from flooding the Console.
- The remaining startup validation message is intentional and confirms that combat, catalogs, spell compilation, deterministic generation, and automated layout tests passed.

## Included fixes

- Includes the v1.0.1 Unity 6.5/Input System compatibility work.
- Includes the v1.0.2 runtime theme and unique UIDocument-host fixes.
- Existing profiles, checkpoints, and the Linux save path remain compatible.
