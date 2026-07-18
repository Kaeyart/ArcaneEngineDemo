# Arcane Engine — v1.0.4 Title Input Hotfix

This release directly addresses the fully rendered but non-interactive title screen shown in the Unity 6000.5.2f1 Game view.

## Fixed

- The title screen no longer sets `Time.timeScale` to zero. Home Base remains blocked by modal state, so the world stays safe while the input/update loop remains fully active.
- Added an IMGUI event-level mouse dispatcher to every runtime document. It maps a mouse release to the UI Toolkit panel, finds the visible `ReliableButton` under that exact rendered point, and invokes its action directly.
- Added IMGUI key handling for Enter, Numpad Enter, 1, 2, and Escape. These shortcuts no longer depend on `Keyboard.current` polling.
- Updated fallback suppression so a Toolkit click and direct click cannot perform the same action twice.

## Expected title controls

- Click `NEW EXPEDITION`, or press Enter/1, to open starting-spell preparation.
- Click `ENTER HOME BASE`, or press 2/Escape, to close the title and move around Home Base.
- Click `CONTINUE SAVED RUN` to restore the valid room-entry checkpoint displayed on the screen.

## Included work

- Includes all v1.0.1 Unity compatibility and Input System fixes.
- Includes all v1.0.2 runtime theme and UIDocument fixes.
- Includes all v1.0.3 menu safety and AudioListener cleanup.
