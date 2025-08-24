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
        foreach (var observer in _observers)
            observer.Notify(_totalGold);
        // Suscribimos eventos
        EventManager.Subscribe("IncreaseGold", IncreaseAlert);
        EventManager.Subscribe("DecreaseGold", DecreaseAlert);
        EventManager.Subscribe("ResetGold", ResetAlert);
        EventManager.Subscribe("ObtainGold", ObtainAlert);
    }

    private void OnDestroy()
    {
        // Limpiamos suscripciones
        EventManager.Unsubscribe("IncreaseGold", IncreaseAlert);
        EventManager.Unsubscribe("DecreaseGold", DecreaseAlert);
        EventManager.Unsubscribe("ResetGold", ResetAlert);
        EventManager.Unsubscribe("ObtainGold", ObtainAlert);
    }

    // ----------- Métodos internos -----------
    private void IncreaseAlert(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        _totalGold += cantidad;
        Debug.Log("Oro incrementado: " + _totalGold);
        foreach (var observer in _observers)
            observer.Notify(_totalGold);
    }

    private void DecreaseAlert(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        _totalGold -= cantidad;
        Debug.Log("Contador decrementado: " + _totalGold);
        foreach (var observer in _observers)
            observer.Notify(_totalGold);
    }

    private void ResetAlert(params object[] parametros)
    {
        _totalGold = 0;
        foreach (var observer in _observers)
            observer.Notify(_totalGold);
        Debug.Log("Contador reiniciado.");
    }

    private void ObtainAlert(params object[] parametros)
    {
        EventManager.Trigger("ReceiveAlertValue", _totalGold);
    }

    // Si prefieres acceso directo:
    public int ObtainValue() => _totalGold;
    [SerializeField] List<IGoldObserver> _observers = new List<IGoldObserver>();
    public void Subscribe(IGoldObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Add(x);
    }

    public void Unsubscribe(IGoldObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Remove(x);
    }
}
