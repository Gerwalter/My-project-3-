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
    }

    private void Cast()
    {
        if (Input.GetKeyDown(KeyCode.E))
            anim.SetTrigger("Cast");
    }

    public void CastEnd()
    {

        anim.ResetTrigger("Cast");
    }

    public void SwordReveal()
    {
        sword.SetActive(!sword.activeSelf);
    }


  //  public void Fight()
  //  {
  //      if (Input.GetKeyDown(KeyCode.F))
  //      {
  //          isFighting = !isFighting;
  //          anim.SetBool("Fight", isFighting);        
  //      }
  //  }
    public void Attack()
    {
        _player.Attack();
    }

    public void Die()
    {
        _player.Die();
    }

    public void Interact()
    {
        _player.Interact();
    }


    public void triggerReset()
    {
        anim.ResetTrigger("Hit");
    }
}
