using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public void PrintNum() => EventManager.Trigger("PrintNum", 1f);
    public void JumpAttack() => EventManager.Trigger("OnJumpAttack", 6f, 3f);
    public void EnemyLift() => EventManager.Trigger("OnEnemyLift");
    public void Die() => EventManager.Trigger("OnDie");
    public void Jump() => EventManager.Trigger("OnJump");
    public void Attack() => EventManager.Trigger("OnAttack");
    public void DisableHeavyAttack() => EventManager.Trigger("Bool", "HeavyAttack", false);
    public void DisableLightAttack() => EventManager.Trigger("Bool", "LightAttack", false);
    public void EnableHeavyAttack() => EventManager.Trigger("Bool", "HeavyAttack", true);
    public void EnableLightAttack() => EventManager.Trigger("Bool", "LightAttack", true);
    public void Interact() => EventManager.Trigger("OnInteract");
    public void PlayVFX() => EventManager.Trigger("OnPlayVFX");
    public void PlayVFXAttack() => EventManager.Trigger("OnPlayVFXAttack");
}
