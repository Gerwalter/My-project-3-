using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockProyectile : MonoBehaviour
{
    [Header("Configuración del proyectil")]
    [Tooltip("Fuerza con la que se lanza la piedra")]
    public float throwForce = 10f;
    [Tooltip("Tag de los objetos con los que debe activar el sonido al impactar")]
    public string groundTag = "Ground";
    [Tooltip("Tiempo antes de destruir la piedra después del impacto")]
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
        // Lanza hacia adelante si se instancia con orientación del jugador
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Evita múltiples activaciones
        if (hasCollided) return;
        hasCollided = true;

        // Opcional: verificar si golpeó el suelo o una superficie válida
        if (collision.gameObject.CompareTag(groundTag))
        {
            soundEmitter.Emit(); //  Emitir distracción
        }
        else
        {
            // Si no tiene tag "Ground", igual puede emitir sonido más débil
            soundEmitter.Emit();
        }


        // Si no quieres que se destruya, comenta esta línea
        Destroy(gameObject, destroyDelay);
    }
}
