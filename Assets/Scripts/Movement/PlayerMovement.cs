using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float walking_speed, rotate_speed;
    public Transform player_Transform;
    public AnimationScript anim;

    [Header("Dash")]
    public float dashForce = 10.0f;
    public float dashCooldown = 2.0f;
    public float dashDuration = 0.2f;
    private float lastDashTimeForward = -Mathf.Infinity;
    private float lastDashTimeBackward = -Mathf.Infinity;
    private bool isDashingForward = false;
    private bool isDashingBackward = false;

    private float verticalInput;
    private float horizontalInput;

    private void FixedUpdate()
    {
        if (isDashingForward || isDashingBackward) return; // No permitir movimiento durante el dash

        MovementPlayer();

        if (verticalInput != 0)
        {
            RotationPlayer();
        }
    }

    private void MovementPlayer()
    {
        verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput > 0)
        {
            Vector3 movement = transform.forward * verticalInput * walking_speed * Time.deltaTime;
            movement.y = rb.velocity.y; // Mantener la velocidad vertical existente
            rb.velocity = movement;

            if (Input.GetKey(KeyCode.LeftShift) && Time.time >= lastDashTimeForward + dashCooldown)
            {
                StartCoroutine(PerformForwardDash());
            }
        }
        else if (verticalInput < 0)
        {
            Vector3 movement = -transform.forward * -verticalInput * walking_speed * Time.deltaTime;
            movement.y = rb.velocity.y; // Mantener la velocidad vertical existente
            rb.velocity = movement;

            if (Input.GetKey(KeyCode.LeftShift) && Time.time >= lastDashTimeBackward + dashCooldown)
            {
                StartCoroutine(PerformBackwardDash());
            }
        }
        else
        {
            Vector3 movement = rb.velocity;
            movement.x = 0;
            movement.z = 0;
            rb.velocity = movement; // Mantener solo la velocidad vertical
        }
    }

    private void RotationPlayer()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        float rotation = horizontalInput * rotate_speed * Time.deltaTime;
        player_Transform.Rotate(0, rotation, 0);
    }

    public DashCooldownUI dashCooldownUI; // Asigna esto desde el Inspector

    private IEnumerator PerformForwardDash()
    {
        isDashingForward = true;
        lastDashTimeForward = Time.time;

        // Aplicar una fuerza al Rigidbody para simular el dash
        Vector3 dashDirection = transform.forward;
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector3.zero; // Opcional: Resetea la velocidad después del dash

        isDashingForward = false;

        dashCooldownUI.StartCooldown(); // Inicia el cooldown de la UI
    }

    private IEnumerator PerformBackwardDash()
    {
        isDashingBackward = true;
        lastDashTimeBackward = Time.time;

        // Aplicar una fuerza al Rigidbody para simular el dash
        Vector3 dashDirection = -transform.forward;
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector3.zero; // Opcional: Resetea la velocidad después del dash

        isDashingBackward = false;

        dashCooldownUI.StartCooldown(); // Inicia el cooldown de la UI
    }
}
