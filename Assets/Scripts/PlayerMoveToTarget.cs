using UnityEngine;

public class PlayerMoveToTarget : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stopDistance = 1f;
    public Transform target;
    public CameraLocker cameraFollow;
    private bool isMoving = false;
    private bool fight = false;  // Booleano para activar el modo de combate

    void Update()
    {
        // Activar o desactivar el modo de combate con la tecla F
        if (Input.GetKeyDown(KeyCode.F))
        {
            fight = !fight;
        }

        // Usar la tecla Q en lugar de la T si está en modo combate
        if ((Input.GetKeyDown(KeyCode.T) && !fight || Input.GetKeyDown(KeyCode.Q) && fight) && target != null)
        {
            isMoving = !isMoving;

            // Si hay un objetivo y el movimiento se activa, asegúrate de que la cámara esté bloqueada en el objetivo.
            if (isMoving)
            {
                cameraFollow.isLockedOnTarget = true;
            }
        }

        if (isMoving && target != null)
        {
            MoveTowardsTarget();

            if (Vector3.Distance(transform.position, target.position) <= stopDistance)
            {
                // Detén el movimiento si llegas al objetivo
                isMoving = false;
                cameraFollow.isLockedOnTarget = false;
                Invoke("resetTarget", 1f);

                // Bloquear la cámara en el objetivo si está en modo combate
                if (fight)
                {
                    cameraFollow.LockOnTarget(target);
                }
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        // Reinicia el movimiento si se establece un nuevo objetivo y la tecla T o Q se ha presionado.
        if (isMoving && target != null)
        {
            isMoving = false; // Detén cualquier movimiento anterior
            isMoving = true;  // Reactiva el movimiento con el nuevo objetivo
            cameraFollow.isLockedOnTarget = true;
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void resetTarget()
    {
        target = null;
    }
}
