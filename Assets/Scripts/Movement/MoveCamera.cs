using UnityEngine;

public class MouseCameraControl : MonoBehaviour
{

    public float mouseSensitivity = 100f;  // Sensibilidad del ratón
    public Transform playerBody;  // Referencia al jugador
    public Vector3 offset;  // Posición inicial de la cámara respecto al jugador
    public float minDistance = 1.0f;  // Distancia mínima de la cámara al jugador
    public LayerMask collisionMask;  // Máscara de capas para detectar colisiones (evitar objetos específicos si es necesario)

    private float xRotation = 0f;

    void Start()
    {
        // Bloquear el cursor en el centro de la pantalla y ocultarlo
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Obtener la entrada del ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Controlar la rotación vertical (cámara)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limitar la rotación vertical

        // Aplicar la rotación vertical a la cámara (rotación en el eje X)
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y, 0f);

        // Rotar solo la cámara en el eje Y (horizontal)
        transform.Rotate(Vector3.up * mouseX);

        // Calcular la posición deseada de la cámara
        Vector3 desiredPosition = playerBody.position + offset;

        // Realizar un Raycast para detectar si hay objetos entre la cámara y el jugador
        RaycastHit hit;
        if (Physics.Raycast(playerBody.position, (desiredPosition - playerBody.position).normalized, out hit, offset.magnitude, collisionMask))
        {
            // Si el Raycast detecta una colisión, mover la cámara a la posición del impacto, ajustándola para que no atraviese el objeto
            transform.position = hit.point + hit.normal * minDistance;  // Mueve la cámara justo antes del objeto
        }
        else
        {
            // Si no hay colisión, coloca la cámara en su posición deseada
            transform.position = desiredPosition;
        }
    }
}
