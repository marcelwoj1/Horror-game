using System.Collections;
using UnityEngine;

public class BossSlamAttack : MonoBehaviour
{
    private Transform player;
    private Boss_manager boss_manager;

    [Header("Movement Settings")]
    private float flyHeight = 10f;
    private float flySpeed = 20f;
    private float followSpeed = 20f; // slightly faster for responsiveness
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

        // KEEPING YOUR SHADOW POSITION EXACTLY THE SAME
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

        _groundShadow = Instantiate(
            GroundShadowPrefab,
            new Vector3(transform.position.x, 28.2f, transform.position.z),
            Quaternion.identity
        );

        // --- 1. FLY UP (RELATIVE HEIGHT) ---
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

        // --- 2. FOLLOW PLAYER ---
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

        // --- LOCK POSITION (IMPORTANT FOR SHARP FEEL) ---
        Vector3 lockedPos = new Vector3(player.position.x, targetY, player.position.z);
        transform.position = lockedPos;

        // --- 3. SHORT HOVER ---
        yield return new WaitForSeconds(hoverTime);

        // --- 4. SLAM DOWN ---
        while (transform.position.y > StartY)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        // Snap exactly to ground level
        transform.position = new Vector3(transform.position.x, StartY, transform.position.z);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer >= detectionDistance)
        {
            boss_manager.BossStunned();
        }
        else
        {
            float side = transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;
            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
        }

        // Cleanup shadow
        if (_groundShadow != null)
            Destroy(_groundShadow);

        GroundPoundAttacking = false;
    }
}