using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                // El objetivo que la cámara seguirá
    public float smoothSpeed = 0.125f;      // La velocidad de suavizado del movimiento
    public Vector3 OriginalOffset; // Offset original
    public Vector3 alternateOffset = new Vector3(1.2f, 0.35f, 0.85f); // Offset alternativo

    public float mouseSensitivity = 100f;  // Sensibilidad del ratón
    public Transform playerBody;           // El cuerpo del jugador, para rotar junto con la cámara

    private float xRotation = 0f;          // Rotación en el eje X (vertical)
    private Vector3 currentOffset;         // Offset actual que se aplicará a la cámara
    private bool usingAlternateOffset = false; // Estado para saber si se usa el offset alternativo

    void Start()
    {
        // Inicializar el offset actual con el offset original al iniciar el juego
        currentOffset = OriginalOffset;

        // Bloquear y ocultar el cursor al iniciar el juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Cambiar entre offset original y alternativo al presionar la tecla F
        if (Input.GetKeyDown(KeyCode.F))
        {
            usingAlternateOffset = !usingAlternateOffset; // Alternar el estado
            currentOffset = usingAlternateOffset ? alternateOffset : OriginalOffset; // Cambiar el offset
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target no asignado en el script CameraFollow.");
            return;
        }

        // Mover la cámara hacia la posición deseada suavemente
        Vector3 desiredPosition = target.position + currentOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Rotación de la cámara basada en el movimiento del ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar la rotación vertical para evitar volcarse

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
