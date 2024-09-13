using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraLocker : MonoBehaviour
{
    public CameraPointer cameraPointer;
    public bool isLockedOnTarget = false;

    void Update()
    {
        Locker();
    }

    private void Locker()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (cameraPointer.target != null)
            {
                isLockedOnTarget = !isLockedOnTarget;
            }
        }

        if (isLockedOnTarget && cameraPointer.target != null)
        {
            Transform targetTransform = cameraPointer.target.transform;
            transform.LookAt(targetTransform);
        }
    }
}