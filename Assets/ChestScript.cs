using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : ButtonBehaviour
{
    public int valorObjetivo = 10;
    [Header("Configuración del detector")]
    public float radioDeteccion = 5f;        // Radio del OverlapSphere
    public LayerMask capaJugador;            // Para filtrar solo el jugador

    [Header("Referencia UI")]
    public GameObject imagenUI; // La imagen o panel de UI que quieres mostrar

    private bool jugadorEnRango = false;
    private void Awake()
    {
        EventManager.Subscribe("RespuestaContador", RecibirValor);
    }

    private void Start()
    {
        imagenUI.SetActive(false);
    }

    void Update()
    {
        // Detectar todos los colliders dentro de la esfera
        Collider[] objetosDetectados = Physics.OverlapSphere(transform.position, radioDeteccion, capaJugador);

        bool jugadorDetectado = false;

        foreach (Collider col in objetosDetectados)
        {
            if (col.CompareTag("Player"))
            {
                jugadorDetectado = true;
                break; // ya encontramos al jugador, no hace falta seguir
            }
        }

        // Si cambió el estado, actualizamos la UI
        if (jugadorDetectado != jugadorEnRango)
        {
            jugadorEnRango = jugadorDetectado;
            imagenUI.SetActive(jugadorEnRango);
        }
    }
    // Dibujar la esfera en el editor para debug
    private void OnDrawGizmos()
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
