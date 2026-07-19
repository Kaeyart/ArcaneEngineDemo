using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class SpellMorphologyOwner21 : MonoBehaviour
    {
        public PresentationOwnerKind21 ownerKind;
        public int ownerId;
        public string contractId;
        public bool cancelOnDisable = true;
        private readonly List<GameObject> _owned = new List<GameObject>();

        public void Own(GameObject value)
        {
            if (value != null && !_owned.Contains(value))
                _owned.Add(value);
        }

        public void Release(GameObject value)
        {
            _owned.Remove(value);
        }

        public void Cancel()
        {
            for (int i = _owned.Count - 1; i >= 0; i--)
            {
                GameObject value = _owned[i];
                if (value != null)
                    Destroy(value);
            }
            _owned.Clear();
        }

        private void OnDisable()
        {
            if (cancelOnDisable)
                Cancel();
        }

        private void OnDestroy()
        {
            Cancel();
        }
    }

    public sealed class MorphologyBodyPart21 : MonoBehaviour
    {
        public BodyPartSpec21 spec;
        public Renderer targetRenderer;
        public MaterialPropertyBlock properties;
        public float phase;
        public float pulse;
        public float fracture;
        public float dissolve;
        public float compression;
        public Vector3 baseScale;
        public Vector3 basePosition;
        public Quaternion baseRotation;

        public void Initialize(BodyPartSpec21 value, Renderer renderer)
        {
            spec = value;
            targetRenderer = renderer;
            properties = new MaterialPropertyBlock();
            baseScale = transform.localScale;
            basePosition = transform.localPosition;
            baseRotation = transform.localRotation;
            phase = StableSeed21.Unit(value.seed) * Mathf.PI * 2f;
        }

        public void ApplySurface(
            float age,
            SpellPhase21 lifecyclePhase,
            float charge,
            float instability,
            bool returning)
        {
            if (spec == null)
                return;

            pulse = 0.5f + Mathf.Sin(age * (2.5f + spec.emissive * 1.4f) + phase) * 0.5f;
            float phaseScale = 1f;
            if (lifecyclePhase == SpellPhase21.Charge)
                phaseScale = Mathf.Lerp(0.65f, 1f, charge);
            else if (lifecyclePhase == SpellPhase21.Release)
                phaseScale = 1f + pulse * 0.18f;
            else if (lifecyclePhase == SpellPhase21.Contact)
                phaseScale = 1f + pulse * 0.28f;
            else if (lifecyclePhase == SpellPhase21.Expire || lifecyclePhase == SpellPhase21.Cancel)
                phaseScale = Mathf.Max(0.02f, 1f - dissolve);

            if (spec.kind == BodyPartKind21.InternalEnergy)
                phaseScale *= 0.75f + pulse * 0.45f;
            if (spec.kind == BodyPartKind21.FractureSeam)
                phaseScale *= Mathf.Lerp(0.4f, 1.2f, fracture);
            if (spec.kind == BodyPartKind21.Shell)
                phaseScale *= 1f + compression * 0.18f;

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                baseScale * phaseScale,
                Mathf.Clamp01(Time.unscaledDeltaTime * 14f));

            if (returning)
                transform.localRotation = baseRotation * Quaternion.Euler(0f, -age * 140f, 0f);
            else if (spec.kind == BodyPartKind21.InternalEnergy || spec.kind == BodyPartKind21.Ring)
                transform.localRotation = baseRotation * Quaternion.Euler(0f, age * (35f + instability * 120f), 0f);

            if (targetRenderer != null)
            {
                targetRenderer.GetPropertyBlock(properties);
                properties.SetFloat("_Pulse", pulse);
                properties.SetFloat("_Charge", charge);
                properties.SetFloat("_Fracture", fracture);
                properties.SetFloat("_Dissolve", dissolve);
                properties.SetFloat("_Compression", compression);
                properties.SetFloat("_FlowDirection", returning ? -1f : 1f);
                properties.SetFloat("_Instability", instability);
                targetRenderer.SetPropertyBlock(properties);
            }
        }
    }

    public static class MorphologyBodyBuilder21
    {
        public static GameObject Build(
            Transform parent,
            SpellVisualContract21 contract,
            GeneratedSpellHostKind kind,
            SpellMorphologyOwner21 owner,
            List<MorphologyBodyPart21> output)
        {
            if (parent == null || contract == null)
                return null;

            GameObject root = new GameObject("AE21 Morphology · " + contract.displayName);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            if (owner != null) owner.Own(root);

            int quality = (int)Patch200PresentationSettings.Quality;
            for (int i = 0; i < contract.bodyParts.Count; i++)
            {
                BodyPartSpec21 spec = contract.bodyParts[i];
                if (!spec.required && spec.qualityMinimum > quality)
                    continue;
                if (spec.qualityMinimum > quality)
                    continue;

                GameObject part = new GameObject(spec.id);
                part.transform.SetParent(root.transform, false);
                part.transform.localPosition = spec.localPosition;
                part.transform.localRotation = Quaternion.Euler(spec.localEuler);
                part.transform.localScale = spec.localScale;

                MeshFilter filter = part.AddComponent<MeshFilter>();
                filter.sharedMesh = GeneratedAssetRuntime21.Mesh(spec.meshKey, spec.seed);
                MeshRenderer renderer = part.AddComponent<MeshRenderer>();
                Color primary = GeneratedAssetRuntime21.ElementColor(spec.element, false);
                Color secondary = GeneratedAssetRuntime21.ElementColor(spec.element, true);
                renderer.sharedMaterial = GeneratedAssetRuntime21.Material(
                    spec.kind,
                    spec.element,
                    primary,
                    secondary,
                    spec.emissive,
                    spec.opacity,
                    spec.seed);
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;

                MorphologyBodyPart21 bodyPart = part.AddComponent<MorphologyBodyPart21>();
                bodyPart.Initialize(spec, renderer);
                output.Add(bodyPart);
            }

            BuildRuneCircuits(root.transform, contract, quality);
            BuildRequiredTrail(root.transform, contract, kind);
            return root;
        }

        private static void BuildRuneCircuits(
            Transform root,
            SpellVisualContract21 contract,
            int quality)
        {
            if (contract.runeGraph.Count == 0)
                return;

            LineRenderer circuit = root.gameObject.AddComponent<LineRenderer>();
            circuit.name = "AE21 Rune Circuit";
            circuit.useWorldSpace = false;
            circuit.loop = contract.topology != null && contract.topology.approximatelySymmetric;
            circuit.positionCount = contract.runeGraph.Count + 1;
            circuit.startWidth = Mathf.Clamp(contract.size * 0.018f, 0.012f, 0.055f);
            circuit.endWidth = circuit.startWidth * 0.55f;
            circuit.numCapVertices = quality >= 1 ? 4 : 0;
            circuit.numCornerVertices = quality >= 2 ? 4 : 0;
            circuit.material = GeneratedAssetRuntime21.Material(
                BodyPartKind21.InternalPath,
                contract.catalyst,
                GeneratedAssetRuntime21.ElementColor(contract.catalyst, false),
                GeneratedAssetRuntime21.ElementColor(contract.catalyst, true),
                1.5f,
                0.8f,
                contract.seeds.operators);
            circuit.startColor = GeneratedAssetRuntime21.ElementColor(contract.catalyst, true);
            circuit.endColor = GeneratedAssetRuntime21.ElementColor(contract.primaryElement, false);
            circuit.SetPosition(0, Vector3.zero);

            for (int i = 0; i < contract.runeGraph.Count; i++)
            {
                RuneOperatorNode21 node = contract.runeGraph[i];
                float angle = node.rotation / 6f * Mathf.PI * 2f;
                float distance = Mathf.Clamp(0.2f + (Mathf.Abs(node.boardQ) + Mathf.Abs(node.boardR)) * 0.07f, 0.2f, 0.75f);
                circuit.SetPosition(i + 1, new Vector3(Mathf.Cos(angle) * distance, (node.branch - 1) * 0.05f, Mathf.Sin(angle) * distance));
            }
        }

        private static void BuildRequiredTrail(
            Transform root,
            SpellVisualContract21 contract,
            GeneratedSpellHostKind kind)
        {
            if (kind != GeneratedSpellHostKind.Projectile && kind != GeneratedSpellHostKind.Familiar)
                return;

            TrailRenderer trail = root.gameObject.AddComponent<TrailRenderer>();
            trail.time = Mathf.Clamp(0.16f + contract.size * 0.1f, 0.14f, 1.2f);
            trail.minVertexDistance = Patch200PresentationSettings.Quality == PresentationQuality.Low ? 0.12f : 0.04f;
            trail.startWidth = Mathf.Clamp(contract.size * 0.12f, 0.035f, 0.42f);
            trail.endWidth = 0f;
            trail.numCapVertices = Patch200PresentationSettings.Quality == PresentationQuality.High ? 4 : 0;
            trail.numCornerVertices = Patch200PresentationSettings.Quality == PresentationQuality.High ? 3 : 0;
            trail.startColor = GeneratedAssetRuntime21.ElementColor(contract.motionElement, false);
            Color end = GeneratedAssetRuntime21.ElementColor(contract.residueElement, true);
            end.a = 0f;
            trail.endColor = end;
            trail.material = GeneratedAssetRuntime21.Material(
                BodyPartKind21.TrailEmitter,
                contract.motionElement,
                trail.startColor,
                GeneratedAssetRuntime21.ElementColor(contract.motionElement, true),
                1.2f,
                0.85f,
                contract.seeds.trail);
        }
    }

    public sealed class GeneratedSpellMorphologyHost21 : MonoBehaviour
    {
        private SpellVisualContract21 _contract;
        private CompiledSpell _spell;
        private CastRequest _request;
        private GeneratedSpellHostKind _kind;
        private SpellMorphologyOwner21 _owner;
        private GameObject _bodyRoot;
        private readonly List<MorphologyBodyPart21> _parts = new List<MorphologyBodyPart21>();
        private float _created;
        private float _nextNearMiss;
        private float _lastSpeed;
        private Vector3 _lastPosition;
        private bool _returning;
        private bool _initialized;
        private int _phaseIndex;
        private bool _debugPhaseOverride;
        private SpellPhase21 _debugPhase;
        private float _debugProgress;

        public SpellVisualContract21 Contract { get { return _contract; } }
        public SpellPhase21 CurrentPhase { get; private set; }

        public void Initialize(
            SpellVisualContract21 contract,
            CompiledSpell spell,
            CastRequest request,
            GeneratedSpellHostKind kind)
        {
            _contract = contract;
            _spell = spell;
            _request = request;
            _kind = kind;
            if (_contract == null)
                return;

            if (_initialized)
            {
                Rebuild();
                return;
            }

            _initialized = true;
            _created = Time.unscaledTime;
            _lastPosition = transform.position;
            _owner = GetComponent<SpellMorphologyOwner21>();
            if (_owner == null) _owner = gameObject.AddComponent<SpellMorphologyOwner21>();
            _owner.ownerKind = kind == GeneratedSpellHostKind.Zone
                ? PresentationOwnerKind21.Field
                : kind == GeneratedSpellHostKind.Familiar
                    ? PresentationOwnerKind21.Familiar
                    : PresentationOwnerKind21.Projectile;
            _owner.ownerId = GetInstanceID();
            _owner.contractId = _contract.contractId;
            Rebuild();
            CreateElementEmitters();
            ProceduralSpellAudio21.PlayPhase(_contract, SpellPhase21.Emit, transform.position, transform);
        }

        public void SetDebugPhase(SpellPhase21 phase, float progress)
        {
            _debugPhaseOverride = true;
            _debugPhase = phase;
            _debugProgress = Mathf.Clamp01(progress);
        }

        public void ClearDebugPhase()
        {
            _debugPhaseOverride = false;
        }

        public void MarkReturning()
        {
            _returning = true;
            CurrentPhase = SpellPhase21.Return;
            ProceduralSpellAudio21.PlayPhase(_contract, SpellPhase21.Return, transform.position, transform);
        }

        public void MarkContact(Vector3 position)
        {
            CurrentPhase = SpellPhase21.Contact;
            ApplyPartState(1f, 0.75f);
        }

        public void MarkExpire()
        {
            CurrentPhase = SpellPhase21.Expire;
            ApplyPartState(0.35f, 1f);
        }

        private void Rebuild()
        {
            if (_bodyRoot != null)
                Destroy(_bodyRoot);
            _parts.Clear();
            _bodyRoot = MorphologyBodyBuilder21.Build(transform, _contract, _kind, _owner, _parts);
        }

        private void CreateElementEmitters()
        {
            if (_contract.baseRecipe == null)
                return;

            for (int i = 0; i < _contract.baseRecipe.elementLayers.Count; i++)
            {
                ElementVisualLayer layer = _contract.baseRecipe.elementLayers[i];
                if (layer.element == ReactionElement.Blood && Patch200PresentationSettings.BloodMode == BloodPresentationMode.Hidden)
                    continue;

                float intensity = layer.intensity;
                if (layer.element == ReactionElement.Blood && Patch200PresentationSettings.BloodMode == BloodPresentationMode.Reduced)
                    intensity *= 0.35f;

                PresentationParticleRequest request = new PresentationParticleRequest
                {
                    purpose = _kind == GeneratedSpellHostKind.Zone
                        ? PresentationParticlePurpose.Field
                        : PresentationParticlePurpose.Trail,
                    position = transform.position,
                    direction = transform.forward,
                    follow = transform,
                    signature = _contract.signature,
                    primary = layer.element,
                    primaryColor = layer.primary,
                    secondaryColor = layer.secondary,
                    radius = _kind == GeneratedSpellHostKind.Zone ? _contract.radius : Mathf.Max(0.15f, _contract.size * 0.23f),
                    duration = Mathf.Max(0.8f, _contract.duration + 0.8f),
                    intensity = intensity,
                    speed = Mathf.Max(0.5f, _contract.speed * 0.08f),
                    seed = StableSeed21.Combine(_contract.seeds.elements, i),
                    count = Mathf.Clamp(8 + Mathf.RoundToInt(intensity * 16f), 8, 40),
                    looping = true,
                    worldSpace = true,
                    priority = layer.role == ElementVisualRole.PrimarySilhouette || layer.role == ElementVisualRole.Catalyst
                        ? PresentationPriority.Important
                        : PresentationPriority.Decorative
                };
                PresentationParticlePool2.Spawn(request);
            }
        }

        private void Update()
        {
            if (!_initialized || _contract == null)
                return;

            float age = Time.unscaledTime - _created;
            float normalized = _kind == GeneratedSpellHostKind.Zone
                ? Mathf.Repeat(age / Mathf.Max(0.5f, _contract.duration), 1f)
                : Mathf.Clamp01(age / Mathf.Max(0.35f, _contract.duration));
            CurrentPhase = _debugPhaseOverride ? _debugPhase : ResolvePhase(normalized);
            if (_debugPhaseOverride) normalized = _debugProgress;
            float charge = CurrentPhase == SpellPhase21.Charge
                ? Mathf.InverseLerp(0f, 0.25f, normalized)
                : 1f;
            float instability = Instability();
            float fracture = FractureForPhase(normalized);
            float dissolve = CurrentPhase == SpellPhase21.Expire || CurrentPhase == SpellPhase21.Cancel
                ? Mathf.InverseLerp(0.88f, 1f, normalized)
                : 0f;
            float compression = CurrentPhase == SpellPhase21.Hold || CurrentPhase == SpellPhase21.Return
                ? 0.6f + Mathf.Sin(age * 7f) * 0.25f
                : 0f;

            for (int i = 0; i < _parts.Count; i++)
            {
                MorphologyBodyPart21 part = _parts[i];
                if (part == null) continue;
                part.fracture = fracture;
                part.dissolve = dissolve;
                part.compression = compression;
                part.ApplySurface(age, CurrentPhase, charge, instability, _returning);
                AnimateStructuralPart(part, age, normalized);
            }

            UpdateVelocity(age);
            if (Patch210Settings21.NearMiss && Time.unscaledTime >= _nextNearMiss)
            {
                _nextNearMiss = Time.unscaledTime + 0.10f;
                NearMissPresentation21.Sample(this, _lastSpeed);
            }
        }

        private void AnimateStructuralPart(MorphologyBodyPart21 part, float age, float normalized)
        {
            BodyPartSpec21 spec = part.spec;
            if (spec == null) return;

            if (spec.kind == BodyPartKind21.Orbital)
            {
                float direction = RotationDirection(spec.runeKind);
                float angle = age * (1.5f + Mathf.Abs(_contract.speed) * 0.05f) * direction + part.phase;
                float radius = Mathf.Max(0.2f, spec.localPosition.magnitude);
                part.transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle * 1.7f) * 0.12f, Mathf.Sin(angle) * radius);
            }
            else if (spec.kind == BodyPartKind21.SecondaryCore)
            {
                float split = HasPhasePassed(SpellPhase21.Emit, normalized) ? 1f : 0.15f;
                part.transform.localPosition = Vector3.Lerp(Vector3.zero, spec.localPosition * (1f + split * 1.7f), split);
            }
            else if (spec.kind == BodyPartKind21.FieldPanel && CurrentPhase == SpellPhase21.Persist)
            {
                float unfold = Mathf.Clamp01((normalized - 0.35f) * 4f);
                part.transform.localScale = Vector3.Lerp(Vector3.one * 0.05f, part.baseScale, unfold);
                part.transform.localPosition = Vector3.down * 0.08f * unfold;
            }
            else if (spec.kind == BodyPartKind21.RuneNode)
            {
                float pulse = 0.82f + Mathf.Sin(age * 4f + part.phase) * 0.18f;
                part.transform.localScale = part.baseScale * pulse;
            }
        }

        private SpellPhase21 ResolvePhase(float normalized)
        {
            if (_returning)
                return SpellPhase21.Return;
            for (int i = 0; i < _contract.lifecycle.Count; i++)
            {
                LifecycleNode21 phase = _contract.lifecycle[i];
                if (normalized >= phase.normalizedStart && normalized < phase.normalizedStart + phase.normalizedDuration)
                {
                    if (_phaseIndex != i)
                    {
                        _phaseIndex = i;
                        ProceduralSpellAudio21.PlayPhase(_contract, phase.phase, transform.position, transform);
                    }
                    return phase.phase;
                }
            }
            return SpellPhase21.Expire;
        }

        private bool HasPhasePassed(SpellPhase21 phase, float normalized)
        {
            for (int i = 0; i < _contract.lifecycle.Count; i++)
                if (_contract.lifecycle[i].phase == phase)
                    return normalized >= _contract.lifecycle[i].normalizedStart;
            return false;
        }

        private float FractureForPhase(float normalized)
        {
            bool split = false;
            bool delay = false;
            for (int i = 0; i < _contract.runeGraph.Count; i++)
            {
                split |= _contract.runeGraph[i].kind == RuneVisualOperatorKind.Split;
                delay |= _contract.runeGraph[i].kind == RuneVisualOperatorKind.Delay;
            }
            if (split && normalized < 0.38f)
                return Mathf.InverseLerp(0.08f, 0.38f, normalized);
            if (delay && CurrentPhase == SpellPhase21.Hold)
                return 0.45f + Mathf.PingPong(Time.unscaledTime * 0.7f, 0.55f);
            return 0f;
        }

        private float Instability()
        {
            float value = _contract.runeGraph.Count / 10f;
            if (_contract.personality == VisualPersonality21.Unstable) value += 0.45f;
            if ((int)_contract.reactionTier >= (int)ReactionTier.Calamity) value += 0.25f;
            return Mathf.Clamp01(value);
        }

        private float RotationDirection(RuneVisualOperatorKind kind)
        {
            for (int i = 0; i < _contract.runeGraph.Count; i++)
            {
                if (_contract.runeGraph[i].kind == kind)
                    return (_contract.runeGraph[i].rotation & 1) == 0 ? 1f : -1f;
            }
            return 1f;
        }

        private void UpdateVelocity(float age)
        {
            Vector3 current = transform.position;
            float delta = Mathf.Max(0.0001f, Time.unscaledDeltaTime);
            _lastSpeed = Vector3.Distance(current, _lastPosition) / delta;
            _lastPosition = current;
        }

        private void ApplyPartState(float fracture, float compression)
        {
            for (int i = 0; i < _parts.Count; i++)
            {
                if (_parts[i] == null) continue;
                _parts[i].fracture = fracture;
                _parts[i].compression = compression;
            }
        }

        private void OnDisable()
        {
            CurrentPhase = SpellPhase21.Cancel;
        }
    }

    public static class SpellMorphologyPresentation21
    {
        public static SpellVisualContract21 LastContract { get; private set; }

        public static SpellVisualContract21 Compile(
            CompiledSpell spell,
            SpellBoard board,
            GeneratedVisualRecipe recipe)
        {
            LastContract = SpellMorphologyCompiler21.Compile(spell, board, recipe);
            return LastContract;
        }

        public static SpellVisualContract21 Contract(CompiledSpell spell)
        {
            SpellVisualContract21 contract = SpellMorphologyCompiler21.Get(spell);
            if (contract != null) LastContract = contract;
            return contract;
        }

        public static void AttachHost(
            GameObject host,
            CompiledSpell spell,
            CastRequest request,
            GeneratedSpellHostKind kind)
        {
            if (host == null || spell == null)
                return;
            GeneratedSpellMorphologyHost21 visual = host.GetComponent<GeneratedSpellMorphologyHost21>();
            if (visual == null) visual = host.AddComponent<GeneratedSpellMorphologyHost21>();
            visual.Initialize(Contract(spell), spell, request, kind);
        }

        public static void EmitCast(CompiledSpell spell, CastRequest request)
        {
            SpellVisualContract21 contract = Contract(spell);
            if (contract == null) return;
            Vector3 position = request == null ? PlayerPosition() : request.origin;
            Vector3 direction = request == null ? Vector3.forward : request.direction;
            MorphologyPresentationDirector21.Instance.EmitCast(contract, position, direction);
        }

        public static void EmitImpact(
            CompiledSpell spell,
            CastRequest request,
            Vector3 position,
            bool critical)
        {
            SpellVisualContract21 contract = Contract(spell);
            if (contract == null) return;
            Vector3 direction = request == null ? Vector3.down : request.direction;
            MorphologyPresentationDirector21.Instance.EmitImpact(contract, position, direction, critical);
            MarkNearbyHost(position, false, true);
        }

        public static void EmitExpire(
            CompiledSpell spell,
            Vector3 position)
        {
            SpellVisualContract21 contract = Contract(spell);
            if (contract == null) return;
            MorphologyPresentationDirector21.Instance.EmitExpire(contract, position);
            Collider[] colliders = Physics.OverlapSphere(position, 1.5f);
            for (int i = 0; i < colliders.Length; i++)
            {
                GeneratedSpellMorphologyHost21 host = colliders[i].GetComponentInParent<GeneratedSpellMorphologyHost21>();
                if (host != null) host.MarkExpire();
            }
        }

        public static void EmitDirectionChange(
            CompiledSpell spell,
            Vector3 position,
            bool returning)
        {
            SpellVisualContract21 contract = Contract(spell);
            if (contract == null) return;
            MorphologyPresentationDirector21.Instance.EmitDirectionChange(contract, position, returning);
            MarkNearbyHost(position, returning, false);
        }

        public static void EmitMovement(
            CompiledSpell spell,
            Vector3 from,
            Vector3 to)
        {
            SpellVisualContract21 contract = Contract(spell);
            if (contract == null) return;
            MorphologyPresentationDirector21.Instance.EmitMovement(contract, from, to);
        }

        private static void MarkNearbyHost(Vector3 position, bool returning, bool contact)
        {
            Collider[] colliders = Physics.OverlapSphere(position, 1.5f);
            for (int i = 0; i < colliders.Length; i++)
            {
                GeneratedSpellMorphologyHost21 host = colliders[i].GetComponentInParent<GeneratedSpellMorphologyHost21>();
                if (host == null) continue;
                if (returning) host.MarkReturning();
                if (contact) host.MarkContact(position);
            }
        }

        private static Vector3 PlayerPosition()
        {
            return GameWorld.Instance != null && GameWorld.Instance.Player != null
                ? GameWorld.Instance.Player.transform.position
                : Vector3.zero;
        }
    }

    public static class NearMissPresentation21
    {
        private static float _nextGlobal;

        public static void Sample(GeneratedSpellMorphologyHost21 host, float speed)
        {
            if (host == null || host.Contract == null || speed < 3f || Time.unscaledTime < _nextGlobal)
                return;
            GameWorld world = GameWorld.Instance;
            if (world == null || world.Player == null || host.gameObject == world.Player.gameObject)
                return;

            float distance = Vector3.Distance(host.transform.position, world.Player.transform.position);
            float radius = Mathf.Clamp(0.55f + host.Contract.size * 0.25f, 0.55f, 1.8f);
            if (distance > radius)
                return;

            _nextGlobal = Time.unscaledTime + 0.18f;
            Vector3 direction = (host.transform.position - world.Player.transform.position).normalized;
            Vector3 from = world.Player.transform.position + Vector3.up;
            PresentationMotionStreaks2.Spawn(
                from,
                from + direction * Mathf.Clamp(speed * 0.04f, 0.3f, 1.4f),
                host.Contract.signature,
                3,
                0.18f,
                PresentationPriority.Normal,
                host.Contract.seeds.trail);
            ProceduralSpellAudio21.PlayNearMiss(host.Contract, world.Player.transform.position, speed);
        }
    }
}
