using UnityEngine;

/// <summary>
/// Highly customizable 2D character controller for side-scroller games.
/// Designed for use with Rigidbody2D with friction disabled, 0 gravity, and 0 damping.
/// Includes animation state machine with sprite cycling for idle, walk, jump, and fall states.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
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

    [Header("Jump Parameters")]
    [Tooltip("Force applied when jumping")]
    [SerializeField] private float jumpForce = 12f;
    
    [Header("Animation Settings")]
    [Tooltip("Sprites for idle animation (will cycle through these)")]
    public Sprite[] idleSprites;
    
    [Tooltip("Sprites for walking animation (will cycle through these)")]
    public Sprite[] walkSprites;
    
    [Tooltip("Sprites for jumping animation (will cycle through these)")]
    public Sprite[] jumpSprites;
    
    [Tooltip("Sprites for falling animation (will cycle through these)")]
    public Sprite[] fallSprites;
    
    [Tooltip("Animation frame rate (frames per second)")]
    [Range(1f, 30f)]
    public float animationFrameRate = 10f;
    
    [Tooltip("Threshold for considering the character as falling (positive value)")]
    public float fallThreshold = 0.1f;
    
    [Tooltip("Threshold for considering the character as grounded (for jump detection)")]
    public float groundThreshold = 0.05f;
    
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

    public Transform FeetLocation;
    
    // Animation states
    private enum AnimationState
    {
        Idle,
        Walking,
        Jumping,
        Falling
    }
    
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    // Input
    private float horizontalInput;
    
    // Current velocity
    private Vector2 currentVelocity;
    
    // Animation variables
    private AnimationState currentState = AnimationState.Idle;
    private AnimationState previousState = AnimationState.Idle;
    private int currentFrameIndex = 0;
    private float frameTimer = 0f;
    private bool wasGrounded = true;

    // Jump variables
    private bool isGrounded;
    private bool jumpRequested;

    // Ground detection
    [Header("Ground Detection")]
    [Tooltip("Length of the ground check ray")]
    [SerializeField] private float groundCheckDistance = 0.6f;
    
    [Tooltip("Width of the ground check area")]
    [SerializeField] private float groundCheckWidth = 0.8f;
    
    [Tooltip("Layers that count as ground")]
    [SerializeField] private LayerMask groundLayers = 1;
    
    [Tooltip("Visual offset for ground check gizmos")]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb.linearDamping != 0)
        {
            Debug.LogWarning($"Movement: Rigidbody2D linear drag is {rb.linearDamping}. Expected 0 for manual velocity control.");
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogError("Movement: SpriteRenderer component not found! Animation system will not work.");
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

        // Handle jump input
        HandleJumpInput();

        // Flip sprite based on movement direction
        HandleSpriteFlipping();
        
        // Update animation state
        UpdateAnimationState();
        
        // Update animation frame
        UpdateAnimationFrame();
    }

    private void HandleJumpInput()
    {
        // Simple jump: just check if jump was pressed and we're grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
            if (showDebugInfo)
            {
                Debug.Log("JUMP REQUESTED: Jump button pressed while grounded");
            }
        }
    }

    private void HandleSpriteFlipping()
    {
        if (horizontalInput != 0)
        {
            // Flip sprite to face movement direction
            spriteRenderer.flipX = horizontalInput > 0;
        }
    }

    private void FixedUpdate()
    {
        currentVelocity = rb.linearVelocity;
        
        // Check if grounded using raycast
        CheckGrounded();
        
        // Handle horizontal movement
        HandleHorizontalMovement();
        
        // Handle jumping - execute immediately when jump is requested
        if (jumpRequested)
        {
            // Apply jump force
            currentVelocity.y = jumpForce;
            jumpRequested = false;
            
            // Set animation state
            currentState = AnimationState.Jumping;
            currentFrameIndex = 0;
            frameTimer = 0f;
            
            if (showDebugInfo)
            {
                Debug.Log($"JUMP EXECUTED: Velocity Y set to {jumpForce:F2}, Animation state set to Jumping");
            }
        }
        
        // Apply velocity to rigidbody
        rb.linearVelocity = currentVelocity;
        
        // Update grounded state for animation
        if (!wasGrounded && isGrounded)
        {
            // Just landed
            wasGrounded = true;
        }
        else if (wasGrounded && !isGrounded && currentVelocity.y < 0)
        {
            // Started falling
            wasGrounded = false;
        }
        
        // Debug information
        if (showDebugInfo)
        {
            Debug.Log($"Velocity: {currentVelocity.magnitude:F2} | H-Input: {horizontalInput:F2} | Speed: {currentVelocity.x:F2} | State: {currentState} | Grounded: {isGrounded}");
        }
    }

    private void CheckGrounded()
    {
        // Perform ground check using raycast
        Vector2 leftRayOrigin = new Vector2(transform.position.x - groundCheckWidth / 2, transform.position.y) + groundCheckOffset;
        Vector2 rightRayOrigin = new Vector2(transform.position.x + groundCheckWidth / 2, transform.position.y) + groundCheckOffset;
        Vector2 centerRayOrigin = new Vector2(transform.position.x, transform.position.y) + groundCheckOffset;
        
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, groundCheckDistance, groundLayers);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, groundCheckDistance, groundLayers);
        RaycastHit2D centerHit = Physics2D.Raycast(centerRayOrigin, Vector2.down, groundCheckDistance, groundLayers);
        
        bool wasGroundedLastFrame = isGrounded;
        isGrounded = leftHit.collider != null || rightHit.collider != null || centerHit.collider != null;
        
        if (showDebugInfo)
        {
            Debug.Log($"GROUND CHECK: Left={leftHit.collider != null}, Right={rightHit.collider != null}, Center={centerHit.collider != null}, Final={isGrounded}, Was={wasGroundedLastFrame}");
            if (leftHit.collider != null) Debug.Log($"  Left hit: {leftHit.collider.name}");
            if (rightHit.collider != null) Debug.Log($"  Right hit: {rightHit.collider.name}");
            if (centerHit.collider != null) Debug.Log($"  Center hit: {centerHit.collider.name}");
        }
        
        // Log when landing (for debugging)
        if (!wasGroundedLastFrame && isGrounded && showDebugInfo)
        {
            Debug.Log("LANDED: Character has landed on ground");
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

    private void UpdateAnimationState()
    {
        previousState = currentState;
        
        // Determine current animation state based on movement
        bool isMoving = Mathf.Abs(currentVelocity.x) > stopThreshold;
        bool isFalling = currentVelocity.y < -fallThreshold;
        bool isRising = currentVelocity.y > fallThreshold;
        
        if (!isGrounded)
        {
            if (isFalling)
            {
                currentState = AnimationState.Falling;
            }
            else if (isRising)
            {
                currentState = AnimationState.Jumping;
            }
        }
        else
        {
            if (isMoving)
            {
                currentState = AnimationState.Walking;
            }
            else
            {
                currentState = AnimationState.Idle;
            }
        }
        
        // Reset frame index when state changes
        if (previousState != currentState)
        {
            currentFrameIndex = 0;
            frameTimer = 0f;
        }
    }

    private void UpdateAnimationFrame()
    {
        // Get the current sprite array based on state
        Sprite[] currentSprites = GetCurrentSpriteArray();
        
        if (currentSprites == null || currentSprites.Length == 0)
        {
            return; // No sprites to animate
        }
        
        // Update frame timer
        frameTimer += Time.deltaTime;
        float frameDuration = 1f / animationFrameRate;
        
        // Check if it's time to advance to the next frame
        if (frameTimer >= frameDuration)
        {
            frameTimer = 0f;
            currentFrameIndex = (currentFrameIndex + 1) % currentSprites.Length;
            
            // Update the sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = currentSprites[currentFrameIndex];
            }
        }
    }

    private Sprite[] GetCurrentSpriteArray()
    {
        switch (currentState)
        {
            case AnimationState.Idle:
                return idleSprites;
            case AnimationState.Walking:
                return walkSprites;
            case AnimationState.Jumping:
                return jumpSprites;
            case AnimationState.Falling:
                return fallSprites;
            default:
                return idleSprites;
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
    
    /// <summary>
    /// Get the current animation state
    /// </summary>
    public string GetCurrentAnimationState()
    {
        return currentState.ToString();
    }
    
    /// <summary>
    /// Force set the animation state (useful for cutscenes or special events)
    /// </summary>
    public void SetAnimationState(string state)
    {
        if (System.Enum.TryParse<AnimationState>(state, true, out AnimationState newState))
        {
            previousState = currentState;
            currentState = newState;
            currentFrameIndex = 0;
            frameTimer = 0f;
        }
    }
    
    /// <summary>
    /// Manually trigger a jump (sets vertical velocity and animation state)
    /// </summary>
    public void TriggerJump(float jumpVelocity)
    {
        SetVerticalVelocity(jumpVelocity);
        wasGrounded = false;
        currentState = AnimationState.Jumping;
        currentFrameIndex = 0;
        frameTimer = 0f;
    }
    
    /// <summary>
    /// Check if the character is currently grounded
    /// </summary>
    public bool IsGrounded()
    {
        return Mathf.Abs(currentVelocity.y) < groundThreshold;
    }
    
    /// <summary>
    /// Check if the character is currently falling
    /// </summary>
    public bool IsFalling()
    {
        return currentVelocity.y < -fallThreshold;
    }
    
    /// <summary>
    /// Check if the character is currently jumping/rising
    /// </summary>
    public bool IsJumping()
    {
        return currentVelocity.y > fallThreshold;
    }
    
    /// <summary>
    /// Reset animation to first frame of current state
    /// </summary>
    public void ResetAnimation()
    {
        currentFrameIndex = 0;
        frameTimer = 0f;
        UpdateAnimationFrame();
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
        
        // Draw ground check rays
        Gizmos.color = isGrounded ? Color.cyan : Color.red;
        Vector3 leftRayOrigin = transform.position + new Vector3(-groundCheckWidth / 2, 0, 0) + new Vector3(groundCheckOffset.x, groundCheckOffset.y, 0);
        Vector3 rightRayOrigin = transform.position + new Vector3(groundCheckWidth / 2, 0, 0) + new Vector3(groundCheckOffset.x, groundCheckOffset.y, 0);
        Vector3 centerRayOrigin = transform.position + new Vector3(groundCheckOffset.x, groundCheckOffset.y, 0);
        
        Gizmos.DrawLine(leftRayOrigin, leftRayOrigin + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(rightRayOrigin, rightRayOrigin + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(centerRayOrigin, centerRayOrigin + Vector3.down * groundCheckDistance);
    }
    
    #endregion
}
