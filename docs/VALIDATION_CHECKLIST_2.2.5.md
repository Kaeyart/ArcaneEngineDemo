# Patch 2.2.5 Unity Validation Checklist

1. Close Unity before installing the patch.
2. Reopen the project with Unity 6000.5.2f1.
3. Confirm the project compiles without the reported `Cleanup` error.
4. Confirm the Console no longer reports obsolete `FindObjectsSortMode` warnings from `SpellEffectLifecycle224.cs`.
5. Confirm the Console no longer reports `_lastImportantEvent` as an unused field.
6. Run `Arcane Engine > 2.2.5 > Validate Compile Cleanup Hotfix`.
7. Enter Play Mode, create reaction fields, then change rooms and die.
8. Confirm reaction fields and transient spell presentation are still removed correctly.
9. Re-test Rune adjacency and equipped Spell Links to confirm Patch 2.2.4 behavior remains intact.
