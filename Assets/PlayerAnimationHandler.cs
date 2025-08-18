using System;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour, IAnimObserver
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody _rb;
    public void OnAttackTriggered(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
    private void Start()
    {
        EventManager.Subscribe("Input", PlayerInput);
        EventManager.Subscribe("Float", PlayerFloat);
        EventManager.Subscribe("Bool", PlayerBool);

        EventManager.Subscribe("PrintNum", OnAttack);
        EventManager.Subscribe("OnJumpAttack", OnJumpAttack);


    }
    public GameObject observable;
    private void Awake()
    {
        if (observable.GetComponent<IAnimObservable>() != null)
            observable.GetComponent<IAnimObservable>().Subscribe(this);
    }

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
    private void PlayerInput(params object[] args)
    {
        var input = (string)args[0];
        animator.SetTrigger(input);
    }
    private void PlayerFloat(params object[] args)
    {
        var input = (string)args[0];
        var Numb = (float)args[1];
        animator.SetFloat(input, Numb);
    }

    private void PlayerBool(params object[] args)
    {
        var input = (string)args[0];
        var Numb = (bool)args[1];
        animator.SetBool(input, Numb);
    }
    public void OnShootStateChanged(bool isShooting)
    {
        animator.SetBool("Shoot", isShooting);
    }
}
