using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmbush : MonoBehaviour
{
    public float radioDeteccion = 5f;
    public LayerMask capaJugador;
    public GameObject imagenUI;
    private bool jugadorEnRango = false;

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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}
