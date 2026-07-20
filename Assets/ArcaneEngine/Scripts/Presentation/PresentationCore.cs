using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ArcaneEngine
{
    public enum SpellPresentationEventType
    {
        None,
        CastStarted,
        CastReleased,
        ProjectileSpawned,
        ProjectileRedirected,
        ProjectileReturned,
        ProjectileSplit,
        ProjectileImpacted,
        BeamConnected,
        ZoneCreated,
        ZonePulsed,
        FamiliarCreated,
        MovementDeparture,
        MovementArrival,
        AilmentApplied,
        AilmentBuildupChanged,
        MajorAilmentActivated,
        ReactionAssemblyStarted,
        ReactionSignatureUpgraded,
        ReactionResolved,
        ReactionDeathTriggered,
        ReactionMechanic,
        FieldCreated,
        FieldMerged,
        FieldPulsed,
        FieldExpired,
        StatusSpread,
        StatusConsumed,
        BarrierChanged,
        LinkActivated,
        EffectExpired
    }

    public enum PresentationPriority
    {
        Decorative = 0,
        Normal = 1,
        Important = 2,
        Critical = 3,
        Reserved = 4
    }

    public enum PresentationQuality
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum BloodPresentationMode
    {
        Hidden = 0,
        Reduced = 1,
        Full = 2
    }

    public enum RuneVisualOperatorKind
    {
        None,
        Homing,
        Arc,
        Pierce,
        Chain,
        Bounce,
        Return,
        Orbit,
        Accelerate,
        Split,
        Delay,
        Persistent,
        SpreadStatus,
        ConsumeStatus,
        Barrier,
        Pull,
        Reflect,
        Repeat,
        Summon,
        Trigger,
        Movement
    }

    public enum ElementVisualRole
    {
        None,
        PrimarySilhouette,
        Catalyst,
        Motion,
        Impact,
        Secondary,
        Residue
    }

    public enum VisualChassisKind
    {
        Unknown,
        Projectile,
        Beam,
        Zone,
        Nova,
        Meteor,
        Melee,
        Familiar,
        Movement
    }

    [Serializable]
    public sealed class RuneVisualOperatorSpec
    {
        public RuneVisualOperatorKind kind;
        public float magnitude;
        public int count;
        public float duration;
        public int order;
        public string source;
        public string fallback;

        public override string ToString()
        {
            return kind +
                   (count > 1 ? " ×" + count : string.Empty) +
                   (magnitude > 0.001f ? " @" + magnitude.ToString("0.##") : string.Empty);
        }
    }

    [Serializable]
    public sealed class ElementVisualLayer
    {
        public ReactionElement element;
        public ElementVisualRole role;
        public float intensity;
        public Color primary;
        public Color secondary;
        public string motionLanguage;
        public string impactLanguage;
        public string residueLanguage;
    }

    [Serializable]
    public sealed class BoardTopologySummary
    {
        public int runeCount;
        public int branchCount;
        public int maxHexDistance;
        public int clockwiseBias;
        public bool approximatelySymmetric;
        public Vector2 dominantDirection;
        public string orderedRuneIds = string.Empty;
        public string topologyText = string.Empty;
    }

    [Serializable]
    public sealed class GeneratedVisualRecipe
    {
        public const int CurrentSchemaVersion = 20000;

        public int schemaVersion = CurrentSchemaVersion;
        public int stableSeed;
        public int configurationHash;
        public string coreId;
        public string displayName;
        public string deliveryName;
        public VisualChassisKind chassis;
        public ReactionElement signature;
        public ReactionElement catalyst;
        public ReactionElement primaryElement;
        public ReactionElement motionElement;
        public ReactionElement impactElement;
        public ReactionElement residueElement;
        public ReactionTier reactionTier;
        public string reactionName;
        public string reactionGraphId;
        public Color primaryColor;
        public Color secondaryColor;
        public float size;
        public float speed;
        public float radius;
        public float duration;
        public int projectileCount;
        public int visualPriority;
        public BoardTopologySummary topology;
        public readonly List<ElementVisualLayer> elementLayers = new List<ElementVisualLayer>();
        public readonly List<RuneVisualOperatorSpec> operators = new List<RuneVisualOperatorSpec>();
        public readonly List<string> requiredLayers = new List<string>();
        public readonly List<string> decorativeLayers = new List<string>();
        public readonly List<string> fallbackLayers = new List<string>();

        public string ElementText
        {
            get { return ElementalReactionCodex.SignatureText(signature); }
        }

        public string OperatorText
        {
            get
            {
                List<string> values = new List<string>();
                for (int i = 0; i < operators.Count; i++)
                    values.Add(operators[i].kind.ToString());

                return values.Count == 0 ? "None" : string.Join(" → ", values);
            }
        }

        public string DebugSummary
        {
            get
            {
                return
                    "Core: " + coreId + "\n" +
                    "Delivery: " + deliveryName + "\n" +
                    "Elements: " + ElementText + "\n" +
                    "Catalyst: " + catalyst + "\n" +
                    "Primary silhouette: " + primaryElement + "\n" +
                    "Motion: " + motionElement + "\n" +
                    "Impact: " + impactElement + "\n" +
                    "Residue: " + residueElement + "\n" +
                    "Operators: " + OperatorText + "\n" +
                    "Board: " + (topology == null ? "Unavailable" : topology.topologyText) + "\n" +
                    "Seed: 0x" + stableSeed.ToString("X8") + "\n" +
                    "Schema: " + schemaVersion;
            }
        }
    }

    public struct SpellPresentationEvent
    {
        public SpellPresentationEventType type;
        public PresentationPriority priority;
        public CompiledSpell spell;
        public GeneratedVisualRecipe recipe;
        public Transform source;
        public Transform target;
        public GameObject host;
        public Vector3 position;
        public Vector3 secondaryPosition;
        public Vector3 direction;
        public ReactionElement signature;
        public ReactionElement catalyst;
        public ReactionTier tier;
        public ReactionMechanicType mechanicType;
        public string mechanicId;
        public int stackCount;
        public int threshold;
        public int count;
        public float normalizedProgress;
        public float radius;
        public float duration;
        public float intensity;
        public float delay;
        public bool critical;
        public bool death;
        public long originCastId;
        public int propagationGeneration;
        public ReactionSourceKind22 reactionSourceKind;
        public ReactionFieldAuthority22 fieldAuthority;
        public bool coalesced;
        public float time;

        public string Summary
        {
            get
            {
                string name = recipe == null
                    ? spell == null ? string.Empty : spell.displayName
                    : recipe.displayName;

                return type +
                       (string.IsNullOrEmpty(name) ? string.Empty : " · " + name) +
                       (signature == ReactionElement.None ? string.Empty : " · " + ElementalReactionCodex.SignatureText(signature)) +
                       (string.IsNullOrEmpty(mechanicId) ? string.Empty : " · " + mechanicId);
            }
        }
    }

    public static class SpellPresentationBus
    {
        private const int HistoryLimit = 80;
        private static readonly Queue<SpellPresentationEvent> History =
            new Queue<SpellPresentationEvent>(HistoryLimit);

        public static event Action<SpellPresentationEvent> Published;

        public static int PublishedCount { get; private set; }
        public static SpellPresentationEvent LastEvent { get; private set; }

        public static void Emit(SpellPresentationEvent presentationEvent)
        {
            if (!ReactionPresentationCoalescer22.TryAccept(ref presentationEvent))
                return;

            presentationEvent.time = Time.unscaledTime;
            LastEvent = presentationEvent;
            PublishedCount++;

            while (History.Count >= HistoryLimit)
                History.Dequeue();

            History.Enqueue(presentationEvent);

            Action<SpellPresentationEvent> handler = Published;
            if (handler != null)
                handler(presentationEvent);
        }

        public static SpellPresentationEvent[] Snapshot()
        {
            return History.ToArray();
        }

        public static void Clear()
        {
            History.Clear();
            PublishedCount = 0;
            LastEvent = default(SpellPresentationEvent);
        }
    }

    public sealed class ElementVisualProfile2
    {
        public ReactionElement element;
        public string displayName;
        public Color primary;
        public Color secondary;
        public Color smoke;
        public PrimitiveType authorityShape;
        public PrimitiveType accentShape;
        public float turbulence;
        public float upwardBias;
        public float pulseFrequency;
        public float particleLifetime;
        public float particleSpeed;
        public float particleSize;
        public string motionLanguage;
        public string impactLanguage;
        public string residueLanguage;
        public int shapeSymbol;
    }

    public static class ElementVisualProfileRegistry2
    {
        private static readonly Dictionary<ReactionElement, ElementVisualProfile2> Profiles =
            Build();

        public static IEnumerable<ElementVisualProfile2> All
        {
            get { return Profiles.Values; }
        }

        public static ElementVisualProfile2 Get(ReactionElement element)
        {
            ElementVisualProfile2 profile;

            if (Profiles.TryGetValue(element, out profile))
                return profile;

            return Profiles[ReactionElement.None];
        }

        private static Dictionary<ReactionElement, ElementVisualProfile2> Build()
        {
            Dictionary<ReactionElement, ElementVisualProfile2> result =
                new Dictionary<ReactionElement, ElementVisualProfile2>();

            result[ReactionElement.None] = Profile(
                ReactionElement.None,
                "Arcane",
                new Color(0.5f, 0.75f, 1f),
                new Color(0.82f, 0.95f, 1f),
                new Color(0.24f, 0.38f, 0.55f, 0.45f),
                PrimitiveType.Sphere,
                PrimitiveType.Cube,
                0.35f,
                0.08f,
                2.4f,
                0.65f,
                1.3f,
                0.15f,
                "geometric orbit",
                "glyph pulse",
                "arcane seal",
                0);

            result[ReactionElement.Fire] = Profile(
                ReactionElement.Fire,
                "Fire",
                new Color(1f, 0.2f, 0.015f),
                new Color(1f, 0.72f, 0.08f),
                new Color(0.16f, 0.09f, 0.06f, 0.58f),
                PrimitiveType.Sphere,
                PrimitiveType.Capsule,
                0.8f,
                0.75f,
                5.5f,
                0.52f,
                2.4f,
                0.17f,
                "upward flame turbulence",
                "expanding combustion bloom",
                "embers, smoke and burning ground",
                1);

            result[ReactionElement.Cold] = Profile(
                ReactionElement.Cold,
                "Cold",
                new Color(0.22f, 0.82f, 1f),
                new Color(0.82f, 0.97f, 1f),
                new Color(0.58f, 0.88f, 1f, 0.34f),
                PrimitiveType.Cube,
                PrimitiveType.Cube,
                0.18f,
                0.05f,
                1.8f,
                0.8f,
                0.72f,
                0.13f,
                "contracting crystalline drift",
                "faceted shatter",
                "frost, mist and fracture lines",
                2);

            result[ReactionElement.Lightning] = Profile(
                ReactionElement.Lightning,
                "Lightning",
                new Color(1f, 0.91f, 0.15f),
                new Color(0.72f, 0.92f, 1f),
                new Color(0.35f, 0.38f, 0.32f, 0.25f),
                PrimitiveType.Capsule,
                PrimitiveType.Cube,
                1.25f,
                0.15f,
                11f,
                0.18f,
                4.5f,
                0.08f,
                "staccato branching conduction",
                "forked discharge",
                "conductor nodes and surface arcs",
                3);

            result[ReactionElement.Physical] = Profile(
                ReactionElement.Physical,
                "Physical",
                new Color(0.72f, 0.62f, 0.46f),
                new Color(0.92f, 0.86f, 0.72f),
                new Color(0.34f, 0.29f, 0.22f, 0.5f),
                PrimitiveType.Cube,
                PrimitiveType.Capsule,
                0.55f,
                0.18f,
                2.2f,
                0.48f,
                2.8f,
                0.16f,
                "ballistic material force",
                "pressure shock and fracture",
                "dust, debris and ground cracks",
                4);

            result[ReactionElement.Blood] = Profile(
                ReactionElement.Blood,
                "Blood",
                new Color(0.68f, 0.015f, 0.035f),
                new Color(0.98f, 0.12f, 0.1f),
                new Color(0.22f, 0.01f, 0.02f, 0.5f),
                PrimitiveType.Sphere,
                PrimitiveType.Capsule,
                0.5f,
                -0.28f,
                3.6f,
                0.7f,
                1.8f,
                0.12f,
                "organic ribbon flow",
                "arterial rupture",
                "droplets, wounds and pools",
                5);

            result[ReactionElement.Toxic] = Profile(
                ReactionElement.Toxic,
                "Toxic",
                new Color(0.36f, 0.92f, 0.08f),
                new Color(0.82f, 1f, 0.22f),
                new Color(0.19f, 0.31f, 0.08f, 0.52f),
                PrimitiveType.Sphere,
                PrimitiveType.Sphere,
                0.7f,
                0.22f,
                2.1f,
                1.45f,
                0.65f,
                0.22f,
                "drifting contaminated volume",
                "viscous splash",
                "vapor, spores and contamination",
                6);

            result[ReactionElement.Void] = Profile(
                ReactionElement.Void,
                "Void",
                new Color(0.45f, 0.025f, 0.78f),
                new Color(0.86f, 0.24f, 1f),
                new Color(0.08f, 0.01f, 0.13f, 0.64f),
                PrimitiveType.Cylinder,
                PrimitiveType.Sphere,
                0.9f,
                -0.05f,
                3.2f,
                0.88f,
                -1.45f,
                0.18f,
                "inward spatial collapse",
                "implosion and delayed release",
                "distortion rings and spatial tears",
                7);

            return result;
        }

        private static ElementVisualProfile2 Profile(
            ReactionElement element,
            string displayName,
            Color primary,
            Color secondary,
            Color smoke,
            PrimitiveType authorityShape,
            PrimitiveType accentShape,
            float turbulence,
            float upwardBias,
            float pulseFrequency,
            float particleLifetime,
            float particleSpeed,
            float particleSize,
            string motionLanguage,
            string impactLanguage,
            string residueLanguage,
            int shapeSymbol)
        {
            return new ElementVisualProfile2
            {
                element = element,
                displayName = displayName,
                primary = primary,
                secondary = secondary,
                smoke = smoke,
                authorityShape = authorityShape,
                accentShape = accentShape,
                turbulence = turbulence,
                upwardBias = upwardBias,
                pulseFrequency = pulseFrequency,
                particleLifetime = particleLifetime,
                particleSpeed = particleSpeed,
                particleSize = particleSize,
                motionLanguage = motionLanguage,
                impactLanguage = impactLanguage,
                residueLanguage = residueLanguage,
                shapeSymbol = shapeSymbol
            };
        }
    }

    public static class Patch200PresentationSettings
    {
        private const string Prefix = "ArcaneEngine.Patch200.Presentation.";

        public static PresentationQuality Quality
        {
            get
            {
                return (PresentationQuality)Mathf.Clamp(
                    PlayerPrefs.GetInt(Prefix + "Quality", 2),
                    0,
                    2);
            }
            set
            {
                PlayerPrefs.SetInt(Prefix + "Quality", (int)value);
                PlayerPrefs.Save();
            }
        }

        public static float Density
        {
            get
            {
                return Mathf.Clamp(
                    PlayerPrefs.GetFloat(Prefix + "Density", 1f),
                    0.25f,
                    1f);
            }
            set
            {
                PlayerPrefs.SetFloat(Prefix + "Density", Mathf.Clamp(value, 0.25f, 1f));
                PlayerPrefs.Save();
            }
        }

        public static bool ReducedMotion
        {
            get { return PlayerPrefs.GetInt(Prefix + "ReducedMotion", 0) != 0; }
            set
            {
                PlayerPrefs.SetInt(Prefix + "ReducedMotion", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public static float FlashIntensity
        {
            get { return Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "FlashIntensity", 0.75f)); }
            set
            {
                PlayerPrefs.SetFloat(Prefix + "FlashIntensity", Mathf.Clamp01(value));
                PlayerPrefs.Save();
            }
        }

        public static float CameraFeedback
        {
            get { return Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "CameraFeedback", 0.7f)); }
            set
            {
                PlayerPrefs.SetFloat(Prefix + "CameraFeedback", Mathf.Clamp01(value));
                PlayerPrefs.Save();
            }
        }

        public static float EffectIntensity
        {
            get { return Mathf.Clamp(PlayerPrefs.GetFloat(Prefix + "EffectIntensity", 1f), 0.35f, 1.25f); }
            set
            {
                PlayerPrefs.SetFloat(Prefix + "EffectIntensity", Mathf.Clamp(value, 0.35f, 1.25f));
                PlayerPrefs.Save();
            }
        }

        public static BloodPresentationMode BloodMode
        {
            get
            {
                return (BloodPresentationMode)Mathf.Clamp(
                    PlayerPrefs.GetInt(Prefix + "BloodMode", 2),
                    0,
                    2);
            }
            set
            {
                PlayerPrefs.SetInt(Prefix + "BloodMode", (int)value);
                PlayerPrefs.Save();
            }
        }

        public static bool Distortion
        {
            get { return PlayerPrefs.GetInt(Prefix + "Distortion", 1) != 0; }
            set
            {
                PlayerPrefs.SetInt(Prefix + "Distortion", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public static bool DebugOverlay
        {
            get { return PlayerPrefs.GetInt(Prefix + "DebugOverlay", 0) != 0; }
            set
            {
                PlayerPrefs.SetInt(Prefix + "DebugOverlay", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public static int MaxParticles
        {
            get
            {
                switch (Quality)
                {
                    case PresentationQuality.Low: return 850;
                    case PresentationQuality.Medium: return 1850;
                    default: return 3400;
                }
            }
        }

        public static int MaxParticleSystems
        {
            get
            {
                switch (Quality)
                {
                    case PresentationQuality.Low: return 36;
                    case PresentationQuality.Medium: return 72;
                    default: return 118;
                }
            }
        }

        public static int MaxResidues
        {
            get
            {
                switch (Quality)
                {
                    case PresentationQuality.Low: return 16;
                    case PresentationQuality.Medium: return 28;
                    default: return 44;
                }
            }
        }

        public static int MaxAttachments
        {
            get
            {
                switch (Quality)
                {
                    case PresentationQuality.Low: return 24;
                    case PresentationQuality.Medium: return 52;
                    default: return 84;
                }
            }
        }
    }

    public static class ProceduralSpellVisualCompiler2
    {
        private sealed class RecipeHolder
        {
            public GeneratedVisualRecipe value;
        }

        private static readonly ConditionalWeakTable<CompiledSpell, RecipeHolder> BySpell =
            new ConditionalWeakTable<CompiledSpell, RecipeHolder>();

        private static readonly Dictionary<int, GeneratedVisualRecipe> ByHash =
            new Dictionary<int, GeneratedVisualRecipe>();

        public static int CachedRecipeCount
        {
            get { return ByHash.Count; }
        }

        public static GeneratedVisualRecipe Compile(
            CompiledSpell spell,
            SpellBoard board)
        {
            if (spell == null)
                return null;

            ReactionElement signature = ResolveSignature(spell);
            ReactionElement catalyst = ResolveCatalyst(spell, signature);
            BoardTopologySummary topology = ResolveTopology(board);
            int hash = ComputeConfigurationHash(spell, signature, catalyst, topology);

            GeneratedVisualRecipe cached;

            if (ByHash.TryGetValue(hash, out cached))
            {
                Bind(spell, cached);
                return cached;
            }

            GeneratedVisualRecipe recipe = BuildRecipe(
                spell,
                signature,
                catalyst,
                topology,
                hash);

            ByHash[hash] = recipe;
            Bind(spell, recipe);
            return recipe;
        }

        public static GeneratedVisualRecipe Get(CompiledSpell spell)
        {
            if (spell == null)
                return null;

            RecipeHolder holder;

            if (BySpell.TryGetValue(spell, out holder) && holder.value != null)
                return holder.value;

            return Compile(spell, null);
        }

        public static void ClearCache()
        {
            ByHash.Clear();
        }

        private static void Bind(
            CompiledSpell spell,
            GeneratedVisualRecipe recipe)
        {
            BySpell.Remove(spell);
            BySpell.Add(spell, new RecipeHolder { value = recipe });
        }

        private static GeneratedVisualRecipe BuildRecipe(
            CompiledSpell spell,
            ReactionElement signature,
            ReactionElement catalyst,
            BoardTopologySummary topology,
            int hash)
        {
            GeneratedVisualRecipe recipe = new GeneratedVisualRecipe();
            recipe.configurationHash = hash;
            recipe.stableSeed = hash;
            recipe.coreId = spell.coreId;
            recipe.displayName = spell.displayName;
            recipe.deliveryName = Convert.ToString(Read(spell, "delivery", "Unknown"));
            recipe.chassis = ResolveChassis(recipe.deliveryName);
            recipe.signature = signature;
            recipe.catalyst = catalyst;
            recipe.primaryColor = spell.primaryColor;
            recipe.secondaryColor = spell.accentColor;
            recipe.size = Mathf.Max(0.15f, ReadFloat(spell, "size", 1f));
            recipe.speed = ReadFloat(spell, "speed", 0f);
            recipe.radius = Mathf.Max(
                0.4f,
                Mathf.Max(
                    ReadFloat(spell, "radius", 1f),
                    ReadFloat(spell, "explosionRadius", 0f)));
            recipe.duration = Mathf.Max(
                0.1f,
                Mathf.Max(
                    ReadFloat(spell, "lifetime", 0.8f),
                    ReadFloat(spell, "zoneDuration", 0f)));
            recipe.projectileCount = Mathf.Max(1, ReadInt(spell, "projectileCount", 1));
            recipe.topology = topology;

            int elementCount = ElementalReactionCodex.CountBits(signature);
            recipe.reactionTier = elementCount >= 2
                ? (ReactionTier)Mathf.Clamp(elementCount, 2, 7)
                : ReactionTier.Fusion;

            ElementalReactionDefinition reaction =
                elementCount >= 2 ? ElementalReactionCodex.Get(signature) : null;

            recipe.reactionName = reaction == null
                ? string.Empty
                : reaction.displayName;

            ReactionMechanicPlan plan =
                reaction == null
                    ? null
                    : ElementalReactionMechanicCodex.Get(signature);

            recipe.reactionGraphId = plan == null
                ? string.Empty
                : plan.graphId;

            AssignElementRoles(recipe);
            BuildOperators(recipe, spell);
            BuildLayerRequirements(recipe, spell);

            int tierPriority = elementCount >= 6 ? 4 : elementCount >= 4 ? 3 : elementCount >= 2 ? 2 : 1;
            recipe.visualPriority = Mathf.Max(tierPriority, ReadInt(spell, "visualPriority", 1));

            return recipe;
        }

        private static void AssignElementRoles(GeneratedVisualRecipe recipe)
        {
            ReactionElement signature = recipe.signature;
            ReactionElement catalyst = recipe.catalyst;
            int count = ElementalReactionCodex.CountBits(signature);

            ReactionElement primary = catalyst;

            if (count >= 3 && ElementalReactionCodex.Contains(signature, ReactionElement.Toxic))
                primary = ReactionElement.Toxic;
            else if (count >= 4 && ElementalReactionCodex.Contains(signature, ReactionElement.Void))
                primary = ReactionElement.Void;
            else if (ElementalReactionCodex.Contains(signature, ReactionElement.Fire))
                primary = ReactionElement.Fire;

            ReactionElement motion =
                ElementalReactionCodex.Contains(signature, ReactionElement.Lightning)
                    ? ReactionElement.Lightning
                    : ElementalReactionCodex.Contains(signature, ReactionElement.Void)
                        ? ReactionElement.Void
                        : catalyst;

            ReactionElement impact =
                ElementalReactionCodex.Contains(signature, ReactionElement.Physical)
                    ? ReactionElement.Physical
                    : ElementalReactionCodex.Contains(signature, ReactionElement.Cold)
                        ? ReactionElement.Cold
                        : ElementalReactionCodex.Contains(signature, ReactionElement.Fire)
                            ? ReactionElement.Fire
                            : catalyst;

            ReactionElement residue =
                ElementalReactionCodex.Contains(signature, ReactionElement.Toxic)
                    ? ReactionElement.Toxic
                    : ElementalReactionCodex.Contains(signature, ReactionElement.Fire)
                        ? ReactionElement.Fire
                        : ElementalReactionCodex.Contains(signature, ReactionElement.Blood)
                            ? ReactionElement.Blood
                            : ElementalReactionCodex.Contains(signature, ReactionElement.Void)
                                ? ReactionElement.Void
                                : primary;

            recipe.primaryElement = primary;
            recipe.motionElement = motion;
            recipe.impactElement = impact;
            recipe.residueElement = residue;

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(signature))
            {
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(element);
                ElementVisualRole role = ElementVisualRole.Secondary;
                float intensity = count <= 1 ? 1f : 0.48f;

                if (element == primary)
                {
                    role = ElementVisualRole.PrimarySilhouette;
                    intensity = 1f;
                }
                else if (element == catalyst)
                {
                    role = ElementVisualRole.Catalyst;
                    intensity = 0.88f;
                }
                else if (element == motion)
                {
                    role = ElementVisualRole.Motion;
                    intensity = 0.72f;
                }
                else if (element == impact)
                {
                    role = ElementVisualRole.Impact;
                    intensity = 0.68f;
                }
                else if (element == residue)
                {
                    role = ElementVisualRole.Residue;
                    intensity = 0.62f;
                }

                if (element == ReactionElement.Blood &&
                    Patch200PresentationSettings.BloodMode == BloodPresentationMode.Hidden)
                {
                    intensity = 0f;
                }
                else if (element == ReactionElement.Blood &&
                         Patch200PresentationSettings.BloodMode == BloodPresentationMode.Reduced)
                {
                    intensity *= 0.35f;
                }

                recipe.elementLayers.Add(new ElementVisualLayer
                {
                    element = element,
                    role = role,
                    intensity = intensity,
                    primary = profile.primary,
                    secondary = profile.secondary,
                    motionLanguage = profile.motionLanguage,
                    impactLanguage = profile.impactLanguage,
                    residueLanguage = profile.residueLanguage
                });
            }

            if (recipe.elementLayers.Count == 0)
            {
                ElementVisualProfile2 arcane = ElementVisualProfileRegistry2.Get(ReactionElement.None);
                recipe.elementLayers.Add(new ElementVisualLayer
                {
                    element = ReactionElement.None,
                    role = ElementVisualRole.PrimarySilhouette,
                    intensity = 1f,
                    primary = recipe.primaryColor,
                    secondary = recipe.secondaryColor,
                    motionLanguage = arcane.motionLanguage,
                    impactLanguage = arcane.impactLanguage,
                    residueLanguage = arcane.residueLanguage
                });
            }
        }

        private static void BuildOperators(
            GeneratedVisualRecipe recipe,
            CompiledSpell spell)
        {
            int order = 0;

            AddOperator(recipe, RuneVisualOperatorKind.Homing, ReadFloat(spell, "homingStrength", 0f), 1, 0f, order++, "CompiledSpell.homingStrength", "directional pulse");
            AddOperator(recipe, RuneVisualOperatorKind.Arc, ReadFloat(spell, "arcAmount", 0f), 1, 0f, order++, "CompiledSpell.arcAmount", "curved guide trail");
            AddOperator(recipe, RuneVisualOperatorKind.Pierce, ReadInt(spell, "pierce", 0), ReadInt(spell, "pierce", 0), 0f, order++, "CompiledSpell.pierce", "contact marks");
            AddOperator(recipe, RuneVisualOperatorKind.Chain, ReadInt(spell, "chainTargets", 0), ReadInt(spell, "chainTargets", 0), 0f, order++, "CompiledSpell.chainTargets", "readable transfer line");
            AddOperator(recipe, RuneVisualOperatorKind.Bounce, ReadInt(spell, "bounce", 0), ReadInt(spell, "bounce", 0), 0f, order++, "CompiledSpell.bounce", "redirect flash");
            AddBooleanOperator(recipe, spell, RuneVisualOperatorKind.Return, "returnsToCaster", order++, "reversed trail flow");
            AddBooleanOperator(recipe, spell, RuneVisualOperatorKind.Orbit, "orbitCaster", order++, "orbital guide ring");
            AddOperator(recipe, RuneVisualOperatorKind.Accelerate, Mathf.Abs(ReadFloat(spell, "acceleration", 0f)), 1, 0f, order++, "CompiledSpell.acceleration", "speed streak");
            AddOperator(recipe, RuneVisualOperatorKind.Split, ReadInt(spell, "splitCount", 0), ReadInt(spell, "splitCount", 0), 0f, order++, "CompiledSpell.splitCount", "child formation flash");
            AddOperator(recipe, RuneVisualOperatorKind.Delay, ReadFloat(spell, "detonationDelay", 0f), 1, ReadFloat(spell, "detonationDelay", 0f), order++, "CompiledSpell.detonationDelay", "countdown ring");
            float persistentDuration = Mathf.Max(
                ReadFloat(spell, "zoneDuration", 0f),
                ReadFloat(spell, "trailDuration", 0f));
            AddOperator(recipe, RuneVisualOperatorKind.Persistent, persistentDuration, 1, persistentDuration, order++, "CompiledSpell.zoneDuration/trailDuration", "field boundary");
            AddBooleanOperator(recipe, spell, RuneVisualOperatorKind.SpreadStatus, "spreadsStatus", order++, "transfer filament");
            AddBooleanOperator(recipe, spell, RuneVisualOperatorKind.ConsumeStatus, "consumesStatus", order++, "inward extraction ring");
            AddOperator(recipe, RuneVisualOperatorKind.Barrier, ReadFloat(spell, "barrierOnManualCast", 0f), 1, 0f, order++, "CompiledSpell.barrierOnManualCast", "defensive ring");
            AddBooleanOperator(recipe, spell, RuneVisualOperatorKind.Pull, "pullsEnemies", order++, "inward streaks");
            AddBooleanOperator(recipe, spell, RuneVisualOperatorKind.Reflect, "reflectsProjectiles", order++, "reflection facet");
            AddOperator(recipe, RuneVisualOperatorKind.Repeat, ReadInt(spell, "repeatCount", 1) - 1, ReadInt(spell, "repeatCount", 1), ReadFloat(spell, "repeatDelay", 0f), order++, "CompiledSpell.repeatCount", "repeat echo");
            AddOperator(recipe, RuneVisualOperatorKind.Summon, ReadInt(spell, "summonCount", 0), ReadInt(spell, "summonCount", 0), ReadFloat(spell, "lifetime", 0f), order++, "CompiledSpell.summonCount/lifetime", "familiar chassis");

            IList executionLayers = ReadList(spell, "executionLayers");

            if (executionLayers != null)
            {
                for (int i = 0; i < executionLayers.Count; i++)
                {
                    string layer = Convert.ToString(executionLayers[i]);

                    if (string.IsNullOrEmpty(layer))
                        continue;

                    if (layer.IndexOf("Trigger", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        layer.IndexOf("OnHit", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        layer.IndexOf("OnKill", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        recipe.operators.Add(new RuneVisualOperatorSpec
                        {
                            kind = RuneVisualOperatorKind.Trigger,
                            magnitude = 1f,
                            count = 1,
                            duration = 0f,
                            order = order++,
                            source = layer,
                            fallback = "trigger-link glyph"
                        });
                    }
                }
            }

            recipe.operators.Sort((a, b) => a.order.CompareTo(b.order));
        }

        private static void AddBooleanOperator(
            GeneratedVisualRecipe recipe,
            CompiledSpell spell,
            RuneVisualOperatorKind kind,
            string member,
            int order,
            string fallback)
        {
            if (!ReadBool(spell, member, false))
                return;

            recipe.operators.Add(new RuneVisualOperatorSpec
            {
                kind = kind,
                magnitude = 1f,
                count = 1,
                duration = 0f,
                order = order,
                source = "CompiledSpell." + member,
                fallback = fallback
            });
        }

        private static void AddOperator(
            GeneratedVisualRecipe recipe,
            RuneVisualOperatorKind kind,
            float magnitude,
            int count,
            float duration,
            int order,
            string source,
            string fallback)
        {
            if (magnitude <= 0.001f && count <= 0)
                return;

            recipe.operators.Add(new RuneVisualOperatorSpec
            {
                kind = kind,
                magnitude = magnitude,
                count = Mathf.Max(1, count),
                duration = duration,
                order = order,
                source = source,
                fallback = fallback
            });
        }

        private static void BuildLayerRequirements(
            GeneratedVisualRecipe recipe,
            CompiledSpell spell)
        {
            recipe.requiredLayers.Add("delivery silhouette");
            recipe.requiredLayers.Add("direction or authoritative boundary");
            recipe.requiredLayers.Add("impact timing");
            recipe.requiredLayers.Add("element shape symbol");

            if (recipe.operators.Exists(value => value.kind == RuneVisualOperatorKind.Chain))
                recipe.requiredLayers.Add("chain target connection");

            if (recipe.operators.Exists(value => value.kind == RuneVisualOperatorKind.Delay))
                recipe.requiredLayers.Add("delay countdown");

            if (recipe.operators.Exists(value => value.kind == RuneVisualOperatorKind.Persistent))
                recipe.requiredLayers.Add("persistent hazard boundary");

            recipe.decorativeLayers.Add("secondary particles");
            recipe.decorativeLayers.Add("smoke or vapor density");
            recipe.decorativeLayers.Add("secondary fragments");
            recipe.decorativeLayers.Add("temporary light");
            recipe.decorativeLayers.Add("distortion");

            recipe.fallbackLayers.Add("procedural primitive chassis");
            recipe.fallbackLayers.Add("line-renderer telegraph");
            recipe.fallbackLayers.Add("unlit emissive material");
            recipe.fallbackLayers.Add("shape-symbol element marker");
        }

        private static ReactionElement ResolveSignature(CompiledSpell spell)
        {
            ReactionElement signature = ElementalReactionCodex.FromSpellElement(spell.element);

            if (ReadFloat(spell, "burnDamage", 0f) > 0f)
                signature |= ReactionElement.Fire;

            if (ReadFloat(spell, "chillMagnitude", 0f) > 0f ||
                ReadFloat(spell, "freezeSeconds", 0f) > 0f)
                signature |= ReactionElement.Cold;

            if (ReadFloat(spell, "shockMagnitude", 0f) > 0f)
                signature |= ReactionElement.Lightning;

            if (ReadFloat(spell, "poisonDamage", 0f) > 0f)
                signature |= ReactionElement.Toxic;

            if (ReadFloat(spell, "bleedDamage", 0f) > 0f)
                signature |= ReactionElement.Blood;

            if (ReadBool(spell, "pullsEnemies", false))
                signature |= ReactionElement.Void;

            if (ReadInt(spell, "pierce", 0) > 0 ||
                ReadFloat(spell, "knockback", 0f) > 0f)
                signature |= ReactionElement.Physical;

            IList layers = ReadList(spell, "executionLayers");

            if (layers != null)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    string value = Convert.ToString(layers[i]);
                    signature |= ElementsFromText(value);
                }
            }

            return signature;
        }

        private static ReactionElement ResolveCatalyst(
            CompiledSpell spell,
            ReactionElement signature)
        {
            ReactionElement catalyst = ElementalReactionCodex.FromSpellElement(spell.element);
            IList layers = ReadList(spell, "executionLayers");

            if (layers != null)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    ReactionElement layerElements = ElementsFromText(Convert.ToString(layers[i]));

                    foreach (ReactionElement element in ElementalReactionCodex.Enumerate(layerElements))
                        catalyst = element;
                }
            }

            if (catalyst == ReactionElement.None ||
                !ElementalReactionCodex.Contains(signature, catalyst))
            {
                catalyst = ElementalReactionCodex.PrimaryElement(signature);
            }

            return catalyst;
        }

        private static ReactionElement ElementsFromText(string value)
        {
            if (string.IsNullOrEmpty(value))
                return ReactionElement.None;

            ReactionElement result = ReactionElement.None;

            if (Contains(value, "Fire") || Contains(value, "Burn") || Contains(value, "Scorch"))
                result |= ReactionElement.Fire;

            if (Contains(value, "Frost") || Contains(value, "Cold") || Contains(value, "Chill") || Contains(value, "Freeze"))
                result |= ReactionElement.Cold;

            if (Contains(value, "Lightning") || Contains(value, "Shock") || Contains(value, "Static"))
                result |= ReactionElement.Lightning;

            if (Contains(value, "Physical") || Contains(value, "Impact") || Contains(value, "Trauma") || Contains(value, "Pierce"))
                result |= ReactionElement.Physical;

            if (Contains(value, "Blood") || Contains(value, "Bleed") || Contains(value, "Wound"))
                result |= ReactionElement.Blood;

            if (Contains(value, "Toxic") || Contains(value, "Poison"))
                result |= ReactionElement.Toxic;

            if (Contains(value, "Void") || Contains(value, "Curse") || Contains(value, "Corruption") || Contains(value, "Gravity"))
                result |= ReactionElement.Void;

            return result;
        }

        private static bool Contains(string value, string token)
        {
            return value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static BoardTopologySummary ResolveTopology(SpellBoard board)
        {
            BoardTopologySummary summary = new BoardTopologySummary();

            if (board == null)
            {
                summary.topologyText = "Compiled without board topology";
                return summary;
            }

            object placedObject = Read(board, "placed", null);
            IEnumerable enumerable = placedObject as IEnumerable;

            if (enumerable == null)
            {
                summary.topologyText = "Board topology unavailable";
                return summary;
            }

            List<string> ids = new List<string>();
            Dictionary<string, int> coordinates = new Dictionary<string, int>();
            int positiveRotation = 0;
            int negativeRotation = 0;
            Vector2 direction = Vector2.zero;

            foreach (object piece in enumerable)
            {
                if (piece == null)
                    continue;

                string id = Convert.ToString(Read(piece, "modifierId", string.Empty));
                int rotation = ReadInt(piece, "rotation", 0);
                object anchor = Read(piece, "anchor", Read(piece, "origin", null));
                int q = ReadInt(anchor, "q", 0);
                int r = ReadInt(anchor, "r", 0);
                int distance = Mathf.Max(Mathf.Abs(q), Mathf.Max(Mathf.Abs(r), Mathf.Abs(q + r)));

                summary.runeCount++;
                summary.maxHexDistance = Mathf.Max(summary.maxHexDistance, distance);
                direction += new Vector2(q + r * 0.5f, r * 0.8660254f);
                ids.Add(id + "@" + q + "," + r + ":" + rotation);
                coordinates[q + "," + r] = 1;

                int normalized = ((rotation % 6) + 6) % 6;

                if (normalized == 1 || normalized == 2 || normalized == 3)
                    positiveRotation++;
                else if (normalized == 4 || normalized == 5)
                    negativeRotation++;
            }

            ids.Sort(StringComparer.Ordinal);
            summary.orderedRuneIds = string.Join("|", ids);
            summary.clockwiseBias = Math.Sign(positiveRotation - negativeRotation);
            summary.dominantDirection = direction.sqrMagnitude > 0.001f
                ? direction.normalized
                : Vector2.zero;

            int branches = 0;
            Vector2[] directions =
            {
                new Vector2(1f, 0f),
                new Vector2(0.5f, 0.866f),
                new Vector2(-0.5f, 0.866f),
                new Vector2(-1f, 0f),
                new Vector2(-0.5f, -0.866f),
                new Vector2(0.5f, -0.866f)
            };

            for (int i = 0; i < directions.Length; i++)
            {
                if (direction.sqrMagnitude > 0.001f &&
                    Vector2.Dot(direction.normalized, directions[i]) > 0.34f)
                {
                    branches++;
                }
            }

            summary.branchCount = Mathf.Clamp(branches, 1, 6);
            summary.approximatelySymmetric =
                summary.runeCount <= 1 || direction.magnitude <= Mathf.Max(1f, summary.runeCount * 0.25f);

            summary.topologyText =
                summary.runeCount + " Runes · " +
                summary.branchCount + " branch estimate · " +
                (summary.approximatelySymmetric ? "approximately symmetric" : "directionally biased") +
                (summary.clockwiseBias > 0 ? " · clockwise rotation bias" : summary.clockwiseBias < 0 ? " · counterclockwise rotation bias" : string.Empty);

            return summary;
        }

        private static int ComputeConfigurationHash(
            CompiledSpell spell,
            ReactionElement signature,
            ReactionElement catalyst,
            BoardTopologySummary topology)
        {
            uint hash = 2166136261u;
            AddHash(ref hash, GeneratedVisualRecipe.CurrentSchemaVersion.ToString());
            AddHash(ref hash, spell.coreId);
            AddHash(ref hash, spell.displayName);
            AddHash(ref hash, Convert.ToString(Read(spell, "delivery", string.Empty)));
            AddHash(ref hash, Convert.ToString((int)signature));
            AddHash(ref hash, Convert.ToString((int)catalyst));
            AddHash(ref hash, Convert.ToString(Read(spell, "relicId", string.Empty)));
            AddHash(ref hash, topology == null ? string.Empty : topology.orderedRuneIds);
            AddHash(ref hash, ReadFloat(spell, "size", 1f).ToString("R"));
            AddHash(ref hash, ReadInt(spell, "projectileCount", 1).ToString());
            AddHash(ref hash, ReadInt(spell, "chainTargets", 0).ToString());
            AddHash(ref hash, ReadInt(spell, "splitCount", 0).ToString());
            AddHash(ref hash, ReadInt(spell, "bounce", 0).ToString());
            AddHash(ref hash, ReadInt(spell, "pierce", 0).ToString());

            return unchecked((int)hash);
        }

        private static void AddHash(ref uint hash, string value)
        {
            if (value == null)
                value = string.Empty;

            for (int i = 0; i < value.Length; i++)
            {
                hash ^= value[i];
                hash *= 16777619u;
            }
        }

        private static VisualChassisKind ResolveChassis(string delivery)
        {
            if (string.IsNullOrEmpty(delivery))
                return VisualChassisKind.Unknown;

            if (Contains(delivery, "Projectile")) return VisualChassisKind.Projectile;
            if (Contains(delivery, "Beam")) return VisualChassisKind.Beam;
            if (Contains(delivery, "Zone")) return VisualChassisKind.Zone;
            if (Contains(delivery, "Nova")) return VisualChassisKind.Nova;
            if (Contains(delivery, "Meteor")) return VisualChassisKind.Meteor;
            if (Contains(delivery, "Melee")) return VisualChassisKind.Melee;
            if (Contains(delivery, "Summon") || Contains(delivery, "Familiar")) return VisualChassisKind.Familiar;
            if (Contains(delivery, "Movement") || Contains(delivery, "Dash") || Contains(delivery, "Teleport")) return VisualChassisKind.Movement;
            return VisualChassisKind.Unknown;
        }

        private static IList ReadList(object instance, string member)
        {
            return Read(instance, member, null) as IList;
        }

        private static bool ReadBool(object instance, string member, bool fallback)
        {
            object value = Read(instance, member, fallback);

            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return fallback;
            }
        }

        private static int ReadInt(object instance, string member, int fallback)
        {
            object value = Read(instance, member, fallback);

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return fallback;
            }
        }

        private static float ReadFloat(object instance, string member, float fallback)
        {
            object value = Read(instance, member, fallback);

            try
            {
                return Convert.ToSingle(value);
            }
            catch
            {
                return fallback;
            }
        }

        private static object Read(object instance, string member, object fallback)
        {
            if (instance == null || string.IsNullOrEmpty(member))
                return fallback;

            Type type = instance.GetType();
            FieldInfo field = type.GetField(
                member,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            if (field != null)
                return field.GetValue(instance);

            PropertyInfo property = type.GetProperty(
                member,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            if (property != null && property.CanRead)
                return property.GetValue(instance, null);

            return fallback;
        }
    }
}
