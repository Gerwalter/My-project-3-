using System;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public float velocity = 0.0f;
    public float minusvelocity = 0.0f;
    public float acceleration = 0.0f;
    public float unacceleration = 0.0f;
    public Animator anim;
    public LayerMask groundLayer;

    public bool isGrounded;

    private void Update()
    {
        Fight();
        FowardMove();
        BackwardsMove();
        HandleJump();
    }

    public void FowardMove()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool pressedMove = verticalInput > 0.0f;

        if (pressedMove && velocity < 1.0f)
        {
            velocity += Time.deltaTime * acceleration;
        }
        else if (!pressedMove && velocity > 0.0f)
        {
            velocity = 0;
        }

        velocity = Mathf.Clamp(velocity, 0.0f, 1.0f);
        anim.SetFloat("Velocidad", velocity);
    }

    public void BackwardsMove()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool pressednegativeMove = verticalInput < 0.0f;

        if (pressednegativeMove && minusvelocity < 1.0f)
        {
            minusvelocity += Time.deltaTime * unacceleration;
        }

        if (!pressednegativeMove && minusvelocity > 0.0f)
        {
            minusvelocity = 0;
        }

        minusvelocity = Mathf.Clamp(minusvelocity, 0.0f, 1.0f);
        anim.SetFloat("-Velocidad", minusvelocity);
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



    [SerializeField] private bool isFighting = false;

    public void Fight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFighting = !isFighting;
            anim.SetBool("Fight", isFighting);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("Attack");
        }

    }
}
