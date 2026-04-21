using System.Collections;
using UnityEngine;

public class Janitor : MonoBehaviour
{
    private Transform player;
    private PlayerManager _playerManager;
    private SpriteAnimator _animator;
    private Enemy enemy;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolRange = 4f;

    [Header("Chase Toggle")]
    public bool chasePlayer = false;

    [Header("Wait")]
    public float waitTime = 5f;

    private float leftPoint;
    private float rightPoint;
    private float targetX;

    private bool isWaiting;

    void Start()
    {
        float startX = transform.position.x;

        leftPoint = startX - patrolRange;
        rightPoint = startX + patrolRange;

        targetX = rightPoint;

        _animator = GetComponent<SpriteAnimator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerManager = player.GetComponent<PlayerManager>();
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if(_playerManager.IsHiding == true)
        {
            chasePlayer = false;
        }
        if(enemy.isAggressive == true && _playerManager.IsHiding == false)
        {
            chasePlayer = true;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Janitor"), false);
        }
        if (isWaiting)
        {
            _animator.Play("Mopping");
            if(chasePlayer == true)
            {
                isWaiting = false;
            }
            return;
        }

        if (chasePlayer)
            Chase();
        else
            Patrol();
    }

    void Patrol()
    {
        Vector3 target = new Vector3(targetX, transform.position.y, transform.position.z);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            patrolSpeed * Time.deltaTime
        );

        _animator.Play("Walk");

        if (Mathf.Abs(transform.position.x - targetX) < 0.05f)
        {
            StartCoroutine(Wait());
        }

        Flip(targetX);
    }

    void Chase()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            chaseSpeed * Time.deltaTime
        );

        _animator.Play("Walk");

        Flip(player.position.x);
    }

    IEnumerator Wait()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        targetX = Mathf.Approximately(targetX, rightPoint)
            ? leftPoint
            : rightPoint;

        isWaiting = false;
    }

    void Flip(float targetXPos)
    {
        transform.localScale = new Vector3(
            targetXPos > transform.position.x ? 1 : -1,
            1,
            1
        );
    }
}