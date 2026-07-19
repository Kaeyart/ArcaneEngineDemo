using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public abstract class WorldRoomInteractable : MonoBehaviour
    {
        private static readonly List<WorldRoomInteractable> ActiveInteractions = new List<WorldRoomInteractable>();
        public static IReadOnlyList<WorldRoomInteractable> Active { get { return ActiveInteractions; } }

        public string PromptTitle { get; protected set; }
        public string PromptDescription { get; protected set; }
        public string PromptAction { get; protected set; }
        public float InteractionRadius { get; protected set; } = 2.8f;
        public virtual bool SupportsHoldInteraction { get { return false; } }

        private TextMesh _label;
        private InteractionStatePresenter _statePresenter;
        private bool _visualRegistryActive;

        public Vector3 PromptWorldPosition { get { return transform.position + Vector3.up * 2.25f; } }

        protected virtual void OnEnable()
        {
            if (!ActiveInteractions.Contains(this)) ActiveInteractions.Add(this);
            if (!_visualRegistryActive) { VisualRuntimeRegistry.Register(VisualRuntimeKind.InteractionPresentation); _visualRegistryActive = true; }
        }

        protected virtual void OnDisable()
        {
            ActiveInteractions.Remove(this);
            if (_visualRegistryActive) { VisualRuntimeRegistry.Unregister(VisualRuntimeKind.InteractionPresentation); _visualRegistryActive = false; }
        }

        protected void CreateLabel(Color color)
        {
            GameObject labelObject = new GameObject("World Tooltip");
            labelObject.transform.SetParent(transform, false);
            labelObject.transform.localPosition = Vector3.up * 2.1f;
            _label = labelObject.AddComponent<TextMesh>();
            _label.anchor = TextAnchor.LowerCenter;
            _label.alignment = TextAlignment.Center;
            _label.fontSize = 52;
            _label.characterSize = 0.045f;
            _label.fontStyle = FontStyle.Bold;
            _label.color = color;
            MeshRenderer renderer = labelObject.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.sortingOrder = 40;
            _statePresenter = GetComponent<InteractionStatePresenter>();
            if (_statePresenter == null) _statePresenter = gameObject.AddComponent<InteractionStatePresenter>();
            _statePresenter.Initialize(color);
            RefreshLabel(false);
        }

        protected void SetLabelVerticalOffset(float height)
        {
            if (_label != null) _label.transform.localPosition = Vector3.up * height;
        }

        public void RefreshLabel(bool focused)
        {
            if (_label == null) return;
            string key = ProfileManager.Current == null ? "E" : ProfileManager.Current.controls.interact.ToString().ToUpperInvariant();
            _label.text = PromptTitle + "\n" + (focused ? "[ " + key + " ]  " + PromptAction : PromptAction);
            _label.characterSize = focused ? 0.052f : 0.043f;
            if (_statePresenter != null) _statePresenter.SetState(focused ? WorldInteractionVisualState.Focused : WorldInteractionVisualState.Available);
        }

        public void SetVisualState(WorldInteractionVisualState state)
        {
            if (_statePresenter != null) _statePresenter.SetState(state);
        }

        protected virtual void LateUpdate()
        {
            if (_label == null || Camera.main == null) return;
            _label.transform.rotation = Camera.main.transform.rotation;
        }

        public abstract void Interact();
        public virtual void HoldInteract() { Interact(); }
    }

    public sealed class WorldInteractionController : MonoBehaviour
    {
        public static WorldInteractionController Instance { get; private set; }
        public WorldRoomInteractable Current { get; private set; }
        private float _holdStarted;
        private bool _holding;
        private bool _holdTriggered;

        private void Awake() { Instance = this; }

        private void Update()
        {
            GameWorld world = GameWorld.Instance;
            WorldRoomInteractable previous = Current;
            Current = null;
            if (world != null && (world.RunActive || HomeBaseController.IsHomeActive) && world.Player != null && !world.ModalOpen)
            {
                Vector3 player = world.Player.transform.position;
                float closest = float.MaxValue;
                IReadOnlyList<WorldRoomInteractable> interactions = WorldRoomInteractable.Active;
                for (int i = 0; i < interactions.Count; i++)
                {
                    WorldRoomInteractable candidate = interactions[i];
                    if (candidate == null || !candidate.gameObject.activeInHierarchy) continue;
                    LootPickup loot = candidate as LootPickup;
                    if (loot != null && loot.HiddenByFilter) continue;
                    Vector3 delta = candidate.transform.position - player;
                    delta.y = 0f;
                    float distance = delta.sqrMagnitude;
                    float pickupBonus = world.Stats == null ? 0f : world.Stats.pickupRange;
                    float effectiveRadius = candidate.InteractionRadius + pickupBonus;
                    if (distance > effectiveRadius * effectiveRadius || distance >= closest) continue;
                    closest = distance;
                    Current = candidate;
                }
            }

            if (previous != null && previous != Current)
            {
                previous.RefreshLabel(false); _holding = false; _holdTriggered = false;
            }
            if (Current != null)
            {
                Current.RefreshLabel(true);
                KeyCode interact = ProfileManager.Current == null ? KeyCode.E : ProfileManager.Current.controls.interact;
                if (!Current.SupportsHoldInteraction)
                {
                    if (ArcaneInput.GetKeyDown(interact)) Current.Interact();
                }
                else
                {
                    if (ArcaneInput.GetKeyDown(interact)) { _holding = true; _holdTriggered = false; _holdStarted = Time.unscaledTime; }
                    float interactionSpeed = world.Stats == null ? 0f : world.Stats.interactionSpeed;
                    float holdDuration = 0.55f / (1f + Mathf.Max(0f, interactionSpeed));
                    if (_holding && !_holdTriggered && ArcaneInput.GetKey(interact) && Time.unscaledTime - _holdStarted >= holdDuration)
                    {
                        _holdTriggered = true; Current.HoldInteract();
                    }
                    if (_holding && ArcaneInput.GetKeyUp(interact))
                    {
                        if (!_holdTriggered && Current != null) Current.Interact();
                        _holding = false; _holdTriggered = false;
                    }
                }
            }
            else { _holding = false; _holdTriggered = false; }
        }

        public static void ClearRoomInteractions()
        {
            for (int i = WorldRoomInteractable.Active.Count - 1; i >= 0; i--)
            {
                WorldRoomInteractable interaction = WorldRoomInteractable.Active[i];
                if (interaction == null) continue;
                interaction.gameObject.SetActive(false);
                Destroy(interaction.gameObject);
            }
            if (Instance != null) Instance.Current = null;
        }

        public static void ClearType<T>() where T : WorldRoomInteractable
        {
            for (int i = WorldRoomInteractable.Active.Count - 1; i >= 0; i--)
            {
                T interaction = WorldRoomInteractable.Active[i] as T;
                if (interaction == null) continue;
                interaction.gameObject.SetActive(false);
                Destroy(interaction.gameObject);
            }
        }
    }

    public sealed class RoomRewardPickup : WorldRoomInteractable
    {
        private RunDirector _run;
        private int _choiceIndex;

        public static void SpawnChoices(RunDirector run, IList<RewardOffer> rewards)
        {
            WorldInteractionController.ClearType<RoomRewardPickup>();
            WorldInteractionController.ClearType<RewardRerollStation>();
            int count = rewards == null ? 0 : rewards.Count;
            for (int i = 0; i < count; i++)
            {
                RewardOffer reward = rewards[i];
                float x = (i - (count - 1) * 0.5f) * 3.6f;
                GameObject root = new GameObject("Room Reward · " + reward.title);
                root.transform.position = new Vector3(x, 0.15f, 1.25f);
                RoomRewardPickup pickup = root.AddComponent<RoomRewardPickup>();
                pickup._run = run;
                pickup._choiceIndex = i;
                pickup.PromptTitle = reward.title.ToUpperInvariant();
                pickup.PromptDescription = reward.description;
                pickup.PromptAction = "TAKE REWARD";
                PrimitiveType shape = reward.category == RewardCategory.Equipment ? PrimitiveType.Capsule :
                    reward.category == RewardCategory.SpellCore ? PrimitiveType.Cylinder : PrimitiveType.Cube;
                GameObject visual = RuntimeVisuals.Primitive("Reward", shape, root.transform.position + Vector3.up * 0.75f,
                    reward.category == RewardCategory.Equipment ? new Vector3(0.7f, 0.95f, 0.7f) : Vector3.one * 0.78f, reward.color, root.transform);
                RuntimeVisuals.RemoveCollider(visual);
                visual.transform.localPosition = Vector3.up * 0.75f;
                LineRenderer ring = RuntimeVisuals.Ring("Reward Ring", root.transform.position, reward.color, 0.9f, 0.09f, root.transform);
                ring.transform.localPosition = Vector3.zero;
                pickup.CreateLabel(Color.Lerp(Color.white, reward.color, 0.3f));
                RewardVisualSystem.AttachOffer(root.transform, reward);
                root.AddComponent<RewardRevealMotion>().Initialize(i * 0.08f);
            }
            if (run != null && run.RewardRerollsRemaining > 0) RewardRerollStation.Create(run);
        }

        private void Update()
        {
            transform.Rotate(0f, 18f * Time.deltaTime, 0f, Space.World);
        }

        public override void Interact()
        {
            if (_run == null || !_run.PendingRewards) return;
            RewardPresentationFlow.Select(this);
            _run.ChooseReward(_choiceIndex);
        }
    }

    public sealed class RoomDoor : WorldRoomInteractable
    {
        private RunDirector _run;
        private int _choiceIndex;

        public static void SpawnChoices(RunDirector run, IList<RoomTemplate> rooms)
        {
            WorldInteractionController.ClearType<RoomDoor>();
            RoomDoorStateVisuals.ClearLocked();
            int count = rooms == null ? 0 : rooms.Count;
            for (int i = 0; i < count; i++)
            {
                RoomTemplate room = rooms[i];
                float x = count == 1 ? 0f : Mathf.Lerp(-9f, 9f, i / (float)(count - 1));
                GameObject root = new GameObject("Door · " + room.displayName);
                root.transform.position = new Vector3(x, 0f, 14.25f);
                RoomDoor door = root.AddComponent<RoomDoor>();
                door._run = run;
                door._choiceIndex = i;
                door.InteractionRadius = 3.3f;
                door.PromptTitle = RoomIcon(room.type) + "  " + room.displayName.ToUpperInvariant();
                door.PromptDescription = FriendlyType(room.type) + " · Difficulty " + room.difficulty + (room.hasHazards ? " · Hazards" : string.Empty);
                door.PromptAction = "ENTER ROOM";

                Color color = room.accentColor;
                GameObject left = RuntimeVisuals.Primitive("Door Frame L", PrimitiveType.Cube, root.transform.position, new Vector3(0.3f, 3.4f, 0.35f), color, root.transform);
                GameObject right = RuntimeVisuals.Primitive("Door Frame R", PrimitiveType.Cube, root.transform.position, new Vector3(0.3f, 3.4f, 0.35f), color, root.transform);
                GameObject top = RuntimeVisuals.Primitive("Door Frame Top", PrimitiveType.Cube, root.transform.position, new Vector3(2.7f, 0.3f, 0.35f), color, root.transform);
                left.transform.localPosition = new Vector3(-1.35f, 1.7f, 0f);
                right.transform.localPosition = new Vector3(1.35f, 1.7f, 0f);
                top.transform.localPosition = new Vector3(0f, 3.4f, 0f);
                RuntimeVisuals.RemoveCollider(left); RuntimeVisuals.RemoveCollider(right); RuntimeVisuals.RemoveCollider(top);
                GameObject icon = RuntimeVisuals.Primitive("Room Type Icon", IconShape(room.type), root.transform.position,
                    Vector3.one * 0.72f, Color.Lerp(Color.white, color, 0.35f), root.transform);
                icon.transform.localPosition = new Vector3(0f, 2.35f, -0.18f);
                RuntimeVisuals.RemoveCollider(icon);
                door.CreateLabel(Color.Lerp(Color.white, color, 0.25f));
                BiomeLightingProfile lighting = ProceduralLightingDirector.CurrentProfile;
                PriorityLightAnchor.Attach(root, lighting == null ? color : Color.Lerp(color, lighting.activeDoorPriority, 0.5f), 3.8f, 0.48f, 3);
                ProceduralDungeonVisuals.DecorateDoor(root.transform, room);
            }
        }

        public override void Interact()
        {
            if (_run == null || !_run.PendingRoute) return;
            SetVisualState(WorldInteractionVisualState.Selected);
            RoomTransitionCurtain.Play();
            _run.ChooseRoute(_choiceIndex);
        }

        private static PrimitiveType IconShape(DungeonRoomType type)
        {
            if (type == DungeonRoomType.Boss || type == DungeonRoomType.Elite || type == DungeonRoomType.Miniboss) return PrimitiveType.Capsule;
            if (type == DungeonRoomType.Shop || type == DungeonRoomType.TreasureVault) return PrimitiveType.Cube;
            if (type == DungeonRoomType.HealingSanctuary || type == DungeonRoomType.SafeWorkshop) return PrimitiveType.Sphere;
            return PrimitiveType.Cylinder;
        }

        private static string RoomIcon(DungeonRoomType type)
        {
            if (type == DungeonRoomType.Boss) return "[BOSS]";
            if (type == DungeonRoomType.Elite || type == DungeonRoomType.Miniboss) return "[ELITE]";
            if (type == DungeonRoomType.Shop) return "[SHOP]";
            if (type == DungeonRoomType.HealingSanctuary) return "[HEAL]";
            if (type == DungeonRoomType.SafeWorkshop) return "[SAFE]";
            if (type == DungeonRoomType.ModifierReward) return "[SPELL]";
            if (type == DungeonRoomType.EquipmentReward) return "[GEAR]";
            if (type == DungeonRoomType.SpellCoreReward) return "[CORE]";
            if (type == DungeonRoomType.Extraction) return "[EXIT]";
            return "[FIGHT]";
        }

        private static string FriendlyType(DungeonRoomType type)
        {
            return type.ToString().Replace("ModifierReward", "Support Rune").Replace("SpellCoreReward", "Spell Core")
                .Replace("EquipmentReward", "Equipment").Replace("SafeWorkshop", "Safe Room").Replace("HealingSanctuary", "Healing Room")
                .Replace("NarrativeEvent", "Event").Replace("CursedBargain", "Cursed Choice");
        }
    }

    public sealed class RewardRerollStation : WorldRoomInteractable
    {
        private RunDirector _run;
        public static void Create(RunDirector run)
        {
            GameObject root = new GameObject("Reward Reroll Shrine"); root.transform.position = new Vector3(0f, 0f, -3f);
            RewardRerollStation station = root.AddComponent<RewardRerollStation>(); station._run = run;
            station.PromptTitle = "REWARD REROLL"; station.PromptAction = "REROLL ALL REWARDS";
            station.PromptDescription = run.RewardRerollsRemaining + " reroll(s) remain.";
            GameObject visual = RuntimeVisuals.Primitive("Reroll Crystal", PrimitiveType.Sphere, root.transform.position + Vector3.up * 0.65f,
                Vector3.one * 0.65f, new Color(0.25f, 0.85f, 1f), root.transform);
            RuntimeVisuals.RemoveCollider(visual); visual.transform.localPosition = Vector3.up * 0.65f;
            station.CreateLabel(new Color(0.7f, 0.95f, 1f));
        }
        public override void Interact()
        {
            if (_run == null || !_run.PendingRewards) return;
            _run.RerollRewards();
        }
    }

    public sealed class WorldEnemyHealthBar : MonoBehaviour
    {
        private EnemyController _enemy;
        private Transform _fill;
        private TextMesh _label;

        public static WorldEnemyHealthBar Create(EnemyController enemy)
        {
            GameObject root = new GameObject("World Health Bar · " + enemy.DisplayName);
            WorldEnemyHealthBar bar = root.AddComponent<WorldEnemyHealthBar>();
            bar._enemy = enemy;
            GameObject background = RuntimeVisuals.Primitive("Health Background", PrimitiveType.Cube, Vector3.zero,
                new Vector3(1.9f, 0.16f, 0.045f), new Color(0.015f, 0.02f, 0.03f), root.transform);
            GameObject fill = RuntimeVisuals.Primitive("Health Fill", PrimitiveType.Cube, Vector3.zero,
                new Vector3(1.82f, 0.105f, 0.055f), enemy.IsEliteOrBoss ? new Color(1f, 0.28f, 0.08f) : new Color(0.92f, 0.08f, 0.14f), root.transform);
            RuntimeVisuals.RemoveCollider(background); RuntimeVisuals.RemoveCollider(fill);
            background.transform.localPosition = Vector3.zero;
            fill.transform.localPosition = new Vector3(0f, 0f, -0.04f);
            bar._fill = fill.transform;
            GameObject labelObject = new GameObject("Health Label");
            labelObject.transform.SetParent(root.transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 0.27f, 0f);
            bar._label = labelObject.AddComponent<TextMesh>();
            bar._label.anchor = TextAnchor.LowerCenter;
            bar._label.alignment = TextAlignment.Center;
            bar._label.fontSize = 46;
            bar._label.characterSize = 0.034f;
            bar._label.fontStyle = FontStyle.Bold;
            bar._label.color = Color.white;
            return bar;
        }

        private void LateUpdate()
        {
            if (_enemy == null || _enemy.IsDead || Camera.main == null) { gameObject.SetActive(false); return; }
            bool always = ProfileManager.Current == null || ProfileManager.Current.accessibility.alwaysShowEnemyHealth;
            bool visible = !_enemy.IsBoss && (always || _enemy.Health < _enemy.MaxHealth || _enemy.IsEliteOrBoss);
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) renderer.enabled = visible;
            if (!visible) return;
            transform.position = _enemy.HealthBarWorldPosition;
            transform.rotation = Camera.main.transform.rotation;
            float ratio = _enemy.HealthRatio;
            _fill.localScale = new Vector3(1.82f * ratio, 0.105f, 0.055f);
            _fill.localPosition = new Vector3(-0.91f * (1f - ratio), 0f, -0.04f);
            string status = _enemy.StatusSummary;
            _label.text = (_enemy.IsEliteOrBoss ? _enemy.DisplayName.ToUpperInvariant() : string.Empty) +
                (string.IsNullOrEmpty(status) ? string.Empty : ( _enemy.IsEliteOrBoss ? "  ·  " : string.Empty) + status);
        }
    }
}
