using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth_ = 0;
    private int health_ = 0;

    public event Action<int,int> OnHealthChanged;

    private void Awake()
    {
        health_ = maxHealth_;
    }

    void Start()
    {
        OnHealthChanged?.Invoke(health_, maxHealth_);
    }

    public void AddDamege(int damage)
    {
        health_ -= damage;
        health_ = Math.Max(0, health_);
        OnHealthChanged?.Invoke(health_, maxHealth_);
    }

    public void AddHeel(int heel)
    {
        health_ += heel;
        health_ = Math.Min(maxHealth_, health_);
        OnHealthChanged?.Invoke(health_, maxHealth_);
    }

    public int GetHealth()
    {
        return health_;
    }

    public int GetMaxHealth()
    {
        return maxHealth_;
    }
}
