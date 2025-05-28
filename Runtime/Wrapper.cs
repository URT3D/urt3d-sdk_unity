using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Urt3d.Utilities;
#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// A MonoBehaviour wrapper for URT3D Asset objects that facilitates loading and managing 
    /// URT3D Assets within the Unity hierarchy. This class handles the lifecycle of URT3D Assets, 
    /// including loading, instantiation, parenting to GameObjects, and proper cleanup.
    /// </summary>
    [ExecuteInEditMode]
    [SuppressMessage("ReSharper", "InvertIf")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "ConvertToAutoPropertyWithPrivateSetter")]
    public class Wrapper : MonoBehaviour
    {
        #region Public Enumerations
        
        /// <summary>
        /// Defines the method used to load and identify the URT3D object.
        /// This enum determines whether the object is loaded from a file path or by GUID reference.
        /// </summary>
        public enum ModeType
        {
            /// <summary>Load the URT3D object from a file path on disk.</summary>
            Local_File = 0,
            
            /// <summary>Load the URT3D object using its GUID from the CDN.</summary>
            Remote_Guid = 1
        }
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// 
        /// </summary>
        public string Json
        {
            get => _json;
            set
            {
                _json = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        /// <summary>
        /// Specifies the loading mode for the URT3D object.
        /// Controls whether the object is loaded from a file path or by GUID.
        /// Default at edit-time is Local File. Run-time always uses Remote Guid.
        /// </summary>
        public ModeType Mode
        {
#if UNITY_EDITOR
            get
            {
                if (_mode < 0)
                {
                    Mode = ModeType.Local_File;
                }
                return (ModeType)_mode;
            }
            set
            {
                if ((ModeType)_mode != value)
                {
                    _mode = (int)value;
                    EditorUtility.SetDirty(this);
                }
            }
#else
            get => (ModeType)_mode;
            set => _mode = (int)value;     
#endif
        }

        /// <summary>
        /// Gets or sets the Definition GUID of the URT3D Asset to load.
        /// When in GUID mode, this property is used to identify and load the object.
        /// The GUID is validated to ensure it meets Urt3d requirements.
        /// </summary>
        public Guid Guid
        {
            get => Guid.TryParse(_guidUrt3d, out var guid) ? guid : Guid.Empty;
            private set
            {
                // Set the string GUID if changed
                var guidString = value.ToString();
                if (_guidUrt3d != guidString)
                {
                    _guidUrt3d = guidString;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
            }
        }

        /// <summary>
        /// Attempt to extract the object path using the Unity asset guid.
        /// If the Unity asset guid (or associated asset) is invalid, just use the raw path.
        /// </summary>
        public string Path
        {
#if UNITY_EDITOR
            get
            {
                if (AssetDatabase.AssetPathExists(PathRaw))
                {
                    _guidUnity = AssetDatabase.AssetPathToGUID(PathRaw);
                }
                var path = AssetDatabase.GUIDToAssetPath(_guidUnity);
                return string.IsNullOrEmpty(path) ? PathRaw : path;
            }
            set
            {
                if (PathRaw != value)
                {
                    PathRaw = value;
                    EditorUtility.SetDirty(this);
                }
            }
#else
            get => PathRaw;
            set => PathRaw = value;
#endif
        }

        /// <summary>
        /// The importer may only write to a regular public field.
        /// It will fail if this is a method or property with get/set operations.
        /// </summary>
        public string PathRaw = null; // TODO

        /// <summary>
        /// Gets the currently loaded URT3D Asset instance.
        /// This property provides access to the URT3D Asset that this wrapper is managing.
        /// </summary>
        public Asset Asset => _asset;
        
        /// <summary>
        /// Gets or sets the execution mode for scripts
        /// </summary>
        public Scripting.ScriptExecutionMode ExecutionMode
        {
            get => _executionMode;
            set => _executionMode = value;
        }
        
        #endregion

        #region Private Fields
        
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _json = ""; // Non-default value to force Unity serialization
        
        /// <summary>
        /// Cache the Unity asset guid in case the asset is moved within the Unity project.
        /// This is saved to the Unity scene file (instead of the main Path property).
        /// </summary>
        [SerializeField]
        private string _guidUnity = ""; // Non-default value to force Unity serialization

        /// <summary>
        /// This is saved to the Unity scene file (instead of the main Guid property).
        /// </summary>
        [SerializeField]
        private string _guidUrt3d = ""; // Non-default value to force Unity serialization

        /// <summary>
        /// This is saved to the Unity scene file (instead of the main Mode property).
        /// </summary>
        [SerializeField]
        private int _mode = -1; // Non-default value to force Unity serialization
        
        /// <summary>
        /// The loaded Asset instance.
        /// </summary>
        private Asset _asset = null;

        /// <summary>
        /// Indicates whether a URT3D object is currently being loaded.
        /// This property prevents multiple concurrent load operations.
        /// </summary>
        private bool _isLoading = false;

        /// <summary>
        /// Indicates whether the Start method has been called.
        /// This property is used to ensure loading operations don't proceed until the component is properly initialized.
        /// </summary>
        private bool _hasStarted = false;
        
        /// <summary>
        /// When scripts should execute
        /// </summary>
        [SerializeField]
        private Scripting.ScriptExecutionMode _executionMode = Scripting.ScriptExecutionMode.Both;
        
        /// <summary>
        /// Script engine for running MiniScript code
        /// </summary>
        private Scripting.UrtScriptEngine _scriptEngine;
        
        /// <summary>
        /// Flag indicating if scripts are ready to run
        /// </summary>
        private bool _scriptsReady = false;

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Reloads the URT3D object based on the current Mode setting.
        /// This method will destroy any existing object and load a new one
        /// using either the file path or GUID, depending on the current Mode.
        /// In editor builds, both File and Guid modes are supported, but in runtime
        /// builds, only Guid mode is used.
        /// </summary>
        public void Reload()
        {
#if UNITY_EDITOR
            switch (Mode)
            {
                case ModeType.Local_File:
                    EditorCoroutineUtility.StartCoroutine(Load(Path), this);
                    return;

                case ModeType.Remote_Guid:
                    EditorCoroutineUtility.StartCoroutine(Load(Guid), this);
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
#else
            // Load via GUID in builds
            StartCoroutine(Load(Guid));
#endif
        }
        
        #endregion

        #region Script Execution Methods
        
        /// <summary>
        /// Initialize the script engine
        /// </summary>
        /// <returns>True if initialization was successful</returns>
        private bool InitializeScriptEngine()
        {
            if (_scriptEngine != null && _scriptsReady)
            {
                return true;
            }
            
            if (_asset == null)
            {
                Debug.LogWarning("Cannot initialize script engine: Asset is not loaded");
                return false;
            }
            
            try
            {
                _scriptEngine = new Scripting.UrtScriptEngine(_asset);
                _scriptEngine.OnScriptCompleted += HandleScriptCompleted;
                _scriptEngine.OnScriptOutput += HandleScriptOutput;
                _scriptsReady = true;
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize script engine: {ex.Message}");
                _scriptEngine = null;
                _scriptsReady = false;
                return false;
            }
        }
        
        /// <summary>
        /// Run a specific script
        /// </summary>
        /// <param name="script">The script to run</param>
        /// <returns>True if the script started executing</returns>
        public bool RunScriptReference(Scripting.Script script)
        {
            if (script == null)
            {
                Debug.LogError("Cannot run script: Script is null");
                return false;
            }
            
            if (!_scriptsReady && !InitializeScriptEngine())
            {
                Debug.LogWarning("Cannot run script: Script engine is not initialized");
                return false;
            }
            
            if (!ShouldExecuteInCurrentMode())
            {
                Debug.LogWarning("Cannot run script: Script execution is not enabled in current mode");
                return false;
            }
            
            try
            {
                return _scriptEngine.ExecuteScript(script);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error executing script: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Run scripts by trigger type
        /// </summary>
        /// <param name="triggerType">The trigger type to run</param>
        public void RunScriptsByTrigger(Scripting.ScriptTriggerType triggerType)
        {
            if (_asset == null || _asset.Scripts == null || _asset.Scripts.Count == 0)
            {
                return;
            }
            
            if (!_scriptsReady && !InitializeScriptEngine())
            {
                Debug.LogWarning("Cannot run scripts: Script engine is not initialized");
                return;
            }
            
            if (!ShouldExecuteInCurrentMode())
            {
                Debug.LogWarning("Cannot run scripts: Script execution is not enabled in current mode");
                return;
            }
            
            try
            {
                _scriptEngine.ExecuteScriptsByTrigger(_asset.Scripts.ToList(), triggerType);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error executing scripts: {ex.Message}");
                _scriptsReady = false;
                _scriptEngine = null;
            }
        }
        
        /// <summary>
        /// Trigger a custom event
        /// </summary>
        /// <param name="eventName">The name of the custom event</param>
        public void TriggerCustomEvent(string eventName)
        {
            if (_asset == null || _asset.Scripts == null || _asset.Scripts.Count == 0)
            {
                return;
            }
            
            if (!_scriptsReady && !InitializeScriptEngine())
            {
                Debug.LogWarning("Cannot trigger event: Script engine is not initialized");
                return;
            }
            
            if (!ShouldExecuteInCurrentMode())
            {
                Debug.LogWarning("Cannot trigger event: Script execution is not enabled in current mode");
                return;
            }
            
            try
            {
                _scriptEngine.ExecuteScriptsByCustomEvent(_asset.Scripts.ToList(), eventName);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error triggering event: {ex.Message}");
                _scriptsReady = false;
                _scriptEngine = null;
            }
        }
        
        /// <summary>
        /// Stop all scripts
        /// </summary>
        public void StopScripts()
        {
            if (!_scriptsReady || _scriptEngine == null)
            {
                return;
            }
            
            _scriptEngine.StopAllScripts();
        }
        
        /// <summary>
        /// Handle script completion
        /// </summary>
        /// <param name="scriptId">The ID of the script</param>
        /// <param name="success">Whether the script completed successfully</param>
        /// <param name="errorMessage">Error message if any</param>
        private void HandleScriptCompleted(Guid guid, bool success, string errorMessage)
        {
            if (!success && !string.IsNullOrEmpty(errorMessage))
            {
                Debug.LogError($"Script execution failed: {errorMessage}");
            }
        }
        
        /// <summary>
        /// Handle script output
        /// </summary>
        /// <param name="scriptId">The ID of the script</param>
        /// <param name="output">The output text</param>
        private void HandleScriptOutput(Guid guid, string output)
        {
            // Log output to console
            Debug.Log($"[Script Output] {output}");
        }
        
        /// <summary>
        /// Check if scripts should execute in the current mode
        /// </summary>
        /// <returns>True if scripts should execute, false otherwise</returns>
        private bool ShouldExecuteInCurrentMode()
        {
#if UNITY_EDITOR
            // In editor mode
            return _executionMode == Scripting.ScriptExecutionMode.EditorOnly ||
                   _executionMode == Scripting.ScriptExecutionMode.Both;
#else
            // In runtime mode
            return _executionMode == Scripting.ScriptExecutionMode.RuntimeOnly ||
                   _executionMode == Scripting.ScriptExecutionMode.Both;
#endif
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Loads a URT3D object using its GUID.
        /// This coroutine handles the asynchronous loading process when loading by GUID,
        /// validating the GUID and delegating to the shared loading logic.
        /// </summary>
        /// <param name="guid">The GUID of the URT3D object to load</param>
        /// <returns>An IEnumerator for coroutine execution</returns>
        private IEnumerator Load(Guid guid)
        {
            if (_isLoading) yield break;

            if (guid == Guid.Empty)
            {
                Log.Error("Invalid GUID, load via File Path to extract valid GUID");
                yield break;
            }

            _isLoading = true;
            yield return Load(Asset.Construct(guid));
            _isLoading = false;
        }

        /// <summary>
        /// Loads a URT3D object from a file path.
        /// This coroutine handles the asynchronous loading process when loading from a file,
        /// validating the file exists and delegating to the shared loading logic.
        /// </summary>
        /// <param name="path">The file path of the URT3D object to load</param>
        /// <returns>An IEnumerator for coroutine execution</returns>
        private IEnumerator Load(string path)
        {
            if (_isLoading) yield break;

            if (!File.Exists(path))
            {
                Log.Error($"Invalid file path: {path}, load via GUID to download file");
                yield break;
            }

            _isLoading = true;
            yield return Load(Asset.Construct(path));
            _isLoading = false;
        }

        /// <summary>
        /// Shared object loading logic that handles the Task-based construction process.
        /// </summary>
        /// <param name="task">The Task that's constructing the URT3D object</param>
        /// <returns>An IEnumerator for coroutine execution</returns>
        private IEnumerator Load(Task<Asset> task)
        {
            // Clear previous
            Asset?.Destroy();

            // Ensure a clean hierarchy
            foreach (Transform child in transform)
            {
                GameObjectUtils.Destroy(child.gameObject);
            }

            // Wait for start
            yield return new WaitUntil(() => _hasStarted);

            // Wait for construction
            yield return new WaitUntil(() => task.IsCompleted);

            // Validate result
            if (task.IsFaulted)
            {
                Log.Error($"Failed to construct URT3D Asset for {name}: {task.Exception.Message}");
                yield break;
            }

            // Validate instantiation
            _asset = task.Result;
            if (_asset == null)
            {
                Log.Error($"Failed to instantiate URT3D Asset for {name}: {task.Exception.Message}");
                yield break;
            }

            // Test for destruction
            if (this == null)
            {
                if (_asset != null)
                {
                    _asset.Destroy();
                    _asset = null;
                }
                yield break;
            }

            // Adjust hierarchy
            GameObject go = Asset.Actor;
            go.transform.SetParent(transform, false);

            // Update GUID property to match Asset definition
            Guid = Asset.Metadata.GuidDefinition;

            // Apply JSON to this Asset
            if (!string.IsNullOrEmpty(Json))
            {
                JsonUtil.FromJson(Json, _asset);
            }
            
            // Initialize script engine
            InitializeScriptEngine();
        }

        /// <summary>
        /// Clean up URT3D content.
        /// </summary>
        private void OnDestroy()
        {
            StopAllCoroutines();

            // Stop and cleanup scripts
            if (_scriptEngine != null)
            {
                _scriptEngine.StopAllScripts();
                _scriptEngine.OnScriptCompleted -= HandleScriptCompleted;
                _scriptEngine.OnScriptOutput -= HandleScriptOutput;
            }
            
            Asset?.Destroy();
        }
        
        /// <summary>
        /// Called upon creation and each time we enter or leave play mode.
        /// </summary>
        private void Start()
        {
            Reload();
            _hasStarted = true;
        }

        /// <summary>
        /// Validate hierarchy relative to URT3D assumptions.
        /// </summary>
        private void Update()
        {
            if (_lastChildCount == transform.childCount) return;
            _lastChildCount = transform.childCount;

            if (!_isLoading && transform.childCount != 1)
            {
                Log.Error($"Detected direct-modification of URT3D Asset wrapper: {name}, this is not allowed!\n" +
                          $" • If parenting is desired, use constraints rather than modifying the Actor/GameObject hierarchy.\n" +
                          $" • If destruction is desired, do so on the URT3D Asset itself (rather than on the Actor/GameObject).");
            }
        }
        private int _lastChildCount = 0;

        #endregion
    }
}