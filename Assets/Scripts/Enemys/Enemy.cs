using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public PlayerHealth _playerHealth;
    private SpriteAnimator _animator;

    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            _animator.Play("Explosion");
        }
        else
        {
            _animator.Play("Hurt");
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _playerHealth.TakeDamage(1);
            _animator.Play("Attack");
        }
    }
}
