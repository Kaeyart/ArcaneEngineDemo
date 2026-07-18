# Arcane Engine 1.7.0 Demo — Visual Overhaul II: Enemies & Impact

## Implemented

- Added modular runtime visual definitions for all 11 existing enemy archetypes: Crawler, Bulwark, Hexer, Charger, Warden, Leech, Mirror, Assassin, Controller, Dungeon Warden, and Training Dummy.
- Each archetype now receives a role-driven silhouette assembled from body, head, and role modules. These modules are presentation only and retain the existing collider, movement, attack, and damage model.
- Added procedural idle, attack, telegraph, stagger, and boss motion layered on the visual assembly without moving the authoritative enemy root.
- Added persistent visual marks for all six Elite affixes: Frenzied, Shielded, Volatile, Vampiric, Resistant, and Summoner.
- Added target-attached Burning, Poison, Shock, Chill, Freeze, and Stagger treatments that read the enemy's live status state and turn off when the state ends.
- Added event-driven melee, charge, ranged, and boss telegraphs. Their radius and direction come from the existing attacks.
- Added element-shaped hit and death feedback, critical-hit variants, and multi-layer Dungeon Warden phase transitions.
- Health-bar anchors now refresh their renderer list after the modular visual assembly is complete, so their world position follows the rendered bounds instead of the original hidden primitive.

## Mechanical boundaries

- Enemy visual modules do not add collision, health, armor, resistance, movement, targeting, attacks, or drops.
- Telegraph effects communicate existing attack windows; they do not delay or accelerate those windows.
- Enemy logic and the single-camera aiming fix remain unchanged.

## Verification status

- All C# sources pass a full tree-sitter syntax parse.
- Play Mode readability, overlap, boss stress, and health-bar placement still require verification in Unity 6000.5.2f1.
