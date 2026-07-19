using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public static class Patch200PresentationBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            GameObject existing = GameObject.Find("Arcane Engine 2.0 Presentation");
            if (existing != null)
                return;

            GameObject root = new GameObject("Arcane Engine 2.0 Presentation");
            UnityEngine.Object.DontDestroyOnLoad(root);
            root.AddComponent<SpellPresentationDirector2>();
            root.AddComponent<Patch200PresentationOverlay>();
            root.AddComponent<Patch200PresentationDiagnosticsOverlay>();
            root.AddComponent<Patch200PresentationStressRuntime>();
        }
    }

    public static class ProceduralSpellPresentation
    {
        public static GeneratedVisualRecipe LastRecipe { get; private set; }

        public static GeneratedVisualRecipe Compile(
            CompiledSpell spell,
            SpellBoard board)
        {
            GeneratedVisualRecipe recipe =
                ProceduralSpellVisualCompiler2.Compile(spell, board);

            if (recipe != null)
            {
                LastRecipe = recipe;
                SpellMorphologyPresentation21.Compile(spell, board, recipe);
            }

            return recipe;
        }

        public static GeneratedVisualRecipe Recipe(CompiledSpell spell)
        {
            GeneratedVisualRecipe recipe =
                ProceduralSpellVisualCompiler2.Compile(spell, null);

            if (recipe != null)
            {
                LastRecipe = recipe;
                SpellMorphologyPresentation21.Contract(spell);
            }

            return recipe;
        }

        public static void EmitCast(CompiledSpell spell, CastRequest request)
        {
            if (spell == null)
                return;

            GeneratedVisualRecipe recipe = Recipe(spell);
            Vector3 position = request.Equals(default)
                ? PlayerPosition()
                : request.origin;

            Vector3 direction = request.Equals(default)
                ? Vector3.forward
                : request.direction;

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.CastStarted,
                priority = ResolvePriority(recipe, false),
                spell = spell,
                recipe = recipe,
                source = PlayerTransform(),
                position = position,
                direction = direction,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                tier = recipe == null ? ReactionTier.Fusion : recipe.reactionTier,
                radius = recipe == null ? 1f : recipe.radius,
                duration = 0.3f,
                intensity = 1f
            });
        
            SpellMorphologyPresentation21.EmitCast(spell, request);
}

        public static void EmitImpact(
            CompiledSpell spell,
            CastRequest request,
            Vector3 position,
            bool critical)
        {
            GeneratedVisualRecipe recipe = Recipe(spell);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.ProjectileImpacted,
                priority = ResolvePriority(recipe, critical),
                spell = spell,
                recipe = recipe,
                source = PlayerTransform(),
                position = position,
                direction = request.Equals(default) ? Vector3.up : request.direction,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                tier = recipe == null ? ReactionTier.Fusion : recipe.reactionTier,
                radius = recipe == null ? 1f : Mathf.Max(recipe.radius, recipe.size),
                duration = 0.55f,
                intensity = critical ? 1.45f : 1f,
                critical = critical
            });
        
            SpellMorphologyPresentation21.EmitImpact(spell, request, position, critical);
}

        public static void EmitExpire(
            CompiledSpell spell,
            CastRequest request,
            Vector3 position)
        {
            GeneratedVisualRecipe recipe = Recipe(spell);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.EffectExpired,
                priority = PresentationPriority.Normal,
                spell = spell,
                recipe = recipe,
                source = PlayerTransform(),
                position = position,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                radius = recipe == null ? 0.8f : recipe.radius,
                duration = 0.25f,
                intensity = 0.65f
            });
        
            SpellMorphologyPresentation21.EmitExpire(spell, position);
}

        public static void EmitDirectionChange(
            CompiledSpell spell,
            Vector3 position,
            bool returning)
        {
            GeneratedVisualRecipe recipe = Recipe(spell);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = returning
                    ? SpellPresentationEventType.ProjectileReturned
                    : SpellPresentationEventType.ProjectileRedirected,
                priority = PresentationPriority.Important,
                spell = spell,
                recipe = recipe,
                position = position,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                radius = recipe == null ? 0.5f : recipe.size,
                duration = 0.22f,
                intensity = 0.9f
            });
        
            SpellMorphologyPresentation21.EmitDirectionChange(spell, position, returning);
}

        public static void EmitStatusSpread(
            CompiledSpell spell,
            Vector3 from,
            Vector3 to)
        {
            GeneratedVisualRecipe recipe = Recipe(spell);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.StatusSpread,
                priority = PresentationPriority.Important,
                spell = spell,
                recipe = recipe,
                position = from,
                secondaryPosition = to,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                duration = 0.22f,
                intensity = 0.85f
            });
        }

        public static void EmitStatusConsume(
            CompiledSpell spell,
            Vector3 position)
        {
            GeneratedVisualRecipe recipe = Recipe(spell);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.StatusConsumed,
                priority = PresentationPriority.Important,
                spell = spell,
                recipe = recipe,
                position = position,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                radius = recipe == null ? 1f : recipe.radius,
                duration = 0.35f,
                intensity = 1f
            });
        }

        public static void AttachHost(
            GameObject host,
            CompiledSpell spell,
            CastRequest request)
        {
            if (host == null || spell == null)
                return;

            GeneratedSpellHostVisual2 visual =
                host.GetComponent<GeneratedSpellHostVisual2>();

            if (visual == null)
                visual = host.AddComponent<GeneratedSpellHostVisual2>();

            visual.Initialize(Recipe(spell), spell, request, GeneratedSpellHostKind.Projectile);
        
            SpellMorphologyPresentation21.AttachHost(host, spell, request, GeneratedSpellHostKind.Projectile);
}

        public static void AttachZone(
            GameObject zone,
            CompiledSpell spell,
            CastRequest request,
            float radius,
            float lifetime)
        {
            if (zone == null || spell == null)
                return;

            GeneratedSpellHostVisual2 visual =
                zone.GetComponent<GeneratedSpellHostVisual2>();

            if (visual == null)
                visual = zone.AddComponent<GeneratedSpellHostVisual2>();

            GeneratedVisualRecipe recipe = Recipe(spell);
            visual.Initialize(recipe, spell, request, GeneratedSpellHostKind.Zone);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.ZoneCreated,
                priority = ResolvePriority(recipe, false),
                spell = spell,
                recipe = recipe,
                host = zone,
                position = zone.transform.position,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                radius = radius,
                duration = lifetime,
                intensity = 1f
            });
        
            SpellMorphologyPresentation21.AttachHost(zone, spell, request, GeneratedSpellHostKind.Zone);
}

        public static void AttachFamiliar(
            GameObject familiar,
            CompiledSpell spell,
            CastRequest request,
            int index)
        {
            if (familiar == null || spell == null)
                return;

            GeneratedSpellHostVisual2 visual =
                familiar.GetComponent<GeneratedSpellHostVisual2>();

            if (visual == null)
                visual = familiar.AddComponent<GeneratedSpellHostVisual2>();

            GeneratedVisualRecipe recipe = Recipe(spell);
            visual.Initialize(recipe, spell, request, GeneratedSpellHostKind.Familiar);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.FamiliarCreated,
                priority = PresentationPriority.Important,
                spell = spell,
                recipe = recipe,
                host = familiar,
                position = familiar.transform.position,
                count = index,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                duration = recipe == null ? 2f : recipe.duration,
                intensity = 1f
            });
        
            SpellMorphologyPresentation21.AttachHost(familiar, spell, request, GeneratedSpellHostKind.Familiar);
}

        public static void EmitMovement(
            CompiledSpell spell,
            CastRequest request,
            Vector3 from,
            Vector3 to)
        {
            GeneratedVisualRecipe recipe = Recipe(spell);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.MovementArrival,
                priority = PresentationPriority.Important,
                spell = spell,
                recipe = recipe,
                source = PlayerTransform(),
                position = from,
                secondaryPosition = to,
                direction = (to - from).normalized,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                duration = 0.4f,
                intensity = 1f
            });
        
            SpellMorphologyPresentation21.EmitMovement(spell, from, to);
}

        public static void EmitBarrier(
            Transform player,
            Color color,
            bool broke,
            float strength)
        {
            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.BarrierChanged,
                priority = broke ? PresentationPriority.Critical : PresentationPriority.Important,
                source = player,
                position = player == null ? PlayerPosition() : player.position,
                radius = Mathf.Clamp(0.8f + strength * 0.05f, 0.8f, 3f),
                duration = broke ? 0.45f : 0.25f,
                intensity = broke ? 1.3f : 0.8f,
                critical = broke
            });
        }

        public static void EmitLink(
            CompiledSpell source,
            CompiledSpell destination,
            Vector3 position,
            TriggerMoment moment)
        {
            GeneratedVisualRecipe recipe = Recipe(source);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.LinkActivated,
                priority = PresentationPriority.Important,
                spell = source,
                recipe = recipe,
                source = PlayerTransform(),
                position = position,
                signature = recipe == null ? ReactionElement.None : recipe.signature,
                catalyst = recipe == null ? ReactionElement.None : recipe.catalyst,
                mechanicId = moment.ToString(),
                duration = 0.35f,
                intensity = 1f
            });
        }

        public static void EmitAilment(
            EnemyController owner,
            ReactionElement element,
            float buildup,
            float threshold,
            float duration,
            bool propagated)
        {
            if (owner == null)
                return;

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.AilmentBuildupChanged,
                priority = buildup >= threshold
                    ? PresentationPriority.Critical
                    : PresentationPriority.Important,
                target = owner.transform,
                host = owner.gameObject,
                position = owner.DamagePoint,
                signature = element,
                catalyst = element,
                stackCount = Mathf.CeilToInt(buildup),
                threshold = Mathf.CeilToInt(threshold),
                normalizedProgress = Mathf.Clamp01(buildup / Mathf.Max(0.01f, threshold)),
                duration = duration,
                intensity = propagated ? 0.72f : 1f
            });
        }

        public static void EmitMajorAilment(
            EnemyController owner,
            ReactionElement element,
            float duration)
        {
            if (owner == null)
                return;

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.MajorAilmentActivated,
                priority = PresentationPriority.Critical,
                target = owner.transform,
                host = owner.gameObject,
                position = owner.DamagePoint,
                signature = element,
                catalyst = element,
                normalizedProgress = 1f,
                radius = 1.2f,
                duration = duration,
                intensity = 1.35f
            });
        }

        public static void EmitAssembly(
            EnemyController owner,
            ReactionElement signature,
            ReactionElement catalyst,
            float duration,
            bool upgraded)
        {
            if (owner == null)
                return;

            ElementalReactionDefinition definition =
                ElementalReactionCodex.Get(signature);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = upgraded
                    ? SpellPresentationEventType.ReactionSignatureUpgraded
                    : SpellPresentationEventType.ReactionAssemblyStarted,
                priority = DefinitionPriority(definition),
                target = owner.transform,
                host = owner.gameObject,
                position = owner.DamagePoint,
                signature = signature,
                catalyst = catalyst,
                tier = definition == null ? ReactionTier.Fusion : definition.tier,
                mechanicId = definition == null ? string.Empty : definition.displayName,
                radius = definition == null ? 1.2f : definition.radius,
                duration = duration,
                intensity = 1f + ElementalReactionCodex.CountBits(signature) * 0.08f
            });
        }

        public static void EmitReactionResolved(
            EnemyController owner,
            ElementalReactionDefinition definition,
            float baseDamage,
            bool death)
        {
            if (owner == null || definition == null)
                return;

            ReactionMechanicPlan plan =
                ElementalReactionMechanicCodex.Get(definition.signature);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = death
                    ? SpellPresentationEventType.ReactionDeathTriggered
                    : SpellPresentationEventType.ReactionResolved,
                priority = DefinitionPriority(definition),
                target = owner.transform,
                host = owner.gameObject,
                position = owner.DamagePoint,
                signature = definition.signature,
                catalyst = definition.catalyst,
                tier = definition.tier,
                mechanicId = plan == null ? definition.id : plan.graphId,
                radius = definition.radius,
                duration = definition.duration,
                intensity = Mathf.Clamp(0.75f + definition.ElementCount * 0.17f, 0.9f, 2f),
                death = death
            });
        }

        public static void EmitSingleElementDeath(
            EnemyController owner,
            ReactionElement element,
            float radius,
            float duration)
        {
            if (owner == null)
                return;

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.ReactionDeathTriggered,
                priority = PresentationPriority.Critical,
                target = owner.transform,
                host = owner.gameObject,
                position = owner.DamagePoint,
                signature = element,
                catalyst = element,
                radius = radius,
                duration = duration,
                intensity = 1.25f,
                death = true
            });
        }

        public static void EmitReactionMechanic(
            ReactionMechanicSpec spec,
            ElementalReactionDefinition definition,
            EnemyController owner,
            float baseDamage,
            bool death,
            int mechanicIndex)
        {
            if (owner == null || definition == null)
                return;

            ReactionElement signature = spec.payload == ReactionElement.None
                ? definition.signature
                : spec.payload;

            ReactionElement catalyst = spec.element == ReactionElement.None
                ? definition.catalyst
                : spec.element;

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = SpellPresentationEventType.ReactionMechanic,
                priority = DefinitionPriority(definition),
                target = owner.transform,
                host = owner.gameObject,
                position = owner.DamagePoint,
                signature = signature,
                catalyst = catalyst,
                tier = definition.tier,
                mechanicType = spec.type,
                mechanicId = definition.id + ":" + mechanicIndex + ":" + spec.type,
                radius = spec.radius,
                duration = spec.duration,
                delay = spec.delay,
                count = spec.count,
                intensity = Mathf.Max(0.1f, spec.magnitude),
                death = death
            });
        }

        public static void EmitField(
            SpellPresentationEventType type,
            GameObject field,
            ReactionElement signature,
            float radius,
            float duration,
            float intensity)
        {
            ElementalReactionDefinition definition =
                ElementalReactionCodex.Get(signature);

            SpellPresentationBus.Emit(new SpellPresentationEvent
            {
                type = type,
                priority = DefinitionPriority(definition),
                host = field,
                target = field == null ? null : field.transform,
                position = field == null ? Vector3.zero : field.transform.position,
                signature = signature,
                catalyst = definition == null
                    ? ElementalReactionCodex.PrimaryElement(signature)
                    : definition.catalyst,
                tier = definition == null ? ReactionTier.Fusion : definition.tier,
                radius = radius,
                duration = duration,
                intensity = intensity
            });
        }

        private static PresentationPriority ResolvePriority(
            GeneratedVisualRecipe recipe,
            bool critical)
        {
            if (critical)
                return PresentationPriority.Critical;

            if (recipe == null)
                return PresentationPriority.Normal;

            int elements = ElementalReactionCodex.CountBits(recipe.signature);

            if (elements >= 6)
                return PresentationPriority.Reserved;

            if (elements >= 4)
                return PresentationPriority.Critical;

            return recipe.visualPriority >= 4
                ? PresentationPriority.Important
                : PresentationPriority.Normal;
        }

        private static PresentationPriority DefinitionPriority(
            ElementalReactionDefinition definition)
        {
            if (definition == null)
                return PresentationPriority.Important;

            switch (definition.tier)
            {
                case ReactionTier.Apex:
                case ReactionTier.Calamity:
                    return PresentationPriority.Reserved;

                case ReactionTier.Convergence:
                case ReactionTier.Catastrophe:
                    return PresentationPriority.Critical;

                default:
                    return PresentationPriority.Important;
            }
        }

        private static Transform PlayerTransform()
        {
            return GameWorld.Instance == null ||
                   GameWorld.Instance.Player == null
                ? null
                : GameWorld.Instance.Player.transform;
        }

        private static Vector3 PlayerPosition()
        {
            Transform player = PlayerTransform();
            return player == null ? Vector3.zero : player.position;
        }
    }
}
