using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerDistraction : MonoBehaviour
{
    [Header("Distracciones")]
    public GameObject stonePrefab;
    public GameObject coinPrefab;
    public float throwForce = 10f;
    public Transform throwPoint;

    [Header("Trayectoria")]
    public int trajectoryPoints = 30;
    public float timeStep = 0.05f;
    public LayerMask collisionMask; // Capas con las que la trayectoria colisiona

    private LineRenderer lineRenderer;
    private GameObject currentPrefab;
    private bool isAiming = false;
    private Vector3 aimDirection;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        // Iniciar apuntado
        if (Input.GetKeyDown(KeyCode.Z))
            StartAiming(stonePrefab);
        if (Input.GetKeyDown(KeyCode.X))
            StartAiming(coinPrefab);

        // Mientras apunta
        if (isAiming)
        {
            aimDirection = throwPoint.forward;
            UpdateTrajectory();
        }

        // Soltar para lanzar
        if (isAiming && (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.X)))
        {
            ThrowDistraction(currentPrefab, aimDirection);
            StopAiming();
        }
    }

    private void StartAiming(GameObject prefab)
    {
        if (prefab == null) return;
        currentPrefab = prefab;
        isAiming = true;
        lineRenderer.positionCount = trajectoryPoints;
    }

    private void StopAiming()
    {
        isAiming = false;
        lineRenderer.positionCount = 0;
    }

    private void ThrowDistraction(GameObject prefab, Vector3 direction)
    {
        if (prefab == null) return;

        Vector3 spawnPos = throwPoint.position + direction * 0.3f;
        GameObject thrownObject = Instantiate(prefab, spawnPos, Quaternion.identity);

        Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Collider playerCollider = GetComponent<Collider>();
            Collider objCollider = thrownObject.GetComponent<Collider>();
            if (playerCollider && objCollider)
                Physics.IgnoreCollision(playerCollider, objCollider, true);

            rb.AddForce(direction.normalized * throwForce, ForceMode.Impulse);
        }
    }

    private void UpdateTrajectory()
    {
        Vector3 startPos = throwPoint.position;
        Vector3 startVel = throwPoint.forward * throwForce;
        Vector3 gravity = Physics.gravity;

        Vector3 previousPoint = startPos;
        lineRenderer.SetPosition(0, startPos);
        int pointsUsed = 1;

        for (int i = 1; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPos + startVel * t + 0.5f * gravity * t * t;

            // Comprobamos colisión entre el punto anterior y el nuevo
            if (Physics.Linecast(previousPoint, point, out RaycastHit hit, collisionMask))
            {
                lineRenderer.SetPosition(pointsUsed, hit.point);
                pointsUsed++;
                break; // Detenemos el cálculo aquí
            }

            lineRenderer.SetPosition(pointsUsed, point);
            previousPoint = point;
            pointsUsed++;
        }

        // Ajustar la cantidad real de puntos usados
        lineRenderer.positionCount = pointsUsed;
    }
}
