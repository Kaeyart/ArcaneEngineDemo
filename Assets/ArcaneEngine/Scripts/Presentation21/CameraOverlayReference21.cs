using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    [RequireComponent(typeof(Camera))]
    public sealed class SpellCameraPost21 : MonoBehaviour
    {
        private static SpellCameraPost21 _cameraInstance;
        private static float _requestedImpulse;
        private static Vector3 _requestedDirection;
        private static float _impulseUntil;
        private static float _flashAmount;
        private static float _flashUntil;
        private Camera _camera;
        private Material _postMaterial;
        private Vector3 _appliedOffset;
        private float _noisePhase;

        public static float CurrentFlash { get { return _flashAmount; } }
        public static float CurrentImpulse { get { return _requestedImpulse; } }

        public static void RequestImpulse(Vector3 direction, float magnitude, float duration)
        {
            magnitude *= Patch200PresentationSettings.CameraFeedback;
            if (Patch200PresentationSettings.ReducedMotion) magnitude *= 0.25f;
            if (magnitude <= 0.001f) return;
            _requestedDirection = direction.sqrMagnitude < 0.01f ? Vector3.up : direction.normalized;
            _requestedImpulse = Mathf.Max(_requestedImpulse, Mathf.Clamp(magnitude, 0f, 0.5f));
            _impulseUntil = Mathf.Max(_impulseUntil, Time.unscaledTime + Mathf.Clamp(duration, 0.03f, 0.5f));
            EnsureCameraComponent();
        }

        public static void RequestFlash(float amount, float duration)
        {
            amount *= Patch210Settings21.FlashLimit;
            if (amount <= 0.005f) return;
            _flashAmount = Mathf.Max(_flashAmount, Mathf.Clamp01(amount));
            _flashUntil = Mathf.Max(_flashUntil, Time.unscaledTime + Mathf.Clamp(duration, 0.02f, 0.3f));
            EnsureCameraComponent();
        }

        private static void EnsureCameraComponent()
        {
            if (_cameraInstance != null) return;
            Camera camera = Camera.main;
            if (camera == null) return;
            _cameraInstance = camera.GetComponent<SpellCameraPost21>();
            if (_cameraInstance == null) _cameraInstance = camera.gameObject.AddComponent<SpellCameraPost21>();
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (_cameraInstance != null && _cameraInstance != this)
            {
                Destroy(this);
                return;
            }
            _cameraInstance = this;
            Shader shader = Shader.Find("Arcane/VFX21/PostBloom");
            if (shader != null)
            {
                _postMaterial = new Material(shader);
                _postMaterial.hideFlags = HideFlags.DontSave;
            }
        }

        private void LateUpdate()
        {
            if (_appliedOffset.sqrMagnitude > 0f)
            {
                transform.localPosition -= _appliedOffset;
                _appliedOffset = Vector3.zero;
            }

            if (Time.unscaledTime < _impulseUntil && _requestedImpulse > 0.001f)
            {
                _noisePhase += Time.unscaledDeltaTime * 43f;
                float remaining = Mathf.InverseLerp(_impulseUntil, _impulseUntil - 0.22f, Time.unscaledTime);
                Vector3 directional = transform.InverseTransformDirection(_requestedDirection) * -0.35f;
                Vector3 noise = new Vector3(
                    Mathf.PerlinNoise(_noisePhase, 0.13f) - 0.5f,
                    Mathf.PerlinNoise(0.51f, _noisePhase) - 0.5f,
                    Mathf.PerlinNoise(_noisePhase, 0.87f) - 0.5f);
                _appliedOffset = (directional + noise) * _requestedImpulse * remaining;
                transform.localPosition += _appliedOffset;
            }
            else
                _requestedImpulse = Mathf.MoveTowards(_requestedImpulse, 0f, Time.unscaledDeltaTime * 4f);

            if (Time.unscaledTime >= _flashUntil)
                _flashAmount = Mathf.MoveTowards(_flashAmount, 0f, Time.unscaledDeltaTime * 6f);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_postMaterial == null || (!Patch210Settings21.BloomEnabled && _flashAmount <= 0.001f))
            {
                Graphics.Blit(source, destination);
                return;
            }

            bool bloom = Patch210Settings21.BloomEnabled && Patch200PresentationSettings.Quality != PresentationQuality.Low;
            _postMaterial.SetFloat("_BloomEnabled", bloom ? 1f : 0f);
            _postMaterial.SetFloat("_BloomStrength", bloom
                ? Patch200PresentationSettings.Quality == PresentationQuality.High ? 0.36f : 0.19f
                : 0f);
            _postMaterial.SetFloat("_Threshold", 0.72f);
            _postMaterial.SetFloat("_Flash", Mathf.Clamp01(_flashAmount));
            _postMaterial.SetColor("_FlashColor", Color.white);
            Graphics.Blit(source, destination, _postMaterial, 0);
        }

        private void OnDestroy()
        {
            if (_cameraInstance == this) _cameraInstance = null;
            if (_postMaterial != null) Destroy(_postMaterial);
        }
    }

    public sealed class Patch210RuntimeOverlay21 : MonoBehaviour
    {
        private bool _visible;
        private Vector2 _scroll;
        private Rect _window = new Rect(18f, 18f, 560f, 650f);
        private GUIStyle _label;
        private GUIStyle _header;
        private GUIStyle _box;

        private void Update()
        {
            if (ArcaneInput.GetKeyDown(KeyCode.F6))
            {
                _visible = !_visible;
                Patch210Settings21.MorphologyOverlay = _visible;
            }
            if (Patch210Settings21.MorphologyOverlay != _visible)
                _visible = Patch210Settings21.MorphologyOverlay;
        }

        private void OnGUI()
        {
            if (!_visible) return;
            BuildStyles();
            _window = GUI.Window(2100, _window, DrawWindow, "Patch 2.1 · Spell Morphology Inspector");
        }

        private void DrawWindow(int id)
        {
            SpellVisualContract21 contract = SpellMorphologyPresentation21.LastContract;
            _scroll = GUILayout.BeginScrollView(_scroll);
            GUILayout.Label("F6 toggles this inspector. F7 runs the reference preview.", _label);
            GUILayout.Space(6f);
            DrawSettings();
            GUILayout.Space(8f);
            if (contract == null)
            {
                GUILayout.Label("No spell visual contract has been compiled in this session.", _label);
            }
            else
            {
                GUILayout.Label(contract.DebugSummary, _box);
                GUILayout.Space(6f);
                GUILayout.Label("Core invariants", _header);
                for (int i = 0; i < contract.coreInvariants.Count; i++)
                {
                    CoreIdentityInvariant21 invariant = contract.coreInvariants[i];
                    GUILayout.Label((invariant.required ? "REQUIRED · " : "OPTIONAL · ") + invariant.description, _label);
                }
                GUILayout.Space(6f);
                GUILayout.Label("Ordered Rune graph", _header);
                for (int i = 0; i < contract.runeGraph.Count; i++)
                {
                    RuneOperatorNode21 node = contract.runeGraph[i];
                    GUILayout.Label(node + "\n    " + node.deliveryImplementation + "\n    fallback: " + node.fallback, _label);
                }
                GUILayout.Space(6f);
                GUILayout.Label("Lifecycle", _header);
                for (int i = 0; i < contract.lifecycle.Count; i++)
                {
                    LifecycleNode21 phase = contract.lifecycle[i];
                    GUILayout.Label(
                        phase.phase + " · " + phase.source +
                        " · " + phase.normalizedStart.ToString("0.00") +
                        "–" + (phase.normalizedStart + phase.normalizedDuration).ToString("0.00"),
                        _label);
                }
                GUILayout.Space(6f);
                GUILayout.Label("Operator interactions", _header);
                if (contract.interactionRules.Count == 0) GUILayout.Label("No specialized compound rules required.", _label);
                for (int i = 0; i < contract.interactionRules.Count; i++) GUILayout.Label(contract.interactionRules[i], _label);
                GUILayout.Space(6f);
                GUILayout.Label("Runtime", _header);
                GUILayout.Label("Cached contracts: " + SpellMorphologyCompiler21.CachedContractCount, _label);
                GUILayout.Label("Audio voices: " + ProceduralSpellAudio21.ActiveVoices, _label);
                GUILayout.Label("Target responses: " + MorphologyPresentationDirector21.Instance.ActiveTargetResponses, _label);
                GUILayout.Label("Flash energy: " + MorphologyPresentationDirector21.Instance.FlashEnergy.ToString("0.00"), _label);
                GUILayout.Label("Camera impulse: " + SpellCameraPost21.CurrentImpulse.ToString("0.00"), _label);
            }
            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }

        private void DrawSettings()
        {
            GUILayout.Label("Quality and accessibility", _header);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Low")) Patch200PresentationSettings.Quality = PresentationQuality.Low;
            if (GUILayout.Button("Medium")) Patch200PresentationSettings.Quality = PresentationQuality.Medium;
            if (GUILayout.Button("High")) Patch200PresentationSettings.Quality = PresentationQuality.High;
            GUILayout.EndHorizontal();
            Patch210Settings21.BloomEnabled = GUILayout.Toggle(Patch210Settings21.BloomEnabled, "Controlled bloom");
            Patch210Settings21.DistortionEnabled = GUILayout.Toggle(Patch210Settings21.DistortionEnabled, "Controlled distortion");
            Patch210Settings21.TargetResponse = GUILayout.Toggle(Patch210Settings21.TargetResponse, "Local target response");
            Patch210Settings21.NearMiss = GUILayout.Toggle(Patch210Settings21.NearMiss, "Near-miss travel response");
            Patch200PresentationSettings.ReducedMotion = GUILayout.Toggle(Patch200PresentationSettings.ReducedMotion, "Reduced motion");
            GUILayout.Label("Flash limit " + Patch210Settings21.FlashLimit.ToString("0.00"), _label);
            Patch210Settings21.FlashLimit = GUILayout.HorizontalSlider(Patch210Settings21.FlashLimit, 0.05f, 1f);
        }

        private void BuildStyles()
        {
            if (_label != null) return;
            _label = new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 12 };
            _header = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
            _box = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, wordWrap = true, fontSize = 12, padding = new RectOffset(9, 9, 9, 9) };
        }
    }

    public sealed class Patch210ReferenceRuntime21 : MonoBehaviour
    {
        private readonly List<GameObject> _previews = new List<GameObject>();
        private int _generation;

        private void Update()
        {
            if (ArcaneInput.GetKeyDown(KeyCode.F7))
                BuildReferencePreview();
        }

        public void BuildReferencePreview()
        {
            Clear();
            GameWorld world = GameWorld.Instance;
            if (world == null || world.Player == null)
            {
                Debug.LogWarning("Patch 2.1 reference preview requires an active GameWorld and player.");
                return;
            }

            Vector3 center = world.Player.transform.position + world.Player.transform.forward * 4f;
            Vector3 right = world.Player.transform.right;
            int created = 0;
            for (int i = 0; i < 3; i++)
            {
                CompiledSpell spell = world.GetSpell((SpellSlot)i);
                if (spell == null) continue;
                SpellVisualContract21 contract = SpellMorphologyCompiler21.Get(spell);
                if (contract == null) continue;
                GameObject host = new GameObject("AE21 Reference " + i + " · " + contract.displayName);
                host.transform.position = center + right * (i - 1) * 2.6f + Vector3.up * 1.2f;
                SpellMorphologyOwner21 owner = host.AddComponent<SpellMorphologyOwner21>();
                owner.ownerKind = PresentationOwnerKind21.SceneEvent;
                owner.ownerId = host.GetEntityId().GetHashCode();
                owner.contractId = contract.contractId;
                MorphologyBodyBuilder21.Build(host.transform, contract, GeneratedSpellHostKind.Projectile, owner, new List<MorphologyBodyPart21>());
                PreviewOrbiter21 orbiter = host.AddComponent<PreviewOrbiter21>();
                orbiter.Initialize(center + right * (i - 1) * 2.6f + Vector3.up * 1.2f, contract, _generation);
                _previews.Add(host);
                created++;
            }
            _generation++;
            Debug.Log("Patch 2.1 reference preview created " + created + " generated spell bodies. Press F7 again to rebuild.");
        }

        private void Clear()
        {
            for (int i = 0; i < _previews.Count; i++) if (_previews[i] != null) Destroy(_previews[i]);
            _previews.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }

    public sealed class PreviewOrbiter21 : MonoBehaviour
    {
        private Vector3 _center;
        private SpellVisualContract21 _contract;
        private float _phase;
        private float _created;

        public void Initialize(Vector3 center, SpellVisualContract21 contract, int generation)
        {
            _center = center;
            _contract = contract;
            _phase = StableSeed21.Unit(StableSeed21.Combine(contract.seeds.body, generation)) * Mathf.PI * 2f;
            _created = Time.unscaledTime;
        }

        private void Update()
        {
            if (_contract == null) return;
            float age = Time.unscaledTime - _created;
            float radius = 0.35f + Mathf.Sin(age * 0.7f + _phase) * 0.08f;
            transform.position = _center + new Vector3(Mathf.Cos(age + _phase) * radius, Mathf.Sin(age * 1.3f + _phase) * 0.16f, Mathf.Sin(age + _phase) * radius);
            transform.Rotate(Vector3.up, Time.unscaledDeltaTime * 32f, Space.World);
        }
    }
}
