using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int health = 3;

    private SpriteAnimator _animator;
    private Rigidbody2D rb;
    private EnemyPatrol patrol;
    [HideInInspector] public bool isDead = false;

    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();
        patrol = GetComponent<EnemyPatrol>();
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (patrol != null && patrol.isKnockedBack) return;
        if (isDead) return;

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
        if (patrol != null) patrol.isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.35f);

        if (patrol != null) patrol.isKnockedBack = false;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}