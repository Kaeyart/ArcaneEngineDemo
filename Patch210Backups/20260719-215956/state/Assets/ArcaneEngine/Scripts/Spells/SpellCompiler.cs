using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public static class SpellCompiler
    {
        public static CompiledSpell Compile(SpellBoard board, PlayerStats stats, EquipmentInventory equipment)
        {
            if (board == null || string.IsNullOrEmpty(board.coreId)) return null;
            SpellCoreDefinition core = DemoCatalog.GetCore(board.coreId);
            if (core == null) throw new InvalidOperationException("Missing spell core: " + board.coreId);

            CompiledSpell spell = FromCore(core, board.slot);
            if (!string.IsNullOrEmpty(board.relicId)) ApplyRelic(spell, MegaCatalog.GetRelic(board.relicId));
            equipment.ApplyFoundationMutation(spell, board.slot);
            spell.activeUniqueMutations.AddRange(equipment.ActiveMutations);

            List<PlacedModifier> active = board.GetActivePlacements();
            foreach (PlacedModifier placed in active)
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(placed.modifierId);
                if (definition == null || !definition.availableAsSupport) continue;
                ApplyModifier(spell, definition);
                spell.activeModifierNames.Add(definition.displayName);
                spell.executionLayers.Add(definition.category + ": " + definition.displayName);
            }

            if (active.Count < board.placed.Count)
                spell.warnings.Add((board.placed.Count - active.Count) + " Support Rune(s) are disconnected and provide no effect.");
            int conversions = active.Count(p =>
            {
                SpellModifierDefinition d = DemoCatalog.GetModifier(p.modifierId);
                return d != null && (d.effect == ModifierEffect.ToxicConversion || d.effect == ModifierEffect.FrostConversion || d.effect == ModifierEffect.LightningConversion);
            });
            if (conversions > 1) spell.warnings.Add("Multiple element conversions are legal; execution order makes the final connected conversion dominant.");

            equipment.ApplyGlobalSpellStats(spell);
            spell.damage *= stats.spellPower;
            spell.manaCost = Mathf.Max(0f, spell.manaCost * stats.manaCostMultiplier);
            float scaledCooldown =
    spell.cooldown * stats.cooldownMultiplier;

spell.cooldown =
    spell.castMethod == SpellCastMethod.Channeled
        ? Mathf.Max(0.08f, scaledCooldown)
        : Mathf.Max(0f, scaledCooldown);
            spell.instability = Mathf.Max(0f, spell.instability - stats.instabilityResistance);
            spell.projectileCount = Mathf.Clamp(spell.projectileCount, 1, 24);
            spell.repeatCount = Mathf.Clamp(spell.repeatCount, 1, 8);
            spell.summonCount = Mathf.Clamp(spell.summonCount, 0, 8);
            CompiledSpell result = BalanceTuning.EnforceSpellCaps(spell);
            SpellVisualCompiler.Rebuild(result);
            ProceduralSpellPresentation.Compile(result, board);
            return result;
        }

        private static CompiledSpell FromCore(SpellCoreDefinition core, SpellSlot slot)
        {
            CompiledSpell result = new CompiledSpell
            {
                coreId = core.id,
                displayName = core.displayName,
                slot = slot,
                delivery = core.delivery,
                castMethod = core.castMethod,
                element = core.element,
                primaryColor = core.color,
                accentColor = Color.white,
                damage = core.baseDamage,
                manaCost = core.manaCost,
                cooldown = core.cooldown,
                speed = core.speed,
                lifetime = core.lifetime,
                size = core.size,
                radius = core.radius,
                explosionRadius = core.delivery == SpellDelivery.Projectile || core.delivery == SpellDelivery.Meteor ? core.radius : 0f,
                zoneDuration = core.delivery == SpellDelivery.Zone ? core.lifetime : 0f,
                summonCount = core.delivery == SpellDelivery.Summon ? 1 : 0
            };
            if (core.element == SpellElement.Frost) result.freezeSeconds = 0.8f;
            if (core.element == SpellElement.Toxic) { result.poisonDamage = core.baseDamage * 0.16f; result.poisonDuration = 4f; }
            if (core.id == "storm_orb") result.chainTargets = 2;
            if (core.id == "gravity_field") result.pullsEnemies = true;
            result.executionLayers.Add("Core: " + core.displayName);
            result.executionLayers.Add("Delivery: " + core.delivery);
            result.executionLayers.Add("Element: " + core.element);
            return result;
        }

        private static void ApplyRelic(CompiledSpell spell, RelicDefinition relic)
        {
            if (relic == null || relic.sourceCoreId != spell.coreId) return;
            spell.relicId = relic.id;
            spell.relicSignature = relic.signature;
            spell.displayName = relic.displayName;
            spell.primaryColor = relic.color;
            spell.accentColor = Color.white;
            spell.executionLayers.Add("Legendary Effect: " + relic.displayName);

            switch (relic.signature)
            {
                case RelicSignature.DyingSun:
                    spell.delivery = SpellDelivery.Projectile; spell.damage *= 1.55f; spell.speed *= 0.3f; spell.lifetime *= 1.8f;
                    spell.size *= 2.2f; spell.explosionRadius = Mathf.Max(4.5f, spell.explosionRadius * 2f); spell.pullsEnemies = true; spell.isMiniatureSun = true;
                    spell.manaCost *= 1.4f; break;
                case RelicSignature.PhoenixSeed:
                    spell.damage *= 1.15f; spell.splitCount += 3; spell.splitOnHit = true; spell.manaCost *= 1.3f; break;
                case RelicSignature.Hellstorm:
                    spell.delivery = SpellDelivery.Meteor; spell.repeatCount = 6; spell.damage *= 0.62f; spell.radius = Mathf.Max(4f, spell.radius * 2f); spell.manaCost *= 1.45f; break;
                case RelicSignature.AbsoluteZero:
                    spell.freezeSeconds = Mathf.Max(2.4f, spell.freezeSeconds); spell.repeatCount += 1; spell.consumesStatus = true; spell.damage *= 1.2f; break;
                case RelicSignature.FrostOrbit:
                    spell.delivery = SpellDelivery.Projectile; spell.projectileCount = 5; spell.orbitCaster = true; spell.speed = 5f; spell.lifetime = 5f; spell.damage *= 0.55f; break;
                case RelicSignature.ThunderJudgment:
                    spell.delivery = SpellDelivery.Meteor; spell.damage *= 1.7f; spell.detonationDelay = 0.65f; spell.chainTargets += 2; break;
                case RelicSignature.LivingCircuit:
                    spell.chainTargets += 6; spell.repeatCount = 2; spell.damage *= 0.72f; break;
                case RelicSignature.InfiniteBeam:
                    spell.delivery = SpellDelivery.Beam; spell.repeatCount = 4; spell.repeatDelay = 0.12f; spell.damage *= 0.72f; break;
                case RelicSignature.PrismLance:
                    spell.delivery = SpellDelivery.Beam; spell.pierce += 5; spell.splitCount += 3; spell.damage *= 1.2f; break;
                case RelicSignature.FallingWorld:
                    spell.delivery = SpellDelivery.Meteor; spell.damage *= 2.1f; spell.detonationDelay = 1.25f; spell.explosionRadius = Mathf.Max(5f, spell.radius); spell.zoneDuration = 6f; break;
                case RelicSignature.Starfall:
                    spell.delivery = SpellDelivery.Meteor; spell.repeatCount = 8; spell.repeatDelay = 0.18f; spell.damage *= 0.45f; break;
                case RelicSignature.WispLegion:
                    spell.delivery = SpellDelivery.Summon; spell.summonCount = 4; spell.damage *= 0.62f; break;
                case RelicSignature.SoulTwin:
                    spell.delivery = SpellDelivery.Summon; spell.summonCount = 1; spell.lifetime = 18f; spell.damage *= 1.25f; break;
                case RelicSignature.RiftStep:
                    spell.delivery = SpellDelivery.Movement; spell.zoneDuration = 3f; spell.damage *= 1.35f; break;
                case RelicSignature.Afterimage:
                    spell.delivery = SpellDelivery.Movement; spell.triggers.Add(NewTrigger("relic_afterimage", TriggerMoment.OnCast, SpellSlot.Slot1, 36f, 0.5f, 2, TargetContext.Player)); break;
                case RelicSignature.WorldWall:
                    spell.delivery = SpellDelivery.Zone; spell.reflectsProjectiles = true; spell.zoneDuration = Mathf.Max(6f, spell.lifetime); break;
                case RelicSignature.DevouringZone:
                    spell.delivery = SpellDelivery.Zone; spell.pullsEnemies = true; spell.zoneDuration = Mathf.Max(6f, spell.lifetime); spell.damage *= 1.35f; break;
                case RelicSignature.SpellbladeDance:
                    spell.delivery = SpellDelivery.Melee; spell.repeatCount = 4; spell.repeatDelay = 0.12f; spell.damage *= 0.62f; break;
                case RelicSignature.ExecutionEdge:
                    spell.delivery = SpellDelivery.Melee; spell.storesExcessDamage = true; spell.damage *= 1.45f; break;
                case RelicSignature.PerfectAegis:
                    spell.delivery = SpellDelivery.Defensive; spell.lifetime = 2.4f; spell.cooldown *= 0.8f; break;
                case RelicSignature.RetributionWard:
                    spell.delivery = SpellDelivery.Defensive; spell.explosionRadius = Mathf.Max(4f, spell.radius); spell.damage *= 1.5f; break;
                case RelicSignature.ShatterTide:
                    spell.delivery = SpellDelivery.Nova; spell.repeatCount = 3; spell.repeatDelay = 0.28f; spell.freezeSeconds = Mathf.Max(1.8f, spell.freezeSeconds); spell.consumesStatus = true; spell.damage *= 0.78f; break;
                case RelicSignature.StormbrandEcho:
                    spell.delivery = SpellDelivery.Meteor; spell.repeatCount = 3; spell.repeatDelay = 0.32f; spell.chainTargets += 2; spell.damage *= 0.82f; break;
                case RelicSignature.HorizonRay:
                    spell.delivery = SpellDelivery.Beam; spell.element = SpellElement.Void; spell.pullsEnemies = true; spell.explosionRadius = Mathf.Max(2.5f, spell.radius); spell.damage *= 1.25f; break;
                case RelicSignature.CometWake:
                    spell.delivery = SpellDelivery.Meteor; spell.repeatCount = 5; spell.repeatDelay = 0.18f; spell.zoneDuration = 3.5f; spell.damage *= 0.62f; break;
                case RelicSignature.CovenantSwarm:
                    spell.delivery = SpellDelivery.Summon; spell.summonCount = 3; spell.repeatCount = 2; spell.damage *= 0.72f; break;
                case RelicSignature.PhaseCascade:
                    spell.delivery = SpellDelivery.Movement; spell.repeatCount = 3; spell.repeatDelay = 0.1f; spell.explosionRadius = Mathf.Max(2.8f, spell.radius); spell.damage *= 0.75f; break;
                case RelicSignature.MirrorBastion:
                    spell.delivery = SpellDelivery.Zone; spell.reflectsProjectiles = true; spell.repeatCount = 2; spell.explosionRadius = Mathf.Max(3f, spell.radius); spell.zoneDuration = 7f; break;
                case RelicSignature.BloodMoonEdge:
                    spell.delivery = SpellDelivery.Melee; spell.returnsToCaster = true; spell.healthCost += Mathf.Max(3f, spell.damage * 0.08f); spell.damage *= 1.8f; break;
                case RelicSignature.ChronoGuard:
                    spell.delivery = SpellDelivery.Defensive; spell.chillMagnitude = Mathf.Max(0.45f, spell.chillMagnitude); spell.cooldown *= 0.55f; spell.explosionRadius = Mathf.Max(3.5f, spell.radius); break;
                case RelicSignature.PlagueOrchard:
                    spell.delivery = SpellDelivery.Projectile; spell.zoneDuration = 5f; spell.spreadsStatus = true; spell.poisonDamage += spell.damage * 0.3f; break;
                case RelicSignature.VenomRebirth:
                    spell.delivery = SpellDelivery.Projectile; spell.splitOnHit = true; spell.splitCount += 3; spell.homingStrength = Mathf.Max(4f, spell.homingStrength); spell.damage *= 0.8f; break;
                case RelicSignature.CrystalGuillotine:
                    spell.delivery = SpellDelivery.Meteor; spell.detonationDelay = 0.8f; spell.pierce += 6; spell.consumesStatus = true; spell.damage *= 1.9f; break;
                case RelicSignature.WintersNeedle:
                    spell.delivery = SpellDelivery.Beam; spell.repeatCount = 5; spell.repeatDelay = 0.1f; spell.pierce += 4; spell.damage *= 0.48f; break;
                case RelicSignature.TempestCrown:
                    spell.delivery = SpellDelivery.Projectile; spell.projectileCount = 4; spell.orbitCaster = true; spell.chainTargets += 2; spell.damage *= 0.66f; break;
                case RelicSignature.BallLightningEngine:
                    spell.delivery = SpellDelivery.Projectile; spell.speed *= 0.38f; spell.lifetime *= 1.8f; spell.repeatCount = 5; spell.chainTargets += 3; spell.damage *= 0.58f; break;
                case RelicSignature.EventHorizon:
                    spell.delivery = SpellDelivery.Zone; spell.pullsEnemies = true; spell.consumesStatus = true; spell.zoneDuration = 6f; spell.explosionRadius = Mathf.Max(5f, spell.radius); spell.damage *= 1.35f; break;
                case RelicSignature.CrushingOrbit:
                    spell.delivery = SpellDelivery.Projectile; spell.projectileCount = 3; spell.orbitCaster = true; spell.pullsEnemies = true; spell.repeatCount = 3; spell.damage *= 0.6f; break;
            }
        }

        private static void ApplyModifier(CompiledSpell spell, SpellModifierDefinition modifier)
        {
            switch (modifier.effect)
            {
                case ModifierEffect.MultiProjectile:
                    spell.projectileCount += Mathf.Max(1, Mathf.RoundToInt(modifier.magnitude)); spell.spreadDegrees = Mathf.Max(spell.spreadDegrees, 15f);
                    spell.damage *= 0.72f; spell.manaCost *= 1.18f; break;
                case ModifierEffect.ToxicConversion:
                    spell.element = SpellElement.Toxic; spell.primaryColor = new Color(0.25f, 1f, 0.18f); spell.accentColor = new Color(0.75f, 1f, 0.1f);
                    spell.poisonDamage += spell.damage * 0.16f; spell.poisonDuration = Mathf.Max(spell.poisonDuration, 4f); break;
                case ModifierEffect.FrostConversion:
                    spell.element = SpellElement.Frost; spell.primaryColor = new Color(0.25f, 0.85f, 1f); spell.freezeSeconds = Mathf.Max(spell.freezeSeconds, 1.4f); spell.damage *= 0.9f; break;
                case ModifierEffect.LightningConversion:
                    spell.element = SpellElement.Lightning; spell.primaryColor = new Color(0.42f, 0.65f, 1f); spell.chainTargets += 1; spell.damage *= 0.94f; break;
                case ModifierEffect.Homing:
                    if (!ProjectileCompatible(spell)) spell.warnings.Add(modifier.displayName + " requires moving projectile delivery.");
                    else { spell.homingStrength += modifier.magnitude; spell.speed *= 0.88f; spell.manaCost *= 1.1f; } break;
                case ModifierEffect.ArcFlight:
                    if (!ProjectileCompatible(spell)) spell.warnings.Add(modifier.displayName + " requires moving projectile delivery.");
                    else { spell.arcAmount += modifier.magnitude; spell.damage *= 1.08f; } break;
                case ModifierEffect.Pierce:
                    spell.pierce += Mathf.Max(1, Mathf.RoundToInt(modifier.magnitude)); spell.damage *= 0.9f; break;
                case ModifierEffect.BiggerExplosion:
                    spell.explosionRadius = Mathf.Max(1f, spell.explosionRadius + modifier.magnitude); spell.radius += modifier.magnitude * 0.35f; spell.manaCost *= 1.12f; break;
                case ModifierEffect.ChainLightning:
                    spell.chainTargets += Mathf.Max(1, Mathf.RoundToInt(modifier.magnitude)); spell.accentColor = new Color(0.5f, 0.7f, 1f); spell.manaCost *= 1.15f; break;
                case ModifierEffect.RepeatPulse:
                    spell.repeatCount += Mathf.Max(1, Mathf.RoundToInt(modifier.magnitude)); spell.damage *= 0.78f; spell.manaCost *= 1.2f; break;
                case ModifierEffect.TriggerSlot2OnHit:
                    spell.triggers.Add(NewTrigger(modifier.id, TriggerMoment.OnHit, SpellSlot.Slot2, 32f, 0.65f, 6, spell.targetContext)); spell.manaCost *= 1.18f; break;
                case ModifierEffect.TriggerSlot1OnKill:
                    spell.triggers.Add(NewTrigger(modifier.id, TriggerMoment.OnKill, SpellSlot.Slot1, 38f, 0.55f, 4, spell.targetContext)); break;
                case ModifierEffect.TriggerSlot3OnCast:
                    spell.triggers.Add(NewTrigger(modifier.id, TriggerMoment.OnCast, SpellSlot.Slot3, 44f, 0.45f, 2, spell.targetContext)); break;
                case ModifierEffect.TriggerSlot1OnExpire:
                    spell.triggers.Add(NewTrigger(modifier.id, TriggerMoment.OnExpire, SpellSlot.Slot1, 34f, 0.5f, 3, spell.targetContext)); break;
                case ModifierEffect.NovaDelivery:
                    spell.delivery = SpellDelivery.Nova; spell.projectileCount = 1; spell.speed = 0f; spell.radius = Mathf.Max(3.5f, spell.radius * 2.5f);
                    spell.explosionRadius = 0f; spell.displayName = "Nova " + spell.displayName; break;
                case ModifierEffect.EfficientCasting:
                    spell.manaCost *= Mathf.Clamp(modifier.magnitude, 0.35f, 0.95f); spell.damage *= 0.9f; break;
                case ModifierEffect.UnstablePower:
                    spell.damage *= 1f + modifier.magnitude; spell.size *= 1.15f; spell.instability += 25f * modifier.magnitude; break;
                case ModifierEffect.Bounce:
                    spell.bounce += Mathf.Max(1, Mathf.RoundToInt(modifier.magnitude)); spell.damage *= 0.88f; break;
                case ModifierEffect.Return:
                    spell.returnsToCaster = true; spell.lifetime *= 1.5f; break;
                case ModifierEffect.Orbit:
                    spell.orbitCaster = true; spell.lifetime = Mathf.Max(4f, spell.lifetime); spell.speed *= 0.7f; break;
                case ModifierEffect.SplitAfterDistance:
                    spell.splitCount += Mathf.Max(2, Mathf.RoundToInt(modifier.magnitude)); spell.splitDistance = Mathf.Max(4f, spell.radius + 4f); break;
                case ModifierEffect.SplitOnHit:
                    spell.splitCount += Mathf.Max(2, Mathf.RoundToInt(modifier.magnitude)); spell.splitOnHit = true; break;
                case ModifierEffect.Accelerate:
                    spell.acceleration += modifier.magnitude; break;
                case ModifierEffect.Decelerate:
                    spell.acceleration += modifier.magnitude; spell.damage *= 1.28f; break;
                case ModifierEffect.DelayedDetonation:
                    spell.detonationDelay = Mathf.Max(spell.detonationDelay, modifier.magnitude); spell.damage *= 1.35f; break;
                case ModifierEffect.PersistentTrail:
                    spell.trailDuration = Mathf.Max(spell.trailDuration, modifier.magnitude); break;
                case ModifierEffect.DamageZone:
                    spell.zoneDuration = Mathf.Max(spell.zoneDuration, modifier.magnitude); break;
                case ModifierEffect.StatusSpread:
                    spell.spreadsStatus = true; break;
                case ModifierEffect.StatusConsume:
                    spell.consumesStatus = true; spell.damage *= 1f + modifier.magnitude; break;
                case ModifierEffect.SummonPayload:
                    spell.summonCount += Mathf.Max(1, Mathf.RoundToInt(modifier.magnitude)); break;
                case ModifierEffect.ReflectProjectiles:
                    spell.reflectsProjectiles = true; break;
                case ModifierEffect.StoreExcessDamage:
                    spell.storesExcessDamage = true; break;
                case ModifierEffect.HealthSacrifice:
                    spell.healthCost += modifier.magnitude; spell.damage *= 1.4f; break;
                case ModifierEffect.ManaConversion:
                    spell.manaCost *= 1.3f; spell.damage *= 1f + modifier.magnitude; break;
                case ModifierEffect.TargetPlayer:
                    ApplyTargetContext(spell, TargetContext.Player); break;
                case ModifierEffect.TargetEnemy:
                    ApplyTargetContext(spell, TargetContext.NearestEnemy); break;
                case ModifierEffect.TargetImpact:
                    ApplyTargetContext(spell, TargetContext.Impact); break;
                case ModifierEffect.WildConnector:
                    spell.instability += 12f; break;
                case ModifierEffect.ConePattern:
                    if (!ProjectileCompatible(spell)) spell.warnings.Add(modifier.displayName + " requires projectile delivery.");
                    else
                    {
                        spell.projectilePattern = ProjectilePattern.Cone;
                        spell.projectileCount = Mathf.Max(5, spell.projectileCount);
                        spell.spreadDegrees = Mathf.Max(21f, spell.spreadDegrees);
                        spell.damage *= 0.56f;
                        spell.manaCost *= 1.18f;
                    }
                    break;
                case ModifierEffect.ConvergingProjectiles:
                    if (!ProjectileCompatible(spell)) spell.warnings.Add(modifier.displayName + " requires projectile delivery.");
                    else
                    {
                        spell.projectilePattern = ProjectilePattern.Converge;
                        spell.projectileCount = Mathf.Max(3, spell.projectileCount);
                        spell.damage *= 0.82f;
                        spell.manaCost *= 1.12f;
                    }
                    break;
                case ModifierEffect.RingPattern:
                    if (!ProjectileCompatible(spell)) spell.warnings.Add(modifier.displayName + " requires projectile delivery.");
                    else
                    {
                        spell.projectilePattern = ProjectilePattern.Ring;
                        spell.projectileCount = Mathf.Max(8, spell.projectileCount);
                        spell.damage *= 0.42f;
                        spell.manaCost *= 1.24f;
                    }
                    break;
                case ModifierEffect.ForkOnHit:
                    if (!ProjectileCompatible(spell)) spell.warnings.Add(modifier.displayName + " requires projectile delivery.");
                    else { spell.splitCount += 2; spell.splitOnHit = true; spell.damage *= 0.88f; }
                    break;
                case ModifierEffect.MeteorDelivery:
                    spell.delivery = SpellDelivery.Meteor;
                    spell.projectileCount = 1;
                    spell.detonationDelay = Mathf.Max(0.75f, spell.detonationDelay);
                    spell.explosionRadius = Mathf.Max(2.8f, spell.explosionRadius);
                    spell.damage *= 1.22f;
                    spell.manaCost *= 1.25f;
                    spell.displayName = "Meteor " + spell.displayName;
                    break;
                case ModifierEffect.BeamDelivery:
                    spell.delivery = SpellDelivery.Beam;
                    spell.projectileCount = 1;
                    spell.radius = Mathf.Max(9f, spell.radius);
                    spell.pierce = Mathf.Max(3, spell.pierce);
                    spell.repeatCount = Mathf.Max(3, spell.repeatCount);
                    spell.repeatDelay = Mathf.Min(spell.repeatDelay, 0.12f);
                    spell.damage *= 0.42f;
                    spell.manaCost *= 1.18f;
                    spell.displayName = spell.displayName + " Beam";
                    break;
                case ModifierEffect.Burning:
                    spell.burnDamage += spell.damage * Mathf.Max(0.08f, modifier.magnitude);
                    spell.burnDuration = Mathf.Max(spell.burnDuration, 4f);
                    break;
                case ModifierEffect.Shock:
                    spell.shockMagnitude = Mathf.Max(spell.shockMagnitude, Mathf.Clamp(modifier.magnitude, 0.05f, 0.3f));
                    spell.shockDuration = Mathf.Max(spell.shockDuration, 4f);
                    break;
                case ModifierEffect.Chill:
                    spell.chillMagnitude = Mathf.Max(spell.chillMagnitude, Mathf.Clamp(modifier.magnitude, 0.1f, 0.6f));
                    spell.chillDuration = Mathf.Max(spell.chillDuration, 3f);
                    break;
                case ModifierEffect.Bleeding:
                    spell.bleedDamage += spell.damage * Mathf.Max(0.08f, modifier.magnitude);
                    spell.bleedDuration = Mathf.Max(spell.bleedDuration, 4f);
                    break;
                case ModifierEffect.Cursing:
                    spell.curseMagnitude = Mathf.Max(spell.curseMagnitude, Mathf.Clamp(modifier.magnitude, 0.05f, 0.5f));
                    spell.curseDuration = Mathf.Max(spell.curseDuration, 5f);
                    break;
                case ModifierEffect.Weaken:
                    spell.weakenMagnitude = Mathf.Max(spell.weakenMagnitude, Mathf.Clamp(modifier.magnitude, 0.05f, 0.5f));
                    spell.weakenDuration = Mathf.Max(spell.weakenDuration, 4f);
                    break;
                case ModifierEffect.Vulnerability:
                    spell.vulnerabilityMagnitude = Mathf.Max(spell.vulnerabilityMagnitude, Mathf.Clamp(modifier.magnitude, 0.05f, 0.5f));
                    spell.vulnerabilityDuration = Mathf.Max(spell.vulnerabilityDuration, 4f);
                    break;
                case ModifierEffect.CloseRangePower:
                    spell.closeRangeMultiplier *= 1f + Mathf.Max(0.1f, modifier.magnitude);
                    spell.longRangeMultiplier *= 0.85f;
                    break;
                case ModifierEffect.LongRangePower:
                    spell.closeRangeMultiplier *= 0.85f;
                    spell.longRangeMultiplier *= 1f + Mathf.Max(0.1f, modifier.magnitude);
                    break;
                case ModifierEffect.BarrierOnCast:
                    spell.barrierOnManualCast += spell.damage * Mathf.Max(0.25f, modifier.magnitude);
                    break;
            }
            spell.instability += modifier.instability;
        }

        private static bool ProjectileCompatible(CompiledSpell spell)
        {
            return spell.delivery == SpellDelivery.Projectile || spell.delivery == SpellDelivery.Summon;
        }

        private static void ApplyTargetContext(CompiledSpell spell, TargetContext context)
        {
            spell.targetContext = context;
            foreach (TriggerSpec trigger in spell.triggers) trigger.targetContext = context;
        }

        private static TriggerSpec NewTrigger(string id, TriggerMoment moment, SpellSlot slot, float energy, float inheritedPower, int max, TargetContext target)
        {
            return new TriggerSpec
            {
                sourceId = id, moment = moment, linkedSlot = slot, targetContext = target,
                energyCost = energy, inheritedPower = inheritedPower, maxActivationsPerEvent = max, internalCooldown = 0.05f
            };
        }
    }
}
