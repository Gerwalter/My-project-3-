using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerDistraction : MonoBehaviour
{
    [Header("Referencias")]
    public Transform throwPoint;          // Punto desde donde se lanzan los objetos
    public GameObject rockPrefab;         // Prefab de la piedra
    public GameObject coinPrefab;         // Prefab de la moneda

    [Header("Fuerza de lanzamiento")]
    public float throwForce = 10f;        // Velocidad del lanzamiento
    public float upwardForce = 2f;        // Componente vertical opcional

    private GameObject currentItem;       // Objeto que se va a lanzar
    private bool isAiming = false;        // Si est� apuntando actualmente
    private KeyCode currentKey;           // Qu� tecla se est� usando

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Presionar tecla de piedra
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartAiming(rockPrefab, KeyCode.Z);
        }

        // Presionar tecla de moneda
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartAiming(coinPrefab, KeyCode.X);
        }

        // Soltar tecla de apuntado
        if (isAiming && Input.GetKeyUp(currentKey))
        {
            ThrowCurrentItem();
        }
    }

    void StartAiming(GameObject prefab, KeyCode key)
    {
        // Si ya est� apuntando, no iniciar de nuevo
        if (isAiming) return;

        currentItem = prefab;
        currentKey = key;
        isAiming = true;

        // Aqu� podr�as activar animaciones o una ret�cula de apuntado
        Debug.Log("Apuntando con: " + prefab.name);
    }

    void ThrowCurrentItem()
    {
        if (currentItem == null || throwPoint == null)
        {
            isAiming = false;
            return;
        }

        // Instanciar el objeto
        GameObject thrown = Instantiate(currentItem, throwPoint.position, throwPoint.rotation);

        // Aplicar fuerza
        Rigidbody rb = thrown.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceDir = throwPoint.forward * throwForce + Vector3.up * upwardForce;
            rb.AddForce(forceDir, ForceMode.VelocityChange);
        }

        Debug.Log("Lanzado: " + currentItem.name);

        // Resetear estado
        isAiming = false;
        currentItem = null;
    }
}