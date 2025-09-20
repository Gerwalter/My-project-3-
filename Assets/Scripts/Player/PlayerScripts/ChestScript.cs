using UnityEngine;

public class ChestScript : ButtonBehaviour
{
    public int valorObjetivo = 10;
    public float radioDeteccion = 5f;
    public LayerMask capaJugador;
    public GameObject imagenUI;
    private bool jugadorEnRango = false;
    [SerializeField] private CoinSpawer spawner;

    // Cofre actualmente interactuado
    public static ChestScript cofreActivo;

    private void Start()
    {
        imagenUI.SetActive(false);
    }

    void Update()
    {
        Collider[] objetosDetectados = Physics.OverlapSphere(transform.position, radioDeteccion, capaJugador);

        bool jugadorDetectado = false;

        foreach (Collider col in objetosDetectados)
        {
            if (col.CompareTag("Player"))
            {
                jugadorDetectado = true;
                break;
            }
        }

        if (jugadorDetectado != jugadorEnRango)
        {
            jugadorEnRango = jugadorDetectado;
            imagenUI.SetActive(jugadorEnRango);
        }
    }

    private void RecibirValor(params object[] parametros)
    {
        // Solo procesar si este cofre es el activo
        if (cofreActivo != this) return;

        int valor = (int)parametros[0];

        if (valor >= valorObjetivo)
        {
            Debug.Log("El contador alcanzó o superó " + valorObjetivo);
            spawner.SpawnRandomCoins();
            EventManager.Trigger("ResetAlert");
        }
        else
        {
            Debug.Log("El contador aún no llegó a " + valorObjetivo);
            EventManager.Trigger("IncreaseAlert", 10);
        }

        // Liberar el cofre activo después de procesar
        cofreActivo = null;
        EventManager.Unsubscribe("ReceiveAlertValue", RecibirValor);
    }

    public override void OnInteract()
    {
        // Marcar este cofre como activo
        cofreActivo = this;

        // Suscribirse solo cuando interactúa
        EventManager.Subscribe("ReceiveAlertValue", RecibirValor);

        // Pedir el contador
        EventManager.Trigger("ObtainAlert");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}
