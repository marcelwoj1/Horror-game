using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform player;
    public Hiding _hiding;
    private float startY;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Vision")]
    public float viewDistance = 6f;
    public float viewAngle = 60f;
    public LayerMask visionMask;

    [Header("Patrol")]
    public Transform patrolPointsParent;
    
    List<Vector3> patrolPoints = new List<Vector3>();
    int currentPoint;

    public bool isKnockedBack;
    private SpriteAnimator _animator;

    enum EnemyState
    {
        Patrol,
        Chase,
        Search
    }

    EnemyState currentState;

    void Start()
    {
        startY = transform.position.y;
        foreach (Transform point in patrolPointsParent)
        {
            patrolPoints.Add(point.position);
        }

        currentState = EnemyState.Patrol;
        _animator = GetComponent<SpriteAnimator>();
    }

    void Update()
    {
        if (isKnockedBack)
            return;
            
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

                if (hit.collider != null && hit.collider.CompareTag("Player") && !_hiding.IsHiding)
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
        if (_hiding.IsHiding)
            currentState = EnemyState.Search;

        float direction = Mathf.Sign(player.position.x - transform.position.x);

        Vector3 target = new Vector3(player.position.x, startY, transform.position.z);

        Vector3 move = new Vector3(direction * chaseSpeed * Time.deltaTime, 0, 0);
        _animator.Play("Walk");
        float distance = Vector2.Distance(transform.position, player.position);
        transform.position += move;

        if (distance > viewDistance * 1.5f)
        {
            currentState = EnemyState.Search;
        }
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1,1,1);
        else
            transform.localScale = new Vector3(-1,1,1);
    }

    void Search()
    {
        // Simple version: return to patrol
        currentState = EnemyState.Patrol;
    }
    void OnDrawGizmos()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, viewDistance);
}
}