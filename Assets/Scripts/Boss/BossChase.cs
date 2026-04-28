using UnityEngine;

/// <summary>
/// Controls boss movement towards the player during active combat.
/// </summary>
/// <remarks>
/// This script is responsible for:
/// - Moving the boss towards the player using Rigidbody physics
/// - Playing movement animations
/// - Enabling chase behaviour when triggered
///
/// Chase behaviour is typically activated by other systems (e.g., boss state changes).
/// </remarks>
public class BossChase : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Reference to the player transform.</summary>
    private Transform player;

    /// <summary>Rigidbody used for physics-based movement.</summary>
    private Rigidbody2D rb;

    /// <summary>Handles animation playback.</summary>
    private SpriteAnimator animator;

    /// <summary>Reference to the boss manager for state checks.</summary>
    private Boss_manager boss_manager;

    [Header("Variables")]

    /// <summary>Movement speed of the boss.</summary>
    public float speed = 2f;

    /// <summary>Determines whether the boss is currently chasing the player.</summary>
    public bool isChasing = false;

    /// <summary>
    /// Initialises component references.
    /// </summary>
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();
        boss_manager = GetComponent<Boss_manager>();
    }

    /// <summary>
    /// Updates movement behaviour each frame.
    /// </summary>
    /// <remarks>
    /// The boss will only chase the player if:
    /// - Chasing has been enabled
    /// - The boss is not dead
    ///
    /// Movement is handled using Rigidbody velocity for consistent physics interaction.
    /// </remarks>
    void Update()
    {
        if (isChasing && !boss_manager.isDead)
        {
            // Calculate direction towards the player
            Vector2 direction = (player.position - transform.position).normalized;

            // Apply velocity to move towards the player
            rb.linearVelocity = direction * speed;

            animator.Play("Walk");
        }
    }

    /// <summary>
    /// Enables chase behaviour and sets the boss to an aggressive state.
    /// </summary>
    /// <remarks>
    /// Typically called by other systems (e.g., boss manager or triggers)
    /// when combat begins.
    /// </remarks>
    public void StartChasing()
    {
        isChasing = true;
        boss_manager.isAggressive = true;
    }
}