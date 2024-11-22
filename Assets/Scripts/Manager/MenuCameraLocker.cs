using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraLocker : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;


    // También podrías tener métodos dedicados a bloquear o desbloquear la cámara
    public void LockCamera()
    {
        if (_cameraController != null)
        {
            _cameraController.IsCameraFixed = true; // Fijar la cámara
        }
    }

    public void UnlockCamera()
    {
        if (_cameraController != null)
        {
            _cameraController.IsCameraFixed = false; // Liberar la cámara
        }
    }
}
