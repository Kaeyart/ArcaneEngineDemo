using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    [Flags]
    public enum ReactionElement
    {
        None = 0,
        Fire = 1 << 0,
        Cold = 1 << 1,
        Lightning = 1 << 2,
        Physical = 1 << 3,
        Blood = 1 << 4,
        Toxic = 1 << 5,
        Void = 1 << 6,
        All = Fire | Cold | Lightning | Physical | Blood | Toxic | Void
    }

    public enum ReactionTier
    {
        Fusion = 2,
        Compound = 3,
        Catastrophe = 4,
        Convergence = 5,
        Calamity = 6,
        Apex = 7
    }

    [Serializable]
    public sealed class ElementalReactionDefinition
    {
        public readonly string id;
        public readonly ReactionElement signature;
        public readonly string displayName;
        public readonly ReactionTier tier;
        public readonly string description;
        public readonly ReactionElement catalyst;
        public readonly float damageMultiplier;
        public readonly float radius;
        public readonly float duration;
        public readonly int pulseCount;
        public readonly int spreadStacks;
        public readonly float assemblyWindow;

        public int ElementCount
        {
            get { return ElementalReactionCodex.CountBits(signature); }
        }

        public ElementalReactionDefinition(
            string reactionId,
            ReactionElement reactionSignature,
            string reactionName,
            ReactionTier reactionTier,
            string reactionDescription,
            ReactionElement catalystElement,
            float damageScale,
            float reactionRadius,
            float reactionDuration,
            int pulses,
            int spread,
            float window)
        {
            id = reactionId;
            signature = reactionSignature;
            displayName = reactionName;
            tier = reactionTier;
            description = reactionDescription;
            catalyst = catalystElement;
            damageMultiplier = damageScale;
            radius = reactionRadius;
            duration = reactionDuration;
            pulseCount = Mathf.Max(1, pulses);
            spreadStacks = Mathf.Max(1, spread);
            assemblyWindow = Mathf.Max(0.1f, window);
        }
    }

    public static partial class ElementalReactionCodex
    {
        private static readonly Dictionary<int, ElementalReactionDefinition>
            Definitions = BuildDefinitions();

        private static Dictionary<int, ElementalReactionDefinition>
            BuildDefinitions()
        {
            Dictionary<int, ElementalReactionDefinition> definitions =
                new Dictionary<int, ElementalReactionDefinition>(120);

            PopulateGenerated(definitions);
            return definitions;
        }

        public static int Count
        {
            get { return Definitions.Count; }
        }

        public static IEnumerable<ElementalReactionDefinition> All
        {
            get { return Definitions.Values; }
        }

        public static bool TryGet(
            ReactionElement signature,
            out ElementalReactionDefinition definition)
        {
            return Definitions.TryGetValue((int)signature, out definition);
        }

        public static ElementalReactionDefinition Get(
            ReactionElement signature)
        {
            ElementalReactionDefinition definition;
            return TryGet(signature, out definition) ? definition : null;
        }

        public static int CountBits(ReactionElement value)
        {
            int bits = (int)value;
            int count = 0;

            while (bits != 0)
            {
                count += bits & 1;
                bits >>= 1;
            }

            return count;
        }

        public static bool Contains(
            ReactionElement signature,
            ReactionElement element)
        {
            return (signature & element) != 0;
        }

        public static IEnumerable<ReactionElement> Enumerate(
            ReactionElement signature)
        {
            ReactionElement[] ordered =
            {
                ReactionElement.Fire,
                ReactionElement.Cold,
                ReactionElement.Lightning,
                ReactionElement.Physical,
                ReactionElement.Blood,
                ReactionElement.Toxic,
                ReactionElement.Void
            };

            foreach (ReactionElement element in ordered)
            {
                if (Contains(signature, element))
                    yield return element;
            }
        }

        public static int IndexOf(ReactionElement element)
        {
            switch (element)
            {
                case ReactionElement.Fire: return 0;
                case ReactionElement.Cold: return 1;
                case ReactionElement.Lightning: return 2;
                case ReactionElement.Physical: return 3;
                case ReactionElement.Blood: return 4;
                case ReactionElement.Toxic: return 5;
                case ReactionElement.Void: return 6;
                default: return -1;
            }
        }

        public static ReactionElement FromSpellElement(SpellElement element)
        {
            switch (element)
            {
                case SpellElement.Fire: return ReactionElement.Fire;
                case SpellElement.Frost: return ReactionElement.Cold;
                case SpellElement.Lightning: return ReactionElement.Lightning;
                case SpellElement.Toxic: return ReactionElement.Toxic;
                case SpellElement.Void: return ReactionElement.Void;
                case SpellElement.Physical: return ReactionElement.Physical;
                case SpellElement.Blood: return ReactionElement.Blood;
                default: return ReactionElement.None;
            }
        }

        public static SpellElement ToSpellElement(ReactionElement element)
        {
            switch (element)
            {
                case ReactionElement.Fire: return SpellElement.Fire;
                case ReactionElement.Cold: return SpellElement.Frost;
                case ReactionElement.Lightning: return SpellElement.Lightning;
                case ReactionElement.Toxic: return SpellElement.Toxic;
                case ReactionElement.Void: return SpellElement.Void;
                case ReactionElement.Physical: return SpellElement.Physical;
                case ReactionElement.Blood: return SpellElement.Blood;
                default: return SpellElement.Arcane;
            }
        }

        public static ReactionElement PrimaryElement(
            ReactionElement signature)
        {
            foreach (ReactionElement element in Enumerate(signature))
                return element;

            return ReactionElement.None;
        }

        public static string DisplayName(ReactionElement element)
        {
            switch (element)
            {
                case ReactionElement.Cold: return "Cold";
                case ReactionElement.Physical: return "Physical";
                default: return element.ToString();
            }
        }

        public static string SignatureText(ReactionElement signature)
        {
            List<string> names = new List<string>();

            foreach (ReactionElement element in Enumerate(signature))
                names.Add(DisplayName(element));

            return string.Join(" + ", names);
        }

        public static Color ColorFor(ReactionElement element)
        {
            switch (element)
            {
                case ReactionElement.Fire:
                    return new Color(1f, 0.27f, 0.04f);
                case ReactionElement.Cold:
                    return new Color(0.25f, 0.82f, 1f);
                case ReactionElement.Lightning:
                    return new Color(0.95f, 0.9f, 0.2f);
                case ReactionElement.Physical:
                    return new Color(0.75f, 0.67f, 0.55f);
                case ReactionElement.Blood:
                    return new Color(0.75f, 0.03f, 0.08f);
                case ReactionElement.Toxic:
                    return new Color(0.35f, 0.95f, 0.12f);
                case ReactionElement.Void:
                    return new Color(0.52f, 0.08f, 0.85f);
                default:
                    return Color.white;
            }
        }

        public static Color BlendColor(ReactionElement signature)
        {
            Color total = Color.black;
            int count = 0;

            foreach (ReactionElement element in Enumerate(signature))
            {
                total += ColorFor(element);
                count++;
            }

            if (count == 0)
                return Color.white;

            total /= count;
            total.a = 1f;
            return total;
        }

        public static float Threshold(ReactionElement element)
        {
            switch (element)
            {
                case ReactionElement.Fire: return 6f;
                case ReactionElement.Cold: return 8f;
                case ReactionElement.Lightning: return 6f;
                case ReactionElement.Physical: return 7f;
                case ReactionElement.Blood: return 6f;
                case ReactionElement.Toxic: return 8f;
                case ReactionElement.Void: return 7f;
                default: return float.MaxValue;
            }
        }

        public static string MajorStateName(ReactionElement element)
        {
            switch (element)
            {
                case ReactionElement.Fire: return "IGNITED";
                case ReactionElement.Cold: return "FROZEN";
                case ReactionElement.Lightning: return "OVERCHARGED";
                case ReactionElement.Physical: return "BROKEN";
                case ReactionElement.Blood: return "HEMORRHAGING";
                case ReactionElement.Toxic: return "SATURATED";
                case ReactionElement.Void: return "UNSTABLE";
                default: return string.Empty;
            }
        }

        public static string StackName(ReactionElement element)
        {
            switch (element)
            {
                case ReactionElement.Fire: return "SCORCH";
                case ReactionElement.Cold: return "CHILL";
                case ReactionElement.Lightning: return "STATIC";
                case ReactionElement.Physical: return "TRAUMA";
                case ReactionElement.Blood: return "WOUND";
                case ReactionElement.Toxic: return "POISON";
                case ReactionElement.Void: return "CORRUPTION";
                default: return string.Empty;
            }
        }
    }
}
