using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLocker : MonoBehaviour
{
    public bool isLockedOnTarget = false;
    public Transform lockedTarget;
    public float smoothSpeed = 0.125f;

    void Update()
    {
        LockCamera();
    }

    private void LockCamera()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isLockedOnTarget = !isLockedOnTarget;

            // Si se desactiva isLockedOnTarget, establecer lockedTarget a null
            if (!isLockedOnTarget)
            {
                lockedTarget = null;
            }

            // Si se activa isLockedOnTarget pero no hay lockedTarget, desactivarlo al instante
            if (isLockedOnTarget && lockedTarget == null)
            {
                isLockedOnTarget = false;
            }
        }

        if (isLockedOnTarget && lockedTarget != null)
        {
            LookAtTarget(lockedTarget);
        }
    }
    public void LockOnTarget(Transform target)
    {
        lockedTarget = target;
    }

    void LookAtTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, smoothSpeed);
    }
}
