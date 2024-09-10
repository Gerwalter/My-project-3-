using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : Jump
{
    [Header("References")]
    public float walking_speed;
    public Transform player_Transform;

    [Header("Dash")]
    public float dashForce = 10.0f;
    public float dashCooldown = 2.0f;
    public float dashDuration = 0.2f;
    private float lastDashTimeForward = -Mathf.Infinity;
    private bool isDashingForward = false;

    private float verticalInput;
    private float horizontalInput;

    public DashCooldownUI dashCooldownUI;
    public Animator animator;

    private void FixedUpdate()
    {
        if (isDashingForward) return;

        MovementPlayer();
        Dash();
    }

    private void MovementPlayer()
    {
      
        if (IsAnyComboAnimationPlaying())
        {
            return;
        }

        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 movement = (transform.forward * verticalInput + transform.right * horizontalInput) * walking_speed * Time.deltaTime;
        movement.y = rb.velocity.y;
        rb.velocity = movement;
    }

    private void Dash()
    {
        
        if (IsAnyComboAnimationPlaying())
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Time.time >= lastDashTimeForward + dashCooldown)
        {
            StartCoroutine(PerformForwardDash());
        }
    }

    private IEnumerator PerformForwardDash()
    {
        isDashingForward = true;
        lastDashTimeForward = Time.time;

        Vector3 dashDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector3.zero;

        isDashingForward = false;

        dashCooldownUI.StartCooldown();
    }

    private bool IsAnyComboAnimationPlaying()
    {
        return IsAnimationPlaying("Sword And Shield Slash") || IsAnimationPlaying("Sword And Shield Attack") || IsAnimationPlaying("Sword And Shield Finisher");
    }

    private bool IsAnimationPlaying(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }
}
