using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour, IDamagable
{
    [SerializeField] public float maxLife;
    [SerializeField] private float currentLife;
    [SerializeField] private Animator anim;


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


        if (GetLife <= 0)
        {
            if (anim != null) 
            {
                anim.SetTrigger("Die");
            }

        }
    }

    public void Health(float amount)
    {

        GetLife += amount;
    }


}
