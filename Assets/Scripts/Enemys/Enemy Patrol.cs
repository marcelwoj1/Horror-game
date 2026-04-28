using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls enemy behaviour using a priority-based AI system.
/// </summary>
/// <remarks>
/// The enemy selects behaviour based on the following priority:
/// 1. Food (highest priority)
/// 2. Player (if visible and not hiding)
/// 3. Patrol (default behaviour)
///
/// This is not a full finite state machine, but a dynamic priority system
/// evaluated every frame.
/// </remarks>
public class EnemyPatrol : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Reference to the player transform.</summary>
    private Transform player;

    /// <summary>Current food target being pursued.</summary>
    public Transform foodTarget;

    /// <summary>Provides access to player state (e.g., hiding).</summary>
    private PlayerManager _playerManager;

    /// <summary>Rigidbody used for physics interactions.</summary>
    private Rigidbody2D rb;

    /// <summary>Handles animation playback.</summary>
    private SpriteAnimator _animator;

    [Header("Movement")]

    /// <summary>Speed while patrolling.</summary>
    public float patrolSpeed = 2f;

    /// <summary>Speed while chasing targets.</summary>
    public float chaseSpeed = 4f;

    [Header("Vision")]

    /// <summary>Maximum detection distance.</summary>
    public float viewDistance = 6f;

    /// <summary>Field of view angle (degrees).</summary>
    public float viewAngle = 60f;

    /// <summary>
    /// Maximum vertical difference allowed when detecting targets.
    /// Prevents detection across different platforms.
    /// </summary>
    public float levelTolerance = 0.5f;

    /// <summary>Layer mask used for raycasting detection.</summary>
    public LayerMask enemyLayer;

    [Header("Patrol")]

    /// <summary>Parent object containing patrol points.</summary>
    public Transform patrolPointsParent;

    /// <summary>List of patrol positions.</summary>
    private List<Vector3> patrolPoints = new List<Vector3>();

    /// <summary>Current patrol point index.</summary>
    private int currentPoint;

    [Header("Chase Randomness")]

    /// <summary>Maximum horizontal offset applied during chasing.</summary>
    public float offsetMagnitude = 2f;

    /// <summary>Speed of offset variation.</summary>
    public float offsetSpeed = 2f;

    /// <summary>Delay between direction changes.</summary>
    public float flipDebounceTime = 0.5f;

    /// <summary>Internal timer for Perlin noise.</summary>
    private float _noiseTime;

    /// <summary>Cooldown preventing rapid direction flipping.</summary>
    private float _flipTimer;

    /// <summary>Indicates if the enemy is currently knocked back.</summary>
    public bool isKnockedBack;

    // =========================
    // Food System
    // =========================

    /// <summary>
    /// Global list of all active food objects.
    /// </summary>
    public static List<SpiderFood> AllFood = new List<SpiderFood>();

    /// <summary>
    /// Registers a food object for detection.
    /// </summary>
    public static void RegisterFood(SpiderFood food)
    {
        if (!AllFood.Contains(food))
            AllFood.Add(food);
    }

    /// <summary>
    /// Removes a food object from detection.
    /// </summary>
    public static void UnregisterFood(SpiderFood food)
    {
        if (AllFood.Contains(food))
            AllFood.Remove(food);
    }

    /// <summary>
    /// Defines possible AI states.
    /// </summary>
    public enum EnemyState
    {
        /// <summary>Default patrol behaviour.</summary>
        Patrol,

        /// <summary>Chasing the player.</summary>
        Chase,

        /// <summary>Moving towards food.</summary>
        Food
    }

    /// <summary>Current active state.</summary>
    public EnemyState currentState;

    /// <summary>
    /// Initialises components and patrol points.
    /// </summary>
    void Start()
    {
        foreach (Transform point in patrolPointsParent)
            patrolPoints.Add(point.position);

        currentState = EnemyState.Patrol;

        _animator = GetComponent<SpriteAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerManager = player.GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Evaluates priorities and updates behaviour each frame.
    /// </summary>
    void Update()
    {
        if (isKnockedBack)
            return;

        if (TryGetFood())
            currentState = EnemyState.Food;
        else if (CanSeePlayer())
            currentState = EnemyState.Chase;
        else
            currentState = EnemyState.Patrol;

        switch (currentState)
        {
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Chase: ChasePlayer(); break;
            case EnemyState.Food: GoToFood(); break;
        }
    }

    /// <summary>
    /// Finds the closest valid food target.
    /// </summary>
    /// <returns>True if a valid food target exists.</returns>
    bool TryGetFood()
    {
        if (AllFood.Count == 0)
        {
            foodTarget = null;
            return false;
        }

        SpiderFood closest = null;
        float minDist = Mathf.Infinity;

        foreach (var food in AllFood)
        {
            if (food == null) continue;

            float yDiff = Mathf.Abs(food.transform.position.y - transform.position.y);
            if (yDiff > levelTolerance) continue;

            float dist = Vector2.Distance(transform.position, food.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = food;
            }
        }

        if (closest != null)
        {
            foodTarget = closest.transform;
            return true;
        }

        foodTarget = null;
        return false;
    }

    /// <summary>
    /// Determines if the player is visible.
    /// </summary>
    /// <returns>True if player is within range, angle, and not hidden.</returns>
    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position);
        float distance = directionToPlayer.magnitude;

        if (distance > viewDistance)
            return false;

        directionToPlayer.Normalize();
        Vector2 forward = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;

        float angle = Vector2.Angle(forward, directionToPlayer);
        if (angle > viewAngle * 0.5f)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, viewDistance, enemyLayer);

        if (hit.collider == null)
            return false;

        return hit.collider.GetComponentInParent<PlayerManager>() != null &&
               !_playerManager.IsHiding;
    }

    /// <summary>
    /// Moves between patrol points.
    /// </summary>
    void Patrol()
    {
        Vector3 target = patrolPoints[currentPoint];

        transform.position = Vector2.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);
        _animator.Play("Walk");

        if (Vector2.Distance(transform.position, target) < 0.2f)
            currentPoint = (currentPoint + 1) % patrolPoints.Count;

        transform.localScale = target.x > transform.position.x ? Vector3.one : new Vector3(-1, 1, 1);
    }

    /// <summary>
    /// Chases the player with slight randomness.
    /// </summary>
    /// <remarks>
    /// Uses Perlin noise to create less predictable movement patterns.
    /// </remarks>
    void ChasePlayer()
    {
        chaseSpeed = 4;
        if (_playerManager.IsHiding)
            return;

        _noiseTime += Time.deltaTime * offsetSpeed;
        float xOffset = (Mathf.PerlinNoise(_noiseTime, 0f) - 0.5f) * 2f * offsetMagnitude;

        float targetX = player.position.x + xOffset;
        float targetDirection = Mathf.Sign(targetX - transform.position.x);

        transform.position += new Vector3(targetDirection * chaseSpeed * Time.deltaTime, 0, 0);
        transform.localScale = new Vector3(targetDirection, 1, 1);

        _animator.Play("Walk");
    }

    /// <summary>
    /// Moves towards food and consumes it when close.
    /// </summary>
    void GoToFood()
    {
        if (foodTarget == null)
            return;

        transform.position = Vector2.MoveTowards(transform.position, foodTarget.position, chaseSpeed * Time.deltaTime);
        _animator.Play("Walk");

        if (Vector2.Distance(transform.position, foodTarget.position) < 0.3f)
        {
            StartCoroutine(EatFood(foodTarget.GetComponent<SpiderFood>()));
            foodTarget = null;
        }
    }

    /// <summary>
    /// Handles food consumption behaviour.
    /// </summary>
    IEnumerator EatFood(SpiderFood food)
    {
        float oldSpeed = chaseSpeed;
        chaseSpeed = 0f;

        yield return new WaitForSeconds(5f);

        if (food != null)
            Destroy(food.gameObject);

        chaseSpeed = 4;
    }

    /// <summary>
    /// Draws vision range in the editor.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}