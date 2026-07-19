using UnityEngine;

namespace ArcaneEngine
{
    /// <summary>
    /// Centralized game configuration — replaces hundreds of magic numbers scattered across
    /// the codebase. Create one instance via Assets > Create > Arcane Engine > Game Config
    /// and drop it into Resources/V21Content/.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Arcane Engine/Game Config", order = 0)]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Player")]
        public float playerMoveSpeed = 7.5f;
        public float playerDashSpeed = 18f;
        public float playerDashDuration = 0.18f;
        public float playerDashCooldown = 1.2f;
        public float playerMaxMana = 200f;
        public float playerManaRegen = 8f;
        public float playerMaxHealth = 100f;
        public int playerSpellSlotCount = 5;

        [Header("Combat")]
        public float defaultCastRange = 12f;
        public float globalCooldown = 0.25f;
        public float globalCooldownLegacy { get { return globalCooldown; } set { globalCooldown = value; } }
        public float defaultProjectileSpeed = 18f;
        public float aimAssistRadius = 0.8f;
        public float aimCastTolerance = 2.5f;
        public float damageNumberLifetime = 0.95f;
        public int maxDamageNumbers = 70;
        public float combatStateTimeout = 5f;

        [Header("Enemies")]
        public float enemyAggroRange = 18f;
        public float enemyLeashRange = 30f;
        public float enemyPathUpdateInterval = 0.25f;
        public float enemyAttackCooldown = 1.5f;
        public float eliteHealthMultiplier = 2.5f;
        public float eliteDamageMultiplier = 1.5f;
        public float bossHealthMultiplier = 8f;
        public float bossDamageMultiplier = 2f;

        [Header("Rooms")]
        public int maxEnemiesPerRoom = 12;
        public int maxElitesPerRoom = 2;
        public float roomClearRadius = 22f;
        public int minCombatRoomsPerRun = 6;
        public int maxCombatRoomsPerRun = 10;

        [Header("Spells")]
        public int hexBoardCapacityBase = 6;
        public int hexBoardCapacityPerLevel = 3;
        public int maxSpellLinks = 4;
        public int maxTriggerActivations = 8;
        public float triggerInternalCooldown = 0.15f;

        [Header("Items")]
        public int inventorySlotCount = 24;
        public int equipmentSlotCount = 6;
        public int maxAffixesPerItem = 4;
        public float legendaryDropChance = 0.02f;

        [Header("Visuals")]
        public int maxActiveVisuals = 128;
        public int maxDynamicLights = 8;
        public int materialCacheLimit = 128;
        public int objectPoolCapacity = 200;

        [Header("UI")]
        public float menuFadeDuration = 0.25f;
        public float healthBarUpdateInterval = 0.1f;
        public float crosshairSize = 7f;
        public float bossBarWidth = 720f;
        public float bossBarHeight = 74f;

        private static GameConfig _cached;

        /// <summary>Loads the config from Resources. Cached after first load.</summary>
        public static GameConfig Load()
        {
            if (_cached != null) return _cached;
            _cached = Resources.Load<GameConfig>("V21Content/GameConfig");
            if (_cached == null)
            {
                Debug.LogWarning("[GameConfig] No GameConfig asset found in Resources/V21Content/ — using code defaults.");
                _cached = ScriptableObject.CreateInstance<GameConfig>();
            }
            return _cached;
        }

        /// <summary>Invalidate cached config (for testing).</summary>
        public static void ClearCache()
        {
            _cached = null;
        }
    }
}