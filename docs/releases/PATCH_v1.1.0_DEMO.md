# Arcane Engine — Demo Update 1.1.0

## The Forge Reborn

Demo Update 1.1.0 rebuilds the moment-to-moment interaction layer and introduces the first complete action-RPG itemization pass. Aiming, enemy readability, spell construction, equipment, loot decisions, crafting, filtering, save migration, and validation now operate as connected gameplay systems.

---

## Update highlights

- Rebuilt combat around an absolute hardware cursor that aims independently from WASD movement.
- Replaced detached enemy UI with pooled world-following Health and Shield bars.
- Added genuine drag, rotate, move, remove, cancel, undo, and redo interactions to the hex Spell Workshop.
- Added deterministic Common, Magic, Rare, Unique, and Corrupted equipment generation.
- Shipped 48+ standard bases, 84 affix families, five tiers, 24+ spell-reactive families, 18 Uniques, and 12 corruptions.
- Added an eight-action in-run Forge, equipment dragging, six inventory sorts, advanced tooltips, and protected loot filters.
- Added versioned item migration and expanded automated release validation.

---

## 1. Absolute mouse aiming

- The operating-system cursor remains visible, unlocked, and free across the Game view.
- Mouse position is sampled from Unity's Input System every frame; intermittent IMGUI events are fallback input only.
- A camera ray resolves the cursor against the combat plane and supplies one authoritative `AimPoint`.
- Player facing, cast direction, targeted effects, projectiles, beams, movement skills, and the ground marker use that same point.
- WASD controls movement only. Moving in one direction while casting in another is fully supported.
- Middle Mouse camera rotation, wheel zoom, and R camera reset remain separate from aim.
- The obsolete software crosshair is disabled during combat.

## 2. Enemy Health-bar rebuild

- Standard and elite enemies now receive pooled world-space bars.
- Bars are positioned from cached combined renderer bounds after enemy and camera movement.
- Health, Shield, elite name, exact values, and status text share one synchronized view.
- Dead, missing, pooled, and off-run enemies release their bar immediately.
- Bosses retain the dedicated top-screen boss frame.
- Options now include always-show behavior, numerical values, scale, and vertical offset.
- The pool starts with 24 bars and expands only when the active enemy budget requires it.

## 3. Spell Workshop 2.0

- Modifier tiles drag directly from inventory onto the active spell board.
- The complete rotated polyhex footprint previews before a drop.
- Cyan cells indicate a legal drop; red cells and plain-language messages explain invalid placement.
- Installed modifiers can be dragged to a new legal anchor.
- Installed modifiers can be dropped back over inventory to remove them.
- Q/E and Mouse Wheel rotate the active drag; Right Mouse cancels safely.
- Invalid or interrupted drops preserve the original board and inventory quantity.
- Undo and redo cover placement, movement, rotation, and removal.
- Click-to-place, right-click removal, and Shift-click rotation remain available as alternatives.
- Closing the Workshop cancels transient drag state and writes a run checkpoint.

## 4. Board readability and live compilation

- Directional input/output connectors remain visible on every occupied tile.
- Active routing is recalculated from the Base Spell through compatible connector families.
- Disconnected pieces remain installed but explain why they are inactive.
- Execution order, instability, Trigger Energy, damage, Mana, cooldown, projectile count, and mechanic preview update with the build.
- A virtual-center Unique rule can activate compatible first-ring modifiers without bypassing occupancy rules.
- A modifier copy installed on one board is unavailable to the other boards.

## 5. Itemization 2.0

Every generated item now stores independent data for:

1. Equipment slot and base definition.
2. Base family and implicit.
3. Item level and deterministic generation seed.
4. Instance rarity.
5. Prefixes and suffixes.
6. Tier, legal range, rolled value, group, source, and local/global flag.
7. Quality, upgrade state, lock state, favorite/junk state, and banking state.
8. Crafted affix and optional irreversible corruption.

Left and Right Shoulders and Gloves remain independent. Two-handed weapons displace and disable the Offhand slot.

## 6. Rarity, affixes, tiers, and legality

- Common items contain their base and implicit.
- Magic items support up to one prefix and one suffix.
- Rare items support up to three prefixes and three suffixes and may retain open slots.
- Unique items use handcrafted rules rather than random Rare construction.
- Corrupted is an irreversible state layered onto an eligible item.
- The catalog contains 42 prefix and 42 suffix families.
- Every scalable family supplies five numerical tiers.
- Item level controls the best eligible tier and higher tiers remain less common.
- Weighted tags and slot restrictions make bases favor appropriate rolls.
- Duplicate modifier groups and illegal slot/affix combinations are rejected.
- One item can contain at most one crafted affix in this demo.

## 7. Spell-reactive equipment

- Elemental, projectile, area, beam, zone, summon, melee, trigger, and ailment damage can react to the compiled spell.
- Projectile speed, duration, area, homing, chaining, piercing, ailment strength, ailment duration, and trigger recovery change spell behavior.
- Conditional power supports low Mana, high Health, active Shield, recent Dodge, first cast, Mana scaling, and missing-Health scaling.
- Character affixes now drive life/mana regeneration, Shield power, spell leech, elite/boss damage, cooldown-on-kill, pickup range, interaction speed, Gold find, and rarity find.
- Affixes continue to obey projectile, entity, recursion, and Trigger Energy budgets.

## 8. Unique equipment

- The demo now contains 18 handcrafted Unique items across the equipment slots.
- New rules include all-Lightning/no-critical builds, a virtual board connector, excess-healing storage, orbit-then-seek projectiles, a persistent Mana-based Shield, triggered-spell dominance, ailment convergence, a two-handed trigger engine, and linked gloves.
- Orbiting projectiles now orbit briefly and then release toward the exact cursor target.
- Triggered Dominion is evaluated from the actual cast request: manual casts lose power and triggered casts gain power.
- Excess healing is stored and consumed by the next manual spell instead of existing only as a static tooltip bonus.
- Every Unique is compiled against every Base Spell during full release validation.

## 9. Corruption

- Twelve deterministic corruption outcomes can improve, transform, or damage an item.
- Corruption can change quality, resources, movement, trigger behavior, cooldown, instability, or elemental spell power.
- The result is stored as a separate corruption ID and affix source.
- Corrupted items cannot use ordinary Forge operations.
- Uniques and Corrupted ground drops are protected from loot filters.

## 10. In-run Forge

The inventory Forge now provides eight Gold-funded actions:

- Upgrade rarity.
- Add one legal crafted affix.
- Reroll one unlocked affix.
- Improve a roll inside its existing tier.
- Remove one unlocked affix.
- Lock one affix.
- Add quality up to the demo cap.
- Apply irreversible corruption.

Failed actions refund their exact cost without receiving Gold-find bonuses. Successful actions recalculate player/spell stats and save the run checkpoint.

## 11. Loot generation and presentation

- Item generation is deterministic from item level, definition, stable ID, and source seed.
- Rarity-find modifies the instance rarity roll while base minimum rarity is respected.
- Smart-loot weighting favors empty equipment slots and tags used by prepared triggered/caster builds without guaranteeing an upgrade.
- Recent backpack/equipment base IDs receive a strong duplicate-weight penalty.
- Existing room, elite, boss, shop, challenge, secret, and reward-source tables continue to provide different reward pressure.
- Loot beams scale with instance rarity.
- Ground names and descriptions use the actual rolled item rather than the base template.
- Nearby ground labels stack vertically to reduce overlap.
- Incidental ground drops stop spawning at the 24-pickup presentation budget; guaranteed boss loot bypasses the cap.
- Currency remains the only default auto-pickup category.

## 12. Item tooltips and comparison

- Tooltips show rolled name, rarity, item level, slot, base family, quality, implicit, explicit affixes, corruption, and Unique rule.
- Prefixes, suffixes, crafted affixes, locked affixes, and corruption are labeled separately.
- Advanced mode displays tier, legal roll range, local/global behavior, modifier group, and tags.
- The comparison score includes base values, all affixes, quality, item level, upgrades, and Unique value.
- Two-handed weapon comparisons include the value of the displaced Offhand.
- Left/Right equipment slots compare independently.

## 13. Inventory and equipment interaction

- Backpack items can be dragged onto their matching equipment slot.
- Equipped items can be dragged back to the inventory area.
- Invalid slot drops return the item without mutation.
- Click-to-equip and click-to-unequip remain available.
- Favorite, lock, junk, corruption, quality, rarity, and affixes remain attached to the item instance.
- Sorting modes include Newest, Rarity, Item Level, Slot, Value, and Build relevance.
- Visible inventory cards are paged in groups of 18 to cap per-frame IMGUI work.
- Search matches item names and affix text; slot, rarity, and tag filters can be combined.

## 14. Loot filters

- Minimum-rarity, equipment-slot, and build-tag ground filters are saved in the profile.
- Filtered items are hidden rather than destroyed or silently sold.
- Holding Left or Right Alt reveals all filtered ground loot.
- Unique and Corrupted items bypass hiding protection.
- The interaction controller ignores hidden pickups, preventing invisible focus prompts.

## 15. Saves and migration

- Profile schema is version 6 and run checkpoints are version 5.
- Item instance data is version 11.
- Saved affixes include tier, kind, group, legal range, crafted/locked/local flags, and value.
- Saved items include instance rarity, quality, seed, corruption, and crafted-affix identity.
- Legacy flat affixes migrate to the closest catalog family when possible and remain marked Legacy otherwise.
- The first migrated equipment save uses the existing atomic temporary-file, checksum, rotating-backup, and quarantine system.
- Workshop and equipment changes checkpoint only after committed operations or panel closure.

## 16. Performance and reliability

- Input transitions remain deduplicated between Input System and IMGUI fallback paths.
- The live Input System pointer is authoritative.
- Enemy bars use pooling, cached renderer bounds, and no per-frame scene searches.
- Enemy membership sync is throttled while position updates remain exact in `LateUpdate`.
- Inventory card work is page-capped.
- Ground loot stacking uses a maintained active list rather than scene searches.
- Item generation uses deterministic tables without reflection.
- Existing global entity, projectile, trigger, enemy, particle, and pooled-impact budgets remain enforced.

## 17. Accessibility

- Hardware cursor visibility, high contrast, and cursor scale remain configurable.
- The exact world aim marker is always available during active combat.
- Workshop and inventory provide both drag and click interaction paths.
- Beginner and advanced tooltip modes separate readable decisions from technical tiers.
- Enemy-bar visibility, values, scale, and height are adjustable.
- Existing reduced motion, reduced flashes, effect density, colorblind connectors, high-contrast telegraphs, UI scale, and rebindable controls remain supported.

## 18. Removed or replaced behavior

- Removed reliance on a software combat crosshair.
- Disabled the obsolete screen-space enemy layer in favor of the pooled world system.
- Replaced fixed unstructured random stat bundles with legal prefixes and suffixes.
- Replaced silent loot-filter selling with protected hide/reveal behavior.
- Replaced inventory-wide item rendering with bounded pages.
- Replaced tooltip-only behavior on several new Unique and conditional affix rules with runtime evaluation.

## 19. Balance direction

- A focused Rare can compete with a Unique through six legal affix slots and stronger rolls.
- Uniques prioritize rule changes over automatic numerical superiority.
- Movement, critical chance, mitigation, cooldown, projectile count, recursion, and Trigger Energy remain capped.
- High-tier affixes are meaningful but not required for a standard expedition.
- Standard affixes support the spell board without replacing connector and modifier decisions.

## 20. Release validation

The Unity menu `Arcane Engine > Validate 1.1 Demo` now validates:

- Combat-plane swept collision.
- Non-empty and unique content IDs.
- Every Base Spell compilation.
- Every Unique × Base Spell compilation.
- Hex-board geometry and six-step rotation identity.
- Stable deterministic hashing.
- 2,000 generated dungeon-layout seeds.
- At least 48 standard bases, 84 affix families, 24 spell-reactive families, 18 Uniques, and 12 corruptions.
- 10,000 generated equipment instances for tier, slot, rarity-cap, and modifier-group legality.
- Spell-build validation, mitigation limits, and trigger caps.

The intended result is a readable beginner loop with enough deterministic depth for an expert to discover genuinely strange spell-and-item combinations.
