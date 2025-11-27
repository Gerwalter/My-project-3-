using System;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour, IAnimObserver
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerController player;
    [SerializeField] private float _fadeDuration = 0.15f;
    [SerializeField] private GameObject observable;

    private void Awake()
    {
        // Asegurar que haya referencia al PlayerController
        if (player == null)
            player = GetComponent<PlayerController>();

        // Asegurar que haya referencia al Rigidbody
        if (_rb == null)
            _rb = player != null ? player.Rigidbody : GetComponent<Rigidbody>();

        // Suscripción al observable si existe
        if (observable != null)
        {
            var animObservable = observable.GetComponent<IAnimObservable>();
            if (animObservable != null)
                animObservable.Subscribe(this);
        }
        else
        {
            Debug.LogWarning("No se asignó observable en PlayerAnimationHandler.");
        }
    }

    private void Start()
    {
        // Subscribirse a eventos del EventManager
        EventManager.Subscribe("Input", PlayerInput);
        EventManager.Subscribe("Float", PlayerFloat);
        EventManager.Subscribe("Bool", PlayerBool);
        EventManager.Subscribe("PrintNum", OnAttack);
        EventManager.Subscribe("OnJumpAttack", OnJumpAttack);

        // Reconfirmar referencias en caso de recarga de escena
        if (player == null)
            player = GetComponent<PlayerController>();
        if (_rb == null && player != null)
            _rb = player.Rigidbody;
    }

    public void OnAttackTriggered(ComboNode node)
    {
        if (node != null && node.animationClip != null)
        {
            int attackLayerIndex = animator.GetLayerIndex("AttackLayer");
            animator.CrossFade(node.animationClip.name, _fadeDuration, attackLayerIndex, 0f);
        }
        else
        {
            Debug.LogWarning("El nodo no tiene animación asignada.");
        }
    }

    private void OnAttack(params object[] args)
    {
        float forward = (float)args[0];
        AnimationMoveImpulse(forward);
    }

    private void OnJumpAttack(params object[] args)
    {
        float forward = (float)args[0];
        float up = (float)args[1];
        ApplyForwardJumpImpulse(forward, up);
    }

    public void AnimationMoveImpulse(float force)
    {
        if (_rb == null)
        {
            Debug.LogError("Rigidbody no asignado en PlayerAnimationHandler.");
            return;
        }
        Vector3 forwardDirection = transform.forward;
        _rb.AddForce(forwardDirection * force, ForceMode.Impulse);
    }

    public void ApplyForwardJumpImpulse(float forwardForce, float jumpForce)
    {
        if (_rb == null)
        {
            Debug.LogError("Rigidbody no asignado en PlayerAnimationHandler.");
            return;
        }

        Vector3 forwardDirection = transform.forward * forwardForce;
        Vector3 upwardImpulse = Vector3.up * jumpForce;
        _rb.AddForce(forwardDirection + upwardImpulse, ForceMode.Impulse);
    }

    private void PlayerInput(params object[] args)
    {
        animator.SetTrigger((string)args[0]);
    }

    private void PlayerFloat(params object[] args)
    {
        animator.SetFloat((string)args[0], (float)args[1]);
    }

    private void PlayerBool(params object[] args)
    {
        animator.SetBool((string)args[0], (bool)args[1]);
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
