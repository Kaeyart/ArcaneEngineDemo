# Patch 2.2.5 Implementation Manifest

## Modified project files

- `Assets/ArcaneEngine/Scripts/Combat/ElementalReactionField.cs`
  - Replaces the invalid `Cleanup()` call in `ClearAll224()` with the class's existing `CleanupList()` method.

- `Assets/ArcaneEngine/Scripts/Spells/SpellEffectLifecycle224.cs`
  - Replaces both deprecated `FindObjectsByType<T>(FindObjectsInactive, FindObjectsSortMode)` calls with `FindObjectsByType<T>(FindObjectsInactive)`.
  - Continues including inactive GameObjects during transient-effect cleanup.

- `Assets/ArcaneEngine/Scripts/Presentation21/MorphologyPresentationDirector21.cs`
  - Removes the unused `_lastImportantEvent` field and its reset assignment.

## Added project files

- `Assets/ArcaneEngine/Editor/Patch225CompileCleanupValidator.cs`
- `Assets/ArcaneEngine/PATCH_2_2_5.txt`
- `docs/PATCH_NOTES_2.2.5.md`
- `docs/IMPLEMENTATION_MANIFEST_2.2.5.md`
- `docs/VALIDATION_CHECKLIST_2.2.5.md`

## Installer behavior

- Requires Patch 2.2.4 and Unity 6000.5.2f1.
- Creates a process-unique timestamped backup.
- Applies source transforms idempotently.
- Verifies all Patch 2.2.5 markers.
- Rejects remaining `FindObjectsSortMode` and `_lastImportantEvent` references in the affected sources.
- Generates an exact rollback script.
