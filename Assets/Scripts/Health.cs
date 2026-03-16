using System;
using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    public int CurrentHealth { get; private set; }

    public event Action<int, int> OnHealthChange;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChange?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        GetComponent<PlayerController>().Die();
    }

    public void TakeDamage(int damage) //
    {
        CurrentHealth -= damage;

        if(CurrentHealth< 0) CurrentHealth = 0;

        OnHealthChange?.Invoke(CurrentHealth, maxHealth);

        if(CurrentHealth == 0)
        {
            Die();
        }
    }
}
