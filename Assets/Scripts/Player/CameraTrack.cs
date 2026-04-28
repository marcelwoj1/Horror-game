using UnityEngine;

/// <summary>
/// Controls camera movement, following a target with smoothing,
/// bounds clamping, and optional screen shake effects.
/// </summary>
/// <remarks>
/// Features:
/// - Smooth camera following using interpolation
/// - Dynamic bounds system to restrict camera movement
/// - Runtime switching of target and bounds
/// - Screen shake effect for feedback (e.g., impacts)
///
/// The camera ensures the target remains visible while preventing
/// movement outside defined level boundaries.
/// </remarks>
public class CameraTrack : MonoBehaviour
{
    [Header("Target")]

    /// <summary>Current target the camera follows.</summary>
    public static Transform Target;

    /// <summary>Initial target assigned at startup.</summary>
    [SerializeField] private Transform initialTarget;

    [Header("Follow Settings")]

    /// <summary>Smoothing speed for camera movement.</summary>
    [SerializeField] private float smoothSpeed = 5f;

    /// <summary>Offset applied to the target position.</summary>
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Header("Bounds (Position = Center, Scale = Size)")]

    /// <summary>Transform defining camera movement bounds.</summary>
    public static Transform BoundsTransform;

    /// <summary>Initial bounds assigned at startup.</summary>
    [SerializeField] private Transform initialBoundsTransform;

    [Header("Shake Settings")]

    /// <summary>Speed at which shake effect fades.</summary>
    [SerializeField] private float shakeDecay = 5f;

    /// <summary>Camera component reference.</summary>
    private Camera cam;

    /// <summary>Previously used bounds transform.</summary>
    private Transform previousBoundsTransform;

    /// <summary>Half height of the camera view.</summary>
    private float camHalfHeight;

    /// <summary>Half width of the camera view.</summary>
    private float camHalfWidth;

    /// <summary>Current interpolated camera position.</summary>
    private Vector3 currentCameraPos;

    /// <summary>Current shake intensity.</summary>
    private float currentShakeMagnitude = 0f;

    /// <summary>
    /// Initialises camera references and assigns default target and bounds.
    /// </summary>
    private void Awake()
    {
        cam = GetComponent<Camera>();
        currentCameraPos = transform.position;

        if (Target == null && initialTarget != null)
            Target = initialTarget;

        if (BoundsTransform == null && initialBoundsTransform != null)
            BoundsTransform = initialBoundsTransform;

        previousBoundsTransform = BoundsTransform;
    }

    /// <summary>
    /// Updates camera position after all movement logic is processed.
    /// </summary>
    /// <remarks>
    /// Uses LateUpdate to ensure the camera follows the final position of the target.
    /// Applies smoothing, bounds clamping, and optional screen shake.
    /// </remarks>
    private void LateUpdate()
    {
        if (Target == null || BoundsTransform == null) return;

        UpdateCameraSize();

        // Detect bounds change
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
            currentCameraPos = Vector3.Lerp(
                currentCameraPos,
                targetPosition,
                smoothSpeed * Time.deltaTime
            );
        }

        // Apply screen shake
        Vector3 finalPos = currentCameraPos;

        if (currentShakeMagnitude > 0f)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * currentShakeMagnitude;
            finalPos += new Vector3(shakeOffset.x, shakeOffset.y, 0f);

            currentShakeMagnitude = Mathf.Lerp(
                currentShakeMagnitude,
                0f,
                shakeDecay * Time.deltaTime
            );

            if (currentShakeMagnitude < 0.01f)
                currentShakeMagnitude = 0f;
        }

        transform.position = finalPos;
    }

    /// <summary>
    /// Applies a screen shake effect.
    /// </summary>
    /// <param name="magnitude">Strength of the shake effect.</param>
    public void Shake(float magnitude)
    {
        currentShakeMagnitude = magnitude;
    }

    /// <summary>
    /// Updates camera bounds at runtime.
    /// </summary>
    /// <param name="newBounds">New bounds transform.</param>
    /// <remarks>
    /// The camera will instantly snap to the new bounds.
    /// </remarks>
    public static void SetBounds(Transform newBounds)
    {
        BoundsTransform = newBounds;
    }

    /// <summary>
    /// Updates the camera target at runtime.
    /// </summary>
    /// <param name="newTarget">New target transform.</param>
    public static void SetTarget(Transform newTarget)
    {
        Target = newTarget;
    }

    /// <summary>
    /// Calculates the visible camera size.
    /// </summary>
    private void UpdateCameraSize()
    {
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    /// <summary>
    /// Calculates the camera position clamped within bounds.
    /// </summary>
    /// <returns>Clamped world position for the camera.</returns>
    /// <remarks>
    /// Ensures the camera does not move outside level boundaries.
    /// Also handles cases where bounds are smaller than the camera view.
    /// </remarks>
    private Vector3 GetClampedPosition()
    {
        Vector3 desiredPosition = new Vector3(
            Target.position.x + offset.x,
            Target.position.y + offset.y,
            currentCameraPos.z
        );

        Vector3 boundsCenter = BoundsTransform.position;
        Vector3 boundsSize = BoundsTransform.lossyScale;

        float halfWidth = boundsSize.x / 2f;
        float halfHeight = boundsSize.y / 2f;

        float minX = boundsCenter.x - halfWidth + camHalfWidth;
        float maxX = boundsCenter.x + halfWidth - camHalfWidth;
        float minY = boundsCenter.y - halfHeight + camHalfHeight;
        float maxY = boundsCenter.y + halfHeight - camHalfHeight;

        float clampedX = (minX > maxX)
            ? boundsCenter.x
            : Mathf.Clamp(desiredPosition.x, minX, maxX);

        float clampedY = (minY > maxY)
            ? boundsCenter.y
            : Mathf.Clamp(desiredPosition.y, minY, maxY);

        return new Vector3(clampedX, clampedY, desiredPosition.z);
    }
}