# Arcane Engine Demo — Patch 2.2.5

## Compile Cleanup Hotfix

**Requires:** Patch 2.2.4  
**Target editor:** Unity 6000.5.2f1  
**Patch type:** Compilation and warning cleanup  
**Gameplay balance changes:** None

Patch 2.2.5 fixes the compile error and warnings reported immediately after installing Patch 2.2.4.

### Fixed — Reaction-field lifecycle cleanup

`ElementalReactionField.ClearAll224()` called a method named `Cleanup()`, but this class exposes its stale-reference cleanup as `CleanupList()`.

Patch 2.2.5 changes the lifecycle entry point to call the existing method. Room changes, death cleanup, run termination, and manual spell-effect cleanup can therefore clear reaction fields without producing `CS0103`.

### Fixed — Unity 6000.5 object-search API migration

`SpellEffectLifecycle224` used the deprecated overload:

```csharp
FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)
```

Patch 2.2.5 uses the Unity 6000.5 overload that does not take `FindObjectsSortMode`:

```csharp
FindObjectsByType<T>(FindObjectsInactive.Include)
```

Inactive transient spell objects remain included. The lifecycle cleanup does not depend on result ordering, so removing the sort parameter does not change cleanup behavior.

### Fixed — Unused presentation field

The unused `_lastImportantEvent` field and its cleanup assignment are removed from `MorphologyPresentationDirector21`.

### Not changed

This hotfix does not alter:

- Spell damage.
- Elemental buildup.
- Reaction propagation.
- Chain behavior.
- Spell Links.
- Rune adjacency.
- Persistent-field limits.
- VFX appearance.
- Audio behavior.
- Save data.
