using System;
using UnityEngine;

public class HP : MonoBehaviour, IDamagable
{
    [SerializeField] public float maxLife;
    [SerializeField] private float currentLife;

    public event Action<float> OnDamageReceived;
    public event Action<float> OnHeal;
    public event Action OnDeath;

    public float GetLife
    {
        get { return currentLife; }
        set { currentLife = Mathf.Clamp(value, 0, maxLife); }
    }

    private void Start()
    {
        currentLife = maxLife;
    }

    public void ReciveDamage(float damage)
    {
        GetLife -= damage;
        OnDamageReceived?.Invoke(damage);

        if (GetLife <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Health(float amount)
    {
        GetLife += amount;
        OnHeal?.Invoke(amount);
    }
}
