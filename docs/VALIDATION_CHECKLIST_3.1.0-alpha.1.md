# Validation Checklist — 3.1.0-alpha.1 First Descent

Use Unity 6000.5.2f1. Start with Console Collapse disabled and clear the Console before each phase.

## Compilation and static content

- [ ] Unity completes script compilation with no errors.
- [ ] Run `Arcane Engine > 3.1 > Validate First Descent`.
- [ ] Validator reports PASS and the expected content counts.
- [ ] No missing-script or missing-reference warnings appear.

## Front end and characters

- [ ] Launch enters the new title screen.
- [ ] With no characters, Start Game highlights an empty slot/Create Character route.
- [ ] Create Mage, Ranger and Warrior test characters; class presentation and starter Core differ correctly.
- [ ] Name validation, duplicate-name rejection and final confirmation work.
- [ ] Last-played character is selected by default.
- [ ] Continue loads the last-played character.
- [ ] Rename persists after restart.
- [ ] Delete requires the exact character name and moves the save to recoverable storage.
- [ ] Keyboard, mouse and controller navigation can reach every primary action.

## Refuge and opening

- [ ] New character enters the physical Astral Refuge directly.
- [ ] Character starts at Level 0 with one Core and no gear/Runes/Links/currency/passives.
- [ ] SpellForge Altar advances the first objective.
- [ ] Map Device advances the second objective and offers a free Tier 0 map.
- [ ] Stash, Crafting, Constellation, Character and Training stations open their real interfaces.
- [ ] Training target records telemetry and reset works.

## Maps, combat and rewards

- [ ] Tier 0 opens a real combat room and spawns enemies plus one guardian.
- [ ] Ground equipment/currency/map/Core/Rune pickups can be collected.
- [ ] Guardian Cache appears after all enemies die and must be secured.
- [ ] Completion returns to the Refuge, persists rewards and grants a connected map.
- [ ] Basic Completion and Mastery state persist.
- [ ] Tier 0 through Tier 5 can be completed without exhausting playable maps.
- [ ] Magic/Rare map modifiers change difficulty/reward behavior.
- [ ] Death and abandonment consume the active map but retain the character and collected rewards.
- [ ] Free Tier 0 recovery remains available after failure.

## Items, crafting and Constellations

- [ ] Equipment drops use valid bases, rarity, item level, prefixes/suffixes and requirements.
- [ ] Inventory, stash, equip, unequip, lock and comparison work and persist.
- [ ] Each of the five core crafting verbs succeeds on a valid target.
- [ ] Invalid crafting does not consume currency or partially mutate the item.
- [ ] Six Constellations are visible when their discovery requirements are met.
- [ ] Sequential allocation, point spending, stat updates, Completion Boon Attunement and reset work.

## Persistence and recovery

- [ ] Save indicator reports success and failures visibly.
- [ ] Exit and reload preserve character, level, inventory, equipment, stash, currencies, maps, completion, Mastery, discoveries, SpellForge and Constellations.
- [ ] Multiple characters remain isolated.
- [ ] Corrupting the active JSON in a disposable test copy restores a rotating backup.
- [ ] Playtime and last-played order update.

## Acceptance route

- [ ] Launch → create character → Refuge → starting Core → Tier 0 → guardian/cache → loot → Refuge → equip → craft → allocate Star → Tier 1 → die → reload → retain progression → complete through Tier 5.

Do not mark runtime validation complete unless every applicable item passes in the target editor.
