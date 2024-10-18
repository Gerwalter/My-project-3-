using System.Collections;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public Animator anim;
    public LayerMask groundLayer;
    [SerializeField] private bool NoAttack = false;
    [SerializeField] Player _player;
    //public GameObject particles;

    public GameObject sword;

    public bool isGrounded;

    [Header("<color=yellow>Attack</color>")]
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private float _atkRayDist = 1.0f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private int _atkDmg = 20; 
    

    private Ray _atkRay;
    private RaycastHit _atkHit;

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
    public void Attack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.ReciveDamage(_atkDmg);
            }
        }
    }

    public void Die()
    {
        _player.Die();
    }

    public void Interact()
    {
        _player.Interact();
    }
    private IEnumerator AttackTimeout()
    {
        yield return new WaitForSeconds(attackTimeout);
        AttackFinished();
        yield return new WaitForSeconds(1f);
    }
    public void AttackFinished()
    {
        NoAttack = !NoAttack;
        anim.SetBool("NoAttack", true);
    }

    public void triggerReset()
    {
        anim.ResetTrigger("Hit");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_atkRay);
    }
}
