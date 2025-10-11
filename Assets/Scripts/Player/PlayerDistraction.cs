using UnityEngine;

public class PlayerDistraction : MonoBehaviour
{
    [Header("Distracciones")]
    public GameObject stonePrefab;  // Prefab de la piedra
    public GameObject coinPrefab;   // Prefab de la moneda
    public float throwForce = 10f;  // Fuerza de lanzamiento
    public Transform throwPoint;    // Punto de origen (e.g., mano del jugador, asigna en Inspector)

    private void Update()
    {
        // Ejemplo: Q para piedra, E para moneda (usa Input System si lo tienes)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ThrowDistraction(stonePrefab);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowDistraction(coinPrefab);
        }
    }

    private void ThrowDistraction(GameObject prefab)
    {
        if (prefab == null) return;

        // Instanciar el objeto en el punto de lanzamiento
        GameObject thrownObject = Instantiate(prefab, throwPoint.position, throwPoint.rotation);

        // Aplicar fuerza física
        Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
        }
    }
}