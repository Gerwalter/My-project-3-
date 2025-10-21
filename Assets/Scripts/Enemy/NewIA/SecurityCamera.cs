using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Movement Settins")]
    [SerializeField] private float rotationSpeed = 30f; // Velocidad de rotación en grados/segundo
    [SerializeField] private float rotationAngle = 45f; // Ángulo máximo de rotación (radio)

    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 10f; // Rango de visión
    [SerializeField, Range(0, 180)] private float visionAngle = 45f; // Ángulo del cono de visión
    [SerializeField] private LayerMask playerLayer; // Capa del jugador
    [SerializeField] private LayerMask obstacleLayer; // Capa para obstáculos

    private float initialRotation;
    private bool rotatingRight = true;
    private bool playerDetected = false;
    private Transform detectedPlayer;

    void Start()
    {
        initialRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        RotateCamera();
        CheckPlayerDetection();
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
        playerDetected = false;
        detectedPlayer = null;

        // Buscar todos los objetos en el rango de visión que estén en la capa de jugador
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, visionRange, playerLayer);

        foreach (var playerCollider in playersInRange)
        {
            Vector3 directionToPlayer = playerCollider.transform.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            // Verificar si el jugador está dentro del ángulo del cono
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= visionAngle * 0.5f)
            {
                // Verificar si no hay obstáculos entre la cámara y el jugador
                if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer))
                {
                    playerDetected = true;
                    detectedPlayer = playerCollider.transform;

                    // Debug visual
                    Debug.DrawRay(transform.position, directionToPlayer, Color.red);
                    break; // Ya detectó a un jugador, no hace falta seguir
                }
            }
        }

        if (!playerDetected)
        {
            Debug.DrawRay(transform.position, transform.forward * visionRange, Color.green);
        }
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
