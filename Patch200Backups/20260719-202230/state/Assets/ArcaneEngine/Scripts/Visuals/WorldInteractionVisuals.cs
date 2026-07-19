using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum WorldInteractionVisualState { Idle, Available, Focused, Unavailable, Selected, Completed }

    public sealed class InteractionStatePresenter : MonoBehaviour
    {
        private Color _baseColor;
        private LineRenderer _focusRing;
        private WorldInteractionVisualState _state;
        private Vector3 _baseScale;
        private Light _focusLight;

        public void Initialize(Color color)
        {
            _baseColor = color;
            _baseScale = transform.localScale;
            if (_focusRing == null)
            {
                _focusRing = RuntimeVisuals.Ring("Interaction State Ring", transform.position, color, 1.15f, 0.055f, transform);
                _focusRing.transform.localPosition = Vector3.up * 0.035f;
            }
            SetState(WorldInteractionVisualState.Available);
        }

        public void SetState(WorldInteractionVisualState state)
        {
            _state = state;
            if (_focusRing == null) return;
            Color color = state == WorldInteractionVisualState.Focused ? Color.Lerp(_baseColor, Color.white, 0.4f) :
                state == WorldInteractionVisualState.Unavailable || state == WorldInteractionVisualState.Completed ? new Color(0.28f, 0.31f, 0.35f) :
                state == WorldInteractionVisualState.Selected ? new Color(1f, 0.92f, 0.34f) : _baseColor;
            _focusRing.startColor = color;
            _focusRing.endColor = color;
            _focusRing.startWidth = _focusRing.endWidth = state == WorldInteractionVisualState.Focused || state == WorldInteractionVisualState.Selected ? 0.12f : 0.055f;
            _focusRing.enabled = state != WorldInteractionVisualState.Completed;
            if (state == WorldInteractionVisualState.Focused || state == WorldInteractionVisualState.Selected)
            {
                if (!OwnsFocusLight())
                {
                    _focusLight = null;
                    _focusLight = ProceduralVisualRuntime.LimitedLight(transform.position + Vector3.up * 0.8f, color, 3.2f, 0.42f, 999f, transform, state == WorldInteractionVisualState.Selected ? 4 : 3);
                }
            }
            else if (OwnsFocusLight())
            {
                ProceduralVisualRuntime.Release(_focusLight.gameObject);
                _focusLight = null;
            }
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || renderer is LineRenderer || renderer.GetComponent<TextMesh>() != null) continue;
                if (state == WorldInteractionVisualState.Unavailable || state == WorldInteractionVisualState.Completed)
                    renderer.sharedMaterial = RuntimeVisuals.Material(Color.Lerp(_baseColor, Color.gray, 0.72f), 0.05f);
            }
        }

        private void Update()
        {
            if (_focusRing == null) return;
            float motionScale = ProfileManager.Current != null && ProfileManager.Current.accessibility.reducedMotion ? 0.2f : 1f;
            float pulse = _state == WorldInteractionVisualState.Focused ? 1f + Mathf.Sin(Time.unscaledTime * 5f) * 0.08f * motionScale :
                _state == WorldInteractionVisualState.Selected ? 1.12f : 1f;
            _focusRing.transform.localScale = Vector3.one * pulse;
        }

        private void OnDestroy()
        {
            if (!OwnsFocusLight()) { _focusLight = null; return; }
            _focusLight.transform.SetParent(null, true);
            ProceduralVisualRuntime.Release(_focusLight.gameObject);
            _focusLight = null;
        }

        private bool OwnsFocusLight()
        {
            if (_focusLight == null || !_focusLight.gameObject.activeInHierarchy || _focusLight.transform.parent != transform) return false;
            PooledVisualMarker marker = _focusLight.GetComponent<PooledVisualMarker>();
            return marker != null && marker.InUse;
        }
    }

    public sealed class RewardRevealMotion : MonoBehaviour
    {
        private float _delay;
        private float _age;
        private Vector3 _targetScale;
        public void Initialize(float delay)
        {
            _delay = delay;
            _targetScale = transform.localScale;
            transform.localScale = Vector3.one * 0.05f;
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age < _delay) return;
            float t = Mathf.Clamp01((_age - _delay) / 0.38f);
            float overshoot = 1f + Mathf.Sin(t * Mathf.PI) * 0.14f;
            transform.localScale = Vector3.Lerp(Vector3.one * 0.05f, _targetScale * overshoot, Mathf.SmoothStep(0f, 1f, t));
            if (t >= 1f) { transform.localScale = _targetScale; enabled = false; }
        }
    }

    public static class RewardPresentationFlow
    {
        public static void Select(RoomRewardPickup selected)
        {
            IReadOnlyList<WorldRoomInteractable> interactions = WorldRoomInteractable.Active;
            for (int i = 0; i < interactions.Count; i++)
            {
                RoomRewardPickup reward = interactions[i] as RoomRewardPickup;
                if (reward == null) continue;
                reward.SetVisualState(reward == selected ? WorldInteractionVisualState.Selected : WorldInteractionVisualState.Unavailable);
                if (reward == selected)
                {
                    ProceduralVisualRuntime.Ring("Selected Reward Confirmation", reward.transform.position, new Color(1f, 0.92f, 0.32f), 1.45f, 0.16f, 0.32f);
                    ProceduralVisualRuntime.LimitedLight(reward.transform.position + Vector3.up, new Color(1f, 0.82f, 0.3f), 4f, 1.2f, 0.22f, null, 4);
                }
            }
        }
    }

    public static class RoomDoorStateVisuals
    {
        private static GameObject _lockedRoot;

        public static void SpawnLocked(Color accent)
        {
            ClearLocked();
            _lockedRoot = new GameObject("Locked Route Doors");
            for (int door = 0; door < 3; door++)
            {
                float x = Mathf.Lerp(-9f, 9f, door / 2f);
                GameObject root = new GameObject("Locked Door " + (door + 1));
                root.transform.SetParent(_lockedRoot.transform, false);
                root.transform.position = new Vector3(x, 0f, 14.25f);
                Color locked = Color.Lerp(accent, new Color(0.32f, 0.12f, 0.18f), 0.65f);
                for (int side = -1; side <= 1; side += 2)
                {
                    GameObject post = RuntimeVisuals.Primitive("Locked Door Post", PrimitiveType.Cube, root.transform.position,
                        new Vector3(0.28f, 3.2f, 0.35f), locked, root.transform);
                    RuntimeVisuals.RemoveCollider(post); post.transform.localPosition = new Vector3(side * 1.25f, 1.6f, 0f);
                }
                GameObject lintel = RuntimeVisuals.Primitive("Locked Door Lintel", PrimitiveType.Cube, root.transform.position,
                    new Vector3(2.5f, 0.28f, 0.35f), locked, root.transform);
                RuntimeVisuals.RemoveCollider(lintel); lintel.transform.localPosition = new Vector3(0f, 3.2f, 0f);
                for (int bar = -1; bar <= 1; bar++)
                {
                    GameObject seal = RuntimeVisuals.Primitive("Encounter Seal Bar", PrimitiveType.Cube, root.transform.position,
                        new Vector3(2.1f, 0.08f, 0.12f), Color.Lerp(locked, Color.white, 0.18f), root.transform);
                    RuntimeVisuals.RemoveCollider(seal); seal.transform.localPosition = new Vector3(0f, 1.55f + bar * 0.5f, -0.08f); seal.transform.localRotation = Quaternion.Euler(0f, 0f, bar * 18f);
                }
                RuntimeVisuals.Ring("Locked Door Seal", root.transform.position, locked, 0.72f, 0.09f, root.transform).transform.localPosition = new Vector3(0f, 1.6f, -0.16f);
            }
        }

        public static void ClearLocked()
        {
            if (_lockedRoot == null) return;
            Object.Destroy(_lockedRoot);
            _lockedRoot = null;
        }
    }

    public sealed class RoomTransitionCurtain : MonoBehaviour
    {
        private static RoomTransitionCurtain _instance;
        private float _started;
        private float _duration;

        public static void Play(float duration = 0.42f)
        {
            if (_instance == null)
            {
                GameObject root = new GameObject("Room Transition Curtain");
                Object.DontDestroyOnLoad(root);
                _instance = root.AddComponent<RoomTransitionCurtain>();
            }
            _instance._started = Time.unscaledTime;
            _instance._duration = Mathf.Max(0.18f, duration);
            _instance.enabled = true;
        }

        private void OnGUI()
        {
            float t = Mathf.Clamp01((Time.unscaledTime - _started) / _duration);
            float alpha = Mathf.Sin(t * Mathf.PI) * 0.92f;
            Color previous = GUI.color;
            GUI.color = new Color(0.015f, 0.02f, 0.04f, alpha);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = previous;
            if (t >= 1f) enabled = false;
        }
    }

    public static class HubStationVisualBuilder
    {
        public static void Build(Transform root, HubStationType type, Color color)
        {
            if (root == null) return;
            switch (type)
            {
                case HubStationType.StartRun: Gate(root, color); break;
                case HubStationType.Loadout: Loadout(root, color); break;
                case HubStationType.SpellArchive: Archive(root, color); break;
                case HubStationType.Armory: Armory(root, color); break;
                case HubStationType.Forge: Forge(root, color); break;
                case HubStationType.Upgrades: Shrine(root, color); break;
                case HubStationType.Collection: Collection(root, color); break;
                case HubStationType.TrainingOptions: Training(root, color); break;
                case HubStationType.Tutorial: Tutorial(root, color); break;
                case HubStationType.SaveProfile: Memory(root, color); break;
            }
            RuntimeVisuals.Ring(type + " Function Ring", root.position, color, 1.4f, 0.11f, root).transform.localPosition = Vector3.zero;
            PriorityLightAnchor.Attach(root.gameObject, color, 3.6f, 0.55f, type == HubStationType.StartRun ? 4 : 2);
        }

        private static GameObject Part(Transform root, string name, PrimitiveType shape, Vector3 local, Vector3 scale, Color color)
        {
            GameObject part = RuntimeVisuals.Primitive(name, shape, root.position, scale, color, root);
            RuntimeVisuals.RemoveCollider(part); part.transform.localPosition = local; return part;
        }
        private static void Gate(Transform r, Color c) { Part(r, "Dungeon Gate Left", PrimitiveType.Cube, new Vector3(-0.9f, 1.35f, 0f), new Vector3(0.35f, 2.7f, 0.45f), c); Part(r, "Dungeon Gate Right", PrimitiveType.Cube, new Vector3(0.9f, 1.35f, 0f), new Vector3(0.35f, 2.7f, 0.45f), c); Part(r, "Dungeon Gate Keystone", PrimitiveType.Cube, new Vector3(0f, 2.65f, 0f), new Vector3(1.8f, 0.35f, 0.45f), c); }
        private static void Loadout(Transform r, Color c) { for (int i = -1; i <= 1; i++) Part(r, "Loadout Spell Socket " + i, PrimitiveType.Cylinder, new Vector3(i * 0.65f, 0.7f, 0f), new Vector3(0.42f, 0.18f, 0.42f), c); Part(r, "Loadout Rack", PrimitiveType.Cube, new Vector3(0f, 1.3f, 0.35f), new Vector3(1.45f, 0.18f, 0.25f), c); }
        private static void Archive(Transform r, Color c) { for (int i = -1; i <= 1; i++) Part(r, "Spell Archive Tablet", PrimitiveType.Cube, new Vector3(i * 0.55f, 1f + Mathf.Abs(i) * 0.2f, 0f), new Vector3(0.38f, 0.65f, 0.12f), c); }
        private static void Armory(Transform r, Color c) { Part(r, "Armory Torso", PrimitiveType.Capsule, new Vector3(0f, 1.15f, 0f), new Vector3(0.68f, 1f, 0.42f), c); for (int i = -1; i <= 1; i += 2) Part(r, "Armory Weapon", PrimitiveType.Cube, new Vector3(i * 0.85f, 1.1f, 0f), new Vector3(0.12f, 1.2f, 0.12f), c); }
        private static void Forge(Transform r, Color c) { Part(r, "Forge Anvil", PrimitiveType.Cube, new Vector3(0f, 0.65f, 0f), new Vector3(1.5f, 0.55f, 0.75f), c); Part(r, "Forge Flame", PrimitiveType.Capsule, new Vector3(0f, 1.45f, 0f), new Vector3(0.28f, 0.62f, 0.28f), Color.Lerp(c, Color.white, 0.2f)); }
        private static void Shrine(Transform r, Color c) { for (int i = 0; i < 5; i++) Part(r, "Upgrade Branch " + i, PrimitiveType.Capsule, new Vector3(Mathf.Cos(i / 5f * Mathf.PI * 2f) * 0.8f, 0.8f + i * 0.15f, Mathf.Sin(i / 5f * Mathf.PI * 2f) * 0.8f), new Vector3(0.15f, 0.55f, 0.15f), c); }
        private static void Collection(Transform r, Color c) { Part(r, "Bestiary Pedestal", PrimitiveType.Cylinder, new Vector3(0f, 0.45f, 0f), new Vector3(1.15f, 0.32f, 1.15f), c); Part(r, "Bestiary Specimen", PrimitiveType.Sphere, new Vector3(0f, 1.25f, 0f), Vector3.one * 0.55f, c); }
        private static void Training(Transform r, Color c) { Part(r, "Training Dummy", PrimitiveType.Capsule, new Vector3(0f, 1.05f, 0f), new Vector3(0.58f, 1f, 0.58f), c); Part(r, "Options Dial", PrimitiveType.Cylinder, new Vector3(0.9f, 0.8f, 0f), new Vector3(0.42f, 0.08f, 0.42f), c); }
        private static void Tutorial(Transform r, Color c) { for (int i = 0; i < 3; i++) Part(r, "Apprentice Step " + (i + 1), PrimitiveType.Cube, new Vector3((i - 1) * 0.55f, 0.35f + i * 0.42f, 0f), Vector3.one * 0.36f, c); }
        private static void Memory(Transform r, Color c) { Part(r, "Memory Crystal", PrimitiveType.Cube, new Vector3(0f, 1.1f, 0f), Vector3.one * 0.72f, c).transform.localRotation = Quaternion.Euler(35f, 45f, 35f); for (int i = 0; i < 3; i++) Part(r, "Memory Orbit " + i, PrimitiveType.Sphere, new Vector3(Mathf.Cos(i / 3f * Mathf.PI * 2f) * 0.9f, 0.9f, Mathf.Sin(i / 3f * Mathf.PI * 2f) * 0.9f), Vector3.one * 0.16f, c); }
    }
}
