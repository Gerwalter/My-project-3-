using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
   [SerializeField] public VisualEffect _bloodVFX;
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
               // _bloodVFX.SendEvent("OnTakeDamage");
            }
        }

    }

    public void Health(float amount)
    {

        GetLife += amount;
    }


}
