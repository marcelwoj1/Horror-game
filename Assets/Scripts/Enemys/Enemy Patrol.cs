using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Components")]
    private Transform player;
    public Transform foodTarget;
    private PlayerManager _playerManager;
    private Rigidbody2D rb;
    private SpriteAnimator _animator;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Vision")]
    public float viewDistance = 6f;
    public float viewAngle = 60f;
    public float levelTolerance = 0.5f;
    public LayerMask enemyLayer;

    [Header("Patrol")]
    public Transform patrolPointsParent;
    private List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPoint;

    [Header("Chase Randomness")]
    public float offsetMagnitude = 2f;
    public float offsetSpeed = 2f;
    public float flipDebounceTime = 0.5f;

    private float _noiseTime;
    private float _chaseDirection = 1f;
    private float _flipTimer;

    public bool isKnockedBack;

    public static List<SpiderFood> AllFood = new List<SpiderFood>();

    // Register food when it spawns
    public static void RegisterFood(SpiderFood food)
    {
        if (!AllFood.Contains(food))
            AllFood.Add(food);
    }

    public static void UnregisterFood(SpiderFood food)
    {
        if (AllFood.Contains(food))
            AllFood.Remove(food);
    }

    // Enemy State
    public enum EnemyState
    {
        Patrol,
        Chase,
        Food
    }

    public EnemyState currentState;

    void Start()
    {
        // Add all patrol points in children
        foreach (Transform point in patrolPointsParent)
            patrolPoints.Add(point.position);

        currentState = EnemyState.Patrol;

        _animator = GetComponent<SpriteAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerManager = player.GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isKnockedBack)
            return;

        // PRIORITY SYSTEM
        if (TryGetFood())
        {
            // Enemy Chases food
            currentState = EnemyState.Food;
        }
        else if (CanSeePlayer())
        {
            // Enemy Chases player
            currentState = EnemyState.Chase;
        }
        else
        {
            // Enemy Patrols
            currentState = EnemyState.Patrol;
        }

        // EXECUTE BEHAVIOUR
        switch (currentState)
        {
            // Patrol
            case EnemyState.Patrol:
                Patrol();
                break;

            // Chase
            case EnemyState.Chase:
                ChasePlayer();
                break;

            // Food
            case EnemyState.Food:
                GoToFood();
                break;
        }
    }

    //Checks if any food is spawned- top priority
    bool TryGetFood()
    {
        if (AllFood.Count == 0)
        {
            foodTarget = null;
            return false;
        }

        // Find the closest food
        SpiderFood closest = null;
        float minDist = Mathf.Infinity;

        // Loop through all food
        foreach (var food in AllFood)
        {
            // Skip if food is null
            if (food == null) continue;

            // Skip if food is on a different level
            float yDiff = Mathf.Abs(food.transform.position.y - transform.position.y);
            if (yDiff > levelTolerance) continue;

            // Get distance to food
            float dist = Vector2.Distance(transform.position, food.transform.position);

            // Set closest food
            if (dist < minDist)
            {
                minDist = dist;
                closest = food;
            }
        }

        // Set food target
        if (closest != null)
        {
            foodTarget = closest.transform;
            return true;
        }

        foodTarget = null;
        return false;
    }

    // Checks if the enemy can see the player
    bool CanSeePlayer()
    {
        // Calculates direction to player
        Vector2 directionToPlayer = (player.position - transform.position);
        float distance = directionToPlayer.magnitude;

        // Skip if player is too far away
        if (distance > viewDistance)
            return false;

        // Skip if player is not within line of sight
        directionToPlayer.Normalize();
        Vector2 forward = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;

        float angle = Vector2.Angle(forward, directionToPlayer);

        if (angle > viewAngle * 0.5f)
            return false;

        // Draws line to player
        Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.red);

        //Raycast that will hit player
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer,
            viewDistance,
            enemyLayer
        );

        // Skip if raycast doesn't hit anything
        if (hit.collider == null)
            return false;

        // Skip if player is hiding
        if (hit.collider.GetComponentInParent<PlayerManager>() != null &&
            !_playerManager.IsHiding)
        {
            return true;
        }

        return false;
    }

    // Enemy Patrols through set points
    void Patrol()
    {
        // Unignore collision with player
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy Hitbox"), false);

        // Set target
        Vector3 target = patrolPoints[currentPoint];

        // Move towards target
        transform.position = Vector2.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);
        _animator.Play("Walk");

        // Go to next patrol point
        if (Vector2.Distance(transform.position, target) < 0.2f)
        {
            currentPoint++;
            if (currentPoint >= patrolPoints.Count)
                currentPoint = 0;
        }

        // Flips enemy sprite towards target
        transform.localScale = target.x > transform.position.x
            ? new Vector3(1, 1, 1)
            : new Vector3(-1, 1, 1);
    }

    // Enemy chases player
    void ChasePlayer()
    {
        // Unignore collision with player
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy Hitbox"), false);

        // Restores chase speed
        chaseSpeed = 4;

        // Enemy stops chasing when player is hiding
        if (_playerManager.IsHiding)
            return;

        // Enemy adds offset to player position
        _noiseTime += Time.deltaTime * offsetSpeed;
        float xOffset = (Mathf.PerlinNoise(_noiseTime, 0f) - 0.5f) * 2f * offsetMagnitude;
        float targetX = player.position.x + xOffset;

        // Flips enemy sprite towards target
        float targetDirection = Mathf.Sign(targetX - transform.position.x);

        if (targetDirection != _chaseDirection && _flipTimer <= 0)
        {
            _chaseDirection = targetDirection;
            _flipTimer = flipDebounceTime;
        }

        if (_flipTimer > 0)
            _flipTimer -= Time.deltaTime;

        // Move towards target
        transform.position += new Vector3(_chaseDirection * chaseSpeed * Time.deltaTime, 0, 0);
        _animator.Play("Walk");

        // Flips enemy sprite towards target
        transform.localScale = new Vector3(_chaseDirection, 1, 1);
    }

    // Enemy goes to food
    void GoToFood()
    {
        // Ignore collision with player when eating food
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy Hitbox"), true);

        // Skip if food target is null
        if (foodTarget == null)
            return;

        Vector2 current = transform.position;
        Vector2 target = foodTarget.position;

        Vector2 dir = (target - current).normalized;

        // Move towards target
        transform.position = Vector2.MoveTowards(current, target, chaseSpeed * Time.deltaTime);
        _animator.Play("Walk");

        // Flips enemy sprite towards target
        if (dir.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Eat food if close enough
        if (Vector2.Distance(current, target) < 0.3f)
        {
            StartCoroutine(EatFood(foodTarget.GetComponent<SpiderFood>()));
            foodTarget = null;
        }
    }

    // Enemy eats food
    IEnumerator EatFood(SpiderFood food)
    {
        // Enemy slows down while eating
        float oldSpeed = chaseSpeed;
        chaseSpeed = 0f;

        yield return new WaitForSeconds(5f);

        // Destroy food
        if (food != null)
            Destroy(food.gameObject);

        // Enemy speeds up after eating
        chaseSpeed = oldSpeed;
    }

    // Draw gizmos for testing
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}