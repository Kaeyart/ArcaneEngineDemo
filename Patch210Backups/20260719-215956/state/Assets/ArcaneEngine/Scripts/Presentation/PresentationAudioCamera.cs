using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum PresentationAudioCue
    {
        Cast,
        Impact,
        Assembly,
        Reaction,
        DeathReaction,
        MajorAilment,
        FieldPulse
    }

    public static class ProceduralPresentationAudio2
    {
        private static readonly Dictionary<int, AudioClip> Clips =
            new Dictionary<int, AudioClip>();
        private static readonly List<AudioSource> Sources =
            new List<AudioSource>();
        private static Transform _root;
        private const int MaximumSources = 18;

        public static int ActiveSourceCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Sources.Count; i++)
                {
                    if (Sources[i] != null && Sources[i].isPlaying)
                        count++;
                }
                return count;
            }
        }

        public static void Play(
            Vector3 position,
            ReactionElement signature,
            PresentationAudioCue cue,
            float intensity,
            PresentationPriority priority)
        {
            AudioClip clip = GetClip(signature, cue);
            if (clip == null)
                return;

            AudioSource source = Acquire(priority);
            if (source == null)
                return;

            source.transform.position = position;
            source.clip = clip;
            source.volume = Mathf.Clamp01(0.18f + intensity * 0.24f);
            source.pitch = Pitch(signature, cue, intensity);
            source.spatialBlend = 0.72f;
            source.minDistance = 2f;
            source.maxDistance = 20f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.loop = false;
            source.Play();
        }

        private static AudioSource Acquire(PresentationPriority priority)
        {
            for (int i = 0; i < Sources.Count; i++)
            {
                if (Sources[i] != null && !Sources[i].isPlaying)
                    return Sources[i];
            }

            if (Sources.Count >= MaximumSources)
            {
                if (priority < PresentationPriority.Critical)
                    return null;

                AudioSource source = Sources[0];
                if (source != null)
                    source.Stop();
                return source;
            }

            GameObject gameObject = new GameObject("AE2 Procedural Audio");
            gameObject.transform.SetParent(Root(), false);
            AudioSource created = gameObject.AddComponent<AudioSource>();
            created.playOnAwake = false;
            Sources.Add(created);
            return created;
        }

        private static AudioClip GetClip(
            ReactionElement signature,
            PresentationAudioCue cue)
        {
            int key = ((int)signature << 8) | (int)cue;
            AudioClip clip;

            if (Clips.TryGetValue(key, out clip) && clip != null)
                return clip;

            int sampleRate = 22050;
            float length = Length(cue);
            int samples = Mathf.Max(512, Mathf.RoundToInt(sampleRate * length));
            float[] data = new float[samples];
            float baseFrequency = BaseFrequency(signature);
            float overtone = Overtone(signature);
            float noiseAmount = Noise(signature, cue);
            int seed = key * 1103515245 + 12345;
            System.Random random = new System.Random(seed);

            for (int i = 0; i < samples; i++)
            {
                float time = i / (float)sampleRate;
                float normalized = i / (float)(samples - 1);
                float envelope = Envelope(cue, normalized);
                float frequency = ModulatedFrequency(
                    baseFrequency,
                    cue,
                    normalized);
                float sine = Mathf.Sin(time * frequency * Mathf.PI * 2f);
                float harmonic = Mathf.Sin(time * frequency * overtone * Mathf.PI * 2f) * 0.35f;
                float sub = Mathf.Sin(time * frequency * 0.5f * Mathf.PI * 2f) * 0.18f;
                float noise = ((float)random.NextDouble() * 2f - 1f) * noiseAmount;
                float pulse = signature == ReactionElement.Lightning ||
                              ElementalReactionCodex.Contains(signature, ReactionElement.Lightning)
                    ? Mathf.Sign(Mathf.Sin(time * 37f)) * 0.12f
                    : 0f;
                data[i] = Mathf.Clamp((sine + harmonic + sub + noise + pulse) * envelope * 0.34f, -1f, 1f);
            }

            clip = AudioClip.Create(
                "AE2 " + cue + " " + signature,
                samples,
                1,
                sampleRate,
                false);
            clip.SetData(data, 0);
            Clips[key] = clip;
            return clip;
        }

        private static float Length(PresentationAudioCue cue)
        {
            switch (cue)
            {
                case PresentationAudioCue.Cast: return 0.22f;
                case PresentationAudioCue.Impact: return 0.28f;
                case PresentationAudioCue.Assembly: return 0.4f;
                case PresentationAudioCue.Reaction: return 0.56f;
                case PresentationAudioCue.DeathReaction: return 0.72f;
                case PresentationAudioCue.MajorAilment: return 0.42f;
                default: return 0.3f;
            }
        }

        private static float BaseFrequency(ReactionElement signature)
        {
            ReactionElement primary = ElementalReactionCodex.PrimaryElement(signature);

            switch (primary)
            {
                case ReactionElement.Fire: return 170f;
                case ReactionElement.Cold: return 430f;
                case ReactionElement.Lightning: return 760f;
                case ReactionElement.Physical: return 95f;
                case ReactionElement.Blood: return 125f;
                case ReactionElement.Toxic: return 210f;
                case ReactionElement.Void: return 72f;
                default: return 260f;
            }
        }

        private static float Overtone(ReactionElement signature)
        {
            int count = Mathf.Max(1, ElementalReactionCodex.CountBits(signature));
            return 1.5f + count * 0.13f;
        }

        private static float Noise(
            ReactionElement signature,
            PresentationAudioCue cue)
        {
            float result = 0.04f;

            if (ElementalReactionCodex.Contains(signature, ReactionElement.Fire))
                result += 0.08f;
            if (ElementalReactionCodex.Contains(signature, ReactionElement.Physical))
                result += 0.12f;
            if (ElementalReactionCodex.Contains(signature, ReactionElement.Toxic))
                result += 0.05f;
            if (cue == PresentationAudioCue.Impact || cue == PresentationAudioCue.DeathReaction)
                result += 0.08f;

            return result;
        }

        private static float Envelope(
            PresentationAudioCue cue,
            float normalized)
        {
            float attack = Mathf.Clamp01(normalized / 0.08f);
            float release = Mathf.Clamp01((1f - normalized) / 0.3f);
            float envelope = attack * release;

            if (cue == PresentationAudioCue.Assembly)
                envelope *= Mathf.Lerp(0.35f, 1f, normalized);

            if (cue == PresentationAudioCue.DeathReaction)
                envelope *= 1f - Mathf.Pow(normalized, 2f) * 0.4f;

            return envelope;
        }

        private static float ModulatedFrequency(
            float baseFrequency,
            PresentationAudioCue cue,
            float normalized)
        {
            switch (cue)
            {
                case PresentationAudioCue.Cast:
                    return baseFrequency * Mathf.Lerp(0.7f, 1.25f, normalized);
                case PresentationAudioCue.Assembly:
                    return baseFrequency * Mathf.Lerp(0.65f, 1.5f, normalized);
                case PresentationAudioCue.DeathReaction:
                    return baseFrequency * Mathf.Lerp(1.2f, 0.45f, normalized);
                default:
                    return baseFrequency * Mathf.Lerp(1.05f, 0.8f, normalized);
            }
        }

        private static float Pitch(
            ReactionElement signature,
            PresentationAudioCue cue,
            float intensity)
        {
            int count = Mathf.Max(1, ElementalReactionCodex.CountBits(signature));
            return Mathf.Clamp(
                0.82f + count * 0.025f + intensity * 0.03f,
                0.72f,
                1.18f);
        }

        private static Transform Root()
        {
            if (_root != null)
                return _root;

            GameObject root = GameObject.Find("Arcane Engine 2.0 Audio Pool");
            if (root == null)
            {
                root = new GameObject("Arcane Engine 2.0 Audio Pool");
                UnityEngine.Object.DontDestroyOnLoad(root);
            }

            _root = root.transform;
            return _root;
        }
    }

    public sealed class PresentationCameraFeedback2 : MonoBehaviour
    {
        private struct ImpulseData
        {
            public Vector3 source;
            public float magnitude;
            public float duration;
            public float started;
            public PresentationPriority priority;
        }

        private static readonly List<ImpulseData> Pending =
            new List<ImpulseData>();
        private static PresentationCameraFeedback2 _instance;
        private Camera _camera;
        private Vector3 _lastOffset;

        public static void Impulse(
            Vector3 source,
            float magnitude,
            float duration,
            PresentationPriority priority)
        {
            if (Patch200PresentationSettings.CameraFeedback <= 0.01f ||
                Patch200PresentationSettings.ReducedMotion)
            {
                return;
            }

            Ensure();
            Pending.Add(new ImpulseData
            {
                source = source,
                magnitude = magnitude * Patch200PresentationSettings.CameraFeedback,
                duration = Mathf.Max(0.05f, duration),
                started = Time.unscaledTime,
                priority = priority
            });

            while (Pending.Count > 8)
                Pending.RemoveAt(0);
        }

        private static void Ensure()
        {
            if (_instance != null)
                return;

            GameObject root = GameObject.Find("Arcane Engine 2.0 Camera Feedback");
            if (root == null)
            {
                root = new GameObject("Arcane Engine 2.0 Camera Feedback");
                UnityEngine.Object.DontDestroyOnLoad(root);
            }

            _instance = root.GetComponent<PresentationCameraFeedback2>();
            if (_instance == null)
                _instance = root.AddComponent<PresentationCameraFeedback2>();
        }

        private void LateUpdate()
        {
            if (_camera == null)
                _camera = Camera.main;

            if (_camera == null)
                return;

            if (_lastOffset.sqrMagnitude > 0f)
            {
                _camera.transform.position -= _lastOffset;
                _lastOffset = Vector3.zero;
            }

            Vector3 offset = Vector3.zero;

            for (int i = Pending.Count - 1; i >= 0; i--)
            {
                ImpulseData impulse = Pending[i];
                float progress = (Time.unscaledTime - impulse.started) / impulse.duration;

                if (progress >= 1f)
                {
                    Pending.RemoveAt(i);
                    continue;
                }

                float envelope = (1f - progress) * (1f - progress);
                float phase = Time.unscaledTime * 73f + i * 2.17f;
                Vector3 local = new Vector3(
                    Mathf.Sin(phase),
                    Mathf.Cos(phase * 1.37f),
                    0f) * impulse.magnitude * envelope;
                offset += _camera.transform.TransformDirection(local);
            }

            offset = Vector3.ClampMagnitude(offset, 0.18f);
            _camera.transform.position += offset;
            _lastOffset = offset;
        }

        private void OnDisable()
        {
            if (_camera != null && _lastOffset.sqrMagnitude > 0f)
                _camera.transform.position -= _lastOffset;

            _lastOffset = Vector3.zero;
            Pending.Clear();
        }
    }
}
