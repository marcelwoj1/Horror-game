using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Player")]
    public Movement _movement;
    
    [Header("Attack")]
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(1, 1);
    public int attackDamage = 1;
    public float knockbackForce = 10f;
    
    [Header("Layers")]
    public LayerMask hittableLayers;
    public LayerMask enemyLayer;

    private SpriteRenderer _spriteRenderer;


    void Update()
    {
        if (attackPoint != null)
        {
            //Gets the absolute position of the attack point
            float xPos = Mathf.Abs(attackPoint.transform.localPosition.x);

            //Sets the position of the attack point based on whether the player is facing left or right
            if (_movement._spriteRenderer.flipX) // Right
            {
                attackPoint.transform.localPosition = new Vector3(xPos, attackPoint.transform.localPosition.y, attackPoint.transform.localPosition.z);
            }
            else // Left
            {
                attackPoint.transform.localPosition = new Vector3(-xPos, attackPoint.transform.localPosition.y, attackPoint.transform.localPosition.z);
            }
        }
    }

    //Deals damage to any enemies in the attack range
    public void Attack()
    {
        SoundService.Instance?.Play("PlayerAttack");
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(
            attackPoint.position,
            attackSize,
            0f,
            hittableLayers
        );
    
        //Depends on what is hit, does different things
        foreach (Collider2D hitCollider in enemiesHit)
        {
            //Boss
            Boss_manager boss_manager = hitCollider.GetComponent<Boss_manager>();
            if (boss_manager != null)
            {
                //Calculates knockback direction
                Vector2 knockbackDir = (boss_manager.transform.position - transform.position);
                knockbackDir.Normalize();
                knockbackDir.y = 0.2f;
                //Applies damage and knockback
                boss_manager.TakeDamage(attackDamage, knockbackDir * knockbackForce);
            }

            //Enemy
            Enemy enemy = hitCollider.GetComponent<Enemy>();

            if(enemy != null)
            {                
                //Calculates knockback direction
                Vector2 knockbackDir = (enemy.transform.position - transform.position);

                knockbackDir.Normalize();

                knockbackDir.y = 0.2f; 
                //Applies damage and knockback

                enemy.TakeDamage(attackDamage, knockbackDir * knockbackForce);
            }

            //WoodenPlank
            Doors door = hitCollider.GetComponent<Doors>();
            if (door != null)
            {
                //Breaks the planks
                door.breakPlank();
            }
        }

    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}

