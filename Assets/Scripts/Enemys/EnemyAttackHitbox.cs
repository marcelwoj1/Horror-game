using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private Enemy enemy;

    public int damage = 1;
    public float detectionDistance = 1.5f;
    public float damageCooldown = 1.0f;
    public float knockbackForce = 15f;
    private SpriteAnimator _animator;
    private Hiding _hiding;
    private Transform _playerTransform;
    private PlayerHealth _playerHealth;
    private float _nextDamageTime;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        _animator = GetComponentInParent<SpriteAnimator>();
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _hiding = player.GetComponent<Hiding>();
            _playerTransform = player.transform;
            _playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (_playerTransform == null || enemy == null || _hiding == null || _playerHealth == null) return;
        if (enemy.isDead) return;
        if (_hiding.IsHiding) return;
        if (Time.time < _nextDamageTime) return;

        float distance = Vector2.Distance(transform.position, _playerTransform.position);
        if (distance <= detectionDistance)
        {
            _animator.Play("Attack");

            // Direction for knockback (based on enemy facing direction)
            float side = enemy.transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;

            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
            _nextDamageTime = Time.time + damageCooldown;
        }
    }
}