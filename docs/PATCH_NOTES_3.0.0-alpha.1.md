# Arcane Engine Demo
# Patch 3.0.0-alpha.1 — Endgame-First ARPG Foundation

**Status:** Playable architecture alpha

**Requires:** Patch 2.2.5

**Target editor:** Unity 6000.5.2f1

## Purpose

This release begins the transition from a run-first roguelite structure to a persistent, map-first action RPG.

The existing roguelite systems are retained. They remain available through their original Home Base interfaces and can later be formalized as optional Fracture Runs. They are no longer the only intended progression model.

Patch 3.0.0-alpha.1 creates a separate persistent ARPG profile and a functional progression loop:

1. Choose Mage, Ranger, or Warrior.
2. Begin at Level 0 with one minimal Spell Core.
3. Enter a freely generated Tier 0 map.
4. Defeat a scaled map encounter and guardian.
5. Secure persistent experience, equipment, currency, maps, Runes, Cores, and Spell Link conditions.
6. Complete and master Atlas maps.
7. Craft and equip the replacement item system.
8. Allocate modular Constellation passives.
9. Unlock an Ascendancy and spend milestone points.
10. Advance through forty standard map tiers.

This is not the final 3.0 content release. It is the first executable foundation on which the complete ARPG can be built without repeatedly rewriting persistence, map progression, items, currencies, classes, or passive allocation.

---

## New persistent ARPG profile

A dedicated JSON profile is stored separately from the existing legacy progression data.

The profile stores:

- Character identity.
- Base class.
- Ascendancy.
- Level and experience.
- Constellation points.
- Atlas points.
- Ascendancy points.
- Completed and mastered maps.
- Discovered and allocated Constellations.
- Allocated Ascendancy nodes.
- Persistent equipment inventory.
- Equipped items.
- Currency inventory.
- Map inventory.
- Discovered Spell Cores.
- Discovered Support Runes.
- Discovered Spell Link conditions.
- Highest completed map tier.
- Total maps completed.
- Total deaths.

Saving is atomic. The previous valid profile is retained as a backup before the active file is replaced.

The new profile does not delete the existing legacy profile. An Editor command is provided to reset only the new ARPG profile.

---

## Level-zero start

A new ARPG character begins with:

- Level 0.
- No ARPG equipment.
- No ARPG currency.
- No Support Runes.
- No Spell Links.
- One class-appropriate Spell Core selected from the project catalog.
- One SpellForge board.
- No additional active spell slots.

The bridge clears the runtime legacy equipment and modifier inventory when the new character is created. The old profile data remains on disk so the retained roguelite mode is not destroyed.

### Mage

The starter lookup favours Bolt, Fireball, Orb, or projectile-style Cores.

Base persistent bonuses:

- Increased maximum Mana.
- Increased Spell Power.

### Ranger

The starter lookup favours Arrow, Shot, Spear, or projectile-style Cores.

Base persistent bonuses:

- Increased movement speed.
- Increased critical chance.

### Warrior

The starter lookup favours Strike, Slam, Wave, or Nova-style Cores.

Base persistent bonuses:

- Increased maximum Health.
- Increased recovery.

If a class-specific Core cannot be found, the bridge uses the existing Fireball Core as a safe fallback rather than preventing character creation.

---

## Astral Refuge

The existing Home Base environment now also acts as the initial Astral Refuge for the new ARPG loop.

Press **F7** to open the persistent ARPG interface.

The interface contains:

- Character progression.
- Ascendancy selection and allocation.
- Atlas progression and map inventory.
- Constellation allocation.
- Persistent equipment and crafting.
- Spell Core, Rune, Link, and currency discoveries.

Tier 0 maps can always be opened for free. This prevents early progression from becoming permanently blocked by map drought.

---

## Forty standard map tiers

The new standard progression contains forty map tiers.

| Band | Tiers | Primary role |
|---|---:|---|
| White | 0–9 | Foundation and onboarding |
| Blue | 10–19 | Build formation |
| Yellow | 20–29 | Build specialization |
| Red | 30–39 | Endgame optimization |

The initial Atlas content library contains:

- Forty standard tiers.
- Two persistent completion nodes per tier.
- Eighty standard map definitions.
- Four named progression regions.
- Region-specific map names.
- Named guardian identities.

The current runtime maps these definitions onto the project’s existing procedural combat-room library. Future releases can replace or expand individual layouts without changing the persistent Atlas IDs or completion data.

---

## Map items

Maps are persistent craftable items.

Each map records:

- Map definition.
- Tier.
- Seed.
- Normal, Magic, or Rare rarity.
- Corruption state.
- Quality.
- Map affixes.

A map item is consumed when opened.

Tier 0 maps are the exception. They are generated freely by the Atlas interface and do not require an inventory item.

### Map difficulty

Map affixes are translated into the existing `DifficultySettings` runtime.

The first map-affix library includes:

- Frenzied enemies.
- Bulwark enemies.
- Glass Soul.
- Mana Drought.
- Additional elite properties.
- Reduced healing.
- Unstable world.
- Adaptive enemies.
- Timed collapse.
- No legacy starting equipment.
- No legacy starting modifiers.
- Additional boss-phase budget.

Higher map rarity increases the number of affixes and therefore the reward multiplier.

### Map encounter runtime

Opening a map now:

- Consumes the selected map item.
- Clears old room effects.
- Activates the persistent ARPG combat state.
- Builds a deterministic procedural combat room.
- Spawns a tier-scaled enemy group.
- Adds elite enemies at higher tiers.
- Spawns a guardian.
- Applies map difficulty flags.
- Tracks kills and experience.
- Detects completion, death, or abandonment.

Map failure consumes the map and grants no completion rewards.

Map completion secures all generated persistent rewards before returning the player to the Astral Refuge.

---

## Monster scaling

Monster encounter density grows from five ordinary enemies in the earliest maps to a capped high-tier group, plus the map guardian.

Tier is passed into the existing enemy scaling system as encounter depth.

Difficulty grows through:

- Tier-scaled enemy depth.
- Increased pack size.
- More frequent elite enemies.
- Map modifiers.
- Guardian scaling.
- Timed pressure where present.

The alpha deliberately avoids an unbounded density curve. The maximum ordinary pack size is capped to protect gameplay readability and the reaction-propagation budgets introduced in Patch 2.2.

---

## Map completion

Every standard map can award two persistent completion states.

### Basic completion

Defeat the map guardian and clear the encounter.

Basic completion:

- Records the map as completed.
- Advances highest completed tier.
- Grants persistent rewards.
- Guarantees a connected higher-tier map on first completion, until Tier 39.

### Mastery completion

The required map preparation escalates by band:

- White: defeat the guardian.
- Blue: complete while the map is at least Magic.
- Yellow: complete while the map is Rare.
- Red: complete while the map is Rare and Corrupted.

The first Mastery completion grants one Atlas Point.

### Map sustain

The system includes several anti-lockout rules:

- Free Tier 0 maps.
- Guaranteed next-tier map on first completion.
- Additional current-tier or next-tier map-drop chance.
- Persistent Map Sustain modifiers from equipment and Constellations.
- Reward scaling from map quality and affixes.

---

## Persistent character levels

The character level range is 0–100.

Experience is earned from:

- Enemy kills.
- Map completion.
- Map reward multipliers.
- Persistent Experience Gain modifiers.

Every level grants one Constellation Point.

Level also grants baseline persistent Health, Mana, and Spell Power according to class.

The level curve grows progressively rather than using the legacy temporary run-level system.

---

## Three classes and nine Ascendancies

### Mage

- Elementalist.
- Chronomancer.
- Voidcaller.

### Ranger

- Deadeye.
- Stormrunner.
- Warden.

### Warrior

- Juggernaut.
- Spellblade.
- Bastion.

Each Ascendancy currently contains:

- Eight ordered nodes.
- Persistent mechanical stat bonuses.
- A named specialization identity.
- A stable node ID for future bespoke hooks.

Ascendancy Points are awarded in pairs on first completion of:

- Tier 9.
- Tier 19.
- Tier 29.
- Tier 39.

The first Ascendancy may be selected after completing Tier 9 and obtaining the first two points.

Patch 3.0.0-alpha.1 implements the allocation framework and active stat effects. Bespoke mechanics such as projectile routing, stored time phases, weapon imbuement, and barrier conversion remain future Ascendancy content rather than being falsely represented as complete in this alpha.

---

## Constellation passive system

The passive system is modular rather than one connected tree.

The alpha contains:

- Thirty-nine Constellations.
- Ten Stars per Constellation.
- Three hundred ninety allocatable nodes.
- Small Stars.
- Medium Stars.
- Large Stars.
- Completion Boons.
- Level and map-tier discovery requirements.
- Internal prerequisite chains.
- Constellation Point costs.
- Completion Attunement costs.
- Full reset using Null Orbs.

Constellation categories include:

- Elemental.
- Rune.
- Defence.
- Weapon.
- Atlas.
- Class.

### Completion Boons

Completion Boons currently grant a substantial two-stat mechanical package and consume Attunement.

Each Completion Boon also carries a stable identity target for later bespoke combat integration. The target descriptions are deliberately labelled as future identity hooks; the active alpha effect is the displayed mechanical stat package.

This allows the passive economy, allocation, persistence, Attunement limits, and UI to be validated before hundreds of bespoke combat hooks are added.

### Attunement

Attunement limits the number of completed Constellations.

Attunement is gained from:

- Base character capacity.
- Character levels.
- Map Mastery milestones.
- Specific passive and item modifiers.

Constellations cost between one and three Attunement in the initial library.

---

## Replacement itemization foundation

Patch 3.0.0-alpha.1 adds a separate persistent equipment model rather than extending the temporary run-item model indefinitely.

### Equipment slots

- Main hand.
- Off hand.
- Helmet.
- Body armour.
- Gloves.
- Boots.
- Belt.
- Amulet.
- Two rings.
- Relic.

### Initial item bases

The alpha includes twenty-nine bases covering:

- Wands.
- Staves.
- Ritual daggers.
- Bows.
- Crossbows.
- Spears.
- Swords.
- Axes.
- Maces.
- Two-handed weapons.
- Class-aligned off-hands.
- Armour.
- Jewellery.
- Relics.

### Item rarity

- Normal.
- Magic.
- Rare.
- Unique framework value.
- Exceptional.

The initial generator actively produces Normal, Magic, Rare, and Exceptional items. Unique data structures are present, but a curated Unique library is not yet included.

### Affixes

The initial library includes fourteen affix families with four tiers each, for fifty-six tiered affixes.

Affixes include:

- Spell Power.
- Health.
- Mana.
- Critical chance.
- Movement speed.
- Cooldown recovery.
- Mana efficiency.
- Recovery.
- Trigger Energy.
- Item rarity.
- Currency discovery.
- Map sustain.
- Map quantity.
- Attunement.

Affixes enforce:

- Item-level requirements.
- Valid equipment slots.
- Prefix or suffix role.
- Family conflict prevention.
- Weighted generation.
- Prefix and suffix limits.

### Persistent equipment effects

Equipped ARPG items now modify the existing `PlayerStats` pipeline through a single GameWorld integration hook.

The hook applies:

- Class bonuses.
- Level bonuses.
- Allocated Constellations.
- Allocated Ascendancy nodes.
- Item implicits.
- Item affixes.

No old combat formulas are duplicated.

---

## Currency crafting

The alpha includes thirteen persistent currencies and their gameplay verbs.

- Refinement Shard: increases quality.
- Spark of Alteration: rerolls a Magic item or Magic map.
- Rune of Augmentation: adds a missing Magic modifier.
- Sigil of Elevation: upgrades item or map rarity.
- Arcane Exalt: adds a Rare modifier.
- Null Orb: removes one random item modifier and pays passive reset costs.
- Reformation Orb: rerolls item values.
- Chaos Fragment: replaces an item modifier or rerolls a Rare map.
- Elemental Essence: forces a spell-aligned modifier family.
- Omen of Control: directs a modifier addition.
- Corruption Catalyst: irreversibly corrupts an item or map.
- Divine Measure: rerolls values or map reward seed.
- Fracture Rune: marks an item as carrying a protected fractured modifier.

Crafting is performed directly from the F7 interface.

Corrupted items and maps are sealed against ordinary further modification.

---

## SpellForge discoveries

The persistent ARPG profile begins with one Core and no other SpellForge components.

Map rewards can discover:

- Support Runes.
- Spell Cores.
- Spell Link conditions.

The first completed Tier 0 map guarantees the first Support Rune when none is owned.

Higher maps can award additional Runes, Cores, and Link conditions.

Discoveries are restored into the existing SpellForge runtime on load. The existing deterministic Rune adjacency and Link-depth corrections from Patch 2.2.4 remain active.

The current legacy workshop remains the editing surface while the new persistent discovery model is validated. A dedicated 3.0 SpellForge inventory interface remains future work.

---

## Existing roguelite mode

The existing run modes are not deleted.

They remain accessible through the original Home Base interface.

Patch 3.0.0-alpha.1 does not yet merge their temporary reward economy into the persistent ARPG profile. That conversion belongs to a later Fracture Run integration release.

---

## Editor tools

New menu commands:

```text
Arcane Engine
└── 3.0
    ├── Validate Endgame Foundation
    ├── Open Persistent Profile Folder
    └── Reset ARPG Profile
```

The validator checks:

- Three classes.
- Nine Ascendancies.
- At least thirty-nine Constellations.
- At least three hundred ninety Constellation nodes.
- Eighty standard maps.
- Forty standard tiers.
- At least twenty-eight item bases.
- At least fifty tiered affixes.
- At least twelve map affixes.
- Thirteen currency actions.
- Unique content IDs.
- Patch marker.
- GameWorld stat hook.

---

## Known alpha limitations

This release intentionally does not claim to finish the entire ARPG pivot.

The following remain later 3.0 work:

- Bespoke active mechanics for all Ascendancy nodes.
- Bespoke active mechanics for all Constellation Completion identities.
- A curated Unique-item library.
- Dedicated map layouts for all eighty Atlas nodes.
- Regional boss encounters and pinnacle bosses.
- Full resistance, armour, evasion, Ward, penetration, and attribute replacement.
- Loot filtering.
- Vendor economy.
- Dedicated 3.0 SpellForge collection and loadout persistence UI.
- Fracture Run reward integration.
- Atlas specialization tree.
- Multiplayer, trading, leagues, and economy services.

The alpha does implement the persistent structures these systems require. Future releases can add content without replacing the save model or primary progression loop.

---

## Acceptance target

Patch 3.0.0-alpha.1 is successful when a new player can:

1. Create a level-zero class.
2. Enter a free Tier 0 map.
3. Fight and defeat a scaled encounter.
4. Die and correctly lose the map attempt.
5. Complete a map and retain rewards.
6. Receive a connected higher-tier map.
7. Gain persistent levels and Constellation Points.
8. Allocate passive nodes and observe stat changes.
9. Find, equip, and craft persistent items.
10. Discover Support Runes, Spell Cores, and Link conditions.
11. Master maps and gain Atlas Points.
12. Reach Tier 9 and select an Ascendancy.
13. Quit and reload without losing the ARPG profile.
14. Continue using the existing optional roguelite modes.
