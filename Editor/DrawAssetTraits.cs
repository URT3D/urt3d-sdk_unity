using System.Diagnostics.CodeAnalysis;
using UnityEditor;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Editor
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Specialized inspector class responsible for drawing the Traits section in the URT3D Manager.
    /// Extends Urt3dManagerInspect to utilize common parameter rendering functionality.
    /// Manages the rendering and interaction of trait parameters for URT3D instances.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DrawAssetTraits : DrawAsset
    {
        /// <summary>
        /// Dictionary to track the expanded/collapsed state of traits foldouts for each URT3D instance.
        /// The key is the URT3D instance's unique GuidInstance.
        /// </summary>
        private readonly System.Collections.Generic.Dictionary<System.Guid, bool> traitsFoldouts = new();

        /// <summary>
        /// Draws the Traits section for a specific URT3D instance.
        /// Displays a foldout to expand/collapse the entire traits section.
        /// When expanded, displays all trait parameters if available, or a message if no traits are available.
        /// </summary>
        /// <param name="guid">The unique identifier of the URT3D instance.</param>
        /// <param name="asset">The URT3D instance whose traits are to be displayed.</param>
        public void DrawTraits(System.Guid guid, Asset asset)
        {
            // Initialize foldout state for this URT3D instance if it doesn't exist
            if (!traitsFoldouts.ContainsKey(guid))
            {
                traitsFoldouts[guid] = false;
            }

            // Draw the traits foldout
            traitsFoldouts[guid] = EditorGUILayout.Foldout(traitsFoldouts[guid], "Traits", true);

            // Only draw the contents if the foldout is expanded
            if (traitsFoldouts[guid])
            {
                // Increase indentation for nested content
                EditorGUI.indentLevel++;

                // Check if traits exist and display them
                if (asset.Traits is { Count: > 0 })
                {
                    // Draw all trait params using the common base method
                    DrawParams(asset.Traits);
                }
                else
                {
                    // Display a message when no traits are available
                    EditorGUILayout.LabelField("No traits available.");
                }

                // Restore indentation
                EditorGUI.indentLevel--;

                // Add spacing after the traits section
                EditorGUILayout.Space();
            }
        }
    }
}
