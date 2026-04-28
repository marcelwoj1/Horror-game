using UnityEngine;

/// <summary>
/// Handles boss attack detection and applies damage to the player when in range.
/// </summary>
/// <remarks>
/// This script is responsible for:
/// - Detecting when the player is within attack range
/// - Enforcing attack cooldown timing
/// - Applying directional knockback to the player
/// - Preventing attacks during restricted states (e.g., slam attack, hiding)
///
/// It integrates with multiple boss systems such as:
/// - Boss state (aggressive, dead)
/// - Slam attack state
/// - Shadow grab behaviour
/// </remarks>
public class BossAttackHitbox : MonoBehaviour
{
    /// <summary>Reference to the boss manager controlling overall behaviour.</summary>
    private Boss_manager boss_manager;

    [Header("Variables")]

    /// <summary>Damage dealt to the player per attack.</summary>
    public int damage = 1;

    /// <summary>Maximum distance required to trigger an attack.</summary>
    public float detectionDistance = 1.5f;

    /// <summary>Minimum time between consecutive attacks.</summary>
    public float damageCooldown = 1.0f;

    /// <summary>Force applied to the player when hit.</summary>
    public float knockbackForce = 25f;

    /// <summary>Timestamp indicating when the next attack is allowed.</summary>
    private float _nextDamageTime;

    [Header("Components")]

    /// <summary>Handles attack animations.</summary>
    private SpriteAnimator _animator;

    /// <summary>Provides access to player state (e.g., hiding).</summary>
    private PlayerManager _playerManager;

    /// <summary>Reference to the player transform.</summary>
    private Transform _playerTransform;

    /// <summary>Handles player health and damage application.</summary>
    private PlayerHealth _playerHealth;

    /// <summary>Reference to slam attack state.</summary>
    private BossSlamAttack _bossSlamAttack;

    [Header("References")]

    /// <summary>Reference to shadow grab behaviour.</summary>
    private ShadowGrab _shadowGrab;

    /// <summary>
    /// Initialises component references.
    /// </summary>
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

    /// <summary>
    /// Checks attack conditions each frame and applies damage if valid.
    /// </summary>
    /// <remarks>
    /// The boss will not attack if:
    /// - Required references are missing
    /// - The boss is dead
    /// - The player is hiding
    /// - The attack cooldown has not elapsed
    /// - The boss is not aggressive
    /// - A slam attack is currently active
    ///
    /// When conditions are met:
    /// - The attack animation is played
    /// - The player receives damage and knockback
    /// - Shadow grab state is reset if active
    /// </remarks>
    private void Update()
    {
        // Validate required references
        if (_playerTransform == null || boss_manager == null || _playerManager == null || _playerHealth == null)
            return;

        // Prevent attacking in invalid states
        if (boss_manager.isDead) return;
        if (_playerManager.IsHiding) return;
        if (Time.time < _nextDamageTime) return;
        if (!boss_manager.isAggressive) return;
        if (_bossSlamAttack.GroundPoundAttacking) return;

        // Check distance to player
        float distance = Vector2.Distance(transform.position, _playerTransform.position);

        if (distance <= detectionDistance)
        {
            // Reset shadow grab interaction state
            _shadowGrab.waitingForClick = false;

            // Play attack animation
            _animator.Play("Attack");

            // Calculate knockback direction based on boss facing direction
            float side = boss_manager.transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;

            // Apply damage and knockback to player
            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);

            // Set next allowed attack time
            _nextDamageTime = Time.time + damageCooldown;
        }
    }
}