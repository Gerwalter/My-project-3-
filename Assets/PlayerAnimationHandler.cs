using System;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour, IAnimObserver
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _fadeDuration;
    public void OnAttackTriggered(ComboNode node)
    {
        if (node != null && node.animationClip != null)
        {
            // Reproducir el clip directamente en el Animator
            int attackLayerIndex = animator.GetLayerIndex("AttackLayer");
            animator.CrossFade(node.animationClip.name, _fadeDuration, attackLayerIndex, 0f);
        }
        else
        {
            Debug.LogWarning("El nodo no tiene animación asignada.");
        }
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

    private void OnDestroy()
    {
        EventManager.Unsubscribe("Input", PlayerInput);
        EventManager.Unsubscribe("Float", PlayerFloat);
        EventManager.Unsubscribe("Bool", PlayerBool);

        EventManager.Unsubscribe("PrintNum", OnAttack);
        EventManager.Unsubscribe("OnJumpAttack", OnJumpAttack);
    }
}
