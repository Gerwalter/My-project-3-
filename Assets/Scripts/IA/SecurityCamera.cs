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
    [SerializeField] private float visionAngle = 45f; // Ángulo del cono de visión
    [SerializeField] private LayerMask playerLayer; // Capa del jugador
    [SerializeField] private LayerMask obstacleLayer; // Capa para obstáculos

    private float initialRotation;
    private bool rotatingRight = true;
    private Transform player;
    private bool playerDetected = false;

    void Start()
    {
        initialRotation = transform.eulerAngles.y;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        RotateCamera();
        CheckPlayerDetection();
    }

    void RotateCamera()
    {
        float currentRotation = transform.eulerAngles.y - initialRotation;
        float targetAngle = rotatingRight ? rotationAngle : -rotationAngle;

        // Convertir la rotación actual a un rango de -180 a 180
        if (currentRotation > 180) currentRotation -= 360;
        if (currentRotation < -180) currentRotation += 360;

        // Cambiar dirección si se alcanza el límite
        if (rotatingRight && currentRotation >= rotationAngle)
            rotatingRight = false;
        else if (!rotatingRight && currentRotation <= -rotationAngle)
            rotatingRight = true;

        // Rotar la cámara
        float rotationStep = rotationSpeed * Time.deltaTime * (rotatingRight ? 1 : -1);
        transform.Rotate(0, rotationStep, 0);
    }

    void CheckPlayerDetection()
    {
        playerDetected = false;
        Vector3 directionToPlayer = player.position - transform.position;

        // Verificar si el jugador está dentro del rango de visión
        if (directionToPlayer.magnitude <= visionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            // Verificar si el jugador está dentro del cono de visión
            if (angleToPlayer <= visionAngle * 0.5f)
            {
                // Verificar si no hay obstáculos bloqueando la visión
                if (!Physics.Raycast(transform.position, directionToPlayer, directionToPlayer.magnitude, obstacleLayer))
                {
                    playerDetected = true;
                    Debug.Log("¡Jugador detectado!");
                }
            }
        }

        // Visualización del cono de visión en el editor
        Debug.DrawRay(transform.position, transform.forward * visionRange, playerDetected ? Color.red : Color.green);
    }

    // Visualización en el editor
    void OnDrawGizmos()
    {
        Gizmos.color = playerDetected ? Color.red : Color.green;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle * 0.5f, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle * 0.5f, 0) * transform.forward * visionRange;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
