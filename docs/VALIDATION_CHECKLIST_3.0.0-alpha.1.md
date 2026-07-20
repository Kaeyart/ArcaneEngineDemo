# Arcane Engine 3.0.0-alpha.1 Validation Checklist

## Compilation

- Open with Unity 6000.5.2f1.
- Allow script compilation and domain reload to finish.
- Confirm that the Console contains no compile errors.
- Run `Arcane Engine > 3.0 > Validate Endgame Foundation`.

Expected content totals:

- Classes: 3.
- Ascendancies: 9.
- Constellations: 39.
- Constellation nodes: 390.
- Atlas maps: 80.
- Standard tiers: 40.
- Item bases: 29.
- Tiered affixes: 56.
- Map affixes: 12.
- Currency actions: 13.

## Fresh character

- Enter Play Mode.
- Confirm the class-selection window appears.
- Select Mage.
- Confirm Level 0.
- Confirm no ARPG items or currency.
- Confirm one active Spell Core.
- Confirm no owned Support Runes or Link conditions.
- Repeat on a reset profile for Ranger and Warrior.
- Confirm the starter-core lookup chooses a catalog Core or safely falls back to Fireball.

## Tier 0 map

- Press F7.
- Open a free Tier 0 map.
- Confirm no map item was required.
- Confirm the refuge was replaced by a combat room.
- Confirm ordinary enemies and one guardian spawned.
- Confirm casting and reactions function.
- Confirm the F7 status strip shows the active map.

## Death and abandonment

- Die during a map.
- Confirm the encounter clears.
- Confirm the player returns to the refuge.
- Confirm no completion or map rewards are granted.
- Open an inventory map and abandon it.
- Confirm the map remains consumed.
- Confirm no effects persist into the refuge.

## Completion and sustain

- Complete a Tier 0 map.
- Confirm persistent experience is granted.
- Confirm at least one item is granted.
- Confirm at least one currency is granted.
- Confirm the first Support Rune is granted when none is owned.
- Confirm a Tier 1 map is guaranteed on first completion.
- Quit and re-enter Play Mode.
- Confirm the rewards persist.

## Atlas

- Confirm each band shows its completion totals.
- Complete both variants of a tier.
- Confirm the correct map IDs are stored independently.
- Complete a White map and confirm Mastery.
- Complete a Blue map as Normal and confirm no Mastery.
- Complete the same Blue map as Magic or Rare and confirm Mastery.
- Complete a Yellow map as Rare and confirm Mastery.
- Complete a Red map as Rare but uncorrupted and confirm no Mastery.
- Complete a Red map as Rare and Corrupted and confirm Mastery.
- Confirm first Mastery grants one Atlas Point.

## Map crafting

- Use Refinement Shards to increase quality.
- Use Elevation on a Normal map.
- Use Alteration on a Magic map.
- Use Chaos on a Rare map.
- Corrupt a map.
- Confirm the corrupted map cannot receive ordinary further modifications.
- Confirm map affixes activate corresponding difficulty flags.

## Character levels

- Gain several levels.
- Confirm experience requirements increase.
- Confirm each level grants one Constellation Point.
- Confirm Health, Mana, and Spell Power change after recalculation.
- Confirm Level 100 stops further level advancement.

## Constellations

- Confirm early Constellations are immediately visible.
- Allocate a Small Star.
- Confirm the prerequisite chain prevents skipping ahead.
- Allocate Medium and Large Stars.
- Complete a Constellation.
- Confirm Attunement is consumed.
- Attempt another completion without enough Attunement and confirm rejection.
- Gain levels or Masteries and confirm Attunement increases.
- Reset passives using Null Orbs.
- Confirm all spent points are refunded.

## Items

- Equip one item in every slot.
- Confirm equipping replaces the previous item in that slot.
- Confirm item implicits and affixes affect persistent stats.
- Confirm Magic items have at most one prefix and one suffix.
- Confirm Rare and Exceptional items respect three-prefix and three-suffix limits.
- Confirm affix families do not duplicate on one item.
- Confirm item-level restrictions affect available tiers.

## Item crafting

- Refine quality.
- Alter a Magic item.
- Augment a Magic item with one open modifier.
- Elevate a Normal item to Magic.
- Elevate a Magic item to Rare.
- Exalt a Rare item with an open modifier.
- Remove a modifier with a Null Orb.
- Replace a modifier with Chaos.
- Reroll values with Reformation and Divine Measure.
- Apply an Essence.
- Apply an Omen.
- Corrupt an item.
- Confirm the corrupted item is sealed.
- Apply a Fracture Rune to a valid modified item.

## SpellForge discoveries

- Complete the first Tier 0 map and verify a Support Rune appears.
- Verify the Rune can be placed and remains connected through physical adjacency.
- Discover a new Spell Core.
- Confirm the Core appears in Stored Spells.
- Discover a Spell Link condition.
- Confirm the condition appears in the Link system.
- Reload and confirm discoveries are restored.

## Ascendancy

- First-complete Tier 9.
- Confirm two Ascendancy Points are granted.
- Choose one of the three class Ascendancies.
- Confirm other Ascendancies cannot be selected afterward.
- Allocate nodes in order.
- Confirm persistent stats change.
- First-complete Tiers 19, 29, and 39 and confirm two points at each milestone.

## Roguelite coexistence

- Return to the original Home Base interface.
- Start a legacy run.
- Confirm the legacy run still starts.
- Confirm Patch 2.2 reaction and lifecycle systems remain active.
- Confirm the new ARPG JSON profile is not deleted by a legacy run.

## Persistence and recovery

- Close the game after several rewards and allocations.
- Confirm `arcane_arpg_300_profile.json` exists.
- Relaunch and verify all progression.
- Corrupt or remove the active JSON file manually in a test copy.
- Confirm the backup file is used.
- Use the Editor reset command.
- Confirm only the ARPG profile is deleted.
- Confirm legacy profile data remains.
