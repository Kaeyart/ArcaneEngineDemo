using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class UniqueMutationVisualProfile
    {
        public UniqueMutation mutation;
        public string functionalRule;
        public PrimitiveType authorityShape;
        public PrimitiveType accentShape;
        public int marks;
        public bool inward;
        public bool tether;
    }

    /// <summary>
    /// Presentation registry keyed by the functional mutation enum. Item names are
    /// deliberately not used, so renamed or procedurally rolled items retain the
    /// correct visual rule.
    /// </summary>
    public static class UniqueMutationVisualRegistry
    {
        private static readonly Dictionary<UniqueMutation, UniqueMutationVisualProfile> Profiles = new Dictionary<UniqueMutation, UniqueMutationVisualProfile>();

        public static UniqueMutationVisualProfile Get(UniqueMutation mutation)
        {
            if (mutation == UniqueMutation.None) return null;
            UniqueMutationVisualProfile profile;
            if (Profiles.TryGetValue(mutation, out profile)) return profile;
            profile = Build(mutation);
            Profiles[mutation] = profile;
            return profile;
        }

        private static UniqueMutationVisualProfile Build(UniqueMutation mutation)
        {
            UniqueMutationVisualProfile value = new UniqueMutationVisualProfile
            {
                mutation = mutation,
                functionalRule = mutation.ToString(),
                authorityShape = PrimitiveType.Cube,
                accentShape = PrimitiveType.Sphere,
                marks = 2
            };
            switch (mutation)
            {
                case UniqueMutation.MiniatureSunPrimary: value.functionalRule = "Primary spell miniature sun"; value.authorityShape = PrimitiveType.Sphere; value.accentShape = PrimitiveType.Capsule; value.marks = 4; value.inward = true; break;
                case UniqueMutation.VoidMawSecondary: value.functionalRule = "Secondary spell gravity maw"; value.authorityShape = PrimitiveType.Cylinder; value.marks = 4; value.inward = true; break;
                case UniqueMutation.DodgeCastsSecondary: value.functionalRule = "Dodge casts Secondary"; value.authorityShape = PrimitiveType.Capsule; value.marks = 3; value.tether = true; break;
                case UniqueMutation.BloodCasting: value.functionalRule = "Health-powered casting"; value.authorityShape = PrimitiveType.Sphere; value.accentShape = PrimitiveType.Capsule; value.marks = 3; value.inward = true; break;
                case UniqueMutation.EchoPrimaryIntoSecondary: value.functionalRule = "Primary echoes into Secondary"; value.authorityShape = PrimitiveType.Cylinder; value.marks = 2; value.tether = true; break;
                case UniqueMutation.WildTriggerReservoir: value.functionalRule = "Wild trigger reservoir"; value.authorityShape = PrimitiveType.Sphere; value.accentShape = PrimitiveType.Cube; value.marks = 5; break;
                case UniqueMutation.ImpossibleConnectors: value.functionalRule = "Connector restrictions ignored"; value.authorityShape = PrimitiveType.Cube; value.marks = 6; break;
                case UniqueMutation.ThirdSpellEcho: value.functionalRule = "Third spell echo"; value.authorityShape = PrimitiveType.Cylinder; value.marks = 3; value.tether = true; break;
                case UniqueMutation.CorruptedPower: value.functionalRule = "Corruption converted to power"; value.authorityShape = PrimitiveType.Capsule; value.accentShape = PrimitiveType.Cube; value.marks = 4; value.inward = true; break;
                case UniqueMutation.AllLightningNoCritical: value.functionalRule = "All spells become Lightning; criticals disabled"; value.authorityShape = PrimitiveType.Capsule; value.marks = 3; break;
                case UniqueMutation.CentralVirtualConnector: value.functionalRule = "Virtual central connector"; value.authorityShape = PrimitiveType.Cylinder; value.marks = 6; value.inward = true; break;
                case UniqueMutation.HealingReservoir: value.functionalRule = "Damage fills healing reservoir"; value.authorityShape = PrimitiveType.Sphere; value.marks = 3; value.inward = true; break;
                case UniqueMutation.OrbitingArsenal: value.functionalRule = "Projectiles orbit before release"; value.authorityShape = PrimitiveType.Capsule; value.marks = 5; break;
                case UniqueMutation.ManaWardExchange: value.functionalRule = "Mana and Ward exchange"; value.authorityShape = PrimitiveType.Cylinder; value.accentShape = PrimitiveType.Sphere; value.marks = 2; value.tether = true; break;
                case UniqueMutation.TriggeredDominion: value.functionalRule = "Triggered spell dominion"; value.authorityShape = PrimitiveType.Cube; value.marks = 3; value.tether = true; break;
                case UniqueMutation.AilmentConvergence: value.functionalRule = "Ailments converge"; value.authorityShape = PrimitiveType.Sphere; value.accentShape = PrimitiveType.Capsule; value.marks = 4; value.inward = true; break;
                case UniqueMutation.TwoHandedCircuit: value.functionalRule = "Two-handed spell circuit"; value.authorityShape = PrimitiveType.Capsule; value.marks = 2; value.tether = true; break;
                case UniqueMutation.TwinGloveLink: value.functionalRule = "Twin glove spell link"; value.authorityShape = PrimitiveType.Sphere; value.marks = 2; value.tether = true; break;
            }
            return value;
        }
    }

    /// <summary>
    /// Constructs the visible spell language from the compiled descriptor. Every delivery
    /// has its own geometry and event rhythm; none of these objects owns combat truth.
    /// </summary>
    public static class SpellDeliveryVisuals
    {
        public static void BeginCast(CompiledSpell spell, CastRequest request)
        {
            SpellVisualDescriptor descriptor = SpellVisualCompiler.Compile(spell, request);
            if (descriptor == null) return;
            Anticipation(descriptor, request.origin, request.direction);
            PatternCue(descriptor, request.origin, request.direction);
            DeliveryManifestation(descriptor, request);
            SupportAccents(descriptor, request.origin, request.direction);
            UniqueEquipmentAccents(descriptor, request.origin, request.direction);
            LinkContextAccent(descriptor, request.origin);
            LegendaryAccent(descriptor, request.origin, request.targetPosition);
            OverloadAccent(descriptor, request.origin);
        }

        public static void Impact(CompiledSpell spell, CastRequest request, Vector3 position, bool critical)
        {
            SpellVisualDescriptor descriptor = SpellVisualCompiler.Compile(spell, request);
            if (descriptor == null) return;
            float radius = Mathf.Clamp(descriptor.coreScale * (critical ? 1.2f : 0.72f), 0.24f, 2.8f);
            switch (descriptor.impactFamily)
            {
                case SpellImpactFamily.Bloom:
                    for (int i = 0; i < 5; i++) RadialPart("Fire Bloom", PrimitiveType.Capsule, position, descriptor.primary, radius, i, 5, 0.13f);
                    break;
                case SpellImpactFamily.Shatter:
                    for (int i = 0; i < 6; i++) RadialPart("Frost Shard", PrimitiveType.Cube, position, descriptor.primary, radius, i, 6, 0.16f);
                    break;
                case SpellImpactFamily.Fork:
                    for (int i = 0; i < 3; i++)
                    {
                        Vector3 end = position + Quaternion.Euler(0f, i * 120f + 22f, 0f) * Vector3.forward * radius * 1.45f;
                        ProceduralVisualRuntime.Beam("Lightning Impact Fork", position, end, descriptor.primary, 0.06f, 0.12f, true);
                    }
                    break;
                case SpellImpactFamily.Splash:
                    for (int i = 0; i < 4; i++) RadialPart("Toxic Splash Lobe", PrimitiveType.Sphere, position, descriptor.primary, radius, i, 4, 0.2f);
                    break;
                case SpellImpactFamily.Collapse:
                    for (int i = 0; i < 3; i++) ProceduralVisualRuntime.Ring("Void Collapse " + i, position, descriptor.primary, radius * (1.5f - i * 0.32f), 0.06f, 0.16f, null, true);
                    break;
                default:
                    Glyph(position, descriptor.primary, radius, 0.16f, "Arcane Impact Glyph");
                    break;
            }
            if (critical) CriticalAccent(descriptor, position);
        }

        public static void Resolve(CompiledSpell spell, CastRequest request, Vector3 position)
        {
            SpellVisualDescriptor descriptor = SpellVisualCompiler.Compile(spell, request);
            if (descriptor == null) return;
            ProceduralVisualRuntime.Ring("Resolution · " + descriptor.delivery, position, descriptor.secondary,
                Mathf.Max(0.4f, descriptor.coreScale), 0.055f, 0.22f, null, descriptor.persistent);
            if (spell.returnsToCaster) ProceduralVisualRuntime.Burst("Return Resolution", position, descriptor.secondary, 0.32f, 0.12f, PrimitiveType.Cube);
            if (descriptor.legendaryProfile != null && descriptor.legendaryProfile.usesGroundSeal)
                Glyph(position, descriptor.secondary, Mathf.Max(0.8f, descriptor.radius * 0.35f), 0.28f, descriptor.legendaryProfile.construction + " Resolution");
        }

        public static void AttachZone(GameObject zone, CompiledSpell spell, CastRequest request, float radius, float lifetime)
        {
            if (zone == null) return;
            SpellVisualDescriptor descriptor = SpellVisualCompiler.Compile(spell, request);
            if (descriptor == null) return;
            ZoneSustainVisual sustain = zone.AddComponent<ZoneSustainVisual>();
            sustain.Initialize(descriptor, radius, lifetime);
        }

        public static void AttachFamiliar(GameObject familiar, CompiledSpell spell, CastRequest request, int index)
        {
            if (familiar == null) return;
            SpellVisualDescriptor descriptor = SpellVisualCompiler.Compile(spell, request);
            if (descriptor == null) return;
            FamiliarSustainVisual sustain = familiar.AddComponent<FamiliarSustainVisual>();
            sustain.Initialize(descriptor, index);
        }

        public static void MovementArrived(CompiledSpell spell, CastRequest request, Vector3 from, Vector3 to)
        {
            SpellVisualDescriptor descriptor = SpellVisualCompiler.Compile(spell, request);
            if (descriptor == null) return;
            Glyph(from, descriptor.secondary, 0.78f, 0.25f, "Movement Departure");
            Glyph(to, descriptor.primary, 1.05f, 0.32f, "Movement Arrival");
            for (int i = 0; i < 3; i++)
                ProceduralVisualRuntime.Beam("Movement Echo " + i, from + Vector3.up * (0.15f + i * 0.16f), to + Vector3.up * (0.15f + i * 0.16f),
                    Color.Lerp(descriptor.secondary, descriptor.primary, i / 2f), 0.055f + i * 0.018f, 0.18f + i * 0.04f, descriptor.motionFamily == SpellMotionFamily.Imploding);
        }

        public static void BarrierEvent(Transform player, Color color, bool broke, float strength)
        {
            if (player == null) return;
            int layers = broke ? 4 : Mathf.Clamp(Mathf.CeilToInt(strength / 25f), 1, 3);
            for (int i = 0; i < layers; i++)
            {
                LineRenderer ring = ProceduralVisualRuntime.Ring(broke ? "Barrier Break Facet" : "Barrier Absorb Layer", player.position, color,
                    0.9f + i * 0.16f, broke ? 0.075f : 0.11f, broke ? 0.22f : 0.12f, player);
                if (ring != null) ring.transform.localRotation = Quaternion.Euler(i * 19f, i * 27f, broke ? 35f : 0f);
            }
        }

        public static void TriggerLink(CompiledSpell source, CompiledSpell destination, Vector3 position, TriggerMoment moment)
        {
            if (source == null || destination == null || GameWorld.Instance == null || GameWorld.Instance.Player == null) return;
            Transform player = GameWorld.Instance.Player.transform;
            Color color = Color.Lerp(source.accentColor, destination.primaryColor, 0.5f);
            int slotIndex = (int)source.slot;
            float angle = slotIndex / 3f * Mathf.PI * 2f + Mathf.PI * 0.5f;
            Vector3 marker = player.position + new Vector3(Mathf.Cos(angle), 0.12f, Mathf.Sin(angle)) * 1.25f;
            ProceduralVisualRuntime.Ring("Trigger Source Slot " + (slotIndex + 1), marker, source.accentColor, 0.24f, 0.055f, 0.22f);
            ProceduralVisualRuntime.Beam("Trigger Link · " + moment, marker, position, color, 0.055f, 0.18f, moment == TriggerMoment.OnCriticalHit || moment == TriggerMoment.OnBounce);
            PrimitiveType accent = moment == TriggerMoment.OnKill || moment == TriggerMoment.OnCriticalHit || moment == TriggerMoment.OnFreeze || moment == TriggerMoment.OnShatter || moment == TriggerMoment.OnDamageTaken
                ? PrimitiveType.Cube : moment == TriggerMoment.OnBounce || moment == TriggerMoment.OnStatusApplied || moment == TriggerMoment.OnChannelTick ? PrimitiveType.Capsule : PrimitiveType.Sphere;
            ProceduralVisualRuntime.Burst("Trigger Condition · " + moment, position, color, 0.28f, 0.14f, accent);
            if (moment == TriggerMoment.OnCriticalHit || moment == TriggerMoment.OnShatter)
                for (int i = 0; i < 4; i++) ProceduralVisualRuntime.Beam("Trigger Fracture · " + moment, position,
                    position + Quaternion.Euler(0f, i * 90f + 45f, 0f) * Vector3.forward * 0.72f, color, 0.03f, 0.13f, moment == TriggerMoment.OnCriticalHit);
            else if (moment == TriggerMoment.OnExpire)
                for (int i = 0; i < 2; i++) ProceduralVisualRuntime.Ring("Expire Collapse " + i, position, color, 0.72f - i * 0.24f, 0.04f, 0.16f);
            else if (moment == TriggerMoment.OnDodge)
                for (int i = -1; i <= 1; i += 2) ProceduralVisualRuntime.Beam("Dodge Trigger Afterimage", position + Vector3.right * i * 0.22f,
                    position + Vector3.right * i * 0.22f - Vector3.forward * 0.8f, color, 0.035f, 0.14f, false);
            else if (moment == TriggerMoment.OnDistance)
                ProceduralVisualRuntime.Beam("Distance Trigger Measure", marker, position, color, 0.03f, 0.2f, false);
            else if (moment == TriggerMoment.OnChannelTick)
                for (int i = 0; i < 3; i++) ProceduralVisualRuntime.Ring("Channel Tick " + i, position, color, 0.22f + i * 0.11f, 0.03f, 0.12f + i * 0.025f);
        }

        private static void Anticipation(SpellVisualDescriptor descriptor, Vector3 origin, Vector3 direction)
        {
            float radius = descriptor.delivery == SpellDelivery.Nova || descriptor.delivery == SpellDelivery.Defensive ? Mathf.Max(0.85f, descriptor.radius * 0.28f) : Mathf.Max(0.48f, descriptor.coreScale * 0.72f);
            Color color = descriptor.triggered ? descriptor.secondary : descriptor.primary;
            ProceduralVisualRuntime.Ring(descriptor.triggered ? "Triggered Anticipation" : "Manual Anticipation", origin, color,
                radius, descriptor.triggered ? 0.045f : 0.09f, descriptor.triggered ? 0.1f : 0.16f);
            if (!descriptor.triggered)
                ProceduralVisualRuntime.Beam("Cast Direction", origin, origin + direction.normalized * Mathf.Max(0.8f, radius), color, 0.045f, 0.1f, false);
            TextMesh symbol = VisualAccessibility.AddWorldSymbol(TemporaryRoot(origin, 0.28f).transform, VisualAccessibility.ElementSymbol(descriptor.element), color, 0.15f);
            if (symbol != null) symbol.transform.localPosition = Vector3.up * 0.45f;
        }

        private static void DeliveryManifestation(SpellVisualDescriptor descriptor, CastRequest request)
        {
            switch (descriptor.delivery)
            {
                case SpellDelivery.Projectile:
                    ProceduralVisualRuntime.Burst("Projectile Muzzle", request.origin, descriptor.primary, descriptor.coreScale * 0.48f, 0.1f, descriptor.coreShape);
                    break;
                case SpellDelivery.Nova:
                    ProceduralVisualRuntime.Ring("Nova Interior", request.origin, descriptor.secondary, Mathf.Max(0.65f, descriptor.radius * 0.18f), 0.14f, 0.2f);
                    for (int i = 0; i < 8; i++) RadialPart("Nova Spoke", PrimitiveType.Capsule, request.origin, descriptor.primary, Mathf.Max(0.7f, descriptor.radius * 0.22f), i, 8, 0.14f);
                    break;
                case SpellDelivery.Hitscan:
                    Glyph(request.targetPosition, descriptor.primary, Mathf.Max(0.45f, descriptor.coreScale), 0.12f, "Hitscan Target Mark");
                    ProceduralVisualRuntime.Beam("Hitscan Trace", request.origin, request.targetPosition, descriptor.primary, 0.045f, 0.09f, descriptor.element == SpellElement.Lightning);
                    break;
                case SpellDelivery.Beam:
                    for (int i = 0; i < 3; i++)
                    {
                        LineRenderer aperture = ProceduralVisualRuntime.Ring("Beam Aperture " + i, request.origin, Color.Lerp(descriptor.secondary, descriptor.primary, i / 2f),
                            0.35f + i * 0.18f, 0.07f, 0.26f);
                        if (aperture != null) aperture.transform.rotation = Quaternion.LookRotation(request.direction, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
                    }
                    break;
                case SpellDelivery.Meteor:
                    Glyph(request.targetPosition, descriptor.secondary, Mathf.Max(1.2f, descriptor.radius), Mathf.Max(0.4f, descriptor.lifetime * 0.2f), "Meteor Landing Lattice");
                    ProceduralVisualRuntime.Beam("Meteor Descent Axis", request.targetPosition + Vector3.up * 8f, request.targetPosition, descriptor.primary, 0.055f, 0.28f, false);
                    break;
                case SpellDelivery.Summon:
                    SummonSeal(request.origin, descriptor);
                    break;
                case SpellDelivery.Movement:
                    Glyph(request.origin, descriptor.secondary, 0.75f, 0.24f, "Movement Departure Seal");
                    ProceduralVisualRuntime.Beam("Movement Intent", request.origin, request.targetPosition, descriptor.primary, 0.065f, 0.16f, descriptor.element == SpellElement.Void);
                    break;
                case SpellDelivery.Zone:
                    ZoneBoundary(request.targetPosition, descriptor.radius, descriptor, "Zone Manifestation");
                    break;
                case SpellDelivery.Melee:
                    MeleeSweep(request.origin, request.direction, descriptor);
                    break;
                case SpellDelivery.Defensive:
                    DefensiveFacets(request.origin, descriptor);
                    break;
            }
        }

        private static void PatternCue(SpellVisualDescriptor descriptor, Vector3 origin, Vector3 direction)
        {
            if (descriptor.delivery != SpellDelivery.Projectile || descriptor.projectileCount <= 1) return;
            int count = Mathf.Clamp(descriptor.projectileCount, 2, 12);
            Vector3 right = Vector3.Cross(Vector3.up, direction.normalized);
            for (int i = 0; i < count; i++)
            {
                Vector3 from = origin;
                Vector3 to;
                if (descriptor.pattern == ProjectilePattern.Ring)
                    to = origin + Quaternion.Euler(0f, i / (float)count * 360f, 0f) * Vector3.forward * 1.25f;
                else if (descriptor.pattern == ProjectilePattern.Converge)
                {
                    from += right * Mathf.Lerp(-1.1f, 1.1f, i / (float)(count - 1));
                    to = origin + direction.normalized * 1.5f;
                }
                else
                {
                    float spread = descriptor.pattern == ProjectilePattern.Cone ? 38f : 16f;
                    to = origin + Quaternion.Euler(0f, Mathf.Lerp(-spread, spread, i / (float)(count - 1)), 0f) * direction.normalized * 1.2f;
                }
                ProceduralVisualRuntime.Beam("Projectile Pattern · " + descriptor.pattern, from, to, descriptor.secondary, 0.035f, 0.12f, false);
            }
        }

        private static void SupportAccents(SpellVisualDescriptor descriptor, Vector3 origin, Vector3 direction)
        {
            SupportVisualFlags flags = descriptor.supportFlags;
            if ((flags & SupportVisualFlags.Homing) != 0)
                for (int i = 0; i < 2; i++) ProceduralVisualRuntime.Ring("Homing Lock", origin + direction * (0.45f + i * 0.24f), descriptor.secondary, 0.22f + i * 0.08f, 0.035f, 0.16f);
            if ((flags & SupportVisualFlags.Arc) != 0)
                for (int i = 0; i < 3; i++) ProceduralVisualRuntime.Beam("Arc Flight Guide " + i, origin + direction * i * 0.35f,
                    origin + direction * (i + 1) * 0.35f + Vector3.up * Mathf.Sin((i + 1) / 3f * Mathf.PI) * 0.42f, descriptor.secondary, 0.03f, 0.14f, false);
            if ((flags & SupportVisualFlags.Pierce) != 0)
                ProceduralVisualRuntime.Beam("Pierce Spine", origin, origin + direction * 1.7f, Color.Lerp(Color.white, descriptor.primary, 0.55f), 0.035f, 0.14f, false);
            if ((flags & SupportVisualFlags.Chain) != 0)
                for (int i = -1; i <= 1; i += 2) ProceduralVisualRuntime.Beam("Chain Branch", origin + direction * 0.55f,
                    origin + direction * 0.9f + Quaternion.Euler(0f, i * 42f, 0f) * direction * 0.45f, descriptor.secondary, 0.035f, 0.14f, true);
            if ((flags & SupportVisualFlags.Bounce) != 0)
            {
                LineRenderer bounce = ProceduralVisualRuntime.Ring("Bounce Facet", origin + direction * 0.48f, descriptor.secondary, 0.26f, 0.04f, 0.16f);
                if (bounce != null) bounce.transform.rotation = Quaternion.Euler(45f, 0f, 45f);
            }
            if ((flags & SupportVisualFlags.Return) != 0)
                ProceduralVisualRuntime.Ring("Return Hook", origin - direction * 0.35f, descriptor.secondary, 0.36f, 0.05f, 0.18f);
            if ((flags & SupportVisualFlags.Orbit) != 0)
            {
                LineRenderer orbit = ProceduralVisualRuntime.Ring("Orbit Track", origin, descriptor.secondary, 0.72f, 0.04f, 0.18f);
                if (orbit != null) orbit.transform.rotation = Quaternion.Euler(22f, 0f, 12f);
            }
            if ((flags & SupportVisualFlags.Accelerate) != 0)
                for (int i = 0; i < 3; i++) ProceduralVisualRuntime.Beam("Acceleration Chevron " + i,
                    origin + direction * (0.25f + i * 0.24f), origin + direction * (0.42f + i * 0.31f), descriptor.primary, 0.025f + i * 0.008f, 0.12f, false);
            if ((flags & SupportVisualFlags.Split) != 0)
                for (int i = -1; i <= 1; i += 2) ProceduralVisualRuntime.Beam("Split Branch", origin, origin + Quaternion.Euler(0f, i * 32f, 0f) * direction * 0.9f, descriptor.secondary, 0.04f, 0.14f, false);
            if ((flags & SupportVisualFlags.Delay) != 0)
                for (int i = 0; i < 2; i++) ProceduralVisualRuntime.Ring("Delayed Detonation Clock " + i, origin, descriptor.secondary, 0.38f + i * 0.16f, 0.03f, 0.2f + i * 0.04f);
            if ((flags & SupportVisualFlags.Persistent) != 0)
                ProceduralVisualRuntime.Ring("Persistent Effect Anchor", origin, Color.Lerp(descriptor.primary, descriptor.secondary, 0.5f), 0.62f, 0.045f, 0.2f, null, true);
            if ((flags & SupportVisualFlags.SpreadStatus) != 0)
                for (int i = 0; i < 3; i++) ProceduralVisualRuntime.Beam("Status Spread Branch " + i, origin,
                    origin + Quaternion.Euler(0f, i * 120f, 0f) * Vector3.forward * 0.68f, descriptor.secondary, 0.028f, 0.14f, false);
            if ((flags & SupportVisualFlags.ConsumeStatus) != 0)
                ProceduralVisualRuntime.Ring("Status Consumption Inward Rule", origin, Color.Lerp(descriptor.secondary, Color.white, 0.3f), 0.48f, 0.055f, 0.14f);
            if ((flags & SupportVisualFlags.Barrier) != 0)
                DefensiveFacets(origin, descriptor);
            if ((flags & SupportVisualFlags.Reflect) != 0)
                ReflectiveFacets(origin, descriptor, direction);
            if ((flags & SupportVisualFlags.Pull) != 0)
                for (int i = 0; i < 2; i++) ProceduralVisualRuntime.Ring("Pull Inward Cue", origin, descriptor.secondary, 1.2f - i * 0.35f, 0.04f, 0.2f);
            if ((flags & SupportVisualFlags.Repeat) != 0)
                for (int i = 0; i < Mathf.Min(3, descriptor.repeatCount); i++) ProceduralVisualRuntime.Beam("Repeat Beat " + (i + 1),
                    origin + Vector3.right * (i - 1) * 0.18f, origin + Vector3.right * (i - 1) * 0.18f + Vector3.up * (0.24f + i * 0.06f), descriptor.secondary, 0.035f, 0.16f, false);
            if ((flags & SupportVisualFlags.Summon) != 0)
                ProceduralVisualRuntime.Ring("Summon Payload Count", origin, descriptor.secondary, 0.42f + Mathf.Min(4, descriptor.summonCount) * 0.06f, 0.04f, 0.18f);
        }

        private static void LegendaryAccent(SpellVisualDescriptor descriptor, Vector3 origin, Vector3 target)
        {
            LegendaryVisualProfile profile = descriptor.legendaryProfile;
            if (profile == null) return;
            Vector3 center = profile.usesGroundSeal ? target : origin;
            float baseScale = Mathf.Max(0.55f, descriptor.coreScale * profile.scale);
            if (profile.usesGroundSeal) Glyph(center, descriptor.secondary, Mathf.Max(1f, descriptor.radius * 0.4f), 0.35f, profile.construction + " Seal");
            int layers = Mathf.Min(profile.layerCount, ProceduralVisualRuntime.Quality == ArcaneVisualQuality.Low ? 2 : profile.layerCount);
            for (int i = 0; i < layers; i++)
            {
                float angle = i / (float)Mathf.Max(1, layers) * Mathf.PI * 2f;
                Vector3 offset = profile.usesOrbit ? new Vector3(Mathf.Cos(angle), 0.35f + i * 0.08f, Mathf.Sin(angle)) * baseScale : Vector3.up * (0.25f + i * 0.22f);
                GameObject part = ProceduralVisualRuntime.AcquirePrimitive(i == 0 ? profile.authorityShape : profile.accentShape,
                    "Legendary · " + profile.construction + " · Layer " + i, center + offset, Vector3.one * baseScale * (i == 0 ? 0.28f : 0.13f),
                    Color.Lerp(descriptor.primary, descriptor.secondary, i / (float)Mathf.Max(1, layers - 1)), null, 1.35f);
                if (part != null) BeginLifetime(part, 0.32f + i * 0.035f, true, 0.8f);
            }
            if (profile.usesTether) ProceduralVisualRuntime.Beam("Legendary Tether · " + profile.construction, origin, target, descriptor.secondary, 0.07f, 0.24f, descriptor.element == SpellElement.Lightning);
        }

        private static void UniqueEquipmentAccents(SpellVisualDescriptor descriptor, Vector3 origin, Vector3 direction)
        {
            if (descriptor.equipmentMutations == null || descriptor.equipmentMutations.Length == 0) return;
            int count = Mathf.Min(descriptor.equipmentMutations.Length, ProceduralVisualRuntime.Quality == ArcaneVisualQuality.Low ? 1 : 2);
            for (int i = 0; i < count; i++)
            {
                UniqueMutation mutation = descriptor.equipmentMutations[i];
                UniqueMutationVisualProfile profile = UniqueMutationVisualRegistry.Get(mutation);
                if (profile == null) continue;
                Color color = Color.Lerp(descriptor.secondary, Color.white, 0.18f + i * 0.18f);
                float side = i == 0 ? -1f : 1f;
                Vector3 center = origin + Vector3.Cross(Vector3.up, direction.normalized) * side * (0.48f + i * 0.12f);
                int marks = Mathf.Min(profile.marks, ProceduralVisualRuntime.Quality == ArcaneVisualQuality.Low ? 2 : 6);
                for (int mark = 0; mark < marks; mark++)
                {
                    float angle = mark / (float)Mathf.Max(1, marks) * Mathf.PI * 2f;
                    float radius = profile.inward ? Mathf.Lerp(0.62f, 0.24f, mark / (float)Mathf.Max(1, marks - 1)) : 0.42f + mark * 0.035f;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0.12f + mark * 0.025f, Mathf.Sin(angle)) * radius;
                    GameObject accent = ProceduralVisualRuntime.AcquirePrimitive(mark == 0 ? profile.authorityShape : profile.accentShape,
                        "Unique Mutation · " + profile.functionalRule + " · " + mark, center + offset, Vector3.one * (mark == 0 ? 0.16f : 0.09f), color, null, 1.05f);
                    if (accent != null) BeginLifetime(accent, 0.18f + mark * 0.018f, true, profile.inward ? 0.35f : 1.35f);
                }
                ProceduralVisualRuntime.Ring("Unique Rule · " + profile.functionalRule, center, color, 0.28f + i * 0.06f, 0.045f, 0.2f);
                if (profile.tether)
                    ProceduralVisualRuntime.Beam("Unique Functional Tether · " + profile.functionalRule, center,
                        center + direction.normalized * (0.68f + i * 0.16f), color, 0.04f, 0.18f, mutation == UniqueMutation.AllLightningNoCritical);
            }
        }

        private static void LinkContextAccent(SpellVisualDescriptor descriptor, Vector3 origin)
        {
            if (descriptor.outgoingSpellLinks <= 0 && descriptor.incomingSpellLinks <= 0) return;
            if (descriptor.outgoingSpellLinks > 0)
                ProceduralVisualRuntime.Ring("Outgoing Spell Link Socket", origin + Vector3.left * 0.38f, descriptor.secondary, 0.16f, 0.035f, descriptor.triggered ? 0.08f : 0.14f);
            if (descriptor.incomingSpellLinks > 0)
                ProceduralVisualRuntime.Ring("Incoming Spell Link Socket", origin + Vector3.right * 0.38f, Color.Lerp(descriptor.primary, Color.white, 0.35f), 0.16f, 0.035f, descriptor.triggered ? 0.08f : 0.14f);
        }

        private static void OverloadAccent(SpellVisualDescriptor descriptor, Vector3 origin)
        {
            int layers = descriptor.overloadTier == SpellOverloadTier.Critical ? 3 : descriptor.overloadTier == SpellOverloadTier.Volatile ? 2 : descriptor.overloadTier == SpellOverloadTier.Charged ? 1 : 0;
            for (int i = 0; i < layers; i++)
            {
                Color color = i == layers - 1 ? Color.Lerp(descriptor.primary, Color.white, 0.35f) : descriptor.secondary;
                LineRenderer ring = ProceduralVisualRuntime.Ring("Overload " + descriptor.overloadTier + " Layer " + i, origin, color, 0.55f + i * 0.22f, 0.045f, 0.18f + i * 0.03f);
                if (ring != null) ring.transform.rotation = Quaternion.Euler(20f + i * 31f, i * 47f, 0f);
            }
        }

        private static void CriticalAccent(SpellVisualDescriptor descriptor, Vector3 position)
        {
            Color color = Color.Lerp(Color.white, descriptor.primary, 0.35f);
            for (int i = 0; i < 4; i++) RadialPart("Critical Directional Accent", PrimitiveType.Cube, position, color, descriptor.coreScale * 1.2f, i, 4, 0.14f);
            if (ProfileManager.Current == null || !ProfileManager.Current.accessibility.reducedFlashes)
                ProceduralVisualRuntime.LimitedLight(position + Vector3.up * 0.5f, descriptor.primary, Mathf.Clamp(descriptor.coreScale * 4f, 2f, 7f), 1.65f, 0.09f, null, 3);
        }

        private static void SummonSeal(Vector3 position, SpellVisualDescriptor descriptor)
        {
            Glyph(position, descriptor.primary, 1.15f, 0.32f, "Summon Binding Seal");
            for (int i = 0; i < Mathf.Clamp(descriptor.summonCount, 1, 6); i++)
            {
                float angle = i / (float)Mathf.Max(1, descriptor.summonCount) * Mathf.PI * 2f;
                ProceduralVisualRuntime.Beam("Summon Binding", position, position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 1.15f,
                    descriptor.secondary, 0.04f, 0.2f, false);
            }
        }

        private static void ZoneBoundary(Vector3 position, float radius, SpellVisualDescriptor descriptor, string name)
        {
            float bounded = Mathf.Clamp(radius, 0.7f, 8f);
            ProceduralVisualRuntime.Ring(name + " Boundary", position, descriptor.primary, bounded, 0.11f, 0.3f, null, true);
            ProceduralVisualRuntime.Ring(name + " Inner Rule", position, descriptor.secondary, bounded * 0.62f, 0.045f, 0.3f, null, true);
            Glyph(position, descriptor.secondary, bounded * 0.38f, 0.3f, name + " Center");
        }

        private static void MeleeSweep(Vector3 position, Vector3 direction, SpellVisualDescriptor descriptor)
        {
            // Current melee mechanic is radial. The forward mark communicates facing while
            // the complete ring remains the authoritative damage footprint.
            float radius = Mathf.Max(1.1f, descriptor.radius);
            ProceduralVisualRuntime.Ring("Melee Authoritative Radius", position, descriptor.primary, radius, 0.17f, 0.22f);
            for (int i = -2; i <= 2; i++)
            {
                Vector3 end = position + Quaternion.Euler(0f, i * 16f, 0f) * direction.normalized * radius;
                ProceduralVisualRuntime.Beam("Spellblade Sweep", position, end, descriptor.secondary, 0.055f + (2 - Mathf.Abs(i)) * 0.015f, 0.16f, false);
            }
        }

        private static void DefensiveFacets(Vector3 position, SpellVisualDescriptor descriptor)
        {
            for (int i = 0; i < 6; i++)
            {
                float angle = i / 6f * Mathf.PI * 2f;
                GameObject facet = ProceduralVisualRuntime.AcquirePrimitive(PrimitiveType.Cube, "Defensive Facet", position + new Vector3(Mathf.Cos(angle), 0.72f, Mathf.Sin(angle)) * 1.05f,
                    new Vector3(0.34f, 0.58f, 0.08f), Color.Lerp(descriptor.primary, descriptor.secondary, i / 5f), null, 0.9f);
                if (facet == null) continue;
                facet.transform.rotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 8f);
                BeginLifetime(facet, 0.3f, false, 1f);
            }
        }

        private static void ReflectiveFacets(Vector3 position, SpellVisualDescriptor descriptor, Vector3 direction)
        {
            Vector3 right = Vector3.Cross(Vector3.up, direction.normalized);
            for (int i = -1; i <= 1; i++)
            {
                GameObject facet = ProceduralVisualRuntime.AcquirePrimitive(PrimitiveType.Cube, "Projectile Reflection Facet",
                    position + direction.normalized * 0.72f + right * i * 0.42f + Vector3.up * 0.55f,
                    new Vector3(0.28f, 0.5f, 0.055f), Color.Lerp(descriptor.secondary, Color.white, 0.4f), null, 1.05f);
                if (facet == null) continue;
                facet.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up) * Quaternion.Euler(0f, i * 18f, 0f);
                BeginLifetime(facet, 0.22f, false, 1f);
            }
        }

        private static void Glyph(Vector3 position, Color color, float radius, float lifetime, string name)
        {
            ProceduralVisualRuntime.Ring(name + " Outer", position, color, radius, 0.055f, lifetime, null, true);
            for (int i = 0; i < 4; i++)
            {
                float angle = i * Mathf.PI * 0.5f + Mathf.PI * 0.25f;
                Vector3 from = position + new Vector3(Mathf.Cos(angle), 0.05f, Mathf.Sin(angle)) * radius;
                Vector3 to = position + new Vector3(Mathf.Cos(angle + Mathf.PI * 0.5f), 0.05f, Mathf.Sin(angle + Mathf.PI * 0.5f)) * radius;
                ProceduralVisualRuntime.Beam(name + " Rule " + i, from, to, color, 0.03f, lifetime, false);
            }
        }

        private static void RadialPart(string name, PrimitiveType shape, Vector3 position, Color color, float radius, int index, int count, float lifetime)
        {
            float angle = index / (float)Mathf.Max(1, count) * Mathf.PI * 2f;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0.25f, Mathf.Sin(angle)) * Mathf.Max(0.2f, radius * 0.62f);
            GameObject part = ProceduralVisualRuntime.AcquirePrimitive(shape, name, position + offset,
                shape == PrimitiveType.Capsule ? new Vector3(0.1f, radius * 0.45f, 0.1f) : Vector3.one * Mathf.Max(0.1f, radius * 0.22f), color, null, 1.1f);
            if (part == null) return;
            part.transform.rotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, shape == PrimitiveType.Capsule ? 90f : 35f);
            BeginLifetime(part, lifetime, true, 0.9f);
        }

        private static void BeginLifetime(GameObject part, float lifetime, bool expand, float growth)
        {
            PooledVisualLifetime life = part.GetComponent<PooledVisualLifetime>();
            if (life == null) life = part.AddComponent<PooledVisualLifetime>();
            life.Begin(lifetime, expand, growth);
        }

        private static GameObject TemporaryRoot(Vector3 position, float lifetime)
        {
            GameObject root = new GameObject("Temporary Visual Symbol");
            root.transform.position = position;
            Object.Destroy(root, lifetime);
            return root;
        }
    }

    public sealed class ZoneSustainVisual : MonoBehaviour
    {
        private SpellVisualDescriptor _descriptor;
        private float _radius;
        private float _remaining;
        private float _tick;
        private readonly List<Transform> _innerParts = new List<Transform>();

        public void Initialize(SpellVisualDescriptor descriptor, float radius, float lifetime)
        {
            _descriptor = descriptor;
            _radius = Mathf.Clamp(radius, 0.7f, 8f);
            _remaining = lifetime;
            int parts = ProceduralVisualRuntime.Quality == ArcaneVisualQuality.Low ? 3 : 6;
            for (int i = 0; i < parts; i++)
            {
                float angle = i / (float)parts * Mathf.PI * 2f;
                GameObject part = ProceduralVisualRuntime.AcquirePrimitive(descriptor.shellShape, "Zone Interior Rule", transform.position,
                    Vector3.one * Mathf.Clamp(_radius * 0.08f, 0.08f, 0.32f), descriptor.secondary, transform, 0.75f);
                if (part == null) continue;
                part.transform.localPosition = new Vector3(Mathf.Cos(angle), 0.12f, Mathf.Sin(angle)) * _radius * 0.62f;
                _innerParts.Add(part.transform);
            }
        }

        private void Update()
        {
            if (_descriptor == null) return;
            _remaining -= Time.deltaTime;
            _tick -= Time.deltaTime;
            float speed = _descriptor.motionFamily == SpellMotionFamily.Staccato ? 95f : _descriptor.motionFamily == SpellMotionFamily.Imploding ? -32f : 24f;
            for (int i = 0; i < _innerParts.Count; i++)
            {
                Transform part = _innerParts[i];
                if (!OwnsPart(part)) continue;
                part.Rotate(0f, speed * Time.deltaTime, 0f, Space.Self);
            }
            if (_tick <= 0f)
            {
                _tick = 0.65f;
                ProceduralVisualRuntime.Ring("Zone Authoritative Tick", transform.position, _descriptor.secondary, _radius, 0.035f, 0.12f, null, true);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _innerParts.Count; i++) if (OwnsPart(_innerParts[i])) ProceduralVisualRuntime.Release(_innerParts[i].gameObject);
            _innerParts.Clear();
        }

        private bool OwnsPart(Transform part)
        {
            if (part == null || part.parent != transform) return false;
            PooledVisualMarker marker = part.GetComponent<PooledVisualMarker>();
            return marker != null && marker.InUse;
        }
    }

    public sealed class FamiliarSustainVisual : MonoBehaviour
    {
        private SpellVisualDescriptor _descriptor;
        private int _index;
        private float _pulse;
        public void Initialize(SpellVisualDescriptor descriptor, int index) { _descriptor = descriptor; _index = index; }
        private void Update()
        {
            if (_descriptor == null || GameWorld.Instance == null || GameWorld.Instance.Player == null) return;
            _pulse -= Time.deltaTime;
            if (_pulse > 0f) return;
            _pulse = 0.55f + _index * 0.04f;
            ProceduralVisualRuntime.Beam("Familiar Command Tether", GameWorld.Instance.Player.transform.position, transform.position,
                _descriptor.secondary, 0.025f, 0.12f, _descriptor.element == SpellElement.Lightning);
        }
    }
}
