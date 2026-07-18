using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneEngine
{
    [DefaultExecutionOrder(-40)]
    public sealed class V21ProductUI : MonoBehaviour
    {
        private enum ScreenKind { None, Workshop, Links, Inventory, Training }
        public static V21ProductUI Instance { get; private set; }
        public bool IsOpen { get { return _screen != ScreenKind.None; } }
        public bool WorkshopOpen { get { return _screen == ScreenKind.Workshop; } }
        public bool LinksOpen { get { return _screen == ScreenKind.Links; } }
        public bool InventoryOpen { get { return _screen == ScreenKind.Inventory; } }

        private UIDocument _document;
        private VisualElement _root;
        private VisualElement _content;
        private Label _title;
        private Label _message;
        private ScreenKind _screen;
        private SpellSlot _slot;
        private string _selectedRune;
        private int _rotation;
        private HexCoord? _selectedCell;
        private string _dragRune;
        private PlacedModifier _dragPiece;
        private int _linkSource;
        private int _linkDestination = 1;
        private SpellLinkCondition _linkCondition = SpellLinkCondition.OnHit;
        private bool _inventoryGrid = true;
        private string _inventorySearch = string.Empty;
        private int _inventorySort;
        private int _inventoryRarity = -1;
        private int _inventorySlot = -1;
        private bool _inventoryJunkOnly;
        private ItemInstance _dragItem;
        private string _pendingDestructiveItem;
        private string _persistentMessage = string.Empty;
        private readonly List<VisualElement> _previewActors = new List<VisualElement>();
        private CompiledSpell _previewSpell;
        private int _ignoreHexClickFrame = -100;
        private readonly Dictionary<HexCoord, Vector2> _boardCellCenters = new Dictionary<HexCoord, Vector2>();
        private bool _isWorkshopDragging;

        private static readonly Color Panel = new Color(0.025f, 0.04f, 0.07f, 0.98f);
        private static readonly Color Card = new Color(0.055f, 0.085f, 0.13f, 0.98f);
        private static readonly Color Cyan = new Color(0.08f, 0.75f, 0.9f);
        private static readonly Color Text = new Color(0.9f, 0.95f, 1f);
        private static readonly Color Muted = new Color(0.58f, 0.68f, 0.78f);

        private void Awake()
        {
            Instance = this;
            _document = RuntimeUIFactory.CreateDocument(transform, "Arcane Engine Product Screens", 80);
            _root = _document.rootVisualElement;
            _root.style.position = Position.Absolute;
            _root.style.left = _root.style.right = _root.style.top = _root.style.bottom = 0f;
            _root.style.backgroundColor = new Color(0f, 0.015f, 0.025f, 0.86f);
            _root.style.paddingLeft = _root.style.paddingRight = 42f;
            _root.style.paddingTop = _root.style.paddingBottom = 28f;
            _root.style.display = DisplayStyle.None;
            _root.RegisterCallback<PointerUpEvent>(HandleWorkshopDrop, TrickleDown.TrickleDown);
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Update()
        {
            GameWorld world = GameWorld.Instance;
            if (world == null) return;
            ControlSettings controls = ProfileManager.Current.controls;
            if (world.RunActive && ArcaneInput.GetKeyDown(controls.workshop)) Toggle(ScreenKind.Workshop);
            else if (world.RunActive && ArcaneInput.GetKeyDown(controls.spellLinks)) Toggle(ScreenKind.Links);
            else if (world.RunActive && ArcaneInput.GetKeyDown(controls.inventory)) Toggle(ScreenKind.Inventory);
            else if (world.TrainingMode && ArcaneInput.GetKeyDown(KeyCode.F2)) Toggle(ScreenKind.Training);
            if (IsOpen && (ArcaneInput.GetKeyDown(KeyCode.Escape) || ArcaneInput.GamepadCancelDown)) Close();
            if (_screen == ScreenKind.Workshop && world.CanEditSpells)
            {
                if (ArcaneInput.GetKeyDown(KeyCode.Q)) { _rotation = (_rotation + 5) % 6; Rebuild(); }
                if (ArcaneInput.GetKeyDown(KeyCode.E)) { _rotation = (_rotation + 1) % 6; Rebuild(); }
            }
            AnimateSpellPreview();
        }

        public void OpenWorkshop() { Open(ScreenKind.Workshop); }
        public void OpenLinks() { Open(ScreenKind.Links); }
        public void OpenInventory() { Open(ScreenKind.Inventory); }
        public void OpenTraining() { Open(ScreenKind.Training); }

        public void Close()
        {
            _screen = ScreenKind.None;
            _root.style.display = DisplayStyle.None;
            _dragRune = null;
            _dragPiece = null;
            _dragItem = null;
            if (GameWorld.Instance != null && GameWorld.Instance.RunActive) Time.timeScale = 1f;
            ArcaneInput.SuppressPointerTransitions();
        }

        private void Toggle(ScreenKind screen) { if (_screen == screen) Close(); else Open(screen); }

        private void Open(ScreenKind screen)
        {
            _screen = screen;
            _root.style.display = DisplayStyle.Flex;
            if (GameWorld.Instance != null && GameWorld.Instance.RunActive) Time.timeScale = 0f;
            Rebuild();
            ArcaneInput.SuppressPointerTransitions();
        }

        private void Rebuild()
        {
            _root.Clear();
            VisualElement frame = new VisualElement();
            frame.style.flexGrow = 1f;
            frame.style.backgroundColor = Panel;
            frame.style.borderLeftWidth = frame.style.borderRightWidth = frame.style.borderTopWidth = frame.style.borderBottomWidth = 2f;
            frame.style.borderLeftColor = frame.style.borderRightColor = frame.style.borderTopColor = frame.style.borderBottomColor = Cyan;
            frame.style.paddingLeft = frame.style.paddingRight = 22f;
            frame.style.paddingTop = frame.style.paddingBottom = 18f;
            _root.Add(frame);

            VisualElement header = Row();
            _title = LabelFor(TitleFor(_screen), 25, Text, true);
            _title.style.flexGrow = 1f;
            header.Add(_title);
            Button close = ActionButton("CLOSE  [ESC / B]", Close);
            close.style.width = 190f;
            header.Add(close);
            frame.Add(header);
            _message = LabelFor(string.Empty, 12, Muted, false);
            _message.text = _persistentMessage;
            _message.style.height = 24f;
            frame.Add(_message);
            _content = new VisualElement();
            _content.style.flexGrow = 1f;
            frame.Add(_content);

            if (_screen == ScreenKind.Workshop) BuildWorkshop();
            else if (_screen == ScreenKind.Links) BuildLinks();
            else if (_screen == ScreenKind.Inventory) BuildInventory();
            else if (_screen == ScreenKind.Training) BuildTraining();
        }

        private void BuildWorkshop()
        {
            GameWorld world = GameWorld.Instance;
            if (world.PendingSpellUpgrade)
            {
                VisualElement upgrade = CardPanel(0f);
                upgrade.style.flexGrow = 0f;
                upgrade.style.width = Length.Percent(100f);
                upgrade.Add(LabelFor("SPELL UPGRADE REWARD · CHOOSE ONE EQUIPPED SPELL", 15, Cyan, true));
                VisualElement upgradeChoices = Row();
                for (int i = 0; i < 3; i++)
                {
                    int upgradeSlot = i;
                    SpellBoard candidate = world.GetBoard((SpellSlot)i);
                    string label = candidate == null ? "SLOT " + (i + 1) + " · EMPTY" : "SLOT " + (i + 1) + " · LEVEL " + candidate.spellLevel + " → " + Mathf.Min(5, candidate.spellLevel + 1);
                    Button choice = ActionButton(label, () =>
                    {
                        string upgradeMessage;
                        if (world.ApplySpellUpgrade((SpellSlot)upgradeSlot, out upgradeMessage)) Changed(upgradeMessage); else SetMessage(upgradeMessage);
                    });
                    choice.SetEnabled(candidate != null && candidate.spellLevel < 5 && world.CanEditSpells);
                    choice.style.flexGrow = 1f;
                    upgradeChoices.Add(choice);
                }
                upgrade.Add(upgradeChoices);
                _content.Add(upgrade);
            }
            VisualElement tabs = Row();
            for (int i = 0; i < 3; i++)
            {
                int captured = i;
                CompiledSpell spell = world.GetSpell((SpellSlot)i);
                Button tab = ActionButton((i + 1) + "  " + (spell == null ? "EMPTY" : spell.displayName.ToUpperInvariant()), () =>
                {
                    _slot = (SpellSlot)captured; _selectedCell = null; Rebuild();
                });
                tab.style.flexGrow = 1f;
                tab.style.backgroundColor = _slot == (SpellSlot)i ? Cyan : Card;
                tabs.Add(tab);
            }
            _content.Add(tabs);

            SpellBoard board = world.GetBoard(_slot);
            if (board == null) { _content.Add(LabelFor("This slot has no equipped Spell Core.", 18, Muted, false)); return; }
            VisualElement body = Row();
            body.style.flexGrow = 1f;
            body.style.marginTop = 12f;
            _content.Add(body);

            VisualElement palette = CardPanel(330f);
            palette.Add(LabelFor("SUPPORT RUNES", 16, Text, true));
            palette.Add(LabelFor("Drag a Rune onto a hex, or select it and click a hex. Q/E or the buttons rotate it. Right-click a placed Rune to rotate; Shift + right-click removes it.", 11, Muted, false));
            ScrollView runeScroll = new ScrollView();
            runeScroll.style.flexGrow = 1f;
            foreach (KeyValuePair<string, int> pair in world.ModifierInventory.Where(value => value.Value > 0).OrderBy(value => DemoCatalog.GetModifier(value.Key).displayName))
            {
                SpellModifierDefinition rune = DemoCatalog.GetModifier(pair.Key);
                if (rune == null) continue;
                string id = rune.id;
                Button button = ActionButton(rune.FullDisplayName.Replace("\n", " — ") + "  ×" + pair.Value + "\n" +
                    "Capacity " + rune.capacityCost + " · " + rune.shortDescription, () => { _selectedRune = id; _dragRune = id; Rebuild(); });
                button.style.height = 66f;
                button.style.whiteSpace = WhiteSpace.Normal;
                button.style.backgroundColor = _selectedRune == id ? rune.uiColor : Card;
                button.RegisterCallback<PointerDownEvent>(_ => _dragRune = id);
                runeScroll.Add(button);
            }
            palette.Add(runeScroll);
            body.Add(palette);

            VisualElement boardPanel = CardPanel(620f);
            VisualElement boardHeader = Row();
            boardHeader.Add(ActionButton("↶ ROTATE LEFT", () => { _rotation = (_rotation + 5) % 6; Rebuild(); }));
            boardHeader.Add(ActionButton("ROTATION " + _rotation, () => { }));
            boardHeader.Add(ActionButton("ROTATE RIGHT ↷", () => { _rotation = (_rotation + 1) % 6; Rebuild(); }));
            boardHeader.Add(ActionButton("UNDO", () => { if (board.Undo()) Changed("Undo applied."); }));
            boardHeader.Add(ActionButton("REDO", () => { if (board.Redo()) Changed("Redo applied."); }));
            boardPanel.Add(boardHeader);
            VisualElement boardCanvas = new VisualElement();
            boardCanvas.style.width = 570f;
            boardCanvas.style.height = 570f;
            boardCanvas.style.position = Position.Relative;
            boardCanvas.style.alignSelf = Align.Center;
            foreach (HexCoord cell in board.AllCells()) boardCanvas.Add(CreateHexCell(world, board, cell));
            boardPanel.Add(boardCanvas);
            body.Add(boardPanel);

            VisualElement details = CardPanel(390f);
            CompiledSpell compiled = world.GetSpell(_slot);
            details.Add(LabelFor("COMPILED SPELL", 16, Text, true));
            details.Add(LabelFor(compiled == null ? "Unavailable" : compiled.CompactSummary(), 14, Text, true));
            details.Add(LabelFor("LEVEL " + board.spellLevel + " · CAPACITY " + board.UsedCapacity() + " / " + board.Capacity +
                " · ACTIVE RUNES " + board.GetActivePlacements().Count + " / " + board.placed.Count, 12, Cyan, true));
            if (compiled != null)
            {
                details.Add(BuildSpellPreview(compiled));
                SpellCoreDefinition original = DemoCatalog.GetCore(compiled.coreId);
                if (original != null)
                    details.Add(LabelFor("BEFORE → CURRENT\nDamage " + original.baseDamage.ToString("0.0") + " → " + compiled.damage.ToString("0.0") +
                        " · Mana " + original.manaCost.ToString("0.0") + " → " + compiled.manaCost.ToString("0.0") +
                        " · Cooldown " + original.cooldown.ToString("0.00") + "s → " + compiled.cooldown.ToString("0.00") + "s\n" +
                        "Projectiles 1 → " + compiled.projectileCount + " · Area " + original.radius.ToString("0.0") + "m → " +
                        Mathf.Max(compiled.radius, compiled.explosionRadius).ToString("0.0") + "m · Duration " + original.lifetime.ToString("0.0") +
                        "s → " + Mathf.Max(compiled.lifetime, compiled.zoneDuration).ToString("0.0") + "s", 11, Cyan, false));
                ScrollView layers = new ScrollView();
                layers.style.flexGrow = 1f;
                foreach (string layer in compiled.executionLayers) layers.Add(LabelFor("• " + layer, 11, Muted, false));
                foreach (string warning in compiled.warnings) layers.Add(LabelFor("⚠ " + warning, 11, new Color(1f, 0.7f, 0.2f), false));
                details.Add(layers);
            }
            if (!world.CanEditSpells) details.Add(LabelFor("LOCKED · " + world.SpellEditLockReason, 13, new Color(1f, 0.35f, 0.2f), true));
            body.Add(details);
        }

        private const float HexRadius = 33.333f;
        private static readonly float HexWidth = Mathf.Sqrt(3f) * HexRadius;
        private const float HexRowStep = HexRadius * 1.5f;

        private VisualElement CreateHexCell(GameWorld world, SpellBoard board, HexCoord cell)
        {
            float radius = HexRadius;
            float originX = 285f;
            float originY = 285f;
            float centerX = originX + (cell.q + cell.r * 0.5f) * HexWidth;
            float centerY = originY + cell.r * HexRowStep;
            PlacedModifier piece = board.PieceAt(cell);
            bool core = cell.Equals(new HexCoord(0, 0));
            SpellModifierDefinition definition = piece == null ? null : DemoCatalog.GetModifier(piece.modifierId);
            string text = core ? "CORE" : definition == null ? (board.IsCellUnlocked(cell) ? "·" : "×") : definition.displayName.Substring(0, Mathf.Min(4, definition.displayName.Length)).ToUpperInvariant();
            Color cellColor = core ? DemoCatalog.GetCore(board.coreId).color :
                definition != null ? definition.uiColor : board.IsCellUnlocked(cell) ? new Color(0.07f, 0.12f, 0.17f) : new Color(0.025f, 0.035f, 0.05f);
            HexCellElement cellEl = new HexCellElement();
            cellEl.name = "SpellHex_" + cell.q + "_" + cell.r;
            cellEl.userData = cell;
            cellEl.style.position = Position.Absolute;
            cellEl.style.left = centerX - HexWidth * 0.5f;
            cellEl.style.top = centerY - radius;
            cellEl.style.width = HexWidth;
            cellEl.style.height = radius * 2f;
            cellEl.FillColor = cellColor;
            cellEl.BorderColor = new Color(0.35f, 0.55f, 0.85f, 0.8f);
            cellEl.BorderWidth = 2f;
            Label label = new Label(text);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.color = Text;
            label.style.fontSize = 11;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.pickingMode = PickingMode.Ignore;
            cellEl.Add(label);
            _boardCellCenters[cell] = new Vector2(centerX, centerY);
            cellEl.RegisterCallback<PointerDownEvent>(evt =>
            {
                _selectedCell = cell;
                if (piece != null && evt.button == 0)
                {
                    _dragPiece = piece;
                    _isWorkshopDragging = true;
                }
                if (evt.button == 1 && piece != null)
                {
                    _ignoreHexClickFrame = Time.frameCount;
                    evt.StopPropagation();
                    if (!world.CanEditSpells) { SetMessage(world.SpellEditLockReason); return; }
                    string removedId;
                    if (board.RemoveAt(cell, out removedId)) { world.ReturnModifier(removedId); Changed("Support Rune returned to the Run inventory."); }
                    _dragRune = null;
                    _selectedRune = null;
                    _selectedCell = null;
                    _isWorkshopDragging = false;
                }
            });
            cellEl.RegisterCallback<PointerUpEvent>(_ =>
            {
                if (_isWorkshopDragging && _dragPiece != null && piece != _dragPiece)
                {
                    _ignoreHexClickFrame = Time.frameCount;
                    string reason;
                    if (board.TryMove(_dragPiece, cell, _rotation, out reason)) Changed(reason); else SetMessage(reason);
                }
                else if (!string.IsNullOrEmpty(_dragRune))
                {
                    _ignoreHexClickFrame = Time.frameCount; 
                    PlaceRune(world, board, _dragRune, cell);
                }
                _dragPiece = null;
                _dragRune = null;
                _isWorkshopDragging = false;
            });
            return cellEl;
        }

        private void HandleWorkshopDrop(PointerUpEvent evt)
        {
            if (_screen != ScreenKind.Workshop || (string.IsNullOrEmpty(_dragRune) && _dragPiece == null)) return;
            VisualElement picked = _root.panel == null ? null : _root.panel.Pick(evt.position);
            while (picked != null && !(picked.userData is HexCoord)) picked = picked.parent;
            if (picked == null)
            {
                _dragRune = null;
                _dragPiece = null;
                SetMessage("Drop cancelled: release the Rune over an unlocked hex.");
                return;
            }
            HexCoord cell = (HexCoord)picked.userData;
            GameWorld world = GameWorld.Instance;
            SpellBoard board = world == null ? null : world.GetBoard(_slot);
            if (board == null) return;
            _ignoreHexClickFrame = Time.frameCount;
            if (_dragPiece != null) MovePiece(world, board, _dragPiece, cell);
            else PlaceRune(world, board, _dragRune, cell);
            _dragRune = null;
            _dragPiece = null;
            evt.StopPropagation();
        }

        private void OnHexClicked(GameWorld world, SpellBoard board, HexCoord cell)
        {
            if (_ignoreHexClickFrame == Time.frameCount) return;
            _selectedCell = cell;
            if (!world.CanEditSpells) { SetMessage(world.SpellEditLockReason); return; }
            if (!string.IsNullOrEmpty(_selectedRune)) PlaceRune(world, board, _selectedRune, cell);
            else
            {
                PlacedModifier piece = board.PieceAt(cell);
                if (piece != null)
                {
                    string reason;
                    if (board.RotateAt(cell, 1, out reason)) Changed(reason); else SetMessage(reason);
                }
            }
        }

        private void PlaceRune(GameWorld world, SpellBoard board, string id, HexCoord cell)
        {
            if (!world.CanEditSpells) { SetMessage(world.SpellEditLockReason); return; }
            if (!world.ConsumeModifier(id)) { SetMessage("No spare copy of that Support Rune remains."); return; }
            string reason;
            if (!board.TryPlace(id, cell, _rotation, out reason)) { world.ReturnModifier(id); SetMessage(reason); return; }
            Changed(reason);
        }

        private void MovePiece(GameWorld world, SpellBoard board, PlacedModifier piece, HexCoord cell)
        {
            if (!world.CanEditSpells) { SetMessage(world.SpellEditLockReason); return; }
            string reason;
            if (board.TryMove(piece, cell, _rotation, out reason)) Changed(reason); else SetMessage(reason);
        }

        private void RemoveAt(GameWorld world, SpellBoard board, HexCoord cell)
        {
            if (!world.CanEditSpells) { SetMessage(world.SpellEditLockReason); return; }
            string id;
            if (board.RemoveAt(cell, out id)) { world.ReturnModifier(id); Changed("Support Rune returned to the Run inventory."); }
        }

        private VisualElement BuildSpellPreview(CompiledSpell spell)
        {
            _previewSpell = spell;
            _previewActors.Clear();
            VisualElement preview = new VisualElement();
            preview.style.height = 210f;
            preview.style.marginTop = preview.style.marginBottom = 10f;
            preview.style.backgroundColor = new Color(spell.primaryColor.r * 0.15f, spell.primaryColor.g * 0.15f, spell.primaryColor.b * 0.15f, 1f);
            preview.style.borderLeftWidth = preview.style.borderRightWidth = preview.style.borderTopWidth = preview.style.borderBottomWidth = 1f;
            preview.style.borderLeftColor = preview.style.borderRightColor = preview.style.borderTopColor = preview.style.borderBottomColor = spell.primaryColor;
            Label delivery = LabelFor("LIVE PREVIEW · " + spell.delivery.ToString().ToUpperInvariant(), 14, spell.primaryColor, true);
            delivery.style.unityTextAlign = TextAnchor.MiddleCenter;
            preview.Add(delivery);
            VisualElement arena = new VisualElement();
            arena.style.height = 118f;
            arena.style.position = Position.Relative;
            arena.style.overflow = Overflow.Hidden;
            arena.style.backgroundColor = new Color(0.01f, 0.02f, 0.035f, 0.9f);
            int actorCount = spell.delivery == SpellDelivery.Projectile ? Mathf.Clamp(spell.projectileCount, 1, 12) :
                spell.delivery == SpellDelivery.Summon ? Mathf.Clamp(spell.summonCount, 1, 8) : 1;
            for (int i = 0; i < actorCount; i++)
            {
                VisualElement actor = new VisualElement();
                actor.userData = i;
                actor.style.position = Position.Absolute;
                actor.style.width = actor.style.height = Mathf.Clamp(9f + spell.size * 8f, 10f, 30f);
                actor.style.borderTopLeftRadius = actor.style.borderTopRightRadius = 30f;
                actor.style.borderBottomLeftRadius = actor.style.borderBottomRightRadius = 30f;
                actor.style.backgroundColor = spell.primaryColor;
                arena.Add(actor);
                _previewActors.Add(actor);
            }
            preview.Add(arena);
            preview.Add(LabelFor(spell.projectileCount + " projectiles · " + spell.chainTargets + " chains · " + spell.repeatCount +
                " repeats · " + spell.explosionRadius.ToString("0.0") + "m area\n" +
                (spell.homingStrength > 0f ? "Homing · " : string.Empty) + (spell.returnsToCaster ? "Returning · " : string.Empty) +
                (spell.orbitCaster ? "Orbiting · " : string.Empty) + (spell.zoneDuration > 0f ? "Persistent · " : string.Empty) +
                (spell.triggers.Count > 0 ? spell.triggers.Count + " internal trigger(s)" : "No internal triggers"), 11, Text, false));
            return preview;
        }

        private void AnimateSpellPreview()
        {
            if (_screen != ScreenKind.Workshop || _previewSpell == null || _previewActors.Count == 0) return;
            float phase = Mathf.Repeat(Time.unscaledTime * 0.42f, 1f);
            for (int i = 0; i < _previewActors.Count; i++)
            {
                VisualElement actor = _previewActors[i];
                float centered = i - (_previewActors.Count - 1) * 0.5f;
                float x = 18f + phase * 310f;
                float y = 50f + centered * Mathf.Clamp(_previewSpell.spreadDegrees * 0.16f + 7f, 7f, 18f) * phase;
                if (_previewSpell.delivery == SpellDelivery.Nova || _previewSpell.delivery == SpellDelivery.Defensive)
                {
                    float size = 18f + phase * 92f;
                    actor.style.width = actor.style.height = size;
                    actor.style.left = 174f - size * 0.5f;
                    actor.style.top = 58f - size * 0.5f;
                    actor.style.opacity = 1f - phase * 0.8f;
                }
                else if (_previewSpell.delivery == SpellDelivery.Beam || _previewSpell.delivery == SpellDelivery.Hitscan)
                {
                    actor.style.width = 300f;
                    actor.style.height = Mathf.Clamp(5f + _previewSpell.size * 3f, 5f, 16f);
                    actor.style.left = 22f;
                    actor.style.top = 54f;
                    actor.style.opacity = 0.55f + Mathf.Sin(Time.unscaledTime * 8f) * 0.25f;
                }
                else if (_previewSpell.delivery == SpellDelivery.Meteor)
                {
                    actor.style.left = 170f;
                    actor.style.top = Mathf.Lerp(-8f, 82f, phase);
                    actor.style.opacity = 1f;
                }
                else if (_previewSpell.delivery == SpellDelivery.Summon)
                {
                    float angle = Time.unscaledTime * 1.8f + i / (float)_previewActors.Count * Mathf.PI * 2f;
                    actor.style.left = 170f + Mathf.Cos(angle) * 72f;
                    actor.style.top = 50f + Mathf.Sin(angle) * 38f;
                    actor.style.opacity = 1f;
                }
                else
                {
                    if (_previewSpell.orbitCaster)
                    {
                        float angle = Time.unscaledTime * 2f + i / (float)_previewActors.Count * Mathf.PI * 2f;
                        x = 170f + Mathf.Cos(angle) * 75f;
                        y = 50f + Mathf.Sin(angle) * 38f;
                    }
                    else if (_previewSpell.returnsToCaster && phase > 0.5f) x = Mathf.Lerp(328f, 18f, (phase - 0.5f) * 2f);
                    if (_previewSpell.arcAmount > 0f) y += Mathf.Sin(phase * Mathf.PI) * -34f;
                    actor.style.left = x;
                    actor.style.top = y;
                    actor.style.opacity = 1f;
                }
            }
        }

        private void BuildLinks()
        {
            GameWorld world = GameWorld.Instance;
            SpellLinkSystem links = world.SpellLinks;
            if (links.PendingChoices.Count > 0)
            {
                VisualElement reward = CardPanel(0f);
                reward.style.flexGrow = 0f;
                reward.style.width = Length.Percent(100f);
                reward.Add(LabelFor("SPELL LINK REWARD · CHOOSE ONE EVENT", 15, Cyan, true));
                VisualElement choices = Row();
                foreach (SpellLinkCondition condition in links.PendingChoices.ToArray())
                {
                    SpellLinkCondition pending = condition;
                    Button choice = ActionButton(SpellLinkRules.DisplayName(condition) + "\n" + SpellLinkRules.Description(condition), () =>
                    {
                        string pendingMessage;
                        if (world.SelectSpellLinkCondition(pending, out pendingMessage)) { _linkCondition = pending; Changed(pendingMessage); }
                        else SetMessage(pendingMessage);
                    });
                    choice.style.flexGrow = 1f;
                    choices.Add(choice);
                }
                reward.Add(choices);
                _content.Add(reward);
            }
            if (links.PendingCondition.HasValue) _linkCondition = links.PendingCondition.Value;
            VisualElement body = Row();
            body.style.flexGrow = 1f;
            _content.Add(body);

            VisualElement builder = CardPanel(520f);
            builder.Add(LabelFor("CREATE SPELL LINK", 17, Text, true));
            builder.Add(LabelFor("Links are separate from Support Runes. Safe cycles are allowed because trigger Energy and generation limits guarantee termination.", 12, Muted, false));
            builder.Add(LabelFor("SOURCE SPELL", 12, Cyan, true));
            builder.Add(SpellSlotPicker(true));
            builder.Add(LabelFor("DESTINATION SPELL", 12, Cyan, true));
            builder.Add(SpellSlotPicker(false));
            builder.Add(LabelFor("TRIGGER EVENT", 12, Cyan, true));
            ScrollView conditions = new ScrollView();
            conditions.style.height = 260f;
            foreach (SpellLinkCondition condition in links.OwnedConditions)
            {
                SpellLinkCondition captured = condition;
                string compatibility;
                bool compatible = SpellLinkRules.IsCompatible(world.GetSpell((SpellSlot)_linkSource), world.GetSpell((SpellSlot)_linkDestination), condition, out compatibility);
                Button button = ActionButton(SpellLinkRules.DisplayName(condition) + "\n" + SpellLinkRules.Description(condition) +
                    (compatible ? string.Empty : "\nUNAVAILABLE · " + compatibility), () =>
                {
                    _linkCondition = captured; Rebuild();
                });
                button.style.height = 58f;
                button.style.whiteSpace = WhiteSpace.Normal;
                button.style.backgroundColor = _linkCondition == condition ? Cyan : Card;
                button.SetEnabled(compatible);
                conditions.Add(button);
            }
            builder.Add(conditions);
            Button create = ActionButton("CREATE LINK", () =>
            {
                string message;
                if (world.CreateSpellLink(_linkSource, _linkDestination, _linkCondition, out message)) Changed(message);
                else SetMessage(message);
            });
            create.style.height = 46f;
            builder.Add(create);
            body.Add(builder);

            VisualElement current = CardPanel(720f);
            current.Add(LabelFor("ACTIVE LINKS  " + links.Links.Count + " / " + links.Slots, 17, Text, true));
            foreach (SpellLinkSave link in links.Links)
            {
                SpellLinkSave captured = link;
                VisualElement row = Row();
                CompiledSpell source = world.GetSpell((SpellSlot)link.sourceSlot);
                CompiledSpell destination = world.GetSpell((SpellSlot)link.destinationSlot);
                row.Add(LabelFor((source == null ? "Empty" : source.displayName) + "  →  " +
                    SpellLinkRules.DisplayName(link.condition) + "  →  " + (destination == null ? "Empty" : destination.displayName) +
                    "\nCooldown " + SpellLinkRules.Cooldown(link.condition).ToString("0.00") + "s · inherited power " +
                    (SpellLinkRules.TriggerPower(link.condition) * 100f).ToString("0") + "% · current " +
                    links.CooldownRemaining(link).ToString("0.0") + "s", 13, Text, false));
                Button remove = ActionButton("REMOVE", () =>
                {
                    string message;
                    if (links.Remove(captured.instanceId, out message)) Changed(message); else SetMessage(message);
                });
                remove.style.width = 130f;
                row.Add(remove);
                current.Add(row);
            }
            if (links.Links.Count == 0) current.Add(LabelFor("No active Links. Obtain Link events from room rewards, then connect two equipped spells.", 14, Muted, false));
            body.Add(current);
        }

        private VisualElement SpellSlotPicker(bool source)
        {
            VisualElement row = Row();
            for (int i = 0; i < 3; i++)
            {
                int captured = i;
                CompiledSpell spell = GameWorld.Instance.GetSpell((SpellSlot)i);
                bool selected = (source ? _linkSource : _linkDestination) == i;
                Button button = ActionButton((i + 1) + " · " + (spell == null ? "EMPTY" : spell.displayName), () =>
                {
                    if (source) _linkSource = captured; else _linkDestination = captured;
                    Rebuild();
                });
                button.style.flexGrow = 1f;
                button.style.backgroundColor = selected ? Cyan : Card;
                row.Add(button);
            }
            return row;
        }

        private void BuildInventory()
        {
            GameWorld world = GameWorld.Instance;
            EquipmentInventory inventory = world.Equipment;
            VisualElement toolbar = Row();
            toolbar.Add(ActionButton(_inventoryGrid ? "VIEW · GRID" : "VIEW · LIST", () => { _inventoryGrid = !_inventoryGrid; Rebuild(); }));
            toolbar.Add(ActionButton("SORT · " + (_inventorySort == 0 ? "RARITY" : _inventorySort == 1 ? "LEVEL" : "NAME"), () => { _inventorySort = (_inventorySort + 1) % 3; Rebuild(); }));
            toolbar.Add(ActionButton("RARITY · " + (_inventoryRarity < 0 ? "ALL" : ((ItemRarity)_inventoryRarity).ToString().ToUpperInvariant()), () =>
            { _inventoryRarity++; if (_inventoryRarity >= Enum.GetValues(typeof(ItemRarity)).Length) _inventoryRarity = -1; Rebuild(); }));
            toolbar.Add(ActionButton("SLOT · " + (_inventorySlot < 0 ? "ALL" : ((EquipmentSlot)_inventorySlot).ToString().ToUpperInvariant()), () =>
            { _inventorySlot++; if (_inventorySlot >= Enum.GetValues(typeof(EquipmentSlot)).Length) _inventorySlot = -1; Rebuild(); }));
            toolbar.Add(ActionButton(_inventoryJunkOnly ? "SHOW · JUNK" : "SHOW · ALL", () => { _inventoryJunkOnly = !_inventoryJunkOnly; Rebuild(); }));
            TextField search = new TextField();
            search.value = _inventorySearch;
            search.style.flexGrow = 1f;
            search.RegisterValueChangedCallback(evt => { _inventorySearch = evt.newValue ?? string.Empty; Rebuild(); });
            toolbar.Add(search);
            toolbar.Add(LabelFor(inventory.EquipmentLocked ? "LOADOUT LOCKED DURING EXPEDITION" : "HOME BASE · EQUIPMENT EDITING ENABLED",
                12, inventory.EquipmentLocked ? new Color(1f, 0.45f, 0.2f) : Cyan, true));
            _content.Add(toolbar);

            VisualElement body = Row();
            body.style.flexGrow = 1f;
            body.style.marginTop = 10f;
            _content.Add(body);
            VisualElement equipped = CardPanel(390f);
            equipped.Add(LabelFor("EQUIPPED LOADOUT", 17, Text, true));
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                ItemInstance item;
                inventory.equipped.TryGetValue(slot, out item);
                VisualElement row = Row();
                row.Add(LabelFor(slot + "\n" + (item == null ? "Empty" : item.DisplayName), 12, item == null ? Muted : Text, item != null));
                EquipmentSlot dropSlot = slot;
                row.RegisterCallback<PointerUpEvent>(_ =>
                {
                    if (_dragItem == null || inventory.EquipmentLocked || _dragItem.Definition == null || _dragItem.Definition.slot != dropSlot) return;
                    string dragMessage;
                    if (inventory.Equip(_dragItem, out dragMessage)) { inventory.SaveSanctuaryEquipment(); Changed(dragMessage); }
                    else SetMessage(dragMessage);
                    _dragItem = null;
                });
                if (item != null && !inventory.EquipmentLocked)
                {
                    EquipmentSlot captured = slot;
                    Button remove = ActionButton("UNEQUIP", () =>
                    {
                        string message;
                        if (inventory.Unequip(captured, out message)) { inventory.SaveSanctuaryEquipment(); Changed(message); }
                        else SetMessage(message);
                    });
                    remove.style.width = 105f;
                    row.Add(remove);
                }
                equipped.Add(row);
            }
            body.Add(equipped);

            VisualElement stash = CardPanel(600f);
            stash.Add(LabelFor("PERMANENT STASH", 17, Text, true));
            ScrollView stashScroll = new ScrollView();
            stashScroll.style.flexGrow = 1f;
            AddItems(stashScroll, inventory.backpack.Where(MatchesInventory), false);
            stash.Add(stashScroll);
            body.Add(stash);

            VisualElement runBag = CardPanel(600f);
            runBag.Add(LabelFor("UNSECURED RUN BAG", 17, Text, true));
            runBag.Add(LabelFor("These items cannot be equipped during the expedition. Extract to keep them, or salvage them now for run resources.", 11, Muted, false));
            ScrollView runScroll = new ScrollView();
            runScroll.style.flexGrow = 1f;
            AddItems(runScroll, inventory.runBag.Where(MatchesInventory), true);
            runBag.Add(runScroll);
            body.Add(runBag);
        }

        private void AddItems(VisualElement parent, IEnumerable<ItemInstance> source, bool unsecured)
        {
            IEnumerable<ItemInstance> ordered = _inventorySort == 1 ? source.OrderByDescending(value => value.itemLevel).ThenByDescending(value => value.rarity) :
                _inventorySort == 2 ? source.OrderBy(value => value.DisplayName) : source.OrderByDescending(value => value.rarity).ThenByDescending(value => value.itemLevel);
            List<ItemInstance> items = ordered.ToList();
            if (items.Count == 0) { parent.Add(LabelFor("No matching items.", 12, Muted, false)); return; }
            VisualElement gridRow = null;
            foreach (ItemInstance item in items)
            {
                if (_inventoryGrid && (gridRow == null || gridRow.childCount >= 3))
                {
                    gridRow = Row();
                    parent.Add(gridRow);
                }
                VisualElement card = new VisualElement();
                card.style.backgroundColor = Card;
                card.style.marginTop = card.style.marginBottom = card.style.marginLeft = card.style.marginRight = 4f;
                card.style.paddingLeft = card.style.paddingRight = card.style.paddingTop = card.style.paddingBottom = 8f;
                if (_inventoryGrid) { card.style.width = Length.Percent(31.5f); card.style.minHeight = 150f; }
                ItemInstance captured = item;
                card.RegisterCallback<PointerDownEvent>(_ => _dragItem = captured);
                card.Add(LabelFor(item.DisplayName + "\n" + item.rarity.ToString().ToUpperInvariant() + " · LEVEL " + item.itemLevel +
                    "\n" + item.Definition.slot + (item.favorite ? " · ★ FAVORITE" : string.Empty) +
                    (item.locked ? " · 🔒 PROTECTED" : string.Empty), 12, item.Definition.color, true));
                card.Add(LabelFor(V11Itemization.BuildTooltip(item, false), 10, Muted, false));
                if (!unsecured)
                {
                    card.Add(LabelFor("COMPARE\n" + V11Itemization.BuildComparison(GameWorld.Instance.Equipment, item, ProfileManager.Current), 10, Cyan, false));
                }
                VisualElement actions = Row();
                if (!unsecured && !GameWorld.Instance.Equipment.EquipmentLocked)
                    actions.Add(ActionButton("EQUIP", () =>
                    {
                        string message;
                        if (GameWorld.Instance.Equipment.Equip(captured, out message))
                        {
                            GameWorld.Instance.Equipment.SaveSanctuaryEquipment(); Changed(message);
                        }
                        else SetMessage(message);
                    }));
                if (unsecured) actions.Add(ActionButton(_pendingDestructiveItem == item.instanceId ? "CONFIRM SALVAGE" : "SALVAGE", () => RequestSalvage(captured)));
                else if (!GameWorld.Instance.Equipment.EquipmentLocked)
                    actions.Add(ActionButton(_pendingDestructiveItem == item.instanceId ? "CONFIRM DISMANTLE" : "DISMANTLE", () => RequestDismantle(captured)));
                actions.Add(ActionButton(item.junk ? "UNMARK JUNK" : "MARK JUNK", () => { captured.junk = !captured.junk; PersistItemFlags(!unsecured); }));
                actions.Add(ActionButton(item.favorite ? "UNFAVORITE" : "FAVORITE", () => { captured.favorite = !captured.favorite; PersistItemFlags(!unsecured); }));
                actions.Add(ActionButton(item.locked ? "UNPROTECT" : "PROTECT", () => { captured.locked = !captured.locked; PersistItemFlags(!unsecured); }));
                card.Add(actions);
                (_inventoryGrid ? gridRow : parent).Add(card);
            }
        }

        private bool MatchesInventory(ItemInstance item)
        {
            if (item == null || item.Definition == null) return false;
            if (_inventoryRarity >= 0 && (int)item.rarity != _inventoryRarity) return false;
            if (_inventorySlot >= 0 && (int)item.Definition.slot != _inventorySlot) return false;
            if (_inventoryJunkOnly && !item.junk) return false;
            if (string.IsNullOrWhiteSpace(_inventorySearch)) return true;
            string query = _inventorySearch.Trim();
            return item.DisplayName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   item.Definition.slot.ToString().IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   item.rarity.ToString().IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   item.affixes.Any(value => value != null && value.id.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void RequestSalvage(ItemInstance item)
        {
            bool protectedItem = item.favorite || item.locked || item.corrupted || item.rarity == ItemRarity.Rare || item.rarity == ItemRarity.Unique;
            if (protectedItem && _pendingDestructiveItem != item.instanceId) { _pendingDestructiveItem = item.instanceId; SetPersistentMessage("Protected item: press Confirm Salvage to destroy it."); Rebuild(); return; }
            string message;
            RunDirector run = GameWorld.Instance.GetComponent<RunDirector>();
            if (GameWorld.Instance.Equipment.SalvageRunItem(item, run, protectedItem, out message)) { _pendingDestructiveItem = null; Changed(message); }
            else SetMessage(message);
        }

        private void RequestDismantle(ItemInstance item)
        {
            bool protectedItem = item.favorite || item.locked || item.corrupted || item.rarity == ItemRarity.Rare || item.rarity == ItemRarity.Unique;
            if (protectedItem && _pendingDestructiveItem != item.instanceId) { _pendingDestructiveItem = item.instanceId; SetPersistentMessage("Protected item: press Confirm Dismantle to destroy it."); Rebuild(); return; }
            string message;
            if (GameWorld.Instance.Equipment.DismantlePermanentItem(item, protectedItem, out message)) { _pendingDestructiveItem = null; Changed(message); }
            else SetMessage(message);
        }

        private void PersistItemFlags(bool permanent)
        {
            if (permanent) GameWorld.Instance.Equipment.SaveSanctuaryEquipment();
            Changed("Item flag updated.");
        }

        private void BuildTraining()
        {
            V21TrainingAnalytics analytics = V21TrainingAnalytics.Instance;
            if (analytics == null) { _content.Add(LabelFor("Training analytics are unavailable.", 16, Muted, false)); return; }
            VisualElement profiles = Row();
            foreach (V21TrainingTargetProfile profile in Enum.GetValues(typeof(V21TrainingTargetProfile)))
            {
                V21TrainingTargetProfile captured = profile;
                Button button = ActionButton(profile.ToString().ToUpperInvariant(), () => { analytics.SetTargetProfile(captured); Rebuild(); });
                button.style.flexGrow = 1f;
                button.style.backgroundColor = analytics.TargetProfile == profile ? Cyan : Card;
                profiles.Add(button);
            }
            _content.Add(profiles);
            _content.Add(LabelFor(analytics.Report(), 18, Text, true));
            _content.Add(LabelFor("Direct " + analytics.Snapshot.directDamage.ToString("0") + " · Triggered " +
                analytics.Snapshot.triggeredDamage.ToString("0") + " · Mana spent " + analytics.Snapshot.manaSpent.ToString("0.0"), 14, Cyan, false));
            foreach (KeyValuePair<string, float> pair in analytics.Snapshot.damageBySpell.OrderByDescending(value => value.Value))
                _content.Add(LabelFor(pair.Key + " · " + pair.Value.ToString("0") + " damage", 12, Muted, false));
            _content.Add(ActionButton("RESET ANALYTICS WINDOW", () => { analytics.ResetWindow(); Rebuild(); }));
        }

        private void Changed(string message)
        {
            GameWorld.Instance.RecalculateModifierAvailability();
            GameWorld.Instance.MarkSpellsDirty();
            RunDirector run = GameWorld.Instance.GetComponent<RunDirector>();
            if (run != null) run.SaveRunCheckpoint();
            SetPersistentMessage(message);
            Rebuild();
        }

        private void SetMessage(string message)
        {
            if (_message != null) _message.text = message ?? string.Empty;
            if (GameWorld.Instance != null && !string.IsNullOrEmpty(message)) GameWorld.Instance.Log(message);
        }

        private void SetPersistentMessage(string message)
        {
            _persistentMessage = message ?? string.Empty;
            SetMessage(_persistentMessage);
        }

        private static VisualElement Row()
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.flexShrink = 0f;
            return row;
        }

        private static VisualElement CardPanel(float width)
        {
            VisualElement panel = new VisualElement();
            panel.style.width = width;
            panel.style.flexGrow = 1f;
            panel.style.marginLeft = panel.style.marginRight = 5f;
            panel.style.paddingLeft = panel.style.paddingRight = panel.style.paddingTop = panel.style.paddingBottom = 12f;
            panel.style.backgroundColor = Card;
            return panel;
        }

        private static Label LabelFor(string text, int size, Color color, bool bold)
        {
            Label label = new Label(text ?? string.Empty);
            label.style.fontSize = size * (ProfileManager.Current == null ? 1f : ProfileManager.Current.accessibility.tooltipScale);
            label.style.color = color;
            label.style.whiteSpace = WhiteSpace.Normal;
            if (bold) label.style.unityFontStyleAndWeight = FontStyle.Bold;
            return label;
        }

        private static Button ActionButton(string text, Action action)
        {
            Button button = RuntimeUIFactory.CreateButton(action);
            button.text = text;
            button.style.minHeight = 34f;
            button.style.marginLeft = button.style.marginRight = button.style.marginTop = button.style.marginBottom = 3f;
            button.style.backgroundColor = Card;
            button.style.color = Text;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.whiteSpace = WhiteSpace.Normal;
            return button;
        }

        private static string TitleFor(ScreenKind screen)
        {
            if (screen == ScreenKind.Workshop) return "SPELL WORKSHOP";
            if (screen == ScreenKind.Links) return "SPELL LINKS";
            if (screen == ScreenKind.Inventory) return "EQUIPMENT & INVENTORY";
            if (screen == ScreenKind.Training) return "TRAINING ANALYTICS";
            return "ARCANE ENGINE";
        }
    }
}