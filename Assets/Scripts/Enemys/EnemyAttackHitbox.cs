using UnityEngine;

/// <summary>
/// Handles enemy attack detection and applies damage to the player
/// when they are within a specified range.
/// </summary>
/// <remarks>
/// This script is responsible for:
/// - Detecting when the player is close enough to be attacked
/// - Enforcing attack cooldown timing
/// - Applying directional knockback to the player
/// - Preventing attacks during invalid states (e.g., hiding, death, or eating)
/// </remarks>
public class EnemyAttackHitbox : MonoBehaviour
{
    /// <summary>Reference to the parent enemy.</summary>
    private Enemy enemy;

    [Header("Variables")]

    /// <summary>Amount of damage dealt to the player per attack.</summary>
    public int damage = 1;

    /// <summary>Maximum distance required to trigger an attack.</summary>
    public float detectionDistance = 1.5f;

    /// <summary>Minimum time between consecutive attacks.</summary>
    public float damageCooldown = 1.0f;

    /// <summary>Force applied to the player when hit.</summary>
    public float knockbackForce = 15f;

    /// <summary>Timestamp of when the next attack is allowed.</summary>
    private float _nextDamageTime;

    [Header("Components")]

    /// <summary>Handles enemy attack animations.</summary>
    private SpriteAnimator _animator;

    /// <summary>Provides access to player state (e.g., hiding).</summary>
    private PlayerManager _playerManager;

    /// <summary>Reference to the player transform.</summary>
    private Transform _playerTransform;

    /// <summary>Handles player health and damage processing.</summary>
    private PlayerHealth _playerHealth;

    /// <summary>Reference to patrol behaviour for state checks.</summary>
    private EnemyPatrol _enemyPatrol;

    /// <summary>
    /// Initialises references to required components.
    /// </summary>
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        _enemyPatrol = GetComponentInParent<EnemyPatrol>();
        _animator = GetComponentInParent<SpriteAnimator>();

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
    /// The enemy will not attack if:
    /// - The player or required references are missing
    /// - The enemy is dead
    /// - The player is hiding
    /// - The attack cooldown has not elapsed
    /// - The enemy is not aggressive
    /// - The enemy is currently in a non-combat state (e.g., eating food)
    /// </remarks>
    private void Update()
    {
        // Validate required references
        if (_playerTransform == null || enemy == null || _playerManager == null || _playerHealth == null) return;

        // Prevent attacking in invalid states
        if (enemy.isDead) return;
        if (_playerManager.IsHiding) return;
        if (Time.time < _nextDamageTime) return;
        if (enemy.isAggressive == false) return;

        // Prevent attacking while enemy is prioritising food
        if (_enemyPatrol != null &&
            _enemyPatrol.currentState == EnemyPatrol.EnemyState.Food)
            return;

        // Check distance to player
        float distance = Vector2.Distance(transform.position, _playerTransform.position);

        if (distance <= detectionDistance)
        {
            // Play attack animation
            _animator.Play("Attack");

            // Determine knockback direction based on enemy facing direction
            float side = enemy.transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;

            // Apply damage and knockback to the player
            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);

            // Set next allowed attack time
            _nextDamageTime = Time.time + damageCooldown;
        }
    }
}