using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public static class DemoCatalog
    {
        private static readonly Dictionary<string, SpellCoreDefinition> Cores = new Dictionary<string, SpellCoreDefinition>();
        private static readonly Dictionary<string, SpellModifierDefinition> Modifiers = new Dictionary<string, SpellModifierDefinition>();
        private static readonly Dictionary<string, ItemDefinition> Items = new Dictionary<string, ItemDefinition>();
        private static bool _initialized;

        public static IEnumerable<SpellCoreDefinition> AllCores { get { Ensure(); return Cores.Values; } }
        public static IEnumerable<SpellModifierDefinition> AllModifiers { get { Ensure(); return Modifiers.Values.Where(value => value.availableAsSupport); } }
        public static IEnumerable<ItemDefinition> AllItems { get { Ensure(); return Items.Values; } }

        public static void Ensure()
        {
            if (_initialized) return;
            _initialized = true;
            BuildCores();
            BuildModifiers();
            BuildItems();
            V21AuthoredContentOverlay.Apply(Cores, Modifiers, Items);
            MegaCatalog.Ensure();
            ContentValidator.ValidateAll();
        }

        public static SpellCoreDefinition GetCore(string id)
        {
            Ensure();
            SpellCoreDefinition value;
            Cores.TryGetValue(id, out value);
            return value;
        }

        public static SpellModifierDefinition GetModifier(string id)
        {
            Ensure();
            SpellModifierDefinition value;
            Modifiers.TryGetValue(id, out value);
            return value;
        }

        public static ItemDefinition GetItem(string id)
        {
            Ensure();
            ItemDefinition value;
            Items.TryGetValue(id, out value);
            return value;
        }

        private static void BuildCores()
        {
            AddCore("fireball", "Fireball", "A fast projectile that detonates on impact.", SpellDelivery.Projectile, SpellCastMethod.Instant,
                SpellElement.Fire, new Color(1f, 0.2f, 0.04f), 24f, 13f, 0.32f, 13f, 2.4f, 0.55f, 1.4f, "projectile", "impact");
            AddCore("ice_nova", "Ice Nova", "An expanding frost pulse centered at the cast position.", SpellDelivery.Nova, SpellCastMethod.Instant,
                SpellElement.Frost, new Color(0.2f, 0.8f, 1f), 18f, 22f, 1.3f, 0f, 0.45f, 0.35f, 5f, "nova", "freeze");
            AddCore("lightning_strike", "Lightning Strike", "An instant strike at the aimed enemy.", SpellDelivery.Hitscan, SpellCastMethod.Instant,
                SpellElement.Lightning, new Color(0.42f, 0.65f, 1f), 30f, 18f, 0.72f, 0f, 0.12f, 0.35f, 8f, "hitscan", "chain");
            AddCore("arc_beam", "Arc Beam", "A focused line of arcane energy.", SpellDelivery.Beam, SpellCastMethod.Channeled,
                SpellElement.Arcane, new Color(0.86f, 0.35f, 1f), 16f, 11f, 0.38f, 0f, 0.3f, 0.3f, 10f, "beam", "channel");
            AddCore("meteor", "Meteor", "Marks an area before a heavy delayed impact.", SpellDelivery.Meteor, SpellCastMethod.Delayed,
                SpellElement.Fire, new Color(1f, 0.42f, 0.08f), 58f, 32f, 2.2f, 0f, 0.9f, 1f, 3.8f, "delayed", "area");
            AddCore("wisp_summon", "Wisp Familiar", "Summons a temporary familiar that attacks nearby enemies.", SpellDelivery.Summon, SpellCastMethod.Instant,
                SpellElement.Arcane, new Color(0.25f, 1f, 0.72f), 12f, 28f, 4f, 8f, 8f, 0.45f, 7f, "summon", "projectile");
            AddCore("blink", "Arcane Step", "Teleports toward the cursor and damages the arrival point.", SpellDelivery.Movement, SpellCastMethod.Movement,
                SpellElement.Void, new Color(0.65f, 0.18f, 1f), 20f, 20f, 2.5f, 0f, 0.25f, 0.45f, 2.5f, "movement", "nova");
            AddCore("arcane_wall", "Arcane Wall", "Creates a persistent damaging boundary.", SpellDelivery.Zone, SpellCastMethod.Instant,
                SpellElement.Arcane, new Color(0.18f, 0.75f, 1f), 9f, 26f, 3.2f, 0f, 4.5f, 0.3f, 4f, "zone", "persistent");
            AddCore("spellblade", "Spellblade", "A chargeable close magical sweep that rewards aggressive positioning.", SpellDelivery.Melee, SpellCastMethod.Charged,
                SpellElement.Arcane, new Color(1f, 0.32f, 0.68f), 34f, 14f, 0.65f, 0f, 0.25f, 0.5f, 3f, "melee", "critical");
            AddCore("aegis", "Aegis", "Raises a short-lived ward and releases a defensive pulse.", SpellDelivery.Defensive, SpellCastMethod.Instant,
                SpellElement.Frost, new Color(0.35f, 0.8f, 1f), 12f, 24f, 4.2f, 0f, 2f, 0.45f, 3.5f, "defensive", "shield");
            AddCore("poison_bolt", "Poison Bolt", "A quick toxic projectile that poisons enemies on hit.", SpellDelivery.Projectile, SpellCastMethod.Instant,
                SpellElement.Toxic, new Color(0.28f, 0.95f, 0.16f), 17f, 9f, 0.24f, 16f, 2.1f, 0.4f, 0.7f, "projectile", "poison");
            AddCore("frost_lance", "Frost Lance", "A narrow piercing beam that freezes enemies.", SpellDelivery.Beam, SpellCastMethod.Channeled,
                SpellElement.Frost, new Color(0.3f, 0.85f, 1f), 22f, 15f, 0.5f, 0f, 0.35f, 0.22f, 11f, "beam", "freeze");
            AddCore("storm_orb", "Storm Orb", "A slower lightning orb built for chaining and area damage.", SpellDelivery.Projectile, SpellCastMethod.Instant,
                SpellElement.Lightning, new Color(0.4f, 0.58f, 1f), 27f, 19f, 0.7f, 7f, 3.2f, 0.85f, 1.7f, "projectile", "chain");
            AddCore("gravity_field", "Gravity Field", "Creates a damaging field at the cursor that pulls nearby enemies.", SpellDelivery.Zone, SpellCastMethod.Instant,
                SpellElement.Void, new Color(0.55f, 0.12f, 0.9f), 11f, 27f, 3.5f, 0f, 5f, 0.5f, 4.2f, "zone", "pull");
            AddCore("void_maw", "Maw Between Worlds", "An item-exclusive singularity projectile.", SpellDelivery.Projectile, SpellCastMethod.Instant,
                SpellElement.Void, new Color(0.5f, 0.08f, 0.9f), 32f, 28f, 1.8f, 5f, 3.5f, 1.4f, 2.2f, "unique", "projectile");
        }

        private static void BuildModifiers()
        {
            AddModifier("triple", "Forked Trinity", "Adds two projectiles with reduced individual damage.", "Pattern: +2 projectiles, spread, 72% damage, +18% mana.",
                ModifierEffect.MultiProjectile, ModifierCategory.Pattern, ConnectorFamily.Flow, 2f, 3, new[] { 0, 5 }, Shape(0, 0, 0, -1), new Color(1f, 0.72f, 0.2f), 4, 2);
            AddModifier("toxic", "Viridian Conversion", "Converts damage to Toxic and applies poison.", "Element replacement: poison deals damage for four seconds.",
                ModifierEffect.ToxicConversion, ModifierCategory.Element, ConnectorFamily.Element, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(0.25f, 1f, 0.15f), 3, 1);
            AddModifier("frost", "Glacial Conversion", "Converts damage to Frost and freezes enemies.", "Element replacement: 90% damage and at least 1.4 seconds freeze.",
                ModifierEffect.FrostConversion, ModifierCategory.Element, ConnectorFamily.Element, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(0.2f, 0.85f, 1f), 2, 1);
            AddModifier("lightning", "Storm Conversion", "Converts damage to Lightning and gains a chain.", "Element replacement: adds one diminishing chain target.",
                ModifierEffect.LightningConversion, ModifierCategory.Element, ConnectorFamily.Element, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(0.45f, 0.65f, 1f), 3, 1);
            AddModifier("homing", "Predator Sigil", "Projectiles seek nearby enemies.", "Movement: steering, -12% speed, +10% mana.",
                ModifierEffect.Homing, ModifierCategory.Movement, ConnectorFamily.Flow, 5.5f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.3f, 0.72f), 5, 2);
            AddModifier("arc", "Serpent Flight", "Projectiles visibly weave.", "Movement: lateral sine path and +8% damage.",
                ModifierEffect.ArcFlight, ModifierCategory.Movement, ConnectorFamily.Flow, 1.2f, 3, new[] { 0 }, Shape(0, 0), new Color(0.85f, 0.45f, 1f), 3, 1);
            AddModifier("pierce", "Needle Geometry", "Projectiles pierce two additional enemies.", "Collision: +2 pierce, 90% direct damage.",
                ModifierEffect.Pierce, ModifierCategory.Collision, ConnectorFamily.Payload, 2f, 3, new[] { 0 }, Shape(0, 0, 1, -1), new Color(0.9f, 0.9f, 0.96f), 6, 2);
            AddModifier("explosion", "Hungry Detonation", "Greatly increases effect radius.", "Payload: +2.2 effect radius and +12% mana.",
                ModifierEffect.BiggerExplosion, ModifierCategory.Payload, ConnectorFamily.Payload, 2.2f, 3, new[] { 0, 1 }, Shape(0, 0), new Color(1f, 0.42f, 0.1f), 6, 2);
            AddModifier("chain", "Storm Thread", "Damage chains to nearby enemies.", "Payload: seeks three additional targets at diminishing power.",
                ModifierEffect.ChainLightning, ModifierCategory.Payload, ConnectorFamily.Payload, 3f, 3, new[] { 0 }, Shape(0, 0), new Color(0.4f, 0.65f, 1f), 7, 3);
            AddModifier("repeat", "Echo Chamber", "Area effects repeat twice.", "Spawn: +2 pulses or repetitions, reduced damage, +20% mana.",
                ModifierEffect.RepeatPulse, ModifierCategory.Spawn, ConnectorFamily.Power, 2f, 3, new[] { 0 }, Shape(0, 0, 1, 0), new Color(0.6f, 0.4f, 1f), 8, 3);
            AddModifier("trigger_slot2", "Collision Invocation", "On hit, cast customized Slot 2 at impact.", "Trigger: 32 Energy, 65% inherited power, six activations.",
                ModifierEffect.TriggerSlot2OnHit, ModifierCategory.Trigger, ConnectorFamily.Trigger, 1f, 3, new[] { 0 }, Shape(0, 0, 0, 1), new Color(1f, 0.15f, 0.9f), 12, 5);
            AddModifier("trigger_slot1", "Execution Invocation", "On kill, cast customized Slot 1.", "Trigger: 38 Energy, 55% inherited power, four activations.",
                ModifierEffect.TriggerSlot1OnKill, ModifierCategory.Trigger, ConnectorFamily.Trigger, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.1f, 0.5f), 10, 5);
            AddModifier("trigger_slot3", "Triskelion Invocation", "On cast, invoke customized Slot 3.", "Trigger: 44 Energy, 45% inherited power, two activations.",
                ModifierEffect.TriggerSlot3OnCast, ModifierCategory.Trigger, ConnectorFamily.Trigger, 1f, 3, new[] { 0 }, Shape(0, 0, 1, 0, 0, 1), new Color(0.7f, 0.25f, 1f), 14, 6);
            AddModifier("trigger_expire", "Terminal Invocation", "On expiry, cast customized Slot 1.", "Trigger: useful for zones, returns, and delayed machinery.",
                ModifierEffect.TriggerSlot1OnExpire, ModifierCategory.Trigger, ConnectorFamily.Trigger, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.3f, 0.65f), 11, 5);
            AddModifier("nova_delivery", "Radial Rewrite", "Replaces delivery with an expanding nova.", "Delivery replacement removes projectile movement.",
                ModifierEffect.NovaDelivery, ModifierCategory.Pattern, ConnectorFamily.Wild, 1f, 3, new[] { 0 }, Shape(0, 0, 1, 0, 0, 1), new Color(1f, 0.8f, 0.25f), 14, 5);
            AddModifier("efficient", "Measured Casting", "Costs less mana but deals slightly less damage.", "Resource: 68% mana and 90% damage.",
                ModifierEffect.EfficientCasting, ModifierCategory.Resource, ConnectorFamily.Power, 0.68f, 3, new[] { 0, 1 }, Shape(0, 0), new Color(0.2f, 1f, 0.8f), 0, 1);
            AddModifier("unstable", "Forbidden Amplifier", "Massively increases damage and instability.", "Power: +60% damage with dangerous backlash.",
                ModifierEffect.UnstablePower, ModifierCategory.Instability, ConnectorFamily.Wild, 0.6f, 3, new[] { 0, 1, 5 }, Shape(0, 0, 1, 0), new Color(1f, 0.05f, 0.2f), 18, 6);
            AddModifier("bounce", "Ricochet Rune", "Projectiles bounce toward another enemy.", "Collision: two intelligent bounces at reduced power.",
                ModifierEffect.Bounce, ModifierCategory.Collision, ConnectorFamily.Payload, 2f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.65f, 0.25f), 5, 2);
            AddModifier("return", "Returning Arc", "Projectiles return to the caster after travelling.", "Movement: reverses at mid-life and may hit new targets.",
                ModifierEffect.Return, ModifierCategory.Movement, ConnectorFamily.Flow, 1f, 3, new[] { 0 }, Shape(0, 0, 1, -1), new Color(0.35f, 1f, 0.8f), 5, 2);
            AddModifier("orbit", "Orbital Law", "Delivery orbits the caster before release.", "Movement: persistent circular path around the player.",
                ModifierEffect.Orbit, ModifierCategory.Movement, ConnectorFamily.Power, 1f, 3, new[] { 0 }, Shape(0, 0, 0, -1), new Color(0.85f, 0.65f, 1f), 8, 4);
            AddModifier("split_distance", "Measured Fission", "Splits after travelling a fixed distance.", "Spawn: creates two children at 58% inherited power.",
                ModifierEffect.SplitAfterDistance, ModifierCategory.Spawn, ConnectorFamily.Payload, 2f, 3, new[] { 0, 5 }, Shape(0, 0, 1, 0), new Color(1f, 0.55f, 0.35f), 8, 3);
            AddModifier("split_hit", "Impact Fission", "Splits when hitting an enemy.", "Spawn: impact releases two inherited child projectiles.",
                ModifierEffect.SplitOnHit, ModifierCategory.Spawn, ConnectorFamily.Payload, 2f, 3, new[] { 0, 1 }, Shape(0, 0), new Color(1f, 0.4f, 0.3f), 9, 4);
            AddModifier("accelerate", "Gathering Velocity", "Projectiles accelerate over time.", "Movement: +8 speed per second.",
                ModifierEffect.Accelerate, ModifierCategory.Movement, ConnectorFamily.Flow, 8f, 3, new[] { 0 }, Shape(0, 0), new Color(0.3f, 0.85f, 1f), 2, 1);
            AddModifier("decelerate", "Heavy Approach", "Projectiles slow while gaining damage.", "Movement: deceleration and +28% payload.",
                ModifierEffect.Decelerate, ModifierCategory.Movement, ConnectorFamily.Flow, -4f, 3, new[] { 0 }, Shape(0, 0), new Color(0.8f, 0.55f, 0.25f), 3, 2);
            AddModifier("delay", "Patient Detonation", "Effects detonate after a delay.", "Payload: 0.8 second telegraph, +35% damage.",
                ModifierEffect.DelayedDetonation, ModifierCategory.Payload, ConnectorFamily.Power, 0.8f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.25f, 0.15f), 5, 2);
            AddModifier("trail", "Scorched Script", "Moving spells leave a damaging trail.", "Spawn: persistent ground trail for three seconds.",
                ModifierEffect.PersistentTrail, ModifierCategory.Spawn, ConnectorFamily.Payload, 3f, 3, new[] { 0 }, Shape(0, 0, 0, 1), new Color(1f, 0.3f, 0.08f), 8, 4);
            AddModifier("zone", "Lingering Geometry", "Impacts create a persistent damage zone.", "Spawn: four-second zone dealing periodic payload damage.",
                ModifierEffect.DamageZone, ModifierCategory.Spawn, ConnectorFamily.Payload, 4f, 3, new[] { 0 }, Shape(0, 0, 1, 0), new Color(0.65f, 0.2f, 1f), 10, 4);
            AddModifier("status_spread", "Contagion Theorem", "Statuses spread to nearby enemies.", "Payload: struck targets share poison and freeze.",
                ModifierEffect.StatusSpread, ModifierCategory.Payload, ConnectorFamily.Element, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(0.3f, 1f, 0.45f), 7, 3);
            AddModifier("status_consume", "Shatter Engine", "Consumes status for a powerful burst.", "Payload: frozen or poisoned targets take +65% damage and lose status.",
                ModifierEffect.StatusConsume, ModifierCategory.Payload, ConnectorFamily.Element, 0.65f, 3, new[] { 0 }, Shape(0, 0, 1, -1), new Color(0.65f, 0.9f, 1f), 9, 4);
            AddModifier("summon_payload", "Familiar Clause", "Impacts may create a temporary wisp.", "Spawn: summons one payload-inheriting familiar.",
                ModifierEffect.SummonPayload, ModifierCategory.Spawn, ConnectorFamily.Wild, 1f, 3, new[] { 0 }, Shape(0, 0, 0, -1), new Color(0.35f, 1f, 0.75f), 12, 5);
            AddModifier("reflect", "Mirror Formula", "Zones and wards reflect enemy projectiles.", "Collision: hostile bolts reverse ownership.",
                ModifierEffect.ReflectProjectiles, ModifierCategory.Collision, ConnectorFamily.Power, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(0.85f, 0.9f, 1f), 8, 4);
            AddModifier("overkill", "Cruel Accounting", "Stores excess kill damage for the next hit.", "Payload: up to one base hit of overkill is carried forward.",
                ModifierEffect.StoreExcessDamage, ModifierCategory.Payload, ConnectorFamily.Power, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.2f, 0.35f), 10, 5);
            AddModifier("blood_cost", "Sanguine Cost", "Adds a life sacrifice for greater power.", "Resource: costs 8 life and grants +40% damage.",
                ModifierEffect.HealthSacrifice, ModifierCategory.Resource, ConnectorFamily.Power, 8f, 3, new[] { 0 }, Shape(0, 0), new Color(0.9f, 0.05f, 0.12f), 8, 4);
            AddModifier("mana_conversion", "Mana Furnace", "Converts excess mana cost into damage.", "Resource: +30% cost and +35% power.",
                ModifierEffect.ManaConversion, ModifierCategory.Resource, ConnectorFamily.Power, 0.35f, 3, new[] { 0 }, Shape(0, 0), new Color(0.2f, 0.45f, 1f), 5, 3);
            AddModifier("target_player", "Inward Context", "Linked effects target the player.", "Target context is explicitly replaced with the caster.",
                ModifierEffect.TargetPlayer, ModifierCategory.Targeting, ConnectorFamily.Trigger, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(0.4f, 1f, 0.9f), 1, 1);
            AddModifier("target_enemy", "Predatory Context", "Linked effects target the nearest enemy.", "Target context seeks the closest legal hostile target.",
                ModifierEffect.TargetEnemy, ModifierCategory.Targeting, ConnectorFamily.Trigger, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.35f, 0.55f), 2, 2);
            AddModifier("target_impact", "Impact Context", "Linked effects occur at the impact point.", "Target context inherits the collision position.",
                ModifierEffect.TargetImpact, ModifierCategory.Targeting, ConnectorFamily.Trigger, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.6f, 0.3f), 2, 2);
            AddModifier("wild", "Impossible Junction", "Connects broadly at the cost of instability.", "Wild connector accepts any predecessor family and branches three ways.",
                ModifierEffect.WildConnector, ModifierCategory.Instability, ConnectorFamily.Wild, 1f, 3, new[] { 0, 1, 5 }, Shape(0, 0), new Color(1f, 0.1f, 0.85f), 16, 5);
            AddModifier("double_shot", "Double Shot", "Adds one projectile with slightly lower damage.", "Pattern: +1 projectile, moderate spread, and 72% damage per projectile.",
                ModifierEffect.MultiProjectile, ModifierCategory.Pattern, ConnectorFamily.Flow, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.75f, 0.25f), 2, 1);
            AddModifier("large_area", "Large Area", "Increases explosion and area size.", "Payload: +1.4 area radius and +12% Mana cost.",
                ModifierEffect.BiggerExplosion, ModifierCategory.Payload, ConnectorFamily.Payload, 1.4f, 3, new[] { 0 }, Shape(0, 0, 1, 0), new Color(1f, 0.45f, 0.15f), 4, 2);
            AddModifier("fast_projectile", "Fast Projectile", "Projectiles accelerate while travelling.", "Movement: +5 speed each second.",
                ModifierEffect.Accelerate, ModifierCategory.Movement, ConnectorFamily.Flow, 5f, 3, new[] { 0 }, Shape(0, 0), new Color(0.3f, 0.85f, 1f), 1, 1);
            AddModifier("slow_heavy", "Slow and Heavy", "Projectiles slow down and deal more damage.", "Movement: decelerates and gains 28% damage.",
                ModifierEffect.Decelerate, ModifierCategory.Movement, ConnectorFamily.Flow, -2.5f, 3, new[] { 0 }, Shape(0, 0, 0, 1), new Color(0.82f, 0.58f, 0.28f), 3, 2);
            AddModifier("delayed_blast", "Delayed Blast", "The impact explodes after a short delay.", "Payload: 0.45 second warning and +35% damage.",
                ModifierEffect.DelayedDetonation, ModifierCategory.Payload, ConnectorFamily.Power, 0.45f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.28f, 0.12f), 4, 2);
            AddModifier("damage_field", "Damage Field", "Impacts leave a damaging area for a short time.", "Spawn: creates a 2.5-second ground area.",
                ModifierEffect.DamageZone, ModifierCategory.Spawn, ConnectorFamily.Payload, 2.5f, 3, new[] { 0 }, Shape(0, 0, 1, -1), new Color(0.58f, 0.24f, 1f), 7, 3);

            AddModifier("cone", "Fanmaker Seal", "Fires compatible projectiles in a broad forward cone.", "Pattern: at least five projectiles spread across a 42 degree cone.",
                ModifierEffect.ConePattern, ModifierCategory.Pattern, ConnectorFamily.Flow, 1f, 3, new[] { 0, 1 }, Shape(0, 0), new Color(1f, 0.72f, 0.22f), 4, 3);
            AddModifier("converge", "Hunter's Focus", "Projectiles begin apart and converge at the aimed position.", "Pattern: projectiles launch from lateral offsets and converge at the target.",
                ModifierEffect.ConvergingProjectiles, ModifierCategory.Pattern, ConnectorFamily.Flow, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.6f, 0.22f), 5, 3);
            AddModifier("ring_cast", "Sunwheel Script", "Fires compatible projectiles in every direction around the caster.", "Pattern: radial release with at least eight projectiles.",
                ModifierEffect.RingPattern, ModifierCategory.Pattern, ConnectorFamily.Flow, 1f, 3, new[] { 0, 1, 5 }, Shape(0, 0), new Color(1f, 0.82f, 0.25f), 8, 4);
            AddModifier("fork", "Dividing Fang", "Projectiles fork into two children after their first hit.", "Collision: creates two children at reduced inherited power.",
                ModifierEffect.ForkOnHit, ModifierCategory.Collision, ConnectorFamily.Payload, 2f, 3, new[] { 0, 1 }, Shape(0, 0), new Color(1f, 0.48f, 0.28f), 7, 3);
            AddModifier("meteor_delivery", "Falling Star", "Changes a compatible spell into a delayed strike from above.", "Delivery replacement: delayed impact at the aimed position.",
                ModifierEffect.MeteorDelivery, ModifierCategory.Pattern, ConnectorFamily.Wild, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.35f, 0.12f), 10, 4);
            AddModifier("beam_delivery", "Luminous Thread", "Changes a compatible spell into a focused beam.", "Delivery replacement: immediate line damage using the spell's range.",
                ModifierEffect.BeamDelivery, ModifierCategory.Pattern, ConnectorFamily.Wild, 1f, 3, new[] { 0 }, Shape(0, 0), new Color(0.72f, 0.4f, 1f), 8, 4);
            AddModifier("burning", "Cinder Mark", "Adds Burning damage over time to hits.", "Status: Burning deals a portion of direct damage for four seconds.",
                ModifierEffect.Burning, ModifierCategory.Element, ConnectorFamily.Element, 0.18f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.28f, 0.06f), 4, 2);
            AddModifier("shock", "Voltaic Brand", "Hits Shock enemies, causing them to take more damage.", "Status: 15% increased damage taken for four seconds.",
                ModifierEffect.Shock, ModifierCategory.Element, ConnectorFamily.Element, 0.15f, 3, new[] { 0 }, Shape(0, 0), new Color(0.42f, 0.66f, 1f), 5, 3);
            AddModifier("chilling", "Winter's Touch", "Hits Chill enemies, slowing them and building toward a freeze.", "Status: 35% slow for three seconds. Repeated applications reach 100% Chill and freeze the target.",
                ModifierEffect.Chill, ModifierCategory.Element, ConnectorFamily.Element, 0.35f, 3, new[] { 0 }, Shape(0, 0), new Color(0.3f, 0.86f, 1f), 3, 2);
            AddModifier("bleeding", "Open Wound", "Hits apply stacking Bleed damage over time.", "Status: Bleed stacks up to five times and deals physical damage each second for four seconds.",
                ModifierEffect.Bleeding, ModifierCategory.Payload, ConnectorFamily.Payload, 0.11f, 3, new[] { 0 }, Shape(0, 0), new Color(0.82f, 0.08f, 0.14f), 5, 2);
            AddModifier("cursing", "Doom Script", "Hits Curse enemies, increasing all damage they take.", "Status: Curse increases damage taken by 12% for five seconds.",
                ModifierEffect.Cursing, ModifierCategory.Trigger, ConnectorFamily.Trigger, 0.12f, 3, new[] { 0 }, Shape(0, 0, 1, 0), new Color(0.62f, 0.12f, 0.82f), 7, 3);
            AddModifier("weaken", "Dulling Seal", "Hits Weaken enemies, reducing the damage they deal.", "Status: Weaken reduces outgoing damage by 20% for four seconds.",
                ModifierEffect.Weaken, ModifierCategory.Payload, ConnectorFamily.Power, 0.2f, 3, new[] { 0 }, Shape(0, 0), new Color(0.65f, 0.62f, 0.72f), 4, 2);
            AddModifier("vulnerability", "Expose Fault", "Hits make enemies Vulnerable to subsequent damage.", "Status: Vulnerable increases damage taken by 18% for four seconds.",
                ModifierEffect.Vulnerability, ModifierCategory.Payload, ConnectorFamily.Power, 0.18f, 3, new[] { 0 }, Shape(0, 0, 0, 1), new Color(1f, 0.42f, 0.35f), 6, 3);
            AddModifier("close_range", "Nearfield Equation", "Deals more damage near the caster and less at long range.", "Condition: up to 35% more damage inside four metres, 15% less beyond ten.",
                ModifierEffect.CloseRangePower, ModifierCategory.Payload, ConnectorFamily.Power, 0.35f, 3, new[] { 0 }, Shape(0, 0), new Color(1f, 0.34f, 0.3f), 5, 3);
            AddModifier("long_range", "Farshot Formula", "Deals more damage after travelling a meaningful distance.", "Condition: up to 40% more damage beyond ten metres, 15% less inside four.",
                ModifierEffect.LongRangePower, ModifierCategory.Payload, ConnectorFamily.Power, 0.4f, 3, new[] { 0 }, Shape(0, 0), new Color(0.35f, 0.8f, 1f), 5, 3);
            AddModifier("barrier_cast", "Warding Pulse", "Manual casts grant a small temporary barrier.", "Defence: gain a barrier based on the spell's base damage; triggered casts do not grant it.",
                ModifierEffect.BarrierOnCast, ModifierCategory.Resource, ConnectorFamily.Power, 0.65f, 3, new[] { 0 }, Shape(0, 0), new Color(0.35f, 1f, 0.82f), 6, 3);

            ApplyV14SupportDesign();
        }

        private static void BuildItems()
        {
            AddItem("starter_helm", "Initiate Hood", "A simple life-lined hood.", EquipmentSlot.Helmet, ItemRarity.Common, new Color(0.72f, 0.75f, 0.8f), health: 8f);
            AddItem("starter_shoulder_l", "Initiate Mantle L", "Light protection for the left shoulder.", EquipmentSlot.LeftShoulder, ItemRarity.Common, new Color(0.72f, 0.75f, 0.8f), armor: 2f);
            AddItem("starter_shoulder_r", "Initiate Mantle R", "Light protection for the right shoulder.", EquipmentSlot.RightShoulder, ItemRarity.Common, new Color(0.72f, 0.75f, 0.8f), armor: 2f);
            AddItem("starter_chest", "Initiate Robe", "A balanced novice robe.", EquipmentSlot.Chest, ItemRarity.Common, new Color(0.72f, 0.75f, 0.8f), health: 12f, mana: 8f);
            AddItem("starter_glove_l", "Initiate Glove L", "A minor casting aid.", EquipmentSlot.LeftGlove, ItemRarity.Common, new Color(0.72f, 0.75f, 0.8f), spellPower: 0.03f);
            AddItem("starter_glove_r", "Initiate Glove R", "A minor casting aid.", EquipmentSlot.RightGlove, ItemRarity.Common, new Color(0.72f, 0.75f, 0.8f), cooldown: 0.02f);
            AddItem("starter_pants", "Initiate Leggings", "Simple armored leggings.", EquipmentSlot.Pants, ItemRarity.Common, new Color(0.72f, 0.75f, 0.8f), armor: 3f);
            AddItem("runner_boots", "Runner's Boots", "Simple boots with strong movement speed.", EquipmentSlot.Boots, ItemRarity.Common, new Color(0.75f, 0.78f, 0.82f), movement: 1.2f);
            AddItem("iron_chest", "Ironweave Chest", "Warden set. A dependable layer of armor and life.", EquipmentSlot.Chest, ItemRarity.Common, new Color(0.65f, 0.68f, 0.72f), health: 28f, armor: 12f, setId: "warden");
            AddItem("seer_helm", "Seer's Circlet", "Storm set. A light helm tuned for critical spellwork.", EquipmentSlot.Helmet, ItemRarity.Magic, new Color(0.25f, 0.55f, 1f), mana: 22f, critChance: 0.08f, setId: "storm");
            AddItem("ember_glove_l", "Ember Glove", "Ember set. Spell power and critical damage.", EquipmentSlot.LeftGlove, ItemRarity.Rare, new Color(0.75f, 0.25f, 1f), critDamage: 0.35f, spellPower: 0.14f, setId: "ember");
            AddItem("quick_glove_r", "Quickweave Glove", "Ember set. Shortens spell cooldowns.", EquipmentSlot.RightGlove, ItemRarity.Rare, new Color(0.75f, 0.25f, 1f), cooldown: 0.16f, setId: "ember");
            AddItem("ward_shoulder_l", "Warden's Left Mantle", "Life, armor and resistance.", EquipmentSlot.LeftShoulder, ItemRarity.Magic, new Color(0.2f, 0.55f, 1f), health: 12f, armor: 5f, resistance: 0.08f, setId: "warden");
            AddItem("spark_shoulder_r", "Sparkbound Mantle", "Mana and spell power.", EquipmentSlot.RightShoulder, ItemRarity.Magic, new Color(0.2f, 0.55f, 1f), mana: 15f, spellPower: 0.08f, setId: "storm");
            AddItem("resistant_pants", "Runed Legguards", "Warden set. Resistance and armor.", EquipmentSlot.Pants, ItemRarity.Magic, new Color(0.2f, 0.55f, 1f), armor: 8f, resistance: 0.12f, setId: "warden");
            AddItem("apprentice_wand", "Apprentice Wand", "A one-handed focus with modest spell power.", EquipmentSlot.Weapon, ItemRarity.Common, new Color(0.75f, 0.78f, 0.82f), spellPower: 0.1f);
            AddItem("mana_orb", "Mana Orb", "Storm set. An offhand reservoir.", EquipmentSlot.Offhand, ItemRarity.Magic, new Color(0.2f, 0.55f, 1f), mana: 30f, triggerEnergy: 15f, setId: "storm");

            AddItem("dying_star", "Heart of the Dying Star", "Two-handed. Slot 1 becomes a slow miniature sun while remaining fully modifiable.", EquipmentSlot.Weapon, ItemRarity.Unique,
                new Color(1f, 0.35f, 0.05f), health: -15f, spellPower: 0.18f, mutation: UniqueMutation.MiniatureSunPrimary, twoHanded: true);
            AddItem("void_codex", "Codex of the Rift", "Offhand. Grants the item-exclusive Maw Between Worlds foundation to Slot 2.", EquipmentSlot.Offhand, ItemRarity.Unique,
                new Color(0.65f, 0.12f, 1f), mana: 18f, mutation: UniqueMutation.VoidMawSecondary, grantedCoreId: "void_maw");
            AddItem("gravewalker", "Gravewalker Boots", "Dodging casts customized Slot 2 where you departed.", EquipmentSlot.Boots, ItemRarity.Unique,
                new Color(0.3f, 0.95f, 0.8f), movement: 0.8f, mutation: UniqueMutation.DodgeCastsSecondary);
            AddItem("blood_covenant", "Bloodletter's Covenant", "Spells consume life instead of mana and gain power.", EquipmentSlot.Chest, ItemRarity.Unique,
                new Color(0.9f, 0.05f, 0.14f), health: 45f, spellPower: 0.25f, mutation: UniqueMutation.BloodCasting);
            AddItem("echoing_saint", "Hand of the Echoing Saint", "Slot 1 manual casts also invoke customized Slot 2.", EquipmentSlot.RightGlove, ItemRarity.Unique,
                new Color(1f, 0.82f, 0.2f), mutation: UniqueMutation.EchoPrimaryIntoSecondary);
            AddItem("trigger_relic", "Mantle of the Wild Circuit", "A huge Trigger Energy reservoir enables longer recursive chains.", EquipmentSlot.RightShoulder, ItemRarity.Unique,
                new Color(1f, 0.2f, 0.8f), triggerEnergy: 20f, mutation: UniqueMutation.WildTriggerReservoir);
            AddItem("impossible_helm", "Crown of Impossible Angles", "Reduces final Spell Overload by 45% after all Support Runes and other equipment are compiled.", EquipmentSlot.Helmet, ItemRarity.Unique,
                new Color(0.85f, 0.15f, 1f), resistance: 0.1f, mutation: UniqueMutation.ImpossibleConnectors);
            AddItem("third_echo", "Left Hand of Recurrence", "Slot 3 hits can invoke customized Slot 1.", EquipmentSlot.LeftGlove, ItemRarity.Unique,
                new Color(0.35f, 0.85f, 1f), mutation: UniqueMutation.ThirdSpellEcho);
            AddItem("corrupted_legs", "Legguards of the Red Equation", "Corrupted power raises spell output and instability.", EquipmentSlot.Pants, ItemRarity.Unique,
                new Color(1f, 0.08f, 0.18f), spellPower: 0.18f, mutation: UniqueMutation.CorruptedPower);

            // 1.1 base-item families. Rarity is rolled on the instance; these
            // definitions supply slot identity, implicits, tags and local baselines.
            AddItem("iron_crown", "Iron Crown", "A heavy defensive helmet base.", EquipmentSlot.Helmet, ItemRarity.Common, new Color(0.58f, 0.62f, 0.68f), health: 15f, armor: 6f, baseFamily: "Armored Headgear", implicitText: "+6 Armor", itemTags: new[] { "armor" });
            AddItem("ward_circlet", "Ward Circlet", "A Mana-focused caster helmet.", EquipmentSlot.Helmet, ItemRarity.Common, new Color(0.32f, 0.58f, 0.9f), mana: 18f, baseFamily: "Ward Headgear", implicitText: "+18 Maximum Mana", itemTags: new[] { "ward", "caster" });
            AddItem("wind_hood", "Wind Hood", "A lightweight mobile helmet.", EquipmentSlot.Helmet, ItemRarity.Common, new Color(0.45f, 0.78f, 0.72f), movement: 0.2f, baseFamily: "Mobile Headgear", implicitText: "+0.2 Movement Speed", itemTags: new[] { "mobile" });
            AddItem("ritual_mask", "Ritual Mask", "A trigger-attuned ceremonial mask.", EquipmentSlot.Helmet, ItemRarity.Common, new Color(0.62f, 0.35f, 0.82f), triggerEnergy: 9f, baseFamily: "Ritual Headgear", implicitText: "+9 Trigger Energy", itemTags: new[] { "trigger", "caster" });

            AddItem("iron_pauldron_l", "Iron Pauldron", "A defensive left shoulder base.", EquipmentSlot.LeftShoulder, ItemRarity.Common, new Color(0.58f, 0.62f, 0.68f), armor: 5f, baseFamily: "Armored Shoulder", implicitText: "+5 Armor", itemTags: new[] { "armor" });
            AddItem("ward_mantle_l", "Ward Mantle", "A left shoulder mantle for Shield builds.", EquipmentSlot.LeftShoulder, ItemRarity.Common, new Color(0.32f, 0.58f, 0.9f), mana: 10f, baseFamily: "Ward Shoulder", implicitText: "+10 Maximum Mana", itemTags: new[] { "ward", "caster" });
            AddItem("wind_guard_l", "Wind Guard", "A mobile left shoulder guard.", EquipmentSlot.LeftShoulder, ItemRarity.Common, new Color(0.45f, 0.78f, 0.72f), movement: 0.15f, baseFamily: "Mobile Shoulder", implicitText: "+0.15 Movement Speed", itemTags: new[] { "mobile" });
            AddItem("iron_pauldron_r", "Iron Pauldron", "A defensive right shoulder base.", EquipmentSlot.RightShoulder, ItemRarity.Common, new Color(0.58f, 0.62f, 0.68f), armor: 5f, baseFamily: "Armored Shoulder", implicitText: "+5 Armor", itemTags: new[] { "armor" });
            AddItem("circuit_mantle_r", "Circuit Mantle", "A right shoulder mantle for triggered spells.", EquipmentSlot.RightShoulder, ItemRarity.Common, new Color(0.68f, 0.28f, 0.78f), triggerEnergy: 12f, baseFamily: "Trigger Shoulder", implicitText: "+12 Trigger Energy", itemTags: new[] { "trigger", "caster" });
            AddItem("ward_guard_r", "Ward Guard", "A balanced right shoulder ward.", EquipmentSlot.RightShoulder, ItemRarity.Common, new Color(0.32f, 0.58f, 0.9f), mana: 10f, baseFamily: "Ward Shoulder", implicitText: "+10 Maximum Mana", itemTags: new[] { "ward" });

            AddItem("bastion_plate", "Bastion Plate", "A high-Armor chest base.", EquipmentSlot.Chest, ItemRarity.Common, new Color(0.52f, 0.56f, 0.64f), health: 20f, armor: 14f, baseFamily: "Armored Chest", implicitText: "+14 Armor", itemTags: new[] { "armor" });
            AddItem("astral_robe", "Astral Robe", "A Mana-rich spellcaster robe.", EquipmentSlot.Chest, ItemRarity.Common, new Color(0.3f, 0.52f, 0.9f), mana: 28f, spellPower: 0.04f, baseFamily: "Caster Robe", implicitText: "+4% Spell Power", itemTags: new[] { "ward", "caster" });
            AddItem("wayfarer_coat", "Wayfarer Coat", "A flexible chest base for mobile builds.", EquipmentSlot.Chest, ItemRarity.Common, new Color(0.42f, 0.72f, 0.64f), health: 12f, movement: 0.25f, baseFamily: "Mobile Chest", implicitText: "+0.25 Movement Speed", itemTags: new[] { "mobile" });

            AddItem("channel_glove_l", "Channel Glove", "A left glove tuned for spell channels.", EquipmentSlot.LeftGlove, ItemRarity.Common, new Color(0.42f, 0.58f, 0.88f), spellPower: 0.04f, baseFamily: "Caster Glove", implicitText: "+4% Spell Power", itemTags: new[] { "caster" });
            AddItem("needle_glove_l", "Needle Glove", "A left glove tuned for projectiles.", EquipmentSlot.LeftGlove, ItemRarity.Common, new Color(0.62f, 0.66f, 0.72f), critChance: 0.025f, baseFamily: "Projectile Glove", implicitText: "+2.5% Critical Chance", itemTags: new[] { "weapon", "caster" });
            AddItem("ritual_glove_l", "Ritual Glove", "A left glove for ailment rituals.", EquipmentSlot.LeftGlove, ItemRarity.Common, new Color(0.48f, 0.72f, 0.42f), mana: 8f, baseFamily: "Ritual Glove", implicitText: "+8 Maximum Mana", itemTags: new[] { "caster", "ward" });
            AddItem("channel_glove_r", "Channel Glove", "A right glove tuned for rapid casting.", EquipmentSlot.RightGlove, ItemRarity.Common, new Color(0.42f, 0.58f, 0.88f), cooldown: 0.025f, baseFamily: "Caster Glove", implicitText: "+2.5% Cooldown Recovery", itemTags: new[] { "caster" });
            AddItem("circuit_glove_r", "Circuit Glove", "A right glove tuned for triggers.", EquipmentSlot.RightGlove, ItemRarity.Common, new Color(0.7f, 0.3f, 0.78f), triggerEnergy: 8f, baseFamily: "Trigger Glove", implicitText: "+8 Trigger Energy", itemTags: new[] { "trigger", "caster" });
            AddItem("fleet_glove_r", "Fleet Glove", "A responsive right glove.", EquipmentSlot.RightGlove, ItemRarity.Common, new Color(0.42f, 0.72f, 0.64f), movement: 0.12f, baseFamily: "Mobile Glove", implicitText: "+0.12 Movement Speed", itemTags: new[] { "mobile" });

            AddItem("bastion_leggings", "Bastion Leggings", "Heavy defensive leg armor.", EquipmentSlot.Pants, ItemRarity.Common, new Color(0.52f, 0.56f, 0.64f), health: 16f, armor: 9f, baseFamily: "Armored Legwear", implicitText: "+9 Armor", itemTags: new[] { "armor" });
            AddItem("ritual_trousers", "Ritual Trousers", "Mana-lined ritual legwear.", EquipmentSlot.Pants, ItemRarity.Common, new Color(0.34f, 0.52f, 0.86f), mana: 16f, baseFamily: "Ward Legwear", implicitText: "+16 Maximum Mana", itemTags: new[] { "ward", "caster" });
            AddItem("strider_leggings", "Strider Leggings", "Flexible legwear for dodging.", EquipmentSlot.Pants, ItemRarity.Common, new Color(0.4f, 0.72f, 0.62f), movement: 0.24f, baseFamily: "Mobile Legwear", implicitText: "+0.24 Movement Speed", itemTags: new[] { "mobile" });

            AddItem("iron_treads", "Iron Treads", "Stable armored boots.", EquipmentSlot.Boots, ItemRarity.Common, new Color(0.54f, 0.58f, 0.64f), armor: 5f, movement: 0.35f, baseFamily: "Armored Boots", implicitText: "+5 Armor", itemTags: new[] { "armor", "mobile" });
            AddItem("windstep_boots", "Windstep Boots", "Fast lightweight boots.", EquipmentSlot.Boots, ItemRarity.Common, new Color(0.4f, 0.76f, 0.66f), movement: 0.9f, baseFamily: "Mobile Boots", implicitText: "+0.9 Movement Speed", itemTags: new[] { "mobile" });
            AddItem("wardstep_boots", "Wardstep Boots", "Mana-lined boots for mobile casters.", EquipmentSlot.Boots, ItemRarity.Common, new Color(0.34f, 0.55f, 0.88f), mana: 10f, movement: 0.5f, baseFamily: "Ward Boots", implicitText: "+10 Maximum Mana", itemTags: new[] { "ward", "mobile" });

            AddItem("ember_wand", "Ember Wand", "A one-handed Fire caster base.", EquipmentSlot.Weapon, ItemRarity.Common, new Color(0.9f, 0.35f, 0.12f), spellPower: 0.08f, baseFamily: "One-Handed Wand", implicitText: "+8% Spell Power", itemTags: new[] { "weapon", "caster" });
            AddItem("storm_scepter", "Storm Scepter", "A one-handed critical caster base.", EquipmentSlot.Weapon, ItemRarity.Common, new Color(0.38f, 0.58f, 0.92f), critChance: 0.04f, baseFamily: "One-Handed Scepter", implicitText: "+4% Critical Chance", itemTags: new[] { "weapon", "caster" });
            AddItem("ritual_blade", "Ritual Blade", "A one-handed melee-spell base.", EquipmentSlot.Weapon, ItemRarity.Common, new Color(0.68f, 0.32f, 0.72f), spellPower: 0.06f, critDamage: 0.08f, baseFamily: "One-Handed Spellblade", implicitText: "+8% Critical Damage", itemTags: new[] { "weapon" });
            AddItem("archmage_staff", "Archmage Staff", "A two-handed high-power caster base.", EquipmentSlot.Weapon, ItemRarity.Common, new Color(0.44f, 0.38f, 0.82f), spellPower: 0.18f, mana: 18f, twoHanded: true, baseFamily: "Two-Handed Staff", implicitText: "+18% Spell Power", itemTags: new[] { "weapon", "caster", "trigger" });
            AddItem("circuit_staff", "Circuit Staff", "A two-handed trigger engine base.", EquipmentSlot.Weapon, ItemRarity.Common, new Color(0.78f, 0.22f, 0.72f), triggerEnergy: 28f, twoHanded: true, baseFamily: "Two-Handed Circuit", implicitText: "+28 Trigger Energy", itemTags: new[] { "weapon", "trigger", "caster" });

            AddItem("ward_focus", "Ward Focus", "An Offhand focused on Mana and Shield.", EquipmentSlot.Offhand, ItemRarity.Common, new Color(0.3f, 0.58f, 0.9f), mana: 22f, baseFamily: "Focus", implicitText: "+22 Maximum Mana", itemTags: new[] { "ward", "caster" });
            AddItem("iron_buckler", "Iron Buckler", "A defensive Offhand shield.", EquipmentSlot.Offhand, ItemRarity.Common, new Color(0.56f, 0.6f, 0.66f), health: 12f, armor: 8f, baseFamily: "Shield", implicitText: "+8 Armor", itemTags: new[] { "armor" });
            AddItem("trigger_grimoire", "Trigger Grimoire", "An Offhand written for recursive casting.", EquipmentSlot.Offhand, ItemRarity.Common, new Color(0.68f, 0.28f, 0.78f), triggerEnergy: 18f, baseFamily: "Grimoire", implicitText: "+18 Trigger Energy", itemTags: new[] { "trigger", "caster" });
            AddItem("prismatic_orb", "Prismatic Orb", "A balanced elemental Offhand.", EquipmentSlot.Offhand, ItemRarity.Common, new Color(0.5f, 0.72f, 0.88f), resistance: 0.04f, spellPower: 0.04f, baseFamily: "Ritual Orb", implicitText: "+4% Resistance", itemTags: new[] { "ward", "caster" });

            AddItem("storm_without_chance", "Storm Without Chance", "All prepared spells become Lightning and gain power, but critical strikes are disabled.", EquipmentSlot.Helmet, ItemRarity.Unique, new Color(0.3f, 0.62f, 1f), mutation: UniqueMutation.AllLightningNoCritical);
            AddItem("axis_of_six", "Axis of Six", "Adds a virtual central connector to each prepared spell board.", EquipmentSlot.Offhand, ItemRarity.Unique, new Color(0.8f, 0.25f, 1f), mutation: UniqueMutation.CentralVirtualConnector);
            AddItem("chalice_of_overflow", "Chalice of Overflow", "Excess healing becomes a reservoir of spell power.", EquipmentSlot.Chest, ItemRarity.Unique, new Color(0.85f, 0.15f, 0.35f), health: 34f, mutation: UniqueMutation.HealingReservoir);
            AddItem("orbiting_arsenal", "Orbiting Arsenal", "Projectile spells orbit before seeking the cursor aim point.", EquipmentSlot.RightShoulder, ItemRarity.Unique, new Color(0.62f, 0.42f, 1f), mutation: UniqueMutation.OrbitingArsenal);
            AddItem("reservoir_of_pale_moon", "Reservoir of the Pale Moon", "Trades maximum Health for Mana, Shield support and Trigger Energy.", EquipmentSlot.Offhand, ItemRarity.Unique, new Color(0.38f, 0.78f, 1f), mutation: UniqueMutation.ManaWardExchange);
            AddItem("dominion_engine", "Dominion Engine", "Triggered spells gain power while manual casts lose power.", EquipmentSlot.Weapon, ItemRarity.Unique, new Color(0.92f, 0.22f, 0.72f), twoHanded: true, mutation: UniqueMutation.TriggeredDominion);
            AddItem("convergence_glove", "Glove of Convergence", "Ailments spread when another ailment is consumed.", EquipmentSlot.LeftGlove, ItemRarity.Unique, new Color(0.38f, 0.92f, 0.48f), mutation: UniqueMutation.AilmentConvergence);
            AddItem("circuit_monolith", "Circuit Monolith", "A two-handed recursive spell engine with faster trigger recovery and added instability.", EquipmentSlot.Weapon, ItemRarity.Unique, new Color(0.78f, 0.18f, 0.82f), twoHanded: true, triggerEnergy: 35f, mutation: UniqueMutation.TwoHandedCircuit);
            AddItem("paired_oath", "Paired Oath", "Links both glove slots; wearing both grants spell power and cooldown recovery.", EquipmentSlot.RightGlove, ItemRarity.Unique, new Color(1f, 0.7f, 0.25f), mutation: UniqueMutation.TwinGloveLink);
        }

        private static void AddCore(string id, string name, string description, SpellDelivery delivery, SpellCastMethod castMethod, SpellElement element,
            Color color, float damage, float mana, float cooldown, float speed, float lifetime, float size, float radius, params string[] tags)
        {
            SpellCoreDefinition definition = ScriptableObject.CreateInstance<SpellCoreDefinition>();
            definition.hideFlags = HideFlags.HideAndDontSave;
            definition.id = id; definition.displayName = name; definition.description = description;
            definition.delivery = delivery; definition.castMethod = castMethod; definition.element = element; definition.color = color;
            definition.baseDamage = damage; definition.manaCost = mana; definition.cooldown = cooldown;
            definition.speed = speed; definition.lifetime = lifetime; definition.size = size; definition.radius = radius; definition.tags = tags;
            Cores[id] = definition;
        }

        private static void AddModifier(string id, string name, string shortText, string advanced, ModifierEffect effect,
            ModifierCategory category, ConnectorFamily connector, float magnitude, int input, int[] outputs, HexCoord[] shape,
            Color color, int instability, int preparationCost)
        {
            SpellModifierDefinition definition = ScriptableObject.CreateInstance<SpellModifierDefinition>();
            definition.hideFlags = HideFlags.HideAndDontSave;
            definition.id = id; definition.displayName = name; definition.shortDescription = shortText; definition.advancedDescription = advanced;
            definition.effect = effect; definition.category = category; definition.connectorFamily = connector; definition.magnitude = magnitude;
            definition.inputSide = input; definition.outputSides = outputs; definition.shape = shape; definition.uiColor = color;
            definition.instability = instability; definition.preparationCost = preparationCost;
            definition.capacityCost = Mathf.Clamp(preparationCost, 1, 6);
            Modifiers[id] = definition;
        }

        private static void ApplyV14SupportDesign()
        {
            ConfigureSupport("triple", "Multishot", 4, Shape(0, 0, 0, 1, 1, -1), new[] { 0, 1, 5 }, false, SpellDelivery.Projectile, SpellDelivery.Summon);
            ConfigureSupport("toxic", "Toxic Conversion", 2, Shape(0, 0, 0, 1), new[] { 0 }, false);
            ConfigureSupport("frost", "Frost Conversion", 2, Shape(0, 0, 0, 1), new[] { 0 }, false);
            ConfigureSupport("lightning", "Lightning Conversion", 2, Shape(0, 0, 0, 1), new[] { 0 }, false);
            ConfigureSupport("homing", "Homing", 2, Shape(0, 0, 0, 1), new[] { 0 }, false, SpellDelivery.Projectile, SpellDelivery.Summon);
            ConfigureSupport("arc", "Weaving Flight", 1, Shape(0, 0), new[] { 0 }, true, SpellDelivery.Projectile, SpellDelivery.Summon);
            ConfigureSupport("pierce", "Pierce", 3, Shape(0, 0, 0, 1, 0, -1), new[] { 0 }, true, SpellDelivery.Projectile, SpellDelivery.Hitscan, SpellDelivery.Beam);
            ConfigureSupport("explosion", "Greater Area", 3, Shape(0, 0, 0, 1, 1, -1), new[] { 0, 5 }, true);
            ConfigureSupport("chain", "Chain", 4, Shape(0, 0, 0, 1, 1,-1), new[] { 0, 1 }, true, SpellDelivery.Projectile, SpellDelivery.Hitscan, SpellDelivery.Beam);
            ConfigureSupport("repeat", "Repeat", 4, Shape(0, 0, 0, 1, 0, -1), new[] { 0 }, true);
            ConfigureSupport("nova_delivery", "Nova", 4, Shape(0, 0, 0, 1, 0, -1, 1, -1), new[] { 0 }, false);
            ConfigureSupport("efficient", "Efficient Casting", 1, Shape(0, 0), new[] { 0, 1 }, true);
            ConfigureSupport("unstable", "Forbidden Power", 6, Shape(0, 0, 0, 1, 0, -1, 1, -1), new[] { 0, 1, 5 }, true);
            ConfigureSupport("bounce", "Ricochet", 3, Shape(0, 0, 0, 1), new[] { 0 }, true, SpellDelivery.Projectile);
            ConfigureSupport("return", "Return", 2, Shape(0, 0, 0, 1, 1, -1), new[] { 0 }, false, SpellDelivery.Projectile);
            ConfigureSupport("orbit", "Orbit", 4, Shape(0, 0, 0, 1, 0, -1), new[] { 0 }, false, SpellDelivery.Projectile);
            ConfigureSupport("split_distance", "Split After Distance", 3, Shape(0, 0, 0, 1, 1, -1), new[] { 0, 5 }, true, SpellDelivery.Projectile);
            ConfigureSupport("split_hit", "Split on Hit", 3, Shape(0, 0, 0, 1, 1, -1), new[] { 0, 1 }, true, SpellDelivery.Projectile);
            ConfigureSupport("accelerate", "Acceleration", 1, Shape(0, 0), new[] { 0 }, true, SpellDelivery.Projectile);
            ConfigureSupport("decelerate", "Heavy Projectile", 2, Shape(0, 0, 0, 1), new[] { 0 }, true, SpellDelivery.Projectile);
            ConfigureSupport("delay", "Greater Delayed Blast", 3, Shape(0, 0, 0, 1, 0, -1), new[] { 0 }, true);
            ConfigureSupport("trail", "Damaging Trail", 3, Shape(0, 0, 0, 1, 1, -1), new[] { 0 }, true, SpellDelivery.Projectile, SpellDelivery.Movement);
            ConfigureSupport("zone", "Greater Damage Field", 3, Shape(0, 0, 0, 1, 0, -1), new[] { 0 }, true);
            ConfigureSupport("status_spread", "Status Spread", 3, Shape(0, 0, 0, 1, 1, -1), new[] { 0 }, false);
            ConfigureSupport("status_consume", "Consume Status", 4, Shape(0, 0, 0, 1, 0, -1), new[] { 0 }, false);
            ConfigureSupport("summon_payload", "Summon Familiar", 5, Shape(0, 0, 0, 1, 0, -1, 1, -1), new[] { 0 }, true);
            ConfigureSupport("reflect", "Reflect Projectiles", 4, Shape(0, 0, 0, 1, 0, -1), new[] { 0 }, false, SpellDelivery.Zone, SpellDelivery.Defensive);
            ConfigureSupport("overkill", "Store Overkill", 3, Shape(0, 0, 0, 1), new[] { 0 }, false);
            ConfigureSupport("blood_cost", "Blood Casting", 4, Shape(0, 0, 0, 1), new[] { 0 }, true);
            ConfigureSupport("mana_conversion", "Mana Furnace", 3, Shape(0, 0, 0, 1), new[] { 0 }, true);
            ConfigureSupport("wild", "Wild Junction", 1, Shape(0, 0), new[] { 0, 1, 5 }, true);
            ConfigureSupport("double_shot", "Double Shot", 2, Shape(0, 0, 0, 1), new[] { 0 }, true, SpellDelivery.Projectile, SpellDelivery.Summon);
            ConfigureSupport("large_area", "Larger Area", 2, Shape(0, 0, 0, 1), new[] { 0 }, true);
            ConfigureSupport("fast_projectile", "Faster Projectile", 1, Shape(0, 0), new[] { 0 }, true, SpellDelivery.Projectile);
            ConfigureSupport("slow_heavy", "Slow and Heavy", 2, Shape(0, 0, 0, 1), new[] { 0 }, true, SpellDelivery.Projectile);
            ConfigureSupport("delayed_blast", "Delayed Blast", 2, Shape(0, 0), new[] { 0 }, true);
            ConfigureSupport("damage_field", "Damage Field", 2, Shape(0, 0, 0, 1), new[] { 0 }, true);

            ConfigureSupport("cone", "Cone", 3, Shape(0, 0, 0, 1, 0, -1), new[] { 0, 1 }, false, SpellDelivery.Projectile, SpellDelivery.Summon);
            ConfigureSupport("converge", "Converge", 3, Shape(0, 0, 0, 1, 1, -1), new[] { 0 }, false, SpellDelivery.Projectile, SpellDelivery.Summon);
            ConfigureSupport("ring_cast", "Ring Cast", 4, Shape(0, 0, 0, 1, 0, -1, 1, -1), new[] { 0, 1, 5 }, false, SpellDelivery.Projectile, SpellDelivery.Summon);
            ConfigureSupport("fork", "Fork", 3, Shape(0, 0, 0, 1, 1, -1), new[] { 0, 1 }, true, SpellDelivery.Projectile);
            ConfigureSupport("meteor_delivery", "Meteor", 4, Shape(0, 0, 0, 1, 0, -1, 1, -1), new[] { 0 }, false, SpellDelivery.Projectile, SpellDelivery.Hitscan, SpellDelivery.Beam, SpellDelivery.Nova);
            ConfigureSupport("beam_delivery", "Beam", 4, Shape(0, 0, 0, 1, 0, -1, 1, -1), new[] { 0 }, false, SpellDelivery.Projectile, SpellDelivery.Hitscan);
            ConfigureSupport("burning", "Burning", 2, Shape(0, 0, 0, 1), new[] { 0 }, true);
            ConfigureSupport("shock", "Shock", 3, Shape(0, 0, 0, 1, 1, -1), new[] { 0 }, false);
            ConfigureSupport("chilling", "Chilling", 2, Shape(0, 0, 0, 1), new[] { 0 }, false);
            ConfigureSupport("close_range", "Close-Range Power", 3, Shape(0, 0, 0, 1, 0, -1), new[] { 0 }, false);
            ConfigureSupport("long_range", "Long-Range Power", 3, Shape(0, 0, 0, 1, 0, -1), new[] { 0 }, false);
            ConfigureSupport("barrier_cast", "Barrier on Cast", 3, Shape(0, 0, 0, 1, 1, -1), new[] { 0 }, false);

            SetVariantText("double_shot", "Basic Multishot: adds one projectile for a lower Capacity Cost and an easier shape.");
            SetVariantText("triple", "Advanced Multishot: adds two projectiles, costs more Capacity, and occupies a branching shape.");
            SetVariantText("large_area", "Basic area support: a smaller radius increase with a lower Capacity Cost.");
            SetVariantText("explosion", "Advanced area support: a larger radius increase with a wider branching shape.");
            SetVariantText("fast_projectile", "Basic speed support: moderate acceleration for one Capacity.");
            SetVariantText("accelerate", "Advanced speed support: stronger acceleration using the same compact shape.");
            SetVariantText("slow_heavy", "Basic heavy-projectile support: moderate slowdown and damage for two Capacity.");
            SetVariantText("decelerate", "Advanced heavy-projectile support: stronger deceleration for the same Capacity.");
            SetVariantText("delayed_blast", "Basic delayed blast: a short warning and compact one-hex shape.");
            SetVariantText("delay", "Advanced delayed blast: a longer warning, more damage, and a three-hex shape.");
            SetVariantText("damage_field", "Basic damage field: a short-lived area for two Capacity.");
            SetVariantText("zone", "Advanced damage field: a longer-lasting area with a larger shape and higher Capacity Cost.");

            foreach (string legacy in new[] { "trigger_slot2", "trigger_slot1", "trigger_slot3", "trigger_expire", "target_player", "target_enemy", "target_impact" })
            {
                SpellModifierDefinition definition;
                if (Modifiers.TryGetValue(legacy, out definition)) definition.availableAsSupport = false;
            }
        }

        private static void ConfigureSupport(string id, string functionalName, int capacityCost, HexCoord[] shape, int[] outputs,
            bool stackable, params SpellDelivery[] compatible)
        {
            SpellModifierDefinition definition;
            if (!Modifiers.TryGetValue(id, out definition)) return;
            definition.flavorName = definition.displayName;
            definition.displayName = functionalName;
            definition.capacityCost = Mathf.Clamp(capacityCost, 1, 6);
            definition.shape = shape;
            definition.inputSide = 3;
            definition.outputSides = outputs ?? new[] { 0 };
            definition.stackable = stackable;
            definition.compatibleDeliveries = compatible ?? new SpellDelivery[0];
            definition.connectorFamily = ConnectorFamily.Flow;
            definition.availableAsSupport = true;
        }

        private static void SetVariantText(string id, string relationship)
        {
            SpellModifierDefinition definition;
            if (!Modifiers.TryGetValue(id, out definition)) return;
            definition.shortDescription = definition.shortDescription + " " + relationship;
            definition.advancedDescription = definition.advancedDescription + " " + relationship;
        }

        private static HexCoord[] Shape(params int[] values)
        {
            List<HexCoord> result = new List<HexCoord>();
            for (int i = 0; i + 1 < values.Length; i += 2) result.Add(new HexCoord(values[i], values[i + 1]));
            return result.ToArray();
        }

        private static void AddItem(string id, string name, string description, EquipmentSlot slot, ItemRarity rarity, Color color,
            float health = 0f, float mana = 0f, float movement = 0f, float armor = 0f, float resistance = 0f,
            float critChance = 0f, float critDamage = 0f, float spellPower = 0f, float cooldown = 0f,
            float triggerEnergy = 0f, UniqueMutation mutation = UniqueMutation.None, bool twoHanded = false,
            string setId = null, string grantedCoreId = null,
            string baseFamily = null, string implicitText = null, string[] itemTags = null)
        {
            ItemDefinition definition = ScriptableObject.CreateInstance<ItemDefinition>();
            definition.hideFlags = HideFlags.HideAndDontSave;
            definition.id = id; definition.displayName = name; definition.description = description; definition.slot = slot;
            definition.rarity = rarity; definition.color = color; definition.health = health; definition.mana = mana;
            definition.movementSpeed = movement; definition.armor = armor; definition.resistance = resistance;
            definition.critChance = critChance; definition.critDamage = critDamage; definition.spellPower = spellPower;
            definition.cooldownReduction = cooldown; definition.triggerEnergy = triggerEnergy; definition.mutation = mutation;
            definition.twoHanded = twoHanded; definition.setId = setId; definition.grantedCoreId = grantedCoreId;
            definition.baseFamily = baseFamily; definition.implicitText = implicitText; definition.itemTags = itemTags ?? DefaultItemTags(slot);
            Items[id] = definition;
        }

        private static string[] DefaultItemTags(EquipmentSlot slot)
        {
            if (slot == EquipmentSlot.Weapon) return new[] { "weapon", "caster" };
            if (slot == EquipmentSlot.Offhand) return new[] { "ward", "caster" };
            if (slot == EquipmentSlot.Boots) return new[] { "mobile" };
            if (slot == EquipmentSlot.LeftGlove || slot == EquipmentSlot.RightGlove) return new[] { "caster", "weapon" };
            return new[] { "armor", "ward" };
        }
    }

    public static class ContentValidator
    {
        public static void ValidateAll()
        {
            HashSet<string> ids = new HashSet<string>();
            foreach (SpellCoreDefinition core in DemoCatalog.AllCores)
                if (core == null || string.IsNullOrEmpty(core.id) || !ids.Add("core:" + core.id)) Debug.LogError("Invalid or duplicate Spell Core definition.");
            foreach (SpellModifierDefinition modifier in DemoCatalog.AllModifiers)
            {
                if (modifier == null || string.IsNullOrEmpty(modifier.id) || !ids.Add("modifier:" + modifier.id)) Debug.LogError("Invalid or duplicate modifier definition.");
                if (modifier != null && (modifier.shape == null || modifier.shape.Length == 0)) Debug.LogError("Modifier has no shape: " + modifier.id);
                if (modifier != null && (modifier.inputSide < 0 || modifier.inputSide > 5)) Debug.LogError("Modifier has invalid input: " + modifier.id);
                if (modifier != null && (modifier.capacityCost < 1 || modifier.capacityCost > 6)) Debug.LogError("Support Rune has invalid Capacity Cost: " + modifier.id);
                if (modifier != null && string.IsNullOrEmpty(modifier.displayName)) Debug.LogError("Support Rune has no functional name: " + modifier.id);
            }
            foreach (RelicDefinition relic in MegaCatalog.AllRelics)
                if (DemoCatalog.GetCore(relic.sourceCoreId) == null) Debug.LogError("Relic has missing source core: " + relic.id);
            foreach (SpellCoreDefinition core in DemoCatalog.AllCores.Where(value => value.id != "void_maw"))
            {
                List<RelicDefinition> paths = MegaCatalog.RelicsForCore(core.id).ToList();
                if (paths.Count != 3) Debug.LogError(core.displayName + " must have exactly three Legendary evolutions; found " + paths.Count + ".");
                if (paths.Select(value => value.signature).Distinct().Count() != paths.Count)
                    Debug.LogError(core.displayName + " has reused Legendary signatures.");
            }
            foreach (ItemDefinition item in DemoCatalog.AllItems)
            {
                if (item == null || string.IsNullOrEmpty(item.id) || !ids.Add("item:" + item.id)) Debug.LogError("Invalid or duplicate item definition.");
                if (item != null && !string.IsNullOrEmpty(item.grantedCoreId) && DemoCatalog.GetCore(item.grantedCoreId) == null)
                    Debug.LogError("Item grants a missing Base Spell: " + item.id);
            }
            foreach (RoomTemplate room in MegaCatalog.AllRooms)
                if (room == null || string.IsNullOrEmpty(room.id) || !ids.Add("room:" + room.id)) Debug.LogError("Invalid or duplicate room definition.");
        }
    }
}
