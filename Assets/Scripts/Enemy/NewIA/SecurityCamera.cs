using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 30f; // Velocidad de rotación en grados/segundo
    [SerializeField] private float rotationAngle = 45f; // Ángulo máximo de rotación (radio)

    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 10f; // Rango de visión
    [SerializeField, Range(0, 180)] private float visionAngle = 45f; // Ángulo del cono de visión
    [SerializeField] private LayerMask playerLayer; // Capa del jugador
    [SerializeField] private LayerMask obstacleLayer; // Capa para obstáculos

    [Header("Light Feedback")]
    [SerializeField] private Light indicatorLight; // Luz indicadora
    [SerializeField] private Color safeColor = Color.green;
    [SerializeField] private Color alertColor = Color.red;
    [SerializeField] private float lightChangeSpeed = 5f;

    [Header("Alert Settings")]
    [SerializeField] private float alertAmount = 10f; // Cuánto aumenta el nivel de alerta al detectar al jugador
    [SerializeField] private float alertCooldown = 1f; // Tiempo mínimo entre alertas consecutivas

    private float initialRotation;
    private bool rotatingRight = true;
    private bool playerDetected = false;
    private Transform detectedPlayer;
    private float lastAlertTime = -999f;

    void Start()
    {
        initialRotation = transform.eulerAngles.y;

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
        float currentRotation = transform.eulerAngles.y - initialRotation;
        if (currentRotation > 180) currentRotation -= 360;
        if (currentRotation < -180) currentRotation += 360;

        if (rotatingRight && currentRotation >= rotationAngle)
            rotatingRight = false;
        else if (!rotatingRight && currentRotation <= -rotationAngle)
            rotatingRight = true;

        float rotationStep = rotationSpeed * Time.deltaTime * (rotatingRight ? 1 : -1);
        transform.Rotate(0, rotationStep, 0);
    }

    void CheckPlayerDetection()
    {
        bool detectedThisFrame = false;
        detectedPlayer = null;

        Collider[] playersInRange = Physics.OverlapSphere(transform.position, visionRange, playerLayer);

        foreach (var playerCollider in playersInRange)
        {
            Vector3 directionToPlayer = playerCollider.transform.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= visionAngle * 0.5f)
            {
                if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer))
                {
                    detectedThisFrame = true;
                    detectedPlayer = playerCollider.transform;
                    Debug.DrawRay(transform.position, directionToPlayer, Color.red);

                    // Llamar al evento solo si no fue llamado hace poco
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

        if (!playerDetected)
        {
            Debug.DrawRay(transform.position, transform.forward * visionRange, Color.green);
        }
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
