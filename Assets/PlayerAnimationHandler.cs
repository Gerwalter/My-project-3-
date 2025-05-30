using System;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour, IAnimObserver
{
    [SerializeField] private Animator animator;

    public void OnAttackTriggered(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
    private void Start()
    {
        EventManager.Subscribe("Input", PlayerInput);
        EventManager.Subscribe("Float", PlayerFloat);
        EventManager.Subscribe("Bool", PlayerBool);

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
