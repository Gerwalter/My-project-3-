using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerDistraction : MonoBehaviour
{
    [Header("Referencias")]
    public Transform throwPoint;
    public GameObject rockPrefab;
    public GameObject coinPrefab;

    [Header("Lanzamiento")]
    public float minForce = 5f;           // Fuerza mínima al toque
    public float maxForce = 20f;          // Fuerza máxima
    public float chargeSpeed = 10f;       // Qué tan rápido aumenta la fuerza
    public float upwardForce = 2f;        // Componente vertical

    [Header("Visual de apuntado")]
    public int linePoints = 30;
    public float timeStep = 0.1f;

    private GameObject currentItem;
    private bool isAiming = false;
    private KeyCode currentKey;
    private LineRenderer lineRenderer;
    private float currentForce;
    private bool charging = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        HandleInput();

        if (isAiming)
        {
            // Aumentar fuerza mientras se mantiene presionado
            if (charging)
            {
                currentForce += chargeSpeed * Time.deltaTime;
                currentForce = Mathf.Clamp(currentForce, minForce, maxForce);
            }

            ShowTrajectory();
        }
    }

    void HandleInput()
    {
        // Iniciar apuntado con piedra
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartAiming(rockPrefab, KeyCode.Z);
        }

        // Iniciar apuntado con moneda
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartAiming(coinPrefab, KeyCode.X);
        }

        // Soltar tecla (lanzar)
        if (isAiming && Input.GetKeyUp(currentKey))
        {
            ThrowCurrentItem();
        }
    }

    void StartAiming(GameObject prefab, KeyCode key)
    {
        if (isAiming) return;

        currentItem = prefab;
        currentKey = key;
        isAiming = true;
        charging = true;
        currentForce = minForce;
        lineRenderer.enabled = true;
    }

    void ThrowCurrentItem()
    {
        if (currentItem == null || throwPoint == null)
        {
            ResetAiming();
            return;
        }

        GameObject thrown = Instantiate(currentItem, throwPoint.position, currentItem.transform.rotation);
        Rigidbody rb = thrown.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 forceDir = throwPoint.forward * currentForce + Vector3.up * upwardForce;
            rb.AddForce(forceDir, ForceMode.VelocityChange);
        }

        Debug.Log($"Lanzado {currentItem.name} con fuerza {currentForce:F2}");

        ResetAiming();
    }

    void ResetAiming()
    {
        isAiming = false;
        charging = false;
        currentItem = null;
        lineRenderer.enabled = false;
    }

    void ShowTrajectory()
    {
        Vector3 startPos = throwPoint.position;
        Vector3 startVel = throwPoint.forward * currentForce + Vector3.up * upwardForce;

        Vector3[] points = new Vector3[linePoints];

        for (int i = 0; i < linePoints; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPos + startVel * t + 0.5f * Physics.gravity * t * t;
            points[i] = point;

            if (i > 0 && Physics.Linecast(points[i - 1], points[i], out RaycastHit hit))
            {
                points[i] = hit.point;
                System.Array.Resize(ref points, i + 1);
                break;
            }
        }

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);

        // Color dinámico opcional según carga
        float tColor = (currentForce - minForce) / (maxForce - minForce);
        lineRenderer.startColor = Color.Lerp(Color.green, Color.red, tColor);
        lineRenderer.endColor = lineRenderer.startColor;
    }
}