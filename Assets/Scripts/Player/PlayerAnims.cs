using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ComboSystem;

public class PlayerAnims : Player
{
    public Animator _anim;



    [SerializeField] private GameObject gameObje;

    public override void Health(float amount)
    {
        GetLife += amount;
    }
    public void Start()
    {

    }/*
    public override void ReciveDamage(float damage)
    {
        GetLife -= damage;

        if (isDead) return; // Si ya está muerto, no hacer nada

        if (GetLife <= 0)
        {
            if (_anim != null)
            {
                _anim.SetTrigger("Die");
            }
            isDead = true; // Marcar como muerto
        }
        else
        {
            if (_anim != null)
            {
                _anim.SetTrigger("Hit");
                // _bloodVFX.SendEvent("OnTakeDamage");
            }
        }
    }*/
    
}
