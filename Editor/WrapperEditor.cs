using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Editor
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(Wrapper))]
    public class WrapperEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Shorthand accessor for the target Wrapper component being edited.
        /// </summary>
        private Wrapper self => (Wrapper)target;

        /// <summary>
        /// 
        /// </summary>
        protected WrapperEditor()
        {
            // Assembly reload events
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            
            // Scene events
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneSaving += OnSceneSaving;
        }

        /// <summary>
        /// Unsubscribe from all events to prevent memory leaks
        /// </summary>
        ~WrapperEditor()
        {
            // Assembly reload events
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            
            // Scene events
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneSaving -= OnSceneSaving;
        }

        /// <summary>
        /// Reloading the assembly ungracefully removes the URT3D object from memory.
        /// So, force a reload of all objects so that they exist in memory as expected.
        /// </summary>
        private void OnAfterAssemblyReload()
        {
            self?.Reload();
        }

        /// <summary>
        /// Called before assembly reload
        /// </summary>
        private void OnBeforeAssemblyReload() 
        {
            // NO-OP
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            // NO-OP
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="_"></param>
        private void OnSceneSaving(Scene scene, string _)
        {
            if (self == null) return;
            self.Json = JsonUtil.ToJson(self.Asset);
        }

        /// <summary>
        /// Overrides the default Inspector GUI to create a custom interface
        /// for URT3D Asset Wrappers, divided into three main sections:
        /// asset loading configuration, component configuration, and scene management.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawSectionBase();
            DrawSectionConfig();
        }
        
        /// <summary>
        /// Draws a standardized inspector section for editing URT3D wrapper properties.
        /// This method creates a consistent UI for all URT3D content types, displaying
        /// mode selection, file path, GUID information, and relevant warnings or error messages.
        /// </summary>
        private void DrawSectionBase()
        {
            // Draw the header
            GUILayout.Label($"URT3D Asset", EditorStyles.boldLabel);

            // Draw the information box
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This component is used to dynamically load URT3D content in both Edit and Play modes. "+
                                    "It may be modified to control what URT3D Asset is loaded. " +
                                    "It's children are programmatically managed and must not be modified!", MessageType.Info);

            // Draw the Mode selection
            EditorGUILayout.Space();
            self.Mode = (Wrapper.ModeType)EditorGUILayout.EnumPopup("Mode", self.Mode);

            // Draw the Edit File section
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("File Path", self.Path);
            }

            // Draw the Invalid File Path warning
            var isValidFile = true;
            if (self.Mode == Wrapper.ModeType.Local_File && !File.Exists(self.Path))
            {
                isValidFile = false;
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox($"Invalid file path: {self.Path}", MessageType.Error);
            }

            // Check if the GUID is valid
            var isValidGuid = self.Guid != Guid.Empty;
            
            // Draw the GUID section with regenerate button
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(true))
            {
                // Show GUID with appropriate color based on validity
                var currentGuid = self.Guid.ToString();
                
                var originalColor = GUI.contentColor;
                if (!isValidGuid)
                {
                    GUI.contentColor = Color.red;
                }
                
                EditorGUILayout.TextField("Asset GUID", currentGuid);
                
                // Reset color
                GUI.contentColor = originalColor;
            }
            EditorGUILayout.EndHorizontal();

            // Draw the Invalid GUID warning
            if (self.Mode == Wrapper.ModeType.Remote_Guid && self.Guid == Guid.Empty)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox($"Invalid GUID: {self.Guid.ToString()}", MessageType.Error);
            }

            // Draw the File Path runtime warning
            if (self.Mode == Wrapper.ModeType.Local_File)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Load from file path is only available within the Unity Editor. " +
                                        "Builds will always access URT3D Assets from the CDN via GUID. " +
                                        "This allows for dynamic updates without recompilation or redeployment. " +
                                        "Please ensure that all Assets have been uploaded to the CDN prior to release.", MessageType.Warning);
            }
            
            // Draw the Reload Scene button
            var isValid = isValidFile || isValidGuid;
            using (new EditorGUI.DisabledScope(!isValid))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Reload Asset"))
                {
                    self?.Reload();
                }
                if (GUILayout.Button("Upload Asset"))
                {
                    if (EditorUtility.DisplayDialog("Asset Upload", "Are you sure you want to upload this Asset to the CDN?\n\n" +
                                                                    "If the Asset doesn't exist yet, it will be created. If it does already exist, " +
                                                                    "a new variation (with a new GUID) will be generated.", "Yes", "No"))
                    {
                        Network.Instance.Upload(self.Path, _ => { }, data =>
                        {
                            if (data.isValid)
                            {
                                EditorUtility.DisplayDialog("Success", data.message, "Okay");
                                Debug.Log(data.message);
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("Error", data.message, "Okay");
                                Debug.LogError(data.message);
                            }
                        });
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Draws the Config section of the inspector, displaying metadata, trait, cause, and effect
        /// configuration interfaces for the loaded URT3D Asset. Uses specialized inspectors
        /// for each element type to provide a comprehensive configuration interface.
        /// Shows a warning message if no asset is currently loaded.
        /// </summary>
        private void DrawSectionConfig()
        {
            // Draw divider line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Draw the header
            GUILayout.Label("URT3D Config", EditorStyles.boldLabel);

            // Draw inspectors
            EditorGUILayout.Space();
            if (self.Asset != null)
            {
                EditorGUI.indentLevel++;

                // Draw the Metadata section first
                _metadataInspector.DrawMetadata(self.Asset.Metadata.GuidInstance, self.Asset);

                // Draw the other sections
                _traitsInspector.DrawTraits(self.Asset.Metadata.GuidInstance, self.Asset);

                //
                _scriptsInspector.DrawScripts(self.Guid, self.Asset, this);
                
                EditorGUI.indentLevel--;
            }
            else
            {
                switch (self.Mode)
                {
                    case Wrapper.ModeType.Local_File:
                        EditorGUILayout.HelpBox("Failed to instantiate URT3D Asset via Local File. " +
                                                "Please ensure that the Asset file exists and then click 'Reload Scene'.", MessageType.Error);
                        break;
                    case Wrapper.ModeType.Remote_Guid:
                        EditorGUILayout.HelpBox("Failed to instantiate URT3D Asset via Remote GUID. " +
                                                "Please ensure that the Asset has been uploaded to the CDN and then click 'Reload Scene'.", MessageType.Error);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private readonly DrawAssetMetadata _metadataInspector = new();
        private readonly DrawAssetTraits _traitsInspector = new();
        private readonly DrawAssetScripts _scriptsInspector = new();
    }
}