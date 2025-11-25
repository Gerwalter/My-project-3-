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

    private bool playerInside = false;
    private Transform player;
    public GrapplingHook grapplingHook;


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

        }
    }

    private void Update()
    {
        if (!playerInside) return;

        // Mientras F está presionada mirar al objetivo
        if (Input.GetKey(KeyCode.G))
        {
            CameraController.Instance._isCameraPointing = true;

            Vector3 dir = (focusTarget.position - CameraController.Instance.transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);

            CameraController.Instance.transform.rotation =
                Quaternion.Lerp(CameraController.Instance.transform.rotation, targetRot, Time.deltaTime * lookSpeed);
        }
        if (Input.GetKeyUp(KeyCode.G) && !Input.GetKey(KeyCode.LeftControl))
        {
            CameraController.Instance._isCameraPointing = false;

            if (grapplingHook != null)
            {
                grapplingHook.StartGrappleToPoint(focusTarget.position);
            }
        }            // Al soltar F disparar gancho

    }

}
