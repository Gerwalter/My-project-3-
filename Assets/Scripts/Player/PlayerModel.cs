using System;
using UnityEngine;

public class PlayerModel
{
    public float MaxLife { get; private set; }
    public float CurrentLife { get; private set; }
    public int Gold { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    public PlayerModel(float maxLife)
    {
        MaxLife = maxLife;
        CurrentLife = maxLife;
    }

    public void TakeDamage(float dmg)
    {
        CurrentLife -= dmg;
        OnHealthChanged?.Invoke(CurrentLife, MaxLife);
        if (CurrentLife <= 0) OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        CurrentLife = Mathf.Min(CurrentLife + amount, MaxLife);
        OnHealthChanged?.Invoke(CurrentLife, MaxLife);
    }

    public void AddGold(int amount) => Gold += amount;
}
