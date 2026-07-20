using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ReactionDiagnosticsOverlay22 : MonoBehaviour
    {
        private bool _visible;
        private Vector2 _scroll;
        private GUIStyle _panel;
        private GUIStyle _title;
        private GUIStyle _line;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<ReactionDiagnosticsOverlay22>() != null)
                return;

            GameObject host = new GameObject("Patch 2.2 Propagation Diagnostics");
            DontDestroyOnLoad(host);
            host.hideFlags = HideFlags.DontSave;
            host.AddComponent<ReactionDiagnosticsOverlay22>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
                _visible = !_visible;
        }

        private void EnsureStyles()
        {
            if (_panel != null)
                return;

            _panel = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                padding = new RectOffset(14, 14, 12, 12)
            };
            _title = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            _line = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true
            };
        }

        private void OnGUI()
        {
            if (!_visible)
                return;

            EnsureStyles();
            Rect area = new Rect(18f, 18f, Mathf.Min(610f, Screen.width - 36f), Mathf.Min(720f, Screen.height - 36f));
            GUILayout.BeginArea(area, _panel);
            GUILayout.Label("Patch 2.2 — Reaction Propagation Diagnostics", _title);
            GUILayout.Label("F5 toggles this panel. Values are runtime counters for the current session.", _line);
            GUILayout.Space(8f);

            GUILayout.Label("Direct buildup applications: " + ReactionDiagnostics22.DirectApplications, _line);
            GUILayout.Label("Propagated buildup applications: " + ReactionDiagnostics22.PropagatedApplications, _line);
            GUILayout.Label("Recursive effects blocked: " + ReactionDiagnostics22.BlockedRecursiveEffects, _line);
            GUILayout.Label("Target revisits blocked: " + ReactionDiagnostics22.TargetRevisitsBlocked, _line);
            GUILayout.Label("Mechanics selected / discarded: " + ReactionDiagnostics22.SelectedMechanics + " / " + ReactionDiagnostics22.DiscardedMechanics, _line);
            GUILayout.Label("Presentation events coalesced: " + ReactionDiagnostics22.CoalescedPresentationEvents, _line);
            GUILayout.Label("Fields merged / rejected: " + ReactionDiagnostics22.FieldsMerged + " / " + ReactionDiagnostics22.FieldsRejected, _line);
            GUILayout.Label("Cascade reproduction ratio: " + ReactionDiagnostics22.ReproductionRatio.ToString("0.00"), _line);
            GUILayout.Label(
                "Active gameplay fields: " +
                (ElementalReactionField.Snapshot().Length +
                 PersistentSpellZone.Active.Count),
                _line);
            GUILayout.Space(8f);

            GUILayout.Label("Recent lineage decisions", _title);
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));
            string[] recent = ReactionDiagnostics22.Snapshot();
            for (int i = recent.Length - 1; i >= 0; i--)
                GUILayout.Label(recent[i], _line);
            GUILayout.EndScrollView();

            if (GUILayout.Button("Reset Diagnostics"))
                ReactionDiagnostics22.Reset();
            GUILayout.EndArea();
        }
    }
}
