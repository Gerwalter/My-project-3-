using UnityEngine;

public class PointerCamera : MonoBehaviour
{
    [Header("Layer a detectar")]
    public LayerMask detectionLayer;

    [Header("Distancia del rayo")]
    public float rayDistance = 100f;

    void Update()
    {
        // Creamos un rayo desde la posición de la cámara hacia adelante
        Ray ray = new Ray(transform.position, transform.forward);

        // Si golpea algo en nuestra capa específica
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, detectionLayer))
        {
            Debug.Log("Objeto detectado: " + hit.collider.name);
        }

        // (Opcional) Visualizar el rayo en la escena
        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.red);
    }
}
