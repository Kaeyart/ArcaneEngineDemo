using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum PresentationParticlePurpose
    {
        Generic,
        Cast,
        Impact,
        Trail,
        Status,
        Field,
        Shard,
        Smoke,
        Spark,
        Mist,
        Debris,
        Residue
    }

    public struct PresentationParticleRequest
    {
        public PresentationParticlePurpose purpose;
        public Vector3 position;
        public Vector3 direction;
        public Transform follow;
        public ReactionElement signature;
        public ReactionElement primary;
        public Color primaryColor;
        public Color secondaryColor;
        public float radius;
        public float duration;
        public float intensity;
        public float speed;
        public int seed;
        public int count;
        public bool looping;
        public bool worldSpace;
        public PresentationPriority priority;
    }

    public static class PresentationMaterialLibrary2
    {
        private static readonly Dictionary<int, Material> ParticleMaterials =
            new Dictionary<int, Material>();

        private static readonly Dictionary<int, Material> MeshMaterials =
            new Dictionary<int, Material>();

        public static Material Particle(Color color, float alpha)
        {
            int key = ColorKey(color, alpha, 1);
            Material material;

            if (ParticleMaterials.TryGetValue(key, out material) && material != null)
                return material;

            Shader shader = Shader.Find("Particles/Standard Unlit");
            if (shader == null)
                shader = Shader.Find("Legacy Shaders/Particles/Additive");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            material = new Material(shader);
            material.name = "AE2 Particle " + key;
            Color resolved = color;
            resolved.a = alpha;
            material.color = resolved;

            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", resolved);

            if (material.HasProperty("_EmissionColor"))
                material.SetColor("_EmissionColor", color * Mathf.Max(0.6f, alpha));

            ParticleMaterials[key] = material;
            return material;
        }

        public static Material Mesh(Color color, float alpha)
        {
            int key = ColorKey(color, alpha, 2);
            Material material;

            if (MeshMaterials.TryGetValue(key, out material) && material != null)
                return material;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");
            if (shader == null)
                shader = Shader.Find("Standard");

            material = new Material(shader);
            material.name = "AE2 Mesh " + key;
            Color resolved = color;
            resolved.a = alpha;
            material.color = resolved;

            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", resolved);

            if (material.HasProperty("_Surface"))
                material.SetFloat("_Surface", alpha < 0.99f ? 1f : 0f);

            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 0.8f);
            }

            MeshMaterials[key] = material;
            return material;
        }

        private static int ColorKey(Color color, float alpha, int family)
        {
            Color32 value = color;
            int a = Mathf.RoundToInt(Mathf.Clamp01(alpha) * 255f);
            return family * 100000000 +
                   value.r * 1000000 +
                   value.g * 10000 +
                   value.b * 100 +
                   a;
        }
    }

    public static class PresentationParticlePool2
    {
        private static readonly Stack<PooledPresentationParticle2> Available =
            new Stack<PooledPresentationParticle2>();

        private static readonly List<PooledPresentationParticle2> Active =
            new List<PooledPresentationParticle2>();

        private static Transform _root;
        private static int _created;
        private static int _denied;
        private static int _estimatedParticles;

        public static int ActiveCount
        {
            get { Cleanup(); return Active.Count; }
        }

        public static int AvailableCount
        {
            get { return Available.Count; }
        }

        public static int CreatedCount
        {
            get { return _created; }
        }

        public static int DeniedCount
        {
            get { return _denied; }
        }

        public static int EstimatedParticleCount
        {
            get { return Mathf.Max(0, _estimatedParticles); }
        }

        public static PooledPresentationParticle2 Spawn(
            PresentationParticleRequest request)
        {
            Cleanup();

            int requested = Mathf.Clamp(
                request.count <= 0 ? DefaultCount(request) : request.count,
                1,
                180);

            requested = Mathf.Max(
                1,
                Mathf.RoundToInt(
                    requested *
                    Patch200PresentationSettings.Density *
                    Patch200PresentationSettings.EffectIntensity));

            bool reserved = request.priority >= PresentationPriority.Critical;

            if (!reserved &&
                (Active.Count >= Patch200PresentationSettings.MaxParticleSystems ||
                 _estimatedParticles + requested > Patch200PresentationSettings.MaxParticles))
            {
                _denied++;
                return null;
            }

            if (reserved && Active.Count >= Patch200PresentationSettings.MaxParticleSystems + 12)
            {
                ReleaseOldestDecorative();
            }

            PooledPresentationParticle2 effect = null;

            while (Available.Count > 0 && effect == null)
                effect = Available.Pop();

            if (effect == null)
            {
                GameObject gameObject = new GameObject("AE2 Pooled Particle");
                gameObject.transform.SetParent(Root(), false);
                effect = gameObject.AddComponent<PooledPresentationParticle2>();
                _created++;
            }

            request.count = requested;
            effect.gameObject.SetActive(true);
            effect.Begin(request);
            Active.Add(effect);
            _estimatedParticles += requested;
            return effect;
        }

        internal static void Release(PooledPresentationParticle2 effect, int estimate)
        {
            if (effect == null)
                return;

            Active.Remove(effect);
            _estimatedParticles -= Mathf.Max(0, estimate);
            effect.transform.SetParent(Root(), false);
            effect.gameObject.SetActive(false);
            Available.Push(effect);
        }

        public static void ClearAll()
        {
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                PooledPresentationParticle2 effect = Active[i];
                if (effect != null)
                    effect.StopAndRelease();
            }

            Active.Clear();
            _estimatedParticles = 0;
        }

        private static void ReleaseOldestDecorative()
        {
            for (int i = 0; i < Active.Count; i++)
            {
                PooledPresentationParticle2 effect = Active[i];
                if (effect != null && effect.Priority <= PresentationPriority.Normal)
                {
                    effect.StopAndRelease();
                    return;
                }
            }
        }

        private static int DefaultCount(PresentationParticleRequest request)
        {
            float scaled = 12f + request.radius * 6f + request.intensity * 8f;

            switch (request.purpose)
            {
                case PresentationParticlePurpose.Status:
                    scaled *= 0.55f;
                    break;
                case PresentationParticlePurpose.Field:
                case PresentationParticlePurpose.Residue:
                    scaled *= 1.25f;
                    break;
                case PresentationParticlePurpose.Impact:
                case PresentationParticlePurpose.Shard:
                    scaled *= 1.4f;
                    break;
            }

            return Mathf.RoundToInt(scaled);
        }

        private static void Cleanup()
        {
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                if (Active[i] == null)
                    Active.RemoveAt(i);
            }
        }

        private static Transform Root()
        {
            if (_root != null)
                return _root;

            GameObject root = GameObject.Find("Arcane Engine 2.0 Particle Pool");

            if (root == null)
            {
                root = new GameObject("Arcane Engine 2.0 Particle Pool");
                UnityEngine.Object.DontDestroyOnLoad(root);
            }

            _root = root.transform;
            return _root;
        }
    }

    public sealed class PooledPresentationParticle2 : MonoBehaviour
    {
        private ParticleSystem _system;
        private ParticleSystemRenderer _renderer;
        private Transform _follow;
        private Vector3 _followOffset;
        private float _releaseAt;
        private int _estimatedCount;
        private bool _looping;
        private bool _releasing;

        public PresentationPriority Priority { get; private set; }

        private void Awake()
        {
            _system = gameObject.GetComponent<ParticleSystem>();
            if (_system == null)
                _system = gameObject.AddComponent<ParticleSystem>();

            _renderer = gameObject.GetComponent<ParticleSystemRenderer>();
        }

        public void Begin(PresentationParticleRequest request)
        {
            if (_system == null)
                Awake();

            _releasing = false;
            Priority = request.priority;
            _follow = request.follow;
            _followOffset = request.follow == null
                ? Vector3.zero
                : request.position - request.follow.position;

            transform.position = request.position;
            transform.rotation = request.direction.sqrMagnitude > 0.001f
                ? Quaternion.LookRotation(request.direction.normalized, Vector3.up)
                : Quaternion.identity;

            _estimatedCount = request.count;
            _looping = request.looping;
            _releaseAt = Time.unscaledTime + Mathf.Max(0.15f, request.duration + 0.65f);

            ConfigureMain(request);
            ConfigureEmission(request);
            ConfigureShape(request);
            ConfigureVelocity(request);
            ConfigureNoise(request);
            ConfigureColor(request);
            ConfigureRenderer(request);

            _system.Clear(true);
            _system.Play(true);
        }

        public void StopAndRelease()
        {
            if (_releasing)
                return;

            _releasing = true;

            if (_system != null)
            {
                _system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _system.Clear(true);
            }

            PresentationParticlePool2.Release(this, _estimatedCount);
        }

        private void Update()
        {
            if (_follow != null)
                transform.position = _follow.position + _followOffset;

            if (!_looping && Time.unscaledTime >= _releaseAt)
                StopAndRelease();
        }

        private void OnDisable()
        {
            _follow = null;
        }

        private void ConfigureMain(PresentationParticleRequest request)
        {
            ParticleSystem.MainModule main = _system.main;
            main.loop = request.looping;
            main.playOnAwake = false;
            main.duration = Mathf.Max(0.1f, request.duration);
            main.startLifetime = new ParticleSystem.MinMaxCurve(
                Mathf.Clamp(request.duration * 0.35f, 0.18f, 1.2f),
                Mathf.Clamp(request.duration * 0.8f, 0.35f, 2.8f));

            ElementVisualProfile2 profile =
                ElementVisualProfileRegistry2.Get(request.primary);

            float baseSpeed = request.speed > 0f
                ? request.speed
                : profile == null ? 1.5f : profile.particleSpeed;

            main.startSpeed = new ParticleSystem.MinMaxCurve(
                baseSpeed * 0.55f,
                baseSpeed * 1.25f);

            float baseSize = profile == null ? 0.12f : profile.particleSize;
            baseSize *= Mathf.Clamp(request.intensity, 0.25f, 2f);
            main.startSize = new ParticleSystem.MinMaxCurve(
                baseSize * 0.55f,
                baseSize * 1.35f);

            main.simulationSpace = request.worldSpace
                ? ParticleSystemSimulationSpace.World
                : ParticleSystemSimulationSpace.Local;

            main.maxParticles = Mathf.Max(8, request.count * 3);
            _system.randomSeed = unchecked((uint)request.seed);
            main.useUnscaledTime = false;
            main.gravityModifier = Gravity(request.signature);
        }

        private void ConfigureEmission(PresentationParticleRequest request)
        {
            ParticleSystem.EmissionModule emission = _system.emission;
            emission.enabled = true;

            if (request.looping)
            {
                emission.rateOverTime = Mathf.Max(
                    2f,
                    request.count / Mathf.Max(0.25f, request.duration));
                emission.SetBursts(new ParticleSystem.Burst[0]);
            }
            else
            {
                emission.rateOverTime = 0f;
                ParticleSystem.Burst burst = new ParticleSystem.Burst(
                    0f,
                    (short)Mathf.Clamp(request.count, 1, 180));
                emission.SetBursts(new[] { burst });
            }
        }

        private void ConfigureShape(PresentationParticleRequest request)
        {
            ParticleSystem.ShapeModule shape = _system.shape;
            shape.enabled = true;
            shape.radius = Mathf.Max(0.05f, request.radius * 0.5f);
            shape.radiusThickness = 0.35f;

            switch (request.purpose)
            {
                case PresentationParticlePurpose.Trail:
                    shape.shapeType = ParticleSystemShapeType.Cone;
                    shape.angle = 8f;
                    shape.radius = Mathf.Max(0.03f, request.radius * 0.12f);
                    break;

                case PresentationParticlePurpose.Field:
                case PresentationParticlePurpose.Residue:
                    shape.shapeType = ParticleSystemShapeType.Circle;
                    shape.radius = Mathf.Max(0.3f, request.radius);
                    shape.radiusThickness = 1f;
                    break;

                case PresentationParticlePurpose.Status:
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    shape.radius = Mathf.Max(0.18f, request.radius * 0.45f);
                    break;

                default:
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    break;
            }
        }

        private void ConfigureVelocity(PresentationParticleRequest request)
        {
            ParticleSystem.VelocityOverLifetimeModule velocity =
                _system.velocityOverLifetime;
            velocity.enabled = true;

            ReactionElement primary = request.primary;

            if (primary == ReactionElement.Fire)
            {
                velocity.y = new ParticleSystem.MinMaxCurve(0.55f, 1.65f);
            }
            else if (primary == ReactionElement.Cold)
            {
                velocity.y = new ParticleSystem.MinMaxCurve(-0.12f, 0.2f);
            }
            else if (primary == ReactionElement.Void)
            {
                velocity.radial = new ParticleSystem.MinMaxCurve(-1.8f, -0.5f);
            }
            else if (primary == ReactionElement.Blood)
            {
                velocity.y = new ParticleSystem.MinMaxCurve(-0.65f, 0.35f);
            }
            else if (primary == ReactionElement.Toxic)
            {
                velocity.y = new ParticleSystem.MinMaxCurve(0.12f, 0.75f);
            }
            else
            {
                velocity.y = new ParticleSystem.MinMaxCurve(-0.1f, 0.3f);
            }
        }

        private void ConfigureNoise(PresentationParticleRequest request)
        {
            ParticleSystem.NoiseModule noise = _system.noise;
            noise.enabled = true;

            ElementVisualProfile2 profile =
                ElementVisualProfileRegistry2.Get(request.primary);

            float turbulence = profile == null ? 0.3f : profile.turbulence;
            turbulence *= Patch200PresentationSettings.ReducedMotion ? 0.4f : 1f;
            noise.strength = new ParticleSystem.MinMaxCurve(
                turbulence * 0.45f,
                turbulence);
            noise.frequency = request.primary == ReactionElement.Lightning
                ? 1.8f
                : 0.65f;
            noise.scrollSpeed = 0.35f;
            noise.damping = true;
        }

        private void ConfigureColor(PresentationParticleRequest request)
        {
            Color primary = request.primaryColor;
            Color secondary = request.secondaryColor;

            if (primary.a <= 0.001f)
                primary = ElementalReactionCodex.BlendColor(request.signature);

            if (secondary.a <= 0.001f)
            {
                ElementVisualProfile2 profile =
                    ElementVisualProfileRegistry2.Get(request.primary);
                secondary = profile == null ? Color.white : profile.secondary;
            }

            ParticleSystem.ColorOverLifetimeModule color =
                _system.colorOverLifetime;
            color.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.Lerp(Color.white, primary, 0.35f), 0f),
                    new GradientColorKey(primary, 0.35f),
                    new GradientColorKey(secondary, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.95f, 0.08f),
                    new GradientAlphaKey(0.75f, 0.7f),
                    new GradientAlphaKey(0f, 1f)
                });

            color.color = gradient;
        }

        private void ConfigureRenderer(PresentationParticleRequest request)
        {
            if (_renderer == null)
                return;

            Color color = request.primaryColor.a > 0.001f
                ? request.primaryColor
                : ElementalReactionCodex.BlendColor(request.signature);

            _renderer.material = PresentationMaterialLibrary2.Particle(color, 0.85f);
            _renderer.renderMode = ParticleSystemRenderMode.Billboard;
            _renderer.alignment = ParticleSystemRenderSpace.View;
            _renderer.sortMode = ParticleSystemSortMode.Distance;

            if (request.primary == ReactionElement.Cold ||
                request.purpose == PresentationParticlePurpose.Shard ||
                request.purpose == PresentationParticlePurpose.Debris)
            {
                _renderer.renderMode = ParticleSystemRenderMode.Stretch;
                _renderer.velocityScale = 0.35f;
                _renderer.lengthScale = 1.4f;
            }
        }

        private static float Gravity(ReactionElement signature)
        {
            if (ElementalReactionCodex.Contains(signature, ReactionElement.Blood))
                return 0.22f;

            if (ElementalReactionCodex.Contains(signature, ReactionElement.Physical))
                return 0.4f;

            if (ElementalReactionCodex.Contains(signature, ReactionElement.Void))
                return -0.08f;

            return 0f;
        }
    }
}
