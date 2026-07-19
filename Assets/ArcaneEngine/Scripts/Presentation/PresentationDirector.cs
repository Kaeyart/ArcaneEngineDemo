using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class SpellPresentationDirector2 : MonoBehaviour
    {
        public static SpellPresentationDirector2 Instance { get; private set; }

        private readonly Dictionary<GameObject, ReactionAssemblyVisual2> _assemblies =
            new Dictionary<GameObject, ReactionAssemblyVisual2>();

        public int ProcessedEvents { get; private set; }
        public int ReducedEvents { get; private set; }
        public int CriticalEvents { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SpellPresentationBus.Published += Handle;
        }

        private void OnDisable()
        {
            SpellPresentationBus.Published -= Handle;
        }

        private void Handle(SpellPresentationEvent presentationEvent)
        {
            ProcessedEvents++;

            if (presentationEvent.priority >= PresentationPriority.Critical)
                CriticalEvents++;

            switch (presentationEvent.type)
            {
                case SpellPresentationEventType.CastStarted:
                    HandleCast(presentationEvent);
                    break;

                case SpellPresentationEventType.ProjectileImpacted:
                    HandleImpact(presentationEvent);
                    break;

                case SpellPresentationEventType.ProjectileRedirected:
                case SpellPresentationEventType.ProjectileReturned:
                    HandleDirectionChange(presentationEvent);
                    break;

                case SpellPresentationEventType.StatusSpread:
                    HandleStatusSpread(presentationEvent);
                    break;

                case SpellPresentationEventType.StatusConsumed:
                    HandleStatusConsumed(presentationEvent);
                    break;

                case SpellPresentationEventType.EffectExpired:
                    HandleExpire(presentationEvent);
                    break;

                case SpellPresentationEventType.ZoneCreated:
                    HandleZone(presentationEvent);
                    break;

                case SpellPresentationEventType.FamiliarCreated:
                    HandleFamiliar(presentationEvent);
                    break;

                case SpellPresentationEventType.MovementArrival:
                    HandleMovement(presentationEvent);
                    break;

                case SpellPresentationEventType.BarrierChanged:
                    HandleBarrier(presentationEvent);
                    break;

                case SpellPresentationEventType.LinkActivated:
                    HandleLink(presentationEvent);
                    break;

                case SpellPresentationEventType.AilmentBuildupChanged:
                    HandleAilment(presentationEvent, false);
                    break;

                case SpellPresentationEventType.MajorAilmentActivated:
                    HandleAilment(presentationEvent, true);
                    break;

                case SpellPresentationEventType.ReactionAssemblyStarted:
                case SpellPresentationEventType.ReactionSignatureUpgraded:
                    HandleAssembly(presentationEvent);
                    break;

                case SpellPresentationEventType.ReactionResolved:
                case SpellPresentationEventType.ReactionDeathTriggered:
                    HandleReactionResolution(presentationEvent);
                    break;

                case SpellPresentationEventType.ReactionMechanic:
                    HandleReactionMechanic(presentationEvent);
                    break;

                case SpellPresentationEventType.FieldCreated:
                case SpellPresentationEventType.FieldMerged:
                case SpellPresentationEventType.FieldPulsed:
                case SpellPresentationEventType.FieldExpired:
                    HandleField(presentationEvent);
                    break;
            }
        }

        private void HandleCast(SpellPresentationEvent value)
        {
            GeneratedVisualRecipe recipe = value.recipe;
            ReactionElement signature = ResolveSignature(value);
            ReactionElement primary = ResolvePrimary(value);
            Color color = ResolveColor(value);
            float radius = Mathf.Max(0.35f, value.radius * 0.35f);

            PresentationGeometry2.Ring(
                value.position,
                color,
                radius,
                0.045f,
                0.28f,
                value.priority,
                true);

            PresentationParticlePool2.Spawn(
                ParticleRequest(
                    value,
                    PresentationParticlePurpose.Cast,
                    primary,
                    value.position + Vector3.up * 0.45f,
                    Mathf.Max(0.18f, radius),
                    0.4f,
                    12,
                    false));

            if (recipe != null)
            {
                ApplyCastChassis(value, recipe);
                ApplyCastOperators(value, recipe);
            }

            PresentationCameraFeedback2.Impulse(
                value.position,
                0.04f + value.intensity * 0.025f,
                0.12f,
                value.priority);

            ProceduralPresentationAudio2.Play(
                value.position,
                signature,
                PresentationAudioCue.Cast,
                0.6f + value.intensity * 0.12f,
                value.priority);
        }

        private void ApplyCastChassis(
            SpellPresentationEvent value,
            GeneratedVisualRecipe recipe)
        {
            switch (recipe.chassis)
            {
                case VisualChassisKind.Beam:
                    PresentationGeometry2.Beam(
                        value.position + Vector3.up * 0.55f,
                        value.position + Vector3.up * 0.55f +
                        SafeDirection(value.direction) * Mathf.Max(2f, recipe.radius),
                        recipe.primaryColor,
                        Mathf.Clamp(recipe.size * 0.08f, 0.035f, 0.16f),
                        0.2f,
                        value.priority,
                        recipe.motionElement == ReactionElement.Lightning,
                        recipe.stableSeed);
                    break;

                case VisualChassisKind.Nova:
                    PresentationPulseSequence2.Spawn(
                        value.position,
                        recipe.signature,
                        0.35f,
                        Mathf.Max(1.5f, recipe.radius),
                        2,
                        0.14f,
                        value.priority,
                        recipe.stableSeed,
                        false);
                    break;

                case VisualChassisKind.Meteor:
                    PresentationGeometry2.Beam(
                        value.position + Vector3.up * 5f,
                        value.position,
                        recipe.primaryColor,
                        0.08f,
                        0.26f,
                        value.priority,
                        false,
                        recipe.stableSeed);
                    PresentationGeometry2.Ring(
                        value.position,
                        recipe.secondaryColor,
                        Mathf.Max(0.8f, recipe.radius),
                        0.055f,
                        0.55f,
                        value.priority,
                        false);
                    break;

                case VisualChassisKind.Melee:
                    PresentationArcSweep2.Spawn(
                        value.position + Vector3.up * 0.3f,
                        SafeDirection(value.direction),
                        Mathf.Max(0.9f, recipe.radius),
                        recipe.primaryColor,
                        0.25f,
                        value.priority,
                        recipe.stableSeed);
                    break;

                case VisualChassisKind.Zone:
                    PresentationGeometry2.Ring(
                        value.position,
                        recipe.primaryColor,
                        Mathf.Max(0.8f, recipe.radius),
                        0.055f,
                        0.42f,
                        value.priority,
                        false);
                    break;

                case VisualChassisKind.Movement:
                    PresentationMotionStreaks2.Spawn(
                        value.position,
                        value.position + SafeDirection(value.direction) * Mathf.Max(1f, recipe.radius),
                        recipe.signature,
                        3,
                        0.26f,
                        value.priority,
                        recipe.stableSeed);
                    break;
            }
        }

        private void ApplyCastOperators(
            SpellPresentationEvent value,
            GeneratedVisualRecipe recipe)
        {
            for (int i = 0; i < recipe.operators.Count; i++)
            {
                RuneVisualOperatorSpec operation = recipe.operators[i];

                switch (operation.kind)
                {
                    case RuneVisualOperatorKind.Split:
                        PresentationRadialMarkers2.Spawn(
                            value.position + Vector3.up * 0.35f,
                            recipe.signature,
                            Mathf.Clamp(operation.count, 2, 10),
                            Mathf.Max(0.3f, recipe.size * 0.3f),
                            0.28f,
                            value.priority,
                            recipe.stableSeed + i);
                        break;

                    case RuneVisualOperatorKind.Orbit:
                        PresentationOrbitalNodesVisual2.Spawn(
                            value.source,
                            value.position + Vector3.up * 0.45f,
                            recipe.signature,
                            Mathf.Clamp(operation.count, 2, 8),
                            Mathf.Max(0.4f, recipe.size * 0.4f),
                            0.45f,
                            value.priority,
                            recipe.stableSeed + i);
                        break;

                    case RuneVisualOperatorKind.Delay:
                        PresentationDelayedVisual2.Spawn(
                            value.position,
                            recipe.signature,
                            Mathf.Max(0.18f, operation.duration),
                            Mathf.Max(0.8f, recipe.radius * 0.55f),
                            value.priority,
                            recipe.stableSeed + i);
                        break;

                    case RuneVisualOperatorKind.Pull:
                        PresentationMotionStreaks2.SpawnInward(
                            value.position,
                            recipe.signature,
                            Mathf.Max(1f, recipe.radius),
                            7,
                            0.35f,
                            value.priority,
                            recipe.stableSeed + i);
                        break;

                    case RuneVisualOperatorKind.Barrier:
                        PresentationGeometry2.Ring(
                            value.position,
                            Color.Lerp(recipe.primaryColor, Color.white, 0.5f),
                            Mathf.Max(0.7f, recipe.size * 0.7f),
                            0.065f,
                            0.34f,
                            value.priority,
                            false);
                        break;
                }
            }
        }

        private void HandleImpact(SpellPresentationEvent value)
        {
            ReactionElement signature = ResolveSignature(value);
            ReactionElement primary = ResolvePrimary(value);
            float radius = Mathf.Max(0.55f, value.radius);
            int seed = value.recipe == null
                ? value.position.GetHashCode()
                : value.recipe.stableSeed;

            PresentationGeometry2.Burst(
                value.position,
                signature,
                radius,
                value.critical ? 0.65f : 0.45f,
                value.intensity,
                value.priority,
                seed);

            PresentationParticlePool2.Spawn(
                ParticleRequest(
                    value,
                    PresentationParticlePurpose.Impact,
                    primary,
                    value.position + Vector3.up * 0.22f,
                    radius,
                    value.critical ? 0.8f : 0.55f,
                    value.critical ? 30 : 20,
                    false));

            if (value.recipe != null)
                ApplyImpactOperators(value, value.recipe);

            PresentationCameraFeedback2.Impulse(
                value.position,
                Mathf.Clamp(0.05f + value.intensity * 0.06f, 0.04f, 0.18f),
                value.critical ? 0.24f : 0.14f,
                value.priority);

            ProceduralPresentationAudio2.Play(
                value.position,
                signature,
                PresentationAudioCue.Impact,
                Mathf.Clamp(0.65f + value.intensity * 0.18f, 0.6f, 1.1f),
                value.priority);
        }

        private void ApplyImpactOperators(
            SpellPresentationEvent value,
            GeneratedVisualRecipe recipe)
        {
            for (int i = 0; i < recipe.operators.Count; i++)
            {
                RuneVisualOperatorSpec operation = recipe.operators[i];

                switch (operation.kind)
                {
                    case RuneVisualOperatorKind.Chain:
                        PresentationChainVisual2.FromPoint(
                            value.position + Vector3.up * 0.5f,
                            recipe.signature,
                            Mathf.Clamp(operation.count, 1, 8),
                            Mathf.Max(2f, recipe.radius + 1.5f),
                            0.22f,
                            value.priority,
                            recipe.stableSeed + i);
                        break;

                    case RuneVisualOperatorKind.Split:
                        PresentationShardVisual2.SpawnNova(
                            value.position + Vector3.up * 0.35f,
                            recipe.signature,
                            Mathf.Clamp(operation.count * 2, 4, 20),
                            Mathf.Max(0.8f, recipe.radius),
                            0.55f,
                            value.priority,
                            recipe.stableSeed + i);
                        break;

                    case RuneVisualOperatorKind.Persistent:
                        PresentationResidue2.SpawnStandalone(
                            value.position,
                            recipe.signature,
                            Mathf.Max(0.8f, recipe.radius),
                            Mathf.Max(1.5f, recipe.duration),
                            recipe.stableSeed + i,
                            value.priority);
                        break;

                    case RuneVisualOperatorKind.Bounce:
                    case RuneVisualOperatorKind.Return:
                        PresentationGeometry2.Ring(
                            value.position,
                            recipe.secondaryColor,
                            Mathf.Max(0.4f, recipe.size * 0.45f),
                            0.04f,
                            0.2f,
                            PresentationPriority.Normal,
                            true);
                        break;

                    case RuneVisualOperatorKind.ConsumeStatus:
                        PresentationMotionStreaks2.SpawnInward(
                            value.position,
                            recipe.signature,
                            Mathf.Max(0.8f, recipe.radius),
                            9,
                            0.42f,
                            value.priority,
                            recipe.stableSeed + i);
                        break;
                }
            }
        }

        private void HandleDirectionChange(SpellPresentationEvent value)
        {
            PresentationGeometry2.Burst(
                value.position,
                ResolveSignature(value),
                Mathf.Max(0.3f, value.radius),
                0.25f,
                value.intensity,
                value.priority,
                value.position.GetHashCode());
        }

        private void HandleStatusSpread(SpellPresentationEvent value)
        {
            ReactionElement signature = ResolveSignature(value);
            bool jagged = ElementalReactionCodex.Contains(signature, ReactionElement.Lightning);

            PresentationGeometry2.Beam(
                value.position,
                value.secondaryPosition,
                ResolveColor(value),
                0.055f,
                Mathf.Max(0.12f, value.duration),
                value.priority,
                jagged,
                value.position.GetHashCode() ^ value.secondaryPosition.GetHashCode());

            PresentationTransferVisual2.Spawn(
                value.position,
                value.secondaryPosition,
                signature,
                value.duration,
                value.priority,
                value.position.GetHashCode());
        }

        private void HandleStatusConsumed(SpellPresentationEvent value)
        {
            PresentationMotionStreaks2.SpawnInward(
                value.position,
                ResolveSignature(value),
                Mathf.Max(0.6f, value.radius),
                10,
                Mathf.Max(0.25f, value.duration),
                value.priority,
                value.position.GetHashCode());

            PresentationDelayedVisual2.Spawn(
                value.position,
                ResolveSignature(value),
                0.14f,
                Mathf.Max(0.5f, value.radius * 0.75f),
                value.priority,
                value.position.GetHashCode());
        }

        private void HandleExpire(SpellPresentationEvent value)
        {
            PresentationGeometry2.Ring(
                value.position,
                ResolveColor(value),
                Mathf.Max(0.3f, value.radius * 0.35f),
                0.03f,
                Mathf.Max(0.16f, value.duration),
                PresentationPriority.Normal,
                false);
        }

        private void HandleZone(SpellPresentationEvent value)
        {
            if (value.host == null)
                return;

            PresentationResidue2.AttachSpellZone(
                value.host,
                ResolveSignature(value),
                value.radius,
                value.duration,
                value.recipe == null ? value.host.GetEntityId().GetHashCode() : value.recipe.stableSeed,
                value.priority);
        }

        private void HandleFamiliar(SpellPresentationEvent value)
        {
            PresentationGeometry2.Ring(
                value.position,
                ResolveColor(value),
                Mathf.Max(0.45f, value.radius * 0.35f),
                0.04f,
                0.35f,
                value.priority,
                true);
        }

        private void HandleMovement(SpellPresentationEvent value)
        {
            PresentationMotionStreaks2.Spawn(
                value.position,
                value.secondaryPosition,
                ResolveSignature(value),
                5,
                Mathf.Max(0.22f, value.duration),
                value.priority,
                value.position.GetHashCode());

            PresentationGeometry2.Burst(
                value.position,
                ResolveSignature(value),
                0.7f,
                0.35f,
                0.8f,
                value.priority,
                value.position.GetHashCode());

            PresentationGeometry2.Burst(
                value.secondaryPosition,
                ResolveSignature(value),
                1f,
                0.42f,
                1f,
                value.priority,
                value.secondaryPosition.GetHashCode());
        }

        private void HandleBarrier(SpellPresentationEvent value)
        {
            PresentationGeometry2.Ring(
                value.position,
                value.critical ? Color.white : new Color(0.35f, 0.85f, 1f),
                value.radius,
                value.critical ? 0.09f : 0.06f,
                value.duration,
                value.priority,
                value.critical);

            PresentationGeometry2.Burst(
                value.position + Vector3.up * 0.7f,
                ReactionElement.Cold | ReactionElement.Lightning,
                value.radius,
                value.duration,
                value.intensity,
                value.priority,
                value.position.GetHashCode());
        }

        private void HandleLink(SpellPresentationEvent value)
        {
            Transform source = value.source;
            Vector3 from = source == null
                ? value.position + Vector3.up * 0.5f
                : source.position + Vector3.up * 0.8f;

            PresentationGeometry2.Beam(
                from,
                value.position + Vector3.up * 0.3f,
                ResolveColor(value),
                0.055f,
                value.duration,
                value.priority,
                true,
                value.mechanicId == null ? 0 : value.mechanicId.GetHashCode());
        }

        private void HandleAilment(SpellPresentationEvent value, bool major)
        {
            EnemyController enemy = value.host == null
                ? null
                : value.host.GetComponent<EnemyController>();

            if (enemy == null)
                return;

            ElementalStatusVisual2 visual = ElementalStatusVisual2.Attach(enemy);

            if (visual != null)
            {
                visual.Set(
                    ResolvePrimary(value),
                    major ? 1f : value.normalizedProgress,
                    value.duration,
                    major);
            }

            if (major)
            {
                PresentationGeometry2.Burst(
                    value.position,
                    value.signature,
                    Mathf.Max(0.8f, value.radius),
                    0.5f,
                    value.intensity,
                    value.priority,
                    enemy.GetEntityId().GetHashCode());

                ProceduralPresentationAudio2.Play(
                    value.position,
                    value.signature,
                    PresentationAudioCue.MajorAilment,
                    0.85f,
                    value.priority);
            }
        }

        private void HandleAssembly(SpellPresentationEvent value)
        {
            if (value.host == null)
                return;

            ReactionAssemblyVisual2 visual;

            if (!_assemblies.TryGetValue(value.host, out visual) || visual == null)
            {
                visual = value.host.GetComponent<ReactionAssemblyVisual2>();

                if (visual == null)
                    visual = value.host.AddComponent<ReactionAssemblyVisual2>();

                _assemblies[value.host] = visual;
            }

            visual.Begin(
                value.signature,
                value.catalyst,
                value.duration,
                value.tier,
                value.type == SpellPresentationEventType.ReactionSignatureUpgraded);

            ProceduralPresentationAudio2.Play(
                value.position,
                value.signature,
                PresentationAudioCue.Assembly,
                0.65f,
                value.priority);
        }

        private void HandleReactionResolution(SpellPresentationEvent value)
        {
            if (value.host != null)
            {
                ReactionAssemblyVisual2 assembly =
                    value.host.GetComponent<ReactionAssemblyVisual2>();

                if (assembly != null)
                    assembly.Resolve();

                _assemblies.Remove(value.host);
            }

            int elements = Mathf.Max(1, ElementalReactionCodex.CountBits(value.signature));
            float radius = Mathf.Max(1f, value.radius);
            int seed = value.mechanicId == null
                ? value.position.GetHashCode()
                : value.mechanicId.GetHashCode();

            PresentationGeometry2.Burst(
                value.position,
                value.signature,
                radius,
                Mathf.Clamp(0.45f + elements * 0.08f, 0.5f, 1.15f),
                value.intensity,
                value.priority,
                seed);

            PresentationPulseSequence2.Spawn(
                value.position,
                value.signature,
                radius * 0.25f,
                radius,
                Mathf.Clamp(elements, 2, 7),
                Mathf.Clamp(0.11f + elements * 0.018f, 0.12f, 0.24f),
                value.priority,
                seed,
                ElementalReactionCodex.Contains(value.signature, ReactionElement.Void));

            PresentationParticlePool2.Spawn(
                ParticleRequest(
                    value,
                    PresentationParticlePurpose.Impact,
                    value.catalyst == ReactionElement.None
                        ? ElementalReactionCodex.PrimaryElement(value.signature)
                        : value.catalyst,
                    value.position + Vector3.up * 0.45f,
                    radius,
                    Mathf.Max(0.8f, value.duration * 0.18f),
                    24 + elements * 8,
                    false));

            if (elements >= 4 || value.death)
            {
                PresentationResidue2.SpawnStandalone(
                    value.position,
                    value.signature,
                    radius * 0.7f,
                    Mathf.Clamp(value.duration, 1.5f, 7f),
                    seed,
                    value.priority);
            }

            PresentationCameraFeedback2.Impulse(
                value.position,
                Mathf.Clamp(0.08f + elements * 0.025f, 0.08f, 0.28f),
                Mathf.Clamp(0.14f + elements * 0.035f, 0.16f, 0.42f),
                value.priority);

            ProceduralPresentationAudio2.Play(
                value.position,
                value.signature,
                value.death
                    ? PresentationAudioCue.DeathReaction
                    : PresentationAudioCue.Reaction,
                Mathf.Clamp(0.7f + elements * 0.06f, 0.75f, 1.12f),
                value.priority);
        }

        private void HandleReactionMechanic(SpellPresentationEvent value)
        {
            ReactionElement signature = ResolveSignature(value);
            int seed = value.mechanicId == null
                ? value.position.GetHashCode()
                : value.mechanicId.GetHashCode();
            float radius = Mathf.Max(0.65f, value.radius);
            int count = Mathf.Max(1, value.count);

            switch (value.mechanicType)
            {
                case ReactionMechanicType.Burst:
                    PresentationGeometry2.Burst(
                        value.position,
                        signature,
                        radius,
                        0.48f,
                        value.intensity,
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.PulseNova:
                    PresentationPulseSequence2.Spawn(
                        value.position,
                        signature,
                        radius * 0.28f,
                        radius,
                        count,
                        Mathf.Max(0.1f, value.duration / count),
                        value.priority,
                        seed,
                        false);
                    break;

                case ReactionMechanicType.ChainArc:
                    PresentationChainVisual2.FromPoint(
                        value.position,
                        signature,
                        count,
                        radius,
                        Mathf.Max(0.12f, value.duration * 0.12f),
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.Pull:
                    PresentationMotionStreaks2.SpawnInward(
                        value.position,
                        signature,
                        radius,
                        Mathf.Clamp(count + 5, 6, 18),
                        Mathf.Max(0.25f, value.duration),
                        value.priority,
                        seed);
                    PresentationGeometry2.Ring(
                        value.position,
                        ResolveColor(value),
                        radius,
                        0.055f,
                        0.42f,
                        value.priority,
                        false);
                    break;

                case ReactionMechanicType.Push:
                    PresentationMotionStreaks2.SpawnOutward(
                        value.position,
                        signature,
                        radius,
                        Mathf.Clamp(count + 6, 7, 20),
                        Mathf.Max(0.25f, value.duration),
                        value.priority,
                        seed);
                    PresentationGeometry2.Ring(
                        value.position,
                        ResolveColor(value),
                        radius * 0.25f,
                        0.065f,
                        0.45f,
                        value.priority,
                        true);
                    break;

                case ReactionMechanicType.Freeze:
                    PresentationShardVisual2.SpawnNova(
                        value.position + Vector3.up * 0.45f,
                        ReactionElement.Cold,
                        Mathf.Clamp(count * 2 + 6, 8, 24),
                        radius,
                        Mathf.Max(0.4f, value.duration),
                        value.priority,
                        seed);
                    PresentationGeometry2.Ring(
                        value.position,
                        ElementalReactionCodex.ColorFor(ReactionElement.Cold),
                        radius,
                        0.055f,
                        0.42f,
                        value.priority,
                        false);
                    break;

                case ReactionMechanicType.Vulnerability:
                    PresentationFractureVisual2.Spawn(
                        value.position,
                        signature,
                        radius,
                        Mathf.Clamp(count + 5, 6, 18),
                        Mathf.Max(0.35f, value.duration),
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.BuildupWave:
                    PresentationPulseSequence2.Spawn(
                        value.position,
                        signature,
                        radius * 0.35f,
                        radius,
                        Mathf.Clamp(count, 1, 6),
                        0.18f,
                        value.priority,
                        seed,
                        false);
                    PresentationParticlePool2.Spawn(
                        ParticleRequest(
                            value,
                            PresentationParticlePurpose.Status,
                            ResolvePrimary(value),
                            value.position,
                            radius,
                            Mathf.Max(0.4f, value.duration),
                            18,
                            false));
                    break;

                case ReactionMechanicType.Field:
                    PresentationResidue2.SpawnStandalone(
                        value.position,
                        signature,
                        radius,
                        Mathf.Max(1f, value.duration),
                        seed,
                        value.priority);
                    break;

                case ReactionMechanicType.SplitFields:
                    PresentationSplitResidues2.Spawn(
                        value.position,
                        signature,
                        radius,
                        Mathf.Max(1f, value.duration),
                        Mathf.Clamp(count, 2, 12),
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.DelayedEcho:
                    for (int i = 0; i < Mathf.Clamp(count, 1, 8); i++)
                    {
                        PresentationDelayedVisual2.Spawn(
                            value.position,
                            signature,
                            Mathf.Max(0.05f, value.delay + i * 0.22f),
                            radius * Mathf.Lerp(0.45f, 1f, (i + 1f) / count),
                            value.priority,
                            seed + i);
                    }
                    break;

                case ReactionMechanicType.OrbitingNodes:
                    PresentationOrbitalNodesVisual2.Spawn(
                        value.target,
                        value.position,
                        signature,
                        Mathf.Clamp(count, 2, 12),
                        radius,
                        Mathf.Max(0.5f, value.duration),
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.ShardNova:
                    PresentationShardVisual2.SpawnNova(
                        value.position + Vector3.up * 0.5f,
                        signature,
                        Mathf.Clamp(count, 5, 32),
                        radius,
                        Mathf.Max(0.45f, value.duration),
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.Execute:
                    PresentationExecuteVisual2.Spawn(
                        value.position,
                        signature,
                        radius,
                        Mathf.Max(0.35f, value.duration),
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.ThermalCycle:
                    PresentationThermalCycleVisual2.Spawn(
                        value.position,
                        radius,
                        Mathf.Clamp(count, 2, 8),
                        Mathf.Max(0.1f, value.duration / count),
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.Rebound:
                    PresentationPulseSequence2.Spawn(
                        value.position,
                        signature,
                        radius,
                        radius * 0.25f,
                        Mathf.Clamp(count, 1, 5),
                        0.2f,
                        value.priority,
                        seed,
                        true);
                    PresentationMotionStreaks2.SpawnOutward(
                        value.position,
                        signature,
                        radius,
                        10,
                        0.38f,
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.FieldSurge:
                    PresentationResidue2.SurgeNear(
                        value.position,
                        radius,
                        signature,
                        Mathf.Max(0.8f, value.duration),
                        value.intensity);
                    break;

                case ReactionMechanicType.TrailLine:
                    PresentationTrailResidue2.Spawn(
                        value.position,
                        signature,
                        radius,
                        Mathf.Max(0.8f, value.duration),
                        Mathf.Clamp(count, 2, 8),
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.Stagger:
                    PresentationFractureVisual2.Spawn(
                        value.position,
                        ReactionElement.Physical | signature,
                        radius,
                        Mathf.Clamp(count + 5, 7, 20),
                        Mathf.Max(0.35f, value.duration),
                        value.priority,
                        seed);
                    PresentationMotionStreaks2.SpawnOutward(
                        value.position,
                        signature,
                        radius,
                        8,
                        0.3f,
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.DetonateBuildup:
                    PresentationStackDetonationVisual2.Spawn(
                        value.position,
                        signature,
                        radius,
                        Mathf.Max(0.4f, value.duration),
                        value.intensity,
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.Contagion:
                    PresentationChainVisual2.FromPoint(
                        value.position,
                        signature,
                        Mathf.Clamp(count, 2, 12),
                        radius,
                        0.25f,
                        value.priority,
                        seed);
                    PresentationTransferVisual2.SpawnRadial(
                        value.position,
                        signature,
                        radius,
                        Mathf.Clamp(count, 2, 12),
                        0.45f,
                        value.priority,
                        seed);
                    break;

                case ReactionMechanicType.Compression:
                    PresentationMotionStreaks2.SpawnInward(
                        value.position,
                        signature,
                        radius,
                        16,
                        Mathf.Max(0.35f, value.duration),
                        value.priority,
                        seed);
                    PresentationDelayedVisual2.Spawn(
                        value.position,
                        signature,
                        Mathf.Max(0.1f, value.delay),
                        radius,
                        value.priority,
                        seed);
                    break;
            }
        }

        private void HandleField(SpellPresentationEvent value)
        {
            if (value.type == SpellPresentationEventType.FieldExpired)
            {
                if (value.host != null)
                {
                    PresentationResidue2 residue =
                        value.host.GetComponent<PresentationResidue2>();
                    if (residue != null)
                        Destroy(residue);
                }
                return;
            }

            if (value.host == null)
                return;

            PresentationResidue2 visual =
                value.host.GetComponent<PresentationResidue2>();

            if (visual == null)
            {
                visual = PresentationResidue2.AttachReactionField(
                    value.host,
                    value.signature,
                    value.radius,
                    value.duration,
                    value.host.GetEntityId().GetHashCode() ^ (int)value.signature,
                    value.priority);
            }
            else if (value.type == SpellPresentationEventType.FieldMerged)
            {
                visual.Merge(
                    value.signature,
                    value.radius,
                    value.duration,
                    value.intensity);
            }
            else if (value.type == SpellPresentationEventType.FieldPulsed)
            {
                visual.Pulse(value.intensity);
            }
        }

        private static PresentationParticleRequest ParticleRequest(
            SpellPresentationEvent value,
            PresentationParticlePurpose purpose,
            ReactionElement primary,
            Vector3 position,
            float radius,
            float duration,
            int count,
            bool looping)
        {
            GeneratedVisualRecipe recipe = value.recipe;
            ElementVisualProfile2 profile =
                ElementVisualProfileRegistry2.Get(primary);
            ReactionElement signature = ResolveSignature(value);

            return new PresentationParticleRequest
            {
                purpose = purpose,
                position = position,
                direction = SafeDirection(value.direction),
                follow = null,
                signature = signature,
                primary = primary,
                primaryColor = recipe == null
                    ? ElementalReactionCodex.BlendColor(signature)
                    : recipe.primaryColor,
                secondaryColor = profile == null
                    ? Color.white
                    : profile.secondary,
                radius = radius,
                duration = duration,
                intensity = value.intensity,
                speed = profile == null ? 1.5f : profile.particleSpeed,
                seed = value.mechanicId == null
                    ? value.position.GetHashCode()
                    : value.mechanicId.GetHashCode(),
                count = count,
                looping = looping,
                worldSpace = true,
                priority = value.priority
            };
        }

        private static ReactionElement ResolveSignature(
            SpellPresentationEvent value)
        {
            if (value.signature != ReactionElement.None)
                return value.signature;

            if (value.recipe != null && value.recipe.signature != ReactionElement.None)
                return value.recipe.signature;

            return ReactionElement.Physical;
        }

        private static ReactionElement ResolvePrimary(
            SpellPresentationEvent value)
        {
            if (value.catalyst != ReactionElement.None)
                return value.catalyst;

            if (value.recipe != null && value.recipe.primaryElement != ReactionElement.None)
                return value.recipe.primaryElement;

            return ElementalReactionCodex.PrimaryElement(ResolveSignature(value));
        }

        private static Color ResolveColor(SpellPresentationEvent value)
        {
            if (value.recipe != null)
                return value.recipe.primaryColor;

            return ElementalReactionCodex.BlendColor(ResolveSignature(value));
        }

        private static Vector3 SafeDirection(Vector3 direction)
        {
            return direction.sqrMagnitude < 0.001f
                ? Vector3.forward
                : direction.normalized;
        }
    }

    public sealed class ReactionAssemblyVisual2 : MonoBehaviour
    {
        private ReactionElement _signature;
        private ReactionElement _catalyst;
        private float _startedAt;
        private float _endsAt;
        private ReactionTier _tier;
        private LineRenderer _ring;
        private PooledPresentationParticle2 _particles;
        private float _nextCue;

        public void Begin(
            ReactionElement signature,
            ReactionElement catalyst,
            float duration,
            ReactionTier tier,
            bool upgraded)
        {
            _signature = signature;
            _catalyst = catalyst == ReactionElement.None
                ? ElementalReactionCodex.PrimaryElement(signature)
                : catalyst;
            _startedAt = Time.time;
            _endsAt = Time.time + Mathf.Max(0.15f, duration);
            _tier = tier;
            _nextCue = Time.time;

            if (_particles != null)
                _particles.StopAndRelease();

            ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(_catalyst);
            _particles = PresentationParticlePool2.Spawn(
                new PresentationParticleRequest
                {
                    purpose = PresentationParticlePurpose.Status,
                    position = transform.position + Vector3.up * 0.7f,
                    follow = transform,
                    direction = Vector3.up,
                    signature = signature,
                    primary = _catalyst,
                    primaryColor = ElementalReactionCodex.BlendColor(signature),
                    secondaryColor = profile == null ? Color.white : profile.secondary,
                    radius = 0.7f + ElementalReactionCodex.CountBits(signature) * 0.08f,
                    duration = duration + 0.4f,
                    intensity = upgraded ? 1.35f : 1f,
                    seed = GetEntityId().GetHashCode() ^ (int)signature,
                    count = 12 + ElementalReactionCodex.CountBits(signature) * 4,
                    looping = true,
                    worldSpace = true,
                    priority = tier >= ReactionTier.Catastrophe
                        ? PresentationPriority.Critical
                        : PresentationPriority.Important
                });

            if (upgraded)
            {
                PresentationGeometry2.Burst(
                    transform.position + Vector3.up * 0.55f,
                    signature,
                    1.2f,
                    0.38f,
                    1.1f,
                    PresentationPriority.Critical,
                    GetEntityId().GetHashCode());
            }
        }

        public void Resolve()
        {
            if (_particles != null)
                _particles.StopAndRelease();

            _particles = null;
            Destroy(this);
        }

        private void Update()
        {
            float duration = Mathf.Max(0.01f, _endsAt - _startedAt);
            float progress = Mathf.Clamp01((Time.time - _startedAt) / duration);

            if (Time.time >= _nextCue)
            {
                _nextCue = Time.time + Mathf.Lerp(0.38f, 0.08f, progress);
                PresentationGeometry2.Ring(
                    transform.position,
                    ElementalReactionCodex.BlendColor(_signature),
                    Mathf.Lerp(1.1f, 0.35f, progress),
                    Mathf.Lerp(0.035f, 0.08f, progress),
                    0.16f,
                    _tier >= ReactionTier.Catastrophe
                        ? PresentationPriority.Critical
                        : PresentationPriority.Important,
                    false);
            }

            if (Time.time >= _endsAt + 0.25f)
                Resolve();
        }

        private void OnDestroy()
        {
            if (_particles != null)
                _particles.StopAndRelease();
        }
    }
}
