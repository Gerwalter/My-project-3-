using UnityEngine;

public class TargetMove : MonoBehaviour
{
    public CameraPointer cameraPointer;
    public float moveSpeed = 5f;
    public float detect = 3f;
    public bool isMoving = false;
    public float sphereRadius = 0.5f; // Radio del SphereCast
    public LayerMask layerMask; // Máscara para ignorar la capa del jugador

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
        float distance = moveSpeed * Time.deltaTime;

        // Realiza un SphereCast en la dirección del movimiento
        if (Physics.SphereCast(transform.position, sphereRadius, direction, out RaycastHit hit, distance, layerMask))
        {
            // Si el SphereCast detecta algo, detiene el movimiento
            isMoving = false;
            cameraPointer.TargetNull();
            return;
        }

        // Si no detecta nada, continúa moviéndose
        transform.position += direction * distance;

        // Verifica si está cerca del objetivo
        if (Vector3.Distance(transform.position, targetPosition) < detect)
        {
            isMoving = false;
            cameraPointer.TargetNull();
        }
    }

    public void MoveTowardsTarget()
    {
        isMoving = true;
    }

    // Dibuja el SphereCast en la dirección del movimiento
    private void OnDrawGizmos()
    {
        if (isMoving && cameraPointer.target != null)
        {
            Vector3 direction = (cameraPointer.target.transform.position - transform.position).normalized;
            float distance = moveSpeed * Time.deltaTime;

            // Configuración del color del gizmo
            Gizmos.color = Color.red;

            // Dibuja la esfera inicial
            Gizmos.DrawWireSphere(transform.position, sphereRadius);

            // Dibuja el "cilindro" entre la posición actual y el final del SphereCast
            Gizmos.DrawWireSphere(transform.position + direction * distance, sphereRadius);
            Gizmos.DrawLine(transform.position, transform.position + direction * distance);
        }
    }
}
