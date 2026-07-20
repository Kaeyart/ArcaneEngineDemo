using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class PresentationResidue2 : MonoBehaviour
    {
        private const int LocalResidueCap22 = 10;
        private static readonly List<PresentationResidue2> Active =
            new List<PresentationResidue2>();

        private ReactionElement _signature;
        private float _radius;
        private float _expiresAt;
        private int _seed;
        private PresentationPriority _priority;
        private LineRenderer _boundary;
        private bool _ownsLifetime;
        private bool _gameplayField;

        public ReactionElement Signature
        {
            get { return _signature; }
        }

        public static int ActiveCount
        {
            get
            {
                Cleanup();
                return Active.Count;
            }
        }

        public static PresentationResidue2 AttachSpellZone(
            GameObject host,
            ReactionElement signature,
            float radius,
            float duration,
            int seed,
            PresentationPriority priority)
        {
            return Attach(
                host, signature, radius, duration, seed, priority, false, true);
        }

        public static PresentationResidue2 AttachReactionField(
            GameObject host,
            ReactionElement signature,
            float radius,
            float duration,
            int seed,
            PresentationPriority priority)
        {
            return Attach(
                host, signature, radius, duration, seed, priority, false, true);
        }

        public static void SurgeNear(
            Vector3 position,
            float radius,
            ReactionElement signature,
            float duration,
            float intensity)
        {
            Cleanup();
            float radiusSquared = radius * radius;
            PresentationResidue2 best = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < Active.Count; i++)
            {
                PresentationResidue2 residue = Active[i];
                if (residue == null || !residue._gameplayField)
                    continue;

                Vector3 delta = residue.transform.position - position;
                delta.y = 0f;
                if (delta.sqrMagnitude > radiusSquared ||
                    delta.sqrMagnitude >= bestDistance)
                    continue;

                best = residue;
                bestDistance = delta.sqrMagnitude;
            }

            if (best != null)
            {
                best.Merge(signature, best._radius, duration, intensity);
                best.Pulse(Mathf.Min(0.65f, intensity));
            }
        }

        public static PresentationResidue2 SpawnStandalone(
            Vector3 position,
            ReactionElement signature,
            float radius,
            float duration,
            int seed,
            PresentationPriority priority)
        {
            GameObject host = new GameObject(
                "AE2 Cosmetic Residue " +
                ElementalReactionCodex.SignatureText(signature));
            host.transform.position = position;
            return Attach(
                host,
                signature,
                radius,
                Mathf.Min(1.5f, duration),
                seed,
                priority,
                true,
                false);
        }

        private static PresentationResidue2 Attach(
            GameObject host,
            ReactionElement signature,
            float radius,
            float duration,
            int seed,
            PresentationPriority priority,
            bool ownsLifetime,
            bool gameplayField)
        {
            if (host == null)
                return null;

            Cleanup();

            int cap = Mathf.Min(Patch200PresentationSettings.MaxResidues, LocalResidueCap22);
            if (Active.Count >= cap &&
                !gameplayField &&
                (int)priority < (int)PresentationPriority.Critical)
            {
                return null;
            }

            PresentationResidue2 residue = host.GetComponent<PresentationResidue2>();
            if (residue == null)
                residue = host.AddComponent<PresentationResidue2>();

            residue.Initialize(
                signature,
                radius,
                duration,
                seed,
                priority,
                ownsLifetime,
                gameplayField);

            if (!Active.Contains(residue))
                Active.Add(residue);

            return residue;
        }

        public void Initialize(
            ReactionElement signature,
            float radius,
            float duration,
            int seed,
            PresentationPriority priority,
            bool ownsLifetime)
        {
            Initialize(
                signature, radius, duration, seed, priority, ownsLifetime,
                !ownsLifetime);
        }

        private void Initialize(
            ReactionElement signature,
            float radius,
            float duration,
            int seed,
            PresentationPriority priority,
            bool ownsLifetime,
            bool gameplayField)
        {
            _signature = signature;
            _radius = Mathf.Max(0.35f, radius);
            float lifetime = gameplayField
                ? Mathf.Max(0.35f, duration)
                : Mathf.Clamp(duration, 0.35f, 1.5f);
            _expiresAt = Mathf.Max(_expiresAt, Time.time + lifetime);
            _seed = seed;
            _priority = priority;
            _ownsLifetime = ownsLifetime;
            _gameplayField = gameplayField;
            RebuildBoundary();
        }

        public void Merge(
            ReactionElement signature,
            float radius,
            float duration,
            float intensity)
        {
            ReactionElement previous = _signature;
            _signature |= signature;
            _radius = Mathf.Max(_radius, radius);
            _expiresAt = Mathf.Max(
                _expiresAt,
                Time.time + (_gameplayField
                    ? Mathf.Max(0.35f, duration)
                    : Mathf.Min(1.5f, duration)));

            if (previous != _signature || _boundary == null)
                RebuildBoundary();
        }

        public void Pulse(float intensity)
        {
            if (!_gameplayField || intensity <= 0.01f)
                return;

            Color color = ElementalReactionCodex.BlendColor(_signature);
            color.a = 0.56f;

            PresentationGeometry2.Ring(
                transform.position,
                color,
                Mathf.Max(0.4f, _radius * 0.20f),
                0.035f,
                0.28f,
                _priority,
                true);

            EmitSingleMotif(Mathf.Clamp01(intensity));
        }

        private void RebuildBoundary()
        {
            if (_boundary != null)
                Destroy(_boundary.gameObject);

            _boundary = CreatePersistentBoundary();
        }

        private LineRenderer CreatePersistentBoundary()
        {
            GameObject gameObject = new GameObject(
                _gameplayField
                    ? "AE2 Gameplay Field Boundary"
                    : "AE2 Cosmetic Residue Boundary");
            gameObject.transform.SetParent(transform, false);
            LineRenderer line = gameObject.AddComponent<LineRenderer>();
            int segments = Patch200PresentationSettings.Quality == PresentationQuality.Low
                ? 20
                : 32;
            line.loop = true;
            line.positionCount = segments;
            line.useWorldSpace = false;
            line.startWidth = _gameplayField ? 0.045f : 0.022f;
            line.endWidth = line.startWidth;
            Color color = ElementalReactionCodex.BlendColor(_signature);
            color.a = _gameplayField ? 0.72f : 0.30f;
            line.startColor = color;
            line.endColor = color;
            line.material = PresentationMaterialLibrary2.Mesh(
                color,
                _gameplayField ? 0.64f : 0.25f);

            for (int i = 0; i < segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                float irregular = _gameplayField
                    ? 1f
                    : 0.92f + 0.08f * Mathf.Sin(angle * 3f + (_seed & 31));
                line.SetPosition(
                    i,
                    new Vector3(
                        Mathf.Cos(angle) * _radius * irregular,
                        0.045f,
                        Mathf.Sin(angle) * _radius * irregular));
            }

            return line;
        }

        private void Update()
        {
            if (_ownsLifetime && Time.time >= _expiresAt)
            {
                Destroy(gameObject);
                return;
            }

            if (!_ownsLifetime && Time.time >= _expiresAt)
            {
                Destroy(this);
                return;
            }

            if (_boundary != null && _gameplayField)
            {
                float amplitude = Patch200PresentationSettings.ReducedMotion
                    ? 0.005f
                    : 0.015f;
                float pulse = 1f +
                    Mathf.Sin(Time.time * 1.6f + (_seed & 31)) * amplitude;
                _boundary.transform.localScale = new Vector3(pulse, 1f, pulse);
            }
        }

        private void EmitSingleMotif(float intensity)
        {
            ReactionElement primary =
                ElementalReactionCodex.PrimaryElement(_signature);
            float angle = ((_seed & 255) * 0.017f) + Time.time * 0.25f;
            Vector3 point = transform.position + new Vector3(
                Mathf.Cos(angle) * _radius * 0.50f,
                0.1f,
                Mathf.Sin(angle) * _radius * 0.50f);

            if (primary == ReactionElement.Lightning)
            {
                Vector3 opposite = transform.position + new Vector3(
                    -Mathf.Cos(angle) * _radius * 0.45f,
                    0.35f,
                    -Mathf.Sin(angle) * _radius * 0.45f);
                PresentationGeometry2.Beam(
                    point + Vector3.up * 0.15f,
                    opposite,
                    ElementalReactionCodex.ColorFor(primary),
                    0.028f,
                    0.10f,
                    PresentationPriority.Normal,
                    true,
                    _seed);
            }
            else
            {
                PresentationParticlePool2.Spawn(new PresentationParticleRequest
                {
                    purpose = PresentationParticlePurpose.Residue,
                    position = point,
                    direction = Vector3.up,
                    signature = primary,
                    primary = primary,
                    primaryColor = ElementalReactionCodex.ColorFor(primary),
                    secondaryColor = Color.white,
                    radius = Mathf.Max(0.12f, _radius * 0.08f),
                    duration = 0.32f,
                    intensity = intensity * 0.45f,
                    seed = _seed,
                    count = 2,
                    priority = PresentationPriority.Decorative,
                    worldSpace = true
                });
            }
        }

        private void OnDestroy()
        {
            Active.Remove(this);
            if (_boundary != null)
                Destroy(_boundary.gameObject);
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
}
