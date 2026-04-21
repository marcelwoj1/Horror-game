using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int health = 3;
    public bool isAggressive = true;
    public bool isDead = false;
    public bool Affectedbybugspray = true;
    public GameObject Key;
    public bool KeyDropped = false;

    private SpriteAnimator _animator;
    private Rigidbody2D rb;
    private EnemyPatrol patrol;

    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();
        patrol = GetComponent<EnemyPatrol>();
    }
    void Update()
    {
        if(isAggressive == false)
        {
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Janitor"),
                true
        );
        }
        else
        {
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Janitor"),
                false
        );
        }
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (patrol != null && patrol.isKnockedBack) return;
        if (isDead) return;
        isAggressive = true;

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
        if(Key != null)
        {
            if(gameObject.name == "Janitor" && KeyDropped == false)
            {
                Instantiate(Key, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
                KeyDropped = true;
            }
        }
        Destroy(gameObject);
    }
}