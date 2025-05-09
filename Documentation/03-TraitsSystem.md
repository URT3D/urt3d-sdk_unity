# URT3D Traits System

The Traits system is a key feature of the URT3D SDK, providing a flexible way to extend asset functionality.

## What are Traits?

Traits are reusable components that add specific capabilities or properties to URT3D assets. Each trait represents a discrete piece of functionality that can be added to any URT3D asset.

Think of traits as a more flexible alternative to inheritance - instead of creating deep inheritance hierarchies, URT3D assets can be composed by adding various traits as needed.

## Available Traits

The URT3D SDK includes several built-in traits:

### TraitPosition3d

Provides position control for URT3D assets in 3D space.

```csharp
// Setting position
asset.GetTrait<TraitPosition3d>()?.SetValue(new Vector3(0, 1, 0));

// Getting position
Vector3 position = asset.GetTrait<TraitPosition3d>()?.GetValue() ?? Vector3.zero;
```

### TraitRotation3d

Handles rotation of URT3D assets in 3D space.

```csharp
// Setting rotation (in Euler angles)
asset.GetTrait<TraitRotation3d>()?.SetValue(new Vector3(0, 90, 0));

// Getting rotation
Vector3 rotation = asset.GetTrait<TraitRotation3d>()?.GetValue() ?? Vector3.zero;
```

### TraitScale3d

Manages scaling of URT3D assets.

```csharp
// Setting uniform scale
asset.GetTrait<TraitScale3d>()?.SetValue(2.0f);

// Setting non-uniform scale
asset.GetTrait<TraitScale3d>()?.SetValue(new Vector3(1, 2, 1));
```

### TraitAnimatable

Provides animation control for URT3D assets that have animation capability.

```csharp
// Play an animation by name
asset.GetTrait<TraitAnimatable>()?.PlayAnimation("Walk");

// Stop all animations
asset.GetTrait<TraitAnimatable>()?.StopAllAnimations();
```

### TraitInteractable

Makes URT3D assets respond to user interaction like clicks and hovers.

```csharp
// Subscribe to interaction events
var interactable = asset.GetTrait<TraitInteractable>();
if (interactable != null)
{
    interactable.OnClicked += HandleClick;
    interactable.OnHoverEnter += HandleHoverEnter;
    interactable.OnHoverExit += HandleHoverExit;
}

// Handler method
private void HandleClick(InteractionEventArgs args)
{
    Debug.Log($"Asset clicked: {args.Asset.Metadata.NameInstance}");
}
```

## Using Traits in Code

### Accessing Traits

You can access traits through the URT3D asset:

```csharp
// Get a reference to a Wrapper component
var wrapper = GetComponent<Urt3d.Wrapper>();

// Access a trait from its asset
var positionTrait = wrapper.Asset.GetTrait<TraitPosition3d>();
if (positionTrait != null)
{
    // Use the trait
    positionTrait.SetValue(new Vector3(0, 1, 0));
}
```

### Checking if a Trait Exists

Always check if a trait exists before using it, as not all assets will have all traits:

```csharp
if (asset.GetTrait<TraitInteractable>() != null)
{
    // Asset is interactable
}
```

### Trait Events

Many traits provide events you can subscribe to:

```csharp
var animatable = asset.GetTrait<TraitAnimatable>();
if (animatable != null)
{
    animatable.OnAnimationComplete += (name) => {
        Debug.Log($"Animation '{name}' completed!");
    };
}
```

## Creating Custom Traits

You can create custom traits to extend URT3D functionality for your specific needs:

1. Create a class that inherits from `Trait`
2. Implement required functionality
3. Add the trait to your assets

Example of a custom trait:

```csharp
using Urt3d.Traits;
using UnityEngine;

// Custom trait for health in a game
public class TraitHealth : Trait
{
    private float _maxHealth = 100;
    private float _currentHealth;
    
    // Event for health changes
    public event System.Action<float> OnHealthChanged;
    
    // Event for when health reaches zero
    public event System.Action OnDied;
    
    public TraitHealth()
    {
        Name = "Health";
        _currentHealth = _maxHealth;
    }
    
    public float MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }
    
    public float CurrentHealth => _currentHealth;
    
    public void TakeDamage(float amount)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(_currentHealth);
        
        if (_currentHealth <= 0)
        {
            OnDied?.Invoke();
        }
    }
    
    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(_currentHealth);
    }
}
```

## Traits in the Inspector

The URT3D Inspector provides a user-friendly interface for managing traits:

- View all traits on an asset
- Configure trait properties
- Add/remove traits (if supported)

## Next Steps

- Learn about the [Scripting System](04-ScriptingSystem.md) to add dynamic behavior
- See [Examples and Best Practices](06-ExamplesAndBestPractices.md) for common trait usage patterns
- Check the [API Reference](05-APIReference.md) for detailed trait class information
