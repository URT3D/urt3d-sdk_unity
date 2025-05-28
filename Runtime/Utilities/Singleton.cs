using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Provides a generic singleton pattern implementation with thread safety.
    /// Supports both MonoBehaviour components and regular classes.
    /// </summary>
    public static class Singleton
    {
        private static readonly Dictionary<Type, object> s_cache = new();
        private static readonly object s_lock = new object();

        /// <summary>
        /// Gets or creates a thread-safe singleton instance of a non-MonoBehaviour class.
        /// </summary>
        /// <typeparam name="T">The class type to get as a singleton</typeparam>
        /// <returns>A singleton instance of the specified type</returns>
        public static T GetInstance<T>() where T : class, new()
        {
            // Use double-check locking pattern for thread safety
            if (!s_cache.TryGetValue(typeof(T), out var tmp) || tmp == null)
            {
                lock (s_lock) // Lock to ensure thread safety
                {
                    // Double-check after acquiring lock
                    if (!s_cache.TryGetValue(typeof(T), out tmp) || tmp == null)
                    {
                        var instance = new T();
                        s_cache[typeof(T)] = instance;
                        return instance;
                    }
                }
            }
            
            return (T)tmp;
        }

        /// <summary>
        /// Gets or creates a thread-safe singleton instance of the specified MonoBehaviour type.
        /// First checks the cache, then searches the scene, and finally creates a new instance if needed.
        /// </summary>
        /// <typeparam name="T">The MonoBehaviour type to get as a singleton</typeparam>
        /// <returns>A singleton instance of the specified type</returns>
        public static T Instance<T>() where T : MonoBehaviour
        {
            // Use double-check locking pattern for thread safety
            if (!s_cache.TryGetValue(typeof(T), out var tmp) || tmp == null)
            {
                lock (s_lock) // Lock to ensure thread safety
                {
                    // Double-check after acquiring lock
                    if (!s_cache.TryGetValue(typeof(T), out tmp) || tmp == null)
                    {
                        // Check hierarchy
                        var self = FindComponent<T>();
                        
                        if (self == null)
                        {
                            // Create new instance
                            var go = new GameObject(typeof(T).FullName);
                            go.transform.SetParent(Parent);
                            self = go.AddComponent<T>();
                        }
                        
                        s_cache[typeof(T)] = self;
                        return self;
                    }
                }
            }
            
            return (T)tmp;
        }

        /// <summary>
        /// Searches for an existing component of the specified type in the active scene.
        /// Checks all root GameObjects and their children.
        /// </summary>
        /// <typeparam name="T">The MonoBehaviour type to find</typeparam>
        /// <returns>The found component or null if none exists</returns>
        private static T FindComponent<T>() where T : MonoBehaviour
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.isLoaded)
            {
                Debug.LogWarning($"Active scene is not loaded: {scene}");
                return null;
            }

            var gos = scene.GetRootGameObjects();
            foreach (var go in gos)
            {
                var component = go.GetComponentInChildren<T>(true);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets or creates a parent Transform to organize all singleton instances in the hierarchy.
        /// Uses a dedicated GameObject named "URT3D Singletons" to keep the hierarchy clean.
        /// Thread-safe implementation with double-check locking.
        /// </summary>
        private static Transform Parent
        {
            get
            {
                if (s_parent != null)
                {
                    return s_parent;
                }

                lock (s_lock)
                {
                    if (s_parent != null)
                    {
                        return s_parent;
                    }

                    const string name = "URT3D Singletons";

                    // Check hierarchy (slower)
                    var go = GameObject.Find(name);
                    if (go != null)
                    {
                        s_parent = go.transform;
                        return s_parent;
                    }

                    // Create new instance
                    s_parent = new GameObject(name)
                    {
                        // Don't include Singleton in saved scenes
                        hideFlags = HideFlags.DontSave
                    }.transform;
                    return s_parent;
                }
            }
        }

        /// <summary>
        /// Initializes the singleton system by subscribing to application quit events.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // Subscribe to application quit event
            Application.quitting += () =>
            {
                // Clear all singletons on application quit
                lock (s_lock)
                {
                    s_cache.Clear();
                    s_parent = null;
                }
            };
        }

        private static Transform s_parent = null;
    }
}