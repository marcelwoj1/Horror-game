using UnityEngine;
using System.Collections;

public class ShadowAttack : MonoBehaviour
{
    [Header("Components")]
    private Transform player;
    private SpriteAnimator _animator;
    private PlayerHealth _playerHealth;
    private BoxCollider2D _hitbox;

    [Header("Variables")]
    public int damage = 1;
    public float detectionDistance = 1.5f;
    public float knockbackForce = 15f;

    void Start()
    {
        // Getting components
        _animator = GetComponent<SpriteAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerHealth = player.GetComponent<PlayerHealth>();
        _hitbox = GetComponent<BoxCollider2D>();
        _hitbox.enabled = false;
        StartCoroutine(AttackRoutine());
    }
    
    IEnumerator AttackRoutine()
    {
        float timer = 0f;

        // Following player X for 4 seconds
        while (timer < 4f)
        {
            if (gameObject != null)
            {
                Vector3 pos = transform.position;
                pos.x = player.position.x;
                transform.position = pos;
            }

            timer += Time.deltaTime;
            yield return null;

            _animator.Play("Attack");
        }
    }
    public void AttackPlayer()
    {
        // Enabling hitbox for 1 second to detect player
        _hitbox.enabled = true;
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionDistance)
        {
            // Direction for knockback (based on enemy facing direction)
            float side = transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;

            _playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
        }
        _animator.Play("Death");
    }

    // Destroys the Shadow object at end of animation
    public void Destroy()
    {
        Destroy(gameObject);
    }


    
}
