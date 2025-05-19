using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptFollower : MonoBehaviour
{
    public Animator anim;
    [SerializeField] PlayerFollower _player;

    public GameObject sword;

    private void Update()
    {
        Fight();
    }

    void TriggerAnimator(string triggerName)
    {
        if (anim != null)
        {
            anim.SetTrigger(triggerName); // Disparar el trigger
        }
    }
    public void SwordReveal()
    {
        sword.SetActive(!sword.activeSelf);
    }

    public CameraController cameraController;
    private void Start()
    {
        cameraController = CameraController.Instance;
    }
    void Fight()
    {
     //   if (cameraController.IsCameraFixed) return;
        if (Input.GetMouseButtonDown(0)) // Mouse0 para Light Attack
        {
            TriggerAnimator("LightAttack");  // Disparar el trigger LightAttack en el Animator
        }
        if (Input.GetMouseButtonDown(1)) // Mouse1 para Heavy Attack
        {
            TriggerAnimator("HeavyAttack"); // Disparar el trigger HeavyAttack en el Animator
        }
    }

    public void PrintNum()
    {
      //  _player.AnimationMoveImpulse(1f); ;
    }

    public void JumpAttack()
    {
     //   _player.ApplyForwardJumpImpulse(6f, 3f);
    }



    public void Die()
    {
      //  _player.Die();
    }
    public void Jump()
    {
      //  _player.Jump();
    }
    public void DisableMovement()
    {
        _player.Freeze = true;
    }

    public void EnableMovement()
    {
        _player.Freeze = false;
    }

    public void Interact()
    {
      //  _player.Interact();
    }

    public void triggerReset()
    {
        anim.ResetTrigger("Hit");
    }
}
