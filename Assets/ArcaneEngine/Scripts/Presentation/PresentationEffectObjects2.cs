using UnityEngine;

namespace ArcaneEngine
{
    public static class PresentationShardVisual2
    {
        public static void SpawnNova(
            Vector3 center,
            ReactionElement signature,
            int count,
            float radius,
            float duration,
            PresentationPriority priority,
            int seed)
        {
            count = Mathf.Clamp(count, 4, 36);
            duration = Mathf.Max(0.18f, duration);

            for (int i = 0; i < count; i++)
            {
                float golden = i * 2.39996323f + seed * 0.001f;
                float y = Mathf.Lerp(-0.25f, 0.85f, (i % 7) / 6f);
                Vector3 direction = new Vector3(
                    Mathf.Cos(golden),
                    y,
                    Mathf.Sin(golden)).normalized;

                ReactionElement element = PresentationEffectUtility2.ElementAt(signature, i);
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(element);
                PrimitiveType type = element == ReactionElement.Cold
                    ? PrimitiveType.Cube
                    : element == ReactionElement.Physical
                        ? PrimitiveType.Cube
                        : profile == null ? PrimitiveType.Sphere : profile.accentShape;

                Vector3 scale = new Vector3(
                    Mathf.Clamp(radius * 0.035f, 0.04f, 0.18f),
                    Mathf.Clamp(radius * 0.1f, 0.1f, 0.52f),
                    Mathf.Clamp(radius * 0.035f, 0.04f, 0.18f));

                PresentationGeometry2.Primitive(
                    "AE2 Reaction Shard " + element,
                    type,
                    center,
                    scale,
                    profile == null ? ElementalReactionCodex.ColorFor(element) : profile.secondary,
                    duration,
                    priority,
                    false,
                    direction * radius / duration);
            }
        }
    }

    public static class PresentationFractureVisual2
    {
        public static void Spawn(
            Vector3 center,
            ReactionElement signature,
            float radius,
            int count,
            float duration,
            PresentationPriority priority,
            int seed)
        {
            count = Mathf.Clamp(count, 4, 24);
            Color color = ElementalReactionCodex.Contains(signature, ReactionElement.Cold)
                ? ElementalReactionCodex.ColorFor(ReactionElement.Cold)
                : ElementalReactionCodex.Contains(signature, ReactionElement.Physical)
                    ? ElementalReactionCodex.ColorFor(ReactionElement.Physical)
                    : ElementalReactionCodex.BlendColor(signature);

            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f + seed * 0.004f;
                float inner = radius * Mathf.Lerp(0.08f, 0.3f, (i % 4) / 3f);
                float outer = radius * Mathf.Lerp(0.55f, 1f, ((i * 3) % 7) / 6f);
                Vector3 from = center + new Vector3(
                    Mathf.Cos(angle) * inner,
                    0.04f,
                    Mathf.Sin(angle) * inner);
                Vector3 to = center + new Vector3(
                    Mathf.Cos(angle + (i % 2 == 0 ? 0.08f : -0.08f)) * outer,
                    0.045f,
                    Mathf.Sin(angle + (i % 2 == 0 ? 0.08f : -0.08f)) * outer);

                PresentationGeometry2.Beam(
                    from,
                    to,
                    color,
                    0.025f + (i % 3) * 0.008f,
                    duration,
                    priority,
                    true,
                    seed + i);
            }
        }
    }

    public static class PresentationExecuteVisual2
    {
        public static void Spawn(
            Vector3 center,
            ReactionElement signature,
            float radius,
            float duration,
            PresentationPriority priority,
            int seed)
        {
            Color color = ElementalReactionCodex.BlendColor(signature);
            Vector3 top = center + Vector3.up * Mathf.Max(1.2f, radius * 0.8f);
            Vector3 bottom = center + Vector3.down * 0.08f;

            PresentationGeometry2.Beam(
                top + Vector3.left * radius * 0.2f,
                bottom + Vector3.right * radius * 0.2f,
                color,
                0.12f,
                duration,
                priority,
                false,
                seed);

            PresentationGeometry2.Beam(
                top + Vector3.right * radius * 0.2f,
                bottom + Vector3.left * radius * 0.2f,
                Color.Lerp(color, Color.white, 0.5f),
                0.07f,
                duration,
                priority,
                false,
                seed + 1);

            PresentationGeometry2.Ring(
                center,
                color,
                radius * 0.55f,
                0.075f,
                duration,
                priority,
                true);
        }
    }

    public sealed class PresentationThermalCycleVisual2 : MonoBehaviour
    {
        private Vector3 _position;
        private float _radius;
        private int _remaining;
        private int _total;
        private float _interval;
        private float _next;
        private PresentationPriority _priority;
        private int _seed;

        public static PresentationThermalCycleVisual2 Spawn(
            Vector3 position,
            float radius,
            int count,
            float interval,
            PresentationPriority priority,
            int seed)
        {
            GameObject gameObject = new GameObject("AE2 Thermal Cycle");
            PresentationThermalCycleVisual2 effect = gameObject.AddComponent<PresentationThermalCycleVisual2>();
            effect._position = position;
            effect._radius = radius;
            effect._remaining = effect._total = Mathf.Clamp(count, 2, 10);
            effect._interval = Mathf.Max(0.07f, interval);
            effect._next = Time.time;
            effect._priority = priority;
            effect._seed = seed;
            return effect;
        }

        private void Update()
        {
            if (Time.time < _next)
                return;

            int index = _total - _remaining;
            ReactionElement element = index % 2 == 0
                ? ReactionElement.Fire
                : ReactionElement.Cold;
            Color color = ElementalReactionCodex.ColorFor(element);
            float radius = _radius * Mathf.Lerp(0.35f, 1f, (index + 1f) / _total);

            PresentationGeometry2.Ring(
                _position,
                color,
                radius,
                0.06f,
                Mathf.Max(0.16f, _interval * 1.5f),
                _priority,
                index % 2 == 0);

            PresentationParticlePool2.Spawn(new PresentationParticleRequest
            {
                purpose = PresentationParticlePurpose.Impact,
                position = _position + Vector3.up * 0.25f,
                signature = element,
                primary = element,
                primaryColor = color,
                secondaryColor = Color.white,
                radius = radius,
                duration = _interval * 1.5f,
                intensity = 0.8f,
                seed = _seed + index,
                count = 10,
                priority = _priority,
                worldSpace = true
            });

            _remaining--;
            _next = Time.time + _interval;

            if (_remaining <= 0)
                Destroy(gameObject, _interval);
        }
    }

    public static class PresentationSplitResidues2
    {
        public static void Spawn(
            Vector3 center,
            ReactionElement signature,
            float radius,
            float duration,
            int count,
            PresentationPriority priority,
            int seed)
        {
            count = Mathf.Clamp(count, 2, 12);
            float orbit = Mathf.Max(0.8f, radius * 0.55f);
            float fieldRadius = Mathf.Max(0.45f, radius * 0.28f);

            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f + seed * 0.003f;
                Vector3 point = center + new Vector3(
                    Mathf.Cos(angle) * orbit,
                    0f,
                    Mathf.Sin(angle) * orbit);
                ReactionElement element = PresentationEffectUtility2.ElementAt(signature, i);
                ReactionElement fieldSignature = element;

                if (ElementalReactionCodex.CountBits(signature) > 2)
                    fieldSignature |= PresentationEffectUtility2.ElementAt(signature, i + 1);

                PresentationResidue2.SpawnStandalone(
                    point,
                    fieldSignature,
                    fieldRadius,
                    duration,
                    seed + i * 71,
                    priority);
            }
        }
    }

    public static class PresentationTrailResidue2
    {
        public static void Spawn(
            Vector3 center,
            ReactionElement signature,
            float length,
            float duration,
            int count,
            PresentationPriority priority,
            int seed)
        {
            count = Mathf.Clamp(count, 2, 10);
            float angle = (seed % 360) * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 side = Vector3.Cross(direction, Vector3.up);

            for (int i = 0; i < count; i++)
            {
                float t = count <= 1 ? 0f : i / (float)(count - 1);
                Vector3 point = center + direction * Mathf.Lerp(-length * 0.5f, length * 0.5f, t);
                point += side * Mathf.Sin(t * Mathf.PI * 2f) * length * 0.08f;
                PresentationResidue2.SpawnStandalone(
                    point,
                    signature,
                    Mathf.Max(0.35f, length * 0.14f),
                    duration,
                    seed + i,
                    priority);
            }
        }
    }

    public static class PresentationStackDetonationVisual2
    {
        public static void Spawn(
            Vector3 center,
            ReactionElement signature,
            float radius,
            float duration,
            float intensity,
            PresentationPriority priority,
            int seed)
        {
            int elements = Mathf.Max(1, ElementalReactionCodex.CountBits(signature));

            for (int i = 0; i < elements; i++)
            {
                ReactionElement element = PresentationEffectUtility2.ElementAt(signature, i);
                float delay = i * 0.08f;
                PresentationDelayedVisual2.Spawn(
                    center,
                    element,
                    delay + 0.08f,
                    radius * Mathf.Lerp(0.45f, 1f, (i + 1f) / elements),
                    priority,
                    seed + i);
            }

            PresentationMotionStreaks2.SpawnInward(
                center,
                signature,
                radius,
                8 + elements * 2,
                duration,
                priority,
                seed);
        }
    }
}
