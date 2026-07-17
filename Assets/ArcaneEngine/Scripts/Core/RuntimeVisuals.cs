using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public static class RuntimeVisuals
    {
        private const int MaximumSharedMaterials = 160;
        private static readonly Dictionary<int, Material> Materials = new Dictionary<int, Material>();
        private static readonly Dictionary<int, Color> MaterialColors = new Dictionary<int, Color>();
        private static readonly Dictionary<int, float> MaterialEmissions = new Dictionary<int, float>();
        public static int MaterialCount { get { return Materials.Count; } }
        public static int MaterialLimit { get { return MaximumSharedMaterials; } }

        public static Material Material(Color color, float emission = 0.2f)
        {
            // Runtime colors are intentionally quantized. Seeded visual variation must never
            // manufacture an unbounded number of shared materials during a long session.
            Color boundedColor = QuantizeColor(color);
            float boundedEmission = Mathf.Round(Mathf.Clamp(emission, 0f, 2f) * 4f) * 0.25f;
            int key = MaterialKey(boundedColor, boundedEmission);
            Material cached;
            if (Materials.TryGetValue(key, out cached) && cached != null) return cached;

            if (Materials.Count >= MaximumSharedMaterials)
                return ClosestMaterial(boundedColor, boundedEmission);

            bool scriptedPipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null;
            Shader shader = scriptedPipeline ? Shader.Find("Universal Render Pipeline/Lit") : Shader.Find("Standard");
            if (shader == null && scriptedPipeline) shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Hidden/Internal-Colored");
            if (shader == null) return ClosestMaterial(boundedColor, boundedEmission);
            Material material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
            material.name = "Arcane Shared " + key;
            material.color = boundedColor;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", boundedColor);
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", boundedColor * boundedEmission);
            }
            Materials[key] = material;
            MaterialColors[key] = boundedColor;
            MaterialEmissions[key] = boundedEmission;
            return material;
        }

        private static Color QuantizeColor(Color color)
        {
            const float levels = 15f;
            return new Color(
                Mathf.Round(Mathf.Clamp01(color.r) * levels) / levels,
                Mathf.Round(Mathf.Clamp01(color.g) * levels) / levels,
                Mathf.Round(Mathf.Clamp01(color.b) * levels) / levels,
                Mathf.Round(Mathf.Clamp01(color.a) * levels) / levels);
        }

        private static int MaterialKey(Color color, float emission)
        {
            int r = Mathf.RoundToInt(color.r * 15f);
            int g = Mathf.RoundToInt(color.g * 15f);
            int b = Mathf.RoundToInt(color.b * 15f);
            int a = Mathf.RoundToInt(color.a * 15f);
            int e = Mathf.RoundToInt(emission * 4f);
            return r | g << 4 | b << 8 | a << 12 | e << 16;
        }

        private static Material ClosestMaterial(Color color, float emission)
        {
            Material best = null;
            float bestScore = float.MaxValue;
            foreach (KeyValuePair<int, Material> entry in Materials)
            {
                if (entry.Value == null) continue;
                Color candidate = MaterialColors[entry.Key];
                float candidateEmission = MaterialEmissions[entry.Key];
                float score = (candidate.r - color.r) * (candidate.r - color.r) +
                    (candidate.g - color.g) * (candidate.g - color.g) +
                    (candidate.b - color.b) * (candidate.b - color.b) +
                    (candidateEmission - emission) * (candidateEmission - emission) * 0.08f;
                if (score >= bestScore) continue;
                bestScore = score;
                best = entry.Value;
            }
            return best;
        }

        public static GameObject Primitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color, Transform parent = null)
        {
            GameObject result = GameObject.CreatePrimitive(type);
            result.name = name;
            result.transform.position = position;
            result.transform.localScale = scale;
            if (parent != null) result.transform.SetParent(parent, true);
            Renderer renderer = result.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = Material(color, 0.3f);
            return result;
        }

        public static LineRenderer Ring(string name, Vector3 position, Color color, float radius, float width, Transform parent = null)
        {
            GameObject go = new GameObject(name);
            go.transform.position = position;
            if (parent != null) go.transform.SetParent(parent, true);
            LineRenderer line = go.AddComponent<LineRenderer>();
            line.sharedMaterial = Material(color, 0.65f);
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = 49;
            line.startWidth = width;
            line.endWidth = width;
            line.startColor = color;
            line.endColor = color;
            for (int i = 0; i < line.positionCount; i++)
            {
                float angle = i / 48f * Mathf.PI * 2f;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0.08f, Mathf.Sin(angle) * radius));
            }
            return line;
        }

        public static LineRenderer Beam(Vector3 from, Vector3 to, Color color, float lifetime = 0.12f)
        {
            GameObject go = new GameObject("Chain Arc");
            LineRenderer line = go.AddComponent<LineRenderer>();
            line.sharedMaterial = Material(color, 1.2f);
            line.positionCount = 4;
            line.startWidth = 0.12f;
            line.endWidth = 0.035f;
            line.startColor = color;
            line.endColor = Color.white;
            Vector3 delta = to - from;
            Vector3 side = Vector3.Cross(Vector3.up, delta.normalized);
            line.SetPosition(0, from + Vector3.up * 0.5f);
            line.SetPosition(1, Vector3.Lerp(from, to, 0.33f) + Vector3.up * 0.65f + side * 0.2f);
            line.SetPosition(2, Vector3.Lerp(from, to, 0.66f) + Vector3.up * 0.35f - side * 0.16f);
            line.SetPosition(3, to + Vector3.up * 0.5f);
            Object.Destroy(go, lifetime);
            return line;
        }

        public static void RemoveCollider(GameObject go)
        {
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) { collider.enabled = false; Object.Destroy(collider); }
        }
    }

    public static class RuntimeAudio
    {
        private static readonly Dictionary<int, AudioClip> Clips = new Dictionary<int, AudioClip>();
        private static float _lastSoundTime;

        public static void PlaySpell(SpellElement element, Vector3 position, bool impact, int generation)
        {
            if (V21AudioDirector.Instance != null &&
                V21AudioDirector.Instance.Play(impact ? "spell_impact" : "spell_cast", position, generation > 2 ? 0.55f : 1f,
                    generation > 2 ? 0.05f : 0.018f)) return;
            float density = ProfileManager.Current.accessibility.effectDensity;
            float minimumGap = Mathf.Lerp(0.08f, 0.018f, density);
            if (Time.unscaledTime - _lastSoundTime < minimumGap || (generation > 2 && density < 0.8f)) return;
            _lastSoundTime = Time.unscaledTime;
            int key = (int)element * 2 + (impact ? 1 : 0);
            AudioClip clip;
            if (!Clips.TryGetValue(key, out clip) || clip == null)
            {
                clip = BuildClip(element, impact);
                Clips[key] = clip;
            }
            AccessibilitySettings settings = ProfileManager.Current.accessibility;
            float volume = Mathf.Clamp(0.28f * density, 0.08f, 0.28f) * Mathf.Clamp01(settings.masterVolume) * Mathf.Clamp01(settings.effectsVolume);
            if (settings.monoAudio && Camera.main != null) position = Camera.main.transform.position;
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        private static AudioClip BuildClip(SpellElement element, bool impact)
        {
            const int sampleRate = 22050;
            float duration = impact ? 0.14f : 0.2f;
            int samples = Mathf.RoundToInt(sampleRate * duration);
            float[] data = new float[samples];
            float baseFrequency = element == SpellElement.Fire ? 135f : element == SpellElement.Frost ? 520f :
                element == SpellElement.Lightning ? 760f : element == SpellElement.Toxic ? 185f : element == SpellElement.Void ? 85f : 330f;
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float envelope = Mathf.Pow(1f - i / (float)samples, impact ? 3f : 1.5f);
                float sweep = impact ? 1f - t * 2.8f : 1f + t * 1.4f;
                data[i] = Mathf.Sin(Mathf.PI * 2f * baseFrequency * sweep * t) * envelope * 0.35f;
            }
            AudioClip clip = AudioClip.Create("Procedural " + element + (impact ? " Impact" : " Cast"), samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
