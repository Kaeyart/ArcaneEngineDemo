using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum GeneratedSpellHostKind
    {
        Projectile,
        Zone,
        Familiar
    }

    public sealed class GeneratedSpellHostVisual2 : MonoBehaviour
    {
        private GeneratedVisualRecipe _recipe;
        private CompiledSpell _spell;
        private CastRequest _request;
        private GeneratedSpellHostKind _kind;
        private readonly List<PooledPresentationParticle2> _particles =
            new List<PooledPresentationParticle2>();
        private readonly List<GameObject> _ornaments = new List<GameObject>();
        private TrailRenderer _trail;
        private float _phase;
        private float _nextPulse;
        private float _nextOperatorCue;
        private bool _initialized;

        public GeneratedVisualRecipe Recipe
        {
            get { return _recipe; }
        }

        public void Initialize(
            GeneratedVisualRecipe recipe,
            CompiledSpell spell,
            CastRequest request,
            GeneratedSpellHostKind kind)
        {
            _recipe = recipe;
            _spell = spell;
            _request = request;
            _kind = kind;

            if (_recipe == null || _initialized)
                return;

            _initialized = true;
            _phase = (_recipe.stableSeed & 1023) / 1023f * Mathf.PI * 2f;
            BuildRequiredSilhouette();
            BuildElementLayers();
            BuildRuneOperators();
            BuildTrail();
            _nextPulse = Time.time + 0.2f;
            _nextOperatorCue = Time.time + 0.4f;
        }

        private void BuildRequiredSilhouette()
        {
            if (_kind == GeneratedSpellHostKind.Zone)
            {
                PresentationResidue2.AttachSpellZone(
                    gameObject,
                    _recipe.signature,
                    Mathf.Max(0.7f, _recipe.radius),
                    Mathf.Max(0.5f, _recipe.duration),
                    _recipe.stableSeed,
                    PresentationPriority.Important);
                return;
            }

            ElementVisualProfile2 profile =
                ElementVisualProfileRegistry2.Get(_recipe.primaryElement);

            PrimitiveType type = profile == null
                ? PrimitiveType.Sphere
                : profile.authorityShape;

            float scale = Mathf.Clamp(_recipe.size * 0.24f, 0.12f, 0.8f);

            GameObject ornament = PresentationGeometry2.Primitive(
                "AE2 " + _recipe.displayName + " Chassis",
                type,
                transform.position,
                Vector3.one * scale,
                _recipe.primaryColor,
                Mathf.Max(0.4f, _recipe.duration + 1f),
                PresentationPriority.Important,
                false,
                Vector3.zero);

            ornament.transform.SetParent(transform, true);
            ornament.transform.localPosition = Vector3.zero;
            _ornaments.Add(ornament);

            if (_kind == GeneratedSpellHostKind.Familiar)
            {
                GameObject ring = PresentationGeometry2.Primitive(
                    "AE2 Familiar Core",
                    PrimitiveType.Cylinder,
                    transform.position,
                    Vector3.one * Mathf.Max(0.2f, scale * 1.5f),
                    _recipe.secondaryColor,
                    Mathf.Max(1f, _recipe.duration + 1f),
                    PresentationPriority.Important,
                    false,
                    Vector3.zero);

                ring.transform.SetParent(transform, true);
                ring.transform.localPosition = Vector3.zero;
                _ornaments.Add(ring);
            }
        }

        private void BuildElementLayers()
        {
            for (int i = 0; i < _recipe.elementLayers.Count; i++)
            {
                ElementVisualLayer layer = _recipe.elementLayers[i];

                if (layer.element == ReactionElement.Blood &&
                    Patch200PresentationSettings.BloodMode == BloodPresentationMode.Hidden)
                {
                    continue;
                }

                float intensity = layer.intensity;

                if (layer.element == ReactionElement.Blood &&
                    Patch200PresentationSettings.BloodMode == BloodPresentationMode.Reduced)
                {
                    intensity *= 0.35f;
                }

                PresentationParticleRequest request =
                    new PresentationParticleRequest
                    {
                        purpose = _kind == GeneratedSpellHostKind.Zone
                            ? PresentationParticlePurpose.Field
                            : _kind == GeneratedSpellHostKind.Familiar
                                ? PresentationParticlePurpose.Status
                                : PresentationParticlePurpose.Trail,
                        position = transform.position,
                        direction = transform.forward,
                        follow = transform,
                        signature = _recipe.signature,
                        primary = layer.element,
                        primaryColor = layer.primary,
                        secondaryColor = layer.secondary,
                        radius = _kind == GeneratedSpellHostKind.Zone
                            ? _recipe.radius
                            : Mathf.Max(0.18f, _recipe.size * 0.2f),
                        duration = Mathf.Max(0.8f, _recipe.duration + 1f),
                        intensity = intensity,
                        speed = Mathf.Max(0.5f, _recipe.speed * 0.08f),
                        seed = _recipe.stableSeed + i * 97,
                        count = Mathf.RoundToInt(10f + intensity * 14f),
                        looping = true,
                        worldSpace = true,
                        priority = layer.role == ElementVisualRole.PrimarySilhouette ||
                                   layer.role == ElementVisualRole.Catalyst
                            ? PresentationPriority.Important
                            : PresentationPriority.Decorative
                    };

                PooledPresentationParticle2 particle =
                    PresentationParticlePool2.Spawn(request);

                if (particle != null)
                    _particles.Add(particle);
            }
        }

        private void BuildTrail()
        {
            if (_kind != GeneratedSpellHostKind.Projectile)
                return;

            _trail = gameObject.GetComponent<TrailRenderer>();

            if (_trail == null)
                _trail = gameObject.AddComponent<TrailRenderer>();

            _trail.time = Mathf.Clamp(
                0.16f + _recipe.size * 0.08f,
                0.15f,
                0.85f);
            _trail.minVertexDistance = 0.04f;
            _trail.startWidth = Mathf.Clamp(_recipe.size * 0.14f, 0.05f, 0.38f);
            _trail.endWidth = 0f;
            _trail.startColor = _recipe.primaryColor;
            _trail.endColor = new Color(
                _recipe.secondaryColor.r,
                _recipe.secondaryColor.g,
                _recipe.secondaryColor.b,
                0f);
            _trail.material = PresentationMaterialLibrary2.Mesh(
                _recipe.primaryColor,
                0.85f);
            _trail.emitting = true;
        }

        private void BuildRuneOperators()
        {
            for (int i = 0; i < _recipe.operators.Count; i++)
            {
                RuneVisualOperatorSpec operation = _recipe.operators[i];

                switch (operation.kind)
                {
                    case RuneVisualOperatorKind.Orbit:
                        AddOrbitals(operation, i);
                        break;

                    case RuneVisualOperatorKind.Barrier:
                        AddBarrier(operation);
                        break;

                    case RuneVisualOperatorKind.Split:
                        AddSplitCrown(operation, i);
                        break;

                    case RuneVisualOperatorKind.Chain:
                        AddConductorNodes(operation, i);
                        break;

                    case RuneVisualOperatorKind.Return:
                        AddReturnMarker(operation);
                        break;

                    case RuneVisualOperatorKind.Delay:
                        AddDelayMarker(operation);
                        break;

                    case RuneVisualOperatorKind.Pull:
                        AddCompressionShell(operation);
                        break;
                }
            }
        }

        private void AddOrbitals(RuneVisualOperatorSpec operation, int index)
        {
            int count = Mathf.Clamp(operation.count <= 0 ? 3 : operation.count, 2, 8);
            float radius = Mathf.Clamp(0.25f + _recipe.size * 0.18f, 0.25f, 1.4f);

            for (int i = 0; i < count; i++)
            {
                ReactionElement element = ElementAt(i);
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(element);
                GameObject node = PresentationGeometry2.Primitive(
                    "AE2 Orbit Node",
                    profile == null ? PrimitiveType.Sphere : profile.accentShape,
                    transform.position,
                    Vector3.one * Mathf.Clamp(_recipe.size * 0.08f, 0.06f, 0.22f),
                    profile == null ? _recipe.secondaryColor : profile.primary,
                    Mathf.Max(0.7f, _recipe.duration + 1f),
                    PresentationPriority.Normal,
                    false,
                    Vector3.zero);

                node.transform.SetParent(transform, true);
                PresentationHostOrbiter2 orbiter = node.AddComponent<PresentationHostOrbiter2>();
                orbiter.Initialize(
                    transform,
                    radius,
                    i / (float)count * Mathf.PI * 2f,
                    _recipe.topology == null || _recipe.topology.clockwiseBias >= 0 ? 1f : -1f,
                    1.5f + index * 0.18f);
                _ornaments.Add(node);
            }
        }

        private void AddBarrier(RuneVisualOperatorSpec operation)
        {
            GameObject shell = PresentationGeometry2.Primitive(
                "AE2 Barrier Shell",
                PrimitiveType.Sphere,
                transform.position,
                Vector3.one * Mathf.Clamp(0.45f + operation.magnitude, 0.55f, 2.2f),
                Color.Lerp(_recipe.primaryColor, Color.white, 0.45f),
                Mathf.Max(0.7f, _recipe.duration + 1f),
                PresentationPriority.Important,
                false,
                Vector3.zero);

            shell.transform.SetParent(transform, true);
            shell.transform.localPosition = Vector3.zero;
            _ornaments.Add(shell);
        }

        private void AddSplitCrown(RuneVisualOperatorSpec operation, int index)
        {
            int count = Mathf.Clamp(operation.count <= 0 ? 2 : operation.count, 2, 10);

            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f;
                Vector3 local = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) *
                                Mathf.Clamp(0.18f + _recipe.size * 0.09f, 0.18f, 0.75f);

                GameObject shard = PresentationGeometry2.Primitive(
                    "AE2 Split Marker",
                    PrimitiveType.Cube,
                    transform.position,
                    Vector3.one * Mathf.Clamp(_recipe.size * 0.055f, 0.04f, 0.16f),
                    Color.Lerp(_recipe.primaryColor, _recipe.secondaryColor, i / (float)count),
                    Mathf.Max(0.7f, _recipe.duration + 1f),
                    PresentationPriority.Normal,
                    false,
                    Vector3.zero);

                shard.transform.SetParent(transform, true);
                shard.transform.localPosition = local;
                _ornaments.Add(shard);
            }
        }

        private void AddConductorNodes(RuneVisualOperatorSpec operation, int index)
        {
            int count = Mathf.Clamp(operation.count <= 0 ? 2 : operation.count, 2, 6);

            for (int i = 0; i < count; i++)
            {
                GameObject node = PresentationGeometry2.Primitive(
                    "AE2 Chain Conductor",
                    PrimitiveType.Sphere,
                    transform.position,
                    Vector3.one * Mathf.Clamp(_recipe.size * 0.065f, 0.045f, 0.15f),
                    _recipe.catalyst == ReactionElement.Lightning
                        ? ElementalReactionCodex.ColorFor(ReactionElement.Lightning)
                        : _recipe.secondaryColor,
                    Mathf.Max(0.7f, _recipe.duration + 1f),
                    PresentationPriority.Normal,
                    false,
                    Vector3.zero);

                node.transform.SetParent(transform, true);
                node.transform.localPosition = new Vector3(
                    0f,
                    (i - (count - 1) * 0.5f) * 0.12f,
                    0f);
                _ornaments.Add(node);
            }
        }

        private void AddReturnMarker(RuneVisualOperatorSpec operation)
        {
            PresentationParticleRequest request = new PresentationParticleRequest
            {
                purpose = PresentationParticlePurpose.Trail,
                position = transform.position,
                direction = -transform.forward,
                follow = transform,
                signature = _recipe.signature,
                primary = _recipe.catalyst,
                primaryColor = _recipe.secondaryColor,
                secondaryColor = _recipe.primaryColor,
                radius = Mathf.Max(0.12f, _recipe.size * 0.12f),
                duration = Mathf.Max(0.8f, _recipe.duration + 1f),
                intensity = 0.75f,
                seed = _recipe.stableSeed ^ 0x5A512,
                count = 8,
                looping = true,
                worldSpace = true,
                priority = PresentationPriority.Normal
            };

            PooledPresentationParticle2 particle =
                PresentationParticlePool2.Spawn(request);
            if (particle != null)
                _particles.Add(particle);
        }

        private void AddDelayMarker(RuneVisualOperatorSpec operation)
        {
            GameObject marker = PresentationGeometry2.Primitive(
                "AE2 Delay Marker",
                PrimitiveType.Cylinder,
                transform.position,
                new Vector3(0.28f, 0.035f, 0.28f),
                _recipe.secondaryColor,
                Mathf.Max(0.7f, _recipe.duration + 1f),
                PresentationPriority.Important,
                false,
                Vector3.zero);

            marker.transform.SetParent(transform, true);
            marker.transform.localPosition = Vector3.down * 0.18f;
            marker.AddComponent<PresentationPulseScale2>()
                .Initialize(2.8f, 0.75f, 1.25f);
            _ornaments.Add(marker);
        }

        private void AddCompressionShell(RuneVisualOperatorSpec operation)
        {
            GameObject shell = PresentationGeometry2.Primitive(
                "AE2 Pull Shell",
                PrimitiveType.Sphere,
                transform.position,
                Vector3.one * Mathf.Clamp(0.35f + operation.magnitude * 0.18f, 0.35f, 1.5f),
                Color.Lerp(_recipe.primaryColor, Color.black, 0.35f),
                Mathf.Max(0.7f, _recipe.duration + 1f),
                PresentationPriority.Normal,
                false,
                Vector3.zero);

            shell.transform.SetParent(transform, true);
            shell.transform.localPosition = Vector3.zero;
            shell.AddComponent<PresentationPulseScale2>()
                .Initialize(4.5f, 1.15f, 0.75f);
            _ornaments.Add(shell);
        }

        private ReactionElement ElementAt(int index)
        {
            int current = 0;

            foreach (ReactionElement element in ElementalReactionCodex.Enumerate(_recipe.signature))
            {
                if (current == index % Mathf.Max(1, ElementalReactionCodex.CountBits(_recipe.signature)))
                    return element;
                current++;
            }

            return _recipe.primaryElement;
        }

        private void Update()
        {
            if (!_initialized || _recipe == null)
                return;

            _phase += Time.deltaTime *
                      (Patch200PresentationSettings.ReducedMotion ? 0.45f : 1f);

            for (int i = 0; i < _ornaments.Count; i++)
            {
                GameObject ornament = _ornaments[i];
                if (ornament == null || ornament.transform.parent != transform)
                    continue;

                ornament.transform.Rotate(
                    Vector3.up,
                    Time.deltaTime * (22f + i * 4f),
                    Space.Self);
            }

            if (Time.time >= _nextPulse)
            {
                _nextPulse = Time.time +
                    Mathf.Clamp(0.65f - _recipe.visualPriority * 0.05f, 0.28f, 0.85f);

                if (_kind == GeneratedSpellHostKind.Zone ||
                    _kind == GeneratedSpellHostKind.Familiar)
                {
                    PresentationGeometry2.Ring(
                        transform.position,
                        _recipe.secondaryColor,
                        Mathf.Max(0.3f, _kind == GeneratedSpellHostKind.Zone ? _recipe.radius * 0.35f : _recipe.size * 0.35f),
                        0.035f,
                        0.28f,
                        PresentationPriority.Normal,
                        true);
                }
            }

            if (Time.time >= _nextOperatorCue)
            {
                _nextOperatorCue = Time.time + 0.8f;
                EmitOperatorCue();
            }
        }

        private void EmitOperatorCue()
        {
            for (int i = 0; i < _recipe.operators.Count; i++)
            {
                RuneVisualOperatorSpec operation = _recipe.operators[i];

                if (operation.kind == RuneVisualOperatorKind.Accelerate)
                {
                    PresentationParticlePool2.Spawn(new PresentationParticleRequest
                    {
                        purpose = PresentationParticlePurpose.Trail,
                        position = transform.position,
                        direction = -transform.forward,
                        signature = _recipe.signature,
                        primary = _recipe.motionElement,
                        primaryColor = _recipe.primaryColor,
                        secondaryColor = _recipe.secondaryColor,
                        radius = 0.12f,
                        duration = 0.2f,
                        intensity = 0.7f,
                        speed = 2.5f,
                        seed = _recipe.stableSeed + i,
                        count = 5,
                        priority = PresentationPriority.Decorative,
                        worldSpace = true
                    });
                }
                else if (operation.kind == RuneVisualOperatorKind.Persistent &&
                         _kind == GeneratedSpellHostKind.Zone)
                {
                    PresentationGeometry2.Ring(
                        transform.position,
                        _recipe.primaryColor,
                        Mathf.Max(0.5f, _recipe.radius * 0.75f),
                        0.03f,
                        0.45f,
                        PresentationPriority.Important,
                        false);
                }
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                if (_particles[i] != null)
                    _particles[i].StopAndRelease();
            }

            _particles.Clear();

            if (_trail != null)
                _trail.emitting = false;
        }
    }

    public sealed class PresentationHostOrbiter2 : MonoBehaviour
    {
        private Transform _center;
        private float _radius;
        private float _angle;
        private float _direction;
        private float _speed;

        public void Initialize(
            Transform center,
            float radius,
            float angle,
            float direction,
            float speed)
        {
            _center = center;
            _radius = radius;
            _angle = angle;
            _direction = direction;
            _speed = speed;
        }

        private void LateUpdate()
        {
            if (_center == null)
            {
                Destroy(gameObject);
                return;
            }

            _angle += Time.deltaTime * _speed * _direction *
                      (Patch200PresentationSettings.ReducedMotion ? 0.45f : 1f);

            transform.position = _center.position + new Vector3(
                Mathf.Cos(_angle) * _radius,
                Mathf.Sin(_angle * 2f) * _radius * 0.18f,
                Mathf.Sin(_angle) * _radius);
        }
    }

    public sealed class PresentationPulseScale2 : MonoBehaviour
    {
        private float _frequency;
        private float _minimum;
        private float _maximum;
        private Vector3 _baseScale;

        public void Initialize(float frequency, float minimum, float maximum)
        {
            _frequency = frequency;
            _minimum = minimum;
            _maximum = maximum;
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            float motion = Patch200PresentationSettings.ReducedMotion ? 0.35f : 1f;
            float value = Mathf.Lerp(
                _minimum,
                _maximum,
                Mathf.Sin(Time.time * _frequency * motion) * 0.5f + 0.5f);
            transform.localScale = _baseScale * value;
        }
    }

    public sealed class ElementalStatusVisual2 : MonoBehaviour
    {
        private sealed class State
        {
            public ReactionElement element;
            public float progress;
            public float expiresAt;
            public bool major;
            public PooledPresentationParticle2 particles;
            public GameObject symbol;
        }

        private static int _activeAttachmentCount;
        private readonly Dictionary<ReactionElement, State> _states =
            new Dictionary<ReactionElement, State>();
        private EnemyController _owner;
        private float _nextRefresh;
        private bool _counted;

        public static int ActiveAttachmentCount
        {
            get { return _activeAttachmentCount; }
        }

        public static ElementalStatusVisual2 Attach(EnemyController owner)
        {
            if (owner == null)
                return null;

            ElementalStatusVisual2 visual =
                owner.GetComponent<ElementalStatusVisual2>();

            if (visual == null)
                visual = owner.gameObject.AddComponent<ElementalStatusVisual2>();

            visual._owner = owner;

            if (!visual._counted)
            {
                visual._counted = true;
                _activeAttachmentCount++;
            }

            return visual;
        }

        public void Set(
            ReactionElement element,
            float progress,
            float duration,
            bool major)
        {
            if (element == ReactionElement.None)
                return;

            State state;

            if (!_states.TryGetValue(element, out state))
            {
                state = new State { element = element };
                _states[element] = state;
            }

            state.progress = Mathf.Max(state.progress, Mathf.Clamp01(progress));
            state.expiresAt = Mathf.Max(state.expiresAt, Time.time + Mathf.Max(0.2f, duration));
            state.major |= major;
            Rebuild(state);
        }

        private void Rebuild(State state)
        {
            if (_owner == null)
                return;

            if (state.particles != null)
                state.particles.StopAndRelease();

            float intensity = Mathf.Lerp(0.35f, 1.3f, state.progress);
            if (state.major)
                intensity *= 1.35f;

            if (state.element == ReactionElement.Blood)
            {
                if (Patch200PresentationSettings.BloodMode == BloodPresentationMode.Hidden)
                    intensity = 0f;
                else if (Patch200PresentationSettings.BloodMode == BloodPresentationMode.Reduced)
                    intensity *= 0.3f;
            }

            if (intensity > 0.01f)
            {
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(state.element);
                state.particles = PresentationParticlePool2.Spawn(
                    new PresentationParticleRequest
                    {
                        purpose = PresentationParticlePurpose.Status,
                        position = _owner.DamagePoint,
                        follow = _owner.transform,
                        direction = Vector3.up,
                        signature = state.element,
                        primary = state.element,
                        primaryColor = profile == null ? ElementalReactionCodex.ColorFor(state.element) : profile.primary,
                        secondaryColor = profile == null ? Color.white : profile.secondary,
                        radius = Mathf.Lerp(0.25f, 0.75f, state.progress),
                        duration = Mathf.Max(0.5f, state.expiresAt - Time.time),
                        intensity = intensity,
                        speed = profile == null ? 1f : profile.particleSpeed,
                        seed = GetEntityId().GetHashCode() ^ (int)state.element,
                        count = Mathf.RoundToInt(5f + state.progress * 16f),
                        looping = true,
                        worldSpace = true,
                        priority = state.major
                            ? PresentationPriority.Critical
                            : PresentationPriority.Important
                    });
            }

            if (state.major && state.symbol == null)
            {
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(state.element);
                PrimitiveType type = profile == null ? PrimitiveType.Sphere : profile.accentShape;
                state.symbol = PresentationGeometry2.Primitive(
                    "AE2 " + ElementalReactionCodex.MajorStateName(state.element),
                    type,
                    _owner.DamagePoint,
                    Vector3.one * 0.18f,
                    profile == null ? ElementalReactionCodex.ColorFor(state.element) : profile.secondary,
                    Mathf.Max(0.5f, state.expiresAt - Time.time),
                    PresentationPriority.Critical,
                    false,
                    Vector3.zero);

                state.symbol.transform.SetParent(_owner.transform, true);
                state.symbol.transform.localPosition = Vector3.up * 1.15f;
                state.symbol.AddComponent<PresentationPulseScale2>()
                    .Initialize(3.5f, 0.82f, 1.2f);
            }
        }

        private void Update()
        {
            if (_owner == null || _owner.IsDead)
            {
                Destroy(this);
                return;
            }

            if (Time.time < _nextRefresh)
                return;

            _nextRefresh = Time.time + 0.25f;
            List<ReactionElement> expired = new List<ReactionElement>();

            foreach (KeyValuePair<ReactionElement, State> pair in _states)
            {
                State state = pair.Value;

                if (Time.time >= state.expiresAt)
                {
                    if (state.particles != null)
                        state.particles.StopAndRelease();

                    if (state.symbol != null)
                        Destroy(state.symbol);

                    expired.Add(pair.Key);
                }
            }

            for (int i = 0; i < expired.Count; i++)
                _states.Remove(expired[i]);

            if (_states.Count == 0)
                Destroy(this);
        }

        private void OnDestroy()
        {
            foreach (State state in _states.Values)
            {
                if (state.particles != null)
                    state.particles.StopAndRelease();

                if (state.symbol != null)
                    Destroy(state.symbol);
            }

            _states.Clear();

            if (_counted)
            {
                _counted = false;
                _activeAttachmentCount = Mathf.Max(0, _activeAttachmentCount - 1);
            }
        }
    }
}
