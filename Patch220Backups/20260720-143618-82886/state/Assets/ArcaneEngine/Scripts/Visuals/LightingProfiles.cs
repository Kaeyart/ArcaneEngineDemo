using UnityEngine;

namespace ArcaneEngine
{
    public sealed class BiomeLightingProfile
    {
        public Color ambient;
        public Color keyColor;
        public Color rimColor;
        public Color fogColor;
        public Color playerSeparation;
        public Color enemySeparation;
        public Color objectivePriority;
        public Color activeDoorPriority;
        public float keyIntensity;
        public float rimIntensity;
        public float fogDensity;
        public float emissiveMultiplier;
        public float minimumExposure;
        public float maximumExposure;
        public int shadowPolicy;
    }

    public static class BiomeLightingCatalog
    {
        public static BiomeLightingProfile Get(BiomeVisualDefinition biome, bool bossOverlay)
        {
            if (biome == null)
                return new BiomeLightingProfile { ambient = new Color(0.16f, 0.18f, 0.24f), keyColor = Color.white, rimColor = new Color(0.2f, 0.8f, 1f),
                    fogColor = new Color(0.02f, 0.03f, 0.05f), playerSeparation = new Color(0.2f, 0.85f, 1f), enemySeparation = new Color(1f, 0.28f, 0.2f),
                    objectivePriority = new Color(0.3f, 1f, 0.8f), activeDoorPriority = new Color(0.18f, 0.75f, 1f), keyIntensity = 0.92f, rimIntensity = 0.3f,
                    fogDensity = 0f, emissiveMultiplier = 1f, minimumExposure = 0.72f, maximumExposure = 1.18f, shadowPolicy = 1 };
            Color key = Color.Lerp(Color.white, biome.emission, bossOverlay ? 0.46f : 0.3f);
            return new BiomeLightingProfile
            {
                ambient = bossOverlay ? Color.Lerp(biome.ambient, new Color(0.18f, 0.025f, 0.06f), 0.45f) : biome.ambient,
                keyColor = key,
                rimColor = bossOverlay ? Color.Lerp(biome.emission, new Color(1f, 0.04f, 0.2f), 0.62f) : biome.emission,
                fogColor = bossOverlay ? Color.Lerp(biome.fog, new Color(0.08f, 0.012f, 0.035f), 0.48f) : biome.fog,
                playerSeparation = Color.Lerp(biome.emission, Color.white, 0.45f),
                enemySeparation = Color.Lerp(new Color(1f, 0.18f, 0.08f), biome.emission, 0.24f),
                objectivePriority = Color.Lerp(new Color(0.2f, 1f, 0.72f), biome.emission, 0.28f),
                activeDoorPriority = Color.Lerp(new Color(0.18f, 0.72f, 1f), biome.emission, 0.32f),
                keyIntensity = bossOverlay ? 1.18f : biome.id == BiomeVisualId.SunkenArchive ? 0.82f : biome.id == BiomeVisualId.EmberFoundry ? 1.02f : 0.94f,
                rimIntensity = bossOverlay ? 0.52f : 0.34f,
                fogDensity = bossOverlay ? biome.fogDensity * 1.12f : biome.fogDensity,
                emissiveMultiplier = bossOverlay ? 1.2f : biome.id == BiomeVisualId.VenomCistern ? 0.92f : 1f,
                minimumExposure = bossOverlay ? 0.64f : 0.72f,
                maximumExposure = bossOverlay ? 1.12f : 1.22f,
                shadowPolicy = bossOverlay || biome.id == BiomeVisualId.EmberFoundry ? 2 : 1
            };
        }
    }

    public sealed class PriorityLightAnchor : MonoBehaviour
    {
        private Color _color;
        private float _range;
        private float _intensity;
        private int _priority;
        private Light _light;
        private float _retryAt;

        public static PriorityLightAnchor Attach(GameObject target, Color color, float range, float intensity, int priority)
        {
            if (target == null) return null;
            PriorityLightAnchor anchor = target.GetComponent<PriorityLightAnchor>();
            if (anchor == null) anchor = target.AddComponent<PriorityLightAnchor>();
            anchor._color = color; anchor._range = range; anchor._intensity = intensity; anchor._priority = priority;
            anchor.TryAcquire();
            return anchor;
        }

        private void Update()
        {
            if (OwnsLight()) return;
            _light = null;
            if (Time.unscaledTime < _retryAt) return;
            TryAcquire();
        }

        private void TryAcquire()
        {
            _retryAt = Time.unscaledTime + 1.1f;
            _light = ProceduralVisualRuntime.LimitedLight(transform.position + Vector3.up * 0.8f, _color, _range, _intensity, 999f, transform, _priority);
            if (_light != null) _light.transform.localPosition = Vector3.up * 0.8f;
        }

        private void OnDestroy()
        {
            if (!OwnsLight()) { _light = null; return; }
            _light.transform.SetParent(null, true);
            ProceduralVisualRuntime.Release(_light.gameObject);
            _light = null;
        }

        private bool OwnsLight()
        {
            if (_light == null || !_light.gameObject.activeInHierarchy || _light.transform.parent != transform) return false;
            PooledVisualMarker marker = _light.GetComponent<PooledVisualMarker>();
            return marker != null && marker.InUse;
        }
    }
}
