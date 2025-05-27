using UnityEngine;
using UnityEngine.UI;

public class Player : HP
{
    public Animator _anim;
    [SerializeField] public bool freeze = false;
    private void Awake()
    {
        // GameManager.Instance.Player = this;

    }
    public virtual void AnimationMoveImpulse(float force)
    {
       
    }

    public virtual void ApplyForwardJumpImpulse(float forwardForce, float jumpForce)
    {
       
    }
    public virtual void Jump()
    {
       
    }

    public virtual void Attack()
    {
       
    }
    public virtual void Die()
    {
    }
    public virtual void Interact() 
    {
       
    }
    public virtual void PlayVFX()
    {
      
    }
    public virtual void PlayVFXAttack() 
    {
       
    }
    public virtual void Cast() 
    {
        
    }
    public virtual void PerformLiftAttack() 
    {
       
    }
}
