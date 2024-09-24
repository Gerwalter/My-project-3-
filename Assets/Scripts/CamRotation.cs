using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotation : MonoBehaviour
{
    [Header("Sensibilidad")]
    [SerializeField] float _xSens;
    [SerializeField] float _ySens;
    [SerializeField] float _y;
    [SerializeField] float _z;

    // Inputs
    float _mouseX, _mouseY;

    float _xRotation, _yRotation;

    [SerializeField] Transform _playerOrientation;
    [SerializeField] Transform _meshOrientation;


    // Referencia al CameraLocker
    [SerializeField] private CameraLocker cameraLocker;

    private void Start()
    {
        // centra y oculta mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Update()
    {
        if (cameraLocker.isLockedOnTarget && cameraLocker.cameraPointer.target != null)
        {
            // Mirar al objetivo
            Transform targetTransform = cameraLocker.cameraPointer.target.transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            // Rotación normal de la cámara
            _mouseX = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * _xSens;
            _mouseY = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * _ySens;

            _yRotation += _mouseX;
            _xRotation -= _mouseY;

            _xRotation = Mathf.Clamp(_xRotation, -_y, _z);

            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        }
    }

    private void LateUpdate()
    {
        if (!cameraLocker.isLockedOnTarget)
        {
            _playerOrientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        }
    }
}
