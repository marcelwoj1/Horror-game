using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    [Header("Target")]
    public static Transform Target;
    [SerializeField] private Transform initialTarget;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Header("Bounds (Position = Center, Scale = Size)")]
    public static Transform BoundsTransform;
    [SerializeField] private Transform initialBoundsTransform;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDecay = 5f;

    private Camera cam;
    private Transform previousBoundsTransform;
    private float camHalfHeight;
    private float camHalfWidth;
    private Vector3 currentCameraPos;
    private float currentShakeMagnitude = 0f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        currentCameraPos = transform.position;
        
        // Set initial static variables
        if (Target == null && initialTarget != null)
            Target = initialTarget;
        
        if (BoundsTransform == null && initialBoundsTransform != null)
            BoundsTransform = initialBoundsTransform;
        
        previousBoundsTransform = BoundsTransform;
    }

    private void LateUpdate()
    {
        if (Target == null || BoundsTransform == null) return;

        UpdateCameraSize();
        
        // Check if bounds transform changed at runtime - snap instantly
        bool boundsChanged = previousBoundsTransform != BoundsTransform;
        previousBoundsTransform = BoundsTransform;

        Vector3 targetPosition = GetClampedPosition();

        if (boundsChanged)
        {
            // Snap instantly to new bounds
            currentCameraPos = targetPosition;
        }
        else
        {
            // Smooth follow
            currentCameraPos = Vector3.Lerp(currentCameraPos, targetPosition, smoothSpeed * Time.deltaTime);
        }

        Vector3 finalPos = currentCameraPos;
        if (currentShakeMagnitude > 0f)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * currentShakeMagnitude;
            finalPos += new Vector3(shakeOffset.x, shakeOffset.y, 0f);
            currentShakeMagnitude = Mathf.Lerp(currentShakeMagnitude, 0f, shakeDecay * Time.deltaTime);
            if (currentShakeMagnitude < 0.01f) currentShakeMagnitude = 0f;
        }

        transform.position = finalPos;
    }

    // Applies a screen shake effect with the given magnitude
    public void Shake(float magnitude)
    {
        currentShakeMagnitude = magnitude;
    }

    // Updates the camera bounds at runtime. Camera will snap instantly to new bounds
    public static void SetBounds(Transform newBounds)
    {
        BoundsTransform = newBounds;
    }

    // Updates the camera target at runtime
    public static void SetTarget(Transform newTarget)
    {
        Target = newTarget;
    }

    private void UpdateCameraSize()
    {
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    private Vector3 GetClampedPosition()
    {
        Vector3 desiredPosition = new Vector3(
            Target.position.x + offset.x,
            Target.position.y + offset.y,
            currentCameraPos.z
        );

        // Get bounds from transform
        Vector3 boundsCenter = BoundsTransform.position;
        Vector3 boundsSize = BoundsTransform.lossyScale;
        
        float halfWidth = boundsSize.x / 2f;
        float halfHeight = boundsSize.y / 2f;

        // Calculate effective bounds
        float minX = boundsCenter.x - halfWidth + camHalfWidth;
        float maxX = boundsCenter.x + halfWidth - camHalfWidth;
        float minY = boundsCenter.y - halfHeight + camHalfHeight;
        float maxY = boundsCenter.y + halfHeight - camHalfHeight;

        // Handle cases where bounds are smaller than camera view
        float clampedX = (minX > maxX) ? boundsCenter.x : Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedY = (minY > maxY) ? boundsCenter.y : Mathf.Clamp(desiredPosition.y, minY, maxY);

        return new Vector3(clampedX, clampedY, desiredPosition.z);
    }

    private void OnDrawGizmos()
    {
        // Don't draw gizmos if not enabled
        if (!showGizmos) return;

        // Get bounds from transform
        Transform boundsToShow = BoundsTransform != null ? BoundsTransform : initialBoundsTransform;
        if (boundsToShow == null) return;

        Vector3 center = boundsToShow.position;
        Vector3 size = boundsToShow.lossyScale;

        // Draw the full camera bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
        
        // Draw effective bounds 
        if (cam == null) cam = GetComponent<Camera>();
        if (cam != null)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;
            
            Vector3 effectiveSize = new Vector3(
                Mathf.Max(0, size.x - halfWidth * 2),
                Mathf.Max(0, size.y - halfHeight * 2),
                0
            );
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center, effectiveSize);
            
            // Draw current camera view
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(halfWidth * 2, halfHeight * 2, 0));
        }
    }
}
