using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Urt3d.Scripting;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Editor
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Specialized inspector class responsible for drawing the Scripts section in the URT3D Manager.
    /// Extends DrawAsset to utilize common parameter rendering functionality.
    /// Manages the rendering and interaction of scripts for URT3D instances, allowing for
    /// script creation, editing, and execution directly from the Unity Editor.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DrawAssetScripts : DrawAsset
    {
        /// <summary>
        /// Dictionary to track the expanded/collapsed state of scripts foldouts for each URT3D instance.
        /// The key is the URT3D instance's unique GuidInstance.
        /// </summary>
        private readonly Dictionary<Guid, bool> _scriptsFoldouts = new();

        // UI state for script editing
        private ReorderableList _scriptsList;
        private int _selectedScriptIndex = -1;
        private string _currentScriptContent = string.Empty;
        private Vector2 _scriptListScrollPosition = Vector2.zero;
        private Vector2 _scriptContentScrollPosition = Vector2.zero;
        private bool _showScriptContent = true;
        private bool _showScriptSettings = true;
        private ScriptReference _currentScript;
        private Asset _targetAsset;

        // Style cache
        private GUIStyle _headerStyle;
        private GUIStyle _codeStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _foldoutStyle;

        /// <summary>
        /// Wrapper class to adapt Script to ScriptReference interface
        /// </summary>
        private class ScriptReference
        {
            private readonly Script _script;

            public ScriptReference(Script script)
            {
                _script = script;
            }

            public string Name 
            { 
                get => _script.Name;
                set => _script.Name = value;
            }
            public bool Enabled
            {
                get => _script.Enabled;
                set => _script.Enabled = value;
            }
            public ScriptTriggerType TriggerType
            {
                get => _script.TriggerType;
                set => _script.TriggerType = value;
            }
            public string CustomEventName
            {
                get => _script.CustomEventName;
                set => _script.CustomEventName = value;
            }

            public string ScriptContent
            {
                get => _script.ScriptContent;
                set => _script.ScriptContent = value;
            }

            public Script UnderlyingScript => _script;
        }

        /// <summary>
        /// Draws the Scripts section for a specific URT3D instance.
        /// Displays a foldout to expand/collapse the entire scripts section.
        /// When expanded, displays the script list, editor, and controls for managing scripts.
        /// </summary>
        /// <param name="guid">The unique identifier of the URT3D instance.</param>
        /// <param name="asset">The URT3D instance whose scripts are to be displayed.</param>
        /// <param name="wrapper">The wrapper component that contains this asset (for script operations).</param>
        public void DrawScripts(Guid guid, Asset asset, WrapperEditor wrapper)
        {
            // Initialize styles
            InitializeStyles();

            // Initialize foldout state for this URT3D instance if it doesn't exist
            if (!_scriptsFoldouts.ContainsKey(guid))
            {
                _scriptsFoldouts[guid] = false;
            }

            // Draw the scripts foldout
            _scriptsFoldouts[guid] = EditorGUILayout.Foldout(_scriptsFoldouts[guid], "Scripts", true, _foldoutStyle);

            // Only draw the contents if the foldout is expanded
            if (_scriptsFoldouts[guid])
            {
                // Set the target asset
                _targetAsset = asset;

                // Initialize the scripts list if needed
                if (_scriptsList == null)
                {
                    InitializeScriptsList();
                }

                // Increase indentation for nested content
                EditorGUI.indentLevel++;

                // Start drawing the scripts section
                EditorGUILayout.BeginVertical("box");

                // Draw the execution mode selector (if SerializedObject is available from wrapper)
                var serializedObject = wrapper?.serializedObject;
                if (serializedObject != null)
                {
                    serializedObject.Update();
                    var executionModeProperty = serializedObject.FindProperty("_executionMode");
                    if (executionModeProperty != null)
                    {
                        EditorGUILayout.PropertyField(
                            executionModeProperty,
                            new GUIContent("Execution Mode", "When scripts should be allowed to execute"));
                    }
                    serializedObject.ApplyModifiedProperties();
                }

                EditorGUILayout.Space();

                // Draw the scripts list
                _scriptListScrollPosition = EditorGUILayout.BeginScrollView(_scriptListScrollPosition,
                    GUILayout.Height(150));
                if (_scriptsList != null)
                {
                    _scriptsList.DoLayoutList();
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();

                // If we have a selected script, show its details
                if (_currentScript != null)
                {
                    // Draw the script settings section
                    _showScriptSettings = EditorGUILayout.Foldout(_showScriptSettings, "Script Settings", true, _foldoutStyle);
                    if (_showScriptSettings)
                    {
                        EditorGUILayout.BeginVertical("box");

                        // Script name
                        var newName = EditorGUILayout.TextField("Name", _currentScript.Name);
                        if (newName != _currentScript.Name)
                        {
                            _currentScript.Name = newName;
                            SetDirty();
                        }

                        // Script enabled toggle
                        var newEnabled = EditorGUILayout.Toggle("Enabled", _currentScript.Enabled);
                        if (newEnabled != _currentScript.Enabled)
                        {
                            _currentScript.Enabled = newEnabled;
                            SetDirty();
                        }

                        // Script trigger type
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Trigger Type", GUILayout.Width(120));
                        var newTriggerType = (ScriptTriggerType)EditorGUILayout.EnumPopup(_currentScript.TriggerType);
                        if (newTriggerType != _currentScript.TriggerType)
                        {
                            _currentScript.TriggerType = newTriggerType;
                            SetDirty();
                        }
                        EditorGUILayout.EndHorizontal();

                        // Custom event name (only if trigger type is OnCustomEvent)
                        if (_currentScript.TriggerType == ScriptTriggerType.OnCustomEvent)
                        {
                            var newEventName = EditorGUILayout.TextField("Event Name", _currentScript.CustomEventName);
                            if (newEventName != _currentScript.CustomEventName)
                            {
                                _currentScript.CustomEventName = newEventName;
                                SetDirty();
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    // Draw the script content section
                    _showScriptContent = EditorGUILayout.Foldout(_showScriptContent, "Script Content", true, _foldoutStyle);
                    if (_showScriptContent)
                    {
                        EditorGUILayout.BeginVertical("box");

                        // Script content text area
                        _scriptContentScrollPosition = EditorGUILayout.BeginScrollView(_scriptContentScrollPosition,
                            GUILayout.Height(300));
                        var newContent = EditorGUILayout.TextArea(_currentScriptContent, _codeStyle,
                            GUILayout.ExpandHeight(true));
                        if (newContent != null && newContent != _currentScriptContent)
                        {
                            _currentScriptContent = newContent;
                        }
                        EditorGUILayout.EndScrollView();

                        EditorGUILayout.BeginHorizontal();

                        // Save button
                        if (GUILayout.Button("Save", _buttonStyle, GUILayout.Width(100)))
                        {
                            SaveScriptContent();
                        }

                        // Test Run button
                        if (GUILayout.Button("Test Run", _buttonStyle, GUILayout.Width(100)))
                        {
                            SaveScriptContent();
                            TestRunScript(wrapper);
                        }

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.Space();

                // Add buttons for script execution
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Run OnLoad Scripts", _buttonStyle))
                {
                    RunScripts(wrapper, ScriptTriggerType.OnLoad);
                }

                if (GUILayout.Button("Stop All Scripts", _buttonStyle))
                {
                    StopAllScripts(wrapper);
                }

                GUILayout.EndHorizontal();

                // Add a section for triggering custom events
                if (_targetAsset != null && _targetAsset.Scripts.Count > 0 &&
                    _targetAsset.Scripts.Any(s => s.TriggerType == ScriptTriggerType.OnCustomEvent))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Custom Events", _headerStyle);

                    EditorGUILayout.BeginVertical("box");

                    // Get unique custom event names
                    var customEvents = new List<string>();
                    foreach (var script in _targetAsset.Scripts.Where(script => 
                        script.TriggerType == ScriptTriggerType.OnCustomEvent &&
                        !string.IsNullOrEmpty(script.CustomEventName) &&
                        !customEvents.Contains(script.CustomEventName)))
                    {
                        customEvents.Add(script.CustomEventName);
                    }

                    // Create buttons for each custom event
                    foreach (var eventName in customEvents)
                    {
                        if (GUILayout.Button($"Trigger '{eventName}'", _buttonStyle))
                        {
                            TriggerCustomEvent(wrapper, eventName);
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();

                // Restore indentation
                EditorGUI.indentLevel--;

                // Add spacing after the scripts section
                EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// Create a list of ScriptReference objects from the Asset's Scripts collection
        /// </summary>
        /// <returns>A list of ScriptReference objects</returns>
        private List<ScriptReference> GetScriptReferences()
        {
            if (_targetAsset == null || _targetAsset.Scripts == null)
            {
                return new List<ScriptReference>();
            }

            return _targetAsset.Scripts.Select(script => new ScriptReference(script)).ToList();
        }

        /// <summary>
        /// Initialize the scripts list based on the current Asset
        /// </summary>
        private void InitializeScriptsList()
        {
            var scriptReferences = GetScriptReferences();

            // Create a new ReorderableList for scripts
            _scriptsList = new ReorderableList(
                scriptReferences,
                typeof(ScriptReference),
                true, true, true, true)
            {
                drawHeaderCallback = DrawScriptsListHeader,
                drawElementCallback = DrawScriptsListElement,
                onSelectCallback = OnScriptSelected,
                onAddCallback = OnScriptAdded,
                onRemoveCallback = OnScriptRemoved,
                elementHeightCallback = index => EditorGUIUtility.singleLineHeight + 4
            };
        }

        /// <summary>
        /// Draw the scripts list header
        /// </summary>
        private static void DrawScriptsListHeader(Rect rect) => EditorGUI.LabelField(rect, "Script Files");

        /// <summary>
        /// Draw a script list element
        /// </summary>
        private void DrawScriptsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var scriptReferences = _scriptsList.list as List<ScriptReference>;
            if (scriptReferences == null || index >= scriptReferences.Count)
            {
                return;
            }

            var script = scriptReferences[index];
            if (script == null)
            {
                return;
            }

            // Adjust the rect to avoid overlapping with reorderable list controls
            rect.y += 2;
            rect.height -= 4;

            // Create rects for the script properties
            var enabledRect = new Rect(rect.x, rect.y, 20, rect.height);
            var nameRect = new Rect(rect.x + 25, rect.y, rect.width - 125, rect.height);
            var triggerRect = new Rect(rect.x + rect.width - 100, rect.y, 100, rect.height);

            // Draw the enabled toggle
            EditorGUI.BeginChangeCheck();
            var newEnabled = EditorGUI.Toggle(enabledRect, script.Enabled);
            if (EditorGUI.EndChangeCheck() && newEnabled != script.Enabled)
            {
                script.Enabled = newEnabled;
                SetDirty();
            }

            // Draw the script name
            EditorGUI.LabelField(nameRect, script.Name);

            // Draw the trigger type
            EditorGUI.LabelField(triggerRect, script.TriggerType.ToString());
        }

        /// <summary>
        /// Handle script selection
        /// </summary>
        private void OnScriptSelected(ReorderableList list)
        {
            _selectedScriptIndex = list.index;
            var scriptReferences = list.list as List<ScriptReference>;

            if (scriptReferences != null && _selectedScriptIndex >= 0 && _selectedScriptIndex < scriptReferences.Count)
            {
                LoadScriptContent(scriptReferences[_selectedScriptIndex]);
            }
            else
            {
                _currentScript = null;
                _currentScriptContent = string.Empty;
            }
        }

        /// <summary>
        /// Handle adding a new script
        /// </summary>
        private void OnScriptAdded(ReorderableList list)
        {
            if (_targetAsset == null)
            {
                Debug.LogWarning("Cannot add script: Target asset is not available");
                return;
            }

            try
            {
                // Generate a unique name
                const string baseName = "NewScript";
                var index = 1;
                var scriptName = baseName;

                while (_targetAsset.Scripts.Any(s => s.Name == scriptName))
                {
                    scriptName = $"{baseName}{index++}";
                }

                // Default script content
                var defaultContent = "// MiniScript file\n" +
                                   $"// Name: {scriptName}\n" +
                                   $"// Created: {DateTime.Now}\n\n" +
                                   "debug(\"Hello from MiniScript!\")\n";

                // Create a new script reference
                var script = new Script
                {
                    Name = scriptName,
                    Enabled = true,
                    TriggerType = ScriptTriggerType.OnLoad,
                    Guid = Guid.NewGuid()
                };

                // Save the script content
                script.ScriptContent = defaultContent;

                // Add to the asset
                _targetAsset.AddScript(script);

                // Refresh the script list
                InitializeScriptsList();

                // Select the new script
                var scriptReferences = list.list as List<ScriptReference>;
                if (scriptReferences != null)
                {
                    _selectedScriptIndex = scriptReferences.Count - 1;
                    list.index = _selectedScriptIndex;

                    if (_selectedScriptIndex >= 0 && _selectedScriptIndex < scriptReferences.Count)
                    {
                        LoadScriptContent(scriptReferences[_selectedScriptIndex]);
                    }
                }

                // Mark as dirty
                SetDirty();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to add script: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to add script: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Handle removing a script
        /// </summary>
        private void OnScriptRemoved(ReorderableList list)
        {
            // If target asset is not available, we can't proceed
            if (_targetAsset == null)
            {
                Debug.LogWarning("Cannot remove script: Target asset is not available");
                return;
            }

            var scriptReferences = list.list as List<ScriptReference>;
            if (scriptReferences == null)
            {
                return;
            }

            // Get the selected script
            if (list.index < 0 || list.index >= scriptReferences.Count)
            {
                return;
            }

            var scriptRef = scriptReferences[list.index];
            if (scriptRef == null)
            {
                return;
            }

            // Confirm removal
            if (!EditorUtility.DisplayDialog("Remove Script",
                    $"Are you sure you want to remove the script '{scriptRef.Name}'?\n\nThis will delete the script file from disk.",
                    "Remove", "Cancel"))
            {
                return;
            }

            try
            {
                // Remove the script from the asset
                _targetAsset.RemoveScript(scriptRef.UnderlyingScript);

                // Clear the current script
                _currentScript = null;
                _currentScriptContent = string.Empty;

                // Reinitialize the list
                InitializeScriptsList();

                // Mark the asset as dirty
                SetDirty();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to remove script: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to remove script: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Load the content of a script
        /// </summary>
        private void LoadScriptContent(ScriptReference script)
        {
            if (script == null)
            {
                return;
            }

            _currentScript = script;

            try
            {
                // Get the script content
                _currentScriptContent = script.ScriptContent;
            }
            catch (Exception ex)
            {
                // If we can't load the content, provide an empty script
                Debug.LogError($"Failed to load script content: {ex.Message}");
                _currentScriptContent = string.Empty;
            }
        }

        /// <summary>
        /// Save the current script content
        /// </summary>
        private void SaveScriptContent()
        {
            if (_currentScript == null)
            {
                return;
            }

            try
            {
                // Save the script content
                _currentScript.ScriptContent = _currentScriptContent;

                // Mark as dirty
                SetDirty();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save script content: {ex.Message}");
                EditorUtility.DisplayDialog("Save Error",
                    $"Failed to save script content: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Test run the current script
        /// </summary>
        private void TestRunScript(WrapperEditor wrapper)
        {
            if (_currentScript == null)
            {
                Debug.LogError("No Script Selected");
                return;
            }

            // Save the current script content first
            SaveScriptContent();

            try
            {
                // Add the RunScriptReference method to the Wrapper class
                var wrapperType = wrapper.GetType();
                var targetProperty = wrapperType.GetProperty("self", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (targetProperty != null)
                {
                    var target = targetProperty.GetValue(wrapper) as Wrapper;
                    if (target != null)
                    {
                        // Call RunScriptReference method if it exists
                        var method = target.GetType().GetMethod("RunScriptReference", 
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | 
                            System.Reflection.BindingFlags.NonPublic);
                        
                        method?.Invoke(target, new object[] { _currentScript.UnderlyingScript });
                    }
                    else
                    {
                        Debug.LogError("Failed to get Wrapper instance from WrapperEditor");
                    }
                }
                else
                {
                    Debug.LogError("Failed to find 'self' property in WrapperEditor");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to execute script: {ex.Message}");
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Run scripts by trigger type
        /// </summary>
        private void RunScripts(WrapperEditor wrapper, ScriptTriggerType triggerType)
        {
            try
            {
                var wrapperType = wrapper.GetType();
                var targetProperty = wrapperType.GetProperty("self", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (targetProperty != null)
                {
                    var target = targetProperty.GetValue(wrapper) as Wrapper;
                    if (target != null)
                    {
                        // Call RunScriptsByTrigger method if it exists
                        var method = target.GetType().GetMethod("RunScriptsByTrigger", 
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic);
                        
                        method?.Invoke(target, new object[] { triggerType });
                    }
                    else
                    {
                        Debug.LogError("Failed to get Wrapper instance from WrapperEditor");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to run scripts: {ex.Message}");
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Trigger a custom event
        /// </summary>
        private void TriggerCustomEvent(WrapperEditor wrapper, string eventName)
        {
            try
            {
                var wrapperType = wrapper.GetType();
                var targetProperty = wrapperType.GetProperty("self", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (targetProperty != null)
                {
                    var target = targetProperty.GetValue(wrapper) as Wrapper;
                    if (target != null)
                    {
                        // Call TriggerCustomEvent method if it exists
                        var method = target.GetType().GetMethod("TriggerCustomEvent", 
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic);
                        
                        method?.Invoke(target, new object[] { eventName });
                    }
                    else
                    {
                        Debug.LogError("Failed to get Wrapper instance from WrapperEditor");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to trigger custom event: {ex.Message}");
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Stop all scripts
        /// </summary>
        private void StopAllScripts(WrapperEditor wrapper)
        {
            try
            {
                var wrapperType = wrapper.GetType();
                var targetProperty = wrapperType.GetProperty("self", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (targetProperty != null)
                {
                    var target = targetProperty.GetValue(wrapper) as Wrapper;
                    if (target != null)
                    {
                        // Call StopScripts method if it exists
                        var method = target.GetType().GetMethod("StopScripts", 
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic);
                        
                        method?.Invoke(target, null);
                    }
                    else
                    {
                        Debug.LogError("Failed to get Wrapper instance from WrapperEditor");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to stop scripts: {ex.Message}");
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Initialize GUI styles
        /// </summary>
        private void InitializeStyles()
        {
            _headerStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                margin = new RectOffset(0, 0, 10, 5)
            };

            if (_codeStyle == null)
            {
                _codeStyle = new GUIStyle(EditorStyles.textArea)
                {
                    font = EditorGUIUtility.Load("Fonts/RobotoMono/RobotoMono-Regular.ttf") as Font
                };
                if (_codeStyle.font == null)
                {
                    // Fallback to a monospace font if RobotoMono isn't available
                    _codeStyle.font = EditorGUIUtility.Load("Fonts/consola.ttf") as Font;
                }
                _codeStyle.fontSize = 12;
                _codeStyle.padding = new RectOffset(5, 5, 5, 5);
            }

            _buttonStyle ??= new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(10, 10, 5, 5)
            };

            _foldoutStyle ??= new GUIStyle(EditorStyles.foldout);
        }
    }
}