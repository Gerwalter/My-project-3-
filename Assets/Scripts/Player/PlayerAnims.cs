using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ComboSystem;

public class PlayerAnims : Player, IAnimObserver
{
    public Animator _anim;

    [SerializeField] private Rigidbody _rb;

    [SerializeField] private GameObject gameObje;
    private bool isDead = false;

    public override void Health(float amount)
    {
        GetLife += amount;
    }
    public void Start()
    {
        EventManager.Subscribe("PrintNum", OnAttack);
        EventManager.Subscribe("OnJumpAttack", OnJumpAttack);

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
    void OnAttack(params object[] args)
    {
        float forward = (float)args[0];
        AnimationMoveImpulse(forward); // método original
    }

    void OnJumpAttack(params object[] args)
    {
        float forward = (float)args[0];
        float up = (float)args[1];
        ApplyForwardJumpImpulse(forward, up);
    }
    public void AnimationMoveImpulse(float force)
    {
        Vector3 forwardDirection = transform.forward; // Dirección actual del jugador
        _rb.AddForce(forwardDirection * force, ForceMode.Impulse);
    }

    public void ApplyForwardJumpImpulse(float forwardForce, float jumpForce)
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
    public void OnAttackTriggered(string triggerName)
    {
        _anim.SetTrigger(triggerName);
    }

    public void OnShootStateChanged(bool isShooting)
    {
        _anim.SetBool("Shoot", isShooting);
    }
}
