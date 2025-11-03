using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ejemplo simple: al apretar el botón left mouse el tip se lanza hacia adelante con una velocidad, y luego se retrae.
// Ajusta a tu sistema de combate / entrada.
public class WhipTipController : MonoBehaviour
{
    public Transform origin;         // punto de donde parte el lanzamiento (ej: mano del personaje)
    public Vector3 launchDirection = Vector3.forward;
    public float launchSpeed = 25f;
    public float retractSpeed = 40f;
    public float maxDistance = 6f;
    public bool usePhysics = false;  // si quieres usar rigidbody para colisiones reales
    public Rigidbody rb;

    Vector3 startPos;
    bool isLaunched = false;
    bool isRetracting = false;
    Vector3 currentVelocity;

    void Start()
    {
        if (usePhysics && rb == null) rb = GetComponent<Rigidbody>();
        ResetToOrigin();
    }

    void Update()
    {
        // Ejemplo simple de input
        if (Input.GetMouseButtonDown(0) && !isLaunched)
        {
            Launch();
        }

        if (isLaunched && !usePhysics)
        {
            // movimiento manual
            if (!isRetracting)
            {
                transform.position += transform.TransformDirection(launchDirection.normalized) * launchSpeed * Time.deltaTime;

                if (Vector3.Distance(origin.position, transform.position) >= maxDistance)
                    isRetracting = true;
            }
            else
            {
                // retraer hacia origin
                transform.position = Vector3.MoveTowards(transform.position, origin.position, retractSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, origin.position) < 0.05f)
                {
                    ResetToOrigin();
                }
            }
        }
    }

    public void Launch()
    {
        isLaunched = true;
        isRetracting = false;
        startPos = transform.position;

        if (usePhysics && rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = transform.TransformDirection(launchDirection.normalized) * launchSpeed;
        }
    }

    void ResetToOrigin()
    {
        isLaunched = false;
        isRetracting = false;
        transform.position = origin != null ? origin.position : Vector3.zero;
        if (usePhysics && rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // aquí puedes manejar colisiones del tip (daño, golpear enemigos, etc)
        // al chocar, que empiece a retraer
        if (isLaunched)
        {
            isRetracting = true;
            if (usePhysics && rb != null) rb.velocity = Vector3.zero;
        }
    }
}
