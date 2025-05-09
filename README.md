# URT3D SDK for Unity

The URT3D SDK enables Unity developers to load, display, and interact with URT3D assets - a specialized 3D format designed for interactive experiences. This SDK provides a comprehensive set of tools for incorporating URT3D content into Unity projects with minimal effort.

## Features

- **Asset Management**: Load URT3D assets from local files or remote sources using GUIDs
- **Component System**: Pre-built components for easy integration with Unity's GameObject hierarchy
- **Scripting System**: Built-in MiniScript interpreter for dynamic behaviors
- **Traits Framework**: Extensible traits system for adding functionality to assets
- **Editor Tools**: Custom inspectors and tools for working with URT3D assets
- **Sample Content**: Example assets and scenes to help you get started

## Requirements

- Unity 6 or newer
- C# 8.0 or newer

## Installation

### Using Unity Package Manager (via Git URL)

1. Open your Unity project
2. Go to Window > Package Manager
3. Click the "+" button in the top-left corner
4. Select "Add package from git URL..."
5. Enter the following URL:
   ```
   https://github.com/URT3D/urt3d-sdk_unity.git
   ```
6. Click "Add"

### Manual Installation

1. Clone this repository
2. In your Unity project, navigate to the Packages folder
3. Copy the cloned repository into this folder
4. Open your Unity project and the package will be available

## Usage

### Adding a URT3D Asset to a Scene

Simply drag and drop a URT3D asset (*.urta) directly into your scene hierarchy. The SDK will automatically:

1. Create a new GameObject with all necessary components
2. Set up and load the asset in your scene
3. Provide a URT3D Inspector that allows you to:
   - Switch between "Remote" and "Local" loading modes
   - View and edit asset metadata
   - Manage traits 
   - Create and edit scripts

No manual component setup is required.

### Using Scripts with URT3D Assets

URT3D assets can include MiniScript scripts that define behavior. Through the URT3D Inspector, you can manage scripts that can be triggered:

- **OnLoad**: When the asset is first loaded
- **OnUpdate**: Every frame during the Update cycle
- **OnCustomEvent**: When a custom event is triggered by name

You can configure script execution modes for each asset:

- **EditorOnly**: Scripts execute only in the Unity Editor
- **RuntimeOnly**: Scripts execute only during Play mode
- **Both**: Scripts execute in both Editor and Play mode

```csharp
// Example: Triggering a custom event on a URT3D asset
var wrapper = GetComponent<Urt3d.Wrapper>();
wrapper.TriggerCustomEvent("myCustomEvent");
```

### Working with Asset Traits

Traits add specific functionality to URT3D assets. The URT3D Inspector provides a user-friendly interface to manage traits. To access a trait programmatically:

```csharp
var wrapper = GetComponent<Urt3d.Wrapper>();
var myTrait = wrapper.Asset.GetTrait<MyCustomTrait>();
if (myTrait != null)
{
    myTrait.DoSomething();
}
```

### Loading URT3D Assets at Runtime

In addition to drag-and-drop in the editor, you can load URT3D assets programmatically at runtime:

#### Using async/await pattern:

```csharp

public class AssetLoader : MonoBehaviour
{
    // Load via file path
    public async Task LoadFromPathAsync(string path)
    {
        var asset = await Asset.Construct(path);
        if (asset != null)
        {
            // Set initial position and rotation data
            asset.GetTrait<TraitPosition3d>()?.SetValue(Vector3.zero);
            asset.GetTrait<TraitRotation3d>()?.SetValue(Vector3.zero);
            
            // Now you can work with the asset
            Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
        }
    }
    
    // Load via GUID
    public async Task LoadFromGuidAsync(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            var asset = await Asset.Construct(guid);
            if (asset != null)
            {
                // Set initial position and rotation data
                asset.GetTrait<TraitPosition3d>()?.SetValue(Vector3.zero);
                asset.GetTrait<TraitRotation3d>()?.SetValue(Vector3.zero);
                
                // Now you can work with the asset
                Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
            }
        }
    }
}
```

#### Using callback pattern:

```csharp

public class AssetLoader : MonoBehaviour
{
    // Load via file path
    public void LoadFromPath(string path)
    {
        Asset.Construct(path, asset =>
        {
            if (asset == null) return;

            // Set initial position and rotation data
            asset.GetTrait<TraitPosition3d>()?.SetValue(Vector3.zero);
            asset.GetTrait<TraitRotation3d>()?.SetValue(Vector3.zero);
                
            // Now you can work with the asset
            Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
        });
    }
    
    // Load via GUID
    public void LoadFromGuid(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            Asset.Construct(guid, asset =>
            {
                if (asset == null) return;

                // Set initial position and rotation data
                asset.GetTrait<TraitPosition3d>()?.SetValue(Vector3.zero);
                asset.GetTrait<TraitRotation3d>()?.SetValue(Vector3.zero);
                    
                // Now you can work with the asset
                Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
            });
        }
    }
}
```

## Documentation

Comprehensive documentation for the URT3D SDK is available at https://docs.urt3d.com

The SDK architecture consists of several key components:

- **Asset**: The core class representing a URT3D object
- **Actor**: The visual representation (3D model)
- **Preview**: A 2D image representation
- **Metadata**: Asset information and properties
- **Wrapper**: MonoBehaviour for easy integration with Unity scenes
- **Traits**: Extensions that add functionality to assets
- **Scripts**: MiniScript code that defines dynamic behavior

## Samples

The SDK includes sample scenes to help you get started:

- **Empty Scene**: A basic scene setup for adding your own URT3D assets
- **URT3D Scene**: A demo scene with pre-configured URT3D assets

Sample content:
- **Duck Encrypted**: An example of an encrypted URT3D asset
- **Duck Unencrypted**: An example of an unencrypted URT3D asset

## Support

For support, please visit [URT3D Support](https://urt3d.com) or contact us at [info@urt3d.com](mailto:info@urt3d.com).
