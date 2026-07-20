using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum SpellPhase21
    {
        Prime,
        Charge,
        Release,
        Emit,
        Hold,
        Travel,
        AcquireTarget,
        Contact,
        Resolve,
        Repeat,
        Persist,
        Return,
        Expire,
        Cancel
    }

    public enum PhaseMutation21
    {
        Preserve,
        InsertBefore,
        InsertAfter,
        Replace,
        Repeat,
        Branch,
        Redirect,
        Transform,
        Terminate
    }

    public enum RuneSeverity21
    {
        Modifier,
        Structural,
        Transformative
    }

    public enum PresentationOwnerKind21
    {
        Cast,
        Projectile,
        TargetStatus,
        ReactionAssembly,
        ReactionResolution,
        Field,
        Familiar,
        SceneEvent
    }

    public enum VisualPersonality21
    {
        Precise,
        Violent,
        Heavy,
        Elegant,
        Predatory,
        Defensive,
        Contagious,
        Ritualistic,
        Chaotic,
        Unstable
    }

    public enum ContactSurface21
    {
        Unknown,
        Enemy,
        Ground,
        Wall,
        Barrier,
        WaterLike,
        ElementalField
    }

    public enum BodyPartKind21
    {
        Core,
        Shell,
        InternalEnergy,
        RuneNode,
        SecondaryCore,
        Ring,
        Band,
        Fin,
        Spike,
        Crystal,
        Orbital,
        FractureSeam,
        InternalPath,
        MotionGuide,
        TrailEmitter,
        ImpactAnchor,
        FieldPanel,
        TerminationPart
    }

    [Serializable]
    public sealed class CoreIdentityInvariant21
    {
        public string id;
        public string description;
        public bool required;
        public float weight = 1f;
    }

    [Serializable]
    public sealed class LifecycleNode21
    {
        public string id;
        public SpellPhase21 phase;
        public string source;
        public float normalizedStart;
        public float normalizedDuration;
        public bool required;
        public readonly List<string> next = new List<string>();

        public override string ToString()
        {
            return phase + " · " + source;
        }
    }

    [Serializable]
    public sealed class RuneOperatorNode21
    {
        public string id;
        public RuneVisualOperatorKind kind;
        public RuneSeverity21 severity;
        public PhaseMutation21 mutation;
        public SpellPhase21 targetPhase;
        public int order;
        public int boardQ;
        public int boardR;
        public int rotation;
        public int branch;
        public float magnitude;
        public int count;
        public float duration;
        public string sourceRuneId;
        public string displayName;
        public string deliveryImplementation;
        public string fallback;
        public bool visibleDuringCasting;
        public bool visibleDuringTravel;
        public bool visibleDuringResolution;
        public bool visibleDuringTermination;

        public override string ToString()
        {
            return order + " · " + kind + " · " + mutation + " " + targetPhase;
        }
    }

    [Serializable]
    public sealed class BodyPartSpec21
    {
        public string id;
        public BodyPartKind21 kind;
        public string meshKey;
        public ReactionElement element;
        public RuneVisualOperatorKind runeKind;
        public Vector3 localPosition;
        public Vector3 localEuler;
        public Vector3 localScale = Vector3.one;
        public float emissive = 1f;
        public float opacity = 1f;
        public int seed;
        public int qualityMinimum;
        public bool required;
        public bool instanced;
    }

    [Serializable]
    public sealed class VisualCost21
    {
        public int emitters;
        public int maximumParticles;
        public float transparentCoverage;
        public int lineSegments;
        public int trailPoints;
        public int meshInstances;
        public int temporaryLights;
        public int distortionLayers;
        public int audioVoices;
        public int cameraEvents;
        public int residues;

        public int Score
        {
            get
            {
                return emitters * 12 +
                       maximumParticles / 10 +
                       Mathf.RoundToInt(transparentCoverage * 40f) +
                       lineSegments / 2 +
                       trailPoints / 4 +
                       meshInstances * 3 +
                       temporaryLights * 25 +
                       distortionLayers * 40 +
                       audioVoices * 8 +
                       cameraEvents * 6 +
                       residues * 12;
            }
        }

        public override string ToString()
        {
            return "Particles " + maximumParticles +
                   " · Emitters " + emitters +
                   " · Meshes " + meshInstances +
                   " · Lines " + lineSegments +
                   " · Trails " + trailPoints +
                   " · Lights " + temporaryLights +
                   " · Distortion " + distortionLayers +
                   " · Audio " + audioVoices +
                   " · Cost " + Score;
        }
    }

    [Serializable]
    public sealed class HierarchicalSeeds21
    {
        public int spell;
        public int core;
        public int elements;
        public int operators;
        public int body;
        public int trail;
        public int impact;
        public int residue;
        public int audio;
        public int icon;
    }

    [Serializable]
    public sealed class SpellVisualContract21
    {
        public const int CurrentSchemaVersion = 21000;

        public int schemaVersion = CurrentSchemaVersion;
        public int configurationHash;
        public string contractId;
        public string coreId;
        public string displayName;
        public VisualChassisKind chassis;
        public string deliveryName;
        public ReactionElement signature;
        public ReactionElement catalyst;
        public ReactionElement primaryElement;
        public ReactionElement motionElement;
        public ReactionElement impactElement;
        public ReactionElement residueElement;
        public ReactionTier reactionTier;
        public VisualPersonality21 personality;
        public PresentationPriority priority;
        public float size;
        public float speed;
        public float radius;
        public float duration;
        public int projectileCount;
        public bool supportsReturn;
        public bool supportsPersistence;
        public bool supportsTargeting;
        public BoardTopologySummary topology;
        public GeneratedVisualRecipe baseRecipe;
        public HierarchicalSeeds21 seeds = new HierarchicalSeeds21();
        public VisualCost21 cost = new VisualCost21();
        public readonly List<CoreIdentityInvariant21> coreInvariants = new List<CoreIdentityInvariant21>();
        public readonly List<LifecycleNode21> lifecycle = new List<LifecycleNode21>();
        public readonly List<RuneOperatorNode21> runeGraph = new List<RuneOperatorNode21>();
        public readonly List<BodyPartSpec21> bodyParts = new List<BodyPartSpec21>();
        public readonly List<string> interactionRules = new List<string>();
        public readonly List<string> requiredLayers = new List<string>();
        public readonly List<string> decorativeLayers = new List<string>();
        public readonly List<string> fallbacks = new List<string>();
        public readonly List<string> validationWarnings = new List<string>();

        public string PhaseText
        {
            get
            {
                List<string> values = new List<string>();
                for (int i = 0; i < lifecycle.Count; i++)
                    values.Add(lifecycle[i].phase.ToString());
                return string.Join(" → ", values.ToArray());
            }
        }

        public string RuneText
        {
            get
            {
                List<string> values = new List<string>();
                for (int i = 0; i < runeGraph.Count; i++)
                    values.Add(runeGraph[i].kind.ToString());
                return values.Count == 0 ? "None" : string.Join(" → ", values.ToArray());
            }
        }

        public string DebugSummary
        {
            get
            {
                return "Contract: " + contractId + "\n" +
                       "Core: " + coreId + " · " + chassis + "\n" +
                       "Elements: " + ElementalReactionCodex.SignatureText(signature) + "\n" +
                       "Catalyst: " + catalyst + "\n" +
                       "Personality: " + personality + "\n" +
                       "Runes: " + RuneText + "\n" +
                       "Phases: " + PhaseText + "\n" +
                       "Body parts: " + bodyParts.Count + "\n" +
                       "Cost: " + cost + "\n" +
                       "Seed: 0x" + seeds.spell.ToString("X8") + "\n" +
                       "Schema: " + schemaVersion;
            }
        }
    }

    public static class StableSeed21
    {
        public static int Combine(int seed, int value)
        {
            unchecked
            {
                uint hash = (uint)seed;
                hash ^= (uint)value + 0x9e3779b9u + (hash << 6) + (hash >> 2);
                hash *= 16777619u;
                return (int)hash;
            }
        }

        public static int Combine(int seed, string value)
        {
            unchecked
            {
                int hash = seed;
                if (string.IsNullOrEmpty(value))
                    return Combine(hash, 0);
                for (int i = 0; i < value.Length; i++)
                    hash = Combine(hash, value[i]);
                return hash;
            }
        }

        public static float Unit(int seed)
        {
            unchecked
            {
                uint value = (uint)seed;
                value ^= value << 13;
                value ^= value >> 17;
                value ^= value << 5;
                return (value & 0x00ffffffu) / 16777215f;
            }
        }

        public static Vector3 UnitVector(int seed)
        {
            float a = Unit(Combine(seed, 17)) * Mathf.PI * 2f;
            float y = Unit(Combine(seed, 31)) * 2f - 1f;
            float r = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));
            return new Vector3(Mathf.Cos(a) * r, y, Mathf.Sin(a) * r);
        }
    }
}
