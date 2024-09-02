using UnityEngine;
using UnityEngine.EventSystems; // Necesario para el manejo de eventos UI

public class AimDetector : MonoBehaviour
{
    public Camera playerCamera;          // La c�mara desde la cual se realizar� el raycast
    public RectTransform crosshair;      // La mira en el canvas

    void Update()
    {
        // Verifica si se ha presionado la tecla Z
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DetectObjectUnderCrosshair();
        }
    }

    void DetectObjectUnderCrosshair()
    {
        if (playerCamera == null || crosshair == null)
        {
            Debug.LogWarning("C�mara o mira no asignada en el script AimDetector.");
            return;
        }

        // Obt�n la posici�n de la mira en el canvas
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(playerCamera, crosshair.position);

        // Realiza un raycast desde la posici�n de la mira en la pantalla
        Ray ray = playerCamera.ScreenPointToRay(screenPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Obt�n el nombre del objeto que se ha impactado
            Debug.Log("Objeto al que se est� apuntando: " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("No se ha detectado ning�n objeto bajo la mira.");
        }
    }
}
