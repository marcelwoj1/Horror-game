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
    private SpriteAnimator _animator;

    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        // Store world positions of patrol points
        foreach (Transform point in patrolPointsParent)
        {
            patrolPoints.Add(point.position);
        }
    }

    void Update()
    {
        if (patrolPoints.Count == 0) return;

        if (waiting)
        {
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
        _animator.Play("Walk");
        transform.position = Vector3.MoveTowards(
            transform.position,
            patrolPoints[currentPoint],
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, patrolPoints[currentPoint]) < 0.05f)
        {
            waiting = true;
            _animator.Play("Idle");
        }
        Vector3 direction = patrolPoints[currentPoint] - transform.position;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
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
