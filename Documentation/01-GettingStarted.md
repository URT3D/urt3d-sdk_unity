# Getting Started with URT3D SDK

This guide will help you get up and running with the URT3D SDK for Unity.

## Installation

### Prerequisites

- Unity 6000.0 or newer
- C# 8.0 or newer

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

1. Clone the repository
2. In your Unity project, navigate to the Packages folder
3. Copy the cloned repository into this folder
4. Open your Unity project and the package will be available

## Your First URT3D Asset

### Adding a URT3D Asset to a Scene

The easiest way to work with URT3D assets is to simply drag and drop a .urta file into your scene hierarchy:

1. Find a URT3D asset file (.urta) in your project
2. Drag it directly into your scene hierarchy in the Unity Editor
3. The SDK will automatically create a GameObject with the necessary components

The SDK automatically provides a custom inspector that allows you to:
- Switch between "Remote" and "Local" loading modes
- View and edit asset metadata
- Create and edit scripts
- Manage traits

### Sample Scenes

The SDK includes two sample scenes to help you get started:

- **Empty Scene**: A basic scene ready for you to add URT3D assets
- **URT3D Scene**: A pre-configured scene demonstrating URT3D assets in action

To open a sample scene:
1. Navigate to URT3D-SDK/Samples/Scenes in your Project panel
2. Double-click on either "Empty Scene.unity" or "URT3D Scene.unity"

### Sample Assets

The SDK includes sample URT3D assets in URT3D-SDK/Samples/Content:

- **Duck Encrypted**: An example of an encrypted URT3D asset
- **Duck Unencrypted**: An example of an unencrypted URT3D asset

Try dragging one of these assets into your scene to see how it works.

## What's Next?

Now that you've got the basics, consider exploring:

- [Core Concepts](02-CoreConcepts.md) to understand the URT3D architecture
- [Scripting Guide](04-ScriptingSystem.md) to learn how to add behavior to your assets
- [Traits Guide](03-TraitsSystem.md) to understand how to extend asset functionality
- [Examples and Best Practices](06-ExamplesAndBestPractices.md) for common usage patterns

For a full reference of the API, see the [API Reference](05-APIReference.md).
