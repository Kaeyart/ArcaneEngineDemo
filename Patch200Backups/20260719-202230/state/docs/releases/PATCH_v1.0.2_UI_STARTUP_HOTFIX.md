# Arcane Engine — v1.0.2 UI Startup Hotfix

This maintenance release fixes the runtime UI initialization failures reported after the v1.0.1 compatibility update.

## Fixed

- Fixed the `No Theme Style Sheet set to PanelSettings` warning by adding `ArcaneRuntimeTheme.tss`, which explicitly imports Unity's built-in default runtime theme.
- Fixed the `RunStartScreen.Awake` null reference.
- Fixed the `V1TitleScreen.Awake` null reference.
- Prevented the same document-hosting fault from affecting the combat HUD or future runtime screens.

## Architecture

- Added `RuntimeUIFactory` as the single creation path for runtime UI Toolkit documents.
- Each screen now owns a dedicated child GameObject and `UIDocument`. This avoids attempting to add multiple `UIDocument` components to the bootstrap GameObject.
- All runtime documents share one Panel Settings instance, guaranteeing consistent scaling, theme assignment, and UIDocument sorting.
- Panel Settings are initialized before a document host is activated, preventing partially initialized panels during `Awake`.

## Compatibility

- Unity Editor: 6000.5.2f1.
- Project version: 1.0.2-demo.
- Includes every v1.0.1 compiler, API, Input System, and batching fix.
- Existing profiles, checkpoints, builds, and save paths remain compatible.
