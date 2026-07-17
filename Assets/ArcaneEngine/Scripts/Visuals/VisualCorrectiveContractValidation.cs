using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    /// <summary>
    /// Source/runtime invariants for the 2.0 corrective visual contract. This is
    /// deliberately separate from Play Mode sign-off: it can reject missing catalog
    /// coverage and invalid budgets, but it does not claim that Unity was manually run.
    /// </summary>
    public static class VisualCorrectiveContractValidation
    {
        public static int Validate()
        {
            int failures = 0;
            if (RuntimeVisuals.MaterialLimit > 192 || RuntimeVisuals.MaterialLimit < 64) Fail("Runtime material cache limit is outside the bounded 64–192 policy.", ref failures);

            VisualQualityBudget low = Budget(ArcaneVisualQuality.Low);
            VisualQualityBudget medium = Budget(ArcaneVisualQuality.Medium);
            VisualQualityBudget high = Budget(ArcaneVisualQuality.High);
            if (!(low.maxActiveVisuals < medium.maxActiveVisuals && medium.maxActiveVisuals < high.maxActiveVisuals)) Fail("Quality visual caps are not strictly increasing.", ref failures);
            if (!(low.maxLights < medium.maxLights && medium.maxLights < high.maxLights)) Fail("Quality light caps are not strictly increasing.", ref failures);
            if (!(low.ringSegments < medium.ringSegments && medium.ringSegments < high.ringSegments)) Fail("Quality ring budgets are not strictly increasing.", ref failures);

            HashSet<string> elementSymbols = new HashSet<string>();
            foreach (SpellElement element in Enum.GetValues(typeof(SpellElement)))
            {
                string symbol = VisualAccessibility.ElementSymbol(element);
                if (string.IsNullOrEmpty(symbol) || !elementSymbols.Add(symbol)) Fail("Element accessibility symbol is missing or duplicated for " + element + ".", ref failures);
            }
            HashSet<string> raritySymbols = new HashSet<string>();
            foreach (ItemRarity rarity in Enum.GetValues(typeof(ItemRarity)))
            {
                string symbol = VisualAccessibility.RaritySymbol(rarity);
                if (string.IsNullOrEmpty(symbol) || !raritySymbols.Add(symbol)) Fail("Rarity accessibility symbol is missing or duplicated for " + rarity + ".", ref failures);
            }

            HashSet<string> silhouettes = new HashSet<string>();
            foreach (EnemyArchetype archetype in Enum.GetValues(typeof(EnemyArchetype)))
            {
                EnemyVisualDefinition definition = EnemyVisualCatalog.Get(archetype);
                if (definition == null || string.IsNullOrEmpty(definition.silhouette)) Fail("Enemy silhouette definition missing for " + archetype + ".", ref failures);
                else if (!silhouettes.Add(definition.silhouette)) Fail("Enemy silhouette name is duplicated: " + definition.silhouette + ".", ref failures);
                if (definition != null && (string.IsNullOrEmpty(definition.motionProfile) || string.IsNullOrEmpty(definition.hitResponseProfile) ||
                    string.IsNullOrEmpty(definition.deathProfile) || string.IsNullOrEmpty(definition.allowedBiomeDecoration)))
                    Fail("Enemy visual metadata is incomplete for " + archetype + ".", ref failures);
            }

            foreach (BiomeVisualId biome in Enum.GetValues(typeof(BiomeVisualId)))
            {
                BiomeVisualDefinition definition = BiomeVisualCatalog.Get(biome);
                BiomeLightingProfile lighting = BiomeLightingCatalog.Get(definition, biome == BiomeVisualId.WardenSanctum);
                if (definition == null || string.IsNullOrEmpty(definition.structuralLanguage)) Fail("Biome kit definition missing for " + biome + ".", ref failures);
                if (lighting == null || lighting.keyIntensity <= 0f || lighting.fogDensity < 0f) Fail("Lighting profile missing or invalid for " + biome + ".", ref failures);
                if (lighting != null && (lighting.maximumExposure < lighting.minimumExposure || lighting.shadowPolicy < 0 || lighting.shadowPolicy > 2 ||
                    lighting.objectivePriority.maxColorComponent <= 0f || lighting.activeDoorPriority.maxColorComponent <= 0f || lighting.enemySeparation.maxColorComponent <= 0f))
                    Fail("Lighting priority/exposure contents are invalid for " + biome + ".", ref failures);
            }

            foreach (UniqueMutation mutation in Enum.GetValues(typeof(UniqueMutation)))
            {
                if (mutation == UniqueMutation.None) continue;
                UniqueMutationVisualProfile profile = UniqueMutationVisualRegistry.Get(mutation);
                if (profile == null || string.IsNullOrEmpty(profile.functionalRule) || profile.marks < 1) Fail("Unique mutation visual profile missing for " + mutation + ".", ref failures);
            }

            int legendaryCount = 0;
            foreach (RelicSignature signature in Enum.GetValues(typeof(RelicSignature)))
            {
                if (signature == RelicSignature.None) continue;
                LegendaryVisualProfile profile = LegendaryVisualRegistry.Get(signature);
                if (profile == null || string.IsNullOrEmpty(profile.construction) || profile.layerCount < 1) Fail("Legendary visual profile missing for " + signature + ".", ref failures);
                legendaryCount++;
            }
            if (legendaryCount < 20) Fail("Legendary visual registry contains fewer than 20 explicit signatures.", ref failures);

            if (Enum.GetValues(typeof(SpellDelivery)).Length != 10) Fail("The spell visual contract requires exactly ten delivery families.", ref failures);
            if (Enum.GetValues(typeof(DungeonRoomType)).Length != 17) Fail("The room-purpose contract requires exactly seventeen room types.", ref failures);
            if (Enum.GetValues(typeof(SpellOverloadTier)).Length != 4) Fail("Spell overload presentation requires four bounded tiers.", ref failures);

            RunSnapshotData sample = new RunSnapshotData { runSeed = 1977, roomIndex = 3, totalRooms = 11, roomId = "visual-validation" };
            string first = VisualContinuationValidation.Compute(sample);
            string second = VisualContinuationValidation.Compute(sample);
            if (string.IsNullOrEmpty(first) || first != second) Fail("Visual continuation signature is not deterministic.", ref failures);

            if (failures == 0) Debug.Log("Arcane Engine 2.0 corrective visual-contract source validation passed. Play Mode acceptance remains required.");
            return failures;
        }

        private static VisualQualityBudget Budget(ArcaneVisualQuality quality)
        {
            return VisualQualityBudget.ForQuality(quality);
        }

        private static void Fail(string message, ref int failures)
        {
            Debug.LogError("2.0 visual contract validation: " + message);
            failures++;
        }
    }
}
