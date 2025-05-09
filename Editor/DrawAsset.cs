using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Editor
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Abstract base class for drawing inspection sections in the URT3D Manager.
    /// Provides common functionality for rendering a list of Params with their names and editable controls.
    /// This class is extended by specialized inspectors for different sections of the URT3D Manager.
    /// </summary>
    public abstract class DrawAsset
    {
        /// <summary>
        /// Draws a list of parameters with their names (and tooltips) and appropriate editable controls.
        /// Handles different parameter types including string, int, float, bool, Vector2, Vector3, and DynamicEnum.
        /// For each parameter, displays its name, tooltip, and the appropriate editor control based on its type.
        /// Updates the parameter value when the user changes it.
        /// </summary>
        /// <param name="parameters">The collection of parameters to render.</param>
        protected void DrawParams(IEnumerable<Param> parameters)
        {
            foreach (var param in parameters)
            {
                // Create a horizontal layout for parameter name and its input field
                EditorGUILayout.BeginHorizontal();

                // Create GUIContent with parameter name and tooltip
                var content = new GUIContent(param.Name, param.Tooltip);

                // Display the parameter name (fixed width to align the input fields)
                EditorGUILayout.LabelField(content, GUILayout.Width(150));

                // Use type pattern matching to handle different parameter types with appropriate controls

                // String parameters - TextField
                if (param is Param<string> stringParam)
                {
                    var newValue = EditorGUILayout.TextField(stringParam.Value);
                    if (newValue != stringParam.Value)
                    {
                        SetDirty();
                        stringParam.Value = newValue;
                    }
                }
                // Integer parameters - IntField
                else if (param is Param<int> intParam)
                {
                    var newValue = EditorGUILayout.IntField(intParam.Value);
                    if (newValue != intParam.Value)
                    {
                        SetDirty();
                        intParam.Value = newValue;
                    }
                }
                // Float parameters - FloatField (with threshold to avoid floating point precision issues)
                else if (param is Param<float> floatParam)
                {
                    var newValue = EditorGUILayout.FloatField(floatParam.Value);
                    // Use a small threshold for float comparison to avoid floating-point precision issues
                    if (Math.Abs(newValue - floatParam.Value) > 0.0001f)
                    {
                        SetDirty();
                        floatParam.Value = newValue;
                    }
                }
                // Boolean parameters - Toggle checkbox
                else if (param is Param<bool> boolParam)
                {
                    var newValue = EditorGUILayout.Toggle(boolParam.Value);
                    if (newValue != boolParam.Value)
                    {
                        SetDirty();
                        boolParam.Value = newValue;
                    }
                }
                // Vector2 parameters - Vector2Field
                else if (param is Param<Vector2> vector2Param)
                {
                    var newValue = EditorGUILayout.Vector2Field("", vector2Param.Value);
                    if (newValue != vector2Param.Value)
                    {
                        SetDirty();
                        vector2Param.Value = newValue;
                    }
                }
                // Vector3 parameters - Vector3Field
                else if (param is Param<Vector3> vector3Param)
                {
                    var newValue = EditorGUILayout.Vector3Field("", vector3Param.Value);
                    if (newValue != vector3Param.Value)
                    {
                        SetDirty();
                        vector3Param.Value = newValue;
                    }
                }
                // Dynamic Enum parameters - Popup dropdown
                else if (param is Param<Utilities.DynamicEnum> dynamicEnumParam)
                {
                    var dynamicEnum = dynamicEnumParam.Value;
                    if (dynamicEnum != null && dynamicEnum.Values.Count > 0)
                    {
                        // Create string array for the popup options
                        var options = new string[dynamicEnum.Values.Count];
                        for (var i = 0; i < dynamicEnum.Values.Count; i++)
                        {
                            options[i] = dynamicEnum.Values[i];
                        }

                        // Display the dropdown and update the selected index if changed
                        var newIndex = EditorGUILayout.Popup(dynamicEnum.Index, options);
                        if (newIndex != dynamicEnum.Index)
                        {
                            SetDirty();
                            dynamicEnum.Index = newIndex;
                        }
                    }
                    else
                    {
                        // Show a message when no enum values are available
                        EditorGUILayout.LabelField("No values available");
                    }
                }
                // Fall back to displaying string representation for unsupported types
                else
                {
                    EditorGUILayout.LabelField(param.ValueObject?.ToString());
                }

                // End the horizontal layout
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected static void SetDirty()
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
