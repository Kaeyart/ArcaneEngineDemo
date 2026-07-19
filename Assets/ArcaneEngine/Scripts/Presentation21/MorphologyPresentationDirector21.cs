using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcaneEngine
{
    public sealed class MorphologyPresentationDirector21 : MonoBehaviour
    {
        private static MorphologyPresentationDirector21 _instance;
        private readonly Dictionary<int, TargetSurfaceResponse21> _targetResponses =
            new Dictionary<int, TargetSurfaceResponse21>();
        private readonly List<SpellMorphologyOwner21> _sceneOwners =
            new List<SpellMorphologyOwner21>();
        private float _lastFlash;
        private float _flashEnergy;
        private float _lastImportantEvent;

        public static MorphologyPresentationDirector21 Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject root = new GameObject("Arcane Engine Patch 2.1 Presentation");
                    DontDestroyOnLoad(root);
                    _instance = root.AddComponent<MorphologyPresentationDirector21>();
                    root.AddComponent<Patch210RuntimeOverlay21>();
                    root.AddComponent<Patch210ReferenceRuntime21>();
                }
                return _instance;
            }
        }

        public int ActiveTargetResponses
        {
            get
            {
                CleanupTargets();
                return _targetResponses.Count;
            }
        }

        public float FlashEnergy { get { return _flashEnergy; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SpellPresentationBus.Published += OnPresentationEvent;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SpellPresentationBus.Published -= OnPresentationEvent;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_instance == this) _instance = null;
        }

        private void Update()
        {
            _flashEnergy = Mathf.MoveTowards(_flashEnergy, 0f, Time.unscaledDeltaTime * 1.8f);
            CleanupTargets();
        }

        public void EmitCast(
            SpellVisualContract21 contract,
            Vector3 position,
            Vector3 direction)
        {
            if (contract == null) return;
            SpellMorphologyOwner21 owner = CreateSceneOwner(
                "AE21 Cast · " + contract.displayName,
                PresentationOwnerKind21.Cast,
                contract,
                position,
                1.25f);
            CastAnticipation21.Build(owner, contract, position, direction);
            ProceduralSpellAudio21.PlayPhase(contract, SpellPhase21.Charge, position, owner.transform);
            SpellCameraPost21.RequestImpulse(direction, 0.06f + (int)contract.priority * 0.025f, 0.12f);
        }

        public void EmitImpact(
            SpellVisualContract21 contract,
            Vector3 position,
            Vector3 direction,
            bool critical)
        {
            if (contract == null) return;
            ContactSurface21 surface;
            Vector3 normal;
            Collider target;
            EnvironmentContactResolver21.Resolve(position, direction, out surface, out normal, out target);
            ImpactComposition21.Build(contract, position, normal, surface, critical);
            if (Patch210Settings21.TargetResponse && target != null)
                ApplyTargetResponse(contract, target, position, normal, critical);

            float emphasis = Mathf.Clamp(
                0.08f + contract.radius * 0.018f + (critical ? 0.10f : 0f) +
                (int)contract.priority * 0.035f,
                0.06f,
                0.42f);
            SpellCameraPost21.RequestImpulse(-direction.normalized, emphasis, 0.16f + emphasis * 0.25f);
            RequestFlash(emphasis * 1.4f);
            ProceduralSpellAudio21.PlayPhase(contract, SpellPhase21.Resolve, position, null);

            if ((int)contract.priority >= (int)PresentationPriority.Critical)
                ProceduralSpellAudio21.Duck(0.55f, 0.28f);
            else if (critical)
                ProceduralSpellAudio21.Duck(0.78f, 0.12f);
        }

        public void EmitExpire(
            SpellVisualContract21 contract,
            Vector3 position)
        {
            if (contract == null) return;
            SpellMorphologyOwner21 owner = CreateSceneOwner(
                "AE21 Expire · " + contract.displayName,
                PresentationOwnerKind21.Cast,
                contract,
                position,
                0.55f);
            Color core = GeneratedAssetRuntime21.ElementColor(contract.primaryElement, true);
            Color accent = GeneratedAssetRuntime21.ElementColor(contract.catalyst, false);
            PresentationGeometry2.Ring(
                position,
                core,
                Mathf.Max(0.18f, contract.size * 0.34f),
                0.035f,
                0.35f,
                PresentationPriority.Normal,
                true);
            PresentationMotionStreaks2.SpawnInward(
                position,
                contract.signature,
                Mathf.Max(0.35f, contract.size * 0.7f),
                Mathf.Clamp(4 + contract.runeGraph.Count, 4, 12),
                0.34f,
                PresentationPriority.Normal,
                contract.seeds.impact);
            GameObject fragment = GameObject.CreatePrimitive(PrimitiveType.Quad);
            fragment.name = "AE21 Expiration Glyph";
            fragment.transform.SetParent(owner.transform, false);
            fragment.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            fragment.transform.localScale = Vector3.one * Mathf.Max(0.12f, contract.size * 0.18f);
            Collider collider = fragment.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            Renderer renderer = fragment.GetComponent<Renderer>();
            renderer.sharedMaterial = GeneratedAssetRuntime21.Material(
                BodyPartKind21.RuneNode,
                contract.primaryElement,
                core,
                accent,
                1.15f,
                0.82f,
                contract.seeds.impact);
            TransientScale21 scale = fragment.AddComponent<TransientScale21>();
            scale.Initialize(0.38f, Vector3.one * 0.4f, Vector3.one * 2.2f);
            ProceduralSpellAudio21.PlayPhase(contract, SpellPhase21.Expire, position, owner.transform);
        }

        public void EmitDirectionChange(
            SpellVisualContract21 contract,
            Vector3 position,
            bool returning)
        {
            if (contract == null) return;
            Color color = GeneratedAssetRuntime21.ElementColor(contract.motionElement, false);
            PresentationGeometry2.Ring(
                position,
                color,
                Mathf.Clamp(contract.size * 0.45f, 0.18f, 1.2f),
                0.035f,
                0.22f,
                PresentationPriority.Important,
                true);
            PresentationMotionStreaks2.SpawnInward(
                position,
                contract.signature,
                Mathf.Clamp(contract.size * 0.8f, 0.4f, 1.6f),
                returning ? 8 : 5,
                0.2f,
                PresentationPriority.Normal,
                contract.seeds.trail);
            ProceduralSpellAudio21.PlayPhase(
                contract,
                returning ? SpellPhase21.Return : SpellPhase21.Contact,
                position,
                null);
        }

        public void EmitMovement(
            SpellVisualContract21 contract,
            Vector3 from,
            Vector3 to)
        {
            if (contract == null) return;
            PresentationMotionStreaks2.Spawn(
                from,
                to,
                contract.signature,
                Mathf.Clamp(3 + contract.runeGraph.Count, 3, 12),
                0.42f,
                PresentationPriority.Important,
                contract.seeds.trail);
            PresentationGeometry2.Ring(
                from,
                GeneratedAssetRuntime21.ElementColor(contract.primaryElement, false),
                0.45f,
                0.04f,
                0.22f,
                PresentationPriority.Normal,
                true);
            PresentationGeometry2.Ring(
                to,
                GeneratedAssetRuntime21.ElementColor(contract.catalyst, true),
                0.6f,
                0.05f,
                0.3f,
                PresentationPriority.Important,
                true);
            SpellCameraPost21.RequestImpulse((to - from).normalized, 0.08f, 0.16f);
            ProceduralSpellAudio21.PlayPhase(contract, SpellPhase21.Travel, from, null);
            ProceduralSpellAudio21.PlayPhase(contract, SpellPhase21.Contact, to, null);
        }

        public void RequestFlash(float energy)
        {
            float now = Time.unscaledTime;
            float limiter = Patch210Settings21.FlashLimit;
            float minimumInterval = Mathf.Lerp(0.38f, 0.08f, limiter);
            if (now - _lastFlash < minimumInterval)
                energy *= Mathf.Clamp01((now - _lastFlash) / minimumInterval);
            energy = Mathf.Min(energy, Mathf.Max(0.08f, 1f - _flashEnergy));
            if (energy <= 0.01f) return;
            _lastFlash = now;
            _flashEnergy = Mathf.Clamp01(_flashEnergy + energy * limiter);
            SpellCameraPost21.RequestFlash(_flashEnergy, 0.06f + energy * 0.08f);
        }

        private void OnPresentationEvent(SpellPresentationEvent value)
        {
            GeneratedVisualRecipe recipe = value.recipe;
            CompiledSpell spell = value.spell;
            SpellVisualContract21 contract = spell == null
                ? null
                : SpellMorphologyCompiler21.Get(spell);

            if (contract == null && recipe != null)
                contract = SpellMorphologyPresentation21.LastContract;

            switch (value.type)
            {
                case SpellPresentationEventType.AilmentApplied:
                case SpellPresentationEventType.AilmentBuildupChanged:
                case SpellPresentationEventType.MajorAilmentActivated:
                    if (value.target != null)
                        ApplyStatusResponse(value, contract);
                    break;

                case SpellPresentationEventType.ReactionAssemblyStarted:
                case SpellPresentationEventType.ReactionSignatureUpgraded:
                    BuildReactionAssembly(value, contract);
                    break;

                case SpellPresentationEventType.ReactionResolved:
                case SpellPresentationEventType.ReactionDeathTriggered:
                    BuildReactionResolution(value, contract);
                    break;

                case SpellPresentationEventType.FieldMerged:
                case SpellPresentationEventType.FieldPulsed:
                    BuildFieldEvent(value, contract);
                    break;

                case SpellPresentationEventType.BarrierChanged:
                    BuildBarrierEvent(value, contract);
                    break;
            }
        }

        private void ApplyTargetResponse(
            SpellVisualContract21 contract,
            Collider collider,
            Vector3 position,
            Vector3 normal,
            bool critical)
        {
            Transform root = collider.transform;
            EnemyController enemy = collider.GetComponentInParent<EnemyController>();
            if (enemy != null) root = enemy.transform;
            int id = root.GetEntityId().GetHashCode();
            TargetSurfaceResponse21 response;
            if (!_targetResponses.TryGetValue(id, out response) || response == null)
            {
                response = root.GetComponent<TargetSurfaceResponse21>();
                if (response == null) response = root.gameObject.AddComponent<TargetSurfaceResponse21>();
                _targetResponses[id] = response;
            }
            response.ApplyImpact(contract, position, normal, critical);
        }

        private void ApplyStatusResponse(
            SpellPresentationEvent value,
            SpellVisualContract21 contract)
        {
            if (value.target == null) return;
            int id = value.target.GetEntityId().GetHashCode();
            TargetSurfaceResponse21 response;
            if (!_targetResponses.TryGetValue(id, out response) || response == null)
            {
                response = value.target.GetComponent<TargetSurfaceResponse21>();
                if (response == null) response = value.target.gameObject.AddComponent<TargetSurfaceResponse21>();
                _targetResponses[id] = response;
            }
            response.ApplyStatus(
                value.signature,
                value.stackCount,
                value.threshold,
                value.type == SpellPresentationEventType.MajorAilmentActivated,
                contract);
        }

        private void BuildReactionAssembly(
            SpellPresentationEvent value,
            SpellVisualContract21 contract)
        {
            ReactionElement signature = value.signature;
            Color color = ElementalReactionCodex.BlendColor(signature);
            float radius = Mathf.Max(0.7f, value.radius);
            PresentationGeometry2.Ring(
                value.position,
                color,
                radius,
                0.055f,
                Mathf.Max(0.25f, value.duration),
                PresentationPriority.Critical,
                false);
            PresentationMotionStreaks2.SpawnInward(
                value.position,
                signature,
                radius,
                6 + ElementalReactionCodex.CountBits(signature) * 2,
                Mathf.Max(0.25f, value.duration),
                PresentationPriority.Important,
                value.mechanicId == null ? 210 : value.mechanicId.GetHashCode());
            RequestFlash(0.08f + ElementalReactionCodex.CountBits(signature) * 0.025f);
        }

        private void BuildReactionResolution(
            SpellPresentationEvent value,
            SpellVisualContract21 contract)
        {
            ReactionElement signature = value.signature;
            int count = Mathf.Max(2, ElementalReactionCodex.CountBits(signature));
            float radius = Mathf.Max(1f, value.radius);
            PresentationGeometry2.Burst(
                value.position,
                signature,
                radius,
                Mathf.Clamp(value.duration, 0.3f, 1.2f),
                Mathf.Max(1f, value.intensity),
                count >= 5 ? PresentationPriority.Reserved : PresentationPriority.Critical,
                value.mechanicId == null ? 421 : value.mechanicId.GetHashCode());
            PresentationMotionStreaks2.SpawnOutward(
                value.position,
                signature,
                radius,
                5 + count * 2,
                0.34f + count * 0.04f,
                PresentationPriority.Important,
                count * 313);
            RequestFlash(Mathf.Clamp(0.12f + count * 0.055f, 0.15f, 0.62f));
            if (contract != null)
                ProceduralSpellAudio21.PlayPhase(contract, SpellPhase21.Resolve, value.position, null);
            if (count >= 5)
                ProceduralSpellAudio21.Duck(count == 7 ? 0.34f : 0.48f, 0.35f + count * 0.05f);
        }

        private void BuildFieldEvent(
            SpellPresentationEvent value,
            SpellVisualContract21 contract)
        {
            float radius = Mathf.Max(0.6f, value.radius);
            Color color = ElementalReactionCodex.BlendColor(value.signature);
            PresentationGeometry2.Ring(
                value.position,
                color,
                radius,
                0.035f,
                0.35f,
                PresentationPriority.Important,
                value.type == SpellPresentationEventType.FieldPulsed);
            if (value.type == SpellPresentationEventType.FieldMerged)
            {
                PresentationMotionStreaks2.SpawnInward(
                    value.position,
                    value.signature,
                    radius,
                    10,
                    0.38f,
                    PresentationPriority.Important,
                    value.mechanicId == null ? 17 : value.mechanicId.GetHashCode());
            }
        }

        private void BuildBarrierEvent(
            SpellPresentationEvent value,
            SpellVisualContract21 contract)
        {
            Color color = contract == null
                ? Color.cyan
                : GeneratedAssetRuntime21.ElementColor(contract.primaryElement, false);
            PresentationGeometry2.Ring(
                value.position,
                color,
                Mathf.Max(0.75f, value.radius),
                value.critical ? 0.09f : 0.05f,
                0.32f,
                PresentationPriority.Important,
                value.critical);
        }

        private SpellMorphologyOwner21 CreateSceneOwner(
            string name,
            PresentationOwnerKind21 kind,
            SpellVisualContract21 contract,
            Vector3 position,
            float lifetime)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.position = position;
            SpellMorphologyOwner21 owner = gameObject.AddComponent<SpellMorphologyOwner21>();
            owner.ownerKind = kind;
            owner.ownerId = gameObject.GetEntityId().GetHashCode();
            owner.contractId = contract == null ? string.Empty : contract.contractId;
            _sceneOwners.Add(owner);
            Destroy(gameObject, Mathf.Max(0.1f, lifetime));
            return owner;
        }

        private void CleanupTargets()
        {
            List<int> remove = null;
            foreach (KeyValuePair<int, TargetSurfaceResponse21> pair in _targetResponses)
            {
                if (pair.Value != null) continue;
                if (remove == null) remove = new List<int>();
                remove.Add(pair.Key);
            }
            if (remove != null)
                for (int i = 0; i < remove.Count; i++) _targetResponses.Remove(remove[i]);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            for (int i = _sceneOwners.Count - 1; i >= 0; i--)
            {
                if (_sceneOwners[i] != null)
                    Destroy(_sceneOwners[i].gameObject);
            }
            _sceneOwners.Clear();
            _targetResponses.Clear();
            GeneratedAssetRuntime21.ClearRuntimeCaches();
        }
    }

    public static class CastAnticipation21
    {
        public static void Build(
            SpellMorphologyOwner21 owner,
            SpellVisualContract21 contract,
            Vector3 position,
            Vector3 direction)
        {
            if (owner == null || contract == null) return;
            Color primary = GeneratedAssetRuntime21.ElementColor(contract.primaryElement, false);
            Color catalyst = GeneratedAssetRuntime21.ElementColor(contract.catalyst, true);
            PresentationGeometry2.Ring(
                position,
                primary,
                Mathf.Clamp(contract.size * 0.5f, 0.28f, 1.3f),
                0.04f,
                0.35f,
                PresentationPriority.Important,
                false);
            PresentationMotionStreaks2.SpawnInward(
                position,
                contract.signature,
                Mathf.Clamp(0.8f + contract.size * 0.4f, 0.8f, 2.2f),
                4 + Mathf.Min(10, contract.runeGraph.Count),
                0.35f,
                PresentationPriority.Normal,
                contract.seeds.body);

            for (int i = 0; i < contract.runeGraph.Count; i++)
            {
                RuneOperatorNode21 rune = contract.runeGraph[i];
                float angle = rune.rotation / 6f * Mathf.PI * 2f;
                Vector3 nodePosition = position + new Vector3(Mathf.Cos(angle), 0.4f + rune.branch * 0.08f, Mathf.Sin(angle)) * (0.45f + i * 0.035f);
                GameObject glyph = CreateGlyph(contract, rune, nodePosition, catalyst, 0.45f);
                owner.Own(glyph);
            }
        }

        private static GameObject CreateGlyph(
            SpellVisualContract21 contract,
            RuneOperatorNode21 rune,
            Vector3 position,
            Color color,
            float lifetime)
        {
            GameObject glyph = new GameObject("AE21 Cast Glyph · " + rune.kind);
            glyph.transform.position = position;
            glyph.transform.rotation = Quaternion.Euler(90f, rune.rotation * 60f, 0f);
            glyph.transform.localScale = Vector3.one * 0.22f;
            MeshFilter filter = glyph.AddComponent<MeshFilter>();
            filter.sharedMesh = GeneratedAssetRuntime21.Mesh("rune-glyph", StableSeed21.Combine(contract.seeds.operators, rune.sourceRuneId));
            MeshRenderer renderer = glyph.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = GeneratedAssetRuntime21.Material(
                BodyPartKind21.RuneNode,
                contract.catalyst,
                color,
                GeneratedAssetRuntime21.ElementColor(contract.primaryElement, false),
                1.6f,
                0.9f,
                contract.seeds.operators + rune.order);
            glyph.AddComponent<PresentationPulseScale2>().Initialize(4f, 0.7f, 1.2f);
            UnityEngine.Object.Destroy(glyph, lifetime);
            return glyph;
        }
    }

    public static class ImpactComposition21
    {
        public static void Build(
            SpellVisualContract21 contract,
            Vector3 position,
            Vector3 normal,
            ContactSurface21 surface,
            bool critical)
        {
            if (contract == null) return;
            float radius = Mathf.Clamp(Mathf.Max(contract.radius, contract.size) * (critical ? 1.15f : 1f), 0.35f, 7f);
            PresentationGeometry2.Burst(
                position,
                contract.signature,
                radius,
                critical ? 0.78f : 0.52f,
                critical ? 1.45f : 1f,
                critical ? PresentationPriority.Critical : PresentationPriority.Important,
                contract.seeds.impact);
            PresentationGeometry2.Ring(
                position + normal * 0.025f,
                GeneratedAssetRuntime21.ElementColor(contract.impactElement, true),
                radius * 0.32f,
                critical ? 0.075f : 0.045f,
                critical ? 0.48f : 0.34f,
                critical ? PresentationPriority.Critical : PresentationPriority.Important,
                true);
            EnvironmentContactVisual21.Build(contract, position, normal, surface, radius);
        }
    }

    public static class EnvironmentContactResolver21
    {
        public static void Resolve(
            Vector3 position,
            Vector3 direction,
            out ContactSurface21 surface,
            out Vector3 normal,
            out Collider target)
        {
            surface = ContactSurface21.Unknown;
            normal = direction.sqrMagnitude > 0.001f ? -direction.normalized : Vector3.up;
            target = null;
            RaycastHit hit;
            Vector3 origin = position - normal * 0.35f;
            if (Physics.SphereCast(origin, 0.15f, normal, out hit, 0.9f, ~0, QueryTriggerInteraction.Collide))
            {
                target = hit.collider;
                normal = hit.normal.sqrMagnitude < 0.01f ? normal : hit.normal;
                surface = Classify(hit.collider, normal);
                return;
            }

            Collider[] nearby = Physics.OverlapSphere(position, 0.45f, ~0, QueryTriggerInteraction.Collide);
            if (nearby.Length > 0)
            {
                target = nearby[0];
                surface = Classify(target, normal);
            }
            else if (Vector3.Dot(normal, Vector3.up) > 0.62f)
                surface = ContactSurface21.Ground;
            else
                surface = ContactSurface21.Wall;
        }

        private static ContactSurface21 Classify(Collider collider, Vector3 normal)
        {
            if (collider == null) return ContactSurface21.Unknown;
            if (collider.GetComponentInParent<EnemyController>() != null) return ContactSurface21.Enemy;
            string name = collider.name.ToLowerInvariant();
            string tag = collider.tag == null ? string.Empty : collider.tag.ToLowerInvariant();
            if (name.Contains("barrier") || name.Contains("shield") || tag.Contains("barrier")) return ContactSurface21.Barrier;
            if (name.Contains("water") || tag.Contains("water")) return ContactSurface21.WaterLike;
            if (collider.GetComponentInParent<ElementalReactionField>() != null) return ContactSurface21.ElementalField;
            if (Vector3.Dot(normal, Vector3.up) > 0.62f) return ContactSurface21.Ground;
            return ContactSurface21.Wall;
        }
    }

    public static class EnvironmentContactVisual21
    {
        public static void Build(
            SpellVisualContract21 contract,
            Vector3 position,
            Vector3 normal,
            ContactSurface21 surface,
            float radius)
        {
            if (surface == ContactSurface21.Enemy) return;
            Vector3 tangent = Vector3.Cross(normal, Vector3.up);
            if (tangent.sqrMagnitude < 0.01f) tangent = Vector3.right;
            tangent.Normalize();
            Vector3 bitangent = Vector3.Cross(normal, tangent).normalized;
            int count = Patch200PresentationSettings.Quality == PresentationQuality.Low ? 4 : 8;
            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f;
                Vector3 direction = tangent * Mathf.Cos(angle) + bitangent * Mathf.Sin(angle);
                PresentationGeometry2.Beam(
                    position + normal * 0.02f,
                    position + normal * 0.02f + direction * radius * Mathf.Lerp(0.25f, 0.65f, StableSeed21.Unit(contract.seeds.residue + i)),
                    GeneratedAssetRuntime21.ElementColor(contract.residueElement, i % 2 == 0),
                    0.018f + (i % 3) * 0.007f,
                    0.35f,
                    PresentationPriority.Normal,
                    contract.motionElement == ReactionElement.Lightning,
                    contract.seeds.residue + i);
            }

            GameObject residue = new GameObject("AE21 Surface Residue · " + surface);
            residue.transform.position = position + normal * 0.012f;
            residue.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
            residue.transform.localScale = new Vector3(radius, 0.035f, radius) * 0.55f;
            MeshFilter filter = residue.AddComponent<MeshFilter>();
            filter.sharedMesh = GeneratedAssetRuntime21.Mesh(
                surface == ContactSurface21.Wall ? "fracture" : "ground",
                contract.seeds.residue);
            MeshRenderer renderer = residue.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = GeneratedAssetRuntime21.Material(
                BodyPartKind21.FieldPanel,
                contract.residueElement,
                GeneratedAssetRuntime21.ElementColor(contract.residueElement, false),
                GeneratedAssetRuntime21.ElementColor(contract.catalyst, true),
                0.8f,
                0.62f,
                contract.seeds.residue);
            UnityEngine.Object.Destroy(residue, Mathf.Clamp(1.5f + contract.duration * 0.3f, 1.5f, 8f));
        }
    }

    public sealed class TargetSurfaceResponse21 : MonoBehaviour
    {
        private sealed class Mark
        {
            public GameObject gameObject;
            public ReactionElement element;
            public float expires;
            public int intensity;
        }

        private readonly List<Mark> _marks = new List<Mark>();
        private Vector3 _baseScale;
        private float _deformUntil;
        private Vector3 _deformDirection;
        private float _deformMagnitude;
        private int _statusIntensity;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        public void ApplyImpact(
            SpellVisualContract21 contract,
            Vector3 worldPosition,
            Vector3 normal,
            bool critical)
        {
            if (contract == null) return;
            ReactionElement element = contract.impactElement == ReactionElement.None
                ? contract.primaryElement
                : contract.impactElement;
            CreateMark(contract, element, worldPosition, normal, critical ? 2 : 1);
            _deformDirection = transform.InverseTransformDirection(-normal).normalized;
            _deformMagnitude = Mathf.Clamp(0.04f + contract.radius * 0.012f + (critical ? 0.05f : 0f), 0.04f, 0.16f);
            _deformUntil = Time.unscaledTime + 0.12f + (critical ? 0.06f : 0f);
        }

        public void ApplyStatus(
            ReactionElement signature,
            int stacks,
            int threshold,
            bool major,
            SpellVisualContract21 contract)
        {
            _statusIntensity = major ? 4 : Mathf.Clamp(Mathf.CeilToInt((stacks / (float)Mathf.Max(1, threshold)) * 3f), 1, 3);
            ReactionElement element = ElementalReactionCodex.PrimaryElement(signature);
            if (element == ReactionElement.None && contract != null) element = contract.primaryElement;
            int needed = Mathf.Clamp(_statusIntensity - Count(element), 0, 4);
            for (int i = 0; i < needed; i++)
            {
                int seed = StableSeed21.Combine(GetEntityId().GetHashCode(), i + _marks.Count * 17);
                Vector3 local = StableSeed21.UnitVector(seed) * 0.48f;
                local.y = Mathf.Abs(local.y) * 0.8f;
                Vector3 position = transform.TransformPoint(local);
                CreateMark(contract, element, position, local.normalized, major ? 3 : 1);
            }
        }

        private void CreateMark(
            SpellVisualContract21 contract,
            ReactionElement element,
            Vector3 worldPosition,
            Vector3 normal,
            int intensity)
        {
            int seed = StableSeed21.Combine(GetEntityId().GetHashCode(), _marks.Count + 1);
            GameObject mark = new GameObject("AE21 Target Mark · " + element);
            mark.transform.SetParent(transform, true);
            mark.transform.position = worldPosition + normal.normalized * 0.015f;
            mark.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal.sqrMagnitude < 0.01f ? Vector3.up : normal.normalized);
            mark.transform.localScale = Vector3.one * Mathf.Lerp(0.12f, 0.32f, intensity / 3f);
            MeshFilter filter = mark.AddComponent<MeshFilter>();
            string meshKey = element == ReactionElement.Cold ? "crystal" :
                             element == ReactionElement.Lightning ? "conductor" :
                             element == ReactionElement.Physical ? "fracture" :
                             element == ReactionElement.Void ? "ring" : "rune-glyph";
            filter.sharedMesh = GeneratedAssetRuntime21.Mesh(meshKey, seed);
            MeshRenderer renderer = mark.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = GeneratedAssetRuntime21.Material(
                element == ReactionElement.Cold ? BodyPartKind21.Crystal : BodyPartKind21.RuneNode,
                element,
                GeneratedAssetRuntime21.ElementColor(element, false),
                GeneratedAssetRuntime21.ElementColor(element, true),
                1f + intensity * 0.45f,
                element == ReactionElement.Blood ? 0.75f : 0.9f,
                seed);
            _marks.Add(new Mark
            {
                gameObject = mark,
                element = element,
                expires = Time.unscaledTime + Mathf.Lerp(1.2f, 5.5f, intensity / 3f),
                intensity = intensity
            });
        }

        private int Count(ReactionElement element)
        {
            int count = 0;
            for (int i = 0; i < _marks.Count; i++)
                if (_marks[i].element == element && _marks[i].gameObject != null) count++;
            return count;
        }

        private void Update()
        {
            if (Time.unscaledTime < _deformUntil)
            {
                float remaining = Mathf.InverseLerp(_deformUntil, _deformUntil - 0.16f, Time.unscaledTime);
                Vector3 compression = Vector3.one;
                compression.x -= Mathf.Abs(_deformDirection.x) * _deformMagnitude * remaining;
                compression.y -= Mathf.Abs(_deformDirection.y) * _deformMagnitude * remaining;
                compression.z -= Mathf.Abs(_deformDirection.z) * _deformMagnitude * remaining;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.Scale(_baseScale, compression), Time.unscaledDeltaTime * 22f);
            }
            else
                transform.localScale = Vector3.Lerp(transform.localScale, _baseScale, Time.unscaledDeltaTime * 14f);

            for (int i = _marks.Count - 1; i >= 0; i--)
            {
                Mark mark = _marks[i];
                if (mark.gameObject == null || Time.unscaledTime >= mark.expires)
                {
                    if (mark.gameObject != null) Destroy(mark.gameObject);
                    _marks.RemoveAt(i);
                }
                else
                {
                    float pulse = 0.92f + Mathf.Sin(Time.unscaledTime * (2f + mark.intensity) + i) * 0.08f;
                    mark.gameObject.transform.localScale *= pulse / Mathf.Max(0.001f, 0.92f + Mathf.Sin((Time.unscaledTime - Time.unscaledDeltaTime) * (2f + mark.intensity) + i) * 0.08f);
                }
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _marks.Count; i++)
                if (_marks[i].gameObject != null) Destroy(_marks[i].gameObject);
            _marks.Clear();
        }
    }

    public sealed class TransientScale21 : MonoBehaviour
    {
        private float _created;
        private float _duration;
        private Vector3 _from;
        private Vector3 _to;

        public void Initialize(float duration, Vector3 from, Vector3 to)
        {
            _created = Time.unscaledTime;
            _duration = Mathf.Max(0.05f, duration);
            _from = from;
            _to = to;
            transform.localScale = from;
        }

        private void Update()
        {
            float t = Mathf.Clamp01((Time.unscaledTime - _created) / _duration);
            float eased = 1f - (1f - t) * (1f - t);
            transform.localScale = Vector3.LerpUnclamped(_from, _to, eased);
            if (t >= 1f) enabled = false;
        }
    }

    public static class Patch210Bootstrap21
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            MorphologyPresentationDirector21 unused = MorphologyPresentationDirector21.Instance;
        }
    }
}
