using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.SocialPlatforms.Impl;

public class Water : MonoBehaviour
{
    public GameObject Player;
    public Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisiona es la espada
        if (other.gameObject == Player)
        {
            rb.drag = 3;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            rb.drag = 0;
        }
    }
}
