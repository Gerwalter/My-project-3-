using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;              // Referencia al transform del jugador
    public Vector3 offset;                // Offset para la posici�n de la c�mara
    public float sensitivity = 5f;        // Sensibilidad del mouse
    public float fastSpeedX = 10f;        // Velocidad r�pida cuando el cursor est� en el borde para el eje X
    public float slowSpeedX = 2f;         // Velocidad lenta cuando el cursor est� en el centro para el eje X
    public float fastSpeedY = 7f;         // Velocidad r�pida cuando el cursor est� en el borde para el eje Y
    public float slowSpeedY = 1.5f;       // Velocidad lenta cuando el cursor est� en el centro para el eje Y
    public float xBorder = 0.1f;          // Distancia al borde para el eje X
    public float yBorder = 0.05f;         // Distancia al borde para el eje Y
    public LayerMask wallMask;            // Capa de las paredes u obst�culos
    public float cameraCollisionRadius = 0.2f;  // Radio del "colisionador" de la c�mara

    private float mouseX;
    private float mouseY;
    private float initialCameraY; // Guarda la posici�n Y inicial de la c�mara

    void Start()
    {
        // Inicializa la posici�n Y de la c�mara
        initialCameraY = transform.position.y;
    }

    void Update()
    {
        // Obtener la posici�n del rat�n en relaci�n a la pantalla
        float mousePosX = Input.mousePosition.x / Screen.width;
        float mousePosY = Input.mousePosition.y / Screen.height;

        // Calcular la velocidad basada en la proximidad a los bordes para el eje X
        float speedX = (mousePosX < xBorder || mousePosX > (1 - xBorder)) ? fastSpeedX : slowSpeedX;

        // Calcular la velocidad basada en la proximidad a los bordes para el eje Y
        float speedY = (mousePosY < yBorder || mousePosY > (1 - yBorder)) ? fastSpeedY : slowSpeedY;

        // Calcular movimiento del rat�n solo si est� fuera del rango muerto
        float mouseMovementX = (mousePosX < xBorder || mousePosX > (1 - xBorder)) ? Input.GetAxis("Mouse X") : 0;
        float mouseMovementY = (mousePosY < yBorder || mousePosY > (1 - yBorder)) ? Input.GetAxis("Mouse Y") : 0;

        mouseX += mouseMovementX * speedX * sensitivity;
        mouseY -= mouseMovementY * speedY * sensitivity; // Restar para que el movimiento del mouse hacia arriba sea hacia arriba en la c�mara

        // Limitar el movimiento vertical de la c�mara
        mouseY = Mathf.Clamp(mouseY, -80f, 80f); // Ajusta los l�mites verticales seg�n tus necesidades

        // Rotar alrededor del jugador y ajustar la rotaci�n vertical
        Quaternion horizontalRotation = Quaternion.Euler(0, mouseX, 0);
        Quaternion verticalRotation = Quaternion.Euler(mouseY, 0, 0);
        Quaternion combinedRotation = horizontalRotation * verticalRotation;

        // Calcular la posici�n deseada de la c�mara
        Vector3 desiredCameraPosition = player.position + combinedRotation * offset;

        // Realizar raycast entre el jugador y la c�mara para detectar paredes
        RaycastHit hit;
        Vector3 directionToCamera = (desiredCameraPosition - player.position).normalized;
        float distanceToCamera = Vector3.Distance(player.position, desiredCameraPosition);

        // Ajustar la posici�n de la c�mara para mantenerla arriba del suelo
        Vector3 clampedCameraPosition = new Vector3(
            desiredCameraPosition.x,
            Mathf.Max(desiredCameraPosition.y, player.position.y + 1f),
            desiredCameraPosition.z
        );

        if (Physics.SphereCast(player.position, cameraCollisionRadius, directionToCamera, out hit, distanceToCamera, wallMask))
        {
            // Ajustar la posici�n de la c�mara al punto de colisi�n
            transform.position = hit.point - directionToCamera * cameraCollisionRadius;
        }
        else
        {
            // Sin colisi�n, usar la posici�n ajustada
            transform.position = clampedCameraPosition;
        }

        // Hacer que la c�mara mire al jugador
        transform.LookAt(player.position + new Vector3(0, offset.y, 0));
    }
}
