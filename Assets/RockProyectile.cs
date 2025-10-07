using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockProyectile : MonoBehaviour
{
    [Header("Configuraci�n del proyectil")]
    [Tooltip("Fuerza con la que se lanza la piedra")]
    public float throwForce = 10f;
    [Tooltip("Tag de los objetos con los que debe activar el sonido al impactar")]
    public string groundTag = "Ground";
    [Tooltip("Tiempo antes de destruir la piedra despu�s del impacto")]
    public float destroyDelay = 1.5f;

    private Rigidbody rb;
    private SoundEmitter soundEmitter;
    private bool hasCollided = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        soundEmitter = GetComponent<SoundEmitter>();
    }

    private void Start()
    {
        // Lanza hacia adelante si se instancia con orientaci�n del jugador
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Evita m�ltiples activaciones
        if (hasCollided) return;
        hasCollided = true;

        // Opcional: verificar si golpe� el suelo o una superficie v�lida
        if (collision.gameObject.CompareTag(groundTag))
        {
            soundEmitter.Emit(); //  Emitir distracci�n
        }
        else
        {
            // Si no tiene tag "Ground", igual puede emitir sonido m�s d�bil
            soundEmitter.Emit();
        }


        // Si no quieres que se destruya, comenta esta l�nea
        Destroy(gameObject, destroyDelay);
    }
}
