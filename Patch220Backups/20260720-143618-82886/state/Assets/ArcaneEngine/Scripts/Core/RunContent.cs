using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum RelicSignature
    {
        None,
        DyingSun,
        PhoenixSeed,
        Hellstorm,
        AbsoluteZero,
        FrostOrbit,
        ThunderJudgment,
        LivingCircuit,
        InfiniteBeam,
        PrismLance,
        FallingWorld,
        Starfall,
        WispLegion,
        SoulTwin,
        RiftStep,
        Afterimage,
        WorldWall,
        DevouringZone,
        SpellbladeDance,
        ExecutionEdge,
        PerfectAegis,
        RetributionWard,
        ShatterTide,
        StormbrandEcho,
        HorizonRay,
        CometWake,
        CovenantSwarm,
        PhaseCascade,
        MirrorBastion,
        BloodMoonEdge,
        ChronoGuard,
        PlagueOrchard,
        VenomRebirth,
        CrystalGuillotine,
        WintersNeedle,
        TempestCrown,
        BallLightningEngine,
        EventHorizon,
        CrushingOrbit
    }

    public sealed class RelicDefinition : ScriptableObject
    {
        public string id;
        public string sourceCoreId;
        public string displayName;
        [TextArea] public string description;
        [TextArea] public string signatureRule;
        public RelicSignature signature;
        public int shardCost;
        public int requiredBossKills;
        public Color color;
    }

    public enum DungeonRoomType
    {
        Combat,
        Elite,
        ModifierReward,
        SpellCoreReward,
        EquipmentReward,
        TreasureVault,
        Shop,
        SafeWorkshop,
        HealingSanctuary,
        CursedBargain,
        Puzzle,
        NarrativeEvent,
        Challenge,
        Secret,
        Miniboss,
        Boss,
        Extraction
    }

    public sealed class RoomTemplate
    {
        public string id;
        public string displayName;
        public string biome;
        public DungeonRoomType type;
        public int difficulty;
        public int obstaclePattern;
        public bool hasHazards;
        public Color floorColor;
        public Color accentColor;
    }

    public enum RewardCategory
    {
        Modifier,
        SpellCore,
        Equipment,
        Drachmas,
        Essence,
        Healing,
        Blessing,
        CursedPower,
        BoardExpansion,
        ModifierTransformation,
        EquipmentUpgrade,
        ShopDiscount,
        MapReveal,
        RelicShardChallenge,
        SpellUpgrade,
        SpellLink,
        LinkUpgrade
    }

    [Serializable]
    public sealed class RewardOffer
    {
        public string id;
        public RewardCategory category;
        public string title;
        public string description;
        public string contentId;
        public int amount;
        public bool risky;
        public ItemSaveData generatedItem;
        public Color color = Color.white;
    }

    [Serializable]
    public sealed class ShopOffer
    {
        public string id;
        public string title;
        public string description;
        public string contentId;
        public RewardCategory category;
        public int price;
        public bool sold;
        public ItemSaveData generatedItem;
    }

    public static class MegaCatalog
    {
        private static readonly Dictionary<string, RelicDefinition> Relics = new Dictionary<string, RelicDefinition>();
        private static readonly List<RoomTemplate> Rooms = new List<RoomTemplate>();
        private static bool _initialized;

        public static IEnumerable<RelicDefinition> AllRelics { get { Ensure(); return Relics.Values; } }
        public static IEnumerable<RoomTemplate> AllRooms { get { Ensure(); return Rooms; } }

        public static void Ensure()
        {
            if (_initialized) return;
            _initialized = true;
            BuildRelics();
            BuildRooms();
            V21AuthoredContentOverlay.ApplyRelicsRooms(Relics, Rooms);
        }

        public static RelicDefinition GetRelic(string id)
        {
            Ensure();
            RelicDefinition value;
            Relics.TryGetValue(id, out value);
            return value;
        }

        public static IEnumerable<RelicDefinition> RelicsForCore(string coreId)
        {
            Ensure();
            return Relics.Values.Where(r => r.sourceCoreId == coreId);
        }

        public static RoomTemplate RandomRoom(DungeonRoomType type, int seed, IEnumerable<string> excludedIds)
        {
            Ensure();
            HashSet<string> excluded = new HashSet<string>(excludedIds ?? Enumerable.Empty<string>());
            List<RoomTemplate> candidates = Rooms.Where(r => r.type == type && !excluded.Contains(r.id)).ToList();
            if (candidates.Count == 0) candidates = Rooms.Where(r => r.type == type).ToList();
            if (candidates.Count == 0) candidates = Rooms.Where(r => r.type == DungeonRoomType.Combat).ToList();
            return candidates[Mathf.Abs(seed) % candidates.Count];
        }

        private static void BuildRelics()
        {
            AddRelic("dying_sun", "fireball", "Dying Sun", RelicSignature.DyingSun, 1, 1,
                "A slow miniature sun pulls enemies inward and detonates on expiry.", "Immutable: gravitational miniature sun.", new Color(1f, 0.26f, 0.02f));
            AddRelic("phoenix_seed", "fireball", "Phoenix Seed", RelicSignature.PhoenixSeed, 2, 2,
                "Embeds in a victim. Death erupts into inherited Fireballs.", "Immutable: marked deaths hatch new projectiles.", new Color(1f, 0.58f, 0.05f));
            AddRelic("hellstorm", "fireball", "Hellstorm", RelicSignature.Hellstorm, 2, 3,
                "Targets an area and rains repeated fiery impacts.", "Immutable: delayed impact storm.", new Color(0.95f, 0.08f, 0.02f));

            AddRelic("absolute_zero", "ice_nova", "Absolute Zero", RelicSignature.AbsoluteZero, 1, 1,
                "The final pulse shatters frozen targets for stored damage.", "Immutable: freeze culminates in shatter.", new Color(0.2f, 0.9f, 1f));
            AddRelic("frost_orbit", "ice_nova", "Crown of Winter", RelicSignature.FrostOrbit, 2, 2,
                "The nova becomes orbiting frost satellites that pulse around you.", "Immutable: persistent orbiting frost crown.", new Color(0.5f, 0.75f, 1f));

            AddRelic("thunder_judgment", "lightning_strike", "Thunder Judgment", RelicSignature.ThunderJudgment, 1, 1,
                "A delayed pillar executes wounded enemies and chains from kills.", "Immutable: execution lightning pillar.", new Color(0.35f, 0.65f, 1f));
            AddRelic("living_circuit", "lightning_strike", "Living Circuit", RelicSignature.LivingCircuit, 2, 2,
                "Lightning remembers struck targets and repeatedly reconnects them.", "Immutable: targets form a living circuit.", new Color(0.6f, 0.45f, 1f));

            AddRelic("infinite_beam", "arc_beam", "The Unending Line", RelicSignature.InfiniteBeam, 1, 1,
                "The beam persists briefly and intensifies while held on one victim.", "Immutable: focusing beam gains intensity.", new Color(0.85f, 0.35f, 1f));
            AddRelic("prism_lance", "arc_beam", "Prism Lance", RelicSignature.PrismLance, 2, 2,
                "A piercing beam splits into elemental rays on impact.", "Immutable: impact creates prismatic branches.", new Color(1f, 0.35f, 0.8f));

            AddRelic("falling_world", "meteor", "The Falling World", RelicSignature.FallingWorld, 2, 2,
                "A massive delayed meteor leaves a long-lived burning crater.", "Immutable: catastrophic crater impact.", new Color(1f, 0.18f, 0.02f));
            AddRelic("starfall", "meteor", "Starfall Choir", RelicSignature.Starfall, 1, 1,
                "Calls many smaller meteors across the target area.", "Immutable: repeated scattered impacts.", new Color(1f, 0.7f, 0.2f));

            AddRelic("wisp_legion", "wisp_summon", "Wisp Legion", RelicSignature.WispLegion, 1, 1,
                "Summons several fragile wisps that inherit projectile payloads.", "Immutable: multiple inherited familiars.", new Color(0.2f, 1f, 0.65f));
            AddRelic("soul_twin", "wisp_summon", "Soul Twin", RelicSignature.SoulTwin, 2, 2,
                "Summons one permanent twin that echoes manual casts.", "Immutable: familiar echoes the player.", new Color(0.65f, 1f, 0.85f));

            AddRelic("rift_step", "blink", "Rift Step", RelicSignature.RiftStep, 1, 1,
                "Movement tears a damaging rift between departure and arrival.", "Immutable: traversal leaves a rift.", new Color(0.6f, 0.1f, 1f));
            AddRelic("afterimage", "blink", "Army of Afterimages", RelicSignature.Afterimage, 2, 2,
                "Movement leaves echoes that repeat the next cast.", "Immutable: casting afterimages.", new Color(0.85f, 0.4f, 1f));

            AddRelic("world_wall", "arcane_wall", "World Wall", RelicSignature.WorldWall, 1, 1,
                "Creates a long barrier that reflects hostile projectiles.", "Immutable: projectile-reflecting barrier.", new Color(0.15f, 0.8f, 1f));
            AddRelic("devouring_zone", "arcane_wall", "Devouring Boundary", RelicSignature.DevouringZone, 2, 2,
                "The wall collapses into a zone that consumes enemy magic.", "Immutable: spell-devouring zone.", new Color(0.55f, 0.05f, 0.75f));

            AddRelic("spellblade_dance", "spellblade", "Spellblade Dance", RelicSignature.SpellbladeDance, 1, 1,
                "A rapid orbit of blades repeats on critical hits.", "Immutable: orbiting blade sequence.", new Color(1f, 0.3f, 0.6f));
            AddRelic("execution_edge", "spellblade", "Execution Edge", RelicSignature.ExecutionEdge, 2, 2,
                "Excess kill damage is stored and released by the next strike.", "Immutable: overkill becomes stored power.", new Color(1f, 0.08f, 0.15f));

            AddRelic("perfect_aegis", "aegis", "Perfect Aegis", RelicSignature.PerfectAegis, 1, 1,
                "A precisely timed ward negates damage and refunds its cost.", "Immutable: perfect timing fully negates.", new Color(0.25f, 0.75f, 1f));
            AddRelic("retribution_ward", "aegis", "Retribution Ward", RelicSignature.RetributionWard, 2, 2,
                "Absorbed damage is released as a customized nova.", "Immutable: absorbed harm becomes retaliation.", new Color(1f, 0.75f, 0.2f));

            AddRelic("venom_burst", "poison_bolt", "Venom Burst", RelicSignature.DevouringZone, 1, 1,
                "Poison Bolt becomes a lasting toxic field that pulls enemies inward.", "Legendary Effect: pulling poison field.", new Color(0.25f, 1f, 0.12f));
            AddRelic("frozen_beam", "frost_lance", "Frozen Beam", RelicSignature.InfiniteBeam, 1, 1,
                "Frost Lance repeats rapidly and strengthens its freeze.", "Legendary Effect: repeating frost beam.", new Color(0.25f, 0.85f, 1f));
            AddRelic("chain_storm", "storm_orb", "Chain Storm", RelicSignature.LivingCircuit, 2, 2,
                "Storm Orb repeatedly reconnects every enemy it hits.", "Legendary Effect: repeated chain lightning.", new Color(0.4f, 0.55f, 1f));
            AddRelic("black_hole", "gravity_field", "Black Hole", RelicSignature.DyingSun, 2, 2,
                "Gravity Field becomes a moving singularity with a final explosion.", "Legendary Effect: moving black hole.", new Color(0.5f, 0.05f, 0.85f));
            AddRelic("shatter_tide", "ice_nova", "Shatter Tide", RelicSignature.ShatterTide, 2, 3,
                "The nova travels outward in three freezing waves and consumes Freeze on the final wave.", "Immutable: three-stage shatter wave.", new Color(0.55f, 0.95f, 1f));
            AddRelic("stormbrand_echo", "lightning_strike", "Stormbrand Echo", RelicSignature.StormbrandEcho, 2, 3,
                "The struck target becomes a storm anchor that repeats diminishing strikes.", "Immutable: repeated branded judgment.", new Color(0.55f, 0.7f, 1f));
            AddRelic("horizon_ray", "arc_beam", "Event Horizon Ray", RelicSignature.HorizonRay, 2, 3,
                "A widening Void beam pulls enemies toward its line before collapsing.", "Immutable: pulling collapse beam.", new Color(0.72f, 0.15f, 1f));
            AddRelic("comet_wake", "meteor", "Comet Wake", RelicSignature.CometWake, 2, 3,
                "The meteor crosses the arena and leaves a chain of burning impact zones.", "Immutable: travelling impact wake.", new Color(1f, 0.38f, 0.08f));
            AddRelic("covenant_swarm", "wisp_summon", "Covenant Swarm", RelicSignature.CovenantSwarm, 2, 3,
                "Three bonded wisps focus the same target and intensify repeated hits.", "Immutable: coordinated familiar focus.", new Color(0.3f, 1f, 0.72f));
            AddRelic("phase_cascade", "blink", "Phase Cascade", RelicSignature.PhaseCascade, 2, 3,
                "Blink repeats twice along the chosen line and detonates at each step.", "Immutable: chained movement detonations.", new Color(0.78f, 0.28f, 1f));
            AddRelic("mirror_bastion", "arcane_wall", "Mirror Bastion", RelicSignature.MirrorBastion, 2, 3,
                "Creates crossing barriers that reflect projectiles and pulse when struck.", "Immutable: crossing reflective bastion.", new Color(0.2f, 0.92f, 1f));
            AddRelic("blood_moon_edge", "spellblade", "Blood Moon Edge", RelicSignature.BloodMoonEdge, 2, 3,
                "Spends a small amount of Health to create a returning execution sweep.", "Immutable: blood-powered returning edge.", new Color(1f, 0.08f, 0.28f));
            AddRelic("chrono_guard", "aegis", "Chrono Guard", RelicSignature.ChronoGuard, 2, 3,
                "A successful block accelerates cooldown recovery and releases a slowing pulse.", "Immutable: time-shifting guard.", new Color(0.35f, 0.9f, 1f));
            AddRelic("plague_orchard", "poison_bolt", "Plague Orchard", RelicSignature.PlagueOrchard, 2, 2,
                "Impacts grow toxic zones that spread poison to nearby enemies.", "Immutable: spreading toxic growths.", new Color(0.35f, 1f, 0.08f));
            AddRelic("venom_rebirth", "poison_bolt", "Venom Rebirth", RelicSignature.VenomRebirth, 2, 3,
                "Poisoned kills split into seeking venom bolts.", "Immutable: poisoned deaths hatch projectiles.", new Color(0.65f, 1f, 0.05f));
            AddRelic("crystal_guillotine", "frost_lance", "Crystal Guillotine", RelicSignature.CrystalGuillotine, 2, 2,
                "A delayed piercing lance consumes Freeze for a heavy shatter strike.", "Immutable: delayed frozen execution.", new Color(0.25f, 0.85f, 1f));
            AddRelic("winters_needle", "frost_lance", "Winter's Needle", RelicSignature.WintersNeedle, 2, 3,
                "The beam becomes a rapid sequence of narrow piercing frost needles.", "Immutable: repeating piercing needles.", new Color(0.7f, 0.95f, 1f));
            AddRelic("tempest_crown", "storm_orb", "Tempest Crown", RelicSignature.TempestCrown, 2, 2,
                "Storm Orbs orbit the player before launching at separate targets.", "Immutable: orbiting storm crown.", new Color(0.45f, 0.6f, 1f));
            AddRelic("ball_lightning_engine", "storm_orb", "Ball Lightning Engine", RelicSignature.BallLightningEngine, 2, 3,
                "A slow orb repeatedly chains lightning while travelling.", "Immutable: travelling chain engine.", new Color(0.65f, 0.75f, 1f));
            AddRelic("event_horizon", "gravity_field", "Event Horizon", RelicSignature.EventHorizon, 2, 2,
                "The field contracts while pulling harder, then consumes statuses in a final collapse.", "Immutable: contracting status collapse.", new Color(0.62f, 0.08f, 0.9f));
            AddRelic("crushing_orbit", "gravity_field", "Crushing Orbit", RelicSignature.CrushingOrbit, 2, 3,
                "Three gravity nodes orbit the target area and repeatedly pull enemies through their paths.", "Immutable: orbiting gravity nodes.", new Color(0.82f, 0.18f, 1f));
        }

        private static void BuildRooms()
        {
            AddRoomSet(DungeonRoomType.Combat, "Ossuary", 4, new Color(0.07f, 0.08f, 0.12f), new Color(0.2f, 0.55f, 0.8f));
            AddRoomSet(DungeonRoomType.Elite, "Blood Gallery", 3, new Color(0.11f, 0.045f, 0.06f), new Color(0.85f, 0.12f, 0.2f));
            AddRoomSet(DungeonRoomType.ModifierReward, "Rune Atelier", 2, new Color(0.06f, 0.09f, 0.11f), new Color(0.15f, 0.95f, 0.85f));
            AddRoomSet(DungeonRoomType.SpellCoreReward, "Silent Archive", 2, new Color(0.07f, 0.055f, 0.12f), new Color(0.7f, 0.35f, 1f));
            AddRoomSet(DungeonRoomType.EquipmentReward, "Knight Reliquary", 2, new Color(0.09f, 0.075f, 0.06f), new Color(0.95f, 0.65f, 0.2f));
            AddRoomSet(DungeonRoomType.TreasureVault, "Gilded Crypt", 2, new Color(0.11f, 0.09f, 0.04f), new Color(1f, 0.78f, 0.15f));
            AddRoomSet(DungeonRoomType.Shop, "Lantern Market", 2, new Color(0.055f, 0.09f, 0.08f), new Color(0.2f, 1f, 0.65f));
            AddRoomSet(DungeonRoomType.SafeWorkshop, "Quiet Forge", 2, new Color(0.055f, 0.07f, 0.1f), new Color(0.3f, 0.75f, 1f));
            AddRoomSet(DungeonRoomType.HealingSanctuary, "Mercy Chapel", 2, new Color(0.075f, 0.08f, 0.1f), new Color(0.65f, 0.95f, 1f));
            AddRoomSet(DungeonRoomType.CursedBargain, "Whispering Altar", 2, new Color(0.09f, 0.035f, 0.095f), new Color(1f, 0.1f, 0.65f));
            AddRoomSet(DungeonRoomType.Puzzle, "Rotating Seal", 2, new Color(0.065f, 0.075f, 0.1f), new Color(0.4f, 0.9f, 1f));
            AddRoomSet(DungeonRoomType.NarrativeEvent, "Memory Vault", 2, new Color(0.08f, 0.06f, 0.1f), new Color(0.8f, 0.55f, 1f));
            AddRoomSet(DungeonRoomType.Challenge, "Trial Circle", 2, new Color(0.095f, 0.055f, 0.055f), new Color(1f, 0.35f, 0.1f));
            AddRoomSet(DungeonRoomType.Secret, "Forgotten Annex", 2, new Color(0.04f, 0.055f, 0.065f), new Color(0.15f, 0.8f, 0.65f));
            AddRoomSet(DungeonRoomType.Miniboss, "Bone Court", 2, new Color(0.095f, 0.055f, 0.07f), new Color(0.9f, 0.2f, 0.4f));
            AddRoomSet(DungeonRoomType.Boss, "Throne of the First Warden", 2, new Color(0.105f, 0.035f, 0.055f), new Color(1f, 0.12f, 0.25f));
            AddRoomSet(DungeonRoomType.Extraction, "Return Gate", 2, new Color(0.04f, 0.085f, 0.085f), new Color(0.15f, 1f, 0.8f));
        }

        private static void AddRelic(string id, string core, string name, RelicSignature signature, int shards, int bosses,
            string description, string rule, Color color)
        {
            RelicDefinition definition = ScriptableObject.CreateInstance<RelicDefinition>();
            definition.hideFlags = HideFlags.HideAndDontSave;
            definition.id = id;
            definition.sourceCoreId = core;
            definition.displayName = name;
            definition.signature = signature;
            definition.shardCost = shards;
            definition.requiredBossKills = bosses;
            definition.description = description;
            definition.signatureRule = rule;
            definition.color = color;
            Relics[id] = definition;
        }

        private static void AddRoomSet(DungeonRoomType type, string name, int variants, Color floor, Color accent)
        {
            for (int i = 0; i < variants; i++)
            {
                Rooms.Add(new RoomTemplate
                {
                    id = type.ToString().ToLowerInvariant() + "_" + i,
                    displayName = name + (variants > 1 ? " " + Roman(i + 1) : string.Empty),
                    biome = "The Ossuary Catacombs",
                    type = type,
                    difficulty = 1 + i,
                    obstaclePattern = i,
                    hasHazards = type == DungeonRoomType.Elite || type == DungeonRoomType.Challenge || type == DungeonRoomType.Boss,
                    floorColor = floor,
                    accentColor = accent
                });
            }
        }

        private static string Roman(int value)
        {
            return value == 1 ? "I" : value == 2 ? "II" : value == 3 ? "III" : "IV";
        }
    }
}
