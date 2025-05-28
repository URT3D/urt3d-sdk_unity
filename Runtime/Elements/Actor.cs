using System;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Actor is a wrapper for a GameObject.
    /// </summary>
    public class Actor
    {
        #region Public Methods: Construction and Destruction

        /// <summary>
        /// Construct a new Actor from the given file.
        /// </summary>
        /// <param name="pathToActorFile">Fully qualified path to Actor file.</param>
        /// <returns>Fully initialized Actor object.</returns>
        public static async Task<Actor> Construct(string pathToActorFile)
        {
            // Configure GLTF import
            var gltf = new GltfImport
            (
                deferAgent: new UninterruptedDeferAgent(),
                materialGenerator: null
            );
            var settings = new ImportSettings
            {
                AnimationMethod = AnimationMethod.Legacy,
                GenerateMipMaps = true
            };

            // Load raw GLTF data from disk
            if (!await gltf.Load(pathToActorFile, settings))
            {
                Log.Error($"Failed to instantiate GLTF from {pathToActorFile}");
                return new Actor(new GameObject());
            }

            // Instantiate asset(s) from GLTF data
            var go = new GameObject("Temporary");
            if (!await gltf.InstantiateMainSceneAsync(go.transform))
            {
                Log.Error($"Failed to instantiate GLTF from {pathToActorFile}");
                return new Actor(new GameObject());
            }

            return new Actor(go);
        }

        /// <summary>
        /// Construct a new Actor from the given file.
        /// </summary>
        /// <param name="pathToActorFile">Fully qualified path to Actor file.</param>
        /// <param name="callback">Callback invoked when Actor is constructed.</param>
        public static void Construct(string pathToActorFile, Action<Actor> callback)
        {
            var task = Construct(pathToActorFile);
            task.ContinueWith(action =>
            {
                if (action.IsFaulted)
                {
                    Debug.LogException(action.Exception);
                }
                callback?.Invoke(action.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Properly dispose of this object.
        /// </summary>
        public void Destroy()
        {
            // Sanity check the game object
            if (_gameObject == null) return;

            if (Application.isPlaying)
            {
                // Modified materials must be manually purged!
                var renderers = _gameObject.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    var materials = renderer.materials; // These are instanced materials (copies)
                    var materialsShared = renderer.sharedMaterials; // These are the originals from the asset

                    if (materials.Length != materialsShared.Length)
                    {
                        Log.Debug($"Unexpected mismatch between and materials and sharedMaterials for Actor: {_gameObject.name}");
                        continue;
                    }

                    // Manually destroy instanced materials
                    for (var i = 0; i < materials.Length; ++i)
                    {
                        if (materials[i] != materialsShared[i])
                        {
                            GameObject.Destroy(materials[i]);
                        }
                    }
                }

                // Destroy game object
                GameObject.Destroy(_gameObject);
            }
            else
            {
                // Immediately destroy game object
                GameObject.DestroyImmediate(_gameObject);
            }
        }

        #endregion

        #region Public Methods: Implicit Conversions

        /// <summary>
        /// Implicit conversion from Actor to GameObject
        /// </summary>
        /// <param name="actor">Actor object from which to convert.</param>
        /// <returns>GameObject associated with the given Actor.</returns>
        public static implicit operator GameObject(Actor actor)
        {
            return actor._gameObject;
        }

        #endregion

        #region Public Methods: Miscellaneous
        
        /// <summary>
        /// Initializes the Actor with a reference to its parent Asset.
        /// Sets up name synchronization and wrapper component initialization.
        /// </summary>
        /// <param name="asset">The parent Asset that owns this Actor</param>
        public void Initialize(Asset asset)
        {
            void SetName(string name)
            {
                _gameObject.name = $"URT3D Actor: {name}";
            }
            
            //
            SetName(asset.Metadata.NameInstance);
            asset.Metadata.OnNameInstanceChanged += SetName;

            //
            _gameObject.GetComponent<Bridge>()?.Initialize(asset);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Construct a new Actor with the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject to visually represent this Actor.</param>
        private Actor(GameObject gameObject)
        {
            // Validate game object
            if (gameObject == null)
            {
                _gameObject = new GameObject();
                Log.Error("Attempted to construct an Actor with an invalid GameObject");
            }
            else
            {
                _gameObject = gameObject;
            }

            // Don't include Actor in saved scenes
            _gameObject.hideFlags = HideFlags.DontSave;

            // Ensure that Bridge component exists
            if (!_gameObject.GetComponent<Bridge>())
            {
                _gameObject.AddComponent<Bridge>();
            }
        }

        #endregion

        #region Private Variables

        private readonly GameObject _gameObject;

        #endregion

        /// <summary>
        /// Bridge component attached to the Actor's GameObject.
        /// Provides a link between the Unity GameObject hierarchy and the Urt3d Asset system.
        /// Handles automatic cleanup of Assets when their Actor GameObjects are destroyed.
        /// </summary>
        public class Bridge : MonoBehaviour
        {
            private Asset _asset;

            /// <summary>
            /// Initializes the bridge component with the Asset's GUID.
            /// Stores the GUID for later reference to find the Asset in the AssetCache.
            /// </summary>
            /// <param name="guid">The unique identifier of the parent Asset</param>
            public void Initialize(Asset asset)
            {
                _asset = asset;
            }

            /// <summary>
            /// Called when the GameObject is destroyed.
            /// Automatically destroys the associated Asset when its Actor is destroyed,
            /// maintaining the lifecycle relationship between Visual and Data representations.
            /// </summary>
            private void OnDestroy()
            {
                _asset?.Destroy();
            }

#if UNITY_EDITOR
            /// <summary>
            /// Watch for scene hierarchy changes to detected destruction when not in Play mode.
            /// </summary>
            private Bridge()
            {
                UnityEditor.EditorApplication.hierarchyChanged += OnHierarchyChanged;
            }
            /// <summary>
            /// Deregister for all subscribed events.
            /// </summary>
            ~Bridge()
            {
                UnityEditor.EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            }
            /// <summary>
            /// Process destruction when not in Play mode (since OnDestroy doesn't fire in Edit mode).
            /// </summary>
            private void OnHierarchyChanged()
            {
                if (this == null)
                {
                    OnDestroy();
                }
            }
#endif
        }
    }
}
