# Arcane Engine v1.4.0 — Spellcraft and Spell Links

## Release status

This file describes code and content included in the v1.4.0 package. Grammar-level C# parsing and source validation were performed before packaging. Unity compilation and the Play Mode checklist still need to be run in Unity `6000.5.2f1`; untested behavior is not described here as verified.

## 1. Clearer terminology

- Renamed the run-time construction screen from Spell Workshop to **Spellcraft**.
- Kept **Spell Forge** as the name of the permanent hub station.
- Standardized **Spell Core**, **Spell Board**, **Support Rune**, **Capacity**, **Capacity Cost**, **Connected**, **Disconnected** and **Overload Chance**.
- Support Runes now show the functional name first and the flavor name second. Example: `Homing / Predator Sigil`.
- Reworded placement failures and common interface text so they refer to Support Runes and Spell Cores instead of internal modifier terminology.

Reason: a reward's function must be understandable before its setting-specific name.

## 2. Per-run spell levels

- Added levels 1–5 to every equipped spell.
- Capacity is 6, 8, 10, 12 and 15 at levels 1–5.
- Added **Spell Upgrade** to rare room rewards and shop stock.
- Taking a Spell Upgrade opens Spellcraft and requires the player to choose one equipped spell.
- A spell at level 5 cannot receive another upgrade; an unusable reward converts to 25 Gold.
- Levels are stored in the current run checkpoint and reset after the run.

Reason: the player now has a predictable way to grow one favored spell without rewarding only high-hit-rate builds.

## 3. Capacity and sealed cells

- Connected Support Runes consume their listed Capacity Cost.
- Disconnected Support Runes remain installed but provide no effect and consume no Capacity.
- A placement or move that would exceed Capacity is rejected before it changes the build.
- The outer radius-three cells are visible at level 1 and unlock in groups at levels 2–5.
- Locked cells show their required spell level.
- Spellcraft displays used Capacity, maximum Capacity and Overload Chance as separate values.

Reason: Capacity limits raw power while shape and direction remain the spatial puzzle.

## 4. Universal directional connections

- Removed Flow, Payload, Trigger, Element, Power and Wild family matching from ordinary board connections.
- Every Support Rune now uses one directional input and one or more directional outputs.
- Any output may feed any input when the sides face each other.
- Multi-hex runes calculate their output from the correct outer edge cell rather than only from their anchor.
- Connected paths are bright; disconnected runes are gray and explain why they are inactive.

Reason: rotation, shape and Capacity provide enough decisions without an additional hidden compatibility table.

## 5. Shape and Capacity review

- Reviewed all Support Runes remaining in the normal pool.
- The v1.4 pool contains 49 Support Runes with this exact shape distribution:
  - 6 one-hex runes
  - 15 two-hex runes
  - 22 three-hex runes
  - 6 four-hex runes
- Capacity Costs range from 1 to 6.
- Branching effects generally use branching shapes; major delivery replacements use four-hex shapes.
- Updated output ports so several shapes can branch without using connector families.

Difference from the plan: the plan targeted roughly ten one-hex runes and two shapes larger than four cells. Testing the actual radius-three board favored fewer one-hex pieces, but no five-cell piece was added in this implementation. This is stated directly instead of presenting the target distribution as delivered.

## 6. Basic and advanced variants

The following pairs remain separate, but their descriptions now state the relationship and practical tradeoff:

- Double Shot / Multishot
- Larger Area / Greater Area
- Faster Projectile / Acceleration
- Slow and Heavy / Heavy Projectile
- Delayed Blast / Greater Delayed Blast
- Damage Field / Greater Damage Field

Reason: the cheaper version is a valid early-run choice; the advanced version uses more Capacity or a harder shape for a stronger result.

## 7. Twelve new Support Runes

- **Cone / Fanmaker Seal:** fires at least five projectiles across a forward cone.
- **Converge / Hunter's Focus:** launches projectiles from separated origins toward the aimed point.
- **Ring Cast / Sunwheel Script:** fires at least eight projectiles around the caster.
- **Fork / Dividing Fang:** creates two reduced-power projectiles after the first hit.
- **Meteor / Falling Star:** replaces compatible delivery with a telegraphed delayed area strike.
- **Beam / Luminous Thread:** replaces compatible delivery with a short three-tick piercing line attack.
- **Burning / Cinder Mark:** adds four seconds of Fire damage over time.
- **Shock / Voltaic Brand:** makes the affected enemy take 15% increased damage for four seconds.
- **Chilling / Winter's Touch:** slows enemies; repeated applications reach the freeze threshold.
- **Close-Range Power / Nearfield Equation:** increases damage inside four metres and reduces damage beyond ten metres.
- **Long-Range Power / Farshot Formula:** reduces close damage and increases damage beyond ten metres.
- **Barrier on Cast / Warding Pulse:** manual casts grant a short barrier; triggered casts do not.

Projectile patterns are also used by familiar attacks when installed on a compatible Summon Spell Core. Range scaling applies to direct hits, explosions and chains.

## 8. Separate Spell Links system

- Removed trigger construction from the Spell Board.
- Added a separate **Spell Links** screen, opened with `L` by default or from Spellcraft.
- A link contains a source spell, condition and destination spell.
- The destination is compiled from its complete current Spell Board whenever the link activates.
- Removing a link does not change either Spell Board.

Reason: modifying a spell and deciding when it casts another spell are different tasks and are now taught separately.

## 9. Link conditions and targeting

Implemented six conditions:

- On Cast
- On Hit
- On Kill
- On Expire
- On Critical Hit
- On Status Applied

On Hit, On Kill, On Critical Hit and On Status Applied use the event position. On Expire uses the projectile, nova, zone, meteor or familiar expiry position. On Cast preserves the manually aimed target.

Each link can activate only once for one root source cast, including multi-projectile and repeated-hit spells.

## 10. Link limits and safety

- Runs begin with one Link Slot.
- Rare **Link Upgrade** rewards raise the run limit to a maximum of three.
- Link rewards offer three conditions, prioritizing conditions not already owned.
- Each condition displays Trigger Power and Link Cooldown.
- Triggered casts do not consume Mana and do not use the destination spell's manual cooldown.
- A source spell can have one outgoing standard link.
- Self-links and cyclic graphs are rejected before confirmation.
- Non-cyclic chains such as Slot 1 → Slot 2 → Slot 3 remain allowed.
- The existing six-generation global safety cap remains as a second defensive limit for Unique and Legendary effects.

## 11. Spellcraft interface changes

- Displays spell level, Capacity, Overload Chance and connected-rune count.
- Shows locked cells and their required levels.
- Shows Capacity Cost, size and compatibility for the selected Support Rune.
- Marks incompatible runes as disabled for the selected Spell Core and explains the required delivery.
- Shows green placement cells for a connected valid drop and red cells for blocked or disconnected drops.
- Adds a compiled before/after preview during a valid drag or move, including damage, projectile count, theoretical hit total, area, Mana and cooldown.
- Preserves drag, move, rotate, remove, undo, redo and saved-layout flows.

## 12. Spell Links interface

- Displays Link Slots used and available.
- Presents source, condition and destination as three explicit choices.
- Displays Trigger Power, cooldown and ready/cooling-down state.
- Supports creating and removing links during a run from any room where normal menus may be opened.
- A pending Link reward cannot be silently dismissed; the condition may be kept and linked later.

## 13. Tutorial and training

- Updated the guided training sequence to require collecting a Support Rune, opening Spellcraft, creating an actually connected placement, closing the menu and casting the changed spell.
- Updated help text for Capacity, universal directional connections and the separate Spell Links system.
- Existing durable training targets continue to display damage and restore Health after testing.

## 14. Save schema and migration

- Profile schema is now version 8.
- Run snapshot schema is now version 7.
- Checkpoints store spell levels, Link Slots, owned conditions and active links.
- Invalid link slots, same-slot links and unknown conditions are removed during normalization.
- Cyclic saved links are rejected safely while restoring through the normal link rules.
- Collision Invocation, Execution Invocation, Triskelion Invocation and Terminal Invocation leave the Support Rune pool and map to On Hit, On Kill, On Cast and On Expire conditions.
- Inward Context, Predatory Context and Impact Context leave the pool because links now use documented default targets.
- Layouts containing retired pieces are copied into `retiredSpellLayouts` before those pieces are removed from the active layout.
- Permanently unlocked trigger pieces become permanent Link-condition unlocks.
- Each removed targeting-piece content unlock refunds 18 Essence during the one-time schema-8 migration.

## 15. Validation added

The Unity validation command now checks:

- every Spell Core compiles without supports;
- every available Support Rune has a compatible Spell Core, a legal Capacity Cost and at least one connected placement;
- all twelve new supports set the required compiled runtime behavior;
- 25 named two-rune combinations connect, compile and remain within Capacity;
- level 1 and level 5 Capacity and cell availability;
- valid and cyclic Spell Link graphs;
- existing encounter, item, deterministic-layout and generated-affix checks.

## 16. Not changed by this patch

- No new bosses, enemy families or room objectives.
- No final art, animation or audio replacement.
- No controller support.
- No cyclic Spell Link engines or advanced manual link targeting.
- No permanent spell levels.
- No claim that all possible Support Rune combinations are balanced.

## 17. Required manual verification

The source package does not include results from Unity Play Mode because Unity was unavailable in the packaging environment. Before treating v1.4.0 as release-verified, run `Arcane Engine > Validate 1.4.0 Demo` and complete `TESTING_CHECKLIST_v1.4.0.md`. Record any failure as a known issue rather than changing this status to “passed.”
