using UnityEngine;

public class BossChase : MonoBehaviour
{
    [Header("Components")]
    private Transform player;
    private Rigidbody2D rb;
    private SpriteAnimator animator;
    private Boss_manager boss_manager;

    [Header("Variables")]
    public float speed = 2f;
    public bool isChasing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();
        boss_manager = GetComponent<Boss_manager>();
    }

    void Update()
    {
        if (isChasing && boss_manager.isDead == false)
        {
            // Calculates direction to player
            Vector2 direction = (player.position - transform.position).normalized;

            // Moves boss in the direction of the player
            rb.linearVelocity = direction * speed;
            animator.Play("Walk");
        }
    }

    // Function to start chasing and attacking player
    public void StartChasing()
    {
        isChasing = true;
        boss_manager.isAggressive = true;
    }
}
