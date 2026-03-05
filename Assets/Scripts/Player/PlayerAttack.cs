using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Movement _movement;
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(1, 1);
    public LayerMask enemyLayer;
    
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

    foreach (Collider2D enemy in enemiesHit)
    {
        enemy.GetComponent<Enemy>().TakeDamage(1);
    }
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}

