using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

namespace Urt3d.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class GameObjectUtils
    {
        #region Coroutine

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coroutine"></param>
        /// <param name="monoBehaviour"></param>
        public static Coroutine StartCoroutine(IEnumerator coroutine, MonoBehaviour monoBehaviour = null)
        {
            if (Application.isPlaying)
            {
                if (monoBehaviour != null)
                {
                    return monoBehaviour.StartCoroutine(coroutine);
                }
                Log.Error($"Runtime coroutines require a parent MonoBehavior: {coroutine}");
                return null;
            }

            var wrapper = new CoroutineWrapper(coroutine);
            if (monoBehaviour == null)
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(EditorUpdateCoroutine(wrapper));
                return null;
            }
            EditorCoroutineUtility.StartCoroutine(EditorUpdateCoroutine(wrapper), monoBehaviour);
            return null;
        }

        #endregion

        #region Destroy

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public static void Destroy(Component component)
        {
            if (component == null) return;

            if (Application.isPlaying)
            {
                GameObject.Destroy(component);
            }
            else
            {
                GameObject.DestroyImmediate(component);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        public static void Destroy(GameObject gameObject)
        {
            if (gameObject == null) return;

            if (Application.isPlaying)
            {
                GameObject.Destroy(gameObject);
            }
            else
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        public static void Destroy(Texture2D texture)
        {
            if (texture == null) return;

            if (Application.isPlaying)
            {
                GameObject.Destroy(texture);
            }
            else
            {
                GameObject.DestroyImmediate(texture);
            }
        }

        #endregion

        #region

        /// <summary>
        /// Internal wrapper class that manages coroutine execution in Edit mode.
        /// Implements a custom coroutine system that mimics Unity's native coroutine behavior.
        /// </summary>
        private class CoroutineWrapper
        {
            /// <summary>
            /// Stack of currently executing coroutines and nested coroutines.
            /// Used to maintain proper execution order and nesting of coroutines.
            /// </summary>
            private Stack<IEnumerator> stack = new();

            /// <summary>
            /// Initializes a new instance of the CoroutineWrapper class.
            /// </summary>
            /// <param name="routine">The initial coroutine to wrap.</param>
            public CoroutineWrapper(IEnumerator routine)
            {
                stack.Push(routine);
            }

            /// <summary>
            /// Advances the coroutine execution by one step.
            /// Handles nested coroutines by pushing them onto the stack when encountered.
            /// </summary>
            /// <returns>True if the coroutine is still running, false if it has completed.</returns>
            public bool MoveNext()
            {
                if (stack.Count == 0)
                {
                    return false;
                }

                var top = stack.Peek();

                if (top.MoveNext())
                {
                    if (top.Current is IEnumerator nested)
                    {
                        stack.Push(nested);
                    }
                    return true;
                }

                stack.Pop();
                return stack.Count > 0;
            }
        }

        /// <summary>
        /// Updates all active coroutines in Edit mode.
        /// This method is registered with EditorApplication.update when coroutines are active.
        /// It advances each coroutine by one step and removes completed coroutines.
        /// </summary>
        private static IEnumerator EditorUpdateCoroutine(CoroutineWrapper wrapper)
        {
            while (true)
            {
                if (wrapper == null || !wrapper.MoveNext())
                {
                    yield break;
                }
                yield return new EditorWaitForSeconds(1f / 60f);
            }
        }

        #endregion
    }
}