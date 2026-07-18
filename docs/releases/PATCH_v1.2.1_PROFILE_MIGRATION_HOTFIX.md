# Arcane Engine v1.2.1 — Profile Migration Hotfix

## Fixed

- Moved the schema-7 drag-and-drop accessibility migration into `ProfileData.Normalize()`.
- Fixed `CS0103` compiler errors for `loadedVersion` and `accessibility` in `ProfileSystem.cs`.
- Restored the intended backup-loading control flow after checking the rotating profile backups.
- Preserved schema-7 defaults for drag threshold, double-click timing, and click-placement accessibility.

## Compatibility

- Uses the existing profile and run-checkpoint locations.
- Does not delete, reset, or rename saves.
- Installs over the current Arcane Engine project; no new Unity Hub project is required.
