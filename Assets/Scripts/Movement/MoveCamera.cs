using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 OriginalOffset;

    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;
    public bool isLockedOnTarget = false;
    public Transform lockedTarget;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

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

    void LateUpdate()
    {
        if (!isLockedOnTarget)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
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
