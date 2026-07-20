# Arcane Engine v0.2.0 — acceptance checklist

## Boot and persistence

- Open with Unity 6000.5.2f1 and confirm there are no compile errors.
- Press Play from `Main.unity`; confirm the Sanctuary appears without scene setup.
- Spend Essence, change preparation, equip items, save options, stop Play Mode, and restart; confirm persistence.
- Switch among all three profiles and confirm independent data.
- Confirm a `.bak` profile is created after the second save.

## Starting progression and preparation

- On a clean profile, confirm only Slot 1 starts filled even though Slots 2 and 3 have prepared choices.
- Buy StartingSpells rank 1 and 2; confirm two, then three slots start filled.
- Select the same standard Core in two slots with one copy; confirm preparation refuses it.
- Forge/select a Relic and confirm a second active Relic is refused.
- Fill Preparation Budget exactly; confirm one more modifier is refused.
- Enable no-equipment and no-modifier contracts; confirm both are voluntary and rewarded.

## Equipment and storage

- Equip all ten equipment positions and begin a run fully equipped.
- Equip a two-handed weapon; confirm the offhand returns to inventory.
- Attempt to equip an offhand while a two-handed weapon is active; confirm refusal.
- Save and load three named equipment loadouts.
- Favorite and lock Armory items; confirm sort and persistence.
- Find an item in a run, die, and confirm it is not banked.
- Find an item, defeat the boss, extract, and confirm it appears permanently.
- Buy a cursed-shop item and confirm its corruption changes stats and presentation.

## Spells and Workshop

- Confirm LMB, RMB, and Q cast Slots 1–3 independently.
- Drag a modifier onto a legal hex; confirm its physical count decreases.
- Place one disconnected; confirm gray inactive state and explanation.
- Rotate, undo, redo, reclaim, save a layout, and reload it.
- Attempt to load a layout without enough physical copies; confirm refusal.
- Put the only copy of a modifier on Slot 1; confirm it is unavailable to Slots 2 and 3.
- Replace a Core; confirm its modifiers return safely.
- Exercise each delivery: projectile, nova, hitscan, beam, meteor, summon, movement, zone, melee, defensive.
- Exercise bounce, return, orbit, split, accelerate, delay, trail, status, reflection, resource, and target-context modifiers.
- Build Slot 1 → Slot 2 → Slot 1 recursion; confirm it terminates and the inspector reports limits.
- Forge Dying Sun, Phoenix Seed, or Hellstorm and confirm the signature remains compatible with modifiers.

## Dungeon and economy

- Complete routes until a boss; confirm finite length, branches, a pre-boss safe option, boss, and extraction.
- Confirm shops do not appear before room 3.
- Confirm a player with empty spell slots gets an early Core route.
- Confirm room rewards present three distinct roles.
- Confirm modifier/equipment/Core pity guarantees prevent long droughts.
- Buy shop stock and the duplication, tempering, cleansing, and fate services.
- Sell a reserve Core and an unbanked item for Drachmas.
- Die and confirm Drachmas disappear while Essence remains.
- Defeat the boss and confirm its Relic Shard is secured immediately.
- Extract and confirm unspent Drachmas and temporary modifiers disappear.

## Enemies, contracts, and feedback

- Verify melee telegraphs, charges, bolts, hazards, shields, healing, adaptation, and elite affixes.
- Confirm the Ossuary Warden changes phases and that its phase contract adds mechanics.
- Toggle every difficulty contract and confirm the reward multiplier changes.
- Confirm unstable-world builds gain instability and adaptive enemies resist repeated elements.
- Lower effect density and confirm recursive builds create fewer visual/audio entities without changing core damage rules.
- Enable reduced flashes, set screen shake to zero, toggle connector palette, and change text scale.
- Rebind Slot 3, dodge, Workshop, and inventory; confirm the new keys work.
- Enter Training, test spells, and leave with Escape without changing run storage.
