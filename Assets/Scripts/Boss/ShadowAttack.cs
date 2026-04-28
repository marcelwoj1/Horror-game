using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a shadow-based boss attack that tracks the player's position
/// before executing a timed strike.
/// </summary>
/// <remarks>
/// Attack flow:
/// 1. Shadow follows the player's horizontal position for a short duration
/// 2. Attack animation plays while tracking
/// 3. Hitbox activates briefly to deal damage
/// 4. Shadow is destroyed after completing the attack
/// </remarks>
public class ShadowAttack : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Reference to the player transform.</summary>
    private Transform player;

    /// <summary>Handles animation playback.</summary>
    private SpriteAnimator _animator;

    /// <summary>Handles player health and damage.</summary>
    private PlayerHealth _playerHealth;

    /// <summary>Collider used as the attack hitbox.</summary>
    private BoxCollider2D _hitbox;

    [Header("Variables")]

    /// <summary>Damage dealt by the attack.</summary>
    public int damage = 1;

    /// <summary>Distance required for the attack to hit the player.</summary>
    public float detectionDistance = 1.5f;

    /// <summary>Force applied to the player on hit.</summary>
    public float knockbackForce = 15f;

    /// <summary>
    /// Initialises references and begins the attack sequence.
    /// </summary>
    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerHealth = player.GetComponent<PlayerHealth>();
        _hitbox = GetComponent<BoxCollider2D>();

        // Disable hitbox until attack is triggered
        _hitbox.enabled = false;

        StartCoroutine(AttackRoutine());
    }

    /// <summary>
    /// Handles the tracking phase of the attack.
    /// </summary>
    /// <returns>Coroutine controlling tracking duration.</returns>
    /// <remarks>
    /// The shadow follows the player's horizontal position for a fixed time,
    /// creating a telegraphed attack that the player can react to.
    /// </remarks>
    IEnumerator AttackRoutine()
    {
        float timer = 0f;

        while (timer < 4f)
        {
            if (gameObject != null)
            {
                // Follow player on X axis only
                Vector3 pos = transform.position;
                pos.x = player.position.x;
                transform.position = pos;
            }

            _animator.Play("Attack");

            timer += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Activates the attack hitbox and applies damage if the player is in range.
    /// </summary>
    /// <remarks>
    /// This is typically triggered via animation event timing.
    /// The hitbox is briefly enabled to detect the player.
    /// </remarks>
    public void AttackPlayer()
    {
        // Enable hitbox for attack window
        _hitbox.enabled = true;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionDistance)
        {
            // Calculate knockback direction based on facing direction
            float side = transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;

            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
        }

        _animator.Play("Death");
    }

    /// <summary>
    /// Destroys the shadow object after the attack finishes.
    /// </summary>
    /// <remarks>
    /// Typically called at the end of the death animation via an animation event.
    /// </remarks>
    public void Destroy()
    {
        Destroy(gameObject);
    }
}