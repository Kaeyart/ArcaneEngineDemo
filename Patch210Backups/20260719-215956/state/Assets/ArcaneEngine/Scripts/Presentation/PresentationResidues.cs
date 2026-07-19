using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class PresentationResidue2 : MonoBehaviour
    {
        private static readonly List<PresentationResidue2> Active =
            new List<PresentationResidue2>();

        private ReactionElement _signature;
        private float _radius;
        private float _expiresAt;
        private int _seed;
        private PresentationPriority _priority;
        private PooledPresentationParticle2 _particles;
        private LineRenderer _boundary;
        private float _nextPulse;
        private float _nextMotif;
        private bool _ownsLifetime;

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
                host,
                signature,
                radius,
                duration,
                seed,
                priority,
                false);
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
                host,
                signature,
                radius,
                duration,
                seed,
                priority,
                false);
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
            bool found = false;

            for (int i = 0; i < Active.Count; i++)
            {
                PresentationResidue2 residue = Active[i];
                if (residue == null)
                    continue;

                Vector3 delta = residue.transform.position - position;
                delta.y = 0f;

                if (delta.sqrMagnitude > radiusSquared)
                    continue;

                residue.Merge(signature, residue._radius, duration, intensity);
                residue.Pulse(intensity);
                found = true;
            }

            if (!found)
            {
                SpawnStandalone(
                    position,
                    signature,
                    Mathf.Max(0.8f, radius * 0.55f),
                    duration,
                    position.GetHashCode() ^ (int)signature,
                    PresentationPriority.Critical);
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
                "AE2 Residue " + ElementalReactionCodex.SignatureText(signature));
            host.transform.position = position;
            return Attach(
                host,
                signature,
                radius,
                duration,
                seed,
                priority,
                true);
        }

        private static PresentationResidue2 Attach(
            GameObject host,
            ReactionElement signature,
            float radius,
            float duration,
            int seed,
            PresentationPriority priority,
            bool ownsLifetime)
        {
            if (host == null)
                return null;

            Cleanup();

            if (Active.Count >= Patch200PresentationSettings.MaxResidues &&
                priority < PresentationPriority.Critical)
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
                ownsLifetime);

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
            _signature = signature;
            _radius = Mathf.Max(0.35f, radius);
            _expiresAt = Mathf.Max(_expiresAt, Time.time + Mathf.Max(0.35f, duration));
            _seed = seed;
            _priority = priority;
            _ownsLifetime = ownsLifetime;
            _nextPulse = Time.time + 0.12f;
            _nextMotif = Time.time + 0.28f;
            Rebuild();
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
            _expiresAt = Mathf.Max(_expiresAt, Time.time + duration);

            if (previous != _signature)
            {
                Rebuild();
                PresentationGeometry2.Burst(
                    transform.position + Vector3.up * 0.08f,
                    _signature,
                    Mathf.Max(0.6f, _radius * 0.65f),
                    0.5f,
                    intensity,
                    _priority,
                    _seed ^ (int)_signature);
            }
        }

        public void Pulse(float intensity)
        {
            Color color = ElementalReactionCodex.BlendColor(_signature);

            PresentationGeometry2.Ring(
                transform.position,
                color,
                Mathf.Max(0.4f, _radius * 0.22f),
                0.045f,
                0.42f,
                _priority,
                true);

            EmitElementMotifs(intensity);
        }

        private void Rebuild()
        {
            if (_particles != null)
                _particles.StopAndRelease();

            ReactionElement primary = ElementalReactionCodex.PrimaryElement(_signature);
            ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(primary);

            _particles = PresentationParticlePool2.Spawn(
                new PresentationParticleRequest
                {
                    purpose = PresentationParticlePurpose.Field,
                    position = transform.position + Vector3.up * 0.06f,
                    follow = transform,
                    direction = Vector3.up,
                    signature = _signature,
                    primary = primary,
                    primaryColor = ElementalReactionCodex.BlendColor(_signature),
                    secondaryColor = profile == null ? Color.white : profile.secondary,
                    radius = _radius,
                    duration = Mathf.Max(0.5f, _expiresAt - Time.time),
                    intensity = 0.75f + ElementalReactionCodex.CountBits(_signature) * 0.08f,
                    speed = profile == null ? 0.7f : profile.particleSpeed,
                    seed = _seed,
                    count = Mathf.RoundToInt(12f + _radius * 7f),
                    looping = true,
                    worldSpace = true,
                    priority = _priority
                });

            if (_boundary != null)
                Destroy(_boundary.gameObject);

            _boundary = CreatePersistentBoundary();
        }

        private LineRenderer CreatePersistentBoundary()
        {
            GameObject gameObject = new GameObject("AE2 Residue Boundary");
            gameObject.transform.SetParent(transform, false);
            LineRenderer line = gameObject.AddComponent<LineRenderer>();
            int segments = Patch200PresentationSettings.Quality == PresentationQuality.Low
                ? 28
                : 48;
            line.loop = true;
            line.positionCount = segments;
            line.useWorldSpace = false;
            line.startWidth = 0.045f;
            line.endWidth = 0.045f;
            Color color = ElementalReactionCodex.BlendColor(_signature);
            color.a = 0.8f;
            line.startColor = color;
            line.endColor = color;
            line.material = PresentationMaterialLibrary2.Mesh(color, 0.72f);

            for (int i = 0; i < segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                line.SetPosition(
                    i,
                    new Vector3(
                        Mathf.Cos(angle) * _radius,
                        0.045f,
                        Mathf.Sin(angle) * _radius));
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

            if (_boundary != null)
            {
                float pulse = 1f +
                    Mathf.Sin(Time.time * 3.2f + (_seed & 31)) *
                    (Patch200PresentationSettings.ReducedMotion ? 0.01f : 0.035f);
                _boundary.transform.localScale = new Vector3(pulse, 1f, pulse);
            }

            if (Time.time >= _nextPulse)
            {
                _nextPulse = Time.time + 0.75f;
                Pulse(0.65f);
            }

            if (Time.time >= _nextMotif)
            {
                _nextMotif = Time.time + 0.42f;
                EmitElementMotifs(0.4f);
            }
        }

        private void EmitElementMotifs(float intensity)
        {
            int index = 0;

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(_signature))
            {
                float angle = ((_seed & 255) * 0.017f) +
                              index * Mathf.PI * 2f /
                              Mathf.Max(1, ElementalReactionCodex.CountBits(_signature));

                Vector3 point = transform.position + new Vector3(
                    Mathf.Cos(angle) * _radius * 0.55f,
                    0.1f,
                    Mathf.Sin(angle) * _radius * 0.55f);

                if (element == ReactionElement.Lightning)
                {
                    Vector3 opposite = transform.position +
                        new Vector3(-Mathf.Cos(angle), 0.55f, -Mathf.Sin(angle)) * _radius * 0.55f;
                    PresentationGeometry2.Beam(
                        point + Vector3.up * 0.2f,
                        opposite,
                        ElementalReactionCodex.ColorFor(element),
                        0.04f,
                        0.16f,
                        PresentationPriority.Normal,
                        true,
                        _seed + index);
                }
                else if (element == ReactionElement.Fire ||
                         element == ReactionElement.Toxic ||
                         element == ReactionElement.Blood)
                {
                    PresentationParticlePool2.Spawn(new PresentationParticleRequest
                    {
                        purpose = element == ReactionElement.Fire
                            ? PresentationParticlePurpose.Smoke
                            : PresentationParticlePurpose.Residue,
                        position = point,
                        direction = Vector3.up,
                        signature = element,
                        primary = element,
                        primaryColor = ElementalReactionCodex.ColorFor(element),
                        secondaryColor = Color.white,
                        radius = Mathf.Max(0.15f, _radius * 0.12f),
                        duration = 0.5f,
                        intensity = intensity,
                        seed = _seed + index * 37,
                        count = 4,
                        priority = PresentationPriority.Decorative,
                        worldSpace = true
                    });
                }
                else if (element == ReactionElement.Cold ||
                         element == ReactionElement.Physical)
                {
                    ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(element);
                    PresentationGeometry2.Primitive(
                        "AE2 Residue Accent " + element,
                        profile == null ? PrimitiveType.Cube : profile.accentShape,
                        point,
                        Vector3.one * Mathf.Clamp(_radius * 0.045f, 0.05f, 0.22f),
                        profile == null ? Color.white : profile.secondary,
                        0.4f,
                        PresentationPriority.Decorative,
                        false,
                        Vector3.up * 0.35f);
                }
                else if (element == ReactionElement.Void)
                {
                    PresentationGeometry2.Ring(
                        point,
                        ElementalReactionCodex.ColorFor(element),
                        Mathf.Max(0.12f, _radius * 0.08f),
                        0.025f,
                        0.28f,
                        PresentationPriority.Normal,
                        false);
                }

                index++;
            }
        }

        private void OnDestroy()
        {
            Active.Remove(this);

            if (_particles != null)
                _particles.StopAndRelease();

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
