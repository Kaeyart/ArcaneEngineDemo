# Arcane Engine v1.4.0 — Unity verification checklist

Use Unity `6000.5.2f1`. Open `Assets/ArcaneEngine/Scenes/Main.unity`, clear the Console, and run `Arcane Engine > Validate 1.4.0 Demo` before Play Mode testing.

## Compilation and startup

- [ ] Unity compiles with zero errors.
- [ ] The title footer reads `VERSION 1.4.0-DEMO`.
- [ ] Continue, New Expedition and Home Base remain usable.
- [ ] Exactly one active gameplay camera exists after startup.
- [ ] Hardware mouse aiming, enemy Health bars and screen projection remain correct.

## Spell levels and Capacity

- [ ] Each spell starts at level 1 with 6 Capacity.
- [ ] A Spell Upgrade reward opens the spell selector.
- [ ] Only the selected spell levels up.
- [ ] Capacity progresses through 6, 8, 10, 12 and 15.
- [ ] A level-5 spell cannot be upgraded again.
- [ ] Sealed cells reject placement and show their required level.
- [ ] Newly unlocked cells accept valid placements without moving existing runes.
- [ ] A checkpoint restores all three spell levels.
- [ ] A finished or failed run does not store temporary spell levels permanently.

## Spellcraft interaction

- [ ] Tab opens Spellcraft and the panel stays open until deliberately closed.
- [ ] A Support Rune can be dragged from the list to the board.
- [ ] Q, E and Mouse Wheel rotate the dragged rune.
- [ ] An installed rune can be moved and right-clicked to remove it.
- [ ] Green preview means connected and valid.
- [ ] Red preview explains overlap, locked cells, insufficient Capacity, incompatibility or disconnection.
- [ ] Before/after values update while dragging without changing the saved board.
- [ ] Undo and redo restore layout, Capacity and compiled behavior.
- [ ] Disconnected runes are gray, provide no effect and consume no Capacity.
- [ ] An over-Capacity move or placement leaves the previous build unchanged.

## New Support Runes

- [ ] Cone fires forward-spread projectiles.
- [ ] Converge starts projectiles apart and meets near the cursor.
- [ ] Ring Cast fires around the caster.
- [ ] Fork creates two child projectiles after a hit.
- [ ] Meteor produces a delayed strike at the aimed position.
- [ ] Beam produces three short line-damage ticks.
- [ ] Burning deals visible Fire damage over time.
- [ ] Shock increases later incoming damage.
- [ ] Chilling slows and repeated applications freeze.
- [ ] Close-Range Power is stronger near the cast origin.
- [ ] Long-Range Power is stronger beyond ten metres.
- [ ] Barrier on Cast grants Ward only from manual casts.

## Spell Links

- [ ] L opens and closes Spell Links without also casting or moving.
- [ ] A Link reward offers three conditions.
- [ ] A link can be created from two different equipped spells.
- [ ] On Cast activates only from a manual source cast.
- [ ] On Hit activates once for one multi-projectile source cast.
- [ ] On Kill uses the defeated enemy position.
- [ ] On Expire works for projectiles, zones and familiars.
- [ ] On Critical Hit activates only from a critical hit.
- [ ] On Status Applied activates when the source applies a status.
- [ ] Triggered casts consume no Mana and do not block manual destination casting.
- [ ] The per-link cooldown prevents an immediate second activation.
- [ ] Slot 1 → Slot 2 → Slot 3 can complete as a non-cyclic chain.
- [ ] Slot 3 → Slot 1 is rejected when it would close that cycle.
- [ ] Removing a link leaves both Spell Boards unchanged.
- [ ] Link Upgrade stops at three slots.
- [ ] A checkpoint restores Link Slots, conditions and active links.

## Migration

- [ ] Back up an existing pre-1.4 profile before testing migration.
- [ ] The profile loads with no exception.
- [ ] Ordinary Support Runes remain owned.
- [ ] Old trigger unlocks appear as equivalent Link conditions.
- [ ] Old trigger/target runes are absent from normal rewards.
- [ ] Affected saved layouts remain in `retiredSpellLayouts` and active copies contain no retired pieces.
- [ ] Removed target unlocks refund 18 Essence each exactly once.

## Regression pass

- [ ] Complete at least three combat rooms and one shop.
- [ ] Room rewards, three route doors and E interactions still work.
- [ ] Equipment and inventory can be changed during a run.
- [ ] Save and Continue restore the room-entry checkpoint.
- [ ] No continuous exception, warning flood, duplicate AudioListener or background hum appears.
- [ ] Frame time remains stable with a Beam, Meteor, familiar projectiles and a three-spell Link chain active.
