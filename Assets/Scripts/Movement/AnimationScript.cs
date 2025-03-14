using TMPro.Examples;
using UnityEngine;
using static ComboSystem;

public class AnimationScript : MonoBehaviour
{
    public Animator anim;
    [SerializeField] Player _player;
    public GameObject sword;

    private void Update()
    {
        Fight();
        Cast();
    }

    private void Cast()
    {
        if (Input.GetKeyDown(KeyCode.E))
            anim.SetTrigger("Element");
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
