# Arcane Engine Demo 2.1 — Promise Completion Source Candidate

Unity editor: **6000.5.2f1**. Open `Assets/ArcaneEngine/Scenes/Main.unity` after import.

This archive is cumulative: it contains the complete project source carried forward from 2.0 plus the 2.1 implementation work. It is not a binary build. The archive was statically parsed and structurally checked outside Unity; Unity compilation, Play Mode tests, hardware checks, visual review, audio review, clean-clone verification and profiler capture must be run in Unity before this candidate can be called an accepted release.

## First import

1. Open the project with Unity 6000.5.2f1.
2. Allow Package Manager and script compilation to finish.
3. Open `Assets/ArcaneEngine/Scenes/Main.unity`.
4. Run `Arcane Engine > 2.1 > Rebuild Authored Content` once if the generated `Resources/V21Content` assets are absent.
5. Run `Arcane Engine > 2.1 > Validate Promise Completion Source`.
6. Open Test Runner and run Edit Mode, then Play Mode tests.

## Important design locks

- Equipment cannot be changed during an active expedition.
- Unsecured Run Bag equipment can be salvaged during a run; its materials are secured only through the normal extraction rules.
- Spell Workshop editing is locked while combat is active and unlocks after room clearance.
- Spell Links are independent of Support Rune adjacency.
- Rune placement is controlled by shape, rotation, board bounds, Capacity, ownership and spell compatibility.

See `IMPLEMENTATION_STATUS_2.1.0.md` for the candid requirement ledger and `PLAY_MODE_ACCEPTANCE_2.1.0.md` for the verification procedure.

