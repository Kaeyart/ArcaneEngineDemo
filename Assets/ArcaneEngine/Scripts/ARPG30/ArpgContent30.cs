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
        public string starterCoreHint;
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
        public string environmentHint;
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

        public static IReadOnlyList<ArpgClassDefinition30> Classes { get { Ensure(); return ClassList; } }
        public static IReadOnlyList<ArpgAscendancyDefinition30> Ascendancies { get { Ensure(); return AscendancyList; } }
        public static IReadOnlyList<ArpgConstellationDefinition30> Constellations { get { Ensure(); return ConstellationList; } }
        public static IReadOnlyList<ArpgItemBaseDefinition30> ItemBases { get { Ensure(); return ItemBaseList; } }
        public static IReadOnlyList<ArpgAffixDefinition30> Affixes { get { Ensure(); return AffixList; } }
        public static IReadOnlyList<ArpgMapDefinition30> Maps { get { Ensure(); return MapList; } }
        public static IReadOnlyList<ArpgMapAffixDefinition30> MapAffixes { get { Ensure(); return MapAffixList; } }

        public static void Ensure()
        {
            if (_ready) return;
            _ready = true;
            BuildClasses();
            BuildAscendancies();
            BuildConstellations();
            BuildItemBases();
            BuildAffixes();
            BuildMaps();
            BuildMapAffixes();
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

        private static void BuildClasses()
        {
            ClassList.Add(new ArpgClassDefinition30
            {
                id = ArpgClass30.Mage,
                displayName = "Mage",
                description = "Mana, constructed spells, elemental precision and complex Rune arrangements.",
                starterCoreHint = "bolt, fireball, orb, projectile",
                baseModifiers = Mods(new ArpgStatModifier30(ArpgStat30.MaximumMana, 25f), new ArpgStatModifier30(ArpgStat30.SpellPower, 0.12f)),
                ascendancies = new List<ArpgAscendancy30> { ArpgAscendancy30.Elementalist, ArpgAscendancy30.Chronomancer, ArpgAscendancy30.Voidcaller }
            });
            ClassList.Add(new ArpgClassDefinition30
            {
                id = ArpgClass30.Ranger,
                displayName = "Ranger",
                description = "Precision, mobility, projectiles, Chain routing and controlled terrain.",
                starterCoreHint = "arrow, shot, spear, projectile",
                baseModifiers = Mods(new ArpgStatModifier30(ArpgStat30.MoveSpeed, 0.8f), new ArpgStatModifier30(ArpgStat30.CriticalChance, 0.035f)),
                ascendancies = new List<ArpgAscendancy30> { ArpgAscendancy30.Deadeye, ArpgAscendancy30.Stormrunner, ArpgAscendancy30.Warden }
            });
            ClassList.Add(new ArpgClassDefinition30
            {
                id = ArpgClass30.Warrior,
                displayName = "Warrior",
                description = "Life, armour, melee delivery, impact, barriers and weapon-spell integration.",
                starterCoreHint = "strike, slam, wave, nova",
                baseModifiers = Mods(new ArpgStatModifier30(ArpgStat30.MaximumHealth, 30f), new ArpgStatModifier30(ArpgStat30.Healing, 0.08f)),
                ascendancies = new List<ArpgAscendancy30> { ArpgAscendancy30.Juggernaut, ArpgAscendancy30.Spellblade, ArpgAscendancy30.Bastion }
            });
        }

        private static void BuildAscendancies()
        {
            AddAscendancy(ArpgAscendancy30.Elementalist, ArpgClass30.Mage, "Elementalist", "Buildup, reactions, fields and controlled elemental cascades.",
                new[] { ArpgStat30.SpellPower, ArpgStat30.CooldownRecovery, ArpgStat30.TriggerEnergy, ArpgStat30.ManaEfficiency },
                new[] { "Prismatic Initiate", "Catalyst Authority", "Prepared Cascade", "Elemental Dominion", "Field Alchemist", "Threshold Savant", "Conserved Proliferation", "Crown of Seven" });
            AddAscendancy(ArpgAscendancy30.Chronomancer, ArpgClass30.Mage, "Chronomancer", "Delay, Echo, Return, cooldown manipulation and repeated spell phases.",
                new[] { ArpgStat30.CooldownRecovery, ArpgStat30.ManaEfficiency, ArpgStat30.TriggerEnergy, ArpgStat30.MoveSpeed },
                new[] { "Second Hand", "Borrowed Moment", "Echo Chamber", "Reversal", "Stored Release", "Temporal Shelter", "Causal Loop", "Hour Without End" });
            AddAscendancy(ArpgAscendancy30.Voidcaller, ArpgClass30.Mage, "Voidcaller", "Compression, spatial control, Corruption and unstable high-risk casting.",
                new[] { ArpgStat30.SpellPower, ArpgStat30.MaximumMana, ArpgStat30.MaximumHealth, ArpgStat30.TriggerEnergy },
                new[] { "Hollow Spark", "Gravitic Script", "Rift Memory", "Controlled Collapse", "Abyssal Reserve", "Event Horizon", "Black Geometry", "Voice Beyond" });

            AddAscendancy(ArpgAscendancy30.Deadeye, ArpgClass30.Ranger, "Deadeye", "Projectile geometry, precision, Split, Pierce and distance control.",
                new[] { ArpgStat30.CriticalChance, ArpgStat30.MoveSpeed, ArpgStat30.SpellPower, ArpgStat30.ItemRarity },
                new[] { "True Line", "Far Shot", "Split Calculation", "Ricochet Memory", "Weak Point", "Unerring Release", "Terminal Velocity", "One Perfect Trajectory" });
            AddAscendancy(ArpgAscendancy30.Stormrunner, ArpgClass30.Ranger, "Stormrunner", "Lightning, Chain-energy retention, momentum and movement casting.",
                new[] { ArpgStat30.MoveSpeed, ArpgStat30.CooldownRecovery, ArpgStat30.TriggerEnergy, ArpgStat30.CriticalChance },
                new[] { "Static Step", "Conductive Route", "Momentum Cast", "Returning Current", "Arc Sprint", "No Revisit", "Storm Circuit", "Living Lightning" });
            AddAscendancy(ArpgAscendancy30.Warden, ArpgClass30.Ranger, "Warden", "Placed mechanisms, persistent control, Toxic and Cold territory.",
                new[] { ArpgStat30.MaximumHealth, ArpgStat30.Healing, ArpgStat30.MapQuantity, ArpgStat30.SpellPower },
                new[] { "Rune Anchor", "Cold Snare", "Venom Reservoir", "Moving Territory", "Contained Hunt", "Patient Engine", "Wild Mechanism", "Keeper of the Verge" });

            AddAscendancy(ArpgAscendancy30.Juggernaut, ArpgClass30.Warrior, "Juggernaut", "Armour, impact, Broken, mass and unstoppable movement.",
                new[] { ArpgStat30.MaximumHealth, ArpgStat30.Healing, ArpgStat30.SpellPower, ArpgStat30.MoveSpeed },
                new[] { "Mass Driver", "Unbroken Pace", "Trauma Engine", "Pressure Wave", "Iron Recovery", "Seismic Script", "Immovable", "Worldweight" });
            AddAscendancy(ArpgAscendancy30.Spellblade, ArpgClass30.Warrior, "Spellblade", "Weapon-spell alternation, elemental imbuement and close-range triggers.",
                new[] { ArpgStat30.SpellPower, ArpgStat30.CriticalChance, ArpgStat30.TriggerEnergy, ArpgStat30.ManaEfficiency },
                new[] { "Runed Edge", "Stored Element", "Alternating Rhythm", "Imbued Impact", "Close Conduit", "Weapon Memory", "Twin Discipline", "Living Arsenal" });
            AddAscendancy(ArpgAscendancy30.Bastion, ArpgClass30.Warrior, "Bastion", "Barrier, retaliation, defensive fields and converted prevention.",
                new[] { ArpgStat30.MaximumHealth, ArpgStat30.Healing, ArpgStat30.TriggerEnergy, ArpgStat30.MapSustain },
                new[] { "Raised Wall", "Retaliatory Script", "Sheltering Field", "Stored Prevention", "Unyielding Core", "Ward Relay", "Citadel Pulse", "The Last Refuge" });
        }

        private static void AddAscendancy(ArpgAscendancy30 id, ArpgClass30 requiredClass, string name, string description, ArpgStat30[] stats, string[] nodeNames)
        {
            ArpgAscendancyDefinition30 definition = new ArpgAscendancyDefinition30
            {
                id = id,
                requiredClass = requiredClass,
                displayName = name,
                description = description
            };
            for (int index = 0; index < nodeNames.Length; index++)
            {
                ArpgStat30 stat = stats[index % stats.Length];
                float value = AscendancyValue(stat, index);
                definition.nodes.Add(new ArpgAscendancyNodeDefinition30
                {
                    id = "asc." + id.ToString().ToLowerInvariant() + "." + index,
                    displayName = nodeNames[index],
                    description = AscendancyDescription(nodeNames[index], stat, value, index == nodeNames.Length - 1),
                    modifiers = Mods(new ArpgStatModifier30(stat, value))
                });
            }
            AscendancyList.Add(definition);
        }

        private static float AscendancyValue(ArpgStat30 stat, int index)
        {
            float multiplier = index == 7 ? 2.25f : index >= 4 ? 1.45f : 1f;
            return BaseNodeValue(stat, ArpgNodeSize30.Large) * multiplier;
        }

        private static string AscendancyDescription(string name, ArpgStat30 stat, float value, bool capstone)
        {
            return (capstone ? "Ascendancy capstone. " : string.Empty) + name + " grants " + FormatStat(stat, value) + ".";
        }

        private static void BuildConstellations()
        {
            AddThemeConstellations("Elemental", new[]
            {
                Theme("burning-crown", "The Burning Crown", ArpgStat30.SpellPower, ArpgStat30.TriggerEnergy, "Excess Fire preparation is conserved for the next direct application."),
                Theme("winter-wolf", "The Winter Wolf", ArpgStat30.MaximumHealth, ArpgStat30.CooldownRecovery, "Prepared Cold targets retain part of their setup after a controlled Shatter."),
                Theme("storm-serpent", "The Storm Serpent", ArpgStat30.CriticalChance, ArpgStat30.TriggerEnergy, "Chain routes retain more authority without removing generation limits."),
                Theme("iron-titan", "The Iron Titan", ArpgStat30.MaximumHealth, ArpgStat30.SpellPower, "Heavy impacts gain force from defensive investment."),
                Theme("crimson-heart", "The Crimson Heart", ArpgStat30.Healing, ArpgStat30.SpellPower, "Recovery and Blood expenditure reinforce one another."),
                Theme("venom-orchid", "The Venom Orchid", ArpgStat30.MapQuantity, ArpgStat30.SpellPower, "Persistent Toxic effects trade pulse speed for stronger controlled applications."),
                Theme("hollow-star", "The Hollow Star", ArpgStat30.MaximumMana, ArpgStat30.TriggerEnergy, "Void effects gain power from deliberate termination rather than recursion.")
            });

            AddThemeConstellations("Rune", new[]
            {
                Theme("chain-conductor", "The Chain Conductor", ArpgStat30.TriggerEnergy, ArpgStat30.CriticalChance, "The final Chain target refunds part of the route budget."),
                Theme("split-prism", "The Split Prism", ArpgStat30.SpellPower, ArpgStat30.ItemRarity, "Split children share power more efficiently but remain under one lineage."),
                Theme("returning-comet", "The Returning Comet", ArpgStat30.CooldownRecovery, ArpgStat30.SpellPower, "Returning spells gain a stronger final homeward contact."),
                Theme("orbiting-moons", "The Orbiting Moons", ArpgStat30.MaximumMana, ArpgStat30.TriggerEnergy, "Orbiters store authority and release it in bounded volleys."),
                Theme("patient-hourglass", "The Patient Hourglass", ArpgStat30.CooldownRecovery, ArpgStat30.ManaEfficiency, "Delayed spells gain power for each distinct phase they wait."),
                Theme("enduring-circle", "The Enduring Circle", ArpgStat30.MaximumHealth, ArpgStat30.MapSustain, "Explicit Persistent fields receive priority over secondary residue."),
                Theme("mirror-gate", "The Mirror Gate", ArpgStat30.TriggerEnergy, ArpgStat30.ManaEfficiency, "Spell Links use less trigger energy when connecting different deliveries."),
                Theme("barrier-ram", "The Barrier Ram", ArpgStat30.MaximumHealth, ArpgStat30.SpellPower, "Prevented damage contributes to the next direct impact.")
            });

            AddThemeConstellations("Defence", new[]
            {
                Theme("great-oak", "The Great Oak", ArpgStat30.MaximumHealth, ArpgStat30.Healing, "Large hits start a short recovery window."),
                Theme("silver-shell", "The Silver Shell", ArpgStat30.MaximumHealth, ArpgStat30.ManaEfficiency, "Resource efficiency rises while recently damaged."),
                Theme("wandering-hare", "The Wandering Hare", ArpgStat30.MoveSpeed, ArpgStat30.CooldownRecovery, "Movement recovery improves after avoiding danger."),
                Theme("deep-well", "The Deep Well", ArpgStat30.MaximumMana, ArpgStat30.ManaEfficiency, "High mana reserves improve controlled casting."),
                Theme("phoenix-feather", "The Phoenix Feather", ArpgStat30.Healing, ArpgStat30.MaximumHealth, "Map completion restores additional resources."),
                Theme("quiet-mountain", "The Quiet Mountain", ArpgStat30.MaximumHealth, ArpgStat30.MapSustain, "Defensive map completion has improved sustain.")
            });

            AddThemeConstellations("Weapon", new[]
            {
                Theme("archers-eye", "The Archer's Eye", ArpgStat30.CriticalChance, ArpgStat30.MoveSpeed, "Distance improves precision without granting free projectile copies."),
                Theme("duelist-ribbon", "The Duelist's Ribbon", ArpgStat30.CriticalChance, ArpgStat30.SpellPower, "Alternating deliveries retain tempo."),
                Theme("war-hammer", "The War Hammer", ArpgStat30.SpellPower, ArpgStat30.MaximumHealth, "Slow direct hits gain additional impact authority."),
                Theme("runic-staff", "The Runic Staff", ArpgStat30.MaximumMana, ArpgStat30.SpellPower, "Complex boards gain power from completed constellations."),
                Theme("twin-fangs", "The Twin Fangs", ArpgStat30.CooldownRecovery, ArpgStat30.CriticalChance, "Rapid alternating contacts build precision."),
                Theme("tower-shield", "The Tower Shield", ArpgStat30.MaximumHealth, ArpgStat30.Healing, "Defensive investment improves retaliation.")
            });

            AddThemeConstellations("Atlas", new[]
            {
                Theme("cartographers-lantern", "The Cartographer's Lantern", ArpgStat30.MapSustain, ArpgStat30.MapQuantity, "First completion strongly favours a connected higher-tier map."),
                Theme("treasure-whale", "The Treasure Whale", ArpgStat30.ItemRarity, ArpgStat30.CurrencyFind, "Map bosses favour equipment and currency."),
                Theme("hunters-moon", "The Hunter's Moon", ArpgStat30.ExperienceGain, ArpgStat30.MapRarity, "Rare enemies are more rewarding."),
                Theme("corrupted-sun", "The Corrupted Sun", ArpgStat30.MapQuantity, ArpgStat30.ItemRarity, "Corrupted maps trade safety for reward authority."),
                Theme("pilgrims-road", "The Pilgrim's Road", ArpgStat30.MapSustain, ArpgStat30.ExperienceGain, "Repeated lower-tier maps recover progression without becoming optimal farming."),
                Theme("fractured-compass", "The Fractured Compass", ArpgStat30.CurrencyFind, ArpgStat30.MapRarity, "Fracture Runs contribute resources to the persistent Atlas.")
            });

            AddThemeConstellations("Class", new[]
            {
                Theme("archmage-sigil", "The Archmage Sigil", ArpgStat30.MaximumMana, ArpgStat30.SpellPower, "Mana investment amplifies direct spell construction."),
                Theme("rangers-trail", "The Ranger's Trail", ArpgStat30.MoveSpeed, ArpgStat30.CriticalChance, "Movement and precision reinforce one another."),
                Theme("warriors-oath", "The Warrior's Oath", ArpgStat30.MaximumHealth, ArpgStat30.Healing, "Standing through danger strengthens the next direct action."),
                Theme("hybrid-eclipse", "The Hybrid Eclipse", ArpgStat30.TriggerEnergy, ArpgStat30.SpellPower, "Off-class equipment requirements are easier to support."),
                Theme("astral-forge", "The Astral Forge", ArpgStat30.ManaEfficiency, ArpgStat30.ItemRarity, "SpellForge-aligned affixes appear more often."),
                Theme("masterless-star", "The Masterless Star", ArpgStat30.Attunement, ArpgStat30.ExperienceGain, "Unaligned builds gain one additional Attunement and broader discovery.")
            });
        }

        private sealed class ThemeData
        {
            public string id;
            public string name;
            public ArpgStat30 primary;
            public ArpgStat30 secondary;
            public string boon;
        }

        private static ThemeData Theme(string id, string name, ArpgStat30 primary, ArpgStat30 secondary, string boon)
        {
            return new ThemeData { id = id, name = name, primary = primary, secondary = secondary, boon = boon };
        }

        private static void AddThemeConstellations(string category, ThemeData[] themes)
        {
            for (int index = 0; index < themes.Length; index++)
            {
                ThemeData theme = themes[index];
                int globalIndex = ConstellationList.Count;
                int requiredTier = globalIndex < 12 ? 0 : globalIndex < 24 ? 10 : globalIndex < 36 ? 20 : 30;
                int requiredLevel = requiredTier == 0 ? 0 : requiredTier == 10 ? 20 : requiredTier == 20 ? 45 : 70;
                int attunement = globalIndex % 7 == 0 ? 3 : globalIndex % 3 == 0 ? 2 : 1;
                ArpgConstellationDefinition30 constellation = new ArpgConstellationDefinition30
                {
                    id = "constellation." + theme.id,
                    displayName = theme.name,
                    category = category,
                    description = theme.boon,
                    attunementCost = attunement,
                    requiredLevel = requiredLevel,
                    requiredTier = requiredTier
                };
                string previous = string.Empty;
                for (int nodeIndex = 0; nodeIndex < 10; nodeIndex++)
                {
                    ArpgNodeSize30 size = nodeIndex <= 4 ? ArpgNodeSize30.Small : nodeIndex <= 6 ? ArpgNodeSize30.Medium : nodeIndex <= 8 ? ArpgNodeSize30.Large : ArpgNodeSize30.Completion;
                    ArpgStat30 stat = nodeIndex % 2 == 0 ? theme.primary : theme.secondary;
                    float value = BaseNodeValue(stat, size);
                    string nodeId = constellation.id + "." + nodeIndex;
                    string display = size == ArpgNodeSize30.Completion ? theme.name + " Complete" : size + " Star " + (nodeIndex + 1);
                    float secondaryValue = size == ArpgNodeSize30.Completion ? BaseNodeValue(theme.secondary, ArpgNodeSize30.Large) : 0f;
                    string description = size == ArpgNodeSize30.Completion
                        ? "Completion Boon: grants " + FormatStat(stat, value) + " and " + FormatStat(theme.secondary, secondaryValue) + ". Identity target for later bespoke hooks: " + theme.boon
                        : "Grants " + FormatStat(stat, value) + ".";
                    List<ArpgStatModifier30> nodeModifiers = size == ArpgNodeSize30.Completion
                        ? Mods(new ArpgStatModifier30(stat, value), new ArpgStatModifier30(theme.secondary, secondaryValue))
                        : Mods(new ArpgStatModifier30(stat, value));
                    constellation.nodes.Add(new ArpgConstellationNodeDefinition30
                    {
                        id = nodeId,
                        displayName = display,
                        description = description,
                        size = size,
                        pointCost = size == ArpgNodeSize30.Small ? 1 : size == ArpgNodeSize30.Medium ? 2 : size == ArpgNodeSize30.Large ? 3 : 1,
                        prerequisiteId = previous,
                        modifiers = nodeModifiers
                    });
                    previous = nodeId;
                }
                ConstellationList.Add(constellation);
            }
        }

        private static float BaseNodeValue(ArpgStat30 stat, ArpgNodeSize30 size)
        {
            float scale = size == ArpgNodeSize30.Small ? 1f : size == ArpgNodeSize30.Medium ? 2f : size == ArpgNodeSize30.Large ? 3.5f : 5f;
            switch (stat)
            {
                case ArpgStat30.MaximumHealth: return 8f * scale;
                case ArpgStat30.MaximumMana: return 7f * scale;
                case ArpgStat30.MoveSpeed: return 0.12f * scale;
                case ArpgStat30.TriggerEnergy: return 3f * scale;
                case ArpgStat30.Attunement: return size == ArpgNodeSize30.Completion ? 1f : 0f;
                default: return 0.0125f * scale;
            }
        }

        private static string FormatStat(ArpgStat30 stat, float value)
        {
            switch (stat)
            {
                case ArpgStat30.MaximumHealth: return Mathf.RoundToInt(value) + " maximum Health";
                case ArpgStat30.MaximumMana: return Mathf.RoundToInt(value) + " maximum Mana";
                case ArpgStat30.MoveSpeed: return value.ToString("0.00") + " movement speed";
                case ArpgStat30.TriggerEnergy: return Mathf.RoundToInt(value) + " Trigger Energy";
                case ArpgStat30.Attunement: return Mathf.RoundToInt(value) + " Attunement";
                default: return Mathf.RoundToInt(value * 100f) + "% " + stat;
            }
        }

        private static void BuildItemBases()
        {
            AddWeapon("apprentice-wand", "Apprentice Wand", "Wand", ArpgClass30.Mage, ArpgStat30.SpellPower, 0.06f);
            AddWeapon("astral-wand", "Astral Wand", "Wand", ArpgClass30.Mage, ArpgStat30.MaximumMana, 12f);
            AddWeapon("ritual-dagger", "Ritual Dagger", "Dagger", ArpgClass30.Mage, ArpgStat30.CriticalChance, 0.025f);
            AddWeapon("conduit-staff", "Conduit Staff", "Staff", ArpgClass30.Mage, ArpgStat30.TriggerEnergy, 12f);
            AddWeapon("hunting-bow", "Hunting Bow", "Bow", ArpgClass30.Ranger, ArpgStat30.CriticalChance, 0.02f);
            AddWeapon("storm-bow", "Storm Bow", "Bow", ArpgClass30.Ranger, ArpgStat30.MoveSpeed, 0.3f);
            AddWeapon("compact-crossbow", "Compact Crossbow", "Crossbow", ArpgClass30.Ranger, ArpgStat30.SpellPower, 0.055f);
            AddWeapon("wayfarer-spear", "Wayfarer Spear", "Spear", ArpgClass30.Ranger, ArpgStat30.MaximumHealth, 10f);
            AddWeapon("iron-sword", "Iron Sword", "Sword", ArpgClass30.Warrior, ArpgStat30.SpellPower, 0.05f);
            AddWeapon("war-axe", "War Axe", "Axe", ArpgClass30.Warrior, ArpgStat30.MaximumHealth, 14f);
            AddWeapon("stone-maul", "Stone Maul", "Mace", ArpgClass30.Warrior, ArpgStat30.Healing, 0.035f);
            AddWeapon("runed-greatblade", "Runed Greatblade", "Two-Handed", ArpgClass30.Warrior, ArpgStat30.TriggerEnergy, 10f);

            AddOffhand("arcane-focus", "Arcane Focus", ArpgClass30.Mage, ArpgStat30.MaximumMana, 18f);
            AddOffhand("etched-quiver", "Etched Quiver", ArpgClass30.Ranger, ArpgStat30.CriticalChance, 0.025f);
            AddOffhand("tower-shield", "Tower Shield", ArpgClass30.Warrior, ArpgStat30.MaximumHealth, 24f);

            AddArmour(ArpgItemSlot30.Helmet, "hood", "Astral Hood", ArpgStat30.MaximumMana, 8f);
            AddArmour(ArpgItemSlot30.Helmet, "helm", "Iron Helm", ArpgStat30.MaximumHealth, 10f);
            AddArmour(ArpgItemSlot30.BodyArmour, "robe", "Conduit Robe", ArpgStat30.MaximumMana, 16f);
            AddArmour(ArpgItemSlot30.BodyArmour, "leathers", "Wayfarer Leathers", ArpgStat30.MoveSpeed, 0.25f);
            AddArmour(ArpgItemSlot30.BodyArmour, "plate", "Bulwark Plate", ArpgStat30.MaximumHealth, 22f);
            AddArmour(ArpgItemSlot30.Gloves, "gloves", "Runed Gloves", ArpgStat30.SpellPower, 0.025f);
            AddArmour(ArpgItemSlot30.Gloves, "gauntlets", "Impact Gauntlets", ArpgStat30.MaximumHealth, 7f);
            AddArmour(ArpgItemSlot30.Boots, "boots", "Trail Boots", ArpgStat30.MoveSpeed, 0.35f);
            AddArmour(ArpgItemSlot30.Boots, "greaves", "Stone Greaves", ArpgStat30.MaximumHealth, 8f);
            AddArmour(ArpgItemSlot30.Belt, "belt", "Binding Belt", ArpgStat30.MaximumHealth, 9f);
            AddArmour(ArpgItemSlot30.Amulet, "amulet", "Constellation Amulet", ArpgStat30.Attunement, 1f);
            AddArmour(ArpgItemSlot30.Ring1, "ring-a", "Arcane Ring", ArpgStat30.MaximumMana, 7f);
            AddArmour(ArpgItemSlot30.Ring2, "ring-b", "Conductor Ring", ArpgStat30.TriggerEnergy, 5f);
            AddArmour(ArpgItemSlot30.Relic, "relic", "Unwritten Relic", ArpgStat30.ItemRarity, 0.04f);
        }

        private static void AddWeapon(string id, string name, string family, ArpgClass30 affinity, ArpgStat30 stat, float value)
        {
            ItemBaseList.Add(new ArpgItemBaseDefinition30 { id = "base." + id, displayName = name, slot = ArpgItemSlot30.MainHand, family = family, affinity = affinity, implicitModifiers = Mods(new ArpgStatModifier30(stat, value)) });
        }

        private static void AddOffhand(string id, string name, ArpgClass30 affinity, ArpgStat30 stat, float value)
        {
            ItemBaseList.Add(new ArpgItemBaseDefinition30 { id = "base." + id, displayName = name, slot = ArpgItemSlot30.OffHand, family = "Off-Hand", affinity = affinity, implicitModifiers = Mods(new ArpgStatModifier30(stat, value)) });
        }

        private static void AddArmour(ArpgItemSlot30 slot, string id, string name, ArpgStat30 stat, float value)
        {
            ItemBaseList.Add(new ArpgItemBaseDefinition30 { id = "base." + id, displayName = name, slot = slot, family = slot.ToString(), affinity = ArpgClass30.Unchosen, implicitModifiers = Mods(new ArpgStatModifier30(stat, value)) });
        }

        private static void BuildAffixes()
        {
            ArpgItemSlot30[] allSlots = (ArpgItemSlot30[])Enum.GetValues(typeof(ArpgItemSlot30));
            ArpgItemSlot30[] weapons = { ArpgItemSlot30.MainHand, ArpgItemSlot30.OffHand };
            ArpgItemSlot30[] armour = { ArpgItemSlot30.Helmet, ArpgItemSlot30.BodyArmour, ArpgItemSlot30.Gloves, ArpgItemSlot30.Boots, ArpgItemSlot30.Belt };
            ArpgItemSlot30[] jewellery = { ArpgItemSlot30.Amulet, ArpgItemSlot30.Ring1, ArpgItemSlot30.Ring2, ArpgItemSlot30.Relic };
            AddAffixFamily("potency", "Potency", true, ArpgStat30.SpellPower, 0.025f, 0.15f, allSlots);
            AddAffixFamily("vitality", "Vitality", true, ArpgStat30.MaximumHealth, 8f, 70f, allSlots);
            AddAffixFamily("reservoir", "Reservoir", true, ArpgStat30.MaximumMana, 7f, 60f, allSlots);
            AddAffixFamily("precision", "Precision", true, ArpgStat30.CriticalChance, 0.01f, 0.075f, weapons.Concat(jewellery).ToArray());
            AddAffixFamily("acceleration", "Acceleration", false, ArpgStat30.MoveSpeed, 0.08f, 0.65f, new[] { ArpgItemSlot30.Boots, ArpgItemSlot30.MainHand, ArpgItemSlot30.Ring1, ArpgItemSlot30.Ring2 });
            AddAffixFamily("tempo", "Temporal Recovery", false, ArpgStat30.CooldownRecovery, 0.015f, 0.12f, weapons.Concat(jewellery).ToArray());
            AddAffixFamily("efficiency", "Mana Efficiency", false, ArpgStat30.ManaEfficiency, 0.015f, 0.13f, allSlots);
            AddAffixFamily("restoration", "Restoration", false, ArpgStat30.Healing, 0.02f, 0.16f, armour.Concat(jewellery).ToArray());
            AddAffixFamily("trigger", "Trigger Capacity", true, ArpgStat30.TriggerEnergy, 3f, 26f, weapons.Concat(jewellery).ToArray());
            AddAffixFamily("fortune", "Fortune", false, ArpgStat30.ItemRarity, 0.01f, 0.09f, jewellery);
            AddAffixFamily("currency", "Currency Discovery", false, ArpgStat30.CurrencyFind, 0.01f, 0.08f, jewellery);
            AddAffixFamily("cartography", "Map Sustain", false, ArpgStat30.MapSustain, 0.01f, 0.08f, new[] { ArpgItemSlot30.Relic, ArpgItemSlot30.Amulet, ArpgItemSlot30.Belt });
            AddAffixFamily("quantity", "Map Quantity", true, ArpgStat30.MapQuantity, 0.01f, 0.08f, new[] { ArpgItemSlot30.Relic, ArpgItemSlot30.Amulet });
            AddAffixFamily("attunement", "Celestial Attunement", true, ArpgStat30.Attunement, 1f, 1f, new[] { ArpgItemSlot30.Amulet, ArpgItemSlot30.Relic });
        }

        private static void AddAffixFamily(string id, string name, bool prefix, ArpgStat30 stat, float min, float max, ArpgItemSlot30[] slots)
        {
            for (int tier = 1; tier <= 4; tier++)
            {
                float low = Mathf.Lerp(min, max, (tier - 1) / 4f);
                float high = Mathf.Lerp(min, max, tier / 4f);
                AffixList.Add(new ArpgAffixDefinition30
                {
                    id = "affix." + id + ".t" + tier,
                    displayName = name + " T" + tier,
                    prefix = prefix,
                    stat = stat,
                    minimum = low,
                    maximum = high,
                    minimumItemLevel = (tier - 1) * 22,
                    weight = Mathf.Max(10, 120 - tier * 20),
                    family = id,
                    validSlots = slots.ToList()
                });
            }
        }

        private static void BuildMaps()
        {
            string[] regionNames = { "The Pale Verge", "The Azure Expanse", "The Gilded Fracture", "The Crimson Beyond" };
            string[][] mapNames =
            {
                new[] { "Ashen Causeway", "Frostbitten Hollow", "Broken Arcade", "Quiet Ossuary", "Cinder Grove", "Flooded Archive", "Iron Orchard", "Pale Observatory", "Forgotten Quarry", "Glass Marsh" },
                new[] { "Storm Gallery", "Blue Labyrinth", "Venom Basilica", "Drowned Conduit", "Howling Rampart", "Astral Menagerie", "Cracked Reservoir", "Moonlit Foundry", "Sapphire Necropolis", "Warden's Reach" },
                new[] { "Gilded Chasm", "Blood Meridian", "Toxic Conservatory", "Sunken Citadel", "Runebound Steppe", "Mechanical Choir", "Fracture Palace", "Titan's Spine", "Hollow Crown", "Amber Abyss" },
                new[] { "Crimson Firmament", "Black Star Vault", "Final Crucible", "Rift Cathedral", "Worldbreak Engine", "Sanguine Horizon", "Stormgrave", "Ashen Throne", "Void Meridian", "The Last Geometry" }
            };
            string[] bosses = { "Conduit Warden", "Glass Matriarch", "Rune-Eater", "Hollow Knight", "Fracture Beast", "Astral Judge", "Crimson Architect", "Worldweight Titan" };
            for (int tier = 0; tier < 40; tier++)
            {
                int band = tier / 10;
                for (int variant = 0; variant < 2; variant++)
                {
                    string baseName = mapNames[band][tier % 10];
                    string suffix = variant == 0 ? string.Empty : " · Echo";
                    MapList.Add(new ArpgMapDefinition30
                    {
                        id = "map.t" + tier.ToString("00") + ".v" + variant,
                        displayName = baseName + suffix,
                        tier = tier,
                        region = regionNames[band],
                        bossName = bosses[(tier + variant * 3) % bosses.Length],
                        environmentHint = band == 0 ? "foundation" : band == 1 ? "formation" : band == 2 ? "specialization" : "optimization"
                    });
                }
            }
        }

        private static void BuildMapAffixes()
        {
            AddMapAffix("frenzied", "Frenzied Host", "Enemies act and move more aggressively.", 3, 1.14f, "frenziedEnemies");
            AddMapAffix("bulwark", "Bulwark Host", "Enemies are substantially harder to kill.", 5, 1.18f, "bulwarkEnemies");
            AddMapAffix("glass-soul", "Glass Soul", "Both danger and reward are amplified.", 8, 1.22f, "glassSoul");
            AddMapAffix("mana-drought", "Mana Drought", "Mana pressure is increased.", 10, 1.17f, "manaDrought");
            AddMapAffix("elite-host", "Elite Host", "Additional elite properties appear.", 12, 1.21f, "extraEliteAffixes");
            AddMapAffix("scarred-recovery", "Scarred Recovery", "Healing is reduced.", 15, 1.19f, "reducedHealing");
            AddMapAffix("unstable-world", "Unstable World", "Spell instability is increased.", 18, 1.18f, "unstableWorld");
            AddMapAffix("adaptive", "Adaptive Enemies", "Enemies respond to repeated tactics.", 20, 1.22f, "adaptiveEnemies");
            AddMapAffix("timed", "Collapsing Map", "The encounter applies time pressure.", 22, 1.25f, "timedRooms");
            AddMapAffix("no-starting-gear", "Naked Entry", "Legacy equipment is disabled for this map.", 25, 1.35f, "noStartingEquipment");
            AddMapAffix("no-starting-runes", "Unwritten Entry", "Legacy starting modifiers are disabled.", 28, 1.28f, "noStartingModifiers");
            AddMapAffix("boss-phase", "Awakened Guardian", "The map guardian has an additional phase budget.", 30, 1.32f, "newBossPhase");
        }

        private static void AddMapAffix(string id, string name, string description, int minimumTier, float rewardMultiplier, string flag)
        {
            MapAffixList.Add(new ArpgMapAffixDefinition30 { id = "map-affix." + id, displayName = name, description = description, minimumTier = minimumTier, rewardMultiplier = rewardMultiplier, difficultyFlag = flag });
        }

        private static List<ArpgStatModifier30> Mods(params ArpgStatModifier30[] values)
        {
            return values == null ? new List<ArpgStatModifier30>() : values.ToList();
        }

        public static string Validate()
        {
            Ensure();
            List<string> failures = new List<string>();
            if (Classes.Count != 3) failures.Add("Expected exactly 3 classes.");
            if (Ascendancies.Count != 9) failures.Add("Expected exactly 9 Ascendancies.");
            if (Constellations.Count < 39) failures.Add("Expected at least 39 constellations.");
            if (Constellations.Sum(value => value.nodes.Count) < 390) failures.Add("Expected at least 390 constellation nodes.");
            if (Maps.Count != 80) failures.Add("Expected exactly 80 standard Atlas maps.");
            if (Maps.Select(value => value.tier).Distinct().Count() != 40) failures.Add("Expected all 40 standard map tiers.");
            if (ItemBases.Count < 28) failures.Add("Expected at least 28 item bases.");
            if (Affixes.Count < 50) failures.Add("Expected at least 50 tiered affixes.");
            if (MapAffixes.Count < 12) failures.Add("Expected at least 12 map affixes.");
            if (Constellations.Select(value => value.id).Distinct().Count() != Constellations.Count) failures.Add("Constellation IDs are not unique.");
            if (Maps.Select(value => value.id).Distinct().Count() != Maps.Count) failures.Add("Map IDs are not unique.");
            if (Affixes.Select(value => value.id).Distinct().Count() != Affixes.Count) failures.Add("Affix IDs are not unique.");
            return failures.Count == 0 ? string.Empty : string.Join("\n", failures.ToArray());
        }
    }
}
