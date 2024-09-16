using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 25f;
    public float runSpeedMultiplier = 2f;
    public float jumpForce = 10f;
    public float rotationSpeed = 60f;
    public float acceleration = 5f;
    public float deceleration = 10f;
    public float gravityMultiplier = 2f;

    public Transform cameraTransform;

    private Rigidbody rb;
    private bool isGrounded;
    private PlayerInputs playerInputs;
    private bool isRunning;

    public event Action OnTurboActivated;
    public event Action OnTurboDeactivated;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInputs = GetComponent<PlayerInputs>();
        rb.useGravity = false; // Desactiva la gravedad por defecto y usa tu propia gravedad
        OnTurboActivated += EnableTankControl;
        OnTurboDeactivated += DisableTankControl;

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Usa detección continua
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyCustomGravity();
        HandleJump();
    }

    private void CheckForTurbo()
    {
        if (playerInputs.run && !isRunning)
        {
            isRunning = true;
            OnTurboActivated?.Invoke();
        }
        else if (!playerInputs.run && isRunning)
        {
            isRunning = false;
            OnTurboDeactivated?.Invoke();
        }
    }

    private void HandleMovement()
    {
        CheckForTurbo();
        float moveSpeed = isRunning ? maxSpeed * runSpeedMultiplier : maxSpeed;
        Vector3 moveDirection = Vector3.zero;

        if (playerInputs.forwardInput != 0 || playerInputs.rotationInput != 0)
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            Vector3 right = cameraTransform.right;
            right.y = 0;

            moveDirection = (forward * playerInputs.forwardInput + right * playerInputs.rotationInput).normalized;
        }

        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            // Ajustar solo la velocidad horizontal, mantener la velocidad vertical intacta
            Vector3 horizontalVelocity = moveDirection * moveSpeed;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }
        else
        {
            // Frenado suave cuando no se presionan las teclas de movimiento
            Vector3 velocity = rb.velocity;
            velocity.x = Mathf.Lerp(velocity.x, 0, deceleration * Time.fixedDeltaTime);
            velocity.z = Mathf.Lerp(velocity.z, 0, deceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && playerInputs.jump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void ApplyCustomGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    private void EnableTankControl()
    {
        Debug.Log("Modo turbo activado: control de tanque habilitado.");
    }

    private void DisableTankControl()
    {
        Debug.Log("Modo turbo desactivado: control de tanque deshabilitado.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0 && collision.contacts[0].normal == Vector3.up)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Verifica si hay contactos antes de intentar acceder a ellos
        if (collision.contacts.Length > 0 && collision.contacts[0].normal == Vector3.up)
        {
            isGrounded = false;
        }
    }
}
