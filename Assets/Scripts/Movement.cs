using UnityEngine;

/// <summary>
/// Highly customizable 2D character controller for side-scroller games.
/// Designed for use with Rigidbody2D with friction disabled, 0 gravity, and 0 damping.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [Tooltip("Maximum horizontal speed the character can reach")]
    [SerializeField] private float maxSpeed = 8f;
    
    [Tooltip("How quickly the character accelerates to max speed")]
    [SerializeField] private float acceleration = 25f;
    
    [Tooltip("How quickly the character decelerates when no input is given")]
    [SerializeField] private float deceleration = 20f;
    
    [Tooltip("How quickly the character can change direction while moving")]
    [SerializeField] private float turnSpeed = 30f;
    
    [Header("Advanced Settings")]
    [Tooltip("Minimum speed threshold - below this, velocity is set to zero")]
    [SerializeField] private float stopThreshold = 0.01f;
    
    [Tooltip("Input deadzone - input values below this are ignored")]
    [SerializeField] private float inputDeadzone = 0.1f;
    
    [Tooltip("Multiply input by this value for fine-tuned control")]
    [SerializeField] private float inputSensitivity = 1f;
    
    [Tooltip("If true, uses smooth acceleration curve instead of linear")]
    [SerializeField] private bool useSmoothAcceleration = false;
    
    [Tooltip("Power curve for smooth acceleration (higher = more gradual start)")]
    [SerializeField] private float accelerationCurve = 2f;
    
    [Header("Debug")]
    [Tooltip("Show debug information in the console")]
    [SerializeField] private bool showDebugInfo = false;
    
    // Components
    private Rigidbody2D rb;
    
    // Input
    private float horizontalInput;
    
    // Current velocity
    private Vector2 currentVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        
        if (rb.linearDamping != 0)
        {
            Debug.LogWarning($"Movement: Rigidbody2D linear drag is {rb.linearDamping}. Expected 0 for manual velocity control.");
        }
    }

    private void Update()
    {
        // Get horizontal input (A/D or Left/Right arrow keys)
        horizontalInput = Input.GetAxisRaw("Horizontal") * inputSensitivity;
        
        // Apply deadzone
        if (Mathf.Abs(horizontalInput) < inputDeadzone)
        {
            horizontalInput = 0f;
        }
    }

    private void FixedUpdate()
    {
        currentVelocity = rb.linearVelocity;
        
        // Handle horizontal movement
        HandleHorizontalMovement();
        
        // Apply velocity to rigidbody
        rb.linearVelocity = currentVelocity;
        
        // Debug information
        if (showDebugInfo)
        {
            Debug.Log($"Velocity: {currentVelocity.magnitude:F2} | H-Input: {horizontalInput:F2} | Speed: {currentVelocity.x:F2}");
        }
    }

    private void HandleHorizontalMovement()
    {
        if (horizontalInput != 0)
        {
            // Player is giving input - accelerate
            float targetSpeed = horizontalInput * maxSpeed;
            float speedDifference = targetSpeed - currentVelocity.x;
            
            // Determine acceleration rate
            float accelRate;
            
            // If changing direction (input and velocity have opposite signs)
            if (Mathf.Sign(horizontalInput) != Mathf.Sign(currentVelocity.x) && Mathf.Abs(currentVelocity.x) > stopThreshold)
            {
                accelRate = turnSpeed;
            }
            else
            {
                accelRate = acceleration;
            }
            
            // Apply smooth acceleration curve if enabled
            if (useSmoothAcceleration)
            {
                float speedRatio = Mathf.Abs(currentVelocity.x) / maxSpeed;
                float curveMultiplier = 1f - Mathf.Pow(speedRatio, accelerationCurve);
                accelRate *= curveMultiplier;
            }
            
            // Calculate velocity change
            float velocityChange = speedDifference * accelRate * Time.fixedDeltaTime;
            
            // Apply acceleration
            currentVelocity.x += velocityChange;
            
            // Clamp to max speed
            currentVelocity.x = Mathf.Clamp(currentVelocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            // No input - decelerate
            if (Mathf.Abs(currentVelocity.x) > stopThreshold)
            {
                float decelerationAmount = deceleration * Time.fixedDeltaTime;
                float targetVelocity = Mathf.MoveTowards(currentVelocity.x, 0f, decelerationAmount);
                currentVelocity.x = targetVelocity;
            }
            else
            {
                currentVelocity.x = 0f;
            }
        }
    }

    #region Public Methods
    
    /// <summary>
    /// Set the horizontal movement speed directly
    /// </summary>
    public void SetHorizontalVelocity(float velocity)
    {
        Vector2 vel = rb.linearVelocity;
        vel.x = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);
        rb.linearVelocity = vel;
    }
    
    /// <summary>
    /// Set the vertical velocity (useful for external systems like knockback)
    /// </summary>
    public void SetVerticalVelocity(float velocity)
    {
        Vector2 vel = rb.linearVelocity;
        vel.y = velocity;
        rb.linearVelocity = vel;
    }
    
    /// <summary>
    /// Add an impulse force to the character (useful for knockback, explosions, etc.)
    /// </summary>
    public void AddImpulse(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    
    /// <summary>
    /// Stop all movement immediately
    /// </summary>
    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
    }
    
    /// <summary>
    /// Get the current movement velocity
    /// </summary>
    public Vector2 GetVelocity()
    {
        return rb.linearVelocity;
    }
    
    /// <summary>
    /// Get the current horizontal velocity
    /// </summary>
    public float GetHorizontalVelocity()
    {
        return rb.linearVelocity.x;
    }
    
    /// <summary>
    /// Check if the character is currently moving horizontally
    /// </summary>
    public bool IsMoving()
    {
        return Mathf.Abs(rb.linearVelocity.x) > stopThreshold;
    }
    
    /// <summary>
    /// Get the current horizontal input value (-1 to 1)
    /// </summary>
    public float GetHorizontalInput()
    {
        return horizontalInput;
    }
    
    /// <summary>
    /// Get the direction the character is moving (-1 = left, 0 = stationary, 1 = right)
    /// </summary>
    public int GetMovementDirection()
    {
        if (Mathf.Abs(currentVelocity.x) < stopThreshold)
            return 0;
        return currentVelocity.x > 0 ? 1 : -1;
    }
    
    #endregion
    
    #region Gizmos
    
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || rb == null) return;
        
        // Draw velocity vector
        Gizmos.color = Color.green;
        Vector3 velocityEnd = transform.position + new Vector3(rb.linearVelocity.x, 0, 0);
        Gizmos.DrawLine(transform.position, velocityEnd);
        
        // Draw max speed indicators
        Gizmos.color = Color.yellow;
        Vector3 leftMaxSpeed = transform.position + Vector3.left * maxSpeed;
        Vector3 rightMaxSpeed = transform.position + Vector3.right * maxSpeed;
        Gizmos.DrawWireSphere(leftMaxSpeed, 0.2f);
        Gizmos.DrawWireSphere(rightMaxSpeed, 0.2f);
    }
    
    #endregion
}
