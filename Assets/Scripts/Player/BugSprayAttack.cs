using UnityEngine;
using System.Collections;

/// <summary>
/// Handles continuous damage over time when the bug spray effect is active.
/// </summary>
/// <remarks>
/// This system:
/// - Continuously attacks nearby enemies while bug spray is active
/// - Uses a coroutine to apply periodic damage
/// - Only affects enemies flagged as vulnerable to bug spray
///
/// The attack uses an area-based detection similar to melee combat,
/// but applies damage repeatedly over time.
/// </remarks>
public class BugSprayAttack : MonoBehaviour
{
    [Header("Attack")]

    /// <summary>Origin point of the spray attack.</summary>
    public Transform attackPoint;

    /// <summary>Size of the attack area.</summary>
    public Vector2 attackSize = new Vector2(1, 1);

    /// <summary>Damage applied per tick.</summary>
    public int attackDamage = 1;

    /// <summary>Knockback force applied to enemies.</summary>
    public float knockbackForce = 1f;
    
    [Header("Layers")]

    /// <summary>Layer used for enemy detection.</summary>
    public LayerMask enemyLayer;

    /// <summary>Layers that can be affected by the attack.</summary>
    public LayerMask hittableLayers;
    
    [Header("Player Components")]

    /// <summary>Reference to player state manager.</summary>
    public PlayerManager _playerManager;

    /// <summary>Reference to movement system.</summary>
    public Movement _movement;
    
    /// <summary>Controls sprite orientation.</summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>Coroutine controlling continuous attack.</summary>
    private Coroutine sprayCoroutine;

    /// <summary>
    /// Initialises component references.
    /// </summary>
    void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _movement = GetComponent<Movement>();
    }

    /// <summary>
    /// Starts or stops the spray coroutine based on player state.
    /// </summary>
    void Update()
    {
        if (_playerManager.IsBugSprayActive && sprayCoroutine == null)
        {
            sprayCoroutine = StartCoroutine(SprayRoutine());
        }
        else if (!_playerManager.IsBugSprayActive && sprayCoroutine != null)
        {
            StopCoroutine(sprayCoroutine);
            sprayCoroutine = null;
        }
    }

    /// <summary>
    /// Continuously applies damage at set intervals.
    /// </summary>
    /// <returns>Coroutine controlling repeated attacks.</returns>
    IEnumerator SprayRoutine()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(1.5f);
        }
    }
    
    /// <summary>
    /// Detects and damages valid enemies within the spray area.
    /// </summary>
    /// <remarks>
    /// Only enemies marked as affected by bug spray will take damage.
    /// Applies knockback and damage to each valid target.
    /// </remarks>
    public void Attack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(
            attackPoint.position,
            attackSize,
            0f,
            hittableLayers
        );

        foreach (Collider2D hitCollider in enemiesHit)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();

            if (enemy != null)
            {
                // Skip enemies not affected by bug spray
                if (!enemy.Affectedbybugspray) continue;

                Vector2 knockbackDir = (enemy.transform.position - transform.position);
                knockbackDir.Normalize();
                knockbackDir.y = 0.2f;

                enemy.TakeDamage(attackDamage, knockbackDir * knockbackForce);
            }
        }
    }

    /// <summary>
    /// Draws the attack area in the editor for debugging.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}