using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ArpgRefugeStationType31
    {
        SpellForge,
        MapDevice,
        Stash,
        Crafting,
        Constellations,
        TrainingTarget,
        Character,
        Arsenal
    }

    public sealed class ArpgRefugeStation31 : MonoBehaviour
    {
        public ArpgRefugeStationType31 stationType;
        public string displayName;
        public string description;
        public float interactionRadius = 3.2f;
    }

    public sealed class ArpgRefuge31 : MonoBehaviour
    {
        public static ArpgRefuge31 Instance { get; private set; }
        public static string CurrentPrompt { get; private set; } = string.Empty;
        public static ArpgRefugeStationType31? NearbyStation { get; private set; }

        private GameWorld _world;
        private readonly List<ArpgRefugeStation31> _stations = new List<ArpgRefugeStation31>();
        private readonly List<Material> _materials = new List<Material>();
        private ArpgRefugeStation31 _nearby;
        private GUIStyle _promptStyle;
        private GUIStyle _labelStyle;
        private Texture2D _promptBackground;
        private float _nextTrainingRefresh;
        private float _interactHold;

        public string TrainingReport
        {
            get
            {
                return "Total Damage: " + ArpgTrainingTarget31.TotalDamage.ToString("0.0") +
                       "\nCurrent DPS: " + ArpgTrainingTarget31.DamagePerSecond.ToString("0.0") +
                       "\nHit Events: " + ArpgTrainingTarget31.HitEvents +
                       "\nLast Event: " + ArpgTrainingTarget31.LastEvent;
            }
        }

        public void ResetTrainingMetrics()
        {
            ArpgTrainingTarget31.ResetMetrics();
        }

        public static void Build(GameWorld world)
        {
            if (world == null) return;
            if (Instance != null) Destroy(Instance.gameObject);

            GameObject root = new GameObject("Astral Refuge · 3.1 Stations");
            DontDestroyOnLoad(root);
            ArpgRefuge31 refuge = root.AddComponent<ArpgRefuge31>();
            refuge._world = world;
            refuge.CreateEnvironment();
        }

        public static void Clear()
        {
            if (Instance != null) Destroy(Instance.gameObject);
            CurrentPrompt = string.Empty;
            NearbyStation = null;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (_promptBackground != null) Destroy(_promptBackground);
            foreach (Material material in _materials)
                if (material != null) Destroy(material);
            _materials.Clear();
            CurrentPrompt = string.Empty;
            NearbyStation = null;
        }

        private void Update()
        {
            if (ArpgFrontend31.Instance == null || !ArpgFrontend31.Instance.IsGameplay) return;
            if (ArpgFoundation30.Instance == null || ArpgFoundation30.Instance.MapActive) return;
            if (_world == null) _world = GameWorld.Instance;
            if (_world == null || _world.Player == null) return;

            Vector3 position = _world.Player.transform.position;
            _nearby = _stations
                .Where(value => value != null)
                .OrderBy(value => Vector3.SqrMagnitude(value.transform.position - position))
                .FirstOrDefault();

            if (_nearby == null || Vector3.Distance(_nearby.transform.position, position) > _nearby.interactionRadius)
            {
                _nearby = null;
                NearbyStation = null;
                CurrentPrompt = string.Empty;
            }
            else
            {
                NearbyStation = _nearby.stationType;
                CurrentPrompt = "[" + ArpgSettings31.BindingLabel(ArpgAction31.Interact) + "] " + (ArpgSettings31.HoldToInteract ? "Hold · " : string.Empty) + _nearby.displayName;
                if (ArpgSettings31.HoldToInteract)
                {
                    _interactHold = ArpgInput31.Held(ArpgAction31.Interact) ? _interactHold + Time.unscaledDeltaTime : 0f;
                    if (_interactHold >= 0.45f)
                    {
                        _interactHold = 0f;
                        Activate(_nearby);
                    }
                }
                else if (ArpgInput31.Pressed(ArpgAction31.Interact)) Activate(_nearby);
            }

            if (Time.unscaledTime >= _nextTrainingRefresh)
            {
                _nextTrainingRefresh = Time.unscaledTime + 2f;
                EnsureTrainingTarget();
            }
        }

        private void OnGUI()
        {
            if (ArpgFrontend31.Instance == null || !ArpgFrontend31.Instance.IsGameplay) return;
            if (ArpgFoundation30.Instance == null || ArpgFoundation30.Instance.MapActive) return;
            EnsureStyles();

            Camera camera = Camera.main;
            if (camera != null)
            {
                foreach (ArpgRefugeStation31 station in _stations)
                {
                    if (station == null) continue;
                    Vector3 screen = camera.WorldToScreenPoint(station.transform.position + Vector3.up * 2.4f);
                    if (screen.z <= 0f) continue;
                    Rect rect = new Rect(screen.x - 110f, Screen.height - screen.y - 18f, 220f, 36f);
                    GUI.Label(rect, station.displayName, _labelStyle);
                }
            }

            if (!string.IsNullOrEmpty(CurrentPrompt))
            {
                Rect box = new Rect(Screen.width * 0.5f - 240f, Screen.height - 130f, 480f, 56f);
                GUI.DrawTexture(box, _promptBackground);
                GUI.Label(box, CurrentPrompt, _promptStyle);
            }
        }

        private void Activate(ArpgRefugeStation31 station)
        {
            if (station == null || ArpgInterface30.Instance == null) return;

            switch (station.stationType)
            {
                case ArpgRefugeStationType31.SpellForge:
                    MarkObjective("refuge.spellforge");
                    ArpgInterface30.Instance.OpenPanel(ArpgPanel31.SpellForge);
                    break;
                case ArpgRefugeStationType31.MapDevice:
                    MarkObjective("refuge.map-device");
                    ArpgInterface30.Instance.OpenPanel(ArpgPanel31.MapDevice);
                    break;
                case ArpgRefugeStationType31.Stash:
                    ArpgInterface30.Instance.OpenPanel(ArpgPanel31.Stash);
                    break;
                case ArpgRefugeStationType31.Crafting:
                    ArpgInterface30.Instance.OpenPanel(ArpgPanel31.Crafting);
                    break;
                case ArpgRefugeStationType31.Constellations:
                    ArpgInterface30.Instance.OpenPanel(ArpgPanel31.Constellations);
                    break;
                case ArpgRefugeStationType31.Character:
                    ArpgInterface30.Instance.OpenPanel(ArpgPanel31.Character);
                    break;
                case ArpgRefugeStationType31.TrainingTarget:
                    ArpgInterface30.Instance.OpenPanel(ArpgPanel31.Training);
                    break;
                case ArpgRefugeStationType31.Arsenal:
                    if (ArpgArsenalUI32.Instance != null)
                        ArpgArsenalUI32.Instance.Open(ArpgArsenalTab32.Inventory);
                    break;
            }
        }

        private void MarkObjective(string flag)
        {
            ArpgProfile30 profile = ArpgFoundation30.Profile;
            if (profile == null) return;
            if (!profile.objectiveFlags.Contains(flag)) profile.objectiveFlags.Add(flag);
            if (profile.objectiveFlags.Contains("refuge.spellforge") &&
                profile.objectiveFlags.Contains("refuge.map-device"))
                profile.firstRefugeSequenceComplete = true;
            ArpgProfileStore30.Save(profile);
        }

        private void CreateEnvironment()
        {
            CreateStation(
                ArpgRefugeStationType31.SpellForge,
                "SPELLFORGE ALTAR",
                "Equip Cores, inspect Support Runes, and manage persistent discoveries.",
                new Vector3(-6.5f, 0.8f, 2.5f),
                PrimitiveType.Cube,
                new Color(0.26f, 0.62f, 1f),
                new Vector3(2.2f, 1.4f, 2.2f));

            CreateStation(
                ArpgRefugeStationType31.MapDevice,
                "MAP DEVICE",
                "Open owned maps or regenerate a free Tier 0 map.",
                new Vector3(0f, 0.65f, 7f),
                PrimitiveType.Cylinder,
                new Color(0.55f, 0.28f, 1f),
                new Vector3(2.8f, 0.65f, 2.8f));

            CreateStation(
                ArpgRefugeStationType31.Stash,
                "STASH",
                "Store persistent equipment outside the active inventory.",
                new Vector3(6.5f, 0.9f, 2.5f),
                PrimitiveType.Cube,
                new Color(0.2f, 0.85f, 0.72f),
                new Vector3(2.1f, 1.8f, 1.5f));

            CreateStation(
                ArpgRefugeStationType31.Crafting,
                "CRAFTING STATION",
                "Apply Flux Shards, Binding Seals, Sovereign Embers, Astral Needles, and Tempering Prisms.",
                new Vector3(7.2f, 0.75f, -3f),
                PrimitiveType.Cube,
                new Color(1f, 0.48f, 0.18f),
                new Vector3(2.4f, 1.2f, 1.8f));

            CreateStation(
                ArpgRefugeStationType31.Constellations,
                "CONSTELLATION ORRERY",
                "Allocate Stars, complete Constellations, and manage Attunement.",
                new Vector3(-7.2f, 1.15f, -3f),
                PrimitiveType.Sphere,
                new Color(0.85f, 0.72f, 0.25f),
                new Vector3(1.8f, 1.8f, 1.8f));

            CreateStation(
                ArpgRefugeStationType31.Character,
                "CHARACTER ARCHIVE",
                "Inspect equipment, progression, statistics, and discoveries.",
                new Vector3(0f, 0.9f, -6.5f),
                PrimitiveType.Capsule,
                new Color(0.4f, 0.78f, 1f),
                new Vector3(1.3f, 1.7f, 1.3f));

            CreateStation(
                ArpgRefugeStationType31.TrainingTarget,
                "TRAINING TARGET",
                "Test damage and inspect live combat telemetry.",
                new Vector3(10f, 1.1f, 7.5f),
                PrimitiveType.Capsule,
                new Color(0.9f, 0.26f, 0.24f),
                new Vector3(1.2f, 1.9f, 1.2f));

            CreateStation(
                ArpgRefugeStationType31.Arsenal,
                "ARSENAL & EXCHANGE",
                "Manage variable-size equipment, shared storage, crafting, filters, vendors, and salvage.",
                new Vector3(-10f, 1f, 7.5f),
                PrimitiveType.Cube,
                new Color(0.95f, 0.52f, 0.12f),
                new Vector3(2.4f, 2f, 1.8f));

            CreatePortalRing(new Vector3(0f, 0.15f, 10.5f));
        }

        private void CreateStation(
            ArpgRefugeStationType31 type,
            string name,
            string description,
            Vector3 position,
            PrimitiveType primitive,
            Color color,
            Vector3 scale)
        {
            GameObject stationObject = GameObject.CreatePrimitive(primitive);
            stationObject.name = name;
            stationObject.transform.SetParent(transform, false);
            stationObject.transform.position = position;
            stationObject.transform.localScale = scale;
            Material material = CreateMaterial(color, 2.2f);
            Renderer renderer = stationObject.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = material;

            ArpgRefugeStation31 station = stationObject.AddComponent<ArpgRefugeStation31>();
            station.stationType = type;
            station.displayName = name;
            station.description = description;
            _stations.Add(station);

            GameObject halo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            halo.name = name + " Halo";
            halo.transform.SetParent(stationObject.transform, false);
            halo.transform.localPosition = new Vector3(0f, -0.55f, 0f);
            halo.transform.localScale = new Vector3(1.4f, 0.08f, 1.4f);
            Renderer haloRenderer = halo.GetComponent<Renderer>();
            if (haloRenderer != null) haloRenderer.sharedMaterial = CreateMaterial(new Color(color.r, color.g, color.b, 0.55f), 4f);
            Collider haloCollider = halo.GetComponent<Collider>();
            if (haloCollider != null) Destroy(haloCollider);
        }

        private void CreatePortalRing(Vector3 position)
        {
            GameObject portal = new GameObject("Map Portal Anchor");
            portal.transform.SetParent(transform, false);
            portal.transform.position = position;
            for (int index = 0; index < 12; index++)
            {
                float angle = index / 12f * Mathf.PI * 2f;
                GameObject shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shard.name = "Portal Shard";
                shard.transform.SetParent(portal.transform, false);
                shard.transform.localPosition = new Vector3(Mathf.Cos(angle) * 2.2f, 1.8f + Mathf.Sin(angle) * 2.2f, 0f);
                shard.transform.localRotation = Quaternion.Euler(0f, 0f, -angle * Mathf.Rad2Deg);
                shard.transform.localScale = new Vector3(0.25f, 0.8f, 0.25f);
                Renderer renderer = shard.GetComponent<Renderer>();
                if (renderer != null) renderer.sharedMaterial = CreateMaterial(new Color(0.35f, 0.65f, 1f), 3f);
                Collider collider = shard.GetComponent<Collider>();
                if (collider != null) Destroy(collider);
            }
        }

        private void EnsureTrainingTarget()
        {
            if (_world == null || _world.Player == null) return;
            if (_world.Enemies.Any(value => value != null && value.gameObject.GetComponent<ArpgTrainingTarget31>() != null)) return;

            EnemyArchetype? training = null;
            foreach (EnemyArchetype value in Enum.GetValues(typeof(EnemyArchetype)))
            {
                if (value.ToString().IndexOf("training", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    training = value;
                    break;
                }
            }

            if (!training.HasValue) return;
            EnemyController controller = EnemyController.Spawn(
                new Vector3(10f, 0.75f, 7.5f), training.Value, 1, new DifficultySettings(), false, false);
            if (controller != null) controller.gameObject.AddComponent<ArpgTrainingTarget31>();
        }

        private void EnsureStyles()
        {
            if (_promptBackground != null) return;
            _promptBackground = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _promptBackground.SetPixel(0, 0, new Color(0.02f, 0.05f, 0.09f, 0.94f));
            _promptBackground.Apply();
            _promptBackground.hideFlags = HideFlags.HideAndDontSave;

            _promptStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.72f, 0.88f, 1f) }
            };
        }

        private Material CreateMaterial(Color color, float emission)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = "ArcaneEngine31_Refuge";
            material.color = color;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * emission);
            }
            material.hideFlags = HideFlags.HideAndDontSave;
            _materials.Add(material);
            return material;
        }
    }

    public sealed class ArpgTrainingTarget31 : MonoBehaviour
    {
        public static float TotalDamage { get; private set; }
        public static float DamagePerSecond { get; private set; }
        public static int HitEvents { get; private set; }
        public static string LastEvent { get; private set; } = "Awaiting damage.";

        private EnemyController _enemy;
        private float _lastHealth = -1f;
        private float _windowDamage;
        private float _windowStarted;

        private void Awake()
        {
            _enemy = GetComponent<EnemyController>();
            _windowStarted = Time.time;
            ResetMetrics();
        }

        public static void ResetMetrics()
        {
            TotalDamage = 0f;
            DamagePerSecond = 0f;
            HitEvents = 0;
            LastEvent = "Training target ready.";
        }

        private void Update()
        {
            if (_enemy == null) return;
            float health = ReadFloat(_enemy, "Health", "CurrentHealth", "health", "_health");
            if (health < 0f) return;
            if (_lastHealth < 0f)
            {
                _lastHealth = health;
                return;
            }

            if (health < _lastHealth)
            {
                float damage = _lastHealth - health;
                TotalDamage += damage;
                _windowDamage += damage;
                HitEvents++;
                LastEvent = damage.ToString("0.0") + " damage";
            }
            _lastHealth = health;

            float elapsed = Time.time - _windowStarted;
            if (elapsed >= 1f)
            {
                DamagePerSecond = _windowDamage / Mathf.Max(0.01f, elapsed);
                _windowDamage = 0f;
                _windowStarted = Time.time;
            }
        }

        private static float ReadFloat(object source, params string[] names)
        {
            if (source == null) return -1f;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type type = source.GetType();
            foreach (string name in names)
            {
                PropertyInfo property = type.GetProperty(name, flags);
                if (property != null && property.CanRead && property.PropertyType == typeof(float))
                    return (float)property.GetValue(source, null);
                FieldInfo field = type.GetField(name, flags);
                if (field != null && field.FieldType == typeof(float))
                    return (float)field.GetValue(source);
            }
            return -1f;
        }
    }
}
