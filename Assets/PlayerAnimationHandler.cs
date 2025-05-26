using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour, IAnimObserver
{
    [SerializeField] private Animator animator;

    public void OnAttackTriggered(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void OnShootStateChanged(bool isShooting)
    {
        animator.SetBool("Shoot", isShooting);
    }
}
