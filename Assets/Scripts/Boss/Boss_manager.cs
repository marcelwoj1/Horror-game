using UnityEngine;
using System.Collections;

public class Boss_manager : MonoBehaviour
{
    public int health = 21;
    public bool isAggressive = true;
    public bool isDead = false;
    public int HitsTaken = 0;

    private SpriteAnimator _animator;
    private Rigidbody2D rb;

    [Header("Variables")]
    public GameObject shadow;
    private Transform player;

    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if(HitsTaken == 3)
        {
            SpawnShadows();
            HitsTaken = 0;
        }
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (isDead) return;
        isAggressive = true;
        HitsTaken++;

        health -= damage;

        if (health <= 0)
        {
            _animator.Play("Death");
            isDead = true;
        }
        else
        {
            StartCoroutine(KnockbackRoutine(knockback));

            _animator.Play("Hurt");

            GetComponent<Rigidbody2D>().AddForce(knockback, ForceMode2D.Impulse);
        }

    }

    IEnumerator KnockbackRoutine(Vector2 knockback)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.35f);
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void SpawnShadows()
    {
        Instantiate(shadow, player.position - new Vector3(0, 1.6f, 0), Quaternion.identity);
    }
}