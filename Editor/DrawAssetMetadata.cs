using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using Urt3d.Utilities;
#pragma warning disable CS0618

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Editor
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Specialized inspector class responsible for drawing the Metadata section in the URT3D Manager.
    /// Extends Urt3dManagerAssets to utilize common parameter rendering functionality.
    /// Manages the rendering and interaction of metadata properties for URT3D instances.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DrawAssetMetadata : DrawAsset
    {
        /// <summary>
        /// Dictionary to track the expanded/collapsed state of metadata foldouts for each URT3D instance.
        /// The key is the URT3D instance's unique GuidInstance.
        /// </summary>
        private readonly System.Collections.Generic.Dictionary<System.Guid, bool> _metadataFoldouts = new();

        /// <summary>
        /// Draws the Metadata section for a specific URT3D instance.
        /// Displays a foldout to expand/collapse the entire metadata section.
        /// When expanded, displays all metadata properties including GUID, name, and type information.
        /// </summary>
        /// <param name="guid">The unique identifier of the URT3D instance.</param>
        /// <param name="asset">The URT3D instance whose metadata is to be displayed.</param>
        public void DrawMetadata(System.Guid guid, Asset asset)
        {
            // Initialize the foldout state for this metadata section if it doesn't exist
            if (!_metadataFoldouts.ContainsKey(guid))
            {
                _metadataFoldouts[guid] = false;
            }

            // Draw the metadata foldout
            _metadataFoldouts[guid] = EditorGUILayout.Foldout(_metadataFoldouts[guid], "Metadata", true);

            // Only show metadata details if the foldout is expanded
            if (_metadataFoldouts[guid])
            {
                // Increase indentation for nested content
                EditorGUI.indentLevel++;

                // Check if metadata exists
                if (asset?.Metadata != null)
                {
                    // Get metadata reference for cleaner code
                    var metadata = asset.Metadata;

                    // Find the Unity Object that contains this asset
                    UnityEngine.Object targetObject = FindUrt3dWrapper(asset);
                    
                    // Display GuidDefinition with validation but w/out "New Guid" button since GuidDefinition is immutable
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.TextField("Guid Definition", metadata.GuidDefinition.ToString());
                    }

                    // Display GuidInstance with validation and button
                    DrawGuidField("Guid Instance", metadata.GuidInstance, newGuid =>
                    {
                        if (targetObject != null)
                        {
                            Undo.RecordObject(targetObject, "Change Asset Instance GUID");
                        }
                        metadata.GuidInstance = newGuid;
                        if (targetObject != null)
                        {
                            EditorUtility.SetDirty(targetObject);
                        }
                        SetDirty();
                    });

                    // Display read-only metadata properties
                    EditorGUILayout.LabelField("Name Definition", metadata.NameDefinition);
                    EditorGUILayout.LabelField("Type Definition", metadata.TypeDefinition);
                    EditorGUILayout.LabelField("Type Instance", metadata.TypeInstance != null ? metadata.TypeInstance.ToString() : "null");

                    // Display editable name instance field
                    EditorGUI.BeginChangeCheck();
                    var newNameInstance = EditorGUILayout.TextField("Name Instance", metadata.NameInstance);
                    if (EditorGUI.EndChangeCheck())
                    {
                        metadata.NameInstance = newNameInstance;
                        SetDirty();
                    }

                    // Format and display bundled assets as comma-separated list
                    var bundledAssets = metadata.BundledAssets is { Length: > 0 } ? string.Join(", ", metadata.BundledAssets) : "None";
                    EditorGUILayout.LabelField("Bundled Assets", bundledAssets);

                    // Format and display bundled scripts as comma-separated list
                    var bundledScripts = metadata.BundledScripts is { Length: > 0 } ? string.Join(", ", metadata.BundledScripts) : "None";
                    EditorGUILayout.LabelField("Bundled Scripts", bundledScripts);
                }
                else
                {
                    // Display a message when no metadata is available
                    EditorGUILayout.LabelField("No metadata available.");
                }

                // Restore indentation level
                EditorGUI.indentLevel--;

                // Add spacing after the metadata section
                EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// Finds the Unity wrapper object that contains this Urt3d asset
        /// </summary>
        /// <param name="asset">The asset to find the wrapper for</param>
        /// <returns>The Unity object wrapper, or null if not found</returns>
        private UnityEngine.Object FindUrt3dWrapper(Asset asset)
        {
            // Find all wrappers in the scene
            var assetWrappers = GameObject.FindObjectsOfType<Wrapper>();
            foreach (var wrapper in assetWrappers)
            {
                if (wrapper.Asset == asset)
                {
                    return wrapper;
                }
            }
            return null;
        }

        /// <summary>
        /// Draws a GUID field with validation and a button to regenerate the GUID
        /// </summary>
        /// <param name="label">The label to display</param>
        /// <param name="guid">The current GUID value</param>
        /// <param name="onNewGuidGenerated">Action to call when a new GUID is generated</param>
        private void DrawGuidField(string label, Guid guid, Action<Guid> onNewGuidGenerated)
        {
            // Check if the GUID is valid (not empty)
            bool isGuidValid = guid != Guid.Empty;
            
            EditorGUILayout.BeginHorizontal();
            
            // Show GUID with appropriate color based on validity
            using (new EditorGUI.DisabledScope(true))
            {
                var originalColor = GUI.contentColor;
                if (!isGuidValid)
                {
                    GUI.contentColor = Color.red;
                }
                
                EditorGUILayout.TextField(label, guid.ToString());
                
                // Reset color
                GUI.contentColor = originalColor;
            }
            
            // Add the Cycle GUID button
            if (GUILayout.Button("New GUID", GUILayout.Width(80)))
            {
                var newGuid = Guid.NewGuid();
                onNewGuidGenerated?.Invoke(newGuid);
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Show validation error if GUID is invalid
            if (!isGuidValid)
            {
                EditorGUILayout.HelpBox($"Invalid {label}: {guid}", MessageType.Error);
            }
        }
    }
}
