using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace ArcaneEngine
{
    public static class ProceduralLightingDirector
    {
        public static BiomeLightingProfile CurrentProfile { get; private set; }

        public static void Apply(Transform environment, Color fallbackAccent)
        {
            if (environment == null) return;
            ProceduralRoomVisual roomVisual = environment.GetComponentInChildren<ProceduralRoomVisual>();
            BiomeVisualDefinition biome = roomVisual == null ? null : BiomeVisualCatalog.Get(roomVisual.biomeId);
            BiomeLightingProfile profile = BiomeLightingCatalog.Get(biome, roomVisual != null && roomVisual.wardenSanctumOverlay);
            CurrentProfile = profile;
            if (biome == null)
            {
                profile.keyColor = Color.Lerp(profile.keyColor, fallbackAccent, 0.3f);
                profile.rimColor = Color.Lerp(profile.rimColor, fallbackAccent, 0.45f);
            }
            Color ambient = profile.ambient;
            AccessibilitySettings settings = ProfileManager.Current == null ? new AccessibilitySettings() : ProfileManager.Current.accessibility;

            GameObject keyObject = new GameObject("Biome Key Light");
            keyObject.transform.SetParent(environment, false);
            keyObject.transform.rotation = Quaternion.Euler(52f, -28f, 0f);
            Light key = keyObject.AddComponent<Light>();
            key.type = LightType.Directional;
            key.intensity = profile.keyIntensity;
            key.color = profile.keyColor;
            int effectiveShadow = Mathf.Min(settings.shadowQuality, profile.shadowPolicy);
            key.shadows = effectiveShadow <= 0 ? LightShadows.None : effectiveShadow == 1 ? LightShadows.Hard : LightShadows.Soft;
            key.shadowStrength = settings.shadowQuality <= 0 ? 0f : 0.46f;
            key.shadowResolution = settings.shadowQuality >= 2 ? UnityEngine.Rendering.LightShadowResolution.High : UnityEngine.Rendering.LightShadowResolution.Medium;

            GameObject rimObject = new GameObject("Biome Rim Light");
            rimObject.transform.SetParent(environment, false);
            rimObject.transform.rotation = Quaternion.Euler(38f, 142f, 0f);
            Light rim = rimObject.AddComponent<Light>();
            rim.type = LightType.Directional;
            rim.intensity = profile.rimIntensity;
            rim.color = profile.rimColor;
            rim.shadows = LightShadows.None;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = ambient;
            RenderSettings.fog = biome != null;
            if (biome != null)
            {
                RenderSettings.fogMode = FogMode.ExponentialSquared;
                RenderSettings.fogColor = profile.fogColor;
                RenderSettings.fogDensity = profile.fogDensity;
            }

            Camera camera = Camera.main;
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = biome == null ? new Color(0.025f, 0.03f, 0.055f) : Color.Lerp(profile.fogColor, Color.black, 0.56f);
                camera.farClipPlane = 160f;
            }
            if (GameWorld.Instance != null && GameWorld.Instance.Player != null)
                PriorityLightAnchor.Attach(GameWorld.Instance.Player.gameObject, profile.playerSeparation, 4.2f, 0.62f, 5);
        }
    }

    public enum RewardVisualFamily { Equipment, SupportRune, SpellCore, Currency, Healing, Upgrade, Link, Risk }

    [Serializable]
    public sealed class RewardVisualDescriptor
    {
        public RewardVisualFamily family;
        public ItemRarity rarity;
        public Color color;
        public PrimitiveType primaryShape;
        public PrimitiveType symbolShape;
        public int ringCount;
        public float beamHeight;
        public bool risky;
        public bool corrupted;
        public bool unique;
        public UniqueMutation uniqueMutation;
        public EquipmentSlot equipmentSlot;
        public HexCoord[] runeFootprint;
        public ModifierCategory runeCategory;
        public SpellElement coreElement;
        public SpellDelivery coreDelivery;
        public string resourceKind;
        public string dataSummary;
    }

    public static class RewardVisualSystem
    {
        public static RewardVisualDescriptor FromPickup(LootPickup pickup, Color color)
        {
            RewardVisualDescriptor descriptor = new RewardVisualDescriptor();
            descriptor.rarity = pickup == null ? ItemRarity.Common : pickup.DisplayedRarity;
            descriptor.color = color;
            descriptor.ringCount = pickup != null && pickup.kind == LootKind.Item ? 1 + (int)descriptor.rarity : 1;
            descriptor.beamHeight = pickup != null && pickup.kind == LootKind.Item ? 1.8f + (int)descriptor.rarity * 0.7f : 1.5f;
            if (pickup == null) { descriptor.family = RewardVisualFamily.Currency; descriptor.primaryShape = PrimitiveType.Sphere; descriptor.symbolShape = PrimitiveType.Cylinder; return descriptor; }
            switch (pickup.kind)
            {
                case LootKind.Item: descriptor.family = RewardVisualFamily.Equipment; descriptor.primaryShape = PrimitiveType.Capsule; descriptor.symbolShape = PrimitiveType.Cube; break;
                case LootKind.Modifier: descriptor.family = RewardVisualFamily.SupportRune; descriptor.primaryShape = PrimitiveType.Cube; descriptor.symbolShape = PrimitiveType.Cylinder; break;
                case LootKind.SpellCore: descriptor.family = RewardVisualFamily.SpellCore; descriptor.primaryShape = PrimitiveType.Cylinder; descriptor.symbolShape = PrimitiveType.Sphere; break;
                default: descriptor.family = RewardVisualFamily.Currency; descriptor.primaryShape = PrimitiveType.Sphere; descriptor.symbolShape = pickup.kind == LootKind.Essence ? PrimitiveType.Cube : PrimitiveType.Cylinder; break;
            }
            if (pickup.kind == LootKind.Item && pickup.ItemDefinition != null)
            {
                descriptor.equipmentSlot = pickup.ItemDefinition.slot;
                descriptor.corrupted = pickup.PreviewItem != null && pickup.PreviewItem.corrupted;
                descriptor.unique = descriptor.rarity == ItemRarity.Unique || pickup.ItemDefinition.mutation != UniqueMutation.None;
                descriptor.uniqueMutation = pickup.ItemDefinition.mutation;
            }
            else if (pickup.kind == LootKind.Modifier && pickup.ModifierDefinition != null)
            {
                descriptor.runeFootprint = pickup.ModifierDefinition.shape;
                descriptor.runeCategory = pickup.ModifierDefinition.category;
            }
            else if (pickup.kind == LootKind.SpellCore && pickup.CoreDefinition != null)
            {
                descriptor.coreElement = pickup.CoreDefinition.element;
                descriptor.coreDelivery = pickup.CoreDefinition.delivery;
            }
            descriptor.resourceKind = pickup.kind.ToString();
            descriptor.dataSummary = pickup.kind + " · " + pickup.contentId + " · " + descriptor.rarity;
            return descriptor;
        }

        public static RewardVisualDescriptor FromOffer(RewardOffer offer)
        {
            RewardVisualDescriptor descriptor = new RewardVisualDescriptor();
            descriptor.color = offer == null ? Color.white : offer.color;
            descriptor.risky = offer != null && offer.risky;
            descriptor.rarity = offer != null && offer.generatedItem != null && Enum.IsDefined(typeof(ItemRarity), offer.generatedItem.rarity)
                ? (ItemRarity)offer.generatedItem.rarity : ItemRarity.Common;
            RewardCategory category = offer == null ? RewardCategory.Drachmas : offer.category;
            if (category == RewardCategory.Equipment || category == RewardCategory.EquipmentUpgrade) descriptor.family = RewardVisualFamily.Equipment;
            else if (category == RewardCategory.Modifier || category == RewardCategory.ModifierTransformation || category == RewardCategory.BoardExpansion) descriptor.family = RewardVisualFamily.SupportRune;
            else if (category == RewardCategory.SpellCore || category == RewardCategory.SpellUpgrade) descriptor.family = RewardVisualFamily.SpellCore;
            else if (category == RewardCategory.Healing) descriptor.family = RewardVisualFamily.Healing;
            else if (category == RewardCategory.SpellLink || category == RewardCategory.LinkUpgrade) descriptor.family = RewardVisualFamily.Link;
            else if (offer != null && offer.risky || category == RewardCategory.CursedPower || category == RewardCategory.RelicShardChallenge) descriptor.family = RewardVisualFamily.Risk;
            else if (category == RewardCategory.Blessing || category == RewardCategory.MapReveal || category == RewardCategory.ShopDiscount) descriptor.family = RewardVisualFamily.Upgrade;
            else descriptor.family = RewardVisualFamily.Currency;
            descriptor.primaryShape = ShapeFor(descriptor.family);
            descriptor.symbolShape = SymbolFor(descriptor.family);
            descriptor.ringCount = descriptor.family == RewardVisualFamily.Equipment ? 1 + (int)descriptor.rarity : descriptor.risky ? 3 : 2;
            descriptor.beamHeight = 1.7f + (int)descriptor.rarity * 0.55f;
            descriptor.dataSummary = category + " · " + (offer == null ? string.Empty : offer.contentId) + " · " + descriptor.rarity;
            descriptor.resourceKind = category.ToString();
            if (offer != null && offer.generatedItem != null)
            {
                ItemInstance item = new ItemInstance(offer.generatedItem);
                ItemDefinition definition = item.Definition;
                if (definition != null) descriptor.equipmentSlot = definition.slot;
                descriptor.corrupted = item.corrupted;
                descriptor.unique = item.rarity == ItemRarity.Unique || definition != null && definition.mutation != UniqueMutation.None;
                descriptor.uniqueMutation = definition == null ? UniqueMutation.None : definition.mutation;
            }
            if (descriptor.family == RewardVisualFamily.SupportRune)
            {
                SpellModifierDefinition modifier = offer == null ? null : DemoCatalog.GetModifier(offer.contentId);
                if (modifier != null) { descriptor.runeFootprint = modifier.shape; descriptor.runeCategory = modifier.category; }
            }
            if (descriptor.family == RewardVisualFamily.SpellCore)
            {
                SpellCoreDefinition core = offer == null ? null : DemoCatalog.GetCore(offer.contentId);
                if (core != null) { descriptor.coreElement = core.element; descriptor.coreDelivery = core.delivery; }
            }
            return descriptor;
        }

        public static void AttachPickup(LootPickup pickup, Color color)
        {
            if (pickup == null) return;
            Build(pickup.transform, FromPickup(pickup, color));
        }

        public static void AttachOffer(Transform root, RewardOffer offer)
        {
            if (root == null || offer == null) return;
            Build(root, FromOffer(offer));
        }

        public static void BuildPreview(Transform root, RewardVisualDescriptor descriptor)
        {
            if (root == null || descriptor == null) return;
            Build(root, descriptor);
        }

        public static void ShowForgeGrant(Vector3 position, int dust, int runes, int cores)
        {
            string[] names = { "Forge Dust", "Binding Runes", "Corruption Cores" };
            int[] amounts = { dust, runes, cores };
            Color[] colors = { new Color(0.72f, 0.76f, 0.82f), new Color(0.32f, 0.88f, 1f), new Color(0.92f, 0.08f, 0.32f) };
            for (int i = 0; i < amounts.Length; i++)
            {
                if (amounts[i] <= 0) continue;
                GameObject root = new GameObject(names[i] + " Grant +" + amounts[i]);
                root.transform.position = position + new Vector3((i - 1) * 0.9f, 0f, -1.2f);
                RewardVisualDescriptor descriptor = new RewardVisualDescriptor { family = RewardVisualFamily.Currency, rarity = ItemRarity.Common,
                    color = colors[i], primaryShape = i == 0 ? PrimitiveType.Sphere : i == 1 ? PrimitiveType.Cube : PrimitiveType.Capsule,
                    symbolShape = PrimitiveType.Cube, ringCount = 1 + i, beamHeight = 1.3f + i * 0.25f, resourceKind = names[i], dataSummary = names[i] + " +" + amounts[i] };
                Build(root.transform, descriptor);
                Object.Destroy(root, 1.25f);
            }
        }

        public static void ShowRelicShard(Vector3 position)
        {
            GameObject root = new GameObject("Legendary Shard Secured"); root.transform.position = position;
            RewardVisualDescriptor descriptor = new RewardVisualDescriptor { family = RewardVisualFamily.Currency, rarity = ItemRarity.Unique,
                color = new Color(1f, 0.32f, 0.08f), primaryShape = PrimitiveType.Capsule, symbolShape = PrimitiveType.Cube,
                ringCount = 4, beamHeight = 3f, resourceKind = "Legendary Shard", dataSummary = "Legendary Shard · secured" };
            Build(root.transform, descriptor);
            ProceduralVisualRuntime.LimitedLight(position + Vector3.up, descriptor.color, 6f, 1.6f, 0.5f, null, 5);
            Object.Destroy(root, 1.8f);
        }

        public static void ShowRunResultItem(ItemSaveData saved, Vector3 position, bool secured)
        {
            if (saved == null) return;
            ItemInstance item = new ItemInstance(saved);
            ItemDefinition definition = item.Definition;
            Color color = definition == null ? Color.white : definition.color;
            if (!secured) color = Color.Lerp(color, new Color(0.55f, 0.05f, 0.08f), 0.62f);
            GameObject root = new GameObject((secured ? "Secured" : "Lost") + " Run Item · " + item.DisplayName);
            root.transform.position = position;
            RewardVisualDescriptor descriptor = new RewardVisualDescriptor
            {
                family = RewardVisualFamily.Equipment,
                rarity = item.rarity,
                color = color,
                primaryShape = PrimitiveType.Capsule,
                symbolShape = PrimitiveType.Cube,
                ringCount = secured ? 1 + (int)item.rarity : 1,
                beamHeight = secured ? 1.8f + (int)item.rarity * 0.45f : 1.2f,
                equipmentSlot = definition == null ? EquipmentSlot.Helmet : definition.slot,
                corrupted = item.corrupted,
                unique = item.rarity == ItemRarity.Unique || definition != null && definition.mutation != UniqueMutation.None,
                uniqueMutation = definition == null ? UniqueMutation.None : definition.mutation,
                resourceKind = secured ? "SECURED" : "LOST",
                dataSummary = (secured ? "SECURED · " : "LOST · ") + item.DisplayName
            };
            Build(root.transform, descriptor);
            Object.Destroy(root, 4.8f);
        }

        public static void ShowRunResultResource(string kind, int amount, Vector3 position, bool secured, Color color)
        {
            if (amount <= 0) return;
            if (!secured) color = Color.Lerp(color, new Color(0.55f, 0.04f, 0.08f), 0.62f);
            GameObject root = new GameObject((secured ? "Secured" : "Lost") + " Resource · " + kind + " ×" + amount); root.transform.position = position;
            RewardVisualDescriptor descriptor = new RewardVisualDescriptor { family = RewardVisualFamily.Currency, rarity = ItemRarity.Common, color = color,
                primaryShape = PrimitiveType.Sphere, symbolShape = PrimitiveType.Cylinder, ringCount = secured ? 2 : 1, beamHeight = secured ? 1.7f : 1.1f,
                resourceKind = kind, dataSummary = (secured ? "SECURED · " : "LOST · ") + amount + " " + kind };
            Build(root.transform, descriptor);
            Object.Destroy(root, 4.8f);
        }

        private static void Build(Transform root, RewardVisualDescriptor descriptor)
        {
            GameObject presentation = new GameObject("Reward Presentation · " + descriptor.dataSummary);
            presentation.transform.SetParent(root, false);
            VisualCounterRegistration.Attach(presentation, VisualRuntimeKind.RewardPresentation);
            GameObject pedestal = RuntimeVisuals.Primitive("Reward Pedestal", PrimitiveType.Cylinder, root.position,
                new Vector3(0.82f, 0.14f, 0.82f), Color.Lerp(descriptor.color, Color.black, 0.48f), presentation.transform);
            RuntimeVisuals.RemoveCollider(pedestal); pedestal.transform.localPosition = Vector3.up * 0.05f;
            BuildFamilySymbol(presentation.transform, descriptor);
            int rings = Mathf.Clamp(descriptor.ringCount, 1, 4);
            for (int i = 0; i < rings; i++)
            {
                LineRenderer ring = RuntimeVisuals.Ring("Actual " + descriptor.family + " Ring " + i, root.position, descriptor.color, 0.62f + i * 0.16f, 0.045f, presentation.transform);
                if (ring != null) { ring.transform.localPosition = Vector3.up * (0.03f + i * 0.04f); ring.transform.localRotation = Quaternion.Euler(i * 9f, 0f, i * 13f); }
            }
            if (descriptor.risky)
            {
                GameObject warning = RuntimeVisuals.Primitive("Risk Warning", PrimitiveType.Capsule, root.position, new Vector3(0.1f, 0.5f, 0.1f), new Color(1f, 0.08f, 0.18f), presentation.transform);
                RuntimeVisuals.RemoveCollider(warning); warning.transform.localPosition = new Vector3(0f, 1.58f, 0f);
            }
            string symbolText = descriptor.family == RewardVisualFamily.SpellCore ? VisualAccessibility.ElementSymbol(descriptor.coreElement) :
                descriptor.family == RewardVisualFamily.Equipment ? VisualAccessibility.RaritySymbol(descriptor.rarity) : string.Empty;
            TextMesh accessibility = VisualAccessibility.AddWorldSymbol(presentation.transform, symbolText, Color.white, 0.17f);
            if (accessibility != null) accessibility.transform.localPosition = Vector3.up * 1.78f;
        }

        private static void BuildFamilySymbol(Transform root, RewardVisualDescriptor descriptor)
        {
            if (descriptor.family == RewardVisualFamily.Equipment) BuildEquipment(root, descriptor);
            else if (descriptor.family == RewardVisualFamily.SupportRune) BuildRune(root, descriptor);
            else if (descriptor.family == RewardVisualFamily.SpellCore) BuildCore(root, descriptor);
            else if (descriptor.family == RewardVisualFamily.Currency) BuildResource(root, descriptor);
            else BuildGeneric(root, descriptor);
        }

        private static GameObject Part(Transform root, string name, PrimitiveType shape, Vector3 position, Vector3 scale, Color color)
        {
            GameObject part = RuntimeVisuals.Primitive(name, shape, root.position, scale, color, root);
            RuntimeVisuals.RemoveCollider(part); part.transform.localPosition = position; return part;
        }

        private static void BuildEquipment(Transform root, RewardVisualDescriptor descriptor)
        {
            PrimitiveType shape = descriptor.equipmentSlot == EquipmentSlot.Helmet ? PrimitiveType.Sphere :
                descriptor.equipmentSlot == EquipmentSlot.Chest ? PrimitiveType.Cube :
                descriptor.equipmentSlot == EquipmentSlot.Weapon ? PrimitiveType.Capsule :
                descriptor.equipmentSlot == EquipmentSlot.Offhand ? PrimitiveType.Cylinder :
                descriptor.equipmentSlot == EquipmentSlot.LeftShoulder || descriptor.equipmentSlot == EquipmentSlot.RightShoulder ? PrimitiveType.Capsule :
                descriptor.equipmentSlot == EquipmentSlot.LeftGlove || descriptor.equipmentSlot == EquipmentSlot.RightGlove ? PrimitiveType.Sphere :
                descriptor.equipmentSlot == EquipmentSlot.Pants ? PrimitiveType.Capsule : PrimitiveType.Cube;
            Vector3 scale = descriptor.equipmentSlot == EquipmentSlot.Weapon ? new Vector3(0.16f, 0.82f, 0.16f) :
                descriptor.equipmentSlot == EquipmentSlot.Chest ? new Vector3(0.62f, 0.72f, 0.28f) :
                descriptor.equipmentSlot == EquipmentSlot.Offhand ? new Vector3(0.58f, 0.08f, 0.58f) :
                descriptor.equipmentSlot == EquipmentSlot.LeftShoulder || descriptor.equipmentSlot == EquipmentSlot.RightShoulder ? new Vector3(0.28f, 0.5f, 0.28f) :
                descriptor.equipmentSlot == EquipmentSlot.LeftGlove || descriptor.equipmentSlot == EquipmentSlot.RightGlove ? new Vector3(0.36f, 0.3f, 0.42f) :
                descriptor.equipmentSlot == EquipmentSlot.Pants ? new Vector3(0.48f, 0.7f, 0.48f) :
                descriptor.equipmentSlot == EquipmentSlot.Boots ? new Vector3(0.44f, 0.28f, 0.72f) : new Vector3(0.46f, 0.52f, 0.38f);
            GameObject item = Part(root, descriptor.equipmentSlot + " World Item", shape, Vector3.up * 1.02f, scale, descriptor.color);
            item.AddComponent<RewardSymbolMotion>().Initialize(1.25f);
            if (descriptor.equipmentSlot == EquipmentSlot.LeftShoulder || descriptor.equipmentSlot == EquipmentSlot.RightShoulder ||
                descriptor.equipmentSlot == EquipmentSlot.LeftGlove || descriptor.equipmentSlot == EquipmentSlot.RightGlove || descriptor.equipmentSlot == EquipmentSlot.Boots)
                Part(root, "Paired Slot Echo", shape, new Vector3(0.42f, 0.92f, 0f), scale * 0.72f, Color.Lerp(descriptor.color, Color.white, 0.16f));
            if (descriptor.corrupted)
                for (int i = 0; i < 4; i++) Part(root, "Corruption Thorn", PrimitiveType.Capsule,
                    new Vector3(Mathf.Cos(i * Mathf.PI * 0.5f) * 0.62f, 1.05f, Mathf.Sin(i * Mathf.PI * 0.5f) * 0.62f), new Vector3(0.08f, 0.42f, 0.08f), new Color(0.8f, 0.04f, 0.24f));
            if (descriptor.unique)
                for (int i = 0; i < 3; i++) Part(root, "Unique Crown", PrimitiveType.Cube,
                    new Vector3((i - 1) * 0.28f, 1.72f + (i == 1 ? 0.16f : 0f), 0f), Vector3.one * 0.16f, new Color(1f, 0.58f, 0.08f));
            UniqueMutationVisualProfile mutation = UniqueMutationVisualRegistry.Get(descriptor.uniqueMutation);
            if (mutation != null)
            {
                int marks = Mathf.Min(4, mutation.marks);
                for (int i = 0; i < marks; i++)
                {
                    float angle = i / (float)Mathf.Max(1, marks) * Mathf.PI * 2f;
                    Part(root, "Unique Functional Signature · " + mutation.functionalRule, i == 0 ? mutation.authorityShape : mutation.accentShape,
                        new Vector3(Mathf.Cos(angle) * 0.72f, 1.12f + (i & 1) * 0.22f, Mathf.Sin(angle) * 0.72f),
                        Vector3.one * (i == 0 ? 0.17f : 0.1f), Color.Lerp(descriptor.color, new Color(1f, 0.58f, 0.08f), 0.48f));
                }
            }
        }

        private static void BuildRune(Transform root, RewardVisualDescriptor descriptor)
        {
            HexCoord[] footprint = descriptor.runeFootprint == null || descriptor.runeFootprint.Length == 0 ? new[] { new HexCoord(0, 0) } : descriptor.runeFootprint;
            float scale = footprint.Length > 4 ? 0.2f : 0.26f;
            for (int i = 0; i < footprint.Length; i++)
            {
                float x = (footprint[i].q + footprint[i].r * 0.5f) * scale * 1.9f;
                float z = footprint[i].r * scale * 1.62f;
                Part(root, "Rune Footprint Cell " + footprint[i], PrimitiveType.Cylinder, new Vector3(x, 1.02f, z), new Vector3(scale, 0.08f, scale), descriptor.color);
            }
            PrimitiveType category = descriptor.runeCategory == ModifierCategory.Pattern || descriptor.runeCategory == ModifierCategory.Spawn ? PrimitiveType.Sphere :
                descriptor.runeCategory == ModifierCategory.Movement || descriptor.runeCategory == ModifierCategory.Targeting ? PrimitiveType.Capsule : PrimitiveType.Cube;
            Part(root, "Rune Category · " + descriptor.runeCategory, category, Vector3.up * 1.42f, Vector3.one * 0.18f, Color.Lerp(descriptor.color, Color.white, 0.38f));
        }

        private static void BuildCore(Transform root, RewardVisualDescriptor descriptor)
        {
            PrimitiveType elementShape = descriptor.coreElement == SpellElement.Frost ? PrimitiveType.Cube : descriptor.coreElement == SpellElement.Lightning ? PrimitiveType.Capsule :
                descriptor.coreElement == SpellElement.Void ? PrimitiveType.Cylinder : PrimitiveType.Sphere;
            GameObject core = Part(root, "Spell Core · " + descriptor.coreElement, elementShape, Vector3.up * 1.02f, Vector3.one * 0.46f, descriptor.color);
            core.AddComponent<RewardSymbolMotion>().Initialize(1.4f);
            int markers = descriptor.coreDelivery == SpellDelivery.Projectile ? 1 : descriptor.coreDelivery == SpellDelivery.Beam || descriptor.coreDelivery == SpellDelivery.Hitscan ? 2 :
                descriptor.coreDelivery == SpellDelivery.Summon ? 4 : 3;
            for (int i = 0; i < markers; i++)
            {
                float angle = i / (float)markers * Mathf.PI * 2f;
                Part(root, "Delivery Marker · " + descriptor.coreDelivery, descriptor.coreDelivery == SpellDelivery.Zone || descriptor.coreDelivery == SpellDelivery.Nova ? PrimitiveType.Cylinder : PrimitiveType.Cube,
                    new Vector3(Mathf.Cos(angle) * 0.68f, 1.02f, Mathf.Sin(angle) * 0.68f), Vector3.one * 0.12f, Color.Lerp(descriptor.color, Color.white, 0.45f));
            }
        }

        private static void BuildResource(Transform root, RewardVisualDescriptor descriptor)
        {
            bool gold = descriptor.resourceKind.Contains("Drachma") || descriptor.resourceKind.Contains("Gold");
            bool essence = descriptor.resourceKind.Contains("Essence");
            bool shard = descriptor.resourceKind.Contains("Shard");
            bool dust = descriptor.resourceKind.Contains("Dust");
            bool binding = descriptor.resourceKind.Contains("Binding") || descriptor.resourceKind.Contains("Support Rune");
            bool corruption = descriptor.resourceKind.Contains("Corruption");
            PrimitiveType shape = gold ? PrimitiveType.Cylinder : essence || binding ? PrimitiveType.Cube : shard || corruption ? PrimitiveType.Capsule : PrimitiveType.Sphere;
            for (int i = 0; i < 3; i++)
            {
                GameObject resource = Part(root, descriptor.resourceKind + " Resource " + i, shape, new Vector3((i - 1) * 0.32f, 0.72f + i * 0.18f, 0f),
                    gold ? new Vector3(0.24f, 0.045f, 0.24f) : dust ? Vector3.one * (0.16f + i * 0.05f) :
                    binding ? new Vector3(0.2f, 0.08f, 0.2f) : corruption ? new Vector3(0.1f, 0.34f, 0.1f) : Vector3.one * (0.22f + i * 0.03f), descriptor.color);
                resource.transform.localRotation = Quaternion.Euler(gold ? 90f : binding ? 45f : 30f, i * (binding ? 60f : 42f), gold ? 0f : corruption ? 18f : 35f);
                resource.AddComponent<RewardSymbolMotion>().Initialize(gold ? 0.8f : essence ? 1.25f : shard ? 2.1f : binding ? 1.55f : corruption ? 2.6f : 1.05f);
            }
        }

        private static void BuildGeneric(Transform root, RewardVisualDescriptor descriptor)
        {
            GameObject symbol = Part(root, descriptor.family + " Symbol", descriptor.symbolShape, Vector3.up * 1.08f, Vector3.one * 0.3f, Color.Lerp(Color.white, descriptor.color, 0.55f));
            symbol.AddComponent<RewardSymbolMotion>().Initialize(descriptor.family == RewardVisualFamily.Risk ? 2.8f : 1.25f);
        }

        private static PrimitiveType ShapeFor(RewardVisualFamily family)
        {
            if (family == RewardVisualFamily.Equipment) return PrimitiveType.Capsule;
            if (family == RewardVisualFamily.SupportRune || family == RewardVisualFamily.Upgrade) return PrimitiveType.Cube;
            if (family == RewardVisualFamily.SpellCore || family == RewardVisualFamily.Link) return PrimitiveType.Cylinder;
            return PrimitiveType.Sphere;
        }

        private static PrimitiveType SymbolFor(RewardVisualFamily family)
        {
            if (family == RewardVisualFamily.Equipment || family == RewardVisualFamily.Risk) return PrimitiveType.Cube;
            if (family == RewardVisualFamily.SupportRune || family == RewardVisualFamily.Link) return PrimitiveType.Cylinder;
            if (family == RewardVisualFamily.SpellCore || family == RewardVisualFamily.Healing) return PrimitiveType.Sphere;
            return PrimitiveType.Capsule;
        }
    }

    public sealed class RewardSymbolMotion : MonoBehaviour
    {
        private float _rate;
        private Vector3 _basePosition;
        public void Initialize(float rate) { _rate = rate; _basePosition = transform.localPosition; }
        private void Update()
        {
            bool reducedMotion = ProfileManager.Current != null && ProfileManager.Current.accessibility.reducedMotion;
            float motion = reducedMotion ? 0.015f : 0.09f;
            transform.localPosition = _basePosition + Vector3.up * Mathf.Sin(Time.time * _rate) * motion;
            float rotationScale = reducedMotion ? 0.15f : 1f;
            transform.Rotate(18f * rotationScale * Time.deltaTime, 42f * rotationScale * Time.deltaTime, 11f * rotationScale * Time.deltaTime);
        }
    }

    public sealed class VisualDiagnosticsOverlay : MonoBehaviour
    {
        public static VisualDiagnosticsOverlay Instance { get; private set; }
        private float _smoothedFrameTime = 16.67f;
        private float _sampleTimer;
        private float _slowTimer;
        private float _recoverTimer;
        private int _enemyCount;
        private int _projectileCount;
        private int _roomRenderers;
        private int _activeMaterials;
        private int _liveMaterialInstances;
        private int _activeParticleSystems;
        private int _shadowCastingLights;
        private string _descriptorVersions = "none";
        private GUIStyle _box;
        private GUIStyle _text;

        private void Awake() { Instance = this; }
        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Update()
        {
            float milliseconds = Time.unscaledDeltaTime * 1000f;
            _smoothedFrameTime = Mathf.Lerp(_smoothedFrameTime, milliseconds, 0.08f);
            if (ArcaneInput.GetKeyDown(KeyCode.F10) && ProfileManager.Current != null)
                ProfileManager.Current.accessibility.showVisualDiagnostics = !ProfileManager.Current.accessibility.showVisualDiagnostics;
            bool visible = ProfileManager.Current != null && ProfileManager.Current.accessibility.showVisualDiagnostics;
            if (visible)
            {
                _sampleTimer -= Time.unscaledDeltaTime;
                if (_sampleTimer <= 0f) { _sampleTimer = 0.25f; RefreshCounts(); }
            }
            UpdateAdaptiveBudget();
        }

        private void RefreshCounts()
        {
            GameWorld world = GameWorld.Instance;
            _enemyCount = world == null ? 0 : world.Enemies.Count(value => value != null && !value.IsDead);
            _projectileCount = VisualRuntimeRegistry.Count(VisualRuntimeKind.Projectile);
            _roomRenderers = VisualRuntimeRegistry.RoomRenderers;
            _activeMaterials = RuntimeVisuals.MaterialCount;
            HashSet<Material> liveMaterials = new HashSet<Material>();
            Renderer[] renderers = FindObjectsByType<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] shared = renderers[i] == null ? null : renderers[i].sharedMaterials;
                if (shared == null) continue;
                for (int material = 0; material < shared.Length; material++) if (shared[material] != null) liveMaterials.Add(shared[material]);
            }
            _liveMaterialInstances = liveMaterials.Count;
            ParticleSystem[] particles = FindObjectsByType<ParticleSystem>();
            _activeParticleSystems = particles.Count(value => value != null && value.isPlaying);
            Light[] lights = FindObjectsByType<Light>();
            _shadowCastingLights = lights.Count(value => value != null && value.enabled && value.shadows != LightShadows.None);
            List<string> descriptors = new List<string>();
            if (world != null)
            {
                for (int slot = 0; slot < 3; slot++)
                {
                    CompiledSpell spell = world.GetSpell((SpellSlot)slot);
                    if (spell != null) descriptors.Add(((SpellSlot)slot) + " r" + spell.visualRevision);
                }
            }
            _descriptorVersions = descriptors.Count == 0 ? "none" : string.Join(" · ", descriptors);
        }

        private void UpdateAdaptiveBudget()
        {
            if (_smoothedFrameTime > 31f)
            {
                _slowTimer += Time.unscaledDeltaTime; _recoverTimer = 0f;
                if (_slowTimer > 2.5f) { ProceduralVisualRuntime.AdaptiveScale = Mathf.Max(0.55f, ProceduralVisualRuntime.AdaptiveScale - 0.1f); _slowTimer = 0f; }
            }
            else if (_smoothedFrameTime < 21f)
            {
                _recoverTimer += Time.unscaledDeltaTime; _slowTimer = 0f;
                if (_recoverTimer > 6f) { ProceduralVisualRuntime.AdaptiveScale = Mathf.Min(1f, ProceduralVisualRuntime.AdaptiveScale + 0.05f); _recoverTimer = 0f; }
            }
            else { _slowTimer = 0f; _recoverTimer = 0f; }
        }

        private void OnGUI()
        {
            if (ProfileManager.Current == null || !ProfileManager.Current.accessibility.showVisualDiagnostics) return;
            if (_box == null)
            {
                _box = new GUIStyle(GUI.skin.box); _box.alignment = TextAnchor.UpperLeft; _box.padding = new RectOffset(12, 12, 10, 10);
                _text = new GUIStyle(GUI.skin.label); _text.fontSize = 13; _text.normal.textColor = new Color(0.75f, 0.95f, 1f); _text.richText = true;
            }
            float fps = _smoothedFrameTime <= 0.01f ? 0f : 1000f / _smoothedFrameTime;
            string roomName = VisualRuntimeRegistry.RoomLabel;
            GUI.Box(new Rect(14f, 14f, 470f, 334f), GUIContent.none, _box);
            GUI.Label(new Rect(28f, 24f, 445f, 312f),
                "<b>VISUAL DIAGNOSTICS  [F10]</b>\n" +
                "Frame  " + _smoothedFrameTime.ToString("0.0") + " ms   " + fps.ToString("0") + " FPS\n" +
                "Room   " + roomName + "\n" +
                "Enemies " + _enemyCount + "   Projectiles " + _projectileCount + "\n" +
                "Spell bodies " + SpellVisualAttachment.ActiveCount + "   Pooled visuals " + ProceduralVisualRuntime.ActiveVisuals + " / " + ProceduralVisualRuntime.ActiveVisualLimit + " · " + ProceduralVisualRuntime.PooledVisuals + " cached\n" +
                "Primitives " + ProceduralVisualRuntime.ActivePrimitives + "   Rings " + ProceduralVisualRuntime.ActiveRings + "   Beams/arcs " + ProceduralVisualRuntime.ActiveBeams + "   Trails " + ProceduralVisualRuntime.ActiveTrails + "\n" +
                "Particle systems " + _activeParticleSystems + " active · 0 pooled (geometry pipeline)\n" +
                "Zones " + VisualRuntimeRegistry.Count(VisualRuntimeKind.PersistentZone) + "   Telegraphs " + VisualRuntimeRegistry.Count(VisualRuntimeKind.Telegraph) + "   Status layers " + VisualRuntimeRegistry.Count(VisualRuntimeKind.StatusLayer) + "\n" +
                "Lights " + ProceduralVisualRuntime.ActiveLights + " / " + ProceduralVisualRuntime.ActiveLightLimit + "   Shadow lights " + _shadowCastingLights + "   Decals " + ProceduralVisualRuntime.ActiveDecals + " / " + ProceduralVisualRuntime.ActiveDecalLimit + "\n" +
                "Room renderers " + _roomRenderers + "   Reward presentations " + VisualRuntimeRegistry.Count(VisualRuntimeKind.RewardPresentation) + "\n" +
                "Shared material families " + _activeMaterials + " / " + RuntimeVisuals.MaterialLimit + "   Live material instances " + _liveMaterialInstances + "\n" +
                "Spell descriptor versions " + _descriptorVersions + "\n" +
                "Telegraph audit " + AttackTelegraphAudit.Count + " attacks · " + AttackTelegraphAudit.MismatchCount + " mismatches\n" +
                "Adaptive decorative scale " + ProceduralVisualRuntime.AdaptiveScale.ToString("0.00") + "\n" +
                "Quality " + ProceduralVisualRuntime.Quality + "   Spell density " + ProceduralVisualRuntime.Density.ToString("0.00"), _text);
        }
    }

    public static class ProceduralVisualValidation
    {
        public static int Validate()
        {
            int failures = 0;
            foreach (SpellElement element in Enum.GetValues(typeof(SpellElement)))
                if (!Enum.IsDefined(typeof(SpellShapeFamily), (int)element)) { Debug.LogError("Visual validation: missing element grammar for " + element); failures++; }
            foreach (EnemyArchetype archetype in Enum.GetValues(typeof(EnemyArchetype)))
                if (EnemyVisualCatalog.Get(archetype) == null) { Debug.LogError("Visual validation: missing enemy definition for " + archetype); failures++; }
            int seed = 1977;
            BiomeVisualId first = BiomeVisualCatalog.Resolve(seed, 0, 10, DungeonRoomType.Combat);
            BiomeVisualId firstAgain = BiomeVisualCatalog.Resolve(seed, 0, 10, DungeonRoomType.Combat);
            BiomeVisualId second = BiomeVisualCatalog.Resolve(seed, 5, 10, DungeonRoomType.Combat);
            if (first != firstAgain || first == second) { Debug.LogError("Visual validation: biome route is not deterministic and distinct."); failures++; }
            if (BiomeVisualCatalog.Resolve(seed, 9, 10, DungeonRoomType.Boss) == BiomeVisualId.WardenSanctum) { Debug.LogError("Visual validation: boss Sanctum replaced the route biome instead of overlaying it."); failures++; }
            return failures;
        }
    }
}
