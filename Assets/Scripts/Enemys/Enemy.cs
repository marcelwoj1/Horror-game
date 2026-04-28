using UnityEngine;
using System.Collections;

/// <summary>
/// Base enemy class responsible for handling core functionality such as
/// health, damage processing, knockback, aggression state, and death behaviour.
/// </summary>
/// <remarks>
/// This script acts as a shared foundation for different enemy types.
/// It manages interactions with the player, including collision behaviour
/// and conditional item drops (e.g., keys).
/// </remarks>
public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]

    /// <summary>Current health value of the enemy.</summary>
    public int health = 3;

    [Header("Enemy State")]

    /// <summary>Determines whether the enemy will interact with the player.</summary>
    public bool isAggressive = true;

    /// <summary>Indicates whether the enemy has been defeated.</summary>
    public bool isDead = false;

    /// <summary>Specifies if the enemy can be affected by bug spray mechanics.</summary>
    public bool Affectedbybugspray = true;

    [Header("Enemy Drop")]

    /// <summary>Prefab of the key dropped upon death (if applicable).</summary>
    public GameObject Key;

    /// <summary>Ensures the key is only spawned once.</summary>
    public bool KeyDropped = false;

    [Header("Components")]

    /// <summary>Handles animation playback.</summary>
    private SpriteAnimator _animator;

    /// <summary>Rigidbody used for physics interactions and knockback.</summary>
    private Rigidbody2D rb;

    /// <summary>Reference to patrol behaviour (used for knockback control).</summary>
    private EnemyPatrol patrol;

    /// <summary>
    /// Initialises component references on start.
    /// </summary>
    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();
        patrol = GetComponent<EnemyPatrol>();
    }

    /// <summary>
    /// Updates collision behaviour based on aggression state.
    /// </summary>
    /// <remarks>
    /// When the enemy is not aggressive, it ignores collisions with the player.
    /// This can be used for stealth mechanics or temporary disengagement.
    /// </remarks>
    void Update()
    {
        if (isAggressive == false)
        {
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Janitor"),
                true
            );
        }
        else
        {
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Janitor"),
                false
            );
        }
    }

    /// <summary>
    /// Applies damage to the enemy and triggers appropriate reactions.
    /// </summary>
    /// <param name="damage">Amount of health to remove.</param>
    /// <param name="knockback">Force applied to the enemy on hit.</param>
    /// <remarks>
    /// Damage will not be applied if the enemy is already dead or currently
    /// in a knockback state. When damaged, the enemy becomes aggressive.
    /// </remarks>
    public void TakeDamage(int damage, Vector2 knockback)
    {
        // Prevent damage if already knocked back or dead
        if (patrol != null && patrol.isKnockedBack) return;
        if (isDead) return;

        isAggressive = true;
        health -= damage;

        if (health <= 0)
        {
            _animator.Play("Death");
            isDead = true;
        }
        else
        {
            // Apply knockback and play hurt animation
            StartCoroutine(KnockbackRoutine(knockback));
            _animator.Play("Hurt");
        }
    }

    /// <summary>
    /// Applies temporary knockback using physics forces.
    /// </summary>
    /// <param name="knockback">Force vector applied to the enemy.</param>
    /// <returns>Coroutine controlling knockback duration.</returns>
    /// <remarks>
    /// During knockback, the enemy is temporarily prevented from performing
    /// other actions such as movement or taking further damage.
    /// </remarks>
    IEnumerator KnockbackRoutine(Vector2 knockback)
    {
        if (patrol != null) patrol.isKnockedBack = true;

        // Reset velocity before applying force for consistent behaviour
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.35f);

        if (patrol != null) patrol.isKnockedBack = false;
    }

    /// <summary>
    /// Handles enemy death and optional item drop.
    /// </summary>
    /// <remarks>
    /// If the enemy is identified as a "Janitor", it will drop a key
    /// upon death, ensuring the key is only spawned once.
    /// </remarks>
    public void Die()
    {
        if (Key != null)
        {
            if (gameObject.name == "Janitor" && KeyDropped == false)
            {
                Instantiate(Key, transform.position, Quaternion.identity);
                KeyDropped = true;
            }
        }

        Destroy(gameObject);
    }
}