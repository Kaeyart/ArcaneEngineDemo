using System.Collections.Generic;

namespace ArcaneEngine
{
    public static partial class ElementalReactionCodex
    {
        private static void PopulateGenerated(
            Dictionary<int, ElementalReactionDefinition> definitions)
        {
            definitions.Add(
                0x03,
                new ElementalReactionDefinition(
                    "reaction_03",
                    ReactionElement.Fire | ReactionElement.Cold,
                    "Thermal Shock",
                    ReactionTier.Fusion,
                    "Fusion of Fire, Cold. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x03.",
                    ReactionElement.Cold,
                    1.230f,
                    3.450f,
                    4.180f,
                    2,
                    2,
                    1.250f));
            definitions.Add(
                0x05,
                new ElementalReactionDefinition(
                    "reaction_05",
                    ReactionElement.Fire | ReactionElement.Lightning,
                    "Plasma",
                    ReactionTier.Fusion,
                    "Fusion of Fire, Lightning. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x05.",
                    ReactionElement.Lightning,
                    1.330f,
                    3.240f,
                    4.090f,
                    2,
                    2,
                    1.210f));
            definitions.Add(
                0x09,
                new ElementalReactionDefinition(
                    "reaction_09",
                    ReactionElement.Fire | ReactionElement.Physical,
                    "Blastwave",
                    ReactionTier.Fusion,
                    "Fusion of Fire, Physical. It combines burning, spreading flames and area denial; impact, armor break, stagger and displacement. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x09.",
                    ReactionElement.Physical,
                    1.530f,
                    3.520f,
                    3.910f,
                    2,
                    1,
                    1.210f));
            definitions.Add(
                0x11,
                new ElementalReactionDefinition(
                    "reaction_11",
                    ReactionElement.Fire | ReactionElement.Blood,
                    "Searing Hemorrhage",
                    ReactionTier.Fusion,
                    "Fusion of Fire, Blood. It combines burning, spreading flames and area denial; wounds, execution pressure and organic spread. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x11.",
                    ReactionElement.Fire,
                    1.280f,
                    3.520f,
                    4.450f,
                    4,
                    2,
                    1.170f));
            definitions.Add(
                0x21,
                new ElementalReactionDefinition(
                    "reaction_21",
                    ReactionElement.Fire | ReactionElement.Toxic,
                    "Combustion",
                    ReactionTier.Fusion,
                    "Fusion of Fire, Toxic. It combines burning, spreading flames and area denial; poison, contamination and persistent clouds. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x21.",
                    ReactionElement.Fire,
                    1.430f,
                    3.450f,
                    3.640f,
                    2,
                    1,
                    1.250f));
            definitions.Add(
                0x41,
                new ElementalReactionDefinition(
                    "reaction_41",
                    ReactionElement.Fire | ReactionElement.Void,
                    "Blackflame",
                    ReactionTier.Fusion,
                    "Fusion of Fire, Void. It combines burning, spreading flames and area denial; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x41.",
                    ReactionElement.Void,
                    1.455f,
                    3.380f,
                    4.180f,
                    3,
                    1,
                    1.250f));
            definitions.Add(
                0x06,
                new ElementalReactionDefinition(
                    "reaction_06",
                    ReactionElement.Cold | ReactionElement.Lightning,
                    "Superconduct",
                    ReactionTier.Fusion,
                    "Fusion of Cold, Lightning. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x06.",
                    ReactionElement.Cold,
                    1.405f,
                    3.520f,
                    3.640f,
                    4,
                    1,
                    1.250f));
            definitions.Add(
                0x0A,
                new ElementalReactionDefinition(
                    "reaction_0A",
                    ReactionElement.Cold | ReactionElement.Physical,
                    "Shatter",
                    ReactionTier.Fusion,
                    "Fusion of Cold, Physical. It combines chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x0A.",
                    ReactionElement.Cold,
                    1.280f,
                    3.170f,
                    4.450f,
                    4,
                    2,
                    1.210f));
            definitions.Add(
                0x12,
                new ElementalReactionDefinition(
                    "reaction_12",
                    ReactionElement.Cold | ReactionElement.Blood,
                    "Crimson Frost",
                    ReactionTier.Fusion,
                    "Fusion of Cold, Blood. It combines chill, freeze, brittleness and shattering; wounds, execution pressure and organic spread. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x12.",
                    ReactionElement.Blood,
                    1.355f,
                    3.170f,
                    4.000f,
                    3,
                    2,
                    1.210f));
            definitions.Add(
                0x22,
                new ElementalReactionDefinition(
                    "reaction_22",
                    ReactionElement.Cold | ReactionElement.Toxic,
                    "Cryotoxin",
                    ReactionTier.Fusion,
                    "Fusion of Cold, Toxic. It combines chill, freeze, brittleness and shattering; poison, contamination and persistent clouds. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x22.",
                    ReactionElement.Toxic,
                    1.505f,
                    3.100f,
                    4.180f,
                    4,
                    1,
                    1.250f));
            definitions.Add(
                0x42,
                new ElementalReactionDefinition(
                    "reaction_42",
                    ReactionElement.Cold | ReactionElement.Void,
                    "Entropic Prison",
                    ReactionTier.Fusion,
                    "Fusion of Cold, Void. It combines chill, freeze, brittleness and shattering; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x42.",
                    ReactionElement.Cold,
                    1.480f,
                    2.960f,
                    4.540f,
                    4,
                    1,
                    1.250f));
            definitions.Add(
                0x0C,
                new ElementalReactionDefinition(
                    "reaction_0C",
                    ReactionElement.Lightning | ReactionElement.Physical,
                    "Thunderclap",
                    ReactionTier.Fusion,
                    "Fusion of Lightning, Physical. It combines chains, rapid pulses and conduction; impact, armor break, stagger and displacement. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x0C.",
                    ReactionElement.Physical,
                    1.380f,
                    3.030f,
                    4.360f,
                    3,
                    2,
                    1.210f));
            definitions.Add(
                0x14,
                new ElementalReactionDefinition(
                    "reaction_14",
                    ReactionElement.Lightning | ReactionElement.Blood,
                    "Neuroshock",
                    ReactionTier.Fusion,
                    "Fusion of Lightning, Blood. It combines chains, rapid pulses and conduction; wounds, execution pressure and organic spread. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x14.",
                    ReactionElement.Blood,
                    1.455f,
                    2.960f,
                    3.910f,
                    3,
                    1,
                    1.170f));
            definitions.Add(
                0x24,
                new ElementalReactionDefinition(
                    "reaction_24",
                    ReactionElement.Lightning | ReactionElement.Toxic,
                    "Ionized Miasma",
                    ReactionTier.Fusion,
                    "Fusion of Lightning, Toxic. It combines chains, rapid pulses and conduction; poison, contamination and persistent clouds. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x24.",
                    ReactionElement.Toxic,
                    1.280f,
                    3.520f,
                    4.090f,
                    4,
                    2,
                    1.250f));
            definitions.Add(
                0x44,
                new ElementalReactionDefinition(
                    "reaction_44",
                    ReactionElement.Lightning | ReactionElement.Void,
                    "Gravity Storm",
                    ReactionTier.Fusion,
                    "Fusion of Lightning, Void. It combines chains, rapid pulses and conduction; pulling, spatial distortion and collapse. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x44.",
                    ReactionElement.Lightning,
                    1.255f,
                    3.380f,
                    4.450f,
                    4,
                    2,
                    1.210f));
            definitions.Add(
                0x18,
                new ElementalReactionDefinition(
                    "reaction_18",
                    ReactionElement.Physical | ReactionElement.Blood,
                    "Rupture",
                    ReactionTier.Fusion,
                    "Fusion of Physical, Blood. It combines impact, armor break, stagger and displacement; wounds, execution pressure and organic spread. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x18.",
                    ReactionElement.Physical,
                    1.330f,
                    3.310f,
                    3.730f,
                    2,
                    2,
                    1.170f));
            definitions.Add(
                0x28,
                new ElementalReactionDefinition(
                    "reaction_28",
                    ReactionElement.Physical | ReactionElement.Toxic,
                    "Aerosolization",
                    ReactionTier.Fusion,
                    "Fusion of Physical, Toxic. It combines impact, armor break, stagger and displacement; poison, contamination and persistent clouds. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x28.",
                    ReactionElement.Toxic,
                    1.480f,
                    3.240f,
                    3.910f,
                    4,
                    1,
                    1.210f));
            definitions.Add(
                0x48,
                new ElementalReactionDefinition(
                    "reaction_48",
                    ReactionElement.Physical | ReactionElement.Void,
                    "Implosion",
                    ReactionTier.Fusion,
                    "Fusion of Physical, Void. It combines impact, armor break, stagger and displacement; pulling, spatial distortion and collapse. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x48.",
                    ReactionElement.Void,
                    1.455f,
                    3.100f,
                    4.180f,
                    3,
                    1,
                    1.210f));
            definitions.Add(
                0x30,
                new ElementalReactionDefinition(
                    "reaction_30",
                    ReactionElement.Blood | ReactionElement.Toxic,
                    "Sepsis",
                    ReactionTier.Fusion,
                    "Fusion of Blood, Toxic. It combines wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x30.",
                    ReactionElement.Blood,
                    1.230f,
                    3.170f,
                    4.450f,
                    3,
                    1,
                    1.170f));
            definitions.Add(
                0x50,
                new ElementalReactionDefinition(
                    "reaction_50",
                    ReactionElement.Blood | ReactionElement.Void,
                    "Soul Rupture",
                    ReactionTier.Fusion,
                    "Fusion of Blood, Void. It combines wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x50.",
                    ReactionElement.Void,
                    1.255f,
                    3.100f,
                    4.090f,
                    4,
                    1,
                    1.210f));
            definitions.Add(
                0x60,
                new ElementalReactionDefinition(
                    "reaction_60",
                    ReactionElement.Toxic | ReactionElement.Void,
                    "Decay Well",
                    ReactionTier.Fusion,
                    "Fusion of Toxic, Void. It combines poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x60.",
                    ReactionElement.Void,
                    1.405f,
                    3.030f,
                    4.270f,
                    2,
                    2,
                    1.250f));
            definitions.Add(
                0x07,
                new ElementalReactionDefinition(
                    "reaction_07",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning,
                    "Cinder Rime Volt Tempest",
                    ReactionTier.Compound,
                    "Compound of Fire, Cold, Lightning. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x07.",
                    ReactionElement.Lightning,
                    1.770f,
                    3.580f,
                    4.720f,
                    3,
                    2,
                    1.135f));
            definitions.Add(
                0x0B,
                new ElementalReactionDefinition(
                    "reaction_0B",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Physical,
                    "Cinder Rime Ruin Maelstrom",
                    ReactionTier.Compound,
                    "Compound of Fire, Cold, Physical. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x0B.",
                    ReactionElement.Cold,
                    1.645f,
                    3.860f,
                    4.450f,
                    5,
                    3,
                    1.135f));
            definitions.Add(
                0x13,
                new ElementalReactionDefinition(
                    "reaction_13",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Blood,
                    "Cinder Rime Crimson Cascade",
                    ReactionTier.Compound,
                    "Compound of Fire, Cold, Blood. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; wounds, execution pressure and organic spread. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x13.",
                    ReactionElement.Fire,
                    1.720f,
                    3.860f,
                    5.080f,
                    5,
                    2,
                    1.095f));
            definitions.Add(
                0x23,
                new ElementalReactionDefinition(
                    "reaction_23",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Toxic,
                    "Cinder Rime Miasma Confluence",
                    ReactionTier.Compound,
                    "Compound of Fire, Cold, Toxic. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; poison, contamination and persistent clouds. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x23.",
                    ReactionElement.Fire,
                    1.870f,
                    3.790f,
                    5.260f,
                    3,
                    2,
                    1.135f));
            definitions.Add(
                0x43,
                new ElementalReactionDefinition(
                    "reaction_43",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Void,
                    "Cinder Rime Umbral Tempest",
                    ReactionTier.Compound,
                    "Compound of Fire, Cold, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x43.",
                    ReactionElement.Fire,
                    1.570f,
                    3.720f,
                    4.810f,
                    4,
                    2,
                    1.175f));
            definitions.Add(
                0x0D,
                new ElementalReactionDefinition(
                    "reaction_0D",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Physical,
                    "Cinder Volt Ruin Maelstrom",
                    ReactionTier.Compound,
                    "Compound of Fire, Lightning, Physical. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; impact, armor break, stagger and displacement. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x0D.",
                    ReactionElement.Fire,
                    1.745f,
                    3.720f,
                    4.360f,
                    5,
                    2,
                    1.135f));
            definitions.Add(
                0x15,
                new ElementalReactionDefinition(
                    "reaction_15",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Blood,
                    "Cinder Volt Crimson Cascade",
                    ReactionTier.Compound,
                    "Compound of Fire, Lightning, Blood. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; wounds, execution pressure and organic spread. Fire is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x15.",
                    ReactionElement.Fire,
                    1.820f,
                    3.650f,
                    4.990f,
                    4,
                    2,
                    1.095f));
            definitions.Add(
                0x25,
                new ElementalReactionDefinition(
                    "reaction_25",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Toxic,
                    "Cinder Volt Miasma Confluence",
                    ReactionTier.Compound,
                    "Compound of Fire, Lightning, Toxic. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; poison, contamination and persistent clouds. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x25.",
                    ReactionElement.Fire,
                    1.695f,
                    3.650f,
                    4.360f,
                    4,
                    3,
                    1.175f));
            definitions.Add(
                0x45,
                new ElementalReactionDefinition(
                    "reaction_45",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Void,
                    "Cinder Volt Umbral Tempest",
                    ReactionTier.Compound,
                    "Compound of Fire, Lightning, Void. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; pulling, spatial distortion and collapse. Fire is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x45.",
                    ReactionElement.Void,
                    1.670f,
                    3.510f,
                    4.720f,
                    4,
                    3,
                    1.175f));
            definitions.Add(
                0x19,
                new ElementalReactionDefinition(
                    "reaction_19",
                    ReactionElement.Fire | ReactionElement.Physical | ReactionElement.Blood,
                    "Cinder Ruin Crimson Confluence",
                    ReactionTier.Compound,
                    "Compound of Fire, Physical, Blood. It combines burning, spreading flames and area denial; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x19.",
                    ReactionElement.Physical,
                    1.695f,
                    3.930f,
                    4.720f,
                    4,
                    3,
                    1.175f));
            definitions.Add(
                0x29,
                new ElementalReactionDefinition(
                    "reaction_29",
                    ReactionElement.Fire | ReactionElement.Physical | ReactionElement.Toxic,
                    "Cinder Ruin Miasma Cascade",
                    ReactionTier.Compound,
                    "Compound of Fire, Physical, Toxic. It combines burning, spreading flames and area denial; impact, armor break, stagger and displacement; poison, contamination and persistent clouds. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x29.",
                    ReactionElement.Toxic,
                    1.570f,
                    3.930f,
                    5.170f,
                    3,
                    2,
                    1.175f));
            definitions.Add(
                0x49,
                new ElementalReactionDefinition(
                    "reaction_49",
                    ReactionElement.Fire | ReactionElement.Physical | ReactionElement.Void,
                    "Cinder Ruin Umbral Maelstrom",
                    ReactionTier.Compound,
                    "Compound of Fire, Physical, Void. It combines burning, spreading flames and area denial; impact, armor break, stagger and displacement; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x49.",
                    ReactionElement.Physical,
                    1.870f,
                    3.790f,
                    4.540f,
                    3,
                    2,
                    1.135f));
            definitions.Add(
                0x31,
                new ElementalReactionDefinition(
                    "reaction_31",
                    ReactionElement.Fire | ReactionElement.Blood | ReactionElement.Toxic,
                    "Cinder Crimson Miasma Bloom",
                    ReactionTier.Compound,
                    "Compound of Fire, Blood, Toxic. It combines burning, spreading flames and area denial; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x31.",
                    ReactionElement.Blood,
                    1.645f,
                    3.930f,
                    4.810f,
                    3,
                    3,
                    1.135f));
            definitions.Add(
                0x51,
                new ElementalReactionDefinition(
                    "reaction_51",
                    ReactionElement.Fire | ReactionElement.Blood | ReactionElement.Void,
                    "Cinder Crimson Umbral Cascade",
                    ReactionTier.Compound,
                    "Compound of Fire, Blood, Void. It combines burning, spreading flames and area denial; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Fire is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x51.",
                    ReactionElement.Fire,
                    1.620f,
                    3.790f,
                    5.080f,
                    3,
                    3,
                    1.095f));
            definitions.Add(
                0x61,
                new ElementalReactionDefinition(
                    "reaction_61",
                    ReactionElement.Fire | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder Miasma Umbral Confluence",
                    ReactionTier.Compound,
                    "Compound of Fire, Toxic, Void. It combines burning, spreading flames and area denial; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x61.",
                    ReactionElement.Fire,
                    1.770f,
                    3.720f,
                    5.260f,
                    4,
                    3,
                    1.175f));
            definitions.Add(
                0x0E,
                new ElementalReactionDefinition(
                    "reaction_0E",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical,
                    "Rime Volt Ruin Cascade",
                    ReactionTier.Compound,
                    "Compound of Cold, Lightning, Physical. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x0E.",
                    ReactionElement.Cold,
                    1.820f,
                    3.930f,
                    4.990f,
                    4,
                    2,
                    1.135f));
            definitions.Add(
                0x16,
                new ElementalReactionDefinition(
                    "reaction_16",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Blood,
                    "Rime Volt Crimson Bloom",
                    ReactionTier.Compound,
                    "Compound of Cold, Lightning, Blood. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; wounds, execution pressure and organic spread. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x16.",
                    ReactionElement.Cold,
                    1.570f,
                    3.930f,
                    4.540f,
                    3,
                    2,
                    1.095f));
            definitions.Add(
                0x26,
                new ElementalReactionDefinition(
                    "reaction_26",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Toxic,
                    "Rime Volt Miasma Crucible",
                    ReactionTier.Compound,
                    "Compound of Cold, Lightning, Toxic. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; poison, contamination and persistent clouds. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x26.",
                    ReactionElement.Toxic,
                    1.720f,
                    3.860f,
                    4.720f,
                    5,
                    3,
                    1.135f));
            definitions.Add(
                0x46,
                new ElementalReactionDefinition(
                    "reaction_46",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Void,
                    "Rime Volt Umbral Confluence",
                    ReactionTier.Compound,
                    "Compound of Cold, Lightning, Void. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x46.",
                    ReactionElement.Lightning,
                    1.695f,
                    3.720f,
                    5.080f,
                    5,
                    3,
                    1.135f));
            definitions.Add(
                0x1A,
                new ElementalReactionDefinition(
                    "reaction_1A",
                    ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Blood,
                    "Rime Ruin Crimson Crucible",
                    ReactionTier.Compound,
                    "Compound of Cold, Physical, Blood. It combines chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x1A.",
                    ReactionElement.Physical,
                    1.770f,
                    3.580f,
                    4.360f,
                    3,
                    2,
                    1.095f));
            definitions.Add(
                0x2A,
                new ElementalReactionDefinition(
                    "reaction_2A",
                    ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Toxic,
                    "Rime Ruin Miasma Bloom",
                    ReactionTier.Compound,
                    "Compound of Cold, Physical, Toxic. It combines chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; poison, contamination and persistent clouds. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x2A.",
                    ReactionElement.Physical,
                    1.595f,
                    3.510f,
                    4.540f,
                    4,
                    3,
                    1.135f));
            definitions.Add(
                0x4A,
                new ElementalReactionDefinition(
                    "reaction_4A",
                    ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Void,
                    "Rime Ruin Umbral Cascade",
                    ReactionTier.Compound,
                    "Compound of Cold, Physical, Void. It combines chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; pulling, spatial distortion and collapse. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x4A.",
                    ReactionElement.Physical,
                    1.620f,
                    3.440f,
                    5.080f,
                    5,
                    2,
                    1.175f));
            definitions.Add(
                0x32,
                new ElementalReactionDefinition(
                    "reaction_32",
                    ReactionElement.Cold | ReactionElement.Blood | ReactionElement.Toxic,
                    "Rime Crimson Miasma Triune",
                    ReactionTier.Compound,
                    "Compound of Cold, Blood, Toxic. It combines chill, freeze, brittleness and shattering; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x32.",
                    ReactionElement.Cold,
                    1.670f,
                    3.510f,
                    5.080f,
                    4,
                    3,
                    1.095f));
            definitions.Add(
                0x52,
                new ElementalReactionDefinition(
                    "reaction_52",
                    ReactionElement.Cold | ReactionElement.Blood | ReactionElement.Void,
                    "Rime Crimson Umbral Bloom",
                    ReactionTier.Compound,
                    "Compound of Cold, Blood, Void. It combines chill, freeze, brittleness and shattering; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x52.",
                    ReactionElement.Blood,
                    1.695f,
                    3.440f,
                    4.720f,
                    4,
                    3,
                    1.135f));
            definitions.Add(
                0x62,
                new ElementalReactionDefinition(
                    "reaction_62",
                    ReactionElement.Cold | ReactionElement.Toxic | ReactionElement.Void,
                    "Rime Miasma Umbral Crucible",
                    ReactionTier.Compound,
                    "Compound of Cold, Toxic, Void. It combines chill, freeze, brittleness and shattering; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x62.",
                    ReactionElement.Cold,
                    1.845f,
                    4.000f,
                    4.810f,
                    3,
                    2,
                    1.175f));
            definitions.Add(
                0x1C,
                new ElementalReactionDefinition(
                    "reaction_1C",
                    ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood,
                    "Volt Ruin Crimson Crucible",
                    ReactionTier.Compound,
                    "Compound of Lightning, Physical, Blood. It combines chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x1C.",
                    ReactionElement.Lightning,
                    1.870f,
                    3.440f,
                    5.260f,
                    3,
                    2,
                    1.095f));
            definitions.Add(
                0x2C,
                new ElementalReactionDefinition(
                    "reaction_2C",
                    ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Toxic,
                    "Volt Ruin Miasma Bloom",
                    ReactionTier.Compound,
                    "Compound of Lightning, Physical, Toxic. It combines chains, rapid pulses and conduction; impact, armor break, stagger and displacement; poison, contamination and persistent clouds. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x2C.",
                    ReactionElement.Lightning,
                    1.695f,
                    4.000f,
                    4.360f,
                    4,
                    3,
                    1.135f));
            definitions.Add(
                0x4C,
                new ElementalReactionDefinition(
                    "reaction_4C",
                    ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Void,
                    "Volt Ruin Umbral Cascade",
                    ReactionTier.Compound,
                    "Compound of Lightning, Physical, Void. It combines chains, rapid pulses and conduction; impact, armor break, stagger and displacement; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x4C.",
                    ReactionElement.Lightning,
                    1.720f,
                    3.930f,
                    4.990f,
                    5,
                    3,
                    1.135f));
            definitions.Add(
                0x34,
                new ElementalReactionDefinition(
                    "reaction_34",
                    ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Toxic,
                    "Volt Crimson Miasma Triune",
                    ReactionTier.Compound,
                    "Compound of Lightning, Blood, Toxic. It combines chains, rapid pulses and conduction; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x34.",
                    ReactionElement.Lightning,
                    1.770f,
                    3.930f,
                    4.990f,
                    3,
                    2,
                    1.095f));
            definitions.Add(
                0x54,
                new ElementalReactionDefinition(
                    "reaction_54",
                    ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Void,
                    "Volt Crimson Umbral Bloom",
                    ReactionTier.Compound,
                    "Compound of Lightning, Blood, Void. It combines chains, rapid pulses and conduction; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x54.",
                    ReactionElement.Lightning,
                    1.795f,
                    3.860f,
                    4.540f,
                    4,
                    3,
                    1.135f));
            definitions.Add(
                0x64,
                new ElementalReactionDefinition(
                    "reaction_64",
                    ReactionElement.Lightning | ReactionElement.Toxic | ReactionElement.Void,
                    "Volt Miasma Umbral Crucible",
                    ReactionTier.Compound,
                    "Compound of Lightning, Toxic, Void. It combines chains, rapid pulses and conduction; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x64.",
                    ReactionElement.Void,
                    1.620f,
                    3.790f,
                    4.720f,
                    3,
                    2,
                    1.175f));
            definitions.Add(
                0x38,
                new ElementalReactionDefinition(
                    "reaction_38",
                    ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic,
                    "Ruin Crimson Miasma Paradox",
                    ReactionTier.Compound,
                    "Compound of Physical, Blood, Toxic. It combines impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x38.",
                    ReactionElement.Blood,
                    1.645f,
                    3.650f,
                    4.810f,
                    3,
                    3,
                    1.095f));
            definitions.Add(
                0x58,
                new ElementalReactionDefinition(
                    "reaction_58",
                    ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Void,
                    "Ruin Crimson Umbral Crucible",
                    ReactionTier.Compound,
                    "Compound of Physical, Blood, Void. It combines impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x58.",
                    ReactionElement.Blood,
                    1.670f,
                    3.580f,
                    4.360f,
                    4,
                    3,
                    1.095f));
            definitions.Add(
                0x68,
                new ElementalReactionDefinition(
                    "reaction_68",
                    ReactionElement.Physical | ReactionElement.Toxic | ReactionElement.Void,
                    "Ruin Miasma Umbral Bloom",
                    ReactionTier.Compound,
                    "Compound of Physical, Toxic, Void. It combines impact, armor break, stagger and displacement; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x68.",
                    ReactionElement.Toxic,
                    1.820f,
                    3.510f,
                    4.540f,
                    5,
                    2,
                    1.175f));
            definitions.Add(
                0x70,
                new ElementalReactionDefinition(
                    "reaction_70",
                    ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Crimson Miasma Umbral Triune",
                    ReactionTier.Compound,
                    "Compound of Blood, Toxic, Void. It combines wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x70.",
                    ReactionElement.Toxic,
                    1.570f,
                    3.440f,
                    5.080f,
                    4,
                    2,
                    1.135f));
            definitions.Add(
                0x0F,
                new ElementalReactionDefinition(
                    "reaction_0F",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical,
                    "Cinder–Rime–Volt–Ruin Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Lightning, Physical. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x0F.",
                    ReactionElement.Lightning,
                    2.185f,
                    3.990f,
                    5.980f,
                    6,
                    2,
                    1.020f));
            definitions.Add(
                0x17,
                new ElementalReactionDefinition(
                    "reaction_17",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Blood,
                    "Cinder–Rime–Volt–Crimson Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Lightning, Blood. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; wounds, execution pressure and organic spread. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x17.",
                    ReactionElement.Blood,
                    1.935f,
                    3.990f,
                    5.620f,
                    5,
                    3,
                    1.020f));
            definitions.Add(
                0x27,
                new ElementalReactionDefinition(
                    "reaction_27",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Toxic,
                    "Cinder–Rime–Volt–Miasma Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Lightning, Toxic. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; poison, contamination and persistent clouds. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x27.",
                    ReactionElement.Fire,
                    2.135f,
                    3.990f,
                    5.980f,
                    5,
                    2,
                    1.100f));
            definitions.Add(
                0x47,
                new ElementalReactionDefinition(
                    "reaction_47",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Void,
                    "Cinder–Rime–Volt–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Lightning, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x47.",
                    ReactionElement.Void,
                    2.110f,
                    4.480f,
                    5.350f,
                    5,
                    3,
                    1.060f));
            definitions.Add(
                0x1B,
                new ElementalReactionDefinition(
                    "reaction_1B",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Blood,
                    "Cinder–Rime–Ruin–Crimson Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Physical, Blood. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x1B.",
                    ReactionElement.Blood,
                    2.135f,
                    4.270f,
                    5.350f,
                    5,
                    2,
                    1.100f));
            definitions.Add(
                0x2B,
                new ElementalReactionDefinition(
                    "reaction_2B",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Toxic,
                    "Cinder–Rime–Ruin–Miasma Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Physical, Toxic. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; poison, contamination and persistent clouds. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x2B.",
                    ReactionElement.Cold,
                    2.010f,
                    4.270f,
                    5.800f,
                    4,
                    3,
                    1.060f));
            definitions.Add(
                0x4B,
                new ElementalReactionDefinition(
                    "reaction_4B",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Void,
                    "Cinder–Rime–Ruin–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Physical, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x4B.",
                    ReactionElement.Fire,
                    1.985f,
                    4.130f,
                    5.170f,
                    4,
                    3,
                    1.060f));
            definitions.Add(
                0x33,
                new ElementalReactionDefinition(
                    "reaction_33",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Blood | ReactionElement.Toxic,
                    "Cinder–Rime–Crimson–Miasma Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Blood, Toxic. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x33.",
                    ReactionElement.Blood,
                    2.085f,
                    4.270f,
                    5.350f,
                    6,
                    3,
                    1.060f));
            definitions.Add(
                0x53,
                new ElementalReactionDefinition(
                    "reaction_53",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Blood | ReactionElement.Void,
                    "Cinder–Rime–Crimson–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Blood, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x53.",
                    ReactionElement.Cold,
                    2.060f,
                    4.130f,
                    5.710f,
                    6,
                    3,
                    1.020f));
            definitions.Add(
                0x63,
                new ElementalReactionDefinition(
                    "reaction_63",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder–Rime–Miasma–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Cold, Toxic, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x63.",
                    ReactionElement.Toxic,
                    2.210f,
                    4.060f,
                    5.890f,
                    5,
                    2,
                    1.100f));
            definitions.Add(
                0x1D,
                new ElementalReactionDefinition(
                    "reaction_1D",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood,
                    "Cinder–Volt–Ruin–Crimson Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Lightning, Physical, Blood. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x1D.",
                    ReactionElement.Lightning,
                    1.910f,
                    4.130f,
                    5.260f,
                    5,
                    3,
                    1.100f));
            definitions.Add(
                0x2D,
                new ElementalReactionDefinition(
                    "reaction_2D",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Toxic,
                    "Cinder–Volt–Ruin–Miasma Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Lightning, Physical, Toxic. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; poison, contamination and persistent clouds. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x2D.",
                    ReactionElement.Toxic,
                    2.110f,
                    4.130f,
                    5.710f,
                    4,
                    3,
                    1.060f));
            definitions.Add(
                0x4D,
                new ElementalReactionDefinition(
                    "reaction_4D",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Void,
                    "Cinder–Volt–Ruin–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Lightning, Physical, Void. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x4D.",
                    ReactionElement.Physical,
                    2.085f,
                    3.990f,
                    5.080f,
                    4,
                    3,
                    1.060f));
            definitions.Add(
                0x35,
                new ElementalReactionDefinition(
                    "reaction_35",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Toxic,
                    "Cinder–Volt–Crimson–Miasma Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Lightning, Blood, Toxic. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x35.",
                    ReactionElement.Fire,
                    2.185f,
                    4.060f,
                    5.260f,
                    6,
                    2,
                    1.020f));
            definitions.Add(
                0x55,
                new ElementalReactionDefinition(
                    "reaction_55",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Void,
                    "Cinder–Volt–Crimson–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Lightning, Blood, Void. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x55.",
                    ReactionElement.Void,
                    2.160f,
                    3.920f,
                    5.620f,
                    6,
                    2,
                    1.020f));
            definitions.Add(
                0x65,
                new ElementalReactionDefinition(
                    "reaction_65",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder–Volt–Miasma–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Lightning, Toxic, Void. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x65.",
                    ReactionElement.Fire,
                    1.985f,
                    4.480f,
                    5.800f,
                    5,
                    3,
                    1.060f));
            definitions.Add(
                0x39,
                new ElementalReactionDefinition(
                    "reaction_39",
                    ReactionElement.Fire | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic,
                    "Cinder–Ruin–Crimson–Miasma Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Physical, Blood, Toxic. It combines burning, spreading flames and area denial; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x39.",
                    ReactionElement.Fire,
                    2.060f,
                    4.340f,
                    5.080f,
                    6,
                    3,
                    1.020f));
            definitions.Add(
                0x59,
                new ElementalReactionDefinition(
                    "reaction_59",
                    ReactionElement.Fire | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Void,
                    "Cinder–Ruin–Crimson–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Physical, Blood, Void. It combines burning, spreading flames and area denial; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x59.",
                    ReactionElement.Void,
                    2.035f,
                    4.200f,
                    5.440f,
                    6,
                    3,
                    1.020f));
            definitions.Add(
                0x69,
                new ElementalReactionDefinition(
                    "reaction_69",
                    ReactionElement.Fire | ReactionElement.Physical | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder–Ruin–Miasma–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Physical, Toxic, Void. It combines burning, spreading flames and area denial; impact, armor break, stagger and displacement; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x69.",
                    ReactionElement.Physical,
                    2.185f,
                    4.130f,
                    5.530f,
                    4,
                    2,
                    1.060f));
            definitions.Add(
                0x71,
                new ElementalReactionDefinition(
                    "reaction_71",
                    ReactionElement.Fire | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder–Crimson–Miasma–Umbral Event",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Fire, Blood, Toxic, Void. It combines burning, spreading flames and area denial; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x71.",
                    ReactionElement.Toxic,
                    1.985f,
                    4.200f,
                    5.440f,
                    4,
                    2,
                    1.060f));
            definitions.Add(
                0x1E,
                new ElementalReactionDefinition(
                    "reaction_1E",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood,
                    "Rime–Volt–Ruin–Crimson Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Lightning, Physical, Blood. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x1E.",
                    ReactionElement.Cold,
                    1.985f,
                    4.340f,
                    5.800f,
                    4,
                    3,
                    1.100f));
            definitions.Add(
                0x2E,
                new ElementalReactionDefinition(
                    "reaction_2E",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Toxic,
                    "Rime–Volt–Ruin–Miasma Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Lightning, Physical, Toxic. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; poison, contamination and persistent clouds. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x2E.",
                    ReactionElement.Physical,
                    2.135f,
                    4.270f,
                    5.980f,
                    5,
                    2,
                    1.060f));
            definitions.Add(
                0x4E,
                new ElementalReactionDefinition(
                    "reaction_4E",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Void,
                    "Rime–Volt–Ruin–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Lightning, Physical, Void. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; pulling, spatial distortion and collapse. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x4E.",
                    ReactionElement.Lightning,
                    2.160f,
                    4.200f,
                    5.620f,
                    6,
                    2,
                    1.060f));
            definitions.Add(
                0x36,
                new ElementalReactionDefinition(
                    "reaction_36",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Toxic,
                    "Rime–Volt–Crimson–Miasma Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Lightning, Blood, Toxic. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x36.",
                    ReactionElement.Toxic,
                    2.210f,
                    4.270f,
                    5.620f,
                    4,
                    2,
                    1.020f));
            definitions.Add(
                0x56,
                new ElementalReactionDefinition(
                    "reaction_56",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Void,
                    "Rime–Volt–Crimson–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Lightning, Blood, Void. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x56.",
                    ReactionElement.Blood,
                    1.910f,
                    4.200f,
                    5.170f,
                    5,
                    2,
                    1.020f));
            definitions.Add(
                0x66,
                new ElementalReactionDefinition(
                    "reaction_66",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Toxic | ReactionElement.Void,
                    "Rime–Volt–Miasma–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Lightning, Toxic, Void. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x66.",
                    ReactionElement.Cold,
                    2.060f,
                    4.130f,
                    5.350f,
                    6,
                    3,
                    1.100f));
            definitions.Add(
                0x3A,
                new ElementalReactionDefinition(
                    "reaction_3A",
                    ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic,
                    "Rime–Ruin–Crimson–Miasma Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Physical, Blood, Toxic. It combines chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x3A.",
                    ReactionElement.Toxic,
                    2.085f,
                    3.920f,
                    5.350f,
                    4,
                    3,
                    1.100f));
            definitions.Add(
                0x5A,
                new ElementalReactionDefinition(
                    "reaction_5A",
                    ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Void,
                    "Rime–Ruin–Crimson–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Physical, Blood, Void. It combines chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x5A.",
                    ReactionElement.Blood,
                    2.110f,
                    4.480f,
                    5.980f,
                    5,
                    3,
                    1.020f));
            definitions.Add(
                0x6A,
                new ElementalReactionDefinition(
                    "reaction_6A",
                    ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Toxic | ReactionElement.Void,
                    "Rime–Ruin–Miasma–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Physical, Toxic, Void. It combines chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x6A.",
                    ReactionElement.Cold,
                    1.935f,
                    4.410f,
                    5.170f,
                    6,
                    2,
                    1.060f));
            definitions.Add(
                0x72,
                new ElementalReactionDefinition(
                    "reaction_72",
                    ReactionElement.Cold | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Rime–Crimson–Miasma–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Cold, Blood, Toxic, Void. It combines chill, freeze, brittleness and shattering; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x72.",
                    ReactionElement.Blood,
                    2.010f,
                    4.410f,
                    5.710f,
                    5,
                    3,
                    1.020f));
            definitions.Add(
                0x3C,
                new ElementalReactionDefinition(
                    "reaction_3C",
                    ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic,
                    "Volt–Ruin–Crimson–Miasma Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Lightning, Physical, Blood, Toxic. It combines chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x3C.",
                    ReactionElement.Physical,
                    2.185f,
                    4.410f,
                    5.260f,
                    4,
                    2,
                    1.100f));
            definitions.Add(
                0x5C,
                new ElementalReactionDefinition(
                    "reaction_5C",
                    ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Void,
                    "Volt–Ruin–Crimson–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Lightning, Physical, Blood, Void. It combines chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x5C.",
                    ReactionElement.Physical,
                    2.210f,
                    4.340f,
                    5.890f,
                    4,
                    2,
                    1.020f));
            definitions.Add(
                0x6C,
                new ElementalReactionDefinition(
                    "reaction_6C",
                    ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Toxic | ReactionElement.Void,
                    "Volt–Ruin–Miasma–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Lightning, Physical, Toxic, Void. It combines chains, rapid pulses and conduction; impact, armor break, stagger and displacement; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x6C.",
                    ReactionElement.Toxic,
                    2.035f,
                    4.270f,
                    5.080f,
                    6,
                    3,
                    1.060f));
            definitions.Add(
                0x74,
                new ElementalReactionDefinition(
                    "reaction_74",
                    ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Volt–Crimson–Miasma–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Lightning, Blood, Toxic, Void. It combines chains, rapid pulses and conduction; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x74.",
                    ReactionElement.Void,
                    2.110f,
                    4.200f,
                    5.620f,
                    5,
                    3,
                    1.020f));
            definitions.Add(
                0x78,
                new ElementalReactionDefinition(
                    "reaction_78",
                    ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Ruin–Crimson–Miasma–Umbral Collapse",
                    ReactionTier.Catastrophe,
                    "Catastrophe of Physical, Blood, Toxic, Void. It combines impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x78.",
                    ReactionElement.Void,
                    1.985f,
                    3.920f,
                    5.440f,
                    5,
                    2,
                    1.020f));
            definitions.Add(
                0x1F,
                new ElementalReactionDefinition(
                    "reaction_1F",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood,
                    "Cinder Rime Convergence of Lightning–Physical–Blood",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Lightning, Physical, Blood. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x1F.",
                    ReactionElement.Cold,
                    2.350f,
                    4.400f,
                    5.890f,
                    5,
                    3,
                    1.025f));
            definitions.Add(
                0x2F,
                new ElementalReactionDefinition(
                    "reaction_2F",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Toxic,
                    "Cinder Rime Aetherstorm of Lightning–Physical–Toxic",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Lightning, Physical, Toxic. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; poison, contamination and persistent clouds. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x2F.",
                    ReactionElement.Lightning,
                    2.550f,
                    4.400f,
                    6.340f,
                    5,
                    2,
                    0.985f));
            definitions.Add(
                0x4F,
                new ElementalReactionDefinition(
                    "reaction_4F",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Void,
                    "Cinder Rime Devouring Crown of Lightning–Physical–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Lightning, Physical, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x4F.",
                    ReactionElement.Void,
                    2.525f,
                    4.890f,
                    6.610f,
                    5,
                    2,
                    0.985f));
            definitions.Add(
                0x37,
                new ElementalReactionDefinition(
                    "reaction_37",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Toxic,
                    "Cinder Rime Ascendance of Lightning–Blood–Toxic",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Lightning, Blood, Toxic. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Fire is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x37.",
                    ReactionElement.Toxic,
                    2.300f,
                    4.400f,
                    5.890f,
                    7,
                    3,
                    0.945f));
            definitions.Add(
                0x57,
                new ElementalReactionDefinition(
                    "reaction_57",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Void,
                    "Cinder Rime Convergence of Lightning–Blood–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Lightning, Blood, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x57.",
                    ReactionElement.Cold,
                    2.275f,
                    4.890f,
                    6.250f,
                    7,
                    2,
                    0.945f));
            definitions.Add(
                0x67,
                new ElementalReactionDefinition(
                    "reaction_67",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder Rime Aetherstorm of Lightning–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Lightning, Toxic, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x67.",
                    ReactionElement.Fire,
                    2.425f,
                    4.820f,
                    6.430f,
                    5,
                    3,
                    0.985f));
            definitions.Add(
                0x3B,
                new ElementalReactionDefinition(
                    "reaction_3B",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic,
                    "Cinder Rime Convergence of Physical–Blood–Toxic",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Physical, Blood, Toxic. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x3B.",
                    ReactionElement.Physical,
                    2.500f,
                    4.680f,
                    6.700f,
                    7,
                    2,
                    0.945f));
            definitions.Add(
                0x5B,
                new ElementalReactionDefinition(
                    "reaction_5B",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Void,
                    "Cinder Rime Grand Crucible of Physical–Blood–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Physical, Blood, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x5B.",
                    ReactionElement.Void,
                    2.475f,
                    4.540f,
                    5.980f,
                    7,
                    2,
                    1.025f));
            definitions.Add(
                0x6B,
                new ElementalReactionDefinition(
                    "reaction_6B",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder Rime Devouring Crown of Physical–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Physical, Toxic, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x6B.",
                    ReactionElement.Toxic,
                    2.300f,
                    4.470f,
                    6.160f,
                    5,
                    2,
                    0.985f));
            definitions.Add(
                0x73,
                new ElementalReactionDefinition(
                    "reaction_73",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder Rime Convergence of Blood–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Cold, Blood, Toxic, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Fire is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x73.",
                    ReactionElement.Toxic,
                    2.425f,
                    4.540f,
                    6.070f,
                    5,
                    3,
                    0.985f));
            definitions.Add(
                0x3D,
                new ElementalReactionDefinition(
                    "reaction_3D",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic,
                    "Cinder Volt Aetherstorm of Physical–Blood–Toxic",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Lightning, Physical, Blood, Toxic. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x3D.",
                    ReactionElement.Physical,
                    2.275f,
                    4.540f,
                    6.610f,
                    7,
                    2,
                    0.945f));
            definitions.Add(
                0x5D,
                new ElementalReactionDefinition(
                    "reaction_5D",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Void,
                    "Cinder Volt Devouring Crown of Physical–Blood–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Lightning, Physical, Blood, Void. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x5D.",
                    ReactionElement.Void,
                    2.250f,
                    4.400f,
                    5.890f,
                    6,
                    2,
                    1.025f));
            definitions.Add(
                0x6D,
                new ElementalReactionDefinition(
                    "reaction_6D",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder Volt Worldstorm of Physical–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Lightning, Physical, Toxic, Void. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x6D.",
                    ReactionElement.Fire,
                    2.450f,
                    4.400f,
                    6.340f,
                    6,
                    3,
                    0.985f));
            definitions.Add(
                0x75,
                new ElementalReactionDefinition(
                    "reaction_75",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder Volt Aetherstorm of Blood–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Lightning, Blood, Toxic, Void. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x75.",
                    ReactionElement.Blood,
                    2.525f,
                    4.960f,
                    5.980f,
                    5,
                    2,
                    0.985f));
            definitions.Add(
                0x79,
                new ElementalReactionDefinition(
                    "reaction_79",
                    ReactionElement.Fire | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Cinder Ruin Devouring Crown of Blood–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Fire, Physical, Blood, Toxic, Void. It combines burning, spreading flames and area denial; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x79.",
                    ReactionElement.Fire,
                    2.400f,
                    4.610f,
                    6.700f,
                    5,
                    3,
                    0.945f));
            definitions.Add(
                0x3E,
                new ElementalReactionDefinition(
                    "reaction_3E",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic,
                    "Rime Volt Ascendance of Physical–Blood–Toxic",
                    ReactionTier.Convergence,
                    "Convergence of Cold, Lightning, Physical, Blood, Toxic. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x3E.",
                    ReactionElement.Cold,
                    2.300f,
                    4.680f,
                    5.890f,
                    7,
                    3,
                    1.025f));
            definitions.Add(
                0x5E,
                new ElementalReactionDefinition(
                    "reaction_5E",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Void,
                    "Rime Volt Convergence of Physical–Blood–Void",
                    ReactionTier.Convergence,
                    "Convergence of Cold, Lightning, Physical, Blood, Void. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x5E.",
                    ReactionElement.Cold,
                    2.325f,
                    4.610f,
                    6.520f,
                    5,
                    2,
                    0.945f));
            definitions.Add(
                0x6E,
                new ElementalReactionDefinition(
                    "reaction_6E",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Toxic | ReactionElement.Void,
                    "Rime Volt Aetherstorm of Physical–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Cold, Lightning, Physical, Toxic, Void. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x6E.",
                    ReactionElement.Toxic,
                    2.475f,
                    4.540f,
                    6.700f,
                    7,
                    3,
                    0.985f));
            definitions.Add(
                0x76,
                new ElementalReactionDefinition(
                    "reaction_76",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Rime Volt Ascendance of Blood–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Cold, Lightning, Blood, Toxic, Void. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Toxic is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x76.",
                    ReactionElement.Cold,
                    2.550f,
                    4.540f,
                    6.250f,
                    6,
                    2,
                    0.945f));
            definitions.Add(
                0x7A,
                new ElementalReactionDefinition(
                    "reaction_7A",
                    ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Rime Ruin Convergence of Blood–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Cold, Physical, Blood, Toxic, Void. It combines chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x7A.",
                    ReactionElement.Void,
                    2.425f,
                    4.820f,
                    6.070f,
                    6,
                    3,
                    0.945f));
            definitions.Add(
                0x7C,
                new ElementalReactionDefinition(
                    "reaction_7C",
                    ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Volt Ruin Aetherstorm of Blood–Toxic–Void",
                    ReactionTier.Convergence,
                    "Convergence of Lightning, Physical, Blood, Toxic, Void. It combines chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Void is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x7C.",
                    ReactionElement.Toxic,
                    2.525f,
                    4.680f,
                    5.980f,
                    5,
                    2,
                    1.025f));
            definitions.Add(
                0x3F,
                new ElementalReactionDefinition(
                    "reaction_3F",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic,
                    "Prismatic Detonation",
                    ReactionTier.Calamity,
                    "Calamity of Fire, Cold, Lightning, Physical, Blood, Toxic. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x3F.",
                    ReactionElement.Toxic,
                    2.715f,
                    5.440f,
                    7.240f,
                    7,
                    4,
                    0.950f));
            definitions.Add(
                0x5F,
                new ElementalReactionDefinition(
                    "reaction_5F",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Void,
                    "Astral Tempest",
                    ReactionTier.Calamity,
                    "Calamity of Fire, Cold, Lightning, Physical, Blood, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x5F.",
                    ReactionElement.Blood,
                    2.690f,
                    5.300f,
                    6.520f,
                    7,
                    4,
                    0.950f));
            definitions.Add(
                0x6F,
                new ElementalReactionDefinition(
                    "reaction_6F",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Toxic | ReactionElement.Void,
                    "Elemental Cataclysm",
                    ReactionTier.Calamity,
                    "Calamity of Fire, Cold, Lightning, Physical, Toxic, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Physical is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x6F.",
                    ReactionElement.Cold,
                    2.890f,
                    5.300f,
                    6.970f,
                    7,
                    3,
                    0.910f));
            definitions.Add(
                0x77,
                new ElementalReactionDefinition(
                    "reaction_77",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Living Apocalypse",
                    ReactionTier.Calamity,
                    "Calamity of Fire, Cold, Lightning, Blood, Toxic, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x77.",
                    ReactionElement.Toxic,
                    2.640f,
                    5.300f,
                    6.520f,
                    6,
                    3,
                    0.870f));
            definitions.Add(
                0x7B,
                new ElementalReactionDefinition(
                    "reaction_7B",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Silent Cataclysm",
                    ReactionTier.Calamity,
                    "Calamity of Fire, Cold, Physical, Blood, Toxic, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x7B.",
                    ReactionElement.Blood,
                    2.840f,
                    4.950f,
                    7.330f,
                    8,
                    4,
                    0.870f));
            definitions.Add(
                0x7D,
                new ElementalReactionDefinition(
                    "reaction_7D",
                    ReactionElement.Fire | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Worldfire Collapse",
                    ReactionTier.Calamity,
                    "Calamity of Fire, Lightning, Physical, Blood, Toxic, Void. It combines burning, spreading flames and area denial; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Lightning is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x7D.",
                    ReactionElement.Void,
                    2.615f,
                    5.440f,
                    7.240f,
                    8,
                    3,
                    0.870f));
            definitions.Add(
                0x7E,
                new ElementalReactionDefinition(
                    "reaction_7E",
                    ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Absolute Extinction",
                    ReactionTier.Calamity,
                    "Calamity of Cold, Lightning, Physical, Blood, Toxic, Void. It combines chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Cold is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x7E.",
                    ReactionElement.Toxic,
                    2.640f,
                    4.950f,
                    6.520f,
                    6,
                    3,
                    0.950f));
            definitions.Add(
                0x7F,
                new ElementalReactionDefinition(
                    "reaction_7F",
                    ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical | ReactionElement.Blood | ReactionElement.Toxic | ReactionElement.Void,
                    "Worldbreak",
                    ReactionTier.Apex,
                    "Apex of Fire, Cold, Lightning, Physical, Blood, Toxic, Void. It combines burning, spreading flames and area denial; chill, freeze, brittleness and shattering; chains, rapid pulses and conduction; impact, armor break, stagger and displacement; wounds, execution pressure and organic spread; poison, contamination and persistent clouds; pulling, spatial distortion and collapse. Blood is the default catalyst expression, changing pulse timing, propagation and residue behavior for signature 0x7F.",
                    ReactionElement.Fire,
                    3.055f,
                    5.710f,
                    7.870f,
                    9,
                    4,
                    0.795f));
        }
    }
}
