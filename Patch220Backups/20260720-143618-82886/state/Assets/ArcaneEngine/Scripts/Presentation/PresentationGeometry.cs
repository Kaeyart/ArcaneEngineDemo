using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public static class PresentationGeometry2
    {
        private static readonly Stack<GameObject> Rings = new Stack<GameObject>();
        private static readonly Stack<GameObject> Beams = new Stack<GameObject>();
        private static readonly Stack<GameObject> Primitives = new Stack<GameObject>();
        private static readonly List<PooledPresentationGeometry2> Active =
            new List<PooledPresentationGeometry2>();

        public static int ActiveCount
        {
            get
            {
                Cleanup();
                return Active.Count;
            }
        }

        public static LineRenderer Ring(
            Vector3 position,
            Color color,
            float radius,
            float width,
            float lifetime,
            PresentationPriority priority,
            bool expanding)
        {
            GameObject gameObject = Acquire(Rings, "AE2 Ring");
            LineRenderer line = gameObject.GetComponent<LineRenderer>();

            if (line == null)
                line = gameObject.AddComponent<LineRenderer>();

            int segments = Patch200PresentationSettings.Quality == PresentationQuality.Low
                ? 28
                : Patch200PresentationSettings.Quality == PresentationQuality.Medium ? 42 : 64;

            line.loop = true;
            line.positionCount = segments;
            line.useWorldSpace = true;
            line.startWidth = width;
            line.endWidth = width;
            line.startColor = color;
            line.endColor = color;
            line.material = PresentationMaterialLibrary2.Mesh(color, 0.9f);
            line.numCapVertices = 2;
            line.numCornerVertices = 2;

            for (int i = 0; i < segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                line.SetPosition(
                    i,
                    position + new Vector3(
                        Mathf.Cos(angle) * radius,
                        0.035f,
                        Mathf.Sin(angle) * radius));
            }

            PooledPresentationGeometry2 lifetimeComponent =
                ConfigureLifetime(gameObject, lifetime, Rings, priority);

            lifetimeComponent.BeginRing(
                position,
                radius,
                expanding ? radius * 1.8f : radius,
                width,
                color,
                lifetime);

            return line;
        }

        public static LineRenderer Beam(
            Vector3 from,
            Vector3 to,
            Color color,
            float width,
            float lifetime,
            PresentationPriority priority,
            bool jagged,
            int seed)
        {
            GameObject gameObject = Acquire(Beams, "AE2 Beam");
            LineRenderer line = gameObject.GetComponent<LineRenderer>();

            if (line == null)
                line = gameObject.AddComponent<LineRenderer>();

            int segments = jagged ? 9 : 2;
            line.loop = false;
            line.positionCount = segments;
            line.useWorldSpace = true;
            line.startWidth = width;
            line.endWidth = Mathf.Max(0.015f, width * 0.62f);
            line.startColor = color;
            line.endColor = Color.Lerp(color, Color.white, 0.28f);
            line.material = PresentationMaterialLibrary2.Mesh(color, 0.95f);
            line.numCapVertices = 2;

            Vector3 direction = to - from;
            Vector3 side = Vector3.Cross(direction.normalized, Vector3.up);
            if (side.sqrMagnitude < 0.01f)
                side = Vector3.right;

            System.Random random = new System.Random(seed);

            for (int i = 0; i < segments; i++)
            {
                float t = segments <= 1 ? 0f : i / (float)(segments - 1);
                Vector3 point = Vector3.Lerp(from, to, t);

                if (jagged && i > 0 && i < segments - 1)
                {
                    float offset = ((float)random.NextDouble() * 2f - 1f) *
                                   Mathf.Min(0.36f, direction.magnitude * 0.06f);
                    point += side * offset;
                    point += Vector3.up * (((float)random.NextDouble() * 2f - 1f) * 0.12f);
                }

                line.SetPosition(i, point);
            }

            ConfigureLifetime(gameObject, lifetime, Beams, priority)
                .BeginStatic(lifetime);

            return line;
        }

        public static GameObject Primitive(
            string name,
            PrimitiveType type,
            Vector3 position,
            Vector3 scale,
            Color color,
            float lifetime,
            PresentationPriority priority,
            bool expand,
            Vector3 velocity)
        {
            GameObject gameObject = AcquirePrimitive(type);
            gameObject.name = name;
            gameObject.transform.position = position;
            gameObject.transform.rotation = RandomRotation(name, position);
            gameObject.transform.localScale = scale;

            Collider collider = gameObject.GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;

            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = PresentationMaterialLibrary2.Mesh(color, 0.78f);

            ConfigureLifetime(gameObject, lifetime, Primitives, priority)
                .BeginMoving(lifetime, expand, velocity, scale);

            return gameObject;
        }

        public static void Burst(
            Vector3 position,
            ReactionElement signature,
            float radius,
            float lifetime,
            float intensity,
            PresentationPriority priority,
            int seed)
        {
            int elements = Mathf.Max(1, ElementalReactionCodex.CountBits(signature));
            int count = Mathf.Clamp(
                Mathf.RoundToInt((5 + elements * 2) * Patch200PresentationSettings.Density),
                4,
                26);

            List<ReactionElement> elementList = new List<ReactionElement>(
                ElementalReactionCodex.Enumerate(signature));

            if (elementList.Count == 0)
                elementList.Add(ReactionElement.Physical);

            for (int i = 0; i < count; i++)
            {
                float angle = (i / (float)count) * Mathf.PI * 2f + seed * 0.0137f;
                ReactionElement element = elementList[i % elementList.Count];
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(element);
                Color color = profile == null
                    ? ElementalReactionCodex.BlendColor(signature)
                    : i % 2 == 0 ? profile.primary : profile.secondary;

                Vector3 direction = new Vector3(
                    Mathf.Cos(angle),
                    Mathf.Lerp(0.18f, 0.85f, (i % 5) / 4f),
                    Mathf.Sin(angle)).normalized;

                PrimitiveType type = profile == null
                    ? PrimitiveType.Sphere
                    : profile.accentShape;

                float size = Mathf.Lerp(0.08f, 0.26f, intensity / 2f);
                Primitive(
                    "AE2 Burst " + element,
                    type,
                    position + Vector3.up * 0.15f,
                    Vector3.one * size,
                    color,
                    lifetime,
                    priority,
                    false,
                    direction * radius / Mathf.Max(0.1f, lifetime));
            }

            Ring(
                position,
                ElementalReactionCodex.BlendColor(signature),
                Mathf.Max(0.25f, radius * 0.2f),
                Mathf.Clamp(0.035f + intensity * 0.018f, 0.035f, 0.11f),
                lifetime,
                priority,
                true);
        }

        internal static void Release(
            PooledPresentationGeometry2 component,
            Stack<GameObject> pool)
        {
            if (component == null)
                return;

            Active.Remove(component);
            component.gameObject.SetActive(false);
            component.transform.SetParent(Root(), false);
            pool.Push(component.gameObject);
        }

        private static GameObject Acquire(
            Stack<GameObject> pool,
            string name)
        {
            GameObject gameObject = null;

            while (pool.Count > 0 && gameObject == null)
                gameObject = pool.Pop();

            if (gameObject == null)
                gameObject = new GameObject(name);

            gameObject.SetActive(true);
            gameObject.name = name;
            gameObject.transform.SetParent(Root(), false);
            return gameObject;
        }

        private static GameObject AcquirePrimitive(PrimitiveType type)
        {
            GameObject gameObject = null;

            while (Primitives.Count > 0 && gameObject == null)
            {
                GameObject candidate = Primitives.Pop();
                PresentationPrimitiveKind2 kind =
                    candidate == null ? null : candidate.GetComponent<PresentationPrimitiveKind2>();

                if (kind != null && kind.type == type)
                    gameObject = candidate;
                else if (candidate != null)
                    UnityEngine.Object.Destroy(candidate);
            }

            if (gameObject == null)
            {
                gameObject = GameObject.CreatePrimitive(type);
                PresentationPrimitiveKind2 kind = gameObject.AddComponent<PresentationPrimitiveKind2>();
                kind.type = type;
            }

            gameObject.SetActive(true);
            gameObject.transform.SetParent(Root(), false);
            return gameObject;
        }

        private static PooledPresentationGeometry2 ConfigureLifetime(
            GameObject gameObject,
            float lifetime,
            Stack<GameObject> pool,
            PresentationPriority priority)
        {
            PooledPresentationGeometry2 component =
                gameObject.GetComponent<PooledPresentationGeometry2>();

            if (component == null)
                component = gameObject.AddComponent<PooledPresentationGeometry2>();

            component.Configure(pool, priority);

            if (!Active.Contains(component))
                Active.Add(component);

            return component;
        }

        private static Quaternion RandomRotation(string name, Vector3 position)
        {
            int seed = name.GetHashCode() ^ position.GetHashCode();
            float x = Mathf.Abs(seed % 360);
            float y = Mathf.Abs((seed / 7) % 360);
            float z = Mathf.Abs((seed / 17) % 360);
            return Quaternion.Euler(x, y, z);
        }

        private static Transform Root()
        {
            GameObject root = GameObject.Find("Arcane Engine 2.0 Geometry Pool");

            if (root == null)
            {
                root = new GameObject("Arcane Engine 2.0 Geometry Pool");
                UnityEngine.Object.DontDestroyOnLoad(root);
            }

            return root.transform;
        }

        private static void Cleanup()
        {
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                if (Active[i] == null)
                    Active.RemoveAt(i);
            }
        }
    }

    public sealed class PresentationPrimitiveKind2 : MonoBehaviour
    {
        public PrimitiveType type;
    }

    public sealed class PooledPresentationGeometry2 : MonoBehaviour
    {
        private Stack<GameObject> _pool;
        private float _remaining;
        private float _initialLifetime;
        private bool _moving;
        private bool _expand;
        private Vector3 _velocity;
        private Vector3 _baseScale;
        private bool _ring;
        private Vector3 _ringCenter;
        private float _ringStart;
        private float _ringEnd;
        private float _ringWidth;
        private Color _ringColor;
        private LineRenderer _line;

        public PresentationPriority priority;

        public void Configure(
            Stack<GameObject> pool,
            PresentationPriority presentationPriority)
        {
            _pool = pool;
            priority = presentationPriority;
            _line = GetComponent<LineRenderer>();
        }

        public void BeginStatic(float lifetime)
        {
            _remaining = _initialLifetime = Mathf.Max(0.03f, lifetime);
            _moving = false;
            _expand = false;
            _ring = false;
            enabled = true;
        }

        public void BeginMoving(
            float lifetime,
            bool expand,
            Vector3 velocity,
            Vector3 baseScale)
        {
            _remaining = _initialLifetime = Mathf.Max(0.03f, lifetime);
            _moving = velocity.sqrMagnitude > 0.0001f;
            _expand = expand;
            _velocity = velocity;
            _baseScale = baseScale;
            _ring = false;
            enabled = true;
        }

        public void BeginRing(
            Vector3 center,
            float startRadius,
            float endRadius,
            float width,
            Color color,
            float lifetime)
        {
            _remaining = _initialLifetime = Mathf.Max(0.03f, lifetime);
            _moving = false;
            _expand = false;
            _ring = true;
            _ringCenter = center;
            _ringStart = startRadius;
            _ringEnd = endRadius;
            _ringWidth = width;
            _ringColor = color;
            _line = GetComponent<LineRenderer>();
            enabled = true;
        }

        private void Update()
        {
            _remaining -= Time.deltaTime;
            float normalized = Mathf.Clamp01(1f - _remaining / _initialLifetime);

            if (_moving)
                transform.position += _velocity * Time.deltaTime;

            if (_expand)
                transform.localScale = _baseScale * Mathf.Lerp(1f, 2.2f, normalized);

            if (_ring && _line != null)
            {
                float radius = Mathf.Lerp(_ringStart, _ringEnd, normalized);
                int count = _line.positionCount;

                for (int i = 0; i < count; i++)
                {
                    float angle = i / (float)count * Mathf.PI * 2f;
                    _line.SetPosition(
                        i,
                        _ringCenter + new Vector3(
                            Mathf.Cos(angle) * radius,
                            0.035f,
                            Mathf.Sin(angle) * radius));
                }

                Color faded = _ringColor;
                faded.a *= 1f - normalized;
                _line.startColor = faded;
                _line.endColor = faded;
                _line.startWidth = _ringWidth * Mathf.Lerp(1f, 0.35f, normalized);
                _line.endWidth = _line.startWidth;
            }

            if (_remaining <= 0f)
            {
                enabled = false;
                PresentationGeometry2.Release(this, _pool);
            }
        }
    }
}
