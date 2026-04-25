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
    private bool isChasing = false;

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
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
            animator.Play("Walk");
        }
    }

    public void StartChasing()
    {
        isChasing = true;
        boss_manager.isAggressive = true;
    }
}
