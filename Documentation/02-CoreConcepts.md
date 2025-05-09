# URT3D Core Concepts

This document explains the fundamental concepts and architecture of the URT3D SDK.

## Architecture Overview

The URT3D SDK is built around a modular architecture with several key components:

// Architecture diagram would be shown here

### Key Components

#### Asset

The `Asset` class is the foundational element of the URT3D system. It represents a complete URT3D object and contains:

- Visual representation (Actor)
- 2D preview image (Preview)
- Metadata
- Traits (optional extensions)
- Scripts (optional behaviors)

```csharp
// The Asset class is abstract and must be extended
public abstract class Asset
{
    // Core elements
    public Actor Actor { get; }
    public Preview Preview { get; }
    public Metadata Metadata { get; }
    
    // Collections
    public ReadOnlyCollection<Trait> Traits { get; }
    public ReadOnlyCollection<Script> Scripts { get; }
    
    // Construction methods
    public static Task<Asset> Construct(string path);
    public static Task<Asset> Construct(Guid guid);
    // ...
}
```

#### Actor

The `Actor` represents the visual, 3D representation of a URT3D asset. It typically contains:

- 3D model data (loaded from GLB/GLTF)
- Transform information
- Rendering properties

#### Preview

The `Preview` provides a 2D image representation of the URT3D asset, useful for:

- Thumbnails in asset browsers
- UI elements
- Loading screens while the 3D asset is being prepared

#### Metadata

The `Metadata` contains descriptive information about the URT3D asset:

- GUID identifiers
- Names (instance and definition)
- Type information
- Custom properties

#### Wrapper

The `Wrapper` is a Unity MonoBehaviour that facilitates integration of URT3D assets into Unity scenes:

- Handles loading assets from files or GUIDs
- Manages lifecycle (initialization, updating, cleanup)
- Provides inspector integration
- Serializes settings with Unity scenes

## Loading and Management

### Asset Loading

URT3D assets can be loaded in several ways:

1. **Editor Drag and Drop**: Simply drag a .urta file into your scene hierarchy
2. **Programmatic Loading via Path**:
   ```csharp
   var asset = await Asset.Construct("/path/to/asset.urta");
   ```
3. **Programmatic Loading via GUID**:
   ```csharp
   var asset = await Asset.Construct(guid);
   ```

### Loading Modes

The SDK supports two primary loading modes:

- **Local File**: Loads assets from the local filesystem
- **Remote GUID**: Loads assets from a remote source using their GUID

## File Format

URT3D assets (.urta files) are packaged files that contain:

- A 3D model (GLB/GLTF)
- A preview image (JPG/PNG)
- Metadata (JSON)
- Optional additional resources

These files can be:
- **Encrypted**: For secure distribution
- **Unencrypted**: For easier development and debugging

## Next Steps

- Learn about the [Traits System](03-TraitsSystem.md)
- Explore the [Scripting System](04-ScriptingSystem.md)
- Check the [API Reference](05-APIReference.md) for detailed class information
