using UnityEngine;

public class TargetMove : MonoBehaviour
{
    public CameraPointer cameraPointer;
    public float moveSpeed = 5f;
    public bool isMoving = false;

    void Update()
    {
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
            cameraPointer.TargetNull();
            isMoving = false;
        }
    }

    public void MoveTowardsTarget()
    {
        isMoving = true;
    }
}
