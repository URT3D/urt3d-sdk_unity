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
                    Log.Exception(action.Exception);
                }
                callback?.Invoke(action.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Properly dispose of this object.
        /// </summary>
        public void Destroy()
        {
            GameObjectUtils.Destroy(_gameObject);
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
            return actor?._gameObject;
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
        }

        #endregion

        #region Private Variables

        private readonly GameObject _gameObject;

        #endregion
    }
}
