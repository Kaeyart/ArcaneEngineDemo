using UnityEngine;

namespace ArcaneEngine
{
    [CreateAssetMenu(menuName = "Arcane Engine/2.1/Room Definition", fileName = "RoomDefinition")]
        public sealed class V21RoomDefinitionAsset : ScriptableObject
        {
            public string stableId;
            public string displayName;
            public string biome;
            public DungeonRoomType roomType;
            public int difficulty;
            public int obstaclePattern;
            public bool hasHazards;
            public Color floorColor = Color.gray;
            public Color accentColor = Color.cyan;
            public RoomTemplate ToRuntime() { return new RoomTemplate { id = stableId, displayName = displayName, biome = biome, type = roomType,
                difficulty = difficulty, obstaclePattern = obstaclePattern, hasHazards = hasHazards, floorColor = floorColor, accentColor = accentColor }; }
        }
}
