using System.Collections;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public Animator anim;
    public LayerMask groundLayer;
    [SerializeField] private bool NoAttack = false;
    //public GameObject particles;

    public GameObject sword;

    public bool isGrounded;

    public void Start()
    {
        sword.SetActive(false);
    }

    private void Update()
    {
        Fight();
        Cast();
    }

    private void Cast()
    {
        if (Input.GetKeyDown(KeyCode.E))
            anim.SetTrigger("Cast");
    }

    public void CastEnd()
    {
        anim.SetTrigger("Cast");
    }

    public void SwordReveal()
    {
        sword.SetActive(!sword.activeSelf);
    }

    [SerializeField] private bool isFighting = false;
    [SerializeField] private float attackTimeout = 2.0f;
    private Coroutine attackCoroutine;

    public void Fight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFighting = !isFighting;
            anim.SetBool("Fight", isFighting);


            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && isFighting)
        {
            anim.SetBool("NoAttack", false);
            anim.SetTrigger("Attack");


            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
            attackCoroutine = StartCoroutine(AttackTimeout());
        }
    }
    private IEnumerator AttackTimeout()
    {
        yield return new WaitForSeconds(attackTimeout);
        AttackFinished();
        yield return new WaitForSeconds(2f);
    }
    public void AttackFinished()
    {
        NoAttack = !NoAttack;
        anim.SetBool("NoAttack", NoAttack);
    }
}
