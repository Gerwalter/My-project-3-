using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour, IDamagable
{
    [SerializeField] public float maxLife;
    [SerializeField] private float currentLife;

    public float GetLife
    {
        get { return currentLife; }
        set { currentLife = Mathf.Clamp(value, 0, maxLife); }
    }


    public void ReciveDamage(float damage)
    {
        GetLife -= damage;


        if (GetLife <= 0)
        {
            Die();
        }
    }

    public void Health(float amount)
    {

        GetLife += amount;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
