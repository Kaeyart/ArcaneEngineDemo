using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcaneEngine
{
    /// <summary>
    /// Additive scene loading for rooms — replaces single-scene room instantiation.
    /// Rooms are separate scenes loaded additively and unloaded when the player leaves.
    /// Reduces memory pressure and enables async transitions.
    /// </summary>
    public sealed class RoomSceneManager : MonoBehaviour
    {
        public static RoomSceneManager Instance { get; private set; }

        private string _currentRoomScene;
        private readonly Queue<string> _preloadQueue = new Queue<string>();
        private readonly Dictionary<string, AsyncOperation> _pendingOperations = new Dictionary<string, AsyncOperation>();
        private Coroutine _preloadCoroutine;

        [Header("Scene Names (must match Build Settings)")]
        public string persistentScene = "Main";
        public string homeBaseScene = "Rooms/HomeBase";
        public string[] roomScenes;

        private void Awake()
        {
            Instance = this;
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister(this);
            if (Instance == this) Instance = null;
        }

        /// <summary>
        /// Load a room scene additively. Unloads the current room if one exists.
        /// </summary>
        public IEnumerator LoadRoomAsync(string sceneName, Action onComplete = null)
        {
            // Unload current room
            if (!string.IsNullOrEmpty(_currentRoomScene) && _currentRoomScene != sceneName)
            {
                yield return UnloadRoomAsync(_currentRoomScene);
            }

            // Load new room additively
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                if (loadOp == null)
                {
                    Debug.LogError($"[RoomSceneManager] Failed to load scene: {sceneName}");
                    onComplete?.Invoke();
                    yield break;
                }

                loadOp.allowSceneActivation = false;

                // Wait for load to complete (0.9 progress = fully loaded, waiting for activation)
                while (loadOp.progress < 0.9f)
                    yield return null;

                loadOp.allowSceneActivation = true;

                while (!loadOp.isDone)
                    yield return null;

                Scene loaded = SceneManager.GetSceneByName(sceneName);
                if (loaded.isLoaded)
                    SceneManager.SetActiveScene(loaded);
            }

            _currentRoomScene = sceneName;
            onComplete?.Invoke();
        }

        /// <summary>Unload a specific room scene.</summary>
        public IEnumerator UnloadRoomAsync(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded) yield break;

            var unloadOp = SceneManager.UnloadSceneAsync(scene);
            if (unloadOp == null) yield break;

            while (!unloadOp.isDone)
                yield return null;

            if (_currentRoomScene == sceneName)
                _currentRoomScene = null;
        }

        /// <summary>
        /// Preload a room in the background so it's ready when needed.
        /// Room is loaded but not activated until LoadRoomAsync is called.
        /// </summary>
        public void PreloadRoom(string sceneName)
        {
            _preloadQueue.Enqueue(sceneName);
            if (_preloadCoroutine == null)
                _preloadCoroutine = StartCoroutine(ProcessPreloadQueue());
        }

        private IEnumerator ProcessPreloadQueue()
        {
            while (_preloadQueue.Count > 0)
            {
                string sceneName = _preloadQueue.Dequeue();

                // Skip if already loaded or being loaded
                if (SceneManager.GetSceneByName(sceneName).isLoaded) continue;
                if (_pendingOperations.ContainsKey(sceneName)) continue;

                var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                if (loadOp == null) continue;

                loadOp.allowSceneActivation = false;
                _pendingOperations[sceneName] = loadOp;

                // Wait until loaded (0.9) but don't activate
                while (loadOp.progress < 0.9f)
                    yield return null;

                Debug.Log($"[RoomSceneManager] Preloaded: {sceneName}");
            }
            _preloadCoroutine = null;
        }

        /// <summary>Activate a preloaded room instantly.</summary>
        public void ActivatePreloadedRoom(string sceneName)
        {
            if (_pendingOperations.TryGetValue(sceneName, out AsyncOperation op))
            {
                op.allowSceneActivation = true;
                _pendingOperations.Remove(sceneName);

                Scene loaded = SceneManager.GetSceneByName(sceneName);
                if (loaded.isLoaded)
                {
                    SceneManager.SetActiveScene(loaded);
                    _currentRoomScene = sceneName;
                }
            }
        }

        /// <summary>Check if a room is currently loaded.</summary>
        public bool IsRoomLoaded(string sceneName)
            => SceneManager.GetSceneByName(sceneName).isLoaded;

        /// <summary>Get the currently active room scene name.</summary>
        public string CurrentRoom => _currentRoomScene;
    }
}