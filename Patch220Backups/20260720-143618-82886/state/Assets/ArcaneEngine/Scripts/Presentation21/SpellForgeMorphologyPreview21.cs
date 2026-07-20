using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneEngine
{
    public enum SpellForgePreviewMode21
    {
        Identity,
        Lifecycle,
        RuneIsolation,
        Readability
    }

    public static class SpellForgeMorphologyPreview21
    {
        private static readonly Dictionary<int, Texture2D> IconCache = new Dictionary<int, Texture2D>();
        private static readonly Dictionary<string, SpellVisualContract21> PreviousByCore = new Dictionary<string, SpellVisualContract21>();
        private static SpellForgePreviewMode21 _mode = SpellForgePreviewMode21.Lifecycle;
        private static int _isolatedRune;

        public static VisualElement Build(CompiledSpell spell, SpellBoard board)
        {
            VisualElement panel = new VisualElement();
            panel.name = "Patch210MorphologyPreview";
            panel.style.marginTop = 8f;
            panel.style.marginBottom = 8f;
            panel.style.paddingLeft = 9f;
            panel.style.paddingRight = 9f;
            panel.style.paddingTop = 8f;
            panel.style.paddingBottom = 8f;
            panel.style.backgroundColor = new Color(0.018f, 0.03f, 0.055f, 0.98f);
            SetBorder(panel, new Color(0.2f, 0.72f, 0.95f, 0.88f), 1f);

            if (spell == null)
            {
                panel.Add(Label("PATCH 2.1 MORPHOLOGY PREVIEW · unavailable", 12, Color.gray, true));
                return panel;
            }

            GeneratedVisualRecipe recipe = ProceduralSpellVisualCompiler2.Compile(spell, board);
            SpellVisualContract21 contract = SpellMorphologyCompiler21.Compile(spell, board, recipe);
            if (contract == null)
            {
                panel.Add(Label("PATCH 2.1 MORPHOLOGY PREVIEW · contract generation failed", 12, Color.red, true));
                return panel;
            }

            VisualElement header = Row();
            VisualElement icon = new VisualElement();
            icon.style.width = 64f;
            icon.style.height = 64f;
            icon.style.flexShrink = 0f;
            icon.style.backgroundImage = new StyleBackground(Icon(contract));
            icon.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            SetBorder(icon, GeneratedAssetRuntime21.ElementColor(contract.catalyst, true), 1f);
            header.Add(icon);

            VisualElement identity = new VisualElement();
            identity.style.flexGrow = 1f;
            identity.style.paddingLeft = 9f;
            identity.Add(Label("PROCEDURAL SPELL MORPHOLOGY", 12, new Color(0.25f, 0.85f, 1f), true));
            identity.Add(Label(
                contract.chassis + " · " + contract.personality + " · " +
                ElementalReactionCodex.SignatureText(contract.signature),
                11,
                Color.white,
                true));
            identity.Add(Label(
                "Visual fingerprint " + contract.contractId +
                " · cost " + contract.cost.Score +
                " · " + contract.bodyParts.Count + " body parts",
                9,
                new Color(0.58f, 0.68f, 0.78f),
                false));
            header.Add(identity);
            panel.Add(header);

            VisualElement controls = Row();
            controls.style.marginTop = 6f;
            controls.Add(ModeButton("IDENTITY", SpellForgePreviewMode21.Identity, panel, spell, board));
            controls.Add(ModeButton("LIFECYCLE", SpellForgePreviewMode21.Lifecycle, panel, spell, board));
            controls.Add(ModeButton("RUNE", SpellForgePreviewMode21.RuneIsolation, panel, spell, board));
            controls.Add(ModeButton("READABILITY", SpellForgePreviewMode21.Readability, panel, spell, board));
            panel.Add(controls);

            BuildModeContent(panel, contract);
            PreviousByCore[contract.coreId ?? string.Empty] = contract;
            return panel;
        }

        public static string DiffSummary(
            CompiledSpell before,
            CompiledSpell after,
            SpellBoard previewBoard)
        {
            if (before == null || after == null)
                return string.Empty;

            SpellVisualContract21 beforeContract = SpellMorphologyCompiler21.Compile(
                before,
                null,
                ProceduralSpellVisualCompiler2.Compile(before, null));
            SpellVisualContract21 afterContract = SpellMorphologyCompiler21.Compile(
                after,
                previewBoard,
                ProceduralSpellVisualCompiler2.Compile(after, previewBoard));

            if (beforeContract == null || afterContract == null)
                return string.Empty;

            List<string> changes = new List<string>();
            if (beforeContract.lifecycle.Count != afterContract.lifecycle.Count)
                changes.Add("phases " + beforeContract.lifecycle.Count + "→" + afterContract.lifecycle.Count);
            if (beforeContract.runeGraph.Count != afterContract.runeGraph.Count)
                changes.Add("Rune graph " + beforeContract.runeGraph.Count + "→" + afterContract.runeGraph.Count);
            if (beforeContract.bodyParts.Count != afterContract.bodyParts.Count)
                changes.Add("body parts " + beforeContract.bodyParts.Count + "→" + afterContract.bodyParts.Count);
            if (beforeContract.personality != afterContract.personality)
                changes.Add("personality " + beforeContract.personality + "→" + afterContract.personality);
            if (beforeContract.cost.Score != afterContract.cost.Score)
                changes.Add("visual cost " + beforeContract.cost.Score + "→" + afterContract.cost.Score);

            RuneOperatorNode21 added = FindAdded(beforeContract, afterContract);
            if (added != null)
            {
                changes.Insert(0, added.kind + " rewrites " + added.targetPhase);
                if (added.visibleDuringCasting) changes.Add("casting cue");
                if (added.visibleDuringTravel) changes.Add("travel/sustain cue");
                if (added.visibleDuringResolution) changes.Add("resolution cue");
                if (added.visibleDuringTermination) changes.Add("termination cue");
            }

            if (changes.Count == 0)
                return "Visual morphology remains structurally stable.";
            return "Visual: " + string.Join(" · ", changes.ToArray());
        }

        public static Texture2D Icon(SpellVisualContract21 contract)
        {
            if (contract == null) return null;
            Texture2D cached;
            if (IconCache.TryGetValue(contract.seeds.icon, out cached) && cached != null)
                return cached;

            const int size = 96;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false, true);
            texture.name = "AE21 Icon " + contract.contractId;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            Color primary = GeneratedAssetRuntime21.ElementColor(contract.primaryElement, false);
            Color secondary = GeneratedAssetRuntime21.ElementColor(contract.catalyst, true);
            Color background = new Color(primary.r * 0.08f, primary.g * 0.08f, primary.b * 0.08f, 1f);
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (x + 0.5f) / size * 2f - 1f;
                    float v = (y + 0.5f) / size * 2f - 1f;
                    float distance = Mathf.Sqrt(u * u + v * v);
                    float angle = Mathf.Atan2(v, u);
                    float coreShape = CoreShape(contract.chassis, u, v, distance);
                    float runeShape = RuneShape(contract, u, v, angle, distance);
                    float border = Mathf.SmoothStep(0.06f, 0f, Mathf.Abs(distance - 0.88f));
                    float noise = StableSeed21.Unit(StableSeed21.Combine(contract.seeds.icon, x + y * size));
                    Color value = background;
                    value = Color.Lerp(value, primary, coreShape * (0.72f + noise * 0.15f));
                    value = Color.Lerp(value, secondary, runeShape * 0.9f);
                    value = Color.Lerp(value, Color.white, border * 0.55f);
                    value.a = 1f;
                    pixels[y * size + x] = value;
                }
            }
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            IconCache[contract.seeds.icon] = texture;
            return texture;
        }

        private static void BuildModeContent(VisualElement panel, SpellVisualContract21 contract)
        {
            VisualElement content = new VisualElement();
            content.name = "Patch210MorphologyContent";
            content.style.marginTop = 6f;
            panel.Add(content);

            switch (_mode)
            {
                case SpellForgePreviewMode21.Identity:
                    BuildIdentity(content, contract);
                    break;
                case SpellForgePreviewMode21.RuneIsolation:
                    BuildRuneIsolation(content, contract);
                    break;
                case SpellForgePreviewMode21.Readability:
                    BuildReadability(content, contract);
                    break;
                default:
                    BuildLifecycle(content, contract);
                    break;
            }
        }

        private static void BuildIdentity(VisualElement content, SpellVisualContract21 contract)
        {
            content.Add(Label("CORE IDENTITY", 11, Color.white, true));
            for (int i = 0; i < contract.coreInvariants.Count; i++)
            {
                CoreIdentityInvariant21 invariant = contract.coreInvariants[i];
                content.Add(Label((invariant.required ? "◆ " : "◇ ") + invariant.description, 10,
                    invariant.required ? new Color(0.78f, 0.93f, 1f) : new Color(0.58f, 0.68f, 0.78f), false));
            }
            content.Add(Label(
                "Material roles · silhouette " + contract.primaryElement +
                " · catalyst " + contract.catalyst +
                " · motion " + contract.motionElement +
                " · impact " + contract.impactElement +
                " · residue " + contract.residueElement,
                9,
                new Color(0.58f, 0.68f, 0.78f),
                false));
            content.Add(BuildAnimatedArena(contract, -1, false));
        }

        private static void BuildLifecycle(VisualElement content, SpellVisualContract21 contract)
        {
            content.Add(Label("LIFECYCLE · " + contract.PhaseText, 10, Color.white, true));
            VisualElement timeline = Row();
            timeline.style.height = 28f;
            for (int i = 0; i < contract.lifecycle.Count; i++)
            {
                LifecycleNode21 phase = contract.lifecycle[i];
                VisualElement block = new VisualElement();
                block.style.flexGrow = Mathf.Max(0.4f, phase.normalizedDuration * 8f);
                block.style.height = 22f;
                block.style.marginRight = 2f;
                block.style.backgroundColor = Color.Lerp(
                    GeneratedAssetRuntime21.ElementColor(contract.primaryElement, false),
                    GeneratedAssetRuntime21.ElementColor(contract.catalyst, true),
                    i / (float)Mathf.Max(1, contract.lifecycle.Count - 1));
                Label text = Label(phase.phase.ToString().Substring(0, Mathf.Min(3, phase.phase.ToString().Length)).ToUpperInvariant(), 8, Color.white, true);
                text.style.unityTextAlign = TextAnchor.MiddleCenter;
                block.Add(text);
                timeline.Add(block);
            }
            content.Add(timeline);
            content.Add(BuildAnimatedArena(contract, -1, false));
        }

        private static void BuildRuneIsolation(VisualElement content, SpellVisualContract21 contract)
        {
            if (contract.runeGraph.Count == 0)
            {
                content.Add(Label("No structural Rune operators are active.", 10, new Color(0.58f, 0.68f, 0.78f), false));
                content.Add(BuildAnimatedArena(contract, -1, false));
                return;
            }

            _isolatedRune = Mathf.Clamp(_isolatedRune, 0, contract.runeGraph.Count - 1);
            VisualElement selector = Row();
            Button previous = new Button(delegate { _isolatedRune = (_isolatedRune + contract.runeGraph.Count - 1) % contract.runeGraph.Count; RebuildContent(content.parent, contract); });
            previous.text = "◀";
            selector.Add(previous);
            RuneOperatorNode21 node = contract.runeGraph[_isolatedRune];
            Label title = Label(node.kind + " · " + node.mutation + " " + node.targetPhase, 10, Color.white, true);
            title.style.flexGrow = 1f;
            title.style.unityTextAlign = TextAnchor.MiddleCenter;
            selector.Add(title);
            Button next = new Button(delegate { _isolatedRune = (_isolatedRune + 1) % contract.runeGraph.Count; RebuildContent(content.parent, contract); });
            next.text = "▶";
            selector.Add(next);
            content.Add(selector);
            content.Add(Label(
                node.deliveryImplementation + "\n" +
                "Casting " + YesNo(node.visibleDuringCasting) +
                " · Travel " + YesNo(node.visibleDuringTravel) +
                " · Resolution " + YesNo(node.visibleDuringResolution) +
                " · Termination " + YesNo(node.visibleDuringTermination) +
                "\nFallback · " + node.fallback,
                9,
                new Color(0.68f, 0.78f, 0.88f),
                false));
            content.Add(BuildAnimatedArena(contract, _isolatedRune, false));
        }

        private static void BuildReadability(VisualElement content, SpellVisualContract21 contract)
        {
            content.Add(Label("REQUIRED READABILITY LAYERS", 10, Color.white, true));
            for (int i = 0; i < contract.requiredLayers.Count; i++)
                content.Add(Label("◆ " + contract.requiredLayers[i], 9, new Color(0.78f, 0.93f, 1f), false));
            content.Add(Label("Low quality preserves chassis, direction/boundary, element symbol, structural Rune nodes and impact timing.", 9, new Color(0.58f, 0.68f, 0.78f), false));
            content.Add(BuildAnimatedArena(contract, -1, true));
        }

        private static VisualElement BuildAnimatedArena(
            SpellVisualContract21 contract,
            int isolatedRune,
            bool requiredOnly)
        {
            VisualElement arena = new VisualElement();
            arena.style.height = 118f;
            arena.style.marginTop = 6f;
            arena.style.position = Position.Relative;
            arena.style.overflow = Overflow.Hidden;
            arena.style.backgroundColor = new Color(0.008f, 0.014f, 0.025f, 0.98f);
            SetBorder(arena, new Color(0.12f, 0.28f, 0.42f), 1f);

            List<VisualElement> actors = new List<VisualElement>();
            int maximum = requiredOnly ? 8 : 18;
            for (int i = 0; i < contract.bodyParts.Count && actors.Count < maximum; i++)
            {
                BodyPartSpec21 part = contract.bodyParts[i];
                if (requiredOnly && !part.required) continue;
                if (isolatedRune >= 0 && part.runeKind != RuneVisualOperatorKind.None &&
                    part.runeKind != contract.runeGraph[isolatedRune].kind) continue;

                VisualElement actor = new VisualElement();
                actor.userData = part;
                actor.style.position = Position.Absolute;
                float size = Mathf.Clamp(7f + part.localScale.magnitude * 14f, 7f, 28f);
                actor.style.width = size;
                actor.style.height = size;
                actor.style.borderTopLeftRadius = size;
                actor.style.borderTopRightRadius = size;
                actor.style.borderBottomLeftRadius = size;
                actor.style.borderBottomRightRadius = size;
                actor.style.backgroundColor = GeneratedAssetRuntime21.ElementColor(part.element, false);
                actor.style.opacity = part.kind == BodyPartKind21.Shell ? 0.55f : 0.95f;
                if (part.kind == BodyPartKind21.RuneNode || part.kind == BodyPartKind21.FractureSeam)
                {
                    actor.style.borderLeftWidth = actor.style.borderRightWidth = actor.style.borderTopWidth = actor.style.borderBottomWidth = 2f;
                    actor.style.borderLeftColor = actor.style.borderRightColor = actor.style.borderTopColor = actor.style.borderBottomColor = Color.white;
                    actor.style.backgroundColor = Color.clear;
                }
                arena.Add(actor);
                actors.Add(actor);
            }

            float start = Time.unscaledTime;
            arena.schedule.Execute((Action)delegate
            {
                if (arena.panel == null) return;
                float age = Time.unscaledTime - start;
                float phase = Mathf.Repeat(age * 0.22f, 1f);
                SpellPhase21 current = PhaseAt(contract, phase);
                for (int i = 0; i < actors.Count; i++)
                {
                    VisualElement actor = actors[i];
                    BodyPartSpec21 part = actor.userData as BodyPartSpec21;
                    if (part == null) continue;
                    float x = 174f + part.localPosition.x * 82f;
                    float y = 55f - part.localPosition.z * 42f - part.localPosition.y * 45f;
                    if (part.kind == BodyPartKind21.Orbital)
                    {
                        float angle = age * 1.8f + StableSeed21.Unit(part.seed) * Mathf.PI * 2f;
                        x = 174f + Mathf.Cos(angle) * 64f;
                        y = 55f + Mathf.Sin(angle) * 32f;
                    }
                    else if (part.kind == BodyPartKind21.SecondaryCore)
                    {
                        float separation = (int)current >= (int)SpellPhase21.Emit ? 1f : 0.12f;
                        x = 174f + part.localPosition.x * 130f * separation;
                        y = 55f + part.localPosition.z * 65f * separation;
                    }
                    else if (current == SpellPhase21.Travel && contract.chassis == VisualChassisKind.Projectile)
                        x += Mathf.Sin(age * 2f) * 42f;
                    else if (current == SpellPhase21.Return)
                        x -= Mathf.PingPong(age * 45f, 75f);
                    actor.style.left = x - actor.resolvedStyle.width * 0.5f;
                    actor.style.top = y - actor.resolvedStyle.height * 0.5f;
                    float pulse = 0.78f + Mathf.Sin(age * 4f + i) * 0.18f;
                    actor.style.opacity = Mathf.Clamp01((part.kind == BodyPartKind21.Shell ? 0.5f : 0.85f) * pulse + 0.1f);
                }
            }).Every(33);

            return arena;
        }

        private static SpellPhase21 PhaseAt(SpellVisualContract21 contract, float normalized)
        {
            for (int i = 0; i < contract.lifecycle.Count; i++)
            {
                LifecycleNode21 phase = contract.lifecycle[i];
                if (normalized >= phase.normalizedStart && normalized < phase.normalizedStart + phase.normalizedDuration)
                    return phase.phase;
            }
            return SpellPhase21.Expire;
        }

        private static Button ModeButton(
            string text,
            SpellForgePreviewMode21 mode,
            VisualElement panel,
            CompiledSpell spell,
            SpellBoard board)
        {
            Button button = new Button(delegate
            {
                _mode = mode;
                VisualElement replacement = Build(spell, board);
                panel.parent.Insert(panel.parent.IndexOf(panel), replacement);
                panel.RemoveFromHierarchy();
            });
            button.text = text;
            button.style.flexGrow = 1f;
            button.style.height = 25f;
            button.style.fontSize = 9;
            button.style.backgroundColor = _mode == mode
                ? new Color(0.08f, 0.55f, 0.72f)
                : new Color(0.05f, 0.08f, 0.12f);
            return button;
        }

        private static void RebuildContent(VisualElement panel, SpellVisualContract21 contract)
        {
            if (panel == null) return;
            VisualElement old = panel.Q<VisualElement>("Patch210MorphologyContent");
            if (old != null) old.RemoveFromHierarchy();
            BuildModeContent(panel, contract);
        }

        private static RuneOperatorNode21 FindAdded(
            SpellVisualContract21 before,
            SpellVisualContract21 after)
        {
            Dictionary<RuneVisualOperatorKind, int> counts = new Dictionary<RuneVisualOperatorKind, int>();
            for (int i = 0; i < before.runeGraph.Count; i++)
            {
                RuneVisualOperatorKind kind = before.runeGraph[i].kind;
                int count; counts.TryGetValue(kind, out count); counts[kind] = count + 1;
            }
            for (int i = 0; i < after.runeGraph.Count; i++)
            {
                RuneVisualOperatorKind kind = after.runeGraph[i].kind;
                int count; counts.TryGetValue(kind, out count);
                if (count <= 0) return after.runeGraph[i];
                counts[kind] = count - 1;
            }
            return null;
        }

        private static float CoreShape(
            VisualChassisKind chassis,
            float u,
            float v,
            float distance)
        {
            switch (chassis)
            {
                case VisualChassisKind.Beam:
                    return Mathf.SmoothStep(0.16f, 0f, Mathf.Abs(v)) * Mathf.SmoothStep(0.82f, 0.28f, Mathf.Abs(u));
                case VisualChassisKind.Zone:
                    return Mathf.SmoothStep(0.08f, 0f, Mathf.Abs(distance - 0.52f));
                case VisualChassisKind.Nova:
                    return Mathf.SmoothStep(0.12f, 0f, Mathf.Abs(distance - 0.45f)) + Mathf.SmoothStep(0.15f, 0f, distance);
                case VisualChassisKind.Meteor:
                    return Mathf.SmoothStep(0.22f, 0f, Mathf.Abs(u) + Mathf.Abs(v * 0.65f + 0.1f) - 0.42f);
                case VisualChassisKind.Melee:
                    return Mathf.SmoothStep(0.09f, 0f, Mathf.Abs(distance - 0.55f)) * Mathf.Clamp01((u + v + 1f) * 0.6f);
                case VisualChassisKind.Familiar:
                    return Mathf.SmoothStep(0.28f, 0f, distance) + Mathf.SmoothStep(0.08f, 0f, Mathf.Abs(distance - 0.48f));
                default:
                    return Mathf.SmoothStep(0.45f, 0.05f, distance);
            }
        }

        private static float RuneShape(
            SpellVisualContract21 contract,
            float u,
            float v,
            float angle,
            float distance)
        {
            if (contract.runeGraph.Count == 0) return 0f;
            RuneOperatorNode21 primary = contract.runeGraph[0];
            float spokes = Mathf.Abs(Mathf.Sin(angle * (2f + ((int)primary.kind % 5)) + primary.rotation));
            float ring = Mathf.SmoothStep(0.055f, 0f, Mathf.Abs(distance - 0.63f));
            float line = Mathf.SmoothStep(0.04f, 0f, Mathf.Abs(v - Mathf.Sin(u * 5f + primary.rotation) * 0.08f));
            return Mathf.Clamp01(ring * spokes + line * 0.55f);
        }

        private static string YesNo(bool value)
        {
            return value ? "YES" : "—";
        }

        private static VisualElement Row()
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            return row;
        }

        private static Label Label(string text, int size, Color color, bool bold)
        {
            Label label = new Label(text);
            label.style.fontSize = size;
            label.style.color = color;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
            return label;
        }

        private static void SetBorder(VisualElement element, Color color, float width)
        {
            element.style.borderLeftWidth = element.style.borderRightWidth =
                element.style.borderTopWidth = element.style.borderBottomWidth = width;
            element.style.borderLeftColor = element.style.borderRightColor =
                element.style.borderTopColor = element.style.borderBottomColor = color;
        }
    }
}
