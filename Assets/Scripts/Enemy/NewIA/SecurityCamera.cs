using UnityEngine;
public enum RotationAxis
{
    None = 0,
    X = 1,
    Y = 2,
    Z = 4
}
public class SecurityCamera : MonoBehaviour
{
    [Header("Rotation Target")]
    [SerializeField] private Transform rotatingPart;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float rotationAngle = 45f;

    [Header("Axis Lock")]
    [SerializeField] private RotationAxis allowedAxes = RotationAxis.Y;
    // Por defecto solo gira en Y

    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField, Range(0, 180)] private float visionAngle = 45f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Light Feedback")]
    [SerializeField] private Light indicatorLight;
    [SerializeField] private Color safeColor = Color.green;
    [SerializeField] private Color alertColor = Color.red;
    [SerializeField] private float lightChangeSpeed = 5f;

    [Header("Alert Settings")]
    [SerializeField] private float alertAmount = 10f;
    [SerializeField] private float alertCooldown = 1f;

    private float initialRotation;
    private bool rotatingRight = true;
    private bool playerDetected = false;
    private Transform detectedPlayer;
    private float lastAlertTime = -999f;

    void Start()
    {
        if (rotatingPart == null)
            rotatingPart = transform;

        initialRotation = rotatingPart.eulerAngles.y;

        if (indicatorLight != null)
            indicatorLight.color = safeColor;
    }

    void Update()
    {
        RotateCamera();
        CheckPlayerDetection();
        UpdateLightColor();
    }

    void RotateCamera()
    {
        float currentRotation = rotatingPart.eulerAngles.y - initialRotation;
        if (currentRotation > 180) currentRotation -= 360;
        if (currentRotation < -180) currentRotation += 360;

        if (rotatingRight && currentRotation >= rotationAngle)
            rotatingRight = false;
        else if (!rotatingRight && currentRotation <= -rotationAngle)
            rotatingRight = true;

        float rotationStep = rotationSpeed * Time.deltaTime * (rotatingRight ? 1 : -1);

        Vector3 rotation = Vector3.zero;

        if ((allowedAxes & RotationAxis.X) != 0)
            rotation.x = rotationStep;

        if ((allowedAxes & RotationAxis.Y) != 0)
            rotation.y = rotationStep;

        if ((allowedAxes & RotationAxis.Z) != 0)
            rotation.z = rotationStep;

        rotatingPart.Rotate(rotation, Space.Self);
    }

    void CheckPlayerDetection()
    {
        bool detectedThisFrame = false;
        detectedPlayer = null;

        Collider[] playersInRange = Physics.OverlapSphere(rotatingPart.position, visionRange, playerLayer);

        foreach (var playerCollider in playersInRange)
        {
            Vector3 directionToPlayer = playerCollider.transform.position - rotatingPart.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            float angleToPlayer = Vector3.Angle(rotatingPart.forward, directionToPlayer);
            if (angleToPlayer <= visionAngle * 0.5f)
            {
                if (!Physics.Raycast(rotatingPart.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer))
                {
                    detectedThisFrame = true;
                    detectedPlayer = playerCollider.transform;

                    if (Time.time - lastAlertTime >= alertCooldown)
                    {
                        lastAlertTime = Time.time;
                        EventManager.Trigger("IncreaseAlert", alertAmount);
                    }
                    break;
                }
            }
        }

        playerDetected = detectedThisFrame;
    }

    void UpdateLightColor()
    {
        if (indicatorLight == null) return;

        Color targetColor = playerDetected ? alertColor : safeColor;
        indicatorLight.color = Color.Lerp(indicatorLight.color, targetColor, Time.deltaTime * lightChangeSpeed);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = playerDetected ? Color.red : Color.green;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle * 0.5f, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle * 0.5f, 0) * transform.forward * visionRange;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
        Gizmos.DrawWireSphere(transform.position, visionRange);

        if (playerDetected && detectedPlayer != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, detectedPlayer.position);
        }
    }
}


