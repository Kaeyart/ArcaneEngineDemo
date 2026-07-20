using System;
using System.Collections.Generic;
using System.Linq;

namespace ArcaneEngine
{
    [Serializable]
    public sealed class PlacedModifier
    {
        public string modifierId;
        public HexCoord anchor;
        public int rotation;
        public int placementOrder;

        public PlacedModifier(string id, HexCoord at, int rotated, int order)
        {
            modifierId = id;
            anchor = at;
            rotation = ((rotated % 6) + 6) % 6;
            placementOrder = order;
        }

        public PlacedModifier Clone() { return new PlacedModifier(modifierId, anchor, rotation, placementOrder); }
    }

    [Serializable]
    public sealed class SpellBoard
    {
        public const int BaseRadius = 3;
        public SpellSlot slot;
        public string coreId;
        public string relicId;
        public int spellLevel = 1;
        public int temporaryRadiusBonus;
        public readonly List<PlacedModifier> placed = new List<PlacedModifier>();
        private readonly Stack<List<PlacedModifier>> _undo = new Stack<List<PlacedModifier>>();
        private readonly Stack<List<PlacedModifier>> _redo = new Stack<List<PlacedModifier>>();
        private int _nextOrder;

        public int Radius { get { return BaseRadius + MathfCompat.Clamp(temporaryRadiusBonus, 0, 2); } }
        public int Capacity
        {
            get
            {
                switch (MathfCompat.Clamp(spellLevel, 1, 5))
                {
                    case 1: return 6;
                    case 2: return 8;
                    case 3: return 10;
                    case 4: return 12;
                    default: return 15;
                }
            }
        }

        public SpellBoard(SpellSlot boardSlot, string spellCoreId, string evolutionId = null)
        {
            slot = boardSlot;
            coreId = spellCoreId;
            relicId = evolutionId;
        }

        public bool TryPlace(string modifierId, HexCoord anchor, int rotation, out string reason)
        {
            SpellModifierDefinition definition = DemoCatalog.GetModifier(modifierId);
            if (definition == null) { reason = "Unknown Support Rune."; return false; }
            if (!definition.stackable && placed.Any(p => p.modifierId == modifierId))
            {
                reason = "That rule can only appear once on a board.";
                return false;
            }

            if (!CanPlace(modifierId, anchor, rotation, null, out reason)) return false;

            PushUndo();
            PlacedModifier piece = new PlacedModifier(modifierId, anchor, rotation, _nextOrder++);
            placed.Add(piece);
            reason = IsPlacementConnected(piece) ? "Connected. Spell recompiled." : GetInactiveReason(piece);
            return true;
        }

        public bool CanPlace(string modifierId, HexCoord anchor, int rotation, PlacedModifier ignoredPiece, out string reason)
        {
            SpellModifierDefinition definition = DemoCatalog.GetModifier(modifierId);
            if (definition == null) { reason = "Unknown Support Rune."; return false; }
            if (!definition.availableAsSupport) { reason = "That legacy trigger now belongs in Spell Links."; return false; }
            SpellCoreDefinition core = DemoCatalog.GetCore(coreId);
            if (core == null) { reason = "This Spell Core is missing from the catalog."; return false; }
            if (!definition.IsCompatible(core)) { reason = definition.displayName + " is not compatible with " + core.displayName + "."; return false; }
            List<HexCoord> cells = GetOccupiedCells(definition, anchor, rotation);
            if (cells.Any(c => c.DistanceFromOrigin() > Radius)) { reason = "The Support Rune extends beyond the Spell Board."; return false; }
            if (cells.Any(c => !IsCellUnlocked(c))) { reason = "That cell unlocks at a higher Spell Level."; return false; }
            if (cells.Any(c => c.Equals(new HexCoord(0, 0)))) { reason = "The Spell Core cannot be covered."; return false; }
            Dictionary<HexCoord, PlacedModifier> occupied = GetOccupiedMap();
            if (ignoredPiece != null)
                foreach (HexCoord previous in GetOccupiedCells(definition, ignoredPiece.anchor, ignoredPiece.rotation)) occupied.Remove(previous);
            if (cells.Any(occupied.ContainsKey)) { reason = "The Support Rune overlaps an occupied hex."; return false; }
            int projectedCapacity = ProjectedCapacity(modifierId, anchor, rotation, ignoredPiece);
            if (projectedCapacity > Capacity)
            {
                reason = "Not enough Capacity: this layout needs " + projectedCapacity + " but the spell has " + Capacity + ".";
                return false;
            }
            reason = "Valid placement.";
            return true;
        }

        public bool TryMove(PlacedModifier piece, HexCoord anchor, int rotation, out string reason)
        {
            if (piece == null || !placed.Contains(piece)) { reason = "The installed Support Rune no longer exists."; return false; }
            if (!CanPlace(piece.modifierId, anchor, rotation, piece, out reason)) return false;
            PushUndo();
            piece.anchor = anchor;
            piece.rotation = ((rotation % 6) + 6) % 6;
            reason = IsPlacementConnected(piece) ? "Support Rune moved and connected." : GetInactiveReason(piece);
            return true;
        }

        public bool RemoveAt(HexCoord cell, out string modifierId)
        {
            for (int i = placed.Count - 1; i >= 0; i--)
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(placed[i].modifierId);
                if (definition == null || !GetOccupiedCells(definition, placed[i].anchor, placed[i].rotation).Contains(cell)) continue;
                PushUndo();
                modifierId = placed[i].modifierId;
                placed.RemoveAt(i);
                return true;
            }
            modifierId = null;
            return false;
        }

        public bool RotateAt(HexCoord cell, int steps, out string reason)
        {
            PlacedModifier piece = PieceAt(cell);
            if (piece == null) { reason = "No Support Rune occupies that hex."; return false; }
            int proposed = ((piece.rotation + steps) % 6 + 6) % 6;
            if (!CanPlace(piece.modifierId, piece.anchor, proposed, piece, out reason)) return false;
            PushUndo();
            piece.rotation = proposed;
            reason = IsPlacementConnected(piece) ? "Rotated and connected." : GetInactiveReason(piece);
            return true;
        }

        public bool Undo()
        {
            if (_undo.Count == 0) return false;
            _redo.Push(ClonePlaced());
            Restore(_undo.Pop());
            return true;
        }

        public bool Redo()
        {
            if (_redo.Count == 0) return false;
            _undo.Push(ClonePlaced());
            Restore(_redo.Pop());
            return true;
        }

        public void Clear(bool recordHistory)
        {
            if (recordHistory) PushUndo();
            placed.Clear();
        }

        public void LoadLayout(SavedSpellLayout layout)
        {
            if (layout == null) return;
            PushUndo();
            placed.Clear();
            coreId = layout.coreId;
            relicId = layout.relicId;
            foreach (PlacedModifierSave saved in layout.pieces)
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(saved.modifierId);
                if (definition == null || !definition.availableAsSupport) continue;
                string ignored;
                HexCoord anchor = new HexCoord(saved.q, saved.r);
                if (!CanPlace(saved.modifierId, anchor, saved.rotation, null, out ignored)) continue;
                placed.Add(new PlacedModifier(saved.modifierId, anchor, saved.rotation, _nextOrder++));
            }
        }

        public SavedSpellLayout CreateLayout(string name)
        {
            SavedSpellLayout result = new SavedSpellLayout { name = name, coreId = coreId, relicId = relicId };
            foreach (PlacedModifier piece in placed)
                result.pieces.Add(new PlacedModifierSave { modifierId = piece.modifierId, q = piece.anchor.q, r = piece.anchor.r, rotation = piece.rotation });
            return result;
        }

        public Dictionary<HexCoord, PlacedModifier> GetOccupiedMap()
        {
            Dictionary<HexCoord, PlacedModifier> result = new Dictionary<HexCoord, PlacedModifier>();
            foreach (PlacedModifier piece in placed)
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(piece.modifierId);
                if (definition == null) continue;
                foreach (HexCoord cell in GetOccupiedCells(definition, piece.anchor, piece.rotation)) result[cell] = piece;
            }
            return result;
        }

        public PlacedModifier PieceAt(HexCoord cell)
        {
            PlacedModifier piece;
            return GetOccupiedMap().TryGetValue(cell, out piece) ? piece : null;
        }

        public List<PlacedModifier> GetActivePlacements()
        {
            List<PlacedModifier> active = new List<PlacedModifier>();
            HashSet<PlacedModifier> activeSet = new HashSet<PlacedModifier>();
            bool changed = true;
            int safety = 0;
            while (changed && safety++ < 64)
            {
                changed = false;
                foreach (PlacedModifier piece in placed.OrderBy(p => p.placementOrder))
                {
                    if (activeSet.Contains(piece)) continue;
                    if (!InputTouchesCore(piece) && !InputTouchesActiveOutput(piece, activeSet)) continue;
                    activeSet.Add(piece);
                    active.Add(piece);
                    changed = true;
                }
            }
            return active.OrderBy(p => GraphDistance(p, activeSet)).ThenBy(p => p.placementOrder).ToList();
        }

        public bool IsPlacementConnected(PlacedModifier piece) { return GetActivePlacements().Contains(piece); }

        public string GetInactiveReason(PlacedModifier piece)
        {
            if (piece == null) return "No piece selected.";
            SpellModifierDefinition definition = DemoCatalog.GetModifier(piece.modifierId);
            int side = RotateSide(definition.inputSide, piece.rotation);
            HexCoord predecessor = piece.anchor + HexCoord.Directions[side];
            if (predecessor.DistanceFromOrigin() > Radius) return "Inactive: the input points outside the board.";
            if (!GetOccupiedMap().ContainsKey(predecessor) && !predecessor.Equals(new HexCoord(0, 0)))
                return "Inactive: its input receives no adjacent output.";
            return "Inactive: the neighboring output is not connected to the Spell Core.";
        }

        public int TotalInstability()
        {
            return GetActivePlacements().Sum(p => { SpellModifierDefinition d = DemoCatalog.GetModifier(p.modifierId); return d == null ? 0 : d.instability; });
        }

        public int UsedCapacity()
        {
            return GetActivePlacements().Sum(piece =>
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(piece.modifierId);
                return definition == null ? 0 : MathfCompat.Clamp(definition.capacityCost, 0, 99);
            });
        }

        public SpellBoard CreatePlacementPreview(string modifierId, HexCoord anchor, int rotation, PlacedModifier ignoredPiece)
        {
            string reason;
            if (!CanPlace(modifierId, anchor, rotation, ignoredPiece, out reason)) return null;
            SpellBoard preview = new SpellBoard(slot, coreId, relicId)
            {
                spellLevel = spellLevel,
                temporaryRadiusBonus = temporaryRadiusBonus
            };
            foreach (PlacedModifier existing in placed)
                if (existing != ignoredPiece) preview.placed.Add(existing.Clone());
            int order = ignoredPiece == null ? (placed.Count == 0 ? 0 : placed.Max(value => value.placementOrder) + 1) : ignoredPiece.placementOrder;
            preview.placed.Add(new PlacedModifier(modifierId, anchor, rotation, order));
            return preview;
        }

        public bool TryLevelUp(out string reason)
        {
            if (spellLevel >= 5) { reason = "That spell is already Level 5."; return false; }
            spellLevel++;
            reason = "Spell Level " + spellLevel + " reached. Capacity is now " + Capacity + ".";
            return true;
        }

        public bool IsCellUnlocked(HexCoord cell)
        {
            int distance = cell.DistanceFromOrigin();
            if (distance <= 2) return true;
            if (distance > BaseRadius) return distance <= Radius;
            if (spellLevel >= 5) return true;
            HexCoord[] ring = LevelRing;
            int index = Array.IndexOf(ring, cell);
            if (index < 0) return false;
            int unlocked = spellLevel <= 1 ? 0 : spellLevel == 2 ? 3 : spellLevel == 3 ? 6 : 12;
            return index < unlocked;
        }

        public static int RequiredLevelForCell(HexCoord cell)
        {
            if (cell.DistanceFromOrigin() <= 2) return 1;
            int index = Array.IndexOf(LevelRing, cell);
            if (index < 0) return 5;
            if (index < 3) return 2;
            if (index < 6) return 3;
            if (index < 12) return 4;
            return 5;
        }

        public List<HexCoord> AllCells()
        {
            List<HexCoord> result = new List<HexCoord>();
            for (int q = -Radius; q <= Radius; q++)
            {
                int rMin = Math.Max(-Radius, -q - Radius);
                int rMax = Math.Min(Radius, -q + Radius);
                for (int r = rMin; r <= rMax; r++) result.Add(new HexCoord(q, r));
            }
            return result;
        }

        public static List<HexCoord> GetOccupiedCells(SpellModifierDefinition definition, HexCoord anchor, int rotation)
        {
            return definition.shape.Select(c => anchor + HexCoord.Rotate(c, rotation)).ToList();
        }

        public static int RotateSide(int side, int rotation) { return ((side + rotation) % 6 + 6) % 6; }

        private bool InputTouchesCore(PlacedModifier piece)
        {
            if (GameWorld.Instance != null && GameWorld.Instance.Equipment != null &&
                GameWorld.Instance.Equipment.HasMutation(UniqueMutation.CentralVirtualConnector) && piece.anchor.DistanceFromOrigin() == 1) return true;
            SpellModifierDefinition definition = DemoCatalog.GetModifier(piece.modifierId);
            int side = RotateSide(definition.inputSide, piece.rotation);
            return (piece.anchor + HexCoord.Directions[side]).Equals(new HexCoord(0, 0));
        }

        private bool InputTouchesActiveOutput(PlacedModifier piece, HashSet<PlacedModifier> active)
        {
            SpellModifierDefinition definition = DemoCatalog.GetModifier(piece.modifierId);
            int inputSide = RotateSide(definition.inputSide, piece.rotation);
            HexCoord predecessorCell = piece.anchor + HexCoord.Directions[inputSide];
            foreach (PlacedModifier candidate in active)
            {
                int requiredOutput = (inputSide + 3) % 6;
                SpellModifierDefinition candidateDefinition = DemoCatalog.GetModifier(candidate.modifierId);
                bool faces = candidateDefinition.outputSides.Any(side =>
                {
                    int absoluteSide = RotateSide(side, candidate.rotation);
                    return absoluteSide == requiredOutput && GetOutputPortCell(candidate, absoluteSide).Equals(predecessorCell);
                });
                if (faces) return true;
            }
            return false;
        }

        public static HexCoord GetOutputPortCell(PlacedModifier piece, int absoluteSide)
        {
            SpellModifierDefinition definition = piece == null ? null : DemoCatalog.GetModifier(piece.modifierId);
            if (definition == null) return piece == null ? new HexCoord() : piece.anchor;
            List<HexCoord> cells = GetOccupiedCells(definition, piece.anchor, piece.rotation);
            HexCoord direction = HexCoord.Directions[absoluteSide];
            HashSet<HexCoord> occupied = new HashSet<HexCoord>(cells);
            List<HexCoord> edge = cells.Where(cell => !occupied.Contains(cell + direction)).ToList();
            if (edge.Count == 0) return piece.anchor;
            return edge.OrderByDescending(cell => DirectionScore(cell, absoluteSide)).First();
        }

        private int GraphDistance(PlacedModifier piece, HashSet<PlacedModifier> active)
        {
            int distance = 1;
            PlacedModifier current = piece;
            HashSet<PlacedModifier> visited = new HashSet<PlacedModifier>();
            while (current != null && visited.Add(current) && distance < 64)
            {
                SpellModifierDefinition definition = DemoCatalog.GetModifier(current.modifierId);
                int side = RotateSide(definition.inputSide, current.rotation);
                HexCoord predecessor = current.anchor + HexCoord.Directions[side];
                if (predecessor.Equals(new HexCoord(0, 0))) return distance;
                int requiredOutput = (side + 3) % 6;
                current = active.FirstOrDefault(candidate =>
                {
                    SpellModifierDefinition candidateDefinition = DemoCatalog.GetModifier(candidate.modifierId);
                    return candidateDefinition != null && candidateDefinition.outputSides.Any(output =>
                    {
                        int absoluteSide = RotateSide(output, candidate.rotation);
                        return absoluteSide == requiredOutput && GetOutputPortCell(candidate, absoluteSide).Equals(predecessor);
                    });
                });
                distance++;
            }
            return distance;
        }

        private void PushUndo()
        {
            _undo.Push(ClonePlaced());
            _redo.Clear();
            while (_undo.Count > 30)
            {
                List<List<PlacedModifier>> keep = _undo.Reverse().Skip(1).ToList();
                _undo.Clear();
                foreach (List<PlacedModifier> entry in keep) _undo.Push(entry);
            }
        }

        private List<PlacedModifier> ClonePlaced() { return placed.Select(p => p.Clone()).ToList(); }

        private void Restore(List<PlacedModifier> state)
        {
            placed.Clear();
            placed.AddRange(state.Select(p => p.Clone()));
            _nextOrder = placed.Count == 0 ? 0 : placed.Max(p => p.placementOrder) + 1;
        }

        private int ProjectedCapacity(string modifierId, HexCoord anchor, int rotation, PlacedModifier ignoredPiece)
        {
            SpellBoard simulation = new SpellBoard(slot, coreId, relicId)
            {
                spellLevel = spellLevel,
                temporaryRadiusBonus = temporaryRadiusBonus
            };
            foreach (PlacedModifier existing in placed)
                if (existing != ignoredPiece) simulation.placed.Add(existing.Clone());
            simulation.placed.Add(new PlacedModifier(modifierId, anchor, rotation, int.MaxValue));
            return simulation.UsedCapacity();
        }

        private static float DirectionScore(HexCoord cell, int side)
        {
            float x = cell.q + cell.r * 0.5f;
            float y = cell.r * 0.8660254f;
            HexCoord direction = HexCoord.Directions[side];
            float directionX = direction.q + direction.r * 0.5f;
            float directionY = direction.r * 0.8660254f;
            return x * directionX + y * directionY;
        }

        private static readonly HexCoord[] LevelRing =
        {
            new HexCoord(3, 0), new HexCoord(2, 1), new HexCoord(1, 2),
            new HexCoord(0, 3), new HexCoord(-1, 3), new HexCoord(-2, 3),
            new HexCoord(-3, 3), new HexCoord(-3, 2), new HexCoord(-3, 1),
            new HexCoord(-3, 0), new HexCoord(-2, -1), new HexCoord(-1, -2),
            new HexCoord(0, -3), new HexCoord(1, -3), new HexCoord(2, -3),
            new HexCoord(3, -3), new HexCoord(3, -2), new HexCoord(3, -1)
        };
    }

    internal static class MathfCompat
    {
        public static int Clamp(int value, int min, int max) { return value < min ? min : value > max ? max : value; }
    }
}
