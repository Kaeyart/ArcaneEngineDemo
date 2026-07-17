#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.Editor
{
    public static class V21ContentBuilder
    {
        private const string Root = "Assets/ArcaneEngine/Resources/V21Content";
        private const string SessionKey = "ArcaneEngine.V21.ContentChecked";

        [InitializeOnLoadMethod]
        private static void QueueInitialBuild()
        {
            if (SessionState.GetBool(SessionKey, false)) return;
            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += RebuildIfMissing;
        }

        private static void RebuildIfMissing()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += RebuildIfMissing;
                return;
            }
            string[] required = { Root + "/Spells", Root + "/Runes", Root + "/Items", Root + "/Relics", Root + "/Rooms",
                Root + "/Affixes", Root + "/RoomDefinitions", Root + "/Shops", Root + "/Rewards" };
            if (required.All(AssetDatabase.IsValidFolder) &&
                AssetDatabase.FindAssets("t:V21RoomLayoutAsset", new[] { Root + "/Rooms" }).Length >= 36 &&
                AssetDatabase.FindAssets("t:V21AffixContentAsset", new[] { Root + "/Affixes" }).Length > 0) return;
            RebuildAll();
        }

        [MenuItem("Arcane Engine/2.1/Rebuild Authored Content")]
        public static void RebuildAll()
        {
            EnsureFolder("Assets/ArcaneEngine/Resources");
            EnsureFolder(Root);
            foreach (string name in new[] { "Spells", "Runes", "Items", "Relics", "Rooms", "Enemies", "Audio", "Affixes", "RoomDefinitions", "Shops", "Rewards" })
                EnsureFolder(Root + "/" + name);
            EnsureFolder("Assets/ArcaneEngine/Prefabs");
            EnsureFolder("Assets/ArcaneEngine/Prefabs/Rooms");

            DemoCatalog.Ensure();
            foreach (SpellCoreDefinition source in DemoCatalog.AllCores)
                CopyAsset(source, Root + "/Spells/Spell_" + Safe(source.id) + ".asset");
            foreach (SpellModifierDefinition source in DemoCatalog.AllModifiers)
                CopyAsset(source, Root + "/Runes/Rune_" + Safe(source.id) + ".asset");
            foreach (ItemDefinition source in DemoCatalog.AllItems)
                CopyAsset(source, Root + "/Items/Item_" + Safe(source.id) + ".asset");
            foreach (RelicDefinition source in MegaCatalog.AllRelics)
                CopyAsset(source, Root + "/Relics/Legendary_" + Safe(source.id) + ".asset");

            BuildAffixAssets();
            BuildRoomDefinitionAssets();
            BuildExtensionAssets();

            BuildRooms();
            BuildEnemies();
            BuildAudioDefinitions();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Arcane Engine 2.1 authored content rebuilt: persistent spells, runes, items, Legendaries, rooms, enemies and audio events.");
        }

        private static void BuildRooms()
        {
            string[] biomes = { "The Ossuary Catacombs", "Ember Foundry", "Sunken Archive", "Venom Cistern" };
            for (int biomeIndex = 0; biomeIndex < biomes.Length; biomeIndex++)
            {
                for (int variant = 0; variant < 6; variant++)
                    CreateRoom(biomes[biomeIndex], DungeonRoomType.Combat, variant, biomeIndex);
                CreateRoom(biomes[biomeIndex], DungeonRoomType.Elite, 0, biomeIndex);
                CreateRoom(biomes[biomeIndex], DungeonRoomType.Challenge, 1, biomeIndex);
            }
            CreateRoom("Shared", DungeonRoomType.Shop, 0, 4);
            CreateRoom("Shared", DungeonRoomType.Shop, 1, 4);
            CreateRoom("Shared", DungeonRoomType.SafeWorkshop, 0, 5);
            CreateRoom("Shared", DungeonRoomType.SafeWorkshop, 1, 5);
            foreach (int biomeIndex in Enumerable.Range(0, biomes.Length))
            {
                CreateRoom(biomes[biomeIndex], DungeonRoomType.Miniboss, 0, biomeIndex);
                CreateRoom(biomes[biomeIndex], DungeonRoomType.Boss, 0, biomeIndex);
            }
        }

        private static void BuildAffixAssets()
        {
            foreach (V11AffixDefinition source in V11Itemization.AllAffixes)
            {
                string path = Root + "/Affixes/Affix_" + Safe(source.id) + ".asset";
                V21AffixContentAsset asset = AssetDatabase.LoadAssetAtPath<V21AffixContentAsset>(path);
                if (asset == null) { asset = ScriptableObject.CreateInstance<V21AffixContentAsset>(); AssetDatabase.CreateAsset(asset, path); }
                asset.stableId = source.id; asset.displayName = source.displayName; asset.stat = source.stat; asset.kind = source.kind;
                asset.group = source.group; asset.tags = source.tags; asset.slots = source.slots; asset.baseMinimum = source.baseMinimum;
                asset.baseMaximum = source.baseMaximum; asset.percentage = source.percentage; asset.local = source.local; asset.weight = source.weight;
                EditorUtility.SetDirty(asset);
            }
        }

        private static void BuildRoomDefinitionAssets()
        {
            foreach (RoomTemplate source in MegaCatalog.AllRooms.ToArray())
            {
                string path = Root + "/RoomDefinitions/RoomDefinition_" + Safe(source.id) + ".asset";
                V21RoomDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<V21RoomDefinitionAsset>(path);
                if (asset == null) { asset = ScriptableObject.CreateInstance<V21RoomDefinitionAsset>(); AssetDatabase.CreateAsset(asset, path); }
                asset.stableId = source.id; asset.displayName = source.displayName; asset.biome = source.biome; asset.roomType = source.type;
                asset.difficulty = source.difficulty; asset.obstaclePattern = source.obstaclePattern; asset.hasHazards = source.hasHazards;
                asset.floorColor = source.floorColor; asset.accentColor = source.accentColor; EditorUtility.SetDirty(asset);
            }
        }

        private static void BuildExtensionAssets()
        {
            string shopPath = Root + "/Shops/ShopService_ReserveRecovery.asset";
            V21ShopServiceAsset shop = AssetDatabase.LoadAssetAtPath<V21ShopServiceAsset>(shopPath);
            if (shop == null) { shop = ScriptableObject.CreateInstance<V21ShopServiceAsset>(); AssetDatabase.CreateAsset(shop, shopPath); }
            shop.stableId = "reserve_recovery"; shop.specialization = "Apothecary"; shop.title = "Apothecary · Reserve Tonic";
            shop.description = "Restore Health and Mana. This authored service is stocked only by the Apothecary.";
            shop.serviceContentId = "service:recovery"; shop.category = RewardCategory.Healing; shop.price = 27; EditorUtility.SetDirty(shop);

            string rewardPath = Root + "/Rewards/Reward_EssenceCache.asset";
            V21RewardDefinitionAsset reward = AssetDatabase.LoadAssetAtPath<V21RewardDefinitionAsset>(rewardPath);
            if (reward == null) { reward = ScriptableObject.CreateInstance<V21RewardDefinitionAsset>(); AssetDatabase.CreateAsset(reward, rewardPath); }
            reward.stableId = "essence_cache"; reward.category = RewardCategory.Essence; reward.title = "Secured Essence Cache";
            reward.description = "Gain permanent Essence immediately."; reward.amount = 4; reward.color = new Color(0.25f, 1f, 0.82f); EditorUtility.SetDirty(reward);
        }

        private static void CreateRoom(string biome, DungeonRoomType type, int variant, int biomeIndex)
        {
            string id = Safe(biome).ToLowerInvariant() + "_" + type.ToString().ToLowerInvariant() + "_" + variant;
            string path = Root + "/Rooms/Room_" + id + ".asset";
            V21RoomLayoutAsset asset = AssetDatabase.LoadAssetAtPath<V21RoomLayoutAsset>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<V21RoomLayoutAsset>();
                AssetDatabase.CreateAsset(asset, path);
            }
            asset.stableId = id;
            asset.displayName = biome + " " + type + " " + (variant + 1);
            asset.biome = biome == "Shared" ? string.Empty : biome;
            asset.roomType = type;
            asset.shell = (variant + biomeIndex * 2 + (int)type) % 8;
            asset.floorSize = type == DungeonRoomType.Boss
                ? new Vector2(40f, 40f)
                : new Vector2(32f + (variant % 3) * 2f, 30f + ((variant + 1) % 3) * 2f);
            asset.cameraSize = type == DungeonRoomType.Boss ? 19f : 16f;
            asset.obstaclePositions.Clear();
            asset.obstacleScales.Clear();
            asset.hazardAnchors.Clear();
            asset.enemySpawnZones.Clear();
            asset.objectiveAnchors.Clear();
            int obstacleCount = type == DungeonRoomType.Boss ? 4 : 3 + variant % 3;
            for (int i = 0; i < obstacleCount; i++)
            {
                float angle = (i / Mathf.Max(1f, obstacleCount)) * Mathf.PI * 2f + asset.shell * 0.27f;
                float radius = 4.5f + (i % 2) * 2.5f;
                asset.obstaclePositions.Add(new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
                asset.obstacleScales.Add(new Vector3(1.5f + (i + variant) % 3, 1.2f + (i % 2), 1.4f + ((i + 1) % 3)));
            }
            asset.hazardAnchors.Add(new Vector3(-8f, 0.02f, 6f));
            asset.hazardAnchors.Add(new Vector3(8f, 0.02f, -6f));
            asset.enemySpawnZones.AddRange(new[]
            {
                new Vector3(-10f, 0f, 9f), new Vector3(10f, 0f, 9f),
                new Vector3(-10f, 0f, -9f), new Vector3(10f, 0f, -9f)
            });
            asset.objectiveAnchors.AddRange(new[]
            {
                new Vector3(-7f, 0f, 0f), new Vector3(7f, 0f, 0f), new Vector3(0f, 0f, 7f)
            });
            asset.rewardAnchor = Vector3.zero;
            EditorUtility.SetDirty(asset);
            BuildRoomPreviewPrefab(asset);
        }

        private static void BuildRoomPreviewPrefab(V21RoomLayoutAsset asset)
        {
            GameObject root = new GameObject("RoomTemplate_" + asset.stableId);
            root.SetActive(false);
            root.AddComponent<V21RoomPrefabMarker>().layout = asset;
            foreach (Vector3 value in asset.enemySpawnZones)
                CreateMarker(root.transform, "Enemy Spawn", value, new Color(1f, 0.2f, 0.2f));
            foreach (Vector3 value in asset.objectiveAnchors)
                CreateMarker(root.transform, "Objective Anchor", value, new Color(0.2f, 0.8f, 1f));
            CreateMarker(root.transform, "Reward Anchor", asset.rewardAnchor, new Color(1f, 0.75f, 0.1f));
            PrefabUtility.SaveAsPrefabAsset(root, "Assets/ArcaneEngine/Prefabs/Rooms/" + root.name + ".prefab");
            UnityEngine.Object.DestroyImmediate(root);
        }

        private static void CreateMarker(Transform parent, string name, Vector3 position, Color color)
        {
            GameObject marker = new GameObject(name);
            marker.transform.SetParent(parent, false);
            marker.transform.localPosition = position;
            marker.AddComponent<V21AuthoringMarker>().color = color;
        }

        private static void BuildEnemies()
        {
            string[] biomes = { "The Ossuary Catacombs", "Ember Foundry", "Sunken Archive", "Venom Cistern" };
            EnemyArchetype[] archetypes = (EnemyArchetype[])Enum.GetValues(typeof(EnemyArchetype));
            foreach (string biome in biomes)
            foreach (EnemyArchetype archetype in archetypes.Where(value => value != EnemyArchetype.TrainingDummy))
            {
                string id = Safe(biome).ToLowerInvariant() + "_" + archetype.ToString().ToLowerInvariant();
                string path = Root + "/Enemies/Enemy_" + id + ".asset";
                V21EnemyContentAsset asset = AssetDatabase.LoadAssetAtPath<V21EnemyContentAsset>(path);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<V21EnemyContentAsset>();
                    AssetDatabase.CreateAsset(asset, path);
                }
                asset.stableId = id;
                asset.displayName = biome + " " + archetype;
                asset.archetype = archetype;
                asset.biome = biome;
                asset.mechanicalFamily = archetype.ToString();
                asset.counterplay = Counterplay(archetype);
                asset.primary = Color.HSVToRGB((Mathf.Abs(id.GetHashCode()) % 1000) / 1000f, 0.55f, 0.9f);
                asset.telegraph = archetype == EnemyArchetype.Hexer
                    ? new Color(0.85f, 0.25f, 1f)
                    : new Color(1f, 0.25f, 0.12f);
                asset.anticipationSeconds = archetype == EnemyArchetype.Charger ? 0.9f : 0.65f;
                asset.activeSeconds = 0.2f;
                asset.recoverySeconds = archetype == EnemyArchetype.Bulwark ? 0.8f : 0.5f;
                asset.damageRadius = archetype == EnemyArchetype.Controller ? 3.5f : 1.2f;
                EditorUtility.SetDirty(asset);
            }
        }

        private static void BuildAudioDefinitions()
        {
            string[] events =
            {
                "spell_cast", "spell_travel", "spell_impact", "status_apply", "status_consume", "spell_trigger", "summon", "defensive",
                "enemy_telegraph", "enemy_attack", "enemy_hurt", "enemy_death", "reward_reveal", "door", "shop_buy", "ui_confirm", "ui_cancel",
                "boss_phase", "music_explore", "music_combat", "music_elite", "music_boss", "music_reward", "music_home"
            };
            foreach (string id in events)
            {
                string path = Root + "/Audio/Audio_" + id + ".asset";
                V21AudioEventAsset asset = AssetDatabase.LoadAssetAtPath<V21AudioEventAsset>(path);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<V21AudioEventAsset>();
                    AssetDatabase.CreateAsset(asset, path);
                }
                asset.stableId = id;
                asset.category = id.StartsWith("music_") ? "Music"
                    : id.StartsWith("ui_") ? "Interface"
                    : id.StartsWith("enemy_") ? "Enemy" : "Effects";
                asset.pitchRange = id.StartsWith("music_") ? Vector2.one : new Vector2(0.95f, 1.05f);
                asset.volumeRange = new Vector2(0.82f, 1f);
                asset.maxVoices = id.StartsWith("music_") ? 1 : 8;
                asset.loop = id.StartsWith("music_") || id == "spell_travel";
                List<AudioClip> clips = AssetDatabase.LoadAllAssetsAtPath(path).OfType<AudioClip>().ToList();
                int wantedClips = id.StartsWith("music_") ? 1 : 3;
                while (clips.Count < wantedClips)
                {
                    AudioClip clip = CreateAuthoredClip(id, clips.Count, id.StartsWith("music_"));
                    AssetDatabase.AddObjectToAsset(clip, asset);
                    clips.Add(clip);
                }
                asset.clips = clips.Take(wantedClips).ToArray();
                EditorUtility.SetDirty(asset);
            }
        }

        private static AudioClip CreateAuthoredClip(string id, int variation, bool music)
        {
            const int rate = 44100;
            float seconds = music ? 4f : 0.24f + variation * 0.035f;
            int samples = Mathf.RoundToInt(rate * seconds);
            float[] data = new float[samples];
            int stable = id.Aggregate(17, (current, character) => current * 31 + character);
            float root = 90f + Mathf.Abs(stable % 420);
            int[] chord = { 0, 4, 7, 12 };
            for (int i = 0; i < samples; i++)
            {
                float time = i / (float)rate;
                float phase = i / (float)Mathf.Max(1, samples - 1);
                float envelope = music ? Mathf.SmoothStep(0f, 1f, Mathf.Min(phase * 16f, (1f - phase) * 16f)) :
                    Mathf.Pow(Mathf.Sin(Mathf.PI * phase), 1.4f);
                int note = music ? chord[Mathf.FloorToInt(time * 2f + variation) % chord.Length] : variation * 3;
                float frequency = root * Mathf.Pow(2f, note / 12f);
                float fundamental = Mathf.Sin(time * frequency * Mathf.PI * 2f);
                float harmonic = Mathf.Sin(time * frequency * 2.01f * Mathf.PI * 2f) * 0.24f;
                float sub = music ? Mathf.Sin(time * root * 0.5f * Mathf.PI * 2f) * 0.12f : 0f;
                data[i] = (fundamental + harmonic + sub) * envelope * (music ? 0.055f : 0.16f);
            }
            AudioClip clip = AudioClip.Create(id + "_take_" + (variation + 1), samples, 1, rate, false);
            clip.SetData(data, 0);
            clip.name = id + "_take_" + (variation + 1);
            return clip;
        }

        private static void CopyAsset<T>(T source, string path) where T : ScriptableObject
        {
            T target = AssetDatabase.LoadAssetAtPath<T>(path);
            if (target == source)
            {
                target.hideFlags = HideFlags.None;
                EditorUtility.SetDirty(target);
                return;
            }
            if (target == null)
            {
                target = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(target, path);
            }
            EditorUtility.CopySerialized(source, target);
            target.hideFlags = HideFlags.None;
            EditorUtility.SetDirty(target);
        }

        private static string Counterplay(EnemyArchetype archetype)
        {
            switch (archetype)
            {
                case EnemyArchetype.Charger: return "Sidestep the committed charge, then punish recovery.";
                case EnemyArchetype.Bulwark: return "Attack around the shield or break it with sustained pressure.";
                case EnemyArchetype.Hexer: return "Leave the marked zone before the cast completes.";
                case EnemyArchetype.Controller: return "Destroy control hazards and keep space to reposition.";
                case EnemyArchetype.Mirror: return "Stop firing into reflection and change angle.";
                case EnemyArchetype.Leech: return "Interrupt the support channel or eliminate the linked target.";
                default: return "Read anticipation, avoid the active shape, and punish recovery.";
            }
        }

        private static string Safe(string value)
        {
            if (string.IsNullOrEmpty(value)) return "unnamed";
            return new string(value.Select(character => char.IsLetterOrDigit(character) ? character : '_').ToArray());
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string name = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
