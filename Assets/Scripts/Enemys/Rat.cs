using UnityEngine;

public class Rat : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 6f;
    public float wakeDelay = 1f;

    [Header("Components")]
    private Transform player;
    private PlayerManager playerManager;
    private Rigidbody2D rb;
    private SpriteAnimator animator;

    private float wakeTime;

    void Start()
    {
        //Get player reference
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerManager = player.GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();

        //Set wake time
        wakeTime = Time.time + wakeDelay;
    }

    void FixedUpdate()
    {
        //Rat is idle for 1 second after being spawned
        // Idle conditions
        if (Time.time < wakeTime || playerManager.IsHiding)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.Play("Idle");
            return;
        }

        //Get direction to player
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // Move using Rigidbody
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // Animate + flip
        animator.Play("Walk");
        transform.localScale = new Vector3(direction, 1, 1);
    }
}