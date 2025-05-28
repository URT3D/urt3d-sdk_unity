using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Miniscript;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;
using Urt3d.Utilities;
using Object = UnityEngine.Object;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Manages MiniScript interpreters for URT3D scripting
    /// </summary>
    public class UrtScriptEngine
    {
        /// <summary>
        /// Default timeout for script execution (in seconds)
        /// </summary>
        private const double DefaultTimeout = 5.0;

        /// <summary>
        /// The URT3D asset this engine is attached to
        /// </summary>
        private readonly object _targetAsset;

        /// <summary>
        /// Bridge to expose URT3D APIs to MiniScript
        /// </summary>
        private readonly UrtScriptBridge _bridge;

        /// <summary>
        /// Dictionary of currently running scripts, keyed by script ID
        /// </summary>
        private readonly Dictionary<Guid, Interpreter> _interpreters = new();

        /// <summary>
        /// Queue of scripts waiting for execution
        /// </summary>
        private readonly ConcurrentQueue<Script> _scriptQueue = new ConcurrentQueue<Script>();

        /// <summary>
        /// Dictionary of script execution results, keyed by script ID
        /// </summary>
        private readonly Dictionary<Guid, string> _scriptResults = new();

        /// <summary>
        /// Event raised when a script completes execution
        /// </summary>
        public event Action<Guid, bool, string> OnScriptCompleted;

        /// <summary>
        /// Event raised when a script has output
        /// </summary>
        public event Action<Guid, string> OnScriptOutput;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="targetAsset">The URT3D asset this engine is attached to</param>
        public UrtScriptEngine(object targetAsset)
        {
            _targetAsset = targetAsset;
            _bridge = new UrtScriptBridge(targetAsset);
        }

        /// <summary>
        /// Execute a script
        /// </summary>
        /// <param name="script">The script to execute</param>
        /// <returns>True if script execution started successfully, false otherwise</returns>
        public bool ExecuteScript(Script script)
        {
            if (script is not { Enabled: true })
            {
                Debug.LogWarning("[URT3D] Script is null or disabled");
                return false;
            }

            if (_targetAsset == null)
            {
                Debug.LogWarning("[URT3D] Target asset is null");
                return false;
            }

            Debug.Log($"[URT3D] Target Asset: {_targetAsset.GetType().Name}");

            try
            {
                // Get script content
                if (string.IsNullOrEmpty(script.ScriptContent))
                {
                    Debug.LogWarning("[URT3D] Script is empty");
                    return false;
                }

                // Create a new interpreter for this script
                var interpreter = new Interpreter(script.ScriptContent)
                {
                    // Set up standard output handler
                    standardOutput = (output) => HandleScriptOutput(script.Guid, output), // Set up error output handler
                    errorOutput = (error) => HandleScriptError(script.Guid, error)
                };

                // Store the interpreter
                _interpreters[script.Guid] = interpreter;

                // Compile the script
                interpreter.Compile(); ;

                var scriptObject = new GameObject("ScriptRunner");

#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(RunScriptAsync(script, interpreter, scriptObject), scriptObject);
#else
                Lifecycle.StartCoroutine(RunScriptAsync(script, interpreter, scriptObject));
#endif
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error executing script {script.Name}: {ex.Message}");
                Debug.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Run a script asynchronously
        /// </summary>
        /// <param name="script">The script to run</param>
        /// <param name="interpreter">The MiniScript interpreter</param>
        /// <param name="bridge"></param>
        private IEnumerator RunScriptAsync(Script script, Interpreter interpreter, GameObject bridge)
        {
            Debug.Log($"[URT3D] Running script {script.Name} with timeout {DefaultTimeout} seconds");
            while (!interpreter.done)
            {
                // Run the script with default timeout
                interpreter.RunUntilDone(DefaultTimeout);
                yield return null;
            }

            // Destroy the bridge
            GameObjectUtils.Destroy(bridge);

            // Remove the interpreter
            _interpreters.Remove(script.Guid);

            OnScriptCompleted?.Invoke(script.Guid, true, null);
        }

        /// <summary>
        /// Handle script output
        /// </summary>
        /// <param name="scriptId">The ID of the script</param>
        /// <param name="output">The output text</param>
        private void HandleScriptOutput(Guid guid, string output)
        {
            Debug.Log($"[URT3D] HandleScriptOutput called with: {output}");

            if (!string.IsNullOrEmpty(output))
            {
                OnScriptOutput?.Invoke(guid, output);

                // Store the result
                if (_scriptResults.ContainsKey(guid))
                {
                    _scriptResults[guid] += output + "\n";
                }
                else
                {
                    _scriptResults[guid] = output + "\n";
                }

                // Log the output
                Debug.Log($"[MiniScript] {output}");
            }
        }

        /// <summary>
        /// Handle script errors
        /// </summary>
        /// <param name="scriptId">The ID of the script</param>
        /// <param name="error">The error message</param>
        private void HandleScriptError(Guid guid, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                // Log the error
                Debug.LogError($"[MiniScript] {error}");

                // Store the error
                if (_scriptResults.ContainsKey(guid))
                {
                    _scriptResults[guid] += "ERROR: " + error + "\n";
                }
                else
                {
                    _scriptResults[guid] = "ERROR: " + error + "\n";
                }
            }
        }

        /// <summary>
        /// Execute all scripts of a specific trigger type
        /// </summary>
        /// <param name="scripts">The list of scripts to check</param>
        /// <param name="triggerType">The trigger type to execute</param>
        public void ExecuteScriptsByTrigger(List<Script> scripts, ScriptTriggerType triggerType)
        {
            foreach (var script in scripts)
            {
                if (script.Enabled && script.TriggerType == triggerType)
                {
                    ExecuteScript(script);
                }
            }
        }

        /// <summary>
        /// Execute scripts triggered by a custom event
        /// </summary>
        /// <param name="scripts">The list of scripts to check</param>
        /// <param name="eventName">The name of the custom event</param>
        public void ExecuteScriptsByCustomEvent(List<Script> scripts, string eventName)
        {
            foreach (var script in scripts)
            {
                if (script.Enabled &&
                    script.TriggerType == ScriptTriggerType.OnCustomEvent &&
                    script.CustomEventName == eventName)
                {
                    ExecuteScript(script);
                }
            }
        }

        /// <summary>
        /// Stop a specific script by ID
        /// </summary>
        /// <param name="scriptId">The ID of the script to stop</param>
        /// <returns>True if the script was stopped, false if not found</returns>
        public bool StopScript(Guid guid)
        {
            if (_interpreters.TryGetValue(guid, out var interpreter))
            {
                interpreter.Stop();
                _interpreters.Remove(guid);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Stop all running scripts
        /// </summary>
        public void StopAllScripts()
        {
            foreach (var interpreter in _interpreters.Values)
            {
                interpreter.Stop();
            }
            _interpreters.Clear();
        }

        /// <summary>
        /// Get the execution result of a script
        /// </summary>
        /// <param name="scriptId">The ID of the script</param>
        /// <returns>The execution result or null if not found</returns>
        public string GetScriptResult(Guid guid)
        {
            if (_scriptResults.TryGetValue(guid, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Clear the result of a script
        /// </summary>
        /// <param name="scriptId">The ID of the script</param>
        public void ClearScriptResult(Guid guid)
        {
            if (_scriptResults.ContainsKey(guid))
            {
                _scriptResults.Remove(guid);
            }
        }

        /// <summary>
        /// Clear all script results
        /// </summary>
        public void ClearAllScriptResults()
        {
            _scriptResults.Clear();
        }
    }
}
