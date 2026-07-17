# Arcane Engine — The Relic Forge

Unity 6 systems demo · version 1.4.0 · Spellcraft and Spell Links

## Open the project

Use Unity `6000.5.2f1`, open `Assets/ArcaneEngine/Scenes/Main.unity`, wait for package and script compilation, clear the Console, and press Play.

The project uses Unity Input System `1.19.0`. The runtime removes duplicate scene cameras before creating the isometric gameplay camera; this is required for correct mouse aiming and world-space Health bars.

## Version 1.4 focus

- Spell Workshop is now named **Spellcraft**; the permanent hub station remains the **Spell Forge**.
- Every equipped spell has a per-run level from 1 to 5 and Capacity values of `6 / 8 / 10 / 12 / 15`.
- Spell Upgrade room and shop rewards let the player choose which equipped spell gains a level.
- Outer Spell Board cells unlock with spell levels.
- Connected Support Runes consume Capacity; disconnected runes provide no effect and consume no Capacity.
- Ordinary rune connections are universal and directional. The old connector-family restriction is no longer used for construction.
- The available catalog contains 49 Support Runes after retiring seven old trigger/target pieces and adding twelve runtime supports.
- Support Rune shapes were revised to six one-hex, fifteen two-hex, twenty-two three-hex and six four-hex pieces.
- The twelve new supports are Cone, Converge, Ring Cast, Fork, Meteor, Beam, Burning, Shock, Chilling, Close-Range Power, Long-Range Power and Barrier on Cast.
- Spell Links are configured on a separate screen opened with `L` by default.
- Links support On Cast, On Hit, On Kill, On Expire, On Critical Hit and On Status Applied.
- Standard link graphs cannot contain cycles. Links use per-condition Trigger Power and cooldowns and do not consume player Mana or the destination spell's manual cooldown.
- Run checkpoints store spell levels, Link Slots, owned Link conditions and active Spell Links.
- Profile schema 8 migrates retired trigger supports into Link conditions, archives affected layouts, and refunds removed targeting unlocks.

See `PATCH_v1.4.0_SPELLCRAFT_AND_SPELL_LINKS.md` for exact delivered behavior and limitations. See `TESTING_CHECKLIST_v1.4.0.md` for the required Unity Play Mode checks.

## Important controls

- Move: WASD
- Aim: hardware mouse position
- Cast Spell Slots 1 and 2: Left and Right Mouse
- Cast Spell Slot 3: Q
- Dodge: Space
- Interact: E
- Spellcraft: Tab
- Spell Links: L
- Equipment and inventory: I
- Map: M
- Help: F1
- Close panel or pause: Escape

## Validation

Run `Arcane Engine > Validate 1.4.0 Demo` inside Unity. This performs source-driven catalog, Capacity, geometry, Spell Link cycle, curated combination, deterministic layout and item-generation checks.

That command is not a replacement for Play Mode testing. This package was assembled without a Unity Editor executable in the build environment, so the checklist remains the release sign-off authority.
