using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    private Boss_manager boss_manager;

    [Header("Variables")]
    public int damage = 1;
    public float detectionDistance = 1.5f;
    public float damageCooldown = 1.0f;
    public float knockbackForce = 25f;
    private float _nextDamageTime;

    [Header("Components")]
    private SpriteAnimator _animator;
    private PlayerManager _playerManager;
    private Transform _playerTransform;
    private PlayerHealth _playerHealth;
    private BossSlamAttack _bossSlamAttack;

    [Header("References")]
    private ShadowGrab _shadowGrab;

    void Start()
    {
        boss_manager = GetComponentInParent<Boss_manager>();
        _animator = GetComponentInParent<SpriteAnimator>();
        _bossSlamAttack = GetComponentInParent<BossSlamAttack>();
        _shadowGrab = GetComponentInParent<ShadowGrab>();
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
        // Checks if any of the required components are missing or if the boss is dead
        if (_playerTransform == null || boss_manager == null || _playerManager == null || _playerHealth == null) return;

        // Checks if boss is dead, player is hiding, cooldown is not ready, or boss is not aggressive
        if (boss_manager.isDead) return;
        if (_playerManager.IsHiding) return;
        if (Time.time < _nextDamageTime) return;
        if (boss_manager.isAggressive == false) return;
        if (_bossSlamAttack.GroundPoundAttacking == true) return;

        // Checks if player is in range
        float distance = Vector2.Distance(transform.position, _playerTransform.position);
        if (distance <= detectionDistance)
        {
            // Resets shadow grab
            _shadowGrab.waitingForClick = false;
            
            // Boss plays attack animation
            _animator.Play("Attack");

            // Calculates direction to player and deals knockback to player in that direction
            float side = boss_manager.transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;

            // Deals damage to player
            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
            _nextDamageTime = Time.time + damageCooldown;
        }
    }
    
    
}