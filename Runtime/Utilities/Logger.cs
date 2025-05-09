using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Convenience extension to simplify usage of the Logger class.
    /// Provides a shorter class name (Log) with the same functionality as Logger.
    /// This allows for more concise code when logging messages: Log.Debug() instead of Logger.Debug().
    /// </summary>
    public abstract class Log : Logger
    {
        // NO-OP
    }

    /// <summary>
    /// Simple wrapper for Unity's logger to prevent UnityEngine inclusion in core Urt3d classes.
    /// Provides static methods to log different types of messages to the Unity console.
    /// </summary>
    [ExecuteInEditMode]
    public class Logger : EditPlayBridge
    {
        /// <summary>
        /// Logs an informational message to the Unity console.
        /// Equivalent to calling UnityEngine.Debug.Log().
        /// </summary>
        /// <param name="msg">The message to log</param>
        public static void Debug(string msg)
        {
            if (StateData.Instance.IsPlaying)
            {
                Instance.EnqueueMessage(Instance.msgDebug, msg);
            }
            else
            {
                UnityEngine.Debug.Log(msg);
            }
        }

        /// <summary>
        /// Logs a warning message to the Unity console.
        /// Equivalent to calling UnityEngine.Debug.LogWarning().
        /// </summary>
        /// <param name="msg">The warning message to log</param>
        public static void Warning(string msg)
        {
            if (StateData.Instance.IsPlaying)
            {
                Instance.EnqueueMessage(Instance.msgWarning, msg);
            }
            else
            {
                UnityEngine.Debug.LogWarning(msg);
            }
        }

        /// <summary>
        /// Logs an error message to the Unity console.
        /// Equivalent to calling UnityEngine.Debug.LogError().
        /// </summary>
        /// <param name="msg">The error message to log</param>
        public static void Error(string msg)
        {
            if (StateData.Instance.IsPlaying)
            {
                Instance.EnqueueMessage(Instance.msgError, msg);
            }
            else
            {
                UnityEngine.Debug.LogError(msg);
            }
        }

        /// <summary>
        /// Logs an exception to the Unity console with stack trace information.
        /// Equivalent to calling UnityEngine.Debug.LogException().
        /// </summary>
        /// <param name="msg">The exception message to log</param>
        public static void Exception(string msg)
        {
            if (StateData.Instance.IsPlaying)
            {
                Instance.EnqueueMessage(Instance.msgException, new Exception(msg));
            }
            else
            {
                UnityEngine.Debug.LogException(new Exception(msg));
            }
        }

        /// <summary>
        /// Logs an exception to the Unity console with stack trace information.
        /// Equivalent to calling UnityEngine.Debug.LogException().
        /// </summary>
        /// <param name="err">The exception to log</param>
        public static void Exception(Exception err)
        {
            if (StateData.Instance.IsPlaying)
            {
                Instance.EnqueueMessage(Instance.msgException, err);
            }
            else
            {
                UnityEngine.Debug.LogException(err);
            }
        }

        #region Private

        /// <summary>
        /// Thread-safe singleton instance of the Logger class.
        /// Creates a new GameObject with Logger component if one doesn't exist.
        /// </summary>
        private static Logger Instance => Singleton.Instance<Logger>();

        /// <summary>
        /// Queue up messages so they only fire once per frame (rather than immediately).
        /// This results in dramatic runtime performance gains when lots of messages are logged.
        /// This implementation is thread-safe as it only modifies thread-local queues.
        /// </summary>
        private void Update()
        {
            while (GetQueueCount(msgDebug) > 0)
            {
                UnityEngine.Debug.Log(DequeueMessage(msgDebug));
            }
            while (GetQueueCount(msgWarning) > 0)
            {
                UnityEngine.Debug.LogWarning(DequeueMessage(msgWarning));
            }
            while (GetQueueCount(msgError) > 0)
            {
                UnityEngine.Debug.LogError(DequeueMessage(msgError));
            }
            while (GetQueueCount(msgException) > 0)
            {
                UnityEngine.Debug.LogException(DequeueMessage(msgException));
            }
        }

        // Thread-safe queues using lock-based synchronization
        private readonly Queue<string> msgDebug = new();
        private readonly Queue<string> msgWarning = new();
        private readonly Queue<string> msgError = new();
        private readonly Queue<Exception> msgException = new();
        private readonly object queueLock = new object();

        // Lock and add item to queue
        private void EnqueueMessage<T>(Queue<T> queue, T message)
        {
            lock (queueLock)
            {
                queue.Enqueue(message);
            }
        }

        // Lock and dequeue item from queue
        private T DequeueMessage<T>(Queue<T> queue)
        {
            lock (queueLock)
            {
                return queue.Dequeue();
            }
        }

        // Lock and check queue count
        private int GetQueueCount<T>(Queue<T> queue)
        {
            lock (queueLock)
            {
                return queue.Count;
            }
        }

        #endregion
    }
}
