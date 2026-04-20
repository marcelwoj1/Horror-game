using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Janitor : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;

    [Header("Patrol")]
    public Transform patrolPointsParent;
    public float waitTime = 5f;

    [Header("Chase")]
    public bool chasePlayer = false;

    private List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPoint;

    private bool isWaiting = false;

    private Transform player;
    private SpriteAnimator _animator;

    void Start()
    {
        foreach (Transform point in patrolPointsParent)
        {
            patrolPoints.Add(point.position);
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<SpriteAnimator>();
    }

    void Update()
    {
        if (chasePlayer)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (isWaiting)
        {
            _animator.Play("Mopping");
            return;
        }

        Vector3 target = patrolPoints[currentPoint];

        transform.position = Vector2.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);
        _animator.Play("Walk");

        if (Vector2.Distance(transform.position, target) < 0.2f)
        {
            StartCoroutine(WaitAtPoint());
        }

        Flip(target.x);
    }

    void Chase()
    {
        if (player == null) return;

        isWaiting = false; // cancel any waiting

        Vector3 target = player.position;

        transform.position = Vector2.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);
        _animator.Play("Walk");

        Flip(target.x);
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        currentPoint++;
        if (currentPoint >= patrolPoints.Count)
            currentPoint = 0;

        isWaiting = false;
    }

    void Flip(float targetX)
    {
        if (targetX > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }
}