using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThiefAlertSystem : MonoBehaviour, IAlertSystemObservable
{
    public static ThiefAlertSystem instance;

    [SerializeField] private int _alert = 0;
    [SerializeField] private int _MaxAlert = 100;

    [SerializeField] private List<IAlertSystemObserver> _observers = new List<IAlertSystemObserver>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Suscribimos los eventos globales
            EventManager.Subscribe("IncreaseAlert", IncreaseAlert);
            EventManager.Subscribe("DecreaseAlert", DecreaseAlert);
            EventManager.Subscribe("ResetAlert", ResetAlert);
            EventManager.Subscribe("ObtainAlert", ObtainAlert);

            // Limpieza automática al cambiar de escena
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Notificamos a los observers iniciales (si los hay)
        foreach (var observer in _observers)
            observer.Notify(_alert, _MaxAlert);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe("IncreaseAlert", IncreaseAlert);
        EventManager.Unsubscribe("DecreaseAlert", DecreaseAlert);
        EventManager.Unsubscribe("ResetAlert", ResetAlert);
        EventManager.Unsubscribe("ObtainAlert", ObtainAlert);

        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    // Limpieza automática al cambiar de escena
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
     //   Debug.Log($"[ThiefAlertSystem] Cambió de escena ({oldScene.name} → {newScene.name}), limpiando observers...");
        _observers.Clear();
    }

    // ----------- Métodos internos -----------
    private void IncreaseAlert(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        _alert += cantidad;
        Debug.Log("Contador incrementado: " + _alert);
        NotifyAll();
    }

    private void DecreaseAlert(params object[] parametros)
    {
        int cantidad = (parametros.Length > 0) ? (int)parametros[0] : 1;
        _alert -= cantidad;
        Debug.Log("Contador decrementado: " + _alert);
        NotifyAll();
    }

    private void ResetAlert(params object[] parametros)
    {
        _alert = 0;
        Debug.Log("Contador reiniciado.");
        NotifyAll();
    }

    private void ObtainAlert(params object[] parametros)
    {
        EventManager.Trigger("ReceiveAlertValue", _alert);
    }

    private void NotifyAll()
    {
        foreach (var observer in _observers)
            observer.Notify(_alert, _MaxAlert);
    }

    // ----------- Interfaz Observable -----------
    public int ObtainValue() => _alert;

    public void Subscribe(IAlertSystemObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Add(x);
        x.Notify(_alert, _MaxAlert);
    }

    public void Unsubscribe(IAlertSystemObserver x)
    {
        if (_observers.Contains(x))
            _observers.Remove(x);
    }
}
