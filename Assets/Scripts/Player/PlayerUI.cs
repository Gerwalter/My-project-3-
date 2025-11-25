using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : Health, ILifeObservable
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
        GetLife = maxHealth;
    }
    public override void OnTakeDamage(float damage)
    {
        GetLife -= damage;
        if (_bloodVFX != null)
        {
            _bloodVFX.SendEvent("OnTakeDamage"); // Activa el sistema de partículas
        }
        foreach (var observer in _observers)
            observer.Notify(GetLife, maxHealth);
        EventManager.Trigger("Input","TakeDamage");
        if (GetLife <= 0)
            Debug.Log("GAME OVE");
    }

    public void PlayerCall(params object[] args)
    {
        Debug.Log("Alguien ejecuto el evento EnemyCall, con el numero = " + (int)args[0]);
    }
}
