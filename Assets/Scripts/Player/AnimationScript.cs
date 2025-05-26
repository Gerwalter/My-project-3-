using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public Animator anim;
    [SerializeField] private List<Player> _players = new List<Player>();


    private void Cast()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CallPlayers(p => p.Cast());
        }

    }
    private void CallPlayers(System.Action<Player> action)
    {
        foreach (var player in _players)
        {
            action(player);
        }
    }
    void TriggerAnimator(string triggerName)
    {
        if (anim != null)
        {
            anim.SetTrigger(triggerName); // Disparar el trigger
        }
    }

    public void PrintNum() => CallPlayers(p => p.AnimationMoveImpulse(1f));
    public void JumpAttack() => CallPlayers(p => p.ApplyForwardJumpImpulse(6f, 3f));
    public void EnemyLift() => CallPlayers(p => p.PerformLiftAttack());
    public void Attack() => CallPlayers(p => p.Attack());
    public void Die() => CallPlayers(p => p.Die());
    public void Jump() => CallPlayers(p => p.Jump());
    public void DisableMovement() => CallPlayers(p => p.DisableMovement());
    public void EnableMovement() => CallPlayers(p => p.EnableMovement());
    public void Interact() => CallPlayers(p => p.Interact());
    public void PlayVFX() => CallPlayers(p => p.PlayVFX());
    public void PlayVFXAttack() => CallPlayers(p => p.PlayVFXAttack());
    public void triggerReset() => anim.ResetTrigger("Hit");
}
