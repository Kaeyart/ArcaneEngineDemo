# Arcane Engine Refactor — Phase 1

This package is a stabilization pass for the current public `main` branch. It deliberately avoids a wholesale rewrite of gameplay systems because the project must remain testable in Unity after every structural change.

## Applied changes

### Runtime input

- Replaces manual screen-size/Y-flip calculations in `RuntimeUIDocumentInput` with `RuntimePanelUtils.ScreenToPanel`.
- Uses panel picking and walks up the visual hierarchy so labels inside buttons resolve to their containing `ReliableButton`.
- Restricts fallback input to the top visible `UIDocument`.

### Player movement

- Separates run resource simulation from movement gating.
- Allows movement in the sanctuary/home base.
- Keeps dodge, regeneration, cooldowns, status effects, and combat-only behavior gated by `RunActive`.
- Guarantees `desiredVelocity` is declared in the same scope where it is consumed.

### Generated content

- Makes asset creation recover when an invalid YAML asset already occupies the intended path.
- Adds `Arcane Engine > 2.1 > Repair Generated Content`.
- Adds detection for `m_Script: {fileID: 0}` generated assets.

### Bootstrap and services

- Moves the bootstrap component manifest into one ordered list.
- Resets static services during `SubsystemRegistration`, which matters when Unity domain reload is disabled.
- Makes service registration idempotent and removes stale destroyed Unity object references.

### Developer tooling

- Adds `Arcane Engine > Diagnostics > Run Project Health Check`.
- Adds `Tools/arcane-refactor-audit.sh` for Linux static checks.
- Adds a repository `.editorconfig`.
- Installer creates a complete rollback manifest before modifying files.

## Not included in Phase 1

The large classes (`GameWorld`, `RunDirector`, `DemoUI`, and `V21ProductUI`) are not split in this package. Extracting them safely requires PlayMode verification after each subsystem boundary is introduced. Recommended Phase 2 order:

1. Extract Workshop drag state from `V21ProductUI`.
2. Extract room construction from `GameWorld`.
3. Extract run rewards/shop state from `RunDirector`.
4. Disable or feature-gate legacy UI and diagnostics at bootstrap.
5. Add focused PlayMode tests for boot, sanctuary movement, Workshop placement, and one room transition.

## Verification

After installation:

1. Open with Unity `6000.5.2f1`.
2. Allow script import and compilation to complete.
3. Run `Arcane Engine > Diagnostics > Run Project Health Check`.
4. Rebuild authored content if the health checker reports missing or invalid generated assets.
5. Test: title click, home-base WASD, Workshop open/close, one run start, one spell cast.
