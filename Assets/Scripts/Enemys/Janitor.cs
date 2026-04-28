using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the behaviour of the Janitor enemy, including patrolling,
/// chasing the player, and idle waiting behaviour.
/// </summary>
/// <remarks>
/// Behaviour system:
/// - Patrol: Moves between two points within a defined range
/// - Wait: Stops and performs an idle action (mopping)
/// - Chase: Pursues the player when aggressive and visible
/// 
/// The Janitor transitions between states based on:
/// - Player visibility (hiding state)
/// - Aggression state from the Enemy component
/// </remarks>
public class Janitor : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Reference to the player transform.</summary>
    private Transform player;

    /// <summary>Provides access to player state (e.g., hiding).</summary>
    private PlayerManager _playerManager;

    /// <summary>Handles animation playback.</summary>
    private SpriteAnimator _animator;

    /// <summary>Reference to the base enemy component.</summary>
    private Enemy enemy;

    [Header("Movement")]

    /// <summary>Speed while patrolling between points.</summary>
    public float patrolSpeed = 2f;

    /// <summary>Speed while chasing the player.</summary>
    public float chaseSpeed = 4f;

    /// <summary>Total horizontal distance covered during patrol.</summary>
    public float patrolRange = 4f;

    [Header("Chase Toggle")]

    /// <summary>Indicates whether the Janitor is currently chasing the player.</summary>
    public bool chasePlayer = false;

    [Header("Wait")]

    /// <summary>Duration spent waiting at patrol points.</summary>
    public float waitTime = 5f;

    // Patrol points

    /// <summary>Left boundary of patrol movement.</summary>
    private float leftPoint;

    /// <summary>Right boundary of patrol movement.</summary>
    private float rightPoint;

    /// <summary>Current horizontal target position.</summary>
    private float targetX;

    /// <summary>Indicates whether the Janitor is currently waiting.</summary>
    private bool isWaiting;

    /// <summary>
    /// Initialises patrol boundaries and component references.
    /// </summary>
    void Start()
    {
        float startX = transform.position.x;

        // Set patrol boundaries relative to starting position
        leftPoint = startX - patrolRange;
        rightPoint = startX + patrolRange;

        // Set initial patrol target
        targetX = rightPoint;

        _animator = GetComponent<SpriteAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerManager = player.GetComponent<PlayerManager>();
        enemy = GetComponent<Enemy>();
    }

    /// <summary>
    /// Updates behaviour each frame based on player visibility and aggression.
    /// </summary>
    /// <remarks>
    /// Behaviour priority:
    /// - If player is hiding → disable chase
    /// - If aggressive and player visible → chase
    /// - Otherwise → patrol
    /// </remarks>
    void Update()
    {
        // Player hiding disables chase behaviour
        if (_playerManager.IsHiding)
        {
            chasePlayer = false;
        }

        // Become aggressive and chase if conditions are met
        if (enemy.isAggressive && !_playerManager.IsHiding)
        {
            chasePlayer = true;

            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Janitor"),
                false
            );
        }

        // Waiting state (idle behaviour)
        if (isWaiting)
        {
            _animator.Play("Mopping");

            // Interrupt waiting if player becomes a target
            if (chasePlayer)
            {
                isWaiting = false;
            }

            return;
        }

        // Execute behaviour based on current state
        if (chasePlayer)
            Chase();
        else
            Patrol();
    }

    /// <summary>
    /// Moves the Janitor between patrol points.
    /// </summary>
    /// <remarks>
    /// When reaching a patrol boundary, the Janitor pauses briefly
    /// before switching direction.
    /// </remarks>
    void Patrol()
    {
        // Define target position
        Vector3 target = new Vector3(targetX, transform.position.y, transform.position.z);

        // Move towards patrol target
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            patrolSpeed * Time.deltaTime
        );

        _animator.Play("Walk");

        // Start waiting when close to target
        if (Mathf.Abs(transform.position.x - targetX) < 0.05f)
        {
            StartCoroutine(Wait());
        }

        // Face movement direction
        Flip(targetX);
    }

    /// <summary>
    /// Moves directly towards the player.
    /// </summary>
    /// <remarks>
    /// This behaviour overrides patrol when the player is detected.
    /// </remarks>
    void Chase()
    {
        if (player == null) return;

        // Move towards player position
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            chaseSpeed * Time.deltaTime
        );

        _animator.Play("Walk");

        // Face the player
        Flip(player.position.x);
    }

    /// <summary>
    /// Pauses movement at patrol points before reversing direction.
    /// </summary>
    /// <returns>Coroutine controlling wait duration.</returns>
    IEnumerator Wait()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        // Switch patrol direction
        targetX = Mathf.Approximately(targetX, rightPoint)
            ? leftPoint
            : rightPoint;

        isWaiting = false;
    }

    /// <summary>
    /// Flips the sprite to face a given horizontal target.
    /// </summary>
    /// <param name="targetXPos">Target X position used to determine facing direction.</param>
    void Flip(float targetXPos)
    {
        transform.localScale = new Vector3(
            targetXPos > transform.position.x ? 1 : -1,
            1,
            1
        );
    }
}