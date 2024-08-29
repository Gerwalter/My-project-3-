using UnityEngine;

public class PlayerMoveToTarget : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stopDistance = 1f;
    public Transform target;
    public CameraFollow cameraFollow;
    private bool isMoving = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && target != null)
        {
            isMoving = !isMoving;
        }

        if (isMoving && target != null)
        {
            MoveTowardsTarget();

            if (Vector3.Distance(transform.position, target.position) <= stopDistance)
            {
                target = null;
                isMoving = false;
                cameraFollow.isLockedOnTarget = cameraFollow.isLockedOnTarget = false;
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
