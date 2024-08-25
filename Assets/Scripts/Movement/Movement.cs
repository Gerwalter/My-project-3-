using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody rb;
    public float walking_speed, rotate_speed;
    public Transform player_Transform;
    public AnimationScript anim;

    private float verticalInput;
    private float horizontalInput;

    private void FixedUpdate()
    {
        MovementPlayer();
    }

    private void MovementPlayer()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 movement = (transform.forward * verticalInput + transform.right * horizontalInput) * walking_speed * Time.deltaTime;
        movement.y = rb.velocity.y; // Mantener la velocidad vertical existente
        rb.velocity = movement;
    }
}
