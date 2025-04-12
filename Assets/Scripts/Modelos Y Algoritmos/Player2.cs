using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player2 : MonoBehaviour, IObservable
{
    [SerializeField] float _maxLife;
    [SerializeField] float _life;

    List<IObserver> _observers = new List<IObserver>();

    [SerializeField] KeyCode keyDmg;

    private void Awake()
    {
        _life = _maxLife;

        EventManager.Subscribe("EnemyCall", PlayerCall);
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyDmg)) //Solo para testear
            TakeDamage(10);
    }

    public void TakeDamage(float damage)
    {
        _life -= damage;

        foreach (var observer in _observers)
            observer.Notify(_life, _maxLife);

        if (_life <= 0)
            Debug.Log("GAME OVER");
    }

    public void PlayerCall(params object[] args)
    {
        Debug.Log("Alguien ejecuto el evento EnemyCall, con el numero = " + (int)args[0]);
    }

    public void Subscribe(IObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Add(x);
    }

    public void Unsubscribe(IObserver x)
    {
        if(_observers.Contains(x)) 
            _observers.Remove(x);    
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe("EnemyCall", PlayerCall);
    }
}
