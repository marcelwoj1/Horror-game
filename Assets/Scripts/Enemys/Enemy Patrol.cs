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

    public float distance;

    public static List<SpiderFood> AllFood = new List<SpiderFood>();

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

    public enum EnemyState
    {
        Patrol,
        Chase,
        Food
    }

    public EnemyState currentState;

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

    void Update()
    {
        if (isKnockedBack)
            return;

        // PRIORITY SYSTEM
        if (TryGetFood())
        {
            currentState = EnemyState.Food;
        }
        else if (CanSeePlayer())
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Patrol;
        }

        // EXECUTE BEHAVIOUR
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                ChasePlayer();
                break;

            case EnemyState.Food:
                GoToFood();
                break;
        }
    }

    // FOOD CHECK (TOP PRIORITY)
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

    // PLAYER DETECTION
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

    Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.red);

    RaycastHit2D hit = Physics2D.Raycast(
        transform.position,
        directionToPlayer,
        viewDistance,
        ~enemyLayer
    );

    if (hit.collider == null)
        return false;

    Debug.Log("Ray hit: " + hit.collider.name);

    if (hit.collider.GetComponentInParent<PlayerManager>() != null &&
        !_playerManager.IsHiding)
    {
        return true;
    }

    return false;
}

    // PATROL
    void Patrol()
    {
        Vector3 target = patrolPoints[currentPoint];

        transform.position = Vector2.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);
        _animator.Play("Walk");

        if (Vector2.Distance(transform.position, target) < 0.2f)
        {
            currentPoint++;
            if (currentPoint >= patrolPoints.Count)
                currentPoint = 0;
        }

        transform.localScale = target.x > transform.position.x
            ? new Vector3(1, 1, 1)
            : new Vector3(-1, 1, 1);
    }

    // CHASE
    void ChasePlayer()
    {
        if (_playerManager.IsHiding)
            return;

        _noiseTime += Time.deltaTime * offsetSpeed;
        float xOffset = (Mathf.PerlinNoise(_noiseTime, 0f) - 0.5f) * 2f * offsetMagnitude;
        float targetX = player.position.x + xOffset;

        float targetDirection = Mathf.Sign(targetX - transform.position.x);

        if (targetDirection != _chaseDirection && _flipTimer <= 0)
        {
            _chaseDirection = targetDirection;
            _flipTimer = flipDebounceTime;
        }

        if (_flipTimer > 0)
            _flipTimer -= Time.deltaTime;

        transform.position += new Vector3(_chaseDirection * chaseSpeed * Time.deltaTime, 0, 0);
        _animator.Play("Walk");

        transform.localScale = new Vector3(_chaseDirection, 1, 1);
    }

    // GO TO FOOD
    void GoToFood()
    {
        if (foodTarget == null)
            return;

        Vector2 current = transform.position;
        Vector2 target = foodTarget.position;

        Vector2 dir = (target - current).normalized;

        transform.position = Vector2.MoveTowards(current, target, chaseSpeed * Time.deltaTime);
        _animator.Play("Walk");

        if (dir.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (Vector2.Distance(current, target) < 0.3f)
        {
            StartCoroutine(EatFood(foodTarget.GetComponent<SpiderFood>()));
            foodTarget = null;
        }
    }

    // EAT FOOD
    IEnumerator EatFood(SpiderFood food)
    {
        float oldSpeed = chaseSpeed;
        chaseSpeed = 0f;

        yield return new WaitForSeconds(5f);

        if (food != null)
            Destroy(food.gameObject);

        chaseSpeed = oldSpeed;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}