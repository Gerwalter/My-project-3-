using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float rotationSpeed = 5.0f;
    public float smoothSpeed = 0.125f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("No se ha asignado un objetivo para la c�mara.");
        }

        // Bloquear el cursor en el centro de la pantalla y ocultarlo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Liberar el cursor al presionar Escape (opcional)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Movimiento con el rat�n
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, -45f, 45f);  // Limitar la rotaci�n vertical para evitar giros extra�os

            // Calcular la rotaci�n
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            // Aplicar el offset y la rotaci�n
            Vector3 desiredPosition = target.position + rotation * offset;
           Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
           transform.position = smoothedPosition;
            //transform.position = desiredPosition;

            // Mirar hacia el objetivo
            transform.LookAt(target.position);
        }
    }
}
