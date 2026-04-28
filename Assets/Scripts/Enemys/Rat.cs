using UnityEngine;

/// <summary>
/// Controls a simple enemy (rat) that activates after a short delay
/// and moves towards the player using physics-based movement.
/// </summary>
/// <remarks>
/// Behaviour overview:
/// - Remains idle for a short period after spawning
/// - Does not move if the player is hiding
/// - Uses Rigidbody velocity for consistent physics interaction
/// - Continuously moves horizontally towards the player once active
/// </remarks>
public class Rat : MonoBehaviour
{
    [Header("Settings")]

    /// <summary>Horizontal movement speed of the rat.</summary>
    public float speed = 6f;

    /// <summary>Delay before the rat becomes active after spawning.</summary>
    public float wakeDelay = 1f;

    [Header("Components")]

    /// <summary>Reference to the player transform.</summary>
    private Transform player;

    /// <summary>Provides access to player state (e.g., hiding).</summary>
    private PlayerManager playerManager;

    /// <summary>Rigidbody used for physics-based movement.</summary>
    private Rigidbody2D rb;

    /// <summary>Handles animation playback.</summary>
    private SpriteAnimator animator;

    /// <summary>Time at which the rat becomes active.</summary>
    private float wakeTime;

    /// <summary>
    /// Initialises references and sets the activation delay.
    /// </summary>
    void Start()
    {
        // Get player reference
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerManager = player.GetComponent<PlayerManager>();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();

        // Calculate when the rat should begin moving
        wakeTime = Time.time + wakeDelay;
    }

    /// <summary>
    /// Handles movement and animation using physics updates.
    /// </summary>
    /// <remarks>
    /// Movement is performed in FixedUpdate to ensure consistent physics behaviour.
    /// The rat remains idle if:
    /// - The wake delay has not elapsed
    /// - The player is currently hiding
    /// </remarks>
    void FixedUpdate()
    {
        // Idle conditions
        if (Time.time < wakeTime || playerManager.IsHiding)
        {
            // Stop horizontal movement while preserving vertical velocity
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            animator.Play("Idle");
            return;
        }

        // Determine horizontal direction towards the player (-1 or 1)
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // Apply movement using Rigidbody velocity
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // Update animation and sprite orientation
        animator.Play("Walk");
        transform.localScale = new Vector3(direction, 1, 1);
    }
}