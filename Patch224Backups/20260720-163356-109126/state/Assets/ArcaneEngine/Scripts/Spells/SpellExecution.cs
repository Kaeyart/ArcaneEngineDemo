using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class SpellCastBudget
    {
        private static int _nextEventId;
        private readonly Dictionary<string, int> _activations = new Dictionary<string, int>();
        private readonly Dictionary<string, float> _lastActivation = new Dictionary<string, float>();
        private int _entities;
        private int _totalActivations;

        public readonly int eventId;
        public float remainingEnergy;
        public float storedExcessDamage;

        // ARCANE_PATCH_220_REACTION_CLARITY
        private float _chainEnergy22 = 1.35f;
        private int _chainNetworks22;

        public float ReserveChainNetwork(int structuralCopies)
        {
            structuralCopies = Mathf.Max(1, structuralCopies);
            float desired = Mathf.Clamp(1f / Mathf.Sqrt(structuralCopies), 0.28f, 1f);
            float remainingNetworks = Mathf.Max(1f, structuralCopies - _chainNetworks22);
            float fairShare = _chainEnergy22 / remainingNetworks;
            float granted = Mathf.Clamp(Mathf.Min(desired, fairShare), 0f, 1f);
            _chainEnergy22 = Mathf.Max(0f, _chainEnergy22 - granted);
            _chainNetworks22++;
            return granted;
        }

        public SpellCastBudget(float energy)
        {
            eventId = ++_nextEventId;
            remainingEnergy = energy;
        }

        public bool TrySpend(TriggerSpec trigger)
        {
            int count;
            float last;
            _activations.TryGetValue(trigger.sourceId, out count);
            _lastActivation.TryGetValue(trigger.sourceId, out last);
            if (_totalActivations >= BalanceTuning.MaximumTriggerActivations || count >= trigger.maxActivationsPerEvent || remainingEnergy < trigger.energyCost) return false;
            if (trigger.internalCooldown > 0f && Time.time - last < trigger.internalCooldown) return false;
            remainingEnergy -= trigger.energyCost;
            _activations[trigger.sourceId] = count + 1;
            _totalActivations++;
            _lastActivation[trigger.sourceId] = Time.time;
            return true;
        }

        public bool TryReserveEntity(int amount = 1)
        {
            int limit = Mathf.RoundToInt(V1PerformanceBudget.MaximumSpellEntities * Mathf.Clamp(ProfileManager.Current.accessibility.effectDensity, 0.35f, 1f));
            if (_entities + amount > Mathf.Max(36, limit)) return false;
            _entities += amount;
            return true;
        }
    }

    public struct CastRequest
    {
        public Vector3 origin;
        public Vector3 castOrigin;
        public Vector3 direction;
        public Vector3 targetPosition;
        public float powerScale;
        public int generation;
        public SpellCastBudget budget;
        public bool manualCast;
    }

    public static class SpellExecutor
    {
        private const int MaxGeneration = 6;

        public static void Cast(CompiledSpell spell, CastRequest request)
        {
            if (spell == null || GameWorld.Instance == null || !GameWorld.Instance.RunActive) return;
            if (request.generation > MaxGeneration)
            {
                GameWorld.Instance.Log("Trigger chain ended at generation limit.");
                return;
            }
            if (request.budget == null) request.budget = new SpellCastBudget(GameWorld.Instance.Stats.triggerEnergy);
            if (request.castOrigin.sqrMagnitude < 0.0001f) request.castOrigin = request.origin;
            if (GameWorld.Instance.Equipment.HasMutation(UniqueMutation.TriggeredDominion)) request.powerScale *= request.manualCast ? 0.82f : 1.3f;
            if (request.direction.sqrMagnitude < 0.01f) request.direction = Vector3.forward;
            request.direction.y = 0f;
            request.direction.Normalize();
            ResolveTargetContext(spell.targetContext, ref request);
            V12CombatEventBus.Publish(request.manualCast ? V12CombatEventType.ManualCast : V12CombatEventType.TriggeredCast,
                spell.coreId, string.Empty, request.powerScale, request.origin, spell.displayName + " · " + spell.delivery, request.generation);
            RuntimeAudio.PlaySpell(spell.element, request.origin, false, request.generation);
            SpellVisualEvents.Cast(spell, request);

            if (spell.instability > 0f && Random.value < Mathf.Clamp01(spell.instability / 450f))
            {
                float backlash = 5f + spell.instability * 0.12f;
                GameWorld.Instance.Player.TakeDamage(backlash, "Spell Overload backlash");
                GameWorld.Instance.Log("Overload backlash: " + Mathf.RoundToInt(backlash) + " damage.");
            }

            if (request.manualCast && spell.barrierOnManualCast > 0f)
            {
                GameWorld.Instance.Player.AddWard(spell.barrierOnManualCast * request.powerScale, 2.5f);
                PlayerBarrierVisual.Ensure(GameWorld.Instance.Player, spell.accentColor, 2.5f);
            }

            switch (spell.delivery)
            {
                case SpellDelivery.Projectile: SpawnProjectiles(spell, request, true); break;
                case SpellDelivery.Nova: NovaEffect.Create(spell, request); break;
                case SpellDelivery.Hitscan: PerformHitscan(spell, request, false); break;
                case SpellDelivery.Beam: PerformHitscan(spell, request, true); break;
                case SpellDelivery.Meteor: DelayedSpellEffect.Create(spell, request); break;
                case SpellDelivery.Summon: SpawnFamiliars(spell, request); break;
                case SpellDelivery.Movement: CastMovement(spell, request); break;
                case SpellDelivery.Zone: PersistentSpellZone.Create(spell, request, request.targetPosition, Mathf.Max(2.5f, spell.radius)); break;
                case SpellDelivery.Melee: NovaEffect.Create(spell, request); break;
                case SpellDelivery.Defensive: CastDefensive(spell, request); break;
            }
            InvokeTriggers(spell, TriggerMoment.OnCast, request, request.origin);
        }

        public static void Impact(CompiledSpell spell, CastRequest request, EnemyController enemy, Vector3 position)
        {
            if (enemy == null || enemy.IsDead) return;
            RuntimeAudio.PlaySpell(spell.element, position, true, request.generation);
            float damage = spell.damage * request.powerScale * RangePowerMultiplier(spell, request, position);
            if (enemy.IsBoss) damage *= 1f + GameWorld.Instance.Stats.bossDamage;
            else if (enemy.IsEliteOrBoss) damage *= 1f + GameWorld.Instance.Stats.eliteDamage;
            if (spell.consumesStatus && enemy.HasStatus)
            {
                damage += enemy.ConsumeStatus() + spell.damage * request.powerScale * 0.45f;
                SpellVisualEvents.StatusConsume(spell, enemy.transform.position);
                InvokeTriggers(spell, TriggerMoment.OnStatusConsumed, request, position);
            }
            if (spell.storesExcessDamage && request.budget != null && request.budget.storedExcessDamage > 0f)
            {
                damage += request.budget.storedExcessDamage;
                request.budget.storedExcessDamage = 0f;
            }
            bool critical = Random.value < GameWorld.Instance.Stats.critChance;
            if (critical) damage *= GameWorld.Instance.Stats.critDamage;
            float healthBefore = enemy.Health;
            ReactionContext22 impactContext22 = SpellReactionBridge22.ContextFor(
            request,
            request.generation <= 0
                ? ReactionSourceKind22.DirectCast
                : ReactionSourceKind22.RuneDerived);
        bool killed;
        using (ElementalReactionRuntime.UseContext(impactContext22))
            killed = enemy.TakeDamage(damage, spell.primaryColor, spell.element, critical);
            SpellVisualEvents.Impact(spell, request, position, critical);
            V12CombatEventBus.Publish(critical ? V12CombatEventType.CriticalHit : V12CombatEventType.Hit,
                spell.coreId, enemy.GetEntityId().ToString(), damage, position, spell.element + " · " + spell.delivery, request.generation);
            if (GameWorld.Instance.Stats.spellLeech > 0f) GameWorld.Instance.Player.Heal(damage * GameWorld.Instance.Stats.spellLeech);
            enemy.ApplyImpact(request.direction, damage);
            if (RunStatistics.Instance != null) RunStatistics.Instance.RecordSpellDamage(spell.displayName, damage, critical);
            GameFeelSystem.ElementImpact(position, spell.element, spell.primaryColor, critical, damage);
            bool statusApplied = ApplyStatuses(spell, request, enemy);
            if (statusApplied)
                V12CombatEventBus.Publish(V12CombatEventType.StatusApplied, spell.coreId, enemy.GetEntityId().ToString(),
                    spell.poisonDamage + spell.burnDamage + spell.shockMagnitude + spell.chillMagnitude + spell.freezeSeconds,
                    position, spell.element.ToString(), request.generation);
            if (critical) InvokeTriggers(spell, TriggerMoment.OnCriticalHit, request, position);
            if (statusApplied) InvokeTriggers(spell, TriggerMoment.OnStatusApplied, request, position);
            if (spell.freezeSeconds > 0f) InvokeTriggers(spell, TriggerMoment.OnFreeze, request, position);

            if (spell.relicSignature == RelicSignature.PhoenixSeed && !killed) enemy.EmbedPhoenix(spell, request);
            if (spell.explosionRadius > 0.1f)
            {
                if (spell.detonationDelay > 0f) DelayedSpellEffect.CreateImpact(spell, request, position, spell.detonationDelay);
                else Explode(spell, request, position, enemy);
            }
            if (spell.zoneDuration > 0f) PersistentSpellZone.Create(spell, request, position, Mathf.Max(1.8f, spell.explosionRadius));
            if (spell.chainTargets > 0) Chain(spell, request, enemy);
            if (spell.spreadsStatus) SpreadStatus(spell, request, enemy);
            if (spell.summonCount > 0 && spell.delivery != SpellDelivery.Summon) SpawnFamiliars(spell, request, position);
            if (spell.splitOnHit && spell.splitCount > 0) SpawnSplitChildren(spell, request, position, request.direction);
            InvokeTriggers(spell, TriggerMoment.OnHit, request, position);
            if (spell.delivery == SpellDelivery.Projectile)
                InvokeTriggers(spell, TriggerMoment.OnProjectileImpact, request, position);
            if (killed)
            {
                if (spell.storesExcessDamage && request.budget != null) request.budget.storedExcessDamage = Mathf.Clamp(damage - healthBefore, 0f, spell.damage);
                NotifyKill(spell, request, position);
            }
            Camera camera = Camera.main;
            if (camera != null)
            {
                IsometricCamera rig = camera.GetComponent<IsometricCamera>();
                if (rig != null) rig.Shake(Mathf.Clamp(damage / 180f, 0.03f, 0.22f), position, critical ? 3 : 1, critical ? 0.18f : 0.1f);
            }
        }

        public static void NotifyKill(CompiledSpell spell, CastRequest request, Vector3 position)
        {
            V12CombatEventBus.Publish(V12CombatEventType.EnemyKilled, spell == null ? "spell" : spell.coreId,
                string.Empty, 1f, position, spell == null ? string.Empty : spell.displayName, request.generation);
            if (GameWorld.Instance.Stats.cooldownOnKillReduction > 0f)
                GameWorld.Instance.Player.ReduceCooldownsAfterKill(GameWorld.Instance.Stats.cooldownOnKillReduction);
            InvokeTriggers(spell, TriggerMoment.OnKill, request, position);
        }

        public static void NotifyPlayerEvent(TriggerMoment moment, Vector3 position, Vector3 direction)
        {
            if (GameWorld.Instance == null || !GameWorld.Instance.RunActive) return;
            SpellCastBudget budget = new SpellCastBudget(GameWorld.Instance.Stats.triggerEnergy);
            for (int i = 0; i < 3; i++)
            {
                CompiledSpell source = GameWorld.Instance.GetSpell((SpellSlot)i);
                if (source == null) continue;
                CastRequest request = new CastRequest { origin = position, targetPosition = position, direction = direction,
                    powerScale = 1f, generation = 0, budget = budget, manualCast = false };
                InvokeTriggers(source, moment, request, position);
            }
        }

        public static void Expire(CompiledSpell spell, CastRequest request, Vector3 position)
        {
            SpellVisualEvents.Expire(spell, request, position);
            if (spell.explosionRadius > 0.1f)
            {
                if (spell.detonationDelay > 0f) DelayedSpellEffect.CreateImpact(spell, request, position, spell.detonationDelay);
                else Explode(spell, request, position, null);
            }
            if (spell.zoneDuration > 0f) PersistentSpellZone.Create(spell, request, position, Mathf.Max(1.8f, spell.explosionRadius));
            InvokeTriggers(spell, TriggerMoment.OnExpire, request, position);
            if (spell.delivery == SpellDelivery.Projectile)
                InvokeTriggers(spell, TriggerMoment.OnProjectileExpire, request, position);
        }

        public static void PhoenixErupt(CompiledSpell spell, CastRequest request, Vector3 position)
        {
            if (spell == null) return;
            if (spell.relicSignature == RelicSignature.PhoenixSeed) GameWorld.Instance.Log("Phoenix Seed hatches from its fallen host.");
            SpawnSplitChildren(spell, request, position, request.direction, Mathf.Max(3, spell.splitCount));
        }

        public static void DelayedAreaImpact(CompiledSpell spell, CastRequest request, Vector3 position)
        {
            Explode(spell, request, position, null);
            InvokeTriggers(spell, TriggerMoment.OnHit, request, position);
        }

        public static void ZoneExpired(CompiledSpell spell, CastRequest request, Vector3 position)
        {
            InvokeTriggers(spell, TriggerMoment.OnExpire, request, position);
        }

        private static void SpawnProjectiles(CompiledSpell spell, CastRequest request, bool canSplit)
        {
            int count = Mathf.Clamp(spell.projectileCount, 1, 24);
            for (int i = 0; i < count; i++)
            {
                if (!request.budget.TryReserveEntity()) break;
                CastRequest projectileRequest = request;
                Vector3 direction;
                if (spell.projectilePattern == ProjectilePattern.Ring)
                {
                    float angle = i / (float)count * 360f;
                    direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
                }
                else
                {
                    float spread = spell.projectilePattern == ProjectilePattern.Cone ? Mathf.Max(21f, spell.spreadDegrees) : spell.spreadDegrees;
                    float offset = count == 1 ? 0f : Mathf.Lerp(-spread, spread, i / (float)(count - 1));
                    direction = Quaternion.Euler(0f, offset, 0f) * request.direction;
                    if (spell.projectilePattern == ProjectilePattern.Converge)
                    {
                        Vector3 right = Vector3.Cross(Vector3.up, request.direction).normalized;
                        float lateral = count == 1 ? 0f : Mathf.Lerp(-1.6f, 1.6f, i / (float)(count - 1));
                        projectileRequest.origin += right * lateral;
                        Vector3 convergence = request.targetPosition - projectileRequest.origin;
                        convergence.y = 0f;
                        if (convergence.sqrMagnitude > 0.01f) direction = convergence.normalized;
                    }
                }
                SpellProjectile.Create(spell, projectileRequest, direction, canSplit);
            }
        }

        private static void SpawnSplitChildren(CompiledSpell spell, CastRequest parent, Vector3 position, Vector3 forward, int overrideCount = -1)
        {
            if (parent.generation >= MaxGeneration) return;
            int count = Mathf.Clamp(overrideCount > 0 ? overrideCount : spell.splitCount, 2, 8);
            for (int i = 0; i < count; i++)
            {
                if (!parent.budget.TryReserveEntity()) break;
                float angle = count == 1 ? 0f : Mathf.Lerp(-50f, 50f, i / (float)(count - 1));
                CastRequest child = parent;
                child.origin = position + Vector3.up * 0.1f;
                child.direction = Quaternion.Euler(0f, angle, 0f) * forward;
                child.powerScale *= 0.58f;
                child.generation++;
                child.manualCast = false;
                SpellProjectile.Create(spell, child, child.direction, false);
            }
        }

        private static void PerformHitscan(CompiledSpell spell, CastRequest request, bool beam, bool scheduleRepeats = true)
        {
            Vector3 end = request.origin + request.direction * Mathf.Max(6f, spell.radius);
            List<EnemyController> targets = GameWorld.Instance.Enemies.Where(e => e != null && !e.IsDead)
                .Select(e => { Vector3 delta = e.transform.position - request.origin; delta.y = 0f; return new { enemy = e,
                    along = Vector3.Dot(delta, request.direction), side = Mathf.Abs(Vector3.Cross(request.direction, delta).y) }; })
                .Where(v => v.along > 0f && v.along <= Mathf.Max(6f, spell.radius) && v.side < (beam ? 0.7f : 1.0f) + v.enemy.HitRadius)
                .OrderBy(v => v.along).Select(v => v.enemy).Take(Mathf.Max(1, spell.pierce + 1)).ToList();
            if (targets.Count > 0) end = targets.Last().transform.position;
            ProceduralVisualRuntime.Beam(beam ? "Spell Beam" : "Hitscan", request.origin, end, spell.primaryColor,
                beam ? Mathf.Clamp(spell.size * 0.22f, 0.12f, 0.65f) : 0.09f, beam ? 0.28f : 0.12f, spell.element == SpellElement.Lightning);
            foreach (EnemyController target in targets) Impact(spell, request, target, target.transform.position);
            if (targets.Count == 0 && spell.explosionRadius > 0f) Explode(spell, request, request.targetPosition, null);
            if (beam && scheduleRepeats && spell.repeatCount > 1) DelayedSpellEffect.CreateBeamRepeats(spell, request);
        }

        public static void RepeatBeam(CompiledSpell spell, CastRequest request)
        {
            if (spell == null || GameWorld.Instance == null || !GameWorld.Instance.RunActive) return;
            PerformHitscan(spell, request, true, false);
        }

        private static void CastMovement(CompiledSpell spell, CastRequest request)
        {
            Vector3 from = GameWorld.Instance.Player.transform.position;
            GameWorld.Instance.Player.TeleportToward(request.targetPosition, Mathf.Max(4f, spell.radius * 2f));
            Vector3 to = GameWorld.Instance.Player.transform.position;
            SpellDeliveryVisuals.MovementArrived(spell, request, from, to);
            ProceduralVisualRuntime.Beam("Movement Path", from, to, spell.primaryColor, 0.12f, 0.25f, spell.element == SpellElement.Lightning || spell.element == SpellElement.Void);
            if (spell.relicSignature == RelicSignature.RiftStep || spell.zoneDuration > 0f)
                PersistentSpellZone.CreateLine(spell, request, from, to);
            NovaEffect.Create(spell, new CastRequest { origin = to, targetPosition = to, direction = request.direction,
                powerScale = request.powerScale, generation = request.generation, budget = request.budget, manualCast = request.manualCast });
        }

        private static void CastDefensive(CompiledSpell spell, CastRequest request)
        {
            float ward = spell.damage * request.powerScale * (spell.relicSignature == RelicSignature.PerfectAegis ? 2.4f : 1.5f);
            GameWorld.Instance.Player.AddWard(ward, Mathf.Max(1.2f, spell.lifetime));
            PlayerBarrierVisual.Ensure(GameWorld.Instance.Player, spell.primaryColor, Mathf.Max(1.2f, spell.lifetime));
            LineRenderer ring = RuntimeVisuals.Ring(spell.displayName, GameWorld.Instance.Player.transform.position, spell.primaryColor, spell.radius, 0.18f,
                GameWorld.Instance.Player.transform);
            Object.Destroy(ring.gameObject, Mathf.Max(0.5f, spell.lifetime));
            if (spell.relicSignature == RelicSignature.RetributionWard || spell.explosionRadius > 0f) NovaEffect.Create(spell, request);
        }

        private static void SpawnFamiliars(CompiledSpell spell, CastRequest request, Vector3? position = null)
        {
            int count = Mathf.Clamp(Mathf.Max(1, spell.summonCount), 1, 8);
            for (int i = 0; i < count; i++)
            {
                if (!request.budget.TryReserveEntity(2)) break;
                Vector3 origin = position ?? request.origin;
                float angle = i / (float)count * Mathf.PI * 2f;
                SpellFamiliar.Create(spell, request, origin + new Vector3(Mathf.Cos(angle), 0.5f, Mathf.Sin(angle)) * 1.3f, i);
            }
        }

        private static void Explode(CompiledSpell spell, CastRequest request, Vector3 position, EnemyController directTarget)
        {
            float radius = Mathf.Max(0.5f, spell.explosionRadius);
            LineRenderer ring = RuntimeVisuals.Ring("Spell Detonation", position + Vector3.up * 0.08f, spell.primaryColor, radius, 0.16f);
            Object.Destroy(ring.gameObject, 0.22f);
            if (ProfileManager.Current.accessibility.effectDensity > 0.55f)
            {
                GameObject flash = RuntimeVisuals.Primitive("Detonation Flash", PrimitiveType.Sphere, position + Vector3.up * 0.4f,
                    Vector3.one * radius * 0.55f, new Color(spell.primaryColor.r, spell.primaryColor.g, spell.primaryColor.b, 0.55f));
                RuntimeVisuals.RemoveCollider(flash);
                Object.Destroy(flash, 0.14f);
            }
            foreach (EnemyController target in GameWorld.Instance.Enemies.ToArray())
            {
                if (target == null || target.IsDead || target == directTarget || CombatMath.PlanarDistanceSquared(target.transform.position, position) > radius * radius) continue;
                ReactionContext22 explosionContext22 = SpellReactionBridge22.ContextFor(
                request,
                request.generation <= 0
                    ? ReactionSourceKind22.DirectCast
                    : ReactionSourceKind22.RuneDerived)
                .Derive(ReactionSourceKind22.Echo)
                .WithoutFieldCreation();
            bool wasAlive22 = !target.IsDead;
            ElementalReactionRuntime.DealReactionDamage(
                target,
                spell.damage * request.powerScale * 0.55f *
                    RangePowerMultiplier(spell, request, target.transform.position),
                ElementalReactionCodex.FromSpellElement(spell.element),
                spell.primaryColor,
                false,
                explosionContext22);
            ElementalReactionRuntime.ApplyBuildup(
                target,
                ElementalReactionCodex.FromSpellElement(spell.element),
                0.35f,
                4f,
                explosionContext22);
            bool killed = wasAlive22 && target.IsDead;
                if (killed) NotifyKill(spell, request, target.transform.position);
            }
        }

        private static void Chain(CompiledSpell spell, CastRequest request, EnemyController source)
    {
        if (spell == null || source == null || request.budget == null)
            return;

        float networkScale22 = SpellReactionBridge22.ChainNetworkScale(request.budget, spell);
        if (networkScale22 <= 0.05f)
            return;

        HashSet<EntityId> excluded = new HashSet<EntityId> { source.GetEntityId() };
        Vector3 from = source.transform.position;
        int targetLimit = SpellReactionBridge22.ChainTargetLimit(spell);
        ReactionContext22 chainContext22 = SpellReactionBridge22.ChainContext(request);
        ReactionElement reactionElement22 = ElementalReactionCodex.FromSpellElement(spell.element);

        for (int i = 0; i < targetLimit; i++)
        {
            EnemyController next = GameWorld.Instance.NearestEnemy(from, 6.5f, excluded);
            if (next == null)
                break;

            excluded.Add(next.GetEntityId());
            if (!ReactionLineageRegistry22.TryMarkTarget(chainContext22, next, 0.35f))
                continue;

            float visualScale22 = ReactionBalance22.ChainVisualIntensity(i);
            ProceduralVisualRuntime.Beam(
                "Chain Event",
                from,
                next.transform.position,
                spell.accentColor,
                0.095f * visualScale22,
                0.12f,
                true);

            ReactionContext22 jumpContext22 = chainContext22.WithCoefficients(
                ReactionBalance22.ChainDamageCoefficient(i) * networkScale22,
                networkScale22,
                1f);

            bool wasAlive22 = !next.IsDead;
            ElementalReactionRuntime.DealReactionDamage(
                next,
                spell.damage * request.powerScale *
                    RangePowerMultiplier(spell, request, next.transform.position),
                reactionElement22,
                spell.accentColor,
                false,
                jumpContext22);

            ElementalReactionRuntime.ApplyBuildup(
                next,
                reactionElement22,
                ReactionBalance22.ChainBuildupAmount(i) * networkScale22,
                4f,
                jumpContext22);

            ReactionPresentation22.EmitChainContact(
                from,
                next.transform.position,
                reactionElement22,
                i,
                jumpContext22);

            if (wasAlive22 && next.IsDead)
                NotifyKill(spell, request, next.transform.position);

            from = next.transform.position;
        }
    }

    private static bool ApplyStatuses(CompiledSpell spell, CastRequest request, EnemyController enemy)
        {
            bool applied = false;
            if (spell.poisonDamage > 0f && spell.poisonDuration > 0f) { enemy.ApplyPoison(spell.poisonDamage, spell.poisonDuration, spell, request); applied = true; }
            if (spell.burnDamage > 0f && spell.burnDuration > 0f) { enemy.ApplyBurn(spell.burnDamage, spell.burnDuration, spell, request); applied = true; }
            if (spell.shockMagnitude > 0f && spell.shockDuration > 0f) { enemy.ApplyShock(spell.shockMagnitude, spell.shockDuration); applied = true; }
            if (spell.chillMagnitude > 0f && spell.chillDuration > 0f) { enemy.ApplyChill(spell.chillMagnitude, spell.chillDuration); applied = true; }
            if (spell.bleedDamage > 0f && spell.bleedDuration > 0f) { enemy.ApplyBleed(spell.bleedDamage, spell.bleedDuration, spell, request); applied = true; }
            if (spell.curseMagnitude > 0f && spell.curseDuration > 0f) { enemy.ApplyCurse(spell.curseMagnitude, spell.curseDuration); applied = true; }
            if (spell.weakenMagnitude > 0f && spell.weakenDuration > 0f) { enemy.ApplyWeaken(spell.weakenMagnitude, spell.weakenDuration); applied = true; }
            if (spell.vulnerabilityMagnitude > 0f && spell.vulnerabilityDuration > 0f) { enemy.ApplyVulnerability(spell.vulnerabilityMagnitude, spell.vulnerabilityDuration); applied = true; }
            if (spell.freezeSeconds > 0f) { enemy.ApplyFreeze(spell.freezeSeconds); applied = true; }
            return applied;
        }

        private static float RangePowerMultiplier(CompiledSpell spell, CastRequest request, Vector3 position)
        {
            float travelDistance = Vector3.Distance(request.castOrigin, position);
            if (travelDistance <= 4f) return spell.closeRangeMultiplier;
            if (travelDistance >= 10f) return spell.longRangeMultiplier;
            return Mathf.Lerp(spell.closeRangeMultiplier, spell.longRangeMultiplier, (travelDistance - 4f) / 6f);
        }

        private static void SpreadStatus(CompiledSpell spell, CastRequest request, EnemyController source)
    {
        ReactionElement element22 = ElementalReactionCodex.FromSpellElement(spell.element);
        if (element22 == ReactionElement.None)
            return;

        ReactionContext22 spreadContext22 = SpellReactionBridge22.ContextFor(
            request,
            request.generation <= 0
                ? ReactionSourceKind22.DirectCast
                : ReactionSourceKind22.RuneDerived)
            .Derive(ReactionSourceKind22.Echo)
            .WithoutFieldCreation();

        EnemyController[] targets22 = GameWorld.Instance.Enemies
            .Where(e => e != null && e != source && !e.IsDead &&
                CombatMath.PlanarDistanceSquared(e.transform.position, source.transform.position) < 16f)
            .Take(3)
            .ToArray();

        float totalBudget22 = 1.05f;
        float perTarget22 = targets22.Length <= 0 ? 0f : totalBudget22 / targets22.Length;
        for (int i = 0; i < targets22.Length; i++)
        {
            EnemyController target22 = targets22[i];
            if (!ReactionLineageRegistry22.TryMarkTarget(spreadContext22, target22, 0.35f))
                continue;

            ElementalReactionRuntime.ApplyBuildup(
                target22,
                element22,
                Mathf.Min(0.45f, perTarget22),
                4f,
                spreadContext22);
            SpellVisualEvents.StatusSpread(spell, source.transform.position, target22.transform.position);
        }
    }

    private static void InvokeTriggers(CompiledSpell spell, TriggerMoment moment, CastRequest parent, Vector3 position)
        {
            if (spell == null || parent.generation >= MaxGeneration || parent.budget == null) return;
            if (GameWorld.Instance != null && GameWorld.Instance.SpellLinks != null)
                GameWorld.Instance.SpellLinks.Activate(spell.slot, moment, parent, position);
            foreach (TriggerSpec trigger in spell.triggers.Where(t => t.moment == moment))
            {
                if (!parent.budget.TrySpend(trigger)) continue;
                CompiledSpell linked = GameWorld.Instance.GetSpell(trigger.linkedSlot);
                if (linked == null) continue;
                SpellVisualEvents.LinkActivation(spell, linked, position, moment);
                CastRequest child = parent;
                child.origin = position + Vector3.up * 0.08f;
                child.castOrigin = child.origin;
                child.powerScale = parent.powerScale * trigger.inheritedPower;
                child.generation = parent.generation + 1;
                child.manualCast = false;
                ResolveTargetContext(trigger.targetContext, ref child);
                Cast(linked, child);
            }
        }

        private static void ResolveTargetContext(TargetContext context, ref CastRequest request)
        {
            EnemyController nearest;
            switch (context)
            {
                case TargetContext.Player:
                    request.targetPosition = GameWorld.Instance.Player.transform.position; request.origin = request.targetPosition; break;
                case TargetContext.NearestEnemy:
                    nearest = GameWorld.Instance.NearestEnemy(request.origin);
                    if (nearest != null) { request.targetPosition = nearest.transform.position; request.direction = request.targetPosition - request.origin; } break;
                case TargetContext.Impact:
                    request.targetPosition = request.origin; break;
                case TargetContext.BetweenTargets:
                    nearest = GameWorld.Instance.NearestEnemy(request.origin);
                    if (nearest != null) request.targetPosition = Vector3.Lerp(request.origin, nearest.transform.position, 0.5f); break;
            }
            request.direction.y = 0f;
            if (request.direction.sqrMagnitude < 0.01f) request.direction = Vector3.forward;
            request.direction.Normalize();
        }
    }

    public sealed class SpellProjectile : MonoBehaviour
    {
        private CompiledSpell _spell;
        private CastRequest _request;
        private Vector3 _direction;
        private Vector3 _centerPosition;
        private Vector3 _collisionPosition;
        private Vector3 _side;
        private float _age;
        private float _distance;
        private float _currentSpeed;
        private float _trailTick;
        private int _remainingContacts;
        private int _remainingBounces;
        private bool _ended;
        private bool _returning;
        private bool _orbitReleased;
        private bool _canSplit;
        private bool _splitDone;
        private readonly HashSet<EntityId> _hitEnemies = new HashSet<EntityId>();
        private bool _visualRegistered;

        public static int ActiveVisualCount { get { return VisualRuntimeRegistry.Count(VisualRuntimeKind.Projectile); } }

        public static int InterceptNear(Vector3 position, float radius, Vector3 reflectTarget, float reflectedDamage)
        {
            int intercepted = 0;
            float radiusSquared = radius * radius;
            foreach (SpellProjectile projectile in FindObjectsByType<SpellProjectile>())
            {
                if (projectile == null || projectile._ended || CombatMath.PlanarDistanceSquared(projectile.transform.position, position) > radiusSquared) continue;
                Vector3 direction = reflectTarget - position;
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.01f)
                    EnemyBolt.Create(position + Vector3.up * 0.55f, direction.normalized, Mathf.Max(1f, reflectedDamage), new Color(0.85f, 0.85f, 1f));
                projectile.End(false);
                intercepted++;
                if (intercepted >= 3) break;
            }
            return intercepted;
        }

        private void OnEnable()
        {
            if (_visualRegistered) return;
            _visualRegistered = true;
            VisualRuntimeRegistry.Register(VisualRuntimeKind.Projectile);
        }

        private void OnDisable()
        {
            if (!_visualRegistered) return;
            _visualRegistered = false;
            VisualRuntimeRegistry.Unregister(VisualRuntimeKind.Projectile);
        }

        public static SpellProjectile Create(CompiledSpell spell, CastRequest request, Vector3 direction, bool canSplit)
        {
            float scale = Mathf.Clamp(spell.size, 0.2f, 4f);
            Vector3 combatOrigin = request.origin;
            combatOrigin.y = 0.82f;
            GameObject go = RuntimeVisuals.Primitive(spell.displayName + " Projectile", PrimitiveType.Sphere, combatOrigin + Vector3.up * 0.45f, Vector3.one * scale, spell.primaryColor);
            RuntimeVisuals.RemoveCollider(go);
            RuntimeEntityToken token = go.AddComponent<RuntimeEntityToken>();
            if (!token.Acquire(request.manualCast ? RuntimeEntityKind.PlayerProjectile : RuntimeEntityKind.TriggeredCast)) return null;
            if (ProfileManager.Current.accessibility.effectDensity > 0.5f)
            {
                GameObject core = RuntimeVisuals.Primitive("Accent Core", PrimitiveType.Sphere, go.transform.position, Vector3.one * scale * 0.45f, spell.accentColor, go.transform);
                core.transform.localPosition = Vector3.zero; RuntimeVisuals.RemoveCollider(core);
            }
            SpellVisualAttachment.Attach(go, spell, request);
            SpellProjectile projectile = go.AddComponent<SpellProjectile>();
            projectile._spell = spell; projectile._request = request; projectile._direction = direction.normalized;
            projectile._centerPosition = combatOrigin; projectile._collisionPosition = combatOrigin; projectile._side = Vector3.Cross(Vector3.up, projectile._direction).normalized;
            projectile._remainingContacts = Mathf.Max(1, spell.pierce + 1); projectile._remainingBounces = spell.bounce;
            projectile._currentSpeed = spell.speed; projectile._canSplit = canSplit;
            return projectile;
        }

        private void Update()
        {
            if (_ended || GameWorld.Instance == null || !GameWorld.Instance.RunActive) return;
            float deltaTime = Time.deltaTime;
            _age += deltaTime;
            if (_age >= _spell.lifetime) { End(true); return; }

            Vector3 previousCollision = _collisionPosition;
            if (_spell.orbitCaster && !_orbitReleased && GameWorld.Instance.Player != null && _age < 0.8f)
            {
                float angle = _age * Mathf.Max(1f, _spell.speed) + GetEntityId().GetHashCode() * 0.01f;
                _centerPosition = GameWorld.Instance.Player.transform.position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * Mathf.Max(2f, _spell.radius * 0.65f);
                _centerPosition.y = 0.82f;
            }
            else
            {
                if (_spell.orbitCaster && !_orbitReleased)
                {
                    _orbitReleased = true;
                    Vector3 releaseDirection = _request.targetPosition - _centerPosition;
                    releaseDirection.y = 0f;
                    if (releaseDirection.sqrMagnitude > 0.01f) _direction = releaseDirection.normalized;
                }
                if (_spell.returnsToCaster && !_returning && _age >= _spell.lifetime * 0.5f) { _returning = true; _hitEnemies.Clear(); }
                if (_returning && _age - deltaTime < _spell.lifetime * 0.5f) SpellVisualEvents.DirectionChange(_spell, _collisionPosition, true);
                if (_returning && GameWorld.Instance.Player != null)
                {
                    Vector3 desired = GameWorld.Instance.Player.transform.position - _centerPosition; desired.y = 0f;
                    if (desired.sqrMagnitude > 0.01f) _direction = Vector3.RotateTowards(_direction, desired.normalized, 8f * deltaTime, 0f).normalized;
                }
                else if (_spell.homingStrength > 0f)
                {
                    EnemyController target = GameWorld.Instance.NearestEnemy(_centerPosition, 10f, _hitEnemies);
                    if (target != null)
                    {
                        Vector3 desired = target.transform.position - _centerPosition; desired.y = 0f;
                        if (desired.sqrMagnitude > 0.01f) _direction = Vector3.RotateTowards(_direction, desired.normalized, _spell.homingStrength * deltaTime, 0f).normalized;
                    }
                }
                _currentSpeed = Mathf.Max(1.5f, _currentSpeed + _spell.acceleration * deltaTime);
                Vector3 step = _direction * _currentSpeed * deltaTime;
                _centerPosition += step; _distance += step.magnitude;
            }

            _side = Vector3.Cross(Vector3.up, _direction).normalized;
            float wave = Mathf.Sin(_age * 7.5f) * _spell.arcAmount;
            _collisionPosition = _centerPosition + _side * wave;
            _collisionPosition.y = 0.82f;
            Vector3 visualPosition = _collisionPosition;
            visualPosition.y = 1.27f + Mathf.Abs(Mathf.Sin(_age * 4f)) * _spell.arcAmount * 0.18f;
            transform.position = visualPosition;
            transform.Rotate(90f * deltaTime, 150f * deltaTime, 60f * deltaTime);

            if (_spell.pullsEnemies)
            {
                float pullRadius = _spell.isVoidMaw ? 5.5f : 4.5f;
                foreach (EnemyController enemy in GameWorld.Instance.Enemies.ToArray())
                    if (enemy != null && CombatMath.PlanarDistanceSquared(enemy.transform.position, _collisionPosition) < pullRadius * pullRadius)
                        enemy.PullToward(_collisionPosition, _spell.isVoidMaw ? 4.2f : 2.8f);
            }

            if (_canSplit && !_splitDone && _spell.splitDistance > 0f && _distance >= _spell.splitDistance)
            {
                _splitDone = true;
                SpellExecutor.PhoenixErupt(_spell, _request, _collisionPosition);
            }
            if (_spell.trailDuration > 0f)
            {
                _trailTick -= deltaTime;
                if (_trailTick <= 0f) { _trailTick = 0.35f; PersistentSpellZone.Create(_spell, _request, _collisionPosition, Mathf.Max(0.75f, _spell.size)); }
            }

            float hitRadius = Mathf.Clamp(_spell.size * 0.5f, 0.18f, 2.4f);
            IReadOnlyList<BreakableProp> props = BreakableProp.Active;
            for (int i = 0; i < props.Count; i++)
            {
                BreakableProp prop = props[i];
                if (prop == null || CombatMath.PlanarSegmentDistanceSquared(prop.transform.position, previousCollision, _collisionPosition) > (hitRadius + 0.65f) * (hitRadius + 0.65f)) continue;
                prop.Damage(_spell.damage * _request.powerScale);
                End(false);
                return;
            }
            IReadOnlyList<WorldRoomInteractable> interactions = WorldRoomInteractable.Active;
            for (int i = 0; i < interactions.Count; i++)
            {
                ObjectiveNode node = interactions[i] as ObjectiveNode;
                if (node == null || CombatMath.PlanarSegmentDistanceSquared(node.transform.position, previousCollision, _collisionPosition) > (hitRadius + 0.8f) * (hitRadius + 0.8f)) continue;
                if (node.Damage(Mathf.Max(1, Mathf.RoundToInt(_spell.damage * _request.powerScale / 35f))))
                {
                    End(false);
                    return;
                }
            }
            foreach (EnemyController enemy in GameWorld.Instance.Enemies.ToArray())
            {
                if (enemy == null || enemy.IsDead || _hitEnemies.Contains(enemy.GetEntityId())) continue;
                float combinedRadius = hitRadius + enemy.HitRadius;
                if (CombatMath.PlanarSegmentDistanceSquared(enemy.transform.position, previousCollision, _collisionPosition) > combinedRadius * combinedRadius) continue;
                _hitEnemies.Add(enemy.GetEntityId());
                SpellExecutor.Impact(_spell, _request, enemy, _collisionPosition);
                _remainingContacts--;
                if (_remainingBounces > 0)
                {
                    _remainingBounces--;
                    EnemyController next = GameWorld.Instance.NearestEnemy(_collisionPosition, 9f, _hitEnemies);
                    if (next != null)
                    {
                        Vector3 bounceDirection = next.transform.position - _collisionPosition;
                        bounceDirection.y = 0f;
                        if (bounceDirection.sqrMagnitude > 0.01f) _direction = bounceDirection.normalized;
                        SpellVisualEvents.DirectionChange(_spell, _collisionPosition, false);
                        _remainingContacts++;
                        SpellExecutor.NotifyPlayerEvent(TriggerMoment.OnBounce, _collisionPosition, _direction);
                    }
                }
                if (_remainingContacts <= 0) { End(false); return; }
            }
            if (Mathf.Abs(_collisionPosition.x) > 18.5f || Mathf.Abs(_collisionPosition.z) > 18.5f) End(true);
        }

        private void End(bool invokeExpire)
        {
            if (_ended) return;
            _ended = true;
            if (invokeExpire) SpellExecutor.Expire(_spell, _request, _collisionPosition);
            Destroy(gameObject);
        }
    }

    public sealed class NovaEffect : MonoBehaviour
    {
        private CompiledSpell _spell;
        private CastRequest _request;
        private LineRenderer _ring;
        private readonly HashSet<EntityId> _hit = new HashSet<EntityId>();
        private float _pulseAge;
        private float _wait;
        private int _pulseIndex;
        private const float PulseDuration = 0.48f;

        public static NovaEffect Create(CompiledSpell spell, CastRequest request)
        {
            if (!request.budget.TryReserveEntity()) return null;
            GameObject go = new GameObject(spell.displayName + " Nova"); go.transform.position = request.origin;
            NovaEffect nova = go.AddComponent<NovaEffect>(); nova._spell = spell; nova._request = request; nova.BeginPulse(); return nova;
        }

        private void Update()
        {
            if (GameWorld.Instance == null || !GameWorld.Instance.RunActive) { Destroy(gameObject); return; }
            if (_wait > 0f) { _wait -= Time.deltaTime; if (_wait <= 0f) BeginPulse(); return; }
            _pulseAge += Time.deltaTime;
            float normalized = Mathf.Clamp01(_pulseAge / PulseDuration);
            float radius = Mathf.SmoothStep(0.35f, Mathf.Max(2f, _spell.radius), normalized);
            if (_ring != null) _ring.transform.localScale = Vector3.one * radius;
            foreach (EnemyController enemy in GameWorld.Instance.Enemies.ToArray())
            {
                if (enemy == null || enemy.IsDead || _hit.Contains(enemy.GetEntityId()) || CombatMath.PlanarDistanceSquared(enemy.transform.position, transform.position) > radius * radius) continue;
                _hit.Add(enemy.GetEntityId()); SpellExecutor.Impact(_spell, _request, enemy, enemy.transform.position);
            }
            if (_pulseAge < PulseDuration) return;
            if (_ring != null) Destroy(_ring.gameObject);
            _pulseIndex++;
            if (_pulseIndex >= Mathf.Clamp(_spell.repeatCount, 1, 8)) { SpellExecutor.Expire(_spell, _request, transform.position); Destroy(gameObject); }
            else _wait = _spell.repeatDelay;
        }

        private void BeginPulse()
        {
            _pulseAge = 0f; _wait = 0f; _hit.Clear();
            _ring = RuntimeVisuals.Ring("Expanding " + _spell.displayName, transform.position + Vector3.up * 0.08f, _spell.primaryColor, 1f,
                Mathf.Clamp(_spell.size * 0.3f, 0.08f, 0.3f), transform); _ring.transform.localPosition = Vector3.up * 0.08f;
        }
    }

    public sealed class DelayedSpellEffect : MonoBehaviour
    {
        private CompiledSpell _spell;
        private CastRequest _request;
        private Vector3 _position;
        private float _timer;
        private int _remaining;
        private bool _beamRepeats;

        public static DelayedSpellEffect Create(CompiledSpell spell, CastRequest request)
        {
            return CreateInternal(spell, request, request.targetPosition, Mathf.Max(0.45f, spell.detonationDelay > 0f ? spell.detonationDelay : 0.8f), spell.repeatCount, false);
        }

        public static DelayedSpellEffect CreateImpact(CompiledSpell spell, CastRequest request, Vector3 position, float delay)
        {
            return CreateInternal(spell, request, position, delay, 1, false);
        }

        public static DelayedSpellEffect CreateBeamRepeats(CompiledSpell spell, CastRequest request)
        {
            return CreateInternal(spell, request, request.targetPosition, spell.repeatDelay, spell.repeatCount - 1, true);
        }

        private static DelayedSpellEffect CreateInternal(CompiledSpell spell, CastRequest request, Vector3 position, float delay, int repeats, bool beam)
        {
            if (!request.budget.TryReserveEntity()) return null;
            GameObject go = new GameObject("Delayed " + spell.displayName); go.transform.position = position;
            DelayedSpellEffect effect = go.AddComponent<DelayedSpellEffect>(); effect._spell = spell; effect._request = request; effect._position = position;
            effect._timer = delay; effect._remaining = Mathf.Clamp(repeats, 1, 10); effect._beamRepeats = beam;
            if (!beam) RuntimeVisuals.Ring("Impact Telegraph", position, spell.primaryColor, Mathf.Max(1f, spell.radius), 0.12f, go.transform).transform.localPosition = Vector3.zero;
            if (!beam && spell.delivery == SpellDelivery.Meteor) go.AddComponent<MeteorVisualCue>().Initialize(spell, position, delay);
            return effect;
        }

        private void Update()
        {
            if (GameWorld.Instance == null || !GameWorld.Instance.RunActive) { Destroy(gameObject); return; }
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            if (_beamRepeats)
            {
                SpellExecutor.RepeatBeam(_spell, _request);
            }
            else
            {
                Vector3 impact = _position + (_remaining > 1 ? new Vector3(Random.Range(-_spell.radius, _spell.radius), 0f, Random.Range(-_spell.radius, _spell.radius)) : Vector3.zero);
                SpellExecutor.DelayedAreaImpact(_spell, _request, impact);
                LineRenderer ring = RuntimeVisuals.Ring("Meteor Impact", impact, _spell.primaryColor, Mathf.Max(1f, _spell.radius), 0.25f); Destroy(ring.gameObject, 0.25f);
                if (_spell.zoneDuration > 0f) PersistentSpellZone.Create(_spell, _request, impact, _spell.radius);
            }
            _remaining--;
            if (_remaining <= 0) { SpellExecutor.ZoneExpired(_spell, _request, _position); Destroy(gameObject); }
            else _timer = Mathf.Max(0.12f, _spell.repeatDelay);
        }
    }

    public sealed class PersistentSpellZone : MonoBehaviour
{
    private static readonly List<PersistentSpellZone> ActiveZones =
        new List<PersistentSpellZone>();

    public static IReadOnlyList<PersistentSpellZone> Active
    {
        get
        {
            CleanupZones();
            return ActiveZones;
        }
    }

    public bool ReflectsProjectiles { get; private set; }
    public float Radius { get; private set; }

    private CompiledSpell _spell;
    private CastRequest _request;
    private ReactionContext22 _context;
    private ReactionFieldAuthority22 _authority;
    private float _life;
    private float _baseLife;
    private float _power = 1f;
    private float _tick;
    private float _createdAt;
    private bool _expired;

    private void OnEnable()
    {
        if (!ActiveZones.Contains(this))
            ActiveZones.Add(this);
        VisualRuntimeRegistry.Register(VisualRuntimeKind.PersistentZone);
    }

    private void OnDisable()
    {
        ActiveZones.Remove(this);
        VisualRuntimeRegistry.Unregister(VisualRuntimeKind.PersistentZone);
    }

    private static void CleanupZones()
    {
        for (int i = ActiveZones.Count - 1; i >= 0; i--)
        {
            if (ActiveZones[i] == null)
                ActiveZones.RemoveAt(i);
        }
    }

    public static PersistentSpellZone Create(
        CompiledSpell spell,
        CastRequest request,
        Vector3 position,
        float radius)
    {
        return CreateInternal(spell, request, position, radius, 1f);
    }

    private static PersistentSpellZone CreateInternal(
        CompiledSpell spell,
        CastRequest request,
        Vector3 position,
        float radius,
        float initialPower)
    {
        if (spell == null || request.budget == null)
            return null;

        CleanupZones();
        float desiredRadius = Mathf.Clamp(radius, 0.7f, 8f);
        float desiredLife = Mathf.Max(
            1.2f,
            spell.zoneDuration > 0f
                ? spell.zoneDuration
                : spell.trailDuration);

        ReactionContext22 baseContext = SpellReactionBridge22.ContextFor(
            request,
            request.generation <= 0
                ? ReactionSourceKind22.DirectCast
                : ReactionSourceKind22.RuneDerived);
        ReactionFieldAuthority22 authority =
            request.generation <= 0 && spell.zoneDuration > 0f
                ? ReactionFieldAuthority22.ExplicitPersistent
                : ReactionFieldAuthority22.SecondaryPropagation;

        if (!baseContext.canCreateField)
        {
            PresentationResidue2.SpawnStandalone(
                position,
                ElementalReactionCodex.FromSpellElement(spell.element),
                desiredRadius,
                Mathf.Min(1.2f, desiredLife),
                unchecked((int)(baseContext.originCastId ^
                    (baseContext.originCastId >> 32))),
                PresentationPriority.Normal);
            return null;
        }

        PersistentSpellZone mergeTarget =
            FindCompatible(spell, position, desiredRadius);
        if (mergeTarget != null)
        {
            mergeTarget.Merge(
                spell,
                request,
                desiredRadius,
                desiredLife,
                authority,
                initialPower);
            return mergeTarget;
        }

        if (!EnsureCapacity(position, desiredLife))
        {
            ReactionDiagnostics22.RecordFieldRejected(
                ElementalReactionCodex.FromSpellElement(spell.element));
            return null;
        }

        if (!request.budget.TryReserveEntity())
            return null;

        GameObject go = new GameObject(spell.displayName + " Zone");
        go.transform.position = position;

        PersistentSpellZone zone = go.AddComponent<PersistentSpellZone>();
        zone._spell = spell;
        zone._request = request;
        zone._context = PrepareContext(baseContext, authority);
        zone._authority = authority;
        zone.Radius = desiredRadius;
        zone._baseLife = desiredLife;
        zone._life = desiredLife;
        zone._power = Mathf.Clamp(initialPower, 0.25f, 1f);
        zone._createdAt = Time.time;
        zone._tick = 0.35f;
        zone.ReflectsProjectiles = spell.reflectsProjectiles;

        SpellDeliveryVisuals.AttachZone(
            go,
            spell,
            request,
            zone.Radius,
            zone._life);

        ReactionElement signature =
            ElementalReactionCodex.FromSpellElement(spell.element);
        int seed = unchecked((int)(zone._context.originCastId ^
            (zone._context.originCastId >> 32)));
        PresentationResidue2.AttachSpellZone(
            go,
            signature,
            zone.Radius,
            zone._life,
            seed,
            PresentationPriority.Important);

        ReactionPresentation22.EmitField(
            SpellPresentationEventType.FieldCreated,
            go,
            signature,
            zone.Radius,
            zone._life,
            0.55f,
            zone._context,
            zone._authority);

        return zone;
    }

    public static void CreateLine(
        CompiledSpell spell,
        CastRequest request,
        Vector3 from,
        Vector3 to)
    {
        int segments = Mathf.Clamp(
            Mathf.CeilToInt(Vector3.Distance(from, to) / 1.8f),
            2,
            5);

        float segmentPower = Mathf.Clamp(
            1.15f / Mathf.Sqrt(segments),
            0.35f,
            0.65f);
        for (int i = 0; i < segments; i++)
        {
            CreateInternal(
                spell,
                request,
                Vector3.Lerp(from, to, i / (float)(segments - 1)),
                1.1f,
                segmentPower);
        }
    }

    private static PersistentSpellZone FindCompatible(
        CompiledSpell spell,
        Vector3 position,
        float radius)
    {
        PersistentSpellZone best = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < ActiveZones.Count; i++)
        {
            PersistentSpellZone zone = ActiveZones[i];
            if (zone == null || zone._spell == null ||
                zone._spell.element != spell.element)
                continue;

            Vector3 delta = zone.transform.position - position;
            delta.y = 0f;
            float mergeDistance = Mathf.Max(zone.Radius, radius) * 0.80f;
            if (delta.sqrMagnitude > mergeDistance * mergeDistance ||
                delta.sqrMagnitude >= bestDistance)
                continue;

            best = zone;
            bestDistance = delta.sqrMagnitude;
        }

        return best;
    }

    private static bool EnsureCapacity(Vector3 position, float desiredLife)
    {
        ElementalReactionField[] reactionFields =
            ElementalReactionField.Snapshot();
        int total = ActiveZones.Count + reactionFields.Length;
        int local = 0;
        float localRadiusSquared = 12f * 12f;
        PersistentSpellZone weakestLocal = null;
        float weakestLocalScore = float.MaxValue;

        for (int i = 0; i < ActiveZones.Count; i++)
        {
            PersistentSpellZone zone = ActiveZones[i];
            if (zone == null)
                continue;

            Vector3 delta = zone.transform.position - position;
            delta.y = 0f;
            if (delta.sqrMagnitude <= localRadiusSquared)
            {
                local++;
                float score = zone._life + zone._power * 2f;
                if (score < weakestLocalScore)
                {
                    weakestLocalScore = score;
                    weakestLocal = zone;
                }
            }
        }

        for (int i = 0; i < reactionFields.Length; i++)
        {
            ElementalReactionField field = reactionFields[i];
            if (field == null)
                continue;
            Vector3 delta = field.transform.position - position;
            delta.y = 0f;
            if (delta.sqrMagnitude <= localRadiusSquared)
                local++;
        }

        if (local >= ReactionBalance22.MaximumLocalGameplayFields)
        {
            if (weakestLocal == null || weakestLocal._life >= desiredLife)
                return false;
            ActiveZones.Remove(weakestLocal);
            Destroy(weakestLocal.gameObject);
            total--;
        }

        while (total >= ReactionBalance22.MaximumGameplayFields)
        {
            PersistentSpellZone weakest = FindWeakest();
            if (weakest == null || weakest._life >= desiredLife)
                return false;
            ActiveZones.Remove(weakest);
            Destroy(weakest.gameObject);
            total--;
        }

        return true;
    }

    private static PersistentSpellZone FindWeakest()
    {
        PersistentSpellZone weakest = null;
        float weakestScore = float.MaxValue;
        for (int i = 0; i < ActiveZones.Count; i++)
        {
            PersistentSpellZone zone = ActiveZones[i];
            if (zone == null)
                continue;
            float score = zone._life + zone._power * 2f +
                Mathf.Max(0f, Time.time - zone._createdAt) * -0.02f;
            if (score < weakestScore)
            {
                weakest = zone;
                weakestScore = score;
            }
        }
        return weakest;
    }

    private static ReactionContext22 PrepareContext(
        ReactionContext22 context,
        ReactionFieldAuthority22 authority)
    {
        context = context
            .AsSource(ReactionSourceKind22.Field)
            .WithoutFieldCreation();

        if (authority != ReactionFieldAuthority22.ExplicitPersistent)
        {
            context.canActivateMajor = false;
            context.canTriggerDeathReaction = false;
            context.canRechargeReaction = false;
            context.reactionRechargeCoefficient = 0f;
        }

        return context;
    }

    private void Merge(
        CompiledSpell spell,
        CastRequest request,
        float radius,
        float duration,
        ReactionFieldAuthority22 authority,
        float incomingPower)
    {
        if (spell.damage * request.powerScale >
            _spell.damage * _request.powerScale)
        {
            _spell = spell;
            _request = request;
        }

        if (ReactionBalance22.FieldPriority(authority) >
            ReactionBalance22.FieldPriority(_authority))
        {
            _authority = authority;
            _context = PrepareContext(
                SpellReactionBridge22.ContextFor(
                    request,
                    request.generation <= 0
                        ? ReactionSourceKind22.DirectCast
                        : ReactionSourceKind22.RuneDerived),
                authority);
        }

        Radius = Mathf.Max(Radius, radius);
        _baseLife = Mathf.Max(_baseLife, duration);
        _life = Mathf.Min(
            _baseLife * 1.25f,
            Mathf.Max(_life, duration) + duration * 0.20f);
        _power = Mathf.Min(
            ReactionBalance22.FieldPowerCap,
            Mathf.Max(_power, incomingPower) +
                ReactionBalance22.FieldReinforcement);
        ReflectsProjectiles |= spell.reflectsProjectiles;

        PresentationResidue2 residue = GetComponent<PresentationResidue2>();
        if (residue != null)
        {
            residue.Merge(
                ElementalReactionCodex.FromSpellElement(spell.element),
                Radius,
                _life,
                _power);
        }

        ReactionDiagnostics22.RecordFieldMerge(
            ElementalReactionCodex.FromSpellElement(spell.element));
        ReactionPresentation22.EmitField(
            SpellPresentationEventType.FieldMerged,
            gameObject,
            ElementalReactionCodex.FromSpellElement(spell.element),
            Radius,
            _life,
            0.45f,
            _context,
            _authority);
    }

    private void Update()
    {
        if (GameWorld.Instance == null || !GameWorld.Instance.RunActive)
        {
            Destroy(gameObject);
            return;
        }

        _life -= Time.deltaTime;
        _tick -= Time.deltaTime;

        if (_tick <= 0f)
        {
            _tick = 0.85f;
            Pulse();
        }

        if (_life <= 0f)
            Expire();
    }

    private void Pulse()
    {
        if (_spell == null || GameWorld.Instance == null)
            return;

        ReactionElement element =
            ElementalReactionCodex.FromSpellElement(_spell.element);
        ReactionContext22 fieldContext = _context
            .AsSource(ReactionSourceKind22.Field)
            .WithoutFieldCreation();

        EnemyController[] targets = GameWorld.Instance.Enemies
            .Where(e => e != null && !e.IsDead &&
                CombatMath.PlanarDistanceSquared(
                    e.transform.position,
                    transform.position) < Radius * Radius)
            .ToArray();

        for (int i = 0; i < targets.Length; i++)
        {
            EnemyController enemy = targets[i];
            bool wasAlive = !enemy.IsDead;
            ElementalReactionRuntime.DealReactionDamage(
                enemy,
                _spell.damage * _request.powerScale * 0.28f * _power *
                    ReactionBalance22.FieldDamageAuthority(_authority),
                element,
                _spell.primaryColor,
                false,
                fieldContext);
            ElementalReactionRuntime.ApplyBuildup(
                enemy,
                element,
                0.22f * _power *
                    ReactionBalance22.FieldBuildupAuthority(_authority),
                4f,
                fieldContext);

            if (wasAlive && enemy.IsDead)
                SpellExecutor.NotifyKill(
                    _spell,
                    _request,
                    enemy.transform.position);
        }

        if (_spell.pullsEnemies)
        {
            foreach (EnemyController enemy in targets.Take(4))
                enemy.PullToward(transform.position, 1.4f);
        }

        PresentationResidue2 residue = GetComponent<PresentationResidue2>();
        if (residue != null)
            residue.Pulse(Mathf.Min(0.65f, 0.32f * _power));

        ReactionPresentation22.EmitField(
            SpellPresentationEventType.FieldPulsed,
            gameObject,
            element,
            Radius,
            0.25f,
            0.38f * _power,
            fieldContext,
            _authority);
    }

    private void Expire()
    {
        if (_expired)
            return;
        _expired = true;

        ReactionPresentation22.EmitField(
            SpellPresentationEventType.FieldExpired,
            gameObject,
            ElementalReactionCodex.FromSpellElement(_spell.element),
            Radius,
            0.35f,
            0.32f,
            _context,
            _authority);
        SpellExecutor.ZoneExpired(_spell, _request, transform.position);
        Destroy(gameObject);
    }
}

public sealed class SpellFamiliar : MonoBehaviour
    {
        private CompiledSpell _spell;
        private CastRequest _request;
        private float _life;
        private float _attack;
        private int _index;

        public static SpellFamiliar Create(CompiledSpell spell, CastRequest request, Vector3 position, int index)
        {
            GameObject go = RuntimeVisuals.Primitive(spell.displayName + " Familiar", PrimitiveType.Sphere, position, Vector3.one * 0.45f, spell.primaryColor);
            RuntimeVisuals.RemoveCollider(go);
            VisualCounterRegistration.Attach(go, VisualRuntimeKind.Familiar);
            SpellFamiliar familiar = go.AddComponent<SpellFamiliar>(); familiar._spell = spell; familiar._request = request;
            SpellVisualAttachment.Attach(go, spell, request);
            SpellDeliveryVisuals.AttachFamiliar(go, spell, request, index);
            familiar._life = Mathf.Max(5f, spell.lifetime); familiar._index = index; return familiar;
        }

        private void Update()
        {
            if (GameWorld.Instance == null || !GameWorld.Instance.RunActive) { Destroy(gameObject); return; }
            _life -= Time.deltaTime; _attack -= Time.deltaTime;
            Vector3 center = GameWorld.Instance.Player.transform.position;
            float angle = Time.time * 1.6f + _index * 2.1f;
            Vector3 desired = center + new Vector3(Mathf.Cos(angle) * 2f, 1.2f, Mathf.Sin(angle) * 2f);
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-5f * Time.deltaTime));
            if (_attack <= 0f)
            {
                EnemyController target = GameWorld.Instance.NearestEnemy(transform.position, 9f);
                if (target != null)
                {
                    _attack = 0.85f;
                    CastRequest shot = _request; shot.origin = transform.position; shot.targetPosition = target.transform.position;
                    shot.castOrigin = shot.origin;
                    shot.direction = (target.transform.position - transform.position).normalized; shot.powerScale *= 0.42f; shot.generation++;
                    int count = Mathf.Clamp(_spell.projectileCount, 1, 8);
                    for (int projectileIndex = 0; projectileIndex < count; projectileIndex++)
                    {
                        if (shot.budget == null || !shot.budget.TryReserveEntity()) break;
                        CastRequest projectileRequest = shot;
                        Vector3 direction;
                        if (_spell.projectilePattern == ProjectilePattern.Ring)
                        {
                            direction = Quaternion.Euler(0f, projectileIndex / (float)count * 360f, 0f) * Vector3.forward;
                        }
                        else
                        {
                            float spread = _spell.projectilePattern == ProjectilePattern.Cone ? Mathf.Max(21f, _spell.spreadDegrees) : _spell.spreadDegrees;
                            float offset = count == 1 ? 0f : Mathf.Lerp(-spread, spread, projectileIndex / (float)(count - 1));
                            direction = Quaternion.Euler(0f, offset, 0f) * shot.direction;
                            if (_spell.projectilePattern == ProjectilePattern.Converge)
                            {
                                Vector3 right = Vector3.Cross(Vector3.up, shot.direction).normalized;
                                projectileRequest.origin += right * (count == 1 ? 0f : Mathf.Lerp(-1.1f, 1.1f, projectileIndex / (float)(count - 1)));
                                Vector3 convergence = shot.targetPosition - projectileRequest.origin;
                                convergence.y = 0f;
                                if (convergence.sqrMagnitude > 0.01f) direction = convergence.normalized;
                            }
                        }
                        SpellProjectile.Create(_spell, projectileRequest, direction, false);
                    }
                }
            }
            if (_life <= 0f) { SpellExecutor.Expire(_spell, _request, transform.position); Destroy(gameObject); }
        }
    }
}
