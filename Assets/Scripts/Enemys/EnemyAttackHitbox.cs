using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private Enemy enemy;

    public int damage = 1;
    private SpriteAnimator _animator;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        _animator = GetComponentInParent<SpriteAnimator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (enemy.isDead == true) return;
        
        _animator.Play("Attack");

        PlayerHealth ph = collision.GetComponent<PlayerHealth>();

        if (ph != null)
        {
            ph.TakeDamage(damage);
        }
    }
}