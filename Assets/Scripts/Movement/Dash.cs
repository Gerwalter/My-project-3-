using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash: MonoBehaviour
{
    [Header("References")]
    public Transform player_Transform;

    [Header("Dash")]
    public float dashForce = 10.0f;
    public float dashCooldown = 2.0f;
    public float dashDuration = 0.2f;
    private float lastDashTimeForward = -Mathf.Infinity;
    private bool isDashingForward = false;
    private Rigidbody rb;

    private float verticalInput;
    private float horizontalInput;

    public DashCooldownUI dashCooldownUI;
    public Animator animator;

    private void FixedUpdate()
    {
        if (isDashingForward) return;
        DashFoward();
    }


    private void DashFoward()
    {
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
}
