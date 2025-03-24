using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public float acceleration = 500f;
    public float maxSpeed = 20f;
    public float turnSpeed = 100f;
    public float brakeForce = 1000f;
    public float handbrakeForce = 5000f;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private bool isBraking;
    private bool isHandbraking;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
        isBraking = Input.GetKey(KeyCode.LeftShift);
        isHandbraking = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        Move();
        Turn();
        ApplyBrakes();
    }

    void Move()
    {
        if (moveInput != 0)
        {
            Vector3 force = transform.forward * moveInput * acceleration * Time.fixedDeltaTime;
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(force, ForceMode.Acceleration);
            }
        }
    }

    void Turn()
    {
        if (turnInput != 0 && rb.velocity.magnitude > 0.1f)
        {
            float turn = turnInput * turnSpeed * Time.fixedDeltaTime * (rb.velocity.magnitude / maxSpeed);
            transform.Rotate(0, turn, 0);
        }
    }

    void ApplyBrakes()
    {
        if (isBraking)
        {
            rb.AddForce(-rb.velocity.normalized * brakeForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        if (isHandbraking)
        {
            rb.drag = 3f; // Aumenta la fricción para simular el freno de mano
        }
        else
        {
            rb.drag = 0.1f;
        }
    }
}
