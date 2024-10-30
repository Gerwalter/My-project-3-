using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetMove : MonoBehaviour
{
    public CameraPointer cameraPointer;
    public float moveSpeed = 5f; 
    public bool isMoving = false;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (cameraPointer.target != null)
            {
                MoveTowardsTarget();
            }
        }

        if (isMoving && cameraPointer.target != null)
        {
            MoveTowards(cameraPointer.target.transform.position);
        }
    }

    void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            isMoving = false;
            cameraPointer.isLockedOnTarget = false;
            cameraPointer.TargetNull();
        }
    }

    void MoveTowardsTarget()
    {
        isMoving = true;
    }
}
        