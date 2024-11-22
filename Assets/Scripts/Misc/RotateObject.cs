using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Velocidad de rotaci�n en grados por segundo
    public float rotationSpeed = 90f;

    // Duraci�n del intervalo de rotaci�n en segundos
    public float rotationInterval = 2f;

    // Contador de tiempo
    private float timeCounter = 0f;

    // Booleanos para controlar la rotaci�n en cada eje
    public bool rotateX = true;
    public bool rotateY = false;
    public bool rotateZ = false;

    void FixedUpdate()
    {
        // Incrementar el contador de tiempo en funci�n del tiempo transcurrido
        timeCounter += Time.fixedDeltaTime;

        // Si el contador supera el intervalo de rotaci�n, alternar la rotaci�n
        if (timeCounter >= rotationInterval)
        {
            timeCounter = 0f; // Reinicia el contador
        }

        // Crear un vector para acumular la rotaci�n seg�n los ejes habilitados
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

        // Aplicar la rotaci�n acumulada al objeto
        transform.Rotate(rotation);
    }
}
