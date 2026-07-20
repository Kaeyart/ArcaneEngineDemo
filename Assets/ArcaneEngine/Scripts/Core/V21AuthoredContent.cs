using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArcaneEngine
{

public static class V21AuthoredContentOverlay
    {
        public static void Apply(Dictionary<string, SpellCoreDefinition> cores,
            Dictionary<string, SpellModifierDefinition> modifiers, Dictionary<string, ItemDefinition> items)
        {
            foreach (SpellCoreDefinition value in Resources.LoadAll<SpellCoreDefinition>("V21Content/Spells"))
                if (value != null && !string.IsNullOrEmpty(value.id)) cores[value.id] = value;
            foreach (SpellModifierDefinition value in Resources.LoadAll<SpellModifierDefinition>("V21Content/Runes"))
                if (value != null && !string.IsNullOrEmpty(value.id)) modifiers[value.id] = value;
            foreach (ItemDefinition value in Resources.LoadAll<ItemDefinition>("V21Content/Items"))
                if (value != null && !string.IsNullOrEmpty(value.id)) items[value.id] = value;
        }

        public static void ApplyRelicsRooms(Dictionary<string, RelicDefinition> relics, List<RoomTemplate> rooms)
        {
            foreach (RelicDefinition value in Resources.LoadAll<RelicDefinition>("V21Content/Relics"))
                if (value != null && !string.IsNullOrEmpty(value.id)) relics[value.id] = value;
            foreach (V21RoomDefinitionAsset value in Resources.LoadAll<V21RoomDefinitionAsset>("V21Content/RoomDefinitions"))
            {
                if (value == null || string.IsNullOrEmpty(value.stableId)) continue;
                rooms.RemoveAll(room => room != null && room.id == value.stableId);
                rooms.Add(value.ToRuntime());
            }
        }

        public static IReadOnlyList<V21ShopServiceAsset> ShopServices { get { return Resources.LoadAll<V21ShopServiceAsset>("V21Content/Shops"); } }
        public static IReadOnlyList<V21RewardDefinitionAsset> Rewards { get { return Resources.LoadAll<V21RewardDefinitionAsset>("V21Content/Rewards"); } }

        public static IReadOnlyList<string> ValidatePersistentAssets()
        {
            List<string> failures = new List<string>();
#if UNITY_EDITOR
            // Resources.LoadAll can return empty in EditMode test contexts — fall back to AssetDatabase.
            ValidateIds(LoadRuntimeOrEditor<SpellCoreDefinition>("V21Content/Spells", "t:SpellCoreDefinition", value => value == null ? null : value.id), "Spell", failures);
            ValidateIds(LoadRuntimeOrEditor<SpellModifierDefinition>("V21Content/Runes", "t:SpellModifierDefinition", value => value == null ? null : value.id), "Rune", failures);
            ValidateIds(LoadRuntimeOrEditor<ItemDefinition>("V21Content/Items", "t:ItemDefinition", value => value == null ? null : value.id), "Item", failures);
            ValidateIds(LoadRuntimeOrEditor<RelicDefinition>("V21Content/Relics", "t:RelicDefinition", value => value == null ? null : value.id), "Legendary", failures);
            ValidateIds(LoadRuntimeOrEditor<V21RoomLayoutAsset>("V21Content/Rooms", "t:V21RoomLayoutAsset", value => value == null ? null : value.stableId), "Room", failures);
            ValidateIds(LoadRuntimeOrEditor<V21AffixContentAsset>("V21Content/Affixes", "t:V21AffixContentAsset", value => value == null ? null : value.stableId), "Affix", failures);
            ValidateIds(LoadRuntimeOrEditor<V21RoomDefinitionAsset>("V21Content/RoomDefinitions", "t:V21RoomDefinitionAsset", value => value == null ? null : value.stableId), "Room definition", failures);
            ValidateIds(LoadRuntimeOrEditor<V21ShopServiceAsset>("V21Content/Shops", "t:V21ShopServiceAsset", value => value == null ? null : value.stableId), "Shop service", failures);
            ValidateIds(LoadRuntimeOrEditor<V21RewardDefinitionAsset>("V21Content/Rewards", "t:V21RewardDefinitionAsset", value => value == null ? null : value.stableId), "Reward", failures);
#else
            ValidateIds(Resources.LoadAll<SpellCoreDefinition>("V21Content/Spells").Select(value => value == null ? null : value.id), "Spell", failures);
            ValidateIds(Resources.LoadAll<SpellModifierDefinition>("V21Content/Runes").Select(value => value == null ? null : value.id), "Rune", failures);
            ValidateIds(Resources.LoadAll<ItemDefinition>("V21Content/Items").Select(value => value == null ? null : value.id), "Item", failures);
            ValidateIds(Resources.LoadAll<RelicDefinition>("V21Content/Relics").Select(value => value == null ? null : value.id), "Legendary", failures);
            ValidateIds(Resources.LoadAll<V21RoomLayoutAsset>("V21Content/Rooms").Select(value => value == null ? null : value.stableId), "Room", failures);
            ValidateIds(Resources.LoadAll<V21AffixContentAsset>("V21Content/Affixes").Select(value => value == null ? null : value.stableId), "Affix", failures);
            ValidateIds(Resources.LoadAll<V21RoomDefinitionAsset>("V21Content/RoomDefinitions").Select(value => value == null ? null : value.stableId), "Room definition", failures);
            ValidateIds(Resources.LoadAll<V21ShopServiceAsset>("V21Content/Shops").Select(value => value == null ? null : value.stableId), "Shop service", failures);
            ValidateIds(Resources.LoadAll<V21RewardDefinitionAsset>("V21Content/Rewards").Select(value => value == null ? null : value.stableId), "Reward", failures);
#endif
            return failures;
        }

#if UNITY_EDITOR
        private static IEnumerable<string> LoadRuntimeOrEditor<T>(string resourcePath, string filter, Func<T, string> idSelector) where T : UnityEngine.Object
        {
            T[] loaded = Resources.LoadAll<T>(resourcePath);
            if (loaded != null && loaded.Length > 0)
                return loaded.Select(value => idSelector(value));
            string folder = "Assets/ArcaneEngine/Resources/" + resourcePath;
            string[] guids = AssetDatabase.FindAssets(filter, new[] { folder });
            List<string> ids = new List<string>();
            foreach (string guid in guids)
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null) ids.Add(idSelector(asset));
            }
            return ids;
        }
#endif

        private static void ValidateIds(IEnumerable<string> source, string family, List<string> failures)
        {
            HashSet<string> ids = new HashSet<string>();
            int count = 0;
            foreach (string id in source)
            {
                count++;
                if (string.IsNullOrEmpty(id)) failures.Add(family + " asset has no stable ID.");
                else if (!ids.Add(id)) failures.Add(family + " has duplicate ID " + id + ".");
            }
            if (count == 0) failures.Add(family + " authored assets have not been generated. Use Arcane Engine > 2.1 > Rebuild Authored Content.");
        }
    }

    public static class V21RoomLayoutRuntime
    {
        private static V21RoomLayoutAsset[] _layouts;

        public static List<Vector3> Build(Transform parent, RoomTemplate room, int depth, int seed)
        {
            V21RoomLayoutAsset layout = Select(room, seed);
            Vector2 size = layout == null ? new Vector2(34f, 34f) : layout.floorSize;
            int shell = layout == null ? Mathf.Abs(V1Determinism.Combine(seed, depth, room.id)) % 8 : layout.shell;
            Color wall = Color.Lerp(room.floorColor, room.accentColor, 0.24f);
            BuildShell(parent, shell, size, room.floorColor, wall);
            List<Vector3> reserved = new List<Vector3>();

            if (layout != null)
            {
                layout.Normalize();
                for (int i = 0; i < layout.obstaclePositions.Count; i++)
                {
                    Vector3 position = layout.obstaclePositions[i];
                    Vector3 scale = i < layout.obstacleScales.Count ? layout.obstacleScales[i] : new Vector3(2f, 1.6f, 2f);
                    CreateObstacle(parent, position, scale, wall, i);
                    reserved.Add(position);
                }
                reserved.AddRange(layout.hazardAnchors);
                return reserved;
            }

            UnityEngine.Random.State old = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);
            int count = room.type == DungeonRoomType.Boss ? 4 : 3 + room.obstaclePattern;
            for (int i = 0; i < count; i++)
            {
                Vector3 scale = new Vector3(UnityEngine.Random.Range(1.2f, 3.2f), UnityEngine.Random.Range(1f, 2.4f), UnityEngine.Random.Range(1.2f, 3.2f));
                Vector3 position = PatternPosition(shell, i, count, size);
                CreateObstacle(parent, position, scale, wall, i);
                reserved.Add(position);
            }
            UnityEngine.Random.state = old;
            return reserved;
        }

        public static V21RoomLayoutAsset Select(RoomTemplate room, int seed)
        {
            if (_layouts == null) _layouts = Resources.LoadAll<V21RoomLayoutAsset>("V21Content/Rooms");
            if (_layouts == null || _layouts.Length == 0) return null;
            List<V21RoomLayoutAsset> exact = _layouts.Where(value => value != null && value.roomType == room.type &&
                (string.IsNullOrEmpty(value.biome) || string.IsNullOrEmpty(room.biome) || value.biome == room.biome)).ToList();
            if (exact.Count == 0) exact = _layouts.Where(value => value != null && value.roomType == room.type).ToList();
            if (exact.Count == 0) exact = _layouts.Where(value => value != null && value.roomType == DungeonRoomType.Combat).ToList();
            return exact.Count == 0 ? null : exact[Mathf.Abs(V1Determinism.Combine(seed, room.difficulty, room.id)) % exact.Count];
        }

        private static void BuildShell(Transform parent, int shell, Vector2 size, Color floor, Color wall)
        {
            float width = size.x;
            float depth = size.y;
            if (shell == 1 || shell == 5)
            {
                Floor(parent, new Vector3(0f, -0.35f, 0f), new Vector3(width * 0.48f, 0.7f, depth), floor);
                Floor(parent, new Vector3(0f, -0.34f, 0f), new Vector3(width, 0.7f, depth * 0.48f), floor);
            }
            else if (shell == 2 || shell == 6)
            {
                Floor(parent, new Vector3(-width * 0.2f, -0.35f, 0f), new Vector3(width * 0.56f, 0.7f, depth * 0.72f), floor);
                Floor(parent, new Vector3(width * 0.24f, -0.34f, 0f), new Vector3(width * 0.42f, 0.7f, depth), floor);
            }
            else if (shell == 3 || shell == 7)
            {
                Floor(parent, new Vector3(0f, -0.35f, -depth * 0.2f), new Vector3(width, 0.7f, depth * 0.58f), floor);
                Floor(parent, new Vector3(0f, -0.34f, depth * 0.25f), new Vector3(width * 0.65f, 0.7f, depth * 0.4f), floor);
            }
            else Floor(parent, new Vector3(0f, -0.35f, 0f), new Vector3(width, 0.7f, depth), floor);

            float hx = width * 0.5f;
            float hz = depth * 0.5f;
            RuntimeVisuals.Primitive("North Boundary", PrimitiveType.Cube, new Vector3(0f, 1f, hz), new Vector3(width + 2f, 2.7f, 1f), wall, parent);
            RuntimeVisuals.Primitive("South Boundary", PrimitiveType.Cube, new Vector3(0f, 1f, -hz), new Vector3(width + 2f, 2.7f, 1f), wall, parent);
            RuntimeVisuals.Primitive("East Boundary", PrimitiveType.Cube, new Vector3(hx, 1f, 0f), new Vector3(1f, 2.7f, depth + 2f), wall, parent);
            RuntimeVisuals.Primitive("West Boundary", PrimitiveType.Cube, new Vector3(-hx, 1f, 0f), new Vector3(1f, 2.7f, depth + 2f), wall, parent);
        }

        private static void Floor(Transform parent, Vector3 position, Vector3 scale, Color color)
        {
            RuntimeVisuals.Primitive("Authored Floor Module", PrimitiveType.Cube, position, scale, color, parent);
        }

        private static void CreateObstacle(Transform parent, Vector3 position, Vector3 scale, Color color, int index)
        {
            position.y = Mathf.Max(0.55f, scale.y * 0.45f);
            GameObject obstacle = RuntimeVisuals.Primitive("Authored Obstacle " + (index + 1), index % 2 == 0 ? PrimitiveType.Cube : PrimitiveType.Cylinder,
                position, scale, color, parent);
            obstacle.AddComponent<DungeonObstacle>().radius = Mathf.Max(scale.x, scale.z) * 0.58f;
        }

        private static Vector3 PatternPosition(int shell, int index, int count, Vector2 size)
        {
            float angle = (index / Mathf.Max(1f, count)) * Mathf.PI * 2f + shell * 0.31f;
            float radius = Mathf.Min(size.x, size.y) * (0.16f + (index % 2) * 0.08f);
            if (shell == 2 || shell == 6) return new Vector3(index % 2 == 0 ? -6f : 5f, 0f, Mathf.Sin(angle) * radius);
            if (shell == 3 || shell == 7) return new Vector3(Mathf.Cos(angle) * radius, 0f, index % 2 == 0 ? -5f : 4f);
            return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }
    }
}
