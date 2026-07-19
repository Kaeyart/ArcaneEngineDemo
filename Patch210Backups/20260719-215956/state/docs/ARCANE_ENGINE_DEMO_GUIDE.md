# Arcane Engine Demo — Complete Project Guide

> **For new developers, testers, and anyone encountering this project for the first time.**

---

## 1. What Is This?

**Arcane Engine Demo** is a **prototype/vertical slice** of an action-RPG / roguelite game built in **Unity 6000.5.2f1**.

The core fantasy: you are a spellcaster who crafts their own spells by placing "Support Runes" on a hexagonal grid (the "Spell Board"). Each rune modifies how a spell behaves — adding projectiles, damage over time, homing, area-of-effect, chaining, and more. You then take these custom-crafted spells into procedurally generated rooms and fight enemies.

### Genre
- **Action-RPG** (real-time combat, dodge, aim)
- **Roguelite** (procedural rooms, permadeath with permanent upgrades between runs)
- **Spellcrafting** (the unique mechanic — build spells from components on a hex grid)

### Current Status
**v2.1.0 — Demo / Source Candidate.** The project has never been fully compiled and tested inside Unity as a complete run. It was validated statically (code analysis) but may have runtime bugs.

---

## 2. Project Structure (Top-Level)

```
ArcaneEngineDemo/
├── Assets/                     # Unity assets (scripts, scenes, prefabs, resources)
│   └── ArcaneEngine/           # All game code
├── Packages/                   # Unity package dependencies
├── ProjectSettings/            # Unity project configuration
├── Tools/                      # Build/release scripts
├── docs/                       # Release notes, patch documentation
└── [Root files]               # README, changelog, install guides
```

---

## 3. How the Game Works (High-Level Flow)

### Boot Sequence
1. **`GameBootstrap.cs`** runs on Unity startup (`RuntimeInitializeOnLoadMethod`)
2. Creates a persistent **"Arcane Engine Demo"** GameObject (DontDestroyOnLoad)
3. Attaches ~32 components including: `GameWorld`, `PlayerController`, `RunDirector`, `V21ProductUI`, `DemoUI`, audio, visuals, training analytics
4. `GameWorld.Awake()` loads the player profile, initializes the spell catalog, builds the sanctuary (home base) room

### Game Loop
```
Home Base (sanctuary)
    → Prepare spells (workshop) + equipment (armory)
    → Begin expedition (RunDirector)
        → Procedural rooms with enemies
        → Clear room → choose next path
        → Find loot, runes, new spell cores
        → Boss room → extract or die
    → Return to home base with rewards
    → Upgrade permanently → repeat
```

### Home Base Screens (opened via keyboard shortcuts)
| Key | Screen | Purpose |
|-----|--------|---------|
| **Tab** | Spell Workshop | Craft spells by placing runes on hex boards |
| **I** | Inventory & Equipment | Equip gear, manage stash, salvage items |
| **C** (or #) | Spell Links | Create trigger chains between spells |
| **F2** | Training | DPS testing against training dummies |
| **Escape** | Pause/Close | Close current screen or pause |

---

## 4. File Structure — All Source Code

### `Assets/ArcaneEngine/Scripts/Core/` (25 files)
The engine's heart — game state, combat, input, run flow.

| File | What It Does |
|------|-------------|
| **`GameBootstrap.cs`** | Entry point. Creates the persistent game object on startup, attaches all components. |
| **`GameWorld.cs`** (976 lines) | Central singleton. Owns: player reference, enemies list, spell boards (3 slots), equipment inventory, modifier inventory, run state save/restore, room building, loot spawning, camera, lighting. **The god-object.** |
| **`GameConfig.cs`** | ScriptableObject with tunable game parameters (move speed, damage numbers, cooldowns, room counts, etc.). |
| **`PlayerController.cs`** | Player entity: health/mana/ward, movement, 3-slot spell casting, cooldown management, dodge, death/respawn. |
| **`EnemyController.cs`** | Enemy AI: pathfinding, attack patterns, telegraphing, taking damage, death. |
| **`CombatMath.cs`** | Static damage calculation: resistance, armor, critical hits, elemental modifiers, shields. |
| **`RunDirector.cs`** (1515 lines) | Run state machine: start run, enter rooms, complete encounters, rewards, shops, bosses, difficulty scaling, save checkpoints. |
| **`RoomSceneManager.cs`** | Manages transitions between rooms during a run. |
| **`RunContent.cs`** | Defines room content: enemy spawns, rewards, encounter types. |
| **`ServiceLocator.cs`** | Dependency injection registry — components register/unregister themselves, others resolve via `Get<T>()`. |
| **`ArcaneInput.cs`** | Input system wrapper. Abstracts keyboard, mouse, controller (gamepad) into unified actions. |
| **`HardwareMouseAim.cs`** | Raw mouse input for aiming the crosshair. |
| **`ProfileSystem.cs`** (in Progression/) | Save/load player profile (unlocks, inventory, prepared spells, settings). |
| **`DemoCatalog.cs`** | Defines all spell cores (15), modifiers (64+), items (64+) available in the demo. |
| **`DemoV05Systems.cs`** | Legacy demo systems for the earliest prototype. |
| **`V1Foundation.cs`** | Core game foundation from v1.x series. |
| **`V12Systems.cs`** | Systems added in v1.2 overhaul. |
| **`V21AudioDirector.cs`** | Audio event system — manages procedurally generated sound effects. |
| **`V21AuthoredContent.cs`** | ScriptableObject definitions for persistent authored content: room layouts, enemy definitions, affixes, shop services, rewards, audio events. |
| **`V21AuthoringMarkers.cs`** | Editor markers for room prefab authoring. |
| **`V21BindingService.cs`** | Key/controller bindings service. |
| **`V21MinibossMechanics.cs`** | Mini-boss and boss encounter scripts. |
| **`V21TrainingAnalytics.cs`** | Tracks DPS, damage breakdown, and training metrics. |
| **`BossEncounterMechanics.cs`** | Boss-specific attack patterns and phases. |
| **`RuntimeVisuals.cs`** | Helper to create runtime primitive visual objects (cubes, spheres, cylinders). |
| **`WorldGameSpaces.cs`** | World/room layout definitions. |
| **`WorldInteractions.cs`** | Interactable objects in rooms (chests, altars, doors). |

### `Assets/ArcaneEngine/Scripts/Items/` (3 files)
| File | What It Does |
|------|-------------|
| **`ItemSystem.cs`** | `EquipmentInventory` — equip/unequip items, backpack management, salvage, stat calculation from gear. |
| **`V11Itemization.cs`** | All item and affix definitions (rarities, slots, stat ranges, tooltip generation). |
| **`LootPickup.cs`** | World-space loot objects (items, gold, modifiers, essence). |

### `Assets/ArcaneEngine/Scripts/Spells/` (5 files)
| File | What It Does |
|------|-------------|
| **`SpellDefinitions.cs`** | Data definitions: `SpellCoreDefinition` (base spell type, damage, delivery method), `SpellModifierDefinition` (rune data: shape, color, cost, compatibility). |
| **`HexBoard.cs`** | The hex grid board. `SpellBoard` class: places/removes/rotates runes, validates placement, tracks used capacity, undo/redo. |
| **`SpellCompiler.cs`** | Takes a `SpellBoard` (core + placed runes) and produces a `CompiledSpell` with final stats (damage, cooldown, projectile count, effects). |
| **`SpellExecution.cs`** | Runtime execution of compiled spells — creates projectiles, applies damage, handles triggers. |
| **`SpellLinks.cs`** | `SpellLinkSystem` — trigger chains between spells (OnHit → cast Spell B). |

### `Assets/ArcaneEngine/Scripts/UI/` (10 files)
| File | What It Does |
|------|-------------|
| **`V21ProductUI.cs`** | **Main UI** (900+ lines). 4 screens: Spell Workshop, Spell Links, Inventory, Training. Uses UI Toolkit + IMGUI fallback. |
| **`HexCellElement.cs`** | Custom `VisualElement` that draws a pointy-top hexagon via `Painter2D` with proper hexagonal hit-testing. |
| **`RuntimeUIFactory.cs`** | Creates UIDocument instances, manages the shared PanelSettings and theme, contains `ReliableButton` (IMGUI + UI Toolkit hybrid button) and `RuntimeUIDocumentInput` (OnGUI fallback handler). |
| **`DemoUI.cs`** (1861 lines) | Legacy IMGUI-based full game UI. 8 home base tabs (Expedition, Preparation, Archives, Armory, Forge, Upgrades, Codex, Training/Options). Active when `V21ProductUI` isn't loaded. |
| **`ModernCombatHUD.cs`** | In-combat HUD: health/mana/ward bars, spell slot display, cooldown indicators, damage numbers, status effects. Uses UI Toolkit. |
| **`CombatPresentation.cs`** | Legacy shim — forwards damage number rendering to ModernCombatHUD. |
| **`ArcaneUIHUDController.cs`** | Bridges UI Toolkit HUD (`ArcaneHUD.uxml`) to `GameWorld` data. Animated bars, cooldowns. |
| **`V1FrontEndUI.cs`** | Title screen, main menu, new game / continue. |
| **`RunStartScreen.cs`** | Difficulty selection screen before starting a run. |
| **`V11EnemyHealthBars.cs`** | Renders health bars above enemies during combat. |

### `Assets/ArcaneEngine/Scripts/Visuals/` (11 files)
| File | What It Does |
|------|-------------|
| **`ProceduralVisualRuntime.cs`** | Runtime procedural mesh/visual generation. |
| **`ProceduralDungeonVisuals.cs`** | Generates room visuals (floor, walls, decorations). |
| **`ProceduralEnemyVisuals.cs`** | Enemy appearance generation. |
| **`SpellVisualConstructions.cs`** | Visual effects for spells (projectile trails, impact explosions, zone effects). |
| **`VisualPresentationDirector.cs`** | Orchestrates complex visual sequences. |
| **`VisualStressScenarios.cs`** | Stress-test visual scenarios for performance testing. |
| **`VisualCorrectiveContractValidation.cs`** | Validates visual contracts at startup. |
| **`VisualRuntimePolicy.cs`** | Rules for runtime visual object pooling/cleanup. |
| **`VisualContinuationValidation.cs`** | Validates visual state after scene transitions. |
| **`LightingProfiles.cs`** | Room lighting configurations. |
| **`RunResultVisuals.cs`** | End-of-run summary visuals. |
| **`WorldInteractionVisuals.cs`** | Visual feedback for interactable objects. |

### `Assets/ArcaneEngine/Editor/` (3 files)
Editor-only tools (not included in builds).

| File | What It Does |
|------|-------------|
| **`V21ContentBuilder.cs`** | Menu-driven tool (`Arcane Engine > 2.1 > Rebuild Authored Content`) that generates all ScriptableObject content assets (spells, runes, items, rooms, enemies, audio). |
| **`V21AcceptanceValidation.cs`** | Automated validation tests that check content integrity at assembly load. |
| **`ArcaneEngineBuild.cs`** | Custom build pipeline script. |

### `Assets/ArcaneEngine/Tests/` (5 files)
| File | What It Tests |
|------|--------------|
| **`CombatMathTests.cs`** | 9 tests: damage calculation, resistance, armor, critical hits, shields. |
| **`SpellSystemTests.cs`** | 9 tests: spell compilation, hex board capacity, spell links, cast budget. |
| **`ItemSystemTests.cs`** | 8 tests: item creation, inventory management, rarity, affixes. |
| **`ArcaneEngineEditModeTests.cs`** | 5 tests: core catalog validation, spell compilation across all cores, legendary signatures, hex rotation, cyclic link detection. |
| **`ArcaneEnginePlayModeTests.cs`** | 3 PlayMode tests: camera creation, UI panels, run initialization. |

---

## 5. Core Mechanics Explained

### Spellcrafting (The Main Feature)

Each character has **3 spell slots**. Each slot contains:

1. **A Spell Core** (the base spell type — Fireball, Frost Lance, Lightning Strike, etc.)
2. **A Hex Board** (a hexagonal grid that starts at level 1 with ~6 cells capacity, grows with levels)
3. **Placed Support Runes** (modifiers placed on the hex grid that change the spell's behavior)

**The Workshop (Tab key):**
```
┌─────────────────────────────────────────────────┐
│  SLOT 1 [Fireball]   SLOT 2 [Frost Lance]  SLOT 3 [EMPTY]
├──────────┬────────────────────────┬──────────────┤
│ RUNES    │     HEX BOARD          │ SPELL PREVIEW│
│ [Accel]  │    ⬡ ⬡ ⬡ ⬡          │ Damage: 45→78│
│ [Burning]│     ⬡ ⬢ ⬡ ⬡         │ Mana: 24→36  │
│ [Fork]   │    ⬡ ⬡ ⬡ ⬡          │ Cooldown: 1.8 │
│ [Pierce] │     ⬡ ⬡ ⬡ ⬡         │ ───────────── │
│          │                        │ LIVE ANIMATION│
└──────────┴────────────────────────┴──────────────┘
```

- **Place a rune:** Drag from the left panel onto the hex board
- **Rotate:** Q/E keys or scroll wheel while hovering
- **Remove:** Right-click on a placed rune
- **Move:** Drag an existing rune to a new hex
- **Preview:** The right panel shows damage/mana/cooldown changes in real-time

**Rune Compatibility:**
Not all runes work with all spells. A rune like "Beam Delivery" only works with cores that support beam delivery. Incompatible runes are greyed out in the palette.

### Combat
- Mouse to aim (crosshair follows cursor)
- WASD to move
- Q/E rotate runes in workshop, in combat they may have other bindings
- Left-click to cast selected spell (1/2/3 number keys select slot)
- Dodge roll (Space or Shift by default)
- Enemies telegraph attacks with colored indicators before striking

### Progression
Between runs, you earn:
- **Essence** → permanent stat upgrades (health, mana, power ranks)
- **Drachmas (gold)** → buy items in the shop
- **Forge materials** → craft and upgrade items
- **New spell cores** → unlock more base spells
- **Rune unlocks** → permanent modifier access
- **Items** → gear with randomized affixes (common → magic → rare → unique)

---

## 6. Key Architecture Decisions

### UI System: UI Toolkit + IMGUI Fallback
The game uses **Unity UI Toolkit** (`VisualElement`, `UIDocument`) for rendering. However, Unity's UI Toolkit has unreliable click handling in some versions, so there's a **`RuntimeUIDocumentInput`** component with an `OnGUI()` fallback that intercepts mouse events. This fallback was recently updated to be **drag-aware** (6px threshold) so that dragging doesn't accidentally trigger button clicks.

### Runtime-Only (No Scene Dependencies)
The game builds its entire world at runtime — the Main scene is an empty root transform. `GameWorld.BuildRoom()` creates the camera, player, room geometry, enemies, and loot procedurally. This means:
- No scene setup needed
- Everything can be scripted/tuned from code
- The game boots directly into the sanctuary (home base)

### Service Locator Pattern
`ServiceLocator.cs` replaces traditional singletons. Components register themselves (`ServiceLocator.Register<IFoo>(this)`) in Awake and others resolve them (`ServiceLocator.Get<IFoo>()`).

### Spell Compilation Pipeline
```
SpellBoard (hex grid + core + placed runes)
    → SpellCompiler.Compile()
        → Reads core stats
        → Applies each rune's modifier effect
        → Summons triggers from SpellLinks
    → CompiledSpell (final stats, execution layers)
    → SpellBehaviorGraph (optimized execution plan)
```

---

## 7. Known Issues & Limitations

- **Compilation:** One 3rd-party package (`com.unity.cloud.gltfast@6.19.0`) has a known `LitMaterialExport` type-missing error. This does not block the game — it's a cache artifact.
- **PlayMode tests:** Cannot run because the game's title screen blocks test coroutine execution.
- **EditMode tests:** 7/32 fail due to `Resources.LoadAll<T>()` returning empty in EditMode test context (the .asset files exist on disk but aren't loadable through Resources in tests).
- **Burst compiler:** Reports a non-critical warning about missing `Assembly-CSharp-Editor` (expected with asmdef-based projects).
- **GameWorld.cs** is a 976-line god-class that handles too many responsibilities.
- **No URP/HDRP** — the project uses Unity's Built-in Render Pipeline, so post-processing effects (Bloom, Vignette) are not available.

---

## 8. Keyboard Shortcuts

| Key | Action |
|-----|--------|
| **Tab** | Open/close Spell Workshop |
| **I** | Inventory & Equipment |
| **C** / **#** | Spell Links |
| **F2** | Training mode |
| **Escape** / **B** | Close current screen |
| **W/A/S/D** | Move player |
| **Mouse** | Aim |
| **Left Click** | Cast selected spell |
| **1/2/3** | Select spell slot |
| **Space** / **Shift** | Dodge roll |
| **Q** | Rotate rune CCW (in workshop) |
| **E** | Rotate rune CW (in workshop) |
| **Scroll Wheel** | Rotate rune (in workshop) |
| **Right-click** | Remove placed rune (on hex) |

---

## 9. How to Build

### From Unity Editor
1. Open the project in Unity 6000.5.2f1
2. Open `Assets/ArcaneEngine/Scenes/Main.unity`
3. Press Play
4. The game boots into the sanctuary

### From Command Line
```bash
# Build for Linux standalone
./Tools/build-release-linux.sh

# Run edit-mode tests
./Tools/run-tests-linux.sh
```

### Menu Items (Editor)
| Menu Path | Action |
|-----------|--------|
| `Arcane Engine > 2.1 > Rebuild Authored Content` | Generate all ScriptableObject content assets |
| `Window > MCP for Unity > Toggle MCP Window` | Open AI assistant bridge panel |

---

## 10. Package Dependencies (`Packages/manifest.json`)

| Package | Version | Purpose |
|---------|---------|---------|
| `com.unity.cinemachine` | 3.1.7 | Camera system |
| `com.unity.inputsystem` | 1.19.0 | New input system |
| `com.unity.probuilder` | 6.1.2 | In-editor 3D modeling |
| `com.unity.cloud.gltfast` | 6.19.0 | glTF model import |
| `com.unity.textmeshpro` | 3.0.9 | Text rendering |
| `com.unity.visualeffectgraph` | 17.5.0 | VFX Graph |
| `com.unity.test-framework` | 1.4.6 | Unity Test Framework |
| `com.unity.ai.assistant` | 2.15.0-pre.1 | Unity AI Assistant |
| `com.unity.ai.inference` | 2.6.1 | AI inference SDK |
| `com.coplaydev.unity-mcp` | git | MCP bridge for AI tool integration |

---

## 11. Quick Reference: Key Classes & Their Data

| Class | Location | Key Fields |
|-------|----------|------------|
| `SpellCoreDefinition` | Spells/SpellDefinitions.cs | `id`, `displayName`, `element`, `delivery`, `baseDamage`, `manaCost`, `cooldown`, `color` |
| `SpellModifierDefinition` | Spells/SpellDefinitions.cs | `id`, `displayName`, `shape` (HexCoord[]), `uiColor`, `capacityCost`, `compatibleDeliveries`, `IsCompatible()` |
| `SpellBoard` | Spells/HexBoard.cs | `coreId`, `spellLevel`, `placed` (List<PlacedModifier>), `Capacity`, `TryPlace()`, `RemoveAt()`, `TryMove()`, `RotateAt()`, `PieceAt()`, `AllCells()` |
| `CompiledSpell` | Spells/SpellExecution.cs | `damage`, `manaCost`, `cooldown`, `projectileCount`, `primaryColor`, `executionLayers`, `warnings` |
| `HexCoord` | Spells/SpellDefinitions.cs | `q`, `r` (axial coordinates). `Rotate(value, steps)` → rotated coord |
| `GameWorld` | Core/GameWorld.cs | `Instance`, `Player`, `Stats`, `Enemies`, `Equipment`, `ModifierInventory`, `GetBoard(slot)`, `GetSpell(slot)`, `CanEditSpells` |
| `RunDirector` | Core/RunDirector.cs | `RoomIndex`, `CurrentSeed`, `Drachmas`, `ForgeDust`, `BeginRun()`, `EnterRoom()`, `CompleteRoom()` |
| `PlacedModifier` | Spells/HexBoard.cs | `modifierId`, `anchor` (HexCoord), `rotation` (0-5), `placementOrder` |
| `ItemInstance` | Items/ItemSystem.cs | `definitionId`, `rarity`, `itemLevel`, `affixes`, `DisplayName`, `Definition` |
| `PlayerStats` | Items/ItemSystem.cs | `maxHealth`, `maxMana`, `spellPower`, `moveSpeed`, `critChance`, `armor`, `resistance` |

---

## 12. The Hex Grid Coordinate System

The workshop uses **axial coordinates** for the hex grid:

```
(q, r) where:
  q = column (increases to the right)
  r = row (increases downward)

Neighbors:
  (q+1, r), (q-1, r)
  (q, r+1), (q, r-1)
  (q+1, r-1), (q-1, r+1)
```

**Rune shapes** are defined as arrays of `HexCoord` offsets from the anchor point.
Example: a rune that occupies 3 hexes in a line might have shape `{ (0,0), (1,0), (2,0) }`.

**Rotation** transforms these offsets using `HexCoord.Rotate(coord, steps)`:
```
(q, r) → (-r, q+r)  (60° clockwise, 1 step)
```

**Pixel positioning** (used in `CreateHexCell`):
```
centerX = 285 + (q + r × 0.5) × HexWidth
centerY = 285 + r × HexRowStep
where HexWidth = √3 × 33.333, HexRowStep = 50