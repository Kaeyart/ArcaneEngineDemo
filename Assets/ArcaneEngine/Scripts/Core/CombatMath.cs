using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ArcaneEngine
{
    public static class CombatMath
    {
        public static float PlanarDistanceSquared(Vector3 first, Vector3 second)
        {
            float x = first.x - second.x;
            float z = first.z - second.z;
            return x * x + z * z;
        }

        public static float PlanarSegmentDistanceSquared(Vector3 point, Vector3 start, Vector3 end)
        {
            point.y = start.y = end.y = 0f;
            Vector3 segment = end - start;
            float lengthSquared = segment.sqrMagnitude;
            if (lengthSquared < 0.0001f) return (point - start).sqrMagnitude;
            float t = Mathf.Clamp01(Vector3.Dot(point - start, segment) / lengthSquared);
            return (point - (start + segment * t)).sqrMagnitude;
        }
    }

    public static class RuntimeValidation
    {
        public static int ValidateRuntimeEssentials()
        {
            int failures = 0;
            if (CombatMath.PlanarSegmentDistanceSquared(new Vector3(5f, 99f, 0f), Vector3.zero, new Vector3(10f, -99f, 0f)) > 0.001f)
            {
                Debug.LogError("Combat validation failed: swept collision is sensitive to height.");
                failures++;
            }
            if (!DemoCatalog.AllCores.Any() || !DemoCatalog.AllModifiers.Any() || !DemoCatalog.AllItems.Any())
            {
                Debug.LogError("Content validation failed: one or more runtime catalogs are empty.");
                failures++;
            }
            return failures;
        }

        public static int ValidateCombatAndContent()
        {
            int failures = 0;
            if (CombatMath.PlanarSegmentDistanceSquared(new Vector3(5f, 99f, 0f), Vector3.zero, new Vector3(10f, -99f, 0f)) > 0.001f)
            {
                Debug.LogError("Combat validation failed: swept collision is sensitive to height.");
                failures++;
            }
            EquipmentInventory emptyEquipment = new EquipmentInventory();
            PlayerStats stats = new PlayerStats();
            HashSet<string> contentIds = new HashSet<string>();
            foreach (SpellCoreDefinition core in DemoCatalog.AllCores)
            {
                if (core == null || string.IsNullOrEmpty(core.id) || !contentIds.Add("core:" + core.id))
                {
                    Debug.LogError("Content validation failed: missing or duplicate Base Spell ID."); failures++; continue;
                }
                try
                {
                    CompiledSpell spell = SpellCompiler.Compile(new SpellBoard(SpellSlot.Slot1, core.id), stats, emptyEquipment);
                    if (spell == null || spell.damage < 0f || spell.cooldown <= 0f || spell.manaCost < 0f)
                    {
                        Debug.LogError("Combat validation failed for Base Spell: " + core.id);
                        failures++;
                    }
                }
                catch (System.Exception exception)
                {
                    Debug.LogError("Base Spell failed to compile: " + core.id + " · " + exception.Message);
                    failures++;
                }
            }
            foreach (SpellModifierDefinition modifier in DemoCatalog.AllModifiers)
            {
                if (modifier == null || string.IsNullOrEmpty(modifier.id) || !contentIds.Add("modifier:" + modifier.id))
                { Debug.LogError("Content validation failed: missing or duplicate Support Rune ID."); failures++; continue; }
                if (modifier.capacityCost < 1 || modifier.capacityCost > 6)
                { Debug.LogError("Capacity validation failed for Support Rune: " + modifier.id); failures++; }
                SpellCoreDefinition compatibleCore = DemoCatalog.AllCores.FirstOrDefault(modifier.IsCompatible);
                if (compatibleCore == null)
                { Debug.LogError("No compatible Spell Core exists for Support Rune: " + modifier.id); failures++; continue; }
                bool placed = false;
                SpellBoard supportBoard = new SpellBoard(SpellSlot.Slot1, compatibleCore.id) { spellLevel = 5 };
                foreach (HexCoord cell in supportBoard.AllCells().Where(value => value.DistanceFromOrigin() > 0))
                {
                    for (int rotation = 0; rotation < 6 && !placed; rotation++)
                    {
                        string reason;
                        placed = supportBoard.TryPlace(modifier.id, cell, rotation, out reason);
                    }
                    if (placed) break;
                }
                if (!placed) { Debug.LogError("Placement validation failed for Support Rune: " + modifier.id); failures++; }
                else
                {
                    CompiledSpell compiledSupport = SpellCompiler.Compile(supportBoard, stats, emptyEquipment);
                    if (compiledSupport == null || !compiledSupport.activeModifierNames.Contains(modifier.displayName) ||
                        !V14SupportRuntimeIsConfigured(modifier, compiledSupport))
                    { Debug.LogError("Runtime compilation validation failed for Support Rune: " + modifier.id); failures++; }
                }
            }
            foreach (ItemDefinition item in DemoCatalog.AllItems)
                if (item == null || string.IsNullOrEmpty(item.id) || !contentIds.Add("item:" + item.id))
                { Debug.LogError("Content validation failed: missing or duplicate item ID."); failures++; }

            foreach (ItemDefinition unique in DemoCatalog.AllItems.Where(value => value != null && value.rarity == ItemRarity.Unique))
            {
                EquipmentInventory uniqueEquipment = new EquipmentInventory();
                ItemInstance uniqueInstance = new ItemInstance(unique.id, 20);
                uniqueEquipment.equipped[unique.slot] = uniqueInstance;
                foreach (SpellCoreDefinition core in DemoCatalog.AllCores)
                {
                    try
                    {
                        CompiledSpell compiled = SpellCompiler.Compile(new SpellBoard(SpellSlot.Slot1, core.id), stats, uniqueEquipment);
                        if (compiled == null || compiled.damage < 0f || compiled.triggers.Count > BalanceTuning.MaximumTriggerActivations)
                        { Debug.LogError("Unique compatibility failed: " + unique.id + " with " + core.id); failures++; }
                    }
                    catch (System.Exception exception)
                    { Debug.LogError("Unique compatibility exception: " + unique.id + " with " + core.id + " · " + exception.Message); failures++; }
                }
            }

            SpellCoreDefinition firstCore = DemoCatalog.AllCores.FirstOrDefault();
            SpellBoard geometry = firstCore == null ? null : new SpellBoard(SpellSlot.Slot1, firstCore.id);
            if (geometry == null) { Debug.LogError("Content validation failed: no Base Spells are available."); failures++; }
            else if (geometry.AllCells().Count != 37) { Debug.LogError("Hex validation failed: radius-three board must contain 37 cells."); failures++; }
            HexCoord rotated = new HexCoord(2, -1);
            for (int i = 0; i < 6; i++) rotated = HexCoord.Rotate(rotated, 1);
            if (!rotated.Equals(new HexCoord(2, -1))) { Debug.LogError("Hex validation failed: six rotations must return to origin orientation."); failures++; }

            SpellBoard capacityBoard = new SpellBoard(SpellSlot.Slot1, firstCore == null ? "fireball" : firstCore.id);
            if (capacityBoard.Capacity != 6) { Debug.LogError("Spell Level 1 must provide 6 Capacity."); failures++; }
            capacityBoard.spellLevel = 5;
            if (capacityBoard.Capacity != 15 || capacityBoard.AllCells().Any(cell => !capacityBoard.IsCellUnlocked(cell)))
            { Debug.LogError("Spell Level 5 must provide 15 Capacity and unlock its standard board."); failures++; }

            List<SpellLinkSave> validLinks = new List<SpellLinkSave>
            {
                new SpellLinkSave { sourceSlot = 0, destinationSlot = 1 },
                new SpellLinkSave { sourceSlot = 1, destinationSlot = 2 }
            };
            if (SpellLinkRules.HasCycle(validLinks)) { Debug.LogError("Spell Link validation rejected a non-cyclic chain."); failures++; }
            validLinks.Add(new SpellLinkSave { sourceSlot = 2, destinationSlot = 0 });
            if (!SpellLinkRules.HasCycle(validLinks)) { Debug.LogError("Spell Link validation failed to detect a cycle."); failures++; }

            string[][] curatedCombinations =
            {
                new[] { "double_shot", "homing" }, new[] { "triple", "burning" }, new[] { "cone", "burning" },
                new[] { "converge", "long_range" }, new[] { "ring_cast", "homing" }, new[] { "fork", "toxic" },
                new[] { "meteor_delivery", "burning" }, new[] { "beam_delivery", "shock" }, new[] { "pierce", "long_range" },
                new[] { "chain", "shock" }, new[] { "return", "burning" }, new[] { "orbit", "chilling" },
                new[] { "split_distance", "fork" }, new[] { "accelerate", "long_range" }, new[] { "decelerate", "close_range" },
                new[] { "delay", "large_area" }, new[] { "trail", "burning" }, new[] { "zone", "shock" },
                new[] { "status_spread", "burning" }, new[] { "status_consume", "chilling" }, new[] { "summon_payload", "burning" },
                new[] { "overkill", "close_range" }, new[] { "blood_cost", "barrier_cast" }, new[] { "efficient", "beam_delivery" },
                new[] { "wild", "unstable" }
            };
            foreach (string[] combination in curatedCombinations)
            {
                SpellBoard combinationBoard;
                if (!TryBuildConnectedCombination("fireball", combination[0], combination[1], out combinationBoard))
                { Debug.LogError("Curated Spellcraft combination could not be connected: " + string.Join(" + ", combination)); failures++; continue; }
                CompiledSpell combined = SpellCompiler.Compile(combinationBoard, stats, emptyEquipment);
                if (combined == null || combinationBoard.GetActivePlacements().Count != 2 || combinationBoard.UsedCapacity() > combinationBoard.Capacity)
                { Debug.LogError("Curated Spellcraft combination failed compilation: " + string.Join(" + ", combination)); failures++; }
            }

            if (V1Determinism.StableHash("arcane-engine") != V1Determinism.StableHash("arcane-engine"))
            { Debug.LogError("Seed validation failed: stable hashes differ."); failures++; }
            UnityEngine.Random.State previous = UnityEngine.Random.state;
            for (int seed = 1; seed <= 2000; seed++)
            {
                UnityEngine.Random.InitState(seed);
                List<Vector3> placements = new List<Vector3>();
                for (int i = 0; i < 8; i++)
                {
                    Vector3 position = DungeonLayoutValidator.FindPlacement(-10f, 10f, 1.2f, placements);
                    if (!DungeonLayoutValidator.IsValidPlacement(position, 1.2f, placements))
                    { Debug.LogError("Generation validation failed at seed " + seed + "."); failures++; break; }
                    placements.Add(position);
                }
            }
            UnityEngine.Random.state = previous;

            failures += V11Itemization.ValidateGeneratedItems(10000);
            failures += V12SystemsDirector.ValidateReleaseSurface();

            SpellBuildValidationReport report = SpellBuildValidator.Validate(geometry, stats, emptyEquipment);
            if (geometry == null || !report.valid || string.IsNullOrEmpty(report.stableId))
            { Debug.LogError("Spell build validation failed for an empty valid board."); failures++; }
            if (BalanceTuning.ArmorMultiplier(0f) != 1f || BalanceTuning.ResistanceMultiplier(1f) < 0.34f)
            { Debug.LogError("Balance validation failed: mitigation caps are invalid."); failures++; }

            if (failures == 0) Debug.Log("Arcane Engine v2.0 source validation passed: input contexts, combat plane, locked loadouts, unsecured loot ownership, spell edit locks, Capacity, Support Rune placement, Spell Link cycles, catalogs, deterministic seeds, generated layouts, affix items, and procedural presentation are valid.");
            return failures;
        }

        private static bool V14SupportRuntimeIsConfigured(SpellModifierDefinition modifier, CompiledSpell spell)
        {
            switch (modifier.effect)
            {
                case ModifierEffect.ConePattern: return spell.projectilePattern == ProjectilePattern.Cone && spell.projectileCount >= 5;
                case ModifierEffect.ConvergingProjectiles: return spell.projectilePattern == ProjectilePattern.Converge && spell.projectileCount >= 3;
                case ModifierEffect.RingPattern: return spell.projectilePattern == ProjectilePattern.Ring && spell.projectileCount >= 8;
                case ModifierEffect.ForkOnHit: return spell.splitOnHit && spell.splitCount >= 2;
                case ModifierEffect.MeteorDelivery: return spell.delivery == SpellDelivery.Meteor && spell.detonationDelay > 0f;
                case ModifierEffect.BeamDelivery: return spell.delivery == SpellDelivery.Beam && spell.repeatCount >= 3;
                case ModifierEffect.Burning: return spell.burnDamage > 0f && spell.burnDuration > 0f;
                case ModifierEffect.Shock: return spell.shockMagnitude > 0f && spell.shockDuration > 0f;
                case ModifierEffect.Chill: return spell.chillMagnitude > 0f && spell.chillDuration > 0f;
                case ModifierEffect.Bleeding: return spell.bleedDamage > 0f && spell.bleedDuration > 0f;
                case ModifierEffect.Cursing: return spell.curseMagnitude > 0f && spell.curseDuration > 0f;
                case ModifierEffect.Weaken: return spell.weakenMagnitude > 0f && spell.weakenDuration > 0f;
                case ModifierEffect.Vulnerability: return spell.vulnerabilityMagnitude > 0f && spell.vulnerabilityDuration > 0f;
                case ModifierEffect.CloseRangePower: return spell.closeRangeMultiplier > 1f && spell.longRangeMultiplier < 1f;
                case ModifierEffect.LongRangePower: return spell.longRangeMultiplier > 1f && spell.closeRangeMultiplier < 1f;
                case ModifierEffect.BarrierOnCast: return spell.barrierOnManualCast > 0f;
                default: return true;
            }
        }

        private static bool TryBuildConnectedCombination(string coreId, string firstId, string secondId, out SpellBoard result)
        {
            SpellBoard geometry = new SpellBoard(SpellSlot.Slot1, coreId) { spellLevel = 5 };
            List<HexCoord> cells = geometry.AllCells().Where(value => value.DistanceFromOrigin() > 0).ToList();
            foreach (HexCoord firstCell in cells)
            for (int firstRotation = 0; firstRotation < 6; firstRotation++)
            {
                SpellBoard candidate = new SpellBoard(SpellSlot.Slot1, coreId) { spellLevel = 5 };
                string ignored;
                if (!candidate.TryPlace(firstId, firstCell, firstRotation, out ignored) || candidate.GetActivePlacements().Count != 1) continue;
                foreach (HexCoord secondCell in cells)
                for (int secondRotation = 0; secondRotation < 6; secondRotation++)
                {
                    SpellBoard completed = new SpellBoard(SpellSlot.Slot1, coreId) { spellLevel = 5 };
                    if (!completed.TryPlace(firstId, firstCell, firstRotation, out ignored)) continue;
                    if (!completed.TryPlace(secondId, secondCell, secondRotation, out ignored)) continue;
                    if (completed.GetActivePlacements().Count != 2 || completed.UsedCapacity() > completed.Capacity) continue;
                    result = completed;
                    return true;
                }
            }
            result = null;
            return false;
        }
    }
}
