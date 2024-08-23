using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public int jump = 500;
    public Rigidbody rb;
    public bool isGrounded;
    public AnimationScript anim;
    public LayerMask groundLayer;

    private void Update()
    {
        JumpMovement();
    }

    private void JumpMovement()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector3(0, jump, 0), ForceMode.Impulse);
            isGrounded = false; // Para prevenir múltiples saltos en el aire
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Comprueba si el objeto con el que colisionas pertenece a la capa "Ground"
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Verifica si dejaste de tocar el suelo
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = false;
        }
    }
}
