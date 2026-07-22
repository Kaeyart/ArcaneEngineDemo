using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ArpgCurrency32
    {
        FluxShard, BindingSeal, SovereignEmber, AstralNeedle, TemperingPrism,
        ArtisansMeasure, SeveranceChisel, TransferenceSigil, ResonantBrand,
        AscendantSpark, PrefixWard, SuffixWard, MemoryLoom, FractureRune, OmenOfRevision
    }

    [Serializable] public sealed class ArpgFootprintDefinition32 { public ArpgItemSlot30 slot; public int width; public int height; }
    [Serializable] public sealed class ArpgCurrencyDefinition32 { public ArpgCurrency32 id; public string displayName; public string description; public string rarity; }
    [Serializable] public sealed class ArpgUniqueDefinition32
    {
        public string id; public string displayName; public string baseId; public string description;
        public string constraint; public string bossId;
        public List<ArpgStatModifier30> modifiers = new List<ArpgStatModifier30>();
    }
    [Serializable] public sealed class ArpgCorruptedImplicitDefinition32
    {
        public string id; public string displayName; public string description;
        public ArpgStatModifier30 modifier; public ArpgStatModifier30 penalty;
    }

    public static class ArpgArsenalContent32
    {
        private static bool _ready;
        private static readonly List<ArpgItemBaseDefinition30> Bases = new List<ArpgItemBaseDefinition30>();
        private static readonly List<ArpgAffixDefinition30> Affixes = new List<ArpgAffixDefinition30>();
        private static readonly List<ArpgMapAffixDefinition30> Maps = new List<ArpgMapAffixDefinition30>();
        private static readonly List<ArpgCurrencyDefinition32> Currencies = new List<ArpgCurrencyDefinition32>();
        private static readonly List<ArpgUniqueDefinition32> Uniques = new List<ArpgUniqueDefinition32>();
        private static readonly List<ArpgCorruptedImplicitDefinition32> Corruptions = new List<ArpgCorruptedImplicitDefinition32>();

        public static IReadOnlyList<ArpgItemBaseDefinition30> ItemBases { get { Ensure(); return Bases; } }
        public static IReadOnlyList<ArpgAffixDefinition30> ItemAffixes { get { Ensure(); return Affixes; } }
        public static IReadOnlyList<ArpgMapAffixDefinition30> MapAffixes { get { Ensure(); return Maps; } }
        public static IReadOnlyList<ArpgCurrencyDefinition32> CurrencyDefinitions { get { Ensure(); return Currencies; } }
        public static IReadOnlyList<ArpgUniqueDefinition32> UniqueItems { get { Ensure(); return Uniques; } }
        public static IReadOnlyList<ArpgCorruptedImplicitDefinition32> CorruptedImplicitDefinitions { get { Ensure(); return Corruptions; } }

        public static void Ensure()
        {
            if (_ready) return;
            _ready = true;
            BuildBases(); BuildAffixes(); BuildMaps(); BuildCurrencies(); BuildUniques(); BuildCorruptions();
        }

        public static void ExtendLegacyCatalogues(List<ArpgItemBaseDefinition30> bases, List<ArpgAffixDefinition30> affixes, List<ArpgMapAffixDefinition30> maps)
        {
            Ensure();
            foreach (ArpgItemBaseDefinition30 v in Bases) if (!bases.Any(x => x.id == v.id)) bases.Add(v);
            foreach (ArpgAffixDefinition30 v in Affixes) if (!affixes.Any(x => x.id == v.id)) affixes.Add(v);
            foreach (ArpgMapAffixDefinition30 v in Maps) if (!maps.Any(x => x.id == v.id)) maps.Add(v);
        }

        public static ArpgCurrencyDefinition32 Currency(ArpgCurrency32 id) { return CurrencyDefinitions.FirstOrDefault(x => x.id == id); }
        public static ArpgUniqueDefinition32 Unique(string id) { return UniqueItems.FirstOrDefault(x => x.id == id); }
        public static ArpgCorruptedImplicitDefinition32 CorruptedImplicit(string id) { return CorruptedImplicitDefinitions.FirstOrDefault(x => x.id == id); }

        public static ArpgFootprintDefinition32 Footprint(ArpgItem30 item)
        {
            ArpgItemSlot30 slot = item == null ? ArpgItemSlot30.Relic : item.slot;
            if (slot == ArpgItemSlot30.MainHand)
            {
                ArpgItemBaseDefinition30 b = item == null ? null : ArpgContent30.ItemBase(item.baseId);
                bool two = b != null && (b.family.Contains("Bow") || b.family.Contains("Maul") || b.family.Contains("Staff") || b.family.Contains("Great"));
                return new ArpgFootprintDefinition32 { slot = slot, width = 2, height = two ? 4 : 3 };
            }
            if (slot == ArpgItemSlot30.OffHand || slot == ArpgItemSlot30.BodyArmour)
                return new ArpgFootprintDefinition32 { slot = slot, width = 2, height = 3 };
            if (slot == ArpgItemSlot30.Helmet || slot == ArpgItemSlot30.Gloves || slot == ArpgItemSlot30.Boots || slot == ArpgItemSlot30.Relic)
                return new ArpgFootprintDefinition32 { slot = slot, width = 2, height = 2 };
            if (slot == ArpgItemSlot30.Belt)
                return new ArpgFootprintDefinition32 { slot = slot, width = 2, height = 1 };
            return new ArpgFootprintDefinition32 { slot = slot, width = 1, height = 1 };
        }

        public static ArpgCurrency32 FromLegacy(ArpgCurrency30 value)
        {
            switch (value)
            {
                case ArpgCurrency30.SparkOfAlteration: return ArpgCurrency32.FluxShard;
                case ArpgCurrency30.RuneOfAugmentation: return ArpgCurrency32.BindingSeal;
                case ArpgCurrency30.SigilOfElevation: return ArpgCurrency32.SovereignEmber;
                case ArpgCurrency30.ArcaneExalt: return ArpgCurrency32.AstralNeedle;
                case ArpgCurrency30.ReformationOrb:
                case ArpgCurrency30.DivineMeasure: return ArpgCurrency32.TemperingPrism;
                case ArpgCurrency30.RefinementShard: return ArpgCurrency32.ArtisansMeasure;
                case ArpgCurrency30.NullOrb: return ArpgCurrency32.SeveranceChisel;
                case ArpgCurrency30.ChaosFragment: return ArpgCurrency32.TransferenceSigil;
                case ArpgCurrency30.ElementalEssence: return ArpgCurrency32.ResonantBrand;
                case ArpgCurrency30.OmenOfControl: return ArpgCurrency32.OmenOfRevision;
                case ArpgCurrency30.FractureRune: return ArpgCurrency32.FractureRune;
                default: return ArpgCurrency32.TemperingPrism;
            }
        }

        private static void BuildBases()
        {
            ArpgItemSlot30[] slots = (ArpgItemSlot30[])Enum.GetValues(typeof(ArpgItemSlot30));
            string[] themes = { "Cinder", "Frostglass", "Stormbound", "Ironfall", "Crimson", "Venom", "Voidcall", "Astral", "Warden", "Chronal" };
            string[] nouns = { "Wand", "Focus", "Crown", "Harness", "Grips", "Steps", "Girdle", "Pendant", "Band", "Signet", "Reliquary" };
            ArpgStat30[] stats =
            {
                ArpgStat30.FirePower, ArpgStat30.ColdPower, ArpgStat30.LightningPower, ArpgStat30.PhysicalPower,
                ArpgStat30.BloodPower, ArpgStat30.ToxicPower, ArpgStat30.VoidPower, ArpgStat30.SpellPower,
                ArpgStat30.ReactionPower, ArpgStat30.RuneCapacity
            };
            for (int s = 0; s < slots.Length; s++)
            {
                for (int i = 0; i < 10; i++)
                {
                    ArpgStat30 stat = stats[(s + i) % stats.Length];
                    bool flat = stat == ArpgStat30.RuneCapacity || stat == ArpgStat30.Armour || stat == ArpgStat30.Evasion || stat == ArpgStat30.ArcaneWard;
                    string family = slots[s] == ArpgItemSlot30.MainHand && (i == 2 || i == 5 || i == 8) ? "Great Staff" : slots[s].ToString();
                    Bases.Add(new ArpgItemBaseDefinition30
                    {
                        id = "ae32.base." + slots[s].ToString().ToLowerInvariant() + "." + i.ToString("00"),
                        displayName = themes[i] + " " + nouns[s],
                        slot = slots[s],
                        family = family,
                        affinity = i % 4 == 3 ? ArpgClass30.Unchosen : (ArpgClass30)(i % 3),
                        requiredLevel = i * 2,
                        implicitModifiers = new List<ArpgStatModifier30>
                        {
                            new ArpgStatModifier30(stat, flat ? 1f + i * 2f : 0.03f + i * 0.008f)
                        }
                    });
                }
            }
        }

        private sealed class Seed
        {
            public string id, name; public ArpgStat30 stat; public bool prefix, local;
            public Seed(string i, string n, ArpgStat30 s, bool p, bool l) { id=i; name=n; stat=s; prefix=p; local=l; }
        }

        private static void BuildAffixes()
        {
            Seed[] seeds =
            {
                new Seed("potency","Potency",ArpgStat30.SpellPower,true,false), new Seed("vitality","Vitality",ArpgStat30.MaximumHealth,true,false),
                new Seed("reservoir","Reservoir",ArpgStat30.MaximumMana,true,false), new Seed("precision","Precision",ArpgStat30.CriticalChance,true,false),
                new Seed("velocity","Velocity",ArpgStat30.MoveSpeed,false,false), new Seed("tempo","Tempo",ArpgStat30.CooldownRecovery,false,false),
                new Seed("efficiency","Efficiency",ArpgStat30.ManaEfficiency,false,false), new Seed("restoration","Restoration",ArpgStat30.Healing,false,false),
                new Seed("fortune","Fortune",ArpgStat30.ItemRarity,false,false), new Seed("currency","Prosperity",ArpgStat30.CurrencyFind,false,false),
                new Seed("experience","Insight",ArpgStat30.ExperienceGain,false,false), new Seed("attunement","Attunement",ArpgStat30.Attunement,true,false),
                new Seed("armour","Armour",ArpgStat30.Armour,true,true), new Seed("evasion","Evasion",ArpgStat30.Evasion,true,true),
                new Seed("ward","Arcane Ward",ArpgStat30.ArcaneWard,true,true), new Seed("flame","Flame",ArpgStat30.FirePower,true,false),
                new Seed("frost","Frost",ArpgStat30.ColdPower,true,false), new Seed("storm","Storm",ArpgStat30.LightningPower,true,false),
                new Seed("impact","Impact",ArpgStat30.PhysicalPower,true,false), new Seed("blood","Blood",ArpgStat30.BloodPower,true,false),
                new Seed("toxic","Toxicity",ArpgStat30.ToxicPower,true,false), new Seed("void","Void",ArpgStat30.VoidPower,true,false),
                new Seed("buildup","Catalysis",ArpgStat30.AilmentBuildup,false,false), new Seed("reaction","Reaction",ArpgStat30.ReactionPower,false,false),
                new Seed("area","Expansion",ArpgStat30.AreaOfEffect,false,false), new Seed("duration","Persistence",ArpgStat30.Duration,false,false),
                new Seed("projectile","Projectile Velocity",ArpgStat30.ProjectileSpeed,false,false), new Seed("chain","Chain Retention",ArpgStat30.ChainRetention,false,false),
                new Seed("runes","Rune Capacity",ArpgStat30.RuneCapacity,true,false), new Seed("barrier","Barrier",ArpgStat30.BarrierStrength,true,false),
                new Seed("mapq","Cartographer Yield",ArpgStat30.MapQuantity,false,false), new Seed("mapr","Cartographer Fortune",ArpgStat30.MapRarity,false,false),
                new Seed("maps","Cartographer Continuity",ArpgStat30.MapSustain,false,false), new Seed("trigger","Trigger Energy",ArpgStat30.TriggerEnergy,true,false),
                new Seed("gold","Acquisition",ArpgStat30.GoldFind,false,false), new Seed("fire-react","Burning Catalyst",ArpgStat30.ReactionPower,true,false),
                new Seed("cold-control","Winter Control",ArpgStat30.Duration,true,false), new Seed("light-chain","Storm Circuit",ArpgStat30.ChainRetention,true,false),
                new Seed("physical-barrier","Impact Barrier",ArpgStat30.BarrierStrength,true,false), new Seed("blood-recovery","Crimson Recovery",ArpgStat30.Healing,true,false),
                new Seed("toxic-duration","Venom Persistence",ArpgStat30.Duration,true,false), new Seed("void-area","Void Expansion",ArpgStat30.AreaOfEffect,true,false)
            };
            ArpgItemSlot30[] all = (ArpgItemSlot30[])Enum.GetValues(typeof(ArpgItemSlot30));
            foreach (Seed seed in seeds)
            {
                for (int t = 1; t <= 6; t++)
                {
                    bool flat = seed.stat == ArpgStat30.MaximumHealth || seed.stat == ArpgStat30.MaximumMana ||
                                seed.stat == ArpgStat30.Armour || seed.stat == ArpgStat30.Evasion || seed.stat == ArpgStat30.ArcaneWard ||
                                seed.stat == ArpgStat30.TriggerEnergy || seed.stat == ArpgStat30.RuneCapacity || seed.stat == ArpgStat30.Attunement;
                    float lo = flat ? 4f + t * 7f : 0.008f + t * 0.012f;
                    float hi = flat ? lo + 6f + t * 4f : lo + 0.012f + t * 0.008f;
                    Affixes.Add(new ArpgAffixDefinition30
                    {
                        id="ae32.affix."+seed.id+".t"+t, displayName=seed.name+" "+Roman(t), prefix=seed.prefix, stat=seed.stat,
                        minimum=lo, maximum=hi, minimumItemLevel=(t-1)*8, weight=Mathf.Max(8,110-t*13),
                        family="ae32."+seed.id, conflictGroup="ae32."+seed.id, local=seed.local, validSlots=all.ToList()
                    });
                }
            }
        }

        private static void BuildMaps()
        {
            string[] names =
            {
                "Ravenous","Accelerated","Deadly","Multishot","Critical","Elemental Conversion","Catalytic","Fortified","Armoured",
                "Elusive","Warded","Barriered","Resistant","Reaction-Hardened","Unyielding","Withering Recovery","Expensive Casting",
                "Fractured Barriers","Burning Ground","Frozen Ground","Storm Fields","Blood Pools","Toxic Mire","Void Rifts","Swarming",
                "Elite Patrols","Rare Commanders","Specialist Reinforcements","Hazard Saturation","Empowered Guardian","Abundant",
                "Precious","Currency-Rich","Unique-Rich","Exceptional-Rich","Discovery-Rich"
            };
            string[] flags =
            {
                "monster-damage","monster-speed","monster-critical-damage","additional-projectiles","monster-critical","conversion",
                "ailment-buildup","monster-life","monster-armour","monster-evasion","monster-ward","monster-barrier",
                "monster-resistance","reaction-resistance","control-resistance","reduced-recovery","increased-cost","reduced-barrier",
                "fire-ground","cold-ground","lightning-ground","blood-ground","toxic-ground","void-ground",
                "pack-size","magic-enemies","rare-enemies","specialists","hazards","boss-power",
                "item-quantity","item-rarity","currency","unique","exceptional","discoveries"
            };
            for (int i=0;i<names.Length;i++)
            {
                Maps.Add(new ArpgMapAffixDefinition30
                {
                    id="ae32.map."+i.ToString("00"), displayName=names[i],
                    description=i>=30 ? "Increases "+names[i].ToLowerInvariant()+" rewards." : "Adds "+names[i].ToLowerInvariant()+" pressure.",
                    minimumTier=i<12?0:i<24?2:3, rewardMultiplier=1.04f+(i%6)*0.025f, difficultyFlag=flags[i]
                });
            }
        }

        private static void BuildCurrencies()
        {
            AddCurrency(ArpgCurrency32.FluxShard,"Flux Shard","Rerolls all modifiers on a Magic item.","Common");
            AddCurrency(ArpgCurrency32.BindingSeal,"Binding Seal","Adds one modifier to a Magic item.","Common");
            AddCurrency(ArpgCurrency32.SovereignEmber,"Sovereign Ember","Upgrades Magic equipment to Rare.","Uncommon");
            AddCurrency(ArpgCurrency32.AstralNeedle,"Astral Needle","Adds one modifier to a Rare item.","Uncommon");
            AddCurrency(ArpgCurrency32.TemperingPrism,"Tempering Prism","Rerolls values within current tiers.","Common");
            AddCurrency(ArpgCurrency32.ArtisansMeasure,"Artisan's Measure","Improves quality.","Common");
            AddCurrency(ArpgCurrency32.SeveranceChisel,"Severance Chisel","Removes a removable modifier.","Rare");
            AddCurrency(ArpgCurrency32.TransferenceSigil,"Transference Sigil","Replaces a modifier.","Rare");
            AddCurrency(ArpgCurrency32.ResonantBrand,"Resonant Brand","Forces an elemental family.","Uncommon");
            AddCurrency(ArpgCurrency32.AscendantSpark,"Ascendant Spark","Raises one modifier tier.","Rare");
            AddCurrency(ArpgCurrency32.PrefixWard,"Prefix Ward","Protects prefixes during a reforge.","Rare");
            AddCurrency(ArpgCurrency32.SuffixWard,"Suffix Ward","Protects suffixes during a reforge.","Rare");
            AddCurrency(ArpgCurrency32.MemoryLoom,"Memory Loom","Reforges while preserving one modifier.","Very Rare");
            AddCurrency(ArpgCurrency32.FractureRune,"Fracture Rune","Permanently fractures one modifier.","Very Rare");
            AddCurrency(ArpgCurrency32.OmenOfRevision,"Omen of Revision","Modifies the next crafting action.","Very Rare");
        }
        private static void AddCurrency(ArpgCurrency32 id,string n,string d,string r) { Currencies.Add(new ArpgCurrencyDefinition32{id=id,displayName=n,description=d,rarity=r}); }

        private static void BuildUniques()
        {
            string[] names =
            {
                "Crown of Returning Stars","The Held Thunder","Ring of the Singular Flame","Unstable Weaver's Reliquary",
                "Footsteps of the Pale Road","The Third Covenant","Iron Incantation","Void Tax Collector","Crimson Reprieve",
                "Orbit of the Returning Hand","Catalyst's Verdict","Broken Geometry","Heart-Anvil","Mireheart's Seed",
                "Eye of the Stormcoil","Matron's Frozen Shell","Ashen Grasp","Seconds Stolen","Bastion Without End",
                "Constellation Eater","Event Horizon Band","Cartographer's Debt","Sevenfold Catalyst","Hand Beyond the Board"
            };
            string[] descriptions =
            {
                "Chain retention becomes projectile count.","Barrier damage is stored and released as Lightning power.",
                "Fire buildup is doubled while Cold buildup is disabled.","Disconnected Runes remain active at reduced authority.",
                "Sustained movement creates a Cold field.","Allows one additional Spell Link.","Armour contributes to Physical spell impact.",
                "Arcane Ward can be consumed to amplify Void.","Excess Toxic buildup becomes Blood recovery.",
                "Returning projectiles leave persistent fields.","Critical strikes become reaction-critical events.",
                "The first disconnected Rune is treated as adjacent.","Heavy impacts spend Life to gain Blood power.",
                "Persistent fields grow when enemies die inside them.","Chain events grant movement speed.",
                "Arcane Ward hardens while enemies are chilled.","Ignites intensify from repeated direct hits.",
                "Return and Delay effects recover faster while moving.","Barrier overflow becomes Armour.",
                "Mastery completion grants additional Attunement.","Area contracts while Void power rises.",
                "Failed maps increase later map quantity.","All seven elements contribute to reaction participation.",
                "Rune capacity increases after Mastery completion."
            };
            ArpgStat30[] stats =
            {
                ArpgStat30.ChainRetention,ArpgStat30.BarrierStrength,ArpgStat30.FirePower,ArpgStat30.RuneCapacity,
                ArpgStat30.ColdPower,ArpgStat30.TriggerEnergy,ArpgStat30.Armour,ArpgStat30.VoidPower,
                ArpgStat30.Healing,ArpgStat30.Duration,ArpgStat30.ReactionPower,ArpgStat30.RuneCapacity,
                ArpgStat30.BloodPower,ArpgStat30.ToxicPower,ArpgStat30.LightningPower,ArpgStat30.ArcaneWard,
                ArpgStat30.FirePower,ArpgStat30.CooldownRecovery,ArpgStat30.BarrierStrength,ArpgStat30.Attunement,
                ArpgStat30.VoidPower,ArpgStat30.MapQuantity,ArpgStat30.AilmentBuildup,ArpgStat30.RuneCapacity
            };
            string[] bossIds={"boss.ember-warden","boss.stormcoil-behemoth","boss.bone-regent","boss.astral-sentinel","boss.frostbound-matron","boss.mireheart"};
            string[] constraints=
            {
                "Linked direct spell power is reduced.","Barrier recovery is slower.","Cold buildup is disabled.","Disconnected Runes increase resource cost.",
                "Maximum movement speed is reduced.","Linked spells lose direct power.","Evasion is disabled.","Ward recovery pauses after Void casting.",
                "Maximum Toxic power is reduced.","Projectile speed is reduced.","Ordinary critical chance is reduced.","Connected Rune power is reduced.",
                "Heavy impacts spend Life.","Direct Toxic hits lose power.","Standing still reduces Lightning power.","Fire exposure weakens Ward.",
                "Non-Fire buildup is reduced.","Maximum Life is reduced.","Barrier cannot recover while full.","Constellation refunds cost Gold.",
                "Area of effect is reduced.","Boss reward rarity is reduced.","Single-element damage is reduced.","Rune power decays with board distance."
            };
            for(int i=0;i<24;i++)
            {
                ArpgItemBaseDefinition30 b=Bases[(i*5)%Bases.Count];
                Uniques.Add(new ArpgUniqueDefinition32
                {
                    id="ae32.unique."+i.ToString("00"),displayName=names[i],baseId=b.id,description=descriptions[i],
                    constraint=constraints[i],bossId=i<bossIds.Length?bossIds[i]:string.Empty,
                    modifiers=new List<ArpgStatModifier30>{new ArpgStatModifier30(stats[i], stats[i]==ArpgStat30.RuneCapacity||stats[i]==ArpgStat30.Attunement?2f:0.32f)}
                });
            }
        }

        private static void BuildCorruptions()
        {
            AddCorrupt("flame","Ashen Brand",ArpgStat30.FirePower,0.16f,ArpgStat30.ColdPower,-0.08f);
            AddCorrupt("frost","Pale Brand",ArpgStat30.ColdPower,0.16f,ArpgStat30.FirePower,-0.08f);
            AddCorrupt("storm","Storm Brand",ArpgStat30.LightningPower,0.16f,ArpgStat30.BarrierStrength,-0.08f);
            AddCorrupt("iron","Iron Brand",ArpgStat30.Armour,45f,ArpgStat30.MoveSpeed,-0.05f);
            AddCorrupt("blood","Crimson Brand",ArpgStat30.BloodPower,0.18f,ArpgStat30.MaximumHealth,-18f);
            AddCorrupt("toxic","Venom Brand",ArpgStat30.ToxicPower,0.18f,ArpgStat30.Healing,-0.07f);
            AddCorrupt("void","Void Brand",ArpgStat30.VoidPower,0.18f,ArpgStat30.ArcaneWard,-24f);
            AddCorrupt("chain","Broken Circuit",ArpgStat30.ChainRetention,0.16f,ArpgStat30.ProjectileSpeed,-0.08f);
            AddCorrupt("area","Collapsed Horizon",ArpgStat30.AreaOfEffect,0.18f,ArpgStat30.Duration,-0.08f);
            AddCorrupt("duration","Endless Moment",ArpgStat30.Duration,0.2f,ArpgStat30.CooldownRecovery,-0.07f);
            AddCorrupt("runes","Frayed Weave",ArpgStat30.RuneCapacity,1f,ArpgStat30.ManaEfficiency,-0.08f);
            AddCorrupt("fortune","Black Fortune",ArpgStat30.ItemRarity,0.14f,ArpgStat30.MapSustain,-0.08f);
        }
        private static void AddCorrupt(string id,string name,ArpgStat30 s,float v,ArpgStat30 ps,float pv)
        {
            Corruptions.Add(new ArpgCorruptedImplicitDefinition32{id="ae32.corrupt."+id,displayName=name,description="Power with an irreversible penalty.",modifier=new ArpgStatModifier30(s,v),penalty=new ArpgStatModifier30(ps,pv)});
        }
        private static string Roman(int v) { string[] r={"I","II","III","IV","V","VI"}; return r[Mathf.Clamp(v-1,0,5)]; }
    }
}
