using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Movement _movement;
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(1, 1);
    public LayerMask enemyLayer;
    public int attackDamage = 1;
    public float knockbackForce = 10f;
    
    private SpriteRenderer _spriteRenderer;


    void Update()
    {
        if (attackPoint != null)
        {
            float xPos = Mathf.Abs(attackPoint.transform.localPosition.x);

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
    public void Attack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(
        attackPoint.position,
        attackSize,
        0f,
        enemyLayer
    );
    

    foreach (Collider2D hitCollider in enemiesHit)
    {
        Enemy enemy = hitCollider.GetComponent<Enemy>();

        if(enemy != null)
        {
            
            Vector2 knockbackDir = (enemy.transform.position - transform.position);

            knockbackDir.Normalize();

            knockbackDir.y = 0.2f; 

            enemy.TakeDamage(1, knockbackDir * knockbackForce);
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

