using UnityEngine;

namespace ArcaneEngine
{
    /// <summary>
    /// Lightweight combat presentation for aim feedback only.
    /// All health bars, boss cards, and damage numbers are handled by ModernCombatHUD (UI Toolkit).
    /// This class provides the ShowDamage API for backward compatibility and renders the aim crosshair.
    /// </summary>
    public sealed class CombatPresentation : MonoBehaviour
    {
        public static CombatPresentation Instance { get; private set; }

        private Texture2D _white;
        private GUIStyle _aimStyle;

        private void Awake()
        {
            Instance = this;
            _white = Texture2D.whiteTexture;
        }

        /// <summary>
        /// Backward-compatible damage number dispatch — forwards to ModernCombatHUD.
        /// </summary>
        public static void ShowDamage(Vector3 position, float amount, Color color, bool critical)
        {
            if (amount <= 0.01f || !ProfileManager.Current.accessibility.showDamageNumbers) return;
            ModernCombatHUD.ShowDamage(position, amount, color, critical);
        }

        private void OnGUI()
        {
            GameWorld world = GameWorld.Instance;
            Camera camera = Camera.main;
            if (world == null || camera == null || !world.RunActive || world.ModalOpen) return;
            EnsureStyles();
            DrawAim(world, camera);
        }

        private void DrawAim(GameWorld world, Camera camera)
        {
            if (world.Player == null) return;
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

        private void EnsureStyles()
        {
            if (_aimStyle != null) return;
            _aimStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
        }
    }
}