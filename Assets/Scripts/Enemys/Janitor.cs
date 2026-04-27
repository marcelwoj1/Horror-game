using System.Collections;
using UnityEngine;

public class Janitor : MonoBehaviour
{
    [Header("Components")]
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

    //Patrol Points
    private float leftPoint;
    private float rightPoint;
    private float targetX;

    private bool isWaiting;

    void Start()
    {
        float startX = transform.position.x;

        //Set patrol points
        leftPoint = startX - patrolRange;
        rightPoint = startX + patrolRange;

        //Set starting target
        targetX = rightPoint;

        _animator = GetComponent<SpriteAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerManager = player.GetComponent<PlayerManager>();
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        //If player is hiding, janitor can't see them
        if(_playerManager.IsHiding == true)
        {
            chasePlayer = false;
        }
        //If Janitor is aggressive and player is not hiding, janitor chases player
        if(enemy.isAggressive == true && _playerManager.IsHiding == false)
        {
            chasePlayer = true;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Janitor"), false);
        }

        //If waiting, janitor mops
        if (isWaiting)
        {
            _animator.Play("Mopping");
            if(chasePlayer == true)
            {
                isWaiting = false;
            }
            return;
        }

        //Chase player if chaising
        if (chasePlayer)
            Chase();
        //Patrol if not chaising
        else
            Patrol();
    }

    void Patrol()
    {
        //Set target
        Vector3 target = new Vector3(targetX, transform.position.y, transform.position.z);

        //Move towards target
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            patrolSpeed * Time.deltaTime
        );

        _animator.Play("Walk");

        //If close to target, wait
        if (Mathf.Abs(transform.position.x - targetX) < 0.05f)
        {
            StartCoroutine(Wait());
        }

        //Flip janitor to look at next target
        Flip(targetX);
    }

    void Chase()
    {
        if (player == null) return;

        //Move towards player
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            chaseSpeed * Time.deltaTime
        );

        _animator.Play("Walk");

        //Flip janitor to look at player
        Flip(player.position.x);
    }

    //Wait before moving to next patrol point
    IEnumerator Wait()
    {
        //Set waiting
        isWaiting = true;

        //Wait
        yield return new WaitForSeconds(waitTime);

        //Flip target
        targetX = Mathf.Approximately(targetX, rightPoint)
            ? leftPoint
            : rightPoint;

        //Stop waiting
        isWaiting = false;
    }

    //Flip janitor to look at target
    void Flip(float targetXPos)
    {
        transform.localScale = new Vector3(
            targetXPos > transform.position.x ? 1 : -1,
            1,
            1
        );
    }
}