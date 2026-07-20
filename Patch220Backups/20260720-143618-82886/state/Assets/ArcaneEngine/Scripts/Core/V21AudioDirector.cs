using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class V21AudioDirector : MonoBehaviour
    {
        public static V21AudioDirector Instance { get; private set; }
        private readonly Dictionary<string, V21AudioEventAsset> _events = new Dictionary<string, V21AudioEventAsset>();
        private readonly Dictionary<string, float> _lastPlayed = new Dictionary<string, float>();
        private readonly List<AudioSource> _effects = new List<AudioSource>();
        private AudioSource _music;
        private int _effectIndex;
        private string _musicState;

        private void Awake()
        {
            Instance = this;
            foreach (V21AudioEventAsset definition in Resources.LoadAll<V21AudioEventAsset>("V21Content/Audio"))
                if (definition != null && !string.IsNullOrEmpty(definition.stableId)) _events[definition.stableId] = definition;
            for (int i = 0; i < 16; i++) _effects.Add(CreateSource("Audio Voice " + (i + 1)));
            _music = CreateSource("Adaptive Music State");
            _music.loop = true;
            _music.spatialBlend = 0f;
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Update()
        {
            AccessibilitySettings settings = ProfileManager.Current.accessibility;
            float master = Mathf.Clamp01(settings.masterVolume);
            foreach (AudioSource source in _effects)
                if (source != null) source.mute = master <= 0.001f;
            if (_music != null) _music.volume = master * Mathf.Clamp01(settings.musicVolume) * 0.18f;

            GameWorld world = GameWorld.Instance;
            RunDirector run = world == null ? null : world.GetComponent<RunDirector>();
            string wanted = world == null || !world.RunActive ? "music_home" :
                run != null && run.EncounterActive && run.CurrentRoom != null && run.CurrentRoom.type == DungeonRoomType.Boss ? "music_boss" :
                run != null && run.EncounterActive && run.CurrentRoom != null && run.CurrentRoom.type == DungeonRoomType.Elite ? "music_elite" :
                run != null && run.EncounterActive ? "music_combat" : "music_explore";
            SetMusic(wanted);
        }

        public bool Play(string id, Vector3 position, float volume = 1f, float cooldown = 0.02f)
        {
            V21AudioEventAsset definition;
            if (!_events.TryGetValue(id, out definition) || definition.clips == null || definition.clips.Length == 0) return false;
            float last;
            if (_lastPlayed.TryGetValue(id, out last) && Time.unscaledTime - last < cooldown) return false;
            _lastPlayed[id] = Time.unscaledTime;
            AudioClip clip = definition.clips[Mathf.Abs((Time.frameCount + id.GetHashCode()) % definition.clips.Length)];
            if (clip == null) return false;
            AudioSource source = _effects[_effectIndex++ % _effects.Count];
            source.transform.position = ProfileManager.Current.accessibility.monoAudio && Camera.main != null ? Camera.main.transform.position : position;
            source.spatialBlend = ProfileManager.Current.accessibility.monoAudio ? 0f : 0.72f;
            source.pitch = Random.Range(definition.pitchRange.x, definition.pitchRange.y);
            source.volume = Mathf.Clamp01(volume) * Random.Range(definition.volumeRange.x, definition.volumeRange.y) *
                ProfileManager.Current.accessibility.masterVolume * CategoryVolume(definition.category);
            source.clip = clip;
            source.loop = false;
            source.Play();
            return true;
        }

        private void SetMusic(string id)
        {
            if (_musicState == id || _music == null) return;
            _musicState = id;
            V21AudioEventAsset definition;
            if (!_events.TryGetValue(id, out definition) || definition.clips == null || definition.clips.Length == 0)
            {
                _music.Stop();
                _music.clip = null;
                return;
            }
            _music.clip = definition.clips[0];
            _music.loop = true;
            // Music starts only when a real persisted clip exists. No generated
            // always-on fallback hum is created at runtime.
            _music.Play();
        }

        private float CategoryVolume(string category)
        {
            AccessibilitySettings value = ProfileManager.Current.accessibility;
            if (category == "Music") return value.musicVolume;
            if (category == "Interface") return value.uiVolume;
            if (category == "Enemy") return value.enemyVolume;
            if (category == "Ambience") return value.ambienceVolume;
            if (category == "Voice") return value.voiceVolume;
            return value.effectsVolume;
        }

        private AudioSource CreateSource(string name)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(transform, false);
            AudioSource source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.dopplerLevel = 0f;
            return source;
        }
    }
}
