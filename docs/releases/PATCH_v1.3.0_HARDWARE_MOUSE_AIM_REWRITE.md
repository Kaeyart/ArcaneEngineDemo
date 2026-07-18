# Arcane Engine v1.3.0 — Hardware Mouse Aim Rewrite

## Removed

- Removed combat aiming from the shared `ArcaneInput.MousePosition` compatibility path.
- Removed IMGUI pointer-cache involvement in combat aim.
- Removed Editor/standalone pointer-source switching from combat aim.
- Removed the viewport clamp that could pin a ray to a screen edge.
- Removed the 14-pixel center dead zone.
- Removed the forced minimum world aim distance.
- Removed retained-direction correction outside the exact zero-vector case.
- Removed player-facing interpolation and the obsolete Smooth Player Turning option.
- Removed movement-owned player rotation.
- Removed the old `PlayerController.UpdateAim()` implementation and `ArcaneInput.TryGetPointerRay()` helper.

## New hardware aim system

- Added a dedicated `HardwareMouseAim` component with an explicit execution order between camera tracking and player actions.
- Samples `UnityEngine.InputSystem.Mouse.current.position` directly once per LateUpdate.
- Rejects only missing devices and non-finite values; it does not clamp or freeze at screen boundaries.
- Casts exactly one camera ray against the shared combat plane.
- Produces one exact `WorldPoint` and normalized `WorldDirection`.
- `PlayerController` copies that result and applies rotation immediately with no smoothing.
- Manual spell direction and targeted spell position use the same hardware result.
- Home Base, training, and dungeon gameplay share the same component; modal UI suspends application without changing the sampled source.

## Compatibility

- Existing controls, profiles, checkpoints, spell boards, equipment, progression, and saves remain compatible.
- The old serialized Smooth Player Turning value is retained only so existing profile JSON remains readable; it no longer affects aiming.
