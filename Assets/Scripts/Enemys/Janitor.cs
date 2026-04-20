using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Janitor : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;

    [Header("Patrol")]
    public Transform patrolPointsParent;
    public float waitTime = 5f;

    private List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPoint;

    private bool isWaiting = false;
    private SpriteAnimator _animator;

    void Start()
    {
        foreach (Transform point in patrolPointsParent)
        {
            patrolPoints.Add(point.position);
        }

        _animator = GetComponent<SpriteAnimator>();
    }

    void Update()
    {
        Patrol();
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

        // Flip sprite
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
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
}