# URT3D Examples and Best Practices

This document provides common usage patterns and best practices for working with the URT3D SDK.

## Loading Assets

### Best Practice: Use Async/Await for Sequential Operations

When you need to perform operations in sequence, use the async/await pattern:

```csharp
using Urt3d;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Urt3d.Traits;

public class SequentialLoader : MonoBehaviour
{
    public async Task LoadSequentially(string[] paths)
    {
        foreach (var path in paths)
        {
            var asset = await Asset.Construct(path);
            if (asset != null)
            {
                // Position each asset 2 units apart on X axis
                float xPos = paths.IndexOf(path) * 2.0f;
                asset.GetTrait<TraitPosition3d>()?.SetValue(new Vector3(xPos, 0, 0));
                
                // Wait a bit before loading the next one
                await Task.Delay(500);
            }
        }
        
        Debug.Log("All assets loaded!");
    }
}
```

### Best Practice: Use Callbacks for Fire-and-Forget Operations

When you don't need to wait for the asset to load, use callbacks:

```csharp
using Urt3d;
using System;
using UnityEngine;

public class CatalogLoader : MonoBehaviour
{
    public void LoadCatalog(string[] guids)
    {
        int totalAssets = guids.Length;
        int loadedAssets = 0;
        
        foreach (var guidString in guids)
        {
            if (Guid.TryParse(guidString, out Guid guid))
            {
                Asset.Construct(guid, asset => {
                    loadedAssets++;
                    
                    if (asset != null)
                    {
                        // Do something with the asset
                        Debug.Log($"Loaded: {asset.Metadata.NameInstance}");
                    }
                    
                    if (loadedAssets >= totalAssets)
                    {
                        Debug.Log("All assets loaded!");
                    }
                });
            }
        }
    }
}
```

### Example: Asset Loader UI

This example shows how to create a simple asset loader with progress reporting:

```csharp
using Urt3d;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AssetLoaderUI : MonoBehaviour
{
    [SerializeField] private Transform _contentParent;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Text _statusText;
    
    private List<string> _assetPaths = new List<string>();
    private int _currentIndex = 0;
    
    public void AddAssetPath(string path)
    {
        _assetPaths.Add(path);
        UpdateUI();
    }
    
    public async Task LoadAllAssets()
    {
        _currentIndex = 0;
        int total = _assetPaths.Count;
        
        foreach (var path in _assetPaths)
        {
            _statusText.text = $"Loading {_currentIndex + 1} of {total}";
            _progressSlider.value = (float)_currentIndex / total;
            
            var asset = await Asset.Construct(path);
            if (asset != null)
            {
                // Position in a grid layout
                int row = _currentIndex / 4;
                int col = _currentIndex % 4;
                
                asset.GetTrait<Urt3d.Traits.TraitPosition3d>()?.SetValue(
                    new Vector3(col * 2.0f, 0, row * 2.0f));
                
                asset.Actor.transform.SetParent(_contentParent);
            }
            
            _currentIndex++;
        }
        
        _progressSlider.value = 1.0f;
        _statusText.text = "Loading complete!";
    }
    
    private void UpdateUI()
    {
        _statusText.text = $"Ready to load {_assetPaths.Count} assets";
        _progressSlider.value = 0;
    }
}
```

## Working with Traits

### Best Practice: Always Check if Traits Exist

Never assume an asset has a particular trait:

```csharp
var positionTrait = asset.GetTrait<TraitPosition3d>();
if (positionTrait != null)
{
    // Safe to use the trait
    positionTrait.SetValue(new Vector3(0, 1, 0));
}
else
{
    // Handle the case where the trait is missing
    Debug.LogWarning($"Asset {asset.Metadata.NameInstance} doesn't have Position3d trait!");
}
```

### Best Practice: Use Null Conditional Operator

For concise code, use the null conditional operator with a default value:

```csharp
// Get position, defaulting to Vector3.zero if trait is missing
Vector3 position = asset.GetTrait<TraitPosition3d>()?.GetValue() ?? Vector3.zero;

// Try to set position if trait exists
asset.GetTrait<TraitPosition3d>()?.SetValue(new Vector3(0, 1, 0));
```

### Example: Interactive Object Controller

This example creates a controller for interactive URT3D objects:

```csharp
using Urt3d;
using Urt3d.Traits;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObjectController : MonoBehaviour
{
    [SerializeField] private UnityEvent OnObjectClicked;
    [SerializeField] private float _hoverHeight = 0.5f;
    [SerializeField] private float _hoverSpeed = 2.0f;
    
    private Wrapper _wrapper;
    private TraitInteractable _interactable;
    private TraitPosition3d _position;
    private Vector3 _originalPosition;
    private bool _isHovering = false;
    private float _hoverTime = 0f;
    
    private void Awake()
    {
        _wrapper = GetComponent<Wrapper>();
    }
    
    private void Start()
    {
        if (_wrapper.Asset != null)
        {
            Initialize();
        }
        else
        {
            // Asset might still be loading, wait for it
            _wrapper.OnAssetLoaded += Initialize;
        }
    }
    
    private void Initialize()
    {
        _interactable = _wrapper.Asset.GetTrait<TraitInteractable>();
        _position = _wrapper.Asset.GetTrait<TraitPosition3d>();
        
        if (_interactable != null && _position != null)
        {
            _originalPosition = _position.GetValue();
            
            // Subscribe to interaction events
            _interactable.OnHoverEnter += HandleHoverEnter;
            _interactable.OnHoverExit += HandleHoverExit;
            _interactable.OnClicked += HandleClicked;
        }
        else
        {
            Debug.LogWarning("Asset missing required traits for interactive behavior");
            enabled = false;
        }
    }
    
    private void Update()
    {
        if (_isHovering && _position != null)
        {
            _hoverTime += Time.deltaTime * _hoverSpeed;
            
            // Calculate hovering effect (bobbing up and down)
            float offset = Mathf.Sin(_hoverTime) * _hoverHeight;
            Vector3 newPos = _originalPosition;
            newPos.y += offset;
            
            _position.SetValue(newPos);
        }
    }
    
    private void HandleHoverEnter(InteractionEventArgs args)
    {
        _isHovering = true;
    }
    
    private void HandleHoverExit(InteractionEventArgs args)
    {
        _isHovering = false;
        // Return to original position
        _position?.SetValue(_originalPosition);
    }
    
    private void HandleClicked(InteractionEventArgs args)
    {
        OnObjectClicked?.Invoke();
    }
    
    private void OnDestroy()
    {
        if (_interactable != null)
        {
            _interactable.OnHoverEnter -= HandleHoverEnter;
            _interactable.OnHoverExit -= HandleHoverExit;
            _interactable.OnClicked -= HandleClicked;
        }
    }
}
```

## Working with Scripts

### Best Practice: Use Custom Events for Communication

Use custom events to communicate between C# and MiniScript:

```csharp
// C# Code
public void OnPlayerApproach()
{
    float distance = Vector3.Distance(playerPosition, transform.position);
    
    // Trigger different events based on distance
    var wrapper = GetComponent<Urt3d.Wrapper>();
    
    if (distance < 2.0f)
    {
        wrapper.TriggerCustomEvent("player_near");
    }
    else if (distance < 5.0f)
    {
        wrapper.TriggerCustomEvent("player_medium");
    }
    else
    {
        wrapper.TriggerCustomEvent("player_far");
    }
}
```

Corresponding MiniScript:
```
function OnCustomEvent(eventName)
    if eventName == "player_near" then
        // React to player being very close
        anim = GetTrait("Animatable")
        anim.PlayAnimation("Greeting")
    else if eventName == "player_medium" then
        // React to player being at medium distance
        anim = GetTrait("Animatable")
        anim.PlayAnimation("Wave")
    else if eventName == "player_far" then
        // Reset to idle state
        anim = GetTrait("Animatable")
        anim.PlayAnimation("Idle")
    end if
end function
```

### Example: Script-Driven Animation Controller

This example shows how to control animation through C# and MiniScript:

```csharp
using Urt3d;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private string _idleAnimName = "Idle";
    [SerializeField] private string _walkAnimName = "Walk";
    [SerializeField] private string _runAnimName = "Run";
    [SerializeField] private string _jumpAnimName = "Jump";
    
    private Wrapper _wrapper;
    
    private void Awake()
    {
        _wrapper = GetComponent<Wrapper>();
    }
    
    public void SetIdle()
    {
        _wrapper.TriggerCustomEvent("play_anim_idle");
    }
    
    public void SetWalking()
    {
        _wrapper.TriggerCustomEvent("play_anim_walk");
    }
    
    public void SetRunning()
    {
        _wrapper.TriggerCustomEvent("play_anim_run");
    }
    
    public void Jump()
    {
        _wrapper.TriggerCustomEvent("play_anim_jump");
    }
}
```

Corresponding MiniScript:
```
anim = GetTrait("Animatable")
idleAnim = "Idle"
walkAnim = "Walk"
runAnim = "Run"
jumpAnim = "Jump"
currentAnim = ""

function OnStart()
    // Start with idle animation
    PlayAnim(idleAnim, true)
end function

function OnCustomEvent(eventName)
    if eventName == "play_anim_idle" then
        PlayAnim(idleAnim, true)
    else if eventName == "play_anim_walk" then
        PlayAnim(walkAnim, true)
    else if eventName == "play_anim_run" then
        PlayAnim(runAnim, true)
    else if eventName == "play_anim_jump" then
        PlayAnim(jumpAnim, false)
    end if
end function

function PlayAnim(animName, loop)
    if animName != currentAnim then
        anim.PlayAnimation(animName, loop)
        currentAnim = animName
    end if
end function
```

## Project Organization

### Best Practice: Create Asset Categories

Organize your URT3D assets into categories:

```
Assets/
  URT3D/
    Characters/
      Hero.urta
      NPC.urta
    Props/
      Furniture/
        Chair.urta
        Table.urta
      Weapons/
        Sword.urta
    Environments/
      Trees/
        Oak.urta
        Pine.urta
```

### Best Practice: Create Asset Prefabs

For commonly used URT3D assets with specific configurations:

1. Drag a URT3D asset into the scene
2. Configure its traits, scripts, etc.
3. Drag the GameObject back into the Project panel to create a prefab
4. Use this prefab whenever you need that specific configuration

### Best Practice: Create Reusable Controllers

Create reusable C# controllers for common URT3D behaviors:

```csharp
// A reusable rotation controller
[AddComponentMenu("URT3D/Controllers/Rotate Object")]
public class RotateObjectController : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationSpeed = new Vector3(0, 30, 0);
    [SerializeField] private Space _rotationSpace = Space.World;
    
    private Wrapper _wrapper;
    private TraitRotation3d _rotation;
    
    private void Awake()
    {
        _wrapper = GetComponent<Wrapper>();
    }
    
    private void Start()
    {
        if (_wrapper.Asset != null)
        {
            Initialize();
        }
        else
        {
            _wrapper.OnAssetLoaded += Initialize;
        }
    }
    
    private void Initialize()
    {
        _rotation = _wrapper.Asset.GetTrait<TraitRotation3d>();
        if (_rotation == null)
        {
            Debug.LogWarning("Asset doesn't have Rotation3d trait!");
            enabled = false;
        }
    }
    
    private void Update()
    {
        if (_rotation == null) return;
        
        Vector3 currentRotation = _rotation.GetValue();
        
        if (_rotationSpace == Space.World)
        {
            currentRotation += _rotationSpeed * Time.deltaTime;
        }
        else
        {
            // Calculate local space rotation
            // ...
        }
        
        _rotation.SetValue(currentRotation);
    }
}
```

## Performance Optimization

### Best Practice: Pool Common Assets

For frequently created/destroyed assets, use object pooling:

```csharp
using Urt3d;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class URT3DObjectPool : MonoBehaviour
{
    [Serializable]
    public class PoolItem
    {
        public string Name;
        public string Path;
        public int InitialCount = 5;
    }
    
    [SerializeField] private List<PoolItem> _poolItems = new List<PoolItem>();
    
    private Dictionary<string, Queue<Asset>> _pools = new Dictionary<string, Queue<Asset>>();
    private Dictionary<string, string> _pathLookup = new Dictionary<string, string>();
    
    private async void Start()
    {
        await InitializePools();
    }
    
    private async Task InitializePools()
    {
        foreach (var item in _poolItems)
        {
            _pools[item.Name] = new Queue<Asset>();
            _pathLookup[item.Name] = item.Path;
            
            // Pre-create assets
            for (int i = 0; i < item.InitialCount; i++)
            {
                var asset = await Asset.Construct(item.Path);
                if (asset != null)
                {
                    // Hide the asset
                    asset.Actor.GameObject.SetActive(false);
                    
                    // Add to pool
                    _pools[item.Name].Enqueue(asset);
                }
            }
        }
    }
    
    public Asset GetAsset(string name)
    {
        if (!_pools.ContainsKey(name))
        {
            Debug.LogError($"Pool does not contain assets named {name}");
            return null;
        }
        
        if (_pools[name].Count > 0)
        {
            var asset = _pools[name].Dequeue();
            asset.Actor.GameObject.SetActive(true);
            return asset;
        }
        else
        {
            // Create a new asset if pool is empty
            Debug.Log($"Pool for {name} is empty, creating new asset");
            Asset.Construct(_pathLookup[name], asset => {
                if (asset != null)
                {
                    asset.Actor.GameObject.SetActive(true);
                }
            });
            return null;
        }
    }
    
    public void ReturnAsset(string name, Asset asset)
    {
        if (!_pools.ContainsKey(name))
        {
            Debug.LogError($"Pool does not contain assets named {name}");
            return;
        }
        
        if (asset != null)
        {
            // Hide the asset
            asset.Actor.GameObject.SetActive(false);
            
            // Return to pool
            _pools[name].Enqueue(asset);
        }
    }
}
```

### Best Practice: Optimize Script Execution

For better performance with MiniScript:

1. Use OnLoad scripts for initialization
2. Limit the number of OnUpdate scripts 
3. Use OnCustomEvent for infrequent operations
4. Keep scripts small and focused

## Next Steps

- Review the full [API Reference](05-APIReference.md)
- Explore the [Scripting System](04-ScriptingSystem.md) in more detail
- Join the URT3D community forum at [community.urt3d.com](https://community.urt3d.com)
