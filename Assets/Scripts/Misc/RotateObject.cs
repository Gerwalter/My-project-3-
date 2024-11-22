using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public float rotationSpeed = 90f;

    // Duración del intervalo de rotación en segundos
    public float rotationInterval = 2f;

    // Contador de tiempo
    private float timeCounter = 0f;

    // Booleanos para controlar la rotación en cada eje
    public bool rotateX = true;
    public bool rotateY = false;
    public bool rotateZ = false;

    void FixedUpdate()
    {
        // Incrementar el contador de tiempo en función del tiempo transcurrido
        timeCounter += Time.fixedDeltaTime;

        // Si el contador supera el intervalo de rotación, alternar la rotación
        if (timeCounter >= rotationInterval)
        {
            timeCounter = 0f; // Reinicia el contador
        }

        // Crear un vector para acumular la rotación según los ejes habilitados
        Vector3 rotation = Vector3.zero;

        if (rotateX)
        {
            rotation += Vector3.right * rotationSpeed * Time.fixedDeltaTime;
        }

        if (rotateY)
        {
            rotation += Vector3.up * rotationSpeed * Time.fixedDeltaTime;
        }

        if (rotateZ)
        {
            rotation += Vector3.forward * rotationSpeed * Time.fixedDeltaTime;
        }

        // Aplicar la rotación acumulada al objeto
        transform.Rotate(rotation);
    }
}
