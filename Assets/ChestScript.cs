using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : ButtonBehaviour
{
    public int valorObjetivo = 10;
    [Header("Configuración del detector")]
    public float radioDeteccion = 5f;        // Radio del OverlapSphere
    public LayerMask capaJugador;            // Para filtrar solo el jugador
    private void Awake()
    {
        EventManager.Subscribe("RespuestaContador", RecibirValor);
    }
    void Update()
    {
        // Detectar todos los colliders dentro de la esfera
        Collider[] objetosDetectados = Physics.OverlapSphere(transform.position, radioDeteccion, capaJugador);

        foreach (Collider col in objetosDetectados)
        {
            if (col.CompareTag("Player")) // Verifica que el collider tenga la etiqueta "Player"
            {
                Debug.Log("Jugador detectado en el rango!");
            }
        }
    }

    // Dibujar la esfera en el editor para debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
    private void RecibirValor(params object[] parametros)
    {
        int valor = (int)parametros[0];
        Debug.Log("El valor actual es: " + valor);

        if (valor >= valorObjetivo)
        {
            Debug.Log(" El contador alcanzó o superó " + valorObjetivo);
        }
        else
        {
            Debug.Log(" El contador aún no llegó a " + valorObjetivo);
        }
    }
    public override void OnInteract()
    {
        EventManager.Trigger("ObtenerContador");
    }
}
