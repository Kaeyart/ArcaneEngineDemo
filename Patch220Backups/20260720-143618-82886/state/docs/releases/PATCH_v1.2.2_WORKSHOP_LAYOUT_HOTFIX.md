# Arcane Engine v1.2.2 — Workshop Layout Hotfix

## Fixed

- Fixed repeated `ArgumentException: Getting control ... position` errors in `DemoUI.DrawWorkshop()`.
- Fixed the resulting `Invalid GUILayout state in GameView` errors.
- Rebuilt the execution and trigger inspector around a constant GUILayout control count.
- Spell graph nodes, execution layers, triggers, validation warnings, and validation errors are now consolidated into stable text controls.
- Live spell recompilation can update inspector contents without invalidating Unity's Layout/Repaint control cache.

## Preserved

- Full behavior-graph node details remain available in the Workshop inspector.
- Trigger costs, inherited power, activation limits, execution layers, validation warnings, and blocked-build explanations remain visible.
- Existing profiles, checkpoints, spell layouts, equipment, and progression remain compatible.
