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

        if (verticalInput != 0)
        {
            RotationPlayer();
        }

    }
    private void MovementPlayer()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 movement = transform.forward * verticalInput * walking_speed * Time.deltaTime;
        movement.y = rb.velocity.y; // Mantener la velocidad vertical existente

        rb.velocity = movement;
    }

    private void RotationPlayer()
    {
        float rotation = horizontalInput * rotate_speed * Time.deltaTime;
        player_Transform.Rotate(0, rotation, 0);
    }
}
