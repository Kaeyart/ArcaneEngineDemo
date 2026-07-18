using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    /// <summary>
    /// Centralized service locator — replaces scattered .Instance singletons with a single
    /// lookup point. Services register themselves in Awake() and unregister in OnDestroy().
    /// Other components resolve dependencies via Get{T}().
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>Register a service instance. Call in Awake().</summary>
        public static void Register<T>(T instance) where T : class
        {
            Type key = typeof(T);
            if (_services.ContainsKey(key))
            {
                Debug.LogWarning($"[ServiceLocator] {key.Name} already registered — replacing.");
                _services.Remove(key);
            }
            _services[key] = instance;
        }

        /// <summary>Unregister a service. Call in OnDestroy().</summary>
        public static void Unregister<T>(T instance) where T : class
        {
            Type key = typeof(T);
            if (_services.TryGetValue(key, out object existing) && existing == instance)
                _services.Remove(key);
        }

        /// <summary>Resolve a service. Returns null if not registered.</summary>
        public static T Get<T>() where T : class
        {
            Type key = typeof(T);
            _services.TryGetValue(key, out object service);
            return service as T;
        }

        /// <summary>Resolve a required service. Throws if not found.</summary>
        public static T Require<T>() where T : class
        {
            T service = Get<T>();
            if (service == null)
                throw new InvalidOperationException($"[ServiceLocator] Required service {typeof(T).Name} is not registered.");
            return service;
        }

        /// <summary>Validate that all expected services are registered. Call after boot.</summary>
        public static bool Validate(params Type[] requiredTypes)
        {
            bool valid = true;
            foreach (Type type in requiredTypes)
            {
                if (!_services.ContainsKey(type))
                {
                    Debug.LogError($"[ServiceLocator] Missing required service: {type.Name}");
                    valid = false;
                }
            }
            return valid;
        }

        /// <summary>Clear all registrations (for test teardown).</summary>
        public static void Reset()
        {
            _services.Clear();
        }
    }
}