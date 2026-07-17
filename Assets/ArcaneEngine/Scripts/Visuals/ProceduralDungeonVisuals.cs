using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum BiomeVisualId { OssuaryCatacombs, EmberFoundry, SunkenArchive, VenomCistern, WardenSanctum, HomeBase }

    [Serializable]
    public sealed class BiomeVisualDefinition
    {
        public BiomeVisualId id;
        public string displayName;
        public string structuralLanguage;
        public Color floor;
        public Color wall;
        public Color trim;
        public Color emission;
        public Color ambient;
        public Color fog;
        public PrimitiveType pillarShape;
        public PrimitiveType propShape;
        public float fogDensity;
        public float ambientMotion;

        public Color FloorVariant(int index)
        {
            switch (Mathf.Abs(index) % 4)
            {
                case 1: return Color.Lerp(floor, trim, 0.08f);
                case 2: return Color.Lerp(floor, wall, 0.12f);
                case 3: return Color.Lerp(floor, Color.black, 0.09f);
                default: return floor;
            }
        }
    }

    public static class BiomeVisualCatalog
    {
        private static readonly Dictionary<BiomeVisualId, BiomeVisualDefinition> Definitions = new Dictionary<BiomeVisualId, BiomeVisualDefinition>();

        public static BiomeVisualDefinition Get(BiomeVisualId id)
        {
            BiomeVisualDefinition definition;
            if (Definitions.TryGetValue(id, out definition)) return definition;
            definition = Build(id);
            Definitions[id] = definition;
            return definition;
        }

        public static BiomeVisualId Resolve(int runSeed, int roomIndex, int totalRooms, DungeonRoomType roomType)
        {
            int first = Positive(runSeed) % 4;
            int offset = 1 + Positive(runSeed / 7 + 31) % 3;
            int second = (first + offset) % 4;
            bool afterMidpoint = roomIndex >= Mathf.Max(1, totalRooms / 2);
            return (BiomeVisualId)(afterMidpoint ? second : first);
        }

        public static string RouteSummary(int runSeed, int totalRooms)
        {
            BiomeVisualId first = Resolve(runSeed, 0, totalRooms, DungeonRoomType.Combat);
            BiomeVisualId second = Resolve(runSeed, Mathf.Max(1, totalRooms / 2), totalRooms, DungeonRoomType.Combat);
            return Get(first).displayName + " → " + Get(second).displayName + " → " + Get(BiomeVisualId.WardenSanctum).displayName;
        }

        private static int Positive(int value) { return value == int.MinValue ? int.MaxValue : Mathf.Abs(value); }

        private static BiomeVisualDefinition Build(BiomeVisualId id)
        {
            switch (id)
            {
                case BiomeVisualId.EmberFoundry:
                    return new BiomeVisualDefinition { id = id, displayName = "Ember Foundry", structuralLanguage = "Furnaces, channels, chains and iron braces",
                        floor = new Color(0.075f, 0.055f, 0.05f), wall = new Color(0.15f, 0.105f, 0.08f), trim = new Color(0.28f, 0.19f, 0.12f), emission = new Color(1f, 0.24f, 0.035f), ambient = new Color(0.24f, 0.11f, 0.055f), fog = new Color(0.11f, 0.055f, 0.035f), pillarShape = PrimitiveType.Cube, propShape = PrimitiveType.Cylinder, fogDensity = 0.012f, ambientMotion = 1.3f };
                case BiomeVisualId.SunkenArchive:
                    return new BiomeVisualDefinition { id = id, displayName = "Sunken Archive", structuralLanguage = "Flooded records, shelves and suspended glyphs",
                        floor = new Color(0.035f, 0.075f, 0.085f), wall = new Color(0.055f, 0.12f, 0.14f), trim = new Color(0.08f, 0.24f, 0.25f), emission = new Color(0.24f, 0.88f, 1f), ambient = new Color(0.05f, 0.16f, 0.2f), fog = new Color(0.025f, 0.095f, 0.12f), pillarShape = PrimitiveType.Cube, propShape = PrimitiveType.Cube, fogDensity = 0.014f, ambientMotion = 0.65f };
                case BiomeVisualId.VenomCistern:
                    return new BiomeVisualDefinition { id = id, displayName = "Venom Cistern", structuralLanguage = "Pipes, drains, reservoirs and organic contamination",
                        floor = new Color(0.04f, 0.065f, 0.045f), wall = new Color(0.07f, 0.11f, 0.065f), trim = new Color(0.12f, 0.21f, 0.12f), emission = new Color(0.32f, 1f, 0.12f), ambient = new Color(0.07f, 0.16f, 0.075f), fog = new Color(0.045f, 0.105f, 0.045f), pillarShape = PrimitiveType.Cylinder, propShape = PrimitiveType.Sphere, fogDensity = 0.017f, ambientMotion = 0.85f };
                case BiomeVisualId.WardenSanctum:
                    return new BiomeVisualDefinition { id = id, displayName = "Warden Sanctum", structuralLanguage = "Bone-and-arcane authority architecture",
                        floor = new Color(0.055f, 0.025f, 0.045f), wall = new Color(0.13f, 0.045f, 0.07f), trim = new Color(0.3f, 0.08f, 0.12f), emission = new Color(1f, 0.08f, 0.24f), ambient = new Color(0.18f, 0.035f, 0.075f), fog = new Color(0.085f, 0.018f, 0.04f), pillarShape = PrimitiveType.Cylinder, propShape = PrimitiveType.Cube, fogDensity = 0.02f, ambientMotion = 0.55f };
                case BiomeVisualId.HomeBase:
                    return new BiomeVisualDefinition { id = id, displayName = "The Relic Forge", structuralLanguage = "Arcane workshop and expedition staging hall",
                        floor = new Color(0.035f, 0.05f, 0.072f), wall = new Color(0.055f, 0.09f, 0.12f), trim = new Color(0.08f, 0.22f, 0.25f), emission = new Color(0.16f, 0.92f, 0.82f), ambient = new Color(0.055f, 0.13f, 0.16f), fog = new Color(0.018f, 0.05f, 0.065f), pillarShape = PrimitiveType.Cylinder, propShape = PrimitiveType.Cube, fogDensity = 0.008f, ambientMotion = 0.45f };
                default:
                    return new BiomeVisualDefinition { id = BiomeVisualId.OssuaryCatacombs, displayName = "Ossuary Catacombs", structuralLanguage = "Bone arches, burial recesses and ribbed stone",
                        floor = new Color(0.045f, 0.052f, 0.068f), wall = new Color(0.09f, 0.095f, 0.115f), trim = new Color(0.19f, 0.18f, 0.17f), emission = new Color(0.58f, 0.28f, 0.88f), ambient = new Color(0.1f, 0.095f, 0.13f), fog = new Color(0.04f, 0.045f, 0.065f), pillarShape = PrimitiveType.Cylinder, propShape = PrimitiveType.Capsule, fogDensity = 0.011f, ambientMotion = 0.38f };
            }
        }
    }

    public sealed class ProceduralRoomVisual : MonoBehaviour
    {
        public BiomeVisualId biomeId;
        public bool wardenSanctumOverlay;
        public DungeonRoomType roomType;
        public int visualSeed;
        public int rendererCount;
        public string BiomeName { get { return BiomeVisualCatalog.Get(biomeId).displayName + (wardenSanctumOverlay ? " · Warden Sanctum" : string.Empty); } }
        private void OnDestroy()
        {
            if (VisualRuntimeRegistry.RoomLabel == BiomeName + " / " + roomType) VisualRuntimeRegistry.ClearRoom();
        }
    }

    public static class ProceduralDungeonVisuals
    {
        private static BiomeVisualId? _lastRunBiome;
        private static readonly Vector3[] PerimeterAnchors =
        {
            new Vector3(-14.6f, 0f, -12f), new Vector3(-14.6f, 0f, -5f), new Vector3(-14.6f, 0f, 5f), new Vector3(-14.6f, 0f, 12f),
            new Vector3(14.6f, 0f, -12f), new Vector3(14.6f, 0f, -5f), new Vector3(14.6f, 0f, 5f), new Vector3(14.6f, 0f, 12f),
            new Vector3(-11f, 0f, -14.6f), new Vector3(11f, 0f, -14.6f), new Vector3(-11f, 0f, 14.6f), new Vector3(11f, 0f, 14.6f)
        };

        public static ProceduralRoomVisual Build(Transform roomRoot, RoomTemplate room, int roomIndex, int seed)
        {
            return BuildInternal(roomRoot, room, roomIndex, seed, null);
        }

        public static ProceduralRoomVisual BuildForAudit(Transform roomRoot, RoomTemplate room, BiomeVisualId forcedBiome, int seed)
        {
            return BuildInternal(roomRoot, room, 0, seed, forcedBiome);
        }

        private static ProceduralRoomVisual BuildInternal(Transform roomRoot, RoomTemplate room, int roomIndex, int seed, BiomeVisualId? forcedBiome)
        {
            if (roomRoot == null || room == null) return null;
            DecorationExclusion.BeginRoom(room.type);
            RunDirector run = GameWorld.Instance == null ? null : GameWorld.Instance.GetComponent<RunDirector>();
            bool home = room.id == "sanctuary";
            int runSeed = run == null ? seed : run.CurrentSeed;
            int totalRooms = run == null || run.TotalRooms <= 0 ? 8 : run.TotalRooms;
            BiomeVisualId biome = forcedBiome.HasValue ? forcedBiome.Value : home ? BiomeVisualId.HomeBase : BiomeVisualCatalog.Resolve(runSeed, roomIndex, totalRooms, room.type);
            bool wardenOverlay = !home && room.type == DungeonRoomType.Boss;
            BiomeVisualDefinition definition = BiomeVisualCatalog.Get(biome);
            room.biome = definition.displayName;
            GameObject visualRoot = new GameObject("Procedural Dressing · " + definition.displayName + " · " + room.type);
            visualRoot.transform.SetParent(roomRoot, false);
            ProceduralRoomVisual visual = visualRoot.AddComponent<ProceduralRoomVisual>();
            visual.biomeId = biome; visual.wardenSanctumOverlay = wardenOverlay; visual.roomType = room.type; visual.visualSeed = seed;
            ApplyBaseMaterials(roomRoot, definition, room.accentColor);
            BuildFloorTiles(visualRoot.transform, definition, seed);
            BuildObjectiveFloorGuide(visualRoot.transform, definition, room.type);
            BuildBoundaryTrim(visualRoot.transform, definition);
            BuildArchitecture(visualRoot.transform, definition, seed);
            BuildAuthoredBiomeKit(visualRoot.transform, definition, seed);
            BuildBiomeDecals(visualRoot.transform, definition, seed);
            BuildPurposeOverlay(visualRoot.transform, definition, room.type, room.accentColor, seed);
            if (wardenOverlay) BuildWardenSanctumOverlay(visualRoot.transform, definition, seed);
            BuildBackgroundDepth(visualRoot.transform, definition, seed);
            BuildAmbientMotion(visualRoot.transform, definition, seed);
            if (home) { BuildHomeBaseOverlay(visualRoot.transform, definition); _lastRunBiome = null; }
            else
            {
                if (_lastRunBiome.HasValue && _lastRunBiome.Value != biome) BuildTransitionMarker(visualRoot.transform, BiomeVisualCatalog.Get(_lastRunBiome.Value), definition);
                _lastRunBiome = biome;
            }
            RegisterCameraSafeForeground(roomRoot);
            visual.rendererCount = visualRoot.GetComponentsInChildren<Renderer>(true).Length;
            VisualRuntimeRegistry.SetRoom(visual.BiomeName + " / " + room.type, visual.rendererCount);
            return visual;
        }

        public static void DecorateDoor(Transform doorRoot, RoomTemplate room)
        {
            if (doorRoot == null || room == null) return;
            RunDirector run = GameWorld.Instance == null ? null : GameWorld.Instance.GetComponent<RunDirector>();
            int seed = run == null ? V1Determinism.Combine(7919, 0, room.id == null ? string.Empty : room.id) : run.CurrentSeed;
            int index = run == null ? 0 : run.RoomIndex + 1;
            int total = run == null || run.TotalRooms <= 0 ? 8 : run.TotalRooms;
            BiomeVisualDefinition definition = BiomeVisualCatalog.Get(BiomeVisualCatalog.Resolve(seed, index, total, room.type));
            for (int side = -1; side <= 1; side += 2)
            {
                GameObject brace = RuntimeVisuals.Primitive(definition.displayName + " Door Brace", definition.pillarShape, doorRoot.position,
                    new Vector3(0.22f, 1.1f, 0.22f), definition.trim, doorRoot);
                RuntimeVisuals.RemoveCollider(brace);
                brace.transform.localPosition = new Vector3(side * 1.02f, 2f, -0.12f);
                brace.transform.localRotation = Quaternion.Euler(0f, 0f, side * 18f);
            }
        }

        private static void ApplyBaseMaterials(Transform root, BiomeVisualDefinition definition, Color roomAccent)
        {
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null) continue;
                if (renderer.gameObject.name == "Room Floor") renderer.sharedMaterial = RuntimeVisuals.Material(definition.floor, 0.08f);
                else if (renderer.gameObject.name.Contains("Wall")) renderer.sharedMaterial = RuntimeVisuals.Material(definition.wall, 0.12f);
                else if (renderer.gameObject.name.Contains("Room Rune")) renderer.sharedMaterial = RuntimeVisuals.Material(Color.Lerp(definition.emission, roomAccent, 0.35f), 0.9f);
            }
        }

        private static void BuildFloorTiles(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            float density = ProfileManager.Current == null ? 1f : ProfileManager.Current.accessibility.environmentDensity;
            int stride = density < 0.45f ? 2 : 1;
            for (int x = -2; x <= 2; x += stride)
            {
                for (int z = -2; z <= 2; z += stride)
                {
                    int variant = V1Determinism.Combine(seed, x * 31 + z * 131, definition.displayName) & int.MaxValue;
                    Color color = definition.FloorVariant(variant);
                    CreateDecoration("Walkable Floor Tile", PrimitiveType.Cube, new Vector3(x * 6.35f, 0.012f, z * 6.35f),
                        new Vector3(6.22f, 0.018f, 6.22f), color, parent);
                }
            }
        }

        private static void BuildObjectiveFloorGuide(Transform parent, BiomeVisualDefinition definition, DungeonRoomType roomType)
        {
            Vector3 destination = roomType == DungeonRoomType.Combat || roomType == DungeonRoomType.Elite || roomType == DungeonRoomType.Miniboss || roomType == DungeonRoomType.Boss || roomType == DungeonRoomType.Challenge
                ? Vector3.zero : new Vector3(0f, 0f, 10.6f);
            Vector3 start = new Vector3(0f, 0f, -10.8f);
            Color guide = Color.Lerp(definition.floor, definition.emission, 0.11f);
            for (int i = 1; i <= 5; i++)
            {
                Vector3 point = Vector3.Lerp(start, destination, i / 6f);
                CreateDecoration("Non-hazard Objective Floor Guide", PrimitiveType.Cube, point + Vector3.up * 0.026f,
                    new Vector3(0.72f, 0.014f, 1.2f), guide, parent);
            }
        }

        private static void BuildWardenSanctumOverlay(Transform parent, BiomeVisualDefinition routeBiome, int seed)
        {
            BiomeVisualDefinition sanctum = BiomeVisualCatalog.Get(BiomeVisualId.WardenSanctum);
            GameObject overlay = new GameObject("Warden Sanctum Overlay on " + routeBiome.displayName);
            overlay.transform.SetParent(parent, false);
            for (int i = 0; i < 8; i++)
            {
                float angle = i / 8f * Mathf.PI * 2f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * 12.4f, 1.45f, Mathf.Sin(angle) * 12.4f);
                GameObject authority = CreateDecoration("Warden Authority Spire", i % 2 == 0 ? PrimitiveType.Cylinder : PrimitiveType.Cube,
                    position, new Vector3(0.55f, 2.9f, 0.55f), Color.Lerp(routeBiome.trim, sanctum.trim, 0.68f), overlay.transform);
                authority.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, i % 2 == 0 ? 0f : 45f);
            }
            for (int layer = 0; layer < 3; layer++)
            {
                LineRenderer ring = RuntimeVisuals.Ring("Warden Seal Layer " + layer, Vector3.zero,
                    Color.Lerp(routeBiome.emission, sanctum.emission, 0.55f + layer * 0.18f), 4.5f + layer * 2.2f, 0.1f + layer * 0.025f, overlay.transform);
                if (ring != null) ring.transform.localPosition = Vector3.up * (0.04f + layer * 0.025f);
            }
            GameObject dais = CreateDecoration("Warden Dais", PrimitiveType.Cylinder, new Vector3(0f, 0.09f, 0f),
                new Vector3(3.4f, 0.16f, 3.4f), Color.Lerp(routeBiome.floor, sanctum.wall, 0.72f), overlay.transform);
            dais.transform.localRotation = Quaternion.Euler(0f, seed % 45, 0f);
        }

        private static void BuildBiomeDecals(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            System.Random random = new System.Random(seed ^ 0x6C91);
            float density = ProfileManager.Current == null ? 1f : ProfileManager.Current.accessibility.environmentDensity;
            int count = Mathf.Clamp(Mathf.RoundToInt(7f * density), 2, 7);
            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f + (float)random.NextDouble() * 0.35f;
                float radius = 10.8f + (float)random.NextDouble() * 3.2f;
                GameObject decal = CreateDecoration(definition.displayName + " Floor Mark", PrimitiveType.Cylinder,
                    new Vector3(Mathf.Cos(angle) * radius, 0.045f, Mathf.Sin(angle) * radius),
                    new Vector3(0.25f + (float)random.NextDouble() * 0.55f, 0.012f, 0.25f + (float)random.NextDouble() * 0.55f),
                    Color.Lerp(definition.trim, definition.emission, 0.35f), parent);
                decal.transform.localRotation = Quaternion.Euler(0f, random.Next(0, 360), 0f);
                decal.AddComponent<DecorationExclusionGuard>();
            }
        }

        private static void BuildTransitionMarker(Transform parent, BiomeVisualDefinition from, BiomeVisualDefinition to)
        {
            Vector3 entry = new Vector3(0f, 0.08f, -13.1f);
            for (int i = 0; i < 3; i++)
            {
                Color color = Color.Lerp(from.emission, to.emission, i / 2f);
                LineRenderer ring = RuntimeVisuals.Ring("Biome Transition · " + from.displayName + " to " + to.displayName, entry, color, 1.15f + i * 0.32f, 0.07f, parent);
                if (ring != null) { ring.transform.localPosition = entry + Vector3.up * i * 0.04f; ring.transform.localRotation = Quaternion.Euler(i * 8f, 0f, i * 11f); }
            }
        }

        private static void BuildBoundaryTrim(Transform parent, BiomeVisualDefinition definition)
        {
            CreateDecoration("North Boundary Trim", PrimitiveType.Cube, new Vector3(0f, 0.18f, 15.9f), new Vector3(30f, 0.18f, 0.3f), definition.trim, parent);
            CreateDecoration("South Boundary Trim", PrimitiveType.Cube, new Vector3(0f, 0.18f, -15.9f), new Vector3(30f, 0.18f, 0.3f), definition.trim, parent);
            CreateDecoration("East Boundary Trim", PrimitiveType.Cube, new Vector3(15.9f, 0.18f, 0f), new Vector3(0.3f, 0.18f, 30f), definition.trim, parent);
            CreateDecoration("West Boundary Trim", PrimitiveType.Cube, new Vector3(-15.9f, 0.18f, 0f), new Vector3(0.3f, 0.18f, 30f), definition.trim, parent);
            Vector3[] corners = { new Vector3(-15.45f, 1.1f, -15.45f), new Vector3(15.45f, 1.1f, -15.45f), new Vector3(-15.45f, 1.1f, 15.45f), new Vector3(15.45f, 1.1f, 15.45f) };
            for (int i = 0; i < corners.Length; i++)
            {
                GameObject corner = CreateDecoration("Authoritative Shell Corner " + i, definition.pillarShape, corners[i], new Vector3(0.75f, 2.2f, 0.75f), definition.wall, parent);
                corner.transform.localRotation = Quaternion.Euler(0f, i * 90f + 45f, 0f);
            }
            for (int side = -1; side <= 1; side += 2)
            {
                CreateDecoration("North Entrance Frame", PrimitiveType.Cube, new Vector3(side * 2.4f, 1.55f, 15.35f), new Vector3(0.42f, 3.1f, 0.55f), definition.trim, parent);
                CreateDecoration("South Entrance Frame", PrimitiveType.Cube, new Vector3(side * 2.4f, 1.55f, -15.35f), new Vector3(0.42f, 3.1f, 0.55f), definition.trim, parent);
            }
        }

        private static void BuildArchitecture(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            System.Random random = new System.Random(seed);
            float density = ProfileManager.Current == null ? 1f : ProfileManager.Current.accessibility.environmentDensity;
            int count = Mathf.Clamp(Mathf.RoundToInt(PerimeterAnchors.Length * density), 4, PerimeterAnchors.Length);
            for (int i = 0; i < count; i++)
            {
                int index = (i + random.Next(PerimeterAnchors.Length)) % PerimeterAnchors.Length;
                Vector3 anchor = PerimeterAnchors[index];
                float height = 1.6f + (float)random.NextDouble() * 2.2f;
                GameObject pillar = CreateDecoration(definition.structuralLanguage + " · Support", definition.pillarShape, anchor + Vector3.up * height * 0.5f,
                    new Vector3(0.55f + (float)random.NextDouble() * 0.35f, height, 0.55f + (float)random.NextDouble() * 0.35f), definition.wall, parent);
                pillar.transform.localRotation = Quaternion.Euler(0f, random.Next(0, 4) * 90f, 0f);
                pillar.AddComponent<DecorationExclusionGuard>();
                if ((i & 1) == 0)
                {
                    GameObject prop = CreateDecoration(definition.displayName + " Prop Cluster", definition.propShape, anchor + new Vector3(anchor.x > 0 ? -0.75f : 0.75f, 0.35f, anchor.z > 0 ? -0.55f : 0.55f),
                        new Vector3(0.38f, 0.65f + (float)random.NextDouble() * 0.5f, 0.38f), definition.trim, parent);
                    prop.transform.localRotation = Quaternion.Euler((float)random.NextDouble() * 12f, random.Next(0, 360), (float)random.NextDouble() * 12f);
                    prop.AddComponent<DecorationExclusionGuard>();
                }
            }
        }

        private static void BuildPurposeOverlay(Transform parent, BiomeVisualDefinition definition, DungeonRoomType type, Color roomAccent, int seed)
        {
            Color color = Color.Lerp(definition.emission, roomAccent, 0.52f);
            GameObject apparatus = new GameObject("Room Apparatus · " + FriendlyPurpose(type));
            apparatus.transform.SetParent(parent, false);
            switch (type)
            {
                case DungeonRoomType.Combat: BuildCombatApparatus(apparatus.transform, color); break;
                case DungeonRoomType.Elite: BuildEliteApparatus(apparatus.transform, color, false); break;
                case DungeonRoomType.ModifierReward: BuildRuneApparatus(apparatus.transform, color); break;
                case DungeonRoomType.SpellCoreReward: BuildCoreApparatus(apparatus.transform, color); break;
                case DungeonRoomType.EquipmentReward: BuildArmoryApparatus(apparatus.transform, color); break;
                case DungeonRoomType.TreasureVault: BuildVaultApparatus(apparatus.transform, color); break;
                case DungeonRoomType.Shop: BuildShopApparatus(apparatus.transform, color); break;
                case DungeonRoomType.SafeWorkshop: BuildWorkshopApparatus(apparatus.transform, color); break;
                case DungeonRoomType.HealingSanctuary: BuildHealingApparatus(apparatus.transform, color); break;
                case DungeonRoomType.CursedBargain: BuildCursedApparatus(apparatus.transform, color); break;
                case DungeonRoomType.Puzzle: BuildPuzzleApparatus(apparatus.transform, color); break;
                case DungeonRoomType.NarrativeEvent: BuildNarrativeApparatus(apparatus.transform, color); break;
                case DungeonRoomType.Challenge: BuildChallengeApparatus(apparatus.transform, color); break;
                case DungeonRoomType.Secret: BuildSecretApparatus(apparatus.transform, color); break;
                case DungeonRoomType.Miniboss: BuildEliteApparatus(apparatus.transform, color, true); break;
                case DungeonRoomType.Boss: BuildBossApparatus(apparatus.transform, color); break;
                case DungeonRoomType.Extraction: BuildExtractionApparatus(apparatus.transform, color); break;
            }
        }

        private static void BuildBackgroundDepth(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            System.Random random = new System.Random(seed ^ 0x513A7);
            float density = ProfileManager.Current == null ? 1f : ProfileManager.Current.accessibility.environmentDensity;
            int count = Mathf.Clamp(Mathf.RoundToInt(10f * density), 3, 10);
            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f;
                float distance = 21f + (float)random.NextDouble() * 8f;
                float height = 3.5f + (float)random.NextDouble() * 7f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * distance, height * 0.5f - 0.25f, Mathf.Sin(angle) * distance);
                GameObject depth = CreateDecoration("Distant " + definition.displayName + " Architecture", (i & 1) == 0 ? definition.pillarShape : PrimitiveType.Cube, position,
                    new Vector3(1.2f + (float)random.NextDouble(), height, 1.2f + (float)random.NextDouble()), Color.Lerp(definition.wall, Color.black, 0.35f), parent);
                depth.AddComponent<CameraSafeForeground>();
            }
        }

        private static void RegisterCameraSafeForeground(Transform roomRoot)
        {
            Renderer[] renderers = roomRoot.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || renderer is LineRenderer || renderer.GetComponent<TextMesh>() != null) continue;
                Vector3 local = roomRoot.InverseTransformPoint(renderer.bounds.center);
                bool perimeter = Mathf.Abs(local.x) > 12f || Mathf.Abs(local.z) > 12f;
                if (!perimeter || renderer.bounds.size.y < 1.25f || renderer.GetComponent<CameraSafeForeground>() != null) continue;
                renderer.gameObject.AddComponent<CameraSafeForeground>();
            }
        }

        private static void BuildAmbientMotion(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            if (!VisualQualityBudget.Current.ambientMotion || ProfileManager.Current != null && ProfileManager.Current.accessibility.environmentDensity < 0.4f) return;
            GameObject root = new GameObject("Ambient Motion · " + definition.displayName);
            root.transform.SetParent(parent, false);
            BiomeAmbientMotion motion = root.AddComponent<BiomeAmbientMotion>();
            motion.rate = definition.ambientMotion;
            motion.radius = 13.8f;
            int motes = ProceduralVisualRuntime.Quality == ArcaneVisualQuality.High ? 8 : ProceduralVisualRuntime.Quality == ArcaneVisualQuality.Medium ? 5 : 3;
            for (int i = 0; i < motes; i++)
            {
                float angle = i / (float)motes * Mathf.PI * 2f;
                GameObject mote = CreateDecoration(definition.displayName + " Ambient Mote", i % 3 == 0 ? definition.propShape : PrimitiveType.Sphere,
                    new Vector3(Mathf.Cos(angle) * 13.8f, 0.8f + (i % 3) * 0.55f, Mathf.Sin(angle) * 13.8f), Vector3.one * 0.09f, definition.emission, root.transform);
                motion.Add(mote.transform, angle);
            }
        }

        private static void BuildHomeBaseOverlay(Transform parent, BiomeVisualDefinition definition)
        {
            for (int i = 0; i < 6; i++)
            {
                float angle = i / 6f * Mathf.PI * 2f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * 8.5f, 0.35f, Mathf.Sin(angle) * 8.5f);
                CreateDecoration("Relic Forge Worktable", PrimitiveType.Cylinder, position, new Vector3(1.15f, 0.28f, 1.15f), definition.trim, parent);
                CreateDecoration("Relic Forge Focus", PrimitiveType.Cube, position + Vector3.up * 0.65f, Vector3.one * 0.24f, definition.emission, parent);
            }
        }

        private static void BuildAuthoredBiomeKit(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            switch (definition.id)
            {
                case BiomeVisualId.OssuaryCatacombs: BuildOssuaryKit(parent, definition, seed); break;
                case BiomeVisualId.EmberFoundry: BuildFoundryKit(parent, definition, seed); break;
                case BiomeVisualId.SunkenArchive: BuildArchiveKit(parent, definition, seed); break;
                case BiomeVisualId.VenomCistern: BuildCisternKit(parent, definition, seed); break;
                case BiomeVisualId.HomeBase: BuildRelicForgeKit(parent, definition); break;
            }
        }

        private static void BuildOssuaryKit(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            Vector3[] centers = { new Vector3(-13.8f, 1.45f, 0f), new Vector3(13.8f, 1.45f, 0f), new Vector3(0f, 1.45f, -13.8f), new Vector3(0f, 1.45f, 13.8f) };
            for (int arch = 0; arch < centers.Length; arch++)
            {
                for (int rib = -2; rib <= 2; rib++)
                {
                    Vector3 lateral = arch < 2 ? Vector3.forward * rib * 0.42f : Vector3.right * rib * 0.42f;
                    GameObject bone = CreateDecoration("Ossuary Rib Arch", PrimitiveType.Capsule, centers[arch] + lateral + Vector3.up * (0.5f - Mathf.Abs(rib) * 0.11f),
                        new Vector3(0.12f, 1.1f - Mathf.Abs(rib) * 0.12f, 0.12f), Color.Lerp(definition.trim, Color.white, 0.12f), parent);
                    bone.transform.localRotation = Quaternion.Euler(0f, arch < 2 ? 90f : 0f, rib * 14f);
                }
            }
            for (int i = 0; i < 6; i++)
            {
                float side = i < 3 ? -1f : 1f;
                float offset = (i % 3 - 1) * 3.2f;
                CreateDecoration("Burial Recess", PrimitiveType.Cube, new Vector3(side * 14.9f, 1.05f, offset), new Vector3(0.22f, 1.4f, 1.05f), Color.Lerp(definition.wall, Color.black, 0.22f), parent);
                CreateDecoration("Burial Urn", PrimitiveType.Capsule, new Vector3(side * 14.55f, 0.45f, offset), new Vector3(0.28f, 0.48f, 0.28f), definition.trim, parent);
            }
        }

        private static void BuildFoundryKit(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            for (int i = 0; i < 4; i++)
            {
                float angle = i * Mathf.PI * 0.5f + Mathf.PI * 0.25f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * 13.2f, 1.1f, Mathf.Sin(angle) * 13.2f);
                CreateDecoration("Foundry Furnace", PrimitiveType.Cube, position, new Vector3(1.45f, 2.2f, 1.1f), definition.wall, parent);
                CreateDecoration("Furnace Mouth", PrimitiveType.Cube, position + new Vector3(-Mathf.Cos(angle), 0f, -Mathf.Sin(angle)) * 0.62f,
                    new Vector3(0.72f, 0.62f, 0.12f), definition.emission, parent);
                for (int pipe = 0; pipe < 2; pipe++)
                {
                    GameObject tube = CreateDecoration("Foundry Pressure Pipe", PrimitiveType.Cylinder, position + Vector3.up * (1.3f + pipe * 0.38f),
                        new Vector3(0.16f, 1.35f, 0.16f), definition.trim, parent);
                    tube.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 90f);
                }
            }
            for (int lane = -1; lane <= 1; lane += 2)
                CreateDecoration("Molten Channel", PrimitiveType.Cube, new Vector3(lane * 11.8f, 0.035f, 0f), new Vector3(0.3f, 0.03f, 18f), definition.emission, parent);
        }

        private static void BuildArchiveKit(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            for (int side = -1; side <= 1; side += 2)
                for (int shelf = -2; shelf <= 2; shelf++)
                {
                    Vector3 position = new Vector3(side * 13.9f, 1.25f, shelf * 4.2f);
                    CreateDecoration("Archive Shelf", PrimitiveType.Cube, position, new Vector3(0.65f, 2.5f, 1.45f), definition.wall, parent);
                    for (int row = 0; row < 3; row++) CreateDecoration("Archive Record Row", PrimitiveType.Cube,
                        position + new Vector3(-side * 0.36f, -0.72f + row * 0.72f, 0f), new Vector3(0.08f, 0.16f, 1.12f), Color.Lerp(definition.trim, definition.emission, row * 0.12f), parent);
                }
            for (int i = 0; i < 4; i++)
            {
                float angle = i * Mathf.PI * 0.5f + Mathf.PI * 0.25f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * 11.8f, 1.5f, Mathf.Sin(angle) * 11.8f);
                GameObject glyph = CreateDecoration("Suspended Archive Glyph", PrimitiveType.Cube, position, Vector3.one * 0.38f, definition.emission, parent);
                glyph.transform.localRotation = Quaternion.Euler(35f, i * 57f, 45f);
                glyph.AddComponent<SimpleVisualMotion>();
            }
        }

        private static void BuildCisternKit(Transform parent, BiomeVisualDefinition definition, int seed)
        {
            for (int i = 0; i < 6; i++)
            {
                float angle = i / 6f * Mathf.PI * 2f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * 13.4f, 0.95f, Mathf.Sin(angle) * 13.4f);
                CreateDecoration("Cistern Reservoir", PrimitiveType.Cylinder, position, new Vector3(0.9f, 1.9f, 0.9f), definition.wall, parent);
                CreateDecoration("Visible Contamination", PrimitiveType.Sphere, position + Vector3.up * 0.28f, new Vector3(0.72f, 0.45f, 0.72f), definition.emission, parent);
                GameObject pipe = CreateDecoration("Cistern Feed Pipe", PrimitiveType.Cylinder, position + Vector3.up * 1.2f,
                    new Vector3(0.16f, 1.25f, 0.16f), definition.trim, parent);
                pipe.transform.localRotation = Quaternion.Euler(90f, -angle * Mathf.Rad2Deg, 0f);
            }
            for (int i = 0; i < 4; i++)
                CreateDecoration("Cistern Drain", PrimitiveType.Cylinder, Quaternion.Euler(0f, i * 90f, 0f) * Vector3.forward * 10.8f + Vector3.up * 0.025f,
                    new Vector3(0.78f, 0.025f, 0.78f), Color.Lerp(definition.floor, definition.emission, 0.35f), parent);
        }

        private static void BuildRelicForgeKit(Transform parent, BiomeVisualDefinition definition)
        {
            for (int i = 0; i < 4; i++)
            {
                float angle = i * Mathf.PI * 0.5f + Mathf.PI * 0.25f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * 13.4f, 1.2f, Mathf.Sin(angle) * 13.4f);
                CreateDecoration("Relic Forge Buttress", PrimitiveType.Cube, position, new Vector3(1.1f, 2.4f, 1.1f), definition.wall, parent);
                CreateDecoration("Relic Forge Power Conduit", PrimitiveType.Capsule, position + Vector3.up * 1.1f, new Vector3(0.16f, 0.95f, 0.16f), definition.emission, parent);
            }
        }

        private static void BuildCombatApparatus(Transform parent, Color color)
        {
            for (int i = 0; i < 4; i++) ApparatusPost(parent, "Combat Spawn Pylon", Quaternion.Euler(0f, i * 90f + 45f, 0f) * Vector3.forward * 11.5f, color, PrimitiveType.Capsule, 1.1f);
            PurposeBoundary(parent, "Combat Boundary", color, 11.5f, 0.055f);
        }

        private static void BuildEliteApparatus(Transform parent, Color color, bool miniboss)
        {
            int count = miniboss ? 6 : 4;
            for (int i = 0; i < count; i++) ApparatusPost(parent, miniboss ? "Miniboss Chain Anchor" : "Elite Trial Obelisk",
                Quaternion.Euler(0f, i / (float)count * 360f, 0f) * Vector3.forward * (miniboss ? 9.8f : 11.2f), color, PrimitiveType.Cube, miniboss ? 1.65f : 1.3f);
            PurposeBoundary(parent, miniboss ? "Miniboss Arena" : "Elite Arena", color, miniboss ? 9.8f : 11.2f, 0.12f);
        }

        private static void BuildRuneApparatus(Transform parent, Color color)
        {
            for (int i = 0; i < 7; i++)
            {
                float angle = i / 6f * Mathf.PI * 2f;
                Vector3 position = i == 6 ? new Vector3(0f, 0.12f, 10.8f) : new Vector3(Mathf.Cos(angle) * 1.1f, 0.12f, 10.8f + Mathf.Sin(angle) * 1.1f);
                ApparatusPost(parent, "Support Rune Hex Socket", position, color, PrimitiveType.Cylinder, 0.28f);
            }
        }

        private static void BuildCoreApparatus(Transform parent, Color color)
        {
            Vector3 center = new Vector3(0f, 0.6f, 10.8f);
            ApparatusPost(parent, "Spell Core Cradle", center, color, PrimitiveType.Sphere, 0.68f);
            for (int i = 0; i < 4; i++) ApparatusPost(parent, "Core Element Fin", center + Quaternion.Euler(0f, i * 90f, 0f) * Vector3.forward * 0.9f, color, PrimitiveType.Cube, 0.34f);
        }

        private static void BuildArmoryApparatus(Transform parent, Color color)
        {
            for (int i = -1; i <= 1; i++)
            {
                Vector3 position = new Vector3(i * 2.2f, 0.8f, 11.6f);
                ApparatusPost(parent, "Equipment Display Stand", position, color, i == 0 ? PrimitiveType.Capsule : PrimitiveType.Cube, 0.8f);
                CreateDecoration("Armory Rail", PrimitiveType.Cube, position + Vector3.up * 0.9f, new Vector3(0.8f, 0.08f, 0.08f), color, parent);
            }
        }

        private static void BuildVaultApparatus(Transform parent, Color color)
        {
            for (int x = -1; x <= 1; x++) for (int z = 0; z < 2; z++)
                CreateDecoration("Treasure Vault Lockbox", PrimitiveType.Cube, new Vector3(x * 1.3f, 0.35f + z * 0.35f, 11.2f + z * 0.6f), new Vector3(0.9f, 0.5f, 0.65f), Color.Lerp(color, Color.white, z * 0.12f), parent);
            PurposeBoundary(parent, "Vault Security Seal", color, 3.2f, 0.09f, new Vector3(0f, 0f, 11.2f));
        }

        private static void BuildShopApparatus(Transform parent, Color color)
        {
            for (int i = -1; i <= 1; i++)
            {
                Vector3 position = new Vector3(i * 3.1f, 0.42f, 11.3f);
                CreateDecoration("Shop Counter", PrimitiveType.Cube, position, new Vector3(2.2f, 0.7f, 0.8f), Color.Lerp(color, Color.black, 0.25f), parent);
                ApparatusPost(parent, "Shop Offer Lantern", position + Vector3.up * 1.1f, color, PrimitiveType.Sphere, 0.28f);
            }
        }

        private static void BuildWorkshopApparatus(Transform parent, Color color)
        {
            CreateDecoration("Safe Room Forge Anvil", PrimitiveType.Cube, new Vector3(-2f, 0.55f, 10.8f), new Vector3(1.4f, 0.72f, 0.8f), color, parent);
            CreateDecoration("Safe Room Rune Table", PrimitiveType.Cylinder, new Vector3(2f, 0.42f, 10.8f), new Vector3(1.15f, 0.3f, 1.15f), Color.Lerp(color, Color.white, 0.12f), parent);
            PurposeBoundary(parent, "Safe Workshop Boundary", color, 4f, 0.055f, new Vector3(0f, 0f, 10.8f));
        }

        private static void BuildHealingApparatus(Transform parent, Color color)
        {
            CreateDecoration("Healing Basin", PrimitiveType.Cylinder, new Vector3(0f, 0.15f, 10.5f), new Vector3(2.2f, 0.22f, 2.2f), Color.Lerp(color, Color.white, 0.18f), parent);
            for (int i = 0; i < 5; i++) ApparatusPost(parent, "Healing Droplet", new Vector3((i - 2) * 0.36f, 0.65f + (i % 2) * 0.28f, 10.5f), color, PrimitiveType.Sphere, 0.16f);
            PurposeBoundary(parent, "Healing Pool", color, 2.2f, 0.07f, new Vector3(0f, 0f, 10.5f));
        }

        private static void BuildCursedApparatus(Transform parent, Color color)
        {
            Color risk = Color.Lerp(color, new Color(1f, 0.02f, 0.16f), 0.65f);
            ApparatusPost(parent, "Cursed Bargain Altar", new Vector3(0f, 0.85f, 10.8f), risk, PrimitiveType.Cube, 1.1f);
            for (int i = 0; i < 6; i++) ApparatusPost(parent, "Cursed Thorn", new Vector3(Mathf.Cos(i / 6f * Mathf.PI * 2f) * 1.8f, 0.42f, 10.8f + Mathf.Sin(i / 6f * Mathf.PI * 2f) * 1.8f), risk, PrimitiveType.Capsule, 0.48f);
            PurposeBoundary(parent, "Cursed Contract", risk, 2.8f, 0.12f, new Vector3(0f, 0f, 10.8f));
        }

        private static void BuildPuzzleApparatus(Transform parent, Color color)
        {
            Vector3 center = new Vector3(0f, 0f, 10.8f);
            for (int i = 0; i < 5; i++)
            {
                Vector3 node = center + Quaternion.Euler(0f, i / 5f * 360f, 0f) * Vector3.forward * 2.2f;
                ApparatusPost(parent, "Puzzle Logic Node " + i, node + Vector3.up * 0.4f, color, i % 2 == 0 ? PrimitiveType.Cube : PrimitiveType.Sphere, 0.42f);
                RuntimeVisuals.Beam(center, node, color, 999f).transform.SetParent(parent, true);
            }
        }

        private static void BuildNarrativeApparatus(Transform parent, Color color)
        {
            CreateDecoration("Story Archive Plinth", PrimitiveType.Cylinder, new Vector3(0f, 0.45f, 11f), new Vector3(1.3f, 0.48f, 1.3f), color, parent);
            for (int page = 0; page < 3; page++)
            {
                GameObject record = CreateDecoration("Suspended Story Record", PrimitiveType.Cube, new Vector3((page - 1) * 0.62f, 1.25f + page * 0.12f, 11f), new Vector3(0.42f, 0.06f, 0.58f), Color.Lerp(color, Color.white, page * 0.14f), parent);
                record.transform.localRotation = Quaternion.Euler(10f, page * 23f, page * 9f);
            }
        }

        private static void BuildChallengeApparatus(Transform parent, Color color)
        {
            for (int i = 0; i < 8; i++) ApparatusPost(parent, "Challenge Timer Stake", Quaternion.Euler(0f, i * 45f, 0f) * Vector3.forward * 11.7f, color, PrimitiveType.Capsule, 0.9f);
            CreateDecoration("Challenge Hourglass Upper", PrimitiveType.Cylinder, new Vector3(0f, 1.4f, 12.2f), new Vector3(0.72f, 0.08f, 0.72f), color, parent);
            CreateDecoration("Challenge Hourglass Lower", PrimitiveType.Cylinder, new Vector3(0f, 0.35f, 12.2f), new Vector3(0.72f, 0.08f, 0.72f), color, parent);
        }

        private static void BuildSecretApparatus(Transform parent, Color color)
        {
            Color hidden = Color.Lerp(color, Color.black, 0.42f);
            ApparatusPost(parent, "Secret Keystone", new Vector3(-12.8f, 0.9f, 10.8f), hidden, PrimitiveType.Cube, 0.8f);
            PurposeBoundary(parent, "Secret Trace", hidden, 1.8f, 0.035f, new Vector3(-12.8f, 0f, 10.8f));
        }

        private static void BuildBossApparatus(Transform parent, Color color)
        {
            for (int i = 0; i < 8; i++) ApparatusPost(parent, "Warden Chain Anchor", Quaternion.Euler(0f, i * 45f, 0f) * Vector3.forward * 10.5f, color, i % 2 == 0 ? PrimitiveType.Cube : PrimitiveType.Capsule, 1.3f);
            PurposeBoundary(parent, "Warden Mechanical Arena", color, 10.5f, 0.16f);
        }

        private static void BuildExtractionApparatus(Transform parent, Color color)
        {
            Vector3 center = new Vector3(0f, 0.8f, 10.8f);
            for (int i = 0; i < 5; i++)
            {
                GameObject arch = CreateDecoration("Extraction Portal Segment", PrimitiveType.Capsule,
                    center + new Vector3(Mathf.Cos(Mathf.Lerp(0.15f, Mathf.PI - 0.15f, i / 4f)) * 2.2f, Mathf.Sin(Mathf.Lerp(0.15f, Mathf.PI - 0.15f, i / 4f)) * 2.2f, 0f),
                    new Vector3(0.18f, 0.75f, 0.18f), color, parent);
                arch.transform.localRotation = Quaternion.Euler(0f, 0f, i * 22f - 44f);
            }
            PurposeBoundary(parent, "Extraction Destination Seal", color, 2.4f, 0.12f, new Vector3(0f, 0f, 10.8f));
        }

        private static void ApparatusPost(Transform parent, string name, Vector3 position, Color color, PrimitiveType shape, float height)
        {
            if (DecorationExclusion.IsReserved(position)) return;
            Vector3 scale = shape == PrimitiveType.Cube ? new Vector3(0.42f, height, 0.42f) : new Vector3(0.34f, height, 0.34f);
            CreateDecoration(name, shape, position, scale, color, parent);
        }

        private static void PurposeBoundary(Transform parent, string name, Color color, float radius, float width, Vector3? offset = null)
        {
            Vector3 position = offset ?? Vector3.zero;
            LineRenderer boundary = RuntimeVisuals.Ring(name, position, color, radius, width, parent);
            if (boundary != null) boundary.transform.localPosition = position + Vector3.up * 0.03f;
        }

        private static GameObject CreateDecoration(string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color, Transform parent)
        {
            GameObject go = RuntimeVisuals.Primitive(name, type, position, scale, color, parent);
            RuntimeVisuals.RemoveCollider(go);
            return go;
        }

        private static PrimitiveType PurposeShape(DungeonRoomType type)
        {
            if (type == DungeonRoomType.Boss || type == DungeonRoomType.Elite || type == DungeonRoomType.Miniboss || type == DungeonRoomType.Challenge) return PrimitiveType.Capsule;
            if (type == DungeonRoomType.Shop || type == DungeonRoomType.TreasureVault || type == DungeonRoomType.EquipmentReward) return PrimitiveType.Cube;
            if (type == DungeonRoomType.HealingSanctuary || type == DungeonRoomType.SafeWorkshop || type == DungeonRoomType.Extraction) return PrimitiveType.Sphere;
            return PrimitiveType.Cylinder;
        }

        private static string FriendlyPurpose(DungeonRoomType type)
        {
            return type.ToString().Replace("ModifierReward", "Support Rune Reward").Replace("SpellCoreReward", "Spell Core Reward")
                .Replace("EquipmentReward", "Equipment Reward").Replace("SafeWorkshop", "Safe Room").Replace("HealingSanctuary", "Healing Room")
                .Replace("NarrativeEvent", "Story Event").Replace("CursedBargain", "Cursed Choice");
        }
    }

    public sealed class BiomeAmbientMotion : MonoBehaviour
    {
        private readonly List<Transform> _motes = new List<Transform>();
        private readonly List<float> _phases = new List<float>();
        public float rate;
        public float radius;
        public void Add(Transform mote, float phase) { _motes.Add(mote); _phases.Add(phase); }
        private void Update()
        {
            if (GameWorld.Instance != null && GameWorld.Instance.ModalOpen) return;
            for (int i = 0; i < _motes.Count; i++)
            {
                if (_motes[i] == null) continue;
                float angle = _phases[i] + Time.time * rate * 0.08f;
                _motes[i].localPosition = new Vector3(Mathf.Cos(angle) * radius, 0.7f + Mathf.Sin(Time.time * rate + i) * 0.35f + (i % 3) * 0.45f, Mathf.Sin(angle) * radius);
                _motes[i].Rotate(18f * Time.deltaTime, 32f * Time.deltaTime, 12f * Time.deltaTime);
            }
        }
    }

    public static class DecorationExclusion
    {
        private struct ReservedVolume
        {
            public Vector3 center;
            public float radius;
            public string reason;
        }

        private static readonly List<ReservedVolume> RuntimeVolumes = new List<ReservedVolume>(24);

        public static void BeginRoom(DungeonRoomType roomType)
        {
            RuntimeVolumes.Clear();
            Reserve(Vector3.zero, 5.4f, "reward and combat center");
            Reserve(new Vector3(0f, 0f, -10.5f), 2.7f, "player entry");
            Reserve(new Vector3(-9f, 0f, 14.25f), 3.1f, "left route door");
            Reserve(new Vector3(0f, 0f, 14.25f), 3.1f, "center route door");
            Reserve(new Vector3(9f, 0f, 14.25f), 3.1f, "right route door");
            Reserve(new Vector3(14.2f, 0f, 0f), 2.8f, "east boundary passage");
            Reserve(new Vector3(-14.2f, 0f, 0f), 2.8f, "west boundary passage");
            if (roomType == DungeonRoomType.Shop || roomType == DungeonRoomType.SafeWorkshop ||
                roomType == DungeonRoomType.HealingSanctuary || roomType == DungeonRoomType.Extraction ||
                roomType == DungeonRoomType.Puzzle || roomType == DungeonRoomType.Challenge)
                Reserve(new Vector3(0f, 0f, 10.8f), 3.2f, "room-purpose apparatus");
        }

        public static void Reserve(Vector3 center, float radius, string reason)
        {
            if (radius <= 0f) return;
            RuntimeVolumes.Add(new ReservedVolume { center = center, radius = radius, reason = reason ?? string.Empty });
        }

        public static bool IsReserved(Vector3 position)
        {
            Vector2 point = new Vector2(position.x, position.z);
            for (int i = 0; i < RuntimeVolumes.Count; i++)
            {
                ReservedVolume volume = RuntimeVolumes[i];
                Vector2 center = new Vector2(volume.center.x, volume.center.z);
                if ((point - center).sqrMagnitude < volume.radius * volume.radius) return true;
            }
            return false;
        }
    }

    public sealed class DecorationExclusionGuard : MonoBehaviour
    {
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            Apply();
        }

        private void LateUpdate()
        {
            if (_renderer != null && _renderer.enabled) Apply();
        }

        private void Apply()
        {
            if (_renderer != null && DecorationExclusion.IsReserved(_renderer.bounds.center)) _renderer.enabled = false;
        }
    }

    public sealed class CameraSafeForeground : MonoBehaviour
    {
        private Renderer _renderer;
        private void Awake() { _renderer = GetComponent<Renderer>(); }
        private void LateUpdate()
        {
            if (_renderer == null || Camera.main == null || GameWorld.Instance == null || GameWorld.Instance.Player == null) return;
            Vector3 playerViewport = Camera.main.WorldToViewportPoint(GameWorld.Instance.Player.transform.position + Vector3.up * 0.8f);
            Vector3 objectViewport = Camera.main.WorldToViewportPoint(_renderer.bounds.center);
            bool inFront = objectViewport.z > 0f && objectViewport.z < playerViewport.z;
            bool overlaps = Mathf.Abs(objectViewport.x - playerViewport.x) < 0.07f && Mathf.Abs(objectViewport.y - playerViewport.y) < 0.12f;
            _renderer.enabled = !(inFront && overlaps);
        }
    }

    public sealed class SimpleVisualMotion : MonoBehaviour
    {
        private Vector3 _basePosition;
        private void Awake() { _basePosition = transform.localPosition; }
        private void Update()
        {
            if (ProfileManager.Current != null && ProfileManager.Current.accessibility.reducedMotion) return;
            transform.localPosition = _basePosition + Vector3.up * Mathf.Sin(Time.time * 1.6f + transform.GetSiblingIndex()) * 0.08f;
            transform.Rotate(12f * Time.deltaTime, 28f * Time.deltaTime, 8f * Time.deltaTime);
        }
    }
}
