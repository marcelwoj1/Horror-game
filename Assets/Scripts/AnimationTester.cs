using UnityEngine;

/// <summary>
/// Simple test script to verify the animation state machine system works correctly.
/// Attach this to your character GameObject to test the animation system.
/// </summary>
public class AnimationTester : MonoBehaviour
{
    [Header("Test Settings")]
    [Tooltip("Enable automatic testing")]
    public bool autoTest = true;
    
    [Tooltip("Time between state changes in auto test mode")]
    public float testInterval = 2f;
    
    [Tooltip("Jump velocity for testing")]
    public float testJumpVelocity = 10f;
    
    [Tooltip("Show debug info in console")]
    public bool showDebug = true;
    
    private Movement movementScript;
    private float testTimer = 0f;
    private int testPhase = 0;
    
    void Start()
    {
        movementScript = GetComponent<Movement>();
        
        if (movementScript == null)
        {
            Debug.LogError("AnimationTester: Movement script not found!");
            enabled = false;
            return;
        }
        
        Debug.Log("AnimationTester: Starting animation system test...");
        Debug.Log("Available animation states: Idle, Walking, Jumping, Falling");
        Debug.Log("Make sure to assign sprite arrays in the Movement component!");
    }
    
    void Update()
    {
        if (!autoTest)
        {
            // Manual test mode - use keyboard input
            ManualTest();
            return;
        }
        
        // Auto test mode
        AutoTest();
        
        // Display current state
        if (showDebug && movementScript != null)
        {
            Debug.Log($"Current State: {movementScript.GetCurrentAnimationState()} | " +
                     $"Velocity: {movementScript.GetVelocity():F2} | " +
                     $"Grounded: {movementScript.IsGrounded()}");
        }
    }
    
    void ManualTest()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Manual Test: Setting Idle state");
            movementScript.SetAnimationState("Idle");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Manual Test: Setting Walking state");
            movementScript.SetAnimationState("Walking");
            // Add some horizontal movement
            movementScript.SetHorizontalVelocity(3f);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Manual Test: Triggering Jump");
            movementScript.TriggerJump(testJumpVelocity);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Manual Test: Setting Falling state");
            movementScript.SetAnimationState("Falling");
            // Add some downward velocity
            movementScript.SetVerticalVelocity(-5f);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Manual Test: Resetting animation");
            movementScript.ResetAnimation();
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Current State: {movementScript.GetCurrentAnimationState()}");
            Debug.Log($"Is Grounded: {movementScript.IsGrounded()}");
            Debug.Log($"Is Moving: {movementScript.IsMoving()}");
            Debug.Log($"Is Jumping: {movementScript.IsJumping()}");
            Debug.Log($"Is Falling: {movementScript.IsFalling()}");
        }
    }
    
    void AutoTest()
    {
        testTimer += Time.deltaTime;
        
        if (testTimer >= testInterval)
        {
            testTimer = 0f;
            testPhase = (testPhase + 1) % 4;
            
            switch (testPhase)
            {
                case 0:
                    Debug.Log("Auto Test: Idle state");
                    movementScript.SetAnimationState("Idle");
                    movementScript.StopMovement();
                    break;
                    
                case 1:
                    Debug.Log("Auto Test: Walking state");
                    movementScript.SetAnimationState("Walking");
                    movementScript.SetHorizontalVelocity(4f);
                    break;
                    
                case 2:
                    Debug.Log("Auto Test: Jumping state");
                    movementScript.TriggerJump(testJumpVelocity);
                    break;
                    
                case 3:
                    Debug.Log("Auto Test: Falling state");
                    movementScript.SetAnimationState("Falling");
                    movementScript.SetVerticalVelocity(-6f);
                    break;
            }
        }
    }
    
    void OnGUI()
    {
        if (movementScript == null) return;
        
        // Display UI for testing
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("=== ANIMATION TESTER ===");
        GUILayout.Label($"Current State: {movementScript.GetCurrentAnimationState()}");
        GUILayout.Label($"Velocity: {movementScript.GetVelocity():F2}");
        GUILayout.Label($"Grounded: {movementScript.IsGrounded()}");
        GUILayout.Label($"Moving: {movementScript.IsMoving()}");
        
        GUILayout.Space(10);
        GUILayout.Label("Manual Controls:");
        GUILayout.Label("1: Idle | 2: Walk | 3: Jump | 4: Fall");
        GUILayout.Label("R: Reset Animation | Space: Debug Info");
        
        GUILayout.Space(5);
        autoTest = GUILayout.Toggle(autoTest, "Auto Test Mode");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
