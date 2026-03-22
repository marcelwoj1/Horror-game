using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private Enemy enemy;

    public int damage = 1;
    private SpriteAnimator _animator;
    private Hiding _hiding;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        _animator = GetComponentInParent<SpriteAnimator>();
        _hiding = GameObject.Find("Player").GetComponent<Hiding>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (enemy.isDead == true) return;
        if (_hiding.IsHiding == true) return;
        
        _animator.Play("Attack");

        PlayerHealth ph = collision.GetComponent<PlayerHealth>();

        if (ph != null)
        {
            ph.TakeDamage(damage);
        }
    }
}