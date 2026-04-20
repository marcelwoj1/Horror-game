using UnityEngine;
using System.Collections;

public class BugSprayAttack : MonoBehaviour
{
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(1, 1);
    public LayerMask enemyLayer;
    public int attackDamage = 1;
    public float knockbackForce = 5f;
    public LayerMask hittableLayers;
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
    if (_playerManager.IsBugSprayActive && sprayCoroutine == null)
    {
        sprayCoroutine = StartCoroutine(SprayRoutine());
    }
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
        Attack();
        yield return new WaitForSeconds(1f);
    }
}
    
    public void Attack()
    {
        //SoundService.Instance?.Play("PlayerAttack");
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(
            attackPoint.position,
            attackSize,
            0f,
            hittableLayers
        );
    

        foreach (Collider2D hitCollider in enemiesHit)
        {
            //Enemy
            Enemy enemy = hitCollider.GetComponent<Enemy>();

            if(enemy != null)
            {
                
                Vector2 knockbackDir = (enemy.transform.position - transform.position);

                knockbackDir.Normalize();

                knockbackDir.y = 0.2f; 

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

