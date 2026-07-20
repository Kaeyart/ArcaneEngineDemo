using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum SpellLinkCondition
    {
        OnCast,
        OnHit,
        OnKill,
        OnExpire,
        OnCriticalHit,
        OnStatusApplied,
        OnStatusConsumed,
        OnProjectileImpact,
        OnProjectileExpire,
        OnDodge,
        OnBlock,
        OnShieldBreak,
        OnDamageTaken,
        OnPeriodic
    }

    [Serializable]
    public sealed class SpellLinkSave
    {
        public string instanceId;
        public int sourceSlot;
        public int destinationSlot;
        public SpellLinkCondition condition;

        public SpellLinkSave Clone()
        {
            return new SpellLinkSave
            {
                instanceId = instanceId,
                sourceSlot = sourceSlot,
                destinationSlot = destinationSlot,
                condition = condition
            };
        }
    }

    public static class SpellLinkRules
    {
        public static string DisplayName(SpellLinkCondition condition)
        {
            switch (condition)
            {
                case SpellLinkCondition.OnCast: return "On Cast";
                case SpellLinkCondition.OnHit: return "On Hit";
                case SpellLinkCondition.OnKill: return "On Kill";
                case SpellLinkCondition.OnExpire: return "On Expire";
                case SpellLinkCondition.OnCriticalHit: return "On Critical Hit";
                case SpellLinkCondition.OnStatusApplied: return "On Status Applied";
                case SpellLinkCondition.OnStatusConsumed: return "On Status Consumed";
                case SpellLinkCondition.OnProjectileImpact: return "On Projectile Impact";
                case SpellLinkCondition.OnProjectileExpire: return "On Projectile Expire";
                case SpellLinkCondition.OnDodge: return "On Dodge";
                case SpellLinkCondition.OnBlock: return "On Block";
                case SpellLinkCondition.OnShieldBreak: return "On Shield Break";
                case SpellLinkCondition.OnPeriodic: return "Every 6 Seconds";
                default: return "On Damage Taken";
            }
        }

        public static string Description(SpellLinkCondition condition)
        {
            switch (condition)
            {
                case SpellLinkCondition.OnCast: return "Cast the linked spell when the source is manually cast.";
                case SpellLinkCondition.OnHit: return "Cast the linked spell at the first enemy hit by each source cast.";
                case SpellLinkCondition.OnKill: return "Cast the linked spell where the source kills an enemy.";
                case SpellLinkCondition.OnExpire: return "Cast the linked spell when the source projectile, zone, or summon ends.";
                case SpellLinkCondition.OnCriticalHit: return "Cast the linked spell at the first critical hit from each source cast.";
                case SpellLinkCondition.OnStatusApplied: return "Cast the linked spell when the source first applies a status.";
                case SpellLinkCondition.OnStatusConsumed: return "Cast the linked spell when the source consumes an existing status.";
                case SpellLinkCondition.OnProjectileImpact: return "Cast the linked spell when a projectile from the source impacts.";
                case SpellLinkCondition.OnProjectileExpire: return "Cast the linked spell when a projectile from the source expires.";
                case SpellLinkCondition.OnDodge: return "Cast the linked spell after the player dodges.";
                case SpellLinkCondition.OnBlock: return "Cast the linked spell after a ward blocks damage.";
                case SpellLinkCondition.OnShieldBreak: return "Cast the linked spell when the player's Shield breaks.";
                case SpellLinkCondition.OnPeriodic: return "Cast the linked spell every six seconds while a combat encounter is active.";
                default: return "Cast the linked spell after the player takes damage.";
            }
        }

        public static float TriggerPower(SpellLinkCondition condition)
        {
            switch (condition)
            {
                case SpellLinkCondition.OnKill: return 0.85f;
                case SpellLinkCondition.OnExpire: return 0.70f;
                case SpellLinkCondition.OnCriticalHit: return 0.70f;
                case SpellLinkCondition.OnStatusApplied: return 0.65f;
                case SpellLinkCondition.OnHit: return 0.60f;
                case SpellLinkCondition.OnDodge: return 0.60f;
                case SpellLinkCondition.OnBlock: return 0.65f;
                case SpellLinkCondition.OnShieldBreak: return 0.80f;
                case SpellLinkCondition.OnDamageTaken: return 0.55f;
                case SpellLinkCondition.OnPeriodic: return 0.45f;
                default: return 0.55f;
            }
        }

        public static float Cooldown(SpellLinkCondition condition)
        {
            switch (condition)
            {
                case SpellLinkCondition.OnKill: return 0.25f;
                case SpellLinkCondition.OnCast: return 1.20f;
                case SpellLinkCondition.OnHit: return 1.50f;
                case SpellLinkCondition.OnExpire: return 1.00f;
                case SpellLinkCondition.OnCriticalHit: return 1.00f;
                case SpellLinkCondition.OnDodge: return 1.25f;
                case SpellLinkCondition.OnBlock: return 1.5f;
                case SpellLinkCondition.OnShieldBreak: return 3f;
                case SpellLinkCondition.OnDamageTaken: return 2f;
                case SpellLinkCondition.OnPeriodic: return 6f;
                default: return 1.25f;
            }
        }

        public static TriggerMoment ToMoment(SpellLinkCondition condition)
        {
            switch (condition)
            {
                case SpellLinkCondition.OnCast: return TriggerMoment.OnCast;
                case SpellLinkCondition.OnHit: return TriggerMoment.OnHit;
                case SpellLinkCondition.OnKill: return TriggerMoment.OnKill;
                case SpellLinkCondition.OnExpire: return TriggerMoment.OnExpire;
                case SpellLinkCondition.OnCriticalHit: return TriggerMoment.OnCriticalHit;
                case SpellLinkCondition.OnStatusApplied: return TriggerMoment.OnStatusApplied;
                case SpellLinkCondition.OnStatusConsumed: return TriggerMoment.OnStatusConsumed;
                case SpellLinkCondition.OnProjectileImpact: return TriggerMoment.OnProjectileImpact;
                case SpellLinkCondition.OnProjectileExpire: return TriggerMoment.OnProjectileExpire;
                case SpellLinkCondition.OnDodge: return TriggerMoment.OnDodge;
                case SpellLinkCondition.OnBlock: return TriggerMoment.OnBlock;
                case SpellLinkCondition.OnShieldBreak: return TriggerMoment.OnShieldBreak;
                case SpellLinkCondition.OnPeriodic: return TriggerMoment.OnPeriodic;
                default: return TriggerMoment.OnDamageTaken;
            }
        }

        public static bool IsCompatible(CompiledSpell source, CompiledSpell destination, SpellLinkCondition condition, out string reason)
        {
            if (source == null || destination == null) { reason = "Both Link slots must contain an equipped spell."; return false; }
            if ((condition == SpellLinkCondition.OnProjectileImpact || condition == SpellLinkCondition.OnProjectileExpire) &&
                source.delivery != SpellDelivery.Projectile)
            { reason = "This event requires a projectile source spell."; return false; }
            if ((condition == SpellLinkCondition.OnStatusApplied || condition == SpellLinkCondition.OnStatusConsumed) &&
                source.poisonDamage <= 0f && source.burnDamage <= 0f && source.chillMagnitude <= 0f && source.shockMagnitude <= 0f &&
                source.bleedDamage <= 0f && source.curseMagnitude <= 0f && source.weakenMagnitude <= 0f &&
                source.vulnerabilityMagnitude <= 0f && !source.consumesStatus)
            { reason = "This event requires a source spell that applies or consumes a status."; return false; }
            if ((condition == SpellLinkCondition.OnBlock || condition == SpellLinkCondition.OnShieldBreak) && source.delivery != SpellDelivery.Defensive)
            { reason = "Block events require a defensive source spell."; return false; }
            if (condition == SpellLinkCondition.OnExpire && source.delivery == SpellDelivery.Hitscan)
            { reason = "An instant Hitscan spell has no expiring object."; return false; }
            reason = "Compatible.";
            return true;
        }

        public static bool HasCycle(IEnumerable<SpellLinkSave> links)
        {
            Dictionary<int, int> graph = new Dictionary<int, int>();
            if (links != null)
                foreach (SpellLinkSave link in links)
                    if (link != null) graph[link.sourceSlot] = link.destinationSlot;
            foreach (int origin in graph.Keys)
            {
                HashSet<int> visited = new HashSet<int>();
                int current = origin;
                while (graph.ContainsKey(current))
                {
                    if (!visited.Add(current)) return true;
                    current = graph[current];
                }
            }
            return false;
        }
    }

    public sealed class SpellLinkSystem
    {
        private readonly GameWorld _world;
        private readonly List<SpellLinkSave> _links = new List<SpellLinkSave>();
        private readonly HashSet<SpellLinkCondition> _ownedConditions = new HashSet<SpellLinkCondition>();
        private readonly List<SpellLinkCondition> _pendingChoices = new List<SpellLinkCondition>();
        private readonly Dictionary<string, float> _lastActivationTimes = new Dictionary<string, float>();
        private readonly Dictionary<string, int> _lastEventIds = new Dictionary<string, int>();

        public int Slots { get; private set; } = 1;
        public SpellLinkCondition? PendingCondition { get; private set; }
        public IReadOnlyList<SpellLinkSave> Links { get { return _links; } }
        public IEnumerable<SpellLinkCondition> OwnedConditions { get { return _ownedConditions.OrderBy(value => value); } }
        public IReadOnlyList<SpellLinkCondition> PendingChoices { get { return _pendingChoices; } }
        public bool HasPendingReward { get { return _pendingChoices.Count > 0 || PendingCondition.HasValue; } }

        public SpellLinkSystem(GameWorld world) { _world = world; }

        public void ResetRun()
        {
            _links.Clear();
            _ownedConditions.Clear();
            _pendingChoices.Clear();
            _lastActivationTimes.Clear();
            _lastEventIds.Clear();
            PendingCondition = null;
            Slots = 1;
            if (_world != null) _world.MarkSpellsDirty();
        }

        public bool AddSlot(out string message)
        {
            if (Slots >= 3) { message = "All three Link Slots are already available."; return false; }
            Slots++;
            message = "Link Slot " + Slots + " unlocked for this run.";
            return true;
        }

        public void GrantChoices(int seed)
        {
            _pendingChoices.Clear();
            SpellLinkCondition[] values = (SpellLinkCondition[])Enum.GetValues(typeof(SpellLinkCondition));
            List<SpellLinkCondition> ordered = values.OrderBy(value => _ownedConditions.Contains(value) ? 1 : 0)
                .ThenBy(value => V1Determinism.Combine(seed, (int)value, value.ToString())).ToList();
            foreach (SpellLinkCondition condition in ordered)
            {
                if (_pendingChoices.Contains(condition)) continue;
                _pendingChoices.Add(condition);
                if (_pendingChoices.Count == 3) break;
            }
            PendingCondition = null;
        }

        public bool SelectPendingCondition(SpellLinkCondition condition, out string message)
        {
            if (!_pendingChoices.Contains(condition)) { message = "That condition is not part of the current reward."; return false; }
            _pendingChoices.Clear();
            _ownedConditions.Add(condition);
            PendingCondition = condition;
            message = SpellLinkRules.DisplayName(condition) + " selected. Choose its source and destination spells.";
            return true;
        }

        public void GrantLegacyCondition(SpellLinkCondition condition) { _ownedConditions.Add(condition); }

        public void ClearPendingCondition() { PendingCondition = null; _pendingChoices.Clear(); }

        public bool TryAdd(int sourceSlot, int destinationSlot, SpellLinkCondition condition, out string message)
        {
            if (_links.Count >= Slots) { message = "No Link Slot is available."; return false; }
            if (!_ownedConditions.Contains(condition)) { message = "That Link condition has not been obtained this run."; return false; }
            if (sourceSlot < 0 || sourceSlot > 2 || destinationSlot < 0 || destinationSlot > 2 || sourceSlot == destinationSlot)
            { message = "Choose two different equipped spells."; return false; }
            if (!_world.HasSpell((SpellSlot)sourceSlot) || !_world.HasSpell((SpellSlot)destinationSlot))
            { message = "Both sides of a Spell Link must contain an equipped spell."; return false; }
            if (!SpellLinkRules.IsCompatible(_world.GetSpell((SpellSlot)sourceSlot), _world.GetSpell((SpellSlot)destinationSlot), condition, out message))
                return false;
            if (_links.Any(link => link.sourceSlot == sourceSlot))
            { message = "That source spell already has an outgoing link."; return false; }
            _links.Add(new SpellLinkSave
            {
                instanceId = Guid.NewGuid().ToString("N"),
                sourceSlot = sourceSlot,
                destinationSlot = destinationSlot,
                condition = condition
            });
            if (PendingCondition.HasValue && PendingCondition.Value == condition) PendingCondition = null;
            _world.MarkSpellsDirty();
            bool cyclic = SpellLinkRules.HasCycle(_links);
            message = "Linked Spell Slot " + (sourceSlot + 1) + " to Spell Slot " + (destinationSlot + 1) + " " + SpellLinkRules.DisplayName(condition) +
                (cyclic ? ". Safe cycle enabled: generation and trigger-energy limits guarantee termination." : ".");
            return true;
        }

        public bool Remove(string instanceId, out string message)
        {
            SpellLinkSave link = _links.FirstOrDefault(value => value.instanceId == instanceId);
            if (link == null) { message = "That Spell Link no longer exists."; return false; }
            _links.Remove(link);
            _lastActivationTimes.Remove(instanceId);
            _lastEventIds.Remove(instanceId);
            _world.MarkSpellsDirty();
            message = "Spell Link removed. Its condition remains available this run.";
            return true;
        }

        public void Restore(int slots, IEnumerable<int> ownedConditions, IEnumerable<SpellLinkSave> links)
        {
            ResetRun();
            Slots = Mathf.Clamp(slots <= 0 ? 1 : slots, 1, 3);
            if (ownedConditions != null)
                foreach (int value in ownedConditions)
                    if (Enum.IsDefined(typeof(SpellLinkCondition), value)) _ownedConditions.Add((SpellLinkCondition)value);
            if (links == null) return;
            foreach (SpellLinkSave saved in links)
            {
                if (saved == null || _links.Count >= Slots || !_ownedConditions.Contains(saved.condition)) continue;
                string ignored;
                TryAdd(saved.sourceSlot, saved.destinationSlot, saved.condition, out ignored);
            }
        }

        public void Activate(SpellSlot sourceSlot, TriggerMoment moment, CastRequest parent, Vector3 position)
        {
            if (parent.budget == null || parent.generation >= 6) return;
            foreach (SpellLinkSave link in _links.Where(value => value.sourceSlot == (int)sourceSlot && SpellLinkRules.ToMoment(value.condition) == moment).ToArray())
            {
                if (link.condition == SpellLinkCondition.OnCast && !parent.manualCast) continue;
                int lastEvent;
                if (_lastEventIds.TryGetValue(link.instanceId, out lastEvent) && lastEvent == parent.budget.eventId) continue;
                float lastTime;
                if (_lastActivationTimes.TryGetValue(link.instanceId, out lastTime) && Time.time - lastTime < SpellLinkRules.Cooldown(link.condition)) continue;
                CompiledSpell destination = _world.GetSpell((SpellSlot)link.destinationSlot);
                if (destination == null) continue;

                _lastEventIds[link.instanceId] = parent.budget.eventId;
                _lastActivationTimes[link.instanceId] = Time.time;
                CastRequest child = parent;
                child.origin = position + Vector3.up * 0.08f;
                child.castOrigin = child.origin;
                if (link.condition != SpellLinkCondition.OnCast) child.targetPosition = position;
                Vector3 direction = child.targetPosition - child.origin;
                direction.y = 0f;
                child.direction = direction.sqrMagnitude > 0.01f ? direction.normalized : parent.direction;
                child.powerScale = parent.powerScale * SpellLinkRules.TriggerPower(link.condition);
                child.generation = parent.generation + 1;
                child.manualCast = false;
                SpellVisualEvents.LinkActivation(_world.GetSpell(sourceSlot), destination, position, moment);
                SpellExecutor.Cast(destination, child);
            }
        }

        public float CooldownRemaining(SpellLinkSave link)
        {
            if (link == null) return 0f;
            float last;
            return _lastActivationTimes.TryGetValue(link.instanceId, out last)
                ? Mathf.Max(0f, SpellLinkRules.Cooldown(link.condition) - (Time.time - last)) : 0f;
        }

        private bool WouldCreateCycle(int source, int destination)
        {
            List<SpellLinkSave> projected = _links.Select(link => link.Clone()).ToList();
            projected.RemoveAll(link => link.sourceSlot == source);
            projected.Add(new SpellLinkSave { sourceSlot = source, destinationSlot = destination, condition = SpellLinkCondition.OnHit });
            return SpellLinkRules.HasCycle(projected);
        }
    }
}
