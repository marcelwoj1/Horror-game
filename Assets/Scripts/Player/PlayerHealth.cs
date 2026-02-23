using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 5;
    public int MaxHealth = 5;
    public Action OnDeath;
    public Action OnHealthChanged;

    public void TakeDamage(int damage)
    {
        Health -= damage;
        OnHealthChanged?.Invoke();
        if (Health <= 0)
        {
            Die();
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
