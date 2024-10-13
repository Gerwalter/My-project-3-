using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Velocidad de rotaci�n en grados por segundo
    public float rotationSpeed = 90;

    // Duraci�n del intervalo de rotaci�n en segundos
    public float rotationInterval = 2f;

    // Contador de tiempo
    private float timeCounter = 0f;

    void FixedUpdate()
    {
        // Incrementar el contador de tiempo en funci�n del tiempo transcurrido
        timeCounter += Time.fixedDeltaTime;

        // Si el contador supera el intervalo de rotaci�n, alternar la rotaci�n
        if (timeCounter >= rotationInterval)
        {
            timeCounter = 0f; // Reinicia el contador
        }

        transform.Rotate(Vector3.right * rotationSpeed * Time.fixedDeltaTime);

    }
}
