using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 5;
    public int MaxHealth = 5;
    public Action OnDeath;
    public Action OnHealthChanged;
    private SpriteAnimator _animator;
    private Movement _movement;

    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        _movement = GetComponent<Movement>();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        OnHealthChanged?.Invoke();
        _animator.Play("Hurt");
        SoundService.Instance?.Play("PlayerHurt");
        if (Health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        TakeDamage(damage);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockback, ForceMode2D.Impulse);
            StartCoroutine(KnockbackRoutine());
        }
    }

    IEnumerator KnockbackRoutine()
    {
        if (_movement != null) _movement.isKnockedBack = true;
        yield return new WaitForSeconds(0.35f);
        if (_movement != null) _movement.isKnockedBack = false;
    }

    public void Heal(int healAmount)
    {
        Health += healAmount;
        OnHealthChanged?.Invoke();
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public void Die()
    {
        OnDeath?.Invoke();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }
    }
}
