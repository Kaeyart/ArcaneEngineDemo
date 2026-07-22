using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ArpgMonsterIdentity31 : MonoBehaviour
    {
        public string familyId;
        public string variantId;
        public int tier;
        public bool elite;

        private EnemyController _enemy;
        private float _nextAbility;
        private bool _deathTriggered;

        private void Awake()
        {
            _enemy = GetComponent<EnemyController>();
            _nextAbility = Time.time + UnityEngine.Random.Range(3.5f, 6.5f);
        }

        private void Start()
        {
            ApplyFamilyPresentation();
        }

        private void Update()
        {
            if (_enemy == null) return;
            if (_enemy.IsDead)
            {
                if (!_deathTriggered)
                {
                    _deathTriggered = true;
                    if (familyId == "family.ashbound")
                        ArpgHazard31.Spawn(transform.position, 2.8f, 0.65f, 9f + tier * 1.2f, new Color(1f, 0.24f, 0.08f), "Ashbound Detonation");
                }
                return;
            }

            if (Time.time < _nextAbility || ArpgFoundation30.Instance == null || !ArpgFoundation30.Instance.MapActive) return;
            ArpgMonsterVariantDefinition31 variant = ArpgContent30.MonsterVariant(variantId);
            float speed = variant == null ? 1f : Mathf.Max(0.5f, variant.speedMultiplier);
            _nextAbility = Time.time + UnityEngine.Random.Range(elite ? 3f : 5f, elite ? 5f : 8f) / speed;
            ExecuteVariantAbility(variant);
        }

        private void ExecuteVariantAbility(ArpgMonsterVariantDefinition31 variant)
        {
            GameWorld world = GameWorld.Instance;
            Vector3 player = world != null && world.Player != null ? world.Player.transform.position : transform.position;
            string role = variant == null ? string.Empty : variant.role;
            string ability = variant == null ? "Family Ability" : variant.signatureAbility;
            float damageScale = variant == null ? 1f : Mathf.Max(0.6f, variant.damageMultiplier);

            if (familyId == "family.ashbound")
            {
                if (role == "Ranged" || role == "Support")
                    ArpgHazard31.Spawn(player, elite ? 2.8f : 2.1f, 0.9f, (7f + tier) * damageScale, new Color(1f, 0.28f, 0.05f), ability);
                else
                    ArpgHazard31.Spawn(transform.position, elite ? 3.2f : 2.5f, 0.75f, (8f + tier) * damageScale, new Color(1f, 0.18f, 0.04f), ability);
            }
            else if (familyId == "family.mirekin")
            {
                Vector3 center = role == "Ranged" ? player : transform.position;
                ArpgHazard31.Spawn(center, elite ? 3.2f : 2.45f, 1.15f, (6f + tier * 0.8f) * damageScale, new Color(0.32f, 0.78f, 0.16f), ability);
            }
            else if (familyId == "family.astral-constructs")
            {
                ArpgHazard31.Spawn(player, elite ? 2.8f : 2.1f, role == "Controller" ? 1.2f : 0.8f, (7f + tier * 0.9f) * damageScale, new Color(0.28f, 0.64f, 1f), ability);
            }
        }

        private void ApplyFamilyPresentation()
        {
            Color color = familyId == "family.ashbound"
                ? new Color(1f, 0.24f, 0.08f)
                : familyId == "family.mirekin"
                    ? new Color(0.24f, 0.76f, 0.18f)
                    : new Color(0.25f, 0.58f, 1f);

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                if (renderer == null) continue;
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(block);
                block.SetColor("_Color", color);
                block.SetColor("_BaseColor", color);
                block.SetColor("_EmissionColor", color * (elite ? 1.8f : 0.7f));
                renderer.SetPropertyBlock(block);
            }

            ArpgMonsterVariantDefinition31 variant = ArpgContent30.MonsterVariant(variantId);
            float healthScale = variant == null ? 1f : Mathf.Clamp(variant.healthMultiplier, 0.7f, 1.7f);
            transform.localScale *= (elite ? 1.13f : 1f) * Mathf.Lerp(0.9f, 1.14f, Mathf.InverseLerp(0.7f, 1.7f, healthScale));
            if (variant != null) gameObject.name = variant.displayName + (elite ? " · Rare" : string.Empty);
        }
    }

    public sealed class ArpgBossDirector31 : MonoBehaviour
    {
        public string bossId;
        public string familyId;
        public int tier;
        public bool awakened;

        private EnemyController _enemy;
        private float _nextAttack;
        private int _phase;
        private float _lastKnownPercent = 1f;

        private void Awake()
        {
            _enemy = GetComponent<EnemyController>();
            _nextAttack = Time.time + 2.5f;
        }

        private void Start()
        {
            transform.localScale *= 1.18f;
            ApplyBossPresentation();
        }

        private void Update()
        {
            if (_enemy == null || _enemy.IsDead || ArpgFoundation30.Instance == null || !ArpgFoundation30.Instance.MapActive) return;

            float percent = ReadHealthPercent(_enemy);
            if (percent >= 0f) _lastKnownPercent = percent;
            int newPhase = _lastKnownPercent <= 0.34f ? 2 : _lastKnownPercent <= 0.67f ? 1 : 0;
            if (newPhase > _phase)
            {
                _phase = newPhase;
                TriggerPhaseTransition();
            }

            if (Time.time < _nextAttack) return;
            float interval = Mathf.Max(1.6f, 4.7f - _phase * 0.75f - (awakened ? 0.55f : 0f));
            _nextAttack = Time.time + interval;
            ExecutePattern();
        }

        private void ExecutePattern()
        {
            GameWorld world = GameWorld.Instance;
            if (world == null || world.Player == null) return;
            Vector3 player = world.Player.transform.position;
            Vector3 origin = transform.position;
            int pattern = (Mathf.FloorToInt(Time.time * 0.5f) + _phase + tier) % 3;

            if (bossId == "boss.ember-warden")
            {
                if (pattern == 0) Ring(origin, 4, 2.6f, 1.1f, 11f, new Color(1f, 0.2f, 0.05f), "Ignition Ring");
                else ArpgHazard31.Spawn(player, 3f + _phase * 0.5f, 0.8f, 12f + tier, new Color(1f, 0.35f, 0.05f), "Ashfall");
            }
            else if (bossId == "boss.frostbound-matron")
            {
                if (pattern == 0) Line(origin, player, 5, 2f, 1.1f, 10f + tier, new Color(0.4f, 0.8f, 1f), "Frost Wave");
                else Ring(player, 3 + _phase, 2f, 1f, 10f + tier, new Color(0.55f, 0.9f, 1f), "Frozen Mirrors");
            }
            else if (bossId == "boss.stormcoil-behemoth")
            {
                Ring(player, 3 + _phase, 2.8f, 0.65f, 12f + tier, new Color(0.25f, 0.65f, 1f), "Thunder Mark");
            }
            else if (bossId == "boss.bone-regent")
            {
                if (pattern == 0) Line(origin, player, 6, 1.7f, 0.8f, 11f + tier, new Color(0.9f, 0.84f, 0.72f), "Bone Lance");
                else SpawnRegentAdds();
            }
            else if (bossId == "boss.mireheart")
            {
                Ring(origin, 5 + _phase, 2.3f, 1.1f, 9f + tier, new Color(0.3f, 0.82f, 0.15f), "Toxic Bloom");
                ArpgHazard31.Spawn(player, 3.1f, 1.35f, 8f + tier, new Color(0.6f, 0.08f, 0.18f), "Leeching Roots");
            }
            else
            {
                if (pattern == 0) Line(origin, player, 7, 1.6f, 0.75f, 13f + tier, new Color(0.58f, 0.28f, 1f), "Void Beam");
                else Ring(origin, 4 + _phase, 3.1f, 0.9f, 12f + tier, new Color(0.35f, 0.7f, 1f), "Orbiting Sigils");
            }
        }

        private void TriggerPhaseTransition()
        {
            Color color = bossId == "boss.mireheart"
                ? new Color(0.3f, 0.9f, 0.16f)
                : bossId == "boss.ember-warden"
                    ? new Color(1f, 0.22f, 0.05f)
                    : new Color(0.45f, 0.7f, 1f);
            Ring(transform.position, 6 + _phase * 2, 1.8f, 1.25f, 9f + tier, color, "Guardian Phase " + (_phase + 1));
            if (ArpgInterface30.Instance != null)
            {
                ArpgBossDefinition31 definition = ArpgContent30.Boss(bossId);
                ArpgInterface30.Instance.SetMessage((definition == null ? "Guardian" : definition.displayName) + " entered phase " + (_phase + 1) + ".");
            }
        }

        private void SpawnRegentAdds()
        {
            GameWorld world = GameWorld.Instance;
            if (world == null) return;
            Array values = Enum.GetValues(typeof(EnemyArchetype));
            List<EnemyArchetype> pool = new List<EnemyArchetype>();
            foreach (EnemyArchetype value in values)
            {
                string name = value.ToString().ToLowerInvariant();
                if (name.Contains("training") || name.Contains("boss") || name.Contains("titan") || name.Contains("warden")) continue;
                pool.Add(value);
            }
            if (pool.Count == 0) return;

            for (int index = 0; index < 2 + _phase; index++)
            {
                float angle = index / (float)Mathf.Max(1, 2 + _phase) * Mathf.PI * 2f;
                Vector3 position = transform.position + new Vector3(Mathf.Cos(angle) * 3f, 0f, Mathf.Sin(angle) * 3f);
                EnemyController spawned = EnemyController.Spawn(
                    position, pool[(index + tier) % pool.Count], Mathf.Max(1, tier + 1), new DifficultySettings(), false, false);
                if (spawned != null) ArpgEncounter31.ConfigureMonster(spawned, familyId, tier, false);
            }
        }

        private void Ring(Vector3 center, int count, float radius, float delay, float damage, Color color, string label)
        {
            for (int index = 0; index < count; index++)
            {
                float angle = index / (float)Mathf.Max(1, count) * Mathf.PI * 2f;
                Vector3 point = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                ArpgHazard31.Spawn(point, 1.35f + _phase * 0.12f, delay + index * 0.06f, damage, color, label);
            }
        }

        private void Line(Vector3 start, Vector3 end, int count, float spacing, float delay, float damage, Color color, string label)
        {
            Vector3 direction = (end - start);
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) direction = Vector3.forward;
            direction.Normalize();
            for (int index = 1; index <= count; index++)
                ArpgHazard31.Spawn(start + direction * spacing * index, 1.25f, delay + index * 0.05f, damage, color, label);
        }

        private void ApplyBossPresentation()
        {
            Color color = bossId == "boss.ember-warden"
                ? new Color(1f, 0.22f, 0.05f)
                : bossId == "boss.frostbound-matron"
                    ? new Color(0.45f, 0.82f, 1f)
                    : bossId == "boss.stormcoil-behemoth"
                        ? new Color(0.2f, 0.55f, 1f)
                        : bossId == "boss.bone-regent"
                            ? new Color(0.95f, 0.84f, 0.68f)
                            : bossId == "boss.mireheart"
                                ? new Color(0.3f, 0.82f, 0.16f)
                                : new Color(0.55f, 0.25f, 1f);

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                if (renderer == null) continue;
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(block);
                block.SetColor("_Color", color);
                block.SetColor("_BaseColor", color);
                block.SetColor("_EmissionColor", color * 2f);
                renderer.SetPropertyBlock(block);
            }
        }

        private static float ReadHealthPercent(object source)
        {
            float health = ReadFloat(source, "Health", "CurrentHealth", "health", "_health");
            float maximum = ReadFloat(source, "MaxHealth", "MaximumHealth", "maxHealth", "_maxHealth");
            if (health < 0f || maximum <= 0f) return -1f;
            return Mathf.Clamp01(health / maximum);
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

    public sealed class ArpgHazard31 : MonoBehaviour
    {
        public float radius;
        public float delay;
        public float damage;
        public Color color;
        public string label;

        private float _spawned;
        private bool _resolved;
        private Renderer _renderer;
        private Material _material;

        public static void Spawn(Vector3 position, float radius, float delay, float damage, Color color, string label)
        {
            GameObject hazard = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hazard.name = "Hazard · " + label;
            hazard.transform.position = new Vector3(position.x, 0.08f, position.z);
            hazard.transform.localScale = new Vector3(radius * 2f, 0.06f, radius * 2f);
            Collider collider = hazard.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.Destroy(collider);
            ArpgHazard31 component = hazard.AddComponent<ArpgHazard31>();
            component.radius = radius;
            component.delay = Mathf.Max(0.1f, delay);
            component.damage = Mathf.Max(0f, damage);
            component.color = color;
            component.label = label;
        }

        private void Start()
        {
            _spawned = Time.time;
            _renderer = GetComponent<Renderer>();
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            if (shader == null) shader = Shader.Find("Standard");
            _material = new Material(shader);
            _material.name = "ArcaneEngine31_Hazard";
            _material.color = new Color(color.r, color.g, color.b, 0.25f);
            if (_material.HasProperty("_BaseColor")) _material.SetColor("_BaseColor", _material.color);
            if (_renderer != null) _renderer.sharedMaterial = _material;
        }

        private void Update()
        {
            float progress = Mathf.Clamp01((Time.time - _spawned) / Mathf.Max(0.1f, delay));
            transform.localScale = new Vector3(radius * 2f * Mathf.Lerp(0.35f, 1f, progress), 0.06f, radius * 2f * Mathf.Lerp(0.35f, 1f, progress));
            Color current = Color.Lerp(new Color(color.r, color.g, color.b, 0.18f), new Color(color.r, color.g, color.b, 0.82f), progress);
            if (_material != null)
            {
                _material.color = current;
                if (_material.HasProperty("_BaseColor")) _material.SetColor("_BaseColor", current);
            }

            if (!_resolved && progress >= 1f)
            {
                _resolved = true;
                GameWorld world = GameWorld.Instance;
                if (world != null && world.Player != null)
                {
                    Vector3 delta = world.Player.transform.position - transform.position;
                    delta.y = 0f;
                    if (delta.magnitude <= radius) world.Player.TakeDamage(damage);
                }
                Destroy(gameObject, 0.18f);
            }
        }

        private void OnDestroy()
        {
            if (_material != null) Destroy(_material);
        }
    }

    public static class ArpgEncounter31
    {
        public static void ConfigureMonster(EnemyController enemy, string familyId, int tier, bool elite)
        {
            List<ArpgMonsterVariantDefinition31> variants = ArpgContent30.MonsterVariants.Where(value => value.familyId == familyId).ToList();
            string variantId = variants.Count == 0 ? string.Empty : variants[ArpgDeterminism30.Index(tier + (elite ? 17 : 0), variants.Count)].id;
            ConfigureMonster(enemy, familyId, variantId, tier, elite);
        }

        public static void ConfigureMonster(EnemyController enemy, string familyId, string variantId, int tier, bool elite)
        {
            if (enemy == null) return;
            ArpgMonsterIdentity31 identity = enemy.GetComponent<ArpgMonsterIdentity31>();
            if (identity == null) identity = enemy.gameObject.AddComponent<ArpgMonsterIdentity31>();
            identity.familyId = familyId;
            identity.variantId = variantId;
            identity.tier = tier;
            identity.elite = elite;
        }

        public static void ConfigureBoss(EnemyController enemy, ArpgBossDefinition31 boss, int tier, bool awakened)
        {
            if (enemy == null || boss == null) return;
            ArpgBossDirector31 director = enemy.GetComponent<ArpgBossDirector31>();
            if (director == null) director = enemy.gameObject.AddComponent<ArpgBossDirector31>();
            director.bossId = boss.id;
            director.familyId = boss.familyId;
            director.tier = tier;
            director.awakened = awakened;
        }
    }
}
