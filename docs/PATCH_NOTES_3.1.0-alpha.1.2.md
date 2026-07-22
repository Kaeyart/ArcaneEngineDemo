# Arcane Engine 3.1.0-alpha.1.2 — Mouse UI Hotfix

- Removes WASD, arrow, Enter, Space, and gamepad frontend navigation from title,
  character-selection, and character-creation screens.
- Keyboard input remains available for text fields, Escape/back behavior, and key rebinding.
- Prevents character-name typing from changing the selected class.
- Prevents confirmation-dialog clicks from passing through to the class cards.
- Prevents rename/delete dialog clicks from activating controls behind the dialog.
- Carries forward the frontend `CS0165` compile repair.
