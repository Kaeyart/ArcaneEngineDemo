using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ArcaneVisualQuality { Low, Medium, High }
    public enum SpellShapeFamily { Expanding, Crystalline, Branching, Organic, Hollow, Geometric }
    public enum SpellMotionFamily { Surging, Faceted, Staccato, Drifting, Imploding, Orbiting }
    public enum SpellImpactFamily { Bloom, Shatter, Fork, Splash, Collapse, Glyph }
    public enum SpellOverloadTier { Stable, Charged, Volatile, Critical }

    [Flags]
    public enum SupportVisualFlags
    {
        None = 0,
        Homing = 1 << 0,
        Arc = 1 << 1,
        Pierce = 1 << 2,
        Chain = 1 << 3,
        Bounce = 1 << 4,
        Return = 1 << 5,
        Orbit = 1 << 6,
        Accelerate = 1 << 7,
        Split = 1 << 8,
        Delay = 1 << 9,
        Persistent = 1 << 10,
        SpreadStatus = 1 << 11,
        ConsumeStatus = 1 << 12,
        Barrier = 1 << 13,
        Pull = 1 << 14,
        Reflect = 1 << 15,
        Repeat = 1 << 16,
        Summon = 1 << 17
    }

    [Serializable]
    public sealed class LegendaryVisualProfile
    {
        public RelicSignature signature;
        public string construction;
        public PrimitiveType authorityShape;
        public PrimitiveType accentShape;
        public int layerCount;
        public float scale;
        public bool usesOrbit;
        public bool usesTether;
        public bool usesGroundSeal;
    }

    [Serializable]
    public sealed class SpellVisualDescriptor
    {
        public SpellElement element;
        public SpellDelivery delivery;
        public ProjectilePattern pattern;
        public SpellShapeFamily shapeFamily;
        public SpellMotionFamily motionFamily;
        public SpellImpactFamily impactFamily;
        public SpellOverloadTier overloadTier;
        public SupportVisualFlags supportFlags;
        public RelicSignature relicSignature;
        public LegendaryVisualProfile legendaryProfile;
        public Color primary;
        public Color secondary;
        public PrimitiveType coreShape;
        public PrimitiveType shellShape;
        public float coreScale;
        public float shellScale;
        public float trailWidth;
        public float trailTime;
        public float radius;
        public float impactRadius;
        public float lifetime;
        public float speed;
        public float acceleration;
        public float homingStrength;
        public float arcAmount;
        public float detonationDelay;
        public float zoneDuration;
        public float trailDuration;
        public float instabilityMagnitude;
        public float barrierStrength;
        public float burnStrength;
        public float poisonStrength;
        public float shockStrength;
        public float chillStrength;
        public float freezeDuration;
        public float repeatDelay;
        public int pierceCount;
        public int chainCount;
        public int bounceCount;
        public int splitCount;
        public int orbitals;
        public int qualityCost;
        public int visualPriority;
        public bool triggered;
        public bool unstable;
        public bool homing;
        public bool arcing;
        public bool piercing;
        public bool chaining;
        public bool bouncing;
        public bool returning;
        public bool orbiting;
        public bool accelerating;
        public bool splitting;
        public bool delayed;
        public bool persistent;
        public bool statusSpread;
        public bool statusConsume;
        public bool barrier;
        public bool signature;
        public bool miniatureSun;
        public bool pullsEnemies;
        public bool reflectsProjectiles;
        public bool healthPowered;
        public int projectileCount;
        public int repeatCount;
        public int summonCount;
        public int modifierCount;
        public UniqueMutation[] equipmentMutations;
        public int incomingSpellLinks;
        public int outgoingSpellLinks;
        public SpellLinkCondition[] outgoingLinkConditions;
        public int visualRevision;
        public string functionalSummary;

        public SpellVisualDescriptor ForCast(CastRequest request)
        {
            SpellVisualDescriptor copy = (SpellVisualDescriptor)MemberwiseClone();
            copy.triggered = !request.manualCast;
            if (copy.triggered)
            {
                copy.coreScale *= 0.88f;
                copy.shellScale *= 0.88f;
                copy.trailWidth *= 0.82f;
                copy.qualityCost = Mathf.Max(1, copy.qualityCost - 1);
            }
            copy.functionalSummary = element + " " + delivery +
                (copy.triggered ? " · Triggered" : " · Manual") +
                (copy.homing ? " · Homing" : string.Empty) +
                (copy.chaining ? " · Chain" : string.Empty) +
                (copy.persistent ? " · Persistent" : string.Empty) +
                (relicSignature != RelicSignature.None ? " · " + relicSignature : string.Empty) +
                (equipmentMutations != null && equipmentMutations.Length > 0 ? " · " + equipmentMutations.Length + " Unique item mutation(s)" : string.Empty);
            return copy;
        }
    }

    public static class SpellVisualCompiler
    {
        private static int _revision;

        public static void Rebuild(CompiledSpell spell)
        {
            if (spell == null) return;
            spell.visualDescriptor = CompileTemplate(spell);
            spell.visualRevision = spell.visualDescriptor == null ? 0 : spell.visualDescriptor.visualRevision;
        }

        public static SpellVisualDescriptor Compile(CompiledSpell spell, CastRequest request)
        {
            if (spell == null) return null;
            if (spell.visualDescriptor == null) Rebuild(spell);
            return spell.visualDescriptor == null ? null : spell.visualDescriptor.ForCast(request);
        }

        private static SpellVisualDescriptor CompileTemplate(CompiledSpell spell)
        {
            SpellVisualDescriptor descriptor = new SpellVisualDescriptor();
            descriptor.visualRevision = ++_revision;
            descriptor.element = spell.element;
            descriptor.delivery = spell.delivery;
            descriptor.pattern = spell.projectilePattern;
            descriptor.primary = spell.primaryColor;
            descriptor.secondary = spell.accentColor;
            descriptor.radius = Mathf.Max(0.3f, spell.radius);
            descriptor.impactRadius = Mathf.Max(descriptor.radius, spell.explosionRadius);
            descriptor.lifetime = Mathf.Max(0.12f, spell.lifetime);
            descriptor.speed = spell.speed;
            descriptor.acceleration = spell.acceleration;
            descriptor.homingStrength = spell.homingStrength;
            descriptor.arcAmount = spell.arcAmount;
            descriptor.detonationDelay = spell.detonationDelay;
            descriptor.zoneDuration = spell.zoneDuration;
            descriptor.trailDuration = spell.trailDuration;
            descriptor.instabilityMagnitude = Mathf.Max(0f, spell.instability);
            descriptor.barrierStrength = Mathf.Max(0f, spell.barrierOnManualCast);
            descriptor.burnStrength = Mathf.Max(0f, spell.burnDamage);
            descriptor.poisonStrength = Mathf.Max(0f, spell.poisonDamage);
            descriptor.shockStrength = Mathf.Max(0f, spell.shockMagnitude);
            descriptor.chillStrength = Mathf.Max(0f, spell.chillMagnitude);
            descriptor.freezeDuration = Mathf.Max(0f, spell.freezeSeconds);
            descriptor.repeatDelay = Mathf.Max(0f, spell.repeatDelay);
            descriptor.pierceCount = Mathf.Max(0, spell.pierce);
            descriptor.chainCount = Mathf.Max(0, spell.chainTargets);
            descriptor.bounceCount = Mathf.Max(0, spell.bounce);
            descriptor.splitCount = Mathf.Max(0, spell.splitCount);
            descriptor.coreScale = Mathf.Clamp(spell.size, 0.2f, 4f);
            descriptor.shellScale = descriptor.coreScale * 1.32f;
            descriptor.trailWidth = descriptor.coreScale * (spell.pierce > 0 ? 0.34f : 0.56f);
            descriptor.trailTime = Mathf.Clamp(0.2f + spell.arcAmount * 0.15f + (spell.homingStrength > 0f ? 0.16f : 0f), 0.18f, 0.85f);
            descriptor.unstable = spell.instability > 0f;
            descriptor.homing = spell.homingStrength > 0f;
            descriptor.arcing = spell.arcAmount > 0f;
            descriptor.piercing = spell.pierce > 0;
            descriptor.chaining = spell.chainTargets > 0;
            descriptor.bouncing = spell.bounce > 0;
            descriptor.returning = spell.returnsToCaster;
            descriptor.orbiting = spell.orbitCaster;
            descriptor.accelerating = Mathf.Abs(spell.acceleration) > 0.01f;
            descriptor.splitting = spell.splitOnHit || spell.splitCount > 0;
            descriptor.delayed = spell.detonationDelay > 0f;
            descriptor.persistent = spell.zoneDuration > 0f || spell.trailDuration > 0f;
            descriptor.statusSpread = spell.spreadsStatus;
            descriptor.statusConsume = spell.consumesStatus;
            descriptor.barrier = spell.barrierOnManualCast > 0f || spell.delivery == SpellDelivery.Defensive;
            descriptor.signature = spell.relicSignature != RelicSignature.None || spell.isVoidMaw;
            descriptor.relicSignature = spell.relicSignature;
            descriptor.legendaryProfile = LegendaryVisualRegistry.Get(spell.relicSignature);
            descriptor.miniatureSun = spell.isMiniatureSun;
            descriptor.pullsEnemies = spell.pullsEnemies;
            descriptor.reflectsProjectiles = spell.reflectsProjectiles;
            descriptor.healthPowered = spell.manaFromHealth || spell.healthCost > 0f;
            descriptor.projectileCount = Mathf.Max(1, spell.projectileCount);
            descriptor.repeatCount = Mathf.Max(1, spell.repeatCount);
            descriptor.summonCount = Mathf.Max(0, spell.summonCount);
            descriptor.modifierCount = spell.activeModifierNames == null ? 0 : spell.activeModifierNames.Count;
            descriptor.equipmentMutations = spell.activeUniqueMutations == null ? new UniqueMutation[0] : spell.activeUniqueMutations.ToArray();
            descriptor.incomingSpellLinks = spell.incomingSpellLinks;
            descriptor.outgoingSpellLinks = spell.outgoingSpellLinks;
            descriptor.outgoingLinkConditions = spell.outgoingLinkConditions == null ? new SpellLinkCondition[0] : spell.outgoingLinkConditions.ToArray();
            descriptor.overloadTier = spell.instability >= 300f ? SpellOverloadTier.Critical : spell.instability >= 180f ? SpellOverloadTier.Volatile :
                spell.instability >= 75f ? SpellOverloadTier.Charged : SpellOverloadTier.Stable;
            descriptor.supportFlags = BuildSupportFlags(spell);
            descriptor.orbitals = descriptor.legendaryProfile != null && descriptor.legendaryProfile.usesOrbit ? Mathf.Clamp(descriptor.legendaryProfile.layerCount, 2, 5) :
                descriptor.signature ? 3 : descriptor.overloadTier == SpellOverloadTier.Critical ? 3 : descriptor.overloadTier == SpellOverloadTier.Volatile ? 2 : 0;
            descriptor.qualityCost = 1 + descriptor.orbitals + (descriptor.persistent ? 1 : 0) + (descriptor.signature ? 2 : 0) + Mathf.Min(2, descriptor.equipmentMutations.Length);
            descriptor.visualPriority = descriptor.signature ? 5 : descriptor.barrier ? 4 : descriptor.persistent || descriptor.triggered ? 3 : 2;

            switch (spell.element)
            {
                case SpellElement.Fire:
                    descriptor.shapeFamily = SpellShapeFamily.Expanding;
                    descriptor.motionFamily = SpellMotionFamily.Surging;
                    descriptor.impactFamily = SpellImpactFamily.Bloom;
                    descriptor.coreShape = PrimitiveType.Sphere;
                    descriptor.shellShape = PrimitiveType.Sphere;
                    break;
                case SpellElement.Frost:
                    descriptor.shapeFamily = SpellShapeFamily.Crystalline;
                    descriptor.motionFamily = SpellMotionFamily.Faceted;
                    descriptor.impactFamily = SpellImpactFamily.Shatter;
                    descriptor.coreShape = PrimitiveType.Cube;
                    descriptor.shellShape = PrimitiveType.Cylinder;
                    break;
                case SpellElement.Lightning:
                    descriptor.shapeFamily = SpellShapeFamily.Branching;
                    descriptor.motionFamily = SpellMotionFamily.Staccato;
                    descriptor.impactFamily = SpellImpactFamily.Fork;
                    descriptor.coreShape = PrimitiveType.Capsule;
                    descriptor.shellShape = PrimitiveType.Cube;
                    break;
                case SpellElement.Toxic:
                    descriptor.shapeFamily = SpellShapeFamily.Organic;
                    descriptor.motionFamily = SpellMotionFamily.Drifting;
                    descriptor.impactFamily = SpellImpactFamily.Splash;
                    descriptor.coreShape = PrimitiveType.Sphere;
                    descriptor.shellShape = PrimitiveType.Capsule;
                    break;
                case SpellElement.Void:
                    descriptor.shapeFamily = SpellShapeFamily.Hollow;
                    descriptor.motionFamily = SpellMotionFamily.Imploding;
                    descriptor.impactFamily = SpellImpactFamily.Collapse;
                    descriptor.coreShape = PrimitiveType.Cylinder;
                    descriptor.shellShape = PrimitiveType.Sphere;
                    break;
                default:
                    descriptor.shapeFamily = SpellShapeFamily.Geometric;
                    descriptor.motionFamily = SpellMotionFamily.Orbiting;
                    descriptor.impactFamily = SpellImpactFamily.Glyph;
                    descriptor.coreShape = PrimitiveType.Cube;
                    descriptor.shellShape = PrimitiveType.Cylinder;
                    break;
            }

            descriptor.functionalSummary = spell.element + " " + spell.delivery + " · compiled visual template";
            return descriptor;
        }

        private static SupportVisualFlags BuildSupportFlags(CompiledSpell spell)
        {
            SupportVisualFlags flags = SupportVisualFlags.None;
            if (spell.homingStrength > 0f) flags |= SupportVisualFlags.Homing;
            if (spell.arcAmount > 0f) flags |= SupportVisualFlags.Arc;
            if (spell.pierce > 0) flags |= SupportVisualFlags.Pierce;
            if (spell.chainTargets > 0) flags |= SupportVisualFlags.Chain;
            if (spell.bounce > 0) flags |= SupportVisualFlags.Bounce;
            if (spell.returnsToCaster) flags |= SupportVisualFlags.Return;
            if (spell.orbitCaster) flags |= SupportVisualFlags.Orbit;
            if (Mathf.Abs(spell.acceleration) > 0.01f) flags |= SupportVisualFlags.Accelerate;
            if (spell.splitOnHit || spell.splitCount > 0) flags |= SupportVisualFlags.Split;
            if (spell.detonationDelay > 0f) flags |= SupportVisualFlags.Delay;
            if (spell.zoneDuration > 0f || spell.trailDuration > 0f) flags |= SupportVisualFlags.Persistent;
            if (spell.spreadsStatus) flags |= SupportVisualFlags.SpreadStatus;
            if (spell.consumesStatus) flags |= SupportVisualFlags.ConsumeStatus;
            if (spell.barrierOnManualCast > 0f || spell.delivery == SpellDelivery.Defensive) flags |= SupportVisualFlags.Barrier;
            if (spell.pullsEnemies) flags |= SupportVisualFlags.Pull;
            if (spell.reflectsProjectiles) flags |= SupportVisualFlags.Reflect;
            if (spell.repeatCount > 1) flags |= SupportVisualFlags.Repeat;
            if (spell.summonCount > 0) flags |= SupportVisualFlags.Summon;
            return flags;
        }
    }

    public static class LegendaryVisualRegistry
    {
        private static readonly Dictionary<RelicSignature, LegendaryVisualProfile> Profiles = new Dictionary<RelicSignature, LegendaryVisualProfile>();

        public static LegendaryVisualProfile Get(RelicSignature signature)
        {
            if (signature == RelicSignature.None) return null;
            LegendaryVisualProfile profile;
            if (Profiles.TryGetValue(signature, out profile)) return profile;
            profile = Build(signature);
            Profiles[signature] = profile;
            return profile;
        }

        private static LegendaryVisualProfile Build(RelicSignature signature)
        {
            int visualSeed = Mathf.Abs((int)signature * 37 + 11);
            PrimitiveType[] shapes = { PrimitiveType.Sphere, PrimitiveType.Cube, PrimitiveType.Cylinder, PrimitiveType.Capsule };
            LegendaryVisualProfile value = new LegendaryVisualProfile { signature = signature, construction = signature.ToString() + " signature construction",
                authorityShape = shapes[visualSeed % shapes.Length], accentShape = shapes[(visualSeed / 3 + 1) % shapes.Length],
                layerCount = 3 + visualSeed % 3, scale = 1.15f + (visualSeed % 6) * 0.11f,
                usesOrbit = (visualSeed & 1) != 0, usesTether = (visualSeed & 2) != 0, usesGroundSeal = (visualSeed & 4) != 0 };
            switch (signature)
            {
                case RelicSignature.DyingSun: value.construction = "Gravitational miniature sun"; value.authorityShape = PrimitiveType.Sphere; value.layerCount = 5; value.scale = 1.9f; value.usesOrbit = true; value.usesGroundSeal = true; break;
                case RelicSignature.PhoenixSeed: value.construction = "Phoenix egg and wing embers"; value.authorityShape = PrimitiveType.Sphere; value.accentShape = PrimitiveType.Capsule; value.layerCount = 4; value.scale = 1.35f; value.usesOrbit = true; break;
                case RelicSignature.Hellstorm: value.construction = "Storm crown and impact lattice"; value.authorityShape = PrimitiveType.Cylinder; value.layerCount = 5; value.usesGroundSeal = true; break;
                case RelicSignature.AbsoluteZero: value.construction = "Zero-point crystal cage"; value.authorityShape = PrimitiveType.Cube; value.accentShape = PrimitiveType.Cube; value.layerCount = 5; value.scale = 1.55f; value.usesGroundSeal = true; break;
                case RelicSignature.FrostOrbit: value.construction = "Orbiting frost blades"; value.authorityShape = PrimitiveType.Capsule; value.accentShape = PrimitiveType.Cube; value.layerCount = 5; value.usesOrbit = true; break;
                case RelicSignature.ThunderJudgment: value.construction = "Judgment pillar and forked crown"; value.authorityShape = PrimitiveType.Capsule; value.layerCount = 5; value.scale = 1.6f; value.usesGroundSeal = true; break;
                case RelicSignature.LivingCircuit: value.construction = "Living circuit node network"; value.authorityShape = PrimitiveType.Sphere; value.accentShape = PrimitiveType.Capsule; value.layerCount = 4; value.usesTether = true; break;
                case RelicSignature.InfiniteBeam: value.construction = "Concentric beam aperture"; value.authorityShape = PrimitiveType.Cylinder; value.layerCount = 5; value.scale = 1.5f; break;
                case RelicSignature.PrismLance: value.construction = "Prismatic lance rails"; value.authorityShape = PrimitiveType.Cube; value.accentShape = PrimitiveType.Capsule; value.layerCount = 4; value.scale = 1.45f; break;
                case RelicSignature.FallingWorld: value.construction = "Descending world fragment"; value.authorityShape = PrimitiveType.Sphere; value.accentShape = PrimitiveType.Cube; value.layerCount = 5; value.scale = 2.1f; value.usesOrbit = true; value.usesGroundSeal = true; break;
                case RelicSignature.Starfall: value.construction = "Constellation descent"; value.authorityShape = PrimitiveType.Sphere; value.layerCount = 5; value.usesTether = true; value.usesGroundSeal = true; break;
                case RelicSignature.WispLegion: value.construction = "Wisp command crown"; value.authorityShape = PrimitiveType.Sphere; value.layerCount = 5; value.usesOrbit = true; value.usesTether = true; break;
                case RelicSignature.SoulTwin: value.construction = "Mirrored soul pair"; value.authorityShape = PrimitiveType.Sphere; value.layerCount = 2; value.usesOrbit = true; value.usesTether = true; break;
                case RelicSignature.RiftStep: value.construction = "Split rift doorway"; value.authorityShape = PrimitiveType.Cylinder; value.layerCount = 4; value.usesGroundSeal = true; break;
                case RelicSignature.Afterimage: value.construction = "Layered motion echoes"; value.authorityShape = PrimitiveType.Capsule; value.layerCount = 4; value.usesTether = true; break;
                case RelicSignature.WorldWall: value.construction = "World-wall bastion"; value.authorityShape = PrimitiveType.Cube; value.layerCount = 5; value.scale = 1.8f; value.usesGroundSeal = true; break;
                case RelicSignature.DevouringZone: value.construction = "Devouring concentric maw"; value.authorityShape = PrimitiveType.Cylinder; value.accentShape = PrimitiveType.Sphere; value.layerCount = 5; value.scale = 1.7f; value.usesOrbit = true; value.usesGroundSeal = true; break;
                case RelicSignature.SpellbladeDance: value.construction = "Orbiting spellblade fan"; value.authorityShape = PrimitiveType.Capsule; value.layerCount = 5; value.usesOrbit = true; break;
                case RelicSignature.ExecutionEdge: value.construction = "Execution edge guillotine"; value.authorityShape = PrimitiveType.Cube; value.layerCount = 3; value.scale = 1.65f; break;
                case RelicSignature.PerfectAegis: value.construction = "Nested aegis facets"; value.authorityShape = PrimitiveType.Cylinder; value.accentShape = PrimitiveType.Cube; value.layerCount = 5; value.scale = 1.55f; value.usesOrbit = true; break;
                case RelicSignature.RetributionWard: value.construction = "Retaliation thorns and ward ring"; value.authorityShape = PrimitiveType.Cylinder; value.accentShape = PrimitiveType.Capsule; value.layerCount = 5; value.usesOrbit = true; break;
            }
            return value;
        }
    }

    public static class ProceduralVisualRuntime
    {
        private static readonly Dictionary<PrimitiveType, Stack<GameObject>> PrimitivePools = new Dictionary<PrimitiveType, Stack<GameObject>>();
        private static readonly Stack<GameObject> LinePool = new Stack<GameObject>();
        private static readonly Stack<GameObject> TrailPool = new Stack<GameObject>();
        private static readonly HashSet<PooledVisualMarker> ActiveMarkers = new HashSet<PooledVisualMarker>();
        private static readonly HashSet<PooledVisualMarker> ActiveLightMarkers = new HashSet<PooledVisualMarker>();
        private static Transform _poolRoot;
        private static int _activeVisuals;
        private static int _activeLines;
        private static int _activeRings;
        private static int _activeBeams;
        private static int _activePrimitives;
        private static int _activeTrails;
        private static int _activeLights;
        private static int _activeDecals;
        private const int MaximumPooledPerShape = 64;
        private const int MaximumPooledLines = 96;
        private const int MaximumPooledTrails = 48;
        private const int MaximumActiveLights = 10;
        private static float _adaptiveScale = 1f;

        public static int ActiveVisuals { get { return _activeVisuals; } }
        public static int ActiveLines { get { return _activeLines; } }
        public static int ActiveRings { get { return _activeRings; } }
        public static int ActiveBeams { get { return _activeBeams; } }
        public static int ActivePrimitives { get { return _activePrimitives; } }
        public static int ActiveTrails { get { return _activeTrails; } }
        public static int ActiveLights { get { return _activeLights; } }
        public static int ActiveDecals { get { return _activeDecals; } }
        public static int ActiveVisualLimit { get { return Mathf.RoundToInt(VisualQualityBudget.Current.maxActiveVisuals * Mathf.Clamp(Density, 0.4f, 1f)); } }
        public static int ActiveLightLimit
        {
            get
            {
                AccessibilitySettings settings = ProfileManager.Current == null ? null : ProfileManager.Current.accessibility;
                int lightQuality = settings == null ? 1 : settings.dynamicLightQuality;
                return lightQuality <= 0 ? 0 : Mathf.Min(VisualQualityBudget.Current.maxLights, lightQuality == 1 ? 5 : MaximumActiveLights);
            }
        }
        public static int ActiveDecalLimit { get { return VisualQualityBudget.Current.maxDecals; } }
        public static float AdaptiveScale { get { return _adaptiveScale; } set { _adaptiveScale = Mathf.Clamp(value, 0.45f, 1f); } }
        public static int PooledVisuals
        {
            get
            {
                int count = LinePool.Count + TrailPool.Count;
                foreach (KeyValuePair<PrimitiveType, Stack<GameObject>> entry in PrimitivePools) count += entry.Value.Count;
                return count;
            }
        }

        public static ArcaneVisualQuality Quality
        {
            get
            {
                AccessibilitySettings settings = ProfileManager.Current == null ? null : ProfileManager.Current.accessibility;
                int value = settings == null ? 2 : settings.visualQuality;
                return (ArcaneVisualQuality)Mathf.Clamp(value, 0, 2);
            }
        }

        public static float Density
        {
            get
            {
                AccessibilitySettings settings = ProfileManager.Current == null ? null : ProfileManager.Current.accessibility;
                float configured = settings == null ? 1f : Mathf.Clamp(settings.spellEffectDensity <= 0f ? settings.effectDensity : settings.spellEffectDensity, 0.25f, 1f);
                return configured * _adaptiveScale;
            }
        }

        private static Transform PoolRoot
        {
            get
            {
                if (_poolRoot != null) return _poolRoot;
                GameObject root = new GameObject("Procedural Visual Pool");
                root.hideFlags = HideFlags.HideInHierarchy;
                UnityEngine.Object.DontDestroyOnLoad(root);
                _poolRoot = root.transform;
                return _poolRoot;
            }
        }

        public static GameObject AcquirePrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale, Color color, Transform parent = null, float emission = 0.8f)
        {
            if (_activeVisuals >= ActiveVisualLimit) return null;
            Stack<GameObject> pool;
            if (!PrimitivePools.TryGetValue(type, out pool)) { pool = new Stack<GameObject>(); PrimitivePools[type] = pool; }
            GameObject go = pool.Count > 0 ? pool.Pop() : CreatePrimitive(type);
            if (go == null) return null;
            _activeVisuals++;
            go.name = name;
            go.transform.SetParent(parent, false);
            if (parent == null) go.transform.position = position; else go.transform.position = position;
            go.transform.localScale = scale;
            go.transform.rotation = Quaternion.identity;
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = RuntimeVisuals.Material(color, emission);
            Light staleLight = go.GetComponent<Light>();
            if (staleLight != null) staleLight.enabled = false;
            go.SetActive(true);
            PooledVisualMarker marker = go.GetComponent<PooledVisualMarker>();
            marker.Acquired(type, false, false, false);
            ActiveMarkers.Add(marker);
            _activePrimitives++;
            return go;
        }

        public static LineRenderer AcquireLine(string name, Color color, int points, float width, Transform parent = null)
        {
            if (_activeVisuals >= ActiveVisualLimit) return null;
            GameObject go = LinePool.Count > 0 ? LinePool.Pop() : CreateLine();
            _activeVisuals++;
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            LineRenderer line = go.GetComponent<LineRenderer>();
            line.sharedMaterial = RuntimeVisuals.Material(color, 1.1f);
            line.positionCount = Mathf.Max(2, points);
            line.startWidth = width;
            line.endWidth = width * 0.35f;
            line.startColor = color;
            line.endColor = new Color(color.r, color.g, color.b, 0f);
            line.enabled = true;
            go.SetActive(true);
            PooledVisualMarker marker = go.GetComponent<PooledVisualMarker>();
            marker.Acquired(PrimitiveType.Sphere, true, false, false);
            ActiveMarkers.Add(marker);
            _activeLines++;
            return line;
        }

        public static TrailRenderer AcquireTrail(string name, Transform target, Color primary, Color secondary, float time, float width)
        {
            if (_activeVisuals >= ActiveVisualLimit) return null;
            GameObject go = TrailPool.Count > 0 ? TrailPool.Pop() : CreateTrail();
            _activeVisuals++; _activeTrails++;
            go.name = name;
            go.transform.SetParent(null, false);
            go.transform.position = target == null ? Vector3.zero : target.position;
            TrailRenderer trail = go.GetComponent<TrailRenderer>();
            trail.Clear();
            trail.sharedMaterial = RuntimeVisuals.Material(primary, 1f);
            trail.time = time;
            trail.startWidth = width;
            trail.endWidth = 0f;
            trail.startColor = primary;
            trail.endColor = new Color(secondary.r, secondary.g, secondary.b, 0f);
            trail.minVertexDistance = Quality == ArcaneVisualQuality.Low ? 0.28f : 0.12f;
            trail.emitting = true;
            go.SetActive(true);
            PooledVisualMarker marker = go.GetComponent<PooledVisualMarker>();
            marker.Acquired(PrimitiveType.Sphere, false, false, false, true);
            ActiveMarkers.Add(marker);
            PooledTrailFollower follower = go.GetComponent<PooledTrailFollower>();
            follower.Begin(target, trail.time);
            return trail;
        }

        public static void Release(GameObject go)
        {
            if (go == null) return;
            PooledVisualMarker marker = go.GetComponent<PooledVisualMarker>();
            if (marker == null) { UnityEngine.Object.Destroy(go); return; }
            if (!marker.InUse) return;
            bool wasLine = marker.IsLine;
            bool wasTrail = marker.IsTrail;
            bool wasLight = marker.CountedLight;
            bool wasDecal = marker.CountedDecal;
            marker.MarkReleased();
            ActiveMarkers.Remove(marker);
            ActiveLightMarkers.Remove(marker);
            _activeVisuals = Mathf.Max(0, _activeVisuals - 1);
            if (wasLine) _activeLines = Mathf.Max(0, _activeLines - 1);
            else if (wasTrail) _activeTrails = Mathf.Max(0, _activeTrails - 1);
            else _activePrimitives = Mathf.Max(0, _activePrimitives - 1);
            if (wasLight) _activeLights = Mathf.Max(0, _activeLights - 1);
            if (wasDecal) _activeDecals = Mathf.Max(0, _activeDecals - 1);
            if (marker.LineFamily == 1) _activeRings = Mathf.Max(0, _activeRings - 1);
            else if (marker.LineFamily == 2) _activeBeams = Mathf.Max(0, _activeBeams - 1);
            go.transform.SetParent(PoolRoot, false);
            go.SetActive(false);
            if (wasTrail)
            {
                TrailRenderer trail = go.GetComponent<TrailRenderer>(); if (trail != null) { trail.emitting = false; trail.Clear(); }
                if (TrailPool.Count < MaximumPooledTrails) TrailPool.Push(go); else UnityEngine.Object.Destroy(go);
                return;
            }
            if (marker.IsLine)
            {
                if (LinePool.Count < MaximumPooledLines) LinePool.Push(go); else UnityEngine.Object.Destroy(go);
                return;
            }
            Stack<GameObject> pool;
            if (!PrimitivePools.TryGetValue(marker.Shape, out pool)) { pool = new Stack<GameObject>(); PrimitivePools[marker.Shape] = pool; }
            if (pool.Count < MaximumPooledPerShape) pool.Push(go); else UnityEngine.Object.Destroy(go);
        }

        public static GameObject Burst(string name, Vector3 position, Color color, float radius, float lifetime, PrimitiveType shape = PrimitiveType.Sphere)
        {
            GameObject visual = AcquirePrimitive(shape, name, position + Vector3.up * 0.28f, Vector3.one * Mathf.Max(0.12f, radius), color, null, 1.2f);
            if (visual == null) return null;
            PooledVisualLifetime life = visual.GetComponent<PooledVisualLifetime>();
            if (life == null) life = visual.AddComponent<PooledVisualLifetime>();
            life.Begin(Mathf.Max(0.06f, lifetime), true, Mathf.Max(1.15f, 1f + radius * 0.6f));
            return visual;
        }

        public static LineRenderer Ring(string name, Vector3 position, Color color, float radius, float width, float lifetime, Transform parent = null, bool decal = false)
        {
            if (decal && _activeDecals >= VisualQualityBudget.Current.maxDecals) return null;
            int segments = VisualQualityBudget.Current.ringSegments;
            LineRenderer line = AcquireLine(name, color, segments + 1, width, parent);
            if (line == null) return null;
            line.useWorldSpace = parent == null;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                Vector3 point = new Vector3(Mathf.Cos(angle) * radius, 0.08f, Mathf.Sin(angle) * radius);
                line.SetPosition(i, parent == null ? position + point : point);
            }
            PooledVisualMarker marker = line.GetComponent<PooledVisualMarker>();
            marker.SetLineFamily(1); _activeRings++;
            if (decal) { marker.CountAsDecal(); _activeDecals++; }
            AccessibilitySettings settings = ProfileManager.Current == null ? null : ProfileManager.Current.accessibility;
            if (decal && settings != null) lifetime *= Mathf.Clamp(settings.decalDuration, 0.25f, 2f);
            PooledVisualLifetime life = line.GetComponent<PooledVisualLifetime>();
            if (life == null) life = line.gameObject.AddComponent<PooledVisualLifetime>();
            life.Begin(lifetime, false, 1f);
            return line;
        }

        public static LineRenderer Beam(string name, Vector3 from, Vector3 to, Color color, float width, float lifetime, bool jagged)
        {
            int points = jagged ? 6 : 2;
            LineRenderer line = AcquireLine(name, color, points, width);
            if (line == null) return null;
            PooledVisualMarker marker = line.GetComponent<PooledVisualMarker>();
            marker.SetLineFamily(2); _activeBeams++;
            line.useWorldSpace = true;
            Vector3 delta = to - from;
            Vector3 side = Vector3.Cross(Vector3.up, delta.normalized);
            for (int i = 0; i < points; i++)
            {
                float t = points == 1 ? 0f : i / (float)(points - 1);
                float offset = jagged && i > 0 && i < points - 1 ? ((i & 1) == 0 ? -1f : 1f) * 0.18f : 0f;
                line.SetPosition(i, Vector3.Lerp(from, to, t) + Vector3.up * 0.45f + side * offset);
            }
            PooledVisualLifetime life = line.GetComponent<PooledVisualLifetime>();
            if (life == null) life = line.gameObject.AddComponent<PooledVisualLifetime>();
            life.Begin(lifetime, false, 1f);
            return line;
        }

        public static Light LimitedLight(Vector3 position, Color color, float range, float intensity, float lifetime, Transform parent = null, int priority = 1)
        {
            AccessibilitySettings settings = ProfileManager.Current == null ? null : ProfileManager.Current.accessibility;
            if (settings != null && settings.reducedFlashes) return null;
            int lightQuality = settings == null ? 1 : settings.dynamicLightQuality;
            int cap = ActiveLightLimit;
            if (_activeLights >= cap)
            {
                PooledVisualMarker lowest = null;
                foreach (PooledVisualMarker active in ActiveLightMarkers)
                    if (active != null && active.InUse && active.CountedLight && (lowest == null || active.LightPriority < lowest.LightPriority)) lowest = active;
                if (lowest == null || lowest.LightPriority >= priority) return null;
                Release(lowest.gameObject);
            }
            GameObject go = AcquirePrimitive(PrimitiveType.Sphere, "Spell Light", position, Vector3.one * 0.01f, color, parent);
            if (go == null) return null;
            Renderer renderer = go.GetComponent<Renderer>(); if (renderer != null) renderer.enabled = false;
            Light light = go.GetComponent<Light>(); if (light == null) light = go.AddComponent<Light>();
            light.enabled = true; light.type = LightType.Point; light.color = color; light.range = range; light.intensity = intensity; light.shadows = LightShadows.None;
            PooledVisualMarker marker = go.GetComponent<PooledVisualMarker>(); marker.CountAsLight(priority); ActiveLightMarkers.Add(marker); _activeLights++;
            PooledVisualLifetime life = go.GetComponent<PooledVisualLifetime>(); if (life == null) life = go.AddComponent<PooledVisualLifetime>();
            life.Begin(lifetime, false, 1f);
            return light;
        }

        public static void ClearActive()
        {
            List<PooledVisualMarker> markers = new List<PooledVisualMarker>(ActiveMarkers);
            for (int i = 0; i < markers.Count; i++) if (markers[i] != null && markers[i].InUse) Release(markers[i].gameObject);
        }

        public static void NotifyMarkerDestroyed(PooledVisualMarker marker)
        {
            if (marker == null || !ActiveMarkers.Remove(marker)) return;
            ActiveLightMarkers.Remove(marker);
            _activeVisuals = Mathf.Max(0, _activeVisuals - 1);
            if (marker.IsLine) _activeLines = Mathf.Max(0, _activeLines - 1);
            else if (marker.IsTrail) _activeTrails = Mathf.Max(0, _activeTrails - 1);
            else _activePrimitives = Mathf.Max(0, _activePrimitives - 1);
            if (marker.CountedLight) _activeLights = Mathf.Max(0, _activeLights - 1);
            if (marker.CountedDecal) _activeDecals = Mathf.Max(0, _activeDecals - 1);
            if (marker.LineFamily == 1) _activeRings = Mathf.Max(0, _activeRings - 1);
            else if (marker.LineFamily == 2) _activeBeams = Mathf.Max(0, _activeBeams - 1);
        }

        private static GameObject CreatePrimitive(PrimitiveType type)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            Collider collider = go.GetComponent<Collider>(); if (collider != null) { collider.enabled = false; UnityEngine.Object.Destroy(collider); }
            go.AddComponent<PooledVisualMarker>();
            go.SetActive(false);
            go.transform.SetParent(PoolRoot, false);
            return go;
        }

        private static GameObject CreateLine()
        {
            GameObject go = new GameObject("Pooled Line");
            LineRenderer line = go.AddComponent<LineRenderer>();
            line.alignment = LineAlignment.View;
            line.textureMode = LineTextureMode.Stretch;
            go.AddComponent<PooledVisualMarker>();
            go.SetActive(false);
            go.transform.SetParent(PoolRoot, false);
            return go;
        }

        private static GameObject CreateTrail()
        {
            GameObject go = new GameObject("Pooled Trail");
            TrailRenderer trail = go.AddComponent<TrailRenderer>();
            trail.alignment = LineAlignment.View;
            trail.textureMode = LineTextureMode.Stretch;
            trail.autodestruct = false;
            go.AddComponent<PooledTrailFollower>();
            go.AddComponent<PooledVisualMarker>();
            go.SetActive(false);
            go.transform.SetParent(PoolRoot, false);
            return go;
        }
    }

    public sealed class PooledVisualMarker : MonoBehaviour
    {
        public PrimitiveType Shape { get; private set; }
        public bool IsLine { get; private set; }
        public bool IsTrail { get; private set; }
        public bool InUse { get; private set; }
        public bool CountedLight { get; private set; }
        public bool CountedDecal { get; private set; }
        public int LightPriority { get; private set; }
        public int LineFamily { get; private set; }
        public void Acquired(PrimitiveType shape, bool line, bool light, bool decal, bool trail = false) { Shape = shape; IsLine = line; IsTrail = trail; InUse = true; CountedLight = light; CountedDecal = decal; LineFamily = 0; }
        public void SetLineFamily(int family) { LineFamily = Mathf.Clamp(family, 0, 2); }
        public void CountAsLight(int priority) { CountedLight = true; LightPriority = priority; }
        public void CountAsDecal() { CountedDecal = true; }
        public void MarkReleased() { InUse = false; CountedLight = false; CountedDecal = false; LightPriority = 0; }
        private void OnDestroy() { if (InUse) ProceduralVisualRuntime.NotifyMarkerDestroyed(this); }
    }

    public sealed class PooledVisualLifetime : MonoBehaviour
    {
        private float _remaining;
        private bool _expand;
        private float _growth;
        public void Begin(float lifetime, bool expand, float growth) { _remaining = lifetime; _expand = expand; _growth = growth; enabled = true; }
        private void Update()
        {
            _remaining -= Time.deltaTime;
            if (_expand) transform.localScale *= 1f + Time.deltaTime * _growth;
            if (_remaining <= 0f) { enabled = false; ProceduralVisualRuntime.Release(gameObject); }
        }
    }

    public sealed class PooledTrailFollower : MonoBehaviour
    {
        private Transform _target;
        private float _fadeTime;
        private float _remaining;
        private TrailRenderer _trail;
        public void Begin(Transform target, float fadeTime)
        {
            _target = target; _fadeTime = Mathf.Max(0.08f, fadeTime); _remaining = _fadeTime;
            if (_trail == null) _trail = GetComponent<TrailRenderer>();
            enabled = true;
        }
        public void Detach() { _target = null; _remaining = _fadeTime; if (_trail != null) _trail.emitting = false; }
        public bool Follows(Transform target) { return _target == target; }
        private void LateUpdate()
        {
            if (_target != null) { transform.position = _target.position; _remaining = _fadeTime; return; }
            _remaining -= Time.deltaTime;
            if (_remaining > 0f) return;
            enabled = false;
            ProceduralVisualRuntime.Release(gameObject);
        }
    }

    public sealed class SpellVisualAttachment : MonoBehaviour
    {
        private static int _activeCount;
        private bool _counted;
        private SpellVisualDescriptor _descriptor;
        private readonly List<GameObject> _pooledParts = new List<GameObject>();
        private readonly List<Transform> _orbitals = new List<Transform>();
        private float _phase;
        private TrailRenderer _trail;
        private Vector3 _lastPosition;
        public static int ActiveCount { get { return _activeCount; } }

        public static SpellVisualAttachment Attach(GameObject host, CompiledSpell spell, CastRequest request)
        {
            if (host == null || spell == null) return null;
            SpellVisualAttachment attachment = host.AddComponent<SpellVisualAttachment>();
            attachment._counted = true;
            _activeCount++;
            attachment._descriptor = SpellVisualCompiler.Compile(spell, request);
            attachment.Build();
            return attachment;
        }

        private void Build()
        {
            if (_descriptor == null) return;
            Renderer hostRenderer = GetComponent<Renderer>();
            if (hostRenderer != null) hostRenderer.enabled = false;
            GameObject core = ProceduralVisualRuntime.AcquirePrimitive(_descriptor.coreShape, _descriptor.element + " Core", transform.position,
                Vector3.one * _descriptor.coreScale, _descriptor.primary, transform, 1.05f);
            AddPart(core, Vector3.zero);

            if (VisualQualityBudget.Current.projectileShells || _descriptor.signature)
            {
                Vector3 shellScale = _descriptor.shapeFamily == SpellShapeFamily.Branching ? new Vector3(0.35f, 1.65f, 0.35f) * _descriptor.shellScale :
                    _descriptor.shapeFamily == SpellShapeFamily.Crystalline ? new Vector3(0.72f, 1.28f, 0.72f) * _descriptor.shellScale : Vector3.one * _descriptor.shellScale;
                GameObject shell = ProceduralVisualRuntime.AcquirePrimitive(_descriptor.shellShape, _descriptor.shapeFamily + " Shell", transform.position,
                    shellScale, Color.Lerp(_descriptor.primary, _descriptor.secondary, 0.45f), transform, 0.75f);
                AddPart(shell, Vector3.zero);
            }

            int count = Mathf.Min(_descriptor.orbitals, ProceduralVisualRuntime.Quality == ArcaneVisualQuality.Low ? 1 : 3);
            for (int i = 0; i < count; i++)
            {
                GameObject orbital = ProceduralVisualRuntime.AcquirePrimitive(PrimitiveType.Sphere, "Spell Accent", transform.position,
                    Vector3.one * _descriptor.coreScale * 0.18f, _descriptor.secondary, transform, 1.25f);
                if (orbital == null) continue;
                _pooledParts.Add(orbital); _orbitals.Add(orbital.transform);
            }

            _trail = ProceduralVisualRuntime.AcquireTrail(_descriptor.element + " " + _descriptor.delivery + " Trail", transform,
                _descriptor.primary, _descriptor.secondary, _descriptor.trailTime, _descriptor.trailWidth);
            _lastPosition = transform.position;
        }

        private void AddPart(GameObject go, Vector3 localPosition)
        {
            if (go == null) return;
            go.transform.localPosition = localPosition;
            _pooledParts.Add(go);
        }

        private void Update()
        {
            if (_descriptor == null) return;
            _phase += Time.deltaTime * (_descriptor.unstable ? 8f : 3.5f);
            if (_trail != null)
            {
                PooledVisualMarker trailMarker = _trail.GetComponent<PooledVisualMarker>();
                PooledTrailFollower follower = _trail.GetComponent<PooledTrailFollower>();
                if (trailMarker == null || !trailMarker.InUse || follower == null || !follower.Follows(transform)) _trail = null;
            }
            if (_trail != null && _descriptor.accelerating && Time.deltaTime > 0.0001f)
            {
                float speed = Vector3.Distance(transform.position, _lastPosition) / Time.deltaTime;
                _trail.time = Mathf.Clamp(_descriptor.trailTime * Mathf.Lerp(0.65f, 1.45f, Mathf.InverseLerp(2f, 18f, speed)), 0.12f, 1.1f);
            }
            _lastPosition = transform.position;
            for (int i = 0; i < _orbitals.Count; i++)
            {
                if (_orbitals[i] == null || _orbitals[i].parent != transform) continue;
                PooledVisualMarker marker = _orbitals[i].GetComponent<PooledVisualMarker>();
                if (marker == null || !marker.InUse) continue;
                float angle = _phase + i / (float)Mathf.Max(1, _orbitals.Count) * Mathf.PI * 2f;
                float wobble = _descriptor.unstable ? Mathf.Sin(_phase * 2.7f + i) * 0.18f : 0f;
                _orbitals[i].localPosition = new Vector3(Mathf.Cos(angle), wobble, Mathf.Sin(angle)) * (_descriptor.coreScale * 0.92f);
            }
        }

        private void OnDestroy()
        {
            if (_counted) { _activeCount = Mathf.Max(0, _activeCount - 1); _counted = false; }
            for (int i = 0; i < _pooledParts.Count; i++)
            {
                GameObject part = _pooledParts[i];
                if (!OwnsPart(part)) continue;
                part.transform.SetParent(null, true);
                ProceduralVisualRuntime.Release(part);
            }
            _pooledParts.Clear(); _orbitals.Clear();
            if (_trail != null)
            {
                PooledTrailFollower follower = _trail.GetComponent<PooledTrailFollower>();
                PooledVisualMarker marker = _trail.GetComponent<PooledVisualMarker>();
                if (marker != null && marker.InUse && follower != null && follower.Follows(transform)) follower.Detach();
                _trail = null;
            }
        }

        private bool OwnsPart(GameObject part)
        {
            if (part == null || part.transform.parent != transform) return false;
            PooledVisualMarker marker = part.GetComponent<PooledVisualMarker>();
            return marker != null && marker.InUse;
        }
    }

    public static class SpellVisualEvents
    {
        public static void Cast(CompiledSpell spell, CastRequest request)
        {
            SpellVisualDescriptor descriptor = SpellVisualCompiler.Compile(spell, request);
            if (descriptor == null) return;
            SpellDeliveryVisuals.BeginCast(spell, request);
            if (descriptor.barrier && GameWorld.Instance != null && GameWorld.Instance.Player != null)
                ProceduralVisualRuntime.Ring("Barrier on Cast", GameWorld.Instance.Player.transform.position, descriptor.secondary, 1.15f, 0.12f, 0.32f, GameWorld.Instance.Player.transform);
        }

        public static void Impact(CompiledSpell spell, CastRequest request, Vector3 position, bool critical)
        {
            SpellVisualDescriptor descriptor = SpellVisualCompiler.Compile(spell, request);
            if (descriptor == null) return;
            SpellDeliveryVisuals.Impact(spell, request, position, critical);
            if (descriptor.delayed || descriptor.persistent)
                ProceduralVisualRuntime.Ring("Impact Residue", position, descriptor.secondary, Mathf.Max(0.55f, spell.explosionRadius), 0.05f, 0.45f, null, true);
            if (ProceduralVisualRuntime.Quality == ArcaneVisualQuality.High)
                ProceduralVisualRuntime.LimitedLight(position + Vector3.up * 0.5f, descriptor.primary, Mathf.Clamp(spell.size * 3f, 2f, 6f),
                    critical ? 1.8f : 1.1f, 0.12f, null, descriptor.signature ? 4 : critical ? 3 : descriptor.visualPriority);
        }

        public static void Expire(CompiledSpell spell, CastRequest request, Vector3 position)
        {
            SpellVisualDescriptor descriptor = SpellVisualCompiler.Compile(spell, request);
            if (descriptor == null) return;
            SpellDeliveryVisuals.Resolve(spell, request, position);
        }

        public static void DirectionChange(CompiledSpell spell, Vector3 position, bool returning)
        {
            if (spell == null) return;
            ProceduralVisualRuntime.Burst(returning ? "Return Turn" : "Bounce Turn", position, returning ? spell.accentColor : spell.primaryColor, 0.28f, 0.1f, PrimitiveType.Cube);
        }

        public static void LinkActivation(CompiledSpell source, CompiledSpell destination, Vector3 position, TriggerMoment moment = TriggerMoment.OnCast)
        {
            if (source == null || destination == null || GameWorld.Instance == null || GameWorld.Instance.Player == null) return;
            SpellDeliveryVisuals.TriggerLink(source, destination, position, moment);
        }

        public static void StatusSpread(CompiledSpell spell, Vector3 from, Vector3 to)
        {
            if (spell == null) return;
            ProceduralVisualRuntime.Beam("Status Spread", from, to, spell.accentColor, 0.055f, 0.14f, spell.element == SpellElement.Lightning);
        }

        public static void StatusConsume(CompiledSpell spell, Vector3 position)
        {
            if (spell == null) return;
            ProceduralVisualRuntime.Ring("Status Consumed", position, Color.Lerp(spell.primaryColor, Color.white, 0.42f), Mathf.Max(0.6f, spell.size), 0.1f, 0.2f);
        }
    }

    public sealed class MeteorVisualCue : MonoBehaviour
    {
        private GameObject _body;
        private Vector3 _target;
        private float _duration;
        private float _age;
        public void Initialize(CompiledSpell spell, Vector3 target, float duration)
        {
            _target = target; _duration = Mathf.Max(0.12f, duration);
            PrimitiveType shape = spell.element == SpellElement.Frost || spell.element == SpellElement.Arcane ? PrimitiveType.Cube : PrimitiveType.Sphere;
            _body = ProceduralVisualRuntime.AcquirePrimitive(shape, spell.element + " Meteor Body", target + Vector3.up * 10f,
                Vector3.one * Mathf.Clamp(spell.size * 1.2f, 0.5f, 3f), spell.primaryColor, transform, 1.2f);
        }
        private void Update()
        {
            _age += Time.deltaTime;
            if (OwnsBody())
            {
                float t = Mathf.Clamp01(_age / _duration);
                _body.transform.position = Vector3.Lerp(_target + Vector3.up * 10f, _target + Vector3.up * 0.55f, t * t);
                _body.transform.Rotate(85f * Time.deltaTime, 120f * Time.deltaTime, 55f * Time.deltaTime);
            }
        }
        private void OnDestroy() { if (OwnsBody()) ProceduralVisualRuntime.Release(_body); _body = null; }
        private bool OwnsBody()
        {
            if (_body == null || _body.transform.parent != transform) return false;
            PooledVisualMarker marker = _body.GetComponent<PooledVisualMarker>();
            return marker != null && marker.InUse;
        }
    }

    public sealed class PlayerBarrierVisual : MonoBehaviour
    {
        private LineRenderer _ring;
        private float _expires;
        public static void Ensure(PlayerController player, Color color, float lifetime)
        {
            if (player == null) return;
            PlayerBarrierVisual visual = player.GetComponent<PlayerBarrierVisual>();
            if (visual == null) visual = player.gameObject.AddComponent<PlayerBarrierVisual>();
            visual._expires = Mathf.Max(visual._expires, Time.time + Mathf.Max(0.2f, lifetime));
            if (visual._ring == null)
                visual._ring = ProceduralVisualRuntime.Ring("Player Barrier State", player.transform.position, color, 1.1f, 0.12f, Mathf.Max(0.2f, lifetime), player.transform);
        }
        private void Update()
        {
            PlayerController player = GetComponent<PlayerController>();
            if (player == null || player.Ward <= 0.01f || Time.time >= _expires) { Destroy(this); return; }
            if (!OwnsRing()) { _ring = null; return; }
            _ring.transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * 4f) * 0.04f);
        }
        private void OnDestroy()
        {
            if (OwnsRing()) ProceduralVisualRuntime.Release(_ring.gameObject);
            _ring = null;
        }
        private bool OwnsRing()
        {
            if (_ring == null || _ring.transform.parent != transform) return false;
            PooledVisualMarker marker = _ring.GetComponent<PooledVisualMarker>();
            return marker != null && marker.InUse;
        }
    }
}
