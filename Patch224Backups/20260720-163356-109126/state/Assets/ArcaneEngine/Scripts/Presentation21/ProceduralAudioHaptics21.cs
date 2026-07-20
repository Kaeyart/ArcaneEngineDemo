using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ProceduralAudioPriority21
    {
        Decorative,
        Field,
        Secondary,
        Normal,
        Important,
        Critical
    }

    public struct HapticPresentationEvent21
    {
        public float magnitude;
        public float duration;
        public Vector3 direction;
        public float charge;
        public ReactionTier tier;
        public SpellPhase21 phase;
        public string contractId;
    }

    public static class HapticPresentationBus21
    {
        public static event Action<HapticPresentationEvent21> Published;

        public static void Emit(HapticPresentationEvent21 value)
        {
            Action<HapticPresentationEvent21> handler = Published;
            if (handler != null) handler(value);
        }
    }

    // ARCANE_PATCH_223_AUDIO_POOL
    public sealed class ProceduralAudioVoice21 : MonoBehaviour
    {
        public AudioSource source;
        public ProceduralAudioPriority21 priority;
        public float created;
        public float baseVolume;
        public bool loop;
        public Transform followTarget;

        private Vector3 _followLocalPosition;

        public bool Available
        {
            get { return source == null || !source.isPlaying; }
        }

        public void Configure(
            AudioClip clip,
            Vector3 position,
            Transform follow,
            float volume,
            float pitch,
            ProceduralAudioPriority21 voicePriority,
            bool shouldLoop)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            if (source == null)
                source = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

            source.enabled = true;
            source.Stop();

            followTarget = follow;
            _followLocalPosition = follow == null
                ? Vector3.zero
                : follow.InverseTransformPoint(position);
            transform.position = position;

            priority = voicePriority;
            created = Time.unscaledTime;
            baseVolume = volume;
            loop = shouldLoop;
            source.clip = clip;
            source.playOnAwake = false;
            source.spatialBlend = 0.85f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 2f;
            source.maxDistance = (int)voicePriority >= (int)ProceduralAudioPriority21.Important ? 45f : 24f;
            source.dopplerLevel = 0.15f;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = shouldLoop;

            if (gameObject.activeInHierarchy && source.enabled)
                source.Play();
        }

        public bool IsOwnedBy(Transform owner)
        {
            return owner != null && followTarget == owner;
        }

        public void StopAndReset()
        {
            if (source != null)
                source.Stop();
            followTarget = null;
            loop = false;
        }

        private void Update()
        {
            if (followTarget != null)
            {
                if (followTarget.gameObject.activeInHierarchy)
                {
                    transform.position = followTarget.TransformPoint(_followLocalPosition);
                }
                else
                {
                    if (loop && source != null)
                        source.Stop();
                    followTarget = null;
                    loop = false;
                }
            }

            if (source == null || !source.isPlaying)
            {
                if (!loop)
                    followTarget = null;
                return;
            }

            source.volume = baseVolume * ProceduralSpellAudio21.GlobalDuck;
        }
    }

    public static class ProceduralSpellAudio21
    {
        private const int SampleRate = 22050;
        private const int VoiceLimitLow = 14;
        private const int VoiceLimitMedium = 26;
        private const int VoiceLimitHigh = 40;
        private static readonly List<ProceduralAudioVoice21> Voices = new List<ProceduralAudioVoice21>();
        private static readonly Dictionary<int, AudioClip> Clips = new Dictionary<int, AudioClip>();
        private static GameObject _root;
        private static float _duckUntil;
        private static float _duckTarget = 1f;

        public static float GlobalDuck
        {
            get
            {
                if (Time.unscaledTime >= _duckUntil)
                    return 1f;
                return _duckTarget;
            }
        }

        public static int ActiveVoices
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Voices.Count; i++)
                    if (Voices[i] != null && !Voices[i].Available) count++;
                return count;
            }
        }

        public static bool Hotfix223Ready
        {
            get
            {
                if (!Application.isPlaying)
                    return true;
                EnsureRoot();
                return _root != null && _root.activeInHierarchy;
            }
        }

        public static void PlayPhase(
            SpellVisualContract21 contract,
            SpellPhase21 phase,
            Vector3 position,
            Transform follow)
        {
            if (contract == null)
                return;

            ProceduralAudioPriority21 priority = PriorityFor(contract, phase);
            float duration = DurationFor(phase, contract);
            int seed = StableSeed21.Combine(contract.seeds.audio, (int)phase);
            AudioClip clip = Clip(seed, contract, phase, duration);
            float volume = VolumeFor(contract, phase);
            float pitch = Mathf.Lerp(0.82f, 1.18f, StableSeed21.Unit(seed + 9));
            bool loop = phase == SpellPhase21.Persist && duration > 0.9f;
            Play(clip, position, follow, volume, pitch, priority, loop);

            if (phase == SpellPhase21.Release || phase == SpellPhase21.Resolve)
            {
                HapticPresentationBus21.Emit(new HapticPresentationEvent21
                {
                    magnitude = Mathf.Clamp01(0.18f + (int)contract.priority * 0.15f + contract.radius * 0.03f),
                    duration = Mathf.Clamp(duration, 0.05f, 0.5f),
                    direction = follow == null ? Vector3.forward : follow.forward,
                    charge = phase == SpellPhase21.Release ? 1f : 0f,
                    tier = contract.reactionTier,
                    phase = phase,
                    contractId = contract.contractId
                });
            }
        }

        public static void PlayNearMiss(
            SpellVisualContract21 contract,
            Vector3 position,
            float speed)
        {
            if (contract == null)
                return;
            int seed = StableSeed21.Combine(contract.seeds.audio, 0x4E4D);
            AudioClip clip = Clip(seed, contract, SpellPhase21.Travel, 0.16f);
            Play(clip, position, null, Mathf.Clamp(speed * 0.012f, 0.08f, 0.28f), 1.25f, ProceduralAudioPriority21.Secondary, false);
        }

        public static void Duck(float amount, float duration)
        {
            _duckTarget = Mathf.Clamp(amount, 0.28f, 1f);
            _duckUntil = Mathf.Max(_duckUntil, Time.unscaledTime + Mathf.Max(0.02f, duration));
        }

        public static void StopOwned(Transform owner)
        {
            if (owner == null) return;
            for (int i = 0; i < Voices.Count; i++)
            {
                ProceduralAudioVoice21 voice = Voices[i];
                if (voice != null && voice.IsOwnedBy(owner))
                    voice.StopAndReset();
            }
        }

        private static void Play(
            AudioClip clip,
            Vector3 position,
            Transform follow,
            float volume,
            float pitch,
            ProceduralAudioPriority21 priority,
            bool loop)
        {
            if (clip == null || volume <= 0.001f)
                return;
            EnsureRoot();
            ProceduralAudioVoice21 voice = Acquire(priority);
            if (voice == null) return;
            voice.Configure(clip, position, follow, volume, pitch, priority, loop);
        }

        private static ProceduralAudioVoice21 Acquire(ProceduralAudioPriority21 priority)
        {
            PruneVoices();

            int limit = Patch200PresentationSettings.Quality == PresentationQuality.Low
                ? VoiceLimitLow
                : Patch200PresentationSettings.Quality == PresentationQuality.Medium
                    ? VoiceLimitMedium
                    : VoiceLimitHigh;

            for (int i = 0; i < Voices.Count; i++)
            {
                ProceduralAudioVoice21 available = Voices[i];
                if (available != null && available.Available)
                {
                    PrepareVoice(available);
                    return available;
                }
            }

            if (Voices.Count < limit)
            {
                GameObject item = new GameObject("AE21 Audio Voice " + Voices.Count);
                item.transform.SetParent(_root.transform, false);
                ProceduralAudioVoice21 voice = item.AddComponent<ProceduralAudioVoice21>();
                voice.source = item.AddComponent<AudioSource>();
                Voices.Add(voice);
                PrepareVoice(voice);
                return voice;
            }

            ProceduralAudioVoice21 weakest = null;
            for (int i = 0; i < Voices.Count; i++)
            {
                ProceduralAudioVoice21 candidate = Voices[i];
                if (candidate == null) continue;
                if (weakest == null || (int)candidate.priority < (int)weakest.priority ||
                    (candidate.priority == weakest.priority && candidate.created < weakest.created))
                    weakest = candidate;
            }

            if (weakest != null && (int)weakest.priority <= (int)priority)
            {
                weakest.StopAndReset();
                PrepareVoice(weakest);
                return weakest;
            }
            return null;
        }

        private static void PrepareVoice(ProceduralAudioVoice21 voice)
        {
            if (voice == null) return;
            voice.transform.SetParent(_root.transform, false);
            if (!voice.gameObject.activeSelf)
                voice.gameObject.SetActive(true);
            if (voice.source != null)
                voice.source.enabled = true;
        }

        private static void PruneVoices()
        {
            for (int i = Voices.Count - 1; i >= 0; i--)
                if (Voices[i] == null)
                    Voices.RemoveAt(i);
        }

        private static AudioClip Clip(
            int seed,
            SpellVisualContract21 contract,
            SpellPhase21 phase,
            float duration)
        {
            int key = StableSeed21.Combine(seed, Mathf.RoundToInt(duration * 100f));
            AudioClip cached;
            if (Clips.TryGetValue(key, out cached) && cached != null)
                return cached;

            int samples = Mathf.Clamp(Mathf.RoundToInt(SampleRate * duration), 512, SampleRate * 3);
            float[] data = new float[samples];
            float baseFrequency = FrequencyFor(contract.primaryElement);
            float catalystFrequency = FrequencyFor(contract.catalyst) * 1.5f;
            float runeModulation = 1f + contract.runeGraph.Count * 0.035f;
            float phaseFrequency = PhaseFrequency(phase);
            float noiseAmount = NoiseFor(contract, phase);
            float attack = phase == SpellPhase21.Persist ? 0.12f : 0.025f;
            float release = phase == SpellPhase21.Persist ? 0.25f : 0.14f;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float normalized = i / (float)(samples - 1);
                float envelope = Mathf.Clamp01(normalized / attack) * Mathf.Clamp01((1f - normalized) / release);
                if (phase == SpellPhase21.Persist)
                    envelope = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(normalized / 0.08f)) * Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((1f - normalized) / 0.12f));

                float sweep = 1f;
                if (phase == SpellPhase21.Charge) sweep = Mathf.Lerp(0.55f, 1.4f, normalized);
                else if (phase == SpellPhase21.Return) sweep = Mathf.Lerp(1.35f, 0.72f, normalized);
                else if (phase == SpellPhase21.Resolve) sweep = Mathf.Lerp(1.2f, 0.55f, normalized);
                else if (phase == SpellPhase21.Hold) sweep = 0.8f + Mathf.Pow(normalized, 3f) * 0.9f;

                float f1 = baseFrequency * runeModulation * phaseFrequency * sweep;
                float f2 = catalystFrequency * (0.85f + Mathf.Sin(t * 3f) * 0.05f);
                float tone = Mathf.Sin(t * Mathf.PI * 2f * f1) * 0.55f;
                tone += Mathf.Sin(t * Mathf.PI * 2f * f2 + StableSeed21.Unit(seed) * Mathf.PI * 2f) * 0.25f;
                tone += Mathf.Sin(t * Mathf.PI * 2f * f1 * 0.5f) * 0.12f;
                float noise = PseudoNoise(seed, i) * noiseAmount;
                float rhythmic = 1f;
                if (phase == SpellPhase21.Hold || phase == SpellPhase21.Persist)
                    rhythmic = 0.45f + Mathf.Pow(0.5f + Mathf.Sin(t * Mathf.PI * 2f * (2f + contract.runeGraph.Count * 0.25f)) * 0.5f, 2f) * 0.55f;
                data[i] = Mathf.Clamp((tone + noise) * envelope * rhythmic * 0.55f, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create(
                "AE21 " + contract.contractId + " " + phase,
                samples,
                1,
                SampleRate,
                false);
            clip.SetData(data, 0);
            Clips[key] = clip;
            return clip;
        }

        private static float FrequencyFor(ReactionElement element)
        {
            switch (element)
            {
                case ReactionElement.Fire: return 190f;
                case ReactionElement.Cold: return 330f;
                case ReactionElement.Lightning: return 520f;
                case ReactionElement.Physical: return 95f;
                case ReactionElement.Blood: return 135f;
                case ReactionElement.Toxic: return 155f;
                case ReactionElement.Void: return 72f;
                default: return 260f;
            }
        }

        private static float PhaseFrequency(SpellPhase21 phase)
        {
            switch (phase)
            {
                case SpellPhase21.Charge: return 0.8f;
                case SpellPhase21.Release: return 1.35f;
                case SpellPhase21.Travel: return 1.1f;
                case SpellPhase21.Contact: return 0.65f;
                case SpellPhase21.Resolve: return 0.5f;
                case SpellPhase21.Return: return 0.9f;
                case SpellPhase21.Persist: return 0.7f;
                default: return 1f;
            }
        }

        private static float NoiseFor(SpellVisualContract21 contract, SpellPhase21 phase)
        {
            float value = 0.04f;
            if (contract.primaryElement == ReactionElement.Fire || contract.primaryElement == ReactionElement.Toxic) value += 0.10f;
            if (contract.primaryElement == ReactionElement.Physical) value += 0.18f;
            if (contract.primaryElement == ReactionElement.Void) value += 0.06f;
            if (contract.personality == VisualPersonality21.Unstable) value += 0.12f;
            if (phase == SpellPhase21.Resolve || phase == SpellPhase21.Contact) value += 0.08f;
            return value;
        }

        private static float VolumeFor(SpellVisualContract21 contract, SpellPhase21 phase)
        {
            float value = 0.16f + (int)contract.priority * 0.055f;
            if (phase == SpellPhase21.Release || phase == SpellPhase21.Resolve) value += 0.10f;
            if (phase == SpellPhase21.Persist) value *= 0.45f;
            return Mathf.Clamp(value, 0.06f, 0.55f);
        }

        private static float DurationFor(SpellPhase21 phase, SpellVisualContract21 contract)
        {
            switch (phase)
            {
                case SpellPhase21.Charge: return 0.38f;
                case SpellPhase21.Release: return 0.22f;
                case SpellPhase21.Travel: return 0.28f;
                case SpellPhase21.Contact: return 0.14f;
                case SpellPhase21.Resolve: return Mathf.Clamp(0.32f + contract.radius * 0.03f, 0.32f, 0.75f);
                case SpellPhase21.Persist: return Mathf.Clamp(contract.duration, 0.8f, 2.8f);
                case SpellPhase21.Return: return 0.34f;
                case SpellPhase21.Hold: return 0.35f;
                default: return 0.18f;
            }
        }

        private static ProceduralAudioPriority21 PriorityFor(SpellVisualContract21 contract, SpellPhase21 phase)
        {
            if ((int)contract.priority >= (int)PresentationPriority.Critical) return ProceduralAudioPriority21.Critical;
            if (phase == SpellPhase21.Release || phase == SpellPhase21.Resolve) return ProceduralAudioPriority21.Important;
            if (phase == SpellPhase21.Persist) return ProceduralAudioPriority21.Field;
            return ProceduralAudioPriority21.Normal;
        }

        private static float PseudoNoise(int seed, int sample)
        {
            int value = StableSeed21.Combine(seed, sample * 17);
            return StableSeed21.Unit(value) * 2f - 1f;
        }

        private static void EnsureRoot()
        {
            if (_root != null)
            {
                if (!_root.activeSelf)
                    _root.SetActive(true);
                return;
            }

            _root = new GameObject("Arcane Engine 2.1 Procedural Audio");
            UnityEngine.Object.DontDestroyOnLoad(_root);
        }
    }
}
