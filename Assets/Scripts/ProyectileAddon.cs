using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileAddon : MonoBehaviour
{
    public Rigidbody rb;
    public bool targetHit;
    public Collider playerCollider;  // Asignar el collider del jugador desde el Inspector o desde otro script

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ignorar la colisión entre el proyectil y el jugador
        if (playerCollider != null)
        {
            Collider projectileCollider = GetComponent<Collider>();
            Physics.IgnoreCollision(projectileCollider, playerCollider);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit) return;
        else targetHit = true;

        rb.isKinematic = true;

        transform.SetParent(collision.transform);
    }
}
