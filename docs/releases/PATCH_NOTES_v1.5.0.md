# Arcane Engine Demo v1.5.0 — Loadout & Spellcraft

Version 1.5.0 restructures equipment around a permanent starting loadout and separates it from loot found during an expedition. Spellcraft remains a run system, but its editing controls now respect room danger instead of allowing changes during combat.

These notes describe the code included in this archive. They do not claim final art, final balance, or production UI assets.

## Equipment loadout rules

- All ten equipment slots are prepared at Home Base.
- The complete equipped loadout is locked when an expedition starts.
- Equip, unequip, loadout restore, sale, and Forge operations are rejected by the equipment code while a run is active—not merely hidden in the interface.
- The run-start screen validates the loadout before entering the dungeon.
- A two-handed weapon cannot coexist with an Offhand item. Equipping one at Home Base displaces the other, while an invalid migrated loadout is blocked at run start with a reason.
- No reserve equipment is carried into an expedition.

## Permanent Stash and Unsecured Run Bag

- The old shared runtime backpack has been separated into:
  - **Permanent Stash:** secured, unequipped profile items available at Home Base.
  - **Unsecured Run Bag:** items found or purchased in the active expedition.
  - **Equipped snapshot:** the permanent items worn when the run began.
- Run Bag items cannot be equipped or forged.
- Item tooltips now state `PERMANENT` or `UNSECURED` ownership.
- Favorite and junk changes on run items are stored in the run checkpoint.
- The Run Bag is unlimited in this demo version.

## Extraction, defeat, and save migration

- Successful extraction transfers every Run Bag item to the Permanent Stash.
- Unsecured Forge Dust, Binding Runes, and Corruption Cores become permanent profile resources on extraction.
- Defeat or abandonment removes unsecured equipment and Forge materials while preserving the starting loadout, previously secured items, and death-safe Essence.
- The run recap reports the exact item and Forge-material totals secured or lost.
- Profile save data advances to version 9. Run checkpoint data advances to version 8.
- A legacy active-run backpack is migrated into the Unsecured Run Bag without banking it.
- Existing item IDs, affixes, tiers, rolls, quality, corruption, favorite, junk, and protection flags remain serialized.

## Hybrid equipment rewards

- Ordinary room rewards now focus on current-run power: Support Runes, Spell Upgrades, Spell Links, recovery, Gold, and other immediate effects.
- Equipment remains a guaranteed primary choice in dedicated Equipment Reward rooms.
- Enemies can drop equipment separately from the central room reward.
- Bosses drop a guaranteed Unique item in addition to their Spell Upgrade reward and Legendary Shard.
- Dungeon merchants list equipment as unsecured stock, separate in wording from immediate run services.
- Reward and shop equipment is fully generated before display. The shown rarity, affixes, rolls, corruption, and Unique rule are the instance the player receives.
- Equipment reward generation uses stable run, room, reward, and content identities.
- Cursed-shop equipment receives a deterministic, valid corruption result before it is displayed.

## Immediate salvage

- Hold Interact on ground equipment to salvage it instead of taking it.
- Run Bag items have a dedicated Salvage action.
- Salvage grants a small amount of run-only Gold plus unsecured Forge materials.
- Common, Magic, Rare, Unique, high-level, and corrupted item states produce different material results.
- Rare, Unique, favorite, protected, or corrupted Run Bag items require a second confirmation in the inventory.
- The bulk action only salvages marked Common or Magic junk that is not favorite, protected, or corrupted.
- Every successful salvage is written to the next run checkpoint. Materials are still lost if extraction fails.

## Home Base Armory

- The equipment screen is divided into loadout statistics, ten equipped slots, and the Permanent Stash.
- Selecting an equipment slot also filters the Stash to compatible items.
- Stash tools include text/affix search, slot filtering, minimum rarity, sorting, and All/Favorites/Junk/Unique/Set views.
- Explicit Equip, Unequip, Favorite, Protect Item, and Mark as Junk controls are available without drag-and-drop.
- Three named equipment loadouts can be saved and restored.
- Failed or incomplete loadout restores roll back to the previous equipment state.
- Set bonuses and current loadout statistics are visible beside the equipment slots.

## Exact item comparisons

- The old generic item-power score is no longer used by the item interfaces.
- Comparisons simulate the candidate in the real equipment slot and show exact character-stat changes.
- Two-handed/Offhand consequences and Unique rules are listed separately.
- Ground equipment shows a shortened comparison against the locked loadout.
- The Run Bag, Armory, and Forge show the full comparison.
- Spell-specific conditional affixes remain listed rather than being converted into a misleading universal DPS score.

## Dedicated Equipment Forge

- A new Equipment Forge station and Home Base tab contain all equipment crafting.
- Forge Dust, Binding Runes, and Corruption Cores now have permanent secured wallets.
- All eight existing operations are shown directly:
  1. Upgrade Rarity
  2. Add Affix
  3. Reroll Affix
  4. Improve Roll
  5. Remove Affix
  6. Protect Affix
  7. Add Quality
  8. Corrupt
- Every action displays its full secured-material cost.
- Illegal actions are disabled with a plain-language reason.
- Missing materials are shown before input is accepted.
- Remove Affix, Corrupt, and permanent dismantling use confirmation steps.
- Failed crafting restores the complete previous item state and refunds all materials.
- Permanent Stash items may be dismantled for permanent Forge materials. Equipped items must be unequipped first.

## Item-system correctness changes

- Functional item rarity is now Common, Magic, Rare, or Unique. The unused `RelicTier` value was removed.
- The unused `affixBudget` field and unused `ElementalAdaptation` mutation were removed.
- Standard procedural drops use validated base/set definitions instead of every legacy fixed item.
- Legacy fixed Magic and Rare definitions preserve their minimum rarity when used as set bases.
- Local flat Life, Mana, and Armor affixes scale with the item's upgrade and quality factor; global and percentage affixes do not receive that local scaling.
- Crown of Impossible Angles now has a matching implemented rule: 45% less final Spell Overload.
- Unique effects remain legal duplicate drops; copies are not automatically converted.
- Existing Warden, Storm, and Ember set bonuses remain active and are described in the Armory.

## Spellcraft danger lock

- The Spell Board and Spell Links may be inspected during combat but are read-only.
- Editing unlocks after the encounter ends and no enemies or reinforcements remain.
- The lock is enforced in the gameplay methods for Rune quick-install, Spell Core replacement, Spell Upgrades, and Spell Link creation—not only by disabled buttons.
- Read-only screens display the reason the room is not yet safe.
- Closing an editable spell screen saves the run checkpoint.
- Existing Health, Mana, cooldown, status, Trigger Energy, and instability state is not refreshed by opening or editing the board.

## Spell Board interface

- The existing hex shapes, rotation, Capacity, connectors, compatibility rules, placement validation, Undo, and Redo remain intact.
- The screen now separates the equipped-spell tabs, board, available Support Runes, selected-Rune details, and compiled-spell summary more clearly.
- Support Rune entries show functional name first, flavor name second, owned/available copies, Capacity cost, footprint size, compatibility, and effect wording.
- Placement previews show valid, invalid, disconnected, overlap, and locked-cell states.
- Live comparison distinguishes damage per hit from theoretical multi-hit total.
- Mouse wheel and Q/E rotation remain available. Click placement remains available as an accessibility alternative to drag placement.
- Stored Spell Copies can replace an equipped Spell Core from Spellcraft only when the room is safe.

## Spell Links interface

- The separate Spell Links system remains in place with On Cast, On Hit, On Kill, On Expire, On Critical Hit, and On Status Applied conditions.
- The editing flow is presented as Source Spell → Condition → Destination Spell.
- Active links display direction, trigger condition, Trigger Power, cooldown, and Ready/Recovering state.
- Link creation and removal respect the same danger lock as the Spell Board.
- Existing slot limits, Trigger Energy, cooldown, activation caps, and cycle prevention are unchanged.

## Wording and help

- Player-facing text now consistently uses Permanent Stash, Unsecured Run Bag, secured materials, Support Runes, Spellcraft, Spell Links, Gold, and Home Base.
- Extraction, failure, shops, safe rooms, the Home Base stations, and help text explain when equipment and Forge materials can be used or lost.
- The title/build label now reports version 1.5.0-demo.

## Deliberate limits of this patch

- The Armory uses explicit click controls; equipment drag-and-drop is not included in this archive.
- Spell Links use readable cards and directional text, not a final animated node-graph renderer.
- Forge actions expose costs and legal-state reasons, but randomized affix creation does not preview one guaranteed result before committing.
- Stash capacity, item identification, automatic loot-filter salvage, new equipment slots, new set families, and mid-run equipment swapping are not included.
- No final art, icons, animation, audio, or final UI Toolkit conversion is claimed.
- Source syntax and static contract checks were run for this package. Play Mode behavior still requires verification inside the user's Unity 6.5.2f1 editor and existing full project.
