using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    /// <summary>
    /// Lightweight runtime service registry. Registration is explicit and all stale Unity
    /// object references are discarded when resolved.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        public static int Count => Services.Count;

        public static void Register<T>(T instance) where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type key = typeof(T);
            if (Services.TryGetValue(key, out object existing))
            {
                if (ReferenceEquals(existing, instance))
                    return;

                Debug.LogWarning($"[ServiceLocator] Replacing registered service {key.Name}.");
            }

            Services[key] = instance;
        }

        public static void Unregister<T>(T instance) where T : class
        {
            Type key = typeof(T);
            if (Services.TryGetValue(key, out object existing) && ReferenceEquals(existing, instance))
                Services.Remove(key);
        }

        public static bool IsRegistered<T>() where T : class
        {
            return TryGet(out T _);
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            Type key = typeof(T);
            if (!Services.TryGetValue(key, out object value))
            {
                service = null;
                return false;
            }

            // Unity keeps a managed shell after a UnityEngine.Object is destroyed.
            if (value is UnityEngine.Object unityObject && unityObject == null)
            {
                Services.Remove(key);
                service = null;
                return false;
            }

            service = value as T;
            return service != null;
        }

        public static T Get<T>() where T : class
        {
            TryGet(out T service);
            return service;
        }

        public static T Require<T>() where T : class
        {
            if (TryGet(out T service))
                return service;

            throw new InvalidOperationException(
                $"[ServiceLocator] Required service {typeof(T).Name} is not registered.");
        }

        public static bool Validate(params Type[] requiredTypes)
        {
            if (requiredTypes == null)
                throw new ArgumentNullException(nameof(requiredTypes));

            bool valid = true;
            foreach (Type type in requiredTypes)
            {
                if (type == null || !Services.ContainsKey(type))
                {
                    Debug.LogError($"[ServiceLocator] Missing required service: {type?.Name ?? "<null>"}");
                    valid = false;
                }
            }

            return valid;
        }

        public static void Reset()
        {
            Services.Clear();
        }
    }
}
