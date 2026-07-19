using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ArcaneEngine
{
    public static class SpellMorphologyCompiler21
    {
        private sealed class Holder
        {
            public SpellVisualContract21 contract;
        }

        private sealed class BoardRune
        {
            public string id;
            public string displayName;
            public int q;
            public int r;
            public int rotation;
            public int listOrder;
            public int branch;
            public RuneVisualOperatorKind kind;
            public float magnitude;
            public int count;
            public float duration;
        }

        private static readonly ConditionalWeakTable<CompiledSpell, Holder> BySpell =
            new ConditionalWeakTable<CompiledSpell, Holder>();

        private static readonly Dictionary<int, SpellVisualContract21> Cache =
            new Dictionary<int, SpellVisualContract21>();

        public static int CachedContractCount
        {
            get { return Cache.Count; }
        }

        public static SpellVisualContract21 Compile(
            CompiledSpell spell,
            SpellBoard board,
            GeneratedVisualRecipe baseRecipe)
        {
            if (spell == null)
                return null;

            if (baseRecipe == null)
                baseRecipe = ProceduralSpellVisualCompiler2.Compile(spell, board);

            List<BoardRune> boardRunes = ReadBoardRunes(board);
            int hash = ContractHash(spell, baseRecipe, boardRunes);
            SpellVisualContract21 cached;

            if (Cache.TryGetValue(hash, out cached))
            {
                Bind(spell, cached);
                return cached;
            }

            SpellVisualContract21 contract = Build(spell, baseRecipe, boardRunes, hash);
            Cache[hash] = contract;
            Bind(spell, contract);
            return contract;
        }

        public static SpellVisualContract21 Get(CompiledSpell spell)
        {
            if (spell == null)
                return null;

            Holder holder;
            if (BySpell.TryGetValue(spell, out holder) && holder.contract != null)
                return holder.contract;

            GeneratedVisualRecipe recipe = ProceduralSpellVisualCompiler2.Get(spell);
            return Compile(spell, null, recipe);
        }

        public static void ClearCache()
        {
            Cache.Clear();
        }

        private static void Bind(CompiledSpell spell, SpellVisualContract21 contract)
        {
            BySpell.Remove(spell);
            BySpell.Add(spell, new Holder { contract = contract });
        }

        private static SpellVisualContract21 Build(
            CompiledSpell spell,
            GeneratedVisualRecipe recipe,
            List<BoardRune> boardRunes,
            int hash)
        {
            SpellVisualContract21 contract = new SpellVisualContract21();
            contract.configurationHash = hash;
            contract.contractId = "AE21-" + ((uint)hash).ToString("X8");
            contract.coreId = recipe.coreId;
            contract.displayName = recipe.displayName;
            contract.chassis = recipe.chassis;
            contract.deliveryName = recipe.deliveryName;
            contract.signature = recipe.signature;
            contract.catalyst = recipe.catalyst;
            contract.primaryElement = recipe.primaryElement;
            contract.motionElement = recipe.motionElement;
            contract.impactElement = recipe.impactElement;
            contract.residueElement = recipe.residueElement;
            contract.reactionTier = recipe.reactionTier;
            contract.priority = (PresentationPriority)Mathf.Clamp(recipe.visualPriority, 0, 4);
            contract.size = recipe.size;
            contract.speed = recipe.speed;
            contract.radius = recipe.radius;
            contract.duration = recipe.duration;
            contract.projectileCount = recipe.projectileCount;
            contract.topology = recipe.topology;
            contract.baseRecipe = recipe;
            contract.supportsReturn = HasOperator(recipe, RuneVisualOperatorKind.Return);
            contract.supportsPersistence = HasOperator(recipe, RuneVisualOperatorKind.Persistent) ||
                                           recipe.chassis == VisualChassisKind.Zone;
            contract.supportsTargeting = recipe.chassis != VisualChassisKind.Nova &&
                                         recipe.chassis != VisualChassisKind.Movement;

            BuildSeeds(contract);
            BuildCoreInvariants(contract);
            BuildRuneGraph(contract, recipe, boardRunes);
            BuildLifecycle(contract);
            ApplyRuneGraph(contract);
            BuildInteractions(contract);
            contract.personality = ResolvePersonality(contract);
            BuildBody(contract);
            BuildLayerRequirements(contract);
            EstimateCost(contract);
            ValidateContract(contract);
            return contract;
        }

        private static void BuildSeeds(SpellVisualContract21 contract)
        {
            int spell = StableSeed21.Combine(contract.configurationHash, SpellVisualContract21.CurrentSchemaVersion);
            contract.seeds.spell = spell;
            contract.seeds.core = StableSeed21.Combine(spell, "core:" + contract.coreId);
            contract.seeds.elements = StableSeed21.Combine(spell, "elements:" + (int)contract.signature);
            contract.seeds.operators = StableSeed21.Combine(spell, "operators");
            contract.seeds.body = StableSeed21.Combine(spell, "body");
            contract.seeds.trail = StableSeed21.Combine(spell, "trail");
            contract.seeds.impact = StableSeed21.Combine(spell, "impact");
            contract.seeds.residue = StableSeed21.Combine(spell, "residue");
            contract.seeds.audio = StableSeed21.Combine(spell, "audio");
            contract.seeds.icon = StableSeed21.Combine(spell, "icon");
        }

        private static void BuildCoreInvariants(SpellVisualContract21 contract)
        {
            switch (contract.chassis)
            {
                case VisualChassisKind.Beam:
                    Invariant(contract, "source-anchor", "Continuous source connection", true, 1f);
                    Invariant(contract, "directional-line", "Readable directional line", true, 1f);
                    Invariant(contract, "contact-sustain", "Sustained contact rhythm", true, 0.9f);
                    Invariant(contract, "pulse-rhythm", "Repeated pulse cadence", false, 0.6f);
                    break;

                case VisualChassisKind.Zone:
                    Invariant(contract, "world-anchor", "Stable world anchor", true, 1f);
                    Invariant(contract, "boundary", "Authoritative persistent boundary", true, 1f);
                    Invariant(contract, "pulse", "Repeated field behavior", true, 0.9f);
                    Invariant(contract, "center", "Recognizable center structure", false, 0.5f);
                    break;

                case VisualChassisKind.Nova:
                    Invariant(contract, "centered-origin", "Centered origin", true, 1f);
                    Invariant(contract, "radial-release", "Radial release", true, 1f);
                    Invariant(contract, "expansion", "Outward expansion", true, 0.9f);
                    break;

                case VisualChassisKind.Meteor:
                    Invariant(contract, "descending-body", "Descending body", true, 1f);
                    Invariant(contract, "impact-guide", "Impact prediction", true, 1f);
                    Invariant(contract, "accelerating-approach", "Accelerating approach", true, 0.8f);
                    Invariant(contract, "heavy-resolution", "Heavy environmental resolution", true, 1f);
                    break;

                case VisualChassisKind.Melee:
                    Invariant(contract, "sweep", "Directional sweep", true, 1f);
                    Invariant(contract, "reach", "Readable reach", true, 1f);
                    Invariant(contract, "contact", "Immediate contact response", true, 0.8f);
                    break;

                case VisualChassisKind.Familiar:
                    Invariant(contract, "autonomous-body", "Autonomous body", true, 1f);
                    Invariant(contract, "owner-link", "Visible owner relationship", true, 0.8f);
                    Invariant(contract, "attack-cycle", "Independent attack cycle", true, 0.9f);
                    break;

                case VisualChassisKind.Movement:
                    Invariant(contract, "departure", "Visible departure", true, 1f);
                    Invariant(contract, "travel-path", "Readable travel path", true, 0.9f);
                    Invariant(contract, "arrival", "Visible arrival", true, 1f);
                    break;

                default:
                    Invariant(contract, "traveling-body", "Recognizable traveling body", true, 1f);
                    Invariant(contract, "forward-intent", "Directional forward intent", true, 1f);
                    Invariant(contract, "contact-resolution", "Contact-based resolution", true, 0.9f);
                    Invariant(contract, "compact-core", "Core ancestry remains visible", false, 0.6f);
                    break;
            }
        }

        private static void Invariant(
            SpellVisualContract21 contract,
            string id,
            string description,
            bool required,
            float weight)
        {
            contract.coreInvariants.Add(new CoreIdentityInvariant21
            {
                id = id,
                description = description,
                required = required,
                weight = weight
            });
        }

        private static void BuildRuneGraph(
            SpellVisualContract21 contract,
            GeneratedVisualRecipe recipe,
            List<BoardRune> boardRunes)
        {
            HashSet<RuneVisualOperatorKind> represented = new HashSet<RuneVisualOperatorKind>();
            int order = 0;

            for (int i = 0; i < boardRunes.Count; i++)
            {
                BoardRune rune = boardRunes[i];
                if (rune.kind == RuneVisualOperatorKind.None)
                    continue;

                RuneOperatorNode21 node = NodeFrom(
                    rune.kind,
                    order++,
                    rune.id,
                    rune.displayName,
                    rune.q,
                    rune.r,
                    rune.rotation,
                    rune.branch,
                    rune.magnitude,
                    rune.count,
                    rune.duration,
                    contract.chassis);

                contract.runeGraph.Add(node);
                represented.Add(node.kind);
            }

            for (int i = 0; i < recipe.operators.Count; i++)
            {
                RuneVisualOperatorSpec spec = recipe.operators[i];
                if (spec == null || spec.kind == RuneVisualOperatorKind.None)
                    continue;

                if (represented.Contains(spec.kind))
                    continue;

                RuneOperatorNode21 node = NodeFrom(
                    spec.kind,
                    order++,
                    spec.source,
                    spec.kind.ToString(),
                    0,
                    0,
                    0,
                    0,
                    spec.magnitude,
                    spec.count,
                    spec.duration,
                    contract.chassis);

                node.fallback = spec.fallback;
                contract.runeGraph.Add(node);
            }

            contract.runeGraph.Sort(delegate(RuneOperatorNode21 a, RuneOperatorNode21 b)
            {
                int comparison = a.order.CompareTo(b.order);
                if (comparison != 0) return comparison;
                return string.CompareOrdinal(a.id, b.id);
            });
        }

        private static RuneOperatorNode21 NodeFrom(
            RuneVisualOperatorKind kind,
            int order,
            string runeId,
            string displayName,
            int q,
            int r,
            int rotation,
            int branch,
            float magnitude,
            int count,
            float duration,
            VisualChassisKind chassis)
        {
            RuneOperatorNode21 node = new RuneOperatorNode21();
            node.kind = kind;
            node.order = order;
            node.id = "op-" + order + "-" + kind;
            node.sourceRuneId = string.IsNullOrEmpty(runeId) ? kind.ToString() : runeId;
            node.displayName = string.IsNullOrEmpty(displayName) ? kind.ToString() : displayName;
            node.boardQ = q;
            node.boardR = r;
            node.rotation = ((rotation % 6) + 6) % 6;
            node.branch = branch;
            node.magnitude = Mathf.Max(0f, magnitude);
            node.count = Mathf.Max(1, count);
            node.duration = Mathf.Max(0f, duration);
            node.deliveryImplementation = RuneDeliveryCoverage21.Resolve(kind, chassis);
            ConfigureOperator(node);
            return node;
        }

        private static void ConfigureOperator(RuneOperatorNode21 node)
        {
            node.fallback = "shape-preserving glyph and phase pulse";
            node.visibleDuringCasting = true;
            node.visibleDuringTravel = false;
            node.visibleDuringResolution = true;
            node.visibleDuringTermination = false;

            switch (node.kind)
            {
                case RuneVisualOperatorKind.Split:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.Branch;
                    node.targetPhase = SpellPhase21.Emit;
                    node.visibleDuringTravel = true;
                    node.fallback = "fracture seam and child silhouettes";
                    break;

                case RuneVisualOperatorKind.Chain:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.Branch;
                    node.targetPhase = SpellPhase21.Resolve;
                    node.visibleDuringTravel = true;
                    node.fallback = "conductor node and target line";
                    break;

                case RuneVisualOperatorKind.Orbit:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.InsertBefore;
                    node.targetPhase = SpellPhase21.Travel;
                    node.visibleDuringTravel = true;
                    node.fallback = "orbital rail and orbiting node";
                    break;

                case RuneVisualOperatorKind.Return:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.InsertAfter;
                    node.targetPhase = SpellPhase21.Resolve;
                    node.visibleDuringTravel = true;
                    node.visibleDuringTermination = true;
                    node.fallback = "source tether and reverse-flow trail";
                    break;

                case RuneVisualOperatorKind.Bounce:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.Repeat;
                    node.targetPhase = SpellPhase21.Contact;
                    node.visibleDuringTravel = true;
                    node.fallback = "rebound pulse and redirected trail";
                    break;

                case RuneVisualOperatorKind.Delay:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.InsertBefore;
                    node.targetPhase = SpellPhase21.Resolve;
                    node.visibleDuringTermination = true;
                    node.fallback = "countdown glyph and accelerating pulse";
                    break;

                case RuneVisualOperatorKind.Persistent:
                    node.severity = RuneSeverity21.Transformative;
                    node.mutation = PhaseMutation21.Replace;
                    node.targetPhase = SpellPhase21.Expire;
                    node.visibleDuringTravel = true;
                    node.visibleDuringTermination = true;
                    node.fallback = "body unfolds into bounded field";
                    break;

                case RuneVisualOperatorKind.Barrier:
                    node.severity = RuneSeverity21.Transformative;
                    node.mutation = PhaseMutation21.Transform;
                    node.targetPhase = SpellPhase21.Resolve;
                    node.visibleDuringTravel = true;
                    node.visibleDuringTermination = true;
                    node.fallback = "defensive surface and break state";
                    break;

                case RuneVisualOperatorKind.Summon:
                    node.severity = RuneSeverity21.Transformative;
                    node.mutation = PhaseMutation21.Transform;
                    node.targetPhase = SpellPhase21.Emit;
                    node.visibleDuringTravel = true;
                    node.visibleDuringTermination = true;
                    node.fallback = "autonomous body chassis";
                    break;

                case RuneVisualOperatorKind.Pull:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.Transform;
                    node.targetPhase = SpellPhase21.Resolve;
                    node.visibleDuringTravel = true;
                    node.fallback = "inward vector field";
                    break;

                case RuneVisualOperatorKind.Reflect:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.Redirect;
                    node.targetPhase = SpellPhase21.Contact;
                    node.visibleDuringTravel = true;
                    node.fallback = "reflective facet and reversed path";
                    break;

                case RuneVisualOperatorKind.Repeat:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.Repeat;
                    node.targetPhase = SpellPhase21.Resolve;
                    node.visibleDuringTermination = true;
                    node.fallback = "echo pulse sequence";
                    break;

                case RuneVisualOperatorKind.ConsumeStatus:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.Transform;
                    node.targetPhase = SpellPhase21.Resolve;
                    node.visibleDuringTravel = true;
                    node.fallback = "status extraction into compressed core";
                    break;

                case RuneVisualOperatorKind.SpreadStatus:
                    node.severity = RuneSeverity21.Structural;
                    node.mutation = PhaseMutation21.Branch;
                    node.targetPhase = SpellPhase21.Resolve;
                    node.visibleDuringTravel = true;
                    node.fallback = "status transfer tendril";
                    break;

                case RuneVisualOperatorKind.Homing:
                    node.severity = RuneSeverity21.Modifier;
                    node.mutation = PhaseMutation21.Transform;
                    node.targetPhase = SpellPhase21.AcquireTarget;
                    node.visibleDuringTravel = true;
                    node.fallback = "targeting eye and curved motion guide";
                    break;

                case RuneVisualOperatorKind.Arc:
                    node.severity = RuneSeverity21.Modifier;
                    node.mutation = PhaseMutation21.Transform;
                    node.targetPhase = SpellPhase21.Travel;
                    node.visibleDuringTravel = true;
                    node.fallback = "arched guide line";
                    break;

                case RuneVisualOperatorKind.Pierce:
                    node.severity = RuneSeverity21.Modifier;
                    node.mutation = PhaseMutation21.Repeat;
                    node.targetPhase = SpellPhase21.Contact;
                    node.visibleDuringTravel = true;
                    node.fallback = "forward spike and continuation streak";
                    break;

                case RuneVisualOperatorKind.Accelerate:
                    node.severity = RuneSeverity21.Modifier;
                    node.mutation = PhaseMutation21.Transform;
                    node.targetPhase = SpellPhase21.Travel;
                    node.visibleDuringTravel = true;
                    node.fallback = "tightening wake and faster surface flow";
                    break;

                case RuneVisualOperatorKind.Movement:
                    node.severity = RuneSeverity21.Transformative;
                    node.mutation = PhaseMutation21.Redirect;
                    node.targetPhase = SpellPhase21.Travel;
                    node.visibleDuringCasting = true;
                    node.visibleDuringTravel = true;
                    node.visibleDuringTermination = true;
                    node.fallback = "departure path, travel wake and arrival glyph";
                    break;

                default:
                    node.severity = RuneSeverity21.Modifier;
                    node.mutation = PhaseMutation21.Preserve;
                    node.targetPhase = SpellPhase21.Resolve;
                    break;
            }
        }

        private static void BuildLifecycle(SpellVisualContract21 contract)
        {
            AddPhase(contract, SpellPhase21.Prime, "Core", 0f, 0.05f, false);
            AddPhase(contract, SpellPhase21.Charge, "Core", 0.05f, 0.18f, true);
            AddPhase(contract, SpellPhase21.Release, "Core", 0.23f, 0.06f, true);
            AddPhase(contract, SpellPhase21.Emit, "Core", 0.29f, 0.05f, true);

            switch (contract.chassis)
            {
                case VisualChassisKind.Beam:
                    AddPhase(contract, SpellPhase21.AcquireTarget, "Beam", 0.34f, 0.06f, true);
                    AddPhase(contract, SpellPhase21.Contact, "Beam", 0.40f, 0.08f, true);
                    AddPhase(contract, SpellPhase21.Persist, "Beam sustain", 0.48f, 0.38f, true);
                    AddPhase(contract, SpellPhase21.Resolve, "Beam pulse", 0.72f, 0.08f, true);
                    break;

                case VisualChassisKind.Zone:
                    AddPhase(contract, SpellPhase21.Contact, "World anchor", 0.34f, 0.06f, true);
                    AddPhase(contract, SpellPhase21.Persist, "Zone", 0.40f, 0.48f, true);
                    AddPhase(contract, SpellPhase21.Resolve, "Zone pulse", 0.64f, 0.08f, true);
                    break;

                case VisualChassisKind.Nova:
                    AddPhase(contract, SpellPhase21.Resolve, "Radial expansion", 0.34f, 0.46f, true);
                    break;

                case VisualChassisKind.Meteor:
                    AddPhase(contract, SpellPhase21.Hold, "Impact guide", 0.34f, 0.08f, true);
                    AddPhase(contract, SpellPhase21.Travel, "Descent", 0.42f, 0.30f, true);
                    AddPhase(contract, SpellPhase21.Contact, "Ground contact", 0.72f, 0.04f, true);
                    AddPhase(contract, SpellPhase21.Resolve, "Heavy impact", 0.76f, 0.14f, true);
                    break;

                case VisualChassisKind.Melee:
                    AddPhase(contract, SpellPhase21.Travel, "Sweep", 0.34f, 0.16f, true);
                    AddPhase(contract, SpellPhase21.Contact, "Hit", 0.50f, 0.05f, true);
                    AddPhase(contract, SpellPhase21.Resolve, "Follow-through", 0.55f, 0.18f, true);
                    break;

                case VisualChassisKind.Familiar:
                    AddPhase(contract, SpellPhase21.Persist, "Autonomous body", 0.34f, 0.48f, true);
                    AddPhase(contract, SpellPhase21.AcquireTarget, "Familiar target", 0.50f, 0.08f, true);
                    AddPhase(contract, SpellPhase21.Resolve, "Familiar attack", 0.62f, 0.10f, true);
                    break;

                case VisualChassisKind.Movement:
                    AddPhase(contract, SpellPhase21.Travel, "Movement path", 0.34f, 0.32f, true);
                    AddPhase(contract, SpellPhase21.Contact, "Arrival", 0.66f, 0.05f, true);
                    AddPhase(contract, SpellPhase21.Resolve, "Arrival response", 0.71f, 0.14f, true);
                    break;

                default:
                    AddPhase(contract, SpellPhase21.AcquireTarget, "Targeting", 0.34f, 0.05f, false);
                    AddPhase(contract, SpellPhase21.Travel, "Projectile", 0.39f, 0.32f, true);
                    AddPhase(contract, SpellPhase21.Contact, "Impact", 0.71f, 0.05f, true);
                    AddPhase(contract, SpellPhase21.Resolve, "Impact resolution", 0.76f, 0.12f, true);
                    break;
            }

            AddPhase(contract, SpellPhase21.Expire, "Core termination", 0.90f, 0.10f, true);
            LinkLinear(contract.lifecycle);
        }

        private static void AddPhase(
            SpellVisualContract21 contract,
            SpellPhase21 phase,
            string source,
            float start,
            float duration,
            bool required)
        {
            contract.lifecycle.Add(new LifecycleNode21
            {
                id = "phase-" + contract.lifecycle.Count + "-" + phase,
                phase = phase,
                source = source,
                normalizedStart = start,
                normalizedDuration = duration,
                required = required
            });
        }

        private static void LinkLinear(List<LifecycleNode21> phases)
        {
            for (int i = 0; i < phases.Count - 1; i++)
            {
                if (!phases[i].next.Contains(phases[i + 1].id))
                    phases[i].next.Add(phases[i + 1].id);
            }
        }

        private static void ApplyRuneGraph(SpellVisualContract21 contract)
        {
            for (int i = 0; i < contract.runeGraph.Count; i++)
            {
                RuneOperatorNode21 node = contract.runeGraph[i];
                switch (node.kind)
                {
                    case RuneVisualOperatorKind.Orbit:
                        InsertBefore(contract, SpellPhase21.Travel, SpellPhase21.Hold, node, "Orbital hold");
                        break;
                    case RuneVisualOperatorKind.Delay:
                        InsertBefore(contract, SpellPhase21.Resolve, SpellPhase21.Hold, node, "Latent countdown");
                        break;
                    case RuneVisualOperatorKind.Return:
                        InsertBefore(contract, SpellPhase21.Expire, SpellPhase21.Return, node, "Reverse traversal");
                        break;
                    case RuneVisualOperatorKind.Persistent:
                        ReplaceExpireWithPersist(contract, node);
                        break;
                    case RuneVisualOperatorKind.Repeat:
                    case RuneVisualOperatorKind.Bounce:
                        InsertBefore(contract, SpellPhase21.Expire, SpellPhase21.Repeat, node, "Repeated resolution");
                        break;
                    case RuneVisualOperatorKind.Split:
                    case RuneVisualOperatorKind.Chain:
                    case RuneVisualOperatorKind.SpreadStatus:
                        AddBranchMetadata(contract, node);
                        break;
                }
            }
            RecalculateTimings(contract.lifecycle);
        }

        private static void InsertBefore(
            SpellVisualContract21 contract,
            SpellPhase21 target,
            SpellPhase21 inserted,
            RuneOperatorNode21 operation,
            string source)
        {
            int index = IndexOfPhase(contract.lifecycle, target);
            if (index < 0)
                index = Mathf.Max(0, contract.lifecycle.Count - 1);

            LifecycleNode21 node = new LifecycleNode21
            {
                id = "phase-op-" + operation.order + "-" + inserted,
                phase = inserted,
                source = source + " · " + operation.kind,
                normalizedDuration = operation.duration > 0f
                    ? Mathf.Clamp(operation.duration / Mathf.Max(0.1f, contract.duration), 0.04f, 0.28f)
                    : 0.08f,
                required = true
            };

            contract.lifecycle.Insert(index, node);
            LinkLinear(contract.lifecycle);
        }

        private static void ReplaceExpireWithPersist(
            SpellVisualContract21 contract,
            RuneOperatorNode21 operation)
        {
            int expire = IndexOfPhase(contract.lifecycle, SpellPhase21.Expire);
            if (expire < 0)
                return;

            if (IndexOfPhase(contract.lifecycle, SpellPhase21.Persist) < 0)
            {
                contract.lifecycle.Insert(expire, new LifecycleNode21
                {
                    id = "phase-op-" + operation.order + "-Persist",
                    phase = SpellPhase21.Persist,
                    source = "Body-to-field transformation · Persistent",
                    normalizedDuration = 0.25f,
                    required = true
                });
            }

            contract.lifecycle[contract.lifecycle.Count - 1].source = "Persistent field expiration";
            LinkLinear(contract.lifecycle);
        }

        private static void AddBranchMetadata(
            SpellVisualContract21 contract,
            RuneOperatorNode21 operation)
        {
            int index = IndexOfPhase(contract.lifecycle, operation.targetPhase);
            if (index < 0)
                index = IndexOfPhase(contract.lifecycle, SpellPhase21.Resolve);
            if (index < 0)
                return;

            LifecycleNode21 phase = contract.lifecycle[index];
            phase.source += " · branches through " + operation.kind;
        }

        private static int IndexOfPhase(List<LifecycleNode21> phases, SpellPhase21 phase)
        {
            for (int i = 0; i < phases.Count; i++)
                if (phases[i].phase == phase)
                    return i;
            return -1;
        }

        private static void RecalculateTimings(List<LifecycleNode21> phases)
        {
            float total = 0f;
            for (int i = 0; i < phases.Count; i++)
                total += Mathf.Max(0.02f, phases[i].normalizedDuration);

            float cursor = 0f;
            for (int i = 0; i < phases.Count; i++)
            {
                float duration = Mathf.Max(0.02f, phases[i].normalizedDuration) / total;
                phases[i].normalizedStart = cursor;
                phases[i].normalizedDuration = duration;
                cursor += duration;
                phases[i].next.Clear();
                if (i + 1 < phases.Count)
                    phases[i].next.Add(phases[i + 1].id);
            }
        }

        private static void BuildInteractions(SpellVisualContract21 contract)
        {
            for (int i = 0; i < contract.runeGraph.Count; i++)
            {
                for (int j = i + 1; j < contract.runeGraph.Count; j++)
                {
                    RuneVisualOperatorKind first = contract.runeGraph[i].kind;
                    RuneVisualOperatorKind second = contract.runeGraph[j].kind;
                    string rule = RuneInteractionRules21.Describe(first, second);
                    if (!string.IsNullOrEmpty(rule))
                        contract.interactionRules.Add(first + " → " + second + ": " + rule);
                }
            }
        }

        private static VisualPersonality21 ResolvePersonality(SpellVisualContract21 contract)
        {
            int structural = 0;
            int transformative = 0;
            bool orbit = false;
            bool chain = false;
            bool pull = false;
            bool barrier = false;
            bool unstable = (int)contract.reactionTier >= (int)ReactionTier.Calamity;

            for (int i = 0; i < contract.runeGraph.Count; i++)
            {
                RuneOperatorNode21 node = contract.runeGraph[i];
                if (node.severity == RuneSeverity21.Structural) structural++;
                if (node.severity == RuneSeverity21.Transformative) transformative++;
                orbit |= node.kind == RuneVisualOperatorKind.Orbit;
                chain |= node.kind == RuneVisualOperatorKind.Chain;
                pull |= node.kind == RuneVisualOperatorKind.Pull;
                barrier |= node.kind == RuneVisualOperatorKind.Barrier;
            }

            if (unstable || structural + transformative >= 6)
                return VisualPersonality21.Unstable;
            if (barrier)
                return VisualPersonality21.Defensive;
            if (pull || contract.primaryElement == ReactionElement.Void)
                return VisualPersonality21.Ritualistic;
            if (chain || contract.motionElement == ReactionElement.Lightning)
                return VisualPersonality21.Predatory;
            if (orbit && contract.topology != null && contract.topology.approximatelySymmetric)
                return VisualPersonality21.Elegant;
            if (contract.impactElement == ReactionElement.Physical || contract.chassis == VisualChassisKind.Meteor)
                return VisualPersonality21.Heavy;
            if (contract.primaryElement == ReactionElement.Toxic || contract.primaryElement == ReactionElement.Blood)
                return VisualPersonality21.Contagious;
            if (structural >= 4)
                return VisualPersonality21.Chaotic;
            if (contract.speed > 13f)
                return VisualPersonality21.Precise;
            return VisualPersonality21.Violent;
        }

        private static void BuildBody(SpellVisualContract21 contract)
        {
            AddPart(contract, BodyPartKind21.Core, "core", contract.primaryElement,
                RuneVisualOperatorKind.None, Vector3.zero, Vector3.zero,
                Vector3.one * Mathf.Clamp(contract.size * 0.26f, 0.12f, 1.15f), true, 0);

            AddPart(contract, BodyPartKind21.Shell, "shell", contract.catalyst,
                RuneVisualOperatorKind.None, Vector3.zero, Vector3.zero,
                Vector3.one * Mathf.Clamp(contract.size * 0.34f, 0.16f, 1.45f), true, 0);

            AddPart(contract, BodyPartKind21.InternalEnergy, "energy", contract.motionElement,
                RuneVisualOperatorKind.None, Vector3.zero, Vector3.zero,
                Vector3.one * Mathf.Clamp(contract.size * 0.18f, 0.08f, 0.8f), true, 0);

            for (int i = 0; i < contract.runeGraph.Count; i++)
                AddOperatorParts(contract, contract.runeGraph[i], i);

            if (contract.chassis == VisualChassisKind.Beam)
            {
                AddPart(contract, BodyPartKind21.MotionGuide, "beam", contract.motionElement,
                    RuneVisualOperatorKind.None, new Vector3(0f, 0f, 0.65f), Vector3.zero,
                    new Vector3(0.08f, 0.08f, 1.3f), true, 0);
            }
            else if (contract.chassis == VisualChassisKind.Zone)
            {
                AddPart(contract, BodyPartKind21.Ring, "ring", contract.primaryElement,
                    RuneVisualOperatorKind.Persistent, Vector3.zero, Vector3.zero,
                    new Vector3(contract.radius, 0.04f, contract.radius), true, 0);
                AddPart(contract, BodyPartKind21.FieldPanel, "ground", contract.residueElement,
                    RuneVisualOperatorKind.Persistent, Vector3.zero, Vector3.zero,
                    new Vector3(contract.radius * 0.9f, 0.02f, contract.radius * 0.9f), true, 0);
            }
            else if (contract.chassis == VisualChassisKind.Meteor)
            {
                AddPart(contract, BodyPartKind21.ImpactAnchor, "impact", contract.impactElement,
                    RuneVisualOperatorKind.None, new Vector3(0f, -0.3f, 0f), Vector3.zero,
                    Vector3.one * Mathf.Clamp(contract.radius, 0.4f, 3f), true, 0);
            }

            AddPart(contract, BodyPartKind21.TrailEmitter, "trail", contract.motionElement,
                RuneVisualOperatorKind.None, new Vector3(0f, 0f, -0.25f), Vector3.zero,
                Vector3.one, true, 0);
            AddPart(contract, BodyPartKind21.ImpactAnchor, "impact", contract.impactElement,
                RuneVisualOperatorKind.None, Vector3.forward * 0.3f, Vector3.zero,
                Vector3.one * Mathf.Clamp(contract.radius * 0.25f, 0.12f, 1f), true, 0);
        }

        private static void AddOperatorParts(
            SpellVisualContract21 contract,
            RuneOperatorNode21 operation,
            int index)
        {
            int seed = StableSeed21.Combine(contract.seeds.body, operation.sourceRuneId);
            float angle = (operation.rotation / 6f) * Mathf.PI * 2f;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            float boardDistance = Mathf.Max(1f, HexDistance(operation.boardQ, operation.boardR));
            Vector3 socket = direction * Mathf.Clamp(0.18f + boardDistance * 0.08f, 0.18f, 0.72f);
            socket.y = (StableSeed21.Unit(seed) - 0.5f) * 0.24f;

            AddPart(contract, BodyPartKind21.RuneNode, "rune-glyph", contract.catalyst,
                operation.kind, socket, new Vector3(90f, operation.rotation * 60f, 0f),
                Vector3.one * Mathf.Clamp(0.10f + operation.magnitude * 0.02f, 0.08f, 0.22f), true, 0);

            switch (operation.kind)
            {
                case RuneVisualOperatorKind.Split:
                    int children = Mathf.Clamp(operation.count, 2, 12);
                    for (int i = 0; i < children; i++)
                    {
                        float childAngle = Mathf.PI * 2f * i / children + angle;
                        AddPart(contract, BodyPartKind21.SecondaryCore, "child-core", contract.primaryElement,
                            operation.kind,
                            new Vector3(Mathf.Cos(childAngle), 0f, Mathf.Sin(childAngle)) * 0.18f,
                            Vector3.zero,
                            Vector3.one * Mathf.Clamp(contract.size * 0.11f, 0.06f, 0.36f), true, 0);
                    }
                    AddPart(contract, BodyPartKind21.FractureSeam, "fracture", contract.impactElement,
                        operation.kind, Vector3.zero, Vector3.zero, Vector3.one * 0.42f, true, 0);
                    break;

                case RuneVisualOperatorKind.Orbit:
                    int nodes = Mathf.Clamp(operation.count, 2, 8);
                    for (int i = 0; i < nodes; i++)
                    {
                        float orbitAngle = Mathf.PI * 2f * i / nodes + angle;
                        AddPart(contract, BodyPartKind21.Orbital, "orbital", contract.motionElement,
                            operation.kind,
                            new Vector3(Mathf.Cos(orbitAngle), 0f, Mathf.Sin(orbitAngle)) * 0.46f,
                            Vector3.zero,
                            Vector3.one * 0.12f, true, i > 3 ? 1 : 0);
                    }
                    AddPart(contract, BodyPartKind21.Ring, "orbit-rail", contract.motionElement,
                        operation.kind, Vector3.zero, new Vector3(90f, 0f, 0f),
                        Vector3.one * 0.55f, true, 0);
                    break;

                case RuneVisualOperatorKind.Chain:
                    int conductors = Mathf.Clamp(operation.count + 1, 2, 8);
                    for (int i = 0; i < conductors; i++)
                    {
                        float conductorAngle = Mathf.PI * 2f * i / conductors + angle;
                        AddPart(contract, BodyPartKind21.Spike, "conductor", contract.motionElement,
                            operation.kind,
                            new Vector3(Mathf.Cos(conductorAngle), 0f, Mathf.Sin(conductorAngle)) * 0.32f,
                            new Vector3(0f, -conductorAngle * Mathf.Rad2Deg, 0f),
                            new Vector3(0.06f, 0.06f, 0.32f), true, i > 4 ? 1 : 0);
                    }
                    break;

                case RuneVisualOperatorKind.Return:
                    AddPart(contract, BodyPartKind21.InternalPath, "tether", contract.motionElement,
                        operation.kind, Vector3.back * 0.35f, Vector3.zero,
                        new Vector3(0.04f, 0.04f, 0.7f), true, 0);
                    break;

                case RuneVisualOperatorKind.Persistent:
                    for (int i = 0; i < 4; i++)
                    {
                        float panelAngle = i * 90f + operation.rotation * 60f;
                        AddPart(contract, BodyPartKind21.FieldPanel, "field-panel", contract.residueElement,
                            operation.kind, Vector3.zero, new Vector3(0f, panelAngle, 0f),
                            new Vector3(0.3f, 0.05f, 0.5f), true, i > 1 ? 1 : 0);
                    }
                    break;

                case RuneVisualOperatorKind.Barrier:
                    AddPart(contract, BodyPartKind21.Shell, "barrier-shell", contract.primaryElement,
                        operation.kind, Vector3.zero, Vector3.zero, Vector3.one * 0.75f, true, 0);
                    break;

                case RuneVisualOperatorKind.Pull:
                    AddPart(contract, BodyPartKind21.Ring, "compression-ring", ReactionElement.Void,
                        operation.kind, Vector3.zero, new Vector3(90f, 0f, 0f),
                        Vector3.one * 0.62f, true, 0);
                    break;

                case RuneVisualOperatorKind.Delay:
                    AddPart(contract, BodyPartKind21.Ring, "countdown-ring", contract.catalyst,
                        operation.kind, Vector3.zero, new Vector3(90f, 0f, 0f),
                        Vector3.one * 0.48f, true, 0);
                    break;

                case RuneVisualOperatorKind.Pierce:
                    AddPart(contract, BodyPartKind21.Spike, "pierce-spike", contract.impactElement,
                        operation.kind, Vector3.forward * 0.24f, Vector3.zero,
                        new Vector3(0.10f, 0.10f, 0.62f), true, 0);
                    break;
            }
        }

        private static void AddPart(
            SpellVisualContract21 contract,
            BodyPartKind21 kind,
            string meshKey,
            ReactionElement element,
            RuneVisualOperatorKind rune,
            Vector3 position,
            Vector3 euler,
            Vector3 scale,
            bool required,
            int qualityMinimum)
        {
            int seed = StableSeed21.Combine(contract.seeds.body, contract.bodyParts.Count + 1);
            seed = StableSeed21.Combine(seed, meshKey);
            contract.bodyParts.Add(new BodyPartSpec21
            {
                id = "body-" + contract.bodyParts.Count + "-" + kind,
                kind = kind,
                meshKey = meshKey,
                element = element,
                runeKind = rune,
                localPosition = position,
                localEuler = euler,
                localScale = scale,
                emissive = kind == BodyPartKind21.InternalEnergy ? 2.2f : 1.2f,
                opacity = kind == BodyPartKind21.Shell ? 0.55f : 1f,
                seed = seed,
                qualityMinimum = qualityMinimum,
                required = required,
                instanced = kind == BodyPartKind21.Crystal ||
                            kind == BodyPartKind21.Spike ||
                            kind == BodyPartKind21.Orbital
            });
        }

        private static void BuildLayerRequirements(SpellVisualContract21 contract)
        {
            contract.requiredLayers.Add("Core chassis identity");
            contract.requiredLayers.Add("Authoritative direction or boundary");
            contract.requiredLayers.Add("Primary element shape and motion");
            contract.requiredLayers.Add("Contact and resolution timing");
            contract.requiredLayers.Add("Valid owner and termination path");

            for (int i = 0; i < contract.runeGraph.Count; i++)
            {
                RuneOperatorNode21 node = contract.runeGraph[i];
                if (node.severity != RuneSeverity21.Modifier)
                    contract.requiredLayers.Add(node.kind + " structural contribution");
            }

            contract.decorativeLayers.Add("Secondary motes and fragments");
            contract.decorativeLayers.Add("Additional smoke or vapor");
            contract.decorativeLayers.Add("Secondary lights");
            contract.decorativeLayers.Add("Optional distortion");
            contract.decorativeLayers.Add("Optional bloom contribution");
            contract.fallbacks.Add("Generated primitive or mesh chassis");
            contract.fallbacks.Add("Shape-coded element material");
            contract.fallbacks.Add("Line or trail mechanic path");
            contract.fallbacks.Add("Generated monochrome atlas mask");
            contract.fallbacks.Add("Procedural audio transient");
        }

        private static void EstimateCost(SpellVisualContract21 contract)
        {
            int structural = 0;
            int modifier = 0;
            for (int i = 0; i < contract.runeGraph.Count; i++)
            {
                if (contract.runeGraph[i].severity == RuneSeverity21.Modifier) modifier++;
                else structural++;
            }

            int elements = Mathf.Max(1, ElementalReactionCodex.CountBits(contract.signature));
            contract.cost.meshInstances = contract.bodyParts.Count;
            contract.cost.emitters = 2 + elements + Mathf.Min(5, structural);
            contract.cost.maximumParticles = 36 + elements * 28 + structural * 24 + modifier * 8;
            contract.cost.transparentCoverage = Mathf.Clamp01(0.08f + contract.radius * 0.025f + elements * 0.025f);
            contract.cost.lineSegments = 8 + CountOperator(contract, RuneVisualOperatorKind.Chain) * 18 +
                                         CountOperator(contract, RuneVisualOperatorKind.Orbit) * 24;
            contract.cost.trailPoints = contract.chassis == VisualChassisKind.Zone ? 0 : 24 + structural * 8;
            contract.cost.temporaryLights = (int)contract.priority >= (int)PresentationPriority.Critical ? 2 : 1;
            contract.cost.distortionLayers =
                contract.signature == ReactionElement.None ? 0 :
                ElementalReactionCodex.Contains(contract.signature, ReactionElement.Void) ||
                ElementalReactionCodex.Contains(contract.signature, ReactionElement.Fire) ? 1 : 0;
            contract.cost.audioVoices = 1 + Mathf.Min(4, structural) + (contract.supportsPersistence ? 1 : 0);
            contract.cost.cameraEvents = (int)contract.priority >= (int)PresentationPriority.Important ? 2 : 1;
            contract.cost.residues = contract.supportsPersistence ? Mathf.Clamp(contract.projectileCount, 1, 4) : 0;
        }

        private static int CountOperator(SpellVisualContract21 contract, RuneVisualOperatorKind kind)
        {
            int count = 0;
            for (int i = 0; i < contract.runeGraph.Count; i++)
                if (contract.runeGraph[i].kind == kind)
                    count++;
            return count;
        }

        private static void ValidateContract(SpellVisualContract21 contract)
        {
            if (contract.bodyParts.Count == 0)
                contract.validationWarnings.Add("No generated body parts.");
            if (contract.lifecycle.Count < 4)
                contract.validationWarnings.Add("Lifecycle graph is incomplete.");
            if (contract.signature == ReactionElement.None)
                contract.validationWarnings.Add("No reactive element signature; Arcane fallback is used.");

            for (int i = 0; i < contract.runeGraph.Count; i++)
            {
                RuneOperatorNode21 node = contract.runeGraph[i];
                int visiblePhases = 0;
                if (node.visibleDuringCasting) visiblePhases++;
                if (node.visibleDuringTravel) visiblePhases++;
                if (node.visibleDuringResolution) visiblePhases++;
                if (node.visibleDuringTermination) visiblePhases++;
                if (node.severity != RuneSeverity21.Modifier && visiblePhases < 2)
                    contract.validationWarnings.Add(node.kind + " does not satisfy the two-phase visibility guarantee.");
            }
        }

        private static bool HasOperator(GeneratedVisualRecipe recipe, RuneVisualOperatorKind kind)
        {
            for (int i = 0; i < recipe.operators.Count; i++)
                if (recipe.operators[i] != null && recipe.operators[i].kind == kind)
                    return true;
            return false;
        }

        private static List<BoardRune> ReadBoardRunes(SpellBoard board)
        {
            List<BoardRune> result = new List<BoardRune>();
            if (board == null)
                return result;

            object placedObject = Read(board, "placed", null);
            IEnumerable enumerable = placedObject as IEnumerable;
            if (enumerable == null)
                return result;

            int order = 0;
            Dictionary<int, int> branchByAngle = new Dictionary<int, int>();

            foreach (object piece in enumerable)
            {
                if (piece == null)
                    continue;

                string id = Convert.ToString(Read(piece, "modifierId", string.Empty));
                if (string.IsNullOrEmpty(id))
                    continue;

                object origin = Read(piece, "origin", null);
                int q = ReadInt(origin, "q", 0);
                int r = ReadInt(origin, "r", 0);
                int rotation = ReadInt(piece, "rotation", 0);
                SpellModifierDefinition definition = DemoCatalog.GetModifier(id);
                string displayName = definition == null ? id : definition.displayName;
                string text = id + " " + displayName + " " +
                              (definition == null ? string.Empty : definition.shortDescription) + " " +
                              Convert.ToString(Read(definition, "executionLayer", string.Empty));
                RuneVisualOperatorKind kind = KindFromText(text);
                float angle = Mathf.Atan2(r, q + r * 0.5f) * Mathf.Rad2Deg;
                int sector = Mathf.RoundToInt((angle + 180f) / 60f);
                int branch;
                if (!branchByAngle.TryGetValue(sector, out branch))
                {
                    branch = branchByAngle.Count;
                    branchByAngle[sector] = branch;
                }

                result.Add(new BoardRune
                {
                    id = id,
                    displayName = displayName,
                    q = q,
                    r = r,
                    rotation = rotation,
                    listOrder = order++,
                    branch = branch,
                    kind = kind,
                    magnitude = 1f,
                    count = 1,
                    duration = 0f
                });
            }

            return result;
        }

        private static RuneVisualOperatorKind KindFromText(string text)
        {
            if (Contains(text, "split") || Contains(text, "fork") || Contains(text, "multishot")) return RuneVisualOperatorKind.Split;
            if (Contains(text, "chain")) return RuneVisualOperatorKind.Chain;
            if (Contains(text, "orbit")) return RuneVisualOperatorKind.Orbit;
            if (Contains(text, "return") || Contains(text, "boomerang")) return RuneVisualOperatorKind.Return;
            if (Contains(text, "bounce") || Contains(text, "ricochet")) return RuneVisualOperatorKind.Bounce;
            if (Contains(text, "delay") || Contains(text, "latent")) return RuneVisualOperatorKind.Delay;
            if (Contains(text, "persist") || Contains(text, "zone") || Contains(text, "field")) return RuneVisualOperatorKind.Persistent;
            if (Contains(text, "spread")) return RuneVisualOperatorKind.SpreadStatus;
            if (Contains(text, "consume") || Contains(text, "detonate")) return RuneVisualOperatorKind.ConsumeStatus;
            if (Contains(text, "barrier") || Contains(text, "shield")) return RuneVisualOperatorKind.Barrier;
            if (Contains(text, "pull") || Contains(text, "gravity")) return RuneVisualOperatorKind.Pull;
            if (Contains(text, "reflect")) return RuneVisualOperatorKind.Reflect;
            if (Contains(text, "homing") || Contains(text, "seek")) return RuneVisualOperatorKind.Homing;
            if (Contains(text, "arc") || Contains(text, "lob")) return RuneVisualOperatorKind.Arc;
            if (Contains(text, "pierce")) return RuneVisualOperatorKind.Pierce;
            if (Contains(text, "accelerat") || Contains(text, "speed")) return RuneVisualOperatorKind.Accelerate;
            if (Contains(text, "repeat") || Contains(text, "echo")) return RuneVisualOperatorKind.Repeat;
            if (Contains(text, "summon") || Contains(text, "familiar")) return RuneVisualOperatorKind.Summon;
            if (Contains(text, "trigger") || Contains(text, "on hit") || Contains(text, "on kill")) return RuneVisualOperatorKind.Trigger;
            if (Contains(text, "movement") || Contains(text, "dash") || Contains(text, "blink") || Contains(text, "teleport")) return RuneVisualOperatorKind.Movement;
            return RuneVisualOperatorKind.None;
        }

        private static bool Contains(string text, string token)
        {
            return !string.IsNullOrEmpty(text) &&
                   text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static int ContractHash(
            CompiledSpell spell,
            GeneratedVisualRecipe recipe,
            List<BoardRune> boardRunes)
        {
            int hash = StableSeed21.Combine(recipe.configurationHash, SpellVisualContract21.CurrentSchemaVersion);
            hash = StableSeed21.Combine(hash, spell.coreId);
            hash = StableSeed21.Combine(hash, recipe.deliveryName);
            hash = StableSeed21.Combine(hash, (int)recipe.signature);
            hash = StableSeed21.Combine(hash, (int)recipe.catalyst);
            for (int i = 0; i < boardRunes.Count; i++)
            {
                BoardRune rune = boardRunes[i];
                hash = StableSeed21.Combine(hash, rune.id);
                hash = StableSeed21.Combine(hash, rune.q);
                hash = StableSeed21.Combine(hash, rune.r);
                hash = StableSeed21.Combine(hash, rune.rotation);
                hash = StableSeed21.Combine(hash, rune.listOrder);
            }
            return hash;
        }

        private static int HexDistance(int q, int r)
        {
            int s = -q - r;
            return (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s)) / 2;
        }

        private static object Read(object source, string member, object fallback)
        {
            if (source == null)
                return fallback;

            Type type = source.GetType();
            FieldInfo field = type.GetField(member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field.GetValue(source);

            PropertyInfo property = type.GetProperty(member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanRead)
                return property.GetValue(source, null);

            return fallback;
        }

        private static int ReadInt(object source, string member, int fallback)
        {
            object value = Read(source, member, fallback);
            try { return Convert.ToInt32(value); }
            catch { return fallback; }
        }
    }

    public static class RuneDeliveryCoverage21
    {
        public static string Resolve(RuneVisualOperatorKind kind, VisualChassisKind chassis)
        {
            switch (kind)
            {
                case RuneVisualOperatorKind.Chain:
                    switch (chassis)
                    {
                        case VisualChassisKind.Beam: return "Native · branch from contact";
                        case VisualChassisKind.Zone: return "Context · pulse links inside field";
                        case VisualChassisKind.Nova: return "Context · radial transfer";
                        case VisualChassisKind.Meteor: return "Context · fragment transfer";
                        case VisualChassisKind.Melee: return "Context · hit propagation";
                        case VisualChassisKind.Familiar: return "Native · familiar emission";
                        case VisualChassisKind.Movement: return "Fallback · arrival transfer";
                        default: return "Native · impact target graph";
                    }

                case RuneVisualOperatorKind.Split:
                    switch (chassis)
                    {
                        case VisualChassisKind.Beam: return "Native · beam branches";
                        case VisualChassisKind.Zone: return "Context · multiple fields";
                        case VisualChassisKind.Nova: return "Context · sector division";
                        case VisualChassisKind.Meteor: return "Native · multiple descending bodies";
                        case VisualChassisKind.Melee: return "Context · multiple sweeps";
                        case VisualChassisKind.Familiar: return "Context · attacks or bodies split";
                        case VisualChassisKind.Movement: return "Fallback · path fan";
                        default: return "Native · child bodies";
                    }

                case RuneVisualOperatorKind.Orbit:
                    switch (chassis)
                    {
                        case VisualChassisKind.Beam: return "Context · rotating beam anchor";
                        case VisualChassisKind.Zone: return "Native · orbiting field nodes";
                        case VisualChassisKind.Nova: return "Context · rotating sectors";
                        case VisualChassisKind.Meteor: return "Context · orbital descent";
                        case VisualChassisKind.Melee: return "Context · orbiting blades";
                        case VisualChassisKind.Familiar: return "Native · familiar orbit";
                        case VisualChassisKind.Movement: return "Fallback · orbital destination";
                        default: return "Native · pre-launch orbit";
                    }

                case RuneVisualOperatorKind.Return:
                    switch (chassis)
                    {
                        case VisualChassisKind.Beam: return "Context · reverse sweep";
                        case VisualChassisKind.Zone: return "Context · contracting field";
                        case VisualChassisKind.Nova: return "Native · inward nova";
                        case VisualChassisKind.Meteor: return "Context · returning fragments";
                        case VisualChassisKind.Melee: return "Context · rebound sweep";
                        case VisualChassisKind.Familiar: return "Native · recall";
                        case VisualChassisKind.Movement: return "Native · return path";
                        default: return "Native · reverse traversal";
                    }

                case RuneVisualOperatorKind.Persistent:
                    return chassis == VisualChassisKind.Zone
                        ? "Native · strengthens field identity"
                        : "Transformative · terminal body becomes a field";

                case RuneVisualOperatorKind.Bounce:
                    return chassis == VisualChassisKind.Zone
                        ? "Fallback · pulse rebounds within boundary"
                        : "Native or context-specific redirection";

                case RuneVisualOperatorKind.Movement:
                    return chassis == VisualChassisKind.Movement
                        ? "Native · departure, route and arrival"
                        : "Context · movement path and destination anchors";

                default:
                    return "Generic procedural operator with chassis-specific anchors";
            }
        }
    }

    public static class RuneInteractionRules21
    {
        public static string Describe(RuneVisualOperatorKind first, RuneVisualOperatorKind second)
        {
            if (first == RuneVisualOperatorKind.Split && second == RuneVisualOperatorKind.Chain)
                return "Split creates child bodies; every child receives an attenuated target graph.";
            if (first == RuneVisualOperatorKind.Chain && second == RuneVisualOperatorKind.Split)
                return "The dominant target graph resolves first; recipients produce child effects.";
            if (first == RuneVisualOperatorKind.Split && second == RuneVisualOperatorKind.Orbit)
                return "Children receive deterministic orbital lanes before release.";
            if (first == RuneVisualOperatorKind.Orbit && second == RuneVisualOperatorKind.Split)
                return "The parent orbit establishes first; separation inherits orbital phase.";
            if (first == RuneVisualOperatorKind.Chain && second == RuneVisualOperatorKind.Delay)
                return "Recipient nodes establish, remain latent, then resolve in jump order.";
            if (first == RuneVisualOperatorKind.Delay && second == RuneVisualOperatorKind.Chain)
                return "The initial effect resolves after its countdown, then creates the target graph.";
            if (first == RuneVisualOperatorKind.Persistent && second == RuneVisualOperatorKind.Bounce)
                return "Each ordered contact may leave an attenuated residue.";
            if (first == RuneVisualOperatorKind.Bounce && second == RuneVisualOperatorKind.Persistent)
                return "The bounce sequence completes before the terminal field is created.";
            if (first == RuneVisualOperatorKind.Split && second == RuneVisualOperatorKind.Pull)
                return "Every child may establish a local pull center.";
            if (first == RuneVisualOperatorKind.Pull && second == RuneVisualOperatorKind.Split)
                return "All child paths bend around one shared compression center.";
            if (first == RuneVisualOperatorKind.Return && second == RuneVisualOperatorKind.Orbit)
                return "Return restores an orbit around the source before termination.";
            if (first == RuneVisualOperatorKind.Orbit && second == RuneVisualOperatorKind.Return)
                return "Reverse traversal also reverses orbital handedness and material flow.";
            if (first == RuneVisualOperatorKind.Barrier && second == RuneVisualOperatorKind.Reflect)
                return "Barrier facets display incoming contact and reflected direction.";
            if (first == RuneVisualOperatorKind.ConsumeStatus && second == RuneVisualOperatorKind.Chain)
                return "Extracted status energy propagates through the target graph.";
            if (first == RuneVisualOperatorKind.Chain && second == RuneVisualOperatorKind.ConsumeStatus)
                return "Every target contributes status energy to a shared resolution core.";
            if (first == RuneVisualOperatorKind.Delay && second == RuneVisualOperatorKind.ConsumeStatus)
                return "Extracted status remains visibly suspended until the countdown resolves.";
            return string.Empty;
        }
    }
}
