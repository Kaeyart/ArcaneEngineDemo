# Arcane Engine v0.5.0 — DEMO Patch

This is a single in-place milestone patch for Unity 6000.5.2f1. It preserves the project product name and Linux profile path while migrating profile data to version 4 and run checkpoints to version 3.

## 1. Combat feel and control

- WASD movement now accelerates and decelerates instead of snapping between speeds.
- The player faces the exact mouse-ground point; the software crosshair, ground reticle, and short cast-direction line all use that same point.
- Left Shift aim assistance remains optional and never changes ordinary free aim.
- Hits create element-colored fragments, scorch marks, lightning arcs, Frost bursts, damage numbers, camera response, and short heavy-hit/critical hit-stop.
- Enemies have physical hit radii, spell sweep collision, knockback, armor break, elemental weaknesses, stagger thresholds, and visible status text.
- Dungeon cover is now solid for player movement, dodges, teleports, and enemy movement.

## 2. Dungeon exploration

- Rooms build seeded layouts with cover, wall runes, breakable urns/crates/statues, hazards, rotating blade traps, a chest, and a hidden rune switch.
- Props can be destroyed by spells and may release loot.
- Chests and secret compartments are opened in the world with the Interact key.
- Room layouts, objective choice, routes, rewards, and custom seeds are repeatable.

## 3. Room objectives

Combat rooms procedurally choose a real objective with HUD progress:

1. Eliminate all enemies.
2. Survive a timed onslaught.
3. Protect an Arcane Relic from nearby attackers.
4. Destroy summoning portals with spells or interaction.
5. Eliminate marked priority targets.
6. Interrupt a multi-stage ritual.
7. Clear three escalating waves.
8. Escort an orb to the north seal.
9. Hold the central rune.
10. Defeat Tank, Ranged, then Chaser roles in order for bonus experience.
11. Hunt enemies that remain hidden until approached.
12. Escape a collapsing room before repeated damage pulses.
13. Complete a flawless-clear challenge for bonus experience.

## 4. Enemy roster and readable behavior

- Chasers pressure the player directly.
- Bulwarks and Wardens act as armored Tanks.
- Hexers and Mirrors maintain range and fire projectiles; Mirrors use volleys.
- Chargers telegraph a lane before rushing.
- Leeches heal wounded allies.
- Assassins reposition behind the player and telegraph an ambush strike.
- Controllers create persistent denial zones.
- Elites can be Frenzied, Shielded, Volatile, Vampiric, Resistant, or Summoners; every modifier has a runtime effect.
- Health bars are now a single synchronized UI Toolkit presentation anchored from the live renderer bound. Normal, elite, status, shield, and boss presentations no longer compete with a second world-bar system.

## 5. Spell building during a run

- Tab opens the full hex-board Workshop at any point in a run and pauses safely.
- Every spell keeps its own board and consumes its own physical modifier copies.
- A modifier pickup can be collected with a tap or quick-installed by holding Interact.
- Quick-install first searches the board of the spell most recently cast, then the other equipped spells, and only accepts a connected legal position.
- Base Spell copies can be stored, sold, and installed into empty or occupied spell slots during the expedition.
- Triggered spells, chained casts, recursive limits, Trigger Energy, unique foundations, Legendary signatures, and color/behavior transformation remain fully compiled from the live boards.

## 6. Loot, equipment, and sets

- Loot is physical, hover-animated, color coded, and marked by a rarity-height beam.
- Item tooltips show slot, major stats, equipped comparison, and active loot-filter behavior.
- Tap Interact to store an item or hold Interact to equip it immediately.
- Gold and Essence have independent auto-collect toggles.
- The rarity filter automatically sells nearby Common/Magic/Rare loot below the selected threshold; Unique and Relic-tier items are never auto-sold.
- Inventory supports favorites, junk marking, Sell All Junk, direct selling, equipping, unequipping, and all ten equipment positions.
- Warden, Storm, and Ember equipment sets now activate real two-piece and three-piece bonuses.
- Unique equipment still mutates spell foundations and can then be further modified on the normal spell board.

## 7. Physical shops and special rooms

- Shop rooms contain a visible merchant, nine physical offer pedestals, prices, item/service descriptions, purchase feedback, and a physical exit station.
- Merchants can sell Health, modifiers, equipment, Base Spells, board growth, modifier duplication, equipment upgrades, cleansing, and route/reward recovery.
- Safe and Healing rooms contain separate physical Spell Workbench, Equipment Chest, restoration font, and Continue Expedition stations.
- Restoration fonts are single-use per room.
- Post-combat reward rerolls now use a physical reroll shrine beside the reward choices.
- Extraction is now completed at a physical gate that clearly explains which run resources are banked or left behind.

## 8. Boss encounter

- The Dungeon Warden opens with three breakable ward pillars; boss damage is heavily reduced until the pillars are destroyed.
- Phase 2 begins at 67% Health with a Bone Storm rule, denial hazards, a Controller, and a Leech.
- Phase 3 begins at 34% Health with faster patterns, Assassin reinforcements, and resistance to repeatedly used damage types.
- The boss HUD displays phase and the active counter-rule.
- The arena has a dedicated boundary/sigil, phase announcements, sound cues, telegraphs, and guaranteed Unique loot plus a permanent Legendary Shard.

## 9. Walkable Home Base

- Home Base is a playable space rather than an automatically opened menu.
- Move with WASD and interact with physical stations for the Dungeon Gate, Starting Loadout, Spell Archive, Armory, Permanent Shrine, Collection, Training/Options, Apprentice Trial, and Memory Crystal.
- The Memory Crystal writes the durable profile and displays the actual Linux save directory/status.
- Existing tabs remain available only after using the matching station, keeping management screens out of the action loop.

## 10. Guided onboarding

- The Apprentice Trial is a playable tutorial entered from Home Base.
- It teaches movement, mouse aiming/casting, dodging, physical loot collection, hold-to-install, opening the spell board, and casting the changed spell.
- Tutorial prompts are displayed in the combat HUD and completion is permanently saved per profile.
- Training targets use the real compiler, equipment mutations, Trigger Energy, visuals, and termination guards without risking run loot.

## 11. Run progression and replay modes

- Enemies and rooms award run experience.
- Each run level gives a small baseline power increase, restores resources, and spawns three physical perk choices.
- Perks include Spell Power, Fast Casting, Vitality, Mana Efficiency, Critical Focus, Movement, and Trigger Capacity; ranks stack for the current run.
- Perk ranks and temporary bonuses survive Save & Quit through the upgraded checkpoint.
- Standard mode ends after the final boss and extraction.
- Daily Challenge uses the UTC calendar seed and records the profile's best room count.
- Custom Seed accepts a repeatable numeric Standard-run seed.
- Endless unlocks after a successful extraction, places a boss every five rooms, and offers extraction or continuation after each boss.
- The end-of-run recap reports mode, seed, rooms, level, kills, Essence, damage dealt/taken, critical hits, dodges, and the highest-damage spell.

## 12. Audio and atmosphere

- Runtime-generated music and ambience adapt between Home Base, exploration, combat, and boss intensity.
- Boss phases produce distinct audio cues.
- Spell cast/impact sound respects effect density and the Master/Combat Effects mix.
- Master, Music, Combat Effects, Ambience, and UI volume controls are saved per profile.

## 13. UI, HUD, and accessibility standard

- The combat HUD now includes Health, Mana, Shield, three live spell cards, cooldowns, room/mode/seed data, Run Level/XP, objective progress, recent-room breadcrumb map, interaction details, centered announcements, tutorial prompts, and a dedicated boss bar.
- Enemy bars are synchronized in LateUpdate and use a safe fallback width on their first layout frame.
- Physical interactions display the rebound Interact key instead of hardcoded `E` text.
- Starting Run UI supports Standard, Daily, Endless, Custom Seed, three starting slots, stored spell copies, and the one-active-Legendary rule.
- Accessibility includes UI scale, effect density, screen shake, reduced flashes, connector palette, optional aim assistance, enemy-bar/damage-number visibility, auto-collection, complete audio mix, resolution/fullscreen controls, and key rebinding.

## Save and compatibility notes

- Unity Editor: `6000.5.2f1`.
- Product name remains `Arcane Engine - The Relic Forge`, preserving `~/.config/unity3d/OpenAI/Arcane Engine - The Relic Forge/ArcaneEngineProfiles`.
- Profile migration is forward-safe and keeps old profiles, spell archives, items, loadouts, settings, and progression.
- Run snapshot migration accepts older checkpoints and normalizes missing v0.5 fields.
- The installer creates a timestamped full project backup before replacing files.
