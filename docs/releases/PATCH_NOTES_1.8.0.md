# Arcane Engine 1.8.0 Demo — Visual Overhaul III: Living Dungeons

## Implemented

- Added `BiomeVisualDefinition` and deterministic runtime assembly for four biome kits: Ossuary Catacombs, Ember Foundry, Sunken Archive, and Venom Cistern.
- Standard runs now derive an initial biome from the run seed and transition to a different biome at the midpoint. Daily, custom-seed, and continued runs reconstruct the same order from the saved mechanical seed and room index.
- Boss rooms use a dedicated Warden Sanctum visual override. Home Base uses a separate Relic Forge definition and workshop overlay.
- Added biome-specific floor, wall, trim, support, prop-cluster, background-architecture, ambient-motion, fog-reference, and emission palettes.
- Added generated boundary trim, supports, room-edge clusters, distant architecture, and restrained ambient motes. Environment density controls how many decorative pieces are constructed.
- Added distinct purpose overlays for every one of the existing 17 room types. These are assembled separately from biome dressing, allowing the same room purpose to remain recognizable across different biomes.
- Added biome-aware door braces while preserving the existing room-type icon, interaction prompt, and route logic.
- The active room template now reports its resolved biome name after generation.

## Spatial and mechanical boundaries

- Added dungeon dressing has no colliders. Existing room obstacles and hazards remain the only authoritative generated obstructions.
- Dressing is placed on the perimeter or outside the combat plane and does not participate in navigation, aiming raycasts, spawn selection, reward placement, or door interaction.
- Biome selection changes presentation only. It does not alter enemies, rewards, difficulty, room count, or route probabilities.
- The single isometric camera is unchanged and no overhead piece is generated over the playable center.

## Verification status

- All C# sources pass a full tree-sitter syntax parse.
- A visual route matrix, checkpoint comparison, low-density pass, and camera-occlusion pass still require Play Mode verification in Unity 6000.5.2f1.
