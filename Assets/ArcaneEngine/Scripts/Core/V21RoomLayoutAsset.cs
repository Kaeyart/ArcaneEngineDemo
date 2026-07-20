using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    [CreateAssetMenu(menuName = "Arcane Engine/2.1/Room Layout", fileName = "RoomLayout")]
        public sealed class V21RoomLayoutAsset : ScriptableObject
        {
            public string stableId;
            public string displayName;
            public string biome;
            public DungeonRoomType roomType;
            [Range(0, 7)] public int shell;
            public Vector2 floorSize = new Vector2(34f, 34f);
            public List<Vector3> obstaclePositions = new List<Vector3>();
            public List<Vector3> obstacleScales = new List<Vector3>();
            public List<Vector3> hazardAnchors = new List<Vector3>();
            public List<Vector3> enemySpawnZones = new List<Vector3>();
            public List<Vector3> objectiveAnchors = new List<Vector3>();
            public Vector3 rewardAnchor = Vector3.zero;
            public float cameraSize = 16f;

            public void Normalize()
            {
                if (obstaclePositions == null) obstaclePositions = new List<Vector3>();
                if (obstacleScales == null) obstacleScales = new List<Vector3>();
                if (hazardAnchors == null) hazardAnchors = new List<Vector3>();
                if (enemySpawnZones == null) enemySpawnZones = new List<Vector3>();
                if (objectiveAnchors == null) objectiveAnchors = new List<Vector3>();
                floorSize.x = Mathf.Clamp(floorSize.x, 24f, 44f);
                floorSize.y = Mathf.Clamp(floorSize.y, 24f, 44f);
                cameraSize = Mathf.Clamp(cameraSize, 12f, 22f);
            }
        }
}
