using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefAlertSystem : MonoBehaviour
{// Instancia única accesible desde cualquier script
    public static ThiefAlertSystem instance;

    [SerializeField] private int contador = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Se mantiene entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Suscribimos eventos
        EventManager.Subscribe("IncrementarContador", IncrementarHandler);
        EventManager.Subscribe("DecrementarContador", DecrementarHandler);
        EventManager.Subscribe("ReiniciarContador", ReiniciarHandler);
        EventManager.Subscribe("ObtenerContador", ObtenerHandler);
    }

    private void OnDestroy()
    {
        // Limpiamos suscripciones
        EventManager.Unsubscribe("IncrementarContador", IncrementarHandler);
        EventManager.Unsubscribe("DecrementarContador", DecrementarHandler);
        EventManager.Unsubscribe("ReiniciarContador", ReiniciarHandler);
        EventManager.Unsubscribe("ObtenerContador", ObtenerHandler);
    }

    // ----------- Métodos internos -----------
    private void IncrementarHandler(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        contador += cantidad;
        Debug.Log("Contador incrementado: " + contador);
    }

    private void DecrementarHandler(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        contador -= cantidad;
        Debug.Log("Contador decrementado: " + contador);
    }

    private void ReiniciarHandler(params object[] parametros)
    {
        contador = 0;
        Debug.Log("Contador reiniciado.");
    }

    private void ObtenerHandler(params object[] parametros)
    {
        EventManager.Trigger("RespuestaContador", contador);
    }

    // Si prefieres acceso directo:
    public int ObtenerValor() => contador;
}

