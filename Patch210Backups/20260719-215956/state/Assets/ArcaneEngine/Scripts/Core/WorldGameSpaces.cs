using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public static class PhysicalShopSystem
    {
        public static void Spawn(RunDirector run, IList<ShopOffer> offers)
        {
            WorldInteractionController.ClearType<ShopOfferPedestal>();
            WorldInteractionController.ClearType<RoomServiceStation>();
            GameObject merchant = RuntimeVisuals.Primitive("Dungeon Merchant", PrimitiveType.Capsule, new Vector3(0f, 1f, 8f),
                new Vector3(1.2f, 1.8f, 1.2f), new Color(0.85f, 0.58f, 0.15f));
            RuntimeVisuals.RemoveCollider(merchant);
            merchant.AddComponent<PhysicalRoomProp>();
            int visible = Mathf.Min(offers == null ? 0 : offers.Count, 9);
            for (int i = 0; i < visible; i++)
            {
                float x = -7f + (i % 3) * 7f;
                float z = 2.5f - (i / 3) * 5.3f;
                ShopOfferPedestal.Create(run, i, offers[i], new Vector3(x, 0f, z));
            }
            RoomServiceStation.Create(run, RoomServiceAction.LeaveRoom, new Vector3(0f, 0f, 13.5f), "LEAVE SHOP", "Open the next route doors.", new Color(0.25f, 0.85f, 1f));
        }
    }

    public sealed class PhysicalRoomProp : MonoBehaviour { }

    public sealed class ExtractionGate : WorldRoomInteractable
    {
        private RunDirector _run;
        public static void Spawn(RunDirector run)
        {
            WorldInteractionController.ClearType<ExtractionGate>();
            GameObject root = new GameObject("Extraction Gate"); root.transform.position = new Vector3(0f, 0f, 5f);
            ExtractionGate gate = root.AddComponent<ExtractionGate>(); gate._run = run; gate.InteractionRadius = 3.5f;
            gate.PromptTitle = "EXTRACTION GATE"; gate.PromptAction = "EXTRACT & BANK LOOT";
            gate.PromptDescription = "Finish the run. Secure equipment, Spell Copies, and Forge materials; Gold and dungeon Support Runes are left behind.";
            for (int i = 0; i < 7; i++)
            {
                float angle = i / 7f * Mathf.PI * 2f;
                GameObject shard = RuntimeVisuals.Primitive("Gate Shard", PrimitiveType.Cube,
                    root.transform.position + new Vector3(Mathf.Cos(angle) * 2.2f, 1.6f + Mathf.Sin(angle * 2f) * 0.45f, Mathf.Sin(angle) * 0.4f),
                    new Vector3(0.22f, 0.8f, 0.22f), new Color(0.2f, 1f, 0.75f), root.transform);
                RuntimeVisuals.RemoveCollider(shard);
            }
            RuntimeVisuals.Ring("Extraction Ring", root.transform.position, new Color(0.2f, 1f, 0.75f), 2.4f, 0.18f, root.transform).transform.localPosition = Vector3.zero;
            gate.CreateLabel(new Color(0.65f, 1f, 0.85f));
        }
        public override void Interact()
        {
            if (_run == null || !_run.PendingExtraction) return;
            _run.EndRun(true);
        }
    }

    public sealed class ShopOfferPedestal : WorldRoomInteractable
    {
        private RunDirector _run;
        private int _index;
        private ShopOffer _offer;

        public static ShopOfferPedestal Create(RunDirector run, int index, ShopOffer offer, Vector3 position)
        {
            GameObject root = new GameObject("Shop Pedestal · " + offer.title); root.transform.position = position;
            ShopOfferPedestal pedestal = root.AddComponent<ShopOfferPedestal>(); pedestal._run = run; pedestal._index = index; pedestal._offer = offer;
            pedestal.PromptTitle = offer.title.ToUpperInvariant(); pedestal.PromptAction = offer.price + " GOLD · BUY"; pedestal.PromptDescription = offer.description;
            RewardVisualSystem.AttachOffer(root.transform, new RewardOffer
            {
                id = offer.id,
                category = offer.category,
                title = offer.title,
                description = offer.description,
                contentId = offer.contentId,
                amount = 1,
                generatedItem = offer.generatedItem,
                color = CategoryColor(offer.category)
            });
            pedestal.CreateLabel(Color.white); return pedestal;
        }

        public override void Interact()
        {
            if (_run == null || _offer == null) return;
            string result;
            if (_run.BuyShopOffer(_index, out result))
            {
                PromptAction = "SOLD"; PromptDescription = result; RefreshLabel(true); AdaptiveAudioSystem.PlayUI(true);
                foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = RuntimeVisuals.Material(Color.gray, 0.05f);
            }
            else { PromptDescription = result; AdaptiveAudioSystem.PlayUI(false); }
        }

        private static Color CategoryColor(RewardCategory category)
        {
            if (category == RewardCategory.Equipment) return new Color(1f, 0.55f, 0.15f);
            if (category == RewardCategory.Modifier) return new Color(0.6f, 0.2f, 1f);
            if (category == RewardCategory.SpellCore) return new Color(0.2f, 0.75f, 1f);
            if (category == RewardCategory.Healing) return new Color(0.2f, 1f, 0.45f);
            return new Color(1f, 0.82f, 0.2f);
        }
    }

    public enum RoomServiceAction { Workshop, Equipment, Heal, Training, LeaveRoom }

    public sealed class RoomServiceStation : WorldRoomInteractable
    {
        private RunDirector _run;
        private RoomServiceAction _action;
        private bool _used;
        public static RoomServiceStation Create(RunDirector run, RoomServiceAction action, Vector3 position, string title, string description, Color color)
        {
            GameObject root = new GameObject(title); root.transform.position = position;
            RoomServiceStation station = root.AddComponent<RoomServiceStation>(); station._run = run; station._action = action;
            station.PromptTitle = title; station.PromptAction = "USE"; station.PromptDescription = description;
            GameObject visual = RuntimeVisuals.Primitive(title, action == RoomServiceAction.Heal ? PrimitiveType.Sphere : PrimitiveType.Cylinder,
                position + Vector3.up * 0.65f, new Vector3(0.9f, 0.9f, 0.9f), color, root.transform);
            RuntimeVisuals.RemoveCollider(visual); visual.transform.localPosition = Vector3.up * 0.65f;
            RuntimeVisuals.Ring(title + " Ring", position, color, 1.1f, 0.1f, root.transform).transform.localPosition = Vector3.zero;
            station.CreateLabel(Color.Lerp(Color.white, color, 0.25f));
            if (run != null && action == RoomServiceAction.Heal && run.IsRoomServiceUsed("room_heal"))
            {
                station._used = true; station.PromptAction = "USED"; station.PromptDescription = "The fountain is quiet until the next room."; station.RefreshLabel(true);
            }
            return station;
        }
        public override void Interact()
        {
            if (_action == RoomServiceAction.Workshop && DemoUI.Instance != null) DemoUI.Instance.OpenWorkshopAnywhere();
            else if (_action == RoomServiceAction.Equipment && DemoUI.Instance != null) DemoUI.Instance.OpenInventoryAnywhere();
            else if (_action == RoomServiceAction.Heal && GameWorld.Instance != null)
            {
                if (_used || (_run != null && !_run.TryUseRoomService("room_heal"))) { _used = true; AdaptiveAudioSystem.PlayUI(false); return; }
                GameWorld.Instance.Player.Heal(GameWorld.Instance.Stats.maxHealth * 0.35f); GameWorld.Instance.Player.RestoreMana(GameWorld.Instance.Stats.maxMana * 0.45f);
                _used = true; PromptAction = "USED"; PromptDescription = "The fountain is quiet until the next room."; RefreshLabel(true); AdaptiveAudioSystem.PlayUI(true);
                if (_run != null) _run.SaveRunCheckpoint();
            }
            else if (_action == RoomServiceAction.LeaveRoom && _run != null)
            {
                if (_run.PendingShop) _run.LeaveShop(); else _run.LeaveSafeRoom();
            }
        }
    }

    public static class SafeRoomSystem
    {
        public static void Spawn(RunDirector run, bool healing)
        {
            WorldInteractionController.ClearType<RoomServiceStation>();
            RoomServiceStation.Create(run, RoomServiceAction.Workshop, new Vector3(-7f, 0f, 2f), "SPELL WORKBENCH", "Rebuild any spell board without consuming pieces.", new Color(0.45f, 0.25f, 1f));
            RoomServiceStation.Create(run, RoomServiceAction.Equipment, new Vector3(7f, 0f, 2f), "UNSECURED RUN BAG", "Inspect, compare, mark, or salvage equipment found this expedition.", new Color(1f, 0.55f, 0.12f));
            RoomServiceStation.Create(run, RoomServiceAction.Heal, new Vector3(0f, 0f, 6f), healing ? "GREATER HEALING FONT" : "RESTORATION FONT", "Restore Health and Mana once in this room.", new Color(0.18f, 1f, 0.55f));
            RoomServiceStation.Create(run, RoomServiceAction.LeaveRoom, new Vector3(0f, 0f, 13.5f), "CONTINUE EXPEDITION", "Open the next set of route doors.", new Color(0.2f, 0.85f, 1f));
        }
    }

    public enum HubStationType { StartRun, Loadout, SpellArchive, Armory, Forge, Upgrades, Collection, TrainingOptions, Tutorial, SaveProfile }

    public sealed class HomeBaseController : MonoBehaviour
    {
        public static HomeBaseController Instance { get; private set; }
        public static bool IsHomeActive { get { return GameWorld.Instance != null && !GameWorld.Instance.RunActive && !GameWorld.Instance.TrainingMode; } }
        private Vector3 _velocity;

        private void Awake() { Instance = this; }
        private void Start() { Rebuild(); }

        public void Rebuild()
        {
            if (GameWorld.Instance == null || GameWorld.Instance.Player == null || !IsHomeActive) return;
            WorldInteractionController.ClearType<HubStation>();
            Create(HubStationType.StartRun, new Vector3(0f, 0f, 11f), "DUNGEON GATE", "Choose starting spells and begin a run.", new Color(0.2f, 0.9f, 1f));
            Create(HubStationType.Loadout, new Vector3(-9f, 0f, 7f), "STARTING LOADOUT", "Choose stored Spell Copies and starting Support Runes.", new Color(0.45f, 0.3f, 1f));
            Create(HubStationType.SpellArchive, new Vector3(-12f, 0f, 0f), "SPELL ARCHIVE", "Stored spells, Legendary evolution, and saved layouts.", new Color(0.7f, 0.2f, 1f));
            Create(HubStationType.Armory, new Vector3(-9f, 0f, -8f), "ARMORY", "Equipment, paper-doll slots, loadouts, favorites, and junk.", new Color(1f, 0.5f, 0.12f));
            Create(HubStationType.Forge, new Vector3(0f, 0f, -8f), "EQUIPMENT FORGE", "Craft, improve, protect, corrupt, or dismantle secured equipment.", new Color(1f, 0.25f, 0.12f));
            Create(HubStationType.Upgrades, new Vector3(9f, 0f, -8f), "PERMANENT SHRINE", "Spend Essence on permanent progression.", new Color(0.2f, 1f, 0.68f));
            Create(HubStationType.Collection, new Vector3(12f, 0f, 0f), "BESTIARY & COLLECTION", "Review discovered enemies, rooms, spells, and trophies.", new Color(0.25f, 0.7f, 1f));
            Create(HubStationType.TrainingOptions, new Vector3(9f, 0f, 7f), "TRAINING & OPTIONS", "Training room, accessibility, input, audio, and video.", new Color(0.85f, 0.85f, 1f));
            Create(HubStationType.Tutorial, new Vector3(-5f, 0f, -13f), "APPRENTICE TRIAL", "Play the guided combat and spell-building introduction.", new Color(0.35f, 1f, 0.75f));
            Create(HubStationType.SaveProfile, new Vector3(5f, 0f, -13f), "MEMORY CRYSTAL", "Save the current profile and inspect save status.", new Color(1f, 0.35f, 0.75f));
        }

        private static void Create(HubStationType type, Vector3 position, string title, string description, Color color) { HubStation.Create(type, position, title, description, color); }

        private void Update()
        {
            if (!IsHomeActive || GameWorld.Instance.Player == null || GameWorld.Instance.ModalOpen) { _velocity = Vector3.zero; return; }
            ControlSettings controls = ProfileManager.Current.controls;
            float x = (ArcaneInput.GetKey(controls.moveRight) ? 1f : 0f) - (ArcaneInput.GetKey(controls.moveLeft) ? 1f : 0f);
            float z = (ArcaneInput.GetKey(controls.moveForward) ? 1f : 0f) - (ArcaneInput.GetKey(controls.moveBack) ? 1f : 0f);
            Vector3 movement = new Vector3(x, 0f, z); if (movement.sqrMagnitude > 1f) movement.Normalize();
            Camera camera = Camera.main; IsometricCamera rig = camera == null ? null : camera.GetComponent<IsometricCamera>();
            if (rig != null) movement = rig.PlanarRight * x + rig.PlanarForward * z;
            _velocity = Vector3.MoveTowards(_velocity, movement * 6.2f, 28f * Time.deltaTime);
            Vector3 position = GameWorld.Instance.Player.transform.position + _velocity * Time.deltaTime;
            position.x = Mathf.Clamp(position.x, -15f, 15f); position.z = Mathf.Clamp(position.z, -15f, 15f); position.y = 1f;
            GameWorld.Instance.Player.transform.position = position;
            // PlayerController owns facing in every playable space. Home Base movement must
            // never overwrite the independent mouse aim direction.
        }
    }

    public sealed class HubStation : WorldRoomInteractable
    {
        private HubStationType _type;
        public static HubStation Create(HubStationType type, Vector3 position, string title, string description, Color color)
        {
            GameObject root = new GameObject(title); root.transform.position = position;
            HubStation station = root.AddComponent<HubStation>(); station._type = type; station.InteractionRadius = 3.2f;
            station.PromptTitle = title; station.PromptAction = "USE STATION"; station.PromptDescription = description;
            HubStationVisualBuilder.Build(root.transform, type, color);
            station.CreateLabel(Color.Lerp(Color.white, color, 0.25f)); return station;
        }
        public override void Interact()
        {
            if (_type == HubStationType.StartRun) RunStartScreen.Show(GameWorld.Instance.GetComponent<RunDirector>());
            else if (_type == HubStationType.Loadout) DemoUI.Instance.OpenHomeSection(1);
            else if (_type == HubStationType.SpellArchive) DemoUI.Instance.OpenHomeSection(2);
            else if (_type == HubStationType.Armory) DemoUI.Instance.OpenHomeSection(3);
            else if (_type == HubStationType.Forge) DemoUI.Instance.OpenHomeSection(4);
            else if (_type == HubStationType.Upgrades) DemoUI.Instance.OpenHomeSection(5);
            else if (_type == HubStationType.Collection) DemoUI.Instance.OpenHomeSection(6);
            else if (_type == HubStationType.TrainingOptions) DemoUI.Instance.OpenHomeSection(7);
            else if (_type == HubStationType.Tutorial && TutorialDirector.Instance != null) TutorialDirector.Instance.StartTutorial();
            else if (_type == HubStationType.SaveProfile)
            {
                ProfileManager.Save(); PromptDescription = ProfileManager.LastSaveStatus + " · " + ProfileManager.ProfileFolderPath; AdaptiveAudioSystem.PlayUI(true);
            }
        }
    }

    public sealed class TutorialDirector : MonoBehaviour
    {
        public static TutorialDirector Instance { get; private set; }
        public bool Active { get; private set; }
        public string Prompt { get; private set; }
        private int _step;
        private int _modifierCount;
        private float _started;
        private void Awake() { Instance = this; }
        public void StartTutorial()
        {
            Active = true; _step = 0; _started = Time.time; _modifierCount = 0;
            GameWorld.Instance.EnterTraining(); Prompt = "STEP 1 · Move with WASD.";
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce("APPRENTICE TRIAL", 2f);
        }
        private void Update()
        {
            if (!Active || GameWorld.Instance == null || !GameWorld.Instance.TrainingMode) return;
            ControlSettings controls = ProfileManager.Current.controls;
            if (_step == 0 && (ArcaneInput.GetKey(controls.moveForward) || ArcaneInput.GetKey(controls.moveBack) || ArcaneInput.GetKey(controls.moveLeft) || ArcaneInput.GetKey(controls.moveRight))) Advance("STEP 2 · Aim with the mouse and cast Spell Slot 1.");
            else if (_step == 1 && ArcaneInput.GetMouseButtonDown(0)) Advance("STEP 3 · Press Space to dodge through danger.");
            else if (_step == 2 && ArcaneInput.GetKeyDown(controls.dodge))
            {
                SpellModifierDefinition modifier = DemoCatalog.AllModifiers.FirstOrDefault();
                if (modifier != null) LootPickup.CreateModifier(GameWorld.Instance.Player.transform.position + Vector3.forward * 2f, modifier.id);
                _modifierCount = GameWorld.Instance.OwnedModifierCounts.Values.Sum(); Advance("STEP 4 · Approach the Support Rune and press E to collect it.");
            }
            else if (_step == 3 && GameWorld.Instance.OwnedModifierCounts.Values.Sum() > _modifierCount) Advance("STEP 5 · Press Tab to open Spellcraft.");
            else if (_step == 4 && DemoUI.Instance != null && DemoUI.Instance.WorkshopOpen) Advance("STEP 6 · Drag the Support Rune onto an unlocked cell. Rotate it until its input connects to the Spell Core.");
            else if (_step == 5 && GameWorld.Instance.GetBoard(SpellSlot.Slot1) != null && GameWorld.Instance.GetBoard(SpellSlot.Slot1).GetActivePlacements().Count > 0)
                Advance("STEP 7 · The rune is connected. Close Spellcraft, then cast the modified spell.");
            else if (_step == 6 && (DemoUI.Instance == null || !DemoUI.Instance.WorkshopOpen) && ArcaneInput.GetMouseButtonDown(0) && Time.time - _started > 3f)
            {
                ProfileManager.Current.tutorialCompleted = true; ProfileManager.Save(); Prompt = "TRIAL COMPLETE · Escape returns to Home Base."; Active = false;
                if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce("APPRENTICE TRIAL COMPLETE", 3f);
            }
        }
        private void Advance(string prompt)
        {
            _step++; Prompt = prompt;
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce(prompt, 2.4f);
            if (ProfileManager.Current.accessibility.pauseForTutorialText) StartCoroutine(PauseForReading());
        }

        private System.Collections.IEnumerator PauseForReading()
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(0.65f);
            if (GameWorld.Instance != null && GameWorld.Instance.TrainingMode && !GameWorld.Instance.ModalOpen) Time.timeScale = 1f;
        }
    }

    public sealed class AdaptiveAudioSystem : MonoBehaviour
    {
        public static AdaptiveAudioSystem Instance { get; private set; }
        private AudioSource _music;
        private AudioSource _ambience;
        private AudioSource _ui;
        private AudioSource _enemy;
        private AudioSource _voice;
        private float _musicIntensity;
        private float _voiceLockedUntil;
        private void Awake()
        {
            Instance = this;
            _music = CreateSource("Adaptive Music · Awaiting Authored Audio", null, false);
            _ambience = CreateSource("Dungeon Ambience · Awaiting Authored Audio", null, false);
            _ui = CreateSource("UI Audio", CreateTone("UI Confirm", 660f, 0.08f, false), false);
            _enemy = CreateSource("Enemy Audio", CreateTone("Enemy Warning", 92f, 0.22f, false), false);
            _voice = CreateSource("Narrative Audio", CreateTone("Narrative Cue", 240f, 0.32f, false), false);
        }
        private void Update()
        {
            AccessibilitySettings settings = ProfileManager.Current.accessibility;
            float master = Mathf.Clamp01(settings.masterVolume);
            RunDirector run = GameWorld.Instance == null ? null : GameWorld.Instance.GetComponent<RunDirector>();
            bool combat = run != null && run.EncounterActive;
            bool boss = combat && run.CurrentRoom != null && run.CurrentRoom.type == DungeonRoomType.Boss;
            float wantedIntensity = boss ? 1f : combat ? 0.68f : GameWorld.Instance != null && GameWorld.Instance.RunActive ? 0.28f : 0.12f;
            _musicIntensity = Mathf.MoveTowards(_musicIntensity, wantedIntensity, Time.unscaledDeltaTime * 0.55f);
            float duck = Time.unscaledTime < _voiceLockedUntil ? 0.48f : 1f;
            _music.volume = 0f;
            _music.pitch = Mathf.Lerp(0.82f, boss ? 1.5f : 1.2f, _musicIntensity);
            _ambience.volume = 0f;
            _ui.volume = master * settings.uiVolume * 0.35f;
            _enemy.volume = master * settings.enemyVolume * 0.3f;
            _voice.volume = master * settings.voiceVolume * 0.35f;
        }
        public static void PlayUI(bool positive)
        {
            if (V21AudioDirector.Instance != null && V21AudioDirector.Instance.Play(positive ? "ui_confirm" : "ui_cancel",
                GameWorld.Instance == null || GameWorld.Instance.Player == null ? Vector3.zero : GameWorld.Instance.Player.transform.position, 1f, 0.04f)) return;
            if (Instance == null) return; Instance._ui.pitch = positive ? 1.15f : 0.72f; Instance._ui.Play();
        }

        public static void PlayBossPhase(int phase)
        {
            if (Instance == null) return;
            Instance._ui.pitch = 0.75f + Mathf.Clamp(phase, 1, 3) * 0.22f;
            Instance._ui.Play();
            PlayNarrativeCue("boss_phase_" + phase, "Boss phase " + phase + " begins");
        }

        public static void PlayEnemyCue(string cue, bool urgent)
        {
            if (V21AudioDirector.Instance != null)
                V21AudioDirector.Instance.Play(cue == "melee" ? "enemy_attack" : "enemy_telegraph",
                    GameWorld.Instance == null || GameWorld.Instance.Player == null ? Vector3.zero : GameWorld.Instance.Player.transform.position,
                    urgent ? 1f : 0.75f, urgent ? 0.05f : 0.1f);
            if (Instance == null || Time.unscaledTime < Instance._voiceLockedUntil - 0.25f) return;
            Instance._enemy.pitch = urgent ? 1.35f : 0.9f;
            Instance._enemy.Play();
            if (ProfileManager.Current.accessibility.visualAudioCues && V1GameDirector.Instance != null)
                V1GameDirector.Instance.ShowHint("audio:" + cue, urgent ? "⚠ DANGER NEARBY" : "◉ ENEMY ACTION NEARBY", 1.2f);
        }

        public static void PlayNarrativeCue(string id, string visualText)
        {
            if (Instance == null || Time.unscaledTime < Instance._voiceLockedUntil) return;
            Instance._voiceLockedUntil = Time.unscaledTime + 0.7f;
            Instance._voice.pitch = 0.88f + UnityEngine.Random.Range(-0.04f, 0.04f);
            Instance._voice.Play();
            if (ProfileManager.Current.accessibility.visualAudioCues && V1GameDirector.Instance != null)
                V1GameDirector.Instance.ShowHint("narrative:" + id, "♪ " + visualText, 2.2f);
        }
        private AudioSource CreateSource(string name, AudioClip clip, bool loop)
        {
            GameObject child = new GameObject(name); child.transform.SetParent(transform, false); AudioSource source = child.AddComponent<AudioSource>();
            source.clip = clip; source.loop = loop; source.playOnAwake = false; source.spatialBlend = 0f; if (loop && clip != null) source.Play(); return source;
        }
        private static AudioClip CreateTone(string name, float frequency, float seconds, bool layered)
        {
            int rate = 22050; int samples = Mathf.RoundToInt(rate * seconds); float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)rate; float envelope = layered ? 0.45f + Mathf.Sin(t * Mathf.PI * 0.5f) * 0.15f : Mathf.Sin(Mathf.PI * Mathf.Clamp01(t / seconds));
                data[i] = (Mathf.Sin(t * frequency * Mathf.PI * 2f) + (layered ? Mathf.Sin(t * frequency * 1.5f * Mathf.PI * 2f) * 0.35f : 0f)) * envelope * 0.14f;
            }
            AudioClip clip = AudioClip.Create(name, samples, 1, rate, false); clip.SetData(data, 0); return clip;
        }
    }
}
