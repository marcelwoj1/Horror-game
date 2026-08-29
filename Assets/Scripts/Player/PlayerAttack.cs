using UnityEngine;

/// <summary>
/// Handles player attack behaviour, including hit detection,
/// damage application, and knockback effects.
/// </summary>
/// <remarks>
/// This script:
/// - Positions the attack hitbox based on player direction
/// - Detects objects within attack range using physics queries
/// - Applies damage and knockback to enemies and the boss
/// - Interacts with destructible objects (e.g., doors)
///
/// The system uses a box-based hit detection for consistent melee combat.
/// </remarks>
public class PlayerAttack : MonoBehaviour
{
    [Header("Player")]

    /// <summary>Reference to player movement system.</summary>
    public Movement _movement;

    
    private Player_IK _playerIK;
    
    [Header("Attack")]

    /// <summary>Point from which attacks originate.</summary>
    public Transform attackPoint;

    /// <summary>Size of the attack hitbox.</summary>
    public Vector2 attackSize = new Vector2(1, 1);

    /// <summary>Damage dealt per attack.</summary>
    public int attackDamage = 1;

    /// <summary>Force applied to targets when hit.</summary>
    public float knockbackForce = 10f;
    
    [Header("Layers")]

    /// <summary>Layers that can be hit by the attack.</summary>
    public LayerMask hittableLayers;

    /// <summary>Layer used for enemy detection.</summary>
    public LayerMask enemyLayer;

  
    void Start()
    {
        if (_movement == null)
        {
            _movement = GetComponent<Movement>();
        }
        _playerIK = GetComponent<Player_IK>();
    }

    /// <summary>
    /// Updates attack point position based on player facing direction.
    /// </summary>
    /// <remarks>
    /// Ensures the attack hitbox is always positioned in front of the player.
    /// </remarks>
    void Update()
    {
        if (attackPoint != null)
        {
            float xPos = Mathf.Abs(attackPoint.transform.localPosition.x);

            if (_movement._spriteRenderer.flipX) // Facing right
            {
                attackPoint.transform.localPosition =
                    new Vector3(xPos, attackPoint.transform.localPosition.y, attackPoint.transform.localPosition.z);
            }
            else // Facing left
            {
                attackPoint.transform.localPosition =
                    new Vector3(-xPos, attackPoint.transform.localPosition.y, attackPoint.transform.localPosition.z);
            }
        }
    }

    /// <summary>
    /// Executes an attack and applies effects to all valid targets in range.
    /// </summary>
    /// <remarks>
    /// Uses Physics2D.OverlapBox to detect all objects within the attack area.
    /// Different object types receive different interactions:
    /// - Boss: takes damage and knockback
    /// - Enemy: takes damage and knockback
    /// - Doors: can be destroyed
    /// </remarks>
    public void Attack()
    {
        // Play attack sound
        SoundService.Instance?.Play("PlayerAttack");

        // Trigger the IK swing animation
        if (_playerIK != null)
        {
            _playerIK.PlayAttackSwing();
        }

        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(
            attackPoint.position,
            attackSize,
            0f,
            hittableLayers
        );

        foreach (Collider2D hitCollider in enemiesHit)
        {
            // Boss interaction
            Boss_manager boss_manager = hitCollider.GetComponent<Boss_manager>();
            if (boss_manager != null)
            {
                Vector2 knockbackDir = (boss_manager.transform.position - transform.position);
                knockbackDir.Normalize();
                knockbackDir.y = 0.2f;

                boss_manager.TakeDamage(attackDamage, knockbackDir * knockbackForce);
            }

            // Enemy interaction
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector2 knockbackDir = (enemy.transform.position - transform.position);
                knockbackDir.Normalize();
                knockbackDir.y = 0.2f;

                enemy.TakeDamage(attackDamage, knockbackDir * knockbackForce);
            }

            // Environmental interaction (e.g., destructible doors)
            Doors door = hitCollider.GetComponent<Doors>();
            if (door != null)
            {
                door.breakPlank();
            }
        }
    }

    /// <summary>
    /// Draws the attack hitbox in the editor for debugging.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}