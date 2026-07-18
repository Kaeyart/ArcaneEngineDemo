# Arcane Engine — Demo Update 1.1

## The Forge Reborn

Demo Update 1.1 is a foundational interaction and itemization overhaul focused on making Arcane Engine feel like an action RPG instead of a collection of prototype menus. This release rebuilds mouse aiming, enemy Health bars, the Spell Workshop, item generation, loot presentation and equipment progression around clear, reliable player actions.

This document describes the complete planned scope for the 1.1 demo. Features listed here are release requirements, not optional stretch goals.

---

## Update highlights

- Rebuilt combat around a true absolute ARPG cursor that aims independently from WASD movement.
- Replaced unreliable screen-space enemy bars with pooled world-space Health and Shield bars attached directly to enemies.
- Replaced the click-based Spell Workshop prototype with genuine drag-and-drop hex-grid editing.
- Introduced a complete item-generation system built around base items, implicits, item level, rarity, prefixes, suffixes, tiers, tags, weights and modifier groups.
- Added spell-reactive equipment affixes that interact directly with elements, projectiles, triggers, ailments, Shields, dodging and resources.
- Rebuilt item tooltips, comparisons, loot labels, filters and equipment management for fast in-run decisions.
- Added deterministic item validation and large-sample generation tests to prevent illegal or broken equipment.

---

# 1. Absolute ARPG Mouse Aiming

## New control model

- The hardware mouse cursor remains visible and moves freely across the Game view.
- The cursor is never locked, recentered or replaced by a hidden relative-input mode during normal combat.
- WASD controls movement only and never changes the player’s aim direction.
- The player continuously turns toward the cursor’s world-space position, whether moving or standing still.
- All manually cast spells use a ray from the active camera through the cursor onto the combat ground plane.
- Projectiles, beams, movement skills, meteors, novas and targeted triggers use the same resolved world aim point.
- A ground marker displays the exact point currently used by the spell simulation.
- Projectile direction is calculated from the cast origin to that ground marker, eliminating visual and simulation disagreement.
- Aim remains responsive while moving, dodging, rotating the camera or changing camera zoom.

## Cursor behavior

- Removed the combat software crosshair that attempted to imitate the operating-system cursor.
- Added dedicated cursor states for normal aim, valid target, interactable object, invalid cast and UI drag.
- Cursor scale, contrast and color remain configurable through Accessibility settings.
- Cursor position is preserved when opening or closing an interface.
- Losing and regaining application focus no longer freezes aim or produces a false click.
- Gameplay mouse buttons are suppressed while the pointer is interacting with UI.

## Camera separation

- Camera rotation remains a deliberate Middle Mouse drag action.
- Camera zoom remains bound to the Mouse Wheel outside drag operations.
- Moving the cursor alone never rotates the camera.
- Camera rotation and player aiming use independent input paths.
- A reset-camera binding remains available.

---

# 2. Enemy Health-Bar Rebuild

## World-attached bars

- Removed the existing screen-space UI Toolkit enemy-bar layer.
- Added pooled world-space Health bars attached to every active enemy.
- Bar height is calculated from the combined bounds of all visible enemy renderers.
- Bars follow enemy movement, charges, knockback, pulls and teleportation without smoothing or trailing.
- Bars rotate toward the active camera after enemy and camera movement have completed.
- Dead, despawned and pooled enemies release their bars immediately.
- Off-screen enemies no longer leave floating or stale bars behind.

## Information hierarchy

- Standard enemies display a compact Health bar.
- Damaged enemies show exact Health values when advanced combat details are enabled.
- Shielded enemies display a separate blue Shield layer.
- Elite enemies display their name, elite border and active elite modifiers.
- Important ailments and control effects display compact status icons.
- Bosses use a dedicated top-screen boss frame with Health, Shield, phase and mechanic information.
- Training targets always display bars and damage feedback.

## Display options

- Always show enemy Health bars.
- Show bars only after damage or targeting.
- Show or hide numerical Health values.
- Adjustable enemy-bar scale and vertical offset.
- High-contrast bar colors and colorblind-safe ailment icons.

---

# 3. Spell Workshop 2.0

## True drag-and-drop editing

- Replaced the click-to-place prototype with pointer-driven drag-and-drop.
- Modifier tiles can be dragged directly from the run inventory onto any prepared spell board.
- A translucent copy of the modifier follows the cursor during the drag.
- The complete polyhex footprint is previewed over the board before placement.
- Valid destination cells highlight in cyan.
- Invalid destination cells highlight in red.
- Invalid placement displays a plain-language reason, including occupied cells, disconnected connectors, incorrect entry direction, board limits or missing inventory quantity.
- Dropping on a valid location commits the placement.
- Dropping on an invalid location safely returns the modifier to its previous location.
- Installed modifiers can be dragged to another position on the same board.
- Installed modifiers can be dragged back into the inventory to remove them.
- Modifiers can be moved between spell boards only when an uncommitted inventory copy is available.
- One physical modifier copy can never exist on multiple boards simultaneously.

## Rotation and manipulation

- Q and E rotate the dragged modifier counterclockwise or clockwise.
- Mouse Wheel rotation is available while a modifier is being dragged over the board.
- Right Mouse cancels the current drag without changing the build.
- Double-click selects and centers a modifier for keyboard placement.
- Undo and redo cover placement, removal, movement and rotation.
- Closing the Workshop during a drag cancels the uncommitted operation safely.

## Board readability

- Connector directions are displayed on every occupied modifier cell.
- Compatible connections brighten when a modifier is hovered.
- Broken or blocked connections are identified directly on the board.
- Spell execution order is visualized as a numbered path.
- Trigger links identify the source spell, target spell and activation condition.
- Inactive modifiers remain visible but clearly explain why they are inactive.
- Overloaded sections display their Trigger Energy or entity-budget conflict.

## Live spell preview

- Damage, Mana cost, cooldown, projectile count, area, duration, critical chance, ailments and Trigger Energy update during placement preview.
- The preview distinguishes additive, multiplicative and behavior-changing effects.
- Element conversion updates spell color and preview geometry immediately.
- Delivery changes update the mechanic preview without requiring the Workshop to close.
- Trigger chains display recursion depth, activation limits and expected energy cost.
- Beginner view uses short summaries; advanced view exposes exact calculations and modifier order.

## Workshop access

- The Workshop can be opened anywhere during an active run unless a scripted encounter explicitly prevents pausing.
- Combat pauses while the full Workshop is open.
- The same inventory quantities and boards are preserved across rooms and checkpoints.
- A non-drag click-to-place accessibility alternative remains available.

---

# 4. Itemization 2.0

## Item structure

Every generated equipment item is assembled from the following independent layers:

1. Equipment slot.
2. Base item type.
3. Base defense or offense values.
4. Built-in implicit modifier.
5. Item level.
6. Rarity.
7. Prefixes.
8. Suffixes.
9. Quality and upgrade state.
10. Optional corruption or Unique rule.

Items are generated from data definitions rather than hardcoded item names. The generator produces deterministic results from the run seed, room reward seed and loot source.

## Equipment slots

- Helmet.
- Left Shoulder.
- Right Shoulder.
- Chest.
- Left Glove.
- Right Glove.
- Pants.
- Boots.
- Weapon.
- Offhand.

Left and right Shoulders are independent equipment pieces. Left and right Gloves are also independent pieces. Each can roll different affixes and contribute separately to set bonuses.

Two-handed weapons disable the Offhand slot while equipped. One-handed weapons display whether an Offhand is permitted before the item is equipped.

## Base items and implicits

- Every slot receives multiple base-item families with different strengths and weaknesses.
- Armor bases emphasize Health, Armor and damage reduction.
- Ward bases emphasize Mana, Shield and spell recovery.
- Mobile bases emphasize movement, dodge recovery and conditional avoidance.
- Weapons define base spell power, cast behavior and permitted Offhands.
- Offhands include focuses, grimoires, shields and ritual implements.
- Every base has a fixed implicit modifier that remains separate from random affixes.
- Base defense and offense values scale with item level inside controlled ranges.
- Higher-level bases are not automatically superior when their implicit does not support the player’s build.

---

# 5. Rarity, Prefixes and Suffixes

## Rarity rules

- **Common:** base values and implicit only.
- **Magic:** up to one prefix and one suffix.
- **Rare:** up to three prefixes and three suffixes.
- **Unique:** handcrafted modifiers and rule-changing behavior; not assembled like a normal Rare.
- **Corrupted:** an irreversible additional state that can appear on eligible Magic, Rare or Unique items.

Items may drop with open affix slots. A Rare item is not guaranteed to have all six affixes.

## Prefix identity

Prefixes primarily provide foundational power:

- Maximum Health and Mana.
- Armor, Ward and base defensive scaling.
- Spell damage and elemental damage.
- Added elemental damage.
- Weapon spell power.
- Healing and Shield strength.
- Projectile, area, ailment or triggered-spell specialization.

## Suffix identity

Suffixes primarily provide utility and conditional strength:

- Elemental and status resistance.
- Critical chance and critical damage.
- Cast speed and cooldown recovery.
- Movement speed and dodge recovery.
- Projectile speed and duration.
- Trigger Energy recovery.
- Mana efficiency.
- On-kill, on-crit, on-dodge and while-Shielded effects.

## Affix tiers

- Every standard affix family contains five numerical tiers for the 1.1 demo.
- Item level determines which tiers are eligible.
- Higher tiers use smaller generation weights and are meaningfully rarer.
- Tier ranges overlap only where necessary to prevent obvious item-level boundaries.
- Tooltips display the rolled value, possible range and tier in advanced mode.
- Upgrading an item never silently changes the identity of an existing affix.

## Tags and weights

- Affixes use tags such as Fire, Cold, Lightning, Poison, Projectile, Area, Trigger, Critical, Shield, Armor, Movement and Resource.
- Item bases define which tags are eligible and how their weights are modified.
- Wands favor spell and trigger modifiers.
- Staves favor larger offensive rolls at the cost of the Offhand slot.
- Shields favor defense and retaliation.
- Boots heavily favor movement and dodge utility.
- Gloves favor cast speed, projectiles, critical effects and ailments.
- Conflicting affix groups cannot roll together.
- Duplicate affixes from the same family cannot appear on one item.

## Local and global modifiers

- Local modifiers change the base item itself and are calculated before global character modifiers.
- Global modifiers affect the final player or spell stat calculation.
- Tooltips label local and global modifiers in advanced mode.
- Comparison calculations use the fully equipped result rather than comparing isolated numbers incorrectly.

---

# 6. Spell-Reactive Affixes

Standard equipment can support the Spell Workshop without replacing it.

## Elemental affixes

- Increased damage for a specific element.
- Ailment chance, strength and duration.
- Bonuses after converting a spell’s element.
- Conditional resistance penetration.
- Resource recovery after applying or consuming an ailment.

## Delivery affixes

- Projectile speed, range and critical behavior.
- Area size and area damage.
- Beam duration and channel stability.
- Persistent-zone duration and tick rate.
- Summon duration and familiar attack rate.
- Movement-skill recovery and defensive windows.

## Trigger affixes

- Trigger Energy capacity and recovery.
- Reduced cost for the first triggered spell in a chain.
- Bonuses when one prepared spell casts another.
- Conditional power based on trigger generation depth.
- Internal-cooldown recovery for equipment-granted triggers.
- Clear hard limits preventing infinite trigger loops.

## Defensive and resource affixes

- Gain Shield after dodging.
- Recover Mana after a critical hit.
- Convert excess healing into temporary Shield.
- Reduced Mana cost while above a Health threshold.
- Increased spell power while Shielded.
- Emergency defensive effects with explicit internal cooldowns.

Standard affixes enhance a build. They do not provide unrestricted free projectiles, unconditional spell duplication or infinite resource loops.

---

# 7. Unique Items

## Unique-item philosophy

- Unique items are handcrafted build rules rather than stronger Rare items.
- Every Unique has a recognizable visual identity and a concise mechanical purpose.
- A Unique may be weaker numerically while enabling a new build.
- Unique effects can modify spell compilation, but must still obey recursion, Trigger Energy and entity budgets.
- Unique effects appear in the spell preview and advanced calculation breakdown.
- Uniques can be further affected by permitted upgrades and corruption.

## Planned Unique behaviors

- Convert every prepared spell to Lightning while disabling critical strikes.
- Add a virtual connector to the center of a selected spell board.
- Repeat the first spell cast after a successful dodge.
- Store excess healing and release it as spell power.
- Cause projectiles to orbit briefly before seeking the aim point.
- Exchange maximum Mana for persistent Shield.
- Strengthen triggered spells while weakening manual casts.
- Allow a specific ailment to spread when another ailment is consumed.
- Turn a two-handed weapon into a trigger-focused spell engine.
- Link the effects of the Left and Right Glove slots.

The 1.1 demo targets eighteen fully implemented Unique items distributed across all equipment slots.

---

# 8. Corruption

- Eligible items can gain one irreversible Corrupted modifier.
- Corruption can improve, transform or damage an item.
- Corrupted items cannot use ordinary crafting actions.
- Possible outcomes include an additional implicit, an upgraded affix tier, an elemental transformation, a new drawback or the loss of an affix.
- Corruption cannot create an item that violates equipment-slot rules or trigger safety limits.
- The 1.1 demo targets twelve corruption outcomes with slot and tag restrictions.

---

# 9. In-Run Crafting

## Forge actions

- Upgrade a Common item to Magic.
- Add a random legal affix to an item with an open slot.
- Replace one unlocked random affix.
- Improve the numerical roll inside an affix’s existing tier.
- Remove one random unlocked affix.
- Lock one affix before a risky crafting operation.
- Add quality to improve base values.
- Corrupt an eligible item as a final irreversible action.

## Crafting rules

- Every action previews its possible result category and risk.
- Crafting respects item level, affix capacity, tags, weights and modifier groups.
- Locked affixes are visibly marked.
- An item can have no more than one crafted affix in the 1.1 demo.
- Crafting resources are run-bound and do not replace permanent Essence or Legendary Shards.
- Shop and Forge rooms offer different crafting opportunities.
- Crafting results are included in the run history and save checkpoint.

---

# 10. Loot Generation and Drop Quality

- Enemy type, room depth, difficulty modifiers and reward source influence item level.
- Bosses, elites, challenge rooms, secret rooms and merchants use distinct rarity weights.
- Smart-loot weighting slightly favors equipped weapon archetypes and prepared spell tags without guaranteeing upgrades.
- Duplicate protection reduces repeated base items during short runs.
- Pity rules improve the chance of seeing a Rare or build-relevant item after a long dry streak.
- Drop quantity is reduced while average decision quality is increased.
- Ground loot is capped and pooled to protect performance.
- Loot beams, labels and sounds communicate rarity without obscuring combat.

## 1.1 content targets

- At least forty-eight base item definitions across the ten equipment slots.
- At least eighty-four standard affix families.
- Five numerical tiers for each scalable affix family.
- At least twenty-four spell-reactive affix families.
- Eighteen handcrafted Unique items.
- Twelve corruption outcomes.
- Separate loot tables for normal enemies, elites, bosses, merchants, room rewards and secrets.

---

# 11. Item Tooltips and Comparison

- Rebuilt item tooltips with consistent spacing, colors and stat ordering.
- Base name, base type, rarity, item level and equipment slot appear at the top.
- Implicit modifiers appear in a separate section.
- Prefixes and suffixes are visually distinguishable.
- Advanced mode displays affix type, tier, roll range, tags and local/global behavior.
- Unique rules and drawbacks use their own highlighted section.
- Corruption appears before ordinary affixes and cannot be mistaken for a standard modifier.
- Equipped comparison displays gains in green and losses in red.
- Comparison includes the effect of losing an Offhand when evaluating a two-handed weapon.
- Comparison supports Left/Right Shoulder and Left/Right Glove selection.
- Spell-reactive affixes identify which prepared spells currently benefit.
- Holding the advanced-details key temporarily reveals full technical information.

---

# 12. Inventory and Equipment Interaction

- Equipment is changed through drag-and-drop or a reliable click-to-equip alternative.
- Dragging an item highlights every valid equipment slot.
- Ambiguous paired slots prompt for Left or Right placement.
- Invalid drops return the item safely to its previous location.
- Two-handed weapon swaps clearly show the displaced Offhand before confirmation.
- Favorite, locked and junk states remain visible in every inventory view.
- Sorting supports rarity, item level, slot, newest, value and build relevance.
- Filters support equipment slot, rarity, element, spell tag and affix text.
- Equipped, backpack, storage and merchant inventories use the same item-card language.
- In-run equipment changes remain available outside prohibited scripted moments.

---

# 13. Loot Filters and Ground Labels

- Added minimum-rarity filtering.
- Added slot-specific filtering.
- Added filters for spell tags and elemental support.
- Unique and Corrupted items can never be hidden accidentally.
- Hidden items can be revealed temporarily with a dedicated key.
- Ground labels avoid overlapping through vertical stacking and priority sorting.
- The nearest focused item displays a compact comparison before pickup.
- Auto-pickup remains limited to currency and explicitly enabled low-risk categories.

---

# 14. Save Data and Migration

- Item instances receive stable versioned identifiers.
- Affixes save their definition ID, tier, rolled values, source and lock state.
- Existing v1.0 equipment is migrated into valid 1.1 base items and affixes where possible.
- Legacy flat bonuses are translated into the closest legal affix family.
- Items that cannot migrate are preserved as clearly marked Legacy items instead of silently deleted.
- Profile backups are created before the first 1.1 migration.
- Run checkpoints store workshop drag state only after committed operations.
- Interrupted or failed drag operations cannot duplicate modifiers or items.
- Corrupted saves continue to use quarantine and rotating-backup recovery.

---

# 15. Performance and Technical Rebuild

- Centralized pointer ownership prevents combat and UI from consuming the same mouse event.
- Input transitions are de-duplicated at the action layer.
- Aim calculations use one authoritative cursor sample per frame.
- Enemy Health bars use object pooling and contain no per-frame scene searches.
- Item tooltips reuse visual elements instead of rebuilding complete trees every frame.
- Inventory filtering uses cached indexes by slot, rarity and tag.
- Item generation uses deterministic weighted tables with no reflection or scene lookup.
- Workshop previews reuse footprint and connector buffers.
- Drag operations produce no permanent allocations after initialization.
- Large inventories virtualize off-screen item rows.
- Performance diagnostics track active bars, visible loot labels, tooltip rebuilds and workshop preview cost.

---

# 16. Accessibility and Alternative Interaction

- Visible hardware cursor with adjustable size, color and contrast.
- Optional high-contrast ground aim marker.
- Optional click-to-place Workshop mode for players who cannot comfortably drag.
- Optional click-to-equip inventory mode.
- Adjustable drag threshold and double-click timing.
- Plain-language and advanced item-tooltip modes.
- Enemy-bar scale, numerical Health and visibility controls.
- Colorblind-safe rarity and status presentation that does not rely on color alone.
- Reduced-motion behavior for loot beams, tooltip transitions and modifier ghosts.
- All essential drag actions have keyboard-accessible alternatives.

---

# 17. Removed and Replaced Systems

- Removed the hidden combat software crosshair.
- Removed screen-space UI Toolkit enemy Health bars.
- Removed click-only modifier placement as the default Workshop interaction.
- Removed fixed, unstructured item-stat bundles.
- Removed item generation that ignores affix type, tier, item level or modifier conflicts.
- Removed silent equipment replacement when paired slots are ambiguous.
- Removed tooltips that combine all item effects into an unreadable stat paragraph.
- Removed per-frame global searches used by combat presentation and inventory interaction.

---

# 18. Balance Direction

- A well-rolled Rare should compete with a Unique when its affixes strongly support the build.
- A Unique should enable different decisions rather than automatically provide the largest numbers.
- Standard affixes should enhance the hex-grid build without making modifier placement irrelevant.
- Defensive equipment should remain valuable at every depth.
- Resistances should help against appropriate threats without becoming mandatory binary caps during the short demo.
- Movement speed receives controlled limits to preserve room readability and enemy telegraphs.
- Trigger Energy, cooldown and resource affixes use diminishing availability to prevent runaway loops.
- High-tier affixes should be exciting but not required to complete a standard run.

---

# 19. Release Validation

The 1.1 demo cannot be considered complete until all of the following checks pass:

- Moving the cursor in a full circle while holding WASD rotates the player toward the cursor without changing movement direction.
- Every manual spell targets the visible ground marker at all supported resolutions and UI scales.
- UI interaction never fires a spell or closes from the same physical click.
- Enemy bars remain attached during movement, knockback, charges, pulls, camera rotation and camera zoom.
- No enemy leaves a stale Health bar after death or despawn.
- One hundred repeated modifier drag, rotate, move and removal operations produce no loss or duplication.
- Modifiers cannot be shared illegally across spell boards.
- Workshop undo and redo return inventory quantities and board state exactly.
- At least ten thousand deterministic generated items contain no duplicate affix groups, illegal tiers, invalid slot modifiers or rarity-cap violations.
- Two-handed weapon comparisons correctly include Offhand removal.
- Every Unique compiles safely with every Base Spell family.
- Trigger validation rejects infinite or unbounded item-and-spell combinations.
- Existing profiles migrate without losing currencies, stored spells, equipment or run checkpoints.
- A sixty-minute stress run stays within the entity, Health-bar, loot-label and UI allocation budgets.

---

# 20. Intended 1.1 Player Experience

After Update 1.1, the player should be able to enter a room, move in one direction while aiming in another, immediately understand enemy condition, collect an item worth inspecting, compare its real affixes, equip it through a physical interaction, open the Workshop, drag a newly found modifier into a legal hex pattern and see the spell transform before returning to combat.

Every one of those actions must be readable to a beginner, reliable under pressure and deep enough to reward an expert who understands affix tiers, tags, connector geometry, trigger chains and equipment interactions.

That interaction loop is the release standard for the Arcane Engine 1.1 demo.
