using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ArpgLootKind31
    {
        Item,
        Currency,
        Map,
        Core,
        Rune
    }

    public sealed class ArpgLootPickup31 : MonoBehaviour
    {
        public ArpgLootKind31 kind;
        public ArpgItem30 item;
        public ArpgMapItem30 map;
        public ArpgCurrency30 currency;
        public int amount;
        public int discoverySeed;
        public string label;
        public Color labelColor = Color.white;

        private Vector3 _origin;
        private float _phase;
        private bool _claimed;
        private GUIStyle _style;

        private void Awake()
        {
            _origin = transform.position;
            _phase = UnityEngine.Random.value * Mathf.PI * 2f;
        }

        private void Update()
        {
            transform.Rotate(0f, 80f * Time.deltaTime, 0f, Space.World);
            transform.position = _origin + Vector3.up * (0.2f + Mathf.Sin(Time.time * 2.5f + _phase) * 0.15f);

            if (_claimed || ArpgFoundation30.Profile == null || GameWorld.Instance == null || GameWorld.Instance.Player == null) return;
            float distance = Vector3.Distance(GameWorld.Instance.Player.transform.position, transform.position);
            if (distance <= 1.35f || (distance <= 3f && ArpgInput31.Pressed(ArpgAction31.Interact)))
                Claim();
        }

        private void OnGUI()
        {
            if (_claimed || Camera.main == null || ArpgFrontend31.Instance == null || !ArpgFrontend31.Instance.IsGameplay) return;
            if (!ArpgArsenalRuntime32.ShouldShowLoot(this)) return;
            Vector3 screen = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.8f);
            if (screen.z <= 0f) return;
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = labelColor }
                };
            }
            _style.normal.textColor = ArpgSettings31.HighContrastLoot ? Color.white : labelColor;
            Rect labelRect = new Rect(screen.x - 140f, Screen.height - screen.y - 18f, 280f, 38f);
            if (ArpgSettings31.HighContrastLoot) GUI.Box(labelRect, GUIContent.none);
            GUI.Label(labelRect, label, _style);
        }

        private void Claim()
        {
            if (_claimed) return;
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            if (profile == null) return;

            string message = string.Empty;
            bool success = true;
            switch (kind)
            {
                case ArpgLootKind31.Item:
                    success = ArpgArsenalRuntime32.TryPickupItem(profile, item, out message);
                    break;
                case ArpgLootKind31.Currency:
                    profile.AddCurrency(currency, Mathf.Max(1, amount));
                    ArpgArsenalRuntime32.AddLegacyCurrency(currency, Mathf.Max(1, amount));
                    message = "Picked up " + Mathf.Max(1, amount) + " " + ArpgItems30.CurrencyName(currency) + ".";
                    break;
                case ArpgLootKind31.Map:
                    if (map != null) profile.maps.Add(map);
                    else success = false;
                    message = success ? "Picked up a Tier " + map.tier + " map." : "The map item was invalid.";
                    break;
                case ArpgLootKind31.Core:
                    message = "Spell Core: " + ArpgLegacyBridge30.GrantRandomCore(GameWorld.Instance, profile, discoverySeed);
                    success = !message.EndsWith(": ");
                    break;
                case ArpgLootKind31.Rune:
                    message = "Support Rune: " + ArpgLegacyBridge30.GrantRandomRune(GameWorld.Instance, profile, discoverySeed);
                    success = !message.EndsWith(": ");
                    break;
            }

            if (!success)
            {
                if (ArpgInterface30.Instance != null) ArpgInterface30.Instance.SetMessage(message);
                return;
            }

            _claimed = true;
            ArpgProfileStore30.Save(profile);
            if (ArpgInterface30.Instance != null) ArpgInterface30.Instance.SetMessage(message);
            Destroy(gameObject);
        }
    }

    public sealed class ArpgCompletionCache31 : MonoBehaviour
    {
        public Action onClaim;
        public string bossName;
        public string masteryText;

        private bool _claimed;
        private Vector3 _origin;
        private GUIStyle _style;
        private float _interactHold;

        private void Awake()
        {
            _origin = transform.position;
        }

        private void Update()
        {
            transform.Rotate(0f, 35f * Time.deltaTime, 0f);
            transform.position = _origin + Vector3.up * (0.12f + Mathf.Sin(Time.time * 1.7f) * 0.08f);
            if (_claimed || GameWorld.Instance == null || GameWorld.Instance.Player == null) return;
            float distance = Vector3.Distance(GameWorld.Instance.Player.transform.position, transform.position);
            bool confirmed;
            if (ArpgSettings31.HoldToInteract)
            {
                _interactHold = distance <= 3.3f && ArpgInput31.Held(ArpgAction31.Interact) ? _interactHold + Time.unscaledDeltaTime : 0f;
                confirmed = _interactHold >= 0.45f;
            }
            else confirmed = distance <= 3.3f && ArpgInput31.Pressed(ArpgAction31.Interact);
            if (confirmed)
            {
                _claimed = true;
                Action callback = onClaim;
                onClaim = null;
                if (callback != null) callback();
                Destroy(gameObject);
            }
        }

        private void OnGUI()
        {
            if (_claimed || Camera.main == null || ArpgFrontend31.Instance == null || !ArpgFrontend31.Instance.IsGameplay) return;
            Vector3 screen = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.25f);
            if (screen.z <= 0f) return;
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 17,
                    fontStyle = FontStyle.Bold,
                    wordWrap = true,
                    normal = { textColor = new Color(1f, 0.84f, 0.32f) }
                };
            }
            GUI.Label(new Rect(screen.x - 220f, Screen.height - screen.y - 35f, 440f, 70f),
                "GUARDIAN CACHE\n[" + ArpgSettings31.BindingLabel(ArpgAction31.Interact) + "] Secure Rewards",
                _style);
        }
    }

    public static class ArpgLoot31
    {
        private static GameObject _root;
        private static readonly List<Material> Materials = new List<Material>();
        private static int _dropSequence;

        public static void BeginMap()
        {
            ClearAll();
            _root = new GameObject("Arcane Engine 3.1 · Map Loot");
            UnityEngine.Object.DontDestroyOnLoad(_root);
            _dropSequence = 0;
        }

        public static void ClearAll()
        {
            if (_root != null) UnityEngine.Object.Destroy(_root);
            _root = null;
            foreach (Material material in Materials)
                if (material != null) UnityEngine.Object.Destroy(material);
            Materials.Clear();
            _dropSequence = 0;
        }

        public static void SpawnKillDrops(ArpgProfile30 profile, ArpgMapItem30 map, int kills, Vector3 nearPosition)
        {
            if (profile == null || map == null || kills <= 0) return;
            EnsureRoot();

            for (int index = 0; index < kills; index++)
            {
                int seed = ArpgDeterminism30.Combine(map.seed, profile.totalMapsCompleted, _dropSequence++);
                System.Random random = new System.Random(seed);
                float roll = (float)random.NextDouble();
                Vector3 position = nearPosition + new Vector3(
                    (float)random.NextDouble() * 3f - 1.5f,
                    0.65f,
                    (float)random.NextDouble() * 3f - 1.5f);

                if (roll < 0.12f)
                {
                    ArpgItem30 item = ArpgItems30.GenerateItem(Mathf.Max(1, map.tier * 2 + 1), profile.characterClass, seed, ArpgStatHooks30.ItemRarityBonus(profile));
                    SpawnItem(item, position);
                }
                else if (roll < 0.24f)
                {
                    ArpgCurrency30 currency = RollCoreCurrency(seed);
                    SpawnCurrency(currency, 1, position);
                }
                else if (roll < 0.28f && map.tier >= 2)
                {
                    SpawnDiscovery(random.NextDouble() < 0.72 ? ArpgLootKind31.Rune : ArpgLootKind31.Core, seed, position);
                }
            }
        }

        public static ArpgCompletionCache31 SpawnCompletionCache(
            ArpgMapDefinition30 definition,
            Vector3 position,
            Action onClaim)
        {
            EnsureRoot();
            GameObject cacheObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cacheObject.name = "Guardian Cache · " + (definition == null ? "Unknown" : definition.bossName);
            cacheObject.transform.SetParent(_root.transform, false);
            cacheObject.transform.position = position;
            cacheObject.transform.localScale = new Vector3(1.8f, 1.2f, 1.8f);
            Renderer renderer = cacheObject.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = CreateMaterial(new Color(1f, 0.65f, 0.12f), 3f);
            ArpgCompletionCache31 cache = cacheObject.AddComponent<ArpgCompletionCache31>();
            cache.onClaim = onClaim;
            cache.bossName = definition == null ? "Guardian" : definition.bossName;
            cache.masteryText = definition == null ? string.Empty : definition.masteryDescription;
            return cache;
        }

        public static void SpawnItem(ArpgItem30 item, Vector3 position)
        {
            if (item == null) return;
            GameObject pickup = CreatePickup("Item · " + item.displayName, position, RarityColor(item.rarity));
            ArpgLootPickup31 component = pickup.AddComponent<ArpgLootPickup31>();
            component.kind = ArpgLootKind31.Item;
            component.item = item;
            component.label = item.displayName;
            component.labelColor = RarityColor(item.rarity);
        }

        public static void SpawnCurrency(ArpgCurrency30 currency, int amount, Vector3 position)
        {
            GameObject pickup = CreatePickup("Currency · " + currency, position, new Color(0.95f, 0.72f, 0.22f));
            ArpgLootPickup31 component = pickup.AddComponent<ArpgLootPickup31>();
            component.kind = ArpgLootKind31.Currency;
            component.currency = currency;
            component.amount = Mathf.Max(1, amount);
            component.label = component.amount + " × " + ArpgItems30.CurrencyName(currency);
            component.labelColor = new Color(1f, 0.82f, 0.32f);
        }

        public static void SpawnMap(ArpgMapItem30 map, Vector3 position)
        {
            if (map == null) return;
            GameObject pickup = CreatePickup("Map · Tier " + map.tier, position, new Color(0.45f, 0.75f, 1f));
            ArpgLootPickup31 component = pickup.AddComponent<ArpgLootPickup31>();
            component.kind = ArpgLootKind31.Map;
            component.map = map;
            component.label = "Tier " + map.tier + " " + map.rarity + " Map";
            component.labelColor = new Color(0.55f, 0.82f, 1f);
        }

        public static void SpawnDiscovery(ArpgLootKind31 kind, int seed, Vector3 position)
        {
            Color color = kind == ArpgLootKind31.Core
                ? new Color(0.85f, 0.35f, 1f)
                : new Color(0.25f, 0.95f, 0.72f);
            GameObject pickup = CreatePickup(kind + " Discovery", position, color);
            ArpgLootPickup31 component = pickup.AddComponent<ArpgLootPickup31>();
            component.kind = kind;
            component.discoverySeed = seed;
            component.label = kind == ArpgLootKind31.Core ? "Unidentified Spell Core" : "Unidentified Support Rune";
            component.labelColor = color;
        }

        private static GameObject CreatePickup(string name, Vector3 position, Color color)
        {
            EnsureRoot();
            GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pickup.name = name;
            pickup.transform.SetParent(_root.transform, false);
            pickup.transform.position = position;
            pickup.transform.localScale = Vector3.one * 0.48f;
            Renderer renderer = pickup.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = CreateMaterial(color, 2.5f);
            Collider collider = pickup.GetComponent<Collider>();
            if (collider != null) collider.isTrigger = true;
            return pickup;
        }

        private static void EnsureRoot()
        {
            if (_root != null) return;
            _root = new GameObject("Arcane Engine 3.1 · Map Loot");
            UnityEngine.Object.DontDestroyOnLoad(_root);
        }

        private static ArpgCurrency30 RollCoreCurrency(int seed)
        {
            int value = ArpgDeterminism30.Positive(seed) % 100;
            if (value < 35) return ArpgCurrency30.SparkOfAlteration;
            if (value < 60) return ArpgCurrency30.RuneOfAugmentation;
            if (value < 78) return ArpgCurrency30.SigilOfElevation;
            if (value < 90) return ArpgCurrency30.ReformationOrb;
            return ArpgCurrency30.ArcaneExalt;
        }

        private static Color RarityColor(ArpgItemRarity30 rarity)
        {
            if (rarity == ArpgItemRarity30.Unique) return new Color(1f, 0.42f, 0.12f);
            if (rarity == ArpgItemRarity30.Exceptional) return new Color(0.82f, 0.35f, 1f);
            if (rarity == ArpgItemRarity30.Rare) return new Color(1f, 0.82f, 0.28f);
            if (rarity == ArpgItemRarity30.Magic) return new Color(0.35f, 0.62f, 1f);
            return new Color(0.86f, 0.9f, 0.94f);
        }

        private static Material CreateMaterial(Color color, float emission)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = "ArcaneEngine31_Loot";
            material.color = color;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * emission);
            }
            material.hideFlags = HideFlags.HideAndDontSave;
            Materials.Add(material);
            return material;
        }
    }
}
