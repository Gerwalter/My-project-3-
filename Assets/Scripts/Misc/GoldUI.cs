using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour, IGoldObserver
{
    [SerializeField] private TextMeshProUGUI _goldText;
    public GameObject observable;
    public void Notify(int Gold)
    {
        _goldText.text = "Total Gold: " + Gold;
    }

    private void Start()
    {
        if (observable.GetComponent<IGoldObservable>() != null)
            observable.GetComponent<IGoldObservable>().Subscribe(this);
    }
}
