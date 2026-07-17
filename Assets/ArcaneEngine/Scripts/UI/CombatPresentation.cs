using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    /// <summary>
    /// Runtime combat presentation kept separate from damage simulation. This provides
    /// readable aim feedback, enemy health bars, boss health, and damage numbers without
    /// making any of those visuals responsible for registering a hit.
    /// </summary>
    public sealed class CombatPresentation : MonoBehaviour
    {
        private sealed class DamageNumber
        {
            public Vector3 position;
            public float amount;
            public Color color;
            public bool critical;
            public float created;
        }

        public static CombatPresentation Instance { get; private set; }

        private readonly List<DamageNumber> _numbers = new List<DamageNumber>();
        private Texture2D _white;
        private GUIStyle _healthText;
        private GUIStyle _damageText;
        private GUIStyle _criticalText;
        private GUIStyle _bossText;

        private void Awake()
        {
            Instance = this;
            _white = Texture2D.whiteTexture;
        }

        public static void ShowDamage(Vector3 position, float amount, Color color, bool critical)
        {
            if (Instance == null || amount <= 0.01f || !ProfileManager.Current.accessibility.showDamageNumbers) return;
            Instance._numbers.Add(new DamageNumber
            {
                position = position + Vector3.up * 1.15f,
                amount = amount,
                color = color,
                critical = critical,
                created = Time.unscaledTime
            });
            if (Instance._numbers.Count > 80) Instance._numbers.RemoveAt(0);
        }

        private void Update()
        {
            for (int i = _numbers.Count - 1; i >= 0; i--)
                if (Time.unscaledTime - _numbers[i].created > 1.05f) _numbers.RemoveAt(i);
        }

        private void OnGUI()
        {
            GameWorld world = GameWorld.Instance;
            Camera camera = Camera.main;
            if (world == null || camera == null || !world.RunActive || world.ModalOpen) return;
            EnsureStyles();
            DrawAim(world, camera);
            DrawEnemyBars(world, camera);
            DrawDamageNumbers(camera);
        }

        private void DrawAim(GameWorld world, Camera camera)
        {
            if (world.ModalOpen || world.Player == null) return;
            Vector3 screen = camera.WorldToScreenPoint(world.Player.AimPoint + Vector3.up * 0.08f);
            if (screen.z <= 0f) return;
            float x = screen.x;
            float y = Screen.height - screen.y;
            Color previous = GUI.color;
            GUI.color = new Color(0.3f, 0.95f, 1f, 0.95f);
            GUI.DrawTexture(new Rect(x - 10f, y - 1f, 7f, 2f), _white);
            GUI.DrawTexture(new Rect(x + 3f, y - 1f, 7f, 2f), _white);
            GUI.DrawTexture(new Rect(x - 1f, y - 10f, 2f, 7f), _white);
            GUI.DrawTexture(new Rect(x - 1f, y + 3f, 2f, 7f), _white);
            GUI.color = previous;
        }

        private void DrawEnemyBars(GameWorld world, Camera camera)
        {
            EnemyController boss = null;
            foreach (EnemyController enemy in world.Enemies.ToArray())
            {
                if (enemy == null || enemy.IsDead) continue;
                if (enemy.IsBoss) { boss = enemy; continue; }
                if (!ProfileManager.Current.accessibility.alwaysShowEnemyHealth && enemy.Health >= enemy.MaxHealth) continue;
                DrawWorldBar(camera, enemy);
            }
            if (boss != null) DrawBossBar(boss);
        }

        private void DrawWorldBar(Camera camera, EnemyController enemy)
        {
            Vector3 screen = camera.WorldToScreenPoint(enemy.HealthBarWorldPosition);
            if (screen.z <= 0f) return;
            float width = enemy.IsEliteOrBoss ? 116f : 88f;
            float height = enemy.IsEliteOrBoss ? 11f : 8f;
            Rect rect = new Rect(screen.x - width * 0.5f, Screen.height - screen.y, width, height);
            DrawFilledBar(rect, enemy.HealthRatio, new Color(0.9f, 0.15f, 0.18f), new Color(0.06f, 0.065f, 0.085f, 0.95f));
            if (enemy.ShieldRatio > 0f)
            {
                Rect shield = new Rect(rect.x, rect.y - 4f, rect.width, 3f);
                DrawFilledBar(shield, enemy.ShieldRatio, new Color(0.15f, 0.75f, 1f), Color.black);
            }
            if (enemy.IsEliteOrBoss)
                GUI.Label(new Rect(rect.x - 20f, rect.y - 19f, rect.width + 40f, 18f), enemy.DisplayName, _healthText);
            if (!string.IsNullOrEmpty(enemy.StatusSummary))
                GUI.Label(new Rect(rect.x - 30f, rect.yMax + 1f, rect.width + 60f, 17f), enemy.StatusSummary, _healthText);
        }

        private void DrawBossBar(EnemyController boss)
        {
            float width = Mathf.Min(680f, Screen.width - 80f);
            Rect rect = new Rect((Screen.width - width) * 0.5f, 72f, width, 20f);
            GUI.Label(new Rect(rect.x, rect.y - 27f, rect.width, 24f), boss.DisplayName, _bossText);
            DrawFilledBar(rect, boss.HealthRatio, new Color(0.88f, 0.08f, 0.16f), new Color(0.045f, 0.035f, 0.055f, 0.98f));
            if (boss.ShieldRatio > 0f)
                DrawFilledBar(new Rect(rect.x, rect.yMax + 3f, rect.width, 5f), boss.ShieldRatio, new Color(0.2f, 0.75f, 1f), Color.black);
            GUI.Label(rect, Mathf.CeilToInt(boss.Health) + " / " + Mathf.CeilToInt(boss.MaxHealth), _healthText);
        }

        private void DrawDamageNumbers(Camera camera)
        {
            foreach (DamageNumber number in _numbers)
            {
                float age = Time.unscaledTime - number.created;
                Vector3 world = number.position + Vector3.up * age * 0.9f;
                Vector3 screen = camera.WorldToScreenPoint(world);
                if (screen.z <= 0f) continue;
                Color previous = GUI.color;
                GUI.color = new Color(number.color.r, number.color.g, number.color.b, 1f - Mathf.Clamp01(age / 1.05f));
                GUI.Label(new Rect(screen.x - 55f, Screen.height - screen.y - 18f, 110f, 34f),
                    Mathf.Max(1, Mathf.RoundToInt(number.amount)).ToString(), number.critical ? _criticalText : _damageText);
                GUI.color = previous;
            }
        }

        private void DrawFilledBar(Rect rect, float ratio, Color fill, Color background)
        {
            Color previous = GUI.color;
            GUI.color = background;
            GUI.DrawTexture(rect, _white);
            GUI.color = fill;
            GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, Mathf.Max(0f, (rect.width - 4f) * Mathf.Clamp01(ratio)), Mathf.Max(1f, rect.height - 4f)), _white);
            GUI.color = previous;
        }

        private void EnsureStyles()
        {
            if (_healthText != null) return;
            _healthText = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            _damageText = new GUIStyle(_healthText) { fontSize = 17 };
            _criticalText = new GUIStyle(_healthText) { fontSize = 24, fontStyle = FontStyle.Bold };
            _bossText = new GUIStyle(_healthText) { fontSize = 17, fontStyle = FontStyle.Bold };
        }
    }
}
