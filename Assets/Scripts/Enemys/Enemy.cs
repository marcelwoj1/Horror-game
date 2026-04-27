using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int health = 3;

    [Header("Enemy State")]
    public bool isAggressive = true;
    public bool isDead = false;
    public bool Affectedbybugspray = true;

    [Header("Enemy Drop")]
    public GameObject Key;
    public bool KeyDropped = false;

    [Header("Components")]
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
        // Makes enemy ignore player when not aggressive
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

    // Damage function for taking damage
    public void TakeDamage(int damage, Vector2 knockback)
    {
        // Cant take damage if knocked back or dead
        if (patrol != null && patrol.isKnockedBack) return;//Just for spiders
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

            //Applies knockback force
            //GetComponent<Rigidbody2D>().AddForce(knockback, ForceMode2D.Impulse);
        }

    }

    IEnumerator KnockbackRoutine(Vector2 knockback)
    {
        if (patrol != null) patrol.isKnockedBack = true;

        //Resets velocity and applies knockback force
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.35f);

        if (patrol != null) patrol.isKnockedBack = false;
    }

    // Death function
    public void Die()
    {
        //Checks if there is a key
        if(Key != null)
        {
            //If Janitor, drop the key if not already dropped
            if(gameObject.name == "Janitor" && KeyDropped == false)
            {
                Instantiate(Key, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
                KeyDropped = true;
            }
        }

        //Destroys enemy
        Destroy(gameObject);
    }
}