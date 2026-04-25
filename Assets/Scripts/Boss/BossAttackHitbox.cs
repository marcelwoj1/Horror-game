using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    private Boss_manager boss_manager;

    [Header("Variables")]
    public int damage = 1;
    public float detectionDistance = 1.5f;
    public float damageCooldown = 1.0f;
    public float knockbackForce = 15f;
    private float _nextDamageTime;

    [Header("Components")]
    private SpriteAnimator _animator;
    private PlayerManager _playerManager;
    private Transform _playerTransform;
    private PlayerHealth _playerHealth;

    void Start()
    {
        boss_manager = GetComponentInParent<Boss_manager>();
        _animator = GetComponentInParent<SpriteAnimator>();
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerManager = player.GetComponent<PlayerManager>();
            _playerTransform = player.transform;
            _playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (_playerTransform == null || boss_manager == null || _playerManager == null || _playerHealth == null) return;
        if (boss_manager.isDead) return;
        if (_playerManager.IsHiding) return;
        if (Time.time < _nextDamageTime) return;
        if (boss_manager.isAggressive == false) return;

        float distance = Vector2.Distance(transform.position, _playerTransform.position);
        if (distance <= detectionDistance)
        {
            _animator.Play("Attack");

            // Direction for knockback (based on enemy facing direction)
            float side = boss_manager.transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;

            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
            _nextDamageTime = Time.time + damageCooldown;
        }
    }
    
    
}