using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    [DefaultExecutionOrder(1200)]
    public sealed class V11EnemyHealthBarSystem : MonoBehaviour
    {
        private sealed class Bar
        {
            public GameObject root;
            public Transform health;
            public Transform shield;
            public TextMesh label;
            public EnemyController enemy;

            public void Bind(EnemyController value)
            {
                enemy = value;
                root.name = "Enemy Health Bar · " + value.DisplayName;
                root.SetActive(true);
            }

            public void Release()
            {
                enemy = null;
                root.SetActive(false);
            }
        }

        public static V11EnemyHealthBarSystem Instance { get; private set; }
        private readonly Dictionary<EnemyController, Bar> _active = new Dictionary<EnemyController, Bar>();
        private readonly Stack<Bar> _pool = new Stack<Bar>();
        private readonly List<EnemyController> _stale = new List<EnemyController>();
        private float _nextSync;

        private void Awake()
        {
            Instance = this;
            for (int i = 0; i < 24; i++) _pool.Push(CreateBar(i));
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextSync) return;
            _nextSync = Time.unscaledTime + 0.08f;
            Synchronize();
        }

        private void LateUpdate()
        {
            GameWorld world = GameWorld.Instance;
            Camera camera = Camera.main;
            if (world == null || camera == null) return;
            bool runVisible = world.RunActive;
            AccessibilitySettings settings = ProfileManager.Current == null ? null : ProfileManager.Current.accessibility;
            bool always = settings == null || settings.alwaysShowEnemyHealth;
            foreach (KeyValuePair<EnemyController, Bar> pair in _active)
            {
                EnemyController enemy = pair.Key;
                Bar bar = pair.Value;
                if (enemy == null || enemy.IsDead) { bar.root.SetActive(false); continue; }
                bool visible = runVisible && !enemy.IsBoss && (always || enemy.Health < enemy.MaxHealth || enemy.IsEliteOrBoss);
                if (bar.root.activeSelf != visible) bar.root.SetActive(visible);
                if (!visible) continue;
                bar.root.transform.position = enemy.HealthBarWorldPosition + Vector3.up * (settings == null ? 0f : settings.enemyHealthBarVerticalOffset);
                bar.root.transform.rotation = camera.transform.rotation;
                float scale = (enemy.IsEliteOrBoss ? 1.18f : 1f) * (settings == null ? 1f : settings.enemyHealthBarScale);
                bar.root.transform.localScale = Vector3.one * scale;
                float healthRatio = enemy.HealthRatio;
                bar.health.localScale = new Vector3(1.82f * healthRatio, 0.105f, 0.055f);
                bar.health.localPosition = new Vector3(-0.91f * (1f - healthRatio), 0f, -0.045f);
                float shieldRatio = enemy.ShieldRatio;
                bar.shield.gameObject.SetActive(shieldRatio > 0.001f);
                if (shieldRatio > 0.001f)
                {
                    bar.shield.localScale = new Vector3(1.82f * shieldRatio, 0.045f, 0.06f);
                    bar.shield.localPosition = new Vector3(-0.91f * (1f - shieldRatio), 0.16f, -0.05f);
                }
                string status = enemy.StatusSummary;
                string values = settings != null && settings.showEnemyHealthNumbers
                    ? Mathf.CeilToInt(enemy.Health) + " / " + Mathf.CeilToInt(enemy.MaxHealth)
                    : string.Empty;
                bar.label.text = (enemy.IsEliteOrBoss ? enemy.DisplayName.ToUpperInvariant() + "\n" : string.Empty) + values +
                    (string.IsNullOrEmpty(status) ? string.Empty : (string.IsNullOrEmpty(values) ? string.Empty : "  ·  ") + status);
            }
        }

        private void Synchronize()
        {
            GameWorld world = GameWorld.Instance;
            if (world == null) return;
            _stale.Clear();
            foreach (KeyValuePair<EnemyController, Bar> pair in _active)
                if (pair.Key == null || pair.Key.IsDead || !world.Enemies.Contains(pair.Key)) _stale.Add(pair.Key);
            for (int i = 0; i < _stale.Count; i++)
            {
                EnemyController enemy = _stale[i];
                Bar bar;
                if (!_active.TryGetValue(enemy, out bar)) continue;
                _active.Remove(enemy);
                bar.Release();
                _pool.Push(bar);
            }
            for (int i = 0; i < world.Enemies.Count; i++)
            {
                EnemyController enemy = world.Enemies[i];
                if (enemy == null || enemy.IsDead || enemy.IsBoss || _active.ContainsKey(enemy)) continue;
                Bar bar = _pool.Count > 0 ? _pool.Pop() : CreateBar(_active.Count + 24);
                bar.Bind(enemy);
                _active[enemy] = bar;
            }
        }

        private Bar CreateBar(int index)
        {
            GameObject root = new GameObject("Pooled Enemy Health Bar " + index);
            root.transform.SetParent(transform, false);
            GameObject background = RuntimeVisuals.Primitive("Background", PrimitiveType.Cube, Vector3.zero,
                new Vector3(1.94f, 0.15f, 0.045f), new Color(0.015f, 0.02f, 0.03f, 0.98f), root.transform);
            GameObject health = RuntimeVisuals.Primitive("Health", PrimitiveType.Cube, Vector3.zero,
                new Vector3(1.82f, 0.105f, 0.055f), new Color(0.92f, 0.08f, 0.14f), root.transform);
            GameObject shield = RuntimeVisuals.Primitive("Shield", PrimitiveType.Cube, Vector3.zero,
                new Vector3(1.82f, 0.045f, 0.06f), new Color(0.12f, 0.72f, 1f), root.transform);
            RuntimeVisuals.RemoveCollider(background); RuntimeVisuals.RemoveCollider(health); RuntimeVisuals.RemoveCollider(shield);
            background.transform.localPosition = Vector3.zero;
            health.transform.localPosition = new Vector3(0f, 0f, -0.045f);
            shield.transform.localPosition = new Vector3(0f, 0.16f, -0.05f);
            GameObject labelObject = new GameObject("Name and Status");
            labelObject.transform.SetParent(root.transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 0.23f, -0.06f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.anchor = TextAnchor.LowerCenter;
            label.alignment = TextAlignment.Center;
            label.fontSize = 44;
            label.characterSize = 0.031f;
            label.fontStyle = FontStyle.Bold;
            label.color = Color.white;
            MeshRenderer textRenderer = labelObject.GetComponent<MeshRenderer>();
            if (textRenderer != null) textRenderer.sortingOrder = 80;
            root.SetActive(false);
            return new Bar { root = root, health = health.transform, shield = shield.transform, label = label };
        }
    }
}
