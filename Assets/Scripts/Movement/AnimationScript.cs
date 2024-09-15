using System.Collections;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public float velocity = 0.0f;
    public float horizontalVelocity = 0.0f;
    public float acceleration = 0.0f;
    public float unacceleration = 0.0f;
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
        HandleJump();
        HandleMovement();
        Cast();
    }

    public void HandleMovement()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput > 0.0f)
        {
            velocity = 1;
        }
        else if (verticalInput < 0.0f)
        {
            velocity = -1;
        }
        else
        {
            velocity = 0;
        }

        anim.SetFloat("xAxis", velocity);
        velocity = Mathf.Clamp(velocity, -1.0f, 1.0f);

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0.0f)
        {
            horizontalVelocity = 1;
        }
        else if (horizontalInput < 0.0f)
        {
            horizontalVelocity = -1;
        }
        else
        {
            horizontalVelocity = 0;
        }

        anim.SetFloat("zAxis", horizontalVelocity);
    }

    private void HandleJump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Jump");

        }

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
    public void OnJumpAnimationEnd()
    {
        anim.SetTrigger("Jump");

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
