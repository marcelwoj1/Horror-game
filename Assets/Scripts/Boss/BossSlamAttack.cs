using System.Collections;
using UnityEngine;

public class BossSlamAttack : MonoBehaviour
{
    private Transform player;
    private Boss_manager boss_manager;

    [Header("Movement Settings")]
    private float flyHeight = 10f;
    private float flySpeed = 20f;
    private float followSpeed = 20f;
    private float fallSpeed = 40f;
    private float StartY;

    [Header("Timing")]
    private float followTime = 3f;
    private float hoverTime = 0.4f;

    [Header("Components")]
    public GameObject GroundShadowPrefab;
    private GameObject _groundShadow;
    private PlayerHealth _playerHealth;

    [Header("Attacking")]
    public float detectionDistance = 1.5f;
    public int damage = 2;
    public float knockbackForce = 30f;
    public bool GroundPoundAttacking = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        boss_manager = GetComponent<Boss_manager>();
        _playerHealth = player.GetComponent<PlayerHealth>();
        StartY = transform.position.y;
    }

    void Update()
    {
        // Shadow follows boss position exactly
        if (_groundShadow != null)
        {
            _groundShadow.transform.position = new Vector3(transform.position.x, 28.2f, transform.position.z);
        }
    }

    public void StartSlamAttack()
    {
        if (!GroundPoundAttacking)
            StartCoroutine(SlamRoutine());
    }

    IEnumerator SlamRoutine()
    {
        GroundPoundAttacking = true;

        // Creating shadow on the ground
        _groundShadow = Instantiate(
            GroundShadowPrefab,
            new Vector3(transform.position.x, 28.2f, transform.position.z),
            Quaternion.identity
        );

        // Boss flies into air
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

        // Boss follows player for a set time
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

        // After follow time is up the position is locked
        Vector3 lockedPos = new Vector3(player.position.x, targetY, player.position.z);
        transform.position = lockedPos;

        // Waits into air for another set time
        yield return new WaitForSeconds(hoverTime);

        // Flies down to the ground at locked position
        while (transform.position.y > StartY)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        // Snaps back to earlier ground position
        transform.position = new Vector3(transform.position.x, StartY, transform.position.z);

        // Checks if player is in range
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer >= detectionDistance)
        {
            // Boss gets stunned if the player is not in range
            boss_manager.BossStunned();
        }
        else
        {
            // Boss deals damage to the player if the player is in range
            float side = transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;
            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
        }

        // Removes the shadow from the ground
        if (_groundShadow != null)
            Destroy(_groundShadow);

        // Boss is no longer attacking
        GroundPoundAttacking = false;
    }
}