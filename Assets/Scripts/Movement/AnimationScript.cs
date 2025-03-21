using TMPro.Examples;
using UnityEngine;
using static ComboSystem;

public class AnimationScript : MonoBehaviour
{
    public Animator anim;
    [SerializeField] Player _player;
    [SerializeField] PlayerAttack _playerAttack;
    public GameObject sword;

    private void Update()
    {
        Fight();
        Cast();
    }

    private void Cast()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            anim.SetTrigger("Cast");
            _playerAttack.Cast();
        }

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
        if (cameraController.IsCameraFixed) return;
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
        _player.AnimationMoveImpulse(1f); ;
    }

    public void JumpAttack()
    {
        _player.ApplyForwardJumpImpulse(6f, 3f);
    }

    public void EnemyLift()
    {
        _playerAttack.PerformLiftAttack();
    }

    public void Attack()
    {
        _playerAttack.Attack();
    }

    public void Die()
    {
        _player.Die();
    }
    public void Jump()
    {
        _player.Jump();
    }
    public void DisableMovement()
    {
        _player.DisableMovement();
    }

    public void EnableMovement()
    {
        _player.EnableMovement();
    }

    public void Interact()
    {
        _player.Interact();
    }

    public void PlayVFX()
    {
        _playerAttack.PlayVFX();
    }

    public void PlayVFXAttack()
    {
        _playerAttack.PlayVFXAttack();
    }

    public void triggerReset()
    {
        anim.ResetTrigger("Hit");
    }
}
