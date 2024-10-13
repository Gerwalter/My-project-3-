using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public float rotationSpeed = 90;

    // Duración del intervalo de rotación en segundos
    public float rotationInterval = 2f;

    // Contador de tiempo
    private float timeCounter = 0f;

    void FixedUpdate()
    {
        // Incrementar el contador de tiempo en función del tiempo transcurrido
        timeCounter += Time.fixedDeltaTime;

        // Si el contador supera el intervalo de rotación, alternar la rotación
        if (timeCounter >= rotationInterval)
        {
            timeCounter = 0f; // Reinicia el contador
        }

        transform.Rotate(Vector3.right * rotationSpeed * Time.fixedDeltaTime);

    }
}
