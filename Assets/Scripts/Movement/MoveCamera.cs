using UnityEngine;

public class MouseCameraControl : MonoBehaviour
{

    public float mouseSensitivity = 100f;  // Sensibilidad del rat�n
    public Transform playerBody;  // Referencia al jugador
    public Vector3 offset;  // Posici�n inicial de la c�mara respecto al jugador
    public float minDistance = 1.0f;  // Distancia m�nima de la c�mara al jugador
    public LayerMask collisionMask;  // M�scara de capas para detectar colisiones (evitar objetos espec�ficos si es necesario)

    private float xRotation = 0f;

    void Start()
    {
        // Bloquear el cursor en el centro de la pantalla y ocultarlo
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Obtener la entrada del rat�n
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Controlar la rotaci�n vertical (c�mara)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limitar la rotaci�n vertical

        // Aplicar la rotaci�n vertical a la c�mara (rotaci�n en el eje X)
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y, 0f);

        // Rotar solo la c�mara en el eje Y (horizontal)
        transform.Rotate(Vector3.up * mouseX);

        // Calcular la posici�n deseada de la c�mara
        Vector3 desiredPosition = playerBody.position + offset;

        // Realizar un Raycast para detectar si hay objetos entre la c�mara y el jugador
        RaycastHit hit;
        if (Physics.Raycast(playerBody.position, (desiredPosition - playerBody.position).normalized, out hit, offset.magnitude, collisionMask))
        {
            // Si el Raycast detecta una colisi�n, mover la c�mara a la posici�n del impacto, ajust�ndola para que no atraviese el objeto
            transform.position = hit.point + hit.normal * minDistance;  // Mueve la c�mara justo antes del objeto
        }
        else
        {
            // Si no hay colisi�n, coloca la c�mara en su posici�n deseada
            transform.position = desiredPosition;
        }
    }
}
