using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// This is a base class for MonoBehaviours that need to run Coroutines in both Edit and Play modes.
    /// </summary>
    [ExecuteInEditMode]
    public class EditPlayBridge : MonoBehaviour
    {
#if !UNITY_EDITOR
        /// <summary>
        /// Starts a coroutine in Play mode using Unity's standard coroutine system.
        /// This simplified version is used in builds where Edit mode functionality is not needed.
        /// </summary>
        /// <param name="coroutine">The coroutine to start.</param>
        protected new void StartCoroutine(IEnumerator coroutine)
        {
            base.StartCoroutine(coroutine);
        }
#else
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

                //
                stack.Pop();
                return stack.Count > 0;
            }
        }

        /// <summary>
        /// Starts a coroutine in either Play or Edit mode, using the appropriate system for each context.
        /// In Play mode, uses Unity's standard coroutine system.
        /// In Edit mode, uses a custom implementation based on EditorApplication.update.
        /// </summary>
        /// <param name="coroutine">The coroutine to start.</param>
        protected new void StartCoroutine(IEnumerator coroutine)
        {
            if (Application.isPlaying)
            {
                base.StartCoroutine(coroutine);
            }
            else
            {
                if (coroutine != null)
                {
                    var wrapper = new CoroutineWrapper(coroutine);
                    EditorCoroutineUtility.StartCoroutineOwnerless(EditorUpdateCoroutine(wrapper));
                }
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
#endif
    }
}