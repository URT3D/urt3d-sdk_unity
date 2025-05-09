using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Singleton MonoBehaviour that provides a centralized way to register callbacks
    /// for Unity's Update and LateUpdate lifecycle methods without creating additional GameObjects.
    /// </summary>
    public class Lifecycle : EditPlayBridge
    {
        public delegate void Delegate();

        #region Public

        /// <summary>
        /// Registers a callback to be executed during each Update cycle.
        /// </summary>
        /// <param name="del">The delegate to call during Update.</param>
        public static void OnUpdate(Delegate del)
        {
            Instance._onUpdate.Add(del);
        }

        /// <summary>
        /// Unregisters a previously registered Update callback.
        /// </summary>
        /// <param name="del">The delegate to remove from Update callbacks.</param>
        public static void OnUpdateRemove(Delegate del)
        {
            Instance._onUpdate.Remove(del);
        }

        /// <summary>
        /// Registers a callback to be executed during each LateUpdate cycle.
        /// </summary>
        /// <param name="del">The delegate to call during LateUpdate.</param>
        public static void OnLateUpdate(Delegate del)
        {
            Instance._onLateUpdate.Add(del);
        }

        /// <summary>
        /// Unregisters a previously registered LateUpdate callback.
        /// </summary>
        /// <param name="del">The delegate to remove from LateUpdate callbacks.</param>
        public static void OnLateUpdateRemove(Delegate del)
        {
            Instance._onLateUpdate.Remove(del);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return ((MonoBehaviour)Instance).StartCoroutine(routine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public static void StopCoroutine(Coroutine routine)
        {
            ((MonoBehaviour)Instance).StopCoroutine(routine);
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Singleton instance of the Lifecycle class.
        /// Creates a new GameObject with Lifecycle component if one doesn't exist.
        /// </summary>
        private static Lifecycle Instance => Singleton.Instance<Lifecycle>();

        /// <summary>
        /// Unity's Update lifecycle method. Invokes all registered Update callbacks.
        /// Automatically removes any callbacks that throw exceptions.
        /// </summary>
        private void Update()
        {
            foreach (var del in _onUpdate)
            {
                try
                {
                    del.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    _onUpdate.Remove(del);
                }
            }
        }

        /// <summary>
        /// Unity's LateUpdate lifecycle method. Invokes all registered LateUpdate callbacks.
        /// Automatically removes any callbacks that throw exceptions.
        /// </summary>
        private void LateUpdate()
        {
            foreach (var del in _onLateUpdate)
            {
                try
                {
                    del.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    _onLateUpdate.Remove(del);
                }
            }
        }

        /// <summary>
        /// Collection of delegates registered for 'update'.
        /// </summary>
        private readonly HashSet<Delegate> _onUpdate = new();

        /// <summary>
        /// Collection of delegates registered for 'late update'.
        /// </summary>
        private readonly HashSet<Delegate> _onLateUpdate = new();

        #endregion
    }
}
