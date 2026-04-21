using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Components")]
    private Transform player;
    public Transform foodTarget;
    private PlayerManager _playerManager;
    private float startY;
    private Rigidbody2D rb;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Vision")]
    public float viewDistance = 6f;
    public float viewAngle = 60f;
    public LayerMask visionMask;
    public float levelTolerance = 0.5f;

    [Header("Patrol")]
    public Transform patrolPointsParent;
    
    List<Vector3> patrolPoints = new List<Vector3>();
    int currentPoint;

    public bool isKnockedBack;
    private SpriteAnimator _animator;

    [Header("Chase Randomness")]
    public float offsetMagnitude = 2f;
    public float offsetSpeed = 2f;
    public float flipDebounceTime = 0.5f;

    private float _noiseTime;
    private float _chaseDirection = 1f;
    private float _flipTimer;

    public enum EnemyState
    {
        Patrol,
        Chase,
        Search,
        Food
    }

    public EnemyState currentState;

    void Start()
    {
        startY = transform.position.y;
        foreach (Transform point in patrolPointsParent)
        {
            patrolPoints.Add(point.position);
        }

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

        DetectSpiderFood();
        DetectPlayer();

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                ChasePlayer();
                break;

            case EnemyState.Search:
                Search();
                break;

            case EnemyState.Food:
                GoToFood();
                break;
        }
    }

    void DetectPlayer()
    {
        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        if (distance < viewDistance)
        {
            Vector2 facing = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            float angle = Vector2.Angle(facing, direction);

            if (angle < viewAngle / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, viewDistance, ~visionMask);

                if (hit.collider != null && hit.collider.CompareTag("Player") && _playerManager.IsHiding == false)
                {
                    currentState = EnemyState.Chase;
                }
            }
        }
    }

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
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1,1,1);
        else
            transform.localScale = new Vector3(-1,1,1);
    }

    void ChasePlayer()
    {
        if (_playerManager.IsHiding)
            currentState = EnemyState.Search;

        // Calculate offset target
        _noiseTime += Time.deltaTime * offsetSpeed;
        float xOffset = (Mathf.PerlinNoise(_noiseTime, 0f) - 0.5f) * 2f * offsetMagnitude;
        float targetX = player.position.x + xOffset;

        float targetDirection = Mathf.Sign(targetX - transform.position.x);

        // Debounce flip
        if (targetDirection != _chaseDirection && _flipTimer <= 0)
        {
            _chaseDirection = targetDirection;
            _flipTimer = flipDebounceTime;
        }

        if (_flipTimer > 0)
            _flipTimer -= Time.deltaTime;

        Vector3 move = new Vector3(_chaseDirection * chaseSpeed * Time.deltaTime, 0, 0);
        _animator.Play("Walk");
        transform.position += move;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > viewDistance * 1.5f)
        {
            currentState = EnemyState.Search;
        }

        transform.localScale = new Vector3(_chaseDirection, 1, 1);
    }

    void Search()
    {
        // Simple version: return to patrol
        currentState = EnemyState.Patrol;
    }

    void DetectSpiderFood()
    {
        GameObject food = GameObject.FindGameObjectWithTag("SpiderFood");

        if (food == null)
            return;

        float yDifference = Mathf.Abs(food.transform.position.y - transform.position.y);

        if (yDifference <= levelTolerance)
        {
            foodTarget = food.transform;
            currentState = EnemyState.Food;
        }
    }
    void GoToFood()
    {
        if (foodTarget == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        Vector2 current = transform.position;
        Vector2 target = foodTarget.position;

        Vector2 dir = (target - current).normalized;

        transform.position = Vector2.MoveTowards(
            current,
            target,
            chaseSpeed * Time.deltaTime
        );

        _animator.Play("Walk");

        if (dir.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (Vector2.Distance(current, target) < 0.3f)
        {
            StartCoroutine(EatFood(foodTarget.gameObject));
            foodTarget = null;
            currentState = EnemyState.Search;
        }
    }
    IEnumerator EatFood(GameObject food)
    {
        float oldSpeed = chaseSpeed;
        chaseSpeed = 0f;

        yield return new WaitForSeconds(5f);

        if (food != null)
        Destroy(food);

    chaseSpeed = oldSpeed;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}