using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThiefPointSystem : MonoBehaviour, IPointObservable
{
    public static ThiefPointSystem instance;
    [SerializeField] private List<IPointObserver> _observers = new List<IPointObserver>();
    [SerializeField] private float _points = 0;    
    public void Subscribe(IPointObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Add(x);
      //  x.Notify(_alert, _MaxAlert);
    }
    public void Unsubscribe(IPointObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Remove(x);
    }
    private void IncreasePoints(params object[] parametros)
    {
        float cantidad = (parametros.Length > 0) ? (float)parametros[0] : 1;
        _points += cantidad;
        Debug.Log("Contador incrementado: " + _points);
        NotifyAll();
    }

    private void DecreasePoints(params object[] parametros)
    {
        float cantidad = (parametros.Length > 0) ? (float)parametros[0] : 1;
        _points -= cantidad;
        Debug.Log("Contador decrementado: " + _points);
        NotifyAll();
    }

    private void ResetPoints(params object[] parametros)
    {
        _points = 0;
        Debug.Log("Contador reiniciado.");
        NotifyAll();
    }

    private void ObtainPoints(params object[] parametros)
    {
        EventManager.Trigger("ReceiveAlertValue", _points);
    }

    private void NotifyAll()
    {
        foreach (var observer in _observers)
            observer.Notify(_points);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            EventManager.Subscribe("IncreasePoint", IncreasePoints);
            EventManager.Subscribe("DecreasePoint", DecreasePoints);
            EventManager.Subscribe("ResetPoint", ResetPoints);
            EventManager.Subscribe("ObtainPoint", ObtainPoints);
            // Limpieza automtica al cambiar de escena
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void OnDestroy()
    {
        EventManager.Subscribe("IncreasePoint", IncreasePoints);
        EventManager.Subscribe("DecreasePoint", DecreasePoints);
        EventManager.Subscribe("ResetPoint", ResetPoints);
        EventManager.Subscribe("ObtainPoint", ObtainPoints);

        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        _observers.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
