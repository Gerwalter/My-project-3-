using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public float velocity = 0.0f;
    public float horizontalVelocity = 0.0f;
    public float acceleration = 0.0f;
    public float unacceleration = 0.0f;
    public Animator anim;
    public LayerMask groundLayer;

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

        anim.SetFloat("Velocidad", velocity);
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

        anim.SetFloat("-Velocidad", horizontalVelocity);
    }

    private void HandleJump()
    {
        // Comprueba si el jugador está en el suelo usando un Raycast hacia abajo
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        // Si está en el suelo y se presiona la barra espaciadora, activa el trigger "Jump"
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Jump");
        }

    }

    public void OnAnimationJumpStart()
    {

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

    public void Fight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFighting = !isFighting;
            anim.SetBool("Fight", isFighting);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && isFighting)
        {
            anim.SetTrigger("Attack");
        }

    }
}
