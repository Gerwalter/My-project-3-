using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public Animator anim;
    public void PrintNum() => EventManager.Trigger("OnAttack", 1f);
    public void JumpAttack() => EventManager.Trigger("OnJumpAttack", 6f, 3f);
    public void EnemyLift() => EventManager.Trigger("OnEnemyLift");
    public void Die() => EventManager.Trigger("OnDie");
    public void Jump() => EventManager.Trigger("OnJump");
    public void DisableMovement() => EventManager.Trigger("OnDisableMovement");
    public void EnableMovement() => EventManager.Trigger("OnEnableMovement");
    public void Interact() => EventManager.Trigger("OnInteract");
    public void PlayVFX() => EventManager.Trigger("OnPlayVFX");
    public void PlayVFXAttack() => EventManager.Trigger("OnPlayVFXAttack");
    public void triggerReset() => anim.ResetTrigger("Hit");
}
