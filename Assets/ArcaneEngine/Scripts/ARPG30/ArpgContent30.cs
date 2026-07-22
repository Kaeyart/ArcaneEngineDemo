using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    [Serializable]
    public sealed class ArpgClassDefinition30
    {
        public ArpgClass30 id;
        public string displayName;
        public string description;
        public string resourceIdentity;
        public string defenceIdentity;
        public string starterCoreHint;
        public string complexity;
        public List<ArpgStatModifier30> baseModifiers = new List<ArpgStatModifier30>();
        public List<ArpgAscendancy30> ascendancies = new List<ArpgAscendancy30>();
    }

    [Serializable]
    public sealed class ArpgAscendancyNodeDefinition30
    {
        public string id;
        public string displayName;
        public string description;
        public List<ArpgStatModifier30> modifiers = new List<ArpgStatModifier30>();
    }

    [Serializable]
    public sealed class ArpgAscendancyDefinition30
    {
        public ArpgAscendancy30 id;
        public ArpgClass30 requiredClass;
        public string displayName;
        public string description;
        public List<ArpgAscendancyNodeDefinition30> nodes = new List<ArpgAscendancyNodeDefinition30>();
    }

    [Serializable]
    public sealed class ArpgConstellationNodeDefinition30
    {
        public string id;
        public string displayName;
        public string description;
        public ArpgNodeSize30 size;
        public int pointCost;
        public string prerequisiteId;
        public List<ArpgStatModifier30> modifiers = new List<ArpgStatModifier30>();
    }

    [Serializable]
    public sealed class ArpgConstellationDefinition30
    {
        public string id;
        public string displayName;
        public string category;
        public string description;
        public int attunementCost;
        public int requiredLevel;
        public int requiredTier;
        public List<ArpgConstellationNodeDefinition30> nodes = new List<ArpgConstellationNodeDefinition30>();
    }

    [Serializable]
    public sealed class ArpgItemBaseDefinition30
    {
        public string id;
        public string displayName;
        public ArpgItemSlot30 slot;
        public string family;
        public ArpgClass30 affinity;
        public int requiredLevel;
        public List<ArpgStatModifier30> implicitModifiers = new List<ArpgStatModifier30>();
    }

    [Serializable]
    public sealed class ArpgAffixDefinition30
    {
        public string id;
        public string displayName;
        public bool prefix;
        public ArpgStat30 stat;
        public float minimum;
        public float maximum;
        public int minimumItemLevel;
        public int weight;
        public string family;
        public string conflictGroup;
        public bool local;
        public List<ArpgItemSlot30> validSlots = new List<ArpgItemSlot30>();
    }

    [Serializable]
    public sealed class ArpgMapDefinition30
    {
        public string id;
        public string displayName;
        public int tier;
        public string region;
        public string bossName;
        public string bossId;
        public string monsterFamily;
        public string environmentHint;
        public string masteryRule;
        public string masteryDescription;
        public string rewardFocus;
        public int layoutIndex;
        public int nodeIndex;
        public bool playableIn31;
        public List<string> connectedMapIds = new List<string>();

        public ArpgMapBand30 Band
        {
            get
            {
                if (tier >= 40) return ArpgMapBand30.Pinnacle;
                if (tier >= 30) return ArpgMapBand30.Red;
                if (tier >= 20) return ArpgMapBand30.Yellow;
                if (tier >= 10) return ArpgMapBand30.Blue;
                return ArpgMapBand30.White;
            }
        }
    }

    [Serializable]
    public sealed class ArpgMapAffixDefinition30
    {
        public string id;
        public string displayName;
        public string description;
        public int minimumTier;
        public float rewardMultiplier;
        public string difficultyFlag;
    }

    [Serializable]
    public sealed class ArpgMonsterFamilyDefinition31
    {
        public string id;
        public string displayName;
        public string description;
        public string elementTheme;
        public List<string> archetypeHints = new List<string>();
    }

    [Serializable]
    public sealed class ArpgMonsterVariantDefinition31
    {
        public string id;
        public string displayName;
        public string familyId;
        public string role;
        public string signatureAbility;
        public float healthMultiplier = 1f;
        public float damageMultiplier = 1f;
        public float speedMultiplier = 1f;
        public List<string> archetypeHints = new List<string>();
    }

    [Serializable]
    public sealed class ArpgBossDefinition31
    {
        public string id;
        public string displayName;
        public string familyId;
        public string elementTheme;
        public string arenaTheme;
        public string masteryText;
        public List<string> mechanics = new List<string>();
    }

    public static class ArpgContent30
    {
        private static bool _ready;
        private static readonly List<ArpgClassDefinition30> ClassList = new List<ArpgClassDefinition30>();
        private static readonly List<ArpgAscendancyDefinition30> AscendancyList = new List<ArpgAscendancyDefinition30>();
        private static readonly List<ArpgConstellationDefinition30> ConstellationList = new List<ArpgConstellationDefinition30>();
        private static readonly List<ArpgItemBaseDefinition30> ItemBaseList = new List<ArpgItemBaseDefinition30>();
        private static readonly List<ArpgAffixDefinition30> AffixList = new List<ArpgAffixDefinition30>();
        private static readonly List<ArpgMapDefinition30> MapList = new List<ArpgMapDefinition30>();
        private static readonly List<ArpgMapAffixDefinition30> MapAffixList = new List<ArpgMapAffixDefinition30>();
        private static readonly List<ArpgMonsterFamilyDefinition31> MonsterFamilyList = new List<ArpgMonsterFamilyDefinition31>();
        private static readonly List<ArpgMonsterVariantDefinition31> MonsterVariantList = new List<ArpgMonsterVariantDefinition31>();
        private static readonly List<ArpgBossDefinition31> BossList = new List<ArpgBossDefinition31>();

        public static IReadOnlyList<ArpgClassDefinition30> Classes { get { Ensure(); return ClassList; } }
        public static IReadOnlyList<ArpgAscendancyDefinition30> Ascendancies { get { Ensure(); return AscendancyList; } }
        public static IReadOnlyList<ArpgConstellationDefinition30> Constellations { get { Ensure(); return ConstellationList; } }
        public static IReadOnlyList<ArpgItemBaseDefinition30> ItemBases { get { Ensure(); return ItemBaseList; } }
        public static IReadOnlyList<ArpgAffixDefinition30> Affixes { get { Ensure(); return AffixList; } }
        public static IReadOnlyList<ArpgMapDefinition30> Maps { get { Ensure(); return MapList; } }
        public static IReadOnlyList<ArpgMapAffixDefinition30> MapAffixes { get { Ensure(); return MapAffixList; } }
        public static IReadOnlyList<ArpgMonsterFamilyDefinition31> MonsterFamilies { get { Ensure(); return MonsterFamilyList; } }
        public static IReadOnlyList<ArpgMonsterVariantDefinition31> MonsterVariants { get { Ensure(); return MonsterVariantList; } }
        public static IReadOnlyList<ArpgBossDefinition31> Bosses { get { Ensure(); return BossList; } }

        public static void Ensure()
        {
            if (_ready) return;
            _ready = true;
            BuildClasses();
            BuildAscendancies();
            BuildConstellations();
            BuildItemBases();
            BuildAffixes();
            BuildMonsterFamilies();
            BuildMonsterVariants();
            BuildBosses();
            BuildMaps();
            BuildMapAffixes();
            ArpgArsenalContent32.ExtendLegacyCatalogues(ItemBaseList, AffixList, MapAffixList);
        }

        public static ArpgClassDefinition30 Class(ArpgClass30 id) { return Classes.FirstOrDefault(value => value.id == id); }
        public static ArpgAscendancyDefinition30 Ascendancy(ArpgAscendancy30 id) { return Ascendancies.FirstOrDefault(value => value.id == id); }
        public static ArpgConstellationDefinition30 Constellation(string id) { return Constellations.FirstOrDefault(value => value.id == id); }
        public static ArpgConstellationNodeDefinition30 ConstellationNode(string id)
        {
            return Constellations.SelectMany(value => value.nodes).FirstOrDefault(value => value.id == id);
        }

        public static ArpgAscendancyNodeDefinition30 AscendancyNode(string id)
        {
            return Ascendancies.SelectMany(value => value.nodes).FirstOrDefault(value => value.id == id);
        }

        public static ArpgItemBaseDefinition30 ItemBase(string id) { return ItemBases.FirstOrDefault(value => value.id == id); }
        public static ArpgAffixDefinition30 Affix(string id) { return Affixes.FirstOrDefault(value => value.id == id); }
        public static ArpgMapDefinition30 Map(string id) { return Maps.FirstOrDefault(value => value.id == id); }
        public static ArpgMapAffixDefinition30 MapAffix(string id) { return MapAffixes.FirstOrDefault(value => value.id == id); }
        public static ArpgMonsterFamilyDefinition31 MonsterFamily(string id) { return MonsterFamilies.FirstOrDefault(value => value.id == id); }
        public static ArpgMonsterVariantDefinition31 MonsterVariant(string id) { return MonsterVariants.FirstOrDefault(value => value.id == id); }
        public static ArpgBossDefinition31 Boss(string id) { return Bosses.FirstOrDefault(value => value.id == id); }

        private static void BuildClasses()
        {
            ClassList.Add(new ArpgClassDefinition30
            {
                id = ArpgClass30.Mage,
                displayName = "Mage",
                description = "A high-agency spell architect who converts Mana and Arcane Ward into complex elemental constructions.",
                resourceIdentity = "Mana",
                defenceIdentity = "Arcane Ward",
                starterCoreHint = "bolt,fireball,orb,projectile",
                complexity = "Advanced",
                baseModifiers = Mods(
                    new ArpgStatModifier30(ArpgStat30.MaximumMana, 30f),
                    new ArpgStatModifier30(ArpgStat30.ArcaneWard, 20f),
                    new ArpgStatModifier30(ArpgStat30.SpellPower, 0.12f)),
                ascendancies = new List<ArpgAscendancy30>
                {
                    ArpgAscendancy30.Elementalist,
                    ArpgAscendancy30.Chronomancer,
                    ArpgAscendancy30.Voidcaller
                }
            });

            ClassList.Add(new ArpgClassDefinition30
            {
                id = ArpgClass30.Ranger,
                displayName = "Ranger",
                description = "A mobile projectile specialist built around precision, Evasion, Chain routing, and controlled terrain.",
                resourceIdentity = "Focus",
                defenceIdentity = "Evasion",
                starterCoreHint = "arrow,shot,spear,projectile",
                complexity = "Intermediate",
                baseModifiers = Mods(
                    new ArpgStatModifier30(ArpgStat30.Evasion, 24f),
                    new ArpgStatModifier30(ArpgStat30.MoveSpeed, 0.75f),
                    new ArpgStatModifier30(ArpgStat30.CriticalChance, 0.035f)),
                ascendancies = new List<ArpgAscendancy30>
                {
                    ArpgAscendancy30.Deadeye,
                    ArpgAscendancy30.Stormrunner,
                    ArpgAscendancy30.Warden
                }
            });

            ClassList.Add(new ArpgClassDefinition30
            {
                id = ArpgClass30.Warrior,
                displayName = "Warrior",
                description = "A durable impact caster who combines Armour, Life, stagger pressure, barriers, and melee delivery.",
                resourceIdentity = "Resolve",
                defenceIdentity = "Armour",
                starterCoreHint = "strike,slam,wave,nova",
                complexity = "Direct",
                baseModifiers = Mods(
                    new ArpgStatModifier30(ArpgStat30.MaximumHealth, 35f),
                    new ArpgStatModifier30(ArpgStat30.Armour, 28f),
                    new ArpgStatModifier30(ArpgStat30.BarrierStrength, 0.10f)),
                ascendancies = new List<ArpgAscendancy30>
                {
                    ArpgAscendancy30.Juggernaut,
                    ArpgAscendancy30.Spellblade,
                    ArpgAscendancy30.Bastion
                }
            });
        }

        private static void BuildAscendancies()
        {
            AddAscendancy(ArpgAscendancy30.Elementalist, ArpgClass30.Mage, "Elementalist",
                "Buildup, reactions, fields, and controlled elemental cascades.",
                new[] { ArpgStat30.FirePower, ArpgStat30.ColdPower, ArpgStat30.LightningPower, ArpgStat30.ReactionPower },
                new[] { "Prismatic Initiate", "Catalyst Authority", "Prepared Cascade", "Elemental Dominion", "Field Alchemist", "Threshold Savant", "Conserved Proliferation", "Crown of Seven" });

            AddAscendancy(ArpgAscendancy30.Chronomancer, ArpgClass30.Mage, "Chronomancer",
                "Delay, Echo, Return, cooldown manipulation, and repeated spell phases.",
                new[] { ArpgStat30.CooldownRecovery, ArpgStat30.Duration, ArpgStat30.TriggerEnergy, ArpgStat30.MoveSpeed },
                new[] { "Second Hand", "Borrowed Moment", "Echo Chamber", "Reversal", "Stored Release", "Temporal Shelter", "Causal Loop", "Hour Without End" });

            AddAscendancy(ArpgAscendancy30.Voidcaller, ArpgClass30.Mage, "Voidcaller",
                "Compression, sacrifice, instability, and spatial control.",
                new[] { ArpgStat30.VoidPower, ArpgStat30.SpellPower, ArpgStat30.AreaOfEffect, ArpgStat30.ArcaneWard },
                new[] { "Black Spark", "Compressed Space", "Unstable Offering", "Event Horizon", "Hollow Geometry", "Abyssal Ward", "Collapsed Distance", "Voice Beyond" });

            AddAscendancy(ArpgAscendancy30.Deadeye, ArpgClass30.Ranger, "Deadeye",
                "Projectile geometry, Split, Pierce, ricochet, and weak-point precision.",
                new[] { ArpgStat30.ProjectileSpeed, ArpgStat30.CriticalChance, ArpgStat30.ChainRetention, ArpgStat30.SpellPower },
                new[] { "Far Sight", "Needle Line", "Ricochet Doctrine", "Perfect Angle", "Splinter Logic", "Marked Horizon", "No Wasted Motion", "One Impossible Shot" });

            AddAscendancy(ArpgAscendancy30.Stormrunner, ArpgClass30.Ranger, "Stormrunner",
                "Lightning, Chain, speed, movement casting, and momentum.",
                new[] { ArpgStat30.LightningPower, ArpgStat30.MoveSpeed, ArpgStat30.ChainRetention, ArpgStat30.CooldownRecovery },
                new[] { "Static Step", "Charged Route", "Momentum Coil", "Running Current", "Stormwake", "Unbroken Circuit", "Velocity Cast", "Living Lightning" });

            AddAscendancy(ArpgAscendancy30.Warden, ArpgClass30.Ranger, "Warden",
                "Placed spell structures, persistent control, Toxic and Cold terrain.",
                new[] { ArpgStat30.ToxicPower, ArpgStat30.ColdPower, ArpgStat30.Duration, ArpgStat30.AreaOfEffect },
                new[] { "Rooted Trap", "Bitter Soil", "Patient Hunter", "Warden's Mark", "Spore Lattice", "Frozen Boundary", "Living Construct", "Sovereign Wild" });

            AddAscendancy(ArpgAscendancy30.Juggernaut, ArpgClass30.Warrior, "Juggernaut",
                "Heavy impact, Physical damage, Armour conversion, and unstoppable movement.",
                new[] { ArpgStat30.PhysicalPower, ArpgStat30.Armour, ArpgStat30.MaximumHealth, ArpgStat30.BarrierStrength },
                new[] { "Stone Step", "Impact Reserve", "Armoured Spell", "Unbroken Advance", "Titan Grip", "Seismic Authority", "Worldweight", "Unstoppable" });

            AddAscendancy(ArpgAscendancy30.Spellblade, ArpgClass30.Warrior, "Spellblade",
                "Weapon-spell integration, elemental attacks, and triggered SpellForge effects.",
                new[] { ArpgStat30.SpellPower, ArpgStat30.TriggerEnergy, ArpgStat30.CriticalChance, ArpgStat30.FirePower },
                new[] { "Runed Edge", "Spellsteel", "Catalytic Strike", "Echoing Cut", "Elemental Temper", "Linked Assault", "Arcane Riposte", "Perfect Synthesis" });

            AddAscendancy(ArpgAscendancy30.Bastion, ArpgClass30.Warrior, "Bastion",
                "Shields, barriers, retaliation, defensive fields, and absorbed-damage conversion.",
                new[] { ArpgStat30.BarrierStrength, ArpgStat30.MaximumHealth, ArpgStat30.Healing, ArpgStat30.Armour },
                new[] { "Raised Wall", "Held Ground", "Retaliatory Field", "Sheltering Pulse", "Absorbed Force", "Citadel Heart", "No Passage", "Living Bastion" });
        }

        private static void AddAscendancy(
            ArpgAscendancy30 id,
            ArpgClass30 requiredClass,
            string displayName,
            string description,
            ArpgStat30[] stats,
            string[] nodeNames)
        {
            ArpgAscendancyDefinition30 definition = new ArpgAscendancyDefinition30
            {
                id = id,
                requiredClass = requiredClass,
                displayName = displayName,
                description = description
            };

            for (int index = 0; index < nodeNames.Length; index++)
            {
                ArpgStat30 stat = stats[index % stats.Length];
                float amount = IsFlat(stat) ? 8f + index * 2f : 0.025f + index * 0.006f;
                definition.nodes.Add(new ArpgAscendancyNodeDefinition30
                {
                    id = "asc." + id.ToString().ToLowerInvariant() + "." + index.ToString("00"),
                    displayName = nodeNames[index],
                    description = "Advance " + displayName + " through " + Humanize(stat) + ".",
                    modifiers = Mods(new ArpgStatModifier30(stat, amount))
                });
            }

            AscendancyList.Add(definition);
        }

        private static void BuildConstellations()
        {
            AddConstellation(
                "constellation.pyre-crown",
                "Pyre Crown",
                "Fire",
                "Direct Fire, burn pressure, area growth, and reaction preparation.",
                2,
                ArpgStat30.FirePower,
                ArpgStat30.AilmentBuildup,
                new[] { "Kindled Thought", "Ember Vein", "Expanding Heat", "Prepared Ash", "Brand Memory", "Cinder Reach", "Catalyst Flame", "Crownfire", "Sevenfold Spark" },
                "Direct Fire hits intensify existing burns; non-Fire buildup is reduced.");

            AddConstellation(
                "constellation.winters-veil",
                "Winter's Veil",
                "Cold",
                "Chill, freeze preparation, defensive slowing, and persistent Cold fields.",
                2,
                ArpgStat30.ColdPower,
                ArpgStat30.Duration,
                new[] { "Pale Breath", "Still Water", "Deep Chill", "Frozen Margin", "Long Winter", "White Silence", "Brittle Moment", "Veiled Moon", "Absolute Quiet" },
                "Chilled enemies receive increased control buildup; Fire damage is reduced.");

            AddConstellation(
                "constellation.storm-circuit",
                "Storm Circuit",
                "Lightning",
                "Chain retention, Shock, movement casting, and conserved energy.",
                2,
                ArpgStat30.LightningPower,
                ArpgStat30.ChainRetention,
                new[] { "First Current", "Bright Conductor", "Stored Charge", "Branching Route", "Moving Spark", "Conserved Arc", "Storm Memory", "Closed Circuit", "Infinite Relay" },
                "Chain retains more power and cannot revisit previous targets.");

            AddConstellation(
                "constellation.iron-colossus",
                "Iron Colossus",
                "Physical",
                "Armour, stagger, impact damage, and heavy delivery mechanics.",
                2,
                ArpgStat30.PhysicalPower,
                ArpgStat30.Armour,
                new[] { "Iron Seed", "Weighted Step", "Hammer Pulse", "Granite Blood", "Crushing Arc", "Mass Driver", "Unyielding Frame", "Colossus Heart", "Worldweight Form" },
                "Excess Armour contributes to Physical spell impact; movement acceleration is reduced.");

            AddConstellation(
                "constellation.living-bulwark",
                "Living Bulwark",
                "Survival",
                "Life, recovery, resistance pressure, and barrier strength.",
                2,
                ArpgStat30.MaximumHealth,
                ArpgStat30.BarrierStrength,
                new[] { "Living Wall", "Deep Reserve", "Mended Edge", "Shelter Pulse", "Held Breath", "Recovery Loop", "Guardian Skin", "Unbroken Core", "Second Rampart" },
                "Heavy damage grants a temporary barrier with an internal cooldown.");

            AddConstellation(
                "constellation.weavers-hand",
                "Weaver's Hand",
                "SpellForge",
                "Rune capacity, Core efficiency, Link behavior, and board flexibility.",
                3,
                ArpgStat30.RuneCapacity,
                ArpgStat30.ManaEfficiency,
                new[] { "Open Palm", "Careful Socket", "Measured Link", "Core Memory", "Runic Economy", "Touching Stars", "Ordered Weave", "Master Pattern", "The Weaver's Hand" },
                "The first connected Support Rune gains increased effectiveness; disconnected Runes impose a larger penalty.");
        }

        private static void AddConstellation(
            string id,
            string displayName,
            string category,
            string description,
            int attunementCost,
            ArpgStat30 primary,
            ArpgStat30 secondary,
            string[] nodeNames,
            string completionText)
        {
            ArpgConstellationDefinition30 definition = new ArpgConstellationDefinition30
            {
                id = id,
                displayName = displayName,
                category = category,
                description = description,
                attunementCost = attunementCost,
                requiredLevel = 0,
                requiredTier = 0
            };

            string previous = string.Empty;
            for (int index = 0; index < nodeNames.Length; index++)
            {
                ArpgNodeSize30 size = index < 4
                    ? ArpgNodeSize30.Small
                    : index < 7 ? ArpgNodeSize30.Medium : ArpgNodeSize30.Large;
                ArpgStat30 stat = index % 2 == 0 ? primary : secondary;
                float amount = NodeValue(stat, size);
                string nodeId = id + "." + index.ToString("00");
                definition.nodes.Add(new ArpgConstellationNodeDefinition30
                {
                    id = nodeId,
                    displayName = nodeNames[index],
                    description = Humanize(stat) + " " + FormatValue(stat, amount) + ".",
                    size = size,
                    pointCost = size == ArpgNodeSize30.Small ? 1 : size == ArpgNodeSize30.Medium ? 2 : 3,
                    prerequisiteId = previous,
                    modifiers = Mods(new ArpgStatModifier30(stat, amount))
                });
                previous = nodeId;
            }

            definition.nodes.Add(new ArpgConstellationNodeDefinition30
            {
                id = id + ".completion",
                displayName = displayName + " Completion",
                description = completionText,
                size = ArpgNodeSize30.Completion,
                pointCost = 3,
                prerequisiteId = previous,
                modifiers = Mods(
                    new ArpgStatModifier30(primary, NodeValue(primary, ArpgNodeSize30.Large)),
                    new ArpgStatModifier30(secondary, NodeValue(secondary, ArpgNodeSize30.Medium)))
            });
            ConstellationList.Add(definition);
        }

        private static void BuildItemBases()
        {
            AddBase("base.ember-wand", "Ember Wand", ArpgItemSlot30.MainHand, "Wand", ArpgClass30.Mage, 0, ArpgStat30.FirePower, 0.08f);
            AddBase("base.frost-rod", "Frost Rod", ArpgItemSlot30.MainHand, "Wand", ArpgClass30.Mage, 2, ArpgStat30.ColdPower, 0.09f);
            AddBase("base.void-scepter", "Void Scepter", ArpgItemSlot30.MainHand, "Scepter", ArpgClass30.Mage, 4, ArpgStat30.VoidPower, 0.11f);
            AddBase("base.ash-bow", "Ash Bow", ArpgItemSlot30.MainHand, "Bow", ArpgClass30.Ranger, 0, ArpgStat30.ProjectileSpeed, 0.10f);
            AddBase("base.storm-bow", "Storm Bow", ArpgItemSlot30.MainHand, "Bow", ArpgClass30.Ranger, 2, ArpgStat30.LightningPower, 0.09f);
            AddBase("base.hunter-spear", "Hunter Spear", ArpgItemSlot30.MainHand, "Spear", ArpgClass30.Ranger, 4, ArpgStat30.CriticalChance, 0.04f);
            AddBase("base.iron-maul", "Iron Maul", ArpgItemSlot30.MainHand, "Maul", ArpgClass30.Warrior, 0, ArpgStat30.PhysicalPower, 0.09f);
            AddBase("base.rune-blade", "Rune Blade", ArpgItemSlot30.MainHand, "Blade", ArpgClass30.Warrior, 2, ArpgStat30.SpellPower, 0.08f);
            AddBase("base.bastion-hammer", "Bastion Hammer", ArpgItemSlot30.MainHand, "Hammer", ArpgClass30.Warrior, 4, ArpgStat30.BarrierStrength, 0.10f);

            AddBase("base.arcane-focus", "Arcane Focus", ArpgItemSlot30.OffHand, "Focus", ArpgClass30.Mage, 0, ArpgStat30.MaximumMana, 18f);
            AddBase("base.chain-quiver", "Chain Quiver", ArpgItemSlot30.OffHand, "Quiver", ArpgClass30.Ranger, 0, ArpgStat30.ChainRetention, 0.08f);
            AddBase("base.iron-shield", "Iron Shield", ArpgItemSlot30.OffHand, "Shield", ArpgClass30.Warrior, 0, ArpgStat30.Armour, 18f);
            AddBase("base.astral-talisman", "Astral Talisman", ArpgItemSlot30.OffHand, "Talisman", ArpgClass30.Unchosen, 3, ArpgStat30.TriggerEnergy, 8f);

            AddArmourTriplet(ArpgItemSlot30.Helmet, "hood", "Runed Hood", "Scout Hood", "Iron Crown", ArpgStat30.ArcaneWard, ArpgStat30.Evasion, ArpgStat30.Armour);
            AddArmourTriplet(ArpgItemSlot30.BodyArmour, "coat", "Astral Coat", "Pathfinder Coat", "Plate Harness", ArpgStat30.ArcaneWard, ArpgStat30.Evasion, ArpgStat30.Armour);
            AddArmourTriplet(ArpgItemSlot30.Gloves, "gloves", "Scribe Gloves", "Fletcher Gloves", "War Gauntlets", ArpgStat30.MaximumMana, ArpgStat30.CriticalChance, ArpgStat30.PhysicalPower);
            AddArmourTriplet(ArpgItemSlot30.Boots, "boots", "Aether Boots", "Trail Boots", "Siege Greaves", ArpgStat30.MoveSpeed, ArpgStat30.Evasion, ArpgStat30.Armour);
            AddArmourTriplet(ArpgItemSlot30.Belt, "belt", "Sigil Belt", "Hunter Belt", "Fortress Belt", ArpgStat30.TriggerEnergy, ArpgStat30.MaximumHealth, ArpgStat30.BarrierStrength);

            AddBase("base.star-amulet", "Star Amulet", ArpgItemSlot30.Amulet, "Amulet", ArpgClass30.Unchosen, 0, ArpgStat30.SpellPower, 0.05f);
            AddBase("base.attuned-amulet", "Attuned Amulet", ArpgItemSlot30.Amulet, "Amulet", ArpgClass30.Unchosen, 5, ArpgStat30.Attunement, 1f);
            AddBase("base.cinder-ring", "Cinder Ring", ArpgItemSlot30.Ring1, "Ring", ArpgClass30.Unchosen, 0, ArpgStat30.FirePower, 0.04f);
            AddBase("base.tempest-ring", "Tempest Ring", ArpgItemSlot30.Ring1, "Ring", ArpgClass30.Unchosen, 3, ArpgStat30.LightningPower, 0.04f);
            AddBase("base.frost-ring", "Frost Ring", ArpgItemSlot30.Ring2, "Ring", ArpgClass30.Unchosen, 0, ArpgStat30.ColdPower, 0.04f);
            AddBase("base.iron-ring", "Iron Ring", ArpgItemSlot30.Ring2, "Ring", ArpgClass30.Unchosen, 3, ArpgStat30.PhysicalPower, 0.04f);
            AddBase("base.cartographers-relic", "Cartographer's Relic", ArpgItemSlot30.Relic, "Relic", ArpgClass30.Unchosen, 0, ArpgStat30.MapSustain, 0.05f);
            AddBase("base.weavers-relic", "Weaver's Relic", ArpgItemSlot30.Relic, "Relic", ArpgClass30.Unchosen, 2, ArpgStat30.RuneCapacity, 1f);
        }

        private static void AddArmourTriplet(
            ArpgItemSlot30 slot,
            string id,
            string mageName,
            string rangerName,
            string warriorName,
            ArpgStat30 mageStat,
            ArpgStat30 rangerStat,
            ArpgStat30 warriorStat)
        {
            AddBase("base." + id + ".mage", mageName, slot, slot.ToString(), ArpgClass30.Mage, 0, mageStat, IsFlat(mageStat) ? 14f : 0.05f);
            AddBase("base." + id + ".ranger", rangerName, slot, slot.ToString(), ArpgClass30.Ranger, 0, rangerStat, IsFlat(rangerStat) ? 14f : 0.05f);
            AddBase("base." + id + ".warrior", warriorName, slot, slot.ToString(), ArpgClass30.Warrior, 0, warriorStat, IsFlat(warriorStat) ? 14f : 0.05f);
        }

        private static void AddBase(
            string id,
            string name,
            ArpgItemSlot30 slot,
            string family,
            ArpgClass30 affinity,
            int requiredLevel,
            ArpgStat30 stat,
            float value)
        {
            ItemBaseList.Add(new ArpgItemBaseDefinition30
            {
                id = id,
                displayName = name,
                slot = slot,
                family = family,
                affinity = affinity,
                requiredLevel = requiredLevel,
                implicitModifiers = Mods(new ArpgStatModifier30(stat, value))
            });
        }

        private static void BuildAffixes()
        {
            ArpgItemSlot30[] all = (ArpgItemSlot30[])Enum.GetValues(typeof(ArpgItemSlot30));
            ArpgItemSlot30[] weapons = { ArpgItemSlot30.MainHand, ArpgItemSlot30.OffHand };
            ArpgItemSlot30[] armour = { ArpgItemSlot30.Helmet, ArpgItemSlot30.BodyArmour, ArpgItemSlot30.Gloves, ArpgItemSlot30.Boots, ArpgItemSlot30.Belt };
            ArpgItemSlot30[] jewellery = { ArpgItemSlot30.Amulet, ArpgItemSlot30.Ring1, ArpgItemSlot30.Ring2, ArpgItemSlot30.Relic };
            ArpgItemSlot30[] weaponJewellery = weapons.Concat(jewellery).ToArray();

            AddAffixFamily("potency", "Potency", true, ArpgStat30.SpellPower, 0.02f, 0.15f, all, false);
            AddAffixFamily("vitality", "Vitality", true, ArpgStat30.MaximumHealth, 8f, 75f, all, false);
            AddAffixFamily("reservoir", "Reservoir", true, ArpgStat30.MaximumMana, 7f, 65f, all, false);
            AddAffixFamily("precision", "Precision", true, ArpgStat30.CriticalChance, 0.01f, 0.08f, weaponJewellery, false);
            AddAffixFamily("armour", "Armour", true, ArpgStat30.Armour, 8f, 80f, armour.Concat(new[] { ArpgItemSlot30.OffHand }).ToArray(), true);
            AddAffixFamily("evasion", "Evasion", true, ArpgStat30.Evasion, 8f, 80f, armour, true);
            AddAffixFamily("ward", "Arcane Ward", true, ArpgStat30.ArcaneWard, 8f, 80f, armour.Concat(jewellery).ToArray(), true);
            AddAffixFamily("flame", "Flame", true, ArpgStat30.FirePower, 0.02f, 0.16f, weaponJewellery, false);
            AddAffixFamily("frost", "Frost", true, ArpgStat30.ColdPower, 0.02f, 0.16f, weaponJewellery, false);
            AddAffixFamily("storm", "Storm", true, ArpgStat30.LightningPower, 0.02f, 0.16f, weaponJewellery, false);
            AddAffixFamily("impact", "Impact", true, ArpgStat30.PhysicalPower, 0.02f, 0.16f, weaponJewellery, false);
            AddAffixFamily("acceleration", "Acceleration", false, ArpgStat30.MoveSpeed, 0.06f, 0.55f, new[] { ArpgItemSlot30.Boots, ArpgItemSlot30.MainHand, ArpgItemSlot30.Ring1, ArpgItemSlot30.Ring2 }, false);
            AddAffixFamily("tempo", "Temporal Recovery", false, ArpgStat30.CooldownRecovery, 0.012f, 0.12f, weaponJewellery, false);
            AddAffixFamily("efficiency", "Mana Efficiency", false, ArpgStat30.ManaEfficiency, 0.012f, 0.13f, all, false);
            AddAffixFamily("restoration", "Restoration", false, ArpgStat30.Healing, 0.02f, 0.16f, armour.Concat(jewellery).ToArray(), false);
            AddAffixFamily("chain", "Chain Retention", false, ArpgStat30.ChainRetention, 0.02f, 0.15f, weaponJewellery, false);
            AddAffixFamily("fortune", "Fortune", false, ArpgStat30.ItemRarity, 0.01f, 0.09f, jewellery, false);
            AddAffixFamily("cartography", "Cartography", false, ArpgStat30.MapSustain, 0.01f, 0.09f, new[] { ArpgItemSlot30.Relic, ArpgItemSlot30.Amulet, ArpgItemSlot30.Belt }, false);
        }

        private static void AddAffixFamily(
            string id,
            string name,
            bool prefix,
            ArpgStat30 stat,
            float minimum,
            float maximum,
            ArpgItemSlot30[] slots,
            bool local)
        {
            for (int tier = 1; tier <= 4; tier++)
            {
                float low = Mathf.Lerp(minimum, maximum, (tier - 1) / 4f);
                float high = Mathf.Lerp(minimum, maximum, tier / 4f);
                AffixList.Add(new ArpgAffixDefinition30
                {
                    id = "affix." + id + ".t" + tier,
                    displayName = name + " T" + tier,
                    prefix = prefix,
                    stat = stat,
                    minimum = low,
                    maximum = high,
                    minimumItemLevel = (tier - 1) * 3,
                    weight = Mathf.Max(10, 130 - tier * 22),
                    family = id,
                    conflictGroup = id,
                    local = local,
                    validSlots = slots.ToList()
                });
            }
        }

        private static void BuildMonsterFamilies()
        {
            MonsterFamilyList.Add(new ArpgMonsterFamilyDefinition31
            {
                id = "family.ashbound",
                displayName = "Ashbound",
                description = "Aggressive Fire-aligned attackers using charges, explosions, and burning areas.",
                elementTheme = "Fire",
                archetypeHints = new List<string> { "brute", "charger", "melee", "ember", "fire" }
            });

            MonsterFamilyList.Add(new ArpgMonsterFamilyDefinition31
            {
                id = "family.mirekin",
                displayName = "Mirekin",
                description = "Toxic and Blood-aligned packs using damage over time, leech, and persistent ground pressure.",
                elementTheme = "Toxic/Blood",
                archetypeHints = new List<string> { "swarm", "toxic", "blood", "spitter", "leech" }
            });

            MonsterFamilyList.Add(new ArpgMonsterFamilyDefinition31
            {
                id = "family.astral-constructs",
                displayName = "Astral Constructs",
                description = "Cold, Lightning, and Void constructs using ranged attacks, barriers, beams, and area denial.",
                elementTheme = "Cold/Lightning/Void",
                archetypeHints = new List<string> { "construct", "ranged", "caster", "warden", "sentinel" }
            });
        }

        private static void BuildMonsterVariants()
        {
            AddVariant("variant.ashbound.cinderling", "Cinderling", "family.ashbound", "Skirmisher", "Cinder Rush", 0.78f, 0.82f, 1.22f, "melee", "charger", "ember");
            AddVariant("variant.ashbound.ash-spewer", "Ash Spewer", "family.ashbound", "Ranged", "Ash Volley", 0.9f, 1.05f, 0.92f, "ranged", "caster", "fire");
            AddVariant("variant.ashbound.ironbrand", "Ironbrand", "family.ashbound", "Bruiser", "Brand Slam", 1.35f, 1.18f, 0.74f, "brute", "melee");
            AddVariant("variant.ashbound.flame-binder", "Flame Binder", "family.ashbound", "Support", "Kindling Ward", 0.95f, 0.8f, 0.9f, "caster", "support");
            AddVariant("variant.ashbound.cinder-charger", "Cinder Charger", "family.ashbound", "Specialist", "Explosive Charge", 1.08f, 1.12f, 1.1f, "charger", "beast");
            AddVariant("variant.ashbound.pyre-elite", "Pyre Exactor", "family.ashbound", "Elite", "Funeral Detonation", 1.55f, 1.35f, 1.04f, "warden", "brute");

            AddVariant("variant.mirekin.bogling", "Bogling", "family.mirekin", "Skirmisher", "Venom Bite", 0.72f, 0.78f, 1.2f, "swarm", "melee");
            AddVariant("variant.mirekin.mire-spitter", "Mire Spitter", "family.mirekin", "Ranged", "Toxic Spit", 0.86f, 1.02f, 0.92f, "spitter", "ranged", "toxic");
            AddVariant("variant.mirekin.blood-leech", "Blood Leech", "family.mirekin", "Sustainer", "Sanguine Drain", 1f, 0.96f, 1.08f, "leech", "blood");
            AddVariant("variant.mirekin.rot-shaman", "Rot Shaman", "family.mirekin", "Support", "Mire Bloom", 0.92f, 0.86f, 0.86f, "caster", "support", "toxic");
            AddVariant("variant.mirekin.root-hulk", "Root Hulk", "family.mirekin", "Bruiser", "Root Crush", 1.5f, 1.2f, 0.68f, "brute", "beast");
            AddVariant("variant.mirekin.heart-elite", "Heart Devourer", "family.mirekin", "Elite", "Leeching Roots", 1.62f, 1.3f, 0.9f, "warden", "leech");

            AddVariant("variant.astral.spark-drone", "Spark Drone", "family.astral-constructs", "Skirmisher", "Static Dash", 0.74f, 0.82f, 1.25f, "construct", "swift");
            AddVariant("variant.astral.lens-caster", "Lens Caster", "family.astral-constructs", "Ranged", "Prismatic Beam", 0.88f, 1.08f, 0.88f, "caster", "ranged");
            AddVariant("variant.astral.ward-pylon", "Ward Pylon", "family.astral-constructs", "Support", "Astral Barrier", 1.12f, 0.68f, 0.55f, "construct", "support");
            AddVariant("variant.astral.frost-sentinel", "Frost Sentinel", "family.astral-constructs", "Controller", "Frozen Lock", 1.18f, 1f, 0.78f, "sentinel", "cold");
            AddVariant("variant.astral.void-engine", "Void Engine", "family.astral-constructs", "Bruiser", "Compression Field", 1.46f, 1.18f, 0.66f, "construct", "brute", "void");
            AddVariant("variant.astral.conduit-elite", "Conduit Arbiter", "family.astral-constructs", "Elite", "Chain Pylons", 1.58f, 1.34f, 0.94f, "warden", "sentinel");
        }

        private static void AddVariant(
            string id,
            string name,
            string family,
            string role,
            string ability,
            float health,
            float damage,
            float speed,
            params string[] hints)
        {
            MonsterVariantList.Add(new ArpgMonsterVariantDefinition31
            {
                id = id,
                displayName = name,
                familyId = family,
                role = role,
                signatureAbility = ability,
                healthMultiplier = health,
                damageMultiplier = damage,
                speedMultiplier = speed,
                archetypeHints = hints.ToList()
            });
        }

        private static void BuildBosses()
        {
            AddBoss("boss.ember-warden", "Ember Warden", "family.ashbound", "Fire", "Cinder Court", "Defeat the Warden after surviving its expanding ignition rings.",
                "Ignition Ring", "Cinder Charge", "Ashfall", "Overheat Phase");
            AddBoss("boss.frostbound-matron", "Frostbound Matron", "family.astral-constructs", "Cold", "White Reliquary", "Defeat the Matron without being caught by three consecutive frost waves.",
                "Frost Wave", "Frozen Mirrors", "Shatter Line", "Absolute Zero Phase");
            AddBoss("boss.stormcoil-behemoth", "Stormcoil Behemoth", "family.astral-constructs", "Lightning", "Storm Conduit", "Defeat the Behemoth while the map has at least one modifier.",
                "Chain Pylons", "Static Rush", "Thunder Mark", "Stormcoil Phase");
            AddBoss("boss.bone-regent", "Bone Regent", "family.ashbound", "Physical/Blood", "Quiet Ossuary", "Defeat the Regent before its third sovereign summons.",
                "Bone Lance", "Regent's Guard", "Blood Decree", "Sovereign Phase");
            AddBoss("boss.mireheart", "Mireheart", "family.mirekin", "Toxic/Blood", "Drowned Heart", "Defeat Mireheart on a Magic or Rare map.",
                "Toxic Bloom", "Leeching Roots", "Mire Surge", "Heart Rupture");
            AddBoss("boss.astral-sentinel", "Astral Sentinel", "family.astral-constructs", "Void/Lightning", "Pale Observatory", "Defeat the Sentinel on a Rare map in under six minutes.",
                "Void Beam", "Astral Barrier", "Orbiting Sigils", "Final Geometry");
        }

        private static void AddBoss(
            string id,
            string name,
            string family,
            string element,
            string arena,
            string mastery,
            params string[] mechanics)
        {
            BossList.Add(new ArpgBossDefinition31
            {
                id = id,
                displayName = name,
                familyId = family,
                elementTheme = element,
                arenaTheme = arena,
                masteryText = mastery,
                mechanics = mechanics.ToList()
            });
        }

        private static void BuildMaps()
        {
            string[] names =
            {
                "Ashen Causeway", "Cinder Grove",
                "Frostbitten Hollow", "Quiet Ossuary",
                "Broken Arcade", "Flooded Archive",
                "Iron Orchard", "Forgotten Quarry",
                "Glass Marsh", "Pale Observatory",
                "Astral Menagerie", "Warden's Reach"
            };

            string[] bosses =
            {
                "boss.ember-warden", "boss.ember-warden",
                "boss.frostbound-matron", "boss.bone-regent",
                "boss.stormcoil-behemoth", "boss.mireheart",
                "boss.bone-regent", "boss.stormcoil-behemoth",
                "boss.mireheart", "boss.astral-sentinel",
                "boss.astral-sentinel", "boss.astral-sentinel"
            };

            string[] families =
            {
                "family.ashbound", "family.ashbound",
                "family.astral-constructs", "family.ashbound",
                "family.astral-constructs", "family.mirekin",
                "family.ashbound", "family.astral-constructs",
                "family.mirekin", "family.astral-constructs",
                "family.mirekin", "family.astral-constructs"
            };

            string[] masteryRules =
            {
                "boss", "boss",
                "no-death", "no-death",
                "magic", "magic",
                "modified", "modified",
                "rare", "rare",
                "rare-timed", "rare-timed"
            };

            for (int index = 0; index < 12; index++)
            {
                int tier = index / 2;
                ArpgBossDefinition31 boss = Boss(bosses[index]);
                MapList.Add(new ArpgMapDefinition30
                {
                    id = "map.white." + index.ToString("00"),
                    displayName = names[index],
                    tier = tier,
                    region = "The Pale Verge",
                    bossName = boss == null ? "Unknown Guardian" : boss.displayName,
                    bossId = bosses[index],
                    monsterFamily = families[index],
                    environmentHint = tier < 2 ? "foundation" : tier < 4 ? "formation" : "mastery",
                    masteryRule = masteryRules[index],
                    masteryDescription = boss == null ? "Defeat the map guardian." : boss.masteryText,
                    rewardFocus = index % 3 == 0 ? "Equipment" : index % 3 == 1 ? "Currency" : "Spell Discovery",
                    layoutIndex = index % 6,
                    nodeIndex = index,
                    playableIn31 = true
                });
            }

            for (int index = 0; index < 12; index++)
            {
                ArpgMapDefinition30 map = MapList[index];
                int tier = map.tier;
                if (index % 2 == 0) map.connectedMapIds.Add(MapList[index + 1].id);
                else map.connectedMapIds.Add(MapList[index - 1].id);

                if (tier > 0)
                {
                    map.connectedMapIds.Add(MapList[(tier - 1) * 2].id);
                    map.connectedMapIds.Add(MapList[(tier - 1) * 2 + 1].id);
                }

                if (tier < 5)
                {
                    map.connectedMapIds.Add(MapList[(tier + 1) * 2].id);
                    map.connectedMapIds.Add(MapList[(tier + 1) * 2 + 1].id);
                }

                map.connectedMapIds = map.connectedMapIds.Distinct().ToList();
            }

            string[] futureNames =
            {
                "Storm Gallery", "Blue Labyrinth", "Venom Basilica", "Drowned Conduit",
                "Howling Rampart", "Cracked Reservoir", "Moonlit Foundry", "Sapphire Necropolis",
                "Gilded Chasm", "Blood Meridian", "Toxic Conservatory", "Sunken Citadel",
                "Runebound Steppe", "Mechanical Choir", "Fracture Palace", "Titan's Spine",
                "Crimson Firmament", "Black Star Vault", "Final Crucible", "Rift Cathedral",
                "Worldbreak Engine", "Sanguine Horizon", "Stormgrave", "Ashen Throne",
                "Void Meridian", "The Last Geometry"
            };

            for (int tier = 6; tier < 40; tier++)
            {
                for (int variant = 0; variant < 2; variant++)
                {
                    int sequence = (tier - 6) * 2 + variant;
                    string name = futureNames[sequence % futureNames.Length] + (variant == 0 ? string.Empty : " · Echo");
                    MapList.Add(new ArpgMapDefinition30
                    {
                        id = "map.future.t" + tier.ToString("00") + ".v" + variant,
                        displayName = name,
                        tier = tier,
                        region = tier < 10 ? "The Pale Verge" : tier < 20 ? "The Azure Expanse" : tier < 30 ? "The Gilded Fracture" : "The Crimson Beyond",
                        bossName = "Unreleased Guardian",
                        bossId = "future",
                        monsterFamily = MonsterFamilyList[(tier + variant) % MonsterFamilyList.Count].id,
                        environmentHint = "future-band",
                        masteryRule = "future",
                        masteryDescription = "This map is represented in the forty-tier data model but is not playable in 3.1.0.",
                        rewardFocus = "Future",
                        layoutIndex = sequence % 6,
                        nodeIndex = 12 + sequence,
                        playableIn31 = false
                    });
                }
            }
        }

        private static void BuildMapAffixes()
        {
            AddMapAffix("frenzied", "Frenzied Host", "Enemies move and attack more aggressively.", 2, 1.12f, "frenziedEnemies");
            AddMapAffix("bulwark", "Bulwark Host", "Enemies have stronger defensive scaling.", 2, 1.15f, "bulwarkEnemies");
            AddMapAffix("glass-soul", "Glass Soul", "Both incoming danger and rewards are amplified.", 2, 1.18f, "glassSoul");
            AddMapAffix("mana-drought", "Mana Drought", "Mana pressure is increased.", 2, 1.14f, "manaDrought");
            AddMapAffix("elite-host", "Elite Host", "Additional Magic and Rare properties appear.", 3, 1.18f, "extraEliteAffixes");
            AddMapAffix("scarred-recovery", "Scarred Recovery", "Healing and recovery are reduced.", 3, 1.16f, "reducedHealing");
            AddMapAffix("unstable-world", "Unstable World", "Spell instability is increased.", 3, 1.16f, "unstableWorld");
            AddMapAffix("adaptive", "Adaptive Enemies", "Enemies respond to repeated tactics.", 4, 1.20f, "adaptiveEnemies");
            AddMapAffix("timed", "Collapsing Map", "The encounter applies escalating time pressure.", 4, 1.22f, "timedRooms");
            AddMapAffix("no-starting-gear", "Bare Entry", "Legacy equipment effects are suppressed inside the map.", 4, 1.24f, "noStartingEquipment");
            AddMapAffix("no-starting-runes", "Unwritten Entry", "Legacy starting modifier effects are suppressed.", 5, 1.24f, "noStartingModifiers");
            AddMapAffix("boss-phase", "Awakened Guardian", "The map guardian gains an additional behavior phase.", 5, 1.28f, "newBossPhase");
        }

        private static void AddMapAffix(string id, string name, string description, int minimumTier, float multiplier, string flag)
        {
            MapAffixList.Add(new ArpgMapAffixDefinition30
            {
                id = "map-affix." + id,
                displayName = name,
                description = description,
                minimumTier = minimumTier,
                rewardMultiplier = multiplier,
                difficultyFlag = flag
            });
        }

        private static List<ArpgStatModifier30> Mods(params ArpgStatModifier30[] values)
        {
            return values == null ? new List<ArpgStatModifier30>() : values.ToList();
        }

        private static bool IsFlat(ArpgStat30 stat)
        {
            return stat == ArpgStat30.MaximumHealth ||
                   stat == ArpgStat30.MaximumMana ||
                   stat == ArpgStat30.TriggerEnergy ||
                   stat == ArpgStat30.Attunement ||
                   stat == ArpgStat30.Armour ||
                   stat == ArpgStat30.Evasion ||
                   stat == ArpgStat30.ArcaneWard ||
                   stat == ArpgStat30.RuneCapacity;
        }

        private static float NodeValue(ArpgStat30 stat, ArpgNodeSize30 size)
        {
            if (IsFlat(stat))
            {
                if (size == ArpgNodeSize30.Small) return stat == ArpgStat30.RuneCapacity ? 1f : 8f;
                if (size == ArpgNodeSize30.Medium) return stat == ArpgStat30.RuneCapacity ? 1f : 14f;
                return stat == ArpgStat30.RuneCapacity ? 2f : 24f;
            }

            if (size == ArpgNodeSize30.Small) return 0.025f;
            if (size == ArpgNodeSize30.Medium) return 0.05f;
            return 0.09f;
        }

        private static string Humanize(ArpgStat30 stat)
        {
            string value = stat.ToString();
            return System.Text.RegularExpressions.Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");
        }

        private static string FormatValue(ArpgStat30 stat, float value)
        {
            return IsFlat(stat) ? "+" + Mathf.RoundToInt(value) : "+" + Mathf.RoundToInt(value * 100f) + "%";
        }

        public static string Validate()
        {
            Ensure();
            List<string> failures = new List<string>();
            if (Classes.Count != 3) failures.Add("Expected exactly 3 classes.");
            if (Ascendancies.Count != 9) failures.Add("Expected exactly 9 Ascendancies.");
            if (Constellations.Count != 6) failures.Add("Expected exactly 6 playable starter Constellations.");
            if (Constellations.Sum(value => value.nodes.Count) < 60) failures.Add("Expected at least 60 implemented Constellation Stars.");
            if (ItemBases.Count < 36) failures.Add("Expected at least 36 item bases, found " + ItemBases.Count + ".");
            if (Affixes.Count < 72) failures.Add("Expected at least 72 tiered affixes, found " + Affixes.Count + ".");
            if (Maps.Count != 80) failures.Add("Expected exactly 80 map definitions in the forty-tier model.");
            if (Maps.Count(value => value.playableIn31) != 12) failures.Add("Expected exactly 12 playable White Map nodes.");
            if (Maps.Where(value => value.playableIn31).Select(value => value.tier).Distinct().Count() != 6) failures.Add("Expected playable tiers 0–5.");
            if (MapAffixes.Count < 12) failures.Add("Expected at least 12 map modifiers.");
            if (MonsterFamilies.Count != 3) failures.Add("Expected exactly 3 monster families.");
            if (MonsterVariants.Count != 18) failures.Add("Expected exactly 18 implemented enemy variants.");
            if (MonsterVariants.GroupBy(value => value.familyId).Any(group => group.Count() != 6)) failures.Add("Each monster family must contain exactly six variants.");
            if (Bosses.Count != 6) failures.Add("Expected exactly 6 White Map bosses.");
            if (Constellations.Select(value => value.id).Distinct().Count() != Constellations.Count) failures.Add("Constellation IDs are not unique.");
            if (Maps.Select(value => value.id).Distinct().Count() != Maps.Count) failures.Add("Map IDs are not unique.");
            if (Affixes.Select(value => value.id).Distinct().Count() != Affixes.Count) failures.Add("Affix IDs are not unique.");
            return failures.Count == 0 ? string.Empty : string.Join("\n", failures.ToArray());
        }
    }
}
