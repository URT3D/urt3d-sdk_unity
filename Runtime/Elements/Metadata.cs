using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Urt3d.Configs;
using Urt3d.Entities;
using Urt3d.Symbols;
using Urt3d.Utilities;
using System.Collections.Generic;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Represents the complete metadata system for a URT3D Asset.
    /// This top-level class combines definition and instance metadata, providing
    /// a unified interface for metadata operations. It includes factory methods for
    /// construction from files and manages the complete lifecycle of metadata
    /// including loading, parsing, instantiation, and cleanup.
    /// 
    /// Metadata contains essential information about a URT3D asset including its identity,
    /// name, type, and references to associated assets and scripts. This class inherits 
    /// from MetadataInstance and is the primary entry point for working with URT3D
    /// asset metadata throughout the SDK.
    /// </summary>
    public class Metadata : MetadataInstance
    {
        /// <inheritdoc cref="MetadataInstance(Guid,string,string)"/>
        public Metadata(Guid guid, string name, string type) : base(guid, name, type)
        {
            // NO-OP
        }

        /// <inheritdoc cref="MetadataInstance(MetadataDefinition)"/>
        public Metadata(MetadataDefinition metadataDefinition) : base(metadataDefinition)
        {
            // NO-OP
        }

        /// <inheritdoc cref="MetadataInstance.Construct(string)"/>
        public new static async Task<Metadata> Construct(string pathToMetadataFile)
        {
            var metadata = await MetadataInstance.Construct(pathToMetadataFile);
            return new Metadata(metadata);
        }

        /// <inheritdoc cref="MetadataInstance.Construct(string,Action{MetadataDefinition})"/>
        public static void Construct(string pathToMetadataFile, Action<Metadata> callback)
        {
            MetadataInstance.Construct(pathToMetadataFile, metadata =>
            {
                callback?.Invoke(new Metadata(metadata));
            });
        }
    }

    /// <summary>
    /// Extends MetadataDefinition to provide runtime instance-specific metadata.
    /// MetadataInstance maintains both the original definition metadata and instance-specific
    /// values that can change during runtime. This class handles the duality of maintaining
    /// both the original definition (template) and the current instance state of an asset's metadata.
    /// 
    /// The instance version is what's used at runtime and can be modified, while preserving
    /// the original definition values for reference and reset purposes.
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public class MetadataInstance : MetadataDefinition
    {
        #region Events

        /// <summary>
        /// Event fired when the instance name changes.
        /// </summary>
        public Action<string> OnNameInstanceChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// GUID unique to the file from which this URT3D Asset was loaded.
        /// </summary>
        public Guid GuidDefinition => Guid;

        /// <summary>
        /// GUID unique to this instance of this URT3D Asset.
        /// </summary>
        public Guid GuidInstance { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Name defined by the file from which this URT3D Asset was loaded.
        /// </summary>
        public string NameDefinition => Name;

        /// <summary>
        /// Named defined by the instance of this URT3D Asset.
        /// </summary>
        public string NameInstance
        {
            get => !string.IsNullOrEmpty(_nameInstance) ? _nameInstance : NameDefinition;

            set
            {
                _nameInstance = value;
                OnNameInstanceChanged?.Invoke(value);
            }
        }
        private string _nameInstance = null;

        /// <summary>
        /// Type defined by the file from which this URT3D Asset was loaded.
        /// </summary>
        public string TypeDefinition => Type;

        /// <summary>
        /// Type instance resolved from the type name, with namespace resolution support.
        /// </summary>
        public Type TypeInstance
        {
            get
            {
                if (TypeCache.TryGetValue(Type, out var cachedType))
                {
                    return cachedType;
                }

                try
                {
                    // First try direct type lookup with current type name
                    var type = System.Type.GetType(Type);
                    if (type != null)
                    {
                        TypeCache[Type] = type;
                        return type;
                    }

                    // Define known base types and their fully qualified names
                    var baseTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Entity", "Urt3d.Entities.Entity" },
                        { "Config", "Urt3d.Configs.Config" },
                        { "Symbol", "Urt3d.Symbols.Symbol" }
                        // Add any other base types here
                    };

                    // Check if this is a base type name and try to resolve it
                    if (baseTypeMap.TryGetValue(Type, out var fullyQualifiedName))
                    {
                        // Try to resolve the fully qualified name
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            type = assembly.GetType(fullyQualifiedName);
                            if (type != null)
                            {
                                TypeCache[Type] = type;
                                return type;
                            }
                        }
                    }

                    // If still not found, try searching in all assemblies with current type name
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        // Try in Urt3d namespace first
                        type = assembly.GetType($"Urt3d.{Type}");
                        if (type != null)
                        {
                            TypeCache[Type] = type;
                            return type;
                        }

                        // Then try in specific Urt3d sub-namespaces
                        var subNamespaces = new[] { "Entities", "Configs", "Symbols" };
                        foreach (var ns in subNamespaces)
                        {
                            type = assembly.GetType($"Urt3d.{ns}.{Type}");
                            if (type != null)
                            {
                                TypeCache[Type] = type;
                                return type;
                            }
                        }
                    }

                    // If still not found, try the original fallback logic
                    if (Type.Contains(nameof(Configs)))
                    {
                        Log.Warning($"Failed to locate URT3D Asset type {Type}, falling back to {typeof(Config)}");
                        return typeof(Config);
                    }
                    if (Type.Contains(nameof(Entities)))
                    {
                        Log.Warning($"Failed to locate URT3D Asset type {Type}, falling back to {typeof(Entity)}");
                        return typeof(Entity);
                    }
                    if (Type.Contains(nameof(Symbols)))
                    {
                        Log.Warning($"Failed to locate URT3D Asset type {Type}, falling back to {typeof(Symbol)}");
                        return typeof(Symbol);
                    }

                    // If all resolution attempts fail, log error and return null
                    Log.Error($"Failed to locate URT3D Asset type: {Type}, unable to fall back");
                    return null;
                }
                catch (Exception ex)
                {
                    Log.Error($"Error resolving type {Type}: {ex.Message}");
                    return null;
                }
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc cref="MetadataDefinition(Guid,string,string)"/>
        public MetadataInstance(Guid guid, string name, string type) : base(guid, name, type)
        {
            // NO-OP
        }

        /// <summary>
        /// Constructs a new MetadataInstance from a MetadataDefinition.
        /// This constructor initializes a runtime instance of metadata using
        /// the information provided in the definition.
        /// </summary>
        /// <param name="metadataDefinition">The metadata definition to construct from.</param>
        public MetadataInstance(MetadataDefinition metadataDefinition) : base(metadataDefinition.Guid, 
                                                                              metadataDefinition.Name,
                                                                              metadataDefinition.Type)
        {
            BundledAssets = metadataDefinition.BundledAssets;
            BundledScripts = metadataDefinition.BundledScripts;
        }

        /// <inheritdoc cref="MetadataDefinition.Construct(string)"/>
        public new static async Task<MetadataInstance> Construct(string pathToMetadataFile)
        {
            var metadata = await MetadataDefinition.Construct(pathToMetadataFile);
            return new MetadataInstance(metadata);
        }

        /// <inheritdoc cref="MetadataDefinition.Construct(string,Action{MetadataDefinition})"/>
        public static void Construct(string pathToMetadataFile, Action<MetadataInstance> callback)
        {
            MetadataDefinition.Construct(pathToMetadataFile, metadata =>
            {
                callback?.Invoke(new MetadataInstance(metadata));
            });
        }

        #endregion
    }

    /// <summary>
    /// Represents the base metadata structure that's stored in a URT3D Asset file.
    /// The MetadataDefinition class contains the essential identifying information for a URT3D Asset,
    /// including its GUID, name, and type, as well as references to any bundled assets or scripts.
    /// This class provides the foundation for asset identification and serialization within the URT3D system.
    /// </summary>
    public class MetadataDefinition
    {
        #region Properties

        /// <summary>
        /// GUID as defined in URT3D Asset file.
        /// </summary>
        [JsonProperty]
        protected internal Guid Guid { get; private set; } = Guid.Empty;

        /// <summary>
        /// Name as defined in URT3D Asset file.
        /// </summary>
        [JsonProperty]
        protected internal string Name { get; private set; } = "Default Name";

        /// <summary>
        /// Type as defined in URT3D Asset file.
        /// </summary>
        [JsonProperty]
        protected internal string Type { get; private set; } = typeof(Entity).ToString();

        /// <summary>
        /// Array of absolute paths to assets bundled with this URT3D Asset.
        /// </summary>
        [JsonIgnore]
        public string[] BundledAssets { get; protected set; } = Array.Empty<string>();

        /// <summary>
        /// Array of absolute paths to scripts bundled with this URT3D Asset.
        /// </summary>
        [JsonIgnore]
        public string[] BundledScripts { get; protected set; } = Array.Empty<string>();

        #endregion

        #region Methods

        /// <summary>
        /// Default constructor is not available, please use <see cref="Construct"/> instead.
        /// </summary>
        private MetadataDefinition()
        {
            // NO-OP
        }

        public MetadataDefinition(Guid guid, string name, string type)
        {
            Guid = guid;
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Properly dispose of this object.
        /// </summary>
        public virtual void Destroy()
        {
            // NO-OP
        }

        /// <summary>
        /// Constructs a new MetadataDefinition instance from a metadata file.
        /// This asynchronous method loads and parses the metadata file, creating a properly
        /// initialized MetadataDefinition object with the values specified in the file.
        /// </summary>
        /// <param name="pathToMetadataFile">Fully qualified path to the Metadata file.</param>
        /// <returns>A Task that resolves to the constructed MetadataDefinition object, or null if construction fails</returns>
        public static async Task<MetadataDefinition> Construct(string pathToMetadataFile)
        {
            // Validate path to metadata file
            if (!File.Exists(pathToMetadataFile))
            {
                Log.Error($"Failed to locate metadata file at path: {pathToMetadataFile}");
                return new MetadataDefinition();
            }

            // Deserialize and validate content of metadata file
            var json = await File.ReadAllTextAsync(pathToMetadataFile);
            var metadata = JsonConvert.DeserializeObject<MetadataDefinition>(json);

            if (metadata == null)
            {
                Log.Error($"Encountered malformed metadata file at path: {pathToMetadataFile}");
                return new MetadataDefinition();
            }

            // Ensure backwards compatability with Bonfire-generated content, including legacy Sparks
            metadata.Type = metadata.Type.Replace("DimX.Common.Assets.Types.Sparks.", $"{nameof(Urt3d)}.");
            metadata.Type = metadata.Type.Replace("Sparks.", $"{nameof(Urt3d)}.");
            metadata.Type = metadata.Type.Replace("Urt3d.", $"{nameof(Urt3d)}.");

            // Located bundled assets and scripts
            var root = Path.GetDirectoryName(pathToMetadataFile);

            var pathAssets = Path.Combine(root, "Assets");
            if (Directory.Exists(pathAssets))
            {
                metadata.BundledAssets = Directory.GetFiles(pathAssets, "*.*", SearchOption.AllDirectories);
            }

            var pathScripts = Path.Combine(root, "Scripts");
            if (Directory.Exists(pathScripts))
            {
                metadata.BundledScripts = Directory.GetFiles(pathScripts, "*.*", SearchOption.AllDirectories);
            }

            // Return result
            return metadata;
        }

        /// <summary>
        /// Constructs a new MetadataDefinition instance from a metadata file using a callback approach.
        /// This method provides a callback-based alternative to the async Construct method,
        /// executing the callback on the Unity main thread when construction completes.
        /// </summary>
        /// <param name="pathToMetadataFile">Fully qualified path to the Metadata file.</param>
        /// <param name="callback">Callback to invoke when construction completes, with the MetadataDefinition as parameter</param>
        public static void Construct(string pathToMetadataFile, Action<MetadataDefinition> callback)
        {
            var task = Construct(pathToMetadataFile);
            task.ContinueWith(action =>
            {
                if (action.IsFaulted)
                {
                    Log.Exception(action.Exception);
                }
                callback?.Invoke(action.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        private static readonly Dictionary<string, string> BaseTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Entity", "Urt3d.Entities.Entity" },
            { "Config", "Urt3d.Configs.Config" },
            { "Symbol", "Urt3d.Symbols.Symbol" }
        };

        /// <summary>
        /// Registers a base type mapping for type resolution.
        /// </summary>
        /// <param name="baseTypeName">The simple name of the type</param>
        /// <param name="fullyQualifiedName">The fully qualified name including namespace</param>
        public static void RegisterBaseType(string baseTypeName, string fullyQualifiedName)
        {
            if (string.IsNullOrEmpty(baseTypeName) || string.IsNullOrEmpty(fullyQualifiedName))
            {
                Log.Warning("Cannot register base type with empty name or qualified name");
                return;
            }

            BaseTypeMap[baseTypeName] = fullyQualifiedName;
        }
        protected static readonly Dictionary<string, Type> TypeCache = new();
    }
}
