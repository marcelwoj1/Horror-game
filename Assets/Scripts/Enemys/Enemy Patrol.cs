using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform patrolPointsParent;
    public float speed = 3f;
    public float waitTime = 1f;

    private List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPoint = 0;

    private float waitTimer;
    private bool waiting;

    private Rigidbody2D rb;
    private SpriteAnimator _animator;

    // ⭐ New — prevents patrol from cancelling knockback
    public bool isKnockedBack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<SpriteAnimator>();

        foreach (Transform point in patrolPointsParent)
        {
            patrolPoints.Add(point.position);
        }
    }

    void Update()
    {
        if (patrolPoints.Count == 0) return;

        if (isKnockedBack) return; // ⭐ STOP PATROL DURING KNOCKBACK

        if (waiting)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                waiting = false;
                waitTimer = 0f;
                currentPoint = (currentPoint + 1) % patrolPoints.Count;
            }

            return;
        }

        MoveToPoint();
    }

    void MoveToPoint()
    {
        Vector3 target = patrolPoints[currentPoint];

        float direction = Mathf.Sign(target.x - transform.position.x);

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        _animator.Play("Walk");

        if (Mathf.Abs(transform.position.x - target.x) < 0.1f)
        {
            waiting = true;
            _animator.Play("Idle");
        }

        // Flip sprite
        if (direction > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void OnDrawGizmos()
    {
        if (patrolPointsParent == null) return;

        Gizmos.color = Color.red;

        foreach (Transform point in patrolPointsParent)
        {
            Gizmos.DrawSphere(point.position, 0.2f);
        }
    }
}