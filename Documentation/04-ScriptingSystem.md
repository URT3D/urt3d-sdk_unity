# URT3D Scripting System

The URT3D SDK includes a powerful scripting system based on MiniScript, allowing you to add dynamic behaviors to your assets without requiring C# coding.

## Overview

The scripting system enables:
- Event-driven behaviors
- Animation control
- Trait manipulation
- Interaction handling
- Communication between assets

Scripts are attached to URT3D assets and can be triggered by various events.

## Script Basics

### Script Structure

URT3D scripts are written in MiniScript, a simple yet powerful scripting language with a syntax similar to Basic. A typical script looks like this:

```
// Simple rotation script
position = GetTrait("Position3d")
rotation = GetTrait("Rotation3d")

function Update()
    // Get current rotation
    currentRotation = rotation.GetValue()
    
    // Rotate 1 degree per frame around Y axis
    currentRotation.y = currentRotation.y + 1
    
    // Apply the new rotation
    rotation.SetValue(currentRotation)
end function
```

### Script Triggers

Scripts can be triggered in three ways:

- **OnLoad**: Executed when the asset is first loaded
- **OnUpdate**: Executed every frame
- **OnCustomEvent**: Executed when a specific custom event is triggered

### Execution Modes

You can configure when scripts should execute:

- **EditorOnly**: Scripts run only in the Unity Editor
- **RuntimeOnly**: Scripts run only during Play mode
- **Both**: Scripts run in both Editor and Play mode

## Script Editor

The URT3D Inspector includes a script editor for creating and editing scripts:

1. Select a URT3D asset in the scene
2. Navigate to the Scripts tab in the Inspector
3. Add a new script or edit existing scripts
4. Configure trigger type and execution mode
5. Write your script in the editor

## MiniScript API

The URT3D SDK exposes several functions and objects to MiniScript:

### Trait Access

```
// Get a trait
position = GetTrait("Position3d")

// Check if a trait exists
if HasTrait("Animatable") then
    anim = GetTrait("Animatable")
end if
```

### Asset Properties

```
// Get asset information
name = Asset.Name
guid = Asset.Guid
```

### Math Functions

```
// Vector operations
v1 = Vector3(1, 2, 3)
v2 = Vector3(4, 5, 6)
v3 = v1 + v2

// Math utility functions
angle = Math.Sin(time * Math.PI)
```

### Time and Timing

```
// Get time information
currentTime = Time.Now
deltaTime = Time.Delta

// Use Delay to wait
Delay(2.5)  // Wait for 2.5 seconds
DoSomething()
```

### Events and Messaging

```
// Trigger a custom event on this asset
TriggerEvent("jump")

// Trigger an event on another asset
TriggerEventOn("door_open", "door_asset_guid")
```

## Examples

### Rotating Object

```
rotation = GetTrait("Rotation3d")
speed = 30  // degrees per second

function Update()
    // Rotate around Y axis
    current = rotation.GetValue()
    current.y = current.y + speed * Time.Delta
    rotation.SetValue(current)
end function
```

### Animation Control

```
anim = GetTrait("Animatable")

function OnStart()
    // Play the "Idle" animation on load
    anim.PlayAnimation("Idle", true)  // true = loop
end function

function OnCustomEvent(eventName)
    // Play different animations based on custom events
    if eventName == "walk" then
        anim.PlayAnimation("Walk", true)
    else if eventName == "jump" then
        anim.PlayAnimation("Jump", false)
    else if eventName == "idle" then
        anim.PlayAnimation("Idle", true)
    end if
end function
```

### Interactive Object

```
interactable = GetTrait("Interactable")
position = GetTrait("Position3d")
originalY = 0
hovering = false

function OnStart()
    // Store original Y position
    originalY = position.GetValue().y
    
    // Set up event handlers
    interactable.OnHoverEnter = OnHoverEnter
    interactable.OnHoverExit = OnHoverExit
    interactable.OnClick = OnClick
end function

function OnHoverEnter()
    hovering = true
end function

function OnHoverExit()
    hovering = false
end function

function OnClick()
    // Jump when clicked
    TriggerEvent("jump")
end function

function Update()
    if hovering then
        // Float up when hovering
        pos = position.GetValue()
        pos.y = originalY + 0.2 * Math.Sin(Time.Now * 5)
        position.SetValue(pos)
    else
        // Return to original position
        pos = position.GetValue()
        pos.y = originalY
        position.SetValue(pos)
    end if
end function
```

## Triggering Scripts from C#

You can trigger custom script events from C#:

```csharp
// Get the Wrapper component
var wrapper = GetComponent<Urt3d.Wrapper>();

// Trigger a custom event
wrapper.TriggerCustomEvent("jump");
```

## Script Execution Flow

1. Asset loads and initializes
2. OnLoad scripts execute
3. OnUpdate scripts execute every frame
4. OnCustomEvent scripts execute when triggered by name

## Best Practices

- Keep scripts focused on a single responsibility
- Use custom events for communication between assets
- Avoid heavy computation in OnUpdate scripts
- Use traits to manage asset properties
- Check if traits exist before using them

## Next Steps

- Review [Examples and Best Practices](06-ExamplesAndBestPractices.md)
- See the [API Reference](05-APIReference.md) for detailed MiniScript API information
