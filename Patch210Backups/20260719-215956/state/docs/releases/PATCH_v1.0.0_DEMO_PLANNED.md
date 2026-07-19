# Arcane Engine — 1.0 DEMO Update

## The Road to the Relic Forge

The 1.0 DEMO Update is the point where *Arcane Engine* moves beyond a systems prototype and becomes a complete, self-contained game demo.

The central promise remains unchanged: begin with a recognizable spell, rebuild it through a directional hex board, combine it with other customized spells through triggers, and discover combinations that can range from simple and reliable to spectacularly unstable. Version 1.0 is focused on making that promise readable, balanced, responsive, and dependable from the first minute in Home Base to the final boss and extraction.

This document is the complete implementation target for the 1.0 DEMO. Features listed here are planned for the update and should not be treated as available until the corresponding 1.0 build is released.

---

# Major update highlights

- A complete 30–45 minute Standard expedition with deliberate pacing, escalating encounters, meaningful route decisions, a final boss, and physical extraction.
- Reliable collision, navigation, enemy separation, attack telegraphs, crowd-control rules, and combat-state handling.
- A fully validated spell-construction system with compatibility explanations, execution order, trigger inspection, loop protection, build comparison, and performance limits.
- A comprehensive balance framework for spells, modifiers, equipment, enemies, rewards, shops, permanent progression, and recursive trigger builds.
- Authored encounter groups, objective-specific enemy behavior, distinct minibosses, and a substantially expanded final boss encounter.
- A proper dungeon map, improved route information, objective failure states, bonus challenges, and generation safeguards.
- Complete run progression, permanent progression, economy, itemization, set bonuses, Unique handling, and inventory protection.
- A unified UI Toolkit interface, redesigned HUD, title screen, pause flow, victory/death screens, profiles, options, and run history.
- A restartable guided tutorial, contextual learning, complete input rebinding, and expanded accessibility options.
- Versioned and migration-tested profiles, resumable run checkpoints, backup recovery, transaction safety, and diagnostic tools.
- Performance budgets, object pooling, automated tests, seed validation, long-session testing, and release-grade error handling.
- Adaptive audio mixing, clear narrative context, a standalone Linux build, reproducible releases, and a final consistency pass across the entire game.

---

# Complete 1.0 DEMO patch notes

## 1. Complete expedition structure

Standard expeditions have been rebuilt around an intentional beginning, middle, and conclusion.

- Standard runs now target a consistent 30–45 minute completion time.
- Expeditions begin with introductory encounters that allow a build to form before complex enemy combinations appear.
- Early rooms prioritize Base Spells, fundamental modifiers, Gold, and introductory equipment.
- Mid-run rooms introduce stronger equipment, specialized modifiers, Elite enemies, harder objectives, and build-defining decisions.
- Late-run routes emphasize preparation, risk management, recovery, minibosses, and final build refinement.
- Important room categories are distributed through controlled pacing rules rather than unrestricted random selection.
- Repeated room categories and repeated layouts are suppressed more aggressively.
- Difficulty now increases at defined expedition milestones.
- New enemy roles are introduced progressively instead of appearing from the full roster immediately.
- Shops, Safe Rooms, healing opportunities, minibosses, and the final boss are placed within controlled timing windows.
- Standard victory requires defeating the final boss and using the physical Extraction Gate.
- Death, abandonment, Save & Quit, extraction, and successful completion now have clearly separated outcomes.
- Run-end banking is completed before the player returns to Home Base.
- Daily Challenge, Custom Seed, Standard, and Endless modes now communicate their rules and progression separately.
- A complete run summary records the final build, route, performance, rewards, and result.

## 2. Combat reliability overhaul

Combat has received a foundational reliability pass so that visible actions and calculated results agree.

- Player and enemy attacks now use formal collision layers.
- Projectiles can no longer spawn inside the player, scenery, doors, objectives, or invalid collision volumes.
- Projectile collision is validated at close range, long range, high speed, and every supported camera angle.
- Swept collision is used for fast-moving projectiles to prevent tunnelling.
- Player, enemy, projectile, trap, pickup, objective, and scenery interactions are explicitly separated.
- Enemies maintain separation and can no longer stack inside one another.
- Enemies navigate around solid cover, walls, hazards, objectives, and other enemies.
- Teleports, charges, dashes, dodges, pulls, and knockback validate their destinations before movement is applied.
- Invalid movement destinations fall back to the nearest safe position.
- Attack interruptions now follow consistent rules.
- Staggered enemies receive a short stagger-immunity window before they can be staggered again.
- Repeated crowd control uses diminishing returns.
- Bosses and Elites use explicit resistance rules for knockback, pull, Freeze, stun, and interruption.
- Simultaneous enemy attacks are limited to reduce unavoidable damage combinations.
- Every high-damage attack includes a readable telegraph and recovery period.
- Enemies cannot attack during room-entry protection or introduction sequences.
- Off-screen attacks display directional danger indicators.
- Player deaths identify the final source of damage.
- Room combat now follows a formal entrance, active, completion, reward, and departure state sequence.
- Combat cannot complete while required enemies, objectives, hazards, or scripted actions remain unresolved.

## 3. Spell validation and trigger safety

The Spell Workshop now explains not only what can be built, but why a configuration works.

- Every modifier uses a formal compatibility definition.
- Compatibility checks include spell tags, delivery type, element, required behavior, forbidden behavior, and board connection state.
- Legal placements are highlighted before a modifier is placed.
- Illegal placements display a specific explanation.
- Modifiers that connect but cannot affect the current Base Spell are marked inactive with a precise reason.
- The Workshop displays the complete execution order from the Base Spell through every active modifier.
- Trigger links show activation moment, target context, Trigger Energy cost, inherited power, internal cooldown, activation limit, and generation depth.
- Potential trigger loops are detected before a build is confirmed.
- Recursive chains remain protected by Trigger Energy, generation, activation, entity, sound, and effect budgets.
- On Cast, On Hit, On Kill, On Expire, On Bounce, On Status, and defensive triggers now use consistent event ownership.
- Damage-over-time, summons, persistent zones, secondary projectiles, and triggered spells preserve the correct source spell and kill credit.
- Triggered spells clearly state whether they consume Mana, use cooldowns, inherit critical hits, or activate equipment effects.
- The same board, equipment, seed, and cast conditions now produce deterministic mechanical results.
- A Validate Build command reports inactive pieces, unsupported combinations, loops, excessive entity generation, and missing trigger targets.
- Replacing a Base Spell previews any modifiers that would deactivate or return to inventory.
- Undo and redo remain valid after rotation, removal, replacement, and Base Spell changes.
- Players can save and name spell configurations during a run.
- Current and proposed spells can be compared before a board change is confirmed.
- Spell statistics now separate exact values from estimates.
- Advanced details include damage per cast, estimated damage per second, Mana efficiency, cooldown, projectile count, area, duration, status behavior, instability, Trigger Energy, entity cost, and trigger contribution.

## 4. Global balance framework

Version 1.0 introduces a common mathematical foundation for player power, enemy durability, loot, and rewards.

- Baseline player damage, Health, Mana, movement, critical statistics, and cooldown expectations are defined for every expedition stage.
- Expected enemy survival time is defined separately for normal enemies, Elites, minibosses, and bosses.
- Additional projectiles now account for increased hit frequency and trigger generation.
- Homing pays for its improved reliability through controlled damage, speed, cost, or targeting limits.
- Area damage and single-target damage follow separate power budgets.
- Manual casting and triggered casting are balanced independently.
- Trigger chains have a defined maximum sustainable output.
- Elements now have distinct strategic strengths and weaknesses.
- No element is intended to be universally superior.
- Critical Chance and Critical Damage scale through a shared budget.
- Cooldown Reduction, Mana efficiency, movement speed, projectile count, pierce, bounce, chain targets, summons, statuses, and triggers use hard or soft limits where necessary.
- Exponential combinations receive specific scaling rules rather than unrestricted multiplication.
- Modifier benefits are priced against their drawbacks consistently.
- Item affixes use a formal power budget based on slot, rarity, and item level.
- Enemy Health and damage curves now account for the expected strength of an average build.
- Rewards use minimum and maximum quality guarantees.
- Pity rules prevent long periods without equipment, modifiers, or Base Spell opportunities.
- Shop prices are tied to expected Gold income and room depth.
- Permanent progression improves access and preparation without trivializing early combat.
- Balance validation includes starter builds, average builds, optimized builds, deliberately weak builds, and extreme recursive builds.

## 5. Player movement and input

- Movement acceleration and deceleration have been refined for greater control.
- Camera-relative and world-relative movement are available as separate options.
- Dodges use the current movement direction, with a predictable fallback when no movement input is held.
- Dodge invulnerability timing is consistent and visibly communicated.
- Repeated dodge input during cooldown no longer causes ambiguous behavior.
- Spells and dodges support short input buffers.
- Charged and channelled spells have explicit movement, cancellation, and interruption rules.
- Moving to cancel a cast can be enabled where appropriate.
- Mouse sensitivity and camera-rotation sensitivity can be adjusted independently.
- Camera rotation can be disabled.
- Hold and toggle behavior can be configured for supported actions.
- Every gameplay action can be rebound, including mouse buttons.
- Binding conflicts are detected before changes are saved.
- Controls can be reset to defaults.
- Common non-QWERTY keyboard layouts are supported.
- Gameplay input is suspended when the application loses focus.
- Management screens cannot open during unsafe room transitions or boss phase changes.
- A controller-ready input abstraction is included even where final controller support remains outside the 1.0 demo scope.

## 6. Camera and aiming

- The logical aiming camera is separated from visual camera shake.
- Camera shake can no longer alter the calculated spell direction.
- The mouse cursor, software crosshair, ground reticle, player facing, and projectile trajectory use the same aim solution.
- Aim remains accurate after resolution, fullscreen, aspect-ratio, and camera-rotation changes.
- Ultrawide and high-resolution aim behavior has been corrected.
- The cursor leaving the game window is handled safely.
- Camera zoom and rotation preferences are saved.
- Foreground objects that obscure the player are faded or outlined.
- Camera obstruction handling prevents scenery from completely hiding combat.
- Boss introductions and phase transitions use controlled camera framing.
- Reduced-camera-motion options limit automatic camera changes.
- Zoom limits are configurable within safe gameplay ranges.

## 7. Combat feedback and readability

- Player hits now include a clear directional response.
- Damage indicators identify the direction of off-screen threats.
- Low-Health feedback remains readable without obscuring telegraphs or the cursor.
- Health damage, Shield damage, armor absorption, resistance, immunity, and blocked damage use distinct feedback.
- Zero-damage hits display Immune, Blocked, or Resisted when appropriate.
- Critical hits, elemental weaknesses, armor breaks, staggers, and interrupts have distinct presentations.
- Elite and boss kills receive stronger confirmation than normal kills.
- Insufficient Mana, unavailable spells, and active cooldowns produce immediate feedback near the relevant spell card.
- Charged and channelled spells display clear progress indicators.
- Important statuses display remaining duration.
- Rapid damage ticks can be combined into readable totals.
- Damage-number density and size are configurable.
- A priority system prevents nonessential spell effects from covering enemy telegraphs.
- Pickup confirmation now uses a dedicated feed rather than relying on the debug-style combat log.
- Combat feedback respects reduced flashes, reduced motion, screen shake, sound density, and effect density settings.

## 8. Modifier presentation and Workshop usability

- Every modifier tooltip follows a standardized information format.
- Tooltips display category, rarity, shape, connector directions, compatible tags, mechanical effect, exact values, drawback, and trigger behavior.
- Rotated connector previews appear before placement.
- Newly activated and deactivated modifiers are highlighted after every board change.
- The active path from the Base Spell to each modifier can be inspected.
- Connector symbols remain readable without relying on color.
- Modifiers can be filtered by projectile, area, element, trigger, utility, resource, defensive, compatible, active, and unused categories.
- Sorting is available by name, rarity, compatibility, recently found, and remaining copies.
- Newly discovered modifiers receive a New marker.
- Side-by-side comparison appears when replacing a modifier.
- Players can return all modifiers from one spell board with one command.
- Clearing an entire board requires confirmation.
- Quick-install prioritizes the most recently cast spell but allows the player to choose another destination when several boards are legal.
- Quick-install warns before making a clearly weaker or behavior-changing replacement.
- Search, filters, board zoom, rotation controls, undo, redo, and expert details use a consistent layout.

## 9. Spell identity and build history

- Every Base Spell has a defined tactical identity and intended starting use.
- Modifiers expand that identity without making every Base Spell behave identically.
- Element, delivery, size, targeting, movement, and payload transformations use consistent presentation rules.
- Heavily modified spells receive generated descriptive names while preserving their original foundation in advanced details.
- Players can rename saved configurations.
- The Spell Archive records completed builds and unusual combinations.
- Previously completed builds can be inspected after a run.
- The run recap displays the final configuration of every equipped spell.
- Legendary Spells remain limited to one active selection and retain an exclusive signature rule.
- Unique-item spell foundations can be inspected before the item is equipped.
- Unique foundations remain compatible with the ordinary hex-board modification system.

## 10. Enemy artificial intelligence

- Enemies now use explicit Idle, Entrance, Pursuit, Reposition, Telegraph, Attack, Recovery, Stagger, Retreat, and Death states.
- Ranged enemies require line of sight before firing.
- Ranged enemies seek useful firing positions instead of backing away indefinitely.
- Support enemies select valid wounded or strategically important allies.
- Healing and shielding cannot target invalid, invulnerable, dead, or full-Health units.
- Assassins validate teleport destinations and cannot appear inside scenery or the player.
- Chargers validate their complete charge lane.
- Enemies respond intentionally to protected objectives, Escort targets, Hold zones, and ritual targets.
- Encounter groups use role-aware positioning and attack scheduling.
- The number of simultaneous attackers is limited.
- Enemy spawn points require sufficient distance, valid navigation, and a safe entrance sequence.
- Enemies that become unreachable recover to a legal position.
- Enemies leaving the room bounds are returned or safely removed.
- Anti-stalling behavior resolves encounters that exceed their maximum intended duration.
- Seeded modes use deterministic AI variation where randomness affects meaningful results.

## 11. Enemy readability and Bestiary information

- Every enemy role uses a consistent role icon until final visual assets provide an equivalent silhouette.
- New enemy mechanics are introduced with a concise first-encounter explanation.
- The Bestiary records role, Health class, armor, attacks, weaknesses, support behavior, and possible Elite modifiers.
- Armor, immunity, resistance, and active status conditions appear consistently.
- Healing, shielding, summoning, channeling, controlling, and objective pressure use visible indicators.
- Linked or protected enemies display their relationship.
- Dangerous off-screen ranged enemies receive priority indicators.
- Enemy hazards and player hazards use separate colors, shapes, and outlines.
- Immediate damage, delayed damage, persistent hazards, interruptible attacks, and objective threats follow a consistent telegraph language.
- Color is never the only indicator used to communicate danger.

## 12. Authored encounter direction

- Enemy groups are now selected from authored encounter compositions.
- Separate encounter pools support early, middle, late, Elite, miniboss, challenge, and objective rooms.
- Every composition defines minimum space, maximum enemy count, role limits, reinforcement timing, and reward value.
- High-pressure role combinations are restricted during early rooms.
- The number of simultaneous Elites is controlled.
- New enemy mechanics are introduced one at a time before combinations become more demanding.
- Reinforcements use visible spawn warnings.
- Room layouts provide objective-specific and encounter-specific spawn regions.
- Enemies receive objective-specific priorities.
- Minibosses use dedicated mechanics rather than ordinary Elite scaling alone.
- Challenge rooms include explicit rule variations and enhanced rewards.
- Optional high-risk combat events can be accepted or ignored.
- Encounter-completion safeguards prevent missing enemies or failed scripts from blocking a run permanently.

## 13. Final boss reconstruction

- The Dungeon Warden now has a dedicated introduction and name presentation.
- The entrance sequence is short and skippable after the first viewing.
- Each phase teaches and tests a different combat rule.
- Ward Pillars have their own damage feedback and objective presentation.
- Phase transitions clean up incompatible attacks, projectiles, summons, and hazards.
- New attacks cannot begin underneath a transition overlay.
- The boss receives dedicated attack patterns instead of relying primarily on normal melee behavior.
- Arena interactions change between phases.
- Attack patterns vary through controlled sequences rather than unrestricted randomness.
- Boss crowd-control resistance is explicit and visible.
- An anti-stall escalation begins if a phase lasts far beyond its intended duration.
- Difficulty modifiers can add mechanics or pattern changes without only increasing statistics.
- A practice version becomes available in Training after the first encounter.
- Boss restarts restore the encounter to a clean state.
- Victory triggers a dedicated death sequence and reward presentation.
- The boss result records completion time, damage taken, pillars destroyed, phase performance, and strongest spell.
- Unique loot and the Legendary Shard are granted exactly once and protected against checkpoint duplication.

## 14. Procedural dungeon generation

- Layout generation, encounter selection, objective selection, and reward generation are separated into independent validated stages.
- Every generated room verifies entrance, objective, reward, and exit reachability.
- Rooms reserve safe entrance and departure areas.
- Boss-mechanic positions are reserved before scenery is placed.
- Cover, hazards, traps, chests, enemies, objectives, rewards, and doors obey minimum separation rules.
- Objects cannot spawn inside walls, outside the room, or on inaccessible terrain.
- Escort paths and Hold zones are validated before the room begins.
- Room-layout archetypes create distinct navigation patterns.
- Recent-layout suppression reduces visible repetition.
- Custom and Daily seeds reproduce room layout, encounter group, routes, objective, and reward generation.
- Invalid generation attempts use safe fallback layouts.
- Development builds include spawn-zone and navigation visualization.
- Automated generation tests evaluate thousands of seeds and record any failing seed.

## 15. Routes and dungeon map

- Route choices now communicate risk, likely reward category, difficulty, and known special rules.
- Hidden information is clearly distinguished from revealed information.
- Map-reveal upgrades expose additional route details.
- The three route choices are prevented from becoming functionally identical.
- A proper dungeon map shows the current room, visited path, available branches, discovered special rooms, and extraction route.
- The map includes a legend for room categories and known hazards.
- Route history is preserved in run checkpoints and recaps.
- Rewards and route doors use separate interaction zones.
- Doors cannot be entered while required rewards or level-up choices remain unresolved.
- Boss and extraction routes require a deliberate confirmation interaction.
- Room transitions begin only after the player, camera, HUD, objective system, and save checkpoint are ready.

## 16. Objective refinement

- Every objective includes a concise first-encounter explanation.
- Objective requirements scale with room depth and difficulty.
- Protected targets receive a dedicated stability bar.
- Enemies now attack protected objectives intentionally.
- Escort targets use validated movement paths and recovery behavior.
- Hold zones cannot appear beneath scenery or unavoidable hazards.
- Ritual and portal targets are mechanically and visually separated from defensive targets.
- Objective failure produces an explicit consequence, such as reduced rewards, additional enemies, lost bonus rewards, or a changed completion condition.
- Optional bonus objectives can appear alongside the primary objective.
- Partial success can grant partial rewards where appropriate.
- Objective success and failure use distinct feedback.
- Objective-specific reward bonuses are displayed before collection.
- Objective performance is included in the run recap.
- Objectives that conflict with the generated layout are automatically replaced before the room starts.

## 17. Run Levels and temporary perks

- Run Level choices are protected from enemy attacks and cannot overlap room rewards or route doors.
- Three physical perk choices appear at each Run Level.
- Players can inspect exact current and cumulative effects before choosing.
- Current perks and ranks are visible from the HUD and inventory.
- Perk categories include offense, defense, resource control, movement, critical focus, casting speed, and Trigger Energy.
- Perks stack according to explicit rank rules.
- Perk selection can be rerolled or skipped when the run provides the appropriate resource.
- Run experience follows a defined scaling curve and maximum level.
- Infinite summons, Training targets, and endlessly generated enemies do not grant exploitable experience.
- Rooms cannot be farmed indefinitely for experience.
- Build-aware weighting improves the relevance of choices without making them deterministic.
- Perks, ranks, unspent choices, and temporary bonuses persist through Save & Quit.
- Daily and Custom Seed modes preserve deterministic perk offerings where required.

## 18. Permanent progression

- The permanent upgrade tree has been reorganized around access, preparation, variety, and mastery.
- Raw statistical upgrades remain deliberately limited.
- Every node shows prerequisites, current rank, maximum rank, cost, current effect, and next-rank effect.
- Expensive purchases require confirmation.
- Starting with two and three selected spells follows clear progression milestones.
- Preparation upgrades increase starting modifier flexibility without replacing dungeon discoveries.
- Reward rerolls, saved configurations, archive tools, and discovery information have dedicated progression paths.
- Unlocks enter the correct dungeon reward and shop pools immediately.
- Duplicate protection is applied where repeated unlocks would provide no value.
- Essence and Legendary Shard acquisition rates are balanced around complete and failed runs.
- Progression does not require repeatedly farming trivial early rooms.
- A profile summary shows unlocked systems, content completion, best performance, and next milestones.
- A safe respec option is provided if permanent choices could otherwise trap a profile.
- Permanent transactions use atomic saves and cannot spend the same currency twice.

## 19. Economy and merchants

- Gold income now follows an expected range per room and expedition stage.
- Shop timing accounts for whether the player can reasonably afford at least one meaningful purchase.
- Prices scale by rarity, service value, and depth.
- Sale prices follow the same item-value framework.
- Buying and reselling cannot generate profit.
- Gold, Essence, and Legendary Shards use distinct terminology, presentation, and persistence explanations.
- Merchants display the player's current Gold beside every price.
- Failed purchases explain the exact reason.
- Merchant specializations influence available stock.
- Cursed stock identifies its exact advantage and risk before purchase.
- Shop refreshes are available only through defined services or resources.
- Unused Gold is reported in the recap and follows the selected run-mode rule.
- Economy simulation tests evaluate income, spending, selling, and progression across generated runs.

## 20. Itemization and equipment comparison

- Item level, rarity, slot, affix budget, and upgrade rank now follow formal scaling rules.
- Affixes use defined ranges and families.
- Mutually exclusive affixes cannot roll together.
- Duplicate affixes are prevented unless explicitly supported.
- Item tooltips display base statistics, rolled affixes, set membership, Unique effect, item level, upgrade rank, corruption, and equipped comparison.
- Additive and multiplicative values are labelled clearly.
- Equipping an item immediately updates affected spell information.
- Health and Mana ratios are preserved safely when maximum values change.
- Two-handed weapons and offhands use explicit compatibility rules and confirmation feedback.
- All ten equipment positions are shown on a proper paper-doll layout.
- Inventory sorting includes slot, rarity, item level, newest, name, set, favorite, junk, and value.
- Inventory filters and search remain available during a run and in Home Base.
- New, Favorite, Locked, Junk, Corrupted, Set, and Unique states use consistent markers.
- Locked and Favorite items cannot be bulk sold.
- Selling a Unique requires confirmation.
- Merchant buyback is available for recently sold items where appropriate.
- Duplicate Unique items have a defined sale, conversion, or storage rule.
- Set comparisons show the bonuses gained and lost before equipment changes.
- Transformative Unique effects update spell names, descriptions, statistics, visuals, and trigger rules immediately.
- Item-exclusive spells can be inspected before equipping the item.

## 21. Inventory and transaction safety

- Item instance identifiers are validated and deduplicated during loading.
- An item cannot exist in the backpack and an equipment slot simultaneously.
- Equipping, unequipping, selling, upgrading, cleansing, banking, and loading are completed as protected transactions.
- Save & Quit cannot duplicate or delete run items.
- Missing item definitions are handled safely without invalidating the profile.
- Loadouts referencing missing items report what could not be restored.
- Locked, Favorite, Unique, and equipped items are protected from bulk operations.
- Inventory overflow uses an explicit recovery rule if capacity is introduced.
- Rare and Unique transactions are recorded in the diagnostic history.
- Inventory migration tests cover every supported profile version.

## 22. Unified UI architecture

- Major remaining IMGUI screens are migrated to UI Toolkit.
- Reusable UI components now handle cards, bars, tooltips, tabs, item rows, filters, dialogs, notifications, and confirmation screens.
- Spacing, typography, color, border, focus, hover, selected, disabled, and danger states follow one design system.
- Keyboard navigation is supported throughout menus.
- Controller-navigation architecture is included for future input support.
- Safe margins protect UI at different displays and aspect ratios.
- 16:9, 16:10, ultrawide, 4K, and smaller laptop resolutions are supported.
- Scroll views use consistent focus and nesting behavior.
- Tooltips remain within screen bounds.
- Modal screens follow strict stacking and back-navigation rules.
- Gameplay input cannot pass through UI.
- Gameplay hotkeys do not activate while typing in a text field.
- Destructive operations use standardized confirmation dialogs.
- Closing a screen restores focus, cursor state, and time scale correctly.
- Losing application focus cannot leave the game in an invalid pause state.

## 23. Combat HUD redesign

- HUD information is reorganized by combat priority.
- Permanent control reminders fade after onboarding and can be reopened through Help.
- Health, Mana, Shield, dodge cooldown, spell cooldowns, charges, channels, statuses, temporary effects, and Run Perks use consistent presentation.
- A proper minimap replaces the limited breadcrumb-only presentation.
- Objective information remains concise during combat and expands when inspected.
- Boss mechanics receive dedicated objective and phase elements.
- Elite modifiers include a first-encounter explanation and compact active indicators.
- Off-screen enemy and hazard indicators use priority rules.
- A compact pickup feed reports Gold, Essence, modifiers, Base Spells, equipment, and important upgrades.
- A separate combat-event feed replaces debug-style messages.
- HUD scale, opacity, safe zone, and selected element visibility are configurable.
- Enemy Health bars use distance, importance, visibility, and overlap rules.
- Distant or irrelevant normal-enemy bars can be hidden automatically.
- Bar clustering prevents unreadable stacks in dense encounters.
- Boss Health remains in a dedicated presentation.

## 24. Title screen, pause flow, and run results

- A proper title screen includes New Game, Continue, Profiles, Options, Credits, and Quit.
- First launch begins with profile selection and essential display/audio settings.
- The pause menu includes Resume, Options, Save & Quit, Abandon Run, Return to Title, and Quit.
- Save & Quit and Abandon Run are clearly differentiated.
- Abandoning a run requires confirmation and explains what will be lost.
- Loading transitions use a dedicated screen or transition mask.
- A run-introduction panel summarizes mode, seed, starting spells, equipment, and difficulty modifiers.
- Death displays retained and lost resources clearly.
- Victory displays banked equipment, Base Spells, permanent currency, build summary, and performance.
- Run history records recent victories, failures, seeds, builds, and scores.
- Credits, licenses, application version, and build number are available from the title screen.

## 25. Guided tutorial and contextual learning

- The Apprentice Trial now teaches one mechanic at a time.
- Tutorial steps require the player to demonstrate the action rather than merely press a key.
- Movement, aiming, casting, Mana, cooldowns, Health, enemy telegraphs, dodging, interaction, loot, equipment, hex placement, connectors, inactive modifiers, and triggers are covered.
- The tutorial resets safely if an expected object or state becomes invalid.
- It can be restarted from Home Base.
- Experienced players can skip it.
- Contextual reminders appear after repeated mistakes and can be dismissed.
- First-time explanations cover shops, Run Levels, room objectives, Unique items, Legendary Spells, Legendary Shards, extraction, and resource persistence.
- Long explanations pause action and never appear over unresolved danger.
- A searchable Field Manual records every tutorial topic.
- Tutorial completion and dismissed reminders are saved per profile.

## 26. Accessibility expansion

- Every keyboard and mouse action can be rebound.
- Binding conflicts are shown before confirmation.
- Supported actions offer Hold and Toggle behavior.
- UI scale, HUD scale, tooltip scale, and damage-number scale are configured separately.
- Connector shapes and symbols support colorblind play.
- High-contrast telegraph, cursor, outline, and comparison options are available.
- Reduced Flashes, Reduced Camera Shake, and Reduced Motion are separate controls.
- Damage numbers and enemy bars have independent visibility and density controls.
- Cursor color and size can be adjusted.
- Important sound cues receive optional visual indicators.
- Master, Music, Combat Effects, Enemy Effects, Ambience, Voice, and UI channels can be adjusted independently where used.
- Mono-audio support is included where applicable.
- Tutorial text can pause the game.
- Workshop explanations can use simplified or advanced wording.
- Red and green are never the sole indicators for positive and negative comparisons.
- Extreme spell combinations undergo a photosensitivity and readability review.

## 27. Profile save reliability

- Profile saves occur after permanent purchases, unlocks, Legendary evolution, profile changes, settings changes, tutorial completion, and successful banking.
- An autosave indicator confirms active and successful writes.
- Simultaneous save requests are serialized.
- Every profile write remains atomic.
- Checksums are validated before a profile is accepted.
- Three rotating backups remain available.
- Corrupted primary saves automatically attempt backup recovery.
- Every profile version uses an explicit migration path.
- Migration tests cover v0.1 through v1.0 fixtures.
- Future-version profiles are rejected safely rather than silently downgraded.
- Stable IDs, duplicate records, invalid definitions, and missing references are normalized carefully.
- Save-size and disk-write failures produce actionable messages.
- Read-only directories, insufficient disk space, and interruption during saving are handled without destroying the previous valid save.
- The Options and Profile screens show the last successful save time and save location.
- Development-only progress is excluded from release profiles.

## 28. Run checkpoint integrity

- Run checkpoints store the mode, seed, room, route history, current resources, difficulty rules, currencies, Run Level, experience, perks, spells, boards, modifiers, equipment, backpack, and stored Base Spells.
- Generated room data is reproducible from the saved seed and room state.
- If continuing restarts the current room, the rule is explained before saving and loading.
- One-time fountains, purchased offers, collected permanent currency, boss rewards, and resolved events are tracked.
- Save & Quit cannot repeat room, boss, shop, or permanent rewards.
- Checkpoint banking and profile banking use separate protected transactions.
- An incompatible checkpoint reports why it cannot continue.
- New versions define whether older checkpoints migrate, restart the current room, or return protected contents safely.
- A failed checkpoint load does not modify the permanent profile.

## 29. Runtime architecture refactor

- Persistent profile state, temporary run state, combat simulation, room state, procedural generation, and presentation are separated.
- Large multi-purpose components are divided into focused services.
- Damageable, Targetable, Interactable, Status Receiver, Objective Participant, and Save Participant behaviors use explicit interfaces.
- Runs, rooms, players, enemies, and bosses use formal state machines.
- Authorable definitions use stable IDs and validated content data.
- Shared definitions cannot be mutated by runtime instances.
- Component initialization follows an explicit dependency order.
- Gameplay no longer depends on Unity component creation order.
- Delayed effects, events, and coroutines follow cancellation and cleanup rules.
- Event subscriptions are removed reliably.
- Destroyed objects are removed from registries.
- Structured logging separates saves, combat, generation, content, UI, and performance categories.
- Release builds disable verbose development logging.

## 30. Performance and long-session stability

- CPU, GPU, memory, garbage collection, physics, UI layout, and audio performance are measured against defined targets.
- Per-frame scene-wide object searches are removed from active gameplay.
- Projectiles, damage numbers, common effects, hazards, loot markers, Health bars, and common enemies use object pooling.
- Shared materials are reused safely.
- Repeated LINQ and temporary-array allocations are removed from Update and LateUpdate paths.
- Component references and common query results are cached.
- Physics checks use non-allocating queries where possible.
- Simultaneous audio voices, lights, transparent effects, entities, projectiles, summons, and recursive actions are budgeted.
- Effect-quality presets adjust density and secondary presentation.
- Dynamic reduction protects frame rate during extreme builds.
- Room cleanup verifies that no combat objects remain active.
- Endless runs are tested for memory growth and registry leaks.
- Development builds include entity, projectile, trigger, frame-time, and allocation counters.
- Dedicated stress-test spells validate performance limits.

## 31. Error handling and recovery

- Missing catalog entries use safe fallback behavior and clear diagnostics.
- Invalid rooms use a fallback room rather than blocking the expedition.
- Missing starting spells return the player to a valid loadout selection.
- UI initialization failures display a controlled recovery screen.
- Unrecoverable failures use a readable fatal-error presentation instead of leaving a broken scene active.
- Serious errors record mode, seed, room, build, active effects, and recent actions.
- Testers can export a diagnostic report from the pause or error screen.
- A single broken item, modifier, room, or discovery entry cannot invalidate the complete profile.
- Null and destroyed-object references in active registries are removed safely.

## 32. Automated test coverage

- Hex coordinates, rotation, shapes, connector rules, and legal placement receive unit tests.
- Modifier ownership, board removal, quick-install, undo, redo, and Base Spell replacement receive unit tests.
- Spell compilation and statistic calculation receive unit tests.
- Trigger loops, Trigger Energy, generation depth, activation limits, and entity budgets receive unit tests.
- Damage, armor, resistance, critical hits, weaknesses, statuses, stagger, and crowd control receive unit tests.
- Item statistics, affixes, Unique mutations, set bonuses, selling, and equipment transactions receive unit tests.
- Currency, shops, permanent purchases, banking, and reward pity rules receive unit tests.
- Profile migration and checkpoint restoration receive fixture-based tests.
- Daily and Custom Seed modes receive determinism tests.
- Room completion, rewards, route doors, shops, extraction, victory, death, and abandonment receive Play Mode tests.
- Every objective and boss phase receives Play Mode coverage.
- Recursive-spell and procedural-generation stress tests run automatically.

## 33. Manual quality assurance

- Version 1.0 uses a formal release test matrix.
- Fresh profiles and every supported migrated profile are tested.
- All three profile slots are tested independently.
- Every supported resolution, aspect ratio, window mode, and graphics preset is tested.
- Every Base Spell, modifier, item slot, set bonus, Unique mutation, enemy, Elite modifier, objective, room type, and difficulty modifier is tested.
- Intended and pathological modifier combinations are tested.
- Standard, Daily, Custom Seed, Endless, Training, and Tutorial flows are tested.
- Victory, death, abandonment, Save & Quit, Continue, forced shutdown, and save recovery are tested.
- UI opening is tested during combat, rewards, routes, shops, objectives, and boss transitions.
- Multi-hour soak tests check memory, performance, cleanup, saves, and Endless progression.
- Bugs include reproducible seeds, profiles, checkpoints, and exact reproduction steps.

## 34. Structured playtesting and balance feedback

- External playtests record completion rate, run duration, room reached, cause of death, and boss phase reached.
- Spell, modifier, perk, route, item, and merchant choices are reviewed for ignored or dominant options.
- Objective failure and completion rates are measured.
- Tutorial abandonment and confusion points are recorded.
- Workshop time and build comprehension are evaluated.
- Damage contribution is separated by spell, trigger generation, summon, status, and equipment effect.
- Testers are asked to explain how their build works; unclear explanations are treated as a readability problem.
- Beginner, intermediate, and expert groups are observed without developer guidance.
- Usability issues, balance issues, technical defects, and content shortages are tracked separately.
- At least two complete balance passes occur after feature lock.
- Optional local diagnostics remain opt-in and avoid collecting personal information.

## 35. Adaptive audio implementation

- Audio now uses a proper mixer hierarchy.
- Mixer snapshots support Home Base, exploration, combat, boss, pause, victory, and death states.
- Music transitions respond smoothly to room and combat state.
- Sound priorities and voice limits protect important cues.
- Spell sounds use controlled pitch and volume variation to reduce repetition.
- Recursive and multi-projectile builds obey sound budgets.
- Enemy telegraphs receive priority over decorative spell sounds.
- Important interactions have distinct confirmation and failure sounds.
- Ambience changes by room category.
- Audio settings persist per profile.
- Audio-device changes are handled safely.
- Essential audio information has an optional visual equivalent.

## 36. Narrative context and demo conclusion

- The player receives a clear reason to enter the dungeon.
- Home Base, the dungeon, Essence, Legendary Shards, Base Spells, Spell Modifiers, Legendary evolution, and extraction receive concise in-world explanations.
- The Dungeon Warden has a clear connection to the Relic Forge.
- Room events use concise writing that supports decisions without interrupting action.
- The demo has a defined opening, escalating objective, final confrontation, extraction, and concluding message.
- The ending acknowledges the player's final spell builds and discoveries.
- The Codex combines mechanical information with restrained world context.
- Unfinished narrative hooks that imply unavailable 1.0 systems are removed or clearly framed.
- Credits and acknowledgements are included.

## 37. Standalone Linux release

- The 1.0 DEMO is distributed as a standalone Linux build for ordinary players.
- Unity Editor remains available only for development and project inspection.
- The release is tested on Zorin OS and a clean supported Ubuntu environment.
- Executable permissions and required libraries are verified.
- Windowed, borderless, fullscreen, multiple-monitor, and desktop-scaling behavior are tested.
- Non-English system locales and paths containing spaces or parentheses are supported.
- Saves use the correct persistent-data location.
- Logs and crash diagnostics use a documented location.
- Uninstall instructions preserve profile data unless the player explicitly removes it.
- The Unity project patch remains available separately for development testing.

## 38. Release management and versioning

- The project follows semantic versioning beginning with 1.0.0-demo.
- Every build includes an internal build identifier.
- Release notes, known issues, save compatibility, and minimum requirements are published with the build.
- Development and release configurations are separated.
- Development cheats, validation overlays, and verbose logs are disabled in the public build.
- Builds are reproducible from a documented source state.
- The exact project and standalone package are archived for every release.
- Patch rollback and profile recovery procedures are documented.
- Release archives publish a checksum.
- External dependencies include the required license notices.
- Any future networking or analytics feature requires a separate privacy review and disclosure.

## 39. Final polish, consistency, and 1.0 scope lock

The final phase of development is dedicated to cohesion rather than adding another major system.

- Player-facing terminology is standardized across the HUD, Workshop, inventory, Codex, tutorial, options, run recap, and patch notes.
- Internal enum names and debug labels are removed from player-facing text.
- Buttons, cards, tabs, tooltips, and dialogs use consistent dimensions and behavior.
- Text is verified for clipping, overflow, readability, spelling, and capitalization.
- Rewards, objectives, enemies, traps, doors, and pickups cannot overlap illegally.
- Enemy bars, tooltips, interaction focus, cursor state, audio state, time scale, and room cleanup are verified during every transition.
- No important information relies exclusively on the combat log.
- Inactive modifiers are never silent.
- Permanent rewards are saved immediately and cannot be claimed twice.
- Extraction finishes all banking successfully before the run closes.
- Victory and death results remain visible until dismissed.
- The 1.0 content scope is locked to one polished Home Base, one complete dungeon region, a 30–45 minute Standard expedition, Daily and Custom Seed variants, an experimental unlocked Endless mode, a focused spell roster, validated modifiers, complete equipment coverage, three sets, transformative Uniques, authored room layouts, polished objectives, minibosses, and one complete final boss.
- Features outside this scope are postponed rather than included in an unfinished state.

---

# Content target for the 1.0 DEMO

The following target keeps the demo substantial without sacrificing quality:

- One complete, walkable Home Base.
- One polished dungeon region.
- One 30–45 minute Standard expedition.
- Daily Challenge and Custom Seed modes.
- Endless as a clearly labelled experimental unlock.
- Three selectable starting Base Spells.
- Six to eight additional discoverable Base Spells.
- Approximately 25–35 fully validated Spell Modifiers.
- Six primary enemy roles and two specialist/support enemies.
- Four to six working Elite modifiers.
- Two distinct minibosses.
- One polished three-phase final boss.
- Equipment coverage for all ten slots.
- Three complete equipment sets.
- Six to eight transformative Unique items.
- Eight to ten validated room-layout templates.
- Six deeply polished primary objectives selected from the wider objective library.
- Physical rewards, route doors, merchants, Safe Rooms, healing, level-up perks, and extraction.
- Complete Tutorial, Field Manual, Options, Profiles, Save & Quit, Continue, death, victory, and run recap flows.
- A tested standalone Linux build and a separate Unity development-project patch.

---

# Development principle

Version 1.0 is not defined by the number of systems it contains. It is defined by whether a new player can understand the rules, form a build, make meaningful decisions, finish a run, and trust that every hit, reward, purchase, save, and spell interaction behaves consistently.

No additional major system should enter the 1.0 milestone after scope lock unless it is required to make the existing expedition complete, readable, or reliable.
