# 2D Character Animation State Machine System

## Overview
This enhanced Movement script includes a complete animation state machine system that automatically cycles through sprite animations based on the character's current movement state. The system supports four animation states: Idle, Walking, Jumping, and Falling.

## Features
- **Automatic State Detection**: Automatically detects character state based on velocity and movement
- **Sprite Cycling**: Cycles through multiple sprites per animation state at configurable frame rates
- **Smooth Transitions**: Resets animation frames when state changes for smooth transitions
- **Inspector-Friendly**: All animation variables are public and configurable in Unity Inspector
- **Extensible**: Easy to add new states or modify existing ones
- **Debug Support**: Built-in debugging and testing tools

## Setup Instructions

### 1. Component Requirements
The script automatically requires both Rigidbody2D and SpriteRenderer components. Make sure your character GameObject has:
- Rigidbody2D (with 0 gravity and 0 linear damping for best results)
- SpriteRenderer
- The Movement script attached

### 2. Sprite Assignment
In the Unity Inspector, assign your sprite arrays:
- **Idle Sprites**: Array of sprites for idle animation
- **Walk Sprites**: Array of sprites for walking animation  
- **Jump Sprites**: Array of sprites for jumping animation
- **Fall Sprites**: Array of sprites for falling animation

*Note: Each array can contain 1 or more sprites. The system will cycle through them automatically.*

### 3. Animation Settings
Configure these settings in the Inspector:
- **Animation Frame Rate**: Speed of animation cycling (1-30 FPS)
- **Fall Threshold**: Velocity threshold to detect falling state
- **Ground Threshold**: Velocity threshold to detect grounded state

## Usage Examples

### Basic Usage
```csharp
// The system works automatically - just assign sprites in Inspector
// Character will automatically switch between animations based on movement
```

### Manual State Control
```csharp
Movement movement = GetComponent<Movement>();

// Force specific animation state
movement.SetAnimationState("Jumping");

// Trigger a jump with specific velocity
movement.TriggerJump(10f);

// Reset animation to first frame
movement.ResetAnimation();
```

### State Queries
```csharp
// Get current animation state
string currentState = movement.GetCurrentAnimationState();

// Check character state
bool isGrounded = movement.IsGrounded();
bool isMoving = movement.IsMoving();
bool isJumping = movement.IsJumping();
bool isFalling = movement.IsFalling();
```

### Movement Control
```csharp
// Set horizontal velocity
movement.SetHorizontalVelocity(5f);

// Add vertical velocity (jump)
movement.SetVerticalVelocity(8f);

// Add impulse force
movement.AddImpulse(new Vector2(10f, 5f));

// Stop all movement
movement.StopMovement();
```

## Animation States

### Idle State
- **Trigger**: Character is grounded and not moving horizontally
- **Sprites**: Uses `idleSprites` array
- **Behavior**: Cycles through idle animation frames

### Walking State  
- **Trigger**: Character is grounded and moving horizontally
- **Sprites**: Uses `walkSprites` array
- **Behavior**: Cycles through walking animation frames

### Jumping State
- **Trigger**: Character has upward vertical velocity
- **Sprites**: Uses `jumpSprites` array
- **Behavior**: Cycles through jumping animation frames

### Falling State
- **Trigger**: Character has downward vertical velocity
- **Sprites**: Uses `fallSprites` array
- **Behavior**: Cycles through falling animation frames

## Testing the System

### Using the AnimationTester Script
1. Attach the `AnimationTester.cs` script to your character
2. Press Play in Unity
3. Use the on-screen UI or keyboard controls:
   - **1**: Force Idle state
   - **2**: Force Walking state with movement
   - **3**: Trigger jump
   - **4**: Force Falling state
   - **R**: Reset animation
   - **Space**: Display debug information

### Manual Testing
- Assign sprites to all animation arrays in the Inspector
- Set different frame rates for each state
- Test movement and observe automatic state transitions
- Check console for debug information

## Advanced Tips

### Custom State Logic
Modify the `UpdateAnimationState()` method to add custom state detection logic:
```csharp
// Add custom conditions
if (customCondition)
{
    currentState = AnimationState.CustomState;
}
```

### Frame Rate Per State
Currently all states use the same frame rate. To add per-state frame rates:
1. Add separate frame rate variables for each state
2. Modify `UpdateAnimationFrame()` to use the appropriate rate

### Sprite Direction
For characters that face different directions, you can flip the sprite renderer:
```csharp
// Face direction based on movement
if (movement.GetMovementDirection() != 0)
{
    spriteRenderer.flipX = movement.GetMovementDirection() < 0;
}
```

### Animation Events
To add events at specific frames, modify `UpdateAnimationFrame()`:
```csharp
// Trigger event at specific frame
if (currentFrameIndex == targetFrame)
{
    // Your event logic here
}
```

## Troubleshooting

### Animation Not Playing
- Ensure sprites are assigned in the Inspector
- Check that SpriteRenderer component is present
- Verify animation frame rate is not set too low
- Check console for error messages

### Wrong State Detection
- Adjust `fallThreshold` and `groundThreshold` values
- Check Rigidbody2D gravity and damping settings
- Ensure movement script is updating velocity correctly

### Animation Too Fast/Slow
- Adjust `animationFrameRate` value (higher = faster)
- Consider different frame rates for different states
- Check Time.timeScale if using slow motion effects

## Performance Notes
- The system is optimized for performance with minimal garbage allocation
- State detection runs every frame but only updates when state changes
- Sprite cycling uses efficient modulo operation
- Consider reducing animation frame rates for better performance on low-end devices

## Extension Ideas
- Add attack animation states
- Implement landing animations
- Add swimming/climbing states
- Create animation blending between states
- Add particle effects for state transitions
- Implement sprite color tinting for different states
