using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportFocusTrigger : MonoBehaviour
{

    [Header("Teleport Settings")]
    public Transform teleportDestination;

    [Header("Camera Focus Settings")]
    public Transform focusTarget;
    public float lookSpeed = 5f;

    [Header("Grapple Line Settings")]
    public LineRenderer lineRenderer;    // ← Asignar un LineRenderer en el inspector
    public float hookDelay = 1f;         // Tiempo antes de mover al jugador

    private bool playerInside = false;
    private Transform player;
    public Transform _hookOrigin;
    private bool hookActive = false;

    private void Start()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false; // Ocultar línea al inicio
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            CameraController.Instance._isCameraPointing = false;

            if (lineRenderer != null)
                lineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (!playerInside) return;

        // Mientras F está presionada mirar al objetivo
        if (Input.GetKey(KeyCode.F))
        {
            CameraController.Instance._isCameraPointing = true;

            Vector3 dir = (focusTarget.position - CameraController.Instance.transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);

            CameraController.Instance.transform.rotation =
                Quaternion.Lerp(CameraController.Instance.transform.rotation, targetRot, Time.deltaTime * lookSpeed);
        }

        // Al soltar F disparar gancho
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (!hookActive)
            {
                StartCoroutine(GrappleAndTeleport());
            }
        }
    }

    private IEnumerator GrappleAndTeleport()
    {
        hookActive = true;

        CameraController.Instance._isCameraPointing = false;

        // Activar LineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
        }

        // Tiempo que tardará en llegar (más grande = más lento)
        float travelTime = 0.6f;
        float timer = 0f;

        Vector3 startPos = player.position;
        Vector3 endPos = teleportDestination.position;

        while (timer < travelTime)
        {
            timer += Time.deltaTime;
            float t = timer / travelTime;

            // Curva de aceleración → hace más natural el movimiento
            t = Mathf.SmoothStep(0f, 1f, t);

            // Interpolación del movimiento
            player.position = Vector3.Lerp(startPos, endPos, t);

            // Actualizar línea del gancho
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, _hookOrigin.position);
                lineRenderer.SetPosition(1, focusTarget.position);
            }

            yield return null;
        }

        // Asegurar posición exacta
        player.position = endPos;

        // Ocultar línea
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        hookActive = false;
    }
}
