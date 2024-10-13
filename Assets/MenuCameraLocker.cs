using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraLocker : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;


    // Tambi�n podr�as tener m�todos dedicados a bloquear o desbloquear la c�mara
    public void LockCamera()
    {
        if (_cameraController != null)
        {
            _cameraController.IsCameraFixed = true; // Fijar la c�mara
        }
    }

    public void UnlockCamera()
    {
        if (_cameraController != null)
        {
            _cameraController.IsCameraFixed = false; // Liberar la c�mara
        }
    }
}
