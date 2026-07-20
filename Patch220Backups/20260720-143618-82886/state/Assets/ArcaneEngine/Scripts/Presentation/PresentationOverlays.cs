using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class Patch200PresentationOverlay : MonoBehaviour
    {
        private bool _open;
        private Rect _window = new Rect(24f, 80f, 420f, 590f);

        private void Update()
        {
            if (ArcaneInput.GetKeyDown(KeyCode.F10))
                _open = !_open;
        }

        private void OnGUI()
        {
            if (!_open)
                return;

            _window = GUI.Window(
                20010,
                _window,
                DrawWindow,
                "Arcane Engine 2.0 Presentation Settings");
        }

        private void DrawWindow(int id)
        {
            GUILayout.Label("F10 closes this window.");
            GUILayout.Space(6f);

            GUILayout.Label("Quality");
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(
                Patch200PresentationSettings.Quality == PresentationQuality.Low,
                "Low",
                GUI.skin.button))
            {
                Patch200PresentationSettings.Quality = PresentationQuality.Low;
            }
            if (GUILayout.Toggle(
                Patch200PresentationSettings.Quality == PresentationQuality.Medium,
                "Medium",
                GUI.skin.button))
            {
                Patch200PresentationSettings.Quality = PresentationQuality.Medium;
            }
            if (GUILayout.Toggle(
                Patch200PresentationSettings.Quality == PresentationQuality.High,
                "High",
                GUI.skin.button))
            {
                Patch200PresentationSettings.Quality = PresentationQuality.High;
            }
            GUILayout.EndHorizontal();

            DrawSlider(
                "Effect density",
                Patch200PresentationSettings.Density,
                0.25f,
                1f,
                value => Patch200PresentationSettings.Density = value);

            DrawSlider(
                "Effect intensity",
                Patch200PresentationSettings.EffectIntensity,
                0.35f,
                1.25f,
                value => Patch200PresentationSettings.EffectIntensity = value);

            DrawSlider(
                "Camera feedback",
                Patch200PresentationSettings.CameraFeedback,
                0f,
                1f,
                value => Patch200PresentationSettings.CameraFeedback = value);

            DrawSlider(
                "Flash intensity",
                Patch200PresentationSettings.FlashIntensity,
                0f,
                1f,
                value => Patch200PresentationSettings.FlashIntensity = value);

            bool reducedMotion = GUILayout.Toggle(
                Patch200PresentationSettings.ReducedMotion,
                "Reduced motion");
            if (reducedMotion != Patch200PresentationSettings.ReducedMotion)
                Patch200PresentationSettings.ReducedMotion = reducedMotion;

            bool distortion = GUILayout.Toggle(
                Patch200PresentationSettings.Distortion,
                "Distortion where supported");
            if (distortion != Patch200PresentationSettings.Distortion)
                Patch200PresentationSettings.Distortion = distortion;

            GUILayout.Space(5f);
            GUILayout.Label("Blood presentation");
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(
                Patch200PresentationSettings.BloodMode == BloodPresentationMode.Hidden,
                "Hidden",
                GUI.skin.button))
            {
                Patch200PresentationSettings.BloodMode = BloodPresentationMode.Hidden;
            }
            if (GUILayout.Toggle(
                Patch200PresentationSettings.BloodMode == BloodPresentationMode.Reduced,
                "Reduced",
                GUI.skin.button))
            {
                Patch200PresentationSettings.BloodMode = BloodPresentationMode.Reduced;
            }
            if (GUILayout.Toggle(
                Patch200PresentationSettings.BloodMode == BloodPresentationMode.Full,
                "Full",
                GUI.skin.button))
            {
                Patch200PresentationSettings.BloodMode = BloodPresentationMode.Full;
            }
            GUILayout.EndHorizontal();

            bool diagnostics = GUILayout.Toggle(
                Patch200PresentationSettings.DebugOverlay,
                "Persistent diagnostics overlay");
            if (diagnostics != Patch200PresentationSettings.DebugOverlay)
                Patch200PresentationSettings.DebugOverlay = diagnostics;

            GUILayout.Space(8f);
            GUILayout.Label("Budgets");
            GUILayout.Label("Particle systems: " + Patch200PresentationSettings.MaxParticleSystems);
            GUILayout.Label("Estimated particles: " + Patch200PresentationSettings.MaxParticles);
            GUILayout.Label("Residues: " + Patch200PresentationSettings.MaxResidues);
            GUILayout.Label("Status attachments: " + Patch200PresentationSettings.MaxAttachments);
            GUILayout.Space(8f);
            GUILayout.Label("F8 runs the presentation stress test.");
            GUILayout.Label("F9 opens detailed diagnostics.");

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }

        private static void DrawSlider(
            string label,
            float value,
            float minimum,
            float maximum,
            System.Action<float> setter)
        {
            GUILayout.Label(label + ": " + value.ToString("0.00"));
            float updated = GUILayout.HorizontalSlider(value, minimum, maximum);
            if (Mathf.Abs(updated - value) > 0.001f)
                setter(updated);
        }
    }

    public sealed class Patch200PresentationDiagnosticsOverlay : MonoBehaviour
    {
        private bool _open;
        private Rect _window = new Rect(460f, 80f, 520f, 700f);
        private Vector2 _scroll;

        private void Update()
        {
            if (ArcaneInput.GetKeyDown(KeyCode.F9))
                _open = !_open;
        }

        private void OnGUI()
        {
            if (!_open && !Patch200PresentationSettings.DebugOverlay)
                return;

            _window = GUI.Window(
                20011,
                _window,
                DrawWindow,
                "Arcane Engine 2.0 Presentation Diagnostics");
        }

        private void DrawWindow(int id)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            SpellPresentationDirector2 director = SpellPresentationDirector2.Instance;
            GUILayout.Label("Runtime");
            GUILayout.Label("Published events: " + SpellPresentationBus.PublishedCount);
            GUILayout.Label("Processed events: " + (director == null ? 0 : director.ProcessedEvents));
            GUILayout.Label("Critical events: " + (director == null ? 0 : director.CriticalEvents));
            GUILayout.Label("Cached recipes: " + ProceduralSpellVisualCompiler2.CachedRecipeCount);
            GUILayout.Label("Active particle systems: " + PresentationParticlePool2.ActiveCount +
                            " / " + Patch200PresentationSettings.MaxParticleSystems);
            GUILayout.Label("Estimated particles: " + PresentationParticlePool2.EstimatedParticleCount +
                            " / " + Patch200PresentationSettings.MaxParticles);
            GUILayout.Label("Denied particle requests: " + PresentationParticlePool2.DeniedCount);
            GUILayout.Label("Geometry effects: " + PresentationGeometry2.ActiveCount);
            GUILayout.Label("Status attachments: " + ElementalStatusVisual2.ActiveAttachmentCount +
                            " / " + Patch200PresentationSettings.MaxAttachments);
            GUILayout.Label("Residues: " + PresentationResidue2.ActiveCount +
                            " / " + Patch200PresentationSettings.MaxResidues);
            GUILayout.Label("Procedural audio sources: " + ProceduralPresentationAudio2.ActiveSourceCount);

            GUILayout.Space(8f);
            GUILayout.Label("Last generated recipe");
            GeneratedVisualRecipe recipe = ProceduralSpellPresentation.LastRecipe;
            GUILayout.TextArea(recipe == null ? "No spell recipe generated yet." : recipe.DebugSummary);

            GUILayout.Space(8f);
            GUILayout.Label("Recent presentation events");
            SpellPresentationEvent[] events = SpellPresentationBus.Snapshot();
            int first = Mathf.Max(0, events.Length - 18);

            for (int i = first; i < events.Length; i++)
            {
                GUILayout.Label(
                    events[i].time.ToString("0.00") + " · " +
                    events[i].priority + " · " +
                    events[i].Summary);
            }

            GUILayout.Space(8f);
            if (GUILayout.Button("Clear event history"))
                SpellPresentationBus.Clear();

            if (GUILayout.Button("Clear pooled particles"))
                PresentationParticlePool2.ClearAll();

            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }
    }

    public sealed class Patch200PresentationStressRuntime : MonoBehaviour
    {
        private bool _running;
        private int _step;
        private float _next;
        private Vector3 _center;

        private void Update()
        {
            if (ArcaneInput.GetKeyDown(KeyCode.F8))
                Begin();

            if (!_running || Time.time < _next)
                return;

            RunStep();
        }

        private void Begin()
        {
            Transform player = GameWorld.Instance == null ||
                               GameWorld.Instance.Player == null
                ? null
                : GameWorld.Instance.Player.transform;

            _center = player == null ? Vector3.zero : player.position;
            _step = 0;
            _running = true;
            _next = Time.time;

            if (GameWorld.Instance != null)
                GameWorld.Instance.Log("Arcane Engine 2.0 presentation stress test started.");
        }

        private void RunStep()
        {
            ReactionElement[] signatures =
            {
                ReactionElement.Fire,
                ReactionElement.Cold,
                ReactionElement.Lightning,
                ReactionElement.Physical,
                ReactionElement.Blood,
                ReactionElement.Toxic,
                ReactionElement.Void,
                ReactionElement.Fire | ReactionElement.Toxic,
                ReactionElement.Cold | ReactionElement.Physical,
                ReactionElement.Lightning | ReactionElement.Void,
                ReactionElement.Fire | ReactionElement.Toxic | ReactionElement.Lightning,
                ReactionElement.Cold | ReactionElement.Blood | ReactionElement.Void,
                ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning | ReactionElement.Physical,
                ReactionElement.Fire | ReactionElement.Cold | ReactionElement.Lightning |
                    ReactionElement.Physical | ReactionElement.Blood |
                    ReactionElement.Toxic | ReactionElement.Void
            };

            if (_step >= signatures.Length)
            {
                _running = false;
                if (GameWorld.Instance != null)
                    GameWorld.Instance.Log("Arcane Engine 2.0 presentation stress test completed.");
                return;
            }

            ReactionElement signature = signatures[_step];
            float angle = _step / (float)signatures.Length * Mathf.PI * 2f;
            Vector3 position = _center + new Vector3(
                Mathf.Cos(angle) * 4f,
                0f,
                Mathf.Sin(angle) * 4f);
            int elements = Mathf.Max(1, ElementalReactionCodex.CountBits(signature));

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.ReactionResolved,
                priority = elements >= 6
                    ? PresentationPriority.Reserved
                    : elements >= 4 ? PresentationPriority.Critical : PresentationPriority.Important,
                position = position,
                signature = signature,
                catalyst = ElementalReactionCodex.PrimaryElement(signature),
                tier = elements == 7
                    ? ReactionTier.Apex
                    : elements == 6
                        ? ReactionTier.Calamity
                        : elements == 5
                            ? ReactionTier.Convergence
                            : elements == 4
                                ? ReactionTier.Catastrophe
                                : elements == 3 ? ReactionTier.Compound : ReactionTier.Fusion,
                radius = 1.6f + elements * 0.35f,
                duration = 2f + elements * 0.4f,
                intensity = 0.8f + elements * 0.13f,
                mechanicId = "stress:" + _step
            });

            ReactionMechanicType[] mechanics =
            {
                ReactionMechanicType.Burst,
                ReactionMechanicType.PulseNova,
                ReactionMechanicType.ChainArc,
                ReactionMechanicType.Pull,
                ReactionMechanicType.Push,
                ReactionMechanicType.Freeze,
                ReactionMechanicType.Field,
                ReactionMechanicType.SplitFields,
                ReactionMechanicType.DelayedEcho,
                ReactionMechanicType.OrbitingNodes,
                ReactionMechanicType.ShardNova,
                ReactionMechanicType.ThermalCycle,
                ReactionMechanicType.Contagion,
                ReactionMechanicType.Compression
            };

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.ReactionMechanic,
                priority = elements >= 4 ? PresentationPriority.Critical : PresentationPriority.Important,
                position = position,
                signature = signature,
                catalyst = ElementalReactionCodex.PrimaryElement(signature),
                mechanicType = mechanics[_step % mechanics.Length],
                mechanicId = "stress-mechanic:" + _step,
                radius = 2f + elements * 0.3f,
                duration = 1.5f,
                count = 3 + elements,
                intensity = 1f
            });

            _step++;
            _next = Time.time + 0.42f;
        }
    }
}
