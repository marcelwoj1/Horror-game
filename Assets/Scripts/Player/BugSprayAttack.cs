using UnityEngine;
using System.Collections;

public class BugSprayAttack : MonoBehaviour
{
    [Header("Attack")]
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(1, 1);
    public int attackDamage = 1;
    public float knockbackForce = 1f;
    
    [Header("Layers")]
    public LayerMask enemyLayer;
    public LayerMask hittableLayers;
    
    [Header("Player Components")]
    public PlayerManager _playerManager;
    public Movement _movement;
    
    private SpriteRenderer _spriteRenderer;
    private Coroutine sprayCoroutine;

    void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _movement = GetComponent<Movement>();
    }

    void Update()
    {
        // Start Coroutine if Bug Spray is active and Coroutine is not running
        if (_playerManager.IsBugSprayActive && sprayCoroutine == null)
        {
            sprayCoroutine = StartCoroutine(SprayRoutine());
        }
        // Stop Coroutine if Bug Spray is not active and Coroutine is running
        else if (!_playerManager.IsBugSprayActive && sprayCoroutine != null)
        {
            StopCoroutine(sprayCoroutine);
            sprayCoroutine = null;
        }
    }

    IEnumerator SprayRoutine()
    {
        while (true)
        {
            //Will attack enemies effected by bug spray every .5 seconds
            Attack();
            yield return new WaitForSeconds(1.5f);
        }
    }
    
    public void Attack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(
            attackPoint.position,
            attackSize,
            0f,
            hittableLayers
        );

        foreach (Collider2D hitCollider in enemiesHit)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();

            if(enemy != null)
            {
                //Will not hit enemies that are not affected by bug spray
                if(enemy.Affectedbybugspray == false) return;
                
                //Will calculate the direction of the knockback
                Vector2 knockbackDir = (enemy.transform.position - transform.position);

                knockbackDir.Normalize();

                knockbackDir.y = 0.2f; 

                //Will apply damage and knockback to enemy
                enemy.TakeDamage(attackDamage, knockbackDir * knockbackForce);
            }
        }

    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}

