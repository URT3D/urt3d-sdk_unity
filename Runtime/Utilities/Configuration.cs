using System.IO;
using UnityEngine;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Scriptable object that stores URT3D configuration settings.
    /// This configuration can be accessed at runtime in the built application.
    /// </summary>
    public class Configuration : ScriptableObject
    {
        /// <summary>
        /// The URT3D App Key used for API authentication.
        /// </summary>
        [SerializeField, Tooltip("The URT3D App Key used for API authentication")]
        private string _appKey = string.Empty;

        /// <summary>
        /// Singleton instance of the configuration.
        /// Loads the configuration asset from Resources folder.
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<Configuration>("Urt3dConfig");

                    // If no config exists in Resources, create one
                    if (_instance == null)
                    {
                        _instance = CreateInstance<Configuration>();
#if UNITY_EDITOR
                        var path = Path.Combine("Assets", "Resources", "Urt3dConfig.asset");
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        UnityEditor.AssetDatabase.CreateAsset(_instance, path);
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
#endif
                        Log.Warning("No Urt3dConfig found in Resources folder. Using default configuration.");
                    }
                }
                return _instance;
            }
        }
        private static Configuration _instance;

        /// <summary>
        /// Gets or sets the URT3D App Key.
        /// </summary>
        public string AppKey
        {
            get => _appKey;
            set
            {
                if (_appKey != value)
                {
                    _appKey = value;
#if UNITY_EDITOR                   
                    UnityEditor.EditorUtility.SetDirty(Instance);
                    UnityEditor.AssetDatabase.SaveAssets();
#endif
                }
            }
        }
    }
}
