# Arcane Engine — v1.0.1 Compatibility Hotfix

This maintenance release prepares the 1.0 demo for Unity 6000.5.2f1 without changing the intended game balance, content, progression, or save location.

## Fixed

- Fixed `CS0165` in the custom-seed start flow by giving the parsed seed a safe initial value before conditional parsing.
- Removed the unused loadout variable reported by `CS0168`.
- Replaced every deprecated `FindObjectsByType(...FindObjectsSortMode.None)` call with Unity 6.5's current unsorted overload.
- Replaced deprecated `FindFirstObjectByType` calls with `FindAnyObjectByType` where no ordering guarantee is required.

## Modernized

- Migrated all keyboard, mouse-button, pointer, wheel, and mouse-delta reads away from the deprecated Input Manager to Unity Input System 1.19.0.
- Added `ArcaneInput`, a null-safe input adapter that preserves existing profile `KeyCode` bindings and converts them to current Input System controls at runtime.
- Set the project to use the new Input System exclusively and added the package/assembly references required by Unity 6000.5.
- Disabled deprecated Dynamic Batching. The demo continues to share meshes and materials and remains ready for GPU-instanced or SRP-based rendering in a later visual-production milestone.

## Compatibility

- Unity Editor: 6000.5.2f1.
- Input System package: 1.19.0.
- Project version: 1.0.1-demo.
- Existing profiles, run checkpoints, and save paths remain compatible.
