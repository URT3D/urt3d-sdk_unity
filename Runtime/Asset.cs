using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Urt3d.Scripting;
using Urt3d.Traits;
using Urt3d.Utilities;

namespace Urt3d
{
    /// <summary>
    /// The core class of the URT3D SDK, representing a 3D interactive object.
    /// URT3D consists of an Actor (visual representation), Preview (2D image),
    /// and Metadata, along with optional Traits, Causes, and Effects that define behavior.
    /// This class should be extended to create specific URT3D types with custom functionality.
    ///
    /// URT3D objects form the foundation of interactive 3D content in the SDK, providing a
    /// standardized way to load, manipulate, and interact with 3D assets along with their
    /// associated metadata and behaviors.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public abstract class Asset
    {
        public static ReadOnlyCollection<Asset> Cache => _cache.AsReadOnly();

        #region Public Events

        /// <summary>
        /// Event triggered when a Trait is added to this URT3D.
        /// </summary>
        public Action<Trait> OnTraitAdded;

        /// <summary>
        /// Event triggered when a Trait is removed from this URT3D.
        /// </summary>
        public Action<Trait> OnTraitRemoved;

        /// <summary>
        /// 
        /// </summary>
        public Action<Script> OnScriptAdded;

        /// <summary>
        /// 
        /// </summary>
        public Action<Script> OnScriptRemoved;

        #endregion

        #region Public Properties: Base Elements

        /// <summary>
        /// Visual representation of content.
        /// <see cref="Actor"/> must always exist, otherwise an internal error occurred.
        /// </summary>
        public Actor Actor { get; private set; } = null;

        /// <summary>
        /// Two-dimensional preview of content.
        /// <see cref="Preview"/> must always exist, otherwise an internal error occurred.
        /// </summary>
        public Preview Preview { get; private set; } = null;

        /// <summary>
        /// Metadata associated with this Urt3d.
        /// <see cref="Metadata"/> must always exist, otherwise an internal error occurred.
        /// </summary>
        public Metadata Metadata { get; private set; } = null;

        #endregion

        #region Public Properties: Traits and Scripts

        /// <summary>
        /// Read only list of all <see cref="Trait"/> objects defined by this URT3D.
        /// </summary>
        public ReadOnlyCollection<Trait> Traits => _traits.AsReadOnly();

        /// <summary>
        ///
        /// </summary>
        public ReadOnlyCollection<Script> Scripts => _scripts.AsReadOnly();

        #endregion

        #region Public Methods: Traits and Scripts

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        public void AddScript(Script script)
        {
            _scripts.Add(script);
            OnScriptAdded?.Invoke(script);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cause"></param>
        public void RemoveScript(Script script)
        {
            _scripts.Remove(script);
            OnScriptRemoved?.Invoke(script);
        }

        /// <summary>
        /// Retrieve the <see cref="Trait"/> of type T, if it exists.
        /// </summary>
        /// <typeparam name="T"><see cref="Trait"/> type to be returned.</typeparam>
        /// <returns><see cref="Trait"/> of type T associated with this URT3D.</returns>
        public T GetTrait<T>() where T : Trait
        {
            var value = Traits.FirstOrDefault(x => x is T);
            if (value == null)
            {
                Log.Error($"Failed to locate Trait of type {typeof(T)} in URT3D {Metadata.NameInstance}");
            }
            return (T)value;
        }

        #endregion

        #region Public Methods: Construction and Destruction

        /// <summary>
        /// Construct a new <see cref="Asset"/> instance using a GUID and source location.
        /// This method searches for an Asset file matching the provided GUID in the specified source location,
        /// then delegates to the path-based construction method if found.
        /// </summary>
        /// <param name="guid">Unique identifier for the <see cref="Asset"/> to construct</param>
        /// <returns>A Task that resolves to the constructed <see cref="Asset"/> object, or null if construction fails</returns>
        public static async Task<Asset> Construct(Guid guid)
        {
            var pathToAsset = await AssetFinder.Find(guid);
            if (AssetFinder.IsValid(pathToAsset))
            {
                return await Construct(pathToAsset);
            }
            Log.Error($"Failed to locate URT3D Asset with GUID: {guid}");
            return null;
        }
        
        /// <summary>
        /// Construct a new Asset instance using a GUID and source location.
        /// This method provides a callback-based alternative to the async Construct method.
        /// The callback is executed on the Unity main thread via TaskScheduler.FromCurrentSynchronizationContext().
        /// </summary>
        /// <param name="guid">Unique identifier for the Asset to construct</param>
        /// <param name="callback">Callback to invoke when construction completes, with the Asset as parameter</param>
        public static void Construct(Guid guid, Action<Asset> callback)
        {
            var task = Construct(guid);
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
        /// Construct a new Urt3d from the given file.
        /// </summary>
        /// <param name="pathToAsset">Fully qualified path to the Urt3d file.</param>
        /// <returns>Null or fully initialized Urt3d object.</returns>
        public static async Task<Asset> Construct(string pathToAsset)
        {
            if (!File.Exists(pathToAsset))
            {
                Log.Error($"Failed to locate URT3D Asset file: {pathToAsset}");
                return null;
            }

            // Decrypt and extract this asset
            var path = await Decrypt.Extract(pathToAsset);

            // Locate required assets within temporary directory
            string pathToActor = null, pathToPreview = null, pathToMetadata = null;
            var filePaths = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in filePaths)
            {
                var fileInfo = new FileInfo(file);
                switch (fileInfo.Extension.ToLower())
                {
                    case ".glb":
                        pathToActor = file;
                        break;
                    case ".jpg":
                    case ".png":
                        pathToPreview = file;
                        break;
                    case ".txt":
                    case ".json":
                        pathToMetadata = file;
                        break;
                }
            }

            // For regular assets, validate all required files
            var isValid = true;
            if (string.IsNullOrEmpty(pathToActor))
            {
                isValid = false;
                Log.Error($"Failed to locate Actor in URT3D Asset file: {pathToAsset}");
            }
            if (string.IsNullOrEmpty(pathToPreview))
            {
                isValid = false;
                Log.Error($"Failed to locate Preview in URT3D Asset file: {pathToAsset}");
            }
            if (string.IsNullOrEmpty(pathToMetadata))
            {
                isValid = false;
                Log.Error($"Failed to locate Metadata in URT3D Asset file: {pathToAsset}");
            }
            if (isValid)
            {
                return await Construct(pathToActor, pathToPreview, pathToMetadata);
            }
            return null;
        }

        /// <summary>
        /// Construct a new Urt3d from the given file.
        /// </summary>
        /// <param name="pathToAsset">Fully qualified path to the Urt3d file.</param>
        /// <param name="callback">Callback invoked when Urt3d is constructed.</param>
        public static void Construct(string pathToAsset, Action<Asset> callback)
        {
            var task = Construct(pathToAsset);
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
        /// Construct a new Urt3d from the given files.
        /// </summary>
        /// <param name="pathToActor">Fully qualified path to the Urt3d's Actor file.</param>
        /// <param name="pathToPreview">Fully qualified path to the Urt3d's Preview file.</param>
        /// <param name="pathToMetadata">Fully qualified path to the Urt3d's Metadata file.</param>
        /// <returns>Null or fully initialized Urt3d object.</returns>
        public static async Task<Asset> Construct(string pathToActor, string pathToPreview, string pathToMetadata)
        {
            var actor = await Actor.Construct(pathToActor);
            var preview = await Preview.Construct(pathToPreview);
            var metadata = await Metadata.Construct(pathToMetadata);

            // Try to dynamically instantiate Urt3d type from assembly
            if (Activator.CreateInstance(metadata.TypeInstance) is Asset urt3d)
            {
                // Initialize asset
                urt3d.Actor = actor;
                urt3d.Preview = preview;
                urt3d.Metadata = metadata;
                urt3d.Initialize(); // TODO Initialize in thread...
                
                // Initialize elements
                actor.Initialize(urt3d);
                return urt3d;
            }

            // Log error message and return placeholder Urt3d
            Log.Error("Failed to instantiate URT3D Asset:\n" +
                      $"  Guid: {metadata.GuidDefinition}\n" +
                      $"  Name: {metadata.NameDefinition}\n" +
                      $"  Type: {metadata.TypeDefinition}");
            return null; // TODO
        }

        /// <summary>
        /// Construct a new Urt3d from the given files.
        /// </summary>
        /// <param name="pathToActor">Fully qualified path to the Urt3d's Actor file.</param>
        /// <param name="pathToPreview">Fully qualified path to the Urt3d's Preview file.</param>
        /// <param name="pathToMetadata">Fully qualified path to the Urt3d's Metadata file.</param>
        /// <param name="callback">Callback invoked when Urt3d is constructed.</param>
        public static void Construct(string pathToActor, string pathToPreview, string pathToMetadata, Action<Asset> callback)
        {
            var task = Construct(pathToActor, pathToPreview, pathToMetadata);
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
        /// Properly dispose of this object by removing it from the <see cref="Utilities.AssetCache"/>,
        /// calling <see cref="Deinitialize"/> and destroying all component objects.
        /// </summary>
        public void Destroy()
        {
            // Remove from cache
            _cache.Remove(this);

            // Deinitialize child-classes
            Deinitialize();

            // Destroy base elements
            Actor.Destroy();
            Metadata.Destroy();
            Preview.Destroy();
        }

        #endregion

        #region Protected Methods: Inialize and Deinitialize

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Deinitialize();
        
        #endregion
        
        #region Protected Methods: Traits and Scripts

        /// <summary>
        /// Add the given <see cref="Trait"/> instance to this URT3D.
        /// </summary>
        /// <param name="trait"><see cref="Trait"/> instance to be added.</param>
        protected void AddTrait(Trait trait)
        {
            _traits.Add(trait);
            OnTraitAdded?.Invoke(trait);
        }

        /// <summary>
        /// Remove the given <see cref="Trait"/> instance from this Urt3d.
        /// </summary>
        /// <param name="trait"><see cref="Trait"/> instance to be removed.</param>
        protected void RemoveTrait(Trait trait)
        {
            if (_traits.Contains(trait))
            {
                _traits.Remove(trait);
                OnTraitRemoved?.Invoke(trait);
            }
            else
            {
                Log.Warning($"{nameof(Asset)}.{nameof(RemoveTrait)}: " +
                            $"Trait {trait.Name} is not defined in Urt3d {Metadata.NameInstance}");
            }
        }

        #endregion

        #region Private Methods: Construction and Destruction

        /// <summary>
        /// Protected constructor for Asset instances.
        /// Note: Direct instantiation is not recommended. Please use <see cref="Construct"/> methods instead.
        /// </summary>
        protected Asset()
        {
            _cache.Add(this);
        }
        
        #endregion

        #region Private Variables
        
        private static readonly List<Asset> _cache = new();

        private readonly List<Trait> _traits = new();
        private readonly List<Script> _scripts = new();

        #endregion
    }
}
