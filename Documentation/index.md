# URT3D SDK Documentation

Welcome to the URT3D SDK for Unity documentation. This guide will help you get started with loading, displaying, and interacting with URT3D assets in your Unity projects.

## Documentation Sections

### Getting Started
- [Getting Started Guide](01-GettingStarted.md) - Installation and basic usage

### Core Documentation
- [Core Concepts](02-CoreConcepts.md) - Understand the URT3D architecture and key components
- [Traits System](03-TraitsSystem.md) - Learn about extending asset functionality
- [Scripting System](04-ScriptingSystem.md) - Add dynamic behaviors with MiniScript
- [API Reference](05-APIReference.md) - Detailed class and method references
- [Examples and Best Practices](06-ExamplesAndBestPractices.md) - Code examples and recommended approaches

## Quick Links

### Installation

```
// Via Unity Package Manager
https://github.com/URT3D/urt3d-sdk_unity.git
```

### Basic Usage

```csharp
// Loading a URT3D asset from a file path
Asset.Construct("/path/to/asset.urta", asset => {
    if (asset != null) {
        Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
    }
});

// Loading a URT3D asset by GUID
if (Guid.TryParse(guidString, out var guid)) {
    Asset.Construct(guid, asset => {
        if (asset != null) {
            Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
        }
    });
}
```

### Support

For additional support:
- Visit [docs.urt3d.com](https://docs.urt3d.com) for online documentation
- Contact [support@urt3d.com](mailto:support@urt3d.com) for technical assistance
- Visit [urt3d.com](https://urt3d.com) for general information
