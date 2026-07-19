using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class PresentationPulseSequence2 : MonoBehaviour
    {
        private Vector3 _position;
        private ReactionElement _signature;
        private float _startRadius;
        private float _endRadius;
        private int _remaining;
        private int _total;
        private float _interval;
        private float _next;
        private PresentationPriority _priority;
        private int _seed;
        private bool _reverse;

        public static PresentationPulseSequence2 Spawn(
            Vector3 position,
            ReactionElement signature,
            float startRadius,
            float endRadius,
            int count,
            float interval,
            PresentationPriority priority,
            int seed,
            bool reverse)
        {
            GameObject gameObject = new GameObject("AE2 Pulse Sequence");
            gameObject.transform.position = position;
            PresentationPulseSequence2 effect = gameObject.AddComponent<PresentationPulseSequence2>();
            effect._position = position;
            effect._signature = signature;
            effect._startRadius = Mathf.Max(0.1f, startRadius);
            effect._endRadius = Mathf.Max(0.1f, endRadius);
            effect._remaining = effect._total = Mathf.Clamp(count, 1, 12);
            effect._interval = Mathf.Max(0.05f, interval);
            effect._next = Time.time;
            effect._priority = priority;
            effect._seed = seed;
            effect._reverse = reverse;
            return effect;
        }

        private void Update()
        {
            if (Time.time < _next)
                return;

            int index = _total - _remaining;
            float t = _total <= 1 ? 1f : index / (float)(_total - 1);
            float radius = _reverse
                ? Mathf.Lerp(_endRadius, _startRadius, t)
                : Mathf.Lerp(_startRadius, _endRadius, t);

            ReactionElement element = ElementAt(index);
            Color color = ElementalReactionCodex.ColorFor(element);

            PresentationGeometry2.Ring(
                _position,
                color,
                radius,
                Mathf.Clamp(0.035f + index * 0.006f, 0.035f, 0.09f),
                Mathf.Max(0.18f, _interval * 1.8f),
                _priority,
                !_reverse);

            PresentationParticlePool2.Spawn(new PresentationParticleRequest
            {
                purpose = PresentationParticlePurpose.Impact,
                position = _position + Vector3.up * 0.12f,
                direction = Vector3.up,
                signature = _signature,
                primary = element,
                primaryColor = color,
                secondaryColor = Color.white,
                radius = radius,
                duration = Mathf.Max(0.22f, _interval * 2f),
                intensity = 0.55f,
                speed = 1.2f,
                seed = _seed + index * 31,
                count = 5 + index,
                priority = _priority,
                worldSpace = true
            });

            _remaining--;
            _next = Time.time + _interval;

            if (_remaining <= 0)
                Destroy(gameObject, _interval + 0.1f);
        }

        private ReactionElement ElementAt(int index)
        {
            int count = Mathf.Max(1, ElementalReactionCodex.CountBits(_signature));
            int target = index % count;
            int current = 0;

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(_signature))
            {
                if (current == target)
                    return element;
                current++;
            }

            return ReactionElement.Physical;
        }
    }

    public sealed class PresentationArcSweep2 : MonoBehaviour
    {
        private LineRenderer _line;
        private Vector3 _center;
        private Vector3 _forward;
        private float _radius;
        private float _remaining;
        private float _lifetime;
        private Color _color;

        public static PresentationArcSweep2 Spawn(
            Vector3 center,
            Vector3 forward,
            float radius,
            Color color,
            float lifetime,
            PresentationPriority priority,
            int seed)
        {
            GameObject gameObject = new GameObject("AE2 Arc Sweep");
            PresentationArcSweep2 effect = gameObject.AddComponent<PresentationArcSweep2>();
            effect._center = center;
            effect._forward = forward.sqrMagnitude < 0.01f ? Vector3.forward : forward.normalized;
            effect._radius = radius;
            effect._color = color;
            effect._remaining = effect._lifetime = Mathf.Max(0.1f, lifetime);
            effect.Build();
            return effect;
        }

        private void Build()
        {
            _line = gameObject.AddComponent<LineRenderer>();
            _line.positionCount = 18;
            _line.useWorldSpace = true;
            _line.startWidth = 0.09f;
            _line.endWidth = 0.025f;
            _line.material = PresentationMaterialLibrary2.Mesh(_color, 0.9f);
            _line.startColor = _color;
            _line.endColor = _color;

            float centerAngle = Mathf.Atan2(_forward.z, _forward.x);

            for (int i = 0; i < _line.positionCount; i++)
            {
                float t = i / (float)(_line.positionCount - 1);
                float angle = centerAngle + Mathf.Lerp(-0.8f, 0.8f, t);
                _line.SetPosition(
                    i,
                    _center + new Vector3(
                        Mathf.Cos(angle) * _radius,
                        0.05f,
                        Mathf.Sin(angle) * _radius));
            }
        }

        private void Update()
        {
            _remaining -= Time.deltaTime;
            float t = Mathf.Clamp01(1f - _remaining / _lifetime);
            Color color = _color;
            color.a = 1f - t;
            _line.startColor = color;
            _line.endColor = color;
            transform.Rotate(Vector3.up, Time.deltaTime * 90f);

            if (_remaining <= 0f)
                Destroy(gameObject);
        }
    }

    public static class PresentationRadialMarkers2
    {
        public static void Spawn(
            Vector3 center,
            ReactionElement signature,
            int count,
            float radius,
            float lifetime,
            PresentationPriority priority,
            int seed)
        {
            count = Mathf.Clamp(count, 2, 14);

            for (int i = 0; i < count; i++)
            {
                float angle = (i / (float)count) * Mathf.PI * 2f + seed * 0.001f;
                ReactionElement element = ElementAt(signature, i);
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(element);
                Vector3 point = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius);

                PresentationGeometry2.Primitive(
                    "AE2 Radial Marker",
                    profile == null ? PrimitiveType.Sphere : profile.accentShape,
                    point,
                    Vector3.one * Mathf.Clamp(radius * 0.13f, 0.06f, 0.24f),
                    profile == null ? Color.white : profile.primary,
                    lifetime,
                    priority,
                    false,
                    Vector3.zero);
            }
        }

        private static ReactionElement ElementAt(ReactionElement signature, int index)
        {
            int count = Mathf.Max(1, ElementalReactionCodex.CountBits(signature));
            int target = index % count;
            int current = 0;

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(signature))
            {
                if (current == target)
                    return element;
                current++;
            }

            return ReactionElement.Physical;
        }
    }

    public sealed class PresentationOrbitalNodesVisual2 : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _fixedCenter;
        private ReactionElement _signature;
        private float _radius;
        private float _remaining;
        private float _angle;
        private int _count;
        private int _seed;
        private PresentationPriority _priority;
        private readonly List<GameObject> _nodes = new List<GameObject>();

        public static PresentationOrbitalNodesVisual2 Spawn(
            Transform target,
            Vector3 center,
            ReactionElement signature,
            int count,
            float radius,
            float duration,
            PresentationPriority priority,
            int seed)
        {
            GameObject gameObject = new GameObject("AE2 Orbital Nodes");
            gameObject.transform.position = center;
            PresentationOrbitalNodesVisual2 effect =
                gameObject.AddComponent<PresentationOrbitalNodesVisual2>();
            effect._target = target;
            effect._fixedCenter = center;
            effect._signature = signature;
            effect._count = Mathf.Clamp(count, 2, 12);
            effect._radius = Mathf.Max(0.25f, radius);
            effect._remaining = Mathf.Max(0.2f, duration);
            effect._seed = seed;
            effect._priority = priority;
            effect.Build();
            return effect;
        }

        private void Build()
        {
            for (int i = 0; i < _count; i++)
            {
                ReactionElement element = PresentationEffectUtility2.ElementAt(_signature, i);
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(element);
                GameObject node = GameObject.CreatePrimitive(
                    profile == null ? PrimitiveType.Sphere : profile.accentShape);
                node.name = "AE2 Orbital Node " + element;
                Collider collider = node.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);
                Renderer renderer = node.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = PresentationMaterialLibrary2.Mesh(
                        profile == null ? Color.white : profile.primary,
                        0.85f);
                node.transform.SetParent(transform, false);
                node.transform.localScale = Vector3.one * Mathf.Clamp(_radius * 0.12f, 0.07f, 0.3f);
                _nodes.Add(node);
            }
        }

        private void Update()
        {
            _remaining -= Time.deltaTime;
            Vector3 center = _target == null ? _fixedCenter : _target.position + Vector3.up * 0.55f;
            transform.position = center;
            _angle += Time.deltaTime *
                      (Patch200PresentationSettings.ReducedMotion ? 0.65f : 2.1f);

            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i] == null)
                    continue;

                float angle = _angle + i / (float)_nodes.Count * Mathf.PI * 2f;
                _nodes[i].transform.localPosition = new Vector3(
                    Mathf.Cos(angle) * _radius,
                    Mathf.Sin(angle * 2f + _seed) * _radius * 0.18f,
                    Mathf.Sin(angle) * _radius);
                _nodes[i].transform.Rotate(Vector3.one, Time.deltaTime * 95f);

                if (i > 0 && i % 2 == 0 && Time.frameCount % 8 == 0)
                {
                    PresentationGeometry2.Beam(
                        _nodes[i - 1].transform.position,
                        _nodes[i].transform.position,
                        ElementalReactionCodex.BlendColor(_signature),
                        0.025f,
                        0.08f,
                        PresentationPriority.Decorative,
                        ElementalReactionCodex.Contains(_signature, ReactionElement.Lightning),
                        _seed + i);
                }
            }

            if (_remaining <= 0f)
                Destroy(gameObject);
        }
    }

    public sealed class PresentationDelayedVisual2 : MonoBehaviour
    {
        private Vector3 _position;
        private ReactionElement _signature;
        private float _delay;
        private float _radius;
        private float _started;
        private float _nextRing;
        private PresentationPriority _priority;
        private int _seed;

        public static PresentationDelayedVisual2 Spawn(
            Vector3 position,
            ReactionElement signature,
            float delay,
            float radius,
            PresentationPriority priority,
            int seed)
        {
            GameObject gameObject = new GameObject("AE2 Delayed Visual");
            PresentationDelayedVisual2 effect = gameObject.AddComponent<PresentationDelayedVisual2>();
            effect._position = position;
            effect._signature = signature;
            effect._delay = Mathf.Max(0.08f, delay);
            effect._radius = Mathf.Max(0.3f, radius);
            effect._started = Time.time;
            effect._nextRing = Time.time;
            effect._priority = priority;
            effect._seed = seed;
            return effect;
        }

        private void Update()
        {
            float progress = Mathf.Clamp01((Time.time - _started) / _delay);

            if (Time.time >= _nextRing)
            {
                _nextRing = Time.time + Mathf.Lerp(0.22f, 0.055f, progress);
                PresentationGeometry2.Ring(
                    _position,
                    ElementalReactionCodex.BlendColor(_signature),
                    Mathf.Lerp(_radius, _radius * 0.2f, progress),
                    Mathf.Lerp(0.025f, 0.07f, progress),
                    0.14f,
                    _priority,
                    false);
            }

            if (progress >= 1f)
            {
                PresentationGeometry2.Burst(
                    _position,
                    _signature,
                    _radius,
                    0.42f,
                    1f,
                    _priority,
                    _seed);
                Destroy(gameObject);
            }
        }
    }

    public static class PresentationChainVisual2
    {
        public static void FromPoint(
            Vector3 start,
            ReactionElement signature,
            int count,
            float radius,
            float lifetime,
            PresentationPriority priority,
            int seed)
        {
            List<EnemyController> targets = ClosestEnemies(start, radius, count);
            Vector3 from = start + Vector3.up * 0.3f;
            Color color = ElementalReactionCodex.BlendColor(signature);

            for (int i = 0; i < targets.Count; i++)
            {
                EnemyController target = targets[i];
                Vector3 to = target.DamagePoint;
                PresentationGeometry2.Beam(
                    from,
                    to,
                    color,
                    Mathf.Max(0.025f, 0.07f * Mathf.Pow(0.85f, i)),
                    lifetime,
                    priority,
                    true,
                    seed + i * 53);
                PresentationTransferVisual2.Spawn(
                    from,
                    to,
                    signature,
                    lifetime,
                    priority,
                    seed + i);
                from = to;
            }
        }

        private static List<EnemyController> ClosestEnemies(
            Vector3 position,
            float radius,
            int count)
        {
            List<EnemyController> results = new List<EnemyController>();
            GameWorld world = GameWorld.Instance;

            if (world == null || world.Enemies == null)
                return results;

            float radiusSquared = radius * radius;

            foreach (EnemyController enemy in world.Enemies)
            {
                if (enemy == null || enemy.IsDead)
                    continue;

                Vector3 delta = enemy.transform.position - position;
                delta.y = 0f;

                if (delta.sqrMagnitude <= radiusSquared)
                    results.Add(enemy);
            }

            results.Sort((a, b) =>
                (a.transform.position - position).sqrMagnitude.CompareTo(
                    (b.transform.position - position).sqrMagnitude));

            if (results.Count > count)
                results.RemoveRange(count, results.Count - count);

            return results;
        }
    }

    public sealed class PresentationTransferVisual2 : MonoBehaviour
    {
        private Vector3 _from;
        private Vector3 _to;
        private ReactionElement _signature;
        private float _started;
        private float _duration;
        private GameObject _body;

        public static PresentationTransferVisual2 Spawn(
            Vector3 from,
            Vector3 to,
            ReactionElement signature,
            float duration,
            PresentationPriority priority,
            int seed)
        {
            GameObject root = new GameObject("AE2 Transfer");
            PresentationTransferVisual2 effect = root.AddComponent<PresentationTransferVisual2>();
            effect._from = from;
            effect._to = to;
            effect._signature = signature;
            effect._duration = Mathf.Max(0.08f, duration);
            effect._started = Time.time;
            effect._body = PresentationGeometry2.Primitive(
                "AE2 Transfer Body",
                PrimitiveType.Sphere,
                from,
                Vector3.one * 0.1f,
                ElementalReactionCodex.BlendColor(signature),
                effect._duration + 0.1f,
                priority,
                false,
                Vector3.zero);
            return effect;
        }

        public static void SpawnRadial(
            Vector3 center,
            ReactionElement signature,
            float radius,
            int count,
            float duration,
            PresentationPriority priority,
            int seed)
        {
            count = Mathf.Clamp(count, 2, 12);

            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f + seed * 0.002f;
                Vector3 to = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0.35f,
                    Mathf.Sin(angle) * radius);
                Spawn(center + Vector3.up * 0.35f, to, signature, duration, priority, seed + i);
            }
        }

        private void Update()
        {
            float t = Mathf.Clamp01((Time.time - _started) / _duration);

            if (_body != null)
            {
                Vector3 point = Vector3.Lerp(_from, _to, t);
                point.y += Mathf.Sin(t * Mathf.PI) * 0.35f;
                _body.transform.position = point;
            }

            if (t >= 1f)
                Destroy(gameObject);
        }
    }

    public static class PresentationMotionStreaks2
    {
        public static void Spawn(
            Vector3 from,
            Vector3 to,
            ReactionElement signature,
            int count,
            float lifetime,
            PresentationPriority priority,
            int seed)
        {
            count = Mathf.Clamp(count, 1, 14);
            Vector3 direction = to - from;
            Vector3 side = Vector3.Cross(direction.normalized, Vector3.up);

            for (int i = 0; i < count; i++)
            {
                float offset = (i - (count - 1) * 0.5f) * 0.08f;
                PresentationGeometry2.Beam(
                    from + side * offset + Vector3.up * (i % 3) * 0.06f,
                    to + side * offset + Vector3.up * (i % 3) * 0.06f,
                    ElementalReactionCodex.ColorFor(PresentationEffectUtility2.ElementAt(signature, i)),
                    0.025f + i % 2 * 0.012f,
                    lifetime,
                    priority,
                    ElementalReactionCodex.Contains(signature, ReactionElement.Lightning),
                    seed + i);
            }
        }

        public static void SpawnInward(
            Vector3 center,
            ReactionElement signature,
            float radius,
            int count,
            float lifetime,
            PresentationPriority priority,
            int seed)
        {
            SpawnRadial(center, signature, radius, count, lifetime, priority, seed, true);
        }

        public static void SpawnOutward(
            Vector3 center,
            ReactionElement signature,
            float radius,
            int count,
            float lifetime,
            PresentationPriority priority,
            int seed)
        {
            SpawnRadial(center, signature, radius, count, lifetime, priority, seed, false);
        }

        private static void SpawnRadial(
            Vector3 center,
            ReactionElement signature,
            float radius,
            int count,
            float lifetime,
            PresentationPriority priority,
            int seed,
            bool inward)
        {
            count = Mathf.Clamp(count, 3, 24);

            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f + seed * 0.001f;
                Vector3 edge = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0.25f + (i % 3) * 0.12f,
                    Mathf.Sin(angle) * radius);
                Vector3 middle = center + Vector3.up * 0.35f;
                PresentationGeometry2.Beam(
                    inward ? edge : middle,
                    inward ? middle : edge,
                    ElementalReactionCodex.ColorFor(PresentationEffectUtility2.ElementAt(signature, i)),
                    0.028f,
                    lifetime,
                    priority,
                    false,
                    seed + i);
            }
        }
    }

    public static class PresentationEffectUtility2
    {
        public static ReactionElement ElementAt(ReactionElement signature, int index)
        {
            int count = Mathf.Max(1, ElementalReactionCodex.CountBits(signature));
            int target = index % count;
            int current = 0;

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(signature))
            {
                if (current == target)
                    return element;
                current++;
            }

            return ReactionElement.Physical;
        }
    }
}
