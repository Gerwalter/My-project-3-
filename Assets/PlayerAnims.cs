using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ComboSystem;

public class PlayerAnims : Player, IAnimObserver
{
    public override void Die()
    {
        gameObje.SetActive(false);
    }


    [SerializeField] private Rigidbody _rb;

    [SerializeField] private GameObject gameObje;
    private bool isDead = false;

    public override void Health(float amount)
    {
        GetLife += amount;
    }

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
    }

    public override void AnimationMoveImpulse(float force)
    {
        Vector3 forwardDirection = transform.forward; // Dirección actual del jugador
        _rb.AddForce(forwardDirection * force, ForceMode.Impulse);
    }

    public override void ApplyForwardJumpImpulse(float forwardForce, float jumpForce)
    {
        // Dirección hacia adelante basada en la orientación actual del jugador
        Vector3 forwardDirection = transform.forward * forwardForce;

        // Impulso en el eje Y para el salto
        Vector3 upwardImpulse = Vector3.up * jumpForce;

        // Aplica ambas fuerzas al Rigidbody
        _rb.AddForce(forwardDirection + upwardImpulse, ForceMode.Impulse);
    }
    public GameObject observable;
    private void Awake()
    {
        if (observable.GetComponent<IAnimObservable>() != null)
            observable.GetComponent<IAnimObservable>().Subscribe(this);
    }
    public void Notify(string Input, bool Value)
    {
        _anim.SetBool(Input, Value);
    }

    public void Notify(string Input)
    {
        _anim.SetTrigger(Input);
    }
}
