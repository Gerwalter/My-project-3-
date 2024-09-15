using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public int jump = 500;
    public Rigidbody rb;
    public bool isGrounded;
    public AnimationScript animat;
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
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = true;
            animat.anim.SetBool("isGrounded", isGrounded);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = false;
            animat.anim.SetBool("isGrounded", isGrounded);
        }
    }
}
