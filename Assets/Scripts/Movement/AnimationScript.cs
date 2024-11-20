using System.Collections;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public Animator anim;
    [SerializeField] Player _player;
    public GameObject sword;



    private void Update()
    {
        //Fight();
        Cast();
    }

    private void Cast()
    {
        if (Input.GetKeyDown(KeyCode.E))
            anim.SetTrigger("Element");
    }

    public void SwordReveal()
    {
        sword.SetActive(!sword.activeSelf);
    }

    public void PrintNum()
    {
        _player.MovePlayer(1f); ;
    }

    public void JumpAttack()
    {
        _player.ApplyForwardJumpImpulse(6f, 3f);
    }

    public void EnemyLift()
    {
        _player.PerformLiftAttack();
    }

    public void Attack()
    {
        _player.Attack();
    }

    public void Die()
    {
        _player.Die();
    }

    public void DisableMovement()
    {
        _player.DisableMovement();
    }

    public void Interact()
    {
        _player.Interact();
    }

    public void PlayVFX()
    {
        _player.PlayVFX();
    }

    public void PlayVFXAttack()
    {
        _player.PlayVFXAttack();
    }

    public void triggerReset()
    {
        anim.ResetTrigger("Hit");
    }
}
