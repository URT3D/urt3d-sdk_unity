# URT3D SDK API Reference

This document provides a comprehensive reference for the URT3D SDK API.

## Core Classes

### Urt3d.Asset

The abstract base class for all URT3D assets.

```csharp
public abstract class Asset
{
    // Core Components
    public Actor Actor { get; }
    public Preview Preview { get; }
    public Metadata Metadata { get; }
    
    // Collections
    public ReadOnlyCollection<Trait> Traits { get; }
    public ReadOnlyCollection<Script> Scripts { get; }
    
    // Events
    public Action<Trait> OnTraitAdded;
    public Action<Trait> OnTraitRemoved;
    public Action<Script> OnScriptAdded;
    public Action<Script> OnScriptRemoved;
    
    // Construction Methods
    public static Task<Asset> Construct(string pathToAsset);
    public static Task<Asset> Construct(Guid guid);
    public static void Construct(string pathToAsset, Action<Asset> callback);
    public static void Construct(Guid guid, Action<Asset> callback);
    
    // Destruction
    public void Destroy();
    
    // Traits
    public T GetTrait<T>() where T : Trait;
    
    // Scripts
    public void AddScript(Script script);
    public void RemoveScript(Script script);
}
```

### Urt3d.Wrapper

MonoBehaviour wrapper for URT3D Asset objects, facilitating integration with Unity.

```csharp
public class Wrapper : MonoBehaviour
{
    // Enums
    public enum ModeType
    {
        Local_File = 0,
        Remote_Guid = 1
    }
    
    // Properties
    public string Json { get; set; }
    public ModeType Mode { get; set; }
    public Guid Guid { get; }
    public string Path { get; set; }
    public string PathRaw { get; set; }
    public Asset Asset { get; }
    public Scripting.ScriptExecutionMode ExecutionMode { get; set; }
    
    // Methods
    public void Reload();
    public bool RunScriptReference(Scripting.Script script);
    public void RunScriptsByTrigger(Scripting.ScriptTriggerType triggerType);
    public void TriggerCustomEvent(string eventName);
    public void StopScripts();
}
```

### Urt3d.Actor

Represents the visual 3D model of a URT3D asset.

```csharp
public class Actor
{
    // Properties
    public GameObject GameObject { get; }
    
    // Construction
    public static Task<Actor> Construct(string pathToGlb);
    
    // Destruction
    public void Destroy();
}
```

### Urt3d.Metadata

Contains information about a URT3D asset.

```csharp
public class Metadata
{
    // Properties
    public Guid GuidDefinition { get; }
    public Guid GuidInstance { get; set; }
    public string NameDefinition { get; }
    public string NameInstance { get; set; }
    public Type TypeDefinition { get; }
    public Type TypeInstance { get; set; }
    
    // Construction
    public static Task<Metadata> Construct(string pathToMetadata);
    
    // Destruction
    public void Destroy();
}
```

### Urt3d.Preview

Represents a 2D preview image of a URT3D asset.

```csharp
public class Preview
{
    // Properties
    public Texture2D Texture { get; }
    
    // Construction
    public static Task<Preview> Construct(string pathToImage);
    
    // Destruction
    public void Destroy();
}
```

## Traits System

### Urt3d.Traits.Trait

Base class for all traits.

```csharp
public abstract class Trait
{
    // Properties
    public string Name { get; protected set; }
    public Asset Asset { get; internal set; }
    
    // Methods
    public virtual void Initialize(Asset asset);
    public virtual void Deinitialize();
}
```

### Urt3d.Traits.TraitPosition3d

Trait for controlling 3D position.

```csharp
public class TraitPosition3d : Trait
{
    // Properties
    public Vector3 Value { get; }
    
    // Methods
    public Vector3 GetValue();
    public void SetValue(Vector3 position);
}
```

### Urt3d.Traits.TraitRotation3d

Trait for controlling 3D rotation.

```csharp
public class TraitRotation3d : Trait
{
    // Properties
    public Vector3 Value { get; }
    
    // Methods
    public Vector3 GetValue();
    public void SetValue(Vector3 rotation);
}
```

### Urt3d.Traits.TraitScale3d

Trait for controlling 3D scale.

```csharp
public class TraitScale3d : Trait
{
    // Methods
    public Vector3 GetValue();
    public void SetValue(Vector3 scale);
    public void SetValue(float uniformScale);
}
```

### Urt3d.Traits.TraitAnimatable

Trait for controlling animations.

```csharp
public class TraitAnimatable : Trait
{
    // Events
    public event Action<string> OnAnimationStart;
    public event Action<string> OnAnimationComplete;
    
    // Properties
    public ReadOnlyCollection<string> AvailableAnimations { get; }
    
    // Methods
    public void PlayAnimation(string name, bool loop = false);
    public void StopAnimation(string name);
    public void StopAllAnimations();
    public bool IsPlaying(string name);
}
```

### Urt3d.Traits.TraitInteractable

Trait for handling user interactions.

```csharp
public class TraitInteractable : Trait
{
    // Events
    public event Action<InteractionEventArgs> OnClicked;
    public event Action<InteractionEventArgs> OnHoverEnter;
    public event Action<InteractionEventArgs> OnHoverExit;
    
    // Properties
    public bool IsHovered { get; }
    
    // Methods
    public void EnableInteraction(bool enable);
}
```

## Scripting System

### Urt3d.Scripting.Script

Represents a MiniScript script attached to a URT3D asset.

```csharp
public class Script
{
    // Properties
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public ScriptTriggerType TriggerType { get; set; }
    public string CustomEventName { get; set; }
    public string ScriptContent { get; set; }
    
    // Constructors
    public Script();
    public Script(string name);
}
```

### Urt3d.Scripting.ScriptTriggerType

Defines when a script should be triggered to execute.

```csharp
public enum ScriptTriggerType
{
    OnLoad = 0,
    OnUpdate = 1,
    OnCustomEvent = 2
}
```

### Urt3d.Scripting.ScriptExecutionMode

Defines when scripts should be allowed to execute.

```csharp
public enum ScriptExecutionMode
{
    EditorOnly = 0,
    RuntimeOnly = 1,
    Both = 2
}
```

### Urt3d.Scripting.UrtScriptEngine

Engine for executing MiniScript scripts.

```csharp
public class UrtScriptEngine
{
    // Events
    public event Action<Guid, bool, string> OnScriptCompleted;
    public event Action<Guid, string> OnScriptOutput;
    
    // Constructor
    public UrtScriptEngine(Asset asset);
    
    // Methods
    public bool ExecuteScript(Script script);
    public void ExecuteScriptsByTrigger(List<Script> scripts, ScriptTriggerType triggerType);
    public void ExecuteScriptsByCustomEvent(List<Script> scripts, string eventName);
    public void StopAllScripts();
}
```

## Utility Classes

### Urt3d.Utilities.AssetFinder

Utility for finding URT3D assets.

```csharp
public static class AssetFinder
{
    // Methods
    public static Task<string> Find(Guid guid);
    public static bool IsValid(string path);
}
```

### Urt3d.Utilities.Decrypt

Utility for decrypting URT3D assets.

```csharp
public static class Decrypt
{
    // Methods
    public static Task<string> Extract(string pathToAsset);
}
```

### Urt3d.Utilities.JsonUtil

Utility for JSON serialization/deserialization.

```csharp
public static class JsonUtil
{
    // Methods
    public static string ToJson(object obj);
    public static T FromJson<T>(string json);
    public static void FromJson(string json, object target);
}
```

## Examples

### Loading an Asset from a File Path

```csharp
using Urt3d;
using System.Threading.Tasks;
using UnityEngine;

public class LoadExample : MonoBehaviour
{
    public async Task LoadAsset(string path)
    {
        var asset = await Asset.Construct(path);
        if (asset != null)
        {
            // Position the asset
            asset.GetTrait<Urt3d.Traits.TraitPosition3d>()?.SetValue(Vector3.zero);
            
            // Log success
            Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
        }
    }
}
```

### Triggering a Custom Event

```csharp
using Urt3d;
using UnityEngine;

public class EventExample : MonoBehaviour
{
    public void TriggerJump()
    {
        var wrapper = GetComponent<Urt3d.Wrapper>();
        if (wrapper != null && wrapper.Asset != null)
        {
            wrapper.TriggerCustomEvent("jump");
        }
    }
}
```

### Working with Traits

```csharp
using Urt3d;
using Urt3d.Traits;
using UnityEngine;

public class TraitExample : MonoBehaviour
{
    private Urt3d.Wrapper _wrapper;
    
    private void Start()
    {
        _wrapper = GetComponent<Urt3d.Wrapper>();
    }
    
    public void MoveUp()
    {
        var positionTrait = _wrapper.Asset.GetTrait<TraitPosition3d>();
        if (positionTrait != null)
        {
            var position = positionTrait.GetValue();
            position.y += 1.0f;
            positionTrait.SetValue(position);
        }
    }
    
    public void RotateObject(float angle)
    {
        var rotationTrait = _wrapper.Asset.GetTrait<TraitRotation3d>();
        if (rotationTrait != null)
        {
            var rotation = rotationTrait.GetValue();
            rotation.y += angle;
            rotationTrait.SetValue(rotation);
        }
    }
}
```

## Next Steps

- See [Examples and Best Practices](06-ExamplesAndBestPractices.md) for more code examples
- Explore the [Scripting System](04-ScriptingSystem.md) for MiniScript API details
