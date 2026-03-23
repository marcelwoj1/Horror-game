using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 5;
    public int MaxHealth = 5;
    public Action OnDeath;
    public Action OnHealthChanged;
    private SpriteAnimator _animator;

    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        OnHealthChanged?.Invoke();
        _animator.Play("Hurt");
        if (Health <= 0)
        {
            Die();
        }
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
