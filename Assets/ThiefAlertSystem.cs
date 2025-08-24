using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefAlertSystem : MonoBehaviour, IAlertSystemObservable
{// Instancia única accesible desde cualquier script
    public static ThiefAlertSystem instance;

    [SerializeField] private int _alert = 0;
    [SerializeField] private int _MaxAlert = 100;

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
            observer.Notify(_alert, _MaxAlert);
        // Suscribimos eventos
        EventManager.Subscribe("IncreaseAlert", IncreaseAlert);
        EventManager.Subscribe("DecreaseAlert", DecreaseAlert);
        EventManager.Subscribe("ResetAlert", ResetAlert);
        EventManager.Subscribe("ObtainAlert", ObtainAlert);
    }

    private void OnDestroy()
    {
        // Limpiamos suscripciones
        EventManager.Unsubscribe("IncreaseAlert", IncreaseAlert);
        EventManager.Unsubscribe("DecreaseAlert", DecreaseAlert);
        EventManager.Unsubscribe("ResetAlert", ResetAlert);
        EventManager.Unsubscribe("ObtainAlert", ObtainAlert);
    }

    // ----------- Métodos internos -----------
    private void IncreaseAlert(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        _alert += cantidad;
        Debug.Log("Contador incrementado: " + _alert);
        foreach (var observer in _observers)
            observer.Notify(_alert, _MaxAlert);
    }

    private void DecreaseAlert(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        _alert -= cantidad;
        Debug.Log("Contador decrementado: " + _alert);
        foreach (var observer in _observers)
            observer.Notify(_alert, _MaxAlert);
    }

    private void ResetAlert(params object[] parametros)
    {
        _alert = 0;
        foreach (var observer in _observers)
            observer.Notify(_alert, _MaxAlert);
        Debug.Log("Contador reiniciado.");
    }

    private void ObtainAlert(params object[] parametros)
    {
        EventManager.Trigger("ReceiveAlertValue", _alert);
    }

    // Si prefieres acceso directo:
    public int ObtainValue() => _alert;
    [SerializeField] List<IAlertSystemObserver> _observers = new List<IAlertSystemObserver>();
    public void Subscribe(IAlertSystemObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Add(x);
    }

    public void Unsubscribe(IAlertSystemObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Remove(x);
    }
}

