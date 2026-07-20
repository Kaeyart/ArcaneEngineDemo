using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcaneEngine
{
    public enum SpellEffectCleanupReason224
    {
        RoomTransition,
        PlayerDeathOrRunEnd,
        SceneUnload,
        ManualValidation
    }

    // ARCANE_PATCH_224_LIFECYCLE_OWNERSHIP
    public sealed class SpellEffectLifecycle224 : MonoBehaviour
    {
        private static readonly HashSet<string> TransientComponentNames =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "SpellProjectile",
                "NovaEffect",
                "DelayedSpellEffect",
                "PersistentSpellZone",
                "SpellFamiliar",
                "ElementalReactionField",
                "ElementalReactionPulseEmitter",
                "ElementalReactionOrbiters",
                "ElementalReactionShard",
                "ReactionMechanicStageRunner22",
                "SpellMorphologyOwner21",
                "GeneratedSpellMorphologyHost21",
                "PresentationResidue2",
                "TransientScale21",
                "NearMissPresentation21"
            };

        private static readonly string[] TransientNamePrefixes =
        {
            "AE21 Cast ·",
            "AE21 Expire ·",
            "AE21 Morphology ·",
            "AE21 Cast Glyph ·",
            "AE21 Surface Residue ·",
            "AE2 Cosmetic Residue",
            "Reaction Stage ·",
            "Reaction Pulse ·",
            "Reaction Orbiters ·",
            "Reaction Shards ·"
        };

        private static SpellEffectLifecycle224 _instance;
        private static bool _clearing;
        private bool _previousRunActive;
        private string _previousActiveScene;

        public static int LastClearedObjects { get; private set; }
        public static SpellEffectCleanupReason224 LastReason { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (_instance != null)
                return;

            GameObject host = new GameObject("Arcane Engine 2.2.4 Spell Effect Lifecycle");
            DontDestroyOnLoad(host);
            _instance = host.AddComponent<SpellEffectLifecycle224>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            _previousRunActive = GameWorld.Instance != null && GameWorld.Instance.RunActive;
            _previousActiveScene = SceneManager.GetActiveScene().name;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            if (_instance == this)
                _instance = null;
        }

        private void LateUpdate()
        {
            bool runActive = GameWorld.Instance != null && GameWorld.Instance.RunActive;
            if (_previousRunActive && !runActive)
                ClearAll(SpellEffectCleanupReason224.PlayerDeathOrRunEnd);
            _previousRunActive = runActive;

            string activeScene = SceneManager.GetActiveScene().name;
            if (!string.Equals(_previousActiveScene, activeScene, StringComparison.Ordinal))
            {
                _previousActiveScene = activeScene;
                ClearAll(SpellEffectCleanupReason224.RoomTransition);
            }
        }

        private static void OnActiveSceneChanged(Scene previous, Scene next)
        {
            if (previous.IsValid() && next.IsValid() && previous.handle != next.handle)
                ClearAll(SpellEffectCleanupReason224.RoomTransition);
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            ClearAll(SpellEffectCleanupReason224.SceneUnload);
        }

        public static void ClearAll(SpellEffectCleanupReason224 reason)
        {
            if (!Application.isPlaying || _clearing)
                return;

            _clearing = true;
            try
            {
                LastReason = reason;
                LastClearedObjects = DisableAndDestroyTransientObjects();

                PresentationParticlePool2.ClearAll();
                ProceduralSpellAudio21.StopAll();
                ElementalReactionField.ClearAll224();
                PresentationResidue2.ClearAll224();
                MorphologyPresentationDirector21.ClearTransient224();
                ReactionPresentationCoalescer22.ClearAll224();
                ReactionLineageRegistry22.ClearAll224();
                ReactionDiagnostics22.Reset();
                SpellPresentationBus.Clear();
                ProceduralSpellVisualCompiler2.ClearCache();
            }
            finally
            {
                _clearing = false;
            }
        }

        private static int DisableAndDestroyTransientObjects()
        {
            // ARCANE_PATCH_225_FIND_API_FIX
            MonoBehaviour[] behaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include);
            HashSet<EntityId> destroyed = new HashSet<EntityId>();
            int count = 0;

            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (behaviour == null || behaviour is SpellEffectLifecycle224)
                    continue;

                GameObject target = behaviour.gameObject;
                if (target == null || !IsTransient(behaviour.GetType().Name, target.name))
                    continue;

                EntityId id = target.GetEntityId();
                if (!destroyed.Add(id))
                    continue;

                target.SetActive(false);
                UnityEngine.Object.Destroy(target);
                count++;
            }

            Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(
                FindObjectsInactive.Include);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform value = transforms[i];
                if (value == null || value.gameObject == null || !HasTransientPrefix(value.gameObject.name))
                    continue;

                EntityId id = value.gameObject.GetEntityId();
                if (!destroyed.Add(id))
                    continue;

                value.gameObject.SetActive(false);
                UnityEngine.Object.Destroy(value.gameObject);
                count++;
            }

            return count;
        }

        private static bool IsTransient(string typeName, string objectName)
        {
            return TransientComponentNames.Contains(typeName) || HasTransientPrefix(objectName);
        }

        private static bool HasTransientPrefix(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                return false;
            for (int i = 0; i < TransientNamePrefixes.Length; i++)
                if (objectName.StartsWith(TransientNamePrefixes[i], StringComparison.Ordinal))
                    return true;
            return false;
        }
    }
}
