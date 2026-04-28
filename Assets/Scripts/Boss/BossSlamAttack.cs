using System.Collections;
using UnityEngine;

/// <summary>
/// Controls a multi-phase ground slam attack performed by the boss.
/// </summary>
/// <remarks>
/// Attack phases:
/// 1. Boss ascends into the air
/// 2. Tracks the player horizontally for a short duration
/// 3. Locks position and briefly hovers
/// 4. Slams down rapidly to the ground
/// 5. Applies damage if the player is within range, otherwise enters a stunned state
///
/// A ground shadow is used to visually indicate the impact location,
/// giving the player an opportunity to react.
/// </remarks>
public class BossSlamAttack : MonoBehaviour
{
    /// <summary>Reference to the player transform.</summary>
    private Transform player;

    /// <summary>Reference to the boss manager for state changes.</summary>
    private Boss_manager boss_manager;

    [Header("Movement Settings")]

    /// <summary>Height the boss ascends to before slamming.</summary>
    private float flyHeight = 10f;

    /// <summary>Speed of upward movement.</summary>
    private float flySpeed = 20f;

    /// <summary>Speed at which the boss tracks the player in the air.</summary>
    private float followSpeed = 20f;

    /// <summary>Speed of downward slam.</summary>
    private float fallSpeed = 40f;

    /// <summary>Initial ground Y position.</summary>
    private float StartY;

    [Header("Timing")]

    /// <summary>Duration the boss follows the player while airborne.</summary>
    private float followTime = 3f;

    /// <summary>Time spent hovering before the slam.</summary>
    private float hoverTime = 0.4f;

    [Header("Components")]

    /// <summary>Prefab used to display the ground impact indicator.</summary>
    public GameObject GroundShadowPrefab;

    /// <summary>Instance of the ground shadow indicator.</summary>
    private GameObject _groundShadow;

    /// <summary>Reference to player health system.</summary>
    private PlayerHealth _playerHealth;

    [Header("Attacking")]

    /// <summary>Distance required for the slam to damage the player.</summary>
    public float detectionDistance = 1.5f;

    /// <summary>Damage dealt by the slam.</summary>
    public int damage = 2;

    /// <summary>Force applied to the player on hit.</summary>
    public float knockbackForce = 30f;

    /// <summary>Indicates whether the slam attack is currently active.</summary>
    public bool GroundPoundAttacking = false;

    /// <summary>
    /// Initialises references and stores starting position.
    /// </summary>
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        boss_manager = GetComponent<Boss_manager>();
        _playerHealth = player.GetComponent<PlayerHealth>();

        StartY = transform.position.y;
    }

    /// <summary>
    /// Updates the position of the ground shadow to match the boss.
    /// </summary>
    void Update()
    {
        if (_groundShadow != null)
        {
            _groundShadow.transform.position =
                new Vector3(transform.position.x, 28.2f, transform.position.z);
        }
    }

    /// <summary>
    /// Initiates the slam attack if not already active.
    /// </summary>
    public void StartSlamAttack()
    {
        if (!GroundPoundAttacking)
            StartCoroutine(SlamRoutine());
    }

    /// <summary>
    /// Executes the full slam attack sequence.
    /// </summary>
    /// <returns>Coroutine controlling the multi-phase attack.</returns>
    IEnumerator SlamRoutine()
    {
        GroundPoundAttacking = true;

        // Spawn ground indicator
        _groundShadow = Instantiate(
            GroundShadowPrefab,
            new Vector3(transform.position.x, 28.2f, transform.position.z),
            Quaternion.identity
        );

        // Phase 1: Ascend
        float targetY = transform.position.y + flyHeight;
        Vector3 targetHeight = new Vector3(transform.position.x, targetY, transform.position.z);

        while (Mathf.Abs(transform.position.y - targetY) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetHeight,
                flySpeed * Time.deltaTime
            );

            yield return null;
        }

        // Phase 2: Follow player horizontally
        float timer = 0f;

        while (timer < followTime)
        {
            Vector3 targetPos = new Vector3(player.position.x, targetY, player.position.z);

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                followSpeed * Time.deltaTime
            );

            timer += Time.deltaTime;
            yield return null;
        }

        // Lock final position
        Vector3 lockedPos = new Vector3(player.position.x, targetY, player.position.z);
        transform.position = lockedPos;

        // Phase 3: Hover
        yield return new WaitForSeconds(hoverTime);

        // Phase 4: Slam down
        while (transform.position.y > StartY)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, StartY, transform.position.z);

        // Phase 5: Apply result
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer >= detectionDistance)
        {
            // Player avoided attack → boss is stunned
            boss_manager.BossStunned();
        }
        else
        {
            // Player hit → apply damage
            float side = transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;

            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
        }

        // Cleanup shadow indicator
        if (_groundShadow != null)
            Destroy(_groundShadow);

        GroundPoundAttacking = false;
    }
}