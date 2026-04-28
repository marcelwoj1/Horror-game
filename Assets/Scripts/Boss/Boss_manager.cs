using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the boss behaviour, including health, state transitions,
/// damage handling, and attack selection.
/// </summary>
/// <remarks>
/// The boss operates using a state-driven system:
/// - Aggressive: Actively attacks the player
/// - Stunned: Temporarily disabled after certain interactions
/// - Invincible: Cannot take damage during specific phases
///
/// Special attacks are triggered after a number of hits,
/// introducing a reactive combat mechanic.
/// </remarks>
public class Boss_manager : MonoBehaviour
{
    [Header("Boss Stats")]

    /// <summary>Current health of the boss.</summary>
    public int health = 21;

    /// <summary>Number of hits taken since last special attack.</summary>
    private int HitsTaken;

    /// <summary>Determines if the boss is actively engaging the player.</summary>
    public bool isAggressive = true;

    /// <summary>Indicates whether the boss is defeated.</summary>
    public bool isDead = false;

    /// <summary>Indicates whether the boss is currently stunned.</summary>
    public bool isStunned = false;

    /// <summary>Prevents the boss from taking damage.</summary>
    public bool isInvincible = false;

    [Header("Components")]

    /// <summary>Handles animation playback.</summary>
    private SpriteAnimator _animator;

    /// <summary>Rigidbody used for physics interactions.</summary>
    private Rigidbody2D rb;

    /// <summary>Handles quest progression.</summary>
    private QuestService questService;

    [Header("Prefabs")]

    /// <summary>Shadow attack prefab.</summary>
    public GameObject shadow;

    /// <summary>Cat prefab spawned on boss defeat.</summary>
    public GameObject Cat;

    [Header("References")]

    /// <summary>Reference to the player transform.</summary>
    private Transform player;

    /// <summary>Handles slam attack behaviour.</summary>
    private BossSlamAttack bossSlamAttack;

    /// <summary>Handles chasing behaviour.</summary>
    private BossChase bossChase;

    /// <summary>Handles grab attack behaviour.</summary>
    private ShadowGrab shadowGrab;

    /// <summary>
    /// Initialises component references.
    /// </summary>
    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        bossSlamAttack = GetComponent<BossSlamAttack>();
        bossChase = GetComponent<BossChase>();
        shadowGrab = GetComponent<ShadowGrab>();

        questService = FindAnyObjectByType<QuestService>();
    }

    /// <summary>
    /// Updates boss behaviour and triggers special attacks.
    /// </summary>
    /// <remarks>
    /// After every 3 hits, the boss performs a random special attack:
    /// - Shadow spike
    /// - Ground slam
    /// - Shadow grab
    ///
    /// Special attacks are disabled while stunned.
    /// </remarks>
    void Update()
    {
        // Maintain stunned animation while stunned
        if (isStunned)
        {
            _animator.Play("Stunned");
        }

        // Prevent special attacks during slam attack
        if (!bossSlamAttack.GroundPoundAttacking)
        {
            if (HitsTaken == 3)
            {
                // Cancel attack if stunned
                if (isStunned)
                {
                    HitsTaken = 0;
                    return;
                }

                int attack = Random.Range(0, 3);

                switch (attack)
                {
                    case 0:
                        SpawnShadows();
                        break;

                    case 1:
                        bossSlamAttack.StartSlamAttack();
                        break;

                    case 2:
                        shadowGrab.StartLiftAttack();
                        break;
                }

                HitsTaken = 0;
            }
        }
    }

    /// <summary>
    /// Applies damage to the boss and triggers reactions.
    /// </summary>
    /// <param name="damage">Amount of damage dealt.</param>
    /// <param name="knockback">Force applied to the boss.</param>
    /// <remarks>
    /// Damage is ignored if the boss is dead or invincible.
    /// Each successful hit contributes to triggering special attacks.
    /// </remarks>
    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (isDead || isInvincible)
            return;

        // Aggression depends on stun state
        isAggressive = !isStunned;

        HitsTaken++;
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(KnockbackRoutine(knockback));
            _animator.Play("Hurt");

            rb.AddForce(knockback, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Applies temporary knockback to the boss.
    /// </summary>
    IEnumerator KnockbackRoutine(Vector2 knockback)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.35f);
    }

    /// <summary>
    /// Begins the boss combat phase.
    /// </summary>
    public void BossBegin()
    {
        isAggressive = true;
        isStunned = false;
        isInvincible = false;
    }

    /// <summary>
    /// Places the boss into a stunned state.
    /// </summary>
    /// <remarks>
    /// While stunned:
    /// - The boss cannot attack or chase
    /// - Physics behaviour is modified
    /// - The state lasts for a fixed duration
    /// </remarks>
    public void BossStunned()
    {
        isAggressive = false;
        isStunned = true;

        bossChase.isChasing = false;
        isInvincible = false;

        rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(StunnedRoutine());
        _animator.Play("Stunned");
    }

    /// <summary>
    /// Restores boss state after stun duration.
    /// </summary>
    IEnumerator StunnedRoutine()
    {
        yield return new WaitForSeconds(2f);

        isStunned = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        isAggressive = true;
        bossChase.isChasing = true;
    }

    /// <summary>
    /// Handles boss death and end-of-fight events.
    /// </summary>
    public void Die()
    {
        Instantiate(
            Cat,
            new Vector3(transform.position.x, transform.position.y - 1.6f, transform.position.z),
            Quaternion.identity
        );

        questService.SatisfyQuest("FinalBoss");

        Destroy(gameObject);
    }

    /// <summary>
    /// Spawns a shadow attack above the player.
    /// </summary>
    /// <remarks>
    /// Used as one of the boss's special attacks.
    /// </remarks>
    private void SpawnShadows()
    {
        Instantiate(shadow, new Vector3(player.position.x, 28.32f, 0), Quaternion.identity);
    }
}