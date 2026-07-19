using UnityEngine;

namespace ArcaneEngine
{
    public sealed class RunResultVisualSequence : MonoBehaviour
    {
        private static RunResultVisualSequence _instance;
        private bool _victory;
        private string _title;
        private string _secured;
        private string _lost;
        private string _details;
        private float _until;
        private GUIStyle _panel;
        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;

        public static void Show(RunDirector run)
        {
            if (run == null) return;
            if (_instance == null)
            {
                GameObject root = new GameObject("Run Result Visual Sequence");
                Object.DontDestroyOnLoad(root);
                _instance = root.AddComponent<RunResultVisualSequence>();
            }
            _instance._victory = run.LastRunVictory;
            _instance._title = run.LastRunVictory ? "EXTRACTION COMPLETE" : "EXPEDITION LOST";
            _instance._secured = "KEPT · " + run.LastEssenceAward + " Essence" + (run.LastRunVictory ? " · " + run.LastRunItemCount + " equipment · " +
                run.LastRunSpellCopies + " Spell Copies · " + run.LastRunForgeDust + " Dust · " + run.LastRunBindingRunes + " Runes · " + run.LastRunCorruptionCores + " Cores" : string.Empty);
            _instance._lost = "LOST · " + run.LastRunGold + " Gold" + (run.LastRunVictory ? " · " + run.LastRunSupportRunes + " dungeon Support Runes" :
                " · " + run.LastRunItemCount + " equipment · " + run.LastRunSpellCopies + " Spell Copies · " + run.LastRunSupportRunes + " Support Runes · " +
                run.LastRunForgeDust + " Dust · " + run.LastRunBindingRunes + " Binding Runes · " + run.LastRunCorruptionCores + " Corruption Cores");
            string items = run.LastRunItemNames == null || run.LastRunItemNames.Count == 0 ? "No equipment" : string.Join(", ", run.LastRunItemNames);
            string spells = run.LastRunSpellNames == null || run.LastRunSpellNames.Count == 0 ? "No Spell Copies" : string.Join(", ", run.LastRunSpellNames);
            _instance._details = (run.LastRunVictory ? "SECURED CONTENTS · " : "LOST CONTENTS · ") + items + " · " + spells;
            _instance._until = Time.unscaledTime + 5f;
            _instance.enabled = true;

            Color color = run.LastRunVictory ? new Color(0.18f, 1f, 0.68f) : new Color(1f, 0.12f, 0.24f);
            Vector3 center = GameWorld.Instance == null || GameWorld.Instance.Player == null ? Vector3.zero : GameWorld.Instance.Player.transform.position;
            for (int i = 0; i < 4; i++) ProceduralVisualRuntime.Ring("Run Result Layer " + i, center, color, 1.2f + i * 0.65f, 0.08f + i * 0.025f, 0.65f + i * 0.1f);
            ProceduralVisualRuntime.LimitedLight(center + Vector3.up, color, 7f, 1.4f, 0.65f, null, 5);
            int visibleItems = run.LastRunItems == null ? 0 : run.LastRunItems.Count;
            for (int i = 0; i < visibleItems; i++)
            {
                int ringIndex = i / 8;
                int ringCount = Mathf.Min(8, visibleItems - ringIndex * 8);
                float angle = (i % 8) / (float)Mathf.Max(1, ringCount) * Mathf.PI * 2f;
                float itemRadius = 3.2f + ringIndex * 1.45f;
                RewardVisualSystem.ShowRunResultItem(run.LastRunItems[i], center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * itemRadius, run.LastRunVictory);
            }
            RewardVisualSystem.ShowRunResultResource("Essence", run.LastEssenceAward, center + new Vector3(-4.8f, 0f, -2.4f), true, new Color(0.3f, 1f, 0.85f));
            RewardVisualSystem.ShowRunResultResource("Gold", run.LastRunGold, center + new Vector3(4.8f, 0f, -2.4f), false, new Color(1f, 0.78f, 0.18f));
            RewardVisualSystem.ShowRunResultResource("Forge Dust", run.LastRunForgeDust, center + new Vector3(-4.8f, 0f, 2.4f), run.LastRunVictory, new Color(0.72f, 0.76f, 0.82f));
            RewardVisualSystem.ShowRunResultResource("Binding Runes", run.LastRunBindingRunes, center + new Vector3(0f, 0f, 4.8f), run.LastRunVictory, new Color(0.32f, 0.88f, 1f));
            RewardVisualSystem.ShowRunResultResource("Corruption Cores", run.LastRunCorruptionCores, center + new Vector3(4.8f, 0f, 2.4f), run.LastRunVictory, new Color(0.92f, 0.08f, 0.32f));
            RewardVisualSystem.ShowRunResultResource("Dungeon Support Runes", run.LastRunSupportRunes, center + new Vector3(0f, 0f, -4.8f), false, new Color(0.68f, 0.42f, 1f));
        }

        private void OnGUI()
        {
            if (Time.unscaledTime >= _until) { enabled = false; return; }
            if (_panel == null)
            {
                _panel = new GUIStyle(GUI.skin.box); _panel.padding = new RectOffset(22, 22, 18, 18);
                _titleStyle = new GUIStyle(GUI.skin.label); _titleStyle.fontSize = 25; _titleStyle.fontStyle = FontStyle.Bold; _titleStyle.alignment = TextAnchor.MiddleCenter;
                _bodyStyle = new GUIStyle(GUI.skin.label); _bodyStyle.fontSize = 14; _bodyStyle.alignment = TextAnchor.MiddleCenter; _bodyStyle.wordWrap = true;
            }
            float width = Mathf.Min(920f, Screen.width - 40f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, 24f, width, 170f);
            GUI.Box(panel, GUIContent.none, _panel);
            _titleStyle.normal.textColor = _victory ? new Color(0.25f, 1f, 0.72f) : new Color(1f, 0.22f, 0.32f);
            _bodyStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(panel.x + 12f, panel.y + 10f, panel.width - 24f, 34f), _title, _titleStyle);
            GUI.Label(new Rect(panel.x + 16f, panel.y + 48f, panel.width - 32f, 30f), _secured, _bodyStyle);
            _bodyStyle.normal.textColor = new Color(1f, 0.62f, 0.5f);
            GUI.Label(new Rect(panel.x + 16f, panel.y + 82f, panel.width - 32f, 42f), _lost, _bodyStyle);
            _bodyStyle.normal.textColor = new Color(0.8f, 0.88f, 0.96f);
            GUI.Label(new Rect(panel.x + 16f, panel.y + 124f, panel.width - 32f, 38f), _details, _bodyStyle);
        }
    }
}
