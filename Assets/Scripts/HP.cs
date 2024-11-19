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
    private bool isDead = false;
    public void ReciveDamage(float damage)
    {
        GetLife -= damage;

        if (isDead) return; // Si ya está muerto, no hacer nada

        if (GetLife <= 0)
        {
            if (anim != null)
            {
                anim.SetTrigger("Die");
            }
            isDead = true; // Marcar como muerto
        }
        else {
            if (anim != null)
            {
                anim.SetTrigger("Hit");
            }
        }

    }

    public void Health(float amount)
    {

        GetLife += amount;
    }


}
