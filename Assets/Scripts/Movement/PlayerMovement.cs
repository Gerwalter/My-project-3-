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
    private bool isDashingForward = false;

    private float verticalInput;
    private float horizontalInput;

    public DashCooldownUI dashCooldownUI; // Asigna esto desde el Inspector

    private void FixedUpdate()
    {
        if (isDashingForward) return; // No permitir movimiento durante el dash

        MovementPlayer();
        Dash();
    }

    private void MovementPlayer()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 movement = (transform.forward * verticalInput + transform.right * horizontalInput) * walking_speed * Time.deltaTime;
        movement.y = rb.velocity.y; // Mantener la velocidad vertical existente
        rb.velocity = movement;
    }

    private void Dash()
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

        // Calcula la dirección del dash basada en la dirección de movimiento actual
        Vector3 dashDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector3.zero; // Opcional: Resetea la velocidad después del dash

        isDashingForward = false;

        dashCooldownUI.StartCooldown(); // Inicia el cooldown de la UI
    }
}
