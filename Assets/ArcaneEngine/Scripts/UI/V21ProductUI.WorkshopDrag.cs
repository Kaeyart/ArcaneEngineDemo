using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneEngine
{
    public sealed partial class V21ProductUI
    {
        private VisualElement _workshopFloatingPreview;
        private VisualElement _workshopBoardCanvas;
        private readonly List<VisualElement> _workshopGhosts =
            new List<VisualElement>();

        private SpellBoard _workshopDragBoard;
        private SpellModifierDefinition _workshopDragDefinition;
        private int _workshopPointerId = -1;
        private Vector2 _workshopPointerPosition;
        private HexCoord? _workshopHoverCell;
        private bool _workshopPlacementValid;
        private string _workshopPlacementReason = string.Empty;

        private void InstallWorkshopDragEvents()
        {
            _root.RegisterCallback<PointerMoveEvent>(
                OnWorkshopPointerMove,
                TrickleDown.TrickleDown);

            _root.RegisterCallback<PointerUpEvent>(
                OnWorkshopPointerUp,
                TrickleDown.TrickleDown);

            _root.RegisterCallback<PointerCancelEvent>(
                OnWorkshopPointerCancel,
                TrickleDown.TrickleDown);

            _root.RegisterCallback<WheelEvent>(
                OnWorkshopWheel,
                TrickleDown.TrickleDown);
        }

        private VisualElement CreateWorkshopRuneItem(
            GameWorld world,
            SpellBoard board,
            SpellModifierDefinition rune,
            int count)
        {
            SpellCoreDefinition core = DemoCatalog.GetCore(board.coreId);
            bool compatible =
                rune.availableAsSupport &&
                rune.IsCompatible(core);

            VisualElement item = new VisualElement();
            item.name = "WorkshopRune_" + rune.id;
            item.style.height = 70f;
            item.style.marginBottom = 7f;
            item.style.paddingLeft = 10f;
            item.style.paddingRight = 10f;
            item.style.paddingTop = 7f;
            item.style.paddingBottom = 7f;
            item.style.backgroundColor =
                compatible
                    ? Card
                    : new Color(0.035f, 0.045f, 0.055f, 0.96f);

            item.style.borderLeftWidth =
                item.style.borderRightWidth =
                item.style.borderTopWidth =
                item.style.borderBottomWidth = 1f;

            Color border =
                compatible
                    ? rune.uiColor
                    : new Color(0.2f, 0.23f, 0.27f);

            item.style.borderLeftColor =
                item.style.borderRightColor =
                item.style.borderTopColor =
                item.style.borderBottomColor = border;

            Label title = LabelFor(
                rune.FullDisplayName.Replace("\n", " — ") +
                " ×" + count,
                12,
                compatible ? Text : Muted,
                true);

            title.pickingMode = PickingMode.Ignore;
            item.Add(title);

            string status =
                compatible
                    ? "Capacity " + rune.capacityCost +
                      " · drag onto the board"
                    : "UNAVAILABLE · not supported by " +
                      (core == null ? "this spell" : core.displayName);

            Label description = LabelFor(
                status + "\n" + rune.shortDescription,
                10,
                compatible ? Muted : new Color(0.46f, 0.5f, 0.54f),
                false);

            description.style.whiteSpace = WhiteSpace.Normal;
            description.pickingMode = PickingMode.Ignore;
            item.Add(description);

            item.SetEnabled(compatible && world.CanEditSpells);
            item.style.opacity = compatible ? 1f : 0.4f;

            if (compatible)
            {
                string id = rune.id;

                item.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.button != 0)
                        return;

                    BeginWorkshopDrag(
                        world,
                        board,
                        id,
                        null,
                        evt);

                    evt.StopImmediatePropagation();
                });
            }

            return item;
        }

        private VisualElement CreateWorkshopHexCell(
            GameWorld world,
            SpellBoard board,
            HexCoord cell)
        {
            float centerX =
                285f +
                (cell.q + cell.r * 0.5f) * HexWidth;

            float centerY =
                285f +
                cell.r * HexRowStep;

            PlacedModifier piece = board.PieceAt(cell);
            bool core = cell.Equals(new HexCoord(0, 0));

            SpellModifierDefinition definition =
                piece == null
                    ? null
                    : DemoCatalog.GetModifier(piece.modifierId);

            string labelText =
                core
                    ? "CORE"
                    : definition == null
                        ? board.IsCellUnlocked(cell) ? "·" : "×"
                        : definition.displayName
                            .Substring(
                                0,
                                Mathf.Min(4, definition.displayName.Length))
                            .ToUpperInvariant();

            Color fill =
                core
                    ? DemoCatalog.GetCore(board.coreId).color
                    : definition != null
                        ? definition.uiColor
                        : board.IsCellUnlocked(cell)
                            ? new Color(0.07f, 0.12f, 0.17f)
                            : new Color(0.025f, 0.035f, 0.05f);

            HexCellElement element = new HexCellElement();
            element.name = "SpellHex_" + cell.q + "_" + cell.r;
            element.userData = cell;
            element.style.position = Position.Absolute;
            element.style.left = centerX - HexWidth * 0.5f;
            element.style.top = centerY - HexRadius;
            element.style.width = HexWidth;
            element.style.height = HexRadius * 2f;
            element.FillColor = fill;

            bool connected =
                piece == null ||
                board.IsPlacementConnected(piece);

            element.BorderColor =
                connected
                    ? new Color(0.35f, 0.55f, 0.85f, 0.8f)
                    : new Color(1f, 0.58f, 0.14f, 0.95f);

            element.BorderWidth = piece == null ? 2f : 3f;

            Label label = new Label(labelText);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.color = Text;
            label.style.fontSize = 11;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.pickingMode = PickingMode.Ignore;
            element.Add(label);

            _boardCellCenters[cell] =
                new Vector2(centerX, centerY);

            element.RegisterCallback<PointerDownEvent>(evt =>
            {
                _selectedCell = cell;

                if (evt.button == 0 && piece != null)
                {
                    BeginWorkshopDrag(
                        world,
                        board,
                        piece.modifierId,
                        piece,
                        evt);

                    evt.StopImmediatePropagation();
                    return;
                }

                if (evt.button != 1 || piece == null)
                    return;

                evt.StopImmediatePropagation();

                if (!world.CanEditSpells)
                {
                    SetMessage(world.SpellEditLockReason);
                    return;
                }

                string removedId;

                if (board.RemoveAt(cell, out removedId))
                {
                    world.ReturnModifier(removedId);
                    Changed("Support Rune returned to inventory.");
                }
            });

            return element;
        }

        private void BeginWorkshopDrag(
            GameWorld world,
            SpellBoard board,
            string runeId,
            PlacedModifier movingPiece,
            PointerDownEvent evt)
        {
            if (!world.CanEditSpells)
            {
                SetMessage(world.SpellEditLockReason);
                return;
            }

            SpellModifierDefinition definition =
                DemoCatalog.GetModifier(runeId);

            SpellCoreDefinition core =
                DemoCatalog.GetCore(board.coreId);

            if (definition == null ||
                !definition.availableAsSupport ||
                !definition.IsCompatible(core))
            {
                SetMessage("That Rune is not compatible with this spell.");
                return;
            }

            CancelWorkshopDrag(false);

            _workshopDragBoard = board;
            _workshopDragDefinition = definition;
            _workshopPointerId = evt.pointerId;
            _workshopPointerPosition = evt.position;

            _dragRune = runeId;
            _dragPiece = movingPiece;
            _selectedRune = runeId;
            _isWorkshopDragging = true;

            if (movingPiece != null)
                _rotation = movingPiece.rotation;

            _root.CapturePointer(evt.pointerId);

            CreateWorkshopFloatingPreview();
            MoveWorkshopFloatingPreview(evt.position);
            RefreshWorkshopGhost(evt.position, true);
        }

        private void OnWorkshopPointerMove(PointerMoveEvent evt)
        {
            if (!_isWorkshopDragging ||
                evt.pointerId != _workshopPointerId)
            {
                return;
            }

            MoveWorkshopFloatingPreview(evt.position);
            RefreshWorkshopGhost(evt.position, false);
            evt.StopPropagation();
        }

        private void OnWorkshopPointerUp(PointerUpEvent evt)
        {
            if (!_isWorkshopDragging ||
                evt.pointerId != _workshopPointerId)
            {
                return;
            }

            GameWorld world = GameWorld.Instance;
            SpellBoard board = _workshopDragBoard;
            PlacedModifier piece = _dragPiece;
            string runeId = _dragRune;
            HexCoord? target = _workshopHoverCell;
            bool valid = _workshopPlacementValid;
            string reason = _workshopPlacementReason;

            ReleaseWorkshopPointer();
            ClearWorkshopDragVisuals();

            _isWorkshopDragging = false;
            _dragPiece = null;
            _dragRune = null;
            _selectedRune = null;
            _workshopDragBoard = null;
            _workshopDragDefinition = null;
            _workshopHoverCell = null;
            _workshopPlacementValid = false;

            if (world == null || board == null)
            {
                SetMessage("Drop cancelled: Workshop state changed.");
            }
            else if (!valid || !target.HasValue)
            {
                SetMessage(
                    string.IsNullOrEmpty(reason)
                        ? "Drop cancelled: choose a valid hex."
                        : reason);
            }
            else if (piece != null)
            {
                MovePiece(world, board, piece, target.Value);
            }
            else
            {
                PlaceRune(world, board, runeId, target.Value);
            }

            evt.StopImmediatePropagation();
        }

        private void OnWorkshopPointerCancel(PointerCancelEvent evt)
        {
            if (!_isWorkshopDragging ||
                evt.pointerId != _workshopPointerId)
            {
                return;
            }

            CancelWorkshopDrag(true);
            evt.StopImmediatePropagation();
        }

        private void OnWorkshopWheel(WheelEvent evt)
        {
            if (!_isWorkshopDragging)
                return;

            RotateWorkshopDrag(evt.delta.y > 0f ? 5 : 1);
            evt.StopImmediatePropagation();
        }

        private void RotateWorkshopDrag(int steps)
        {
            _rotation =
                ((_rotation + steps) % 6 + 6) % 6;

            if (!_isWorkshopDragging)
            {
                Rebuild();
                return;
            }

            RebuildWorkshopFloatingPreview();
            RefreshWorkshopGhost(_workshopPointerPosition, true);
        }

        private void CreateWorkshopFloatingPreview()
        {
            ClearWorkshopDragVisuals();

            _workshopFloatingPreview = new VisualElement();
            _workshopFloatingPreview.name =
                "WorkshopFloatingRunePreview";

            _workshopFloatingPreview.pickingMode =
                PickingMode.Ignore;

            _workshopFloatingPreview.style.position =
                Position.Absolute;

            _workshopFloatingPreview.style.width = 170f;
            _workshopFloatingPreview.style.height = 105f;
            _workshopFloatingPreview.style.paddingLeft = 8f;
            _workshopFloatingPreview.style.paddingRight = 8f;
            _workshopFloatingPreview.style.paddingTop = 7f;
            _workshopFloatingPreview.style.paddingBottom = 7f;
            _workshopFloatingPreview.style.backgroundColor =
                new Color(0.025f, 0.045f, 0.07f, 0.95f);

            _workshopFloatingPreview.style.borderLeftWidth =
                _workshopFloatingPreview.style.borderRightWidth =
                _workshopFloatingPreview.style.borderTopWidth =
                _workshopFloatingPreview.style.borderBottomWidth = 2f;

            Color border =
                _workshopDragDefinition == null
                    ? Cyan
                    : _workshopDragDefinition.uiColor;

            _workshopFloatingPreview.style.borderLeftColor =
                _workshopFloatingPreview.style.borderRightColor =
                _workshopFloatingPreview.style.borderTopColor =
                _workshopFloatingPreview.style.borderBottomColor = border;

            _root.Add(_workshopFloatingPreview);
            RebuildWorkshopFloatingPreview();
        }

        private void RebuildWorkshopFloatingPreview()
        {
            if (_workshopFloatingPreview == null ||
                _workshopDragDefinition == null)
            {
                return;
            }

            _workshopFloatingPreview.Clear();

            Label title = LabelFor(
                _workshopDragDefinition.displayName +
                " · ROT " + _rotation,
                11,
                Text,
                true);

            title.style.unityTextAlign = TextAnchor.MiddleCenter;
            title.pickingMode = PickingMode.Ignore;
            _workshopFloatingPreview.Add(title);

            VisualElement shape =
                BuildWorkshopRuneShape(
                    _workshopDragDefinition,
                    _rotation,
                    14f);

            shape.style.alignSelf = Align.Center;
            shape.style.marginTop = 4f;
            shape.pickingMode = PickingMode.Ignore;
            _workshopFloatingPreview.Add(shape);

            Label hint = LabelFor(
                "Q / E or wheel to rotate",
                9,
                Muted,
                false);

            hint.style.unityTextAlign = TextAnchor.MiddleCenter;
            hint.pickingMode = PickingMode.Ignore;
            _workshopFloatingPreview.Add(hint);
            _workshopFloatingPreview.BringToFront();
        }

        private VisualElement BuildWorkshopRuneShape(
            SpellModifierDefinition definition,
            int rotation,
            float radius)
        {
            VisualElement container = new VisualElement();
            container.style.position = Position.Relative;

            HexCoord[] shape =
                definition.shape == null ||
                definition.shape.Length == 0
                    ? new[] { new HexCoord(0, 0) }
                    : definition.shape;

            float width = Mathf.Sqrt(3f) * radius;
            float rowStep = radius * 1.5f;
            List<Vector2> centers = new List<Vector2>();

            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            foreach (HexCoord offset in shape)
            {
                HexCoord rotated = HexCoord.Rotate(offset, rotation);

                Vector2 center = new Vector2(
                    (rotated.q + rotated.r * 0.5f) * width,
                    rotated.r * rowStep);

                centers.Add(center);
                minX = Mathf.Min(minX, center.x - width * 0.5f);
                minY = Mathf.Min(minY, center.y - radius);
                maxX = Mathf.Max(maxX, center.x + width * 0.5f);
                maxY = Mathf.Max(maxY, center.y + radius);
            }

            container.style.width = Mathf.Max(width, maxX - minX);
            container.style.height =
                Mathf.Max(radius * 2f, maxY - minY);

            foreach (Vector2 center in centers)
            {
                HexCellElement hex = new HexCellElement();
                hex.pickingMode = PickingMode.Ignore;
                hex.style.position = Position.Absolute;
                hex.style.left =
                    center.x - minX - width * 0.5f;
                hex.style.top =
                    center.y - minY - radius;
                hex.style.width = width;
                hex.style.height = radius * 2f;
                hex.FillColor = _workshopDragDefinition.uiColor;
                hex.BorderColor = Color.white;
                hex.BorderWidth = 1.5f;
                container.Add(hex);
            }

            return container;
        }

        private void MoveWorkshopFloatingPreview(Vector2 panelPosition)
        {
            _workshopPointerPosition = panelPosition;

            if (_workshopFloatingPreview == null)
                return;

            Vector2 local = _root.WorldToLocal(panelPosition);
            _workshopFloatingPreview.style.left = local.x + 18f;
            _workshopFloatingPreview.style.top = local.y + 22f;
            _workshopFloatingPreview.BringToFront();
        }

        private void RefreshWorkshopGhost(
            Vector2 panelPosition,
            bool force)
        {
            _workshopPointerPosition = panelPosition;

            VisualElement canvas = ResolveWorkshopBoardCanvas();
            HexCoord cell;

            if (canvas == null ||
                !TryFindWorkshopCell(canvas, panelPosition, out cell))
            {
                _workshopHoverCell = null;
                _workshopPlacementValid = false;
                _workshopPlacementReason =
                    "Release the Rune over the Spell Board.";

                ClearWorkshopGhost();

                if (_message != null)
                    _message.text = _workshopPlacementReason;

                return;
            }

            if (!force &&
                _workshopHoverCell.HasValue &&
                _workshopHoverCell.Value.Equals(cell))
            {
                return;
            }

            _workshopHoverCell = cell;
            ClearWorkshopGhost();

            string reason;
            _workshopPlacementValid =
                _workshopDragBoard.CanPlace(
                    _workshopDragDefinition.id,
                    cell,
                    _rotation,
                    _dragPiece,
                    out reason);

            _workshopPlacementReason = reason;

            Color fill =
                _workshopPlacementValid
                    ? new Color(0.2f, 1f, 0.45f, 0.42f)
                    : new Color(1f, 0.2f, 0.18f, 0.48f);

            Color border =
                _workshopPlacementValid
                    ? new Color(0.3f, 1f, 0.55f, 0.98f)
                    : new Color(1f, 0.28f, 0.22f, 0.98f);

            HexCoord[] shape =
                _workshopDragDefinition.shape == null ||
                _workshopDragDefinition.shape.Length == 0
                    ? new[] { new HexCoord(0, 0) }
                    : _workshopDragDefinition.shape;

            foreach (HexCoord offset in shape)
            {
                HexCoord occupied =
                    cell + HexCoord.Rotate(offset, _rotation);

                Vector2 center;

                if (!_boardCellCenters.TryGetValue(
                    occupied,
                    out center))
                {
                    continue;
                }

                HexCellElement ghost = new HexCellElement();
                ghost.pickingMode = PickingMode.Ignore;
                ghost.style.position = Position.Absolute;
                ghost.style.left = center.x - HexWidth * 0.5f;
                ghost.style.top = center.y - HexRadius;
                ghost.style.width = HexWidth;
                ghost.style.height = HexRadius * 2f;
                ghost.FillColor = fill;
                ghost.BorderColor = border;
                ghost.BorderWidth = 3f;

                canvas.Add(ghost);
                ghost.BringToFront();
                _workshopGhosts.Add(ghost);
            }

            UpdateWorkshopPreviewText(cell, reason);
        }

        private void UpdateWorkshopPreviewText(
            HexCoord cell,
            string reason)
        {
            if (_message == null)
                return;

            if (!_workshopPlacementValid)
            {
                _message.text = "INVALID · " + reason;
                return;
            }

            GameWorld world = GameWorld.Instance;

            SpellBoard previewBoard =
                _workshopDragBoard.CreatePlacementPreview(
                    _workshopDragDefinition.id,
                    cell,
                    _rotation,
                    _dragPiece);

            if (world == null || previewBoard == null)
            {
                _message.text = "VALID · " + reason;
                return;
            }

            CompiledSpell before = world.GetSpell(_slot);
            CompiledSpell after = SpellCompiler.Compile(
                previewBoard,
                world.Stats,
                world.Equipment);

            if (before == null || after == null)
            {
                _message.text = "VALID · " + reason;
                return;
            }

            _message.text =
                "PREVIEW · Damage " +
                before.damage.ToString("0.0") + " → " +
                after.damage.ToString("0.0") +
                " · Mana " +
                before.manaCost.ToString("0.0") + " → " +
                after.manaCost.ToString("0.0") +
                " · Cooldown " +
                before.cooldown.ToString("0.00") + "s → " +
                after.cooldown.ToString("0.00") + "s";
        }

        private VisualElement ResolveWorkshopBoardCanvas()
        {
            if (_workshopBoardCanvas != null &&
                _workshopBoardCanvas.panel != null)
            {
                return _workshopBoardCanvas;
            }

            VisualElement core =
                _root.Q<VisualElement>("SpellHex_0_0");

            _workshopBoardCanvas =
                core == null ? null : core.parent;

            return _workshopBoardCanvas;
        }

        private bool TryFindWorkshopCell(
            VisualElement canvas,
            Vector2 panelPosition,
            out HexCoord result)
        {
            result = default(HexCoord);
            Vector2 local = canvas.WorldToLocal(panelPosition);

            float closest = float.MaxValue;
            bool found = false;

            foreach (
                KeyValuePair<HexCoord, Vector2> pair
                in _boardCellCenters)
            {
                float distance =
                    (pair.Value - local).sqrMagnitude;

                if (distance >= closest)
                    continue;

                closest = distance;
                result = pair.Key;
                found = true;
            }

            float threshold = HexRadius * 1.4f;

            return found &&
                   closest <= threshold * threshold;
        }

        private void ClearWorkshopGhost()
        {
            foreach (VisualElement ghost in _workshopGhosts)
            {
                if (ghost != null)
                    ghost.RemoveFromHierarchy();
            }

            _workshopGhosts.Clear();
        }

        private void ClearWorkshopDragVisuals()
        {
            ClearWorkshopGhost();

            if (_workshopFloatingPreview != null)
                _workshopFloatingPreview.RemoveFromHierarchy();

            _workshopFloatingPreview = null;
            _workshopBoardCanvas = null;
        }

        private void ReleaseWorkshopPointer()
        {
            if (_workshopPointerId >= 0 &&
                _root != null &&
                _root.HasPointerCapture(_workshopPointerId))
            {
                _root.ReleasePointer(_workshopPointerId);
            }

            _workshopPointerId = -1;
        }

        private void CancelWorkshopDrag(bool showMessage)
        {
            if (_isWorkshopDragging && showMessage)
                SetMessage("Rune drag cancelled.");

            ReleaseWorkshopPointer();
            ClearWorkshopDragVisuals();

            _isWorkshopDragging = false;
            _dragRune = null;
            _dragPiece = null;
            _selectedRune = null;
            _workshopDragBoard = null;
            _workshopDragDefinition = null;
            _workshopHoverCell = null;
            _workshopPlacementValid = false;
            _workshopPlacementReason = string.Empty;

            if (!showMessage && _message != null)
                _message.text = _persistentMessage;
        }
    }
}
