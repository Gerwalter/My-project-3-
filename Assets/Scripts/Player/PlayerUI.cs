using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : HP, ILifeObservable
{
    [SerializeField] List<ILifeObserver> _observers = new List<ILifeObserver>();

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] KeyCode keyDmg;
    public void Subscribe(ILifeObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Add(x);
    }

    public void Unsubscribe(ILifeObserver x)
    {
        if (_observers.Contains(x))
            _observers.Remove(x);
    }

    private void Start()
    {
        EventManager.Subscribe("EnemyCall", PlayerCall);
        GetLife = maxLife;
    }
    public void TakeDamage(float damage)
    {
        GetLife -= damage;

        foreach (var observer in _observers)
            observer.Notify(GetLife, maxLife);

        if (GetLife <= 0)
            Debug.Log("GAME OVER");
    }
    private void Update()
    {
        if (Input.GetKeyDown(keyDmg)) //Solo para testear
            TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.U)) TakeDamage(-1);
    }

    public void PlayerCall(params object[] args)
    {
        Debug.Log("Alguien ejecuto el evento EnemyCall, con el numero = " + (int)args[0]);
    }
}
