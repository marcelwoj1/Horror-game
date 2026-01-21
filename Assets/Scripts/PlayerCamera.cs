using UnityEngine;

/// <summary>
/// Smooth camera controller for 2D side-scroller games.
/// Follows the player with configurable smoothing, offset, and look-ahead features.
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The player transform to follow")]
    [SerializeField] private Transform target;
    
    [Tooltip("Offset from the target position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
    
    [Header("Smoothing Settings")]
    [Tooltip("Type of smoothing to use")]
    [SerializeField] private SmoothingType smoothingType = SmoothingType.SmoothDamp;
    
    [Tooltip("How quickly the camera follows the target (lower = smoother but slower)")]
    [SerializeField] private float smoothSpeed = 5f;
    
    [Tooltip("SmoothDamp specific - maximum speed the camera can move")]
    [SerializeField] private float maxSmoothSpeed = 10f;
    
    [Header("Look-Ahead Settings")]
    [Tooltip("Enable look-ahead in the direction of player movement")]
    [SerializeField] private bool useLookAhead = true;
    
    [Tooltip("Maximum distance to look ahead based on player velocity")]
    [SerializeField] private float lookAheadDistance = 3f;
    
    [Tooltip("How quickly the look-ahead responds to velocity changes")]
    [SerializeField] private float lookAheadSmoothness = 5f;
    
    [Tooltip("Minimum player speed to trigger look-ahead")]
    [SerializeField] private float lookAheadThreshold = 0.5f;
    
    [Header("Camera Bounds (Optional)")]
    [Tooltip("Enable camera bounds to limit camera movement")]
    [SerializeField] private bool useBounds = false;
    
    [Tooltip("Minimum camera position (X, Y)")]
    [SerializeField] private Vector2 minBounds = new Vector2(-50f, -50f);
    
    [Tooltip("Maximum camera position (X, Y)")]
    [SerializeField] private Vector2 maxBounds = new Vector2(50f, 50f);
    
    [Header("Advanced Settings")]
    [Tooltip("Only follow on X axis (horizontal)")]
    [SerializeField] private bool followXOnly = false;
    
    [Tooltip("Only follow on Y axis (vertical)")]
    [SerializeField] private bool followYOnly = false;
    
    [Tooltip("Dead zone - player can move this much without camera moving")]
    [SerializeField] private float deadZoneX = 0f;
    
    [Tooltip("Vertical dead zone")]
    [SerializeField] private float deadZoneY = 0f;
    
    [Header("Debug")]
    [Tooltip("Show debug information in Scene view")]
    [SerializeField] private bool showDebugGizmos = false;
    
    // Private variables
    private Vector3 currentVelocity;
    private Vector3 currentLookAhead;
    private Movement playerMovement;
    private Camera cam;
    
    // Smoothing types
    public enum SmoothingType
    {
        Lerp,
        SmoothDamp
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
        
        // Auto-find target if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("PlayerCamera: Auto-assigned player as target");
            }
            else
            {
                Debug.LogWarning("PlayerCamera: No target assigned and couldn't find GameObject with 'Player' tag!");
            }
        }
        
        // Try to get the Movement component for velocity-based look-ahead
        if (target != null)
        {
            playerMovement = target.GetComponent<Movement>();
        }
    }

    private void Start()
    {
        // Initialize camera position to target immediately
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = desiredPosition;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate base desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Apply look-ahead
        if (useLookAhead)
        {
            desiredPosition += CalculateLookAhead();
        }
        
        // Apply dead zone
        desiredPosition = ApplyDeadZone(desiredPosition);
        
        // Apply smoothing
        Vector3 smoothedPosition = ApplySmoothing(desiredPosition);
        
        // Apply axis restrictions
        if (followXOnly)
        {
            smoothedPosition.y = transform.position.y;
        }
        if (followYOnly)
        {
            smoothedPosition.x = transform.position.x;
        }
        
        // Apply bounds
        if (useBounds)
        {
            smoothedPosition = ApplyBounds(smoothedPosition);
        }
        
        // Ensure Z position stays correct for 2D
        smoothedPosition.z = offset.z;
        
        // Update camera position
        transform.position = smoothedPosition;
    }

    private Vector3 ApplySmoothing(Vector3 targetPosition)
    {
        switch (smoothingType)
        {
            case SmoothingType.Lerp:
                return Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            
            case SmoothingType.SmoothDamp:
                return Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 
                    1f / smoothSpeed, maxSmoothSpeed, Time.deltaTime);
            
            default:
                return targetPosition;
        }
    }

    private Vector3 CalculateLookAhead()
    {
        Vector3 targetLookAhead = Vector3.zero;
        
        if (playerMovement != null)
        {
            // Get player velocity
            Vector2 velocity = playerMovement.GetVelocity();
            
            // Only apply look-ahead if moving fast enough
            if (velocity.magnitude > lookAheadThreshold)
            {
                // Calculate look-ahead based on normalized velocity direction
                Vector3 velocityDirection = velocity.normalized;
                targetLookAhead = new Vector3(
                    velocityDirection.x * lookAheadDistance,
                    velocityDirection.y * lookAheadDistance,
                    0f
                );
            }
        }
        
        // Smoothly interpolate current look-ahead to target
        currentLookAhead = Vector3.Lerp(currentLookAhead, targetLookAhead, lookAheadSmoothness * Time.deltaTime);
        
        return currentLookAhead;
    }

    private Vector3 ApplyDeadZone(Vector3 desiredPosition)
    {
        Vector3 result = desiredPosition;
        Vector3 currentPos = transform.position;
        
        // Apply horizontal dead zone
        if (deadZoneX > 0)
        {
            float deltaX = desiredPosition.x - currentPos.x;
            if (Mathf.Abs(deltaX) < deadZoneX)
            {
                result.x = currentPos.x;
            }
        }
        
        // Apply vertical dead zone
        if (deadZoneY > 0)
        {
            float deltaY = desiredPosition.y - currentPos.y;
            if (Mathf.Abs(deltaY) < deadZoneY)
            {
                result.y = currentPos.y;
            }
        }
        
        return result;
    }

    private Vector3 ApplyBounds(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        return position;
    }

    #region Public Methods
    
    /// <summary>
    /// Set a new target for the camera to follow
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            playerMovement = target.GetComponent<Movement>();
        }
    }
    
    /// <summary>
    /// Instantly snap camera to target position (no smoothing)
    /// </summary>
    public void SnapToTarget()
    {
        if (target != null)
        {
            Vector3 snapPosition = target.position + offset;
            snapPosition.z = offset.z;
            transform.position = snapPosition;
            currentVelocity = Vector3.zero;
            currentLookAhead = Vector3.zero;
        }
    }
    
    /// <summary>
    /// Set camera offset from target
    /// </summary>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    /// <summary>
    /// Set camera bounds
    /// </summary>
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }
    
    /// <summary>
    /// Enable or disable camera bounds
    /// </summary>
    public void SetUseBounds(bool use)
    {
        useBounds = use;
    }
    
    /// <summary>
    /// Shake the camera (simple implementation)
    /// </summary>
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }
    
    private System.Collections.IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalOffset = offset;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            offset = originalOffset + new Vector3(x, y, 0f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        offset = originalOffset;
    }
    
    #endregion
    
    #region Gizmos
    
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        // Draw camera bounds
        if (useBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(
                (minBounds.x + maxBounds.x) / 2f,
                (minBounds.y + maxBounds.y) / 2f,
                transform.position.z
            );
            Vector3 size = new Vector3(
                maxBounds.x - minBounds.x,
                maxBounds.y - minBounds.y,
                0.1f
            );
            Gizmos.DrawWireCube(center, size);
        }
        
        // Draw target connection
        if (target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(target.position, 0.5f);
        }
        
        // Draw dead zones
        if (deadZoneX > 0 || deadZoneY > 0)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Vector3 deadZoneSize = new Vector3(deadZoneX * 2f, deadZoneY * 2f, 0.1f);
            Gizmos.DrawWireCube(transform.position, deadZoneSize);
        }
        
        // Draw look-ahead
        if (useLookAhead && Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Vector3 lookAheadPos = transform.position + currentLookAhead;
            Gizmos.DrawLine(transform.position, lookAheadPos);
            Gizmos.DrawWireSphere(lookAheadPos, 0.3f);
        }
    }
    
    #endregion
}
