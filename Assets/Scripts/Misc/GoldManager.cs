using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour, IGoldObservable
{
    [SerializeField] private int _totalGold;
    public static GoldManager instance;
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
        EventManager.Subscribe("IncreaseGold", IncreaseGold);
        EventManager.Subscribe("DecreaseGold", DecreaseGold);
        EventManager.Subscribe("ResetGold", ResetGold);

    }

    private void OnDestroy()
    {
        // Limpiamos suscripciones
        EventManager.Unsubscribe("IncreaseGold", IncreaseGold);
        EventManager.Unsubscribe("DecreaseGold", DecreaseGold);
        EventManager.Unsubscribe("ResetGold", ResetGold);
    }

    // ----------- Métodos internos -----------
    private void IncreaseGold(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        _totalGold += cantidad;
        Debug.Log("Oro incrementado: " + _totalGold);
        foreach (var observer in _observers)
            observer.Notify(_totalGold);
    }

    private void DecreaseGold(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        _totalGold -= cantidad;
        Debug.Log("Contador decrementado: " + _totalGold);
        foreach (var observer in _observers)
            observer.Notify(_totalGold);
    }

    private void ResetGold(params object[] parametros)
    {
        _totalGold = 0;
        foreach (var observer in _observers)
            observer.Notify(_totalGold);
        Debug.Log("Contador reiniciado.");
    }


    // Si prefieres acceso directo:
    public int ObtainValue() => _totalGold;
    [SerializeField] List<IGoldObserver> _observers = new List<IGoldObserver>();
    public void Subscribe(IGoldObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Add(x);
        foreach (var observer in _observers)
            observer.Notify(_totalGold);
    }

    public void Unsubscribe(IGoldObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Remove(x);
    }
}
