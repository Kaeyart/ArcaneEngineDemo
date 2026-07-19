using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum SpellSlot
    {
        Slot1 = 0,
        Primary = 0,
        Slot2 = 1,
        Secondary = 1,
        Slot3 = 2,
        ItemGranted = 3
    }

    public enum SpellDelivery { Projectile, Nova, Hitscan, Beam, Meteor, Summon, Movement, Zone, Melee, Defensive }
    public enum SpellCastMethod { Instant, Charged, Channeled, Delayed, Movement }
    public enum SpellElement { Fire, Frost, Lightning, Toxic, Void, Arcane }
    public enum ProjectilePattern { Standard, Cone, Converge, Ring }
    public enum ConnectorFamily { Flow, Payload, Trigger, Element, Power, Wild }
    public enum ModifierCategory { Pattern, Movement, Payload, Element, Collision, Trigger, Spawn, Targeting, Resource, Instability }
    public enum TargetContext { AimPoint, Player, NearestEnemy, Impact, BetweenTargets }

    public enum ModifierEffect
    {
        MultiProjectile,
        ToxicConversion,
        FrostConversion,
        LightningConversion,
        Homing,
        ArcFlight,
        Pierce,
        BiggerExplosion,
        ChainLightning,
        RepeatPulse,
        TriggerSlot2OnHit,
        TriggerSlot1OnKill,
        TriggerSlot3OnCast,
        TriggerSlot1OnExpire,
        NovaDelivery,
        EfficientCasting,
        UnstablePower,
        Bounce,
        Return,
        Orbit,
        SplitAfterDistance,
        SplitOnHit,
        Accelerate,
        Decelerate,
        DelayedDetonation,
        PersistentTrail,
        DamageZone,
        StatusSpread,
        StatusConsume,
        SummonPayload,
        ReflectProjectiles,
        StoreExcessDamage,
        HealthSacrifice,
        ManaConversion,
        TargetPlayer,
        TargetEnemy,
        TargetImpact,
        WildConnector,
        ConePattern,
        ConvergingProjectiles,
        RingPattern,
        ForkOnHit,
        MeteorDelivery,
        BeamDelivery,
        Burning,
        Shock,
        Chill,
        Bleeding,
        Cursing,
        Weaken,
        Vulnerability,
        CloseRangePower,
        LongRangePower,
        BarrierOnCast
    }

    public enum TriggerMoment
    {
        OnCast,
        OnHit,
        OnCriticalHit,
        OnKill,
        OnStatusApplied,
        OnStatusConsumed,
        OnFreeze,
        OnShatter,
        OnBounce,
        OnProjectileImpact,
        OnProjectileExpire,
        OnExpire,
        OnDodge,
        OnBlock,
        OnShieldBreak,
        OnDamageTaken,
        OnPeriodic,
        OnDistance,
        OnChannelTick
    }

    [Serializable]
    public struct HexCoord : IEquatable<HexCoord>
    {
        public int q;
        public int r;

        public HexCoord(int qValue, int rValue)
        {
            q = qValue;
            r = rValue;
        }

        public static HexCoord operator +(HexCoord a, HexCoord b) { return new HexCoord(a.q + b.q, a.r + b.r); }

        public static readonly HexCoord[] Directions =
        {
            new HexCoord(1, 0), new HexCoord(0, 1), new HexCoord(-1, 1),
            new HexCoord(-1, 0), new HexCoord(0, -1), new HexCoord(1, -1)
        };

        public static HexCoord Rotate(HexCoord value, int steps)
        {
            steps = ((steps % 6) + 6) % 6;
            HexCoord result = value;
            for (int i = 0; i < steps; i++) result = new HexCoord(-result.r, result.q + result.r);
            return result;
        }

        public int DistanceFromOrigin() { return (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(q + r)) / 2; }
        public bool Equals(HexCoord other) { return q == other.q && r == other.r; }
        public override bool Equals(object obj) { return obj is HexCoord && Equals((HexCoord)obj); }
        public override int GetHashCode() { unchecked { return (q * 397) ^ r; } }
        public override string ToString() { return "(" + q + ", " + r + ")"; }
    }

    public sealed class SpellCoreDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public SpellDelivery delivery;
        public SpellCastMethod castMethod;
        public SpellElement element;
        public Color color = Color.white;
        public float baseDamage;
        public float manaCost;
        public float cooldown;
        public float speed;
        public float lifetime;
        public float size;
        public float radius;
        public string[] tags = new string[0];
    }

    public sealed class SpellModifierDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public string flavorName;
        [TextArea] public string shortDescription;
        [TextArea] public string advancedDescription;
        public ModifierEffect effect;
        public ModifierCategory category;
        public ConnectorFamily connectorFamily = ConnectorFamily.Flow;
        public float magnitude = 1f;
        public int inputSide = 3;
        public int[] outputSides = { 0 };
        public HexCoord[] shape = { new HexCoord(0, 0) };
        public Color uiColor = Color.white;
        public int instability;
        public int preparationCost = 1;
        public int capacityCost = 1;
        public bool stackable = true;
        public bool availableAsSupport = true;
        public SpellDelivery[] compatibleDeliveries = new SpellDelivery[0];

        public string FullDisplayName
        {
            get { return string.IsNullOrEmpty(flavorName) || flavorName == displayName ? displayName : displayName + "\n" + flavorName; }
        }

        public bool IsCompatible(SpellCoreDefinition core)
        {
            return core != null && (compatibleDeliveries == null || compatibleDeliveries.Length == 0 ||
                System.Array.IndexOf(compatibleDeliveries, core.delivery) >= 0);
        }
    }

    [Serializable]
    public sealed class TriggerSpec
    {
        public string sourceId;
        public TriggerMoment moment;
        public SpellSlot linkedSlot;
        public TargetContext targetContext = TargetContext.Impact;
        public float energyCost;
        public float inheritedPower;
        public int maxActivationsPerEvent;
        public float internalCooldown;

        public TriggerSpec Clone() { return (TriggerSpec)MemberwiseClone(); }
    }

    public sealed class CompiledSpell
    {
        public string coreId;
        public string relicId;
        public RelicSignature relicSignature;
        public string displayName;
        public SpellSlot slot;
        public SpellDelivery delivery;
        public SpellCastMethod castMethod;
        public SpellElement element;
        public Color primaryColor;
        public Color accentColor;
        public float damage;
        public float manaCost;
        public float healthCost;
        public float cooldown;
        public float speed;
        public float lifetime;
        public float size;
        public float radius;
        public ProjectilePattern projectilePattern;
        public int projectileCount = 1;
        public float spreadDegrees;
        public float homingStrength;
        public float arcAmount;
        public int pierce;
        public int bounce;
        public int splitCount;
        public float splitDistance;
        public bool splitOnHit;
        public bool returnsToCaster;
        public bool orbitCaster;
        public float acceleration;
        public int chainTargets;
        public float poisonDamage;
        public float poisonDuration;
        public float burnDamage;
        public float burnDuration;
        public float shockMagnitude;
        public float shockDuration;
        public float chillMagnitude;
        public float chillDuration;
        public float bleedDamage;
        public float bleedDuration;
        public float curseMagnitude;
        public float curseDuration;
        public float weakenMagnitude;
        public float weakenDuration;
        public float vulnerabilityMagnitude;
        public float vulnerabilityDuration;
        public float freezeSeconds;
        public int repeatCount = 1;
        public float repeatDelay = 0.35f;
        public float explosionRadius;
        public float detonationDelay;
        public float zoneDuration;
        public float trailDuration;
        public int summonCount;
        public float instability;
        public TargetContext targetContext = TargetContext.AimPoint;
        public bool spreadsStatus;
        public bool consumesStatus;
        public bool reflectsProjectiles;
        public bool storesExcessDamage;
        public bool manaFromHealth;
        public float closeRangeMultiplier = 1f;
        public float longRangeMultiplier = 1f;
        public float barrierOnManualCast;
        public bool isMiniatureSun;
        public bool pullsEnemies;
        public bool isVoidMaw;
        public SpellVisualDescriptor visualDescriptor;
        public int visualRevision;
        public int incomingSpellLinks;
        public int outgoingSpellLinks;
        public SpellLinkCondition[] outgoingLinkConditions = new SpellLinkCondition[0];
        public readonly List<TriggerSpec> triggers = new List<TriggerSpec>();
        public readonly List<string> activeModifierNames = new List<string>();
        public readonly List<string> warnings = new List<string>();
        public readonly List<string> executionLayers = new List<string>();
        public readonly List<UniqueMutation> activeUniqueMutations = new List<UniqueMutation>();

        public string CompactSummary()
        {
            string deliveryText;
            if (delivery == SpellDelivery.Projectile) deliveryText = projectileCount + " projectile" + (projectileCount == 1 ? "" : "s");
            else if (delivery == SpellDelivery.Nova) deliveryText = repeatCount + " pulse" + (repeatCount == 1 ? "" : "s");
            else deliveryText = delivery.ToString();
            return displayName + " | " + deliveryText + " | " + Mathf.RoundToInt(damage) + " " + element +
                   " | " + manaCost.ToString("0") + " mana | " + cooldown.ToString("0.0") + "s";
        }

        public string ExpertSummary()
        {
            return "Direct " + damage.ToString("0.0") + " · area " + explosionRadius.ToString("0.0") + "m · status " +
                   (poisonDamage > 0f ? poisonDamage.ToString("0.0") + "/s poison" : burnDamage > 0f ? burnDamage.ToString("0.0") + "/s burning" :
                    freezeSeconds > 0f ? freezeSeconds.ToString("0.0") + "s freeze" : shockMagnitude > 0f ? (shockMagnitude * 100f).ToString("0") + "% shock" :
                    chillMagnitude > 0f ? (chillMagnitude * 100f).ToString("0") + "% chill" : "none") +
                   " · Unique/Legendary trigger cost " + triggers.SumSafe(t => t.energyCost).ToString("0") + " · Overload Chance " +
                   (Mathf.Clamp01(instability / 450f) * 100f).ToString("0.0") + "%";
        }
    }

    public static class SpellCollectionExtensions
    {
        public static float SumSafe<T>(this IEnumerable<T> values, Func<T, float> selector)
        {
            float total = 0f;
            if (values == null) return total;
            foreach (T value in values) total += selector(value);
            return total;
        }
    }
}
